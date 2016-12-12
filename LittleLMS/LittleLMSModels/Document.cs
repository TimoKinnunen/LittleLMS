﻿using LittleLMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LittleLMS.LittleLMSModels
{
    public class Document
    {
        public Document()
        {
            DocumentUsers = new List<ApplicationUser>();
            DocumentCourses = new List<Course>();
            DocumentModules = new List<Module>();
            DocumentActivities = new List<Activity>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Dokumentet måste ha en typ!")]
        [Display(Name = "Dokumenttyp")]
        public int DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType DocumentType { get; set; }

        [Display(Name = "Dokumentnamn")]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required(ErrorMessage = "Dokumentet måste ha en beskrivning!")]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Dokumentet måste ha uppladdad av!")]
        [Display(Name = "Uppladdad av")]
        public string UploadedBy { get; set; }

        [Required(ErrorMessage = "Dokumentet måste ha ett registreringsdatum!")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)] //g Default date & time 10/12/2002 10:11 PM
        [Display(Name = "Registreringsdatum")]
        public DateTime TimeOfRegistration { get; set; }

        [Display(Name = "Dokumentets deadline")]
        public DateTime? Deadline { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        [Display(Name = "Feedback")]
        public string FeedbackFromTeacherToStudent { get; set; }

        // navigation property
        [Display(Name = "Användare")]
        public virtual ICollection<ApplicationUser> DocumentUsers { get; set; }

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