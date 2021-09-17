TOP=..
SUBDIRS=mmp mtouch xibuild mlaunch siminstaller

include $(TOP)/Make.config

ifdef ENABLE_INSTALL_SOURCE
SUBDIRS += install-source
endif

ifdef ENABLE_DOTNET
SUBDIRS += dotnet-linker
endif
