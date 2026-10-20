TOP=..
include $(TOP)/Make.config

BUILD_DIR=build
RSP_DIR=rsp
DOTNET_BUILD_DIR=$(BUILD_DIR)/dotnet
PROJECT_DIR=.

include $(TOP)/src/frameworks.sources
include $(TOP)/mk/rules.mk
include $(TOP)/mk/xamarin.mk

MACOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/macos
IOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/ios
TVOS_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/tvos
MACCATALYST_DOTNET_BUILD_DIR=$(DOTNET_BUILD_DIR)/maccatalyst

GENERATOR_FLAGS=-process-enums -core -nostdlib -native-exception-marshalling --ns:ObjCRuntime

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
	/r:$(DOTNET_BCL_DIR)/System.Net.HttpListener.dll \
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
DOTNET_FLAGS=/warnaserror+ /nostdlib+ /deterministic /features:strict /nologo /target:library /debug /unsafe /define:NET $(DOTNET_OR_GREATER_DEFINES) $(DOTNET_REFERENCES) /fullpaths -nullable+

ifeq ($(XCODE_IS_STABLE),true)
DOTNET_FLAGS+=/define:XCODE_IS_STABLE
endif

DOTNET_GENERATOR_FLAGS=$(GENERATOR_FLAGS) --lib=$(DOTNET_BCL_DIR) -attributelib:$(DOTNET_BINDING_ATTRIBUTES) $(DOTNET_REFERENCES)
DOTNET_GENERATOR=$(DOTNET_BUILD_DIR)/bgen/bgen
DOTNET_BINDING_ATTRIBUTES=$(DOTNET_BUILD_DIR)/Xamarin.Apple.BindingAttributes.dll
APPLE_DOC_READER=$(TOP)/packages/appledocreader.$(ADR_RUNTIME_IDENTIFIER)/$(ADR_NUGET_VERSION)/tools/any/$(ADR_RUNTIME_IDENTIFIER)/AppleDocReader
APPLE_DOC_READER_LOCK=$(BUILD_DIR)/.appledocreader.lock

# AppleDocReader (ADR) enriches our generated XML documentation (Microsoft.iOS.xml,
# Microsoft.tvOS.xml, ...) with links to Apple's documentation. It does this by reading Apple's
# *offline* documentation cache that historically shipped inside Xcode as a SQLite database at:
#
#     <Xcode.app>/Contents/SharedFrameworks/DNTDocumentationSupport.framework/Versions/A/Resources/external/cache.db
#
# Starting with Xcode 27, Apple REMOVED this bundled offline documentation cache. In Xcode 27 that
# directory contains only a tombstone file literally named "deleted", and a content-based scan of
# the entire Xcode 27 app bundle finds no replacement documentation database (only an unrelated
# AppIntentSchemas.sqlite). Instead, Xcode 27 downloads documentation on demand into a CoreSpotlight
# index at:
#
#     ~/Library/Developer/Xcode/DocumentationCache/<cacheVersion>/<xcodeVersion>/DeveloperDocumentation.index
#
# which is a proprietary Spotlight metadata store (file magic "8tsd"), not SQLite. That on-demand
# cache is unsuitable as a build input because it:
#   * is not part of the Xcode bundle, so it cannot be a hermetic build input;
#   * only exists after an interactive/on-demand documentation download, so it is absent on clean
#     and CI machines;
#   * is only queryable through Apple's private CoreDocumentation XPC service (there is no public API
#     to query a standalone .index bundle), and its binary format is undocumented and changes between
#     Xcode betas.
#
# As a result, AppleDocReader cannot enrich documentation when building with Xcode 27 or later. Rather
# than fail the build, we detect the absence of the legacy offline cache and skip the enrichment step,
# copying the XML documentation through unchanged (identical to the NO_XCODE behavior below). This only
# affects whether <appledoc> links are present; the XML documentation is otherwise complete and correct.
#
# ENABLE_APPLE_DOC_READER is therefore set only when an Xcode with the legacy offline cache is
# available. This is capability-based on purpose: if a future Xcode restores an offline cache at the
# expected path, enrichment turns back on automatically with no further changes here.
#
# Follow-up: re-implementing documentation enrichment against a hermetic source (e.g. Apple's online
# DocC render JSON at https://developer.apple.com/tutorials/data/documentation/<framework>/<symbol>.json,
# which still carries the Clang USR, e.g. "c:objc(cs)UIView", that ADR keys on) should be tracked
# separately.
ifdef NO_XCODE
ENABLE_APPLE_DOC_READER :=
else
APPLE_DOC_READER_CACHE_DB := $(abspath $(DEVELOPER_DIR)/../../Contents/SharedFrameworks/DNTDocumentationSupport.framework/Versions/A/Resources/external/cache.db)
ENABLE_APPLE_DOC_READER := $(if $(wildcard $(APPLE_DOC_READER_CACHE_DB)),1,)
ifneq ($(ENABLE_APPLE_DOC_READER),1)
$(warning AppleDocReader: the Xcode offline documentation cache was not found at $(APPLE_DOC_READER_CACHE_DB) (Xcode 27+ no longer ships it); XML documentation will be generated WITHOUT Apple documentation links.)
endif
endif

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

# We ignore some xml docs warnings when building the core and apidefinition assemblies, because
# the xml docs might want to reference generated APIs, so the compiler would warn about not
# finding those APIs (because they haven't been generated yet).
XML_DOCS_WARNINGS=CS0419,CS1574,CS1580

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

# warning CS8981: The type name 'nfloat' only contains lower-cased ascii characters. Such names may become reserved for the language.
CSC_WARNINGS_TO_FIX:=$(CSC_WARNINGS_TO_FIX),CS8981

# availability warnings, need to fix https://github.com/dotnet/macios/pull/21185 to stop ignoring this.
CSC_WARNINGS_TO_FIX:=$(CSC_WARNINGS_TO_FIX),CA1416,CA1422

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

define DotNetVariables

$(2)_DEFINES += -d:__$(2)__

## Add new bindings + source files in frameworks.sources, not here.

$(2)_DOTNET_EXTRA_SOURCES = \
	$($(2)_DOTNET_BUILD_DIR)/Constants.cs    \
	$($(2)_DOTNET_BUILD_DIR)/Constants.generated.cs \
	$($(2)_DOTNET_BUILD_DIR)/AssemblyInfo.cs \
	$(SHARED_DESIGNER_CS) \

$(2)_DOTNET_CORE_SOURCES += $$($(2)_DOTNET_EXTRA_SOURCES)

$(2)_DOTNET_SOURCES += $$($(2)_DOTNET_EXTRA_SOURCES)

$($(2)_DOTNET_BUILD_DIR)/Constants.cs: Constants.$(1).cs.in Makefile $(TOP)/Make.config.inc | $($(2)_DOTNET_BUILD_DIR)
	$$(call Q_PROF_GEN,$(3)) sed \
		-e "s/@VERSION@/$($(2)_NUGET_VERSION_MAJOR).$($(2)_NUGET_VERSION_MINOR).$($(2)_NUGET_VERSION_PATCH)/g" \
		-e 's/@REVISION@/$($(2)_NUGET_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(shell git log -1 --pretty=%h))/g' \
		-e "s/@$(2)_SDK_VERSION@/$($(2)_SDK_VERSION)/g" \
		$$< > $$@

$($(2)_DOTNET_BUILD_DIR)/AssemblyInfo.cs: $(TOP)/src/AssemblyInfo.cs.in | $($(2)_DOTNET_BUILD_DIR)
	$$(call Q_PROF_GEN,$(3)) sed \
		-e 's|@PACKAGE_HEAD_REV@|$(PACKAGE_HEAD_REV)|g' \
		-e 's|@PACKAGE_HEAD_BRANCH@|$(CURRENT_BRANCH_SED_ESCAPED)|g' \
		-e 's|@NUGET_VERSION_NO_METADATA@|$($(2)_NUGET_VERSION_NO_METADATA)|g' \
		-e 's|@NUGET_VERSION_MAJOR@|$($(2)_NUGET_VERSION_MAJOR)|g' \
		-e 's|@NUGET_VERSION_MINOR@|$($(2)_NUGET_VERSION_MINOR)|g' \
		-e 's|@NUGET_VERSION_REV@|$($(2)_NUGET_VERSION_PATCH)|g' \
		-e 's|@NUGET_VERSION_BUILD@|$($(2)_NUGET_COMMIT_DISTANCE)|g' \
		-e 's|@DOTNET_PLATFORM@|iOS|g' \
		-e 's|@XCODE_VERSION@|$(subst .,_,$(XCODE_VERSION))|g' \
		< $$< > $$@.tmp
	$$(Q) diff $$@ $$@.tmp >/dev/null 2>&1 || mv -f $$@.tmp $$@
	$$(Q) rm -f $$@.tmp
	$$(Q) touch $$@
endef

# Template variables:
# 1: Platform (correct case)
# 2: PLATFORM (all upper case)
# 3: platform (all lower case)
# pseudo-code:
#     foreach (var platform in DOTNET_PLATFORMS)
#        BuildDotNetIntermediateAssembly (platform, platform.ToUpper (), platform.ToLower ())
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetVariables,$(platform),$(call uppercase,$(platform)),$(call lowercase,$(platform)))))

#
# iOS
#

IOS_DEFINES += -d:IOS
IOS_GENERATOR_FLAGS = -inline-selectors

#
# macOS
#

MACOS_DEFINES += -d:MONOMAC
MACOS_GENERATOR_FLAGS =

#
# tvOS
#

TVOS_DEFINES += -d:TVOS
TVOS_GENERATOR_FLAGS = -inline-selectors

#
# Mac Catalyst
#

# MacCatalyst is a variant of iOS, so it defines the iOS variables as well.
# Note that we have to define __IOS__ here, because it won't be in IOS_DEFINES unless iOS is an included platform.
MACCATALYST_DEFINES += $(IOS_DEFINES) -d:__IOS__ -d:MACCATALYST
MACCATALYST_GENERATOR_FLAGS = -inline-selectors

### .NET ###

include $(TOP)/scripts/generate-sourcelink-json/fragment.mk
include $(TOP)/scripts/rsp-to-csproj/fragment.mk

define BuildDotNetIntermediateAssembly

CORE_$(1)_CSPROJ = $(DOTNET_BUILD_DIR)/$(1)/csproj/core/Core.$(1).csproj
CSPROJECTS += $$(CORE_$(1)_CSPROJ)
$$(CORE_$(1)_CSPROJ): $($(2)_DOTNET_BUILD_DIR)/core-$(3).rsp $(RSP_TO_CSPROJ)
	$$(Q) rm -f $$@
	$$(Q) mkdir -p $$(dir $$@)
	$$(call Q_PROF_GEN,$(1)) $(RSP_TO_CSPROJ_EXEC) \
		--rsp $(abspath $$<) \
		--working-directory $(abspath .) \
		--target-framework $(DOTNET_TFM) \
		--property=LowerCasePlatform=$(3) \
		--output $$@.tmp
	$$(Q) mv $$@.tmp $$@

rsp:: $($(2)_DOTNET_BUILD_DIR)/core-$(3).rsp
$($(2)_DOTNET_BUILD_DIR)/core-$(3).rsp: Makefile frameworks.sources $($(2)_DOTNET_BUILD_DIR)/GeneratedMSBuildEditorConfig.editorconfig $(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp | $($(2)_DOTNET_BUILD_DIR)
	$$(Q_DOTNET_GEN) echo \
		$(DOTNET_FLAGS) \
		$(DOTNET_CORE_WARNINGS_TO_FIX) \
		@$(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp \
		$($(2)_DEFINES) \
		-d:COREBUILD \
		$($(2)_DOTNET_CORE_SOURCES) \
		-warnaserror+ \
		-nowarn:1591 \
		-nowarn:$(XML_DOCS_WARNINGS) \
		-out:$($(2)_DOTNET_BUILD_DIR)/core-$(3).dll \
		-doc:$($(2)_DOTNET_BUILD_DIR)/core-$(3).xml \
		> $$@.tmp
	$$(Q) mv $$@.tmp $$@

core:: $($(2)_DOTNET_BUILD_DIR)/core-$(3).dll
$($(2)_DOTNET_BUILD_DIR)/core-$(3).dll: $($(2)_DOTNET_CORE_SOURCES) $($(2)_DOTNET_BUILD_DIR)/core-$(3).rsp | $($(2)_DOTNET_BUILD_DIR)
	$$(Q_DOTNET_GEN) $(DOTNET_CSC) @$($(2)_DOTNET_BUILD_DIR)/core-$(3).rsp

$($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).rsp: Makefile frameworks.sources $($(2)_DOTNET_BUILD_DIR)/GeneratedMSBuildEditorConfig.editorconfig $(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp | $($(2)_DOTNET_BUILD_DIR)
	$$(Q_DOTNET_GEN) echo \
		-debug \
		-unsafe \
		-target:library \
		-nowarn:436,1591,CA1416,CS8981 \
		-nowarn:$(XML_DOCS_WARNINGS) \
		-warnaserror+ \
		-nullable+ \
		-out:$($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).dll \
		-doc:$($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).xml \
		-r:$($(2)_DOTNET_BUILD_DIR)/core-$(3).dll \
		-nostdlib \
		-nologo \
		-r:$(DOTNET_BINDING_ATTRIBUTES) \
		$($(2)_APIS) \
		$($(2)_DEFINES) \
		@$(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp \
		> $$@.tmp
	$$(Q) mv $$@.tmp $$@

APIDEFINITION_$(1)_CSPROJ = $(DOTNET_BUILD_DIR)/$(1)/csproj/api/ApiDefinition.$(1).csproj
CSPROJECTS += $$(APIDEFINITION_$(1)_CSPROJ)
$$(APIDEFINITION_$(1)_CSPROJ): $($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).rsp $(RSP_TO_CSPROJ)
	$$(Q) rm -f $$@
	$$(Q) mkdir -p $$(dir $$@)
	$$(call Q_PROF_GEN,$(1)) $(RSP_TO_CSPROJ_EXEC) \
		--rsp $(abspath $$<) \
		--working-directory $(abspath .) \
		--target-framework $(DOTNET_TFM) \
		--property=LowerCasePlatform=$(3) \
		--output $$@.tmp
	$$(Q) mv $$@.tmp $$@

apidefinition:: $($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).dll
$($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).dll: $($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).rsp $($(2)_DOTNET_BUILD_DIR)/core-$(3).dll $(DOTNET_BINDING_ATTRIBUTES) $($(2)_APIS)
	$$(Q_GEN) $(DOTNET_CSC) $(DOTNET_FLAGS) @$$<

apidefinition:: $(3)-apidefinition
$(3)-apidefinition: $($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).dll

$($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources: $(DOTNET_GENERATOR) $($(2)_DOTNET_APIS) $($(2)_DOTNET_BUILD_DIR)/core-$(3).dll $($(2)_DOTNET_BUILD_DIR)/apidefinition-$(3).dll $(DOTNET_BINDING_ATTRIBUTES) $($(2)_DOTNET_BUILD_DIR)/$(3).rsp | $($(2)_DOTNET_BUILD_DIR)/generated-sources
	$$(Q_DOTNET_GEN) $$< @$($(2)_DOTNET_BUILD_DIR)/$(3).rsp

$($(2)_DOTNET_BUILD_DIR)/$(3).rsp: Makefile Makefile.generator Makefile.rgenerator frameworks.sources $(ROSLYN_GENERATOR) $(ROSLYN_ANALYZER) | $($(2)_DOTNET_BUILD_DIR)
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

$($(2)_DOTNET_BUILD_DIR)/GeneratedMSBuildEditorConfig.editorconfig: | $($(2)_DOTNET_BUILD_DIR)
	$$(Q) rm -f "$$@.tmp"
	$$(Q) printf "is_global = true\n" >> "$$@.tmp"
	$$(Q) printf "build_property.TargetFramework = $(DOTNET_TFM)-$(3)\n" >> "$$@.tmp"
	$$(Q) printf "build_property.TargetFrameworkIdentifier = .NETCoreApp\n" >> "$$@.tmp"
	$$(Q) printf "build_property.TargetFrameworkVersion = v$(DOTNET_MAJOR_VERSION).0\n" >> "$$@.tmp"
	$$(Q) mv "$$@.tmp" "$$@"

$($(2)_DOTNET_BUILD_DIR)/ILLink.LinkAttributes.xml: $(TOP)/src/ILLink.LinkAttributes.xml.in | $($(2)_DOTNET_BUILD_DIR)
	$$(call Q_PROF_GEN,$(3)) sed < $$< > $$@ 's|@PRODUCT_NAME@|Microsoft.$(1)|g;'

$($(2)_DOTNET_BUILD_DIR)/SourceLink.json: $($(2)_DOTNET_BUILD_DIR) $(GENERATE_SOURCELINK_JSON)
	$$(Q) $(GENERATE_SOURCELINK_JSON_EXEC) "$(PACKAGE_HEAD_REV)" "$(abspath $(TOP)/src)" "$$@"

$($(2)_DOTNET_BUILD_DIR)/embed-files.rsp: $($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources $($(2)_DOTNET_SOURCES) $(TOP)/src/generate-embed-files.sh
	$$(Q) $(TOP)/src/generate-embed-files.sh $($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources "$($(2)_DOTNET_SOURCES)" > $$@.tmp
	$$(Q) mv $$@.tmp $$@

$($(2)_DOTNET_BUILD_DIR)/ILLink.Substitutions.xml: $(TOP)/src/ILLink.Substitutions.$(1).xml | $($(2)_DOTNET_BUILD_DIR)
	$$(Q) $(CP) $$< $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).dll: $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll | $(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)
	$$(Q) $(CP) $$< $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).xml: $($(2)_DOTNET_BUILD_DIR)/doc/Microsoft.$(1).xml | $(DOTNET_DESTDIR)/$($(2)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)
	$$(Q) $(CP) $$< $$@

ifeq ($(ENABLE_APPLE_DOC_READER),1)
# AppleDocReader backs up a shared Xcode SQLite database, so serialize invocations.
$($(2)_DOTNET_BUILD_DIR)/doc/Microsoft.$(1).xml: $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll build/.build-adr-stamp | $($(2)_DOTNET_BUILD_DIR)/doc
	$$(Q_GEN) /usr/bin/lockf "$(APPLE_DOC_READER_LOCK)" "$(APPLE_DOC_READER)" inject docs --assembly="$(abspath $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll)" --input="$(abspath $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml)" --output="$(abspath $$@)" --xcode="$(DEVELOPER_DIR)/../.." --runtimeDll="$(DOTNET_BCL_DIR)/System.Runtime.dll" -f
else
# Skip AppleDocReader and just copy the XML. This happens either when building without Xcode
# (NO_XCODE), or when the Xcode in use does not ship the offline documentation cache that
# AppleDocReader reads (Xcode 27+). See the ENABLE_APPLE_DOC_READER comment near the top of this file.
$($(2)_DOTNET_BUILD_DIR)/doc/Microsoft.$(1).xml: $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml | $($(2)_DOTNET_BUILD_DIR)/doc
	$(Q) $(CP) $$< $$@
endif
endef

ifeq ($(ENABLE_APPLE_DOC_READER),1)
build/.build-adr-stamp:
	$(MAKE) -C $(TOP)/tools/adr
	$(Q) touch $@
endif

# Template variables:
# 1: Platform (correct case)
# 2: PLATFORM (all upper case)
# 3: platform (all lower case)
# pseudo-code:
#     foreach (var platform in DOTNET_PLATFORMS)
#        BuildDotNetIntermediateAssembly (platform, platform.ToUpper (), platform.ToLower ())
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call BuildDotNetIntermediateAssembly,$(platform),$(call uppercase,$(platform)),$(call lowercase,$(platform)))))

define BuildDotNetPlatformAssembly
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

PLATFORM_$(1)_CSPROJ = $(DOTNET_BUILD_DIR)/$(1)/csproj/platform/Microsoft.$(1).csproj
CSPROJECTS += $$(PLATFORM_$(1)_CSPROJ)
$$(PLATFORM_$(1)_CSPROJ): $($(2)_DOTNET_BUILD_DIR)/Microsoft.$(1).rsp Makefile $(RSP_TO_CSPROJ) $($(2)_DOTNET_BUILD_DIR)/embed-files.rsp
	$$(Q) rm -f $$@
	$$(Q) mkdir -p $$(dir $$@)
	$$(call Q_PROF_GEN,$(1)) $(RSP_TO_CSPROJ_EXEC) \
		--rsp $(abspath $($(2)_DOTNET_BUILD_DIR)/Microsoft.$(1).rsp) \
		--working-directory $(abspath .) \
		--target-framework $(DOTNET_TFM) \
		--property=LowerCasePlatform=$(3) \
		--output $$@.tmp
	$$(Q) mv $$@.tmp $$@

rsp:: $($(2)_DOTNET_BUILD_DIR)/Microsoft.$(1).rsp

$($(2)_DOTNET_BUILD_DIR)/Microsoft.$(1).rsp: Makefile frameworks.sources $(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp | $($(2)_DOTNET_BUILD_DIR)
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
		-refout:$($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).dll \
		-doc:$($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1).xml \
		-sourcelink:$($(2)_DOTNET_BUILD_DIR)/SourceLink.json \
		@$$(abspath $($(2)_DOTNET_BUILD_DIR)/embed-files.rsp) \
		$$($(2)_DEFINES) \
		$$(DOTNET_WARNINGS_TO_FIX) \
		@$$(abspath $(RSP_DIR)/dotnet/$(3)-defines-dotnet.rsp) \
		-res:$($(2)_DOTNET_BUILD_DIR)/ILLink.LinkAttributes.xml \
		-res:$($(2)_DOTNET_BUILD_DIR)/ILLink.Substitutions.xml \
		-warnaserror+ \
		$$($(2)_DOTNET_SOURCES) \
		@$$(abspath $($(2)_DOTNET_BUILD_DIR)/$(3)-generated-sources) \
		-out:$($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll \
		> $$@.tmp
	$$(Q) mv $$@.tmp $$@

$($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1)%dll $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1)%pdb $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1)%dll $($(2)_DOTNET_BUILD_DIR)/ref/Microsoft.$(1)%xml: $$($(2)_DOTNET_PLATFORM_ASSEMBLY_DEPENDENCIES) $$(ROSLYN_GENERATOR) $$(ROSLYN_ANALYZER) | $$($(2)_DOTNET_PLATFORM_ASSEMBLY_DIR_DEPENDENCIES)
	$$(call Q_PROF_CSC,dotnet) $(DOTNET_CSC) @$($(2)_DOTNET_BUILD_DIR)/Microsoft.$(1).rsp

ifdef SERIALIZE_CSC
# This is to serialize the csc commands for the platform assembly. The platform assembly takes a while to compile (maxing out the CPU), and csc is already multi-threaded, which
# means that if we're running multiple long csc tasks simultaneously, the CPU of the machine will get absolutely thrashed. csc (aka Roslyn) isn't exactly a cheapskate with memory either...
# It looks a bit weird, but the idea is that in this template, every platform assembly depends on the assembly from the previous template expansion.
# It's opt-in, because it slows down the build a bit.
$($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll: $(LAST_DLL)
LAST_DLL=$($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll
endif

dotnet-$(3):: $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll

DOTNET_TARGETS_$(3) += \
	$($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_MANAGED_NAME)/runtimes/$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_NO_ARCH)/lib/$(DOTNET_TFM)/Microsoft.$(1).dll \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_MANAGED_NAME)/runtimes/$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_NO_ARCH)/lib/$(DOTNET_TFM)/Microsoft.$(1).pdb \

DOTNET_TARGETS_DIRS_$(3) += \
	$($(2)_DOTNET_BUILD_DIR)/$(4) \
	$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_MANAGED_NAME)/runtimes/$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_NO_ARCH)/lib/$(DOTNET_TFM) \

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_MANAGED_NAME)/runtimes/$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_NO_ARCH)/lib/$(DOTNET_TFM)/Microsoft.$(1).dll: $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).dll | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_MANAGED_NAME)/runtimes/$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_NO_ARCH)/lib/$(DOTNET_TFM)
	$(Q) $(CP) $$< $$@

$(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_MANAGED_NAME)/runtimes/$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_NO_ARCH)/lib/$(DOTNET_TFM)/Microsoft.$(1).pdb: $($(2)_DOTNET_BUILD_DIR)/$(4)/Microsoft.$(1).pdb | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_MANAGED_NAME)/runtimes/$(DOTNET_$(2)_RUNTIME_IDENTIFIERS_NO_ARCH)/lib/$(DOTNET_TFM)
	$(Q) $(CP) $$< $$@

DOTNET_TARGETS += $$(DOTNET_TARGETS_$(3))
DOTNET_TARGETS_DIRS += $$(DOTNET_TARGETS_DIRS_$(3))

dotnet-$(3):: $$(DOTNET_TARGETS_$(3))

endef

# Template variables:
# 1: Platform (correct case)
# 2: PLATFORM (all upper case)
# 3: platform (all lower case)
# 4: 64 (bits)
#
# pseudo-code:
#     foreach (var platform in DOTNET_PLATFORMS)
#        BuildDotNetPlatformAssembly (platform, platform.ToUpper (), platform.ToLower (), 64)
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call BuildDotNetPlatformAssembly,$(platform),$(call uppercase,$(platform)),$(call lowercase,$(platform)),64)))

define DefineTargets
$(1)_NUGET_TARGETS = \
	$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/TrimAttributes.LinkDescription.xml \

$(DOTNET_DESTDIR)/$($(1)_NUGET_SDK_NAME)/tools/TrimAttributes.LinkDescription.xml: TrimAttributes.LinkDescription.xml
	$$(Q) $(CP) $$< $$@

DOTNET_TARGETS += $$($(1)_NUGET_TARGETS)
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DefineTargets,$(platform))))

#
# Global targets
#

SHARED_PATH := ../runtime
$(SHARED_PATH)/Delegates.generated.cs: $(SHARED_PATH)/Delegates.cs.t4 $(SHARED_PATH)/delegates.t4
	$(Q) $(MAKE) -C $(SHARED_PATH) Delegates.generated.cs

$(COMMON_TARGET_DIRS):
	$(Q) mkdir -p $@

# This rule means: generate a Constants.<platform>.generated.cs for the frameworks in the variable <PLATFORM>_FRAMEWORKS
include $(TOP)/scripts/generate-frameworks-constants/fragment.mk
$(DOTNET_BUILD_DIR)/%/Constants.generated.cs: Makefile $(GENERATE_FRAMEWORKS_CONSTANTS) | $(DOTNET_BUILD_DIR)
	$(Q) $(GENERATE_FRAMEWORKS_CONSTANTS_EXEC) "$*" "$@.tmp"
	$(Q) mv "$@.tmp" "$@"

# This target builds all the generated project files. It's not really useful by itself,
# except to validate that the csproj files actually work.
build-csproj: $(CSPROJECTS) $(foreach platform,$(DOTNET_PLATFORMS),$($(call uppercase,$(platform))_DOTNET_PLATFORM_ASSEMBLY_DEPENDENCIES))
	$(Q) set -e; \
	for csproj in $(CSPROJECTS); do \
		echo "Building $$csproj..."; \
		$(DOTNET) build $$csproj /bl:"$$(dirname $$csproj)/$$(basename -s .csproj $$csproj).binlog" $(DOTNET_BUILD_VERBOSITY); \
		echo "Built $$csproj successfully."; \
	done

# This targets runs 'dotnet format' on the generated project files.
format-csproj: $(CSPROJECTS) $(foreach platform,$(DOTNET_PLATFORMS),$($(call uppercase,$(platform))_DOTNET_PLATFORM_ASSEMBLY_DEPENDENCIES))
	$(Q) set -e; \
	for csproj in $(CSPROJECTS); do \
		echo "Formatting $$csproj..."; \
		$(DOTNET) format whitespace $$csproj; \
		echo "Formatted $$csproj successfully."; \
	done

# This builds the project files, enabling C# analyzers
analyze:
	$(Q) $(MAKE) -j $(CSPROJECTS) $(foreach platform,$(DOTNET_PLATFORMS),$($(call uppercase,$(platform))_DOTNET_PLATFORM_ASSEMBLY_DEPENDENCIES))
	$(Q) set -e; \
	for csproj in $(CSPROJECTS); do \
		echo "Analyzing $$csproj..."; \
		$(DOTNET) clean $$csproj; \
		$(DOTNET) build $$csproj /p:ANALYZE_CSPROJ=true /p:TreatWarningsAsErrors=false /p:DOTNET_ANALYZERS_DIR=$(DOTNET_ANALYZERS_DIR) /bl:$$csproj-analyze.binlog; \
		echo "Analyzed $$csproj successfully."; \
	done

# This target builds the generated project files with StyleCop enabled.
stylecop-csproj: $(CSPROJECTS) $(foreach platform,$(DOTNET_PLATFORMS),$($(call uppercase,$(platform))_DOTNET_PLATFORM_ASSEMBLY_DEPENDENCIES))
	$(Q) set -e; \
	for csproj in $(CSPROJECTS); do \
		echo "Building $$csproj with StyleCop enabled..."; \
		$(DOTNET) build $$csproj $(DOTNET_BUILD_VERBOSITY) /p:EnableStyleCop=true; \
		echo "Built $$csproj with StyleCop successfully."; \
	done

csproj:: $(CSPROJECTS)
ALL_TARGETS += $(CSPROJECTS)

install-local:: $(INSTALL_TARGETS)
all-local:: $(ALL_TARGETS)

$(DOTNET_TARGETS_DIRS):
	$(Q) mkdir -p $@

dotnet: $(DOTNET_TARGETS)
all-local:: $(DOTNET_TARGETS)

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

# Some validations

SOURCE_FILES_IN_GIT:=$(shell git ls-files -- */*.cs DotNetGlobals.cs MinimumVersions.cs MonoNativeFunctionWrapperAttribute.cs MonoPInvokeCallbackAttribute.cs | grep -v '^bgen/' | sort -u)
SOURCE_FILES_IN_MK:=$(shell grep '[.]cs' frameworks.sources | grep -v '\#' | grep -v ':=' | grep -v 'MinimumVersions.cs' | grep -v ../runtime/Delegates.generated.cs | sed -e 's/\\//' -e 's/^[[:space:]]*//' -e 's/[[:space:]]*$$//' | sort -u)

SOURCE_FILES_ONLY_IN_MK:=$(filter-out $(SOURCE_FILES_IN_GIT),$(SOURCE_FILES_IN_MK))
SOURCE_FILES_ONLY_IN_GIT:=$(filter-out $(SOURCE_FILES_IN_MK),$(SOURCE_FILES_IN_GIT))

all-local:: verify
verify:
	@if [[ "$(words $(SOURCE_FILES_ONLY_IN_MK))" != "0" ]]; then echo "Found $(words $(SOURCE_FILES_ONLY_IN_MK)) file(s) in frameworks.sources not in source control: $(SOURCE_FILES_ONLY_IN_MK) (if you just added these files, commit them to fix this error)"; fi
	@if [[ "$(words $(SOURCE_FILES_ONLY_IN_GIT))" != "0" ]]; then echo "Found $(words $(SOURCE_FILES_ONLY_IN_GIT)) file(s) in source control, but not in frameworks.sources: : $(SOURCE_FILES_ONLY_IN_GIT) (did you add this files, but forget to add them to frameworks.sources?)"; fi
	@if [[ "$(words $(SOURCE_FILES_ONLY_IN_MK))$(words $(SOURCE_FILES_ONLY_IN_GIT))" != "00" ]]; then exit 1; fi

# Using .SECONDARY can cause make to go into an infinite loop.
# See https://github.com/xamarin/maccore/issues/762.
#.SECONDARY:
