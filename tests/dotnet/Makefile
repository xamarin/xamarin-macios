TOP=../..

include $(TOP)/Make.config

# This tells NuGet to use the nupkgs we're building locally,
# and to put any extracted packages in the 'packages' directory (to not clutter up ~/.nuget/packages)
NuGet.config: $(TOP)/NuGet.config Makefile
	$(Q) $(CP) $< $@.tmp
	$(Q) nuget sources add -Name local-dotnet-feed -Source $(abspath $(DOTNET_FEED_DIR)) -ConfigFile $@.tmp
ifdef CUSTOM_DOTNET
	$(Q) nuget sources add -Name dev-runtime-feed -Source $(abspath $(DOTNET_RUNTIME_PATH)/artifacts/packages/Release/Shipping) -ConfigFile $@.tmp
endif
	$(Q) nuget config -Set globalPackagesFolder=$(abspath $(CURDIR)/packages) -Config $@.tmp
	$(Q) nuget config -Set repositorypath=$(abspath $(CURDIR)/packages) -Config $@.tmp
	$(Q) mv $@.tmp $@

# This tells NuGet to use the exact same dotnet version we've configured in Make.config
global.json: $(TOP)/global6.json
	$(CP) $< $@

../bgen/global.json: global.json
	$(Q) $(CP) $< $@

../bgen/NuGet.config: NuGet.config
	$(Q) $(CP) $< $@

TARGETS += \
	NuGet.config \
	global.json \
	../bgen/NuGet.config \
	../bgen/global.json \

# Example TEST_FILTER:
#    TEST_FILTER="--filter FullyQualifiedName~BuildMyCocoaApp"
# Docs: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test#filter-option-details
run-unit-tests:
	cd UnitTests && $(DOTNET) test DotNetUnitTests.csproj $(TEST_FILTER)

all-local:: $(TARGETS)

compare compare-size: $(TARGETS)
	cd size-comparison && git clean -xfdq
	time $(MAKE) build-oldnet
	time $(MAKE) build-dotnet
	$(MAKE) report

report:
	cd ../../tools/app-compare && dotnet run $(abspath size-comparison/MySingleView/oldnet/bin/iPhone/Release/MySingleView.app) $(abspath size-comparison/MySingleView/dotnet/bin/iPhone/Release/net6.0-ios/ios-arm64/MySingleView.app) > $(CURDIR)/report.md
	gist $(CURDIR)/report.md

COMMON_ARGS=/p:Platform=iPhone /p:Configuration=Release
build-oldnet:
	$(SYSTEM_MSBUILD) $(XBUILD_VERBOSITY) size-comparison/MySingleView/oldnet/MySingleView.csproj $(COMMON_ARGS) /bl:$@.binlog

build-dotnet: $(TARGETS)
	$(DOTNET6) build size-comparison/MySingleView/dotnet/MySingleView.csproj --runtime ios-arm64 $(COMMON_ARGS) /bl:$@.binlog

run-dotnet: $(TARGETS)
	$(DOTNET6) build -t:Run size-comparison/MySingleView/dotnet/MySingleView.csproj --runtime ios-arm64 $(COMMON_ARGS)

# this will break the signature, so app won't run anymore. Use it only to compare final size w/legacy
# https://github.com/xamarin/xamarin-macios/issues/11445
strip-dotnet:
	$(foreach file, $(wildcard size-comparison/MySingleView/dotnet/bin/iPhone/Release/net6.0-ios/ios-arm64/MySingleView.app/*.dll), mono-cil-strip $(file) $(file);)

# this target will copy NuGet.config and global.json to the directories that need it for their .NET build to work correctly.
copy-dotnet-config: $(TARGETS)
	$(Q) for dir in $(shell ls -d $(abspath $(TOP)/tests/*/dotnet)); do \
		$(CP) -c NuGet.config global.json $$dir; \
	done
	$(Q) $(CP) -c NuGet.config global.json $(TOP)/external/Touch.Unit/Touch.Client/dotnet
