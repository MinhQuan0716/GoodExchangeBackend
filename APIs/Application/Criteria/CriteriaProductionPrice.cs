using Application.ViewModel.PostModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Criteria
{
    public class CriteriaProductionPrice : ICriteria
    {
        private int? ProductPrice;
        public CriteriaProductionPrice(int? ProductPrice)
        {
            this.ProductPrice = ProductPrice;
        }
        public List<PostViewModel> MeetCriteria(List<PostViewModel> postList)
        {
            if (ProductPrice != null)
            {
                List<PostViewModel> postViewModels = new List<PostViewModel>();
                foreach (PostViewModel postViewModel in postList)
                {
                    if (postViewModel.Product?.ProductPrice == ProductPrice)
                    {
                        postViewModels.Add(postViewModel);
                    }
                }
                return postViewModels;
            }
            return postList;
        }
    }
}
