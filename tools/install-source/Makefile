TOP=../..
SRC_PATH=$(TOP)/src
include $(TOP)/Make.config

ifdef INCLUDE_IOS
install-local:: install-source-ios
endif
ifdef INCLUDE_MAC
install-local:: install-source-mac
endif

IOS_ASSEMBLIES = $(filter-out $(addprefix $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/,MonoTouch.NUnitLite.pdb MonoTouch.Dialog-1.pdb OpenTK-1.0.pdb), $(wildcard $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/*.pdb))
MAC_ASSEMBLIES = $(filter-out $(addprefix $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/,OpenTK.pdb), $(wildcard $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/*.pdb))

install-source-ios: install-source.exe
	@echo "Installing source files for Xamarin.iOS"
	$(Q) $(SYSTEM_MONO) install-source.exe $(IOS_ASSEMBLIES) --link:$(USE_SOURCE_LINKS) --mono-path=$(abspath $(MONO_IOS_SDK_DESTDIR)/ios-sources) --opentk-path=$(abspath $(OPENTK_PATH)/Source) --xamarin-path=$(abspath $(SRC_PATH)) --install-dir=$(IOS_FRAMEWORK_DIR)/Versions/$(IOS_PACKAGE_VERSION) --destination-dir=$(abspath $(IOS_DESTDIR))$(abspath $(MONOTOUCH_PREFIX))

install-source-mac: install-source.exe
	@echo "Installing source files for Xamarin.Mac"
	$(Q) $(SYSTEM_MONO) install-source.exe $(MAC_ASSEMBLIES) --link:$(USE_SOURCE_LINKS) --mono-path=$(abspath $(MONO_IOS_SDK_DESTDIR)/ios-sources) --opentk-path=$(abspath $(OPENTK_PATH)/Source) --xamarin-path=$(abspath $(SRC_PATH)) --install-dir=$(MAC_FRAMEWORK_DIR)/Versions/$(MAC_PACKAGE_VERSION) --destination-dir=$(abspath $(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR))


IOS_SOURCES = \
	IPathMangler.cs \
	MonoPathMangler.cs \
	OpenTKSourceMangler.cs \
	PathManglerFactory.cs \
	Program.cs \
	XamarinSourcesPathMangler.cs


install-source.exe: $(IOS_SOURCES) Makefile install-source.csproj
	$(Q_XBUILD) $(SYSTEM_XBUILD) /restore install-source.csproj $(XBUILD_VERBOSITY)
