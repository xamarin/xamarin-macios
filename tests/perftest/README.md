# Benchmarks

This folder contains a test suite to test performance in Xamarin.iOS/Xamarin.Mac.

There are three variations of the test suite:

* Legacy mode (Xamarin).

* .NET Mode (Mono): runs tests in .NET, with MonoVM.

* .NET Mode (CoreCLR): runs tests using .NET, with CoreCLR.

## Running the benchmarks

Run all three variations of the test suite:

```shell
make run-perftest
```

Run each variation:

### Legacy mode

```shell
make run-perftest-with-legacy
```

### .NET Mode (Mono)

```shell
make run-perftest-with-mono
```

### .NET Mode (CoreCLR)

```shell
make run-perftest-with-coreclr
```

## Results

At the end of the test run, something like this will be printed:

> Log files were stored in xamarin-macios/tests/perftest/output/2021-04-22--21:43:28

and that's where you'll find log files. Console output from the executable is
stored as perflog-[timestamp].log, while BenchmarkDotNet results are exported
to the results/ subfolder (in json, markdown, csv and html formats).

There's no easy way to compare results across variations or runs (yet), the
exported results will have to be examined by a human and compared that way.
