<!-- Copyright (c) Microsoft Corporation. -->
<!-- Licensed under the MIT License. -->
# run-audio-unit-extension-tests

Runs the `monotouch-test` audio-unit app extension from the command line.

It handles:

* registering the host app and `.appex`,
* wiring the `test.name` and `log.file` NSUserDefaults used by `TouchOptions`,
* launching the container host,
* tracking the spawned AppExtension PID so the run can be timed out cleanly, and
* validating that the extension reached real NUnit execution.
