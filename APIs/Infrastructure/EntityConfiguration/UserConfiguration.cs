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
                Id = Guid.NewGuid(),
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
                Id = Guid.NewGuid(),
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
