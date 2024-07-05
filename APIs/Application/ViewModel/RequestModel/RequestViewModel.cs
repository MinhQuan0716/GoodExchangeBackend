﻿using Application.ViewModel.PostModel;
using Application.ViewModel.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.RequestModel
{
    public class ReceiveRequestViewModel
    {
        public Guid RequestId { get; set; }
        public string RequestMessage { get; set; }
        public DateOnly CreationDate { get; set; }
        public PostViewModelForRequest Post { get; set; }
        public UserViewModelForRequest User{ get; set; }
    }
}
