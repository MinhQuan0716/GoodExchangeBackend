using Application.ViewModel.VerifyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public  interface IVerifyUserService
    {
        Task<List<VerifyViewModel>> GetAllWaitingUserToApproveAsync();
    }
}
