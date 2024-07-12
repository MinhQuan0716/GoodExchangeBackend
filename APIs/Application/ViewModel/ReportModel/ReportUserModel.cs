using Application.ViewModel.UserModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.ReportModel
{
    public class ReportUserModel
    {
        public string ReportContent { get; set; }
        public Guid ReportUserId { get; set; }
    }
}
