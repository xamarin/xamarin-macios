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

DIRECTORIES += \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/Sdk \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/Sdk \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/Sdk \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/lib/Xamarin.iOS \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/lib/Xamarin.TVOS \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/lib/Xamarin.WatchOS \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/Sdk \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/lib/Xamarin.Mac \

IOS_TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/Sdk/Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/Sdk/Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/targets/Microsoft.iOS.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/targets/Microsoft.iOS.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/targets/Microsoft.iOS.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/targets/Microsoft.iOS.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/targets/Xamarin.Shared.Sdk.props \

TVOS_TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/Sdk/Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/Sdk/Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/targets/Microsoft.tvOS.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/targets/Microsoft.tvOS.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/targets/Microsoft.tvOS.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/targets/Microsoft.tvOS.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/targets/Xamarin.Shared.Sdk.props \

WATCHOS_TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/Sdk/Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/Sdk/Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/targets/Microsoft.watchOS.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/targets/Microsoft.watchOS.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/targets/Microsoft.watchOS.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/targets/Microsoft.watchOS.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/targets/Xamarin.Shared.Sdk.props \

MACOS_TARGETS = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/Sdk/Sdk.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/Sdk/Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/targets/Microsoft.macOS.TargetFrameworkInference.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/targets/Microsoft.macOS.Sdk.DefaultItems.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/targets/Microsoft.macOS.Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/targets/Microsoft.macOS.Sdk.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/targets/Xamarin.Shared.Sdk.props \

TARGETS += $(IOS_TARGETS) $(TVOS_TARGETS) $(WATCHOS_TARGETS) $(MACOS_TARGETS)

install-symlink:
	DOTNET_DESTDIR=$(DOTNET_DESTDIR) IOS_TARGETDIR=$(IOS_TARGETDIR) MAC_TARGETDIR=$(MAC_TARGETDIR) ./install-into-dotnet.sh

install-local::

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/%: % | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/%: % | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.iOS.Sdk/targets/Xamarin.Shared.%: targets/Xamarin.Shared.% | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.tvOS.Sdk/targets/Xamarin.Shared.%: targets/Xamarin.Shared.% | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Microsoft.watchOS.Sdk/targets/Xamarin.Shared.%: targets/Xamarin.Shared.% | $(DIRECTORIES)
	$(Q) $(CP) $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Microsoft.macOS.Sdk/targets/Xamarin.Shared.%: targets/Xamarin.Shared.% | $(DIRECTORIES)
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

all-local:: $(TARGETS) targets/Xamarin.Shared.Sdk.Versions.props

pack: ref-pack sdk-pack runtime-pack

# just a temporary target while debugging for faster turnaround
packsimple: prepare
	$(Q) rm -Rf nupkgs/*
	$(Q) $(MAKE) ref-pack-iOS sdk-pack-iOS nupkgs/Microsoft.iOS.Runtime.ios-x64.nupkg nupkgs/Microsoft.iOS.Runtime.ios-arm64.nupkg

CURRENT_IOS_VERSION=$(shell grep 'iOSPackageVersion:'     targets/Xamarin.Shared.Sdk.Versions.props | sed -e 's/.*PackageVersion://g' -e 's/.sha.*//' -e 's/[[:space:]]//g')
CURRENT_TVOS_VERSION=$(shell grep 'tvOSPackageVersion:'    targets/Xamarin.Shared.Sdk.Versions.props | sed -e 's/.*PackageVersion://g' -e 's/.sha.*//' -e 's/[[:space:]]//g')
CURRENT_WATCHOS_VERSION=$(shell grep 'watchOSPackageVersion:' targets/Xamarin.Shared.Sdk.Versions.props | sed -e 's/.*PackageVersion://g' -e 's/.sha.*//' -e 's/[[:space:]]//g')
CURRENT_MACOS_VERSION=$(shell grep 'macOSPackageVersion:'   targets/Xamarin.Shared.Sdk.Versions.props | sed -e 's/.*PackageVersion://g' -e 's/.sha.*//' -e 's/[[:space:]]//g')

nupkgs/Microsoft.iOS.%.nupkg: CURRENT_VERSION=$(CURRENT_IOS_VERSION)
nupkgs/Microsoft.tvOS.%.nupkg: CURRENT_VERSION=$(CURRENT_TVOS_VERSION)
nupkgs/Microsoft.watchOS.%.nupkg: CURRENT_VERSION=$(CURRENT_WATCHOS_VERSION)
nupkgs/Microsoft.macOS.%.nupkg: CURRENT_VERSION=$(CURRENT_MACOS_VERSION)

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

runtime-pack-iOS: $(foreach rid,$(IOS_RIDS),nupkgs/Microsoft.iOS.Runtime.ios-$(rid).nupkg)
runtime-pack-tvOS: $(foreach rid,$(TVOS_RIDS),nupkgs/Microsoft.tvOS.Runtime.tvos-$(rid).nupkg)
runtime-pack-watchOS: $(foreach rid,$(WATCHOS_RIDS),nupkgs/Microsoft.watchOS.Runtime.watchos-$(rid).nupkg)
runtime-pack-macOS: $(foreach rid,$(MACOS_RIDS),nupkgs/Microsoft.macOS.Runtime.osx-$(rid).nupkg)

ref-pack-%: nupkgs/Microsoft.%.Ref.nupkg;
ref-pack: $(foreach var,$(PLATFORMS),nupkgs/Microsoft.$(var).Ref.nupkg)
sdk-pack-%: nupkgs/Microsoft.%.Sdk.nupkg;
sdk-pack: $(foreach var,$(PLATFORMS),nupkgs/Microsoft.$(var).Sdk.nupkg)
runtime-pack: $(foreach var,$(PLATFORMS),runtime-pack-$(var))

pack-%:
	$(MAKE) ref-pack-$* runtime-pack-$* sdk-pack-$*

test/NuGet.config: $(TOP)/NuGet.config Makefile
	$(Q) $(CP) $< $@.tmp
	$(Q) nuget sources add -Name local-dotnet-feed -Source $(abspath $(CURDIR)/$(TEST_FEED_PATH)) -ConfigFile $@.tmp
	$(Q) mv $@.tmp $@

test/global.json: Makefile $(TEMPLATED_FILES)
	$(Q_GEN) \
		printf "{\n" > $@; \
		printf "\t\"sdk\": { \"version\": \"5.*\" },\n" >> $@; \
		printf "\t\"msbuild-sdks\": {\n" >> $@; \
		printf "\t\t\"Microsoft.iOS.Sdk\": \"$(CURRENT_IOS_VERSION)\",\n" >> $@; \
		printf "\t\t\"Microsoft.tvOS.Sdk\": \"$(CURRENT_TVOS_VERSION)\",\n" >> $@; \
		printf "\t\t\"Microsoft.watchOS.Sdk\": \"$(CURRENT_WATCHOS_VERSION)\",\n" >> $@; \
		printf "\t\t\"Microsoft.macOS.Sdk\": \"$(CURRENT_MACOS_VERSION)\",\n" >> $@; \
		printf "\t}\n}\n" >> $@

test-nuget: test/NuGet.config test/global.json
	$(Q) $(MAKE) packsimple
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
