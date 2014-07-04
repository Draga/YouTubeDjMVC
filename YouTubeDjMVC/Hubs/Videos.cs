using System;
using System.Linq;
using System.Threading.Tasks;
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

            Clients.All.NowPlayingUpdated(video);
        }

        public override Task OnConnected()
        {
            Clients.All.UpdateNowPlaying();

            var videoData = new VideoData
            {
                Videos = db.Videos.Where(v => v.Status == PlayingStatus.Queued),
            };

            videoData.TotalTime = videoData.Videos
                .Select(v => v.Length)
                .DefaultIfEmpty()
                .ToList()
                .Aggregate((l1, l2) => l1 + l2);

            Clients.Caller.VideoFullUpdate(videoData);

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            return base.OnDisconnected();
        }
    }
}