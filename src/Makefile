TOP=..
include $(TOP)/Make.config

BUILD_DIR=build
RSP_DIR=rsp
DOTNET_BUILD_DIR=$(BUILD_DIR)/dotnet
PROJECT_DIR=.

include $(TOP)/src/frameworks.sources
include $(TOP)/mk/rules.mk

MACOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/macos
IOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/ios
TVOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/tvos
MACCATALYST_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/maccatalyst

GENERATOR_FLAGS=-process-enums -core -nologo -nostdlib -native-exception-marshalling --ns:ObjCRuntime

GENERATOR_TF_VERSION=$(subst net,,$(DOTNET_TFM))

DOTNET_REFERENCES = \
	/r:$(DOTNET_BCL_DIR)/System.Buffers.dll \
	/r:$(DOTNET_BCL_DIR)/System.Collections.Concurrent.dll \
	/r:$(DOTNET_BCL_DIR)/System.Collections.dll \
	/r:$(DOTNET_BCL_DIR)/System.Collections.NonGeneric.dll \
	/r:$(DOTNET_BCL_DIR)/System.Console.dll \
	/r:$(DOTNET_BCL_DIR)/System.Diagnostics.Debug.dll \
	/r:$(DOTNET_BCL_DIR)/System.Diagnostics.Tools.dll \
	/r:$(DOTNET_BCL_DIR)/System.Diagnostics.StackTrace.dll \
	/r:$(DOTNET_BCL_DIR)/System.Drawing.Primitives.dll \
	/r:$(DOTNET_BCL_DIR)/System.IO.Compression.dll \
	/r:$(DOTNET_BCL_DIR)/System.IO.FileSystem.dll \
	/r:$(DOTNET_BCL_DIR)/System.Linq.dll \
	/r:$(DOTNET_BCL_DIR)/System.Memory.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.Http.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.NameResolution.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.Primitives.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.Requests.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.Security.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.ServicePoint.dll \
	/r:$(DOTNET_BCL_DIR)/System.Net.Sockets.dll \
	/r:$(DOTNET_BCL_DIR)/System.Numerics.Vectors.dll \
	/r:$(DOTNET_BCL_DIR)/System.Resources.ResourceManager.dll \
	/r:$(DOTNET_BCL_DIR)/System.Runtime.dll \
	/r:$(DOTNET_BCL_DIR)/System.Runtime.CompilerServices.Unsafe.dll \
	/r:$(DOTNET_BCL_DIR)/System.Runtime.Extensions.dll \
	/r:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll \
	/r:$(DOTNET_BCL_DIR)/System.Runtime.Loader.dll \
	/r:$(DOTNET_BCL_DIR)/System.Security.Cryptography.dll \
	/r:$(DOTNET_BCL_DIR)/System.Security.Cryptography.X509Certificates.dll \
	/r:$(DOTNET_BCL_DIR)/System.Text.RegularExpressions.dll \
	/r:$(DOTNET_BCL_DIR)/System.Threading.dll \
	/r:$(DOTNET_BCL_DIR)/System.Threading.Tasks.dll \
	/r:$(DOTNET_BCL_DIR)/System.Threading.Thread.dll \
	/r:$(DOTNET_BCL_DIR)/System.Xml.dll \
	/r:$(DOTNET_BCL_DIR)/System.Xml.ReaderWriter.dll \

DOTNET_OR_GREATER_DEFINES:=$(foreach version,$(shell seq 6 $(firstword $(subst ., ,$(subst net,,$(DOTNET_TFM))))),/define:NET$(version)_0_OR_GREATER)
DOTNET_FLAGS=/warnaserror+ /nostdlib+ /deterministic /features:strict /nologo /target:library /debug /unsafe /define:NET /define:NET_TODO $(DOTNET_OR_GREATER_DEFINES) $(DOTNET_REFERENCES)

ifeq ($(XCODE_IS_STABLE),true)
DOTNET_FLAGS+=/define:XCODE_IS_STABLE
endif

DOTNET_COMPILER=$(DOTNET_BUILD_DIR)/compiler
DOTNET_GENERATOR_FLAGS=$(GENERATOR_FLAGS) -compiler=$(abspath $(DOTNET_COMPILER)) --lib=$(DOTNET_BCL_DIR) -attributelib:$(DOTNET_BINDING_ATTRIBUTES) $(DOTNET_REFERENCES)
DOTNET_GENERATOR=$(DOTNET_BUILD_DIR)/bgen/bgen
DOTNET_BINDING_ATTRIBUTES=$(DOTNET_BUILD_DIR)/Xamarin.Apple.BindingAttributes.dll

#
# Specific warnings that we want reported as errors by generator
# It takes a comma separated list, but an empty list means
# reporting all warnings as errors.
#
# https://github.com/dotnet/roslyn/issues/41605
NULLABILITY_WARNINGS=nullable
GENERATOR_WARNASERROR=-warnaserror:
IOS_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
TVOS_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
MACOS_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)
MACCATALYST_GENERATOR_WARNASERROR=$(GENERATOR_WARNASERROR)

# this is due to deprecated APIs used in the generated code.
BGEN_WARNINGS_TO_FIX=BI1234

#
# Specific warnings we're ignoring from the C# compiler
# We should look into each of these and see if they're really needed,
# and if so, document why.
#
CSC_WARNINGS_TO_FIX=108,219,618,114,414,1635,3021,4014

# warning CS1591: Missing XML comment for publicly visible type or member
# We (will) post-process xml documentation to add missing members, so
# we can ignore this warning.
CSC_WARNINGS_TO_FIX:=$(CSC_WARNINGS_TO_FIX),1591

WARNINGS_TO_FIX = -nowarn:$(CSC_WARNINGS_TO_FIX)
CORE_WARNINGS_TO_FIX = -nowarn:$(CSC_WARNINGS_TO_FIX),$(BGEN_WARNINGS_TO_FIX)

DOTNET_WARNINGS_TO_FIX = -nowarn:$(CSC_WARNINGS_TO_FIX)
DOTNET_CORE_WARNINGS_TO_FIX = -nowarn:$(CSC_WARNINGS_TO_FIX),$(BGEN_WARNINGS_TO_FIX)

include ./Makefile.generator
include ./Makefile.rgenerator
include ./generator-diff.mk

SHARED_RESX = $(TOP)/tools/mtouch/Errors.resx
SHARED_DESIGNER_CS = $(DOTNET_BUILD_DIR)/common/Errors.Designer.cs

include $(TOP)/scripts/generate-errors/fragment.mk
$(SHARED_DESIGNER_CS): $(SHARED_RESX) $(GENERATE_ERRORS) | $(DOTNET_BUILD_DIR)/common
	$(Q_GEN) $(GENERATE_ERRORS_EXEC) $(SHARED_RESX) $(SHARED_DESIGNER_CS)

DOTNET_TARGETS_DIRS += $(DOTNET_BUILD_DIR)/common

#
# iOS
#

# Add new bindings + source files in frameworks.sources, not here.

IOS_DOTNET_EXTRA_SOURCES = \
	$(IOS_DOTNET_BUILD_DIR)/Constants.cs    \
	$(IOS_DOTNET_BUILD_DIR)/Constants.generated.cs \
	$(IOS_DOTNET_BUILD_DIR)/AssemblyInfo.cs \
	$(SHARED_DESIGNER_CS) \

IOS_DOTNET_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \

IOS_DOTNET_CORE_SOURCES += $(IOS_DOTNET_EXTRA_SOURCES)

IOS_DOTNET_SOURCES += $(IOS_DOTNET_EXTRA_SOURCES) $(IOS_DOTNET_HTTP_SOURCES)

IOS_GENERATOR_DEFINES = -d:IOS
IOS_GENERATOR_FLAGS = -inline-selectors -process-enums $(IOS_GENERATOR_WARNASERROR) $(IOS_GENERATOR_DEFINES)
IOS_DEFINES = -define:IPHONE -define:IOS -define:MONOTOUCH -d:__IOS__ -d:SYSTEM_NET_HTTP

IOS_CORE_DEFINES=-define:COREBUILD $(IOS_DEFINES)

$(IOS_DOTNET_BUILD_DIR)/Constants.cs: Constants.iOS.cs.in Makefile $(TOP)/Make.config.inc | $(IOS_DOTNET_BUILD_DIR)
	$(call Q_PROF_GEN,ios) sed \
		-e "s/@VERSION@/$(IOS_NUGET_VERSION_MAJOR).$(IOS_NUGET_VERSION_MINOR).$(IOS_NUGET_VERSION_PATCH)/g" \
		-e 's/@REVISION@/$(IOS_NUGET_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h))/g' \
		-e "s/@IOS_SDK_VERSION@/$(IOS_SDK_VERSION)/g" \
		$< > $@

$(IOS_DOTNET_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in | $(IOS_DOTNET_BUILD_DIR)
	$(call Q_PROF_GEN,ios) sed \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g' \
		-e 's|@NUGET_VERSION_NO_METADATA@|$(IOS_NUGET_VERSION_NO_METADATA)|g' \
		-e 's|@NUGET_VERSION_MAJOR@|$(IOS_NUGET_VERSION_MAJOR)|g' \
		-e 's|@NUGET_VERSION_MINOR@|$(IOS_NUGET_VERSION_MINOR)|g' \
		-e 's|@NUGET_VERSION_REV@|$(IOS_NUGET_VERSION_PATCH)|g' \
		-e 's|@NUGET_VERSION_BUILD@|$(IOS_NUGET_COMMIT_DISTANCE)|g' \
		-e 's|@DOTNET_PLATFORM@|iOS|g' \
		-e 's|@XCODE_VERSION@|$(subst .,_,$(XCODE_VERSION))|g' \
		< $< > $@.tmp
	$(Q) diff $@ $@.tmp >/dev/null 2>&1 || mv -f $@.tmp $@
	$(Q) rm -f $@.tmp
	$(Q) touch $@

#
# macOS
#

MAC_COMMON_DEFINES = -define:MONOMAC -d:__MACOS__
MACOS_GENERATOR_DEFINES = -d:MONOMAC -d:__MACOS__
MACOS_GENERATOR_FLAGS = $(MACOS_GENERATOR_DEFINES)
MACOS_CORE_DEFINES = $(MAC_COMMON_DEFINES),COREBUILD
MACOS_DEFINES = $(MAC_COMMON_DEFINES) -D:XAMARIN_MODERN

SN_KEY = $(PRODUCT_KEY_PATH)

MAC_DOTNET_EXTRA_CORE_SOURCES += \
	$(MACOS_DOTNET_BUILD_DIR)/Constants.cs \
	$(MACOS_DOTNET_BUILD_DIR)/Constants.generated.cs \

# Add new bindings + source files in frameworks.sources, not here.

MACOS_DOTNET_CORE_SOURCES += \
	$(MAC_DOTNET_EXTRA_CORE_SOURCES) \

MACOS_DOTNET_SOURCES += \
	$(MAC_DOTNET_EXTRA_CORE_SOURCES) \
	$(MACOS_DOTNET_BUILD_DIR)/AssemblyInfo.cs \
	$(SHARED_DESIGNER_CS) \

MAC_CFNETWORK_SOURCES = \
	CFNetwork/Content.cs \
	CFNetwork/MessageHandler.cs \
	CFNetwork/WebRequestStream.cs \
	CFNetwork/WebResponseStream.cs \
	CFNetwork/WorkerThread.cs

MAC_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \

MACOS_DOTNET_SOURCES += \
	$(MAC_CFNETWORK_SOURCES) \
	$(MAC_HTTP_SOURCES) \

$(MACOS_DOTNET_BUILD_DIR)/Constants.cs: Constants.mac.cs.in Makefile $(TOP)/Make.config.inc | $(MACOS_DOTNET_BUILD_DIR)
	$(Q) sed \
			-e "s/@VERSION@/$(MACOS_NUGET_VERSION_MAJOR).$(MACOS_NUGET_VERSION_MINOR).$(MACOS_NUGET_VERSION_PATCH)/g" \
			-e 's/@REVISION@/$(MACOS_NUGET_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h))/g' \
			-e "s/@MACOS_SDK_VERSION@/$(MACOS_SDK_VERSION)/g" \
			-e "s/@MIN_XM_MONO_VERSION@/$(MIN_XM_MONO_VERSION)/g" \
		$< > $@

$(MACOS_DOTNET_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in | $(MACOS_DOTNET_BUILD_DIR)
	$(call Q_PROF_GEN,mac) sed \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@NUGET_VERSION_NO_METADATA@|$(MACOS_NUGET_VERSION_NO_METADATA)|g' \
		-e 's|@NUGET_VERSION_MAJOR@|$(MACOS_NUGET_VERSION_MAJOR)|g' \
		-e 's|@NUGET_VERSION_MINOR@|$(MACOS_NUGET_VERSION_MINOR)|g' \
		-e 's|@NUGET_VERSION_REV@|$(MACOS_NUGET_VERSION_PATCH)|g' \
		-e 's|@NUGET_VERSION_BUILD@|$(MACOS_NUGET_COMMIT_DISTANCE)|g' \
		-e 's|@DOTNET_PLATFORM@|macOS|g' \
		-e 's|@XCODE_VERSION@|$(subst .,_,$(XCODE_VERSION))|g' \
		< $< > $@.tmp
	$(Q) diff $@ $@.tmp >/dev/null 2>&1 || mv -f $@.tmp $@
	$(Q) rm -f $@.tmp
	$(Q) touch $@

#
# tvOS
#

TVOS_DEFINES = -define:IPHONE -define:MONOTOUCH -d:TVOS -d:__TVOS__ -d:SYSTEM_NET_HTTP

TVOS_CORE_DEFINES=$(TVOS_DEFINES) -d:COREBUILD
TVOS_GENERATOR_DEFINES = -d:TVOS
TVOS_GENERATOR_FLAGS = $(TVOS_GENERATOR_DEFINES) -inline-selectors

TVOS_DOTNET_EXTRA_CORE_SOURCES = \
	$(TVOS_DOTNET_BUILD_DIR)/Constants.cs    \
	$(TVOS_DOTNET_BUILD_DIR)/Constants.generated.cs \
	$(TVOS_DOTNET_BUILD_DIR)/AssemblyInfo.cs \

TVOS_DOTNET_CORE_SOURCES += \
	$(TVOS_DOTNET_EXTRA_CORE_SOURCES) \

TVOS_DOTNET_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \
	$(SHARED_DESIGNER_CS) \

TVOS_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \
	$(SHARED_DESIGNER_CS) \

TVOS_DOTNET_SOURCES += \
	$(TVOS_DOTNET_EXTRA_CORE_SOURCES) \
	$(TVOS_HTTP_SOURCES)     \

$(TVOS_DOTNET_BUILD_DIR)/Constants.cs: $(TOP)/src/Constants.tvos.cs.in Makefile $(TOP)/Make.config.inc | $(TVOS_DOTNET_BUILD_DIR)
	$(call Q_PROF_GEN,tvos) sed \
		-e "s/@VERSION@/$(TVOS_NUGET_VERSION_MAJOR).$(TVOS_NUGET_VERSION_MINOR).$(TVOS_NUGET_VERSION_PATCH)/g" \
		-e 's/@REVISION@/$(TVOS_NUGET_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h))/g' \
		-e "s/@TVOS_SDK_VERSION@/$(TVOS_SDK_VERSION)/g" \
		$< > $@

$(TVOS_DOTNET_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in $(TOP)/Make.config  $(GIT_DIRECTORY)/HEAD | $(TVOS_DOTNET_BUILD_DIR)
	$(call Q_PROF_GEN,tvos) sed \
		-e 's|@PRODUCT_NAME@|Xamarin.TVOS|g' \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g' \
		-e 's|@NUGET_VERSION_NO_METADATA@|$(TVOS_NUGET_VERSION_NO_METADATA)|g' \
		-e 's|@NUGET_VERSION_MAJOR@|$(TVOS_NUGET_VERSION_MAJOR)|g' \
		-e 's|@NUGET_VERSION_MINOR@|$(TVOS_NUGET_VERSION_MINOR)|g' \
		-e 's|@NUGET_VERSION_REV@|$(TVOS_NUGET_VERSION_PATCH)|g' \
		-e 's|@NUGET_VERSION_BUILD@|$(TVOS_NUGET_COMMIT_DISTANCE)|g' \
		-e 's|@DOTNET_PLATFORM@|tvOS|g' \
		-e 's|@XCODE_VERSION@|$(subst .,_,$(XCODE_VERSION))|g' \
		$< > $@.tmp
	$(Q) diff $@ $@.tmp >/dev/null 2>&1 || mv -f $@.tmp $@
	$(Q) rm -f $@.tmp
	$(Q) touch $@

#
# Mac Catalyst
#

# MacCatalyst is a variant of iOS, so it defines the iOS variables as well.
MACCATALYST_DEFINES = -define:IPHONE -define:IOS -define:MONOTOUCH -d:__IOS__ -d:__MACCATALYST__ -d:SYSTEM_NET_HTTP

MACCATALYST_CORE_DEFINES=$(MACCATALYST_DEFINES) -d:COREBUILD
MACCATALYST_GENERATOR_DEFINES = -d:__MACCATALYST__ -d:IOS
MACCATALYST_GENERATOR_FLAGS = $(MACCATALYST_GENERATOR_DEFINES) -inline-selectors

MACCATALYST_DOTNET_EXTRA_CORE_SOURCES = \
	$(MACCATALYST_DOTNET_BUILD_DIR)/Constants.cs    \
	$(MACCATALYST_DOTNET_BUILD_DIR)/Constants.generated.cs \
	$(MACCATALYST_DOTNET_BUILD_DIR)/AssemblyInfo.cs \

MACCATALYST_DOTNET_CORE_SOURCES += \
	$(MACCATALYST_DOTNET_EXTRA_CORE_SOURCES) \

MACCATALYST_DOTNET_HTTP_SOURCES = \
	Foundation/NSUrlSessionHandler.cs \
	System.Net.Http/CFContentStream.cs \
	System.Net.Http/CFNetworkHandler.cs \
	$(SHARED_DESIGNER_CS) \

MACCATALYST_DOTNET_SOURCES += \
	$(MACCATALYST_DOTNET_EXTRA_CORE_SOURCES) \
	$(MACCATALYST_DOTNET_HTTP_SOURCES) \

$(MACCATALYST_DOTNET_BUILD_DIR)/Constants.cs: $(TOP)/src/Constants.maccatalyst.cs.in Makefile $(TOP)/Make.config.inc | $(MACCATALYST_DOTNET_BUILD_DIR)
	$(call Q_PROF_GEN,maccatalyst) sed \
		-e "s/@VERSION@/$(MACCATALYST_NUGET_VERSION_MAJOR).$(MACCATALYST_NUGET_VERSION_MINOR).$(MACCATALYST_NUGET_VERSION_PATCH)/g" \
		-e 's/@REVISION@/$(MACCATALYST_NUGET_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h))/g' \
		-e "s/@MACCATALYST_SDK_VERSION@/$(MACCATALYST_SDK_VERSION)/g" \
		$< > $@

$(MACCATALYST_DOTNET_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in $(TOP)/Make.config  $(GIT_DIRECTORY)/HEAD | $(MACCATALYST_DOTNET_BUILD_DIR)
	$(call Q_PROF_GEN,maccatalyst) sed \
		-e 's|@PRODUCT_NAME@|Xamarin.MacCatalyst|g' \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g' \
		-e 's|@NUGET_VERSION_NO_METADATA@|$(MACCATALYST_NUGET_VERSION_NO_METADATA)|g' \
		-e 's|@NUGET_VERSION_MAJOR@|$(MACCATALYST_NUGET_VERSION_MAJOR)|g' \
		-e 's|@NUGET_VERSION_MINOR@|$(MACCATALYST_NUGET_VERSION_MINOR)|g' \
		-e 's|@NUGET_VERSION_REV@|$(MACCATALYST_NUGET_VERSION_PATCH)|g' \
		-e 's|@NUGET_VERSION_BUILD@|$(MACCATALYST_NUGET_COMMIT_DISTANCE)|g' \
		-e 's|@DOTNET_PLATFORM@|MacCatalyst|g' \
		-e 's|@XCODE_VERSION@|$(subst .,_,$(XCODE_VERSION))|g' \
		$< > $@.tmp
	$(Q) diff $@ $@.tmp >/dev/null 2>&1 || mv -f $@.tmp $@
	$(Q) rm -f $@.tmp
	$(Q) touch $@

### .NET ###

include $(TOP)/scripts/generate-sourcelink-json/fragment.mk

define BuildDotNetIntermediateAssembly
rsp:: $($(2)_DOTNET_BUILD_DIR)/core-$(3).rsp
$($(2)_DOTNET_BUILD_DIR)/core-$(3).rsp: Makefile frameworks.sources | $($(2)_DOTNET_BUILD_DIR)
	$$(Q_DOTNET_GEN) echo \
		$(DOTNET_FLAGS) \
		$(DOTNET_CORE_WARNINGS_TO_FIX) \
		@$(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp \
		$($(2)_CORE_DEFINES) \
		$($(2)_DOTNET_CORE_SOURCES) \
		-nullable+ \
		-warnaserror+ \
		-nowarn:1591 \
		-out:$($(2)_DOTNET_BUILD_DIR)/core-$(3).dll \
		-doc:$($(2)_DOTNET_BUILD_DIR)/core-$(3).xml \
		> $$@.tmp
	$$(Q) mv $$@.tmp $$@

$($(2)_DOTNET_BUILD_DIR)/core-$(3).dll: $($(2)_DOTNET_CORE_SOURCES) $($(2)_DOTNET_BUILD_DIR)/core-$(3).rsp | $($(2)_DOTNET_BUILD_DIR)
	$$(Q_DOTNET_GEN) $(DOTNET_CSC) @$($(2)_DOTNET_BUILD_DIR)/core-$(3).rsp

$($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).rsp: Makefile frameworks.sources
	$$(Q_DOTNET_GEN) echo \
		-debug \
		-unsafe \
		-target:library \
		-nowarn:436,1591,CA1416,CS8981 \
		-warnaserror+ \
		-out:$($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).dll \
		-doc:$($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).xml \
		-r:$($(2)_DOTNET_BUILD_DIR)/core-$(3).dll \
		-nostdlib \
		-nologo \
		-r:$(DOTNET_BINDING_ATTRIBUTES) \
		$($(2)_APIS) \
		$($(2)_GENERATOR_DEFINES) \
		@$(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp \
		> $$@.tmp
	$$(Q) mv $$@.tmp $$@

$($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).dll: $($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).rsp $($(2)_DOTNET_BUILD_DIR)/core-$(3).dll $(DOTNET_BINDING_ATTRIBUTES)
	$$(Q_GEN) $(DOTNET_CSC) $(DOTNET_FLAGS) @$$<

apidefinition:: $(3)-apidefinition
$(3)-apidefinition: $($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).dll

$($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources: $(DOTNET_GENERATOR) $($(2)_DOTNET_APIS) $($(2)_DOTNET_BUILD_DIR)/core-$(3).dll $($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).dll $(DOTNET_BINDING_ATTRIBUTES) $($(2)_DOTNET_BUILD_DIR)/$(3).rsp | $($(2)_DOTNET_BUILD_DIR)/generated-sources
	$$(Q_DOTNET_GEN) $$< @$($(2)_DOTNET_BUILD_DIR)/$(3).rsp

$($(2)_DOTNET_BUILD_DIR)/$(3).rsp: Makefile Makefile.generator Makefile.rgenerator frameworks.sources $(ROSLYN_GENERATOR) $(ROSLYN_ANALYZER) $(DOTNET_COMPILER) | $($(2)_DOTNET_BUILD_DIR)
	$(Q) echo \
		$($(2)_GENERATOR_FLAGS) \
		$(DOTNET_GENERATOR_FLAGS) \
		$($(2)_GENERATOR_WARNASERROR) \
		-sourceonly=$($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources \
		-tmpdir=$($(2)_DOTNET_BUILD_DIR)/generated-sources \
		-baselib=$($(2)_DOTNET_BUILD_DIR)/core-$(3).dll \
		--target-framework=.NETCoreApp,Version=$(GENERATOR_TF_VERSION),Profile=$(3) \
		--compiled-api-definition-assembly=$($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).dll \
		> $$@

DOTNET_TARGETS_$(3) += \
	$($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).dll \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).xml \

DOTNET_TARGETS_DIRS_$(3) += \
	$($(2)_DOTNET_BUILD_DIR) \
	$($(2)_DOTNET_BUILD_DIR)/generated-sources \
	$($(2)_DOTNET_BUILD_DIR)/ref \
	$($(2)_DOTNET_BUILD_DIR)/doc \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM) \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/doc/$(DOTNET_TFM) \

dotnet-gen-$(3):: $($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources
dotnet-gen:: dotnet-gen-$(3)

$($(2)_DOTNET_BUILD_DIR)/ILLink.LinkAttributes.xml: $(TOP)/src/ILLink.LinkAttributes.xml.in | $($(2)_DOTNET_BUILD_DIR)
	$$(call Q_PROF_GEN,$(3)) sed < $$< > $$@ 's|@PRODUCT_NAME@|Microsoft.$(1)|g;'

$($(2)_DOTNET_BUILD_DIR)/SourceLink.json: $($(2)_DOTNET_BUILD_DIR) $(GENERATE_SOURCELINK_JSON)
	$$(Q) $(GENERATE_SOURCELINK_JSON_EXEC) "$(PACKAGE_HEAD_REV)" "$(abspath $(TOP)/src)" "$$@"

$($(2)_DOTNET_BUILD_DIR)/embed-files.rsp: $($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources $($(2)_DOTNET_SOURCES) $(TOP)/src/generate-embed-files.sh
	$$(Q) $(TOP)/src/generate-embed-files.sh $($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources "$($(2)_DOTNET_SOURCES)" > $$@.tmp
	$$(Q) mv $$@.tmp $$@

$($(2)_DOTNET_BUILD_DIR)/ILLink.Substitutions.xml: $(TOP)/src/ILLink.Substitutions.$(1).xml | $($(2)_DOTNET_BUILD_DIR)
	$(Q) $(CP) $$< $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).dll: $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll | $(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)
	$(Q) $(CP) $$< $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).xml: $($(2)_DOTNET_BUILD_DIR)/doc/Microsoft.$(1).xml | $(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)
	$(Q) $(CP) $$< $$@

$($(2)_DOTNET_BUILD_DIR)/doc/Microsoft.$(1).xml: $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll build/.build-adr-stamp | $($(2)_DOTNET_BUILD_DIR)/doc
ifdef ENABLE_XAMARIN
	$(Q) $(MAKE) run -C $(ADR_PATH) ASSEMBLY="$(abspath $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll)" INPUT="$(abspath $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml)" OUTPUT="$(abspath $$@)"
else
	$$(Q) $$(CP) $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml $$@
endif
endef

build/.build-adr-stamp:
ifdef ENABLE_XAMARIN
	$(Q) $(MAKE) build -C $(ADR_PATH)
	$(Q) $(MAKE) create -C $(ADR_PATH)
endif
	$(Q) touch $@

# Template variables:
# 1: Platform (correct case)
# 2: PLATFORM (all upper case)
# 3: platform (all lower case)
# pseudo-code:
#     foreach (var platform in DOTNET_PLATFORMS)
#        BuildDotNetIntermediateAssembly (platform, platform.ToUpper (), platform.ToLower ())
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call BuildDotNetIntermediateAssembly,$(platform),$(shell echo $(platform) | tr '[:lower:]' '[:upper:]'),$(shell echo $(platform) | tr '[:upper:]' '[:lower:]'))))

define BuildDotNetPlatformAssembly
## We build the ref assembly when building the 64-bit version of the platform assembly,
## so we need some special logic here.
ifeq ($(4),64)
$(2)_$(4)_REFOUT_ARG = -refout:$($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll
$(2)_$(4)_REF_TARGET = $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1)%dll
$(2)_$(4)_DOC_ARG = -doc:$($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml
$(2)_$(4)_DOC_TARGET = $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1)%xml
endif

$(2)_DOTNET_PLATFORM_ASSEMBLY_DEPENDENCIES = \
	$($(2)_DOTNET_SOURCES) \
	$($(2)_DOTNET_BUILD_DIR)/Microsoft.$(1).rsp \
	$($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources \
	$($(2)_DOTNET_BUILD_DIR)/SourceLink.json \
	$($(2)_DOTNET_BUILD_DIR)/embed-files.rsp \
	$($(2)_DOTNET_BUILD_DIR)/ILLink.LinkAttributes.xml \
	$($(2)_DOTNET_BUILD_DIR)/ILLink.Substitutions.xml \
	$(PRODUCT_KEY_PATH) \

$(2)_DOTNET_PLATFORM_ASSEMBLY_DIR_DEPENDENCIES = \
	$($(2)_DOTNET_BUILD_DIR)/$(4) \
	$($(2)_DOTNET_BUILD_DIR)/ref \

rsp:: $($(2)_DOTNET_BUILD_DIR)/Microsoft.$(1).rsp

$($(2)_DOTNET_BUILD_DIR)/Microsoft.$(1).rsp: Makefile | $($(2)_DOTNET_BUILD_DIR)
	$$(Q) rm -f $$@
	$$(Q) echo \
		$(DOTNET_FLAGS) \
		/analyzer:$(ROSLYN_GENERATOR_COMMON) \
		/analyzer:$(ROSLYN_GENERATOR) \
		/analyzer:$(ROSLYN_ANALYZER) \
		/generatedfilesout:$($(2)_DOTNET_BUILD_DIR)/generated-sources \
		$$(if $(V),/reportanalyzer:$(ROSLYN_GENERATOR),) \
		$$(if $(V),/reportanalyzer:$(ROSLYN_ANALYZER),) \
		-unsafe \
		-optimize \
		$$(ARGS_$(1)) \
		-publicsign -keyfile:$(PRODUCT_KEY_PATH) \
		$$($(2)_$(4)_REFOUT_ARG) \
		$$($(2)_$(4)_DOC_ARG) \
		-sourcelink:$($(2)_DOTNET_BUILD_DIR)/SourceLink.json \
		@$$(abspath $($(2)_DOTNET_BUILD_DIR)/embed-files.rsp) \
		$$($(2)_DEFINES) \
		$(ARGS_$(4)) \
		$$(DOTNET_WARNINGS_TO_FIX) \
		@$$(abspath $(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp) \
		-res:$($(2)_DOTNET_BUILD_DIR)/ILLink.LinkAttributes.xml \
		-res:$($(2)_DOTNET_BUILD_DIR)/ILLink.Substitutions.xml \
		-warnaserror+ \
		-nullable+ \
		$$($(2)_DOTNET_SOURCES) \
		@$$(abspath $($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources) \
		-out:$($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll \
		> $$@.tmp
	$$(Q) mv $$@.tmp $$@

$($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1)%dll $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1)%pdb $$($(2)_$(4)_REF_TARGET) $$($(2)_$(4)_DOC_TARGET): $$($(2)_DOTNET_PLATFORM_ASSEMBLY_DEPENDENCIES) $$(ROSLYN_GENERATOR) $$(ROSLYN_ANALYZER) | $$($(2)_DOTNET_PLATFORM_ASSEMBLY_DIR_DEPENDENCIES)
	$$(call Q_PROF_CSC,dotnet/$(4)-bit) $(DOTNET_CSC) @$($(2)_DOTNET_BUILD_DIR)/Microsoft.$(1).rsp

dotnet-$(3):: $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll

DOTNET_TARGETS_$(3) += \
	$($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll \
	$(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM)/Microsoft.$(1).dll) \
	$(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM)/Microsoft.$(1).pdb) \

DOTNET_TARGETS_DIRS_$(3) += \
	$($(2)_DOTNET_BUILD_DIR)/$(4) \
	$(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM)) \

$(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM)/Microsoft.$(1).dll): $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll | $(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM))
	$(Q) $(CP) $$< $$@

$(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM)/Microsoft.$(1).pdb): $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).pdb | $(foreach rid,$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_$(4)),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/lib/$(DOTNET_TFM))
	$(Q) $(CP) $$< $$@

DOTNET_TARGETS += $$(DOTNET_TARGETS_$(3))
DOTNET_TARGETS_DIRS += $$(DOTNET_TARGETS_DIRS_$(3))

dotnet-$(3):: $$(DOTNET_TARGETS_$(3))

endef

# Template variables:
# 1: Platform (correct case)
# 2: PLATFORM (all upper case)
# 3: platform (all lower case)
# 4: 32 or 64 (architecture bitness)
#
# pseudo-code:
#     foreach (var platform in DOTNET_PLATFORMS)
#        foreach (var bitness in DOTNET_<platform.ToUpper ()>_BITNESSES)
#            BuildDotNetPlatformAssembly (platform, platform.ToUpper (), platform.ToLower (), bitness)
$(foreach platform,$(DOTNET_PLATFORMS),$(foreach bitness, $(DOTNET_$(shell echo $(platform) | tr '[:lower:]' '[:upper:]')_BITNESSES), $(eval $(call BuildDotNetPlatformAssembly,$(platform),$(shell echo $(platform) | tr '[:lower:]' '[:upper:]'),$(shell echo $(platform) | tr '[:upper:]' '[:lower:]'),$(bitness)))))

#
# Global targets
#

SHARED_PATH := ../runtime
$(SHARED_PATH)/Delegates.generated.cs: $(SHARED_PATH)/Delegates.cs.t4 $(SHARED_PATH)/delegates.t4
	$(Q) $(MAKE) -C $(SHARED_PATH) Delegates.generated.cs

$(COMMON_TARGET_DIRS):
	$(Q) mkdir -p $@

$(DOTNET_COMPILER): Makefile $(TOP)/Make.config | $(DOTNET_BUILD_DIR)
	$(Q) echo "#!/bin/zsh -e" > $@
	$(Q) echo "exec $(DOTNET_CSC) $(DOTNET_FLAGS) \"\$$@\"" >> $@
	$(Q) chmod +x $@

DOTNET_GENERATE_FRAMEWORKS_CONSTANTS=generate-frameworks-constants/dotnet/bin/Debug/$(DOTNET_TFM)/generate-frameworks-constants.dll

$(DOTNET_GENERATE_FRAMEWORKS_CONSTANTS): $(wildcard generate-frameworks-constants/*.cs*) $(TOP)/tools/common/Frameworks.cs Makefile
	$(Q) $(DOTNET) build "/bl:$@.binlog" /r generate-frameworks-constants/dotnet/generate-frameworks-constants.csproj $(MSBUILD_VERBOSITY)
	$(Q) touch $@ # Running 'dotnet build' doesn't always touch the target, so make sure we do here, otherwise make can end up confused.

# This rule means: generate a Constants.<platform>.generated.cs for the frameworks in the variable <PLATFORM>_FRAMEWORKS
$(DOTNET_BUILD_DIR)/%/Constants.generated.cs: Makefile $(DOTNET_GENERATE_FRAMEWORKS_CONSTANTS) | $(DOTNET_BUILD_DIR)
	$(Q) $(DOTNET) $(DOTNET_GENERATE_FRAMEWORKS_CONSTANTS) "$*" "$@.tmp"
	$(Q) mv "$@.tmp" "$@"

install-local:: $(INSTALL_TARGETS)
all-local:: $(ALL_TARGETS)

$(DOTNET_TARGETS_DIRS):
	$(Q) mkdir -p $@

define DotNetProjectFiles
$(DOTNET_BUILD_DIR)/projects/$(1)/$(1).csproj: dotnet.tmpl.csproj Makefile $$(wildcard $(CURDIR)/*.sources)
	@mkdir -p $$(dir $$@)
	@sed \
		-e 's*%DOTNET_TFM%*$(DOTNET_TFM)*' \
		-e 's*%PLATFORM%*$(1)*' \
		-e 's*<!--%FILES%-->*$$(foreach file,$$($(2)_DOTNET_SOURCES),<Compile Include="../../../../$$(file)" Link="sources/$$(file)" />)*' \
		-e 's*<!--%APIS%-->*$$(foreach file,$$($(2)_DOTNET_APIS),<None Include="../../../../$$(file)" Link="apis/$$(file)" />)*' \
		$$< | xmllint --format - > $$@
PROJECT_FILES += $(DOTNET_BUILD_DIR)/projects/$(1)/$(1).csproj
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetProjectFiles,$(platform),$(shell echo $(platform) | tr '[:lower:]' '[:upper:]'))))

dotnet: $(DOTNET_TARGETS)
all-local:: $(DOTNET_TARGETS)

project-files: $(PROJECT_FILES)

$(TOP)/tools/common/SdkVersions.cs: $(TOP)/tools/common/SdkVersions.in.cs
	@$(MAKE) -C $(TOP)/tools/mtouch ../common/SdkVersions.cs

COMMA=,
MinimumVersions.cs: MinimumVersions.cs.in Makefile $(TOP)/Make.config
	$(Q_GEN) sed \
		-e 's/@DOTNET_MIN_IOS_SDK_VERSION@/$(DOTNET_MIN_IOS_SDK_VERSION)/g' \
		-e 's/@DOTNET_MIN_TVOS_SDK_VERSION@/$(DOTNET_MIN_TVOS_SDK_VERSION)/' \
		-e 's/@DOTNET_MIN_MACOS_SDK_VERSION@/$(DOTNET_MIN_MACOS_SDK_VERSION)/' \
		-e 's/@DOTNET_MIN_MACCATALYST_SDK_VERSION@/$(DOTNET_MIN_MACCATALYST_SDK_VERSION)/g' \
		$< > $@

# Using .SECONDARY can cause make to go into an infinite loop.
# See https://github.com/xamarin/maccore/issues/762.
#.SECONDARY:
