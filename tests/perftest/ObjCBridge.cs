using System;

using Foundation;
using ObjCRuntime;

using BenchmarkDotNet.Attributes;

using Bindings.Test;

namespace PerfTest {

	public class VM {
		[Benchmark]
		public object CreateObject ()
		{
			return new object ();
		}
	}

	public class ObjCBridge {
		[Benchmark]
		public object NSObjectCreated ()
		{
			return new NSObject ();
		}

		[Benchmark]
		public void NSObjectCreatedAndDisposed ()
		{
			using (var obj = new NSObject ()) { }
		}

		[Benchmark]
		public void CustomSubClassCreated ()
		{
			new CustomSubClass ();
		}

		[Benchmark]
		public void CustomSubClassCreatedAndDisposed ()
		{
			using (var obj = new CustomSubClass ()) { }
		}

		class CustomSubClass : NSObject {

		}

	}

	public class InvokeOnNSObject {
		NSObject obj = new NSObject ();

		[Benchmark]
		public object CallClass ()
		{
			return obj.Class;
		}
	}

	public class InvokeOnCustomClass {
		ObjCRegistrarTest obj = new ObjCRegistrarTest ();
		Subclassed subclassed = new Subclassed ();
		IntPtr subclassed_class = Class.GetHandle (typeof (Subclassed));

		[Benchmark]
		public void CallInstanceMethodInNativeCode ()
		{
			obj.V ();
		}

		[Benchmark]
		public void CallStaticMethodInNativeCode ()
		{
			ObjCRegistrarTest.StaticV ();
		}

		[Benchmark]
		public void CallOverriddenInstanceMethod ()
		{

			MessageSend.void_objc_msgSend (subclassed.Handle, Selector.GetHandle ("V"));
		}

		[Benchmark]
		public void CallExportedInstanceMethod ()
		{
			MessageSend.void_objc_msgSend (subclassed.Handle, Selector.GetHandle ("exportedInstanceMethod"));

		}

		[Benchmark]
		public void CallExportedStaticMethod ()
		{
			MessageSend.void_objc_msgSend (subclassed_class, Selector.GetHandle ("exportedStaticMethod"));
		}

		class Subclassed : ObjCRegistrarTest {
			public override void V ()
			{
			}

			[Export ("exportedInstanceMethod")]
			public void ExportedInstanceMethod ()
			{
			}

			[Export ("exportedStaticMethod")]
			public static void ExportedStaticMethod ()
			{
			}
		}
	}

	public class ArgumentMarshalling {
		ObjCRegistrarTest obj = new ObjCRegistrarTest ();
		IntPtr nsobject_class = Class.GetHandle (typeof (NSObject));

		[Benchmark]
		public object CallReturnString_EmptyString ()
		{
			return obj.GetEmptyString ();
		}

		[Benchmark]
		public object CallReturnString_ShortString ()
		{
			return obj.GetShortString ();
		}

		[Benchmark]
		public object CallReturnString_LongString ()
		{
			return obj.GetLongString ();
		}

		// CallReturnKnownManagedWrapper

		NSObject someObjectKnownManagedWrapper;
		[IterationSetup (Target = nameof (CallReturnKnownManagedWrapper))]
		public void CallReturnKnownManagedWrapperSetup ()
		{
			// Create a new object that the bridge knows about
			someObjectKnownManagedWrapper = new NSObject ();
			MessageSend.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setSomeObject:"), someObjectKnownManagedWrapper.Handle);
		}
		[Benchmark]
		public void CallReturnKnownManagedWrapper ()
		{
			// Now get that value again.
			var rv = obj.SomeObject;
		}
		[IterationCleanup (Target = nameof (CallReturnKnownManagedWrapper))]
		public void CallReturnKnownManagedWrapperCleanup ()
		{
			// cleanup after us
			someObjectKnownManagedWrapper.Dispose ();
			someObjectKnownManagedWrapper = null;
		}

		// CallReturnUnknownManagedWrapper

		IntPtr someObjectUnknownManagedWrapper;
		[IterationSetup (Target = nameof (CallReturnUnknownManagedWrapper))]
		public void CallReturnUnknownManagedWrapperSetup ()
		{
			// Create a new object that the bridge knows about
			someObjectUnknownManagedWrapper = MessageSend.IntPtr_objc_msgSend (MessageSend.IntPtr_objc_msgSend (nsobject_class, Selector.GetHandle ("alloc")), Selector.GetHandle ("init"));
			MessageSend.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setSomeObject:"), someObjectUnknownManagedWrapper);
		}
		[Benchmark]
		public void CallReturnUnknownManagedWrapper ()
		{
			// Now get that value again.
			var rv = obj.SomeObject;
		}
		[IterationCleanup (Target = nameof (CallReturnUnknownManagedWrapper))]
		public void CallReturnUnknownManagedWrapperCleanup ()
		{
			// cleanup after us
			MessageSend.void_objc_msgSend (someObjectUnknownManagedWrapper, Selector.GetHandle ("release"));
			someObjectUnknownManagedWrapper = IntPtr.Zero;
		}
	}

	public class GetArrayTest {
		ObjCRegistrarTest obj = new ObjCRegistrarTest ();
		IntPtr mutablearray_class = Class.GetHandle (typeof (NSMutableArray));
		IntPtr nsobject_class = Class.GetHandle (typeof (NSObject));
		IntPtr nativeArray;

		[Params (0, 1, 100, 10000)]
		public int ArraySize { get; set; }

		[IterationSetup (Target = nameof (CallReturnArray))]
		public void Setup ()
		{
			// Create a new array
			nativeArray = MessageSend.IntPtr_objc_msgSend (MessageSend.IntPtr_objc_msgSend (mutablearray_class, Selector.GetHandle ("alloc")), Selector.GetHandle ("init"));
			for (var i = 0; i < ArraySize; i++) {
				var element = MessageSend.IntPtr_objc_msgSend (MessageSend.IntPtr_objc_msgSend (nsobject_class, Selector.GetHandle ("alloc")), Selector.GetHandle ("init"));
				MessageSend.void_objc_msgSend_IntPtr (nativeArray, Selector.GetHandle ("addObject:"), element);
				MessageSend.void_objc_msgSend (element, Selector.GetHandle ("release"));
			}
			MessageSend.void_objc_msgSend_IntPtr (obj.Handle, Selector.GetHandle ("setSomeArray:"), nativeArray);
		}

		[Benchmark]
		public void CallReturnArray ()
		{
			var rv = obj.SomeArray;
		}

		[IterationCleanup (Target = nameof (CallReturnArray))]
		public void Cleanup ()
		{
			// cleanup after us
			MessageSend.void_objc_msgSend (nativeArray, Selector.GetHandle ("release"));
			nativeArray = IntPtr.Zero;
		}
	}

	public class SetArrayTest {
		ObjCRegistrarTest obj = new ObjCRegistrarTest ();
		IntPtr mutablearray_class = Class.GetHandle (typeof (NSMutableArray));
		IntPtr nsobject_class = Class.GetHandle (typeof (NSObject));
		NSObject [] managedArray;

		[Params (0, 1, 100, 10000)]
		public int ArraySize { get; set; }

		[GlobalSetup]
		public void Setup ()
		{
			// Create a new array
			managedArray = new NSObject [ArraySize];
			for (var i = 0; i < ArraySize; i++) {
				managedArray [i] = new NSObject ();
			}
		}

		[Benchmark]
		public void CallSetArray ()
		{
			obj.SomeArray = managedArray;
		}

		[GlobalCleanup]
		public void Cleanup ()
		{
			for (var i = 0; i < ArraySize; i++) {
				managedArray [i].Dispose ();
			}
			managedArray = null;
		}
	}
}
