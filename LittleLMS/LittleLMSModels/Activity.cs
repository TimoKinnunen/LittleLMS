using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleLMS.LittleLMSModels
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }

        public int ActivityTypeId { get; set; }
        [ForeignKey("ActivityTypeId")]
        public virtual ActivityType ActivityType { get; set; }

        public int ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }

        [Required(ErrorMessage = "Du måste mata in aktivitetens namn.")]
        [Display(Name = "Aktivitetens namn")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Aktivitetens beskrivning")]
        public string Description { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)] //g Default date & time 10/12/2002 10:11 PM
        [Display(Name = "Aktivitetens start datum")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Aktivitetens slut datum")]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)] //g Default date & time 10/12/2002 10:11 PM
        public DateTime EndDate { get; set; }

        // navigation property
        [Display(Name = "Aktivitetens dokumenter")]
        public virtual ICollection<Document> ActivityDocuments { get; set; }
    }
}