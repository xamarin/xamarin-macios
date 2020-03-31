TOP=../..

include $(TOP)/Make.config

TARGETS = \
	bin/Debug/netcoreapp3.0/dotnet-linker.dll \

all-local:: $(TARGETS)
install-local:: $(TARGETS)

# dotnet-linker.csproj.inc contains the dotnet_linker_dependencies variable used to determine if mtouch needs to be rebuilt or not.
dotnet-linker.csproj.inc: dotnet-linker.csproj Makefile ../common/create-makefile-fragment.sh $(TOP)/Make.config $(TOP)/mk/mono.mk
	$(Q_GEN) ../common/create-makefile-fragment.sh $(abspath $<)
-include dotnet-linker.csproj.inc

bin/Debug/netcoreapp3.0/dotnet-linker.dll: Makefile $(dotnet_linker_dependencies)
	$(DOTNET) build $(XBUILD_VERBOSITY)
