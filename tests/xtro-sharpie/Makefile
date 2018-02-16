TOP=../..

include $(TOP)/Make.config

# a 64bits mono is required because of the clang requirement
MONO ?= mono64 --debug

XCODE=$(abspath $(XCODE_DEVELOPER_ROOT)/../..)

all-local::

clean-local::
	rm -f xtro-report/bin/Debug/xtro-report.exe bin/Debug/xtro-sharpie.exe
	rm -f *.tmp
	rm -rf *os*.pch*

bin/Debug/xtro-sharpie.exe xtro-report/bin/Debug/xtro-report.exe build:
	$(Q) nuget restore xtro-sharpie.sln
	$(Q_BUILD) $(SYSTEM_MSBUILD) $(XBUILD_VERBOSITY) xtro-sharpie.sln

XIOS ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/64bits/Xamarin.iOS.dll
XIOS_GL ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/OpenTK-1.0.dll
XIOS_ARCH = arm64
XIOS_PCH = iphoneos$(IOS_SDK_VERSION)-$(XIOS_ARCH).pch

$(XIOS_PCH): .stamp-check-sharpie
	sharpie sdk-db --xcode $(XCODE) -s iphoneos$(IOS_SDK_VERSION) -a $(XIOS_ARCH)


XWATCHOS ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/32bits/Xamarin.WatchOS.dll
XWATCHOS_ARCH = armv7
XWATCHOS_PCH = watchos$(WATCH_SDK_VERSION)-$(XWATCHOS_ARCH).pch

$(XWATCHOS_PCH): .stamp-check-sharpie
	sharpie sdk-db --xcode $(XCODE) -s watchos$(WATCH_SDK_VERSION) -a $(XWATCHOS_ARCH)

XTVOS ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/mono/Xamarin.TVOS/Xamarin.TVOS.dll
XTVOS_GL ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/mono/Xamarin.TVOS/OpenTK-1.0.dll
XTVOS_ARCH = arm64
XTVOS_PCH = appletvos$(TVOS_SDK_VERSION)-$(XTVOS_ARCH).pch

$(XTVOS_PCH): .stamp-check-sharpie
	sharpie sdk-db --xcode $(XCODE) -s appletvos$(TVOS_SDK_VERSION) -a $(XTVOS_ARCH)


XMAC ?= $(TOP)/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/lib/$(XMAC_ARCH)/mobile/Xamarin.Mac.dll
XMAC_ARCH = x86_64
XMAC_PCH = macosx$(OSX_SDK_VERSION)-$(XMAC_ARCH).pch

$(XMAC_PCH): .stamp-check-sharpie
	sharpie sdk-db --xcode $(XCODE) -s macosx$(OSX_SDK_VERSION) -a $(XMAC_ARCH)



ios-$(IOS_SDK_VERSION).g.cs: $(XIOS_PCH)

gen-ios: ios-$(IOS_SDK_VERSION).g.cs .stamp-check-sharpie
	sharpie query -bind $(XIOS_PCH) > ios-$(IOS_SDK_VERSION).g.cs

tvos-$(TVOS_SDK_VERSION).g.cs: $(XTVOS_PCH)

gen-tvos: tvos-$(TVOS_SDK_VERSION).g.cs .stamp-check-sharpie
	sharpie query -bind $(XTVOS_PCH) > tvos-$(TVOS_SDK_VERSION).g.cs

watchos-$(WATCH_SDK_VERSION).g.cs: $(XWATCHOS_PCH)

gen-watchos: watchos-$(WATCH_SDK_VERSION).g.cs .stamp-check-sharpie
	sharpie query -bind $(XWATCHOS_PCH) > watchos-$(WATCH_SDK_VERSION).g.cs

macos-$(OSX_SDK_VERSION).g.cs: $(XMAC_PCH)

gen-macos: macos-$(OSX_SDK_VERSION).g.cs .stamp-check-sharpie
	sharpie query -bind $(XMAC_PCH) > macos-$(OSX_SDK_VERSION).g.cs

gen-all: gen-ios gen-tvos gen-watchos gen-macos
	

wrench: classify

report: build
	rm -rf report/
	$(MONO) xtro-report/bin/Debug/xtro-report.exe .

report-short:
	JENKINS_SERVER_COOKIE=1 make report

classify: build $(XIOS_PCH) $(XWATCHOS_PCH) $(XTVOS_PCH) $(XMAC_PCH)
	rm -f *.unclassified
	$(MONO) bin/Debug/xtro-sharpie.exe $(XIOS_PCH) $(XIOS) $(XIOS_GL)
	$(MONO) bin/Debug/xtro-sharpie.exe $(XWATCHOS_PCH) $(XWATCHOS)
	$(MONO) bin/Debug/xtro-sharpie.exe $(XTVOS_PCH) $(XTVOS) $(XTVOS_GL)
	$(MONO) bin/Debug/xtro-sharpie.exe $(XMAC_PCH) $(XMAC)
	$(MONO) xtro-sanity/bin/Debug/xtro-sanity.exe .
	rm -f *.raw

insane:
	XTRO_SANITY_SKIP=1 make all

all: classify report

.stamp-check-sharpie:
	@$(TOP)/system-dependencies.sh --ignore-all --enforce-sharpie
	@touch $@
