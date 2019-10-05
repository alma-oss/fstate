namespace State

module ConcurrentStorage =
    open System.Collections.Concurrent

    type Key<'UniqueData> = Key of 'UniqueData

    type private Storage<'UniqueData, 'State> = ConcurrentDictionary<Key<'UniqueData>, 'State>
    type State<'UniqueData, 'State> = private State of Storage<'UniqueData, 'State>

    let create<'UniqueData, 'State> () =
        Storage<'UniqueData, 'State>()
        |> State

    let setState (State storage) key state =
        storage.AddOrUpdate(key, state, fun _ _ -> state)
        |> ignore

    let addOrUpdateState (State storage) (update: Key<'a> -> 'b -> 'b -> 'b) key newState =
        storage.AddOrUpdate(key, newState, (fun key currentState -> update key currentState newState))
        |> ignore

    let getState (State storage) key =
        match storage.TryGetValue key with
        | true, state -> Some state
        | _ -> None

    let countAll (State storage) =
        storage.Count

    [<RequireQualifiedAccess>]
    module State =
        let empty = create

        let iter f (State storage) =
            storage
            |> Seq.map (fun kv -> kv.Key, kv.Value)
            |> Seq.iter f

        let length = countAll

        let tryFind key storage =
            key |> getState storage

        let set key value storage =
            value |> setState storage key

        let update key value update storage =
            value |> addOrUpdateState storage update key
