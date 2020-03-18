TOP=../..
include $(TOP)/Make.config

# without this many compiler warnings about unused functions and variables
# in system headers show up.
export CCACHE_CPP2=1

SIMLAUNCHERS = simlauncher32-sgen simlauncher64-sgen

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
	_xamarin_IntPtr_objc_msgSend_IntPtr			\
	_xamarin_IntPtr_objc_msgSendSuper_IntPtr	\
	_xamarin_release_managed_ref				\
	_xamarin_create_managed_ref					\
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
	-framework GameKit				\
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

define SimlauncherTemplate

simlauncher$(3)-sgen: simlauncher.mm $(TOP)/runtime/.libs/iphonesimulator/libxamarin-debug.a Makefile Xamarin.iOS.registrar.ios.simulator.a
	$(QT_CC) $(SIMULATOR_CC) -DDEBUG -DSIMLAUNCHER $(1) -gdwarf-2 \
		-L$(TOP)/runtime/.libs/iphonesimulator \
		-I$(TOP)/runtime \
		$(MONO_IOS_SDK_DESTDIR)/ios-libs/ios-sim/libmonosgen-2.0.a \
		$(foreach u,$(UNREFERENCED_SYMBOLS),-u $u)  \
		$(TOP)/runtime/.libs/iphonesimulator/libapp.a \
		-Wl,-w \
		-lz -liconv $(TOP)/runtime/.libs/iphonesimulator/libxamarin-debug.a simlauncher.mm $(SIMLAUNCHER_FRAMEWORKS) $(SIMULATOR$(2)_OBJC_CFLAGS) -o $$@
	$(Q) xcrun otool -L $$@ > $$@.frameworks
endef

$(eval $(call SimlauncherTemplate,-DXAMCORE_2_0 Xamarin.iOS.registrar.ios.simulator.a,86,32))
$(eval $(call SimlauncherTemplate,-DXAMCORE_2_0 Xamarin.iOS.registrar.ios.simulator.a,64,64))

#
# mtouch
#

MTOUCH_CONF=Release
MTOUCH_DIR=bin/$(MTOUCH_CONF)
LOCAL_MTOUCH=$(MTOUCH_DIR)/mtouch.exe

# mtouch.csproj.inc contains the mtouch_dependencies variable used to determine if mtouch needs to be rebuilt or not.
mtouch.csproj.inc: Makefile ../common/create-makefile-fragment.sh $(TOP)/Make.config $(TOP)/mk/mono.mk
	$(Q_GEN) ../common/create-makefile-fragment.sh $(CURDIR)/mtouch.csproj

-include mtouch.csproj.inc

$(MTOUCH_DIR)/mtouch.exe: $(mtouch_dependencies)
	$(Q_GEN) $(SYSTEM_MSBUILD) $(TOP)/Xamarin.iOS.sln "/t:mtouch" $(XBUILD_VERBOSITY) /p:Configuration=$(MTOUCH_CONF)

#
# Partial static registrar libraries
#

%.registrar.ios.i386.m     %.registrar.ios.i386.h:     $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/%.dll     $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MTOUCH) --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(IOS_SDK_VERSION)   $< --registrar:static --target-framework Xamarin.iOS,v1.0    --abi i386
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.ios.x86_64.m   %.registrar.ios.x86_64.h:   $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/%.dll     $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MTOUCH) --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(IOS_SDK_VERSION)   $< --registrar:static --target-framework Xamarin.iOS,v1.0    --abi x86_64
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.ios.armv7.m   %.registrar.ios.armv7.h:   $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/%.dll     $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MTOUCH) --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(IOS_SDK_VERSION)   $< --registrar:static --target-framework Xamarin.iOS,v1.0    --abi armv7
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.ios.armv7s.m   %.registrar.ios.armv7s.h:   $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/%.dll     $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MTOUCH) --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(IOS_SDK_VERSION)   $< --registrar:static --target-framework Xamarin.iOS,v1.0    --abi armv7s
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.ios.arm64.m   %.registrar.ios.arm64.h:   $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/%.dll     $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MTOUCH) --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(IOS_SDK_VERSION)   $< --registrar:static --target-framework Xamarin.iOS,v1.0    --abi arm64
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.watchos.i386.m %.registrar.watchos.i386.h: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.dll $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MTOUCH) --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(WATCH_SDK_VERSION) $< --registrar:static --target-framework Xamarin.WatchOS,1.0 --abi i386
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.watchos.armv7k.m %.registrar.watchos.armv7k.h: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.dll $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MTOUCH) --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(WATCH_SDK_VERSION) $< --registrar:static --target-framework Xamarin.WatchOS,1.0 --abi armv7k
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.tvos.x86_64.m    %.registrar.tvos.x86_64.h:    $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.dll    $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MTOUCH) --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(TVOS_SDK_VERSION)  $< --registrar:static --target-framework Xamarin.TVOS,1.0    --abi x86_64
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.tvos.arm64.m    %.registrar.tvos.arm64.h:    $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.dll    $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MTOUCH) --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(TVOS_SDK_VERSION)  $< --registrar:static --target-framework Xamarin.TVOS,1.0    --abi arm64
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.ios.simulator.a:     %.registrar.ios.i386.a %.registrar.ios.x86_64.a
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^

%.registrar.ios.device.a:        %.registrar.ios.arm64.a %.registrar.ios.armv7.a %.registrar.ios.armv7s.a
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^

REGISTRAR_CFLAGS=-stdlib=libc++ -std=c++14

%.registrar.ios.i386.a:   %.registrar.ios.i386.m      %.registrar.ios.i386.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATOR86_CFLAGS)    $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk/usr/include

%.registrar.ios.x86_64.a: %.registrar.ios.x86_64.m    %.registrar.ios.x86_64.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATOR64_CFLAGS)    $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk/usr/include

%.registrar.ios.armv7.a:   %.registrar.ios.armv7.m      %.registrar.ios.armv7.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(DEVICE7_CFLAGS)        $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphoneos.sdk/usr/include

%.registrar.ios.armv7s.a:   %.registrar.ios.armv7s.m      %.registrar.ios.armv7s.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(DEVICE7S_CFLAGS)       $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphoneos.sdk/usr/include

%.registrar.ios.arm64.a:   %.registrar.ios.arm64.m      %.registrar.ios.arm64.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(DEVICE64_CFLAGS)       $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphoneos.sdk/usr/include

%.registrar.watchos.i386.a:    %.registrar.watchos.i386.m  %.registrar.watchos.i386.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATORWATCH_CFLAGS) $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/include

%.registrar.watchos.armv7k.a:    %.registrar.watchos.armv7k.m  %.registrar.watchos.armv7k.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(DEVICEWATCH_CFLAGS)    $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/include

%.registrar.tvos.x86_64.a:       %.registrar.tvos.x86_64.m     %.registrar.tvos.x86_64.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATORTV_CFLAGS)    $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/include

%.registrar.tvos.arm64.a:       %.registrar.tvos.arm64.m     %.registrar.tvos.arm64.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(DEVICETV_CFLAGS)       $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/include

TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mtouch                                          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/mtouch.exe                               \
	$(foreach launcher,$(SIMLAUNCHERS),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/$(launcher)) \
	$(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/usr/lib/Xamarin.iOS.registrar.a       \

ifdef INCLUDE_DEVICE
TARGETS += $(IOS_DESTDIR)$(MONOTOUCH_DEVICE_SDK)/usr/lib/Xamarin.iOS.registrar.a
endif

ifdef INCLUDE_WATCH
TARGETS += \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/Xamarin.WatchOS.registrar.a \

ifdef INCLUDE_DEVICE
TARGETS += $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib/Xamarin.WatchOS.registrar.a
endif
endif


ifdef INCLUDE_TVOS
TARGETS +=	\
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/Xamarin.TVOS.registrar.a       \

ifdef INCLUDE_DEVICE
TARGETS += $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib/Xamarin.TVOS.registrar.a
endif
endif

TARGET_DIRS = \
	bin/Makefile                                                 \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch                 \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/etc/mono/assemblies/System \
	$(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/usr/lib             \
	$(IOS_DESTDIR)$(MONOTOUCH_DEVICE_SDK)/usr/lib                \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib          \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib                 \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib             \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib                    \

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

$(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/usr/lib/%.registrar.a: %.registrar.ios.simulator.a | $(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/usr/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/%.registrar.a: %.registrar.watchos.i386.a | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/%.registrar.a: %.registrar.tvos.x86_64.a | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_DEVICE_SDK)/usr/lib/%.registrar.a: %.registrar.ios.device.a | $(IOS_DESTDIR)$(MONOTOUCH_DEVICE_SDK)/usr/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib/%.registrar.a: %.registrar.watchos.armv7k.a | $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib/%.registrar.a: %.registrar.tvos.arm64.a | $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib
	$(Q) $(CP) $< $@

ifdef INCLUDE_IOS
install-local:: $(TARGETS)
all-local:: $(TARGETS)
endif

install-mtouch: \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mtouch                                          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/mtouch.exe                               \

clean-local::
	rm -Rf bin obj
	rm -f $(SIMLAUNCHERS)
	$(SYSTEM_MSBUILD) "/t:Clean" /p:Configuration=$(MTOUCH_CONF) *.csproj

include $(TOP)/mk/rules.mk
include ../common/Make.common

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# marking those files as .SECONDARY will prevent that deletion.
.SECONDARY: $(foreach ext,a h m,Xamarin.iOS.registrar.ios.i386.$(ext) Xamarin.iOS.registrar.ios.x86_64.$(ext))
.SECONDARY: $(foreach ext,a h m,Xamarin.WatchOS.registrar.watchos.$(ext))
.SECONDARY: $(foreach ext,a h m,Xamarin.TVOS.registrar.tvos.$(ext))
