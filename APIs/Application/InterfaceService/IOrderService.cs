﻿using Application.ViewModel.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IOrderService
    {
        Task<bool>SendRequest(CreateOrderModel requestModel);
        Task<List<ReceiveOrderViewModel>> GetAllRequestsOfCurrentUserAsync();
        Task<List<SentOrderViewModel>> GetAllRequestsOfCreatebByUserAsync();
        Task<bool> AcceptRequest(Guid requestId);
        Task<bool> RejectRequest(Guid requestId);
    }
}