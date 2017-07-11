TOP = ..

ifdef INCLUDE_MAC
SUBDIRS += no-mmp
endif
# disabled for now: mac-test

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

MTOUCH=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/mtouch
UNIT_SERVER_DIR=$(TOUCH_UNIT_PATH)/Touch.Server
UNIT_SERVER=$(UNIT_SERVER_DIR)/bin/Debug/Touch.Server.exe
TEST_SUITES=monotouch-test link\ sdk link\ all dont\ link framework-test mini
BCL_TEST_SUITES=mscorlib System System.Core System.Data System.Net.Http System.Numerics System.Runtime.Serialization System.Transactions System.Web.Services System.Xml System.Xml.Linq Mono.Security System.ComponentModel.DataAnnotations System.Json System.ServiceModel.Web Mono.Data.Sqlite
ALL_TEST_SUITES=$(TEST_SUITES) $(BCL_TEST_SUITES)
EXEC_UNIT_SERVER=XCODE_DEVELOPER_ROOT=$(XCODE_DEVELOPER_ROOT) MONOTOUCH_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(SYSTEM_MONO) --debug $(UNIT_SERVER)

export MD_APPLE_SDK_ROOT=$(abspath $(XCODE_DEVELOPER_ROOT)/../..)
export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export XBUILD_FRAMEWORK_FOLDERS_PATH=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPath=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
export XAMMAC_FRAMEWORK_PATH=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

ifneq ($(RELEASE),)
ifneq ($(BITCODE),)
CONFIG=Release-bitcode
else
CONFIG=Release
endif
else
CONFIG=Debug
endif

ifeq ($(V),)
ifeq ($(BUILD_REVISION),)
# non-verbose local build
XHARNESS_VERBOSITY=
else
# wrench build
XHARNESS_VERBOSITY=--verbose
endif
else
# verbose build
XHARNESS_VERBOSITY=--verbose
endif

#
# To run all the tests, just do:
#
#    make run-tests
#
# and go take a long tea break.
#
# For faster turnaround there are specific targets that can be used
# to run specific test suites. For more information read the README.md.
#

package-tests:
	$(MAKE) qa-test-dependencies.zip mac-test-package.zip

test.config: Makefile $(TOP)/Make.config
	@rm -f $@
	@echo "MONOTOUCH_PREFIX=$(abspath $(MONOTOUCH_PREFIX))" >> $@
	@echo "IOS_DESTDIR=$(abspath $(IOS_DESTDIR))" >> $@
	@echo "MAC_DESTDIR=$(abspath $(MAC_DESTDIR))" >> $@
	@echo "WATCH_MONO_PATH=$(abspath $(WATCH_MONO_PATH))" >> $@
	@echo "JENKINS_RESULTS_DIRECTORY=$(abspath $(JENKINS_RESULTS_DIRECTORY))" >> $@
	@echo "INCLUDE_DEVICE=$(INCLUDE_DEVICE)" >> $@

test-system.config:
	@rm -f $@
	@echo "MONOTOUCH_PREFIX=$(IOS_FRAMEWORK_DIR)/Versions/Current" >> $@
	@echo "IOS_DESTDIR=/" >> $@
	@echo "MAC_DESTDIR=/" >> $@
	@echo "WATCH_MONO_PATH=$(abspath $(WATCH_MONO_PATH))" >> $@
	@echo "JENKINS_RESULTS_DIRECTORY=$(abspath $(JENKINS_RESULTS_DIRECTORY))" >> $@
	@echo "INCLUDE_DEVICE=$(INCLUDE_DEVICE)" >> $@

clean-local::
	$(Q) $(SYSTEM_XBUILD) /t:Clean /p:Platform=iPhoneSimulator /p:Configuration=$(CONFIG) $(XBUILD_VERBOSITY) tests.sln
	$(Q) $(SYSTEM_XBUILD) /t:Clean /p:Platform=iPhone /p:Configuration=$(CONFIG) $(XBUILD_VERBOSITY) tests.sln
	$(Q) $(SYSTEM_XBUILD) /t:Clean /p:Platform=iPhoneSimulator /p:Configuration=$(CONFIG) $(XBUILD_VERBOSITY) tests-unified.sln
	$(Q) $(SYSTEM_XBUILD) /t:Clean /p:Platform=iPhone /p:Configuration=$(CONFIG) $(XBUILD_VERBOSITY) tests-unified.sln
	$(Q) $(SYSTEM_XBUILD) /t:Clean /p:Platform=iPhoneSimulator /p:Configuration=$(CONFIG) $(XBUILD_VERBOSITY) tests-tvos.sln
	$(Q) $(SYSTEM_XBUILD) /t:Clean /p:Platform=iPhoneSimulator /p:Configuration=$(CONFIG) $(XBUILD_VERBOSITY) tests-tvos.sln
	$(Q) $(SYSTEM_XBUILD) /t:Clean /p:Platform=iPhone /p:Configuration=$(CONFIG) $(XBUILD_VERBOSITY) tests-watchos.sln
	$(Q) $(SYSTEM_XBUILD) /t:Clean /p:Platform=iPhone /p:Configuration=$(CONFIG) $(XBUILD_VERBOSITY) tests-watchos.sln

## run targets = build + [install] + exec

run run-all run-tests run-test:
	$(Q) for subdir in $(SUBDIRS); do \
		$(MAKE) -C $$subdir run || exit 1; \
	done
	$(Q) $(MAKE) run-local

# separate build-dev-* entries because some of them are build with debug other (llvm) with release
build-dev-bots:
	@echo "These tests are now in the mtouch test suite"

build-% run-% exec-% install-%:
	@echo ""
	@echo "\033[0;31mUnknown target: $@\033[0m"
	@echo ""
	@echo "Syntax is: <action>-<platform>-<where>-<what>"
	@echo "    Action: build, install (only for device), exec, run (= build + install + exec)."
	@echo "    Platform: ios, tvos, watchos (can be skipped, in which case all platforms are executed)"
	@echo "    Where: "
	@echo "        ios: sim = [simclassic + simunified = [sim32, sim64]]; dev = [devclassic + devunified]"
	@echo "        tvos/watchos: sim, dev"
	@echo "    What: The test project to run (BCL tests do not need to be treated specially anymore, just use test project's name)"
	@echo ""
	@echo "Example:"
	@echo ""
	@echo "    # this will run monotouch-test on an Apple TV device"
	@echo "    $ make run-tvos-dev-monotouch-test"
	@echo ""
	@echo "For more info read README.md."
	@echo ""
	@exit 1

check-result:
	@test -z "$(BUILD_REVISION)" || echo "@MonkeyWrench: AddFile: $(abspath $(FILE))"
	@if grep "Tests run" "$(FILE)"; then \
		if grep FAIL "$(FILE)"; then \
			test "x" == "x$(BUILD_REVISION)" -o "x" == "x$(TESTNAME)" || echo "@MonkeyWrench: AddSummary: <b>$(TESTNAME) failed: `grep "Tests run:" "$(FILE)" | sed 's/Tests run: //'`</b><br/>"; \
			echo Test run failed; \
			exit 1; \
		else \
			test "x" == "x$(BUILD_REVISION)" -o "x" == "x$(TESTNAME)" || echo "@MonkeyWrench: AddSummary: $(TESTNAME) succeeded: `grep "Tests run:" "$(FILE)" | sed 's/Tests run: //'`<br/>"; \
			echo Test run succeeded; \
		fi; \
	else \
		test "x" == "x$(BUILD_REVISION)" -o "x" == "x$(TESTNAME)" || echo "@MonkeyWrench: AddSummary: <b><i>$(TESTNAME) crashed</i></b><br/>"; \
		echo Test run crashed; \
		exit 1; \
	fi

logdev:
	$(MTOUCH) --logdev

$(UNIT_SERVER): $(wildcard $(UNIT_SERVER_DIR)/*.cs)
	(cd $(UNIT_SERVER_DIR) && $(SYSTEM_XBUILD))

build-test-libraries:
	@$(MAKE) -C $(TOP)/tests/test-libraries

Makefile.inc: xharness/xharness.exe
	$(Q_GEN) $(SYSTEM_MONO) --debug $< $(XHARNESS_VERBOSITY) --configure --autoconf --rootdir $(CURDIR)

-include Makefile.inc

$(GUI_UNIT_PATH)/bin/net_4_5/GuiUnit.exe:
	XBUILD_FRAMEWORK_FOLDERS_PATH=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks MSBuildExtensionsPath=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) $(GUI_UNIT_PATH)/src/framework/GuiUnit_NET_4_5.csproj

$(GUI_UNIT_PATH)/bin/xammac_mobile/GuiUnit.exe:
	XBUILD_FRAMEWORK_FOLDERS_PATH=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks MSBuildExtensionsPath=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) $(GUI_UNIT_PATH)/src/framework/GuiUnit_xammac_mobile.csproj

Makefile-mac.inc: xharness/xharness.exe
	$(Q_GEN) $(SYSTEM_MONO) --debug $< $(XHARNESS_VERBOSITY) --configure  --autoconf --mac --rootdir $(CURDIR)

-include Makefile-mac.inc

$(TOP)/tools/mtouch/SdkVersions.cs: $(TOP)/tools/mtouch/SdkVersions.cs.in
	@$(MAKE) -C $(TOP)/tools/mtouch SdkVersions.cs

.stamp-src-project-files:
	@$(MAKE) -C $(TOP)/src project-files
	@touch $@

xharness/xharness.exe: $(wildcard xharness/*.cs) xharness/xharness.csproj $(TOP)/tools/mtouch/SdkVersions.cs test.config test-system.config .stamp-src-project-files
	$(Q_GEN) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY_QUIET) xharness/xharness.csproj

killall:
	@killall "iPhone Simulator" >/dev/null 2>&1 || true
	@killall "iOS Simulator" >/dev/null 2>&1 || true
	@killall Touch.Server >/dev/null 2>&1 || true

NUNIT_MSBUILD_DIR=$(TOP)/packages/NUnit.Runners.2.6.4/tools/lib
test-ios-tasks:
	$(SYSTEM_XBUILD) $(TOP)/msbuild/Xamarin.MacDev.Tasks.sln
	cd $(NUNIT_MSBUILD_DIR) && $(SYSTEM_MONO) ../nunit-console.exe ../../../../msbuild/tests/bin/Xamarin.iOS.Tasks.Tests.dll -xml=TestResults_Xamarin.iOS.Tasks.Tests.xml -labels $(TEST_FIXTURE) || touch .failed-stamp
	@[[ -z "$$BUILD_REPOSITORY" ]] || ( xsltproc $(TOP)/tests/HtmlTransform.xslt $(NUNIT_MSBUILD_DIR)/TestResults_Xamarin.iOS.Tasks.Tests.xml > $(TOP)/tests/index.html && echo "@MonkeyWrench: AddFile: $$PWD/index.html" )
	@if test -e $(NUNIT_MSBUILD_DIR)/.failed-stamp; then rm $(NUNIT_MSBUILD_DIR)/.failed-stamp; exit 1; fi

ifdef ENABLE_XAMARIN
ifdef INCLUDE_IOS
qa-test-dependencies.zip:
	@$(MAKE) build-test-libraries
	@# Make sure we start from a clean slate
	$(Q) rm -rf $@ $@.tmpdir
ifdef INCLUDE_TVOS
	@# TVOS
	$(Q) mkdir -p $@.tmpdir/tvos
	$(Q) cp $(TOP)/src/build/tvos/reference/MonoTouch.Dialog-1.dll* $@.tmpdir/tvos
	$(Q) cp $(TOP)/src/build/tvos/reference/MonoTouch.NUnitLite.dll* $@.tmpdir/tvos
	$(Q) cp $(TOP)/tests/test-libraries/.libs/tvos/libtest.a $@.tmpdir/tvos
	$(Q) cp $(TOP)/tests/test-libraries/.libs/tvos/libtest.dylib $@.tmpdir/tvos
	$(Q) cp -a $(TOP)/tests/test-libraries/.libs/tvos/XTest.framework $@.tmpdir/tvos
endif
	@# iOS
	$(Q) mkdir -p $@.tmpdir/ios
	$(Q) cp $(TOP)/tests/test-libraries/.libs/ios/libtest.a $@.tmpdir/ios
	$(Q) cp $(TOP)/tests/test-libraries/.libs/ios/libtest.dylib $@.tmpdir/ios
	$(Q) cp -a $(TOP)/tests/test-libraries/.libs/ios/XTest.framework $@.tmpdir/ios
	$(Q) printf '#!/bin/bash -e\n\n/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mlaunch "$$@"\n' > $@.tmpdir/mlaunch
	$(Q) chmod +x $@.tmpdir/mlaunch
	@# Generate zip
	$(Q_GEN) cd $@.tmpdir && zip -9r $(abspath $@) .
	@# Cleanup
	$(Q) rm -rf $@.tmpdir
else
qa-test-dependencies.zip:
	@echo Not enabled
endif
else
qa-test-dependencies.zip:
	@echo Xamarin build not enabled
endif

ifdef INCLUDE_MAC
mac-test-package.zip:
	./package-mac-tests.sh
else
mac-test-package.zip:
	@echo Not enabled
endif

#XI
ifdef INCLUDE_IOS
wrench-mtouch:
	cd mtouch && $(MAKE)
	git clean -xfdq

wrench-btouch:
	cd generator && $(MAKE) 
	cd generator && git clean -xfdq

wrench-build-unit-test-dev:
	$(MAKE) build-dev-bots
	git clean -xfdq

wrench-sim-linksdk:
	$(Q) $(MAKE) wrench-sim-link\ sdk

wrench-sim-linkall:
	$(Q) $(MAKE) wrench-sim-link\ all

wrench-sim-dontlink:
	$(Q) $(MAKE) wrench-sim-dont\ link

else
wrench-mtouch wrench-btouch wrench-build-unit-test-dev:
	@echo "iOS tests have been disabled [$@]"
endif

## XM
ifdef INCLUDE_MAC
wrench-mac-dontlink:
	$(Q) $(MAKE) run-mac-dontlink
	$(Q) $(MAKE) clean-mac-dontlink
	git clean -xfdq

wrench-mac-mmp:
	@echo Now part of wrench-mac-mmptest

wrench-mac-misc:
	@echo Tests mac-misc tests removed

wrench-mac-apidiff:
	@echo This is done in the normal api-diff target now.

wrench-mac-drm:
	@echo "The DRM tests have all been removed."

wrench-mac-api:
	$(Q) $(MAKE) run-mac-apitest
	$(Q) $(MAKE) clean-mac-apitest
	git clean -xfdq

wrench-mac-mmptest:
	$(Q) $(MAKE) run-mac-mmptest
	$(Q) $(MAKE) clean-mac-mmptest
	$(Q) $(MAKE) -C mmptest/regression
	git clean -xfdq

wrench-mac-msbuild:
	$(Q) $(MAKE) run-mac-msbuild
	$(Q) $(MAKE) clean-mac-msbuild
	git clean -xfdq

wrench-mac-binding-project:
	cd mac-binding-project && $(MAKE)
	cd mac-binding-project && git clean -xfdq

wrench-mac-introspection:
	$(Q) $(MAKE) run-mac-introspection
	$(Q) $(MAKE) clean-mac-introspection
	git clean -xfdq

else
wrench-mac-%:
	@echo "Mac tests have been disabled [$@]"
endif

wrench-msbuild:
	$(MAKE) -C $(TOP)/msbuild .stamp-test-xml
ifdef INCLUDE_IOS
	$(MAKE) test-ios-tasks
else
	@echo "iOS tests have been disabled [$@]"
endif
	cd $(TOP)/msbuild && git clean -xfdq

wrench-docs:
ifdef ENABLE_XAMARIN
ifdef INCLUDE_IOS
	$(MAKE) -C $(MACCORE_PATH) update-docs
else
	@echo "iOS tests have been disabled [$@]"
endif
else
	@echo "Docs have been disabled [$@]"
endif

# the wrenchhelper-* targets are generated.
wrench-sim-%:
	git clean -xfdq
ifdef INCLUDE_IOS
	unset LD_LIBRARY_PATH && script -q /dev/null $(MAKE) "wrenchhelper-$*"
else
	@echo "iOS tests have been disabled [$@]"
endif	

wrench-bcl-sim-%:
	git clean -xfdq
ifdef INCLUDE_IOS
	unset LD_LIBRARY_PATH && script -q /dev/null $(MAKE) "wrenchhelper-$*"
else
	@echo "iOS tests have been disabled [$@]"
endif	

wrench-%:
	git clean -xfdq
ifdef INCLUDE_IOS
	touch "exec-$*.log"
	echo "@MonkeyWrench: AddFile: $(PWD)/exec-$*.log"
	unset LD_LIBRARY_PATH && script -q /dev/null $(MAKE) "run-$*"
	echo "Test succeeded"
else
	@echo "iOS tests have been disabled [$@]"
endif

jenkins: xharness/xharness.exe
	$(Q) $(SYSTEM_MONO) --debug $< $(XHARNESS_VERBOSITY) --jenkins --autoconf --rootdir $(CURDIR) --sdkroot $(XCODE_DEVELOPER_ROOT)

ifdef ENABLE_XAMARIN
wrench-launch-external wrench-report-external:
	$(MAKE) -C $(MACCORE_PATH)/tests/external $@
else
wrench-launch-external wrench-report-external:
	@echo "External tests have been disabled"
endif
