using Application.ViewModel.PostModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Criteria
{
    public class OrCriteria : ICriteria
    {
        private ICriteria criteria;
        private ICriteria otherCriteria;
        public OrCriteria(ICriteria criteria, ICriteria otherCriteria)
        {
            this.criteria = criteria;
            this.otherCriteria = otherCriteria;
        }

        public List<PostViewModel> MeetCriteria(List<PostViewModel> postList)
        {
            List<PostViewModel> findFirstPostCriteria=criteria.MeetCriteria(postList);
            List<PostViewModel> findSecondPostCriteria=criteria.MeetCriteria(postList);
            foreach(PostViewModel post in findSecondPostCriteria)
            {
                if (!findFirstPostCriteria.Contains(post))
                {
                    findFirstPostCriteria.Add(post);
                }
            }
            return findFirstPostCriteria;
        }
    }
}
