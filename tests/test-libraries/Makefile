TOP=../..
include $(TOP)/Make.config

SUBDIRS += custom-type-assembly frameworks nugets libraries

# without this many compiler warnings about unused functions and variables
# in system headers show up.
export CCACHE_CPP2=1

ifeq ($(V),)
ZIP=zip --symlinks --quiet
else
ZIP=zip --symlinks
endif

GENERATED_FILES = \
	libtest.structs.h \
	libtest.decompile.m \
	libtest.properties.h \
	../bindings-test/ApiDefinition.generated.cs \
	../bindings-test/StructsAndEnums.generated.cs \
	RegistrarTest.generated.cs \
	TrampolineTest.generated.cs \

GENERATED_FILES_PATTERN = \
	libtest.structs%h \
	libtest.decompile%m \
	libtest.properties%h \
	../bindings-test/ApiDefinition.generated%cs \
	../bindings-test/StructsAndEnums.generated%cs \
	RegistrarTest.generated%cs \
	TrampolineTest.generated%cs \

testgenerator.exe: testgenerator.cs Makefile
	$(Q) $(SYSTEM_CSC) -out:$@ $<

$(GENERATED_FILES_PATTERN): testgenerator.exe
	$(Q) mono --debug $<

libtest-object.m libtest-ar.m:
	$(Q) ln -fhs libtest.m $@

MACOS_INFO_PLIST_INFIX=/Versions/A/Resources
MACOS_BINARY_INFIX=/Versions/A
MACCATALYST_INFO_PLIST_INFIX=/Versions/A/Resources
MACCATALYST_BINARY_INFIX=/Versions/A

define SymlinksTemplate

$(1)_XTEST_TARGETS += \
	.libs/$(1)/XTest.framework/XTest \
	.libs/$(1)/XTest.framework/Resources \
	.libs/$(1)/XTest.framework/Versions/Current \
	.libs/$(1)/XTest.framework/Versions/A/Resources/Info.plist \

.libs/$(1)/XTest.framework$($(2)_BINARY_INFIX) .libs/$(1)/XTest.framework$($(2)_INFO_PLIST_INFIX):
	$$(Q) mkdir -p $$@

.libs/$(1)/XTest.framework/XTest: | .libs/$(1)/XTest.framework
	$$(Q) ln -fs Versions/A/XTest $$@

.libs/$(1)/XTest.framework/Resources: | .libs/$(1)/XTest.framework
	$$(Q) ln -fs Versions/Current/Resources $$@

.libs/$(1)/XTest.framework/Versions/Current: | .libs/$(1)/XTest.framework/Versions
	$$(Q) ln -fs A $$@

x::
	@echo $(1)_XTEST_TARGETS=$$($(1)_XTEST_TARGETS)

endef
ifdef INCLUDE_MAC
$(eval $(call SymlinksTemplate,macos,MACOS))
endif
ifdef INCLUDE_MACCATALYST
$(eval $(call SymlinksTemplate,maccatalyst,MACCATALYST))
endif

define Template

$(1)_XTEST_TARGETS += \
	.libs/$(1)/XTest.framework$($(2)_BINARY_INFIX)/XTest \
	.libs/$(1)/XTest.framework$($(2)_INFO_PLIST_INFIX)/Info.plist \

$(2)_TARGETS = \
	$$($(1)_XTEST_TARGETS) \
	.libs/$(1)/XStaticObjectTest.framework/XStaticObjectTest \
	.libs/$(1)/XStaticArTest.framework/XStaticArTest \
	.libs/$(1)/libtest.dylib \
	.libs/$(1)/libtest2.a \
	.libs/$(1)/libtest.a \
	.libs/$(1)/libtest-object.a \
	.libs/$(1)/libtest-ar.a \
	$$(foreach arch,$(3),.libs/$(1)/libtest.$$(arch).a) \
	$$(foreach arch,$(3),.libs/$(1)/libtest-object.$$(arch).a) \
	$$(foreach arch,$(3),.libs/$(1)/libtest-ar.$$(arch).a) \
	.libs/$(1)/XTest.framework \
	.libs/$(1)/XTest.framework.stamp \

all-local:: $$($(2)_TARGETS) $(GENERATED_FILES)

.libs/$(1)/XTest.framework.stamp: $$($(1)_XTEST_TARGETS)
	$$(Q) touch $$@

clean-$(1):
	rm -Rf .libs/$(1)

CLEAN_TARGETS += clean-$(1)
EXTRA_DEPENDENCIES = libtest.h $(GENERATED_FILES) rename.h

.libs/$(1)/libtest-object.%.o: export EXTRA_DEFINES=-DPREFIX=1
.libs/$(1)/libtest-ar.%.o: export EXTRA_DEFINES=-DPREFIX=2

.libs/$(1)/libtest%.a: .libs/$(1)/libtest%.o libtest-object.m libtest-ar.m
	$(Q) rm -f $$@
	$$(call Q_2,AR     [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/ar cru $$@ $$<

.libs/$(1)/libtest.a: $$(foreach arch,$(3),.libs/$(1)/libtest.$$(arch).a)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

.libs/$(1)/libtest2.a: $$(foreach arch,$(3),.libs/$(1)/libtest2.$$(arch).a)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

.libs/$(1)/libtest-object.a: $$(foreach arch,$(3),.libs/$(1)/libtest-object.$$(arch).a)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

.libs/$(1)/libtest-ar.a: $$(foreach arch,$(3),.libs/$(1)/libtest-ar.$$(arch).a)
	$(Q) rm -f $$@
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

COMMON_DYLIB_ARGS=-g -dynamiclib -gdwarf-2 -fms-extensions libframework.m -o $$@ -Wall -framework Foundation -lz
.libs/$(1)/libtest.x86.dylib: ARCH=i386
.libs/$(1)/libtest.%.dylib: libframework.m | .libs/$(1)
	$$(call Q_2,CC,    [$(1)]) $$(XCODE_CC) $$(COMMON_DYLIB_ARGS) -arch $$(if $$(ARCH),$$(ARCH),$$*) $(5) $$($(2)_$$(shell echo $$* | tr a-z A-Z)_OBJC_CFLAGS)
	$$(Q) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/install_name_tool -id @rpath/libtest.dylib $$@

## The following arm64_32 rule is required for this to work with newer make versions, otherwise a rule in mk/rules.mk is chosen instead.
## The problem is that 'arm64_32' is longer than 'libtest', the former is a stem in the rule just above, while the latter is a rule in mk/rules.mk,
## and in newer make versions make will choose the rule that matches the shorter stem.
.libs/$(1)/libtest.arm64_32.dylib: libframework.m | .libs/$(1)
	$$(call Q_2,CC,    [$(1)]) $$(XCODE_CC) $$(COMMON_DYLIB_ARGS) -arch arm64_32 $(5) $$($(2)_$$(shell echo $$* | tr a-z A-Z)_OBJC_CFLAGS)
	$$(Q) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/install_name_tool -id @rpath/libtest.dylib $$@

.libs/$(1)/libtest.dylib: $$(foreach arch,$(3),.libs/$(1)/libtest.$$(arch).dylib)
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

# XTest is a framework where the binary code is a (fat) dynamic library
.libs/$(1)/XTest.framework$($(2)_BINARY_INFIX)/XTest: .libs/$(1)/libtest.dylib | .libs/$(1)/XTest.framework$($(2)_BINARY_INFIX)
	$$(Q) $(CP) $$^ $$@
	$$(Q) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/install_name_tool -id @rpath/XTest.framework/XTest $$@

.libs/$(1)/XTest.framework$($(2)_INFO_PLIST_INFIX)/Info.plist: XTest-Info-$(1).plist | .libs/$(1)/XTest.framework$($(2)_INFO_PLIST_INFIX)
	$$(Q) $(CP) $$^ $$@

# XStaticObjectTest is a framework where the binary code is a (fat) object file
.libs/$(1)/XStaticObjectTest.framework/XStaticObjectTest: $$(foreach arch,$(3),.libs/$(1)/libtest-object.$$(arch).o) | .libs/$(1)/XStaticObjectTest.framework
	$(Q) rm -f $$@
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

# XStaticArTest is a framework where the binary code is a (fat) ar archive (of object files)
.libs/$(1)/XStaticArTest.framework/XStaticArTest: $$(foreach arch,$(3),.libs/$(1)/libtest-ar.$$(arch).a) | .libs/$(1)/XStaticArTest.framework
	$(Q) rm -f $$@
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

$(1)_DIRECTORIES = \
	.libs/$(1)/XTest.framework \
	.libs/$(1)/XTest.framework/Versions \
	.libs/$(1)/XStaticObjectTest.framework \
	.libs/$(1)/XStaticArTest.framework \

$$($(1)_DIRECTORIES):
	$$(Q) mkdir -p $$@

endef

ifdef IOS_SUPPORTS_32BIT_ARCHITECTURES
IOS_SIMULATOR_ARCHITECTURES=x86
IOS_DEVICE_ARCHITECTURES=armv7 armv7s
endif
IOS_SIMULATOR_ARCHITECTURES+=x86_64 arm64
IOS_DEVICE_ARCHITECTURES+=arm64

# 1: platform infix
# 2: platform variable
# 3: architectures
# 4: platform name
# 5: min version
# 6: os
$(eval $(call Template,iphonesimulator,IOSSIMULATOR,$(IOS_SIMULATOR_ARCHITECTURES),iPhoneSimulator,-mios-simulator-version-min=8.0 -isysroot $(SIMULATOR_SDK)))
$(eval $(call Template,iphoneos,IPHONEOS,$(IOS_DEVICE_ARCHITECTURES),iPhoneOS,-miphoneos-version-min=8.0 -isysroot $(DEVICE_SDK)))
ifdef INCLUDE_TVOS
$(eval $(call Template,tvsimulator,TVSIMULATOR,x86_64 arm64,AppleTVSimulator,-mtvos-simulator-version-min=9.0 -isysroot $(SIMULATORTV_SDK)))
$(eval $(call Template,tvos,TVOS,arm64,AppleTVOS,-mtvos-version-min=9.0 -isysroot $(DEVICETV_SDK)))
endif
ifdef INCLUDE_WATCH
ifdef WATCHOS_SUPPORTS_32BIT_SIMULATOR_ARCHITECTURES
$(eval $(call Template,watchsimulator,WATCHSIMULATOR,x86 x86_64,WatchSimulator,-mwatchos-simulator-version-min=$(MIN_WATCH_OS_VERSION) -isysroot $(SIMULATORWATCH_SDK)))
else
$(eval $(call Template,watchsimulator,WATCHSIMULATOR,x86_64,WatchSimulator,-mwatchos-simulator-version-min=$(MIN_WATCH_OS_VERSION) -isysroot $(SIMULATORWATCH_SDK)))
endif
ifdef WATCHOS_SUPPORTS_32BIT_DEVICE_ARCHITECTURES
$(eval $(call Template,watchos,WATCHOS,armv7k arm64_32,WatchOS,-mwatchos-version-min=$(MIN_WATCHOS_SDK_VERSION) -fembed-bitcode -isysroot $(DEVICEWATCH_SDK)))
else
$(eval $(call Template,watchos,WATCHOS,arm64_32,WatchOS,-mwatchos-version-min=$(MIN_WATCHOS_SDK_VERSION) -fembed-bitcode -isysroot $(DEVICEWATCH_SDK)))
endif
endif
ifdef INCLUDE_MAC
$(eval $(call Template,macos,MACOS,x86_64 arm64,MacOSX,-mmacosx-version-min=$(MIN_OSX_VERSION_FOR_MAC) -isysroot $(XCODE_DEVELOPER_ROOT)/Platforms/MacOSX.platform/Developer/SDKs/MacOSX$(MACOS_SDK_VERSION).sdk))
endif
ifdef INCLUDE_MACCATALYST
$(eval $(call Template,maccatalyst,MACCATALYST,x86_64 arm64,MacCatalyst,$(MACCATALYST_COMMON_CFLAGS)))
endif

.libs/ios-fat .libs/tvos-fat .libs/watchos-fat .libs/maccatalyst-fat .libs/macos-fat:
	$(Q) mkdir -p $@

define LibTemplate
.libs/$(4)/libtest.$(1).a: $(2) $(3) | .libs/$(4)
	@rm -f $$@
	$$(Q_LIPO) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

$(5)_TARGETS += .libs/$(4)/libtest.$(1).a
all-local:: $$($(5)_TARGETS)
endef

ifdef IOS_SUPPORTS_32BIT_ARCHITECTURES
$(eval $(call LibTemplate,armv7+7s,.libs/iphoneos/libtest.armv7.o,.libs/iphoneos/libtest.armv7s.o,ios-fat,IOS))
$(eval $(call LibTemplate,armv7+x86,.libs/iphoneos/libtest.armv7.o,.libs/iphonesimulator/libtest.x86.o,ios-fat,IOS))
$(eval $(call LibTemplate,armv7s+x86,.libs/iphoneos/libtest.armv7s.o,.libs/iphonesimulator/libtest.x86.o,ios-fat,IOS))
endif

ifdef INCLUDE_TVOS
$(eval $(call LibTemplate,arm64+x86_64,.libs/tvos/libtest.arm64.o,.libs/tvsimulator/libtest.x86_64.o,tvos-fat,TVOS))
endif

ifdef INCLUDE_WATCH
ifdef WATCHOS_SUPPORTS_32BIT_DEVICE_ARCHITECTURES
ifdef WATCHOS_SUPPORTS_32BIT_SIMULATOR_ARCHITECTURES
$(eval $(call LibTemplate,armv7k+x86,.libs/watchos/libtest.armv7k.o,.libs/watchsimulator/libtest.x86.o,watchos-fat,WATCHOS))
endif
endif
endif

ifdef INCLUDE_MACCATALYST
$(eval $(call LibTemplate,x86_64+arm64,.libs/maccatalyst/libtest.x86_64.o,.libs/maccatalyst/libtest.arm64.o,maccatalyst-fat,MACCATALYST))
endif

ifdef INCLUDE_MAC
$(eval $(call LibTemplate,x86_64+arm64,.libs/macos/libtest.x86_64.o,.libs/macos/libtest.arm64.o,macos-fat,MACOS))
endif

define FatFrameworkTemplate
.libs/$(1)/XStaticObjectTest.framework/XStaticObjectTest: .libs/$(2)/XStaticObjectTest.framework/XStaticObjectTest .libs/$(3)/XStaticObjectTest.framework/XStaticObjectTest | .libs/$(1)
	$(Q) mkdir -p $$(dir $$@)
	$(Q) ./lipo-remove-sim-arm64.sh  $$@ $$^

.libs/$(1)/XStaticArTest.framework/XStaticArTest: .libs/$(2)/XStaticArTest.framework/XStaticArTest .libs/$(3)/XStaticArTest.framework/XStaticArTest | .libs/$(1)
	$(Q) mkdir -p $$(dir $$@)
	$(Q) ./lipo-remove-sim-arm64.sh $$@ $$^

.libs/$(1)/XTest.framework$($(4)_BINARY_INFIX)/XTest: .libs/$(2)/XTest.framework$($(4)_BINARY_INFIX)/XTest .libs/$(3)/XTest.framework$($(4)_BINARY_INFIX)/XTest | .libs/$(1)
	$(Q) mkdir -p $$(dir $$@)
	$(Q) ./lipo-remove-sim-arm64.sh  $$@ $$^

.libs/$(1)/XTest.framework$($(4)_INFO_PLIST_INFIX)/Info.plist: .libs/$(2)/XTest.framework$($(4)_INFO_PLIST_INFIX)/Info.plist .libs/$(3)/XTest.framework$($(4)_INFO_PLIST_INFIX)/Info.plist
	$(Q) mkdir -p $$(dir $$@)
ifneq ($(2),$(3))
	@# Check if the Info.plists are identical
	$(Q) diff $$^
endif
	@# Copy one of them
	$(Q) $(CP) $$< $$@

.libs/$(1)/libtest.a: .libs/$(2)/libtest.a .libs/$(3)/libtest.a | .libs/$(1)
	$(Q) ./lipo-remove-sim-arm64.sh $$@ $$^

.libs/$(1)/libtest2.a: .libs/$(2)/libtest2.a .libs/$(3)/libtest2.a | .libs/$(1)
	$(Q) ./lipo-remove-sim-arm64.sh $$@ $$^

.libs/$(1)/libtest.dylib: .libs/$(2)/libtest.dylib .libs/$(3)/libtest.dylib | .libs/$(1)
	$(Q) ./lipo-remove-sim-arm64.sh $$@ $$^

$(4)_TARGETS += \
	.libs/$(1)/XTest.framework$($(4)_BINARY_INFIX)/XTest \
	.libs/$(1)/XTest.framework$($(4)_INFO_PLIST_INFIX)/Info.plist \
	.libs/$(1)/XStaticObjectTest.framework/XStaticObjectTest \
	.libs/$(1)/XStaticArTest.framework/XStaticArTest \
	.libs/$(1)/libtest.dylib \
	.libs/$(1)/libtest.a \
	.libs/$(1)/libtest2.a \

all-local:: $$($(4)_TARGETS)
endef

$(eval $(call FatFrameworkTemplate,ios-fat,iphoneos,iphonesimulator,IPHONESIMULATOR))
ifdef INCLUDE_TVOS
$(eval $(call FatFrameworkTemplate,tvos-fat,tvos,tvsimulator,TVSIMULATOR))
endif
ifdef INCLUDE_WATCH
$(eval $(call FatFrameworkTemplate,watchos-fat,watchos,watchsimulator,WATCHSIMULATOR))
endif
ifdef INCLUDE_MACCATALYST
$(eval $(call FatFrameworkTemplate,maccatalyst-fat,maccatalyst,maccatalyst,MACCATALYST))
endif
ifdef INCLUDE_MAC
$(eval $(call FatFrameworkTemplate,macos-fat,macos,macos,MACOS))
endif

define FatFrameworkSymlinksTemplate
.libs/$(1)/XTest.framework/XTest: | .libs/$(1)
	$(Q) mkdir -p $$(dir $$@)
	$(Q) ln -fs Versions/A/XTest $$@

.libs/$(1)/XTest.framework/Resources: | .libs/$(1)
	$(Q) mkdir -p $$(dir $$@)
	$(Q) ln -fs Versions/A/Resources $$@

.libs/$(1)/XTest.framework/Versions/Current: | .libs/$(1)
	$(Q) mkdir -p $$(dir $$@)
	$(Q) ln -fs A $$@

$(4)_TARGETS += \
	.libs/$(1)/XTest.framework/XTest \
	.libs/$(1)/XTest.framework/Resources \
	.libs/$(1)/XTest.framework/Versions/Current \

all-local:: $$($(4)_TARGETS)
endef

ifdef INCLUDE_MACCATALYST
$(eval $(call FatFrameworkSymlinksTemplate,maccatalyst-fat,maccatalyst,maccatalyst,MACCATALYST))
endif
ifdef INCLUDE_MAC
$(eval $(call FatFrameworkSymlinksTemplate,macos-fat,macos,macos,MACOS))
endif

ifdef INCLUDE_IOS
XCPLATFORMS+= iphoneos iphonesimulator
endif
ifdef INCLUDE_TVOS
XCPLATFORMS+= tvos tvsimulator
endif
ifdef INCLUDE_WATCH
XCPLATFORMS+= watchos watchsimulator
endif
ifdef INCLUDE_MAC
XCPLATFORMS+= macos
endif
ifdef INCLUDE_MACCATALYST
XCPLATFORMS+= maccatalyst
endif

define ZippedFrameworkTemplate
.libs/$(1)/XTest.framework.zip: $$($(2)_TARGETS)
	$$(Q_ZIP) cd .libs/$(1) && $(ZIP) -r XTest.framework.zip XTest.framework

ZIPPED_TARGETS += .libs/$(1)/XTest.framework.zip
endef

ifdef INCLUDE_IOS
$(eval $(call ZippedFrameworkTemplate,ios-fat,IPHONESIMULATOR))
endif
ifdef INCLUDE_TVOS
$(eval $(call ZippedFrameworkTemplate,tvos-fat,TVSIMULATOR))
endif
ifdef INCLUDE_WATCH
$(eval $(call ZippedFrameworkTemplate,watchos-fat,WATCHSIMULATOR))
endif
ifdef INCLUDE_MACCATALYST
$(eval $(call ZippedFrameworkTemplate,maccatalyst-fat,MACCATALYST))
endif
ifdef INCLUDE_MAC
$(eval $(call ZippedFrameworkTemplate,macos-fat,MACOS))
endif

all-local:: $(ZIPPED_TARGETS)
zip: $(ZIPPED_TARGETS)

XTEST_XCFRAMEWORKS += $(foreach platform,$(XCPLATFORMS),.libs/$(platform)/XTest.framework)
XTEST_XCTARGETS += \
	$(foreach platform,$(XCPLATFORMS),.libs/$(platform)/XTest.framework.stamp) \

.libs/XTest.xcframework: $(XTEST_XCTARGETS) Makefile
	$(Q) rm -rf $@
	$(Q_GEN) $(XCODE_DEVELOPER_ROOT)/usr/bin/xcodebuild -quiet -create-xcframework $(foreach fw,$(XTEST_XCFRAMEWORKS),-framework $(fw)) -output $@

all-local:: .libs/XTest.xcframework

LIBTEST_XCFRAMEWORKS += $(foreach platform,$(XCPLATFORMS),.libs/$(platform)/libtest.a)

.libs/libtest.xcframework: $(LIBTEST_XCFRAMEWORKS) Makefile
	$(Q) rm -rf $@
	$(Q_GEN) $(XCODE_DEVELOPER_ROOT)/usr/bin/xcodebuild -quiet -create-xcframework $(foreach lib,$(LIBTEST_XCFRAMEWORKS),-library $(lib)) -output $@

all-local:: .libs/libtest.xcframework

LIBTEST2_XCFRAMEWORKS += $(foreach platform,$(XCPLATFORMS),.libs/$(platform)/libtest2.a)

.libs/libtest2.xcframework: $(LIBTEST2_XCFRAMEWORKS) Makefile
	$(Q) rm -rf $@
	$(Q_GEN) $(XCODE_DEVELOPER_ROOT)/usr/bin/xcodebuild -quiet -create-xcframework $(foreach lib,$(LIBTEST2_XCFRAMEWORKS),-library $(lib)) -output $@

all-local:: .libs/libtest2.xcframework

.libs/XTest.xcframework.zip: .libs/XTest.xcframework
	$(Q_ZIP) cd .libs && $(ZIP) -r "$(notdir $@)" "$(notdir $<)"

all-local:: .libs/XTest.xcframework.zip

# Xamarin.Mac

MAC_CLANG = DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT) $(MAC_CC)
MAC_OBJC_CFLAGS=-ObjC++ -std=c++0x -fno-exceptions
MAC_CFLAGS = -mmacosx-version-min=$(MIN_MACOS_SDK_VERSION) -Wall -DMONOMAC -g
MAC_LDFLAGS = -mmacosx-version-min=$(MIN_MACOS_SDK_VERSION) -framework AppKit

.libs/macos/libtest.%.o: libtest.m $(EXTRA_DEPENDENCIES) | .libs/macos
	$(call Q_2,OBJC,  [mac]) $(MAC_CLANG) -arch $* -c $(MAC_OBJC_CFLAGS) $(MAC_CFLAGS) -o $@ $<

.libs/macos/libtest-object.%.o: libtest.m $(EXTRA_DEPENDENCIES) | .libs/macos
	$(call Q_2,OBJC,  [mac]) $(MAC_CLANG) -arch $* -c $(MAC_OBJC_CFLAGS) $(MAC_CFLAGS) -o $@ $< -DPREFIX=1

.libs/macos/libtest-ar.%.o: libtest.m $(EXTRA_DEPENDENCIES) | .libs/macos
	$(call Q_2,OBJC,  [mac]) $(MAC_CLANG) -arch $* -c $(MAC_OBJC_CFLAGS) $(MAC_CFLAGS) -o $@ $< -DPREFIX=2

.libs/macos/libtest2.%.o: libtest2.m $(EXTRA_DEPENDENCIES) | .libs/macos
	$(call Q_2,OBJC,  [mac]) $(MAC_CLANG) -arch $* -c $(MAC_OBJC_CFLAGS) $(MAC_CFLAGS) -o $@ $<

.libs/macos:
	$(Q) mkdir -p $@

clean-local:: $(CLEAN_TARGETS)

include $(TOP)/mk/rules.mk

.SECONDARY:
