TOP=../..
include $(TOP)/Make.config

all-local:: nnyeah/bin/Debug/net5.0/nnyeah.dll

install-local:: all-local

IOS_DLL=$(TOP)/src/build/ios/native-64/Xamarin.iOS.dll

$(IOS_DLL):
	@echo "Thou shalt run 'make all' in the root directory first"
	@exit 1

nnyeah/bin/Debug/net5.0/nnyeah.dll: $(IOS_DLL) $(wildcard **/*.cs) $(wildcard **/*.csproj) $(wildcard *.sln) Makefile
	$(Q_BUILD) $(SYSTEM_DOTNET) build "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) $(wildcard *.sln)

clean:
	$(Q_BUILD) $(SYSTEM_DOTNET) build "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) /t:Clean $(wildcard *.sln)
