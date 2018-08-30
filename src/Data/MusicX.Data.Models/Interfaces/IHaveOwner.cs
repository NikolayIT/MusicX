namespace MusicX.Data.Models.Interfaces
{
    public interface IHaveOwner
    {
        string OwnerId { get; set; }

        ApplicationUser Owner { get; set; }
    }
}
