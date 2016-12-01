﻿using System.ComponentModel.DataAnnotations;

namespace LittleLMS.LittleLMSModels
{
    public class ActivityType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Activity type")]
        public string Name { get; set; } = "E-learning";
    }
}