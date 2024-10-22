TOP=../..

include $(TOP)/Make.config

export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export MSBUILD_EXE_PATH=$(MONO_PREFIX)/lib/mono/msbuild/15.0/bin/MSBuild.dll
export TargetFrameworkFallbackSearchPaths=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
export XAMMAC_FRAMEWORK_PATH=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

all-local:: run-unit-tests

build-unit-tests:
	$(Q_XBUILD) $(SYSTEM_MSBUILD) -t -- /Library/Frameworks//Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore $(TOP)/src/generator.sln
	$(SYSTEM_MSBUILD) generator-tests.csproj $(XBUILD_VERBOSITY)

run-tests run-unit-tests: build-unit-tests
	rm -f .failed-stamp
	$(TOP)/tools/nunit3-console-3.11.1 $(abspath $(TOP)/tests/generator/bin/Debug/generator-tests.dll) "--result=$(abspath $(CURDIR)/TestResult.xml);format=nunit2" $(TEST_FIXTURE) --labels=After || touch $(CURDIR)/.failed-stamp
	@# Create an html file and tell MonkeyWrench to upload it (if we're running there)
	@[[ -z "$$BUILD_REPOSITORY" ]] || \
		( xsltproc $(TOP)/tests/HtmlTransform.xslt TestResult.xml  > index.html && \
		echo "@MonkeyWrench: AddFile: $$PWD/index.html")
	@[[ ! -e .failed-stamp ]] 

clean-local::
	rm -Rf bin
