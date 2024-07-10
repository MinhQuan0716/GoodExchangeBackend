
﻿using Application.ViewModel.VerifyModel;
﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IVerifyUserService
    {
        Task<List<VerifyViewModel>> GetAllWaitingUserToApproveAsync();
        //Task<bool> UploadImage(IFormFile ImageVerify);
        Task<bool> UploadImageForVerifyUser(IFormFile userImage);
        Task<bool> ApproveImageAsync(Guid verifyId);
        Task<bool> DenyImageAsync(Guid verifyId);
    }
}
