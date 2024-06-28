using Application.InterfaceRepository;
using Application.InterfaceService;
using Backend.Domain.Test;
using FluentAssertions;
using Infrastructure;
using Application.CacheService;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MobileAPI;
using MobileAPI.MobileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;

namespace Backend.Infratructure.Test
{
    public class DependencyInjectionTest:SetupTest
    {
        private readonly IServiceProvider _serviceProvider;
        public DependencyInjectionTest()
        {
            var service=new ServiceCollection();
            service.AddInfrastructureService("Server=Test;uid=Tester;password=Tested;Database=TestDb;TrustServerCertificate=True;Encrypt=false;MultipleActiveResultSets=true");
            service.AddDbContext<AppDbContext>(
               option => option.UseInMemoryDatabase("test"));
            service.AddMobileAPIService("Test","TestMobile");
            service.AddWebAPIService("Test", "TestWeb");   
            _serviceProvider = service.BuildServiceProvider();
        }
        [Fact]
        public void GetService_ShouldRetunCorrectType()
        {
            var userRepositoryResolved = _serviceProvider.GetRequiredService<IUserRepository>();
            var postRepositoryResolved= _serviceProvider.GetRequiredService<IPostRepository>();
            var productRepositoryResolved=_serviceProvider.GetRequiredService<IProductRepository>();
            userRepositoryResolved.GetType().Should().Be(typeof(UserRepository));
            postRepositoryResolved.GetType().Should().Be(typeof (PostRepository));
            productRepositoryResolved.GetType().Should().Be(typeof(ProductRepository)); 
        }
    }
}
