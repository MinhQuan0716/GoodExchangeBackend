using Application.ViewModel.PostModel;
using AutoFixture;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Backend.Domain.Test
{
    public class PostRepositoryTests : SetupTest
    {
        private readonly PostRepository _postRepository;
        private readonly Guid _guid = Guid.Parse("be8844bc-fc8e-4f66-9c28-1b7254cf8b88");
        private readonly ITestOutputHelper _output;
        public PostRepositoryTests(ITestOutputHelper output)
        {
            _output = output;
            _postRepository = new PostRepository(_dbContext, _claimServiceMock.Object, _currentTimeMock.Object, _connectionMock.Object);
        }
        [Fact]
        public async Task GetAllPostWithDetail_ShouldReturnTrue()
        {
            var product = _fixture.Build<Product>().With(x=>x.Id, _guid).Create();
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
        [Fact]
        public async Task SearchPostByProductName_ShouldReturnListOfPostViewModels()
        {
            var postTitle = "Test Title";
            var product = _fixture.Build<Product>()
                                  .With(x => x.Id, _guid)
                                  .Create();

            var post = _fixture.Build<Post>()
                               .With(p => p.PostTitle, postTitle)
                               .With(x => x.ProductId, product.Id)
                               .Create();

            await _dbContext.Posts.AddAsync(post);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _postRepository.SearchPostByProductName(postTitle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(postTitle, result.First().PostTitle);
        }
        [Fact]
        public async Task SortPostByProductCategoryAsync_ShouldReturnCorrectlySortedPosts()
        {
            var categoryId = 1;
            var category1 = _fixture.Build<Category>().With(c => c.CategoryId, categoryId).With(c => c.CategoryName, "Category 1").Create();
            var category2 = _fixture.Build<Category>().With(c => c.CategoryId, 2).With(c => c.CategoryName, "Category 2").Create();
            var conditionType = _fixture.Build<ExchangeCondition>().With(c => c.ConditionId, 1).With(c => c.ConditionType, "New").Create();

            var postsWithCategory1 = _fixture.Build<Post>()
                                             .With(p => p.Product, _fixture.Build<Product>()
                                                                           .With(pr => pr.CategoryId, 1)
                                                                           .With(pr => pr.ConditionType, conditionType)
                                                                           .Create())
                                             .CreateMany(5).ToList();

            var postsWithCategory2 = _fixture.Build<Post>()
                                             .With(p => p.Product, _fixture.Build<Product>()
                                                                           .With(pr => pr.CategoryId, 2)
                                                                           .With(pr => pr.ConditionType, conditionType)
                                                                           .Create())
                                             .CreateMany(5).ToList();
            await _dbContext.Posts.AddRangeAsync(postsWithCategory1);
            await _dbContext.Posts.AddRangeAsync(postsWithCategory2);
            await _dbContext.SaveChangesAsync();

            // Verify setup
            var allPosts = await _dbContext.Posts.Include(p => p.Product).ThenInclude(pr => pr.Category).ToListAsync();
            _output.WriteLine($"Total Posts: {allPosts.Count}");
            foreach (var post in allPosts)
            {
                _output.WriteLine($"Post ID: {post.Id}, Category ID: {post.Product.Category.CategoryId}");
            }

            Assert.Equal(15, allPosts.Count);

            // Act
            var result = await _postRepository.SortPostByProductCategoryAsync(categoryId);
            _output.WriteLine($"Filtered Posts Count: {result.Count}");
            foreach (var post in result)
            {
                _output.WriteLine($"Filtered Post ID: {post.Id}, Category ID: {post.Product.Category.CategoryId}");
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count);
            Assert.All(result, p => Assert.Equal(categoryId, p.Product.Category.CategoryId));
        }
    }
}

