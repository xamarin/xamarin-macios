TOP=../..
include $(TOP)/Make.config

# without this many compiler warnings about unused functions and variables
# in system headers show up.
export CCACHE_CPP2=1

ifdef IOS_SUPPORTS_32BIT_ARCHITECTURES
SIMLAUNCHERS = simlauncher32-sgen simlauncher64-sgen
else
SIMLAUNCHERS = simlauncher64-sgen
endif

UNREFERENCED_SYMBOLS = 							\
	_xamarin_dyn_objc_msgSend 					\
	_xamarin_dyn_objc_msgSendSuper 				\
	_xamarin_dyn_objc_msgSend_stret 			\
	_xamarin_dyn_objc_msgSendSuper_stret 		\
	_xamarin_init_nsthread						\
	_xamarin_get_block_descriptor				\
	_xamarin_get_locale_country_code			\
	_xamarin_log								\
	_xamarin_localized_string_format			\
	_xamarin_localized_string_format_1			\
	_xamarin_localized_string_format_2			\
	_xamarin_localized_string_format_3			\
	_xamarin_localized_string_format_4			\
	_xamarin_localized_string_format_5			\
	_xamarin_localized_string_format_6			\
	_xamarin_localized_string_format_7			\
	_xamarin_localized_string_format_8			\
	_xamarin_localized_string_format_9			\
	_xamarin_start_wwan							\
	_xamarin_timezone_get_data					\
	_xamarin_timezone_get_names					\
	_xamarin_timezone_get_local_name			\
	_xamarin_IntPtr_objc_msgSend_IntPtr_exception		\
	_xamarin_IntPtr_objc_msgSendSuper_IntPtr_exception	\
	_xamarin_release_managed_ref				\
	_xamarin_CGPoint__VNNormalizedFaceBoundingBoxPointForLandmarkPoint_Vector2_CGRect_nuint_nuint_string \
	_xamarin_CGPoint__VNImagePointForFaceLandmarkPoint_Vector2_CGRect_nuint_nuint_string \
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
	-weak_framework ReplayKit					\
	-weak_framework Contacts					\
	-weak_framework ContactsUI					\
	-weak_framework CoreSpotlight				\
	-weak_framework WatchConnectivity			\
	-weak_framework ModelIO						\
	-weak_framework GameplayKit					\
	-weak_framework Metal						\
	-weak_framework MetalKit					\
	-weak_framework MetalPerformanceShaders		\
	-weak_framework MetalPerformanceShadersGraph\
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
						\
	-weak_framework IdentityLookup			\
	-weak_framework BusinessChat			\
	-weak_framework ClassKit				\
						\
	-weak_framework AuthenticationServices	\
	-weak_framework CarPlay					\
	-weak_framework IdentityLookupUI		\
	-weak_framework NaturalLanguage			\
	-weak_framework Network				\
	-weak_framework IdentityLookupUI		\
	-weak_framework VisionKit		\
						\
	-weak_framework BackgroundTasks 		\
	-weak_framework CoreHaptics \
	-weak_framework LinkPresentation 		\
	-weak_framework MetricKit		\
	-weak_framework PencilKit			\
	-weak_framework QuickLookThumbnailing		\
	-weak_framework SoundAnalysis			\
	-weak_framework VisionKit			\
						\
	-weak_framework AutomaticAssessmentConfiguration	\
						\
	-weak_framework Accessibility	\
	-weak_framework AppClip	\
	-weak_framework AppTrackingTransparency	\
	-weak_framework ScreenTime \
	-weak_framework SensorKit \
	-weak_framework UniformTypeIdentifiers \
	-weak_framework NearbyInteraction \
						\
	-weak_framework AdServices	\
						\
	-weak_framework CoreLocationUI \
	-weak_framework OSLog \
						\
	-weak_framework AVRouting \
	-weak_framework BackgroundAssets \
	-weak_framework SharedWithYou \
	-weak_framework SharedWithYouCore \
						\
	-weak_framework Symbols \
	-weak_framework SensitiveContentAnalysis \

SIMLAUNCHER64_FRAMEWORKS =  \
	-framework GameKit				\
# keep the above list of weak_framework
# 1. grouped by iOS versions;
# 2. sorted and
# 3. in sync with the list inside `tools/common/Frameworks.cs`
# except it is no a mistake that GameController and MediaAccessibility (#13636) are not built into simlauncher

# note: there _was_ no CoreAudioKit.framework or Metal.framework for the simulator (before recent iOS9 betas)
# note 2: there's no GameKit, in iOS 9 (beta 3 at least), you're supposed to reference GameCenter instead (looks fixed in beta 4)
# note 3: there's no MetalKit or MetalPerformanceShaders in iOS 9 beta 4 - but other frameworks were added
# note 4: GameCenter was removed in Xcode 7 beta 5, and GameKit is back.
# note 5: Metal* seems to be supported as of some Xcode 7 beta (b2 didn't, but the final release does)
# note 6: Xcode 11 beta 1 shipped without WatchKit.
# note 7: Xcode 11 beta 3 documented that shipping without WatchKit was done on purpose (in the release notes).
# note 8: Xcode 14 beta 2 removed GameKit for 32-bit iOS

define SimlauncherTemplate

simlauncher$(3)-sgen: simlauncher.mm $(TOP)/runtime/.libs/iphonesimulator/libxamarin-debug.a Makefile Xamarin.iOS.registrar.ios.simulator.a
	$(QT_CC) $(SIMULATOR_CC) -DDEBUG -DSIMLAUNCHER $(1) -gdwarf-2 \
		-L$(TOP)/runtime/.libs/iphonesimulator \
		-I$(TOP)/runtime \
		$(MONO_IOS_SDK_DESTDIR)/ios-libs/ios-sim/libmonosgen-2.0.a \
		$(foreach u,$(UNREFERENCED_SYMBOLS),-u $u)  \
		$(TOP)/runtime/.libs/iphonesimulator/libapp.a \
		-Wl,-w \
		-lz -liconv $(TOP)/runtime/.libs/iphonesimulator/libxamarin-debug.a simlauncher.mm $(SIMLAUNCHER_FRAMEWORKS) $(SIMLAUNCHER$(3)_FRAMEWORKS) $(SIMULATOR$(2)_OBJC_CFLAGS) -o $$@
	$(Q) xcrun otool -L $$@ > $$@.frameworks
endef

$(eval $(call SimlauncherTemplate,Xamarin.iOS.registrar.ios.simulator.a,86,32))
$(eval $(call SimlauncherTemplate,Xamarin.iOS.registrar.ios.simulator.a,64,64))

#
# mtouch
#

MTOUCH_CONF=Release
MTOUCH_DIR=bin/$(MTOUCH_CONF)
LOCAL_MTOUCH=$(MTOUCH_DIR)/mtouch.exe
LOCAL_MTOUCH_COMMAND=$(SYSTEM_MONO) --debug $(LOCAL_MTOUCH)

DOTNET_PLATFORMS_MTOUCH=$(filter-out macOS,$(DOTNET_PLATFORMS))

# mtouch.csproj.inc contains the mtouch_dependencies variable used to determine if mtouch needs to be rebuilt or not.
mtouch.csproj.inc: export BUILD_VERBOSITY=$(MSBUILD_VERBOSITY)
-include mtouch.csproj.inc

$(MTOUCH_DIR)/mtouch.exe: $(mtouch_dependencies)
	$(Q_GEN) $(SYSTEM_MSBUILD) $(TOP)/Xamarin.iOS.sln "/t:mtouch" $(XBUILD_VERBOSITY) /p:Configuration=$(MTOUCH_CONF)

Constants.cs: Constants.cs.in Makefile $(TOP)/Make.config.inc
	$(Q_GEN) sed \
		-e "s/@VERSION@/$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)/g" \
		-e 's/@REVISION@/$(IOS_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(CURRENT_HASH))/g' \
		$< > $@
#
# Partial static registrar libraries
#
define RunRegistrar
%.registrar.$(1).$(2).m %.registrar.$(1).$(2).h: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/$(3)bits/$(9)/%.dll $(LOCAL_MTOUCH)
	$$(Q_GEN) $$(LOCAL_MTOUCH_COMMAND) --xamarin-framework-directory=$$(IOS_DESTDIR)/$$(MONOTOUCH_PREFIX) $$(MTOUCH_VERBOSITY) --runregistrar:$$(abspath $$(basename $$@).m) --sdkroot $$(XCODE_DEVELOPER_ROOT) --sdk $(4) $$< --registrar:static --target-framework Xamarin.$(5),v1.0 --abi $(2) -r:$(8)/mscorlib.dll
	$$(Q) touch $$(basename $$@).m $$(basename $$@).h

.libs/Microsoft.$(9).registrar.$(10)%m .libs/Microsoft.$(9).registrar.$(10)%h: $(TOP)/src/build/dotnet/$(1)/$(3)/Microsoft.$(9).dll $(LOCAL_MTOUCH) | .libs
	$$(Q_GEN) $$(LOCAL_MTOUCH_COMMAND) --xamarin-framework-directory=$$(IOS_DESTDIR)/$$(MONOTOUCH_PREFIX) $$(MTOUCH_VERBOSITY) --runregistrar:$$(abspath $$(basename $$@).m) --sdkroot $$(XCODE_DEVELOPER_ROOT) --sdk $(4) $$< --registrar:static --target-framework .NETCoreApp,Version=$(subst net,,$(DOTNET_TFM)),Profile=$(1) --abi $(2) -r:$(DOTNET_BCL_DIR)/System.Runtime.dll -r:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll --rid $(10)
	$$(Q) touch $$(basename $$@).m $$(basename $$@).h

%.registrar.$(1).$(2).a: %.registrar.$(1).$(2).m %.registrar.$(1).$(2).h
	$$(Q_CC) $$(IOS_CC) -DDEBUG -g -gdwarf-2 $(6) -stdlib=libc++ -std=c++14 -x objective-c++ -o $$@ -c $$< -Wall -Wno-unguarded-availability-new -I$(7) -I$(TOP)/runtime

.libs/Microsoft.$(9).registrar.$(10).a: .libs/Microsoft.$(9).registrar.$(10).m .libs/Microsoft.$(9).registrar.$(10).h | .libs
	$$(Q_CC) $$(IOS_CC) -DDEBUG -g -gdwarf-2 $(6) -stdlib=libc++ -std=c++14 -x objective-c++ -o $$@ -c $$< -Wall -Wno-unguarded-availability-new -I$(7) -I$(TOP)/runtime

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# marking those files as .SECONDARY will prevent that deletion.
.SECONDARY: Xamarin.$(5).registrar.$(1).$(2).m Xamarin.$(5).registrar.$(1).$(2).h Xamarin.$(5).registrar.$(1).$(2).a
.SECONDARY: .libs/Microsoft.$(8).registrar.$(2).m .libs/Microsoft.$(8).registrar.$(2).h .libs/Microsoft.$(8).registrar.$(2).a
endef
ifdef IOS_SUPPORTS_32BIT_ARCHITECTURES
$(eval $(call RunRegistrar,ios,i386,32,$(IOS_SDK_VERSION),iOS,$(SIMULATOR86_CFLAGS),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk/include,$(MONOTOUCH_MONO_PATH),iOS,iossimulator-x86))
endif
$(eval $(call RunRegistrar,ios,x86_64,64,$(IOS_SDK_VERSION),iOS,$(SIMULATOR64_CFLAGS),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk/include,$(MONOTOUCH_MONO_PATH),iOS,iossimulator-x64))
$(eval $(call RunRegistrar,ios,arm64,64,$(IOS_SDK_VERSION),iOS,$(SIMULATOR_ARM64_CFLAGS),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk/include,$(MONOTOUCH_MONO_PATH),iOS,iossimulator-arm64))
ifdef IOS_SUPPORTS_32BIT_ARCHITECTURES
$(eval $(call RunRegistrar,ios,armv7,32,$(IOS_SDK_VERSION),iOS,$(DEVICE7_CFLAGS),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphoneos.sdk/include,$(MONOTOUCH_MONO_PATH),iOS,armv7))
$(eval $(call RunRegistrar,ios,armv7s,32,$(IOS_SDK_VERSION),iOS,$(DEVICE7S_CFLAGS),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphoneos.sdk/include,$(MONOTOUCH_MONO_PATH),iOS,armv7s))
endif
$(eval $(call RunRegistrar,ios,arm64,64,$(IOS_SDK_VERSION),iOS,$(DEVICE64_CFLAGS),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphoneos.sdk/include,$(MONOTOUCH_MONO_PATH),iOS,ios-arm64))
$(eval $(call RunRegistrar,tvos,x86_64,64,$(TVOS_SDK_VERSION),TVOS,$(SIMULATORTV_CFLAGS),$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/include,$(MONOTOUCH_TV_MONO_PATH),tvOS,tvossimulator-x64))
$(eval $(call RunRegistrar,tvos,arm64,64,$(TVOS_SDK_VERSION),TVOS,$(SIMULATORTV_ARM64_CFLAGS),$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/include,$(MONOTOUCH_TV_MONO_PATH),tvOS,tvossimulator-arm64))
$(eval $(call RunRegistrar,tvos,arm64,64,$(TVOS_SDK_VERSION),TVOS,$(DEVICETV_CFLAGS),$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/include,$(MONOTOUCH_TV_MONO_PATH),tvOS,tvos-arm64))
$(eval $(call RunRegistrar,watchos,x86_64,64,$(WATCH_SDK_VERSION),WatchOS,$(SIMULATORWATCH64_CFLAGS),$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/include,$(MONOTOUCH_WATCH_MONO_PATH),watchOS,watchos-x64))
ifdef WATCHOS_SUPPORTS_32BIT_SIMULATOR_ARCHITECTURES
$(eval $(call RunRegistrar,watchos,i386,32,$(WATCH_SDK_VERSION),WatchOS,$(SIMULATORWATCH_CFLAGS),$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/include,$(MONOTOUCH_WATCH_MONO_PATH),watchOS,watchos-x86))
endif
ifdef WATCHOS_SUPPORTS_32BIT_DEVICE_ARCHITECTURES
$(eval $(call RunRegistrar,watchos,armv7k,32,$(WATCH_SDK_VERSION),WatchOS,$(DEVICEWATCH_CFLAGS),$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/include,$(MONOTOUCH_WATCH_MONO_PATH),watchOS,armv7k))
endif
$(eval $(call RunRegistrar,watchos,arm64_32,32,$(WATCH_SDK_VERSION),WatchOS,$(DEVICEWATCH64_32_CFLAGS),$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/include,$(MONOTOUCH_WATCH_MONO_PATH),watchOS,arm64_32))
$(eval $(call RunRegistrar,maccatalyst,x86_64,64,$(MACCATALYST_SDK_VERSION),MacCatalyst,$(MACCATALYST_X86_64_CFLAGS),$(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/include,$(MONOTOUCH_MACCATALYST_MONO_PATH),MacCatalyst,maccatalyst-x64))
$(eval $(call RunRegistrar,maccatalyst,arm64,64,$(MACCATALYST_SDK_VERSION),MacCatalyst,$(MACCATALYST_ARM64_CFLAGS),$(IOS_DESTDIR)$(XAMARIN_MACCATALYST_SDK)/include,$(MONOTOUCH_MACCATALYST_MONO_PATH),MacCatalyst,maccatalyst-arm64))

ifdef IOS_SUPPORTS_32BIT_ARCHITECTURES
%.registrar.ios.simulator.a:     %.registrar.ios.i386.a %.registrar.ios.x86_64.a
else
%.registrar.ios.simulator.a:     %.registrar.ios.x86_64.a
endif
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^

ifdef IOS_SUPPORTS_32BIT_ARCHITECTURES
%.registrar.ios.device.a:        %.registrar.ios.arm64.a %.registrar.ios.armv7.a %.registrar.ios.armv7s.a
else
%.registrar.ios.device.a:        %.registrar.ios.arm64.a
endif
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^

ifdef WATCHOS_SUPPORTS_32BIT_DEVICE_ARCHITECTURES
%.registrar.watchos.device.a:    %.registrar.watchos.armv7k.a %.registrar.watchos.arm64_32.a
else
%.registrar.watchos.device.a:    %.registrar.watchos.arm64_32.a
endif
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^

ifdef WATCHOS_SUPPORTS_32BIT_SIMULATOR_ARCHITECTURES
%.registrar.watchos.simulator.a: %.registrar.watchos.i386.a %.registrar.watchos.x86_64.a
else
%.registrar.watchos.simulator.a: %.registrar.watchos.x86_64.a
endif
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^

.libs/Microsoft.iOS.registrar.ios-arm.a: .libs/Microsoft.iOS.registrar.armv7.a .libs/Microsoft.iOS.registrar.armv7s.a
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^

.libs/Microsoft.watchOS.registrar.watchos-arm.a: .libs/Microsoft.watchOS.registrar.armv7k.a .libs/Microsoft.watchOS.registrar.arm64_32.a
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^

TARGETS_DOTNET = \
	$(foreach platform,$(DOTNET_PLATFORMS_MTOUCH),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/native/Microsoft.$(platform).registrar.a)) \

TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mtouch                                          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/mtouch.exe                               \
	$(foreach launcher,$(SIMLAUNCHERS),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/$(launcher)) \
	$(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/lib/Xamarin.iOS.registrar.a                  \

ifdef INCLUDE_DEVICE
TARGETS += $(IOS_DESTDIR)$(MONOTOUCH_DEVICE_SDK)/lib/Xamarin.iOS.registrar.a
endif

ifdef INCLUDE_WATCH
TARGETS += \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib/Xamarin.WatchOS.registrar.a \

ifdef INCLUDE_DEVICE
TARGETS += $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/Xamarin.WatchOS.registrar.a
endif
endif


ifdef INCLUDE_TVOS
TARGETS +=	\
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib/Xamarin.TVOS.registrar.a       \

ifdef INCLUDE_DEVICE
TARGETS += $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/Xamarin.TVOS.registrar.a
endif
endif

TARGET_DIRS_DOTNET = \
	$(foreach platform,$(DOTNET_PLATFORMS_MTOUCH),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/native)) \

TARGET_DIRS = \
	.libs                                                        \
	bin/Makefile                                                 \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch                 \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/etc/mono/assemblies/System \
	$(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/lib                 \
	$(IOS_DESTDIR)$(MONOTOUCH_DEVICE_SDK)/lib                    \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib              \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib                     \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib                 \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib                        \
	$(TARGET_DIRS_DOTNET)                                        \

$(TARGET_DIRS):
	$(Q) mkdir -p $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/mtouch.exe: $(MTOUCH_DIR)/mtouch.exe | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch
	$(Q) $(CP) $(dir $<)/*.exe $(dir $@)
	$(Q) $(CP) $(dir $<)/*.dll $(dir $@)
	$(Q) $(CP) $(dir $<)/*.pdb $(dir $@)

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mtouch: mtouch | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/%: % | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/%: % | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/lib/%.registrar.a: %.registrar.ios.simulator.a | $(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib/%.registrar.a: %.registrar.watchos.simulator.a | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib/%.registrar.a: %.registrar.tvos.x86_64.a | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_DEVICE_SDK)/lib/%.registrar.a: %.registrar.ios.device.a | $(IOS_DESTDIR)$(MONOTOUCH_DEVICE_SDK)/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib/%.registrar.a: %.registrar.watchos.device.a | $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib/%.registrar.a: %.registrar.tvos.arm64.a | $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/lib
	$(Q) $(CP) $< $@

define InstallRegistrar
$(DOTNET_DESTDIR)/$$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/Microsoft.$(1).registrar.a: .libs/Microsoft.$(1).registrar.$(rid).a | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
	$(Q) $(CP) $$< $$@
endef

$(foreach platform,$(DOTNET_PLATFORMS_MTOUCH),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call InstallRegistrar,$(platform),$(rid)))))

dotnet: $(TARGETS_DOTNET)
ifdef ENABLE_DOTNET
install-local:: $(TARGETS_DOTNET)
all-local:: $(TARGETS_DOTNET)
endif

ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_IOS
install-local:: $(TARGETS)
all-local:: $(TARGETS)
endif
endif

install-mtouch: \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mtouch                                          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/mtouch.exe                               \

clean-local::
	rm -Rf bin obj
	rm -f $(SIMLAUNCHERS)
	$(SYSTEM_MSBUILD) "/t:Clean" /p:Configuration=$(MTOUCH_CONF) *.csproj

all-local:: verify-sorted-resx
verify-sorted-resx:
	$(Q) grep '"[A-Z][A-Z][0-9][0-9][0-9][0-9]' Errors.resx | sed -e 's/.*name="[A-Z][A-Z]//' -e 's/" xml:.*//' > .$@.tmp
	$(Q) sort .$@.tmp > .$@-sorted.tmp
	$(Q) if diff -U0 .$@.tmp .$@-sorted.tmp > ".$@-unsorted.tmp" 2>&1; then \
		rm -rf .$@*tmp; \
	else \
		echo "❌ The following errors in Errors.resx are not sorted:"; \
		cat ".$@-unsorted.tmp" | grep "^[+][^+]" | sed 's/^[+]/    /'; \
		rm -rf .$@*tmp; \
		exit 1; \
	fi

clean-dotnet:
	rm -f $(TARGETS_DOTNET)

include $(TOP)/mk/rules.mk
include ../common/Make.common

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# marking those files as .SECONDARY will prevent that deletion.
.SECONDARY: Xamarin.iOS.registrar.ios.simulator.a Xamarin.iOS.registrar.ios.device.a Xamarin.WatchOS.registrar.watchos.device.a
