TOP=../..
include $(TOP)/Make.config

export DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT)

#
# mmp
#

MMP_CONF=Release
MMP_DIR=bin/$(MMP_CONF)
LOCAL_MMP=$(MMP_DIR)/mmp.exe
LOCAL_MMP_COMMAND=$(SYSTEM_MONO) --debug $(LOCAL_MMP)

# mmp.csproj.inc contains the mmp_dependencies variable used to determine if mmp needs to be rebuilt or not.
mmp.csproj.inc: export BUILD_VERBOSITY=$(MSBUILD_VERBOSITY)
-include mmp.csproj.inc

$(MMP_DIR)/mmp.exe: $(mmp_dependencies)
	$(Q_GEN) $(SYSTEM_MSBUILD) "/bl:$@.binlog" mmp.csproj /r $(XBUILD_VERBOSITY) /p:Configuration=$(MMP_CONF)

MMP_TARGETS_DOTNET = \
	$(DOTNET_DESTDIR)/$(osx-x64_NUGET_RUNTIME_NAME)/runtimes/osx-x64/native/Microsoft.macOS.registrar.a \
	$(DOTNET_DESTDIR)/$(osx-arm64_NUGET_RUNTIME_NAME)/runtimes/osx-arm64/native/Microsoft.macOS.registrar.a \
	$(DOTNET_DESTDIR)/$(osx-x64_NUGET_RUNTIME_NAME)/runtimes/osx-x64/native/Microsoft.macOS.registrar.coreclr.a \
	$(DOTNET_DESTDIR)/$(osx-arm64_NUGET_RUNTIME_NAME)/runtimes/osx-arm64/native/Microsoft.macOS.registrar.coreclr.a \

MMP_TARGETS = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mmp \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/mmp.exe \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.mobile.a \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.full.a \

MMP_DIRECTORIES_DOTNET = \
	$(DOTNET_DESTDIR)/$(osx-x64_NUGET_RUNTIME_NAME)/runtimes/osx-x64/native \
	$(DOTNET_DESTDIR)/$(osx-arm64_NUGET_RUNTIME_NAME)/runtimes/osx-arm64/native \

MMP_DIRECTORIES = \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin \
	$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp \
	$(MMP_DIRECTORIES_DOTNET) \

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin/mmp: mmp | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/bin
	$(Q) $(CP) $< $@

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/mmp.exe: $(MMP_DIR)/mmp.exe | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp
	$(Q) $(CP) $(dir $<)/*.exe $(dir $@)
	$(Q) $(CP) $(dir $<)/*.dll $(dir $@)
	$(Q) $(CP) $(dir $<)/*.pdb $(dir $@)

$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp/Xamarin.Mac.registrar.%.a: Xamarin.Mac.registrar.%.a | $(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR)/lib/mmp
	$(Q) $(CP) $<* $(dir $@)

$(DOTNET_DESTDIR)/$(osx-x64_NUGET_RUNTIME_NAME)/runtimes/osx-x64/native/Microsoft.macOS.registrar.a: Microsoft.macOS.registrar.x86_64.a | $(DOTNET_DESTDIR)/$(osx-x64_NUGET_RUNTIME_NAME)/runtimes/osx-x64/native
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/$(osx-x64_NUGET_RUNTIME_NAME)/runtimes/osx-x64/native/Microsoft.macOS.registrar.coreclr.a: Microsoft.macOS.registrar.coreclr.x86_64.a | $(DOTNET_DESTDIR)/$(osx-x64_NUGET_RUNTIME_NAME)/runtimes/osx-x64/native
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/$(osx-arm64_NUGET_RUNTIME_NAME)/runtimes/osx-arm64/native/Microsoft.macOS.registrar.a: Microsoft.macOS.registrar.arm64.a | $(DOTNET_DESTDIR)/$(osx-arm64_NUGET_RUNTIME_NAME)/runtimes/osx-arm64/native
	$(Q) $(CP) $< $@

$(DOTNET_DESTDIR)/$(osx-arm64_NUGET_RUNTIME_NAME)/runtimes/osx-arm64/native/Microsoft.macOS.registrar.coreclr.a: Microsoft.macOS.registrar.coreclr.arm64.a | $(DOTNET_DESTDIR)/$(osx-arm64_NUGET_RUNTIME_NAME)/runtimes/osx-arm64/native
	$(Q) $(CP) $< $@

$(MMP_DIRECTORIES):
	$(Q) mkdir -p $@

#
# Partial static registrar libraries
#

GENERATE_PART_REGISTRAR = $(Q_GEN) $(LOCAL_MMP_COMMAND) --xamarin-framework-directory=$(MAC_DESTDIR)$(MAC_FRAMEWORK_CURRENT_DIR) -q --runregistrar:$(abspath $@) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(MACOS_SDK_VERSION) $(abspath $<) --registrar:static --nolink

Xamarin.Mac.registrar.mobile.x86_64.m: $(TOP)/src/build/mac/mobile-64/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=x86_64 --target-framework Xamarin.Mac,Version=v2.0,Profile=Mobile -a:$(MOBILE_BCL_DIR)/mscorlib.dll
	$(Q) touch Xamarin.Mac.registrar.mobile.x86_64.m Xamarin.Mac.registrar.mobile.x86_64.h

Xamarin.Mac.registrar.mobile.arm64.m: $(TOP)/src/build/mac/mobile-64/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=arm64 --target-framework Xamarin.Mac,Version=v2.0,Profile=Mobile -a:$(MOBILE_BCL_DIR)/mscorlib.dll
	$(Q) touch Xamarin.Mac.registrar.mobile.arm64.m Xamarin.Mac.registrar.mobile.arm64.h

Microsoft.macOS.registrar.x86_64.m: $(TOP)/src/build/dotnet/macos/64/Microsoft.macOS.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=x86_64 --target-framework .NETCoreApp,Version=$(subst net,,$(DOTNET_TFM)),Profile=macos -a:$(DOTNET_BCL_DIR)/System.Runtime.dll -a:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll
	$(Q) touch $@ $(basename $@).h

Microsoft.macOS.registrar.coreclr.x86_64.m: $(TOP)/src/build/dotnet/macos/64/Microsoft.macOS.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=x86_64 --target-framework .NETCoreApp,Version=$(subst net,,$(DOTNET_TFM)),Profile=macos -a:$(DOTNET_BCL_DIR)/System.Runtime.dll -a:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll --xamarin-runtime CoreCLR
	$(Q) touch $@ $(basename $@).h

Xamarin.Mac.registrar.full.x86_64.m:   $(TOP)/src/build/mac/full-64/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=x86_64 --target-framework Xamarin.Mac,Version=v4.5,Profile=Full -a:$(FULL_BCL_DIR)/mscorlib.dll
	$(Q) touch Xamarin.Mac.registrar.full.x86_64.m Xamarin.Mac.registrar.full.x86_64.h

Microsoft.macOS.registrar.arm64.m: $(TOP)/src/build/dotnet/macos/64/Microsoft.macOS.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=arm64 --target-framework .NETCoreApp,Version=$(subst net,,$(DOTNET_TFM)),Profile=macos -a:$(DOTNET_BCL_DIR)/System.Runtime.dll -a:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll
	$(Q) touch $@ $(basename $@).h

Microsoft.macOS.registrar.coreclr.arm64.m: $(TOP)/src/build/dotnet/macos/64/Microsoft.macOS.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=arm64 --target-framework .NETCoreApp,Version=$(subst net,,$(DOTNET_TFM)),Profile=macos -a:$(DOTNET_BCL_DIR)/System.Runtime.dll -a:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll --xamarin-runtime CoreCLR
	$(Q) touch $@ $(basename $@).h

Xamarin.Mac.registrar.full.arm64.m:   $(TOP)/src/build/mac/full-64/Xamarin.Mac.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=arm64 --target-framework Xamarin.Mac,Version=v4.5,Profile=Full -a:$(FULL_BCL_DIR)/mscorlib.dll
	$(Q) touch Xamarin.Mac.registrar.full.arm64.m Xamarin.Mac.registrar.full.arm64.h

%.x86_64.a: %.x86_64.m
	$(Q_CC) $(MAC_CC) -DDEBUG -g -gdwarf-2 -x objective-c++ -std=c++14 -o $@ -c -arch x86_64 $< -Wall -Wno-unguarded-availability-new -I$(TOP)/runtime -mmacosx-version-min=$(MIN_MACOS_SDK_VERSION) -fobjc-runtime=macosx $(EXTRA_DEFINES)

%.arm64.a: %.arm64.m
	$(Q_CC) $(MAC_CC) -DDEBUG -g -gdwarf-2 -x objective-c++ -std=c++14 -o $@ -c -arch arm64 $< -Wall -Wno-unguarded-availability-new -I$(TOP)/runtime -mmacosx-version-min=$(MIN_MACOS_SDK_VERSION) -fobjc-runtime=macosx $(EXTRA_DEFINES)

Xamarin.Mac.registrar.%.a: Xamarin.Mac.registrar.%.x86_64.a Xamarin.Mac.registrar.%.arm64.a
	$(Q_LIPO) $(DEVICE_BIN_PATH)/lipo -create -output $@ $^

dotnet: $(MMP_TARGETS_DOTNET)
ifdef ENABLE_DOTNET
ifdef INCLUDE_MAC
install-local:: $(MMP_TARGETS_DOTNET)
all-local:: $(MMP_TARGETS_DOTNET)
endif
endif

ifdef INCLUDE_XAMARIN_LEGACY
ifdef INCLUDE_MAC
install-local:: $(MMP_TARGETS)
all-local:: $(MMP_TARGETS)
endif
endif

clean-local::
	rm -f mmp.stub.c mmp.helper.o
	rm -rf bin obj
	rm -f Xamarin.Mac.registrar.*
	$(SYSTEM_MSBUILD) "/t:Clean" /p:Configuration=$(MMP_CONF) *.csproj

include $(TOP)/mk/rules.mk
include ../common/Make.common

# make will automatically consider files created in chained implicit rules as temporary files, and delete them afterwards
# marking those files as .SECONDARY will prevent that deletion.
.SECONDARY: $(foreach ext,h m a,Xamarin.Mac.registrar.mobile.x86_64.$(ext) Xamarin.Mac.registrar.mobile.arm64.$(ext) Xamarin.Mac.registrar.full.x86_64.$(ext) Xamarin.Mac.registrar.full.arm64.$(ext))
.SECONDARY: Xamarin.Mac.registrar.mobile.a Xamarin.Mac.registrar.full.a
