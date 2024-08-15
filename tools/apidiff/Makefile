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

APIDIFF_IGNORE = -i 'INSObjectProtocol'

$(MONO_API_INFO): $(wildcard $(API_TOOLS_PATH)/mono-api-info/*.cs*)
	$(Q) $(DOTNET) build $(API_TOOLS_PATH)/mono-api-info/mono-api-info.csproj /bl:$@.binlog $(MSBUILD_VERBOSITY)
	$(Q) touch $@

$(MONO_API_HTML): $(wildcard $(API_TOOLS_PATH)/mono-api-html/*.cs*)
	$(Q) $(DOTNET) build $(API_TOOLS_PATH)/mono-api-html/mono-api-html.csproj /bl:$@.binlog $(MSBUILD_VERBOSITY)
	$(Q) touch $@

# create diff from api info and reference info
# note that we create an empty file (the 'touch' command)
# so that we get a file in all cases (so that we don't have 
# to run mono-api-html every time even if none of the
# dependencies changed)

define DotNetComputeDiff
$(OUTPUT_DIR)/diff/Microsoft.$(1)%html $(OUTPUT_DIR)/diff/Microsoft.$(1)%md: $(APIDIFF_DIR)/references/current/Microsoft.$(1)%xml $(APIDIFF_DIR)/references/comparison/Microsoft.$(1).xml $(MONO_API_HTML)
	$$(Q) mkdir -p $$(dir $$@)
	$$(QF_GEN) $(MONO_API_HTML_EXEC) $(NEW_REGEX) $(ADD_REGEX) $$(abspath $(APIDIFF_DIR)/references/comparison/Microsoft.$(1).xml) $$(abspath $(APIDIFF_DIR)/references/current/Microsoft.$(1).xml) $(APIDIFF_IGNORE) --html "$$(abspath $$(basename $$@).html)" --markdown "$$(abspath $$(basename $$@).md)"
	$$(Q) touch $$@

html:: $(OUTPUT_DIR)/diff/Microsoft.$(1).html
markdown:: $(OUTPUT_DIR)/diff/Microsoft.$(1)%md
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

API_DIFF_DEPENDENCIES += $(foreach platform,$(DOTNET_PLATFORMS),$(OUTPUT_DIR)/diff/Microsoft.$(platform).html)

$(OUTPUT_DIR)/index.html: $(OUTPUT_DIR)/api-diff.html
	$(Q) $(CP) $< $@

#
# Collect all the diffs into a single api-diff.html file
#

define ApiDiffReportHtml
	$(Q) ./report-status.sh "$(1)" "$(OUTPUT_DIR)" html "diff/Microsoft.$(1).html" "diff/Microsoft.$(1).md" "$(2)"

endef

$(OUTPUT_DIR)/api-diff.html: $(API_DIFF_DEPENDENCIES)
	$(Q) rm -f $@
	$(Q) echo "<!DOCTYPE html>" >> $@
	$(Q) echo "<html>" >> $@
	$(Q) echo "<head>" >> $@
	$(Q) echo '<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>' >> $@
	$(Q) echo "<title>API diffs</title>" >> $@
	$(Q) echo "</head>" >> $@
	$(Q) echo "<body>" >> $@
	$(Q) echo "<h1>API diffs</h1>" >> $@

	$(Q) if $(foreach html,$(wildcard $(OUTPUT_DIR)/diff/Microsoft.*.html),! test -s "$(html)" &&) true; then \
		echo "<h2>.NET (empty diffs)</h2>" >> "$@"; \
	elif grep BreakingChangesDetected "$(OUTPUT_DIR)"/diff/Microsoft.*.html &> /dev/null; then \
		echo "<h2>.NET ($(HTML_BREAKING_CHANGES_MESSAGE))</h2>" >> "$@"; \
	else \
		echo "<h2>.NET ($(HTML_NO_BREAKING_CHANGES_MESSAGE))</h2>" >> "$@"; \
	fi
	$(Q) echo "<ul>" >> $@

	@# New .NET vs Stable .NET
	$(Q) $(foreach platform,$(DOTNET_PLATFORMS),$(call ApiDiffReportHtml,$(platform),$@))

	$(Q) echo "</ul>" >> $@

	$(Q) echo "</body>" >> $@
	$(Q) echo "</html>" >> $@

	$(Q) if grep "$(HTML_BREAKING_CHANGES_MESSAGE)" $@ >/dev/null 2>&1; then \
		echo "" >> $@; \
		echo "<!-- BreakingChangesDetected -->" >> $@; \
		echo "" >> $@; \
	fi

	@echo "Created $@"

#
# Collect all the diffs into a single api-diff.md file
#

define ApiDiffReportMarkdown
	$(Q) ./report-status.sh "$(1)" "$(OUTPUT_DIR)" markdown "diff/Microsoft.$(1).html" "diff/Microsoft.$(1).md" "$(2)"

endef

all-markdowns:: $(OUTPUT_DIR)/api-diff.md

$(OUTPUT_DIR)/api-diff.md: $(API_DIFF_DEPENDENCIES)
	$(Q) if $(foreach html,$(wildcard $(OUTPUT_DIR)/diff/Microsoft.*.html),! test -s "$(html)" &&) true; then \
		echo "<details><summary>NET (empty diffs)</summary>" >> "$@"; \
	elif grep BreakingChangesDetected "$(OUTPUT_DIR)"/diff/Microsoft.*.html &> /dev/null; then \
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
BUNDLE_ZIP_$(1)=$(APIDIFF_DIR)/Microsoft.$(1).Ref.$(STABLE_NUGET_VERSION_$(1)).nupkg
STABLE_NUGET_$(1)_URL=https://www.nuget.org/api/v2/package/Microsoft.$(1).Ref.$(STABLE_NUGET_VERSION_$(1))
CACHE_FILE_$(1)=~/Library/Caches/xamarin-macios/Microsoft.$(1).Ref.$(STABLE_NUGET_VERSION_$(1)).nupkg
$$(BUNDLE_ZIP_$(1)):
	$(Q) mkdir -p $$(dir $$@)
	@# download to a temporary filename so interrupted downloads won't prevent re-downloads.
	@echo "Downloading $$(STABLE_NUGET_$(1)_URL)..."
	$$(Q) if test -f $$(CACHE_FILE_$(1)); then \
		echo "Found a cached version: $$(CACHE_FILE_$(1))."; \
		$$(CP) $$(CACHE_FILE_$(1)) $$@.tmp; \
	else \
		$(MAKE) check-token || exit 1; \
		if ! $(CURL_RETRY) -H "Authorization: token $(AUTH_TOKEN_GITHUB_COM)" "$$(STABLE_NUGET_$(1)_URL)" --output $$@.tmp; then \
			echo "Failed to download $$(STABLE_NUGET_$(1)_URL)"; \
			exit 1; \
		fi; \
		if [[ "x$$$$MACIOS_CACHE_DOWNLOADS" != "x" ]]; then \
			mkdir -p $$(dir $$(CACHE_FILE_$(1))); \
			$$(CP) $$@.tmp $$(CACHE_FILE_$(1)); \
			echo "Cached the download in $$(CACHE_FILE_$(1))"; \
		fi; \
	fi
	$$(Q) mv $$@.tmp $$@

download:: $$(BUNDLE_ZIP_$(1))
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DownloadBundle,$(platform))))

# Here we unzip the downloaded nupkg
define UnzipBundle
ifndef COMPARISON_DLL_$(1)
UNZIP_DIR_$(1)=temp/comparison/$(1)
COMPARISON_DLL_$(1)=$$(UNZIP_DIR_$(1))/Microsoft.$(1).dll
$$(COMPARISON_DLL_$(1)): $$(BUNDLE_ZIP_$(1))
	$$(Q) rm -Rf "$$(UNZIP_DIR_$(1))"
	$$(Q) mkdir -p $$(dir $$(UNZIP_DIR_$(1)))
	$$(Q_GEN) unzip -j $$(if $$(V),,-q) -d $$(UNZIP_DIR_$(1)) $$< 'ref/*/Microsoft.$(1).dll'
endif

unzip:: $$(COMPARISON_DLL_$(1))
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call UnzipBundle,$(platform))))

define DotNetUnzipDirectory
APIDIFF_HASH_DOTNET_$(1)=$$(word 5,$$(subst /, ,$$(APIDIFF_REFERENCES_DOTNET_$(1))))
UNZIP_DIR_DOTNET_$(1)=$$(UNZIP_DIR_$$(APIDIFF_HASH_DOTNET_$(1)))
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call DotNetUnzipDirectory,$(platform))))

define GenerateComparisonReferenceXml
$(APIDIFF_DIR)/references/comparison/Microsoft.$(1).xml: $$(COMPARISON_DLL_$(1)) $(MONO_API_INFO)
	$$(Q) mkdir -p $$(dir $$@)
	$$(QF_GEN) $(MONO_API_INFO_EXEC) $$(abspath $$<) -o $$(abspath $$@)

update-comparison-refs:: $(APIDIFF_DIR)/references/comparison/Microsoft.$(1).xml
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call GenerateComparisonReferenceXml,$(platform))))

define GenerateCurrentReferenceXml
$(APIDIFF_DIR)/references/current/Microsoft.$(1).xml: $(DOTNET_DESTDIR)/$($(1)_NUGET_REF_NAME)/ref/$(DOTNET_TFM)/Microsoft.$(1).dll $(MONO_API_INFO)
	$$(Q) mkdir -p $$(dir $$@)
	$$(QF_GEN) $$(MONO_API_INFO_EXEC) $$(abspath $$<) -o $$(abspath $$@)

update-current-refs:: $(APIDIFF_DIR)/references/current/Microsoft.$(1).xml
endef
$(foreach platform,$(DOTNET_PLATFORMS),$(eval $(call GenerateCurrentReferenceXml,$(platform))))

clean-local::
	rm -rf temp references diff dotnet *.exe* api-diff.html
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
