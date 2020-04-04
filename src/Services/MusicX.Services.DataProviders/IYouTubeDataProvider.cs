namespace MusicX.Services.DataProviders
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IYouTubeDataProvider
    {
        Task<bool> CheckIfVideoExists(string youtubeId);

        string SearchVideo(string artist, string songTitle);

        IEnumerable<(string Id, string Title)> RelatedVideos(string videoId);
    }
}
