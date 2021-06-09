TOP=../../../../../..

include $(TOP)/Make.config

build:
	$(SYSTEM_MSBUILD) *.csproj /bl $(MSBUILD_VERBOSITY)

run:
	./bin/Debug/MyStaticRegistrarReferencesApp.app/Contents/MacOS/MyStaticRegistrarReferencesApp
