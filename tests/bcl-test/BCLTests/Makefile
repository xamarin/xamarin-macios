DESTDIR ?= /usr/local/bin/
CONFIGURATION ?= Debug
MONO_ROOT:=../../../external/mono/

test-importer: 
	@make -C ../../../tools/bcl-test-importer/ all
	@rm -Rf bcl-test-importer
	@mkdir -p bcl-test-importer
	cp ../../../tools/bcl-test-importer/BCLTestImporter/bin/Debug/* bcl-test-importer

generate-test-projects: test-importer ./RegisterType.cs.in ./BCLTests.csproj.in
	mono ./bcl-test-importer/BCLTestImporter.exe --generate-all-projects --output=. --clean
	mono ./bcl-test-importer/BCLTestImporter.exe --generate-all-projects --output=. --override --register-type-template=RegisterType.cs.in --project-template=. --plist-template=. --mono-root=../../../external/mono/ -iOS -v

all: build-test-projects

clean: test-importer
	mono ./bcl-test-importer/BCLTestImporter.exe --generate-all-projects --output=. --clean
	@rm -Rf ./bin
	@rm -Rf ./obj
