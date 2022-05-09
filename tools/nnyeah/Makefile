TOP=../..
include $(TOP)/Make.config

all-local:: nnyeah/bin/Debug/net5.0/nnyeah.dll

install-local:: all-local

IOS_DLL=$(TOP)/src/build/ios/native-64/Xamarin.iOS.dll

$(IOS_DLL):
	@echo "Thou shalt run 'make all' in the root directory first"
	@exit 1
  
run-tests::
	$(MAKE) run-tests -C integration

nnyeah/bin/Debug/net5.0/nnyeah.dll: $(IOS_DLL) $(wildcard **/*.cs) $(wildcard **/*.csproj) $(wildcard *.sln) Makefile
	$(Q_BUILD) $(SYSTEM_DOTNET) build "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) $(wildcard *.sln)

clean:
	$(Q_BUILD) $(SYSTEM_DOTNET) build "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) /t:Clean $(wildcard *.sln)

NUPKG_VERSION=$(shell grep '<PackageVersion>' nnyeah/nnyeah.csproj | sed 's_.*<PackageVersion>\(.*\)</PackageVersion>.*_\1_')

nuget::
	$(Q) $(SYSTEM_DOTNET) pack nnyeah/nnyeah.csproj
	$(Q) mkdir -p nupkg
	$(Q) cp nnyeah/bin/Debug/nnyeah.$(NUPKG_VERSION).nupkg ./nupkg/nnyeah.$(NUPKG_VERSION).nupkg

nuget-install:: nuget
	$(Q) dotnet tool install --add-source ./nupkg -g nnyeah

nuget-uninstall::
	$(Q) dotnet tool uninstall --global nnyeah
