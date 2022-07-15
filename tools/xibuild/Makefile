TOP = ../..

include $(TOP)/Make.config

CONFIGURATION=Debug
SRC_FILES=Main.cs $(TOP)/tools/common/StringUtils.cs

all: bin/$(CONFIGURATION)/$(DOTNET_TFM)/xibuild.dll

bin/$(CONFIGURATION)/$(DOTNET_TFM)/xibuild.dll: xibuild.csproj $(SRC_FILES)
	$(Q_GEN) $(DOTNET) build xibuild.csproj /p:Configuration=$(CONFIGURATION) $(DOTNET_BUILD_VERBOSITY)

clean-local::
	$(SYSTEM_MSBUILD) /t:Clean xibuild.csproj
