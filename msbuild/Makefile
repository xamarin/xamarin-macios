TOP = ..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

ifdef INCLUDE_IOS
BUILD_IOS=1
endif
ifdef INCLUDE_TVOS
BUILD_IOS=1
endif
ifdef INCLUDE_MACCATALYST
BUILD_IOS=1
endif

#
# New .targets and .props files are picked up automatically if they're in a directory with already
# existing .targets or .props files. If .targets or .props are added in a new directory, consider
# changing your mind and putting them with any existing .targets or .props files.
#

##
## Common definitions
##

MSBUILD_PRODUCTS             = 
MSBUILD_DIRECTORIES          =

ALL_SOURCES:= \
	$(shell git ls-files | sed 's/ /\\ /g') \
	$(wildcard $(XAMARIN_MACDEV_PATH)/Xamarin.MacDev/*.cs) \
	$(wildcard $(XAMARIN_MACDEV_PATH)/Xamarin.MacDev/*.csproj) \
	Versions.g.cs \
	$(wildcard $(TOP)/tools/common/*.cs) \

CONFIG      = Debug
TARGETFRAMEWORK = netstandard2.0
WINDOWSRUNTIMEIDENTIFIER = win

LOCALIZATION_LANGUAGES=cs de es fr it ja ko pl pt-BR ru tr zh-Hans zh-Hant
TRANSLATED_ASSEMBLIES = $(foreach locale,$(LOCALIZATION_LANGUAGES),$(locale)/Xamarin.Localization.MSBuild)
LOCALIZATION_ASSEMBLIES = Xamarin.Localization.MSBuild

##
## iOS definitions
## 

IOS_WINDOWS_TARGETS =                                                      \
	$(wildcard Xamarin.iOS.Tasks.Windows/Xamarin.*.props)       \
	$(wildcard Xamarin.iOS.Tasks.Windows/Xamarin.*.targets)       \
	$(wildcard Xamarin.iOS.Tasks.Windows/bin/$(CONFIG)/$(TARGETFRAMEWORK)/$(WINDOWSRUNTIMEIDENTIFIER)/Xamarin.*.props)       \
	$(wildcard Xamarin.iOS.Tasks.Windows/bin/$(CONFIG)/$(TARGETFRAMEWORK)/$(WINDOWSRUNTIMEIDENTIFIER)/Xamarin.*.targets)       \

TASK_ASSEMBLIES = Xamarin.MacDev.Tasks $(LOCALIZATION_ASSEMBLIES)
IOS_WINDOWS_TASK_ASSEMBLIES = Xamarin.iOS.Tasks.Windows
IOS_WINDOWS_DEPENDENCIES = Xamarin.iOS.Windows.Client iSign.Core ws2_32 System.Diagnostics.Tracer System.Formats.Asn1 System.Buffers System.Memory System.Numerics.Vectors System.Runtime.CompilerServices.Unsafe System.Security.Cryptography.ProtectedData System.Security.Cryptography.Pkcs Microsoft.Win32.Registry System.Security.AccessControl System.Security.Principal.Windows
IOS_WINDOWS_MOBILEDEVICE_TOOLS = iMobileDevice-net bz2 getopt ideviceactivation idevicebackup idevicebackup2 idevicecrashreport idevicedate idevicedebug idevicedebugserverproxy idevicediagnostics ideviceenterrecovery ideviceimagemounter ideviceinfo ideviceinstaller idevicename idevicenotificationproxy idevicepair ideviceprovision idevicerestore idevicescreenshot idevicesyslog idevice_id imobiledevice-net-lighthouse imobiledevice ios_webkit_debug_proxy iproxy irecovery irecovery libcharset libcurl LIBEAY32 libiconv libusb-1.0 libusb0 libxml2 lzma pcre pcreposix plist plistutil plist_cmp plist_test pthreadVC3 readline SSLEAY32 usbmuxd usbmuxd vcruntime140 zip zlib1

PROPS_AND_TARGETS = \
	$(wildcard Xamarin.Shared/*.props)   \
	$(wildcard Xamarin.Shared/*.targets) \
	$(wildcard Xamarin.ObjcBinding.Tasks/*.targets) \

MSBUILD_TASK_ASSEMBLIES += $(TASK_ASSEMBLIES)

##
## .NET targets ##
##

DOTNET_SHARED_FILES = \
	$(PROPS_AND_TARGETS) \
	$(foreach dll,$(MSBUILD_TASK_ASSEMBLIES),$(dll).dll $(dll).pdb)

define InstallFiles

DOTNET_TARGETS += \
	$(foreach target,$(DOTNET_SHARED_FILES),$(DOTNET_DESTDIR)/$($(2)_NUGET_SDK_NAME)/tools/msbuild/$(notdir $(target))) \
	$(foreach dll,$(TRANSLATED_ASSEMBLIES),$(DOTNET_DESTDIR)/$($(2)_NUGET_SDK_NAME)/tools/msbuild/$(dll).resources.dll) \

DOTNET_DIRECTORIES_$(1) += \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_SDK_NAME)/tools/msbuild \
	$(foreach locale,$(LOCALIZATION_LANGUAGES),$(DOTNET_DESTDIR)/$($(2)_NUGET_SDK_NAME)/tools/msbuild/$(locale)) \

DOTNET_DIRECTORIES += $$(DOTNET_DIRECTORIES_$(1))

$(DOTNET_DESTDIR)/$($(2)_NUGET_SDK_NAME)/tools/msbuild/%: Xamarin.Shared/% | $(DOTNET_DESTDIR)/$($(2)_NUGET_SDK_NAME)/tools/msbuild
	$$(Q) install -m 644 $$< $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_SDK_NAME)/tools/msbuild/%: Xamarin.MacDev.Tasks/bin/$(CONFIG)/$(TARGETFRAMEWORK)/% | $$(DOTNET_DIRECTORIES_$(1))
	$$(Q) install -m 644 $$< $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_SDK_NAME)/tools/msbuild/%: Xamarin.Localization.MSBuild/bin/$(CONFIG)/$(TARGETFRAMEWORK)/% | $$(DOTNET_DIRECTORIES_$(1))
	$$(Q) install -m 644 $$< $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_SDK_NAME)/tools/msbuild/%: Xamarin.ObjcBinding.Tasks/% | $(DOTNET_DESTDIR)/$($(2)_NUGET_SDK_NAME)/tools/msbuild
	$$(Q) install -m 644 $$< $$@
endef

$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call InstallFiles,$(platform),$(shell echo $(platform) | tr 'a-z' 'A-Z'))))

MSBUILD_DIRECTORIES += $(DOTNET_DIRECTORIES)

MSBUILD_PRODUCTS += $(DOTNET_TARGETS)

DOTNET_IOS_WINDOWS_OUTPUT_FILES =                                \
	$(foreach dll,$(IOS_WINDOWS_TASK_ASSEMBLIES),$(dll).*)       \
	$(foreach dll,$(IOS_WINDOWS_DEPENDENCIES),$(dll).*)          \
	iSign.Core.pdb                                               \
	Xamarin.iOS.Windows.Client.pdb                               \
	Broker.zip                                                   \

DOTNET_IOS_WINDOWS_FILES = $(IOS_WINDOWS_TARGETS) $(foreach file,$(DOTNET_IOS_WINDOWS_OUTPUT_FILES),Xamarin.iOS.Tasks.Windows/bin/$(CONFIG)/$(TARGETFRAMEWORK)/$(WINDOWSRUNTIMEIDENTIFIER)/$(file))
DOTNET_IOS_WINDOWS_FILES += Messaging/Xamarin.Messaging.Build/obj/$(CONFIG)/Build.zip
DOTNET_IOS_WINDOWS_MOBILEDEVICE_TOOLS_X86 = $(foreach file,$(IOS_WINDOWS_MOBILEDEVICE_TOOLS),Xamarin.iOS.Tasks.Windows/bin/$(CONFIG)/$(TARGETFRAMEWORK)/$(WINDOWSRUNTIMEIDENTIFIER)/imobiledevice-x86/$(file).*)
DOTNET_IOS_WINDOWS_MOBILEDEVICE_TOOLS_X64 = $(foreach file,$(IOS_WINDOWS_MOBILEDEVICE_TOOLS),Xamarin.iOS.Tasks.Windows/bin/$(CONFIG)/$(TARGETFRAMEWORK)/$(WINDOWSRUNTIMEIDENTIFIER)/imobiledevice-x64/$(file).*)

.copy-windows-files: .build-stamp
	$(Q) mkdir -p $(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS
	$(Q) $(CP) $(DOTNET_IOS_WINDOWS_FILES) $(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS/
	$(Q) mkdir -p $(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS/imobiledevice-x86
	$(Q) $(CP) $(DOTNET_IOS_WINDOWS_MOBILEDEVICE_TOOLS_X86) $(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS/imobiledevice-x86/
	$(Q) mkdir -p $(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS/imobiledevice-x64
	$(Q) $(CP) $(DOTNET_IOS_WINDOWS_MOBILEDEVICE_TOOLS_X64) $(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS/imobiledevice-x64/

.dotnet-windows: .build-stamp .copy-windows-files

all-local:: .dotnet-windows
dotnet:: .dotnet-windows

##
## Common targets ##
##

# we must install locally during 'make all', because the F# build depends on the msbuild targets/assemblies.
all-local:: $(MSBUILD_PRODUCTS) .stamp-test-xml

# I haven't found a way to execute ilrepack.exe on .NET (it will probably have to be built for .NET), so run it using Mono.

.build-stamp: export SYSTEM_MONO:=$(SYSTEM_MONO)
.build-stamp: $(ALL_SOURCES)
	$(Q) $(DOTNET) build "/bl:$@.binlog" $(XBUILD_VERBOSITY)
	$(Q) touch $@

# make all the target assemblies build when any of the sources have changed
$(foreach dll,$(MSBUILD_TASK_ASSEMBLIES),$(dll)/bin/$(CONFIG)/$(TARGETFRAMEWORK)/$(dll).dll $(dll)/bin/$(CONFIG)/$(TARGETFRAMEWORK)/$(dll).pdb): .build-stamp
$(foreach dll,$(TRANSLATED_ASSEMBLIES),Xamarin.Localization.MSBuild/bin/$(CONFIG)/$(TARGETFRAMEWORK)/$(dll).resources.dll): .build-stamp

# this is a directory-creating target.
# we sort to remove duplicates
$(sort $(MSBUILD_DIRECTORIES)):
	$(Q) mkdir -p $@

install-local:: $(MSBUILD_PRODUCTS)

.stamp-test-xml: $(wildcard */*.targets) $(wildcard */*.props)
	$(Q) xmllint --noout $^
	@echo Targets files are valid XML
	@touch $@

test run-test run-tests tests run-tests-msbuild:
	$(Q) $(MAKE) -C $(TOP)/tests run-tests-msbuild

clean-local::
	git clean -xfdq
	cd $(XAMARIN_MACDEV_PATH) && git clean -xfdq

# We use the legacy Xamarin versions for the .NET assemblies as well.
Versions.g.cs: Makefile $(TOP)/Make.config.inc
	$(Q) printf "public static class VersionConstants {\\n" > $@.tmp
	$(Q) printf "\\tpublic const string XamarinMacVersion = \"$(MAC_PACKAGE_VERSION)\";\\n" >> $@.tmp
	$(Q) printf "\\tpublic const string XamarinIOSVersion = \"$(IOS_PACKAGE_VERSION)\";\\n" >> $@.tmp
	$(Q) echo $(foreach platform,$(ALL_DOTNET_PLATFORMS),"\\tpublic const string Microsoft_$(platform)_Version = \"$($(shell echo $(platform) | tr 'a-z' 'A-Z')_NUGET_VERSION_FULL)\";\\n") >> $@.tmp
	$(Q) echo $(foreach platform,$(ALL_DOTNET_PLATFORMS),"\\tpublic const string Microsoft_$(platform)_Standard_Version = \"$($(shell echo $(platform) | tr 'a-z' 'A-Z')_NUGET_VERSION_MAJOR).$($(shell echo $(platform) | tr 'a-z' 'A-Z')_NUGET_VERSION_MINOR).0.$($(shell echo $(platform) | tr 'a-z' 'A-Z')_NUGET_VERSION_PATCH)\";\\n") >> $@.tmp
	$(Q) printf "\\tpublic const string NuGetPrereleaseIdentifier = \"$(NUGET_PRERELEASE_IDENTIFIER)\";\\n" >> $@.tmp
	$(Q) printf "\\tpublic const string NuGetBuildMetadata = \"$(NUGET_BUILD_METADATA)\";\\n" >> $@.tmp
	$(Q) printf "\\tpublic const string Branch = \"$(CURRENT_BRANCH)\";\\n" >> $@.tmp
	$(Q) printf "\\tpublic const string Commit = \"$(CURRENT_HASH)\";\\n" >> $@.tmp
	$(Q) printf "}\\n" >> $@.tmp
	$(Q) mv $@.tmp $@

dotnet:: $(DOTNET_TARGETS)
