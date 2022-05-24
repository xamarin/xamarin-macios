TOP=../..

include $(TOP)/Make.config

all-local::

build:
	$(SYSTEM_MSBUILD) /r

run-tests: build
	$(TOP)/tools/nunit3-console-3.11.1 $(abspath $(TOP)/tests/cecil-tests/bin/Debug/net472/cecil-tests.dll) $(TEST_NAME) -labels=After

clean:
	rm -rf bin/ obj/ TestResult.xml

all: build
