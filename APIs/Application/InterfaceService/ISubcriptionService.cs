﻿using Application.ViewModel.SubcriptionModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface ISubcriptionService
    {
        Task<bool> CreateSubcription(CreateSubcriptionModel createSubcriptionModel);
        Task<List<Subcription>> GetAllSubscriptionAsync();
        Task<bool> ExtendSubscription();
    }
}
