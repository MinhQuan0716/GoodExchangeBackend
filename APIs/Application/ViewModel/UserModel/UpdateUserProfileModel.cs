using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.UserModel
{
    public class UpdateUserProfileModel
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public DateOnly Birthday { get; set; }
        public string Phonenumber { get; set; }
        public IFormFile UploadImage { get; set; }
    }
}
