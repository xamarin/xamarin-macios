DESTDIR ?= /usr/local/bin/
CONFIGURATION ?= Debug
MONO_ROOT:=../../../external/mono/

test-importer:
	make -C ../../../tools/bcl-test-importer/ bcl-test-importer
	cp ../../../tools/bcl-test-importer/bcl-test-importer .

check-test-assemblies:
	for p in ${TEST_PROJECTS}; do \
		v="$$p"_VALUE; \
		assemblies=$$(printenv "$$v"); \
		for a in $$assemblies; do\
			if [[ "$$present_dlls" == "" ]] ; then \
				present_dlls="$$a"; \
			else \
				present_dlls="$$present_dlls $$a"; \
			fi \
		done; \
	done; \
	for p in ${COMMON_IGNORED_TEST_ASSEMBLIES}; do \
		present_dlls="$$present_dlls $$p"; \
	done; \
	present_dlls=$$(echo "$$present_dlls" | tr ' ' '\n' | sort | tr '\n' ' '); \
	mono_dlls=$$(ls ${MONO_ROOT}/mcs/class/lib/monotouch/tests | grep _test.dll | tr "\n" " " | sort); \
	if [[ "$$mono_dlls" != "$$present_dlls" ]] ; then \
		echo "Not all test assemblies have been added or ignored"; \
		echo "Mono assebmlies are:"; \
		echo "$$mono_dlls"; \
		echo "Added assemblies are:"; \
		echo "$$present_dlls"; \
		exit 1; \
	fi

generate-test-projects: test-importer ./RegisterType.cs.in ./BCLTests.csproj.in
	./bcl-test-importer --generate-all-projects --output=. --clean
	./bcl-test-importer --generate-all-projects --output=. --override --register-type-template=./RegisterType.cs.in --project-template=BCLTests.csproj.in --mono-root=../../../external/mono/ --platform=iOS -v

build-test-projects: generate-test-projects
	for p in ${TEST_PROJECTS}; do \
		nuget restore $$p.csproj ; \
		msbuild /p:Configuration=$(CONFIGURATION) $$p.csproj ; \
	done

all: build-test-projects

clean:
	./bcl-test-importer --generate-all-projects --output=. --clean
	@rm -Rf ./bin
	@rm -Rf ./obj
	@rm -f bcl-test-importer
