using Application.InterfaceService;
using Application.ViewModel.RequestModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class RequestService : IRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;
        public RequestService(IUnitOfWork unitOfWork, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
        }

        public async Task<List<RequestViewModel>> GetAllRequestsOfCurrentUserAsync()
        {
            return await _unitOfWork.RequestRepository.GetAllRequestByCurrentUserId(_claimService.GetCurrentUserId);
        }

        public async Task<bool> SendRequest(CreateRequestModel requestModel)
        {
           var user=await _unitOfWork.UserRepository.FindUserByEmail(requestModel.Email);
            var post = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(user.Id);
            if(!post.Where(x=>x.Id==requestModel.PostId).Any())
            {
                throw new Exception("This user do not create this post");
            }
            if (user != null)
            {
                Request request = new Request
                {
                    UserId=user.Id,
                    RequestMessage=requestModel.RequestMessage,
                    PostId=requestModel.PostId,
                    RequestStatusId=1
                };
                await _unitOfWork.RequestRepository.AddAsync(request);
            }
            return await _unitOfWork.SaveChangeAsync()>0;
        }
    }
}
