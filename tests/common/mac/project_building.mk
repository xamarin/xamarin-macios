SOURCES = $(TEST_SRC) \
	$(TOP)/tests/common/Configuration.cs \
	$(TOP)/tests/common/mac/ProjectTestHelpers.cs \
	$(TOP)/tools/common/Driver.cs \
	$(TOP)/tools/common/TargetFramework.cs \
	$(TOP)/src/ObjCRuntime/ErrorHelper.cs \
	$(TOP)/src/ObjCRuntime/RuntimeException.cs \
	$(SOURCES_FOR_LICENSE_MGMT)

# Everything that would invalidate a build
ALL_SOURCE_FILES = $(TEST_SRC) $(SOURCES) $(EXTRA_FILES) Makefile

export MD_APPLE_SDK_ROOT=$(shell dirname `dirname $(XCODE_DEVELOPER_ROOT)`)
export XBUILD_FRAMEWORK_FOLDERS_PATH=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPath=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

ifeq ($(V)$(BUILD_REVISION),)
NUNIT_VERBOSITY=-nologo -nodots
else
NUNIT_VERBOSITY=-labels
endif

all-local:: $(TESTDLL) $(EXTRA_DEPS)

clean-local::
	$(Q) rm -fr ./build

run run-test run-tests:: $(TESTDLL) $(EXTRA_DEPS)
	DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT) $(MONO_PREFIX)/bin/nunit-console $(TESTDLL) -xml=build/TestResult.xml -noshadow -nothread $(NUNIT_VERBOSITY)
	$(Q) test -z "$(BUILD_REVISION)" || echo @"MonkeyWrench: AddFile: $(abspath build/TestResult.xml)"

$(TESTDLL): $(ALL_SOURCE_FILES) 
	$(Q) mkdir -p build
	$(Q) $(SYSTEM_CSC) /debug $(SOURCES) -d:MMP_TEST -d:XAMCORE_2_0 -d:MONOMAC -t:library -r:nunit.framework -out:$(TESTDLL)
