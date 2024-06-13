using Application.InterfaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class SocketServerService : ISocketServerService
    {
        private readonly TcpListener _listener;

        public SocketServerService(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            _listener.Start();
            _ = AcceptClientsAsync();
        }

        public async Task AcceptClientsAsync()
        {
            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        public async Task HandleClientAsync(TcpClient client)
        {
            using var networkStream = client.GetStream();
            var buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var response = Encoding.UTF8.GetBytes($"Echo: {message}");
                await networkStream.WriteAsync(response, 0, response.Length);
            }
        }
    }
}
