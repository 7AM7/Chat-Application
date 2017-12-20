using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class GroupMessage
    {
        [Key]
        public int ID { get; set; }
        public int GroupID { get; set; }
        public string SenderEmail { get; set; }
        public string Message { get; set; }
        public DateTime MessageData { get; set; }
    }
}