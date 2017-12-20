using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimpleChat.Models
{
    public class UserModels
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "* First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "* Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "* Gender is required")]
        [Display(Name = "Gender")]
        public Gender Gander { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "BirthDay")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDay { get; set; }
        [Display(Name = "Profile Image")]
        public string ProfileImage { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ConnectionId { get; set; }

    }
    public enum Gender
    {
        Male,
        Female
    }
}