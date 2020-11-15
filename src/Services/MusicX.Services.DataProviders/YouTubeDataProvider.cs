namespace MusicX.Services.DataProviders
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Google.Apis.Services;
    using Google.Apis.YouTube.v3;

    using MusicX.Common;

    public class YouTubeDataProvider : IYouTubeDataProvider
    {
        // TODO: Extract to config or DB. The API key is limited by IP so its OK to be publicly visible.
        private const string ApiKey = "AIzaSyBJeU082U3Gv3QqJdVbpovrUPbLL7I-8GA";

        private readonly YouTubeService youtubeService;

        public YouTubeDataProvider()
        {
            this.youtubeService = new YouTubeService(
                new BaseClientService.Initializer
                {
                    ApiKey = ApiKey,
                    ApplicationName = GlobalConstants.ApplicationName,
                    GZipEnabled = true,
                });
        }

        public async Task<bool> CheckIfVideoExists(string youtubeId)
        {
            var searchVideoRequest = this.youtubeService.Videos.List("snippet");
            searchVideoRequest.Id = youtubeId;
            var result = await searchVideoRequest.ExecuteAsync();
            return result.Items.Count != 0;
        }

        public string SearchVideo(string artist, string songTitle)
        {
            var listRequest = this.youtubeService.Search.List("snippet");
            listRequest.Q = $"{artist} {songTitle}";
            listRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            listRequest.SafeSearch = SearchResource.ListRequest.SafeSearchEnum.None;

            var searchResponse = listRequest.Execute();

            foreach (var searchResult in searchResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        return searchResult.Id.VideoId;
                }
            }

            return null;
        }

        public IEnumerable<(string Id, string Title)> RelatedVideos(string videoId)
        {
            var listRequest = this.youtubeService.Search.List("snippet");
            listRequest.RelatedToVideoId = videoId;
            listRequest.Type = "video";
            listRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            listRequest.SafeSearch = SearchResource.ListRequest.SafeSearchEnum.None;
            listRequest.MaxResults = 10;
            listRequest.TopicId = "/m/04rlf"; // music

            var searchResponse = listRequest.Execute();

            var results = new List<(string Id, string Title)>();
            foreach (var searchResult in searchResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        results.Add((searchResult.Id.VideoId, searchResult.Snippet.Title));
                        break;
                }
            }

            return results;
        }
    }
}

// Queryable data for video: snippet,contentDetails,player,recordingDetails,statistics,status,topicDetails

//// var searchListRequest = youtubeService.Search.List("snippet");
//// searchListRequest.Q = "Google"; // Replace with your search term.
//// searchListRequest.MaxResults = 50;
