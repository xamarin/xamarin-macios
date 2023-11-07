#include <stdint.h>
#include <stdio.h>

#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>

#include "xamarin/xamarin.h"
#include "runtime-internal.h"
#include "trampolines-internal.h"
#include "slinked-list.h"
#include "delegates.h"
#include "product.h"

#if defined(CORECLR_RUNTIME)
#define ADD_TO_MONOOBJECT_RELEASE_LIST(item) release_list = s_list_prepend (release_list, item)
#else
#define ADD_TO_MONOOBJECT_RELEASE_LIST(item)
#endif

static GCHandle
xamarin_get_exception_for_method (int code, GCHandle inner_exception_gchandle, const char *reason, SEL sel, id self)
{
	GCHandle exception_gchandle = INVALID_GCHANDLE;
	char *msg = xamarin_strdup_printf ("%s\n"
		"Additional information:\n"
		"\tSelector: %s\n"
		"\tType: %s\n", reason, sel_getName (sel), class_getName ([self class]));
	exception_gchandle = xamarin_create_product_exception_with_inner_exception (code, inner_exception_gchandle, msg);
	xamarin_free (msg);
	return exception_gchandle;
}

GCHandle
xamarin_get_exception_for_parameter (int code, GCHandle inner_exception_gchandle, const char *reason, SEL sel, MonoMethod *method, MonoType *p, int i, bool to_managed)
{
	GCHandle exception_gchandle = INVALID_GCHANDLE;
	char *to_name = xamarin_type_get_full_name (p, &exception_gchandle);
	if (exception_gchandle != INVALID_GCHANDLE)
		return exception_gchandle;
	char *method_full_name = mono_method_full_name (method, TRUE);
	char *msg = xamarin_strdup_printf ("%s #%i whose managed type is '%s' %s.\n"
		"Additional information:\n"
		"\tSelector: %s\n"
		"\tMethod: %s\n", reason, i + 1, to_name, to_managed ? "to managed" : "to Objective-C", sel_getName (sel), method_full_name);
	exception_gchandle = xamarin_create_product_exception_with_inner_exception (code, inner_exception_gchandle, msg);
	xamarin_free (msg);
	xamarin_free (to_name);
	xamarin_free (method_full_name);
	return exception_gchandle;
}

NSString *
xamarin_string_to_nsstring (MonoString *obj, bool retain)
{
	if (obj == NULL)
		return NULL;

	char *str = mono_string_to_utf8 (obj);
	NSString *arg;
	if (retain) {
		arg = [[NSString alloc] initWithUTF8String:str];
	} else {
		arg = [NSString stringWithUTF8String:str];
	}
	mono_free (str);
	return arg;
}

MonoString *
xamarin_nsstring_to_string (MonoDomain *domain, NSString *obj)
{
	if (obj == NULL)
		return NULL;

	if (domain == NULL)
		domain = mono_domain_get ();
	return mono_string_new (domain, [obj UTF8String]);
}

void
xamarin_invoke_trampoline (enum TrampolineType type, id self, SEL sel, iterator_func iterator, marshal_return_value_func marshal_return_value, void *context)
{
	// COOP: No managed data in input, but accesses managed data.
	// COOP: FIXME: This method needs a lot of work when the runtime team
	//       implements a handle api for mono objects.
	//       Random notes:
	//       * Must switch to SAFE mode when calling any external code.
	//       * mono_runtime_invoke will have to change, since 'out/ref'
	//         objects arguments are now put into the arg_ptrs array
	//         (clearly not GC-safe upon return).
	MONO_ASSERT_GC_SAFE_OR_DETACHED;

	MonoObject *exception = NULL;
	MonoObject **exception_ptr = xamarin_is_managed_exception_marshaling_disabled () ? NULL : &exception;
	GCHandle exception_gchandle = INVALID_GCHANDLE;
	bool is_static = (type & Tramp_Static) == Tramp_Static;
	bool is_ctor = type == Tramp_Ctor;
	const char *ret_type = NULL;
	MonoType *sig_ret_type = NULL;

	if (is_ctor) {
		bool has_nsobject = xamarin_has_nsobject (self, &exception_gchandle);
		if (exception_gchandle != INVALID_GCHANDLE) {
			xamarin_process_managed_exception_gchandle (exception_gchandle);
			return; // we shouldn't get here.
		}

		if (has_nsobject) {
			self = xamarin_invoke_objc_method_implementation (self, sel, (IMP) xamarin_ctor_trampoline);
			marshal_return_value (context, "|", __SIZEOF_POINTER__, self, NULL, false, NULL, NULL, &exception_gchandle);
			xamarin_process_managed_exception_gchandle (exception_gchandle);
			return;
		}
	}

	MONO_THREAD_ATTACH; // COOP: This will swith to GC_UNSAFE

	MonoDomain *domain = mono_domain_get ();

	// pre-prolog
	SList *dispose_list = NULL;
	SList *free_list = NULL;
	SList *release_list = NULL; // list of MonoObject*'s to release at the end.
	unsigned long num_arg;
	unsigned long managed_arg_count;
	NSMethodSignature *sig;

	if (is_static) {
		sig = [NSMethodSignature signatureWithObjCTypes: method_getTypeEncoding (class_getClassMethod (self, sel))];
	} else {
		sig = [self methodSignatureForSelector: sel];
	}
	num_arg = [sig numberOfArguments] - 2;

	// prolog
	MonoObject *mthis = NULL;
	MethodDescription *desc = NULL;
	MonoMethod *method;
	MonoMethodSignature *msig = NULL;
	MonoReflectionMethod *reflection_method = NULL;
	int semantic;
	bool isCategoryInstance;

	// setup callstack
	unsigned long frame_length;
	void **arg_frame;
	void **arg_ptrs;
	void **arg_copy = NULL; // used to detect if ref/out parameters were changed.
	bool *writeback = NULL; // used to detect if a particular parameter is ref/out parameter.
	void *iter = NULL;
	gboolean needs_writeback = FALSE; // determines if there are any ref/out parameters.
	MonoType *p;
	int ofs;
	unsigned long i;
	unsigned long mofs = 0;

	unsigned long desc_arg_count = num_arg + 2; /* 1 for the return value + 1 if this is a category instance method */
	size_t desc_size = desc_arg_count * sizeof (BindAsData) + sizeof (MethodDescription);
	desc = (MethodDescription *) xamarin_calloc (desc_size);
	desc->bindas_count = (int32_t) desc_arg_count;
	free_list = s_list_prepend (free_list, desc);

	if (is_ctor || is_static) {
		xamarin_get_method_for_selector ([self class], sel, is_static, desc, &exception_gchandle);
	} else {
		GCHandle mthis_handle = INVALID_GCHANDLE;
		xamarin_get_method_and_object_for_selector ([self class], sel, is_static, self, &mthis_handle, desc, &exception_gchandle);
		mthis = xamarin_gchandle_get_target (mthis_handle);
		ADD_TO_MONOOBJECT_RELEASE_LIST (mthis);
		xamarin_gchandle_free (mthis_handle);
	}
	if (exception_gchandle != INVALID_GCHANDLE) {
		exception_gchandle = xamarin_get_exception_for_method (8034, exception_gchandle, "Failed to lookup the required marshalling information.", sel, self);
		goto exception_handling;
	}

	reflection_method = (MonoReflectionMethod *) xamarin_gchandle_get_target (desc->method_handle);
	ADD_TO_MONOOBJECT_RELEASE_LIST (reflection_method);
	method = xamarin_get_reflection_method_method (reflection_method);
	ADD_TO_MONOOBJECT_RELEASE_LIST (method);
	msig = mono_method_signature (method);
	semantic = desc->semantic & ArgumentSemanticMask;
	isCategoryInstance = (desc->semantic & ArgumentSemanticCategoryInstance) == ArgumentSemanticCategoryInstance;

	frame_length = [sig frameLength] - (sizeof (void *) * (isCategoryInstance ? 1 : 2));
	arg_frame = (void **) alloca (frame_length);
	managed_arg_count = num_arg + (isCategoryInstance ? 1 : 0);
	arg_ptrs = (void **) alloca (sizeof (void *) * managed_arg_count);
	
#ifdef TRACE
	memset (arg_ptrs, 0xDEADF00D, num_arg * sizeof (void*));
#endif

	if (isCategoryInstance) {
		// we know this must be an id
		p = mono_signature_get_params (msig, &iter);
		ADD_TO_MONOOBJECT_RELEASE_LIST (p);
		MonoObject* catobj = xamarin_get_nsobject_with_type_for_ptr (self, false, p, &exception_gchandle);
		if (exception_gchandle != INVALID_GCHANDLE) {
			exception_gchandle = xamarin_get_exception_for_parameter (8029, exception_gchandle, "Unable to marshal the parameter", sel, method, p, 0, true);
			goto exception_handling;
		}
		arg_ptrs [0] = catobj;
		ADD_TO_MONOOBJECT_RELEASE_LIST (catobj);
		mofs = 1;
	}

	iterator (IteratorStart, context, NULL, 0, NULL, &exception_gchandle); // start
	if (exception_gchandle != INVALID_GCHANDLE)
		goto exception_handling;

	for (i = 0, ofs = 0; i < num_arg; i++) {
		const char *type = xamarin_skip_encoding_flags ([sig getArgumentTypeAtIndex: (i+2)]);
		unsigned long size = xamarin_objc_type_size (type);
		int frameofs = ofs;
		p = mono_signature_get_params (msig, &iter);
		ADD_TO_MONOOBJECT_RELEASE_LIST (p);
		
#if __SIZEOF_POINTER__ == 4
		if (*type == 'i' || *type == 'I') {
			// this might be a [Native] enum, in which case managed code expects a 64-bit value.

			MonoClass *p_klass = mono_class_from_mono_type (p);
			bool is_native_enum = mono_class_is_enum (p_klass) && mono_class_value_size (p_klass, NULL) == 8;
			xamarin_mono_object_release (&p_klass);
			if (is_native_enum) {
				// Don't bother checking for the attribute (it's quite expensive),
				// just check whether managed code expects a 64-bit value and assume
				// we end up in this condition because it's a [Native] enum.

				iterator (IteratorIterate, context, type, size, &arg_frame [ofs++], &exception_gchandle);
				if (exception_gchandle != INVALID_GCHANDLE)
					goto exception_handling;
				arg_frame [ofs++] = 0;
				arg_ptrs [i + mofs] = &arg_frame [frameofs];
				continue;
			}
		}
#endif
		
		if (size > sizeof (void *)) {
			iterator (IteratorIterate, context, type, size, &arg_frame [ofs], &exception_gchandle);
			if (exception_gchandle != INVALID_GCHANDLE)
				goto exception_handling;
			ofs += size / sizeof (void *);
			arg_ptrs [i + mofs] = &arg_frame [frameofs];
		} else {
			void *arg;
			iterator (IteratorIterate, context, type, size, &arg, &exception_gchandle);
			if (exception_gchandle != INVALID_GCHANDLE)
				goto exception_handling;
			if (desc->bindas [i + 1].original_type_handle != INVALID_GCHANDLE) {
				MonoReflectionType *original_type = (MonoReflectionType *) xamarin_gchandle_get_target (desc->bindas [i + 1].original_type_handle);
				ADD_TO_MONOOBJECT_RELEASE_LIST (original_type);
				MonoType *original_mono_type = mono_reflection_type_get_type (original_type);
				ADD_TO_MONOOBJECT_RELEASE_LIST (original_mono_type);
				arg_ptrs [i + mofs] = xamarin_generate_conversion_to_managed ((id) arg, original_mono_type, p, method, &exception_gchandle, (void *) INVALID_TOKEN_REF, (void **) &free_list, (void **) &release_list);
				if (exception_gchandle != INVALID_GCHANDLE)
					goto exception_handling;
				ofs++;
				continue;
			}
			switch (type [0]) {
				case _C_PTR: {
					switch (type [1]) {
						case _C_CLASS:
						case _C_SEL:
						case _C_ID: {
							MonoClass *p_klass = mono_class_from_mono_type (p);
							ADD_TO_MONOOBJECT_RELEASE_LIST (p_klass);
							if (!mono_type_is_byref (p)) {
								GCHandle ex_handle = xamarin_create_runtime_exception (8040, "Invalid type encoding for parameter", &exception_gchandle);
								if (exception_gchandle == INVALID_GCHANDLE)
									exception_gchandle = ex_handle;
								goto exception_handling;
							}
							MonoReflectionMethod *rmethod = mono_method_get_object (domain, method, NULL);
							bool is_parameter_out = xamarin_is_parameter_out (rmethod, (int) i, &exception_gchandle);
							ADD_TO_MONOOBJECT_RELEASE_LIST (rmethod);
							if (exception_gchandle != INVALID_GCHANDLE)
								goto exception_handling;

							if (!needs_writeback) {
								needs_writeback = TRUE;
								arg_copy = (void **) calloc (managed_arg_count, sizeof (void *));
								writeback = (bool *) calloc (managed_arg_count, sizeof (bool));
								if (!arg_copy || !writeback) {
									exception = (MonoObject *) mono_get_exception_out_of_memory ();
									goto exception_handling;
								}
							}
							arg_frame [ofs] = NULL;
							arg_ptrs [i + mofs] = &arg_frame [frameofs];
							writeback [i + mofs] = TRUE;
							if (is_parameter_out) {
								LOGZ (" argument %i is an out parameter. Passing in a pointer to a NULL value.\n", i + 1);
								if (arg != NULL) {
									// Write NULL to the argument we got right away. Managed code might not write to it (or write NULL),
									// in which case our code to detect if the value changed will say it didn't and not copy it back.
									*(NSObject **) arg = NULL;
								}
							} else if (xamarin_is_class_nsobject (p_klass)) {
								MonoObject *mobj = xamarin_get_nsobject_with_type_for_ptr (*(NSObject **) arg, false, p, &exception_gchandle);
								if (exception_gchandle != INVALID_GCHANDLE) {
									exception_gchandle = xamarin_get_exception_for_parameter (8029, exception_gchandle, "Unable to marshal the byref parameter", sel, method, p, (int) i, true);
									goto exception_handling;
								}
								arg_frame [ofs] = mobj;
								ADD_TO_MONOOBJECT_RELEASE_LIST (mobj);
								LOGZ (" argument %i is a ref NSObject parameter: %p = %p\n", i + 1, arg, arg_frame [ofs]);
							} else if (xamarin_is_class_inativeobject (p_klass)) {
								MonoReflectionType *reflectionp = mono_type_get_object (domain, p);
								ADD_TO_MONOOBJECT_RELEASE_LIST (reflectionp);
								MonoObject *mobj = xamarin_get_inative_object_dynamic (*(NSObject **) arg, false, reflectionp, &exception_gchandle);
								if (exception_gchandle != INVALID_GCHANDLE)
									goto exception_handling;
								arg_frame [ofs] = mobj;
								ADD_TO_MONOOBJECT_RELEASE_LIST (mobj);
								LOGZ (" argument %i is a ref ptr/INativeObject %p: %p\n", i + 1, arg, arg_frame [ofs]);
							} else if (xamarin_is_class_string (p_klass)) {
								MonoString *mstr = xamarin_nsstring_to_string (domain, *(NSString **) arg);
								arg_frame [ofs] = mstr;
								ADD_TO_MONOOBJECT_RELEASE_LIST (mstr);
								LOGZ (" argument %i is a ref NSString %p: %p\n", i + 1, arg, arg_frame [ofs]);
							} else if (xamarin_is_class_array (p_klass)) {
								MonoArray *array_arg = xamarin_nsarray_to_managed_array (*(NSArray **) arg, p, p_klass, &exception_gchandle);
								arg_frame [ofs] = array_arg;
								ADD_TO_MONOOBJECT_RELEASE_LIST (array_arg);
								if (exception_gchandle != INVALID_GCHANDLE) {
									exception_gchandle = xamarin_get_exception_for_parameter (8029, exception_gchandle, "Unable to marshal the byref parameter", sel, method, p, (int) i, true);
									goto exception_handling;
								}
								LOGZ (" argument %i is ref NSArray (%p => %p => %p)\n", i + 1, arg, *(NSArray **) arg, arg_frame [ofs]);
							} else {
								exception_gchandle = xamarin_get_exception_for_parameter (8029, 0, "Unable to marshal the byref parameter", sel, method, p, (int) i, true);
								goto exception_handling;
							}
							arg_copy [i + mofs] = arg_frame [ofs];
							LOGZ (" argument %i's value: %p\n", i + 1, arg_copy [i + mofs]);
							break;
						}
						case _C_PTR: {
							if (mono_type_is_byref (p)) {
								arg_ptrs [i + mofs] = arg;
							} else {
								arg_frame [ofs] = arg;
								arg_ptrs [i + mofs] = &arg_frame [frameofs];
							}
							break;
						}
						default: {
							MonoClass *p_klass = mono_class_from_mono_type (p);
							ADD_TO_MONOOBJECT_RELEASE_LIST  (p_klass);
							if (mono_class_is_delegate (p_klass)) {
								MonoObject *del = xamarin_get_delegate_for_block_parameter (method, INVALID_TOKEN_REF, (int) i, arg, &exception_gchandle);
								if (exception_gchandle != INVALID_GCHANDLE)
									goto exception_handling;
								arg_ptrs [i + mofs] = del;
								ADD_TO_MONOOBJECT_RELEASE_LIST (del);
							} else if (xamarin_is_class_inativeobject (p_klass)) {
								id id_arg = (id) arg;
								if (semantic == ArgumentSemanticCopy) {
									id_arg = [id_arg copy];
									[id_arg autorelease];
								}
								MonoObject *obj;
								MonoReflectionType *reflectionp = mono_type_get_object (domain, p);
								ADD_TO_MONOOBJECT_RELEASE_LIST (reflectionp);
								obj = xamarin_get_inative_object_dynamic (id_arg, false, reflectionp, &exception_gchandle);
								if (exception_gchandle != INVALID_GCHANDLE)
									goto exception_handling;
								ADD_TO_MONOOBJECT_RELEASE_LIST (obj);
								LOGZ (" argument %i is ptr/INativeObject %p: %p\n", i + 1, id_arg, obj);
								arg_ptrs [i + mofs] = obj;
							} else {
								arg_frame [ofs] = arg;
								if (mono_type_is_byref (p)) {
									arg_ptrs [i + mofs] = arg;
								} else if (mono_class_is_valuetype (p_klass)) {
									arg_ptrs [i + mofs] = &arg_frame [frameofs];
								} else {
									arg_ptrs [i + mofs] = arg;
								}
							}
							break;
						}
					}
					break;
				}
				case _C_CLASS: {
					if (!arg) {
						arg_ptrs [i + mofs] = NULL;
						LOGZ (" argument %i is Class: NULL\n", i + 1);
						break;
					} else {
						MonoObject *mclass = xamarin_get_class ((Class) arg, &exception_gchandle);
						arg_ptrs [i + mofs] = mclass;
						ADD_TO_MONOOBJECT_RELEASE_LIST (mclass);
						if (exception_gchandle != INVALID_GCHANDLE)
							goto exception_handling;
						LOGZ (" argument %i is Class: %p = %s\n", i + 1, arg, class_getName ((Class) arg));
						break;
					}
				}
				case _C_SEL: {
					if (!arg) {
						arg_ptrs [i + mofs] = NULL;
						LOGZ (" argument %i is SEL: NULL\n", i + 1);
						break;
					} else {
						MonoObject *msel = xamarin_get_selector ((SEL) arg, &exception_gchandle);
						arg_ptrs [i + mofs] = msel;
						ADD_TO_MONOOBJECT_RELEASE_LIST (msel);
						if (exception_gchandle != INVALID_GCHANDLE)
							goto exception_handling;
						LOGZ (" argument %i is SEL: %p = %s\n", i + 1, arg, sel_getName ((SEL) arg));
						break;
					}
				}
				case _C_CHARPTR: {
					if (!arg) {
						arg_ptrs [i + mofs] = NULL;
						LOGZ (" argument %i is char*: NULL\n", i + 1);
						break;
					} else {
						MonoString *mstr = mono_string_new (domain, (const char *) arg);
						arg_ptrs [i + mofs] = (void *) mstr;
						ADD_TO_MONOOBJECT_RELEASE_LIST (mstr);
						LOGZ (" argument %i is char*: %p = %s\n", i + 1, arg, arg);
						break;
					}
				}
				case _C_ID: {
					id id_arg = (id) arg;
					MonoClass *p_klass = mono_class_from_mono_type (p);
					ADD_TO_MONOOBJECT_RELEASE_LIST (p_klass);
					if (xamarin_is_class_intptr (p_klass)) {
						arg_frame [ofs] = id_arg;
						arg_ptrs [i + mofs] = &arg_frame [frameofs];
						LOGZ (" argument %i is IntPtr: %p\n", i + 1, id_arg);
						break;
#if DOTNET
					} else if (xamarin_is_class_nativehandle (p_klass)) {
						arg_frame [ofs] = id_arg;
						arg_ptrs [i + mofs] = &arg_frame [frameofs];
						LOGZ (" argument %i is NativeHandle: %p\n", i + 1, id_arg);
						break;
#endif
					} else if (!id_arg) {
						arg_ptrs [i + mofs] = NULL;
						break;
					} else {
						if (xamarin_is_class_string (p_klass)) {
							NSString *str = (NSString *) id_arg;
							MonoString *mstr = xamarin_nsstring_to_string (domain, str);
							arg_ptrs [i + mofs] = mstr;
							ADD_TO_MONOOBJECT_RELEASE_LIST (mstr);
							LOGZ (" argument %i is NSString: %p = %s\n", i + 1, id_arg, [str UTF8String]);
						} else if (xamarin_is_class_array (p_klass)) {
							MonoArray *array_arg = xamarin_nsarray_to_managed_array ((NSArray *) id_arg, p, p_klass, &exception_gchandle);
							arg_ptrs [i + mofs] = array_arg;
							ADD_TO_MONOOBJECT_RELEASE_LIST (array_arg);
							if (exception_gchandle != INVALID_GCHANDLE) {
								exception_gchandle = xamarin_get_exception_for_parameter (8029, exception_gchandle, "Unable to marshal the array parameter", sel, method, p, (int) i, true);
								goto exception_handling;
							}
							LOGZ (" argument %i is NSArray\n", i + 1);
						} else if (xamarin_is_class_nsobject (p_klass)) {
							if (semantic == ArgumentSemanticCopy) {
								id_arg = [id_arg copy];
								[id_arg autorelease];
							}
							MonoObject *obj;
							int32_t created = false;
							obj = xamarin_get_nsobject_with_type_for_ptr_created (id_arg, false, p, &created, &exception_gchandle);
							if (exception_gchandle != INVALID_GCHANDLE) {
								exception_gchandle = xamarin_get_exception_for_parameter (8029, exception_gchandle, "Unable to marshal the parameter", sel, method, p, (int) i, true);
								goto exception_handling;
							}
							ADD_TO_MONOOBJECT_RELEASE_LIST (obj);

							if (created && obj) {
								MonoReflectionMethod *rmethod = mono_method_get_object (domain, method, NULL);
								bool is_transient = xamarin_is_parameter_transient (rmethod, (int32_t) i, &exception_gchandle);
								ADD_TO_MONOOBJECT_RELEASE_LIST (rmethod);
								if (exception_gchandle != INVALID_GCHANDLE)
									goto exception_handling;
								if (is_transient)
									dispose_list = s_list_prepend (dispose_list, obj);
							}
#if DEBUG
							xamarin_verify_parameter (obj, sel, self, id_arg, i, p_klass, method);
#endif
							LOGZ (" argument %i is NSObject %p: %p\n", i + 1, id_arg, obj);
							arg_ptrs [i + mofs] = obj;
						} else if (xamarin_is_class_inativeobject (p_klass)) {
							if (semantic == ArgumentSemanticCopy) {
								id_arg = [id_arg copy];
								[id_arg autorelease];
							}
							MonoObject *obj;
							MonoReflectionType *reflectionp = mono_type_get_object (domain, p);
							ADD_TO_MONOOBJECT_RELEASE_LIST (reflectionp);
							obj = xamarin_get_inative_object_dynamic (id_arg, false, reflectionp, &exception_gchandle);
							if (exception_gchandle != INVALID_GCHANDLE)
								goto exception_handling;
							ADD_TO_MONOOBJECT_RELEASE_LIST (obj);
							LOGZ (" argument %i is NSObject/INativeObject %p: %p\n", i + 1, id_arg, obj);
							arg_ptrs [i + mofs] = obj;
						} else if (mono_class_is_delegate (p_klass)) {
							MonoObject *del = xamarin_get_delegate_for_block_parameter (method, INVALID_TOKEN_REF, (int) i, id_arg, &exception_gchandle);
							if (exception_gchandle != INVALID_GCHANDLE)
								goto exception_handling;
							arg_ptrs [i + mofs] = del;
							ADD_TO_MONOOBJECT_RELEASE_LIST (del);
						} else {
							if (semantic == ArgumentSemanticCopy) {
								id_arg = [id_arg copy];
								[id_arg autorelease];
							}
							MonoObject *obj;
							obj = xamarin_get_nsobject_with_type_for_ptr (id_arg, false, p, &exception_gchandle);
							if (exception_gchandle != INVALID_GCHANDLE)
								goto exception_handling;
							ADD_TO_MONOOBJECT_RELEASE_LIST (obj);
#if DEBUG
							xamarin_verify_parameter (obj, sel, self, id_arg, i, p_klass, method);
#endif
							arg_ptrs [i + mofs] = obj;
						}
						break;
					}
				}
				default:
					arg_frame [ofs] = arg;
					arg_ptrs [i + mofs] = &arg_frame [frameofs];
					break;
			}
			ofs++;
		}
	}
	
	iterator (IteratorEnd, context, NULL, 0, NULL, &exception_gchandle);
	if (exception_gchandle != INVALID_GCHANDLE)
		goto exception_handling;
	
	// invoke
	MonoObject *retval;
	MonoObject *ctorval __attribute__((unused));
	if (is_ctor) {
		/* 
		 * Some Objective-C classes overwrite retain, release,
		 * retainCount and those methods are not operational
		 * until one of the init methods has been called
		 * (CALayer for example).  This means that the standard
		 * code path that we have to Retain in NSObject does not
		 * work for objects surfaced through xamarin_ctor_trampoline.
		 * 
		 * To work around this, we will perform the retain here, after
		 * managed code has initialized the NSObject.   To let managed
		 * code know to not invoke the Retain in InitializeObject we set
		 * the NativeRef flag on the object.
		 *
		 * This is checked by NSObject.InitializeObject and it
		 * instruct the method to not attempt to call `retain'
		 * on the object
		 * 
		 * Instead, when surfacing objects by
		 * xamarin_ctor_trampoline, we perform the retain on
		 * behalf of managed code here (managed code owns one
		 * reference, and unmanaged code the other).
		 *
		 * This problem is documented in the following bug:
		 * https://bugzilla.xamarin.com/show_bug.cgi?id=6556
		 */
		MonoClass *declaring_type = mono_method_get_class (method);
		ADD_TO_MONOOBJECT_RELEASE_LIST (declaring_type);

		retval = xamarin_new_nsobject (self, declaring_type, &exception_gchandle);
		if (exception_gchandle != INVALID_GCHANDLE)
			goto exception_handling;
		ADD_TO_MONOOBJECT_RELEASE_LIST (retval);

		ctorval = mono_runtime_invoke (method, retval, (void **) arg_ptrs, exception_ptr);
		if (exception != NULL)
			goto exception_handling;
		ADD_TO_MONOOBJECT_RELEASE_LIST (ctorval);
	} else {
		
#ifdef TRACE
		fprintf (stderr, " calling managed method with %i arguments: ", num_arg);
		for (int i = 0; i < num_arg; i++)
			fprintf (stderr, "%p ", arg_ptrs [i]);
		fprintf (stderr, " writeback: %i", needs_writeback);
		fprintf (stderr, "\n");
#endif

		retval = mono_runtime_invoke (method, mthis, (void **) arg_ptrs, exception_ptr);

#ifdef TRACE
		fprintf (stderr, " called managed method with %i arguments: ", num_arg);
		for (int i = 0; i < num_arg; i++)
			fprintf (stderr, "%p ", arg_ptrs [i]);
		fprintf (stderr, " writeback: %i", needs_writeback);
		fprintf (stderr, "\n");
#endif

		if (exception != NULL)
			goto exception_handling;
		ADD_TO_MONOOBJECT_RELEASE_LIST (retval);
	}

	// writeback
	if (needs_writeback) {
		iter = NULL;
		iterator (IteratorStart, context, NULL, 0, NULL, &exception_gchandle); // start
		if (exception_gchandle != INVALID_GCHANDLE)
			goto exception_handling;
		for (i = 0; i < num_arg; i++) {
			const char *type = [sig getArgumentTypeAtIndex: (i+2)];

			type = xamarin_skip_encoding_flags (type);

			unsigned long size = xamarin_objc_type_size (type);

			p = mono_signature_get_params (msig, &iter);
			ADD_TO_MONOOBJECT_RELEASE_LIST (p);

			// Skip over any structs. In any case they can't be write-back parameters.
			// Also skip over anything we're not supposed to write back to.
			bool skip = size > sizeof (void *) || !writeback [i + mofs];
			void *arg;
			iterator (IteratorIterate, context, type, size, skip ? NULL : &arg, &exception_gchandle);
			if (exception_gchandle != INVALID_GCHANDLE)
				goto exception_handling;
			if (skip) {
				LOGZ (" skipping argument %i (size: %i, pointer: %p)\n", i, size, arg_copy [i + mofs]);
				continue;
			}

			if (arg == NULL) {
				LOGZ (" not writing back to a null pointer\n");
				continue;
			}

			if (type [0] == _C_PTR && (type [1] == _C_ID || type [1] == _C_SEL || type [1] == _C_CLASS)) {
				MonoClass *p_klass = mono_class_from_mono_type (p);
				ADD_TO_MONOOBJECT_RELEASE_LIST (p_klass);
				MonoObject *value = *(MonoObject **) arg_ptrs [i + mofs];
				MonoObject *pvalue = (MonoObject *) arg_copy [i + mofs];
				NSObject *obj = NULL;

				ADD_TO_MONOOBJECT_RELEASE_LIST (value);

				if (value == pvalue) {
					// No need to copy back if the value didn't change
					// For arrays this means that we won't copy back arrays if an element in the array changed (which can happen in managed code: the array pointer and size are immutable, but array elements can change).
					// If the developer wants to change an array element, a new array must be created and assigned to the ref parameter.
					// This is by design: otherwise we'll have to copy arrays even though they didn't change (because we can't know if they changed without comparing every element).
					LOGZ (" not writing back managed object to argument at index %i (%p => %p) because it didn't change\n", i, arg, value);
					continue;
				} else if (value == NULL) {
					LOGZ (" writing back null to argument at index %i (%p)\n", i + 1, arg);
				} else if (xamarin_is_class_string (p_klass)) {
					obj = xamarin_string_to_nsstring ((MonoString *) value, false);
					LOGZ (" writing back managed string %p to argument at index %i (%p)\n", value, i + 1, arg);
				} else if (xamarin_is_class_nsobject (p_klass)) {
					obj = xamarin_get_handle (value, &exception_gchandle);
					if (exception_gchandle != INVALID_GCHANDLE)
						goto exception_handling;
					LOGZ (" writing back managed NSObject %p to argument at index %i (%p)\n", value, i + 1, arg);
				} else if (xamarin_is_class_inativeobject (p_klass)) {
					obj = xamarin_get_handle_for_inativeobject (value, &exception_gchandle);
					if (exception_gchandle != INVALID_GCHANDLE)
						goto exception_handling;
					LOGZ (" writing back managed INativeObject %p to argument at index %i (%p)\n", value, i + 1, arg);
				} else if (xamarin_is_class_array (p_klass)) {
					obj = xamarin_managed_array_to_nsarray ((MonoArray *) value, p, p_klass, &exception_gchandle);
					if (exception_gchandle != INVALID_GCHANDLE) {
						exception_gchandle = xamarin_get_exception_for_parameter (8030, exception_gchandle, "Unable to marshal the out/ref parameter", sel, method, p, (int) i, false);
						goto exception_handling;
					}
					LOGZ (" writing back managed array %p to argument at index %i (%p)\n", value, i + 1, arg);
				} else {
					exception_gchandle = xamarin_get_exception_for_parameter (8030, 0, "Unable to marshal the out/ref parameter", sel, method, p, (int) i, false);
					goto exception_handling;
				}
				*(NSObject **) arg = obj;
			} else {
				exception_gchandle = xamarin_get_exception_for_parameter (8030, 0, "Unable to marshal the out/ref parameter", sel, method, p, (int) i, false);
				goto exception_handling;
			}
		}
		iterator (IteratorEnd, context, NULL, 0, NULL, &exception_gchandle);
		if (exception_gchandle != INVALID_GCHANDLE)
			goto exception_handling;
	}

	ret_type = [sig methodReturnType];
	ret_type = xamarin_skip_encoding_flags (ret_type);

	sig_ret_type = mono_signature_get_return_type (msig);
	if (is_ctor) {
		marshal_return_value (context, "|", __SIZEOF_POINTER__, self, sig_ret_type, (desc->semantic & ArgumentSemanticRetainReturnValue) != 0, method, desc, &exception_gchandle);
	} else if (*ret_type != 'v') {
		marshal_return_value (context, ret_type, [sig methodReturnLength], retval, sig_ret_type, (desc->semantic & ArgumentSemanticRetainReturnValue) != 0, method, desc, &exception_gchandle);
	}
	xamarin_mono_object_release (&sig_ret_type);

exception_handling:
	;

	if (dispose_list) {
		SList *list = dispose_list;
		while (list) {
			GCHandle dispose_exception_gchandle = INVALID_GCHANDLE;
			xamarin_dispose ((MonoObject *) list->data, &dispose_exception_gchandle);
			if (dispose_exception_gchandle != INVALID_GCHANDLE) {
				if (exception_gchandle == INVALID_GCHANDLE) {
					// If we get an exception while disposing, and we don't already have an exception, then we need to throw the dispose exception (later, when done disposing)
					exception_gchandle = dispose_exception_gchandle;
				} else {
					// If we already have an exception, don't overwrite it with an exception from disposing something.
					// However we don't want to silently ignore it, so print it.
					NSLog (@PRODUCT ": An exception occurred while disposing the object %p:", list->data);
					NSLog (@"%@", xamarin_print_all_exceptions (dispose_exception_gchandle));
				}
			}
			list = list->next;
		}
		s_list_free (dispose_list);
	}

	if (desc != NULL) {
		xamarin_gchandle_free (desc->method_handle);
		for (int i = 0; i < desc->bindas_count; i++)
			xamarin_gchandle_free (desc->bindas [i].original_type_handle);
	}

	if (free_list) {
		SList *list = free_list;
		while (list) {
			xamarin_free (list->data);
			list = list->next;
		}
		s_list_free (free_list);
	}

#if defined (CORECLR_RUNTIME)
	if (release_list) {
		SList *list = release_list;
		while (list) {
			xamarin_mono_object_release ((MonoObject **) &list->data);
			list = list->next;
		}
		s_list_free (release_list);
	}
#endif

	if (arg_copy != NULL) {
		free (arg_copy);
		arg_copy = NULL;
	}

	if (writeback) {
		free (writeback);
		writeback = NULL;
	}

	xamarin_bridge_free_mono_signature (&msig);

	MONO_THREAD_DETACH; // COOP: This will switch to GC_SAFE

	if (exception_gchandle != INVALID_GCHANDLE) {
		xamarin_process_managed_exception_gchandle (exception_gchandle);
	} else {
		xamarin_process_managed_exception (exception);
	}
}