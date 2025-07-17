using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.Chat
{
    public class ChatMessageRequest
    {
        public long sender_id { get; set; }
        public string sender_type { get; set; }
        public long receiver_id { get; set; }
        public string receiver_type { get; set; }
    }
}
