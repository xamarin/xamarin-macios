# generate-workloadmanifest-json

This script generates the `WorkloadManifest.json` file, which is a file that describes what's in a workload.

The generated manifest extends the versioned runtime workloads for every supported .NET version, including the latest. This ensures the required packs are installed without also installing tooling for a newer .NET version targeted by the .NET SDK's current runtime workload.

More information about workload manifests can be found here:
https://github.com/dotnet/designs/blob/main/accepted/2020/workloads/workload-manifest.md
