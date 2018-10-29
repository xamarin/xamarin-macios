TOP=../..

include $(TOP)/Make.config

COLOR_GREEN:=$(shell tput setaf 120 2>/dev/null)
COLOR_CLEAR:=$(shell tput sgr0 2>/dev/null)

ifdef ENABLE_XAMARIN
all-local install-local::
	$(MAKE) -C $(MACCORE_PATH)/tools/mlaunch $@
else
all-local install-local::
	$(Q) $(CP) -R $(MACIOS_BINARIES_PATH)/mlaunch/bin/mlaunch $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin
	$(Q) $(CP) -R $(MACIOS_BINARIES_PATH)/mlaunch/lib/mlaunch $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib
endif

ifdef ENABLE_XAMARIN
publish:
	$(Q) mkdir -p $(MACIOS_BINARIES_PATH)/mlaunch/bin
	$(Q) mkdir -p $(MACIOS_BINARIES_PATH)/mlaunch/lib
	$(Q) $(CP) -R $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mlaunch $(MACIOS_BINARIES_PATH)/mlaunch/bin
	$(Q) $(CP) -R $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mlaunch $(MACIOS_BINARIES_PATH)/mlaunch/lib
	$(Q) cd $(MACIOS_BINARIES_PATH) && git add mlaunch && git commit -q -m "Bump mlaunch to xamarin/maccore@$(shell cd $(MACCORE_PATH) && git log -1 --pretty=%h)" && git log -1 --pretty=short
	$(Q) echo "$(COLOR_GREEN)A new version of mlaunch has been copied to $(MACIOS_BINARIES_PATH), and a commit created with the new version.$(COLOR_CLEAR)"
	$(Q) echo "$(COLOR_GREEN)Please create pull request in the macios-binaries repository for the new mlaunch version, and once merged, bump the submodule in xamarin-macios.$(COLOR_CLEAR)"
else
publish:
	@echo "Can't publish mlaunch binaries to the macios-binaries repository unless building mlaunch from source (by enabling the xamarin build)"
	@exit 1
endif
