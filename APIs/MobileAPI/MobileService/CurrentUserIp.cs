using Application.InterfaceService;

namespace MobileAPI.MobileService
{
    public class CurrentUserIp : ICurrentUserIp
    {
        public CurrentUserIp(IHttpContextAccessor httpContextAccessor)
        {
            var ipAdresss = httpContextAccessor.HttpContext?.Connection?.LocalIpAddress;
            UserIp = ipAdresss.ToString();
        }
        public string UserIp { get; }
    }
}
