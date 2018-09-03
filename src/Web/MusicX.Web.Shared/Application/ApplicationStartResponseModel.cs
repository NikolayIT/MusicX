namespace MusicX.Web.Shared.Application
{
    using System;

    public class ApplicationStartResponseModel
    {
        public string Username { get; set; }

        public DateTime VersionBuiltOn { get; set; }

        public string EnvironmentName { get; set; }
    }
}
