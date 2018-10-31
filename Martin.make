ROOT = /Developer/Work/xamarin-macios
MONO_ROOT = $(ROOT)/external/mono
COREFX_ROOT = $(MONO_ROOT)/external/corefx
SIM64_BUILD_ROOT = $(MONO_ROOT)/sdks/builds/ios-sim64-release
SIM64_BUILD_OUT = $(MONO_ROOT)/sdks/out/ios-sim64-release
TARGET32_BUILD_ROOT = $(MONO_ROOT)/sdks/builds/ios-target32-release
TARGET32_BUILD_OUT = $(MONO_ROOT)/sdks/out/ios-target32-release
MAC_BUILD_ROOT = $(ROOT)/builds/mac64
MAC_BUILD_OUT = $(ROOT)/builds/install/mac64

NATIVE_APPLE_DIR = $(COREFX_ROOT)/src/Native/Unix/System.Security.Cryptography.Native.Apple

IOS_SDK_INSTALL = $(ROOT)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git
MAC_SDK_INSTALL = $(ROOT)/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git

all::
	@echo $(ROOT)

dir-native-apple::
	@echo $(NATIVE_APPLE_DIR)

corlib-monotouch::
	$(MAKE) -C $(MONO_ROOT)/mcs/class/corlib PROFILE=monotouch all install
	cp $(MONO_ROOT)/mcs/class/lib/monotouch/mscorlib.dll $(IOS_SDK_INSTALL)/lib/mono/Xamarin.iOS/
	cp $(MONO_ROOT)/mcs/class/lib/monotouch/mscorlib.pdb $(IOS_SDK_INSTALL)/lib/mono/Xamarin.iOS/

system-monotouch::
	$(MAKE) -C $(MONO_ROOT)/mcs/class/System PROFILE=monotouch all install
	cp $(MONO_ROOT)/mcs/class/lib/monotouch/System.dll $(IOS_SDK_INSTALL)/lib/mono/Xamarin.iOS/
	cp $(MONO_ROOT)/mcs/class/lib/monotouch/System.pdb $(IOS_SDK_INSTALL)/lib/mono/Xamarin.iOS/

system-core-monotouch::
	$(MAKE) -C $(MONO_ROOT)/mcs/class/System.Core PROFILE=monotouch all install
	cp $(MONO_ROOT)/mcs/class/lib/monotouch/System.Core.dll $(IOS_SDK_INSTALL)/lib/mono/Xamarin.iOS/
	cp $(MONO_ROOT)/mcs/class/lib/monotouch/System.Core.pdb $(IOS_SDK_INSTALL)/lib/mono/Xamarin.iOS/

sim64-runtime::
	$(MAKE) -C $(SIM64_BUILD_ROOT)/mono all install
	cp $(SIM64_BUILD_OUT)/lib/libmono-apple-crypto.* $(IOS_SDK_INSTALL)/SDKs/MonoTouch.iphonesimulator.sdk/usr/lib/
	cp $(SIM64_BUILD_OUT)/lib/libmonosgen-* $(IOS_SDK_INSTALL)/SDKs/MonoTouch.iphonesimulator.sdk/usr/lib/

target32-runtime::
	$(MAKE) -C $(TARGET32_BUILD_ROOT)/mono all install
	#cp $(TARGET32_BUILD_OUT)/lib/libmono-native* $(IOS_SDK_INSTALL)/SDKs/MonoTouch.iphoneos.sdk/usr/lib/
	#cp $(TARGET32_BUILD_OUT)/lib/libmonosgen-* $(IOS_SDK_INSTALL)/SDKs/MonoTouch.iphoneos.sdk/usr/lib/

mac-runtime::
	$(MAKE) -C $(MAC_BUILD_ROOT)/mono all install
	cp $(MAC_BUILD_OUT)/lib/libmono-apple-crypto.* $(MAC_SDK_INSTALL)/lib/
	cp $(MAC_BUILD_OUT)/lib/libmonosgen-* $(MAC_SDK_INSTALL)/lib/

corlib-xammac::
	$(MAKE) -C $(MONO_ROOT)/mcs/class/corlib PROFILE=xammac all install
	cp $(MONO_ROOT)/mcs/class/lib/xammac/mscorlib.dll $(MAC_SDK_INSTALL)/lib/mono/Xamarin.Mac/
	cp $(MONO_ROOT)/mcs/class/lib/xammac/mscorlib.pdb $(MAC_SDK_INSTALL)/lib/mono/Xamarin.Mac/

system-xammac::
	$(MAKE) -C $(MONO_ROOT)/mcs/class/System PROFILE=xammac all install
	cp $(MONO_ROOT)/mcs/class/lib/xammac/System.dll $(MAC_SDK_INSTALL)/lib/mono/Xamarin.Mac/
	cp $(MONO_ROOT)/mcs/class/lib/xammac/System.pdb $(MAC_SDK_INSTALL)/lib/mono/Xamarin.Mac/

system-core-xammac::
	$(MAKE) -C $(MONO_ROOT)/mcs/class/System.Core PROFILE=xammac all install
	cp $(MONO_ROOT)/mcs/class/lib/xammac/System.Core.dll $(MAC_SDK_INSTALL)/lib/mono/Xamarin.Mac/
	cp $(MONO_ROOT)/mcs/class/lib/xammac/System.Core.pdb $(MAC_SDK_INSTALL)/lib/mono/Xamarin.Mac/

nm-monotouch-crypto::
	nm $(SIM64_BUILD_ROOT)/mono/metadata/.libs/libmono-apple-crypto.dylib

nm-xammac-crypto::
	nm $(MAC_BUILD_ROOT)/mono/metadata/.libs/libmono-apple-crypto.dylib

