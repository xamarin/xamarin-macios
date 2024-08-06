xcsync
-------

xcsync is a closed source tool used to interact with Xcode.

We'll build this tool from souce if the xamarin build is enabled (configured
with --enable-xamarin).

Otherwise we'll get a binary version from the macios-binaries repository. This
version may be somewhat out of date, but should work at least for running
tests.

To update the binary version of xcsync in macios-binaries execute `make publish`
in this directory (when the xamarin build is enabled, so that xcsync
is built from source).
