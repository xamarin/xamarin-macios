TOP=../../..

include $(TOP)/Make.config

.libs/macos/custom-type-assembly.dll: custom-type-assembly.cs Makefile | .libs/macos
	$(MAC_mobile_CSC) $< -out:$@ -r:$(TOP)/_mac-build/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono/Xamarin.Mac/Xamarin.Mac.dll -target:library

.libs/macos:
	$(Q) mkdir -p $@

TARGETS += \
	.libs/macos/custom-type-assembly.dll \

build-assembly: $(TARGETS)
