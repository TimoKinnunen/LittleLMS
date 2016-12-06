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

        [Required(ErrorMessage = "Kursen måste ha ett namn!")]
        [Display(Name = "Kursnamn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Kursen måste ha en beskrivning!")]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kursen måste ha ett startdatum!")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)] 
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        // navigation property
        [Display(Name = "Dokument")]
        public virtual ICollection<Document> CourseDocuments { get; set; }

        [Display(Name = "Moduler")]
        public virtual ICollection<Module> Modules { get; set; }

        [Display(Name = "Användare")]
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}