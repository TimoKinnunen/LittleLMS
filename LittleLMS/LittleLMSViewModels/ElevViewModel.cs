using System;
using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSViewModels
{
    public class ElevViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Roll namn")]
        public string RoleName { get; set; }

        [Display(Name = "Kurs id")]
        public int CourseId { get; set; }

        [Display(Name = "Kurs namn")]
        public string CourseName { get; set; }

        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Display(Name = "Namn")]
        public string FullName { get; set; }

        [Display(Name = "Kurs start datum")]
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)] //g Default date & time 10/12/2002 10:11 PM
        public DateTime TimeOfRegistration { get; set; }
    }
}