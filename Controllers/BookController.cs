﻿using ContosoBooks.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoBooks.Controllers
{
    public class BookController : Controller
    {
        [FromServices]
        public BookContext BookContext { get; set; }

        [FromServices]
        public ILogger<BookController> Logger { get; set; }

        public IActionResult Index()
        {
            var books = BookContext.Books.Include(b => b.Author);
            return View(books);
        }
        public async Task<ActionResult> Details(int id)
        {
            Book book = await BookContext.Books
                .Include(b => b.Author)
                .SingleOrDefaultAsync(b => b.BookID == id);
            if (book == null)
            {
                Logger.LogInformation("Details: Item not found {0}", id);
                return HttpNotFound();
            }
            return View(book);
        }
        public ActionResult Create()
        {
            ViewBag.Items = GetAuthorsListItems();
            return View();
        }
        public IEnumerable<SelectListItem> GetAuthorsListItems(int selected = 0)
        {
            var tmp = BookContext.Authors.ToList();

            return tmp.OrderBy(author => author.LastName)
                .Select(author => new SelectListItem
                {
                    Text = String.Format("{0}, {1}", author.LastName, author.FirstMidName),
                    Value = author.AuthorID.ToString(),
                    Selected = author.AuthorID == selected
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Title", "Year", "Price", "Genre", "AuthorID")] Book book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BookContext.Books.Add(book);
                    await BookContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.");
            }
            return View(book);
        }
        public async Task<ActionResult> Edit(int id)
        {
            Book book = await FindBookAsync(id);
            if (book == null)
            {
                Logger.LogInformation("Edit: Item not found {0}", id);
                return HttpNotFound();
            }

            ViewBag.Items = GetAuthorsListItems(book.AuthorID);
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(int id, [Bind("Title", "Year", "Price", "Genre", "AuthorID")] Book book)
        {
            try
            {
                book.BookID = id;
                BookContext.Books.Attach(book);
                BookContext.Entry(book).State = EntityState.Modified;
                await BookContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.");
            }
            return View(book);
        }

        private Task<Book> FindBookAsync(int id)
        {
            return BookContext.Books.SingleOrDefaultAsync(book => book.BookID == id);
        }
        [HttpGet]
        [ActionName("Delete")]
        public async Task<ActionResult> ConfirmDelete(int id, bool? retry)
        {
            Book book = await FindBookAsync(id);
            if (book == null)
            {
                Logger.LogInformation("Delete: Item not found {0}", id);
                return HttpNotFound();
            }
            ViewBag.Retry = retry ?? false;
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Book book = await FindBookAsync(id);
                BookContext.Books.Remove(book);
                await BookContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return RedirectToAction("Delete", new { id = id, retry = true });
            }
            return RedirectToAction("Index");
        }
    }
}
