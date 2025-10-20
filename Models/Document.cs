using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class Document
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string FileName { get; set; }

        public string OriginalFileName { get; set; }

        public string FilePath { get; set; }

        public long FileSize { get; set; }

        public string ContentType { get; set; }

        public DateTime UploadedDate { get; set; } = DateTime.Now;

        public string UploadedBy { get; set; }

        public string ClaimId { get; set; }
        public Claim Claim { get; set; }
    }
}