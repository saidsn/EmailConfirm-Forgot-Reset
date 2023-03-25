using JuanProject.DAL;
using JuanProject.Models;
using JuanProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace JuanProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;
        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Contact> contacts = _context.Contacts.Where(c=>!c.IsDeleted).ToList();
            List<ContactListVM> contactLists = new();
            foreach (Contact contact in contacts)
            {
                ContactListVM newContact = new ContactListVM()
                { 
                   Id= contact.Id,
                   Name= contact.Name,
                   Email= contact.Email,
                   PhoneNumber= contact.PhoneNumber,
                   Subject= contact.Subject,
                };
                contactLists.Add(newContact);
            }
            return View(contactLists);
        }


        public IActionResult Detail(int? id) 
        {
            if (id == null) return NotFound();

            Contact contact = _context.Contacts.Find(id);

            if (contact == null) return NotFound();

            ContactDretailVM contactDretailVM = new()
            { 
                Id= contact.Id,
                Name= contact.Name,
                Email= contact.Email,
                PhoneNumber= contact.PhoneNumber,
                Subject= contact.Subject,
                Message= contact.Message,
            };


            return View(contactDretailVM);
        }

        public async Task<IActionResult> Delete(int id)
        { 
            if(id== null) return NotFound();

            Contact contact = await _context.Contacts.FindAsync(id);

            if (contact == null) return NotFound();

            _context.Contacts.Remove(contact);

            _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
