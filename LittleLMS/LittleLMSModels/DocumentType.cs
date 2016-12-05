using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSModels
{
    public class DocumentType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Dokument typ")]
        public string Name { get; set; } = "Inlämningsuppgift";
    }
}