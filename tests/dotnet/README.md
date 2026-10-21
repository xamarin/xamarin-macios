# .net tests

## Timed-out test applications

Applications launched by the unit tests' shared execution helper have a 30-second
timeout. On macOS, before killing a timed-out application, the helper records the
OS and architecture, process status, and a five-second `/usr/bin/sample` thread
sample in the test output. These diagnostics are included in CI's NUnit results
and HTML report. Diagnostic commands have separate timeouts; collecting them does
not retry the application or turn the timeout into a passing test.

## size-comparison

To install the latest `appcompare` tool do:

```bash
$ dotnet tool install --global appcompare
```

You can update it to the latest version by running:

```bash
$ dotnet tool update --global appcompare
```

The current directory might point to a different and incompatible dotnet
SDK, resulting in an error. However running the command from a different
location (outside the repo) should work.

### Easier Analysis

If you want to read/compare the IL inside the assemblies you need to disable IL stripping.

* Legacy (oldnet)

Add this option inside the `Release|iPhone` configuration of `size-comparison/MySingleView/oldnet/MySingleView.csproj`

```xml
<MtouchExtraArgs>--nostrip</MtouchExtraArgs>
```

* net6

Build with `/p:EnableAssemblyILStripping=false` set. The `MtouchExtraArgs` legacy option is also honored. 
