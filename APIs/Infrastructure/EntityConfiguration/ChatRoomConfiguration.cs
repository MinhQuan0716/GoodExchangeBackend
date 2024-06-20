using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityConfiguration
{
    internal class ChatRoomConfiguration:IEntityTypeConfiguration<ChatRoom>
    {
        public void Configure(EntityTypeBuilder<ChatRoom> builder)
        {
            builder.HasOne(x => x.Sender).WithMany(u => u.ChatRooms1).HasForeignKey(x => x.SenderId).OnDelete(deleteBehavior:DeleteBehavior.NoAction);
            builder.HasOne(x => x.Receiver).WithMany(u => u.ChatRooms2).HasForeignKey(x => x.ReceiverId).OnDelete(deleteBehavior: DeleteBehavior.NoAction);
        }
    }
}
