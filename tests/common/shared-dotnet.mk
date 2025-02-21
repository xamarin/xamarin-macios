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

BINLOG_TIMESTAMP:=$(shell date +%Y-%m-%d-%H%M%S)

ifeq ($(TESTNAME),)
TESTNAME:=$(notdir $(shell dirname "$(shell dirname "$(CURDIR)")"))
endif

ifeq ($(TEST_TFM),)
TEST_TFM=$(DOTNET_TFM)
endif

ifeq ($(CONFIG),)
CONFIG=Debug
else
CONFIG_ARGUMENT=/p:Configuration=$(CONFIG)
endif

ifeq ($(PLATFORM),)
PLATFORM=$(shell basename "$(CURDIR)")
endif

ifneq ($(RUNTIMEIDENTIFIERS)$(RUNTIMEIDENTIFIER),)
$(error "Don't set RUNTIMEIDENTIFIER or RUNTIMEIDENTIFIERS, set RID instead (RUNTIMEIDENTIFIER=$(RUNTIMEIDENTIFIER), RUNTIMEIDENTIFIERS=$(RUNTIMEIDENTIFIERS))")
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
TEST_VARIATION_ARGUMENT=/p:TestVariation=$(TEST_VARIATION)
endif

ifeq ($(PLATFORM),iOS)
EXECUTABLE="$(abspath .)/bin/$(CONFIG)/$(TEST_TFM)-ios/$(PATH_RID)$(TESTNAME).app/$(TESTNAME)"
else ifeq ($(PLATFORM),tvOS)
EXECUTABLE="$(abspath .)/bin/$(CONFIG)/$(TEST_TFM)-tvos/$(PATH_RID)$(TESTNAME).app/$(TESTNAME)"
else ifeq ($(PLATFORM),MacCatalyst)
EXECUTABLE="$(abspath .)/bin/$(CONFIG)/$(TEST_TFM)-maccatalyst/$(PATH_RID)$(TESTNAME).app/Contents/MacOS/$(TESTNAME)"
else ifeq ($(PLATFORM),macOS)
EXECUTABLE="$(abspath .)/bin/$(CONFIG)/$(TEST_TFM)-macos/$(PATH_RID)$(TESTNAME).app/Contents/MacOS/$(TESTNAME)"
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
	$(Q) $(DOTNET) build "/bl:$(abspath $@-$(BINLOG_TIMESTAMP).binlog)" *.?sproj $(DOTNET_BUILD_VERBOSITY) $(BUILD_ARGUMENTS) $(CONFIG_ARGUMENT) $(UNIVERSAL_ARGUMENT) $(NATIVEAOT_ARGUMENTS) $(TEST_VARIATION_ARGUMENT)

run: prepare
	$(Q) $(DOTNET) build "/bl:$(abspath $@-$(BINLOG_TIMESTAMP).binlog)" *.?sproj $(DOTNET_BUILD_VERBOSITY) $(BUILD_ARGUMENTS) $(CONFIG_ARGUMENT) $(UNIVERSAL_ARGUMENT) $(NATIVEAOT_ARGUMENTS) $(TEST_VARIATION_ARGUMENT) -t:Run

run-bare:
	$(Q) $(EXECUTABLE) --autostart --autoexit $(RUN_ARGUMENTS)

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

list-variations listvariations show-variations showvariations variations:
	$(Q) if ! command -v jq > /dev/null; then echo "$(shell tput setaf 9)jq isn't installed. Install by doing 'brew install jq'$(shell tput sgr0)"; exit 1; fi
	$(Q) echo "Test variations for $(shell tput setaf 6)$(TESTNAME)$(shell tput sgr0):"
	$(Q) echo ""
	$(Q) $(DOTNET) build -getItem:TestVariations | jq '.Items.TestVariations[] | "\(.Identity): \(.Description)"' | sed -e 's/^"/    /' -e 's/"$$//'
	$(Q) echo ""
	$(Q) echo "Build and run a specific variation by doing $(shell tput setaf 6)make build TEST_VARIATION=variation$(shell tput sgr0)"

clean:
	rm -Rf bin obj *.binlog
