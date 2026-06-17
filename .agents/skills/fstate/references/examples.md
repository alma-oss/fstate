# Alma.State Examples

All code examples for this skill live in this file only.

## 1. Basic: Concurrent storage set/get

```fs
module BasicStateExample

open Alma.State.ConcurrentStorage

let createStore () =
    create<string, int>()

let readWriteDemo () =
    let store = createStore ()
    let set = setState store
    let get = getState store

    let activeWorkersKey = Key "Worker.active.count"

    set activeWorkersKey 3

    match get activeWorkersKey with
    | Some value -> value
    | None -> 0
```

## 2. Realistic: Upsert merge strategy

```fs
module UpsertMergeExample

open Alma.State.ConcurrentStorage

let createMetricsStore () =
    create<string, int>()

let mergeLatest _key currentValue incomingValue =
    max currentValue incomingValue

let updatePeakCount incoming =
    let store = createMetricsStore ()
    let upsert = addOrUpdateState store mergeLatest
    let key = Key "ServiceA.metrics.peak"

    upsert key incoming

    getState store key
```

## 3. Integration: Fixed TTL cache vs debounce cache

```fs
module CacheModeExample

open Alma.State
open Feather.ErrorHandling

let fetchSummary () = asyncResult {
    return "summary-from-source"
}

let fixedTtlLoad () =
    TemporaryCache.load "ServiceA.summary" fetchSummary (TemporaryCache.Millisecond.ofMinutes 5)

let slidingTtlLoad () =
    TemporaryCache.debounceLoad "ServiceA.summary" fetchSummary (TemporaryCache.Millisecond.ofMinutes 5)

let run () = asyncResult {
    let! fixedValue = fixedTtlLoad ()
    let! slidingValue = slidingTtlLoad ()
    return fixedValue, slidingValue
}
```

## 4. Advanced Integration: Dynamic TTL plus explicit invalidation

```fs
module DynamicTtlExample

open System
open Alma.State
open Feather.ErrorHandling

type TokenPayload = {
    Value: string
    ExpiresInSeconds: int
}

let fetchToken () = asyncResult {
    return {
        Value = "example-token"
        ExpiresInSeconds = 900
    }
}

let loadToken () =
    TemporaryCache.loadWithTTL "ServiceA.auth.token" (fun () -> asyncResult {
        let! token = fetchToken ()
        return {
            Data = token.Value
            CacheFor = TimeSpan.FromSeconds(float token.ExpiresInSeconds)
        }
    })

let rotateToken () = asyncResult {
    TemporaryCache.invalidate "ServiceA.auth.token"
    return! loadToken ()
}
```

## 5. Test Example: keepLastSortedBy behavior

```fs
module KeepLastSortedByTests

open Expecto
open Alma.State.ConcurrentStorage

let stateOf values =
    let state = State.empty()
    values |> List.iter (fun (k, v) -> state |> State.set (Key k) v)
    state

[<Tests>]
let tests =
    testList "keep last sorted values" [
        testCase "keeps highest keys when trimmed" <| fun _ ->
            let initial = stateOf [ 1, "a"; 2, "b"; 3, "c"; 4, "d" ]
            let result = initial |> State.keepLastSortedBy 2 fst

            Expect.equal (result |> State.items) [ (Key 3, "c"); (Key 4, "d") ] "Should keep last two by key"
    ]
```
