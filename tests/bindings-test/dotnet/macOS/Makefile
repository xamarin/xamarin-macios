TOP=../../../..

include $(TOP)/Make.config

prepare:
	$(MAKE) -C $(TOP)/tests/dotnet
	$(CP) $(TOP)/tests/dotnet/NuGet.config $(TOP)/tests/dotnet/global.json .

build: prepare
	$(DOTNET6) build /bl $(MSBUILD_VERBOSITY)
