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

MONO_API_INFO = $(API_TOOLS_PATH)/mono-api-info/bin/Debug/$(DOTNET_TFM)/mono-api-info.dll
MONO_API_HTML = $(API_TOOLS_PATH)/mono-api-html/bin/Debug/$(DOTNET_TFM)/mono-api-html.dll
MONO_BUILD = $(SYSTEM_MONO)

MONO_API_INFO_EXEC = $(DOTNET) --roll-forward Major $(MONO_API_INFO) --ignore-inherited-interfaces
MONO_API_HTML_EXEC = $(DOTNET) --roll-forward Major $(MONO_API_HTML)

export HTML_BREAKING_CHANGES_MESSAGE=❗️Breaking changes❗️
export HTML_NO_BREAKING_CHANGES_MESSAGE=No breaking changes
export MARKDOWN_BREAKING_CHANGES_MESSAGE=:heavy_exclamation_mark: Breaking changes :heavy_exclamation_mark:
export MARKDOWN_NO_BREAKING_CHANGES_MESSAGE=No breaking changes

ifeq ($(DOTNET_TFM_REFERENCE),)
# Change the below to net9.0 once we have reference assemblies from net9.0 (i.e. once net8.0 goes stable).
DOTNET_TFM_REFERENCE=net8.0
endif

DOTNET_ASSEMBLIES = $(foreach platform,$(DOTNET_PLATFORMS),Microsoft.$(platform).Ref/ref/$(DOTNET_TFM)/Microsoft.$(platform))

APIDIFF_IGNORE = -i 'INSObjectProtocol'

$(MONO_API_INFO): $(wildcard $(API_TOOLS_PATH)/mono-api-info/*.cs*)
	$(Q) $(DOTNET) build $(API_TOOLS_PATH)/mono-api-info/mono-api-info.csproj /bl:$@.binlog $(MSBUILD_VERBOSITY)
	$(Q) touch $@

$(MONO_API_HTML): $(wildcard $(API_TOOLS_PATH)/mono-api-html/*.cs*)
	$(Q) $(DOTNET) build $(API_TOOLS_PATH)/mono-api-html/mono-api-html.csproj /bl:$@.binlog $(MSBUILD_VERBOSITY)
	$(Q) touch $@

# create api info.
define DotNetApiInfo
$(APIDIFF_DIR)/temp/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).xml: $(DOTNET_DESTDIR)/$($(1)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).dll $(MONO_API_INFO)
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
$(OUTPUT_DIR)/diff/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1)%html $(OUTPUT_DIR)/diff/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1)%md: $(APIDIFF_DIR)/temp/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1)%xml $(APIDIFF_DIR)/references/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM_REFERENCE)/Microsoft.$(1).xml $(MONO_API_HTML)
	$$(Q) mkdir -p $$(dir $$@)
	$$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $$(abspath $(APIDIFF_DIR)/references/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM_REFERENCE)/Microsoft.$(1).xml) $$(abspath $(APIDIFF_DIR)/temp/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).xml) $(APIDIFF_IGNORE) --html "$$(abspath $$(basename $$@).html)" --markdown "$$(abspath $$(basename $$@).md)"
	$$(Q) touch $$@
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetComputeDiff,$(platform))))

$(OUTPUT_DIR)/diff/%.html $(OUTPUT_DIR)/diff/%.md: $(APIDIFF_DIR)/temp/%.xml $(APIDIFF_DIR)/references/%.xml $(MONO_API_HTML)
	$(Q) mkdir -p $(dir $@)
	$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $(abspath $(APIDIFF_DIR)/references/$*.xml) $(abspath $(APIDIFF_DIR)/temp/$*.xml) $(APIDIFF_IGNORE) --html "$(abspath $(basename $@).html)" --markdown "$(abspath $(basename $@).md)"
	$(Q) touch $@

# create diff files for all the assemblies per platform

$(OUTPUT_DIR)/%-api-diff.html:
	$(Q) rm -f $@
	$(Q) mkdir -p $$(dirname "$@");
	$(Q) touch $@-toc
	$(Q_GEN) for file in $?; do \
		if [[ "x0" != "x`stat -L -f %z $$file`" ]]; then  \
			cat $$file | sed "s*<h1>*<h1 id='$$file'>*" >> $@;	\
			echo "<br><hr>" >> $@;	\
			echo "<a href='#$$file'>`echo $$file | sed -e 's_html_dll_' -e 's_diff/dotnet/Microsoft.iOS.Ref/ref/net[0-9].[0-9]/__' -e 's_diff/dotnet/Microsoft.tvOS.Ref/ref/net[0-9].[0-9]/__' -e 's_diff/dotnet/Microsoft.macOS.Ref/ref/net[0-9].[0-9]/__' -e 's_diff/dotnet/Microsoft.MacCatalyst.Ref/ref/net[0-9].[0-9]/__' -e 's#/Users/.*apidiff/##' `</a><br/>" >> $@-toc; \
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

API_DIFF_DEPENDENCIES += $(foreach assembly,$(DOTNET_ASSEMBLIES),$(OUTPUT_DIR)/diff/dotnet/$(assembly).html)

$(OUTPUT_DIR)/index.html: $(OUTPUT_DIR)/api-diff.html
	$(Q) $(CP) $< $@

#
# Collect all the diffs into a single api-diff.html file
#

define ApiDiffReportHtml
	$(Q) ./report-status.sh "$(1)" "$(OUTPUT_DIR)" html "diff/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).html" "diff/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).md" "$(2)"

endef

$(OUTPUT_DIR)/api-diff.html: $(API_DIFF_DEPENDENCIES)
	$(Q) rm -f $@
	$(QF_GEN) echo "<!DOCTYPE html>" >> $@
	$(QF_GEN) echo "<html>" >> $@
	$(QF_GEN) echo "<head>" >> $@
	$(QF_GEN) echo '<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>' >> $@
	$(QF_GEN) echo "<title>API diffs</title>" >> $@
	$(QF_GEN) echo "</head>" >> $@
	$(QF_GEN) echo "<body>" >> $@
	$(QF_GEN) echo "<h1>API diffs</h1>" >> $@

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

	$(Q) echo "</ul>" >> $@

	$(QF_GEN) echo "</body>" >> $@
	$(QF_GEN) echo "</html>" >> $@

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

	$(Q) echo "" >> $@
	$(Q) echo "</details>" >> "$@"
	$(Q) echo "" >> $@

	$(Q) if grep "$(MARKDOWN_BREAKING_CHANGES_MESSAGE)" $@ >/dev/null 2>&1; then \
		echo "" >> $@; \
		echo "<!-- BreakingChangesDetected -->" >> $@; \
		echo "" >> $@; \
	fi

# easy-to-type helper targets.
# one rule to create all the api diffs

all-local:: $(OUTPUT_DIR)/index.html $(OUTPUT_DIR)/api-diff.md

# Rules to re-create the reference infos from the current stable 'bundle.zip' assemblies

# First download the bundle zips we need. Multiple platforms may (or may not) share the same bundle.zip (with the same url),
# so account for that and only download each bundle.zip once.
APIDIFF_URLS=$(foreach platform,$(DOTNET_PLATFORMS),$(APIDIFF_REFERENCES_DOTNET_$(platform)))
APIDIFF_UNIQUE_URLS=$(sort $(APIDIFF_URLS))
APIDIFF_UNIQUE_HASHES=$(foreach url,$(APIDIFF_UNIQUE_URLS),$(word 5,$(subst /, ,$(url))))

AUTH_TOKEN_GITHUB_COM_FILE=$(HOME)/.config/AUTH_TOKEN_GITHUB_COM
ifeq ($(AUTH_TOKEN_GITHUB_COM),)
ifeq ($(AUTH_TOKEN_GITHUB_COM_FILE),$(shell ls -1 $(AUTH_TOKEN_GITHUB_COM_FILE) 2>/dev/null))
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
		if ! $(CURL_RETRY) -H "Authorization: token $(AUTH_TOKEN_GITHUB_COM)" "$$(BUNDLE_ZIP_$(1)_URL)" --output $$@.tmp; then \
			echo "Failed to download $$(BUNDLE_ZIP_$(1)_URL)"; \
			exit 1; \
		fi; \
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

define DotNetUnzipDirectory
APIDIFF_HASH_DOTNET_$(1)=$$(word 5,$$(subst /, ,$$(APIDIFF_REFERENCES_DOTNET_$(1))))
UNZIP_DIR_DOTNET_$(1)=$$(UNZIP_DIR_$$(APIDIFF_HASH_DOTNET_$(1)))
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetUnzipDirectory,$(platform))))

DOTNET_REFS = $(foreach file,$(DOTNET_ASSEMBLIES),$(APIDIFF_DIR)/updated-references/dotnet/$(file).xml)

define DotNetGenerateReferenceXml
$(APIDIFF_DIR)/references/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM_REFERENCE)/Microsoft.$(1).xml: $(UNZIP_DIR_DOTNET_$(1))/Microsoft.$(1).Ref/ref/$(DOTNET_TFM_REFERENCE)/Microsoft.$(1).dll  $(MONO_API_INFO)
	$$(Q) mkdir -p $$(dir $$@)
	$$(QF_GEN) $(MONO_API_INFO_EXEC) $$(abspath $$<) -o $$(abspath $$@)
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetGenerateReferenceXml,$(platform))))

define UpdatedReferences
$(APIDIFF_DIR)/updated-references/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).xml: $(DOTNET_DESTDIR)/$($(1)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).dll $(MONO_API_INFO)
	$$(Q) mkdir -p $$(dir $$@) $$(dir $$(APIDIFF_DIR)/references/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).Ref)
	$$(QF_GEN) $$(MONO_API_INFO_EXEC) $$(abspath $$<) -o $$(abspath $$(APIDIFF_DIR)/references/dotnet/Microsoft.$(1).Ref/ref/$(DOTNET_TFM)/Microsoft.$(1).xml)
endef

$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call UpdatedReferences,$(platform))))

update-dotnet: $(DOTNET_REFS)
update-refs: $(DOTNET_REFS)

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
