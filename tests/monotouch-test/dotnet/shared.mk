TOP=../../../..

include $(TOP)/Make.config
include $(TOP)/mk/colors.mk

reload:
	$(Q) rm -Rf $(TOP)/tests/dotnet/packages
	$(Q) $(MAKE) -C $(TOP) -j8 all
	$(Q) $(MAKE) -C $(TOP) -j8 install
	$(Q) git clean -xfdq

prepare:
	$(Q) $(MAKE) -C $(TOP)/tests/dotnet copy-dotnet-config

reload-and-build:
	$(Q) $(MAKE) reload
	$(Q) $(MAKE) build

reload-and-run:
	$(Q) $(MAKE) reload
	$(Q) $(MAKE) run

build: prepare
	$(DOTNET6) build /bl *.csproj $(MSBUILD_VERBOSITY)

run: prepare
	$(DOTNET6) build /bl *.csproj $(MSBUILD_VERBOSITY) -t:Run

diag:
	$(DOTNET6) build /v:diag msbuild.binlog
