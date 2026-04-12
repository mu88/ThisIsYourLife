# ThisIsYourLife — Repo Context

## Architecture (Onion)
Five projects form the onion layers (inner → outer):
- `Entities` — domain types (`LifePoint`, `Person`); no dependencies.
- `DTO` — records with mapping extension methods (`ToDto()`, `ToEntity()`); depends on `Entities`.
- `Persistence` — EF Core + SQLite (`Storage : IStorage`), file system (`FileSystem`), user store (`UserService`); depends on `Entities`.
- `BusinessServices` — orchestration (`LifePointService`, `PersonService`); depends on `Entities`, `DTO`, and `Persistence` via `IStorage`.
- `WebApp` — Blazor Server + REST API entry point; depends on all above.

## Persistence / Testing
- `IStorage` (defined in `BusinessServices`) is the persistence abstraction. `Storage` in `Persistence` is the EF Core implementation.
- For unit tests, use `TestStorage` from `Tests/Doubles/` — never use the real `Storage` in unit tests.
- `Tests/Doubles/` also contains test builders for all core entities (`TestPerson`, `TestLifePoint`, `TestPersonToCreate`, etc.) — extend these rather than creating ad-hoc inline test data.

## Blazor Components
- bUnit tests for the interactive components live in `Tests/Unit/WebApp/Shared/` — follow the established patterns there when adding new component tests.

## Localization
- Two supported cultures: `en` (default) and `de`. User-facing strings in Blazor components must use `IStringLocalizer<T>`.

## App Config
- Path base: `/thisIsYourLife` — URL and redirect assertions in tests must account for this prefix.
- `MapOptions` configuration section controls the initial map viewport (latitude, longitude, zoom).
