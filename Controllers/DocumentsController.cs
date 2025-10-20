using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private static List<Document> _documents = new List<Document>();

        public DocumentsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View(_documents);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(List<IFormFile> files, string claimId = null)
        {
            if (files == null || files.Count == 0)
            {
                ModelState.AddModelError("", "Please select at least one file to upload.");
                return View();
            }

            var uploadedFiles = new List<Document>();
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");

            // Create uploads directory if it doesn't exist
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Generate unique filename
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var document = new Document
                    {
                        Id = Guid.NewGuid().ToString(),
                        OriginalFileName = file.FileName,
                        FileName = fileName,
                        FilePath = filePath,
                        FileSize = file.Length,
                        ContentType = file.ContentType,
                        UploadedDate = DateTime.Now,
                        UploadedBy = "Dr. Sarah Johnson", // Current user
                        ClaimId = claimId
                    };

                    _documents.Add(document);
                    uploadedFiles.Add(document);
                }
            }

            TempData["Success"] = $"Successfully uploaded {uploadedFiles.Count} file(s).";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(string id)
        {
            var document = _documents.FirstOrDefault(d => d.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            if (!System.IO.File.Exists(document.FilePath))
            {
                return NotFound("File not found on server.");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(document.FilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, document.ContentType, document.OriginalFileName);
        }

        public IActionResult View(string id)
        {
            var document = _documents.FirstOrDefault(d => d.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            // For PDF and images, we can display in browser
            if (document.ContentType.StartsWith("image/") ||
                document.ContentType == "application/pdf")
            {
                var fileBytes = System.IO.File.ReadAllBytes(document.FilePath);
                return File(fileBytes, document.ContentType);
            }

            // For other file types, force download
            return RedirectToAction(nameof(Download), new { id });
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var document = _documents.FirstOrDefault(d => d.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            // Delete physical file
            if (System.IO.File.Exists(document.FilePath))
            {
                System.IO.File.Delete(document.FilePath);
            }

            _documents.Remove(document);

            TempData["Success"] = "Document deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}