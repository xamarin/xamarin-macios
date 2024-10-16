TOP=../../../..
include $(TOP)/Make.config

unexport MSBUILD_EXE_PATH

NAME=$(shell basename "$(CURDIR)")
LOWERCASED_NAME:=$(shell echo $(NAME) | tr 'A-Z' 'a-z')

.libs:
	$(Q) mkdir -p $@

PACKAGE_ID=$(shell grep PackageId $(NAME).csproj | sed 's_.*<PackageId>\(.*\)</PackageId>.*_\1_')
PACKAGE_VERSION=$(shell grep '<PackageVersion>' $(NAME).csproj | sed 's_.*<PackageVersion>\(.*\)</PackageVersion>.*_\1_')
NUPKG_PATH=bin/Release/$(PACKAGE_ID).$(PACKAGE_VERSION).nupkg

# Test case for dynamic libraries
$(NUPKG_PATH): export DOTNET_PLATFORMS:=$(shell echo $(DOTNET_PLATFORMS) | tr ' ' ';')
$(NUPKG_PATH): $(NAME).csproj $(wildcard *.cs) | .libs
	$(Q) rm -f $(NUPKG_PATH)
	$(Q) mkdir -p $(abspath $(NUGET_TEST_FEED))
	$(Q_GEN) $(DOTNET) pack /bl $(DOTNET_PACK_VERBOSITY) $<

INSTALLED_PACKAGE=$(NUGET_TEST_FEED)/$(PACKAGE_ID).$(PACKAGE_VERSION).nupkg

$(INSTALLED_PACKAGE): $(NUPKG_PATH)
	rm -rf $(NUGET_TEST_FEED)/$(PACKAGE_ID).*.nupkg
	$(CP) $< $@

all-local:: $(INSTALLED_PACKAGE)
