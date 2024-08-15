using Application.InterfaceService;
using Application.ViewModel.OrderModel;
using AutoMapper;
using Domain.Entities;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
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
        private readonly int _pending = 1;
        private readonly int _accept = 2;
        private readonly int _reject = 3;
        private readonly int _cancel = 4;
        private readonly int _confirm = 5;
        private readonly int _delivered = 6;
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
            if (order.OrderStatusId == _accept || order.OrderStatusId == _reject)
            {
                throw new Exception("You already accepted or rejected this order");
            }

            // Update the Order status
            order.OrderStatusId = _accept;
            _unitOfWork.OrderRepository.Update(order);
            var post = await _unitOfWork.PostRepository.GetPostDetail(order.PostId);
            if (post != null)
            {
                if (post.ConditionTypeId == 1)
                {
                    BackgroundJob.Schedule(() => (ChangeOrderStatus(OrderId, _accept)), TimeSpan.FromHours(12));
                }
            }
            return await _unitOfWork.SaveChangeAsync()>0;
        }

        public async Task<bool> DeliveredOrder(Guid orderId)
        {
            var Order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (Order == null)
            {
                throw new Exception("Order is not found");
            }

            if (Order.OrderStatusId != _accept)
            {
                throw new Exception("Order is not accepted");
            }
            // Update the Order status
            Order.OrderStatusId = _delivered;
            _unitOfWork.OrderRepository.Update(Order);
            BackgroundJob.Schedule(() => (ChangeOrderStatus(orderId, _delivered)), TimeSpan.FromHours(12));
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
                if (order.OrderStatusId == _accept || order.OrderStatusId == _confirm || order.OrderStatusId == _delivered)
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
            if (order.OrderStatusId == _accept)
            {
                throw new Exception("Order has already been accepted and cannot be canceled.");
            }
            if (order.OrderStatusId == _delivered)
            {
                throw new Exception("Order has already been delivered and cannot be canceled.");
            }
            if (order.OrderStatusId == _confirm)
            {
                throw new Exception("Order has already been confirm and cannot be canceled.");
            }
            if (order.OrderStatusId == _cancel)
            {
                throw new Exception("Order has already been cancled.");
            }
            order.OrderStatusId = _cancel;
            _unitOfWork.OrderRepository.Update(order);
            var walletTransaction = await _unitOfWork.WalletTransactionRepository.GetByOrderIdAsync(orderId);
            if (walletTransaction != null)
            {
                WalletTransaction newWalletTransaction = new WalletTransaction
                {
                    Amount = walletTransaction.Amount,
                    OrderId = orderId,
                    WalletId = walletTransaction.WalletId,
                    TransactionType = "Cancelled Pending"
                };
                await _unitOfWork.WalletTransactionRepository.AddAsync(newWalletTransaction);
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

            if (order.OrderStatusId != _delivered)
            {
                throw new Exception("Order is not delivered.");
            }
            order.OrderStatusId = _confirm;
            _unitOfWork.OrderRepository.Update(order);
            var post = await _unitOfWork.PostRepository.GetPostDetail(order.PostId);
            if (post != null)
            {
                var walletTransaction = await _unitOfWork.WalletTransactionRepository.GetByOrderIdAsync(orderId);
                if (walletTransaction != null)
                {
                    var wallet = await _unitOfWork.WalletRepository.FindWalletByUserId(order.UserId);
                    wallet.UserBalance -= post.ProductPrice;
                    var walletTransactionBuyer = new WalletTransaction
                    {
                        Amount = post.ProductPrice,
                        TransactionType = "Purchase complete",
                        WalletId = wallet.Id
                    };
                    await _unitOfWork.WalletTransactionRepository.AddAsync(walletTransactionBuyer);
                    _unitOfWork.WalletTransactionRepository.Update(walletTransaction);
                    _unitOfWork.WalletRepository.Update(wallet);

                    var walletPost = await _unitOfWork.WalletRepository.GetUserWalletByUserId(post.PostAuthor.AuthorId);
                    walletPost.UserBalance += post.ProductPrice;
                    _unitOfWork.WalletRepository.Update(wallet);
                    var walletTransactionPostOwner = new WalletTransaction
                    {
                        Amount = post.ProductPrice,
                        TransactionType = "Product Sale",
                        WalletId = walletPost.Id
                    };
                    await _unitOfWork.WalletTransactionRepository.AddAsync(walletTransactionPostOwner);
                }
            }
            var rejectOrders = await _unitOfWork.OrderRepository.GetOrderByPostId(order.PostId);
            if (rejectOrders != null && rejectOrders.Any())
            {
                foreach (var item in rejectOrders)
                {
                    if (item.Id != order.Id)
                    {
                        item.OrderStatusId = _reject;
                        _unitOfWork.OrderRepository.Update(item);
                        var walletTransaction = await _unitOfWork.WalletTransactionRepository.GetByOrderIdAsync(item.Id);
                        if (walletTransaction != null)
                        {
                            WalletTransaction newWalletTransaction = new WalletTransaction
                            {
                                Amount = walletTransaction.Amount,
                                OrderId = orderId,
                                WalletId = walletTransaction.WalletId,
                                TransactionType = "Purchase denied"
                            };
                            await _unitOfWork.WalletTransactionRepository.AddAsync(newWalletTransaction);
                        }
                    }
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
            order.OrderStatusId = _cancel;
            _unitOfWork.OrderRepository.Update(order);
            var walletTransaction = await _unitOfWork.WalletTransactionRepository.GetByOrderIdAsync(orderId);
            if (walletTransaction != null)
            {
                var post = await _unitOfWork.PostRepository.GetPostDetail(order.PostId);
                var wallet = await _unitOfWork.WalletRepository.FindWalletByUserId(order.UserId);
                if (wallet != null)
                {
                    if (post != null)
                    {
                        if (post.ConditionTypeId == 1)
                        {
                            WalletTransaction newWalletTransaction = new WalletTransaction
                            {
                                Amount = walletTransaction.Amount,
                                OrderId = orderId,
                                WalletId = walletTransaction.WalletId,
                                TransactionType = "Purchase cancle"
                            };
                            await _unitOfWork.WalletTransactionRepository.AddAsync(newWalletTransaction);
                            if (orderStatus == _confirm )
                            {
                                wallet.UserBalance += post.ProductPrice;
                                _unitOfWork.WalletRepository.Update(wallet);
                            }
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
        public async Task<bool> ChangeOrderStatus(Guid orderId, int oldOrderStatusId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if(order != null)
            {
                if (order.OrderStatusId == oldOrderStatusId)
                {
                    order.OrderStatusId = _cancel;
                    _unitOfWork.OrderRepository.Update(order);
                }
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
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

        public async Task<List<OrderViewModelForWeb>> GetAllOrderForWebAsync()
        {
            var listOrder = await _unitOfWork.OrderRepository.GetAllOrderForWeb();
            return listOrder;
        }
    }
}
