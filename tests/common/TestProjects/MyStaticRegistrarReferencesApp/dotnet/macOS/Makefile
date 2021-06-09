TOP=../../../../../..

include $(TOP)/Make.config

prepare:
	$(MAKE) -C $(TOP)/tests/dotnet
	$(CP) $(TOP)/tests/dotnet/NuGet.config $(TOP)/tests/dotnet/global.json .

build: prepare
	$(DOTNET6) build *.csproj /bl $(MSBUILD_VERBOSITY)

run: prepare
	$(DOTNET6) build *.csproj /bl $(MSBUILD_VERBOSITY) -t:Run

exec:
	./bin/Debug/net6.0-macos/osx-x64/MyStaticRegistrarReferencesApp.app/Contents/MacOS/MyStaticRegistrarReferencesApp

diag:
	$(DOTNET6) build *.binlog /v:diag
