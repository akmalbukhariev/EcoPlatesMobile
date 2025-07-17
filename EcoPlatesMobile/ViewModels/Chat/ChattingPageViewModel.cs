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

namespace EcoPlatesMobile.ViewModels.Chat
{
    public partial class ChattingPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<Message> messages;
        [ObservableProperty] private Message selectedMessage;
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
                    }
                    else
                    {
                        Console.WriteLine("Token is missing. Cannot connect WebSocket.");
                    }
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

        public async Task<bool> SendMessage(string msg)
        {
            try
            {
                if (webSocketService.State != WebSocketState.Open)
                {
                    await webSocketService.ConnectAsync();
                }

                await webSocketService.SendMessageAsync(msg);

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
    }
}
