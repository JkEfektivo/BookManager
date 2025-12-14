# CI/CD Pipeline

## Przegląd

Pipeline CI/CD jest skonfigurowany w GitHub Actions i automatycznie uruchamia się przy każdym pushu lub pull requeście.

## Lokalizacja

```
.github/workflows/ci.yml
```

## Triggery

| Wydarzenie | Gałęzie |
|------------|---------|
| Push | `main`, `master`, `develop` |
| Pull Request | `main`, `master` |

## Jobs

### 1. Build & Test

Uruchamia się zawsze przy każdym uchu/PR.

**Kroki:**
1. **Checkout** - pobiera kod z repozytorium
2. **Setup .NET 8** - instaluje SDK .NET 8
3. **Restore** - przywraca pakiety NuGet
4. **Build** - buduje aplikację w trybie Release
5. **Test** - uruchamia testy integracyjne
6. **Upload results** - zapisuje wyniki testów jako artifact

### 2. Publish

Uruchamia się tylko na gałęziach `main`/`master` po pomyślnym buildzie.

**Kroki:**
1. **Checkout** - pobiera kod
2. **Setup .NET 8** - instaluje SDK
3. **Publish** - tworzy paczkę deployment
4. **Upload artifact** - zapisuje paczkę do pobrania

## Artifacts

| Nazwa | Opis |
|-------|------|
| `test-results` | Wyniki testów w formacie TRX |
| `bookmanager-app` | Skompilowana aplikacja gotowa do wdrożenia |

## Status Badge

Dodaj do README.md:

```markdown
![CI/CD](https://github.com/TWOJ-USERNAME/BookManager/actions/workflows/ci.yml/badge.svg)
```

## Lokalne testowanie workflow

Możesz przetestować workflow lokalnie używając [act](https://github.com/nektos/act):

```bash
# Instalacja (Windows)
choco install act-cli

# Uruchomienie
act push
```

## Rozwiązywanie problemów

### Build failed - restore error

```bash
# Sprawdź czy solucja się buduje lokalnie
dotnet restore BookManager/BookManager.sln
dotnet build BookManager/BookManager.sln
```

### Tests failed

```bash
# Uruchom testy lokalnie
dotnet test BookManager.Tests --verbosity normal
```

### Artifact not found

Upewnij się, że ścieżki w workflow są poprawne względem katalogu głównego repozytorium.
