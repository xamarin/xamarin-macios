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
	$$(call Q_2,AR     [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/ar cru $$@ $$^

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
.libs/$(1)/libtest-sim.%.dylib: libframework.m | .libs/$(1)
	$$(call Q_2,CC,    [$(1)]) $$(SIMULATOR_CC) $$(COMMON_DYLIB_ARGS) -arch $$* $(8) -isysroot $(XCODE_DEVELOPER_ROOT)/Platforms/$(6).platform/Developer/SDKs/$(6)$$($(2)_SDK_VERSION).sdk
	$$(Q) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/install_name_tool -id @rpath/XTest.framework/XTest $$@

.libs/$(1)/libtest-dev.%.dylib: libframework.m | .libs/$(1)
	$$(call Q_2,CC,    [$(1)]) $(DEVICE_CC)     $$(COMMON_DYLIB_ARGS) -arch $$* $(9) -isysroot $(XCODE_DEVELOPER_ROOT)/Platforms/$(7).platform/Developer/SDKs/$(7)$$($(2)_SDK_VERSION).sdk
	$$(Q) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/install_name_tool -id @rpath/XTest.framework/XTest $$@

.libs/$(1)/libtest.dylib: $$(foreach arch,$(4),.libs/$(1)/libtest-sim.$$(arch).dylib) $$(foreach arch,$(5),.libs/$(1)/libtest-dev.$$(arch).dylib)
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

# XTest is a framework where the binary code is a (fat) dynamic library
.libs/$(1)/XTest.framework/XTest: .libs/$(1)/libtest.dylib | .libs/$(1)/XTest.framework
	$$(Q) cp $$^ $$@

.libs/$(1)/XTest.framework/Info.plist: XTest-Info.plist | .libs/$(1)/XTest.framework
	$$(Q) cp $$^ $$@

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

# 3: all architectures
# 4: sim architectures
# 5: device architectures
# 6: simulator platform name
# 7: device platform name
# 8: simulator min version
# 9: device min version
$(eval $(call Template,ios,IOS,armv7 armv7s arm64 x86 x86_64,i386 x86_64,armv7 armv7s arm64,iPhoneSimulator,iPhoneOS,-mios-simulator-version-min=8.0,-miphoneos-version-min=8.0))
ifdef INCLUDE_TVOS
$(eval $(call Template,tvos,TVOS,arm64 x86_64,x86_64,arm64,AppleTVSimulator,AppleTVOS,-mtvos-simulator-version-min=9.0,-mtvos-version-min=9.0 -fembed-bitcode))
endif
ifdef INCLUDE_WATCH
$(eval $(call Template,watchos,WATCHOS,armv7k x86,i386,armv7k,WatchSimulator,WatchOS,-mwatchos-simulator-version-min=2.0,-mwatchos-version-min=2.0 -fembed-bitcode))
endif
ifdef INCLUDE_MAC
$(eval $(call Template,macos,MACOS,x86_64,x86_64,,MacOSX,MacOSX,-mmacosx-version-min=$(MIN_OSX_VERSION_FOR_MAC),-mmacosx-version-min=$(MIN_OSX_VERSION_FOR_MAC)))
endif

define LibTemplate

.libs/$(4)/libtest.$(1).a: $(2) $(3)
	@rm -f $$@
	$$(Q_LIPO) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@

$(5)_TARGETS += .libs/$(4)/libtest.$(1).a

endef

$(eval $(call LibTemplate,armv7+7s,.libs/ios/libtest.armv7.o,.libs/ios/libtest.armv7s.o,ios,IOS))
$(eval $(call LibTemplate,armv7+x86,.libs/ios/libtest.armv7.o,.libs/ios/libtest.x86.o,ios,IOS))
$(eval $(call LibTemplate,armv7s+x86,.libs/ios/libtest.armv7s.o,.libs/ios/libtest.x86.o,ios,IOS))

$(eval $(call LibTemplate,arm64+x86_64,.libs/tvos/libtest.arm64.o,.libs/tvos/libtest.x86_64.o,tvos,TVOS))

$(eval $(call LibTemplate,armv7k+x86,.libs/watchos/libtest.armv7k.o,.libs/watchos/libtest.x86.o,watchos,WATCHOS))

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
