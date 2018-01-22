TOP=..
include $(TOP)/Make.config

# without this many compiler warnings about unused functions and variables
# in system headers show up.
export CCACHE_CPP2=1

#
# Common
#

SHARED_INC += \
	delegates.inc                              \

SHIPPED_HEADERS +=         \
	xamarin/mono-runtime.h \
	xamarin/xamarin.h      \
	xamarin/main.h         \
	xamarin/trampolines.h  \
	xamarin/runtime.h      \
	xamarin/runtime-generated.h \

SHARED_SOURCES += mono-runtime.m bindings.m bindings-generated.m shared.m runtime.m trampolines.m trampolines-invoke.m xamarin-support.m nsstring-localization.m trampolines-varargs.m
SHARED_I386_SOURCES += trampolines-i386.m trampolines-i386-asm.s trampolines-i386-objc_msgSend.s trampolines-i386-objc_msgSendSuper.s trampolines-i386-objc_msgSend_stret.s trampolines-i386-objc_msgSendSuper_stret.s
SHARED_X86_64_SOURCES += trampolines-x86_64.m trampolines-x86_64-asm.s trampolines-x86_64-objc_msgSend.s trampolines-x86_64-objc_msgSendSuper.s trampolines-x86_64-objc_msgSend_stret.s trampolines-x86_64-objc_msgSendSuper_stret.s
SHARED_HEADERS += shared.h product.h delegates.h runtime-internal.h $(SHARED_INC) $(SHIPPED_HEADERS) trampolines-internal.h slinked-list.h

SHARED_FILES = $(SHARED_SOURCES) $(SHARED_HEADERS) $(SHARED_I386_SOURCES) $(SHARED_X86_64_SOURCES)

EXTRA_DEPENDENCIES = $(SHARED_HEADERS)

xamarin/mono-runtime.h: mono-runtime.h.t4 exports.t4
	$(Q_GEN) $(TT) $< -o $@

xamarin/runtime-generated.h: runtime-generated.h.t4 delegates.t4
	$(Q_GEN) $(TT) $< -o $@

mono-runtime.m: mono-runtime.m.t4 exports.t4
	$(Q_GEN) $(TT) $< -o $@

delegates.%: delegates.%.t4 delegates.t4
	$(Q_GEN) $(TT) $< -o $@

Delegates.generated.cs: Delegates.cs.t4 delegates.t4
	$(Q_GEN) $(TT) $< -o $@

bindings-generator.exe: bindings-generator.cs
	$(Q) $(SYSTEM_MCS) $< -o:$@ -debug:full

bindings-generated.m: bindings-generator.exe
	$(Q_GEN) $(SYSTEM_MONO) --debug $< $@

zlib-helper.c: $(MONO_PATH)/support/zlib-helper.c
	$(Q) ln -fs $< $@

# our makefiles don't support building the same source file multiple times with different defines,
# so just symlink extension-main.m to another file and compile that instead.
app-main.m watchextension-main.m tvextension-main.m: extension-main.m
	$(Q_LN) ln -fs $< $@

#
# MonoTouch defines (used for all MonoTouch platforms: iOS, WatchOS and TVOS)
#

MONOTOUCH_SOURCES =     \
	monotouch-debug.m   \
	monotouch-main.m    \

MONOTOUCH_HEADERS =     \
    monotouch-debug.h   \

MONOTOUCH_LIBS =               \
	libextension.a             \
	libwatchextension.a        \
	libtvextension.a           \
	libapp.a                   \
	libxamarin.a               \
	libxamarin-debug.a         \
	libxamarin.dylib           \
	libxamarin-debug.dylib     \

MONOTOUCH_FRAMEWORKS =  \
	Xamarin             \
	Xamarin-debug       \

MONOTOUCH_SOURCE_STEMS = $(patsubst %.s,%,$(patsubst %.m,%,$(SHARED_SOURCES) $(MONOTOUCH_SOURCES))) zlib-helper
MONOTOUCH_I386_SOURCE_STEMS = $(patsubst %.s,%,$(patsubst %.m,%,$(SHARED_I386_SOURCES)))
MONOTOUCH_X86_64_SOURCE_STEMS = $(patsubst %.s,%,$(patsubst %.m,%,$(SHARED_X86_64_SOURCES)))

#
# PlatformTemplate contains the install targets and sets up some variables
#

define PlatformTemplate

$(2)_SOURCES       = $(MONOTOUCH_SOURCES)
$(2)_HEADERS       = $(MONOTOUCH_HEADERS)
$(2)_SOURCE_STEMS  = $(MONOTOUCH_SOURCE_STEMS)
$(2)_X86_64_SOURCE_STEMS = $(MONOTOUCH_X86_64_SOURCE_STEMS)
$(2)_I386_SOURCE_STEMS = $(MONOTOUCH_I386_SOURCE_STEMS)
$(2)_LIBRARIES     = $(MONOTOUCH_LIBS)
ifdef INCLUDE_DEVICE
$(2)_DEVICE_ARCHITECTURES = $(3)
endif
$(2)_SIM_ARCHITECTURES = $(4)
$(2)_ARCHITECTURES = $$($(2)_DEVICE_ARCHITECTURES) $$($(2)_SIM_ARCHITECTURES)
$(2)_FRAMEWORKS    = $(MONOTOUCH_FRAMEWORKS)

RUNTIME_$(2)_TARGETS_DIRS +=                                        \
	$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/Frameworks/Xamarin.framework \
	$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/Frameworks/Xamarin-debug.framework \
	$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/usr/lib             \
	$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/usr/include/xamarin \
	$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/Frameworks/Xamarin.framework \
	$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/Frameworks/Xamarin-debug.framework \
	$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/usr/lib                    \
	$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/usr/include/xamarin        \

RUNTIME_$(2)_TARGETS +=                                                                                \
	$$(foreach file,$$($(2)_FRAMEWORKS),$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/Frameworks/$$(file).framework/$$(file))   \
	$$(foreach file,$$($(2)_FRAMEWORKS),$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/Frameworks/$$(file).framework/Info.plist) \
	$$(foreach file,$$($(2)_LIBRARIES),$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/usr/lib/$$(file))   \
	$(foreach file,$(SHIPPED_HEADERS),$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/usr/include/$(file)) \

ifdef INCLUDE_DEVICE
RUNTIME_$(2)_TARGETS +=                                                                                \
	$$(foreach file,$$($(2)_FRAMEWORKS),$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/Frameworks/$$(file).framework/$$(file))   \
	$$(foreach file,$$($(2)_FRAMEWORKS),$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/Frameworks/$$(file).framework/Info.plist) \
	$$(foreach file,$$($(2)_LIBRARIES),$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/usr/lib/$$(file))          \
	$(foreach file,$(SHIPPED_HEADERS),$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/usr/include/$(file))        \

endif

$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/usr/lib/%.a: .libs/$(1)/%.a | $(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/usr/lib
	$(Q) install -m 0644 $$< $$@

$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/Frameworks/Xamarin.framework/Xamarin: .libs/$(1)/Xamarin-sim.framework | $(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/Frameworks/Xamarin.framework
	$(Q) cp $$< $$@

$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/Frameworks/Xamarin-debug.framework/Xamarin-debug: .libs/$(1)/Xamarin-debug-sim.framework | $(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/Frameworks/Xamarin-debug.framework
	$(Q) cp $$< $$@

$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/Frameworks/%.framework/Info.plist: Xamarin.framework-$(1).Info.plist | $(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/Frameworks/%.framework
	$(Q) sed 's/@BUNDLE_EXECUTABLE@/$$*/' $$< > $$@

ifdef INCLUDE_DEVICE
$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/usr/lib/%.a: .libs/$(1)/%.a | $(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/usr/lib
	$(Q) install -m 0644 $$< $$@

$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/Frameworks/Xamarin.framework/Xamarin: .libs/$(1)/Xamarin-dev.framework | $(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/Frameworks/Xamarin.framework
	$(Q) cp $$< $$@

$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/Frameworks/Xamarin-debug.framework/Xamarin-debug: .libs/$(1)/Xamarin-debug-dev.framework | $(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/Frameworks/Xamarin-debug.framework
	$(Q) cp $$< $$@

$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/Frameworks/%.framework/Info.plist: Xamarin.framework-$(1).Info.plist | $(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/Frameworks/%.framework
	$(Q) sed 's/@BUNDLE_EXECUTABLE@/$$*/' $$< > $$@
endif

$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/usr/lib/%.dylib: .libs/$(1)/%-sim.dylib | $(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/usr/lib
	$(Q) install -m 0644 $$< $$@

ifdef INCLUDE_DEVICE
$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/usr/lib/%.dylib: .libs/$(1)/%-dev.dylib | $(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/usr/lib
ifeq (watchos,$(1))
	$$(Q_STRIP) $(DEVICE_BIN_PATH)/bitcode_strip $$< -m -o $$@
else
	$(Q) install -m 0644 $$< $$@
endif
endif

$(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/usr/include/%.h: %.h | $(IOS_DESTDIR)$$(XAMARIN_$(5)SIMULATOR_SDK)/usr/include/xamarin
	$(Q) install -m 0644 $$< $$@

ifdef INCLUDE_DEVICE
$(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/usr/include/%.h: %.h | $(IOS_DESTDIR)$$(XAMARIN_$(6)OS_SDK)/usr/include/xamarin
	$(Q) install -m 0644 $$< $$@
endif

$$(RUNTIME_$(2)_TARGETS_DIRS):
	$(Q) mkdir -p $$@

all-$(5): $$(RUNTIME_$(2)_TARGETS)
all-local:: $$(RUNTIME_$(2)_TARGETS)
install-local:: $$(RUNTIME_$(2)_TARGETS)

endef

# 1: platform name
# 2: variable prefix
# 3: device architectures
# 4: sim architectures
# 5: simulator variable name used in some places
# 6: device variable name used in some places
ifdef INCLUDE_IOS
$(eval $(call PlatformTemplate,ios,IOS,armv7 armv7s arm64,x86 x86_64,IOS,IPHONE))
endif

ifdef INCLUDE_WATCH
$(eval $(call PlatformTemplate,watchos,WATCHOS,armv7k,x86,WATCH,WATCH))
endif

ifdef INCLUDE_TVOS
$(eval $(call PlatformTemplate,tvos,TVOS,arm64,x86_64,TV,TV))
endif

#
# LibTemplate we build three different libraries from the same source code,
# libapp.a, libextension.a and libwatchextension.a.
#
# They're all built from the same soure file (extension-main.m), but with different defines:
# libextension.a has EXTENSION defined, while libwatchextension.a has WATCH_EXTENSION defined.
#

define LibTemplate
$$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/extension-main.$$(arch).o): EXTRA_DEFINES=-DEXTENSION
$$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/watchextension-main.$$(arch).o): EXTRA_DEFINES=-DWATCH_EXTENSION
$$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/tvextension-main.$$(arch).o): EXTRA_DEFINES=-DTV_EXTENSION

.libs/$(1)/libextension.%.a: .libs/$(1)/extension-main.%.o
	$(Q) rm -f $$@
	$$(call Q_2,AR,    [$1]) $(DEVICE_BIN_PATH)/ar cru $$@ $$^

.libs/$(1)/libapp.%.a:       .libs/$(1)/app-main.%.o
	$(Q) rm -f $$@
	$$(call Q_2,AR,    [$1]) $(DEVICE_BIN_PATH)/ar cru $$@ $$^

.libs/$(1)/libextension.a: $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/libextension.$$(arch).a)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@

.libs/$(1)/libapp.a:       $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/libapp.$$(arch).a)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@

.libs/$(1)/libwatchextension.%.a: .libs/$(1)/watchextension-main.%.o
	$(Q) rm -f $$@
	$$(call Q_2,AR,    [$1]) $(DEVICE_BIN_PATH)/ar cru $$@ $$^

.libs/$(1)/libwatchextension.a: $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/libwatchextension.$$(arch).a)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@

.libs/$(1)/libtvextension.%.a: .libs/$(1)/tvextension-main.%.o
	$(Q) rm -f $$@
	$$(call Q_2,AR,    [$1]) $(DEVICE_BIN_PATH)/ar cru $$@ $$^

.libs/$(1)/libtvextension.a: $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/libtvextension.$$(arch).a)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@

.libs/$(1)/libxamarin-sim.dylib:  $$(foreach arch,$$($(2)_SIM_ARCHITECTURES),.libs/$(1)/libxamarin.$$(arch).dylib)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
	$(Q) install_name_tool -id @rpath/libxamarin.dylib $$@
	$(Q) install_name_tool -change @rpath/ @rpath/libmonosgen-2.0.dylib $$@

.libs/$(1)/libxamarin-debug-sim.dylib:  $$(foreach arch,$$($(2)_SIM_ARCHITECTURES),.libs/$(1)/libxamarin-debug.$$(arch).dylib)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
	$(Q) install_name_tool -id @rpath/libxamarin-debug.dylib $$@
	$(Q) install_name_tool -change @rpath/ @rpath/libmonosgen-2.0.dylib $$@

.libs/$(1)/libxamarin-dev.dylib:  $$(foreach arch,$$($(2)_DEVICE_ARCHITECTURES),.libs/$(1)/libxamarin.$$(arch).dylib)
	$(Q) rm -f $$@
ifeq (1,$$(words $$($(2)_DEVICE_ARCHITECTURES)))
	$(Q) cp $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif
	$(Q) install_name_tool -id @rpath/libxamarin.dylib $$@
	$(Q) install_name_tool -change @rpath/ @rpath/libmonosgen-2.0.dylib $$@

.libs/$(1)/libxamarin-debug-dev.dylib:  $$(foreach arch,$$($(2)_DEVICE_ARCHITECTURES),.libs/$(1)/libxamarin-debug.$$(arch).dylib)
	$(Q) rm -f $$@
ifeq (1,$$(words $$($(2)_DEVICE_ARCHITECTURES)))
	$(Q) cp $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif
	$(Q) install_name_tool -id @rpath/libxamarin-debug.dylib $$@
	$(Q) install_name_tool -change @rpath/ @rpath/libmonosgen-2.0.dylib $$@

.libs/$(1)/Xamarin-dev.framework:  $$(foreach arch,$$($(2)_DEVICE_ARCHITECTURES),.libs/$(1)/Xamarin.$$(arch).framework)
	$(Q) rm -f $$@
ifeq (1,$$(words $$($(2)_DEVICE_ARCHITECTURES)))
	$(Q) cp $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif

.libs/$(1)/Xamarin-debug-dev.framework:  $$(foreach arch,$$($(2)_DEVICE_ARCHITECTURES),.libs/$(1)/Xamarin-debug.$$(arch).framework)
	$(Q) rm -f $$@
ifeq (1,$$(words $$($(2)_DEVICE_ARCHITECTURES)))
	$(Q) cp $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif

.libs/$(1)/Xamarin-sim.framework:  $$(foreach arch,$$($(2)_SIM_ARCHITECTURES),.libs/$(1)/Xamarin.$$(arch).framework)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@

.libs/$(1)/Xamarin-debug-sim.framework:  $$(foreach arch,$$($(2)_SIM_ARCHITECTURES),.libs/$(1)/Xamarin-debug.$$(arch).framework)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@

.SECONDARY: $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/app-main.$$(arch).o)
endef

$(eval $(call LibTemplate,ios,IOS))
$(eval $(call LibTemplate,watchos,WATCHOS))
$(eval $(call LibTemplate,tvos,TVOS))

#
# LibXamarinTemplate (and LibXamarinArchTemplate) builds libxamarin.a
#

define LibXamarinArchTemplate
.libs/$(1)/libxamarin$(4).$(5).a: $$($(5)_$(1)$(3)_OBJECTS)
	$$(Q) rm -f $$@
	$$(call Q_2,AR,    [$1]) $(DEVICE_BIN_PATH)/ar Scru $$@ $$^
	$$(call Q_2,RANLIB,[$1]) $(DEVICE_BIN_PATH)/ranlib -no_warning_for_no_symbols -q $$@
endef

define LibXamarinTemplate

$(1)$(3)_COMMON_DYLIB_FLAGS = -lmonosgen-2.0 -Wl,-install_name,libxamarin$(4).dylib -framework Foundation -framework CFNetwork -framework UIKit -lz
$(1)$(3)_COMMON_FRAMEWORK_FLAGS = -framework Mono -Wl,-install_name,@rpath/Xamarin$(4).framework/Xamarin$(4) -framework Foundation -framework CFNetwork -framework UIKit -lz

x86_$(1)$(3)_OBJECTS    = $$(patsubst %,.libs/$(1)/%$(4).x86.o,$$($(2)_SOURCE_STEMS))    $$(patsubst %,.libs/$(1)/%$(4).x86.o,$$($(2)_I386_SOURCE_STEMS))
x86_64_$(1)$(3)_OBJECTS = $$(patsubst %,.libs/$(1)/%$(4).x86_64.o,$$($(2)_SOURCE_STEMS)) $$(patsubst %,.libs/$(1)/%$(4).x86_64.o,$$($(2)_X86_64_SOURCE_STEMS))
armv7_$(1)$(3)_OBJECTS  = $$(patsubst %,.libs/$(1)/%$(4).armv7.o,$$($(2)_SOURCE_STEMS))
armv7s_$(1)$(3)_OBJECTS = $$(patsubst %,.libs/$(1)/%$(4).armv7s.o,$$($(2)_SOURCE_STEMS))
armv7k_$(1)$(3)_OBJECTS = $$(patsubst %,.libs/$(1)/%$(4).armv7k.o,$$($(2)_SOURCE_STEMS))
arm64_$(1)$(3)_OBJECTS  = $$(patsubst %,.libs/$(1)/%$(4).arm64.o,$$($(2)_SOURCE_STEMS))

$$(foreach arch,$$($(2)_ARCHITECTURES),$$(eval $$(call LibXamarinArchTemplate,$(1),$(2),$(3),$(4),$$(arch))))

.libs/$(1)/libxamarin$(4).a: $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/libxamarin$(4).$$(arch).a)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@

.libs/$(1)/libxamarin$(4).x86.dylib: EXTRA_FLAGS=$$($(1)$(3)_COMMON_DYLIB_FLAGS)
.libs/$(1)/libxamarin$(4).x86.dylib: $$(x86_$(1)$(3)_OBJECTS)

.libs/$(1)/libxamarin$(4).x86_64.dylib: EXTRA_FLAGS=$$($(1)$(3)_COMMON_DYLIB_FLAGS)
.libs/$(1)/libxamarin$(4).x86_64.dylib: $$(x86_64_$(1)$(3)_OBJECTS)

.libs/$(1)/libxamarin$(4).armv7.dylib: EXTRA_FLAGS=$$($(1)$(3)_COMMON_DYLIB_FLAGS) -miphoneos-version-min=8.0
.libs/$(1)/libxamarin$(4).armv7.dylib: $$(armv7_$(1)$(3)_OBJECTS)

.libs/$(1)/libxamarin$(4).armv7s.dylib: EXTRA_FLAGS=$$($(1)$(3)_COMMON_DYLIB_FLAGS) -miphoneos-version-min=8.0
.libs/$(1)/libxamarin$(4).armv7s.dylib: $$(armv7s_$(1)$(3)_OBJECTS)

.libs/$(1)/libxamarin$(4).arm64.dylib: EXTRA_FLAGS=$$($(1)$(3)_COMMON_DYLIB_FLAGS)
.libs/$(1)/libxamarin$(4).arm64.dylib: $$(arm64_$(1)$(3)_OBJECTS)

.libs/$(1)/libxamarin$(4).armv7k.dylib: EXTRA_FLAGS=$$($(1)$(3)_COMMON_DYLIB_FLAGS)
.libs/$(1)/libxamarin$(4).armv7k.dylib: $$(armv7k_$(1)$(3)_OBJECTS)

.libs/$(1)/Xamarin$(4).x86.framework: EXTRA_FLAGS=$$($(1)$(3)_COMMON_FRAMEWORK_FLAGS)
.libs/$(1)/Xamarin$(4).x86.framework: $$(x86_$(1)$(3)_OBJECTS)

.libs/$(1)/Xamarin$(4).x86_64.framework: EXTRA_FLAGS=$$($(1)$(3)_COMMON_FRAMEWORK_FLAGS)
.libs/$(1)/Xamarin$(4).x86_64.framework: $$(x86_64_$(1)$(3)_OBJECTS)

.libs/$(1)/Xamarin$(4).armv7.framework: EXTRA_FLAGS=$$($(1)$(3)_COMMON_FRAMEWORK_FLAGS) -miphoneos-version-min=8.0
.libs/$(1)/Xamarin$(4).armv7.framework: $$(armv7_$(1)$(3)_OBJECTS)

.libs/$(1)/Xamarin$(4).armv7s.framework: EXTRA_FLAGS=$$($(1)$(3)_COMMON_FRAMEWORK_FLAGS) -miphoneos-version-min=8.0
.libs/$(1)/Xamarin$(4).armv7s.framework: $$(armv7s_$(1)$(3)_OBJECTS)

.libs/$(1)/Xamarin$(4).arm64.framework: EXTRA_FLAGS=$$($(1)$(3)_COMMON_FRAMEWORK_FLAGS)
.libs/$(1)/Xamarin$(4).arm64.framework: $$(arm64_$(1)$(3)_OBJECTS)

.libs/$(1)/Xamarin$(4).armv7k.framework: EXTRA_FLAGS=$$($(1)$(3)_COMMON_FRAMEWORK_FLAGS)
.libs/$(1)/Xamarin$(4).armv7k.framework: $$(armv7k_$(1)$(3)_OBJECTS)
endef

$(eval $(call LibXamarinTemplate,ios,IOS))
$(eval $(call LibXamarinTemplate,ios,IOS,_DEBUG,-debug))
$(eval $(call LibXamarinTemplate,watchos,WATCHOS))
$(eval $(call LibXamarinTemplate,watchos,WATCHOS,_DEBUG,-debug))
$(eval $(call LibXamarinTemplate,tvos,TVOS))
$(eval $(call LibXamarinTemplate,tvos,TVOS,_DEBUG,-debug))

#
# Xamarin.Mac
#

MAC_ARCHITECTURES = i386 x86_64

CLANG_ARCH = $(addprefix -arch ,$(MAC_ARCHITECTURES))
MAC_CLANG = DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT) $(MAC_CC)

MAC_SHIPPED_HEADERS = xamarin/launch.h

MAC_OBJC_CFLAGS=-ObjC++ -std=c++0x -fno-exceptions
MAC_CFLAGS = -mmacosx-version-min=$(MIN_OSX_SDK_VERSION) -Wall -DMONOMAC -g
MAC_LDFLAGS = -mmacosx-version-min=$(MIN_OSX_SDK_VERSION) -framework AppKit

MAC_STATIC_CFLAGS = $(MAC_CFLAGS) -I$(TOP)/builds/$(BUILD_DESTDIR)/install/mac32/include/mono-2.0

MAC_SOURCES = $(SHARED_SOURCES) $(SHARED_I386_SOURCES) $(SHARED_X86_64_SOURCES) launcher.m

ALLOWED_UNDEFINED_SYMBOLS = _xamarin_enable_debug _xammac_setup

MAC_LIBS =                   \
	XamMacLauncher           \
	libextension.a			 \
	libxammac.a              \
	libxammac-debug.a        \
	libxammac-classic.a      \
	libxammac-classic-debug.a\
	libxammac.dylib          \
	libxammac-debug.dylib    \
	libxammac-system.a       \
	libxammac-system-debug.a \
	libxammac-system-classic.a       \
	libxammac-system-classic-debug.a \

#
# XamMacLauncher: -DDYNAMIC_MONO_RUNTIME -DSYSTEM_LAUNCHER 
#     This is installed as lib/XamMacLauncher, and is used by the XS plugin as the launcher
#     when mmp is not used.
#
# libxammac[-debug].a: (no defines)
#    This is linked into the native executable when embedding the Mono runtime.
#
# libxammac[-debug].dylib: -DDYNAMIC_MONO_RUNTIME -DDYLIB
#    This is not officially supported. It's used when not wanting to use mmp for
#    whatever reason (usually to make build processes easier). There is no good
#    reason to keep using it, since mmp now supports creating empty 'shell' apps
#    with no assemblies, which the user can fill in as they want.
#
# libxammac-system[-debug].a: -DDYNAMIC_MONO_RUNTIME
#    This is used linked into the native executable when using the system Mono (i.e. not)
#    embedding the Mono runtime).
#

all-local:: $(TARGETS)

define ObjTemplate
DYNAMIC_DYLIB$(2)_OBJECTS   = $$(foreach src,$$(MAC_SOURCES),.libs/mac/$$(basename $$(src))$(3).dylib.$(1).o)
STATIC_LAUNCHER$(2)_OBJECTS = $$(foreach src,$$(MAC_SOURCES),.libs/mac/$$(basename $$(src))$(3).static.$(1).o)
SYSTEM_LAUNCHER$(2)_OBJECTS = $$(foreach src,$$(MAC_SOURCES),.libs/mac/$$(basename $$(src))$(3).system.$(1).o)

.libs/mac/%$(3).dylib.$(1).o: %.m $$(SHARED_HEADERS) | .libs/mac
	$$(call Q_2,OBJC,  [mac]) $(MAC_CLANG) -arch $(1) $(4) -c $$(MAC_OBJC_CFLAGS) $$(MAC_CFLAGS) -DDYNAMIC_MONO_RUNTIME -DDYLIB -o $$@ $$<

.libs/mac/%$(3).dylib.$(1).o: %.s $$(SHARED_HEADERS) | .libs/mac
	$$(call Q_2,ASM,   [mac]) $(MAC_CLANG) -arch $(1) $(4) -c $$(MAC_CFLAGS) -DDYNAMIC_MONO_RUNTIME -DDYLIB -o $$@ $$<

.libs/mac/%$(3).static.$(1).o: %.s $$(SHARED_HEADERS) | .libs/mac
	$$(call Q_2,ASM,   [mac]) $(MAC_CLANG) -arch $(1) $(4) -c $$(MAC_STATIC_CFLAGS) -o $$@ $$<

.libs/mac/%$(3).static.$(1).o: %.m $$(SHARED_HEADERS) | .libs/mac
	$$(call Q_2,OBJC,  [mac]) $(MAC_CLANG) -arch $(1) $(4) $$(MAC_OBJC_CFLAGS) -c $$(MAC_STATIC_CFLAGS) -o $$@ $$<

.libs/mac/%$(3).system.$(1).o: %.m $$(SHARED_HEADERS) | .libs/mac
	$$(call Q_2,OBJC,  [mac]) $(MAC_CLANG) -arch $(1) $(4) $$(MAC_OBJC_CFLAGS) -c $$(MAC_CFLAGS) -DDYNAMIC_MONO_RUNTIME -o $$@ $$<

.libs/mac/%$(3).system.$(1).o: %.s $$(SHARED_HEADERS) | .libs/mac
	$$(call Q_2,ASM,   [mac]) $(MAC_CLANG) -arch $(1) $(4) -c $(MAC_CFLAGS) -DDYNAMIC_MONO_RUNTIME -o $$@ $$<

.libs/mac/libxammac$(3).$(1).dylib: $$(DYNAMIC_DYLIB$(2)_OBJECTS)
	$$(call Q_2,LD,    [mac]) $(MAC_CLANG) -arch $(1) -dynamiclib $$(MAC_LDFLAGS) -Wl,-install_name,libxammac$(3).dylib -o $$@ $$^ $$(addprefix -Xlinker -U -Xlinker ,$$(ALLOWED_UNDEFINED_SYMBOLS))

.libs/mac/libxammac$(3).$(1).a: $$(STATIC_LAUNCHER$(2)_OBJECTS)
	$$(call Q_2,LIB,   [mac]) xcrun libtool -no_warning_for_no_symbols -static -o $$@ $$^

.libs/mac/libxammac-system$(3).$(1).a: $$(SYSTEM_LAUNCHER$(2)_OBJECTS)
	$$(call Q_2,LIB,   [mac]) xcrun libtool -no_warning_for_no_symbols -static -o $$@ $$^

endef

$(eval $(call ObjTemplate,i386,32))
$(eval $(call ObjTemplate,x86_64,64))
$(eval $(call ObjTemplate,i386,DEBUG32,-debug,-DDEBUG))
$(eval $(call ObjTemplate,x86_64,DEBUG64,-debug,-DDEBUG))

$(foreach arch,$(MAC_ARCHITECTURES),.libs/mac/extension-main.$(arch).o): EXTRA_DEFINES=-DEXTENSION

.libs/mac/extension-main.%.o:
	$(Q) rm -f $@
	$(call Q_2,CC,  [mac]) $(MAC_CLANG) -c -DEXTENSION extension-main.m -arch $* -o $@

.libs/mac/libextension.%.a: .libs/mac/extension-main.%.o
	$(Q) rm -f $@
	$(call Q_2,AR,    [mac]) $(DEVICE_BIN_PATH)/ar cru $@ $^

.libs/mac/libextension.a: $(foreach arch,$(MAC_ARCHITECTURES),.libs/mac/libextension.$(arch).a)
	$(Q) rm -f $@
	$(call Q_2,LIPO,  [mac]) $(DEVICE_BIN_PATH)/lipo $^ -create -output $@

.libs/mac/libxammac-debug.dylib: .libs/mac/libxammac-debug.x86_64.dylib .libs/mac/libxammac-debug.i386.dylib
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxammac.dylib: .libs/mac/libxammac.x86_64.dylib .libs/mac/libxammac.i386.dylib
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxammac-debug.a: .libs/mac/libxammac-debug.i386.a .libs/mac/libxammac-debug.x86_64.a
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxammac.a: .libs/mac/libxammac.i386.a .libs/mac/libxammac.x86_64.a
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxammac-system-debug.a: .libs/mac/libxammac-system-debug.i386.a .libs/mac/libxammac-system-debug.x86_64.a
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxammac-system.a: .libs/mac/libxammac-system.i386.a .libs/mac/libxammac-system.x86_64.a
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/XamMacLauncher: $(MACIOS_BINARIES_PATH)/XamMacLauncher | .libs/mac
	$(Q) cp $< $@

.libs/mac/libxammac-%.a: $(MACIOS_BINARIES_PATH)/libxammac-%.a | .libs/mac
	$(Q) cp $< $@

.SECONDARY: .libs/mac/libxammac-classic.a .libs/mac/libxammac-system-classic.a .libs/mac/libxammac-classic-debug.a .libs/mac/libxammac-system-classic-debug.a

RUNTIME_MAC_TARGETS_DIRS +=                                    \
	.libs/mac                                                  \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono        \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/include/xamarin \

RUNTIME_MAC_TARGETS +=                                                                                \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/XamMacLauncher                                     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMacLauncher                                \
	$(foreach file,$(SHIPPED_HEADERS),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/include/$(file))     \
	$(foreach file,$(MAC_SHIPPED_HEADERS),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/include/$(file)) \
	$(foreach file,$(MAC_LIBS),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/$(file))                \

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/XamMacLauncher: .libs/mac/XamMacLauncher | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib
	$(Q) install -m 0755 $< $@
	$(Q) xcrun strip -S $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/%: .libs/mac/% | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib
	$(Q) cp $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMacLauncher: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono
	$(Q) ln -sF ../XamMacLauncher $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/include/%.h: %.h | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/include/xamarin
	$(Q) install -m 0644 $< $@

$(RUNTIME_MAC_TARGETS_DIRS):
	$(Q) mkdir -p $@

all-local:: $(RUNTIME_MAC_TARGETS)
install-local:: $(RUNTIME_MAC_TARGETS)

#
# Common
#

clean-local::
	$(Q) rm -f *.o
	$(Q) rm -f *.a
	$(Q) rm -Rf .libs
	$(Q) rm -f xamarin/mono-runtime.h mono-runtime.m delegates.h delegates.inc Delegates.generated.cs


include $(TOP)/mk/rules.mk
