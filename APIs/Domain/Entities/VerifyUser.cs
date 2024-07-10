using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class VerifyUser:BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string? UserImage { get; set; }
        public int VerifyStatusId { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
    }
}
