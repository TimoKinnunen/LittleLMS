using System;
using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSModels
{
    public class Overview {
        public int Id { get; set; }

        [Display(Name = "Course name")]
        public string CourseName { get; set; }

        [Display(Name = "Description")]
        public string CourseDescription { get; set; }

        [Display(Name = "Course start date")]
        public DateTime CourseStartDate { get; set; }

        [Display(Name = "Module name")]
        public string ModuleName { get; set; }

        [Display(Name = "Description")]
        public string ModuleDescription { get; set; }

        [Display(Name = "Module start date")]
        public DateTime ModuleStartDate { get; set; }

        [Display(Name = "Module end date")]
        public DateTime ModuleEndDate { get; set; }
    }
}