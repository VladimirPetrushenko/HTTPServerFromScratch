using HTTPServerFromScratch.ItSelf;

var serverHost = new ServerHost(new ControllerHandler(typeof(Program).Assembly));
await serverHost.StartAsync();
