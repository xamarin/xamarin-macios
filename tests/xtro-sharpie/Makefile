TOP=../..

include $(TOP)/Make.config

# a 64bits mono is required because of the clang requirement
MONO ?= mono64 --debug
SHARPIE ?= sharpie

XCODE=$(abspath $(XCODE_DEVELOPER_ROOT)/../..)

# MacCatalyst is currently broken wrt sharpie and Xcode 14 beta 2, so skip it.
XTRO_DOTNET_PLATFORMS=$(filter-out MacCatalyst,$(DOTNET_PLATFORMS))

ANNOTATIONS_DIR=.
DOTNET_ANNOTATIONS_DIR=api-annotations-dotnet

# The PCSC framework causes compilation errors if CTCarrier.h is included,
# but we don't need the PCSC framework (we don't bind it), so just exclude it.
CORETELEPHONY_HEADERS = \
	-exclude PCSC \
	-i CoreTelephony/CoreTelephonyDefines.h \
	-i CoreTelephony/CTCarrier.h \
	-i CoreTelephony/CTCall.h \
	-i CoreTelephony/CTCallCenter.h \
	-i CoreTelephony/CTTelephonyNetworkInfo.h \
	-i CoreTelephony/CTSubscriber.h \
	-i CoreTelephony/CTSubscriberInfo.h \

all-local::

clean-local::
	rm -f xtro-report/bin/Debug/xtro-report.exe bin/Debug/xtro-sharpie.exe
	rm -f *.tmp
	rm -rf *os*.pch*

bin/Debug/xtro-sharpie.exe xtro-report/bin/Debug/xtro-report.exe xtro-sanity/bin/Debug/xtro-sanity.exe build: .stamp-build

.stamp-build: $(wildcard *.cs) pch-info.proj
	$(Q_BUILD) $(SYSTEM_MSBUILD) $(MSBUILD_VERBOSITY) xtro-sharpie.sln /r /bl:$@.binlog
	$(Q) touch $@

XIOS ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/64bits/iOS/Xamarin.iOS.dll
XIOS_GL ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/OpenTK-1.0.dll
XIOS_ARCH = arm64
XIOS_PCH = iphoneos$(IOS_SDK_VERSION)-$(XIOS_ARCH).pch
XIOS_RID = ios-arm64

$(XIOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s iphoneos$(IOS_SDK_VERSION) -a $(XIOS_ARCH) \
	-exclude RealityKit \
	-i ThreadNetwork/THClient.h \
	$(CORETELEPHONY_HEADERS) \


XWATCHOS ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/32bits/watchOS/Xamarin.WatchOS.dll

XWATCHOS_ARCH = armv7
XWATCHOS_PCH = watchos$(WATCH_SDK_VERSION)-$(XWATCHOS_ARCH).pch

$(XWATCHOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s watchos$(WATCH_SDK_VERSION) -a $(XWATCHOS_ARCH) \
		-exclude RealityKit \
		-exclude SharedWithYou \

XTVOS ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/64bits/tvOS/Xamarin.TVOS.dll
XTVOS_GL ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.TVOS/OpenTK-1.0.dll
XTVOS_ARCH = arm64
XTVOS_PCH = appletvos$(TVOS_SDK_VERSION)-$(XTVOS_ARCH).pch
XTVOS_RID = tvos-arm64

$(XTVOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s appletvos$(TVOS_SDK_VERSION) -a $(XTVOS_ARCH) \
		-exclude RealityKit \
		-exclude SharedWithYou \

XMACOS ?= $(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/64bits/mobile/Xamarin.Mac.dll
XMACOS_ARCH = x86_64
XMACOS_PCH = macosx$(MACOS_SDK_VERSION)-$(XMACOS_ARCH).pch
XMACOS_RID = osx-x64

$(XMACOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s macosx$(MACOS_SDK_VERSION) -a $(XMACOS_ARCH) \
		-exclude NearbyInteraction \
		-exclude RealityKit \
		-exclude JavaNativeFoundation \
		-exclude SharedWithYou \
		$(CORETELEPHONY_HEADERS) \

XMACCATALYST_ARCH = x86_64
XMACCATALYST_PCH = ios$(MACCATALYST_SDK_VERSION)-macabi-$(XMACCATALYST_ARCH).pch
XMACCATALYST_RID = maccatalyst-x64

$(XMACCATALYST_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s ios$(MACCATALYST_SDK_VERSION)-macabi -a $(XMACCATALYST_ARCH) \
	-exclude AGL \
	-exclude AudioVideoBridging \
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
	-exclude QuickLookUI \
	-exclude RealityKit \
	-exclude SecurityInterface \
	-exclude SharedWithYou \
	-exclude Virtualization \
	-i HomeKit/HomeKit.h \
	$(CORETELEPHONY_HEADERS) \

define DotNetAssembly
ifdef TESTS_USE_SYSTEM
X$(2)_DOTNET ?= $(DOTNET_DIR)/packs/$($(X$(2)_RID)_NUGET_RUNTIME_NAME)/$($(2)_WORKLOAD_VERSION)/runtimes/$(X$(2)_RID)/lib/$(DOTNET_TFM)/$(DOTNET_$(2)_ASSEMBLY_NAME).dll
else
X$(2)_DOTNET ?= $(DOTNET_DESTDIR)/$($(X$(2)_RID)_NUGET_RUNTIME_NAME)/runtimes/$(X$(2)_RID)/lib/$(DOTNET_TFM)/$(DOTNET_$(2)_ASSEMBLY_NAME).dll
endif

dotnet-$(1)-$($(2)_SDK_VERSION).g.cs: .stamp-check-sharpie
	$$(SHARPIE) query -bind $$(X$(2)_PCH) > $$@
endef
$(foreach platform,$(XTRO_DOTNET_PLATFORMS),$(eval $(call DotNetAssembly,$(platform),$(shell echo $(platform) | tr a-z A-Z))))

pch-info.proj: Makefile
	$(Q) rm -f $@.tmp
	$(Q) printf "<Project>\\n" >> $@.tmp
	$(Q) printf "\\t<PropertyGroup>\\n" >> $@.tmp
	$(Q) printf "$(foreach platform,$(ALL_DOTNET_PLATFORMS) watchOS,\\t\\t<$(platform)_PCH>$(X$(shell echo $(platform) | tr '[:lower:]' '[:upper:]')_PCH)</$(platform)_PCH>\\n)" >> $@.tmp
	$(Q) printf "$(foreach platform,$(ALL_DOTNET_PLATFORMS),\\t\\t<$(platform)_DLL>$(X$(shell echo $(platform) | tr '[:lower:]' '[:upper:]')_DOTNET)</$(platform)_DLL>\\n)" >> $@.tmp
	$(Q) printf "\\t</PropertyGroup>\\n" >> $@.tmp
	$(Q) printf "</Project>\\n" >> $@.tmp
	$(Q_GEN) mv $@.tmp $@

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

macos-$(MACOS_SDK_VERSION).g.cs: $(XMACOS_PCH)

ifdef INCLUDE_MAC
gen-macos: macos-$(MACOS_SDK_VERSION).g.cs .stamp-check-sharpie
	$(SHARPIE) query -bind $(XMACOS_PCH) > macos-$(MACOS_SDK_VERSION).g.cs
else
gen-macos: ; @true
endif

maccatalyst-$(MACCATALYST_SDK_VERSION).g.cs: $(XMACCATALYST_PCH)

ifdef INCLUDE_MACCATALYST
gen-maccatalyst: maccatalyst-$(MACCATALYST_SDK_VERSION).g.cs .stamp-check-sharpie
	$(SHARPIE) query -bind $(XMACCATALYST_PCH) > maccatalyst-$(MACCATALYST_SDK_VERSION).g.cs
else
gen-maccatalyst: ; @true
endif

gen-all: gen-ios gen-tvos gen-watchos gen-macos

wrench:
	$(MAKE) -j8 classify

dotnet-wrench:
	$(MAKE) -j8 dotnet-classify

ifdef INCLUDE_XAMARIN_LEGACY
report/index.html: xtro-report/bin/Debug/xtro-report.exe .stamp-classify
	rm -rf report/
	$(MONO) xtro-report/bin/Debug/xtro-report.exe $(ANNOTATIONS_DIR) report

report: report/index.html
endif

ifdef ENABLE_DOTNET
dotnet-report/index.html: xtro-report/bin/Debug/xtro-report.exe .stamp-dotnet-classify
	rm -rf report-dotnet
	$(MONO) xtro-report/bin/Debug/xtro-report.exe $(DOTNET_ANNOTATIONS_DIR) report-dotnet

dotnet-report: dotnet-report/index.html
endif

report-short:
	JENKINS_SERVER_COOKIE=1 make report

define DotNetClassify
.stamp-dotnet-classify-$(1): bin/Debug/xtro-sharpie.exe $$(X$(2)_PCH) $$(X$(2)_DOTNET)
	rm -f $$(DOTNET_ANNOTATIONS_DIR)/$(1)-*.raw
	$(MONO) bin/Debug/xtro-sharpie.exe --output-directory $$(DOTNET_ANNOTATIONS_DIR) $$(X$(2)_PCH) $$(X$(2)_DOTNET)
	touch $$@

dotnet-classify-$(1): .stamp-dotnet-classify-$(1)
DOTNET_CLASSIFY += .stamp-dotnet-classify-$(1)
endef
$(foreach platform,$(XTRO_DOTNET_PLATFORMS),$(eval $(call DotNetClassify,$(platform),$(shell echo $(platform) | tr a-z A-Z))))

ifdef INCLUDE_IOS
INCLUDED_PLATFORMS+=iOS
CLASSIFY+=.stamp-classify-ios
.stamp-classify-ios: bin/Debug/xtro-sharpie.exe $(XIOS_PCH) $(XIOS) $(XIOS_GL)
	rm -f $(ANNOTATIONS_DIR)/iOS-*.raw
	$(MONO) bin/Debug/xtro-sharpie.exe --output-directory $(ANNOTATIONS_DIR) $(XIOS_PCH) $(XIOS) $(XIOS_GL)
	$(Q) touch $@
endif

ifdef INCLUDE_TVOS
INCLUDED_PLATFORMS+=tvOS
CLASSIFY+=.stamp-classify-tvos
.stamp-classify-tvos: bin/Debug/xtro-sharpie.exe $(XTVOS_PCH) $(XTVOS) $(XTVOS_GL)
	rm -f $(ANNOTATIONS_DIR)/tvOS-*.raw
	$(MONO) bin/Debug/xtro-sharpie.exe --output-directory $(ANNOTATIONS_DIR) $(XTVOS_PCH) $(XTVOS) $(XTVOS_GL)
	$(Q) touch $@
endif

ifdef INCLUDE_WATCH
INCLUDED_PLATFORMS+=watchOS
CLASSIFY+=.stamp-classify-watchos
.stamp-classify-watchos: bin/Debug/xtro-sharpie.exe $(XWATCHOS_PCH) $(XWATCHOS)
	rm -f $(ANNOTATIONS_DIR)/watchOS-*.raw
	$(MONO) bin/Debug/xtro-sharpie.exe --output-directory $(ANNOTATIONS_DIR) $(XWATCHOS_PCH) $(XWATCHOS)
	$(Q) touch $@
endif

ifdef INCLUDE_MAC
INCLUDED_PLATFORMS+=macOS
CLASSIFY+=.stamp-classify-macos
.stamp-classify-macos: bin/Debug/xtro-sharpie.exe $(XMACOS_PCH) $(XMACOS)
	rm -f $(ANNOTATIONS_DIR)/macOS-*.raw
	$(MONO) bin/Debug/xtro-sharpie.exe --output-directory $(ANNOTATIONS_DIR) $(XMACOS_PCH) $(XMACOS)
	$(Q) touch $@
endif

ifdef INCLUDE_XAMARIN_LEGACY
.stamp-classify: xtro-sanity/bin/Debug/xtro-sanity.exe $(CLASSIFY)
	$(MONO) xtro-sanity/bin/Debug/xtro-sanity.exe $(abspath $(ANNOTATIONS_DIR)) "$(ALL_PLATFORMS)" $(INCLUDED_PLATFORMS)
	$(Q) touch $@

classify: .stamp-classify
endif

ifdef ENABLE_DOTNET
.stamp-dotnet-classify: xtro-sanity/bin/Debug/xtro-sanity.exe $(DOTNET_CLASSIFY)
	$(MONO) xtro-sanity/bin/Debug/xtro-sanity.exe $(abspath $(DOTNET_ANNOTATIONS_DIR)) "$(ALL_DOTNET_PLATFORMS)" $(XTRO_DOTNET_PLATFORMS)
	$(Q) touch $@

dotnet-classify: .stamp-dotnet-classify
endif

insane:
	XTRO_SANITY_SKIP=1 make all

remove-empty:
	find . -size 0 | xargs git rm

ifdef INCLUDE_XAMARIN_LEGACY
all: report
endif
ifdef ENABLE_DOTNET
all: dotnet-report
endif

.stamp-check-sharpie:
	@$(TOP)/system-dependencies.sh --ignore-all --enforce-sharpie
	@touch $@
	
remove-empty-files:
	find . -empty -exec git rm -f {} \;

U2TODO = u2todo/bin/Debug/u2todo.exe
$(U2TODO): $(wildcard u2todo/*.cs u2todo/*.csproj Filter.cs)
	$(Q) $(SYSTEM_MSBUILD) $(MSBUILD_VERBOSITY) /bl:$@.binlog /r
	$(Q) touch $@

unclassified2todo-classic: $(U2TODO)
	cd $(ANNOTATIONS_DIR) && $(MONO) $(abspath $(U2TODO))
	@for filename in $(git status -s | cut -c4- | grep .todo); do \
		sort -o "$filename" "$filename"; \
	done

unclassified2todo-dotnet: $(U2TODO)
	$(SYSTEM_CSC) -features:strict Filter.cs u2todo/u2todo.cs
	cd $(DOTNET_ANNOTATIONS_DIR) && $(MONO) $(abspath $(U2TODO))

unclassified2todo: unclassified2todo-classic unclassified2todo-dotnet
