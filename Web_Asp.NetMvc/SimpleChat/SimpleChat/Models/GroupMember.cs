using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class GroupMember
    {
        [Key]
        public int GroupMemberID { get; set; }
        public int GroupId { get; set; }
        public string UserEmail { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}