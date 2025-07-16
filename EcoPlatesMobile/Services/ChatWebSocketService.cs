using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class ChatWebSocketService
{
   private ClientWebSocket _webSocket;
    private readonly string _token;
    private readonly string _baseUri;

    public event Action<string> OnMessageReceived;

    public WebSocketState State => _webSocket?.State ?? WebSocketState.None;
  
    private string WebSocketUri = ""; 

    public ChatWebSocketService()
    {
        _token = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiI5OTg5OTg4ODc3NjYiLCJhdXRoIjoiUk9MRV9VU0VSIiwiZXhwIjoxNzg0MTEzNTcxfQ.vkgecG0GX_IHgWswD6U0dAfy1m7i7xLs0tOHSFT8_SY";
        _baseUri = "ws://192.168.219.132:8085/ecoplateschatting/api/v1/chat-ws";
    }

    public async Task ConnectAsync()
    { 
        if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            return;

        var uri = new Uri($"{_baseUri}?token={_token}");
        _webSocket = new ClientWebSocket();

        await _webSocket.ConnectAsync(uri, CancellationToken.None);
        Console.WriteLine("✅ Connected to WebSocket");

        _ = ReceiveLoop(); // Run in background
    }

    public async Task SendMessageAsync(string message)
    {
        if (_webSocket?.State != WebSocketState.Open)
            throw new InvalidOperationException("WebSocket is not connected.");

        var buffer = Encoding.UTF8.GetBytes(message);
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
