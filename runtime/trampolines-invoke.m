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

//#define TRACE
#ifdef TRACE
#define LOGZ(...) fprintf (stderr, __VA_ARGS__);
#else
#define LOGZ(...)
#endif

void
xamarin_invoke_trampoline (enum TrampolineType type, id self, SEL sel, iterator_func iterator, marshal_return_value_func marshal_return_value, void *context)
{
	bool is_static = (type & Tramp_Static) == Tramp_Static;
	bool is_ctor = type == Tramp_Ctor;

	if (is_ctor) {
		void *obj = xamarin_try_get_nsobject (self);
		if (obj != NULL) {
			self = xamarin_invoke_objc_method_implementation (self, sel, (IMP) xamarin_ctor_trampoline);
			marshal_return_value (context, "|", sizeof (id), self, NULL, false, NULL);
			return;
		}
	}

	// pre-prolog
	SList *dispose_list = NULL;
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
	MethodDescription desc;

	if (xamarin_use_old_dynamic_registrar || is_ctor || is_static) {
		desc = xamarin_get_method_for_selector ([self class], sel);
	} else {
		desc = xamarin_get_method_and_object_for_selector ([self class], sel, self, &mthis);
	}

	MonoMethod *method = xamarin_get_reflection_method_method (desc.method);
	MonoMethodSignature *msig = mono_method_signature (method);
	int semantic = desc.semantic & ArgumentSemanticMask;
	bool isCategoryInstance = (desc.semantic & ArgumentSemanticCategoryInstance) == ArgumentSemanticCategoryInstance;

	if (!is_static && !is_ctor && xamarin_use_old_dynamic_registrar) {
		mthis = xamarin_get_managed_object_for_ptr (self);
		xamarin_check_for_gced_object (mthis, sel, self, method);
	}

	// setup callstack
	int frame_length = [sig frameLength] - (sizeof (void *) * (isCategoryInstance ? 1 : 2));
	void *arg_frame [frame_length/sizeof (void *)];
	void *arg_ptrs [num_arg + (isCategoryInstance ? 1 : 0)];
	void *iter = NULL;
	gboolean needs_writeback = FALSE;
	MonoType *p;
	int ofs;
	int i;
	int mofs = 0;

#ifdef TRACE
	memset (arg_ptrs, 0xDEADF00D, num_arg * sizeof (void*));
#endif

	if (isCategoryInstance) {
		// we know this must be an id
		p = mono_signature_get_params (msig, &iter);
		arg_ptrs [0] = xamarin_get_nsobject_with_type_for_ptr (self, false, p);
		mofs = 1;
	}

	iterator (IteratorStart, context, NULL, 0, NULL); // start

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

				iterator (IteratorIterate, context, type, size, &arg_frame [ofs++]);
				arg_frame [ofs++] = 0;
				arg_ptrs [i + mofs] = &arg_frame [frameofs];
				continue;
			}
		}
#endif
		
		if (size > sizeof (void *)) {
			iterator (IteratorIterate, context, type, size, &arg_frame [ofs]);
			ofs += size / sizeof (void *);
			arg_ptrs [i + mofs] = &arg_frame [frameofs];
		} else {
			void *arg;
			iterator (IteratorIterate, context, type, size, &arg);
			switch (type [0]) {
				case _C_PTR: {
					switch (type [1]) {
						case _C_ID: {
							MonoClass *p_klass = mono_class_from_mono_type (p);
							if (!mono_type_is_byref (p))
								mono_raise_exception ((MonoException *) mono_get_exception_execution_engine ("Invalid type encoding for parameter"));
							if (xamarin_is_parameter_out (mono_method_get_object (mono_domain_get (), method, NULL), i)) {
								arg_frame [ofs] = NULL;
								arg_ptrs [i + mofs] = &arg_frame [frameofs];
								needs_writeback = TRUE;
								LOGZ (" argument %i is an out parameter. Passing in a pointer to a NULL value.\n", i + 1);
								break;
							} else {
								if (xamarin_is_class_nsobject (p_klass)) {
									MonoObject *obj;
									NSObject *targ = *(NSObject **) arg;

									obj = xamarin_get_nsobject_with_type_for_ptr (targ, false, p);
#if DEBUG
									xamarin_verify_parameter (obj, sel, self, targ, i, p_klass, method);
#endif
									arg_frame [ofs] = obj;
									arg_ptrs [i + mofs] = &arg_frame [frameofs];
									LOGZ (" argument %i is a ref NSObject parameter: %p = %p\n", i + 1, arg, obj);
									needs_writeback = TRUE;
								} else {
									mono_raise_exception ((MonoException *) mono_get_exception_execution_engine ("Unable to marshal byref parameter type"));
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
								arg_ptrs [i + mofs] = xamarin_get_delegate_for_block_parameter (method, i, arg);
							} else if (xamarin_is_class_inativeobject (p_klass)) {
								id id_arg = (id) arg;
								if (semantic == ArgumentSemanticCopy) {
									id_arg = [id_arg copy];
									[id_arg autorelease];
								}
								MonoObject *obj;
								obj = xamarin_get_inative_object_dynamic (id_arg, false, mono_type_get_object (mono_domain_get (), p));
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
						arg_ptrs [i + mofs] = (void *) xamarin_get_class ((Class) arg);
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
						arg_ptrs [i + mofs] = (void *) xamarin_get_selector ((SEL) arg);
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
					if (!id_arg) {
						arg_ptrs [i + mofs] = NULL;
						break;
					} else {
						MonoClass *p_klass = mono_class_from_mono_type (p);
						if (p_klass == mono_get_intptr_class ()) {
							arg_frame [ofs] = id_arg;
							arg_ptrs [i + mofs] = &arg_frame [frameofs];
							LOGZ (" argument %i is IntPtr: %p\n", i + 1, id_arg);
						} else if (p_klass == mono_get_string_class ()) {
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
									obj = xamarin_get_nsobject_with_type_for_ptr (targ, false, e);
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
							obj = xamarin_get_nsobject_with_type_for_ptr_created (id_arg, false, p, &created);

							if (created && obj) {
								bool is_transient = xamarin_is_parameter_transient (mono_method_get_object (mono_domain_get (), method, NULL), i);
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
							obj = xamarin_get_inative_object_dynamic (id_arg, false, mono_type_get_object (mono_domain_get (), p));
							LOGZ (" argument %i is NSObject/INativeObject %p: %p\n", i + 1, id_arg, obj);
							arg_ptrs [i + mofs] = obj;
						} else if (mono_class_is_delegate (p_klass)) {
							arg_ptrs [i + mofs] = xamarin_get_delegate_for_block_parameter (method, i, id_arg);
						} else {
							if (semantic == ArgumentSemanticCopy) {
								id_arg = [id_arg copy];
								[id_arg autorelease];
							}
							MonoObject *obj;
							obj = xamarin_get_nsobject_with_type_for_ptr (id_arg, false, p);
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
	
	iterator (IteratorEnd, context, NULL, 0, NULL);
	
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
		mono_runtime_invoke (method, retval, (void **) arg_ptrs, NULL);
		xamarin_create_managed_ref (self, retval, true);

		xamarin_register_nsobject (retval, self);
	} else {
		
#ifdef TRACE
		fprintf (stderr, " calling managed method with %i arguments: ", num_arg);
		for (int i = 0; i < num_arg; i++)
			fprintf (stderr, "%p ", arg_ptrs [i]);
		fprintf (stderr, "\n");
#endif

		retval = mono_runtime_invoke (method, mthis, (void **) arg_ptrs, NULL);

#ifdef TRACE
		fprintf (stderr, " called managed method with %i arguments: ", num_arg);
		for (int i = 0; i < num_arg; i++)
			fprintf (stderr, "%p ", arg_ptrs [i]);
		fprintf (stderr, "\n");
#endif

	}

	// writeback
	if (needs_writeback) {
		iter = NULL;
		iterator (IteratorStart, context, NULL, 0, NULL); // start
		for (i = 0, ofs = 0; i < num_arg; i++) {
			const char *type = [sig getArgumentTypeAtIndex: (i+2)];
			int size = xamarin_objc_type_size (type);

			p = mono_signature_get_params (msig, &iter);

			if (size > sizeof (void *)) {
				// Skip over any structs. In any case they can't be write-back parameters.
				iterator (IteratorIterate, context, type, size, NULL);
				ofs += size / sizeof (void *);
			} else {
				void *arg;
				iterator (IteratorIterate, context, type, size, &arg);
				if (type [0] == _C_PTR && type [1] == _C_ID) {
					MonoClass *p_klass = mono_class_from_mono_type (p);

					if (p_klass == mono_get_string_class ()) {
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
						*(NSObject **) arg = xamarin_get_handle ((MonoObject *) arg_frame [ofs]);
						LOGZ (" writing back managed NSObject %p to argument at %p\n", *(NSObject **) arg, arg);
					} else if (xamarin_is_class_inativeobject (p_klass)) {
						*(NSObject **) arg = xamarin_get_handle ((MonoObject *) arg_frame [ofs]);
						LOGZ (" writing back managed INativeObject %p to argument at %p\n", *(NSObject **) arg, arg);
					} else {
						char *method_full_name = mono_method_full_name (method, TRUE);
						char *to_name = xamarin_type_get_full_name (p);
						char *msg = xamarin_strdup_printf ("Unable to marshal the out/ref parameter #%i whose managed type is '%s' to Objective-C.\n"
							"Additional information:\n"	
							"\tSelector: %s\n"
							"\tMethod: %s\n", i + 1, to_name, sel_getName (sel), method_full_name);
						MonoException *exc = xamarin_create_exception (msg);
						xamarin_free (msg);
						xamarin_free (to_name);
						xamarin_free (method_full_name);
						mono_raise_exception (exc);
					}
					break;
				}
				ofs++;
			}
		}
		iterator (IteratorEnd, context, NULL, 0, NULL);
	}

	if (dispose_list) {
		SList *list = dispose_list;
		while (list) {
			xamarin_dispose ((MonoObject *) list->data);
			list = list->next;
		}
		s_list_free (dispose_list);
	}

	const char *ret_type = [sig methodReturnType];
	ret_type = xamarin_skip_encoding_flags (ret_type);
	if (is_ctor) {
		marshal_return_value (context, "|", sizeof (id), self, mono_signature_get_return_type (msig), (desc.semantic & ArgumentSemanticRetainReturnValue) != 0, method);
	} else if (*ret_type != 'v') {
		marshal_return_value (context, ret_type, [sig methodReturnLength], retval, mono_signature_get_return_type (msig), (desc.semantic & ArgumentSemanticRetainReturnValue) != 0, method);
	}
}