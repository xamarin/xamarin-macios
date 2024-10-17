TOP=..
include $(TOP)/Make.config

# Keep all intermediate files always.
.SECONDARY:

ifeq ($(shell arch),arm64)
DOTNET_ARCH=arm64
else ifeq ($(shell sysctl -n sysctl.proc_translated 2>/dev/null),1)
DOTNET_ARCH=arm64
else
DOTNET_ARCH=x64
endif

##
## Mono download vs. build
##

download: download-mono
download-mono: \
	downloads/$(basename $(MONO_IOS_FILENAME)) \

downloads/$(basename $(MONO_IOS_FILENAME)): MONO_URL=$(MONO_IOS_URL)

include $(TOP)/mk/colors.mk

DOWNLOADS = \
	downloads/$(MONO_IOS_FILENAME) \

# This target downloads the mono archives, there's one for Xamarin.iOS and one for Xamarin.Mac.
# If doing many clean builds, it's possible to copy the downloaded zip file to ~/Library/Caches/xamarin-macios
# to avoid having to download it every time. The zip files have to be copied manually, otherwise
# we'd end up filling up a lot of hard drives around the world.
$(DOWNLOADS):
	$(Q) mkdir -p downloads
	$(Q) echo "Downloading $(MONO_URL)..."
	$(Q) if test -f ~/Library/Caches/xamarin-macios/$(notdir $@); then \
		echo "Found a cached version of $(MONO_URL) in ~/Library/Caches/xamarin-macios/$(notdir $@)."; \
		$(CP) ~/Library/Caches/xamarin-macios/$(notdir $@) $@.tmp; \
	else \
		EC=0; \
		$(CURL_RETRY) $(MONO_URL) --output $@.tmp || EC=$$?; \
		if [[ x$$EC == x22 ]]; then \
			MSG="Could not download the archive %s because the URL doesn't exist. This can happen if bumping mono very soon after the corresponding commit was pushed to mono (i.e. the archive hasn't been built yet). If so, please wait a bit and try again."; \
			printf "$(COLOR_RED)*** $$MSG$(COLOR_CLEAR)\n" "$(notdir $@)"; \
			if test -n "$$FAILURE_REASON_PATH"; then printf "$$MSG\n" "[$(notdir $@)]($(MONO_URL))" >> "$$FAILURE_REASON_PATH"; fi; \
		fi; \
		if [[ x$$EC != x0 ]]; then exit $$EC; fi; \
		if [[ "x$$MACIOS_CACHE_DOWNLOADS" != "x" ]]; then \
			mkdir -p ~/Library/Caches/xamarin-macios/; \
			$(CP) $@.tmp ~/Library/Caches/xamarin-macios/"$(notdir $@)"; \
			echo "Cached the download of $(notdir $@) in ~/Library/Caches/xamarin-macios"; \
		fi; \
	fi
	$(Q) mv $@.tmp $@
	$(Q) echo "Downloaded $(MONO_URL)"

downloads/%: downloads/%.7z
	$(Q) echo "Unzipping $*..."
	$(Q) rm -Rf $@.tmp
	$(Q) 7z x $< -o$@.tmp
	$(Q) find $@.tmp -exec touch {} +
	$(Q) mv $@.tmp $@
	$(Q) echo "Unzipped $*."

downloads/%: downloads/%.nupkg
	$(Q) echo "Unzipping $*..."
	$(Q) rm -Rf $@.tmp
	$(Q) unzip -d $@.tmp $<
	$(Q) find $@.tmp -exec touch {} +
	$(Q) mv $@.tmp $@
	$(Q) echo "Unzipped $*."

downloads/$(DOTNET_INSTALL_NAME): dotnet-install.sh
	$(Q) echo "Downloading and installing .NET $(DOTNET_VERSION) ($(DOTNET_ARCH)) into $@..."
	$(Q) ./dotnet-install.sh --install-dir "$@.tmp" --version "$(DOTNET_VERSION)" --architecture $(DOTNET_ARCH) --no-path $$DOTNET_INSTALL_EXTRA_ARGS
	$(Q) rm -Rf "$@"
	$(Q) mv "$@.tmp" "$@"
	$(Q) echo "Downloaded and installed .NET $(DOTNET_VERSION) into $@."

# Create a symlink with a persistent (non-version-dependent) name
# The dependency on the stamp file is to ensure the symlink is re-created
# when the .NET version changes.
all-local:: downloads/dotnet
dotnet:: downloads/dotnet
downloads/dotnet: .stamp-dotnet-symlink-$(DOTNET_VERSION)
	$(Q) mkdir -p downloads
	$(Q) rm -f $@
	$(Q) ln -s $(abspath downloads/$(DOTNET_INSTALL_NAME)) $@

.stamp-dotnet-symlink-$(DOTNET_VERSION): Makefile
	$(Q) touch $@

# This is just a helpful target to print the url to the .pkg to download and install the current .NET version into the system.
print-dotnet-pkg-urls: dotnet-install.sh
	$(Q) rm -f $@-found-it.stamp
	$(Q) for url in $$(./dotnet-install.sh --version "$(DOTNET_VERSION)" --architecture $(DOTNET_ARCH) --no-path $$DOTNET_INSTALL_EXTRA_ARGS --dry-run | grep URL.*primary: | sed 's/.*primary: //'); do \
		pkg=$${url/tar.gz/pkg}; \
		if $(CURL) -I "$$pkg" >/dev/null 2>&1; then echo "$$pkg"; touch $@-found-it.stamp; break; fi; \
	done
	$(Q) if ! test -f $@-found-it.stamp; then echo "No working urls were found."; exit 1; fi
	$(Q) rm -f $@-found-it.stamp

DOTNET_DOWNLOAD_URL?=https://dot.net/v1/dotnet-install.sh

dotnet-install.sh: Makefile
	$(Q) $(CURL_RETRY) $(DOTNET_DOWNLOAD_URL) --output $@.tmp
	$(Q) chmod +x $@.tmp
	$(Q) mv $@.tmp $@

.stamp-download-mono: $(TOP)/Make.config $(TOP)/mk/mono.mk
	$(MAKE) download-mono
	$(Q) touch $@

ifdef CUSTOM_DOTNET
DOWNLOAD_DOTNET_VERSION=$(CUSTOM_DOTNET_VERSION)
else
ifneq ($(BUNDLED_NETCORE_PLATFORMS_PACKAGE_VERSION),)
DOWNLOAD_DOTNET_VERSION=$(BUNDLED_NETCORE_PLATFORMS_PACKAGE_VERSION)
endif
endif

.stamp-install-custom-dotnet-runtime-workloads:
ifneq ($(TRACKING_DOTNET_RUNTIME_SEPARATELY),)
	@# mono toolchain
	$(Q) mkdir -p ./downloads/$(DOTNET_INSTALL_NAME)/sdk-manifests/$(MONO_TOOLCHAIN_MANIFEST_VERSION_BAND)
	$(Q) $(CP) $(TOP)/packages/microsoft.net.workload.mono.toolchain.net7.manifest-$(MONO_TOOLCHAIN_MANIFEST_VERSION_BAND)/$(DOWNLOAD_DOTNET_VERSION)/data/WorkloadManifest.* ./downloads/$(DOTNET_INSTALL_NAME)/sdk-manifests/$(MONO_TOOLCHAIN_MANIFEST_VERSION_BAND)/microsoft.net.workload.mono.toolchain.net7/
	@# emscripten, which mono depends on
	$(Q) mkdir -p ./downloads/$(DOTNET_INSTALL_NAME)/sdk-manifests/$(EMSCRIPTEN_MANIFEST_VERSION_BAND)
	$(Q) $(CP) $(TOP)/packages/microsoft.net.workload.emscripten.net7.manifest-$(EMSCRIPTEN_MANIFEST_VERSION_BAND)/$(EMSCRIPTEN_MANIFEST_PACKAGE_VERSION)/data/WorkloadManifest.* ./downloads/$(DOTNET_INSTALL_NAME)/sdk-manifests/$(EMSCRIPTEN_MANIFEST_VERSION_BAND)/microsoft.net.workload.emscripten.net7/
	$(Q) touch $@
endif

package-download/all-package-references.csproj: $(GIT_DIRECTORY)/HEAD $(GIT_DIRECTORY)/index ./create-csproj-for-all-packagereferences.sh
	$(Q_GEN) ./create-csproj-for-all-packagereferences.sh --output "$(abspath $@.tmp)" $(if $(V),-v,)
	$(Q) mv "$@.tmp" "$@"

.stamp-download-dotnet-packages: $(TOP)/Make.config downloads/$(DOTNET_INSTALL_NAME) package-download/all-package-references.csproj
	$(Q_GEN) cd package-download && $(DOTNET) \
		build \
		download-packages.csproj \
		/t:WriteBundledNETCorePlatformsPackageVersion \
		/p:WriteFilePath=$(CURDIR)/BundledNETCorePlatformsPackageVersion.txt \
		/p:PackageRuntimeIdentifiersMonoVM="$(DOTNET_MONOVM_RUNTIME_IDENTIFIERS)" \
		/p:PackageRuntimeIdentifiersCoreCLR="$(DOTNET_CORECLR_RUNTIME_IDENTIFIERS)" \
		/p:CustomDotNetVersion="$(DOWNLOAD_DOTNET_VERSION)" \
		/p:MonoToolChainManifestVersionBand="$(MONO_TOOLCHAIN_MANIFEST_VERSION_BAND)" \
		/p:EmscriptenManifestVersionBand="$(EMSCRIPTEN_MANIFEST_VERSION_BAND)" \
		/p:TrackingDotNetRuntimeSeparately="$(TRACKING_DOTNET_RUNTIME_SEPARATELY)" \
		/bl \
		$(DOTNET_BUILD_VERBOSITY)
	$(MAKE) .stamp-install-custom-dotnet-runtime-workloads
	$(Q) touch $@

.stamp-install-t4: $(TOP)/.config/dotnet-tools.json .stamp-download-dotnet-packages
	@# https://github.com/dotnet/sdk/issues/16165
	$(Q) rm -f ~/.dotnet/toolResolverCache/*/dotnet-t4
	$(Q) rm -f ~/.dotnet/toolResolverCache/*/dotnet-ilrepack
	$(DOTNET) tool restore
	$(Q) touch $@

BundledNETCorePlatformsPackageVersion.txt: .stamp-download-dotnet-packages

DOTNET_DOWNLOADS = \
	downloads/$(DOTNET_INSTALL_NAME) \
	.stamp-download-dotnet-packages \
	.stamp-install-t4 \

dotnet:: $(DOTNET_DOWNLOADS)
all-local:: $(DOTNET_DOWNLOADS)

clean-local::
	$(Q) rm -Rf downloads .stamp-download-mono

all-local:: .stamp-mono-ios-sdk-destdir

.stamp-mono-ios-sdk-destdir: .stamp-$(MONO_BUILD_MODE)
	ln -sf $(MONO_IOS_SDK_DESTDIR) mono-ios-sdk-destdir
	$(Q) touch $@

#
# .NET
#

DOTNET_COMMON_DIRECTORIES += \
	$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)) \
	$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/tools) \

DOTNET_COMMON_TARGETS = \
	$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/Versions.plist) \
	$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/tools/buildinfo) \

define VersionInfo
$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/Versions.plist: $(TOP)/builds/Versions-$(1).plist.in Makefile $(TOP)/Make.config $(TOP)/versions-check.csharp | $(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)
	$$(Q) $(TOP)/versions-check.csharp $$< "$(DOTNET_MIN_IOS_SDK_VERSION)" "$(MAX_IOS_DEPLOYMENT_TARGET)" "$(DOTNET_MIN_TVOS_SDK_VERSION)" "$(MAX_TVOS_DEPLOYMENT_TARGET)" "$(MIN_WATCH_OS_VERSION)" "$(MAX_WATCH_DEPLOYMENT_TARGET)" "$(DOTNET_MIN_MACOS_SDK_VERSION)" "$(MACOS_SDK_VERSION)" "$(DOTNET_MIN_MACCATALYST_SDK_VERSION)" "$(MACCATALYST_SDK_VERSION)"
	$$(Q_GEN) sed -e 's/@XCODE_VERSION@/$(XCODE_VERSION)/g' -e "s/@MONO_VERSION@/$(DOTNET_VERSION)/g" $$< > $$@.tmp
	$$(Q) mv $$@.tmp $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call VersionInfo,$(platform))))

define BuildInfo
$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/buildinfo: $(TOP)/Make.config.inc $(GIT_DIRECTORY)/index | $(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools
	$$(Q_GEN) echo "Version: $$($(2)_NUGET_VERSION)" > $$@.tmp
	$$(Q) echo "Hash: $$(shell git log --oneline -1 --pretty=%h)" >> $$@.tmp
	$$(Q) echo "Branch: $$(CURRENT_BRANCH)" >> $$@.tmp
	$$(Q) echo "Build date: $$(shell date '+%Y-%m-%d %H:%M:%S%z')" >> $$@.tmp
	$$(Q) mv $$@.tmp $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call BuildInfo,$(platform),$(shell echo $(platform) | tr a-z A-Z))))

$(DOTNET_COMMON_DIRECTORIES):
	$(Q) mkdir -p $@

install-dotnet: $(DOTNET_COMMON_TARGETS)
install-local:: install-dotnet
all-local:: install-dotnet

