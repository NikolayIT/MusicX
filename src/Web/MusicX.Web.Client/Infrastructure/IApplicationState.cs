namespace MusicX.Web.Client.Infrastructure
{
    using Microsoft.JSInterop;

    public interface IApplicationState
    {
        bool IsLoggedIn { get; }

        string Username { get; set; }

        string UserToken { get; set; }
    }
}
