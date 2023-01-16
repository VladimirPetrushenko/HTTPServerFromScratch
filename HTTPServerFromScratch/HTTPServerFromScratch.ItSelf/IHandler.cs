namespace HTTPServerFromScratch.ItSelf
{
    public interface IHandler
    {
        void Handle(Stream networkStream, Request request);

        Task HandleAsync(Stream networkStream, Request request);
    }
}