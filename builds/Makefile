TOP=..
include $(TOP)/Make.config

XCODE_DEVELOPER_ROOT=$(XCODE94_DEVELOPER_ROOT)
export DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT)

PREFIX=$(abspath $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/)

IPHONESIMULATOR_SDK=$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk
IPHONESIMULATOR_PREFIX=$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphonesimulator.sdk/usr
IPHONEOS_SDK=$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphoneos.sdk
IPHONEOS_PREFIX=$(MONOTOUCH_PREFIX)/SDKs/MonoTouch.iphoneos.sdk/usr

ifdef ENABLE_XAMARIN
XAMARIN_AUTOGEN_FLAGS=--enable-extension-module=xamarin
XAMARIN_IOS_CONFIGURE_FLAGS=--enable-extension-module=xamarin
XAMARIN_MAC_CONFIGURE_FLAGS=--enable-extension-module=xamarin --enable-extension-module
-include $(MACCORE_PATH)/builds/Makefile.include
endif

ifndef DISABLE_STRIP
INSTALL_STRIP_FLAG=-s
endif

ifndef ENABLE_XI_BTLS
XAMARIN_IOS_CONFIGURE_FLAGS += --disable-btls
endif
ifndef ENABLE_XM_BTLS
XAMARIN_MAC_CONFIGURE_FLAGS += --disable-btls
endif

# this is a list of all the files from mono we care about, so that we
# can use that list as dependencies for our makefile targets
-include .deps.mono.mk
.deps.mono.mk: $(TOP)/.git/modules/external/mono/HEAD
	$(Q) printf 'MONO_DEPENDENCIES += Makefile \\\n' > $@.tmp
	$(Q) cd $(MONO_PATH) && git ls-files 'mcs/class/*.cs' '*.h' '*.c' '*.cpp' | grep -v "/Test/" | sed 's/ /\\ /g' | sed 's@^\(.*\)$$@	$(MONO_PATH)/\1 \\@' >> $(abspath $@).tmp
	$(Q) mv $@.tmp $@

-include .deps.llvm.mk
.deps.llvm.mk: $(TOP)/.git/modules/external/llvm/HEAD
	$(Q) printf 'LLVM_DEPENDENCIES += Makefile \\\n' > $@.tmp
	$(Q) cd $(LLVM_PATH) && git ls-files | grep -v "^test" | sed 's/ /\\ /g' | sed 's@^\(.*\)$$@	$(LLVM_PATH)/\1 \\@' >> $(abspath $@).tmp
	$(Q) mv $@.tmp $@

# ld: weak import of symbol '_fstatat' not supported because of option: -no_weak_imports for architecture x86_64
# according to 'man fstatat' it appeared in OS X 10.10 (iOS 8)
# so disable 'fstatat' by setting using ac_cv_func_fstatat=no
# ld: weak import of symbol '_readlinkat' not supported because of option: -no_weak_imports for architecture x86_64
# according to 'man readlinkat' it appeared in OS X 10.10 (iOS 8)
# so disable 'readlinkat' by setting using ac_cv_func_readlinkat=no
# these can be removed when our min ios/osx version (at runtime) is macOS 10.10+ or iOS 8+

# ld: weak import of symbol '_futimens' not supported because of option: -no_weak_imports for architecture x86_64
# ld: weak import of symbol '_utimensat' not supported because of option: -no_weak_imports for architecture x86_64
# headers say these methods were introduced in 10.13:
# usr/include/sys/stat.h:int	futimens(int __fd, const struct timespec __times[2]) __API_AVAILABLE(macosx(10.13), ios(11.0), tvos(11.0), watchos(4.0));
# int	utimensat(int __fd, const char *__path, const struct timespec __times[2], int __flag) __API_AVAILABLE(macosx(10.13), ios(11.0), tvos(11.0), watchos(4.0));
# so disable 'futimens' and 'utimensat'

COMMON_ACVARS =  \
	ac_cv_func_fstatat=no \
	ac_cv_func_readlinkat=no \
	ac_cv_func_futimens=no \
	ac_cv_func_utimensat=no \

COMMON_LDFLAGS=-Wl,-no_weak_imports

BITCODE_CFLAGS=-fexceptions
BITCODE_LDFLAGS=-framework CoreFoundation -lobjc -lc++
BITCODE_CONFIGURE_FLAGS=--enable-llvm-runtime --with-bitcode=yes

ifdef ENABLE_BITCODE_ON_IOS
IOS_BITCODE_CFLAGS=$(BITCODE_CFLAGS) -fembed-bitcode-marker
IOS_BITCODE_CXXFLAGS=$(BITCODE_CFLAGS) -fembed-bitcode-marker
IOS_BITCODE_LDFLAGS=$(BITCODE_LDFLAGS)
IOS_BITCODE_CONFIGURE_FLAGS=$(BITCODE_CONFIGURE_FLAGS)
endif

#
# The install-* targets should not depend on any of the build-* targets,
# because it will slow down the install targets unnecessarily (the build-*
# targets will recurse into multiple directories, which would be wasteful
# since in most cases the build has already been done)
#
# Currently 'make install' takes about 4 seconds (without running any build-*
# targets)
#
# This means that 'make all install' will not work in this directory, it must
# be 'make all && make install'.
#

# This turns off the clang: warning: argument unused during compilation
# warnings when using clang+ccache together
CCACHE_CFLAGS=-Qunused-arguments
CCACHE_CXXFLAGS=-Qunused-arguments

##
## Xamarin.Mac
##

# We only build the mono runtimes here (mac32 and mac64)
# The xammac classlibs are built as a part of the 'tools64' build.

MAC_ASSEMBLY_NAMES = \
	I18N \
	I18N.CJK \
	I18N.MidEast \
	I18N.Other \
	I18N.Rare \
	I18N.West \
	mscorlib \
	Microsoft.CSharp \
	Mono.CSharp \
	Mono.CompilerServices.SymbolWriter \
	Mono.Data.Sqlite \
	Mono.Data.Tds \
	Mono.Security \
	System.ComponentModel.Composition \
	System.ComponentModel.DataAnnotations \
	System.Core \
	System.Data.Services.Client \
	System.Data \
	System.IdentityModel \
	System.IO.Compression.FileSystem \
	System.IO.Compression \
	System.Json \
	System.Net.Http \
	System.Net \
	System.Numerics \
	System.Runtime.Serialization \
	System.ServiceModel.Web \
	System.ServiceModel \
	System.ServiceModel.Internals \
	System.Transactions \
	System.Web.Services \
	System.Windows \
	System.Xml.Linq \
	System.Xml.Serialization \
	System.Xml \
	System \
	System.Security \
	System.Reflection.Context	\
	System.Net.Http.WinHttpHandler \
	System.Numerics.Vectors

FACADE_SUBDIRS_MK = $(MONO_PATH)/mcs/class/Facades/subdirs.make
# File does not exist before checking out mono
-include $(FACADE_SUBDIRS_MK)

MAC_FACADE_ASSEMBLY_NAMES = $(xammac_PARALLEL_SUBDIRS) $(xammac_SUBDIRS)

MAC_4_5_ADDITIONAL_ASSEMBLY_NAMES = \
	Mono.Messaging \
	Mono.Posix \
	System.Configuration \
	System.Configuration.Install \
	System.Data.Linq \
	System.EnterpriseServices \
	System.IdentityModel.Selectors \
	System.Messaging \
	System.Runtime.Serialization.Formatters.Soap

MAC_4_5_ASSEMBLY_NAMES = $(MAC_ASSEMBLY_NAMES) $(MAC_4_5_ADDITIONAL_ASSEMBLY_NAMES)

MAC_4_5_FACADE_ASSEMBLY_NAMES = $(xammac_net_4_5_PARALLEL_SUBDIRS) $(xammac_net_4_5_SUBDIRS)

MAC_ASSEMBLIES = $(MAC_ASSEMBLY_NAMES)
MAC_FACADE_ASSEMBLIES = $(MAC_FACADE_ASSEMBLY_NAMES)
MAC_NO_MDB_ASSEMBLIES = System.Windows System.Xml.Serialization
MAC_4_5_ASSEMBLIES = $(MAC_4_5_ASSEMBLY_NAMES)
MAC_4_5_FACADE_ASSEMBLIES = $(MAC_4_5_FACADE_ASSEMBLY_NAMES)

MAC_DIRECTORIES = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5 \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Facades \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/include \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/assemblies/System \

MAC_BCL_TARGETS = \
	$(foreach file,$(MAC_ASSEMBLIES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/$(file).dll)                \
	$(foreach file,$(filter-out $(MAC_NO_MDB_ASSEMBLIES),$(MAC_ASSEMBLIES)),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/$(file).pdb)            \
	$(foreach file,$(MAC_FACADE_ASSEMBLIES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/Xamarin.Mac/Facades/$(file).dll) \
	$(foreach file,$(MAC_4_5_ASSEMBLIES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/$(file).dll)                    \
	$(foreach file,$(filter-out $(MAC_NO_MDB_ASSEMBLIES),$(MAC_4_5_ASSEMBLIES)),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/$(file).pdb)                \
	$(foreach file,$(MAC_4_5_FACADE_ASSEMBLIES),$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/4.5/Facades/$(file).dll)     \

MAC_TARGETS = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/libmono-profiler-log.a \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/libmono-2.0.dylib \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/libmono-2.0.a \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig/mono-2.pc \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/assemblies/System/System.config \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/bmac-mobile-mono \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/bmac-mobile-mono-32 \
	$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current \
	$(MAC_DIRECTORIES) \

define MacBuildTemplate
$(2)_CPPFLAGS = \
	-isysroot $(XCODE_MAC_SDKROOT) \
	-mmacosx-version-min=$(MIN_OSX_SDK_VERSION)
$(2)_CFLAGS = -O2 -arch $(1)
$(2)_CXXFLAGS = -O2 -arch $(1)
$(2)_LDFLAGS = $(COMMON_LDFLAGS)
$(2)_CONFIGURE_FLAGS = \
	--prefix=$$(BUILD_DESTDIR)/$(2) \
	--host=$(1)-apple-darwin10 \
	--cache-file=../$(2).config.cache \
	--enable-maintainer-mode \
	--with-glib=embedded \
	--with-mcs-docs=no \
	--disable-nls \
	--disable-iconv \
	--disable-mcs-build \
	--disable-boehm \
	$(XAMARIN_MAC_CONFIGURE_FLAGS) \

$(2)_ACVARS = $(COMMON_ACVARS)

$(2)_CONFIGURE_ENVIRONMENT = \
	$$($(2)_ACVARS) \
	PATH="$(MONO_PREFIX)/bin:$(PATH)" \
	CPPFLAGS="$$($(2)_CPPFLAGS)" \
	CFLAGS="$$($(2)_CFLAGS)" \
	CXXFLAGS="$$($(2)_CXXFLAGS)" \
	LDFLAGS="$$($(2)_LDFLAGS)" \
	CC="$(MAC_CC)" \
	CXX="$(MAC_CXX)" \
	DEVELOPER_DIR="$(XCODE_DEVELOPER_ROOT)" \

setup:: setup-$(2)

setup-$(2): .stamp-configure-$(2)

.stamp-configure-$(2): $(MONO_PATH)/configure
	$(Q) $$($(2)_CONFIGURE_ENVIRONMENT) ./wrap-configure.sh $(2) $(abspath $(MONO_PATH)/configure) $$($(2)_CONFIGURE_FLAGS)

$(BUILD_DESTDIR)/mono-$(2): mono-wrapper.in .stamp-configure-$(2) | $(BUILD_DESTDIR)
	$$(Q_GEN) sed < $$< > $$@ 's|@ARCH@|$(1)|g; s|@MONO@|$(realpath .)/$(2)/mono/mini/mono|g; s|@FRAMEWORK_ROOT@|$(abspath $(MAC_DESTDIR)/$$(MAC_FRAMEWORK_CURRENT_DIR))|g'
	$$(Q) chmod +x $$@

.stamp-build-$(2): .stamp-configure-$(2) $(BUILD_DESTDIR)/mono-$(2) $(MONO_DEPENDENCIES)
	PATH="$(MONO_PREFIX)/bin:$(PATH)" $(MAKE) -C $(2)/mono
	PATH="$(MONO_PREFIX)/bin:$(PATH)" $(MAKE) -C $(2)/mono install
	PATH="$(MONO_PREFIX)/bin:$(PATH)" $(MAKE) -C $(2)/support
	$(Q) touch $$@

build-$(2): .stamp-build-$(2)
endef

$(eval $(call MacBuildTemplate,i386,mac32))
$(eval $(call MacBuildTemplate,x86_64,mac64))

$(MONO_PATH)/mcs/class/lib/%: .stamp-build-tools64; @true

define MacInstallBclTemplate

MAC_DIRECTORIES += \
	$$(BUILD_DESTDIR)/bcl/$(2)/Facades \
	$$(BUILD_DESTDIR)/bcl/$(2)         \

$$(MAC_DESTDIR)$$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/$(1)/%: $$(BUILD_DESTDIR)/bcl/$(2)/% | $$(MAC_DESTDIR)$$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/$(1)/Facades
	$$(Q) install -m 0755 $$< $$@


$$(MAC_DESTDIR)$$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/$(1)/%.pdb: $$(BUILD_DESTDIR)/mac/$(2)/bcl/%.pdb | $$(MAC_DESTDIR)$$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mono/$(1)/Facades
	$$(Q) install -m 0644 $$< $$@
endef

$(eval $(call MacInstallBclTemplate,Xamarin.Mac,xammac))
$(eval $(call MacInstallBclTemplate,4.5,xammac_net_4_5))

$(MAC_DIRECTORIES) $(BUILD_DESTDIR):
	$(Q) mkdir -p $@

mac-facade-check: $(MAC_BCL_TARGETS)
	$(Q) rm -f .$@*
	$(Q) echo $(MAC_4_5_FACADE_ASSEMBLIES) | tr ' ' '\n' | sort > .$@1
	$(Q) ls -1 $(MONO_PATH)/mcs/class/lib/xammac_net_4_5/Facades | grep dll$$ | sed 's/[.]dll//' | sort > .$@2
	$(Q) if ! diff -u .$@1 .$@2; then echo "\n*** There are Facade assemblies in "$(MONO_PATH)/mcs/class/lib/xammac_net_4_5/Facades " not defined in "$(FACADE_SUBDIRS_MK)" ***\n"; exit 1; fi
	$(Q) rm -f .$@*

define lipo_template_static
$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/$1: mac32/$(2)/.libs/$(1) mac64/$(2)/.libs/$(1) | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib
	$(QT_LIPO) lipo -create $$+ -output $$@

mac32/$(2)/.libs/$(1): .stamp-build-mac32
mac64/$(2)/.libs/$(1): .stamp-build-mac64
MAC_TARGETS += $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/$1
endef

define lipo_template_dynamic
$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/$1: mac32/$(2)/.libs/$(1) mac64/$(2)/.libs/$(1) | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib
	$(QT_LIPO) lipo -create $$+ -output $$@
	$(Q) install_name_tool -id @rpath/$1 $$@

mac32/$(2)/.libs/$(1): .stamp-build-mac32
mac64/$(2)/.libs/$(1): .stamp-build-mac64
MAC_TARGETS += $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/$1
endef

$(eval $(call lipo_template_static,libmono-profiler-log.a,mono/profiler))
$(eval $(call lipo_template_static,libmonosgen-2.0.a,mono/mini))
$(eval $(call lipo_template_dynamic,libmonosgen-2.0.dylib,mono/mini))
$(eval $(call lipo_template_dynamic,libMonoPosixHelper.dylib,support))

mac32/data/mono-2.pc: .stamp-build-mac32
install/mac64/bin/mono-sgen: .stamp-build-mac64
install/mac32/bin/mono-sgen: .stamp-build-mac32

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/libmono-2.0.dylib $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/libmono-2.0.a: | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib
	$(Q_LN) ln -sf libmonosgen-2.0$(suffix $@) $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig/mono-2.pc: mac32/data/mono-2.pc | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/pkgconfig
	$(Q_CP) cp $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/assemblies/System/System.config: mac-System.config | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/etc/mono/assemblies/System
	$(Q_CP) cp $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/bmac-mobile-mono: install/mac64/bin/mono-sgen | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/bmac-mobile-mono-32: install/mac32/bin/mono-sgen | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin
	$(Q) install -m 0755 $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Versions/Current:
	$(Q_LN) ln -hfs $(MAC_INSTALL_VERSION) $(MAC_DESTDIR)$(MAC_FRAMEWORK_DIR)/Versions/Current

build:: build-mac
build-mac: $(MAC_TARGETS)

install-local:: install-mac
install-mac: $(MAC_TARGETS)

clean-local:: clean-mac

clean-mac: clean-mac32 clean-mac64

clean-mac32:
	rm -rf mac32 .stamp-configure-mac32 mac32.config.cache

clean-mac64:
	rm -rf mac64 .stamp-configure-mac64 mac64.config.cache

##
## Xamarin.iOS
##

all-local:: build

ifdef INCLUDE_IOS
all-local:: $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/smcs
endif

setup::

ifdef INCLUDE_IOS
install-local:: $(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/smcs
endif

clean-local::
	@rm -rf $(BUILD_DESTDIR) .stamp-configure-*
	@rm -f $(MONO_PATH)/configure

ifdef DISABLE_BUILDS_MAKEFILE_DEP
BUILDS_MAKEFILE_DEP =
else
BUILDS_MAKEFILE_DEP = Makefile
endif

$(MONO_PATH)/configure: $(MONO_PATH)/configure.ac $(BUILDS_MAKEFILE_DEP)
	$(Q) ./wrap-autogen.sh $(MONO_PATH) mono $(XAMARIN_AUTOGEN_FLAGS)

ifneq ($(WATCH_MONO_PATH),$(MONO_PATH))
$(WATCH_MONO_PATH)/configure: $(WATCH_MONO_PATH)/configure.ac $(BUILDS_MAKEFILE_DEP)
	$(Q) ./wrap-autogen.sh $(WATCH_MONO_PATH) mono
endif

configure-mono:
	rm -f $(MONO_PATH)/configure
	$(MAKE) $(MONO_PATH)/configure

# FIXME: Needed because of broken MonoDevelop assumptions
$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)/bin/smcs:
	@mkdir -p $(dir $@)
	@echo Installing smcs script
	@echo "#!/bin/bash" > $@
	@echo >> $@
	@echo 'SMCS=`which mcs`' >> $@
	@echo 'if test -z $$SMCS; then' >> $@
	@echo "    SMCS=/Library/Frameworks/Mono.framework/Versions/Current/bin/mcs" >> $@
	@echo "fi" >> $@
	@echo 'unset MONO_PATH' >> $@
	@echo "\$$SMCS -nostdlib -r:$(abspath $(IOS_TARGETDIR)/$(MONOTOUCH_PREFIX)/lib/mono/2.1/mscorlib.dll) -r:$(abspath $(IOS_TARGETDIR)/$(MONOTOUCH_PREFIX)/lib/mono/2.1/System.dll) \"\$$@\"" >> $@
	@chmod +x $@

#
# Tools 64bit build.  Builds all the class libs as well.
#
TOOLS64_CFLAGS=-arch x86_64 -mmacosx-version-min=$(MIN_OSX_SDK_VERSION)
TOOLS64_CXXFLAGS=-arch x86_64 -mmacosx-version-min=$(MIN_OSX_SDK_VERSION)
TOOLS64_LDFLAGS=-arch x86_64 -mmacosx-version-min=$(MIN_OSX_SDK_VERSION) $(COMMON_LDFLAGS)

TOOLS64_CONFIGURE_FLAGS=	--build=x86_64-apple-darwin10 \
			--with-monotouch_tv=yes \
			--prefix=$(BUILD_DESTDIR)/tools64 \
			--enable-maintainer-mode \
			--cache-file=../tools64.config.cache \
			--with-glib=embedded \
			--enable-minimal=com	\
			--with-profile4_x=no \
			--with-monotouch=yes \
			--with-xammac=yes \
			--with-mcs-docs=no \
			--disable-nls \
			--disable-iconv	\
			--disable-boehm \
			$(XAMARIN_IOS_CONFIGURE_FLAGS) \

ifeq ($(WATCH_MONO_PATH),$(MONO_PATH))
TOOLS64_CONFIGURE_FLAGS += --with-monotouch_watch=yes
else
TOOLS64_CONFIGURE_FLAGS += --with-monotouch_watch=no
endif

TOOLS64_ACVARS = $(COMMON_ACVARS)

TOOLS64_CONFIGURE_ENVIRONMENT = \
	$(TOOLS64_ACVARS) \
	CFLAGS="$(TOOLS64_CFLAGS)" \
	CXXFLAGS="$(TOOLS64_CXXFLAGS)" \
	LDFLAGS="$(TOOLS64_LDFLAGS)" \
	DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT) \
	CC="$(MAC_CC)" \
	CXX="$(MAC_CXX)" \

tools:: build-tools64 install-tools

setup:: setup-tools64
build:: build-tools64
clean-local:: clean-tools64

tools64: build-tools64 install-tools

ifdef INCLUDE_IOS
install-local:: install-tools-ios
install-tools:: install-tools-ios
endif
install-local:: install-tools-mac
install-tools:: install-tools-mac

setup-tools64: .stamp-configure-tools64

.stamp-configure-tools64: $(MONO_PATH)/configure
	$(Q) $(TOOLS64_CONFIGURE_ENVIRONMENT) ./wrap-configure.sh tools64 $(abspath $(MONO_PATH)/configure) $(TOOLS64_CONFIGURE_FLAGS)

build-tools-bcl: build-tools64
build-tools: build-tools64

.stamp-build-tools64: .stamp-configure-tools64 $(MONO_DEPENDENCIES)
	$(MAKE) -C tools64 all EXTERNAL_MCS=$(SYSTEM_MCS) EXTERNAL_RUNTIME=$(SYSTEM_MONO) MONOTOUCH_MCS_FLAGS=$(IOS_MCS_FLAGS) XAMMAC_MCS_FLAGS=$(MAC_MCS_FLAGS)
	# NO_INSTALL is defined in mono/mcs/build/profiles except for net_4_5. If we don't have it, then it attempts to install into a gac, and sometimes fail
	NO_INSTALL=1 $(MAKE) -C tools64 -j 1 install EXTERNAL_MCS=$(SYSTEM_MCS) EXTERNAL_RUNTIME=$(SYSTEM_MONO)
	@touch .stamp-build-tools64

clean-tools64:
	rm -rf tools64 .stamp-*-tools64 tools64.config.cache
	rm -f .stamp-build-tools64

ifdef INCLUDE_WATCH
install-local:: install-tools-watch
install-tools:: install-tools-watch
endif
ifdef INCLUDE_TVOS
install-local:: install-tools-tvos
install-tools:: install-tools-tvos
endif

#
# Xamarin.WatchOS BCL assemblies
#
# We need to build the WatchOS BCL using the same mono checkout as we're using for the
# corresponding Mono runtimes, which means we can't use the normal tools64 build
#
# Also we can't reuse the existing watchsimulator/targetwatch builds, since their
# monos don't run (correctly) on desktop OSX.
#
# So we build an entire Mono just to build the WatchOS BCL.
#

WATCHBCL_CFLAGS=-arch x86_64 -mmacosx-version-min=$(MIN_OSX_SDK_VERSION)
WATCHBCL_CXXFLAGS=-arch x86_64 -mmacosx-version-min=$(MIN_OSX_SDK_VERSION)
WATCHBCL_LDFLAGS=-arch x86_64 -mmacosx-version-min=$(MIN_OSX_SDK_VERSION) $(COMMON_LDFLAGS)
WATCHBCL_ACVARS = $(COMMON_ACVARS)


WATCHBCL_CONFIGURE_FLAGS=	--build=x86_64-apple-darwin10 \
			--prefix=$(BUILD_DESTDIR)/watchbcl \
			--enable-maintainer-mode \
			--cache-file=../watchbcl.config.cache \
			--with-glib=embedded \
			--enable-minimal=com	\
			--with-profile4_x=no \
			--with-monotouch=no \
			--with-monotouch_watch=yes \
			--with-monotouch_tv=no \
			--with-xammac=no \
			--with-mcs-docs=no \
			--disable-nls \
			--disable-iconv	\
			--disable-boehm \
			$(XAMARIN_IOS_CONFIGURE_FLAGS)

WATCHBCL_CONFIGURE_ENVIRONMENT = \
	$(WATCHBCL_ACVARS) \
	CFLAGS="$(WATCHBCL_CFLAGS)" \
	CXXFLAGS="$(WATCHBCL_CXXFLAGS)" \
	LDFLAGS="$(WATCHBCL_LDFLAGS)" \
	CC="$(MAC_CC)" \
	CXX="$(MAC_CXX)" \

ifdef INCLUDE_WATCH
setup:: setup-watchbcl
build:: build-watchbcl
clean-local:: clean-watchbcl
endif

watchbcl: build-watchbcl install-tools-watch

setup-watchbcl: .stamp-configure-watchbcl

ifneq ($(WATCH_MONO_PATH),$(MONO_PATH))
.stamp-configure-watchbcl: $(WATCH_MONO_PATH)/configure
	$(Q) $(WATCHBCL_CONFIGURE_ENVIRONMENT) ./wrap-configure.sh watchbcl ../$(WATCH_MONO_PATH)/configure $(WATCHBCL_CONFIGURE_FLAGS)
endif

build-tools-bcl: build-watchbcl
build-tools: build-watchbcl

ifeq ($(WATCH_MONO_PATH),$(MONO_PATH))
.stamp-build-watchbcl: .stamp-build-tools64 $(MONO_DEPENDENCIES)
else
.stamp-build-watchbcl: .stamp-configure-watchbcl .stamp-build-tools64 $(MONO_DEPENDENCIES)
	$(MAKE) -C watchbcl all EXTERNAL_MCS=$(SYSTEM_MCS) EXTERNAL_RUNTIME=$(SYSTEM_MONO)
	$(MAKE) -C watchbcl install EXTERNAL_MCS=$(SYSTEM_MCS) EXTERNAL_RUNTIME=$(SYSTEM_MONO)
endif
	$(Q) touch $@

clean-watchbcl:
	rm -rf watchbcl .stamp-*-watchbcl watchbcl.config.cache

#
# Xamarin.iOS/WatchOS/TVOS BCL assemblies
#

# note: Mono.Cairo is probably used for Android only (removed from xtouch list)
IOS_ASSEMBLIES = I18N.CJK I18N.MidEast I18N.Other I18N.Rare I18N.West I18N Mono.Data.Sqlite Mono.Data.Tds Mono.Security mscorlib \
	System System.Core System.Data System.Data.Services.Client System.Json System.Runtime.Serialization \
	System.ServiceModel.Web System.ServiceModel System.Transactions System.Web.Services System.Xml.Linq System.Xml \
	System.ServiceModel.Internals \
	System.Numerics System.IdentityModel System.IO.Compression System.IO.Compression.FileSystem \
	System.ComponentModel.DataAnnotations System.ComponentModel.Composition \
	System.Net.Http.WinHttpHandler \
	System.Numerics.Vectors System.Reflection.Context \
	System.Security \
	System.Windows System.Xml.Serialization System.Net \
	Microsoft.CSharp Mono.CSharp

IOS_REPL_ASSEMBLIES = mscorlib System System.Core System.Xml Mono.CSharp

IOS_FACADE_ASSEMBLIES = $(monotouch_PARALLEL_SUBDIRS) $(monotouch_SUBDIRS)

# TVOS/WATCHOS BCL is built from a different mono branch and the assembly list might differ across time
TVOS_ASSEMBLIES = $(IOS_ASSEMBLIES)
WATCHOS_ASSEMBLIES = $(filter-out Mono.Security Mono.Data.Tds,$(IOS_ASSEMBLIES))
TVOS_FACADE_ASSEMBLIES = $(IOS_FACADE_ASSEMBLIES)
WATCHOS_FACADE_ASSEMBLIES = $(IOS_FACADE_ASSEMBLIES)
TVOS_REPL_ASSEMBLIES = $(IOS_REPL_ASSEMBLIES)
WATCHOS_REPL_ASSEMBLIES = $(IOS_REPL_ASSEMBLIES)

ios-facade-check: $(IOS_BCL_TARGETS)
	$(Q) rm -f .$@*
	$(Q) echo $(IOS_FACADE_ASSEMBLIES) | tr ' ' '\n' | sort > .$@1
	$(Q) ls -1 $(MONO_PATH)/mcs/class/lib/monotouch/Facades | grep dll$$ | sed 's/[.]dll//' | sort > .$@2
	$(Q) if ! diff -u .$@1 .$@2; then echo "\n*** There are Facade assemblies in "$(MONO_PATH)/mcs/class/lib/monotouch/Facades" not defined in "$(FACADE_SUBDIRS_MK)"***\n"; exit 1; fi
	$(Q) rm -f .$@*

watch-facade-check: $(WATCH_BCL_TARGETS)
	@true

disabled-watch-facade-check: $(WATCH_BCL_TARGETS)
	$(Q) rm -f .$@*
	$(Q) echo $(WATCHOS_FACADE_ASSEMBLIES) | tr ' ' '\n' | sort > .$@1
	$(Q) ls -1 $(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/Facades | grep dll$$ | sed 's/[.]dll//' | sort > .$@2
	$(Q) if ! diff -u .$@1 .$@2; then echo "\n*** There are Facade assemblies in "$(WATCH_MONO_PATH)/mcs/class/lib/monotouch_watch/Facades" not defined in "$(FACADE_SUBDIRS_MK)" ***\n"; exit 1; fi
	$(Q) rm -f .$@*

tvos-facade-check: $(TVOS_BCL_TARGETS) 
	$(Q) rm -f .$@*
	$(Q) echo $(TVOS_FACADE_ASSEMBLIES) | tr ' ' '\n' | sort > .$@1
	$(Q) ls -1 $(MONO_PATH)/mcs/class/lib/monotouch_tv/Facades | grep dll$$ | sed 's/[.]dll//' | sort > .$@2
	$(Q) if ! diff -u .$@1 .$@2; then echo "\n*** There are Facade assemblies in "$(MONO_PATH)/mcs/class/lib/monotouch_tv/Facades" not defined in "$(FACADE_SUBDIRS_MK)" ***\n"; exit 1; fi
	$(Q) rm -f .$@*

NO_MDB_ASSEMBLIES = System.Windows System.Xml.Serialization

IOS_BCL_TARGETS_DIRS +=                    \
	$(PREFIX)/lib/mono/2.1                 \
	$(PREFIX)/lib/mono/2.1/Facades         \
	$(PREFIX)/lib/mono/Xamarin.iOS         \
	$(PREFIX)/lib/mono/Xamarin.iOS/Facades \
	$(PREFIX)/lib/mono/2.1/repl            \
	$(PREFIX)/lib/mono/Xamarin.iOS/repl    \
	$(BUILD_DESTDIR)/bcl/monotouch         \
	$(BUILD_DESTDIR)/bcl/monotouch/Facades \
	$(BUILD_DESTDIR)/bcl/monotouch_runtime \

WATCH_BCL_TARGETS_DIRS +=                      \
	$(PREFIX)/lib/mono/Xamarin.WatchOS         \
	$(PREFIX)/lib/mono/Xamarin.WatchOS/Facades \
	$(PREFIX)/lib/mono/Xamarin.WatchOS/repl    \
	$(BUILD_DESTDIR)/bcl/monotouch_watch         \
	$(BUILD_DESTDIR)/bcl/monotouch_watch/Facades \
	$(BUILD_DESTDIR)/bcl/monotouch_watch_runtime \

TVOS_BCL_TARGETS_DIRS +=                    \
	$(PREFIX)/lib/mono/Xamarin.TVOS         \
	$(PREFIX)/lib/mono/Xamarin.TVOS/Facades \
	$(PREFIX)/lib/mono/Xamarin.TVOS/repl    \
	$(BUILD_DESTDIR)/bcl/monotouch_tv         \
	$(BUILD_DESTDIR)/bcl/monotouch_tv/Facades \
	$(BUILD_DESTDIR)/bcl/monotouch_tv_runtime \

IOS_BCL_TARGETS +=                                                                                                  \
	$(foreach file,$(IOS_ASSEMBLIES),$(PREFIX)/lib/mono/2.1/$(file).dll)                                                \
	$(foreach file,$(filter-out $(NO_MDB_ASSEMBLIES),$(IOS_ASSEMBLIES)),$(PREFIX)/lib/mono/2.1/$(file).pdb)         \
	$(foreach file,$(IOS_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.iOS/$(file).dll)                                        \
	$(foreach file,$(filter-out $(NO_MDB_ASSEMBLIES),$(IOS_ASSEMBLIES)),$(PREFIX)/lib/mono/Xamarin.iOS/$(file).pdb) \
	$(PREFIX)/lib/mono/Xamarin.iOS/mscorlib.dll                                                                     \
	$(PREFIX)/lib/mono/2.1/mscorlib.dll                                                                             \
	$(foreach file,$(IOS_FACADE_ASSEMBLIES),$(PREFIX)/lib/mono/2.1/Facades/$(file).dll)                                 \
	$(foreach file,$(IOS_FACADE_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.iOS/Facades/$(file).dll)                         \
	$(foreach file,$(IOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/2.1/repl/$(file).dll)                                      \
	$(foreach file,$(IOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/2.1/repl/$(file).pdb)                                  \
	$(foreach file,$(IOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.iOS/repl/$(file).dll)                              \
	$(foreach file,$(IOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.iOS/repl/$(file).pdb)                          \

WATCH_BCL_TARGETS += \
	$(foreach file,$(WATCHOS_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.WatchOS/$(file).dll)                                        \
	$(foreach file,$(filter-out $(NO_MDB_ASSEMBLIES),$(WATCHOS_ASSEMBLIES)),$(PREFIX)/lib/mono/Xamarin.WatchOS/$(file).pdb) \
	$(PREFIX)/lib/mono/Xamarin.WatchOS/mscorlib.dll                                                                             \
	$(foreach file,$(WATCHOS_FACADE_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.WatchOS/Facades/$(file).dll)                         \
	$(foreach file,$(WATCHOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.WatchOS/repl/$(file).dll)                              \
	$(foreach file,$(WATCHOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.WatchOS/repl/$(file).pdb)                          \

TVOS_BCL_TARGETS += \
	$(foreach file,$(TVOS_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.TVOS/$(file).dll)                                        \
	$(foreach file,$(filter-out $(NO_MDB_ASSEMBLIES),$(TVOS_ASSEMBLIES)),$(PREFIX)/lib/mono/Xamarin.TVOS/$(file).pdb) \
	$(PREFIX)/lib/mono/Xamarin.TVOS/mscorlib.dll                                                                          \
	$(foreach file,$(TVOS_FACADE_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.TVOS/Facades/$(file).dll)                         \
	$(foreach file,$(TVOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.TVOS/repl/$(file).dll)                              \
	$(foreach file,$(TVOS_REPL_ASSEMBLIES),$(PREFIX)/lib/mono/Xamarin.TVOS/repl/$(file).pdb)                          \

$(PREFIX)/lib/mono/Xamarin.iOS/repl/%: $(BUILD_DESTDIR)/bcl/monotouch_runtime/% | $(PREFIX)/lib/mono/Xamarin.iOS/repl
	$(Q) install -m 0755 $< $@

$(PREFIX)/lib/mono/Xamarin.iOS/%: $(BUILD_DESTDIR)/bcl/monotouch/% | $(PREFIX)/lib/mono/Xamarin.iOS $(PREFIX)/lib/mono/Xamarin.iOS/Facades
	$(Q) install -m 0644 $< $@

$(PREFIX)/lib/mono/2.1/repl/%: $(BUILD_DESTDIR)/bcl/monotouch_runtime/% | $(PREFIX)/lib/mono/2.1/repl
	$(Q) install -m 0755 $< $@

$(PREFIX)/lib/mono/2.1/%: $(BUILD_DESTDIR)/bcl/monotouch/% | $(PREFIX)/lib/mono/2.1 $(PREFIX)/lib/mono/2.1/Facades
	$(Q) install -m 0755 $< $@

$(PREFIX)/lib/mono/Xamarin.WatchOS/repl/%: $(BUILD_DESTDIR)/bcl/monotouch_watch_runtime/% | $(PREFIX)/lib/mono/Xamarin.WatchOS/repl
	$(Q) install -m 0755 $< $@

$(PREFIX)/lib/mono/Xamarin.WatchOS/%: $(BUILD_DESTDIR)/bcl/monotouch_watch/% | $(PREFIX)/lib/mono/Xamarin.WatchOS $(PREFIX)/lib/mono/Xamarin.WatchOS/Facades
	$(Q) install -m 0755 $< $@

$(PREFIX)/lib/mono/Xamarin.TVOS/repl/%: $(BUILD_DESTDIR)/bcl/monotouch_tv_runtime/% | $(PREFIX)/lib/mono/Xamarin.TVOS/repl
	$(Q) install -m 0755 $< $@

$(PREFIX)/lib/mono/Xamarin.TVOS/%: $(BUILD_DESTDIR)/bcl/monotouch_tv/% | $(PREFIX)/lib/mono/Xamarin.TVOS $(PREFIX)/lib/mono/Xamarin.TVOS/Facades
	$(Q) install -m 0755 $< $@

# copy to temporary directory before signing
# otherwise we end up re-signing if installing into a different directory.

TMP_BCL_TARGET_DIRS = \
	$(BUILD_DESTDIR)/bcl/monotouch \
	$(BUILD_DESTDIR)/bcl/monotouch/Facades \
	$(BUILD_DESTDIR)/bcl/monotouch_runtime \
	$(BUILD_DESTDIR)/bcl/monotouch_tv \
	$(BUILD_DESTDIR)/bcl/monotouch_tv/Facades \
	$(BUILD_DESTDIR)/bcl/monotouch_tv_runtime \
	$(BUILD_DESTDIR)/bcl/monotouch_watch \
	$(BUILD_DESTDIR)/bcl/monotouch_watch/Facades \
	$(BUILD_DESTDIR)/bcl/monotouch_watch_runtime \
	$(BUILD_DESTDIR)/bcl/xammac \
	$(BUILD_DESTDIR)/bcl/xammac/Facades \
	$(BUILD_DESTDIR)/bcl/xammac_net_4_5 \
	$(BUILD_DESTDIR)/bcl/xammac_net_4_5/Facades \

$(BUILD_DESTDIR)/bcl/%.dll: $(MONO_PATH)/mcs/class/lib/%.dll | $(TMP_BCL_TARGET_DIRS)
	$(Q) cp $< $@
	$(Q_SN) MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -R $@ $(PRODUCT_KEY_PATH)

$(BUILD_DESTDIR)/bcl/%.pdb: $(MONO_PATH)/mcs/class/lib/%.pdb | $(TMP_BCL_TARGET_DIRS)
	$(Q) cp $< $@

$(IOS_BCL_TARGETS_DIRS) $(WATCH_BCL_TARGETS_DIRS) $(TVOS_BCL_TARGETS_DIRS):
	$(Q) mkdir -p $@

# Keep all intermediate files always.
.SECONDARY:

install-tools-tvos: $(TVOS_BCL_TARGETS) tvos-facade-check
	@true

install-tools-watch: $(WATCH_BCL_TARGETS) watch-facade-check
	@true

install-tools-ios: $(IOS_BCL_TARGETS) ios-facade-check
	@true

install-tools-mac: $(MAC_BCL_TARGETS) mac-facade-check
	@true

build-watchbcl: $(WATCH_BCL_TARGETS)
	@true

$(IOS_BCL_TARGETS): .stamp-build-tools64
$(TVOS_BCL_TARGETS): .stamp-build-tools64
$(MAC_BCL_TARGETS): .stamp-build-tools64
$(WATCH_BCL_TARGETS): .stamp-build-tools64

ifdef INCLUDE_IOS
build-tools64: $(IOS_BCL_TARGETS)
endif
ifdef INCLUDE_TVOS
build-tools64: $(TVOS_BCL_TARGETS)
endif
ifdef INCLUDE_WATCH
build-tools64: $(WATCH_BCL_TARGETS)
endif

build-tools64: $(MAC_BCL_TARGETS)
	@true

verify-signature:
	for file in $(PREFIX)/lib/mono/2.1/*.dll; do \
		MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $$file;	\
	done
	@MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $(PREFIX)/lib/mono/2.1/monotouch.dll			\
	@MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $(PREFIX)/lib/mono/2.1/MonoTouch.Dialog.dll		\
	@MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $(PREFIX)/lib/mono/2.1/MonoTouch.NUnitLite.dll	\
	@MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $(PREFIX)/lib/mono/2.1/OpenTK.dll			\
	@MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $(PREFIX)/lib/mono/2.1/OpenTK-10.dll
	for file in $(IOS_FACADE_ASSEMBLIES); do \
		MONO_CFG_DIR=$(TOP) $(SYSTEM_SN) -q -v $(PREFIX)/lib/mono/2.1/Facades/$$file.dll;	\
	done

#
# Simulator builds
#

# usage $(call SimulatorBuildTemplate (x86_64,simulator64))

SIM_BIN=$(SIMULATOR_BIN_PATH)
SIM_ACVARS = \
	$(COMMON_ACVARS) \
	ac_cv_func_clock_nanosleep=no \
	mono_cv_uscore=yes \
	ac_cv_func_system=no \

define SimulatorBuildTemplate

$(eval SIM$(2)_CPPFLAGS=-arch $(1) -O2 -DMONOTOUCH -DHOST_IOS -Wl,-application_extension -mios-simulator-version-min=$(MIN_IOS_SDK_VERSION) -isysroot $(SIMULATOR_SDK))
$(eval SIM$(2)_CFLAGS=$(SIM$(2)_CPPFLAGS) $(SIMULATOR_BUILD_CFLAGS))
$(eval SIM$(2)_CXXFLAGS=$(SIM$(2)_CPPFLAGS))
$(eval SIM$(2)_LDFLAGS=$(COMMON_LDFLAGS))
$(eval SIM$(2)_CONFIGURE_FLAGS=	--host=$(1)-apple-darwin10 \
			--cache-file=../$(2).config.cache \
			--enable-maintainer-mode	\
			--prefix=$(BUILD_DESTDIR)/$(2) \
			--with-glib=embedded \
			--without-ikvm-native \
			--with-tls=pthread \
			--enable-minimal=com,remoting,shared_perfcounters \
			--disable-mcs-build \
			--disable-nls \
			--disable-iconv \
			--disable-boehm \
			--disable-executables \
			--disable-visibility-hidden	\
			$(XAMARIN_IOS_CONFIGURE_FLAGS) \
			)

$(eval SIM$(2)_CONFIGURE_ENVIRONMENT= \
	$(SIM_ACVARS) \
	PATH="$(SIM_BIN):$(PATH)" \
	CPPFLAGS="$(SIM$(2)_CPPFLAGS)" \
	CFLAGS="$(SIM$(2)_CFLAGS)" \
	CXXFLAGS="$(SIM$(2)_CXXFLAGS)" \
	LDFLAGS="$(SIM$(2)_LDFLAGS)" \
	CC="$(XCODE_CC)" \
	CXX="$(XCODE_CXX)")

.PHONY: $(2)

$(2):: build-$(2)

setup:: setup-$(2)
build:: build-$(2)
clean-local:: clean-$(2)

setup-$(2): .stamp-configure-$(2)

.stamp-configure-$(2): $(MONO_PATH)/configure
	$(Q) $(SIM$(2)_CONFIGURE_ENVIRONMENT) ./wrap-configure.sh $(2) $(abspath $(MONO_PATH)/configure) $(SIM$(2)_CONFIGURE_FLAGS)

.stamp-build-$(2): .stamp-configure-$(2) $(MONO_DEPENDENCIES)
	PATH="$(SIM_BIN):$(PATH)" $(MAKE) -C $(2)/mono all
	PATH="$(SIM_BIN):$(PATH)" $(MAKE) -C $(2)/support all
	PATH="$(SIM_BIN):$(PATH)" $(MAKE) -C $(2)/mono install
	$(Q) touch $$@

build-$(2): .stamp-build-$(2)

clean-$(2):
	-rm -rf $(2) .stamp-*-$(2) $(2).config.cache

setup-iphonesimulator:: setup-$(2)
build-iphonesimulator:: build-$(2)
clean-iphonesimulator:: clean-$(2)

$(eval SIM_TARGET_LIBMONOSGEN:=$(SIM_TARGET_LIBMONOSGEN) $(BUILD_DESTDIR)/$(2)/lib/libmonosgen-2.0.a)
$(eval SIM_TARGET_SHAREDMONOSGEN:=$(SIM_TARGET_SHAREDMONOSGEN) $(BUILD_DESTDIR)/$(2)/lib/libmonosgen-2.0.dylib)
$(eval SIM_TARGET_LIBLOGPROFILER:=$(SIM_TARGET_LIBLOGPROFILER) $(BUILD_DESTDIR)/$(2)/lib/libmono-profiler-log-static.a)
$(eval SIM_TARGET_SHAREDLIBLOGPROFILER:=$(SIM_TARGET_SHAREDLIBLOGPROFILER) $(BUILD_DESTDIR)/$(2)/tmp-lib/libmono-profiler-log.0.dylib)

$(BUILD_DESTDIR)/$(2)/lib/libmonosgen-2.0.a: .stamp-build-$(2)
$(BUILD_DESTDIR)/$(2)/lib/libmonosgen-2.0.dylib: .stamp-build-$(2)
$(BUILD_DESTDIR)/$(2)/lib/libmono-profiler-log-static.a: .stamp-build-$(2)
$(BUILD_DESTDIR)/$(2)/lib/libmono-profiler-log.0.dylib: .stamp-build-$(2)

$(BUILD_DESTDIR)/$(2)/tmp-lib/libmono-profiler-log.0.dylib: $(BUILD_DESTDIR)/$(2)/lib/libmono-profiler-log.0.dylib | $(BUILD_DESTDIR)/$(2)/tmp-lib
	$(Q) cp $$< $$@
	$(Q) install_name_tool -id @rpath/libmono-profiler-log.dylib -change $(BUILD_DESTDIR)/$(2)/lib/libmonosgen-2.0.1.dylib @rpath/libmonosgen-2.0.dylib $$@

$(BUILD_DESTDIR)/$(2)/tmp-lib:
	$$(Q) mkdir -p $$@

endef

ifdef INCLUDE_IOS
$(eval $(call SimulatorBuildTemplate,i386,simulator86))
$(eval $(call SimulatorBuildTemplate,x86_64,simulator64))
endif

iphonesimulator:: setup-iphonesimulator build-iphonesimulator install-iphonesimulator

# this needs to be here because the normal usage of this makefile is "make all install", but nothing actually lists the -iphonesimulator targets as dependencies.
ifdef INCLUDE_IOS
install-local:: install-iphonesimulator
all-local:: install-iphonesimulator
endif

IPHONESIMULATOR_TARGETS = \
	$(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib/libmonosgen-2.0.a             \
	$(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib/libmonosgen-2.0.dylib         \
	$(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib/libmono-profiler-log.a        \
	$(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib/libmono-profiler-log.dylib    \
	$(IOS_DESTDIR)$(IPHONESIMULATOR_SDK)/Frameworks/Mono.framework/Mono       \
	$(IOS_DESTDIR)$(IPHONESIMULATOR_SDK)/Frameworks/Mono.framework/Info.plist \

IPHONESIMULATOR_DIRECTORIES = \
	$(IOS_DESTDIR)$(IPHONESIMULATOR_SDK)/Frameworks/Mono.framework            \
	$(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib

$(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib/libmonosgen-2.0.a: $(SIM_TARGET_LIBMONOSGEN) | $(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib
	$(Q) lipo $(SIM_TARGET_LIBMONOSGEN) -create -output $@

$(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib/libmonosgen-2.0.dylib: $(SIM_TARGET_SHAREDMONOSGEN) | $(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib
	$(Q) lipo $(SIM_TARGET_SHAREDMONOSGEN) -create -output $@
	$(Q) install_name_tool -id @rpath/libmonosgen-2.0.dylib $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib/libmono-profiler-log.a: $(SIM_TARGET_LIBLOGPROFILER) | $(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib
	$(Q) lipo $(SIM_TARGET_LIBLOGPROFILER) -create -output $@

$(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib/libmono-profiler-log.dylib: $(SIM_TARGET_SHAREDLIBLOGPROFILER) | $(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib
	$(Q) lipo $(SIM_TARGET_SHAREDLIBLOGPROFILER) -create -output $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(IPHONESIMULATOR_DIRECTORIES):
	$(Q) mkdir -p $@

$(IOS_DESTDIR)$(IPHONESIMULATOR_SDK)/Frameworks/Mono.framework/Mono: $(IOS_DESTDIR)$(IPHONESIMULATOR_PREFIX)/lib/libmonosgen-2.0.dylib | $(IOS_DESTDIR)$(IPHONESIMULATOR_SDK)/Frameworks/Mono.framework
	$(Q) cp $< $@
	$(Q) install_name_tool -id @rpath/Mono.framework/Mono $@
	$(Q) dsymutil -t 4 -o $(patsubst %/,%,$(dir $@)).dSYM $@

$(IOS_DESTDIR)$(IPHONESIMULATOR_SDK)/Frameworks/Mono.framework/Info.plist: Mono.framework-Info.plist | $(IOS_DESTDIR)$(IPHONESIMULATOR_SDK)/Frameworks/Mono.framework
	$(Q) cp $< $@

install-iphonesimulator:: $(IPHONESIMULATOR_TARGETS)

#
# Watch simulator build
#

WATCHSIMULATOR_SDK      = $(SIMULATORWATCH_SDK)
WATCHSIMULATOR_BIN_PATH = $(XCODE_DEVELOPER_ROOT)/Platforms/WatchSimulator.platform/Developer/usr/bin
WATCHSIMULATOR_CC       = $(IOS_CC)

WATCHSIMULATOR_FLAGS =                  \
	-isysroot $(WATCHSIMULATOR_SDK)    \
	-mwatchos-simulator-version-min=$(MIN_WATCHOS_SDK_VERSION) \
	-arch i386                          \
	-O2                                 \
	-gdwarf-2                           \
	-Wl,-application_extension          \
	-DMONOTOUCH                         \
	-DHOST_IOS                          \
	-DHOST_WATCHOS                      \
	$(SIMULATOR_BUILD_CFLAGS)           \

WATCHSIMULATOR_CPPFLAGS        = $(WATCHSIMULATOR_FLAGS)
WATCHSIMULATOR_CFLAGS          = $(WATCHSIMULATOR_FLAGS)
WATCHSIMULATOR_CXXFLAGS        = $(WATCHSIMULATOR_FLAGS)
WATCHSIMULATOR_LDFLAGS         = $(COMMON_LDFLAGS)
WATCHSIMULATOR_CONFIGURE_FLAGS =                \
	--host=i386-apple-darwin10                  \
	--disable-boehm                             \
	--disable-executables                       \
	--cache-file=../watchsimulator.config.cache \
	--enable-maintainer-mode                    \
	--prefix=$(BUILD_DESTDIR)/watchsimulator    \
	--with-glib=embedded                        \
	--with-cooperative-gc=yes                   \
	--without-ikvm-native                       \
	--with-tls=pthread                          \
	--enable-minimal=com,remoting,shared_perfcounters \
	--disable-mcs-build                         \
	--disable-nls                               \
	--disable-iconv                             \
	--disable-visibility-hidden	                \
	$(XAMARIN_IOS_CONFIGURE_FLAGS)         \

# FIXME: remove --with-checked-build=yes when we've stabilized a bit.
WATCHSIMULATOR_CONFIGURE_FLAGS += \
	--enable-checked-build=gc                    \

WATCHSIMULATOR_ACVARS =  \
	mono_cv_uscore=yes \
	ac_cv_func_clock_nanosleep=no \
	ac_cv_func_system=no \
	ac_cv_func_pthread_kill=no \
	ac_cv_func_kill=no \
	ac_cv_func_sigaction=no \
	ac_cv_func_fork=no \
	ac_cv_func_execv=no \
	ac_cv_func_execve=no \
	ac_cv_func_execvp=no \
	ac_cv_func_signal=no \
	ac_cv_func_futimens=no \
	ac_cv_func_utimensat=no \

WATCHSIMULATOR_ENVIRONMENT =                  \
	$(WATCHSIMULATOR_ACVARS)                         \
	PATH="$(WATCHSIMULATOR_BIN_PATH):$(PATH)" \
	CC="$(WATCHSIMULATOR_CC)"                 \
	CXX="$(WATCHSIMULATOR_CC)"                \
	CPPFLAGS="$(WATCHSIMULATOR_CPPFLAGS)"     \
	CFLAGS="$(WATCHSIMULATOR_CFLAGS)"         \
	CXXFLAGS="$(WATCHSIMULATOR_CXXFLAGS)"     \
	LDFLAGS="$(WATCHSIMULATOR_LDFLAGS)"       \
	DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT)     \

ifdef INCLUDE_WATCH
setup:: setup-watchsimulator
build:: build-watchsimulator
clean-local:: clean-watchsimulator
install-local:: install-watchsimulator
all-local:: install-watchsimulator

watchsimulator: build-watchsimulator install-watchsimulator

.PHONY: watchsimulator
endif

setup-watchsimulator: .stamp-configure-watchsimulator

.stamp-configure-watchsimulator: $(WATCH_MONO_PATH)/configure
	$(Q) $(WATCHSIMULATOR_ENVIRONMENT) ./wrap-configure.sh watchsimulator $(abspath $(WATCH_MONO_PATH)/configure) $(WATCHSIMULATOR_CONFIGURE_FLAGS)

.stamp-build-watchsimulator: export DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT)
.stamp-build-watchsimulator: export PATH:="$(WATCHSIMULATOR_BIN_PATH):$(PATH)"
.stamp-build-watchsimulator: .stamp-configure-watchsimulator $(MONO_DEPENDENCIES)
	$(Q) $(MAKE) -C watchsimulator/mono all
	$(Q) $(MAKE) -C watchsimulator/support all
	$(Q) $(MAKE) -C watchsimulator/mono install
	$(Q) touch $@

build-watchsimulator: .stamp-build-watchsimulator

clean-watchsimulator:
	$(Q) rm -rf watchsimulator .stamp-*-watchsimulator watchsimulator.config.cache

WATCHSIMULATOR_LIBMONOSGEN          = $(BUILD_DESTDIR)/watchsimulator/lib/libmonosgen-2.0.a
WATCHSIMULATOR_SHAREDMONOSGEN       = $(BUILD_DESTDIR)/watchsimulator/lib/libmonosgen-2.0.dylib
WATCHSIMULATOR_LIBLOGPROFILER       = $(BUILD_DESTDIR)/watchsimulator/lib/libmono-profiler-log-static.a
WATCHSIMULATOR_SHAREDLIBLOGPROFILER = $(BUILD_DESTDIR)/watchsimulator/lib/libmono-profiler-log.0.dylib

$(WATCHSIMULATOR_LIBMONOSGEN): .stamp-build-watchsimulator
$(WATCHSIMULATOR_SHAREDMONOSGEN): .stamp-build-watchsimulator
$(WATCHSIMULATOR_LIBLOGPROFILER): .stamp-build-watchsimulator
$(WATCHSIMULATOR_SHAREDLIBLOGPROFILER): .stamp-build-watchsimulator

WATCHSIMULATOR_TARGETS = \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/libmonosgen-2.0.a            \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/libmonosgen-2.0.dylib        \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/libmono-profiler-log.a       \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/libmono-profiler-log.dylib   \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks/Mono.framework/Mono       \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks/Mono.framework/Info.plist \

WATCHSIMULATOR_DIRECTORIES =                                                 \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks/Mono.framework            \
	$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib                               \

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/libmonosgen-2.0.a: $(WATCHSIMULATOR_LIBMONOSGEN) | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib
	$(Q) $(WATCHOS_BIN_PATH)/lipo $(WATCHSIMULATOR_LIBMONOSGEN) -create -output $@

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/libmonosgen-2.0.dylib: $(WATCHSIMULATOR_SHAREDMONOSGEN) | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib
	$(Q) $(WATCHOS_BIN_PATH)/lipo $(WATCHSIMULATOR_SHAREDMONOSGEN) -create -output $@
	$(Q) $(WATCHOS_BIN_PATH)/install_name_tool -id @rpath/libmonosgen-2.0.dylib $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/libmono-profiler-log.a: $(WATCHSIMULATOR_LIBLOGPROFILER) | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib
	$(Q) $(WATCHOS_BIN_PATH)/lipo $(WATCHSIMULATOR_LIBLOGPROFILER) -create -output $@

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/libmono-profiler-log.dylib: $(WATCHSIMULATOR_SHAREDLIBLOGPROFILER) | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib
	$(Q) $(WATCHOS_BIN_PATH)/lipo $(WATCHSIMULATOR_SHAREDLIBLOGPROFILER) -create -output $@
	$(Q) $(WATCHOS_BIN_PATH)/install_name_tool -id @rpath/libmono-profiler-log.dylib -change $(BUILD_DESTDIR)/watchsimulator/lib/libmonosgen-2.0.1.dylib @rpath/libmonosgen-2.0.dylib $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(WATCHSIMULATOR_DIRECTORIES):
	$(Q) mkdir -p $@

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks/Mono.framework/Mono: $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/usr/lib/libmonosgen-2.0.dylib | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks/Mono.framework
	$(Q) cp $< $@
	$(Q) $(WATCHOS_BIN_PATH)/install_name_tool -id @rpath/Mono.framework/Mono $@
	$(Q) dsymutil -t 4 -o $(patsubst %/,%,$(dir $@)).dSYM $@

$(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks/Mono.framework/Info.plist: Mono.framework-watchos.Info.plist | $(IOS_DESTDIR)$(XAMARIN_WATCHSIMULATOR_SDK)/Frameworks/Mono.framework
	$(Q) cp $< $@

install-watchsimulator: $(WATCHSIMULATOR_TARGETS)

#
# TV simulator build
#

TVSIMULATOR_SDK      = $(SIMULATORTV_SDK)
TVSIMULATOR_BIN_PATH = $(XCODE_DEVELOPER_ROOT)/Platforms/AppleTVSimulator.platform/Developer/usr/bin
TVSIMULATOR_CC       = $(IOS_CC)
TVOS_BIN_PATH        = $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin

TVSIMULATOR_FLAGS =                     \
	-isysroot $(TVSIMULATOR_SDK)        \
	-mtvos-simulator-version-min=$(MIN_TVOS_SDK_VERSION)    \
	-arch x86_64                        \
	-O2                                 \
	-gdwarf-2                           \
	-Wl,-application_extension          \
	-DMONOTOUCH                         \
	-DHOST_APPLETVOS                    \
	-DTARGET_APPLETVOS                  \
	$(SIMULATOR_BUILD_CFLAGS)

TVSIMULATOR_CPPFLAGS        = $(TVSIMULATOR_FLAGS)
TVSIMULATOR_CFLAGS          = $(TVSIMULATOR_FLAGS)
TVSIMULATOR_CXXFLAGS        = $(TVSIMULATOR_FLAGS)
TVSIMULATOR_LDFLAGS         = $(COMMON_LDFLAGS)
TVSIMULATOR_CONFIGURE_FLAGS =                         \
	--host=x86_64-apple-darwin10                      \
	--disable-boehm                                   \
	--disable-executables                             \
	--cache-file=../tvsimulator.config.cache          \
	--enable-maintainer-mode                          \
	--prefix=$(BUILD_DESTDIR)/tvsimulator             \
	--with-glib=embedded                              \
	--with-cooperative-gc=no                          \
	--without-ikvm-native                             \
	--enable-minimal=com,remoting,shared_perfcounters \
	--disable-mcs-build                               \
	--disable-nls                                     \
	--disable-iconv                                   \
	--with-tls=pthread                                \
	--disable-visibility-hidden	                      \
	$(XAMARIN_IOS_CONFIGURE_FLAGS)               \

TVSIMULATOR_ACVARS =  \
	mono_cv_uscore=yes \
	ac_cv_func_clock_nanosleep=no \
	ac_cv_func_system=no \
	ac_cv_func_pthread_kill=no \
	ac_cv_func_kill=no \
	ac_cv_func_sigaction=no \
	ac_cv_func_fork=no \
	ac_cv_func_execv=no \
	ac_cv_func_execve=no \
	ac_cv_func_execvp=no \
	ac_cv_func_signal=no \
	ac_cv_func_futimens=no \
	ac_cv_func_utimensat=no \

TVSIMULATOR_ENVIRONMENT =                  \
	$(TVSIMULATOR_ACVARS)                  \
	PATH="$(TVSIMULATOR_BIN_PATH):$(PATH)" \
	CC="$(TVSIMULATOR_CC)"                 \
	CXX="$(TVSIMULATOR_CC)"                \
	CPPFLAGS="$(TVSIMULATOR_CPPFLAGS)"     \
	CFLAGS="$(TVSIMULATOR_CFLAGS)"         \
	CXXFLAGS="$(TVSIMULATOR_CXXFLAGS)"     \
	LDFLAGS="$(TVSIMULATOR_LDFLAGS)"       \
	DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT)  \

ifdef INCLUDE_TVOS
setup:: setup-tvsimulator
build:: build-tvsimulator
clean-local:: clean-tvsimulator
install-local:: install-tvsimulator
all-local:: install-tvsimulator

tvsimulator: build-tvsimulator install-tvsimulator

.PHONY: tvsimulator
endif

setup-tvsimulator: .stamp-configure-tvsimulator

# for now we re-use the watch mono
.stamp-configure-tvsimulator: $(MONO_PATH)/configure
	$(Q) $(TVSIMULATOR_ENVIRONMENT) ./wrap-configure.sh tvsimulator $(abspath $(MONO_PATH)/configure) $(TVSIMULATOR_CONFIGURE_FLAGS)

.stamp-build-tvsimulator: export DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT)
.stamp-build-tvsimulator: export PATH:="$(TVSIMULATOR_BIN_PATH):$(PATH)"
.stamp-build-tvsimulator: .stamp-configure-tvsimulator $(MONO_DEPENDENCIES)
	$(Q) $(MAKE) -C tvsimulator/mono all
	$(Q) $(MAKE) -C tvsimulator/support all
	$(Q) $(MAKE) -C tvsimulator/mono install
	$(Q) touch $@

build-tvsimulator: .stamp-build-tvsimulator

clean-tvsimulator:
	$(Q) rm -rf tvsimulator .stamp-*-tvsimulator tvsimulator.config.cache

TVSIMULATOR_LIBMONOSGEN          = $(BUILD_DESTDIR)/tvsimulator/lib/libmonosgen-2.0.a
TVSIMULATOR_SHAREDMONOSGEN       = $(BUILD_DESTDIR)/tvsimulator/lib/libmonosgen-2.0.dylib
TVSIMULATOR_LIBLOGPROFILER       = $(BUILD_DESTDIR)/tvsimulator/lib/libmono-profiler-log-static.a
TVSIMULATOR_SHAREDLIBLOGPROFILER = $(BUILD_DESTDIR)/tvsimulator/lib/libmono-profiler-log.0.dylib

$(TVSIMULATOR_LIBMONOSGEN): .stamp-build-tvsimulator
$(TVSIMULATOR_SHAREDMONOSGEN): .stamp-build-tvsimulator
$(TVSIMULATOR_LIBLOGPROFILER): .stamp-build-tvsimulator
$(TVSIMULATOR_SHAREDLIBLOGPROFILER): .stamp-build-tvsimulator

TVSIMULATOR_TARGETS = \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/libmonosgen-2.0.a            \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/libmonosgen-2.0.dylib        \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/libmono-profiler-log.a       \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/libmono-profiler-log.dylib   \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks/Mono.framework/Mono       \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks/Mono.framework/Info.plist \

TVSIMULATOR_DIRECTORIES =                                                 \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks/Mono.framework            \
	$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib                               \

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/libmonosgen-2.0.a: $(TVSIMULATOR_LIBMONOSGEN) | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib
	$(Q) $(TVOS_BIN_PATH)/lipo $(TVSIMULATOR_LIBMONOSGEN) -create -output $@

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/libmonosgen-2.0.dylib: $(TVSIMULATOR_SHAREDMONOSGEN) | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib
	$(Q) $(TVOS_BIN_PATH)/lipo $(TVSIMULATOR_SHAREDMONOSGEN) -create -output $@
	$(Q) $(TVOS_BIN_PATH)/install_name_tool -id @rpath/libmonosgen-2.0.dylib $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/libmono-profiler-log.a: $(TVSIMULATOR_LIBLOGPROFILER) | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib
	$(Q) $(TVOS_BIN_PATH)/lipo $(TVSIMULATOR_LIBLOGPROFILER) -create -output $@

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/libmono-profiler-log.dylib: $(TVSIMULATOR_SHAREDLIBLOGPROFILER) | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib
	$(Q) $(TVOS_BIN_PATH)/lipo $(TVSIMULATOR_SHAREDLIBLOGPROFILER) -create -output $@
	$(Q) $(TVOS_BIN_PATH)/install_name_tool -id @rpath/libmono-profiler-log.dylib -change $(BUILD_DESTDIR)/tvsimulator/lib/libmonosgen-2.0.1.dylib @rpath/libmonosgen-2.0.dylib $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(TVSIMULATOR_DIRECTORIES):
	$(Q) mkdir -p $@

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks/Mono.framework/Mono: $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/usr/lib/libmonosgen-2.0.dylib | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks/Mono.framework
	$(Q) cp $< $@
	$(Q) $(TVOS_BIN_PATH)/install_name_tool -id @rpath/Mono.framework/Mono $@
	$(Q) dsymutil -t 4 -o $(patsubst %/,%,$(dir $@)).dSYM $@

$(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks/Mono.framework/Info.plist: Mono.framework-tvos.Info.plist | $(IOS_DESTDIR)$(XAMARIN_TVSIMULATOR_SDK)/Frameworks/Mono.framework
	$(Q) cp $< $@

install-tvsimulator: $(TVSIMULATOR_TARGETS)

#
# Device builds
#

#
# We use --build=i386-apple-darwin10 explicitly here so that even though osx defaults to 64 bit,
# we end up with a sizeof-compatible built (since all current arms are 32b)
#

PLATFORM_BIN=$(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin
PLATFORM_SDK=$(DEVICE_SDK)

# usage $(call PlatformBuildTemplate (armv7,target7,<platform suffix>,<configure flags>,<unused>))

define PlatformBuildTemplate

# $(4) contains the architecture specific configure flags
# $(5) is unused
# $(6) contains min ios version

# Avoid passing the -arch flag twice in CFLAGS since it disables ccache
$(eval $(2)_BASE_CPPFLAGS=-DSMALL_CONFIG -DDISABLE_POLICY_EVIDENCE=1 -DDISABLE_PROCESS_HANDLING=1 -D_XOPEN_SOURCE -DMONOTOUCH=1 -DHOST_IOS -DHAVE_LARGE_FILE_SUPPORT=1 -Wl,-application_extension -miphoneos-version-min=$(6) -isysroot $(PLATFORM_SDK))
$(eval $(2)_CPPFLAGS=$($(2)_BASE_CPPFLAGS) -arch $(1))
$(eval $(2)_CFLAGS=-O2 -gdwarf-2 $($(2)_BASE_CPPFLAGS) $(DEVICE_BUILD_CFLAGS) $(IOS_BITCODE_CFLAGS))
$(eval $(2)_CXXFLAGS=$($(2)_CPPFLAGS) $(IOS_BITCODE_CXXFLAGS))
$(eval $(2)_LDFLAGS=-arch $(1) $(IOS_BITCODE_LDFLAGS) $(COMMON_LDFLAGS))
$(eval $(2)_CONFIGURE_FLAGS=--build=i386-apple-darwin10 \
			$(4) \
			--enable-maintainer-mode	\
			--cache-file=../$(2).config.cache \
			--prefix=$(BUILD_DESTDIR)/$(2) \
			--with-monotouch \
			--with-lazy-gc-thread-creation=yes \
			--disable-mcs-build \
			--enable-minimal=ssa,com,interpreter,jit,reflection_emit_save,reflection_emit,portability,assembly_remapping,attach,verifier,full_messages,appdomains,security,sgen_remset,sgen_marksweep_par,sgen_marksweep_fixed,sgen_marksweep_fixed_par,sgen_copying,logging,remoting,shared_perfcounters \
			--without-ikvm-native \
			--with-tls=pthread \
			--without-sigaltstack \
			--enable-icall-export \
			--disable-icall-tables \
			--disable-executables \
			--disable-nls \
			--disable-iconv \
			--disable-visibility-hidden	\
			--disable-boehm \
			--enable-dtrace=no \
			$(IOS_BITCODE_CONFIGURE_FLAGS) \
			$(XAMARIN_IOS_CONFIGURE_FLAGS) \
			)

$(eval $(2)_ACVARS= mono_cv_uscore=yes \
		mono_cv_sizeof_sunpath=104 \
		ac_cv_func_posix_getpwuid_r=yes \
		ac_cv_func_finite=no \
		ac_cv_c_bigendian=no \
		ac_cv_header_sys_user_h=no \
		ac_cv_header_curses_h=no \
		ac_cv_header_localcharset_h=no \
		ac_cv_func_getpwuid_r=no)

$(eval $(2)_CONFIGURE_ENVIRONMENT = \
	PATH="$(PLATFORM$(3)_BIN):$(PATH)" \
	CPPFLAGS="$($(2)_CPPFLAGS)" \
	CFLAGS="$($(2)_CFLAGS)" \
	CXXFLAGS="$($(2)_CXXFLAGS)" \
	LDFLAGS="$($(2)_LDFLAGS)" \
	CC="$(DEVICE_CC)" \
	CXX="$(DEVICE_CXX)")

.PHONY: $(2)

$(2): build-$(2)

setup:: setup-$(2)
build:: build-$(2)
clean-local:: clean-$(2)

setup-$(2): .stamp-configure-$(2)

.stamp-configure-$(2): $(MONO_PATH)/configure
	$(Q) $($(2)_ACVARS) $($(2)_CONFIGURE_ENVIRONMENT) ./wrap-configure.sh $(2) $(abspath $(MONO_PATH)/configure) $($(2)_CONFIGURE_FLAGS)

.stamp-build-$(2): .stamp-configure-$(2) $(MONO_DEPENDENCIES)
	PATH="$(PLATFORM_BIN):$(PATH)" $(MAKE) -C $(2)/mono
	PATH="$(PLATFORM_BIN):$(PATH)" $(MAKE) -C $(2)/mono install
	$(Q) touch $$@

build-$(2): .stamp-build-$(2)

clean-$(2):
	-rm -rf $(2) .stamp-*-$(2) $(2).config.cache

setup-iphoneos:: setup-$(2)
build-iphoneos:: build-$(2)
clean-iphoneos:: clean-$(2)

build-ios: build-iphoneos
clean-ios: clean-iphoneos
setup-ios: setup-iphoneos
install-ios: install-iphoneos

$(eval TARGET_LIBMONOSGEN:=$(TARGET_LIBMONOSGEN) $(BUILD_DESTDIR)/$(2)/lib/libmonosgen-2.0.a)
$(eval TARGET_SHAREDMONOSGEN:=$(TARGET_SHAREDMONOSGEN) $(BUILD_DESTDIR)/$(2)/tmp-lib/libmonosgen-2.0.dylib)
$(eval TARGET_LIBLOGPROFILER:=$(TARGET_LIBLOGPROFILER) $(BUILD_DESTDIR)/$(2)/lib/libmono-profiler-log-static.a)
$(eval TARGET_SHAREDLIBLOGPROFILER:=$(TARGET_SHAREDLIBLOGPROFILER) $(BUILD_DESTDIR)/$(2)/tmp-lib/libmono-profiler-log.0.dylib)
$(eval TARGET_MONOFRAMEWORK:=$(TARGET_MONOFRAMEWORK) $(BUILD_DESTDIR)/$(2)/tmp-lib/Mono)

$(BUILD_DESTDIR)/$(2)/lib/libmonosgen-2.0.a: .stamp-build-$(2)
$(BUILD_DESTDIR)/$(2)/lib/libmono-profiler-log-static.a: .stamp-build-$(2)
$(BUILD_DESTDIR)/$(2)/lib/libmono-profiler-log.a: .stamp-build-$(2)

$(BUILD_DESTDIR)/$(2)/tmp-lib:
	$$(Q) mkdir -p $$@

# We have the following requirements:
#
# * libmonos*.a: must have miphone-version-min=5.1.1 (the earliest iOS version we support) - although it's 7.0 for the ARM64 slice.
# * libmonosgen-2.0.dylib: must have miphone-version-min=7.0 (otherwise iOS 9 won't load it; see bug #34267).
# * libmono-profiler-log.dylib: same as libmonosgen-2.0.dylib
# * Mono.framework/Mono: must have miphone-version-min=8.0, otherwise the native linker won't add a LC_ENCRYPTION_INFO load command,
#   which the App Store requires (see bug #32820). This is not a problem for libmonosgen-2.0.dylib, because that library is only
#   used for incremental builds, which are not published).
#
# And we don't want to build Mono 3 times for each architecture...
#
# So what we do is to take the static library (libmonosgen-2.0.a), extract all the object files, and re-link
# them the required times according to how many versions we need.
#
$(BUILD_DESTDIR)/$(2)/tmp-lib/Mono: $(BUILD_DESTDIR)/$(2)/lib/libmonosgen-2.0.a | $(BUILD_DESTDIR)/$(2)/tmp-lib
	$$(Q_GEN) CC="$(IOS_CC)" ./create-shared-library.sh $$< $$@ -arch $(1) -install_name @rpath/Mono.framework/Mono -miphoneos-version-min=8.0 -isysroot $(PLATFORM_SDK) $(IOS_BITCODE_LDFLAGS)

$(BUILD_DESTDIR)/$(2)/tmp-lib/libmonosgen-2.0.dylib: $(BUILD_DESTDIR)/$(2)/lib/libmonosgen-2.0.a | $(BUILD_DESTDIR)/$(2)/tmp-lib
	$$(Q_GEN) CC="$(IOS_CC)" ./create-shared-library.sh $$< $$@ -arch $(1) -install_name @rpath/libmonosgen-2.0.dylib -miphoneos-version-min=7.0 -isysroot $(PLATFORM_SDK) $(IOS_BITCODE_LDFLAGS)

$(BUILD_DESTDIR)/$(2)/tmp-lib/libmono-profiler-log.0.dylib: $(BUILD_DESTDIR)/$(2)/lib/libmono-profiler-log.a $(BUILD_DESTDIR)/$(2)/tmp-lib/libmonosgen-2.0.dylib | $(BUILD_DESTDIR)/$(2)/tmp-lib
	$$(Q_GEN) CC="$(IOS_CC)" ./create-shared-library.sh $$< $$@ -arch $(1) -install_name @rpath/libmono-profiler-log.dylib -miphoneos-version-min=7.0 -isysroot $(PLATFORM_SDK) $(IOS_BITCODE_LDFLAGS) -L$(BUILD_DESTDIR)/$(2)/tmp-lib -lmonosgen-2.0 -lz

endef

ARM_ARCH_CONFIGURE_FLAGS=--host=arm-apple-darwin10 --disable-boehm
ARM64_ARCH_CONFIGURE_FLAGS=--host=aarch64-apple-darwin10 --disable-boehm

ifdef INCLUDE_IOS
ifdef INCLUDE_DEVICE
ifndef DEVICE_RUNTIMES
DEVICE_RUNTIMES="armv7 armv7s arm64 "
endif
endif
endif

# Note the spaces in findstring arguments, this is needed to distinguish between armv7 and armv7s for example
ifneq ($(findstring armv7 , $(DEVICE_RUNTIMES) ),)
$(eval $(call PlatformBuildTemplate,armv7,target7,,$(ARM_ARCH_CONFIGURE_FLAGS),,$(MIN_IOS_SDK_VERSION)))
endif

ifneq ($(findstring armv7s , $(DEVICE_RUNTIMES) ),)
$(eval $(call PlatformBuildTemplate,armv7s,target7s,,$(ARM_ARCH_CONFIGURE_FLAGS),,$(MIN_IOS_SDK_VERSION)))
endif

ifneq ($(findstring arm64 , $(DEVICE_RUNTIMES) ),)
$(eval $(call PlatformBuildTemplate,arm64,target64,,$(ARM64_ARCH_CONFIGURE_FLAGS),,7.0))
endif

ifdef INCLUDE_DEVICE
iphoneos:: setup-iphoneos build-iphoneos install-iphoneos
device:: iphoneos
clean-device: clean-iphoneos
install-device: install-iphoneos

# this needs to be here because the normal usage of this makefile is "make all install", but nothing actually lists the -iphoneos targets as dependencies.
ifdef INCLUDE_IOS
install-local:: install-iphoneos
all-local:: install-iphoneos
endif
endif

LIBMONO_PROFILER_IPHONEOS_DYLIB=$(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib/libmono-profiler-log.dylib

IPHONEOS_TARGETS = \
	$(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib/libmonosgen-2.0.a                                  \
	$(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib/libmonosgen-2.0.dylib                              \
	$(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib/libmono-profiler-log.a                             \
	$(LIBMONO_PROFILER_IPHONEOS_DYLIB)														\
	$(IOS_DESTDIR)$(IPHONEOS_SDK)/Frameworks/Mono.framework/Mono                            \
	$(IOS_DESTDIR)$(IPHONEOS_SDK)/Frameworks/Mono.framework/Info.plist                      \

IPHONEOS_DIRECTORIES = \
	$(IOS_DESTDIR)$(IPHONEOS_SDK)/Frameworks/Mono.framework                      \
	$(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib                                         \

$(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib/libmonosgen-2.0.a: $(TARGET_LIBMONOSGEN) | $(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib
	$(Q) lipo $(TARGET_LIBMONOSGEN) -create -output $@

$(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib/libmonosgen-2.0.dylib: $(TARGET_SHAREDMONOSGEN) | $(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib
	$(Q) lipo $(TARGET_SHAREDMONOSGEN) -create -output $@
	$(Q) install_name_tool -id @rpath/libmonosgen-2.0.dylib $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib/libmono-profiler-log.a: $(TARGET_LIBLOGPROFILER) | $(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib
	$(Q) lipo $(TARGET_LIBLOGPROFILER) -create -output $@

$(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib/libmono-profiler-log.dylib: $(TARGET_SHAREDLIBLOGPROFILER) | $(IOS_DESTDIR)$(IPHONEOS_PREFIX)/lib
	$(Q) lipo $(TARGET_SHAREDLIBLOGPROFILER) -create -output $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(IPHONEOS_DIRECTORIES):
	$(Q) mkdir -p $@

$(IOS_DESTDIR)$(IPHONEOS_SDK)/Frameworks/Mono.framework/Mono: $(TARGET_MONOFRAMEWORK) | $(IOS_DESTDIR)$(IPHONEOS_SDK)/Frameworks/Mono.framework
	$(Q) lipo $(TARGET_MONOFRAMEWORK) -create -output $@
	$(Q) dsymutil -t 4 -o $(patsubst %/,%,$(dir $@)).dSYM $@

$(IOS_DESTDIR)$(IPHONEOS_SDK)/Frameworks/Mono.framework/Info.plist: Mono.framework-Info.plist | $(IOS_DESTDIR)$(IPHONEOS_SDK)/Frameworks/Mono.framework
	$(Q) cp $< $@

install-iphoneos:: $(IPHONEOS_TARGETS)

#
# Watch device build
# 

WATCHOS_SDK      = $(DEVICEWATCH_SDK)
WATCHOS_BIN_PATH = $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin
WATCHOS_CC       = $(IOS_CC)

WATCHOS_FLAGS =                  \
	-isysroot $(WATCHOS_SDK)     \
	-mwatchos-version-min=$(MIN_WATCHOS_SDK_VERSION)    \
	-arch armv7k                 \
	-O2                          \
	-gdwarf-2                    \
	-Wl,-application_extension   \
	-fembed-bitcode              \
	-fno-gnu-inline-asm          \
	-DMONOTOUCH=1                \
	-DHOST_IOS                   \
	-DHOST_WATCHOS               \
	-DSMALL_CONFIG               \
	-DDISABLE_POLICY_EVIDENCE=1  \
	-DDISABLE_PROCESS_HANDLING=1 \
	-D_XOPEN_SOURCE              \
	-DHAVE_LARGE_FILE_SUPPORT=1  \
	$(DEVICE_BUILD_CFLAGS)       \
	$(BITCODE_CFLAGS)

WATCHOS_CPPFLAGS        = $(WATCHOS_FLAGS)
WATCHOS_CFLAGS          = $(WATCHOS_FLAGS)
WATCHOS_CXXFLAGS        = $(WATCHOS_FLAGS)
WATCHOS_LDFLAGS         = -arch armv7k -Wl,-bitcode_bundle -framework CoreFoundation -lobjc $(BITCODE_LDFLAGS) $(COMMON_LDFLAGS)
WATCHOS_CONFIGURE_FLAGS =                        \
	--build=i386-apple-darwin10                  \
	--host=armv7k-apple-darwin10                 \
	--enable-maintainer-mode                     \
	--cache-file=../targetwatch.config.cache     \
	--prefix=$(BUILD_DESTDIR)/targetwatch        \
	--with-cooperative-gc=yes                    \
	--with-monotouch                             \
	--with-lazy-gc-thread-creation=yes           \
	--disable-mcs-build                          \
	--disable-boehm                              \
	--enable-minimal=ssa,com,interpreter,jit,reflection_emit_save,reflection_emit,portability,assembly_remapping,attach,verifier,full_messages,appdomains,security,sgen_remset,sgen_marksweep_par,sgen_marksweep_fixed,sgen_marksweep_fixed_par,sgen_copying,logging,remoting,shared_perfcounters \
	--without-ikvm-native                        \
	--with-tls=pthread                           \
	--without-sigaltstack                        \
	--enable-icall-export                        \
	--disable-icall-tables                       \
	--disable-executables                        \
	--disable-nls                                \
	--disable-iconv                              \
	--disable-visibility-hidden	                 \
	$(XAMARIN_IOS_CONFIGURE_FLAGS)          \
	$(BITCODE_CONFIGURE_FLAGS)

WATCHOS_ACVARS =                    \
	mono_cv_uscore=yes              \
	mono_cv_sizeof_sunpath=104      \
	ac_cv_func_posix_getpwuid_r=yes \
	ac_cv_func_finite=no            \
	ac_cv_c_bigendian=no            \
	ac_cv_header_sys_user_h=no      \
	ac_cv_header_curses_h=no        \
	ac_cv_header_localcharset_h=no  \
	ac_cv_func_getpwuid_r=no        \
	ac_cv_func_system=no			\
	ac_cv_func_pthread_kill=no      \
	ac_cv_func_kill=no              \
	ac_cv_func_sigaction=no         \
	ac_cv_func_fork=no              \
	ac_cv_func_execv=no             \
	ac_cv_func_execve=no            \
	ac_cv_func_execvp=no            \
	ac_cv_func_signal=no

WATCHOS_ENVIRONMENT =                     \
	$(WATCHOS_ACVARS)                     \
	PATH="$(WATCHOS_BIN_PATH):$(PATH)"    \
	CFLAGS="$(WATCHOS_CFLAGS)"            \
	CPPFLAGS="$(WATCHOS_CPPFLAGS)"        \
	CXXFLAGS="$(WATCHOS_CXXFLAGS)"        \
	LDFLAGS="$(WATCHOS_LDFLAGS)"          \
	CC="$(WATCHOS_CC)"                    \
	CXX="$(WATCHOS_CC)"                   \
	DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT) \

ifdef INCLUDE_WATCH
ifdef INCLUDE_DEVICE
setup:: setup-targetwatch
build:: build-targetwatch
clean-local:: clean-targetwatch
install-local:: install-watchos
all-local:: install-watchos

targetwatch: build-targetwatch install-watchos
watchos:: targetwatch

.PHONY: targetwatch
endif
endif

setup-targetwatch: .stamp-configure-targetwatch

.stamp-configure-targetwatch: $(WATCH_MONO_PATH)/configure
	$(Q) $(WATCHOS_ENVIRONMENT) ./wrap-configure.sh targetwatch $(abspath $(WATCH_MONO_PATH)/configure) $(WATCHOS_CONFIGURE_FLAGS)

.stamp-build-targetwatch: export DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT)
.stamp-build-targetwatch: export PATH:="$(WATCHOS_BIN_PATH):$(PATH)"
.stamp-build-targetwatch: .stamp-configure-targetwatch $(MONO_DEPENDENCIES)
	$(Q) $(MAKE) -C targetwatch/mono
	$(Q) $(MAKE) -C targetwatch/mono install
	$(Q) touch $@

build-targetwatch: .stamp-build-targetwatch

clean-targetwatch:
	$(Q) rm -rf targetwatch .stamp-*-targetwatch targetwatch.config.cache

setup-watchos: setup-targetwatch
build-watchos: build-targetwatch
clean-watchos: clean-targetwatch

WATCHOS_TARGET_LIBMONOSGEN          = $(BUILD_DESTDIR)/targetwatch/lib/libmonosgen-2.0.a
WATCHOS_TARGET_SHAREDMONOSGEN       = $(BUILD_DESTDIR)/targetwatch/lib/libmonosgen-2.0.dylib
WATCHOS_TARGET_LIBLOGPROFILER       = $(BUILD_DESTDIR)/targetwatch/lib/libmono-profiler-log-static.a
WATCHOS_TARGET_SHAREDLIBLOGPROFILER = $(BUILD_DESTDIR)/targetwatch/lib/libmono-profiler-log.0.dylib

$(WATCHOS_TARGET_LIBMONOSGEN): .stamp-build-targetwatch
$(WATCHOS_TARGET_SHAREDMONOSGEN): .stamp-build-targetwatch
$(WATCHOS_TARGET_LIBLOGPROFILER): .stamp-build-targetwatch
$(WATCHOS_TARGET_SHAREDLIBLOGPROFILER): .stamp-build-targetwatch

device:: watchos

WATCHOS_TARGETS =                                                             \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib/libmonosgen-2.0.a            \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib/libmonosgen-2.0.dylib        \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib/libmono-profiler-log.a       \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks/Mono.framework/Mono       \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks/Mono.framework/Info.plist \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib/libmono-profiler-log.dylib   \

WATCHOS_DIRECTORIES = \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks/Mono.framework \
	$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib                   \

$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib/libmonosgen-2.0.a: $(WATCHOS_TARGET_LIBMONOSGEN) | $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib
	$(Q) $(WATCHOS_BIN_PATH)/lipo $(WATCHOS_TARGET_LIBMONOSGEN) -create -output $@

$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib/libmonosgen-2.0.dylib: $(WATCHOS_TARGET_SHAREDMONOSGEN) | $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib
	$(Q_STRIP) $(WATCHOS_BIN_PATH)/bitcode_strip $(WATCHOS_TARGET_SHAREDMONOSGEN) -m -o $@
	$(Q) $(WATCHOS_BIN_PATH)/install_name_tool -id @rpath/libmonosgen-2.0.dylib $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib/libmono-profiler-log.a: $(WATCHOS_TARGET_LIBLOGPROFILER) | $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib
	$(Q) $(WATCHOS_BIN_PATH)/lipo $(WATCHOS_TARGET_LIBLOGPROFILER) -create -output $@

$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib/libmono-profiler-log.dylib: $(WATCHOS_TARGET_SHAREDLIBLOGPROFILER) | $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/usr/lib
	$(Q) $(WATCHOS_BIN_PATH)/bitcode_strip $(WATCHOS_TARGET_SHAREDLIBLOGPROFILER) -m -o $@
	$(Q) $(WATCHOS_BIN_PATH)/install_name_tool -id @rpath/libmono-profiler-log.dylib -change $(BUILD_DESTDIR)/targetwatch/lib/libmonosgen-2.0.1.dylib @rpath/libmonosgen-2.0.dylib $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(WATCHOS_DIRECTORIES):
	$(Q) mkdir -p $@

$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks/Mono.framework/Mono: $(BUILD_DESTDIR)/targetwatch/tmp-lib/Mono | $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks/Mono.framework
	$(Q) cp $< $@
	$(Q) dsymutil -t 4 -o $(patsubst %/,%,$(dir $@)).dSYM $@

$(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks/Mono.framework/Info.plist: Mono.framework-watchos.Info.plist | $(IOS_DESTDIR)$(XAMARIN_WATCHOS_SDK)/Frameworks/Mono.framework
	$(Q) cp $< $@

$(BUILD_DESTDIR)/targetwatch/tmp-lib/Mono: $(BUILD_DESTDIR)/targetwatch/lib/libmonosgen-2.0.a
	$(Q_GEN) CC="$(IOS_CC)" ./create-shared-library.sh $< $@ -arch armv7k -install_name @rpath/Mono.framework/Mono -mwatchos-version-min=$(MIN_WATCHOS_SDK_VERSION) -isysroot $(DEVICEWATCH_SDK) -lc++ -fembed-bitcode

install-watchos: $(WATCHOS_TARGETS)

#
# TV device build
# 

TVOS_SDK      = $(DEVICETV_SDK)
TVOS_BIN_PATH = $(XCODE_DEVELOPER_ROOT)/Toolchains/XcodeDefault.xctoolchain/usr/bin
TVOS_CC       = $(IOS_CC)

TVOS_FLAGS =                     \
	-isysroot $(TVOS_SDK)        \
	-mtvos-version-min=$(MIN_TVOS_SDK_VERSION)       \
	-arch arm64                  \
	-O2                          \
	-gdwarf-2                    \
	-Wl,-application_extension   \
	-fembed-bitcode       \
	-fno-gnu-inline-asm          \
	-DMONOTOUCH=1                \
	-DHOST_APPLETVOS             \
	-DTARGET_APPLETVOS           \
	-DSMALL_CONFIG               \
	-DDISABLE_PROCESS_HANDLING=1 \
	-DDISABLE_POLICY_EVIDENCE=1  \
	-D_XOPEN_SOURCE              \
	-DHAVE_LARGE_FILE_SUPPORT=1  \
	$(BITCODE_CFLAGS)			 \
	$(DEVICE_BUILD_CFLAGS)       \

TVOS_CPPFLAGS        = $(TVOS_FLAGS)
TVOS_CFLAGS          = $(TVOS_FLAGS) $(DEVICE_BUILD_CFLAGS)
TVOS_CXXFLAGS        = $(TVOS_FLAGS)
TVOS_LDFLAGS         = -arch arm64 -Wl,-bitcode_bundle -framework CoreFoundation -lobjc $(BITCODE_LDFLAGS) $(COMMON_LDFLAGS)
TVOS_CONFIGURE_FLAGS =                           \
	--build=x86_64-apple-darwin10                \
	--host=aarch64-apple-darwin10                  \
	--enable-maintainer-mode                     \
	--cache-file=../targettv.config.cache        \
	--prefix=$(BUILD_DESTDIR)/targettv           \
	--with-cooperative-gc=no                     \
	--with-monotouch                             \
	--with-lazy-gc-thread-creation=yes           \
	--disable-mcs-build                          \
	--disable-boehm                              \
	--enable-minimal=ssa,com,interpreter,jit,reflection_emit_save,reflection_emit,portability,assembly_remapping,attach,verifier,full_messages,appdomains,security,sgen_remset,sgen_marksweep_par,sgen_marksweep_fixed,sgen_marksweep_fixed_par,sgen_copying,logging,remoting,shared_perfcounters \
	--without-ikvm-native                        \
	--with-tls=pthread                           \
	--without-sigaltstack                        \
	--enable-icall-export                        \
	--disable-icall-tables						 \
	--disable-executables                        \
	--disable-nls                                \
	--disable-iconv                              \
	--disable-visibility-hidden	                 \
	--enable-extension-module                    \
	--enable-dtrace=no                           \
	$(XAMARIN_IOS_CONFIGURE_FLAGS)               \
	$(BITCODE_CONFIGURE_FLAGS)					 \

# explicitly disable dtrace, since it requires inline assembly, which is disabled on AppleTV (and mono's configure.ac doesn't know that (yet at least))

TVOS_ACVARS =                       \
	mono_cv_uscore=yes              \
	mono_cv_sizeof_sunpath=104      \
	ac_cv_func_posix_getpwuid_r=yes \
	ac_cv_func_finite=no            \
	ac_cv_c_bigendian=no            \
	ac_cv_header_sys_user_h=no      \
	ac_cv_header_curses_h=no        \
	ac_cv_header_localcharset_h=no  \
	ac_cv_func_getpwuid_r=no        \
	ac_cv_func_system=no			\
	ac_cv_func_pthread_kill=no      \
	ac_cv_func_kill=no              \
	ac_cv_func_sigaction=no         \
	ac_cv_func_fork=no              \
	ac_cv_func_execv=no             \
	ac_cv_func_execve=no            \
	ac_cv_func_execvp=no            \
	ac_cv_func_signal=no

TVOS_ENVIRONMENT =                        \
	$(TVOS_ACVARS)                     \
	PATH="$(TVOS_BIN_PATH):$(PATH)"    \
	CFLAGS="$(TVOS_CFLAGS)"            \
	CPPFLAGS="$(TVOS_CPPFLAGS)"        \
	CXXFLAGS="$(TVOS_CXXFLAGS)"        \
	LDFLAGS="$(TVOS_LDFLAGS)"          \
	CC="$(TVOS_CC)"                    \
	CXX="$(TVOS_CC)"                   \
	DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT) \

ifdef INCLUDE_TVOS
ifdef INCLUDE_DEVICE
setup:: setup-targettv
build:: build-targettv
clean-local:: clean-targettv
install-local:: install-tvos
all-local:: install-tvos

targettv: build-targettv install-tvos
tvos:: targettv

.PHONY: targettv
endif
endif

setup-targettv: .stamp-configure-targettv

.stamp-configure-targettv: $(MONO_PATH)/configure
	$(Q) $(TVOS_ENVIRONMENT) ./wrap-configure.sh targettv $(abspath $(MONO_PATH)/configure) $(TVOS_CONFIGURE_FLAGS)

.stamp-build-targettv: export DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT)
.stamp-build-targettv: export PATH:="$(TVOS_BIN_PATH):$(PATH)"
.stamp-build-targettv: .stamp-configure-targettv $(MONO_DEPENDENCIES)
	$(Q) $(MAKE) -C targettv/mono
	$(Q) $(MAKE) -C targettv/mono install
	$(Q) touch $@

build-targettv: .stamp-build-targettv

clean-targettv:
	$(Q) rm -rf targettv .stamp-*-targettv targettv.config.cache

setup-tvos: setup-targettv
build-tvos: build-targettv
clean-tvos: clean-targettv

TVOS_TARGET_LIBMONOSGEN          = $(BUILD_DESTDIR)/targettv/lib/libmonosgen-2.0.a
TVOS_TARGET_SHAREDMONOSGEN       = $(BUILD_DESTDIR)/targettv/lib/libmonosgen-2.0.dylib
TVOS_TARGET_LIBLOGPROFILER       = $(BUILD_DESTDIR)/targettv/lib/libmono-profiler-log-static.a
TVOS_TARGET_SHAREDLIBLOGPROFILER = $(BUILD_DESTDIR)/targettv/lib/libmono-profiler-log.0.dylib

$(TVOS_TARGET_LIBMONOSGEN): .stamp-build-targettv
$(TVOS_TARGET_SHAREDMONOSGEN): .stamp-build-targettv
$(TVOS_TARGET_LIBLOGPROFILER): .stamp-build-targettv
$(TVOS_TARGET_SHAREDLIBLOGPROFILER): .stamp-build-targettv

device:: tvos

TVOS_TARGETS =                                                             \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib/libmonosgen-2.0.a            \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib/libmonosgen-2.0.dylib        \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib/libmono-profiler-log.a       \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks/Mono.framework/Mono       \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks/Mono.framework/Info.plist \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib/libmono-profiler-log.dylib   \

TVOS_DIRECTORIES = \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks/Mono.framework \
	$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib                   \

$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib/libmonosgen-2.0.a: $(TVOS_TARGET_LIBMONOSGEN) | $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib
	$(Q) $(TVOS_BIN_PATH)/lipo $(TVOS_TARGET_LIBMONOSGEN) -create -output $@

$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib/libmonosgen-2.0.dylib: $(TVOS_TARGET_SHAREDMONOSGEN) | $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib
	$(Q) $(TVOS_BIN_PATH)/lipo $(TVOS_TARGET_SHAREDMONOSGEN) -create -output $@
	$(Q) $(TVOS_BIN_PATH)/install_name_tool -id @rpath/libmonosgen-2.0.dylib $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib/libmono-profiler-log.a: $(TVOS_TARGET_LIBLOGPROFILER) | $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib
	$(Q) $(TVOS_BIN_PATH)/lipo $(TVOS_TARGET_LIBLOGPROFILER) -create -output $@

$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib/libmono-profiler-log.dylib: $(TVOS_TARGET_SHAREDLIBLOGPROFILER) | $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/usr/lib
	$(Q) lipo $(TVOS_TARGET_SHAREDLIBLOGPROFILER) -create -output $@
	$(Q) $(TVOS_BIN_PATH)/install_name_tool -id @rpath/libmono-profiler-log.dylib -change $(BUILD_DESTDIR)/targettv/lib/libmonosgen-2.0.1.dylib @rpath/libmonosgen-2.0.dylib $@
	$(Q) dsymutil -t 4 -o $@.dSYM $@

$(TVOS_DIRECTORIES):
	$(Q) mkdir -p $@

$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks/Mono.framework/Mono: $(BUILD_DESTDIR)/targettv/tmp-lib/Mono | $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks/Mono.framework
	$(Q) cp $< $@
	$(Q) dsymutil -t 4 -o $(patsubst %/,%,$(dir $@)).dSYM $@

$(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks/Mono.framework/Info.plist: Mono.framework-tvos.Info.plist | $(IOS_DESTDIR)$(XAMARIN_TVOS_SDK)/Frameworks/Mono.framework
	$(Q) cp $< $@

$(BUILD_DESTDIR)/targettv/tmp-lib/Mono: $(BUILD_DESTDIR)/targettv/lib/libmonosgen-2.0.a
	$(Q_GEN) CC="$(IOS_CC)" ./create-shared-library.sh $< $@ -arch arm64 -install_name @rpath/Mono.framework/Mono -mtvos-version-min=$(MIN_TVOS_SDK_VERSION) -isysroot $(DEVICETV_SDK) -lc++ -fembed-bitcode

install-tvos: $(TVOS_TARGETS)


#
# this is the cross aot version of mono.  runs on x86 but the jit/aot spits out arm.
#
# There are two versions of the cross compiler, a 32 bit version targeting arm, and a 64
# bit version targeting arm64.
#

#
# we need to compile the cross build against llvm so we need to configure/build that first
#
ifdef INCLUDE_DEVICE
cross:: build-llvm
cross64:: build-llvm64

setup:: setup-llvm setup-llvm64
clean-local:: clean-llvm clean-llvm64

setup-llvm: .stamp-configure-llvm
setup-llvm64: .stamp-configure-llvm64
endif

LLVM_BASE_CONFIGURE_FLAGS=--enable-libcpp --enable-optimized --enable-assertions=no --disable-jit --disable-docs --disable-doxygen
LLVM_CXXFLAGS=$(CCACHE_CXXFLAGS)
LLVM_BASE_CONFIGURE_ENVIRONMENT=$(COMMON_ACVARS)

# This is used just for linking with the 32 bit cross compiler
# FIXME: Avoid building the libs not needed by mono
.stamp-configure-llvm: $(LLVM_PATH)/configure
	@mkdir -p llvm/
	@echo "Configuring llvm"
	@cd llvm && \
	$(LLVM_BASE_CONFIGURE_ENVIRONMENT) \
	$(abspath $(LLVM_PATH)/configure) --prefix=$$PWD/usr --build=i386-apple-darwin10.7  --enable-targets="arm" $(LLVM_BASE_CONFIGURE_FLAGS) --cache-file=../llvm.config.cache CC="$(CCACHE)clang" CXX="$(CCACHE)clang++" CXXFLAGS="$(LLVM_CXXFLAGS)" > $@.log 2>&1 || (echo "Configuring llvm failed:" && cat $@.log | sed "s/^/    /" && exit 1)
	$(Q) @touch $@
	@echo Configured llvm

.stamp-configure-llvm64: $(LLVM_PATH)/configure
	@mkdir -p llvm64/
	@echo "Configuring llvm64"
	@cd llvm64 && \
	$(LLVM_BASE_CONFIGURE_ENVIRONMENT) \
	$(abspath $(LLVM_PATH)/configure) --prefix=$$PWD/usr --enable-targets="arm arm64" $(LLVM_BASE_CONFIGURE_FLAGS) --cache-file=../llvm64.config.cache CC="$(CCACHE)clang" CXX="$(CCACHE)clang++" CXXFLAGS="-Qunused-arguments" > $@.log 2>&1 || (echo "Configuring llvm64 failed:" && cat $@.log | sed "s/^/    /" && exit 1)
	$(Q) @touch $@
	@echo "Configured llvm64"

build-llvm: .stamp-build-llvm
build-llvm64: .stamp-build-llvm64

.stamp-build-llvm: .stamp-configure-llvm $(LLVM_DEPENDENCIES)
	$(MAKE) -C llvm all install
	$(Q) touch $@

.stamp-build-llvm64: .stamp-configure-llvm64 $(LLVM_DEPENDENCIES)
	$(MAKE) -C llvm64 all install
	$(Q) touch $@

really-build-llvm64:
	$(MAKE) -C llvm64 all install
	$(Q) touch .stamp-configure-llvm64

clean-llvm:
	-rm -rf llvm .stamp-configure-llvm .stamp-build-llvm llvm.config.cache
clean-llvm64:
	-rm -rf llvm64 .stamp-configure-llvm64 .stamp-build-llvm64 llvm64.config.cache

.PHONY: install-llvm64 llvm llvm64

install-llvm: .stamp-build-llvm install-directories

LLVM_TARGETS = \
	$(PREFIX)/LLVM/bin/opt \
	$(PREFIX)/LLVM/bin/llc \

$(PREFIX)/LLVM/bin/%: llvm64/usr/bin/% | $(PREFIX)/LLVM/bin
	$(call Q_2,INSTALL ,[LLVM64]) install -c -m 0755 $(INSTALL_STRIP_FLAG) $^ $@

$(PREFIX)/LLVM/bin:
	$(Q) mkdir -p $@

install-llvm64: .stamp-build-llvm64 $(LLVM_TARGETS)

llvm: setup-llvm build-llvm install-llvm
llvm64: install-llvm64

$(MONO_PATH)/tools/offsets-tool/MonoAotOffsetsDumper.exe: $(wildcard $(MONO_PATH)/tools/offsets-tool/*.cs)
	$(Q) $(MAKE) -C $(dir $@) MonoAotOffsetsDumper.exe

ifneq ($(WATCH_MONO_PATH),$(MONO_PATH))
$(WATCH_MONO_PATH)/tools/offsets-tool/MonoAotOffsetsDumper.exe: $(wildcard $(WATCH_MONO_PATH)/tools/offsets-tool/*.cs)
	$(Q) $(MAKE) -C $(dir $@) MonoAotOffsetsDumper.exe
endif

target7/mono/arch/arm/arm_dpimacros.h: .stamp-configure-target7
	$(Q) $(MAKE) -C $(dir $@) $(notdir $@)

targetwatch/mono/arch/arm/arm_dpimacros.h: .stamp-configure-targetwatch
	$(Q) $(MAKE) -C $(dir $@) $(notdir $@)

# template definition for generating AOT cross offsets for an ABI
# parameters are:
#  $(1): target ABI
#  $(2): output directory
#  $(3): build
define GenerateCrossOffsets
$(3)/$(1).h: .stamp-configure-$(3) target7/mono/arch/arm/arm_dpimacros.h targetwatch/mono/arch/arm/arm_dpimacros.h $(4)/tools/offsets-tool/MonoAotOffsetsDumper.exe
	$(Q) MONO_PATH=$(4)/tools/offsets-tool/CppSharp/osx_32 \
	$(SYSTEM_MONO32) $(4)/tools/offsets-tool/MonoAotOffsetsDumper.exe --abi $(1) --out $(2) --mono $(abspath $(4)) --maccore $(abspath $(TOP))
endef

CROSS_BASE_CFLAGS=-DMONOTOUCH $(CROSS_BUILD_CFLAGS) $(CCACHE_CFLAGS)
CROSS_BASE_CXXFLAGS=-DMONOTOUCH $(CCACHE_CXXFLAGS)

CROSS_BASE_CONFIGURE_FLAGS= \
			--with-glib=embedded \
			--enable-minimal=com,remoting \
			--disable-mcs-build \
			--enable-llvm \
			--enable-icall-symbol-map \
			--disable-nls \
			--disable-iconv \
			--disable-libraries	\
			--disable-boehm \
			$(XAMARIN_IOS_CONFIGURE_FLAGS)

#
# 32 bit cross compiler
#

# mono's configure already adds an -arch i386 flag, and cache doesn't like duplicate -arch flags
CROSS_CFLAGS=$(CROSS_BASE_CFLAGS) -mmacosx-version-min=$(MIN_OSX_SDK_VERSION)
CROSS_CXXFLAGS=$(CROSS_BASE_CXXFLAGS) -mmacosx-version-min=$(MIN_OSX_SDK_VERSION) -stdlib=libc++
CROSS_LDFLAGS=-stdlib=libc++ $(COMMON_LDFLAGS)

CROSS_CONFIGURE_FLAGS=$(CROSS_BASE_CONFIGURE_FLAGS)	\
	--prefix=$(BUILD_DESTDIR)/cross \
	--build=i386-apple-darwin10 \
	--cache-file=../cross.config.cache \
	--target=arm-darwin \
	--with-cross-offsets=arm-apple-darwin10.h \
	--with-llvm=../llvm/usr

CROSS_CONFIGURE_ENVIRONMENT = \
	CFLAGS="$(CROSS_CFLAGS)" \
	CXXFLAGS="$(CROSS_CXXFLAGS)" \
	LDFLAGS="$(CROSS_LDFLAGS)" \
	CC="$(MAC_CC)" \
	CXX="$(MAC_CXX)" \

ifdef INCLUDE_IOS
ifdef INCLUDE_DEVICE
cross:: build-cross install-cross

.PHONY: setup-cross build-cross install-cross

setup:: setup-cross
build:: build-cross
install-local:: install-cross
clean-local:: clean-cross
endif
endif

setup-cross: .stamp-configure-cross

.stamp-configure-cross: $(MONO_PATH)/configure .stamp-build-llvm
	$(Q) $(CROSS_CONFIGURE_ENVIRONMENT) ./wrap-configure.sh cross $(abspath $(MONO_PATH)/configure) $(CROSS_CONFIGURE_FLAGS)

$(eval $(call GenerateCrossOffsets,arm-apple-darwin10,$(TOP)/builds/cross/,cross,$(MONO_PATH)))

.stamp-build-cross: .stamp-configure-cross cross/arm-apple-darwin10.h $(MONO_DEPENDENCIES)
	$(MAKE) -C cross/mono
	$(MAKE) -C cross/mono install
	$(Q) touch $@

build-cross: .stamp-build-cross

CROSS_TARGETS = \
	$(PREFIX)/bin/arm-darwin-mono-sgen  \

$(BUILD_DESTDIR)/cross/bin/arm-darwin-mono-sgen: .stamp-build-cross

$(PREFIX)/bin/arm-darwin-mono-sgen: $(BUILD_DESTDIR)/cross/bin/arm-darwin-mono-sgen | $(PREFIX)/bin
	$(call Q_2,INSTALL ,[CROSS]) install -c -m 0755 $(INSTALL_STRIP_FLAG) $< $@

$(PREFIX)/bin:
	$(Q) mkdir -p $@

install-cross: install-llvm64 $(CROSS_TARGETS)

clean-cross:
	-rm -rf cross .stamp-configure-cross cross.config.cache

#
# 64 bit cross compiler
#

CROSS64_CFLAGS=$(CROSS_BASE_CFLAGS) -mmacosx-version-min=10.8
CROSS64_CXXFLAGS=$(CROSS_BASE_CXXFLAGS) -mmacosx-version-min=10.8 -stdlib=libc++
CROSS64_LDFLAGS=-stdlib=libc++ $(COMMON_LDFLAGS)

CROSS64_CONFIGURE_FLAGS=$(CROSS_BASE_CONFIGURE_FLAGS)	\
	--prefix=$(BUILD_DESTDIR)/cross64 \
	--cache-file=../cross64.config.cache \
	--target=aarch64-darwin \
	--with-cross-offsets=aarch64-apple-darwin10.h \
	--with-llvm=../llvm64/usr

CROSS64_CONFIGURE_ENVIRONMENT = \
	CFLAGS="$(CROSS64_CFLAGS)" \
	CXXFLAGS="$(CROSS64_CXXFLAGS)" \
	LDFLAGS="$(CROSS64_LDFLAGS)" \
	CC="$(MAC_CC)" \
	CXX="$(MAC_CXX)" \

cross64:: build-cross64 install-cross64

ifdef INCLUDE_IOS
ifdef INCLUDE_DEVICE
setup:: setup-cross64
build:: build-cross64
install-local:: install-cross64
clean-local:: clean-cross64
endif
endif

setup-cross64: .stamp-configure-cross64

install-directories:
	install -d $(PREFIX)/bin
	install -d $(PREFIX)/share/mono
	install -d $(PREFIX)/lib/mono/2.1/Facades
	install -d $(PREFIX)/lib/mono/Xamarin.iOS
	install -d $(IOS_DESTDIR)/$(IPHONESIMULATOR_PREFIX)/include
	install -d $(IOS_DESTDIR)/$(IPHONESIMULATOR_PREFIX)/lib
	install -d $(IOS_DESTDIR)/$(IPHONEOS_PREFIX)/include
	install -d $(IOS_DESTDIR)/$(IPHONEOS_PREFIX)/lib
	install -d $(PREFIX)/LLVM/bin

.stamp-configure-cross64: $(MONO_PATH)/configure .stamp-build-llvm64
	$(Q) $(CROSS64_CONFIGURE_ENVIRONMENT) ./wrap-configure.sh cross64 $(abspath $(MONO_PATH)/configure) $(CROSS64_CONFIGURE_FLAGS)

$(eval $(call GenerateCrossOffsets,aarch64-apple-darwin10,$(TOP)/builds/cross64/,cross64,$(MONO_PATH)))

.stamp-build-cross64: .stamp-configure-cross64 cross64/aarch64-apple-darwin10.h $(MONO_DEPENDENCIES)
	$(MAKE) -C cross64/mono
	@$(MAKE) -C cross64/mono install
	$(Q) touch $@

build-cross64: .stamp-build-cross64

CROSS64_TARGETS = \
	$(PREFIX)/bin/arm64-darwin-mono-sgen \

$(BUILD_DESTDIR)/cross64/bin/aarch64-darwin-mono-sgen: .stamp-build-cross64

$(PREFIX)/bin/arm64-darwin-mono-sgen: $(BUILD_DESTDIR)/cross64/bin/aarch64-darwin-mono-sgen | $(PREFIX)/bin
	$(call Q_2,INSTALL ,[CROSS64]) install -c -m 0755 $(INSTALL_STRIP_FLAG) $< $@

install-cross64: install-llvm64 $(CROSS64_TARGETS)

clean-cross64:
	-rm -rf cross64 .stamp-*-cross64 cross64.config.cache


#
# 32 bit watch cross compiler
#

WATCH_CROSS_CONFIGURE_FLAGS=$(CROSS_BASE_CONFIGURE_FLAGS)	\
	--prefix=$(BUILD_DESTDIR)/cross-watch \
	--build=i386-apple-darwin10 \
	--cache-file=../cross-watch.config.cache \
	--target=armv7k-unknown-darwin \
	--with-cooperative-gc=yes \
	--with-cross-offsets=armv7k-apple-darwin.h \
	--with-llvm=../llvm/usr

WATCH_CONFIGURE_ENVIRONMENT = \
	CFLAGS="$(CROSS_CFLAGS)" \
	CXXFLAGS="$(CROSS_CXXFLAGS)" \
	LDFLAGS="$(CROSS_LDFLAGS)" \
	CC="$(MAC_CC)" \
	CXX="$(MAC_CXX)" \

ifdef INCLUDE_WATCH
ifdef INCLUDE_DEVICE
cross:: build-cross-watch install-cross-watch

.PHONY: setup-cross-watch build-cross-watch install-cross-watch

setup:: setup-cross-watch
build:: build-cross-watch
install-local:: install-cross-watch
clean-local:: clean-cross-watch
endif
endif

.PHONY: cross-watch
cross-watch: build-cross-watch install-cross-watch

setup-cross-watch: .stamp-configure-cross-watch

.stamp-configure-cross-watch: $(WATCH_MONO_PATH)/configure .stamp-build-llvm
	$(Q) $(WATCH_CONFIGURE_ENVIRONMENT) ./wrap-configure.sh cross-watch $(abspath $(WATCH_MONO_PATH)/configure) $(WATCH_CROSS_CONFIGURE_FLAGS)

build-cross: setup-cross

$(eval $(call GenerateCrossOffsets,armv7k-apple-darwin,$(TOP)/builds/cross-watch/,cross-watch,$(WATCH_MONO_PATH)))

.stamp-build-cross-watch: .stamp-configure-cross-watch cross-watch/armv7k-apple-darwin.h $(MONO_DEPENDENCIES)
	$(MAKE) -C cross-watch/mono
	$(MAKE) -C cross-watch/mono install
	$(Q) touch $@

build-cross-watch: .stamp-build-cross-watch

CROSS_WATCH_TARGETS = \
	$(PREFIX)/bin/armv7k-unknown-darwin-mono-sgen  \

$(BUILD_DESTDIR)/cross-watch/bin/armv7k-unknown-darwin-mono-sgen: .stamp-build-cross-watch

$(PREFIX)/bin/armv7k-unknown-darwin-mono-sgen: $(BUILD_DESTDIR)/cross-watch/bin/armv7k-unknown-darwin-mono-sgen | $(PREFIX)/bin
	$(call Q_2,INSTALL ,[CROSS]) install -c -m 0755 $(INSTALL_STRIP_FLAG) $< $@

install-cross-watch: install-llvm $(CROSS_WATCH_TARGETS)

clean-cross-watch:
	-rm -rf cross-watch .stamp-configure-cross-watch cross-watch.config.cache
