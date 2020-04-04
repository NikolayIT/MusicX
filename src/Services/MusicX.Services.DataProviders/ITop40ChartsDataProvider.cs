namespace MusicX.Services.DataProviders
{
    using MusicX.Common.Models;

    public interface ITop40ChartsDataProvider
    {
        SongAttributes GetSong(int id);
    }
}
