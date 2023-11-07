
#ifndef __MAIN_INTERNAL_H__
#define __MAIN_INTERNAL_H__

#ifdef __cplusplus
extern "C" {
#endif

//#ifdef EXTENSION
	int NSExtensionMain (int argc, char** argv);
#ifdef MONOMAC
    int xamarin_mac_extension_main (int argc, char **argv);
#endif
//#endif

#ifdef WATCH_EXTENSION
// Some versions of WatchKit.framework defines a main function, others don't.
// We decide which one to use in our GenerateMain function
	int xamarin_watch_extension_main (int argc, char **argv);
#endif

#ifdef TV_EXTENSION
	int TVExtensionMain (int argc, char **argv);	
#endif

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __MAIN_INTERNAL_H__ */
