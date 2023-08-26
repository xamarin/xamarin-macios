TOP=../..
include $(TOP)/Make.config

ifdef INCLUDE_IOS
all-local:: nnyeah/bin/Debug/$(DOTNET_TFM)/nnyeah.dll
install-local:: all-local
endif


IOS_DLL=$(TOP)/src/build/ios/native-64/Xamarin.iOS.dll
NUPKG_VERSION=$(shell grep '<PackageVersion>' nnyeah/nnyeah.csproj | sed 's_.*<PackageVersion>\(.*\)</PackageVersion>.*_\1_')

$(IOS_DLL):
	@echo "Thou shalt run 'make all' in the root directory first"
	@exit 1

# Example TEST_FILTER:
#    TEST_FILTER="--filter FullyQualifiedName~BuildMyCocoaApp"
# Docs: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test#filter-option-details
run-tests:: ./nupkg/nnyeah.$(NUPKG_VERSION).nupkg
	$(DOTNET) test tests/nnyeah-tests.csproj $(TEST_FILTER)

nnyeah/bin/Debug/$(DOTNET_TFM)/nnyeah.dll: $(IOS_DLL) $(wildcard **/*.cs) $(wildcard **/*.csproj) $(wildcard *.sln)
	$(Q_BUILD) $(DOTNET) build "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) $(wildcard *.sln)

clean:
	$(Q_BUILD) $(DOTNET) build "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) /t:Clean $(wildcard *.sln)


./nupkg/nnyeah.$(NUPKG_VERSION).nupkg: nnyeah/bin/Debug/$(DOTNET_TFM)/nnyeah.dll
	$(Q) $(DOTNET) pack nnyeah/nnyeah.csproj
	$(Q) mkdir -p nupkg
	$(Q) cp nnyeah/bin/Debug/nnyeah.$(NUPKG_VERSION).nupkg ./nupkg/nnyeah.$(NUPKG_VERSION).nupkg

nuget-install:: ./nupkg/nnyeah.$(NUPKG_VERSION).nupkg
	$(Q) dotnet tool install --add-source ./nupkg -g nnyeah

nuget-uninstall::
	$(Q) dotnet tool uninstall --global nnyeah
