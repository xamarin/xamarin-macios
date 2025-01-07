TOP=../..
include $(TOP)/Make.config
include $(TOP)/mk/rules.mk

export DEVELOPER_DIR=$(XCODE_DEVELOPER_ROOT)

#
# mmp
#

MMP_CONF=Release
MMP_DIR=bin/$(MMP_CONF)
LOCAL_MMP=$(MMP_DIR)/mmp.dll
LOCAL_MMP_COMMAND=$(DOTNET) exec $(LOCAL_MMP)

# mmp.csproj.inc contains the mmp_dependencies variable used to determine if mmp needs to be rebuilt or not.
mmp.csproj.inc: export BUILD_VERBOSITY=$(MSBUILD_VERBOSITY)
mmp.csproj.inc: export DOTNET:=$(DOTNET)
-include mmp.csproj.inc

$(LOCAL_MMP): $(mmp_dependencies)
	$(Q_GEN) $(DOTNET) build "/bl:$@.binlog" mmp.csproj $(XBUILD_VERBOSITY) /p:Configuration=$(MMP_CONF)

MMP_TARGETS_DOTNET = \
	$(DOTNET_DESTDIR)/$(osx-x64_NUGET_RUNTIME_NAME)/runtimes/osx-x64/native/Microsoft.macOS.registrar.a \
	$(DOTNET_DESTDIR)/$(osx-arm64_NUGET_RUNTIME_NAME)/runtimes/osx-arm64/native/Microsoft.macOS.registrar.a \
	$(DOTNET_DESTDIR)/$(osx-x64_NUGET_RUNTIME_NAME)/runtimes/osx-x64/native/Microsoft.macOS.registrar.coreclr.a \
	$(DOTNET_DESTDIR)/$(osx-arm64_NUGET_RUNTIME_NAME)/runtimes/osx-arm64/native/Microsoft.macOS.registrar.coreclr.a \

MMP_DIRECTORIES_DOTNET = \
	$(DOTNET_DESTDIR)/$(osx-x64_NUGET_RUNTIME_NAME)/runtimes/osx-x64/native \
	$(DOTNET_DESTDIR)/$(osx-arm64_NUGET_RUNTIME_NAME)/runtimes/osx-arm64/native \

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

GENERATE_PART_REGISTRAR = $(Q_GEN) $(LOCAL_MMP_COMMAND) -q --runregistrar:$(abspath $@) --sdkroot $(XCODE_DEVELOPER_ROOT) --sdk $(MACOS_SDK_VERSION) $(abspath $<) --registrar:static --nolink

Microsoft.macOS.registrar.x86_64.m: $(TOP)/src/build/dotnet/macos/64/Microsoft.macOS.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=x86_64 --target-framework .NETCoreApp,Version=$(subst net,,$(DOTNET_TFM)),Profile=macos -a:$(DOTNET_BCL_DIR)/System.Runtime.dll -a:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll
	$(Q) touch $@ $(basename $@).h

Microsoft.macOS.registrar.coreclr.x86_64.m: $(TOP)/src/build/dotnet/macos/64/Microsoft.macOS.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=x86_64 --target-framework .NETCoreApp,Version=$(subst net,,$(DOTNET_TFM)),Profile=macos -a:$(DOTNET_BCL_DIR)/System.Runtime.dll -a:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll --xamarin-runtime CoreCLR
	$(Q) touch $@ $(basename $@).h

Microsoft.macOS.registrar.arm64.m: $(TOP)/src/build/dotnet/macos/64/Microsoft.macOS.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=arm64 --target-framework .NETCoreApp,Version=$(subst net,,$(DOTNET_TFM)),Profile=macos -a:$(DOTNET_BCL_DIR)/System.Runtime.dll -a:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll
	$(Q) touch $@ $(basename $@).h

Microsoft.macOS.registrar.coreclr.arm64.m: $(TOP)/src/build/dotnet/macos/64/Microsoft.macOS.dll $(LOCAL_MMP)
	$(GENERATE_PART_REGISTRAR) --arch=arm64 --target-framework .NETCoreApp,Version=$(subst net,,$(DOTNET_TFM)),Profile=macos -a:$(DOTNET_BCL_DIR)/System.Runtime.dll -a:$(DOTNET_BCL_DIR)/System.Runtime.InteropServices.dll --xamarin-runtime CoreCLR
	$(Q) touch $@ $(basename $@).h

%.x86_64.a: %.x86_64.m
	$(Q_CC) $(CLANG) -isysroot $(macos_SDK) -DDEBUG -g -gdwarf-2 -x objective-c++ -std=c++14 -o $@ -c -arch x86_64 $< -Wall -Wno-unguarded-availability-new -I$(TOP)/runtime -mmacosx-version-min=$(MIN_MACOS_SDK_VERSION) -fobjc-runtime=macosx $(EXTRA_DEFINES)

%.arm64.a: %.arm64.m
	$(Q_CC) $(CLANG) -isysroot $(macos_SDK) -DDEBUG -g -gdwarf-2 -x objective-c++ -std=c++14 -o $@ -c -arch arm64 $< -Wall -Wno-unguarded-availability-new -I$(TOP)/runtime -mmacosx-version-min=$(MIN_MACOS_SDK_VERSION) -fobjc-runtime=macosx $(EXTRA_DEFINES)

dotnet: $(MMP_TARGETS_DOTNET)
ifdef INCLUDE_MAC
install-local:: $(MMP_TARGETS_DOTNET)
all-local:: $(MMP_TARGETS_DOTNET)
endif

clean-local::
	rm -rf bin obj

include ../common/Make.common
