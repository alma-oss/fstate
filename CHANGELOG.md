# Changelog

<!-- There is always Unreleased section on the top. Subsections (Add, Changed, Fix, Removed) should be Add as needed. -->
## Unreleased

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
