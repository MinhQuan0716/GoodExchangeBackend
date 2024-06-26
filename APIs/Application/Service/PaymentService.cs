using Application.InterfaceService;
using Application.Util;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Domain.Entities;
using System.Text.Json.Nodes;
using System.Text.Json;
using Application.VnPay.Config;
using Application.VnPay.Request;
using Application.VnPay.Response;
namespace Application.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly VnPayConfig vnPayConfig;
        private readonly IClaimService _claimsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ICurrentUserIp _currentUserIp;
        public PaymentService(IOptions<VnPayConfig> vnpayConfig
            , IClaimService claimsService,IUnitOfWork unitOfWork, ICacheService cacheService,ICurrentUserIp currentUserIp)
        {
            this.vnPayConfig = vnpayConfig. Value;
            _claimsService = claimsService;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _currentUserIp = currentUserIp;
        }

        public string GetPayemntUrl()
        {
            string paymentUrl = "";
            decimal amount = 50000;
            string key = _claimsService.GetCurrentUserId.ToString() + "_" + "Payment";
            string keyForCount = _claimsService.GetCurrentUserId.ToString() + "_" + "Count";
            int count = _cacheService.GetData<int>(keyForCount);
            if (count != null)
            {
                count++;
            }
            string orderId = key + "_" + count;
            var vnpayRequest = new VnPayRequest(vnPayConfig.Version,
                vnPayConfig.TmnCode, DateTime.UtcNow,
                _currentUserIp.UserIp, amount, "VND", "other", "Nap tien vao vi", vnPayConfig.ReturnUrl,orderId);
            paymentUrl = vnpayRequest.GetLink(vnPayConfig.PaymentUrl, vnPayConfig.HashSecret);
            if (paymentUrl != null)
            {
                _cacheService.SetData<int>(keyForCount, count, DateTimeOffset.UtcNow.AddHours(1));
            }
            return paymentUrl;
        }

        public async Task<VnPayIpnResponse> HandleIpn(VnPayResponse vnPayResponse)
        {
           
            var orderId = vnPayResponse.vnp_TxnRef;
            string[] parts = orderId.Split('_');
            string userId = parts[0];   
            long amount = (long)(vnPayResponse.vnp_Amount / 100);
            var vnpSecureHash=vnPayResponse.vnp_SecureHash;
            bool checkValid = vnPayResponse.IsValidSignature(vnPayConfig.HashSecret);
            if (checkValid)
            {
                Guid checkUserId = Guid.Parse(userId);
                var userWallet = await _unitOfWork.WalletRepository.FindWalletByUserId(checkUserId);
                userWallet.UserBalance += amount;
                _unitOfWork.WalletRepository.Update(userWallet);
            }
            else
            {
                throw new Exception("Has invalid secretkey");
                
            }
            if (await _unitOfWork.SaveChangeAsync() > 0)
            {
                VnPayIpnResponse successVnPayIpnResponse = new VnPayIpnResponse("00", "Payment success");
               return successVnPayIpnResponse;
            }
            else
            {
                VnPayIpnResponse errorVnPayIpnResponse = new VnPayIpnResponse("02", "Payment error");
                return errorVnPayIpnResponse;
            }
        }

    }
}
