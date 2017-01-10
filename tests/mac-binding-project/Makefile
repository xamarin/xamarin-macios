TOP=../..

.NOTPARALLEL:

include $(TOP)/Make.config

export MD_APPLE_SDK_ROOT=$(shell dirname `dirname $(XCODE_DEVELOPER_ROOT)`)
export XBUILD_FRAMEWORK_FOLDERS_PATH=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPath=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XAMMAC_FRAMEWORK_PATH=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

obj/SimpleClass.o:
	$(Q) mkdir -p bin obj
	$(Q) clang -c ../common/mac/SimpleClass.m -o obj/SimpleClass.o -std=gnu99 -mmacosx-version-min=10.9

bin/SimpleClassDylib.dylib: obj/SimpleClass.o
	$(Q) xcrun libtool -dynamic obj/SimpleClass.o -o bin/SimpleClassDylib.dylib -framework Cocoa -lSystem -macosx_version_min 10.9

bin/SimpleClass\ Dylib.dylib: bin/SimpleClassDylib.dylib
	$(Q) cp bin/SimpleClassDylib.dylib bin/SimpleClass\ Dylib.dylib

bin/SimpleClassStatic.a: obj/SimpleClass.o
	$(Q) xcrun libtool -static obj/SimpleClass.o -o bin/SimpleClassStatic.a

bin/Mobile-dynamic/MobileBinding.dll: bin/SimpleClassDylib.dylib
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_dynamic.csproj

bin/Mobile-dynamic-newstyle/MobileBinding.dll: bin/SimpleClassDylib.dylib
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_dynamic_newstyle.csproj

bin/Mobile-dynamic-spaces/Mobile\ Binding.dll: bin/SimpleClass\ Dylib.dylib
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_dynamic_spaces.csproj

bin/Mobile-static/MobileBinding.dll: bin/SimpleClassStatic.a
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_static.csproj

bin/Mobile-static-newstyle/MobileBinding.dll: bin/SimpleClassStatic.a
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_static_newstyle.csproj


bin/Mobile-framework/MobileBinding.dll:
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_framework.csproj


bin/MobileTest-static/MobileTestApp.app: bin/Mobile-static/MobileBinding.dll
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_static.csproj

bin/MobileTest-static-newstyle/MobileTestApp.app: bin/Mobile-static-newstyle/MobileBinding.dll
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_static_newstyle.csproj

bin/MobileTest-withLinker/MobileTestApp.app: bin/Mobile-static/MobileBinding.dll
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_WithLinker.csproj

bin/MobileTest-dynamic/MobileTestApp.app: bin/Mobile-dynamic/MobileBinding.dll
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_dynamic.csproj

bin/MobileTest-dynamic-newstyle/MobileTestApp.app: bin/Mobile-dynamic-newstyle/MobileBinding.dll
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_dynamic_newstyle.csproj

bin/MobileTest-dynamic-spaces/MobileTestApp.app: bin/Mobile-dynamic-spaces/Mobile\ Binding.dll
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_dynamic_spaces.csproj

bin/MobileTest-framework/MobileTestApp.app: bin/Mobile-framework/MobileBinding.dll
	$(Q) $(SYSTEM_XBUILD) $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_framework.csproj




test-static:: bin/MobileTest-static/MobileTestApp.app bin/MobileTest-static-newstyle/MobileTestApp.app
	$(Q) ./bin/MobileTest-static/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null
	$(Q) ./bin/MobileTest-static-newstyle/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null

test-dynamic:: bin/MobileTest-dynamic/MobileTestApp.app bin/MobileTest-dynamic-newstyle/MobileTestApp.app bin/MobileTest-dynamic-spaces/MobileTestApp.app
	$(Q) ./bin/MobileTest-dynamic/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null
	$(Q) test -e bin/MobileTest-dynamic/MobileTestApp.app/Contents/MonoBundle/SimpleClassDylib.dylib
	$(Q) ./bin/MobileTest-dynamic-newstyle/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null
	$(Q) test -e bin/MobileTest-dynamic-newstyle/MobileTestApp.app/Contents/MonoBundle/SimpleClassDylib.dylib
	$(Q) ./bin/MobileTest-dynamic-spaces/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null
	$(Q) test -e bin/MobileTest-dynamic-spaces/MobileTestApp.app/Contents/MonoBundle/SimpleClass\ Dylib.dylib

test-framework:: bin/MobileTest-framework/MobileTestApp.app
	$(Q) test -e bin/MobileTest-framework/MobileTestApp.app/Contents/Frameworks/iTunesLibrary.framework

test-withLinker:: bin/MobileTest-withLinker/MobileTestApp.app
	$(Q) ./bin/MobileTest-withLinker/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null



all-local:: test-static test-dynamic test-framework test-withLinker

clean-local::
	$(Q) rm -rf ./bin ./obj
