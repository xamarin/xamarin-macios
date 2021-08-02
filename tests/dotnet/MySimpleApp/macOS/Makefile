include ../shared.mk

coreclr:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:UseMonoRuntime=false"

coreclr-universal:
	$(Q) $(MAKE) build BUILD_ARGUMENTS='/p:RuntimeIdentifiers=\"osx-arm64\;osx-x64\" /p:UseMonoRuntime=false'

universal:
	$(Q) $(MAKE) build BUILD_ARGUMENTS='/p:RuntimeIdentifiers=\"osx-arm64\;osx-x64\"'
