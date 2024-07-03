using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.UserModel
{
    public class UserViewModelForRequest
    {
        public Guid SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderEmail { get; set; }
    }
}
