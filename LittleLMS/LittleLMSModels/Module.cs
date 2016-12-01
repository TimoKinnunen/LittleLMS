using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleLMS.LittleLMSModels
{
    public class Module
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [Required]
        [Display(Name = "Module name")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)] //g Default date & time 10/12/2002 10:11 PM
        [Display(Name = "Module start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Module end date")]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)] //g Default date & time 10/12/2002 10:11 PM
        public DateTime EndDate { get; set; }

        // navigation property
        [Display(Name = "Module documents")]
        public virtual ICollection<Document> ModuleDocuments { get; set; }

        [Display(Name = "Module activities")]
        public virtual ICollection<Activity> Activities { get; set; }
    }
}