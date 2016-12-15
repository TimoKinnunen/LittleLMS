using System;
using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSViewModels
{
    public class DocumentCourseViewModel
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime CourseStartDate { get; set; }
        public bool IsSelected { get; set; }
    }
}