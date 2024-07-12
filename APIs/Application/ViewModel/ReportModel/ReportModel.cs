using Application.ViewModel.PostModel;
using Application.ViewModel.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.ReportModel
{
    public class ReportModel
    {
        public string ReportContent { get; set; }
        public Guid? ReportUserId { get; set; }
        public Guid? ReportPostId { get; set; }
        public UserViewModelForReport? user { get; set; }
        public PostDetailViewModelForReport? post { get; set; }
    }
    public class UserViewModelForReport
    {
        public Guid userId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public string HomeAddress { get; set; }
    }
    public class PostDetailViewModelForReport
    {
        public Guid PostId { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public string ProductImageUrl { get; set; }
        public long ProductPrice { get; set; }
        public string ProductStatus { get; set; }
        public string RequestedProduct { get; set; }
        public int? ProductQuantity { get; set; }
        public int ConditionTypeId { get; set; }
        public string ConditionTypeName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
