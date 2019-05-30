CONFIGURATION ?= Debug
BCL_TEST_IMPORTER_SRC:=$(wildcard BCLTestImporter/*.cs)

all: BCLTestImporter.exe

BCLTestImporter.exe: $(BCL_TEST_IMPORTER_SRC) Makefile BCLTestImporter/BCLTestImporter.csproj 
	msbuild /restore /p:Configuration=$(CONFIGURATION) BCLTestImporter/BCLTestImporter.sln

clean:
	msbuild /target:Clean BCLTestImporter/BCLTestImporter.sln
	@rm -Rf ./BCLTestImporter/bin
	@rm -Rf ./BCLTestImporter/obj
	@rm bcl-test-importer
