﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using musicBridge.Models;
using musicBridge.ServiceLayers;
using SpotifyArticle = musicBridge.Models.SpotifyArticle;

namespace musicBridge.Views
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        public ResultsWindow()
        {
            InitializeComponent();
        }

        public ResultsWindow(List<SpotifyArticle> albums)
        {
            InitializeComponent();
            var listItems = new List<SpotifyArticle>();

            if (albums != null)
            {
                for (int i = 0; i < albums.Count; i++)
                {
                    listItems.Add(new Models.SpotifyArticle
                    {
                        ThumbnailPath = albums[i].ThumbnailPath,
                        Title = albums[i].Title,
                        Creator = albums[i].Creator,
                        Date = albums[i].Date,
                        Songs = albums[i].Songs,
                    });
                }
            }
            lvItems.ItemsSource = listItems;
        }

        private void BtnShowSongs_Click(object sender, RoutedEventArgs e)
        {
            // Find the parent ListBoxItem of the Button (which is inside the DataTemplate of a ListViewItem)
            var button = sender as System.Windows.Controls.Button;
            var item = FindParent<ListBoxItem>(button);

            // Toggle the visibility of the SongsListBox
            var songsListBox = FindChild<System.Windows.Controls.ListBox>(item, "SongsListBox");
            songsListBox.Visibility = songsListBox.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }

        // Helper method to find a child of a given control by name
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                T childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);

                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;

                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            var album = button.Tag as SpotifyArticle;
 
            var searchQuery = album.Title.ToString() + album.Creator;
            System.Windows.MessageBox.Show(searchQuery);
            var mainWnd = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWnd != null)
            {
                try
                {
                    var youtubePlaylist =await mainWnd.youtubeService.SearchPlaylistsAsync(searchQuery);
                    string playlistId = "https://www.youtube.com/playlist?list=" + youtubePlaylist[1].PlaylistId.ToString();
                    System.Windows.MessageBox.Show(playlistId);
                    ytdlpService.DownloadPlaylist(playlistId, mainWnd.YtdlLocation, mainWnd.DownloadTarget);
                }catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Something went wrong during download" + ex.Message);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("The main window is not initialized or not of the correct type.");
            }   
        }
    }
}
