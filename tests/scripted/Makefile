TOP=../..
include $(TOP)/Make.config

ALL_TESTS=select-native-compiler bug-13945 bug-35786

all-local tests run-tests:: $(ALL_TESTS)

select-native-compiler:
	$(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/clang++ -arch arm64 -miphoneos-version-min=$(IOS_SDK_VERSION) -isysroot $(XCODE_DEVELOPER_ROOT)/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS$(IOS_SDK_VERSION).sdk -Wall  -g  -c select-native-compiler/native.cpp -o select-native-compiler/libNative.a
	mkdir -p select-native-compiler/SelectNativeCompiler.app
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/btouch-native select-native-compiler/binding.cs --link-with=select-native-compiler/libNative.a,Native -s:select-native-compiler/managed.cs -o:select-native-compiler/binding.dll
	$(SYSTEM_CSC) select-native-compiler/app.cs -r:$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Xamarin.iOS.dll -r:select-native-compiler/binding.dll -out:select-native-compiler/app.exe
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/mtouch -r:select-native-compiler/binding.dll select-native-compiler/app.exe --abi=arm64  -sdk "$(IOS_SDK_VERSION)"  -dev:select-native-compiler/SelectNativeCompiler.app -sdkroot $(XCODE_DEVELOPER_ROOT) -r:$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Xamarin.iOS.dll

bug-13945 bug-35786:
	$(MAKE) -C $@

.PHONY: $(ALL_TESTS)

include $(TOP)/mk/rules.mk
