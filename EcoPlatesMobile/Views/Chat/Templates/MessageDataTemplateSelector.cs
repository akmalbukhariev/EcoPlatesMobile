using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Views.Chat.Templates
{
    internal class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SenderMessageTemplate { get; set; }
        public DataTemplate ReceiverMessageTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = (Models.Chat.Message)item;

            if (message.MsgType == Models.Chat.MessageType.Receiver)
                return ReceiverMessageTemplate;

            return SenderMessageTemplate;
        }
    }
}
