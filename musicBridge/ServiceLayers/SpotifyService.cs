using musicBridge.Models;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace musicBridge.ServiceLayers
{
    public class SpotifyService
    {
        private readonly string clientId;
        private readonly Uri redirectUri;
        private SpotifyClient spotifyClient;
        private bool isAuthenticated;

        public SpotifyService()
        {
            Uri manualUri = new System.Uri("http://localhost:5000/callback");
            this.clientId = "995d4175646347d3a5fff2f9db240717";
            this.redirectUri = manualUri;
        }

        public SpotifyService(string clientId, Uri redirectUri)
        {
            this.clientId = clientId;
            this.redirectUri = redirectUri;
        }

        public async Task InitializeAsync()
        {
            var (verifier, challenge) = PKCEUtil.GenerateCodes();
            var loginRequest = new LoginRequest(redirectUri, clientId, LoginRequest.ResponseType.Code)
            {
                CodeChallenge = challenge,
                CodeChallengeMethod = "S256",
                Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.UserReadPlaybackState }
            };
            var uri = loginRequest.ToUri();

            // Method opens the user's browser and navigates to the URI
            OpenBrowser(uri.AbsoluteUri);

            // Start an HTTP server to listen for the callback
            var auth = new EmbedIOAuthServer(redirectUri, 5000);
            await auth.Start();

            var tcs = new TaskCompletionSource<bool>();

            isAuthenticated = false;

            auth.AuthorizationCodeReceived += async (sender, response) =>
            {
                try
                {
                    // This event is triggered once the authorization code is received
                    var initialResponse = await new OAuthClient().RequestToken(
                        new PKCETokenRequest(clientId, response.Code, redirectUri, verifier)
                    );
                    spotifyClient = new SpotifyClient(initialResponse.AccessToken);
                    isAuthenticated = true;
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    tcs.SetException(ex);
                }
                finally
                {
                    await auth.Stop();
                }
            };
            await tcs.Task;
        }

        private void OpenBrowser(string url)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        public async Task<string> GetCurrentUserProfileDisplayName()
        {
            if (spotifyClient == null)
                throw new InvalidOperationException("Spotify Client has not been initialized.");

            var userProfile = await spotifyClient.UserProfile.Current();
            return userProfile.DisplayName;
        }

        public async Task<List<SpotifyArticle>> SearchAlbumsByNameAsync(string query)
        {
            if (spotifyClient == null)
                throw new InvalidOperationException("Spotify Client has not been initialized.");

            var searchRequest = new SearchRequest(SearchRequest.Types.Album, query);
            var searchResponse = await spotifyClient.Search.Item(searchRequest);

            if (searchResponse.Albums == null)
                return new List<SpotifyArticle>();

            var albumTasks = searchResponse.Albums.Items.Select(async album =>
            {
                // Make an additional call to retrieve the tracks for the album
                var tracksResponse = await spotifyClient.Albums.GetTracks(album.Id);
                var songTitles = tracksResponse.Items.Select(track => track.Name).ToArray();

                return new SpotifyArticle
                {
                    ThumbnailPath = album.Images.FirstOrDefault()?.Url,
                    Title = album.Name,
                    Creator = album.Artists.FirstOrDefault()?.Name,
                    Date = DateTime.Parse(album.ReleaseDate),
                    Songs = songTitles
                };
            });

            var albumItems = await Task.WhenAll(albumTasks);
            return albumItems.ToList();
        }
        
        public async Task<List<SpotifyArticle>> SearchAlbumsByIdAsync(string spotifyId)
        {
            if (spotifyClient == null)
                throw new InvalidOperationException("Spotify Client has not been initialized");

            var album = await spotifyClient.Albums.Get(spotifyId);

            if (album == null)
                return null;

            var songTitles = album.Tracks.Items.Select(track => track.Name).ToArray();

            var albumItem = new SpotifyArticle
            {
                ThumbnailPath = album.Images.FirstOrDefault()?.Url,
                Title = album.Name,
                Creator = string.Join(", ", album.Artists.Select(a => a.Name)), // Assuming there can be more than one artist
                Date = DateTime.Parse(album.ReleaseDate),
                Songs = songTitles
            };

            return new List<SpotifyArticle> { albumItem };
        }

        public bool IsAuthenticated
        {
            get
            {
                return isAuthenticated;
            }
        }
    }
}