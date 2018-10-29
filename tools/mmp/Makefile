TOP=../..
include $(TOP)/Make.config

#
# mmp
#

MMP_CONF=Release
MMP_DIR=bin/$(MMP_CONF)
LOCAL_MMP=$(MMP_DIR)/mmp.exe

# mmp.csproj.inc contains the mmp_dependencies variable used to determine if mmp needs to be rebuilt or not.
mmp.csproj.inc: Makefile ../common/create-makefile-fragment.sh
	$(Q_GEN) ../common/create-makefile-fragment.sh $(CURDIR)/mmp.csproj

-include mmp.csproj.inc

$(MMP_DIR)/mmp.exe: $(mmp_dependencies)
	$(Q_GEN) $(SYSTEM_MSBUILD) $(TOP)/Xamarin.Mac.sln "/t:mmp" $(XBUILD_VERBOSITY) /p:Configuration=$(MMP_CONF)

MMP_TARGETS = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mmp \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/mmp.exe \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.mobile.a \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.full.a \

MMP_DIRECTORIES = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp \

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mmp: mmp.in | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin
	$(Q_GEN) sed 's!@XAMARIN_MAC_PREFIX@!$(abspath $(MAC_TARGETDIR)$(MAC_FRAMEWORK_CURRENT_DIR))!g' $< > $@
	$(Q) chmod +x $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/mmp.exe: $(MMP_DIR)/mmp.exe | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp
	$(Q) $(CP) $(dir $<)/*.exe $(dir $@)
	$(Q) $(CP) $(dir $<)/*.dll $(dir $@)
	$(Q) $(CP) $(dir $<)/*.pdb $(dir $@)

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.%.a: Xamarin.Mac.registrar.%.a | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp
	$(Q) $(CP) $<* $(dir $@)

$(MMP_DIRECTORIES):
	$(Q) mkdir -p $@

#
# Partial static registrar libraries
#

GENERATE_PART_REGISTRAR = $(Q_GEN) $(SYSTEM_MONO) --debug $(LOCAL_MMP) --xamarin-framework-directory=$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR) -q --runregistrar:$(abspath $@) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(OSX_SDK_VERSION)   $< --registrar:static

Xamarin.Mac.registrar.mobile.i386.m:   $(TOP)/src/build/mac/mobile-32/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --target-framework Xamarin.Mac,Version=v2.0,Profile=Mobile --arch=i386
	$(Q) touch Xamarin.Mac.registrar.mobile.i386.m Xamarin.Mac.registrar.mobile.i386.h

Xamarin.Mac.registrar.mobile.x86_64.m: $(TOP)/src/build/mac/mobile-64/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --target-framework Xamarin.Mac,Version=v2.0,Profile=Mobile --arch=x86_64
	$(Q) touch Xamarin.Mac.registrar.mobile.x86_64.m Xamarin.Mac.registrar.mobile.x86_64.h

Xamarin.Mac.registrar.full.i386.m:     $(TOP)/src/build/mac/full-32/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --target-framework 4.5 --arch=i386 --xamarin-full-framework --nolink
	$(Q) touch Xamarin.Mac.registrar.full.i386.m Xamarin.Mac.registrar.full.i386.h

Xamarin.Mac.registrar.full.x86_64.m:   $(TOP)/src/build/mac/full-64/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --target-framework 4.5 --arch=x86_64 --xamarin-full-framework --nolink
	$(Q) touch Xamarin.Mac.registrar.full.x86_64.m Xamarin.Mac.registrar.full.x86_64.h

Xamarin.Mac.registrar.%.i386.a:   Xamarin.Mac.registrar.%.i386.m
	$(Q_CC) xcrun -sdk macosx clang -DDEBUG -g -gdwarf-2 -x objective-c++ -stdlib=libc++ -o $@ -c -arch i386 $< -Wall -Wno-unguarded-availability-new -I$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/include -mmacosx-version-min=$(MIN_OSX_SDK_VERSION)

Xamarin.Mac.registrar.%.x86_64.a: Xamarin.Mac.registrar.%.x86_64.m
	$(Q_CC) xcrun -sdk macosx clang -DDEBUG -g -gdwarf-2 -x objective-c++ -stdlib=libc++ -o $@ -c -arch x86_64 $< -Wall -Wno-unguarded-availability-new -I$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/include -mmacosx-version-min=$(MIN_OSX_SDK_VERSION) -fobjc-runtime=macosx

Xamarin.Mac.registrar.%.a: Xamarin.Mac.registrar.%.i386.a Xamarin.Mac.registrar.%.x86_64.a
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^


ifdef INCLUDE_MAC
install-local:: $(MMP_TARGETS)
all-local:: $(MMP_TARGETS)
endif

mmp: \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mmp \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/mmp.exe \

clean-local::
	rm -f mmp.stub.c mmp.helper.o
	rm -f bin obj
	rm -f Xamarin.Mac.registrar.*
	$(SYSTEM_MSBUILD) "/t:Clean" /p:Configuration=$(MMP_CONF) *.csproj

include $(TOP)/mk/rules.mk
include ../common/Make.common

run::
	$(Q) make -C tests

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# marking those files as .SECONDARY will prevent that deletion.
.SECONDARY: $(foreach ext,h m a,Xamarin.Mac.registrar.mobile.i386.$(ext) Xamarin.Mac.registrar.mobile.x86_64.$(ext) Xamarin.Mac.registrar.full.i386.$(ext) Xamarin.Mac.registrar.full.x86_64.$(ext))
.SECONDARY: Xamarin.Mac.registrar.mobile.a Xamarin.Mac.registrar.full.a
