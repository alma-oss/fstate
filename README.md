F-State
=======

Library for handling internal memory state (_concurently, etc._).

## Install
```
dotnet add package -s $NUGET_SERVER_PATH Lmc.State
```
Where `$NUGET_SERVER_PATH` is the URL of nuget server
- it should be http://development-nugetserver-common-stable.service.devel1-services.consul:31794 (_make sure you have a correct port, since it changes with deployment_)
- see http://consul-1.infra.pprod/ui/devel1-services/services/development-nugetServer-common-stable for detailed information (and port)

## Use

```fs
module Example =
    open State.ConcurentStorage

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

## Release
1. Increment version in `src/Kafka.fsproj`
2. Update `CHANGELOG.md`
3. Commit new version and tag it
4. Run `$ fake build target release`

## Development
### Requirements
- [dotnet core](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial)
- [FAKE](https://fake.build/fake-gettingstarted.html)

### Build
```bash
fake build
```

### Watch
```bash
fake build target watch
```
