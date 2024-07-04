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

        public async Task<bool> AcceptRequest(Guid requestId)
        {
            var request = await _unitOfWork.RequestRepository.GetByIdAsync(requestId);
            if (request.RequestStatusId == 2 || request.RequestStatusId == 3)
            {
                throw new Exception("You already accept or reject this request");
            }
            request.RequestStatusId = 2;
            _unitOfWork.RequestRepository.Update(request);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<List<RequestViewModel>> GetAllRequestsOfCurrentUserAsync()
        {
            return await _unitOfWork.RequestRepository.GetAllRequestByCurrentUserId(_claimService.GetCurrentUserId);
        }

        public async Task<bool> RejectRequest(Guid requestId)
        {
            var request = await _unitOfWork.RequestRepository.GetByIdAsync(requestId);
            if (request.RequestStatusId == 2 || request.RequestStatusId == 3)
            {
                throw new Exception("You already accept or reject this request");
            }
            request.RequestStatusId = 3;
            _unitOfWork.RequestRepository.Update(request);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> SendRequest(CreateRequestModel requestModel)
        {
            var post = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(requestModel.AuthorId);
            if (!post.Where(x => x.Id == requestModel.PostId).Any())
            {
                throw new Exception("This user do not create this post");
            }
            var duplicateRequest = await _unitOfWork.RequestRepository.GetRequestByUserIdAndPostId(requestModel.AuthorId, requestModel.PostId);
            if (duplicateRequest != null)
            {
                throw new Exception("You already send the request for this post");
            }
            Request request = new Request
            {
                UserId = requestModel.AuthorId,
                RequestMessage = requestModel.RequestMessage,
                PostId = requestModel.PostId,
                RequestStatusId = 1
            };
            await _unitOfWork.RequestRepository.AddAsync(request);

            return await _unitOfWork.SaveChangeAsync() > 0;
        }
    }
}
