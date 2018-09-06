namespace MusicX.Web.Client.Infrastructure
{
    using System;

    public class ApplicationState : IApplicationState
    {
        private string userToken;

        private string username;

        public ApplicationState()
        {
            this.SessionId = Guid.NewGuid().ToString();
        }

        public event Action OnUserDataChange;

        public string SessionId { get; set; }

        public bool IsLoggedIn => this.UserToken != null;

        public string Username
        {
            get => this.username;
            set
            {
                this.username = value;
                this.OnUserDataChange?.Invoke();
            }
        }

        public string UserToken
        {
            get => this.userToken;
            set
            {
                this.userToken = value;
                this.OnUserDataChange?.Invoke();
            }
        }
    }
}
