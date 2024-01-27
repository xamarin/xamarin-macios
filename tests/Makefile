TOP = ..
SUBDIRS=test-libraries

# disabled for now: mac-test

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

ifdef ENABLE_DOTNET
SUBDIRS += dotnet
endif

MTOUCH=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/mtouch

export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export MSBUILD_EXE_PATH=$(MONO_PREFIX)/lib/mono/msbuild/15.0/bin/MSBuild.dll
export TargetFrameworkFallbackSearchPaths=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
export XAMMAC_FRAMEWORK_PATH=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

ifneq ($(RELEASE),)
CONFIG=Release
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
	$(MAKE) mac-test-package.zip

test.config: Makefile $(TOP)/Make.config $(TOP)/mk/mono.mk $(TOP)/eng/Version.Details.xml
	@rm -f $@
	@echo "MONOTOUCH_PREFIX=$(abspath $(MONOTOUCH_PREFIX))" >> $@
	@echo "IOS_DESTDIR=$(abspath $(IOS_DESTDIR))" >> $@
	@echo "MAC_DESTDIR=$(abspath $(MAC_DESTDIR))" >> $@
	@echo "JENKINS_RESULTS_DIRECTORY=$(abspath $(JENKINS_RESULTS_DIRECTORY))" >> $@
	@echo "INCLUDE_DEVICE=$(INCLUDE_DEVICE)" >> $@
	@echo "XCODE_DEVELOPER_ROOT=$(XCODE_DEVELOPER_ROOT)" >> $@
	@echo "MONO_IOS_SDK_DESTDIR=$(MONO_IOS_SDK_DESTDIR)" >> $@
	@echo "MONO_MAC_SDK_DESTDIR=$(MONO_MAC_SDK_DESTDIR)" >> $@
	@echo "DOTNET=$(DOTNET)" >> $@
	@echo "IOS_SDK_VERSION=$(IOS_SDK_VERSION)" >> $@
	@echo "TVOS_SDK_VERSION=$(TVOS_SDK_VERSION)" >> $@
	@echo "WATCH_SDK_VERSION=$(WATCH_SDK_VERSION)" >> $@
	@echo "MACOS_SDK_VERSION=$(MACOS_SDK_VERSION)" >> $@
	@echo "DOTNET_BCL_DIR=$(DOTNET_BCL_DIR)" >> $@
	@echo "ENABLE_DOTNET=$(ENABLE_DOTNET)" >> $@
	@echo "INCLUDE_XAMARIN_LEGACY=$(INCLUDE_XAMARIN_LEGACY)" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),DOTNET_$(platform)_RUNTIME_IDENTIFIERS='$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS)'\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),DOTNET_$(rid)_ARCHITECTURES='$(DOTNET_$(rid)_ARCHITECTURES)'\\n))" | sed 's/^ //' >> $@
	@echo "DOTNET_CSC_COMMAND='$(DOTNET_CSC)'" >> $@
	@echo "DOTNET_TFM=$(DOTNET_TFM)" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_VERSION_NO_METADATA=$($(platform)_NUGET_VERSION_NO_METADATA)\\n)" | sed 's/^ //' >> $@
	@echo "SYSTEM_MONO=$(SYSTEM_MONO)" >> $@
	@echo "DOTNET_DIR=$(DOTNET_DIR)" >> $@
	@echo "INCLUDE_MAC=$(INCLUDE_MAC)" >> $@
	@echo "INCLUDE_MACCATALYST=$(INCLUDE_MACCATALYST)" >> $@
	@echo "IOS_SUPPORTS_32BIT_ARCHITECTURES=$(IOS_SUPPORTS_32BIT_ARCHITECTURES)" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_SDK_NAME=$($(platform)_NUGET_SDK_NAME)\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_REF_NAME=$($(platform)_NUGET_REF_NAME)\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(rid)_NUGET_RUNTIME_NAME=$($(rid)_NUGET_RUNTIME_NAME)\\n))" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),SUPPORTED_API_VERSIONS_$(platform)='$(SUPPORTED_API_VERSIONS_$(platform))'\\n)" | sed 's/^ //' >> $@

test-system.config: Makefile $(TOP)/Make.config $(TOP)/mk/mono.mk $(TOP)/eng/Version.Details.xml
	@rm -f $@
	@echo "MONOTOUCH_PREFIX=$(IOS_FRAMEWORK_DIR)/Versions/Current" >> $@
	@echo "IOS_DESTDIR=/" >> $@
	@echo "MAC_DESTDIR=/" >> $@
	@echo "JENKINS_RESULTS_DIRECTORY=$(abspath $(JENKINS_RESULTS_DIRECTORY))" >> $@
	@echo "INCLUDE_DEVICE=$(INCLUDE_DEVICE)" >> $@
	@echo "MONO_IOS_SDK_DESTDIR=$(MONO_IOS_SDK_DESTDIR)" >> $@
	@echo "MONO_MAC_SDK_DESTDIR=$(MONO_MAC_SDK_DESTDIR)" >> $@
	@echo "DOTNET=$(DOTNET)" >> $@
	@echo "IOS_SDK_VERSION=$(IOS_SDK_VERSION)" >> $@
	@echo "TVOS_SDK_VERSION=$(TVOS_SDK_VERSION)" >> $@
	@echo "WATCH_SDK_VERSION=$(WATCH_SDK_VERSION)" >> $@
	@echo "MACOS_SDK_VERSION=$(MACOS_SDK_VERSION)" >> $@
	@echo "DOTNET_TFM=$(DOTNET_TFM)" >> $@
	@echo "DOTNET_BCL_DIR=$(DOTNET_BCL_DIR)" >> $@
	@echo "ENABLE_DOTNET=$(ENABLE_DOTNET)" >> $@
	@echo "INCLUDE_XAMARIN_LEGACY=$(INCLUDE_XAMARIN_LEGACY)" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),DOTNET_$(platform)_RUNTIME_IDENTIFIERS='$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS)'\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),DOTNET_$(rid)_ARCHITECTURES='$(DOTNET_$(rid)_ARCHITECTURES)'\\n))" | sed 's/^ //' >> $@
	@echo "DOTNET_CSC_COMMAND='$(DOTNET_CSC)'" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_VERSION_NO_METADATA=$($(platform)_NUGET_VERSION_NO_METADATA)\\n)" | sed 's/^ //' >> $@
	@echo "SYSTEM_MONO=$(SYSTEM_MONO)" >> $@
	@echo "DOTNET_DIR=$(DOTNET_DIR)" >> $@
	@echo "INCLUDE_MAC=$(INCLUDE_MAC)" >> $@
	@echo "INCLUDE_MACCATALYST=$(INCLUDE_MACCATALYST)" >> $@
	@echo "IOS_SUPPORTS_32BIT_ARCHITECTURES=$(IOS_SUPPORTS_32BIT_ARCHITECTURES)" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_SDK_NAME=$($(platform)_NUGET_SDK_NAME)\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_REF_NAME=$($(platform)_NUGET_REF_NAME)\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(rid)_NUGET_RUNTIME_NAME=$($(rid)_NUGET_RUNTIME_NAME)\\n))" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),SUPPORTED_API_VERSIONS_$(platform)='$(SUPPORTED_API_VERSIONS_$(platform))'\\n)" | sed 's/^ //' >> $@

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

build-test-libraries:
	@$(MAKE) -C $(TOP)/tests/test-libraries

.stamp-xharness-configure: xharness/xharness.exe
	$(Q_GEN) $(SYSTEM_MONO) --debug $(CURDIR)/$< $(XHARNESS_VERBOSITY) --configure --autoconf --rootdir $(CURDIR)
	$(Q) touch $@

all-local:: .stamp-xharness-configure

$(TOP)/tools/common/SdkVersions.cs: $(TOP)/tools/common/SdkVersions.in.cs
	@$(MAKE) -C $(TOP)/tools/mtouch ../common/SdkVersions.cs

.stamp-src-project-files:
	@$(MAKE) -C $(TOP)/src project-files
	@touch $@

xharness/xharness.exe: $(xharness_dependencies) test.config test-system.config .stamp-src-project-files $(TOP)/tools/common/SdkVersions.cs
	$(Q_GEN) $(SYSTEM_MSBUILD) "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY_QUIET) xharness/xharness.csproj
xharness/xharness.csproj.inc: export BUILD_VERBOSITY=$(XBUILD_VERBOSITY)
xharness/xharness.csproj.inc: export ABSOLUTE_PATHS=1
-include xharness/xharness.csproj.inc

killall:
	@killall "iPhone Simulator" >/dev/null 2>&1 || true
	@killall "iOS Simulator" >/dev/null 2>&1 || true
	@killall Touch.Server >/dev/null 2>&1 || true

NUNIT_MSBUILD_DIR=$(TOP)/packages/NUnit.Runners.2.6.4/tools/lib
test-ios-tasks: test-macdev-tests test-macdev-tasks

test-macdev-tests: verify-system-vsmac-xcode-match
	$(SYSTEM_MSBUILD) "/bl:$@.binlog" $(TOP)/tests/msbuild/Xamarin.MacDev.Tests/Xamarin.MacDev.Tests.csproj /p:Configuration=Debug /r $(MSBUILD_VERBOSITY)
	cd $(TOP)/tests/msbuild/Xamarin.MacDev.Tests && $(SYSTEM_XIBUILD) -t -- $(abspath $(TOP)/tools/nunit3-console-3.11.1) $(abspath $(TOP)/tests/msbuild/Xamarin.MacDev.Tests/bin/Debug/net472/Xamarin.MacDev.Tests.dll) "--result=$(abspath $(CURDIR)/TestResults_Xamarin.MacDev.Tests.xml);format=nunit2" -labels=After $(TEST_FIXTURE)

test-macdev-tasks: verify-system-vsmac-xcode-match
	$(SYSTEM_MSBUILD) "/bl:$@.binlog" $(TOP)/tests/msbuild/Xamarin.MacDev.Tasks.Tests/Xamarin.MacDev.Tasks.Tests.csproj /p:Configuration=Debug /r  $(MSBUILD_VERBOSITY)
	cd $(TOP)/tests/msbuild/Xamarin.MacDev.Tasks.Tests && $(SYSTEM_XIBUILD) -t -- $(abspath $(TOP)/tools/nunit3-console-3.11.1) $(abspath $(TOP)/tests/msbuild/Xamarin.MacDev.Tasks.Tests/bin/Debug/net472/Xamarin.MacDev.Tasks.Tests.dll) "--result=$(abspath $(CURDIR)/TestResults_Xamarin.MacDev.Tasks.Tests.xml)" -labels=After $(TEST_FIXTURE)

test-install-sources:
	$(SYSTEM_XIBUILD) -- $(TOP)/tools/install-source/InstallSourcesTests/InstallSourcesTests.csproj
	cd $(NUNIT_MSBUILD_DIR) && $(SYSTEM_XIBUILD) -t -- ../nunit-console.exe ../../../../tools/install-source/InstallSourcesTests/bin/Release/InstallSourcesTests.dll -xml=TestResults_InstallSourcesTests.xml -labels $(TEST_FIXTURE) || touch .failed-stamp
	@[[ -z "$$BUILD_REPOSITORY" ]] || ( xsltproc $(TOP)/tests/HtmlTransform.xslt $(NUNIT_MSBUILD_DIR)/TestResults_InstallSourcesTests.xml > $(TOP)/tests/index.html && echo "@MonkeyWrench: AddFile: $$PWD/index.html" )
	@if test -e $(NUNIT_MSBUILD_DIR)/.failed-stamp; then rm $(NUNIT_MSBUILD_DIR)/.failed-stamp; exit 1; fi

mac-test-package.zip:
ifdef INCLUDE_MAC
	./package-mac-tests.sh
else ifdef INCLUDE_MACCATALYST
	./package-mac-tests.sh
else
	@echo Not enabled
endif

ifdef INCLUDE_DOTNET
dotnet-test-package.7z:
	./package-dotnet-tests.sh
else
dotnet-test-package.7z:
	@echo Not enabled
endif

#XI
ifdef INCLUDE_IOS
wrench-mtouch:
	@echo Not here anymore

wrench-btouch:
	@echo Not here anymore

wrench-build-unit-test-dev:
	@echo Not here anymore

wrench-sim-linksdk:
	@echo Not here anymore

wrench-sim-linkall:
	@echo Not here anymore

wrench-sim-dontlink:
	@echo Not here anymore

else
wrench-mtouch wrench-btouch wrench-build-unit-test-dev:
	@echo "iOS tests have been disabled [$@]"
endif

## XM
ifdef INCLUDE_MAC
wrench-mac-dontlink:
	@echo Not here anymore

wrench-mac-mmp:
	@echo Now part of wrench-mac-mmptest

wrench-mac-misc:
	@echo Tests mac-misc tests removed

wrench-mac-apidiff:
	@echo This is done in the normal api-diff target now.

wrench-mac-drm:
	@echo "The DRM tests have all been removed."

wrench-mac-api:
	@echo Not here anymore

wrench-mac-mmptest:
	@echo Not here anymore

wrench-mac-msbuild:
	@echo Not here anymore

wrench-mac-binding-project:
	@echo Not here anymore

wrench-mac-introspection:
	@echo Not here anymore

wrench-mac-xammac_tests:
	@echo Not here anymore
else
wrench-mac-%:
	@echo "Mac tests have been disabled [$@]"
endif

wrench-msbuild:
	@echo Not here anymore

wrench-docs:
	@echo "Docs have been disabled [$@]"

# the wrenchhelper-* targets are generated.
wrench-sim-%:
	@echo Not here anymore

wrench-bcl-sim-%:
	@echo Not here anymore

wrench-%:
	@echo Not here anymore

wrench-jenkins: xharness/xharness.exe
	$(Q) rm -f $@-failed.stamp
	$(Q) ulimit -n 4096 && $(SYSTEM_MONO) --trace=E:all --debug $(CURDIR)/$< $(XHARNESS_VERBOSITY) --jenkins --autoconf --rootdir $(CURDIR) --sdkroot $(XCODE_DEVELOPER_ROOT) --label run-all-tests,skip-device-tests --markdown-summary=$(abspath $(CURDIR))/TestSummary.md $(TESTS_PERIODIC_COMMAND) --use-system=true || echo "$$?" > $@-failed.stamp
	@echo "@MonkeyWrench: SetSummary: <br/>`cat $(abspath $(CURDIR))/TestSummary.md | awk 1 ORS='<br/>'`"
	@echo "@MonkeyWrench: AddFile: $(abspath $(CURDIR))/TestSummary.md"
	$(Q) if test -e $@-failed.stamp; then EC=`cat $@-failed.stamp`; rm -f $@-failed.stamp; exit $$EC; fi

wrench-xtro:
	@echo Not here anymore

jenkins: xharness/xharness.exe
	$(Q) $(SYSTEM_MONO) --trace=E:all,-E:System.Reflection.ReflectionTypeLoadException --debug $(CURDIR)/$< $(XHARNESS_VERBOSITY) --jenkins --autoconf --rootdir $(CURDIR) --sdkroot $(XCODE_DEVELOPER_ROOT) --markdown-summary=$(abspath $(CURDIR))/TestSummary.md --use-system=true $(TESTS_EXTRA_ARGUMENTS) $(TESTS_PERIODIC_COMMAND)

# This will launch xharness' interactive test runner in the system's default browser
runner: xharness/xharness.exe
	@echo "Running xharness in server mode. Press Ctrl-C to exit (or click Quit / press Q in the browser page)"
	$(Q) $(SYSTEM_MONO) $(CURDIR)/xharness/xharness.exe $(XHARNESS_VERBOSITY) --jenkins:server --autoconf --rootdir $(CURDIR) --sdkroot $(XCODE_DEVELOPER_ROOT)

# This makefile target will run the device tests using the Xamarin.iOS version
# installed on the system.
vsts-device-tests: xharness/xharness.exe
	$(MAKE) -C $(TOP)/builds .stamp-mono-ios-sdk-destdir download -j
	$(Q) ulimit -n 4096 && $(SYSTEM_MONO) --debug $(CURDIR)/$< $(XHARNESS_VERBOSITY) --jenkins --autoconf --rootdir $(CURDIR) --sdkroot $(XCODE_DEVELOPER_ROOT) --use-system:true --label=skip-all-tests,run-device-tests --markdown-summary=$(CURDIR)/TestSummary.md $(TESTS_EXTRA_ARGUMENTS) $(TESTS_PERIODIC_COMMAND)

verify-system-vsmac-xcode-match:
	@SYSTEM_XCODE=$$(dirname $$(dirname $$(xcode-select -p))); \
	VSMAC_XCODE=$$(/usr/libexec/PlistBuddy -c 'print :AppleSdkRoot' ~/Library/Preferences/Xamarin/Settings.plist); \
	if [[ "$$SYSTEM_XCODE" != "$$VSMAC_XCODE" ]]; then echo "Error: the system's Xcode ($$SYSTEM_XCODE) does not match the Xcode selected in Visual Studio for Mac ($$VSMAC_XCODE). This will make tests fail in non-trivial ways, so please make sure both match the expected Xcode ($(shell dirname $(shell dirname $(XCODE_DEVELOPER_ROOT))))."; exit 1; fi

package-test-libraries.zip:
	$(Q) $(MAKE) all -C test-libraries
	$(Q) $(MAKE) all -C test-libraries/custom-type-assembly build-assembly
	$(Q_GEN) rm -f "$@" "$@.tmp"
	$(Q_GEN) cd $(TOP) && zip -9r --symlinks $(abspath $@).tmp ./tests/test-libraries
	$(Q_GEN) cd $(TOP) && zip -9r --symlinks $(abspath $@).tmp ./tests/mono-native/macOS/unified ./tests/mono-native/macOS/compat
	$(Q_GEN) cd $(TOP) && zip -9r --symlinks $(abspath $@).tmp ./tests/mono-native/iOS/unified ./tests/mono-native/iOS/compat
	$(Q_GEN) cd $(TOP) && zip -9r --symlinks $(abspath $@).tmp ./tests/bcl-test/generated ./tests/bcl-test/templates
	$(Q_GEN) cd $(TOP) && find tests -regex '.*/generated-projects/.*[c|f]sproj' -exec zip -9r --symlinks $(abspath $@).tmp {} +
	$(Q_GEN) cd $(TOP) && find tests -regex '.*/generated-projects/.*sln' -exec zip -9r --symlinks $(abspath $@).tmp {} +
	$(Q_GEN) cd $(TOP) && find tests -regex 'tests/test-libraries/custom-type-assembly/.libs/.*dll' -exec zip -9r --symlinks $(abspath $@).tmp {} +
	$(Q_GEN) cd $(TOP) && git ls-files -o -- 'tests/*/Info-*plist' | zip -9r --symlinks $(abspath $@).tmp -@
ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_IOS
	$(Q_GEN) cd $(TOP) && zip -9r --symlinks $(abspath $@).tmp ./tools/mtouch/simlauncher*-sgen*
endif
endif
	$(Q) mv "$@".tmp "$@"
