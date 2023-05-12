TOP=../../..

include $(TOP)/Make.config

.libs/macos/custom-type-assembly.dll: custom-type-assembly.cs Makefile | .libs/macos
	$(Q_CSC) $(MAC_mobile_CSC) $< -out:$@ -r:$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll -target:library /nologo

.libs/dotnet/macos/custom-type-assembly.dll: bin/Debug/$(DOTNET_TFM)-macos/custom-type-assembly.dll | .libs/dotnet/macos
	$(Q) $(CP) $< $@

.libs/macos .libs/dotnet/macos:
	$(Q) mkdir -p $@

bin/Debug/$(DOTNET_TFM)-macos/custom-type-assembly.dll: custom-type-assembly.csproj custom-type-assembly.cs
	$(Q) unset MSBUILD_EXE_PATH && $(DOTNET) build $< "/bl:$@.binlog" $(MSBUILD_VERBOSITY)

ifdef INCLUDE_MAC

ifdef INCLUDE_XAMARIN_LEGACY
TARGETS += \
	.libs/macos/custom-type-assembly.dll \

endif

ifdef ENABLE_DOTNET
TARGETS += \
	.libs/dotnet/macos/custom-type-assembly.dll \

endif

endif # INCLUDE_MAC

build-assembly: $(TARGETS)
