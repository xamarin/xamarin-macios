TOP=../..

include $(TOP)/Make.config

all-local:: run-tests

ifneq ($(TEST_CATEGORY),)
WHERE_CONDITION=/where "cat == $(TEST_CATEGORY)"
endif

TEST_RESULT:=TestResult-$(shell date "+%Y%m%d-%H%M%S")

build: bin/Debug/sampletester.dll

run-tests: bin/Debug/sampletester.dll
	$(Q) rm -f .failed-stamp
	$(Q) $(TOP)/tools/nunit3-console-3.11.1 $(WHERE_CONDITION) $(abspath $(CURDIR)/bin/Debug/sampletester.dll) "--result=$(abspath $(CURDIR)/$(TEST_RESULT).xml)" $${TEST_FIXTURE:+"$$TEST_FIXTURE"} --labels=After --inprocess || touch $(CURDIR)/.failed-stamp
	$(Q) xsltproc HtmlReport.xslt "$(TEST_RESULT).xml" > "$(TEST_RESULT).html"
	$(Q) $(CP) "$(TEST_RESULT).html" index.html
	@[[ ! -e .failed-stamp ]]

run-tests-system: bin/Debug/sampletester.dll
	$(MAKE) TESTS_USE_SYSTEM=1

bin/Debug/sampletester.dll: $(wildcard *.cs */*.cs *.csproj Makefile)
	$(Q) $(SYSTEM_MONO) /Library/Frameworks//Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore
	$(Q_BUILD) $(SYSTEM_MSBUILD) sampletester.csproj $(MSBUILD_VERBOSITY)
