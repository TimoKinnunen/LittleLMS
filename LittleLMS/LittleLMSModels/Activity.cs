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

        [Required(ErrorMessage = "Aktiviteten måste ha ett namn!")]
        [Display(Name = "Aktivitetsnamn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Aktiviteten måste ha en beskrivning!")]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Aktiviteten måste ha ett startdatum!")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)] 
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Aktiviteten måste ha ett slutdatum!")]
        [Display(Name = "Slutdatum")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        // navigation property
        [Display(Name = "Dokument")]
        public virtual ICollection<Document> ActivityDocuments { get; set; }
    }
}