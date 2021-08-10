TOP=../../../..
include $(TOP)/Make.config
include $(TOP)/mk/colors.mk

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

build: prepare
	$(Q) $(DOTNET6) build /bl *.csproj $(MSBUILD_VERBOSITY) $(BUILD_ARGUMENTS)

diag: prepare
	$(Q) $(DOTNET6) build /v:diag msbuild.binlog
