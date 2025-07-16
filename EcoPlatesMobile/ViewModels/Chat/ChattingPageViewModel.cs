using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoPlatesMobile.Models.Chat;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.ViewModels.Chat
{
    public partial class ChattingPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableRangeCollection<Message> messages;
        [ObservableProperty] private Message selectedMessage;

        private ChatWebSocketService webSocketService;
        public ChattingPageViewModel(ChatWebSocketService webSocketService)
        {
            this.webSocketService = webSocketService;
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

        private void ReceivedMessage(string msg)
        { 
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Messages.Add(new Message
                {
                    Text = msg,
                    Time = DateTime.Now.ToString("HH:mm"),
                    MsgType = MessageType.Receiver,
                    BackColor = Color.FromArgb(Constants.COLOR_COMPANY)
                });
            });
        }
    }
}
