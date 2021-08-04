include ../shared.mk

dev:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:RuntimeIdentifier=ios-arm64"

run-dev:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:RuntimeIdentifier=ios-arm64 /t:Run"

llvm:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:RuntimeIdentifier=ios-arm64 /p:MtouchUseLlvm=true /p:MtouchLink=SdkOnly /p:Configuration=Release-llvm /bl:@.binlog"

norm:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:RuntimeIdentifier=ios-arm64                       /p:MtouchLink=SdkOnly /p:Configuration=Release-norm /bl:@.binlog"
