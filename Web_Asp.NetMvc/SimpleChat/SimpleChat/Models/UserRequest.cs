using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    [NotMapped]
    public class UserRequest : UserModels
    {
        [NotMapped]
        public byte[] ImageArray { get; set; }
    }
}