mlaunch
-------

mlaunch is a closed source tool used to interact with simulators and devices.

We'll build this tool from souce if the xamarin build is enabled (configured
with --enable-xamarin).

Otherwise we'll get a binary version from the macios-binaries repository. This
version may be somewhat out of date, but should work at least for running
tests.

To update the binary version of mlaunch in macios-binaries execute `make publish`
in this directory (when the xamarin build is enabled, so that mlaunch
is built from source).
