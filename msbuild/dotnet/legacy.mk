LEGACY_DIRECTORIES += \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/Sdk \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/Sdk \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/Sdk \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/lib/Xamarin.iOS \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/lib/Xamarin.TVOS \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/lib/Xamarin.WatchOS \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/RedistList \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.tvOS/RedistList \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.watchOS/RedistList \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/Sdk \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/RedistList \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/lib/Xamarin.Mac \

LEGACY_IOS_TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/Sdk/Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/Sdk/Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets/Xamarin.iOS.Legacy.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets/Xamarin.iOS.Legacy.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets/Xamarin.iOS.Legacy.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets/Xamarin.iOS.Legacy.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.Versions.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/RedistList/FrameworkList.xml \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/lib/Xamarin.iOS/v1.0 \

LEGACY_TVOS_TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/Sdk/Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/Sdk/Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets/Xamarin.tvOS.Legacy.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets/Xamarin.tvOS.Legacy.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets/Xamarin.tvOS.Legacy.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets/Xamarin.tvOS.Legacy.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.Versions.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/RedistList/FrameworkList.xml \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/lib/Xamarin.TVOS/v1.0 \

LEGACY_WATCHOS_TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/Sdk/Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/Sdk/Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets/Xamarin.watchOS.Legacy.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets/Xamarin.watchOS.Legacy.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets/Xamarin.watchOS.Legacy.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets/Xamarin.watchOS.Legacy.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.Versions.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/RedistList/FrameworkList.xml \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/lib/Xamarin.WatchOS/v1.0 \

LEGACY_MACOS_TARGETS = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/Sdk/Sdk.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/Sdk/Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets/Xamarin.macOS.Legacy.Sdk.TargetFrameworkInference.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets/Xamarin.macOS.Legacy.Sdk.DefaultItems.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets/Xamarin.macOS.Legacy.Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets/Xamarin.macOS.Legacy.Sdk.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.TargetFrameworkInference.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.DefaultItems.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.DefaultItems.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.Sdk.Versions.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/RedistList/FrameworkList.xml \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/lib/Xamarin.Mac/v1.0 \

LEGACY_TARGETS = $(LEGACY_IOS_TARGETS) $(LEGACY_TVOS_TARGETS) $(LEGACY_WATCHOS_TARGETS) $(LEGACY_MACOS_TARGETS)
TARGETS += $(LEGACY_TARGETS)
DIRECTORIES += $(LEGACY_DIRECTORIES)

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/RedistList/FrameworkList.xml: ../Xamarin.iOS.Tasks.Core/Xamarin.iOS-FrameworkList.xml.in Makefile | $(DIRECTORIES)
	$(Q) sed 's@%TargetFrameworkDirectory%@$(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS@' $< > $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/RedistList/FrameworkList.xml: ../Xamarin.iOS.Tasks.Core/Xamarin.tvOS-FrameworkList.xml.in Makefile | $(DIRECTORIES)
	$(Q) sed 's@%TargetFrameworkDirectory%@$(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS@' $< > $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/RedistList/FrameworkList.xml: ../Xamarin.iOS.Tasks.Core/Xamarin.watchOS-FrameworkList.xml.in Makefile | $(DIRECTORIES)
	$(Q) sed 's@%TargetFrameworkDirectory%@$(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS@' $< > $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/RedistList/FrameworkList.xml: ../Xamarin.Mac.Tasks/Xamarin.Mac-Mobile-FrameworkList.xml.in Makefile | $(DIRECTORIES)
	$(Q) sed 's@%TargetFrameworkDirectory%@$(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac@' $< > $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/lib/Xamarin.iOS/v1.0: Makefile
	ln -Fhs $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/lib/Xamarin.TVOS/v1.0: Makefile
	ln -Fhs $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/lib/Xamarin.WatchOS/v1.0: Makefile
	ln -Fhs $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/lib/Xamarin.Mac/v1.0: Makefile
	ln -Fhs $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.%: targets/Xamarin.Shared.Legacy.% | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.%: targets/Xamarin.Shared.Legacy.% | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.%: targets/Xamarin.Shared.Legacy.% | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Legacy.Sdk/targets/Xamarin.Shared.Legacy.%: targets/Xamarin.Shared.Legacy.% | $(DIRECTORIES)
	$(Q) $(CP) $< $@

targets/Xamarin.Shared.Legacy.Sdk.Versions.props: targets/Xamarin.Shared.Sdk.Versions.props
	$(Q) $(CP) $< $@

# just a temporary target while debugging for faster turnaround
legacy-pack-simple: legacy-prepare
	$(Q) mkdir -p nupkgs
	$(Q) $(MAKE) legacy-pack-iOS

legacy-pack: $(foreach var,$(PLATFORMS),nupkgs/Xamarin.$(var).Legacy.Sdk.nupkg)
$(foreach var,$(PLATFORMS),nupkgs/Xamarin.$(var).Legacy.Sdk.nupkg): legacy-prepare

test-legacy-nuget: test/NuGet.config test/global.json
	$(Q) $(MAKE) legacy-pack-simple
	@# Clear out anything from the nuget cache from previous tests
	$(Q) rm -Rf $(HOME)/.nuget/packages/xamarin.*.legacy.sdk
	@#$(if $(V),,@echo "BUILD    MySingleView.app";) $(DOTNET) build test/MySingleView/MySingleView.csproj /p:Platform=iPhone $(XBUILD_VERBOSITY)
	export MD_MTOUCH_SDK_ROOT=$(wildcard $(HOME)/.nuget/packages/xamarin.ios.legacy.sdk/13*/); \
	$(DOTNET) build test/MySingleView/MySingleView.csproj /p:Platform=iPhoneSimulator $(XBUILD_VERBOSITY)
	@#$(if $(V),,@echo "BUILD    MyCocoaApp.app";)   $(DOTNET) build test/MyCocoaApp/MyCocoaApp.csproj $(XBUILD_VERBOSITY)

legacy-prepare:
	$(Q) V=$(V) \
	TOP=$(TOP) \
	DOTNET_BCL_DIR=$(DOTNET_BCL_DIR) \
	MACOS_DOTNET_DESTDIR=$(MACOS_LEGACY_DOTNET_DESTDIR) \
	IOS_DOTNET_DESTDIR=$(IOS_LEGACY_DOTNET_DESTDIR) \
	TVOS_DOTNET_DESTDIR=$(TVOS_LEGACY_DOTNET_DESTDIR) \
	WATCHOS_DOTNET_DESTDIR=$(WATCHOS_LEGACY_DOTNET_DESTDIR) \
	MAC_DESTDIR=$(MAC_DESTDIR) \
	IOS_DESTDIR=$(IOS_DESTDIR) \
	MAC_FRAMEWORK_DIR=$(MAC_FRAMEWORK_DIR) \
	MONOTOUCH_PREFIX=$(MONOTOUCH_PREFIX) \
	DOTNET_IOS_SDK_DESTDIR=$(DOTNET_IOS_SDK_DESTDIR) \
	./build-legacy-nugets.sh
