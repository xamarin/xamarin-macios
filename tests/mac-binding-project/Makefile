TOP=../..
include $(TOP)/Make.config

export TargetFrameworkFallbackSearchPaths=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild-frameworks
export MSBuildExtensionsPathFallbackPathsOverride=$(MAC_DESTDIR)/Library/Frameworks/Mono.framework/External/xbuild
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current
export XAMMAC_FRAMEWORK_PATH=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

# VSfM sets XCODE_DEVELOPER_DIR_PATH, which confuses the command-line tools if it doesn't match xcode-select, so just unset it.
unexport XCODE_DEVELOPER_DIR_PATH

bin:
	$(Q) mkdir -p bin

bin/SimpleClassDylib.dylib:  bin
	$(Q) xcrun clang -shared ../common/mac/SimpleClass.m -o bin/SimpleClassDylib.dylib -std=gnu99 -mmacosx-version-min=$(MIN_MACOS_SDK_VERSION) -framework Cocoa -lSystem

bin/SimpleClass\ Dylib.dylib: bin/SimpleClassDylib.dylib
	$(Q) cp bin/SimpleClassDylib.dylib bin/SimpleClass\ Dylib.dylib

bin/SimpleClass.%.a: ../common/mac/SimpleClass.m bin
	$(Q) clang -c $< -o bin/SimpleClass.$*.o -std=gnu99 -mmacosx-version-min=$(MIN_MACOS_SDK_VERSION) -arch $*
	$(Q) xcrun ar -rcs $@ bin/SimpleClass.$*.o

bin/SimpleClassStatic.a: bin/SimpleClass.x86_64.a | bin
	$(Q) $(CP) $< $@

bin/Mobile-dynamic/MobileBinding.dll: bin/SimpleClassDylib.dylib
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_dynamic.csproj

bin/Mobile-dynamic-newstyle/MobileBinding.dll: bin/SimpleClassDylib.dylib
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_dynamic_newstyle.csproj

bin/Mobile-dynamic-spaces/Mobile\ Binding.dll: bin/SimpleClass\ Dylib.dylib
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_dynamic_spaces.csproj

bin/Mobile-static/MobileBinding.dll: bin/SimpleClassStatic.a
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_static.csproj

bin/Mobile-static-newstyle/MobileBinding.dll: bin/SimpleClassStatic.a
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_static_newstyle.csproj


bin/Mobile-framework/MobileBinding.dll:
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileBinding/MobileBinding_framework.csproj


bin/MobileTest-static/MobileTestApp.app/Contents/MacOS/MobileTestApp: bin/Mobile-static/MobileBinding.dll
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_static.csproj

bin/MobileTest-static-newstyle/MobileTestApp.app/Contents/MacOS/MobileTestApp: bin/Mobile-static-newstyle/MobileBinding.dll
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_static_newstyle.csproj

bin/MobileTest-withLinker/MobileTestApp.app/Contents/MacOS/MobileTestApp: bin/Mobile-static/MobileBinding.dll
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_WithLinker.csproj

bin/MobileTest-dynamic/MobileTestApp.app/Contents/MacOS/MobileTestApp: bin/Mobile-dynamic/MobileBinding.dll
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_dynamic.csproj

bin/MobileTest-dynamic-newstyle/MobileTestApp.app/Contents/MacOS/MobileTestApp: bin/Mobile-dynamic-newstyle/MobileBinding.dll
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_dynamic_newstyle.csproj

bin/MobileTest-dynamic-spaces/MobileTestApp.app/Contents/MacOS/MobileTestApp: bin/Mobile-dynamic-spaces/Mobile\ Binding.dll
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_dynamic_spaces.csproj

bin/MobileTest-framework/MobileTestApp.app/Contents/MacOS/MobileTestApp: bin/Mobile-framework/MobileBinding.dll
	$(Q) $(SYSTEM_XIBUILD) -- $(XBUILD_VERBOSITY) MobileTestApp/MobileTestApp_framework.csproj




test-static:: bin/MobileTest-static/MobileTestApp.app/Contents/MacOS/MobileTestApp bin/MobileTest-static-newstyle/MobileTestApp.app/Contents/MacOS/MobileTestApp
	$(Q) ./bin/MobileTest-static/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null
	$(Q) ./bin/MobileTest-static-newstyle/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null

test-dynamic:: bin/MobileTest-dynamic/MobileTestApp.app/Contents/MacOS/MobileTestApp bin/MobileTest-dynamic-newstyle/MobileTestApp.app/Contents/MacOS/MobileTestApp bin/MobileTest-dynamic-spaces/MobileTestApp.app/Contents/MacOS/MobileTestApp
	$(Q) ./bin/MobileTest-dynamic/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null
	$(Q) test -e bin/MobileTest-dynamic/MobileTestApp.app/Contents/MonoBundle/SimpleClassDylib.dylib
	$(Q) ./bin/MobileTest-dynamic-newstyle/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null
	$(Q) test -e bin/MobileTest-dynamic-newstyle/MobileTestApp.app/Contents/MonoBundle/SimpleClassDylib.dylib
	$(Q) ./bin/MobileTest-dynamic-spaces/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null
	$(Q) test -e bin/MobileTest-dynamic-spaces/MobileTestApp.app/Contents/MonoBundle/SimpleClass\ Dylib.dylib

test-framework:: bin/MobileTest-framework/MobileTestApp.app/Contents/MacOS/MobileTestApp
	$(Q) test -e bin/MobileTest-framework/MobileTestApp.app/Contents/Frameworks/iTunesLibrary.framework

test-withLinker:: bin/MobileTest-withLinker/MobileTestApp.app/Contents/MacOS/MobileTestApp
	$(Q) ./bin/MobileTest-withLinker/MobileTestApp.app/Contents/MacOS/MobileTestApp 2>&1 | grep 42 > /dev/null



all-local:: test-static test-dynamic test-framework test-withLinker

clean-local::
	$(Q) rm -rf ./bin ./obj
