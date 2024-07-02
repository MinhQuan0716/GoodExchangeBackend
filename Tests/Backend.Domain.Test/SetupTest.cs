using Application;
using Application.Common;
using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.Util;
using AutoFixture;
using AutoMapper;
using Infrastructure;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Backend.Domain.Test
{
    public class SetupTest : IDisposable
    {
        protected readonly AppDbContext _dbContext;
        protected readonly Fixture _fixture;
        protected readonly IMapper _mapper;
        protected readonly Mock<IUserRepository> _userRepositoryMock;
        protected readonly Mock<IClaimService> _claimServiceMock;
        protected readonly Mock<IUnitOfWork> _unitOfWorkMock;
        protected readonly Mock<ICurrentTime> _currentTimeMock;
        protected readonly Mock<ICacheService> _cacheServiceMock;
        protected readonly Mock<ISendMailHelper> _sendMailHelperMock;
        protected readonly Mock<AppConfiguration> _appConfiguration;
        protected readonly Mock<IPostRepository> _postRepositoryMock;
        protected readonly Mock<IUploadFile> _uploadFileMock;
        protected readonly Mock<IDbConnection> _connectionMock;
        public SetupTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;
            _dbContext = new AppDbContext(options);
            var mapConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperConfig());
            });
            _mapper=mapConfig.CreateMapper();
            _fixture=new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _claimServiceMock = new Mock<IClaimService>();
            _currentTimeMock=new Mock<ICurrentTime>();
            _cacheServiceMock=new Mock<ICacheService>();
            _appConfiguration = new Mock<AppConfiguration>();
            _claimServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.Empty);
            _currentTimeMock.Setup(x=>x.GetCurrentTime()).Returns(DateTime.UtcNow);
            _sendMailHelperMock=new Mock<ISendMailHelper>();
            _postRepositoryMock = new Mock<IPostRepository>();
            _uploadFileMock=new Mock<IUploadFile>();
            _connectionMock=new Mock<IDbConnection>();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public bool IsSorted<T>(IList<T> list) where T : IComparable<T>
        {
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i - 1].CompareTo(list[i]) > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
