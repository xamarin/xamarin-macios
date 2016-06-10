TOP = ../..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

SHARPIE_INSTALLED = $(shell which sharpie)

clean-local::
	@rm -rf ./build

ifeq ($(SHARPIE_INSTALLED),)
$(warning Sharpie not installed, skipping test.)
all-local::
else
all-local::
	mkdir -p build
	rm -rf ./build/SDKs
	sharpie sdk-db -sdk macosx10.10 -o build/SDKs -arch x86_64
	cd src; nuget restore -PackagesDirectory ../build/test/packages -NonInteractive  
	$(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) src/test.csproj
	sharpie ./build/test/Sharpie.APITestTool.dll APITest build/SDKs/macosx10.10-x86_64.pch $(TOP)/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/x86_64/mobile/Xamarin.Mac.dll
endif
