using BookManager.Models;
using BookManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// GET /api/books - Pobierz wszystkie książki
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", error = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/books/{id} - Pobierz książkę po ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound(new { message = "Książka nie znaleziona" });
                }
                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", error = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/books - Utwórz nową książkę
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromBody] CreateBookRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var book = new Book
                {
                    Title = request.Title,
                    Author = request.Author,
                    YearPublished = request.YearPublished
                };

                var createdBook = await _bookService.CreateBookAsync(book);
                return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/books/{id} - Aktualizuj książkę
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var book = new Book
                {
                    Title = request.Title,
                    Author = request.Author,
                    YearPublished = request.YearPublished
                };

                var updatedBook = await _bookService.UpdateBookAsync(id, book);
                if (updatedBook == null)
                {
                    return NotFound(new { message = "Książka nie znaleziona" });
                }

                return Ok(updatedBook);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/books/{id} - Usuń książkę
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var success = await _bookService.DeleteBookAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Książka nie znaleziona" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Błąd serwera", error = ex.Message });
            }
        }
    }

    /// <summary>
    /// DTO dla tworzenia książki
    /// </summary>
    public class CreateBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int YearPublished { get; set; }
    }

    /// <summary>
    /// DTO dla aktualizacji książki
    /// </summary>
    public class UpdateBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int YearPublished { get; set; }
    }
}
