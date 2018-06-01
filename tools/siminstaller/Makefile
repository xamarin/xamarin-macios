TOP=../..
include $(TOP)/Make.config

all-local:: bin/Debug/siminstaller.exe

install-local:: all-local

bin/Debug/siminstaller.exe: $(wildcard *.cs) $(wildcard *.csproj) Makefile
	$(SYSTEM_MSBUILD) $(XBUILD_VERBOSITY) $(wildcard *.csproj)
