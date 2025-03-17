# Changelog

<!-- There is always Unreleased section on the top. Subsections (Add, Changed, Fix, Removed) should be Add as needed. -->
## Unreleased
- Update dependencies

## 9.0.0 - 2025-03-13
- [**BC**] Use net9.0

## 8.0.0 - 2024-01-09
- [**BC**] Use net8.0
- Fix package metadata

## 7.0.0 - 2023-09-11
- [**BC**] Use `Alma` namespace

## 6.0.0 - 2023-08-10
- [**BC**] Use net 7.0

## 5.0.0 - 2022-01-05
- [**BC**] Use net6.0

## 4.1.0 - 2021-02-15
- Update dependencies

## 4.0.0 - 2020-11-23
- [**BC**] Use .netcore 5.0

## 3.0.0 - 2020-11-23
- Update dependencies
- [**BC**] Use .netcore 3.1
- [**BC**] Change namespace to `Lmc.State`

## 2.5.0 - 2020-04-20
- Add `State.keepLastSortedBy` function
- Add tests

## 2.4.0 - 2020-03-31
- Add `TemporaryCache` module

## 2.3.0 - 2020-03-27
- Add `State.clear` function

## 2.2.0 - 2020-03-27
- Add `State.tryRemove` function

## 2.1.0 - 2020-03-06
- Add `State.items` function
- Add `Key.value` function
- Change git host
- Add `AssemblyInfo`

## 2.0.0 - 2019-10-05
- [**BC**] Fix typo in function name - `State.lenght` -> `State.length`
- [**BC**] Fix typo in module name - `ConcurentStorage` -> `ConcurrentStorage`
- Add functions to access keys and values:
    - `getAllKeys` - returns `Generic.ICollection`
    - `getAllValues` - returns `Generic.ICollection`
    - `State.keys` - returns `list`
    - `State.values` - returns `list`

## 1.3.0 - 2019-06-26
- Add `ConcurentStorage.State` module to allow _limited_ collection-like access functions.
    - `empty`
    - `iter`
    - `length`
    - `set`
    - `update`
    - `tryFind`

## 1.2.0 - 2019-06-26
- Add lint

## 1.1.0 - 2019-03-26
- Add `countAll` function

## 1.0.0 - 2019-02-28
- Initial implementation
