set -x
set -e

P=$(cat tmp.p)

if grep ios-64 xamarin-macios/tests/xharness/Jenkins.cs 2>&1 > /dev/null; then
   export TESTS_EXTRA_ARGUMENTS="--label=run-ios-64-tests"
else
   export TESTS_EXTRA_ARGUMENTS="--label=run-ios-tests"
fi

make -C xamarin-macios/builds download -j || true
make -C xamarin-macios/builds downloads -j || true
make -C xamarin-macios/builds .stamp-mono-ios-sdk-destdir -j || true
make -C xamarin-macios/tests vsts-device-tests
