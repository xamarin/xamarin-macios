# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

TOP=../../../../../..

include $(TOP)/Make.config
include $(TOP)/mk/colors.mk
include $(TOP)/scripts/run-audio-unit-extension-tests/fragment.mk

# This file is meant to be included from
# tests/monotouch-test/dotnet/extensions/<extension kind>/<platform>/Makefile.

BINLOG_TIMESTAMP:=$(shell date +%Y-%m-%d-%H%M%S)
RUN_TIMEOUT_SECONDS?=600
BUILD_PARAMETERS+=$(BUILD_ARGUMENTS)

ifeq ($(PLATFORM),)
PLATFORM=$(shell basename "$(CURDIR)")
endif

LOGFILENAME:=$(TMPDIR)/monotouch-test/extensions/$(EXTENSION_KIND)/$(PLATFORM)-$(shell date +%Y-%m-%d--%H:%M:%S).log
RESULTSFILENAME?=$(patsubst %.log,%.nunit-results.xml,$(LOGFILENAME))

# The runtime (CoreCLR/MonoVM), the registrar and other options are selected
# using TEST_VARIATION (run 'make help' to see all variations). Multiple
# variations can be combined with a pipe
# character, e.g. TEST_VARIATION='trimmable-static-registrar|release'.
ifeq ($(findstring |release|,|$(TEST_VARIATION)|),|release|)
CONFIG=Release
endif

ifeq ($(CONFIG),)
CONFIG=Debug
endif
CONFIG_ARGUMENT=/p:Configuration=$(CONFIG)

ifeq ($(RID),)
ifeq ($(PLATFORM),MacCatalyst)
ifeq ($(CONFIG),Release)
RID=maccatalyst-x64;maccatalyst-arm64
else ifneq ($(UNIVERSAL),)
RID=maccatalyst-x64;maccatalyst-arm64
else ifeq ($(shell arch),arm64)
RID=maccatalyst-arm64
else
RID=maccatalyst-x64
endif
else ifeq ($(PLATFORM),macOS)
ifeq ($(CONFIG),Release)
RID=osx-x64;osx-arm64
else ifneq ($(UNIVERSAL),)
RID=osx-x64;osx-arm64
else ifeq ($(shell arch),arm64)
RID=osx-arm64
else
RID=osx-x64
endif
else ifeq ($(PLATFORM),iOS)
RID=iossimulator-arm64
else ifeq ($(PLATFORM),tvOS)
RID=tvossimulator-arm64
else
RID=unknown-platform-$(PLATFORM)
endif
endif

ifneq ($(UNIVERSAL),)
UNIVERSAL_ARGUMENT=/p:UniversalBuild=true
endif

ifneq ($(TEST_VARIATION),)
TEST_VARIATION_ARGUMENT='/p:TestVariation=$(TEST_VARIATION)'
endif

ifneq ($(findstring ;,$(RID)),)
RID_ARGUMENT=/p:RuntimeIdentifiers=$(RID)
PATH_RID=
else
RID_ARGUMENT=/p:RuntimeIdentifier=$(RID)
PATH_RID=$(RID)/
endif

CONTAINER_PROJECT=$(abspath $(CURDIR))/ContainerApp/ContainerApp.csproj
APP_PATH=$(abspath $(CURDIR))/ContainerApp/bin/$(CONFIG)/$(DOTNET_TFM)-$(shell echo $(PLATFORM) | tr 'A-Z' 'a-z')/$(PATH_RID)ContainerApp.app
ifeq ($(filter $(PLATFORM),iOS tvOS),)
EXTENSION_PATH=$(APP_PATH)/Contents/PlugIns/monotouchtest.appex
EXECUTABLE=$(APP_PATH)/Contents/MacOS/ContainerApp
else
EXTENSION_PATH=$(APP_PATH)/PlugIns/monotouchtest.appex
EXECUTABLE=$(APP_PATH)/ContainerApp
endif
LSREGISTER=/System/Library/Frameworks/CoreServices.framework/Frameworks/LaunchServices.framework/Support/lsregister
prepare:
	@# nothing to do here right now

build: prepare
	$(Q) echo "Building app extension test project: $(COLOR_GRAY)$(CONTAINER_PROJECT)$(COLOR_CLEAR) [$(CONFIG) $(RID)]"
	$(Q) rm -rf "$(abspath $(CURDIR))/AppExtension/bin" "$(abspath $(CURDIR))/AppExtension/obj" "$(abspath $(CURDIR))/ContainerApp/bin" "$(abspath $(CURDIR))/ContainerApp/obj"
	$(Q) $(DOTNET) build "$(CONTAINER_PROJECT)" "/bl:$(abspath build-$(BINLOG_TIMESTAMP).binlog)" $(DOTNET_BUILD_VERBOSITY) $(BUILD_PARAMETERS) $(CONFIG_ARGUMENT) $(RID_ARGUMENT) $(UNIVERSAL_ARGUMENT) $(TEST_VARIATION_ARGUMENT)
	$(Q) echo "Build completed."

register-extension: build
	$(Q) "$(LSREGISTER)" -f "$(APP_PATH)"
	$(Q) pluginkit -a "$(EXTENSION_PATH)"

run: build $(RUN_AUDIO_UNIT_EXTENSION_TESTS)
	$(Q) echo "Running monotouch-test from the app extension: $(COLOR_GRAY)$(EXTENSION_PATH)$(COLOR_CLEAR)"
	$(Q) echo "Writing output to: $(COLOR_GRAY)$(LOGFILENAME)$(COLOR_CLEAR)"
	$(Q) $(RUN_AUDIO_UNIT_EXTENSION_TESTS_EXEC) \
		--platform "$(PLATFORM)" \
		--rid "$(RID)" \
		--config "$(CONFIG)" \
		--app "$(APP_PATH)" \
		--extension "$(EXTENSION_PATH)" \
		--executable "$(EXECUTABLE)" \
		--log-file "$(LOGFILENAME)" \
		--results-file "$(RESULTSFILENAME)" \
		--timeout-seconds "$(RUN_TIMEOUT_SECONDS)" \
		--lsregister "$(LSREGISTER)" \
		$(if $(SIMULATOR_UDID),--simulator-udid "$(SIMULATOR_UDID)") \
		$(if $(TEST_FILTER),--test-filter "$(TEST_FILTER)")

run-bare: run

print-app-path:
	@echo $(APP_PATH)

print-extension-path:
	@echo $(EXTENSION_PATH)

print-executable:
	@echo $(EXECUTABLE)

clean:
	rm -Rf AppExtension/bin AppExtension/obj ContainerApp/bin ContainerApp/obj *.binlog
