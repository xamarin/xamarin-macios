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
#     make run TEST_FIXTURE="--filter Xamarin.MMP.Tests.MMPTests.AOT_SmokeTest"
# or:
#     make run XM_TEST_NAME=Xamarin.MMP.Tests.MMPTests.AOT_SmokeTest
# to run multiple tests:
#     make run TEST_FIXTURE="--filter Xamarin.MMP.Tests.MMPTests.AOT_32Bit_SmokeTest --filter Xamarin.MMP.Tests.MMPTests.AOT_SmokeTest"
#

ifeq ($(TEST_FIXTURE),)
ifneq ($(XM_TEST_NAME),)
TEST_FIXTURE=--filter $(XM_TEST_NAME)
endif
endif

run: build
	$(Q) $(DOTNET) test *.csproj --results-directory:. "--logger:console;verbosity=detailed" "--logger:trx;LogFileName=$(CURDIR)/TestResult.trx" "--logger:html;LogFileName=$(CURDIR)/TestResult.html" $(TEST_FIXTURE)

build:
	$(Q) $(DOTNET) build *.csproj $(XBUILD_VERBOSITY)

clean-local::
	@rm -rf ./obj
	@rm -rf ./bin
