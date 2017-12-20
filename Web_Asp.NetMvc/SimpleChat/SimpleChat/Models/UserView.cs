using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class UserView : UserModels
    {
        [Display(Name = "Image")]
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }
        [NotMapped]
        public string ImageTitle { get; set; }
        [NotMapped]
        public string ImagePath { get; set; }
        [NotMapped]
        public byte[] ImageByte { get; set; }
    }
}