TOP=../..

include $(TOP)/Make.config

# a 64bits mono is required because of the clang requirement
MONO ?= mono64 --debug

all-local:: run-ios run-osx run-watchos run-tvos classify
	
sharpie: sharpie-ios sharpie-osx sharpie-watchos sharpie-tvos classify

clean-local::
	rm -f ios.results osx.results watchos.results tvos.results *.tmp
	rm -rf *os*.pch*
	
bin/Debug/xtro-sharpie.exe ./bin/Debug/xtro-plugin.dll build:
	$(Q_BUILD) msbuild $(XBUILD_VERBOSITY) xtro-sharpie.sln

	
XIOS ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/64bits/Xamarin.iOS.dll
XIOS_GL ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/OpenTK-1.0.dll
XIOS_ARCH = arm64
XIOS_PCH = iphoneos$(IOS_SDK_VERSION)-$(XIOS_ARCH).pch

run-ios: build $(XIOS_PCH)
	$(MONO) bin/Debug/xtro-sharpie.exe $(XIOS_PCH) $(XIOS) $(XIOS_GL) | sort > ios.results

sharpie-ios: build $(XIOS_PCH)
	sharpie ./bin/Debug/xtro-plugin.dll extro $(XIOS_PCH) $(XIOS) $(XIOS_GL) | sort > ios.results

$(XIOS_PCH):
	sharpie sdk-db -s iphoneos$(IOS_SDK_VERSION) -a $(XIOS_ARCH)


XWATCHOS ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/32bits/Xamarin.WatchOS.dll
XWATCHOS_ARCH = armv7
XWATCHOS_PCH = watchos$(WATCH_SDK_VERSION)-$(XWATCHOS_ARCH).pch

run-watchos: build $(XWATCHOS_PCH)
	$(MONO) bin/Debug/xtro-sharpie.exe $(XWATCHOS_PCH) $(XWATCHOS) | sort > watchos.results

sharpie-watchos: build $(XWATCHOS_PCH)
	sharpie ./bin/Debug/xtro-plugin.dll extro $(XWATCHOS_PCH) $(XWATCHOS) | sort > watchos.results

$(XWATCHOS_PCH):
	sharpie sdk-db -s watchos$(WATCH_SDK_VERSION) -a $(XWATCHOS_ARCH)


XTVOS ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/mono/Xamarin.TVOS/Xamarin.TVOS.dll
XTVOS_GL ?= $(TOP)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/mono/Xamarin.TVOS/OpenTK-1.0.dll
XTVOS_ARCH = arm64
XTVOS_PCH = appletvos$(TVOS_SDK_VERSION)-$(XTVOS_ARCH).pch

run-tvos: build $(XTVOS_PCH)
	$(MONO) bin/Debug/xtro-sharpie.exe $(XTVOS_PCH) $(XTVOS) $(XTVOS_GL) | sort > tvos.results

sharpie-tvos: build $(XTVOS_PCH)
	sharpie ./bin/Debug/xtro-plugin.dll extro $(XTVOS_PCH) $(XTVOS) $(XTVOS_GL) | sort > tvos.results

$(XTVOS_PCH):
	sharpie sdk-db -s appletvos$(TVOS_SDK_VERSION) -a $(XTVOS_ARCH)


XMAC ?= $(TOP)/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/git/lib/$(XMAC_ARCH)/mobile/Xamarin.Mac.dll
XMAC_ARCH = x86_64
XMAC_PCH = macosx$(OSX_SDK_VERSION)-$(XMAC_ARCH).pch

run-osx: build $(XMAC_PCH)
	$(MONO) bin/Debug/xtro-sharpie.exe $(XMAC_PCH) $(XMAC) | sort > osx.results

sharpie-osx: build $(XMAC_PCH)
	sharpie ./bin/Debug/xtro-plugin.dll extro $(XMAC_PCH) $(XMAC) | sort > osx.results

$(XMAC_PCH):
	sharpie sdk-db -s macosx$(OSX_SDK_VERSION) -a $(XMAC_ARCH) -x InputMethodKit

preclassify: ios.results osx.results
	@comm -12 ios.results osx.results > common.unclassified.tmp

classify-ios: preclassify 
	@sort ios.ignore ios.pending common.ignore common.pending > ios.skip.tmp
	@comm -23 ios.results common.unclassified.tmp > ios.unclassified.tmp
	@comm -23 ios.unclassified.tmp ios.skip.tmp > ios.unclassified

classify-tvos: preclassify 
	@sort tvos.ignore tvos.pending ios.ignore ios.pending common.ignore common.pending > tvos.skip.tmp
	@comm -23 tvos.results common.unclassified.tmp > tvos.unclassified.tmp
	@comm -23 tvos.unclassified.tmp tvos.skip.tmp > tvos.unclassified

classify-watchos: preclassify 
	@sort watchos.ignore watchos.pending common.ignore common.pending > watchos.skip.tmp
	@comm -23 watchos.results common.unclassified.tmp > watchos.unclassified.tmp
	@comm -23 watchos.unclassified.tmp watchos.skip.tmp > watchos.unclassified

classify-osx: preclassify 
	@sort osx.ignore osx.pending common.ignore common.pending > osx.skip.tmp
	@comm -23 osx.results common.unclassified.tmp > osx.unclassified.tmp
	@comm -23 osx.unclassified.tmp osx.skip.tmp > osx.unclassified

classify: classify-ios classify-osx classify-watchos classify-tvos
	@sort common.ignore common.pending ios.skip.tmp osx.skip.tmp > common.skip.tmp
	@comm -23 common.unclassified.tmp common.skip.tmp > common.unclassified
	@rm *.tmp *.results


ios-$(IOS_SDK_VERSION).g.cs: $(XIOS_PCH)

gen-ios: ios-$(IOS_SDK_VERSION).g.cs
	sharpie query -bind $(XIOS_PCH) > ios-$(IOS_SDK_VERSION).g.cs

tvos-$(TVOS_SDK_VERSION).g.cs: $(XTVOS_PCH)

gen-tvos: tvos-$(TVOS_SDK_VERSION).g.cs
	sharpie query -bind $(XTVOS_PCH) > tvos-$(TVOS_SDK_VERSION).g.cs

watchos-$(WATCH_SDK_VERSION).g.cs: $(XWATCHOS_PCH)

gen-watchos: watchos-$(WATCH_SDK_VERSION).g.cs
	sharpie query -bind $(XWATCHOS_PCH) > watchos-$(WATCH_SDK_VERSION).g.cs

macos-$(OSX_SDK_VERSION).g.cs: $(XMAC_PCH)

gen-macos: macos-$(OSX_SDK_VERSION).g.cs
	sharpie query -bind $(XMAC_PCH) > macos-$(OSX_SDK_VERSION).g.cs

gen-all: gen-ios gen-tvos gen-watchos gen-macos
