namespace MusicX.Services.Data.Songs
{
    using System.Collections.Generic;
    using System.Linq;

    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;

    public class SongsService : ISongsService
    {
        private readonly IDeletableEntityRepository<Song> songsRepository;

        public SongsService(IDeletableEntityRepository<Song> songsRepository)
        {
            this.songsRepository = songsRepository;
        }

        public SongInfo GetSongInfo(int id)
        {
            var song = this.songsRepository.All().Where(x => x.Id == id).Select(
                    x => new { x.Name, Artists = x.Artists.OrderBy(a => a.Order).Select(a => a.Artist.Name) })
                .FirstOrDefault();

            return song == null
                       ? null
                       : new SongInfo { Name = song.Name, Artists = this.GetSongArtistName(song.Artists.ToList()), };
        }

        /// <param name="artists">Already sorted list of artist names.</param>
        private string GetSongArtistName(IList<string> artists)
        {
            if (artists == null || !artists.Any())
            {
                return string.Empty;
            }

            if (artists.Count == 1)
            {
                return artists[0];
            }

            if (artists.Count == 2)
            {
                return $"{artists[0]} & {artists[1]}";
            }

            var firstArtists = string.Join(", ", artists.Take(artists.Count - 1));
            return $"{firstArtists} & {artists[artists.Count - 1]}";
        }
    }
}
