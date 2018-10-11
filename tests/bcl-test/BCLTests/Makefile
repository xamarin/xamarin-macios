DESTDIR ?= /usr/local/bin/
CONFIGURATION ?= Debug
MONO_ROOT:=../../../external/mono/
# similar to a dict, we create diff projects, then add the assemblies we want for each of the projects
# that way, we can match a project with more than one assembly since some test assemblies are small
# and we reduce the compilation
TEST_PROJECTS:=first bar baz poo
export first_VALUE:=MONOTOUCH_System.Xml_test.dll MONOTOUCH_System.Xml.Linq_test.dll
export bar_VALUE:=MONOTOUCH_System.Transactions_test.dll
export baz_VALUE:=MONOTOUCH_System.Threading.Tasks.Dataflow_test.dll
export poo_VALUE:=MONOTOUCH_System.ServiceModel_test.dll MONOTOUCH_System.ServiceModel.Web_test.dll

test-importer:
	make -C ../../../tools/bcl-test-importer/ bcl-test-importer
	cp ../../../tools/bcl-test-importer/bcl-test-importer .

generate-test-projects: test-importer ./RegisterType.cs.in ./BCLTests.csproj.in
	rm -Rf ./bin/generated
	mkdir -p ./bin/generated
	for p in ${TEST_PROJECTS}; do \
		mkdir -p ./bin/generated/$$p; \
	done
	for p in ${TEST_PROJECTS}; do \
		v="$$p"_VALUE; \
		assemblies=$$(printenv "$$v"); \
		cmd=""; \
		for a in $$assemblies; do\
			cmd="$$cmd --assembly=$$a"; \
		done; \
		echo "Generating type registration code."; \
		$$(./bcl-test-importer --mono-root=${MONO_ROOT} --platform=iOS --output=./bin/generated/$$p/RegisterType.cs --template=RegisterType.cs.in --generate-type-register --override $$cmd); \
		echo "Generating projects."; \
		$$(./bcl-test-importer --mono-root=${MONO_ROOT} --platform=iOS --output=./$$p.csproj --template=./BCLTests.csproj.in --generate-project --project-name=$$p --regiter-types-path=./bin/generated/$$p/RegisterType.cs --override $$cmd); \
	done

build-test-projects: generate-test-projects
	for p in ${TEST_PROJECTS}; do \
		nuget restore $$p.csproj ; \
		msbuild /p:Configuration=$(CONFIGURATION) $$p.csproj ; \
	done

all: build-test-projects

clean:
	@for p in ${TEST_PROJECTS}; do \
		rm -Rf ./$$p.csproj ;\
	done
	@rm -Rf ./bin
	@rm -Rf ./obj
	@rm -f bcl-test-importer
