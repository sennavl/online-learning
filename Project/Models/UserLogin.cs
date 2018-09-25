using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                            + "@"
                            + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$",
        ErrorMessage = "Only valid email allowed")]
        public string Email { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Password { get; set; }

        public UserLogin()
        {

        }

        public UserLogin(string Email, string Password)
        {
            this.Email = Email;
            this.Password = Password;
        }
    }
}