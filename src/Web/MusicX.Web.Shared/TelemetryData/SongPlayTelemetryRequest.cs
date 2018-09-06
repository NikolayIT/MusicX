namespace MusicX.Web.Shared.TelemetryData
{
    public class SongPlayTelemetryRequest
    {
        public bool PlayedByUser { get; set; }

        public int SongId { get; set; }

        public string SessionId { get; set; }
    }
}
