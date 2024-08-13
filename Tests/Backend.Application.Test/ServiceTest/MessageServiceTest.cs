using Application.Common;
using Application.InterfaceService;
using Application.Service;
using Application.ViewModel.ChatRoomModel;
using Application.ViewModel.PostModel;
using Application.ViewModel.TransactionModel;
using Backend.Domain.Test;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Application.Test.ServiceTest
{
    public class MessageServiceTest : SetupTest
    {
        private IMessageService _messageService;
        public MessageServiceTest()
        {
            _messageService = new MessageService(_unitOfWorkMock.Object, _mapper, _claimServiceMock.Object);
        }
        [Fact]
        public async Task GetOrCreateChatRoomAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            Guid user1 = Guid.NewGuid();
            Guid postId = Guid.NewGuid();
            _unitOfWorkMock.Setup(repo => repo.UserRepository.GetByIdAsync(user1)).ReturnsAsync((User)null);

            // Act
            var result = await _messageService.GetOrCreateChatRoomAsync(user1, postId);

            // Assert
            Assert.Null(result);
        }
       /* [Fact]
        public async Task GetOrCreateChatRoomAsync_ChatRoomExists_ReturnsChatRoomWithOrder()
        {
            // Arrange
            Guid user1 = Guid.NewGuid();
            Guid postId = Guid.NewGuid();
            Guid user2 = Guid.NewGuid();

            var existingChatRoomWithOrder = new ChatRoomWithOrder
            {
                roomId = Guid.NewGuid(),
                SenderId = user2,
                ReceiverId = user1,
                SenderName = "Sender Name",
                ReceiverName = "Receiver Name",
                SenderAvatar = "Sender Avatar URL",
                ReceiverAvatar = "Receiver Avatar URL",
                Messages = new List<MessageDto>(),
                Order = new List<OrderDto>()
            };
            var duplicateOrders = new List<Order>
            {
                new Order { CreatedBy = user2 } // Ensure this matches the `CreatedBy` used in your service
            };
            _unitOfWorkMock.Setup(repo => repo.UserRepository.GetByIdAsync(user1))
                           .ReturnsAsync(new User { Id = user1 });
            _claimServiceMock.Setup(cs => cs.GetCurrentUserId).Returns(user2);
            _unitOfWorkMock.Setup(repo => repo.ChatRoomRepository.GetRoomBy2UserId(user1, user2))
                           .ReturnsAsync(existingChatRoomWithOrder);
            _unitOfWorkMock.Setup(repo => repo.OrderRepository.GetOrderByPostId(postId))
                   .ReturnsAsync(new List<Order>());
            _unitOfWorkMock.Setup(repo => repo.OrderRepository.GetOrderByUserIdAndPostId(user1, postId))
                   .ReturnsAsync(duplicateOrders);
            // Add other necessary mock setups here (e.g., for OrderRepository, WalletRepository, etc.)

            // Act
            var result = await _messageService.GetOrCreateChatRoomAsync(user1, postId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingChatRoomWithOrder.roomId, result.roomId);
            Assert.Equal(existingChatRoomWithOrder.SenderName, result.SenderName);
            Assert.Equal(existingChatRoomWithOrder.ReceiverName, result.ReceiverName);
            Assert.Equal(existingChatRoomWithOrder.SenderAvatar, result.SenderAvatar);
            Assert.Equal(existingChatRoomWithOrder.ReceiverAvatar, result.ReceiverAvatar);
            // Add other assertions as needed
        }*/
        /*[Fact]
        public async Task GetOrCreateChatRoomAsync_NoChatRoomExists_CreatesAndReturnsChatRoom()
        {
            // Arrange
            Guid user1 = Guid.NewGuid();
            Guid postId = Guid.NewGuid();
            Guid user2 = Guid.NewGuid();
            var newChatRoom = new ChatRoomWithOrder
            {
                roomId = Guid.NewGuid(),
                SenderId = user2,
                ReceiverId = user1,
                SenderName = "Sender Name",
                ReceiverName = "Receiver Name",
                SenderAvatar = "Sender Avatar URL",
                ReceiverAvatar = "Receiver Avatar URL",
                Messages = new List<MessageDto>(),
                Order = new List<OrderDto>()
            };
            var duplicateOrders = new List<Order>
            {
                new Order { CreatedBy = user2 } // Ensure this matches the `CreatedBy` used in your service
            };
            _unitOfWorkMock.Setup(repo => repo.UserRepository.GetByIdAsync(user1)).ReturnsAsync(new User { Id = user1 });
            _claimServiceMock.Setup(cs => cs.GetCurrentUserId).Returns(user2);
            _unitOfWorkMock.Setup(repo => repo.ChatRoomRepository.GetRoomBy2UserId(user1, user2)).ReturnsAsync((ChatRoomWithOrder)null);
            _unitOfWorkMock.Setup(repo => repo.ChatRoomRepository.AddAsync(It.IsAny<ChatRoom>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(repo => repo.SaveChangeAsync())
               .ReturnsAsync(1);
            _unitOfWorkMock.Setup(repo => repo.ChatRoomRepository.GetRoomBy2UserId(user1, user2)).ReturnsAsync(newChatRoom);
            _unitOfWorkMock.Setup(repo => repo.OrderRepository.GetOrderByUserIdAndPostId(user1, postId))
                   .ReturnsAsync(duplicateOrders);
            // Act
            var result = await _messageService.GetOrCreateChatRoomAsync(user1, postId);

            // Assert
            Assert.Equal(newChatRoom, result);
        }*/
        [Fact]
        public async Task GetOrCreateChatRoomAsync_PostAlreadySold_ThrowsException()
        {
            // Arrange
            Guid user1 = Guid.NewGuid();
            Guid postId = Guid.NewGuid();
            Guid user2 = Guid.NewGuid();

            var soldOrders = new List<Order>
            {
                new Order { OrderStatusId = 2 } // Sold
            };
            _unitOfWorkMock.Setup(repo => repo.ChatRoomRepository.GetRoomBy2UserId(user1, user2))
               .ReturnsAsync((ChatRoomWithOrder)null);
            _unitOfWorkMock.Setup(repo => repo.UserRepository.GetByIdAsync(user1))
                           .ReturnsAsync(new User { Id = user1 });
            _claimServiceMock.Setup(cs => cs.GetCurrentUserId).Returns(user2);
            _unitOfWorkMock.Setup(repo => repo.OrderRepository.GetOrderByPostId(postId))
                           .ReturnsAsync(soldOrders);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _messageService.GetOrCreateChatRoomAsync(user1, postId));
        }

        [Fact]
        public async Task GetOrCreateChatRoomAsync_InsufficientFunds_ThrowsException()
        {
            // Arrange
            Guid user1 = Guid.NewGuid();
            Guid postId = Guid.NewGuid();
            Guid user2 = Guid.NewGuid();

            var wallet = new Wallet { UserBalance = 50 };
            var walletTransactions = new List<TransactionViewModel>
            {
                new TransactionViewModel { Action = "Purchase pending", Amount = 60 }
            };
            var postForProductPrice = new PostDetailViewModel { ProductPrice = 100 };
            _unitOfWorkMock.Setup(repo => repo.ChatRoomRepository.GetRoomBy2UserId(user1, user2))
               .ReturnsAsync((ChatRoomWithOrder)null);
            _unitOfWorkMock.Setup(repo => repo.UserRepository.GetByIdAsync(user1))
                           .ReturnsAsync(new User { Id = user1 });
            _claimServiceMock.Setup(cs => cs.GetCurrentUserId).Returns(user2);
            _unitOfWorkMock.Setup(repo => repo.OrderRepository.GetOrderByPostId(postId))
                           .ReturnsAsync(new List<Order>());
            _unitOfWorkMock.Setup(repo => repo.OrderRepository.GetOrderByUserIdAndPostId(user1, postId))
                           .ReturnsAsync(new List<Order>());
            _unitOfWorkMock.Setup(repo => repo.WalletRepository.GetUserWalletByUserId(user2))
                           .ReturnsAsync(wallet);
            _unitOfWorkMock.Setup(repo => repo.WalletTransactionRepository.GetAllTransactionByUserId(user2))
                           .ReturnsAsync(walletTransactions);
            _unitOfWorkMock.Setup(repo => repo.PostRepository.GetPostDetail(postId))
                           .ReturnsAsync(postForProductPrice);
            _unitOfWorkMock.Setup(repo => repo.PolicyRepository.GetAllAsync())
                .ReturnsAsync(new List<Policy>
                {
                    new Policy
                    {
                        PostPrice = 15000,
                        OrderCancelledAmount = 3
                    }
                });
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _messageService.GetOrCreateChatRoomAsync(user1, postId));
        }
        [Fact]
        public async Task GetOrCreateChatRoomAsync_DuplicateMessage_CreatesMessage()
        {
            // Arrange
            Guid user1 = Guid.NewGuid();
            Guid postId = Guid.NewGuid();
            Guid user2 = Guid.NewGuid();

            var existingChatRoomWithOrder = new ChatRoomWithOrder
            {
                roomId = Guid.NewGuid(),
                SenderId = user2,
                ReceiverId = user1,
                SenderName = "Sender Name",
                ReceiverName = "Receiver Name",
                SenderAvatar = "Sender Avatar URL",
                ReceiverAvatar = "Receiver Avatar URL",
                Messages = new List<MessageDto>(),
                Order = new List<OrderDto>()
            };

            var postForProductPrice = new PostDetailViewModel
            {
                ProductPrice = 100,
                PostTitle = "Sample Post",
                ProductImageUrl = "http://example.com/image.jpg"
            };

            _unitOfWorkMock.Setup(repo => repo.UserRepository.GetByIdAsync(user1))
                           .ReturnsAsync(new User { Id = user1 });
            _claimServiceMock.Setup(cs => cs.GetCurrentUserId).Returns(user2);
            _unitOfWorkMock.Setup(repo => repo.ChatRoomRepository.GetRoomBy2UserId(user1, user2))
                           .ReturnsAsync(existingChatRoomWithOrder);
            _unitOfWorkMock.Setup(repo => repo.PostRepository.GetPostDetail(postId))
                           .ReturnsAsync(postForProductPrice);
            _unitOfWorkMock.Setup(repo => repo.OrderRepository.GetOrderByPostId(postId))
               .ReturnsAsync(new List<Order>());
            _unitOfWorkMock.Setup(repo => repo.WalletRepository.GetUserWalletByUserId(user2))
               .ReturnsAsync(new Wallet { UserBalance = 160 });
            _unitOfWorkMock.Setup(repo => repo.WalletTransactionRepository.GetAllTransactionByUserId(user2))
               .ReturnsAsync(new List<TransactionViewModel>
               {
                   new TransactionViewModel { Action = "Purchase pending", Amount = 60 }
               });
            _unitOfWorkMock.Setup(repo => repo.MessageRepository.AddAsync(It.IsAny<Message>())).Verifiable();
            _unitOfWorkMock.Setup(repo => repo.PolicyRepository.GetAllAsync())
                .ReturnsAsync(new List<Policy>
                {
                    new Policy
                    {
                        PostPrice = 15000,
                        OrderCancelledAmount = 3
                    }
                });
            // For PostRepository
            _unitOfWorkMock.Setup(repo => repo.PostRepository.GetPostDetail(postId))
                           .ReturnsAsync(new PostDetailViewModel { ProductPrice = 100 });
            var existingMessage = new Message
            {
                MessageContent = "Tôi đang có hứng thú với món đồ Sample Post http://example.com/image.jpg"
            };

            _unitOfWorkMock.Setup(repo => repo.MessageRepository.getByContent(It.IsAny<string>()))
                           .ReturnsAsync(existingMessage);

            // Act
            var result = await _messageService.GetOrCreateChatRoomAsync(user1, postId);

            // Assert
            Assert.NotNull(result);
        }

    }
}