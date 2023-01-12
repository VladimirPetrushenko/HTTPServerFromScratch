namespace HTTPServerFromScratch.ItSelf
{
    internal static class RequestParser
    {
        public static Request RequestParse(string header)
        {
            var split = header.Split(" ");
            return new Request(split[1].Substring(1), GetMethod(split[0]));
        }

        private static HttpMethod GetMethod(string method)
        {
            if(method.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
            {
                return HttpMethod.Get;
            }
            else if(method.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
            {
                return HttpMethod.Post;
            }

            return HttpMethod.Put;
        }
    }
}