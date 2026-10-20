# generate-workloadmanifest-json

This script generates the `WorkloadManifest.json` file, which is a file that describes what's in a workload.

The generated manifest extends both the current runtime workload and the versioned runtime workloads for every supported .NET version, including the latest. This ensures the required packs are installed even when the .NET SDK's current runtime workload targets a newer .NET version than our APIs.

More information about workload manifests can be found here:
https://github.com/dotnet/designs/blob/main/accepted/2020/workloads/workload-manifest.md
