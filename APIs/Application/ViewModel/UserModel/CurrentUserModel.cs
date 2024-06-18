using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.UserModel
{
    public class CurrentUserModel
    {
        public Guid Userid {  get; set; }
        public string? Username { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string?Phonenumber { get; set; } 
        public DateOnly?Birthday { get; set; }
        
    }
}
