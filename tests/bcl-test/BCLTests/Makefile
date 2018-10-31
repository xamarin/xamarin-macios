DESTDIR ?= /usr/local/bin/
CONFIGURATION ?= Debug
MONO_ROOT:=../../../external/mono/

test-importer:
	make -C ../../../tools/bcl-test-importer/ bcl-test-importer
	cp ../../../tools/bcl-test-importer/bcl-test-importer .

generate-test-projects: test-importer ./RegisterType.cs.in ./BCLTests.csproj.in
	./bcl-test-importer --generate-all-projects --output=. --clean
	./bcl-test-importer --generate-all-projects --output=. --override --register-type-template=RegisterType.cs.in --project-template=. --plist-template=. --mono-root=../../../external/mono/ -iOS -v

all: build-test-projects

clean: test-importer
	./bcl-test-importer --generate-all-projects --output=. --clean
	@rm -Rf ./bin
	@rm -Rf ./obj
	@rm -f bcl-test-importer
