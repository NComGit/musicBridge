using musicBridge.Models;
using musicBridge.ServiceLayers;
using musicBridge.Views;
using SpotifyAPI.Web;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace musicBridge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpotifyService service;
        public YoutubeService youtubeService;
        private string _ytdlLocation = "";
        private string _downloadTarget = "";
        private string _youtubeApiKey = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = string.Empty;
            List<SpotifyArticle> albums = new List<SpotifyArticle>();

            switch (TbcSearch.SelectedIndex)
            {
                case 1:
                    searchTerm = TbxAlbumName.Text;
                    albums = await service.SearchAlbumsByNameAsync(searchTerm);
                    break;
                case 2:
                    searchTerm = TbxAlbumId.Text;
                    albums = await service.SearchAlbumsByIdAsync(searchTerm);
                    break;
            }

            ResultsWindow resultsWindow = new ResultsWindow(albums)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
             };
            resultsWindow.ShowDialog();
        }

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                youtubeService = new YoutubeService(YoutubeApiKey);
                service = new SpotifyService();
                await service.InitializeAsync();
                if (service.IsAuthenticated == true)
                {
                    BtnSearch.IsEnabled = true;
                }
                else
                {
                    System.Windows.MessageBox.Show("Please allow access to your spotify account");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(YtdlLocation, DownloadTarget, YoutubeApiKey)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            settingsWindow.ShowDialog();
        }

        public string YtdlLocation
        {
            get => _ytdlLocation;
            set
            {
                _ytdlLocation = value;
            }
        }

        public string DownloadTarget
        {
            get => _downloadTarget;
            set
            {
                _downloadTarget = value;
            }
        }

        public string YoutubeApiKey
        {
            get => _youtubeApiKey;
            set
            {
                _youtubeApiKey = value;
            }
        }
    }
}