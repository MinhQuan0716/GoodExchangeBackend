using Application.InterfaceService;
using Application.Service;
using Application.Util;
using Application.ViewModel.UserModel;
using Application.ViewModel.UserViewModel;
using AutoFixture;
using Backend.Domain.Test;
using Domain.Entities;
using Fare;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Application.Test.ServiceTest
{
    public class UserServiceTest:SetupTest
    {
        private readonly IUserService _userService;
        public UserServiceTest()
        {
            _userService = new UserService(_unitOfWorkMock.Object, _mapper,_appConfiguration.Object,_currentTimeMock.Object,_sendMailHelperMock.Object,_claimServiceMock.Object,_cacheServiceMock.Object,_uploadFileMock.Object) ;
        }
        [Fact]
        public async Task Register_ShouldReturnTrue()
        {
            //Arrange
            var listUser = new List<User>();
            var registerModel = _fixture.Build<RegisterModel>().With(x => x.Phonenumber, new Xeger("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$").Generate).With(x => x.Birthday, "2002-09-12").Create();
            var newUser = new User { Id = Guid.NewGuid(), Email = registerModel.Email };
            var newVerifyUser = new VerifyUser { Id = Guid.NewGuid(), UserId = newUser.Id };
            var newWallet = new Wallet {Id = Guid.NewGuid(), OwnerId = newUser.Id };
            //Act
            _unitOfWorkMock.Setup(um => um.UserRepository.FindAsync(u => u.UserName == registerModel.Username || u.Email == registerModel.Email)).ReturnsAsync(listUser);
            var newAccount = _mapper.Map<User>(registerModel);
            _unitOfWorkMock.Setup(um => um.UserRepository.GetByIdAsync(newAccount.Id)).ReturnsAsync(newUser);
            _unitOfWorkMock.Setup(um => um.UserRepository.AddAsync(newAccount)).Verifiable();
            _unitOfWorkMock.Setup(um => um.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(um => um.VerifyUsersRepository.AddAsync(It.IsAny<VerifyUser>())).Verifiable();
            _unitOfWorkMock.Setup(um => um.VerifyUsersRepository.FindVerifyUserIdByUserId(newUser.Id))
                .ReturnsAsync(newVerifyUser);
            _unitOfWorkMock.Setup(um => um.WalletRepository.FindWalletByUserId(newUser.Id))
                .ReturnsAsync(newWallet);
            bool isCreated = await _userService.CreateAccount(registerModel);
            //Assert
            Assert.True(isCreated);
        }
        [Fact]
        public async Task Regiser_ShouldReturnException()
        {
            //Arrange
            var listUser=_fixture.Build<User>().CreateMany(1).ToList();
            var registerModel = _fixture.Build<RegisterModel>().With(x => x.Phonenumber, new Xeger("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$").Generate)
                .With(x => x.Birthday, "2002-09-12").Create();
            //Act
            _unitOfWorkMock.Setup(um => um.UserRepository.FindAsync(u => u.UserName == registerModel.Username || u.Email == registerModel.Email)).ReturnsAsync(listUser);
            Func<Task> act = async () =>  await _userService.CreateAccount(registerModel);
            //Assert
            act.Should().ThrowAsync<Exception>();
        }
        [Fact]
        public async Task LoginWithMobileAPI_ShouldReturnCorrectData()
        {
            //Arrange
            var mockUser= _fixture.Build<User>().Create();
            var mockWallet = _fixture.Build<Wallet>().With(w => w.OwnerId,mockUser.Id).Create();
            var mockVerifyUser=_fixture.Build<VerifyUser>().With(v=>v.UserId,mockUser.Id).Create();
            var loginDTO = new LoginModel { Email = mockUser.Email, Password = mockUser.PasswordHash };
            mockUser.PasswordHash = mockUser.PasswordHash.Hash();
            _unitOfWorkMock.Setup(unit => unit.UserRepository.FindUserByEmail(mockUser.Email)).ReturnsAsync(mockUser);
            _unitOfWorkMock.Setup(unit => unit.UserRepository.Update(mockUser)).Verifiable();
            _unitOfWorkMock.Setup(unit=>unit.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(unit=>unit.WalletRepository.FindWalletByUserId(mockUser.Id)).ReturnsAsync(mockWallet);
            _unitOfWorkMock.Setup(unit => unit.VerifyUsersRepository.FindVerifyUserIdByUserId(mockUser.Id)).ReturnsAsync(mockVerifyUser);
            _appConfiguration.SetupAllProperties();
            _appConfiguration.Object.JWTSecretKey = "Testtetsttetstetstetetstettsttxtttwtsttwtefdwqw";
            //Act
            var result = await _userService.Login(loginDTO,"Mobile");
            //Assert
            result.Should().NotBeNull();
        }
        [Fact]
        public async Task LoginWithMobileAPI_ShouldReturnException()
        {
            //Arrange
            var mockUser = _fixture.Build<User>().Create();
            var loginDTO = new LoginModel { Email = mockUser.Email, Password = mockUser.PasswordHash };
            mockUser.PasswordHash = mockUser.PasswordHash.Hash();
            //Act
            _unitOfWorkMock.Setup(unit => unit.UserRepository.FindUserByEmail(mockUser.Email)).ReturnsAsync((User)null);
            _unitOfWorkMock.Setup(unit => unit.UserRepository.Update(mockUser)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            //  _unitOfWorkMock.Setup(unit => unit.CacheRepository.GetData<string>(mockUser.Id.ToString())).Returns((string)null);
            _cacheServiceMock.Setup(cache => cache.GetData<string>(mockUser.Id.ToString())).Returns((string)null);
            _appConfiguration.SetupAllProperties();
            _appConfiguration.Object.JWTSecretKey = "Testtetsttetstetstetetstettsttxtttwtsttwtefdwqw";
            //Assert
            Assert.ThrowsAsync<Exception>(async() => await _userService.Login(loginDTO, "Mobile"));
        }
        [Fact]
        public async Task VerifyCode_ShouldReturnCorrect()
        {
            //Arrange 
            var code = StringUtil.RandomString(6);
            var email = _fixture.Create<MailAddress>();
            //Act
            //   _unitOfWorkMock.Setup(unit => unit.CacheRepository.GetData<string>(code)).Returns(email.Address);
            _cacheServiceMock.Setup(cache => cache.GetData<string>(code)).Returns(email.Address);
            bool isCorrectCode =  _userService.CheckVerifyCode(code);
            //Assert
            Assert.True(isCorrectCode);
        }
        [Fact]
        public async Task VerifyCode_ShouldReturnError()
        {
            //Arrange 
            var code = StringUtil.RandomString(6);
            var email = _fixture.Create<MailAddress>();
            //Act
            // _unitOfWorkMock.Setup(unit => unit.CacheRepository.GetData<string>(code)).Returns((string)null);
            _cacheServiceMock.Setup(cache => cache.GetData<string>(code)).Returns((string)null);
            bool isCorrectCode = _userService.CheckVerifyCode(code);
            //Assert
            Assert.False(isCorrectCode);
        }
        [Fact]
        public async Task ResetPassword_ShouldReturnCorrect()
        {
            //Arrange
            var code =StringUtil.RandomString(6);   
            var user= _fixture.Build<User>().Create();
           var resetPasswordModel=_fixture.Build<ResetPasswordModel>().Without(x=>x.Password)
                .Without(x=>x.ConfirmPassword)
                .Do(
                    x =>
                    {
                        x.Password = _fixture.Create<string>();
                        x.ConfirmPassword = x.Password;
                    }
                   ).Create();
            user.PasswordHash = resetPasswordModel.Password.Hash();
            //Act
            _unitOfWorkMock.Setup(unit => unit.UserRepository.FindUserByEmail(user.Email)).ReturnsAsync(user);
          //  _unitOfWorkMock.Setup(unit => unit.CacheRepository.GetData<string>(code)).Returns(user.Email);
           
            _unitOfWorkMock.Setup(unit => unit.UserRepository.Update(user)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _cacheServiceMock.Setup(cache => cache.RemoveData(code)).Verifiable();
            bool isResetSuccess= await _userService.ResetPassword(code, resetPasswordModel);
            //Assert
            Assert.True(isResetSuccess);
        }
        [Fact]
        public async Task ResetPassword_ShouldReturnException()
        {
            //Arrange
            var code = StringUtil.RandomString(6);
            var user = _fixture.Build<User>().Create();
            var resetPasswordModel = _fixture.Build<ResetPasswordModel>().Create();
            user.PasswordHash = resetPasswordModel.Password.Hash();
            //Act
            _unitOfWorkMock.Setup(unit => unit.UserRepository.FindUserByEmail(user.Email)).ReturnsAsync(user);
            // _unitOfWorkMock.Setup(unit => unit.CacheRepository.GetData<string>(code)).Returns(user.Email);
            _cacheServiceMock.Setup(cache => cache.GetData<string>(code)).Returns(user.Email);
            _unitOfWorkMock.Setup(unit => unit.UserRepository.Update(user)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            _cacheServiceMock.Setup(cache => cache.RemoveData(code)).Verifiable();
            //Assert
            Assert.ThrowsAsync<Exception>(async () => await _userService.ResetPassword(code,resetPasswordModel));
        }
       /* [Fact]
        public async Task BanUser_ShouldReturnCorrect()
        {
            //Arrange
            var user= _fixture.Build<User>().Create();
            //Act
            _unitOfWorkMock.Setup(unit => unit.UserRepository.AddAsync(user)).Verifiable();
            _unitOfWorkMock.Setup(unit=>unit.UserRepository.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(unit => unit.UserRepository.SoftRemove(user)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            bool isBan = await _userService.BanUser(user.Id);
            //Assert
            Assert.True(isBan);
        }*/
        [Fact]
        public async Task BanUser_ShouldReturnException()
        {
            //Arrange
            var user = _fixture.Build<User>().Create();
            //Act
            _unitOfWorkMock.Setup(unit => unit.UserRepository.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(unit => unit.UserRepository.SoftRemove(user)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            //Assert
            Assert.ThrowsAsync<Exception>(async () => await _userService.BanUser(Guid.NewGuid()));
        }
        [Fact]
        public async Task UpdatePassword_ShouldReturnCorrect()
        {
            //Arrange 
           
            var updatePasswordDTO=_fixture.Build<UpdatePasswordModel>().With(x=>x.OldPassword,"OldPassword").Create();
            var user=_fixture.Build<User>().With(x=>x.PasswordHash,updatePasswordDTO.OldPassword.Hash()).Create();
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(user.Id);  
            //Act
            _unitOfWorkMock.Setup(unit=>unit.UserRepository.AddAsync(user)).Verifiable(Times.Once());
            _unitOfWorkMock.Setup(unit => unit.UserRepository.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            bool isUpdated = await _userService.UpdatePasswordAsync(updatePasswordDTO);
            //Assert
            Assert.True(isUpdated);
        }
        [Fact]
        public async Task UpdatePassword_ShouldReturnException()
        {
            //Arrange 
            var updatePasswordDTO = _fixture.Build<UpdatePasswordModel>().With(x => x.OldPassword, "OldPassword").Create();
            var user = _fixture.Build<User>().Create();
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(user.Id);
            //Act
            _unitOfWorkMock.Setup(unit => unit.UserRepository.GetByIdAsync(user.Id)).ReturnsAsync(user);
            //Assert
            Assert.ThrowsAsync<Exception>(async()=> await _userService.UpdatePasswordAsync(updatePasswordDTO)) ;
        }
       /* [Fact]
        public async Task UpdateUser_ShouldReturnTrue()
        {
            //Arrage
            var updateUserDTO=_fixture.Build<UpdateUserProfileModel>().With(x=>x.Birthday,DateOnly.FromDateTime(DateTime.UtcNow)).Create();
            var user = _fixture.Build<User>().Create();
            _mapper.Map(updateUserDTO, user, typeof(UpdateUserProfileModel), typeof(User));
           *//* user.BirthDay = updateUserDTO.Birthday.ToDateTime(TimeOnly.MaxValue);*//*
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(user.Id);
            //Act
            _unitOfWorkMock.Setup(unit => unit.UserRepository.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _unitOfWorkMock.Setup(unit=>unit.UserRepository.Update(user)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            bool isUpdated = await _userService.UpdateUserProfileAsync(updateUserDTO);
            //Assert
            Assert.True(isUpdated);
        }
        [Fact]
        public async Task UpdateUser_ShouldReturnException()
        {
            //Arrage
            var updateUserDTO = _fixture.Build<UpdateUserProfileModel>().With(x => x.Birthday, DateOnly.FromDateTime(DateTime.UtcNow)).Create();
            var user = _fixture.Build<User>().Create();
            _mapper.Map(updateUserDTO, user, typeof(UpdateUserProfileModel), typeof(User));
            *//* user.BirthDay = updateUserDTO.Birthday.ToDateTime(TimeOnly.MaxValue);*//*
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(user.Id);
            //Act
          *//*  _unitOfWorkMock.Setup(unit => unit.UserRepository.GetByIdAsync(user.Id)).ReturnsAsync(user);*//*
            _unitOfWorkMock.Setup(unit => unit.UserRepository.Update(user)).Verifiable();
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);
            //Assert
            Assert.ThrowsAsync<Exception>(async()=>await _userService.UpdateUserProfileAsync(updateUserDTO));
        }*/
        [Fact]
        public async Task GetCurrentLoginUser_ShouldReturnNull()
        {
            //Arrange
            var user = _fixture.Build<User>().Create();
            // var currentUserModel = _fixture.Build<CurrentUserModel>().With(x => x.Userid, user.Id).With(x=>x.Birthday, DateOnly.FromDateTime(DateTime.UtcNow)).Create();
            _claimServiceMock.Setup(claim => claim.GetCurrentUserId).Returns(user.Id);
            _unitOfWorkMock.Setup(unit => unit.UserRepository.AddAsync(user)).Verifiable(Times.Once);
            _unitOfWorkMock.Setup(unit => unit.SaveChangeAsync()).ReturnsAsync(1);

            //Act
            var currentLoginUser = await _userService.GetCurrentLoginUser();
            //Assert
            Assert.Null(currentLoginUser);
        }
    }
}
