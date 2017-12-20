using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class ChatUserDetail
    {
        public int ID { get; set; }
        public string AuthId { get; set; }
        public string ConnectionId { get; set; }
        public string FullName { get; set; }
        public string EmailID { get; set; }
    }
}