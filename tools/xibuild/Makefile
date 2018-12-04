TOP = ../..

include $(TOP)/Make.config

CONFIGURATION=Debug
SRC_FILES=Main.cs \
	../../external/mono/mcs/class/Mono.Options/Mono.Options/Options.cs

all: bin/$(CONFIGURATION)/xibuild.exe

bin/$(CONFIGURATION)/xibuild.exe: xibuild.csproj $(SRC_FILES)
	$(SYSTEM_MSBUILD) xibuild.csproj /p:Configuration=$(CONFIGURATION)

clean-local::
	$(SYSTEM_MSBUILD) /t:Clean xibuild.csproj
