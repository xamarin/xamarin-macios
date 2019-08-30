#ifndef __MONOTOUCH_SUPPORT_H__
#define __MONOTOUCH_SUPPORT_H__

#ifdef __cplusplus
extern "C" {
#endif

const char *   xamarin_get_locale_country_code ();
void           xamarin_log (const unsigned short *unicodeMessage);
void *         xamarin_timezone_get_data (const char *name, int *size);
char **        xamarin_timezone_get_names (int *count);
void           xamarin_start_wwan (const char *uri);

#ifdef MONOTOUCH
const char *   xamarin_GetFolderPath (int folder);
#endif

#ifdef MONOMAC
const char *
xamarin_encode_CGAffineTransform ();
#endif

#if defined (__arm64__)
void objc_msgSend_stret (void);
void objc_msgSendSuper_stret (void);
#endif

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif /* __MONOTOUCH_SUPPORT_H__ */
