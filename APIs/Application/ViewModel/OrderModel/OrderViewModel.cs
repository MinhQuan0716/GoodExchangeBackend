﻿using Application.ViewModel.PostModel;
using Application.ViewModel.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.RequestModel
{
    public class ReceiveOrderViewModel
    {
        public Guid OrderId { get; set; }
        public string OrderMessage { get; set; }
        public string OrderStatus { get; set; }
        public DateOnly CreationDate { get; set; }
        public PostViewModelForRequest Post { get; set; }
        public UserViewModelForRequest User{ get; set; }
    }
}