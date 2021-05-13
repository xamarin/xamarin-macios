TOP=../../..

include $(TOP)/Make.config

# This is a temporary variable to enable the .NET workload resolver, because it's opt-in for now.
# Ref: https://github.com/dotnet/sdk/issues/13849
export MSBuildEnableWorkloadResolver=true

prepare:
	cd .. && $(MAKE) global.json NuGet.config
	rm -Rf */bin */obj

all-ios: prepare
	$(DOTNET6) build iOS/*.csproj /bl

all-mac: prepare
	$(DOTNET6) build macOS/*.csproj /bl

run-mac:
	./macOS/bin/Debug/net6.0-macos/osx-x64/NativeReferences.app/Contents/MacOS/NativeReferences

build:
	cd $(TOP) && rm -Rf _build
	cd $(TOP)/dotnet && git clean -xfd
	cd $(TOP)/tests && git clean -xfd
	cd $(TOP)/msbuild && git clean -xfd
	unset MSBuildEnableWorkloadResolver && $(MAKE) -C $(TOP) all -j9 && $(MAKE) -C $(TOP) install -j9

diag:
	cd .. && $(MAKE) global.json NuGet.config
	$(DOTNET6) build /v:diag *binlog
