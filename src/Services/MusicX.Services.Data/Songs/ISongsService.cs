namespace MusicX.Services.Data.Songs
{
    using MusicX.Common.Models;

    public interface ISongsService
    {
        SongArtistsAndTitle GetSongInfo(int id);

        int CreateSong(SongArtistsAndTitle songInfo);
    }
}
