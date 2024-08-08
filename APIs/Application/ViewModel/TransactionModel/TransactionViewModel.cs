using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.TransactionModel
{
    public class TransactionViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Action { get; set; }
        public float Amount { get; set; }
        public DateOnly CreationDate { get; set; }
        public TimeOnly CreationTime { get; set; }
    }
}
