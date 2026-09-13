using BookCatalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly BookCatalogContext _context;
        public AuthorsController(BookCatalogContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var authors = await _context.Authors
                .Select(a => new { Author = a, BookCount = a.Books.Count() })
                .ToListAsync();
            ViewBag.BookCounts = authors.ToDictionary(x => x.Author.Id, x => x.BookCount);
            return View(await _context.Authors.ToListAsync());
        }

        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();
            return View(author);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Author author)
        {
            if (id != author.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            // Проверка, есть ли книги
            var hasBooks = await _context.Books.AnyAsync(b => b.AuthorId == id);
            if (hasBooks)
                TempData["Error"] = "Нельзя удалить автора, у которого есть книги.";
            else
                return View(author);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}