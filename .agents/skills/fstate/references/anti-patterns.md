# Anti-Patterns for Alma.State

Use this format for each issue: mistake -> why -> fix.

## Mistakes and Fixes
- Mistake: Reusing broad cache key strings across unrelated loaders.
  Why: Different payload shapes can collide behind the same key and later reads can fail with type mismatch behavior.
  Fix: Define dedicated key builders per wrapper module and include technical scope segments in every key.

- Mistake: Treating debounceLoad and load as interchangeable.
  Why: Mode selection by habit instead of workload profile can violate freshness expectations.
  Fix: Apply the mode-selection guidance in preferred-patterns.md and keep that decision explicit in your wrapper API.

- Mistake: Writing merge callbacks with hidden side effects.
  Why: Concurrent updates become difficult to reason about and state transitions may depend on execution timing.
  Fix: Keep merge callbacks pure, deterministic, and based only on current and incoming values.

- Mistake: Returning raw library error details from outer service layers.
  Why: It leaks implementation concerns and complicates cross-layer contracts.
  Fix: Map errors once at boundary adapters and expose stable, caller-oriented error shapes.

- Mistake: Building cache keys ad hoc inline at call sites.
  Why: Uncoordinated string construction fragments lookup behavior and weakens key hygiene.
  Fix: Use one key-builder function per wrapper module and reference that builder from all cache entry points.

- Mistake: Assuming state pruning is an in-place operation in all size scenarios.
  Why: Caller assumptions about object identity can break when pruning strategy changes internal storage handling.
  Fix: Treat pruning result as authoritative return value and always continue with the returned state handle.

- Mistake: Treating library helpers as a replacement for persistence or distributed coordination.
  Why: In-memory primitives do not provide cross-process durability or global coordination guarantees.
  Fix: Restrict usage to process-local concerns and integrate external systems for durable/shared requirements.

## Legacy Usage to Avoid
- Mistake: Relying on historical naming from older package versions when writing new code.
  Why: Legacy references cause confusion during maintenance and code search.
  Fix: Use current Alma.State module and function names consistently in wrappers, tests, and docs.
