F-State
=======

[![NuGet](https://img.shields.io/nuget/v/Alma.State.svg)](https://www.nuget.org/packages/Alma.State)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Alma.State.svg)](https://www.nuget.org/packages/Alma.State)
[![Tests](https://github.com/alma-oss/fstate/actions/workflows/tests.yaml/badge.svg)](https://github.com/alma-oss/fstate/actions/workflows/tests.yaml)

Library for handling internal memory state (_concurrently, etc._).

## Install

Add following into `paket.references`
```
Alma.State
```

## Use

```fs
module Example =
    open Alma.State.ConcurrentStorage

    let doSomething () =
        //
        // init state storage
        //
        let storage = create<string, string>()
        let set = setState storage
        let get = getState storage

        let concatStateValue _key oldValue newValue = oldValue + newValue
        let upsert = addOrUpdateState storage concatStateValue

        //
        // working with state
        //
        let keyJohn = Key "John"
        let keyPeter = Key "Peter"
        let keyMary = Key "Mary"

        set keyJohn "Snow"
        set keyPeter "Snow"

        keyJohn
        |> get
        |> Option.map (printfn "State for John is %s")
        |> ignore   // Option.map returns `union option` which must be ignored explicitly
        // prints: "State for John is Snow"

        keyPeter
        |> get
        |> Option.map (printfn "State for Peter is %s")
        |> ignore
        // prints: "State for Peter is Parker"

        keyMary
        |> get
        |> Option.map (printfn "State for Mary is %s")
        |> ignore
        // wont print anything

        // Change/update

        upsert keyMary "Jane Watson"
        upsert keyPeter " (Spider-Man)"
        set keyJohn "Knows nothing"

        keyMary
        |> get
        |> Option.map (printfn "State for Mary is %s")
        |> ignore
        // prints: "State for Mary is Jane Watson"

        keyPeter
        |> get
        |> Option.map (printfn "State for Peter is %s")
        |> ignore
        // prints: "State for Peter is Parker (Spider-Man)"

        keyJohn
        |> get
        |> Option.map (printfn "State for John is %s")
        |> ignore
        // prints: "State for John is Knows nothing"
```

## Temporary Cache

### Load and debounce
```fs
let private searchFreshData api = asyncResult {
    return! api |> executeSearch
}

let loadData api =
    TemporaryCache.Millisecond.ofMinutes 5
    |> TemporaryCache.debounceLoad "api.data" (fun () -> searchFreshData api)

let example () =
    asyncResult {
        let api = "..."

        let! data = loadData api        // fresh data loaded and cached for 5 minutes
        let! data = loadData api        // data loaded from cache and cache is debounce for another 5 minutes

        do! Async.Sleep(4 * 60 * 1000)  // wait for 4 minutes
        let! data = loadData api        // data loaded from cache and cache is debounce for another 5 minutes

        do! Async.Sleep(4 * 60 * 1000)  // wait for 4 minutes
        let! data = loadData api        // data loaded from cache and cache is debounce for another 5 minutes

        return data                     // data old for 8 minutes now
    }
```

### Load
```fs
let private searchFreshData api = asyncResult {
    return! api |> executeSearch
}

let loadData api =
    TemporaryCache.Millisecond.ofMinutes 5
    |> TemporaryCache.load "api.data" (fun () -> searchFreshData api)

let example () =
    asyncResult {
        let api = "..."

        let! data = loadData api        // fresh data loaded and cached for 5 minutes
        let! data = loadData api        // data loaded from cache

        do! Async.Sleep(4 * 60 * 1000)  // wait for 4 minutes
        let! data = loadData api        // data loaded from cache

        do! Async.Sleep(4 * 60 * 1000)  // wait for 4 minutes
        let! data = loadData api        // fresh data loaded and cached for 5 minutes

        return data                     // data old for just a few milliseconds
    }
```

### Load with TTL
```fs
let getOAuthToken () = asyncResult {
    return {|
        Token = "token"
        Expires = 84000
    |}
}

let loadToken () =
    TemporaryCache.loadWithTTL "oauth.token" (fun () -> asyncResult {
        let! tokenInfo = getOAuthToken()

        return {
            Data = tokenInfo.Token
            CacheFor = System.TimeSpan.FromSeconds(float tokenInfo.Expires)
        }
    })

let example () =
    asyncResult {
        // first call loads fresh token and caches it for tokenInfo.Expires seconds
        let! token1 = loadToken ()
        printfn "Got token: %s" token1

        // subsequent call (within TTL) returns cached token
        let! token2 = loadToken ()
        printfn "Got token again: %s" token2

        return token2
    }
```

## Release
1. Increment version in `State.fsproj`
2. Update `CHANGELOG.md`
3. Commit new version and tag it

## Development
### Requirements
- [dotnet core](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)

### Build
```bash
./build.sh build
```

### Tests
```bash
./build.sh -t tests
```
