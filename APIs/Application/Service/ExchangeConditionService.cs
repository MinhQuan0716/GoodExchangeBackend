using Application.Common;
using Application.InterfaceService;
using Application.Util;
using Application.ViewModel.PostModel;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ExchangeConditionService : IExchangeConditionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExchangeConditionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ExchangeCondition>> GetAllExchangeCondition()
        {
            var exchangeConditions = await _unitOfWork.ExchangeConditionRepository.GetAllExchangeConditionAsync();
            return exchangeConditions;
        }
    }
}
