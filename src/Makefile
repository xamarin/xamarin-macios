TOP=..
include $(TOP)/Make.config

BUILD_DIR=build
RSP_DIR=rsp
DOTNET_BUILD_DIR=$(BUILD_DIR)/dotnet
PROJECT_DIR=.

include $(TOP)/src/frameworks.sources
include $(TOP)/mk/rules.mk

include $(TOP)/src/touch-unit.sources

export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

MACOS_BUILD_DIR=$(BUILD_DIR)/mac
IOS_BUILD_DIR=$(BUILD_DIR)/ios
WATCH_BUILD_DIR=$(BUILD_DIR)/watch
TVOS_BUILD_DIR=$(BUILD_DIR)/tvos
MACCATALYST_BUILD_DIR=$(BUILD_DIR)/maccatalyst

MACOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/macos
IOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/ios
TVOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/tvos
MACCATALYST_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/maccatalyst

GENERATOR_FLAGS=-process-enums -core -nologo -nostdlib -noconfig -native-exception-marshalling --ns:ObjCRuntime

GENERATOR_TF_VERSION=$(subst net,,$(DOTNET_TFM))

# This is a workaround to create assemblies with a higher version than the
# stable versions we released for Xcode 13.3. The stable versions we released
# for Xcode 13.3 has a <osmajor>.<osminor>.300 version number, but that
# version scheme has been changed, where the third digit from now on will
# always be 0 in the assembly version. However, this means that until Apple
# releases new OS versions (and we bind those versions), the assembly version
# will be lower than the stable version wherever we've implemented the new
# versioning scheme. This complicates testing, so just bump the third digit to
# 600 until we're using a new Xcode (and thus presumably new OS versions as
# well). This workaround can be removed at that point, but implement it so
# that it will just be skipped/ignored if it isn't removed.
ifeq ($(XCODE_VERSION),13.3)
NUGET_VERSION_THIRD_DIGIT_WORKAROUND=600
else
NUGET_VERSION_THIRD_DIGIT_WORKAROUND=0
endif

DOTNET_REFERENCES = \
	/r:$(DOTNET_BCL_DIR)/System.Buffers.dll \
	/r:$(DOTNET_BCL_DIR)/System.Collections.Concurrent.dll \
	/r:$(DOTNET_BCL_DIR)/System.Collections.dll \
	/r:$(DOTNET_BCL_DIR)/System.Collections.NonGeneric.dll \
	/r:$(DOTNET_BCL_DIR)/System.Console.dll \
	/r:$(DOTNET_BCL_DIR)/System.Diagnostics.Debug.dll \
	/r:$(DOTNET_BCL_DIR)/System.Diagnostics.Tools.dll \
	/r:$(DOTNET_BCL_DIR)/System.Diagnostics.StackTrace.dll \
	/r:$(DOTNET_BCL_DIR)/System.Drawing.Primitives.dll \
	/r:$(DOTNET_BCL_DIR)/System.IO.Compression.dll \
	/r:$(DOTNET_BCL_DIR)/System.IO.FileSystem.dll \
	/r:$(DOTNET_BCL_DIR)/System.Linq.dll \
	/r:$(DOTNET_BCL_DIR)/System.Memory.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.Http.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.NameResolution.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.Primitives.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.Requests.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.Security.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.ServicePoint.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.Sockets.dll \
	/r:$(DOTNET_BCL_DIR)/System.Numerics.Vectors.dll \
	/r:$(DOTNET_BCL_DIR)/System.Resources.ResourceManager.dll \
	/r:$(DOTNET_BCL_DIR)/System.Runtime.dll \
	/r:$(DOTNET_BCL_DIR)/System.Runtime.CompilerServices.Unsafe.dll \
	/r:$(DOTNET_BCL_DIR)/System.Runtime.Extensions.dll \
	/r:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll \
	/r:$(DOTNET_BCL_DIR)/System.Runtime.Loader.dll \
	/r:$(DOTNET_BCL_DIR)/System.Security.Cryptography.dll \
	/r:$(DOTNET_BCL_DIR)/System.Security.Cryptography.X509Certificates.dll \
	/r:$(DOTNET_BCL_DIR)/System.Text.RegularExpressions.dll \
	/r:$(DOTNET_BCL_DIR)/System.Threading.dll \
	/r:$(DOTNET_BCL_DIR)/System.Threading.Tasks.dll \
	/r:$(DOTNET_BCL_DIR)/System.Threading.Thread.dll \
	/r:$(DOTNET_BCL_DIR)/System.Xml.dll \
	/r:$(DOTNET_BCL_DIR)/System.Xml.ReaderWriter.dll \

DOTNET_OR_GREATER_DEFINES:=$(foreach version,$(shell seq 6 $(firstword $(subst ., ,$(subst net,,$(DOTNET_TFM))))),/define:NET$(version)_0_OR_GREATER)
DOTNET_FLAGS=/noconfig /nostdlib+ /deterministic /features:strict /nologo /target:library /debug /unsafe /define:NET /define:NET_TODO /define:XAMCORE_3_0 $(DOTNET_OR_GREATER_DEFINES) $(DOTNET_REFERENCES)

ifeq ($(XCODE_IS_STABLE),true)
DOTNET_FLAGS+=/define:XCODE_IS_STABLE
endif

DOTNET_COMPILER=$(DOTNET_BUILD_DIR)/compiler
DOTNET_GENERATOR_FLAGS=$(GENERATOR_FLAGS) -compiler=$(abspath $(DOTNET_COMPILER)) --lib=$(DOTNET_BCL_DIR) -attributelib:$(DOTNET_BINDING_ATTRIBUTES) $(DOTNET_REFERENCES)
DOTNET_GENERATOR=$(DOTNET_BUILD_DIR)/bgen/bgen
DOTNET_BINDING_ATTRIBUTES=$(DOTNET_BUILD_DIR)/Xamarin.Apple.BindingAttributes.dll

#
# Specific warnings that we want reported as errors by generator
# It takes a comma separated list, but an empty list means
# reporting all warnings as errors.
#
# https://github.com/dotnet/roslyn/issues/41605
NULLABILITY_WARNINGS=nullable
GENERATOR_WARNASERROR=
IOS_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
WATCH_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
TVOS_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
MACOS_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
MACCATALYST_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)

# this is due to deprecated APIs used in the generated code.
BGEN_WARNINGS_TO_FIX=BI1234

#
# Specific warnings we're ignoring from the C# compiler
# We should look into each of these and see if they're really needed,
# and if so, document why.
#
CSC_WARNINGS_TO_FIX=108,219,618,114,414,1635,3021,4014

# warning CS1591: Missing XML comment for publicly visible type or member
# We (will) post-process xml documentation to add missing members, so
# we can ignore this warning.
CSC_WARNINGS_TO_FIX:=$(CSC_WARNINGS_TO_FIX),1591

WARNINGS_TO_FIX = -nowarn:$(CSC_WARNINGS_TO_FIX)
CORE_WARNINGS_TO_FIX = -nowarn:$(CSC_WARNINGS_TO_FIX),$(BGEN_WARNINGS_TO_FIX)

DOTNET_WARNINGS_TO_FIX = -nowarn:$(CSC_WARNINGS_TO_FIX)
DOTNET_CORE_WARNINGS_TO_FIX = -nowarn:$(CSC_WARNINGS_TO_FIX),$(BGEN_WARNINGS_TO_FIX)

include ./Makefile.generator
include ./opentk.mk

include ./generator-diff.mk

COMMON_TARGET_DIRS = \
	$(BUILD_DIR)                    \
	$(BUILD_DIR)/common             \
	$(BUILD_DIR)/common/NativeTypes \

ARGS_32 = -define:ARCH_32
ARGS_64 = -define:ARCH_64

SHARED_SYSTEM_DRAWING_SOURCES = System.Drawing/PointSizeRectangleTypeForwarders.cs
SHARED_RESX = $(TOP)/tools/mtouch/Errors.resx
SHARED_DESIGNER_CS = $(BUILD_DIR)/common/Errors.Designer.cs

$(SHARED_DESIGNER_CS): $(SHARED_RESX)
	$(Q_GEN) ./generate-errors.csharp > $(SHARED_DESIGNER_CS)

#
# Xamarin.iOS
#

# Add new bindings + source files in frameworks.sources, not here.

IOS_DOTNET_EXTRA_SOURCES = \
	$(IOS_BUILD_DIR)/Constants.cs    \
	$(DOTNET_BUILD_DIR)/Constants.ios.generated.cs \
	$(IOS_BUILD_DIR)/AssemblyInfo.cs \
	$(SHARED_DESIGNER_CS) \

IOS_EXTRA_SOURCES = \
	$(IOS_OPENTK_1_0_CORE_SOURCES)   \
	$(IOS_BUILD_DIR)/Constants.cs    \
	$(BUILD_DIR)/Constants.ios.generated.cs \
	$(IOS_BUILD_DIR)/AssemblyInfo.cs \
	$(SHARED_DESIGNER_CS) \
	$(SHARED_SYSTEM_DRAWING_SOURCES) \

IOS_DOTNET_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \

IOS_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \

IOS_DOTNET_CORE_SOURCES += $(IOS_DOTNET_EXTRA_SOURCES)
IOS_CORE_SOURCES += $(IOS_EXTRA_SOURCES)

IOS_DOTNET_SOURCES += $(IOS_DOTNET_EXTRA_SOURCES) $(IOS_DOTNET_HTTP_SOURCES)
IOS_SOURCES += $(IOS_EXTRA_SOURCES) $(IOS_HTTP_SOURCES)

IOS_GENERATOR_FLAGS = -inline-selectors -d:IOS -process-enums -warnaserror:$(IOS_GENERATOR_WARNASERROR)
IOS_DEFINES = -define:IPHONE -define:IOS -define:MONOTOUCH -d:__IOS__ -d:SYSTEM_NET_HTTP
IOS_GENERATOR=$(BUILD_DIR)/common/bgen.exe
IOS_GENERATE=$(SYSTEM_MONO) --debug $(IOS_GENERATOR)

IOS_CORE_DEFINES=-define:COREBUILD $(IOS_DEFINES)

$(IOS_BUILD_DIR)/Constants.cs: Constants.iOS.cs.in Makefile $(TOP)/Make.config.inc | $(IOS_BUILD_DIR)
	$(call Q_PROF_GEN,ios) sed \
		-e "s/@VERSION@/$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)/g" \
		-e 's/@REVISION@/$(IOS_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h))/g' \
		-e "s/@IOS_SDK_VERSION@/$(IOS_SDK_VERSION)/g" \
		$< > $@

$(IOS_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in | $(IOS_BUILD_DIR)
	$(call Q_PROF_GEN,ios) sed \
		-e 's|@PRODUCT_NAME@|$(IOS_PRODUCT)|g' \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g' \
		-e 's|@PACKAGE_VERSION_MAJOR@|$(IOS_PACKAGE_VERSION_MAJOR)|g' \
		-e 's|@PACKAGE_VERSION_MINOR@|$(IOS_PACKAGE_VERSION_MINOR)|g' \
		-e 's|@PACKAGE_VERSION_REV@|$(IOS_PACKAGE_VERSION_REV)|g' \
		-e 's|@PACKAGE_VERSION_BUILD@|$(IOS_PACKAGE_VERSION_BUILD)|g' \
		-e 's|@NUGET_VERSION_NO_METADATA@|$(IOS_NUGET_VERSION_NO_METADATA)|g' \
		-e 's|@NUGET_VERSION_MAJOR@|$(IOS_NUGET_VERSION_MAJOR)|g' \
		-e 's|@NUGET_VERSION_MINOR@|$(IOS_NUGET_VERSION_MINOR)|g' \
		-e 's|@NUGET_VERSION_THIRD_DIGIT_WORKAROUND@|$(NUGET_VERSION_THIRD_DIGIT_WORKAROUND)|g' \
		-e 's|@NUGET_VERSION_REV@|$(IOS_NUGET_VERSION_PATCH)|g' \
		-e 's|@NUGET_VERSION_BUILD@|$(IOS_NUGET_COMMIT_DISTANCE)|g' \
		-e 's|@DOTNET_PLATFORM@|iOS|g' \
		< $< > $@.tmp
	$(Q) diff $@ $@.tmp >/dev/null 2>&1 || mv -f $@.tmp $@
	$(Q) rm -f $@.tmp
	$(Q) touch $@

# core.dll
$(IOS_BUILD_DIR)/native/core.dll: $(IOS_CORE_SOURCES) frameworks.sources $(RSP_DIR)/ios-defines.rsp
	$(Q) mkdir -p $(IOS_BUILD_DIR)native
	$(call Q_PROF_CSC,ios) $(IOS_CSC) -nologo -out:$@ -target:library -debug -unsafe \
		$(CORE_WARNINGS_TO_FIX) \
		@$(RSP_DIR)/ios-defines.rsp \
		$(IOS_CORE_DEFINES) \
		$(IOS_CORE_SOURCES)

# generated_sources
$(IOS_BUILD_DIR)/native/generated_sources: $(IOS_GENERATOR) $(IOS_APIS) $(IOS_BUILD_DIR)/native/core.dll $(IOS_BUILD_DIR)/native/Xamarin.iOS.BindingAttributes.dll $(BUILD_DIR)/ios.rsp
	$(call Q_PROF_GEN,ios) $(IOS_GENERATE) @$(BUILD_DIR)/ios.rsp

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
		@$(RSP_DIR)/ios-defines.rsp \
		> $@

define IOS_TARGETS_template
# Xamarin.iOS.dll
$(IOS_BUILD_DIR)/native-$(1)%Xamarin.iOS.dll $(IOS_BUILD_DIR)/native-$(1)%Xamarin.iOS.pdb $(4): $$(IOS_SOURCES) $(IOS_BUILD_DIR)/native/generated_sources $(PRODUCT_KEY_PATH) | $(IOS_BUILD_DIR)/reference
	@mkdir -p $(IOS_BUILD_DIR)/native-$(1)
	$$(call Q_PROF_CSC,ios/$(1) bit) $$(IOS_CSC) -nologo -out:$(IOS_BUILD_DIR)/native-$(1)/Xamarin.iOS.dll -target:library -debug -unsafe -optimize \
		-deterministic \
		$$(ARGS_$(1)) \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) $$(IOS_DEFINES) \
		$(5) \
		$(WARNINGS_TO_FIX) \
		-warnaserror:$(NULLABILITY_WARNINGS) \
		$$(IOS_CSC_FLAGS_XI) \
		@$(RSP_DIR)/ios-defines.rsp \
		$$(IOS_SOURCES) @$(IOS_BUILD_DIR)/native/generated_sources
endef

$(IOS_BUILD_DIR)/compat/%: $(MACIOS_BINARIES_PATH)/% | $(IOS_BUILD_DIR)/compat
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/%: $(IOS_BUILD_DIR)/compat/%
	$(Q) $(CP) $< $@

$(eval $(call IOS_TARGETS_template,32))
$(eval $(call IOS_TARGETS_template,64,,,$(IOS_BUILD_DIR)/reference%Xamarin.iOS.dll,-refout:$(IOS_BUILD_DIR)/reference/Xamarin.iOS.dll))

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

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/% : $(MACIOS_BINARIES_PATH)/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/Facades/% : $(MACIOS_BINARIES_PATH)/Facades/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/Facades
	$(Q) $(CP) $< $@

$(IOS_BUILD_DIR)/compat/%: $(MACIOS_BINARIES_PATH)/% | $(IOS_BUILD_DIR)/compat
	$(Q) $(CP) $< $@

clean-local::
	rm -rf build
	rm -f $(IOS_TARGETS) generated_sources *.mdb *.pdb

ifdef INCLUDE_IOS
IOS_TARGETS_DIRS += \
	$(IOS_BUILD_DIR)                                           \
	$(IOS_BUILD_DIR)/compat                                    \
	$(IOS_BUILD_DIR)/compat/Facades                            \
	$(IOS_BUILD_DIR)/reference                                 \
	$(IOS_BUILD_DIR)/reference/Facades                         \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin                      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/share/doc/MonoTouch      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS     \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/iOS           \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/iOS           \
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
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/MonoTouch.Dialog-1.dll  \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/MonoTouch.Dialog-1.pdb  \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/MonoTouch.NUnitLite.dll \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/MonoTouch.NUnitLite.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/OpenTK-1.0.dll          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/OpenTK-1.0.pdb          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/OpenTK-1.0.dll.config   \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/iOS/Xamarin.iOS.dll               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/iOS/Xamarin.iOS.pdb               \

ifdef IOS_SUPPORTS_32BIT_ARCHITECTURES
IOS_TARGETS += \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/iOS/Xamarin.iOS.dll               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/iOS/Xamarin.iOS.pdb               \

endif

endif

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

# the actual architecture-specific versions
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/iOS/Xamarin.iOS.dll: $(IOS_BUILD_DIR)/native-32/Xamarin.iOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/iOS
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/iOS/Xamarin.iOS.pdb: $(IOS_BUILD_DIR)/native-32/Xamarin.iOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/iOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/iOS/Xamarin.iOS.dll: $(IOS_BUILD_DIR)/native-64/Xamarin.iOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/iOS
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/iOS/Xamarin.iOS.pdb: $(IOS_BUILD_DIR)/native-64/Xamarin.iOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/iOS
	$(Q) install -m 0644 $< $@

$(IOS_TARGETS_DIRS):
	$(Q) mkdir -p $@

$(PROJECT_DIR)/xamios.csproj: xamios.tmpl.csproj Makefile $(wildcard $(TOP)/src/*.sources)
	@sed -e 's*<!--%FILES%-->*$(foreach file,$(IOS_SOURCES),<Compile Include="$(file)"/>)*' -e 's*<!--%APIS%-->*$(foreach file,$(IOS_APIS),<None Include="$(file)"/>)*' $< | xmllint --format - > $@

$(PROJECT_DIR)/MonoTouch.NUnitLite.csproj: MonoTouch.NUnitLite.templ.csproj Makefile touch-unit.sources $(TOP)/Make.config $(TOP)/mk/mono.mk
	$(Q) sed -e 's*<!--%FILES%-->*$(foreach file,$(IOS_TOUCHUNIT_SOURCES),<Compile Include="$(file)"/>)*' $< | xmllint --format - > $@

PROJECT_FILES += $(PROJECT_DIR)/xamios.csproj $(PROJECT_DIR)/MonoTouch.NUnitLite.csproj

ifdef INCLUDE_IOS
all-ios: $(IOS_TARGETS)
install-ios: $(IOS_TARGETS)
endif

ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_IOS
INSTALL_TARGETS+=install-ios
ALL_TARGETS+=all-ios
endif
endif

#
# Xamarin.Mac
#

MAC_COMMON_DEFINES = -define:MONOMAC -d:__MACOS__
MAC_full_ARGS = -define:NO_SYSTEM_DRAWING -define:XAMMAC_SYSTEM_MONO
MAC_mobile_ARGS = 
MACOS_GENERATOR_FLAGS = -d:MONOMAC -d:__MACOS__
MACOS_CORE_DEFINES = $(MAC_COMMON_DEFINES),COREBUILD
MACOS_DEFINES = $(MAC_COMMON_DEFINES) -D:XAMARIN_MODERN

$(MACOS_BUILD_DIR)/$(1)/$(3).pdb: $(MACOS_BUILD_DIR)/$(1)/$(3).dll

SN_KEY = $(PRODUCT_KEY_PATH)

MAC_DOTNET_EXTRA_CORE_SOURCES += \
	$(MACOS_BUILD_DIR)/Constants.cs \
	$(DOTNET_BUILD_DIR)/Constants.macos.generated.cs \

MAC_EXTRA_CORE_SOURCES += \
	$(MACOS_BUILD_DIR)/Constants.cs \
	$(BUILD_DIR)/Constants.macos.generated.cs \

# Add new bindings + source files in frameworks.sources, not here.

MACOS_DOTNET_CORE_SOURCES += \
	$(MAC_DOTNET_EXTRA_CORE_SOURCES) \

MACOS_CORE_SOURCES += \
	$(MAC_EXTRA_CORE_SOURCES) \

MACOS_DOTNET_SOURCES += \
	$(MAC_DOTNET_EXTRA_CORE_SOURCES) \
	$(MACOS_BUILD_DIR)/AssemblyInfo.cs \
	Darwin/KernelNotification.cs \
	Darwin/SystemLog.cs \
	$(SHARED_DESIGNER_CS) \

MACOS_SOURCES += \
	$(MAC_EXTRA_CORE_SOURCES) \
	$(MACOS_BUILD_DIR)/AssemblyInfo.cs \
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

MACOS_DOTNET_SOURCES += \
	$(MAC_CFNETWORK_SOURCES) \
	$(MAC_HTTP_SOURCES) \

$(MACOS_BUILD_DIR)/Constants.cs: Constants.mac.cs.in Makefile $(TOP)/Make.config.inc | $(MACOS_BUILD_DIR)
	$(Q) sed \
			-e "s/@VERSION@/$(MAC_PACKAGE_VERSION_MAJOR).$(MAC_PACKAGE_VERSION_MINOR).$(MAC_PACKAGE_VERSION_REV)/g" \
			-e 's/@REVISION@/$(MAC_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h))/g' \
			-e "s/@MACOS_SDK_VERSION@/$(MACOS_SDK_VERSION)/g" \
			-e "s/@MIN_XM_MONO_VERSION@/$(MIN_XM_MONO_VERSION)/g" \
		$< > $@

$(PROJECT_DIR)/xammac.csproj: xammac.tmpl.csproj Makefile $(wildcard $(TOP)/src/*.sources)
	@sed -e 's*<!--%FILES%-->*$(foreach file,$(MACOS_SOURCES) $(SHARED_SYSTEM_DRAWING_SOURCES),<Compile Include="$(file)"/>)*' -e 's*<!--%APIS%-->*$(foreach file,$(MACOS_APIS),<None Include="$(file)"/>)*' $< | xmllint --format - > $@

PROJECT_FILES += $(PROJECT_DIR)/xammac.csproj

$(MACOS_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in | $(MACOS_BUILD_DIR)
	$(call Q_PROF_GEN,mac) sed \
		-e 's|@PRODUCT_NAME@|$(MAC_PRODUCT)|g' \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g' \
		-e 's|@PACKAGE_VERSION_MAJOR@|$(MAC_PACKAGE_VERSION_MAJOR)|g' \
		-e 's|@PACKAGE_VERSION_MINOR@|$(MAC_PACKAGE_VERSION_MINOR)|g' \
		-e 's|@PACKAGE_VERSION_REV@|$(MAC_PACKAGE_VERSION_REV)|g' \
		-e 's|@PACKAGE_VERSION_BUILD@|$(MAC_PACKAGE_VERSION_BUILD)|g' \
		-e 's|@NUGET_VERSION_NO_METADATA@|$(MACOS_NUGET_VERSION_NO_METADATA)|g' \
		-e 's|@NUGET_VERSION_MAJOR@|$(MACOS_NUGET_VERSION_MAJOR)|g' \
		-e 's|@NUGET_VERSION_MINOR@|$(MACOS_NUGET_VERSION_MINOR)|g' \
		-e 's|@NUGET_VERSION_THIRD_DIGIT_WORKAROUND@|$(NUGET_VERSION_THIRD_DIGIT_WORKAROUND)|g' \
		-e 's|@NUGET_VERSION_REV@|$(MACOS_NUGET_VERSION_PATCH)|g' \
		-e 's|@NUGET_VERSION_BUILD@|$(MACOS_NUGET_COMMIT_DISTANCE)|g' \
		-e 's|@DOTNET_PLATFORM@|macOS|g' \
		< $< > $@.tmp
	$(Q) diff $@ $@.tmp >/dev/null 2>&1 || mv -f $@.tmp $@
	$(Q) rm -f $@.tmp
	$(Q) touch $@

# We can't pass the --target-framework values as parameters to the templates, because the commas interfere with Make's parameter parsing.
xm_full_profile=--target-framework=Xamarin.Mac,Version=v4.5,Profile=Full
xm_mobile_profile=--target-framework=Xamarin.Mac,Version=v2.0,Profile=Mobile

MAC_GENERATOR=$(BUILD_DIR)/common/bgen.exe
MAC_GENERATE=$(SYSTEM_MONO) --debug $(MAC_GENERATOR)

define MAC_GENERATOR_template
$(MACOS_BUILD_DIR)/$(1)/core.dll: $(MACOS_CORE_SOURCES) frameworks.sources $(RSP_DIR)/macos-defines.rsp
	@mkdir -p $(MACOS_BUILD_DIR)/$(1)
	$$(call Q_PROF_CSC,mac/$(1)) \
		$$(MAC_$(1)_CSC) -nologo -out:$$@ -target:library -debug -unsafe $(CORE_WARNINGS_TO_FIX) \
		@$(RSP_DIR)/macos-defines.rsp \
		$$(MACOS_CORE_DEFINES) \
		$(2) \
		$$(MACOS_CORE_SOURCES)

$(MACOS_BUILD_DIR)/$(1)/generated-sources: $$(MAC_GENERATOR) $(MACOS_APIS) $(MACOS_BUILD_DIR)/$(1)/core.dll $(MACOS_BUILD_DIR)/Xamarin.Mac-$(1).BindingAttributes.dll $(BUILD_DIR)/mac-$(1).rsp
	$$(call Q_PROF_GEN,mac/$(1)) $$(MAC_GENERATE) @$(BUILD_DIR)/mac-$(1).rsp

$(BUILD_DIR)/mac-$(1).rsp: Makefile Makefile.generator frameworks.sources
	$$(Q_GEN) echo \
		$(MACOS_GENERATOR_FLAGS) \
		-compiler:$$(MAC_$(1)_CSC) \
		-nologo \
		-process-enums \
		-warnaserror:$(MACOS_GENERATOR_WARNASERROR) \
		-native-exception-marshalling \
		-core \
		-sourceonly:$(MACOS_BUILD_DIR)/$(1)/generated-sources \
		-tmpdir:$(MACOS_BUILD_DIR)/$(1) \
		-baselib:$(MACOS_BUILD_DIR)/$(1)/core.dll \
		-attributelib:$(MACOS_BUILD_DIR)/Xamarin.Mac-$(1).BindingAttributes.dll \
		-d:NO_SYSTEM_DRAWING \
		--ns=ObjCRuntime \
		$(2) \
		$(xm_$(1)_profile) \
		$(MACOS_APIS) \
		@$(RSP_DIR)/macos-defines.rsp \
		> $$@
endef

$(eval $(call MAC_GENERATOR_template,full,-d:NO_SYSTEM_DRAWING))
$(eval $(call MAC_GENERATOR_template,mobile,$(SHARED_SYSTEM_DRAWING_SOURCES)))

define MACOS_TARGETS_template
$(MACOS_BUILD_DIR)/$(1)-64/Xamarin.Mac%dll $(MACOS_BUILD_DIR)/$(1)-64/Xamarin.Mac%pdb: $(MACOS_BUILD_DIR)/$(1)/generated-sources $(MACOS_SOURCES) $(MAC_CFNETWORK_SOURCES) $(MAC_CLASSIC_SOURCES) $(SN_KEY)
	@mkdir -p $(MACOS_BUILD_DIR)/$(1)-64
	$$(call Q_PROF_CSC,mac/$(1)-64) \
		$$(MAC_$(1)_CSC) -nologo -out:$$(basename $$@).dll -target:library -debug -unsafe \
		-deterministic \
		$$(MAC_COMMON_DEFINES) \
		$$(MAC_$(1)_ARGS) \
		$$(ARGS_64) \
		-publicsign -keyfile:$(SN_KEY) \
		$(WARNINGS_TO_FIX) \
		-warnaserror:$(NULLABILITY_WARNINGS) \
		$$(MAC_CSC_FLAGS_XM) \
		$(MAC_CFNETWORK_SOURCES) $(MAC_HTTP_SOURCES) \
		$(2) \
		$$(MACOS_SOURCES) \
		@$(RSP_DIR)/macos-defines.rsp \
		@$$<

endef

$(eval $(call MACOS_TARGETS_template,mobile,$(SHARED_SYSTEM_DRAWING_SOURCES)))
$(eval $(call MACOS_TARGETS_template,full,))

$(MACOS_BUILD_DIR)/%-reference/Xamarin.Mac.dll: $(MACOS_BUILD_DIR)/%-64/Xamarin.Mac.dll
	@mkdir -p $(@D)
	$(Q) $(CP) $^ $@

$(MACOS_BUILD_DIR)/%-reference/Xamarin.Mac.pdb: $(MACOS_BUILD_DIR)/%-64/Xamarin.Mac.pdb
	@mkdir -p $(@D)
	$(Q) $(CP) $^ $@

MAC_VARIANTS_TARGETS = \
	$(MACOS_BUILD_DIR)/mobile-64/Xamarin.Mac.dll \
	$(MACOS_BUILD_DIR)/full-64/Xamarin.Mac.dll \
	$(MACOS_BUILD_DIR)/mobile-reference/Xamarin.Mac.dll \
	$(MACOS_BUILD_DIR)/full-reference/Xamarin.Mac.dll \

ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_MAC
INSTALL_TARGETS+=install-mac
ALL_TARGETS+=all-mac
endif
endif

MACOS_TARGETS_DIRS += \
	$(MACOS_BUILD_DIR) \
	$(MACOS_BUILD_DIR)/mobile \
	$(MACOS_BUILD_DIR)/mobile/Facades \
	$(MACOS_BUILD_DIR)/full \
	$(MACOS_BUILD_DIR)/compat \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/mobile    \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/full      \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full   \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/net_4_5   \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5         \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin                  \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig        \

MACOS_TARGETS += \
	$(PROJECT_DIR)/xammac.csproj                                                        \
	$(MAC_VARIANTS_TARGETS)                                                             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMac.dll                      \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.dll     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.pdb     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.dll       \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.pdb       \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/mobile/Xamarin.Mac.dll        \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/mobile/Xamarin.Mac.pdb        \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/full/Xamarin.Mac.dll          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/full/Xamarin.Mac.pdb          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.dll     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.pdb     \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.pdb             \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig/xammac.pc

# The XamMac.dll file must exist for VSfM to be able to open XM/Classic projects (so that people can use the migration wizard)
$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMac.dll: $(MACIOS_BINARIES_PATH)/XamMac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.dll: $(MACOS_BUILD_DIR)/mobile-reference/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile/Xamarin.Mac.pdb: $(MACOS_BUILD_DIR)/mobile-reference/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/mobile
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/mobile/Xamarin.Mac.dll: $(MACOS_BUILD_DIR)/mobile-64/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/mobile
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/mobile/Xamarin.Mac.pdb: $(MACOS_BUILD_DIR)/mobile-64/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/mobile
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.dll: $(MACOS_BUILD_DIR)/full-reference/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.pdb : $(MACOS_BUILD_DIR)/full-reference/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/full/Xamarin.Mac.dll: $(MACOS_BUILD_DIR)/full-64/Xamarin.Mac.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/full
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/full/Xamarin.Mac.pdb: $(MACOS_BUILD_DIR)/full-64/Xamarin.Mac.pdb | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/64bits/full
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.dll $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.pdb: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac
	$(Q) ln -sF ../../reference/mobile/$(@F) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.pdb: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5
	$(Q) ln -sF ../../reference/full/$(@F) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig/xammac.pc: $(TOP)/Make.config | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig
	$(Q) sed -e "s/@PACKAGE_VERSION@/$(MAC_PACKAGE_VERSION)/g" xammac.pc.in > $@
	$(Q) chmod 0644 $@

$(MACOS_TARGETS_DIRS):
	$(Q) mkdir -p $@

ifdef INCLUDE_MAC
install-mac: $(MACOS_TARGETS)
all-mac: $(MACOS_TARGETS)
endif

#
# Xamarin.WatchOS
#

WATCH_DEFINES = -define:IPHONE -define:MONOTOUCH -d:WATCH -d:XAMCORE_3_0 -d:__WATCHOS__ -d:SYSTEM_NET_HTTP
WATCH_GENERATOR=$(BUILD_DIR)/common/bgen.exe
WATCH_GENERATE=$(SYSTEM_MONO) --debug $(WATCH_GENERATOR)
WATCH_GENERATED_DEFINES= -d:WATCH -d:XAMCORE_3_0

WATCHOS_CORE_DEFINES = $(WATCH_DEFINES) -define:COREBUILD

WATCHOS_EXTRA_CORE_SOURCES = \
    $(WATCH_BUILD_DIR)/Constants.cs    \
    $(BUILD_DIR)/Constants.watchos.generated.cs \
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
		-e 's/@REVISION@/$(IOS_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h))/g' \
		-e "s/@WATCH_SDK_VERSION@/$(WATCH_SDK_VERSION)/g" \
		$< > $@

$(WATCH_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in $(TOP)/Make.config  $(GIT_DIRECTORY)/HEAD | $(WATCH_BUILD_DIR)
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

$(WATCH_BUILD_DIR)/watch/core.dll: $(WATCHOS_CORE_SOURCES) frameworks.sources $(RSP_DIR)/watchos-defines.rsp | $(WATCH_BUILD_DIR)/watch
	@mkdir -p $(WATCH_BUILD_DIR)/watch
	$(call Q_PROF_CSC,watch) $(WATCH_CSC) -nologo -out:$@ -target:library -debug -unsafe \
		$(CORE_WARNINGS_TO_FIX) \
		@$(RSP_DIR)/watchos-defines.rsp \
		$(WATCHOS_CORE_DEFINES)     \
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
		@$(RSP_DIR)/watchos-defines.rsp                        \
		> $@

$(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS%dll $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS%pdb: $(WATCHOS_SOURCES) $(WATCH_BUILD_DIR)/watch/generated_sources $(PRODUCT_KEY_PATH) | $(WATCH_BUILD_DIR)/watch-32
	$(call Q_PROF_CSC,watch) $(WATCH_CSC) -nologo -out:$(basename $@).dll -target:library -debug -unsafe -optimize \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) $(WATCH_DEFINES) \
		-deterministic \
		$(ARGS_32) \
		$(WARNINGS_TO_FIX) \
		@$(RSP_DIR)/watchos-defines.rsp \
		$(WATCHOS_SOURCES) @$(WATCH_BUILD_DIR)/watch/generated_sources

$(WATCH_BUILD_DIR)/watch-64/Xamarin.WatchOS%dll $(WATCH_BUILD_DIR)/watch-64/Xamarin.WatchOS%pdb $(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS%dll: $(WATCHOS_SOURCES) $(WATCH_BUILD_DIR)/watch/generated_sources $(PRODUCT_KEY_PATH) | $(WATCH_BUILD_DIR)/watch-64 $(WATCH_BUILD_DIR)/reference
	$(call Q_PROF_CSC,watch) $(WATCH_CSC) -nologo -out:$(WATCH_BUILD_DIR)/watch-64/Xamarin.WatchOS.dll -target:library -debug -unsafe -optimize \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) $(WATCH_DEFINES) \
		-deterministic \
		$(ARGS_64) \
		$(WARNINGS_TO_FIX) \
		@$(RSP_DIR)/watchos-defines.rsp \
		-refout:$(WATCH_BUILD_DIR)/reference/Xamarin.WatchOS.dll \
		$(WATCHOS_SOURCES) @$(WATCH_BUILD_DIR)/watch/generated_sources

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
	$(WATCH_BUILD_DIR)/watch-64                                \
	$(WATCH_BUILD_DIR)/reference                               \
	$(WATCH_BUILD_DIR)/reference/Facades                       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/share/doc/Xamarin.WatchOS\
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/watchOS       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/watchOS       \

WATCH_TARGETS += \
	$(PROJECT_DIR)/xamwatch.csproj                                                          \
	$(PROJECT_DIR)/MonoTouch.NUnitLite.watchos.csproj                                       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/Xamarin.WatchOS.dll          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/watchOS/Xamarin.WatchOS.dll                \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/watchOS/Xamarin.WatchOS.dll                \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/watchOS/Xamarin.WatchOS.pdb                \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/watchOS/Xamarin.WatchOS.pdb                \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/MonoTouch.NUnitLite.dll      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/MonoTouch.NUnitLite.pdb

# reference assemblies, this is just for compilation with XS
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.dll: $(WATCH_BUILD_DIR)/reference/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/Facades
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.pdb: $(WATCH_BUILD_DIR)/reference/%.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.config: $(WATCH_BUILD_DIR)/reference/%.config | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS
	$(Q) install -m 0644 $< $@

# the actual architecture-specific versions
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/watchOS/Xamarin.WatchOS.dll: $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/watchOS
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/watchOS/Xamarin.WatchOS.dll: $(WATCH_BUILD_DIR)/watch-64/Xamarin.WatchOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/watchOS
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/watchOS/Xamarin.WatchOS.pdb: $(WATCH_BUILD_DIR)/watch-32/Xamarin.WatchOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/watchOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/watchOS/Xamarin.WatchOS.pdb: $(WATCH_BUILD_DIR)/watch-64/Xamarin.WatchOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/watchOS
	$(Q) install -m 0644 $< $@

$(WATCH_TARGETS_DIRS):
	$(Q) mkdir -p $@

all-watch: $(WATCH_TARGETS)
install-watch: $(WATCH_TARGETS)

ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_WATCH
ALL_TARGETS += all-watch
INSTALL_TARGETS += install-watch
endif
endif

#
# Xamarin.TVOS
#

TVOS_DEFINES = -define:IPHONE -define:MONOTOUCH -d:TVOS -d:XAMCORE_3_0 -d:__TVOS__ -d:SYSTEM_NET_HTTP
TVOS_GENERATOR=$(BUILD_DIR)/common/bgen.exe
TVOS_GENERATE=$(SYSTEM_MONO) --debug $(TVOS_GENERATOR)

TVOS_CORE_DEFINES=$(TVOS_DEFINES) -d:COREBUILD
TVOS_GENERATOR_FLAGS = -d:TVOS -d:XAMCORE_3_0 -inline-selectors

TVOS_DOTNET_EXTRA_CORE_SOURCES = \
	$(TVOS_BUILD_DIR)/Constants.cs    \
	$(DOTNET_BUILD_DIR)/Constants.tvos.generated.cs \
	$(TVOS_BUILD_DIR)/AssemblyInfo.cs \

TVOS_EXTRA_CORE_SOURCES = \
	$(TVOS_BUILD_DIR)/Constants.cs    \
	$(BUILD_DIR)/Constants.tvos.generated.cs \
    $(TVOS_BUILD_DIR)/AssemblyInfo.cs \
	$(IOS_OPENTK_1_0_CORE_SOURCES)    \
	$(SHARED_SYSTEM_DRAWING_SOURCES) \

TVOS_DOTNET_CORE_SOURCES += \
	$(TVOS_DOTNET_EXTRA_CORE_SOURCES) \

TVOS_CORE_SOURCES +=           \
	$(TVOS_EXTRA_CORE_SOURCES) \

TVOS_DOTNET_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \
	$(SHARED_DESIGNER_CS) \

TVOS_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \
	$(SHARED_DESIGNER_CS) \

TVOS_DOTNET_SOURCES += \
	$(TVOS_DOTNET_EXTRA_CORE_SOURCES) \
	$(TVOS_HTTP_SOURCES)     \

TVOS_SOURCES +=                \
	$(TVOS_EXTRA_CORE_SOURCES) \
	$(TVOS_HTTP_SOURCES)     \

$(TVOS_BUILD_DIR)/Constants.cs: $(TOP)/src/Constants.tvos.cs.in Makefile $(TOP)/Make.config.inc | $(TVOS_BUILD_DIR)
	$(call Q_PROF_GEN,tvos) sed \
		-e "s/@VERSION@/$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)/g" \
		-e 's/@REVISION@/$(IOS_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h))/g' \
		-e "s/@TVOS_SDK_VERSION@/$(TVOS_SDK_VERSION)/g" \
		$< > $@

$(TVOS_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in $(TOP)/Make.config  $(GIT_DIRECTORY)/HEAD | $(TVOS_BUILD_DIR)
	$(call Q_PROF_GEN,tvos) sed \
		-e 's|@PRODUCT_NAME@|Xamarin.TVOS|g' \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g' \
		-e 's|@PACKAGE_VERSION_MAJOR@|$(IOS_PACKAGE_VERSION_MAJOR)|g' \
		-e 's|@PACKAGE_VERSION_MINOR@|$(IOS_PACKAGE_VERSION_MINOR)|g' \
		-e 's|@PACKAGE_VERSION_REV@|$(IOS_PACKAGE_VERSION_REV)|g' \
		-e 's|@PACKAGE_VERSION_BUILD@|$(IOS_PACKAGE_VERSION_BUILD)|g' \
		-e 's|@NUGET_VERSION_NO_METADATA@|$(TVOS_NUGET_VERSION_NO_METADATA)|g' \
		-e 's|@NUGET_VERSION_MAJOR@|$(TVOS_NUGET_VERSION_MAJOR)|g' \
		-e 's|@NUGET_VERSION_MINOR@|$(TVOS_NUGET_VERSION_MINOR)|g' \
		-e 's|@NUGET_VERSION_THIRD_DIGIT_WORKAROUND@|$(NUGET_VERSION_THIRD_DIGIT_WORKAROUND)|g' \
		-e 's|@NUGET_VERSION_REV@|$(TVOS_NUGET_VERSION_PATCH)|g' \
		-e 's|@NUGET_VERSION_BUILD@|$(TVOS_NUGET_COMMIT_DISTANCE)|g' \
		-e 's|@DOTNET_PLATFORM@|tvOS|g' \
		$< > $@.tmp
	$(Q) diff $@ $@.tmp >/dev/null 2>&1 || mv -f $@.tmp $@
	$(Q) rm -f $@.tmp
	$(Q) touch $@

$(TVOS_BUILD_DIR)/tvos/core.dll: $(TVOS_CORE_SOURCES) frameworks.sources Makefile $(RSP_DIR)/tvos-defines.rsp | $(TVOS_BUILD_DIR)/tvos
	@mkdir -p $(TVOS_BUILD_DIR)/tvos
	$(call Q_PROF_CSC,tvos) $(TV_CSC) -nologo -out:$@ -target:library -debug -unsafe \
		$(CORE_WARNINGS_TO_FIX) \
		@$(RSP_DIR)/tvos-defines.rsp \
		$(TVOS_CORE_DEFINES)     \
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
		$(TVOS_GENERATOR_FLAGS)				\
		-native-exception-marshalling                            \
		--ns:ObjCRuntime                                         \
		$(TVOS_APIS)                                            \
		--target-framework=Xamarin.TVOS,v1.0                     \
		@$(RSP_DIR)/tvos-defines.rsp                           \
		> $@

$(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS%dll $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS%pdb $(TVOS_BUILD_DIR)/reference/Xamarin.TVOS%dll: $(TVOS_SOURCES) $(TVOS_BUILD_DIR)/tvos/generated_sources $(PRODUCT_KEY_PATH) | $(TVOS_BUILD_DIR)/tvos-64 $(TVOS_BUILD_DIR)/reference
	$(call Q_PROF_CSC,tvos) $(TV_CSC) -nologo -out:$(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS.dll -target:library -debug -unsafe -optimize \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) $(TVOS_DEFINES) \
		-deterministic \
		$(ARGS_64) \
		$(WARNINGS_TO_FIX) \
		@$(RSP_DIR)/tvos-defines.rsp \
		-refout:$(TVOS_BUILD_DIR)/reference/Xamarin.TVOS.dll \
		$(TVOS_SOURCES) @$(TVOS_BUILD_DIR)/tvos/generated_sources

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
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/tvOS         \

TVOS_TARGETS += \
	$(PROJECT_DIR)/xamtvos.csproj                                                        \
	$(PROJECT_DIR)/MonoTouch.NUnitLite.tvos.csproj                                       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Xamarin.TVOS.dll             \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/tvOS/Xamarin.TVOS.dll                   \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/tvOS/Xamarin.TVOS.pdb                   \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/OpenTK-1.0.dll               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/OpenTK-1.0.pdb               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/OpenTK-1.0.dll.config        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.Dialog-1.dll       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.Dialog-1.pdb       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.NUnitLite.dll      \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/MonoTouch.NUnitLite.pdb

#
# Xamarin.MacCatalyst
#

# MacCatalyst is a variant of iOS, so it defines the iOS variables as well.
MACCATALYST_DEFINES = -define:IPHONE -define:IOS -define:MONOTOUCH -d:__IOS__ -d:__MACCATALYST__ -d:SYSTEM_NET_HTTP
MACCATALYST_GENERATOR=$(BUILD_DIR)/common/bgen.exe
MACCATALYST_GENERATE=$(SYSTEM_MONO) --debug $(MACCATALYST_GENERATOR)

MACCATALYST_CORE_DEFINES=$(MACCATALYST_DEFINES) -d:COREBUILD
MACCATALYST_GENERATOR_FLAGS = -d:__MACCATALYST__ -d:IOS -inline-selectors

MACCATALYST_DOTNET_EXTRA_CORE_SOURCES = \
	$(MACCATALYST_BUILD_DIR)/Constants.cs    \
	$(DOTNET_BUILD_DIR)/Constants.maccatalyst.generated.cs \
	$(MACCATALYST_BUILD_DIR)/AssemblyInfo.cs \

MACCATALYST_DOTNET_CORE_SOURCES += \
	$(MACCATALYST_DOTNET_EXTRA_CORE_SOURCES) \

MACCATALYST_DOTNET_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \
	$(SHARED_DESIGNER_CS) \

MACCATALYST_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \
	$(SHARED_DESIGNER_CS) \

MACCATALYST_DOTNET_SOURCES += \
	$(MACCATALYST_DOTNET_EXTRA_CORE_SOURCES) \
	$(MACCATALYST_DOTNET_HTTP_SOURCES) \

MACCATALYST_SOURCES +=                \
	$(MACCATALYST_EXTRA_CORE_SOURCES) \
	$(MACCATALYST_HTTP_SOURCES)     \

$(MACCATALYST_BUILD_DIR)/Constants.cs: $(TOP)/src/Constants.maccatalyst.cs.in Makefile $(TOP)/Make.config.inc | $(MACCATALYST_BUILD_DIR)
	$(call Q_PROF_GEN,maccatalyst) sed \
		-e "s/@VERSION@/$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)/g" \
		-e 's/@REVISION@/$(IOS_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h))/g' \
		-e "s/@MACCATALYST_SDK_VERSION@/$(MACCATALYST_SDK_VERSION)/g" \
		$< > $@

$(MACCATALYST_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in $(TOP)/Make.config  $(GIT_DIRECTORY)/HEAD | $(MACCATALYST_BUILD_DIR)
	$(call Q_PROF_GEN,maccatalyst) sed \
		-e 's|@PRODUCT_NAME@|Xamarin.MacCatalyst|g' \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g' \
		-e 's|@PACKAGE_VERSION_MAJOR@|$(IOS_PACKAGE_VERSION_MAJOR)|g' \
		-e 's|@PACKAGE_VERSION_MINOR@|$(IOS_PACKAGE_VERSION_MINOR)|g' \
		-e 's|@PACKAGE_VERSION_REV@|$(IOS_PACKAGE_VERSION_REV)|g' \
		-e 's|@PACKAGE_VERSION_BUILD@|$(IOS_PACKAGE_VERSION_BUILD)|g' \
		-e 's|@NUGET_VERSION_NO_METADATA@|$(MACCATALYST_NUGET_VERSION_NO_METADATA)|g' \
		-e 's|@NUGET_VERSION_MAJOR@|$(MACCATALYST_NUGET_VERSION_MAJOR)|g' \
		-e 's|@NUGET_VERSION_MINOR@|$(MACCATALYST_NUGET_VERSION_MINOR)|g' \
		-e 's|@NUGET_VERSION_THIRD_DIGIT_WORKAROUND@|$(NUGET_VERSION_THIRD_DIGIT_WORKAROUND)|g' \
		-e 's|@NUGET_VERSION_REV@|$(MACCATALYST_NUGET_VERSION_PATCH)|g' \
		-e 's|@NUGET_VERSION_BUILD@|$(MACCATALYST_NUGET_COMMIT_DISTANCE)|g' \
		-e 's|@DOTNET_PLATFORM@|MacCatalyst|g' \
		$< > $@.tmp
	$(Q) diff $@ $@.tmp >/dev/null 2>&1 || mv -f $@.tmp $@
	$(Q) rm -f $@.tmp
	$(Q) touch $@

$(PROJECT_DIR)/xammaccatalyst.csproj: xammaccatalyst.tmpl.csproj Makefile $(wildcard $(TOP)/*.sources)
	@sed -e 's*<!--%FILES%-->*$(foreach file,$(MACCATALYST_SOURCES),<Compile Include="$(file)"/>)*' -e 's*<!--%APIS%-->*$(foreach file,$(MACCATALYST_APIS),<None Include="$(file)"/>)*' $< | xmllint --format - > $@

$(PROJECT_DIR)/MonoTouch.NUnitLite.maccatalyst.csproj: MonoTouch.NUnitLite.maccatalyst.templ.csproj Makefile touch-unit.sources $(TOP)/Make.config $(TOP)/mk/mono.mk
	$(Q) sed -e 's*<!--%FILES%-->*$(foreach file,$(MACCATALYST_TOUCHUNIT_SOURCES),<Compile Include="$(file)"/>)*' $< | xmllint --format - > $@

MACCATALYST_TARGETS_DIRS +=                                                 \
	$(MACCATALYST_BUILD_DIR)                                                \

# reference assemblies, this is just for compilation with XS
$(MACCATALYST_TARGETS_DIRS):
	$(Q) mkdir -p $@

# reference assemblies, this is just for compilation with XS
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.dll: $(TVOS_BUILD_DIR)/reference/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/Facades
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.pdb: $(TVOS_BUILD_DIR)/reference/%.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.config: $(TVOS_BUILD_DIR)/reference/%.config | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS
	$(Q) install -m 0644 $< $@

# the actual architecture-specific versions
$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/tvOS/Xamarin.TVOS.dll: $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/tvOS
	$(Q) install -m 0755 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/tvOS/Xamarin.TVOS.pdb: $(TVOS_BUILD_DIR)/tvos-64/Xamarin.TVOS.pdb | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/tvOS
	$(Q) install -m 0644 $< $@

$(TVOS_TARGETS_DIRS):
	$(Q) mkdir -p $@

all-tvos: $(TVOS_TARGETS)
install-tvos: $(TVOS_TARGETS)

ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_TVOS
ALL_TARGETS += all-tvos
INSTALL_TARGETS += install-tvos
endif
endif

### .NET ###

define BuildDotNetIntermediateAssembly
$($(2)_DOTNET_BUILD_DIR)/core-$(3).dll: $($(2)_DOTNET_CORE_SOURCES) frameworks.sources $(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp | $($(2)_DOTNET_BUILD_DIR)
	$$(Q_DOTNET_GEN) \
		$(DOTNET_CSC) \
		$(DOTNET_FLAGS) \
		$(DOTNET_CORE_WARNINGS_TO_FIX) \
		@$(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp \
		$($(2)_CORE_DEFINES) \
		$($(2)_DOTNET_CORE_SOURCES) \
		-nullable+ \
		-warnaserror:nullable \
		-out:$$@

$($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources: $(DOTNET_GENERATOR) $($(2)_DOTNET_APIS) $($(2)_DOTNET_BUILD_DIR)/core-$(3).dll $(DOTNET_BINDING_ATTRIBUTES) $($(2)_DOTNET_BUILD_DIR)/$(3).rsp | $($(2)_DOTNET_BUILD_DIR)/generated-sources
	$$(Q_DOTNET_GEN) $$< @$($(2)_DOTNET_BUILD_DIR)/$(3).rsp

$($(2)_DOTNET_BUILD_DIR)/$(3).rsp: Makefile Makefile.generator frameworks.sources $(DOTNET_COMPILER) | $($(2)_DOTNET_BUILD_DIR)
	$(Q) echo \
		$($(2)_GENERATOR_FLAGS) \
		$(DOTNET_GENERATOR_FLAGS) \
		-warnaserror:$($(2)_GENERATOR_WARNASERROR) \
		-sourceonly=$($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources \
		-tmpdir=$($(2)_DOTNET_BUILD_DIR)/generated-sources \
		-baselib=$($(2)_DOTNET_BUILD_DIR)/core-$(3).dll \
		--target-framework=.NETCoreApp,Version=$(GENERATOR_TF_VERSION),Profile=$(3) \
		$($(2)_APIS) \
		@$(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp \
		> $$@

DOTNET_TARGETS_$(3) += \
	$($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).dll \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).xml \

DOTNET_TARGETS_DIRS_$(3) += \
	$($(2)_DOTNET_BUILD_DIR) \
	$($(2)_DOTNET_BUILD_DIR)/generated-sources \
	$($(2)_DOTNET_BUILD_DIR)/ref \
	$($(2)_DOTNET_BUILD_DIR)/doc \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM) \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/doc/$(DOTNET_TFM) \

dotnet-gen-$(3):: $($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources
dotnet-gen:: dotnet-gen-$(3)

$($(2)_DOTNET_BUILD_DIR)/ILLink.LinkAttributes.xml: $(TOP)/src/ILLink.LinkAttributes.xml.in | $($(2)_DOTNET_BUILD_DIR)
	$$(call Q_PROF_GEN,$(3)) sed < $$< > $$@ 's|@PRODUCT_NAME@|Microsoft.$(1)|g;'

$($(2)_DOTNET_BUILD_DIR)/ILLink.Substitutions.xml: $(TOP)/src/ILLink.Substitutions.$(1).xml | $($(2)_DOTNET_BUILD_DIR)
	$(Q) $(CP) $$< $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).dll: $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll | $(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)
	$(Q) $(CP) $$< $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).xml: $($(2)_DOTNET_BUILD_DIR)/doc/Microsoft.$(1).xml | $(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)
	$(Q) $(CP) $$< $$@

$($(2)_DOTNET_BUILD_DIR)/doc/Microsoft.$(1).xml: $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll build/.build-adr-stamp | $($(2)_DOTNET_BUILD_DIR)/doc
ifdef ENABLE_XAMARIN
	$(Q) $(MAKE) run -C $(MACCORE_PATH)/tools/apple-doc-reader ASSEMBLY="$(abspath $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll)" INPUT="$(abspath $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml)" OUTPUT="$(abspath $$@)"
else
	$$(Q) $$(CP) $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml $$@
endif
endef

build/.build-adr-stamp:
ifdef ENABLE_XAMARIN
	$(Q) $(MAKE) build -C $(MACCORE_PATH)/tools/apple-doc-reader
	$(Q) $(MAKE) create -C $(MACCORE_PATH)/tools/apple-doc-reader
endif
	$(Q) touch $@

# Template variables:
# 1: Platform (correct case)
# 2: PLATFORM (all upper case)
# 3: platform (all lower case)
# pseudo-code:
#     foreach (var platform in DOTNET_PLATFORMS)
#        BuildDotNetIntermediateAssembly (platform, platform.ToUpper (), platform.ToLower ())
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call BuildDotNetIntermediateAssembly,$(platform),$(shell echo $(platform) | tr '[:lower:]' '[:upper:]'),$(shell echo $(platform) | tr '[:upper:]' '[:lower:]'))))

define BuildDotNetPlatformAssembly
## We build the ref assembly when building the 64-bit version of the platform assembly,
## so we need some special logic here.
ifeq ($(4),64)
$(2)_$(4)_REFOUT_ARG = -refout:$($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll
$(2)_$(4)_REF_TARGET = $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1)%dll
$(2)_$(4)_DOC_ARG = -doc:$($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml
$(2)_$(4)_DOC_TARGET = $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1)%xml
endif

$(2)_DOTNET_PLATFORM_ASSEMBLY_DEPENDENCIES = \
	$($(2)_DOTNET_SOURCES) \
	$($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources \
	$($(2)_DOTNET_BUILD_DIR)/ILLink.LinkAttributes.xml \
	$($(2)_DOTNET_BUILD_DIR)/ILLink.Substitutions.xml \
	$(PRODUCT_KEY_PATH) \

$(2)_DOTNET_PLATFORM_ASSEMBLY_DIR_DEPENDENCIES = \
	$($(2)_DOTNET_BUILD_DIR)/$(4) \
	$($(2)_DOTNET_BUILD_DIR)/ref \

$($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1)%dll $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1)%pdb $$($(2)_$(4)_REF_TARGET) $$($(2)_$(4)_DOC_TARGET): $$($(2)_DOTNET_PLATFORM_ASSEMBLY_DEPENDENCIES) | $$($(2)_DOTNET_PLATFORM_ASSEMBLY_DIR_DEPENDENCIES)
	$$(call Q_PROF_CSC,dotnet/$(4)-bit) \
		$(DOTNET_CSC)  \
		$(DOTNET_FLAGS) \
		-unsafe \
		-optimize \
		$$(ARGS_$(1)) \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) \
		$$($(2)_$(4)_REFOUT_ARG) \
		$$($(2)_$(4)_DOC_ARG) \
		$$($(2)_DEFINES) \
		$(ARGS_$(4)) \
		$$(DOTNET_WARNINGS_TO_FIX) \
		@$(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp \
		-res:$($(2)_DOTNET_BUILD_DIR)/ILLink.LinkAttributes.xml \
		-res:$($(2)_DOTNET_BUILD_DIR)/ILLink.Substitutions.xml \
		-warnaserror:$(NULLABILITY_WARNINGS) \
		-nullable+ \
		$$($(2)_DOTNET_SOURCES) \
		@$($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources \
		-out:$($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll \

dotnet-$(3):: $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll

DOTNET_TARGETS_$(3) += \
	$($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll \
	$(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM)/Microsoft.$(1).dll) \
	$(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM)/Microsoft.$(1).pdb) \

DOTNET_TARGETS_DIRS_$(3) += \
	$($(2)_DOTNET_BUILD_DIR)/$(4) \
	$(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM)) \

$(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM)/Microsoft.$(1).dll): $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll | $(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM))
	$(Q) $(CP) $$< $$@

$(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM)/Microsoft.$(1).pdb): $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).pdb | $(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM))
	$(Q) $(CP) $$< $$@

DOTNET_TARGETS += $$(DOTNET_TARGETS_$(3))
DOTNET_TARGETS_DIRS += $$(DOTNET_TARGETS_DIRS_$(3))

dotnet-$(3):: $$(DOTNET_TARGETS_$(3))

endef

# Template variables:
# 1: Platform (correct case)
# 2: PLATFORM (all upper case)
# 3: platform (all lower case)
# 4: 32 or 64 (architecture bitness)
#
# pseudo-code:
#     foreach (var platform in DOTNET_PLATFORMS)
#        foreach (var bitness in DOTNET_<platform.ToUpper ()>_BITNESSES)
#            BuildDotNetPlatformAssembly (platform, platform.ToUpper (), platform.ToLower (), bitness)
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach bitness, $(DOTNET_$(shell echo $(platform) | tr '[:lower:]' '[:upper:]')_BITNESSES), $(eval $(call BuildDotNetPlatformAssembly,$(platform),$(shell echo $(platform) | tr '[:lower:]' '[:upper:]'),$(shell echo $(platform) | tr '[:upper:]' '[:lower:]'),$(bitness)))))

#
# Global targets
#

SHARED_PATH := ../runtime
$(SHARED_PATH)/Delegates.generated.cs: $(SHARED_PATH)/Delegates.cs.t4 $(SHARED_PATH)/delegates.t4
	$(Q) $(MAKE) -C $(SHARED_PATH) Delegates.generated.cs

$(BUILD_DIR)/common/NativeTypes/%.cs: $(TOP)/src/NativeTypes/%.tt | $(BUILD_DIR)/common/NativeTypes
	$(Q_GEN) $(TT) $(abspath $<) -o $(abspath $@) 1>/dev/null

$(COMMON_TARGET_DIRS):
	$(Q) mkdir -p $@

$(DOTNET_COMPILER): Makefile $(TOP)/Make.config | $(DOTNET_BUILD_DIR)
	$(Q) echo "#!/bin/zsh -e" > $@
	$(Q) echo "exec $(DOTNET_CSC) $(DOTNET_FLAGS) \"\$$@\"" >> $@
	$(Q) chmod +x $@

GENERATE_FRAMEWORKS_CONSTANTS=generate-frameworks-constants/legacy/bin/Debug/generate-frameworks-constants.exe
DOTNET_GENERATE_FRAMEWORKS_CONSTANTS=generate-frameworks-constants/dotnet/bin/Debug/$(DOTNET_TFM)/generate-frameworks-constants.dll

$(GENERATE_FRAMEWORKS_CONSTANTS): $(wildcard generate-frameworks-constants/*.cs*) $(TOP)/tools/common/Frameworks.cs Makefile
	$(Q) $(SYSTEM_MSBUILD) "/bl:$@.binlog" /r generate-frameworks-constants/legacy/generate-frameworks-constants.csproj $(MSBUILD_VERBOSITY)
	$(Q) touch $@ # Running MSBuild doesn't always touch the target, so make sure we do here, otherwise make can end up confused.

$(DOTNET_GENERATE_FRAMEWORKS_CONSTANTS): $(wildcard generate-frameworks-constants/*.cs*) $(TOP)/tools/common/Frameworks.cs Makefile
	$(Q) $(DOTNET) build "/bl:$@.binlog" /r generate-frameworks-constants/dotnet/generate-frameworks-constants.csproj $(MSBUILD_VERBOSITY)
	$(Q) touch $@ # Running 'dotnet build' doesn't always touch the target, so make sure we do here, otherwise make can end up confused.

# This rule means: generate a Constants.<platform>.generated.cs for the frameworks in the variable <PLATFORM>_FRAMEWORKS
$(BUILD_DIR)/Constants.%.generated.cs: Makefile $(GENERATE_FRAMEWORKS_CONSTANTS) | $(BUILD_DIR)
	$(Q) mono --debug $(GENERATE_FRAMEWORKS_CONSTANTS) "$*" "$@.tmp"
	$(Q) mv "$@.tmp" "$@"

# This rule means: generate a Constants.<platform>.generated.cs for the frameworks in the variable <PLATFORM>_FRAMEWORKS
$(DOTNET_BUILD_DIR)/Constants.%.generated.cs: Makefile $(DOTNET_GENERATE_FRAMEWORKS_CONSTANTS) | $(DOTNET_BUILD_DIR)
	$(Q) $(DOTNET) $(DOTNET_GENERATE_FRAMEWORKS_CONSTANTS) "$*" "$@.tmp"
	$(Q) mv "$@.tmp" "$@"

install-local:: $(INSTALL_TARGETS)
all-local:: $(ALL_TARGETS)

$(DOTNET_TARGETS_DIRS):
	$(Q) mkdir -p $@

ifdef ENABLE_DOTNET
define DotNetProjectFiles
$(DOTNET_BUILD_DIR)/projects/$(1)/$(1).csproj: dotnet.tmpl.csproj Makefile $$(wildcard $(CURDIR)/*.sources)
	@mkdir -p $$(dir $$@)
	@sed \
		-e 's*%DOTNET_TFM%*$(DOTNET_TFM)*' \
		-e 's*%PLATFORM%*$(1)*' \
		-e 's*<!--%FILES%-->*$$(foreach file,$$($(2)_DOTNET_SOURCES),<Compile Include="../../../../$$(file)" Link="sources/$$(file)" />)*' \
		-e 's*<!--%APIS%-->*$$(foreach file,$$($(2)_DOTNET_APIS),<None Include="../../../../$$(file)" Link="apis/$$(file)" />)*' \
		$$< | xmllint --format - > $$@
PROJECT_FILES += $(DOTNET_BUILD_DIR)/projects/$(1)/$(1).csproj
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetProjectFiles,$(platform),$(shell echo $(platform) | tr '[:lower:]' '[:upper:]'))))
endif

dotnet: $(DOTNET_TARGETS)
ifdef ENABLE_DOTNET
all-local:: $(DOTNET_TARGETS)
endif

project-files: $(PROJECT_FILES)

$(TOP)/tools/common/SdkVersions.cs: $(TOP)/tools/common/SdkVersions.in.cs
	@$(MAKE) -C $(TOP)/tools/mtouch ../common/SdkVersions.cs

COMMA=,
MinimumVersions.cs: MinimumVersions.cs.in Makefile $(TOP)/Make.config
	$(Q_GEN) sed \
		-e 's/@DOTNET_MIN_IOS_SDK_VERSION@/$(DOTNET_MIN_IOS_SDK_VERSION)/g' \
		-e 's/@DOTNET_MIN_TVOS_SDK_VERSION@/$(DOTNET_MIN_TVOS_SDK_VERSION)/' \
		-e 's/@DOTNET_MIN_MACOS_SDK_VERSION@/$(DOTNET_MIN_MACOS_SDK_VERSION)/' \
		-e 's/@DOTNET_MIN_MACCATALYST_SDK_VERSION@/$(DOTNET_MIN_MACCATALYST_SDK_VERSION)/g' \
		\
		-e 's/@MIN_IOS_SDK_VERSION@/$(subst .,$(COMMA) ,$(MIN_IOS_SDK_VERSION))/g' \
		-e 's/@MIN_MACOS_SDK_VERSION@/$(subst .,$(COMMA) ,$(MIN_MACOS_SDK_VERSION))/g' \
		-e 's/@MIN_WATCHOS_SDK_VERSION@/$(subst .,$(COMMA) ,$(MIN_WATCHOS_SDK_VERSION))/g' \
		-e 's/@MIN_TVOS_SDK_VERSION@/$(subst .,$(COMMA) ,$(MIN_TVOS_SDK_VERSION))/g' \
		-e 's/@MIN_MACCATALYST_SDK_VERSION@/$(subst .,$(COMMA) ,$(MIN_MACCATALYST_SDK_VERSION))/g' \
		$< > $@

# Using .SECONDARY can cause make to go into an infinite loop.
# See https://github.com/xamarin/maccore/issues/762.
#.SECONDARY:
