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

TEST_FRAMEWORKS+=XTest
TEST_FRAMEWORKS+=XStaticArTest
TEST_FRAMEWORKS+=XStaticObjectTest
TEST_FRAMEWORKS+=SwiftTest
TEST_FRAMEWORKS+=SwiftTest2

DYNAMIC_TEST_FRAMEWORKS+=XTest
STATIC_AR_TEST_FRAMEWORKS+=XStaticArTest
STATIC_OBJECT_TEST_FRAMEWORKS+=XStaticObjectTest
DYNAMIC_TEST_FRAMEWORKS+=SwiftTest
DYNAMIC_TEST_FRAMEWORKS+=SwiftTest2

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

TESTGENERATOR=bin/Debug/testgenerator.dll
$(TESTGENERATOR): export RUNTIMEIDENTIFIER=
$(TESTGENERATOR): export RUNTIMEIDENTIFIERS=
$(TESTGENERATOR): testgenerator.cs Makefile
	$(Q) unset MSBUILD_EXE_PATH && $(DOTNET) build testgenerator.csproj $(DOTNET_BUILD_VERBOSITY) /bl:$@.binlog

$(GENERATED_FILES_PATTERN): $(TESTGENERATOR)
	$(Q) $(DOTNET) exec $<

define DefineInfixes
$(1)_INFO_PLIST_INFIX=/Versions/A/Resources
$(1)_BINARY_INFIX=/Versions/A
endef
$(foreach platform,$(DOTNET_DESKTOP_PLATFORMS),$(foreach rid,$(DOTNET_$(shell echo $(platform)| tr 'a-z' 'A-Z')_RUNTIME_IDENTIFIERS),$(eval $(call DefineInfixes,$(rid)))))
$(foreach platform,$(DOTNET_DESKTOP_PLATFORMS),$(eval $(call DefineInfixes,$(shell echo $(platform) | tr 'A-Z' 'a-z'))))
$(foreach platform,$(DOTNET_DESKTOP_PLATFORMS),$(eval $(call DefineInfixes,$(shell echo $(platform) | tr 'a-z' 'A-Z'))))

# Create the binary for all the dynamic test frameworks
define TestDynamicFrameworkTemplate
TEST_FRAMEWORK_$(1)_$(2)_TARGETS += \
	.libs/$(1)/$(2).framework$($(1)_INFO_PLIST_INFIX)/Info.plist \

.libs/$(1)/$(2).framework$($(1)_BINARY_INFIX)/$(2): .libs/$(1)/lib$(2).dylib | .libs/$(1)/$(2).framework$($(1)_BINARY_INFIX)
	$$(Q) $(CP) $$^ $$@
	$$(Q) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/install_name_tool -id @rpath/$(2).framework/$(2) $$@
	$$(Q) for lib in $$(DYNAMIC_TEST_FRAMEWORKS); do $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/install_name_tool -change .libs/$(1)/lib$$$$lib.dylib @rpath/$$$$lib.framework/$$$$lib $$@; done

.libs/$(1)/$(2).framework$($(1)_INFO_PLIST_INFIX)/Info.plist: plists/$(2)-Info-$(1).plist | .libs/$(1)/$(2).framework$($(1)_INFO_PLIST_INFIX)
	$$(Q) $(CP) $$^ $$@
endef
$(foreach testFramework,$(DYNAMIC_TEST_FRAMEWORKS),$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call TestDynamicFrameworkTemplate,$(rid),$(testFramework)))))

# Create the binary for all the static (.a) test frameworks
define TestStaticArFrameworkTemplate
.libs/$(1)/$(2).framework/$(2): .libs/$(1)/lib$(2).a | .libs/$(1)/$(2).framework
	$$(Q) $(CP) $$^ $$@
endef
$(foreach testFramework,$(STATIC_AR_TEST_FRAMEWORKS),$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call TestStaticArFrameworkTemplate,$(rid),$(testFramework)))))

# Create the binary for all the static (.o) test frameworks
define TestStaticObjectFrameworkTemplate
.libs/$(1)/$(2).framework/$(2): .libs/$(1)/lib$(2).o | .libs/$(1)/$(2).framework
	$$(Q) $(CP) $$^ $$@
endef
$(foreach testFramework,$(STATIC_OBJECT_TEST_FRAMEWORKS),$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call TestStaticObjectFrameworkTemplate,$(rid),$(testFramework)))))

# Create all the test frameworks
define TestDynamicFrameworkTemplate
TEST_DYNAMIC_FRAMEWORK_$(1)_$(2)_TARGETS += \
	.libs/$(1)/$(2).framework$($(1)_BINARY_INFIX)/$(2) \

TEST_DYNAMIC_FRAMEWORK_$(1)_TARGETS = \
	$$(TEST_DYNAMIC_FRAMEWORK_$(1)_$(2)_TARGETS) \
	.libs/$(1)/$(2).framework \
	.libs/$(1)/$(2).framework.stamp \

.libs/$(1)/$(2).framework.stamp: $$(TEST_DYNAMIC_FRAMEWORK_$(1)_$(2)_TARGETS)
	$$(Q) touch $$@

TEST_DYNAMIC_FRAMEWORK_$(1)_DIRECTORIES = \
	.libs/$(1)/$(2).framework/Versions \
	.libs/$(1)/$(2).framework \

$$(TEST_DYNAMIC_FRAMEWORK_$(1)_DIRECTORIES):
	$$(Q) mkdir -p $$@

all-local:: $$(TEST_DYNAMIC_FRAMEWORK_$(1)_TARGETS)
endef
$(foreach testFramework,$(DYNAMIC_TEST_FRAMEWORKS),$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call TestDynamicFrameworkTemplate,$(rid),$(testFramework)))))

define TestStaticFrameworkTemplate
TEST_STATIC_FRAMEWORK_$(1)_$(2)_TARGETS += \
	.libs/$(1)/$(2).framework/$(2) \

TEST_STATIC_FRAMEWORK_$(1)_TARGETS = \
	$$(TEST_STATIC_FRAMEWORK_$(1)_$(2)_TARGETS) \
	.libs/$(1)/$(2).framework \
	.libs/$(1)/$(2).framework.stamp \

.libs/$(1)/$(2).framework.stamp: $$(TEST_STATIC_FRAMEWORK_$(1)_$(2)_TARGETS)
	$$(Q) touch $$@

TEST_STATIC_FRAMEWORK_$(1)_DIRECTORIES = \
	.libs/$(1)/$(2).framework/Versions \
	.libs/$(1)/$(2).framework \

$$(TEST_STATIC_FRAMEWORK_$(1)_DIRECTORIES):
	$$(Q) mkdir -p $$@

all-local:: $$(TEST_STATIC_FRAMEWORK_$(1)_TARGETS)
endef
$(foreach testFramework,$(STATIC_AR_TEST_FRAMEWORKS),$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call TestStaticFrameworkTemplate,$(rid),$(testFramework)))))
$(foreach testFramework,$(STATIC_OBJECT_TEST_FRAMEWORKS),$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call TestStaticFrameworkTemplate,$(rid),$(testFramework)))))


# Create the symlinks for the test frameworks for desktop platforms
define TestFrameworkSymlinksTemplate
SYMLINK_$(1)_TARGETS += \
	.libs/$(1)/$(2).framework/$(2) \
	.libs/$(1)/$(2).framework/Resources \
	.libs/$(1)/$(2).framework/Versions/Current \
	.libs/$(1)/$(2).framework/Versions/A/Resources/Info.plist \

.libs/$(1)/$(2).framework$($(1)_BINARY_INFIX) .libs/$(1)/$(2).framework$($(1)_INFO_PLIST_INFIX):
	$$(Q) mkdir -p $$@

.libs/$(1)/$(2).framework/$(2): | .libs/$(1)/$(2).framework
	$$(Q) ln -fs Versions/A/$(2) $$@

.libs/$(1)/$(2).framework/Resources: | .libs/$(1)/$(2).framework
	$$(Q) ln -fs Versions/Current/Resources $$@

.libs/$(1)/$(2).framework/Versions/Current: | .libs/$(1)/$(2).framework/Versions
	$$(Q) ln -fs A $$@

all-locals:: $$(SYMLINK_$(1)_TARGETS)
endef
$(foreach testFramework,$(DYNAMIC_TEST_FRAMEWORKS),$(foreach platform,$(DOTNET_DESKTOP_PLATFORMS),$(foreach rid,$(DOTNET_$(shell echo $(platform) | tr 'a-z' 'A-Z')_RUNTIME_IDENTIFIERS),$(eval $(call TestFrameworkSymlinksTemplate,$(rid),$(testFramework))))))

# Create a framework for each platform, which would potentially be a fat framework.
# Here 'platform' is defined as a platform for xcframework - ios device is one platform, where ios simulator is another (which would be fat: iossimulator-x64,iossimulator-arm64).
define PlatformFrameworkTemplate
.libs/$(1)/$(2).framework$($(1)_BINARY_INFIX)/$(2): $$(foreach rid,$(3),.libs/$$(rid)/$(2).framework$($(1)_BINARY_INFIX)/$(2))
	$$(Q) mkdir -p $$(dir $$@)
	$$(Q) lipo -create -output "$$@" $$^

.libs/$(1)/$(2).framework.stamp: .libs/$(1)/$(2).framework$($(1)_BINARY_INFIX)/$(2)
	$$(Q) touch $$@

PLATFORM_FRAMEWORK_TEMPLATE_$(1)_TARGETS += \
	.libs/$(1)/$(2).framework$($(1)_BINARY_INFIX)/$(2) \

all-local:: $$(PLATFORM_FRAMEWORK_TEMPLATE_$(1)_TARGETS)
endef
$(foreach testFramework,$(DYNAMIC_TEST_FRAMEWORKS),$(foreach xcframeworkPlatform,$(XCFRAMEWORK_PLATFORMS),$(eval $(call PlatformFrameworkTemplate,$(xcframeworkPlatform),$(testFramework),$(XCFRAMEWORK_$(xcframeworkPlatform)_RUNTIME_IDENTIFIERS)))))

# Create a framework for each platform, which would potentially be a fat framework.
# Here 'platform' is defined as a platform for xcframework - ios device is one platform, where ios simulator is another (which would be fat: iossimulator-x64,iossimulator-arm64).
define PlatformDynamicFrameworkTemplate
.libs/$(1)/$(2).framework$($(1)_INFO_PLIST_INFIX)/Info.plist: $$(foreach rid,$(3),.libs/$$(rid)/$(2).framework$($(1)_INFO_PLIST_INFIX)/Info.plist)
	$$(Q) mkdir -p $$(dir $$@)
	@# Check if the Info.plists are identical (if there are more than one)
	$$(Q) test "$$(words $$^)" -eq 1 || diff $$^
	@# Copy one of them
	$$(Q) $(CP) $$< $$@

.libs/$(1)/$(2).framework.stamp: .libs/$(1)/$(2).framework$($(1)_INFO_PLIST_INFIX)/Info.plist

all-local:: .libs/$(1)/$(2).framework$($(1)_INFO_PLIST_INFIX)/Info.plist
endef
$(foreach testFramework,$(DYNAMIC_TEST_FRAMEWORKS),$(foreach xcframeworkPlatform,$(XCFRAMEWORK_PLATFORMS),$(eval $(call PlatformDynamicFrameworkTemplate,$(xcframeworkPlatform),$(testFramework),$(XCFRAMEWORK_$(xcframeworkPlatform)_RUNTIME_IDENTIFIERS)))))

# Create a framework for each platform, which would potentially be a fat framework.
# Here 'platform' is defined as a platform for xcframework - ios device is one platform, where ios simulator is another (which would be fat: iossimulator-x64,iossimulator-arm64).
define PlatformStaticFrameworkTemplate
.libs/$(1)/$(2).framework/$(2): $$(foreach rid,$(3),.libs/$$(rid)/$(2).framework/$(2))
	$$(Q) mkdir -p $$(dir $$@)
	$$(Q) lipo -create -output "$$@" $$^

.libs/$(1)/$(2).framework.stamp: .libs/$(1)/$(2).framework/$(2)
	$$(Q) touch $$@

PLATFORM_STATIC_FRAMEWORK_TEMPLATE_$(1)_TARGETS += \
	.libs/$(1)/$(2).framework/$(2) \

all-local:: $$(PLATFORM_STATIC_FRAMEWORK_TEMPLATE_$(1)_TARGETS)
endef
$(foreach testFramework,$(STATIC_AR_TEST_FRAMEWORKS),$(foreach xcframeworkPlatform,$(XCFRAMEWORK_PLATFORMS),$(eval $(call PlatformStaticFrameworkTemplate,$(xcframeworkPlatform),$(testFramework),$(XCFRAMEWORK_$(xcframeworkPlatform)_RUNTIME_IDENTIFIERS)))))
$(foreach testFramework,$(STATIC_OBJECT_TEST_FRAMEWORKS),$(foreach xcframeworkPlatform,$(XCFRAMEWORK_PLATFORMS),$(eval $(call PlatformStaticFrameworkTemplate,$(xcframeworkPlatform),$(testFramework),$(XCFRAMEWORK_$(xcframeworkPlatform)_RUNTIME_IDENTIFIERS)))))

# Create a static library for each platform, which would potentially be a fat static library.
# Here 'platform' is defined as a platform for xcframework - ios device is one platform, where ios simulator is another (which would be fat: iossimulator-x64,iossimulator-arm64).
define PlatformStaticTemplate
.libs/$(1)/$(2).a: $$(foreach rid,$(3),.libs/$$(rid)/$(2).a)
	$$(Q) mkdir -p $$(dir $$@)
	$$(Q) lipo -create -output "$$@" $$^

all-local:: .libs/$(1)/$(2).a
endef
$(foreach testFramework,libtest libtest2,$(foreach xcframeworkPlatform,$(XCFRAMEWORK_PLATFORMS),$(eval $(call PlatformStaticTemplate,$(xcframeworkPlatform),$(testFramework),$(XCFRAMEWORK_$(xcframeworkPlatform)_RUNTIME_IDENTIFIERS)))))

# Create a dynamic library for each platform, which would potentially be a fat dynamic library.
# Here 'platform' is defined as a platform for xcframework - ios device is one platform, where ios simulator is another (which would be fat: iossimulator-x64,iossimulator-arm64).
define PlatformDynamicTemplate
.libs/$(1)/$(2).dylib: $$(foreach rid,$(3),.libs/$$(rid)/$(2).dylib)
	$$(Q) mkdir -p $$(dir $$@)
	$$(Q) lipo -create -output "$$@" $$^

all-local:: .libs/$(1)/$(2).dylib
endef
$(foreach testFramework,libframework,$(foreach xcframeworkPlatform,$(XCFRAMEWORK_PLATFORMS),$(eval $(call PlatformDynamicTemplate,$(xcframeworkPlatform),$(testFramework),$(XCFRAMEWORK_$(xcframeworkPlatform)_RUNTIME_IDENTIFIERS)))))

# Create the symlinks for the per-platform frameworks (for desktop platforms)
# Here 'platform' is defined as a platform for xcframework (see above)
define PlatformFrameworkSymlinksTemplate
.libs/$(1)/$(2).framework/$(2):
	$$(Q) mkdir -p $$(dir $$@)
	$$(Q) ln -fs Versions/A/$(2) $$@

.libs/$(1)/$(2).framework/Resources:
	$$(Q) mkdir -p $$(dir $$@)
	$$(Q) ln -fs Versions/A/Resources $$@

.libs/$(1)/$(2).framework/Versions/Current:
	$$(Q) mkdir -p $$(dir $$@)
	$$(Q) ln -fs A $$@

PLATFORM_FRAMEWORK_SYMLINKS_TEMPLATE_$(4)_TARGETS += \
	.libs/$(1)/$(2).framework/$(2) \
	.libs/$(1)/$(2).framework/Resources \
	.libs/$(1)/$(2).framework/Versions/Current \

.libs/$(1)/$(2).framework.stamp: | .libs/$(1)/$(2).framework/$(2)
.libs/$(1)/$(2).framework.stamp: | .libs/$(1)/$(2).framework/Versions/Current
.libs/$(1)/$(2).framework.stamp: | .libs/$(1)/$(2).framework/Resources

all-local:: $$(PLATFORM_FRAMEWORK_SYMLINKS_TEMPLATE_$(4)_TARGETS)
endef
$(foreach testFramework,$(DYNAMIC_TEST_FRAMEWORKS),$(foreach xcframeworkPlatform,$(XCFRAMEWORK_DESKTOP_PLATFORMS),$(eval $(call PlatformFrameworkSymlinksTemplate,$(xcframeworkPlatform),$(testFramework)))))

define ZippedFrameworkTemplate
.libs/$(1)/$(2).framework.zip: .libs/$(1)/$(2).framework.stamp
	$$(Q_ZIP) cd .libs/$(1) && $(ZIP) -r $(2).framework.zip $(2).framework

all-local:: .libs/$(1)/$(2).framework.zip
zip:: .libs/$(1)/$(2).framework.zip
endef
$(foreach testFramework,$(TEST_FRAMEWORKS),$(foreach xcframeworkPlatform,$(XCFRAMEWORK_PLATFORMS),$(eval $(call ZippedFrameworkTemplate,$(xcframeworkPlatform),$(testFramework)))))

define Template
$(1)_TARGETS = \
	.libs/$(1)/libtest2.a \
	.libs/$(1)/libtest.a \

all-local:: $$($(1)_TARGETS) $(GENERATED_FILES)

EXTRA_DEPENDENCIES = libtest.h $(GENERATED_FILES) rename.h

.libs/$(1)/lib%.a: .libs/$(1)/lib%.o
	$(Q) rm -f $$@
	$$(call Q_2,AR     [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/ar cru $$@ $$<

.libs/$(1)/libframework.dylib: libframework.m

# XTest is a framework where the binary code is a (fat) dynamic library
.libs/$(1)/libXTest.dylib: .libs/$(1)/libframework.dylib
	$$(Q) $(CP) $$^ $$@
	$$(Q) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/install_name_tool -id @rpath/XTest.framework/XTest $$@

# XStaticObjectTest is a framework where the binary code is a (fat) object file
.libs/$(1)/libXStaticObjectTest.o: .libs/$(1)/libtest-object.o
	$$(Q) rm -f $$@
	$$(Q) $$(CP) $$^ $$@

# XStaticArTest is a framework where the binary code is a (fat) ar archive (of object files)
.libs/$(1)/libXStaticArTest.a: .libs/$(1)/libtest-ar.a
	$$(Q) rm -f $$@
	$$(Q) $$(CP) $$^ $$@
endef

$(foreach platform,$(DOTNET_PLATFORMS),$(foreach rid,$(DOTNET_$(shell echo $(platform) | tr 'a-z' 'A-Z')_RUNTIME_IDENTIFIERS),$(eval $(call Template,$(rid),$(platform)))))

#
# Swift libraries and frameworks
#

EXTRA_libSwiftTest_FLAGS=-enable-library-evolution -emit-objc-header
EXTRA_libSwiftTest2_FLAGS=-emit-objc-header -lSwiftTest

# SwiftTest is a framework where the binary code is a (fat) dynamic library
SWIFT_TESTS+=SwiftTest
# SwiftTest2 is a framework where the binary code is a (fat) dynamic library, and which depends on SwiftTest.
SWIFT_TESTS+=SwiftTest2

define SwiftTestsPerPlatform
.libs/$(1)/$(3).dylib: $$(foreach rid,$(2),.libs/$(1)/$(3).dylib) | .libs/$(1)
	$$(call Q_2,LIPO   [$(1)]) $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin/lipo $$^ -create -output $$@
endef
$(foreach swiftTest,$(SWIFT_TESTS),$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call SwiftTestsPerPlatform,$(platform),$(DOTNET_$(shell echo $(platform) | tr 'a-z' 'A-Z')_RUNTIME_IDENTIFIERS),$(swiftTest)))))

# Make sure libSwiftTest is built before libSwiftTest2, because libSwiftTest2 depends on libSwiftTest
define SwiftTest2AddDependency
.libs/$(1)/libSwiftTest2.dylib: .libs/$(1)/libSwiftTest.dylib
endef
$(foreach rid,$(DOTNET_RUNTIME_IDENTIFIERS),$(eval $(call SwiftTest2AddDependency,$(rid))))

# create an xcframework of frameworks
define TestFrameworkXCFramework
$(1)_XCFRAMEWORKS += $$(foreach xcframeworkPlatform,$$(XCFRAMEWORK_PLATFORMS),.libs/$$(xcframeworkPlatform)/$(1).framework)
$(1)_XCTARGETS += $$(foreach xcframeworkPlatform,$$(XCFRAMEWORK_PLATFORMS),.libs/$$(xcframeworkPlatform)/$(1).framework.stamp)
.libs/$(1).xcframework: $$($(1)_XCTARGETS) Makefile
	$$(Q) rm -rf $$@
	$$(Q_GEN) $$(XCODE_DEVELOPER_ROOT)/usr/bin/xcodebuild -quiet -create-xcframework $$(foreach fw,$$($(1)_XCFRAMEWORKS),-framework $$(fw)) -output $$@
all-local:: .libs/$(1).xcframework
endef
$(foreach testFramework,$(TEST_FRAMEWORKS),$(eval $(call TestFrameworkXCFramework,$(testFramework))))

# create an xcframework of libraries
define TestLibraryXCFramework
$(1)_XCFRAMEWORKS += $$(foreach xcframeworkPlatform,$$(XCFRAMEWORK_PLATFORMS),.libs/$$(xcframeworkPlatform)/$(1).a)
$(1)_XCTARGETS += $$(foreach xcframeworkPlatform,$$(XCFRAMEWORK_PLATFORMS),.libs/$$(xcframeworkPlatform).stamp)
.libs/$(1).xcframework: $$($(1)_XCTARGETS) Makefile
	$$(Q) rm -rf $$@
	$$(Q_GEN) $$(XCODE_DEVELOPER_ROOT)/usr/bin/xcodebuild -quiet -create-xcframework $$(foreach fw,$$($(1)_XCFRAMEWORKS),-library $$(fw)) -output $$@
all-local:: .libs/$(1).xcframework
endef
$(eval $(call TestLibraryXCFramework,libtest))
$(eval $(call TestLibraryXCFramework,libtest2))

define ZippedXcframework
.libs/$(1).xcframework.zip: .libs/$(1).xcframework
	$$(Q_ZIP) cd .libs && $(ZIP) -r "$$(notdir $$@)" "$$(notdir $$<)"
all-local:: .libs/$(1).xcframework.zip
endef
#$(foreach testFramework,$(TEST_FRAMEWORKS),$(call ZippedXcframework,$(testFramework)))
$(foreach testFramework,$(TEST_FRAMEWORKS),$(eval $(call ZippedXcframework,$(testFramework))))
$(eval $(call ZippedXcframework,libtest))
$(eval $(call ZippedXcframework,libtest2))

include $(TOP)/mk/rules.mk

.SECONDARY:
