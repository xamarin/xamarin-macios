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
	rm -rf */bin */obj
	rm -f *.tmp
	rm -rf *os*.pch*

XTRO_REPORT=xtro-report/bin/Debug/xtro-report
XTRO_REPORT_EXEC=$(XTRO_REPORT)
XTRO_SANITY=xtro-sanity/bin/Debug/xtro-sanity
XTRO_SANITY_EXEC=$(XTRO_SANITY)
XTRO_SHARPIE=xtro-sharpie/bin/Debug/xtro-sharpie.exe
XTRO_SHARPIE_EXEC=$(MONO) --debug $(XTRO_SHARPIE)

build: $(XTRO_SHARPIE) $(XTRO_REPORT) $(XTRO_SANITY)

.stamp-build: $(wildcard *.cs) pch-info.proj $(XTRO_REPORT) $(XTRO_SANITY) $(XTRO_SHARPIE)
	$(Q) touch $@

$(XTRO_SHARPIE): $(wildcard xtro-sharpie/*.cs) $(wildcard xtro-sharpie/*.csproj) pch-info.proj $(XTRO_REPORT)
	$(Q_BUILD) $(SYSTEM_MSBUILD) $(MSBUILD_VERBOSITY) xtro-sharpie/xtro-sharpie.csproj /r /bl:$@.binlog

$(XTRO_REPORT): $(wildcard xtro-report/*.cs) $(wildcard xtro-report/*.csproj) xtro-sharpie/Filter.cs Makefile
	$(Q_GEN) unset MSBUILD_EXE_PATH && $(DOTNET) build xtro-report/xtro-report.csproj /bl:xtro-report.binlog $(DOTNET_BUILD_VERBOSITY)

$(XTRO_SANITY): $(wildcard xtro-sanity/*.cs) $(wildcard xtro-sanity/*.csproj) $(wildcard $(TOP)/tools/common/*.cs) Makefile
	$(Q_GEN) unset MSBUILD_EXE_PATH && $(DOTNET) build xtro-sanity/xtro-sanity.csproj /bl:xtro-sanity.binlog $(DOTNET_BUILD_VERBOSITY)

XIOS ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/64bits/iOS/Xamarin.iOS.dll
XIOS_LIBDIR ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS
XIOS_GL ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/OpenTK-1.0.dll
XIOS_ARCH = arm64
XIOS_PCH = iphoneos$(IOS_SDK_VERSION)-$(XIOS_ARCH).pch
XIOS_RID = ios-arm64

$(XIOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s iphoneos$(IOS_SDK_VERSION) -a $(XIOS_ARCH) \
	-exclude FactoryOTAWifiUtils \
	-exclude FactoryOTALogger \
	-exclude FactoryOTANetworkUtils \
	-i ThreadNetwork/THClient.h \
	$(CORETELEPHONY_HEADERS) \


XWATCHOS ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/32bits/watchOS/Xamarin.WatchOS.dll
XWATCHOS_LIBDIR ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.WatchOS

XWATCHOS_ARCH = armv7
XWATCHOS_PCH = watchos$(WATCH_SDK_VERSION)-$(XWATCHOS_ARCH).pch

$(XWATCHOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s watchos$(WATCH_SDK_VERSION) -a $(XWATCHOS_ARCH)

XTVOS ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/64bits/tvOS/Xamarin.TVOS.dll
XTVOS_LIBDIR ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.TVOS/
XTVOS_GL ?= $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.TVOS/OpenTK-1.0.dll
XTVOS_ARCH = arm64
XTVOS_PCH = appletvos$(TVOS_SDK_VERSION)-$(XTVOS_ARCH).pch
XTVOS_RID = tvos-arm64

$(XTVOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s appletvos$(TVOS_SDK_VERSION) -a $(XTVOS_ARCH)

XMACOS ?= $(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/64bits/mobile/Xamarin.Mac.dll
XMACOS_LIBDIR ?= $(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac
XMACOS_ARCH = x86_64
XMACOS_PCH = macosx$(MACOS_SDK_VERSION)-$(XMACOS_ARCH).pch
XMACOS_RID = osx-x64

$(XMACOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s macosx$(MACOS_SDK_VERSION) -a $(XMACOS_ARCH) \
		-exclude JavaNativeFoundation \
		-exclude ParavirtualizedGraphics \
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
	-exclude SecurityInterface \
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
report/index.html: $(XTRO_REPORT) .stamp-classify
	$(Q) rm -rf report/
	$(Q_GEN) $(XTRO_REPORT_EXEC) $(ANNOTATIONS_DIR) report

report: report/index.html
endif

ifdef ENABLE_DOTNET
report-dotnet/index.html: $(XTRO_REPORT) .stamp-dotnet-classify
	$(Q) rm -rf report-dotnet
	$(Q_GEN) $(XTRO_REPORT_EXEC) $(DOTNET_ANNOTATIONS_DIR) report-dotnet

dotnet-report: report-dotnet/index.html
endif

report-short:
	JENKINS_SERVER_COOKIE=1 make report

define DotNetClassify
.stamp-dotnet-classify-$(1): $(XTRO_SHARPIE) $$(X$(2)_PCH) $$(X$(2)_DOTNET)
	$(Q) rm -f $$(DOTNET_ANNOTATIONS_DIR)/$(1)-*.raw
	$(Q_GEN) $(XTRO_SHARPIE_EXEC) --output-directory $$(DOTNET_ANNOTATIONS_DIR) --lib $(DOTNET_BCL_DIR) $$(X$(2)_PCH) $$(X$(2)_DOTNET)
	$(Q) touch $$@

dotnet-classify-$(1): .stamp-dotnet-classify-$(1)
DOTNET_CLASSIFY += .stamp-dotnet-classify-$(1)
endef
$(foreach platform,$(XTRO_DOTNET_PLATFORMS),$(eval $(call DotNetClassify,$(platform),$(shell echo $(platform) | tr a-z A-Z))))

ifdef INCLUDE_IOS
INCLUDED_PLATFORMS+=iOS
CLASSIFY+=.stamp-classify-ios
.stamp-classify-ios: $(XTRO_SHARPIE) $(XIOS_PCH) $(XIOS) $(XIOS_GL)
	$(Q) rm -f $(ANNOTATIONS_DIR)/iOS-*.raw
	$(Q_GEN) $(XTRO_SHARPIE_EXEC) --output-directory $(ANNOTATIONS_DIR) --lib $(XIOS_LIBDIR) $(XIOS_PCH) $(XIOS) $(XIOS_GL)
	$(Q) touch $@
endif

ifdef INCLUDE_TVOS
INCLUDED_PLATFORMS+=tvOS
CLASSIFY+=.stamp-classify-tvos
.stamp-classify-tvos: $(XTRO_SHARPIE) $(XTVOS_PCH) $(XTVOS) $(XTVOS_GL)
	$(Q) rm -f $(ANNOTATIONS_DIR)/tvOS-*.raw
	$(Q_GEN) $(XTRO_SHARPIE_EXEC) --output-directory $(ANNOTATIONS_DIR) --lib $(XTVOS_LIBDIR) $(XTVOS_PCH) $(XTVOS) $(XTVOS_GL)
	$(Q) touch $@
endif

ifdef INCLUDE_WATCH
INCLUDED_PLATFORMS+=watchOS
CLASSIFY+=.stamp-classify-watchos
.stamp-classify-watchos: $(XTRO_SHARPIE) $(XWATCHOS_PCH) $(XWATCHOS)
	$(Q) rm -f $(ANNOTATIONS_DIR)/watchOS-*.raw
	$(Q_GEN) $(XTRO_SHARPIE_EXEC) --output-directory $(ANNOTATIONS_DIR) --lib $(XWATCHOS_LIBDIR) $(XWATCHOS_PCH) $(XWATCHOS)
	$(Q) touch $@
endif

ifdef INCLUDE_MAC
INCLUDED_PLATFORMS+=macOS
CLASSIFY+=.stamp-classify-macos
.stamp-classify-macos: $(XTRO_SHARPIE) $(XMACOS_PCH) $(XMACOS)
	$(Q) rm -f $(ANNOTATIONS_DIR)/macOS-*.raw
	$(Q_GEN) $(XTRO_SHARPIE_EXEC) --output-directory $(ANNOTATIONS_DIR) --lib $(XMACOS_LIBDIR) $(XMACOS_PCH) $(XMACOS)
	$(Q) touch $@
endif

ifdef INCLUDE_XAMARIN_LEGACY
.stamp-classify: $(XTRO_SANITY) $(CLASSIFY)
	$(Q_GEN) $(XTRO_SANITY_EXEC) $(abspath $(ANNOTATIONS_DIR)) "$(ALL_PLATFORMS)" $(INCLUDED_PLATFORMS)
	$(Q) touch $@

classify: .stamp-classify
endif

ifdef ENABLE_DOTNET
.stamp-dotnet-classify: $(XTRO_SANITY) $(DOTNET_CLASSIFY)
	$(Q_GEN) $(XTRO_SANITY_EXEC) $(abspath $(DOTNET_ANNOTATIONS_DIR)) "$(ALL_DOTNET_PLATFORMS)" $(XTRO_DOTNET_PLATFORMS)
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

U2TODO = u2todo/bin/Debug/u2todo.dll
$(U2TODO): $(wildcard u2todo/*.cs u2todo/*.csproj xtro-sharpie/Filter.cs)
	$(Q) unset MSBUILD_EXE_PATH && $(DOTNET) build u2todo/u2todo.csproj $(DOTNET_BUILD_VERBOSITY) /bl:$@.binlog
	$(Q) touch $@

unclassified2todo-classic: $(U2TODO)
	cd $(ANNOTATIONS_DIR) && $(DOTNET) exec $(abspath $(U2TODO))
	@for filename in $(git status -s | cut -c4- | grep .todo); do \
		sort -o "$filename" "$filename"; \
	done

unclassified2todo-dotnet: $(U2TODO)
	cd $(DOTNET_ANNOTATIONS_DIR) && $(DOTNET) exec $(abspath $(U2TODO))

unclassified2todo: unclassified2todo-classic unclassified2todo-dotnet
