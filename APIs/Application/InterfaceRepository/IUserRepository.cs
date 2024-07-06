using Application.ViewModel.UserModel;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceRepository
{
    public interface IUserRepository:IGenericRepository<User>
    {
        Task<User> FindUserByEmail (string email);
        Task UpdateUserAsync(User user);
        Task<CurrentUserModel> GetCurrentLoginUserAsync(Guid userId);
        Task<CurrentLoginUserForWebViewModel> GetCurrentLoginUserForWebAsync(Guid userId);
    }
}
