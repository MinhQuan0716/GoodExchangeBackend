using Application.ViewModel.RequestModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface IRequestRepository:IGenericRepository<Request>
    {
        Task<List<ReceiveRequestViewModel>> GetAllRequestByCurrentUserId(Guid userId);
        Task<List<SentRequestViewModel>> GetAllRequestByCreatedByUserId(Guid userId);
        Task<List<Request>> GetRequestByUserIdAndPostId(Guid userId, Guid postId);
    }
}
