using Application.InterfaceService;
using Application.ViewModel.RequestModel;
using AutoMapper;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimService _claimService;
        private readonly IMapper _mapper;
        public OrderService(IUnitOfWork unitOfWork, IClaimService claimService,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _claimService = claimService;
            _mapper = mapper;
        }

        public async Task<bool> AcceptRequest(Guid requestId)
        {
            var request = await _unitOfWork.OrderRepository.GetByIdAsync(requestId);
            if (request == null)
            {
                throw new Exception("Request not found");
            }

            if (request.OrderStatusId == 2 || request.OrderStatusId == 3)
            {
                throw new Exception("You already accepted or rejected this order");
            }

            // Update the request status
            request.OrderStatusId = 2;
            _unitOfWork.OrderRepository.Update(request);

            // Get all other requests by post ID and update their statuses to rejected
            var rejectOrders = await _unitOfWork.OrderRepository.GetRequestByPostId(request.PostId);
            if (rejectOrders != null && rejectOrders.Any())
            {
                // Update their statuses to rejected
                foreach (var item in rejectOrders)
                {
                    if (item.Id != request.Id)  // Skip the already updated request
                    {
                        item.OrderStatusId = 3;
                        _unitOfWork.OrderRepository.Update(item);
                    }
                }
            }

            // Save all changes
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
        public async Task<bool> DeliveredOrder(Guid orderId)
        {
            var request = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (request == null)
            {
                throw new Exception("Order not found");
            }

            if (request.OrderStatusId != 2)
            {
                throw new Exception("Order not accepted");
            }

            // Update the request status
            request.OrderStatusId = 4;
            _unitOfWork.OrderRepository.Update(request);
            // Save all changes
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<List<SentOrderViewModel>> GetAllRequestsOfCreatebByUserAsync()
        {
            return await _unitOfWork.OrderRepository.GetAllRequestByCreatedByUserId(_claimService.GetCurrentUserId);
        }

        public async Task<List<ReceiveOrderViewModel>> GetAllRequestsOfCurrentUserAsync()
        {
            return await _unitOfWork.OrderRepository.GetAllRequestByCurrentUserId(_claimService.GetCurrentUserId);
        }
        public async Task<bool> SendRequest(CreateOrderModel requestModel)
        {
            var post = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(requestModel.AuthorId);
            if (!post.Where(x => x.Id == requestModel.PostId).Any())
            {
                throw new Exception("This user do not create this post");
            }
            var duplicateRequest = await _unitOfWork.OrderRepository.GetRequestByUserIdAndPostId(requestModel.AuthorId,requestModel.PostId);
            if (duplicateRequest.Where(x=>x.CreatedBy==_claimService.GetCurrentUserId).Any())
            {
                throw new Exception("You already send the request");
            }
            Order request =_mapper.Map<Order>(requestModel);
            request.OrderStatusId = 1;
            await _unitOfWork.OrderRepository.AddAsync(request);

            return await _unitOfWork.SaveChangeAsync() > 0;
        }
        public async Task<bool> CheckOrderStatusByPostId(Guid postId)
        {
            var OrderList = await _unitOfWork.OrderRepository.GetRequestByPostId(postId);
            foreach(var order in OrderList)
            {
                if (order.OrderStatusId == 2)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<ReceiveOrderViewModel> GetOrderDetailAsync(Guid orderId)
        {
            return await _unitOfWork.OrderRepository.GetOrderDetail(orderId);
        }
    }
}
