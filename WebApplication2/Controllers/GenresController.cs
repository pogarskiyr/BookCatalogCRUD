using BookCatalog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Controllers
{
    public class GenresController : Controller
    {
        private readonly BookCatalogContext _context;

        public GenresController(BookCatalogContext context)
        {
            _context = context;
        }

        // GET: Genres
        // Список всех жанров + количество книг в каждом (через LINQ)
        public async Task<IActionResult> Index()
        {
            // Используем LINQ: Select + Count для подсчёта книг
            var genresWithCount = await _context.Genres
                .Select(g => new
                {
                    Genre = g,
                    BookCount = g.Books.Count()
                })
                .ToListAsync();

            // Сохраняем количество книг в ViewBag, чтобы отобразить в таблице
            ViewBag.BookCounts = genresWithCount.ToDictionary(x => x.Genre.Id, x => x.BookCount);

            return View(genresWithCount.Select(x => x.Genre).ToList());
        }

        // GET: Genres/Details/5 — просмотр подробностей (опционально, но полезно)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (genre == null) return NotFound();

            return View(genre);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Genre genre)
        {
            if (ModelState.IsValid)
            {
                // Проверка на дубликат по названию (через LINQ Any)
                bool exists = await _context.Genres.AnyAsync(g => g.Name == genre.Name);
                if (exists)
                {
                    ModelState.AddModelError("Name", "Жанр с таким названием уже существует.");
                    return View(genre);
                }

                _context.Add(genre);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Жанр успешно добавлен!";
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();

            return View(genre);
        }

        // POST: Genres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Genre genre)
        {
            if (id != genre.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Проверка на дубликат (кроме самого себя)
                    bool exists = await _context.Genres
                        .AnyAsync(g => g.Name == genre.Name && g.Id != genre.Id);
                    if (exists)
                    {
                        ModelState.AddModelError("Name", "Жанр с таким названием уже существует.");
                        return View(genre);
                    }

                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Жанр обновлён!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Genres.Any(e => e.Id == genre.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var genre = await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (genre == null) return NotFound();

            // Проверка: если есть книги, нельзя удалять — вернёмся на Index с сообщением
            if (genre.Books.Any())
            {
                TempData["Error"] = $"Нельзя удалить жанр \"{genre.Name}\", так как с ним связано {genre.Books.Count} книг(и).";
                return RedirectToAction(nameof(Index));
            }

            return View(genre);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre != null)
            {
                // Двойная проверка на сервере (на случай обхода через прямой POST)
                if (genre.Books.Any())
                {
                    TempData["Error"] = "Нельзя удалить жанр, у которого есть книги.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Жанр удалён.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}