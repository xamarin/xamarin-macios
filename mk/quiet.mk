Q=$(if $(V),,@)
# echo -e "\\t" does not work on some systems, so use 5 spaces
Q_GEN=  $(if $(V),,@echo "GEN      $(@F)";)
QF_GEN= $(if $(V),,@echo "GEN      $@";)
Q_LN=   $(if $(V),,@echo "LN       $(@F)";)
QF_LN=  $(if $(V),,@echo "LN       $@";)
Q_MCS=  $(if $(V),,@echo "MCS      $(@F)";)
Q_CSC=  $(if $(V),,@echo "CSC      $(@F)";)
Q_GMCS= $(if $(V),,@echo "GMCS     $(@F)";)
Q_DMCS= $(if $(V),,@echo "DMCS     $(@F)";)
Q_SMCS= $(if $(V),,@echo "SMCS     $(@F)";)
Q_PMCS= $(if $(V),,@echo "PMCS     $(@F)";)
Q_AS=   $(if $(V),,@echo "AS       $(@F)";)
Q_CC=   $(if $(V),,@echo "CC       $(@F)";)
QT_CC=  $(if $(V),,@echo "CC       $$(@F)";)
Q_CXX=  $(if $(V),,@echo "CXX      $(@F)";)
Q_CCLD= $(if $(V),,@echo "CCLD     $(@F)";)
Q_OBJC= $(if $(V),,@echo "OBJC     $(@F)";)
QT_OBJC=$(if $(V),,@echo "OBJC     $$(@F)";)
Q_STRIP=$(if $(V),,@echo "STRIP    $(@F)";)
Q_CP=   $(if $(V),,@echo "CP       $(@F)";)
Q_AR=   $(if $(V),,@echo "AR       $(@F)";)
QT_AR=   $(if $(V),,@echo "AR      $$(@F)";)
Q_LIPO= $(if $(V),,@echo "LIPO     $(@F)";)
QT_LIPO= $(if $(V),,@echo "LIPO    $$(@F)";)
Q_MDB=  $(if $(V),,@echo "MDB      $(@F)";)
Q_NUNIT= $(if $(V),,@echo "NUNIT     $(@F)";)

Q_SN=   $(if $(V),,@echo "SN       $(@F)";)
Q_XBUILD=$(if $(V),,@echo "XBUILD  $(@F)";)
Q_TT=   $(if $(V),,@echo "TT       $(@F)";)
Q_BUILD=$(if $(V),,@echo "BUILD    $(@F)";)

Q_PROF_MCS =  $(if $(V),,@echo "MCS      [$(1)] $(@F)";)
Q_PROF_CSC =  $(if $(V),,@echo "CSC      [$(1)] $(@F)";)
Q_PROF_GEN  = $(if $(V),,@echo "GEN      [$(1)] $(@F)";)
Q_PROF_SN   = $(if $(V),,@echo "SN       [$(1)] $(@F)";)
Q_1 = $(if $(V),,@echo "$(1) $(@F)";)
Q_2 = $(if $(V),,@echo "$(1) $(2) $(@F)";)

ASCOMPILE   = $(Q_AS) $(AS)
CSCOMPILE   = $(Q_MCS) $(MCS)
CCOMPILE    = $(Q_CC) $(CC)
CXXCOMPILE  = $(Q_CC) $(CXX)
OBJCCOMPILE = $(Q_OBJC) $(CC)

DEVICE_OBJCCOMPILE = $(Q_OBJC) $(DEVICE_CC)
DEVICE_OBJCTCOMPILE = $(QT_OBJC) $(DEVICE_CC)
DEVICE_CCOMPILE = $(Q_CC) $(DEVICE_CC)
DEVICE_CTCOMPILE = $(QT_CC) $(DEVICE_CC)

SIMULATOR_OBJCCOMPILE = $(Q_OBJC) $(SIMULATOR_CC)
SIMULATOR_OBJCTCOMPILE = $(QT_OBJC) $(SIMULATOR_CC)
SIMULATOR_CCOMPILE = $(Q_CC) $(SIMULATOR_CC)
SIMULATOR_CTCOMPILE = $(QT_CC) $(SIMULATOR_CC)
SIMULATOR_ASCOMPILE = $(Q_AS) $(SIMULATOR_CC)

ifeq ($(V),)
ifeq ($(BUILD_REVISION)$(JENKINS_HOME),)
# non-verbose local build
XBUILD_VERBOSITY=/nologo /verbosity:quiet
XBUILD_VERBOSITY_QUIET=/nologo /verbosity:quiet
MMP_VERBOSITY=-q
MTOUCH_VERBOSITY=-q
MDTOOL_VERBOSITY=
else
# wrench build
XBUILD_VERBOSITY=/nologo /verbosity:normal
XBUILD_VERBOSITY_QUIET=/nologo /verbosity:quiet
MMP_VERBOSITY=-vvvv
MTOUCH_VERBOSITY=-vvvv
MDTOOL_VERBOSITY=-v -v -v -v
endif
else
# verbose build
XBUILD_VERBOSITY=/verbosity:diagnostic
XBUILD_VERBOSITY_QUIET=/verbosity:diagnostic
MMP_VERBOSITY=-vvvv
MTOUCH_VERBOSITY=-vvvv
MDTOOL_VERBOSITY=-v -v -v -v
endif
MSBUILD_VERBOSITY=$(XBUILD_VERBOSITY)
MSBUILD_VERBOSITY_QUIET=$(XBUILD_VERBOSITY_QUIET)
XIBUILD_VERBOSITY=$(XBUILD_VERBOSITY)
