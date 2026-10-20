include $(TOP)/Make.config
include $(TOP)/mk/colors.mk

#
# The targets here are available for .NET projects in the following directories: tests/<test>/dotnet/<platform>
#
# Targets:
#
# ➡️ build
#
#         Builds the project.
#
# ➡️ run
#
#         Runs the project using 'dotnet run' - this is the only way (in this
#         makefile) to run mobile projects (they'll run in the simulator by
#         default). This also works for desktop platforms, but you won't see
#         stdout/stderr, which is kind of annoying.
#
# ➡️ run-bare
#
#         Runs the executable. This doesn't work for mobile platforms, but is
#         usually the best option for desktop platforms, since it will
#         print stdout/stderr from the test app to the terminal.
#
# ➡️ run-old
#
#         Runs in the oldest simulator we can (only applicable for iOS and tvOS).
#
# ➡️ reload
#
#         Rebuilds all of xamarin-macios, and cleans up some stuff, so that
#         everything should be ready to try again.
#
#         ⚠️ This target may hang for reasons I haven't been able to figure ⚠️
#         ⚠️ out yet, in which case just cancel (Ctrl-C) and try again      ⚠️
#
# ➡️ list-variations
#
#         Lists all the test variations for the current test project.
#
# Options:
#
# ➡️ UNIVERSAL=1: build a universal app instead of whatever the default is.
# ➡️ RUNTIMEIDENTIFIERS=...: list the runtime identifiers to build for.
# ➡️ CONFIG=configuration: the configuration to use (defaults to Debug)
# ➡️ TEST_VARIATION=variation: the test variation to use. Run 'make list-variations' to get a list of all the valid test variations.
#
# Example to run monotouch-test on Mac Catalyst:
#
#     $ cd tests/monotouch-test/dotnet/MacCactalyst
#     $ make build
#     $ make run-bare
#
# After a change (anywhere in the repo), run reload first to rebuild:
#
#    $ make reload
#    $ make build
#    $ make run-bare
#

unexport MSBUILD_EXE_PATH

SPAWNER?=$(TOP)/tools/spawner/spawner

BINLOG_TIMESTAMP:=$(shell date +%Y-%m-%d-%H%M%S)

ifeq ($(TESTNAME),)
TESTNAME:=$(notdir $(shell dirname "$(shell dirname "$(CURDIR)")"))
endif

ifeq ($(TEST_TFM),)
TEST_TFM=$(DOTNET_TFM)
endif

ifeq ($(findstring |release|,|$(TEST_VARIATION)|),|release|)
CONFIG=Release
endif

ifeq ($(CONFIG),)
CONFIG=Debug
else
CONFIG_ARGUMENT=/p:Configuration=$(CONFIG)
endif

ifeq ($(PLATFORM),)
PLATFORM=$(shell basename "$(CURDIR)")
endif
PLATFORM_UPPERCASE:=$(call uppercase,$(PLATFORM))

ifneq ($(RUNTIMEIDENTIFIERS)$(RUNTIMEIDENTIFIER),)
$(error "Don't set RUNTIMEIDENTIFIER or RUNTIMEIDENTIFIERS, set RID instead")
endif

ifeq ($(RID),)
ifeq ($(PLATFORM),iOS)
RID=iossimulator-arm64
else ifeq ($(PLATFORM),tvOS)
RID=tvossimulator-arm64
else ifeq ($(PLATFORM),MacCatalyst)
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
else
RID=unknown-platform-$(PLATFORM)
endif
endif

ifneq ($(UNIVERSAL),)
UNIVERSAL_ARGUMENT=/p:UniversalBuild=true
endif

ifeq ($(findstring ;,$(RID)),;)
PATH_RID=
export RUNTIMEIDENTIFIERS=$(RID)
else
PATH_RID=$(RID)/
export RUNTIMEIDENTIFIER=$(RID)
endif

ifneq ($(TEST_VARIATION),)
TEST_VARIATION_ARGUMENT='/p:TestVariation=$(TEST_VARIATION)'
endif

ifeq ($(PLATFORM),iOS)
EXECUTABLE="$(abspath .)/bin/$(CONFIG)/$(TEST_TFM)-ios/$(PATH_RID)$(TESTNAME).app/$(TESTNAME)"
else ifeq ($(PLATFORM),tvOS)
EXECUTABLE="$(abspath .)/bin/$(CONFIG)/$(TEST_TFM)-tvos/$(PATH_RID)$(TESTNAME).app/$(TESTNAME)"
else ifeq ($(PLATFORM),MacCatalyst)
EXECUTABLE="$(abspath .)/bin/$(CONFIG)/$(TEST_TFM)-maccatalyst/$(PATH_RID)$(TESTNAME).app/Contents/MacOS/$(TESTNAME)"
APPMANIFEST:=$(abspath .)/bin/$(CONFIG)/$(TEST_TFM)-maccatalyst/$(PATH_RID)$(TESTNAME).app/Contents/Info.plist
BUNDLE_ID=$(shell defaults read "$(APPMANIFEST)" CFBundleIdentifier 2>/dev/null)
else ifeq ($(PLATFORM),macOS)
EXECUTABLE="$(abspath .)/bin/$(CONFIG)/$(TEST_TFM)-macos/$(PATH_RID)$(TESTNAME).app/Contents/MacOS/$(TESTNAME)"
APPMANIFEST:=$(abspath .)/bin/$(CONFIG)/$(TEST_TFM)-macos/$(PATH_RID)$(TESTNAME).app/Contents/Info.plist
BUNDLE_ID=$(shell defaults read "$(APPMANIFEST)" CFBundleIdentifier 2>/dev/null)
else
EXECUTABLE="unknown-executable-platform-$(PLATFORM)"
endif

ifneq ($(PUBLISHAOT)$(NATIVEAOT),)
NATIVEAOT_ARGUMENTS=/p:PublishAot=true /p:_IsPublishing=true
endif

prepare:
	@# nothing to do here right now

reload:
	$(Q) $(MAKE) -C $(TOP) -j8 all
	$(Q) $(MAKE) -C $(TOP) -j8 install
	$(Q) git clean -xfdq
	$(Q) $(DOTNET) build-server shutdown # make sure msbuild picks up any new task assemblies we built

reload-and-build:
	$(Q) $(MAKE) reload
	$(Q) $(MAKE) build

reload-and-run:
	$(Q) $(MAKE) reload
	$(Q) $(MAKE) run

build: prepare
	@echo "Building $(wildcard *.?sproj)... (binlog: $(shell tput setaf 6 2>/dev/null)$(abspath $@-$(BINLOG_TIMESTAMP).binlog)$(shell tput sgr0 2>/dev/null))"
	$(Q) $(DOTNET) build "/bl:$(abspath $@-$(BINLOG_TIMESTAMP).binlog)" *.?sproj $(DOTNET_BUILD_VERBOSITY) $(BUILD_ARGUMENTS) $(CONFIG_ARGUMENT) $(UNIVERSAL_ARGUMENT) $(NATIVEAOT_ARGUMENTS) $(TEST_VARIATION_ARGUMENT)

run: export SIMCTL_CHILD_NUNIT_AUTOSTART=true
run: export SIMCTL_CHILD_NUNIT_AUTOEXIT=true
run: prepare
	@echo "Running $(wildcard *.?sproj)..."
	$(Q) $(DOTNET) build "/bl:$(abspath $@-$(BINLOG_TIMESTAMP).binlog)" *.?sproj $(DOTNET_BUILD_VERBOSITY) $(BUILD_ARGUMENTS) $(CONFIG_ARGUMENT) $(UNIVERSAL_ARGUMENT) $(NATIVEAOT_ARGUMENTS) $(TEST_VARIATION_ARGUMENT) -t:Run $(RUN_ARGUMENTS) -tl:off

# Delete the saved application state for the app, to prevent the
# "Do you want to try to reopen its windows again?" dialog from
# blocking automated runs after a crash. See https://github.com/dotnet/macios/issues/25922
delete-saved-state:
	$(Q) if test -n "$(BUNDLE_ID)"; then rm -rf "$(HOME)/Library/Saved Application State/$(BUNDLE_ID).savedState" && echo "Deleted saved state for $(BUNDLE_ID)"; fi

# Clear RUNTIMEIDENTIFIER(S) so the recursive '$(MAKE) delete-saved-state' below doesn't
# inherit the exported value and trip the guard that forbids setting it (set RID instead).
run-bare: export RUNTIMEIDENTIFIER=
run-bare: export RUNTIMEIDENTIFIERS=
run-bare: delete-saved-state
	$(Q) $(SPAWNER) $(EXECUTABLE) --autostart --autoexit $(RUN_ARGUMENTS)
	$(Q) $(MAKE) delete-saved-state

# Get the list of applicable simulators, and pick the first in the list.
# Make sure to have a matching simulator runtime installed, otherwise this won't work.
run-old: RUN_ARGUMENTS=-p:Device=$(shell xcrun simctl list devices "$(PLATFORM) $(MIN_$(PLATFORM_UPPERCASE)_SIMULATOR_VERSION)" -j | jq -c '.[][][].udid' | head -1 | sed 's/"//g')
run-old: export RUNTIMEIDENTIFIER=
run-old:
	$(MAKE) run

print-executable:
	@echo $(EXECUTABLE)

run-remote:
	$(Q) test -n "$(REMOTE_HOST)" || ( echo "Must specify the remote machine by setting the REMOTE_HOST environment variable"; exit 1 )
	@echo "Copying the '$(TESTNAME)' test app to $(REMOTE_HOST)..."
	rsync -avz ./bin/$(CONFIG)/$(TEST_TFM)-*/*/"$(TESTNAME)".app $(USER)@$(REMOTE_HOST):/tmp/test-run-remote-execution/
	@echo "Killing any existing test executables ('$(TESTNAME)')"
	ssh $(USER)@$(REMOTE_HOST) -- pkill -9 "$(TESTNAME)" || true
	@echo "Executing '$(TESTNAME)' on $(REMOTE_HOST)..."
	ssh $(USER)@$(REMOTE_HOST) -- /tmp/test-run-remote-execution/"$(TESTNAME)".app/Contents/MacOS/"$(TESTNAME)" --autostart --autoexit

delete-remote:
	$(Q) test -n "$(REMOTE_HOST)" || ( echo "Must specify the remote machine by setting the REMOTE_HOST environment variable"; exit 1 )
	ssh $(USER)@$(REMOTE_HOST) -- rm -rf /tmp/test-run-remote-execution/"$(TESTNAME)".app

BINLOGS:=$(wildcard *.binlog)
diag: prepare
	$(Q) if [[ "$(words $(BINLOGS))" == "1" ]]; then \
		$(DOTNET) build /v:diag $(BINLOGS); \
	else \
		echo "Choose your binlog to print:"; \
		select binlog in $(BINLOGS); do $(DOTNET) build /v:diag $$binlog; break; done \
	fi

list-variations listvariations show-variations showvariations variations help:
	$(Q) if ! command -v jq > /dev/null; then echo "$(shell tput setaf 9)jq isn't installed. Install by doing 'brew install jq'$(shell tput sgr0)"; exit 1; fi
	$(Q) echo "Test variations for $(shell tput setaf 6)$(TESTNAME)$(shell tput sgr0):"
	$(Q) echo ""
	$(Q) $(DOTNET) build -getItem:TestVariations | jq '.Items.TestVariations[] | "\(.Identity): \(.Description)"' | sed -e 's/^"/    /' -e 's/"$$//'
	$(Q) echo ""
	$(Q) echo "Build and run a specific variation by doing $(shell tput setaf 6)make build TEST_VARIATION=variation$(shell tput sgr0)."
	$(Q) echo "It's possible to select multiple variations by separating them with a pipe character: $(shell tput setaf 6)make build TEST_VARIATION='variation1|variation2'$(shell tput sgr0)."

clean:
	rm -Rf bin obj *.binlog
