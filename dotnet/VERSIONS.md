# Versioning

## Scheme

Our NuGet packages are versioned using [Semver 2.0.0][2].

This is the scheme: `OsMajor.OsMinor.InternalRelease[-prereleaseX]+sha.1b2c3d4`.

* Major: The major OS version.
* Minor: The minor OS version.
* Patch: Our internal release version based. This is the commit distance since
  the Major.Minor version changed.
* Pre-release: Optional (e.g.: Xcode previews, CI, etc.)
    * For CI we use a `ci` prefix + the branch name (cleaned up to only be
      alphanumeric).
        * Example: `iOS 15.0.123-ci.main`
        * Alphanumeric means `a-zA-Z0-9-`: any character not in this range
          will be replaced with a `-`.
    * Pull requests have `pr` prefix, followed by `gh`+ PR number.
        * Example: `tvOS 15.1.123-ci.pr.gh3333`
    * If we have a particular feature we want people to subscribe to (such as
      an Xcode release), we publish previews with a custom pre-release
      identifier:
        * Example: `iOS 15.1.123-xcode13-1.beta`
        * This way people can sign up for only official previews, by
          referencing `iOS *-xcode13-1.beta.*`
        * It's still possible to sign up for all `xcode13-1` builds, by
          referencing `iOS *-ci.xcode11-3.*`
    * Long versions are sometimes problematic on Windows, because of MAX_PATH
      issues. Versions for release builds don't contain the pre-release part,
      and are thus usually short, but pre-release versions (which include an
      arbitrarily long branch name) can get too long for Windows. This is a
      complication when testing a release pipeline/process: we have to use
      final versioning just for testing. This isn't ideal, so we special-case
      branch names that start with `release-test/rt/`:
        * Example: `iOS 15.1.123-rt` (and nothing else). This makes these
          versions exactly 3 characters longer than the release version, which
          is hopefully enough to avoid MAX_PATH issues on Windows.
* Build metadata: Required Hash
    * This is `sha.` + the short commit hash.
        * Use the short hash because the long hash is quite long and
          cumbersome. This leaves the complete version open for duplication,
          but this is extremely unlikely.
    * Example: `iOS 14.0.123+sha.1a2b3c`
    * Example (CI build): `iOS 15.0.123-ci.main+sha.1a2b3c`
    * Since the build metadata is required for all builds, we're able to
      recognize incomplete version numbers and determine if a particular
      version string refers to a stable version or not.
        * Example: `iOS 15.0.123`: incomplete version
        * Example: `iOS 15.0.123+sha.1a2b3c`: stable
        * Example: `iOS 15.0.123-ci.d17-0+sha.1a2b3c`: CI build
        * Example: `iOS 15.0.123-xcode13-1.beta+sha.1a2b3c`: official
          preview
            * Technically it's possible to remove the prerelease part, but
              we’d still be able to figure out it’s not a stable version by
              using the commit hash.

[1]: https://github.com/dotnet/designs/blob/master/accepted/2018/sdk-version-scheme.md
[2]: https://semver.org
