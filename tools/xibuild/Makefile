TOP = ../..

include $(TOP)/Make.config

CONFIGURATION=Debug
SRC_FILES=Main.cs $(TOP)/tools/common/StringUtils.cs

all: bin/$(CONFIGURATION)/xibuild.exe

bin/$(CONFIGURATION)/xibuild.exe: xibuild.csproj $(SRC_FILES)
	$(SYSTEM_MSBUILD) "/bl:$@.binlog" /restore xibuild.csproj /p:Configuration=$(CONFIGURATION)

clean-local::
	$(SYSTEM_MSBUILD) /t:Clean xibuild.csproj
