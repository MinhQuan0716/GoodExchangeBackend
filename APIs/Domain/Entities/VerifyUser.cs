﻿using System;
using System.Collections.Generic;
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
        public bool IsStudentAccount { get; set; }
    }
}
