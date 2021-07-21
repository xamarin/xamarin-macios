# .net tests

## size-comparison

### Easier Analysis

If you want to read/compare the IL inside the assemblies you need to disable IL stripping.

* Legacy (oldnet)

Add this option inside the `Release|iPhone` configuration of `size-comparison/MySingleView/oldnet/MySingleView.csproj`

```xml
<MtouchExtraArgs>--nostrip</MtouchExtraArgs>
```

* net6

**IL stripping is not yet available** so there's nothing to disable right now.

If you want to compare (trimmed) size you can manually call `mono-cil-strip`
on each assembly inside the app bundle.

`make strip-dotnet` will remove the IL from the dotnet app version.
However this is done after the code signature so it will not be possible
to deploy and execute the app afterward. Use for binary analysis only!
