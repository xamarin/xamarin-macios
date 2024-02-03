TOP = ..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

ifdef INCLUDE_IOS
BUILD_IOS=1
endif
ifdef INCLUDE_TVOS
BUILD_IOS=1
endif
ifdef INCLUDE_WATCH
BUILD_IOS=1
endif
ifdef INCLUDE_MACCATALYST
BUILD_IOS=1
endif

#
# To add a new MSBuild assembly, add the base name to the corresponding [IOS|MAC|WATCH]_[BINDING]_TASK_ASSEMBLIES variable.
#
# New .targets and .props files are picked up automatically if they're in a directory with already
# existing .targets or .props files. If .targets or .props are added in a new directory, add that
# directory using a wildcard function to the corresponding [IOS|MAC|WATCH]_TARGETS variable.
#

##
## Common definitions
##

MSBUILD_PRODUCTS             = 
MSBUILD_DIRECTORIES          =
MSBUILD_SYMLINKS             =
MSBUILD_TASK_ASSEMBLIES      =

ALL_SOURCES:= \
	$(shell git ls-files | sed 's/ /\\ /g') \
	$(wildcard $(XAMARIN_MACDEV_PATH)/Xamarin.MacDev/*.cs) \
	$(wildcard $(XAMARIN_MACDEV_PATH)/Xamarin.MacDev/*.csproj) \
	Versions.g.cs \
	$(wildcard $(TOP)/tools/common/*.cs) \

CONFIG      = Debug
TARGETFRAMEWORK = netstandard2.0
WINDOWSRUNTIMEIDENTIFIER = win

##
## XI definitions
## 

IOS_TARGETS =                                                      \
	$(wildcard Xamarin.Shared/Xamarin.MonoTouch.*.targets) \
	$(wildcard Xamarin.Shared/Xamarin.iOS.*.props)         \
	$(wildcard Xamarin.Shared/Xamarin.iOS.*.targets)       \
	$(wildcard Xamarin.Shared/Xamarin.Shared*.props)       \
	$(wildcard Xamarin.Shared/Xamarin.Shared*.targets)       \
	$(wildcard Xamarin.Shared/Xamarin.ImplicitFacade.targets) \

IOS_WINDOWS_TARGETS =                                                      \
	$(wildcard Xamarin.iOS.Tasks.Windows/Xamarin.*.props)       \
	$(wildcard Xamarin.iOS.Tasks.Windows/Xamarin.*.targets)       \
	$(wildcard Xamarin.iOS.Tasks.Windows/bin/$(CONFIG)/$(TARGETFRAMEWORK)/$(WINDOWSRUNTIMEIDENTIFIER)/Xamarin.*.props)       \
	$(wildcard Xamarin.iOS.Tasks.Windows/bin/$(CONFIG)/$(TARGETFRAMEWORK)/$(WINDOWSRUNTIMEIDENTIFIER)/Xamarin.*.targets)       \

IOS_BINDING_TARGETS = $(wildcard Xamarin.ObjcBinding.Tasks/*.targets)

LOCALIZATION_LANGUAGES=cs de es fr it ja ko pl pt-BR ru tr zh-Hans zh-Hant
TRANSLATED_ASSEMBLIES = $(foreach locale,$(LOCALIZATION_LANGUAGES),$(locale)/Xamarin.Localization.MSBuild)
LOCALIZATION_ASSEMBLIES = Xamarin.Localization.MSBuild
IOS_TASK_ASSEMBLIES = Xamarin.iOS.Tasks $(LOCALIZATION_ASSEMBLIES)
IOS_WINDOWS_TASK_ASSEMBLIES = Xamarin.iOS.Tasks.Windows
IOS_WINDOWS_DEPENDENCIES = Xamarin.iOS.Windows.Client iSign.Core ws2_32 System.Diagnostics.Tracer BouncyCastle.Crypto System.Formats.Asn1 System.Buffers System.Memory System.Numerics.Vectors System.Runtime.CompilerServices.Unsafe System.Security.Cryptography.ProtectedData System.Security.Cryptography.Pkcs Microsoft.Win32.Registry System.Security.AccessControl System.Security.Principal.Windows
IOS_WINDOWS_MOBILEDEVICE_TOOLS = iMobileDevice-net bz2 getopt ideviceactivation idevicebackup idevicebackup2 idevicecrashreport idevicedate idevicedebug idevicedebugserverproxy idevicediagnostics ideviceenterrecovery ideviceimagemounter ideviceinfo ideviceinstaller idevicename idevicenotificationproxy idevicepair ideviceprovision idevicerestore idevicescreenshot idevicesyslog idevice_id imobiledevice-net-lighthouse imobiledevice ios_webkit_debug_proxy iproxy irecovery irecovery libcharset libcurl LIBEAY32 libiconv libusb-1.0 libusb0 libxml2 lzma pcre pcreposix plist plistutil plist_cmp plist_test pthreadVC3 readline SSLEAY32 usbmuxd usbmuxd vcruntime140 zip zlib1

IOS_DIRECTORIES =                                                                                           \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS                                                  \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/RedistList                                       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS                                                           \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.iOS/v1.0/RedistList \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin                                \
	$(foreach language,$(LOCALIZATION_LANGUAGES),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/$(language)) \

IOS_2_1_SYMLINKS = \
	$(foreach target,$(IOS_BINDING_TARGETS)       ,$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/$(notdir $(target)))       \

# these point to a file in /Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS
IOS_X_I_SYMLINKS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/RedistList/FrameworkList.xml                                                                \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.iOS/v1.0/RedistList/FrameworkList.xml                          \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/iOS                                                                       \

IOS_SYMLINKS = $(IOS_X_I_SYMLINKS) $(IOS_2_1_SYMLINKS)

IOS_PRODUCTS =                                                                                                         \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/FrameworkList.xml                                                       \
	$(foreach target,$(IOS_TARGETS)               ,$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/$(notdir $(target)))     \
	$(foreach target,$(IOS_BINDING_TARGETS)       ,$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/$(notdir $(target)))     \
	$(foreach dll,$(IOS_TASK_ASSEMBLIES)          ,$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/$(dll).dll)              \
	$(foreach dll,$(IOS_TASK_ASSEMBLIES)          ,$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/$(dll).pdb)              \
	$(foreach dll,$(TRANSLATED_ASSEMBLIES)        ,$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/$(dll).resources.dll)    \
	$(IOS_SYMLINKS)

all-ios: $(IOS_PRODUCTS)
symlinks-ios: $(IOS_SYMLINKS)

ifdef BUILD_IOS
MSBUILD_PRODUCTS += all-ios
MSBUILD_SYMLINKS += symlinks-ios
MSBUILD_TASK_ASSEMBLIES += $(IOS_TASK_ASSEMBLIES) $(IOS_WINDOWS_TASK_ASSEMBLIES)
MSBUILD_DIRECTORIES += $(IOS_DIRECTORIES)
endif


##
## MacCatalyst definitions
##

MACCATALYST_TARGETS =                                                \
	$(wildcard Xamarin.Shared/Xamarin.MacCatalyst.*.props)   \
	$(wildcard Xamarin.Shared/Xamarin.MacCatalyst.*.targets) \
	$(wildcard Xamarin.Shared/Xamarin.ImplicitFacade.targets) \

MACCATALYST_DIRECTORIES = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.MacCatalyst/RedistList                                       \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.MacCatalyst/v1.0/RedistList \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/MacCatalyst                                                       \

MACCATALYST_SYMLINKS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.MacCatalyst/RedistList/FrameworkList.xml                                                                \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.MacCatalyst/v1.0/RedistList/FrameworkList.xml                          \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/MacCatalyst                                                                       \

MACCATALYST_PRODUCTS += \
	$(MACCATALYST_SYMLINKS) \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/MacCatalyst/FrameworkList.xml                                            \
	$(foreach target,$(MACCATALYST_TARGETS),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/MacCatalyst/$(notdir $(target))) \

all-maccatalyst: $(MACCATALYST_PRODUCTS)
symlinks-maccatalyst: $(MACCATALYST_SYMLINKS)

ifdef INCLUDE_MACCATALYST
MSBUILD_PRODUCTS += all-maccatalyst
MSBUILD_DIRECTORIES += $(MACCATALYST_DIRECTORIES)
MSBUILD_SYMLINKS += symlinks-maccatalyst
endif

##
## XW definitions
##

WATCH_TARGETS =                                                  \
	$(wildcard Xamarin.Shared/Xamarin.WatchOS.*.props)   \
	$(wildcard Xamarin.Shared/Xamarin.WatchOS.*.targets) \
	Xamarin.MacDev.Tasks/NoCode.cs                             \
	$(wildcard Xamarin.Shared/Xamarin.ImplicitFacade.targets) \

WATCH_DIRECTORIES =                                                                                             \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/RedistList                                       \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.WatchOS/v1.0/RedistList \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS                                                       \

# XW symlinks the entire $(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS directory to 
# /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/WatchOS instead of 
# each separate file (like XI does).

WATCH_SYMLINKS =                                                                                                                  \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/RedistList/FrameworkList.xml                                       \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.WatchOS/v1.0/RedistList/FrameworkList.xml \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/WatchOS                                              \

WATCH_PRODUCTS =                                                                                                                \
	$(WATCH_SYMLINKS)                                                                                                           \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS/FrameworkList.xml                                                     \
	$(foreach target,$(WATCH_TARGETS)               ,$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS/$(notdir $(target))) \

all-watch: $(WATCH_PRODUCTS)
symlinks-watch: $(WATCH_SYMLINKS)

ifdef INCLUDE_WATCH
MSBUILD_PRODUCTS += all-watch
MSBUILD_DIRECTORIES += $(WATCH_DIRECTORIES)
MSBUILD_SYMLINKS += symlinks-watch
endif

##
## Xamarin.TVOS definitions
##

TVOS_TARGETS =                                                \
	$(wildcard Xamarin.Shared/Xamarin.TVOS.*.props)   \
	$(wildcard Xamarin.Shared/Xamarin.TVOS.*.targets) \
	$(wildcard Xamarin.Shared/Xamarin.ImplicitFacade.targets) \

TVOS_DIRECTORIES =                                                                                           \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/RedistList                                       \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.TVOS/v1.0/RedistList \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS                                                       \

# Xamarin.TVOS symlinks the entire $(MONOTOUCH_PREFIX)/lib/msbuild/TVOS directory to 
# /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/TVOS instead of 
# each separate file (like XI does).

TVOS_SYMLINKS =                                                                                                                  \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/RedistList/FrameworkList.xml                                       \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.TVOS/v1.0/RedistList/FrameworkList.xml \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/TVOS                                              \

TVOS_PRODUCTS =                                                                                                                \
	$(TVOS_SYMLINKS)                                                                                                           \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS/FrameworkList.xml                                                     \
	$(foreach target,$(TVOS_TARGETS)               ,$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS/$(notdir $(target))) \

all-tvos: $(TVOS_PRODUCTS)
symlinks-tvos: $(TVOS_SYMLINKS)

ifdef INCLUDE_TVOS
MSBUILD_PRODUCTS += all-tvos
MSBUILD_DIRECTORIES += $(TVOS_DIRECTORIES)
MSBUILD_SYMLINKS += symlinks-tvos
endif

##
## XM definitions
##

MAC_TARGETS = \
	$(wildcard Xamarin.Shared/Xamarin.Mac.*.props) \
	$(wildcard Xamarin.Shared/Xamarin.Mac.*.targets) \
	$(wildcard Xamarin.Shared/Xamarin.Shared*.props) \
	$(wildcard Xamarin.Shared/Xamarin.Shared*.targets) \
	$(wildcard Xamarin.Shared/Xamarin.ImplicitFacade.targets) \

MAC_BINDING_TARGETS =

MAC_TASK_ASSEMBLIES = Xamarin.Mac.Tasks $(LOCALIZATION_ASSEMBLIES)

MAC_DIRECTORIES =                                                                                           \
	$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin                                \
	$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.Mac/v2.0/RedistList \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/RedistList                              \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild                                                  \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/RedistList                                      \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/4.5                                                 \
	$(foreach language,$(LOCALIZATION_LANGUAGES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/$(language)) \

MAC_SYMLINKS =                                                                                                                                       \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/RedistList/FrameworkList.xml                                                     \
	$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.Mac/v2.0/RedistList/FrameworkList.xml                        \
	$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Mac                                                                     \

MAC_PRODUCTS =                                                                                                                 \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/FrameworkList.xml                                                   \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/RedistList/FrameworkList.xml                                       \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/4.5/machine.config                                                     \
	$(foreach target,$(MAC_TARGETS)               ,$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/$(notdir $(target))) \
	$(foreach target,$(MAC_BINDING_TARGETS)       ,$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/$(notdir $(target))) \
	$(foreach dll,$(MAC_TASK_ASSEMBLIES)          ,$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/$(dll).dll)          \
	$(foreach dll,$(MAC_TASK_ASSEMBLIES)          ,$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/$(dll).pdb)          \
	$(MAC_SYMLINKS)                                                                                                            \
	$(foreach dll,$(TRANSLATED_ASSEMBLIES)        ,$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/$(dll).resources.dll)\

all-mac: $(MAC_PRODUCTS)
symlinks-mac: $(MAC_SYMLINKS)

ifdef INCLUDE_MAC
MSBUILD_PRODUCTS += all-mac
MSBUILD_DIRECTORIES += $(MAC_DIRECTORIES)
MSBUILD_SYMLINKS += symlinks-mac
MSBUILD_TASK_ASSEMBLIES += $(MAC_TASK_ASSEMBLIES)
endif

##
## Common definitions
##

##
## XI ##
##

# Symlinks
#
# Everything in /Library/Frameworks/Mono.framework/External are links into /Library/Frameworks/Xamarin.iOS.framework/Versions/Current
# This makes it easy to switch XI, by just pointing /Library/Frameworks/Xamarin.iOS.framework/Versions/Current somewhere else

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.iOS/v1.0/RedistList/FrameworkList.xml: | $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.iOS/v1.0/RedistList
	$(Q) ln -fs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/iOS/$(notdir $@) $@

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/iOS: | $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin
	$(Q) ln -Fhs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/iOS $@

$(IOS_2_1_SYMLINKS): | $(IOS_DIRECTORIES)
	$(Q) ln -fs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/iOS/$(notdir $@) $@

# The actual content goes into /Library/Frameworks/Xamarin.iOS.framework/Versions/Current

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/FrameworkList.xml: Xamarin.Shared/Xamarin.iOS-FrameworkList.xml.in Makefile | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS
	$(Q) sed 's@%TargetFrameworkDirectory%@$(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS@' $< > $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/RedistList/FrameworkList.xml: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/FrameworkList.xml | $(IOS_DIRECTORIES)
	$(Q) ln -fs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/iOS/$(notdir $@) $@ 

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/%: Xamarin.ObjcBinding.Tasks/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS
	$(Q) install -m 644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/%: Xamarin.MacDev.Tasks/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS
	$(Q) install -m 644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/%: Xamarin.Shared/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS
	$(Q) install -m 644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/%: Xamarin.iOS.Tasks/bin/$(CONFIG)/$(TARGETFRAMEWORK)/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS $(foreach language,$(LOCALIZATION_LANGUAGES),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/$(language))
	$(Q) install -m 644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/%: Xamarin.Localization.MSBuild/bin/$(CONFIG)/$(TARGETFRAMEWORK)/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS
	$(Q) install -m 644 $< $@

##
## MacCatalyst
##

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.MacCatalyst/v1.0/RedistList/FrameworkList.xml: | $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.MacCatalyst/v1.0/RedistList
	$(Q) ln -fs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/MacCatalyst/$(notdir $@) $@

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/MacCatalyst: | $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin
	$(Q) ln -Fhs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/MacCatalyst $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/MacCatalyst/FrameworkList.xml: Xamarin.Shared/Xamarin.MacCatalyst-FrameworkList.xml.in Makefile | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/MacCatalyst
	$(Q) sed 's@%TargetFrameworkDirectory%@$(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.MacCatalyst@' $< > $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.MacCatalyst/RedistList/FrameworkList.xml: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/MacCatalyst/FrameworkList.xml | $(MACCATALYST_DIRECTORIES)
	$(Q) ln -fs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/MacCatalyst/$(notdir $@) $@ 

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/MacCatalyst/%: Xamarin.Shared/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/MacCatalyst
	$(Q) install -m 644 $< $@

##
## XW
##

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.WatchOS/v1.0/RedistList/FrameworkList.xml: | $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.WatchOS/v1.0/RedistList
	$(Q) ln -fs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/WatchOS/$(notdir $@) $@

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/WatchOS: | $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin
	$(Q) ln -Fhs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/WatchOS $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS/FrameworkList.xml: Xamarin.Shared/Xamarin.WatchOS-FrameworkList.xml.in Makefile | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS
	$(Q) sed 's@%TargetFrameworkDirectory%@$(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.WatchOS@' $< > $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/RedistList/FrameworkList.xml: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS/FrameworkList.xml | $(WATCH_DIRECTORIES)
	$(Q) ln -fs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/WatchOS/$(notdir $@) $@ 

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS/%: Xamarin.Shared/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS
	$(Q) install -m 644 $< $@

##
## Xamarin.TVOS
##

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.TVOS/v1.0/RedistList/FrameworkList.xml: | $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.TVOS/v1.0/RedistList
	$(Q) ln -fs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/TVOS/$(notdir $@) $@

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/TVOS: $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin
	$(Q) ln -Fhs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/TVOS $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS/FrameworkList.xml: Xamarin.Shared/Xamarin.TVOS-FrameworkList.xml.in Makefile | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS
	$(Q) sed 's@%TargetFrameworkDirectory%@$(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.TVOS@' $< > $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/RedistList/FrameworkList.xml: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS/FrameworkList.xml | $(TVOS_DIRECTORIES)
	$(Q) ln -fs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/TVOS/$(notdir $@) $@ 

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS/%: Xamarin.Shared/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS
	$(Q) install -m 644 $< $@

##
## XM ##
##

# Symlinks
#
# Everything in /Library/Frameworks/Mono.framework/External are links into /Library/Frameworks/Xamarin.Mac.framework/Versions/Current
# This makes it easy to switch XM, by just pointing /Library/Frameworks/Xamarin.Mac.framework/Versions/Current somewhere else

$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Mac: $(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin
	$(Q) ln -Fhs $(MAC_TARGETDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/msbuild $@

$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.Mac/v2.0/RedistList/FrameworkList.xml: | $(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.Mac/v2.0/RedistList
	$(Q) ln -fs $(MAC_TARGETDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/msbuild/$(notdir $@) $@

# The actual content goes into /Library/Frameworks/Xamarin.Mac.framework/Versions/Current

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/FrameworkList.xml: Xamarin.Shared/Xamarin.Mac-Mobile-FrameworkList.xml.in Makefile | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild
	$(Q) sed 's@%TargetFrameworkDirectory%@$(MAC_TARGETDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac@' $< > $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/RedistList/FrameworkList.xml: $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/FrameworkList.xml | $(MAC_DIRECTORIES)
	$(Q) ln -fs $(MAC_TARGETDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/msbuild/$(notdir $@) $@ 

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/RedistList/FrameworkList.xml: Xamarin.Shared/Xamarin.Mac-Full-FrameworkList.xml.in Makefile | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/RedistList
	$(Q) sed 's@%TargetFrameworkDirectory%@$(MAC_TARGETDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/4.5@' $< > $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/4.5/machine.config: $(TOP)/tools/mmp/machine.4_5.config | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/4.5
	$(Q) $(CP) $(TOP)/tools/mmp/machine.4_5.config $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/%: Xamarin.Mac.Tasks/% | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild
	$(Q) install -m 644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/%: Xamarin.Shared/% | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild
	$(Q) install -m 644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/%: Xamarin.Mac.Tasks/bin/$(CONFIG)/$(TARGETFRAMEWORK)/% | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild $(foreach language,$(LOCALIZATION_LANGUAGES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/$(language))
	$(Q) install -m 644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/%: Xamarin.Localization.MSBuild/bin/$(CONFIG)/$(TARGETFRAMEWORK)/% | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild
	$(Q) install -m 644 $< $@
##
## net5 targets ##
##

# tvOS and watchOS ships most of the iOS targets as well  (for now, we don't need all of these, so this is optimizable)
DOTNET_SHARED_FILES = $(IOS_TARGETS) $(IOS_BINDING_TARGETS) $(foreach dll,$(IOS_TASK_ASSEMBLIES),$(dll).dll $(dll).pdb)
DOTNET_IOS_FILES = $(DOTNET_SHARED_FILES) FrameworkList.xml
DOTNET_TVOS_FILES = $(TVOS_TARGETS) FrameworkList.xml
DOTNET_WATCHOS_FILES = $(WATCH_TARGETS) FrameworkList.xml
DOTNET_MACOS_FILES = $(MAC_TARGETS) $(MAC_BINDING_TARGETS) $(foreach dll,$(MAC_TASK_ASSEMBLIES),$(dll).dll $(dll).pdb) FrameworkList.xml
DOTNET_MACCATALYST_FILES = $(MACCATALYST_TARGETS) FrameworkList.xml

# iOS
ifdef INCLUDE_IOS
DOTNET_TARGETS += \
	$(foreach target,$(DOTNET_IOS_FILES)          ,$(DOTNET_DESTDIR)/$(IOS_NUGET_SDK_NAME)/tools/msbuild/iOS/$(notdir $(target)))               \

endif

# tvOS: contains all of the files for iOS as well (for now, we don't need all of them, so this is optimizable)
ifdef INCLUDE_TVOS
DOTNET_TARGETS += \
	$(foreach target,$(DOTNET_SHARED_FILES)       ,$(DOTNET_DESTDIR)/$(TVOS_NUGET_SDK_NAME)/tools/msbuild/iOS/$(notdir $(target)))              \
	$(foreach target,$(DOTNET_TVOS_FILES)         ,$(DOTNET_DESTDIR)/$(TVOS_NUGET_SDK_NAME)/tools/msbuild/tvOS/$(notdir $(target)))             \

endif

# macOS
ifdef INCLUDE_MAC
DOTNET_TARGETS += \
	$(foreach target,$(DOTNET_MACOS_FILES)        ,$(DOTNET_DESTDIR)/$(MACOS_NUGET_SDK_NAME)/tools/msbuild/macOS/$(notdir $(target)))           \

endif

# Mac Catalyst
ifdef INCLUDE_MACCATALYST
DOTNET_TARGETS += \
	$(foreach target,$(DOTNET_SHARED_FILES)       ,$(DOTNET_DESTDIR)/$(MACCATALYST_NUGET_SDK_NAME)/tools/msbuild/iOS/$(notdir $(target)))              \
	$(foreach target,$(DOTNET_MACCATALYST_FILES)  ,$(DOTNET_DESTDIR)/$(MACCATALYST_NUGET_SDK_NAME)/tools/msbuild/MacCatalyst/$(notdir $(target)))      \

endif

DOTNET_DIRECTORIES += \
	$(DOTNET_DESTDIR)/$(IOS_NUGET_SDK_NAME)/tools/msbuild/iOS \
	$(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS \
	$(DOTNET_DESTDIR)/$(TVOS_NUGET_SDK_NAME)/tools/msbuild/iOS \
	$(DOTNET_DESTDIR)/$(TVOS_NUGET_SDK_NAME)/tools/msbuild/tvOS \
	$(DOTNET_DESTDIR)/$(MACOS_NUGET_SDK_NAME)/tools/msbuild/macOS \
	$(DOTNET_DESTDIR)/$(MACCATALYST_NUGET_SDK_NAME)/tools/msbuild/iOS \
	$(DOTNET_DESTDIR)/$(MACCATALYST_NUGET_SDK_NAME)/tools/msbuild/MacCatalyst \

$(DOTNET_DESTDIR)/$(MACOS_NUGET_SDK_NAME)/tools/msbuild/macOS/%: $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/% | $(DOTNET_DIRECTORIES)
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/$(IOS_NUGET_SDK_NAME)/tools/msbuild/%: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/% | $(DOTNET_DIRECTORIES)
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/$(TVOS_NUGET_SDK_NAME)/tools/msbuild/%: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/% | $(DOTNET_DIRECTORIES)
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/$(TVOS_NUGET_SDK_NAME)/tools/msbuild/tvOS/%: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS/% | $(DOTNET_DIRECTORIES)
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/$(MACCATALYST_NUGET_SDK_NAME)/tools/msbuild/%: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/% | $(DOTNET_DIRECTORIES)
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/$(MACCATALYST_NUGET_SDK_NAME)/tools/msbuild/MacCatalyst/%: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/MacCatalyst/% | $(DOTNET_DIRECTORIES)
	$(Q) $(CP) $< $@

MSBUILD_DIRECTORIES += $(DOTNET_DIRECTORIES)

ifdef ENABLE_DOTNET
MSBUILD_PRODUCTS += $(DOTNET_TARGETS)

DOTNET_IOS_WINDOWS_OUTPUT_FILES =                                \
	$(foreach dll,$(IOS_WINDOWS_TASK_ASSEMBLIES),$(dll).*)       \
	$(foreach dll,$(IOS_WINDOWS_DEPENDENCIES),$(dll).*)          \
	iSign.Core.pdb                                               \
	Xamarin.iOS.Windows.Client.pdb                               \
	Broker.zip                                                   \
	Build.zip                                                    \

DOTNET_IOS_WINDOWS_FILES = $(IOS_WINDOWS_TARGETS) $(foreach file,$(DOTNET_IOS_WINDOWS_OUTPUT_FILES),Xamarin.iOS.Tasks.Windows/bin/$(CONFIG)/$(TARGETFRAMEWORK)/$(WINDOWSRUNTIMEIDENTIFIER)/$(file))
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
endif

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

# Always remake the symlinks
.PHONY: $(MSBUILD_SYMLINKS)

# this is a directory-creating target.
# we sort to remove duplicates, which can happen if MAC_DESTDIR and IOS_DESTDIR are the same (both create a '/Library/Frameworks/Mono.framework/External/xbuild/Xamarin' target)
$(sort $(MSBUILD_DIRECTORIES)) build:
	$(Q) mkdir -p $@

install-symlinks: $(MSBUILD_SYMLINKS)

install-local:: $(MSBUILD_PRODUCTS)

.stamp-test-xml: $(wildcard */*.targets) $(wildcard */*.props)
	$(Q) xmllint --noout $^
	@echo Targets files are valid XML
	@touch $@

test run-test: run-test-ios run-test-mac

run-test-ios:
	$(MAKE) -C $(TOP)/tests test-ios-tasks

run-test-mac:
	$(MAKE) -C $(TOP)/tests run-mac-msbuild

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

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# marking those files as .SECONDARY will prevent that deletion.
.SECONDARY: $(foreach file,$(MSBUILD_TASK_ASSEMBLIES),unbuild/$(file).dll) $(foreach file,$(MSBUILD_TASK_ASSEMBLIES),build/$(file).dll)
