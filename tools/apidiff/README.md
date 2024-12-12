# New Releases

Inside `Make.config` update the `STABLE_NUGET_VERSION_<platform>=` line to point to the currently stable ref pack's NuGet version.

E.g. for the ref pack `Microsoft.iOS.Ref.net8.0_17.5` with the version `17.5.8030` it would be this:

```make
STABLE_NUGET_VERSION_iOS=net8.0_17.5/17.5.8030
```

# New Revisions

On the bots each revision rebuilds every assemblies. Each of them will be compared to the downloaded stable version from the stable NuGet. Any changes (addition/removal) to the public API will be reported in HTML files.

This can be done manually with `make`. The `.\diff\` directory will contain the diffs in HTML format.

The helper `make merge` target creates a single `api-diff.html` file (from all the `diff\*.html` files) that be used for the documentation web site.

# Technical implementation

When comparing against the stable version of the ref pack NuGet, the process goes like this:

1. Download the stable NuGet/nupkg (using a direct link, not 'dotnet restore', because we know exactly which version we need, so a direct link is both faster and less likely to fail).
	a. Cache the nupkg locally if `MACIOS_CACHE_DOWNLOADS=1` is set (the nupkg will be fetched from the cache instead if being downloaded if it exists in the cache)
2. Extract the reference assembly from the nupkg.
3. Create an api description xml for the ref pack's ref assembly.
4. Create an api description xml for the locally built ref assembly.
5. Compare the two reference xml files, and write out the result, both in html and markdown.
6. Create an api-diff.html file that summarizes the comparison result.

When comparing against a different commit (using the tools/compare-commits.sh
script), the script will change the logic above to use the other commit's ref
assembly instead of the ref pack's ref assembly, otherwise the process is
identical.
