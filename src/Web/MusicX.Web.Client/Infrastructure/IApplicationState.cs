namespace MusicX.Web.Client.Infrastructure
{
    public interface IApplicationState
    {
        bool IsLoggedIn { get; }

        string Username { get; set; }

        string UserToken { get; set; }
    }
}
