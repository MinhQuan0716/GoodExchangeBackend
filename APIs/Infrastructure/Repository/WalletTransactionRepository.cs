﻿using Application.InterfaceRepository;
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
    public class WalletTransactionRepository : GenericRepository<WalletTransaction>, IWalletTransactionRepository
    {
        private readonly AppDbContext _appDbContext;
        public WalletTransactionRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Guid> GetLastSaveWalletTransactionId()
        {
            var lasSaveWalletTransaction =  await _appDbContext.WalletTransactions.Where(x => x.IsDelete == false)
                                                         .OrderBy(x => x.CreationDate)
                                                         .LastAsync();
            return lasSaveWalletTransaction.Id;
        }
    }
}