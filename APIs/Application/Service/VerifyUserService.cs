
﻿using Application.InterfaceService;
using Application.ViewModel.VerifyModel;
﻿using Application.Common;
using Application.InterfaceService;
using Application.Util;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Expo.Server.Client;
using Expo.Server.Models;

namespace Application.Service
{
    public class VerifyUserService : IVerifyUserService
    {
        private readonly IMapper _mapper;
        private readonly AppConfiguration _appConfiguration;
        private readonly ICurrentTime _currentTime;
        private readonly ISendMailHelper _sendMailHelper;
        private readonly IClaimService _claimService;
        private readonly ICacheService _cacheService;
        private readonly IUploadFile _uploadFile;
        private readonly IUnitOfWork _unitOfWork;
        public VerifyUserService(IUnitOfWork unitOfWork, IMapper mapper, AppConfiguration appConfiguration, ICurrentTime currentTime
            , ISendMailHelper sendMailHelper, IClaimService claimService, ICacheService cacheService, IUploadFile uploadFile)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appConfiguration = appConfiguration;
            _currentTime = currentTime;
            _sendMailHelper = sendMailHelper;
            _claimService = claimService;
            _cacheService = cacheService;
            _uploadFile = uploadFile;
        }

        public async Task<bool> ApproveImageAsync(Guid verifyId)
        {
            string tokenDevice1 = "ExponentPushToken[jFQjSOOFhKJScFOKZIm95C]";
            string tokenDevice2 = "ExponentPushToken[RNG5q5H4mFj9-ufnKlpckw]";
            var findVerified=await _unitOfWork.VerifyUsersRepository.GetByIdAsync(verifyId);
            if(findVerified == null)
            {
                return false;   
            }
            if(findVerified.VerifyStatusId==2)
            {
                return false;
            }
            findVerified.VerifyStatusId = 2;
            _unitOfWork.VerifyUsersRepository.Update(findVerified);
            List<string> newList=new List<string>();
            newList.Add(tokenDevice2);
            newList.Add(tokenDevice1);
            var expoSDKClient = new PushApiClient();
            var pushTicketReq = new PushTicketRequest()
            {
                PushTo =newList, // Target device token
                PushBadgeCount = 1, // Badge count to be displayed on the app icon
                PushBody = "Your verification has been approved" // Message content of the push notification
            };
            var result = await expoSDKClient.PushSendAsync(pushTicketReq);
            if (result?.PushTicketErrors?.Count() > 0)
            {
                foreach (var error in result.PushTicketErrors)
                {
                    Console.WriteLine($"Error: {error.ErrorCode} - {error.ErrorMessage}");
                }
                return false;   
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> DenyImageAsync(Guid verifyId)
        {
            string tokenDevice1 = "ExponentPushToken[jFQjSOOFhKJScFOKZIm95C]";
            string tokenDevice2 = "ExponentPushToken[RNG5q5H4mFj9-ufnKlpckw]";
            List<string> newList = new List<string>();
            newList.Add(tokenDevice2);
            newList.Add(tokenDevice1);
            var findVerified = await _unitOfWork.VerifyUsersRepository.GetByIdAsync(verifyId);
            if (findVerified == null)
            {
                return false;
            }
            if(findVerified.VerifyStatusId == 2)
            {
                return false;
            }
            findVerified.VerifyStatusId = 3;
            _unitOfWork.VerifyUsersRepository.Update(findVerified);
            var expoSDKClient = new PushApiClient();
            var pushTicketReq = new PushTicketRequest()
            {
                PushTo = newList, // Target device token
                PushBadgeCount = 1, // Badge count to be displayed on the app icon
                PushBody = "Your verification has been denied" // Message content of the push notification
            };
            var result = await expoSDKClient.PushSendAsync(pushTicketReq);
            if (result?.PushTicketErrors?.Count() > 0)
            {
                foreach (var error in result.PushTicketErrors)
                {
                    Console.WriteLine($"Error: {error.ErrorCode} - {error.ErrorMessage}");
                }
                return false;
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<List<VerifyViewModel>> GetAllWaitingUserToApproveAsync()
        {
           return await _unitOfWork.VerifyUsersRepository.GetAllVerifyUserAsync();
        }
        public async Task<bool> UploadImageForVerifyUser(IFormFile userImage)
        {
            var verifyUser = await _unitOfWork.VerifyUsersRepository.FindVerifyUserIdByUserId(_claimService.GetCurrentUserId);
            if (verifyUser != null)
            {
                string imageUrl = await _uploadFile.UploadFileToFireBase(userImage, "VerifyUser");
                verifyUser.UserImage = imageUrl;
                verifyUser.VerifyStatusId = 1;
                _unitOfWork.VerifyUsersRepository.Update(verifyUser);
            }
            return await _unitOfWork.SaveChangeAsync() > 0;
        }
        public async Task<bool> UploadImage(IFormFile ImageVerify)
        {
            var imageUrl = await _uploadFile.UploadFileToFireBase(ImageVerify, "Verify");
            var userId = _claimService.GetCurrentUserId;
            var verifyUser = await _unitOfWork.VerifyUsersRepository.FindVerifyUserIdByUserId(userId);
            if (verifyUser == null)
            {
                VerifyUser newVerifyUser = new VerifyUser
                {
                    UserId = userId,
                    UserImage = imageUrl,
                    VerifyStatusId = 1
                };
                await _unitOfWork.VerifyUsersRepository.AddAsync(newVerifyUser);
                return await _unitOfWork.SaveChangeAsync() > 0;
            }
            else
            {
                verifyUser.UserImage = imageUrl;
                verifyUser.VerifyStatusId= 1;
                _unitOfWork.VerifyUsersRepository.Update(verifyUser);
                return await _unitOfWork.SaveChangeAsync() > 0;
            }
        }
        public async Task<string> getVerifyStatus()
        {
            var userId = _claimService.GetCurrentUserId;
            var verifyUser = await _unitOfWork.VerifyUsersRepository.FindVerifyUserIdByUserId(userId);
            if (verifyUser == null)
            {
                return "no verification";
            }
            return verifyUser.VerificationStatus.VerificationStatusName;
        }

        public async Task<VerifyViewModel> GetVerifyModelDetailByUserIdAsync(Guid userId)
        {
            return await _unitOfWork.VerifyUsersRepository.GetVerifyUserDetailAsync(userId);
        }
    }
}
