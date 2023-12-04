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
# Options:
#
# ➡️ UNIVERSAL=1: build a universal app instead of whatever the default is.
# ➡️ RUNTIMEIDENTIFIERS=...: list the runtime identifiers to build for.
# ➡️ CONFIG=configuration: the configuration to use (defaults to Debug)
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

ifeq ($(RUNTIMEIDENTIFIERS),)
ifeq ($(PLATFORM),iOS)
RUNTIMEIDENTIFIERS=ios-arm64
else ifeq ($(PLATFORM),tvOS)
RUNTIMEIDENTIFIERS=tvos-arm64
else ifeq ($(PLATFORM),MacCatalyst)
ifeq ($(CONFIG),Release)
RUNTIMEIDENTIFIERS=maccatalyst-x64;maccatalyst-arm64
else ifneq ($(UNIVERSAL),)
RUNTIMEIDENTIFIERS=maccatalyst-x64;maccatalyst-arm64
else ifeq ($(shell arch),arm64)
RUNTIMEIDENTIFIERS=maccatalyst-arm64
else
RUNTIMEIDENTIFIERS=maccatalyst-x64
endif
else ifeq ($(PLATFORM),macOS)
ifeq ($(CONFIG),Release)
RUNTIMEIDENTIFIERS=osx-x64;osx-arm64
else ifneq ($(UNIVERSAL),)
RUNTIMEIDENTIFIERS=osx-x64;osx-arm64
else
RUNTIMEIDENTIFIERS=osx-x64
endif
else
RUNTIMEIDENTIFIERS=unknown-platform-$(PLATFORM)
endif
endif

ifneq ($(UNIVERSAL),)
UNIVERSAL_ARGUMENT=/p:UniversalBuild=true
endif

ifeq ($(findstring ;,$(RUNTIMEIDENTIFIERS)),;)
PATH_RID=
else
PATH_RID=$(RUNTIMEIDENTIFIERS)/
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
	$(Q) $(DOTNET) build "/bl:$(abspath $@-$(BINLOG_TIMESTAMP).binlog)" *.?sproj $(MSBUILD_VERBOSITY) $(BUILD_ARGUMENTS) $(CONFIG_ARGUMENT) $(UNIVERSAL_ARGUMENT) $(NATIVEAOT_ARGUMENTS)

run: prepare
	$(Q) $(DOTNET) build "/bl:$(abspath $@-$(BINLOG_TIMESTAMP).binlog)" *.?sproj $(MSBUILD_VERBOSITY) $(BUILD_ARGUMENTS) $(CONFIG_ARGUMENT) $(UNIVERSAL_ARGUMENT) $(NATIVEAOT_ARGUMENTS) -t:Run

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

clean:
	rm -Rf bin obj *.binlog
