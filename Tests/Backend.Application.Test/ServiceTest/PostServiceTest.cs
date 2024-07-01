using Application.InterfaceService;
using Application.Service;
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
    public class PostServiceTest:SetupTest
    {
        private IPostService _postService;
        public PostServiceTest()
        {
            _postService=new PostService(_unitOfWorkMock.Object,_mapper,_appConfiguration.Object,_currentTimeMock.Object,_claimServiceMock.Object,_uploadFileMock.Object);
        }
        [Fact]
        public async Task BanPost_ShouldReturnCorrect()
        {
            //Arrange
            var post = _fixture.Build<Post>().Create();
            //Act
            _unitOfWorkMock.Setup(unit=>unit.PostRepository.GetByIdAsync(post.Id)).ReturnsAsync(post);
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
            Assert.ThrowsAsync<Exception>(async()=>await _postService.BanPost(Guid.NewGuid()));    
        }

    }
}
