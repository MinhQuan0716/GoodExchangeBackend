using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatRoom:BaseEntity
    {
        public Guid SenderId { get; set; }
        public User Sender {  get; set; }
        public Guid ReceiverId { get; set;}
        public User Receiver { get; set;}
        public ICollection<Message> Messages { get; set;}
    }
}
