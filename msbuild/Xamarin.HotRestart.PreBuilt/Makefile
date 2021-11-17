TOP=../..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

NuGet.config: $(TOP)/tests/dotnet/NuGet.config
	$(Q) $(CP) $< $@

global.json: $(TOP)/tests/dotnet/global.json
	$(Q) $(CP) $< $@

$(TOP)/tests/dotnet/%:
	$(Q) $(MAKE) -C $(dir $@) $*

$(DOTNET_DESTDIR)/$(IOS_WINDOWS_NUGET).Sdk/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip: NuGet.config global.json
	mkdir -p $(DOTNET_DESTDIR)/$(IOS_WINDOWS_NUGET).Sdk/tools/msbuild/iOS
	touch $@

all-local:: $(DOTNET_DESTDIR)/$(IOS_WINDOWS_NUGET).Sdk/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip

install-local:: $(DOTNET_DESTDIR)/$(IOS_WINDOWS_NUGET).Sdk/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip
