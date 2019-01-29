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
	-weak_framework NaturalLanguage			\
	-weak_framework Network				\
	-weak_framework IdentityLookupUI		\

# note: there _was_ no CoreAudioKit.framework or Metal.framework for the simulator (before recent iOS9 betas)
# note 2: there's no GameKit, in iOS 9 (beta 3 at least), you're supposed to reference GameCenter instead (looks fixed in beta 4)
# note 3: there's no MetalKit or MetalPerformanceShaders in iOS 9 beta 4 - but other frameworks were added
# note 4: GameCenter was removed in Xcode 7 beta 5, and GameKit is back.
# note 5: Metal* seems to be supported as of some Xcode 7 beta (b2 didn't, but the final release does)

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

#
# mtouch
#

MTOUCH_CONF=Release
MTOUCH_DIR=bin/$(MTOUCH_CONF)
LOCAL_MTOUCH=$(MTOUCH_DIR)/mtouch.exe

# mtouch.csproj.inc contains the mtouch_dependencies variable used to determine if mtouch needs to be rebuilt or not.
mtouch.csproj.inc: Makefile ../common/create-makefile-fragment.sh
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

%.registrar.watchos.m %.registrar.watchos.h: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.dll $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MTOUCH) --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(WATCH_SDK_VERSION) $< --registrar:static --target-framework Xamarin.WatchOS,1.0 --abi i386
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.tvos.m    %.registrar.tvos.h:    $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.dll    $(LOCAL_MTOUCH)
	$(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MTOUCH) --xamarin-framework-directory=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) $(MTOUCH_VERBOSITY) --runregistrar:$(abspath $(basename $@).m) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(TVOS_SDK_VERSION)  $< --registrar:static --target-framework Xamarin.TVOS,1.0    --abi x86_64
	$(Q) touch $(basename $@).m $(basename $@).h

%.registrar.ios.a:        %.registrar.ios.i386.a %.registrar.ios.x86_64.a
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^

# In Xcode 10 beta 1 simd/vector.h won't compile as Objective-C++ (can't find 'cmath'), so include a directory that contains the cmath file.
# This is filed as rdar://40824697
REGISTRAR_CFLAGS=-stdlib=libc++

%.registrar.ios.i386.a:   %.registrar.ios.i386.m      %.registrar.ios.i386.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATOR86_CFLAGS)    $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk/usr/include

%.registrar.ios.x86_64.a: %.registrar.ios.x86_64.m    %.registrar.ios.x86_64.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATOR64_CFLAGS)    $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk/usr/include

%.registrar.watchos.a:    %.registrar.watchos.m  %.registrar.watchos.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATORWATCH_CFLAGS) $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/include

%.registrar.tvos.a:       %.registrar.tvos.m     %.registrar.tvos.h
	$(Q_CC) $(IOS_CC) -DDEBUG -g -gdwarf-2 $(SIMULATORTV_CFLAGS)    $(REGISTRAR_CFLAGS) -x objective-c++ -o $@ -c $< -Wall -Wno-unguarded-availability-new -I$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/include

TARGETS = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mtouch                                          \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/mtouch.exe                               \
	$(foreach launcher,$(SIMLAUNCHERS),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/$(launcher)) \
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

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch/mtouch.exe: $(MTOUCH_DIR)/mtouch.exe | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mtouch
	$(Q) $(CP) $(dir $<)/*.exe $(dir $@)
	$(Q) $(CP) $(dir $<)/*.dll $(dir $@)
	$(Q) $(CP) $(dir $<)/*.pdb $(dir $@)

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/mtouch: mtouch.in | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin
	$(Q_GEN) sed 's!@MONOTOUCH_PREFIX@!$(abspath $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX))!g' $< > $@
	$(Q) chmod +x $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin/%: % | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/bin
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/%: % | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/usr/lib/%.registrar.a: %.registrar.ios.a | $(IOS_DESTDIR)$(MONOTOUCH_SIMULATOR_SDK)/usr/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/%.registrar.a: %.registrar.watchos.a | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib
	$(Q) $(CP) $< $@

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/%.registrar.a: %.registrar.tvos.a | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib
	$(Q) $(CP) $< $@

ifdef INCLUDE_IOS
install-local:: $(TARGETS)
all-local:: $(TARGETS)
endif

mtouch: \
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
