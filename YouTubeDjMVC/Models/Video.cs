using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace YouTubeDjMVC.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Video
    {
        public Video()
        {
            Created = DateTime.Now;
        }

        public Video(dynamic youTubeVideoString)
        {
            YouTubeID = ((string)youTubeVideoString.id["$t"]).Split(':').LastOrDefault();
            Title = youTubeVideoString.title["$t"];
            Length = new TimeSpan(0, 0, int.Parse(((string)youTubeVideoString["media$group"]["yt$duration"].seconds)));
            Created = DateTime.Now;
        }

        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string YouTubeID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Length { get; set; }

        [NotMapped]
        public bool BeforeEndOfDay { get; set; }

        [Required]
        public DateTime Created { get; private set; }

        public string UserName { get; set; }
 
        [Required]
        [DefaultValue(false)]
        public PlayingStatus Status { get; set; }

        public TimeSpan? PlayedTime { get; set; }
    }
}