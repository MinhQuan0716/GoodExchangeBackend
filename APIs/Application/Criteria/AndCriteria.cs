using Application.ViewModel.PostModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Criteria
{
    public class AndCriteria : ICriteria
    {
        private ICriteria criteria;
        private ICriteria otherCriteria;
        public AndCriteria(ICriteria criteria, ICriteria otherCriteria)
        {
            this.criteria = criteria;
            this.otherCriteria = otherCriteria;
        }

        public List<PostViewModel> MeetCriteria(List<PostViewModel> postList)
        {
           List<PostViewModel> findPostViewModel=criteria.MeetCriteria(postList);
            return otherCriteria.MeetCriteria(findPostViewModel);
        }
    }
}
