using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.RequestModel
{
    public class CreateRequestModel
    {
        public Guid PostId { get; set; }
        public string RequestMessage { get; set; }
        public string Email { get; set; }
    }
}
