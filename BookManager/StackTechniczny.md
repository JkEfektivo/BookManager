# Stack Techniczny - BookManager

**Wersja:** 1.0
**Data:** 2025-11-02
**Projekt:** BookManager - Webowa aplikacja do zarządzania biblioteką

---

## Spis treści
1. [Overview](#overview)
2. [Backend](#backend)
3. [Frontend](#frontend)
4. [Baza danych](#baza-danych)
5. [Architektura systemu](#architektura-systemu)
6. [Wymagania systemowe](#wymagania-systemowe)
7. [Setup i konfiguracja](#setup-i-konfiguracja)
8. [Deployment](#deployment)

---

## Overview

BookManager wykorzystuje **nowoczesny, sprawdzony stack** oparty na technologiach Microsoft ekosystemu. Kombinacja ASP.NET Core 8 i Blazor Server zapewnia szybki development, wysoką wydajność i łatwe utrzymanie aplikacji.

### Architektura na wysokim poziomie
```
┌─────────────────────────────────────────────┐
│         Blazor Server (Frontend)             │
│  - Bootstrap 5 UI Components                │
│  - Real-time Communication                  │
└──────────────────┬──────────────────────────┘
                   │ HTTP/SignalR
┌──────────────────▼──────────────────────────┐
│     ASP.NET Core 8 (Backend)                │
│  - API Controllers                          │
│  - Business Logic (Services)                │
│  - Validation & Error Handling              │
└──────────────────┬──────────────────────────┘
                   │ EF Core 8
┌──────────────────▼──────────────────────────┐
│      SQLite Database (app.db)               │
│  - Books Table                              │
│  - Migrations                               │
└─────────────────────────────────────────────┘
```

---

## Backend

### ASP.NET Core 8

**Wersja:** 8.0 LTS (Long-Term Support)
**Język:** C# 12
**Runtime:** .NET 8

#### Charakterystyka
- Najnowsza wersja LTS z długim wsparciem (do 2026)
- Wysoką wydajność (benchmarks pokazują 2-3x szybciej niż .NET Framework)
- Pełne wsparcie dla nowoczesnych wzorców (Async/Await, Dependency Injection)
- Cross-platform (Windows, Linux, macOS)

#### Instalacja

**Wymagania:**
- Visual Studio 2022 v17.8+ lub Visual Studio Code
- .NET 8 SDK (https://dotnet.microsoft.com/download/dotnet/8.0)

**Weryfikacja instalacji:**
```bash
dotnet --version  # Powinno zwrócić 8.x.x
```

#### Główne komponenty

##### Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddDbContext<BookManagerContext>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

##### Dependency Injection
- Wbudowany DI container (no external packages)
- Rejestracja serwisów w `Program.cs`
- Scoped lifetime dla DbContext

##### Controllers
- REST API controllers (`/api/books`)
- Attribute-based routing
- Wsparcie dla JSON input/output
- Automatic model binding & validation

---

### Entity Framework Core 8

**Wersja:** 8.0
**ORM:** Object-Relational Mapping

#### Charakterystyka
- Automatyczne migracje bazy danych
- LINQ queries (strongly-typed, compile-time checking)
- Change tracking
- Lazy loading / Eager loading
- Support dla multiple databases (SQLite, SQL Server, PostgreSQL, etc.)

#### DbContext

```csharp
public class BookManagerContext : DbContext
{
    public DbSet<Book> Books { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }
}
```

#### Migracje

Tworzenie migracji:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Pliki migracji przechowywane w: `Data/Migrations/`

#### Modele (Entities)

**Book.cs:**
```csharp
public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; }

    [Required]
    [StringLength(255)]
    public string Author { get; set; }

    [Range(1000, 9999)]
    public int YearPublished { get; set; }

    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}
```

---

### Walidacja

#### DataAnnotations
```csharp
[Required(ErrorMessage = "Tytuł jest wymagany")]
[StringLength(255, MinimumLength = 1)]
public string Title { get; set; }
```

#### FluentValidation (opcjonalnie)
```csharp
public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.YearPublished).Must(BeValidYear);
    }
}
```

---

### Konfiguracja

**appsettings.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db;"
  },
  "AllowedHosts": "*"
}
```

**appsettings.Development.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug"
    }
  }
}
```

---

## Frontend

### Blazor Server

**Wersja:** .NET 8 Razor Components
**Renderowanie:** Server-side

#### Charakterystyka
- **Real-time communication** (SignalR WebSocket)
- **C# w przeglądarce** (brak konieczności JavaScript dla logiki)
- **Komponenty** (reusable UI pieces)
- **Data binding** (two-way binding support)
- **Event handling**
- **Form validation** (client-side + server-side)

#### Komponenty

```
Components/
├── Pages/
│   ├── Login.razor
│   ├── Dashboard.razor
│   ├── AddBook.razor
│   ├── EditBook.razor
│   └── Error.razor
├── Shared/
│   ├── MainLayout.razor
│   ├── NavMenu.razor
│   └── BookTable.razor
└── _Imports.razor
```

#### Przykład komponenty - Dashboard.razor

```razor
@page "/dashboard"
@using BookManager.Models
@inject IBookService BookService
@inject NavigationManager Navigation

<div class="container mt-4">
    <h1>Moja Biblioteka</h1>

    <button class="btn btn-primary mb-3" @onclick="() => Navigation.NavigateTo('/addbook')">
        Dodaj nową
    </button>

    @if (books == null)
    {
        <p>Ładowanie...</p>
    }
    else if (books.Count == 0)
    {
        <p>Brak książek. Dodaj nową!</p>
    }
    else
    {
        <table class="table">
            <thead>
                <tr>
                    <th>Tytuł</th>
                    <th>Autor</th>
                    <th>Rok</th>
                    <th>Data dodania</th>
                    <th>Akcje</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var book in books)
                {
                    <tr>
                        <td>@book.Title</td>
                        <td>@book.Author</td>
                        <td>@book.YearPublished</td>
                        <td>@book.DateAdded.ToString("dd.MM.yyyy HH:mm")</td>
                        <td>
                            <button @onclick="() => Edit(book.Id)" class="btn btn-sm btn-warning">Edytuj</button>
                            <button @onclick="() => Delete(book.Id)" class="btn btn-sm btn-danger">Usuń</button>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

@code {
    private List<Book> books;

    protected override async Task OnInitializedAsync()
    {
        books = await BookService.GetAllBooksAsync();
    }

    private void Edit(int id)
    {
        Navigation.NavigateTo($"/editbook/{id}");
    }

    private async Task Delete(int id)
    {
        if (await JS.InvokeAsync<bool>("confirm", "Czy na pewno chcesz usunąć tę książkę?"))
        {
            await BookService.DeleteBookAsync(id);
            books = await BookService.GetAllBooksAsync();
        }
    }
}
```

#### Formularze - EditForm

```razor
<EditForm Model="@book" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="form-group">
        <label for="title">Tytuł</label>
        <InputText id="title" @bind-Value="book.Title" class="form-control" />
        <ValidationMessage For="@(() => book.Title)" />
    </div>

    <div class="form-group">
        <label for="author">Autor</label>
        <InputText id="author" @bind-Value="book.Author" class="form-control" />
        <ValidationMessage For="@(() => book.Author)" />
    </div>

    <div class="form-group">
        <label for="year">Rok wydania</label>
        <InputNumber id="year" @bind-Value="book.YearPublished" class="form-control" />
        <ValidationMessage For="@(() => book.YearPublished)" />
    </div>

    <button type="submit" class="btn btn-primary">Zapisz</button>
    <button type="button" class="btn btn-secondary" @onclick="Cancel">Anuluj</button>
</EditForm>
```

#### HTTP Communication

```csharp
@inject HttpClient Http

private async Task<List<Book>> GetBooksAsync()
{
    return await Http.GetFromJsonAsync<List<Book>>("/api/books");
}

private async Task<bool> CreateBookAsync(Book book)
{
    var response = await Http.PostAsJsonAsync("/api/books", book);
    return response.IsSuccessStatusCode;
}
```

---

### Bootstrap 5

**Wersja:** 5.3.x
**Instalacja:** Wbudowana w projekcie

#### Utilites
- Spacing (margin, padding)
- Colors (text, background)
- Typography
- Responsive grid (12 kolumn)
- Flex utilities
- Borders, shadows, etc.

#### Komponenty
- Buttons (primary, secondary, danger, warning)
- Forms (input, textarea, select)
- Tables
- Modals
- Cards
- Navigation (navbar, breadcrumb)
- Alerts & toasts

#### Responsive Breakpoints
```css
xs: < 576px    (mobile)
sm: ≥ 576px    (small devices)
md: ≥ 768px    (tablets)
lg: ≥ 992px    (desktops)
xl: ≥ 1200px   (large desktops)
xxl: ≥ 1400px  (extra large)
```

#### Przykład - Responsive Grid

```html
<div class="container">
    <div class="row">
        <div class="col-12 col-md-6 col-lg-4">Kolumna 1</div>
        <div class="col-12 col-md-6 col-lg-4">Kolumna 2</div>
        <div class="col-12 col-lg-4">Kolumna 3</div>
    </div>
</div>
```

---

## Baza danych

### SQLite 3

**Wersja:** Latest (wbudowana w .NET)
**Typ:** File-based embedded database

#### Charakterystyka
- Nie wymaga serwera (single file: `app.db`)
- Zero administracji
- ACID compliant
- Wysoka wydajność dla MVP
- Łatwe skalowanie do SQL Server/PostgreSQL

#### Connection String

```
Data Source=app.db;
Data Source=./app.db;                    # Ścieżka względna
Data Source=C:\data\app.db;              # Ścieżka bezwzględna
```

#### Schema - Tabela `Books`

```sql
CREATE TABLE Books (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title NVARCHAR(255) NOT NULL,
    Author NVARCHAR(255) NOT NULL,
    YearPublished INTEGER NOT NULL
        CHECK (YearPublished >= 1000 AND YearPublished <= CAST(strftime('%Y', 'now') AS INTEGER)),
    DateAdded DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT CK_YearRange CHECK (YearPublished >= 1000)
);

CREATE INDEX idx_books_dateadded ON Books(DateAdded DESC);
```

#### SQL Queries

```sql
-- SELECT
SELECT * FROM Books ORDER BY DateAdded DESC;

-- INSERT
INSERT INTO Books (Title, Author, YearPublished, DateAdded)
VALUES ('Clean Code', 'Robert Martin', 2008, CURRENT_TIMESTAMP);

-- UPDATE
UPDATE Books SET Title = 'Nowy tytuł' WHERE Id = 1;

-- DELETE
DELETE FROM Books WHERE Id = 1;
```

---

## Architektura systemu

### Folder Structure

```
BookManager/
├── Components/
│   ├── Pages/
│   │   ├── Login.razor
│   │   ├── Dashboard.razor
│   │   ├── AddBook.razor
│   │   ├── EditBook.razor
│   │   └── Error.razor
│   ├── Shared/
│   │   ├── MainLayout.razor
│   │   ├── NavMenu.razor
│   │   └── BookTable.razor
│   ├── App.razor
│   ├── Routes.razor
│   └── _Imports.razor
├── Controllers/
│   └── BooksController.cs
├── Models/
│   └── Book.cs
├── Data/
│   ├── BookManagerContext.cs
│   └── Migrations/
│       └── ...
├── Services/
│   └── BookService.cs
├── wwwroot/
│   ├── css/
│   │   └── app.css
│   ├── bootstrap/
│   │   └── bootstrap.min.css
│   └── favicon.png
├── Properties/
│   └── launchSettings.json
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── BookManager.csproj
├── PRD.md
└── StackTechniczny.md
```

### Layers

#### 1. Presentation Layer (Blazor Components)
- `Components/Pages/` - Strony (Login, Dashboard, AddBook, EditBook)
- `Components/Shared/` - Komponenty wielokrotnego użytku
- Odpowiedzialność: UI rendering, user interactions

#### 2. API Layer (Controllers)
- `Controllers/BooksController.cs` - REST API endpoints
- Odpowiedzialność: HTTP request handling, routing

#### 3. Business Logic Layer (Services)
- `Services/BookService.cs` - Business rules, validations
- Odpowiedzialność: Data operations, business logic

#### 4. Data Access Layer (EF Core)
- `Data/BookManagerContext.cs` - DbContext
- `Models/Book.cs` - Entity models
- Odpowiedzialność: Database operations via ORM

#### 5. Database Layer (SQLite)
- `app.db` - SQLite file
- Odpowiedzialność: Data persistence

---

## Wymagania systemowe

### Minimum (Development)
- **OS:** Windows 10+, macOS 10.15+, Linux (Ubuntu 18.04+)
- **RAM:** 4 GB
- **Storage:** 2 GB (dla .NET SDK + project)
- **Processor:** Dual-core 2 GHz+

### Recommended (Development)
- **OS:** Windows 11, macOS 12+, Ubuntu 22.04+
- **RAM:** 8+ GB
- **Storage:** 10 GB SSD
- **Processor:** Quad-core 3 GHz+

### Runtime (Production)
- **OS:** Windows Server 2019+, Linux (any recent distro)
- **RAM:** 512 MB - 1 GB
- **Storage:** 100 MB (binary) + app.db size
- **Processor:** Single-core 1 GHz+

### Oprogramowanie

**Wymagane:**
- .NET 8 SDK (https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 lub Visual Studio Code

**Opcjonalnie:**
- SQL Server Management Studio (dla inspektacji bazy - jeśli SQL Server)
- Git (version control)
- Docker (containerization)

---

## Setup i konfiguracja

### Instalacja .NET 8

#### Windows
```bash
# Via chocolatey
choco install dotnet-sdk-8.0

# Or download from: https://dotnet.microsoft.com/download/dotnet/8.0
```

#### macOS
```bash
brew install dotnet-sdk@8
```

#### Linux (Ubuntu)
```bash
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 8.0
```

### Klonowanie i setup projektu

```bash
# Clone repository
git clone https://github.com/YourOrg/BookManager.git
cd BookManager/BookManager

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Apply migrations
dotnet ef database update

# Run application
dotnet run
```

### Uruchamianie aplikacji

**Development:**
```bash
dotnet run
# App będzie dostępna na: https://localhost:5001
```

**Production build:**
```bash
dotnet publish -c Release -o ./publish
# Output w `./publish` folder
```

### IDE Setup

#### Visual Studio 2022
1. Open `BookManager.sln`
2. Build solution (Ctrl+Shift+B)
3. Run (F5) lub Start without debugging (Ctrl+F5)

#### Visual Studio Code
1. Otwórz folder projektu
2. Install C# extension (OmniSharp)
3. Restore packages: `dotnet restore`
4. Run: `dotnet run` w terminal

---

## Deployment

### Local Development
```bash
dotnet run
```
Aplikacja dostępna: `https://localhost:5001`

### Azure App Service
```bash
# Publish to Azure
dotnet publish -c Release

# Deploy using Azure CLI
az webapp up --name bookmanager-app --resource-group MyResourceGroup
```

### Docker (opcjonalnie)
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BookManager.csproj", "."]
RUN dotnet restore "BookManager.csproj"
COPY . .
RUN dotnet build "BookManager.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
EXPOSE 5000
ENTRYPOINT ["dotnet", "BookManager.dll"]
```

```bash
# Build image
docker build -t bookmanager:latest .

# Run container
docker run -p 8080:5000 bookmanager:latest
```

### IIS (Windows)
1. Publish project: `dotnet publish -c Release`
2. Copy files do `wwwroot` folderu IIS
3. Create application pool (.NET CLR version: No Managed Code)
4. Create website w IIS pointing do published files

---

## Wersjonowanie

| Komponent | Wersja | Link |
|-----------|--------|------|
| .NET | 8.0 LTS | https://dotnet.microsoft.com/ |
| C# | 12 | https://docs.microsoft.com/en-us/dotnet/csharp/ |
| ASP.NET Core | 8.0 | https://docs.microsoft.com/en-us/aspnet/core/ |
| Blazor | .NET 8 | https://docs.microsoft.com/en-us/aspnet/core/blazor/ |
| Entity Framework Core | 8.0 | https://docs.microsoft.com/en-us/ef/core/ |
| Bootstrap | 5.3.x | https://getbootstrap.com/ |
| SQLite | 3.x | https://www.sqlite.org/ |

---

## Zasoby i dokumentacja

### Oficjalna dokumentacja
- [Microsoft .NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

### Community Resources
- [Stack Overflow](https://stackoverflow.com/questions/tagged/blazor)
- [GitHub Discussions](https://github.com/dotnet/aspnetcore/discussions)
- [Microsoft Learn](https://learn.microsoft.com/)

### Tools
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- [Visual Studio Code](https://code.visualstudio.com/)
- [SQL Browser](https://sqlitebrowser.org/) - SQLite inspection
- [Postman](https://www.postman.com/) - API testing

---

## Checklist Setup

- [ ] .NET 8 SDK zainstalowany
- [ ] IDE (Visual Studio / VS Code) zainstalowany
- [ ] Projekt sklonowany
- [ ] `dotnet restore` uruchomiony
- [ ] `dotnet build` powodzenie
- [ ] `dotnet ef database update` - migracje zaaplikowane
- [ ] `dotnet run` - aplikacja uruchomiona
- [ ] Login dostępny na `https://localhost:5001`
- [ ] Baza danych `app.db` utworzona

---

**Wersja dokumentu:** 1.0
**Ostatnia aktualizacja:** 2025-11-02
**Autor:** Zespół BookManager
