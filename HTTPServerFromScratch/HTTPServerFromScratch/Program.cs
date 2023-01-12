using HTTPServerFromScratch.ItSelf;

var serverHost = new ServerHost(new StaticFileHandler(Path.Combine(Environment.CurrentDirectory, "www")));
serverHost.Start();
