using Application.ViewModel.PostModel;
using AutoFixture;
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
        [Fact]
        public async Task GetAllPostWithDetail_ShouldReturnTrue()
        {
            var product = _fixture.Build<Product>().With(x=>x.Id, Guid.Parse("be8844bc-fc8e-4f66-9c28-1b7254cf8b88")).Create();
            var listPost = _fixture.Build<Post>().CreateMany(5).ToList();
            var newPostWithProductId=_fixture.Build<Post>().With(x=>x.ProductId,product.Id).Create();
            listPost.Add(newPostWithProductId);
            await _dbContext.Posts.AddRangeAsync(listPost);
            _dbContext.SaveChanges();
            var findPost = await _postRepository.GetAllPostsWithDetailsAsync();
            var originalProductIds = listPost.Select(p => p.ProductId).ToList();
            var retrievedProductIds = findPost.Select(fp => fp.Product.Id).ToList();
            Assert.True(retrievedProductIds.All(id => originalProductIds.Contains(id)));
        }
        [Fact]
        public async Task GetPostDetail_ShouldReturnPostDetailViewModel()
        {
            // Create and add posts to the database
            var posts = _fixture.Build<Post>()
                                .With(p => p.CreationDate, DateTime.UtcNow)
                                .CreateMany(5)
                                .ToList();

            await _dbContext.Posts.AddRangeAsync(posts);
            await _dbContext.SaveChangesAsync();

            // Retrieve sorted posts
            var sortedPosts = await _postRepository.GetAllPostsWithDetailsSortByCreationDayAsync();

            // Log the CreationDate values before and after sorting for debugging
            var expectedSortedDates = posts.OrderBy(x => x.CreationDate).Select(x => x.CreationDate).ToList();
            var actualSortedDates = sortedPosts.Select(x => x.CreationDate).ToList();
            // Compare the CreationDate of the posts to ensure sorting is correct
            Assert.Equal(expectedSortedDates.Last(), actualSortedDates.Last());
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

