DESTDIR ?= /usr/local/bin/
CONFIGURATION ?= Debug

all: BCLTestImporter.exe

BCLTestImporter.exe: BCLTestImporter/BCLTestImporter.sln BCLTestImporter/Program.cs BCLTestImporter/RegisterTypeGenerator.cs BCLTestImporter/ApplicationOptions.cs BCLTestImporter/ProjectGenerator.cs Makefile 
	nuget restore BCLTestImporter/BCLTestImporter.sln
	msbuild /p:Configuration=$(CONFIGURATION) BCLTestImporter/BCLTestImporter.sln

bcl-test-importer: BCLTestImporter.exe
	mkbundle --simple ./BCLTestImporter/bin/$(CONFIGURATION)/BCLTestImporter.exe -o $@ -L ./BCLTestImporter/bin/$(CONFIGURATION)

install: bcl-test-importer
	install -d $(DESTDIR)
	cp BCLTestImporter $(DESTDIR)

clean:
	msbuild /target:Clean BCLTestImporter/BCLTestImporter.sln
	@rm -Rf ./BCLTestImporter/bin
	@rm -Rf ./BCLTestImporter/obj
	@rm bcl-test-importer
