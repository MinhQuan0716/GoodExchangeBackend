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
        private readonly ZaloPayConfig zaloPayConfig;
        private readonly VnPayConfig vnPayConfig;
        private readonly IClaimService _claimsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ICurrentUserIp _currentUserIp;
        public PaymentService(IOptions<ZaloPayConfig> zaloPayConfig,IOptions<VnPayConfig> vnpayConfig
            , IClaimService claimsService,IUnitOfWork unitOfWork, ICacheService cacheService,ICurrentUserIp currentUserIp)
        {
            this.zaloPayConfig = zaloPayConfig.Value;
            this.vnPayConfig = vnpayConfig. Value;
            _claimsService = claimsService;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _currentUserIp = currentUserIp;
        }

        public async Task<bool> AddMoneyToWallet()
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
        }

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
            /*   long amount = 5000;
               string userId=_claimsService.GetCurrentUserId.ToString();
               userId.Replace("-", "");
               string key = _claimsService.GetCurrentUserId.ToString()+"_"+"Payment";
               key.Replace("-", "");
               string email = _unitOfWork.UserRepository.GetByIdAsync(_claimsService.GetCurrentUserId).Result.Email;
               Dictionary<string, string> data = new Dictionary<string, string>();
               data.Add("email", email);
               CreateExtraDataModel createExtraDataModel = new CreateExtraDataModel(data);
               string base64EncodedData=createExtraDataModel.ToBase64String();
               var momoRequest = new MomoOneTimePaymentRequest(momoConfig.PartnerCode
                   , userId, amount, key
                   , "Nap vao vi", momoConfig.ReturnUrl, momoConfig.IpnUrl, "payWithATM", "eyJ1c2VybmFtZSI6ICJtb21vIn0=");
               momoRequest.MakeSignature(momoConfig.AccessKey, momoConfig.SecretKey);
               (bool isCreatedMomo, string momoPaymentUrl) = momoRequest.GetLink(momoConfig.PaymentUrl);
               if(isCreatedMomo)
               {
                   paymentUrl = momoPaymentUrl;
               }*/
            decimal amount = 50000;
            string key = _claimsService.GetCurrentUserId.ToString() + "_" + "Payment";
            key.Replace("-", "");
            var vnpayRequest = new VnPayRequest(vnPayConfig.Version,
                vnPayConfig.TmnCode, DateTime.UtcNow,
                _currentUserIp.UserIp, amount, "VND", "other", "Nap tien vao vi", vnPayConfig.ReturnUrl, key);
            paymentUrl = vnpayRequest.GetLink(vnPayConfig.PaymentUrl, vnPayConfig.HashSecret);
            return paymentUrl;
        }

        public Task<VnPayIpnResponse> HandleIpn()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Refund()
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
        }
    }
}
