using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YouTubeDjMVC.Models
{
    public class VideoData
    {
        public IQueryable<Video> Videos { get; set; }

        public TimeSpan TotalTime { get; set; }
    }
}