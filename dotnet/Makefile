TOP = ..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

DOTNET_MANIFESTS_PATH=$(DOTNET_DIR)/sdk-manifests/$(DOTNET_MANIFEST_VERSION_WITH_PRERELEASE_COMPONENT)
DOTNET_PACKS_PATH=$(DOTNET_DIR)/packs
DOTNET_TEMPLATE_PACKS_PATH=$(DOTNET_DIR)/template-packs
TMP_PKG_DIR=_pkg

DOTNET_PLATFORMS_UPPERCASE:=$(shell echo $(DOTNET_PLATFORMS) | tr a-z A-Z)

# Create variables prefixed with the correctly cased platform name from the upper-cased platform name. This makes some of the next sections somewhat simpler.
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(platform)_NUGET_VERSION_NO_METADATA:=$($(shell echo $(platform) | tr a-z A-Z)_NUGET_VERSION_NO_METADATA)))

DOTNET_iOS_GLOBAL_USINGS=CoreGraphics Foundation UIKit
DOTNET_tvOS_GLOBAL_USINGS=CoreGraphics Foundation UIKit
DOTNET_MacCatalyst_GLOBAL_USINGS=CoreGraphics Foundation UIKit
DOTNET_macOS_GLOBAL_USINGS=AppKit CoreGraphics Foundation

define DefineTargets
$(1)_NUGET_TARGETS = \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/data/UnixFilePermissions.xml \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/Sdk/AutoImport.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/Sdk/Sdk.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.ImplicitNamespaceImports.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.SupportedTargetPlatforms.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.DefaultItems.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.Versions.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.targets \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Xamarin.Shared.Sdk.DefaultItems.targets \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Xamarin.Shared.Sdk.Publish.targets \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Xamarin.Shared.Sdk.TargetFrameworkInference.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Xamarin.Shared.Sdk.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/Xamarin.Shared.Sdk.targets \

endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DefineTargets,$(platform))))

define DefineWindowsTargets
$(1)_WINDOWS_NUGET_TARGETS = \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Windows.Sdk/Sdk/Sdk.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Windows.Sdk/targets/Microsoft.$(1).Windows.Sdk.props \
	$(DOTNET_DESTDIR)/Microsoft.$(1).Windows.Sdk/targets/Microsoft.$(1).Windows.Sdk.targets \

endef
$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(eval $(call DefineWindowsTargets,$(platform))))

ifdef INCLUDE_HOTRESTART

iOS_WINDOWS_NUGET_TARGETS += \
	$(DOTNET_DESTDIR)/Microsoft.iOS.Windows.Sdk/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip \

endif

DIRECTORIES += \
	$(DOTNET_NUPKG_DIR) \
	$(DOTNET_PKG_DIR) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/data) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/Sdk) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Templates) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.NET.Sdk.$(platform)) \
	$(DOTNET_MANIFESTS_PATH) \
	$(DOTNET_PACKS_PATH) \
	$(DOTNET_TEMPLATE_PACKS_PATH) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_PACKS_PATH)/Microsoft.$(platform).Sdk) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_PACKS_PATH)/Microsoft.$(platform).Ref) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(DOTNET_PACKS_PATH)/Microsoft.$(platform).Runtime.$(rid))) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_TEMPLATE_PACKS_PATH)/Microsoft.$(platform).Templates) \
	$(TMP_PKG_DIR) \
	$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Windows.Sdk/Sdk) \
	$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Windows.Sdk/targets) \

$(DIRECTORIES):
	$(Q) mkdir -p $@

.stamp-cp-mono-workload: $(TOP)/Make.config
	mkdir -p $(DOTNET_MANIFESTS_PATH)
	$(CP) -r $(DOTNET_MANIFESTS_PATH)/../$(DOTNET_MANIFEST_VERSION_BAND)/* $(DOTNET_MANIFESTS_PATH)/
	$(Q) touch $@
TARGETS += .stamp-cp-mono-workload

CURRENT_HASH_LONG:=$(shell git log -1 --pretty=%H)

$(DOTNET_DESTDIR)/Microsoft.%: Microsoft.% | $(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/data $(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/Sdk $(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/targets $(DOTNET_DESTDIR)/Microsoft.$(platform).Templates) \
		$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Windows.Sdk/Sdk $(DOTNET_DESTDIR)/Microsoft.$(platform).Windows.Sdk/targets)
	$(Q) $(CP) $< $@

define CopyTargets
$$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets/%: targets/% | $$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/targets
	$$(Q) $$(CP) $$< $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CopyTargets,$(platform))))

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
		$$< > $$@.tmp
	$$(Q) mv $$@.tmp $$@

Microsoft.$1.Sdk/targets/Microsoft.$1.Sdk.Versions.props: targets/Microsoft.$1.Sdk.Versions.props
	$$(Q) $$(CP) $$< $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call VersionsTemplate,$(platform),$(shell echo $(platform) | tr a-z A-Z),$(DOTNET_$(shell echo $(platform) | tr a-z A-Z)_RUNTIME_IDENTIFIERS))))

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
x:: Microsoft.$1.Sdk/targets/Microsoft.$1.Sdk.ImplicitNamespaceImports.props
	cat Microsoft.$1.Sdk/targets/Microsoft.$1.Sdk.ImplicitNamespaceImports.props
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call ImplicitNamespaceImports,$(platform),$(foreach using,$(DOTNET_$(platform)_GLOBAL_USINGS),\n\t\t<Using Include=\"$(using)\" Platform=\"$(platform)\" \/>))))

define SupportedTargetPlatforms
Microsoft.$(1).Sdk/targets/Microsoft.$(1).Sdk.SupportedTargetPlatforms.props: $(TOP)/Versions-ios.plist.in $(TOP)/Versions-mac.plist.in Makefile ./generate-target-platforms.csharp Makefile
	$(Q) rm -f $$@.tmp
	$(Q) ./generate-target-platforms.csharp $(1) $$@.tmp
	$(Q) mv $$@.tmp $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call SupportedTargetPlatforms,$(platform))))

define WorkloadTargets
Workloads/Microsoft.NET.Sdk.$(1)/WorkloadManifest.json: targets/WorkloadManifest.$(1).template.json Makefile $(TOP)/Make.config.inc $(TOP)/.git/HEAD $(TOP)/.git/index Makefile
	$$(Q) rm -f $$@.tmp
	$$(Q_GEN) sed \
		-e "s/@VERSION@/$3/g" \
		-e "s/@BUNDLED_NETCORE_PLATFORMS_PACKAGE_VERSION@/$(BUNDLED_NETCORE_PLATFORMS_PACKAGE_VERSION)/g" \
		-e "s/@PLATFORM_LOWERCASE@/$2/g" \
		-e "s/@PLATFORM@/$1/g" \
		$$< > $$@.tmp
	$$(Q) mv $$@.tmp $$@

Workloads/Microsoft.NET.Sdk.$(1)/LICENSE: $(TOP)/LICENSE
	$$(Q) $(CP) $$< $$@

WORKLOAD_TARGETS += Workloads/Microsoft.NET.Sdk.$(1)/WorkloadManifest.json
WORKLOAD_TARGETS += Workloads/Microsoft.NET.Sdk.$(1)/LICENSE

LOCAL_WORKLOAD_TARGETS += Workloads/Microsoft.NET.Sdk.$(1)/WorkloadManifest.json
LOCAL_WORKLOAD_TARGETS += Workloads/Microsoft.NET.Sdk.$(1)/LICENSE
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call WorkloadTargets,$(platform),$(shell echo $(platform) | tr A-Z a-z),$($(platform)_NUGET_VERSION_NO_METADATA))))

$(DOTNET_NUPKG_DIR)/vs-workload.props: Workloads/vs-workload.template.props
	$(Q) rm -f $@.tmp
	$(Q_GEN) sed \
		$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),-e "s/@$(platform)_WORKLOAD_VERSION@/$($(platform)_NUGET_VERSION).$($(platform)_NUGET_COMMIT_DISTANCE)/g") \
		-e "s/@PACK_VERSION_LONG@/$(IOS_NUGET_VERSION_NO_METADATA)/g" \
		-e "s/@PACK_VERSION_SHORT@/$(IOS_NUGET_VERSION).$(IOS_NUGET_COMMIT_DISTANCE)/g" \
		-e "s/@MACOS_PACK_VERSION_LONG@/$(MACOS_NUGET_VERSION_NO_METADATA)/g" \
		-e "s/@MACOS_PACK_VERSION_SHORT@/$(MACOS_NUGET_VERSION).$(MACOS_NUGET_COMMIT_DISTANCE)/g" \
		-e "s/@VSMAN_VERSION@/$(DOTNET_TFM)/g" \
		$< > $@.tmp
	$(Q) mv $@.tmp $@

$(DOTNET_NUPKG_DIR)/SignList.xml: Workloads/SignList.xml
	$(Q) $(CP) $< $@

$(DOTNET_NUPKG_DIR)/SignList.targets: Workloads/SignList.targets
	$(Q) $(CP) $< $@

TEMPLATED_FILES = \
	$(foreach platform,$(DOTNET_PLATFORMS),Microsoft.$(platform).Sdk/targets/Microsoft.$(platform).Sdk.Versions.props) \
	$(foreach platform,$(DOTNET_PLATFORMS),Microsoft.$(platform).Sdk/targets/Microsoft.$(platform).Sdk.SupportedTargetPlatforms.props) \

nupkgs/$(IOS_WINDOWS_NUGET).%.nupkg: CURRENT_VERSION_NO_METADATA=$(IOS_WINDOWS_NUGET_VERSION_NO_METADATA)
nupkgs/$(IOS_WINDOWS_NUGET).%.nupkg: CURRENT_VERSION_FULL=$(IOS_WINDOWS_NUGET_VERSION_FULL)

define NupkgDefinition
nupkgs/$($(1)_NUGET).%.nupkg: CURRENT_VERSION_NO_METADATA=$($(1)_NUGET_VERSION_NO_METADATA)
nupkgs/$($(1)_NUGET).%.nupkg: CURRENT_VERSION_FULL=$($(1)_NUGET_VERSION_FULL)
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call NupkgDefinition,$(platform))))

# Create the nuget in a temporary directory (nupkgs/)
define CreateNuGetTemplate
nupkgs/$(1)$(4).$(2).nupkg: $(TEMPLATED_FILES) $(WORKLOAD_TARGETS) $(3) package/$(1)/package.csproj $(wildcard package/*.csproj) $(wildcard $(DOTNET_DESTDIR)/$(1)/* $(DOTNET_DESTDIR)/$(1)/*/* $(DOTNET_DESTDIR)/$(1)/*/*/* $(DOTNET_DESTDIR)/$(1)/*/*/*/*) .stamp-workaround-for-maccore-issue-2427
	@# Delete any versions of the nuget we're building
	$$(Q) rm -f nupkgs/$(1).*.nupkg
	$$(Q_PACK) $(DOTNET) pack package/$(1)/package.csproj -p:VersionBand=$(5) --output "$$(dir $$@)" $(DOTNET_PACK_VERBOSITY) "/bl:$$@.binlog"
endef

define CreateWindowsNuGetTemplate
nupkgs/$(1).$(2).nupkg: $(3) $(WORKLOAD_TARGETS) package/$(1)/package.csproj $(wildcard package/*.csproj) $(wildcard $(DOTNET_DESTDIR)/$(1)/* $(DOTNET_DESTDIR)/$(1)/*/* $(DOTNET_DESTDIR)/$(1)/*/*/* $(DOTNET_DESTDIR)/$(1)/*/*/*/*) .stamp-workaround-for-maccore-issue-2427
	@# Delete any versions of the nuget we're building
	$$(Q) rm -f nupkgs/$(1).*.nupkg
	$$(Q_PACK) $(DOTNET) pack package/$(1)/package.csproj --output "$$(dir $$@)" $(DOTNET_PACK_VERBOSITY) "/bl:$$@.binlog"
endef

# Create the NuGet packaging targets. It's amazing what make allows you to do...
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreateNuGetTemplate,Microsoft.$(platform).Sdk,$($(platform)_NUGET_VERSION_NO_METADATA),$($(platform)_NUGET_TARGETS),,$(DOTNET_VERSION_BAND))))
$(foreach platform,$(DOTNET_WINDOWS_PLATFORMS),$(eval $(call CreateWindowsNuGetTemplate,Microsoft.$(platform).Windows.Sdk,$(IOS_WINDOWS_NUGET_VERSION_NO_METADATA),$($(platform)_WINDOWS_NUGET_TARGETS),,$(DOTNET_VERSION_BAND))))
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreateNuGetTemplate,Microsoft.$(platform).Ref,$($(platform)_NUGET_VERSION_NO_METADATA),,,$(DOTNET_VERSION_BAND))))
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreateNuGetTemplate,Microsoft.$(platform).Templates,$($(platform)_NUGET_VERSION_NO_METADATA),,,$(DOTNET_VERSION_BAND))))
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreateNuGetTemplate,Microsoft.NET.Sdk.$(platform),$($(platform)_NUGET_VERSION_NO_METADATA),,.Manifest-$(DOTNET_MANIFEST_VERSION_WITH_PRERELEASE_COMPONENT),$(DOTNET_MANIFEST_VERSION_WITH_PRERELEASE_COMPONENT))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call CreateNuGetTemplate,Microsoft.$(platform).Runtime.$(rid),$($(platform)_NUGET_VERSION_NO_METADATA),,,$(DOTNET_VERSION_BAND)))))

# Copy the nuget from the temporary directory into the final directory
$(DOTNET_NUPKG_DIR)/%.nupkg: nupkgs/%.nupkg | $(DOTNET_NUPKG_DIR)
	$(Q) $(CP) $< $@

ifdef INCLUDE_IOS
SDK_PACK_IOS_WINDOWS = $(DOTNET_NUPKG_DIR)/$(IOS_WINDOWS_NUGET).Sdk.$(IOS_WINDOWS_NUGET_VERSION_NO_METADATA).nupkg
SDK_PACKS_WINDOWS += $(SDK_PACK_IOS_WINDOWS)
endif

pack-ios-windows: $(SDK_PACK_IOS_WINDOWS)

define PacksDefinitions
RUNTIME_PACKS_$(1) = $$(foreach rid,$$(DOTNET_$(1)_RUNTIME_IDENTIFIERS),$(DOTNET_NUPKG_DIR)/$($(1)_NUGET).Runtime.$$(rid).$($(1)_NUGET_VERSION_NO_METADATA).nupkg)
RUNTIME_PACKS += $$(RUNTIME_PACKS_$(1))
REF_PACKS_$(1) = $(DOTNET_NUPKG_DIR)/$($(1)_NUGET).Ref.$($(1)_NUGET_VERSION_NO_METADATA).nupkg
REF_PACKS += $$(REF_PACKS_$(1))
SDK_PACKS_$(1) = $(DOTNET_NUPKG_DIR)/$($(1)_NUGET).Sdk.$($(1)_NUGET_VERSION_NO_METADATA).nupkg
SDK_PACKS += $$(SDK_PACKS_$(1))
TEMPLATE_PACKS_$(1) = $(DOTNET_NUPKG_DIR)/$($(1)_NUGET).Templates.$($(1)_NUGET_VERSION_NO_METADATA).nupkg
TEMPLATE_PACKS += $$(TEMPLATE_PACKS_$(1))
WORKLOAD_PACKS_$(1) = $(DOTNET_NUPKG_DIR)/$(subst Microsoft.,Microsoft.NET.Sdk.,$($(1)_NUGET)).Manifest-$(DOTNET_MANIFEST_VERSION_WITH_PRERELEASE_COMPONENT).$($(1)_NUGET_VERSION_NO_METADATA).nupkg
WORKLOAD_PACKS += $$(WORKLOAD_PACKS_$(1))
pack-$(shell echo $(1) | tr A-Z a-z): $$(RUNTIME_PACKS_$(1)) $$(REF_PACKS_$(1)) $$(SDK_PACKS_$(1)) $$(TEMPLATE_PACKS_$(1)) $$(WORKLOAD_PACKS_$(1))
endef
$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(eval $(call PacksDefinitions,$(platform))))

TARGETS += $(RUNTIME_PACKS) $(REF_PACKS) $(SDK_PACKS) $(TEMPLATE_PACKS) $(WORKLOAD_PACKS) $(DOTNET_NUPKG_DIR)/vs-workload.props $(DOTNET_NUPKG_DIR)/SignList.xml $(DOTNET_NUPKG_DIR)/SignList.targets $(SDK_PACKS_WINDOWS)

define InstallWorkload
# .NET comes with a workload for us, but we don't want that, we want our own. So delete the workload that comes with .NET.
.stamp-workload-replace-$1-$(DOTNET_VERSION):
	$(Q) echo "Removing existing workload shipped with .NET $(DOTNET_VERSION): $(DOTNET_MANIFESTS_PATH)/microsoft.net.sdk.$1"
	$(Q) rm -Rf $(DOTNET_MANIFESTS_PATH)/microsoft.net.sdk.$3
	$(Q) touch $$@

$(DOTNET_MANIFESTS_PATH)/microsoft.net.sdk.$3: .stamp-workload-replace-$1-$(DOTNET_VERSION) | $(DOTNET_MANIFESTS_PATH)
	$$(Q_LN) ln -Fhs $$(abspath Workloads/Microsoft.NET.Sdk.$1) $$(abspath $$@)

$(DOTNET_PACKS_PATH)/Microsoft.$1.Sdk/$2: | $(DOTNET_PACKS_PATH)/Microsoft.$1.Sdk
	$$(Q_LN) ln -Fhs $$(abspath $(DOTNET_DESTDIR)/Microsoft.$1.Sdk) $$(abspath $$@)

$(DOTNET_PACKS_PATH)/Microsoft.$1.Ref/$2: | $(DOTNET_PACKS_PATH)/Microsoft.$1.Ref
	$$(Q_LN) ln -Fhs $$(abspath $(DOTNET_DESTDIR)/Microsoft.$1.Ref) $$(abspath $$@)

$(DOTNET_TEMPLATE_PACKS_PATH)/Microsoft.$1.Templates.$(2).nupkg: $(TEMPLATE_PACKS_$(shell echo $(1) | tr a-z A-Z)) | $(DOTNET_TEMPLATE_PACKS_PATH)
	$$(Q) $$(CP) $$< $$@

WORKLOAD_TARGETS += \
	$(DOTNET_TEMPLATE_PACKS_PATH)/Microsoft.$1.Templates.$(2).nupkg \
	$(DOTNET_MANIFESTS_PATH)/microsoft.net.sdk.$3 \
	$(DOTNET_PACKS_PATH)/Microsoft.$1.Sdk/$2 \
	$(DOTNET_PACKS_PATH)/Microsoft.$1.Ref/$2
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call InstallWorkload,$(platform),$($(platform)_NUGET_VERSION_NO_METADATA),$(shell echo $(platform) | tr A-Z a-z))))

define InstallRuntimeWorkload
$(DOTNET_PACKS_PATH)/Microsoft.$1.Runtime.$2/$3: | $(DOTNET_PACKS_PATH)/Microsoft.$1.Runtime.$2
	$$(Q_LN) ln -Fhs $$(abspath $(DOTNET_DESTDIR)/Microsoft.$1.Runtime.$2) $$(abspath $$@)

WORKLOAD_TARGETS += \
	$(DOTNET_PACKS_PATH)/Microsoft.$1.Runtime.$2/$3
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
	$$(Q) mkdir -p tmpdir/Microsoft.NET.Sdk.$1.$2/usr/local/share/dotnet/sdk-manifests/$(DOTNET_MANIFEST_VERSION_WITH_PRERELEASE_COMPONENT)/
	$$(Q) $$(CP) -r Workloads/Microsoft.NET.Sdk.$1 tmpdir/Microsoft.NET.Sdk.$1.$2/usr/local/share/dotnet/sdk-manifests/$(DOTNET_MANIFEST_VERSION_WITH_PRERELEASE_COMPONENT)/microsoft.net.sdk.$3
	$$(Q_GEN) pkgbuild --quiet --version '$2' --root tmpdir/Microsoft.NET.Sdk.$1.$2 --component-plist PackageInfo.plist  --install-location / --identifier com.microsoft.net.$3.workload.pkg $$@.tmp
	$$(Q) mv $$@.tmp $$@

# The sdk package
$(TMP_PKG_DIR)/Microsoft.$1.Sdk.$2.pkg: $(REF_PACK_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -f $$@
	$$(Q) rm -rf tmpdir/Microsoft.$1.Sdk.$2/
	$$(Q) mkdir -p tmpdir/Microsoft.$1.Sdk.$2/usr/local/share/dotnet/packs/Microsoft.$1.Sdk/$2/
	$$(Q) $$(CP) -r $(DOTNET_DESTDIR)/Microsoft.$1.Sdk/ tmpdir/Microsoft.$1.Sdk.$2/usr/local/share/dotnet/packs/Microsoft.$1.Sdk/$2/
	$$(Q_GEN) pkgbuild --quiet --version '$2' --root tmpdir/Microsoft.$1.Sdk.$2 --component-plist PackageInfo.plist --install-location / --identifier com.microsoft.net.$3.sdk.pkg $$@.tmp
	$$(Q) mv $$@.tmp $$@

# The ref package
$(TMP_PKG_DIR)/Microsoft.$1.Ref.$2.pkg: $(SDK_PACK_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -f $$@
	$$(Q) rm -rf tmpdir/Microsoft.$1.Ref.$2/
	$$(Q) mkdir -p tmpdir/Microsoft.$1.Ref.$2/usr/local/share/dotnet/packs/Microsoft.$1.Ref/$2/
	$$(Q) $$(CP) -r $(DOTNET_DESTDIR)/Microsoft.$1.Ref/ tmpdir/Microsoft.$1.Ref.$2/usr/local/share/dotnet/packs/Microsoft.$1.Ref/$2/
	$$(Q_GEN) pkgbuild --quiet --version '$2' --root tmpdir/Microsoft.$1.Ref.$2 --component-plist PackageInfo.plist --install-location / --identifier com.microsoft.net.$3.ref.pkg $$@.tmp
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
$(TMP_PKG_DIR)/Microsoft.$1.Bundle.$2.pkg: $(TMP_PKG_DIR)/Microsoft.$1.Workload.$2.pkg $(TMP_PKG_DIR)/Microsoft.$1.Sdk.$2.pkg $(TMP_PKG_DIR)/Microsoft.$1.Ref.$2.pkg $(TMP_PKG_DIR)/Microsoft.$1.Templates.$2.pkg
	$$(Q) rm -f $$@
	$$(Q_GEN) productbuild \
		--quiet \
		--identifier com.microsoft.net.$3.pkg \
		--version '$2' \
		--package $(TMP_PKG_DIR)/Microsoft.$1.Workload.$2.pkg \
		--package $(TMP_PKG_DIR)/Microsoft.$1.Sdk.$2.pkg \
		--package $(TMP_PKG_DIR)/Microsoft.$1.Ref.$2.pkg \
		--package $(TMP_PKG_DIR)/Microsoft.$1.Templates.$2.pkg \
		$$@.tmp
	$$(Q) mv $$@.tmp $$@

# Copy the bundle package from our temporary directory to the target directory
$(DOTNET_PKG_DIR)/%: $(TMP_PKG_DIR)/% | $(DOTNET_PKG_DIR)
	$$(Q) $(CP) $$< $$@

PACKAGE_TARGETS += $(DOTNET_PKG_DIR)/Microsoft.$1.Bundle.$2.pkg

$(TMP_PKG_DIR)/Microsoft.$1.Bundle.$2.zip: $($(1)_NUGET_TARGETS) $(WORKLOAD_TARGETS) Makefile $(REF_PACK_$(4)) $(SDK_PACK_$(4)) $(TEMPLATE_PACKS_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -rf $$@ $$@.tmpdir $$@.tmp
	$$(Q) mkdir -p $$@.tmpdir/dotnet/sdk-manifests/$(DOTNET_MANIFEST_VERSION_WITH_PRERELEASE_COMPONENT)/
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/Microsoft.$1.Sdk
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/Microsoft.$1.Ref
	$$(Q) mkdir -p $$@.tmpdir/dotnet/template-packs
	$$(Q) $(CP) -r Workloads/Microsoft.NET.Sdk.$1 $$@.tmpdir/dotnet/sdk-manifests/$(DOTNET_MANIFEST_VERSION_WITH_PRERELEASE_COMPONENT)/microsoft.net.sdk.$3
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/Microsoft.$1.Sdk $$@.tmpdir/dotnet/packs/Microsoft.$1.Sdk/$2
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/Microsoft.$1.Ref $$@.tmpdir/dotnet/packs/Microsoft.$1.Ref/$2
	$$(Q) $(CP) $(TEMPLATE_PACKS_$(4)) $$@.tmpdir/dotnet/template-packs/$(subst +$(NUGET_BUILD_METADATA),,$(notdir $(TEMPLATE_PACKS_$(4))))
	$$(Q_GEN) cd $$@.tmpdir && zip -9rq $$(abspath $$@.tmp) .
	$$(Q) mv $$@.tmp $$@
	$$(Q) echo Created $$@

PACKAGE_TARGETS += $(DOTNET_PKG_DIR)/Microsoft.$1.Bundle.$2.zip
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call CreatePackage,$(platform),$($(platform)_NUGET_VERSION_NO_METADATA),$(shell echo $(platform) | tr A-Z a-z),$(shell echo $(platform) | tr a-z A-Z))))

define CreateWindowsBundle
$(TMP_PKG_DIR)/Microsoft.$1.Windows.Bundle.$2.zip: $($(1)_NUGET_TARGETS) $($(1)_WINDOWS_NUGET_TARGETS) $(WORKLOAD_TARGETS) Makefile $(REF_PACK_$(4)) $(SDK_PACK_$(4)) $(SDK_PACK_$(4)_WINDOWS) $(TEMPLATE_PACKS_$(4)) | $(TMP_PKG_DIR)
	$$(Q) rm -rf $$@ $$@.tmpdir $$@.tmp
	$$(Q) mkdir -p $$@.tmpdir/dotnet/sdk-manifests/$(DOTNET_MANIFEST_VERSION_WITH_PRERELEASE_COMPONENT)/
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/Microsoft.$1.Sdk
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/Microsoft.$1.Windows.Sdk
	$$(Q) mkdir -p $$@.tmpdir/dotnet/packs/Microsoft.$1.Ref
	$$(Q) mkdir -p $$@.tmpdir/dotnet/template-packs
	$$(Q) $(CP) -r Workloads/Microsoft.NET.Sdk.$1 $$@.tmpdir/dotnet/sdk-manifests/$(DOTNET_MANIFEST_VERSION_WITH_PRERELEASE_COMPONENT)/microsoft.net.sdk.$3
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/Microsoft.$1.Sdk $$@.tmpdir/dotnet/packs/Microsoft.$1.Sdk/$2
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/Microsoft.$1.Windows.Sdk $$@.tmpdir/dotnet/packs/Microsoft.$1.Windows.Sdk/$2
	$$(Q) $(CP) -r $(DOTNET_DESTDIR)/Microsoft.$1.Ref $$@.tmpdir/dotnet/packs/Microsoft.$1.Ref/$2
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
		--source $(DOTNET_NUPKG_DIR) --verbosity diag \
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

ifdef ENABLE_DOTNET
all-local:: $(TARGETS)
endif

clean-local::
	$(Q) rm -Rf $(DOTNET_NUPKG_DIR)
	$(Q) git clean -xfdq

.stamp-workaround-for-maccore-issue-2427: $(TOP)/eng/Versions.props $(LOCAL_WORKLOAD_TARGETS)
	$(Q) $(DOTNET) restore package/workaround-for-maccore-issue-2427/restore.csproj /bl:package/workaround-for-maccore-issue-2427/restore.binlog $(MSBUILD_VERBOSITY)
	$(Q) touch $@

$(DOTNET_DESTDIR)/Microsoft.iOS.Windows.Sdk/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip: .stamp-install-workloads
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
