using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using YouTubeDjMVC.Models;

namespace YouTubeDjMVC.Hubs
{
    public class Videos : Hub
    {
        private readonly VideoDbContext db = new VideoDbContext();

        public void UpdatePlayTime(int videoId, double? playedTime)
        {
            var video = db.Videos.FirstOrDefault(v => v.ID == videoId);
            if (video == null)
            {
                return;
            }

            if (playedTime.HasValue)
            {
                video.PlayedTime = TimeSpan.FromSeconds((int)playedTime.Value);
            }
            else
            {
                video.PlayedTime = null;
            }

            db.SaveChanges();

            Clients.All.NowPlayingUpdated();
        }
    }
}