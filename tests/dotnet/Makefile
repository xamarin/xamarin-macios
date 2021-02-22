TOP=../..

include $(TOP)/Make.config

# This is a temporary variable to enable the .NET workload resolver, because it's opt-in for now.
# Ref: https://github.com/dotnet/sdk/issues/13849
export MSBuildEnableWorkloadResolver=true

# This tells NuGet to use the nupkgs we're building locally,
# and to put any extracted packages in the 'packages' directory (to not clutter up ~/.nuget/packages)
NuGet.config: $(TOP)/NuGet.config Makefile
	$(Q) $(CP) $< $@.tmp
	$(Q) nuget sources add -Name local-dotnet-feed -Source $(abspath $(DOTNET_FEED_DIR)) -ConfigFile $@.tmp
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
