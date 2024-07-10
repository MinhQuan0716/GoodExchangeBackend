using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ViewModel.VerifyModel;
namespace Application.InterfaceRepository
{
    public interface IVerifyUsersRepository : IGenericRepository<VerifyUser>
    {
        Task<VerifyUser> FindVerifyUserIdByUserId(Guid userId);
        Task<List<VerifyViewModel>> GetAllVerifyUserAsync();
    }
}
