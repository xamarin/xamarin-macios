# generate-sourcelink-json

This script generates a SourceLink.json file that maps paths of local source
code into links to GitHub source code.

This SourceLink.json file is then passed to the C# compiler, which embeds it
in the pdb file, and which is then consumed by debuggers or IDEs to find the
source code online for any given source file the pdb refers to.

Example output file:

```json
{
  "documents": {
    "/local/path/to/xamarin-macios/src*": "https://raw.githubusercontent.com/xamarin/xamarin-macios/c2c617bf000c4ff864cbba9d65421f915941136b/src*"
  }
}
```
