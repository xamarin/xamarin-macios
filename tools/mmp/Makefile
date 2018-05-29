TOP=../..
include $(TOP)/Make.config

CECIL_PATH=$(MONO_PATH)/external/cecil
CECIL_CONFIG=net_4_0_Release

MONO_CECIL_DLL=$(CECIL_PATH)/bin/$(CECIL_CONFIG)/Mono.Cecil.dll
MONO_CECIL_MDB_DLL=$(CECIL_PATH)/bin/$(CECIL_CONFIG)/Mono.Cecil.Mdb.dll
MONO_CECIL_FILES=$(wildcard $(CECIL_PATH)/*.cs) $(wildcard $(CECIL_PATH)/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*/*/*.cs)

$(MONO_CECIL_DLL): $(CECIL_PATH)/Mono.Cecil.csproj $(MONO_CECIL_FILES)
	$(Q_XBUILD) cd $(CECIL_PATH) && $(SYSTEM_XBUILD) /p:Configuration=$(CECIL_CONFIG) Mono.Cecil.csproj $(XBUILD_VERBOSITY)

$(MONO_CECIL_MDB_DLL): $(MONO_CECIL_DLL) $(CECIL_PATH)/symbols/mdb/Mono.Cecil.Mdb.csproj $(MONO_CECIL_FILES)
	$(Q_XBUILD) cd $(CECIL_PATH) && $(SYSTEM_XBUILD) /p:Configuration=$(CECIL_CONFIG) symbols/mdb/Mono.Cecil.Mdb.csproj $(XBUILD_VERBOSITY)

BCL_LOCATION=$(MONO_PATH)/mcs/class/lib/xammac_net_4_5

LINKER_DIR=$(LINKER_TOOLS_PATH)/linker
TUNER_DIR=$(LINKER_TOOLS_PATH)/tuner

linker_sources = \
	$(LINKER_DIR)/Linker/Pipeline.cs				\
	$(LINKER_DIR)/Linker/AssemblyResolver.cs			\
	$(LINKER_DIR)/Linker/TypePreserve.cs			\
	$(LINKER_DIR)/Linker/TypeNameParser.cs			\
	$(LINKER_DIR)/Linker/Annotations.cs			\
	$(LINKER_DIR)/Linker/LinkContext.cs			\
	$(LINKER_DIR)/Linker/ILogger.cs				\
	$(LINKER_DIR)/Linker/ConsoleLogger.cs			\
	$(LINKER_DIR)/Linker/LoadException.cs			\
	$(LINKER_DIR)/Linker/MarkException.cs			\
	$(LINKER_DIR)/Linker/AssemblyAction.cs			\
	$(LINKER_DIR)/Linker/I18nAssemblies.cs			\
	$(LINKER_DIR)/Linker/MethodAction.cs			\
	$(LINKER_DIR)/Linker/XApiReader.cs				\
	$(LINKER_DIR)/Linker/IXApiVisitor.cs			\
	$(LINKER_DIR)/Linker/MethodReferenceExtensions.cs		\
	$(LINKER_DIR)/Linker/MarkingHelpers.cs		\
	$(LINKER_DIR)/Linker/Tracer.cs		\
	$(LINKER_DIR)/Linker/TypeReferenceExtensions.cs		\
	$(LINKER_DIR)/Linker/AssemblyUtilities.cs	\
	$(LINKER_DIR)/Linker.Steps/BaseStep.cs			\
	$(LINKER_DIR)/Linker.Steps/LoadReferencesStep.cs		\
	$(LINKER_DIR)/Linker.Steps/LoadI18nAssemblies.cs		\
	$(LINKER_DIR)/Linker.Steps/ResolveFromXmlStep.cs		\
	$(LINKER_DIR)/Linker.Steps/SweepStep.cs			\
	$(LINKER_DIR)/Linker.Steps/RegenerateGuidStep.cs		\
	$(LINKER_DIR)/Linker.Steps/ResolveFromXApiStep.cs		\
	$(LINKER_DIR)/Linker.Steps/ResolveFromAssemblyStep.cs	\
	$(LINKER_DIR)/Linker.Steps/ResolveStep.cs			\
	$(LINKER_DIR)/Linker.Steps/CleanStep.cs			\
	$(LINKER_DIR)/Linker.Steps/MarkStep.cs			\
	$(LINKER_DIR)/Linker.Steps/BlacklistStep.cs		\
	$(LINKER_DIR)/Linker.Steps/OutputStep.cs			\
	$(LINKER_DIR)/Linker.Steps/TypeMapStep.cs			\
	$(LINKER_DIR)/Linker.Steps/IStep.cs

tuner_sources = \
	$(linker_sources)						\
	$(TUNER_DIR)/Mono.Tuner/ApplyPreserveAttributeBase.cs		\
	$(TUNER_DIR)/Mono.Tuner/Dispatcher.cs				\
	$(TUNER_DIR)/Mono.Tuner/CecilRocks.cs				\
	$(TUNER_DIR)/Mono.Tuner/CustomizeActions.cs			\
	$(TUNER_DIR)/Mono.Tuner/Extensions.cs				\
	$(TUNER_DIR)/Mono.Tuner/MarkNSObjectsBase.cs			\
	$(TUNER_DIR)/Mono.Tuner/PreserveCrypto.cs			\
	$(TUNER_DIR)/Mono.Tuner/PreserveSoapHttpClients.cs		\
	$(TUNER_DIR)/Mono.Tuner/Profile.cs				\
	$(TUNER_DIR)/Mono.Tuner/RemoveAttributesBase.cs			\
	$(TUNER_DIR)/Mono.Tuner/RemoveResources.cs			\
	$(TUNER_DIR)/Mono.Tuner/RemoveSecurity.cs			\
	linker/MonoMac.Tuner/MacMobileProfile.cs			\
	linker/MonoMac.Tuner/MacRemoveResources.cs			\
	linker/MonoMac.Tuner/MacBaseProfile.cs				\
	linker/MonoMac.Tuner/MonoMacProfile.cs				\
	linker/MonoMac.Tuner/XamarinMacProfile.cs			\
	linker/MonoMac.Tuner/OptimizeGeneratedCodeSubStep.cs		\
	linker/MonoMac.Tuner/MonoMacNamespaces.cs			\
	linker/MonoMac.Tuner/MonoMacPreserveCode.cs			\
	linker/MonoMac.Tuner/MonoMacMarkStep.cs				\
	$(TOP)/tools/linker/ApplyPreserveAttribute.cs		\
	$(TOP)/tools/linker/BaseProfile.cs			\
	$(TOP)/tools/linker/CoreMarkStep.cs			\
	$(TOP)/tools/linker/CoreOptimizeGeneratedCode.cs	\
	$(TOP)/tools/linker/CorePreserveCode.cs		\
	$(TOP)/tools/linker/CoreRemoveAttributes.cs		\
	$(TOP)/tools/linker/CoreHttpMessageHandler.cs		\
	$(TOP)/tools/linker/CoreRemoveSecurity.cs		\
	$(TOP)/tools/linker/CoreSweepStep.cs		\
	$(TOP)/tools/linker/CoreTypeMapStep.cs		\
	$(TOP)/tools/linker/ObjCExtensions.cs			\
	$(TOP)/tools/linker/MarkNSObjects.cs			\
	$(TOP)/tools/linker/MobileExtensions.cs		\
	$(TOP)/tools/linker/MobileMarkStep.cs			\
	$(TOP)/tools/linker/MobileProfile.cs			\
	$(TOP)/tools/linker/MobileRemoveAttributes.cs		\
	$(TOP)/tools/linker/MobileSweepStep.cs			\
	$(TOP)/tools/linker/RemoveSelectors.cs			\
	$(TOP)/tools/linker/ExceptionalSubStep.cs		\
	$(TOP)/tools/linker/RemoveUserResourcesSubStep.cs \
	../linker/MonoTouch.Tuner/Extensions.cs	\
	../linker/MonoTouch.Tuner/ListExportedSymbols.cs	\
	../linker/MonoTouch.Tuner/ProcessExportedFields.cs	\
	../linker/MonoTouch.Tuner/PreserveSmartEnumConversionsSubStep.cs		\
	../../src/build/mac/Constants.cs

linker_resources = \
	linker/Descriptors/mscorlib.xml

mmp_sources = \
	$(TOP)/tools/common/cache.cs    \
	driver.cs 	\
	aot.cs 	\
	$(TOP)/src/ObjCRuntime/ErrorHelper.cs \
	$(TOP)/src/ObjCRuntime/ExceptionMode.cs \
	error.cs	\
	resolver.cs	\
	$(MONO_PATH)/mcs/class/Mono.Options/Mono.Options/Options.cs	\
	Tuning.cs \
	$(TOP)/tools/common/PInvokeWrapperGenerator.cs	\
	$(TOP)/tools/common/TargetFramework.cs \
	$(TOP)/tools/common/Driver.cs \
	$(TOP)/tools/common/Frameworks.cs \
	$(TOP)/tools/common/StaticRegistrar.cs \
	$(TOP)/tools/common/Symbols.cs    \
	$(TOP)/tools/common/MachO.cs           \
	$(TOP)/src/ObjCRuntime/Registrar.core.cs	\
	$(TOP)/src/ObjCRuntime/Registrar.cs \
	$(TOP)/src/ObjCRuntime/RuntimeOptions.cs \
	Assembly.cs \
	Application.cs \
	SdkVersions.cs \
	$(TOP)/tools/common/Target.cs \
	$(TOP)/tools/common/Application.cs \
	$(TOP)/tools/common/Assembly.cs \
	$(TOP)/tools/common/CoreResolver.cs \
	$(TOP)/tools/common/DerivedLinkContext.cs \
	$(TOP)/tools/common/StringUtils.cs \
	$(TOP)/tools/common/Optimizations.cs \
	$(TOP)/src/Foundation/ConnectAttribute.cs \
	$(TOP)/src/Foundation/ExportAttribute.cs \
	$(TOP)/src/ObjCRuntime/ArgumentSemantic.cs \
	$(TOP)/src/ObjCRuntime/BindingImplAttribute.cs	\
	$(TOP)/src/ObjCRuntime/LinkWithAttribute.cs \
	$(TOP)/src/ObjCRuntime/PlatformAvailability2.cs \

DEFINES = -d:MONOMAC -d:MMP $(APPLETLS_DEFINES)

LOCAL_MMP = \
	mmp.exe \
	Mono.Cecil.dll \
	Mono.Cecil.Mdb.dll \

mmp.exe: Makefile $(MONO_CECIL_DLL) $(MONO_CECIL_MDB_DLL) $(mmp_sources) config config_mobile Info.plist.tmpl Info-framework.plist.tmpl $(tuner_sources) $(linker_resources)
	$(Q_CSC) $(SYSTEM_CSC) -unsafe -out:mmp.exe $(DEFINES) -r:$(MONO_CECIL_DLL) -r:$(MONO_CECIL_MDB_DLL) \
	-noconfig -nostdlib -r:$(MONO_PATH)/external/binary-reference-assemblies/mono/Mono.Security.dll -r:$(MONO_PATH)/external/binary-reference-assemblies/v4.7/mscorlib.dll -r:$(MONO_PATH)/external/binary-reference-assemblies/v4.6.1/System.Xml.dll -r:$(MONO_PATH)/external/binary-reference-assemblies/v4.6.1/System.Core.dll -r:$(MONO_PATH)/external/binary-reference-assemblies/v4.7/System.dll \
	-resource:config -resource:config_mobile -resource:machine.4_5.config -resource:Info.plist.tmpl -resource:Info-framework.plist.tmpl $(linker_resources:%=-resource:%) $(tuner_sources) $(mmp_sources) -debug:portable -nologo -deterministic
	$(Q) cp $(MONO_CECIL_DLL) $(MONO_CECIL_MDB_DLL) .

Mono.Cecil.dll: $(MONO_CECIL_DLL)
	$(Q) cp $<* $(dir $@)

Mono.Cecil.Mdb.dll: $(MONO_CECIL_MDB_DLL)
	$(Q) cp $<* $(dir $@)

MMP_TARGETS = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mmp \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/mmp.exe \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Mono.Cecil.dll \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Mono.Cecil.Mdb.dll \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.mobile.a \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.full.a \

MMP_DIRECTORIES = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp \

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mmp: mmp.in | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin
	$(Q_GEN) sed 's!@XAMARIN_MAC_PREFIX@!$(abspath $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR))!g' $< > $@
	$(Q) chmod +x $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/mmp.exe: mmp.exe | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp
	$(Q) cp $< $(dir $@)
	$(Q) cp $(<:.exe=.pdb) $(dir $@)

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Mono.Cecil.dll: $(MONO_CECIL_DLL) | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp
	$(Q) cp $< $(dir $@)
	$(Q) cp $(<:.exe=.pdb) $(dir $@)

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Mono.Cecil.Mdb.dll: $(MONO_CECIL_MDB_DLL) | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp
	$(Q) cp $< $(dir $@)
	$(Q) cp $(<:.exe=.pdb) $(dir $@)

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.%.a: Xamarin.Mac.registrar.%.a | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp
	$(Q) cp $<* $(dir $@)

$(MMP_DIRECTORIES):
	$(Q) mkdir -p $@

GENERATE_PART_REGISTRAR = $(Q_GEN) $(SYSTEM_MONO) --debug mmp.exe --xamarin-framework-directory=$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR) -q --runregistrar:$(abspath $@) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(OSX_SDK_VERSION)   $< --registrar:static

Xamarin.Mac.registrar.mobile.i386.m:   $(TOP)/src/build/mac/mobile-32/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --target-framework Xamarin.Mac,Version=v2.0,Profile=Mobile --arch=i386
	$(Q) touch Xamarin.Mac.registrar.mobile.i386.m Xamarin.Mac.registrar.mobile.i386.h

Xamarin.Mac.registrar.mobile.x86_64.m: $(TOP)/src/build/mac/mobile-64/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --target-framework Xamarin.Mac,Version=v2.0,Profile=Mobile --arch=x86_64
	$(Q) touch Xamarin.Mac.registrar.mobile.x86_64.m Xamarin.Mac.registrar.mobile.x86_64.h

Xamarin.Mac.registrar.full.i386.m:     $(TOP)/src/build/mac/full-32/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --target-framework 4.5 --arch=i386 --xamarin-full-framework --nolink
	$(Q) touch Xamarin.Mac.registrar.full.i386.m Xamarin.Mac.registrar.full.i386.h

Xamarin.Mac.registrar.full.x86_64.m:   $(TOP)/src/build/mac/full-64/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --target-framework 4.5 --arch=x86_64 --xamarin-full-framework --nolink
	$(Q) touch Xamarin.Mac.registrar.full.x86_64.m Xamarin.Mac.registrar.full.x86_64.h

Xamarin.Mac.registrar.%.i386.a:   Xamarin.Mac.registrar.%.i386.m
	$(Q_CC) xcrun -sdk macosx clang -DDEBUG -g -gdwarf-2 -x objective-c++ -o $@ -c -arch i386 $< -Wall -Wno-unguarded-availability-new -I$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/include -mmacosx-version-min=$(MIN_OSX_SDK_VERSION)

Xamarin.Mac.registrar.%.x86_64.a: Xamarin.Mac.registrar.%.x86_64.m
	$(Q_CC) xcrun -sdk macosx clang -DDEBUG -g -gdwarf-2 -x objective-c++ -o $@ -c -arch x86_64 $< -Wall -Wno-unguarded-availability-new -I$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/include -mmacosx-version-min=$(MIN_OSX_SDK_VERSION) -fobjc-runtime=macosx

Xamarin.Mac.registrar.%.a: Xamarin.Mac.registrar.%.i386.a Xamarin.Mac.registrar.%.x86_64.a
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^


ifdef INCLUDE_MAC
install-local:: $(MMP_TARGETS)
all-local:: $(MMP_TARGETS)
endif

clean-local::
	rm -f mmp mmp.exe mmp.exe.mdb mmp-uninstalled
	rm -f mmp.stub.c mmp.helper.o
	rm -f output/*
	rm -f $(LOCAL_MMP)
	rm -f *.mdb
	rm -f Xamarin.Mac.registrar.*
	$(SYSTEM_XBUILD) /t:Clean /p:Configuration=$(CECIL_CONFIG) $(CECIL_PATH)/Mono.Cecil.csproj
	$(SYSTEM_XBUILD) /t:Clean /p:Configuration=$(CECIL_CONFIG) $(CECIL_PATH)/symbols/mdb/Mono.Cecil.Mdb.csproj

include $(TOP)/mk/rules.mk
include ../common/Make.common

run::
	$(Q) make -C tests

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# marking those files as .SECONDARY will prevent that deletion.
.SECONDARY: $(foreach ext,h m a,Xamarin.Mac.registrar.mobile.i386.$(ext) Xamarin.Mac.registrar.mobile.x86_64.$(ext) Xamarin.Mac.registrar.full.i386.$(ext) Xamarin.Mac.registrar.full.x86_64.$(ext))
.SECONDARY: Xamarin.Mac.registrar.mobile.a Xamarin.Mac.registrar.full.a
