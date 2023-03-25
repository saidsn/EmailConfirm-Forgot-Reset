using JuanProject.DAL;
using JuanProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace JuanProject.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly AppDbContext _context;
        public ContactUsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Contact contact)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }


            _context.Contacts.Add(contact);

            _context.SaveChanges();
        
            return RedirectToAction(nameof(Index));
        }
    }
}
