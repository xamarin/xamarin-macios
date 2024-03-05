TOP=../..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export TargetFrameworkFallbackSearchPaths=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild

check: run-tests

all-local::
	$(MAKE) dependencies
	$(MAKE) run-tests

# example command to run a single test:
#     make run-tests TEST_FIXTURE=-test=Xamarin.MTouch.MT1016
# or multiple tests:
#     make run-tests TEST_FIXTURE="-test=Xamarin.MTouch.MT1016,Xamarin.MTouch.MT1017"

run-tests run-unit-tests: bin/Debug/mtouchtests.dll test.config
	rm -f .failed-stamp
	$(TOP)/tools/nunit3-console-3.11.1 "$(abspath $<)" "--result=$(abspath $(CURDIR)/TestResult.xml);format=nunit2" $(TEST_FIXTURE) --labels=After --inprocess || touch $(CURDIR)/.failed-stamp
	@# Create an html file and tell MonkeyWrench to upload it (if we're running there)
	@[[ -z "$$BUILD_REPOSITORY" ]] || \
		( xsltproc $(TOP)/tests/HtmlTransform.xslt TestResult.xml  > index.html && \
		echo "@MonkeyWrench: AddFile: $$PWD/index.html")
	@[[ ! -e .failed-stamp ]] 

# mtouchtests.csproj.inc contains the mtouchtests_dependencies variable used to determine if mtouchtests.dll needs to be rebuilt or not.
mtouchtests.csproj.inc: export BUILD_VERBOSITY=$(MSBUILD_VERBOSITY)
-include mtouchtests.csproj.inc

bin/Debug/mtouchtests.dll: $(mtouchtests_dependencies)
	$(SYSTEM_XIBUILD) -- mtouchtests.csproj /r $(XBUILD_VERBOSITY) /bl
	$(Q) rm -f .failed-stamp

$(abspath $(TOP)/tools/mtouch/Constants.cs):
	$(Q) $(MAKE) $(notdir $@) -C $(dir $@)

$(TOP)/tools/common/ProductConstants.cs:
	$(Q) $(MAKE) -C $(TOP)/tools/mtouch ../common/ProductConstants.cs

build: bin/Debug/mtouchtests.dll

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
