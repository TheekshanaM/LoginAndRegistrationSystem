using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginAndRegistrationSystem.Models
{
    [MetadataType(typeof(UserMetaData))]
    public partial class User
    {
        public string confirmPassword { get; set; }
    }

    public class UserMetaData
    {
        [Display(Name ="First Name")]
        [Required(AllowEmptyStrings = false , ErrorMessage ="First Name Required !")]    
        public string firstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name Required !")]
        public string lastName { get; set; }

        [Display(Name = "Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Address Required !")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Required(AllowEmptyStrings =false , ErrorMessage ="password is Required !")]
        [DataType(DataType.Password)]
        [MinLength(4 , ErrorMessage ="Required atleast 4 characters !")]
        public string password { get; set; }

        [Display(Name ="Confirm Password !")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage ="Passwords are mismatch !")]
        public string confirmPassword { get; set; }

    }
}