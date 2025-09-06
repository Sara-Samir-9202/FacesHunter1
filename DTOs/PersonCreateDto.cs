
// DTOs/PersonCreateDto.cs
using FacesHunter.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FacesHunter.DTOs
{
    public class PersonCreateDto
    {
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [EnumDataType(typeof(Gender), ErrorMessage = "Please select a valid gender")]
        public Gender Gender { get; set; } = Gender.Select;

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = string.Empty;


        [Required(ErrorMessage = "Last seen location is required")]
        public string LastSeenLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "DateMissing is required")]
        public DateTime DateMissing { get; set; }
        public IFormFile? PersonImage { get; set; }


        // ✅ بيانات المبلّغ
        public string? ReporterName { get; set; }
        public string? ReporterPhone { get; set; }

        // ✅ صورة إثبات الهوية (بطاقة)
        //public IFormFile? ReporterIdImage { get; set; }

        [Required(ErrorMessage = "National ID card image is required")]
        public IFormFile? NationalIdCard { get; set; }
        public string? AdditionalInfo { get; set; }

        public string? ImagePath { get; set; }

        public IFormFile? ReporterIdImage { get; set; }

    

    }
}






