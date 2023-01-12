using System.Net;

namespace HTTPServerFromScratch.ItSelf
{
    public class StaticFileHandler : IHandler
    {
        private readonly string _path;

        public StaticFileHandler(string path)
        {
            _path = path;
        }

        public void Handle(Stream networkStream)
        {
            using (var reader = new StreamReader(networkStream))
            using (var writer = new StreamWriter(networkStream))
            {
                var firstLine = reader.ReadLine();
                //if we don't read all headers from the request, your request is pending
                for (string? line = null; line != string.Empty; line = reader.ReadLine())
                    ;

                var request = RequestParser.RequestParse(firstLine);
                var filePath = Path.Combine(_path, request.Path);

                if (!File.Exists(filePath))
                {
                    //file not found 404
                    ResponseWriter.WriteStatus(HttpStatusCode.NotFound, networkStream);
                }
                else
                {
                    ResponseWriter.WriteStatus(HttpStatusCode.OK, networkStream);
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        fileStream.CopyTo(networkStream);
                    }
                }

                Console.WriteLine(filePath);
            }
        }
    }
}