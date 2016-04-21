TOP=../..
include $(TOP)/Make.config

CECIL_PATH=$(MONO_PATH)/external/cecil
CECIL_CONFIG=net_4_0_Release

MONO_CECIL_DLL=$(CECIL_PATH)/bin/$(CECIL_CONFIG)/Mono.Cecil.dll
MONO_CECIL_MDB_DLL=$(CECIL_PATH)/bin/$(CECIL_CONFIG)/Mono.Cecil.Mdb.dll

$(MONO_CECIL_DLL): $(CECIL_PATH)/Mono.Cecil.csproj
	$(Q_XBUILD) cd $(CECIL_PATH) && $(SYSTEM_XBUILD) /p:Configuration=$(CECIL_CONFIG) Mono.Cecil.csproj $(XBUILD_VERBOSITY)

$(MONO_CECIL_MDB_DLL): $(MONO_CECIL_DLL) $(CECIL_PATH)/symbols/mdb/Mono.Cecil.Mdb.csproj
	$(Q_XBUILD) cd $(CECIL_PATH) && $(SYSTEM_XBUILD) /p:Configuration=$(CECIL_CONFIG) symbols/mdb/Mono.Cecil.Mdb.csproj $(XBUILD_VERBOSITY)

BCL_LOCATION=$(MONO_PATH)/mcs/class/lib/xammac_net_4_5

TOOLS_DIR=$(MONO_PATH)/mcs/tools
LINKER_DIR=$(TOOLS_DIR)/linker
TUNER_DIR=$(TOOLS_DIR)/tuner

XAMMAC_DLL=$(TOP)/src/build/mac/full-64/Xamarin.Mac.dll

linker_sources = \
	$(LINKER_DIR)/Mono.Linker/Pipeline.cs				\
	$(LINKER_DIR)/Mono.Linker/AssemblyResolver.cs			\
	$(LINKER_DIR)/Mono.Linker/TypePreserve.cs			\
	$(LINKER_DIR)/Mono.Linker/Annotations.cs			\
	$(LINKER_DIR)/Mono.Linker/LinkContext.cs			\
	$(LINKER_DIR)/Mono.Linker/AssemblyAction.cs			\
	$(LINKER_DIR)/Mono.Linker/I18nAssemblies.cs			\
	$(LINKER_DIR)/Mono.Linker/MethodAction.cs			\
	$(LINKER_DIR)/Mono.Linker/XApiReader.cs				\
	$(LINKER_DIR)/Mono.Linker/IXApiVisitor.cs			\
	$(LINKER_DIR)/Mono.Linker.Steps/BaseStep.cs			\
	$(LINKER_DIR)/Mono.Linker.Steps/LoadReferencesStep.cs		\
	$(LINKER_DIR)/Mono.Linker.Steps/LoadI18nAssemblies.cs		\
	$(LINKER_DIR)/Mono.Linker.Steps/ResolveFromXmlStep.cs		\
	$(LINKER_DIR)/Mono.Linker.Steps/SweepStep.cs			\
	$(LINKER_DIR)/Mono.Linker.Steps/RegenerateGuidStep.cs		\
	$(LINKER_DIR)/Mono.Linker.Steps/ResolveFromXApiStep.cs		\
	$(LINKER_DIR)/Mono.Linker.Steps/ResolveFromAssemblyStep.cs	\
	$(LINKER_DIR)/Mono.Linker.Steps/ResolveStep.cs			\
	$(LINKER_DIR)/Mono.Linker.Steps/CleanStep.cs			\
	$(LINKER_DIR)/Mono.Linker.Steps/MarkStep.cs			\
	$(LINKER_DIR)/Mono.Linker.Steps/BlacklistStep.cs		\
	$(LINKER_DIR)/Mono.Linker.Steps/OutputStep.cs			\
	$(LINKER_DIR)/Mono.Linker.Steps/TypeMapStep.cs			\
	$(LINKER_DIR)/Mono.Linker.Steps/IStep.cs

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
	linker/MonoMac.Tuner/MonoMacProfile.cs				\
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
	$(TOP)/tools/linker/CoreTlsProviderStep.cs		\
	$(TOP)/tools/linker/CoreRemoveSecurity.cs		\
	$(TOP)/tools/linker/ObjCExtensions.cs			\
	$(TOP)/tools/linker/MarkNSObjects.cs			\
	$(TOP)/tools/linker/MobileExtensions.cs		\
	$(TOP)/tools/linker/MobileMarkStep.cs			\
	$(TOP)/tools/linker/MobileProfile.cs			\
	$(TOP)/tools/linker/MobileRemoveAttributes.cs		\
	$(TOP)/tools/linker/MobileSweepStep.cs			\
	$(TOP)/tools/linker/RemoveSelectors.cs			\
	../linker/MonoTouch.Tuner/RemoveMonoTouchResources.cs	\
	../linker/MonoTouch.Tuner/Extensions.cs	\
	../linker/MonoTouch.Tuner/ListExportedSymbols.cs	\
	../linker/MonoTouch.Tuner/ProcessExportedFields.cs	\
	../../src/build/mac/Constants.cs

linker_resources = \
	linker/Descriptors/mscorlib.xml

monolinker_sources = \
	$(linker_sources)				\
	$(LINKER_DIR)/Mono.Linker/AssemblyInfo.cs	\
	$(LINKER_DIR)/Mono.Linker/Driver.cs

monolinker_resources = \
	$(LINKER_DIR)/Descriptors/mscorlib.xml		\
	$(LINKER_DIR)/Descriptors/System.xml		\
	$(LINKER_DIR)/Descriptors/Mono.Posix.xml

monolinker.exe: $(MONO_PATH)/mcs/tools/linker/Mono.Linker.csproj $(MONO_CECIL_DLL) $(MONO_CECIL_MDB_DLL)
	$(Q_MCS) $(SYSTEM_MCS) -debug -out:monolinker.exe -r:$(MONO_CECIL_DLL) -r:$(MONO_CECIL_MDB_DLL) $(monolinker_resources:%=-resource:%) $(monolinker_sources)

output/mmp.exe: monolinker.exe mmp.exe mmp.xml
	$(call Q_1,LINK    ) $(SYSTEM_MONO) --debug monolinker.exe -a mmp.exe -l none -c link -d $(BCL_LOCATION) -d $(dir $(XAMMAC_DLL)) -d $(dir $(MONO_CECIL_DLL)) -x mmp.xml

mmp_sources = \
	$(TOP)/tools/common/cache.cs    \
	driver.cs 	\
	$(TOP)/src/ObjCRuntime/ErrorHelper.cs \
	error.cs	\
	resolver.cs	\
	$(MONO_PATH)/mcs/class/Mono.Options/Mono.Options/Options.cs	\
	Tuning.cs \
	$(TOP)/tools/common/TargetFramework.cs \
	$(TOP)/tools/common/Driver.cs \
	$(TOP)/tools/common/Frameworks.cs \
	$(TOP)/tools/common/StaticRegistrar.cs \
	$(TOP)/tools/common/MachO.cs           \
	$(TOP)/src/ObjCRuntime/Registrar.core.cs	\
	$(TOP)/src/ObjCRuntime/Registrar.cs \
	$(TOP)/src/ObjCRuntime/RuntimeOptions.cs \
	Assembly.cs \
	Application.cs \
	$(TOP)/tools/common/Target.cs \
	$(TOP)/tools/common/Application.cs \
	$(TOP)/tools/common/Assembly.cs

DEFINES = -d:MONOMAC -d:MMP $(APPLETLS_DEFINES)

mmp.exe: Makefile $(XAMMAC_DLL) $(MONO_CECIL_DLL) $(MONO_CECIL_MDB_DLL) $(mmp_sources) config config_mobile Info.plist.tmpl $(tuner_sources) $(linker_resources)
	$(Q_MCS) $(SYSTEM_MCS) -unsafe -out:mmp.exe $(DEFINES) -r:$(XAMMAC_DLL) -r:$(MONO_CECIL_DLL) -r:$(MONO_CECIL_MDB_DLL) -r:Mono.Security.dll  -resource:config -resource:config_mobile -resource:machine.4_5.config -resource:Info.plist.tmpl $(linker_resources:%=-resource:%) $(tuner_sources) $(mmp_sources)

mmp: output/mmp.exe Makefile mmp.config mmp.machine.config machine.4_5.config mmp.xml
	$(Q) rm -Rf temp-dir-$@
	$(Q) mkdir -p temp-dir-$@
	$(Q_MKBUNDLE) cd temp-dir-$@ && \
		PATH="/usr/bin:$(PATH)" \
		LD_LIBRARY_PATH=$(TOP)/../builds/mac64/support/.libs:$(LD_LIBRARY_PATH) \
		CC="cc -arch x86_64 -framework CoreFoundation -liconv -framework AppKit -mmacosx-version-min=$(MIN_OSX_VERSION_FOR_MAC)" \
		AS="as -arch x86_64 -mmacosx-version-min=$(MIN_OSX_VERSION_FOR_MAC)" \
		$(SYSTEM_MKBUNDLE) -z --static ../output/mmp.exe ../output/*.dll --nodeps -c -o $@.stub.c -oo $@.helper.o --machine-config ../mmp.machine.config --config ../mmp.config
	$(Q_CC) cd temp-dir-$@ && cc -mmacosx-version-min=$(MIN_OSX_VERSION_FOR_MAC) -arch x86_64 -o $@ `$(PKG_CONFIG) --cflags mono-2 glib-2.0` ../$(TOP)/builds/mac64/mono/mini/.libs/libmonosgen-2.0.a  $@.stub.c $@.helper.o -liconv -lz -framework IOKit -framework CoreFoundation -framework Foundation
ifndef DISABLE_STRIP
	$(Q_STRIP) cd temp-dir-$@ && strip -x -S $@
endif
	$(Q) cp temp-dir-$@/$@ .
	@echo BUILD SUCCESSFULLY, file is ./$@;

MMP_TARGETS = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mmp

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mmp: mmp | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin
	$(Q) cp $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin:
	$(Q) mkdir -p $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin

install-local:: $(MMP_TARGETS)
all-local:: $(MMP_TARGETS)

clean-local::
	rm -f mmp mmp.exe mmp.exe.mdb mmp-uninstalled
	rm -f mmp.stub.c mmp.helper.o
	rm -f output/*
	rm -f monolinker.exe* Mono.Touch.Common.dll*
	$(SYSTEM_XBUILD) /t:Clean /p:Configuration=$(CECIL_CONFIG) $(CECIL_PATH)/Mono.Cecil.csproj
	$(SYSTEM_XBUILD) /t:Clean /p:Configuration=$(CECIL_CONFIG) $(CECIL_PATH)/symbols/mdb/Mono.Cecil.Mdb.csproj

include $(TOP)/mk/rules.mk
