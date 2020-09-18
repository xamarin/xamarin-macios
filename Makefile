TOP=.
SUBDIRS=builds runtime fsharp src msbuild tools dotnet
include $(TOP)/Make.config
include $(TOP)/mk/versions.mk

#
# Common
#

all-local:: check-system
install-local::

.PHONY: world
world: check-system
	@$(MAKE) reset-versions
	@$(MAKE) all -j8
	@$(MAKE) install -j8

.PHONY: check-system
check-system:
ifdef INCLUDE_MAC
ifdef INCLUDE_IOS
	@if [[ "x$(IOS_COMMIT_DISTANCE)" != "x$(MAC_COMMIT_DISTANCE)" ]]; then \
		echo "$(COLOR_RED)*** The commit distance for Xamarin.iOS ($(IOS_COMMIT_DISTANCE)) and Xamarin.Mac ($(MAC_COMMIT_DISTANCE)) are different.$(COLOR_CLEAR)"; \
		echo "$(COLOR_RED)*** To fix this problem, bump the revision (the third number) for both $(COLOR_GRAY)IOS_PACKAGE_NUMBER$(COLOR_RED) and $(COLOR_GRAY)MAC_PACKAGE_NUMBER$(COLOR_RED) in Make.versions.$(COLOR_CLEAR)"; \
		echo "$(COLOR_RED)*** Once fixed, you need to commit the changes for them to pass this check.$(COLOR_CLEAR)"; \
		exit 1; \
	elif (( $(IOS_COMMIT_DISTANCE) > 999 || $(MAC_COMMIT_DISTANCE) > 999 )); then \
		echo "$(COLOR_RED)*** The commit distance for Xamarin.iOS ($(IOS_COMMIT_DISTANCE)) and/or Xamarin.Mac ($(MAC_COMMIT_DISTANCE)) are > 999.$(COLOR_CLEAR)"; \
		echo "$(COLOR_RED)*** To fix this problem, bump the revision (the third number) for both $(COLOR_GRAY)IOS_PACKAGE_NUMBER$(COLOR_RED) and $(COLOR_GRAY)MAC_PACKAGE_NUMBER$(COLOR_RED) in Make.versions.$(COLOR_CLEAR)"; \
		echo "$(COLOR_RED)*** Once fixed, you need to commit the changes for them to pass this check.$(COLOR_CLEAR)"; \
		exit 1; \
	fi
endif
endif
	@./system-dependencies.sh
	@echo "Building the packages:"
	@echo "    Xamarin.iOS $(IOS_PACKAGE_VERSION)"
	@echo "    Xamarin.Mac $(MAC_PACKAGE_VERSION)"
	@echo "and the NuGets:"
	@echo "    Xamarin.iOS $(IOS_NUGET_VERSION_FULL)"
	@echo "    Xamarin.tvOS $(TVOS_NUGET_VERSION_FULL)"
	@echo "    Xamarin.watchOS $(WATCHOS_NUGET_VERSION_FULL)"
	@echo "    Xamarin.macOS $(MACOS_NUGET_VERSION_FULL)"

check-permissions:
ifdef INCLUDE_MAC
	@UNREADABLE=`find $(MAC_DESTDIR) ! -perm -0644`; if ! test -z "$$UNREADABLE"; then echo "There are files with invalid permissions (all installed files at least be readable by everybody, and writable by owner: 0644): "; find $(MAC_DESTDIR) ! -perm -0644 | xargs ls -la; exit 1; fi
	@echo Validated file permissions for Xamarin.Mac.
endif
ifdef INCLUDE_IOS
	@UNREADABLE=`find $(IOS_DESTDIR) ! -perm -0644`; if ! test -z "$$UNREADABLE"; then echo "There are files with invalid permissions (all installed files at least be readable by everybody, and writable by owner: 0644): "; find $(IOS_DESTDIR) ! -perm -0644 | xargs ls -la; exit 1; fi
	@echo Validated file permissions for Xamarin.iOS.
endif

all-local:: global.json global6.json
global.json: Make.config Makefile
	$(Q) printf "{\n\t\"sdk\": {\n\t\t\"version\": \"$(DOTNET_VERSION)\"\n\t}\n}\n" > $@

# This tells NuGet to use the exact same dotnet version we've configured in Make.config
global6.json: $(TOP)/Make.config.inc Makefile $(TOP)/.git/HEAD $(TOP)/.git/index
	$(Q_GEN) \
		printf "{\n" > $@; \
		printf "\t\"sdk\": { \"version\": \"$(DOTNET6_VERSION)\" },\n" >> $@; \
		printf "\t\"msbuild-sdks\": {\n" >> $@; \
		printf "\t\t\"Microsoft.DotNet.Build.Tasks.SharedFramework.Sdk\": \"5.0.0-beta.20120.1\"\n" >> $@; \
		printf "\t}\n}\n" >> $@

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
	$(CP) $(MACCORE_PATH)/release/*.pkg ../package
	$(CP) $(MACCORE_PATH)/release/*.zip ../package
	$(CP) $(MACCORE_PATH)/release/*updateinfo ../package

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
	@echo "$(COLOR_RED)Cleaning and resetting all dependencies. This is a destructive operation.$(COLOR_CLEAR)"
	@echo "$(COLOR_RED)You have 5 seconds to cancel (Ctrl-C) if you wish.$(COLOR_CLEAR)"
	@sleep 5
	@echo "Cleaning xamarin-macios..."
	@git clean -xffdq -e external/mono
	@test -d external/mono && echo "Cleaning mono..." && cd external/mono && git clean -xffdq && git submodule foreach -q --recursive 'git clean -xffdq && git reset --hard -q' || true
	@git submodule foreach -q --recursive 'git clean -xffdq && git reset --hard -q'
	@for dir in $(DEPENDENCY_DIRECTORIES); do if test -d $(CURDIR)/$$dir; then echo "Cleaning $$dir" && cd $(CURDIR)/$$dir && git clean -xffdq && git reset --hard -q && git submodule foreach -q --recursive 'git clean -xffdq'; else echo "Skipped  $$dir (does not exist)"; fi; done

	@if [ -n "$(ENABLE_XAMARIN)" ] || [ -n "$(ENABLE_DOTNET)"]; then \
		CONFIGURE_FLAGS=""; \
		if [ -n "$(ENABLE_XAMARIN)" ]; then \
			echo "Xamarin-specific build has been re-enabled"; \
			CONFIGURE_FLAGS="$$CONFIGURE_FLAGS --enable-xamarin"; \
		fi; \
		if [ -n "$(ENABLE_DOTNET)" ]; then \
			echo "Dotnet-specific build has been re-enabled"; \
			CONFIGURE_FLAGS="$$CONFIGURE_FLAGS --enable-dotnet"; \
		fi; \
		./configure "$$CONFIGURE_FLAGS"; \
		$(MAKE) reset; \
		echo "Done"; \
	else \
		echo "Done"; \
	fi; \

ifdef ENABLE_XAMARIN
SUBDIRS += $(MACCORE_PATH)
endif

SUBDIRS += tests
