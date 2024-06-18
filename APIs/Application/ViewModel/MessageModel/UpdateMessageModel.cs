using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.MessageModel
{
    public class UpdateMessageModel
    {
        public Guid Id { get; set; }
        public string MessageContent { get; set; }
    }
}
