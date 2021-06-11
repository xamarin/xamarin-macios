TOP=../../../../../..

include $(TOP)/Make.config

prepare:
	$(MAKE) -C $(TOP)/tests/dotnet
	$(CP) $(TOP)/tests/dotnet/NuGet.config $(TOP)/tests/dotnet/global.json .

build: prepare
	$(DOTNET6) build *.csproj /bl $(MSBUILD_VERBOSITY)

run: prepare
	$(DOTNET6) build *.csproj /bl $(MSBUILD_VERBOSITY) -t:Run

diag:
	$(DOTNET6) build *.binlog /v:diag
