using Application.InterfaceService;
using Application.Service;
using Application.ViewModel.SubcriptionModel;
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
    public  class SubscriptionServiceTest:SetupTest
    {
        private ISubcriptionService _subscriptionService;
        public SubscriptionServiceTest()
        {
            _subscriptionService = new SubcriptionService(_unitOfWorkMock.Object,_mapper,_claimServiceMock.Object);
        }
        [Fact]
        public async Task CreateSubscription_ShouldSucceed()
        {
            var createSubscriptionModel = _fixture.Build<CreateSubcriptionModel>().Create();
            _unitOfWorkMock.Setup(unit=>unit.SubcriptionRepository.AddAsync(It.IsAny<Subscription>())).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            bool isCreated = await _subscriptionService.CreateSubcription(createSubscriptionModel);
            Assert.True(isCreated);
        }
        [Fact]
        public async Task DeactvieSubscription_ShouldSuceed()
        {
            var subscription = _fixture.Build<Subscription>().Create();
            _unitOfWorkMock.Setup(unit => unit.SubcriptionRepository.SoftRemove(It.IsAny<Subscription>())).Verifiable();
            _unitOfWorkMock.Setup(unit=>unit.SubcriptionRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(subscription);  
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            bool isDeactive = await _subscriptionService.DeactiveSubscriptionAsync(subscription.Id);
            Assert.True(isDeactive);
        }
    }
}
