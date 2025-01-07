# run-with-timeout

This script runs an external process with a few extra features:

* The process will be killed if it takes longer than the specified timeout.
* The process will be killed if it doesn't launch within 10 seconds. This is
  implemented by passing the environment variable `LAUNCH_SENTINEL_FILE` (with
  a path to a file) to the process, which has 10 seconds to create said file.
* If the launch timed out, the process will be relaunched, up to a maximum of
  10 attempts.

The intented use case for this tool is to run tests, but with a timeout if the
tests hang. Also re-launch automatically if the test run doesn't successfully
start (this seems to happen randomly for Mac Catalyst sometimes).

Syntax:

```shell
run-with-timeout timeoutInSeconds command cmdarg1 cmdarg2 ...
```
