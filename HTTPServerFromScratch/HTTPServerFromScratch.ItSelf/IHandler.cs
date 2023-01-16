namespace HTTPServerFromScratch.ItSelf
{
    public interface IHandler
    {
        void Handle(Stream stream, Request request);
    }
}