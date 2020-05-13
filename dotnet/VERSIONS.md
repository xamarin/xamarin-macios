# Versioning

## Scheme

Our NuGet packages are versioned using [Semver 2.0.0][2].

This is the scheme: `OsMajor.OsMinor.InternalRelease[-prereleaseX]+sha.1b2c3d4`.

* Major: The major OS version.
* Minor: The minor OS version.
* Patch: Our internal release version based on `100` as a starting point.
    * Service releases will bump the last two digits of the patch version
    * Feature releases will round the patch version up to the nearest 100
      (this is the same as bumping the first digit of the patch version, and
      resetting the last two digits to 0).
    * This follows [how the dotnet SDK does it][1].
* Pre-release: Optional (e.g.: Xcode previews, CI, etc.)
    * For CI we use a `ci` prefix + the branch name (cleaned up to only be
      alphanumeric) + the commit distance (number of commits since any of the
      major.minor.patch versions changed).
        * Example: `iOS 15.0.100-ci.master.1234`
        * Alphanumeric means `a-zA-Z0-9-`: any character not in this range
          will be replaced with a `-`.
    * Pull requests have `pr` prefix, followed by `gh`+ PR number + commit
      distance.
        * Example: `tvOS 15.1.200-ci.pr.gh3333.1234`
    * If we have a particular feature we want people to subscribe to (such as
      an Xcode release), we publish previews with a custom pre-release
      identifier:
        * Example: `iOS 15.1.100-xcode13-1.beta.1`
        * This way people can sign up for only official previews, by
          referencing `iOS *-xcode13-1.beta.*`
        * It's still possible to sign up for all `xcode13-1` builds, by
          referencing `iOS *-ci.xcode11-3.*`
* Build metadata: Required Hash
    * This is `sha.` + the short commit hash.
        * Use the short hash because the long hash is quite long and
          cumbersome. This leaves the complete version open for duplication,
          but this is extremely unlikely.
    * Example: `iOS 14.0.100+sha.1a2b3c`
    * Example (CI build): `iOS 15.0.100-ci.master.1234+sha.1a2b3c`
    * Since the build metadata is required for all builds, we're able to
      recognize incomplete version numbers and determine if a particular
      version string refers to a stable version or not.
        * Example: `iOS 15.0.100`: incomplete version
        * Example: `iOS 15.0.100+sha.1a2b3c`: stable
        * Example: `iOS 15.0.100-ci.d17-0.1234+sha.1a2b3c`: CI build
        * Example: `iOS 15.0.100-xcode13-1.beta.1+sha.1a2b3c`: official
          preview
            * Technically it's possible to remove the prerelease part, but
              we’d still be able to figure out it’s not a stable version by
              using the commit hash.

[1]: https://github.com/dotnet/designs/blob/master/accepted/2018/sdk-version-scheme.md
[2]: https://semver.org
