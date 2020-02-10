TOP=../..

include $(TOP)/Make.config

all-local::

build:
	nuget restore
	$(SYSTEM_MSBUILD)

run-tests: build
	$(SYSTEM_MONO) --debug $(TOP)/packages/NUnit.ConsoleRunner.3.9.0/tools/nunit3-console.exe $(abspath $(TOP)/tests/cecil-tests/bin/Debug/cecil-tests.dll)

clean:
	rm -rf bin/ obj/ TestResult.xml

all: build
