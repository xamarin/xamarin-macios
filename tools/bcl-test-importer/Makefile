DESTDIR ?= /usr/local/bin/
CONFIGURATION ?= Debug
BCL_TEST_IMPORTER_SRC:=BCLTestImporter/ApplicationOptions.cs \
					BCLTestImporter/BCLTestImporter.csproj \
					BCLTestImporter/ConsoleWriter.cs \
					BCLTestImporter/Program.cs \
					BCLTestImporter/BCLTestProjectGenerator.cs \
					BCLTestImporter/RegisterTypeGenerator.cs \
					BCLTestImporter/BCLTestAssemblyDefinition.cs \
					BCLTestImporter/BCLTestProjectDefinition.cs \

all: BCLTestImporter.exe

BCLTestImporter.exe: $(BCL_TEST_IMPORTER_SRC) Makefile 
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
