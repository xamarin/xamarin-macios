# Notes:
# PACKAGE is the name of the nuget package
# VERSION is the version that we want
# FULLPACKAGE is the full name of the nuget package when downloaded
# NUGETAPI is the url to the download API for this package
# TOPLEVEL is the top level directory for xamarin-macios
# DONUT is the "correct" version of dotnet to use
# SDKPATH is the path to the SDK from which DONUT came
# XAM_IOS is the path to Xamarin.iOS.dll
# MS_IOS is the path to Microsoft.iOS.dll
# NNYEAH is the path to the nnyeah tool

PACKAGE=sidebarnavigation
VERSION=2.1.0
FULLPACKAGE=$(PACKAGE).$(VERSION).nupkg
NUGETAPI=http://api.nuget.org/v3-flatcontainer

TOPLEVEL := $(shell git rev-parse --show-superproject-working-tree)
TOPLEVEL := $(shell if [ -d $(TOPLEVEL) ]; then git rev-parse --show-toplevel; fi)
DONUT := $(shell grep ^DOTNET= $(TOPLEVEL)/tests/test.config | sed 's/^DOTNET=//')
SDKPATH = $(shell dirname $(DONUT))
XAM_IOS = $(TOPLEVEL)/_ios-build/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/Xamarin.iOS/Xamarin.iOS.dll
MS_IOS = $(shell find $(SDKPATH)/packs/Microsoft.iOS.Ref -name Microsoft.iOS.dll)
NNYEAH=../../../../nnyeah/nnyeah.csproj

all: prepare
	$(DONUT) build

prepare: ./nuget nuget/$(FULLPACKAGE)
	$(DONUT) run --project $(NNYEAH) \
		--input nuget/lib/xamarinios10/SidebarNavigation.dll \
		--output nuget/SidebarNavigation.dll \
		--xamarin-assembly=$(XAM_IOS) \
		--microsoft-assembly=$(MS_IOS) \
		--force-overwrite
nuget:
	mkdir $@

nuget/$(FULLPACKAGE):
	wget $(NUGETAPI)/$(PACKAGE)/$(VERSION)/$(FULLPACKAGE)
	mv $(FULLPACKAGE) nuget/$(FULLPACKAGE)
	unzip nuget/$(FULLPACKAGE) -d nuget

clean:
	rm -rf ./nuget
	@$(DONUT) clean

spotless: clean
	rm -rf bin obj

