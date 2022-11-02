TOP=../..

include $(TOP)/Make.config

TARGETS=

ifeq ("$(BENCHMARK_OUTPUT_PATH)","")
BENCHMARK_OUTPUT_PATH:=$(abspath $(CURDIR)/output/$(shell date +%Y-%m-%d--%H:%M:%S))
endif
export BENCHMARK_OUTPUT_PATH
BINLOG_TIMESTAMP:=$(shell date +%Y-%m-%d--%H:%M:%S)

PERFLOGFILENAME:=$(BENCHMARK_OUTPUT_PATH)/perflog-$(shell date +%Y-%m-%d--%H:%M:%S).log

# Legacy Xamarin

build-perftest-with-legacy:
	$(Q) $(SYSTEM_MSBUILD) /r "/bl:$@-$(BINLOG_TIMESTAMP).binlog" legacy/*.csproj $(MSBUILD_VERBOSITY)

run-perftest-with-legacy: $(TARGETS)
	@echo "Running perftest (legacy) and writing output to $(PERFLOGFILENAME)..."
	$(Q) mkdir -p $(BENCHMARK_OUTPUT_PATH)
	$(Q) echo "VM: Legacy" > $(PERFLOGFILENAME)
	(time $(abspath legacy/bin/Debug/perftest.app/Contents/MacOS/perftest) $(ARGUMENTS) 2>&1) 2>&1 | tee -a $(PERFLOGFILENAME)

# .NET

build-perftest-with-mono: export XAMARIN_RUNTIME=MonoVM
build-perftest-with-mono: $(TARGETS)
	$(Q) $(MAKE) build-perf-test

build-perftest-with-coreclr: export XAMARIN_RUNTIME=CoreCLR
build-perftest-with-coreclr: $(TARGETS)
	$(Q) $(MAKE) build-perf-test

RUNTIME_IDENTIFIER?=osx-x64

build-perf-test: $(TARGETS)
	$(Q) test -n "$(XAMARIN_RUNTIME)" || ( echo "❌ XAMARIN_RUNTIME must be set! ❌"; exit 1)
	$(Q) rm -Rf bin/$(XAMARIN_RUNTIME) obj/$(XAMARIN_RUNTIME)
	$(Q) if ! (cd dotnet && $(DOTNET) build $(abspath dotnet/perftest.csproj) "/bl:$(abspath $@-$(XAMARIN_RUNTIME)-$(BINLOG_TIMESTAMP).binlog)" $(BUILD_PARAMETERS) /p:Configuration=$(XAMARIN_RUNTIME) /p:_XamarinRuntime=$(XAMARIN_RUNTIME) /p:RuntimeIdentifier=$(RUNTIME_IDENTIFIER) $(DOTNET_BUILD_VERBOSITY)); then \
		$(DOTNET) build /v:diag "$(abspath $@-$(XAMARIN_RUNTIME)-$(BINLOG_TIMESTAMP).binlog)"; \
		echo "Performance test build failed" & \
		exit 1; \
	fi

run-perftest-with-mono: export XAMARIN_RUNTIME=MonoVM
run-perftest-with-mono: $(TARGETS)
	$(Q) $(MAKE) run-perftest-with-runtime

run-perftest-with-coreclr: export XAMARIN_RUNTIME=CoreCLR
run-perftest-with-coreclr: $(TARGETS)
	$(Q) $(MAKE) run-perftest-with-runtime

perftest-with-mono: $(TARGETS)
	$(Q) $(MAKE) build-perftest-with-mono
	$(Q) $(MAKE) run-perftest-with-mono

perftest-with-coreclr: $(TARGETS)
	$(Q) $(MAKE) build-perftest-with-coreclr
	$(Q) $(MAKE) run-perftest-with-coreclr

perftest-with-legacy: $(TARGETS)
	$(Q) $(MAKE) build-perftest-with-legacy
	$(Q) $(MAKE) run-perftest-with-legacy

run-perftest-dotnet:
	$(Q) $(MAKE) build-perftest-with-coreclr
	$(Q) $(MAKE) build-perftest-with-mono
	$(Q) $(MAKE) run-perftest-with-mono
	$(Q) $(MAKE) run-perftest-with-coreclr
	@echo "Log files were stored in $(BENCHMARK_OUTPUT_PATH)"

run-perftest:
	$(Q) $(MAKE) build-perftest-with-legacy
	$(Q) $(MAKE) build-perftest-with-coreclr
	$(Q) $(MAKE) build-perftest-with-mono
	$(Q) $(MAKE) run-perftest-with-legacy
	$(Q) $(MAKE) run-perftest-with-mono
	$(Q) $(MAKE) run-perftest-with-coreclr
	@echo "Log files were stored in $(BENCHMARK_OUTPUT_PATH)"

run-perftest-with-runtime: $(TARGETS)
	@echo "Running perftest ($(XAMARIN_RUNTIME)) and writing output to $(PERFLOGFILENAME)..."
	$(Q) mkdir -p $(BENCHMARK_OUTPUT_PATH)
	$(Q) echo "VM: $(XAMARIN_RUNTIME)" > $(PERFLOGFILENAME)
	$(Q) set -o pipefail; (time $(abspath dotnet/bin/$(XAMARIN_RUNTIME)/$(DOTNET_TFM)-macos/$(RUNTIME_IDENTIFIER)/perftest.app/Contents/MacOS/perftest) $(ARGUMENTS) 2>&1) 2>&1 | tee -a $(PERFLOGFILENAME)
