TOP=../..
SRC_PATH=$(TOP)/src
include $(TOP)/Make.config

ifdef INCLUDE_IOS
install-local:: install-source-ios
endif
ifdef INCLUDE_MAC
install-local:: install-source-mac
endif

IOS_ASSEMBLIES = \
	$(wildcard $(MONO_PATH)/mcs/class/lib/monotouch/*.mdb) \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.dll.mdb

MAC_ASSEMBLIES = \
	$(wildcard $(MONO_PATH)/mcs/class/lib/xammac/*.mdb) \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll.mdb

IOS_MDB_FILES = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.dll.mdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.dll.mdb

MAC_MDB_FILES = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile/Xamarin.Mac.dll.mdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/mobile/Xamarin.Mac.dll.mdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.dll.mdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full/Xamarin.Mac.dll.mdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/full/Xamarin.Mac.dll.mdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMac.CFNetwork.dll.mdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.dll.mdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll.mdb

install-source-ios: install-source.exe
	@echo "Installing source files for Xamarin.iOS"
	$(Q) $(SYSTEM_MONO) install-source.exe $(IOS_ASSEMBLIES) --link:$(USE_SOURCE_LINKS) --mono-path=$(abspath $(MONO_PATH)) --opentk-path=$(abspath $(OPENTK_PATH)/Source) --xamarin-path=$(abspath $(SRC_PATH)) --install-dir=$(abspath $(IOS_DESTDIR))$(abspath $(MONOTOUCH_PREFIX))
	@echo "Rebasing the Xamarin.iOS mdb files."
	for mdb_file in $(IOS_MDB_FILES); do \
		$(MDB_REBASE) -q -i $(abspath $(TOP)/src)/ -o $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/src/Xamarin.iOS/ $$mdb_file ; \
		$(MDB_REBASE) -q -i $(abspath $(TOP)/runtime)/ -o $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/src/Xamarin.iOS/runtime/ $$mdb_file ; \
		$(MDB_REBASE) -q -i $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/src/Xamarin.iOS/build/ios/native/ -o $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/src/Xamarin.iOS/ $$mdb_file ; \
		chmod 0644 $$mdb_file ; \
	done

install-source-mac: install-source.exe
	@echo "Installing source files for Xamarin.Mac"
	$(Q) $(SYSTEM_MONO) install-source.exe $(MAC_ASSEMBLIES) --link:$(USE_SOURCE_LINKS) --mono-path=$(abspath $(MONO_PATH)) --opentk-path=$(abspath $(OPENTK_PATH)/Source) --xamarin-path=$(abspath $(SRC_PATH)) --install-dir=$(abspath $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR))
	@echo "Rebasing the Xamarin.Mac mdb files."
	for mdb_file in $(MAC_MDB_FILES); do \
		$(MDB_REBASE) -q -i $(abspath $(TOP)/external/mono)/ -o $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/src/mono/ $$mdb_file ; \
		$(MDB_REBASE) -q -i $(abspath $(TOP)/runtime)/ -o $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/src/Xamarin.Mac/runtime/ $$mdb_file ; \
		$(MDB_REBASE) -q -i $(abspath $(TOP)/src)/ -o $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/src/Xamarin.Mac/ $$mdb_file ; \
		$(MDB_REBASE) -q -i $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/src/Xamarin.Mac/build/mac/full/ -o $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/src/Xamarin.Mac/ $$mdb_file ; \
		$(MDB_REBASE) -q -i $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/src/Xamarin.Mac/build/mac/mobile/ -o $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/src/Xamarin.Mac/ $$mdb_file ; \
		chmod 0644 $$mdb_file ; \
	done


IOS_SOURCES = \
	IPathMangler.cs \
	MonoPathMangler.cs \
	OpenTKSourceMangler.cs \
	PathManglerFactory.cs \
	Program.cs \
	XamarinSourcesPathMangler.cs \
	$(MONO_PATH)/mcs/class/Mono.Options/Mono.Options/Options.cs

install-source.exe: $(IOS_SOURCES) Makefile install-source.csproj
	$(Q_XBUILD) $(SYSTEM_XBUILD) install-source.csproj $(XBUILD_VERBOSITY)
