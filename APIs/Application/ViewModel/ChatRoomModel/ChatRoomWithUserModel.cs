using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.ChatRoomModel
{
    public class ChatRoomDto
    {
        public Guid roomId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public Guid ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        // Other necessary properties

        public List<MessageDto> Messages { get; set; }
    }

    public class MessageDto
    {
        public Guid messageId { get; set; }
        public string Content { get; set; }
        public Guid? CreatedBy { get; set; }
        public string CreatedByUserName { get; set; }
        public string? CreatedDate { get; set;}
        public string? CreatedTime { get; set; }
        // Other necessary properties
    }
}
