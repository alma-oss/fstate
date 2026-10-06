---
name: fstate
description: >-
  Use when generating or reviewing F# code that imports Alma.State.ConcurrentStorage or TemporaryCache, especially when calling create, setState, getState, addOrUpdateState, State.keepLastSortedBy, TemporaryCache.load, TemporaryCache.debounceLoad, TemporaryCache.loadWithTTL, invalidate, or handling LoadError and AsyncResult-based cache loaders.
---

# F-State

Library: [alma-oss/fstate](https://github.com/alma-oss/fstate)
NuGet: `Alma.State`

## Purpose
Alma.State provides two focused building blocks for in-memory data work in F#: a concurrent key-value state container and a temporary async cache API. This skill helps an agent choose the right module entry points and compose them correctly in application code. It is optimized for fast retrieval while writing or reviewing code that depends on Alma.State.

## When to Use
Use this skill when a task includes shared mutable in-memory state, keyed updates, async cache loading, or TTL-oriented cache invalidation concerns in F# code.

## When NOT to Use
Do not use this skill for distributed caches, persistent databases, external message buses, or architecture-level decisions outside direct Alma.State API usage.

## Main Concepts
- ConcurrentStorage: module that exposes key and state operations over a concurrent container.
- Key<'UniqueData>: strongly typed wrapper used to address values in state operations.
- State<'UniqueData, 'State>: abstracted storage handle used by helper functions in the State submodule.
- State submodule: functional helpers for querying, iterating, trimming, and mutating storage.
- TemporaryCache: module for short-lived async-loaded cache entries with explicit key strings.
- Millisecond: unit-of-measure helper type for cache duration inputs.
- TemporaryCacheResult<'Data>: value with a payload and dynamic cache retention duration.
- LoadError<'Error>: error union that wraps upstream loader failures or stored-data type mismatch.

## Related Libraries
- Feather.ErrorHandling (AsyncResult workflow used by TemporaryCache loaders)
- FSharp.Core (core language/runtime support)

## Keywords for Search
Alma.State, ConcurrentStorage, Key, State.empty, State.set, State.tryFind, State.keepLastSortedBy, TemporaryCache, debounceLoad, loadWithTTL, invalidate, LoadError, AsyncResult, TTL

## Reference Files
For composition principles, API-shape guidance, and testing strategy, read `references/preferred-patterns.md`.
For pitfalls, invalid assumptions, and replacement guidance, read `references/anti-patterns.md`.
For all concrete code snippets and runnable usage patterns, read `references/examples.md`.
