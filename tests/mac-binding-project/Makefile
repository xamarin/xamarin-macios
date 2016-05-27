TOP=../../../xamarin-macios
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
	$(Q) xbuild MobileBinding/MobileBinding_dynamic.csproj /verbosity:quiet /nologo

bin/Mobile-dynamic-newstyle/MobileBinding.dll: bin/SimpleClassDylib.dylib
	$(Q) xbuild MobileBinding/MobileBinding_dynamic_newstyle.csproj /verbosity:quiet /nologo

bin/Mobile-dynamic-spaces/Mobile\ Binding.dll: bin/SimpleClass\ Dylib.dylib
	$(Q) xbuild MobileBinding/MobileBinding_dynamic_spaces.csproj /verbosity:quiet /nologo

bin/Mobile-static/MobileBinding.dll: bin/SimpleClassStatic.a
	$(Q) xbuild MobileBinding/MobileBinding_static.csproj /verbosity:quiet /nologo

bin/Mobile-static-newstyle/MobileBinding.dll: bin/SimpleClassStatic.a
	$(Q) xbuild MobileBinding/MobileBinding_static_newstyle.csproj /verbosity:quiet /nologo


bin/Mobile-framework/MobileBinding.dll:
	$(Q) xbuild MobileBinding/MobileBinding_framework.csproj /verbosity:quiet /nologo


bin/MobileTest-static/MobileTestApp.app: bin/Mobile-static/MobileBinding.dll
	$(Q) xbuild MobileTestApp/MobileTestApp_static.csproj /verbosity:quiet /nologo

bin/MobileTest-static-newstyle/MobileTestApp.app: bin/Mobile-static-newstyle/MobileBinding.dll
	$(Q) xbuild MobileTestApp/MobileTestApp_static_newstyle.csproj /verbosity:quiet /nologo

bin/MobileTest-withLinker/MobileTestApp.app: bin/Mobile-static/MobileBinding.dll
	$(Q) xbuild MobileTestApp/MobileTestApp_WithLinker.csproj /verbosity:quiet /nologo

bin/MobileTest-dynamic/MobileTestApp.app: bin/Mobile-dynamic/MobileBinding.dll
	$(Q) xbuild MobileTestApp/MobileTestApp_dynamic.csproj /verbosity:quiet /nologo

bin/MobileTest-dynamic-newstyle/MobileTestApp.app: bin/Mobile-dynamic-newstyle/MobileBinding.dll
	$(Q) xbuild MobileTestApp/MobileTestApp_dynamic_newstyle.csproj /verbosity:quiet /nologo

bin/MobileTest-dynamic-spaces/MobileTestApp.app: bin/Mobile-dynamic-spaces/Mobile\ Binding.dll
	$(Q) xbuild MobileTestApp/MobileTestApp_dynamic_spaces.csproj /verbosity:quiet /nologo

bin/MobileTest-framework/MobileTestApp.app: bin/Mobile-framework/MobileBinding.dll
	$(Q) xbuild MobileTestApp/MobileTestApp_framework.csproj /verbosity:quiet /nologo




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
