DESTDIR ?= /usr/local/bin/
CONFIGURATION ?= Debug
MONO_ROOT:=../../../external/mono/
# similar to a dict, we create diff projects, then add the assemblies we want for each of the projects
# that way, we can match a project with more than one assembly since some test assemblies are small
# and we reduce the compilation. As it is atm we are using the same number of projects as the old
# way to do it, so that we can get xharness working, we will modify this to improve speed
# IMPORTANT you should notice that there are no '.' in the names, env vars do not like '.'
TEST_PROJECTS:=System \
			   SystemCore \
			   SystemData \
			   SystemNetHttp \
			   SystemNumerics \
			   SystemRuntimeSerialization \
			   SystemTransactions \
			   SystemXml \
			   SystemXmlLinq \
			   MonoSecurity \
			   SystemComponentModelDataAnnotations \
			   SystemJson \
			   SystemServiceModelWeb \
			   MonoDataTds \
			   SystemIOCompression \
			   SystemIOCompressionFileSystem \
			   MonoCSharp \
			   SystemSecurity \
			   SystemServiceModel \

# list of test assemblies to ignore, if the assembly is not present in one of the project or in
# the following list, compilation will fail. We do this so that if mono adds a new test assembly
# we do know that is being tested in the different platforms. TODO: Add an ignore per platform
COMMON_IGNORED_TEST_ASSEMBLIES:=MONOTOUCH_Commons.Xml.Relaxng_test.dll \
								MONOTOUCH_Cscompmgd_test.dll \
								MONOTOUCH_I18N.CJK_test.dll \
								MONOTOUCH_I18N.MidEast_test.dll \
								MONOTOUCH_I18N.Other_test.dll \
								MONOTOUCH_I18N.Rare_test.dll \
								MONOTOUCH_I18N.West_test.dll \
								MONOTOUCH_Mono.C5_test.dll \
								MONOTOUCH_Mono.CodeContracts_test.dll \
								MONOTOUCH_Mono.Parallel_test.dll \
								MONOTOUCH_Mono.Runtime.Tests_test.dll \
								MONOTOUCH_Mono.Tasklets_test.dll \
								MONOTOUCH_Novell.Directory.Ldap_test.dll \
								MONOTOUCH_System.Data.DataSetExtensions_test.dll \
								MONOTOUCH_System.Json.Microsoft_test.dll \
								MONOTOUCH_System.Runtime.Serialization.Formatters.Soap_test.dll \
								MONOTOUCH_System.Threading.Tasks.Dataflow_test.dll \
								MONOTOUCH_corlib_test.dll \

export System_VALUE:=MONOTOUCH_System_test.dll
export SystemCore_VALUE:=MONOTOUCH_System.Core_test.dll
export SystemData_VALUE:=MONOTOUCH_System.Data_test.dll
export SystemNetHttp_VALUE:=MONOTOUCH_System.Net.Http_test.dll
export SystemNumerics_VALUE:=MONOTOUCH_System.Numerics_test.dll
export SystemRuntimeSerialization_VALUE:=MONOTOUCH_System.Runtime.Serialization_test.dll
export SystemTransactions_VALUE:=MONOTOUCH_System.Transactions_test.dll
export SystemXml_VALUE:=MONOTOUCH_System.Xml_test.dll
export SystemXmlLinq_VALUE:=MONOTOUCH_System.Xml.Linq_test.dll
export MonoSecurity_VALUE:=MONOTOUCH_Mono.Security_test.dll
export SystemComponentModelDataAnnotations_VALUE:=MONOTOUCH_System.ComponentModel.DataAnnotations_test.dll
export SystemJson_VALUE:=MONOTOUCH_System.Json_test.dll
export SystemServiceModelWeb_VALUE:=MONOTOUCH_System.ServiceModel.Web_test.dll
export MonoDataTds_VALUE:=MONOTOUCH_Mono.Data.Tds_test.dll
export SystemIOCompression_VALUE:=MONOTOUCH_System.IO.Compression_test.dll
export SystemIOCompressionFileSystem_VALUE:=MONOTOUCH_System.IO.Compression.FileSystem_test.dll
export MonoCSharp_VALUE:=MONOTOUCH_Mono.CSharp_test.dll
export SystemSecurity_VALUE:=MONOTOUCH_System.Security_test.dll
export SystemServiceModel_VALUE:=MONOTOUCH_System.ServiceModel_test.dll

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
