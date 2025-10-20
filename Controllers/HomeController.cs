using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // In a real application, you would get this data from a database
            var dashboardData = new DashboardViewModel
            {
                PendingClaimsCount = 5,
                ApprovedClaimsCount = 12,
                AttentionRequiredCount = 3,
                TotalValue = 42560,
                RecentClaims = GetRecentClaims(),
                Notifications = GetNotifications()
            };

            return View(dashboardData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<Claim> GetRecentClaims()
        {
            return new List<Claim>
            {
                new Claim
                {
                    Id = "CL-2023-0087",
                    SubmittedDate = DateTime.Parse("2023-10-25"),
                    LecturerName = "Dr. Sarah Johnson",
                    HoursWorked = 75,
                    HourlyRate = 89.75m,
                    Status = ClaimStatus.Approved
                },
                new Claim
                {
                    Id = "CL-2023-0088",
                    SubmittedDate = DateTime.Parse("2023-10-28"),
                    LecturerName = "Prof. Michael Brown",
                    HoursWorked = 85,
                    HourlyRate = 92.35m,
                    Status = ClaimStatus.Submitted
                },
                new Claim
                {
                    Id = "CL-2023-0089",
                    SubmittedDate = DateTime.Parse("2023-11-01"),
                    LecturerName = "Dr. Emily Chen",
                    HoursWorked = 60,
                    HourlyRate = 85.25m,
                    Status = ClaimStatus.Rejected
                }
            };
        }

        private List<Notification> GetNotifications()
        {
            return new List<Notification>
            {
                new Notification
                {
                    Type = NotificationType.Warning,
                    Title = "Attention Required",
                    Message = "3 claims require your approval",
                    CreatedDate = DateTime.Now.AddHours(-2)
                },
                new Notification
                {
                    Type = NotificationType.Info,
                    Title = "System Maintenance",
                    Message = "System maintenance scheduled for Sunday, 3:00 AM - 6:00 AM",
                    CreatedDate = DateTime.Now.AddDays(-1)
                },
                new Notification
                {
                    Type = NotificationType.Success,
                    Title = "Claim Approved",
                    Message = "Your claim CL-2023-0087 has been approved",
                    CreatedDate = DateTime.Now.AddHours(-4)
                }
            };
        }
    }

    public class DashboardViewModel
    {
        public int PendingClaimsCount { get; set; }
        public int ApprovedClaimsCount { get; set; }
        public int AttentionRequiredCount { get; set; }
        public decimal TotalValue { get; set; }
        public List<Claim> RecentClaims { get; set; } = new List<Claim>();
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }

    public class Notification
    {
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Danger
    }
}