#if __arm64__
#define OBJCMSGFUNC _xamarin_dyn_objc_msgSend
#define OBJCMSGCALL _objc_msgSend
#define RESOLVESUPER
#include "trampolines-arm64-objc_msgSend.inc.s"
#endif
