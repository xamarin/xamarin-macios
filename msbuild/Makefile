TOP = ..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

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

ALL_SOURCES:= $(shell git ls-files | sed 's/ /\\ /g')
CONFIG      = Debug

##
## XI definitions
## 

IOS_TARGETS =                                                      \
	$(wildcard Xamarin.iOS.Tasks.Core/Xamarin.MonoTouch.*.targets) \
	$(wildcard Xamarin.iOS.Tasks.Core/Xamarin.iOS.*.props)         \
	$(wildcard Xamarin.iOS.Tasks.Core/Xamarin.iOS.*.targets)       \
	$(wildcard Xamarin.Shared/Xamarin.*.targets)       \

IOS_BINDING_TARGETS = $(wildcard Xamarin.ObjcBinding.Tasks/*.targets)

IOS_TASK_ASSEMBLIES = Xamarin.iOS.Tasks

IOS_DIRECTORIES =                                                                                           \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS                                                  \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS                                                           \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.iOS/v1.0/RedistList \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin                                \

IOS_2_1_SYMLINKS = \
	$(foreach target,$(IOS_BINDING_TARGETS)       ,$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/$(notdir $(target)))       \

# these point to a file in /Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS
IOS_X_I_SYMLINKS = \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.iOS/v1.0/RedistList/FrameworkList.xml                          \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/iOS                                                                       \

IOS_SYMLINKS = $(IOS_X_I_SYMLINKS) $(IOS_2_1_SYMLINKS)

IOS_PRODUCTS =                                                                                                         \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/FrameworkList.xml                                                       \
	$(foreach target,$(IOS_TARGETS)               ,$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/$(notdir $(target)))     \
	$(foreach target,$(IOS_BINDING_TARGETS)       ,$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/$(notdir $(target)))     \
	$(foreach dll,$(IOS_TASK_ASSEMBLIES)          ,$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/$(dll).dll)              \
	$(IOS_SYMLINKS)

all-ios: $(IOS_PRODUCTS)
symlinks-ios: $(IOS_SYMLINKS)

ifdef INCLUDE_IOS
MSBUILD_PRODUCTS += all-ios
MSBUILD_DIRECTORIES += $(IOS_DIRECTORIES)
MSBUILD_SYMLINKS += symlinks-ios
MSBUILD_TASK_ASSEMBLIES += $(IOS_TASK_ASSEMBLIES)
endif

##
## XW definitions
##

WATCH_TARGETS =                                                  \
	$(wildcard Xamarin.iOS.Tasks.Core/Xamarin.WatchOS.*.props)   \
	$(wildcard Xamarin.iOS.Tasks.Core/Xamarin.WatchOS.*.targets) \
	Xamarin.iOS.Tasks.Core/NoCode.cs                             \

WATCH_DIRECTORIES =                                                                                             \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.WatchOS/v1.0/RedistList \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS                                                       \

# XW symlinks the entire $(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS directory to 
# /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/WatchOS instead of 
# each separate file (like XI does).

WATCH_SYMLINKS =                                                                                                                  \
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
	$(wildcard Xamarin.iOS.Tasks.Core/Xamarin.TVOS.*.props)   \
	$(wildcard Xamarin.iOS.Tasks.Core/Xamarin.TVOS.*.targets) \

TVOS_DIRECTORIES =                                                                                           \
	$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.TVOS/v1.0/RedistList \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS                                                       \

# Xamarin.TVOS symlinks the entire $(MONOTOUCH_PREFIX)/lib/msbuild/TVOS directory to 
# /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/TVOS instead of 
# each separate file (like XI does).

TVOS_SYMLINKS =                                                                                                                  \
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

MAC_TARGETS = $(wildcard Xamarin.Mac.Tasks/*.props) $(wildcard Xamarin.Mac.Tasks/*.targets) $(wildcard Xamarin.Shared/Xamarin.*.targets)
MAC_BINDING_TARGETS =

MAC_TASK_ASSEMBLIES = Xamarin.Mac.Tasks

MAC_BINDING_TASK_ASSEMBLIES = 

MAC_DIRECTORIES =                                                                                           \
	$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Mac                            \
	$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.Mac/v2.0/RedistList \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild                                                  \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/RedistList                                      \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/4.5                                                 \

MAC_SYMLINKS =                                                                                                                                       \
	$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.Mac/v2.0/RedistList/FrameworkList.xml                        \
	$(foreach target,$(MAC_TARGETS)               ,$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Mac/$(notdir $(target))) \
	$(foreach target,$(MAC_BINDING_TARGETS)       ,$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Mac/$(notdir $(target))) \
	$(foreach dll,$(MAC_TASK_ASSEMBLIES)          ,$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Mac/$(dll).dll)          \
	$(foreach dll,$(MAC_BINDING_TASK_ASSEMBLIES)  ,$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Mac/$(dll).dll)          \

MAC_PRODUCTS =                                                                                                                 \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/FrameworkList.xml                                                   \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/RedistList/FrameworkList.xml                                       \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/4.5/machine.config                                                     \
	$(foreach target,$(MAC_TARGETS)               ,$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/$(notdir $(target))) \
	$(foreach target,$(MAC_BINDING_TARGETS)       ,$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/$(notdir $(target))) \
	$(foreach dll,$(MAC_TASK_ASSEMBLIES)          ,$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/$(dll).dll)          \
	$(foreach dll,$(MAC_BINDING_TASK_ASSEMBLIES)  ,$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/$(dll).dll)          \
	$(MAC_SYMLINKS)                                                                                                            \

all-mac: $(MAC_PRODUCTS)
symlinks-mac: $(MAC_SYMLINKS)

ifdef INCLUDE_MAC
MSBUILD_PRODUCTS += all-mac
MSBUILD_DIRECTORIES += $(MAC_DIRECTORIES)
MSBUILD_SYMLINKS += symlinks-mac
MSBUILD_TASK_ASSEMBLIES += $(MAC_TASK_ASSEMBLIES) $(MAC_BINDING_TASK_ASSEMBLIES)
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

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/FrameworkList.xml: Xamarin.iOS.Tasks.Core/Xamarin.iOS-FrameworkList.xml.in Makefile | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS
	$(Q) sed 's@%TargetFrameworkDirectory%@$(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS@' $< > $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/%: Xamarin.ObjcBinding.Tasks/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS
	$(Q) install -m 644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/%: Xamarin.iOS.Tasks.Core/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS
	$(Q) install -m 644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/%: Xamarin.Shared/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS
	$(Q) install -m 644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS/%.dll: build/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/iOS
	$(Q) install -m 644 $< $@

##
## XW
##

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.WatchOS/v1.0/RedistList/FrameworkList.xml: | $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.WatchOS/v1.0/RedistList
	$(Q) ln -fs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/WatchOS/$(notdir $@) $@

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/WatchOS: | $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin
	$(Q) ln -Fhs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/WatchOS $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS/FrameworkList.xml: Xamarin.iOS.Tasks.Core/Xamarin.WatchOS-FrameworkList.xml.in Makefile | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS
	$(Q) sed 's@%TargetFrameworkDirectory%@$(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.WatchOS@' $< > $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS/%: Xamarin.iOS.Tasks.Core/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS
	$(Q) install -m 644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS/%.dll: build/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/WatchOS
	$(Q) install -m 644 $< $@

##
## Xamarin.TVOS
##

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.TVOS/v1.0/RedistList/FrameworkList.xml: | $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks/Xamarin.TVOS/v1.0/RedistList
	$(Q) ln -fs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/TVOS/$(notdir $@) $@

$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin/TVOS: $(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild/Xamarin
	$(Q) ln -Fhs $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/msbuild/TVOS $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS/FrameworkList.xml: Xamarin.iOS.Tasks.Core/Xamarin.TVOS-FrameworkList.xml.in Makefile | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS
	$(Q) sed 's@%TargetFrameworkDirectory%@$(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.TVOS@' $< > $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS/%: Xamarin.iOS.Tasks.Core/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS
	$(Q) install -m 644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS/%.dll: build/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/msbuild/TVOS
	$(Q) install -m 644 $< $@

##
## XM ##
##

# Symlinks
#
# Everything in /Library/Frameworks/Mono.framework/External are links into /Library/Frameworks/Xamarin.Mac.framework/Versions/Current
# This makes it easy to switch XM, by just pointing /Library/Frameworks/Xamarin.Mac.framework/Versions/Current somewhere else

$(MAC_SYMLINKS): | $(MAC_DIRECTORIES)
	$(Q) ln -fs $(MAC_TARGETDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/msbuild/$(notdir $@) $@

# The actual content goes into /Library/Frameworks/Xamarin.Mac.framework/Versions/Current

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/FrameworkList.xml: Xamarin.Mac.Tasks/FrameworkList.xml.in Makefile | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild
	$(Q) sed 's@%TargetFrameworkDirectory%@$(MAC_TARGETDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac@' $< > $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/RedistList/FrameworkList.xml: Xamarin.Mac.Tasks/FrameworkList.xml.in Makefile | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/RedistList
	$(Q) sed 's@%TargetFrameworkDirectory%@$(MAC_TARGETDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/4.5@' $< > $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/4.5/machine.config: $(TOP)/tools/mmp/machine.4_5.config | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/4.5
	$(Q) $(CP) $(TOP)/tools/mmp/machine.4_5.config $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/%: Xamarin.Mac.Tasks/% | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild
	$(Q) install -m 644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/%: Xamarin.Shared/% | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild
	$(Q) install -m 644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild/%.dll: build/%.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/msbuild
	$(Q) install -m 644 $< $@

##
## Common targets ##
##

# we must install locally during 'make all', because the F# build depends on the msbuild targets/assemblies.
all-local:: $(MSBUILD_PRODUCTS) .stamp-test-xml

.build-stamp: $(ALL_SOURCES)
	$(Q) $(SYSTEM_MONO) /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore
	$(Q) $(SYSTEM_MSBUILD) $(XBUILD_VERBOSITY)
	$(Q) touch $@

# make all the target assemblies build when any of the sources have changed
$(foreach dll,$(MSBUILD_TASK_ASSEMBLIES),$(dll)/bin/$(CONFIG)/$(dll).dll): .build-stamp

# Always remake the symlinks
.PHONY: $(MSBUILD_SYMLINKS)

define copyToBuild
build/$(1).dll: $(1)/bin/$(CONFIG)/$(1).dll | build
	$$(Q) $(CP) $$< $$@
endef

# Create the rules to copy all the dlls into the 'build' directory. 
$(foreach dll,$(sort $(MSBUILD_TASK_ASSEMBLIES)),$(eval $(call copyToBuild,$(dll))))

# this is a directory-creating target.
$(MSBUILD_DIRECTORIES) build:
	$(Q) mkdir -p $@

install-symlinks: $(MSBUILD_SYMLINKS)

install-local:: $(MSBUILD_PRODUCTS)

.stamp-test-xml: $(wildcard */*.targets) $(wildcard */*.props)
	$(Q) xmllint --noout $^
	@echo Targets files are valid XML
	@touch $@

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# marking those files as .SECONDARY will prevent that deletion.
.SECONDARY: $(foreach file,$(MSBUILD_TASK_ASSEMBLIES),unbuild/$(file).dll) $(foreach file,$(MSBUILD_TASK_ASSEMBLIES),build/$(file).dll)
