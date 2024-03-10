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
        public void DownloadVideo(string youtubeUrl, string youtubeDlpPath, string downloadLocation)
        {
            try
            {
                string arguments = $"-x -o \"{downloadLocation}\" \"{youtubeUrl}\"";

                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = youtubeDlpPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                // Start the process
                using (Process process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    Debug.WriteLine(output);
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.WriteLine($"Error: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
