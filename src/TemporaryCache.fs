namespace Lmc.State

open Lmc.ErrorHandling

type CacheData = CacheData of obj

type LoadError<'Error> =
    | LoadFreshDataError of 'Error
    | InvalidTypeOfData

[<RequireQualifiedAccess>]
module LoadError =
    let format formatLoadError = function
        | LoadFreshDataError error -> error |> formatLoadError
        | InvalidTypeOfData -> "Stored data have invalid type."

[<RequireQualifiedAccess>]
module TemporaryCache =
    open System
    open Lmc.State.ConcurrentStorage

    type [<Measure>] Millisecond

    [<RequireQualifiedAccess>]
    module Millisecond =
        let ofMillisecond (miliseconds: int) =
            miliseconds * 1<Millisecond>

        let ofSeconds seconds =
            seconds * 1000 |> ofMillisecond

        let ofMinutes minutes =
            minutes * 60 |> ofSeconds

    type private CacheKey = CacheKey of string
    type private TTL = TTL of DateTime

    let private cache: State<CacheKey, CacheData> = State.empty()
    let private ttls: State<CacheKey, TTL> = State.empty()

    let private debounce key (ttl: int<Millisecond>) =
        ttls |> State.set key (DateTime.Now.AddMilliseconds(ttl |> float) |> TTL)

    let rec private dispose key = async {
        match cache |> State.tryFind key, ttls |> State.tryFind key with
        | Some _, Some (TTL passedDue) when passedDue <= DateTime.Now ->
            cache |> State.tryRemove key
            ttls |> State.tryRemove key

        | Some _, _ ->
            do! Async.Sleep (60 * 1000)
            return! dispose key

        | _ -> ()
    }

    type private Events = {
        OnCacheItemLoaded: Key<CacheKey> * int<Millisecond> -> unit
    }

    let private loadData<'Data, 'Error> key (loadFreshData: unit -> AsyncResult<'Data, 'Error>) ttl events = asyncResult {
        let key = Key (CacheKey key)

        match cache |> State.tryFind key with
        | Some (CacheData data) ->
            events.OnCacheItemLoaded(key, ttl)

            match data with
            | :? 'Data as data -> return data
            | _ -> return! InvalidTypeOfData |> AsyncResult.ofError
        | _ ->
            let! freshData = loadFreshData() |> AsyncResult.mapError LoadFreshDataError

            cache |> State.set key (CacheData freshData)
            debounce key ttl
            dispose key |> Async.Start

            return freshData
    }

    /// Loads data when they are not cached yet, otherwise loads data from cache and debounce a timer to cache them longer
    let debounceLoad<'Data, 'Error> key (loadFreshData: unit -> AsyncResult<'Data, 'Error>) ttl =
        {
            OnCacheItemLoaded = fun (key, ttl) -> debounce key ttl
        }
        |> loadData key loadFreshData ttl

    /// Loads data when they are not cached yet, otherwise loads data from cache and store it for ttl
    let load<'Data, 'Error> key (loadFreshData: unit -> AsyncResult<'Data, 'Error>) ttl =
        {
            OnCacheItemLoaded = ignore
        }
        |> loadData key loadFreshData ttl
