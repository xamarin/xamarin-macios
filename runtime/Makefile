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
	xamarin/monovm-bridge.h \
	xamarin/coreclr-bridge.h \

SHARED_SOURCES += mono-runtime.m bindings.m bindings-generated.m shared.m runtime.m trampolines.m trampolines-invoke.m xamarin-support.m nsstring-localization.m trampolines-varargs.m monovm-bridge.m coreclr-bridge.m nativeaot-bridge.m
SHARED_I386_SOURCES += trampolines-i386.m trampolines-i386-asm.s trampolines-i386-objc_msgSend.s trampolines-i386-objc_msgSendSuper.s trampolines-i386-objc_msgSend_stret.s trampolines-i386-objc_msgSendSuper_stret.s
SHARED_X86_64_SOURCES += trampolines-x86_64.m trampolines-x86_64-asm.s trampolines-x86_64-objc_msgSend.s trampolines-x86_64-objc_msgSendSuper.s trampolines-x86_64-objc_msgSend_stret.s trampolines-x86_64-objc_msgSendSuper_stret.s
SHARED_ARM64_SOURCES += trampolines-arm64.m trampolines-arm64-asm.s trampolines-arm64-objc_msgSend.s trampolines-arm64-objc_msgSendSuper.s
SHARED_HEADERS += shared.h product.h delegates.h runtime-internal.h $(SHARED_INC) $(SHIPPED_HEADERS) trampolines-internal.h slinked-list.h

SHARED_FILES = $(SHARED_SOURCES) $(SHARED_HEADERS) $(SHARED_I386_SOURCES) $(SHARED_X86_64_SOURCES) $(SHARED_ARM64_SOURCES)

EXTRA_DEPENDENCIES = $(SHARED_HEADERS) $(TOP)/Make.config $(TOP)/mk/mono.mk

ifdef IOS_SUPPORTS_32BIT_ARCHITECTURES
IOS_SIMULATOR_ARCHITECTURES=x86
IOS_DEVICE_ARCHITECTURES=armv7 armv7s
endif
IOS_SIMULATOR_ARCHITECTURES+=x86_64
IOS_DEVICE_ARCHITECTURES+=arm64

coreclrhost.h: Makefile
	$(Q_CURL) curl -L --fail --output "$@.tmp" https://raw.githubusercontent.com/dotnet/runtime/6c8f9fff6adcf6c661072646dbdafabed5267ec5/src/coreclr/hosts/inc/coreclrhost.h
	$(Q) mv "$@.tmp" "$@"

coreclr-bridge.m: coreclrhost.h

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
	$(Q) $(SYSTEM_CSC) $< -out:$@ -debug:full -features:strict 

bindings-generated.m: bindings-generator.exe
	$(Q_GEN) $(SYSTEM_MONO) --debug $< $@

# our makefiles don't support building the same source file multiple times with different defines,
# so just symlink extension-main.m to another file and compile that instead.
app-main.m watchextension-main.m tvextension-main.m: extension-main.m
	$(Q_LN) ln -fs $< $@

product.h: product.in.h Makefile $(TOP)/.git/index
	$(Q) sed -e 's/@PRODUCT_HASH@/$(CURRENT_HASH_LONG)/' $< > $@.tmp
	$(Q) mv $@.tmp $@
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
	libtvextension.a           \
	libapp.a                   \
	libxamarin.a               \
	libxamarin-debug.a         \
	libxamarin.dylib           \
	libxamarin-debug.dylib     \

MONOTOUCH_FRAMEWORKS =  \
	Xamarin             \
	Xamarin-debug       \

MONOTOUCH_SOURCE_STEMS = $(patsubst %.s,%,$(patsubst %.m,%,$(SHARED_SOURCES) $(MONOTOUCH_SOURCES)))
MONOTOUCH_I386_SOURCE_STEMS = $(patsubst %.s,%,$(patsubst %.m,%,$(SHARED_I386_SOURCES)))
MONOTOUCH_X86_64_SOURCE_STEMS = $(patsubst %.s,%,$(patsubst %.m,%,$(SHARED_X86_64_SOURCES)))
MONOTOUCH_ARM64_SOURCE_STEMS = $(patsubst %.s,%,$(patsubst %.m,%,$(SHARED_ARM64_SOURCES)))

MONOTOUCH_X86_SOURCE_STEMS = $(MONOTOUCH_I386_SOURCE_STEMS)

#
# FrameworkTemplate contains the install targets and sets up some variables for frameworks
#

define FrameworkTemplate

$(2)_ARCHITECTURES = $(3)
$(2)_FRAMEWORKS    = $(MONOTOUCH_FRAMEWORKS)

RUNTIME_$(2)_TARGETS_DIRS +=                                        \
	$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin.framework \
	$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin-debug.framework \

RUNTIME_$(2)_TARGETS +=                                                                                \
	$$(foreach file,$$($(2)_FRAMEWORKS),$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/$$(file).framework/$$(file))   \
	$$(foreach file,$$($(2)_FRAMEWORKS),$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/$$(file).framework/Info.plist) \
	$$(foreach file,$$($(2)_FRAMEWORKS),$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/$$(file).framework.dSYM/Contents/Info.plist) \

$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin.framework/Xamarin: .libs/$(1)/Xamarin.framework | $(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin.framework
	$(Q) $(CP) $$< $$@

$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin-debug.framework/Xamarin-debug: .libs/$(1)/Xamarin-debug.framework | $(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin-debug.framework
	$(Q) $(CP) $$< $$@

$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/%.framework/Info.plist: Xamarin.framework-$(1).Info.plist | $(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/%.framework
	$(Q) sed 's/@BUNDLE_EXECUTABLE@/$$*/' $$< > $$@

$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin.framework.dSYM/Contents/Info.plist: $(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin.framework/Xamarin $(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin.framework/Info.plist
	$$(Q_GEN) dsymutil -j 4 $$< -o $$(abspath $$(dir $$<)/..)/Xamarin.framework.dSYM

$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin-debug.framework.dSYM/Contents/Info.plist: $(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin-debug.framework/Xamarin-debug $(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/Frameworks/Xamarin-debug.framework/Info.plist
	$$(Q_GEN) dsymutil -j 4 $$< -o $$(abspath $$(dir $$<)/..)/Xamarin-debug.framework.dSYM

.libs/$(1)/Xamarin.framework:  $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/Xamarin.$$(arch).framework)
	$(Q) rm -f $$@
ifeq (1,$$(words $$($(2)_ARCHITECTURES)))
	$(Q) $(CP) $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif

.libs/$(1)/Xamarin-debug.framework:  $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/Xamarin-debug.$$(arch).framework)
	$(Q) rm -f $$@
ifeq (1,$$(words $$($(2)_ARCHITECTURES)))
	$(Q) $(CP) $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif
endef

# 1: platform
# 2: variable prefix
# 3: architectures
ifdef INCLUDE_IOS
ifdef INCLUDE_DEVICE
$(eval $(call FrameworkTemplate,iphoneos,IPHONEOS,$(IOS_DEVICE_ARCHITECTURES)))
endif
$(eval $(call FrameworkTemplate,iphonesimulator,IOSSIMULATOR,$(IOS_SIMULATOR_ARCHITECTURES)))
endif

ifdef INCLUDE_WATCH
ifdef INCLUDE_DEVICE
ifdef WATCHOS_SUPPORTS_32BIT_ARCHITECTURES
$(eval $(call FrameworkTemplate,watchos,WATCHOS,armv7k arm64_32))
else
$(eval $(call FrameworkTemplate,watchos,WATCHOS,arm64_32))
endif
endif
$(eval $(call FrameworkTemplate,watchsimulator,WATCHSIMULATOR,x86))
endif

ifdef INCLUDE_TVOS
ifdef INCLUDE_DEVICE
$(eval $(call FrameworkTemplate,tvos,TVOS,arm64))
endif
$(eval $(call FrameworkTemplate,tvsimulator,TVSIMULATOR,x86_64))
endif

#
# PlatformTemplate contains the install targets and sets up some variables
#

define PlatformTemplate

$(2)_SOURCES       = $(MONOTOUCH_SOURCES)
$(2)_HEADERS       = $(MONOTOUCH_HEADERS)
$(2)_SOURCE_STEMS  = $(MONOTOUCH_SOURCE_STEMS)
$(2)_X86_64_SOURCE_STEMS = $(MONOTOUCH_X86_64_SOURCE_STEMS)
$(2)_I386_SOURCE_STEMS = $(MONOTOUCH_I386_SOURCE_STEMS)
$(2)_ARM64_SOURCE_STEMS = $(MONOTOUCH_ARM64_SOURCE_STEMS)
$(2)_LIBRARIES     = $(MONOTOUCH_LIBS)
$(2)_ARCHITECTURES = $(3)

RUNTIME_$(2)_TARGETS_DIRS +=                                        \
	$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/lib                 \
	$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/include/xamarin     \

RUNTIME_$(2)_TARGETS +=                                                                                \
	$$(foreach file,$$($(2)_LIBRARIES),$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/lib/$$(file))       \
	$(foreach file,$(SHIPPED_HEADERS),$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/include/$(file))     \

$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/lib/%.a: .libs/$(1)/%.a | $(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/lib
	$(Q) install -m 0644 $$< $$@

$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/lib/%.dylib: .libs/$(1)/%.dylib | $(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/lib
	$$(Q_STRIP) $(DEVICE_BIN_PATH)/bitcode_strip $$< -m -o $$@

$(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/include/%.h: %.h | $(IOS_DESTDIR)$$(XAMARIN_$(2)_SDK)/include/xamarin
	$(Q) install -m 0644 $$< $$@

$$(RUNTIME_$(2)_TARGETS_DIRS):
	$(Q) mkdir -p $$@

all-$(2):: $$(RUNTIME_$(2)_TARGETS)

ifdef INCLUDE_XAMARIN_LEGACY
all-local:: $$(RUNTIME_$(2)_TARGETS)
install-local:: $$(RUNTIME_$(2)_TARGETS)
endif

endef

# 1: platform
# 2: variable prefix
# 3: architectures
ifdef INCLUDE_IOS
ifdef INCLUDE_DEVICE
$(eval $(call PlatformTemplate,iphoneos,IPHONEOS,$(IOS_DEVICE_ARCHITECTURES)))
endif
$(eval $(call PlatformTemplate,iphonesimulator,IOSSIMULATOR,$(IOS_SIMULATOR_ARCHITECTURES)))
endif
ifdef INCLUDE_MACCATALYST
$(eval $(call PlatformTemplate,maccatalyst,MACCATALYST,x86_64))
endif

ifdef INCLUDE_WATCH
ifdef INCLUDE_DEVICE
ifdef WATCHOS_SUPPORTS_32BIT_ARCHITECTURES
$(eval $(call PlatformTemplate,watchos,WATCHOS,armv7k arm64_32))
else
$(eval $(call PlatformTemplate,watchos,WATCHOS,arm64_32))
endif
endif
$(eval $(call PlatformTemplate,watchsimulator,WATCHSIMULATOR,x86 x86_64))
endif

ifdef INCLUDE_TVOS
ifdef INCLUDE_DEVICE
$(eval $(call PlatformTemplate,tvos,TVOS,arm64))
endif
$(eval $(call PlatformTemplate,tvsimulator,TVSIMULATOR,x86_64))
endif

#
# LibTemplate we build two different libraries from the same source code,
# libapp.a and libextension.a
#
# They're all built from the same soure file (extension-main.m), but with different defines:
# libextension.a has EXTENSION defined.
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

.libs/$(1)/libtvextension.%.a: .libs/$(1)/tvextension-main.%.o
	$(Q) rm -f $$@
	$$(call Q_2,AR,    [$1]) $(DEVICE_BIN_PATH)/ar cru $$@ $$^

.libs/$(1)/libtvextension.a: $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/libtvextension.$$(arch).a)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@

.libs/$(1)/libxamarin.dylib:  $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/libxamarin.$$(arch).dylib)
	$(Q) rm -f $$@
ifeq (1,$$(words $$($(2)_ARCHITECTURES)))
	$(Q) $(CP) $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif
	$(Q) install_name_tool -id @rpath/libxamarin.dylib $$@
	$(Q) install_name_tool -change @rpath/ @rpath/libmonosgen-2.0.dylib $$@

.libs/$(1)/libxamarin-debug.dylib:  $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/libxamarin-debug.$$(arch).dylib)
	$(Q) rm -f $$@
ifeq (1,$$(words $$($(2)_ARCHITECTURES)))
	$(Q) $(CP) $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif
	$(Q) install_name_tool -id @rpath/libxamarin-debug.dylib $$@
	$(Q) install_name_tool -change @rpath/ @rpath/libmonosgen-2.0.dylib $$@

.SECONDARY: $$(foreach arch,$$($(2)_ARCHITECTURES),.libs/$(1)/app-main.$$(arch).o)
endef

$(eval $(call LibTemplate,iphoneos,IPHONEOS))
$(eval $(call LibTemplate,iphonesimulator,IOSSIMULATOR))
$(eval $(call LibTemplate,maccatalyst,MACCATALYST))
$(eval $(call LibTemplate,watchos,WATCHOS))
$(eval $(call LibTemplate,watchsimulator,WATCHSIMULATOR))
$(eval $(call LibTemplate,tvos,TVOS))
$(eval $(call LibTemplate,tvsimulator,TVSIMULATOR))

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
arm64_$(1)$(3)_OBJECTS  = $$(patsubst %,.libs/$(1)/%$(4).arm64.o,$$($(2)_SOURCE_STEMS))  $$(patsubst %,.libs/$(1)/%$(4).arm64.o,$$($(2)_ARM64_SOURCE_STEMS))
arm64_32_$(1)$(3)_OBJECTS = $$(patsubst %,.libs/$(1)/%$(4).arm64_32.o,$$($(2)_SOURCE_STEMS))

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

.libs/$(1)/libxamarin$(4).arm64_32.dylib: EXTRA_FLAGS=$$($(1)$(3)_COMMON_DYLIB_FLAGS)
.libs/$(1)/libxamarin$(4).arm64_32.dylib: $$(arm64_32_$(1)$(3)_OBJECTS)

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

.libs/$(1)/Xamarin$(4).arm64_32.framework: EXTRA_FLAGS=$$($(1)$(3)_COMMON_FRAMEWORK_FLAGS)
.libs/$(1)/Xamarin$(4).arm64_32.framework: $$(arm64_32_$(1)$(3)_OBJECTS)
endef

$(eval $(call LibXamarinTemplate,iphoneos,IPHONEOS))
$(eval $(call LibXamarinTemplate,iphoneos,IPHONEOS,_DEBUG,-debug))
$(eval $(call LibXamarinTemplate,iphonesimulator,IOSSIMULATOR))
$(eval $(call LibXamarinTemplate,iphonesimulator,IOSSIMULATOR,_DEBUG,-debug))
$(eval $(call LibXamarinTemplate,maccatalyst,MACCATALYST))
$(eval $(call LibXamarinTemplate,maccatalyst,MACCATALYST,_DEBUG,-debug))
$(eval $(call LibXamarinTemplate,watchos,WATCHOS))
$(eval $(call LibXamarinTemplate,watchos,WATCHOS,_DEBUG,-debug))
$(eval $(call LibXamarinTemplate,watchsimulator,WATCHSIMULATOR))
$(eval $(call LibXamarinTemplate,watchsimulator,WATCHSIMULATOR,_DEBUG,-debug))
$(eval $(call LibXamarinTemplate,tvos,TVOS))
$(eval $(call LibXamarinTemplate,tvos,TVOS,_DEBUG,-debug))
$(eval $(call LibXamarinTemplate,tvsimulator,TVSIMULATOR))
$(eval $(call LibXamarinTemplate,tvsimulator,TVSIMULATOR,_DEBUG,-debug))

#
# Xamarin.Mac
#

MAC_ARCHITECTURES = x86_64 arm64

CLANG_ARCH = $(addprefix -arch ,$(MAC_ARCHITECTURES))
MAC_CLANG = DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT) $(MAC_CC) -mmacosx-version-min=$(MIN_MACOS_SDK_VERSION)

MAC_SHIPPED_HEADERS = xamarin/launch.h

MAC_STATIC_CFLAGS = $(MAC_CFLAGS)

MAC_SOURCES = $(SHARED_SOURCES) $(SHARED_X86_64_SOURCES) $(SHARED_ARM64_SOURCES) launcher.m

ALLOWED_UNDEFINED_SYMBOLS = _xamarin_enable_debug _xammac_setup

MAC_LIBS =                   \
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
# libxamarin-coreclr.a:
#    This is used when using the CoreCLR runtime instead of Mono.
#    CORECLR_RUNTIME is defined for these versions of libxamarin.
#
# libxamarin-nativeaot.a:
#    This is used when using NativeAOT.
#    CORECLR_RUNTIME is defined for these versions of libxamarin (because the nativeaot bridge shares *a lot* of code with the coreclr bridge)
#    NATIVEAOT is defined for these versions of libxamarin
#

ifdef INCLUDE_XAMARIN_LEGACY
all-local:: $(TARGETS)
endif

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

$(eval $(call ObjTemplate,x86_64,64,,,64))
$(eval $(call ObjTemplate,x86_64,DEBUG64,-debug,-DDEBUG,64))
$(eval $(call ObjTemplate,arm64,64,,,64))
$(eval $(call ObjTemplate,arm64,DEBUG64,-debug,-DDEBUG,64))

$(foreach arch,$(MAC_ARCHITECTURES),.libs/mac/extension-main.$(arch).o): EXTRA_DEFINES=-DEXTENSION

.libs/mac/extension-main.%.o: | .libs/mac
	$(Q) rm -f $@
	$(call Q_2,CC,  [mac]) $(MAC_CLANG) -c -DEXTENSION extension-main.m -arch $* -o $@

.libs/mac/libextension.%.a: .libs/mac/extension-main.%.o
	$(Q) rm -f $@
	$(call Q_2,AR,    [mac]) $(DEVICE_BIN_PATH)/ar cru $@ $^

.libs/mac/libextension.a: $(foreach arch,$(MAC_ARCHITECTURES),.libs/mac/libextension.$(arch).a)
	$(Q) rm -f $@
	$(call Q_2,LIPO,  [mac]) $(DEVICE_BIN_PATH)/lipo $^ -create -output $@

.libs/mac/libxammac-debug.dylib: .libs/mac/libxammac-debug.x86_64.dylib .libs/mac/libxammac-debug.arm64.dylib
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxammac.dylib: .libs/mac/libxammac.x86_64.dylib .libs/mac/libxammac.arm64.dylib
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxammac-debug.a: .libs/mac/libxammac-debug.x86_64.a .libs/mac/libxammac-debug.arm64.a
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxammac.a: .libs/mac/libxammac.x86_64.a .libs/mac/libxammac.arm64.a
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxammac-system-debug.a: .libs/mac/libxammac-system-debug.x86_64.a .libs/mac/libxammac-system-debug.arm64.a
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxammac-system.a: .libs/mac/libxammac-system.x86_64.a .libs/mac/libxammac-system.arm64.a
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxammac-%.a: $(MACIOS_BINARIES_PATH)/libxammac-%.a | .libs/mac
	$(Q) $(CP) $< $@

.libs/mac/libxamarin%.a: .libs/mac/libxamarin%.x86_64.a
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

.libs/mac/libxamarin%.dylib: .libs/mac/libxamarin%.x86_64.dylib
	$(call Q_2,LIPO,  [mac]) xcrun lipo -create $^ -o $@

RUNTIME_MAC_TARGETS_DIRS +=                                    \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono        \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib                     \
	$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/include/xamarin         \

RUNTIME_MAC_TARGETS +=                                                                                \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/XamMacLauncher                                     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMacLauncher                                \
	$(foreach file,$(SHIPPED_HEADERS),$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/include/$(file))             \
	$(foreach file,$(MAC_SHIPPED_HEADERS),$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/include/$(file))         \
	$(foreach file,$(MAC_LIBS),$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/$(file))                        \

# The XamMacLauncher file must exist for VSfM to be able to open XM/Classic projects (so that people can use the migration wizard)
$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/XamMacLauncher: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib
	$(Q) touch $@

$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib/%: .libs/mac/% | $(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/lib
	$(Q) $(CP) $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMacLauncher: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono
	$(Q) ln -sF ../XamMacLauncher $@

$(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/include/%.h: %.h | $(MAC_DESTDIR)$(XAMARIN_MACOS_SDK)/include/xamarin
	$(Q) install -m 0644 $< $@

$(RUNTIME_MAC_TARGETS_DIRS):
	$(Q) mkdir -p $@

ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_MAC
all-local:: $(RUNTIME_MAC_TARGETS)
install-local:: $(RUNTIME_MAC_TARGETS)
endif
endif

#
# .NET
#

DOTNET_iOS_LIBRARIES = libextension-dotnet.a
DOTNET_tvOS_LIBRARIES = libtvextension-dotnet.a
DOTNET_macOS_LIBRARIES = libextension-dotnet-coreclr.a
DOTNET_MacCatalyst_LIBRARIES =

define DotNetLibTemplate
DOTNET_TARGETS += \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet.dylib \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet-debug.dylib \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet.a \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet-debug.a \
	$$(foreach lib,$$(DOTNET_$(1)_LIBRARIES),$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/$$(lib)) \
	$$(foreach header,$$(SHIPPED_HEADERS),$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/$$(header)) \

DOTNET_TARGET_DIRS += \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/xamarin \

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/xamarin/%.h: xamarin/%.h | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/xamarin
	$$(Q) $$(CP) $$< $$@
endef

$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetLibTemplate,$(platform),$(rid)))))

define DotNetCoreClrLibTemplate
DOTNET_TARGETS += \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet-coreclr.a \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet-coreclr-debug.a \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet-coreclr.dylib \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet-coreclr-debug.dylib \

endef

$(foreach platform,$(DOTNET_CORECLR_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetCoreClrLibTemplate,$(platform),$(rid)))))

define DotNetNativeAotLibTemplate
DOTNET_TARGETS += \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet-nativeaot.a \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet-nativeaot-debug.a \

endef

$(foreach platform,$(DOTNET_NATIVEAOT_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetNativeAotLibTemplate,$(platform),$(rid)))))

# a few lookup tables, because the data we have is not always in the format we need it
DOTNET_iphonesimulator_DYLIB_FLAGS=-lmonosgen-2.0 -licudata -licui18n -licuuc -framework UIKit
DOTNET_iphoneos_DYLIB_FLAGS=-lmonosgen-2.0 -licudata -licui18n -licuuc -framework UIKit
DOTNET_tvsimulator_DYLIB_FLAGS=-lmonosgen-2.0 -licudata -licui18n -licuuc -framework UIKit
DOTNET_tvos_DYLIB_FLAGS=-lmonosgen-2.0 -framework UIKit
DOTNET_maccatalyst_DYLIB_FLAGS=-lmonosgen-2.0 -framework UIKit
DOTNET_mac_DYLIB_FLAGS=-lcoreclr

#
# DotNetInstallLibTemplate lipos or copies libraries into the destination directories
#

define DotNetInstallLibTemplate
$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-%.dylib: $$(foreach arch,$(3),.libs/$(4)/libxamarin-%.$$(arch).dylib) | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
ifeq (1,$$(words $(3)))
	$(Q) $(CP) $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif
	$(Q) install_name_tool -id @rpath/libxamarin-$$*.dylib $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-%.a: $$(foreach arch,$(3),.libs/$(4)/libxamarin-%.$$(arch).a) | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
ifeq (1,$$(words $(3)))
	$(Q) $(CP) $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libapp-%.a: $$(foreach arch,$(3),.libs/$(4)/libapp-%.$$(arch).a) | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
ifeq (1,$$(words $(3)))
	$(Q) $(CP) $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libextension-%.a: $$(foreach arch,$(3),.libs/$(4)/libextension-%.$$(arch).a) | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
ifeq (1,$$(words $(3)))
	$(Q) $(CP) $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libtvextension-%.a: $$(foreach arch,$(3),.libs/$(4)/libtvextension-%.$$(arch).a) | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
ifeq (1,$$(words $(3)))
	$(Q) $(CP) $$^ $$@
else
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endif

endef

# foreach (var platform in DOTNET_PLATFORMS)
#   foreach (var rid in DOTNET_<platform>_RUNTIME_IDENTIFIERS)
#     call DotNetInstallLibTemplate (platform, rid, architectures_for_rid, sdk_platform_for_rid)
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetInstallLibTemplate,$(platform),$(rid),$(DOTNET_$(rid)_ARCHITECTURES),$(DOTNET_$(rid)_SDK_PLATFORM)))))

#
# LibXamarinTemplate builds libxamarin.a
#

define DotNetLibXamarinTemplate

DOTNET$(6)_$(2)_LIBDIR ?= $$(TOP)/packages/microsoft.netcore.app.runtime$(7).$(2)/$(BUNDLED_NETCORE_PLATFORMS_PACKAGE_VERSION)/runtimes/$(2)/native
DOTNET$(6)_$(2)_DYLIB_FLAGS = $(DOTNET_$(1)_DYLIB_FLAGS) -Wl,-install_name,libxamarin$(5).dylib -framework Foundation -framework CFNetwork -lz -L$(abspath $(DOTNET$(6)_$(2)_LIBDIR))

DOTNET_$(2)_$(3)$(4)_OBJECTS   = $$(patsubst %,.libs/$(1)/%$(5).$(3).o,   $(MONOTOUCH_SOURCE_STEMS)) $$(patsubst %,.libs/$(1)/%$(5).$(3).o,   $(MONOTOUCH_$(shell echo $(3) | tr a-z A-Z)_SOURCE_STEMS))

.libs/$(1)/libxamarin$(5).$(3).a: $$(DOTNET_$(2)_$(3)$(4)_OBJECTS)
	$$(Q) rm -f $$@
	$$(call Q_2,AR,    [$1]) $(DEVICE_BIN_PATH)/ar Scru $$@ $$^
	$$(call Q_2,RANLIB,[$1]) $(DEVICE_BIN_PATH)/ranlib -no_warning_for_no_symbols -q $$@

.libs/$(1)/libxamarin$(5).$(3).dylib: EXTRA_FLAGS=$$(DOTNET$(6)_$(2)_DYLIB_FLAGS)
.libs/$(1)/libxamarin$(5).$(3).dylib: $$(DOTNET_$(2)_$(3)$(4)_OBJECTS)

endef

# foreach (var platform in DOTNET_PLATFORMS)
#   foreach (var rid in DOTNET_<platform>_RUNTIME_IDENTIFIERS))
#      foreach (var arch in DOTNET_<rid>_ARCHITECTURES)
#                                           1      2    3      4                  5                     6       7
#        call DotNetLibXamarinTemplate (platform, rid, arch,       ,         "-dotnet",                       ,.mono)
#        call DotNetLibXamarinTemplate (platform, rid, arch, _DEBUG,         "-dotnet-debug",                 ,.mono)
#        call DotNetLibXamarinTemplate (platform, rid, arch, _CORECLR,       "-dotnet-coreclr",       _CORECLR,)
#        call DotNetLibXamarinTemplate (platform, rid, arch, _CORECLR_DEBUG, "-dotnet-coreclr-debug", _CORECLR,)

$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(foreach arch,$(DOTNET_$(rid)_ARCHITECTURES),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),$(arch),,-dotnet,,.mono)))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(foreach arch,$(DOTNET_$(rid)_ARCHITECTURES),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),$(arch),_DEBUG,-dotnet-debug,,.mono)))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(foreach arch,$(DOTNET_$(rid)_ARCHITECTURES),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),$(arch),_CORECLR,-dotnet-coreclr,_CORECLR)))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(foreach arch,$(DOTNET_$(rid)_ARCHITECTURES),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),$(arch),_CORECLR_DEBUG,-dotnet-coreclr-debug,_CORECLR)))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(foreach arch,$(DOTNET_$(rid)_ARCHITECTURES),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),$(arch),_NATIVEAOT,-dotnet-nativeaot,_NATIVEAOT,.mono)))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(foreach arch,$(DOTNET_$(rid)_ARCHITECTURES),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),$(arch),_NATIVEAOT_DEBUG,-dotnet-nativeaot-debug,_NATIVEAOT,.mono)))))

#
# DotNetLibExtensionTemplate builds lib[tv]extension.a
#

define DotNetLibExtensionTemplate
.libs/$(1)/extension-main$(5).$(3).o: EXTRA_DEFINES=-DEXTENSION
.libs/$(1)/tvextension-main$(5).$(3).o: EXTRA_DEFINES=-DTV_EXTENSION

.libs/$(1)/libextension$(5).$(3).a: .libs/$(1)/extension-main$(5).$(3).o
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@

.libs/$(1)/libtvextension$(5).$(3).a: .libs/$(1)/tvextension-main$(5).$(3).o
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(DEVICE_BIN_PATH)/lipo $$^ -create -output $$@
endef

# foreach (var platform in DOTNET_PLATFORMS)
#   foreach (var rid in DOTNET_<platform>_RUNTIME_IDENTIFIERS))
#      foreach (var arch in DOTNET_<rid>_ARCHITECTURES)
#                                             1      2    3      4                  5                     6
#        call DotNetLibExtensionTemplate (platform, rid, arch,       ,         "-dotnet",                       )
#        call DotNetLibExtensionTemplate (platform, rid, arch, _CORECLR,       "-dotnet-coreclr",       _CORECLR)

$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(foreach arch,$(DOTNET_$(rid)_ARCHITECTURES),$(eval $(call DotNetLibExtensionTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),$(arch),,-dotnet,,)))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(foreach arch,$(DOTNET_$(rid)_ARCHITECTURES),$(eval $(call DotNetLibExtensionTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),$(arch),_CORECLR,-dotnet-coreclr,_CORECLR)))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(foreach arch,$(DOTNET_$(rid)_ARCHITECTURES),$(eval $(call DotNetLibExtensionTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),$(arch),_NATOVEAOT,-dotnet-nativeaot,_NATOVEAOT)))))

dotnet: $(DOTNET_TARGETS)

$(DOTNET_TARGET_DIRS):
	$(Q) mkdir -p $@

ifdef ENABLE_DOTNET
all-local:: $(DOTNET_TARGETS)
install-local:: $(DOTNET_TARGETS)
endif

#
# Common
#

clean-local::
	$(Q) rm -f *.o
	$(Q) rm -f *.a
	$(Q) rm -Rf .libs
	$(Q) rm -f xamarin/mono-runtime.h mono-runtime.m delegates.h delegates.inc Delegates.generated.cs


include $(TOP)/mk/rules.mk

.SECONDARY: delegates.h delegates.inc
