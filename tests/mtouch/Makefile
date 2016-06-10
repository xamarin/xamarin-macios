TOP=../..

include $(TOP)/Make.config

export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export XBUILD_FRAMEWORK_FOLDERS_PATH=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPath=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild

check: run-tests

NUNIT_MSBUILD_DIR=$(TOP)/tests/packages/NUnit.Runners.2.6.4/tools/lib

all-local::
	$(MAKE) dependencies
	$(MAKE) run-tests

run-tests: test.config
	rm -f .failed-stamp
	cd $(NUNIT_MSBUILD_DIR) && $(SYSTEM_MONO) ../nunit-console.exe $(abspath $(TOP)/tests/mtouch/bin/Debug/mtouch.dll) -xml=$(abspath $(CURDIR)/TestResult.xml) -noshadow $(TEST_FIXTURE) -labels -nodots || touch $(CURDIR)/.failed-stamp
	@# Create an html file and tell MonkeyWrench to upload it (if we're running there)
	@[[ -z "$$BUILD_REPOSITORY" ]] || \
		( xsltproc $(TOP)/tests/HtmlTransform.xslt TestResult.xml  > index.html && \
		echo "@MonkeyWrench: AddFile: $$PWD/index.html")
	@[[ ! -e .failed-stamp ]] 

build:
	$(Q) $(SYSTEM_MONO) /Library/Frameworks//Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore $(TOP)/tests/tests.sln
	$(SYSTEM_XBUILD) mtouch.csproj $(XBUILD_VERBOSITY)
	$(Q) rm -f .failed-stamp

mt9000: build
	/Library/Frameworks/Mono.framework/Commands/nunit-console4 -labels bin/Debug/*.dll -noshadow -fixture=MTouchTests.MTouch -run MTouchTests.MTouch.MT9008 || touch .failed-stamp

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
