TOP=../../../..
include $(TOP)/Make.config

define INTEGRATION_TEST_template

TEST_BUILD_TARGETS += $(1)/bin/Debug/$(1)Integration.dll
$(1)/bin/Debug/$(1)Integration.dll: $(wildcard *.cs */*.cs Makefile) $(1)/$(1)Integration.csproj
	$(Q) $(SYSTEM_MSBUILD) $(1)/$(1)Integration.csproj $(MSBUILD_VERBOSITY)

TEST_CLEAN_TARGETS += clean-$(1)
clean-$(1)::
	$(Q) $(SYSTEM_MSBUILD) /t:Clean $(1)/$(1)Integration.csproj $(MSBUILD_VERBOSITY)
	$(Q) rm -rf Converted/$(1)

TEST_RUN_TARGETS += run-$(1)
run-$(1):: $(1)/bin/Debug/$(1)Integration.dll
	$(Q) rm -rf Converted/$(1)
	$(Q) mkdir -p Converted/$(1)
	$(Q) $(SYSTEM_DOTNET) run --project ../../nnyeah/nnyeah.csproj --input=$(1)/bin/Debug/$(1)Integration.dll --output=Converted/$(1)/$(1)Integration.dll --xamarin-assembly=$(2) --microsoft-assembly=$(3)

endef

LEGACY_IOS = $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/git/lib/mono/Xamarin.iOS/Xamarin.iOS.dll
NET_IOS = $(DOTNET_DESTDIR)/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.dll
LEGACY_MAC = $(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/git/lib/mono/Xamarin.Mac/Xamarin.Mac.dll
NET_MAC = $(DOTNET_DESTDIR)/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.dll

$(eval $(call INTEGRATION_TEST_template,macOS,$(LEGACY_MAC),$(NET_MAC)))
$(eval $(call INTEGRATION_TEST_template,iOS,$(LEGACY_IOS),$(NET_IOS)))

nuget/.stamp-nuget-download:
	$(Q) mkdir -p nuget/
	$(Q_CURL) curl -o nuget/xamarin.forms.googlemaps.3.3.0.nupkg  https://api.nuget.org/v3-flatcontainer/xamarin.forms.googlemaps/3.3.0/xamarin.forms.googlemaps.3.3.0.nupkg
	$(Q_ZIP) unzip -j nuget/xamarin.forms.googlemaps.3.3.0.nupkg "lib/Xamarin.iOS10/Xamarin.Forms.GoogleMaps.iOS.dll" -d nuget
	$(Q) touch $@

nuget-conversion:: nuget/.stamp-nuget-download
	$(Q) mkdir -p Converted/nuget
	$(Q) $(SYSTEM_DOTNET) run --project ../../nnyeah/nnyeah.csproj --input=nuget/Xamarin.Forms.GoogleMaps.iOS.dll --output=Converted/nuget/Xamarin.Forms.GoogleMaps.iOS.dll --xamarin-assembly=$(LEGACY_IOS) --microsoft-assembly=$(NET_IOS) --force-overwrite

all-local:: $(TEST_RUN_TARGETS)
install-local:: all-local
clean: $(TEST_CLEAN_TARGETS)
