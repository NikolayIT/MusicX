namespace MusicX.Services.Data.Songs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using MusicX.Common;

    public class SongNameSplitter : ISongNameSplitter
    {
        private static readonly string[] ArtistsDelimiters =
        {
            " featuring ", " ft ", " ft. ", " ft.", " f. ", "(ft ", "(ft. ", "(ft.",
            "(f. ", "(f.", " feat ", " feat. ", "feat.", "(feat ", "(feat. ", "(feat.",
            " vs ", " vs. ", "vs.", " with ", "(with ", " and ", " и ", ";", " / ", "&", ","
        };

        private static readonly string[] SongNameRemovals = { "(official video)", "(official hd video)", "(hd)", "(lyrics)", "(текст)", };

        // TODO: Take it as a parameter
        private readonly string[] exceptions =
        {
            "Play and Win", "Play & Win", "Mark F. Angelo"
        };

        private readonly string pattern;

        public SongNameSplitter()
        {
            this.pattern = "(" + string.Join("|", ArtistsDelimiters.Select(Regex.Escape).ToArray()) + ")";
        }

        public (IEnumerable<string> Artists, string Name) Split(string inputString)
        {
            var songName = this.CleanName(inputString).Trim();

            var parts = this.SplitSongName(songName);
            var artistNames = this.SplitArtistName(parts.Artist.Trim());

            return (artistNames, parts.Name.Trim());
        }

        public IEnumerable<string> SplitArtistName(string inputString)
        {
            var listOfArtistNames = new HashSet<string>();
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return listOfArtistNames;
            }

            foreach (var exception in this.exceptions)
            {
                if (inputString.ToLower().Contains(exception.ToLower()))
                {
                    inputString = inputString.ReplaceCaseInsensitive(exception, string.Empty);
                    listOfArtistNames.Add(exception.Trim());
                }
            }

            var inputStringParts =
                Regex.Split(inputString, this.pattern, RegexOptions.IgnoreCase).Where(s => !string.IsNullOrWhiteSpace(s));

            foreach (var inputStringPart in inputStringParts)
            {
                if (ArtistsDelimiters.Contains(inputStringPart.ToLower()))
                {
                    continue;
                }

                var artistName = inputStringPart.Trim();

                if (artistName.IndexOf(')') != -1 && artistName.IndexOf('(') == -1)
                {
                    artistName = artistName.Replace(")", string.Empty);
                }

                listOfArtistNames.Add(artistName.Trim());
            }

            return listOfArtistNames;
        }

        // TODO: Refactor for code reuse. Remove duplicate code blocks.
        public (string Artist, string Name) SplitSongName(string artistAndSongName)
        {
            var partsSeparatedBySpacesAndDash = artistAndSongName.Split(new[] { " - " }, StringSplitOptions.None);
            if (partsSeparatedBySpacesAndDash.Length == 2)
            {
                return (partsSeparatedBySpacesAndDash[0].TrimDashes(), partsSeparatedBySpacesAndDash[1].TrimDashes());
            }

            if (partsSeparatedBySpacesAndDash.Length > 2)
            {
                var partsWithoutEmptyEntries = artistAndSongName.Split(new[] { " - " }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (partsWithoutEmptyEntries.Length < 2)
                {
                    var partsWithEmptyEntries = artistAndSongName.Split(new[] { " - " }, 2, StringSplitOptions.None);
                    return (partsWithEmptyEntries[0].TrimDashes(), partsWithEmptyEntries[1].TrimDashes());
                }

                return (partsWithoutEmptyEntries[0].TrimDashes(), partsWithoutEmptyEntries[1].TrimDashes());
            }

            var partsSeparatedByDash = artistAndSongName.Split(new[] { "-" }, StringSplitOptions.None);
            if (partsSeparatedByDash.Length == 2)
            {
                return (partsSeparatedByDash[0].TrimDashes(), partsSeparatedByDash[1].TrimDashes());
            }

            if (partsSeparatedByDash.Length > 2)
            {
                artistAndSongName = artistAndSongName.Trim('-');
                var partsWithoutEmptyEntries = artistAndSongName.Split(
                    new[] { "-" },
                    2,
                    StringSplitOptions.RemoveEmptyEntries);
                if (partsWithoutEmptyEntries.Length < 2)
                {
                    var partsWithEmptyEntries = artistAndSongName.Split(new[] { "-" }, 2, StringSplitOptions.None);
                    return (partsWithEmptyEntries[0].TrimDashes(), partsWithEmptyEntries[1].TrimDashes());
                }

                return (partsWithoutEmptyEntries[0].TrimDashes(), partsWithoutEmptyEntries[1].TrimDashes());
            }

            return (string.Empty, artistAndSongName.TrimDashes());
        }

        private string CleanName(string songName)
        {
            songName = songName.Replace("[", "(");
            songName = songName.Replace("]", ")");
            songName = songName.Replace("{", "(");
            songName = songName.Replace("}", ")");
            songName = songName.Replace("( ", "(");
            songName = songName.Replace(" )", ")");

            foreach (var songNameRemoval in SongNameRemovals)
            {
                songName = songName.ReplaceCaseInsensitive(songNameRemoval, string.Empty).Trim();
            }

            return songName;
        }
    }
}
