namespace MusicX.Web.Client.Infrastructure
{
    using System;

    public interface IApplicationState
    {
        event Action OnUserDataChange;

        bool IsLoggedIn { get; }

        string Username { get; set; }

        string UserToken { get; set; }
    }
}
