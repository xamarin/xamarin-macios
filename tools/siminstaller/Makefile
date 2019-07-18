TOP=../..
include $(TOP)/Make.config

all-local:: bin/Debug/siminstaller.exe

install-local:: all-local

bin/Debug/siminstaller.exe: $(wildcard *.cs) $(wildcard *.csproj) Makefile
	$(Q_BUILD) $(SYSTEM_MSBUILD) /restore $(MSBUILD_VERBOSITY) $(wildcard *.csproj)
