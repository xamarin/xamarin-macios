//
// UIAppearance.cs: Code to return the class_ptr that we access elsewhere
//
// Author:
//   Miguel de Icaza
//
// Copyright 2011, 2015 Xamarin Inc
//

#if !WATCH

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace UIKit {
	public partial class UIAppearance {
		public override bool Equals (object other)
		{
			UIAppearance ao = other as UIAppearance;
			if (ao is null)
				return false;
			return ao.Handle == Handle;
		}

		public override int GetHashCode ()
		{
			return Handle.GetHashCode ();
		}

		public static bool operator == (UIAppearance a, UIAppearance b)
		{
			if (ReferenceEquals (a, null))
				return ReferenceEquals (b, null);
			else if (ReferenceEquals (b, null))
				return false;

			return a.Handle == b.Handle;
		}

		public static bool operator != (UIAppearance a, UIAppearance b)
		{
			return !(a == b);
		}

		static NativeHandle [] TypesToPointers (Type [] whenFoundIn)
		{
#if TVOS
			var ptrs = new NativeHandle [whenFoundIn.Length];
#else
			if (whenFoundIn.Length > 4)
				throw new ArgumentException ("Only 4 parameters supported currently");

			var ptrs = new NativeHandle [5]; // creating an array of 5 when we support only 4 ensures that the last one is IntPtr.Zero.
#endif
			for (int i = 0; i < whenFoundIn.Length; i++) {
				if (whenFoundIn [i] is null)
					throw new ArgumentException (String.Format ("Parameter {0} was null, must specify a valid type", i));
				if (!typeof (NSObject).IsAssignableFrom (whenFoundIn [i]))
					throw new ArgumentException (String.Format ("Type {0} does not derive from NSObject", whenFoundIn [i]));

				var classHandle = Class.GetHandle (whenFoundIn [i]);
				if (classHandle == NativeHandle.Zero)
					throw new ArgumentException (string.Format ("Could not find the Objective-C class for {0}", whenFoundIn [i].FullName));

				ptrs [i] = classHandle;
			}
			return ptrs;
		}

#if TVOS
		// new in iOS9 but the only option for tvOS
		const string selAppearanceWhenContainedInInstancesOfClasses = "appearanceWhenContainedInInstancesOfClasses:";

		// +(instancetype _Nonnull)appearanceWhenContainedInInstancesOfClasses:(NSArray<Class<UIAppearanceContainer>> * _Nonnull)containerTypes
		public static IntPtr GetAppearance (IntPtr class_ptr, params Type [] whenFoundIn)
		{
			using (var array = NSArray.FromIntPtrs (TypesToPointers (whenFoundIn))) {
				return Messaging.IntPtr_objc_msgSend_IntPtr (class_ptr, 
					Selector.GetHandle (UIAppearance.selAppearanceWhenContainedInInstancesOfClasses), array.Handle);
			}
		}

		const string selAppearanceForTraitCollectionWhenContainedInInstancesOfClasses = "appearanceForTraitCollection:whenContainedInInstancesOfClasses:";

		// new in iOS9 but the only option for tvOS
		public static IntPtr GetAppearance (IntPtr class_ptr, UITraitCollection traits, params Type [] whenFoundIn)
		{
			if (traits is null)
				throw new ArgumentNullException ("traits");

			using (var array = NSArray.FromIntPtrs (TypesToPointers (whenFoundIn))) {
				return Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr (class_ptr, 
					Selector.GetHandle (UIAppearance.selAppearanceForTraitCollectionWhenContainedInInstancesOfClasses),
					traits.Handle, array.Handle);
			}
		}
#else
		const string selAppearanceWhenContainedIn = "appearanceWhenContainedIn:";
		const string selAppearanceForTraitCollectionWhenContainedIn = "appearanceForTraitCollection:whenContainedIn:";

		[BindingImpl (BindingImplOptions.Optimizable)]
		public static IntPtr GetAppearance (IntPtr class_ptr, params Type [] whenFoundIn)
		{
			var ptrs = TypesToPointers (whenFoundIn);

			if (Runtime.IsARM64CallingConvention) {
				// The native function takes a variable number of arguments ('appearanceWhenContainedIn:'), terminated with a nil value.
				// Unfortunately iOS/ARM64 (not the general ARM64 ABI as published by ARM) has a different calling convention for varargs methods
				// than regular methods: all variable arguments are passed on the stack, no matter how many normal arguments there are.
				// Normally 8 arguments are passed in registers, then the subsequent ones are passed on the stack, so what we do is to provide
				// 6 dummy arguments (remember that Objective-C always has two hidden arguments (id, SEL), which are the two first arguments
				// here), in order to push the arguments we care about to the stack.
				return IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (
					class_ptr, // x0
					Selector.GetHandle (UIAppearance.selAppearanceWhenContainedIn), // x1
					ptrs [0], // x2
					IntPtr.Zero, // x3
					IntPtr.Zero, // x4
					IntPtr.Zero, // x5
					IntPtr.Zero, // x6
					IntPtr.Zero, // x7
					ptrs [1], // the rest is on the stack. This is where iOS/ARM64 expects the first varargs arguments.
					ptrs [2], ptrs [3], ptrs [4], IntPtr.Zero);
			} else {
#if NET
				return Messaging.NativeHandle_objc_msgSend_NativeHandle_NativeHandle_NativeHandle_NativeHandle_NativeHandle (class_ptr, Selector.GetHandle (UIAppearance.selAppearanceWhenContainedIn),
					ptrs [0], ptrs [1], ptrs [2], ptrs [3], ptrs [4]);
#else
				return Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (class_ptr, Selector.GetHandle (UIAppearance.selAppearanceWhenContainedIn),
					ptrs [0], ptrs [1], ptrs [2], ptrs [3], ptrs [4]);
#endif
			}
		}

		[BindingImpl (BindingImplOptions.Optimizable)]
		public static IntPtr GetAppearance (IntPtr class_ptr, UITraitCollection traits, params Type [] whenFoundIn)
		{
			if (traits is null)
				throw new ArgumentNullException ("traits");

			var ptrs = TypesToPointers (whenFoundIn);

			if (Runtime.IsARM64CallingConvention) {
				// The native function takes a variable number of arguments ('appearanceWhenContainedIn:'), terminated with a nil value.
				// Unfortunately iOS/ARM64 (not the general ARM64 ABI as published by ARM) has a different calling convention for varargs methods
				// than regular methods: all variable arguments are passed on the stack, no matter how many normal arguments there are.
				// Normally 8 arguments are passed in registers, then the subsequent ones are passed on the stack, so what we do is to provide
				// 6 dummy arguments (remember that Objective-C always has two hidden arguments (id, SEL), which are the two first arguments
				// here), in order to push the arguments we care about to the stack.
				return IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (
					class_ptr, // x0
					Selector.GetHandle (UIAppearance.selAppearanceForTraitCollectionWhenContainedIn), // x1
					traits.Handle, // x2
					ptrs [0], // x3
					IntPtr.Zero, // x4
					IntPtr.Zero, // x5
					IntPtr.Zero, // x6
					IntPtr.Zero, // x7
					ptrs [1], // the rest is on the stack. This is where iOS/ARM64 expects the first varargs arguments.
					ptrs [2], ptrs [3], ptrs [4], IntPtr.Zero);
			} else {
#if NET
				return Messaging.NativeHandle_objc_msgSend_NativeHandle_NativeHandle_NativeHandle_NativeHandle_NativeHandle (class_ptr, Selector.GetHandle (UIAppearance.selAppearanceForTraitCollectionWhenContainedIn),
#else
				return Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (class_ptr, Selector.GetHandle (UIAppearance.selAppearanceForTraitCollectionWhenContainedIn),
#endif
													 traits.Handle, ptrs [0], ptrs [1], ptrs [2], ptrs [3]);
			}
		}

		[DllImport (Messaging.LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
		extern static IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, System.IntPtr arg5, System.IntPtr arg6, System.IntPtr arg7, System.IntPtr arg8, System.IntPtr arg9, System.IntPtr arg10, System.IntPtr arg11);
#endif

		const string selAppearanceForTraitCollection = "appearanceForTraitCollection:";

		public static IntPtr GetAppearance (IntPtr class_ptr, UITraitCollection traits)
		{
			if (traits is null)
				throw new ArgumentNullException ("traits");

			return Messaging.IntPtr_objc_msgSend_IntPtr (class_ptr, Selector.GetHandle (UIAppearance.selAppearanceForTraitCollection), traits.Handle);
		}
	}
}

#endif // !WATCH
