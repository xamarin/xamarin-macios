TOP=../..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

BUILD_DIR=bin/Debug/net7.0

DOTNET_TARGETS += \
	$(BUILD_DIR)/dotnet-linker.dll \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/tools/dotnet-linker/dotnet-linker.dll) \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/tools/dotnet-linker/dotnet-linker.pdb) \

DOTNET_DIRECTORIES += \
	$(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/tools/dotnet-linker) \

# dotnet-linker.csproj.inc contains the dotnet_linker_dependencies variable used to determine if mtouch needs to be rebuilt or not.
dotnet-linker.csproj.inc: export BUILD_EXECUTABLE=$(DOTNET) build
dotnet-linker.csproj.inc: export BUILD_VERBOSITY=$(DOTNET_BUILD_VERBOSITY)
dotnet-linker.csproj.inc: dotnet-linker.csproj
-include dotnet-linker.csproj.inc

$(BUILD_DIR)/dotnet-linker%dll $(BUILD_DIR)/dotnet-linker%pdb: Makefile $(dotnet_linker_dependencies)
	$(Q_DOTNET_BUILD) $(DOTNET) build dotnet-linker.csproj $(DOTNET_BUILD_VERBOSITY)
	$(Q) touch $(BUILD_DIR)/dotnet-linker.dll $(BUILD_DIR)/dotnet-linker.pdb

define InstallTemplate
$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/dotnet-linker/%: $(BUILD_DIR)/% | $(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/dotnet-linker
	$$(Q) $$(CP) $$< $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call InstallTemplate,$(platform))))

$(DOTNET_DIRECTORIES):
	$(Q) mkdir -p $@

all-local:: $(DOTNET_TARGETS)
install-local:: $(DOTNET_TARGETS)

VSCODE?="/Applications/Visual Studio Code.app/Contents/Resources/app/bin/code"
vscode:
	PATH=$(DOTNET_DIR):$(PATH) $(VSCODE) .
