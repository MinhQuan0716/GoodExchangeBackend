using Application.ViewModel.PostModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IExchangeConditionService
    {
        Task<List<ExchangeCondition>> GetAllExchangeCondition();
    }
}
