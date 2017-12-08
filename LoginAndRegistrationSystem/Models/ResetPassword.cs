using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace LoginAndRegistrationSystem.Models
{
    public class ResetPassword
    {
        [Required(ErrorMessage ="password is required !",AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string newPasswoed { get; set; }

        [DataType(DataType.Password)]
        [Compare("newPasswoed", ErrorMessage = "password mismathch !")]
        public string confirmpassword { get; set; }

        [Required]
        public string resetCode { get; set; }
    }
}