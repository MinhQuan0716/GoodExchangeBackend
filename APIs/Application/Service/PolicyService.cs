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

        public async Task<bool> UpdateOrderCancelledTime(Guid id, int orderAmount)
        {
            var foundPolicy=await _unitOfWork.PolicyRepository.GetByIdAsync(id);
            if (foundPolicy!=null)
            {
                foundPolicy.OrderCancelledAmount= orderAmount;  
                _unitOfWork.PolicyRepository.Update(foundPolicy);
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> UpdatePostPrice(Guid id, float postPrice)
        {
            var foundPolicy = await _unitOfWork.PolicyRepository.GetByIdAsync(id);
            if (foundPolicy != null)
            {
                foundPolicy.PostPrice = postPrice;
                _unitOfWork.PolicyRepository.Update(foundPolicy);
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
    }
}
