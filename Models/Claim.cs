using System;
using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class Claim
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Display(Name = "Claim Month")]
        public string ClaimMonth { get; set; }

        [Required]
        [Range(1, 744, ErrorMessage = "Hours worked must be between 1 and 744")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Hourly rate must be greater than 0")]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount => HoursWorked * HourlyRate;

        public string Description { get; set; }

        [Required]
        public string LecturerId { get; set; }
        public User Lecturer { get; set; }

        public string LecturerName { get; set; }

        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        public ClaimStatus Status { get; set; } = ClaimStatus.Draft;

        public List<Document> SupportingDocuments { get; set; } = new List<Document>();

        public string ReviewComments { get; set; }

        public string ReviewedBy { get; set; }

        public DateTime? ReviewedDate { get; set; }
    }

    public enum ClaimStatus
    {
        Draft,
        Submitted,
        Approved,
        Rejected
    }
}