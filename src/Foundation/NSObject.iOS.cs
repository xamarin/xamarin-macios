
#if !MONOMAC

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using XamCore.UIKit;
using XamCore.ObjCRuntime;

namespace XamCore.Foundation {
	public partial class NSObject : INativeObject
#if !COREBUILD
	, IDisposable
#endif
	{
#if !COREBUILD

#if !XAMCORE_4_0
		[Obsolete ("Use PlatformAssembly for easier code sharing across platforms")]
		public readonly static Assembly MonoTouchAssembly = typeof (NSObject).Assembly;
#endif

#if !XAMCORE_2_0
		const string selAccessibilityDecrement = "accessibilityDecrement";
		const string selAccessibilityIncrement = "accessibilityIncrement";
		const string selAccessibilityScroll = "accessibilityScroll";
#endif
		[DllImport ("__Internal")]
		static extern void xamarin_release_managed_ref (IntPtr handle, IntPtr managed_obj);

		[DllImport ("__Internal")]
		static extern void xamarin_create_managed_ref (IntPtr handle, IntPtr obj, bool retain);

		public static NSObject Alloc (Class kls) {
			var h = Messaging.IntPtr_objc_msgSend (kls.Handle, Selector.GetHandle (Selector.Alloc));
			return new NSObject (h, true);
		}

		public void Init () {
			if (handle == IntPtr.Zero)
				throw new Exception ("you have not allocated the native object");

			handle = Messaging.IntPtr_objc_msgSend (handle, Selector.GetHandle ("init"));
		}

		public static void InvokeInBackground (NSAction action)
		{
			// using the parameterized Thread.Start to avoid capturing
			// the 'action' parameter (it'll needlessly create an extra
			// object).
			new System.Threading.Thread ((v) =>
			{
				((NSAction) v) ();
			})
			{
				IsBackground = true,
			}.Start (action);
		}

#if !XAMCORE_2_0
		[Export ("accessibilityDecrement")]
		public virtual void AccessibilityDecrement () {
			if (IsDirectBinding) {
				Messaging.void_objc_msgSend (this.Handle, Selector.GetHandle (selAccessibilityDecrement));
			} else {
				Messaging.void_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle (selAccessibilityDecrement));
			}
		}

		[Export ("accessibilityIncrement")]
		public virtual void AccessibilityIncrement () {
			if (IsDirectBinding) {
				Messaging.void_objc_msgSend (this.Handle, Selector.GetHandle (selAccessibilityIncrement));
			} else {
					Messaging.void_objc_msgSendSuper (this.SuperHandle, Selector.GetHandle (selAccessibilityIncrement));
			}
		}

		[Export ("accessibilityScroll:")]
		public virtual bool AccessibilityScroll (UIAccessibilityScrollDirection direction) {
			if (IsDirectBinding) {
				return Messaging.bool_objc_msgSend_int (this.Handle, Selector.GetHandle (selAccessibilityScroll), (int) direction);
			} else {
					return Messaging.bool_objc_msgSendSuper_int (this.SuperHandle, Selector.GetHandle (selAccessibilityScroll), (int) direction);
			}
		}
	
		public virtual void PerformSelector (Selector sel, NSObject obj, float delay)
		{
			PerformSelector (sel, obj, (double) delay);
		}
#endif
#endif // !COREBUILD
	}
}

#endif // !MONOMAC
