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
	$(Q) rm -Rf $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/
	$(Q) rm -Rf $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/
	$(Q) mkdir -p $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/
	$(Q) mkdir -p $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/
	$(Q) $(CP) -R $(TOP)/packages/microsoft.tools.mlaunch/$(MLAUNCH_NUGET_VERSION)/mlaunch/bin/mlaunch $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/
	$(Q) $(CP) -R $(TOP)/packages/microsoft.tools.mlaunch/$(MLAUNCH_NUGET_VERSION)/mlaunch/lib/mlaunch $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/

define DotNetInstall
$$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/bin/mlaunch: $$(DOWNLOAD_STAMP_FILE)
	$$(Q) rm -rf $$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/bin/mlaunch
	$$(Q) rm -rf $$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/lib/mlaunch
	$$(Q) mkdir -p $$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/bin
	$$(Q) mkdir -p $$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/lib
	$$(Q) $$(CP) -R $(TOP)/packages/microsoft.tools.mlaunch/$$(MLAUNCH_NUGET_VERSION)/mlaunch/bin/mlaunch $$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/bin/mlaunch
	$$(Q) $$(CP) -R $(TOP)/packages/microsoft.tools.mlaunch/$$(MLAUNCH_NUGET_VERSION)/mlaunch/lib/mlaunch $$(DOTNET_DESTDIR)/Microsoft.$(1).Sdk/tools/lib
endef

$(foreach platform,$(DOTNET_PLATFORMS_MOBILE),$(eval $(call DotNetInstall,$(platform))))

clean-local::
ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_IOS
	$(Q) rm -rf $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/
	$(Q) rm -rf $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/
endif
endif
	$(Q) for platform in $(DOTNET_PLATFORMS_MOBILE); do \
		rm -rf $(DOTNET_DESTDIR)/Microsoft.$$platform.Sdk/tools/bin/mlaunch; \
		rm -rf $(DOTNET_DESTDIR)/Microsoft.$$platform.Sdk/tools/lib; \
	done
	$(Q) rm -rf .*.stamp obj .*.binlog

all-local:: $(TARGETS)
install-local:: $(TARGETS)
download: $(DOWNLOAD_STAMP_FILE)
