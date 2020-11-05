TOP = ..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

DOTNET_MANIFESTS_PATH=$(DOTNET6_DIR)/sdk-manifests/$(DOTNET6_VERSION_BAND)
DOTNET_PACKS_PATH=$(DOTNET6_DIR)/packs
TMP_PKG_DIR=_pkg

# Create variables prefixed with the correctly cased platform name from the upper-cased platform name. This makes some of the next sections somewhat simpler.
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(platform)_NUGET_VERSION_NO_METADATA:=$($(shell echo $(platform) | tr a-z A-Z)_NUGET_VERSION_NO_METADATA)))

define DefineTargets
$(1)_NUGET_TARGETS = \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/Sdk/AutoImport.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/Sdk/Sdk.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.SupportedTargetPlatforms.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.DefaultItems.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.targets \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Xamarin.Shared.Sdk.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Xamarin.Shared.Sdk.targets \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Xamarin.Shared.Sdk.Versions.props
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DefineTargets,$(platform))))

DIRECTORIES += \
	$(DOTNET_NUPKG_DIR) \
	$(DOTNET_PKG_DIR) \
	$(DOTNET_FEED_DIR) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/Sdk) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets) \
	$(DOTNET_MANIFESTS_PATH) \
	$(DOTNET_PACKS_PATH) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_PACKS_PATH)/Microsoft.$(platform).Sdk) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_PACKS_PATH)/Microsoft.$(platform).Ref) \
	$(TMP_PKG_DIR) \

$(DIRECTORIES):
	$(Q) mkdir -p $@

CURRENT_HASH_LONG:=$(shell git log -1 --pretty=%H)

$(DOTNET_DESTDIR)/Microsoft.%: Microsoft.% | $(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/Sdk $(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets)
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.iOS.Sdk/targets/%: targets/% | $(DOTNET_DESTDIR)/Microsoft.iOS.Sdk/targets
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.tvOS.Sdk/targets/%: targets/% | $(DOTNET_DESTDIR)/Microsoft.tvOS.Sdk/targets
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.watchOS.Sdk/targets/%: targets/% | $(DOTNET_DESTDIR)/Microsoft.watchOS.Sdk/targets
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.macOS.Sdk/targets/%: targets/% | $(DOTNET_DESTDIR)/Microsoft.macOS.Sdk/targets
	$(Q) $(CP) $< $@

targets/%.props: targets/%.template.props Makefile $(TOP)/Make.config.inc $(TOP)/.git/HEAD $(TOP)/.git/index
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

define DefaultItems
Microsoft.$1.Sdk/targets/Microsoft.$1.Sdk.DefaultItems.props: targets/Microsoft.Sdk.DefaultItems.template.props Makefile
	$$(Q_GEN) sed \
		-e "s/@PLATFORM@/$1/g" \
		$$< > $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DefaultItems,$(platform))))

define SupportedTargetPlatforms
Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.SupportedTargetPlatforms.props: $(TOP)/Versions-ios.plist.in $(TOP)/Versions-mac.plist.in Makefile ./generate-target-platforms.csharp Makefile
	$(Q) rm -f $$@.tmp
	$(Q) ./generate-target-platforms.csharp $(1) $$@.tmp
	$(Q) mv $$@.tmp $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call SupportedTargetPlatforms,$(platform))))

define WorkloadTargets
Microsoft.NET.Workload.$(1)/WorkloadManifest.json: targets/WorkloadManifest.template.json Makefile $(TOP)/Make.config.inc $(TOP)/.git/HEAD $(TOP)/.git/index
	$$(Q) rm -f $$@.tmp
	$$(Q_GEN) sed \
		-e "s/@VERSION@/$3/g" \
		-e "s/@PLATFORM_LOWERCASE@/$2/g" \
		-e "s/@PLATFORM@/$1/g" \
		$$< > $$@.tmp
	$$(Q) mv $$@.tmp $$@

Microsoft.NET.Workload.$(1)/LICENSE: $(TOP)/LICENSE
	$$(Q) $(CP) $$< $$@

WORKLOAD_TARGETS += Microsoft.NET.Workload.$(1)/WorkloadManifest.json
WORKLOAD_TARGETS += Microsoft.NET.Workload.$(1)/LICENSE
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call WorkloadTargets,$(platform),$(shell echo $(platform) | tr A-Z a-z),$($(platform)_NUGET_VERSION_NO_METADATA))))

TEMPLATED_FILES = \
	targets/Xamarin.Shared.Sdk.Versions.props \
	$(foreach platform,$(DOTNET_PLATFORMS),Microsoft.$(platform).Sdk/targets/Microsoft.$(platform).Sdk.SupportedTargetPlatforms.props) \

nupkgs/$(IOS_NUGET).%.nupkg: CURRENT_VERSION_NO_METADATA=$(IOS_NUGET_VERSION_NO_METADATA)
nupkgs/$(TVOS_NUGET).%.nupkg: CURRENT_VERSION_NO_METADATA=$(TVOS_NUGET_VERSION_NO_METADATA)
nupkgs/$(WATCHOS_NUGET).%.nupkg: CURRENT_VERSION_NO_METADATA=$(WATCHOS_NUGET_VERSION_NO_METADATA)
nupkgs/$(MACOS_NUGET).%.nupkg: CURRENT_VERSION_NO_METADATA=$(MACOS_NUGET_VERSION_NO_METADATA)

nupkgs/$(IOS_NUGET).%.nupkg: CURRENT_VERSION_FULL=$(IOS_NUGET_VERSION_FULL)
nupkgs/$(TVOS_NUGET).%.nupkg: CURRENT_VERSION_FULL=$(TVOS_NUGET_VERSION_FULL)
nupkgs/$(WATCHOS_NUGET).%.nupkg: CURRENT_VERSION_FULL=$(WATCHOS_NUGET_VERSION_FULL)
nupkgs/$(MACOS_NUGET).%.nupkg: CURRENT_VERSION_FULL=$(MACOS_NUGET_VERSION_FULL)

# Create the nuget in a temporary directory (nupkgs/)
define CreateNuGetTemplate
nupkgs/$(1).$(2)+$(NUGET_BUILD_METADATA).nupkg: $(TEMPLATED_FILES) $(3) package/$(1)/package.csproj $(wildcard package/*.csproj) $(wildcard $(DOTNET_DESTDIR)/$(1)/* $(DOTNET_DESTDIR)/$(1)/*/* $(DOTNET_DESTDIR)/$(1)/*/*/* $(DOTNET_DESTDIR)/$(1)/*/*/*/*) global.json
	@# Delete any versions of the nuget we're building
	$$(Q) rm -f nupkgs/$(1).*.nupkg
	$$(Q_PACK) $(DOTNET6) pack package/$(1)/package.csproj --output "$$(dir $$@)" $(DOTNET_PACK_VERBOSITY)
	@# Nuget pack doesn't add the metadata to the filename, but we want that, so rename nuget to contain the full name
	$$(Q) mv "nupkgs/$(1).$(2).nupkg" "$$@"
	@# Clean the local feed
	$$(Q_NUGET_DEL) if test -d $(DOTNET_FEED_DIR)/$(shell echo $(1) | tr A-Z a-z)/$(2); then nuget delete $(1) $(2) -source $(abspath $(DOTNET_FEED_DIR)) -NonInteractive $(NUGET_VERBOSITY); fi
	@# Add the nupkg to our local feed
	$$(Q_NUGET_ADD) nuget add "$$@" -source $(DOTNET_FEED_DIR) -NonInteractive $(NUGET_VERBOSITY)
endef

# Create the NuGet packaging targets. It's amazing what make allows you to do...
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreateNuGetTemplate,Microsoft.$(platform).Sdk,$($(platform)_NUGET_VERSION_NO_METADATA),$($(platform)_NUGET_TARGETS))))
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreateNuGetTemplate,Microsoft.$(platform).Ref,$($(platform)_NUGET_VERSION_NO_METADATA))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call CreateNuGetTemplate,Microsoft.$(platform).Runtime.$(rid),$($(platform)_NUGET_VERSION_NO_METADATA)))))

# Copy the nuget from the temporary directory into the final directory
$(DOTNET_NUPKG_DIR)/%.nupkg: nupkgs/%.nupkg | $(DOTNET_NUPKG_DIR)
	$(Q) $(CP) $< $@

RUNTIME_PACKS_IOS = $(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS),$(DOTNET_NUPKG_DIR)/$(IOS_NUGET).Runtime.$(rid).$(IOS_NUGET_VERSION_FULL).nupkg)
RUNTIME_PACKS_TVOS = $(foreach rid,$(DOTNET_TVOS_RUNTIME_IDENTIFIERS),$(DOTNET_NUPKG_DIR)/$(TVOS_NUGET).Runtime.$(rid).$(TVOS_NUGET_VERSION_FULL).nupkg)
RUNTIME_PACKS_WATCHOS = $(foreach rid,$(DOTNET_WATCHOS_RUNTIME_IDENTIFIERS),$(DOTNET_NUPKG_DIR)/$(WATCHOS_NUGET).Runtime.$(rid).$(WATCHOS_NUGET_VERSION_FULL).nupkg)
RUNTIME_PACKS_MACOS = $(foreach rid,$(DOTNET_MACOS_RUNTIME_IDENTIFIERS),$(DOTNET_NUPKG_DIR)/$(MACOS_NUGET).Runtime.$(rid).$(MACOS_NUGET_VERSION_FULL).nupkg)
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

define InstallWorkload
$(DOTNET_MANIFESTS_PATH)/Microsoft.NET.Workload.$1: | $(DOTNET_MANIFESTS_PATH)
	$$(Q_LN) ln -Fhs $$(abspath Microsoft.NET.Workload.$1) $$(abspath $$@)

$(DOTNET_PACKS_PATH)/Microsoft.$1.Sdk/$2: | $(DOTNET_PACKS_PATH)/Microsoft.$1.Sdk
	$$(Q_LN) ln -Fhs $$(abspath $(DOTNET_DESTDIR)/Microsoft.$1.Sdk) $$(abspath $$@)

$(DOTNET_PACKS_PATH)/Microsoft.$1.Ref/$2: | $(DOTNET_PACKS_PATH)/Microsoft.$1.Ref
	$$(Q_LN) ln -Fhs $$(abspath $(DOTNET_DESTDIR)/Microsoft.$1.Ref) $$(abspath $$@)

WORKLOAD_TARGETS += \
	$(DOTNET_MANIFESTS_PATH)/Microsoft.NET.Workload.$1 \
	$(DOTNET_PACKS_PATH)/Microsoft.$1.Sdk/$2 \
	$(DOTNET_PACKS_PATH)/Microsoft.$1.Ref/$2
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call InstallWorkload,$(platform),$($(platform)_NUGET_VERSION_NO_METADATA))))

# We create three packages: one for the workload, one for the sdk package, and one for the ref package,
# and then bundle them into a single pkg for distribution (named Microsoft.<platform>.Bundle.<version>.pkg).
# This way we can create and publish separate updates for each workload/sdk/ref package later if we want to.
define CreatePackage
# The workload package
$(TMP_PKG_DIR)/Microsoft.$1.Workload.$2.pkg: $($(1)_NUGET_TARGETS) $(WORKLOAD_TARGETS) Makefile | $(TMP_PKG_DIR)
	$$(Q) rm -f $$@
	$$(Q_GEN) pkgbuild --quiet --version '$2' --root Microsoft.NET.Workload.$1 --component-plist PackageInfo.plist  --install-location /usr/local/share/dotnet/sdk-manifests/$(DOTNET6_VERSION_BAND)/Microsoft.NET.Workload.$1 --identifier com.microsoft.net.$3.workload.pkg $$@.tmp
	$$(Q) mv $$@.tmp $$@

# The sdk package
$(TMP_PKG_DIR)/Microsoft.$1.Sdk.$2.pkg: $(REF_PACK_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -f $$@
	$$(Q_GEN) pkgbuild --quiet --version '$2' --root $(DOTNET_DESTDIR)/Microsoft.$1.Sdk --component-plist PackageInfo.plist --install-location /usr/local/share/dotnet/packs/Microsoft.$1.Sdk/$2 --identifier com.microsoft.net.$3.sdk.pkg $$@.tmp
	$$(Q) mv $$@.tmp $$@

# The ref package
$(TMP_PKG_DIR)/Microsoft.$1.Ref.$2.pkg: $(SDK_PACK_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -f $$@
	$$(Q_GEN) pkgbuild --quiet --version '$2' --root $(DOTNET_DESTDIR)/Microsoft.$1.Ref --component-plist PackageInfo.plist --install-location /usr/local/share/dotnet/packs/Microsoft.$1.Ref/$2 --identifier com.microsoft.net.$3.ref.pkg $$@.tmp
	$$(Q) mv $$@.tmp $$@

# The final bundle package for distribution
$(TMP_PKG_DIR)/Microsoft.$1.Bundle.$2.pkg: $(TMP_PKG_DIR)/Microsoft.$1.Workload.$2.pkg $(TMP_PKG_DIR)/Microsoft.$1.Sdk.$2.pkg $(TMP_PKG_DIR)/Microsoft.$1.Ref.$2.pkg
	$$(Q) rm -f $$@
	$$(Q_GEN) productbuild \
		--quiet \
		--identifier com.microsoft.net.$3.pkg \
		--version '$2' \
		--package $(TMP_PKG_DIR)/Microsoft.$1.Workload.$2.pkg \
		--package $(TMP_PKG_DIR)/Microsoft.$1.Sdk.$2.pkg \
		--package $(TMP_PKG_DIR)/Microsoft.$1.Ref.$2.pkg \
		$$@.tmp
	$$(Q) mv $$@.tmp $$@

# Copy the bundle package from our temporary directory to the target directory
$(DOTNET_PKG_DIR)/%: $(TMP_PKG_DIR)/% | $(DOTNET_PKG_DIR)
	$$(Q) $(CP) $$< $$@

PACKAGE_TARGETS += $(DOTNET_PKG_DIR)/Microsoft.$1.Bundle.$2.pkg

$(TMP_PKG_DIR)/Microsoft.$1.Bundle.$2.zip: $($(1)_NUGET_TARGETS) $(WORKLOAD_TARGETS) Makefile $(REF_PACK_$(4)) $(SDK_PACK_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -rf $$@ $$@.tmpdir $$@.tmp
	$$(Q) mkdir -p $$@.tmpdir/dotnet/sdk-manifests/$(DOTNET6_VERSION_BAND)/
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/Microsoft.$1.Sdk
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/Microsoft.$1.Ref
	$$(Q) $(CP) -r Microsoft.NET.Workload.$1 $$@.tmpdir/dotnet/sdk-manifests/$(DOTNET6_VERSION_BAND)/
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/Microsoft.$1.Sdk $$@.tmpdir/dotnet/packs/Microsoft.$1.Sdk/$2
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/Microsoft.$1.Ref $$@.tmpdir/dotnet/packs/Microsoft.$1.Ref/$2
	$$(Q_GEN) cd $$@.tmpdir && zip -9rq $$(abspath $$@.tmp) .
	$$(Q) mv $$@.tmp $$@
	$$(Q) echo Created $$@

PACKAGE_TARGETS += $(DOTNET_PKG_DIR)/Microsoft.$1.Bundle.$2.zip
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreatePackage,$(platform),$($(platform)_NUGET_VERSION_NO_METADATA),$(shell echo $(platform) | tr A-Z a-z),$(shell echo $(platform) | tr a-z A-Z))))

define CreateMsi
$(TMP_PKG_DIR)/Microsoft.NET.Workload.$1.$2.wsx: ./generate-wix.csharp Makefile $(TMP_PKG_DIR)/Microsoft.$1.Bundle.$2.zip
	$$(Q_GEN) ./generate-wix.csharp "$1" "$$@" "$(TMP_PKG_DIR)/Microsoft.$1.Bundle.$2.zip.tmpdir/dotnet" "$2"

$(TMP_PKG_DIR)/Microsoft.NET.Workload.$1.$2.msi: $(TMP_PKG_DIR)/Microsoft.NET.Workload.$1.$2.wsx .stamp-check-wixl
	$$(Q_GEN) wixl -o "$$@" "$$<" -a x64

MSI_TARGETS += $(DOTNET_PKG_DIR)/Microsoft.NET.Workload.$1.$2.msi
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreateMsi,$(platform),$($(platform)_NUGET_VERSION_NO_METADATA))))

.stamp-check-wixl:
	$(Q) if ! type wixl; then \
		echo "Installing msitools to get wixl..."; \
		if ! brew install msitools; then \
			if ! type wixl; then \
				echo "Failed to install wixl"; \
				exit 1; \
			fi; \
		fi; \
		echo "Installed msitools"; \
	fi
	$(Q) touch $@

TARGETS += $(WORKLOAD_TARGETS)

msi: $(MSI_TARGETS)
package: $(PACKAGE_TARGETS) $(MSI_TARGETS)

ifdef ENABLE_DOTNET
all-local:: $(TARGETS) targets/Xamarin.Shared.Sdk.Versions.props
endif

# This tells NuGet to use the exact same dotnet version we've configured in Make.config
global.json: $(TOP)/global6.json
	$(CP) $< $@

clean-local::
	$(Q) rm -Rf $(DOTNET_NUPKG_DIR) $(DOTNET_FEED_DIR)
	$(Q) git clean -xfdq

.SECONDARY:
