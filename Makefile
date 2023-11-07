TOP=.
SUBDIRS=builds runtime fsharp src msbuild tools
include $(TOP)/Make.config
include $(TOP)/mk/versions.mk

ifdef ENABLE_DOTNET
SUBDIRS += dotnet
endif

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
	$(Q) $(MAKE) show-versions

show-versions:
	@echo "Building:"
ifdef INCLUDE_XAMARIN_LEGACY
	@echo "    The legacy package(s):"
ifdef INCLUDE_IOS
	@echo "        Xamarin.iOS $(IOS_PACKAGE_VERSION)"
endif
ifdef INCLUDE_MAC
	@echo "        Xamarin.Mac $(MAC_PACKAGE_VERSION)"
endif
endif
ifdef ENABLE_DOTNET
	@echo "    The .NET NuGet(s):"
	@$(foreach platform,$(DOTNET_PLATFORMS),echo "        Microsoft.$(platform) $($(shell echo $(platform) | tr 'a-z' 'A-Z')_NUGET_VERSION_FULL)";)
endif

check-permissions:
ifdef INCLUDE_MAC
	@UNREADABLE=`find $(MAC_DESTDIR) ! -perm -0644`; if ! test -z "$$UNREADABLE"; then echo "There are files with invalid permissions (all installed files at least be readable by everybody, and writable by owner: 0644): "; find $(MAC_DESTDIR) ! -perm -0644 | xargs ls -la; exit 1; fi
	@echo Validated file permissions for Xamarin.Mac.
endif
ifdef INCLUDE_IOS
	@UNREADABLE=`find $(IOS_DESTDIR) ! -perm -0644`; if ! test -z "$$UNREADABLE"; then echo "There are files with invalid permissions (all installed files at least be readable by everybody, and writable by owner: 0644): "; find $(IOS_DESTDIR) ! -perm -0644 | xargs ls -la; exit 1; fi
	@echo Validated file permissions for Xamarin.iOS.
endif

all-local:: global.json

# This tells NuGet to use the exact same dotnet version we've configured in Make.config
global.json: $(TOP)/dotnet.config Makefile $(GIT_DIRECTORY)/HEAD $(GIT_DIRECTORY)/index
	$(Q_GEN) \
		printf "{\n" > $@; \
		printf "  \"sdk\": {\n    \"version\": \"$(DOTNET_VERSION)\"\n  }\n" >> $@; \
		printf "}\n" >> $@

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

.PHONY: package release
package release:
	$(Q) $(MAKE) -C $(TOP)/release release
	# copy .pkg, .zip and *updateinfo to the packages directory to be uploaded to storage
	$(Q) mkdir -p ../package
	$(Q) echo "Output from 'make release':"
	$(Q) ls -la $(TOP)/release | sed 's/^/    /'
	$(Q) if test -n "$$(shopt -s nullglob; echo $(TOP)/release/*.pkg)"; then $(CP) $(TOP)/release/*.pkg ../package; fi
	$(Q) if test -n "$$(shopt -s nullglob; echo $(TOP)/release/*.zip)"; then $(CP) $(TOP)/release/*.zip ../package; fi
	$(Q) if test -n "$$(shopt -s nullglob; echo $(TOP)/release/*updateinfo)"; then $(CP) $(TOP)/release/*updateinfo ../package; fi
	$(Q) echo "Packages:"
	$(Q) ls -la ../package | sed 's/^/    /'

dotnet-install-system:
	$(Q) $(MAKE) -C dotnet install-system

install-system: install-system-ios install-system-mac
	@# Clean up some old files
	$(Q) rm -Rf /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/iOS
	$(Q) rm -Rf /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Xamarin.ObjcBinding.CSharp.targets
	$(Q) rm -Rf /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Xamarin.Common.CSharp.targets
	$(Q) rm -Rf /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Xamarin.ObjcBinding.Tasks.dll
	$(Q) rm -Rf /Library/Frameworks/Mono.framework/External/xbuild/Xamarin/Mac
	$(Q) $(MAKE) install-symlinks MAC_DESTDIR=/ MAC_INSTALL_VERSION=Current IOS_DESTDIR=/ IOS_INSTALL_VERSION=Current -C msbuild V=$(V)

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

fix-xcode-select:
	sudo xcode-select -s $(XCODE_DEVELOPER_ROOT)

fix-xcode-first-run:
	$(XCODE_DEVELOPER_ROOT)/usr/bin/xcodebuild -runFirstLaunch

install-dotnet:
	@echo "Figuring out package link..."
	@export PKG=$$(make -C builds print-dotnet-pkg-urls); \
	echo "Downloading $$(basename $$PKG)..."; \
	curl -LO "$$PKG"; \
	echo "Installing $$(basename $$PKG)..."; \
	time sudo installer -pkg "$$(basename $$PKG)" -target / -verbose -dumplog

git-clean-all:
	@echo "$(COLOR_RED)Cleaning and resetting all dependencies. This is a destructive operation.$(COLOR_CLEAR)"
	@echo "$(COLOR_RED)You have 5 seconds to cancel (Ctrl-C) if you wish.$(COLOR_CLEAR)"
	@sleep 5
	@echo "Cleaning xamarin-macios..."
	@git clean -xffdq -e external/mono
	@test -d external/mono && echo "Cleaning mono..." && cd external/mono && git clean -xffdq && git submodule foreach -q --recursive 'git clean -xffdq && git reset --hard -q' || true
	@git submodule foreach -q --recursive 'git clean -xffdq && git reset --hard -q'
	@for dir in $(DEPENDENCY_DIRECTORIES); do if test -d $(CURDIR)/$$dir; then echo "Cleaning $$dir" && cd $(CURDIR)/$$dir && git clean -xffdq && git reset --hard -q && git submodule foreach -q --recursive 'git clean -xffdq'; else echo "Skipped  $$dir (does not exist)"; fi; done

	@if [ -n "$(ENABLE_XAMARIN)" ] || [ -n "$(ENABLE_DOTNET)" ]; then \
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

SUBDIRS += tests
