using Application.ViewModel.PostModel;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Backend.Domain.Test
{
    public class PostRepositoryTests : SetupTest
    {
        private readonly PostRepository _postRepository;

        public PostRepositoryTests()
        {
            _postRepository = new PostRepository(_dbContext, _claimServiceMock.Object, _currentTimeMock.Object, _connectionMock.Object);
        }

        /*[Fact]
        public async Task GetAllPost_ShouldReturnListOfPostViewModels()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                ProductImageUrl = "url1",
                ProductPrice = 100,
                Category = new Category { CategoryId = 1, CategoryName = "Category 1" },
                ConditionType = new ExchangeCondition { ConditionId = 1, ConditionType = "New" }
            };
            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                ProductImageUrl = "url2",
                ProductPrice = 200,
                Category = new Category { CategoryId = 2, CategoryName = "Category 2" },
                ConditionType = new ExchangeCondition { ConditionId = 2, ConditionType = "Used" }
            };
            var posts = new List<Post>
            {
                new Post { Id = Guid.NewGuid(), PostTitle = "Post 1", PostContent = "Content 1", CreatedBy = Guid.NewGuid(), IsDelete = false, Product = product1 },
                new Post { Id = Guid.NewGuid(), PostTitle = "Post 2", PostContent = "Content 2", CreatedBy = userId, IsDelete = false, Product = product2 }
            };

            _dbContext.Posts.AddRange(posts);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _postRepository.GetAllPost(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Post 1", result[0].PostTitle);
        }
        [Fact]
        public async Task GetPostDetail_ShouldReturnPostDetailViewModel()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var product = new Product
            {
                Id = Guid.NewGuid(),
                ProductImageUrl = "url1",
                ProductPrice = 100,
                Category = new Category { CategoryId = 1, CategoryName = "Category 1" },
                ConditionType = new ExchangeCondition { ConditionId = 1, ConditionType = "New" }
            };
            var post = new Post { Id = postId, PostTitle = "Post 1", PostContent = "Content 1", CreatedBy = Guid.NewGuid(), IsDelete = false, Product = product };

            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _postRepository.GetPostDetail(postId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(postId, result.PostId);
            Assert.Equal("Post 1", result.PostTitle);
        }
        [Fact]
        public async Task SearchPostByProductName_ShouldReturnListOfPostViewModels()
        {
            // Arrange
            var productName = "Post 1";
            var product = new Product
            {
                Id = Guid.NewGuid(),
                ProductImageUrl = "url1",
                ProductPrice = 100,
                ProductStatus = "Test Product",
                Category = new Category { CategoryId = 1, CategoryName = "Category 1" },
                ConditionType = new ExchangeCondition { ConditionId = 1, ConditionType = "New" }
            };
            var post = new Post { Id = Guid.NewGuid(), PostTitle = "Post 1", PostContent = "Content 1", CreatedBy = Guid.NewGuid(), IsDelete = false, Product = product };

            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _postRepository.SearchPostByProductName(productName);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Product", result[0].PostTitle);
        }*/

    }
}
