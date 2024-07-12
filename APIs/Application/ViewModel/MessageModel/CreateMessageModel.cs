using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.MessageModel
{
    public class CreateMessageModel
    {
        public string MessageContent { get; set; }
        public Guid RoomId { get; set; }
        public Guid CreatedBy { get; set; }
    }
}
