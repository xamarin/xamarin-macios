# Versioning

## Scheme

Our NuGet packages are versioned using [Semver 2.0.0][2].

This is the scheme: `OsMajor.OsMinor.InternalRelease[-prereleaseX]+sha.1b2c3d4`.

* Major: The major OS version.
* Minor: The minor OS version + a digit specifying our managed API version (typically "0").
    * See [API changes](#API Changes) below for a description of the two API
      version digits.
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
        * Example: `iOS 15.0.100-ci.main.1234`
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
    * Example (CI build): `iOS 15.0.100-ci.main.1234+sha.1a2b3c`
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

## Changes in the managed API shape.

According to Semver 2.0.0 rules, we must bump either the Major or the Minor
version number when the API changes.

This is also how .NET in general behaves, there won't be an API changes in a
6.0.200 release compared to a 6.0.100 release for instance.

Here's another way to reason about it:

1. It must be possible to use an assembly build with release v1.0.100 by
   somebody else who has v1.0.101 installed (backwards compatibility).
2. It must be possible to use an assembly built with v1.0.101 by somebody show
   has v1.0.100 installed (forward compatibility).

This means that there can't be any changes in v1.0.101 that changes the API
shape in any way (such as adding a class or method), because then an assembly
can build with v1.0.101 using this new API, and it won't work on v1.0.100,
because the new API doesn't exist there.

A safe assumption is to _always_ bump the feature version number when there
are _any_ API changes (in particular this is a much bigger restriction than
just _no breaking changes_).

Since we don't control the major and minor OS versions (Apple decides those,
and when to release them), and we still need a way to publish new releases
with managed API changes when we want to, we've appended a digit to the minor
version. Typically these will be "0", but if we really want/need to, we could
add new managed API outside of an OS release and bump these digits.

Why don't we use "00" to get a scheme similar to the [.NET patch versioning][3]
(6.0.1XX)? Because MSIs have a max of 255 for the major and minor version, so
by adding a single digit only, we're allowing for 25 minor versions for a
given major OS release. Hopefully that will be enough... 🔮

[1]: https://github.com/dotnet/designs/blob/master/accepted/2018/sdk-version-scheme.md
[2]: https://semver.org
[3]: https://docs.microsoft.com/en-us/dotnet/core/versions/#understand-runtime-version-number-changes

