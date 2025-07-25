using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoPlatesMobile.Models.Chat;
using EcoPlatesMobile.Utilities;
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Services.Api;
using System.Net.WebSockets;
using EcoPlatesMobile.Models.Requests.Chat;
using EcoPlatesMobile.Models.Responses.Chat;
using Newtonsoft.Json;

namespace EcoPlatesMobile.ViewModels.Chat
{
    [QueryProperty(nameof(ChatPageModel), nameof(ChatPageModel))]
    public partial class ChattingPageViewModel : ObservableObject
    {
        [ObservableProperty] private ChatPageModel chatPageModel;

        [ObservableProperty] private ObservableRangeCollection<Message> messages;
        [ObservableProperty] private Message selectedMessage;

        [ObservableProperty] private string receiverName;
        [ObservableProperty] private string receiverNumber;
        [ObservableProperty] private string receiverImage;
        [ObservableProperty] private bool isLoading;

        private ChatWebSocketService webSocketService;
        private AppControl appControl;
        private UserSessionService userSessionService;
        private UserApiService userApiService;
        private CompanyApiService companyApiService;
        private ChatApiService chatApiService;

        public event EventHandler? ScrollToBottomRequested;

        public ChattingPageViewModel(ChatWebSocketService webSocketService, AppControl appControl, UserSessionService userSessionService, UserApiService userApiService, CompanyApiService companyApiService, ChatApiService chatApiService)
        {
            this.webSocketService = webSocketService;
            this.appControl = appControl;
            this.userSessionService = userSessionService;
            this.userApiService = userApiService;
            this.companyApiService = companyApiService;
            this.chatApiService = chatApiService;
            
            this.webSocketService.OnMessageReceived += ReceivedMessage;

            messages = new ObservableRangeCollection<Message>();
        }

        public async Task Init()
        {
            IsLoading = true;

            ReceiverName = ChatPageModel.ReceiverName;
            ReceiverNumber = ChatPageModel.ReceiverPhone;
            ReceiverImage = ChatPageModel.ReceiverImage;

            try
            {
                if (webSocketService.State != WebSocketState.Open)
                {
                    string? token = null;
                    if (userSessionService.Role == UserRole.User)
                    {
                        token = await userApiService.GetTokenAsync();
                    }
                    else
                    {
                        token = await companyApiService.GetTokenAsync();
                    }

                    if (!string.IsNullOrEmpty(token))
                    {
                        webSocketService.SetToken(token);
                        //await webSocketService.ConnectAsync();

                        await LoadHistoryMessage();
                    }
                    else
                    {
                        Console.WriteLine("Token is missing. Cannot connect WebSocket.");
                    }
                }
                else
                { 
                    await LoadHistoryMessage();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket connection is error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadHistoryMessage()
        {
            ChatMessageRequest request = new ChatMessageRequest()
            {
                sender_id = ChatPageModel.SenderId,
                sender_type = ChatPageModel.SenderType,
                receiver_id = ChatPageModel.ReceiverId,
                receiver_type = ChatPageModel.ReceiverType,
            };

            ChatMessageResponse response = await chatApiService.GetHistoryMessage(request);
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                var items = response.resultData;

                var historyMessages = items.Select(item =>
                {
                    bool isSender = item.sender_id == ChatPageModel.SenderId && item.sender_type == ChatPageModel.SenderType;

                    return new Message
                    {
                        Time = DateTime.TryParse(item.created_at, out var dt)
                            ? dt.ToString("HH:mm")
                            : item.created_at,
                        Text = item.content,
                        MsgType = isSender ? MessageType.Sender : MessageType.Receiver,
                        BackColor = item.sender_type == UserRole.Company.ToString().ToUpper() ? Constants.COLOR_COMPANY : Constants.COLOR_USER,
                    };
                }).ToList();

                Messages.Clear();
                Messages.AddRange(historyMessages);

                ScrollToBottomRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task<bool> SendMessage(string msg)
        {
            try
            {
                if (webSocketService.State != WebSocketState.Open)
                {
                    await webSocketService.ConnectAsync();
                }

                RegisterMessage message = new RegisterMessage()
                {
                    sender_id = ChatPageModel.SenderId,
                    sender_type = ChatPageModel.SenderType,
                    receiver_id = ChatPageModel.ReceiverId,
                    receiver_type = ChatPageModel.ReceiverType,
                    receiver_phone = ChatPageModel.ReceiverPhone,
                    poster_id = ChatPageModel.PosterId,
                    reply_to_id = 0,
                    content = msg
                };

                await webSocketService.SendMessageAsync(message);

                Messages.Add(new Message
                {
                    Text = msg,
                    Time = DateTime.Now.ToString("HH:mm"),
                    MsgType = MessageType.Sender,
                    BackColor = userSessionService.Role == UserRole.User ? Constants.COLOR_USER : Constants.COLOR_COMPANY
                });

                ScrollToBottomRequested?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send failed: {ex.Message}");
            }

            return false;
        }

        private void ReceivedMessage(string msg)
        { 
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RegisterMessage body = JsonConvert.DeserializeObject<RegisterMessage>(msg);

                Messages.Add(new Message
                {
                    Text = body.content,
                    Time = DateTime.Now.ToString("HH:mm"),
                    MsgType = MessageType.Receiver,
                    BackColor = body.sender_type == UserRole.User.ToString().ToUpper() ? Constants.COLOR_USER : Constants.COLOR_COMPANY
                });

                ScrollToBottomRequested?.Invoke(this, EventArgs.Empty);
            });
        }

        public async Task Disconnect()
        {
            await webSocketService.DisconnectAsync();
        }
    }
}
