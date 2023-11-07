include ../shared.mk

# To run monotouch-test with MonoVM:
#    make run-monovm
# To run monotouch-test with CoreCLR:
#    make run-coreclr
#
# Any arguments to 'dotnet build' can be set using the BUILD_PARAMETERS variable
#
#    make run-coreclr BUILD_PARAMETERS="/p:MonoBundlingExtraArgs=--registrar:static /p:MmpDebug=true"
#
# Any arguments to the test executable can be set using the ARGUMENTS variable (for instance to select which test to run):
#
#    make run-coreclr ARGUMENTS="--test namespace.class.testname"
#
# The test run's output will be written to a timestamped temporary file in /tmp (so subsequent test runs will write to a new file).
# The exact path to the log file will be printed during the test run.
#

TARGETS=prepare

LOGFILENAME:=$(TMPDIR)/monotouch-test/$(shell date +%Y-%m-%d--%H:%M:%S).log

run-monovm: export XAMARIN_RUNTIME=MonoVM
run-coreclr: export XAMARIN_RUNTIME=CoreCLR
run-%: $(TARGETS)
	$(Q) $(MAKE) build-specific
	$(Q) $(MAKE) exec

build-monovm: export XAMARIN_RUNTIME=MonoVM
build-coreclr: export XAMARIN_RUNTIME=CoreCLR
build-%: $(TARGETS)
	$(Q) $(MAKE) build-specific

build-specific:
	$(Q) test -n "$(XAMARIN_RUNTIME)" || (echo "$(COLOR_RED)You must set the variable $(COLOR_GRAY)XAMARIN_RUNTIME$(COLOR_RED) to select which runtime to use (to either $(COLOR_GRAY)CoreCLR$(COLOR_RED) or $(COLOR_GRAY)MonoVM$(COLOR_RED)).$(COLOR_CLEAR)"; exit 1)
	$(Q) rm -rf $(TOP)/tests/monotouch-test/dotnet/macOS/bin $(TOP)/tests/monotouch-test/dotnet/macOS/obj
	if ! $(DOTNET) build $(TOP)/tests/monotouch-test/dotnet/macOS/monotouch-test.csproj "/bl:$@-$(XAMARIN_RUNTIME).binlog" $(BUILD_PARAMETERS) /p:Configuration=$(XAMARIN_RUNTIME) /p:UseMonoRuntime=$(if $(subst MonoVM,,$(XAMARIN_RUNTIME)),false,true) $(MSBUILD_VERBOSITY); then \
			$(DOTNET) build /v:diag "$@-$(XAMARIN_RUNTIME).binlog"; \
			exit 1; \
	fi

exec-monovm: export XAMARIN_RUNTIME=MonoVM
exec-coreclr: export XAMARIN_RUNTIME=CoreCLR
exec-%: $(TARGETS)
	$(Q) $(MAKE) exec

exec:
	$(Q) test -n "$(XAMARIN_RUNTIME)" || (echo "XAMARIN_RUNTIME is not set"; exit 1)
	$(Q) echo "Running monotouch-test: $(COLOR_GRAY)$(abspath $(TOP)/tests/monotouch-test/dotnet/macOS/bin/$(XAMARIN_RUNTIME)/$(DOTNET_TFM)-macos/osx-*/monotouchtest.app/Contents/MacOS/monotouchtest) $(ARGUMENTS)$(COLOR_CLEAR)"
	$(Q) echo "Writing output to: $(COLOR_GRAY)$(LOGFILENAME)$(COLOR_CLEAR)"
	$(Q) mkdir -p "$(dir $(LOGFILENAME))"
	$(Q) echo "VM: $(XAMARIN_RUNTIME)" > "$(LOGFILENAME)"
	$(Q) if ! $(abspath $(TOP)/tests/monotouch-test/dotnet/macOS/bin/$(XAMARIN_RUNTIME)/$(DOTNET_TFM)-macos/osx-*/monotouchtest.app/Contents/MacOS/monotouchtest) $(ARGUMENTS) >> "$(LOGFILENAME)" 2>&1 ; then \
		echo "❌ Test run failed"; \
		grep "[[]FAIL[]]" "$(LOGFILENAME)"; \
		grep "Tests run:" "$(LOGFILENAME)"; \
		exit 1; \
	fi
	$(Q) echo "✅ Test run succeeded"
	$(Q) grep "Tests run:" "$(LOGFILENAME)"
