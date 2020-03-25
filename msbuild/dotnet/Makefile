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
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.Shared.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/targets/Xamarin.Shared.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.iOS.Sdk/lib/Xamarin.iOS/v1.0 \

TVOS_TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/Sdk/Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/Sdk/Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.tvOS.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.tvOS.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.tvOS.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.tvOS.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.Shared.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/targets/Xamarin.Shared.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.tvOS.Sdk/lib/Xamarin.TVOS/v1.0 \

WATCHOS_TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/Sdk/Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/Sdk/Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.watchOS.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.watchOS.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.watchOS.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.watchOS.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.Shared.TargetFrameworkInference.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/targets/Xamarin.Shared.Sdk.props \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/dotnet/Xamarin.watchOS.Sdk/lib/Xamarin.WatchOS/v1.0 \

MACOS_TARGETS = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/Sdk/Sdk.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/Sdk/Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.macOS.TargetFrameworkInference.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.macOS.Sdk.DefaultItems.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.macOS.Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.macOS.Sdk.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.Shared.TargetFrameworkInference.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/targets/Xamarin.Shared.Sdk.props \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/dotnet/Xamarin.macOS.Sdk/lib/Xamarin.Mac/v1.0 \

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

all-local:: $(TARGETS)

pack: prepare
	$(Q) $(MAKE) $(foreach var,iOS tvOS watchOS macOS,pack-$(var)) $(foreach var,iOS tvOS watchOS macOS,ref-pack-$(var))

# just a temporary target while debugging for faster turnaround
pack-simple: prepare
	$(Q) rm -Rf nupkgs
	$(Q) mkdir -p nupkgs
	$(Q) $(MAKE) pack-iOS ref-pack-iOS


ref-pack-%: $(TEMPLATED_FILES)
	$(Q) rm -Rf $(TEST_FEED_PATH)/xamarin.$(shell echo $* | tr '[:upper:]' '[:lower:]').app.ref
	$(Q) rm -f nupkgs/Xamarin.$*.App.Ref.*.nupkg
	$(if $(V),,@echo "PACK     nupkgs/Xamarin.$*.App.Ref.nupkg";) $(DOTNET) pack package/$*/Xamarin.$*.App.ref.csproj --output nupkgs $(DOTNET_PACK_VERBOSITY)
	$(if $(V),,@echo "ADD      nupkgs/Xamarin.$*.App.Ref.nupkg";) nuget add nupkgs/Xamarin.$*.App.Ref.*nupkg -source $(TEST_FEED_PATH) $(NUGET_VERBOSITY)
	$(Q) echo "Created: $$(ls -1 nupkgs/Xamarin.$*.App.Ref.*nupkg)"

pack-%: $(TEMPLATED_FILES)
	$(Q) rm -Rf $(TEST_FEED_PATH)/xamarin.$(shell echo $* | tr '[:upper:]' '[:lower:]').sdk
	$(Q) rm -f nupkgs/Xamarin.$*.Sdk.*.nupkg
	$(if $(V),,@echo "PACK     nupkgs/Xamarin.$*.Sdk.nupkg";) $(DOTNET) pack package/$*/Xamarin.$*.csproj --output nupkgs $(DOTNET_PACK_VERBOSITY)
	$(if $(V),,@echo "ADD      nupkgs/Xamarin.$*.Sdk.nupkg";) nuget add nupkgs/Xamarin.$*.Sdk.*nupkg -source $(TEST_FEED_PATH) $(NUGET_VERBOSITY)
# 	@# Add the hash to the filename
# 	$(Q) CURRENT_HASH=$(shell grep '<CurrentHash>' package/versions.targets | sed -e 's/<[/]*CurrentHash>//g' -e 's/[[:space:]]//g'); \
# 	NAME=$$(echo nupkgs/Xamarin.$*.Sdk.*nupkg | sed -e 's/.nupkg$$//'); \
# 	mv "$$NAME.nupkg" "$$NAME+$$CURRENT_HASH.nupkg"
	$(Q) echo "Created: $$(ls -1 nupkgs/Xamarin.$*.Sdk.*nupkg)"

test/NuGet.config: $(TOP)/NuGet.config
	$(Q_CP) $< $@.tmp
	$(Q) nuget sources add -Name local-dotnet-feed -Source $(PWD)/$(TEST_FEED_PATH) -ConfigFile $@.tmp
	$(Q) mv $@.tmp $@

test/global.json: Makefile $(TEMPLATED_FILES)
	$(Q_GEN) \
		VERSIONS='$(shell msbuild package/versions.targets /t:PrintVersions /v:m /nologo)'; \
		XIPackageVersion=$$(echo $$VERSIONS | sed -e 's/.*XIPackageVersion: //' -e 's/ XMPackageVersion.*//'); \
		XMPackageVersion=$$(echo $$VERSIONS | sed -e 's/.*XMPackageVersion: //'); \
		printf "{\n\t\"msbuild-sdks\": {\n\t\t\"Xamarin.iOS.Sdk\": \"$$XIPackageVersion\",\n\t\t\"Xamarin.tvOS.Sdk\": \"$$XIPackageVersion\",\n\t\t\"Xamarin.watchOS.Sdk\": \"$$XIPackageVersion\",\n\t\t\"Xamarin.macOS.Sdk\": \"$$XMPackageVersion\"\n\t}\n}\n" > $@

test-nuget: test/NuGet.config test/global.json
	$(Q) $(MAKE) pack
	@# Clear out anything from the nuget cache from previous tests
	$(Q) rm -Rf $(HOME)/.nuget/packages/xamarin.*.sdk
	@#$(if $(V),,@echo "BUILD    MySingleView.app";) $(DOTNET) build test/MySingleView/MySingleView.csproj /p:Platform=iPhone $(XBUILD_VERBOSITY)
	$(if $(V),,@echo "BUILD    MySingleView.app";) $(DOTNET) build test/MySingleView/MySingleView.csproj /p:Platform=iPhoneSimulator $(XBUILD_VERBOSITY)
	@#$(if $(V),,@echo "BUILD    MyCocoaApp.app";)   $(DOTNET) build test/MyCocoaApp/MyCocoaApp.csproj $(XBUILD_VERBOSITY)

prepare:
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
	./build-nugets.sh
