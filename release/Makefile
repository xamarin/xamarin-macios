TOP=../../xamarin-macios
include $(TOP)/Make.config

release package:
	$(MAKE) bundle.zip msbuild.zip

# msbuild.zip and bundle.zip are uploaded to wrench and packaged into the VS plugin
msbuild.zip:
	rm -Rf msbuild $@
	mkdir -p msbuild
	mkdir -p msbuild/iOS
	mkdir -p msbuild/WatchOS
	mkdir -p msbuild/TVOS
	mkdir -p msbuild/Mac
	mkdir -p msbuild/maccore/src
	mkdir -p msbuild/maccore/tools/mtouch
	mkdir -p msbuild/maccore/msbuild/Xamarin.ObjcBinding.Tasks
ifdef INCLUDE_IOS
	cp -aL $(TOP)/src/Constants.iOS.cs.in msbuild/maccore/src/Constants.cs
endif
	$(SYSTEM_MSBUILD) $(TOP)/msbuild/Xamarin.iOS.Tasks/Xamarin.iOS.Tasks.csproj -r /p:Configuration=Release /p:"IncludeMSBuildAssets=all"
	cp -R $(TOP)/msbuild/Xamarin.iOS.Tasks/bin/Release/netstandard2.0/ msbuild/iOS
	$(SYSTEM_MSBUILD) $(TOP)/msbuild/Xamarin.iOS.Tasks.Windows/Xamarin.iOS.Tasks.Windows.csproj -r /p:Configuration=Release
	cp -R $(TOP)/msbuild/Xamarin.iOS.Tasks.Windows/bin/Release/netstandard2.0/win/ msbuild/iOS
	$(SYSTEM_MSBUILD) $(TOP)/msbuild/Xamarin.Mac.Tasks/Xamarin.Mac.Tasks.csproj -r /p:Configuration=Release
	cp -R $(TOP)/msbuild/Xamarin.Mac.Tasks/bin/Release/netstandard2.0/ msbuild/Mac
	mv -f msbuild/iOS/*TVOS*.* msbuild/TVOS
	cd msbuild && zip -9 -r $(abspath $@) .
	rm -rf msbuild

bundle.zip: Version Version.rev
	rm -f $@
	$(foreach platform,$(DOTNET_PLATFORMS),export $(platform)_NUGET_REF_NAME=$($(platform)_NUGET_REF_NAME);) \
	for platform in $(DOTNET_PLATFORMS); do \
		cd $(CURDIR); \
		rm -rf tmpdir; \
		mkdir -p Microsoft.$$platform.Ref/; \
		TMPVAR=$${platform}_NUGET_REF_NAME; \
		$(CP) -r $(DOTNET_DESTDIR)/$${!TMPVAR}/ref Microsoft.$$platform.Ref; \
		cd tmpdir; \
		zip -9 -r $(CURDIR)/bundle.zip ./Microsoft.$$platform.Ref/ref/; \
		rm -rf tmpdir; \
	done
	zip -9 $@ Version Version.rev

Version:
	$(Q) echo "$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)" > $@

Version.rev:
	$(Q) echo "$(IOS_PACKAGE_VERSION_BUILD)" > $@

