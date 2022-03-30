TOP=../..

include $(TOP)/Make.config

# This tells NuGet to use the nupkgs we're building locally,
# and to put any extracted packages in the 'packages' directory (to not clutter up ~/.nuget/packages)
NuGet.config: $(TOP)/NuGet.config Makefile
	$(Q) mkdir -p "$(abspath $(NUGET_TEST_FEED))"
	$(Q) $(CP) $< $@.tmp
ifdef CUSTOM_DOTNET
	$(Q) nuget sources add -Name dev-runtime-feed -Source $(abspath $(DOTNET_RUNTIME_PATH)/artifacts/packages/Release/Shipping) -ConfigFile $@.tmp
endif
	$(Q) nuget sources add -Name local-tests-feed -Source $(abspath $(NUGET_TEST_FEED)) -ConfigFile $@.tmp
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

run-unit-tests:
	$(MAKE) -C UnitTests $@

all-local:: $(TARGETS)

reload: $(TARGETS)
	$(Q) $(MAKE) -C $(TOP) -j8 all
	$(Q) $(MAKE) -C $(TOP) -j8 install
	$(Q) $(DOTNET) build-server shutdown # make sure msbuild picks up any new task assemblies we built

compare compare-size: $(TARGETS)
	rm -rf packages
	cd size-comparison && git clean -xfdq
	time $(MAKE) build-oldnet
	time $(MAKE) build-dotnet
	$(MAKE) report

report:
	appcompare $(abspath size-comparison/MySingleView/oldnet/bin/iPhone/Release/MySingleView.app) $(abspath size-comparison/MySingleView/dotnet/bin/iPhone/Release/net6.0-ios/ios-arm64/MySingleView.app) \
		--output-markdown $(CURDIR)/report.md --gist --mapping-file $(CURDIR)/size-comparison/MySingleView.map

COMMON_ARGS=/p:Platform=iPhone /p:Configuration=Release
build-oldnet:
	$(SYSTEM_MSBUILD) $(XBUILD_VERBOSITY) size-comparison/MySingleView/oldnet/MySingleView.csproj $(COMMON_ARGS) /bl:$@.binlog $(MSBUILD_VERBOSITY)

build-dotnet: $(TARGETS)
	$(DOTNET) build size-comparison/MySingleView/dotnet/MySingleView.csproj --runtime ios-arm64 $(COMMON_ARGS) /bl:$@.binlog $(MSBUILD_VERBOSITY)

run-dotnet: $(TARGETS)
	$(DOTNET) build -t:Run size-comparison/MySingleView/dotnet/MySingleView.csproj --runtime ios-arm64 $(COMMON_ARGS) /bl:$@.binlog $(MSBUILD_VERBOSITY)

run-dotnet-sim: $(TARGETS)
	$(DOTNET) build -t:Run size-comparison/MySingleView/dotnet/MySingleView.csproj /p:Configuration=Release --runtime iossimulator-x64 /p:Platform=iPhoneSimulator /bl:$@.binlog

# this target will copy NuGet.config and global.json to the directories that need it for their .NET build to work correctly.
copy-dotnet-config: $(TARGETS)
	$(Q) for dir in $(abspath $(TOP))/tests/*/dotnet $(abspath $(TOP))/tests/linker/*/*/dotnet; do \
		$(CP) -c NuGet.config global.json "$$dir"; \
	done
	$(Q) $(CP) -c NuGet.config global.json $(TOP)/external/Touch.Unit/Touch.Client/dotnet
	$(Q) $(CP) -c NuGet.config global.json $(TOP)/external/MonoTouch.Dialog/MonoTouch.Dialog/dotnet
