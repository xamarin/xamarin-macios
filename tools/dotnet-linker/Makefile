TOP=../..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

BUILD_DIR=bin/Debug/net5.0

DOTNET_TARGETS += \
	$(BUILD_DIR)/dotnet-linker.dll \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/dotnet-linker/dotnet-linker.dll) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/dotnet-linker/dotnet-linker.pdb) \

DOTNET_DIRECTORIES += \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/dotnet-linker) \

# dotnet-linker.csproj.inc contains the dotnet_linker_dependencies variable used to determine if mtouch needs to be rebuilt or not.
dotnet-linker.csproj.inc: export BUILD_EXECUTABLE=$(DOTNET6) build
dotnet-linker.csproj.inc: export BUILD_VERBOSITY=$(DOTNET_BUILD_VERBOSITY)
dotnet-linker.csproj.inc: global.json
-include dotnet-linker.csproj.inc

$(BUILD_DIR)/dotnet-linker%dll $(BUILD_DIR)/dotnet-linker%pdb: Makefile global.json $(dotnet_linker_dependencies)
	$(Q_DOTNET_BUILD) $(DOTNET6) build dotnet-linker.csproj $(DOTNET_BUILD_VERBOSITY)

define InstallTemplate
$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/dotnet-linker/%: $(BUILD_DIR)/% | $(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/dotnet-linker
	$$(Q) $$(CP) $$< $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call InstallTemplate,$(platform))))

$(DOTNET_DIRECTORIES):
	$(Q) mkdir -p $@

# This tells NuGet to use the exact same dotnet version we've configured in Make.config
global.json: $(TOP)/Make.config.inc Makefile $(TOP)/.git/HEAD $(TOP)/.git/index
	$(Q_GEN) \
		printf "{\n" > $@; \
		printf "\t\"sdk\": { \"version\": \"$(DOTNET6_VERSION)\" }\n" >> $@; \
		printf "}\n" >> $@

all-local:: $(DOTNET_TARGETS)
install-local:: $(DOTNET_TARGETS)
