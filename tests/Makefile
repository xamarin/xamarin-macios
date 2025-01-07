TOP = ..
SUBDIRS=test-libraries dotnet

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

XHARNESS_EXECUTABLE=xharness/bin/Debug/xharness.dll

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
	@echo "JENKINS_RESULTS_DIRECTORY=$(abspath $(JENKINS_RESULTS_DIRECTORY))" >> $@
	@echo "INCLUDE_DEVICE=$(INCLUDE_DEVICE)" >> $@
	@echo "XCODE_DEVELOPER_ROOT=$(XCODE_DEVELOPER_ROOT)" >> $@
	@echo "DOTNET=$(DOTNET)" >> $@
	@echo "IOS_SDK_VERSION=$(IOS_SDK_VERSION)" >> $@
	@echo "TVOS_SDK_VERSION=$(TVOS_SDK_VERSION)" >> $@
	@echo "WATCH_SDK_VERSION=$(WATCH_SDK_VERSION)" >> $@
	@echo "MACOS_SDK_VERSION=$(MACOS_SDK_VERSION)" >> $@
	@echo "DOTNET_BCL_DIR=$(DOTNET_BCL_DIR)" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),DOTNET_$(platform)_RUNTIME_IDENTIFIERS='$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS)'\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),DOTNET_$(rid)_ARCHITECTURES='$(DOTNET_$(rid)_ARCHITECTURES)'\\n))" | sed 's/^ //' >> $@
	@echo "DOTNET_CSC_COMMAND='$(DOTNET_CSC)'" >> $@
	@echo "DOTNET_TFM=$(DOTNET_TFM)" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_VERSION_NO_METADATA=$($(platform)_NUGET_VERSION_NO_METADATA)\\n)" | sed 's/^ //' >> $@
	@echo "SYSTEM_MONO=$(SYSTEM_MONO)" >> $@
	@echo "DOTNET_DIR=$(DOTNET_DIR)" >> $@
	@echo "INCLUDE_MAC=$(INCLUDE_MAC)" >> $@
	@echo "INCLUDE_MACCATALYST=$(INCLUDE_MACCATALYST)" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_SDK_NAME=$($(platform)_NUGET_SDK_NAME)\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_REF_NAME=$($(platform)_NUGET_REF_NAME)\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(rid)_NUGET_RUNTIME_NAME=$($(rid)_NUGET_RUNTIME_NAME)\\n))" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),SUPPORTED_API_VERSIONS_$(platform)='$(SUPPORTED_API_VERSIONS_$(platform))'\\n)" | sed 's/^ //' >> $@
	@printf "ENABLE_XAMARIN=$(ENABLE_XAMARIN)\n" >> $@
	@printf "XCODE_IS_STABLE=$(XCODE_IS_STABLE)\n" >> $@
	@printf "XCODE_VERSION=$(XCODE_VERSION)\n" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),DOTNET_MIN_$(platform)_SDK_VERSION=$(DOTNET_MIN_$(platform)_SDK_VERSION)\\n)" | sed 's/^ //' >> $@
	@printf "NUGET_PRERELEASE_IDENTIFIER=$(NUGET_PRERELEASE_IDENTIFIER)\n" >> $@

test-system.config: Makefile $(TOP)/Make.config $(TOP)/mk/mono.mk $(TOP)/eng/Version.Details.xml
	@rm -f $@
	@echo "JENKINS_RESULTS_DIRECTORY=$(abspath $(JENKINS_RESULTS_DIRECTORY))" >> $@
	@echo "INCLUDE_DEVICE=$(INCLUDE_DEVICE)" >> $@
	@echo "DOTNET=$(DOTNET)" >> $@
	@echo "IOS_SDK_VERSION=$(IOS_SDK_VERSION)" >> $@
	@echo "TVOS_SDK_VERSION=$(TVOS_SDK_VERSION)" >> $@
	@echo "WATCH_SDK_VERSION=$(WATCH_SDK_VERSION)" >> $@
	@echo "MACOS_SDK_VERSION=$(MACOS_SDK_VERSION)" >> $@
	@echo "DOTNET_TFM=$(DOTNET_TFM)" >> $@
	@echo "DOTNET_BCL_DIR=$(DOTNET_BCL_DIR)" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),DOTNET_$(platform)_RUNTIME_IDENTIFIERS='$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS)'\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),DOTNET_$(rid)_ARCHITECTURES='$(DOTNET_$(rid)_ARCHITECTURES)'\\n))" | sed 's/^ //' >> $@
	@echo "DOTNET_CSC_COMMAND='$(DOTNET_CSC)'" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_VERSION_NO_METADATA=$($(platform)_NUGET_VERSION_NO_METADATA)\\n)" | sed 's/^ //' >> $@
	@echo "SYSTEM_MONO=$(SYSTEM_MONO)" >> $@
	@echo "DOTNET_DIR=$(DOTNET_DIR)" >> $@
	@echo "INCLUDE_MAC=$(INCLUDE_MAC)" >> $@
	@echo "INCLUDE_MACCATALYST=$(INCLUDE_MACCATALYST)" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_SDK_NAME=$($(platform)_NUGET_SDK_NAME)\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(platform)_NUGET_REF_NAME=$($(platform)_NUGET_REF_NAME)\\n)" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(rid)_NUGET_RUNTIME_NAME=$($(rid)_NUGET_RUNTIME_NAME)\\n))" | sed 's/^ //' >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),SUPPORTED_API_VERSIONS_$(platform)='$(SUPPORTED_API_VERSIONS_$(platform))'\\n)" | sed 's/^ //' >> $@
	@printf "ENABLE_XAMARIN=$(ENABLE_XAMARIN)\n" >> $@
	@printf "XCODE_IS_STABLE=$(XCODE_IS_STABLE)\n" >> $@
	@printf "XCODE_VERSION=$(XCODE_VERSION)\n" >> $@
	@printf "$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),DOTNET_MIN_$(platform)_SDK_VERSION=$(DOTNET_MIN_$(platform)_SDK_VERSION)\\n)" | sed 's/^ //' >> $@
	@printf "NUGET_PRERELEASE_IDENTIFIER=$(NUGET_PRERELEASE_IDENTIFIER)\n" >> $@

## run targets = build + [install] + exec

run run-all run-tests run-test run-unit-tests:
	$(Q) for subdir in $(SUBDIRS); do \
		$(MAKE) -C $$subdir run || exit 1; \
	done
	$(Q) $(MAKE) run-local

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

build-test-libraries:
	@$(MAKE) -C $(TOP)/tests/test-libraries

xharness: $(XHARNESS_EXECUTABLE)

all-local:: xharness

$(TOP)/tools/common/SdkVersions.cs: $(TOP)/tools/common/SdkVersions.in.cs
	@$(MAKE) -C $(TOP)/tools/mtouch ../common/SdkVersions.cs

.stamp-src-project-files:
	@$(MAKE) -C $(TOP)/src project-files
	@touch $@

-include xharness/xharness.csproj.inc
$(XHARNESS_EXECUTABLE): MSBUILD_EXE_PATH=
$(XHARNESS_EXECUTABLE): $(xharness_dependencies) test.config test-system.config .stamp-src-project-files $(TOP)/tools/common/SdkVersions.cs
	$(Q_GEN) $(DOTNET) build "/bl:$@.binlog" $(MSBUILD_VERBOSITY_QUIET) xharness/xharness.csproj
xharness/xharness.csproj.inc: export BUILD_VERBOSITY=$(DOTNET_BUILD_VERBOSITY)
xharness/xharness.csproj.inc: export DOTNET:=$(DOTNET)
xharness/xharness.csproj.inc: export MSBUILD_EXE_PATH=

killall:
	@killall "iPhone Simulator" >/dev/null 2>&1 || true
	@killall "iOS Simulator" >/dev/null 2>&1 || true
	@killall Touch.Server >/dev/null 2>&1 || true

test-msbuild run-tests-msbuild:
	$(MAKE) test-macdev-tests test-macdev-tasks

# Example TEST_FILTER:
#    TEST_FILTER="--filter FullyQualifiedName~BuildMyCocoaApp"
# Docs: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test#filter-option-details
test-macdev-tests: export MSBUILD_EXE_PATH=
test-macdev-tests: verify-system-vsmac-xcode-match
	$(Q) $(DOTNET) test $(TOP)/tests/msbuild/Xamarin.MacDev.Tests/Xamarin.MacDev.Tests.csproj $(TEST_FILTER)

# Example TEST_FILTER:
#    TEST_FILTER="--filter FullyQualifiedName~BuildMyCocoaApp"
# Docs: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test#filter-option-details
test-macdev-tasks: export MSBUILD_EXE_PATH=
test-macdev-tasks: verify-system-vsmac-xcode-match
	$(Q) $(DOTNET) test $(TOP)/tests/msbuild/Xamarin.MacDev.Tasks.Tests/Xamarin.MacDev.Tasks.Tests.csproj $(TEST_FILTER)

mac-test-package.zip:
ifdef INCLUDE_MAC
	./package-mac-tests.sh
else ifdef INCLUDE_MACCATALYST
	./package-mac-tests.sh
else
	@echo Not enabled
endif

jenkins: $(XHARNESS_EXECUTABLE)
	$(Q) $(DOTNET) $< $(XHARNESS_VERBOSITY) --jenkins --rootdir $(CURDIR) --sdkroot $(XCODE_DEVELOPER_ROOT) --markdown-summary=$(abspath $(CURDIR))/TestSummary.md --use-system=true $(TESTS_EXTRA_ARGUMENTS) $(TESTS_PERIODIC_COMMAND)

# This will launch xharness' interactive test runner in the system's default browser
runner: $(XHARNESS_EXECUTABLE)
	@echo "Running xharness in server mode. Press Ctrl-C to exit (or click Quit / press Q in the browser page)"
	$(Q) $(DOTNET) $< $(XHARNESS_VERBOSITY) --jenkins:server --rootdir $(CURDIR) --sdkroot $(XCODE_DEVELOPER_ROOT)

verify-system-vsmac-xcode-match:
	@SYSTEM_XCODE=$$(dirname $$(dirname $$(xcode-select -p))); \
	VSMAC_XCODE=$$(/usr/libexec/PlistBuddy -c 'print :AppleSdkRoot' ~/Library/Preferences/Xamarin/Settings.plist); \
	if [[ "$$SYSTEM_XCODE" != "$$VSMAC_XCODE" ]]; then echo "Error: the system's Xcode ($$SYSTEM_XCODE) does not match the Xcode selected in Visual Studio for Mac ($$VSMAC_XCODE). This will make tests fail in non-trivial ways, so please make sure both match the expected Xcode ($(shell dirname $(shell dirname $(XCODE_DEVELOPER_ROOT))))."; exit 1; fi

package-test-libraries.zip:
	$(Q) $(MAKE) all -C test-libraries
	$(Q) $(MAKE) all -C test-libraries/custom-type-assembly build-assembly
	$(Q_GEN) rm -f "$@" "$@.tmp"
	$(Q_GEN) cd $(TOP) && zip -9r --symlinks $(abspath $@).tmp ./tests/test-libraries
	$(Q_GEN) cd $(TOP) && find tests -regex 'tests/test-libraries/custom-type-assembly/.libs/.*dll' -exec zip -9r --symlinks $(abspath $@).tmp {} +
	$(Q) mv "$@".tmp "$@"

build-all:
	$(MAKE) build -C bgen
	$(MAKE) build -C cecil-tests
	$(MAKE) build -C dotnet/UnitTests
	$(MAKE) build -C rgen
	$(MAKE) build -C xtro-sharpie
	$(MAKE) build-all -C "linker/ios/dont link/dotnet"
	$(MAKE) build-all -C "linker/ios/link all/dotnet"
	$(MAKE) build-all -C "linker/ios/link sdk/dotnet"
	$(MAKE) build-all -C "linker/ios/trimmode copy/dotnet"
	$(MAKE) build-all -C "linker/ios/trimmode link/dotnet"
	$(MAKE) build-all -C fsharp/dotnet
	$(MAKE) build-all -C interdependent-binding-projects/dotnet
	$(MAKE) build-all -C introspection/dotnet
	$(MAKE) build-all -C monotouch-test/dotnet
	$(MAKE) build-all -C xcframework-test/dotnet
