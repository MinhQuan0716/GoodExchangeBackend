using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.VnPay.Response
{
    public class VnPayIpnResponse
    {
        public VnPayIpnResponse(string rspCode,string message)
        {
            this.RspCode = rspCode;
            this.Message = message;
        }
        public void Set(string rspCode, string message)
        {
            RspCode = rspCode;
            Message = message;
        }
        public string RspCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
