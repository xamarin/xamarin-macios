TOP = ..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

PLATFORMS=iOS tvOS watchOS macOS

IOS_RIDS=$(subst ios-,,$(DOTNET_IOS_RUNTIME_IDENTIFIERS))
TVOS_RIDS=$(subst tvos-,,$(DOTNET_TVOS_RUNTIME_IDENTIFIERS))
WATCHOS_RIDS=$(subst watchos-,,$(DOTNET_WATCHOS_RUNTIME_IDENTIFIERS))
MACOS_RIDS=$(subst osx-,,$(DOTNET_MACOS_RUNTIME_IDENTIFIERS))

NUGET_TARGETS += \
	$(foreach platform,$(PLATFORMS), \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/Sdk/Sdk.targets \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/Sdk/Sdk.props \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets/Microsoft.$(platform).Sdk.DefaultItems.props \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets/Microsoft.$(platform).Sdk.props \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets/Microsoft.$(platform).Sdk.targets \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets/Microsoft.$(platform).TargetFrameworkInference.targets \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.props \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets/Xamarin.Shared.Sdk.props \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.targets \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets/Xamarin.Shared.Sdk.targets \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets/Xamarin.Shared.Sdk.Versions.props \
		$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets/Xamarin.Shared.Sdk.Versions.template.props \
	)

DIRECTORIES += \
	$(DOTNET_NUPKG_DIR) \
	$(DOTNET_FEED_DIR) \
	$(foreach platform,$(PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/Sdk) \
	$(foreach platform,$(PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets) \

$(DIRECTORIES):
	$(Q) mkdir -p $@

CURRENT_HASH_LONG:=$(shell git log -1 --pretty=%H)

$(DOTNET_DESTDIR)/Microsoft.%: Microsoft.% | $(foreach platform,$(PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/Sdk $(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets)
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.iOS.Sdk/targets/%: targets/% | $(DOTNET_DESTDIR)/Microsoft.iOS.Sdk/targets
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.tvOS.Sdk/targets/%: targets/% | $(DOTNET_DESTDIR)/Microsoft.tvOS.Sdk/targets
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.watchOS.Sdk/targets/%: targets/% | $(DOTNET_DESTDIR)/Microsoft.watchOS.Sdk/targets
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.macOS.Sdk/targets/%: targets/% | $(DOTNET_DESTDIR)/Microsoft.macOS.Sdk/targets
	$(Q) $(CP) $< $@

targets/%.props: targets/%.template.props Makefile $(TOP)/Make.config.inc
	$(Q_GEN) sed \
		-e "s/@IOS_NUGET_VERSION_NO_METADATA@/$(IOS_NUGET_VERSION_NO_METADATA)/g" \
		-e "s/@TVOS_NUGET_VERSION_NO_METADATA@/$(TVOS_NUGET_VERSION_NO_METADATA)/g" \
		-e "s/@WATCHOS_NUGET_VERSION_NO_METADATA@/$(WATCHOS_NUGET_VERSION_NO_METADATA)/g" \
		-e "s/@MACOS_NUGET_VERSION_NO_METADATA@/$(MACOS_NUGET_VERSION_NO_METADATA)/g" \
		-e "s/@IOS_NUGET_VERSION_FULL@/$(IOS_NUGET_VERSION_FULL)/g" \
		-e "s/@TVOS_NUGET_VERSION_FULL@/$(TVOS_NUGET_VERSION_FULL)/g" \
		-e "s/@WATCHOS_NUGET_VERSION_FULL@/$(WATCHOS_NUGET_VERSION_FULL)/g" \
		-e "s/@MACOS_NUGET_VERSION_FULL@/$(MACOS_NUGET_VERSION_FULL)/g" \
		-e "s/@CURRENT_BRANCH@/$(CURRENT_BRANCH_SED_ESCAPED)/g" \
		-e "s/@CURRENT_HASH_LONG@/$(CURRENT_HASH_LONG)/g" \
	$< > $@

TEMPLATED_FILES = \
	targets/Xamarin.Shared.Sdk.Versions.props \

nupkgs/$(IOS_NUGET).%.nupkg: CURRENT_VERSION_NO_METADATA=$(IOS_NUGET_VERSION_NO_METADATA)
nupkgs/$(TVOS_NUGET).%.nupkg: CURRENT_VERSION_NO_METADATA=$(TVOS_NUGET_VERSION_NO_METADATA)
nupkgs/$(WATCHOS_NUGET).%.nupkg: CURRENT_VERSION_NO_METADATA=$(WATCHOS_NUGET_VERSION_NO_METADATA)
nupkgs/$(MACOS_NUGET).%.nupkg: CURRENT_VERSION_NO_METADATA=$(MACOS_NUGET_VERSION_NO_METADATA)

nupkgs/$(IOS_NUGET).%.nupkg: CURRENT_VERSION_FULL=$(IOS_NUGET_VERSION_FULL)
nupkgs/$(TVOS_NUGET).%.nupkg: CURRENT_VERSION_FULL=$(TVOS_NUGET_VERSION_FULL)
nupkgs/$(WATCHOS_NUGET).%.nupkg: CURRENT_VERSION_FULL=$(WATCHOS_NUGET_VERSION_FULL)
nupkgs/$(MACOS_NUGET).%.nupkg: CURRENT_VERSION_FULL=$(MACOS_NUGET_VERSION_FULL)

# Create the nuget in a temporary directory (nupkgs/)
nupkgs/%.nupkg: $(TEMPLATED_FILES) $(NUGET_TARGETS)
	$(Q) rm -f $@
	$(Q_PACK) $(DOTNET5) pack package/$(shell echo $(notdir $@) | sed 's/[.]$(CURRENT_VERSION_FULL).*//')/package.csproj --output "$(dir $@)" $(DOTNET_PACK_VERBOSITY)
	@# Nuget pack doesn't add the metadata to the filename, but we want that, so rename nuget to contain the full name
	$(Q) mv "nupkgs/$(shell echo $(notdir $@) | sed -e 's/[+]$(NUGET_BUILD_METADATA)//')" "$@"
	@# Add the nupkg to our local feed
	$(Q_NUGET_ADD) nuget add "$@" -source $(DOTNET_FEED_DIR) $(NUGET_VERBOSITY)

# Copy the nuget from the temporary directory into the final directory
$(DOTNET_NUPKG_DIR)/%.nupkg: nupkgs/%.nupkg | $(DOTNET_NUPKG_DIR)
	$(Q) cp $< $@

RUNTIME_PACKS_IOS = $(foreach rid,$(IOS_RIDS),$(DOTNET_NUPKG_DIR)/$(IOS_NUGET).Runtime.ios-$(rid).$(IOS_NUGET_VERSION_FULL).nupkg)
RUNTIME_PACKS_TVOS = $(foreach rid,$(TVOS_RIDS),$(DOTNET_NUPKG_DIR)/$(TVOS_NUGET).Runtime.tvos-$(rid).$(TVOS_NUGET_VERSION_FULL).nupkg)
RUNTIME_PACKS_WATCHOS = $(foreach rid,$(WATCHOS_RIDS),$(DOTNET_NUPKG_DIR)/$(WATCHOS_NUGET).Runtime.watchos-$(rid).$(WATCHOS_NUGET_VERSION_FULL).nupkg)
RUNTIME_PACKS_MACOS = $(foreach rid,$(MACOS_RIDS),$(DOTNET_NUPKG_DIR)/$(MACOS_NUGET).Runtime.osx-$(rid).$(MACOS_NUGET_VERSION_FULL).nupkg)
RUNTIME_PACKS = $(RUNTIME_PACKS_IOS) $(RUNTIME_PACKS_TVOS) $(RUNTIME_PACKS_WATCHOS) $(RUNTIME_PACKS_MACOS)

REF_PACK_IOS = $(DOTNET_NUPKG_DIR)/$(IOS_NUGET).Ref.$(IOS_NUGET_VERSION_FULL).nupkg
REF_PACK_TVOS = $(DOTNET_NUPKG_DIR)/$(TVOS_NUGET).Ref.$(TVOS_NUGET_VERSION_FULL).nupkg
REF_PACK_WATCHOS = $(DOTNET_NUPKG_DIR)/$(WATCHOS_NUGET).Ref.$(WATCHOS_NUGET_VERSION_FULL).nupkg
REF_PACK_MACOS = $(DOTNET_NUPKG_DIR)/$(MACOS_NUGET).Ref.$(MACOS_NUGET_VERSION_FULL).nupkg
REF_PACKS = $(REF_PACK_IOS) $(REF_PACK_TVOS) $(REF_PACK_WATCHOS) $(REF_PACK_MACOS)

SDK_PACK_IOS = $(DOTNET_NUPKG_DIR)/$(IOS_NUGET).Sdk.$(IOS_NUGET_VERSION_FULL).nupkg
SDK_PACK_TVOS = $(DOTNET_NUPKG_DIR)/$(TVOS_NUGET).Sdk.$(TVOS_NUGET_VERSION_FULL).nupkg
SDK_PACK_WATCHOS = $(DOTNET_NUPKG_DIR)/$(WATCHOS_NUGET).Sdk.$(WATCHOS_NUGET_VERSION_FULL).nupkg
SDK_PACK_MACOS = $(DOTNET_NUPKG_DIR)/$(MACOS_NUGET).Sdk.$(MACOS_NUGET_VERSION_FULL).nupkg
SDK_PACKS = $(SDK_PACK_IOS) $(SDK_PACK_TVOS) $(SDK_PACK_WATCHOS) $(SDK_PACK_MACOS)

pack-ios: $(RUNTIME_PACKS_IOS) $(REF_PACK_IOS) $(SDK_PACK_IOS)
pack-tvos: $(RUNTIME_PACKS_TVOS) $(REF_PACK_TVOS) $(SDK_PACK_TVOS)
pack-watchos: $(RUNTIME_PACKS_WATCHOS) $(REF_PACK_WATCHOS) $(SDK_PACK_WATCHOS)
pack-macos: $(RUNTIME_PACKS_MACOS) $(REF_PACK_MACOS) $(SDK_PACK_MACOS)

TARGETS += $(RUNTIME_PACKS) $(REF_PACKS) $(SDK_PACKS)

ifdef ENABLE_DOTNET
all-local:: $(TARGETS) targets/Xamarin.Shared.Sdk.Versions.props
endif

clean-local::
	$(Q) rm -Rf $(DOTNET_NUPKG_DIR) $(DOTNET_FEED_DIR)
	$(Q) git clean -xfdq

.SECONDARY:
