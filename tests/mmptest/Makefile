TOP = ../..
include $(TOP)/Make.config

export TargetFrameworkFallbackSearchPaths=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
export XAMMAC_FRAMEWORK_PATH=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

all-local:: regression run

regression::
ifneq ($(SKIP_REGRESSION), 1)
	$(MAKE) -C ../mmp-regression/
endif

# example command to run a single test:
#     make run TEST_FIXTURE=-test=Xamarin.MMP.Tests.MMPTests.AOT_SmokeTest
# or:
#     make run XM_TEST_NAME=Xamarin.MMP.Tests.MMPTests.AOT_SmokeTest
# to run multiple tests:
#     make run TEST_FIXTURE="-test=Xamarin.MMP.Tests.MMPTests.AOT_32Bit_SmokeTest,Xamarin.MMP.Tests.MMPTests.AOT_SmokeTest"
#

ifeq ($(TEST_FIXTURE),)
ifneq ($(XM_TEST_NAME),)
TEST_FIXTURE=-test=$(XM_TEST_NAME)
endif
endif

run: build
	rm -f .failed-stamp
	$(TOP)/tools/nunit3-console-3.11.1 $(abspath $(TOP)/tests/mmptest/bin/Debug/mmptest.dll) "--result=$(abspath $(CURDIR)/TestResult.xml);format=nunit2" $(TEST_FIXTURE) --labels=After || touch $(CURDIR)/.failed-stamp
	@# Create an html file and tell MonkeyWrench to upload it (if we're running there)
	@[[ -z "$$BUILD_REPOSITORY" ]] || \
		( xsltproc $(TOP)/tests/HtmlTransform.xslt TestResult.xml  > index.html && \
		echo "@MonkeyWrench: AddFile: $$PWD/index.html")
	@[[ ! -e .failed-stamp ]]

build:
	$(Q_XBUILD) $(SYSTEM_XIBUILD) -t -- /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore ../tests-mac.sln
	$(SYSTEM_XIBUILD) -- mmptest.csproj $(XBUILD_VERBOSITY)

clean-local::
	@rm -rf ./obj
	@rm -rf ./bin
