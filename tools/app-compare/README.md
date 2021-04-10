# Application Comparer

Compare the file sizes between two versions of the same app bundle.

The tool outputs markdown so it's easier to read from a gist.

## Usage

```shell
dotnet run <app1> <app2>
```

## Example

```shell
dotnet run MyOldApp.app MyNewApp.app > out
gist out -t md -o
```
