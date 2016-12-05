using LittleLMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSModels
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kurs namn")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)] //g Default date & time 10/12/2002 10:11 PM
        [Display(Name = "Kurs start datum")]
        public DateTime StartDate { get; set; }

        // navigation property
        [Display(Name = "Kurs dokumenter")]
        public virtual ICollection<Document> CourseDocuments { get; set; }

        [Display(Name = "Course modules")]
        public virtual ICollection<Module> Modules { get; set; }

        [Display(Name = "Course users")]
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}