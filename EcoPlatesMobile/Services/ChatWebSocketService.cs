using EcoPlatesMobile.Models.Chat;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EcoPlatesMobile.Utilities;

public class ChatWebSocketService
{
   private ClientWebSocket _webSocket;
    private string _token;
    private readonly string _baseUri;

    public event Action<string> OnMessageReceived;

    public WebSocketState State => _webSocket?.State ?? WebSocketState.None;

    public ChatWebSocketService()
    {
        //_baseUri = "ws://10.0.2.2:8085/ecoplateschatting/api/v1/chat-ws";
        //_baseUri = $"ws://{Constants.IP}:8085/ecoplateschatting/api/v1/chat-ws"; //local pc
        _baseUri = "ws://www.ecoplates.uz:8080/chatting/chat-ws";
    }

    public void SetToken(string token)
    {
        _token = token;
    }

    public async Task ConnectAsync()
    { 
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            return;

        var uri = new Uri($"{_baseUri}?token={_token}");
        _webSocket = new ClientWebSocket();

        await _webSocket.ConnectAsync(uri, CancellationToken.None);
        Console.WriteLine("Connected to WebSocket");

        _ = ReceiveLoop(); // Run in background
    }

    public async Task SendMessageAsync(RegisterMessage message)
    {
        if (_webSocket?.State != WebSocketState.Open)
        {
            Console.WriteLine("WebSocket is not connected. Skipping send.");
            return;
        }

        var json = JsonConvert.SerializeObject(
        message,
        new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        });
        var buffer = Encoding.UTF8.GetBytes(json);

        await _webSocket.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[4096];

        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
            }
            else
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count); 
                
                OnMessageReceived?.Invoke(message);
            }
        }
    }

    public async Task DisconnectAsync()
    {
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
            _webSocket.Dispose();
            _webSocket = null;
        }
    }
}
