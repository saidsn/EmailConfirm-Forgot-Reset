using JuanProject.DAL;
using JuanProject.Models;
using JuanProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JuanProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {

            List<Category> categories = await _context.Categories.Where(m => !m.IsDeleted).ToListAsync();

            List<CategoryListVM> categoryList = new();

            foreach (var category in categories)
            {
                CategoryListVM newCategory = new CategoryListVM
                {
                    Id = category.Id,
                    Name = category.Name,
                };
                categoryList.Add(newCategory);

            }

            return View(categoryList);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM category)
        {
            if (!ModelState.IsValid) return View();

            bool isExsist = await _context.Categories
                .Where(m => !m.IsDeleted)
                .AnyAsync(m => m.Name.ToLower().Trim() == category.Name.ToLower().Trim());

            if (isExsist)
            {
                ModelState.AddModelError("Name", "Name already exsist");
                return View();
            }

            Category newCategory = new()
            {
                Name = category.Name,
            };

            await _context.Categories.AddAsync(newCategory);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();

            Category category = await _context.Categories.FindAsync(id);

            if (category == null) return NotFound();

            CategoryDetailVM categoryDetailVM = new()
            {
                Name = category.Name,
            };

            return View(categoryDetailVM);


        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) return NotFound();

            Category category = await _context.Categories.FindAsync(id);

            if (category == null) return NotFound();

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");


        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null) return NotFound();

            Category category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if (category == null) return NotFound();

            CategoryEditVM categoryEditVM = new()
            {
                Id= category.Id,
                Name = category.Name,
            };

            return View(categoryEditVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryEditVM category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Category dbCategory = await _context.Categories.Where(m => m.IsDeleted == false).FirstOrDefaultAsync(m => m.Id == id);

            if (dbCategory == null) return NotFound();

    

            if (dbCategory.Name.ToLower().Trim() == category.Name.ToLower().Trim())
            {
                ModelState.AddModelError("Name", "Name already exsist");
                return View();
            }
            else
            {
                dbCategory.Name = category.Name;
            }

            bool isExsist = await _context.Categories
           .Where(m => !m.IsDeleted)
           .AnyAsync(m => m.Name.ToLower().Trim() == category.Name.ToLower().Trim());

            if (isExsist)
            {
                ModelState.AddModelError("Name", "Name already exsist");
                return View();
            }

            _context.Categories.Update(dbCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }
    }
}
