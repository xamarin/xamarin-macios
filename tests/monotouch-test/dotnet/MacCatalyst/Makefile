include ../shared.mk

llvm:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:RuntimeIdentifier=maccatalyst-arm64 /p:MtouchUseLlvm=true /p:MtouchLink=SdkOnly /p:Configuration=Release-llvm /bl:$@.binlog"

run-llvm:
	$(Q) $(MAKE) build BUILD_ARGUMENTS="/p:RuntimeIdentifier=maccatalyst-arm64 /p:MtouchUseLlvm=true /p:MtouchLink=SdkOnly /p:Configuration=Release-llvm /bl:$@.binlog /t:Run"
