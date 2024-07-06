using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.UserModel
{
    public class CurrentLoginUserForWebViewModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
