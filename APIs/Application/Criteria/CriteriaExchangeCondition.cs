using Application.ViewModel.PostModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Criteria
{
    public class CriteriaExchangeCondition : ICriteria
    {
        private string? ExchangeCondition;
        public CriteriaExchangeCondition(string? ExchangeCondition)
        {
            this.ExchangeCondition = ExchangeCondition;
        }
        public List<PostViewModel> MeetCriteria(List<PostViewModel> postList)
        {
            if (!ExchangeCondition.IsNullOrEmpty())
            {
                List<PostViewModel> postViewModels = new List<PostViewModel>();
                foreach (PostViewModel postViewModel in postList)
                {
                    if (postViewModel.Product?.ConditionName==ExchangeCondition)
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
