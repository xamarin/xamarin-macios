TOP=../..

include $(TOP)/Make.config

PLATFORMS=iOS tvOS watchOS macOS

BUILD_DIR=bin/Debug/net5.0

DOTNET_TARGETS += \
	$(BUILD_DIR)/dotnet-linker.dll \
	$(foreach platform,$(PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/dotnet-linker/dotnet-linker.dll) \
	$(foreach platform,$(PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/dotnet-linker/dotnet-linker.pdb) \

DOTNET_DIRECTORIES += \
	$(foreach platform,$(PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/dotnet-linker) \

$(BUILD_DIR)/dotnet-linker%dll $(BUILD_DIR)/dotnet-linker%pdb: Makefile $(dotnet_linker_dependencies)
	$(Q_DOTNET_BUILD) $(DOTNET5) build $(XBUILD_VERBOSITY)

$(DOTNET_DESTDIR)/Microsoft.iOS.Sdk/tools/dotnet-linker/%: $(BUILD_DIR)/% | $(DOTNET_DESTDIR)/Microsoft.iOS.Sdk/tools/dotnet-linker
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.tvOS.Sdk/tools/dotnet-linker/%: $(BUILD_DIR)/% | $(DOTNET_DESTDIR)/Microsoft.tvOS.Sdk/tools/dotnet-linker
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.watchOS.Sdk/tools/dotnet-linker/%: $(BUILD_DIR)/% | $(DOTNET_DESTDIR)/Microsoft.watchOS.Sdk/tools/dotnet-linker
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.macOS.Sdk/tools/dotnet-linker/%: $(BUILD_DIR)/% | $(DOTNET_DESTDIR)/Microsoft.macOS.Sdk/tools/dotnet-linker
	$(Q) $(CP) $< $@

$(DOTNET_DIRECTORIES):
	$(Q) mkdir -p $@

# dotnet-linker.csproj.inc contains the dotnet_linker_dependencies variable used to determine if mtouch needs to be rebuilt or not.
dotnet-linker.csproj.inc: export BUILD_EXECUTABLE=$(DOTNET5) build
dotnet-linker.csproj.inc: dotnet-linker.csproj Makefile ../common/create-makefile-fragment.sh $(TOP)/Make.config $(TOP)/mk/mono.mk
	$(Q_GEN) ../common/create-makefile-fragment.sh $(abspath $<)
-include dotnet-linker.csproj.inc

all-local:: $(DOTNET_TARGETS)
install-local:: $(DOTNET_TARGETS)
