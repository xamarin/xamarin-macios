TOP=../..

include $(TOP)/Make.config

# a 64bits mono is required because of the clang requirement
MONO ?= mono64 --debug
SHARPIE ?= sharpie

XCODE=$(abspath $(XCODE_DEVELOPER_ROOT)/../..)

XTRO_DOTNET_PLATFORMS=$(DOTNET_PLATFORMS)

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

$(XTRO_SHARPIE): $(wildcard xtro-sharpie/*.cs) $(wildcard xtro-sharpie/*.csproj) pch-info.proj $(XTRO_REPORT)
	$(Q_GEN) unset MSBUILD_EXE_PATH && $(DOTNET) build xtro-sharpie/xtro-sharpie.csproj /bl:xtro-sharpie.binlog $(DOTNET_BUILD_VERBOSITY)

$(XTRO_REPORT): $(wildcard xtro-report/*.cs) $(wildcard xtro-report/*.csproj) xtro-sharpie/Filter.cs Makefile
	$(Q_GEN) unset MSBUILD_EXE_PATH && $(DOTNET) build xtro-report/xtro-report.csproj /bl:xtro-report.binlog $(DOTNET_BUILD_VERBOSITY)

$(XTRO_SANITY): $(wildcard xtro-sanity/*.cs) $(wildcard xtro-sanity/*.csproj) $(wildcard $(TOP)/tools/common/*.cs) Makefile
	$(Q_GEN) unset MSBUILD_EXE_PATH && $(DOTNET) build xtro-sanity/xtro-sanity.csproj /bl:xtro-sanity.binlog $(DOTNET_BUILD_VERBOSITY)

XIOS_ARCH = arm64
XIOS_PCH = iphoneos$(IOS_SDK_VERSION)-$(XIOS_ARCH).pch
XIOS_RID = ios-arm64

$(XIOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s iphoneos$(IOS_SDK_VERSION) -a $(XIOS_ARCH) \
	-modules false \
	-exclude FactoryOTAWifiUtils \
	-exclude FactoryOTALogger \
	-exclude FactoryOTANetworkUtils \
	-exclude AssetsLibrary \
	-exclude _CoreNFC_UIKit \
	-i ThreadNetwork/THClient.h \
	$(CORETELEPHONY_HEADERS) \

XTVOS_ARCH = arm64
XTVOS_PCH = appletvos$(TVOS_SDK_VERSION)-$(XTVOS_ARCH).pch
XTVOS_RID = tvos-arm64

$(XTVOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s appletvos$(TVOS_SDK_VERSION) -a $(XTVOS_ARCH) \
		-modules false \

XMACOS_ARCH = x86_64
XMACOS_PCH = macosx$(MACOS_SDK_VERSION)-$(XMACOS_ARCH).pch
XMACOS_RID = osx-x64

$(XMACOS_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s macosx$(MACOS_SDK_VERSION) -a $(XMACOS_ARCH) \
		-modules false \
		-exclude AccessorySetupKit \
		-exclude JavaNativeFoundation \
		-exclude ParavirtualizedGraphics \
		$(CORETELEPHONY_HEADERS) \

XMACCATALYST_ARCH = x86_64
XMACCATALYST_PCH = ios$(MACCATALYST_SDK_VERSION)-macabi-$(XMACCATALYST_ARCH).pch
XMACCATALYST_RID = maccatalyst-x64

$(XMACCATALYST_PCH): .stamp-check-sharpie
	$(SHARPIE) sdk-db --xcode $(XCODE) -s ios$(MACCATALYST_SDK_VERSION)-macabi -a $(XMACCATALYST_ARCH) \
	-modules false \
	-exclude _CoreNFC_UIKit \
	-exclude AccessorySetupKit \
	-exclude AGL \
	-exclude AudioVideoBridging \
	-exclude AssetsLibrary \
	-exclude CalendarStore \
	-exclude Carbon \
	-exclude ClockKit \
	-exclude DiscRecordingUI \
	-exclude FSKit \
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

gen-all: gen-ios gen-tvos gen-macos gen-maccatalyst

report-dotnet/index.html: $(XTRO_REPORT) .stamp-dotnet-classify
	$(Q) rm -rf report-dotnet
	$(Q_GEN) $(XTRO_REPORT_EXEC) $(DOTNET_ANNOTATIONS_DIR) report-dotnet

report-dotnet/report.zip: report-dotnet/index.html
	$(Q) rm -f "$@"
	$(Q) cd report-dotnet && zip -r9 report.zip . -x report.zip

dotnet-report: report-dotnet/index.html

report-short:
	JENKINS_SERVER_COOKIE=1 make report

define DotNetClassify
.stamp-dotnet-classify-$(1): $(XTRO_SHARPIE) $$(X$(2)_PCH) $$(X$(2)_DOTNET)
	$$(Q) rm -f $$(DOTNET_ANNOTATIONS_DIR)/$(1)-*.raw
	$$(Q_GEN) $(XTRO_SHARPIE_EXEC) --output-directory $$(DOTNET_ANNOTATIONS_DIR) --lib $(DOTNET_BCL_DIR) $$(X$(2)_PCH) $$(X$(2)_DOTNET)
	$$(Q) touch $$@

dotnet-classify-$(1): .stamp-dotnet-classify-$(1)
DOTNET_CLASSIFY += .stamp-dotnet-classify-$(1)
endef
$(foreach platform,$(XTRO_DOTNET_PLATFORMS),$(eval $(call DotNetClassify,$(platform),$(shell echo $(platform) | tr a-z A-Z))))

.stamp-dotnet-classify: $(XTRO_SANITY) $(DOTNET_CLASSIFY)
	$(Q_GEN) $(XTRO_SANITY_EXEC) $(abspath $(DOTNET_ANNOTATIONS_DIR)) "$(ALL_DOTNET_PLATFORMS)" $(XTRO_DOTNET_PLATFORMS)
	$(Q) touch $@

dotnet-classify: .stamp-dotnet-classify

insane:
	XTRO_SANITY_SKIP=1 make all

remove-empty:
	find . -size 0 | xargs git rm

all: dotnet-report

.stamp-check-sharpie:
	@$(TOP)/system-dependencies.sh --ignore-all --enforce-sharpie
	@touch $@
	
remove-empty-files:
	find . -empty -exec git rm -f {} \;

U2TODO = u2todo/bin/Debug/u2todo.dll
$(U2TODO): $(wildcard u2todo/*.cs u2todo/*.csproj xtro-sharpie/Filter.cs)
	$(Q) unset MSBUILD_EXE_PATH && $(DOTNET) build u2todo/u2todo.csproj $(DOTNET_BUILD_VERBOSITY) /bl:$@.binlog
	$(Q) touch $@

unclassified2todo: $(U2TODO)
	cd $(DOTNET_ANNOTATIONS_DIR) && $(DOTNET) exec $(abspath $(U2TODO))

run-tests run-unit-tests:
	$(Q) $(MAKE) -C UnitTests $@
