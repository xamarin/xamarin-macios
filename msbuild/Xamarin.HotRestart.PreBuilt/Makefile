TOP=../..

include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

APP_DIR=Xamarin.PreBuilt.iOS/bin/Debug/$(DOTNET_TFM)-ios/ios-arm64/Xamarin.PreBuilt.iOS.app

Xamarin.PreBuilt.iOS.app.zip: .build-stamp
	$(Q) rm -rf $@
	$(Q_GEN) cd $(APP_DIR) && zip -9r $(ZIP_VERBOSITY) $(abspath $@) .

$(TOP)/tests/dotnet/%:
	$(Q) $(MAKE) -C $(dir $@) $*

.build-stamp: $(wildcard Xamarin.PreBuilt.iOS/*) Makefile
	$(Q_GEN) if ! $(DOTNET) build Xamarin.PreBuilt.iOS/Xamarin.PreBuilt.iOS.csproj "/bl:$(abspath ./msbuild.binlog)" $(DOTNET_BUILD_VERBOSITY); then echo "Binlog: $(abspath ./msbuild.binlog)"; exit 1; fi
	$(Q) touch $@

$(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip: Xamarin.PreBuilt.iOS.app.zip
	$(Q) mkdir -p $(dir $@)
	$(Q) $(CP) $< $@

all-local:: $(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip

install-local:: $(DOTNET_DESTDIR)/$(IOS_NUGET_WINDOWS_SDK_NAME)/tools/msbuild/iOS/Xamarin.PreBuilt.iOS.app.zip
