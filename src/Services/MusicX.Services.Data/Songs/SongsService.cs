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

        public IEnumerable<SongArtistsTitleAndMetadata> GetSongsInfo(Expression<Func<Song, bool>> predicate)
        {
            var songs = this.songsRepository.All().Where(predicate).Select(
                x => new
                     {
                         x.Name,
                         Artists = x.Artists.OrderBy(a => a.Order).Select(a => a.Artist.Name).ToList(),
                         Metadata = x.Metadata.Select(y => new { y.Type, y.Value }).ToList(),
                     }).ToList();

            var result = new List<SongArtistsTitleAndMetadata>();
            foreach (var song in songs)
            {
                result.Add(
                    new SongArtistsTitleAndMetadata(
                        song.Artists.ToList(),
                        song.Name,
                        new SongAttributes(
                            song.Metadata.Select(x => new Tuple<MetadataType, string>(x.Type, x.Value)))));
            }

            return result;
        }

        // TODO: If not exists
        public int CreateSong(SongArtistsTitleAndMetadata songInfo)
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
            foreach (var metadata in songInfo.SongAttributes.Where(x => x.Key != MetadataType.Artist && x.Key != MetadataType.Title))
            {
                foreach (var value in metadata.Value)
                {
                    dbSong.Metadata.Add(new SongMetadata { Type = metadata.Key, Value = value });
                }
            }

            this.songsRepository.Add(dbSong);
            this.songsRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return dbSong.Id;
        }
    }
}
