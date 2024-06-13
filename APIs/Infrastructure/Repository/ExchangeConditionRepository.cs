using Application.InterfaceRepository;
using Application.InterfaceService;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ExchangeConditionRepository :  IExchangeConditionRepository
    {
        private readonly AppDbContext _dbContext;
        public ExchangeConditionRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }
        public async Task<List<ExchangeCondition>> GetAllExchangeConditionAsync()
        {
            var exchangeConditions = await _dbContext.ExchangeConditions.ToListAsync();
            return exchangeConditions;
        }
    }
}
