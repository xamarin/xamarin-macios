
include $(TOP)/Make.config
include $(TOP)/mk/colors.mk

unexport MSBUILD_EXE_PATH

BINLOG_TIMESTAMP:=$(shell date +%Y-%m-%d-%H%M%S)

ifeq ($(TESTNAME),)
TESTNAME:=$(notdir $(shell dirname "$(shell dirname "$(CURDIR)")"))
endif

ifeq ($(CONFIG),)
CONFIG=Debug
else
CONFIG_ARGUMENT=/p:Configuration=$(CONFIG)
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
	$(Q) $(DOTNET) build "/bl:$(abspath $@-$(BINLOG_TIMESTAMP).binlog)" *.?sproj $(MSBUILD_VERBOSITY) $(BUILD_ARGUMENTS) $(CONFIG_ARGUMENT)

run: prepare
	$(Q) $(DOTNET) build "/bl:$(abspath $@-$(BINLOG_TIMESTAMP).binlog)" *.?sproj $(MSBUILD_VERBOSITY) $(BUILD_ARGUMENTS) $(CONFIG_ARGUMENT) -t:Run

run-bare:
	$(Q) "$(abspath .)"/bin/$(CONFIG)/$(DOTNET_TFM)-*/*/"$(TESTNAME)".app/Contents/MacOS/"$(TESTNAME)" --autostart --autoexit

run-remote:
	$(Q) test -n "$(REMOTE_HOST)" || ( echo "Must specify the remote machine by setting the REMOTE_HOST environment variable"; exit 1 )
	@echo "Copying the '$(TESTNAME)' test app to $(REMOTE_HOST)..."
	rsync -avz ./bin/$(CONFIG)/$(DOTNET_TFM)-*/*/"$(TESTNAME)".app $(USER)@$(REMOTE_HOST):/tmp/test-run-remote-execution/
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
