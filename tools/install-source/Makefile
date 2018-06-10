TOP=../..
SRC_PATH=$(TOP)/src
include $(TOP)/Make.config

ifdef INCLUDE_IOS
install-local:: install-source-ios
endif
ifdef INCLUDE_MAC
install-local:: install-source-mac
endif

CECIL_PATH=$(MONO_PATH)/external/cecil
CECIL_CONFIG=net_4_0_Release
MONO_CECIL_DLL=$(CECIL_PATH)/bin/$(CECIL_CONFIG)/Mono.Cecil.dll
MONO_CECIL_FILES=$(wildcard $(CECIL_PATH)/*.cs) $(wildcard $(CECIL_PATH)/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*/*.cs) $(wildcard $(CECIL_PATH)/*/*/*/*/*.cs)
MONO_CECIL_PDB_FILES=$(wildcard $(CECIL_PATH)/symbols/pdb/*.cs) $(wildcard $(CECIL_PATH)/symbols/pdb/*/*.cs) $(wildcard $(CECIL_PATH)/symbols/pdb/*/*/*.cs) $(wildcard $(CECIL_PATH)/symbols/pdb/*/*/*/*.cs) $(wildcard $(CECIL_PATH)/symbols/pdb/*/*/*/*/*.cs)

Mono.Cecil.Pdb.dll: $(MONO_CECIL_PDB_FILES)
	$(Q_XBUILD) cd $(CECIL_PATH)/symbols/pdb/ && $(SYSTEM_XBUILD) /p:Configuration=$(CECIL_CONFIG) Mono.Cecil.Pdb.csproj $(XBUILD_VERBOSITY)
	$(Q) cp $(CECIL_PATH)/bin/$(CECIL_CONFIG)/*.dll $(dir $@)

$(MONO_CECIL_DLL): $(CECIL_PATH)/Mono.Cecil.csproj $(MONO_CECIL_FILES)
	$(Q_XBUILD) cd $(CECIL_PATH) && $(SYSTEM_XBUILD) /p:Configuration=$(CECIL_CONFIG) Mono.Cecil.csproj $(XBUILD_VERBOSITY)

Mono.Cecil.dll: $(MONO_CECIL_DLL)
	$(Q) cp $<* $(dir $@)

IOS_ASSEMBLIES = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/I18N.CJK.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/I18N.MidEast.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/I18N.Other.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/I18N.Rare.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/I18N.West.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/I18N.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Microsoft.CSharp.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Mono.CSharp.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Mono.Data.Sqlite.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Mono.Data.Tds.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Mono.Security.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/Mono.Simd.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.ComponentModel.Composition.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.ComponentModel.DataAnnotations.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Core.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Data.Services.Client.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Data.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.IO.Compression.FileSystem.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.IO.Compression.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.IdentityModel.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Json.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Net.Http.WinHttpHandler.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Net.Http.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Net.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Numerics.Vectors.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Numerics.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Reflection.Context.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Runtime.Serialization.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Security.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.ServiceModel.Internals.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.ServiceModel.Web.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.ServiceModel.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Transactions.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Web.Services.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Xml.Linq.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.Xml.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/System.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/mscorlib.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.pdb \

MAC_ASSEMBLIES = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/I18N.CJK.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/I18N.MidEast.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/I18N.Other.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/I18N.Rare.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/I18N.West.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/I18N.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Microsoft.CSharp.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Mono.CSharp.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Mono.CompilerServices.SymbolWriter.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Mono.Data.Sqlite.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Mono.Data.Tds.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Mono.Security.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.ComponentModel.Composition.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.ComponentModel.DataAnnotations.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Core.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Data.Services.Client.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Data.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.IO.Compression.FileSystem.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.IO.Compression.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.IdentityModel.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Json.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Net.Http.WinHttpHandler.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Net.Http.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Net.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Numerics.Vectors.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Numerics.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Reflection.Context.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Runtime.Serialization.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Security.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.ServiceModel.Internals.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.ServiceModel.Web.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.ServiceModel.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Transactions.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Web.Services.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Xml.Linq.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.Xml.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/System.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/mscorlib.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.pdb

IOS_MDB_FILES = \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.dll \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/32bits/Xamarin.iOS.dll

MAC_MDB_FILES = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/mobile/Xamarin.Mac.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/mobile/Xamarin.Mac.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/reference/full/Xamarin.Mac.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/x86_64/full/Xamarin.Mac.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/i386/full/Xamarin.Mac.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/XamMac.CFNetwork.dll.mdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Xamarin.Mac.pdb \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.pdb

install-source-ios: install-source.exe
	@echo "Installing source files for Xamarin.iOS"
	$(Q) $(SYSTEM_MONO) install-source.exe $(IOS_ASSEMBLIES) --link:$(USE_SOURCE_LINKS) --mono-path=$(abspath $(MONO_PATH)) --opentk-path=$(abspath $(OPENTK_PATH)/Source) --xamarin-path=$(abspath $(SRC_PATH)) --install-dir=$(IOS_FRAMEWORK_DIR)/Versions/$(IOS_PACKAGE_VERSION) --destination-dir=$(abspath $(IOS_DESTDIR))$(abspath $(MONOTOUCH_PREFIX))

install-source-mac: install-source.exe
	@echo "Installing source files for Xamarin.Mac"
	$(Q) $(SYSTEM_MONO) install-source.exe $(MAC_ASSEMBLIES) --link:$(USE_SOURCE_LINKS) --mono-path=$(abspath $(MONO_PATH)) --opentk-path=$(abspath $(OPENTK_PATH)/Source) --xamarin-path=$(abspath $(SRC_PATH)) --install-dir=$(MAC_FRAMEWORK_DIR)/Versions/$(MAC_PACKAGE_VERSION) --destination-dir=$(abspath $(MAC_DESTDIR)/$(MAC_FRAMEWORK_CURRENT_DIR))


IOS_SOURCES = \
	IPathMangler.cs \
	MonoPathMangler.cs \
	OpenTKSourceMangler.cs \
	PathManglerFactory.cs \
	Program.cs \
	XamarinSourcesPathMangler.cs \
	$(MONO_PATH)/mcs/class/Mono.Options/Mono.Options/Options.cs


install-source.exe: Mono.Cecil.dll Mono.Cecil.Pdb.dll $(IOS_SOURCES) Makefile install-source.csproj $(MONO_CECIL_DLL)
	$(Q_XBUILD) $(SYSTEM_XBUILD) install-source.csproj $(XBUILD_VERBOSITY)
