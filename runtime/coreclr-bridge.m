/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
*  Authors: Rolf Bjarne Kvinge
*
*  Copyright (C) 2021 Microsoft Corp.
*
*/

#if defined (CORECLR_RUNTIME)

#include <sys/stat.h>
#include <inttypes.h>
#include <pthread.h>
#include <sys/mman.h>

#include "product.h"
#include "runtime-internal.h"
#include "slinked-list.h"
#include "xamarin/xamarin.h"
#include "xamarin/coreclr-bridge.h"

#include "coreclrhost.h"

unsigned int coreclr_domainId = 0;
void *coreclr_handle = NULL;
pthread_mutex_t monoobject_lock = PTHREAD_MUTEX_INITIALIZER;
SList *release_at_exit = NULL; // A list of MonoObject*s to be released at process exit

#if defined (TRACK_MONOOBJECTS)

// To enable tracking of MonoObject* instances, uncomment the TRACK_MONOOBJECTS define in:
// * runtime/runtime-internal.h
// * src/ObjCRuntime/Runtime.CoreCLR.cs
// Both defines must be uncommented for tracking to work. Once enabled, you can opt-in to
// capturing the stack trace of when the MonoObject* was created, by setting the
// MONOOBJECT_TRACKING_WITH_STACKTRACES environment variable.

#include <execinfo.h>

static int _Atomic monoobject_created = 0;
static int _Atomic monoobject_destroyed = 0;
static CFMutableDictionaryRef monoobject_dict = NULL;

struct monoobject_tracked_entry {
	char *managed;
	void *addresses [128];
	int frames;
	char *native;
};

static char *
get_stacktrace (void **addresses, int frames)
{
	// get the symbols for the addresses
	char** strs = backtrace_symbols (addresses, frames);

	// compute the total length of all the symbols, adding 1 for every line (for the newline)
	size_t length = 0;
	int i;
	for (i = 0; i < frames; i++)
		length += strlen (strs [i]) + 1;
	length++;

	// format the symbols as one long string with newlines
	char *rv = (char *) calloc (1, length);
	char *buffer = rv;
	size_t left = length;
	for (i = 0; i < frames; i++) {
		snprintf (buffer, left, "%s\n", strs [i]);
		size_t slen = strlen (strs [i]) + 1;
		left -= slen;
		buffer += slen;
	}
	free (strs);

	return rv;
}

void
xamarin_bridge_log_monoobject (MonoObject *mobj, const char *stacktrace)
{
	// add stack traces if we have them / they've been been requested
	if (monoobject_dict != NULL) {
		// create a new entry
		struct monoobject_tracked_entry *value = (struct monoobject_tracked_entry *) calloc (1, sizeof (struct monoobject_tracked_entry));

		value->managed = stacktrace ? xamarin_strdup_printf ("%s", stacktrace) : NULL;
		value->frames = backtrace ((void **) &value->addresses, sizeof (value->addresses) / sizeof (&value->addresses [0]));

		// insert into our dictionary of monoobjects
		pthread_mutex_lock (&monoobject_lock);
		CFDictionarySetValue (monoobject_dict, mobj, value);
		pthread_mutex_unlock (&monoobject_lock);
	}

	atomic_fetch_add (&monoobject_created, 1);
}

void
xamarin_bridge_dump_monoobjects ()
{
	if (monoobject_dict != NULL) {
		// dump the monoobject's that haven't been freed (max 10 entries).
		pthread_mutex_lock (&monoobject_lock);

		// get the keys and values
		unsigned int length = (unsigned int) CFDictionaryGetCount (monoobject_dict);
		MonoObject** keys = (MonoObject **) calloc (1, sizeof (void*) * length);
		char** values = (char **) calloc (1, sizeof (char*) * length);
		CFDictionaryGetKeysAndValues (monoobject_dict, (const void **) keys, (const void **) values);

		// is there anything left in the dictionary? if so, show that
		unsigned int items_to_show = length > 10 ? 10 : length;
		if (items_to_show > 0) {
			fprintf (stderr, "⚠️ There were %i MonoObjects created, %i MonoObjects freed, so %i were not freed.\n", (int) monoobject_created, (int) monoobject_destroyed, (int) (monoobject_created - monoobject_destroyed));
			fprintf (stderr, "Showing the first %i (of %i) MonoObjects:\n", items_to_show, length);
			for (unsigned int i = 0; i < items_to_show; i++) {
				MonoObject *obj = keys [i];
				struct monoobject_tracked_entry *value = (struct monoobject_tracked_entry *) values [i];
				char *fullname = xamarin_get_object_type_fullname (obj->gchandle);
				fprintf (stderr, "Object %i/%i %p RC: %i Type: %s\n", i + 1, (int) length, obj, (int) obj->reference_count, fullname);
				xamarin_free (fullname);
				if (value->managed && *value->managed)
					fprintf (stderr, "\tManaged stack trace:\n%s\n", value->managed);
				if (value->native == NULL && value->frames > 0)
					value->native = get_stacktrace (value->addresses, value->frames);
				if (value->native && *value->native)
					fprintf (stderr, "\tNative stack trace:\n%s\n", value->native);
			}
			fprintf (stderr, "⚠️ There were %i MonoObjects created, %i MonoObjects freed, so %i were not freed.\n", (int) monoobject_created, (int) monoobject_destroyed, (int) (monoobject_created - monoobject_destroyed));
		} else {
			fprintf (stderr, "✅ There were %i MonoObjects created, %i MonoObjects freed, so no leaked MonoObjects.\n", (int) monoobject_created, (int) monoobject_destroyed);
		}
		pthread_mutex_unlock (&monoobject_lock);

		free (keys);
		free (values);
	} else {
		fprintf (stderr, "There were %i MonoObjects created, %i MonoObjects freed, so %i were not freed.\n", (int) monoobject_created, (int) monoobject_destroyed, (int) (monoobject_created - monoobject_destroyed));
	}
}

static void
monoobject_dict_free_value (CFAllocatorRef allocator, const void *value)
{
	struct monoobject_tracked_entry* v = (struct monoobject_tracked_entry *) value;
	xamarin_free (v->managed);
	if (v->native)
		free (v->native);
	free (v);
}

#endif // defined (TRACK_MONOOBJECTS)

/*
 * Toggle-ref support for CoreCLR is a bit different than for MonoVM. It goes like this:
 *
 * 1) We have to opt-in for the required GC support by calling
 *    ObjectiveCMarshal.Initialize (in managed code) at startup. This happens
 *    in Runtime.InitializeCoreCLRBridge (in Runtime.CoreCLR.cs).
 *
 * 2) Types that can be toggled, must have the [ObjectiveCTrackedType]
 *    attribute (on any subclass). We put this attribute on NSObject.
 *
 * 3) We have to call ObjectiveCMarshal.CreateReferenceTrackingHandle when an
 *    object is toggled. This callback returns a GCHandle for the managed
 *    object, and a pointer to native memory (size: 2 pointers) where we can
 *    store whatever we want. We store the native Handle, and the Flags
 *    property. Unfortunately this means duplicating information, and we have
 *    to make sure they're in sync. It didn't see a sane way around this
 *    though, because we need the Handle and the Flags somewhere accessible
 *    from native code during the GC (so we can't store them in managed
 *    memory), while at the same time we don't want to create an additional
 *    native block of memory for every NSObject, nor use some complex logic to
 *    support either a managed or a native storage. With the current solution
 *    we're only using additional native memory for toggled objects.
 *    Additionally, updates have to flow:
 *
 *    a) From managed to native for Handle and Flags, so we update the native
 *       memory (if it's there) when the Handle or Flags properties are set.
 *    b) From native to managed for a single flag value
 *       (NSObjectFlagsInFinalizerQueue), which we fetch in managed code in
 *       the Flags getter.
 *
 *    Note: we call ObjectiveCMarshal.CreateReferenceTrackingHandle for all
 *    NSObjects, not only toggled ones, because we need point 5) below to
 *    happen for all NSObjects, not just toggled ones.
 *
 * 4) The CoreCLR GC will invoke a callback we installed when calling
 *    ObjectiveCMarshal.Initialize to check if that toggled managed object can
 *    be collected or not. This callback is executed during the GC, which
 *    means it's very limited what we can do safely: but we can read and write
 *    to the memory given to us when the managed object was toggled, which is
 *    why we store the Handle and the Flags property - that's what we need to
 *    know to determine whether the managed object can be collected or not.
 *
 * 5) When the managed object is finalized, the GC will invoke another
 *    callback (xamarin_coreclr_reference_tracking_tracked_object_entered_finalization)
 *    to let us know, and we'll set the corresponding flag in the flags
 *
 * 6) Finally, the GCHandle we got in step 3) is freed when the managed peer
 *    is freed and removed from our object map.
 *
 * Caveat: we don't support the server GC (because it uses multiple threads,
 * and thus may call xamarin_coreclr_reference_tracking_begin_end_callback
 * from multiple threads for the same garbage collection, which we don't
 * support right now - but it may be possible to implement by using a
 * different lock in xamarin_gc_event).
 *
 * Ref: https://github.com/dotnet/runtime/issues/44659
 * Ref: https://github.com/dotnet/designs/blob/1bb5844c165195e2f633cb1dbe042c4b92aefc4d/accepted/2021/objectivec-interop.md
 */

struct TrackedObjectInfo {
	id handle;
	enum NSObjectFlags flags;
};

void
xamarin_bridge_setup ()
{
}

void
xamarin_bridge_initialize ()
{
#if defined (TRACK_MONOOBJECTS)
	// Only capture the stack trace if requested explicitly, it has a very significant perf hit (monotouch-test is 3x slower).
	const char *with_stacktraces = getenv ("MONOOBJECT_TRACKING_WITH_STACKTRACES");
	if (with_stacktraces && *with_stacktraces) {
		// Create a dictionary to store the stack traces
		CFDictionaryValueCallBacks value_callbacks = { 0 };
		value_callbacks.release = monoobject_dict_free_value;
		monoobject_dict = CFDictionaryCreateMutable (kCFAllocatorDefault, 0, NULL, &value_callbacks);

		fprintf (stderr, "Stack traces enabled for MonoObject tracking.\n");
	}
#endif // defined (TRACK_MONOOBJECTS)
}

void
xamarin_bridge_shutdown ()
{
	SList *list;

	// Free our list of MonoObject*s to free at process exist.
	// No need to keep the lock locked while we traverse the list, the only thing we need to protect
	// are reads and writes to the 'release_at_exit' variable, so let's do just that.
	pthread_mutex_lock (&monoobject_lock);
	list = release_at_exit;
	release_at_exit = NULL;
	pthread_mutex_unlock (&monoobject_lock);

	while (list) {
		xamarin_mono_object_release ((MonoObject **) &list->data);
		list = list->next;
	}
	s_list_free (list);

#if defined (TRACK_MONOOBJECTS)
	xamarin_bridge_dump_monoobjects ();
#endif
}

static bool reference_tracking_end = false;

// This callback will be called once before the GC starts calling xamarin_coreclr_reference_tracking_is_referenced_callback,
// and once the GC is done. We keep track of which case we're in in the 'reference_tracking_end' variable, and raise the
// corresponding GC event. It will only be called once for each GC, both the begin and the end on the same thread.
void
xamarin_coreclr_reference_tracking_begin_end_callback ()
{
	LOG_CORECLR (stderr, "%s () reference_tracking_end: %i\n", __func__, reference_tracking_end);
	if (reference_tracking_end) {
		xamarin_gc_event (MONO_GC_EVENT_POST_START_WORLD);
	} else {
		xamarin_gc_event (MONO_GC_EVENT_PRE_STOP_WORLD);
	}
	reference_tracking_end = !reference_tracking_end;
}

// This callback is called by the GC to check whether a given managed object
// can be collected or not. The single 'ptr' argument is the native memory
// returned by the managed call to
// ObjectiveCMarshal.CreateReferenceTrackingHandle, and this memory can be
// accessed while the GC is running. In here we store the native Handle for
// the managed object, and any flags, both of which we need to know in this
// method to determine whether the corresponding managed object can be
// collected or not.
int
xamarin_coreclr_reference_tracking_is_referenced_callback (void* ptr)
{
	// This is a callback called by the GC, so there's not much we can do here safely.
	// Most importantly we can't call managed code, nor access managed memory.
	// But we can access the native memory given to us when the object was toggled
	// (and which is passed as the 'ptr' argument), so let's get the data we need from there.
	int rv = 0;
	struct TrackedObjectInfo *info = (struct TrackedObjectInfo *) ptr;
	enum NSObjectFlags flags = info->flags;
	bool isRegisteredToggleRef = (flags & NSObjectFlagsRegisteredToggleRef) == NSObjectFlagsRegisteredToggleRef;
	id handle = info->handle;
	MonoToggleRefStatus res = (MonoToggleRefStatus) 0;

	if (isRegisteredToggleRef) {
		res = xamarin_gc_toggleref_callback (flags, handle, NULL, NULL);

		switch (res) {
		case MONO_TOGGLE_REF_DROP:
			// There's no equivalent to DROP in CoreCLR, so just treat it as weak.
		case MONO_TOGGLE_REF_WEAK:
			rv = 0;
			break;
		case MONO_TOGGLE_REF_STRONG:
			rv = 1;
			break;
		default:
			LOG_CORECLR (stderr, "%s (%p -> handle: %p flags: %i): INVALID toggle ref value: %i\n", __func__, ptr, handle, flags, res);
			break;
		}
	} else {
		// If this isn't a toggle ref, it's effectively a weak gchandle
		rv = 0;
	}

	LOG_CORECLR (stderr, "%s (%p -> handle: %p flags: %i) => %i (res: %i) isRegisteredToggleRef: %i\n", __func__, ptr, handle, flags, rv, res, isRegisteredToggleRef);

	return rv;
}

// This callback is called when an object is queued for finalization. The
// single 'ptr' argument is the native memory returned by the managed call to
// ObjectiveCMarshal.CreateReferenceTrackingHandle, and this memory can be
// accessed while the GC is running (which it is when this method is called).
// In here we set the NSObjectFlagsInFinalizerQueue flag, which managed code
// (the NSObject.flags property) will fetch.
void
xamarin_coreclr_reference_tracking_tracked_object_entered_finalization (void* ptr)
{
	struct TrackedObjectInfo *info = (struct TrackedObjectInfo *) ptr;
	info->flags = (enum NSObjectFlags) (info->flags | NSObjectFlagsInFinalizerQueue);
	LOG_CORECLR (stderr, "%s (%p) flags: %i\n", __func__, ptr, (int) info->flags);
}

void
xamarin_coreclr_unhandled_exception_handler (void *context)
{
	// 'context' is the GCHandle returned by the managed Runtime.UnhandledExceptionPropagationHandler function.
	GCHandle exception_gchandle = (GCHandle) context;

	LOG_CORECLR (stderr, "%s (%p)\n", __func__, context);

	// xamarin_process_managed_exception_gchandle will free the GCHandle
	xamarin_process_managed_exception_gchandle (exception_gchandle);

	// The call to xamarin_process_managed_exception_gchandle should either abort or throw an Objective-C exception,
	// and in neither case should we end up here, so just assert.
	xamarin_assertion_message ("Failed to process unhandled managed exception.");
}

void
xamarin_enable_new_refcount ()
{
	// Nothing to do here.
}

/**
 * xamarin_bridge_decode_value:
 * 
 * This implementation is a slightly modified copy (to make it compile) of mono_metadata_decode_value
 * https://github.com/dotnet/runtime/blob/08a7b2382799082eedb94d70fca6c66eb75f2872/src/mono/mono/metadata/metadata.c#L1525
 */
guint32
xamarin_bridge_decode_value (const char *_ptr, const char **rptr)
{
	const unsigned char *ptr = (const unsigned char *) _ptr;
	unsigned char b = *ptr;
	guint32 len;
	
	if ((b & 0x80) == 0){
		len = b;
		++ptr;
	} else if ((b & 0x40) == 0){
		len = (guint32) ((b & 0x3f) << 8 | ptr [1]);
		ptr += 2;
	} else {
		len = (guint32) (((b & 0x1f) << 24) |
			(ptr [1] << 16) |
			(ptr [2] << 8) |
			ptr [3]);
		ptr += 4;
	}
	if (rptr)
		*rptr = (char*)ptr;
	
	return len;
}

static char *
xamarin_read_config_string (const char **buf)
{
		guint32 configLength = xamarin_bridge_decode_value (*buf, buf);
		char *value = strndup (*buf, configLength);
		*buf = *buf + configLength;
		return value;
}

static void *
xamarin_mmap_runtime_config_file (size_t *length)
{
	if (xamarin_runtime_configuration_name == NULL) {
		LOG (PRODUCT ": No runtime config file provided at build time.\n");
		return NULL;
	}

	char path [1024];
	if (!xamarin_locate_app_resource (xamarin_runtime_configuration_name, path, sizeof (path))) {
		LOG (PRODUCT ": Could not locate the runtime config file '%s' in the app bundle.\n", xamarin_runtime_configuration_name);
		return NULL;
	}
	
	int fd = open (path, O_RDONLY);
	if (fd == -1) {
		LOG (PRODUCT ": Could not open the runtime config file '%s' in the app bundle: %s\n", path, strerror (errno));
		return NULL;
	}

	struct stat stat_buf = { 0 };
	if (fstat (fd, &stat_buf) == -1) {
		LOG (PRODUCT ": Could not stat the runtime config file '%s' in the app bundle: %s\n", path, strerror (errno));
		close (fd);
		return NULL;
	}

	*length = (size_t) stat_buf.st_size;
	void *buffer = mmap (NULL, *length, PROT_READ, MAP_PRIVATE, fd, 0);
	close (fd);
	return buffer;
}

// Input: the property keys + values passed to xamarin_bridge_vm_initialize
// Output: newly allocated arrays of property keys + values that include those passed to xamarin_bridge_vm_initialize together with those in the runtimeconfig.bin file
// Caller must free the allocated arrays + their elements
void
xamarin_bridge_compute_properties (int inputCount, const char **inputKeys, const char **inputValues, int* outputCount, const char ***outputKeys, const char ***outputValues)
{
	size_t fd_len = 0;
	const char *buf = (const char *) xamarin_mmap_runtime_config_file (&fd_len);
	int runtimeConfigCount = 0;

	if (buf != NULL)
		runtimeConfigCount = (int) xamarin_bridge_decode_value (buf, &buf);

	// Allocate the output arrays
	*outputCount = inputCount + runtimeConfigCount;
	*outputKeys = (const char **) calloc ((size_t) *outputCount, sizeof (char *));
	*outputValues = (const char **) calloc ((size_t) *outputCount, sizeof (char *));

	// Read the runtimeconfig properties
	// https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/docs/design/mono/mobile-runtimeconfig-json.md#the-encoded-runtimeconfig-format
	for (int i = 0; i < runtimeConfigCount; i++) {
		char *key = xamarin_read_config_string (&buf);
		char *value = xamarin_read_config_string (&buf);
		(*outputKeys) [i] = key;
		(*outputValues) [i] = value;
	}

	// Copy the input properties
	for (int i = 0; i < inputCount; i++) {
		if (inputKeys [i] != NULL && inputValues [i] != NULL) {
			(*outputKeys) [i + runtimeConfigCount] = strdup (inputKeys [i]);
			(*outputValues) [i + runtimeConfigCount] = strdup (inputValues [i]);
		} else {
			NSLog (@PRODUCT ": No name/value specified for runtime property %s=%s", inputKeys [i], inputValues [i]);
		}
	}

	if (buf != NULL)
		munmap ((void *) buf, fd_len);
}

#if !defined (NATIVEAOT)
bool
xamarin_bridge_vm_initialize (int propertyCount, const char **propertyKeys, const char **propertyValues)
{
	int rv;

	int combinedPropertyCount = 0;
	const char **combinedPropertyKeys = NULL;
	const char **combinedPropertyValues = NULL;

	xamarin_bridge_compute_properties (propertyCount, propertyKeys, propertyValues, &combinedPropertyCount, &combinedPropertyKeys, &combinedPropertyValues);

	const char *executablePath = [[[[NSBundle mainBundle] executableURL] path] UTF8String];
	rv = coreclr_initialize (
		executablePath,
		xamarin_executable_name,
		combinedPropertyCount,
		combinedPropertyKeys,
		combinedPropertyValues,
		&coreclr_handle,
		&coreclr_domainId
		);

	for (int i = 0; i < combinedPropertyCount; i++) {
		free ((void *) combinedPropertyKeys [i]);
		free ((void *) combinedPropertyValues [i]);
	}
	free ((void *) combinedPropertyKeys);
	free ((void *) combinedPropertyValues);

	LOG_CORECLR (stderr, "xamarin_vm_initialize (%i, %p, %p): rv: %i domainId: %i handle: %p\n", combinedPropertyCount, combinedPropertyKeys, combinedPropertyValues, rv, coreclr_domainId, coreclr_handle);

	return rv == 0;
}
#endif // !defined (NATIVEAOT)

void
xamarin_install_nsautoreleasepool_hooks ()
{
	// No need to do anything here for CoreCLR.
}

void
mono_runtime_set_pending_exception (MonoException *exc, mono_bool overwrite)
{
	LOG_CORECLR (stderr, "%s (%p, %i)\n", __func__, exc, overwrite);
	xamarin_bridge_set_pending_exception (exc);
}

void
xamarin_handle_bridge_exception (GCHandle gchandle, const char *method)
{
	if (gchandle == INVALID_GCHANDLE)
		return;

	if (method == NULL)
		method = "<unknown method>";

	fprintf (stderr, "%s threw an exception: %p => %s\n", method, gchandle, [xamarin_print_all_exceptions (gchandle) UTF8String]);
	xamarin_assertion_message ("%s threw an exception: %p = %s", method, gchandle, [xamarin_print_all_exceptions (gchandle) UTF8String]);
}

#if !defined (NATIVEAOT)
typedef void (*xamarin_runtime_initialize_decl)(struct InitializationOptions* options, GCHandle* exception_gchandle);
void
xamarin_bridge_call_runtime_initialize (struct InitializationOptions* options, GCHandle* exception_gchandle)
{
	void *del = NULL;
	int rv = coreclr_create_delegate (coreclr_handle, coreclr_domainId, PRODUCT ", Version=0.0.0.0", "ObjCRuntime.Runtime", "SafeInitialize", &del);
	if (rv != 0)
		xamarin_assertion_message ("xamarin_bridge_call_runtime_initialize: failed to create delegate: %i\n", rv);

	xamarin_runtime_initialize_decl runtime_initialize = (xamarin_runtime_initialize_decl) del;
	runtime_initialize (options, exception_gchandle);
}
#endif // !defined (NATIVEAOT)

void
xamarin_bridge_register_product_assembly (GCHandle* exception_gchandle)
{
#if !defined (NATIVEAOT)
	MonoAssembly *assembly;
	assembly = xamarin_open_and_register (PRODUCT_DUAL_ASSEMBLY, exception_gchandle);
	xamarin_mono_object_release (&assembly);
#endif // !defined (NATIVEAOT)
}

MonoMethod *
xamarin_bridge_get_mono_method (MonoReflectionMethod *method)
{
	// MonoMethod and MonoReflectionMethod are identical in CoreCLR (both are actually MonoObjects).
	// However, we're returning a retained object, so we need to retain here.
	xamarin_mono_object_retain (method);
	LOG_CORECLR (stderr, "%s (%p): rv: %p\n", __func__, method, method);
	return method;
}

MonoType *
xamarin_get_nsnumber_type ()
{
	// xamarin_bridge_lookup_class returns a MonoClass*, and this method returns a MonoType*,
	// but they're interchangeable for CoreCLR (they're all just MonoObject*s), so this is fine.
	MonoClass *rv = xamarin_bridge_lookup_class (XamarinLookupTypes_Foundation_NSNumber);
	LOG_CORECLR (stderr, "%s () => %p\n", __func__, rv);
	return rv;
}

MonoType *
xamarin_get_nsvalue_type ()
{
	// xamarin_bridge_lookup_class returns a MonoClass*, and this method returns a MonoType*,
	// but they're interchangeable for CoreCLR (they're all just MonoObject*s), so this is fine.
	MonoClass *rv = xamarin_bridge_lookup_class (XamarinLookupTypes_Foundation_NSValue);
	LOG_CORECLR (stderr, "%s () => %p\n", __func__, rv);
	return rv;
}

void
xamarin_mono_object_retain (MonoObject *mobj)
{
	atomic_fetch_add (&mobj->reference_count, 1);
}

void
xamarin_mono_object_release (MonoObject **mobj_ref)
{
	MonoObject *mobj = *mobj_ref;

	if (mobj == NULL)
		return;

	int rc = atomic_fetch_sub (&mobj->reference_count, 1) - 1;
	if (rc == 0) {
		if (mobj->gchandle != INVALID_GCHANDLE) {
			xamarin_gchandle_free (mobj->gchandle);
			mobj->gchandle = INVALID_GCHANDLE;
		}

		xamarin_free (mobj->struct_value); // allocated using Marshal.AllocHGlobal.

		xamarin_free (mobj); // allocated using Marshal.AllocHGlobal.

#if defined (TRACK_MONOOBJECTS)
		if (monoobject_dict != NULL) {
			pthread_mutex_lock (&monoobject_lock);
			CFDictionaryRemoveValue (monoobject_dict, mobj);
			pthread_mutex_unlock (&monoobject_lock);
		}
		atomic_fetch_add (&monoobject_destroyed, 1);
#endif
	}

	*mobj_ref = NULL;
}

void
xamarin_mono_object_release_at_process_exit (MonoObject *mobj)
{
	pthread_mutex_lock (&monoobject_lock);
	release_at_exit = s_list_prepend (release_at_exit, mobj);
	pthread_mutex_unlock (&monoobject_lock);
}

/* Implementation of the Mono Embedding API */

// returns a retained MonoAssembly *
MonoAssembly *
mono_assembly_open (const char * filename, MonoImageOpenStatus * status)
{
	MonoAssembly *rv = xamarin_find_assembly (filename);

	LOG_CORECLR (stderr, "mono_assembly_open (%s, %p) => MonoObject=%p GCHandle=%p\n", filename, status, rv, rv->gchandle);

	if (status != NULL)
		*status = rv == NULL ? MONO_IMAGE_ERROR_ERRNO : MONO_IMAGE_OK;

	return rv;
}

const char *
mono_class_get_namespace (MonoClass * klass)
{
	char *rv = xamarin_bridge_class_get_namespace (klass);

	LOG_CORECLR (stderr, "%s (%p) => %s\n", __func__, klass, rv);

	return rv;
}

const char *
mono_class_get_name (MonoClass * klass)
{
	char *rv = xamarin_bridge_class_get_name (klass);

	LOG_CORECLR (stderr, "%s (%p) => %s\n", __func__, klass, rv);

	return rv;
}

char *
mono_method_full_name (MonoMethod *method, mono_bool signature)
{
	char *rv = xamarin_bridge_get_method_full_name (method);

	LOG_CORECLR (stderr, "%s (%p, %i) => %s\n", __func__, method, signature, rv);

	return rv;
}

MonoDomain *
mono_domain_get (void)
{
	// This is not needed for CoreCLR.
	return NULL;
}

MonoType *
mono_class_get_type (MonoClass *klass)
{
	// MonoClass and MonoType are identical in CoreCLR (both are actually MonoObjects).
	// However, we're returning a retained object, so we need to retain here.
	MonoType *rv = klass;

	xamarin_mono_object_retain (rv);

	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, klass, rv);

	return rv;
}

// returns a retained MonoReflectionAssembly *
MonoReflectionAssembly *
mono_assembly_get_object (MonoDomain * domain, MonoAssembly * assembly)
{
	// MonoAssembly and MonoReflectionAssembly are identical in CoreCLR (both are actually MonoObjects).
	// However, we're returning a retained object, so we need to retain here.
	xamarin_mono_object_retain (assembly);
	LOG_CORECLR (stderr, "mono_assembly_get_object (%p, %p): rv: %p\n", domain, assembly, assembly);
	return assembly;
}

MonoReflectionMethod *
mono_method_get_object (MonoDomain *domain, MonoMethod *method, MonoClass *refclass)
{
	// MonoMethod and MonoReflectionMethod are identical in CoreCLR (both are actually MonoObjects).
	// However, we're returning a retained object, so we need to retain here.
	MonoReflectionMethod *rv = method;

	xamarin_mono_object_retain (rv);

	LOG_CORECLR (stderr, "%s (%p, %p, %p) => %p\n", __func__, domain, method, refclass, rv);

	return rv;
}

MonoType *
mono_reflection_type_get_type (MonoReflectionType *reftype)
{
	// MonoType and MonoReflectionType are identical in CoreCLR (both are actually MonoObjects).
	// However, we're returning a retained object, so we need to retain here.
	MonoType *rv = reftype;
	xamarin_mono_object_retain (rv);
	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, reftype, rv);
	return rv;
}

#if !defined (NATIVEAOT)
int
mono_jit_exec (MonoDomain * domain, MonoAssembly * assembly, int argc, const char** argv)
{
	unsigned int exitCode = 0;

	char *assemblyPath = xamarin_bridge_get_assembly_location (assembly->gchandle);

	if (argc > 0) {
		// The first argument is to the native executable, which we don't want to pass on to native code.
		argc--;
		argv = &argv [1];
	}

	LOG_CORECLR (stderr, "mono_jit_exec (%p, %p, %i, %p) => EXECUTING %s\n", domain, assembly, argc, argv, assemblyPath);
	for (int i = 0; i < argc; i++) {
		LOG_CORECLR (stderr, "    Argument #%i: %s\n", i + 1, argv [i]);
	}

	int rv = coreclr_execute_assembly (coreclr_handle, coreclr_domainId, argc, argv, assemblyPath, &exitCode);

	LOG_CORECLR (stderr, "mono_jit_exec (%p, %p, %i, %p) => EXECUTING %s rv: %i exitCode: %i\n", domain, assembly, argc, argv, assemblyPath, rv, exitCode);

	xamarin_free (assemblyPath);

	if (rv != 0)
		xamarin_assertion_message ("mono_jit_exec failed: %i\n", rv);

	return (int) exitCode;
}
#endif // !defined (NATIVEAOT)

MonoGHashTable *
mono_g_hash_table_new_type (GHashFunc hash_func, GEqualFunc key_equal_func, MonoGHashGCType type)
{
	MonoGHashTable *rv = xamarin_bridge_mono_hash_table_create (hash_func, key_equal_func, type);

	LOG_CORECLR (stderr, "%s (%p, %p, %u) => %p\n", __func__, hash_func, key_equal_func, type, rv);

	return rv;
}

gpointer
mono_g_hash_table_lookup (MonoGHashTable *hash, gconstpointer key)
{
	MonoObject *rv = xamarin_bridge_mono_hash_table_lookup (hash, key);
	LOG_CORECLR (stderr, "%s (%p, %p) => %p\n", __func__, hash, key, rv);
	return rv;
}

void
mono_g_hash_table_insert (MonoGHashTable *hash, gpointer k, gpointer v)
{
	MonoObject *obj = (MonoObject *) v;
	LOG_CORECLR (stderr, "%s (%p, %p, %p)\n", __func__, hash, k, v);
	xamarin_bridge_mono_hash_table_insert (hash, k, obj);
}

MonoClass *
mono_method_get_class (MonoMethod * method)
{
	MonoClass *rv = xamarin_bridge_get_method_declaring_type (method);
	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, method, rv);
	return rv;
}

MonoClass *
mono_object_get_class (MonoObject * obj)
{
	MonoClass *rv = xamarin_bridge_object_get_type (obj);
	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, obj, rv);
	return rv;
}

MonoObject *
mono_object_isinst (MonoObject * obj, MonoClass * klass)
{
	bool rv = xamarin_bridge_isinstance (obj, klass);
	LOG_CORECLR (stderr, "%s (%p, %p) => %i\n", __func__, obj, klass, rv);
	return rv ? obj : NULL;
}

MonoObject *
mono_value_box (MonoDomain *domain, MonoClass *klass, void *val)
{
	MonoObject *rv = xamarin_bridge_box (klass, val);
	LOG_CORECLR (stderr, "%s (%p, %p, %p) => %p\n", __func__, domain, klass, val, rv);
	return rv;
}

void *
mono_object_unbox (MonoObject *obj)
{
	void *rv = obj->struct_value;

	if (rv == NULL)
		xamarin_assertion_message ("%s (%p) => no struct value?\n", __func__);

	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, obj, rv);

	return rv;
}

// Return value: NULL, or a retained MonoObject* that must be freed with xamarin_mono_object_release.
// Returns NULL in case of exception.
MonoObject *
mono_runtime_invoke (MonoMethod * method, void * obj, void ** params, MonoObject ** exc)
{
	MonoObject *rv = NULL;
	GCHandle exception_gchandle = INVALID_GCHANDLE;

	LOG_CORECLR (stderr, "%s (%p, %p, %p, %p)\n", __func__, method, obj, params, exc);

	rv = xamarin_bridge_runtime_invoke_method (method, (MonoObject *) obj, params, &exception_gchandle);

	if (exc == NULL) {
		xamarin_handle_bridge_exception (exception_gchandle, __func__);
	} else {
		*exc = xamarin_gchandle_unwrap (exception_gchandle);
	}

	return rv;
}

MonoException *
xamarin_create_system_exception (const char *message)
{
	MonoException *rv = xamarin_bridge_create_exception (XamarinExceptionTypes_System_Exception, message);
	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, message, rv);
	return rv;
}

MonoException *
xamarin_create_system_invalid_cast_exception (const char *message)
{
	MonoException *rv = xamarin_bridge_create_exception (XamarinExceptionTypes_System_InvalidCastException, message);
	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, message, rv);
	return rv;
}

MonoException *
xamarin_create_system_entry_point_not_found_exception (const char *entrypoint)
{
	MonoException *rv = xamarin_bridge_create_exception (XamarinExceptionTypes_System_EntryPointNotFoundException, entrypoint);
	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, entrypoint, rv);
	return rv;
}

MonoException *
mono_get_exception_out_of_memory ()
{
	MonoException *rv = xamarin_bridge_create_exception (XamarinExceptionTypes_System_OutOfMemoryException, NULL);
	LOG_CORECLR (stderr, "%s () => %p\n", __func__, rv);
	return rv;
}

MonoMethodSignature *
mono_method_signature (MonoMethod* method)
{
	MonoMethodSignature *rv = xamarin_bridge_method_get_signature (method);

	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, method, rv);

	return rv;
}

MonoType *
mono_signature_get_params (MonoMethodSignature* sig, void ** iter)
{
	int* p = (int *) iter;
	if (*p >= sig->parameter_count) {
		LOG_CORECLR (stderr, "%s (%p, %p => %i) => DONE\n", __func__, sig, iter, *p);
		return NULL;
	}

	MonoObject *rv = sig->parameters [*p];
	xamarin_mono_object_retain (rv);

	LOG_CORECLR (stderr, "%s (%p, %p => %i) => %p NEXT\n", __func__, sig, iter, *p, rv->gchandle);

	*p = *p + 1;

	return rv;
}

MonoType *
mono_signature_get_return_type (MonoMethodSignature* sig)
{
	MonoType *rv = sig->return_type;
	xamarin_mono_object_retain (rv);

	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, sig, rv);

	return rv;
}

MonoReflectionType *
mono_type_get_object (MonoDomain *domain, MonoType *type)
{
	MonoReflectionType *rv = type;

	xamarin_mono_object_retain (rv);

	LOG_CORECLR (stderr, "%s (%p, %p) => %p\n", __func__, domain, type, rv);

	return rv;
}

void
xamarin_bridge_free_mono_signature (MonoMethodSignature **psig)
{
	MonoMethodSignature *sig = *psig;

	if (sig == NULL)
		return;

	xamarin_mono_object_release (&sig->method);
	for (int i = 0; i < sig->parameter_count; i++) {
		xamarin_mono_object_release (&sig->parameters [i]);
	}
	xamarin_mono_object_release (&sig->return_type);

	mono_free (sig);

	*psig = NULL;
}

void
mono_free (void *ptr)
{
	free (ptr);
}

mono_bool
mono_thread_detach_if_exiting ()
{
	// Nothing to do here for CoreCLR.
	return true;
}

MonoClass *
mono_class_from_mono_type (MonoType *type)
{
	MonoClass *rv = xamarin_bridge_type_to_class (type);
	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, type, rv);
	return rv;
}

MonoClass *
mono_get_string_class ()
{
	MonoClass *rv = xamarin_bridge_lookup_class (XamarinLookupTypes_System_String);
	LOG_CORECLR (stderr, "%s () => %p.\n", __func__, rv);
	return rv;
}

mono_bool
mono_class_is_enum (MonoClass *klass)
{
	bool rv = xamarin_bridge_is_enum (klass);

	LOG_CORECLR (stderr, "%s (%p) => %i\n", __func__, klass, rv);

	return rv;
}

MonoType *
mono_class_enum_basetype (MonoClass *klass)
{
	MonoType *rv = xamarin_bridge_get_enum_basetype (klass);

	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, klass, rv);

	return rv;
}

mono_bool
mono_type_is_byref (MonoType *type)
{
	bool rv = xamarin_bridge_is_byref (type);

	LOG_CORECLR (stderr, "%s (%p) => %i\n", __func__, type, rv);

	return rv;
}

mono_bool
mono_class_is_delegate (MonoClass *klass)
{
	bool rv = xamarin_bridge_is_delegate (klass);
	LOG_CORECLR (stderr, "%s (%p) => %i\n", __func__, klass, rv);
	return rv;
}

mono_bool
mono_class_is_valuetype (MonoClass * klass)
{
	bool rv = xamarin_bridge_is_valuetype (klass);

	LOG_CORECLR (stderr, "%s (%p) => %i\n", __func__, klass, rv);

	return rv;
}

int32_t
mono_class_value_size (MonoClass *klass, uint32_t *align)
{
	int32_t rv = xamarin_bridge_sizeof (klass);

	LOG_CORECLR (stderr, "%s (%p, %p) => %i\n", __func__, klass, align, rv);

	return rv;
}

gboolean
mono_class_is_nullable (MonoClass * klass)
{
	bool rv = xamarin_bridge_is_nullable (klass);

	LOG_CORECLR (stderr, "%s (%p) => %i\n", __func__, klass, rv);

	return rv;
}

MonoClass *
mono_class_get_element_class (MonoClass *klass)
{
	MonoClass *rv = xamarin_bridge_get_element_class (klass);
	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, klass, rv);
	return rv;
}

MonoClass *
mono_class_get_nullable_param (MonoClass * klass)
{
	MonoClass *rv = xamarin_bridge_get_nullable_element_type (klass);
	LOG_CORECLR (stderr, "%s (%p) => %p\n", __func__, klass, rv);
	return rv;
}

bool
xamarin_is_class_nsobject (MonoClass *cls)
{
	return xamarin_bridge_is_class_of_type (cls, XamarinLookupTypes_Foundation_NSObject);
}

bool
xamarin_is_class_inativeobject (MonoClass *cls)
{
	return xamarin_bridge_is_class_of_type (cls, XamarinLookupTypes_ObjCRuntime_INativeObject);
}

bool
xamarin_is_class_nativehandle (MonoClass *cls)
{
	return xamarin_bridge_is_class_of_type (cls, XamarinLookupTypes_ObjCRuntime_NativeHandle);
}

bool
xamarin_is_class_array (MonoClass *cls)
{
	return xamarin_bridge_is_class_of_type (cls, XamarinLookupTypes_System_Array);
}

bool
xamarin_is_class_nsnumber (MonoClass *cls)
{
	return xamarin_bridge_is_class_of_type (cls, XamarinLookupTypes_Foundation_NSNumber);
}

bool
xamarin_is_class_nsvalue (MonoClass *cls)
{
	return xamarin_bridge_is_class_of_type (cls, XamarinLookupTypes_Foundation_NSValue);
}

bool
xamarin_is_class_nsstring (MonoClass *cls)
{
	return xamarin_bridge_is_class_of_type (cls, XamarinLookupTypes_Foundation_NSString);
}

bool
xamarin_is_class_intptr (MonoClass *cls)
{
	return xamarin_bridge_is_class_of_type (cls, XamarinLookupTypes_System_IntPtr);
}

bool
xamarin_is_class_string (MonoClass *cls)
{
	return xamarin_bridge_is_class_of_type (cls, XamarinLookupTypes_System_String);
}

MonoArray *
mono_array_new (MonoDomain *domain, MonoClass *eclass, uintptr_t n)
{
	MonoArray *rv = xamarin_bridge_create_array (eclass, n);
	LOG_CORECLR (stderr, "%s (%p, %p, %" PRIdPTR ") => %p\n", __func__, domain, eclass, n, rv);
	return rv;
}

uintptr_t
mono_array_length (MonoArray *array)
{
	uintptr_t rv = (uintptr_t) xamarin_bridge_get_array_length (array);
	LOG_CORECLR (stderr, "%s (%p) => %llu\n", __func__, array, (uint64_t) rv);
	return rv;
}

char *
mono_string_to_utf8 (MonoString *string_obj)
{
	char *rv = xamarin_bridge_string_to_utf8 (string_obj);

	LOG_CORECLR (stderr, "%s (%p) => %s\n", __func__, string_obj, rv);

	return rv;
}

MonoString *
mono_string_new (MonoDomain *domain, const char *text)
{
	MonoString *rv = xamarin_bridge_new_string (text);

	LOG_CORECLR (stderr, "%s (%p, %s) => %p\n", __func__, domain, text, rv);

	return rv;
}

#endif // CORECLR_RUNTIME
