using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Display(Name = "Namn")]
        public string FullName { get { return FirstName + " " + LastName; } }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Roll")]
        public string RoleAsText { get; set; }

        public int CourseId { get; set; }
    }
}