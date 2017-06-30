// __IOS__
#if __IOS__

#if __TVOS__
#error __TVOS__ should not be defined when __IOS__ is defined
#endif

#if __WATCHOS__
#error __WATCHOS__ should not be defined when __IOS__ is defined
#endif

#if __MACOS__
#error __MACOS__ should not be defined when __IOS__ is defined
#endif

#endif // __IOS__


// __TVOS__
#if __TVOS__

#if __IOS__
#error __IOS__ should not be defined when __TVOS__ is defined
#endif

#if __WATCHOS__
#error __WATCHOS__ should not be defined when __TVOS__ is defined
#endif

#if __MACOS__
#error __MACOS__ should not be defined when __TVOS__ is defined
#endif

#endif // __TVOS__


// __WATCHOS__
#if __WATCHOS__

#if __IOS__
#error __IOS__ should not be defined when __WATCHOS__ is defined
#endif

#if __TVOS__
#error __TVOS__ should not be defined when __WATCHOS__ is defined
#endif

#if __MACOS__
#error __MACOS__ should not be defined when __WATCHOS__ is defined
#endif

#endif // __WATCHOS__


// __MACOS__
#if __MACOS__

#if __IOS__
#error __IOS__ should not be defined when __MACOS__ is defined
#endif

#if __TVOS__
#error __TVOS__ should not be defined when __MACOS__ is defined
#endif

#if __WATCHOS__
#error __WATCHOS__ should not be defined when __MACOS__ is defined
#endif

#endif // __MACOS__
