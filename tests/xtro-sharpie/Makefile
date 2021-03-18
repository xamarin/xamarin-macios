TOP=../..

include $(TOP)/Make.config

# a 64bits mono is required because of the clang requirement
MONO ?= mono64 --debug
SHARPIE ?= sharpie

XCODE=$(abspath $(XCODE_DEVELOPER_ROOT)/../..)

all-local::

clean-local::
	rm -f xtro-report/bin/Debug/xtro-report.exe bin/Debug/xtro-sharpie.exe
	rm -f *.tmp
	rm -rf *os*.pch*

bin/Debug/xtro-sharpie.exe xtro-report/bin/Debug/xtro-report.exe build:
	$(Q) $(SYSTEM_XIBUILD) -t -- /Library/Frameworks/Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore xtro-sharpie.sln
	$(Q_BUILD) $(SYSTEM_MSBUILD) $(MSBUILD_VERBOSITY) xtro-sharpie.sln

XIOS ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/64bits/iOS/Xamarin.iOS.dll
XIOS_GL ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/OpenTK-1.0.dll
XIOS_ARCH = arm64
XIOS_PCH = iphoneos$(IOS_SDK_VERSION)-$(XIOS_ARCH).pch

$(XIOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s iphoneos$(IOS_SDK_VERSION) -a $(XIOS_ARCH) -exclude OSLog


XWATCHOS ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/32bits/watchOS/Xamarin.WatchOS.dll
XWATCHOS_ARCH = armv7
XWATCHOS_PCH = watchos$(WATCH_SDK_VERSION)-$(XWATCHOS_ARCH).pch

$(XWATCHOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s watchos$(WATCH_SDK_VERSION) -a $(XWATCHOS_ARCH) -exclude OSLog

XTVOS ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/64bits/tvOS/Xamarin.TVOS.dll
XTVOS_GL ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/mono/Xamarin.TVOS/OpenTK-1.0.dll
XTVOS_ARCH = arm64
XTVOS_PCH = appletvos$(TVOS_SDK_VERSION)-$(XTVOS_ARCH).pch

$(XTVOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s appletvos$(TVOS_SDK_VERSION) -a $(XTVOS_ARCH) -exclude OSLog


XMAC ?= $(TOP)/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/lib/64bits/mobile/Xamarin.Mac.dll
XMAC_ARCH = x86_64
XMAC_PCH = macosx$(OSX_SDK_VERSION)-$(XMAC_ARCH).pch

$(XMAC_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s macosx$(OSX_SDK_VERSION) -a $(XMAC_ARCH) -exclude ClockKit -exclude JavaNativeFoundation

XCAT ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/64bits/MacCatalyst/Xamarin.MacCatalyst.dll
XCAT_GL ?=
XCAT_ARCH = x86_64
XCAT_PCH = ios$(MACCATALYST_SDK_VERSION)-macabi-$(XCAT_ARCH).pch

$(XCAT_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s ios$(MACCATALYST_SDK_VERSION)-macabi -a $(XCAT_ARCH) \
	-exclude AGL \
	-exclude CalendarStore \
	-exclude Carbon \
	-exclude ClockKit \
	-exclude DiscRecordingUI \
	-exclude GLKit \
	-exclude ICADevices \
	-exclude InputMethodKit \
	-exclude InstallerPlugins \
	-exclude IOBluetooth \
	-exclude IOBluetoothUI \
	-exclude JavaNativeFoundation \
	-exclude LDAP \
	-exclude Python \
	-exclude Quartz \
	-exclude SecurityInterface \


maccatalyst: $(XCAT_PCH)

ios-$(IOS_SDK_VERSION).g.cs: $(XIOS_PCH)

ifdef INCLUDE_IOS
gen-ios: ios-$(IOS_SDK_VERSION).g.cs .stamp-check-sharpie
	$(SHARPIE) query -bind $(XIOS_PCH) > ios-$(IOS_SDK_VERSION).g.cs
else
gen-ios: ; @true
endif

tvos-$(TVOS_SDK_VERSION).g.cs: $(XTVOS_PCH)

ifdef INCLUDE_TVOS
gen-tvos: tvos-$(TVOS_SDK_VERSION).g.cs .stamp-check-sharpie
	$(SHARPIE) query -bind $(XTVOS_PCH) > tvos-$(TVOS_SDK_VERSION).g.cs
else
gen-tvos: ; @true
endif

watchos-$(WATCH_SDK_VERSION).g.cs: $(XWATCHOS_PCH)

ifdef INCLUDE_WATCH
gen-watchos: watchos-$(WATCH_SDK_VERSION).g.cs .stamp-check-sharpie
	$(SHARPIE) query -bind $(XWATCHOS_PCH) > watchos-$(WATCH_SDK_VERSION).g.cs
else
gen-watchos: ; @true
endif

macos-$(OSX_SDK_VERSION).g.cs: $(XMAC_PCH)

ifdef INCLUDE_MAC
gen-macos: macos-$(OSX_SDK_VERSION).g.cs .stamp-check-sharpie
	$(SHARPIE) query -bind $(XMAC_PCH) > macos-$(OSX_SDK_VERSION).g.cs
else
gen-macos: ; @true
endif

maccatalyst-$(MACCATALYST_SDK_VERSION).g.cs: $(XMAC_PCH)

ifdef INCLUDE_MACCATALYST
gen-maccatalyst: maccatalyst-$(MACCATALYST_SDK_VERSION).g.cs .stamp-check-sharpie
	$(SHARPIE) query -bind $(XCAT_PCH) > maccatalyst-$(MACCATALYST_SDK_VERSION).g.cs
else
gen-maccatalyst: ; @true
endif

gen-all: gen-ios gen-tvos gen-watchos gen-macos gen-maccatalyst

wrench: classify

report: build
	rm -rf report/
	$(MONO) xtro-report/bin/Debug/xtro-report.exe .

report-short:
	JENKINS_SERVER_COOKIE=1 make report

ifdef INCLUDE_IOS
classify-ios:
	$(MONO) bin/Debug/xtro-sharpie.exe $(XIOS_PCH) $(XIOS) $(XIOS_GL)
else
classify-ios: ; @true
endif

ifdef INCLUDE_TVOS
classify-tvos:
	$(MONO) bin/Debug/xtro-sharpie.exe $(XTVOS_PCH) $(XTVOS) $(XTVOS_GL)
else
classify-tvos: ; @true
endif

ifdef INCLUDE_WATCH
classify-watchos:
	$(MONO) bin/Debug/xtro-sharpie.exe $(XWATCHOS_PCH) $(XWATCHOS)
else
classify-watchos: ; @true
endif

ifdef INCLUDE_MAC
classify-macos:
	$(MONO) bin/Debug/xtro-sharpie.exe $(XMAC_PCH) $(XMAC)
else
classify-macos: ; @true
endif

ifdef INCLUDE_MACCATALYST
classify-maccatalyst: bin/Debug/xtro-sharpie.exe $(XCAT_PCH)
	$(MONO) bin/Debug/xtro-sharpie.exe $(XCAT_PCH) $(XCAT)
else
classify-maccatalyst: ; @true
endif

classify: build $(XIOS_PCH) $(XWATCHOS_PCH) $(XTVOS_PCH) $(XMAC_PCH) $(XCAT_PCH)
	rm -f *.unclassified
	$(MAKE) -j8 classify-ios classify-tvos classify-watchos classify-macos classify-maccatalyst
	$(MONO) xtro-sanity/bin/Debug/xtro-sanity.exe .
	rm -f *.raw

insane:
	XTRO_SANITY_SKIP=1 make all

remove-empty:
	find . -size 0 | xargs git rm

all: classify report

.stamp-check-sharpie:
	@$(TOP)/system-dependencies.sh --ignore-all --enforce-sharpie
	@touch $@
	
remove-empty-files:
	find . -empty -exec git rm -f {} \;

unclassified2todo:
	$(SYSTEM_CSC) -features:strict Filter.cs u2todo/u2todo.cs
	$(MONO) u2todo.exe
