TOP=..

include $(TOP)/Make.config

ifdef INCLUDE_MAC
ifdef INCLUDE_XAMARIN_LEGACY
SUBDIRS+=mmp
endif
endif

ifdef INCLUDE_XAMARIN_LEGACY
SUBDIRS += mtouch
endif

ifdef INCLUDE_XAMARIN_LEGACY
SUBDIRS += xibuild
endif

ifdef INCLUDE_IOS
INCLUDE_MTOUCH=1
endif
ifdef INCLUDE_TVOS
INCLUDE_MTOUCH=1
endif
ifdef INCLUDE_WATCH
INCLUDE_MTOUCH=1
endif

ifdef INCLUDE_MTOUCH
SUBDIRS += mtouch
endif

SUBDIRS+=mlaunch siminstaller

ifdef ENABLE_DOTNET
ifdef INCLUDE_XAMARIN_LEGACY
SUBDIRS += nnyeah
endif
endif

ifdef INCLUDE_XAMARIN_LEGACY
ifdef ENABLE_INSTALL_SOURCE
SUBDIRS += install-source
endif
endif

ifdef ENABLE_DOTNET
SUBDIRS += dotnet-linker
endif
