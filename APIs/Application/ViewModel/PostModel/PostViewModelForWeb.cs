using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.PostModel
{
    public class PostViewModelForWeb
    {
        public Guid Id { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public DateOnly CreationDate { get; set; }
        public string Status { get; set; }
    }
}
