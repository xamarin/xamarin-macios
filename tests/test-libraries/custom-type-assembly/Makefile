TOP=../../..

include $(TOP)/Make.config

.libs/dotnet/macos/custom-type-assembly.dll: bin/Debug/$(DOTNET_TFM)-macos/custom-type-assembly.dll | .libs/dotnet/macos
	$(Q) $(CP) $< $@

.libs/dotnet/macos:
	$(Q) mkdir -p $@

bin/Debug/$(DOTNET_TFM)-macos/custom-type-assembly.dll: custom-type-assembly.csproj custom-type-assembly.cs
	$(Q) unset MSBUILD_EXE_PATH && $(DOTNET) build $< "/bl:$@.binlog" $(DOTNET_BUILD_VERBOSITY)

ifdef INCLUDE_MAC
TARGETS += \
	.libs/dotnet/macos/custom-type-assembly.dll \

endif # INCLUDE_MAC

build-assembly: $(TARGETS)
