using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using YouTubeDjMVC.Hubs;
using YouTubeDjMVC.Models;
using YouTubeDjMVC.Models.Responses;

namespace YouTubeDjMVC.Controllers
{
    using System.Collections.Generic;

    public class VideoApiController : ApiController
    {
        /// <summary>
        /// Application DB context
        /// </summary>
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        protected UserManager<ApplicationUser> UserManager { get; set; }

        private VideoDbContext db = new VideoDbContext();

        public VideoApiController()
        {
            this.ApplicationDbContext = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        }

        // GET: api/VideoApi
        public VideoData GetVideoData()
        {
            var videoData = new VideoData
            {
                Videos = db.Videos.Where(v => v.Status == PlayingStatus.Queued),
            };
            videoData.TotalTime = videoData.Videos
                .Select(v => v.Length)
                .DefaultIfEmpty()
                .ToList()
                .Aggregate((l1, l2) => l1 + l2);
            return videoData;
        }

        // GET: api/VideoApi/NowPlaying
        public Video GetNowPlaying()
        {
            return db.Videos.FirstOrDefault(v => v.Status == PlayingStatus.Playing);
        }

        // POST: api/VideoApi
        [ResponseType(typeof(Response))]
        public Response PostYouTubeVideo(dynamic youTubeVideo)
        {
            try
            {
                var video = new Video(youTubeVideo);

                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = ApplicationDbContext.Users.FirstOrDefault(x => x.Id == currentUserId);
                video.UserName = currentUser.UserName;

                db.Videos.Add(video);

                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Make me nice
                //throw new Exception(string.Join("; ", ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(ve => ve.ErrorMessage))));
                return new ErrorResponse(string.Join("; ", ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(ve => ve.ErrorMessage))));
            }


            UpdateClientsVideos();

            return new SuccessResponse();
        }

        private static void UpdateClientsVideos()
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<Videos>();
            hubContext.Clients.All.VideoAdded();
        }

        private static void UpdateClientsNowPlaying()
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<Videos>();
            hubContext.Clients.All.NowPlayingUpdated();
        }

        [ResponseType(typeof(Video))]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public Video PopVideo()
        {
            foreach (var video in db.Videos.Where(v => v.Status == PlayingStatus.Playing))
            {
                video.Status = PlayingStatus.Played;
            }

            Video firstVideo = db.Videos.FirstOrDefault(v => v.Status == PlayingStatus.Queued);
            if (firstVideo != null)
            {
                firstVideo.Status = PlayingStatus.Playing;
                //db.Videos.Remove(firstVideo);
            }


            db.SaveChanges();

            UpdateClientsVideos();
            UpdateClientsNowPlaying();

            return firstVideo;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}