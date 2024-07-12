using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.ReportModel
{
    public class ReportPostModel
    {
        public string ReportContent { get; set; }
        public Guid ReportPostId { get; set; }
    }
}
