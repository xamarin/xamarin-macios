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

SHARED_SOURCES += mono-runtime.m bindings.m bindings-generated.m shared.m runtime.m trampolines.m trampolines-invoke.m xamarin-support.m nsstring-localization.m trampolines-varargs.m monovm-bridge.m coreclr-bridge.m nativeaot-bridge.m swift-functions.swift
SHARED_X64_SOURCES += trampolines-x86_64.m trampolines-x86_64-asm.s trampolines-x86_64-objc_msgSend.s trampolines-x86_64-objc_msgSendSuper.s trampolines-x86_64-objc_msgSend_stret.s trampolines-x86_64-objc_msgSendSuper_stret.s
SHARED_ARM64_SOURCES += trampolines-arm64.m trampolines-arm64-asm.s trampolines-arm64-objc_msgSend.s trampolines-arm64-objc_msgSendSuper.s
SHARED_HEADERS += shared.h product.h delegates.h runtime-internal.h $(SHARED_INC) $(SHIPPED_HEADERS) trampolines-internal.h slinked-list.h

SHARED_FILES = $(SHARED_SOURCES) $(SHARED_HEADERS) $(SHARED_X64_SOURCES) $(SHARED_ARM64_SOURCES)

EXTRA_DEPENDENCIES = $(SHARED_HEADERS) $(TOP)/Make.config $(TOP)/mk/mono.mk

IOS_SIMULATOR_ARCHITECTURES+=x86_64
IOS_DEVICE_ARCHITECTURES+=arm64

coreclrhost.h: Makefile
	$(Q_CURL) curl -L --fail --output "$@.tmp" https://raw.githubusercontent.com/dotnet/runtime/f1b8d5a4448d072c9d390744777a87921e45bf2f/src/coreclr/hosts/inc/coreclrhost.h
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

BINDINGS_GENERATOR=bin/Debug/bindings-generator.dll
$(BINDINGS_GENERATOR): bindings-generator.cs bindings-generator.csproj
	$(Q) $(DOTNET) build bindings-generator.csproj $(DOTNET_BUILD_VERBOSITY)

bindings-generated.m: $(BINDINGS_GENERATOR)
	$(Q_GEN) $(DOTNET) exec $< $@

# our makefiles don't support building the same source file multiple times with different defines,
# so just symlink extension-main.m to another file and compile that instead.
app-main.m tvextension-main.m: extension-main.m
	$(Q_LN) ln -fs $< $@

product.h: product.in.h Makefile $(GIT_DIRECTORY)/index
	$(Q) sed -e 's/@PRODUCT_HASH@/$(CURRENT_HASH_LONG)/' $< > $@.tmp
	$(Q) mv $@.tmp $@
#
# MonoTouch defines (used for all MonoTouch platforms: iOS and TVOS)
#

MONOTOUCH_SOURCES =     \
	monotouch-debug.m   \
	monotouch-main.m    \

MONOTOUCH_HEADERS =     \
    monotouch-debug.h   \

MONOTOUCH_SOURCE_STEMS = $(patsubst %.swift,%,$(patsubst %.s,%,$(patsubst %.m,%,$(SHARED_SOURCES) $(MONOTOUCH_SOURCES))))
MONOTOUCH_X64_SOURCE_STEMS = $(patsubst %.s,%,$(patsubst %.m,%,$(SHARED_X64_SOURCES)))
MONOTOUCH_ARM64_SOURCE_STEMS = $(patsubst %.s,%,$(patsubst %.m,%,$(SHARED_ARM64_SOURCES)))

#
# .NET
#

DOTNET_iOS_LIBRARIES = libextension-dotnet.a
DOTNET_tvOS_LIBRARIES = libtvextension-dotnet.a
DOTNET_macOS_LIBRARIES = libextension-dotnet-coreclr.a
DOTNET_MacCatalyst_LIBRARIES =

define DotNetLibTemplate
DOTNET_TARGETS += \
	$$(foreach lib,$$(DOTNET_$(1)_LIBRARIES),$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/$$(lib)) \
	$$(foreach header,$$(SHIPPED_HEADERS),$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/$$(header)) \

DOTNET_TARGET_DIRS += \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/xamarin \

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/xamarin/%.h: xamarin/%.h | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/xamarin
	$$(Q) $$(CP) $$< $$@
endef

$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetLibTemplate,$(platform),$(rid)))))

define DotNetMonoLibTemplate
DOTNET_TARGETS += \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet.dylib \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet-debug.dylib \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet.a \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-dotnet-debug.a \

endef

$(foreach platform,$(DOTNET_MONOVM_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetMonoLibTemplate,$(platform),$(rid)))))

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
DOTNET_iphonesimulator_DYLIB_FLAGS=-lmonosgen-2.0 -framework UIKit
DOTNET_iphonesimulator_CORECLR_DYLIB_FLAGS=-lcoreclr
DOTNET_iphoneos_DYLIB_FLAGS=-lmonosgen-2.0 -framework UIKit
DOTNET_iphoneos_CORECLR_DYLIB_FLAGS=-lcoreclr
DOTNET_tvsimulator_DYLIB_FLAGS=-lmonosgen-2.0 -framework UIKit
DOTNET_tvsimulator_CORECLR_DYLIB_FLAGS=-lcoreclr
DOTNET_tvos_DYLIB_FLAGS=-lmonosgen-2.0 -framework UIKit
DOTNET_tvos_CORECLR_DYLIB_FLAGS=-lcoreclr
DOTNET_maccatalyst_DYLIB_FLAGS=-lmonosgen-2.0 -framework UIKit
DOTNET_maccatalyst_CORECLR_DYLIB_FLAGS=-lcoreclr
DOTNET_mac_CORECLR_DYLIB_FLAGS=-lcoreclr

#
# DotNetInstallLibTemplate lipos or copies libraries into the destination directories
#

define DotNetInstallLibTemplate
$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-%.dylib: .libs/$(2)/libxamarin-%.dylib | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
	$$(Q) $(CP) $$^ $$@
	$$(Q) install_name_tool -id @rpath/libxamarin-$$*.dylib $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libxamarin-%.a: .libs/$(2)/libxamarin-%.a | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
	$$(Q) $(CP) $$^ $$@
	$$(call Q_2,LIPO,  [$1]) $(LIPO) $$^ -create -output $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libapp-%.a: .libs/$(2)/libapp-%.a | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
	$$(Q) $(CP) $$^ $$@
	$$(call Q_2,LIPO,  [$1]) $(LIPO) $$^ -create -output $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libextension-%.a: .libs/$(2)/libextension-%.a | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
	$$(Q) $(CP) $$^ $$@
	$$(call Q_2,LIPO,  [$1]) $(LIPO) $$^ -create -output $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/libtvextension-%.a: .libs/$(2)/libtvextension-%.a | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
	$$(Q) $(CP) $$^ $$@
	$$(call Q_2,LIPO,  [$1]) $(LIPO) $$^ -create -output $$@

endef

# foreach (var platform in DOTNET_PLATFORMS)
#   foreach (var rid in DOTNET_<platform>_RUNTIME_IDENTIFIERS)
#     call DotNetInstallLibTemplate (platform, rid)
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetInstallLibTemplate,$(platform),$(rid)))))

#
# LibXamarinTemplate builds libxamarin.a
#

define DotNetLibXamarinTemplate

DOTNET$(6)_$(2)_LIBDIR ?= $$(TOP)/packages/microsoft.netcore.app.runtime$(7).$(2)/$(BUNDLED_NETCORE_PLATFORMS_PACKAGE_VERSION)/runtimes/$(2)/native
DOTNET$(6)_$(2)_DYLIB_FLAGS = $(DOTNET_$(1)$(6)_DYLIB_FLAGS) -Wl,-install_name,libxamarin$(5).dylib -framework Foundation -framework CFNetwork -lz -L$(abspath $(DOTNET$(6)_$(2)_LIBDIR))

DOTNET_$(2)_$(4)_OBJECTS   = $$(patsubst %,.libs/$(2)/%$(5).o,   $(MONOTOUCH_SOURCE_STEMS)) $$(patsubst %,.libs/$(2)/%$(5).o,   $(MONOTOUCH_$(shell echo $(2) | sed 's/.*-//' | tr a-z A-Z)_SOURCE_STEMS))


.libs/$(2)/libxamarin$(5).a: $$(DOTNET_$(2)_$(4)_OBJECTS)
	$$(Q) rm -f $$@
	$$(call Q_2,AR,    [$1]) $(AR) Scru $$@ $$^
	$$(call Q_2,RANLIB,[$1]) $(RANLIB) -no_warning_for_no_symbols -q $$@

.libs/$(2)/libxamarin$(5).dylib: EXTRA_FLAGS=$$(DOTNET$(6)_$(2)_DYLIB_FLAGS)
.libs/$(2)/libxamarin$(5).dylib: $$(DOTNET_$(2)_$(4)_OBJECTS)

endef

# foreach (var platform in DOTNET_PLATFORMS)
#   foreach (var rid in DOTNET_<platform>_RUNTIME_IDENTIFIERS))
#                                           1      2    3      4                  5                     6       7
#        call DotNetLibXamarinTemplate (platform, rid, <unused>,       ,         "-dotnet",                       ,.mono)
#        call DotNetLibXamarinTemplate (platform, rid, <unused>, _DEBUG,         "-dotnet-debug",                 ,.mono)
#        call DotNetLibXamarinTemplate (platform, rid, <unused>, _CORECLR,       "-dotnet-coreclr",       _CORECLR,)
#        call DotNetLibXamarinTemplate (platform, rid, <unused>, _CORECLR_DEBUG, "-dotnet-coreclr-debug", _CORECLR,)

$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),,,-dotnet,,.mono))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),,_DEBUG,-dotnet-debug,,.mono))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),,_CORECLR,-dotnet-coreclr,_CORECLR))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),,_CORECLR_DEBUG,-dotnet-coreclr-debug,_CORECLR))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),,_NATIVEAOT,-dotnet-nativeaot,_NATIVEAOT,.mono))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetLibXamarinTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),,_NATIVEAOT_DEBUG,-dotnet-nativeaot-debug,_NATIVEAOT,.mono))))

#
# DotNetLibExtensionTemplate builds lib[tv]extension.a
#

define DotNetLibExtensionTemplate
.libs/$(2)/extension-main$(5).o: EXTRA_DEFINES=-DEXTENSION
.libs/$(2)/tvextension-main$(5).o: EXTRA_DEFINES=-DTV_EXTENSION

.libs/$(2)/libextension$(5).a: .libs/$(2)/extension-main$(5).o
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(LIPO) $$^ -create -output $$@

.libs/$(2)/libtvextension$(5).a: .libs/$(2)/tvextension-main$(5).o
	$(Q) rm -f $$@
	$$(call Q_2,LIPO,  [$1]) $(LIPO) $$^ -create -output $$@
endef

# foreach (var platform in DOTNET_PLATFORMS)
#   foreach (var rid in DOTNET_<platform>_RUNTIME_IDENTIFIERS))
#                                             1      2    3      4                  5                     6
#        call DotNetLibExtensionTemplate (platform, rid, <unused>,         ,       "-dotnet",                       )
#        call DotNetLibExtensionTemplate (platform, rid, <unused>, _CORECLR,       "-dotnet-coreclr",       _CORECLR)

$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetLibExtensionTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),,,-dotnet,,))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetLibExtensionTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),,_CORECLR,-dotnet-coreclr,_CORECLR))))
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call DotNetLibExtensionTemplate,$(DOTNET_$(rid)_SDK_PLATFORM),$(rid),,_NATOVEAOT,-dotnet-nativeaot,_NATOVEAOT))))

dotnet: $(DOTNET_TARGETS)

$(DOTNET_TARGET_DIRS):
	$(Q) mkdir -p $@

all-local:: $(DOTNET_TARGETS)
install-local:: $(DOTNET_TARGETS)

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
