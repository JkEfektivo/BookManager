using System.Net;
using System.Net.Http.Json;
using BookManager.Models;

namespace BookManager.Tests;

public class BooksApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Test integracyjny weryfikujący pełny scenariusz użytkownika:
    /// 1. Użytkownik dodaje nową książkę
    /// 2. Użytkownik widzi książkę na liście
    /// 3. Użytkownik edytuje książkę
    /// 4. Użytkownik weryfikuje zmiany
    /// 5. Użytkownik usuwa książkę
    /// 6. Lista jest pusta
    /// </summary>
    [Fact]
    public async Task UserCanPerformFullCrudOperations()
    {
        // === ARRANGE ===
        var newBook = new
        {
            Title = "Wiedźmin",
            Author = "Andrzej Sapkowski",
            YearPublished = 1993
        };

        // === ACT & ASSERT ===

        // 1. CREATE - Użytkownik dodaje nową książkę
        var createResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdBook = await createResponse.Content.ReadFromJsonAsync<Book>();
        Assert.NotNull(createdBook);
        Assert.True(createdBook.Id > 0);
        Assert.Equal("Wiedźmin", createdBook.Title);
        Assert.Equal("Andrzej Sapkowski", createdBook.Author);
        Assert.Equal(1993, createdBook.YearPublished);

        // 2. READ - Użytkownik widzi książkę na liście
        var getAllResponse = await _client.GetAsync("/api/books");
        Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);

        var books = await getAllResponse.Content.ReadFromJsonAsync<List<Book>>();
        Assert.NotNull(books);
        Assert.Single(books);
        Assert.Equal("Wiedźmin", books[0].Title);

        // 3. READ BY ID - Użytkownik pobiera szczegóły książki
        var getByIdResponse = await _client.GetAsync($"/api/books/{createdBook.Id}");
        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

        var bookDetails = await getByIdResponse.Content.ReadFromJsonAsync<Book>();
        Assert.NotNull(bookDetails);
        Assert.Equal(createdBook.Id, bookDetails.Id);

        // 4. UPDATE - Użytkownik edytuje książkę
        var updatedBook = new
        {
            Title = "Wiedźmin: Ostatnie życzenie",
            Author = "Andrzej Sapkowski",
            YearPublished = 1993
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/books/{createdBook.Id}", updatedBook);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var bookAfterUpdate = await updateResponse.Content.ReadFromJsonAsync<Book>();
        Assert.NotNull(bookAfterUpdate);
        Assert.Equal("Wiedźmin: Ostatnie życzenie", bookAfterUpdate.Title);

        // 5. DELETE - Użytkownik usuwa książkę
        var deleteResponse = await _client.DeleteAsync($"/api/books/{createdBook.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // 6. VERIFY - Lista jest pusta po usunięciu
        var finalGetResponse = await _client.GetAsync("/api/books");
        var finalBooks = await finalGetResponse.Content.ReadFromJsonAsync<List<Book>>();
        Assert.NotNull(finalBooks);
        Assert.Empty(finalBooks);
    }

    [Fact]
    public async Task CreateBook_WithInvalidYear_ReturnsBadRequest()
    {
        // Użytkownik próbuje dodać książkę z przyszłym rokiem
        var invalidBook = new
        {
            Title = "Książka z przyszłości",
            Author = "Autor",
            YearPublished = 2099
        };

        var response = await _client.PostAsJsonAsync("/api/books", invalidBook);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBook_WithNonExistentId_ReturnsNotFound()
    {
        // Użytkownik próbuje pobrać nieistniejącą książkę
        var response = await _client.GetAsync("/api/books/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_WithNonExistentId_ReturnsNotFound()
    {
        // Użytkownik próbuje usunąć nieistniejącą książkę
        var response = await _client.DeleteAsync("/api/books/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
