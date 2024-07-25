using Application.InterfaceRepository;
using Application.ViewModel.PostModel;
using AutoFixture;
using Backend.Domain.Test;
using Dapper;
using Domain.Entities;
using Infrastructure.Repository;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infratructure.Test.RepositoryTest
{
    public class PostRepositoryTest : SetupTest
    {
        private readonly IPostRepository _postRepository;
        public PostRepositoryTest()
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
        public async Task SortPostByCreationDate_ShouldReturnCorrectData()
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
    }
    }

