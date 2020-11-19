TOP=..
include $(TOP)/Make.config

BUILD_DIR=build
DOTNET_BUILD_DIR=$(BUILD_DIR)/dotnet
PROJECT_DIR=.

include $(TOP)/src/frameworks.sources
include $(TOP)/mk/rules.mk

include $(TOP)/src/touch-unit.sources

export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

MAC_BUILD_DIR=$(BUILD_DIR)/mac
IOS_BUILD_DIR=$(BUILD_DIR)/ios
WATCH_BUILD_DIR=$(BUILD_DIR)/watch
TVOS_BUILD_DIR=$(BUILD_DIR)/tvos

MACOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/macos
IOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/ios
WATCHOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/watchos
TVOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/tvos

GENERATOR_FLAGS=-process-enums -core -nologo -nostdlib -noconfig -native-exception-marshalling --ns:ObjCRuntime

DOTNET_REFERENCES = \
	/r:$(DOTNET5_BCL_DIR)/System.Buffers.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Collections.Concurrent.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Collections.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Collections.NonGeneric.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Console.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Diagnostics.Debug.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Diagnostics.Tools.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Drawing.Primitives.dll \
	/r:$(DOTNET5_BCL_DIR)/System.IO.Compression.dll \
	/r:$(DOTNET5_BCL_DIR)/System.IO.FileSystem.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Linq.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Memory.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Net.Http.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Net.NameResolution.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Net.Primitives.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Net.Requests.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Net.Security.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Net.ServicePoint.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Net.Sockets.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Resources.ResourceManager.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Runtime.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Runtime.Extensions.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Runtime.InteropServices.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Security.Cryptography.X509Certificates.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Text.RegularExpressions.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Threading.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Threading.Tasks.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Threading.Thread.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Xml.dll \
	/r:$(DOTNET5_BCL_DIR)/System.Xml.ReaderWriter.dll \

DOTNET_FLAGS=/noconfig /nostdlib+ /deterministic /features:strict /nologo /target:library /debug /unsafe /define:NET /define:NET_TODO $(DOTNET_REFERENCES)

DOTNET_COMPILER=$(DOTNET_BUILD_DIR)/compiler
DOTNET_GENERATOR_FLAGS=$(GENERATOR_FLAGS) -compiler=$(abspath $(DOTNET_COMPILER)) --lib=$(DOTNET5_BCL_DIR) -attributelib:$(DOTNET_BINDING_ATTRIBUTES) $(DOTNET_REFERENCES)
DOTNET_GENERATOR=$(DOTNET_BUILD_DIR)/bgen/bgen
DOTNET_BINDING_ATTRIBUTES=$(DOTNET_BUILD_DIR)/Xamarin.Apple.BindingAttributes.dll

include $(TOP)/opentk/Makefile.include

#
# Specific warnings that we want reported as errors by generator
# It takes a comma separated list, but an empty list means
# reporting all warnings as errors.
#
# https://cezarypiatek.github.io/post/non-nullable-references-in-dotnet-core/
NULLABILITY_WARNINGS=8073,8597,8600,8601,8602,8603,8604,8605,8606,8607,8608,8609,8610,8610,87611,8612,8613,8614,8615,8616,8617,8618,8619,8620,8621,8622,8624,8625,8626,8629,8631,8632,8633,8634,8638,8343,8644,8645,8653,8654,8655,8667,8714
GENERATOR_WARNASERROR=$(NULLABILITY_WARNINGS)
IOS_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
WATCH_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
TVOS_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
MAC_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)

#
# This is the "you are redefining the member", probably we should remove
# those from the contract
#
IOS_WARNINGS_THAT_YOU_SHOULD_FIX=108

include ./OpenGLES/Makefile-1.0.include
include ./Makefile.generator

include ./generator-diff.mk

COMMON_TARGET_DIRS = \
	$(BUILD_DIR)/common             \
	$(BUILD_DIR)/common/NativeTypes \

ARGS_32 = -define:ARCH_32
ARGS_64 = -define:ARCH_64

SHARED_SYSTEM_DRAWING_SOURCES = System.Drawing/PointSizeRectangleTypeForwarders.cs
SHARED_RESX = $(TOP)/tools/mtouch/Errors.resx
SHARED_DESIGNER_CS = $(BUILD_DIR)/common/Errors.Designer.cs

$(SHARED_DESIGNER_CS): $(SHARED_RESX)
	./generate-errors.csharp > $(SHARED_DESIGNER_CS)

#
# Xamarin.iOS
#

# Add new bindings + source files in frameworks.sources, not here.

IOS_EXTRA_SOURCES = \
	$(IOS_OPENTK_1_0_CORE_SOURCES)   \
	$(IOS_BUILD_DIR)/Constants.cs    \
	$(IOS_BUILD_DIR)/AssemblyInfo.cs \
	$(SHARED_DESIGNER_CS) \
	$(SHARED_SYSTEM_DRAWING_SOURCES) \

IOS_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \

IOS_CORE_SOURCES += $(IOS_EXTRA_SOURCES)
IOS_SOURCES += $(IOS_EXTRA_SOURCES) $(IOS_HTTP_SOURCES)

IOS_GENERATOR_FLAGS = -inline-selectors -d:IOS -process-enums -warnaserror:$(IOS_GENERATOR_WARNASERROR)
IOS_DEFINES = -define:IPHONE -define:IOS -define:MONOTOUCH -d:__IOS__ $(APPLETLS_DEFINES) -d:SYSTEM_NET_HTTP
IOS_GENERATOR=$(BUILD_DIR)/common/bgen.exe
IOS_GENERATE=$(SYSTEM_MONO) --debug $(IOS_GENERATOR)

IOS_CORE_WARNINGS_TO_FIX=-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX)
IOS_CORE_DEFINES=-define:COREBUILD $(IOS_DEFINES) $(IOS_native_DEFINES)

$(IOS_BUILD_DIR)/Constants.cs: Constants.iOS.cs.in Makefile $(TOP)/Make.config.inc | $(IOS_BUILD_DIR)
	$(call Q_PROF_GEN,ios) sed \
		-e "s/@VERSION@/$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)/g" \
		-e 's/@REVISION@/$(IOS_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h)/g' \
		-e "s/@IOS_SDK_VERSION@/$(IOS_SDK_VERSION)/g" \
		$< > $@

$(IOS_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in
	$(Q) mkdir -p $(IOS_BUILD_DIR)
	$(call Q_PROF_GEN,ios) sed < $< > $@ 's|@PRODUCT_NAME@|$(IOS_PRODUCT)|g; s|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g; s|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g; s|@PACKAGE_VERSION_MAJOR@|$(IOS_PACKAGE_VERSION_MAJOR)|g; s|@PACKAGE_VERSION_MINOR@|$(IOS_PACKAGE_VERSION_MINOR)|g; s|@PACKAGE_VERSION_REV@|$(IOS_PACKAGE_VERSION_REV)|g; s|@PACKAGE_VERSION_BUILD@|$(IOS_PACKAGE_VERSION_BUILD)|g'

# core.dll
$(IOS_BUILD_DIR)/native/core.dll: $(IOS_CORE_SOURCES) frameworks.sources
	$(Q) mkdir -p $(IOS_BUILD_DIR)native
	$(call Q_PROF_CSC,ios) $(IOS_CSC) -nologo -out:$@ -target:library -debug -unsafe \
		-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		-define:COREBUILD $(IOS_DEFINES) \
		$(IOS_CORE_SOURCES)

# generated_sources
$(IOS_BUILD_DIR)/native/generated_sources: $(IOS_GENERATOR) $(IOS_APIS) $(IOS_BUILD_DIR)/native/core.dll $(IOS_BUILD_DIR)/native/Xamarin.iOS.BindingAttributes.dll $(BUILD_DIR)/ios.rsp
	$(call Q_PROF_GEN,ios) $(IOS_GENERATE) @$(BUILD_DIR)/ios.rsp

### .NET ###

$(IOS_DOTNET_BUILD_DIR)/core-ios.dll: $(IOS_CORE_SOURCES) frameworks.sources | $(IOS_DOTNET_BUILD_DIR)
	$(Q_DOTNET_GEN) \
		$(SYSTEM_CSC) $(DOTNET_FLAGS) \
		$(IOS_CORE_WARNINGS_TO_FIX) \
		$(IOS_CORE_DEFINES) \
		$(IOS_CORE_SOURCES) \
		-out:$@

$(IOS_DOTNET_BUILD_DIR)/ios-generated-sources: $(DOTNET_GENERATOR) $(IOS_APIS) $(IOS_DOTNET_BUILD_DIR)/core-ios.dll $(DOTNET_BINDING_ATTRIBUTES) $(IOS_DOTNET_BUILD_DIR)/ios.rsp | $(IOS_DOTNET_BUILD_DIR)/generated-sources
	$(Q_DOTNET_GEN) $< @$(IOS_DOTNET_BUILD_DIR)/ios.rsp

$(BUILD_DIR)/ios.rsp: Makefile Makefile.generator frameworks.sources
	$(Q_GEN) echo \
		$(IOS_GENERATOR_FLAGS) \
		-core \
		-sourceonly=$(IOS_BUILD_DIR)/native/generated_sources \
		-compiler=$(IOS_CSC) \
		-nologo -nostdlib -noconfig \
		-tmpdir=$(IOS_BUILD_DIR)/native \
		-baselib=$(IOS_BUILD_DIR)/native/core.dll \
		-attributelib=$(IOS_BUILD_DIR)/native/Xamarin.iOS.BindingAttributes.dll \
		-native-exception-marshalling \
		--ns=ObjCRuntime \
		--target-framework=Xamarin.iOS,v1.0 \
		$(IOS_APIS) \
		> $@

$(IOS_DOTNET_BUILD_DIR)/ios.rsp: Makefile Makefile.generator frameworks.sources $(DOTNET_COMPILER) | $(IOS_DOTNET_BUILD_DIR)
	$(Q) echo \
		$(IOS_GENERATOR_FLAGS) \
		$(DOTNET_GENERATOR_FLAGS) -inline-selectors \
		-warnaserror:$(IOS_GENERATOR_WARNASERROR) \
		-sourceonly=$(IOS_DOTNET_BUILD_DIR)/ios-generated-sources \
		-tmpdir=$(IOS_DOTNET_BUILD_DIR)/generated-sources \
		-baselib=$(IOS_DOTNET_BUILD_DIR)/core-ios.dll \
		--target-framework=.NETCoreApp,Version=6.0,Profile=ios \
		$(IOS_APIS) \
		> $@

define IOS_TARGETS_template
# Xamarin.iOS.dll
$(IOS_BUILD_DIR)/native-$(1)%Xamarin.iOS.dll $(IOS_BUILD_DIR)/native-$(1)%Xamarin.iOS.pdb: $$(IOS_SOURCES) $(IOS_BUILD_DIR)/native/generated_sources $(PRODUCT_KEY_PATH)
	@mkdir -p $(IOS_BUILD_DIR)/native-$(1)
	$$(call Q_PROF_CSC,ios/$(1) bit) $$(IOS_CSC) -nologo -out:$$(basename $$@).dll -target:library -debug -unsafe -optimize \
		-deterministic \
		$$(ARGS_$(1)) \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) $$(IOS_DEFINES) \
		-nowarn:219,618,114,414,1635,3021,$$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		-warnaserror:$(NULLABILITY_WARNINGS) \
		$$(IOS_CSC_FLAGS_XI) \
		$$(IOS_SOURCES) @$(IOS_BUILD_DIR)/native/generated_sources

$(IOS_DOTNET_BUILD_DIR)/$(1)/Xamarin.iOS%dll $(IOS_DOTNET_BUILD_DIR)/$(1)/Xamarin.iOS%pdb $(2): $$(IOS_SOURCES) $$(IOS_DOTNET_BUILD_DIR)/ios-generated-sources $(PRODUCT_KEY_PATH) | $(IOS_DOTNET_BUILD_DIR)/$(1) $(IOS_DOTNET_BUILD_DIR)/ref
	$$(call Q_PROF_CSC,dotnet/$(1)-bit) $(SYSTEM_CSC) $(DOTNET_FLAGS) -out:$(IOS_DOTNET_BUILD_DIR)/$(1)/Xamarin.iOS.dll -unsafe -optimize \
		$$(ARGS_$(1)) \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) \
		$(3) \
		$$(IOS_DEFINES) \
		$$(IOS_native_DEFINES) \
		$$(IOS_CORE_WARNINGS_TO_FIX) \
		$$(IOS_CSC_FLAGS_XI) \
		$$(IOS_SOURCES) @$(IOS_DOTNET_BUILD_DIR)/ios-generated-sources

endef

$(IOS_BUILD_DIR)/compat/%: $(MACIOS_BINARIES_PATH)/% | $(IOS_BUILD_DIR)/compat
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/%: $(IOS_BUILD_DIR)/compat/%
	$(Q) $(CP) $< $@

$(eval $(call IOS_TARGETS_template,32))
$(eval $(call IOS_TARGETS_template,64,$(IOS_DOTNET_BUILD_DIR)/ref/Xamarin.iOS%dll,-refout:$(IOS_DOTNET_BUILD_DIR)/ref/Xamarin.iOS.dll))

# MonoTouch.Dialog-1
$(IOS_BUILD_DIR)/reference/MonoTouch.Dialog-1.dll: $(MACIOS_BINARIES_PATH)/MonoTouch.Dialog-Unified/ios/MonoTouch.Dialog-1.dll | $(IOS_BUILD_DIR)/reference
	$(Q) $(CP) $< $@

$(IOS_BUILD_DIR)/reference/MonoTouch.Dialog-1.pdb: $(MACIOS_BINARIES_PATH)/MonoTouch.Dialog-Unified/ios/MonoTouch.Dialog-1.pdb | $(IOS_BUILD_DIR)/reference
	$(Q) $(CP) $< $@

# MonoTouch.NUnitLite
$(IOS_BUILD_DIR)/reference%MonoTouch.NUnitLite.dll $(IOS_BUILD_DIR)/reference%MonoTouch.NUnitLite.pdb: $(IOS_TOUCHUNIT_SOURCES) $(IOS_BUILD_DIR)/reference/MonoTouch.Dialog-1.dll $(PRODUCT_KEY_PATH) $(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll
	$(call Q_PROF_CSC,ios) $(IOS_CSC) -nologo -out:$(basename $@).dll -target:library -debug:portable -optimize -publicsign \
		-deterministic \
		-keyfile:$(PRODUCT_KEY_PATH) -r:$(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll -define:MONO -r:$(IOS_BUILD_DIR)/reference/MonoTouch.Dialog-1.dll -r:$(MONOTOUCH_MONO_PATH)/System.dll -r:$(MONOTOUCH_MONO_PATH)/System.Xml.dll \
		-r:$(MONOTOUCH_MONO_PATH)/System.Core.dll \
		-nowarn:3006,612,649,414,1635,659 \
		-define:NUNITLITE,CLR_4_0,NET_4_5,__MOBILE__,MONO $(IOS_DEFINES) \
		-define:XAMCORE_2_0,__UNIFIED__ \
		$(IOS_TOUCHUNIT_SOURCES)

$(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll: $(IOS_BUILD_DIR)/native-64/Xamarin.iOS.dll | $(IOS_BUILD_DIR)/reference
	$(Q_STRIP) mono-cil-strip -q $< $@

$(IOS_BUILD_DIR)/reference/Xamarin.iOS.pdb: $(IOS_BUILD_DIR)/native-64/Xamarin.iOS.pdb | $(IOS_BUILD_DIR)/reference
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/% : $(MACIOS_BINARIES_PATH)/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/Facades/% : $(MACIOS_BINARIES_PATH)/Facades/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/Facades
	$(Q) $(CP) $< $@

$(IOS_BUILD_DIR)/compat/%: $(MACIOS_BINARIES_PATH)/% | $(IOS_BUILD_DIR)/compat
	$(Q) $(CP) $< $@

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
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/Facades     \

IOS_TARGETS += \
	$(PROJECT_DIR)/xamios.csproj                                                   \
	$(PROJECT_DIR)/MonoTouch.NUnitLite.csproj                                      \
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
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.dll                   \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.pdb               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.dll                   \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.pdb               \

DOTNET_TARGETS += \
	$(IOS_DOTNET_BUILD_DIR)/32/Xamarin.iOS.dll \
	$(IOS_DOTNET_BUILD_DIR)/64/Xamarin.iOS.dll \
	$(IOS_DOTNET_BUILD_DIR)/ref/Xamarin.iOS.dll \
	$(DOTNET_DESTDIR)/$(IOS_NUGET).Ref/ref/net6.0/Xamarin.iOS.dll \
	$(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(IOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.iOS.dll) \
	$(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(IOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.iOS.pdb) \

DOTNET_TARGETS_DIRS += \
	$(IOS_DOTNET_BUILD_DIR) \
	$(IOS_DOTNET_BUILD_DIR)/32 \
	$(IOS_DOTNET_BUILD_DIR)/64 \
	$(IOS_DOTNET_BUILD_DIR)/ref \
	$(IOS_DOTNET_BUILD_DIR)/generated-sources \
	$(DOTNET_DESTDIR)/$(IOS_NUGET).Ref/ref/net6.0 \
	$(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(IOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0) \

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/%.dll: $(IOS_BUILD_DIR)/compat/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1 $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/Facades
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
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/%.dll: $(IOS_BUILD_DIR)/reference/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Facades
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/%.pdb: $(IOS_BUILD_DIR)/reference/%.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/%.config: $(IOS_BUILD_DIR)/reference/%.config | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS
	$(Q) install -m 0644 $< $@

$(DOTNET_DESTDIR)/$(IOS_NUGET).Ref/ref/net6.0/Xamarin.iOS.dll: $(IOS_DOTNET_BUILD_DIR)/ref/Xamarin.iOS.dll | $(DOTNET_DESTDIR)/$(IOS_NUGET).Ref/ref/net6.0
	$(Q) $(CP) $< $@

# the actual architecture-specific versions
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.dll: $(IOS_BUILD_DIR)/native-32/Xamarin.iOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.pdb: $(IOS_BUILD_DIR)/native-32/Xamarin.iOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.dll: $(IOS_BUILD_DIR)/native-64/Xamarin.iOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.pdb: $(IOS_BUILD_DIR)/native-64/Xamarin.iOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits
	$(Q) install -m 0644 $< $@

$(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS_32),$(DOTNET_DESTDIR)/$(IOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.iOS.dll): $(IOS_DOTNET_BUILD_DIR)/32/Xamarin.iOS.dll | $(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS_32),$(DOTNET_DESTDIR)/$(IOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0)
	$(Q) $(CP) $< $@

$(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS_32),$(DOTNET_DESTDIR)/$(IOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.iOS.pdb): $(IOS_DOTNET_BUILD_DIR)/32/Xamarin.iOS.pdb | $(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS_32),$(DOTNET_DESTDIR)/$(IOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0)
	$(Q) $(CP) $< $@

$(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS_64),$(DOTNET_DESTDIR)/$(IOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.iOS.dll): $(IOS_DOTNET_BUILD_DIR)/64/Xamarin.iOS.dll | $(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS_64),$(DOTNET_DESTDIR)/$(IOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0)
	$(Q) $(CP) $< $@

$(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS_64),$(DOTNET_DESTDIR)/$(IOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.iOS.pdb): $(IOS_DOTNET_BUILD_DIR)/64/Xamarin.iOS.pdb | $(foreach rid,$(DOTNET_IOS_RUNTIME_IDENTIFIERS_64),$(DOTNET_DESTDIR)/$(IOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0)
	$(Q) $(CP) $< $@

$(IOS_TARGETS_DIRS):
	$(Q) mkdir -p $@

$(PROJECT_DIR)/xamios.csproj: xamios.tmpl.csproj Makefile $(wildcard $(TOP)/src/*.sources)
	@sed -e 's*<!--%FILES%-->*$(foreach file,$(IOS_SOURCES),<Compile Include="$(file)"/>)*' -e 's*<!--%APIS%-->*$(foreach file,$(IOS_APIS),<None Include="$(file)"/>)*' $< | xmllint --format - > $@

$(PROJECT_DIR)/MonoTouch.NUnitLite.csproj: MonoTouch.NUnitLite.templ.csproj Makefile touch-unit.sources $(TOP)/Make.config $(TOP)/mk/mono.mk
	$(Q) sed -e 's*<!--%FILES%-->*$(foreach file,$(IOS_TOUCHUNIT_SOURCES),<Compile Include="$(file)"/>)*' $< | xmllint --format - > $@

PROJECT_FILES += $(PROJECT_DIR)/xamios.csproj $(PROJECT_DIR)/MonoTouch.NUnitLite.csproj

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

MAC_COMMON_DEFINES = -define:MONOMAC
MAC_full_ARGS = -define:NO_SYSTEM_DRAWING -define:XAMMAC_SYSTEM_MONO
MAC_mobile_ARGS = 
MAC_BOOTSTRAP_DEFINES = $(MAC_COMMON_DEFINES),COREBUILD
MAC_GENERATED_DEFINES = -d:MONOMAC

MAC_CORE_DEFINES = $(MAC_BOOTSTRAP_DEFINES)
MAC_CORE_WARNINGS_TO_FIX = -nowarn:3021,612,618,1635

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
	Darwin/KernelNotification.cs \
	Darwin/SystemLog.cs \
	$(SHARED_DESIGNER_CS) \

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

MAC_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \

$(MAC_BUILD_DIR)/Constants.cs: Constants.mac.cs.in Makefile $(TOP)/Make.config.inc | $(MAC_BUILD_DIR)
	$(Q) sed \
			-e "s/@VERSION@/$(MAC_PACKAGE_VERSION_MAJOR).$(MAC_PACKAGE_VERSION_MINOR).$(MAC_PACKAGE_VERSION_REV)/g" \
			-e 's/@REVISION@/$(MAC_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h)/g' \
			-e "s/@OSX_SDK_VERSION@/$(OSX_SDK_VERSION)/g" \
			-e "s/@MIN_XM_MONO_VERSION@/$(MIN_XM_MONO_VERSION)/g" \
		$< > $@

$(PROJECT_DIR)/xammac.csproj: xammac.tmpl.csproj Makefile $(wildcard $(TOP)/src/*.sources)
	@sed -e 's*<!--%FILES%-->*$(foreach file,$(MAC_SOURCES) $(SHARED_SYSTEM_DRAWING_SOURCES),<Compile Include="$(file)"/>)*' -e 's*<!--%APIS%-->*$(foreach file,$(MAC_APIS),<None Include="$(file)"/>)*' $< | xmllint --format - > $@

PROJECT_FILES += $(PROJECT_DIR)/xammac.csproj

$(MAC_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in
	@mkdir -p $(MAC_BUILD_DIR)
	$(call Q_PROF_GEN,mac) sed < $< > $@ 's|@PRODUCT_NAME@|$(MAC_PRODUCT)|g; s|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g; s|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g; s|@PACKAGE_VERSION_MAJOR@|$(MAC_PACKAGE_VERSION_MAJOR)|g; s|@PACKAGE_VERSION_MINOR@|$(MAC_PACKAGE_VERSION_MINOR)|g; s|@PACKAGE_VERSION_REV@|$(MAC_PACKAGE_VERSION_REV)|g; s|@PACKAGE_VERSION_BUILD@|$(MAC_PACKAGE_VERSION_BUILD)|g'

# We can't pass the --target-framework values as parameters to the templates, because the commas interfere with Make's parameter parsing.
xm_full_profile=--target-framework=Xamarin.Mac,Version=v4.5,Profile=Full
xm_mobile_profile=--target-framework=Xamarin.Mac,Version=v2.0,Profile=Mobile

MAC_GENERATOR=$(BUILD_DIR)/common/bgen.exe
MAC_GENERATE=$(SYSTEM_MONO) --debug $(MAC_GENERATOR)

define MAC_GENERATOR_template
$(MAC_BUILD_DIR)/$(1)/core.dll: $(MAC_CORE_SOURCES) frameworks.sources
	@mkdir -p $(MAC_BUILD_DIR)/$(1)
	$$(call Q_PROF_CSC,mac/$(1)) \
		$$(MAC_$(1)_CSC) -nologo -out:$$@ -target:library -debug -unsafe -nowarn:3021,612,618,1635 \
		$$(MAC_BOOTSTRAP_DEFINES) \
		$(2) \
		$$(MAC_CORE_SOURCES)

$(MAC_BUILD_DIR)/$(1)/generated-sources: $$(MAC_GENERATOR) $(MAC_APIS) $(MAC_BUILD_DIR)/$(1)/core.dll $(MAC_BUILD_DIR)/Xamarin.Mac-$(1).BindingAttributes.dll $(BUILD_DIR)/mac-$(1).rsp
	$$(call Q_PROF_GEN,mac/$(1)) $$(MAC_GENERATE) @$(BUILD_DIR)/mac-$(1).rsp

$(BUILD_DIR)/mac-$(1).rsp: Makefile Makefile.generator frameworks.sources
	$$(Q_GEN) echo \
		$(MAC_GENERATED_DEFINES) \
		-compiler:$$(MAC_$(1)_CSC) \
		-nologo \
		-process-enums \
		-warnaserror:$(MAC_GENERATOR_WARNASERROR) \
		-native-exception-marshalling \
		-core \
		-sourceonly:$(MAC_BUILD_DIR)/$(1)/generated-sources \
		-tmpdir:$(MAC_BUILD_DIR)/$(1) \
		-baselib:$(MAC_BUILD_DIR)/$(1)/core.dll \
		-attributelib:$(MAC_BUILD_DIR)/Xamarin.Mac-$(1).BindingAttributes.dll \
		-d:NO_SYSTEM_DRAWING \
		--ns=ObjCRuntime \
		$(2) \
		$(xm_$(1)_profile) \
		$(MAC_APIS) \
		> $$@
endef

$(eval $(call MAC_GENERATOR_template,full,-d:NO_SYSTEM_DRAWING))
$(eval $(call MAC_GENERATOR_template,mobile,$(SHARED_SYSTEM_DRAWING_SOURCES)))

define MAC_TARGETS_template
$(MAC_BUILD_DIR)/$(1)-64/Xamarin.Mac%dll $(MAC_BUILD_DIR)/$(1)-64/Xamarin.Mac%pdb: $(MAC_BUILD_DIR)/$(1)/generated-sources $(MAC_SOURCES) $(MAC_CFNETWORK_SOURCES) $(MAC_CLASSIC_SOURCES) $(SN_KEY)
	@mkdir -p $(MAC_BUILD_DIR)/$(1)-64
	$$(call Q_PROF_CSC,mac/$(1)-64) \
		$$(MAC_$(1)_CSC) -nologo -out:$$(basename $$@).dll -target:library -debug -unsafe \
		-deterministic \
		$$(MAC_COMMON_DEFINES) \
		$$(MAC_$(1)_ARGS) \
		$$(ARGS_64) \
		-publicsign -keyfile:$(SN_KEY) \
		-nowarn:3021,1635,612,618,0219,0414,$(MAC_WARNINGS_TO_FIX) \
		-warnaserror:$(NULLABILITY_WARNINGS) \
		$$(MAC_CSC_FLAGS_XM) \
		$(MAC_CFNETWORK_SOURCES) $(MAC_HTTP_SOURCES) \
		$(2) \
		$$(MAC_SOURCES) \
		@$$<

endef

$(eval $(call MAC_TARGETS_template,mobile,$(SHARED_SYSTEM_DRAWING_SOURCES) $(APPLETLS_DEFINES)))
$(eval $(call MAC_TARGETS_template,full,))

$(MAC_BUILD_DIR)/%-reference/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/%-64/Xamarin.Mac.dll
	@mkdir -p $(@D)
	$(Q) $(CP) $^ $@

$(MAC_BUILD_DIR)/%-reference/Xamarin.Mac.pdb: $(MAC_BUILD_DIR)/%-64/Xamarin.Mac.pdb
	@mkdir -p $(@D)
	$(Q) $(CP) $^ $@

$(DOTNET_DESTDIR)/$(MACOS_NUGET).Ref/ref/net6.0/Xamarin.Mac.dll: $(MACOS_DOTNET_BUILD_DIR)/ref/Xamarin.Mac.dll | $(DOTNET_DESTDIR)/$(MACOS_NUGET).Ref/ref/net6.0
	$(Q) $(CP) $< $@

### .NET ###

$(MACOS_DOTNET_BUILD_DIR)/core-macos.dll: $(MAC_CORE_SOURCES) frameworks.sources | $(MACOS_DOTNET_BUILD_DIR)
	$(Q_DOTNET_GEN) \
		$(SYSTEM_CSC) $(DOTNET_FLAGS) -out:$@ \
		$(MAC_CORE_WARNINGS_TO_FIX) \
		$(MAC_CORE_DEFINES) \
		$(MAC_CORE_SOURCES) \
		-out:$@ \

$(MACOS_DOTNET_BUILD_DIR)/macos-generated-sources: $(DOTNET_GENERATOR) $(MAC_APIS) $(MACOS_DOTNET_BUILD_DIR)/core-macos.dll $(DOTNET_BINDING_ATTRIBUTES) $(MACOS_DOTNET_BUILD_DIR)/macos.rsp | $(MACOS_DOTNET_BUILD_DIR)/generated-sources
	$(Q_DOTNET_GEN) $< @$(MACOS_DOTNET_BUILD_DIR)/macos.rsp

$(MACOS_DOTNET_BUILD_DIR)/macos.rsp: Makefile Makefile.generator frameworks.sources $(DOTNET_COMPILER) | $(MACOS_DOTNET_BUILD_DIR)
	$(Q) echo \
		$(MAC_GENERATED_DEFINES) \
		$(DOTNET_GENERATOR_FLAGS) \
		-warnaserror:$(MAC_GENERATOR_WARNASERROR) \
		-sourceonly:$(MACOS_DOTNET_BUILD_DIR)/macos-generated-sources \
		-tmpdir:$(MACOS_DOTNET_BUILD_DIR)/generated-sources \
		-baselib:$(MACOS_DOTNET_BUILD_DIR)/core-macos.dll \
		--target-framework=.NETCoreApp,Version=6.0,Profile=macos \
		$(MAC_APIS) \
		> $@

$(MACOS_DOTNET_BUILD_DIR)/64/Xamarin.Mac%dll $(MACOS_DOTNET_BUILD_DIR)/64/Xamarin.Mac%pdb $(MACOS_DOTNET_BUILD_DIR)/ref/Xamarin.Mac%dll: $(MACOS_DOTNET_BUILD_DIR)/macos-generated-sources $(MAC_SOURCES) $(MAC_CFNETWORK_SOURCES) $(SN_KEY) | $(MACOS_DOTNET_BUILD_DIR)/64 $(MACOS_DOTNET_BUILD_DIR)/ref
	$(Q_DOTNET_BUILD) \
		$(SYSTEM_CSC) $(DOTNET_FLAGS) -out:$(MACOS_DOTNET_BUILD_DIR)/64/Xamarin.Mac.dll -optimize \
		-publicsign -keyfile:$(SN_KEY) \
		-refout:$(MACOS_DOTNET_BUILD_DIR)/ref/Xamarin.Mac.dll \
		$(MAC_COMMON_DEFINES) \
		$(ARGS_64) \
		-nowarn:3021,1635,612,618,0219,0414,$(MAC_WARNINGS_TO_FIX) \
		$(MAC_CSC_FLAGS_XM) \
		$(MAC_CFNETWORK_SOURCES) $(MAC_HTTP_SOURCES) $(SHARED_SYSTEM_DRAWING_SOURCES) $(APPLETLS_DEFINES) -D:XAMARIN_MODERN \
		$(MAC_SOURCES) \
		@$<

DOTNET_TARGETS += \
	$(MACOS_DOTNET_BUILD_DIR)/64/Xamarin.Mac.dll \
	$(MACOS_DOTNET_BUILD_DIR)/ref/Xamarin.Mac.dll \
	$(DOTNET_DESTDIR)/$(MACOS_NUGET).Ref/ref/net6.0/Xamarin.Mac.dll \
	$(foreach rid,$(DOTNET_MACOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(MACOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.Mac.dll) \
	$(foreach rid,$(DOTNET_MACOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(MACOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.Mac.pdb) \

DOTNET_TARGETS_DIRS += \
	$(MACOS_DOTNET_BUILD_DIR) \
	$(MACOS_DOTNET_BUILD_DIR)/generated-sources \
	$(MACOS_DOTNET_BUILD_DIR)/64 \
	$(MACOS_DOTNET_BUILD_DIR)/ref \
	$(DOTNET_DESTDIR)/$(MACOS_NUGET).Ref/ref/net6.0 \
	$(foreach rid,$(DOTNET_MACOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(MACOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0) \

MAC_VARIANTS_TARGETS = \
	$(MAC_BUILD_DIR)/mobile-64/Xamarin.Mac.dll \
	$(MAC_BUILD_DIR)/full-64/Xamarin.Mac.dll \
	$(MAC_BUILD_DIR)/mobile-reference/Xamarin.Mac.dll \
	$(MAC_BUILD_DIR)/full-reference/Xamarin.Mac.dll \

INSTALL_TARGETS+=install-mac
ALL_TARGETS+=all-mac

MAC_TARGETS_DIRS += \
	$(MAC_BUILD_DIR) \
	$(MAC_BUILD_DIR)/mobile \
	$(MAC_BUILD_DIR)/mobile/Facades \
	$(MAC_BUILD_DIR)/full \
	$(MAC_BUILD_DIR)/compat \
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
	$(PROJECT_DIR)/xammac.csproj                                                        \
	$(MAC_VARIANTS_TARGETS)                                                             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMac.dll                      \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.dll     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.pdb     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.dll       \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.pdb       \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile/Xamarin.Mac.dll        \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile/Xamarin.Mac.pdb        \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full/Xamarin.Mac.dll          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full/Xamarin.Mac.pdb          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.dll     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.pdb     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.pdb             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig/xammac.pc

# The XamMac.dll file must exist for VSfM to be able to open XM/Classic projects (so that people can use the migration wizard)
$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMac.dll: $(MACIOS_BINARIES_PATH)/XamMac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/mobile-reference/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.pdb: $(MAC_BUILD_DIR)/mobile-reference/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/mobile-64/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile/Xamarin.Mac.pdb: $(MAC_BUILD_DIR)/mobile-64/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/full-reference/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.pdb : $(MAC_BUILD_DIR)/full-reference/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full/Xamarin.Mac.dll: $(MAC_BUILD_DIR)/full-64/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full/Xamarin.Mac.pdb: $(MAC_BUILD_DIR)/full-64/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.dll $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.pdb: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac
	$(Q) ln -sF ../../reference/mobile/$(@F) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.pdb: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5
	$(Q) ln -sF ../../reference/full/$(@F) $@

$(foreach rid,$(DOTNET_MACOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(MACOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.Mac.dll): $(MACOS_DOTNET_BUILD_DIR)/64/Xamarin.Mac.dll | $(foreach rid,$(DOTNET_MACOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(MACOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0)
	$(Q) $(CP) $< $@

$(foreach rid,$(DOTNET_MACOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(MACOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.Mac.pdb): $(MACOS_DOTNET_BUILD_DIR)/64/Xamarin.Mac.pdb | $(foreach rid,$(DOTNET_MACOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(MACOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0)
	$(Q) $(CP) $< $@

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

WATCH_DEFINES = -define:IPHONE -define:MONOTOUCH -d:WATCH -d:XAMCORE_3_0 -d:__WATCHOS__ -d:SYSTEM_NET_HTTP
WATCH_GENERATOR=$(BUILD_DIR)/common/bgen.exe
WATCH_GENERATE=$(SYSTEM_MONO) --debug $(WATCH_GENERATOR)
WATCH_GENERATED_DEFINES= -d:WATCH -d:XAMCORE_3_0

WATCHOS_CORE_DEFINES = $(WATCH_DEFINES) -define:COREBUILD
WATCHOS_CORE_WARNINGS_TO_FIX = -nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX)

WATCHOS_WARNINGS_TO_FIX = -nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX)

WATCHOS_EXTRA_CORE_SOURCES = \
    $(WATCH_BUILD_DIR)/Constants.cs    \
    $(WATCH_BUILD_DIR)/AssemblyInfo.cs \
    $(IOS_OPENTK_1_0_CORE_SOURCES)     \
    AudioToolbox/AudioBuffers.cs       \
    AudioToolbox/AudioType.cs          \
    AudioToolbox/AudioFormat.cs          \
    $(SHARED_SYSTEM_DRAWING_SOURCES)   \
    System.Drawing/ColorKnownColorTypeForwarders.cs \

WATCHOS_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \

WATCHOS_CORE_SOURCES +=           \
    $(WATCHOS_EXTRA_CORE_SOURCES) \

WATCHOS_SOURCES +=                \
    $(WATCHOS_EXTRA_CORE_SOURCES) \
    $(WATCHOS_HTTP_SOURCES)        \
    $(SHARED_DESIGNER_CS) \

$(WATCH_BUILD_DIR)/Constants.cs: $(TOP)/src/Constants.watch.cs.in Makefile $(TOP)/Make.config.inc | $(WATCH_BUILD_DIR)
	$(call Q_PROF_GEN,watch) sed \
		-e "s/@VERSION@/$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)/g" \
		-e 's/@REVISION@/$(IOS_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h)/g' \
		-e "s/@WATCH_SDK_VERSION@/$(WATCH_SDK_VERSION)/g" \
		$< > $@

$(WATCH_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in $(TOP)/Make.config  $(TOP)/.git/HEAD | $(WATCH_BUILD_DIR)
	$(call Q_PROF_GEN,watch) sed \
		-e 's|@PRODUCT_NAME@|Xamarin.WatchOS|g' \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g' \
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
	$(call Q_PROF_CSC,watch) $(WATCH_CSC) -nologo -out:$@ -target:library -debug -unsafe \
		-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		-define:COREBUILD    \
		$(WATCH_DEFINES)     \
		$(WATCHOS_CORE_SOURCES)

# generated_sources
$(WATCH_BUILD_DIR)/watch/generated_sources: $(WATCH_GENERATOR) $(WATCHOS_APIS) $(WATCH_BUILD_DIR)/watch/core.dll $(WATCH_BUILD_DIR)/Xamarin.WatchOS.BindingAttributes.dll $(BUILD_DIR)/watchos.rsp
	$(call Q_PROF_GEN,watch) $(WATCH_GENERATE) @$(BUILD_DIR)/watchos.rsp

$(BUILD_DIR)/watchos.rsp: Makefile Makefile.generator frameworks.sources
	$(Q_GEN) echo  \
		-inline-selectors                                        \
		-process-enums                                           \
		-warnaserror:$(WATCH_GENERATOR_WARNASERROR)              \
		-core                                                    \
		-sourceonly=$(WATCH_BUILD_DIR)/watch/generated_sources   \
		-compiler=$(WATCH_CSC) \
		-nologo -nostdlib -noconfig                              \
		-tmpdir=$(WATCH_BUILD_DIR)/watch                         \
		-baselib=$(WATCH_BUILD_DIR)/watch/core.dll               \
		-attributelib=$(WATCH_BUILD_DIR)/Xamarin.WatchOS.BindingAttributes.dll \
		-native-exception-marshalling                            \
		$(WATCH_GENERATED_DEFINES)				\
		--ns:ObjCRuntime                                         \
		$(WATCHOS_APIS)                                            \
		--target-framework=Xamarin.WatchOS,v1.0                     \
		> $@

### .NET ###

$(WATCHOS_DOTNET_BUILD_DIR)/core-watchos.dll: $(WATCHOS_CORE_SOURCES) frameworks.sources | $(WATCHOS_DOTNET_BUILD_DIR)
	$(Q_DOTNET_GEN) \
		$(SYSTEM_CSC) $(DOTNET_FLAGS) \
		$(WATCHOS_CORE_WARNINGS_TO_FIX) \
		$(WATCHOS_CORE_DEFINES) \
		$(WATCHOS_CORE_SOURCES) \
		-out:$@ \

$(WATCHOS_DOTNET_BUILD_DIR)/watchos-generated-sources: $(DOTNET_GENERATOR) $(WATCHOS_APIS) $(WATCHOS_DOTNET_BUILD_DIR)/core-watchos.dll $(DOTNET_BINDING_ATTRIBUTES) $(WATCHOS_DOTNET_BUILD_DIR)/watchos.rsp | $(WATCHOS_DOTNET_BUILD_DIR)/generated-sources
	$(Q_DOTNET_GEN) $< @$(WATCHOS_DOTNET_BUILD_DIR)/watchos.rsp

$(WATCHOS_DOTNET_BUILD_DIR)/watchos.rsp: Makefile Makefile.generator frameworks.sources $(DOTNET_COMPILER) | $(WATCHOS_DOTNET_BUILD_DIR)
	$(Q) echo \
		$(WATCH_GENERATED_DEFINES) \
		$(DOTNET_GENERATOR_FLAGS) -inline-selectors \
		-warnaserror:$(WATCH_GENERATOR_WARNASERROR) \
		-sourceonly=$(WATCHOS_DOTNET_BUILD_DIR)/watchos-generated-sources \
		-tmpdir=$(WATCHOS_DOTNET_BUILD_DIR)/generated-sources \
		-baselib=$(WATCHOS_DOTNET_BUILD_DIR)/core-watchos.dll \
		--target-framework=.NETCoreApp,Version=6.0,Profile=watchos \
		$(WATCHOS_APIS) \
		> $@

$(WATCHOS_DOTNET_BUILD_DIR)/32/Xamarin.WatchOS%dll $(WATCHOS_DOTNET_BUILD_DIR)/32/Xamarin.WatchOS%pdb $(WATCHOS_DOTNET_BUILD_DIR)/ref/Xamarin.WatchOS%dll: $(WATCHOS_DOTNET_BUILD_DIR)/watchos-generated-sources $(WATCHOS_SOURCES) $(PRODUCT_KEY_PATH) | $(WATCHOS_DOTNET_BUILD_DIR)/32 $(WATCHOS_DOTNET_BUILD_DIR)/ref
	$(Q_DOTNET_GEN) \
		$(SYSTEM_CSC) $(DOTNET_FLAGS) -out:$(WATCHOS_DOTNET_BUILD_DIR)/32/Xamarin.WatchOS.dll -optimize \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) \
		-refout:$(WATCHOS_DOTNET_BUILD_DIR)/ref/Xamarin.WatchOS.dll \
		$(WATCH_DEFINES) \
		$(ARGS_32) \
		$(WATCHOS_WARNINGS_TO_FIX) \
		-warnaserror:$(NULLABILITY_WARNINGS) \
		$(IOS_CSC_FLAGS_XI) \
		$(WATCHOS_SOURCES) \
		@$<

$(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS%dll $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS%pdb: $(WATCHOS_SOURCES) $(WATCH_BUILD_DIR)/watch/generated_sources $(PRODUCT_KEY_PATH) | $(WATCH_BUILD_DIR)/watch-32
	$(call Q_PROF_CSC,watch) $(WATCH_CSC) -nologo -out:$(basename $@).dll -target:library -debug -unsafe -optimize \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) $(WATCH_DEFINES) \
		-deterministic \
		$(ARGS_32) \
		-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		$(WATCHOS_SOURCES) @$(WATCH_BUILD_DIR)/watch/generated_sources

$(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll: $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS.dll | $(WATCH_BUILD_DIR)/reference
	$(Q_STRIP) mono-cil-strip -q $< $@

$(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.pdb: $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS.pdb | $(WATCH_BUILD_DIR)/reference
	$(Q) $(CP) $< $@

# MonoTouch.NUnitLite
$(WATCH_BUILD_DIR)/reference/MonoTouch.NUnitLite%dll $(WATCH_BUILD_DIR)/reference/MonoTouch.NUnitLite%pdb: $(WATCHOS_TOUCHUNIT_SOURCES) $(PRODUCT_KEY_PATH) $(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll
	$(call Q_PROF_CSC,watch) $(SYSTEM_CSC) -features:strict -nologo -out:$(basename $@).dll -target:library -debug:portable -optimize -publicsign -noconfig -nostdlib \
		-keyfile:$(PRODUCT_KEY_PATH) -r:$(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll -r:$(MONOTOUCH_WATCH_MONO_PATH)/mscorlib.dll -r:$(MONOTOUCH_WATCH_MONO_PATH)/System.dll -r:$(MONOTOUCH_WATCH_MONO_PATH)/System.Xml.dll \
		-r:$(MONOTOUCH_WATCH_MONO_PATH)/System.Core.dll \
		-nowarn:3006,612,649,414,1635,659 \
		-define:NUNITLITE,CLR_4_0,NET_4_5,__MOBILE__,MONO $(WATCH_DEFINES) \
		-define:XAMCORE_2_0,__UNIFIED__ \
		-deterministic \
		$(WATCHOS_TOUCHUNIT_SOURCES)

$(PROJECT_DIR)/xamwatch.csproj: xamwatch.tmpl.csproj Makefile $(wildcard $(TOP)/*.sources)
	@sed -e 's*<!--%FILES%-->*$(foreach file,$(WATCHOS_SOURCES),<Compile Include="$(file)"/>)*' -e 's*<!--%APIS%-->*$(foreach file,$(WATCHOS_APIS),<None Include="$(file)"/>)*' $< | xmllint --format - > $@

$(PROJECT_DIR)/MonoTouch.NUnitLite.watchos.csproj: MonoTouch.NUnitLite.watchos.templ.csproj Makefile touch-unit.sources $(TOP)/Make.config $(TOP)/mk/mono.mk
	$(Q) sed -e 's*<!--%FILES%-->*$(foreach file,$(WATCHOS_TOUCHUNIT_SOURCES),<Compile Include="$(file)"/>)*' $< | xmllint --format - > $@

PROJECT_FILES += $(PROJECT_DIR)/xamwatch.csproj $(PROJECT_DIR)/MonoTouch.NUnitLite.watchos.csproj

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
	$(PROJECT_DIR)/xamwatch.csproj                                                          \
	$(PROJECT_DIR)/MonoTouch.NUnitLite.watchos.csproj                                       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/Xamarin.WatchOS.dll          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/Xamarin.WatchOS.pdb      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.WatchOS.dll                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.WatchOS.pdb                    \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/MonoTouch.NUnitLite.dll      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/MonoTouch.NUnitLite.pdb

DOTNET_TARGETS += \
	$(WATCHOS_DOTNET_BUILD_DIR)/32/Xamarin.WatchOS.dll \
	$(WATCHOS_DOTNET_BUILD_DIR)/ref/Xamarin.WatchOS.dll \
	$(DOTNET_DESTDIR)/$(WATCHOS_NUGET).Ref/ref/net6.0/Xamarin.WatchOS.dll \
	$(foreach rid,$(DOTNET_WATCHOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(WATCHOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.WatchOS.dll) \
	$(foreach rid,$(DOTNET_WATCHOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(WATCHOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.WatchOS.pdb) \

DOTNET_TARGETS_DIRS += \
	$(WATCHOS_DOTNET_BUILD_DIR) \
	$(WATCHOS_DOTNET_BUILD_DIR)/generated-sources \
	$(WATCHOS_DOTNET_BUILD_DIR)/32 \
	$(WATCHOS_DOTNET_BUILD_DIR)/ref \
	$(DOTNET_DESTDIR)/$(WATCHOS_NUGET).Ref/ref/net6.0 \
	$(foreach rid,$(DOTNET_WATCHOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(WATCHOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0) \

# reference assemblies, this is just for compilation with XS
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.dll: $(WATCH_BUILD_DIR)/reference/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/Facades
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.pdb: $(WATCH_BUILD_DIR)/reference/%.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.config: $(WATCH_BUILD_DIR)/reference/%.config | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS
	$(Q) install -m 0644 $< $@

$(DOTNET_DESTDIR)/$(WATCHOS_NUGET).Ref/ref/net6.0/Xamarin.WatchOS.dll: $(WATCHOS_DOTNET_BUILD_DIR)/ref/Xamarin.WatchOS.dll | $(DOTNET_DESTDIR)/$(WATCHOS_NUGET).Ref/ref/net6.0
	$(Q) $(CP) $< $@

# the actual architecture-specific versions
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.WatchOS.dll: $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.WatchOS.pdb: $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits
	$(Q) install -m 0644 $< $@

$(foreach rid,$(DOTNET_WATCHOS_RUNTIME_IDENTIFIERS_32),$(DOTNET_DESTDIR)/$(WATCHOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.WatchOS.dll): $(WATCHOS_DOTNET_BUILD_DIR)/32/Xamarin.WatchOS.dll | $(foreach rid,$(DOTNET_WATCHOS_RUNTIME_IDENTIFIERS_32),$(DOTNET_DESTDIR)/$(WATCHOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0)
	$(Q) $(CP) $< $@

$(foreach rid,$(DOTNET_WATCHOS_RUNTIME_IDENTIFIERS_32),$(DOTNET_DESTDIR)/$(WATCHOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.WatchOS.pdb): $(WATCHOS_DOTNET_BUILD_DIR)/32/Xamarin.WatchOS.pdb | $(foreach rid,$(DOTNET_WATCHOS_RUNTIME_IDENTIFIERS_32),$(DOTNET_DESTDIR)/$(WATCHOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0)
	$(Q) $(CP) $< $@

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

TVOS_DEFINES = -define:IPHONE -define:MONOTOUCH -d:TVOS -d:XAMCORE_3_0 -d:__TVOS__ $(APPLETLS_DEFINES) -d:SYSTEM_NET_HTTP
TVOS_GENERATOR=$(BUILD_DIR)/common/bgen.exe
TVOS_GENERATE=$(SYSTEM_MONO) --debug $(TVOS_GENERATOR)
TVOS_GENERATED_DEFINES= -d:TVOS -d:XAMCORE_3_0

TVOS_CORE_WARNINGS_TO_FIX=-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX)
TVOS_CORE_DEFINES=$(TVOS_DEFINES) -d:COREBUILD
TVOS_GENERATOR_DEFINES = $(TVOS_GENERATED_DEFINES)
TVOS_WARNINGS_TO_FIX=-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX)

TVOS_EXTRA_CORE_SOURCES = \
	$(TVOS_BUILD_DIR)/Constants.cs    \
    $(TVOS_BUILD_DIR)/AssemblyInfo.cs \
	$(IOS_OPENTK_1_0_CORE_SOURCES)    \
	$(SHARED_SYSTEM_DRAWING_SOURCES) \

TVOS_CORE_SOURCES +=           \
	$(TVOS_EXTRA_CORE_SOURCES) \

TVOS_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \
	$(SHARED_DESIGNER_CS) \

TVOS_SOURCES +=                \
	$(TVOS_EXTRA_CORE_SOURCES) \
	$(TVOS_HTTP_SOURCES)     \

$(TVOS_BUILD_DIR)/Constants.cs: $(TOP)/src/Constants.tvos.cs.in Makefile $(TOP)/Make.config.inc | $(TVOS_BUILD_DIR)
	$(call Q_PROF_GEN,tvos) sed \
		-e "s/@VERSION@/$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)/g" \
		-e 's/@REVISION@/$(IOS_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h)/g' \
		-e "s/@TVOS_SDK_VERSION@/$(TVOS_SDK_VERSION)/g" \
		$< > $@

$(TVOS_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in $(TOP)/Make.config  $(TOP)/.git/HEAD | $(TVOS_BUILD_DIR)
	$(call Q_PROF_GEN,tvos) sed \
		-e 's|@PRODUCT_NAME@|Xamarin.TVOS|g' \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g' \
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
	$(call Q_PROF_CSC,tvos) $(TV_CSC) -nologo -out:$@ -target:library -debug -unsafe \
		-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		-define:COREBUILD    \
		$(TVOS_DEFINES)     \
		$(TVOS_CORE_SOURCES)

# generated_sources
$(TVOS_BUILD_DIR)/tvos/generated_sources: $(TVOS_GENERATOR) $(TVOS_APIS) $(TVOS_BUILD_DIR)/tvos/core.dll $(TVOS_BUILD_DIR)/Xamarin.TVOS.BindingAttributes.dll $(BUILD_DIR)/tvos.rsp
	$(call Q_PROF_GEN,tvos) $(TVOS_GENERATE) @$(BUILD_DIR)/tvos.rsp

$(BUILD_DIR)/tvos.rsp: Makefile Makefile.generator frameworks.sources
	$(Q_GEN) echo \
		-inline-selectors                                        \
		-process-enums                                           \
		-warnaserror:$(TVOS_GENERATOR_WARNASERROR)               \
		-core                                                    \
		-sourceonly=$(TVOS_BUILD_DIR)/tvos/generated_sources     \
		-compiler=$(TV_CSC) \
		-nologo -nostdlib -noconfig                              \
		-tmpdir=$(TVOS_BUILD_DIR)/tvos                         \
		-baselib=$(TVOS_BUILD_DIR)/tvos/core.dll               \
		-attributelib=$(TVOS_BUILD_DIR)/Xamarin.TVOS.BindingAttributes.dll \
		$(TVOS_GENERATED_DEFINES)				\
		-native-exception-marshalling                            \
		--ns:ObjCRuntime                                         \
		$(TVOS_APIS)                                            \
		--target-framework=Xamarin.TVOS,v1.0                     \
		> $@

### .NET ###

$(TVOS_DOTNET_BUILD_DIR)/core-tvos.dll: $(TVOS_CORE_SOURCES) frameworks.sources Makefile | $(TVOS_DOTNET_BUILD_DIR)
	$(Q_DOTNET_GEN) \
		$(SYSTEM_CSC) $(DOTNET_FLAGS) \
		$(TVOS_CORE_WARNINGS_TO_FIX) \
		$(TVOS_CORE_DEFINES) \
		$(TVOS_CORE_SOURCES) \
		-out:$@ \

$(TVOS_DOTNET_BUILD_DIR)/tvos-generated-sources: $(DOTNET_GENERATOR) $(TVOS_APIS) $(TVOS_DOTNET_BUILD_DIR)/core-tvos.dll $(DOTNET_BINDING_ATTRIBUTES) $(TVOS_DOTNET_BUILD_DIR)/tvos.rsp | $(TVOS_DOTNET_BUILD_DIR)/generated-sources
	$(Q_DOTNET_GEN) $< @$(TVOS_DOTNET_BUILD_DIR)/tvos.rsp

$(TVOS_DOTNET_BUILD_DIR)/tvos.rsp: Makefile Makefile.generator frameworks.sources $(DOTNET_COMPILER) | $(TVOS_DOTNET_BUILD_DIR)
	$(Q) echo \
		$(TVOS_GENERATOR_DEFINES) \
		$(DOTNET_GENERATOR_FLAGS) -inline-selectors \
		-warnaserror:$(TVOS_GENERATOR_WARNASERROR) \
		-sourceonly=$(TVOS_DOTNET_BUILD_DIR)/tvos-generated-sources \
		-tmpdir=$(TVOS_DOTNET_BUILD_DIR)/generated-sources \
		-baselib=$(TVOS_DOTNET_BUILD_DIR)/core-tvos.dll \
		--target-framework=.NETCoreApp,Version=6.0,Profile=tvos \
		$(TVOS_APIS) \
		> $@

$(TVOS_DOTNET_BUILD_DIR)/64/Xamarin.TVOS%dll $(TVOS_DOTNET_BUILD_DIR)/64/Xamarin.TVOS%pdb $(TVOS_DOTNET_BUILD_DIR)/ref/Xamarin.TVOS%dll: $(TVOS_DOTNET_BUILD_DIR)/tvos-generated-sources $(TVOS_SOURCES) $(PRODUCT_KEY_PATH) | $(TVOS_DOTNET_BUILD_DIR)/64 $(TVOS_DOTNET_BUILD_DIR)/ref
	$(Q_DOTNET_GEN) \
		$(SYSTEM_CSC) $(DOTNET_FLAGS) -out:$(TVOS_DOTNET_BUILD_DIR)/64/Xamarin.TVOS.dll -optimize \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) \
		-refout:$(TVOS_DOTNET_BUILD_DIR)/ref/Xamarin.TVOS.dll \
		$(TVOS_DEFINES) \
		$(ARGS_64) \
		$(TVOS_WARNINGS_TO_FIX) \
		-warnaserror:$(NULLABILITY_WARNINGS) \
		$(IOS_CSC_FLAGS_XI) \
		$(TVOS_SOURCES) \
		@$<

$(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS%dll $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS%pdb: $(TVOS_SOURCES) $(TVOS_BUILD_DIR)/tvos/generated_sources $(PRODUCT_KEY_PATH) | $(TVOS_BUILD_DIR)/tvos-64
	$(call Q_PROF_CSC,tvos) $(TV_CSC) -nologo -out:$(basename $@).dll -target:library -debug -unsafe -optimize \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) $(TVOS_DEFINES) \
		-deterministic \
		$(ARGS_64) \
		-nowarn:219,618,114,414,1635,3021,$(IOS_WARNINGS_THAT_YOU_SHOULD_FIX) \
		$(TVOS_SOURCES) @$(TVOS_BUILD_DIR)/tvos/generated_sources

$(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll: $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS.dll | $(TVOS_BUILD_DIR)/reference
	$(Q_STRIP) mono-cil-strip -q $< $@

$(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.pdb: $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS.pdb | $(TVOS_BUILD_DIR)/reference
	$(Q) $(CP) $< $@

# MonoTouch.NUnitLite
$(TVOS_BUILD_DIR)/reference/MonoTouch.NUnitLite%dll $(TVOS_BUILD_DIR)/reference/MonoTouch.NUnitLite%pdb: $(TVOS_TOUCHUNIT_SOURCES) $(PRODUCT_KEY_PATH) $(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll $(TVOS_BUILD_DIR)/reference/MonoTouch.Dialog-1.dll
	$(call Q_PROF_CSC,tvos) $(SYSTEM_CSC) -features:strict -nologo -r:$(MONOTOUCH_TV_MONO_PATH)/mscorlib.dll -out:$(basename $@).dll -target:library -debug:portable -optimize -noconfig -nostdlib -publicsign \
		-keyfile:$(PRODUCT_KEY_PATH) -r:$(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll -r:$(TVOS_BUILD_DIR)/reference/MonoTouch.Dialog-1.dll -r:$(MONOTOUCH_TV_MONO_PATH)/System.dll -r:$(MONOTOUCH_TV_MONO_PATH)/System.Xml.dll \
		-r:$(MONOTOUCH_TV_MONO_PATH)/System.Core.dll \
		-nowarn:3006,612,649,414,1635,659 \
		-define:NUNITLITE,CLR_4_0,NET_4_5,__MOBILE__,MONO $(TVOS_DEFINES) \
		-define:XAMCORE_2_0,__UNIFIED__ \
		-deterministic \
		$(TVOS_TOUCHUNIT_SOURCES)

# MonoTouch.Dialog-1
$(TVOS_BUILD_DIR)/reference/MonoTouch.Dialog-1.dll: $(MACIOS_BINARIES_PATH)/MonoTouch.Dialog-Unified/tvos/MonoTouch.Dialog-1.dll | $(TVOS_BUILD_DIR)/reference
	$(Q) $(CP) $< $@

$(TVOS_BUILD_DIR)/reference/MonoTouch.Dialog-1.pdb: $(MACIOS_BINARIES_PATH)/MonoTouch.Dialog-Unified/tvos/MonoTouch.Dialog-1.pdb | $(TVOS_BUILD_DIR)/reference
	$(Q) $(CP) $< $@

$(PROJECT_DIR)/xamtvos.csproj: xamtvos.tmpl.csproj Makefile $(wildcard $(TOP)/*.sources)
	@sed -e 's*<!--%FILES%-->*$(foreach file,$(TVOS_SOURCES),<Compile Include="$(file)"/>)*' -e 's*<!--%APIS%-->*$(foreach file,$(TVOS_APIS),<None Include="$(file)"/>)*' $< | xmllint --format - > $@

$(PROJECT_DIR)/MonoTouch.NUnitLite.tvos.csproj: MonoTouch.NUnitLite.tvos.templ.csproj Makefile touch-unit.sources $(TOP)/Make.config $(TOP)/mk/mono.mk
	$(Q) sed -e 's*<!--%FILES%-->*$(foreach file,$(TVOS_TOUCHUNIT_SOURCES),<Compile Include="$(file)"/>)*' $< | xmllint --format - > $@

PROJECT_FILES += $(PROJECT_DIR)/xamtvos.csproj $(PROJECT_DIR)/MonoTouch.NUnitLite.tvos.csproj

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
	$(PROJECT_DIR)/xamtvos.csproj                                                        \
	$(PROJECT_DIR)/MonoTouch.NUnitLite.tvos.csproj                                       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Xamarin.TVOS.dll             \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Xamarin.TVOS.pdb             \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.TVOS.dll                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.TVOS.pdb                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/OpenTK-1.0.dll               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/OpenTK-1.0.pdb               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/OpenTK-1.0.dll.config        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.Dialog-1.dll       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.Dialog-1.pdb       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.NUnitLite.dll      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.NUnitLite.pdb

DOTNET_TARGETS += \
	$(TVOS_DOTNET_BUILD_DIR)/64/Xamarin.TVOS.dll \
	$(TVOS_DOTNET_BUILD_DIR)/ref/Xamarin.TVOS.dll \
	$(DOTNET_DESTDIR)/$(TVOS_NUGET).Ref/ref/net6.0/Xamarin.TVOS.dll \
	$(foreach rid,$(DOTNET_TVOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(TVOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.TVOS.dll) \
	$(foreach rid,$(DOTNET_TVOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(TVOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.TVOS.pdb) \

DOTNET_TARGETS_DIRS += \
	$(TVOS_DOTNET_BUILD_DIR) \
	$(TVOS_DOTNET_BUILD_DIR)/generated-sources \
	$(TVOS_DOTNET_BUILD_DIR)/64 \
	$(TVOS_DOTNET_BUILD_DIR)/ref \
	$(DOTNET_DESTDIR)/$(TVOS_NUGET).Ref/ref/net6.0 \
	$(foreach rid,$(DOTNET_TVOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(TVOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0) \

# reference assemblies, this is just for compilation with XS
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.dll: $(TVOS_BUILD_DIR)/reference/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Facades
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.pdb: $(TVOS_BUILD_DIR)/reference/%.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.config: $(TVOS_BUILD_DIR)/reference/%.config | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS
	$(Q) install -m 0644 $< $@

$(DOTNET_DESTDIR)/$(TVOS_NUGET).Ref/ref/net6.0/Xamarin.TVOS.dll: $(TVOS_DOTNET_BUILD_DIR)/ref/Xamarin.TVOS.dll | $(DOTNET_DESTDIR)/$(TVOS_NUGET).Ref/ref/net6.0
	$(Q) $(CP) $< $@

# the actual architecture-specific versions
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.TVOS.dll: $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.TVOS.pdb: $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits
	$(Q) install -m 0644 $< $@

$(foreach rid,$(DOTNET_TVOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(TVOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.TVOS.dll): $(TVOS_DOTNET_BUILD_DIR)/64/Xamarin.TVOS.dll | $(foreach rid,$(DOTNET_TVOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(TVOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0)
	$(Q) $(CP) $< $@

$(foreach rid,$(DOTNET_TVOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(TVOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0/Xamarin.TVOS.pdb): $(TVOS_DOTNET_BUILD_DIR)/64/Xamarin.TVOS.pdb | $(foreach rid,$(DOTNET_TVOS_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$(TVOS_NUGET).Runtime.$(rid)/runtimes/$(rid)/lib/net6.0)
	$(Q) $(CP) $< $@

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

$(DOTNET_COMPILER): Makefile $(TOP)/Make.config | $(DOTNET_BUILD_DIR)
	$(Q) echo "#!/bin/zsh -e" > $@
	$(Q) echo "exec $(SYSTEM_CSC) $(DOTNET_FLAGS) \"\$$@\"" >> $@
	$(Q) chmod +x $@

# This tells NuGet to use the exact same dotnet version we've configured in Make.config
global.json: $(TOP)/global6.json
	$(Q) $(CP) $< $@

install-local:: $(INSTALL_TARGETS)
all-local:: $(ALL_TARGETS)

$(DOTNET_TARGETS_DIRS):
	$(Q) mkdir -p $@

dotnet: $(DOTNET_TARGETS)
ifdef ENABLE_DOTNET
all-local:: $(DOTNET_TARGETS)
endif

project-files: $(PROJECT_FILES)

# Using .SECONDARY can cause make to go into an infinite loop.
# See https://github.com/xamarin/maccore/issues/762.
#.SECONDARY:
