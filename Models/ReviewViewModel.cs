using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class ReviewViewModel
    {
        public List<Claim> Claims { get; set; } = new List<Claim>();

        [Display(Name = "Filter by Status")]
        public string FilterStatus { get; set; } = "all";

        [Display(Name = "Search")]
        public string SearchTerm { get; set; }

        // Statistics for the review dashboard
        public int TotalClaimsCount { get; set; }
        public int PendingClaimsCount { get; set; }
        public int ApprovedClaimsCount { get; set; }
        public int RejectedClaimsCount { get; set; }
        public decimal TotalApprovedAmount { get; set; }

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)TotalClaimsCount / PageSize);

        // Filter options
        public List<string> AvailableStatuses => new List<string>
        {
            "all", "draft", "submitted", "approved", "rejected"
        };

        public List<string> AvailableMonths { get; set; } = new List<string>();

        [Display(Name = "Date From")]
        [DataType(DataType.Date)]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Date To")]
        [DataType(DataType.Date)]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Lecturer")]
        public string SelectedLecturer { get; set; }

        public List<string> AvailableLecturers { get; set; } = new List<string>();

        // Bulk action support
        public List<string> SelectedClaimIds { get; set; } = new List<string>();

        // Chart data for analytics
        public Dictionary<string, int> ClaimsByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ClaimsByMonth { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> AmountByLecturer { get; set; } = new Dictionary<string, decimal>();
    }

    // Additional supporting classes for more complex review functionality
    public class ClaimReviewAction
    {
        public string ClaimId { get; set; }
        public string Action { get; set; } // "approve" or "reject"
        public string Comments { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime ReviewedAt { get; set; } = DateTime.Now;
    }

    public class BulkReviewRequest
    {
        public List<string> ClaimIds { get; set; } = new List<string>();
        public string Action { get; set; }
        public string Comments { get; set; }
        public string ReviewedBy { get; set; }
    }

    public class ReviewStatistics
    {
        public int TotalClaims { get; set; }
        public int ClaimsThisMonth { get; set; }
        public int PendingReview { get; set; }
        public decimal TotalAmountThisMonth { get; set; }
        public decimal AverageProcessingTimeHours { get; set; }
        public string MostActiveReviewer { get; set; }
        public int ClaimsProcessedToday { get; set; }

        // Performance metrics
        public double AverageApprovalRate { get; set; }
        public double AverageRejectionRate { get; set; }
        public TimeSpan AverageReviewTime { get; set; }

        // Trend data
        public List<MonthlyTrend> MonthlyTrends { get; set; } = new List<MonthlyTrend>();
        public List<LecturerSummary> TopLecturers { get; set; } = new List<LecturerSummary>();
    }

    public class MonthlyTrend
    {
        public string Month { get; set; }
        public int SubmittedCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class LecturerSummary
    {
        public string LecturerName { get; set; }
        public int TotalClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public int RejectedClaims { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ApprovalRate => TotalClaims > 0 ? (decimal)ApprovedClaims / TotalClaims * 100 : 0;
    }

    public class ReviewFilter
    {
        public string Status { get; set; }
        public string SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Lecturer { get; set; }
        public string Department { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public int? MinHours { get; set; }
        public int? MaxHours { get; set; }
        public string SortBy { get; set; } = "SubmittedDate";
        public string SortOrder { get; set; } = "desc";
    }

    public class ExportRequest
    {
        public ReviewFilter Filter { get; set; } = new ReviewFilter();
        public string Format { get; set; } = "excel"; // excel, pdf, csv
        public List<string> Columns { get; set; } = new List<string>();
        public bool IncludeDocuments { get; set; }
    }
}