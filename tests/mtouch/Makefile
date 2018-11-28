TOP=../..

include $(TOP)/Make.config

export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export TargetFrameworkFallbackSearchPaths=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild

check: run-tests

NUNIT_MSBUILD_DIR=$(TOP)/packages/NUnit.ConsoleRunner.3.5.0/tools/

all-local::
	$(MAKE) dependencies
	$(MAKE) run-tests

# example command to run a single test:
#     make run-tests TEST_FIXTURE=-test=Xamarin.MTouch.MT1016
# or multiple tests:
#     make run-tests TEST_FIXTURE="-test=Xamarin.MTouch.MT1016,Xamarin.MTouch.MT1017"

run-tests: test.config
	rm -f .failed-stamp
	$(SYSTEM_MONO) --debug $(TOP)/packages/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe $(abspath $(TOP)/tests/mtouch/bin/Debug/mtouch.dll) "--result=$(abspath $(CURDIR)/TestResult.xml);format=nunit2" $(TEST_FIXTURE) --labels=All || touch $(CURDIR)/.failed-stamp
	@# Create an html file and tell MonkeyWrench to upload it (if we're running there)
	@[[ -z "$$BUILD_REPOSITORY" ]] || \
		( xsltproc $(TOP)/tests/HtmlTransform.xslt TestResult.xml  > index.html && \
		echo "@MonkeyWrench: AddFile: $$PWD/index.html")
	@[[ ! -e .failed-stamp ]] 

build:
	$(Q) $(SYSTEM_MONO) /Library/Frameworks//Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore packages.config
	$(SYSTEM_XIBUILD) -- mtouch.csproj $(XBUILD_VERBOSITY)
	$(Q) rm -f .failed-stamp

test.config: $(TOP)/Make.config Makefile
	@rm -f $@
	@echo "MONOTOUCH_PREFIX=$(MONOTOUCH_PREFIX)" >> $@
	@echo "SDK_VERSION=$(IOS_SDK_VERSION)" >> $@
	@echo "XCODE_DEVELOPER_ROOT=$(XCODE_DEVELOPER_ROOT)" >> $@
	@echo "DESTDIR=$(IOS_DESTDIR)" >> $@
	@echo "IOS_DESTDIR=$(IOS_DESTDIR)" >> $@

dependencies:
	cd .. && $(MAKE)
	$(MAKE) -C $(TOP)/tests/test-libraries
	$(MAKE) build
