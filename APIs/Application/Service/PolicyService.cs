using Application.InterfaceService;
using Application.ViewModel.PolicyModel;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class PolicyService : IPolicyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PolicyService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<OrderCancelledTimeViewModel>> GetOrderCancelledTime()
        {
            var policy = await _unitOfWork.PolicyRepository.GetAllAsync();
            var policyViewModel=_mapper.Map<List<OrderCancelledTimeViewModel>>(policy);
            return policyViewModel;
        }

        public async Task<List<PostPriceViewModel>> GetPostPrice()
        {
            var policy = await _unitOfWork.PolicyRepository.GetAllAsync();
            var policyViewModel = _mapper.Map<List<PostPriceViewModel>>(policy);
            return policyViewModel;
        }

        public async Task<bool>  UpdateOrderCancelledTime(UpdateOrderCancelledTimeModel updateOrderCancelledTimeModel)
        {
            var foundPolicy=await _unitOfWork.PolicyRepository.GetByIdAsync(updateOrderCancelledTimeModel.Id);
            _mapper.Map(updateOrderCancelledTimeModel,foundPolicy,typeof(UpdateOrderCancelledTimeModel),typeof(Policy));
            _unitOfWork.PolicyRepository.Update(foundPolicy);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> UpdatePostPrice(UpdatePostPriceModel updatePostPriceModel)
        {
            var foundPolicy = await _unitOfWork.PolicyRepository.GetByIdAsync(updatePostPriceModel.Id);
            _mapper.Map(updatePostPriceModel, foundPolicy, typeof(UpdatePostPriceModel), typeof(Policy));
            _unitOfWork.PolicyRepository.Update(foundPolicy);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
    }
}
