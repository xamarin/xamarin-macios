TOP=../..
include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

# without this many compiler warnings about unused functions and variables
# in system headers show up.
export CCACHE_CPP2=1

#
# mtouch
#

MTOUCH_CONF=Release
MTOUCH_DIR=bin/$(MTOUCH_CONF)
LOCAL_MTOUCH=$(MTOUCH_DIR)/mtouch.dll
LOCAL_MTOUCH_COMMAND=$(DOTNET) exec $(LOCAL_MTOUCH)

DOTNET_PLATFORMS_MTOUCH=$(filter-out macOS,$(DOTNET_PLATFORMS))

# mtouch.csproj.inc contains the mtouch_dependencies variable used to determine if mtouch needs to be rebuilt or not.
mtouch.csproj.inc: export BUILD_VERBOSITY=$(MSBUILD_VERBOSITY)
mtouch.csproj.inc: export DOTNET:=$(DOTNET)
-include mtouch.csproj.inc

$(LOCAL_MTOUCH): $(mtouch_dependencies)
	$(Q_GEN) $(DOTNET) build mtouch.csproj $(DOTNET_BUILD_VERBOSITY) /p:Configuration=$(MTOUCH_CONF)

$(abspath Constants.cs): Constants.cs.in Makefile $(TOP)/Make.config.inc
	$(Q_GEN) sed \
		-e "s/@VERSION@/$(IOS_NUGET_VERSION_MAJOR).$(IOS_NUGET_VERSION_MINOR).$(IOS_NUGET_VERSION_PATCH)/g" \
		-e 's/@REVISION@/$(IOS_NUGET_COMMIT_DISTANCE) ($(CURRENT_BRANCH_SED_ESCAPED): $(CURRENT_HASH))/g' \
		$< > $@
#
# Partial static registrar libraries
#
define RunRegistrar
.libs/Microsoft.$(9).registrar.$(10)%m .libs/Microsoft.$(9).registrar.$(10)%h: $(TOP)/src/build/dotnet/$(1)/$(3)/Microsoft.$(9).dll $(LOCAL_MTOUCH) | .libs
	$$(Q_GEN) $$(LOCAL_MTOUCH_COMMAND) $$(MTOUCH_VERBOSITY) --runregistrar:$$(abspath $$(basename $$@).m) --sdkroot $$(XCODE_DEVELOPER_ROOT) --sdk $(4) $$< --registrar:static --target-framework .NETCoreApp,Version=$(subst net,,$(DOTNET_TFM)),Profile=$(1) --abi $(2) -r:$(DOTNET_BCL_DIR)/System.Runtime.dll -r:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll --rid $(10)
	$$(Q) touch $$(basename $$@).m $$(basename $$@).h

.libs/Microsoft.$(9).registrar.$(10).a: .libs/Microsoft.$(9).registrar.$(10).m .libs/Microsoft.$(9).registrar.$(10).h | .libs
	$$(Q_CC) $$(CLANG) -DDEBUG -g -gdwarf-2 $(6) -stdlib=libc++ -std=c++14 -x objective-c++ -o $$@ -c $$< -Wall -Wno-unguarded-availability-new -I$(TOP)/runtime
endef
$(eval $(call RunRegistrar,ios,x86_64,64,$(IOS_SDK_VERSION),iOS,$(iossimulator-x64_CFLAGS),,,iOS,iossimulator-x64))
$(eval $(call RunRegistrar,ios,arm64,64,$(IOS_SDK_VERSION),iOS,$(iossimulator-arm64_CFLAGS),,,iOS,iossimulator-arm64))
$(eval $(call RunRegistrar,ios,arm64,64,$(IOS_SDK_VERSION),iOS,$(ios-arm64_CFLAGS),,,iOS,ios-arm64))
$(eval $(call RunRegistrar,tvos,x86_64,64,$(TVOS_SDK_VERSION),TVOS,$(tvossimulator-x64_CFLAGS),,,tvOS,tvossimulator-x64))
$(eval $(call RunRegistrar,tvos,arm64,64,$(TVOS_SDK_VERSION),TVOS,$(tvossimulator-arm64_CFLAGS),,,tvOS,tvossimulator-arm64))
$(eval $(call RunRegistrar,tvos,arm64,64,$(TVOS_SDK_VERSION),TVOS,$(tvos-arm64_CFLAGS),,,tvOS,tvos-arm64))
$(eval $(call RunRegistrar,maccatalyst,x86_64,64,$(MACCATALYST_SDK_VERSION),MacCatalyst,$(maccatalyst-x64_CFLAGS),,,MacCatalyst,maccatalyst-x64))
$(eval $(call RunRegistrar,maccatalyst,arm64,64,$(MACCATALYST_SDK_VERSION),MacCatalyst,$(maccatalyst-arm64_CFLAGS),,,MacCatalyst,maccatalyst-arm64))

TARGETS_DOTNET = \
	$(foreach platform,$(DOTNET_PLATFORMS_MTOUCH),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/native/Microsoft.$(platform).registrar.a)) \

TARGET_DIRS_DOTNET = \
	$(foreach platform,$(DOTNET_PLATFORMS_MTOUCH),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(DOTNET_DESTDIR)/$($(rid)_NUGET_RUNTIME_NAME)/runtimes/$(rid)/native)) \

TARGET_DIRS = \
	.libs                                                        \
	bin/Makefile                                                 \
	$(TARGET_DIRS_DOTNET)                                        \

$(TARGET_DIRS):
	$(Q) mkdir -p $@

define InstallRegistrar
$(DOTNET_DESTDIR)/$$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native/Microsoft.$(1).registrar.a: .libs/Microsoft.$(1).registrar.$(rid).a | $(DOTNET_DESTDIR)/$($(2)_NUGET_RUNTIME_NAME)/runtimes/$(2)/native
	$(Q) $(CP) $$< $$@
endef

$(foreach platform,$(DOTNET_PLATFORMS_MTOUCH),$(foreach rid,$(DOTNET_$(platform)_RUNTIME_IDENTIFIERS),$(eval $(call InstallRegistrar,$(platform),$(rid)))))

dotnet: $(TARGETS_DOTNET)
install-local:: $(TARGETS_DOTNET)
all-local:: $(TARGETS_DOTNET)

clean-local::
	rm -Rf bin obj

all-local:: verify-sorted-resx
verify-sorted-resx:
	$(Q) grep '"[A-Z][A-Z][0-9][0-9][0-9][0-9]' Errors.resx | sed -e 's/.*name="[A-Z][A-Z]//' -e 's/" xml:.*//' > .$@.tmp
	$(Q) sort .$@.tmp > .$@-sorted.tmp
	$(Q) if diff -U0 .$@.tmp .$@-sorted.tmp > ".$@-unsorted.tmp" 2>&1; then \
		rm -rf .$@*tmp; \
	else \
		echo "❌ The following errors in Errors.resx are not sorted:"; \
		cat ".$@-unsorted.tmp" | grep "^[+][^+]" | sed 's/^[+]/    /'; \
		rm -rf .$@*tmp; \
		exit 1; \
	fi

clean-dotnet:
	rm -f $(TARGETS_DOTNET)

include ../common/Make.common
