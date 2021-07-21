TOP=../../../..

include $(TOP)/Make.config

prepare:
	$(MAKE) -C $(TOP)/tests/dotnet copy-dotnet-config

build: prepare
	$(DOTNET6) build /bl *.csproj $(MSBUILD_VERBOSITY)

run: prepare
	$(DOTNET6) build /bl *.csproj $(MSBUILD_VERBOSITY) /t:Run
