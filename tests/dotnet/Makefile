TOP=../..

include $(TOP)/Make.config

TARGETS += \

run-tests run-unit-tests:
	$(MAKE) -C UnitTests $@

all-local:: $(TARGETS)

reload: $(TARGETS)
	$(Q) $(MAKE) -C $(TOP) -j8 all
	$(Q) $(MAKE) -C $(TOP) -j8 install
	$(Q) $(DOTNET) build-server shutdown # make sure msbuild picks up any new task assemblies we built

compare compare-size: $(TARGETS)
	$(MAKE) -C size-comparison/MySingleView $@

report:
	$(MAKE) -C size-comparison/MySingleView $@

build-oldnet:
	$(MAKE) -C size-comparison/MySingleView $@

build-dotnet: $(TARGETS)
	$(MAKE) -C size-comparison/MySingleView $@

run-dotnet: $(TARGETS)
	$(DOTNET) build -t:Run size-comparison/MySingleView/dotnet/MySingleView.csproj --runtime ios-arm64 $(COMMON_ARGS) /bl:$@.binlog $(MSBUILD_VERBOSITY)

run-dotnet-sim: $(TARGETS)
	$(DOTNET) build -t:Run size-comparison/MySingleView/dotnet/MySingleView.csproj /p:Configuration=Release --runtime iossimulator-x64 /p:Platform=iPhoneSimulator /bl:$@.binlog
