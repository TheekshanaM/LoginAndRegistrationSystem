using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginAndRegistrationSystem.Models
{
    public class UserLogin
    {
        [Display(Name ="Email Id")]
        [Required(AllowEmptyStrings = false , ErrorMessage ="Email Address Required !")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password Required !")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Display(Name = "Remember me")]
        public Boolean RememberMe { get; set; }
    }
}