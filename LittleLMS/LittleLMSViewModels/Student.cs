using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSViewModels
{
    public class Student
    {
        [Required]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "E-post adress")]
        public string Email { get; set; }

        public int CourseId { get; set; }
    }
}