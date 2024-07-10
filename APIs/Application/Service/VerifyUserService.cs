using Application.InterfaceService;
using Application.ViewModel.VerifyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class VerifyUserService : IVerifyUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public VerifyUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<VerifyViewModel>> GetAllWaitingUserToApproveAsync()
        {
           return await _unitOfWork.VerifyUsersRepository.GetAllVerifyUserAsync();
        }
    }
}
