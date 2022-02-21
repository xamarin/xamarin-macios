TOP=../..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

APP_DIR=Xamarin.PreBuilt.iOS/bin/Debug/net6.0-ios/ios-arm64/Xamarin.PreBuilt.iOS.app

Xamarin.PreBuilt.iOS.app.zip: .build-stamp
	$(Q) rm -rf $@
	$(Q_GEN) cd $(APP_DIR) && zip -9r $(abspath $@) .

NuGet.config: $(TOP)/tests/dotnet/NuGet.config $(TOP)/NuGet.config
	$(Q) $(MAKE) $@ -C $(dir $<)
	$(Q) $(CP) $< $@

global.json: $(TOP)/tests/dotnet/global.json $(TOP)/global6.json
	$(Q) $(MAKE) $@ -C $(dir $<)
	$(Q) $(CP) $< $@

$(TOP)/tests/dotnet/%:
	$(Q) $(MAKE) -C $(dir $@) $*

.build-stamp: $(wildcard Xamarin.PreBuilt.iOS/*) Makefile NuGet.config global.json
	$(Q_GEN) $(DOTNET6) build Xamarin.PreBuilt.iOS/Xamarin.PreBuilt.iOS.csproj "/bl:$(abspath ./msbuild.binlog)" $(MSBUILD_VERBOSITY)
	$(Q) touch $@

$(DOTNET_DESTDIR)/$(IOS_WINDOWS_NUGET).Sdk/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip: Xamarin.PreBuilt.iOS.app.zip
	$(Q) mkdir -p $(dir $@)
	$(Q) $(CP) $< $@

all-local:: $(DOTNET_DESTDIR)/$(IOS_WINDOWS_NUGET).Sdk/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip

install-local:: $(DOTNET_DESTDIR)/$(IOS_WINDOWS_NUGET).Sdk/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip
