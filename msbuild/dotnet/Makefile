TOP = ../..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

ifeq ($(V),)
DOTNET_PACK_VERBOSITY=--verbosity:quiet --nologo
NUGET_VERBOSITY=-verbosity quiet
else
DOTNET_PACK_VERBOSITY=--verbosity:detailed
NUGET_VERBOSITY=-verbosity detailed
endif

TEST_FEED_PATH=test/feed

PLATFORMS=iOS tvOS watchOS macOS

IOS_RIDS=arm64 arm x86 x64
TVOS_RIDS=arm64 x64
WATCHOS_RIDS=arm x86
MACOS_RIDS=x64

include legacy.mk

DIRECTORIES += \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/Sdk \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/Sdk \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/Sdk \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/lib/Xamarin.iOS \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/lib/Xamarin.TVOS \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/lib/Xamarin.WatchOS \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/Sdk \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/lib/Xamarin.Mac \

IOS_TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/Sdk/Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/Sdk/Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.iOS.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.iOS.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.iOS.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.iOS.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.Shared.Sdk.props \

TVOS_TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/Sdk/Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/Sdk/Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.tvOS.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.tvOS.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.tvOS.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.tvOS.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.Shared.Sdk.props \

WATCHOS_TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/Sdk/Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/Sdk/Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.watchOS.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.watchOS.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.watchOS.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.watchOS.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.Shared.Sdk.props \

MACOS_TARGETS = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/Sdk/Sdk.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/Sdk/Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.macOS.TargetFrameworkInference.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.macOS.Sdk.DefaultItems.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.macOS.Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.macOS.Sdk.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.Shared.Sdk.props \

TARGETS += $(IOS_TARGETS) $(TVOS_TARGETS) $(WATCHOS_TARGETS) $(MACOS_TARGETS)

install-symlink:
	DOTNET_DESTDIR=$(DOTNET_DESTDIR) IOS_TARGETDIR=$(IOS_TARGETDIR) MAC_TARGETDIR=$(MAC_TARGETDIR) ./install-into-dotnet.sh

install-local::

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/%: % | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/%: % | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.Shared.%: targets/Xamarin.Shared.% | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.Shared.%: targets/Xamarin.Shared.% | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.Shared.%: targets/Xamarin.Shared.% | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.Shared.%: targets/Xamarin.Shared.% | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(DIRECTORIES):
	$(Q) mkdir -p $@

CURRENT_HASH:=$(shell git log -1 --pretty=%h)
targets/%.props: targets/%.template.props Makefile $(TOP)/Make.config.inc
	$(Q_GEN) sed \
		-e "s/@IOS_PACKAGE_VERSION_MAJOR@/$(IOS_PACKAGE_VERSION_MAJOR)/g" \
		-e "s/@IOS_PACKAGE_VERSION_MINOR@/$(IOS_PACKAGE_VERSION_MINOR)/g" \
		-e "s/@IOS_PACKAGE_VERSION_REV@/$(IOS_PACKAGE_VERSION_REV)/g" \
		-e "s/@TVOS_PACKAGE_VERSION_MAJOR@/$(TVOS_PACKAGE_VERSION_MAJOR)/g" \
		-e "s/@TVOS_PACKAGE_VERSION_MINOR@/$(TVOS_PACKAGE_VERSION_MINOR)/g" \
		-e "s/@TVOS_PACKAGE_VERSION_REV@/$(TVOS_PACKAGE_VERSION_REV)/g" \
		-e "s/@WATCHOS_PACKAGE_VERSION_MAJOR@/$(WATCHOS_PACKAGE_VERSION_MAJOR)/g" \
		-e "s/@WATCHOS_PACKAGE_VERSION_MINOR@/$(WATCHOS_PACKAGE_VERSION_MINOR)/g" \
		-e "s/@WATCHOS_PACKAGE_VERSION_REV@/$(WATCHOS_PACKAGE_VERSION_REV)/g" \
		-e "s/@MACOS_PACKAGE_VERSION_MAJOR@/$(MACOS_PACKAGE_VERSION_MAJOR)/g" \
		-e "s/@MACOS_PACKAGE_VERSION_MINOR@/$(MACOS_PACKAGE_VERSION_MINOR)/g" \
		-e "s/@MACOS_PACKAGE_VERSION_REV@/$(MACOS_PACKAGE_VERSION_REV)/g" \
		-e 's/@IOS_COMMIT_DISTANCE@/$(IOS_COMMIT_DISTANCE)/g' \
		-e 's/@TVOS_COMMIT_DISTANCE@/$(TVOS_COMMIT_DISTANCE)/g' \
		-e 's/@WATCHOS_COMMIT_DISTANCE@/$(WATCHOS_COMMIT_DISTANCE)/g' \
		-e 's/@MACOS_COMMIT_DISTANCE@/$(MACOS_COMMIT_DISTANCE)/g' \
		-e "s/@CURRENT_BRANCH@/$(CURRENT_BRANCH_SED_ESCAPED)/g" \
		-e "s/@CURRENT_HASH@/$(CURRENT_HASH)/g" \
	$< > $@

TEMPLATED_FILES = \
	targets/Xamarin.Shared.Sdk.Versions.props \
	targets/Xamarin.Shared.Legacy.Sdk.Versions.props \

all-local:: $(TARGETS) targets/Xamarin.Shared.Sdk.Versions.props

pack: ref-pack sdk-pack runtime-pack

# just a temporary target while debugging for faster turnaround
packsimple: prepare
	$(Q) rm -Rf nupkgs
	$(Q) mkdir -p nupkgs
	$(Q) $(MAKE) pack-iOS ref-pack-iOS app-pack-iOS

CURRENT_IOS_VERSION=$(shell grep 'iOSPackageVersion:'     targets/Xamarin.Shared.Sdk.Versions.props | sed -e 's/.*PackageVersion://g' -e 's/.sha.*//' -e 's/[[:space:]]//g')
CURRENT_TVOS_VERSION=$(shell grep 'tvOSPackageVersion:'    targets/Xamarin.Shared.Sdk.Versions.props | sed -e 's/.*PackageVersion://g' -e 's/.sha.*//' -e 's/[[:space:]]//g')
CURRENT_WATCHOS_VERSION=$(shell grep 'watchOSPackageVersion:' targets/Xamarin.Shared.Sdk.Versions.props | sed -e 's/.*PackageVersion://g' -e 's/.sha.*//' -e 's/[[:space:]]//g')
CURRENT_MACOS_VERSION=$(shell grep 'macOSPackageVersion:'   targets/Xamarin.Shared.Sdk.Versions.props | sed -e 's/.*PackageVersion://g' -e 's/.sha.*//' -e 's/[[:space:]]//g')

nupkgs/Xamarin.iOS.%.nupkg: CURRENT_VERSION=$(CURRENT_IOS_VERSION)
nupkgs/Xamarin.tvOS.%.nupkg: CURRENT_VERSION=$(CURRENT_TVOS_VERSION)
nupkgs/Xamarin.watchOS.%.nupkg: CURRENT_VERSION=$(CURRENT_WATCHOS_VERSION)
nupkgs/Xamarin.macOS.%.nupkg: CURRENT_VERSION=$(CURRENT_MACOS_VERSION)

nupkgs/%.nupkg: prepare $(TEMPLATED_FILES)
	$(Q) rm -f $@ nupkgs/$*.*.nupkg
	$(Q) rm -Rf $(TEST_FEED_PATH)/$(shell echo $* | tr '[:upper:]' '[:lower:]')
	$(Q) rm -Rf $(HOME)/.nuget/packages/$(shell echo $* | tr '[:upper:]' '[:lower:]')
	$(if $(V),,@echo "PACK     $@";) $(DOTNET) pack package/$*/package.csproj --output nupkgs $(DOTNET_PACK_VERBOSITY) /bl:package/$*/obj/$*.binlog
	@# Rename nuget to contain exact version
	$(Q) mv "nupkgs/$*.$(CURRENT_VERSION).nupkg" "nupkgs/$*.$(CURRENT_VERSION)+sha.$(CURRENT_HASH).nupkg"
	$(Q) echo "Created: nupkgs/$*+sha.$(CURRENT_HASH).nupkg"

install-nugets:
	$(Q) rm -Rf $(TEST_FEED_PATH)
	$(Q) mkdir -p $(TEST_FEED_PATH)
	$(Q) for nupkg in nupkgs/*.nupkg; do \
		echo "ADD `basename $$nupkg`"; \
		nuget add "$$nupkg" -source $(TEST_FEED_PATH) $(NUGET_VERBOSITY); \
	done

runtime-pack-iOS: $(foreach rid,$(IOS_RIDS),nupkgs/Xamarin.iOS.App.Runtime.ios-$(rid).nupkg)
runtime-pack-tvOS: $(foreach rid,$(TVOS_RIDS),nupkgs/Xamarin.tvOS.App.Runtime.tvos-$(rid).nupkg)
runtime-pack-watchOS: $(foreach rid,$(WATCHOS_RIDS),nupkgs/Xamarin.watchOS.App.Runtime.watchos-$(rid).nupkg)
runtime-pack-macOS: $(foreach rid,$(MACOS_RIDS),nupkgs/Xamarin.macOS.App.Runtime.macos-$(rid).nupkg)

ref-pack: $(foreach var,$(PLATFORMS),nupkgs/Xamarin.$(var).App.Ref.nupkg)
sdk-pack: $(foreach var,$(PLATFORMS),nupkgs/Xamarin.$(var).Sdk.nupkg)
runtime-pack: $(foreach var,$(PLATFORMS),runtime-pack-$(var))

test/NuGet.config: $(TOP)/NuGet.config Makefile
	$(Q) $(CP) $< $@.tmp
	$(Q) nuget sources add -Name local-dotnet-feed -Source $(abspath $(CURDIR)/$(TEST_FEED_PATH)) -ConfigFile $@.tmp
	$(Q) mv $@.tmp $@

test/global.json: Makefile $(TEMPLATED_FILES)
	$(Q_GEN) \
		printf "{\n" > $@; \
		printf "\t\"sdk\": { \"version\": \"5.0.100-dev\" },\n" >> $@; \
		printf "\t\"msbuild-sdks\": {\n" >> $@; \
		printf "\t\t\"Xamarin.iOS.Sdk\": \"$(CURRENT_IOS_VERSION)\",\n" >> $@; \
		printf "\t\t\"Xamarin.tvOS.Sdk\": \"$(CURRENT_TVOS_VERSION)\",\n" >> $@; \
		printf "\t\t\"Xamarin.watchOS.Sdk\": \"$(CURRENT_WATCHOS_VERSION)\",\n" >> $@; \
		printf "\t\t\"Xamarin.macOS.Sdk\": \"$(CURRENT_MACOS_VERSION)\",\n" >> $@; \
		printf "\t\t\"Xamarin.iOS.Legacy.Sdk\": \"$(CURRENT_IOS_VERSION)\",\n" >> $@; \
		printf "\t\t\"Xamarin.tvOS.Legacy.Sdk\": \"$(CURRENT_TVOS_VERSION)\",\n" >> $@; \
		printf "\t\t\"Xamarin.watchOS.Legacy.Sdk\": \"$(CURRENT_WATCHOS_VERSION)\",\n" >> $@; \
		printf "\t\t\"Xamarin.macOS.Legacy.Sdk\": \"$(CURRENT_MACOS_VERSION)\"\n" >> $@; \
		printf "\t}\n}\n" >> $@

test-nuget: test/NuGet.config test/global.json
	$(Q) $(MAKE) legacy-pack-simple
	@# Clear out anything from the nuget cache from previous tests
	$(Q) rm -Rf $(HOME)/.nuget/packages/xamarin.*.sdk
	@#$(if $(V),,@echo "BUILD    MySingleView.app";) $(DOTNET) build test/MySingleView/MySingleView.csproj /p:Platform=iPhone $(XBUILD_VERBOSITY)
	$(if $(V),,@echo "BUILD    MySingleView.app";) $(DOTNET) build test/MySingleView/MySingleView.csproj /p:Platform=iPhoneSimulator $(XBUILD_VERBOSITY)
	@#$(if $(V),,@echo "BUILD    MyCocoaApp.app";)   $(DOTNET) build test/MyCocoaApp/MyCocoaApp.csproj $(XBUILD_VERBOSITY)

prepare:
	$(Q) V=$(V) \
	TOP=$(TOP) \
	DOTNET_DESTDIR=$(DOTNET_DESTDIR) \
	MAC_DESTDIR=$(MAC_DESTDIR) \
	IOS_DESTDIR=$(IOS_DESTDIR) \
	MAC_FRAMEWORK_DIR=$(MAC_FRAMEWORK_DIR) \
	MONOTOUCH_PREFIX=$(MONOTOUCH_PREFIX) \
	./build-nugets.sh
