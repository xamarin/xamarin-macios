TOP=../../../..
include $(TOP)/Make.config

unexport MSBUILD_EXE_PATH

.libs:
	$(Q) mkdir -p $@

PACKAGE_ID=$(shell grep PackageId DynamicLibrariesInRuntimesNativeDirectory.csproj | sed 's_.*<PackageId>\(.*\)</PackageId>.*_\1_')
PACKAGE_VERSION=$(shell grep '<PackageVersion>' DynamicLibrariesInRuntimesNativeDirectory.csproj | sed 's_.*<PackageVersion>\(.*\)</PackageVersion>.*_\1_')

# Test case for dynamic libraries
.libs/DynamicLibrariesInRuntimesNativeDirectory.nupkg: export DOTNET_PLATFORMS:=$(shell echo $(DOTNET_PLATFORMS) | tr ' ' ';')
.libs/DynamicLibrariesInRuntimesNativeDirectory.nupkg: DynamicLibrariesInRuntimesNativeDirectory.csproj $(wildcard *.cs) | .libs
	$(Q) mkdir -p $(abspath $(NUGET_TEST_FEED))
	$(Q_GEN) $(DOTNET) pack /bl $(DOTNET_PACK_VERBOSITY) $<
	$(Q) $(CP) bin/Debug/Xamarin.Tests.DynamicLibrariesInRuntimesNativeDirectory.$(PACKAGE_VERSION).nupkg $@

INSTALLED_PACKAGE=$(NUGET_TEST_FEED)/xamarin.tests.DynamicLibrariesInRuntimesNativeDirectory/$(PACKAGE_VERSION)/xamarin.tests.DynamicLibrariesInRuntimesNativeDirectory.$(PACKAGE_VERSION).nupkg

$(INSTALLED_PACKAGE): .libs/DynamicLibrariesInRuntimesNativeDirectory.nupkg
	if test -d $(NUGET_TEST_FEED)/$(PACKAGE_ID)/$(PACKAGE_VERSION); then nuget delete $(PACKAGE_ID) $(PACKAGE_VERSION) -source $(abspath $(NUGET_TEST_FEED)) -NonInteractive || true; fi
	rm -Rf $(TOP)/tests/dotnet/packages/xamarin.tests.DynamicLibrariesInRuntimesNativeDirectory
	mkdir -p $(abspath $(NUGET_TEST_FEED))
	nuget add "$<" -source $(abspath $(NUGET_TEST_FEED)) -NonInteractive

ifdef ENABLE_DOTNET
all-local:: $(INSTALLED_PACKAGE)
endif
