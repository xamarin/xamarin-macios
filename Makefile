TOP=.
SUBDIRS=builds runtime fsharp src msbuild tools
include $(TOP)/Make.config
include $(TOP)/mk/versions.mk

MONO_VERSION="`grep AC_INIT $(MONO_PATH)/configure.ac | sed -e 's/.*\[//' -e 's/\].*//'`"

#
# Xamarin.iOS
#

IOS_DIRECTORIES += \
	$(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX) \
	$(IOS_DESTDIR)/Developer/MonoTouch \
	$(IOS_DESTDIR)/Developer/MonoTouch/usr \
	$(IOS_DESTDIR)/Developer/MonoTouch/usr/lib/mono \

IOS_TARGETS += \
	$(IOS_INSTALL_DIRECTORIES) \
	$(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current \
	$(IOS_DESTDIR)/Developer/MonoTouch/usr/bin \
	$(IOS_DESTDIR)/Developer/MonoTouch/usr/lib/mono/2.1 \
	$(IOS_DESTDIR)/Developer/MonoTouch/usr/lib/mono/Xamarin.iOS \
	$(IOS_DESTDIR)/Developer/MonoTouch/updateinfo \
	$(IOS_DESTDIR)/Developer/MonoTouch/Version \
	$(IOS_DESTDIR)/Developer/MonoTouch/usr/share \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/buildinfo \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/Version \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/updateinfo \
	$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/Versions.plist \

$(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current: | $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions
	$(Q_LN) ln -hfs $(IOS_INSTALL_VERSION) $@

$(IOS_DESTDIR)/Developer/MonoTouch/usr/bin: | $(IOS_DESTDIR)/Developer/MonoTouch/usr
	$(Q_LN) ln -Fs $(abspath $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin) $@

$(IOS_DESTDIR)/Developer/MonoTouch/usr/lib/mono/2.1: | $(IOS_DESTDIR)/Developer/MonoTouch/usr/lib/mono
	$(Q_LN) ln -Fs $(abspath $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/2.1) $@

$(IOS_DESTDIR)/Developer/MonoTouch/usr/lib/mono/Xamarin.iOS: | $(IOS_DESTDIR)/Developer/MonoTouch/usr/lib/mono
	$(Q_LN) ln -Fs $(abspath $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS) $@

$(IOS_DESTDIR)/Developer/MonoTouch/updateinfo: | $(IOS_DESTDIR)/Developer/MonoTouch
	$(Q_LN) ln -fs $(abspath $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/updateinfo) $@

$(IOS_DESTDIR)/Developer/MonoTouch/Version: | $(IOS_DESTDIR)/Developer/MonoTouch
	$(Q_LN) ln -fs $(abspath $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/Version) $@

$(IOS_DESTDIR)/Developer/MonoTouch/usr/share: | $(IOS_DESTDIR)/Developer/MonoTouch/usr
	$(Q_LN) ln -Fs $(abspath $(IOS_TARGETDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/share) $@

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/buildinfo: Make.config.inc .git/index | $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
	$(Q_GEN) echo "Version: $(IOS_PACKAGE_VERSION)" > $@
	$(Q) echo "Hash: $(shell git log --oneline -1 --pretty=%h)" >> $@
	$(Q) echo "Branch: $(CURRENT_BRANCH)" >> $@
	$(Q) echo "Build date: $(shell date '+%Y-%m-%d %H:%M:%S%z')" >> $@

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/Version: Make.config.inc | $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
	$(Q) echo $(IOS_PACKAGE_VERSION) > $@

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/updateinfo: Make.config.inc
	$(Q) echo "4569c276-1397-4adb-9485-82a7696df22e $(IOS_PACKAGE_UPDATE_ID)" > $@

$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/Versions.plist: Versions-ios.plist.in Makefile $(TOP)/Make.config versions-check.csharp
	$(Q) ./versions-check.csharp $< "$(MIN_IOS_SDK_VERSION)" "$(IOS_SDK_VERSION)" "$(MIN_TVOS_SDK_VERSION)" "$(TVOS_SDK_VERSION)" "$(MIN_WATCH_OS_VERSION)" "$(WATCH_SDK_VERSION)" "$(MIN_OSX_SDK_VERSION)" "$(OSX_SDK_VERSION)"
	$(Q_GEN) sed -e 's/@XCODE_VERSION@/$(XCODE_VERSION)/g' -e "s/@MONO_VERSION@/$(MONO_VERSION)/g" $< > $@

ifdef INCLUDE_IOS
TARGETS += $(IOS_TARGETS)
DIRECTORIES += $(IOS_DIRECTORIES)
endif

#
# Xamarin.Mac
#

MAC_DIRECTORIES += \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Versions \
	$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR) \

MAC_TARGETS += \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Versions/Current \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Commands \
	$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/buildinfo \
	$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/Version \
	$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/updateinfo \
	$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/Versions.plist \

$(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Versions/Current: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Versions
	$(Q_LN) ln -hfs $(MAC_INSTALL_VERSION) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Commands:
	$(Q_LN) ln -hfs $(MAC_TARGETDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/bin $@

$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/buildinfo: Make.config.inc .git/index | $(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)
	$(Q_GEN) echo "Version: $(MAC_PACKAGE_VERSION)" > $@
	$(Q) echo "Hash: $(shell git log --oneline -1 --pretty=%h)" >> $@
	$(Q) echo "Branch: $(shell git symbolic-ref --short HEAD)" >> $@
	$(Q) echo "Build date: $(shell date '+%Y-%m-%d %H:%M:%S%z')" >> $@

$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/updateinfo: Make.config.inc
	$(Q) echo "0ab364ff-c0e9-43a8-8747-3afb02dc7731 $(MAC_PACKAGE_UPDATE_ID)" > $@

$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/Version: Make.config.inc
	$(Q) echo $(MAC_PACKAGE_VERSION) > $@

$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/Versions.plist: Versions-mac.plist.in Makefile $(TOP)/Make.config versions-check.csharp
	$(Q) ./versions-check.csharp $< "$(MIN_IOS_SDK_VERSION)" "$(IOS_SDK_VERSION)" "$(MIN_TVOS_SDK_VERSION)" "$(TVOS_SDK_VERSION)" "$(MIN_WATCH_OS_VERSION)" "$(WATCH_SDK_VERSION)" "$(MIN_OSX_SDK_VERSION)" "$(OSX_SDK_VERSION)"
	$(Q_GEN) sed -e 's/@XCODE_VERSION@/$(XCODE_VERSION)/g' -e "s/@MONO_VERSION@/$(MONO_VERSION)/g" $< > $@

ifdef INCLUDE_MAC
TARGETS += $(MAC_TARGETS)
DIRECTORIES += $(MAC_DIRECTORIES)
endif

#
# Common
#

.PHONY: world
world: check-system
	@$(MAKE) reset-versions
	@$(MAKE) all -j8
	@$(MAKE) install -j8

.PHONY: check-system
check-system:
	@./system-dependencies.sh

$(DIRECTORIES):
	$(Q) mkdir -p $@

$(TARGETS): | check-system

all-local:: $(TARGETS)
install-local:: $(TARGETS)

check-permissions:
ifdef INCLUDE_MAC
	@UNREADABLE=`find $(MAC_DESTDIR) ! -perm -0644`; if ! test -z "$$UNREADABLE"; then echo "There are files with invalid permissions (all installed files at least be readable by everybody, and writable by owner: 0644): "; find $(MAC_DESTDIR) ! -perm -0644 | xargs ls -la; exit 1; fi
	@echo Validated file permissions for Xamarin.Mac.
endif
ifdef INCLUDE_IOS
	@UNREADABLE=`find $(IOS_DESTDIR) ! -perm -0644`; if ! test -z "$$UNREADABLE"; then echo "There are files with invalid permissions (all installed files at least be readable by everybody, and writable by owner: 0644): "; find $(IOS_DESTDIR) ! -perm -0644 | xargs ls -la; exit 1; fi
	@echo Validated file permissions for Xamarin.iOS.
endif

install-hook::
	@$(MAKE) check-permissions
ifdef INCLUDE_IOS
ifneq ($(findstring $(IOS_DESTDIR)$(MONOTOUCH_PREFIX),$(shell ls -l /Library/Frameworks/Xamarin.iOS.framework/Versions/Current 2>&1)),)
	@echo
	@echo "	This build of Xamarin.iOS is the now default version on your system. "
	@echo
else
	@echo
	@echo "	Xamarin.iOS has not been installed into your system by 'make install'"
	@echo "	In order to set the currently built Xamarin.iOS as your system version,"
	@echo "	execute 'make install-system'".
	@echo
endif
endif
ifdef INCLUDE_MAC
ifndef INCLUDE_IOS
	@echo
endif
ifneq ($(findstring $(abspath $(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Versions),$(shell ls -l $(MAC_FRAMEWORK_DIR)/Versions/Current 2>&1)),)
	@echo "	This build of Xamarin.Mac is the now default version on your system. "
	@echo
else
	@echo "	Xamarin.Mac has not been installed into your system by 'make install'"
	@echo "	In order to set the currently built Xamarin.Mac as your system version,"
	@echo "	execute 'make install-system'".
	@echo
endif
endif

package:
	mkdir -p ../package
	$(MAKE) -C $(MACCORE_PATH) package
	# copy .pkg, .zip and *updateinfo to the packages directory to be uploaded to storage
	cp $(MACCORE_PATH)/release/*.pkg ../package
	cp $(MACCORE_PATH)/release/*.zip ../package
	cp $(MACCORE_PATH)/release/*updateinfo ../package

install-system: install-system-ios install-system-mac
	@# Clean up some old files
	$(Q) rm -Rf /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/iOS
	$(Q) rm -Rf /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Xamarin.ObjcBinding.CSharp.targets
	$(Q) rm -Rf /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Xamarin.Common.CSharp.targets
	$(Q) rm -Rf /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Xamarin.ObjcBinding.Tasks.dll
	$(Q) rm -Rf /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Mac
	$(Q) $(MAKE) install-symlinks MAC_DESTDIR=/ MAC_INSTALL_VERSION=Current IOS_DESTDIR=/ IOS_INSTALL_VERSION=Current -C msbuild V=$(V)
ifdef ENABLE_XAMARIN
	$(Q) $(MAKE) install-symlinks MAC_DESTDIR=/ MAC_INSTALL_VERSION=Current IOS_DESTDIR=/ IOS_INSTALL_VERSION=Current -C $(MACCORE_PATH) V=$(V)
endif

install-system-ios:
ifdef INCLUDE_IOS
	@if ! test -s "$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/buildinfo"; then echo "The Xamarin.iOS build seems incomplete. Did you run \"make install\"?"; exit 1; fi
	$(Q) rm -f /Library/Frameworks/Xamarin.iOS.framework/Versions/Current
	$(Q) mkdir -p /Library/Frameworks/Xamarin.iOS.framework/Versions
	$(Q) ln -s $(IOS_DESTDIR)$(MONOTOUCH_PREFIX) /Library/Frameworks/Xamarin.iOS.framework/Versions/Current
	$(Q) echo Installed Xamarin.iOS into /Library/Frameworks/Xamarin.iOS.framework/Versions/Current
endif

install-system-mac:
ifdef INCLUDE_MAC
	@if ! test -s "$(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR)/buildinfo" ; then echo "The Xamarin.Mac build seems incomplete. Did you run \"make install\"?"; exit 1; fi
	$(Q) rm -f $(MAC_FRAMEWORK_DIR)/Versions/Current
	$(Q) mkdir -p $(MAC_FRAMEWORK_DIR)/Versions
	$(Q) ln -s $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR) $(MAC_FRAMEWORK_DIR)/Versions/Current
	$(Q) echo Installed Xamarin.Mac into $(MAC_FRAMEWORK_DIR)/Versions/Current
endif

fix-install-permissions:
	sudo mkdir -p /Library/Frameworks/Mono.framework/External/
	sudo mkdir -p /Library/Frameworks/Xamarin.iOS.framework
	sudo mkdir -p /Library/Frameworks/Xamarin.Mac.framework
	sudo chown -R $(USER) /Library/Frameworks/Mono.framework/External/
	sudo chown -R $(USER) /Library/Frameworks/Xamarin.iOS.framework
	sudo chown -R $(USER) /Library/Frameworks/Xamarin.Mac.framework

git-clean-all:
	@echo "Cleaning and resetting all dependencies. This is a destructive operation."
	@echo "You have 5 seconds to cancel (Ctrl-C) if you wish."
	@sleep 5
	@echo "Cleaning xamarin-macios..."
	@git clean -xffdq
	@git submodule foreach -q --recursive 'git clean -xffdq'
	@for dir in $(DEPENDENCY_DIRECTORIES); do if test -d $(CURDIR)/$$dir; then echo "Cleaning $$dir" && cd $(CURDIR)/$$dir && git clean -xffdq && git reset --hard -q && git submodule foreach -q --recursive 'git clean -xffdq'; else echo "Skipped  $$dir (does not exist)"; fi; done
ifdef ENABLE_XAMARIN
	@./configure --enable-xamarin
	$(MAKE) reset
	@echo "Done (Xamarin-specific build has been re-enabled)"
else
	@echo "Done"
endif

ifdef ENABLE_XAMARIN
SUBDIRS += $(MACCORE_PATH)
endif

SUBDIRS += tests
