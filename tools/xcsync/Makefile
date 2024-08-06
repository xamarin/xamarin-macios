TOP=../..

include $(TOP)/Make.config
include $(TOP)/mk/xamarin.mk

DOWNLOAD_STAMP_FILE=.download-$(XCSYNC_NUGET_VERSION).stamp

TARGETS += $(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/bin/xcsync)

$(DOWNLOAD_STAMP_FILE):
	$(Q) $(DOTNET) restore download-xcsync.csproj /bl:$@.binlog $(MSBUILD_VERBOSITY) /p:XcsyncNuGetVersion=$(XCSYNC_NUGET_VERSION)
	$(Q) touch $@

define DotNetInstall
$$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/bin/xcsync: $$(DOWNLOAD_STAMP_FILE)
	$$(Q) rm -rf $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/bin/dotnet-xcsync
	$$(Q) rm -rf $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/lib/xcsync
	$$(Q) mkdir -p $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/bin
	$$(Q) mkdir -p $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/lib/xcsync
	$$(Q) $$(CP) -R $(TOP)/packages/microsoft.tools.xcsync/$$(XCSYNC_NUGET_VERSION)/bin/dotnet-xcsync $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/bin/dotnet-xcsync
	$$(Q) $$(CP) -R $(TOP)/packages/microsoft.tools.xcsync/$$(XCSYNC_NUGET_VERSION)/lib/. $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/lib/xcsync
	$$(Q) chmod a+x $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/bin/dotnet-xcsync
	$$(Q) chmod a+x $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/lib/xcsync/xcsync
endef

$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetInstall,$(platform))))

clean-local::
ifdef ENABLE_DOTNET
	$(Q) rm -rf $(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/tools/bin/dotnet-xcsync)
	$(Q) rm -rf $(foreach platform,$(DOTNET_PLATFORMS),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/tools/lib/xcsync)
endif
	$(Q) rm -rf .*.stamp obj .*.binlog

all-local:: $(TARGETS)
install-local:: $(TARGETS)
download: $(DOWNLOAD_STAMP_FILE)
