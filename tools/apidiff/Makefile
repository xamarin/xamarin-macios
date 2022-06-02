TOP = ../..

include $(TOP)/Make.config

ifdef SKIP_NEW_APIS #Requires https://github.com/mono/mono/commit/4c6a463678f3f0bfa599caeb66de72c7217fa95d
NEW_REGEX = "-n:.?"
endif

ifdef SKIP_ADDED_APIS
ADD_REGEX = "-a:.?"
endif

ifeq ($(APIDIFF_DIR),)
APIDIFF_DIR=temp
endif
ifeq ($(OUTPUT_DIR),)
OUTPUT_DIR=output
endif


MONO_API_INFO = $(API_TOOLS_PATH)/mono-api-info/bin/Debug/net6.0/mono-api-info.dll
MONO_API_HTML = $(API_TOOLS_PATH)/mono-api-html/bin/Debug/net6.0/mono-api-html.dll
MONO_BUILD = $(SYSTEM_MONO)

MONO_API_INFO_EXEC = $(SYSTEM_DOTNET) $(MONO_API_INFO) --ignore-inherited-interfaces
MONO_API_HTML_EXEC = $(SYSTEM_DOTNET) $(MONO_API_HTML)

export HTML_BREAKING_CHANGES_MESSAGE=❗️Breaking changes❗️
export HTML_NO_BREAKING_CHANGES_MESSAGE=No breaking changes
export MARKDOWN_BREAKING_CHANGES_MESSAGE=:heavy_exclamation_mark: Breaking changes :heavy_exclamation_mark:
export MARKDOWN_NO_BREAKING_CHANGES_MESSAGE=No breaking changes

# I18N are excluded - but otherwise if should be like ../../builds/Makefile + what XI adds
# in the order to the api-diff.html merged file
MONO_ASSEMBLIES = mscorlib System System.Core System.Numerics\
	System.ComponentModel.DataAnnotations System.ComponentModel.Composition \
	System.Data System.Data.Services.Client \
	System.Json System.Runtime.Serialization System.ServiceModel System.ServiceModel.Web System.Web.Services \
	System.Xml System.Xml.Linq System.Xml.Serialization \
	System.IO.Compression System.IO.Compression.FileSystem System.Net System.Transactions System.Windows \
	Mono.Data.Sqlite Mono.Data.Tds Mono.Security

# this lists the mono-BCL assemblies (without extension) as they appear installed
# (relative to /Library/Frameworks/Xamarin.[Mac|iOS].framework/Versions/Current/lib/mono)

IOS_SRC_ASSEMBLIES     = \
	Xamarin.iOS/MonoTouch.Dialog-1 Xamarin.iOS/MonoTouch.NUnitLite Xamarin.iOS/OpenTK-1.0 Xamarin.iOS/System.Net.Http Xamarin.iOS/Xamarin.iOS
MAC_SRC_ASSEMBLIES     = \
	Xamarin.Mac/Xamarin.Mac Xamarin.Mac/OpenTK \
	4.5/Xamarin.Mac 4.5/OpenTK
WATCHOS_SRC_ASSEMBLIES = Xamarin.WatchOS/Xamarin.WatchOS Xamarin.WatchOS/MonoTouch.NUnitLite Xamarin.WatchOS/System.Net.Http
TVOS_SRC_ASSEMBLIES    = Xamarin.TVOS/Xamarin.TVOS Xamarin.TVOS/MonoTouch.Dialog-1 Xamarin.TVOS/MonoTouch.NUnitLite Xamarin.TVOS/OpenTK-1.0 Xamarin.TVOS/System.Net.Http

IOS_ASSEMBLIES     = $(foreach file,$(MONO_ASSEMBLIES),Xamarin.iOS/$(file))    $(IOS_SRC_ASSEMBLIES)
MAC_ASSEMBLIES     = $(foreach file,$(MONO_ASSEMBLIES),Xamarin.Mac/$(file))     $(MAC_SRC_ASSEMBLIES)
WATCHOS_ASSEMBLIES = $(foreach file,$(filter-out Mono.Data.Tds Mono.Security,$(MONO_ASSEMBLIES)),Xamarin.WatchOS/$(file)) $(WATCHOS_SRC_ASSEMBLIES)
TVOS_ASSEMBLIES    = $(foreach file,$(MONO_ASSEMBLIES),Xamarin.TVOS/$(file))    $(TVOS_SRC_ASSEMBLIES)
DOTNET_LEGACY_ASSEMBLIES = Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS \
	Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS
DOTNET_ASSEMBLIES = $(foreach platform,$(DOTNET_PLATFORMS),Microsoft.$(platform).Ref/ref/$(DOTNET_TFM)/Microsoft.$(platform))

IOS_ARCH_ASSEMBLIES = native-32/Xamarin.iOS native-64/Xamarin.iOS
MAC_ARCH_ASSEMBLIES =                       native-64/Xamarin.Mac

APIDIFF_IGNORE = -i 'INSObjectProtocol'

$(MONO_API_INFO): $(wildcard $(API_TOOLS_PATH)/mono-api-info/*.cs*)
	$(Q) $(SYSTEM_DOTNET) build $(API_TOOLS_PATH)/mono-api-info/mono-api-info.csproj /bl:$@.binlog $(MSBUILD_VERBOSITY)
	$(Q) touch $@

$(MONO_API_HTML): $(wildcard $(API_TOOLS_PATH)/mono-api-html/*.cs*)
	$(Q) $(SYSTEM_DOTNET) build $(API_TOOLS_PATH)/mono-api-html/mono-api-html.csproj /bl:$@.binlog $(MSBUILD_VERBOSITY)
	$(Q) touch $@

# create api info. Directory hierarchy is based on installed hierarchy
# (XM goes into temp/xm, and XI goes into temp/xi)

$(APIDIFF_DIR)/temp/xi/%.xml: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/%.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $@)

$(APIDIFF_DIR)/temp/xm/%.xml: $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/%.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_INFO_EXEC) -d $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac $(abspath $<) -o $(abspath $@)

$(APIDIFF_DIR)/temp/xm/4.5/Xamarin.Mac.xml: $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Xamarin.Mac.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_INFO_EXEC) -d $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5 $(abspath $<) -o $(abspath $@)

$(APIDIFF_DIR)/temp/native-%/Xamarin.Mac.xml: $(TOP)/src/build/mac/mobile-%/Xamarin.Mac.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_INFO_EXEC) -d $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac $(abspath $<) -o $(abspath $@)

$(APIDIFF_DIR)/temp/native-%/Xamarin.iOS.xml: $(TOP)/src/build/ios/native-%/Xamarin.iOS.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $@)

$(APIDIFF_DIR)/temp/dotnet/legacy-diff/%.xml: $(DOTNET_DESTDIR)/%.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $@)

$(APIDIFF_DIR)/temp/dotnet/%.xml: $(DOTNET_DESTDIR)/%.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $@)

# create diff from api info and reference info
# note that we create an empty file (the 'touch' command)
# so that we get a file in all cases (so that we don't have 
# to run mono-api-html every time even if none of the
# dependencies changed)

$(OUTPUT_DIR)/diff/%.html $(OUTPUT_DIR)/diff/%.md: $(APIDIFF_DIR)/temp/%.xml $(APIDIFF_DIR)/references/%.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $(APIDIFF_DIR)/references/$*.xml) $(abspath $(APIDIFF_DIR)/temp/$*.xml) $(APIDIFF_IGNORE) --html "$(abspath $(basename $@).html)" --markdown "$(abspath $(basename $@).md)"
	$(Q) touch $@

$(APIDIFF_DIR)/references/xi/Xamarin.iOS/Xamarin.iOS-converted.xml: $(APIDIFF_DIR)/references/xi/Xamarin.iOS/Xamarin.iOS.xml
	$(Q) sed 's/assembly name="Xamarin.iOS"/assembly name="Microsoft.iOS"/' $< > $@

$(APIDIFF_DIR)/references/xm/Xamarin.Mac/Xamarin.Mac-converted.xml: $(APIDIFF_DIR)/references/xm/Xamarin.Mac/Xamarin.Mac.xml
	$(Q) sed 's/assembly name="Xamarin.Mac"/assembly name="Microsoft.macOS"/' $< > $@

$(APIDIFF_DIR)/references/xi/Xamarin.TVOS/Xamarin.TVOS-converted.xml: $(APIDIFF_DIR)/references/xi/Xamarin.TVOS/Xamarin.TVOS.xml
	$(Q) sed 's/assembly name="Xamarin.TVOS"/assembly name="Microsoft.tvOS"/' $< > $@

$(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst-as-iOS.xml: $(APIDIFF_DIR)/temp/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst.xml
	$(Q) sed -e 's/assembly name="Microsoft.MacCatalyst"/assembly name="Microsoft.iOS"/' $< > $@

$(OUTPUT_DIR)/diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/%.html $(OUTPUT_DIR)/diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/%.md: $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.xml $(APIDIFF_DIR)/references/xi/Xamarin.iOS/Xamarin.iOS-converted.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $(APIDIFF_DIR)/references/xi/Xamarin.iOS/Xamarin.iOS-converted.xml) $(abspath $<) $(APIDIFF_IGNORE) --html $(abspath $(basename $@).html) --markdown $(abspath $(basename $@).md)
	$(Q) touch $@

$(OUTPUT_DIR)/diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/%.html $(OUTPUT_DIR)/diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/%.md: $(APIDIFF_DIR)/temp/dotnet/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.xml $(APIDIFF_DIR)/references/xm/Xamarin.Mac/Xamarin.Mac-converted.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $(APIDIFF_DIR)/references/xm/Xamarin.Mac/Xamarin.Mac-converted.xml) $(abspath $<) $(APIDIFF_IGNORE) --html $(abspath $(basename $@).html) --markdown $(abspath $(basename $@).md)
	$(Q) touch $@

$(OUTPUT_DIR)/diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/%.html $(OUTPUT_DIR)/diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/%.md: $(APIDIFF_DIR)/temp/dotnet/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.xml $(APIDIFF_DIR)/references/xi/Xamarin.TVOS/Xamarin.TVOS-converted.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $(APIDIFF_DIR)/references/xi/Xamarin.TVOS/Xamarin.TVOS-converted.xml) $(abspath $<) $(APIDIFF_IGNORE) --html $(abspath $(basename $@).html) --markdown $(abspath $(basename $@).md)
	$(Q) touch $@

$(OUTPUT_DIR)/diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/%.html $(OUTPUT_DIR)/diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/%.md: $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.xml $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst-as-iOS.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $<) $(abspath $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst-as-iOS.xml) $(APIDIFF_IGNORE) --html $(abspath $(basename $@).html) --markdown $(abspath $(basename $@).md)

# create diff files for all the assemblies per platform

$(OUTPUT_DIR)/mac-api-diff.html:     $(foreach file,$(MAC_ASSEMBLIES),$(OUTPUT_DIR)/diff/xm/$(file).html)
$(OUTPUT_DIR)/ios-api-diff.html:     $(foreach file,$(IOS_ASSEMBLIES),$(OUTPUT_DIR)/diff/xi/$(file).html)
$(OUTPUT_DIR)/watchos-api-diff.html: $(foreach file,$(WATCHOS_ASSEMBLIES),$(OUTPUT_DIR)/diff/xi/$(file).html)
$(OUTPUT_DIR)/tvos-api-diff.html:    $(foreach file,$(TVOS_ASSEMBLIES),$(OUTPUT_DIR)/diff/xi/$(file).html)

$(OUTPUT_DIR)/%-api-diff.html:
	$(Q) rm -f $@
	$(Q) mkdir -p $$(dirname "$@");
	$(Q) touch $@-toc
	$(Q_GEN) for file in $?; do \
		if [[ "x0" != "x`stat -L -f %z $$file`" ]]; then  \
			cat $$file | sed "s*<h1>*<h1 id='$$file'>*" >> $@;	\
			echo "<br><hr>" >> $@;	\
			echo "<a href='#$$file'>`echo $$file | sed -e 's_html_dll_' -e 's_diff/xi/Xamarin.iOS/__' -e 's_diff/xi/Xamarin.WatchOS/__' -e 's_diff/xi/Xamarin.TVOS/__' -e 's_diff/xm/4.5/_\(Full profile\) _' -e 's_diff/xm/Xamarin.Mac/_\(Mobile profile\) _' -e 's_diff/xm/_\(Classic profile\) _' -e 's_diff/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/__' -e 's_diff/dotnet/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/__' -e 's_diff/dotnet/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/__' -e 's_diff/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/__' -e 's_diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/__' -e 's_diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/__' -e 's_diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/__' -e 's_diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/__' -e 's#/Users/.*apidiff/##' `</a><br/>" >> $@-toc; \
		fi; \
	done
	$(Q) if [ ! -f $@ ]; then \
		echo "No change detected" > $@;	\
	fi;
	$(Q) echo "<h1>API diff</h1>" > $@.temp
	$(Q) cat $@-toc >> $@.temp
	$(Q) echo "<br><hr>" >> $@.temp
	$(Q) cat $@ >> $@.temp
	$(Q) mv $@.temp $@
	$(Q) rm -f $@-toc

ifdef INCLUDE_MAC
API_DIFF_DEPENDENCIES += $(OUTPUT_DIR)/mac-api-diff.html
endif
ifdef INCLUDE_IOS
API_DIFF_DEPENDENCIES += $(OUTPUT_DIR)/ios-api-diff.html
ifdef INCLUDE_WATCH
API_DIFF_DEPENDENCIES += $(OUTPUT_DIR)/watchos-api-diff.html
endif
ifdef INCLUDE_TVOS
API_DIFF_DEPENDENCIES += $(OUTPUT_DIR)/tvos-api-diff.html
endif
ifdef INCLUDE_MACCATALYST
ifdef ENABLE_DOTNET
API_DIFF_DEPENDENCIES += $(OUTPUT_DIR)/diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst.html
endif
endif
endif # INCLUDE_IOS
ifdef ENABLE_DOTNET
API_DIFF_DEPENDENCIES += $(foreach assembly,$(DOTNET_ASSEMBLIES),$(OUTPUT_DIR)/diff/dotnet/$(assembly).html)
API_DIFF_DEPENDENCIES += $(foreach assembly,$(DOTNET_LEGACY_ASSEMBLIES),$(OUTPUT_DIR)/diff/dotnet/legacy-diff/$(assembly).html)
endif

$(OUTPUT_DIR)/index.html: $(OUTPUT_DIR)/api-diff.html
	$(Q) $(CP) $< $@

#
# Collect all the diffs into a single api-diff.html file
#

define ApiDiffReportHtml
	$(Q) ./report-status.sh "$(1)" "$(OUTPUT_DIR)" html "diff/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).html" "diff/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).md" "$(2)"

endef

$(OUTPUT_DIR)/api-diff.html: $(API_DIFF_DEPENDENCIES)
	$(QF_GEN) echo "<h1>API diffs</h1>" > $@
ifdef INCLUDE_IOS
	$(Q) ./report-status.sh "iOS" "$(OUTPUT_DIR)" html "ios-api-diff.html" "ios-api-diff.md" "$@"
endif
ifdef INCLUDE_TVOS
	$(Q) ./report-status.sh "tvOS" "$(OUTPUT_DIR)" html "tvos-api-diff.html" "tvos-api-diff.md" "$@"
endif
ifdef INCLUDE_WATCH
	$(Q) ./report-status.sh "watchOS" "$(OUTPUT_DIR)" html "watchos-api-diff.html" "watchos-api-diff.md" "$@"
endif
ifdef INCLUDE_MAC
	$(Q) ./report-status.sh "macOS" "$(OUTPUT_DIR)" html "mac-api-diff.html" "mac-api-diff.md" "$@"
endif

	@# New .NET vs Stable .NET
	$(Q) $(foreach platform,$(DOTNET_PLATFORMS),$(call ApiDiffReportHtml,$(platform),$@))

	@# .NET vs legacy (stable)
ifdef INCLUDE_IOS
	$(Q) echo "<h2><a href='dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS-api-diff.html'>Microsoft.iOS (.NET) vs Xamarin.iOS (stable) API diff</a></h2>" >> $@
endif
ifdef INCLUDE_TVOS
	$(Q) echo "<h2><a href='dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS-api-diff.html'>Microsoft.tvOS (.NET) vs Xamarin.TVOS (stable) API diff</a></h2>" >> $@
endif
ifdef INCLUDE_MAC
	$(Q) echo "<h2><a href='dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS-api-diff.html'>Microsoft.macOS (.NET) vs Xamarin.Mac (stable) API diff</a></h2>" >> $@
endif

	@# .NET: Microsoft.iOS vs Microsoft.MacCatalyst
ifdef INCLUDE_IOS
ifdef INCLUDE_MACCATALYST
	$(Q) echo "<h2><a href='dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst-api-diff.html'>Microsoft.iOS vs Microsoft.MacCatalyst API diff</a></h2>" >> $@
endif
endif

# easy-to-type helper targets.
# one rule to create all the api diffs

all-local:: $(OUTPUT_DIR)/index.html $(OUTPUT_DIR)/api-diff.md all-markdowns

# Rules to re-create the reference infos from the current stable 'bundle.zip' assemblies

# First download the bundle zips we need. Multiple platforms may (or may not) share the same bundle.zip (with the same url),
# so account for that and only download each bundle.zip once.
APIDIFF_URLS=$(APIDIFF_REFERENCES_iOS) $(APIDIFF_REFERENCES_Mac) $(foreach platform,$(DOTNET_PLATFORMS),$(APIDIFF_REFERENCES_DOTNET_$(platform)))
APIDIFF_UNIQUE_URLS=$(sort $(APIDIFF_URLS))
APIDIFF_UNIQUE_HASHES=$(foreach url,$(APIDIFF_UNIQUE_URLS),$(word 5,$(subst /, ,$(url))))

define DownloadBundle
BUNDLE_ZIP_$(1)=$(APIDIFF_DIR)/bundle-$(1).zip
BUNDLE_ZIP_$(1)_URL=$(shell echo $(APIDIFF_UNIQUE_URLS) | tr ' ' '\n' | grep '/$(1)/')
$$(BUNDLE_ZIP_$(1)):
	@# download to a temporary filename so interrupted downloads won't prevent re-downloads.
	@echo "Downloading $$(BUNDLE_ZIP_$(1)_URL)..."
	$$(Q) if test -f ~/Library/Caches/xamarin-macios/$$(notdir $$@); then \
		echo "Found a cached version of $$(notdir $$@) in ~/Library/Caches/xamarin-macios/$$(notdir $$@)."; \
		$$(CP) ~/Library/Caches/xamarin-macios/$$(notdir $$@) $$@.tmp; \
	else \
		curl -f -L $$(if $$(V),-v,-s) "$$(BUNDLE_ZIP_$(1)_URL)" --output $$@.tmp; \
		if [[ "x$$$$MACIOS_CACHE_DOWNLOADS" != "x" ]]; then \
			mkdir -p ~/Library/Caches/xamarin-macios/; \
			$$(CP) $$@.tmp ~/Library/Caches/xamarin-macios/"$$(notdir $$@)"; \
			echo "Cached the download of $$(notdir $$@) in ~/Library/Caches/xamarin-macios"; \
		fi; \
	fi
	$$(Q) mv $$@.tmp $$@

BUNDLE_ZIPS+=$$(BUNDLE_ZIP_$(1))
endef
$(foreach hash,$(APIDIFF_UNIQUE_HASHES),$(eval $(call DownloadBundle,$(hash))))

download: $(BUNDLE_ZIPS)

# Here we unzip the downloaded bundle.
define UnzipBundle
UNZIP_STAMP_$(1)=$(APIDIFF_DIR)/.unzip.$(1).stamp
UNZIP_DIR_$(1)=temp/downloads/$(1)
$$(UNZIP_STAMP_$(1)): $$(BUNDLE_ZIP_$(1))
	$$(Q) rm -Rf "$$(UNZIP_DIR_$(1))"
	$$(Q) mkdir -p $$(dir $$(UNZIP_DIR_$(1)))
	$$(Q_GEN) unzip $$(if $$(V),,-q) -d $$(UNZIP_DIR_$(1)) $$<
	$$(Q) touch $$@

# the semi-colon at the end means an empty recipe, and is required for make to consider pattern rules
$$(UNZIP_DIR_$(1))/%.dll: $$(UNZIP_STAMP_$(1)) ;

UNZIP_STAMPS+=$$(UNZIP_STAMP_$(1))
endef
$(foreach hash,$(APIDIFF_UNIQUE_HASHES),$(eval $(call UnzipBundle,$(hash))))

unzip: $(UNZIP_STAMPS)

# Compute the unzip dir per platform
APIDIFF_HASH_iOS=$(word 5,$(subst /, ,$(APIDIFF_REFERENCES_iOS)))
UNZIP_DIR_iOS=$(UNZIP_DIR_$(APIDIFF_HASH_iOS))
UNZIP_STAMP_iOS=$(UNZIP_STAMP_$(APIDIFF_HASH_iOS))
APIDIFF_HASH_Mac=$(word 5,$(subst /, ,$(APIDIFF_REFERENCES_Mac)))
UNZIP_DIR_Mac=$(UNZIP_DIR_$(APIDIFF_HASH_Mac))
UNZIP_STAMP_Mac=$(UNZIP_STAMP_$(APIDIFF_HASH_Mac))

define DotNetUnzipDirectory
APIDIFF_HASH_DOTNET_$(1)=$$(word 5,$$(subst /, ,$$(APIDIFF_REFERENCES_DOTNET_$(1))))
UNZIP_DIR_DOTNET_$(1)=$$(UNZIP_DIR_$$(APIDIFF_HASH_DOTNET_$(1)))
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetUnzipDirectory,$(platform))))

# the semi-colon at the end means an empty recipe, and is required for make to consider pattern rules
$(UNZIP_DIR_Mac)/%.dll: $(UNZIP_STAMP_Mac) ;
$(UNZIP_DIR_iOS)/%.dll: $(UNZIP_STAMP_iOS) ;

IOS_REFS     = $(foreach file,$(IOS_ASSEMBLIES),$(APIDIFF_DIR)/updated-references/xi/$(file).xml)
MAC_REFS     = $(foreach file,$(MAC_ASSEMBLIES),$(APIDIFF_DIR)/updated-references/xm/$(file).xml)
WATCHOS_REFS = $(foreach file,$(WATCHOS_ASSEMBLIES),$(APIDIFF_DIR)/updated-references/xi/$(file).xml)
TVOS_REFS    = $(foreach file,$(TVOS_ASSEMBLIES),$(APIDIFF_DIR)/updated-references/xi/$(file).xml)
DOTNET_REFS = $(foreach file,$(DOTNET_ASSEMBLIES),$(APIDIFF_DIR)/updated-references/dotnet/$(file).xml)

$(APIDIFF_DIR)/references/xi/%.xml: $(UNZIP_DIR_iOS)/%.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@) $(dir $(APIDIFF_DIR)/references/xi/$*)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $(APIDIFF_DIR)/references/xi/$*.xml)

$(APIDIFF_DIR)/updated-references/xi/%.xml: $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/%.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@) $(dir $(APIDIFF_DIR)/references/xi/$*)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $(APIDIFF_DIR)/references/xi/$*.xml)

$(APIDIFF_DIR)/references/xm/%.xml: $(UNZIP_DIR_Mac)/%.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@) $(dir $(APIDIFF_DIR)/references/xm/$*)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $(APIDIFF_DIR)/references/xm/$*.xml)

$(APIDIFF_DIR)/updated-references/xm/%.xml: $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/%.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@) $(dir $(APIDIFF_DIR)/references/xm/$*)
	$(QF_GEN) $(MONO_API_INFO_EXEC) -d $(dir $<) $(abspath $<) -o $(abspath $(APIDIFF_DIR)/references/xm/$*.xml)

# The dotnet references xmls may come from different hashes, so we need to have separate rules for all of them
$(APIDIFF_DIR)/references/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.xml: $(UNZIP_DIR_DOTNET_iOS)/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.dll  $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $@)

$(APIDIFF_DIR)/references/dotnet/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.xml: $(UNZIP_DIR_DOTNET_macOS)/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.dll  $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $@)

$(APIDIFF_DIR)/references/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst.xml: $(UNZIP_DIR_DOTNET_MacCatalyst)/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst.dll  $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $@)

$(APIDIFF_DIR)/references/dotnet/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.xml: $(UNZIP_DIR_DOTNET_tvOS)/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.dll  $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $@)

$(APIDIFF_DIR)/updated-references/dotnet/%.xml: $(DOTNET_DESTDIR)/%.dll $(MONO_API_INFO)
	$(Q) mkdir -p $(dir $@) $(dir $(APIDIFF_DIR)/references/dotnet/$*)
	$(QF_GEN) $(MONO_API_INFO_EXEC) $(abspath $<) -o $(abspath $(APIDIFF_DIR)/references/dotnet/$*.xml)

update-tvos-refs: $(TVOS_REFS)
update-watchos-refs: $(WATCHOS_REFS)
update-ios-refs: $(IOS_REFS)
update-mac-refs: $(MAC_REFS)
ifdef ENABLE_DOTNET
update-dotnet: $(DOTNET_REFS)
endif

update-refs: $(WATCHOS_REFS) $(TVOS_REFS) $(IOS_REFS) $(MAC_REFS)
ifdef ENABLE_DOTNET
update-refs: $(DOTNET_REFS)
endif

clean-local::
	rm -rf temp references updated-references diff dotnet *.exe* api-diff.html
	rm -rf *.dll* bundle-*.zip $(UNZIP_STAMPS)
	rm -rf ios-*.md tvos-*.md watchos-*.md macos-*.md dotnet-*.md

DIRS += $(APIDIFF_DIR)/temp $(OUTPUT_DIR)/diff

# dir creation target
$(DIRS):
	$(Q) mkdir -p $@

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# defining a .SECONDARY rule will prevent that deletion.
.SECONDARY:

merger.exe: merger.cs
	$(Q) $(SYSTEM_CSC) -debug $< -out:$@ /nologo

ifdef INCLUDE_IOS
$(OUTPUT_DIR)/ios-api-diff.md: merger.exe $(foreach file,$(IOS_ASSEMBLIES),$(OUTPUT_DIR)/diff/xi/$(file).md)
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.iOS $(OUTPUT_DIR)/diff/xi/Xamarin.iOS/ ios $(abspath $@)
all-markdowns:: $(OUTPUT_DIR)/ios-api-diff.md
endif

ifdef INCLUDE_TVOS
$(OUTPUT_DIR)/tvos-api-diff.md: merger.exe $(foreach file,$(TVOS_ASSEMBLIES),$(OUTPUT_DIR)/diff/xi/$(file).md)
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.TVOS $(OUTPUT_DIR)/diff/xi/Xamarin.TVOS/ tvos $(abspath $@)
all-markdowns:: $(OUTPUT_DIR)/tvos-api-diff.md
endif

ifdef INCLUDE_WATCH
$(OUTPUT_DIR)/watchos-api-diff.md: merger.exe $(foreach file,$(WATCHOS_ASSEMBLIES),$(OUTPUT_DIR)/diff/xi/$(file).md)
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.WatchOS $(OUTPUT_DIR)/diff/xi/Xamarin.WatchOS/ watchos $(abspath $@)
all-markdowns:: $(OUTPUT_DIR)/watchos-api-diff.md
endif

ifdef INCLUDE_MAC
$(OUTPUT_DIR)/mac-api-diff.md: merger.exe $(foreach file,$(MAC_ASSEMBLIES),$(OUTPUT_DIR)/diff/xm/$(file).md)
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.Mac $(OUTPUT_DIR)/diff/xm/Xamarin.Mac/ macos $(abspath $@)
all-markdowns:: $(OUTPUT_DIR)/mac-api-diff.md
endif
