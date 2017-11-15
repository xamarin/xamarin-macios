TOP=../..

include $(TOP)/Make.config

export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

#
# * Use [IOS|MAC]_TESTS for simple tests that just verify the code can compile / generate.
#   To add such a test, just add the file, and add the name of the file (without extension) to [IOS|MAC]_TESTS.
#
# * Use [IOS|MAC]_CUSTOM_TESTS for more advanced tests that needs more validation.
#   In this case you additionally need to add a target for the name of the test, with the 
#   your test logic.
#

all-local:: run-unit-tests

build-unit-tests:
	$(Q) $(SYSTEM_MONO) /Library/Frameworks//Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore $(TOP)/src/generator.sln
	$(SYSTEM_MSBUILD) generator-tests.csproj $(XBUILD_VERBOSITY)

run-unit-tests: build-unit-tests
	rm -f .failed-stamp
	$(SYSTEM_MONO) --debug $(TOP)/packages/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe $(abspath $(TOP)/tests/generator/bin/Debug/generator-tests.dll) "--result=$(abspath $(CURDIR)/TestResult.xml);format=nunit2" $(TEST_FIXTURE) --labels=All || touch $(CURDIR)/.failed-stamp
	@# Create an html file and tell MonkeyWrench to upload it (if we're running there)
	@[[ -z "$$BUILD_REPOSITORY" ]] || \
		( xsltproc $(TOP)/tests/HtmlTransform.xslt TestResult.xml  > index.html && \
		echo "@MonkeyWrench: AddFile: $$PWD/index.html")
	@[[ ! -e .failed-stamp ]] 

clean-local::
	rm -f *.dll *.source
	rm -Rf *.tmpdir
