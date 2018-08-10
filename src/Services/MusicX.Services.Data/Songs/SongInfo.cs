namespace MusicX.Services.Data.Songs
{
    public class SongInfo
    {
        public string Artists { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"{this.Artists} - {this.Name}";
        }
    }
}
