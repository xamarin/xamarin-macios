TOP=../../..
include $(TOP)/Make.config

all-local:: mac/bin/Debug/MacIntegration.dll ios/bin/Debug/iOSIntegration.dll tv/bin/Debug/TVIntegration.dll

install-local:: all-local

mac/bin/Debug/MacIntegration.dll: $(wildcard *.cs */*.cs Makefile) mac/MacIntegration.csproj
	$(Q_BUILD) $(SYSTEM_MSBUILD) mac/MacIntegration.csproj $(MSBUILD_VERBOSITY)

ios/bin/Debug/iOSIntegration.dll: $(wildcard *.cs */*.cs Makefile) ios/iOSIntegration.csproj
	$(Q_BUILD) $(SYSTEM_MSBUILD) ios/iOSIntegration.csproj $(MSBUILD_VERBOSITY)

tv/bin/Debug/TVIntegration.dll: $(wildcard *.cs */*.cs Makefile) tv/TVIntegration.csproj
	$(Q_BUILD) $(SYSTEM_MSBUILD) tv/TVIntegration.csproj $(MSBUILD_VERBOSITY)

clean:
	$(Q_BUILD) $(SYSTEM_MSBUILD) /t:Clean mac/MacIntegration.csproj $(MSBUILD_VERBOSITY)
	$(Q_BUILD) $(SYSTEM_MSBUILD) /t:Clean ios/iOSIntegration.csproj $(MSBUILD_VERBOSITY)
	$(Q_BUILD) $(SYSTEM_MSBUILD) /t:Clean tv/TVIntegration.csproj $(MSBUILD_VERBOSITY)
