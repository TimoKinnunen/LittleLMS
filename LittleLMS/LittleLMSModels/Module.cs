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
        [Display(Name = "Kursens namn")]
        public virtual Course Course { get; set; }

        [Required]
        [Display(Name = "Modulens namn")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Modulens beskrivning")]
        public string Description { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)] //g Default date & time 10/12/2002 10:11 PM
        [Display(Name = "Modulens start datum")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Modulens slut datum")]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)] //g Default date & time 10/12/2002 10:11 PM
        public DateTime EndDate { get; set; }

        // navigation property
        [Display(Name = "Modulens dokumenter")]
        public virtual ICollection<Document> ModuleDocuments { get; set; }

        [Display(Name = "Modulens aktiviteter")]
        public virtual ICollection<Activity> Activities { get; set; }
    }
}