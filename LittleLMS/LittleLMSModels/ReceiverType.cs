using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSModels
{
    public class ReceiverType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Mottagartyp")]
        public string Name { get; set; }
    }
}