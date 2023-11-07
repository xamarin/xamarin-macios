#if __arm64__
#define OBJCMSGFUNC _xamarin_dyn_objc_msgSendSuper
#define OBJCMSGCALL _objc_msgSendSuper
#define RESOLVESUPER ldr	x0, [x0]
#include "trampolines-arm64-objc_msgSend.inc.s"
#endif

