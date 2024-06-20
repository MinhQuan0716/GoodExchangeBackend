using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User:BaseEntity
    {
        public string UserName { get; set; }    
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? HomeAddress { get; set; }
        public bool? IsBuisnessAccount { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public Guid? VerifyUserId {  get; set; }
        public VerifyUser VerifyUser { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<ChatRoom> ChatRooms1 { get; set; }
        public ICollection<ChatRoom> ChatRooms2 { get; set; }
        public ICollection<Rating> Raters {  get; set; }
        public ICollection<Rating> RatedUsers { get; set; }
    }
}
