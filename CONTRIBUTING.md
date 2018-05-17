Thanks for thinking about contributing to the project! All suggestions, bugs, even typo fixes are most appreciated

Please open an issue and have a chat about any features you're thinking of implementing before you start,
so we can discuss the best way to go about it

## Purpose

Our aim when making this library was to enable absolute minimum code and effort on the client side. 
All the logging complexity is handled in the library, and client is only made to write minimum of declarative and simple code

## Running

Clone out the repo, fire up with Visual Studio, and run all the tests

When working on the feature use this checklist:
* Implemented change
* Unit tested the change
* (Recommended) ReSharper code inspections ran (and code is clean)
* Green build
* Green unit tests
* Documentation is up to date

## Deploying

* Change major/minor versions in `appveyor.yml` if needed (build number is handled by AppVeyor)
* Update `CHANGELOG.md` to note expected build number after AppVeyor runs
* The NuGet package is automatically generated on AppVeyor and deployed to NuGet.org
* Create a git tag in the `v1.2.3` format