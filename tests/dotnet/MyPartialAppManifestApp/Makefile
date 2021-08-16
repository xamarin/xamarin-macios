TOP=../../..

include $(TOP)/Make.config

prepare:
	cd .. && $(MAKE) global.json NuGet.config
	rm -Rf */bin */obj

all-ios: prepare
	$(DOTNET6) build iOS/*.csproj /bl

all-mac: prepare
	$(DOTNET6) build macOS/*.csproj /bl

run-mac:
	./macOS/bin/Debug/net6.0-macos/osx-x64/$(notdir $(CURDIR)).app/Contents/MacOS/$(notdir $(CURDIR))

diag:
	cd .. && $(MAKE) global.json NuGet.config
	$(DOTNET6) build /v:diag *binlog
