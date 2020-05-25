TOP=../..

include $(TOP)/Make.config

# This tells NuGet to use the nupkgs we're building locally,
# and to put any extracted packages in the 'packages' directory (to not clutter up ~/.nuget/packages)
NuGet.config: $(TOP)/NuGet.config Makefile
	$(Q) $(CP) $< $@.tmp
	$(Q) nuget sources add -Name local-dotnet-feed -Source $(abspath $(DOTNET_FEED_DIR)) -ConfigFile $@.tmp
	$(Q) nuget sources add -Name dotnet5 -Source "https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet5/nuget/v3/index.json" -ConfigFile $@.tmp
	$(Q) nuget config -Set globalPackagesFolder=packages -Config $@.tmp
	$(Q) mv $@.tmp $@

# This tells NuGet to use the version we're building locally.
global.json: $(TOP)/Make.config.inc Makefile
	$(Q_GEN) \
		printf "{\n" > $@; \
		printf "\t\"sdk\": { \"version\": \"5.*\" },\n" >> $@; \
		printf "\t\"msbuild-sdks\": {\n" >> $@; \
		printf "\t\t\"Microsoft.iOS.Sdk\": \"$(IOS_NUGET_VERSION_NO_METADATA)\",\n" >> $@; \
		printf "\t\t\"Microsoft.tvOS.Sdk\": \"$(TVOS_NUGET_VERSION_NO_METADATA)\",\n" >> $@; \
		printf "\t\t\"Microsoft.watchOS.Sdk\": \"$(WATCHOS_NUGET_VERSION_NO_METADATA)\",\n" >> $@; \
		printf "\t\t\"Microsoft.macOS.Sdk\": \"$(MACOS_NUGET_VERSION_NO_METADATA)\"\n" >> $@; \
		printf "\t}\n}\n" >> $@

TARGETS += \
	NuGet.config \
	global.json \

all-local:: $(TARGETS)
