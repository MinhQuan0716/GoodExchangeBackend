using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface ISocketServerService
    {
        void Start();
        Task AcceptClientsAsync();
        Task HandleClientAsync(TcpClient client);
    }
}
