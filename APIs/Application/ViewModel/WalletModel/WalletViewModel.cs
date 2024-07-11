using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.WalletModel
{
    public  class WalletViewModel
    {
        public Guid Id { get; set; }
        public float UserBalance { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
