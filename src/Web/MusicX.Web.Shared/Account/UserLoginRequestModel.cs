namespace MusicX.Web.Shared.Account
{
    using System.ComponentModel.DataAnnotations;

    public class UserLoginRequestModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
