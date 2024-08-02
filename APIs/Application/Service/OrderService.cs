using Application.InterfaceService;
using Application.ViewModel.RequestModel;
using AutoMapper;
using Domain.Entities;
using Hangfire.Dashboard;
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
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var order = await _unitOfWork.OrderRepository.GetByIdAsync(requestId);
                    if (order == null)
                    {
                        throw new Exception("Order not found");
                    }

                    if (order.OrderStatusId == 2 || order.OrderStatusId == 3)
                    {
                        throw new Exception("You already accepted or rejected this order");
                    }

                    // Update the request status
                    order.OrderStatusId = 2;
                    _unitOfWork.OrderRepository.Update(order);

                    var rejectOrders = await _unitOfWork.OrderRepository.GetRequestByPostId(order.PostId);
                    if (rejectOrders != null && rejectOrders.Any())
                    {
                        foreach (var item in rejectOrders)
                        {
                            if (item.Id != order.Id)
                            {
                                item.OrderStatusId = 3;
                                _unitOfWork.OrderRepository.Update(item);
                                var walletTransaction = await _unitOfWork.WalletTransactionRepository.GetByOrderIdAsync(item.Id);
                                if (walletTransaction != null)
                                {
                                    walletTransaction.TransactionType = "Purchase Denied";
                                    _unitOfWork.WalletTransactionRepository.Update(walletTransaction);
                                }
                            }
                        }
                    }

                    var post = await _unitOfWork.PostRepository.GetPostDetail(order.PostId);
                    if (post != null)
                    {
                        if (post.ConditionTypeId == 1)
                        {
                            var wallet = await _unitOfWork.WalletRepository.FindWalletByUserId(order.UserId);
                            var walletTransaction = await _unitOfWork.WalletTransactionRepository.GetByOrderIdAsync(order.Id);
                            wallet.UserBalance -= post.ProductPrice;
                            if (walletTransaction != null)
                            {
                                walletTransaction.TransactionType = "Purchase complete";
                                _unitOfWork.WalletTransactionRepository.Update(walletTransaction);
                            }

                            _unitOfWork.WalletRepository.Update(wallet);
                            await _unitOfWork.SaveChangeAsync();
                        }
                    }

                    // Commit the transaction
                    await transaction.CommitAsync();
                    return await _unitOfWork.SaveChangeAsync() > 0;
                }
                catch (Exception)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync();
                    throw;
                }
            }
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
                if (order.OrderStatusId == 2 || order.OrderStatusId == 4 || order.OrderStatusId == 5)
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

        public async Task<bool> CancleOrder(Guid orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            if (order.OrderStatusId == 2)
            {
                throw new Exception("Order has already been accepted and cannot be canceled.");
            }
            if (order.OrderStatusId == 6)
            {
                throw new Exception("Order has already been cancled.");
            }
            order.OrderStatusId = 6;
            _unitOfWork.OrderRepository.Update(order);
            var walletTransaction = await _unitOfWork.WalletTransactionRepository.GetByOrderIdAsync(orderId);
            walletTransaction.TransactionType = "purchase cancled";
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> ConfirmOrder(Guid orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            if (order.OrderStatusId != 4)
            {
                throw new Exception("Order is not delivered.");
            }
            order.OrderStatusId = 5;
            _unitOfWork.OrderRepository.Update(order);
            var post = await _unitOfWork.PostRepository.GetPostDetail(order.PostId);
            if (post != null)
            {
                var wallet = await _unitOfWork.WalletRepository.GetUserWalletByUserId(post.PostAuthor.AuthorId);
                wallet.UserBalance += post.ProductPrice;
                _unitOfWork.WalletRepository.Update(wallet);
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> CancleOrderForAdmin(Guid orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            if (order.OrderStatusId == 1)
            {
                throw new Exception("Order is pending");
            }
            if (order.OrderStatusId == 3)
            {
                throw new Exception("Order is deny");
            }
            if (order.OrderStatusId == 6)
            {
                throw new Exception("Order is already cancle");
            }
            order.OrderStatusId = 6;
            _unitOfWork.OrderRepository.Update(order);
            var walletTransaction = await _unitOfWork.WalletTransactionRepository.GetByOrderIdAsync(orderId);
            walletTransaction.TransactionType = "purchase cancled";
            var post = await _unitOfWork.PostRepository.GetPostDetail(order.PostId);
            var wallet = await _unitOfWork.WalletRepository.FindWalletByUserId(order.UserId);
            if (wallet != null)
            {
                if (post != null)
                {
                    if (post.ConditionTypeId ==1)
                    {
                        wallet.UserBalance += post.ProductPrice;
                        _unitOfWork.WalletRepository.Update(wallet);
                    }
                }
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<List<ReceiveOrderViewModel>> GetAllOrderAsync()
        {
            return await _unitOfWork.OrderRepository.GetAllOrder();
        }

        public async Task<List<ReceiveOrderViewModel>> GetAllOrderByChatRoomId(Guid chatRoomID)
        {
            var chatRoom = await _unitOfWork.ChatRoomRepository.GetByIdAsync(chatRoomID);
            if(chatRoom != null)
            {
                var listOrder= await _unitOfWork.OrderRepository.GetAllRequestBy2UserId(chatRoom.SenderId, chatRoom.ReceiverId) ?? new List<ReceiveOrderViewModel>();
                return listOrder;
            }
            throw new Exception("chatRoom not exist");
        }

        public async Task<List<ReceiveOrderViewModel>> GetAllOrderByCurrentUser()
        {
            return await _unitOfWork.OrderRepository.GetAllOrderByUserId(_claimService.GetCurrentUserId) ?? new List<ReceiveOrderViewModel>();
        }

        public async Task<List<SentOrderViewModel>> GetSendOrderByChatRoomId(Guid chatRoomId)
        {
            var chatRoom = await _unitOfWork.ChatRoomRepository.GetByIdAsync(chatRoomId);
            if (chatRoom != null)
            {
                var currentUserId = _claimService.GetCurrentUserId;
                var postOwnerId = new Guid();
                if (currentUserId == chatRoom.SenderId)
                {
                    postOwnerId = chatRoom.ReceiverId;
                }
                else
                {
                    currentUserId = chatRoom.ReceiverId;
                    postOwnerId = chatRoom.SenderId;
                }
                var listOrder = await _unitOfWork.OrderRepository.GetAllSendOrderBy2UserId(currentUserId, postOwnerId) ?? new List<SentOrderViewModel>();
                return listOrder;
            }
            throw new Exception("chatRoom not exist");
        }

        public async Task<List<ReceiveOrderViewModel>> GetReceiveOrderByChatRoomId(Guid chatRoomId)
        {
            var chatRoom = await _unitOfWork.ChatRoomRepository.GetByIdAsync(chatRoomId);
            if (chatRoom != null)
            {
                var currentUserId = _claimService.GetCurrentUserId;
                var orderCreatedBy = new Guid();
                if (currentUserId == chatRoom.SenderId)
                {
                    orderCreatedBy = chatRoom.ReceiverId;
                }
                else
                {
                    currentUserId = chatRoom.ReceiverId;
                    orderCreatedBy = chatRoom.SenderId;
                }
                var listOrder = await _unitOfWork.OrderRepository.GetAllReceiveOrderBy2UserId(orderCreatedBy, currentUserId) ?? new List<ReceiveOrderViewModel>();
                return listOrder;
            }
            throw new Exception("chatRoom not exist");
        }
    }
}
