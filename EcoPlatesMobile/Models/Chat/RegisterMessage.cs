using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Chat
{
    public class RegisterMessage
    {
        public long sender_id;
        public string sender_type;
        public long receiver_id;
        public string receiver_type;
        public string receiver_phone;
        public long poster_id;
        public string content;
        public long reply_to_id;
    }
}
