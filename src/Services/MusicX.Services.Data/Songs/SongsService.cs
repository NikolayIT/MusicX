namespace MusicX.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using MusicX.Common;
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

        public async Task<int> CreateSongAsync(string songTitle, IList<string> songArtists, string sourceName, string sourceItemId)
        {
            if (songTitle == null || string.IsNullOrWhiteSpace(songTitle))
            {
                throw new ArgumentException("Song title should be non-empty.", nameof(songTitle));
            }

            if (songArtists == null || !songArtists.Any())
            {
                throw new ArgumentException("Artists list should be non-empty.", nameof(songArtists));
            }

            songTitle = songTitle.Trim();

            // TODO: Merge artist names when similar?
            var artists = songArtists.Select(x => x.Trim()).Distinct().ToList();

            var similarSongs = this.songsRepository.AllWithDeleted().Where(x => x.Name == songTitle).Select(x => new { x.Id, Artists = x.Artists.Select(a => a.Artist.Name) }).ToList();
            foreach (var similarSong in similarSongs)
            {
                if (!similarSong.Artists.Except(artists).Any() && !artists.Except(similarSong.Artists).Any())
                {
                    // Found the same song
                    return similarSong.Id;
                }
            }

            var sourceId = this.sourcesRepository.All().FirstOrDefault(x => x.Name == sourceName)?.Id;
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

            this.songsRepository.Add(dbSong);
            await this.songsRepository.SaveChangesAsync();

            return dbSong.Id;
        }

        public async Task UpdateSongsSystemDataAsync(int songId)
        {
            var song = this.songsRepository.All().FirstOrDefault(x => x.Id == songId);
            var songSearchData = this.songsRepository.AllAsNoTracking().Where(x => x.Id == songId)
                .Select(x => new { x.Name, Artists = x.Artists.Select(a => a.Artist.Name) }).FirstOrDefault();
            if (song == null || songSearchData == null)
            {
                return;
            }

            var searchTerms = new List<string>();
            foreach (var artist in songSearchData.Artists)
            {
                searchTerms.Add(artist.ConvertCyrillicToLatinLetters());
                searchTerms.Add(artist.ConvertLatinToCyrillicLetters());
            }

            searchTerms.Add(songSearchData.Name.ConvertCyrillicToLatinLetters());
            searchTerms.Add(songSearchData.Name.ConvertLatinToCyrillicLetters());

            song.SearchTerms = string.Join(" ", searchTerms.Distinct());
            await this.songsRepository.SaveChangesAsync();
        }

        private static IEnumerable<SongArtistsTitleAndMetadata> GetSongArtistsTitleAndMetadata(IQueryable<Song> songsQuery)
        {
            var songs = songsQuery.Select(
                x => new
                     {
                         x.Name,
                         x.Id,
                         Artists = x.Artists.OrderBy(a => a.Order).Select(a => a.Artist.Name),
                         Metadata = x.Metadata.Select(y => new { y.Type, y.Value }),
                     }).ToList();

            var result = new List<SongArtistsTitleAndMetadata>();
            foreach (var song in songs)
            {
                result.Add(
                    new SongArtistsTitleAndMetadata(
                        song.Id,
                        song.Artists.ToList(),
                        song.Name,
                        new SongAttributes(song.Metadata.Select(x => new Tuple<SongMetadataType, string>(x.Type, x.Value)))));
            }

            return result;
        }
    }
}
