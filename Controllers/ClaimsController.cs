using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        private static List<Claim> _claims = new List<Claim>();
        private static int _claimCounter = 90;

        public ClaimsController()
        {
            // Initialize with sample data
            if (_claims.Count == 0)
            {
                _claims.AddRange(new List<Claim>
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
                        SubmittedDate = DateTime.Parse("2023-10-25")
                    },
                    new Claim
                    {
                        Id = "CL-2023-0088",
                        ClaimMonth = "2023-10",
                        HoursWorked = 85,
                        HourlyRate = 92.35m,
                        Description = "Research supervision and thesis review",
                        LecturerName = "Prof. Michael Brown",
                        Status = ClaimStatus.Submitted,
                        SubmittedDate = DateTime.Parse("2023-10-28")
                    }
                });
            }
        }

        public IActionResult Index()
        {
            return View(_claims.Where(c => c.Status != ClaimStatus.Draft).ToList());
        }

        public IActionResult MyClaims()
        {
            // In real app, filter by current user
            var myClaims = _claims.Where(c => c.LecturerName == "Dr. Sarah Johnson").ToList();
            return View(myClaims);
        }

        public IActionResult Create()
        {
            var claim = new Claim
            {
                ClaimMonth = DateTime.Now.ToString("yyyy-MM")
            };
            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim claim, List<IFormFile> supportingDocuments)
        {
            if (ModelState.IsValid)
            {
                // Generate claim ID
                _claimCounter++;
                claim.Id = $"CL-2023-{_claimCounter:D4}";
                claim.LecturerName = "Dr. Sarah Johnson"; // Current user
                claim.Status = ClaimStatus.Submitted;
                claim.SubmittedDate = DateTime.Now;

                // Handle file uploads
                if (supportingDocuments != null && supportingDocuments.Count > 0)
                {
                    claim.SupportingDocuments = new List<Document>();

                    foreach (var file in supportingDocuments)
                    {
                        if (file.Length > 0)
                        {
                            var document = new Document
                            {
                                Id = Guid.NewGuid().ToString(),
                                OriginalFileName = file.FileName,
                                FileName = $"{Guid.NewGuid()}_{file.FileName}",
                                FileSize = file.Length,
                                ContentType = file.ContentType,
                                UploadedDate = DateTime.Now,
                                UploadedBy = "Dr. Sarah Johnson"
                            };

                            // In real application, save file to storage
                            // For now, we'll just store the document info
                            claim.SupportingDocuments.Add(document);
                        }
                    }
                }

                _claims.Add(claim);

                TempData["Success"] = "Claim submitted successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveDraft(Claim claim)
        {
            if (ModelState.IsValid)
            {
                claim.Status = ClaimStatus.Draft;
                claim.LecturerName = "Dr. Sarah Johnson"; // Current user

                if (string.IsNullOrEmpty(claim.Id))
                {
                    _claimCounter++;
                    claim.Id = $"CL-2023-{_claimCounter:D4}";
                    _claims.Add(claim);
                }
                else
                {
                    var existingClaim = _claims.FirstOrDefault(c => c.Id == claim.Id);
                    if (existingClaim != null)
                    {
                        existingClaim.HoursWorked = claim.HoursWorked;
                        existingClaim.HourlyRate = claim.HourlyRate;
                        existingClaim.Description = claim.Description;
                        existingClaim.ClaimMonth = claim.ClaimMonth;
                    }
                }

                TempData["Success"] = "Claim saved as draft successfully!";
                return RedirectToAction(nameof(MyClaims));
            }

            return View("Create", claim);
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

        public IActionResult Edit(string id)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == id);
            if (claim == null)
            {
                return NotFound();
            }

            // Only allow editing of drafts
            if (claim.Status != ClaimStatus.Draft)
            {
                TempData["Error"] = "Only draft claims can be edited.";
                return RedirectToAction(nameof(MyClaims));
            }

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Claim updatedClaim)
        {
            if (id != updatedClaim.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingClaim = _claims.FirstOrDefault(c => c.Id == id);
                if (existingClaim == null)
                {
                    return NotFound();
                }

                existingClaim.HoursWorked = updatedClaim.HoursWorked;
                existingClaim.HourlyRate = updatedClaim.HourlyRate;
                existingClaim.Description = updatedClaim.Description;
                existingClaim.ClaimMonth = updatedClaim.ClaimMonth;

                TempData["Success"] = "Claim updated successfully!";
                return RedirectToAction(nameof(MyClaims));
            }

            return View(updatedClaim);
        }

        [HttpPost]
        public IActionResult SubmitDraft(string id)
        {
            var claim = _claims.FirstOrDefault(c => c.Id == id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.Status = ClaimStatus.Submitted;
            claim.SubmittedDate = DateTime.Now;

            TempData["Success"] = "Claim submitted successfully!";
            return RedirectToAction(nameof(MyClaims));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult MyClaims(string status = "")
        {
            // In real app, filter by current user
            var myClaims = _claims.Where(c => c.LecturerName == "Dr. Sarah Johnson");

            if (!string.IsNullOrEmpty(status))
            {
                var statusEnum = Enum.Parse<ClaimStatus>(status, true);
                myClaims = myClaims.Where(c => c.Status == statusEnum);
            }

            return View(myClaims.ToList());
        }
    }
}