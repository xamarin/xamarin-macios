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

//#define TRACE
#ifdef TRACE
#define LOGZ(...) fprintf (stderr, __VA_ARGS__);
#else
#define LOGZ(...)
#endif

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
	guint32 exception_gchandle = 0;
	bool is_static = (type & Tramp_Static) == Tramp_Static;
	bool is_ctor = type == Tramp_Ctor;
	const char *ret_type = NULL;

	if (is_ctor) {
		bool has_nsobject = xamarin_has_nsobject (self, &exception_gchandle);
		if (exception_gchandle != 0) {
			xamarin_process_managed_exception_gchandle (exception_gchandle);
			return; // we shouldn't get here.
		}

		if (has_nsobject) {
			self = xamarin_invoke_objc_method_implementation (self, sel, (IMP) xamarin_ctor_trampoline);
			marshal_return_value (context, "|", sizeof (id), self, NULL, false, NULL, NULL, &exception_gchandle);
			xamarin_process_managed_exception_gchandle (exception_gchandle);
			return;
		}
	}

	MONO_THREAD_ATTACH; // COOP: This will swith to GC_UNSAFE

	// pre-prolog
	SList *dispose_list = NULL;
	SList *free_list = NULL;
	int num_arg;
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
	MonoMethodSignature *msig;
	int semantic;
	bool isCategoryInstance;

	// setup callstack
	int frame_length;
	void **arg_frame;
	void **arg_ptrs;
	void *iter = NULL;
	gboolean needs_writeback = FALSE;
	MonoType *p;
	int ofs;
	int i;
	int mofs = 0;

	int desc_arg_count = num_arg + 2; /* 1 for the return value + 1 if this is a category instance method */
	size_t desc_size = desc_arg_count * sizeof (BindAsData) + sizeof (MethodDescription);
	desc = (MethodDescription *) xamarin_calloc (desc_size);
	desc->bindas_count = desc_arg_count;
	free_list = s_list_prepend (free_list, desc);

	if (is_ctor || is_static) {
		xamarin_get_method_for_selector ([self class], sel, is_static, desc, &exception_gchandle);
	} else {
		xamarin_get_method_and_object_for_selector ([self class], sel, is_static, self, &mthis, desc, &exception_gchandle);
	}
	if (exception_gchandle != 0)
		goto exception_handling;

	method = xamarin_get_reflection_method_method (desc->method);
	msig = mono_method_signature (method);
	semantic = desc->semantic & ArgumentSemanticMask;
	isCategoryInstance = (desc->semantic & ArgumentSemanticCategoryInstance) == ArgumentSemanticCategoryInstance;

	frame_length = [sig frameLength] - (sizeof (void *) * (isCategoryInstance ? 1 : 2));
	arg_frame = (void **) alloca (frame_length);
	arg_ptrs = (void **) alloca (sizeof (void *) * (num_arg + (isCategoryInstance ? 1 : 0)));
	
#ifdef TRACE
	memset (arg_ptrs, 0xDEADF00D, num_arg * sizeof (void*));
#endif

	if (isCategoryInstance) {
		// we know this must be an id
		p = mono_signature_get_params (msig, &iter);
		arg_ptrs [0] = xamarin_get_nsobject_with_type_for_ptr (self, false, p, sel, method, &exception_gchandle);
		if (exception_gchandle != 0)
			goto exception_handling;
		mofs = 1;
	}

	iterator (IteratorStart, context, NULL, 0, NULL, &exception_gchandle); // start
	if (exception_gchandle != 0)
		goto exception_handling;

	for (i = 0, ofs = 0; i < num_arg; i++) {
		const char *type = xamarin_skip_encoding_flags ([sig getArgumentTypeAtIndex: (i+2)]);
		int size = xamarin_objc_type_size (type);
		int frameofs = ofs;
		p = mono_signature_get_params (msig, &iter);
		
#if __SIZEOF_POINTER__ == 4
		if (*type == 'i' || *type == 'I') {
			// this might be a [Native] enum, in which case managed code expects a 64-bit value.

			MonoClass *p_klass = mono_class_from_mono_type (p);
			if (mono_class_is_enum (p_klass) && mono_class_value_size (p_klass, NULL) == 8) {
				// Don't bother checking for the attribute (it's quite expensive),
				// just check whether managed code expects a 64-bit value and assume
				// we end up in this condition because it's a [Native] enum.

				iterator (IteratorIterate, context, type, size, &arg_frame [ofs++], &exception_gchandle);
				if (exception_gchandle != 0)
					goto exception_handling;
				arg_frame [ofs++] = 0;
				arg_ptrs [i + mofs] = &arg_frame [frameofs];
				continue;
			}
		}
#endif
		
		if (size > sizeof (void *)) {
			iterator (IteratorIterate, context, type, size, &arg_frame [ofs], &exception_gchandle);
			if (exception_gchandle != 0)
				goto exception_handling;
			ofs += size / sizeof (void *);
			arg_ptrs [i + mofs] = &arg_frame [frameofs];
		} else {
			void *arg;
			iterator (IteratorIterate, context, type, size, &arg, &exception_gchandle);
			if (exception_gchandle != 0)
				goto exception_handling;
			if (desc->bindas [i + 1].original_type != NULL) {
				arg_ptrs [i + mofs] = xamarin_generate_conversion_to_managed ((id) arg, mono_reflection_type_get_type (desc->bindas [i + 1].original_type), p, method, &exception_gchandle, INVALID_TOKEN_REF, (void **) &free_list);
				if (exception_gchandle != 0)
					goto exception_handling;
				ofs++;
				continue;
			}
			switch (type [0]) {
				case _C_PTR: {
					switch (type [1]) {
						case _C_ID: {
							MonoClass *p_klass = mono_class_from_mono_type (p);
							if (!mono_type_is_byref (p)) {
								exception = (MonoObject *) mono_get_exception_execution_engine ("Invalid type encoding for parameter");
								goto exception_handling;
							}
							bool is_parameter_out = xamarin_is_parameter_out (mono_method_get_object (mono_domain_get (), method, NULL), i, &exception_gchandle);
							if (exception_gchandle != 0)
								goto exception_handling;
							if (is_parameter_out) {
								arg_frame [ofs] = NULL;
								arg_ptrs [i + mofs] = &arg_frame [frameofs];
								needs_writeback = TRUE;
								LOGZ (" argument %i is an out parameter. Passing in a pointer to a NULL value.\n", i + 1);
								break;
							} else {
								if (xamarin_is_class_nsobject (p_klass)) {
									MonoObject *obj;
									NSObject *targ = *(NSObject **) arg;

									obj = xamarin_get_nsobject_with_type_for_ptr (targ, false, p, sel, method, &exception_gchandle);
									if (exception_gchandle != 0)
										goto exception_handling;
#if DEBUG
									xamarin_verify_parameter (obj, sel, self, targ, i, p_klass, method);
#endif
									arg_frame [ofs] = obj;
									arg_ptrs [i + mofs] = &arg_frame [frameofs];
									LOGZ (" argument %i is a ref NSObject parameter: %p = %p\n", i + 1, arg, obj);
									needs_writeback = TRUE;
								} else {
									exception = (MonoObject *) mono_get_exception_execution_engine ("Unable to marshal byref parameter type");
									goto exception_handling;
								}
								break;
							}
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
							if (mono_class_is_delegate (p_klass)) {
								arg_ptrs [i + mofs] = xamarin_get_delegate_for_block_parameter (method, INVALID_TOKEN_REF, i, arg, &exception_gchandle);
								if (exception_gchandle != 0)
									goto exception_handling;
							} else if (xamarin_is_class_inativeobject (p_klass)) {
								id id_arg = (id) arg;
								if (semantic == ArgumentSemanticCopy) {
									id_arg = [id_arg copy];
									[id_arg autorelease];
								}
								MonoObject *obj;
								obj = xamarin_get_inative_object_dynamic (id_arg, false, mono_type_get_object (mono_domain_get (), p), &exception_gchandle);
								if (exception_gchandle != 0)
									goto exception_handling;
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
						arg_ptrs [i + mofs] = (void *) xamarin_get_class ((Class) arg, &exception_gchandle);
						if (exception_gchandle != 0)
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
						arg_ptrs [i + mofs] = (void *) xamarin_get_selector ((SEL) arg, &exception_gchandle);
						if (exception_gchandle != 0)
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
						arg_ptrs [i + mofs] = (void *) mono_string_new (mono_domain_get (), (const char *) arg);
						LOGZ (" argument %i is char*: %p = %s\n", i + 1, arg, arg);
						break;
					}
				}
				case _C_ID: {
					id id_arg = (id) arg;
					MonoClass *p_klass = mono_class_from_mono_type (p);
					if (p_klass == mono_get_intptr_class ()) {
						arg_frame [ofs] = id_arg;
						arg_ptrs [i + mofs] = &arg_frame [frameofs];
						LOGZ (" argument %i is IntPtr: %p\n", i + 1, id_arg);
						break;
					} else if (!id_arg) {
						arg_ptrs [i + mofs] = NULL;
						break;
					} else {
						if (p_klass == mono_get_string_class ()) {
							NSString *str = (NSString *) id_arg;
							arg_ptrs [i + mofs] = mono_string_new (mono_domain_get (), [str UTF8String]);
							LOGZ (" argument %i is NSString: %p = %s\n", i + 1, id_arg, [str UTF8String]);
						} else if (xamarin_is_class_array (p_klass)) {
#if DEBUG
							xamarin_check_objc_type (id_arg, [NSArray class], sel, self, i, method);
#endif
							NSArray *arr = (NSArray *) id_arg;
							MonoClass *e_klass = mono_class_get_element_class (p_klass);
							MonoType *e = mono_class_get_type (e_klass);
							MonoArray *m_arr = mono_array_new (mono_domain_get (), e_klass, [arr count]);
							int j;

							for (j = 0; j < [arr count]; j++) {
								if (e_klass == mono_get_string_class ()) {
									NSString *sv = (NSString *) [arr objectAtIndex: j];
									mono_array_set (m_arr, MonoString *, j, mono_string_new (mono_domain_get (), [sv UTF8String]));
								} else {
									MonoObject *obj;
									id targ = [arr objectAtIndex: j];
									obj = xamarin_get_nsobject_with_type_for_ptr (targ, false, e, sel, method, &exception_gchandle);
									if (exception_gchandle != 0)
										goto exception_handling;
#if DEBUG
									xamarin_verify_parameter (obj, sel, self, targ, i, e_klass, method);
#endif
									mono_array_set (m_arr, MonoObject *, j, obj);
								}
							}

							LOGZ (" argument %i is NSArray\n", i + 1);
							arg_ptrs [i + mofs] = m_arr;
						} else if (xamarin_is_class_nsobject (p_klass)) {
							if (semantic == ArgumentSemanticCopy) {
								id_arg = [id_arg copy];
								[id_arg autorelease];
							}
							MonoObject *obj;
							int32_t created = false;
							obj = xamarin_get_nsobject_with_type_for_ptr_created (id_arg, false, p, &created, sel, method, &exception_gchandle);
							if (exception_gchandle != 0)
								goto exception_handling;

							if (created && obj) {
								bool is_transient = xamarin_is_parameter_transient (mono_method_get_object (mono_domain_get (), method, NULL), i, &exception_gchandle);
								if (exception_gchandle != 0)
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
							obj = xamarin_get_inative_object_dynamic (id_arg, false, mono_type_get_object (mono_domain_get (), p), &exception_gchandle);
							if (exception_gchandle != 0)
								goto exception_handling;
							LOGZ (" argument %i is NSObject/INativeObject %p: %p\n", i + 1, id_arg, obj);
							arg_ptrs [i + mofs] = obj;
						} else if (mono_class_is_delegate (p_klass)) {
							arg_ptrs [i + mofs] = xamarin_get_delegate_for_block_parameter (method, INVALID_TOKEN_REF, i, id_arg, &exception_gchandle);
							if (exception_gchandle != 0)
								goto exception_handling;
						} else {
							if (semantic == ArgumentSemanticCopy) {
								id_arg = [id_arg copy];
								[id_arg autorelease];
							}
							MonoObject *obj;
							obj = xamarin_get_nsobject_with_type_for_ptr (id_arg, false, p, sel, method, &exception_gchandle);
							if (exception_gchandle != 0)
								goto exception_handling;
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
	if (exception_gchandle != 0)
		goto exception_handling;
	
	// invoke
	MonoObject *retval;
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
		retval = mono_object_new (mono_domain_get (), mono_method_get_class (method));

		xamarin_set_nsobject_handle (retval, self);
		xamarin_set_nsobject_flags (retval, NSObjectFlagsNativeRef);
		mono_runtime_invoke (method, retval, (void **) arg_ptrs, exception_ptr);
		if (exception != NULL)
			goto exception_handling;
		xamarin_create_managed_ref (self, retval, true);

		xamarin_register_nsobject (retval, self, &exception_gchandle);
		if (exception_gchandle != 0)
			goto exception_handling;
	} else {
		
#ifdef TRACE
		fprintf (stderr, " calling managed method with %i arguments: ", num_arg);
		for (int i = 0; i < num_arg; i++)
			fprintf (stderr, "%p ", arg_ptrs [i]);
		fprintf (stderr, "\n");
#endif

		retval = mono_runtime_invoke (method, mthis, (void **) arg_ptrs, exception_ptr);

#ifdef TRACE
		fprintf (stderr, " called managed method with %i arguments: ", num_arg);
		for (int i = 0; i < num_arg; i++)
			fprintf (stderr, "%p ", arg_ptrs [i]);
		fprintf (stderr, "\n");
#endif

		if (exception != NULL)
			goto exception_handling;
	}

	// writeback
	if (needs_writeback) {
		iter = NULL;
		iterator (IteratorStart, context, NULL, 0, NULL, &exception_gchandle); // start
		if (exception_gchandle != 0)
			goto exception_handling;
		for (i = 0, ofs = 0; i < num_arg; i++) {
			const char *type = [sig getArgumentTypeAtIndex: (i+2)];
			int size = xamarin_objc_type_size (type);

			p = mono_signature_get_params (msig, &iter);

			if (size > sizeof (void *)) {
				// Skip over any structs. In any case they can't be write-back parameters.
				iterator (IteratorIterate, context, type, size, NULL, &exception_gchandle);
				if (exception_gchandle != 0)
					goto exception_handling;
				ofs += size / sizeof (void *);
			} else {
				void *arg;
				iterator (IteratorIterate, context, type, size, &arg, &exception_gchandle);
				if (exception_gchandle != 0)
					goto exception_handling;
				if (type [0] == _C_PTR && type [1] == _C_ID) {
					MonoClass *p_klass = mono_class_from_mono_type (p);

					if (arg == NULL) {
						// Can't write back to a NULL pointer, so ignore this.
						LOGZ (" not writing back to a null pointer\n");
					} else if (p_klass == mono_get_string_class ()) {
						MonoString *value = (MonoString *) arg_frame [ofs];
						if (value == NULL) {
							*(NSObject **) arg = NULL;
							LOGZ (" writing back managed null string to argument at %p\n", arg);
						} else {
							char *str = mono_string_to_utf8 (value);
							*(NSObject **) arg = [[[NSString alloc] initWithUTF8String:str] autorelease];
							LOGZ (" writing back managed string %p = %s to argument at %p\n", *(NSObject **) arg, str, arg);
							mono_free (str);
						}
					} else if (xamarin_is_class_nsobject (p_klass)) {
						*(NSObject **) arg = xamarin_get_handle ((MonoObject *) arg_frame [ofs], &exception_gchandle);
						if (exception_gchandle != 0)
							goto exception_handling;
						LOGZ (" writing back managed NSObject %p to argument at %p\n", *(NSObject **) arg, arg);
					} else if (xamarin_is_class_inativeobject (p_klass)) {
						*(NSObject **) arg = xamarin_get_handle ((MonoObject *) arg_frame [ofs], &exception_gchandle);
						if (exception_gchandle != 0)
							goto exception_handling;
						LOGZ (" writing back managed INativeObject %p to argument at %p\n", *(NSObject **) arg, arg);
					} else {
						char *to_name = xamarin_type_get_full_name (p, &exception_gchandle);
						if (exception_gchandle != 0)
							goto exception_handling;
						char *method_full_name = mono_method_full_name (method, TRUE);
						char *msg = xamarin_strdup_printf ("Unable to marshal the out/ref parameter #%i whose managed type is '%s' to Objective-C.\n"
							"Additional information:\n"	
							"\tSelector: %s\n"
							"\tMethod: %s\n", i + 1, to_name, sel_getName (sel), method_full_name);
						MonoException *exc = xamarin_create_exception (msg);
						xamarin_free (msg);
						xamarin_free (to_name);
						xamarin_free (method_full_name);
						exception = (MonoObject *) exc;
						goto exception_handling;
					}
					break;
				}
				ofs++;
			}
		}
		iterator (IteratorEnd, context, NULL, 0, NULL, &exception_gchandle);
		if (exception_gchandle != 0)
			goto exception_handling;
	}

	ret_type = [sig methodReturnType];
	ret_type = xamarin_skip_encoding_flags (ret_type);
	if (is_ctor) {
		marshal_return_value (context, "|", sizeof (id), self, mono_signature_get_return_type (msig), (desc->semantic & ArgumentSemanticRetainReturnValue) != 0, method, desc, &exception_gchandle);
	} else if (*ret_type != 'v') {
		marshal_return_value (context, ret_type, [sig methodReturnLength], retval, mono_signature_get_return_type (msig), (desc->semantic & ArgumentSemanticRetainReturnValue) != 0, method, desc, &exception_gchandle);
	}

exception_handling:
	;

	if (dispose_list) {
		SList *list = dispose_list;
		while (list) {
			guint32 dispose_exception_gchandle = 0;
			xamarin_dispose ((MonoObject *) list->data, &dispose_exception_gchandle);
			if (dispose_exception_gchandle != 0) {
				if (exception_gchandle == 0) {
					// If we get an exception while disposing, and we don't already have an exception, then we need to throw the dispose exception (later, when done disposing)
					exception_gchandle = dispose_exception_gchandle;
				} else {
					// If we already have an exception, don't overwrite it with an exception from disposing something.
					// However we don't want to silently ignore it, so print it.
					NSLog (@PRODUCT ": An exception occurred while disposing the object %p:", list->data);
					NSLog (@"%@", xamarin_print_all_exceptions (mono_gchandle_get_target (dispose_exception_gchandle)));
				}
			}
			list = list->next;
		}
		s_list_free (dispose_list);
	}
	if (free_list) {
		SList *list = free_list;
		while (list) {
			xamarin_free (list->data);
			list = list->next;
		}
		s_list_free (free_list);
	}

	MONO_THREAD_DETACH; // COOP: This will switch to GC_SAFE

	if (exception_gchandle != 0) {
		xamarin_process_managed_exception_gchandle (exception_gchandle);
	} else {
		xamarin_process_managed_exception (exception);
	}
}
