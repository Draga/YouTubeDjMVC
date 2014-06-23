using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using YouTubeDjMVC.Models;

namespace YouTubeDjMVC.Controllers
{
    public class VideoController : Controller
    {
        private VideoDbContext db = new VideoDbContext();

        // GET: Video
        public ActionResult Index()
        {
            return View(db.Videos.ToList());
        }
        public ActionResult Player()
        {
            return View();
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
