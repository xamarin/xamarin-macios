#if TARGET_OS_IOS || TARGET_OS_TV || TARGET_OS_WATCH
#define HAVE_UIKIT 1
#else
#define HAVE_UIKIT 0
#endif

#if TARGET_OS_IOS || TARGET_OS_TV || TARGET_OS_WATCH || defined (__x86_64__) || defined (__aarch64__)
#define HAVE_MAPKIT 1
#else
#define HAVE_MAPKIT 0
#endif

#if !TARGET_OS_WATCH
#define HAVE_COREMEDIA 1
#else
#define HAVE_COREMEDIA 0
#endif

#if !TARGET_OS_WATCH
#define HAVE_COREANIMATION 1
#else
#define HAVE_COREANIMATION 0
#endif
