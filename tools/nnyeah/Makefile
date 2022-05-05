TOP=../..
include $(TOP)/Make.config

all-local:: nnyeah/bin/Debug/net5.0/nnyeah.dll

install-local:: all-local

nnyeah/bin/Debug/net5.0/nnyeah.dll: $(wildcard **/*.cs) $(wildcard **/*.csproj) $(wildcard *.sln) Makefile
	$(Q_BUILD) $(SYSTEM_DOTNET) build "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) $(wildcard *.sln)

clean:
	$(Q_BUILD) $(SYSTEM_DOTNET) build "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) /t:Clean $(wildcard *.sln)

nuget::
	$(Q) $(SYSTEM_DOTNET) pack nnyeah/nnyeah.csproj
	$(Q) mkdir -p nupkg
	$(Q) cp nnyeah/bin/Debug/nnyeah.0.1.0.nupkg ./nupkg/nnyeah.0.1.0.nupkg

nuget-install:: nuget
	$(Q) dotnet tool install --add-source ./nupkg -g nnyeah

nuget-uninstall::
	$(Q) dotnet tool uninstall --global nnyeah