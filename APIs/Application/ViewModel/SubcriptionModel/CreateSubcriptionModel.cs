using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.SubcriptionModel
{
    public class CreateSubcriptionModel
    {
        public long Price { get; set; }
        public bool IsPriority { get; set; }
        public string SubcriptionType { get; set; }
        public float ExpiryMonth { get; set; }
    }
}
