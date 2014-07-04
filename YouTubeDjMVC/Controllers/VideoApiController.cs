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

                VideoAdded(video);

                return new SuccessResponse();
            }
            catch (DbEntityValidationException ex)
            {
                // Make me nice
                //throw new Exception(string.Join("; ", ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(ve => ve.ErrorMessage))));
                return new ErrorResponse(string.Join("; ", ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(ve => ve.ErrorMessage))));
            }


        }

        private static void VideoAdded(Video video)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<Videos>();
            hubContext.Clients.All.VideoAdded(video);
        }

        private static void VideoRemoved(Video video)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<Videos>();
            hubContext.Clients.All.VideoRemoved(video);
        }

        private static void UpdateClientsNowPlaying(Video video)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<Videos>();
            hubContext.Clients.All.NowPlayingUpdated(video);
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

            VideoRemoved(firstVideo);
            UpdateClientsNowPlaying(firstVideo);

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