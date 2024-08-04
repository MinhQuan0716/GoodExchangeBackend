using Application.Util;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntityConfiguration
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(x => x.Email).IsUnique(true);
            builder.HasData(new User
            {
                Id = Guid.Parse("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"),
                UserName = "Admin",
                Email = "admin@gmail.com",
                PasswordHash = new string("Admin@123").Hash(),
                RoleId = 1,
                WalletId = Guid.Empty,
                VerifyUserId = Guid.Empty,
                IsDelete = false,
            },
            new User
            {
                Id = Guid.Parse("319d5597-f149-4fa5-9c05-60e4f7120b8f"),
                UserName = "Moderator",
                Email = "moderator@gmail.com",
                PasswordHash = new string("Moderator").Hash(),
                RoleId = 2,
                WalletId = Guid.Empty,
                VerifyUserId = Guid.Empty,
                IsDelete = false,
            }
            );
        }
    }
}
