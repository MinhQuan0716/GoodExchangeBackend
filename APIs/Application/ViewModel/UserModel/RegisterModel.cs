using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.UserViewModel
{
    public class RegisterModel
    {
        public string Username { get; set; }
       // [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Birthday { get; set; }
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Please enter valid phone number.")]
        public string Phonenumber { get; set; }
        public string Homeaddress { get; set; }
        public string? ProfileImage { get; set; }
        public string code { get; set; }
    }
}
