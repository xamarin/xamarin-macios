# versions-check

This script verifies the OS versions listed in `xamarin-macios/builds/Versions-<platform>.list.in`:

* No versions below the minimum deployment target.
* Both minimum deployment target and current deployment target must be listed.
* That the `SupportedTargetPlatformVersions` list is coherent with the
  `KnownVersions` list (all versions in `KnownVersions` must also be in
  `SupportedTargetPlatformVersions`).