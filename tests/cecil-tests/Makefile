TOP=../..

include $(TOP)/Make.config

all-local::

build:
	$(Q) $(DOTNET) build $(DOTNET_BUILD_VERBOSITY) /bl

run-tests run-unit-tests: build
	$(DOTNET) test $(abspath $(TOP)/tests/cecil-tests/bin/Debug/$(DOTNET_TFM)/cecil-tests.dll) $(TEST_NAME) '--logger:console;verbosity=detailed'

clean:
	rm -rf bin/ obj/ TestResult.xml

all: build
