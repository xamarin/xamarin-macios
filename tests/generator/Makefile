TOP=../..

include $(TOP)/Make.config

export MD_MTOUCH_SDK_ROOT=$(IOS_DESTDIR)/$(MONOTOUCH_PREFIX)
export XamarinMacFrameworkRoot=$(MAC_DESTDIR)/Library/Frameworks/Xamarin.Mac.framework/Versions/Current

#
# * Use [IOS|MAC]_TESTS for simple tests that just verify the code can compile / generate.
#   To add such a test, just add the file, and add the name of the file (without extension) to [IOS|MAC]_TESTS.
#
# * Use [IOS|MAC]_CUSTOM_TESTS for more advanced tests that needs more validation.
#   In this case you additionally need to add a target for the name of the test, with the 
#   your test logic.
#

IOS_CURRENT_DIR=$(IOS_DESTDIR)/Library/Frameworks/Xamarin.iOS.framework/Versions/Current
IOS_GENERATOR = $(IOS_CURRENT_DIR)/bin/bgen /unsafe --target-framework=Xamarin.iOS,v1.0
IOS_CUSTOM_TESTS = bug53076withmodel fieldenumtests smartenumwithframework

ALL_TESTS = 
ifdef INCLUDE_IOS
ALL_TESTS += $(IOS_CUSTOM_TESTS)
endif

all-local:: $(ALL_TESTS) run-unit-tests

build-unit-tests:
	$(Q) $(SYSTEM_MONO) /Library/Frameworks//Mono.framework/Versions/Current/lib/mono/nuget/NuGet.exe restore $(TOP)/src/generator.sln
	$(SYSTEM_XBUILD) generator-tests.csproj $(XBUILD_VERBOSITY)

run-unit-tests: build-unit-tests
	rm -f .failed-stamp
	$(SYSTEM_MONO) --debug $(TOP)/packages/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe $(abspath $(TOP)/tests/generator/bin/Debug/generator-tests.dll) "--result=$(abspath $(CURDIR)/TestResult.xml);format=nunit2" $(TEST_FIXTURE) --labels=All || touch $(CURDIR)/.failed-stamp
	@# Create an html file and tell MonkeyWrench to upload it (if we're running there)
	@[[ -z "$$BUILD_REPOSITORY" ]] || \
		( xsltproc $(TOP)/tests/HtmlTransform.xslt TestResult.xml  > index.html && \
		echo "@MonkeyWrench: AddFile: $$PWD/index.html")
	@[[ ! -e .failed-stamp ]] 

bug46292:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs --process-enums
	@if ! grep -r Obsolete bug46292.tmpdir/BindingTests > /dev/null; then \
		echo "error: Could not find Obsolete attribute in generated code."; exit 1; \
	fi
	@if [ `grep -r Obsolete bug46292.tmpdir/BindingTests | wc -l` -ne 2 ]; then \
		echo "Error: Expected 2 Obsolete attributes in generated code."; exit 1; \
	fi

forcedtype:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs
	@if ! grep -r GetINativeObject forcedtype.tmpdir > /dev/null; then \
		echo "error: Could not find GetINativeObject usage in generated code."; exit 1; \
	fi
	@if [ `grep -r GetINativeObject forcedtype.tmpdir | wc -l` -ne 12 ]; then \
		echo "Error: Expected 12 GetINativeObject usages in generated code."; exit 1; \
	fi

bug53076withmodel:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs
	@if [ `grep -r "Async (this IMyFooProtocol" $@.tmpdir/Bug53076WithModelTest | wc -l` -ne 10 ]; then \
		echo "Error: Expected 10 'Async (this IMyFooProtocol' matches in generated code. If you modified code that generates extension FooRequiredMethodAsync (AsyncAttribute) please update the 'Async (this IMyFooProtocol' count."; exit 1; \
	fi

fieldenumtests:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs --process-enums

smartenumwithframework:
	@rm -Rf $@.tmpdir
	@mkdir -p $@.tmpdir
	$(if $(V),,@echo "$@";) $(IOS_GENERATOR) --sourceonly:$@.source -tmpdir=$@.tmpdir $@.cs --process-enums
	@if [ `grep -r "Libraries.CoreImage.Handle" $@.tmpdir/SmartEnumWithFramework | wc -l` -ne 2 ]; then \
		echo "Error: Expected 2 'Libraries.CoreImage.Handle'."; exit 1; \
	fi

clean-local::
	rm -f *.dll *.source
	rm -Rf *.tmpdir
