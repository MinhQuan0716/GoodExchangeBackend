using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.ChatRoomModel
{
    public class ChatRoomWithUserModel
    {
        public Guid Id { get; set; }
        public Guid userId { get; set; }
        public UserModel User { get; set; }
        public MessageModel Messages { get; set; }
        
    }
    public class UserModel
    {
        public string UserName { get; set; }
    }
    public class MessageModel
    {
        public string MessageContent { get; set; }
        public Guid RoomId { get; set; }
        public Guid CreatedBy { get; set; }

    }
}
