namespace MusicX.Web.Client.Infrastructure
{
    public class ApplicationState : IApplicationState
    {
        public bool IsLoggedIn { get; private set; }

        public void SetUserToken(string token)
        {
            this.IsLoggedIn = true;
        }
    }
}
