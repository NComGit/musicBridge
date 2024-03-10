using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace musicBridge.ServiceLayers
{
    public class YoutubeService
    {
        private readonly YouTubeService _youtubeService;

        public YoutubeService(string apiKey)
        {
            _youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = this.GetType().ToString()
            });
        }

        public async Task<List<(string Title, string PlaylistId)>> SearchPlaylistsAsync(string query, int maxResults = 10)
        {
            var searchListRequest = _youtubeService.Search.List("snippet");
            searchListRequest.Q = query; // Replace with your search term.
            searchListRequest.Type = "playlist";
            searchListRequest.MaxResults = maxResults;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<(string Title, string PlaylistId)> playlists = new List<(string Title, string PlaylistId)>();

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#playlist")
                {
                    playlists.Add((searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                }
            }

            return playlists;
        }

        public async Task<List<(string Title, string VideoUrl)>> GetPlaylistVideosAsync(string playlistId)
        {
            var playlistItemsListRequest = _youtubeService.PlaylistItems.List("snippet");
            playlistItemsListRequest.PlaylistId = playlistId;
            playlistItemsListRequest.MaxResults = 50; // Adjust as necessary

            // Retrieve the list of videos in the specified playlist.
            var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

            List<(string Title, string VideoUrl)> videos = new List<(string Title, string VideoUrl)>();

            foreach (var playlistItem in playlistItemsListResponse.Items)
            {
                string videoId = playlistItem.Snippet.ResourceId.VideoId;
                string videoTitle = playlistItem.Snippet.Title;
                string videoUrl = $"https://www.youtube.com/watch?v={videoId}";
                videos.Add((videoTitle, videoUrl));
            }

            return videos;
        }
    }
}
