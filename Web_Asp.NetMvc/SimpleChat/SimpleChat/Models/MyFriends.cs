using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class MyFriends
    {
        [Key]
        public int Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public Status Status { get; set; }
        public DateTime Data { get; set; }


    }
    public enum Status
    {
        Pending,
        Accepted
    }

}