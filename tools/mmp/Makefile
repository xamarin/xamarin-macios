TOP=../..
include $(TOP)/Make.config

export DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT)

#
# mmp
#

MMP_CONF=Release
MMP_DIR=bin/$(MMP_CONF)/netcoreapp3.1/publish
LOCAL_MMP=$(MMP_DIR)/mmp
LOCAL_MMP_COMMAND=$(MMP_DIR)/mmp

# mmp.csproj.inc contains the mmp_dependencies variable used to determine if mmp needs to be rebuilt or not.
-include mmp.csproj.inc

$(MMP_DIR)/mmp: $(mmp_dependencies)
	$(Q_GEN) $(DOTNET) publish $(XBUILD_VERBOSITY) /p:Configuration=$(MMP_CONF)

MMP_TARGETS = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mmp \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/mmp \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.mobile.a \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.full.a \

MMP_DIRECTORIES = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp \

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mmp: mmp | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin
	$(Q) $(CP) $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/mmp: $(MMP_DIR)/mmp | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp
	$(Q) rm -Rf $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/
	$(Q) $(CP) -r $(MMP_DIR)/ $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.%.a: Xamarin.Mac.registrar.%.a | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp
	$(Q) $(CP) $<* $(dir $@)

$(MMP_DIRECTORIES):
	$(Q) mkdir -p $@

#
# Partial static registrar libraries
#

GENERATE_PART_REGISTRAR = $(Q_GEN) $(LOCAL_MMP_COMMAND) --xamarin-framework-directory=$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR) -q --runregistrar:$(abspath $@) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(OSX_SDK_VERSION)   $< --registrar:static

Xamarin.Mac.registrar.mobile.x86_64.m: $(TOP)/src/build/mac/mobile-64/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --target-framework Xamarin.Mac,Version=v2.0,Profile=Mobile --arch=x86_64
	$(Q) touch Xamarin.Mac.registrar.mobile.x86_64.m Xamarin.Mac.registrar.mobile.x86_64.h

Xamarin.Mac.registrar.full.x86_64.m:   $(TOP)/src/build/mac/full-64/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --target-framework Xamarin.Mac,Version=v4.5,Profile=Full --arch=x86_64 --xamarin-full-framework --nolink
	$(Q) touch Xamarin.Mac.registrar.full.x86_64.m Xamarin.Mac.registrar.full.x86_64.h

Xamarin.Mac.registrar.%.x86_64.a: Xamarin.Mac.registrar.%.x86_64.m
	$(Q_CC) xcrun -sdk macosx clang -DDEBUG -g -gdwarf-2 -x objective-c++ -stdlib=libc++ -std=c++14 -o $@ -c -arch x86_64 $< -Wall -Wno-unguarded-availability-new -I$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/include -mmacosx-version-min=$(MIN_OSX_SDK_VERSION) -fobjc-runtime=macosx

Xamarin.Mac.registrar.%.a: Xamarin.Mac.registrar.%.x86_64.a
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^


ifdef INCLUDE_MAC
install-local:: $(MMP_TARGETS)
all-local:: $(MMP_TARGETS)
endif

clean-local::
	rm -f mmp.stub.c mmp.helper.o
	rm -rf bin obj
	rm -f Xamarin.Mac.registrar.*

include $(TOP)/mk/rules.mk
include ../common/Make.common

run::
	$(Q) make -C $(TOP)/tests/mmp-aot-tests

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# marking those files as .SECONDARY will prevent that deletion.
.SECONDARY: $(foreach ext,h m a,Xamarin.Mac.registrar.mobile.x86_64.$(ext) Xamarin.Mac.registrar.full.x86_64.$(ext))
.SECONDARY: Xamarin.Mac.registrar.mobile.a Xamarin.Mac.registrar.full.a
