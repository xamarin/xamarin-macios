TOP=../../..
include $(TOP)/Make.config

all-local:: bug-13945

bug-13945:
	@git clean -xfdq
	@echo "void foo () {}" > nativeA.m
	@echo "void bar () {}" > nativeB.m
	@mkdir -p .libs
	@$(MAKE) .libs/ios/nativeA.armv7.o
	@$(MAKE) .libs/ios/nativeB.armv7.o
	@rm -Rf TheApp.app cache cache-first
	@mkdir -p TheApp.app
	@mkdir -p cache

	@cp .libs/ios/nativeA.armv7.o libNative.a
	@$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/btouch binding.cs --link-with=libNative.a,Native -s:managed.cs -o:binding.dll
	@$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/smcs app.cs -r:$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/lib/mono/2.1/monotouch.dll -r:binding.dll -out:app.exe
	@$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/mtouch -r:binding.dll app.exe -nosign --abi=armv7  -sdk "$(IOS_SDK_VERSION)"  -dev:TheApp.app -sdkroot $(XCODE_DEVELOPER_ROOT) --cache=$(shell pwd)/cache

	@cp -Rp cache cache-first

	@cp .libs/ios/nativeB.armv7.o libNative.a
	@$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/btouch binding.cs --link-with=libNative.a,Native -s:managed.cs -o:binding.dll
	# do not rebuild the .exe
	@$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/mtouch -r:binding.dll app.exe -nosign --abi=armv7  -sdk "$(IOS_SDK_VERSION)"  -dev:TheApp.app -sdkroot $(XCODE_DEVELOPER_ROOT) --cache=$(shell pwd)/cache

	# this will verify that binding.dll wasn't AOT'ed again - if binding.dll.armv7.s differ then the AOT compiler executed.
	@diff -u cache-first/binding.dll.armv7.s cache/binding.dll.armv7.s
	@echo "$@: Success"

include $(TOP)/mk/rules.mk
