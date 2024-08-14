using Application.InterfaceService;
using Application.ViewModel.PolicyModel;
using AutoMapper;
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
        public async Task<OrderCancelledTimeViewModel> GetOrderCancelledTime()
        {
            var policy = await _unitOfWork.PolicyRepository.GetAllAsync();
            var policyViewModel=_mapper.Map<List<OrderCancelledTimeViewModel>>(policy);
            return policyViewModel.First();
        }

        public async Task<PostPriceViewModel> GetPostPrice()
        {
            var policy = await _unitOfWork.PolicyRepository.GetAllAsync();
            var policyViewModel = _mapper.Map<List<PostPriceViewModel>>(policy);
            return policyViewModel.First();
        }

        public async Task<bool> UpdateOrderCancelledTime(OrderCancelledTimeViewModel orderCancelledTimeViewModel)
        {
            var foundPolicy=await _unitOfWork.PolicyRepository.GetByIdAsync(orderCancelledTimeViewModel.Id);
            if (foundPolicy!=null)
            {
                foundPolicy.OrderCancelledAmount= orderCancelledTimeViewModel.OrderCancelledAmount;  
                _unitOfWork.PolicyRepository.Update(foundPolicy);
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> UpdatePostPrice(PostPriceViewModel postPriceViewModel)
        {
            var foundPolicy = await _unitOfWork.PolicyRepository.GetByIdAsync(postPriceViewModel.Id);
            if (foundPolicy != null)
            {
                foundPolicy.PostPrice = postPriceViewModel.PostPrice;
                _unitOfWork.PolicyRepository.Update(foundPolicy);
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
    }
}
