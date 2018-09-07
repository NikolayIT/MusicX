namespace MusicX.Services.DataProviders
{
    using System;
    using System.Threading.Tasks;

    using Google.Apis.Services;
    using Google.Apis.YouTube.v3;

    using MusicX.Common;

    public class YouTubeDataProvider
    {
        // TODO: Extract to config or DB. The API key is limited by IP so its OK to be publicly visible.
        private const string ApiKey = "AIzaSyACdUSMzCxCknVFZoJPAenh6nakrGj1eug";

        public async Task<bool> CheckIfVideoExists(string youtubeId)
        {
            var youtubeService = new YouTubeService(
                new BaseClientService.Initializer
                {
                    ApiKey = ApiKey, ApplicationName = GlobalConstants.ApplicationName, GZipEnabled = true,
                });

            var searchVideoRequest = youtubeService.Videos.List("snippet");
            searchVideoRequest.Id = youtubeId;
            var result = await searchVideoRequest.ExecuteAsync();
            return result.Items.Count != 0;
        }
    }
}

// Queryable data for video: snippet,contentDetails,player,recordingDetails,statistics,status,topicDetails

//// var searchListRequest = youtubeService.Search.List("snippet");
//// searchListRequest.Q = "Google"; // Replace with your search term.
//// searchListRequest.MaxResults = 50;
