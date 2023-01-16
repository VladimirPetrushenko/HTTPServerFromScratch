using HTTPServerFromScratch.ItSelf;

namespace HTTPServerFromScratch.Controllers
{
    public class UsersController : IController
    {
        public User[] Index()
        {
            Thread.Sleep(30);

            return new []
            {
                new User("Vladimir", "barrierofdust", "Itransition"),
                new User("Vladimir", "Vladimir", "Itransition"),
                new User("Vladimir", "superPuper", "Itransition")
            };
        }

        public async Task<User[]> IndexAsync()
        {
            await Task.Delay(30);

            return new[]
            {
                new User("Vladimir", "barrierofdust", "Itransition"),
                new User("Vladimir", "Vladimir", "Itransition"),
                new User("Vladimir", "Vladimir", "Itransition"),
                new User("Vladimir", "Vladimir", "Itransition"),
                new User("Vladimir", "Vladimir", "Itransition"),
                new User("Vladimir", "superPuper", "Itransition")
            };
        }
    }
}
