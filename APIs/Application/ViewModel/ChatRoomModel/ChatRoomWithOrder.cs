using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.ChatRoomModel
{
    public class ChatRoomWithOrder
    {
        public Guid roomId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public Guid ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string SenderAvatar { get; set; }  // Add this line
        public string ReceiverAvatar { get; set; }
        // Other necessary properties
        public List<MessageDto> Messages { get; set; }
        public OrderDto Order { get; set; }
    }
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public int OrderStatusId { get; set; }
    }
}
