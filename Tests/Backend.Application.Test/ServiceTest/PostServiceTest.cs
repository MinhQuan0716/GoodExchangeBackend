using Application.InterfaceService;
using Application.Service;
using Application.ViewModel.PostModel;
using Application.ViewModel.ProductModel;
using Application.ViewModel.SubscriptionHistoryModel;
using Application.ViewModel.UserViewModel;
using Application.ViewModel.VerifyModel;
using Application.ViewModel.WishListModel;
using AutoFixture;
using Backend.Domain.Test;
using Domain.Entities;
using FluentAssertions;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Application.Test.ServiceTest
{
    public class PostServiceTest : SetupTest
    {
        private IPostService _postService;
        public PostServiceTest()
        {
            _postService = new PostService(_unitOfWorkMock.Object, _mapper, _appConfiguration.Object, _currentTimeMock.Object, _claimServiceMock.Object, _uploadFileMock.Object, _backgroundJobClientMock.Object,_cacheServiceMock.Object);
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
        public async Task UnbanPost_ShouldReturnCorrect()
        {
            //Arrange
            var post = _fixture.Build<Post>().With(x=>x.IsDelete,true).Create();
            //Act
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetBannedPostById(post.Id)).ReturnsAsync(post);
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            bool isUnbanned = await _postService.UnbanPost(post.Id);
            //Assert
            Assert.True(isUnbanned);
        }
        [Fact]
        public async Task UnbanPost_ShouldReturnException()
        {
            //Arrange
            var post = _fixture.Build<Post>().Create();
            //Act
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetBannedPostById(It.IsAny<Guid>())).ReturnsAsync((Post)null);
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            //Assert
            Assert.ThrowsAsync<Exception>(async () => await _postService.BanPost(post.Id));
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
      /*  [Fact]
        public async Task CreatePost_WithWalletOption_ShouldBeSucceeded()
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
            var verifyModel = _fixture.Build<VerifyViewModel>().With(x => x.VerifyStatus, "Accept").Create();
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
            _unitOfWorkMock.Setup(unit => unit.VerifyUsersRepository.GetVerifyUserDetailByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync(verifyModel);
            _uploadFileMock.Setup(upload => upload.UploadFileToFireBase(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("Testlink");
            var isCreated = await _postService.CreatePost(postModel);
            Assert.True(isCreated);
        }
        [Fact]
        public async Task CreatePost_WithSubscriptionOption_ShouldBeSucceeded()
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
            var verifyModel = _fixture.Build<VerifyViewModel>().With(x => x.VerifyStatus, "Accept").Create();
            var productModel = _fixture.Build<CreateProductModel>().With(x => x.ProductImage, productFile).Create();
            var product = _mapper.Map<Product>(productModel);
            var subscriptionHistory = _fixture.Build<SubscriptionHistoryDetailViewModel>()
                .With(x => x.StartDate,DateTime.UtcNow)
                .With(x => x.Status, "Available")
                .With(x => x.SubscriptionId, new Guid())
                .With(x => x.EndDate,DateTime.UtcNow.AddDays(1)).CreateMany(2).ToList();
            var subscription = _fixture.Build<Subscription>().With(x => x.Description, "priority").Create();
            var postModel = _fixture.Build<CreatePostModel>().With(x => x.PaymentType, "Subscription").With(x => x.productModel, productModel).Create();
            var post = _mapper.Map<Post>(postModel);
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(Guid.Parse("981b9606-4f84-41b4-8a46-7b578bc1823d"));
            _unitOfWorkMock.Setup(unit => unit.PostRepository.AddAsync(post)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.ProductRepository.AddAsync(product)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(unit => unit.SubscriptionHistoryRepository.GetUserPurchaseSubscription(It.IsAny<Guid>())).ReturnsAsync(subscriptionHistory);
            _unitOfWorkMock.Setup(unit => unit.VerifyUsersRepository.GetVerifyUserDetailByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync(verifyModel);
            _unitOfWorkMock.Setup(unit => unit.SubcriptionRepository.GetByIdAsync(subscriptionHistory.FirstOrDefault().SubscriptionId)).ReturnsAsync(subscription);
            _uploadFileMock.Setup(upload => upload.UploadFileToFireBase(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("Testlink");
            var isCreated = await _postService.CreatePost(postModel);
            Assert.True(isCreated);
        }
        [Fact]
        public async Task CreatePost_WithWalletOption_ShouldThrowException()
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
            var verifyModel = _fixture.Build<VerifyViewModel>().With(x => x.VerifyStatus, "Accept").Create();
            var productModel = _fixture.Build<CreateProductModel>().With(x => x.ProductImage, productFile).Create();
            var product = _mapper.Map<Product>(productModel);
            var wallet = _fixture.Build<Wallet>().With(x => x.UserBalance, 14000)
                                               .With(x => x.OwnerId, Guid.Parse("981b9606-4f84-41b4-8a46-7b578bc1823d")).Create();
            var postModel = _fixture.Build<CreatePostModel>().With(x => x.PaymentType, "Wallet").With(x => x.productModel, productModel).Create();
            var post = _mapper.Map<Post>(postModel);
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(Guid.Parse("981b9606-4f84-41b4-8a46-7b578bc1823d"));
            _unitOfWorkMock.Setup(unit => unit.PostRepository.AddAsync(post)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.ProductRepository.AddAsync(product)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(unit => unit.WalletRepository.GetUserWalletByUserId(It.IsAny<Guid>())).ReturnsAsync(wallet);
            _unitOfWorkMock.Setup(unit => unit.VerifyUsersRepository.GetVerifyUserDetailByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync(verifyModel);
            _uploadFileMock.Setup(upload => upload.UploadFileToFireBase(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("Testlink");
            Func<Task> act = async () => await _postService.CreatePost(postModel);
            act.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task CreatePost_WithSubscriptionOption_ShouldThrowException()
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
            var verifyModel = _fixture.Build<VerifyViewModel>().With(x => x.VerifyStatus, "Accept").Create();
            var productModel = _fixture.Build<CreateProductModel>().With(x => x.ProductImage, productFile).Create();
            var product = _mapper.Map<Product>(productModel);
            var postModel = _fixture.Build<CreatePostModel>().With(x => x.PaymentType, "Subscription").With(x => x.productModel, productModel).Create();
            var post = _mapper.Map<Post>(postModel);
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(Guid.Parse("981b9606-4f84-41b4-8a46-7b578bc1823d"));
            _unitOfWorkMock.Setup(unit => unit.PostRepository.AddAsync(post)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.ProductRepository.AddAsync(product)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(unit => unit.VerifyUsersRepository.GetVerifyUserDetailByUserIdAsync(It.IsAny<Guid>())).ReturnsAsync(verifyModel);
            _uploadFileMock.Setup(upload => upload.UploadFileToFireBase(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("Testlink");
            Func<Task> act = async () => await _postService.CreatePost(postModel);
            act.Should().ThrowAsync<Exception>();
        }*/
        [Fact]
        public async Task DeletePost_ShouldBeSucceeded()
        {
            var post = _fixture.Build<Post>().With(x => x.Id, Guid.Parse("68c5b643-fd14-45be-8ef6-884c1372ffa3")).Create();
            _unitOfWorkMock.Setup(unit => unit.PostRepository.AddAsync(post)).Verifiable();
            _unitOfWorkMock.Setup(unit=>unit.SaveChangeAsync()).ReturnsAsync(1);  
            _unitOfWorkMock.Setup(unit=>unit.PostRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);
            _unitOfWorkMock.Setup(unit=>unit.PostRepository.SoftRemove(It.IsAny<Post>())).Verifiable();
            bool isDelete = await _postService.DeletePost(Guid.Parse("68c5b643-fd14-45be-8ef6-884c1372ffa3"));
            Assert.True(isDelete);
        }
        [Fact]
        public async Task UpdatePost_ShouldBeSucceeded()
        {
            //Arrange
            var post = _fixture.Build<Post>().With(x => x.Id, Guid.Parse("68c5b643-fd14-45be-8ef6-884c1372ffa3")).Create();
            var product = _fixture.Build<Product>().With(x => x.Id, Guid.Parse("ecae1bd3-ccae-48b3-ba80-5cdb48486e3d")).Create();
            IFormFile productFile = null;
            string exePath = Environment.CurrentDirectory.ToString();
            string filePath = exePath + "/ImageFolder/Class Diagram-UpdatePost.drawio.png";
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
            var productModel = _fixture.Build<UpdateProductModel>()
                                      .With(x => x.ProductImage, productFile)
                                      .Create();
            var postModel = _fixture.Build<UpdatePostModel>()
                                    .With(x => x.PostId, Guid.Parse("68c5b643-fd14-45be-8ef6-884c1372ffa3"))
                                    .With(x=>x.productModel,productModel).Create();
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetProductIdFromPostId(It.IsAny<Guid>())).ReturnsAsync(Guid.Parse("ecae1bd3-ccae-48b3-ba80-5cdb48486e3d"));
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);
            _unitOfWorkMock.Setup(unit => unit.ProductRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(product);
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _uploadFileMock.Setup(upload => upload.UploadFileToFireBase(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("Update test link");
            var isUpdated = await _postService.UpdatePost(postModel);
            Assert.True(isUpdated);
        }
        [Fact]
        public async Task UpdatePost_ShouldThrowProductNotFoundException()
        {
            //Arrange
            var post = _fixture.Build<Post>().With(x => x.Id, Guid.Parse("68c5b643-fd14-45be-8ef6-884c1372ffa3")).Create();
            IFormFile productFile = null;
            string exePath = Environment.CurrentDirectory.ToString();
            string filePath = exePath + "/ImageFolder/Class Diagram-UpdatePost.drawio.png";
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
            var productModel = _fixture.Build<UpdateProductModel>()
                                      .With(x => x.ProductImage, productFile)
                                      .Create();
            var postModel = _fixture.Build<UpdatePostModel>()
                                    .With(x => x.PostId, Guid.Parse("68c5b643-fd14-45be-8ef6-884c1372ffa3"))
                                    .With(x => x.productModel, productModel).Create();
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetProductIdFromPostId(It.IsAny<Guid>())).ReturnsAsync(Guid.Parse("ecae1bd3-ccae-48b3-ba80-5cdb48486e3d"));
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(post);
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _uploadFileMock.Setup(upload => upload.UploadFileToFireBase(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("Update test link");
            Func<Task> act = async () => await _postService.UpdatePost(postModel);
            act.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task AddPostToWishList_ShouldBeSucceeded()
        {
            var testPost = _fixture.Build<Post>()
                                  .Create();
            var testListPost=_fixture.Build<Post>()
                                    .CreateMany(2).ToList();
          /*  testListPost.Add(testPost);*/
            var wishlistList = _fixture.Build<WishList>()
                                      .CreateMany(2).ToList();
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(Guid.Parse("cfc110bc-2537-4b0b-9d41-6cbf6d3dd12d"));
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(unit => unit.WishListRepository.FindWishListByPostId(It.IsAny<Guid>())).ReturnsAsync(wishlistList);
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetAllPostsByCreatedByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testListPost);
            bool isAdded = await _postService.AddPostToWishList(testPost.Id);
            Assert.True(isAdded);
        }
        [Fact]
        public async Task AddPostToWishList_ShouldThrowCannotAddYourOwnPostException()
        {
            var testPost = _fixture.Build<Post>()
                                  .Create();
            var testListPost = _fixture.Build<Post>()
                                    .CreateMany(2).ToList();
            testListPost.Add(testPost);
            var wishlistList = _fixture.Build<WishList>()
                                      .CreateMany(2).ToList();
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(Guid.Parse("cfc110bc-2537-4b0b-9d41-6cbf6d3dd12d"));
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(unit => unit.WishListRepository.FindWishListByPostId(It.IsAny<Guid>())).ReturnsAsync(wishlistList);
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetAllPostsByCreatedByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testListPost);
            Func<Task> act = async()=>await _postService.AddPostToWishList(testPost.Id);
            act.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task AddPostToWishList_ShouldThrowPostAlreadyInWishlistException()
        {
            var testPost = _fixture.Build<Post>()
                                  .Create();
            var testListPost = _fixture.Build<Post>()
                                    .CreateMany(2).ToList();
            testListPost.Add(testPost);
            var wishlistList = _fixture.Build<WishList>()
                                       .With(x=>x.UserId, Guid.Parse("cfc110bc-2537-4b0b-9d41-6cbf6d3dd12d"))
                                       .CreateMany(2).ToList();
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(Guid.Parse("cfc110bc-2537-4b0b-9d41-6cbf6d3dd12d"));
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(unit => unit.WishListRepository.FindWishListByPostId(It.IsAny<Guid>())).ReturnsAsync(wishlistList);
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetAllPostsByCreatedByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testListPost);
            Func<Task> act = async () => await _postService.AddPostToWishList(testPost.Id);
            act.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task CheckIfPostInWishList_ShouldReturnCorrect()
        {
            var currentUser = _fixture.Build<User>().With(x => x.Id, Guid.Parse("025d3a71-f990-4df8-b9f2-c0f17c8fce4d")).Create();
            var post = _fixture.Build<Post>().With(x => x.CreatedBy, currentUser.Id).Create();
            var postViewModel = _fixture.Build<PostViewModel>().With(x => x.PostId, post.Id).With(x=>x.CreationDate,DateOnly.FromDateTime(DateTime.UtcNow)).Create();
            var listWishList = _fixture.Build<WishListViewModel>().With(x=>x.post,postViewModel).CreateMany(2).ToList();
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(currentUser.Id);
            _unitOfWorkMock.Setup(unit => unit.WishListRepository.FindWishListByUserId(It.IsAny<Guid>())).ReturnsAsync(listWishList);
            var isExisted = await _postService.CheckIfPostInWishList(post.Id);
            Assert.True(isExisted);
        }
        [Fact]
        public async Task CheckIfPostInWishList_ShouldReturnFalseIfPostDoNotExist()
        {
            var currentUser = _fixture.Build<User>().With(x => x.Id, Guid.Parse("025d3a71-f990-4df8-b9f2-c0f17c8fce4d")).Create();
            var post = _fixture.Build<Post>().With(x => x.CreatedBy, currentUser.Id).Create();
            var postViewModel = _fixture.Build<PostViewModel>().With(x => x.CreationDate, DateOnly.FromDateTime(DateTime.UtcNow)).Create();
            var listWishList = _fixture.Build<WishListViewModel>().With(x => x.post, postViewModel).CreateMany(2).ToList();
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(currentUser.Id);
            _unitOfWorkMock.Setup(unit => unit.WishListRepository.FindWishListByUserId(It.IsAny<Guid>())).ReturnsAsync(listWishList);
            var isExisted = await _postService.CheckIfPostInWishList(post.Id);
            Assert.False(isExisted);
        }
        [Fact]
        public async Task SearchAndFilterPost_ShouldReturnCorrectData_WhenSearchByPostTitle()
        {
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
                    PostTitle = "Test post",
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
            var listSearchPost = await _postService.SearchPostByPostTitleAndFilterPostByProductStatusAndPrice("Test",null,null);
            Assert.Equal(listSearchPost.Count(), 2);
        }
        [Fact]
        public async Task SearchAndFilterPost_ShouldReturnCorrectData_WhenSearchByPostTitleFilterByProductStatus()
        {
            var posts = _fixture.Build<Post>().CreateMany(2).ToList();
            var product = _fixture.Build<Product>().With(x=>x.ProductStatus,"New").Create();
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
                    PostTitle = "Test post",
                    Product = new ProductModel()
                    {
                        CategoryId = product.CategoryId,
                        CategoryName = "",
                        ConditionId = product.ConditionId,
                        ConditionName = "",
                        ProductId = product.Id,
                        ProductImageUrl = product.ProductImageUrl,
                        ProductPrice = product.ProductPrice,
                        ProductStatus =product.ProductStatus,
                        RequestedProduct = ""
                    }
                };
                list.Add(postViewModel);
            }
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(Guid.Parse("981b9606-4f84-41b4-8a46-7b578bc1823d"));
            _unitOfWorkMock.Setup(unit => unit.PostRepository.AddRangeAsync(posts.ToList())).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(unit => unit.PostRepository.GetAllPost(It.IsAny<Guid>())).ReturnsAsync(list);
            var listSearchPost = await _postService.SearchPostByPostTitleAndFilterPostByProductStatusAndPrice("Test", product.ProductStatus, null);
            Assert.Equal(listSearchPost.Count(), 2);
        }
        [Fact]
        public async Task SearchAndFilterPost_ShouldReturnCorrectData_WhenSearchByPostTileFilterByExchangeCondition()
        {
            var posts = _fixture.Build<Post>().CreateMany(2).ToList();
            var exchangeCondition = _fixture.Build<ExchangeCondition>()
                                         .With(x => x.ConditionId, 1)
                                         .With(x => x.ConditionId, 2)
                                         .With(x => x.ConditionId, 3)
                                         .With(x => x.ConditionType, "Sell")
                                         .With(x => x.ConditionType, "Exchange")
                                         .With(x => x.ConditionType, "Donation")
                                         .CreateMany(3).ToList();
            var product = _fixture.Build<Product>().With(x=>x.ConditionType,exchangeCondition.Where(x=>x.ConditionId==3).First()).Create();
           
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
                    PostTitle = "Test post",
                    Product = new ProductModel()
                    {
                        CategoryId = product.CategoryId,
                        CategoryName = "",
                        ConditionId = product.ConditionId,
                        ConditionName = product.ConditionType.ConditionType,
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
            var listSearchPost = await _postService.SearchPostByPostTitleAndFilterPostByProductStatusAndPrice("Test", null, exchangeCondition.Where(x => x.ConditionId == 3).First().ConditionType);
            Assert.Equal(listSearchPost.Count(), 2);
        }
        [Fact]
        public async Task SearchAndFilterPost_ShouldReturnCorrectData_WhenSearchByPostTileFilterByExchangeConditionAndProductStatus()
        {
            var posts = _fixture.Build<Post>().CreateMany(2).ToList();
            var exchangeCondition = _fixture.Build<ExchangeCondition>()
                                         .With(x => x.ConditionId, 1)
                                         .With(x => x.ConditionId, 2)
                                         .With(x => x.ConditionId, 3)
                                         .With(x => x.ConditionType, "Sell")
                                         .With(x => x.ConditionType, "Exchange")
                                         .With(x => x.ConditionType, "Donation")
                                         .CreateMany(3).ToList();
            var product = _fixture.Build<Product>()
                                  .With(x => x.ConditionType, exchangeCondition.Where(x => x.ConditionId == 3).First())
                                  .With(x=>x.ProductStatus,"New").Create();

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
                    PostTitle = "Test post",
                    Product = new ProductModel()
                    {
                        CategoryId = product.CategoryId,
                        CategoryName = "",
                        ConditionId = product.ConditionId,
                        ConditionName = product.ConditionType.ConditionType,
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
            var listSearchPost = await _postService.SearchPostByPostTitleAndFilterPostByProductStatusAndPrice("Test", product.ProductStatus, exchangeCondition.Where(x => x.ConditionId == 3).First().ConditionType);
            Assert.Equal(listSearchPost.Count(), 2);
        }
    }
}
