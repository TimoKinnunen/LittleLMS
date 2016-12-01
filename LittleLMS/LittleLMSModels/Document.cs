using LittleLMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleLMS.LittleLMSModels
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Document type id")]
        public int DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType DocumentType { get; set; }

        [Required]
        [Display(Name = "Document name")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)] //g Default date & time 10/12/2002 10:11 PM
        [Display(Name = "Document start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Feedback")]
        public string FeedbackFromTeacherToStudent { get; set; }


        // navigation property
        [Display(Name = "Document applicationUsers")]
        public virtual ICollection<ApplicationUser> DocumentApplicationUsers { get; set; }

        // navigation property
        [Display(Name = "Document courses")]
        public virtual ICollection<Course> DocumentCourses { get; set; }

        // navigation property
        [Display(Name = "Document mmodules")]
        public virtual ICollection<Module> DocumentModules { get; set; }

        // navigation property
        [Display(Name = "Document activities")]
        public virtual ICollection<Activity> DocumentActivities { get; set; }
    }
}