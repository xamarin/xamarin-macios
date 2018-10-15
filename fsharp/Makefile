TOP=..
include $(TOP)/Make.config

FSHARP_FILES:=$(shell git --git-dir=$(FSHARP_PATH)/.git --work-tree=$(FSHARP_PATH) ls-tree --full-tree -r HEAD --name-only | sed 's_^\(.*\)_$(FSHARP_PATH)/\1_' | sed 's/ /\\ /g')

FSHARP_TARGET_FILES =   \
	FSharp.Core.dll     \
	FSharp.Core.dll.mdb \
	FSharp.Core.optdata \
	FSharp.Core.sigdata \
	FSharp.Core.xml     \

BIN_DIR=$(FSHARP_PATH)/lib/release
IOS_BIN_DIR = $(FSHARP_PATH)/lib/release/monotouch
WATCH_BIN_DIR = $(FSHARP_PATH)/lib/release/xamarinwatchos
TVOS_BIN_DIR = $(FSHARP_PATH)/lib/release/xamarintvos
MAC_MOBILE_BIN_DIR = $(FSHARP_PATH)/lib/release/xamarinmacmobile
MAC_45_BIN_DIR = $(FSHARP_PATH)/lib/release/xamarinmacfull

# Xamarin.iOS

IOS_FSHARP_TARGETS =                                                                                      \
	$(foreach file,$(FSHARP_TARGET_FILES),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/$(file))         \
	$(foreach file,$(FSHARP_TARGET_FILES),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/$(file)) \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/LICENSE-fsharp                                         \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/LICENSE-fsharp                                 \

IOS_FSHARP_DIRECTORIES =                                   \
	build/monotouch                                        \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1         \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS \

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/LICENSE-fsharp: $(FSHARP_PATH)/LICENSE | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/%.dll: build/monotouch/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1/%: $(IOS_BIN_DIR)/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/2.1
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/LICENSE-fsharp: $(FSHARP_PATH)/LICENSE | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/%.dll: build/monotouch/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS/%: $(IOS_BIN_DIR)/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.iOS
	$(Q) install -m 0644 $< $@

# Xamarin.WatchOS


WATCH_FSHARP_TARGETS =                                                                                        \
	$(foreach file,$(FSHARP_TARGET_FILES),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/$(file)) \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/LICENSE-fsharp                                 \

WATCH_FSHARP_DIRECTORIES =                                     \
	build/xamarinwatchos                                       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS \

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/LICENSE-fsharp: $(FSHARP_PATH)/LICENSE | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%.dll: build/xamarinwatchos/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS/%: $(WATCH_BIN_DIR)/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.WatchOS
	$(Q) install -m 0644 $< $@

# Xamarin.TVOS


TVOS_FSHARP_TARGETS =                                                                                      \
	$(foreach file,$(FSHARP_TARGET_FILES),$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/$(file)) \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/LICENSE-fsharp                                 \

TVOS_FSHARP_DIRECTORIES =                                   \
	build/xamarintvos                                       \
	$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS \

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/LICENSE-fsharp: $(FSHARP_PATH)/LICENSE | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%.dll: build/xamarintvos/%.dll | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS
	$(Q) install -m 0644 $< $@

$(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS/%: $(TVOS_BIN_DIR)/% | $(IOS_DESTDIR)$(MONOTOUCH_PREFIX)/lib/mono/Xamarin.TVOS
	$(Q) install -m 0644 $< $@

# Xamarin.Mac

MAC_FSHARP_TARGETS =                                                                                               \
	$(foreach file,$(FSHARP_TARGET_FILES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/$(file))         \
	$(foreach file,$(FSHARP_TARGET_FILES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/$(file)) \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/LICENSE-fsharp                                         \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/LICENSE-fsharp                                 \

MAC_FSHARP_DIRECTORIES =                                            \
	build/xamarinmacfull                                            \
	build/xamarinmacmobile                                          \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5         \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac \

## XM full

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/LICENSE-fsharp: $(FSHARP_PATH)/LICENSE | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/%.dll: build/xamarinmacfull/%.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/%: $(MAC_45_BIN_DIR)/% | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5
	$(Q) install -m 0644 $< $@

## XM mobile

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/LICENSE-fsharp: $(FSHARP_PATH)/LICENSE | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/%.dll: build/xamarinmacmobile/%.dll | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac
	$(Q) install -m 0644 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/%: $(MAC_MOBILE_BIN_DIR)/% | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac
	$(Q) install -m 0644 $< $@

# Common

ifdef INCLUDE_IOS
FSHARP_TARGETS += $(IOS_FSHARP_TARGETS)
FSHARP_DIRECTORIES += $(IOS_FSHARP_DIRECTORIES)
FSHARP_BUILD_TARGETS += only-monotouch
ifdef INCLUDE_WATCH
FSHARP_TARGETS += $(WATCH_FSHARP_TARGETS)
FSHARP_DIRECTORIES += $(WATCH_FSHARP_DIRECTORIES)
FSHARP_BUILD_TARGETS += only-xamarinwatchos
endif
ifdef INCLUDE_TVOS
FSHARP_TARGETS += $(TVOS_FSHARP_TARGETS)
FSHARP_DIRECTORIES += $(TVOS_FSHARP_DIRECTORIES)
FSHARP_BUILD_TARGETS += only-xamarintvos
endif
endif

ifdef INCLUDE_MAC
FSHARP_TARGETS += $(MAC_FSHARP_TARGETS)
FSHARP_DIRECTORIES += $(MAC_FSHARP_DIRECTORIES)
FSHARP_BUILD_TARGETS += only-xamarinmac
endif

$(FSHARP_DIRECTORIES):
	$(Q) mkdir -p $@

### Sign the assemblies. This is done in a temporary directory so that we don't have to re-sign on every install.
build/%.dll: $(BIN_DIR)/%.dll | build/monotouch build/xamarinmacmobile build/xamarinmacfull build/xamarinwatchos build/xamarintvos
	$(Q) cp $< $@
	$(Q_SN) MONO_CFG_DIR="$(TOP)" sn -q -R "$@" "$(PRODUCT_KEY_PATH)"

build.stamp: export TargetFrameworkFallbackSearchPaths=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
build.stamp: export MSBuildExtensionsPathFallbackPathsOverride=$(IOS_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
build.stamp: $(FSHARP_FILES) $(FSHARP_PATH)/Makefile
	$(MAKE) -C $(FSHARP_PATH)/src/fsharp build-proto
	$(MAKE) -C $(FSHARP_PATH)/src/fsharp $(FSHARP_BUILD_TARGETS)
	$(Q) touch $@

$(FSHARP_TARGETS): build.stamp

all-local:: $(FSHARP_TARGETS)
install-local:: $(FSHARP_TARGETS)

$(FSHARP_PATH)/Makefile: $(FSHARP_PATH)/autogen.sh $(FSHARP_FILES) /Library/Frameworks/Mono.framework/Versions/Current/updateinfo
	@echo "Executing autogen for fsharp..."
	$(Q) cd $(FSHARP_PATH) && ./autogen.sh --prefix=/Library/Frameworks/Mono.framework/Versions/Current
	@echo "Executed autogen for fsharp"

clean-local::
	git clean -xfd
	cd $(FSHARP_PATH) && git clean -xfd

include $(TOP)/mk/rules.mk
