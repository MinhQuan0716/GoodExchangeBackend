using Application.ViewModel.PostModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Criteria
{
    public class CriteriaProductStatus : ICriteria
    {
        private string? ProductStatus;
        public CriteriaProductStatus(string? ProductStatus)
        {
            this.ProductStatus = ProductStatus;
        }
        public List<PostViewModel> MeetCriteria(List<PostViewModel> postList)
        {
            if (!ProductStatus.IsNullOrEmpty())
            {
                List<PostViewModel> postDetailViewModels = new List<PostViewModel>();
                foreach (var postViewModel in postList)
                {
                    if (postViewModel.Product.ProductStatus == ProductStatus)
                    {
                        postDetailViewModels.Add(postViewModel);
                    }
                }
                return postDetailViewModels;
            }
            return postList;
        }
    }
}
