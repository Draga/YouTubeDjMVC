using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace YouTubeDjMVC.Models
{
    public class VideoDbContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public VideoDbContext() : base("name=VideoDbContext")
        {
        }

        public System.Data.Entity.DbSet<YouTubeDjMVC.Models.Video> Videos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            var videoEntity = entityEntry.Entity as Video;
            if (videoEntity != null)
            {
                var video = videoEntity;
                var alreadyExists = this.Videos.Any(v => v.YouTubeID == video.YouTubeID && v.Status != PlayingStatus.Played);
                
                if (alreadyExists)
                {
                    var list = new List<DbValidationError>
                    {
                        new DbValidationError("YouTubeID", "YouTubeID is a duplicate")
                    };

                    return new DbEntityValidationResult(entityEntry, list);
                }
            }

            return base.ValidateEntity(entityEntry, items);
        }
        
    }
}
