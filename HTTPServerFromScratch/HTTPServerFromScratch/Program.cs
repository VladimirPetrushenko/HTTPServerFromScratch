using HTTPServerFromScratch.ItSelf;

var serverHost = new ServerHost(new ControllerHandler(typeof(Program).Assembly));
serverHost.Start();
