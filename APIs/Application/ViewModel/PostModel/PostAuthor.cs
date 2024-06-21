using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.PostModel
{
    public class PostAuthor
    {
        public Guid AuthorId { get; set; }
        public string FulName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string HomeAddress { get; set; }
        public string AuthorImage { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public float Rating { get; set; }
    }
}
