using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<VerifyUser> VerifyUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ExchangeCondition> ExchangeConditions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcription> Subcriptions { get; set; }
        public DbSet<SubcriptionHistory> SubcriptionHistories { get; set; } 
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<VerificationStatus> VerificationStatuses { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
