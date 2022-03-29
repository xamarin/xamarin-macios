TOP=../..
include $(TOP)/Make.config

all-local:: nnyeah/bin/Debug/net5.0/nnyeah.dll

install-local:: all-local

nnyeah/bin/Debug/net5.0/nnyeah.dll: $(wildcard **/*.cs) $(wildcard **/*.csproj) $(wildcard *.sln) Makefile
	$(Q_BUILD) $(SYSTEM_DOTNET) build "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) $(wildcard *.sln)

clean:
	$(Q_BUILD) $(SYSTEM_DOTNET) build "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) /t:Clean $(wildcard *.sln)
