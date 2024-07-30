using Application.InterfaceService;
using Application.Service;
using Application.ViewModel.PostModel;
using Application.ViewModel.ProductModel;
using Application.ViewModel.SubscriptionHistoryModel;
using AutoFixture;
using Backend.Domain.Test;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Application.Test.ServiceTest
{
    public class PostServiceTest : SetupTest
    {
        private IPostService _postService;
        public PostServiceTest()
        {
            _postService = new PostService(_unitOfWorkMock.Object, _mapper, _appConfiguration.Object, _currentTimeMock.Object, _claimServiceMock.Object, _uploadFileMock.Object);
        }
        [Fact]
        public async Task BanPost_ShouldReturnCorrect()
        {
            //Arrange
            var post = _fixture.Build<Post>().Create();
            //Act
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetByIdAsync(post.Id)).ReturnsAsync(post);
            _unitOfWorkMock.Setup(unit => unit.PostRepository.SoftRemove(post)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            bool isDelete = await _postService.BanPost(post.Id);
            //Assert
            Assert.True(isDelete);
        }
        [Fact]
        public async Task BanPost_ShouldThrowException()
        {
            //Arrange
            var post = _fixture.Build<Post>().Create();
            //Act
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetByIdAsync(post.Id)).ReturnsAsync(post);
            _unitOfWorkMock.Setup(unit => unit.PostRepository.SoftRemove(post)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            //Assert
            Assert.ThrowsAsync<Exception>(async () => await _postService.BanPost(Guid.NewGuid()));
        }
        [Fact]
        public async Task GetAllPost_ShouldReturnCorrect()
        {
            //Arrage 

            var posts = _fixture.Build<Post>().CreateMany(2).ToList();
            var product = _fixture.Build<Product>().Create();
            var newPost = _fixture.Build<Post>().With(x => x.CreatedBy, Guid.Parse("981b9606-4f84-41b4-8a46-7b578bc1823d")).Create();
            posts.Add(newPost);
            List<PostViewModel> list = new List<PostViewModel>();
            var filterPost = posts.Where(x => x.CreatedBy != Guid.Parse("981b9606-4f84-41b4-8a46-7b578bc1823d")).ToList();
            foreach (var post in filterPost)
            {
                PostViewModel postViewModel = new PostViewModel()
                {
                    PostId = post.Id,
                    CreationDate = DateOnly.FromDateTime(post.CreationDate.Value),
                    PostContent = post.PostContent,
                    PostTitle = post.PostTitle,
                    Product = new ProductModel()
                    {
                        CategoryId = product.CategoryId,
                        CategoryName = "",
                        ConditionId = product.ConditionId,
                        ConditionName = "",
                        ProductId = product.Id,
                        ProductImageUrl = product.ProductImageUrl,
                        ProductPrice = product.ProductPrice,
                        ProductStatus = product.ProductStatus,
                        RequestedProduct = ""
                    }
                };
                list.Add(postViewModel);
            }
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(Guid.Parse("981b9606-4f84-41b4-8a46-7b578bc1823d"));
            _unitOfWorkMock.Setup(unit => unit.PostRepository.AddRangeAsync(posts.ToList())).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetAllPost(It.IsAny<Guid>())).ReturnsAsync(list);
            var pagintaedPost = await _postService.GetAllPost();
            Assert.Equal(pagintaedPost.Count(), 2);
        }
        [Fact]
        public async Task CreatePost_WithWalletOption_ShouldBeSuceeded()
        {
            //Arrange 
            IFormFile productFile = null;
            string exePath = Environment.CurrentDirectory.ToString();
            string filePath = exePath + "/ImageFolder/Class Diagram-Create Post.drawio.png";
            var fileInfo = new FileInfo(filePath);
            var memoryStream = new MemoryStream();

            using (var stream = fileInfo.OpenRead())
            {
                stream.CopyTo(memoryStream);
            }
            memoryStream.Position = 0;
            productFile = new FormFile(memoryStream, 0, memoryStream.Length, fileInfo.Name, fileInfo.Name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png",// Adjust the content type as needed

            };
            var productModel = _fixture.Build<CreateProductModel>().With(x => x.ProductImage, productFile).Create();
            var product = _mapper.Map<Product>(productModel);
            var wallet = _fixture.Build<Wallet>().With(x => x.UserBalance, 15000)
                                               .With(x => x.OwnerId, Guid.Parse("981b9606-4f84-41b4-8a46-7b578bc1823d")).Create();
            var postModel = _fixture.Build<CreatePostModel>().With(x => x.PaymentType, "Wallet").With(x => x.productModel, productModel).Create();
            var post = _mapper.Map<Post>(postModel);
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(Guid.Parse("981b9606-4f84-41b4-8a46-7b578bc1823d"));
            _unitOfWorkMock.Setup(unit => unit.PostRepository.AddAsync(post)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.ProductRepository.AddAsync(product)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(unit => unit.WalletRepository.GetUserWalletByUserId(It.IsAny<Guid>())).ReturnsAsync(wallet);
            _uploadFileMock.Setup(upload => upload.UploadFileToFireBase(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("Testlink");
            var isCreated = await _postService.CreatePost(postModel);
            Assert.True(isCreated);
        }
        [Fact]
        public async Task CreatePost_WithSubscriptionOption_ShouldBeSuceeded()
        {
            //Arrange 
            IFormFile productFile = null;
            string exePath = Environment.CurrentDirectory.ToString();
            string filePath = exePath + "/ImageFolder/Class Diagram-Create Post.drawio.png";
            var fileInfo = new FileInfo(filePath);
            var memoryStream = new MemoryStream();

            using (var stream = fileInfo.OpenRead())
            {
                stream.CopyTo(memoryStream);
            }
            memoryStream.Position = 0;
            productFile = new FormFile(memoryStream, 0, memoryStream.Length, fileInfo.Name, fileInfo.Name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png",// Adjust the content type as needed

            };
            var productModel = _fixture.Build<CreateProductModel>().With(x => x.ProductImage, productFile).Create();
            var product = _mapper.Map<Product>(productModel);
            var subscriptionHistory = _fixture.Build<SubscriptionHistoryDetailViewModel>()
                .With(x => x.StartDate, DateOnly.FromDateTime(DateTime.UtcNow))
                .With(x => x.EndDate, DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1))).CreateMany(2).ToList();
            var postModel = _fixture.Build<CreatePostModel>().With(x => x.PaymentType, "Subscription").With(x => x.productModel, productModel).Create();
            var post = _mapper.Map<Post>(postModel);
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(Guid.Parse("981b9606-4f84-41b4-8a46-7b578bc1823d"));
            _unitOfWorkMock.Setup(unit => unit.PostRepository.AddAsync(post)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.ProductRepository.AddAsync(product)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(unit => unit.SubscriptionHistoryRepository.GetUserPruchaseSubscription(It.IsAny<Guid>())).ReturnsAsync(subscriptionHistory);
            _uploadFileMock.Setup(upload => upload.UploadFileToFireBase(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("Testlink");
            var isCreated = await _postService.CreatePost(postModel);
            Assert.True(isCreated);
        }
    }
}
