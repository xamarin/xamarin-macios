TOP=../..
include $(TOP)/Make.config

# without this many compiler warnings about unused functions and variables
# in system headers show up.
export CCACHE_CPP2=1

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
	$(Q) mcs -out:$@ $<

$(GENERATED_FILES_PATTERN): testgenerator.exe
	$(Q) mono --debug $<

libtest-object.m libtest-ar.m:
	$(Q) ln -fhs libtest.m $@

define Template

$(2)_TARGETS = \
	.libs/$(1)/XTest.framework/XTest \
	.libs/$(1)/XTest.framework/Info.plist \
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

all-local:: $$($(2)_TARGETS) $(GENERATED_FILES)

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
$$(foreach arch,$(3),.libs/$(1)/libtest.$$(arch).dylib): libframework.m | .libs/$(1)
	$$(call Q_2,CC,    [$(1)]) $$(XCODE_CC) $$(COMMON_DYLIB_ARGS) -arch $$(if $$(ARCH),i386,$$(subst libtest.,,$$(basename $$(notdir $$@)))) $(5) -isysroot $(XCODE_DEVELOPER_ROOT)/Platforms/$(4).platform/Developer/SDKs/$(4)$$($(6)_SDK_VERSION).sdk
	$$(Q) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/install_name_tool -id @rpath/XTest.framework/XTest $$@

.libs/$(1)/libtest.dylib: $$(foreach arch,$(3),.libs/$(1)/libtest.$$(arch).dylib)
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

# XTest is a framework where the binary code is a (fat) dynamic library
.libs/$(1)/XTest.framework/XTest: .libs/$(1)/libtest.dylib | .libs/$(1)/XTest.framework
	$$(Q) $(CP) $$^ $$@

.libs/$(1)/XTest.framework/Info.plist: XTest-Info.plist | .libs/$(1)/XTest.framework
	$$(Q) $(CP) $$^ $$@

# XStaticObjectTest is a framework where the binary code is a (fat) object file
.libs/$(1)/XStaticObjectTest.framework/XStaticObjectTest: $$(foreach arch,$(3),.libs/$(1)/libtest-object.$$(arch).o) | .libs/$(1)/XStaticObjectTest.framework
	$(Q) rm -f $$@
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

# XStaticArTest is a framework where the binary code is a (fat) ar archive (of object files)
.libs/$(1)/XStaticArTest.framework/XStaticArTest: $$(foreach arch,$(3),.libs/$(1)/libtest-ar.$$(arch).a) | .libs/$(1)/XStaticArTest.framework
	$(Q) rm -f $$@
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

.libs/$(1)/XTest.framework .libs/$(1)/XStaticObjectTest.framework .libs/$(1)/XStaticArTest.framework:
	$$(Q) mkdir -p $$@

endef

# 1: platform infix
# 2: platform variable
# 3: architectures
# 4: platform name
# 5: min version
# 6: os
$(eval $(call Template,iphonesimulator,IOSSIMULATOR,x86 x86_64,iPhoneSimulator,-mios-simulator-version-min=8.0,IOS))
$(eval $(call Template,iphoneos,IPHONEOS,armv7 armv7s arm64,iPhoneOS,-miphoneos-version-min=8.0,IOS))
ifdef INCLUDE_TVOS
$(eval $(call Template,tvsimulator,TVSIMULATOR,x86_64,AppleTVSimulator,-mtvos-simulator-version-min=9.0,TVOS))
$(eval $(call Template,tvos,TVOS,arm64,AppleTVOS,-mtvos-version-min=9.0 -fembed-bitcode,TVOS))
endif
ifdef INCLUDE_WATCH
$(eval $(call Template,watchsimulator,WATCHSIMULATOR,x86,WatchSimulator,-mwatchos-simulator-version-min=2.0,WATCHOS))
$(eval $(call Template,watchos,WATCHOS,armv7k arm64_32,WatchOS,-mwatchos-version-min=2.0 -fembed-bitcode,WATCHOS))
endif
ifdef INCLUDE_MAC
$(eval $(call Template,macos,MACOS,x86_64,MacOSX,-mmacosx-version-min=$(MIN_OSX_VERSION_FOR_MAC),MACOS,x86_64))
endif

.libs/ios-fat .libs/tvos-fat .libs/watchos-fat:
	$(Q) mkdir -p $@

define LibTemplate
.libs/$(4)/libtest.$(1).a: $(2) $(3) | .libs/$(4)
	@rm -f $$@
	$$(Q_LIPO) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

$(5)_TARGETS += .libs/$(4)/libtest.$(1).a
all-local:: $$($(5)_TARGETS)
endef

$(eval $(call LibTemplate,armv7+7s,.libs/iphoneos/libtest.armv7.o,.libs/iphoneos/libtest.armv7s.o,ios-fat,IOS))
$(eval $(call LibTemplate,armv7+x86,.libs/iphoneos/libtest.armv7.o,.libs/iphonesimulator/libtest.x86.o,ios-fat,IOS))
$(eval $(call LibTemplate,armv7s+x86,.libs/iphoneos/libtest.armv7s.o,.libs/iphonesimulator/libtest.x86.o,ios-fat,IOS))

$(eval $(call LibTemplate,arm64+x86_64,.libs/tvos/libtest.arm64.o,.libs/tvsimulator/libtest.x86_64.o,tvos-fat,TVOS))

$(eval $(call LibTemplate,armv7k+x86,.libs/watchos/libtest.armv7k.o,.libs/watchsimulator/libtest.x86.o,watchos-fat,WATCHOS))

define FatFrameworkTemplate
.libs/$(1)/XStaticObjectTest.framework/XStaticObjectTest: .libs/$(2)/XStaticObjectTest.framework/XStaticObjectTest .libs/$(3)/XStaticObjectTest.framework/XStaticObjectTest | .libs/$(1)
	$(Q) mkdir -p $$(dir $$@)
	$(Q) lipo -create -output $$@ $$^

.libs/$(1)/XStaticArTest.framework/XStaticArTest: .libs/$(2)/XStaticArTest.framework/XStaticArTest .libs/$(3)/XStaticArTest.framework/XStaticArTest | .libs/$(1)
	$(Q) mkdir -p $$(dir $$@)
	$(Q) lipo -create -output $$@ $$^

.libs/$(1)/XTest.framework/XTest: .libs/$(2)/XTest.framework/XTest .libs/$(3)/XTest.framework/XTest | .libs/$(1)
	$(Q) mkdir -p $$(dir $$@)
	$(Q) lipo -create -output $$@ $$^

.libs/$(1)/XTest.framework/Info.plist: .libs/$(2)/XTest.framework/Info.plist .libs/$(3)/XTest.framework/Info.plist
	$(Q) mkdir -p $$(dir $$@)
	@# Check if the Info.plists are identical
	$(Q) diff $$^
	@# Copy one of them
	$(Q) $(CP) $$< $$@

.libs/$(1)/libtest.a: .libs/$(2)/libtest.a .libs/$(3)/libtest.a | .libs/$(1)
	$(Q) lipo -create -output $$@ $$^

.libs/$(1)/libtest2.a: .libs/$(2)/libtest2.a .libs/$(3)/libtest2.a | .libs/$(1)
	$(Q) lipo -create -output $$@ $$^

$(3)_TARGETS += \
	.libs/$(1)/XTest.framework/XTest .libs/$(1)/XTest.framework/Info.plist \
	.libs/$(1)/XStaticObjectTest.framework/XStaticObjectTest \
	.libs/$(1)/XStaticArTest.framework/XStaticArTest \
	.libs/$(1)/libtest.a \
	.libs/$(1)/libtest2.a \

all-local:: $$($(3)_TARGETS)
endef

$(eval $(call FatFrameworkTemplate,ios-fat,iphoneos,iphonesimulator))
$(eval $(call FatFrameworkTemplate,tvos-fat,tvos,tvsimulator))
$(eval $(call FatFrameworkTemplate,watchos-fat,watchos,watchsimulator))

# Xamarin.Mac

MAC_CLANG = DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT) $(MAC_CC)
MAC_OBJC_CFLAGS=-ObjC++ -std=c++0x -fno-exceptions
MAC_CFLAGS = -mmacosx-version-min=$(MIN_OSX_SDK_VERSION) -Wall -DMONOMAC -g
MAC_LDFLAGS = -mmacosx-version-min=$(MIN_OSX_SDK_VERSION) -framework AppKit

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
