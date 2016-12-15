using System;
using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSViewModels
{
    public class DocumentModuleViewModel
    {
        public string ModuleId { get; set; }
        public string ModuleCourseName { get; set; }
        public string ModuleName { get; set; }
        public string ModuleDescription { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime ModuleStartDate { get; set; }
        public bool IsSelected { get; set; }
    }
}