using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using musicBridge.Views;

namespace musicBridge.ServiceLayers
{
    public class ytdlpService
    {
        public static void DownloadPlaylist(string youtubeUrl, string youtubeDlpPath, string downloadLocation)
        {
            try
            {
                string strCmdText;
                strCmdText = $"/C yt-dlp -x {youtubeUrl}";

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = strCmdText;
              //  startInfo.Verb = "runas /user:Administrator";
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
