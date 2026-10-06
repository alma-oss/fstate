# Preferred Patterns for Alma.State

## Core Principles
- Keep state key design explicit early; choose unique key payloads that reflect the technical identity you query by.
- Prefer a small wrapper module per use case that centralizes cache key literals and state helper composition.
- Keep merge functions deterministic and side-effect free so concurrent update behavior remains predictable.

## Recommended API Usage
- Use the direct ConcurrentStorage functions when you need low-level control over set/get/upsert wiring.
- Use State submodule helpers when you want pipeline-friendly operations for iteration, lookup, and pruning.
- Use TemporaryCache.load for fixed retention windows where reads should not prolong item lifetime.
- Use TemporaryCache.debounceLoad for hot-read scenarios where each successful read should extend retention.
- Use TemporaryCache.loadWithTTL when retention duration is computed from loader output at runtime.
- Use TemporaryCache.invalidate when callers need explicit lifecycle control after external state transitions.
- For concrete call shapes, see examples.md and select the section matching your scenario complexity.

## Error Handling
- Keep loader functions in AsyncResult form and map library-specific errors near the call boundary.
- Surface LoadError at service boundaries when callers need to distinguish loader failures from cache-data issues.
- Convert LoadError to transport- or UI-friendly errors only at outer layers, not in core cache wrappers.

## Composition
- Build small functions that close over storage/cache handles and expose narrow operations to callers.
- Compose state and cache helpers with partial application to reduce repeated wiring.
- Keep cache-key construction in one place to avoid accidental key drift across modules.

## Integration with Other Libraries
- Pair TemporaryCache with Feather.ErrorHandling computation expressions for concise async error flows.
- Keep serialization or external I/O outside cache wrapper functions; let loaders return already-shaped domain-neutral DTOs.

## Naming Conventions
- Name storage wrappers by capability, such as CacheInstance, StateRegistry, or SessionStateStore.
- Name cache keys by technical namespace shape, such as ServiceA.resource.summary.
- Name upsert functions to describe merge intent, such as mergeLatest, appendChunk, or keepHigherVersion.

## Testing Recommendations
- Use table-driven tests for trimming logic and merge behavior boundaries.
- Verify both cache miss and cache hit paths when testing cache wrapper functions.
- Include explicit tests for key uniqueness contracts in modules that build dynamic cache keys.
- For runnable test-style snippets, see examples.md.
