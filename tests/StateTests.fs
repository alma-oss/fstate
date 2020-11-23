module Lmc.State.Tests

open Expecto
open Lmc.State.ConcurrentStorage

type KeepTestData = {
    InitialState: State<int, string>
    Keep: int
    Expected: State<int, string>
}

let stateOf (min, max) =
    let state = State.empty()
    [ for i in min .. max do
        state |> State.set (Key i) (sprintf "value %i" i)
    ]
    |> ignore
    state

let provideData = [
    { InitialState = State.empty(); Keep = 5; Expected = State.empty() }, "Empty state"
    { InitialState = stateOf (0, 3); Keep = 5; Expected = stateOf (0, 3) }, "State with less then to keep"
    { InitialState = stateOf (0, 4); Keep = 5; Expected = stateOf (0, 4) }, "State with same as to keep"
    { InitialState = stateOf (0, 8); Keep = 5; Expected = stateOf (4, 8) }, "State with items to delete from current"
    { InitialState = stateOf (0, 15); Keep = 5; Expected = stateOf (11, 15) }, "State with too much items to delete, it should create a new state"
    { InitialState = stateOf (0, 15); Keep = 0; Expected = State.empty() }, "Keep none from list"
    { InitialState = stateOf (0, 15); Keep = -10; Expected = State.empty() }, "Keep none from list"
]

[<Tests>]
let stateTests =
    testList "State" [
        testCase "should keep last sorted by key" <| fun _ ->
            provideData
            |> List.iter (fun ({ InitialState = initial; Keep = keep; Expected = expected }, description) ->
                let normalizedKeep = if keep < 0 then 0 else keep
                Expect.isLessThanOrEqual (expected |> State.length) normalizedKeep (description + " - wrong expectation!")

                let result =
                    initial
                    |> State.keepLastSortedBy keep fst

                Expect.isLessThanOrEqual (result |> State.length) normalizedKeep description
                Expect.equal (result |> State.items) (expected |> State.items) description
            )
    ]
