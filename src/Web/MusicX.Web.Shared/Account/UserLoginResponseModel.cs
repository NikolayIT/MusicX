namespace MusicX.Web.Shared.Account
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    public class UserLoginResponseModel
    {
        // TODO: Check why this approach doesn't work: [DataMember(Name = "access_token")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        // ReSharper disable once InconsistentNaming
        public string access_token { get; set; }
    }
}
