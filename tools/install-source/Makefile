TOP=../..
include $(TOP)/Make.config

ifdef INCLUDE_IOS
install-local:: install-source-ios
endif
ifdef INCLUDE_MAC
install-local:: install-source-mac
endif

IOS_ASSEMBLIES = \
	$(wildcard $(MONO_PATH)/mcs/class/lib/monotouch/*.mdb)

MAC_ASSEMBLIES = \
	$(wildcard $(MONO_PATH)/mcs/class/lib/xammac/*.mdb)

install-source-ios: install-source.exe
	@echo "Installing source files for Xamarin.iOS"
	$(Q) $(SYSTEM_MONO) install-source.exe $(IOS_ASSEMBLIES) --link:$(USE_SOURCE_LINKS) --mono-path=$(abspath $(MONO_PATH)) --install-dir=$(abspath $(IOS_DESTDIR))/$(abspath $(MONOTOUCH_PREFIX))

install-source-mac: install-source.exe
	@echo "Installing source files for Xamarin.Mac"
	$(Q) $(SYSTEM_MONO) install-source.exe $(MAC_ASSEMBLIES) --link:$(USE_SOURCE_LINKS) --mono-path=$(abspath $(MONO_PATH)) --install-dir=$(abspath $(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR))

IOS_SOURCES = \
	Program.cs \
	$(MONO_PATH)/mcs/class/Mono.Options/Mono.Options/Options.cs

install-source.exe: $(IOS_SOURCES) Makefile install-source.csproj
	$(Q_XBUILD) $(SYSTEM_XBUILD) install-source.csproj $(XBUILD_VERBOSITY)
