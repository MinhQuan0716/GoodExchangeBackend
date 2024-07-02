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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infratructure.Test.RepositoryTest
{
    public class PostRepositoryTest:SetupTest
    {
        private readonly IPostRepository _postRepository;
        public PostRepositoryTest()
        {
            _postRepository = new PostRepository(_dbContext,_claimServiceMock.Object, _currentTimeMock.Object, _connectionMock.Object);
        }
        [Fact]
        public async Task GetAllPostWithDetail_ShouldReturnTrue()
        {
            var product = _fixture.Build<Product>().Create();
            var post = _fixture.Build<Post>().With(x => x.ProductId, product.Id).CreateMany(5);
            await _dbContext.Posts.AddRangeAsync(post);
            _dbContext.SaveChanges();
            var findPost = await _postRepository.GetAllPostsWithDetailsAsync();
            var originalProductIds = post.Select(p => p.ProductId).ToList();
            var retrievedProductIds = findPost.Select(fp => fp.Product.Id).ToList();
            Assert.Equal(originalProductIds, retrievedProductIds);
        }
        [Fact]
        public async Task SortPostByCreationDate_ShouldReturnCorrectData()
        {
            var post = _fixture.Build<Post>().CreateMany(5);
            await _dbContext.Posts.AddRangeAsync(post);
            _dbContext.SaveChanges();
            var sortedPost = await _postRepository.GetAllPostsWithDetailsSortByCreationDayAsync();
            Assert.Equal(post.OrderBy(x => x.CreationDate), sortedPost);
        }
    }
}
