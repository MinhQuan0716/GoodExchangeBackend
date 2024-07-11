
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
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> DenyImageAsync(Guid verifyId)
        {
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
    }
}
