using System.Net.Sockets;
using System.Net;

namespace HTTPServerFromScratch.ItSelf
{
    public class ServerHost
    {
        private readonly IHandler _handler;

        public ServerHost(IHandler handler)
        {
            _handler = handler;
        }

        public void Start()
        {
            var listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();


            while(true) 
            {
                var client = listener.AcceptTcpClient();

                using (var stream = client.GetStream())
                {
                    _handler.Handle(stream);
                }
            }
        }
    }
}