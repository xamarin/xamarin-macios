TOP=..

include $(TOP)/Make.config

SUBDIRS += devops

ifdef INCLUDE_MAC
SUBDIRS+=mmp
endif

ifdef INCLUDE_IOS
INCLUDE_MTOUCH=1
endif
ifdef INCLUDE_TVOS
INCLUDE_MTOUCH=1
endif
ifdef INCLUDE_MACCATALYST
INCLUDE_MTOUCH=1
endif

ifdef INCLUDE_MTOUCH
SUBDIRS += mtouch
endif

SUBDIRS+=mlaunch siminstaller

SUBDIRS += dotnet-linker
