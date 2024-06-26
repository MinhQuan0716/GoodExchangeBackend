using Application.InterfaceService;
using Application.Util;
using Application.ZaloPay.Config;
using Application.ZaloPay.Request;
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

      /*  public async Task<bool> AddMoneyToWallet()
        {
            int statusCode = ReturnTransactionStatus();
            string key=_claimsService.GetCurrentUserId.ToString()+"_"+"Payment";
            Wallet foundWallet = await _unitOfWork.WalletRepository.FindWalletByUserId(_claimsService.GetCurrentUserId);
            string apptransid = _cacheService.GetData<string>(key);
            long amount = _cacheService.GetData<long>(apptransid);
            if (statusCode > 0)
            {
                if (foundWallet == null)
                {
                    Wallet wallet = new Wallet()
                    {
                        OwnerId = _claimsService.GetCurrentUserId,
                        UserBalance = amount,
                    };
                   // _cacheService.RemoveData(key);
                   // _cacheService.RemoveData(apptransid);
                    await _unitOfWork.WalletRepository.AddAsync(wallet);
                }
                else
                {
                    foundWallet.UserBalance += amount;
                    _unitOfWork.WalletRepository.Update(foundWallet);
                }

            }
            else
            {
                throw new Exception("Update user balance error");
            }
            return await _unitOfWork.SaveChangeAsync()>0;
        }*/

        public string GetPayemntUrl()
        {
            string paymentUrl = "";
            /* long amount=50000;
             string key=_claimsService.GetCurrentUserId.ToString()+"_"+"Payment";
             string keyForCount=_claimsService.GetCurrentUserId.ToString()+"_"+"Count";
             int count = _cacheService.GetData<int>(keyForCount);
             if(count!=null)
             {
                 count++;
             }
             var zaloPayRequest = new CreateZaloPayRequest(zaloPayConfig.AppId, zaloPayConfig.AppUser, DateTime.UtcNow.GetTimeStamp()
                 , amount, DateTime.UtcNow.ToString("yyMMdd") + "_" + _claimsService.GetCurrentUserId.ToString()+"0"+count.ToString(), "zalopayapp", "ZaloPay demo");
             zaloPayRequest.MakeSignature(zaloPayConfig.Key1);
             (bool createZaloPayLinkResult, string? createZaloPayMessage) = zaloPayRequest.GetLink(zaloPayConfig.PaymentUrl);
             if (createZaloPayLinkResult)
             {
                 _cacheService.SetData<string>(key, zaloPayRequest.AppTransId, DateTimeOffset.UtcNow.AddHours(20));
                 _cacheService.SetData<long>(zaloPayRequest.AppTransId, amount, DateTimeOffset.UtcNow.AddDays(2));
                 _cacheService.SetData<int>(keyForCount, count, DateTimeOffset.UtcNow.AddHours(20));
                 paymentUrl = createZaloPayMessage;
             }*/
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

       /* public async Task<bool> Refund()
        {

            var wallet = await _unitOfWork.WalletRepository.FindWalletByUserId(_claimsService.GetCurrentUserId);
            if(wallet == null)
            {
                throw new Exception("Chưa nạp tiền vào ví");
            }
            int count = 0;
            string userId = _claimsService.GetCurrentUserId.ToString();
            userId = userId.Replace("-", "");
            string zpKey = _claimsService.GetCurrentUserId.ToString() + "_" + "ZpTransId";
            string key = _claimsService.GetCurrentUserId.ToString() + "_" + "Payment";
            long zpTransId = _cacheService.GetData<long>(zpKey);
            string apptransid = _cacheService.GetData<string>(key);
            if (apptransid != null)
            {
                count++;
            }
            string refundid= DateTime.UtcNow.ToString("yyMMdd")+"_"+ zaloPayConfig.AppId +"_"+ userId+"0"+count.ToString();
            

            long amount = _cacheService.GetData<long>(apptransid);
            var zaloPayRefundRequest = new CreateZaloPayRefundRequest(refundid, zaloPayConfig.AppId, zpTransId,amount, DateTime.UtcNow.GetTimeStamp(), "Refund");
            zaloPayRefundRequest.MakeSignature(zaloPayConfig.Key1);
            (bool createRefundResult, string refundMessage) = zaloPayRefundRequest.GetRefundLink(zaloPayConfig.RefundUrl);
            if (createRefundResult)
            {
                wallet.UserBalance -= amount;
                _cacheService.RemoveData(key);
                _cacheService.RemoveData(zpKey);
            }
            return createRefundResult;
        }

        public int ReturnTransactionStatus()
        {
            int status = 0;
            string key = _claimsService.GetCurrentUserId.ToString() + "_" + "Payment";
            string zpKey=_claimsService.GetCurrentUserId.ToString()+ "_" + "ZpTransId";
            string apptransid = _cacheService.GetData<string>(key);
            var zaloPayRequest = new CreateZaloPayRequest(zaloPayConfig.AppId, null, 0, 0, apptransid, null, null);
            zaloPayRequest.MakeSignatureForAppTransStatus(zaloPayConfig.Key1);
            (int appTransStatus, long zptransId) = zaloPayRequest.GetStatus(zaloPayConfig.AppTransStatusUrl);
            if (appTransStatus != 0)
            {
                status = appTransStatus;
                _cacheService.SetData<long>(zpKey, zptransId,DateTimeOffset.UtcNow.AddDays(2));
                return status;
            }
            return status;
        }*/
    }
}
