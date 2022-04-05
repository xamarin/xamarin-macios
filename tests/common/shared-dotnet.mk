
include $(TOP)/Make.config
include $(TOP)/mk/colors.mk

unexport MSBUILD_EXE_PATH

BINLOG_TIMESTAMP:=$(shell date +%Y-%m-%d-%H%M%S)

ifeq ($(TESTNAME),)
TESTNAME:=$(notdir $(shell dirname "$(shell dirname "$(CURDIR)")"))
endif

prepare:
	$(Q) $(MAKE) -C $(TOP)/tests/dotnet copy-dotnet-config

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
	$(Q) $(DOTNET) build "/bl:$(abspath $@-$(BINLOG_TIMESTAMP).binlog)" *.?sproj $(MSBUILD_VERBOSITY) $(BUILD_ARGUMENTS)

run: prepare
	$(Q) $(DOTNET) build "/bl:$(abspath $@-$(BINLOG_TIMESTAMP).binlog)" *.?sproj $(MSBUILD_VERBOSITY) $(BUILD_ARGUMENTS) -t:Run

run-bare:
	$(Q) "$(abspath .)"/bin/Debug/$(DOTNET_TFM)-*/*/"$(TESTNAME)".app/Contents/MacOS/"$(TESTNAME)" --autostart --autoexit

run-remote:
	$(Q) test -n "$(REMOTE_HOST)" || ( echo "Must specify the remote machine by setting the REMOTE_HOST environment variable"; exit 1 )
	@echo "Copying the '$(TESTNAME)' test app to $(REMOTE_HOST)..."
	rsync -avz ./bin/Debug/$(DOTNET_TFM)-*/*/"$(TESTNAME)".app $(USER)@$(REMOTE_HOST):/tmp/test-run-remote-execution/
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
