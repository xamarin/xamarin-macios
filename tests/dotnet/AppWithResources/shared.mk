TOP=../../../..
include $(TOP)/Make.config

prepare:
	$(Q) $(MAKE) -C $(TOP)/tests/dotnet copy-dotnet-config

reload:
	$(Q) $(MAKE) -C $(TOP)/tests/dotnet reload
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

diag: prepare
	$(Q) $(DOTNET6) build /v:diag msbuild.binlog
