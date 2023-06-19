TOP = ../..

include $(TOP)/Make.config
include $(TOP)/mk/colors.mk

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

ifndef SKIP_XAMARIN_VS_DOTNET
INCLUDE_XAMARIN_VS_DOTNET=1
endif
ifndef SKIP_IOS_VS_MACCATALYST
INCLUDE_IOS_VS_MACCATALYST=1
endif

MONO_API_INFO = $(API_TOOLS_PATH)/mono-api-info/bin/Debug/net6.0/mono-api-info.dll
MONO_API_HTML = $(API_TOOLS_PATH)/mono-api-html/bin/Debug/net6.0/mono-api-html.dll
MONO_BUILD = $(SYSTEM_MONO)

MONO_API_INFO_EXEC = $(DOTNET) --roll-forward Major $(MONO_API_INFO) --ignore-inherited-interfaces
MONO_API_HTML_EXEC = $(DOTNET) --roll-forward Major $(MONO_API_HTML)

export HTML_BREAKING_CHANGES_MESSAGE=❗️Breaking changes❗️
export HTML_NO_BREAKING_CHANGES_MESSAGE=No breaking changes
export MARKDOWN_BREAKING_CHANGES_MESSAGE=:heavy_exclamation_mark: Breaking changes :heavy_exclamation_mark:
export MARKDOWN_NO_BREAKING_CHANGES_MESSAGE=No breaking changes

# Change the below to net7.0 once we have reference assemblies from net7.0 (i.e. once net7.0 goes stable).
DOTNET_TFM_REFERENCE_iOS=net6.0
DOTNET_TFM_REFERENCE_tvOS=net6.0
DOTNET_TFM_REFERENCE_macOS=net6.0
DOTNET_TFM_REFERENCE_MacCatalyst=net6.0

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
DOTNET_LEGACY_PLATFORMS = $(filter-out MacCatalyst,$(DOTNET_PLATFORMS))
DOTNET_LEGACY_ASSEMBLIES = $(foreach platform,$(DOTNET_LEGACY_PLATFORMS),Microsoft.$(platform).Ref/ref/$(DOTNET_TFM)/Microsoft.$(platform))
DOTNET_ASSEMBLIES = $(foreach platform,$(DOTNET_PLATFORMS),Microsoft.$(platform).Ref/ref/$(DOTNET_TFM)/Microsoft.$(platform))

IOS_ARCH_ASSEMBLIES = native-32/Xamarin.iOS native-64/Xamarin.iOS
MAC_ARCH_ASSEMBLIES =                       native-64/Xamarin.Mac

APIDIFF_IGNORE = -i 'INSObjectProtocol'

$(MONO_API_INFO): $(wildcard $(API_TOOLS_PATH)/mono-api-info/*.cs*)
	$(Q) $(DOTNET) build $(API_TOOLS_PATH)/mono-api-info/mono-api-info.csproj /bl:$@.binlog $(MSBUILD_VERBOSITY)
	$(Q) touch $@

$(MONO_API_HTML): $(wildcard $(API_TOOLS_PATH)/mono-api-html/*.cs*)
	$(Q) $(DOTNET) build $(API_TOOLS_PATH)/mono-api-html/mono-api-html.csproj /bl:$@.binlog $(MSBUILD_VERBOSITY)
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

define DotNetApiInfo
$(APIDIFF_DIR)/temp/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).xml: $(DOTNET_DESTDIR)/$($(1)_SDK_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).dll $(MONO_API_INFO)
	$$(Q) mkdir -p $$(dir $$@)
	$$(QF_GEN) $(MONO_API_INFO_EXEC) $$(abspath $$<) -o $$(abspath $$@)
endef

$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetApiInfo,$(platform))))

# create diff from api info and reference info
# note that we create an empty file (the 'touch' command)
# so that we get a file in all cases (so that we don't have 
# to run mono-api-html every time even if none of the
# dependencies changed)

define DotNetComputeDiff
$(OUTPUT_DIR)/diff/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1)%html $(OUTPUT_DIR)/diff/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1)%md: $(APIDIFF_DIR)/temp/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1)%xml $(APIDIFF_DIR)/references/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM_REFERENCE_$(1))/Microsoft.$(1).xml $(MONO_API_HTML)
	$$(Q) mkdir -p $$(dir $$@)
	$$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $$(abspath $(APIDIFF_DIR)/references/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM_REFERENCE_$(1))/Microsoft.$(1).xml) $$(abspath $(APIDIFF_DIR)/temp/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).xml) $(APIDIFF_IGNORE) --html "$$(abspath $$(basename $$@).html)" --markdown "$$(abspath $$(basename $$@).md)"
	$$(Q) touch $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetComputeDiff,$(platform))))

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
			echo "<a href='#$$file'>`echo $$file | sed -e 's_html_dll_' -e 's_diff/xi/Xamarin.iOS/__' -e 's_diff/xi/Xamarin.WatchOS/__' -e 's_diff/xi/Xamarin.TVOS/__' -e 's_diff/xm/4.5/_\(Full profile\) _' -e 's_diff/xm/Xamarin.Mac/_\(Mobile profile\) _' -e 's_diff/xm/_\(Classic profile\) _' -e 's_diff/dotnet/Microsoft.iOS.Ref/ref/net[0-9].[0-9]/__' -e 's_diff/dotnet/Microsoft.tvOS.Ref/ref/net[0-9].[0-9]/__' -e 's_diff/dotnet/Microsoft.macOS.Ref/ref/net[0-9].[0-9]/__' -e 's_diff/dotnet/Microsoft.MacCatalyst.Ref/ref/net[0-9].[0-9]/__' -e 's_diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/net[0-9].[0-9]/__' -e 's_diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/net[0-9].[0-9]/__' -e 's_diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/net[0-9].[0-9]/__' -e 's_diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/net[0-9].[0-9]/__' -e 's#/Users/.*apidiff/##' `</a><br/>" >> $@-toc; \
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

ifdef INCLUDE_XAMARIN_LEGACY
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
endif
ifdef INCLUDE_MACCATALYST
ifdef ENABLE_DOTNET
ifdef INCLUDE_IOS_VS_MACCATALYST
API_DIFF_DEPENDENCIES += $(OUTPUT_DIR)/diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst.html
endif
endif
endif
endif # INCLUDE_IOS
ifdef ENABLE_DOTNET
API_DIFF_DEPENDENCIES += $(foreach assembly,$(DOTNET_ASSEMBLIES),$(OUTPUT_DIR)/diff/dotnet/$(assembly).html)
ifdef INCLUDE_XAMARIN_VS_DOTNET
API_DIFF_DEPENDENCIES += $(foreach assembly,$(DOTNET_LEGACY_ASSEMBLIES),$(OUTPUT_DIR)/diff/dotnet/legacy-diff/$(assembly).html)
endif
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

ifdef INCLUDE_XAMARIN_LEGACY
	$(Q) if $(foreach html,$(wildcard $(OUTPUT_DIR)/*-api-diff.html),! test -s "$(html)" &&) true; then \
		echo "<h2>Legacy Xamarin (empty diffs)</h2>" >> "$@"; \
	elif grep BreakingChangesDetected "$(OUTPUT_DIR)"/*-api-diff.html &> /dev/null; then \
		echo "<h2>Legacy Xamarin ($(HTML_BREAKING_CHANGES_MESSAGE))</h2>" >> "$@"; \
	else \
		echo "<h2>Legacy Xamarin ($(HTML_NO_BREAKING_CHANGES_MESSAGE))</h2>" >> "$@"; \
	fi
	$(Q) echo "<ul>" >> $@
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
	$(Q) echo "</ul>" >> $@
endif # INCLUDE_XAMARIN_LEGACY

ifdef ENABLE_DOTNET
	$(Q) if $(foreach html,$(wildcard $(OUTPUT_DIR)/diff/dotnet/Microsoft.*.Ref/ref/$(DOTNET_TFM)/Microsoft.*.html),! test -s "$(html)" &&) true; then \
		echo "<h2>.NET (empty diffs)</h2>" >> "$@"; \
	elif grep BreakingChangesDetected "$(OUTPUT_DIR)"/diff/dotnet/Microsoft.*.Ref/ref/"$(DOTNET_TFM)"/Microsoft.*.html &> /dev/null; then \
		echo "<h2>.NET ($(HTML_BREAKING_CHANGES_MESSAGE))</h2>" >> "$@"; \
	else \
		echo "<h2>.NET ($(HTML_NO_BREAKING_CHANGES_MESSAGE))</h2>" >> "$@"; \
	fi
	$(Q) echo "<ul>" >> $@

	@# New .NET vs Stable .NET
	$(Q) $(foreach platform,$(DOTNET_PLATFORMS),$(call ApiDiffReportHtml,$(platform),$@))

	@# .NET: Microsoft.iOS vs Microsoft.MacCatalyst
ifdef INCLUDE_IOS
ifdef INCLUDE_MACCATALYST
ifdef INCLUDE_IOS_VS_MACCATALYST
	$(Q) echo "<li>Microsoft.iOS vs Microsoft.MacCatalyst: <a href='diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst.html'>html</a> <a href='diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst.md'>markdown</a></li>" >> $@
endif
endif
endif
	$(Q) echo "</ul>" >> $@

ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_XAMARIN_VS_DOTNET
	$(Q) echo "<h2>Legacy Xamarin (stable) vs .NET</h2>" >> $@
	$(Q) echo "<ul>" >> $@
	@# .NET vs legacy (stable)
ifdef INCLUDE_IOS
	$(Q) echo "<li>iOS: <a href='diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.html'>html</a> <a href='diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.md'>markdown</a></li>" >> $@
endif
ifdef INCLUDE_TVOS
	$(Q) echo "<li>tvOS: <a href='diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.html'>html</a> <a href='diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.md'>markdown</a></li>" >> $@
endif
ifdef INCLUDE_MAC
	$(Q) echo "<li>macOS: <a href='diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.html'>html</a> <a href='diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.md'>markdown</a></li>" >> $@
endif
	$(Q) echo "</ul>" >> $@
endif # INCLUDE_XAMARIN_VS_DOTNET
endif # INCLUDE_XAMARIN_LEGACY

endif # DOTNET
	$(Q) if grep "$(HTML_BREAKING_CHANGES_MESSAGE)" $@ >/dev/null 2>&1; then \
		echo "" >> $@; \
		echo "<!-- BreakingChangesDetected -->" >> $@; \
		echo "" >> $@; \
	fi

#
# Collect all the diffs into a single api-diff.md file
#

define ApiDiffReportMarkdown
	$(Q) ./report-status.sh "$(1)" "$(OUTPUT_DIR)" markdown "diff/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).html" "diff/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).md" "$(2)"

endef

all-markdowns:: $(OUTPUT_DIR)/api-diff.md

$(OUTPUT_DIR)/api-diff.md: $(API_DIFF_DEPENDENCIES)
ifdef INCLUDE_XAMARIN_LEGACY
	$(Q) if $(foreach html,$(wildcard $(OUTPUT_DIR)/*-api-diff.html),! test -s "$(html)" &&) true; then \
		echo "<details><summary>Legacy Xamarin (empty diffs)</summary>" >> "$@"; \
	elif grep BreakingChangesDetected "$(OUTPUT_DIR)"/*-api-diff.html &> /dev/null; then \
		echo "<details><summary>Legacy Xamarin ($(MARKDOWN_BREAKING_CHANGES_MESSAGE))</summary>" >> "$@"; \
	else \
		echo "<details><summary>Legacy Xamarin ($(MARKDOWN_NO_BREAKING_CHANGES_MESSAGE))</summary>" >> "$@"; \
	fi
	$(Q) echo "" >> $@
ifdef INCLUDE_IOS
	$(Q) ./report-status.sh "iOS" "$(OUTPUT_DIR)" markdown "ios-api-diff.html" "ios-api-diff.md" "$@"
endif
ifdef INCLUDE_TVOS
	$(Q) ./report-status.sh "tvOS" "$(OUTPUT_DIR)" markdown "tvos-api-diff.html" "tvos-api-diff.md" "$@"
endif
ifdef INCLUDE_WATCH
	$(Q) ./report-status.sh "watchOS" "$(OUTPUT_DIR)" markdown "watchos-api-diff.html" "watchos-api-diff.md" "$@"
endif
ifdef INCLUDE_MAC
	$(Q) ./report-status.sh "macOS" "$(OUTPUT_DIR)" markdown "mac-api-diff.html" "mac-api-diff.md" "$@"
endif
	$(Q) echo "" >> $@
	$(Q) echo "</details>" >> "$@"
	$(Q) echo "" >> $@
endif # INCLUDE_XAMARIN_LEGACY

ifdef ENABLE_DOTNET
	$(Q) if $(foreach html,$(wildcard $(OUTPUT_DIR)/diff/dotnet/Microsoft.*.Ref/ref/$(DOTNET_TFM)/Microsoft.*.html),! test -s "$(html)" &&) true; then \
		echo "<details><summary>NET (empty diffs)</summary>" >> "$@"; \
	elif grep BreakingChangesDetected "$(OUTPUT_DIR)"/diff/dotnet/Microsoft.*.Ref/ref/"$(DOTNET_TFM)"/Microsoft.*.html &> /dev/null; then \
		echo "<details><summary>.NET ($(MARKDOWN_BREAKING_CHANGES_MESSAGE))</summary>" >> "$@"; \
	else \
		echo "<details><summary>.NET ($(MARKDOWN_NO_BREAKING_CHANGES_MESSAGE))</summary>" >> "$@"; \
	fi
	$(Q) echo "" >> $@

	@# New .NET vs Stable .NET
	$(Q) $(foreach platform,$(DOTNET_PLATFORMS),$(call ApiDiffReportMarkdown,$(platform),$@))

	@# .NET: Microsoft.iOS vs Microsoft.MacCatalyst
ifdef INCLUDE_IOS
ifdef INCLUDE_MACCATALYST
ifdef INCLUDE_IOS_VS_MACCATALYST
	$(Q) echo "* Microsoft.iOS vs Microsoft.MacCatalyst: [vsdrops](diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst.html) [gist](diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst.md)" >> $@
endif # INCLUDE_IOS_VS_MACCATALYST
endif # INCLUDE_MACCATALYST
endif # INCLUDE_IOS
	$(Q) echo "" >> $@
	$(Q) echo "</details>" >> "$@"
	$(Q) echo "" >> $@

	@# .NET vs legacy (stable)
ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_XAMARIN_VS_DOTNET
	$(Q) echo "<details><summary>Legacy Xamarin (stable) vs .NET</summary>" >> "$@"
	$(Q) echo "" >> $@
ifdef INCLUDE_IOS
	$(Q) echo "* iOS: [vsdrops](diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.html) [gist](diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.md)" >> $@
endif
ifdef INCLUDE_TVOS
	$(Q) echo "* tvOS: [vsdrops](diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.html) [gist](diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.md)" >> $@
endif
ifdef INCLUDE_MAC
	$(Q) echo "* macOS: [vsdrops](diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.html) [gist](diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.md)" >> $@
endif
	$(Q) echo "" >> $@
	$(Q) echo "</details>" >> "$@"
	$(Q) echo "" >> $@
endif # INCLUDE_XAMARIN_VS_DOTNET
endif # INCLUDE_XAMARIN_LEGACY

endif # DOTNET
	$(Q) if grep "$(MARKDOWN_BREAKING_CHANGES_MESSAGE)" $@ >/dev/null 2>&1; then \
		echo "" >> $@; \
		echo "<!-- BreakingChangesDetected -->" >> $@; \
		echo "" >> $@; \
	fi

# easy-to-type helper targets.
# one rule to create all the api diffs

all-local:: $(OUTPUT_DIR)/index.html $(OUTPUT_DIR)/api-diff.md

ifdef INCLUDE_XAMARIN_LEGACY
all-local:: all-markdowns
endif

# Rules to re-create the reference infos from the current stable 'bundle.zip' assemblies

# First download the bundle zips we need. Multiple platforms may (or may not) share the same bundle.zip (with the same url),
# so account for that and only download each bundle.zip once.
APIDIFF_URLS=$(APIDIFF_REFERENCES_iOS) $(APIDIFF_REFERENCES_Mac) $(foreach platform,$(DOTNET_PLATFORMS),$(APIDIFF_REFERENCES_DOTNET_$(platform)))
APIDIFF_UNIQUE_URLS=$(sort $(APIDIFF_URLS))
APIDIFF_UNIQUE_HASHES=$(foreach url,$(APIDIFF_UNIQUE_URLS),$(word 5,$(subst /, ,$(url))))

AUTH_TOKEN_GITHUB_COM_FILE=$(HOME)/.config/AUTH_TOKEN_GITHUB_COM
ifeq ($(AUTH_TOKEN_GITHUB_COM),)
ifeq ($(AUTH_TOKEN_GITHUB_COM_FILE),$(shell ls -1 $(AUTH_TOKEN_GITHUB_COM_FILE 2>/dev/null)))
AUTH_TOKEN_GITHUB_COM:=$(shell cat $(AUTH_TOKEN_GITHUB_COM_FILE))
endif
endif

check-token:
	@if test -z "$(AUTH_TOKEN_GITHUB_COM)"; then echo "$(COLOR_RED)Can't download API references because the environment variable $(COLOR_BLUE)AUTH_TOKEN_GITHUB_COM$(COLOR_RED) isn't set. Please see the README.md file for more information.$(COLOR_CLEAR)"; exit 1; fi

.PHONY: check-token

define DownloadBundle
BUNDLE_ZIP_$(1)=$(APIDIFF_DIR)/bundle-$(1).zip
BUNDLE_ZIP_$(1)_URL=$(shell echo $(APIDIFF_UNIQUE_URLS) | tr ' ' '\n' | grep '/$(1)/')
$$(BUNDLE_ZIP_$(1)):
	$(Q) mkdir -p $$(dir $$@)
	@# download to a temporary filename so interrupted downloads won't prevent re-downloads.
	@echo "Downloading $$(BUNDLE_ZIP_$(1)_URL)..."
	$$(Q) if test -f ~/Library/Caches/xamarin-macios/$$(notdir $$@); then \
		echo "Found a cached version of $$(notdir $$@) in ~/Library/Caches/xamarin-macios/$$(notdir $$@)."; \
		$$(CP) ~/Library/Caches/xamarin-macios/$$(notdir $$@) $$@.tmp; \
	else \
		$(MAKE) check-token || exit 1; \
		$(CURL_RETRY) -H "Authorization: token $(AUTH_TOKEN_GITHUB_COM)" "$$(BUNDLE_ZIP_$(1)_URL)" --output $$@.tmp; \
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

define DotNetGenerateReferenceXml
$(APIDIFF_DIR)/references/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM_REFERENCE_$(1))/Microsoft.$(1).xml: $(UNZIP_DIR_DOTNET_$(1))/Microsoft.$(1).Ref/ref/$(DOTNET_TFM_REFERENCE_$(1))/Microsoft.$(1).dll  $(MONO_API_INFO)
	$$(Q) mkdir -p $$(dir $$@)
	$$(QF_GEN) $(MONO_API_INFO_EXEC) $$(abspath $$<) -o $$(abspath $$@)
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetGenerateReferenceXml,$(platform))))

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

ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_IOS
update-refs: $(IOS_REFS)
endif
ifdef INCLUDE_TVOS
update-refs: $(TVOS_REFS)
endif
ifdef INCLUDE_WATCH
update-refs: $(WATCHOS_REFS)
endif
ifdef INCLUDE_MAC
update-refs: $(MAC_REFS)
endif
endif

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

ifdef INCLUDE_XAMARIN_LEGACY
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
endif
