using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.ProductModel
{
    public class RequestProductViewModel
    {
        public Guid RequestProductId { get; set; }
        public string RequestProductName { get; set; }
        public string RequestProductDescription { get; set;}
    }
}
