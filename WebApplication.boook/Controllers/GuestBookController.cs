using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication.boook.Data;
using WebApplication.boook.Models;
using System.Diagnostics;

namespace WebApplication.boook.Controllers
{
    public class GuestBookController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10; // Пагинация по 10 записей

        public GuestBookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GuestBook
        public async Task<IActionResult> Index(int page = 1)
        {
            // Проверяем, есть ли данные в базе
            if (!await _context.GuestBookEntries.AnyAsync())
            {
                // Добавляем тестовые данные при первом запуске
                _context.GuestBookEntries.AddRange(
                    new GuestBookEntry
                    {
                        Author = "Администратор",
                        Text = "Добро пожаловать в гостевую книгу!",
                        CreatedAt = DateTime.UtcNow
                    },
                    new GuestBookEntry
                    {
                        Author = "Пользователь",
                        Text = "Отличный функционал!",
                        CreatedAt = DateTime.UtcNow.AddHours(-2)
                    },
                    new GuestBookEntry
                    {
                        Author = "Гость",
                        Text = "Спасибо за возможность оставить отзыв",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                );
                await _context.SaveChangesAsync();
            }

            var totalCount = await _context.GuestBookEntries.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            
            // Проверяем корректность номера страницы
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var entries = await _context.GuestBookEntries
                .OrderByDescending(e => e.CreatedAt) // Сортировка: новые в начале
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(entries);
        }

        // GET: GuestBook/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GuestBook/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Author,Text")] GuestBookEntry entry)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    entry.CreatedAt = DateTime.UtcNow;
                    _context.Add(entry);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Отзыв успешно добавлен!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ошибка при сохранении: {ex.Message}");
                }
            }
            
            // Если есть ошибки валидации, возвращаем обратно на форму
            return View(entry);
        }

        // GET: GuestBook/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.GuestBookEntries.FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }
            
            return View(entry);
        }

        // POST: GuestBook/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Author,Text,CreatedAt")] GuestBookEntry entry)
        {
            if (id != entry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entry);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Отзыв успешно обновлен!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntryExists(entry.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ошибка при обновлении: {ex.Message}");
                }
            }
            
            return View(entry);
        }

        // GET: GuestBook/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.GuestBookEntries
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        // POST: GuestBook/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry = await _context.GuestBookEntries.FindAsync(id);
            if (entry != null)
            {
                _context.GuestBookEntries.Remove(entry);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Отзыв успешно удален!";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: GuestBook/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entry = await _context.GuestBookEntries
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (entry == null)
            {
                return NotFound();
            }

            return View(entry);
        }

        private bool EntryExists(int id)
        {
            return _context.GuestBookEntries.Any(e => e.Id == id);
        }
    }
}