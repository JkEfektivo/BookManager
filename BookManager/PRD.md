# Product Requirements Document (PRD)
## BookManager - Webowa aplikacja do zarządzania biblioteką

**Wersja:** 1.0
**Data:** 2025-11-02
**Status:** MVP - In Development
**Typ dokumentu:** Product Requirements Document

---

## 1. Przegląd produktu

### 1.1 Nazwa i cel
**BookManager** to webowa aplikacja CRUD (Create, Read, Update, Delete) przeznaczona do zarządzania osobistą kolekcją książek. Aplikacja umożliwia użytkownikom szybkie i intuicyjne dodawanie, edytowanie, usuwanie oraz przeglądanie informacji o posiadanych książkach.

### 1.2 Stack techniczny
- **Backend:** ASP.NET Core 8
- **Frontend:** Blazor Server
- **Baza danych:** SQLite
- **ORM:** Entity Framework Core
- **Stylizacja:** Bootstrap 5
- **Język:** C#

### 1.3 Grupa docelowa
- Czytelnicy pragnący zorganizować swoją bibliotekę
- Osoby chcące śledzić swoją kolekcję książek

---

## 2. Cel biznesowy

BookManager ma być **prostym, niskoprożogowym rozwiązaniem** do zarządzania osobistą biblioteką. Celem jest:

1. Umożliwienie użytkownikom **szybkiego rejestrowania posiadanych książek**
2. Przechowywanie **podstawowych metadanych** o każdej książce
3. Dostarczenie **intuicyjnego interfejsu** do codziennego użytku
4. Stanowienie **fundacji dla przyszłych rozszerzeń** (kategorie, recenzje, itd.)

### Hipoteza biznesowa
Prosty system zarządzania biblioteką bez skomplikowanych funkcji może być wartościowy dla użytkowników pragną organizacji swoich zbiorów.

---

## 3. Funkcjonalności

### 3.1 Uwierzytelnienie i autoryzacja

#### 3.1.1 Ekran logowania
- **Komponenta:** Login Page (`Pages/Login.razor`)
- **Pola:**
  - Username (text input)
  - Hasło (password input)
- **Przycisk:** "Zaloguj" (submit)
- **Layout:** Centrowany, responsywny formularz

#### 3.1.2 Mock-up uwierzytelnienia (MVP)
- **Dane twardkodowane:**
  - Username: `admin`
  - Hasło: `admin`
- **Typ:** Basic authentication (do pełnego uwierzytelnienia na produkcji)
- **Sesja:** Sesja użytkownika utrzymywana po stronie serwera

#### 3.1.3 Sesja użytkownika
- Zalogowany użytkownik widzi menu nawigacyjne
- Przycisk "Wyloguj" w nagłówku aplikacji
- Odświeżenie strony zachowuje sesję
- Wylogowanie czyści sesję i redirectuje na login

#### 3.1.4 Wymagania bezpieczeństwa
- Hasła nie są wyświetlane w logu
- Sesje wygasają po nieaktywności (time-based)
- HTTPS obowiązkowy w produkcji
- CSRF protection dla formularzy

---

### 3.2 Zarządzanie książkami (CRUD)

#### 3.2.1 CREATE - Dodawanie nowej książki

**Ekran:** Strona formularza dodawania (`Pages/AddBook.razor`)

**Formularz zawiera:**
| Pole | Typ | Obowiązkowe | Walidacja |
|------|-----|------------|-----------|
| Tytuł | string | TAK | Min. 1, Max. 255 znaków |
| Autor | string | TAK | Min. 1, Max. 255 znaków |
| Rok wydania | int | TAK | 1000 ≤ rok ≤ rok_bieżący |

**Walidacja:**
- Client-side: HTML5 validation + JavaScript
- Server-side: ASP.NET Core ModelState
- Wszystkie pola są obowiązkowe
- Rok wydania musi być w przedziale [1000, current_year]
- Komunikaty błędów wyświetlane pod polami

**Akcje:**
- **Przycisk "Dodaj":** Wysyła dane do API, zapisuje w bazie, redirectuje do Dashboard
- **Przycisk "Anuluj":** Wraca do Dashboard bez zapisywania

**Timestamp:**
- `DateAdded` przypisywany automatycznie serverem (UTC)
- Format: ISO 8601

**API Endpoint:**
```
POST /api/books
Content-Type: application/json

{
  "title": "string",
  "author": "string",
  "yearPublished": 2024
}

Response: 201 Created
{
  "id": 1,
  "title": "string",
  "author": "string",
  "yearPublished": 2024,
  "dateAdded": "2025-11-02T12:30:45Z"
}
```

---

#### 3.2.2 READ - Wyświetlanie listy książek

**Ekran:** Dashboard (`Pages/Dashboard.razor`)

**Komponenta: Tabela Książek**
- Wyświetlana po zalogowaniu
- Ładowana z bazy danych

**Kolumny tabeli:**
| Kolumna | Typ | Opis |
|---------|-----|------|
| Tytuł | string | Nazwa książki |
| Autor | string | Imię i nazwisko autora |
| Rok wydania | int | Rok publikacji |
| Data dodania | datetime | Kiedy dodana do systemu (format: DD.MM.YYYY HH:MM) |
| Akcje | buttons | Edytuj / Usuń |

**Funkcjonalności:**
- Lista ładowana automatycznie po zalogowaniu
- Sortowanie: domyślnie chronologicznie (najnowsze na górze)
- Paginacja: nie wymagana dla MVP (wszystkie rekordy na stronie)
- Stan pusty: komunikat "Brak książek. Dodaj nową!" z przyciskiem
- Ładowanie: loader/spinner podczas pobierania danych

**API Endpoint:**
```
GET /api/books

Response: 200 OK
[
  {
    "id": 1,
    "title": "string",
    "author": "string",
    "yearPublished": 2024,
    "dateAdded": "2025-11-02T12:30:45Z"
  }
]
```

**Przycisk "Dodaj nową":**
- Lokacja: Górny lewy róg tabeli lub strona Dashboard
- Nawigacja do `/addbook`

---

#### 3.2.3 UPDATE - Edytowanie książki

**Ekran:** Strona edycji (`Pages/EditBook.razor?id={bookId}`)

**Workflow:**
1. Użytkownik klika "Edytuj" na liście
2. System ładuje dane książki (GET /api/books/{id})
3. Formularz wypełniony istniejącymi danymi (prefill)
4. Użytkownik modyfikuje pola

**Formularz:** Identyczny jak CREATE, z prefillanymi danymi

**Walidacja:** Identyczna jak CREATE

**Akcje:**
- **Przycisk "Zapisz":** Wysyła PUT request, aktualizuje bazę, redirectuje do Dashboard
- **Przycisk "Anuluj":** Wraca do Dashboard bez zmian

**Komunikaty:**
- Sukces: Toast notification "Książka zaktualizowana"
- Błąd: Wyświetlenie komunikatu błędu walidacji

**API Endpoint:**
```
PUT /api/books/{id}
Content-Type: application/json

{
  "title": "string",
  "author": "string",
  "yearPublished": 2024
}

Response: 200 OK / 404 Not Found / 400 Bad Request
```

---

#### 3.2.4 DELETE - Usuwanie książki

**Trigger:** Klikniecie przycisku "Usuń" na liście

**Potwierdzenie:**
- Modal dialog / alert: "Czy na pewno chcesz usunąć '{tytuł}'?"
- Przyciski: "Tak, usuń" / "Anuluj"

**Akcja usunięcia:**
- DELETE request do API
- Po sukcesie: odświeżenie listy, toast notification "Książka usunięta"
- Po błędzie: komunikat błędu

**API Endpoint:**
```
DELETE /api/books/{id}

Response: 204 No Content / 404 Not Found
```

**Uwagi:**
- Usunięcie nieodwracalne
- Nie przywracamy z kosza (MVP)

---

### 3.3 Logika biznesowa

#### 3.3.1 Walidacja roku wydania
```
if (yearPublished < 1000 || yearPublished > DateTime.Now.Year) {
  error: "Rok wydania musi być między 1000 a {current_year}"
}
```

**Reguły:**
- Minimalny rok: 1000 (książki starsze uważamy za anomalie)
- Maksymalny rok: rok bieżący
- Nie można dodawać książek z przyszłości

#### 3.3.2 Timestamp dodania
- Przypisywany serverem (nie klientem)
- Format: UTC ISO 8601
- Nie edytowalny przez użytkownika
- Wyświetlany w lokalnej strefie czasowej użytkownika

#### 3.3.3 Unikalność danych
- Brak unikatnościowości na tytuł/autora (można mieć kilka kopii)
- Każdy rekord ma unikatowy `Id` (PK)

---

## 4. Wymagania techniczne

### 4.1 Backend - ASP.NET Core 8

#### 4.1.1 Struktura projektów
```
BookManager/
├── Controllers/
│   └── BooksController.cs
├── Models/
│   └── Book.cs (entity)
├── Data/
│   ├── BookManagerContext.cs (DbContext)
│   └── Migrations/
├── Services/
│   └── BookService.cs (business logic)
├── Program.cs
└── appsettings.json
```

#### 4.1.2 API Endpoints

| Metoda | Endpoint | Opis |
|--------|----------|------|
| GET | /api/books | Pobierz wszystkie książki |
| GET | /api/books/{id} | Pobierz książkę po ID |
| POST | /api/books | Utwórz nową książkę |
| PUT | /api/books/{id} | Zaktualizuj książkę |
| DELETE | /api/books/{id} | Usuń książkę |

#### 4.1.3 Entity Framework Core
- **DbContext:** `BookManagerContext`
- **DbSet:** `Books`
- **Migracje:** Automatyczne przy starcie (InitialCreate)
- **Seed data:** Opcjonalne dla testowania

#### 4.1.4 Walidacja
- FluentValidation dla Business Logic
- DataAnnotations dla Model Binding
- Server-side walidacja przed zapisem w BD

#### 4.1.5 Obsługa błędów
- Try-catch w controllersach
- Zwracanie odpowiednich HTTP status codes
- Logowanie błędów serwera

---

### 4.2 Frontend - Blazor Server

#### 4.2.1 Struktura komponentów
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
│   └── BookTable.razor (component)
└── Layout/
    └── MainLayout.razor
```

#### 4.2.2 Formularze
- **EditForm** z Blazor Validation
- Client-side validation feedback
- Wyłączenie przycisku submit podczas wysyłania

#### 4.2.3 HTTP Communication
- `HttpClient` service do API calls
- JSON serialization/deserialization

#### 4.2.4 State Management
- Sesja serwera dla authenticated user
- AuthenticationStateProvider (jeśli wymagane)

#### 4.2.5 CSS i Responsywność
- Bootstrap 5 klasy
- Breakpoints: Mobile-first (xs, sm, md, lg, xl)
- Viewport meta tag
- Kontener fluid dla responsywności

---

### 4.3 Baza danych - SQLite

#### 4.3.1 Struktura tabeli `Books`

```sql
CREATE TABLE Books (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Title NVARCHAR(255) NOT NULL,
    Author NVARCHAR(255) NOT NULL,
    YearPublished INTEGER NOT NULL CHECK (YearPublished >= 1000 AND YearPublished <= CAST(strftime('%Y', 'now') AS INTEGER)),
    DateAdded DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT CK_YearRange CHECK (YearPublished >= 1000)
);
```

#### 4.3.2 Indeksy (opcjonalnie dla MVP)
```sql
CREATE INDEX idx_books_dateadded ON Books(DateAdded DESC);
```

#### 4.3.3 Plik bazy danych
- Lokacja: `app.db` w katalogu roboczym aplikacji
- Connection String: `Data Source=app.db;`
- Automatycznie tworzony przy pierwszym uruchomieniu

---

### 4.4 Bezpieczeństwo (MVP)

- SQL Injection: Parametrized queries (EF Core)
- XSS: Blazor auto-escaping
- CSRF: Anti-forgery tokens w formularach
- Password hashing: Nie dotyczy (hardcoded credentials MVP)
- HTTPS: Wymagane w produkcji
- Authentication: Session-based

---

## 5. User Flow

### 5.1 Scenariusz główny

```
1. START - Użytkownik wchodzi na aplikację
   ↓
2. Wyświetlenie strony Login
   ↓
3. Użytkownik wprowadza: username: admin, password: admin
   ↓
4. Klik "Zaloguj"
   ↓
5. Weryfikacja danych (backend)
   ↓
6. Ustawienie sesji (sukces) lub wyświetlenie błędu (niepowodzenie)
   ↓
7. Redirect na Dashboard
   ↓
8. Wyświetlenie listy książek (lub "Brak książek")
   ↓
9. OPCJE AKCJI:
   a) Dodaj nową - klik "Dodaj nową" → formularz
   b) Edytuj - klik "Edytuj" → prefilled formularz
   c) Usuń - klik "Usuń" → potwierdzenie → usunięcie
   d) Wyloguj - klik "Wyloguj" → logout → Login
```

### 5.2 Dodawanie książki
```
Dashboard → Klik "Dodaj nową"
↓
Formularz dodawania (pusta forma)
↓
Wypełnia: Tytuł, Autor, Rok wydania
↓
Klik "Dodaj"
↓
Walidacja client-side + server-side
↓
Zapis w bazie (DateAdded = teraz)
↓
Sukces → Dashboard (zaktualizowana lista)
```

### 5.3 Edytowanie książki
```
Dashboard → Klik "Edytuj" obok książki
↓
Formularz edycji (prefilowane danymi)
↓
Zmiana dowolnych pól
↓
Klik "Zapisz"
↓
Walidacja
↓
UPDATE w bazie
↓
Sukces → Dashboard (zaktualizowana lista)
```

### 5.4 Usuwanie książki
```
Dashboard → Klik "Usuń"
↓
Modal: "Czy usunąć '{tytuł}'?"
↓
Klik "Tak, usuń"
↓
DELETE w bazie
↓
Sukces → Dashboard (lista bez książki)
```

---

## 6. Kryteria akceptacji

### Uwierzytelnienie
- [ ] Użytkownik może zalogować się z danymi: admin/admin
- [ ] Po zalogowaniu sesja jest utrzymywana
- [ ] Przycisk "Wyloguj" usuwa sesję i redirectuje na login
- [ ] Niewylogowany użytkownik nie może uzyskać dostęp do Dashboard

### Zarządzanie książkami - READ
- [ ] Po zalogowaniu widać listę wszystkich książek
- [ ] Lista wyświetla: Tytuł, Autor, Rok, Data dodania, Akcje
- [ ] Pusta lista pokazuje komunikat "Brak książek"
- [ ] Lista ładuje się w poniżej 1 sekundy

### Zarządzanie książkami - CREATE
- [ ] Przycisk "Dodaj nową" przenosi do formularza
- [ ] Formularz zawiera pola: Tytuł, Autor, Rok wydania
- [ ] Wszystkie pola są obowiązkowe (walidacja)
- [ ] Rok: walidacja w przedziale [1000, current_year]
- [ ] Przycisk "Dodaj" zapisuje dane i wraca do Dashboard
- [ ] Przycisk "Anuluj" wraca bez zapisywania
- [ ] `DateAdded` przypisywany automatycznie (serwer)
- [ ] Nowa książka pojawia się na liście

### Zarządzanie książkami - UPDATE
- [ ] Przycisk "Edytuj" otwiera formularz z prefillanymi danymi
- [ ] Pola mają te same walidacje co przy CREATE
- [ ] Przycisk "Zapisz" aktualizuje dane i wraca do Dashboard
- [ ] Przycisk "Anuluj" wraca bez zmian
- [ ] Zmieniona książka pojawia się na liście ze zaktualizowanymi danymi

### Zarządzanie książkami - DELETE
- [ ] Przycisk "Usuń" wyświetla potwierdzenie
- [ ] Potwierdzenie zawiera nazwę książki
- [ ] Po potwierdzeniu książka jest usuwana
- [ ] Po usunięciu lista się odświeża
- [ ] Książka nie pojawia się na liście

### Walidacja i logika biznesowa
- [ ] Rok wydania < 1000 wyświetla błąd
- [ ] Rok wydania > rok_bieżący wyświetla błąd
- [ ] Puste pola wyświetlają błędy walidacji
- [ ] Pola tekstowe akceptują znaki specjalne
- [ ] `DateAdded` jest w formacie DD.MM.YYYY HH:MM

### Testy
- [ ] Unit testy pokrywają walidację roku
- [ ] Unit testy pokrywają walidację pól obowiązkowych
- [ ] Integration testy pokrywają CRUD operacje

### DevOps
- [ ] CI/CD pipeline buduje kod bez błędów
- [ ] CI/CD pipeline uruchamia testy
- [ ] Kod puszczony na `main` jest weryfikowany

---

## 7. Niefunkcjonalne wymagania

### 7.1 Wydajność
- **Czas ładowania listy:** < 1000ms (poniżej 1 sekundy)
- **Czas odpowiedzi API:** < 500ms
- **Rozmiar bazy danych:** Bez limitu dla MVP (SQLite wystarczy do miliona rekordów)

### 7.2 Responsywność
| Urządzenie | Breakpoint | Testowanie |
|-----------|-----------|-----------|
| Mobile | < 576px | iPhone 12, Galaxy S21 |
| Tablet | 576px - 992px | iPad Air |
| Desktop | > 992px | 1920x1080, 2560x1440 |

### 7.3 Bezpieczeństwo (MVP - podstawowe)
- Sesje bez HTTPS (ale w produkcji HTTPS obowiązkowe)
- Basic auth (mock)
- CSRF protection
- SQL injection prevention (EF Core)
- XSS prevention (Blazor escaping)

### 7.4 Kompatybilność przeglądarek
- Chrome 90+
- Firefox 88+
- Edge 90+
- Safari 14+

### 7.5 Dostępność (A11y)
- Semantyczne HTML (labels, form elements)
- Kontrast kolorów: WCAG AA
- Keyboard navigation: Tab, Enter, Escape

### 7.6 Skalowalność
- MVP dla jednego użytkownika
- Baza SQLite (nie wymagana skalowanie w pionie)
- Architektura pozwala na przejście na SQL Server/PostgreSQL

---


## 9. Definicje i Słownik

| Termin | Definicja |
|--------|----------|
| **MVP** | Minimum Viable Product - minimalna funkcjonalność do uruchomienia |
| **CRUD** | Create, Read, Update, Delete - podstawowe operacje |
| **Session** | Sesja użytkownika utrzymana na serwerze |
| **Prefill** | Wstępne wypełnienie formularza istniejącymi danymi |
| **DateAdded** | Timestamp gdy książka została dodana do systemu |
| **Entity** | Model danych reprezentujący tabelę |
| **DbContext** | Klasa zarządzająca bazą danych (EF Core) |
| **Mock-up** | Prototyp / uproszczona implementacja |

---

## 10. Harmonogram i Milestones

### Phase 1: MVP (Tygodnie 1-2)
- [ ] Setup projektu (ASP.NET Core 8 + Blazor)
- [ ] Baza danych (Entity Framework migrations)
- [ ] Login screen (mock auth)
- [ ] CRUD operacje (API + UI)

### Phase 2: Testing & Polish (Tydzień 3)
- [ ] Unit testy (validacja)
- [ ] Integration testy (CRUD)
- [ ] Bug fixes
- [ ] UI/UX polish

### Phase 3: Deployment & Documentation (Tydzień 4)
- [ ] Dokumentacja API
- [ ] README.md
- [ ] Instrukcje deploymentu
- [ ] CI/CD setup

---

## 11. Ryzyka i Mitygacja

| Ryzyko | Wpływ | Prawdopodobieństwo | Mitygacja |
|--------|--------|-------------------|-----------|
| Problema z EF Core migrations | Średni | Niska | Testy na lokalnym środowisku |
| Performance Blazor przy dużej liście | Średni | Średnia | Implementacja paginacji |
| Bug w walidacji roku | Wysoki | Niska | Unit testy, manual testing |
| Brak responsesu API | Wysoki | Niska | Error handling, logging |

---

## 12. Sukces projektu

Projekt będzie uważany za **sukces**, gdy:

1. ✅ Wszystkie kryteria akceptacji zostaną spełnione
2. ✅ Unit testy pokrywają > 80% logiki biznesowej
3. ✅ Brak krytycznych bugów
4. ✅ Czas ładowania < 1s
5. ✅ Kod przechodzi code review
6. ✅ Dokumentacja jest aktualna
7. ✅ Aplikacja jest deployowalna na produkcję

---

**Dokument wersji:** 1.0
**Ostatnia aktualizacja:** 2025-11-02
