using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.PostModel
{
    public class PostAuthor
    {
        public string Fulname { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public double Rating { get; set; }
    }
}
