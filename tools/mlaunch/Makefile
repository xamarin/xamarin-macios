TOP=../..

include $(TOP)/Make.config

COLOR_GREEN:=$(shell tput setaf 120 2>/dev/null)
COLOR_CLEAR:=$(shell tput sgr0 2>/dev/null)

ifdef INCLUDE_XAMARIN_LEGACY
ifdef ENABLE_XAMARIN
ifdef INCLUDE_MAC
ifdef INCLUDE_IOS
ENABLE_MLAUNCH=1
endif
endif
endif
endif

DOTNET_PLATFORMS_MOBILE=$(filter-out macOS MacCatalyst,$(DOTNET_PLATFORMS))

ifdef ENABLE_MLAUNCH
build-and-install:
	$(MAKE) -C $(MACCORE_PATH)/tools/mlaunch all
	$(MAKE) -C $(MACCORE_PATH)/tools/mlaunch install
else
build-and-install:
	$(Q) $(CP) -R $(MACIOS_BINARIES_PATH)/mlaunch/bin/mlaunch $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin
	$(Q) $(CP) -R $(MACIOS_BINARIES_PATH)/mlaunch/lib/mlaunch $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib
endif

all-local:: build-and-install

ifdef ENABLE_MLAUNCH
clean-local::
	$(MAKE) -C $(MACCORE_PATH)/tools/mlaunch $@
endif

ifdef ENABLE_DOTNET
ifdef INCLUDE_IOS
all-local:: build-and-install
	$(MAKE) $(foreach platform,$(DOTNET_PLATFORMS_MOBILE),$(DOTNET_DESTDIR)/Microsoft.$(platform).Sdk/tools/bin/mlaunch)
	$(Q) for platform in $(DOTNET_PLATFORMS_MOBILE); do \
		$(CP) -R $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mlaunch $(DOTNET_DESTDIR)/Microsoft.$$platform.Sdk/tools/lib; \
	done

$(DOTNET_DESTDIR)/Microsoft.%.Sdk/tools/bin/mlaunch: $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mlaunch | $(DOTNET_DESTDIR)/Microsoft.%.Sdk/tools/bin
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/Microsoft.%.Sdk/tools/bin:
	$(Q) mkdir -p $@
endif
endif

ifdef ENABLE_XAMARIN
MACCORE_HASH:=$(shell cd $(MACCORE_PATH) && git log -1 --pretty=%h)
publish:
	$(Q) mkdir -p $(MACIOS_BINARIES_PATH)/mlaunch/bin
	$(Q) mkdir -p $(MACIOS_BINARIES_PATH)/mlaunch/lib
	$(Q) $(CP) -R $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mlaunch $(MACIOS_BINARIES_PATH)/mlaunch/bin
	$(Q) $(CP) -R $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mlaunch $(MACIOS_BINARIES_PATH)/mlaunch/lib
	$(Q) cd $(MACIOS_BINARIES_PATH) && git add mlaunch && git commit -q -m $$'Bump mlaunch to xamarin/maccore@$(MACCORE_HASH)\n\nRef: xamarin/maccore@$(MACCORE_HASH)' && git log -1 --pretty=short
	$(Q) echo "$(COLOR_GREEN)A new version of mlaunch has been copied to $(MACIOS_BINARIES_PATH), and a commit created with the new version.$(COLOR_CLEAR)"
	$(Q) echo "$(COLOR_GREEN)Please create pull request in the macios-binaries repository for the new mlaunch version, and once merged, bump the submodule in xamarin-macios.$(COLOR_CLEAR)"
else
publish:
	@echo "Can't publish mlaunch binaries to the macios-binaries repository unless building mlaunch from source (by enabling the xamarin build)"
	@exit 1
endif
