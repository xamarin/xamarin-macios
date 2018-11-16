TOP=../..

include $(TOP)/Make.config

all-local:: run-unit-tests

build-unit-tests:
	$(Q) $(SYSTEM_MONO) /Library/Frameworks//Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore $(TOP)/src/generator.sln
	$(SYSTEM_XIBUILD) -- generator-tests.csproj $(XBUILD_VERBOSITY)

run-unit-tests: build-unit-tests
	rm -f .failed-stamp
	$(SYSTEM_MONO) --debug $(XIBUILD_EXE_PATH) -t -- $(TOP)/packages/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe $(abspath $(TOP)/tests/generator/bin/Debug/generator-tests.dll) "--result=$(abspath $(CURDIR)/TestResult.xml);format=nunit2" $(TEST_FIXTURE) --labels=All || touch $(CURDIR)/.failed-stamp
	@# Create an html file and tell MonkeyWrench to upload it (if we're running there)
	@[[ -z "$$BUILD_REPOSITORY" ]] || \
		( xsltproc $(TOP)/tests/HtmlTransform.xslt TestResult.xml  > index.html && \
		echo "@MonkeyWrench: AddFile: $$PWD/index.html")
	@[[ ! -e .failed-stamp ]] 

clean-local::
	rm -Rf bin
