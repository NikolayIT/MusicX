namespace MusicX.Web.Client.Infrastructure
{
    public class ApplicationState : IApplicationState
    {
        public bool IsLoggedIn => this.UserToken != null;

        public string Username { get; set; }

        public string UserToken { get; set; }
    }
}
