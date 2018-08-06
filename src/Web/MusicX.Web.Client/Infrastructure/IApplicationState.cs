namespace MusicX.Web.Client.Infrastructure
{
    public interface IApplicationState
    {
        bool IsLoggedIn { get; }

        void SetUserToken(string token);
    }
}