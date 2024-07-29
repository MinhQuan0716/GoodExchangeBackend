using Application.InterfaceService;
using Application.Service;
using Application.ViewModel.PostModel;
using AutoFixture;
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
            var pagintaedPost = await _postService.GetAllPost(0, 1);
            Assert.Equal(pagintaedPost.TotalItemsCount, 2);
        }
        /*  [Fact]
          public async Task FilterByProductStatus_ShouldReturnCorrectData()
          {
              var posts = _fixture.Build<Post>().CreateMany(2);
              var product = _fixture.Build<Product>().With(p => p.ProductStatus, "New").Create();
              List<PostViewModel> list = new List<PostViewModel>();
              foreach (var post in posts)
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
              _unitOfWorkMock.Setup(unit => unit.PostRepository.AddRangeAsync(posts.ToList())).Verifiable();
              _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
              _unitOfWorkMock.Setup(unit => unit.PostRepository.GetAllPost()).ReturnsAsync(list);
              var filterPost = await _postService.FilterPostByProductStatusAndPrice("New", null, 0, 2);
              Assert.Equal(filterPost.TotalItemsCount, 2);
          }*/

        /* [Fact]
         public async Task FilterByExchangeCondition_ShouldReturnCorrectData()
         {
             var posts = _fixture.Build<Post>().CreateMany(2);
             var condition = _fixture.Build<ExchangeCondition>().With(ex => ex.ConditionType, "For selling").Create();
             var product = _fixture.Build<Product>().With(p => p.ProductStatus, "New").With(p=>p.ConditionType,condition).Create();

             List<PostViewModel> list = new List<PostViewModel>();
             foreach (var post in posts)
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
             _unitOfWorkMock.Setup(unit => unit.PostRepository.AddRangeAsync(posts.ToList())).Verifiable();
             _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
             _unitOfWorkMock.Setup(unit => unit.PostRepository.GetAllPost()).ReturnsAsync(list);
             var filterPost = await _postService.FilterPostByProductStatusAndPrice(null, "For selling", 0, 2);
             Assert.Equal(filterPost.TotalItemsCount, 2);
         }*/
    }
}
