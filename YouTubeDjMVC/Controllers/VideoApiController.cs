using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
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
            var video = new Video(youTubeVideo);
            db.Videos.Add(video);
            // TODO: validate
            //IEnumerable<DbEntityValidationResult> validationErrors = db.GetValidationErrors();
            //if (validationErrors.Any())
            //{
            //    //return BadRequest(ModelState);
            //    return new ErrorResponse("Validation error" + validationErrors.Select(e=>e.Entry.CurrentValues.PropertyNames));
            //}
            db.SaveChanges();

            return new SuccessResponse();
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