# generate-defines

This script takes the list of frameworks that are supported for a given
platform, and generates a response file for the C# compiler with a
`HAS_<framework>` define for each framework.

Example output file for iOS:

```
-d:HAS_ACCELERATE
-d:HAS_ACCESSIBILITY
-d:HAS_ACCESSORYSETUPKIT
-d:HAS_ACCOUNTS
[...]
-d:HAS_WEBKIT
-d:HAS_XKIT
```
