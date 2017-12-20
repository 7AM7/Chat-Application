using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class ChatAllMessageDetail
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string EmailID { get; set; }
    }
}