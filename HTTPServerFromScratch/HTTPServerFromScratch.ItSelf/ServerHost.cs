using System.Net.Sockets;
using System.Net;

namespace HTTPServerFromScratch.ItSelf
{
    public class ServerHost
    {
        public ServerHost()
        {

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
                    using (var reader = new StreamReader(stream))
                    using (var writer = new StreamWriter(stream))
                    {
                        for (string? line = null; line != string.Empty; line = reader.ReadLine())
                        {
                            Console.WriteLine(line);
                        }

                        writer.WriteLine("Hello from server");
                    };
                }
            }
        }
    }
}