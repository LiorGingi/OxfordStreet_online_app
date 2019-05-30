using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OxfordStreet_online_app.Models
{
    public class User
    {
        public int UserId { get; set; }

        [MinLength(3)]
        [MaxLength(10)]
        [Required]
        public string FirstName { get; set; }

        [MinLength(3)]
        [MaxLength(15)]
        [Required]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool IsEmployee { get; set; }
        public string Password { get; set; } //need to add password encrypt and decrypt
    }
}