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

        [Required(ErrorMessage = "Modulen måste ha ett namn!")]
        [Display(Name = "Modulnamn")]
        public string Name { get; set; }

 
        [Required(ErrorMessage = "Modulen måste ha en beskrivning!")]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Modulen måste ha ett startdatum!")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)] 
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Slutdatum")]
        [Required(ErrorMessage = "Modulen måste ha ett slutdatum!")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)] 
        public DateTime EndDate { get; set; }

        // navigation property
        [Display(Name = "Dokument")]
        public virtual ICollection<Document> ModuleDocuments { get; set; }

        [Display(Name = "Aktiviteter")]
        public virtual ICollection<Activity> Activities { get; set; }
    }
}