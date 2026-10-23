# How to publish updated API reference documentation

This document describes how to update the reference documentation on https://learn.microsoft.com for the API we publish (for instance, this is the documentation for [UIView](https://learn.microsoft.com/dotnet/api/uikit.uiview)).

API reference documentation should be updated every time we add new APIs, which typically happen when we add support for a new Xcode version.

Since we don't want to go through this process more than once per release, we
don't start until we have the final hash we're going to release (the easiest
is often to wait until packages have been published to NuGet).

The steps are:

1. [Create new monikers](#how-to-create-new-monikers).
	This might take a few days, and can be done before we have the final packages.

2. Clone the [binaries](https://apidrop.visualstudio.com/_git/binaries) repository if you haven't already done so.

3. Add our assemblies to the [binaries](https://apidrop.visualstudio.com/_git/binaries) repository.

    This can be done automatically with the `update-api-docs.sh` script (can be found next to this document), but for documentation purposes, these are the steps:

    * Create new branch off `master`named `netX.Y-xcodeZ.W`.
    * Create new directories inside the `dotnet-macios` directory, named according to the monikers created above.
    * Copy our platform assemblies and their xml files into their corresponding directory.
    * Create a new commit and push it to origin.

4. Go here: [OPS dotnet/macios-api-docs / Continuous Integration](https://ops.microsoft.com/#/repos/85f784f4-01e7-ffb8-ed06-a012f7d649c0?tabName=ci) (if you have to go through the authentication workflow you'll end up on the OPS homepage, in which case just click the link again) and then:

	* Expand the '.NET macios API docs' job, and then:
	* Change `Target Repo` -> `Target Branch` to `netX.Y-xcodeZ.W` (same branch as created above).
	* Change `Source Dll Repo` -> `Repo Branch` to `netX.Y-xcodeZ.W` (yup, same branch again).
	* Save.

5. Run this pipeline: [.NET macios API docs](https://apidrop.visualstudio.com/Content%20CI/_build?definitionId=8026). Don't change any options. This will take ~30 minutes.

    A new build will show up in OPS a little while after the pipeline has finished running: [Build History](https://ops.microsoft.com#/repos/85f784f4-01e7-ffb8-ed06-a012f7d649c0?tabName=builds) (might need VPN enabled, otherwise sometimes you'll get a 403 error page)
	
	Look for errors/warnings in the build report. If anything needs to be fixed, do that.

	Some problems can be fixed in the [dotnet/macios-api-docs](https://github.com/dotnet/macios-api-docs) repository directly, just commit and push them to the `netX.Y-xcodeZ.W` branch. This will automatically create a new build in [Build History](https://ops.microsoft.com#/repos/85f784f4-01e7-ffb8-ed06-a012f7d649c0?tabName=builds).

	If any fixes need to be done in [dotnet/macios](https://github.com/dotnet/macios), new packages must be built afterwards, which means starting at the top of this list again.
	
	Please fix all warnings, I went to a great effort to make sure we are free of warnings.

6. Create a pull request in the [dotnet/macios-api-docs](https://github.com/dotnet/macios-api-docs) repository, from the branch `netX.Y-xcodeZ.W` to `main`.

    Once this pull request has been merged, an automated publishing job will get any changes in the `main` branch into the `live` branch, which will then show up on https://learn.microsoft.com.

7. Create a pull request in the [binaries](https://apidrop.visualstudio.com/_git/binaries) repository for the `netX.Y-xcodeZ.W` we created with the new assemblies (into the `master` branch).

8. The final step is to flip the monikers from prerelease to live monikers (another ticket has to be created for this). Sample ticket: [#540739](https://dev.azure.com/msft-skilling/Content/_workitems/edit/540739).

## How to create new monikers

First read the general information about monikers: [How to define and use monikers](https://learn.microsoft.com/en-us/help/platform/reference-define-use-monikers) (need to be signed in to view)

We need a separate moniker for each platform, so 4 in total.

The format is: "net-<ios|tvos|macos|maccatalyst>-<os version>-<net version>".

So for the initial .NET 10 release, when we shipped APIs for iOS 26.0, tvOS 26.0, macOS 26.0 and Mac Catalyst 26.0, the monikers were:

* net-ios-26.0-10.0
* net-tvos-26.0-10.0
* net-macos-26.0-10.0
* net-maccatalyst-26.0-10.0

We can't create monikers ourselves, we have to request their creation:

1. Go here: [Learn Request Central](https://microsoft.sharepoint.com/teams/Partnerships/SitePages/Learn%20Request%20Central.aspx) (you might have to request access the first time you go here)
	1. Under "GitHub Publishing Services" click [Submit your request](https://forms.office.com/Pages/ResponsePage.aspx?id=v4j5cvGGr0GRqy180BHbRxxUz-ZV53lLrgTaBjGRmtBUNlkzT01CSzNBSE1SRU8yRzU5UTZFNjQyOC4u)
	2. Fill in the form with:
		1. I am working on publishing a new content set.
		2. I need help with updating information architecture (IA) or navigation.
		3. Request monikers.
		This will lead you to a template to fill an issue. Here's a sample of how I filled it out once: [#480959](https://dev.azure.com/msft-skilling/Content/_workitems/edit/480959).

Note: monikers are typically created as prerelease monikers. Another request has to be created to flip them to live monikers once everything else is in place.

It can be helpful to view info about all monikers here: [All monikers](https://ops.microsoft.com#/monikers)

## References

* [Document our documentation process/workflow/guidelines - #17401](https://github.com/dotnet/macios/issues/17401)
* [Figure out how to publish XML documentation on our website - #17396](https://github.com/dotnet/macios/issues/17396)
* [Code Documentation Guidelines](code-documentation-guidelines.md)
