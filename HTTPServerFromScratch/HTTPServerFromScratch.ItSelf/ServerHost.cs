using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;

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


            while (true)
            {
                var client = listener.AcceptTcpClient();

                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                {
                    var firstLine = reader.ReadLine();
                    //if we don't read all headers from the request, your request is pending
                    for (string? line = null; line != string.Empty; line = reader.ReadLine())
                        ;

                    var request = RequestParser.RequestParse(firstLine);

                    _handler.Handle(stream, request);
                }
            }
        }
    }
}