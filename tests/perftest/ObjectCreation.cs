using System;

using Foundation;
using ObjCRuntime;

using BenchmarkDotNet.Attributes;

using Bindings.Test;

namespace PerfTest {
	public class ObjectCreation {

		[Register ("CustomClass")]
		class CustomClass : NSObject { }

		/* new NSObject () */
		NSObject obj;

		[Benchmark]
		public object NSObjectCreation ()
		{
			return obj = new NSObject ();
		}

		[IterationCleanup (Target = nameof (NSObjectCreation))]
		public void NSObjectCreation_Cleanup ()
		{
			obj.Dispose ();
		}

		/* new CustomClass () */
		CustomClass custom_obj;

		[Benchmark]
		public object CustomClassCreation ()
		{
			return custom_obj = new CustomClass ();
		}

		[IterationCleanup (Target = nameof (CustomClassCreation))]
		public void CustomClassCreation_Cleanup ()
		{
			custom_obj.Dispose ();
		}

		/*
		 * Runtime.GetNSObject ([[NSObject alloc] init])
		 */

		IntPtr NSObjectClassHandle = Class.GetHandle (typeof (NSObject));

		NSObject native_nsobject;

		[Benchmark]
		public object NativeNSObjectCreation ()
		{

			var ptr = Messaging.IntPtr_objc_msgSend (Messaging.IntPtr_objc_msgSend (NSObjectClassHandle, Selector.GetHandle ("alloc")), Selector.GetHandle ("init"));
			var obj = Runtime.GetNSObject (ptr);
			Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			return native_nsobject = obj;
		}

		[IterationCleanup (Target = nameof (NativeNSObjectCreation))]
		public void NativeNSObjectCreation_Cleanup ()
		{
			native_nsobject.Dispose ();
		}

		/*
		 *  id obj = [[NSObject alloc] init])
		 *  [obj retain]
		 *  Runtime.GetNSObject (obj)
		 *  [obj release]
		 */

		NSObject native_nsobject_retain_release;

		[Benchmark]
		public object NativeNSObjectRetainReleaseCreation ()
		{

			var ptr = Messaging.IntPtr_objc_msgSend (Messaging.IntPtr_objc_msgSend (NSObjectClassHandle, Selector.GetHandle ("alloc")), Selector.GetHandle ("init"));
			Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("retain"));
			var obj = Runtime.GetNSObject (ptr);
			Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			return native_nsobject_retain_release = obj;
		}

		[IterationCleanup (Target = nameof (NativeNSObjectRetainReleaseCreation))]
		public void NativeNSObjectRetainReleaseCreation_Cleanup ()
		{
			native_nsobject_retain_release.Dispose ();
		}

		/*
		 * [[CustomClass alloc] init]
		 */

		IntPtr CustomClassClassHandle = Class.GetHandle (typeof (CustomClass));

		[Benchmark]
		public void NativeCustomClassCreation ()
		{

			var ptr = Messaging.IntPtr_objc_msgSend (Messaging.IntPtr_objc_msgSend (CustomClassClassHandle, Selector.GetHandle ("alloc")), Selector.GetHandle ("init"));
			Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
		}

		/*
		 * Runtime.GetNSObject ([[CustomClass alloc] init])
		 */

		NSObject native_customclass_surface;

		[Benchmark]
		public object NativeCustomClassSurfaceCreation ()
		{

			var ptr = Messaging.IntPtr_objc_msgSend (Messaging.IntPtr_objc_msgSend (CustomClassClassHandle, Selector.GetHandle ("alloc")), Selector.GetHandle ("init"));
			obj = Runtime.GetNSObject (ptr);
			Messaging.void_objc_msgSend (ptr, Selector.GetHandle ("release"));
			return native_customclass_surface = obj;
		}

		[IterationCleanup (Target = nameof (NativeCustomClassSurfaceCreation))]
		public void NativeCustomClassSurfaceCreation_Cleanup ()
		{
			native_customclass_surface.Dispose ();
		}
	}
}
