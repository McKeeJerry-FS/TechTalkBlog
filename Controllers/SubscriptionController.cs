using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TechTalkBlog.Data;
using TechTalkBlog.Models;

namespace TechTalkBlog.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Subscribe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("Email", "Email is required.");
                return View();
            }

            if (_context.Subscribers.Any(s => s.Email == email))
            {
                ViewBag.Message = "You are already subscribed!";
                return View();
            }

            var subscriber = new Subscriber { Email = email };
            _context.Subscribers.Add(subscriber);
            await _context.SaveChangesAsync();
            ViewBag.Message = "Thank you for subscribing!";
            return View();
        }

        [HttpGet]
        public IActionResult Unsubscribe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unsubscribe(string email)
        {
            var subscriber = _context.Subscribers.FirstOrDefault(s => s.Email == email);
            if (subscriber != null)
            {
                _context.Subscribers.Remove(subscriber);
                await _context.SaveChangesAsync();
                ViewBag.Message = "You have been unsubscribed.";
            }
            else
            {
                ViewBag.Message = "Email not found.";
            }
            return View();
        }
    }
}
