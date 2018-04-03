TOP=../..
include $(TOP)/Make.config

# without this many compiler warnings about unused functions and variables
# in system headers show up.
export CCACHE_CPP2=1

LINKER_DIR=../linker

CECIL_PATH=$(MONO_PATH)/external/cecil
MONO_CECIL_FILES = $(wildcard $(CECIL_PATH)/*.cs) $(wildcard $(CECIL_PATH)/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*/*/*.cs)

MONO_DIR=$(MONO_PATH)
MONOLINKER=$(LINKER_TOOLS_PATH)/linker
MONO_TUNER=$(LINKER_TOOLS_PATH)/tuner

LINKER_RESOURCES = \
	$(LINKER_DIR)/Descriptors/mscorlib.xml

LINKER_SOURCES = \
	$(MONOLINKER)/Mono.Linker/Annotations.cs			\
	$(MONOLINKER)/Mono.Linker/AssemblyAction.cs			\
	$(MONOLINKER)/Mono.Linker/AssemblyResolver.cs			\
	$(MONOLINKER)/Mono.Linker/I18nAssemblies.cs			\
	$(MONOLINKER)/Mono.Linker/IXApiVisitor.cs			\
	$(MONOLINKER)/Mono.Linker/ILogger.cs				\
	$(MONOLINKER)/Mono.Linker/ConsoleLogger.cs			\
	$(MONOLINKER)/Mono.Linker/LinkContext.cs			\
	$(MONOLINKER)/Mono.Linker/LoadException.cs			\
	$(MONOLINKER)/Mono.Linker/MarkException.cs			\
	$(MONOLINKER)/Mono.Linker/MethodAction.cs			\
	$(MONOLINKER)/Mono.Linker/Pipeline.cs				\
	$(MONOLINKER)/Mono.Linker/MethodReferenceExtensions.cs		\
	$(MONOLINKER)/Mono.Linker/TypeReferenceExtensions.cs		\
	$(MONOLINKER)/Mono.Linker/TypePreserve.cs			\
	$(MONOLINKER)/Mono.Linker/XApiReader.cs				\
	$(MONOLINKER)/Mono.Linker.Steps/BaseStep.cs			\
	$(MONOLINKER)/Mono.Linker.Steps/BlacklistStep.cs		\
	$(MONOLINKER)/Mono.Linker.Steps/CleanStep.cs			\
	$(MONOLINKER)/Mono.Linker.Steps/IStep.cs			\
	$(MONOLINKER)/Mono.Linker.Steps/LoadI18nAssemblies.cs		\
	$(MONOLINKER)/Mono.Linker.Steps/LoadReferencesStep.cs		\
	$(MONOLINKER)/Mono.Linker.Steps/MarkStep.cs			\
	$(MONOLINKER)/Mono.Linker.Steps/OutputStep.cs			\
	$(MONOLINKER)/Mono.Linker.Steps/ResolveFromAssemblyStep.cs	\
	$(MONOLINKER)/Mono.Linker.Steps/ResolveFromXApiStep.cs		\
	$(MONOLINKER)/Mono.Linker.Steps/ResolveFromXmlStep.cs		\
	$(MONOLINKER)/Mono.Linker.Steps/ResolveStep.cs			\
	$(MONOLINKER)/Mono.Linker.Steps/SweepStep.cs			\
	$(MONOLINKER)/Mono.Linker.Steps/TypeMapStep.cs			\
	$(MONO_TUNER)/Mono.Tuner/ApplyPreserveAttributeBase.cs		\
	$(MONO_TUNER)/Mono.Tuner/CecilRocks.cs				\
	$(MONO_TUNER)/Mono.Tuner/CustomizeActions.cs			\
	$(MONO_TUNER)/Mono.Tuner/Dispatcher.cs				\
	$(MONO_TUNER)/Mono.Tuner/Extensions.cs				\
	$(MONO_TUNER)/Mono.Tuner/FixModuleFlags.cs			\
	$(MONO_TUNER)/Mono.Tuner/PreserveSoapHttpClients.cs		\
	$(MONO_TUNER)/Mono.Tuner/Profile.cs				\
	$(MONO_TUNER)/Mono.Tuner/RemoveAttributesBase.cs		\
	$(MONO_TUNER)/Mono.Tuner/RemoveResources.cs			\
	$(MONO_TUNER)/Mono.Tuner/RemoveSecurity.cs			\
	$(LINKER_DIR)/MonoTouch.Tuner/Extensions.cs			\
	$(LINKER_DIR)/MonoTouch.Tuner/InlinerSubStep.cs		\
	$(LINKER_DIR)/MonoTouch.Tuner/ListExportedSymbols.cs	\
	$(LINKER_DIR)/MonoTouch.Tuner/MetadataReducerSubStep.cs	\
	$(LINKER_DIR)/MonoTouch.Tuner/MonoTouchMarkStep.cs		\
	$(LINKER_DIR)/MonoTouch.Tuner/MonoTouchProfile.cs		\
	$(LINKER_DIR)/MonoTouch.Tuner/MonoTouchSweepStep.cs		\
	$(LINKER_DIR)/MonoTouch.Tuner/MonoTouchTypeMap.cs		\
	$(LINKER_DIR)/MonoTouch.Tuner/OptimizeGeneratedCodeSubStep.cs	\
	$(LINKER_DIR)/MonoTouch.Tuner/PreserveCode.cs			\
	$(LINKER_DIR)/MonoTouch.Tuner/ProcessExportedFields.cs	\
	$(LINKER_DIR)/MonoTouch.Tuner/RemoveAttributes.cs		\
	$(LINKER_DIR)/MonoTouch.Tuner/RemoveCode.cs			\
	$(LINKER_DIR)/MonoTouch.Tuner/SealerSubStep.cs	\
	$(LINKER_DIR)/MonoTouch.Tuner/PreserveSmartEnumConversionsSubStep.cs		\
	$(TOP)/tools/linker/ApplyPreserveAttribute.cs 		\
	$(TOP)/tools/linker/BaseProfile.cs			\
	$(TOP)/tools/linker/CoreHttpMessageHandler.cs	\
	$(TOP)/tools/linker/CoreMarkStep.cs			\
	$(TOP)/tools/linker/CoreOptimizeGeneratedCode.cs	\
	$(TOP)/tools/linker/CorePreserveCode.cs		\
	$(TOP)/tools/linker/CoreRemoveAttributes.cs		\
	$(TOP)/tools/linker/CoreRemoveSecurity.cs		\
	$(TOP)/tools/linker/ExceptionalSubStep.cs		\
	$(TOP)/tools/linker/MarkNSObjects.cs			\
	$(TOP)/tools/linker/MobileExtensions.cs		\
	$(TOP)/tools/linker/MobileMarkStep.cs			\
	$(TOP)/tools/linker/MobileProfile.cs			\
	$(TOP)/tools/linker/MobileRemoveAttributes.cs		\
	$(TOP)/tools/linker/MobileResolveMainAssemblyStep.cs	\
	$(TOP)/tools/linker/MobileSweepStep.cs			\
	$(TOP)/tools/linker/ObjCExtensions.cs			\
	$(TOP)/tools/linker/RemoveUserResourcesSubStep.cs \


COMMON_SOURCES = 											\
	$(TOP)/src/build/ios/Constants.cs						\
	error.cs \
	$(TOP)/src/ObjCRuntime/ErrorHelper.cs			\
	$(TOP)/src/ObjCRuntime/ExceptionMode.cs			\
	$(MONO_DIR)/mcs/class/Mono.Options/Mono.Options/Options.cs

MTOUCH_SOURCES = \
	$(COMMON_SOURCES)	\
	$(LINKER_SOURCES)	\
	$(TOP)/src/ObjCRuntime/ArgumentSemantic.cs	\
	$(TOP)/src/ObjCRuntime/LinkWithAttribute.cs	\
	$(TOP)/src/ObjCRuntime/NativeAttribute.cs	\
	$(TOP)/src/ObjCRuntime/PlatformAvailability2.cs	\
	$(TOP)/src/ObjCRuntime/ObjectWrapper.cs	\
	Application.cs			\
	Assembly.cs			\
	$(TOP)/tools/common/cache.cs			\
	$(TOP)/tools/common/Driver.cs		\
	$(TOP)/tools/common/Frameworks.cs	\
	$(TOP)/tools/common/MachO.cs		\
	$(TOP)/tools/common/Symbols.cs			\
	mtouch.cs		\
	$(TOP)/tools/common/PInvokeWrapperGenerator.cs	\
	$(TOP)/tools/common/TargetFramework.cs	\
	$(TOP)/tools/common/BuildTasks.cs	\
	$(TOP)/tools/common/StringUtils.cs		\
	$(TOP)/tools/common/StaticRegistrar.cs	\
	$(TOP)/src/ObjCRuntime/Registrar.core.cs	\
	$(TOP)/src/ObjCRuntime/Registrar.cs	\
	$(TOP)/src/ObjCRuntime/RuntimeOptions.cs \
	Tuning.cs	\
	Stripper.cs	\
	AssemblyResolver.cs	\
	Target.cs		\
	SdkVersions.cs \
	BitcodeConverter.cs	\
	BuildTasks.mtouch.cs \
	$(TOP)/tools/common/Application.cs \
	$(TOP)/tools/common/Assembly.cs \
	$(TOP)/tools/common/DerivedLinkContext.cs \
	$(TOP)/tools/common/Target.cs \
	$(TOP)/tools/common/CompilerFlags.cs \
	$(TOP)/src/Foundation/ExportAttribute.cs \
	$(TOP)/src/Foundation/ConnectAttribute.cs \
	$(XAMARIN_MACDEV_PATH)/Xamarin.MacDev/PListObject.cs \

CECIL_CONFIG=net_4_0_Release
MONO_CECIL_DLL=$(CECIL_PATH)/bin/$(CECIL_CONFIG)/Mono.Cecil.dll
MONO_CECIL_MDB_DLL=$(CECIL_PATH)/bin/$(CECIL_CONFIG)/Mono.Cecil.Mdb.dll
MONO_CECIL_FILES=$(wildcard $(CECIL_PATH)/*.cs) $(wildcard $(CECIL_PATH)/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*/*/*.cs)

$(MONO_CECIL_DLL): $(CECIL_PATH)/Mono.Cecil.csproj $(MONO_CECIL_FILES)
	$(Q_XBUILD) cd $(CECIL_PATH) && $(SYSTEM_XBUILD) /p:Configuration=$(CECIL_CONFIG) Mono.Cecil.csproj $(XBUILD_VERBOSITY)

$(MONO_CECIL_MDB_DLL): $(MONO_CECIL_DLL) $(CECIL_PATH)/symbols/mdb/Mono.Cecil.Mdb.csproj $(MONO_CECIL_FILES)
	$(Q_XBUILD) cd $(CECIL_PATH) && $(SYSTEM_XBUILD) /p:Configuration=$(CECIL_CONFIG) symbols/mdb/Mono.Cecil.Mdb.csproj $(XBUILD_VERBOSITY)

SIMLAUNCHERS = simlauncher32-sgen simlauncher64-sgen

monotouch-fixes.i386.dylib: monotouch-fixes.c Makefile
	$(SIMULATOR_CCOMPILE) -DDEBUG -Wl,-headerpad_max_install_names -g -dynamiclib -gdwarf-2 -I$(TOP)/runtime/.libs -L$(TOP)/runtime/.libs $(SIMULATOR86_CFLAGS) monotouch-fixes.c -o $@ -Wall

monotouch-fixes.x86_64.dylib: monotouch-fixes.c Makefile
	$(SIMULATOR_CCOMPILE) -DDEBUG -Wl,-headerpad_max_install_names -g -dynamiclib -gdwarf-2 -I$(TOP)/runtime/.libs -L$(TOP)/runtime/.libs $(SIMULATOR64_CFLAGS) monotouch-fixes.c -o $@ -Wall

monotouch-fixes.dylib: monotouch-fixes.i386.dylib monotouch-fixes.x86_64.dylib
	$(Q_LIPO) lipo -create -output $@ $^

UNREFERENCED_SYMBOLS = 							\
	_xamarin_dyn_objc_msgSend 					\
	_xamarin_dyn_objc_msgSendSuper 				\
	_xamarin_dyn_objc_msgSend_stret 			\
	_xamarin_dyn_objc_msgSendSuper_stret 		\
	_xamarin_init_nsthread						\
	_xamarin_get_block_descriptor				\
	_xamarin_get_locale_country_code			\
	_xamarin_log								\
	_xamarin_start_wwan							\
	_xamarin_timezone_get_data					\
	_xamarin_timezone_get_names					\
	_xamarin_IntPtr_objc_msgSend_IntPtr			\
	_xamarin_IntPtr_objc_msgSendSuper_IntPtr	\
	_xamarin_release_managed_ref				\
	_xamarin_create_managed_ref					\
	_CloseZStream								\
	_CreateZStream								\
	_Flush										\
	_ReadZStream								\
	_WriteZStream

# The frameworks order is important as some symbols can be duplicated 
# e.g. NSURLResponse is both in CFNetwork and Foundation in iOS8 but linking to
# CFNetwork will result in binaries that won't work with earlier iOS versions
SIMLAUNCHER_FRAMEWORKS =  \
	-framework CoreFoundation		\
	-framework CoreGraphics			\
	-framework Foundation			\
	-framework AVFoundation			\
	-framework Accelerate			\
	-framework AddressBook			\
	-framework AddressBookUI		\
	-framework AudioToolbox			\
	-framework CFNetwork			\
	-framework CoreAudio			\
	-framework CoreData			\
	-framework CoreLocation			\
	-framework CoreText			\
	-framework ExternalAccessory		\
	-framework GameKit				\
	-framework IOKit			\
	-framework MapKit			\
	-framework MediaPlayer			\
	-framework MessageUI			\
	-framework MobileCoreServices		\
	-framework OpenAL			\
	-framework OpenGLES			\
	-framework StoreKit			\
	-framework SystemConfiguration		\
	-framework UIKit			\
						\
	-weak_framework AssetsLibrary		\
	-weak_framework CoreMedia		\
	-weak_framework CoreMidi		\
	-weak_framework CoreMotion		\
	-weak_framework CoreTelephony		\
	-weak_framework CoreVideo		\
	-weak_framework EventKit		\
	-weak_framework EventKitUI		\
	-weak_framework ImageIO			\
	-weak_framework Security		\
	-weak_framework QuickLook		\
	-weak_framework iAd			\
	-weak_framework Accounts		\
	-weak_framework GLKit			\
	-weak_framework NewsstandKit		\
	-weak_framework CoreImage		\
	-weak_framework CoreBluetooth		\
	-weak_framework Twitter			\
	-weak_framework PassKit			\
	-weak_framework Social			\
	-weak_framework AdSupport		\
	-weak_framework JavaScriptCore		\
	-weak_framework MultipeerConnectivity	\
	-weak_framework SafariServices		\
	-weak_framework SpriteKit		\
						\
	-weak_framework HomeKit			\
	-weak_framework HealthKit		\
	-weak_framework LocalAuthentication	\
	-weak_framework NotificationCenter	\
	-weak_framework PushKit			\
	-weak_framework Photos			\
	-weak_framework PhotosUI		\
	-weak_framework SceneKit		\
	-weak_framework CloudKit		\
	-weak_framework AVKit			\
	-weak_framework WebKit			\
	-weak_framework NetworkExtension	\
	-weak_framework Metal				\
	-weak_framework CoreAudioKit		\
						\
	-weak_framework WatchKit		\
						\
	-weak_framework ReplayKit					\
	-weak_framework Contacts					\
	-weak_framework ContactsUI					\
	-weak_framework CoreSpotlight				\
	-weak_framework WatchConnectivity			\
	-weak_framework ModelIO						\
	-weak_framework GameplayKit					\
						\
	-weak_framework HealthKitUI					\
						\
	-weak_framework CallKit					\
	-weak_framework Messages				\
	-weak_framework Speech					\
	-weak_framework VideoSubscriberAccount	\
	-weak_framework UserNotifications		\
	-weak_framework UserNotificationsUI		\
	-weak_framework Intents					\
	-weak_framework IntentsUI					\
						\
	-weak_framework ARKit					\
	-weak_framework FileProvider			\
	-weak_framework FileProviderUI			\
	-weak_framework BusinessChat			\
	-weak_framework ClassKit

# note: there _was_ no CoreAudioKit.framework or Metal.framework for the simulator (before recent iOS9 betas)
# note 2: there's no GameKit, in iOS 9 (beta 3 at least), you're supposed to reference GameCenter instead (looks fixed in beta 4)
# note 3: there's no MetalKit or MetalPerformanceShaders in iOS 9 beta 4 - but other frameworks were added
# note 4: GameCenter was removed in Xcode 7 beta 5, and GameKit is back.

# keep the above list of weak_framework in sync with mtouch.cs
# except it is no a mistake that GameController and MediaAccessibility (#13636) are not built into simlauncher

define SimlauncherTemplate

simlauncher$(5)$(1): simlauncher.m $(TOP)/runtime/.libs/ios/libxamarin.a $(BUILD_DESTDIR)/simulator$(4)/lib/libmono$(3)-2.0.a Makefile Xamarin.iOS.registrar.ios.a
	$(QT_CC) $(SIMULATOR_CC) -DDEBUG -DSIMLAUNCHER $(2) -gdwarf-2 \
		-L$(TOP)/runtime/.libs/ios \
		-L$(BUILD_DESTDIR)/simulator$(4)/lib/ \
		-I$(TOP)/runtime \
		-I$(BUILD_DESTDIR)/simulator$(4)/include/mono-2.0 \
		-I$(TOP)/builds/simulator$(4)/mono/eglib \
		-I$(MONO_PATH)/mono/eglib \
		$(BUILD_DESTDIR)/simulator$(4)/lib/libmono$(3)-2.0.a \
		$(foreach u,$(UNREFERENCED_SYMBOLS),-u $u)  \
		$(TOP)/runtime/.libs/ios/libapp.a \
		-Wl,-w \
		-lz -liconv $(TOP)/runtime/.libs/ios/libxamarin-debug.a simlauncher.m $(SIMLAUNCHER_FRAMEWORKS) $(SIMULATOR$(4)_OBJC_CFLAGS) -o $$@

endef

$(eval $(call SimlauncherTemplate,-sgen,$(BUILD_DESTDIR)/simulator86/lib/libmono-profiler-log.a -u _mono_profiler_init_log -DXAMCORE_2_0 Xamarin.iOS.registrar.ios.a,sgen,86,32))
$(eval $(call SimlauncherTemplate,-sgen,$(BUILD_DESTDIR)/simulator64/lib/libmono-profiler-log.a -u _mono_profiler_init_log -DXAMCORE_2_0 Xamarin.iOS.registrar.ios.a,sgen,64,64))

IOS_DEFINES = -d:MONOTOUCH -d:MTOUCH $(APPLETLS_DEFINES)

ifdef ENABLE_BITCODE_ON_IOS
MTOUCH_DEFINES= -define:ENABLE_BITCODE_ON_IOS
endif

MTOUCH_COMPILE_DEPS = $(MTOUCH_SOURCES) $(MONO_CECIL_DLL) $(MONO_CECIL_MDB_DLL) Makefile $(LINKER_RESOURCES)
MTOUCH_COMPILE_ARGS = -unsafe $(IOS_DEFINES) -r:$(MONO_CECIL_DLL) -r:$(MONO_CECIL_MDB_DLL) -r:System.Xml.Linq.dll $(MTOUCH_SOURCES) -nowarn:0436 $(LINKER_RESOURCES:%=-resource:%) $(MTOUCH_DEFINES) -debug:portable

# bin/Debug/mtouch.exe is used from mtouch.csproj when debugging
bin/Debug/mtouch.exe: $(MTOUCH_COMPILE_DEPS)
	$(Q) mkdir -p bin/Debug
	$(Q_CSC) $(SYSTEM_CSC) $(MTOUCH_COMPILE_ARGS) -out:$@ -d:DEV

# bin/Makefile/mtouch.exe is what we build from the makefile (and end up releasing)
bin/Makefile/mtouch.exe: $(MTOUCH_COMPILE_DEPS)
	$(Q) mkdir -p $(dir $@)
	$(Q_CSC) $(SYSTEM_CSC) $(MTOUCH_COMPILE_ARGS) -out:$@

LOCAL_MTOUCH = \
	bin/Makefile/mtouch.exe \
	bin/Makefile/Mono.Cecil.dll \
	bin/Makefile/Mono.Cecil.Mdb.dll \

bin/Makefile/Mono.Cecil.dll: $(MONO_CECIL_DLL) | bin/Makefile
	$(Q) cp $<* $(dir $@)

bin/Makefile/Mono.Cecil.Mdb.dll: $(MONO_CECIL_MDB_DLL) | bin/Makefile
	$(Q) cp $<* $(dir $@)

%.registrar.ios.i386.m     %.registrar.ios.i386.h:     $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/%.dll     $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug bin/Makefile/mtouch.exe --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(IOS_SDK_VERSION)   $< --registrar:static --target-framework Xamarin.iOS,v1.0    --abi i386
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.ios.x86_64.m   %.registrar.ios.x86_64.h:   $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/%.dll     $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug bin/Makefile/mtouch.exe --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(IOS_SDK_VERSION)   $< --registrar:static --target-framework Xamarin.iOS,v1.0    --abi x86_64
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.watchos.m %.registrar.watchos.h: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.dll $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug bin/Makefile/mtouch.exe --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(WATCH_SDK_VERSION) $< --registrar:static --target-framework Xamarin.WatchOS,1.0 --abi i386
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.tvos.m    %.registrar.tvos.h:    $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.dll    $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug bin/Makefile/mtouch.exe --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(TVOS_SDK_VERSION)  $< --registrar:static --target-framework Xamarin.TVOS,1.0    --abi x86_64
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.ios.a:        %.registrar.ios.i386.a %.registrar.ios.x86_64.a
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^

%.registrar.ios.i386.a:   %.registrar.ios.i386.m      %.registrar.ios.i386.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATOR86_CFLAGS)    -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk/usr/include

%.registrar.ios.x86_64.a: %.registrar.ios.x86_64.m    %.registrar.ios.x86_64.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATOR64_CFLAGS)    -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk/usr/include

%.registrar.watchos.a:    %.registrar.watchos.m  %.registrar.watchos.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATORWATCH_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/include

%.registrar.tvos.a:       %.registrar.tvos.m     %.registrar.tvos.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATORTV_CFLAGS)    -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/include

TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mtouch                                          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/mtouch.exe                               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/Mono.Cecil.dll                           \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/Mono.Cecil.Mdb.dll                       \
	$(foreach launcher,$(SIMLAUNCHERS),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/$(launcher)) \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/monotouch-fixes.dylib                           \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk/usr/lib/Xamarin.iOS.registrar.a         \

ifdef INCLUDE_WATCH
TARGETS += $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/Xamarin.WatchOS.registrar.a
endif

ifdef INCLUDE_TVOS
TARGETS +=	\
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/Xamarin.TVOS.registrar.a       \

endif

TARGET_DIRS = \
	bin/Makefile                                                 \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch                 \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/etc/mono/assemblies/System \
	$(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/usr/lib             \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib          \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib             \

$(TARGET_DIRS):
	$(Q) mkdir -p $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/mtouch.exe: bin/Makefile/mtouch.exe | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch
	$(Q) cp $< $(dir $@)
	$(Q) cp $(<:.exe=.pdb) $(dir $@)

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/Mono.Cecil.dll: $(MONO_CECIL_DLL) | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch
	$(Q) cp $< $(dir $@)
	$(Q) cp $(<:.exe=.pdb) $(dir $@)

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/Mono.Cecil.Mdb.dll: $(MONO_CECIL_MDB_DLL) | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch
	$(Q) cp $< $(dir $@)
	$(Q) cp $(<:.exe=.pdb) $(dir $@)

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mtouch: mtouch.in | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin
	$(Q_GEN) sed 's!@MONOTOUCH_PREFIX@!$(abspath $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX))!g' $< > $@
	$(Q) chmod +x $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/%: % | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin
	$(Q) cp $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/%: % | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib
	$(Q) cp $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/monotouch-fixes.dylib: monotouch-fixes.dylib | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib
	$(Q) cp $< $@
	$(Q) install_name_tool -id $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/lib/monotouch-fixes.dylib $@

$(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/usr/lib/%.registrar.a: %.registrar.ios.a | $(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/usr/lib
	$(Q) cp $< $@

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/%.registrar.a: %.registrar.watchos.a | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib
	$(Q) cp $< $@

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/%.registrar.a: %.registrar.tvos.a | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib
	$(Q) cp $< $@

ifdef INCLUDE_IOS
install-local:: $(TARGETS)
all-local:: $(TARGETS)
endif

mtouch: \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mtouch                                          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/mtouch.exe                               \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/Mono.Cecil.dll                           \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/Mono.Cecil.Mdb.dll

clean-local::
	rm -Rf bin obj
	rm -f $(SIMLAUNCHERS)
	$(SYSTEM_XBUILD) /t:Clean /p:Configuration=$(CECIL_CONFIG) $(CECIL_PATH)/Mono.Cecil.csproj
	$(SYSTEM_XBUILD) /t:Clean /p:Configuration=$(CECIL_CONFIG) $(CECIL_PATH)/symbols/mdb/Mono.Cecil.Mdb.csproj

include $(TOP)/mk/rules.mk
include ../common/Make.common

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# marking those files as .SECONDARY will prevent that deletion.
.SECONDARY: $(foreach ext,a h m,Xamarin.iOS.registrar.ios.i386.$(ext) Xamarin.iOS.registrar.ios.x86_64.$(ext))
.SECONDARY: $(foreach ext,a h m,Xamarin.WatchOS.registrar.watchos.$(ext))
.SECONDARY: $(foreach ext,a h m,Xamarin.TVOS.registrar.tvos.$(ext))
