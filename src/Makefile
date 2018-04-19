TOP=..
include $(TOP)/Make.config

BUILD_DIR=build

include $(TOP)/src/frameworks.sources
include $(TOP)/mk/rules.mk

include $(TOP)/src/system-drawing.sources
include $(TOP)/src/monotouch-dialog.sources
include $(TOP)/src/touch-unit.sources

export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

MAC_BUILD_DIR=$(BUILD_DIR)/mac
IOS_BUILD_DIR=$(BUILD_DIR)/ios
WATCH_BUILD_DIR=$(BUILD_DIR)/watch
TVOS_BUILD_DIR=$(BUILD_DIR)/tvos

include $(TOP)/opentk/Makefile.include

#
# Specific warnings that we want reported as errors by generator
# It takes a comma separated list, but an empty list means
# reporting all warnings as errors.
#
GENERATOR_WARNASERROR=
IOS_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
WATCH_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
TVOS_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
MAC_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)

#
# This is the "you are redefining the member", probably we should remove
# those from the contract
#
IOS_WARNINGS_THAT_YOU_SHOULD_FIX=108

include ./OpenGLES/Makefile.include
include ./OpenGLES/Makefile-1.0.include
include ./Makefile.generator

include ./generator-diff.mk

COMMON_TARGET_DIRS = \
	$(BUILD_DIR)/common             \
	$(BUILD_DIR)/common/NativeTypes \

ARGS_32 = -define:ARCH_32
ARGS_64 = -define:ARCH_64

#
# Xamarin.iOS
#

# Add new bindings + source files in frameworks.sources, not here.

IOS_EXTRA_SOURCES = \
	$(IOS_OPENTK_1_0_CORE_SOURCES)   \
	$(IOS_BUILD_DIR)/Constants.cs    \
	$(IOS_BUILD_DIR)/AssemblyInfo.cs \
	Compat.iOS.cs                    \
	$(SHARED_SYSTEM_DRAWING_SOURCES) \

IOS_CORE_SOURCES += $(IOS_EXTRA_SOURCES)
IOS_SOURCES += $(IOS_EXTRA_SOURCES)

IOS_GENERATOR_FLAGS = -inline-selectors -d:IOS -process-enums -warnaserror:$(IOS_GENERATOR_WARNASERROR)
IOS_GENERATOR_native_FLAGS = -d:XAMCORE_2_0 -d:__UNIFIED__
IOS_DEFINES = -define:IPHONE -define:IOS -define:MONOTOUCH -d:NET_2_0 -d:__IOS__ $(APPLETLS_DEFINES)
IOS_native_DEFINES = -d:XAMCORE_2_0 -d:__UNIFIED__
IOS_LIBDIR = $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
IOS_BMONO = MONO_PATH=$(IOS_LIBDIR) $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/btouch-mono
IOS_GENERATOR=$(BUILD_DIR)/common/bgen.exe
IOS_GENERATE=$(SYSTEM_MONO) --debug $(IOS_GENERATOR)
IOS_native_GENERATOR=$(IOS_GENERATOR)
IOS_native_GENERATE=$(IOS_GENERATE)
$(IOS_BUILD_DIR)/Constants.cs: Constants.iOS.cs.in Makefile $(TOP)/Make.config.inc | $(IOS_BUILD_DIR)
	$(call Q_PROF_GEN,ios) sed \
		-e "s/@VERSION@/$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)/g" \
		-e "s/@REVISION@/$(IOS_COMMIT_DISTANCE) ($(CURRENT_BRANCH): $(shell git log -1 --pretty=%h))/g" \
		-e "s/@IOS_SDK_VERSION@/$(IOS_SDK_VERSION)/g" \
		$< > $@

$(IOS_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in
	$(Q) mkdir -p $(IOS_BUILD_DIR)
	$(call Q_PROF_GEN,ios) sed < $< > $@ 's|@PRODUCT_NAME@|$(IOS_PRODUCT)|g; s|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g; s|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH)|g; s|@PACKAGE_VERSION_MAJOR@|$(IOS_PACKAGE_VERSION_MAJOR)|g; s|@PACKAGE_VERSION_MINOR@|$(IOS_PACKAGE_VERSION_MINOR)|g; s|@PACKAGE_VERSION_REV@|$(IOS_PACKAGE_VERSION_REV)|g; s|@PACKAGE_VERSION_BUILD@|$(IOS_PACKAGE_VERSION_BUILD)|g'

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/Version: Version.in $(TOP)/Make.config
	$(Q) sed "s/@VERSION@/$(IOS_PACKAGE_VERSION)/g" Version.in > $@

# filled by TARGETS_template below
IOS_VARIANTS_TARGETS =

# Template to expand into assembly targets for
# each variation (compat 32, native 32, native 64)
#
# Template Arguments:
#
#   - $(3) -> Product assembly name (monotouch.dll | Xamarin.iOS.dll)
#   - $(4) -> MonoTouch.Dialog assembly name
#   - $(5) -> MonoTouch.NUnitLite assembly name
#   - $(6) -> Extra mcs arguments to pass to mcs when *not* using pmcs
#

define IOS_GENERATOR_template
# core.dll
$(IOS_BUILD_DIR)/$(1)/core.dll: $$(IOS_CORE_SOURCES) frameworks.sources
	@mkdir -p $(IOS_BUILD_DIR)/$(1)
	$$(call Q_PROF_CSC,ios/$(1)) $$(IOS_CSC) -out:$$@ -target:library -debug -unsafe \
		-r:$(IOS_LIBDIR)/Mono.Security.dll \
		-nowarn:219,618,114,414,1635,3021,$$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		-define:COREBUILD $$(IOS_DEFINES) \
		$$(IOS_$(1)_DEFINES) \
		$$(IOS_CORE_SOURCES)

# generator.exe
$(IOS_BUILD_DIR)/$(1)/generator.exe: $$(GENERATOR_SOURCES) $(IOS_BUILD_DIR)/$(1)/core.dll
	$$(call Q_PROF_CSC,ios/$(1)) $$(IOS_CSC) -out:$$@ -debug -unsafe \
		-r:$(IOS_BUILD_DIR)/$(1)/core.dll \
		$$(GENERATOR_DEFINES)             \
		$$(GENERATOR_SOURCES)

# generated_sources
# Unfortunatelly, generator uses reflection to load api which means we need to build another
# mono (tools64) to be able to load 2.1 mscorlib (mscorlib internal version must match runtime version)

$(IOS_BUILD_DIR)/$(1)/generated_sources: $$(IOS_$(1)_GENERATOR) $$(IOS_APIS) $(IOS_BUILD_DIR)/$(1)/core.dll $(IOS_BUILD_DIR)/native/$(4)
	$$(call Q_PROF_GEN,ios/$(1)) $$(IOS_$(1)_GENERATE) \
		$$(IOS_GENERATOR_FLAGS) \
		$$(IOS_GENERATOR_$(1)_FLAGS) \
		-core \
		-sourceonly=$$@ \
		-compiler=$$(IOS_CSC) \
		-nostdlib -noconfig \
		-no-mono-path \
		-tmpdir=$(IOS_BUILD_DIR)/$(1) \
		-baselib=$(IOS_BUILD_DIR)/$(1)/core.dll \
		-attributelib=$(IOS_BUILD_DIR)/native/$(4) \
		-r=$$(MONO_PATH)/mcs/class/lib/monotouch/System.dll \
		-ns=MonoTouch.ObjCRuntime \
		-native-exception-marshalling \
		$(2) \
		--target-framework=Xamarin.iOS,v1.0 \
		$$(IOS_APIS)
endef
$(eval $(call IOS_GENERATOR_template,native,--ns=ObjCRuntime,native,Xamarin.iOS.BindingAttributes.dll))

define IOS_TARGETS_template
IOS_VARIANTS_TARGETS += $(IOS_BUILD_DIR)/$(1)/$(3)

# Xamarin.iOS.dll
$(IOS_BUILD_DIR)/$(1)/$(3): $$(IOS_SOURCES) $(IOS_BUILD_DIR)/$(4)/generated_sources $(PRODUCT_KEY_PATH)
	@mkdir -p $(IOS_BUILD_DIR)/$(1)
	$$(call Q_PROF_CSC,ios/$(1)) $$(IOS_CSC) -out:$$@ -target:library -debug -unsafe -optimize \
		-r:$(IOS_LIBDIR)/Mono.Security.dll \
		$$(ARGS_$(6)) \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) $$(IOS_DEFINES) \
		$$(IOS_$(4)_DEFINES) \
		-nowarn:219,618,114,414,1635,3021,$$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		$$(IOS_CSC_FLAGS_XI) \
		$$(IOS_SOURCES) @$(IOS_BUILD_DIR)/$(4)/generated_sources

$(IOS_BUILD_DIR)/$(1)/$(3:.dll=.pdb): $(IOS_BUILD_DIR)/$(1)/$(3)

endef

$(IOS_BUILD_DIR)/compat/%: $(MACIOS_BINARIES_PATH)/% | $(IOS_BUILD_DIR)/compat
	$(Q) cp $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/%: $(IOS_BUILD_DIR)/compat/%
	$(Q) cp $< $@

$(eval $(call IOS_TARGETS_template,native-32,--ns=ObjCRuntime,Xamarin.iOS.dll,native,native-32,32))
$(eval $(call IOS_TARGETS_template,native-64,--ns=ObjCRuntime,Xamarin.iOS.dll,native,native-64,64))

define IOS_LIBS_template
IOS_VARIANTS_TARGETS += $(addprefix $(IOS_BUILD_DIR)/$(1)/,$(4) $(5))

# MonoTouch.Dialog-1
$(IOS_BUILD_DIR)/$(1)%$(4) $(IOS_BUILD_DIR)/$(1)%$(4:.dll=.pdb): $$(IOS_SOURCES) $(IOS_BUILD_DIR)/$(1)/$(3) $$(MONOTOUCH_DIALOG_SOURCES) $(PRODUCT_KEY_PATH) $(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll
	$$(call Q_PROF_CSC,ios/$(1)) $$(IOS_CSC) -out:$$(basename $$@).dll -target:library -debug:portable -optimize -publicsign \
		-keyfile:$(PRODUCT_KEY_PATH) -r:$(IOS_BUILD_DIR)/$(1)/$(3) $(6) -r:$(MONOTOUCH_MONO_PATH)/System.Core.dll -r:$(MONOTOUCH_MONO_PATH)/System.dll -r:$(MONOTOUCH_MONO_PATH)/System.Json.dll \
		-nowarn:219,618,114,414,1635,3021,$$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		$$(MONOTOUCH_DIALOG_SOURCES) $$(MONOTOUCH_DIALOG_RESOURCES)

# MonoTouch.NUnitLite
$(IOS_BUILD_DIR)/$(1)%$(5) $(IOS_BUILD_DIR)/$(1)%$(5:.dll=.pdb): $$(IOS_TOUCHUNIT_SOURCES) $(IOS_BUILD_DIR)/$(1)/$(4) $(PRODUCT_KEY_PATH) $(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll
	$$(call Q_PROF_CSC,ios/$(1)) $$(IOS_CSC) -out:$$(basename $$@).dll -target:library -debug:portable -optimize -publicsign \
		-keyfile:$(PRODUCT_KEY_PATH) -r:$(IOS_BUILD_DIR)/$(1)/$(3) $(6) -r:$(IOS_BUILD_DIR)/$(1)/$(4) -r:$(MONOTOUCH_MONO_PATH)/System.dll -r:$(MONOTOUCH_MONO_PATH)/System.Xml.dll \
		-nowarn:3006,612,649,414,1635 \
		-define:NUNITLITE,CLR_4_0,NET_4_5,__MOBILE__ $$(IOS_DEFINES) \
		$$(IOS_TOUCHUNIT_SOURCES)

# System.Drawing
# not installed or shipped yet - must be compiled manually using "make System.Drawing.dll"
$(IOS_BUILD_DIR)/$(1)/System.Drawing.dll: $$(IOS_SYSTEM_DRAWING_SOURCES) monotouch.dll $(PRODUCT_KEY_PATH)
	$$(call Q_PROF_CSC,ios/$(1)) $$(IOS_CSC) -out:$$@ -target:library -debug:portable -optimize -publicsign \
		-keyfile:$(PRODUCT_KEY_PATH) -r:$(IOS_BUILD_DIR)/$(1)/$(3) $(6) \
		-nowarn:114,169,414,1635 $$(IOS_DEFINES) \
		$$(IOS_SYSTEM_DRAWING_SOURCES)
endef

$(eval $(call IOS_LIBS_template,reference,--ns=ObjCRuntime,Xamarin.iOS.dll,MonoTouch.Dialog-1.dll,MonoTouch.NUnitLite.dll,-define:XAMCORE_2_0 -define:__UNIFIED__))

$(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll: $(IOS_BUILD_DIR)/native-64/Xamarin.iOS.dll | $(IOS_BUILD_DIR)/reference
	@# Don't strip, btouch-native needs to execute code from Xamarin.iOS,
	@# and that'll break if we strip out the code from the reference assembly.
	@# Note that there is only btouch-native executable for both 32 and 64 bits.
	@#$(Q_GEN) mono-cil-strip $< $@
	$(Q) cp $< $@

$(IOS_BUILD_DIR)/reference/Xamarin.iOS.pdb: $(IOS_BUILD_DIR)/native-64/Xamarin.iOS.pdb | $(IOS_BUILD_DIR)/reference
	$(Q) cp $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/% : $(MACIOS_BINARIES_PATH)/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
	$(Q) cp $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/Facades/% : $(MACIOS_BINARIES_PATH)/Facades/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
	$(Q) cp $< $@

$(IOS_BUILD_DIR)/compat/%: $(MACIOS_BINARIES_PATH)/% | $(IOS_BUILD_DIR)/compat
	$(Q) cp $< $@

$(IOS_BUILD_DIR)/compat/Facades/System.Drawing.Primitives.dll : $(MACIOS_BINARIES_PATH)/Facades/System.Drawing.Primitives.dll | $(IOS_BUILD_DIR)/compat/Facades
	$(Q) cp $< $@

#
# Assemblies which require special handling as they have dependency on Xamarin.iOS.dll
# hence we cannot built it as part of Mono BCL
#
$(MONO_PATH)/mcs/class/lib/monotouch/reference_Facades/System.Drawing.Primitives.dll: $(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll
	$(call Q_PROF_CSC,ios/unified) $(MAKE) $(if $(V),,-s) -C $(MONO_PATH)/mcs/class/Facades/System.Drawing.Primitives PROFILE=monotouch LIBRARY_SUBDIR=reference_Facades EXTERNAL_FACADE_DRAWING_REFERENCE="$(abspath $(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll)"
	@touch $@

$(IOS_BUILD_DIR)/%/Facades/System.Drawing.Primitives.dll: $(MONO_PATH)/mcs/class/lib/monotouch/%_Facades/System.Drawing.Primitives.dll | $(IOS_BUILD_DIR)/compat/Facades $(IOS_BUILD_DIR)/reference/Facades
	$(Q) cp $< $@

$(MONO_PATH)/mcs/class/lib/monotouch/reference_Facades/netstandard.dll: $(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll $(IOS_BUILD_DIR)/reference/OpenTK-1.0.dll
	$(call Q_PROF_CSC,ios/unified) $(MAKE) $(if $(V),,-s) -C $(MONO_PATH)/mcs/class/Facades/netstandard PROFILE=monotouch LIBRARY_SUBDIR=reference_Facades EXTERNAL_FACADE_DRAWING_REFERENCE="$(abspath $(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll),$(abspath $(IOS_BUILD_DIR)/reference/OpenTK-1.0.dll)"
	@touch $@

$(IOS_BUILD_DIR)/%/Facades/netstandard.dll: $(MONO_PATH)/mcs/class/lib/monotouch/%_Facades/netstandard.dll | $(IOS_BUILD_DIR)/compat/Facades $(IOS_BUILD_DIR)/reference/Facades
	$(Q) cp $< $@

#
# System.Net.Http is special
#
# System.Net.Http has a 2-step build process:
#
# 1. We build it like all the other BCL assemblies (part of the
#    regular BCL build), since there are other BCL assemblies (Facades)
#    that reference System.Net.Http.dll
# 2. Then we build it a second time in src/, where we add additional
#    source code (public types) and a reference to Xamarin.iOS.dll.
#
# This means we need to install the correct System.Net.Http.dll from here
# (builds/Makefile does not install the BCL version of System.Net.Http.dll)
#

IOS_EXTRA_SYSTEM_NET_HTTP_FILES = \
	$(abspath $(MONO_PATH)/mcs/class/System.Net.Http/CFContentStream.cs)  \
	$(abspath $(MONO_PATH)/mcs/class/System.Net.Http/CFNetworkHandler.cs) \
	$(abspath $(MONO_PATH)/mcs/class/System.Net.Http/HttpClientEx.cs) \
	$(abspath $(TOP)/src/Foundation/NSUrlSessionHandler.cs) \
	$(abspath $(TOP)/src/ObjCRuntime/RuntimeOptions.cs) \

# build (into custom LIBRARY_SUBDIR)
$(MONO_PATH)/mcs/class/lib/monotouch/reference/System.Net.Http%dll $(MONO_PATH)/mcs/class/lib/monotouch/reference/System.Net.Http%pdb: $(IOS_EXTRA_SYSTEM_NET_HTTP_FILES) $(MONO_PATH)/mcs/class/lib/monotouch/System.Net.Http.dll $(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll
	$(call Q_PROF_CSC,ios/unified) $(MAKE) $(if $(V),,-s) -C $(MONO_PATH)/mcs/class/System.Net.Http PROFILE=monotouch LIBRARY_SUBDIR=reference MONOTOUCH_MCS_FLAGS=$(IOS_CSC_FLAGS) EXTRA_LIB_MCS_FLAGS="-r:$(abspath $(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll) $(IOS_EXTRA_SYSTEM_NET_HTTP_FILES) -D:XAMCORE_2_0 -D:XAMARIN_MODERN -D:SYSTEM_NET_HTTP -D:UNIFIED -D:__UNIFIED__"
	@# If the make we executed didn't result in building the dll/pdb (because make determined it wasn't necessary to rebuild),
	@# then this make can end up confused and in an infinite loop. So touch the output files, to make sure they look
	@# rebuilt to make. Unfortunately there's no special variable that means "all targets", $@ only means "target that triggered the rule",
	@# which can be either the dll or the pdb. So add some weird logic to make sure we touch both the dll and the pdb in either case
	@# (we'll touch one of them twice, but that shouldn't be a problem)
	$(Q) touch $@ $(@:.dll=.pdb) $(@:.pdb=.dll)

# sign dll (one target for both compat+reference)
$(IOS_BUILD_DIR)/%/System.Net.Http.dll: $(MONO_PATH)/mcs/class/lib/monotouch/%/System.Net.Http.dll | $(IOS_BUILD_DIR)/compat $(IOS_BUILD_DIR)/reference
	$(Q) cp $< $@
	$(call Q_PROF_SN,ios/$@) MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -R $@ $(PRODUCT_KEY_PATH)

# (one target for both compat+reference)
$(IOS_BUILD_DIR)/%/System.Net.Http.pdb: $(MONO_PATH)/mcs/class/lib/monotouch/%/System.Net.Http.pdb | $(IOS_BUILD_DIR)/compat $(IOS_BUILD_DIR)/reference
	$(Q) cp $< $@

clean-local::
	rm -rf build
	rm -f $(IOS_TARGETS) generated_sources *.mdb *.pdb

IOS_TARGETS_DIRS += \
	$(IOS_BUILD_DIR)                                           \
	$(IOS_BUILD_DIR)/compat                                    \
	$(IOS_BUILD_DIR)/compat/Facades                            \
	$(IOS_BUILD_DIR)/reference                                 \
	$(IOS_BUILD_DIR)/reference/Facades                         \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin                      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/share/doc/MonoTouch      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS     \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1             \

IOS_TARGETS += \
	xamios.csproj                                                                  \
	MonoTouch.NUnitLite.csproj                                                     \
	MonoTouch.Dialog-1.csproj                                                      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/Version                                      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/System.Net.Http.dll             \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/System.Net.Http.pdb             \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/monotouch.dll                   \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/monotouch.dll.mdb               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/OpenTK.dll                      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/OpenTK.pdb                      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/OpenTK-1.0.dll                  \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/OpenTK-1.0.pdb                  \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/OpenTK-1.0.dll.config           \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/MonoTouch.Dialog-1.dll          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/MonoTouch.Dialog-1.pdb          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/MonoTouch.NUnitLite.dll         \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/MonoTouch.NUnitLite.pdb         \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/Facades/System.Drawing.Primitives.dll     \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/Facades/netstandard.dll         \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Xamarin.iOS.dll         \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Xamarin.iOS.pdb     \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/MonoTouch.Dialog-1.dll  \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/MonoTouch.Dialog-1.pdb  \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/MonoTouch.NUnitLite.dll \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/MonoTouch.NUnitLite.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/OpenTK-1.0.dll          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/OpenTK-1.0.pdb          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/OpenTK-1.0.dll.config   \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Net.Http.dll     \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Net.Http.pdb     \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Facades/System.Drawing.Primitives.dll     \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Facades/netstandard.dll     \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.dll                   \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.pdb               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.dll                   \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.pdb               \

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/%.dll: $(IOS_BUILD_DIR)/compat/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/Facades/%.dll: $(IOS_BUILD_DIR)/compat/Facades/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/Facades
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/%.pdb: $(IOS_BUILD_DIR)/compat/%.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/%.mdb: $(IOS_BUILD_DIR)/compat/%.mdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/%.config: $(IOS_BUILD_DIR)/compat/%.config | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/share/doc/MonoTouch/%: api-diffs/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/share/doc/MonoTouch
	$(Q) install -m 0644 $< $@

# reference assemblies, this is just for compilation with XS
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/%.dll: $(IOS_BUILD_DIR)/reference/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Facades/%.dll: $(IOS_BUILD_DIR)/reference/Facades/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Facades
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/%.pdb: $(IOS_BUILD_DIR)/reference/%.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/%.config: $(IOS_BUILD_DIR)/reference/%.config | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS
	$(Q) install -m 0644 $< $@

# the actual architecture-specific versions
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.dll: $(IOS_BUILD_DIR)/native-32/Xamarin.iOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.pdb: $(IOS_BUILD_DIR)/native-32/Xamarin.iOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.dll: $(IOS_BUILD_DIR)/native-64/Xamarin.iOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.pdb: $(IOS_BUILD_DIR)/native-64/Xamarin.iOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits
	$(Q) install -m 0644 $< $@

$(IOS_TARGETS_DIRS):
	$(Q) mkdir -p $@

xamios.csproj: xamios.tmpl.csproj Makefile $(wildcard $(TOP)/src/*.sources)
	@sed -e 's*<!--%FILES%-->*$(foreach file,$(IOS_SOURCES),<Compile Include="$(file)"/>)*' -e 's*<!--%APIS%-->*$(foreach file,$(IOS_APIS),<None Include="$(file)"/>)*' $< | xmllint --format - > $@

MonoTouch.NUnitLite.csproj: MonoTouch.NUnitLite.templ.csproj Makefile touch-unit.sources
	$(Q) sed -e 's*<!--%FILES%-->*$(foreach file,$(IOS_TOUCHUNIT_SOURCES),<Compile Include="$(file)"/>)*' $< | xmllint --format - > $@

MonoTouch.Dialog-1.csproj: MonoTouch.Dialog-1.templ.csproj Makefile touch-unit.sources
	$(Q) sed -e 's*<!--%FILES%-->*$(foreach file,$(MONOTOUCH_DIALOG_SOURCES),<Compile Include="$(file)"/>)*' $< | xmllint --format - > $@

PROJECT_FILES += xamios.csproj MonoTouch.NUnitLite.csproj MonoTouch.Dialog-1.csproj

all-ios: $(IOS_TARGETS)
install-ios: $(IOS_TARGETS)

ifdef INCLUDE_IOS
INSTALL_TARGETS+=install-ios
ALL_TARGETS+=all-ios
endif

#
# Xamarin.Mac
#

MAC_WARNINGS_TO_FIX := 114,108
# 4014 is "The statement is not awaited and execution of current method continues before the call is completed. Consider using `await' operator or calling `Wait' method"
MAC_WARNINGS_TO_FIX := $(MAC_WARNINGS_TO_FIX),4014

MAC_COMMON_DEFINES = -define:NET_2_0,XAMARIN_MAC,MONOMAC,XAMCORE_2_0,__UNIFIED__
MAC_full_ARGS = -define:NO_SYSTEM_DRAWING -define:XAMMAC_SYSTEM_MONO
MAC_mobile_ARGS = 
MAC_BOOTSTRAP_DEFINES = $(MAC_COMMON_DEFINES),COREBUILD
MAC_GENERATED_DEFINES = -d:MONOMAC -d:XAMARIN_MAC -d:XAMCORE_2_0 -d:__UNIFIED__

$(MAC_BUILD_DIR)/$(1)/$(3).pdb: $(MAC_BUILD_DIR)/$(1)/$(3).dll

SN_KEY = $(PRODUCT_KEY_PATH)

MAC_EXTRA_CORE_SOURCES += \
	$(MAC_BUILD_DIR)/Constants.cs \

# Add new bindings + source files in frameworks.sources, not here.

MAC_CORE_SOURCES += \
	$(MAC_EXTRA_CORE_SOURCES) \

MAC_SOURCES += \
	$(MAC_EXTRA_CORE_SOURCES) \
	$(MAC_BUILD_DIR)/AssemblyInfo.cs \
	Obsoletes.cs \
	Darwin/KernelNotification.cs \
	Darwin/SystemLog.cs \

# CFNetwork is shipped:
# * In a separate library (XamMac.CFNetwork.dll) for Classic.
# * Inside the other platform assemblies (Xamarin.Mac.dll) for all the other profiles.
# This means we can't use the standard framework logic for these sources.

MAC_CFNETWORK_SOURCES = \
	CFNetwork/Content.cs \
	CFNetwork/MessageHandler.cs \
	CFNetwork/WebRequestStream.cs \
	CFNetwork/WebResponseStream.cs \
	CFNetwork/WorkerThread.cs

MAC_MODERNHTTP_SOURCES = \
	$(abspath $(MONO_PATH)/mcs/class/System.Net.Http/HttpClientEx.cs) \
	$(abspath $(TOP)/src/Foundation/NSUrlSessionHandler.cs) \
	$(abspath $(MONO_PATH)/mcs/class/System.Net.Http/CFContentStream.cs)  \
	$(abspath $(MONO_PATH)/mcs/class/System.Net.Http/CFNetworkHandler.cs) \

$(MAC_BUILD_DIR)/Constants.cs: Constants.mac.cs.in Makefile $(TOP)/Make.config.inc | $(MAC_BUILD_DIR)
	$(Q) sed \
			-e "s/@VERSION@/$(MAC_PACKAGE_VERSION_MAJOR).$(MAC_PACKAGE_VERSION_MINOR).$(MAC_PACKAGE_VERSION_REV)/g" \
			-e "s/@REVISION@/$(MAC_COMMIT_DISTANCE) ($(CURRENT_BRANCH): $(shell git log -1 --pretty=%h))/g" \
			-e "s/@OSX_SDK_VERSION@/$(OSX_SDK_VERSION)/g" \
		$< > $@

xammac.csproj: xammac.tmpl.csproj Makefile $(wildcard $(TOP)/src/*.sources)
	@sed -e 's*<!--%FILES%-->*$(foreach file,$(MAC_SOURCES) $(SHARED_SYSTEM_DRAWING_SOURCES),<Compile Include="$(file)"/>)*' -e 's*<!--%APIS%-->*$(foreach file,$(MAC_APIS),<None Include="$(file)"/>)*' $< | xmllint --format - > $@

PROJECT_FILES += xammac.csproj

$(MAC_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in
	@mkdir -p $(MAC_BUILD_DIR)
	$(call Q_PROF_GEN,mac) sed < $< > $@ 's|@PRODUCT_NAME@|$(MAC_PRODUCT)|g; s|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g; s|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH)|g; s|@PACKAGE_VERSION_MAJOR@|$(MAC_PACKAGE_VERSION_MAJOR)|g; s|@PACKAGE_VERSION_MINOR@|$(MAC_PACKAGE_VERSION_MINOR)|g; s|@PACKAGE_VERSION_REV@|$(MAC_PACKAGE_VERSION_REV)|g; s|@PACKAGE_VERSION_BUILD@|$(MAC_PACKAGE_VERSION_BUILD)|g'

# We can't pass the --target-framework values as parameters to the templates, because the commas interfere with Make's parameter parsing.
xm_full_profile=--target-framework=Xamarin.Mac,Version=v4.5,Profile=Full
xm_mobile_profile=--target-framework=Xamarin.Mac,Version=v2.0,Profile=Mobile

MAC_GENERATOR=$(BUILD_DIR)/common/bgen.exe
MAC_GENERATE=$(SYSTEM_MONO) --debug $(MAC_GENERATOR)
MAC_full_GENERATOR=$(MAC_GENERATOR)
MAC_full_GENERATE=$(MAC_GENERATE)
MAC_mobile_GENERATOR=$(MAC_GENERATOR)
MAC_mobile_GENERATE=$(MAC_GENERATE)

define MAC_GENERATOR_template
$(MAC_BUILD_DIR)/$(1)/core.dll: $(MAC_CORE_SOURCES) frameworks.sources
	@mkdir -p $(MAC_BUILD_DIR)/$(1)
	$$(call Q_PROF_CSC,mac/$(1)) \
		$$(MAC_$(1)_CSC) -out:$$@ -target:library -debug -unsafe -nowarn:3021,612,618,1635 \
		$$(MAC_BOOTSTRAP_DEFINES) \
		$(3) \
		$$(MAC_CORE_SOURCES)

$(MAC_BUILD_DIR)/$(1)/_bmac.exe: $(MAC_BUILD_DIR)/$(1)/core.dll $(MAC_APIS) $(GENERATOR_SOURCES)
	$$(call Q_PROF_CSC,mac/$(1)) \
		$$(MAC_$(1)_CSC) -out:$$@ -debug -unsafe \
		$(GENERATOR_DEFINES) \
		-r:$$(@D)/core.dll \
		$(GENERATOR_SOURCES)

$(MAC_BUILD_DIR)/$(1)/generated-sources: $$(MAC_$(1)_GENERATOR) $(MAC_APIS) $(MAC_BUILD_DIR)/$(1)/core.dll $(MAC_BUILD_DIR)/$(7)
	$$(call Q_PROF_GEN,mac/$(1)) $$(MAC_$(1)_GENERATE) \
		$(MAC_GENERATED_DEFINES) \
		-compiler:$$(MAC_$(1)_CSC) \
		-process-enums \
		-warnaserror:$(MAC_GENERATOR_WARNASERROR) \
		-native-exception-marshalling \
		-core \
		-sourceonly:$$@ \
		-tmpdir:$$(@D) \
		-baselib:$$(@D)/core.dll \
		-attributelib:$(MAC_BUILD_DIR)/$(7) \
		-d:NO_SYSTEM_DRAWING \
		$(2) \
		$(3) \
		$(xm_$(1)_profile) \
		$(MAC_APIS)
endef

$(eval $(call MAC_GENERATOR_template,full,--ns=ObjCRuntime,-d:NO_SYSTEM_DRAWING,,,full,Xamarin.Mac-full.BindingAttributes.dll))
$(eval $(call MAC_GENERATOR_template,mobile,--ns=ObjCRuntime,$(SHARED_SYSTEM_DRAWING_SOURCES),,,mobile,Xamarin.Mac-mobile.BindingAttributes.dll))

define MAC_TARGETS_template
$(MAC_BUILD_DIR)/$(1)/$(2): $(MAC_BUILD_DIR)/$(3)/generated-sources $(MAC_SOURCES) $(MAC_CFNETWORK_SOURCES) $(MAC_CLASSIC_SOURCES) $(SN_KEY)
	@mkdir -p $(MAC_BUILD_DIR)/$(1)
	$$(call Q_PROF_CSC,mac/$(1)) \
		$$(MAC_$(3)_CSC) -out:$$@ -target:library -debug -unsafe \
		$$(MAC_COMMON_DEFINES),OBJECT_REF_TRACKING \
		-r:Mono.Security.dll \
		$$(MAC_$(3)_ARGS) \
		$$(ARGS_$(6)) \
		-publicsign -keyfile:$(SN_KEY) \
		-nowarn:3021,1635,612,618,0219,0414,$(MAC_WARNINGS_TO_FIX) \
		$$(MAC_CSC_FLAGS_XM) \
		$(4) \
		@$$< $$(MAC_SOURCES)

$(MAC_BUILD_DIR)/$(1)/$(2:.dll=.pdb): $(MAC_BUILD_DIR)/$(1)/$(2)

endef

$(eval $(call MAC_TARGETS_template,mobile-32,Xamarin.Mac.dll,mobile,$(MAC_CFNETWORK_SOURCES) $(MAC_MODERNHTTP_SOURCES) $(SHARED_SYSTEM_DRAWING_SOURCES) $(APPLETLS_DEFINES) -D:XAMARIN_MODERN -D:UNIFIED -D:__UNIFIED__,mobile-32,32))
$(eval $(call MAC_TARGETS_template,mobile-64,Xamarin.Mac.dll,mobile,$(MAC_CFNETWORK_SOURCES) $(MAC_MODERNHTTP_SOURCES) $(SHARED_SYSTEM_DRAWING_SOURCES) $(APPLETLS_DEFINES) -D:XAMARIN_MODERN -D:UNIFIED -D:__UNIFIED__,mobile-64,64))
$(eval $(call MAC_TARGETS_template,full-32,Xamarin.Mac.dll,full,$(MAC_CFNETWORK_SOURCES) $(MAC_MODERNHTTP_SOURCES) -D:XAMARIN_MODERN -D:UNIFIED -D:__UNIFIED__,full-32,32))
$(eval $(call MAC_TARGETS_template,full-64,Xamarin.Mac.dll,full,$(MAC_CFNETWORK_SOURCES) $(MAC_MODERNHTTP_SOURCES) -D:XAMARIN_MODERN -D:UNIFIED -D:__UNIFIED__,full-64,64))

$(MAC_BUILD_DIR)/%-reference/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/%-64/Xamarin.Mac.dll
	@mkdir -p $(@D)
	$(Q) cp $^ $@

$(MAC_BUILD_DIR)/%-reference/Xamarin.Mac.pdb: $(MAC_BUILD_DIR)/%-64/Xamarin.Mac.pdb
	@mkdir -p $(@D)
	$(Q) cp $^ $@

# System.Drawing.Primitives.dll is special


$(MONO_PATH)/mcs/class/lib/xammac/reference_Facades/System.Drawing.Primitives.dll: $(MAC_BUILD_DIR)/mobile-reference/Xamarin.Mac.dll
	$(call Q_PROF_CSC,xammac) $(MAKE) $(if $(V),,-s) -C $(MONO_PATH)/mcs/class/Facades/System.Drawing.Primitives PROFILE=xammac LIBRARY_SUBDIR=reference_Facades EXTERNAL_FACADE_DRAWING_REFERENCE="$(abspath $(MAC_BUILD_DIR)/mobile-reference/Xamarin.Mac.dll)"
	@touch $@

$(MAC_BUILD_DIR)/mobile/Facades/System.Drawing.Primitives.dll: $(MONO_PATH)/mcs/class/lib/xammac/reference_Facades/System.Drawing.Primitives.dll $(MAC_BUILD_DIR)/mobile/Facades 
	$(Q) cp $< $@

# netstandard.dll is special

$(MONO_PATH)/mcs/class/lib/xammac/reference_Facades/netstandard.dll: $(MAC_BUILD_DIR)/mobile-reference/Xamarin.Mac.dll $(MAC_BUILD_DIR)/mobile-reference/OpenTK.dll
	$(call Q_PROF_CSC,xammac) $(MAKE) $(if $(V),,-s) -C $(MONO_PATH)/mcs/class/Facades/netstandard PROFILE=xammac LIBRARY_SUBDIR=reference_Facades EXTERNAL_FACADE_DRAWING_REFERENCE="$(abspath $(MAC_BUILD_DIR)/mobile-reference/Xamarin.Mac.dll),$(abspath $(MAC_BUILD_DIR)/mobile-reference/OpenTK.dll)"
	@touch $@

$(MAC_BUILD_DIR)/mobile/Facades/netstandard.dll: $(MONO_PATH)/mcs/class/lib/xammac/reference_Facades/netstandard.dll $(MAC_BUILD_DIR)/mobile/Facades
	$(Q) cp $< $@

$(MAC_BUILD_DIR)/compat/XamMac.CFNetwork.dll: $(MACIOS_BINARIES_PATH)/XamMac.CFNetwork.dll | $(MAC_BUILD_DIR)/compat
	$(Q) cp $< $@
	$(Q) cp $<.mdb $@.mdb

$(MAC_BUILD_DIR)/compat/XamMac.dll: $(MACIOS_BINARIES_PATH)/XamMac.dll | $(MAC_BUILD_DIR)/compat
	$(Q) cp $< $@
	$(Q) cp $<.mdb $@.mdb

MAC_VARIANTS_TARGETS = \
	$(MAC_BUILD_DIR)/compat/XamMac.dll \
	$(MAC_BUILD_DIR)/mobile-32/Xamarin.Mac.dll \
	$(MAC_BUILD_DIR)/mobile-64/Xamarin.Mac.dll \
	$(MAC_BUILD_DIR)/full-32/Xamarin.Mac.dll \
	$(MAC_BUILD_DIR)/full-64/Xamarin.Mac.dll \
	$(MAC_BUILD_DIR)/mobile-reference/Xamarin.Mac.dll \
	$(MAC_BUILD_DIR)/full-reference/Xamarin.Mac.dll \
	$(MAC_BUILD_DIR)/mobile/Facades/System.Drawing.Primitives.dll \
	$(MAC_BUILD_DIR)/mobile/Facades/netstandard.dll \
	$(MAC_BUILD_DIR)/compat/XamMac.CFNetwork.dll

INSTALL_TARGETS+=install-mac
ALL_TARGETS+=all-mac

MAC_TARGETS_DIRS += \
	$(MAC_BUILD_DIR) \
	$(MAC_BUILD_DIR)/mobile \
	$(MAC_BUILD_DIR)/mobile/Facades \
	$(MAC_BUILD_DIR)/full \
	$(MAC_BUILD_DIR)/compat \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/mobile      \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/full        \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile    \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full      \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full   \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/net_4_5   \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5         \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin                  \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig        \

MAC_TARGETS += \
	xammac.csproj                                                                       \
	$(MAC_VARIANTS_TARGETS)                                                             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMac.dll                      \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.dll     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.pdb     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.dll       \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.pdb       \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile/Xamarin.Mac.dll        \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile/Xamarin.Mac.pdb        \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/mobile/Xamarin.Mac.dll          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/mobile/Xamarin.Mac.pdb          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full/Xamarin.Mac.dll          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full/Xamarin.Mac.pdb          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/full/Xamarin.Mac.dll            \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/full/Xamarin.Mac.pdb            \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMac.CFNetwork.dll            \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.dll     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.pdb     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.pdb             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/Version                                  \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig/xammac.pc                  \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades/System.Drawing.Primitives.dll          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades/netstandard.dll

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades/System.Drawing.Primitives.dll: $(MAC_BUILD_DIR)/mobile/Facades/System.Drawing.Primitives.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades/netstandard.dll: $(MAC_BUILD_DIR)/mobile/Facades/netstandard.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMac.dll: $(MAC_BUILD_DIR)/compat/XamMac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono
	$(Q) install -m 0755 $< $@
	$(Q) install -m 0644 $<.mdb $@.mdb

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/mobile-reference/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.pdb: $(MAC_BUILD_DIR)/mobile-reference/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/mobile-64/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile/Xamarin.Mac.pdb: $(MAC_BUILD_DIR)/mobile-64/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/mobile/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/mobile-32/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/mobile
	$(Q) install -m 0755 $< $@
	
$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/mobile/Xamarin.Mac.pdb: $(MAC_BUILD_DIR)/mobile-32/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/mobile
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/full-reference/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.pdb : $(MAC_BUILD_DIR)/full-reference/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/full-64/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full/Xamarin.Mac.pdb: $(MAC_BUILD_DIR)/full-64/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/full/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/full-32/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/full
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/full/Xamarin.Mac.pdb : $(MAC_BUILD_DIR)/full-32/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/full
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMac.CFNetwork.dll: $(MAC_BUILD_DIR)/compat/XamMac.CFNetwork.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono
	$(Q) install -m 0755 $< $@
	$(Q) install -m 0644 $< $(@:.dll=.pdb)

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.dll $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.pdb: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac
	$(Q) ln -sF ../../reference/mobile/$(@F) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.pdb: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5
	$(Q) ln -sF ../../reference/full/$(@F) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/Version: $(TOP)/Make.config
	$(Q) echo $(MAC_PACKAGE_VERSION) > $@
	$(Q) chmod 0644 $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig/xammac.pc: $(TOP)/Make.config | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig
	$(Q) sed -e "s/@PACKAGE_VERSION@/$(MAC_PACKAGE_VERSION)/g" xammac.pc.in > $@
	$(Q) chmod 0644 $@

$(MAC_TARGETS_DIRS):
	$(Q) mkdir -p $@

install-mac: $(MAC_TARGETS)
all-mac: $(MAC_TARGETS)

#
# Xamarin.WatchOS
#

WATCH_DEFINES = -define:IPHONE -define:MONOTOUCH -d:NET_2_0 -d:WATCH -d:XAMCORE_2_0 -d:XAMCORE_3_0 -d:__WATCHOS__ -d:__UNIFIED__
WATCH_LIBDIR = $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS
WATCH_BMONO = MONO_PATH=$(WATCH_LIBDIR)/repl $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/bwatch-mono
WATCH_GENERATOR=$(BUILD_DIR)/common/bgen.exe
WATCH_GENERATE=$(SYSTEM_MONO) --debug $(WATCH_GENERATOR)
WATCH_GENERATED_DEFINES= -d:WATCH -d:XAMCORE_3_0 -d:XAMCORE_2_0 -d:__UNIFIED__
WATCH_BCL_DIR = $(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch

WATCHOS_EXTRA_CORE_SOURCES = \
    $(WATCH_BUILD_DIR)/Constants.cs    \
    $(WATCH_BUILD_DIR)/AssemblyInfo.cs \
    $(IOS_OPENTK_1_0_CORE_SOURCES)     \
    AudioToolbox/AudioBuffers.cs       \
    AudioToolbox/AudioType.cs          \
	$(SHARED_SYSTEM_DRAWING_SOURCES)   \
	$(MONO_PATH)/mcs/class/System.Drawing/System.Drawing/Color.cs \
	$(MONO_PATH)/mcs/class/System.Drawing/System.Drawing/KnownColor.cs \
	$(MONO_PATH)/mcs/class/System.Drawing/System.Drawing/KnownColors.cs

WATCHOS_CORE_SOURCES +=           \
    $(WATCHOS_EXTRA_CORE_SOURCES) \

WATCHOS_SOURCES +=                \
    $(WATCHOS_EXTRA_CORE_SOURCES) \

$(WATCH_BUILD_DIR)/Constants.cs: $(TOP)/src/Constants.watch.cs.in Makefile $(TOP)/Make.config.inc | $(WATCH_BUILD_DIR)
	$(call Q_PROF_GEN,watch) sed \
		-e "s/@VERSION@/$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)/g" \
		-e "s/@REVISION@/$(IOS_COMMIT_DISTANCE) ($(CURRENT_BRANCH): $(shell git log -1 --pretty=%h))/g" \
		-e "s/@WATCH_SDK_VERSION@/$(WATCH_SDK_VERSION)/g" \
		$< > $@

$(WATCH_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in $(TOP)/Make.config  $(TOP)/.git/HEAD | $(WATCH_BUILD_DIR)
	$(call Q_PROF_GEN,watch) sed \
		-e 's|@PRODUCT_NAME@|Xamarin.WatchOS|g' \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH)|g' \
		-e 's|@PACKAGE_VERSION_MAJOR@|$(IOS_PACKAGE_VERSION_MAJOR)|g' \
		-e 's|@PACKAGE_VERSION_MINOR@|$(IOS_PACKAGE_VERSION_MINOR)|g' \
		-e 's|@PACKAGE_VERSION_REV@|$(IOS_PACKAGE_VERSION_REV)|g' \
		-e 's|@PACKAGE_VERSION_BUILD@|$(IOS_PACKAGE_VERSION_BUILD)|g' \
		$< > $@.tmp
	$(Q) diff $@ $@.tmp >/dev/null 2>&1 || mv -f $@.tmp $@
	$(Q) rm -f $@.tmp
	$(Q) touch $@

$(WATCH_BUILD_DIR)/watch/core.dll: $(WATCHOS_CORE_SOURCES) frameworks.sources | $(WATCH_BUILD_DIR)/watch
	@mkdir -p $(WATCH_BUILD_DIR)/watch
	$(call Q_PROF_CSC,watch) $(WATCH_CSC) -out:$@ -target:library -debug -unsafe \
		-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		-define:COREBUILD    \
		$(WATCH_DEFINES)     \
		$(WATCHOS_CORE_SOURCES)

# generator.exe
$(WATCH_BUILD_DIR)/watch/generator.exe: $(GENERATOR_SOURCES) $(WATCH_BUILD_DIR)/watch/core.dll
	$(call Q_PROF_CSC,watch) $(WATCH_CSC) -out:$@ -debug -unsafe \
		-r:$(WATCH_BUILD_DIR)/watch/core.dll \
		$(GENERATOR_SOURCES)                  \
		$(WATCH_DEFINES)     \
		$(GENERATOR_DEFINES)                  \

# generated_sources
$(WATCH_BUILD_DIR)/watch/generated_sources: $(WATCH_GENERATOR) $(WATCHOS_APIS) $(WATCH_BUILD_DIR)/watch/core.dll $(WATCH_BUILD_DIR)/Xamarin.WatchOS.BindingAttributes.dll
	$(call Q_PROF_GEN,watch) $(WATCH_GENERATE) \
		-inline-selectors                                        \
		-process-enums                                           \
		-warnaserror:$(WATCH_GENERATOR_WARNASERROR)              \
		-core                                                    \
		-sourceonly=$@                                           \
		-compiler=$(WATCH_CSC) \
		-nostdlib -noconfig                                      \
		-no-mono-path                                            \
		-tmpdir=$(WATCH_BUILD_DIR)/watch                         \
		-baselib=$(WATCH_BUILD_DIR)/watch/core.dll               \
		-attributelib=$(WATCH_BUILD_DIR)/Xamarin.WatchOS.BindingAttributes.dll \
		-r=$(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/System.dll \
		-ns=MonoTouch.ObjCRuntime                                \
		-native-exception-marshalling                            \
		$(WATCH_GENERATED_DEFINES)				\
		--ns:ObjCRuntime                                         \
		$(WATCHOS_APIS)                                            \
		--target-framework=Xamarin.WatchOS,v1.0                     \

$(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS%dll $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS%pdb: $(WATCHOS_SOURCES) $(WATCH_BUILD_DIR)/watch/generated_sources $(PRODUCT_KEY_PATH) | $(WATCH_BUILD_DIR)/watch-32
	$(call Q_PROF_CSC,watch) $(WATCH_CSC) -out:$(basename $@).dll -target:library -debug -unsafe -optimize \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) $(WATCH_DEFINES) \
		$(ARGS_32) \
		-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		$(WATCHOS_SOURCES) @$(WATCH_BUILD_DIR)/watch/generated_sources

$(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll: $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS.dll | $(WATCH_BUILD_DIR)/reference
	@# Don't strip, bwatch needs to execute code from Xamarin.WatchOS (attributes),
	@# and that'll break if we strip out the code from the reference assembly.
	@# Note that there is only bwatch executable for both 32 and 64 bits.
	@#$(Q_GEN) mono-cil-strip $< $@
	$(Q) cp $< $@

$(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.pdb: $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS.pdb | $(WATCH_BUILD_DIR)/reference
	$(Q) cp $< $@

# MonoTouch.NUnitLite
$(WATCH_BUILD_DIR)/reference/MonoTouch.NUnitLite%dll $(WATCH_BUILD_DIR)/reference/MonoTouch.NUnitLite%pdb: $(WATCHOS_TOUCHUNIT_SOURCES) $(PRODUCT_KEY_PATH) $(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll
	$(call Q_PROF_CSC,watch) $(SYSTEM_CSC) -out:$(basename $@).dll -target:library -debug:portable -optimize -publicsign -noconfig -nostdlib \
		-keyfile:$(PRODUCT_KEY_PATH) -r:$(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll -r:$(MONOTOUCH_WATCH_MONO_PATH)/mscorlib.dll -r:$(MONOTOUCH_WATCH_MONO_PATH)/System.dll -r:$(MONOTOUCH_WATCH_MONO_PATH)/System.Xml.dll \
		-nowarn:3006,612,649,414,1635 \
		-define:NUNITLITE,CLR_4_0,NET_4_5,__MOBILE__ $(WATCH_DEFINES) \
		$(WATCHOS_TOUCHUNIT_SOURCES)

# System.Drawing.Primitives.dll is special

$(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/reference_Facades/System.Drawing.Primitives.dll: $(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll
	$(call Q_PROF_CSC,watch) $(MAKE) $(if $(V),,-s) -C $(WATCH_MONO_PATH)/mcs/class/Facades/System.Drawing.Primitives PROFILE=monotouch_watch LIBRARY_SUBDIR=reference_Facades EXTERNAL_FACADE_DRAWING_REFERENCE="$(abspath $(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll)"
	@touch $@

$(WATCH_BUILD_DIR)/%/Facades/System.Drawing.Primitives.dll: $(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/%_Facades/System.Drawing.Primitives.dll | $(WATCH_BUILD_DIR)/reference/Facades
	$(Q) cp $< $@

# netstandard.dll is special

$(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/reference_Facades/netstandard.dll: $(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll
	$(call Q_PROF_CSC,watch) $(MAKE) $(if $(V),,-s) -C $(WATCH_MONO_PATH)/mcs/class/Facades/netstandard PROFILE=monotouch_watch LIBRARY_SUBDIR=reference_Facades EXTERNAL_FACADE_DRAWING_REFERENCE="$(abspath $(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll)"
	@touch $@

$(WATCH_BUILD_DIR)/%/Facades/netstandard.dll: $(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/%_Facades/netstandard.dll | $(WATCH_BUILD_DIR)/reference/Facades
	$(Q) cp $< $@

# System.Net.Http.dll is special. See comment in src/Makefile

WATCH_EXTRA_SYSTEM_NET_HTTP_FILES = \
	$(abspath $(WATCH_MONO_PATH)/mcs/class/System.Net.Http/HttpClientEx.cs) \
	$(abspath $(TOP)/src/Foundation/NSUrlSessionHandler.cs) \
	$(abspath $(TOP)/src/ObjCRuntime/RuntimeOptions.cs) \

# build (into custom LIBRARY_SUBDIR)
$(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/reference/System.Net.Http%dll $(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/reference/System.Net.Http%pdb: $(WATCH_EXTRA_SYSTEM_NET_HTTP_FILES) $(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/System.Net.Http.dll $(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll
	$(call Q_PROF_CSC,watch) $(MAKE) $(if $(V),,-s) -C $(WATCH_MONO_PATH)/mcs/class/System.Net.Http PROFILE=monotouch_watch LIBRARY_SUBDIR=reference MONOTOUCH_MCS_FLAGS=$(IOS_CSC_FLAGS) EXTRA_LIB_MCS_FLAGS="-r:$(abspath $(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll) $(WATCH_EXTRA_SYSTEM_NET_HTTP_FILES) -D:XAMCORE_2_0 -D:XAMCORE_3_0 -D:XAMARIN_MODERN -D:SYSTEM_NET_HTTP -D:UNIFIED -D:__UNIFIED__"
	@# If the make we executed didn't result in building the dll/pdb (because make determined it wasn't necessary to rebuild),
	@# then this make can end up confused and in an infinite loop. So touch the output files, to make sure they look
	@# rebuilt to make. Unfortunately there's no special variable that means "all targets", $@ only means "target that triggered the rule",
	@# which can be either the dll or the pdb. So add some weird logic to make sure we touch both the dll and the pdb in either case
	@# (we'll touch one of them twice, but that shouldn't be a problem)
	$(Q) touch $@ $(@:.dll=.pdb) $(@:.pdb=.dll)

# sign dll
$(WATCH_BUILD_DIR)/reference/System.Net.Http.dll: $(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/reference/System.Net.Http.dll | $(WATCH_BUILD_DIR)/reference
	$(Q) cp $< $@
	$(call Q_PROF_SN,watch) MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -R $@ $(PRODUCT_KEY_PATH)

$(WATCH_BUILD_DIR)/reference/System.Net.Http.pdb: $(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/reference/System.Net.Http.pdb | $(WATCH_BUILD_DIR)/reference
	$(Q) cp $< $@

xamwatch.csproj: xamwatch.tmpl.csproj Makefile $(wildcard $(TOP)/*.sources)
	@sed -e 's*<!--%FILES%-->*$(foreach file,$(WATCHOS_SOURCES),<Compile Include="$(file)"/>)*' -e 's*<!--%APIS%-->*$(foreach file,$(WATCHOS_APIS),<None Include="$(file)"/>)*' $< | xmllint --format - > $@

MonoTouch.NUnitLite.watchos.csproj: MonoTouch.NUnitLite.watchos.templ.csproj Makefile touch-unit.sources
	$(Q) sed -e 's*<!--%FILES%-->*$(foreach file,$(WATCHOS_TOUCHUNIT_SOURCES),<Compile Include="$(file)"/>)*' $< | xmllint --format - > $@

PROJECT_FILES += xamwatch.csproj MonoTouch.NUnitLite.watchos.csproj

clean-watch:
	$(Q) rm -rf $(WATCH_BUILD_DIR)
	$(Q) rm -f $(WATCH_TARGETS)

WATCH_TARGETS_DIRS +=                                          \
	$(WATCH_BUILD_DIR)                                         \
	$(WATCH_BUILD_DIR)/watch                                   \
	$(WATCH_BUILD_DIR)/watch-32                                \
	$(WATCH_BUILD_DIR)/reference                               \
	$(WATCH_BUILD_DIR)/reference/Facades                       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/share/doc/Xamarin.WatchOS\

WATCH_TARGETS += \
	xamwatch.csproj                                                                \
	MonoTouch.NUnitLite.watchos.csproj                                                      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/Xamarin.WatchOS.dll          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/Xamarin.WatchOS.pdb      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.WatchOS.dll                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.WatchOS.pdb                    \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/System.Net.Http.dll          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/System.Net.Http.pdb          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/MonoTouch.NUnitLite.dll      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/MonoTouch.NUnitLite.pdb      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/Facades/System.Drawing.Primitives.dll      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/Facades/netstandard.dll      \

# reference assemblies, this is just for compilation with XS
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.dll: $(WATCH_BUILD_DIR)/reference/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/Facades/%.dll: $(WATCH_BUILD_DIR)/reference/Facades/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/Facades
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.pdb: $(WATCH_BUILD_DIR)/reference/%.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.config: $(WATCH_BUILD_DIR)/reference/%.config | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS
	$(Q) install -m 0644 $< $@

# the actual architecture-specific versions
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.WatchOS.dll: $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.WatchOS.pdb: $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits
	$(Q) install -m 0644 $< $@

$(WATCH_TARGETS_DIRS):
	$(Q) mkdir -p $@

all-watch: $(WATCH_TARGETS)
install-watch: $(WATCH_TARGETS)

ifdef INCLUDE_WATCH
ALL_TARGETS += all-watch
INSTALL_TARGETS += install-watch
endif

#
# Xamarin.TVOS
#

TVOS_DEFINES = -define:IPHONE -define:MONOTOUCH -d:NET_2_0 -d:TVOS -d:XAMCORE_2_0 -d:XAMCORE_3_0 -d:__TVOS__ $(APPLETLS_DEFINES) -d:__UNIFIED__
TVOS_LIBDIR = $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS
TVOS_BMONO = MONO_PATH=$(TVOS_LIBDIR)/repl $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/btv-mono
TVOS_GENERATOR=$(BUILD_DIR)/common/bgen.exe
TVOS_GENERATE=$(SYSTEM_MONO) --debug $(TVOS_GENERATOR)
TVOS_GENERATED_DEFINES= -d:TVOS -d:XAMCORE_3_0 -d:XAMCORE_2_0 -d:__UNIFIED__

TVOS_EXTRA_CORE_SOURCES = \
	$(TVOS_BUILD_DIR)/Constants.cs    \
    $(TVOS_BUILD_DIR)/AssemblyInfo.cs \
	$(IOS_OPENTK_1_0_CORE_SOURCES)    \
	$(SHARED_SYSTEM_DRAWING_SOURCES) \

TVOS_CORE_SOURCES +=           \
	$(TVOS_EXTRA_CORE_SOURCES) \

TVOS_SOURCES +=                \
	$(TVOS_EXTRA_CORE_SOURCES) \

$(TVOS_BUILD_DIR)/Constants.cs: $(TOP)/src/Constants.tvos.cs.in Makefile $(TOP)/Make.config.inc | $(TVOS_BUILD_DIR)
	$(call Q_PROF_GEN,tvos) sed \
		-e "s/@VERSION@/$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)/g" \
		-e "s/@REVISION@/$(IOS_COMMIT_DISTANCE) ($(CURRENT_BRANCH): $(shell git log -1 --pretty=%h))/g" \
		-e "s/@TVOS_SDK_VERSION@/$(TVOS_SDK_VERSION)/g" \
		$< > $@

$(TVOS_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in $(TOP)/Make.config  $(TOP)/.git/HEAD | $(TVOS_BUILD_DIR)
	$(call Q_PROF_GEN,tvos) sed \
		-e 's|@PRODUCT_NAME@|Xamarin.TVOS|g' \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH)|g' \
		-e 's|@PACKAGE_VERSION_MAJOR@|$(IOS_PACKAGE_VERSION_MAJOR)|g' \
		-e 's|@PACKAGE_VERSION_MINOR@|$(IOS_PACKAGE_VERSION_MINOR)|g' \
		-e 's|@PACKAGE_VERSION_REV@|$(IOS_PACKAGE_VERSION_REV)|g' \
		-e 's|@PACKAGE_VERSION_BUILD@|$(IOS_PACKAGE_VERSION_BUILD)|g' \
		$< > $@.tmp
	$(Q) diff $@ $@.tmp >/dev/null 2>&1 || mv -f $@.tmp $@
	$(Q) rm -f $@.tmp
	$(Q) touch $@


$(TVOS_BUILD_DIR)/tvos/core.dll: $(TVOS_CORE_SOURCES) frameworks.sources Makefile | $(TVOS_BUILD_DIR)/tvos
	@mkdir -p $(TVOS_BUILD_DIR)/tvos
	$(call Q_PROF_CSC,tvos) $(TV_CSC) -out:$@ -target:library -debug -unsafe \
		-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		-define:COREBUILD    \
		$(TVOS_DEFINES)     \
		$(TVOS_CORE_SOURCES)

# generator.exe
$(TVOS_BUILD_DIR)/tvos/generator.exe: $(GENERATOR_SOURCES) $(TVOS_BUILD_DIR)/tvos/core.dll
	$(call Q_PROF_CSC,tvos) $(TV_CSC) --keep -out:$@ -debug -unsafe \
		-r:$(TVOS_BUILD_DIR)/tvos/core.dll \
		$(GENERATOR_SOURCES)                  \
		$(GENERATOR_DEFINES)                  \

# generated_sources
$(TVOS_BUILD_DIR)/tvos/generated_sources: $(TVOS_GENERATOR) $(TVOS_APIS) $(TVOS_BUILD_DIR)/tvos/core.dll $(TVOS_BUILD_DIR)/Xamarin.TVOS.BindingAttributes.dll
	$(call Q_PROF_GEN,tvos) $(TVOS_GENERATE) \
		-inline-selectors                                        \
		-process-enums                                           \
		-warnaserror:$(TVOS_GENERATOR_WARNASERROR)               \
		-core                                                    \
		-sourceonly=$@                                           \
		-compiler=$(TV_CSC) \
		-nostdlib -noconfig                                      \
		-no-mono-path                                            \
		-tmpdir=$(TVOS_BUILD_DIR)/tvos                         \
		-baselib=$(TVOS_BUILD_DIR)/tvos/core.dll               \
		-attributelib=$(TVOS_BUILD_DIR)/Xamarin.TVOS.BindingAttributes.dll \
		-r=$(MONO_PATH)/mcs/class/lib/monotouch_tv/System.dll \
		$(TVOS_GENERATED_DEFINES)				\
		-ns=MonoTouch.ObjCRuntime                                \
		-native-exception-marshalling                            \
		--ns:ObjCRuntime                                         \
		$(TVOS_APIS)                                            \
		--target-framework=Xamarin.TVOS,v1.0                     \

$(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS%dll $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS%pdb: $(TVOS_SOURCES) $(TVOS_BUILD_DIR)/tvos/generated_sources $(PRODUCT_KEY_PATH) | $(TVOS_BUILD_DIR)/tvos-64
	$(call Q_PROF_CSC,tvos) $(TV_CSC) -out:$(basename $@).dll -target:library -debug -unsafe -optimize \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) $(TVOS_DEFINES) \
		-r:$(TVOS_LIBDIR)/Mono.Security.dll \
		$(ARGS_64) \
		-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		$(TVOS_SOURCES) @$(TVOS_BUILD_DIR)/tvos/generated_sources

$(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll: $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS.dll | $(TVOS_BUILD_DIR)/reference
	@# Don't strip, btv needs to execute code from Xamarin.TVOS (attributes),
	@# and that'll break if we strip out the code from the reference assembly.
	@#$(Q_GEN) mono-cil-strip $< $@
	$(Q) cp $< $@

$(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.pdb: $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS.pdb | $(TVOS_BUILD_DIR)/reference
	$(Q) cp $< $@

# MonoTouch.NUnitLite
$(TVOS_BUILD_DIR)/reference/MonoTouch.NUnitLite%dll $(TVOS_BUILD_DIR)/reference/MonoTouch.NUnitLite%pdb: $(TVOS_TOUCHUNIT_SOURCES) $(PRODUCT_KEY_PATH) $(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll $(TVOS_BUILD_DIR)/reference/MonoTouch.Dialog-1.dll
	$(call Q_PROF_CSC,tvos) $(SYSTEM_CSC) -r:$(MONOTOUCH_TV_MONO_PATH)/mscorlib.dll -out:$(basename $@).dll -target:library -debug:portable -optimize -noconfig -nostdlib -publicsign \
		-keyfile:$(PRODUCT_KEY_PATH) -r:$(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll -r:$(TVOS_BUILD_DIR)/reference/MonoTouch.Dialog-1.dll -r:$(MONOTOUCH_TV_MONO_PATH)/System.dll -r:$(MONOTOUCH_TV_MONO_PATH)/System.Xml.dll \
		-nowarn:3006,612,649,414,1635 \
		-define:NUNITLITE,CLR_4_0,NET_4_5,__MOBILE__ $(TVOS_DEFINES) \
		$(TVOS_TOUCHUNIT_SOURCES)
	@touch $(basename $@).dll $(basename $@).pdb

$(TVOS_BUILD_DIR)/%.pdb: $(TVOS_BUILD_DIR)/%.dll
	@touch $@

$(TVOS_BUILD_DIR)/reference/MonoTouch.Dialog-1%dll $(TVOS_BUILD_DIR)/reference/MonoTouch.Dialog-1%pdb: $(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll $(TVOS_SOURCES) $(MONOTOUCH_DIALOG_SOURCES) $(PRODUCT_KEY_PATH)
	$(call Q_PROF_CSC,tvos) $(SYSTEM_CSC) -r:$(MONOTOUCH_TV_MONO_PATH)/mscorlib.dll -out:$(basename $@).dll -target:library -debug:portable -optimize -noconfig -nostdlib -publicsign \
		-keyfile:$(PRODUCT_KEY_PATH) -r:$< -r:$(MONOTOUCH_TV_MONO_PATH)/System.dll -r:$(MONOTOUCH_TV_MONO_PATH)/System.Core.dll -r:$(MONOTOUCH_TV_MONO_PATH)/System.Json.dll \
		-define:XAMCORE_2_0 -define:XAMCORE_3_0 -define:TVOS -define:__TVOS__ -define:__UNIFIED__ \
		-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		$(MONOTOUCH_DIALOG_SOURCES) $(MONOTOUCH_DIALOG_RESOURCES)
	@touch $(basename $@).dll $(basename $@).pdb

# System.Drawing.Primitives.dll is special

$(MONO_PATH)/mcs/class/lib/monotouch_tv/reference_Facades/System.Drawing.Primitives.dll: $(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll
	$(call Q_PROF_CSC,tvos) $(MAKE) $(if $(V),,-s) -C $(MONO_PATH)/mcs/class/Facades/System.Drawing.Primitives PROFILE=monotouch_tv LIBRARY_SUBDIR=reference_Facades EXTERNAL_FACADE_DRAWING_REFERENCE="$(abspath $(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll)"
	@touch $@

$(TVOS_BUILD_DIR)/%/Facades/System.Drawing.Primitives.dll: $(MONO_PATH)/mcs/class/lib/monotouch_tv/%_Facades/System.Drawing.Primitives.dll | $(TVOS_BUILD_DIR)/reference/Facades
	$(Q) cp $< $@

# netstandard.dll is special

$(MONO_PATH)/mcs/class/lib/monotouch_tv/reference_Facades/netstandard.dll: $(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll $(TVOS_BUILD_DIR)/reference/OpenTK-1.0.dll
	$(call Q_PROF_CSC,tvos) $(MAKE) $(if $(V),,-s) -C $(MONO_PATH)/mcs/class/Facades/netstandard PROFILE=monotouch_tv LIBRARY_SUBDIR=reference_Facades EXTERNAL_FACADE_DRAWING_REFERENCE="$(abspath $(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll),$(abspath $(TVOS_BUILD_DIR)/reference/OpenTK-1.0.dll)"
	@touch $@

$(TVOS_BUILD_DIR)/%/Facades/netstandard.dll: $(MONO_PATH)/mcs/class/lib/monotouch_tv/%_Facades/netstandard.dll | $(TVOS_BUILD_DIR)/reference/Facades
	$(Q) cp $< $@

# System.Net.Http.dll is special. See comment in src/Makefile

TVOS_EXTRA_SYSTEM_NET_HTTP_FILES = \
	$(abspath $(MONO_PATH)/mcs/class/System.Net.Http/CFContentStream.cs)  \
	$(abspath $(MONO_PATH)/mcs/class/System.Net.Http/CFNetworkHandler.cs) \
	$(abspath $(MONO_PATH)/mcs/class/System.Net.Http/HttpClientEx.cs) \
	$(abspath $(TOP)/src/Foundation/NSUrlSessionHandler.cs) \
	$(abspath $(TOP)/src/ObjCRuntime/RuntimeOptions.cs) \

# build (into custom LIBRARY_SUBDIR)
$(MONO_PATH)/mcs/class/lib/monotouch_tv/reference/System.Net.Http%dll $(MONO_PATH)/mcs/class/lib/monotouch_tv/reference/System.Net.Http%pdb: $(TVOS_EXTRA_SYSTEM_NET_HTTP_FILES) $(MONO_PATH)/mcs/class/lib/monotouch_tv/System.Net.Http.dll $(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll
	$(call Q_PROF_CSC,tvos) $(MAKE) $(if $(V),,-s) -C $(MONO_PATH)/mcs/class/System.Net.Http PROFILE=monotouch_tv LIBRARY_SUBDIR=reference MONOTOUCH_MCS_FLAGS=$(IOS_CSC_FLAGS) EXTRA_LIB_MCS_FLAGS="-r:$(abspath $(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll) $(TVOS_EXTRA_SYSTEM_NET_HTTP_FILES) -D:XAMCORE_2_0 -D:XAMCORE_3_0 -D:XAMARIN_MODERN -D:SYSTEM_NET_HTTP -D:UNIFIED -D:__UNIFIED__"
	@# If the make we executed didn't result in building the dll/pdb (because make determined it wasn't necessary to rebuild),
	@# then this make can end up confused and in an infinite loop. So touch the output files, to make sure they look
	@# rebuilt to make. Unfortunately there's no special variable that means "all targets", $@ only means "target that triggered the rule",
	@# which can be either the dll or the pdb. So add some weird logic to make sure we touch both the dll and the pdb in either case
	@# (we'll touch one of them twice, but that shouldn't be a problem)
	$(Q) touch $@ $(@:.dll=.pdb) $(@:.pdb=.dll)

# sign dll
$(TVOS_BUILD_DIR)/reference/System.Net.Http.dll: $(MONO_PATH)/mcs/class/lib/monotouch_tv/reference/System.Net.Http.dll | $(TVOS_BUILD_DIR)/reference
	$(Q) cp $< $@
	$(call Q_PROF_SN,tvos) MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -R $@ $(PRODUCT_KEY_PATH)

$(TVOS_BUILD_DIR)/reference/System.Net.Http.pdb: $(MONO_PATH)/mcs/class/lib/monotouch_tv/reference/System.Net.Http.pdb | $(TVOS_BUILD_DIR)/reference
	$(Q) cp $< $@

xamtvos.csproj: xamtvos.tmpl.csproj Makefile $(wildcard $(TOP)/*.sources)
	@sed -e 's*<!--%FILES%-->*$(foreach file,$(TVOS_SOURCES),<Compile Include="$(file)"/>)*' -e 's*<!--%APIS%-->*$(foreach file,$(TVOS_APIS),<None Include="$(file)"/>)*' $< | xmllint --format - > $@

MonoTouch.NUnitLite.tvos.csproj: MonoTouch.NUnitLite.tvos.templ.csproj Makefile touch-unit.sources
	$(Q) sed -e 's*<!--%FILES%-->*$(foreach file,$(TVOS_TOUCHUNIT_SOURCES),<Compile Include="$(file)"/>)*' $< | xmllint --format - > $@

MonoTouch.Dialog-1.tvos.csproj: MonoTouch.Dialog-1.tvos.templ.csproj Makefile touch-unit.sources
	$(Q) sed -e 's*<!--%FILES%-->*$(foreach file,$(MONOTOUCH_DIALOG_SOURCES),<Compile Include="$(file)"/>)*' $< | xmllint --format - > $@

PROJECT_FILES += xamtvos.csproj MonoTouch.NUnitLite.tvos.csproj MonoTouch.Dialog-1.tvos.csproj

clean-tvos:
	$(Q) rm -rf $(TVOS_BUILD_DIR)
	$(Q) rm -f $(TVOS_TARGETS)

TVOS_TARGETS_DIRS +=                                          \
	$(TVOS_BUILD_DIR)                                         \
	$(TVOS_BUILD_DIR)/tvos                                    \
	$(TVOS_BUILD_DIR)/tvos-64                                 \
	$(TVOS_BUILD_DIR)/reference                               \
	$(TVOS_BUILD_DIR)/reference/Facades                       \
	$(TVOS_BUILD_DIR)/NativeTypes                             \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS   \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Facades \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/share/doc/Xamarin.TVOS  \

TVOS_TARGETS += \
	xamtvos.csproj                                                                      \
	MonoTouch.NUnitLite.tvos.csproj                                                      \
	MonoTouch.Dialog-1.tvos.csproj                                                       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Xamarin.TVOS.dll             \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Xamarin.TVOS.pdb             \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.TVOS.dll                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.TVOS.pdb                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/System.Net.Http.dll          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/System.Net.Http.pdb          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/OpenTK-1.0.dll               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/OpenTK-1.0.pdb               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/OpenTK-1.0.dll.config        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.Dialog-1.dll       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.Dialog-1.pdb       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.NUnitLite.dll      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.NUnitLite.pdb      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Facades/System.Drawing.Primitives.dll          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Facades/netstandard.dll      \

# reference assemblies, this is just for compilation with XS
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.dll: $(TVOS_BUILD_DIR)/reference/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Facades/%.dll: $(TVOS_BUILD_DIR)/reference/Facades/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Facades
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.pdb: $(TVOS_BUILD_DIR)/reference/%.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.config: $(TVOS_BUILD_DIR)/reference/%.config | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS
	$(Q) install -m 0644 $< $@

# the actual architecture-specific versions
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.TVOS.dll: $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.TVOS.pdb: $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits
	$(Q) install -m 0644 $< $@

$(TVOS_TARGETS_DIRS):
	$(Q) mkdir -p $@

all-tvos: $(TVOS_TARGETS)
install-tvos: $(TVOS_TARGETS)

ifdef INCLUDE_TVOS
ALL_TARGETS += all-tvos
INSTALL_TARGETS += install-tvos
endif

#
# Global targets
#

SHARED_PATH := ../runtime
$(SHARED_PATH)/Delegates.generated.cs: $(SHARED_PATH)/Delegates.cs.t4 $(SHARED_PATH)/delegates.t4
	$(Q) $(MAKE) -C $(SHARED_PATH) Delegates.generated.cs

$(BUILD_DIR)/common/NativeTypes/%.cs: $(TOP)/src/NativeTypes/%.tt | $(BUILD_DIR)/common/NativeTypes
	$(Q_GEN) $(TT) $< -o $@ 1>/dev/null

$(COMMON_TARGET_DIRS):
	$(Q) mkdir -p $@

install-local:: $(INSTALL_TARGETS)
all-local:: $(ALL_TARGETS)

project-files: $(PROJECT_FILES)

.SECONDARY:
