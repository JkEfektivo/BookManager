using BookManager.Data;
using BookManager.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> CreateBookAsync(Book book);
        Task<Book?> UpdateBookAsync(int id, Book book);
        Task<bool> DeleteBookAsync(int id);
    }

    public class BookService : IBookService
    {
        private readonly BookManagerContext _context;

        public BookService(BookManagerContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .OrderByDescending(b => b.DateAdded)
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            if (book.YearPublished < 1000 || book.YearPublished > DateTime.Now.Year)
            {
                throw new ArgumentException($"Rok wydania musi być między 1000 a {DateTime.Now.Year}");
            }

            book.DateAdded = DateTime.UtcNow;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book?> UpdateBookAsync(int id, Book book)
        {
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
            {
                return null;
            }

            if (book.YearPublished < 1000 || book.YearPublished > DateTime.Now.Year)
            {
                throw new ArgumentException($"Rok wydania musi być między 1000 a {DateTime.Now.Year}");
            }

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.YearPublished = book.YearPublished;

            await _context.SaveChangesAsync();
            return existingBook;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return false;
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
