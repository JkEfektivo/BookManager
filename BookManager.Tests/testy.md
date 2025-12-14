# Testy BookManager

## Przegląd

Projekt zawiera testy integracyjne API, które weryfikują działanie aplikacji z perspektywy użytkownika. Testy używają prawdziwego serwera HTTP (TestServer) z bazą SQLite in-memory.

## Struktura testów

```
BookManager.Tests/
├── BooksApiTests.cs              # Testy integracyjne API
├── CustomWebApplicationFactory.cs # Konfiguracja TestServer
├── BookManager.Tests.csproj      # Projekt testowy
└── testy.md                      # Ta dokumentacja
```

## Uruchomienie testów

```bash
# Z katalogu głównego solucji
cd C:\PROGRAMOWANIE\BookManager
dotnet test

# Lub z większą szczegółowością
dotnet test --verbosity normal

# Tylko projekt testowy
dotnet test BookManager.Tests
```

## Lista testów

### 1. UserCanPerformFullCrudOperations

**Pełny scenariusz użytkownika** - testuje cały cykl życia książki:

```
1. CREATE  → Użytkownik dodaje "Wiedźmin" Andrzeja Sapkowskiego
2. READ    → Użytkownik widzi książkę na liście
3. GET     → Użytkownik pobiera szczegóły książki
4. UPDATE  → Użytkownik zmienia tytuł na "Wiedźmin: Ostatnie życzenie"
5. DELETE  → Użytkownik usuwa książkę
6. VERIFY  → Lista jest pusta
```

### 2. CreateBook_WithInvalidYear_ReturnsBadRequest

Weryfikuje walidację roku wydania - próba dodania książki z roku 2099 powinna zwrócić błąd 400.

### 3. GetBook_WithNonExistentId_ReturnsNotFound

Weryfikuje obsługę błędów - próba pobrania nieistniejącej książki (ID: 99999) powinna zwrócić 404.

### 4. DeleteBook_WithNonExistentId_ReturnsNotFound

Weryfikuje obsługę błędów - próba usunięcia nieistniejącej książki powinna zwrócić 404.

## Architektura testów

### CustomWebApplicationFactory

Klasa konfigurująca TestServer z bazą SQLite in-memory:

```csharp
// Kluczowe elementy:
- Używa SQLite in-memory zamiast produkcyjnej bazy
- Połączenie pozostaje otwarte przez czas życia testu
- Każdy test dostaje czystą bazę danych
```

### Dlaczego SQLite in-memory?

| Opcja | Problem |
|-------|---------|
| EF InMemory Provider | Nie wspiera wszystkich funkcji SQL |
| SQLite plik | Wymaga czyszczenia między testami |
| **SQLite in-memory** | Szybkie, izolowane, pełna kompatybilność |

## Pakiety NuGet

```xml
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
<PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.0" />
<PackageReference Include="xunit" Version="2.9.2" />
```

## Typowe problemy

### Błąd: PipeWriter.UnflushedBytes

**Przyczyna:** Niezgodność wersji .NET między projektem głównym a testowym.

**Rozwiązanie:** Upewnij się, że oba projekty używają tej samej wersji .NET:
```xml
<TargetFramework>net8.0</TargetFramework>
```

### Błąd: Plik bazy zablokowany

**Przyczyna:** Aplikacja BookManager działa w tle.

**Rozwiązanie:** Zatrzymaj aplikację przed uruchomieniem testów.

## Przykładowe dane testowe

```csharp
var newBook = new
{
    Title = "Wiedźmin",
    Author = "Andrzej Sapkowski",
    YearPublished = 1993
};
```

*Bo jak testować polską aplikację do zarządzania książkami, to tylko z polską klasyką fantasy!*
