
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

reload-and-build:
	$(Q) $(MAKE) reload
	$(Q) $(MAKE) build

reload-and-run:
	$(Q) $(MAKE) reload
	$(Q) $(MAKE) run

build: prepare
	$(Q) $(DOTNET6) build "/bl:$@-$(BINLOG_TIMESTAMP).binlog" *.?sproj $(MSBUILD_VERBOSITY) $(BUILD_ARGUMENTS)

run: prepare
	$(Q) $(DOTNET6) build "/bl:$@-$(BINLOG_TIMESTAMP).binlog" *.?sproj $(MSBUILD_VERBOSITY) $(BUILD_ARGUMENTS) -t:Run

run-bare:
	$(Q) ./bin/Debug/net6.0-*/*/"$(TESTNAME)".app/Contents/MacOS/"$(TESTNAME)" --autostart --autoexit

BINLOGS:=$(wildcard *.binlog)
diag: prepare
	$(Q) if [[ "$(words $(BINLOGS))" == "1" ]]; then \
		$(DOTNET6) build /v:diag $(BINLOGS); \
	else \
		echo "Choose your binlog to print:"; \
		select binlog in $(BINLOGS); do $(DOTNET6) build /v:diag $$binlog; break; done \
	fi

clean:
	rm -Rf bin obj *.binlog
