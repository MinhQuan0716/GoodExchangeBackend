using Application.VnPay.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IPaymentService
    {
        public string GetPayemntUrl();
        public int ReturnTransactionStatus();
        public Task<bool> AddMoneyToWallet();
        public Task<bool> Refund();
        public Task<VnPayIpnResponse> HandleIpn();
    }
}
