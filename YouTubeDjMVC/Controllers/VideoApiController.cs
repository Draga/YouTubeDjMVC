using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.SignalR;
using YouTubeDjMVC.Hubs;
using YouTubeDjMVC.Models;
using YouTubeDjMVC.Models.Responses;

namespace YouTubeDjMVC.Controllers
{
    using System.Collections.Generic;

    public class VideoApiController : ApiController
    {
        private VideoDbContext db = new VideoDbContext();

        // GET: api/VideoApi
        public IQueryable<Video> GetVideos()
        {
            return db.Videos;
        }

        // POST: api/VideoApi
        [ResponseType(typeof(Response))]
        public Response PostYouTubeVideo(dynamic youTubeVideo)
        {
            try
            {
                var video = new Video(youTubeVideo);
                db.Videos.Add(video);

                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Make me nice
                //throw new Exception(string.Join("; ", ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(ve => ve.ErrorMessage))));
                return new ErrorResponse(string.Join("; ", ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors.Select(ve => ve.ErrorMessage))));
            }
            

            UpdateClients();

            return new SuccessResponse();
        }

        private static void UpdateClients()
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<Videos>();
            hubContext.Clients.All.VideoAdded();
        }

        [ResponseType(typeof(Video))]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpGet]
        public Video PopVideo()
        {
            Video firstVideo = db.Videos.FirstOrDefault();
            if (firstVideo != null)
            {
                db.Videos.Remove(firstVideo);
                db.SaveChanges();
            }

            UpdateClients();

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