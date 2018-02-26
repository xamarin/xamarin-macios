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
	$(MONO_PATH)/mcs/class/lib/monotouch/I18N.CJK.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/I18N.MidEast.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/I18N.Other.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/I18N.Rare.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/I18N.West.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/I18N.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/Microsoft.CSharp.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/Mono.CSharp.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/Mono.Data.Sqlite.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/Mono.Data.Tds.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/Mono.Security.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/Mono.Simd.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.ComponentModel.Composition.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.ComponentModel.DataAnnotations.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Core.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Data.Services.Client.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Data.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.IO.Compression.FileSystem.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.IO.Compression.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.IdentityModel.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Json.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Net.Http.WinHttpHandler.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Net.Http.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Net.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Numerics.Vectors.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Numerics.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Reflection.Context.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Runtime.Serialization.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Security.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.ServiceModel.Internals.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.ServiceModel.Web.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.ServiceModel.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Transactions.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Web.Services.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Xml.Linq.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.Xml.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/System.pdb \
	$(MONO_PATH)/mcs/class/lib/monotouch/mscorlib.pdb \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/64bits/Xamarin.iOS.pdb

MAC_ASSEMBLIES = \
	$(MONO_PATH)/mcs/class/lib/xammac/I18N.CJK.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/I18N.MidEast.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/I18N.Other.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/I18N.Rare.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/I18N.West.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/I18N.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/Microsoft.CSharp.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/Mono.CSharp.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/Mono.CompilerServices.SymbolWriter.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/Mono.Data.Sqlite.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/Mono.Data.Tds.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/Mono.Security.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.ComponentModel.Composition.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.ComponentModel.DataAnnotations.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Core.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Data.Services.Client.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Data.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.IO.Compression.FileSystem.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.IO.Compression.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.IdentityModel.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Json.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Net.Http.WinHttpHandler.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Net.Http.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Net.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Numerics.Vectors.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Numerics.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Reflection.Context.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Runtime.Serialization.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Security.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.ServiceModel.Internals.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.ServiceModel.Web.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.ServiceModel.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Transactions.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Web.Services.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Xml.Linq.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.Xml.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/System.pdb \
	$(MONO_PATH)/mcs/class/lib/xammac/mscorlib.pdb \
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
