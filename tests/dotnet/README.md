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

Build with `/p:EnableAssemblyILStripping=false` set. The `MtouchExtraArgs` legacy option is also honored. 
