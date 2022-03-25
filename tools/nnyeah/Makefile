TOP=../..
include $(TOP)/Make.config

all-local:: bin/Debug/net5.0/nnyeah.dll

install-local:: all-local

bin/Debug/net5.0/nnyeah.dll: $(wildcard **/*.cs) $(wildcard **/*.csproj) $(wildcard *.snl) Makefile
	$(Q_BUILD) $(SYSTEM_MSBUILD) "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) $(wildcard *.snl)

clean:
	$(Q_BUILD) $(SYSTEM_MSBUILD) "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) /t:Clean $(wildcard *.snl)
