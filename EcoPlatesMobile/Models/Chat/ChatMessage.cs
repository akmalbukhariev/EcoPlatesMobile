using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoPlatesMobile.Converters;

namespace EcoPlatesMobile.Models.Chat
{
    public class ChatMessage
    {
        public long message_id;
        public long sender_id;
        public string sender_type;
        public long receiver_id;
        public string receiver_type;
        public long poster_id;
        public String content;
        public long reply_to_id;
        [JsonProperty("created_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime created_at { get; set; }
        public bool is_deleted_by_sender;
        public bool is_deleted_by_receiver;
    }
}
