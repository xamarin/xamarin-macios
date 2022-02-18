TOP=../../..

include $(TOP)/Make.config

.libs/macos/custom-type-assembly.dll: custom-type-assembly.cs Makefile | .libs/macos
	$(Q_CSC) $(MAC_mobile_CSC) $< -out:$@ -r:$(TOP)/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll -target:library

.libs/dotnet/macos/custom-type-assembly.dll: custom-type-assembly.cs Makefile | .libs/dotnet/macos
	$(Q_CSC) $(DOTNET6_CSC) $< -out:$@ -r:$(TOP)/_build/Microsoft.macOS.Ref/ref/net6.0/Microsoft.macOS.dll -target:library -r:$(DOTNET6_BCL_DIR)/System.Runtime.dll /nologo

.libs/macos .libs/dotnet/macos:
	$(Q) mkdir -p $@

ifdef INCLUDE_XAMARIN_LEGACY
TARGETS += \
	.libs/macos/custom-type-assembly.dll \

endif

ifdef ENABLE_DOTNET
TARGETS += \
	.libs/dotnet/macos/custom-type-assembly.dll \

endif

build-assembly: $(TARGETS)
