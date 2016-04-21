
#ifndef __MAIN_INTERNAL_H__
#define __MAIN_INTERNAL_H__

#ifdef __cplusplus
extern "C" {
#endif

#ifdef EXTENSION
	int NSExtensionMain (int argc, char** argv);
#endif

#ifdef WATCH_EXTENSION
// WatchKit.framework defines a main function.
	int main (int argc, char **argv);	
#endif

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __MAIN_INTERNAL_H__ */
