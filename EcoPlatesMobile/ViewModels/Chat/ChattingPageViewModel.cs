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

namespace EcoPlatesMobile.ViewModels.Chat
{
    [QueryProperty(nameof(ChatPageModel), nameof(ChatPageModel))]
    public partial class ChattingPageViewModel : ObservableObject
    {
        [ObservableProperty] private ChatPageModel chatPageModel;

        [ObservableProperty] private ObservableRangeCollection<Message> messages;
        [ObservableProperty] private Message selectedMessage;

        [ObservableProperty] private string companyName;
        [ObservableProperty] private string companyNumber;
        [ObservableProperty] private string companyImage;
        [ObservableProperty] private bool isLoading;

        private ChatWebSocketService webSocketService;
        private AppControl appControl;
        private UserSessionService userSessionService;
        private UserApiService userApiService;
        private CompanyApiService companyApiService;

        public ChattingPageViewModel(ChatWebSocketService webSocketService, AppControl appControl, UserSessionService userSessionService, UserApiService userApiService, CompanyApiService companyApiService)
        {
            this.webSocketService = webSocketService;
            this.appControl = appControl;
            this.userSessionService = userSessionService;
            this.userApiService = userApiService;
            this.companyApiService = companyApiService;
            
            this.webSocketService.OnMessageReceived += ReceivedMessage;

            messages = new ObservableRangeCollection<Message>();

            #region For test
            /*
            Message msgRec1 = new Message()
            {
                MsgType = MessageType.Receiver,
                Name = "Acdcsddc",
                Time = "18:35",
                Text = "Hey there! What\'s up?",
                BackColor = Color.FromArgb(Constants.COLOR_COMPANY)
            };
            Message msgSend1 = new Message()
            {
                MsgType = MessageType.Sender,
                Time = "15:00",
                Text = "Hello this is for the test version",
                BackColor = Color.FromArgb(Constants.COLOR_USER)
            };
            Message msgSend2 = new Message()
            {
                MsgType = MessageType.Sender,
                Time = "10:10",
                Text = "Are you there?!",
                BackColor = Color.FromArgb(Constants.COLOR_USER)
            };
            Message msgRec2 = new Message()
            {
                MsgType = MessageType.Receiver,
                Name = "Mcnjdh",
                Time = "00:08",
                Text = "Yes I am here!",
                BackColor = Color.FromArgb(Constants.COLOR_COMPANY)
            };
            Message msgSend3 = new Message()
            {
                MsgType = MessageType.Sender,
                Time = "12:23",
                Text = "Okay I thought you are not here?!",
                BackColor = Color.FromArgb(Constants.COLOR_USER)
            };

            Messages.Add(msgRec1);
            Messages.Add(msgSend1);
            Messages.Add(msgSend2);
            Messages.Add(msgRec2);
            Messages.Add(msgSend3);
            */
            #endregion
        }

        public async Task Init()
        {
            IsLoading = true;

            CompanyName = ChatPageModel.CompanyName;
            CompanyNumber = ChatPageModel.CompanyPhone;
            CompanyImage = ChatPageModel.CompanyImage;

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
                        await webSocketService.ConnectAsync();

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

            ChatMessageResponse response = await userApiService.GetHistoryMessage(request);
            if (response.resultCode == ApiResult.SUCCESS.GetCodeToString())
            {
                var items = response.resultData;

                var historyMessages = items.Select(item =>
                {
                    bool isSender = item.sender_id == ChatPageModel.SenderId && item.sender_type == ChatPageModel.SenderType;

                    return new Message
                    {
                        Text = item.content,
                        Time = item.created_at,
                        MsgType = isSender ? MessageType.Sender : MessageType.Receiver,
                        BackColor = isSender
                            ? (userSessionService.Role == UserRole.User ? Constants.COLOR_USER : Constants.COLOR_COMPANY)
                            : Colors.LightGray
                    };
                }).ToList();

                Messages.AddRange(historyMessages);
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

                ChatMessage message = new ChatMessage()
                {
                    sender_id = ChatPageModel.SenderId,
                    sender_type = ChatPageModel.SenderType,
                    receiver_id = ChatPageModel.ReceiverId,
                    receiver_type = ChatPageModel.ReceiverType,
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
                Messages.Add(new Message
                {
                    Text = msg,
                    Time = DateTime.Now.ToString("HH:mm"),
                    MsgType = MessageType.Receiver,
                    BackColor = Constants.COLOR_COMPANY
                });
            });
        }

        public async Task Disconnect()
        {
            await webSocketService.DisconnectAsync();
        }
    }
}
