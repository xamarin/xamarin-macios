TOP=../../../..
include $(TOP)/Make.config
include $(TOP)/mk/colors.mk

TESTNAME:=$(notdir $(shell dirname $(shell dirname $(CURDIR))))

prepare:
	$(Q) $(MAKE) -C $(TOP)/tests/dotnet copy-dotnet-config

reload:
	$(Q) rm -Rf $(TOP)/tests/dotnet/packages
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
	$(Q) $(DOTNET6) build /bl *.csproj $(MSBUILD_VERBOSITY) $(BUILD_ARGUMENTS)

run: prepare
	$(Q) $(DOTNET6) build /bl *.csproj $(MSBUILD_VERBOSITY) $(BUILD_ARGUMENTS) -t:Run

run-bare:
	$(Q) ./bin/Debug/net6.0-*/*/$(TESTNAME).app/Contents/MacOS/$(TESTNAME)

diag: prepare
	$(Q) $(DOTNET6) build /v:diag msbuild.binlog

clean:
	rm -Rf bin obj *.binlog
