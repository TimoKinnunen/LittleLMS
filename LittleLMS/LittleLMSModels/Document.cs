﻿using LittleLMS.Models;
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

        [Required(ErrorMessage = "Dokumentet måste ha en typ!")]
        [Display(Name = "Dokumenttyp")]
        public int DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType DocumentType { get; set; }

        [Required(ErrorMessage = "Dokumentet måste ha ett namn!")]
        [Display(Name = "Dokumentnamn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Dokumentet måste ha en beskrivning!")]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Dokumentet måste ha ett datum!")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)] //g Default date & time 10/12/2002 10:11 PM
        [Display(Name = "Datum")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Feedback")]
        public string FeedbackFromTeacherToStudent { get; set; }


        // navigation property
        [Display(Name = "Användare")]
        public virtual ICollection<ApplicationUser> DocumentApplicationUsers { get; set; }

        // navigation property
        [Display(Name = "Kurser")]
        public virtual ICollection<Course> DocumentCourses { get; set; }

        // navigation property
        [Display(Name = "Moduler")]
        public virtual ICollection<Module> DocumentModules { get; set; }

        // navigation property
        [Display(Name = "Aktiviteter")]
        public virtual ICollection<Activity> DocumentActivities { get; set; }
    }
}