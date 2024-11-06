# helpful rules to compile things for the various architectures

ifdef ENABLE_CCACHE
# Note the space at the end the line
CCACHE=ccache 
endif

iossimulator_SDK=$(XCODE_DEVELOPER_ROOT)/Platforms/iPhoneSimulator.platform/Developer/SDKs/iPhoneSimulator$(IOS_SDK_VERSION).sdk
ios_SDK=$(XCODE_DEVELOPER_ROOT)/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS$(IOS_SDK_VERSION).sdk
tvossimulator_SDK=$(XCODE_DEVELOPER_ROOT)/Platforms/AppleTVSimulator.platform/Developer/SDKs/AppleTVSimulator$(TVOS_SDK_VERSION).sdk
tvos_SDK=$(XCODE_DEVELOPER_ROOT)/Platforms/AppleTVOS.platform/Developer/SDKs/AppleTVOS$(TVOS_SDK_VERSION).sdk
maccatalyst_SDK=$(XCODE_DEVELOPER_ROOT)/Platforms/MacOSX.platform/Developer/SDKs/MacOSX$(MACOS_SDK_VERSION).sdk
macos_SDK=$(XCODE_DEVELOPER_ROOT)/Platforms/MacOSX.platform/Developer/SDKs/MacOSX$(MACOS_SDK_VERSION).sdk

ios_DEFINES=-DMONOTOUCH
tvos_DEFINES=-DMONOTOUCH
maccatalyst_DEFINES=-DMONOTOUCH
macos_DEFINES=-DMONOMAC


# Clang will by default emit objc_msgSend stubs in Xcode 14, which ld from earlier Xcodes doesn't understand.
# We disable this by passing -fno-objc-msgsend-selector-stubs to clang.
# We can probably remove this flag once we require developers to use Xcode 14.
# Ref: https://github.com/xamarin/xamarin-macios/issues/16223
OBJC_CFLAGS=-ObjC++ -std=c++14 -fno-exceptions -fno-objc-msgsend-selector-stubs -fobjc-abi-version=2 -fobjc-legacy-dispatch
CFLAGS= -Wall -fms-extensions -Werror -Wconversion -Wdeprecated -Wuninitialized -fstack-protector-strong -Wformat -Wformat-security -Werror=format-security -g -I.
SWIFTFLAGS=-g -emit-library

SWIFT_TOOLCHAIN_iossimulator=iphonesimulator
SWIFT_TOOLCHAIN_ios=iphoneos
SWIFT_TOOLCHAIN_maccatalyst=macosx
SWIFT_TOOLCHAIN_tvossimulator=appletvsimulator
SWIFT_TOOLCHAIN_tvos=appletvos
SWIFT_TOOLCHAIN_macos=macosx

# iOS

# macOS

# tvOS

# Mac Catalyst

## Mac Catalyst is special :/

maccatalyst_CFLAGS += \
	-isystem $(macos_SDK)/System/iOSSupport/usr/include \
	-iframework $(macos_SDK)/System/iOSSupport/System/Library/Frameworks \

maccatalyst_SWIFTFLAGS += -Fsystem $(macos_SDK)/System/iOSSupport/System/Library/Frameworks

# Common

# 1: xcframework platform
# 2: platform
# 3: PLATFORM (platform uppercased)
define FlagsTemplate1
$(1)_CFLAGS += \
	-isysroot $($(1)_SDK) \
	$(CFLAGS) \
	$($(2)_DEFINES) \
	-L$($(1)_SDK)/usr/lib/swift \
	-L$(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/lib/swift/$(SWIFT_TOOLCHAIN_$(1)) \
	-Wno-unused-command-line-argument

$(1)_SWIFTFLAGS += $(SWIFTFLAGS) -sdk $($(1)_SDK)

flags::
	@printf "$(1) = $(2) = $(3) = \n\t$(1)_CFLAGS=$$($(1)_CFLAGS)\n\t$(1)_SWIFTFLAGS=$$($(1)_SWIFTFLAGS)\n"
endef
$(foreach xcframeworkPlatform,$(XCFRAMEWORK_PLATFORMS),$(eval $(call FlagsTemplate1,$(xcframeworkPlatform),$(DOTNET_$(xcframeworkPlatform)_PLATFORM),$(shell echo $(DOTNET_$(xcframeworkPlatform)_PLATFORM) | tr 'a-z' 'A-Z'))))

# 1: xcframework platform
# 2: platform
# 3: PLATFORM (platform uppercased)
# 4: runtime identifier
define FlagsTemplate2
$(4)_CFLAGS += $$($(1)_CFLAGS) -arch $(DOTNET_$(4)_ARCHITECTURES) $(CLANG_$(4)_VERSION_MIN)
$(4)_OBJC_FLAGS += $$($(4)_CFLAGS) $(OBJC_CFLAGS)
$(4)_SWIFTFLAGS += $$($(1)_SWIFTFLAGS) $(SWIFTC_$(4)_VERSION_MIN)

flags::
	@printf "$(1) = $(2) = $(3) = $(4)\n\t$(4)_CFLAGS=$$($(4)_CFLAGS)\n\t$(4)_OBJC_FLAGS=$$($(4)_OBJC_FLAGS)\n\t$(4)_SWIFTFLAGS=$$($(4)_SWIFTFLAGS)\n"
endef
$(foreach xcframeworkPlatform,$(XCFRAMEWORK_PLATFORMS),$(foreach rid,$(XCFRAMEWORK_$(xcframeworkPlatform)_RUNTIME_IDENTIFIERS),$(eval $(call FlagsTemplate2,$(xcframeworkPlatform),$(DOTNET_$(xcframeworkPlatform)_PLATFORM),$(shell echo $(DOTNET_$(xcframeworkPlatform)_PLATFORM) | tr 'a-z' 'A-Z'),$(rid)))))

DEBUG_FLAGS=-DDEBUG -D_LIBCPP_HARDENING_MODE=_LIBCPP_HARDENING_MODE_DEBUG
RELEASE_FLAGS=-O2 -D_LIBCPP_HARDENING_MODE=_LIBCPP_HARDENING_MODE_FAST

# 1: xcframework platform
# 2: platform
# 3: PLATFORM (platform uppercased)
# 1: runtime identifier
# 2: suffix
# 3: additional compiler flags
define NativeCompilationTemplate
# Compile Objective-C source (.m) into object file (.o)
.libs/$(1)/%$(2).o: %.m $(EXTRA_DEPENDENCIES) | .libs/$(1)
	$$(call Q_2,OBJC,  [$(1)]) $(CLANG) $$($(1)_OBJC_FLAGS) $$(EXTRA_DEFINES) $(3) -c $$< -o $$@

# Compile C source (.c) into object file (.o)
.libs/$(1)/%$(2).o: %.c $(EXTRA_DEPENDENCIES) | .libs/$(1)
	$$(call Q_2,CC,    [$(1)]) $(CLANG) $$($(1)_CFLAGS) $$(EXTRA_DEFINES) $(3) -c $$< -o $$@

# Compile Assembly source (.s) into object file (.o)
.libs/$(1)/%$(2).o: %.s $(EXTRA_DEPENDENCIES) | .libs/$(1)
	$$(call Q_2,ASM,   [$(1)]) $(CLANG) $$($(1)_CFLAGS)                   $(3) -c $$< -o $$@

# Compile Swift source (.swift) into dynamic library (.dylib)
.libs/$(1)/%$(2).dylib: %.swift | .libs/$(1)
	$$(call Q_2,SWIFT, [$(1)]) $(SWIFTC) $$($(1)_SWIFTFLAGS) $(EXTRA_SWIFTFLAGS) $$(EXTRA_$$*_FLAGS) $$< -o $$@ -emit-module -L$$(dir $$@) -I$$(dir $$@) -module-name $$*

# Compile Swift source (.swift) into object file (.o)
.libs/$(1)/%$(2).o: %.swift | .libs/$(1)
	$$(call Q_2,SWIFT, [$(1)]) $(SWIFTC) $$($(1)_SWIFTFLAGS) $(EXTRA_SWIFTFLAGS) $$(EXTRA_$$*_FLAGS) $$< -o $$@ -emit-object

# Compile anything into dynamic library (.dylib) using clang.
# Set the dependencies for this target to add source code to be compiled or other libraries or object files to be linked into the resulting dylib.
.libs/$(1)/%$(2).dylib: | .libs/$(1)
	$$(call Q_2,LD,    [$(1)]) $(CLANG) $$($(1)_CFLAGS) $$(EXTRA_FLAGS) -dynamiclib -o $$@ $$^ -fapplication-extension -framework Foundation
endef
$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call NativeCompilationTemplate,$(rid),,-O2)))
$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call NativeCompilationTemplate,$(rid),-debug,$(DEBUG_FLAGS))))
$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call NativeCompilationTemplate,$(rid),-dotnet,$(RELEASE_FLAGS) -DDOTNET)))
$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call NativeCompilationTemplate,$(rid),-dotnet-debug,$(DEBUG_FLAGS) -DDOTNET)))
$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call NativeCompilationTemplate,$(rid),-dotnet-coreclr,$(RELEASE_FLAGS) -DCORECLR_RUNTIME -DDOTNET)))
$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call NativeCompilationTemplate,$(rid),-dotnet-coreclr-debug,$(DEBUG_FLAGS) -DCORECLR_RUNTIME -DDOTNET)))
$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call NativeCompilationTemplate,$(rid),-dotnet-nativeaot,$(RELEASE_FLAGS) -DCORECLR_RUNTIME -DDOTNET -DNATIVEAOT)))
$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call NativeCompilationTemplate,$(rid),-dotnet-nativeaot-debug,$(DEBUG_FLAGS) -DCORECLR_RUNTIME -DDOTNET -DNATIVEAOT)))

%.csproj.inc: %.csproj $(TOP)/Make.config $(TOP)/mk/mono.mk $(TOP)/tools/common/create-makefile-fragment.sh
	$(Q) $(TOP)/tools/common/create-makefile-fragment.sh $(abspath $<) $(abspath $@)

DIRS = \
	$(foreach xcframeworkPlatform,$(XCFRAMEWORK_PLATFORMS),.libs/$(xcframeworkPlatform)) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(foreach xcplatform,$(DOTNET_$(platform)_SDK_PLATFORMS),.libs/$(xcplatform))) \
	$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),.libs/$(rid)) \

$(sort $(DIRS)):
	$(Q) mkdir -p $@
