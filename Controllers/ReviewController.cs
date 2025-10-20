using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ReviewController : Controller
    {
        private static List<Claim> _claims;

        public ReviewController()
        {
            // Initialize with sample data matching the ClaimsController
            if (_claims == null)
            {
                _claims = new List<Claim>
                {
                    new Claim
                    {
                        Id = "CL-2023-0087",
                        ClaimMonth = "2023-10",
                        HoursWorked = 75,
                        HourlyRate = 89.75m,
                        Description = "Monthly teaching services for undergraduate courses",
                        LecturerName = "Dr. Sarah Johnson",
                        Status = ClaimStatus.Approved,
                        SubmittedDate = DateTime.Parse("2023-10-25"),
                        ReviewedBy = "Academic Manager",
                        ReviewedDate = DateTime.Parse("2023-10-28"),
                        SupportingDocuments = new List<Document>
                        {
                            new Document {
                                Id = Guid.NewGuid().ToString(),
                                OriginalFileName = "timesheet.pdf",
                                FileSize = 2450000,
                                UploadedDate = DateTime.Parse("2023-10-25")
                            },
                            new Document {
                                Id = Guid.NewGuid().ToString(),
                                OriginalFileName = "contract.pdf",
                                FileSize = 1800000,
                                UploadedDate = DateTime.Parse("2023-10-25")
                            }
                        }
                    },
                    new Claim
                    {
                        Id = "CL-2023-0088",
                        ClaimMonth = "2023-10",
                        HoursWorked = 85,
                        HourlyRate = 92.35m,
                        Description = "Research supervision and thesis review for postgraduate students",
                        LecturerName = "Prof. Michael Brown",
                        Status = ClaimStatus.Submitted,
                        SubmittedDate = DateTime.Parse("2023-10-28"),
                        SupportingDocuments = new List<Document>
                        {
                            new Document {
                                Id = Guid.NewGuid().ToString(),
                                OriginalFileName = "timesheet.pdf",
                                FileSize = 2100000,
                                UploadedDate = DateTime.Parse("2023-10-28")
                            },
                            new Document {
                                Id = Guid.NewGuid().ToString(),
                                OriginalFileName = "approval_letter.pdf",
                                FileSize = 3200000,
                                UploadedDate = DateTime.Parse("2023-10-28")
                            }
                        }
                    },
                    new Claim
                    {
                        Id = "CL-2023-0090",
                        ClaimMonth = "2023-10",
                        HoursWorked = 92,
                        HourlyRate = 95.50m,
                        Description = "Course development and lecture delivery",
                        LecturerName = "Dr. Robert Wilson",
                        Status = ClaimStatus.Submitted,
                        SubmittedDate = DateTime.Parse("2023-11-02"),
                        SupportingDocuments = new List<Document>
                        {
                            new Document {
                                Id = Guid.NewGuid().ToString(),
                                OriginalFileName = "timesheet_signed.pdf",
                                FileSize = 2500000,
                                UploadedDate = DateTime.Parse("2023-11-02")
                            }
                        }
                    },
                    new Claim
                    {
                        Id = "CL-2023-0089",
                        ClaimMonth = "2023-10",
                        HoursWorked = 60,
                        HourlyRate = 85.25m,
                        Description = "Guest lectures and workshop facilitation",
                        LecturerName = "Dr. Emily Chen",
                        Status = ClaimStatus.Rejected,
                        SubmittedDate = DateTime.Parse("2023-11-01"),
                        ReviewComments = "Insufficient documentation provided for workshop activities.",
                        ReviewedBy = "Academic Manager",
                        ReviewedDate = DateTime.Parse("2023-11-03"),
                        SupportingDocuments = new List<Document>
                        {
                            new Document {
                                Id = Guid.NewGuid().ToString(),
                                OriginalFileName = "workshop_materials.zip",
                                FileSize = 15700000,
                                UploadedDate = DateTime.Parse("2023-11-01")
                            }
                        }
                    },
                    new Claim
                    {
                        Id = "CL-2023-0091",
                        ClaimMonth = "2023-11",
                        HoursWorked = 80,
                        HourlyRate = 88.00m,
                        Description = "Student assessment and marking",
                        LecturerName = "Dr. Sarah Johnson",
                        Status = ClaimStatus.Submitted,
                        SubmittedDate = DateTime.Parse("2023-11-05"),
                        SupportingDocuments = new List<Document>
                        {
                            new Document {
                                Id = Guid.NewGuid().ToString(),
                                OriginalFileName = "marking_schedule.pdf",
                                FileSize = 1800000,
                                UploadedDate = DateTime.Parse("2023-11-05")
                            }
                        }
                    }
                };
            }
        }

        public IActionResult Index()
        {
            var pendingClaims = _claims.Where(c => c.Status == ClaimStatus.Submitted).ToList();
            return View(pendingClaims);
        }

        public IActionResult AllClaims(string status = "all", string search = "", string lecturer = "",
                                     DateTime? dateFrom = null, DateTime? dateTo = null, int page = 1)
        {
            var claims = _claims.AsEnumerable();

            // Filter by status
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                var statusEnum = Enum.Parse<ClaimStatus>(status, true);
                claims = claims.Where(c => c.Status == statusEnum);
            }

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                claims = claims.Where(c =>
                    c.Id.ToLower().Contains(search) ||
                    c.LecturerName.ToLower().Contains(search) ||
                    (c.Description != null && c.Description.ToLower().Contains(search)) ||
                    c.ClaimMonth.ToLower().Contains(search)
                );
            }

            // Filter by lecturer
            if (!string.IsNullOrEmpty(lecturer))
            {
                claims = claims.Where(c => c.LecturerName == lecturer);
            }

            // Date range filter
            if (dateFrom.HasValue)
            {
                claims = claims.Where(c => c.SubmittedDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                claims = claims.Where(c => c.SubmittedDate <= dateTo.Value);
            }

            // Get unique lecturers for dropdown
            var availableLecturers = _claims.Select(c => c.LecturerName).Distinct().ToList();

            // Get unique months for dropdown
            var availableMonths = _claims.Select(c => DateTime.Parse(c.ClaimMonth + "-01").ToString("yyyy-MM"))
                                       .Distinct()
                                       .OrderDescending()
                                       .ToList();

            // Calculate statistics
            var totalClaimsCount = _claims.Count;
            var pendingClaimsCount = _claims.Count(c => c.Status == ClaimStatus.Submitted);
            var approvedClaimsCount = _claims.Count(c => c.Status == ClaimStatus.Approved);
            var rejectedClaimsCount = _claims.Count(c => c.Status == ClaimStatus.Rejected);
            var totalApprovedAmount = _claims.Where(c => c.Status == ClaimStatus.Approved).Sum(c => c.TotalAmount);

            // Pagination
            var pageSize = 10;
            var totalPages = (int)Math.Ceiling((double)claims.Count() / pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));
            var pagedClaims = claims.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new ReviewViewModel
            {
                Claims = pagedClaims,
                FilterStatus = status,
                SearchTerm = search,
                SelectedLecturer = lecturer,
                AvailableLecturers = availableLecturers,
                AvailableMonths = availableMonths,
                DateFrom = dateFrom,
                DateTo = dateTo,
                CurrentPage = page,
                PageSize = pageSize,
                TotalClaimsCount = totalClaimsCount,
                PendingClaimsCount = pendingClaimsCount,
                ApprovedClaimsCount = approvedClaimsCount,
                RejectedClaimsCount = rejectedClaimsCount,
                TotalApprovedAmount = totalApprovedAmount
            };

            return View(viewModel);
        }

        public IActionResult Details(string id)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(string id, string comments)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = ClaimStatus.Approved;
            claim.ReviewComments = comments;
            claim.ReviewedBy = User.Identity?.Name ?? "Academic Manager"; // Current user
            claim.ReviewedDate = DateTime.Now;

            TempData["Success"] = $"Claim {id} has been approved successfully.";
            return RedirectToAction(nameof(AllClaims));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(string id, string comments)
        {
            if (string.IsNullOrWhiteSpace(comments))
            {
                TempData["Error"] = "Please provide comments when rejecting a claim.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var claim = _claims.FirstOrDefault(c => c.Id == id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = ClaimStatus.Rejected;
            claim.ReviewComments = comments;
            claim.ReviewedBy = User.Identity?.Name ?? "Academic Manager"; // Current user
            claim.ReviewedDate = DateTime.Now;

            TempData["Success"] = $"Claim {id} has been rejected.";
            return RedirectToAction(nameof(AllClaims));
        }

        [HttpPost]
        public IActionResult BulkApprove(List<string> claimIds, string comments = "")
        {
            if (claimIds == null || claimIds.Count == 0)
            {
                TempData["Error"] = "Please select at least one claim to approve.";
                return RedirectToAction(nameof(AllClaims));
            }

            var approvedCount = 0;
            foreach (var claimId in claimIds)
            {
                var claim = _claims.FirstOrDefault(c => c.Id == claimId);
                if (claim != null && claim.Status == ClaimStatus.Submitted)
                {
                    claim.Status = ClaimStatus.Approved;
                    claim.ReviewComments = comments;
                    claim.ReviewedBy = User.Identity?.Name ?? "Academic Manager"; // Current user
                    claim.ReviewedDate = DateTime.Now;
                    approvedCount++;
                }
            }

            TempData["Success"] = $"Successfully approved {approvedCount} claim(s).";
            return RedirectToAction(nameof(AllClaims));
        }

        [HttpPost]
        public IActionResult BulkReject(List<string> claimIds, string comments)
        {
            if (claimIds == null || claimIds.Count == 0)
            {
                TempData["Error"] = "Please select at least one claim to reject.";
                return RedirectToAction(nameof(AllClaims));
            }

            if (string.IsNullOrWhiteSpace(comments))
            {
                TempData["Error"] = "Please provide comments when rejecting claims.";
                return RedirectToAction(nameof(AllClaims));
            }

            var rejectedCount = 0;
            foreach (var claimId in claimIds)
            {
                var claim = _claims.FirstOrDefault(c => c.Id == claimId);
                if (claim != null && claim.Status == ClaimStatus.Submitted)
                {
                    claim.Status = ClaimStatus.Rejected;
                    claim.ReviewComments = comments;
                    claim.ReviewedBy = User.Identity?.Name ?? "Academic Manager"; // Current user
                    claim.ReviewedDate = DateTime.Now;
                    rejectedCount++;
                }
            }

            TempData["Success"] = $"Successfully rejected {rejectedCount} claim(s).";
            return RedirectToAction(nameof(AllClaims));
        }

        public IActionResult ExportToExcel(string status = "all", string search = "", string lecturer = "")
        {
            // In a real application, this would generate an Excel file
            // For now, we'll just return a success message
            var filteredClaims = _claims.AsEnumerable();

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                var statusEnum = Enum.Parse<ClaimStatus>(status, true);
                filteredClaims = filteredClaims.Where(c => c.Status == statusEnum);
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                filteredClaims = filteredClaims.Where(c =>
                    c.Id.ToLower().Contains(search) ||
                    c.LecturerName.ToLower().Contains(search)
                );
            }

            if (!string.IsNullOrEmpty(lecturer))
            {
                filteredClaims = filteredClaims.Where(c => c.LecturerName == lecturer);
            }

            var count = filteredClaims.Count();
            TempData["Success"] = $"Exported {count} claims to Excel successfully.";
            return RedirectToAction(nameof(AllClaims), new { status, search, lecturer });
        }

        public IActionResult Statistics()
        {
            var statistics = new ReviewStatistics
            {
                TotalClaims = _claims.Count,
                ClaimsThisMonth = _claims.Count(c => c.SubmittedDate.Month == DateTime.Now.Month && c.SubmittedDate.Year == DateTime.Now.Year),
                PendingReview = _claims.Count(c => c.Status == ClaimStatus.Submitted),
                TotalAmountThisMonth = _claims
                    .Where(c => c.SubmittedDate.Month == DateTime.Now.Month && c.SubmittedDate.Year == DateTime.Now.Year)
                    .Sum(c => c.TotalAmount),
                ClaimsProcessedToday = _claims.Count(c => c.ReviewedDate?.Date == DateTime.Today),
                MostActiveReviewer = "Academic Manager",
                AverageApprovalRate = 75.5,
                AverageRejectionRate = 24.5
            };

            // Calculate monthly trends
            var monthlyGroups = _claims
                .GroupBy(c => DateTime.Parse(c.ClaimMonth + "-01").ToString("yyyy-MM"))
                .OrderBy(g => g.Key);

            foreach (var group in monthlyGroups)
            {
                statistics.MonthlyTrends.Add(new MonthlyTrend
                {
                    Month = DateTime.Parse(group.Key + "-01").ToString("MMM yyyy"),
                    SubmittedCount = group.Count(c => c.Status == ClaimStatus.Submitted),
                    ApprovedCount = group.Count(c => c.Status == ClaimStatus.Approved),
                    RejectedCount = group.Count(c => c.Status == ClaimStatus.Rejected),
                    TotalAmount = group.Sum(c => c.TotalAmount)
                });
            }

            // Calculate top lecturers
            var lecturerGroups = _claims
                .GroupBy(c => c.LecturerName)
                .OrderByDescending(g => g.Sum(c => c.TotalAmount))
                .Take(5);

            foreach (var group in lecturerGroups)
            {
                statistics.TopLecturers.Add(new LecturerSummary
                {
                    LecturerName = group.Key,
                    TotalClaims = group.Count(),
                    ApprovedClaims = group.Count(c => c.Status == ClaimStatus.Approved),
                    RejectedClaims = group.Count(c => c.Status == ClaimStatus.Rejected),
                    TotalAmount = group.Sum(c => c.TotalAmount)
                });
            }

            return View(statistics);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}