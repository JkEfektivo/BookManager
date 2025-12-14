# Books API Documentation

## Informacje ogólne

| Parametr | Wartość |
|----------|---------|
| Base URL | `/api/books` |
| Content-Type | `application/json` |
| Kodowanie | UTF-8 |

---

## Endpointy

### 1. Pobierz wszystkie książki

```
GET /api/books
```

**Opis:** Zwraca listę wszystkich książek posortowaną od najnowszych.

**Response 200 OK:**
```json
[
  {
    "id": 1,
    "title": "Wiedźmin",
    "author": "Andrzej Sapkowski",
    "yearPublished": 1993,
    "dateAdded": "2025-12-14T10:30:00Z"
  },
  {
    "id": 2,
    "title": "Solaris",
    "author": "Stanisław Lem",
    "yearPublished": 1961,
    "dateAdded": "2025-12-14T09:15:00Z"
  }
]
```

**Response 500 Internal Server Error:**
```json
{
  "message": "Błąd serwera",
  "error": "Szczegóły błędu"
}
```

---

### 2. Pobierz książkę po ID

```
GET /api/books/{id}
```

**Parametry URL:**
| Parametr | Typ | Wymagany | Opis |
|----------|-----|----------|------|
| id | int | Tak | ID książki |

**Response 200 OK:**
```json
{
  "id": 1,
  "title": "Wiedźmin",
  "author": "Andrzej Sapkowski",
  "yearPublished": 1993,
  "dateAdded": "2025-12-14T10:30:00Z"
}
```

**Response 404 Not Found:**
```json
{
  "message": "Książka nie znaleziona"
}
```

---

### 3. Utwórz nową książkę

```
POST /api/books
```

**Request Body:**
```json
{
  "title": "Wiedźmin",
  "author": "Andrzej Sapkowski",
  "yearPublished": 1993
}
```

**Walidacja:**
| Pole | Wymagane | Walidacja |
|------|----------|-----------|
| title | Tak | 1-255 znaków |
| author | Tak | 1-255 znaków |
| yearPublished | Tak | 1000 - rok bieżący |

**Response 201 Created:**
```json
{
  "id": 1,
  "title": "Wiedźmin",
  "author": "Andrzej Sapkowski",
  "yearPublished": 1993,
  "dateAdded": "2025-12-14T10:30:00Z"
}
```

**Headers:**
```
Location: /api/books/1
```

**Response 400 Bad Request (błąd walidacji):**
```json
{
  "message": "Rok wydania musi być między 1000 a 2025"
}
```

---

### 4. Aktualizuj książkę

```
PUT /api/books/{id}
```

**Parametry URL:**
| Parametr | Typ | Wymagany | Opis |
|----------|-----|----------|------|
| id | int | Tak | ID książki |

**Request Body:**
```json
{
  "title": "Wiedźmin: Ostatnie życzenie",
  "author": "Andrzej Sapkowski",
  "yearPublished": 1993
}
```

**Response 200 OK:**
```json
{
  "id": 1,
  "title": "Wiedźmin: Ostatnie życzenie",
  "author": "Andrzej Sapkowski",
  "yearPublished": 1993,
  "dateAdded": "2025-12-14T10:30:00Z"
}
```

**Response 404 Not Found:**
```json
{
  "message": "Książka nie znaleziona"
}
```

**Response 400 Bad Request:**
```json
{
  "message": "Rok wydania musi być między 1000 a 2025"
}
```

---

### 5. Usuń książkę

```
DELETE /api/books/{id}
```

**Parametry URL:**
| Parametr | Typ | Wymagany | Opis |
|----------|-----|----------|------|
| id | int | Tak | ID książki |

**Response 204 No Content:**
Brak treści (książka usunięta pomyślnie)

**Response 404 Not Found:**
```json
{
  "message": "Książka nie znaleziona"
}
```

---

## Modele danych

### Book (Entity)

```csharp
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int YearPublished { get; set; }
    public DateTime DateAdded { get; set; }
}
```

### CreateBookRequest (DTO)

```csharp
public class CreateBookRequest
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int YearPublished { get; set; }
}
```

### UpdateBookRequest (DTO)

```csharp
public class UpdateBookRequest
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int YearPublished { get; set; }
}
```

---

## Kody odpowiedzi HTTP

| Kod | Status | Opis |
|-----|--------|------|
| 200 | OK | Żądanie zakończone sukcesem |
| 201 | Created | Zasób utworzony pomyślnie |
| 204 | No Content | Zasób usunięty pomyślnie |
| 400 | Bad Request | Błąd walidacji danych wejściowych |
| 404 | Not Found | Zasób nie istnieje |
| 500 | Internal Server Error | Błąd serwera |

---

## Przykłady użycia (cURL)

### Pobierz wszystkie książki
```bash
curl -X GET http://localhost:5000/api/books
```

### Pobierz książkę po ID
```bash
curl -X GET http://localhost:5000/api/books/1
```

### Dodaj nową książkę
```bash
curl -X POST http://localhost:5000/api/books \
  -H "Content-Type: application/json" \
  -d '{"title":"Wiedźmin","author":"Andrzej Sapkowski","yearPublished":1993}'
```

### Aktualizuj książkę
```bash
curl -X PUT http://localhost:5000/api/books/1 \
  -H "Content-Type: application/json" \
  -d '{"title":"Wiedźmin: Ostatnie życzenie","author":"Andrzej Sapkowski","yearPublished":1993}'
```

### Usuń książkę
```bash
curl -X DELETE http://localhost:5000/api/books/1
```
