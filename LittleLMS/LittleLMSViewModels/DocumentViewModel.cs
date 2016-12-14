using LittleLMS.LittleLMSModels;
using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSViewModels
{
    public class DocumentViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Dokumentet måste ha en dokumenttyp!")]
        [Display(Name = "Dokumenttyp")]
        public int DocumentTypeId { get; set; }

        public virtual DocumentType DocumentType { get; set; }

        [Required(ErrorMessage = "Dokumentet måste ha en mottagartyp!")]
        [Display(Name = "Mottagartyp")]
        public int ReceiverTypeId { get; set; }

        public virtual ReceiverType ReceiverType { get; set; }

        [Required(ErrorMessage = "Dokumentet måste ha ett namn!")]
        [Display(Name = "Dokumentnamn")]
        [StringLength(255)]
        public string DocumentName { get; set; }

        [Required(ErrorMessage = "Dokumentet måste ha en beskrivning!")]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }
    }
}