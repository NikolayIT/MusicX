namespace MusicX.Services.Data.Tests
{
    using System.Linq;

    using Xunit;

    public class SongNameSplitterTests
    {
        [Theory]
        [InlineData("F.O. & M.W.P. (056) feat. Hoodini - Няма да се дам (Official Video)", "Няма да се дам", "F.O.", "M.W.P. (056)", "Hoodini")]
        [InlineData("SPENS feat GOODSLAV - НОВАТА ВЪЛНА [ Official HD Video ]", "НОВАТА ВЪЛНА", "SPENS", "GOODSLAV")]
        [InlineData("A   and    B    - C {     OfficiaL   VideO    }", "C", "A", "B")]
        [InlineData("А и Б - В", "В", "А", "Б")]
        [InlineData("EMILIA & DENIS TEOFIKOV - AKULA / Eмилия и Денис Теофиков - Акула, 2019", "Акула", "Eмилия", "Денис Теофиков")]
        //// TODO: [InlineData("A - C (feat. B)", "C", "A", "B")]
        //// TODO: [InlineData("Eminem - Lucky You ft. Joyner Lucas", "Lucky You", "Eminem", "Joyner Lucas")]
        public void SplitShouldWorkCorrectly(
            string inputString,
            string songName,
            params string[] artists)
        {
            var splitter = new SongNameSplitter();
            var result = splitter.Split(inputString);
            Assert.Equal(songName, result.Title);
            Assert.Equal(artists, result.Artists);
        }

        [Theory]
        [InlineData("Linkin Park - Faint", "Linkin Park", "Faint")]
        [InlineData("Linkin Park ---- Faint", "Linkin Park", "Faint")]
        [InlineData("Linkin Park-Faint", "Linkin Park", "Faint")]
        [InlineData("Linkin Park----Faint", "Linkin Park", "Faint")]
        [InlineData("Linkin Park -Faint", "Linkin Park", "Faint")]
        [InlineData("Linkin Park- Faint", "Linkin Park", "Faint")]
        [InlineData("Linkin Park - Faint-2", "Linkin Park", "Faint-2")]
        [InlineData("Linkin Park-Faint-2", "Linkin Park", "Faint-2")]
        [InlineData("Linkin Park- Faint-2", "Linkin Park", "Faint-2")]
        [InlineData("Linkin Park -Faint-2", "Linkin Park", "Faint-2")]
        [InlineData("-Linkin Park -Faint-2", "Linkin Park", "Faint-2")]
        [InlineData("- Linkin Park - Faint-2", "Linkin Park", "Faint-2")]
        [InlineData("Linkin Park -Faint-2-", "Linkin Park", "Faint-2")]
        [InlineData("Faint", "", "Faint")]
        [InlineData("- Faint", "", "Faint")]
        [InlineData("Linkin Park -", "Linkin Park", "")]
        public void SplitSongNameShouldWorkCorrectly(string textToParse, string expectedArtist, string expectedSongTitle)
        {
            var songNameArtistNameSplitter = new SongNameSplitter();
            var result = songNameArtistNameSplitter.SplitSongName(textToParse);
            Assert.NotNull(result.Artist);
            Assert.NotNull(result.Title);
            Assert.Equal(expectedArtist, result.Artist);
            Assert.Equal(expectedSongTitle, result.Title);
        }

        [Theory]
        [InlineData("ABBA")]
        [InlineData("AC/DC")]
        [InlineData("Ace of base")]
        public void SplitArtistNameShouldReturnOnlyOneItemWhenSingleArtistNameIsGiven(string artistName)
        {
            var splitter = new SongNameSplitter();
            var items = splitter.SplitArtistName(artistName).ToList();

            Assert.Single(items);
            Assert.Equal(artistName, items.First());
        }

        [Theory]
        [InlineData("David Guetta ft. Sia", "David Guetta", "Sia")]
        [InlineData("David Guetta Ft. Sia", "David Guetta", "Sia")]
        [InlineData("Katy Perry (Feat. Snoop Dogg)", "Katy Perry", "Snoop Dogg")]
        [InlineData("Adela vs. Radio Killer", "Adela", "Radio Killer")]
        [InlineData("B.B. King & Eric Clapton", "B.B. King", "Eric Clapton")]
        [InlineData("Dr. Ablan feat. Yamboo", "Dr. Ablan", "Yamboo")]
        [InlineData("The RZA, Charles Bernstein", "The RZA", "Charles Bernstein")]
        [InlineData("James Brown with Rev. James Cleveland Choir", "James Brown", "Rev. James Cleveland Choir")]
        [InlineData(" Danzel Vs Dj F.R.A.N.K.", "Danzel", "Dj F.R.A.N.K.")]
        [InlineData("David Guetta featuring Sia", "David Guetta", "Sia")]
        public void SplitArtistNameShouldReturnTwoItemsWhenComplexArtistNameIsGiven(
            string artistName,
            string firstArtistName,
            string secondArtistName)
        {
            var splitter = new SongNameSplitter();
            var items = splitter.SplitArtistName(artistName).ToList();

            Assert.Equal(2, items.Count);
            Assert.Equal(firstArtistName, items[0]);
            Assert.Equal(secondArtistName, items[1]);
        }

        [Theory]
        [InlineData("Ray Charles, Jake & Elwood ", "Ray Charles", "Jake", "Elwood")]
        [InlineData("Dr Dre ft Snoop Dog ft Jj", "Dr Dre", "Snoop Dog", "Jj")]
        [InlineData("Bisollini & Masurski feat.Nevena", "Bisollini", "Masurski", "Nevena")]
        [InlineData("Akon Feat. Vishal Dadlani & Shruti Pathak", "Akon", "Vishal Dadlani", "Shruti Pathak")]
        [InlineData("Akon Ft.Keri Hilson And Rock City", "Akon", "Keri Hilson", "Rock City")]
        [InlineData("Dj Sava feat. Andreea D & J.Yolo", "Dj Sava", "Andreea D", "J.Yolo")]
        public void SplitArtistNameShouldReturnThreeItemsWhenComplexArtistNameIsGiven(
            string artistName,
            string firstArtistName,
            string secondArtistName,
            string thirdArtistName)
        {
            var splitter = new SongNameSplitter();
            var items = splitter.SplitArtistName(artistName).ToList();

            Assert.Equal(3, items.Count);
            Assert.Equal(firstArtistName, items[0]);
            Assert.Equal(secondArtistName, items[1]);
            Assert.Equal(thirdArtistName, items[2]);
        }

        [Theory]
        [InlineData("Avicii & Project 46 & You feat. Daphne", 4, "Avicii", "Daphne")]
        [InlineData("Sheikh & Sundave feat. Heidi Anne vs Syntheticsax", 4, "Sheikh", "Syntheticsax")]
        [InlineData("Arash ft. Sean Paul vs Rob ft. Chris", 4, "Arash", "Chris")]
        [InlineData("Dr. Alban, Dr.Victor & Sash Ft. Cantona", 4, "Dr. Alban", "Cantona")]
        [InlineData("Benny Benassi Ft. Kelis, Apl de App & Jean Baptiste", 4, "Benny Benassi", "Jean Baptiste")]
        [InlineData("Slim feat.martini, bug, spens", 4, "Slim", "spens")]
        [InlineData("Desloc Piccalo Feat. Jermaine Dupri, Pitbull, Flo Rida & Notty Black", 5, "Desloc Piccalo", "Notty Black")]
        [InlineData("Daddy Yankee ft Don Omar, Omega, Yomo, Javi & Voltio", 6, "Daddy Yankee", "Voltio")]
        [InlineData("Kokness,S.asa,Chepika,Flo46,Slient city,Barona,Bisollini", 7, "Kokness", "Bisollini")]
        public void SplitArtistNameShouldReturnProperNumberOfArtists(
            string artistName,
            int artistsCount,
            string firstArtist,
            string lastArtist)
        {
            var splitter = new SongNameSplitter();
            var items = splitter.SplitArtistName(artistName).ToList();

            Assert.Equal(artistsCount, items.Count);
            Assert.Equal(firstArtist, items[0]);
            Assert.Equal(lastArtist, items[items.Count - 1]);
        }

        [Theory]
        [InlineData(null, 0, null, null)]
        [InlineData("", 0, null, null)]
        [InlineData("Spens & & & & & Slim", 2, "Spens", "Slim")]
        [InlineData("Спенс и Слим", 2, "Спенс", "Слим")]
        [InlineData("Dj Tiesto feat. Nelly Furtado; Dj Tiesto feat. Nelly Furtado", 2, "Dj Tiesto", "Nelly Furtado")]
        [InlineData("Lady GaGa Feat. Rodney Jerkins; Lady GaGa Feat. Rodney Jerkins; Lady GaGa Feat. Rodney Jerkins; Lady GaGa Feat. Rodney Jerkins; Lady GaGa Feat. Rodney Jerkins", 2, "Lady GaGa", "Rodney Jerkins")]
        [InlineData(" Ke$ha; Ke$ha; Ke$ha; Ke$ha; Ke$ha ", 1, "Ke$ha", null)]
        public void SplitArtistNameShouldReturnProperArtistsWhenStrangeInputIsGiven(
            string artistName,
            int artistsCount,
            string firstArtist,
            string lastArtist)
        {
            var splitter = new SongNameSplitter();
            var items = splitter.SplitArtistName(artistName).ToList();

            Assert.NotNull(items);
            Assert.Equal(artistsCount, items.Count);
            if (artistsCount > 0)
            {
                Assert.Equal(firstArtist, items[0]);
            }

            if (artistsCount > 1)
            {
                Assert.Equal(lastArtist, items[items.Count - 1]);
            }
        }

        [Theory]
        [InlineData("Andreea Banica feat. Play & Win", 2, "Play & Win")]
        [InlineData("Andreea Banica feat. Play And Win", 2, "Play and Win")]
        [InlineData("Play And Win And Andreea", 2, "Play and Win")]
        [InlineData("Mark F. Angelo Feat. Shaya", 2, "Mark F. Angelo")]
        public void SplitArtistNameShouldRespectExceptions(
            string artistName,
            int artistsCount,
            string expectedArtist)
        {
            var splitter = new SongNameSplitter();
            var items = splitter.SplitArtistName(artistName).ToList();

            Assert.NotNull(items);
            Assert.Equal(artistsCount, items.Count);
            Assert.Contains(expectedArtist, items);
        }
    }
}
