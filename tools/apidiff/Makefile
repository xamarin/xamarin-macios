TOP = ../..

include $(TOP)/Make.config

ifdef SKIP_NEW_APIS #Requires https://github.com/mono/mono/commit/4c6a463678f3f0bfa599caeb66de72c7217fa95d
NEW_REGEX = "-n:.?"
endif

ifdef SKIP_ADDED_APIS
ADD_REGEX = "-a:.?"
endif

APIDIFF_DIR=.

MONO_API_INFO = $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_tools/mono-api-info.exe
MONO_API_HTML = $(MONO_IOS_SDK_DESTDIR)/ios-bcl/monotouch_tools/mono-api-html.exe
MONO_BUILD = $(SYSTEM_MONO)

MONO_API_INFO_EXEC = $(MONO_BUILD) --debug $(MONO_API_INFO)
MONO_API_HTML_EXEC = $(MONO_BUILD) --debug $(MONO_API_HTML)

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

$(APIDIFF_DIR)/.download-$(MONO_HASH).stamp:
	$(MAKE) -C $(TOP)/builds download
	$(Q) mkdir -p $(dir $@)
	$(Q) touch $@

$(MONO_API_INFO) $(MONO_API_HTML): $(APIDIFF_DIR)/.download-$(MONO_HASH).stamp

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

$(APIDIFF_DIR)/diff/%.html: $(APIDIFF_DIR)/temp/%.xml $(APIDIFF_DIR)/references/%.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $(APIDIFF_DIR)/references/$*.xml) $(abspath $(APIDIFF_DIR)/temp/$*.xml) $(APIDIFF_IGNORE) $(abspath $@)
	$(Q) touch $@

$(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.html: $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.xml $(APIDIFF_DIR)/references/xi/Xamarin.iOS/Xamarin.iOS.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $(APIDIFF_DIR)/references/xi/Xamarin.iOS/Xamarin.iOS.xml) $(abspath $<) $(APIDIFF_IGNORE) $(abspath $@)
	$(Q) touch $@

$(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.html: $(APIDIFF_DIR)/temp/dotnet/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.xml $(APIDIFF_DIR)/references/xm/Xamarin.Mac/Xamarin.Mac.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $(APIDIFF_DIR)/references/xm/Xamarin.Mac/Xamarin.Mac.xml) $(abspath $<) $(APIDIFF_IGNORE) $(abspath $@)
	$(Q) touch $@

$(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.html: $(APIDIFF_DIR)/temp/dotnet/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.xml $(APIDIFF_DIR)/references/xi/Xamarin.TVOS/Xamarin.TVOS.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $(APIDIFF_DIR)/references/xi/Xamarin.TVOS/Xamarin.TVOS.xml) $(abspath $<) $(APIDIFF_IGNORE) $(abspath $@)
	$(Q) touch $@

$(APIDIFF_DIR)/diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst.html: $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.xml $(APIDIFF_DIR)/temp/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(Q) sed -e 's_<assembly name="Xamarin.MacCatalyst" version="0.0.0.0">_<assembly name="Xamarin.iOS" version="0.0.0.0">_' $(APIDIFF_DIR)/temp/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst.xml > $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst-as-iOS.xml
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $<) $(abspath $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst-as-iOS.xml) $(APIDIFF_IGNORE) $(abspath $@)

# this is a hack to show the difference between iOS and tvOS
$(APIDIFF_DIR)/diff/ios-to-tvos.html: $(APIDIFF_DIR)/temp/xi/Xamarin.iOS/Xamarin.iOS.xml $(APIDIFF_DIR)/temp/xi/Xamarin.TVOS/Xamarin.TVOS.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(Q) sed -e 's_<assembly name="Xamarin.TVOS" version="0.0.0.0">_<assembly name="Xamarin.iOS" version="0.0.0.0">_' $(APIDIFF_DIR)/temp/xi/Xamarin.TVOS/Xamarin.TVOS.xml > $(APIDIFF_DIR)/temp/Xamarin.TVOS-as-iOS.xml
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(abspath $<) $(abspath $(APIDIFF_DIR)/temp/Xamarin.TVOS-as-iOS.xml) $(abspath $@)

# create diff files for all the assemblies per platform

$(APIDIFF_DIR)/mac-api-diff.html:     $(foreach file,$(MAC_ASSEMBLIES),$(APIDIFF_DIR)/diff/xm/$(file).html)
$(APIDIFF_DIR)/ios-api-diff.html:     $(foreach file,$(IOS_ASSEMBLIES),$(APIDIFF_DIR)/diff/xi/$(file).html)
$(APIDIFF_DIR)/watchos-api-diff.html: $(foreach file,$(WATCHOS_ASSEMBLIES),$(APIDIFF_DIR)/diff/xi/$(file).html)
$(APIDIFF_DIR)/tvos-api-diff.html:    $(foreach file,$(TVOS_ASSEMBLIES),$(APIDIFF_DIR)/diff/xi/$(file).html)

# Compare new dotnet vs. stable dotnet
$(APIDIFF_DIR)/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS-api-diff.html:                 $(APIDIFF_DIR)/diff/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.html
$(APIDIFF_DIR)/dotnet/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS-api-diff.html:               $(APIDIFF_DIR)/diff/dotnet/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.html
$(APIDIFF_DIR)/dotnet/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS-api-diff.html:               $(APIDIFF_DIR)/diff/dotnet/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.html
$(APIDIFF_DIR)/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst-api-diff.html: $(APIDIFF_DIR)/diff/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst.html

# Compare new dotnet vs. stable legacy
$(APIDIFF_DIR)/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS-api-diff.html:     $(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.html
$(APIDIFF_DIR)/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS-api-diff.html:   $(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.html
$(APIDIFF_DIR)/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS-api-diff.html:   $(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.html

# Compare Dotnet iOS vs. Dotnet MacCatalyst
$(APIDIFF_DIR)/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst-api-diff.html: $(APIDIFF_DIR)/diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst.html

$(APIDIFF_DIR)/%-api-diff.html:
	$(Q) rm -f $@
	mkdir -p $$(dirname "$@");
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
API_DIFF_DEPENDENCIES += $(APIDIFF_DIR)/mac-api-diff.html
endif
ifdef INCLUDE_IOS
API_DIFF_DEPENDENCIES += $(APIDIFF_DIR)/ios-api-diff.html
ifdef INCLUDE_WATCH
API_DIFF_DEPENDENCIES += $(APIDIFF_DIR)/watchos-api-diff.html
endif
ifdef INCLUDE_TVOS
API_DIFF_DEPENDENCIES += $(APIDIFF_DIR)/tvos-api-diff.html
API_DIFF_DEPENDENCIES += $(APIDIFF_DIR)/diff/ios-to-tvos.html
endif
ifdef INCLUDE_MACCATALYST
ifdef ENABLE_DOTNET
API_DIFF_DEPENDENCIES += $(APIDIFF_DIR)/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst-api-diff.html
endif
endif
endif # INCLUDE_IOS
ifdef ENABLE_DOTNET
API_DIFF_DEPENDENCIES += $(foreach assembly,$(DOTNET_ASSEMBLIES),$(APIDIFF_DIR)/dotnet/$(assembly)-api-diff.html)
API_DIFF_DEPENDENCIES += $(foreach assembly,$(DOTNET_LEGACY_ASSEMBLIES),$(APIDIFF_DIR)/dotnet/legacy-diff/$(assembly)-api-diff.html)
endif

$(APIDIFF_DIR)/api-diff.html: $(API_DIFF_DEPENDENCIES)
	$(QF_GEN) echo "<h1>API diffs</h1>" > $@
ifdef INCLUDE_IOS
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/ios-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='ios-api-diff.html'>Xamarin.iOS API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Xamarin.iOS API diff is empty</h2>" >> $@; \
	fi;
ifdef INCLUDE_TVOS
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/tvos-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='tvos-api-diff.html'>Xamarin.TVOS API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Xamarin.TVOS API diff is empty</h2>" >> $@; \
	fi;
endif
ifdef INCLUDE_WATCH
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/watchos-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='watchos-api-diff.html'>Xamarin.WatchOS API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Xamarin.WatchOS API diff is empty</h2>" >> $@; \
	fi;
endif
endif
ifdef INCLUDE_MAC
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/mac-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='mac-api-diff.html'>Xamarin.Mac API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Xamarin.Mac API diff is empty</h2>" >> $@; \
	fi;
endif

# New Dotnet vs Stable Dotnet
ifdef ENABLE_DOTNET
ifdef INCLUDE_IOS
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS-api-diff.html'>Microsoft.iOS Dotnet API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Microsoft.iOS Dotnet API diff is empty</h2>" >> $@; \
	fi;
ifdef INCLUDE_TVOS
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/dotnet/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='dotnet/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS-api-diff.html'>Microsoft.tvOS Dotnet API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Microsoft.tvOS Dotnet API diff is empty</h2>" >> $@; \
	fi;
endif
ifdef INCLUDE_MACCATALYST
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst-api-diff.html'>Microsoft.MacCatalyst Dotnet API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Microsoft.MacCatalyst Dotnet API diff is empty</h2>" >> $@; \
	fi;
endif
endif
ifdef INCLUDE_MAC
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/dotnet/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='dotnet/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS-api-diff.html'>Microsoft.macOS Dotnet API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Microsoft.macOS Dotnet API diff is empty</h2>" >> $@; \
	fi;
endif

# New Dotnet vs. Stable Legacy
ifdef INCLUDE_IOS
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS-api-diff.html'>Microsoft.iOS New Dotnet vs Legacy Stable API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Microsoft.iOS New Dotnet vs Legacy Stable API diff is empty</h2>" >> $@; \
	fi;
ifdef INCLUDE_TVOS
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS-api-diff.html'>Microsoft.tvOS New Dotnet vs Legacy Stable API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Microsoft.tvOS New Dotnet vs Legacy Stable API diff is empty</h2>" >> $@; \
	fi;
endif
endif
ifdef INCLUDE_MAC
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS-api-diff.html'>Microsoft.macOS New Dotnet vs Legacy Stable API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Microsoft.macOS New Dotnet vs Legacy Stable API diff is empty</h2>" >> $@; \
	fi;
endif

# Dotnet iOS vs. Dotnet MacCatalyst
ifdef INCLUDE_IOS
ifdef INCLUDE_MACCATALYST
	$(Q) if ! grep "No change detected" $(APIDIFF_DIR)/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst-api-diff.html >/dev/null 2>&1; then  \
		echo "<h2><a href='dotnet/iOS-MacCatalyst-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.MacCatalyst-api-diff.html'>Microsoft.iOS-MacCatalyst Dotnet iOS vs Dotnet MacCatalyst API diff</a></h2>" >> $@; \
	else \
		echo "<h2>Microsoft.iOS-MacCatalyst Dotnet iOS vs Dotnet MacCatalyst API diff is empty</h2>" >> $@; \
	fi;
endif
endif
endif # ENABLE_DOTNET

# easy-to-type helper targets.
# one rule to create all the api diffs

all-local:: $(APIDIFF_DIR)/api-diff.html

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

# targets to verify that the 32-bit and 64-bit assemblies have identical API.

verify-reference-assemblies-ios: $(APIDIFF_DIR)/temp/native-32/Xamarin.iOS.xml $(APIDIFF_DIR)/temp/native-64/Xamarin.iOS.xml
	$(Q) diff $(APIDIFF_DIR)/temp/native-32/Xamarin.iOS.xml $(APIDIFF_DIR)/temp/native-64/Xamarin.iOS.xml -u
	@echo iOS reference assemblies are identical

clean-local::
	rm -rf temp references updated-references diff dotnet *.exe* api-diff.html
	rm -rf *.dll* bundle-*.zip $(UNZIP_STAMPS)
	rm -rf ios-*.md tvos-*.md watchos-*.md macos-*.md dotnet-*.md

DIRS += $(APIDIFF_DIR)/temp $(APIDIFF_DIR)/diff

# dir creation target
$(DIRS):
	$(Q) mkdir -p $@

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# defining a .SECONDARY rule will prevent that deletion.
.SECONDARY:

merger.exe: merger.cs
	$(Q) $(SYSTEM_CSC) -debug $< -out:$@

ifdef INCLUDE_IOS
ios-markdown: merger.exe $(foreach file,$(IOS_ASSEMBLIES),$(APIDIFF_DIR)/diff/xi/$(file).md)
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.iOS $(APIDIFF_DIR)/diff/xi/Xamarin.iOS/ ios $(APIDIFF_DIR)
dotnet-ios-markdown: merger.exe $(APIDIFF_DIR)/diff/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.md
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.iOS.Dotnet $(APIDIFF_DIR)/diff/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/ dotnet-ios $(APIDIFF_DIR)
dotnet-legacy-ios-markdown: merger.exe $(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.md
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.iOS.Stable.Legacy $(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/ dotnet-legacy-ios $(APIDIFF_DIR)
else
ios-markdown: ; @true
dotnet-ios-markdown: ; @true
dotnet-legacy-ios-markdown: ; @true
endif

ifdef INCLUDE_TVOS
tvos-markdown: merger.exe $(foreach file,$(TVOS_ASSEMBLIES),$(APIDIFF_DIR)/diff/xi/$(file).md)
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.TVOS $(APIDIFF_DIR)/diff/xi/Xamarin.TVOS/ tvos $(APIDIFF_DIR)
dotnet-tv-markdown: merger.exe $(APIDIFF_DIR)/diff/dotnet/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.md
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.tvOS.Dotnet $(APIDIFF_DIR)/diff/dotnet/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/ dotnet-tvos $(APIDIFF_DIR)
dotnet-legacy-tv-markdown: merger.exe $(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.md
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.tvOS.Stable.Legacy $(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/ dotnet-legacy-tvos $(APIDIFF_DIR)
else
tvos-markdown: ; @true
dotnet-tv-markdown: ; @true
dotnet-legacy-tv-markdown: ; @true
endif

ifdef INCLUDE_WATCH
watchos-markdown: merger.exe $(foreach file,$(WATCHOS_ASSEMBLIES),$(APIDIFF_DIR)/diff/xi/$(file).md)
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.WatchOS $(APIDIFF_DIR)/diff/xi/Xamarin.WatchOS/ watchos $(APIDIFF_DIR)
else
watchos-markdown: ; @true
endif

ifdef INCLUDE_MACCATALYST
dotnet-maccatalyst-markdown: merger.exe $(APIDIFF_DIR)/diff/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst.md
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.MacCatalyst.Dotnet $(APIDIFF_DIR)/diff/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/ dotnet-maccatalyst $(APIDIFF_DIR) $(APIDIFF_DIR)
ifdef ENABLE_DOTNET
dotnet-iOS-MacCatalyst-markdown: merger.exe $(APIDIFF_DIR)/diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS-MacCatalyst.md
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.Dotnet.iOS-MacCatalyst $(APIDIFF_DIR)/diff/dotnet/iOS-MacCatalyst-diff/ dotnet-maccatios $(APIDIFF_DIR)
else
dotnet-iOS-MacCatalyst-markdown: ; @true
endif # ENABLE_DOTNET
else
dotnet-maccatalyst-markdown: ; @true
dotnet-iOS-MacCatalyst-markdown: ; @true
endif

ifdef INCLUDE_MAC
macos-markdown: merger.exe $(foreach file,$(MAC_ASSEMBLIES),$(APIDIFF_DIR)/diff/xm/$(file).md)
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.Mac $(APIDIFF_DIR)/diff/xm/Xamarin.Mac/ macos $(APIDIFF_DIR)
dotnet-macos-markdown: merger.exe $(APIDIFF_DIR)/diff/dotnet/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.md
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.macOS.Dotnet $(APIDIFF_DIR)/diff/dotnet/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/ dotnet-macos $(APIDIFF_DIR)
dotnet-legacy-macos-markdown: merger.exe $(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.md
	$(Q) $(SYSTEM_MONO) --debug merger.exe Xamarin.macOS.Stable.Legacy $(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/ dotnet-legacy-macos $(APIDIFF_DIR)
else
macos-markdown: ; @true
dotnet-macos-markdown: ; @true
dotnet-legacy-macos-markdown: ; @true
endif

ifdef ENABLE_DOTNET
dotnet-markdown: dotnet-ios-markdown dotnet-macos-markdown dotnet-maccatalyst-markdown dotnet-tv-markdown
dotnet-legacy-markdown: dotnet-legacy-ios-markdown dotnet-legacy-macos-markdown dotnet-legacy-tv-markdown
else
dotnet-markdown: ; @true
dotnet-legacy-markdown: ; @true
endif

all-markdowns: ios-markdown tvos-markdown watchos-markdown macos-markdown dotnet-markdown dotnet-legacy-markdown dotnet-iOS-MacCatalyst-markdown

$(APIDIFF_DIR)/diff/%.md: $(APIDIFF_DIR)/temp/%.xml $(APIDIFF_DIR)/references/%.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $(APIDIFF_DIR)/references/$*.xml) $(abspath $(APIDIFF_DIR)/temp/$*.xml) $(APIDIFF_IGNORE) --md $(abspath $@)

# Dotnet Legacy markdowns need to compare against legacy xmls
$(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.md: $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.xml $(APIDIFF_DIR)/references/xi/Xamarin.iOS/Xamarin.iOS.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $< $(abspath $(APIDIFF_DIR)/references/xi/Xamarin.iOS/Xamarin.iOS.xml) $(APIDIFF_IGNORE) --md $(abspath $@)

$(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.md: $(APIDIFF_DIR)/temp/dotnet/Microsoft.macOS.Ref/ref/$(DOTNET_TFM)/Microsoft.macOS.xml $(APIDIFF_DIR)/references/xm/Xamarin.Mac/Xamarin.Mac.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $< $(abspath $(APIDIFF_DIR)/references/xm/Xamarin.Mac/Xamarin.Mac.xml) $(APIDIFF_IGNORE) --md $(abspath $@)

$(APIDIFF_DIR)/diff/dotnet/legacy-diff/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.md: $(APIDIFF_DIR)/temp/dotnet/Microsoft.tvOS.Ref/ref/$(DOTNET_TFM)/Microsoft.tvOS.xml $(APIDIFF_DIR)/references/xi/Xamarin.TVOS/Xamarin.TVOS.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $< $(abspath $(APIDIFF_DIR)/references/xi/Xamarin.TVOS/Xamarin.TVOS.xml) $(APIDIFF_IGNORE) --md $(abspath $@)

# Dotnet iOS vs Dotnet MacCatalyst
$(APIDIFF_DIR)/diff/dotnet/iOS-MacCatalyst-diff/Microsoft.iOS-MacCatalyst.md: $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.iOS.xml $(APIDIFF_DIR)/temp/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(Q) sed -e 's_<assembly name="Xamarin.MacCatalyst" version="0.0.0.0">_<assembly name="Xamarin.iOS" version="0.0.0.0">_' $(APIDIFF_DIR)/temp/dotnet/Microsoft.MacCatalyst.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst.xml > $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst-as-iOS.xml
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $<) $(abspath $(APIDIFF_DIR)/temp/dotnet/Microsoft.iOS.Ref/ref/$(DOTNET_TFM)/Microsoft.MacCatalyst-as-iOS.xml) $(APIDIFF_IGNORE) --md $(abspath $@)
	$(Q) touch $@

wrench-api-diff:
	@echo "@MonkeyWrench: AddDirectory: $(CURDIR)/diff/xi/Xamarin.iOS"
	@echo "@MonkeyWrench: AddDirectory: $(CURDIR)/diff/xi/Xamarin.WatchOS"
	@echo "@MonkeyWrench: AddDirectory: $(CURDIR)/diff/xi/Xamarin.TVOS"
	@echo "@MonkeyWrench: AddDirectory: $(CURDIR)/diff/xi/Xamarin.MacCatalyst"
ifdef INCLUDE_MAC
	@echo "@MonkeyWrench: AddDirectory: $(CURDIR)/diff/xm"
	@echo "@MonkeyWrench: AddDirectory: $(CURDIR)/diff/xm/4.5"
	@echo "@MonkeyWrench: AddDirectory: $(CURDIR)/diff/xm/Xamarin.Mac"
	@echo "@MonkeyWrench: AddFile: $(CURDIR)/mac-api-diff.html"
endif
ifdef INCLUDE_IOS
	@echo "@MonkeyWrench: AddFile: $(CURDIR)/ios-api-diff.html"
ifdef INCLUDE_WATCH
	@echo "@MonkeyWrench: AddFile: $(CURDIR)/watchos-api-diff.html"
endif
ifdef INCLUDE_TVOS
	@echo "@MonkeyWrench: AddFile: $(CURDIR)/tvos-api-diff.html"
	@echo "@MonkeyWrench: AddFile: $(CURDIR)/diff/ios-to-tvos.html"
endif
endif
	$(Q) $(MAKE) $(UNZIP_STAMPS)
	$(Q) $(MAKE) all -j8
	$(Q) $(MAKE) all-markdowns
	$(Q) $(CP) api-diff.html index.html
	@echo "@MonkeyWrench: AddFile: $(CURDIR)/index.html"
	@echo "@MonkeyWrench: AddFile: $(CURDIR)/api-diff.html"
	@# remove empty files so they're not uploaded
	@-find $(CURDIR)/diff -size 0 | xargs rm
ifdef INCLUDE_IOS
	$(MAKE) verify-reference-assemblies-ios -j8
endif

jenkins-api-diff:
	-$(MAKE) wrench-api-diff
	$(Q) mkdir -p $(JENKINS_RESULTS_DIRECTORY)/api-diff
	$(Q) $(CP) *.html $(JENKINS_RESULTS_DIRECTORY)/api-diff
	$(Q) $(CP) *.md $(JENKINS_RESULTS_DIRECTORY)/api-diff
	$(Q) $(CP) -R diff $(JENKINS_RESULTS_DIRECTORY)/api-diff
	$(Q) $(CP) -R dotnet $(JENKINS_RESULTS_DIRECTORY)/api-diff
