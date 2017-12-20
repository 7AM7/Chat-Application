using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class ChatPrivateMessageDetail
    {
        public int ID { get; set; }
        public string MasterEmailID { get; set; }
        public string ChatToEmailID { get; set; }
        public string Message { get; set; }
        //public string Image { get; set; }
        public DateTime MessageData { get; set; }
    }
}