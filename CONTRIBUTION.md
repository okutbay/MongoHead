# Contributing to MongoHead #

Contributions are welcome, whether new core features, bug fixes or examples. Please follow these guidelines.

## Workflow ##
* All contributions should be based off the latest `develop` branch, and pull requests must be submitted to `develop`. Any pull requests to `master` will be rejected.
* Create a separate branch for each patch or feature
* Any modifications to the core must pass unit tests. Whenever possible, especially for new features, please add a test.
* New features should have an example.
* Any copied assets (e.g. images, code etc.) should have the appropriate license and credit allowing for their use in this repository.
* When refactoring existing code, make sure all relevant examples still work or are updated to work.
* If a patch is relevant to a [GitHub issue](https://github.com/okutbay/MongoHead/issues?state=open), please reference the issue number with a hash in the commit message.
* All code should follow the style guide below

## Code Style ##
* Indent with tabs, not spaces
* No trailing spaces
* If JS use single quotes for strings
* Spaces around binary operators (e.g. `a + b`, not `a+b`), but not unary ones
* `if`, `while`, `try`, etc. must all have braces and span multiple lines 
* Insert an empty line before and after code blocks like if
* Use /// to document
* Lots of comments please, but put them on the line before the code, not at the end of the line.
* When in doubt, refer to existing code.

## Best Practices ##
* Not yet