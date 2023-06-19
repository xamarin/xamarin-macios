TOP=../../xamarin-macios
include $(TOP)/Make.config

IOS_PACKAGE_DESTDIR=$(abspath .)/_ios-install
MAC_PACKAGE_DESTDIR=$(abspath .)/_mac-install

IOS_PAYLOAD=$(abspath $(IOS_PACKAGE_DESTDIR))
MAC_PAYLOAD=$(abspath $(MAC_PACKAGE_DESTDIR))

MAC_INSTALL_PKG=xamarin.mac-$(MAC_PACKAGE_VERSION).pkg
MAC_UNINSTALL_PKG=xamarin.mac-uninstall-$(MAC_PACKAGE_VERSION).pkg

MAC_SED_PROCESS_IN = sed \
	-e "s/@INSTALLKBYTES@/`du -sk $(MAC_PAYLOAD) | awk '{print $$1}'`/g" \
	-e "s/@NUMFILES@/`lsbom mac-tmp-package/$(MAC_INSTALL_PKG)/Bom 2>/dev/null | wc -l | awk '{print $$1}'`/g" \
	-e "s/@INSTALL_PKG@/$(MAC_INSTALL_PKG)/g" \
	-e "s/@UNINSTALL_PKG@/$(MAC_UNINSTALL_PKG)/g" \
	-e "s,@PACKAGE_INSTALL_LOCATION@,$(MAC_PACKAGE_INSTALL_LOCATION),g" \
	-e "s/@PACKAGE_VERSION@/$(MAC_PACKAGE_VERSION)/g" \
	-e "s/@PRODUCT@/$(MAC_PRODUCT)/g" \
	-e "s,@XAMMAC_PREFIX@,$(MAC_FRAMEWORK_CURRENT_DIR),g"

ifdef INCLUDE_MAC
ALL_PACKAGES+=$(MAC_PACKAGE_FILENAME)
endif
ifdef INCLUDE_IOS
ALL_PACKAGES+=$(IOS_PACKAGE_FILENAME)
endif

build-for-package:
	@# make sure we start with a clean slate
	$(Q) git clean -xfd
	$(Q) $(MAKE) -C $(TOP) install USE_SOURCE_LINKS=0 DEVMODE=0  \
		IOS_DESTDIR=$(IOS_PACKAGE_DESTDIR) IOS_TARGETDIR=/ IOS_INSTALL_VERSION=$(IOS_PACKAGE_VERSION) \
		MAC_DESTDIR=$(MAC_PACKAGE_DESTDIR) MAC_TARGETDIR=/ MAC_INSTALL_VERSION=$(MAC_PACKAGE_VERSION)

release package:
	$(Q) $(MAKE) build-for-package
	$(Q) $(MAKE) $(ALL_PACKAGES)
	@# updateinfo, uploaded to wrench
ifdef INCLUDE_MAC
	$(Q) cp $(MAC_PACKAGE_DESTDIR)/$(MAC_FRAMEWORK_DIR)/Versions/Current/updateinfo mac-updateinfo
endif
ifdef INCLUDE_IOS
	$(Q) cp $(IOS_PACKAGE_DESTDIR)/$(IOS_FRAMEWORK_DIR)/Versions/Current/updateinfo ios-updateinfo
endif
	$(MAKE) bundle.zip msbuild.zip

# this are just a useful targets to run locally if you're not interested in the XI/XM package
# it should be almost identical to the normal 'package' target (only difference should be that the XM/XI
# bits aren't built/packaged)
ifdef INCLUDE_MAC
mac-package:
	$(Q) $(MAKE) build-for-package
	$(Q) $(MAKE) $(MAC_PACKAGE_FILENAME)
endif
ifdef INCLUDE_IOS
ios-package:
	$(Q) $(MAKE) build-for-package
	$(Q) $(MAKE) $(IOS_PACKAGE_FILENAME)
endif

prechecks:
ifdef CROSS_BUILD_CFLAGS
	echo "CROSS_BUILD_CFLAGS can't be set when building a package. Please fix Make.local.config" && exit 1
endif
ifdef DEVICE_BUILD_CFLAGS
	echo "DEVICE_BUILD_CFLAGS can't be set when building a package. Please fix Make.local.config" && exit 1
endif
	@echo Package prechecks completed successfully.

monotouch.pkg: Makefile
	@echo Building $@
	$(Q) $(MAKE) prechecks
	$(Q) mkdir -p ios-work/commercial
	$(Q) cp postflight-ios ios-work/commercial/postinstall
	$(Q) chmod a+x ios-work/commercial/postinstall
	$(Q) mkdir -p ios-tmp-package/Resources/en.lproj
	$(Q) mkdir -p ios-tmp-package/monotouch.pkg
	$(Q) cp $(TOP)/tools/scripts/License.txt ios-tmp-package/Resources/en.lproj/License.txt
	$(Q) mkbom $(IOS_PAYLOAD) ios-tmp-package/monotouch.pkg/Bom
	$(Q) (cd $(IOS_PAYLOAD); lsbom -p f $(abspath ./ios-tmp-package/monotouch.pkg/Bom) | cpio -o | gzip -9 -n > $(abspath ios-tmp-package/monotouch.pkg/Payload)) 2>/dev/null
	$(Q) (cd ios-work/commercial; ls -d . ./postinstall | cpio -o | gzip -9 -n > ../../ios-tmp-package/monotouch.pkg/Scripts) 2>/dev/null
	$(Q) INSTALLKBYTES=`du -sk $(IOS_PAYLOAD) | awk '{print $$1}'`; NUMFILES=`lsbom ios-tmp-package/monotouch.pkg/Bom | wc -l | awk '{print $$1}'`; sed -e "s/@INSTALLKBYTES@/$$INSTALLKBYTES/g" -e "s/@NUMFILES@/$$NUMFILES/g" -e "s/@FULL_PACKAGE_UTI@/$(IOS_PACKAGE_UTI)/g" -e "s,@MONOTOUCH_PREFIX@,$(MONOTOUCH_PREFIX),g" PackageInfo.ios.in > ios-tmp-package/monotouch.pkg/PackageInfo
	@echo Built $@

$(IOS_PACKAGE_FILENAME): delete-xma-build-host.pkg monotouch.pkg
	@echo Building $@
	@rm -Rf ios-meta-package
	@cp -R ios-tmp-package ios-meta-package
	@cp -R xma-tmp-package/delete-xma-build-host.pkg ios-meta-package
	@$(Q) INSTALLBYTES_MTVS=`grep installKBytes ios-meta-package/delete-xma-build-host.pkg/PackageInfo | sed 's/.*installKBytes="\([0-9]*\)".*/\1/'` INSTALLKBYTES=`du -sk $(IOS_PAYLOAD) | awk '{print $$1}'`; sed -e s/@INSTALLKBYTES@/$$INSTALLKBYTES/ -e s/@INSTALLBYTES_MTVS@/$$INSTALLBYTES_MTVS/ Distribution.ios.in > ios-meta-package/Distribution
	@$(Q_GEN) (cd ios-meta-package; xar --no-compress monotouch.pkg/Payload --no-compress monotouch.pkg/Scripts --no-compress delete-xma-build-host.pkg/Payload --no-compress delete-xma-build-host.pkg/Scripts -c -f ../$@ *)
	@echo Built $@

# delete-xma-build-host is a package with the same id as the old MTVS installer, but with 0 files to install.
# This makes OSX remove all files installed by the old MTVS installer.
delete-xma-build-host.pkg:
	@echo Building $@
	$(Q) rm -rf xma-work xma-tmp-package
	$(Q) mkdir -p xma-work/commercial/payload
	$(Q) cp preinstall.xma.in xma-work/commercial/preinstall
	$(Q) chmod a+x xma-work/commercial/preinstall
	$(Q) mkdir -p xma-tmp-package/$@
	$(Q) mkbom xma-work/commercial/payload xma-tmp-package/$@/Bom
	$(Q) (cd xma-work/commercial/payload; lsbom -p f $(abspath ./xma-tmp-package/$@/Bom) | cpio -o | gzip -9 -n > $(abspath xma-tmp-package/$@/Payload)) 2>/dev/null
	$(Q) (cd xma-work/commercial; ls -d . ./preinstall | cpio -o | gzip -9 -n > ../../xma-tmp-package/$@/Scripts) 2>/dev/null
	$(Q) cp PackageInfo.xma.in xma-tmp-package/$@/PackageInfo
	$(Q) cp Distribution.xma.in xma-tmp-package/Distribution
	$(Q) (cd xma-tmp-package; xar --no-compress $@/Payload --no-compress $@/Scripts -c -f ../$@ *)
	@echo Built $@

$(MAC_PACKAGE_FILENAME):
	@echo Building $@
	$(Q) $(MAKE) prechecks
# xamarin.mac-v.v.v.pkg meta package
	$(Q) mkdir -p mac-work/commercial
	$(Q) cp postflight-mac mac-work/commercial/postflight-mac
	$(Q) chmod a+x mac-work/commercial/postflight-mac
	$(Q) mkdir -p mac-tmp-package/$(MAC_INSTALL_PKG)
	$(Q) mkdir -p mac-tmp-package/Resources/en.lproj
	$(Q) cp $(TOP)/tools/scripts/License.txt mac-tmp-package/Resources/en.lproj/License.txt
	$(Q) mkbom $(MAC_PAYLOAD) mac-tmp-package/$(MAC_INSTALL_PKG)/Bom
	$(Q) (cd $(MAC_PAYLOAD); lsbom -p f $(abspath ./mac-tmp-package/$(MAC_INSTALL_PKG)/Bom) | cpio -o | gzip -9 -n > $(abspath ./mac-tmp-package/$(MAC_INSTALL_PKG)/Payload)) 2>/dev/null
	$(Q) (cd mac-work/commercial; ls -d . ./postflight-mac | cpio -o | gzip -9 -n > ../../mac-tmp-package/$(MAC_INSTALL_PKG)/Scripts) 2>/dev/null
	$(Q) ls -d ./mono-check.sh | cpio -o | gzip -9 -n > mac-tmp-package/Scripts 2>/dev/null
	$(Q) $(MAC_SED_PROCESS_IN) PackageInfo.mac.in > mac-tmp-package/$(MAC_INSTALL_PKG)/PackageInfo
# xamarin.mac-uninstall-v.v.v.pkg meta package
	$(Q) mkdir -p mac-work-uninstall/commercial
	$(Q) $(MAC_SED_PROCESS_IN) uninstall.in > mac-work-uninstall/commercial/uninstall
	$(Q) chmod a+x mac-work-uninstall/commercial/uninstall
	$(Q) mkdir -p mac-work-uninstall/payload
	$(Q) mkdir -p mac-tmp-package/$(MAC_UNINSTALL_PKG)
	$(Q) mkbom mac-work-uninstall/payload mac-tmp-package/$(MAC_UNINSTALL_PKG)/Bom
	$(Q) (cd mac-work-uninstall/payload; lsbom -p f ../../mac-tmp-package/$(MAC_UNINSTALL_PKG)/Bom | cpio -o | gzip -9 -n > ../../mac-tmp-package/$(MAC_UNINSTALL_PKG)/Payload) 2>/dev/null
	$(Q) (cd mac-work-uninstall/commercial; ls -d . ./uninstall | cpio -o | gzip -9 -n > ../../mac-tmp-package/$(MAC_UNINSTALL_PKG)/Scripts) 2>/dev/null
	$(Q) $(MAC_SED_PROCESS_IN) UninstallPackageInfo.mac.in > mac-tmp-package/$(MAC_UNINSTALL_PKG)/PackageInfo
# final package
	$(Q) $(MAC_SED_PROCESS_IN) Distribution.mac.in > mac-tmp-package/Distribution
	$(Q_GEN) (cd mac-tmp-package; xar --no-compress Scripts --no-compress $(MAC_INSTALL_PKG)/Payload --no-compress $(MAC_INSTALL_PKG)/Scripts --no-compress $(MAC_UNINSTALL_PKG)/Payload --no-compress $(MAC_UNINSTALL_PKG)/Scripts -c -f ../$(MAC_PACKAGE_FILENAME) *)
	@echo Built $@

clean-local::
	rm -rf ios-tmp-package ios-work ios-meta-package
	rm -rf mac-tmp-package mac-work mac-work-uninstall
	rm -rf *.pkg

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
ifdef INCLUDE_WATCH
	mv -f msbuild/iOS/*WatchOS*.* msbuild/WatchOS
	mv -f msbuild/iOS/NoCode.cs msbuild/WatchOS
endif
	mv -f msbuild/iOS/*TVOS*.* msbuild/TVOS
	cd msbuild && zip -9 -r $(abspath $@) .
	rm -rf msbuild

bundle.zip: Version Version.rev
	rm -f $@
ifdef INCLUDE_XAMARIN_LEGACY
	if test -d $(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono; then cd $(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/lib/mono     && zip -9 -r $(CURDIR)/bundle.zip .; fi
	if test -d $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono; then cd $(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono     && zip -9 -r $(CURDIR)/bundle.zip .; fi
endif
ifdef ENABLE_DOTNET
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
endif
	zip -9 $@ Version Version.rev

Version:
	$(Q) echo "$(IOS_PACKAGE_VERSION_MAJOR).$(IOS_PACKAGE_VERSION_MINOR).$(IOS_PACKAGE_VERSION_REV)" > $@

Version.rev:
	$(Q) echo "$(IOS_PACKAGE_VERSION_BUILD)" > $@

