# AGENTS.md — Alma.State

## Agent Skills

This repo ships Agent Skill for the `Alma.State` library. Compatible agents discover it automatically; see `.agents/skills/fstate/SKILL.md`.

## Project Purpose

F# library for handling in-memory state management with concurrency support. Provides thread-safe concurrent storage (key-value store with add/update semantics) and a temporary cache with configurable TTL and debounce-based loading. Published as NuGet package `Alma.State`.

## Tech Stack

- **Language:** F# (.NET 10)
- **Framework:** .NET SDK library
- **Package management:** Paket
- **Build system:** FAKE (F# Make) via `build.sh`
- **Testing:** Expecto (via `YoloDev.Expecto.TestSdk`)
- **Linting:** fsharplint
- **CI/CD:** GitHub Actions
- **Key dependencies:** `FSharp.Core ~> 10.0`, `Feather.ErrorHandling ~> 2.0`

## Commands

```bash
# Install dependencies
dotnet tool restore && dotnet paket install

# Build
./build.sh build

# Run tests
./build.sh -t tests

# Lint
dotnet fsharplint lint State.fsproj
```

## Project Structure

```
fstate/
├── State.fsproj                # Main project (PackageId: Alma.State, v11.0.0)
├── src/
│   ├── State.fs                # ConcurrentStorage — thread-safe key-value store
│   └── TemporaryCache.fs       # TemporaryCache — TTL-based caching with debounce loading
├── tests/
│   └── tests.fsproj            # Expecto test project
├── build/
│   └── ...
├── build.sh
├── paket.dependencies
├── paket.references            # FSharp.Core, Feather.ErrorHandling
├── global.json                 # .NET SDK 10.0.0
├── fsharplint.json
├── CHANGELOG.md
└── .github/workflows/
    ├── tests.yaml
    ├── pr-check.yaml
    └── publish.yaml
```

## Architecture

Two main modules:

### ConcurrentStorage (`State.fs`)
- `create<'Key, 'Value>` — creates a new concurrent storage
- `setState` / `getState` — basic set/get operations
- `addOrUpdateState` — upsert with custom merge function
- Thread-safe via `ConcurrentDictionary`

### TemporaryCache (`TemporaryCache.fs`)
- `TemporaryCache.Millisecond.ofMinutes` — create TTL duration
- `TemporaryCache.debounceLoad` — cached async data loading with TTL expiry
- Prevents redundant API calls by caching results for a configurable duration

## Build System (FAKE)

Standard library target chain: `Clean → AssemblyInfo → Build → Lint → Tests → Release → Publish`

## CI/CD

- **tests.yaml** — runs on PRs and nightly
- **pr-check.yaml** — blocks fixup commits, runs ShellCheck
- **publish.yaml** — publishes to NuGet on semver tags

## Release Process

1. Increment `<Version>` in `State.fsproj`
2. Update `CHANGELOG.md`
3. Commit, tag with version, push

## Conventions

- Functional style with partial application for dependency injection
- `Key` wrapper type for storage keys
- `asyncResult {}` CE for async operations with error handling
- `Feather.ErrorHandling` for Result/AsyncResult patterns

## Pitfalls

- **No AssemblyInfo.fs** — this project doesn't have a root `AssemblyInfo.fs`; build generates it
- **Concurrency** — `ConcurrentStorage` is thread-safe, but custom merge functions in `addOrUpdateState` must also be thread-safe
- **Cache key collisions** — `debounceLoad` uses string keys; ensure uniqueness across callers
- **Paket, not NuGet CLI** — use `dotnet paket install`
- **Test framework** — uses Expecto, not xUnit/NUnit
