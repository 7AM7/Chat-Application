using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class GroupDetails
    {
        [Key]
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        [NotMapped]
        public HttpPostedFileBase GroupImageFile { get; set; }
        public string GroupImage { get; set; }
        public string CreatorEmail { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}