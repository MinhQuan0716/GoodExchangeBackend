using Application.ViewModel.PostModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Criteria
{
    public interface ICriteria
    {
        List<PostViewModel> MeetCriteria(List<PostViewModel> postList); 
    }
}
