using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace musicBridge.Models
{
   public class SpotifyArticle
    {
        public string ThumbnailPath { get; set; }
        public string Title { get; set; }
        public string Creator { get; set; }
        public DateTime Date { get; set; }
        public string[] Songs { get; set; }
    }
}
