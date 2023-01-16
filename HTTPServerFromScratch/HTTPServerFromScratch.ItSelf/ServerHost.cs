using System.Net;
using System.Net.Sockets;

namespace HTTPServerFromScratch.ItSelf
{
    public class ServerHost
    {
        private readonly IHandler _handler;

        public ServerHost(IHandler handler)
        {
            _handler = handler;
        }

        public void StartV1()
        {
            Console.WriteLine("Start V1");
            var listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            while (true)
            {
                using (var client = listener.AcceptTcpClient())
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

        public void StartV2()
        {
            Console.WriteLine("Start V2");
            var listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();


            while (true)
            {
                var client = listener.AcceptTcpClient();
                ProcessClient(client);

            }
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Start Async");
            var listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();


            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                var _ = ProcessClientAsync(client);
            }
        }

        private void ProcessClient(TcpClient client)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                using (client)
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
            });
        }

        private async Task ProcessClientAsync(TcpClient client)
        {
            try
            {
                using (client)
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                {
                    var firstLine = await reader.ReadLineAsync();
                    //if we don't read all headers from the request, your request is pending
                    for (string? line = null; line != string.Empty; line = await reader.ReadLineAsync())
                        ;

                    var request = RequestParser.RequestParse(firstLine);

                    await _handler.HandleAsync(stream, request);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}