# Cecil-based Tests

Even with the huge variety of tests that we have there's still a few things that are hard to test properly. Some of them can be tested by analyzing the assemblies (IL or metadata). So here we are...

`SignListTest` checks the workload `.nupkg` files in `DOTNET_NUPKG_DIR` against `dotnet/Workloads/SignList.xml`, including files in nested ZIPs. Build or download the workload packages before running the full Cecil suite; CI copies downloaded packages into this directory when installing workloads. When all platforms are enabled, the test prints signing entries not used by any package without failing.
