namespace MusicX.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using MusicX.Data.Common.Models;

    public class WorkerTask : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(100)]
        public string TypeName { get; set; }

        public string Parameters { get; set; }

        public DateTime? RunAfter { get; set; }

        public int Priority { get; set; }

        public bool Processing { get; set; }

        public bool Processed { get; set; }

        public string ProcessingComment { get; set; }

        public string Result { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
