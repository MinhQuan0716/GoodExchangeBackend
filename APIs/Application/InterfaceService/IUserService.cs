using Application.ViewModel.UserModel;
using Application.ViewModel.UserViewModel;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IUserService
    {
        Task<bool> CreateAccount(RegisterModel registerModel);
        Task<Token> Login (LoginModel loginModel,string apiOrigin);
        Task<bool> SendVerificationCodeToEmail(string email);
        bool CheckVerifyCode(string key);
        Task<bool> ResetPassword(string code,ResetPasswordModel resetPasswordModel);
        Task<bool> Logout(string apiOrigin);
        Task<Token> LoginGoogle(string token, string apiOrigin);
        Task<bool> BanUser(Guid userId);
        Task<List<User>> GetAllUserAsync();
        Task<bool> UpdateUserProfileAsync(UpdateUserProfileModel updateUserProfileModel);
        Task<List<User>> GetAllUser();
        Task<bool> PromoteUserToModerator(Guid userId);
        Task<CurrentUserModel> GetCurrentLoginUser();
        Task<bool> UpdatePasswordAsync(UpdatePasswordModel updatePasswordModel);
        
        Task<CurrentLoginUserForWebViewModel> GetCurrentLoginUserForWeb();
        Task<bool> UnBanUserAsync(Guid userId);
        Task<UserDetailViewModel> GetUserInformation(Guid userId);
        Task<List<UserViewModelForWeb>> GetAllUserForWeb();
    }
}
