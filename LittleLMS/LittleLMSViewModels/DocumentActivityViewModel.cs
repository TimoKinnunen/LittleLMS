using System;
using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSViewModels
{
    public class DocumentActivityViewModel
    {
        public string ActivityId { get; set; }
        public string ActivityCourseName { get; set; }
        public string ActivityModuleName { get; set; }
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime ActivityStartDate { get; set; }
        public bool IsSelected { get; set; }
    }
}