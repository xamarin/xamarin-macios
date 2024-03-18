TOP=../../../..

include $(TOP)/Make.config

TARGETS += \
	.install-workloads.stamp \

all-local:: compare

compare compare-size: $(TARGETS)
	rm -rf ../../packages
	git clean -xfdq
	time $(MAKE) build-oldnet
	time $(MAKE) build-dotnet
	$(MAKE) report

PROJECT_OLD_NAME?=$(PROJECT)
PROJECT_NEW_NAME?=$(PROJECT)

PROJECT_OLD_FILE?=./oldnet/$(PROJECT_OLD_NAME).csproj
PROJECT_NEW_FILE?=./dotnet/$(PROJECT_NEW_NAME).csproj

PROJECT_OLD_APP?=./oldnet/bin/iPhone/Release/$(PROJECT_NEW_NAME).app
PROJECT_NEW_APP?=./dotnet/bin/iPhone/Release/$(DOTNET_TFM)-ios/ios-arm64/$(PROJECT).app

APPCOMPARE?=appcompare

report:
	$(APPCOMPARE) \
		$(abspath $(PROJECT_OLD_APP)) \
		$(abspath $(PROJECT_NEW_APP)) \
		--output-markdown $(abspath ./report.md) \
		--gist \
		--mapping-file $(abspath ./$(PROJECT).map) \

COMMON_ARGS=/p:Platform=iPhone /p:Configuration=Release $(MSBUILD_VERBOSITY)

build-oldnet: export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
build-oldnet: export MSBUILD_EXE_PATH=$(MONO_PREFIX)/lib/mono/msbuild/15.0/bin/MSBuild.dll
build-oldnet: export TargetFrameworkFallbackSearchPaths=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
build-oldnet: export MSBuildExtensionsPathFallbackPathsOverride=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
build-oldnet:
	$(SYSTEM_MSBUILD) $(PROJECT_OLD_FILE) $(COMMON_ARGS) /bl:$@.binlog /r

build-dotnet: $(TARGETS)
	$(DOTNET) build $(PROJECT_NEW_FILE) $(COMMON_ARGS) /bl:$@.binlog /p:RuntimeIdentifier=ios-arm64

run-dotnet: $(TARGETS)
	$(DOTNET) build -t:Run $(PROJECT_NEW_FILE) $(COMMON_ARGS) /p:RuntimeIdentifier=ios-arm64 /bl:$@.binlog

run-dotnet-sim:
	$(DOTNET) build -t:Run $(PROJECT_NEW_FILE) $(COMMON_ARGS) /p:RuntimeIdentifier=iossimulator-x64 /p:Platform=iPhoneSimulator /bl:$@.binlog

.install-workloads.stamp:
	$(DOTNET) workload install maui-maccatalyst --skip-manifest-update
	$(Q) touch $@
