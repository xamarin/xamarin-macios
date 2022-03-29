TOP=..

include $(TOP)/Make.config

ifdef INCLUDE_MAC
SUBDIRS+=mmp
endif

SUBDIRS+=mtouch xibuild mlaunch siminstaller nnyeah

ifdef ENABLE_INSTALL_SOURCE
SUBDIRS += install-source
endif

ifdef ENABLE_DOTNET
SUBDIRS += dotnet-linker
endif
