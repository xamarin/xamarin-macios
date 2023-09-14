TOP = ..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

DOTNET_SDK_MANIFESTS_PATH=$(DOTNET_DIR)/sdk-manifests
DOTNET_PACKS_PATH=$(DOTNET_DIR)/packs
DOTNET_TEMPLATE_PACKS_PATH=$(DOTNET_DIR)/template-packs
TMP_PKG_DIR=_pkg

DOTNET_PLATFORMS_UPPERCASE:=$(shell echo $(DOTNET_PLATFORMS) | tr a-z A-Z)

# Create variables prefixed with the correctly cased platform name from the upper-cased platform name. This makes some of the next sections somewhat simpler.
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(platform)_NUGET_VERSION_NO_METADATA:=$($(shell echo $(platform) | tr a-z A-Z)_NUGET_VERSION_NO_METADATA)))
$(foreach platform,$(DOTNET_PLATFORMS),$(eval NET7_$(platform)_NUGET_VERSION_NO_METADATA:=$(NET7_$(shell echo $(platform) | tr a-z A-Z)_NUGET_VERSION_NO_METADATA)))

DOTNET_iOS_GLOBAL_USINGS=CoreGraphics Foundation UIKit
DOTNET_tvOS_GLOBAL_USINGS=CoreGraphics Foundation UIKit
DOTNET_MacCatalyst_GLOBAL_USINGS=CoreGraphics Foundation UIKit
DOTNET_macOS_GLOBAL_USINGS=AppKit CoreGraphics Foundation

define DefineTargets
$(1)_NUGET_TARGETS = \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/data/UnixFilePermissions.xml \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/Sdk/AutoImport.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/Sdk/Sdk.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/Sdk/Sdk-error.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/Sdk/Sdk-eol.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/Microsoft.$(1).Sdk.ImplicitNamespaceImports.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/Microsoft.$(1).Sdk.SupportedTargetPlatforms.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/Microsoft.$(1).Sdk.DefaultItems.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/Microsoft.$(1).Sdk.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/Microsoft.$(1).Sdk.Versions.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/Microsoft.$(1).Sdk.targets \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/Xamarin.Shared.Sdk.Publish.targets \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/Xamarin.Shared.Sdk.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/Xamarin.Shared.Sdk.targets \

endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DefineTargets,$(platform))))

define DefineWindowsTargets
$(1)_WINDOWS_NUGET_TARGETS = \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_WINDOWS_SDK_NAME)/Sdk/Sdk.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_WINDOWS_SDK_NAME)/targets/Microsoft.$(1).Windows.Sdk.props \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_WINDOWS_SDK_NAME)/targets/Microsoft.$(1).Windows.Sdk.targets \

endef
$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(eval $(call DefineWindowsTargets,$(platform))))

ifdef INCLUDE_HOTRESTART

iOS_WINDOWS_NUGET_TARGETS += \
	$(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip \

endif

DIRECTORIES += \
	$(DOTNET_NUPKG_DIR) \
	$(DOTNET_PKG_DIR) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/data) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/Sdk) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/targets) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Templates) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.NET.Sdk.$(platform)) \
	$(DOTNET_SDK_MANIFESTS_PATH)/$(MACIOS_MANIFEST_VERSION_BAND) \
	$(DOTNET_PACKS_PATH) \
	$(DOTNET_TEMPLATE_PACKS_PATH) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_PACKS_PATH)/$($(platform)_NUGET_SDK_NAME)) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_PACKS_PATH)/$($(platform)_NUGET_REF_NAME)) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(DOTNET_PACKS_PATH)/$($(rid)_NUGET_RUNTIME_NAME))) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_TEMPLATE_PACKS_PATH)/Microsoft.$(platform).Templates) \
	$(TMP_PKG_DIR) \
	$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_WINDOWS_SDK_NAME)/Sdk) \
	$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_WINDOWS_SDK_NAME)/targets) \
	$(foreach platform,$(DOTNET_PLATFORMS),Workloads/Microsoft.NET.Sdk.$(platform)) \

$(DIRECTORIES):
	$(Q) mkdir -p $@

CURRENT_HASH_LONG:=$(shell git log -1 --pretty=%H)


$(DOTNET_DESTDIR)/Microsoft.%: Microsoft.% | \
		$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/data $(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/Sdk $(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/targets $(DOTNET_DESTDIR)/Microsoft.$(platform).Templates) \
		$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_WINDOWS_SDK_NAME)/Sdk $(DOTNET_DESTDIR)/$($(platform)_NUGET_WINDOWS_SDK_NAME)/targets)
	$(Q) $(CP) $< $@

define CopyTargets
$$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets/%: targets/% | $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets
	$$(Q) $$(CP) $$< $$@

$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/%: Microsoft.$(1).Sdk/% | \
		$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/data \
		$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/Sdk \
		$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/targets
	$(Q) $(CP) $$< $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CopyTargets,$(platform))))

define CopyTargetsWindows
$(DOTNET_DESTDIR)/$($(1)_NUGET_WINDOWS_SDK_NAME)/%: Microsoft.$(1).Windows.Sdk/% | \
		$(DOTNET_DESTDIR)/$($(1)_NUGET_WINDOWS_SDK_NAME)/Sdk \
		$(DOTNET_DESTDIR)/$($(1)_NUGET_WINDOWS_SDK_NAME)/targets
	$(Q) $(CP) $$< $$@
endef
$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(eval $(call CopyTargetsWindows,$(platform))))

define VersionsTemplate
targets/Microsoft.$(1).Sdk.Versions.props: targets/Microsoft.Sdk.Versions.template.props Makefile $(TOP)/Make.config.inc $(TOP)/.git/HEAD $(TOP)/.git/index
	$$(Q_GEN) sed \
		-e "s/@PLATFORM@/$(1)/g" \
		-e "s/@NUGET_VERSION_NO_METADATA@/$$($(2)_NUGET_VERSION_NO_METADATA)/g" \
		-e "s/@NUGET_VERSION_FULL@/$$($(2)_NUGET_VERSION_FULL)/g" \
		-e "s/@DEFAULT_TARGET_PLATFORM_VERSION@/$$(DEFAULT_TARGET_PLATFORM_VERSION_$(2))/g" \
		-e "s/@CURRENT_BRANCH@/$$(CURRENT_BRANCH_SED_ESCAPED)/g" \
		-e "s/@CURRENT_HASH_LONG@/$$(CURRENT_HASH_LONG)/g" \
		-e 's*@VALID_RUNTIME_IDENTIFIERS@*$(foreach rid,$(3),\n\t\t<_XamarinValidRuntimeIdentifier Include="$(rid)" Platform="$(1)" />)*' \
		-e 's/@DOTNET_TFM@/$(DOTNET_TFM)/g' \
		-e 's/@RUNTIME_PACK_RUNTIME_IDENTIFIERS@/$(4)/g' \
		-e 's/@XCODE_IS_PREVIEW@/$(XCODE_IS_PREVIEW)/g' \
		-e 's/@NUGET_OS_VERSION@/$$($(2)_NUGET_OS_VERSION)/g' \
		$$< > $$@.tmp
	$$(Q) mv $$@.tmp $$@

Microsoft.$1.Sdk/targets/Microsoft.$1.Sdk.Versions.props: targets/Microsoft.$1.Sdk.Versions.props
	$$(Q) $$(CP) $$< $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call VersionsTemplate,$(platform),$(shell echo $(platform) | tr a-z A-Z),$(DOTNET_$(shell echo $(platform) | tr a-z A-Z)_RUNTIME_IDENTIFIERS),$(shell echo $(DOTNET_$(shell echo $(platform) | tr a-z A-Z)_RUNTIME_IDENTIFIERS) | tr ' ' ';'))))

version-props: $(foreach platform,$(DOTNET_PLATFORMS),targets/Microsoft.$(platform).Sdk.Versions.props)

define AutoImports
Microsoft.$(1).Sdk/Sdk/AutoImport.props: targets/AutoImport.template.props Makefile
	$(Q) rm -f $$@.tmp
	$$(Q_GEN) sed \
		-e "s/@PLATFORM@/$(1)/g" \
		$$< > $$@.tmp
	$(Q) mv $$@.tmp $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call AutoImports,$(platform))))

define DefaultItems
Microsoft.$1.Sdk/targets/Microsoft.$1.Sdk.DefaultItems.props: targets/Microsoft.Sdk.DefaultItems.template.props Makefile
	$$(Q_GEN) sed \
		-e "s/@PLATFORM@/$1/g" \
		-e "s/@TARGET_FRAMEWORK_VERSION@/$(subst net,,$(DOTNET_TFM))/g" \
		-e "s/@CURRENT_HASH_LONG@/$$(CURRENT_HASH_LONG)/g" \
		$$< > $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DefaultItems,$(platform))))

define ImplicitNamespaceImports
Microsoft.$1.Sdk/targets/Microsoft.$1.Sdk.ImplicitNamespaceImports.props: targets/Microsoft.Sdk.ImplicitNamespaceImports.template.props Makefile
	$$(Q_GEN) sed \
		-e "s/@PLATFORM@/$1/g" \
		-e "s/@TARGET_FRAMEWORK_VERSION@/$(subst net,,$(DOTNET_TFM))/g" \
		-e "s/@GLOBAL_USINGS@/$(2)/g" \
		$$< > $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call ImplicitNamespaceImports,$(platform),$(foreach using,$(DOTNET_$(platform)_GLOBAL_USINGS),\n\t\t<Using Include=\"$(using)\" Platform=\"$(platform)\" \/>))))

define SupportedTargetPlatforms
Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.SupportedTargetPlatforms.props: $(TOP)/Versions-ios.plist.in $(TOP)/Versions-mac.plist.in Makefile ./generate-target-platforms.csharp Makefile
	$(Q) rm -f $$@.tmp
	$(Q) ./generate-target-platforms.csharp $(1) "$(DOTNET_TFM)" "$$(SUPPORTED_API_VERSIONS_$(2))" $$@.tmp
	$(Q) mv $$@.tmp $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call SupportedTargetPlatforms,$(platform),$(shell echo $(platform) | tr a-z A-Z))))

define WorkloadTargets
Workloads/Microsoft.NET.Sdk.$(1)/WorkloadManifest.json: Makefile $(TOP)/Make.config.inc $(TOP)/.git/HEAD $(TOP)/.git/index Makefile generate-workloadmanifest-json.csharp | Workloads/Microsoft.NET.Sdk.$(1)
	$$(Q) rm -f $$@.tmp
	$$(Q_GEN) ./generate-workloadmanifest-json.csharp "$(1)" "$(3)" "$$(DOTNET_$(4)_RUNTIME_IDENTIFIERS)" "$$@.tmp" "$$(DOTNET_WINDOWS_PLATFORMS)" "$(DOTNET_TFM)_$$($(4)_NUGET_OS_VERSION)" "$$(SUPPORTED_API_VERSIONS_$(4))" $(TOP)/eng/Versions.props
	$$(Q) mv $$@.tmp $$@

Workloads/Microsoft.NET.Sdk.$(1)/WorkloadManifest.targets: Makefile $(TOP)/Make.config.inc $(TOP)/.git/HEAD $(TOP)/.git/index Makefile generate-workloadmanifest-targets.csharp | Workloads/Microsoft.NET.Sdk.$(1)
	$$(Q) rm -f $$@.tmp
	$$(Q_GEN) ./generate-workloadmanifest-targets.csharp "$(1)" "$$@.tmp" "$$(DOTNET_WINDOWS_PLATFORMS)" "$$(SUPPORTED_API_VERSIONS_$(4))" "$$(DEFAULT_TARGET_PLATFORM_VERSIONS_$(4))" "$$(DOTNET_TFM)-$($(4)_NUGET_OS_VERSION)"
	$$(Q) mv $$@.tmp $$@

Workloads/Microsoft.NET.Sdk.$(1)/LICENSE: $(TOP)/LICENSE | Workloads/Microsoft.NET.Sdk.$(1)
	$$(Q) $(CP) $$< $$@

WORKLOAD_TARGETS += Workloads/Microsoft.NET.Sdk.$(1)/WorkloadManifest.json
WORKLOAD_TARGETS += Workloads/Microsoft.NET.Sdk.$(1)/WorkloadManifest.targets
WORKLOAD_TARGETS += Workloads/Microsoft.NET.Sdk.$(1)/LICENSE

LOCAL_WORKLOAD_TARGETS += Workloads/Microsoft.NET.Sdk.$(1)/WorkloadManifest.json
LOCAL_WORKLOAD_TARGETS += Workloads/Microsoft.NET.Sdk.$(1)/WorkloadManifest.targets
LOCAL_WORKLOAD_TARGETS += Workloads/Microsoft.NET.Sdk.$(1)/LICENSE
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call WorkloadTargets,$(platform),$(shell echo $(platform) | tr A-Z a-z),$($(platform)_NUGET_VERSION_NO_METADATA),$(shell echo $(platform) | tr a-z A-Z),$(NET7_$(platform)_NUGET_VERSION_NO_METADATA))))

# Always use the commit distance as the third number in the VS component version, as it should always increase across all branches.
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(platform)_MSI_VERSION:=$($(shell echo $(platform) | tr '[:lower:]' '[:upper:]')_NUGET_OS_VERSION).$($(shell echo $(platform) | tr a-z A-Z)_NUGET_COMMIT_DISTANCE).0))

$(DOTNET_NUPKG_DIR)/vs-workload.props: Makefile generate-vs-workload.csharp
	$(Q) rm -f $@.tmp
	$(Q_GEN) ./generate-vs-workload.csharp \
		$(foreach platform,$(DOTNET_PLATFORMS),--platform $(platform) $($(platform)_MSI_VERSION)) \
		$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),--windows-platform $(platform)) \
		$(foreach platform,$(DOTNET_PLATFORMS),--shorten $($(shell echo $(platform) | tr '[:lower:]' '[:upper:]')_NUGET_VERSION_NO_METADATA)=$($(platform)_MSI_VERSION)) \
		--shorten Microsoft.MacCatalyst.Runtime.maccatalyst=Microsoft.MacCatalyst.Runtime \
		--shorten Microsoft.NET.Sdk.MacCatalyst.Manifest=Microsoft.MacCatalyst.Manifest \
		--shorten Microsoft.tvOS.Runtime.tvossimulator=Microsoft.tvOS.Runtime \
		--tfm $(DOTNET_TFM) \
		--output $@.tmp
	$(Q) mv $@.tmp $@

TEMPLATED_FILES = \
	$(foreach platform,$(DOTNET_PLATFORMS),Microsoft.$(platform).Sdk/targets/Microsoft.$(platform).Sdk.Versions.props) \
	$(foreach platform,$(DOTNET_PLATFORMS),Microsoft.$(platform).Sdk/targets/Microsoft.$(platform).Sdk.SupportedTargetPlatforms.props) \

ifdef NupkgWindowsDefinition
nupkgs/$($(1)_NUGET_WINDOWS_SDK_NAME).%.nupkg: CURRENT_VERSION_NO_METADATA=$($(2)_WINDOWS_NUGET_VERSION_NO_METADATA)
nupkgs/$($(1)_NUGET_WINDOWS_SDK_NAME).%.nupkg: CURRENT_VERSION_FULL=$($(2)_WINDOWS_NUGET_VERSION_FULL)
endif
$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(eval $(call NupkgWindowsDefinition,$(platform),$(shell echo $(platform) | tr '[:lower:]' '[:upper:]'))))

define NupkgDefinition
nupkgs/$($(1)_NUGET).%.nupkg: CURRENT_VERSION_NO_METADATA=$($(1)_NUGET_VERSION_NO_METADATA)
nupkgs/$($(1)_NUGET).%.nupkg: CURRENT_VERSION_FULL=$($(1)_NUGET_VERSION_FULL)
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call NupkgDefinition,$(platform))))

# Create the nuget in a temporary directory (nupkgs/)
define CreateNuGetTemplate
nupkgs/$(1)$(4).$(DOTNET_TFM)_$($(6)_NUGET_OS_VERSION).$(2).nupkg: $(TEMPLATED_FILES) $(WORKLOAD_TARGETS) $(3) package/$(1)/package.csproj $(wildcard package/*.csproj) $(wildcard $(DOTNET_DESTDIR)/$(1)/* $(DOTNET_DESTDIR)/$(1)/*/* $(DOTNET_DESTDIR)/$(1)/*/*/* $(DOTNET_DESTDIR)/$(1)/*/*/*/*) .stamp-workaround-for-maccore-issue-2427
	@# Delete any versions of the nuget we're building
	$$(Q) rm -f nupkgs/$(1).*.nupkg
	$$(Q_PACK) $(DOTNET) pack package/$(1)/package.csproj -p:VersionBand=$(5) --output "$$(dir $$@)" $(DOTNET_PACK_VERBOSITY) "/bl:$$@.binlog" /p:PackageOSTargetVersion=$(DOTNET_TFM)_$($(6)_NUGET_OS_VERSION)
endef

define CreateNuGetTemplateNoTargetFramework
nupkgs/$(1)$(4).$(2).nupkg: $(TEMPLATED_FILES) $(WORKLOAD_TARGETS) $(3) package/$(1)/package.csproj $(wildcard package/*.csproj) $(wildcard $(DOTNET_DESTDIR)/$(1)/* $(DOTNET_DESTDIR)/$(1)/*/* $(DOTNET_DESTDIR)/$(1)/*/*/* $(DOTNET_DESTDIR)/$(1)/*/*/*/*) .stamp-workaround-for-maccore-issue-2427
	@# Delete any versions of the nuget we're building
	$$(Q) rm -f nupkgs/$(1).*.nupkg
	$$(Q_PACK) $(DOTNET) pack package/$(1)/package.csproj -p:VersionBand=$(5) --output "$$(dir $$@)" $(DOTNET_PACK_VERBOSITY) "/bl:$$@.binlog"
endef

define CreateWindowsNuGetTemplate
nupkgs/$$($(6)_NUGET_WINDOWS_SDK_NAME).$(2).nupkg: $(3) $(WORKLOAD_TARGETS) package/$(1)/package.csproj $(wildcard package/*.csproj) $(wildcard $(DOTNET_DESTDIR)/$(1)/* $(DOTNET_DESTDIR)/$(1)/*/* $(DOTNET_DESTDIR)/$(1)/*/*/* $(DOTNET_DESTDIR)/$(1)/*/*/*/*) .stamp-workaround-for-maccore-issue-2427
	@# Delete any versions of the nuget we're building
	$$(Q) rm -f nupkgs/$(1).*.nupkg
	$$(Q_PACK) $(DOTNET) pack package/$(1)/package.csproj --output "$$(dir $$@)" $(DOTNET_PACK_VERBOSITY) "/bl:$$@.binlog" /p:PackageOSTargetVersion=$(DOTNET_TFM)_$($(6)_NUGET_OS_VERSION)
endef

# Create the NuGet packaging targets. It's amazing what make allows you to do...
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreateNuGetTemplate,Microsoft.$(platform).Sdk,$($(platform)_NUGET_VERSION_NO_METADATA),$($(platform)_NUGET_TARGETS),,$(DOTNET_VERSION_BAND),$(platform))))
$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(eval $(call CreateWindowsNuGetTemplate,Microsoft.$(platform).Windows.Sdk,$($(shell echo $(platform) | tr '[:lower:]' '[:upper:]')_WINDOWS_NUGET_VERSION_NO_METADATA),$($(platform)_WINDOWS_NUGET_TARGETS),,$(DOTNET_VERSION_BAND),$(platform))))
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreateNuGetTemplate,Microsoft.$(platform).Ref,$($(platform)_NUGET_VERSION_NO_METADATA),,,$(DOTNET_VERSION_BAND),$(platform))))
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreateNuGetTemplateNoTargetFramework,Microsoft.$(platform).Templates,$($(platform)_NUGET_VERSION_NO_METADATA),,,$(DOTNET_VERSION_BAND),$(platform))))
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreateNuGetTemplateNoTargetFramework,Microsoft.NET.Sdk.$(platform),$($(platform)_NUGET_VERSION_NO_METADATA),,.Manifest-$(MACIOS_MANIFEST_VERSION_BAND),$(MACIOS_MANIFEST_VERSION_BAND),$(platform))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call CreateNuGetTemplate,Microsoft.$(platform).Runtime.$(rid),$($(platform)_NUGET_VERSION_NO_METADATA),,,$(DOTNET_VERSION_BAND),$(platform)))))

# Copy the nuget from the temporary directory into the final directory
$(DOTNET_NUPKG_DIR)/%.nupkg: nupkgs/%.nupkg | $(DOTNET_NUPKG_DIR)
	$(Q) $(CP) $< $@

ifdef INCLUDE_IOS
SDK_PACKS_IOS_WINDOWS = $(DOTNET_NUPKG_DIR)/$(IOS_NUGET_WINDOWS_SDK_NAME).$(IOS_WINDOWS_NUGET_VERSION_NO_METADATA).nupkg
SDK_PACKS_WINDOWS += $(SDK_PACKS_IOS_WINDOWS)
endif

pack-ios-windows: $(SDK_PACKS_IOS_WINDOWS)

define PacksDefinitions
RUNTIME_PACKS_$(1) = $$(foreach rid,$$(DOTNET_$(1)_RUNTIME_IDENTIFIERS),$(DOTNET_NUPKG_DIR)/$$($$(rid)_NUGET_RUNTIME_NAME).$($(1)_NUGET_VERSION_NO_METADATA).nupkg)
RUNTIME_PACKS += $$(RUNTIME_PACKS_$(1))
REF_PACKS_$(1) = $(DOTNET_NUPKG_DIR)/$($(1)_NUGET_REF_NAME).$($(1)_NUGET_VERSION_NO_METADATA).nupkg
REF_PACKS += $$(REF_PACKS_$(1))
SDK_PACKS_$(1) = $(DOTNET_NUPKG_DIR)/$($(1)_NUGET_SDK_NAME).$($(1)_NUGET_VERSION_NO_METADATA).nupkg
SDK_PACKS += $$(SDK_PACKS_$(1))
TEMPLATE_PACKS_$(1) = $(DOTNET_NUPKG_DIR)/$($(1)_NUGET).Templates.$($(1)_NUGET_VERSION_NO_METADATA).nupkg
TEMPLATE_PACKS += $$(TEMPLATE_PACKS_$(1))
WORKLOAD_PACKS_$(1) = $(DOTNET_NUPKG_DIR)/$(subst Microsoft.,Microsoft.NET.Sdk.,$($(1)_NUGET)).Manifest-$(MACIOS_MANIFEST_VERSION_BAND).$($(1)_NUGET_VERSION_NO_METADATA).nupkg
WORKLOAD_PACKS += $$(WORKLOAD_PACKS_$(1))
pack-$(shell echo $(1) | tr A-Z a-z): $$(RUNTIME_PACKS_$(1)) $$(REF_PACKS_$(1)) $$(SDK_PACKS_$(1)) $$(TEMPLATE_PACKS_$(1)) $$(WORKLOAD_PACKS_$(1))
endef
$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(eval $(call PacksDefinitions,$(platform))))

TARGETS += $(RUNTIME_PACKS) $(REF_PACKS) $(SDK_PACKS) $(TEMPLATE_PACKS) $(WORKLOAD_PACKS) $(DOTNET_NUPKG_DIR)/vs-workload.props $(SDK_PACKS_WINDOWS)

define InstallWorkload
# .NET comes with a workload for us, but we don't want that, we want our own. So delete the workload that comes with .NET.
.stamp-workload-replace-$1-$(DOTNET_VERSION):
	$(Q) echo "Removing existing workload shipped with .NET $(DOTNET_VERSION): $(shell echo $(DOTNET_SDK_MANIFESTS_PATH)/*/microsoft.net.sdk.$3)"
	$(Q) rm -Rf $(DOTNET_SDK_MANIFESTS_PATH)/*/microsoft.net.sdk.$3
	$(Q) touch $$@

$(DOTNET_SDK_MANIFESTS_PATH)/$(MACIOS_MANIFEST_VERSION_BAND)/microsoft.net.sdk.$3: .stamp-workload-replace-$1-$(DOTNET_VERSION) | $(DOTNET_SDK_MANIFESTS_PATH)/$(MACIOS_MANIFEST_VERSION_BAND)
	$$(Q_LN) ln -Fhs $$(abspath Workloads/Microsoft.NET.Sdk.$1) $$(abspath $$@)

$(DOTNET_PACKS_PATH)/$($(1)_NUGET_SDK_NAME)/$2: | $(DOTNET_PACKS_PATH)/$($(1)_NUGET_SDK_NAME)
	$$(Q_LN) ln -Fhs $$(abspath $(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)) $$(abspath $$@)

$(DOTNET_PACKS_PATH)/$($(1)_NUGET_REF_NAME)/$2: | $(DOTNET_PACKS_PATH)/$($(1)_NUGET_REF_NAME)
	$$(Q_LN) ln -Fhs $$(abspath $(DOTNET_DESTDIR)/$($(1)_NUGET_REF_NAME)) $$(abspath $$@)

$(DOTNET_TEMPLATE_PACKS_PATH)/Microsoft.$1.Templates.$(2).nupkg: $(TEMPLATE_PACKS_$(shell echo $(1) | tr a-z A-Z)) | $(DOTNET_TEMPLATE_PACKS_PATH)
	$$(Q) $$(CP) $$< $$@

WORKLOAD_TARGETS += \
	$(DOTNET_TEMPLATE_PACKS_PATH)/Microsoft.$1.Templates.$(2).nupkg \
	$(DOTNET_SDK_MANIFESTS_PATH)/$(MACIOS_MANIFEST_VERSION_BAND)/microsoft.net.sdk.$3 \
	$(DOTNET_PACKS_PATH)/$($(1)_NUGET_SDK_NAME)/$2 \
	$(DOTNET_PACKS_PATH)/$($(1)_NUGET_REF_NAME)/$2
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call InstallWorkload,$(platform),$($(platform)_NUGET_VERSION_NO_METADATA),$(shell echo $(platform) | tr A-Z a-z))))

define InstallRuntimeWorkload
$(DOTNET_PACKS_PATH)/$$($(2)_NUGET_RUNTIME_NAME)/$3: | $(DOTNET_PACKS_PATH)/$$($(2)_NUGET_RUNTIME_NAME)
	$$(Q_LN) ln -Fhs $$(abspath $(DOTNET_DESTDIR)/$$($(2)_NUGET_RUNTIME_NAME)) $$(abspath $$@)

WORKLOAD_TARGETS += \
	$(DOTNET_PACKS_PATH)/$$($(2)_NUGET_RUNTIME_NAME)/$3
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call InstallRuntimeWorkload,$(platform),$(rid),$($(platform)_NUGET_VERSION_NO_METADATA)))))

# We create four packages: one for the workload, one for the sdk package, one for the ref package, and one for templates
# and then bundle them into a single pkg for distribution (named Microsoft.<platform>.Bundle.<version>.pkg).
# This way we can create and publish separate updates for each workload/sdk/ref package later if we want to.
define CreatePackage
# The workload package
$(TMP_PKG_DIR)/Microsoft.$1.Workload.$2.pkg: $($(1)_NUGET_TARGETS) $(WORKLOAD_TARGETS) Makefile | $(TMP_PKG_DIR)
	$$(Q) rm -f $$@
	$$(Q) rm -rf tmpdir/Microsoft.NET.Sdk.$1.$2/
	$$(Q) mkdir -p tmpdir/Microsoft.NET.Sdk.$1.$2/usr/local/share/dotnet/sdk-manifests/$(MACIOS_MANIFEST_VERSION_BAND)/
	$$(Q) $$(CP) -r Workloads/Microsoft.NET.Sdk.$1 tmpdir/Microsoft.NET.Sdk.$1.$2/usr/local/share/dotnet/sdk-manifests/$(MACIOS_MANIFEST_VERSION_BAND)/microsoft.net.sdk.$3
	$$(Q_GEN) pkgbuild --quiet --version '$2' --root tmpdir/Microsoft.NET.Sdk.$1.$2 --component-plist PackageInfo.plist  --install-location / --identifier com.microsoft.net.$3.workload.pkg $$@.tmp
	$$(Q) mv $$@.tmp $$@

# The sdk package
$(TMP_PKG_DIR)/$($(1)_NUGET_SDK_NAME).$2.pkg: $(SDK_PACKS_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -f $$@
	$$(Q) rm -rf tmpdir/Microsoft.$1.Sdk.$2/
	$$(Q) mkdir -p tmpdir/Microsoft.$1.Sdk.$2/usr/local/share/dotnet/packs/Microsoft.$1.Sdk/$2/
	$$(Q) $$(CP) -r $(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/ tmpdir/Microsoft.$1.Sdk.$2/usr/local/share/dotnet/packs/Microsoft.$1.Sdk/$2/
	$$(Q_GEN) pkgbuild --quiet --version '$2' --root tmpdir/Microsoft.$1.Sdk.$2 --component-plist PackageInfo.plist --install-location / --identifier com.microsoft.net.$3.sdk.$(DOTNET_TFM)_$$($(1)_NUGET_OS_VERSION).pkg $$@.tmp
	$$(Q) mv $$@.tmp $$@

# The ref package
$(TMP_PKG_DIR)/$($(1)_NUGET_REF_NAME).$2.pkg: $(REF_PACKS_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -f $$@
	$$(Q) rm -rf tmpdir/Microsoft.$1.Ref.$2/
	$$(Q) mkdir -p tmpdir/Microsoft.$1.Ref.$2/usr/local/share/dotnet/packs/Microsoft.$1.Ref/$2/
	$$(Q) $$(CP) -r $(DOTNET_DESTDIR)/$($(1)_NUGET_REF_NAME)/ tmpdir/Microsoft.$1.Ref.$2/usr/local/share/dotnet/packs/Microsoft.$1.Ref/$2/
	$$(Q_GEN) pkgbuild --quiet --version '$2' --root tmpdir/Microsoft.$1.Ref.$2 --component-plist PackageInfo.plist --install-location / --identifier com.microsoft.net.$3.ref.$(DOTNET_TFM)_$$($(1)_NUGET_OS_VERSION).pkg $$@.tmp
	$$(Q) mv $$@.tmp $$@

# The templates package
$(TMP_PKG_DIR)/Microsoft.$1.Templates.$2.pkg: $(TEMPLATE_PACKS_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -f $$@
	$$(Q) rm -rf tmpdir/Microsoft.$1.Templates.$2/
	$$(Q) mkdir -p tmpdir/Microsoft.$1.Templates.$2/usr/local/share/dotnet/template-packs/
	$$(Q) $$(CP) $(TEMPLATE_PACKS_$(4)) tmpdir/Microsoft.$1.Templates.$2/usr/local/share/dotnet/template-packs/$(subst +$(NUGET_BUILD_METADATA),,$(notdir $(TEMPLATE_PACKS_$(4))))
	$$(Q_GEN) pkgbuild --quiet --version '$2' --root tmpdir/Microsoft.$1.Templates.$2 --component-plist PackageInfo.plist --install-location / --identifier com.microsoft.net.$3.template.pkg $$@.tmp
	$$(Q) mv $$@.tmp $$@

# The final bundle package for distribution
$(TMP_PKG_DIR)/Microsoft.$1.Bundle.$2.pkg: $(TMP_PKG_DIR)/Microsoft.$1.Workload.$2.pkg $(TMP_PKG_DIR)/$($(1)_NUGET_SDK_NAME).$2.pkg $(TMP_PKG_DIR)/$($(1)_NUGET_REF_NAME).$2.pkg $(TMP_PKG_DIR)/Microsoft.$1.Templates.$2.pkg
	$$(Q) rm -f $$@
	$$(Q_GEN) productbuild \
		--quiet \
		--identifier com.microsoft.net.$3.pkg \
		--version '$2' \
		--package $(TMP_PKG_DIR)/Microsoft.$1.Workload.$2.pkg \
		--package $(TMP_PKG_DIR)/$($(1)_NUGET_SDK_NAME).$2.pkg \
		--package $(TMP_PKG_DIR)/$($(1)_NUGET_REF_NAME).$2.pkg \
		--package $(TMP_PKG_DIR)/Microsoft.$1.Templates.$2.pkg \
		$$@.tmp
	$$(Q) mv $$@.tmp $$@

# Copy the bundle package from our temporary directory to the target directory
$(DOTNET_PKG_DIR)/%: $(TMP_PKG_DIR)/% | $(DOTNET_PKG_DIR)
	$$(Q) $(CP) $$< $$@

PACKAGE_TARGETS += $(DOTNET_PKG_DIR)/Microsoft.$1.Bundle.$2.pkg

$(TMP_PKG_DIR)/Microsoft.$1.Bundle.$2.zip: $($(1)_NUGET_TARGETS) $(WORKLOAD_TARGETS) Makefile $(REF_PACKS_$(4)) $(SDK_PACKs_$(4)) $(TEMPLATE_PACKS_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -rf $$@ $$@.tmpdir $$@.tmp
	$$(Q) mkdir -p $$@.tmpdir/dotnet/sdk-manifests/$(MACIOS_MANIFEST_VERSION_BAND)/
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/$($(1)_NUGET_SDK_NAME)
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/$($(1)_NUGET_REF_NAME)
	$$(Q) mkdir -p $$@.tmpdir/dotnet/template-packs
	$$(Q) $(CP) -r Workloads/Microsoft.NET.Sdk.$1 $$@.tmpdir/dotnet/sdk-manifests/$(MACIOS_MANIFEST_VERSION_BAND)/microsoft.net.sdk.$3
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME) $$@.tmpdir/dotnet/packs/$($(1)_NUGET_SDK_NAME)/$2
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/$($(1)_NUGET_REF_NAME) $$@.tmpdir/dotnet/packs/$($(1)_NUGET_REF_NAME)/$2
	$$(Q) $(CP) $(TEMPLATE_PACKS_$(4)) $$@.tmpdir/dotnet/template-packs/$(subst +$(NUGET_BUILD_METADATA),,$(notdir $(TEMPLATE_PACKS_$(4))))
	$$(Q_GEN) cd $$@.tmpdir && zip -9rq $$(abspath $$@.tmp) .
	$$(Q) mv $$@.tmp $$@
	$$(Q) echo Created $$@

PACKAGE_TARGETS += $(DOTNET_PKG_DIR)/Microsoft.$1.Bundle.$2.zip

install-system-$(3): $(DOTNET_PKG_DIR)/Microsoft.$1.Bundle.$2.pkg
	$(Q) echo "Installing $$<..."
	$(Q) sudo installer -pkg "$$<" -target / $$(INSTALLER_VERBOSITY)
	$(Q) echo "Installed $$<"
	$(Q) echo "Execute this in the project directory to be built for NuGet to find the runtime packs:"
	$(Q) echo "    nuget sources add -Name local-macios-packages -ConfigFile NuGet.config -Source file://$(abspath $(DOTNET_NUPKG_DIR))"

INSTALL_SYSTEM_PACKAGE_TARGETS += install-system-$(3)
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreatePackage,$(platform),$($(platform)_NUGET_VERSION_NO_METADATA),$(shell echo $(platform) | tr A-Z a-z),$(shell echo $(platform) | tr a-z A-Z))))

define CreateWindowsBundle
$(TMP_PKG_DIR)/Microsoft.$1.Windows.Bundle.$2.zip: $($(1)_NUGET_TARGETS) $($(1)_WINDOWS_NUGET_TARGETS) $(WORKLOAD_TARGETS) Makefile $(REF_PACKS_$(4)) $(SDK_PACKS_$(4)) $(SDK_PACKS_$(4)_WINDOWS) $(TEMPLATE_PACKS_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -rf $$@ $$@.tmpdir $$@.tmp
	$$(Q) mkdir -p $$@.tmpdir/dotnet/sdk-manifests/$(MACIOS_MANIFEST_VERSION_BAND)/
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/$($(1)_NUGET_SDK_NAME)
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/$($(1)_NUGET_WINDOWS_SDK_NAME)
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/$($(1)_NUGET_REF_NAME)
	$$(Q) mkdir -p $$@.tmpdir/dotnet/template-packs
	$$(Q) $(CP) -r Workloads/Microsoft.NET.Sdk.$1 $$@.tmpdir/dotnet/sdk-manifests/$(MACIOS_MANIFEST_VERSION_BAND)/microsoft.net.sdk.$3
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME) $$@.tmpdir/dotnet/packs/$($(1)_NUGET_SDK_NAME)/$2
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/$($(1)_NUGET_WINDOWS_SDK_NAME) $$@.tmpdir/dotnet/packs/$($(1)_NUGET_WINDOWS_SDK_NAME)/$2
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/$($(1)_NUGET_REF_NAME) $$@.tmpdir/dotnet/packs/$($(1)_NUGET_REF_NAME)/$2
	$$(Q) $(CP) $(TEMPLATE_PACKS_$(4)) $$@.tmpdir/dotnet/template-packs/$(subst +$(NUGET_BUILD_METADATA),,$(notdir $(TEMPLATE_PACKS_$(4))))
	$$(Q_GEN) cd $$@.tmpdir && zip -9rq $$(abspath $$@.tmp) .
	$$(Q) mv $$@.tmp $$@
	$$(Q) echo Created $$@

WINDOWS_PACKAGE_TARGETS += $(DOTNET_PKG_DIR)/Microsoft.$1.Windows.Bundle.$2.zip
endef
$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(eval $(call CreateWindowsBundle,$(platform),$($(platform)_NUGET_VERSION_NO_METADATA),$(shell echo $(platform) | tr A-Z a-z),$(shell echo $(platform) | tr a-z A-Z))))

define CreateMsi
$(TMP_PKG_DIR)/Microsoft.NET.Workload.$1.$2.wsx: ./generate-wix.csharp Makefile $(TMP_PKG_DIR)/Microsoft.$1.Windows.Bundle.$2.zip
	$$(Q_GEN) ./generate-wix.csharp "$1" "$$@" "$(TMP_PKG_DIR)/Microsoft.$1.Windows.Bundle.$2.zip.tmpdir/dotnet" "$2"

$(TMP_PKG_DIR)/Microsoft.NET.Workload.$1.$2.msi: $(TMP_PKG_DIR)/Microsoft.NET.Workload.$1.$2.wsx .stamp-check-wixl
	$$(Q_GEN) wixl -o "$$@" "$$<" -a x64

MSI_TARGETS += $(DOTNET_PKG_DIR)/Microsoft.NET.Workload.$1.$2.msi
endef
$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(eval $(call CreateMsi,$(platform),$($(platform)_NUGET_VERSION_NO_METADATA))))

NUGET_SOURCES:=$(shell grep https://pkgs.dev.azure.com $(TOP)/NuGet.config | sed -e 's/.*value="//'  -e 's/".*//')
.stamp-install-workloads: Makefile $(WORKLOAD_TARGETS) $(RUNTIME_PACKS) $(REF_PACKS) $(SDK_PACKS) $(TEMPLATE_PACKS) $(WORKLOAD_PACKS)
	$(Q) $(DOTNET) workload install --skip-manifest-update \
		$(foreach source,$(NUGET_SOURCES),--source $(source)) \
		--source $(DOTNET_NUPKG_DIR) $(DOTNET_WORKLOAD_VERBOSITY) \
		$(foreach platform,$(DOTNET_PLATFORMS),$(shell echo $(platform) | tr A-Z a-z))
	$(Q) touch $@

TARGETS += .stamp-install-workloads

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

TARGETS += $(WORKLOAD_TARGETS) $(WINDOWS_PACKAGE_TARGETS)

msi: $(MSI_TARGETS)
package: $(PACKAGE_TARGETS) $(MSI_TARGETS) $(WINDOWS_PACKAGE_TARGETS)

# Helper targets for templates
#
# "make templates": only build the template nupkgs
# "make install-templates": builds the template nupkgs, and install them into the current working copy's local .NET. This is typically needed to test templates in our own tests, or when using the templates from the command line with the local .NET.
# "make install-templates-system": builds the template nupkgs, and installs them into the system's .NET. This is typically needed to test templates in VSMac.
# On Windows: copy $(TOP)/_build/nupkg/*Template* to your windows machine, and "dotnet new --install template.nupkg" for each nupkg, then they'll show up in VS.
#
templates: $(TEMPLATE_PACKS)

# We manually clean first, because the makefile dependencies aren't configured correctly for the templates (the templates contain file names with spaces, which is quite hard to get right in make without making the code unbeliavably complicated)
install-templates:
	$(Q) $(MAKE) clean-templates
	$(Q) $(MAKE) $(TEMPLATE_PACKS) -j4
	$(Q) for platform in $(DOTNET_PLATFORMS); do \
		echo "Uninstalling: $$platform"; \
		$(DOTNET) new --uninstall Microsoft.$$platform.Templates; \
	done
	$(Q) for nupkg in $(TEMPLATE_PACKS); do \
		echo "Installing $$nupkg"; \
		$(DOTNET) new --install $$nupkg; \
	done

# We need to run the system dotnet, and we can't run it from inside this working copy, because we have a top-level
# NuGet.config / global.json which will want to use the local dotnet. So run dotnet from the HOME directory instead.
# We manually clean first, because the makefile dependencies aren't configured correctly for the templates (the templates contain file names with spaces, which is quite hard to get right in make without making the code unbeliavably complicated)
install-templates-system:
	$(Q) $(MAKE) clean-templates
	$(Q) $(MAKE) $(TEMPLATE_PACKS) -j4
	$(Q) for platform in $(DOTNET_PLATFORMS); do \
		echo "Uninstalling: $$platform"; \
		cd $(HOME) && dotnet new --uninstall Microsoft.$$platform.Templates || true; \
	done
	$(Q) for nupkg in $(TEMPLATE_PACKS); do \
		echo "Installing $$nupkg"; \
		cd $(HOME) && dotnet new --install $(CURDIR)/$$nupkg; \
	done

clean-templates:
	$(Q) rm -f -- $(TEMPLATE_PACKS)

install-system:
	@# We don't want to execute these in parallel
	@for target in $(INSTALL_SYSTEM_PACKAGE_TARGETS); do \
		$(MAKE) $$target; \
	done

ifdef ENABLE_DOTNET
all-local:: $(TARGETS)
endif

clean-local::
	$(Q) rm -Rf $(DOTNET_NUPKG_DIR)
	$(Q) git clean -xfdq

.stamp-workaround-for-maccore-issue-2427: $(TOP)/eng/Versions.props $(LOCAL_WORKLOAD_TARGETS)
	$(Q) $(DOTNET) restore package/workaround-for-maccore-issue-2427/restore.csproj /bl:package/workaround-for-maccore-issue-2427/restore.binlog $(MSBUILD_VERBOSITY)
	$(Q) touch $@

$(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip: .stamp-install-workloads
	$(Q) $(MAKE) -C $(TOP)/msbuild/Xamarin.HotRestart.PreBuilt all
	$(Q) touch $@

# We need to shut down the builder server, because:
#   We're using parallel make, and parallel make will start a jobserver, managed by file descriptors, where these file descriptors must be closed in all subprocesses for make to realize it's done.
#   'dotnet pack' might have started a build server
#   The build server does not close any file descriptors it may have inherited when daemonizing itself.
#   Thus the build server (which will still be alive after we're done building here) might have a file descriptor open which make is waiting for.
#   The proper fix is to fix the build server to close its file descriptors.
#   The intermediate working is to shut down the build server instead. An alternative solution would be to pass /p:UseSharedCompilation=false to 'dotnet pack' to disable the usage of the build server.
#
# The 'shutdown-build-server' is executed in a sub-make (and not as a dependency to the all-hook target),
# to make sure it's executed after everything else is built in this file.
all-hook::
	$(Q) $(MAKE) shutdown-build-server

shutdown-build-server:
	$(Q) $(DOTNET) build-server shutdown
