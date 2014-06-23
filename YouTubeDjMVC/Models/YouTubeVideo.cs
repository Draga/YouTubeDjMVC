using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YouTubeDjMVC.Models
{
    using Newtonsoft.Json.Linq;

    public class YouTubeVideo
    {
        public YouTubeVideo() { }
        public YouTubeVideo(dynamic youTubeVideoString)
        {
            string youTubeID = youTubeVideoString.id["$t"].Value.ToString();
            ID = youTubeID.Split(':').LastOrDefault();
        }

        public string ID { get; set; }
    }
}