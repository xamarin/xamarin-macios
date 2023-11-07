TOP=../..

include $(TOP)/Make.config
include $(TOP)/mk/xamarin.mk

DOTNET_PLATFORMS_MOBILE=$(filter-out macOS MacCatalyst,$(DOTNET_PLATFORMS))

DOWNLOAD_STAMP_FILE=.download-$(MLAUNCH_NUGET_VERSION).stamp

ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_IOS
TARGETS += $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mlaunch
endif
endif

TARGETS += $(foreach platform,$(DOTNET_PLATFORMS_MOBILE),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/bin/mlaunch)

$(DOWNLOAD_STAMP_FILE):
	$(Q) $(DOTNET) restore download-mlaunch.csproj /bl:$@.binlog $(NSBUILD_VERBOSITY) /p:MlaunchNuGetVersion=$(MLAUNCH_NUGET_VERSION)
	$(Q) touch $@

$(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mlaunch: $(DOWNLOAD_STAMP_FILE)
	$(Q) rm -Rf $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mlaunch
	$(Q) rm -Rf $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mlaunch
	$(Q) mkdir -p $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/
	$(Q) mkdir -p $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/
	$(Q) $(CP) -R $(TOP)/packages/microsoft.tools.mlaunch/$(MLAUNCH_NUGET_VERSION)/mlaunch/bin/mlaunch $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/
	$(Q) $(CP) -R $(TOP)/packages/microsoft.tools.mlaunch/$(MLAUNCH_NUGET_VERSION)/mlaunch/lib/mlaunch $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/
	$(Q) chmod a+x $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mlaunch
	$(Q) chmod a+x $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mlaunch/mlaunch.app/Contents/MacOS/mlaunch

define DotNetInstall
$$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/bin/mlaunch: $$(DOWNLOAD_STAMP_FILE)
	$$(Q) rm -rf $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/bin/mlaunch
	$$(Q) rm -rf $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/lib/mlaunch
	$$(Q) mkdir -p $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/bin
	$$(Q) mkdir -p $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/lib
	$$(Q) $$(CP) -R $(TOP)/packages/microsoft.tools.mlaunch/$$(MLAUNCH_NUGET_VERSION)/mlaunch/bin/mlaunch $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/bin/mlaunch
	$$(Q) $$(CP) -R $(TOP)/packages/microsoft.tools.mlaunch/$$(MLAUNCH_NUGET_VERSION)/mlaunch/lib/mlaunch $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/lib
	$$(Q) chmod a+x $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/bin/mlaunch
	$$(Q) chmod a+x $$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/lib/mlaunch/mlaunch.app/Contents/MacOS/mlaunch
endef

$(foreach platform,$(DOTNET_PLATFORMS_MOBILE),$(eval $(call DotNetInstall,$(platform))))

clean-local::
ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_IOS
	$(Q) rm -rf $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mlaunch
	$(Q) rm -rf $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mlaunch
endif
endif
ifdef ENABLE_DOTNET
	$(Q) rm -rf $(foreach platform,$(DOTNET_PLATFORMS_MOBILE),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/tools/bin/mlaunch)
	$(Q) rm -rf $(foreach platform,$(DOTNET_PLATFORMS_MOBILE),$(DOTNET_DESTDIR)/$($(platform)_NUGET_SDK_NAME)/tools/lib/mlaunch)
endif
	$(Q) rm -rf .*.stamp obj .*.binlog

all-local:: $(TARGETS)
install-local:: $(TARGETS)
download: $(DOWNLOAD_STAMP_FILE)
