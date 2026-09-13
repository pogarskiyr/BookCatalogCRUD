using BookCatalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookCatalogContext _context;

        public BooksController(BookCatalogContext context)
        {
            _context = context;
        }

        // GET: Books (главная страница с поиском и фильтром)
        public async Task<IActionResult> Index(string searchString, int? authorId)
        {
            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .AsQueryable();

            // Поиск по названию
            if (!string.IsNullOrEmpty(searchString))
                books = books.Where(b => b.Title.Contains(searchString));

            // Фильтр по автору
            if (authorId.HasValue && authorId.Value > 0)
                books = books.Where(b => b.AuthorId == authorId.Value);

            // Сортировка по году (новые сверху)
            books = books.OrderByDescending(b => b.Year);

            // Передаём список авторов для выпадающего списка фильтра
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.CurrentAuthorId = authorId;
            ViewBag.CurrentSearch = searchString;

            return View(await books.ToListAsync());
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Year,Price,AuthorId,GenreId")] Book book)
        {
            if (ModelState.IsValid)
            {
                // Проверка, что автор и жанр существуют (доп. безопасность)
                var authorExists = await _context.Authors.AnyAsync(a => a.Id == book.AuthorId);
                var genreExists = await _context.Genres.AnyAsync(g => g.Id == book.GenreId);
                if (!authorExists || !genreExists)
                {
                    ModelState.AddModelError("", "Выбранный автор или жанр не существуют.");
                    ViewBag.Authors = await _context.Authors.ToListAsync();
                    ViewBag.Genres = await _context.Genres.ToListAsync();
                    return View(book);
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Книга успешно добавлена!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,Price,AuthorId,GenreId")] Book book)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var authorExists = await _context.Authors.AnyAsync(a => a.Id == book.AuthorId);
                    var genreExists = await _context.Genres.AnyAsync(g => g.Id == book.GenreId);
                    if (!authorExists || !genreExists)
                    {
                        ModelState.AddModelError("", "Выбранный автор или жанр не существуют.");
                        ViewBag.Authors = await _context.Authors.ToListAsync();
                        ViewBag.Genres = await _context.Genres.ToListAsync();
                        return View(book);
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Книга обновлена!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Books.Any(e => e.Id == book.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Книга удалена.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}