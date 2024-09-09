TOP=../..
include $(TOP)/Make.config

EXECUTABLE=bin/Debug/siminstaller.dll

all-local:: $(EXECUTABLE)

install-local:: all-local

# we build using the system .NET, because we might execute before we've downloaded our own version of .NET (i.e. while provisioning dependencies)
$(EXECUTABLE): $(wildcard *.cs) $(wildcard *.csproj) Makefile
	$(Q_BUILD) cd $(HOME) && dotnet build $(abspath $(CURDIR))/*.csproj "/bl:$(abspath $@.binlog)" /p:UseSharedCompilation=false $(DOTNET_BUILD_VERBOSITY)
	$(Q) touch $@

print-simulators: $(EXECUTABLE)
	$(Q) cd $(HOME) && dotnet exec $(abspath $<) --xcode=$(abspath $(XCODE_DEVELOPER_ROOT)/../..) --print-simulators

only-check: $(EXECUTABLE)
	$(Q) cd $(HOME) && dotnet exec $(abspath $<) --xcode=$(abspath $(XCODE_DEVELOPER_ROOT)/../..) -q $(INSTALL_SIMULATORS) --only-check

install-simulators: $(EXECUTABLE)
	$(Q) cd $(HOME) && dotnet exec $(abspath $<) --xcode=$(abspath $(XCODE_DEVELOPER_ROOT)/../..) -q $(INSTALL_SIMULATORS)
