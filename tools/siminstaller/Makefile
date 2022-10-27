TOP=../..
include $(TOP)/Make.config

all-local:: bin/Debug/siminstaller.exe

install-local:: all-local

bin/Debug/siminstaller.exe: $(wildcard *.cs) $(wildcard *.csproj) Makefile
	$(Q_BUILD) $(SYSTEM_MSBUILD) "/bl:$@.binlog" /restore $(MSBUILD_VERBOSITY) $(wildcard *.csproj)

print-simulators: bin/Debug/siminstaller.exe
	mono $< --print-simulators --xcode=$(abspath $(XCODE_DEVELOPER_ROOT)/../..)
