TOP=../..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

BUILD_DIR=bin/Debug/net7.0

DOTNET_TARGETS += \
	$(BUILD_DIR)/dotnet-linker.dll \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/dotnet-linker/dotnet-linker.dll) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/dotnet-linker/dotnet-linker.pdb) \

DOTNET_DIRECTORIES += \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/dotnet-linker) \

# dotnet-linker.csproj.inc contains the dotnet_linker_dependencies variable used to determine if mtouch needs to be rebuilt or not.
dotnet-linker.csproj.inc: export BUILD_EXECUTABLE=$(DOTNET) build
dotnet-linker.csproj.inc: export BUILD_VERBOSITY=$(DOTNET_BUILD_VERBOSITY)
dotnet-linker.csproj.inc: dotnet-linker.csproj
-include dotnet-linker.csproj.inc

$(BUILD_DIR)/dotnet-linker%dll $(BUILD_DIR)/dotnet-linker%pdb: Makefile $(dotnet_linker_dependencies)
	$(Q_DOTNET_BUILD) $(DOTNET) build dotnet-linker.csproj $(DOTNET_BUILD_VERBOSITY)
	$(Q) touch $(BUILD_DIR)/dotnet-linker.dll $(BUILD_DIR)/dotnet-linker.pdb

define InstallTemplate
$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/dotnet-linker/%: $(BUILD_DIR)/% | $(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/dotnet-linker
	$$(Q) $$(CP) $$< $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call InstallTemplate,$(platform))))

$(DOTNET_DIRECTORIES):
	$(Q) mkdir -p $@

all-local:: $(DOTNET_TARGETS)
install-local:: $(DOTNET_TARGETS)

vscode-maccatalyst:
	cp .vscode/launch-maccatalyst.json .vscode/launch.json
	$(MAKE) vscode-prepare

vscode-ios-arm64:
	cp .vscode/launch-ios-arm64.json .vscode/launch.json
	$(MAKE) vscode-prepare

vscode-prepare:
	sed -i '' s_/dotnet-sdk-[0-9a-zA-Z.-]*/_/dotnet-sdk-$(DOTNET_VERSION)/_ .vscode/launch.json
	sed -i '' s_/Debug/_/ManagedStaticRegistrar/_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/sdk/.*/Sdks/Microsoft.NET.ILLink.Tasks/tools/$(DOTNET_TFM)/illink.dll_$(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/sdk/*/Sdks/Microsoft.NET.ILLink.Tasks/tools/$(DOTNET_TFM)/illink.dll))_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.NETCore.App.Runtime.Mono.maccatalyst-x64/.*/runtimes/maccatalyst-x64/lib/$(DOTNET_TFM)/_$(dir $(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.NETCore.App.Runtime.Mono.maccatalyst-x64/*/runtimes/maccatalyst-x64/lib/$(DOTNET_TFM)/System.AppContext.dll)))_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.NETCore.App.Runtime.Mono.maccatalyst-x64/.*/runtimes/maccatalyst-x64/native/_$(dir $(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.NETCore.App.Runtime.Mono.maccatalyst-x64/7*/runtimes/maccatalyst-x64/native/System.Private.CoreLib.dll)))_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.NETCore.App.Runtime.Mono.ios-arm64/.*/runtimes/ios-arm64/lib/$(DOTNET_TFM)/_$(dir $(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.NETCore.App.Runtime.Mono.ios-arm64/*/runtimes/ios-arm64/lib/$(DOTNET_TFM)/System.AppContext.dll)))_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.NETCore.App.Runtime.Mono.ios-arm64/.*/runtimes/ios-arm64/native/_$(dir $(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.NETCore.App.Runtime.Mono.ios-arm64/7*/runtimes/ios-arm64/native/System.Private.CoreLib.dll)))_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.MacCatalyst.Sdk/.*/tools/dotnet-linker/dotnet-linker.dll_$(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.MacCatalyst.Sdk/*-*/tools/dotnet-linker/dotnet-linker.dll))_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.iOS.Sdk/.*/tools/dotnet-linker/dotnet-linker.dll_$(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.iOS.Sdk/*-*/tools/dotnet-linker/dotnet-linker.dll))_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.tvOS.Sdk/.*/tools/dotnet-linker/dotnet-linker.dll_$(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.tvOS.Sdk/*-*/tools/dotnet-linker/dotnet-linker.dll))_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.macOS.Sdk/.*/tools/dotnet-linker/dotnet-linker.dll_$(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.macOS.Sdk/*-*/tools/dotnet-linker/dotnet-linker.dll))_ .vscode/launch.json

	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.iOS.Runtime.ios-arm64/.*/runtimes/ios-arm64/lib/net7.0/Microsoft.iOS.dll_$(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.iOS.Runtime.ios-arm64/*/runtimes/ios-arm64/lib/$(DOTNET_TFM)/Microsoft.iOS.dll))_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.iOS.Runtime.iossimulator-x64/.*/runtimes/iossimulator-x64/lib/net7.0/Microsoft.iOS.dll_$(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.iOS.Runtime.iossimulator-x64/*/runtimes/iossimulator-x64/lib/$(DOTNET_TFM)/Microsoft.iOS.dll))_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.MacCatalyst.Runtime.maccatalyst-x64/.*/runtimes/maccatalyst-x64/lib/net7.0/Microsoft.MacCatalyst.dll_$(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.MacCatalyst.Runtime.maccatalyst-x64/*/runtimes/maccatalyst-x64/lib/$(DOTNET_TFM)/Microsoft.MacCatalyst.dll))_ .vscode/launch.json
	sed -i '' s_/Users/rolf/work/maccore/nativeaot/xamarin-macios/builds/downloads/dotnet-sdk-[0-9a-zA-Z.-]*/packs/Microsoft.MacCatalyst.Runtime.maccatalyst-arm64/.*/runtimes/maccatalyst-arm64/lib/net7.0/Microsoft.MacCatalyst.dll_$(abspath $(wildcard $(TOP)/builds/downloads/dotnet-sdk-$(DOTNET_VERSION)/packs/Microsoft.MacCatalyst.Runtime.maccatalyst-arm64/*/runtimes/maccatalyst-arm64/lib/$(DOTNET_TFM)/Microsoft.MacCatalyst.dll))_ .vscode/launch.json

vscode: vscode-prepare
	PATH=$(DOTNET_DIR):$(PATH) code .
