include ../shared.mk

dev:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:RuntimeIdentifier=ios-arm64"

llvm:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:RuntimeIdentifier=ios-arm64 /p:MtouchUseLlvm=true /p:MtouchLink=SdkOnly /p:Configuration=Release-llvm /bl:$@.binlog"

run-llvm:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:RuntimeIdentifier=ios-arm64 /p:MtouchUseLlvm=true /p:MtouchLink=SdkOnly /p:Configuration=Release-llvm /bl:$@.binlog /t:Run"

norm:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:RuntimeIdentifier=ios-arm64                       /p:MtouchLink=SdkOnly /p:Configuration=Release-norm /bl:$@.binlog"

run-norm:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:RuntimeIdentifier=ios-arm64                       /p:MtouchLink=SdkOnly /p:Configuration=Release-norm /bl:$@.binlog /t:Run"
