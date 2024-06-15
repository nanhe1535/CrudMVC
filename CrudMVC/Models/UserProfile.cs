using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CrudMVC.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string ProfilePhotoPath { get; set; }
    }
}