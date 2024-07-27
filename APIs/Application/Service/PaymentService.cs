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

        public async Task<bool> BuySubscription(Guid subscriptionId)
        {
            var subscription=await _unitOfWork.SubcriptionRepository.GetByIdAsync(subscriptionId);
            if(subscription == null)
            {
                throw new Exception("Cannot find subscription");
            }
            var userWallet = await _unitOfWork.WalletRepository.GetWalletByUserId(_claimsService.GetCurrentUserId);
            var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(userWallet.Id);
            if(userWallet == null)
            {
                throw new Exception("Cannot find wallet");
            }
            if (userWallet.UserBalance < subscription.Price)
            {
                throw new Exception("User balance not enough to purchase");
            }
           wallet.UserBalance=userWallet.UserBalance-subscription.Price;
            _unitOfWork.WalletRepository.Update(wallet);
            WalletTransaction walletTransaction = new WalletTransaction()
            {
                WalletId=wallet.Id,
                CreatedBy=userWallet.Id,
                SubscriptionId=subscriptionId,
                TransactionType=$"Purchase subscription {subscription.Description}"
            };
            SubscriptionHistory subcriptionHistory = new SubscriptionHistory()
            {
                SubcriptionId=subscriptionId,
                UserId=_claimsService.GetCurrentUserId,
                StartDate=DateTime.UtcNow,
                EndDate=DateTime.UtcNow.AddMonths((int)subscription.ExpiryMonth),
                Status=true
            };
            _unitOfWork.WalletRepository.Update(wallet);
            await _unitOfWork.WalletTransactionRepository.AddAsync(walletTransaction);
            await _unitOfWork.SubscriptionHistoryRepository.AddAsync(subcriptionHistory);
          return await _unitOfWork.SaveChangeAsync()>0;
        }

        public string GetPayemntUrl(int choice)
        {
            switch (choice)
            {
                case 1:
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
                _currentUserIp.UserIp, amount, "VND", "other", "Nap tien vao vi", vnPayConfig.ReturnUrl, orderId);
            paymentUrl = vnpayRequest.GetLink(vnPayConfig.PaymentUrl, vnPayConfig.HashSecret);
            if (paymentUrl != null)
            {
                _cacheService.SetData<int>(keyForCount, count, DateTimeOffset.UtcNow.AddHours(24));
            }
                  
            return paymentUrl;
                case 2:
                     paymentUrl = "";
                    amount = 100000;
                    key = _claimsService.GetCurrentUserId.ToString() + "_" + "Payment";
                    keyForCount = _claimsService.GetCurrentUserId.ToString() + "_" + "Count";
                     count = _cacheService.GetData<int>(keyForCount);
                    if (count != null)
                    {
                        count++;
                    }
                    orderId = key + "_" + count;
                     vnpayRequest = new VnPayRequest(vnPayConfig.Version,
                        vnPayConfig.TmnCode, DateTime.UtcNow,
                        _currentUserIp.UserIp, amount, "VND", "other", "Nap tien vao vi", vnPayConfig.ReturnUrl, orderId);
                    paymentUrl = vnpayRequest.GetLink(vnPayConfig.PaymentUrl, vnPayConfig.HashSecret);
                    if (paymentUrl != null)
                    {
                        _cacheService.SetData<int>(keyForCount, count, DateTimeOffset.UtcNow.AddHours(24));
                    }
                    return paymentUrl;
                   case 3:
                    paymentUrl = "";
                    amount = 200000;
                    key = _claimsService.GetCurrentUserId.ToString() + "_" + "Payment";
                    keyForCount = _claimsService.GetCurrentUserId.ToString() + "_" + "Count";
                    count = _cacheService.GetData<int>(keyForCount);
                    if (count != null)
                    {
                        count++;
                    }
                    orderId = key + "_" + count;
                    vnpayRequest = new VnPayRequest(vnPayConfig.Version,
                       vnPayConfig.TmnCode, DateTime.UtcNow,
                       _currentUserIp.UserIp, amount, "VND", "other", "Nap tien vao vi", vnPayConfig.ReturnUrl, orderId);
                    paymentUrl = vnpayRequest.GetLink(vnPayConfig.PaymentUrl, vnPayConfig.HashSecret);
                    if (paymentUrl != null)
                    {
                        _cacheService.SetData<int>(keyForCount, count, DateTimeOffset.UtcNow.AddHours(24));
                    }
                    return paymentUrl;
                default:
                    return null;
            }
           
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
