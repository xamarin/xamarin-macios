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

install-source-ios: install-source.exe
	@echo "Installing source files for Xamarin.iOS"
	$(Q) $(SYSTEM_MONO) install-source.exe $(IOS_ASSEMBLIES) --link:$(USE_SOURCE_LINKS) --mono-path=$(abspath $(MONO_PATH)) --opentk-path=$(abspath $(OPENTK_PATH)/Source) --xamarin-path=$(abspath $(SRC_PATH)) --install-dir=$(abspath $(IOS_DESTDIR))/$(abspath $(MONOTOUCH_PREFIX))
	@echo "Rebasing the Xamarin.iOS mdb files."
	$(Q_MDB) $(MDB_REBASE) -q -i $(abspath $(TOP)/src)/ -o $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/src/Xamarin.iOS/ $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.dll.mdb
	$(Q_MDB) $(MDB_REBASE) -q -i $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/src/Xamarin.iOS/build/ios/native/ -o $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/src/Xamarin.iOS/ $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.dll.mdb
	$(Q) chmod 0644 $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.dll.mdb
	$(Q_MDB) $(MDB_REBASE) -q -i $(abspath $(TOP)/src)/ -o $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/src/Xamarin.iOS/ $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.dll.mdb
	$(Q_MDB) $(MDB_REBASE) -q -i $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/src/Xamarin.iOS/build/ios/native/ -o $(IOS_TARGETDIR)$(MONOTOUCH_PREFIX)/src/Xamarin.iOS/ $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.dll.mdb
	$(Q) chmod 0644 $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.dll.mdb

install-source-mac: install-source.exe
	@echo "Installing source files for Xamarin.Mac"
	$(Q) $(SYSTEM_MONO) install-source.exe $(MAC_ASSEMBLIES) --link:$(USE_SOURCE_LINKS) --mono-path=$(abspath $(MONO_PATH)) --opentk-path=$(abspath $(OPENTK_PATH)/Source) --xamarin-path=$(abspath $(SRC_PATH)) --install-dir=$(abspath $(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR))
	@echo "Rebasing the Xamarin.Mac mdb files."
	@echo "Path is $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/src/mono/build/mac/full/"
	$(Q_MDB) $(MDB_REBASE) -q -i $(abspath $(TOP)/src)/ -o $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/src/Xamarin.Mac/ $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll.mdb
	$(Q_MDB) $(MDB_REBASE) -q -i $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/src/Xamarin.Mac/build/mac/full/ -o $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/src/Xamarin.Mac/ $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll.mdb
	$(Q) chmod 0644 $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll.mdb


IOS_SOURCES = \
	Program.cs \
	$(MONO_PATH)/mcs/class/Mono.Options/Mono.Options/Options.cs

install-source.exe: $(IOS_SOURCES) Makefile install-source.csproj
	$(Q_XBUILD) $(SYSTEM_XBUILD) install-source.csproj $(XBUILD_VERBOSITY)
