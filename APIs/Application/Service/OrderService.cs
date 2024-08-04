using Application.InterfaceService;
using Application.ViewModel.OrderModel;
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

        public async Task<bool> AcceptOrder(Guid OrderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(OrderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            //check order
            if (order.OrderStatusId == 2 || order.OrderStatusId == 3)
            {
                throw new Exception("You already accepted or rejected this order");
            }

            // Update the Order status
            order.OrderStatusId = 2;
            _unitOfWork.OrderRepository.Update(order);

            var rejectOrders = await _unitOfWork.OrderRepository.GetOrderByPostId(order.PostId);
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
                            walletTransaction.TransactionType = "Purchase denied";
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
                }
            }
            return await _unitOfWork.SaveChangeAsync()>0;
        }

        public async Task<bool> DeliveredOrder(Guid orderId)
        {
            var Order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (Order == null)
            {
                throw new Exception("Order not found");
            }

            if (Order.OrderStatusId != 2)
            {
                throw new Exception("Order not accepted");
            }
            // Update the Order status
            Order.OrderStatusId = 4;
            _unitOfWork.OrderRepository.Update(Order);
            // Save all changes
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<List<SentOrderViewModel>> GetAllOrdersOfCreatebByUserAsync()
        {
            return await _unitOfWork.OrderRepository.GetAllOrderByCreatedByUserId(_claimService.GetCurrentUserId);
        }

        public async Task<List<ReceiveOrderViewModel>> GetAllOrdersOfCurrentUserAsync()
        {
            return await _unitOfWork.OrderRepository.GetAllOrderByCurrentUserId(_claimService.GetCurrentUserId);
        }
        public async Task<bool> CheckOrderStatusByPostId(Guid postId)
        {
            var OrderList = await _unitOfWork.OrderRepository.GetOrderByPostId(postId);
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
            if (walletTransaction != null)
            {
                walletTransaction.TransactionType = "purchase cancled";
                _unitOfWork.WalletTransactionRepository.Update(walletTransaction);
            }
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
                var walletTransaction = await _unitOfWork.WalletTransactionRepository.GetByOrderIdAsync(orderId);
                if (walletTransaction != null)
                {
                    var wallet = await _unitOfWork.WalletRepository.GetUserWalletByUserId(post.PostAuthor.AuthorId);
                    wallet.UserBalance += post.ProductPrice;
                    _unitOfWork.WalletRepository.Update(wallet);
                }
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
            var orderStatus = order.OrderStatusId;
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
                        if (orderStatus == 2 || orderStatus == 4 || orderStatus == 6)
                        {
                            wallet.UserBalance += post.ProductPrice;
                            _unitOfWork.WalletRepository.Update(wallet);
                        }
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
                var listOrder= await _unitOfWork.OrderRepository.GetAllOrderBy2UserId(chatRoom.SenderId, chatRoom.ReceiverId) ?? new List<ReceiveOrderViewModel>();
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
