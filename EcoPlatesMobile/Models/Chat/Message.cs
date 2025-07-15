using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Chat
{
    public enum MessageType
    {
        Sender,
        Receiver
    }
    public class Message : IEquatable<Message>
    {
        public MessageType MsgType { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Time { get; set; }
        public Color BackColor { get; set; }

        public bool Equals(Message other)
        {
            return MsgType == other.MsgType &&
                   Name == other.Name &&
                   Text == other.Text &&
                   Time == other.Time;
        }
    }
}
