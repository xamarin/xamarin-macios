TOP=..

include $(TOP)/Make.config

ifdef INCLUDE_MAC
SUBDIRS+=mmp
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

SUBDIRS+=xibuild mlaunch siminstaller nnyeah

ifdef ENABLE_INSTALL_SOURCE
SUBDIRS += install-source
endif

ifdef ENABLE_DOTNET
SUBDIRS += dotnet-linker
endif
