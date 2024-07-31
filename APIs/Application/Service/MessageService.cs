using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.ChatRoomModel;
using Application.ViewModel.MessageModel;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimService _claimService;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper, IClaimService claimService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimService = claimService;
        }

        public async Task<Message> CreateMessage(CreateMessageModel messageModel)
        {
            var newMessage = _mapper.Map<Message>(messageModel);
            newMessage.CreationDate = DateTime.UtcNow;
            newMessage.CreatedBy = messageModel.CreatedBy;
            await _unitOfWork.MessageRepository.AddAsync(newMessage);
            await _unitOfWork.SaveChangeAsync();
            return newMessage;
        }

        public async Task<bool> DeleteMessage(Guid messageId)
        {
            var message = await _unitOfWork.MessageRepository.GetByIdAsync(messageId);
            if (message != null)
            {
                _unitOfWork.MessageRepository.SoftRemove(message);
                return await _unitOfWork.SaveChangeAsync() > 0;
            }
            return false;
        }

        public async Task<List<Message>> GetAllMessages()
        {
            return await _unitOfWork.MessageRepository.GetAllAsync();
        }

        public async Task<Message> GetMessageById(Guid id)
        {
            return await _unitOfWork.MessageRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateMessage(UpdateMessageModel messageModel)
        {
            var updateMessage = _mapper.Map<Message>(messageModel);
            _unitOfWork.MessageRepository.Update(updateMessage);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<ChatRoomWithOrder> GetOrCreateChatRoomAsync(Guid user1, Guid postId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(user1);
            if (user == null)
            {
                return null;
            }
            Guid user2 = _claimService.GetCurrentUserId;
            var chatRoom = await _unitOfWork.ChatRoomRepository.GetRoomBy2UserId(user1, user2);
            if (chatRoom != null)
            {
                var post = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(user1);
                if (!post.Where(x => x.Id == postId).Any())
                {

                }
                else
                {
                    var checkOrders = await _unitOfWork.OrderRepository.GetRequestByPostId(postId);
                    if (checkOrders != null && checkOrders.Any())
                    {
                        foreach (var item in checkOrders)
                        {
                            if (item.OrderStatusId == 2 || item.OrderStatusId == 4 || item.OrderStatusId == 5)
                            {
                                throw new Exception("This post already been sold");
                            }
                        }
                    }
                    // create order
                    var duplicateRequest = await _unitOfWork.OrderRepository.GetRequestByUserIdAndPostId(user1, postId);
                    if (duplicateRequest.Where(x => x.CreatedBy == _claimService.GetCurrentUserId).Any())
                    {
                        throw new Exception("You already order this");
                    }
                    else
                    {
                        Order order = new Order
                        {
                            PostId = postId,
                            OrderStatusId = 1,
                            OrderMessage = "",
                            UserId = user2,
                            CreatedBy = user2,
                        };
                        await _unitOfWork.OrderRepository.AddAsync(order);
                        await _unitOfWork.SaveChangeAsync();
                        // create pending transaction
                        var wallet = await _unitOfWork.WalletRepository.GetUserWalletByUserId(user2);
                        var wallletTransaction = await _unitOfWork.WalletTransactionRepository.GetAllTransactionByUserId(user2);
                        var postForProductPrice = await _unitOfWork.PostRepository.GetPostDetail(postId);
                        float pendingTransaction = 0;
                        if (wallletTransaction != null)
                        {
                            foreach (var item in wallletTransaction)
                            {
                                if (item.Action == "Purchase pending")
                                {
                                    pendingTransaction += item.Amount;
                                }
                            }
                        }
                        if (wallet.UserBalance - pendingTransaction < postForProductPrice.ProductPrice)
                        {
                            throw new Exception("You don't have enough money to order this transaction");
                        }
                        var newWalletTransaction = new WalletTransaction
                        {
                            OrderId = order.Id,
                            Amount = postForProductPrice.ProductPrice,
                            TransactionType = "Purchase pending",
                            WalletId = wallet.Id,
                        };
                        await _unitOfWork.WalletTransactionRepository.AddAsync(newWalletTransaction);
                        await _unitOfWork.SaveChangeAsync();
                    }
                }
                return chatRoom;
            }
            var newRoom = new ChatRoom
            {
                SenderId = user2,
                ReceiverId = user1
            };
            var room = _mapper.Map<ChatRoom>(newRoom);
            await _unitOfWork.ChatRoomRepository.AddAsync(newRoom);
            await _unitOfWork.SaveChangeAsync();
            if (postId != Guid.Empty)
            {
                var postModel = await _unitOfWork.PostRepository.GetPostDetail(postId);
                var duplicateMessage = _unitOfWork.MessageRepository.getByContent("Tôi đang có hứng thú với món đồ " + postModel.PostTitle + " " + postModel.ProductImageUrl);
                if (duplicateMessage!=null)
                {
                    var createMessageModel = new CreateMessageModel
                    {
                        MessageContent = "Tôi đang có hứng thú với món đồ " + postModel.PostTitle + " " + postModel.ProductImageUrl,
                        RoomId = room.Id
                    };
                    var newMessage = _mapper.Map<Message>(createMessageModel);
                    newMessage.CreationDate = DateTime.UtcNow;
                    newMessage.CreatedBy = user2;
                    await _unitOfWork.MessageRepository.AddAsync(newMessage);
                }
                var post = await _unitOfWork.PostRepository.GetAllPostsByCreatedByIdAsync(user1);
                if (!post.Where(x => x.Id == postId).Any())
                {

                }
                else
                {
                    var checkOrders = await _unitOfWork.OrderRepository.GetRequestByPostId(postId);
                    if (checkOrders != null && checkOrders.Any())
                    {
                        foreach (var item in checkOrders)
                        {
                            if (item.OrderStatusId == 2 || item.OrderStatusId == 4 || item.OrderStatusId == 5)
                            {
                                throw new Exception("This post already been sold");
                            }
                        }
                    }
                    //check duplicate order
                    var duplicateRequest = await _unitOfWork.OrderRepository.GetRequestByUserIdAndPostId(user1, postId);
                    if (duplicateRequest.Where(x => x.CreatedBy == _claimService.GetCurrentUserId).Any())
                    {
                        throw new Exception("You already order this");
                    }
                    else
                    {
                        // create order
                        Order order = new Order
                        {
                            PostId = postId,
                            OrderStatusId = 1,
                            OrderMessage = "",
                            UserId = user2,
                            CreatedBy = user2,
                        };
                        await _unitOfWork.OrderRepository.AddAsync(order);
                        await _unitOfWork.SaveChangeAsync();
                        // create pending transaction
                        var wallet = await _unitOfWork.WalletRepository.GetUserWalletByUserId(user2);
                        var wallletTransaction = await _unitOfWork.WalletTransactionRepository.GetAllTransactionByUserId(user2);
                        var postForProductPrice = await _unitOfWork.PostRepository.GetPostDetail(postId);
                        float pendingTransaction = 0;
                        if (wallletTransaction != null)
                        {
                            foreach (var item in wallletTransaction)
                            {
                                if (item.Action == "Purchase pending")
                                {
                                    pendingTransaction += item.Amount;
                                }
                            }
                        }
                        if (wallet.UserBalance - pendingTransaction < postForProductPrice.ProductPrice)
                        {
                            throw new Exception("You don't have enough money to order this transaction");
                        }
                        var newWalletTransaction = new WalletTransaction
                        {
                            OrderId = order.Id,
                            Amount = postForProductPrice.ProductPrice,
                            TransactionType = "Purchase pending",
                            WalletId = wallet.Id
                        };
                        await _unitOfWork.WalletTransactionRepository.AddAsync(newWalletTransaction);
                        await _unitOfWork.SaveChangeAsync();
                    }
                }
            }
            var messages = await _unitOfWork.ChatRoomRepository.GetMessagesByRoomId(newRoom.Id);
            return messages;
        }

        public async Task<ChatRoomWithOrder> GetMessagesByChatRoomId(Guid chatRoomId)
        {
            var messages = await _unitOfWork.ChatRoomRepository.GetMessagesByRoomId(chatRoomId);
            return messages;
        }

        public async Task<ChatRoom> GetChatRoomByIdAsync(Guid chatRoomId)
        {
            var chatroom = await _unitOfWork.ChatRoomRepository.GetByIdAsync(chatRoomId);
            return chatroom;
        }

        public async Task<List<ChatRoomWithOrder>> GetAllChatRoomsByUserIdAsync()
        {
            var userId = _claimService.GetCurrentUserId;
            var chatroom = await _unitOfWork.ChatRoomRepository.GetByUserIdAsync(userId);
            return chatroom;
        }
    }
}
