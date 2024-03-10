using Microsoft.Win32;
using System;
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
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace musicBridge.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(string ytdlp, string dlLocation)
        {
            InitializeComponent();
            StartUp(ytdlp, dlLocation);
        }

        private void StartUp(string ytdlp, string dlLocation)
        {
            txtDownloadTarget.Text = dlLocation;
            txtYtDlpLocation.Text = ytdlp;
        }

        private void BrowseYtDlpLocation_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*"; // Filter to only show executables
            if (openFileDialog.ShowDialog() == true)
            {
                txtYtDlpLocation.Text = openFileDialog.FileName;
            }
        }

        private void BrowseDownloadTarget_Click(object sender, RoutedEventArgs e)
        {
            txtDownloadTarget.Text = BrowseFolder();
        }
        public static string BrowseFolder(string description = "Select a folder")
        {
            string selectedPath = string.Empty;
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = description;
                dialog.UseDescriptionForTitle = true;

                var result = dialog.ShowDialog();

                // Check if the user selected a folder and pressed OK
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    selectedPath = dialog.SelectedPath;
                }
            }

            return selectedPath;
        }

        private void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var mainWnd = System.Windows.Application.Current.MainWindow as MainWindow;
            if (mainWnd != null)
            {
                mainWnd.YtdlLocation = txtYtDlpLocation.Text;
                mainWnd.DownloadTarget = txtDownloadTarget.Text;
                this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("The main window is not initialized or not of the correct type.");
            }
        }
    }
}
