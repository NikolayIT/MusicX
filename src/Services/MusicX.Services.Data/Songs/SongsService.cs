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

        private readonly IRepository<Source> sourcesRepository;

        public SongsService(
            IDeletableEntityRepository<Song> songsRepository,
            IDeletableEntityRepository<Artist> artistsRepository,
            IRepository<Source> sourcesRepository)
        {
            this.songsRepository = songsRepository;
            this.artistsRepository = artistsRepository;
            this.sourcesRepository = sourcesRepository;
        }

        public SongArtistsAndTitle GetSongInfo(int id)
        {
            var song = this.songsRepository.All().Where(x => x.Id == id).Select(
                    x => new { x.Name, Artists = x.Artists.OrderBy(a => a.Order).Select(a => a.Artist.Name) })
                .FirstOrDefault();

            return song == null ? null : new SongArtistsAndTitle(song.Artists.ToList(), song.Name);
        }

        public int CountSongs(Expression<Func<Song, bool>> predicate = null)
        {
            IQueryable<Song> songsQuery = this.songsRepository.All();
            if (predicate != null)
            {
                songsQuery = songsQuery.Where(predicate);
            }

            return songsQuery.Count();
        }

        public IEnumerable<SongArtistsTitleAndMetadata> GetSongsInfo(
            Expression<Func<Song, bool>> predicate = null,
            Expression<Func<Song, object>> orderBySelector = null,
            int? skip = null,
            int? take = null)
        {
            IQueryable<Song> songsQuery = this.songsRepository.All();
            if (predicate != null)
            {
                songsQuery = songsQuery.Where(predicate);
            }

            if (orderBySelector != null)
            {
                songsQuery = songsQuery.OrderBy(orderBySelector);
            }

            if (skip != null)
            {
                songsQuery = songsQuery.Skip(skip.Value);
            }

            if (take != null)
            {
                songsQuery = songsQuery.Take(take.Value);
            }

            return GetSongArtistsTitleAndMetadata(songsQuery);
        }

        public IEnumerable<SongArtistsTitleAndMetadata> GetRandomSongs(int count, Expression<Func<Song, bool>> predicate = null)
        {
            var songsQuery = this.songsRepository.All();
            if (predicate != null)
            {
                songsQuery = songsQuery.Where(predicate);
            }

            var ids = songsQuery.Select(x => x.Id).ToList().OrderBy(x => Guid.NewGuid()).Take(count);
            var idsQuery = this.songsRepository.All().Where(x => ids.Contains(x.Id));
            return GetSongArtistsTitleAndMetadata(idsQuery);
        }

        public int CreateSong(SongArtistsTitleAndMetadata songInfo, string sourceName, string sourceItemId)
        {
            // TODO: Merge artist names when similar?
            if (songInfo == null)
            {
                return 0;
            }

            var songTitle = songInfo.Title.Trim();
            if (string.IsNullOrWhiteSpace(songTitle))
            {
                return 0;
            }

            var artists = songInfo.Artists.Select(x => x.Trim()).Distinct().ToList();

            var similarSongs = this.songsRepository.AllWithDeleted().Where(x => x.Name == songTitle).Select(x => new { Artists = x.Artists.Select(a => a.Artist.Name) }).ToList();
            foreach (var similarSong in similarSongs)
            {
                if (!similarSong.Artists.Except(artists).Any() && !artists.Except(similarSong.Artists).Any())
                {
                    // Found the same song
                    return 0;
                }
            }

            var sourceId = this.sourcesRepository.All().FirstOrDefault(x => x.Name == sourceItemId)?.Id;
            var dbSong = new Song { Name = songTitle, SourceId = sourceId, SourceItemId = sourceItemId };
            var dbSongArtists = new List<SongArtist>();
            for (var index = 0; index < artists.Count; index++)
            {
                var dbArtist = this.artistsRepository.AllWithDeleted().FirstOrDefault(x => x.Name == artists[index])
                               ?? new Artist { Name = artists[index] };
                var dbSongArtist = new SongArtist { Artist = dbArtist, Song = dbSong, Order = index + 1 };
                dbSongArtists.Add(dbSongArtist);
            }

            dbSong.Artists = dbSongArtists;
            foreach (var metadata in songInfo.SongAttributes.Where(x => x.Key != MetadataType.Artist && x.Key != MetadataType.Title))
            {
                foreach (var value in metadata.Value)
                {
                    dbSong.Metadata.Add(
                        new SongMetadata
                        {
                            Type = metadata.Key, Value = value, SourceId = sourceId, SourceItemId = sourceItemId
                        });
                }
            }

            this.songsRepository.Add(dbSong);
            this.songsRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return dbSong.Id;
        }

        private static IEnumerable<SongArtistsTitleAndMetadata> GetSongArtistsTitleAndMetadata(IQueryable<Song> songsQuery)
        {
            var songs = songsQuery.Select(
                x => new
                     {
                         x.Name,
                         Artists = x.Artists.OrderBy(a => a.Order).Select(a => a.Artist.Name),
                         Metadata = x.Metadata.Where(y => y.Type != MetadataType.Lyrics).Select(y => new { y.Type, y.Value }),
                     }).ToList();

            var result = new List<SongArtistsTitleAndMetadata>();
            foreach (var song in songs)
            {
                result.Add(
                    new SongArtistsTitleAndMetadata(
                        song.Artists.ToList(),
                        song.Name,
                        new SongAttributes(song.Metadata.Select(x => new Tuple<MetadataType, string>(x.Type, x.Value)))));
            }

            return result;
        }
    }
}
