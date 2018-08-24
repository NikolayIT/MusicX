namespace MusicX.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using MusicX.Common.Models;
    using MusicX.Data.Common.Repositories;
    using MusicX.Data.Models;

    public class SongsService : ISongsService
    {
        private readonly IDeletableEntityRepository<Song> songsRepository;

        private readonly IDeletableEntityRepository<Artist> artistsRepository;

        public SongsService(
            IDeletableEntityRepository<Song> songsRepository,
            IDeletableEntityRepository<Artist> artistsRepository)
        {
            this.songsRepository = songsRepository;
            this.artistsRepository = artistsRepository;
        }

        public SongArtistsAndTitle GetSongInfo(int id)
        {
            var song = this.songsRepository.All().Where(x => x.Id == id).Select(
                    x => new { x.Name, Artists = x.Artists.OrderBy(a => a.Order).Select(a => a.Artist.Name) })
                .FirstOrDefault();

            return song == null ? null : new SongArtistsAndTitle(song.Artists.ToList(), song.Name);
        }

        public IEnumerable<SongArtistsAndTitle> GetSongsInfo(Expression<Func<Song, bool>> predicate)
        {
            var songs = this.songsRepository.All().Where(predicate).Select(
                    x => new { x.Name, Artists = x.Artists.OrderBy(a => a.Order).Select(a => a.Artist.Name) })
                .ToList();

            var result = new List<SongArtistsAndTitle>();
            foreach (var song in songs)
            {
                result.Add(new SongArtistsAndTitle(song.Artists.ToList(), song.Name));
            }

            return result;
        }

        // TODO: If not exists
        public int CreateSong(SongArtistsAndTitle songInfo)
        {
            var dbSong = new Song { Name = songInfo.Title, };
            var dbSongArtists = new List<SongArtist>();
            for (var index = 0; index < songInfo.Artists.Count; index++)
            {
                var artist = songInfo.Artists[index].Trim();
                var dbArtist = this.artistsRepository.AllWithDeleted().FirstOrDefault(x => x.Name == artist)
                               ?? new Artist { Name = artist };
                var dbSongArtist = new SongArtist { Artist = dbArtist, Song = dbSong, Order = index + 1 };
                dbSongArtists.Add(dbSongArtist);
            }

            dbSong.Artists = dbSongArtists;
            this.songsRepository.Add(dbSong);
            this.songsRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return dbSong.Id;
        }
    }
}
