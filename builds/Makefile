TOP=..
include $(TOP)/Make.config

PREFIX=$(abspath $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/)

# Keep all intermediate files always.
.SECONDARY:

ifeq ($(shell arch),arm64)
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
	downloads/$(basename $(MONO_MAC_FILENAME)) \
	downloads/$(basename $(MONO_MACCATALYST_FILENAME)) \

downloads/$(basename $(MONO_IOS_FILENAME)): MONO_URL=$(MONO_IOS_URL)
downloads/$(basename $(MONO_MAC_FILENAME)): MONO_URL=$(MONO_MAC_URL)
downloads/$(basename $(MONO_MACCATALYST_FILENAME)): MONO_URL=$(MONO_MACCATALYST_URL)

include $(TOP)/mk/colors.mk

DOWNLOADS = \
	downloads/$(MONO_IOS_FILENAME) \
	downloads/$(MONO_MAC_FILENAME) \
	downloads/$(MONO_MACCATALYST_FILENAME) \

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
	$(Q) echo "Downloading and installing .NET $(DOTNET_VERSION) into $@..."
	$(Q) ./dotnet-install.sh --install-dir "$@.tmp" --version "$(DOTNET_VERSION)" --architecture $(DOTNET_ARCH) --no-path $$DOTNET_INSTALL_EXTRA_ARGS
	$(Q) rm -Rf "$@"
	$(Q) mv "$@.tmp" "$@"
	$(Q) echo "Downloaded and installed .NET $(DOTNET_VERSION) into $@."

# This is just a helpful target to print the url to the .pkg to download and install the current .NET version into the system.
print-dotnet-pkg-urls: dotnet-install.sh
	$(Q) rm -f $@-found-it.stamp
	$(Q) for url in $$(./dotnet-install.sh --version "$(DOTNET_VERSION)" --architecture $(DOTNET_ARCH) --no-path $$DOTNET_INSTALL_EXTRA_ARGS --dry-run | grep URL.*primary: | sed 's/.*primary: //'); do \
		pkg=$${url/tar.gz/pkg}; \
		if $(CURL) -I "$$pkg" >/dev/null 2>&1; then echo "$$pkg"; touch $@-found-it.stamp; break; fi; \
	done
	$(Q) if ! test -f $@-found-it.stamp; then echo "No working urls were found."; exit 1; fi
	$(Q) rm -f $@-found-it.stamp

dotnet-install.sh: Makefile
	$(Q) $(CURL_RETRY) https://dot.net/v1/dotnet-install.sh --output $@.tmp
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
		/p:PackageRuntimeIdentifiers="$(DOTNET_RUNTIME_IDENTIFIERS)" \
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
	$(DOTNET) tool restore
	$(Q) touch $@

BundledNETCorePlatformsPackageVersion.txt: .stamp-download-dotnet-packages

DOTNET_DOWNLOADS = \
	downloads/$(DOTNET_INSTALL_NAME) \
	.stamp-download-dotnet-packages \
	.stamp-install-t4 \

dotnet: $(DOTNET_DOWNLOADS)
all-local:: $(DOTNET_DOWNLOADS)

clean-local::
	$(Q) rm -Rf downloads .stamp-download-mono

ifeq ($(MONO_BUILD_FROM_SOURCE),)

all-local:: .stamp-download-mono .stamp-mono-ios-sdk-destdir

else

# this is a list of all the files from mono we care about, so that we
# can use that list as dependencies for our makefile targets
-include .deps.mono.mk
.deps.mono.mk: $(TOP)/Make.config $(TOP)/mk/mono.mk
	$(Q) printf 'MONO_DEPENDENCIES += Makefile \\\n' > $@.tmp
	$(Q) cd $(MONO_PATH) && git ls-files --recurse-submodules 'mcs/class/*.cs' 'mcs/build/*.cs' 'external/*.cs' '*.h' '*.c' '*.cpp' | sed 's/ /\\ /g' | sed 's@^\(.*\)$$@	$(MONO_PATH)/\1 \\@' >> $(abspath $@).tmp
	$(Q) mv $@.tmp $@

#
# Configuration for the mono sdk makefiles
#
SDK_ARGS= \
	XCODE_DIR=$(XCODE_DEVELOPER_ROOT) \
	IOS_VERSION=$(IOS_SDK_VERSION) IOS_VERSION_MIN=$(MIN_IOS_SDK_VERSION) \
	TVOS_VERSION=$(TVOS_SDK_VERSION) TVOS_VERSION_MIN=$(MIN_TVOS_SDK_VERSION) \
	WATCHOS_VERSION=$(WATCH_SDK_VERSION) WATCHOS_VERSION_MIN=$(MIN_WATCHOS_SDK_VERSION) \
	WATCHOS64_32_VERSION=$(WATCH_SDK_VERSION) WATCHOS64_32_VERSION_MIN=$(MIN_WATCHOS64_32_SDK_VERSION) \
	MACOS_VERSION=$(MACOS_SDK_VERSION) MACOS_VERSION_MIN=$(MIN_MACOS_SDK_VERSION)

SDK_CONFIG = $(MONO_PATH)/sdks/Make.config
SDK_BUILDDIR = $(MONO_PATH)/sdks/builds

$(SDK_CONFIG):
	echo "ENABLE_IOS=1" >> $@
	echo "ENABLE_MAC=1" >> $@
	echo "ENABLE_MACCAT=1" >> $@


ifdef DISABLE_BUILDS_MAKEFILE_DEP
BUILDS_MAKEFILE_DEP =
else
BUILDS_MAKEFILE_DEP = Makefile
endif

.stamp-configure-mono: $(SDK_CONFIG)
	ulimit -n 4096 && $(MAKE) -C $(SDK_BUILDDIR) configure-ios configure-mac $(SDK_ARGS)
	$(Q) touch $@

.stamp-compile-mono: .stamp-configure-mono $(MONO_DEPENDENCIES) $(BUILDS_MAKEFILE_DEP) $(TOP)/Make.config $(TOP)/mk/mono.mk
	$(MAKE) -C $(SDK_BUILDDIR) package-ios package-ios-bcl $(SDK_ARGS)
	$(MAKE) -C $(SDK_BUILDDIR) package-mac package-mac-bcl $(SDK_ARGS)
	$(MAKE) -C $(SDK_BUILDDIR) package-maccat package-maccat-bcl $(SDK_ARGS)
	$(Q) touch $@

clean-local::
	$(MAKE) -C $(SDK_BUILDDIR) clean $(SDK_ARGS)
	$(Q) rm -f .stamp-compile-mono

all-local:: .stamp-compile-mono .stamp-mono-ios-sdk-destdir

endif

.stamp-mono-ios-sdk-destdir: .stamp-$(MONO_BUILD_MODE)
	ln -sf $(MONO_IOS_SDK_DESTDIR) mono-ios-sdk-destdir
	$(Q) touch $@

ifndef DISABLE_STRIP
INSTALL_STRIP_FLAG=-s
endif

#
# Xamarin.iOS/WatchOS/TVOS/Mac/MacCatalyst BCL assemblies
#
install-local:: install-bcl
all-local:: install-bcl

$(MONO_IOS_SDK_DESTDIR)/ios-bcl/%: .stamp-$(MONO_BUILD_MODE)
$(MONO_MAC_SDK_DESTDIR)/mac-bcl/%: .stamp-$(MONO_BUILD_MODE)

IOS_ASSEMBLIES =  \
	I18N \
	I18N.CJK \
	I18N.MidEast \
	I18N.Other \
	I18N.Rare \
	I18N.West \
	Microsoft.CSharp \
	Mono.CSharp \
	Mono.Data.Sqlite \
	Mono.Data.Tds \
	Mono.Security \
	mscorlib \
	System \
	System.ComponentModel.Composition \
	System.ComponentModel.DataAnnotations \
	System.Core \
	System.Data \
	System.Data.Services.Client \
	System.Data.DataSetExtensions \
	System.IdentityModel \
	System.IO.Compression \
	System.IO.Compression.FileSystem \
	System.Json \
	System.Net \
	System.Net.Http \
	System.Net.Http.WinHttpHandler \
	System.Numerics \
	System.Numerics.Vectors \
	System.Reflection.Context \
	System.Runtime.Serialization \
	System.Security \
	System.ServiceModel \
	System.ServiceModel.Internals \
	System.ServiceModel.Web \
	System.Transactions \
	System.Web.Services \
	System.Windows \
	System.Xml \
	System.Xml.Linq \
	System.Xml.Serialization

MAC_ASSEMBLIES = \
	I18N \
	I18N.CJK \
	I18N.MidEast \
	I18N.Other \
	I18N.Rare \
	I18N.West \
	mscorlib \
	Microsoft.CSharp \
	Mono.CSharp \
	Mono.CompilerServices.SymbolWriter \
	Mono.Data.Sqlite \
	Mono.Data.Tds \
	Mono.Security \
	System.ComponentModel.Composition \
	System.ComponentModel.DataAnnotations \
	System.Core \
	System.Data.Services.Client \
	System.Data.DataSetExtensions \
	System.Data \
	System.IdentityModel \
	System.IO.Compression.FileSystem \
	System.IO.Compression \
	System.Json \
	System.Net.Http \
	System.Net \
	System.Numerics \
	System.Runtime.Serialization \
	System.ServiceModel.Web \
	System.ServiceModel \
	System.ServiceModel.Internals \
	System.Transactions \
	System.Web.Services \
	System.Windows \
	System.Xml.Linq \
	System.Xml.Serialization \
	System.Xml \
	System \
	System.Security \
	System.Reflection.Context	\
	System.Net.Http.WinHttpHandler \
	System.Numerics.Vectors

MAC_4_5_ASSEMBLIES = \
	$(MAC_ASSEMBLIES) \
	Mono.Messaging \
	Mono.Posix \
	System.Configuration \
	System.Configuration.Install \
	System.Data.Linq \
	System.EnterpriseServices \
	System.IdentityModel.Selectors \
	System.Messaging \
	System.Runtime.Serialization.Formatters.Soap

COMMON_FACADE_ASSEMBLIES = \
    Microsoft.Win32.Primitives \
    Microsoft.Win32.Registry \
    Microsoft.Win32.Registry.AccessControl \
    System.AppContext \
    System.Collections \
    System.Collections.Concurrent \
    System.Collections.NonGeneric \
    System.Collections.Specialized \
    System.ComponentModel \
    System.ComponentModel.Annotations \
    System.ComponentModel.EventBasedAsync \
    System.ComponentModel.Primitives \
    System.ComponentModel.TypeConverter \
    System.Console \
    System.Data.Common \
    System.Data.SqlClient \
    System.Diagnostics.Contracts \
    System.Diagnostics.Debug \
    System.Diagnostics.FileVersionInfo \
    System.Diagnostics.Process \
    System.Diagnostics.StackTrace \
    System.Diagnostics.TextWriterTraceListener \
    System.Diagnostics.Tools \
    System.Diagnostics.TraceEvent \
    System.Diagnostics.TraceSource \
    System.Diagnostics.Tracing \
    System.Drawing.Common \
    System.Drawing.Primitives \
    System.Dynamic.Runtime \
    System.Globalization \
    System.Globalization.Calendars \
    System.Globalization.Extensions \
    System.IO \
    System.IO.Compression.ZipFile \
    System.IO.FileSystem \
    System.IO.FileSystem.AccessControl \
    System.IO.FileSystem.DriveInfo \
    System.IO.FileSystem.Primitives \
    System.IO.FileSystem.Watcher \
    System.IO.IsolatedStorage \
    System.IO.MemoryMappedFiles \
    System.IO.Pipes \
    System.IO.UnmanagedMemoryStream \
    System.Linq \
    System.Linq.Expressions \
    System.Linq.Parallel \
    System.Linq.Queryable \
    System.Net.AuthenticationManager \
    System.Net.Cache \
    System.Net.HttpListener \
    System.Net.Mail \
    System.Net.NameResolution \
    System.Net.NetworkInformation \
    System.Net.Ping \
    System.Net.Primitives \
    System.Net.Requests \
    System.Net.Security \
    System.Net.ServicePoint \
    System.Net.Sockets \
    System.Net.Utilities \
    System.Net.WebHeaderCollection \
    System.Net.WebSockets \
    System.Net.WebSockets.Client \
    System.ObjectModel \
    System.Reflection \
    System.Reflection.Emit \
    System.Reflection.Emit.ILGeneration \
    System.Reflection.Emit.Lightweight \
    System.Reflection.Extensions \
    System.Reflection.Primitives \
    System.Reflection.TypeExtensions \
    System.Resources.Reader \
    System.Resources.ReaderWriter \
    System.Resources.ResourceManager \
    System.Resources.Writer \
    System.Runtime \
    System.Runtime.CompilerServices.VisualC \
    System.Runtime.Extensions \
    System.Runtime.Handles \
    System.Runtime.InteropServices \
    System.Runtime.InteropServices.RuntimeInformation \
    System.Runtime.InteropServices.WindowsRuntime \
    System.Runtime.Numerics \
    System.Runtime.Serialization.Formatters \
    System.Runtime.Serialization.Json \
    System.Runtime.Serialization.Primitives \
    System.Runtime.Serialization.Xml \
    System.Security.AccessControl \
    System.Security.Claims \
    System.Security.Cryptography.Algorithms \
    System.Security.Cryptography.Csp \
    System.Security.Cryptography.DeriveBytes \
    System.Security.Cryptography.Encoding \
    System.Security.Cryptography.Encryption \
    System.Security.Cryptography.Encryption.Aes \
    System.Security.Cryptography.Encryption.ECDiffieHellman \
    System.Security.Cryptography.Encryption.ECDsa \
    System.Security.Cryptography.Hashing \
    System.Security.Cryptography.Hashing.Algorithms \
    System.Security.Cryptography.Primitives \
    System.Security.Cryptography.ProtectedData \
    System.Security.Cryptography.RandomNumberGenerator \
    System.Security.Cryptography.RSA \
    System.Security.Cryptography.X509Certificates \
    System.Security.Principal \
    System.Security.Principal.Windows \
    System.Security.SecureString \
    System.ServiceModel.Duplex \
    System.ServiceModel.Http \
    System.ServiceModel.NetTcp \
    System.ServiceModel.Primitives \
    System.ServiceModel.Security \
    System.ServiceProcess.ServiceController \
    System.Text.Encoding \
    System.Text.Encoding.CodePages \
    System.Text.Encoding.Extensions \
    System.Text.RegularExpressions \
    System.Threading \
    System.Threading.AccessControl \
    System.Threading.Overlapped \
    System.Threading.Tasks \
    System.Threading.Tasks.Parallel \
    System.Threading.Thread \
    System.Threading.ThreadPool \
    System.Threading.Timer \
    System.ValueTuple \
    System.Xml.ReaderWriter \
    System.Xml.XDocument \
    System.Xml.XmlDocument \
    System.Xml.XmlSerializer \
    System.Xml.XPath \
    System.Xml.XPath.XDocument \
    System.Xml.Xsl.Primitives \
    netstandard

MOBILE_ADDITIONAL_FACADE_ASSEMBLIES = \
	System.Buffers \
	System.Memory \
	System.Reflection.DispatchProxy \
	System.Runtime.Loader \
	System.Security.Cryptography.Cng \
	System.Security.Cryptography.OpenSsl \
	System.Security.Cryptography.Pkcs \
	System.Threading.Tasks.Extensions \
	System.Xml.XPath.XmlDocument

TVOS_ASSEMBLIES = $(IOS_ASSEMBLIES)
WATCHOS_ASSEMBLIES = $(filter-out Mono.Security Mono.Data.Tds,$(IOS_ASSEMBLIES))
MACCATALYST_ASSEMBLIES = $(IOS_ASSEMBLIES)

IOS_REPL_ASSEMBLIES = mscorlib System System.Core System.Xml Mono.CSharp
TVOS_REPL_ASSEMBLIES = $(IOS_REPL_ASSEMBLIES)
WATCHOS_REPL_ASSEMBLIES = $(IOS_REPL_ASSEMBLIES)
MACCATALYST_REPL_ASSEMBLIES = $(IOS_REPL_ASSEMBLIES)

IOS_FACADE_ASSEMBLIES = $(COMMON_FACADE_ASSEMBLIES) $(MOBILE_ADDITIONAL_FACADE_ASSEMBLIES)
TVOS_FACADE_ASSEMBLIES = $(COMMON_FACADE_ASSEMBLIES) $(MOBILE_ADDITIONAL_FACADE_ASSEMBLIES)
WATCHOS_FACADE_ASSEMBLIES = $(COMMON_FACADE_ASSEMBLIES) $(MOBILE_ADDITIONAL_FACADE_ASSEMBLIES)
MAC_FACADE_ASSEMBLIES = $(COMMON_FACADE_ASSEMBLIES) $(MOBILE_ADDITIONAL_FACADE_ASSEMBLIES)
MAC_4_5_FACADE_ASSEMBLIES = $(COMMON_FACADE_ASSEMBLIES) System.Net.Http.Rtc
MACCATALYST_FACADE_ASSEMBLIES = $(COMMON_FACADE_ASSEMBLIES) $(MOBILE_ADDITIONAL_FACADE_ASSEMBLIES)

IOS_BCL_DIRECTORIES = \
	$(PREFIX)/lib/mono/2.1 \
	$(PREFIX)/lib/mono/2.1/Facades \
	$(PREFIX)/lib/mono/2.1/repl \
	$(PREFIX)/lib/mono/Xamarin.iOS \
	$(PREFIX)/lib/mono/Xamarin.iOS/Facades \
	$(PREFIX)/lib/mono/Xamarin.iOS/repl

WATCHOS_BCL_DIRECTORIES = \
	$(PREFIX)/lib/mono/Xamarin.WatchOS \
	$(PREFIX)/lib/mono/Xamarin.WatchOS/Facades \
	$(PREFIX)/lib/mono/Xamarin.WatchOS/repl

TVOS_BCL_DIRECTORIES = \
	$(PREFIX)/lib/mono/Xamarin.TVOS \
	$(PREFIX)/lib/mono/Xamarin.TVOS/Facades \
	$(PREFIX)/lib/mono/Xamarin.TVOS/repl

MACCATALYST_BCL_DIRECTORIES = \
	$(PREFIX)/lib/mono/Xamarin.MacCatalyst \
	$(PREFIX)/lib/mono/Xamarin.MacCatalyst/Facades \
	$(PREFIX)/lib/mono/Xamarin.MacCatalyst/repl

MAC_BCL_DIRECTORIES = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5 \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Facades

NO_PDB_ASSEMBLIES = System.Windows System.Xml.Serialization
MAC_NO_PDB_ASSEMBLIES = System.Windows System.Xml.Serialization

IOS_BCL_TARGETS =                                                                                                  \
	$(foreach file,$(IOS_ASSEMBLIES),$(PREFIX)/lib/mono/2.1/$(file).dll)                                                \
	$(foreach file,$(filter-out $(NO_PDB_ASSEMBLIES),$(IOS_ASSEMBLIES)),$(PREFIX)/lib/mono/2.1/$(file).pdb)         \
	$(foreach file,$(IOS_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.iOS/$(file).dll)                                        \
	$(foreach file,$(filter-out $(NO_PDB_ASSEMBLIES),$(IOS_ASSEMBLIES)),$(PREFIX)/lib/mono/Xamarin.iOS/$(file).pdb) \
	$(foreach file,$(IOS_FACADE_ASSEMBLIES),$(PREFIX)/lib/mono/2.1/Facades/$(file).dll)                                 \
	$(foreach file,$(IOS_FACADE_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.iOS/Facades/$(file).dll)                         \
	$(foreach file,$(IOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/2.1/repl/$(file).dll)                                      \
	$(foreach file,$(IOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/2.1/repl/$(file).pdb)                                  \
	$(foreach file,$(IOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.iOS/repl/$(file).dll)                              \
	$(foreach file,$(IOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.iOS/repl/$(file).pdb)                          \

WATCHOS_BCL_TARGETS = \
	$(foreach file,$(WATCHOS_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.WatchOS/$(file).dll)                                        \
	$(foreach file,$(filter-out $(NO_PDB_ASSEMBLIES),$(WATCHOS_ASSEMBLIES)),$(PREFIX)/lib/mono/Xamarin.WatchOS/$(file).pdb) \
	$(foreach file,$(WATCHOS_FACADE_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.WatchOS/Facades/$(file).dll)                         \
	$(foreach file,$(WATCHOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.WatchOS/repl/$(file).dll)                              \
	$(foreach file,$(WATCHOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.WatchOS/repl/$(file).pdb)                          \

TVOS_BCL_TARGETS = \
	$(foreach file,$(TVOS_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.TVOS/$(file).dll)                                        \
	$(foreach file,$(filter-out $(NO_PDB_ASSEMBLIES),$(TVOS_ASSEMBLIES)),$(PREFIX)/lib/mono/Xamarin.TVOS/$(file).pdb) \
	$(foreach file,$(TVOS_FACADE_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.TVOS/Facades/$(file).dll)                         \
	$(foreach file,$(TVOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.TVOS/repl/$(file).dll)                              \
	$(foreach file,$(TVOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.TVOS/repl/$(file).pdb)                          \

MACCATALYST_BCL_TARGETS = \
	$(foreach file,$(MACCATALYST_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.MacCatalyst/$(file).dll)                                    \
	$(foreach file,$(filter-out $(NO_PDB_ASSEMBLIES),$(MACCATALYST_ASSEMBLIES)),$(PREFIX)/lib/mono/Xamarin.MacCatalyst/$(file).pdb) \
	$(foreach file,$(MACCATALYST_FACADE_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.MacCatalyst/Facades/$(file).dll)                     \

# the mono archive doesn't have the repl assemblies yet
MACCATALYST_BCL_TARGETS_DISABLED = \
	$(foreach file,$(MACCATALYST_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.MacCatalyst/repl/$(file).dll)                          \
	$(foreach file,$(MACCATALYST_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.MacCatalyst/repl/$(file).pdb)                          \

MAC_BCL_TARGETS = \
	$(foreach file,$(MAC_ASSEMBLIES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/$(file).dll)                \
	$(foreach file,$(filter-out $(MAC_NO_PDB_ASSEMBLIES),$(MAC_ASSEMBLIES)),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/$(file).pdb)            \
	$(foreach file,$(MAC_FACADE_ASSEMBLIES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades/$(file).dll) \
	$(foreach file,$(MAC_4_5_ASSEMBLIES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/$(file).dll)                    \
	$(foreach file,$(filter-out $(MAC_NO_PDB_ASSEMBLIES),$(MAC_4_5_ASSEMBLIES)),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/$(file).pdb)                \
	$(foreach file,$(MAC_4_5_FACADE_ASSEMBLIES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Facades/$(file).dll)     \

$(PREFIX)/lib/mono/Xamarin.iOS/Facades/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.iOS/Facades
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch/Facades/$(notdir $@) $@

$(PREFIX)/lib/mono/Xamarin.iOS/repl/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.iOS/repl
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_runtime/$(notdir $@) $@

$(PREFIX)/lib/mono/Xamarin.iOS/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.iOS
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch/$(notdir $@) $@

$(PREFIX)/lib/mono/2.1/Facades/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/2.1/Facades
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch/Facades/$(notdir $@) $@

$(PREFIX)/lib/mono/2.1/repl/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/2.1/repl
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_runtime/$(notdir $@) $@

$(PREFIX)/lib/mono/2.1/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/2.1
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch/$(notdir $@) $@

$(PREFIX)/lib/mono/Xamarin.WatchOS/Facades/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.WatchOS/Facades
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_watch/Facades/$(notdir $@) $@

$(PREFIX)/lib/mono/Xamarin.WatchOS/repl/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.WatchOS/repl
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_watch_runtime/$(notdir $@) $@

$(PREFIX)/lib/mono/Xamarin.WatchOS/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.WatchOS
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_watch/$(notdir $@) $@

$(PREFIX)/lib/mono/Xamarin.TVOS/Facades/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.TVOS/Facades
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_tv/Facades/$(notdir $@) $@

$(PREFIX)/lib/mono/Xamarin.TVOS/repl/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.TVOS/repl
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_tv_runtime/$(notdir $@) $@

$(PREFIX)/lib/mono/Xamarin.TVOS/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.TVOS
	$(Q) install -m 0755 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_tv/$(notdir $@) $@

$(PREFIX)/lib/mono/Xamarin.MacCatalyst/Facades/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.MacCatalyst/Facades
	$(Q) install -m 0755 $(MONO_MACCATALYST_SDK_DESTDIR)/maccat-bcl/monotouch/Facades/$(notdir $@) $@

$(PREFIX)/lib/mono/Xamarin.MacCatalyst/repl/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.MacCatalyst/repl
	$(Q) install -m 0755 $(MONO_MACCATALYST_SDK_DESTDIR)/maccat-bcl/monotouch_runtime/$(notdir $@) $@

$(PREFIX)/lib/mono/Xamarin.MacCatalyst/%: .stamp-$(MONO_BUILD_MODE) | $(PREFIX)/lib/mono/Xamarin.MacCatalyst
	$(Q) install -m 0755 $(MONO_MACCATALYST_SDK_DESTDIR)/maccat-bcl/monotouch/$(notdir $@) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades/%: .stamp-$(MONO_BUILD_MODE) | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades
	$(Q) install -m 0755 $(MONO_MAC_SDK_DESTDIR)/mac-bcl/xammac/Facades/$(notdir $@) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/%: .stamp-$(MONO_BUILD_MODE) | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac
	$(Q) install -m 0755 $(MONO_MAC_SDK_DESTDIR)/mac-bcl/xammac/$(notdir $@) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Facades/%: .stamp-$(MONO_BUILD_MODE) | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Facades
	$(Q) install -m 0755 $(MONO_MAC_SDK_DESTDIR)/mac-bcl/xammac_net_4_5/Facades/$(notdir $@) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/%: .stamp-$(MONO_BUILD_MODE) | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5
	$(Q) install -m 0755 $(MONO_MAC_SDK_DESTDIR)/mac-bcl/xammac_net_4_5/$(notdir $@) $@

$(IOS_BCL_DIRECTORIES) $(WATCHOS_BCL_DIRECTORIES) $(TVOS_BCL_DIRECTORIES) $(MAC_BCL_DIRECTORIES) $(MACCATALYST_BCL_DIRECTORIES):
	$(Q) mkdir -p $@

MONO_ARCHIVE_IOS_IGNORED_ASSEMBLIES = Mono.Simd System.Runtime.CompilerServices.Unsafe nunitlite  # TODO check if we should add them
MONO_ARCHIVE_MAC_IGNORED_ASSEMBLIES = System.Runtime.CompilerServices.Unsafe nunitlite  # TODO check if we should add them
MONO_ARCHIVE_MAC_4_5_IGNORED_ASSEMBLIES = SMDiagnostics System.Net.Http.WebRequest System.Runtime.CompilerServices.Unsafe nunitlite  # TODO check if we should add them

ios-assemblies-check: $(IOS_BCL_TARGETS)
	$(Q) rm -f .$@*
	$(Q) echo $(IOS_ASSEMBLIES) $(IOS_FACADE_ASSEMBLIES) $(MONO_ARCHIVE_IOS_IGNORED_ASSEMBLIES) | tr ' ' '\n' | sort > .$@1
	$(Q) ls -1 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch/Facades | grep dll$$ | sed 's/[.]dll//' | sort > .$@2
	$(Q) if ! diff -u .$@1 .$@2; then echo "\n*** There are assemblies in "$(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch" not defined in IOS_ASSEMBLIES or IOS_FACADE_ASSEMBLIES ***\n"; exit 1; fi
	$(Q) rm -f .$@*

watchos-assemblies-check: $(WATCHOS_BCL_TARGETS)
	$(Q) rm -f .$@*
	$(Q) echo $(WATCHOS_ASSEMBLIES) $(WATCHOS_FACADE_ASSEMBLIES) $(MONO_ARCHIVE_IOS_IGNORED_ASSEMBLIES) | tr ' ' '\n' | sort > .$@1
	$(Q) ls -1 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_watch $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_watch/Facades | grep dll$$ | sed 's/[.]dll//' | sort > .$@2
	$(Q) if ! diff -u .$@1 .$@2; then echo "\n*** There are assemblies in "$(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_watch" not defined in WATCHOS_ASSEMBLIES or WATCHOS_FACADE_ASSEMBLIES ***\n"; exit 1; fi
	$(Q) rm -f .$@*

tvos-assemblies-check: $(TVOS_BCL_TARGETS) 
	$(Q) rm -f .$@*
	$(Q) echo $(TVOS_ASSEMBLIES) $(TVOS_FACADE_ASSEMBLIES) $(MONO_ARCHIVE_IOS_IGNORED_ASSEMBLIES) | tr ' ' '\n' | sort > .$@1
	$(Q) ls -1 $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_tv $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_tv/Facades | grep dll$$ | sed 's/[.]dll//' | sort > .$@2
	$(Q) if ! diff -u .$@1 .$@2; then echo "\n*** There are assemblies in "$(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_tv" not defined in TVOS_ASSEMBLIES or TVOS_FACADE_ASSEMBLIES ***\n"; exit 1; fi
	$(Q) rm -f .$@*

mac-assemblies-check: $(MAC_BCL_TARGETS)
	$(Q) rm -f .$@*
	$(Q) echo $(MAC_ASSEMBLIES) $(MAC_FACADE_ASSEMBLIES) $(MONO_ARCHIVE_MAC_IGNORED_ASSEMBLIES) | tr ' ' '\n' | sort > .$@1
	$(Q) ls -1 $(MONO_MAC_SDK_DESTDIR)/mac-bcl/xammac $(MONO_MAC_SDK_DESTDIR)/mac-bcl/xammac/Facades | grep dll$$ | sed 's/[.]dll//' | sort > .$@2
	$(Q) if ! diff -u .$@1 .$@2; then echo "\n*** There are assemblies in "$(MONO_MAC_SDK_DESTDIR)/mac-bcl/xammac " not defined in MAC_ASSEMBLIES or MAC_FACADE_ASSEMBLIES ***\n"; exit 1; fi
	$(Q) rm -f .$@*

mac-45-assemblies-check: $(MAC_BCL_TARGETS)
	$(Q) rm -f .$@*
	$(Q) echo $(MAC_4_5_ASSEMBLIES) $(MAC_4_5_FACADE_ASSEMBLIES) $(MONO_ARCHIVE_MAC_4_5_IGNORED_ASSEMBLIES) | tr ' ' '\n' | sort > .$@1
	$(Q) ls -1 $(MONO_MAC_SDK_DESTDIR)/mac-bcl/xammac_net_4_5 $(MONO_MAC_SDK_DESTDIR)/mac-bcl/xammac_net_4_5/Facades | grep dll$$ | sed 's/[.]dll//' | sort > .$@2
	$(Q) if ! diff -u .$@1 .$@2; then echo "\n*** There are assemblies in "$(MONO_MAC_SDK_DESTDIR)/mac-bcl/xammac_net_4_5 " not defined in MAC_4_5_ASSEMBLIES or MAC_4_5_FACADE_ASSEMBLIES ***\n"; exit 1; fi
	$(Q) rm -f .$@*

maccatalyst-assemblies-check: $(MACCATALYST_BCL_TARGETS)
	$(Q) rm -f .$@*
	$(Q) echo $(MACCATALYST_ASSEMBLIES) $(MACCATALYST_FACADE_ASSEMBLIES) $(MONO_ARCHIVE_IOS_IGNORED_ASSEMBLIES) | tr ' ' '\n' | sort > .$@1
	$(Q) ls -1 $(MONO_MACCATALYST_SDK_DESTDIR)/maccat-bcl/monotouch $(MONO_MACCATALYST_SDK_DESTDIR)/maccat-bcl/monotouch/Facades | grep dll$$ | sed 's/[.]dll//' | sort > .$@2
	$(Q) if ! diff -u .$@1 .$@2; then echo "\n*** There are assemblies in "$(MONO_MACCATALYST_SDK_DESTDIR)/maccat-bcl/monotouch" not defined in MACCATALYST_ASSEMBLIES or MACCATALYST_FACADE_ASSEMBLIES ***\n"; exit 1; fi
	$(Q) rm -f .$@*

install-bcl-ios: $(IOS_BCL_TARGETS) ios-assemblies-check
install-bcl-tvos: $(TVOS_BCL_TARGETS) tvos-assemblies-check
install-bcl-watchos: $(WATCHOS_BCL_TARGETS) watchos-assemblies-check
install-bcl-mac: $(MAC_BCL_TARGETS) mac-assemblies-check mac-45-assemblies-check
install-bcl-maccatalyst: $(MACCATALYST_BCL_TARGETS) maccatalyst-assemblies-check

install-bcl:: install-bcl-ios
install-bcl:: install-bcl-watchos
install-bcl:: install-bcl-tvos
install-bcl:: install-bcl-mac
install-bcl:: install-bcl-maccatalyst
### TODO END

verify-signature:
	for file in $(PREFIX)/lib/mono/2.1/*.dll; do \
		MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $$file;	\
	done
	@MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $(PREFIX)/lib/mono/2.1/monotouch.dll			\
	@MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $(PREFIX)/lib/mono/2.1/MonoTouch.Dialog.dll		\
	@MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $(PREFIX)/lib/mono/2.1/MonoTouch.NUnitLite.dll	\
	@MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $(PREFIX)/lib/mono/2.1/OpenTK.dll			\
	@MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $(PREFIX)/lib/mono/2.1/OpenTK-10.dll
	for file in $$(find $(PREFIX)/lib/mono/2.1/Facades -name "*.dll"); do \
		MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $$file;	\
	done

#
# smcs
# FIXME: Needed because of broken MonoDevelop assumptions
#
install-local:: install-smcs
all-local:: install-smcs

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/smcs:
	@mkdir -p $(dir $@)
	@echo Installing smcs script
	@echo "#!/bin/bash" > $@
	@echo >> $@
	@echo "echo 'This script ($$0) is obsolete. Use csc to compile instead.'" >> $@
	@echo "exit 1" >> $@
	@chmod +x $@

install-smcs: $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/smcs

#
# Xamarin.Mac common pieces
#
install-local:: install-mac-common
all-local:: install-mac-common

MAC_COMMON_DIRECTORIES = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Versions \
	$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR) \

MAC_COMMON_TARGETS = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Versions/Current \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Commands \
	$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/buildinfo \
	$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/Version \
	$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/updateinfo \
	$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/Versions.plist \

$(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Versions/Current: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Versions
	$(Q_LN) ln -hfs $(MAC_INSTALL_VERSION) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Commands:
	$(Q_LN) ln -hfs $(MAC_TARGETDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/bin $@

$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/buildinfo: $(TOP)/Make.config.inc $(GIT_DIRECTORY)/index | $(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)
	$(Q_GEN) echo "Version: $(MAC_PACKAGE_VERSION)" > $@
	$(Q) echo "Hash: $(shell git log --oneline -1 --pretty=%h)" >> $@
	$(Q) echo "Branch: $(CURRENT_BRANCH)" >> $@
	$(Q) echo "Build date: $(shell date '+%Y-%m-%d %H:%M:%S%z')" >> $@

$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/updateinfo: $(TOP)/Make.config.inc
	$(Q) echo "0ab364ff-c0e9-43a8-8747-3afb02dc7731 $(MAC_PACKAGE_UPDATE_ID)" > $@

$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/Version: $(TOP)/Make.config.inc
	$(Q) echo $(MAC_PACKAGE_VERSION) > $@

$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/Versions.plist: $(TOP)/Versions-mac.plist.in Makefile $(TOP)/Make.config $(TOP)/mk/mono.mk $(TOP)/versions-check.csharp .stamp-$(MONO_BUILD_MODE)
	$(Q) $(TOP)/versions-check.csharp $< "$(MIN_IOS_SDK_VERSION)" "$(MAX_IOS_DEPLOYMENT_TARGET)" "$(MIN_TVOS_SDK_VERSION)" "$(MAX_TVOS_DEPLOYMENT_TARGET)" "$(MIN_WATCH_OS_VERSION)" "$(MAX_WATCH_DEPLOYMENT_TARGET)" "$(MIN_MACOS_SDK_VERSION)" "$(MACOS_SDK_VERSION)" "$(MIN_MACCATALYST_SDK_VERSION)" "$(MACCATALYST_SDK_VERSION)"
	$(Q_GEN) sed -e 's/@XCODE_VERSION@/$(XCODE_VERSION)/g' -e "s/@MONO_VERSION@/$(shell cat $(MONO_MAC_SDK_DESTDIR)/mac-mono-version.txt)/g" -e "s/@MIN_XM_MONO_VERSION@/$(MIN_XM_MONO_VERSION)/g" $< > $@

$(DOTNET_DESTDIR)/$(MACOS_NUGET_SDK_NAME)/Versions.plist: $(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/Versions.plist | $(DOTNET_DESTDIR)/$(MACOS_NUGET_SDK_NAME)
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/$(MACOS_NUGET_SDK_NAME)/tools/buildinfo: $(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/buildinfo | $(DOTNET_DESTDIR)/$(MACOS_NUGET_SDK_NAME)/tools
	$(Q) $(CP) $< $@

$(MAC_COMMON_DIRECTORIES):
	$(Q) mkdir -p $@

install-mac-common: $(MAC_COMMON_TARGETS)

#
# Xamarin.Mac Mono runtime pieces
#
install-local:: install-mac
all-local:: install-mac

MAC_DIRECTORIES = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/etc/mono/assemblies/System \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/Frameworks \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/pkgconfig \

MAC_TARGETS = \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libmonosgen-2.0.a                            \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libmono-profiler-log.a                       \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libmono-native-compat.a                      \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libmono-native-unified.a                     \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libmonosgen-2.0.dylib                        \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libMonoPosixHelper.dylib                     \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libmono-native-compat.dylib                  \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libmono-native-unified.dylib                 \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libmono-2.0.dylib                            \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libmono-2.0.a                                \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/pkgconfig/mono-2.pc                          \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/etc/mono/assemblies/System/System.config         \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mono-sgen                            \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/aarch64-darwin-mono-sgen             \

$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/%: .stamp-$(MONO_BUILD_MODE) | $(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib
	$(Q) $(CP) $(MONO_MAC_SDK_DESTDIR)/mac-libs/$(notdir $@) $@

$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libmono-2.0.dylib $(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/libmono-2.0.a: | $(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib
	$(Q) ln -sf libmonosgen-2.0$(suffix $@) $@

$(MONO_MAC_SDK_DESTDIR)/mac-pkgconfig/mono-2.pc: .stamp-$(MONO_BUILD_MODE)

$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/pkgconfig/mono-2.pc: $(MONO_MAC_SDK_DESTDIR)/mac-pkgconfig/mono-2.pc | $(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/pkgconfig
	$(Q) $(CP) $< $@

$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/etc/mono/assemblies/System/System.config: mac-System.config | $(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/etc/mono/assemblies/System
	$(Q) $(CP) $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mono-sgen: .stamp-$(MONO_BUILD_MODE) | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin
	$(Q) install -m 0755 $(MONO_MAC_SDK_DESTDIR)/mac-bin/mono-sgen $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/aarch64-darwin-mono-sgen: .stamp-$(MONO_BUILD_MODE) | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin
	$(Q) install -m 0755 $(MONO_MAC_SDK_DESTDIR)/mac-bin/aarch64-darwin-mono-sgen $@

$(MAC_DIRECTORIES):
	$(Q) mkdir -p $@

install-mac: $(MAC_TARGETS)

#
# Xamarin.iOS/WatchOS/TVOS common pieces
#
install-local:: install-ios-common
all-local:: install-ios-common

IOS_COMMON_DIRECTORIES = \
	$(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) \

IOS_COMMON_TARGETS = \
	$(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/buildinfo \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/Version \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/updateinfo \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/Versions.plist \

$(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current: | $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions
	$(Q_LN) ln -hfs $(IOS_INSTALL_VERSION) $@

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/buildinfo: $(TOP)/Make.config.inc $(GIT_DIRECTORY)/index | $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
	$(Q_GEN) echo "Version: $(IOS_PACKAGE_VERSION)" > $@
	$(Q) echo "Hash: $(shell git log --oneline -1 --pretty=%h)" >> $@
	$(Q) echo "Branch: $(CURRENT_BRANCH)" >> $@
	$(Q) echo "Build date: $(shell date '+%Y-%m-%d %H:%M:%S%z')" >> $@

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/Version: $(TOP)/Make.config.inc | $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
	$(Q) echo $(IOS_PACKAGE_VERSION) > $@

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/updateinfo: $(TOP)/Make.config.inc
	$(Q) echo "4569c276-1397-4adb-9485-82a7696df22e $(IOS_PACKAGE_UPDATE_ID)" > $@

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/Versions.plist: $(TOP)/Versions-ios.plist.in Makefile $(TOP)/Make.config $(TOP)/mk/mono.mk $(TOP)/versions-check.csharp .stamp-$(MONO_BUILD_MODE)
	$(Q) $(TOP)/versions-check.csharp $< "$(MIN_IOS_SDK_VERSION)" "$(MAX_IOS_DEPLOYMENT_TARGET)" "$(MIN_TVOS_SDK_VERSION)" "$(MAX_TVOS_DEPLOYMENT_TARGET)" "$(MIN_WATCH_OS_VERSION)" "$(MAX_WATCH_DEPLOYMENT_TARGET)" "$(MIN_MACOS_SDK_VERSION)" "$(MACOS_SDK_VERSION)" "$(MIN_MACCATALYST_SDK_VERSION)" "$(MACCATALYST_SDK_VERSION)"
	$(Q_GEN) sed -e 's/@XCODE_VERSION@/$(XCODE_VERSION)/g' -e "s/@MONO_VERSION@/$(shell cat $(MONO_IOS_SDK_DESTDIR)/ios-mono-version.txt)/g" $< > $@

define BuildInfo
$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/Versions.plist: $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/Versions.plist | $(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)
	$(Q) $(CP) $$< $$@

$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/buildinfo: $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/buildinfo | $(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools
	$(Q) $(CP) $$< $$@
endef
$(foreach platform,$(filter-out macOS,$(DOTNET_PLATFORMS)),$(eval $(call BuildInfo,$(platform))))

$(IOS_COMMON_DIRECTORIES):
	$(Q) mkdir -p $@

install-ios-common: $(IOS_COMMON_TARGETS)

#
# iPhone simulator Mono runtime pieces
#
install-local:: install-iphonesimulator
all-local:: install-iphonesimulator

IPHONESIMULATOR_DIRECTORIES = \
	$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/Frameworks \
	$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/lib

IPHONESIMULATOR_TARGETS = \
	$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/lib/libmonosgen-2.0.a                 \
	$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/lib/libmono-profiler-log.a            \
	$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/lib/libmono-native-compat.a           \
	$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/lib/libmono-native-unified.a          \
	$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/lib/libmonosgen-2.0.dylib             \
	$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/lib/libmono-native-compat.dylib       \
	$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/lib/libmono-native-unified.dylib      \
	$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/lib/libmono-profiler-log.dylib        \
	$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/Frameworks/Mono.framework/Mono        \

$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/lib/%: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/lib
	$(Q) $(CP) $(MONO_IOS_SDK_DESTDIR)/ios-libs/ios-sim/$(notdir $@) $@
	$(Q) if test -d $(MONO_IOS_SDK_DESTDIR)/ios-libs/ios-sim/$(notdir $@).dSYM; then $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-libs/ios-sim/$(notdir $@).dSYM $(dir $@); fi

$(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/Frameworks/Mono.framework/Mono: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/ios-sim/Mono.framework $(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/ios-sim/Mono.framework.dSYM $(IOS_DESTDIR)$(XAMARIN_IOSSIMULATOR_SDK)/Frameworks

$(IPHONESIMULATOR_DIRECTORIES):
	$(Q) mkdir -p $@

install-iphonesimulator:: $(IPHONESIMULATOR_TARGETS)

#
# Watch simulator Mono runtime pieces
#
install-local:: install-watchsimulator
all-local:: install-watchsimulator

WATCHSIMULATOR_DIRECTORIES = \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib

WATCHSIMULATOR_TARGETS = \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib/libmonosgen-2.0.a                \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib/libmono-profiler-log.a           \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib/libmono-native-compat.a          \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib/libmono-native-unified.a         \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib/libmonosgen-2.0.dylib            \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib/libmono-native-compat.dylib      \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib/libmono-native-unified.dylib     \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib/libmono-profiler-log.dylib       \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks/Mono.framework/Mono       \

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib/%: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib
	$(Q) $(CP) $(MONO_IOS_SDK_DESTDIR)/ios-libs/watchos-sim/$(notdir $@) $@
	$(Q) if test -d $(MONO_IOS_SDK_DESTDIR)/ios-libs/watchos-sim/$(notdir $@).dSYM; then $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-libs/watchos-sim/$(notdir $@).dSYM $(dir $@); fi

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks/Mono.framework/Mono: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/watchos-sim/Mono.framework $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/watchos-sim/Mono.framework.dSYM $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks

$(WATCHSIMULATOR_DIRECTORIES):
	$(Q) mkdir -p $@

install-watchsimulator: $(WATCHSIMULATOR_TARGETS)

#
# TV simulator Mono runtime pieces
#
install-local:: install-tvsimulator
all-local:: install-tvsimulator

TVSIMULATOR_DIRECTORIES = \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib

TVSIMULATOR_TARGETS = \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib/libmonosgen-2.0.a            \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib/libmono-profiler-log.a       \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib/libmono-native-compat.a      \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib/libmono-native-unified.a     \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib/libmonosgen-2.0.dylib        \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib/libmono-native-compat.dylib  \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib/libmono-native-unified.dylib \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib/libmono-profiler-log.dylib   \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks/Mono.framework/Mono       \

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib/%: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib
	$(Q) $(CP) $(MONO_IOS_SDK_DESTDIR)/ios-libs/tvos-sim/$(notdir $@) $@
	$(Q) if test -d $(MONO_IOS_SDK_DESTDIR)/ios-libs/tvos-sim/$(notdir $@).dSYM; then $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-libs/tvos-sim/$(notdir $@).dSYM $(dir $@); fi

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks/Mono.framework/Mono: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/tvos-sim/Mono.framework $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/tvos-sim/Mono.framework.dSYM $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks

$(TVSIMULATOR_DIRECTORIES):
	$(Q) mkdir -p $@

install-tvsimulator: $(TVSIMULATOR_TARGETS)

#
# Mac Catalyst Mono runtime pieces
#
install-local:: install-maccatalyst
all-local:: install-maccatalyst

MACCATALYST_DIRECTORIES = \
	$(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/Frameworks \
	$(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/lib \

MACCATALYST_TARGETS = \
	$(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/lib/libmonosgen-2.0.a                 \
	$(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/lib/libmono-profiler-log.a            \
	$(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/lib/libmono-native.a                  \
	$(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/lib/libmonosgen-2.0.dylib             \
	$(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/lib/libmono-native.dylib              \

$(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/lib/%: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/lib
	$(Q) $(CP) $(MONO_MACCATALYST_SDK_DESTDIR)/maccat-libs/$(notdir $@) $@
	$(Q) if test -d $(MONO_MACCATALYST_SDK_DESTDIR)/maccat-libs/$(notdir $@).dSYM; then $(CP) -R $(MONO_MACCATALYST_SDK_DESTDIR)/maccat-libs/$(notdir $@).dSYM $(dir $@); fi

$(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/Frameworks/Mono.framework/Mono: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_MACCATALYST_SDK_DESTDIR)/maccat-frameworks/ios-sim/Mono.framework $(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_MACCATALYST_SDK_DESTDIR)/maccat-frameworks/ios-sim/Mono.framework.dSYM $(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/Frameworks

$(XAMARIN_MACCATALYST_SDK_DIRECTORIES):
	$(Q) mkdir -p $@

$(MACCATALYST_DIRECTORIES):
	$(Q) mkdir -p $@

install-maccatalyst:: $(MACCATALYST_TARGETS)

#
# iPhone device Mono runtime pieces
#
install-local:: install-iphoneos
all-local:: install-iphoneos

IPHONEOS_DIRECTORIES = \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/Frameworks \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib

IPHONEOS_TARGETS =                                                     \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/libmonosgen-2.0.a                \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/libmono-profiler-log.a           \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/libmono-ee-interp.a              \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/libmono-icall-table.a            \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/libmono-ilgen.a                  \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/libmono-native-compat.a          \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/libmono-native-unified.a         \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/libmonosgen-2.0.dylib            \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/libmono-native-compat.dylib      \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/libmono-native-unified.dylib     \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/libmono-profiler-log.dylib       \
	$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/Frameworks/Mono.framework/Mono       \

$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib/%: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/lib
	$(Q) $(CP) $(MONO_IOS_SDK_DESTDIR)/ios-libs/ios/$(notdir $@) $@
	$(Q) if test -d $(MONO_IOS_SDK_DESTDIR)/ios-libs/ios/$(notdir $@).dSYM; then $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-libs/ios/$(notdir $@).dSYM $(dir $@); fi

$(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/Frameworks/Mono.framework/Mono: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/ios/Mono.framework $(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/ios/Mono.framework.dSYM $(IOS_DESTDIR)$(XAMARIN_IPHONEOS_SDK)/Frameworks

$(IPHONEOS_DIRECTORIES):
	$(Q) mkdir -p $@

install-iphoneos: $(IPHONEOS_TARGETS)

#
# Watch device Mono runtime pieces
#
install-local:: install-watchos
all-local:: install-watchos

WATCHOS_DIRECTORIES = \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib

WATCHOS_TARGETS =                                                             \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/libmonosgen-2.0.a                \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/libmono-profiler-log.a           \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/libmono-ee-interp.a              \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/libmono-icall-table.a            \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/libmono-ilgen.a                  \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/libmono-native-compat.a          \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/libmono-native-unified.a         \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/libmonosgen-2.0.dylib            \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/libmono-native-compat.dylib      \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/libmono-native-unified.dylib     \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/libmono-profiler-log.dylib       \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks/Mono.framework/Mono       \

$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/%: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib
	$(Q) $(CP) $(MONO_IOS_SDK_DESTDIR)/ios-libs/watchos/$(notdir $@) $@
	$(Q) if test -d $(MONO_IOS_SDK_DESTDIR)/ios-libs/watchos/$(notdir $@).dSYM; then $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-libs/watchos/$(notdir $@).dSYM $(dir $@); fi

$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks/Mono.framework/Mono: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/watchos/Mono.framework $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/watchos/Mono.framework.dSYM $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks

$(WATCHOS_DIRECTORIES):
	$(Q) mkdir -p $@

install-watchos: $(WATCHOS_TARGETS)

#
# TV device Mono runtime pieces
#
install-local:: install-tvos
all-local:: install-tvos

TVOS_DIRECTORIES = \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib

TVOS_TARGETS =                                                             \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/libmonosgen-2.0.a                \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/libmono-profiler-log.a           \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/libmono-ee-interp.a              \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/libmono-icall-table.a            \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/libmono-ilgen.a                  \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/libmono-native-compat.a          \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/libmono-native-unified.a         \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/libmonosgen-2.0.dylib            \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/libmono-native-compat.dylib      \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/libmono-native-unified.dylib     \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/libmono-profiler-log.dylib       \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks/Mono.framework/Mono       \

$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/%: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib
	$(Q) $(CP) $(MONO_IOS_SDK_DESTDIR)/ios-libs/tvos/$(notdir $@) $@
	$(Q) if test -d $(MONO_IOS_SDK_DESTDIR)/ios-libs/tvos/$(notdir $@).dSYM; then $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-libs/tvos/$(notdir $@).dSYM $(dir $@); fi

$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks/Mono.framework/Mono: .stamp-$(MONO_BUILD_MODE) | $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/tvos/Mono.framework $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks
	$(Q) $(CP) -R $(MONO_IOS_SDK_DESTDIR)/ios-frameworks/tvos/Mono.framework.dSYM $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks

$(TVOS_DIRECTORIES):
	$(Q) mkdir -p $@

install-tvos: $(TVOS_TARGETS)

#
# .NET
#


DOTNET_COMMON_DIRECTORIES += \
	$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)) \
	$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/tools) \

DOTNET_COMMON_TARGETS = \
	$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/Versions.plist) \
	$(foreach platform,$(DOTNET_PLATFORMS_UPPERCASE),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/tools/buildinfo) \

$(DOTNET_COMMON_DIRECTORIES):
	$(Q) mkdir -p $@

install-dotnet: $(DOTNET_COMMON_TARGETS)
install-local:: install-dotnet
all-local:: install-dotnet

#
# Cross compilers
#
# This is the cross aot version of mono.  runs on x86 but the jit/aot spits out arm.
#
# There are two versions of the cross compiler, a 32 bit version targeting arm, and a 64
# bit version targeting arm64.
# These are compiled using the mono sdk makefiles
#
install-local:: install-cross

$(MONO_IOS_SDK_DESTDIR)/ios-cross32-release/bin/arm-darwin-mono-sgen: .stamp-$(MONO_BUILD_MODE)
$(MONO_IOS_SDK_DESTDIR)/ios-cross64-release/bin/aarch64-darwin-mono-sgen: .stamp-$(MONO_BUILD_MODE)
$(MONO_IOS_SDK_DESTDIR)/ios-crosswatch-release/bin/armv7k-unknown-darwin-mono-sgen: .stamp-$(MONO_BUILD_MODE)
$(MONO_IOS_SDK_DESTDIR)/ios-crosswatch-release/bin/aarch64-apple-darwin10_ilp32-mono-sgen: .stamp-$(MONO_BUILD_MODE)

$(PREFIX)/bin/arm-darwin-mono-sgen: $(MONO_IOS_SDK_DESTDIR)/ios-cross32-release/bin/arm-darwin-mono-sgen | $(PREFIX)/bin
	$(call Q_2,INSTALL ,[CROSS]) install -c -m 0755 $(INSTALL_STRIP_FLAG) $^ $@

$(PREFIX)/bin/arm64-darwin-mono-sgen: $(MONO_IOS_SDK_DESTDIR)/ios-cross64-release/bin/aarch64-darwin-mono-sgen | $(PREFIX)/bin
	$(call Q_2,INSTALL ,[CROSS]) install -c -m 0755 $(INSTALL_STRIP_FLAG) $^ $@

$(PREFIX)/bin/armv7k-unknown-darwin-mono-sgen: $(MONO_IOS_SDK_DESTDIR)/ios-crosswatch-release/bin/armv7k-unknown-darwin-mono-sgen | $(PREFIX)/bin
	$(call Q_2,INSTALL ,[CROSS]) install -c -m 0755 $(INSTALL_STRIP_FLAG) $^ $@

$(PREFIX)/bin/arm64_32-darwin-mono-sgen: $(MONO_IOS_SDK_DESTDIR)/ios-crosswatch64_32-release/bin/aarch64-apple-darwin10_ilp32-mono-sgen | $(PREFIX)/bin
	$(call Q_2,INSTALL ,[CROSS]) install -c -m 0755 $(INSTALL_STRIP_FLAG) $^ $@

$(PREFIX)/bin:
	$(Q) mkdir -p $@

.PHONY: install-cross
install-cross: $(PREFIX)/bin/arm-darwin-mono-sgen $(PREFIX)/bin/arm64-darwin-mono-sgen $(PREFIX)/bin/armv7k-unknown-darwin-mono-sgen $(PREFIX)/bin/arm64_32-darwin-mono-sgen

#
# LLVM
#
install-local:: install-llvm

LLVM64_TARGETS = \
	$(PREFIX)/LLVM/bin/opt \
	$(PREFIX)/LLVM/bin/llc \

$(MONO_IOS_SDK_DESTDIR)/llvm-llvm64/bin/%: .stamp-$(MONO_BUILD_MODE)

$(PREFIX)/LLVM/bin/%: $(MONO_IOS_SDK_DESTDIR)/llvm-llvm64/bin/% | $(PREFIX)/LLVM/bin
	$(call Q_2,INSTALL ,[LLVM64]) install -c -m 0755 $(INSTALL_STRIP_FLAG) $^ $@

$(PREFIX)/LLVM/bin:
	$(Q) mkdir -p $@

.PHONY: install-llvm64 install-llvm
install-llvm: install-llvm64
install-llvm64: $(LLVM64_TARGETS)
