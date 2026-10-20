TOP=../../../..

include $(TOP)/Make.config

TARGETS += \
	.install-workloads.stamp \

all-local:: compare

check:
	if test -z "$(PROJECT_OLD_APP)"; then \
		echo "The app to compare against must be specified with the PROJECT_OLD_APP environment variable."; \
		exit 1; \
	fi

compare compare-size: check $(TARGETS)
	rm -rf ../../packages
	git clean -xfdq
	time $(MAKE) build
	$(MAKE) report

PROJECT?=$(shell basename $(PWD))
PROJECT_NEW_NAME?=$(PROJECT)

PROJECT_NEW_FILE?=./$(PROJECT_NEW_NAME).csproj
PROJECT_NEW_APP?=./bin/Release/$(DOTNET_TFM)-ios/ios-arm64/$(PROJECT).app

APPCOMPARE?=appcompare

report: check
	$(APPCOMPARE) \
		$(abspath $(PROJECT_OLD_APP)) \
		$(abspath $(PROJECT_NEW_APP)) \
		$(abspath ./report.md)
	echo "Created $(abspath ./report.md)"
	gist "$(abspath ./report.md)"

COMMON_ARGS=/p:Configuration=Release $(DOTNET_BUILD_VERBOSITY)

build: $(TARGETS)
	$(DOTNET) build $(PROJECT_NEW_FILE) $(COMMON_ARGS) /bl:$@.binlog /p:RuntimeIdentifier=ios-arm64

run: $(TARGETS)
	$(DOTNET) run   $(PROJECT_NEW_FILE) $(COMMON_ARGS) /bl:$@.binlog /p:RuntimeIdentifier=ios-arm64

run-sim: $(TARGETS)
	$(DOTNET) run   $(PROJECT_NEW_FILE) $(COMMON_ARGS) /bl:$@.binlog /p:RuntimeIdentifier=iossimulator-arm64

.install-workloads.stamp:
	$(DOTNET) workload install maui-ios --skip-manifest-update
	$(Q) touch $@
