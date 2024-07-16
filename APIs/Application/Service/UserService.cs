using Application.Common;
using Application.InterfaceService;
using Application.Util;
using Application.ViewModel.UserModel;
using Application.ViewModel.UserViewModel;
using AutoMapper;
using Domain.Entities;
using Domain.Enum;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly AppConfiguration _appConfiguration;
        private readonly ICurrentTime _currentTime;
        private readonly ISendMailHelper _sendMailHelper;
        private readonly IClaimService _claimService;
        private readonly ICacheService _cacheService;
        private readonly IUploadFile _uploadFile;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, AppConfiguration appConfiguration, ICurrentTime currentTime
            , ISendMailHelper sendMailHelper, IClaimService claimService, ICacheService cacheService,IUploadFile uploadFile)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appConfiguration = appConfiguration;
            _currentTime = currentTime;
            _sendMailHelper = sendMailHelper;
            _claimService = claimService;
            _cacheService = cacheService;
            _uploadFile= uploadFile;    
        }

        public bool CheckVerifyCode(string key)
        {
            var email = _cacheService.GetData<string>(key);
            if (email == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CreateAccount(RegisterModel registerModel)
        {
            var user = await _unitOfWork.UserRepository.FindUserByEmail(registerModel.Email);
            if (user != null)
            {
                throw new Exception("Email already exist");
            }
            DateTime birthDay;
            if (!DateTime.TryParseExact(registerModel.Birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDay))
            {
                throw new Exception("Invalid Birthday format. Please use 'yyyy-MM-dd' format.");
            }
            var newAccount = _mapper.Map<User>(registerModel);
            newAccount.BirthDay = birthDay;
            newAccount.RoleId = 3;
            newAccount.PasswordHash = registerModel.Password.Hash();
            (newAccount.FirstName, newAccount.LastName) = StringUtil.SplitName(registerModel.Fullname);
            newAccount.ProfileImage = "https://firebasestorage.googleapis.com/v0/b/firestorage-4ee45.appspot.com/o/Product%2Favatar-trang-4.jpg?alt=media&token=b5970145-10b1-4adf-b04a-2b73b9aa6088";
            await _unitOfWork.UserRepository.AddAsync(newAccount);
            return await _unitOfWork.SaveChangeAsync()>0;
        }

        public async Task<Token> Login(LoginModel loginModel, string apiOrigin)
        {
            var user = await _unitOfWork.UserRepository.FindUserByEmail(loginModel.Email);
            if (user == null)
            {
                throw new Exception("Email do not exist");
            }
            if (!loginModel.Password.CheckPassword(user.PasswordHash))
            {
                throw new Exception("Password is not correct");
            }
            var findKey = user.Id.ToString() + "_" + apiOrigin;
            var accessToken = user.GenerateTokenString(_appConfiguration!.JWTSecretKey, _currentTime.GetCurrentTime());
            var refreshToken = RefreshToken.GetRefreshToken();
            var key = user.Id.ToString() + "_" + apiOrigin;
            var accessTokenKey = user.Id.ToString() + "_" + "accesstoken";
            var cacheData = _cacheService.SetData<string>(key, refreshToken, _currentTime.GetCurrentTime().AddDays(2));
            var accessTokeData = _cacheService.SetData<string>(accessTokenKey, accessToken, _currentTime.GetCurrentTime().AddDays(2));
            var findUserWallet=await _unitOfWork.WalletRepository.FindWalletByUserId(user.Id);
            var checkVerifyUser=await _unitOfWork.VerifyUsersRepository.FindVerifyUserIdByUserId(user.Id);
            user.ProfileImage = "https://firebasestorage.googleapis.com/v0/b/firestorage-4ee45.appspot.com/o/Product%2Favatar-trang-4.jpg?alt=media&token=b5970145-10b1-4adf-b04a-2b73b9aa6088";
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();
            if (user.RoleId == 3)
            {
                if (findUserWallet == null)
                {
                    var walletId = await CreateWallet(user.Id);
                    user.WalletId = walletId;
                    _unitOfWork.UserRepository.Update(user);
                    await _unitOfWork.SaveChangeAsync();
                }
                if (checkVerifyUser == null)
                {
                    var verfiyUserId = await CreateVerifyUser(user.Id);
                    user.VerifyUserId = verfiyUserId;
                    _unitOfWork.UserRepository.Update(user);
                    await _unitOfWork.SaveChangeAsync();
                }
            }
            return new Token
            {
                userId=user.Id,
                userName=user.UserName,
                accessToken = accessToken,
                refreshToken = refreshToken,
            };
        }

        public async Task<bool> Logout(string apiOrigin)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(_claimService.GetCurrentUserId);
            string key = user.Id.ToString() + "_" + apiOrigin;
            bool isDelete = (bool)_cacheService.RemoveData(key);
            return isDelete;
        }

        public async Task<bool> ResetPassword(string code, ResetPasswordModel resetPasswordModel)
        {
            if (resetPasswordModel.Password != resetPasswordModel.ConfirmPassword)
            {
                throw new Exception("Password do not match");
            }
            var email = _cacheService.GetData<string>(code);
            var user = await _unitOfWork.UserRepository.FindUserByEmail(email);
            if (user != null)
            {
                if (resetPasswordModel.Password.CheckPassword(user.PasswordHash))
                {
                    throw new Exception("Password must not be the same as old password");
                }
                user.PasswordHash = resetPasswordModel.Password.Hash();
                _unitOfWork.UserRepository.Update(user);
                _cacheService.RemoveData(code);
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
        public async Task<bool> SendVerificationCodeToEmail(string email)
        {
            var findAccount = await _unitOfWork.UserRepository.FindUserByEmail(email);
            string key;
            if (findAccount == null)
            {
                throw new Exception("Account do not exist");
            }
            try
            {
                key = StringUtil.RandomString(6);
                //Get project's directory and fetch ForgotPasswordTemplate content from EmailTemplate
                string exePath = Environment.CurrentDirectory.ToString();
                string FilePath = exePath + @"/EmailTemplate/ForgotPassword.html";
                StreamReader streamreader = new StreamReader(FilePath);
                string MailText = streamreader.ReadToEnd();
                streamreader.Close();
                //Replace [resetpasswordkey] = key
                MailText = MailText.Replace("[resetpasswordkey]", key);
                //Replace [emailaddress] = email
                MailText = MailText.Replace("[emailaddress]", email);
                var result = await _sendMailHelper.SendMailAsync(email, "ResetPassword", MailText);
                if (!result)
                {
                    return false;
                };

                _cacheService.SetData(key, email, DateTimeOffset.Now.AddMinutes(10));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        public async Task<Token> LoginGoogle(string token, string apiOrigin)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token);
                string email = payload.Email;
                string firstName = payload.GivenName;
                string lastName = payload.FamilyName;
                string pictureUrl = payload.Picture;
                var loginUser = await _unitOfWork.UserRepository.FindUserByEmail(email);
                if (loginUser == null)
                {
                    var newAcc = new User()
                    {
                        Email = email,
                        RoleId = 3,
                        IsDelete = false,
                        UserName = firstName + " " + lastName,
                        FirstName = firstName,
                        LastName = lastName,
                        PasswordHash = " ",
                        PhoneNumber = " ",
                        ProfileImage = "https://firebasestorage.googleapis.com/v0/b/firestorage-4ee45.appspot.com/o/Product%2Favatar-trang-4.jpg?alt=media&token=b5970145-10b1-4adf-b04a-2b73b9aa6088",
                        IsBuisnessAccount = false,
                        WalletId = new Guid(),
                    };
                    await _unitOfWork.UserRepository.AddAsync(newAcc);
                    await _unitOfWork.SaveChangeAsync();
                    loginUser = await _unitOfWork.UserRepository.FindUserByEmail(email);
                    var findUserWallet = await _unitOfWork.WalletRepository.FindWalletByUserId(loginUser.Id);
                    var findVerifyByUserId=await _unitOfWork.VerifyUsersRepository.FindVerifyUserIdByUserId(loginUser.Id);
                    if (findUserWallet == null)
                    {
                        var WalletId = await CreateWallet(loginUser.Id);
                        loginUser.WalletId = WalletId;
                    }
                    if(findVerifyByUserId == null)
                    {
                        var VerifyUserId = await CreateVerifyUser(loginUser.Id);

                        loginUser.VerifyUserId = VerifyUserId;
                    }
                    await _unitOfWork.UserRepository.UpdateUserAsync(loginUser);
                    await _unitOfWork.SaveChangeAsync();
                }
                var accessToken = loginUser.GenerateTokenString(_appConfiguration!.JWTSecretKey, _currentTime.GetCurrentTime());
                var refreshToken = RefreshToken.GetRefreshToken();
                var key = loginUser.Id.ToString() + "_" + apiOrigin;
                var cacheData = _cacheService.SetData<string>(key, refreshToken, _currentTime.GetCurrentTime().AddDays(2));
                return new Token
                {
                    userId=loginUser.Id,
                    userName=loginUser.UserName,
                    accessToken = accessToken,
                    refreshToken = refreshToken,
                };
            }
            catch (InvalidJwtException ex)
            {
                // Token is invalid
                throw new Exception("Invalid token", ex);
            }
            catch (Exception ex)
            {
                // Other exceptions
                throw new Exception("Failed to validate token", ex);
            }
        }

        public async Task<bool> BanUser(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId,u=>u.Role);
            if (user == null)
            {
                throw new Exception("User cannot be found");
            }
            if (user.Role.RoleName == nameof(RoleName.Admin))
            {
                throw new Exception("You cannot ban this user");
            }
            _unitOfWork.UserRepository.SoftRemove(user);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<List<User>> GetAllUserAsync()
        {
            List<User> users = await _unitOfWork.UserRepository.GetAllAsync();
            if (users == null)
            {
                throw new Exception("Error in getting user");
            }
            return users;
        }

        public async Task<bool> UpdateUserProfileAsync(UpdateUserProfileModel updateUserProfileModel)
        {
            var findUser = await _unitOfWork.UserRepository.GetByIdAsync(_claimService.GetCurrentUserId);
            if (findUser == null)
            {
                throw new Exception("Cannot find user");
            }
            _mapper.Map(updateUserProfileModel, findUser, typeof(UpdateUserProfileModel), typeof(User));
            (findUser.FirstName, findUser.LastName) = StringUtil.SplitName(updateUserProfileModel.Fullname);
            _unitOfWork.UserRepository.Update(findUser);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<List<User>> GetAllUser()
        {
            return await _unitOfWork.UserRepository.GetAllAsync();
        }
        public async Task<Guid> CreateWallet(Guid userId)
        {
            Wallet newWallet = new Wallet()
            {
                OwnerId = userId,
                UserBalance = 0,
            };
            await _unitOfWork.WalletRepository.AddAsync(newWallet);
            await _unitOfWork.SaveChangeAsync();
            var wallet = await _unitOfWork.WalletRepository.FindWalletByUserId(userId);
            return wallet.Id;
        }
        public async Task<Guid> CreateVerifyUser(Guid userId)
        {
            VerifyUser newVerifyUser = new VerifyUser
            {
                UserId = userId,
                VerifyStatusId=1,
            };
            await _unitOfWork.VerifyUsersRepository.AddAsync(newVerifyUser);
            await _unitOfWork.SaveChangeAsync();
            var verifyUser = await _unitOfWork.WalletRepository.FindWalletByUserId(userId);
            return verifyUser.Id;
        }
        public async Task<bool> PromoteUserToModerator(Guid userId)
        {
            var foundUser = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (foundUser == null)
            {
                throw new Exception("Cannot found user");
            }
            if (foundUser.RoleId == 2)
            {
                throw new Exception("User already a moderator");
            }
            else if (foundUser.RoleId == 1)
            {
                throw new Exception("User is a admin");
            }
            foundUser.RoleId = 2;
            _unitOfWork.UserRepository.Update(foundUser);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<CurrentUserModel> GetCurrentLoginUser()
        {
            var currentLoginUser = await _unitOfWork.UserRepository.GetCurrentLoginUserAsync(_claimService.GetCurrentUserId);
            return currentLoginUser;
        }

        public async Task<bool> UpdatePasswordAsync(UpdatePasswordModel updatePasswordModel)
        {
            var loginUser=await _unitOfWork.UserRepository.GetByIdAsync(_claimService.GetCurrentUserId);
            if (!updatePasswordModel.OldPassword.CheckPassword(loginUser.PasswordHash))
            {
                throw new Exception("Password incorrect");
            }
            loginUser.PasswordHash=  updatePasswordModel.NewPassword.Hash();
            _unitOfWork.UserRepository.Update(loginUser);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

    

        public async Task<CurrentLoginUserForWebViewModel> GetCurrentLoginUserForWeb()
        {
            return await _unitOfWork.UserRepository.GetCurrentLoginUserForWebAsync(_claimService.GetCurrentUserId);
        }

        public async Task<bool> UploadProfileImage(IFormFile userImage)
        {
            var findUser = await _unitOfWork.UserRepository.GetByIdAsync(_claimService.GetCurrentUserId);
            if (findUser !=null)
            {
                return false;
            }
            string userImageUrl = await _uploadFile.UploadFileToFireBase(userImage, "Profile Image");
            findUser.ProfileImage= userImageUrl;
            _unitOfWork.UserRepository.Update(findUser); 
            return await _unitOfWork.SaveChangeAsync()>0;
        }

        public async Task<bool> UnBanUserAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetBannedUserById(userId);
            if (user == null)
            {
                throw new Exception("User cannot be found");
            }
            if (user.Role.RoleName == nameof(RoleName.Admin))
            {
                throw new Exception("You cannot unban this user");
            }
            user.IsDelete= false;
            _unitOfWork.UserRepository.Update(user);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<UserDetailViewModel> GetUserInformation(Guid userId)
        {
            return await _unitOfWork.UserRepository.GetUserDetail(userId);
        }

        public async Task<List<UserViewModelForWeb>> GetAllUserForWeb()
        {
            return await _unitOfWork.UserRepository.GetAllUserForWeb();
        }
    }
}
