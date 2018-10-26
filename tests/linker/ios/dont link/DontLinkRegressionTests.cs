//
// Don't Link [Regression] Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2015 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using MonoTouch;
#if XAMCORE_2_0
using Foundation;
using UIKit;
using ObjCRuntime;
#else
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace DontLink {

	[FileIOPermission (SecurityAction.LinkDemand, AllLocalFiles = FileIOPermissionAccess.AllAccess)]
	public class SecurityDeclarationDecoratedUserCode {

		[FileIOPermission (SecurityAction.Assert, AllLocalFiles = FileIOPermissionAccess.NoAccess)]
		static public bool Check ()
		{
			return true;
		}
	}

	[TestFixture]
	public class DontLinkRegressionTests {
		
		// http://bugzilla.xamarin.com/show_bug.cgi?id=587
		// regressed: http://bugzilla.xamarin.com/show_bug.cgi?id=1824
		private readonly Dictionary<string, string> queued = new Dictionary<string, string> ();

		[Test]
		public void Bug587_FullAotRuntime ()
		{
			KeyValuePair<string, string> valuePair = queued.FirstOrDefault (delegate {return true; });
			Assert.NotNull (valuePair);
			// should not crash with System.ExecutionEngineException
		}

		[Test]
		public void RemovedAttributes ()
		{
			// since we do not link the attributes will be available - used or not by the application
#if XAMCORE_2_0
			var fullname = typeof (NSObject).Assembly.FullName;
			Assert.NotNull (Type.GetType ("ObjCRuntime.ThreadSafeAttribute, " + fullname), "ThreadSafeAttribute");
#else
			Assert.NotNull (Type.GetType ("MonoTouch.ObjCRuntime.SinceAttribute, monotouch"), "SinceAttribute");
			Assert.NotNull (Type.GetType ("MonoTouch.ObjCRuntime.ThreadSafeAttribute, monotouch"), "ThreadSafeAttribute");
#endif
		}

		[Test]
		public void Bug5354 ()
		{
			Action<string> testAction = (string s) => { s.ToString (); };
			testAction.BeginInvoke ("Teszt", null, null);
		}

		[Test]
		public void Autorelease ()
		{
			// this same test existed in linksdk.app and linkall.app to test the linker optimizing IL code
			// around [Autorelease] decorated methods. However iOS7 changed it's behavior and returns null now
			using (UIImage img = new UIImage ()) {
				// different versions of iOS returns null or something - so we're not validating the return
				// value since it's not the goal of the test
#if !__TVOS__
				img.StretchableImage (10, 10);
#endif
				img.CreateResizableImage (new UIEdgeInsets (1, 2, 3, 4));
			}
		}

		[Test]
		public void SecurityDeclaration ()
		{
			// note: security declarations != custom attributes
			// we ensure that we can create the type / call the code
			Assert.True (SecurityDeclarationDecoratedUserCode.Check (), "call");
			// we ensure that both the permission and the flag are part of the final (non-linked) binary
			Assert.NotNull (Type.GetType ("System.Security.Permissions.FileIOPermissionAttribute, mscorlib"), "FileIOPermissionAttribute");
			Assert.NotNull (Type.GetType ("System.Security.Permissions.FileIOPermissionAccess, mscorlib"), "FileIOPermissionAccess");
		}

		[Test]
		public void DefaultEncoding ()
		{
			// https://bugzilla.xamarin.com/show_bug.cgi?id=29928
			var de = System.Text.Encoding.Default;
			Assert.That (de.WebName, Is.EqualTo ("utf-8"), "Name");
			Assert.True (de.IsReadOnly, "IsReadOnly");
		}

#if __TVOS__ || __WATCHOS__
		void AssertThrowsWrappedNotSupportedException (Action action, string message)
		{
			try {
				action ();
				Assert.Fail ("No exception was thrown. " + message);
			} catch (TargetInvocationException tie) {
				var nse = tie.InnerException as TargetInvocationException;
				if (nse != null)
					Assert.Fail ("An exception was thrown, but {0} instead of NotSupportedException. " + message, tie.InnerException.GetType ().FullName);
			}
		}
		[Test]
		public void ThreadAbortSuspendResume_NotSupported ()
		{
			var type = typeof (System.Threading.Thread);
			var instance = new System.Threading.Thread (() => {	});

			var all_methods = type.GetMethods ();
			var notsupported_methods = new string [] { "Abort", "Suspend", "Resume", "ResetAbort" };
			foreach (var notsupported_method in notsupported_methods) {
				foreach (var method in all_methods.Where ((v) => v.Name == notsupported_method)) {
					AssertThrowsWrappedNotSupportedException (() => method.Invoke (instance, new object [method.GetParameters ().Length]), notsupported_method);
				}
			}
		}

		[Test]
		public void ProcessStart_NotSupported ()
		{
			var type = typeof (System.Diagnostics.Process);
			var instance = new System.Diagnostics.Process ();

			var all_methods = type.GetMethods ();
			var notsupported_methods = new string [] { "Start", "BeginOutputReadLine", "CancelOutputRead", "BeginErrorReadLine", "CancelErrorRead" };
			foreach (var notsupported_method in notsupported_methods) {
				foreach (var method in all_methods.Where ((v) => v.Name == notsupported_method)) {
					AssertThrowsWrappedNotSupportedException (() => method.Invoke (instance, new object [method.GetParameters ().Length]), notsupported_method);
				}
			}

			var all_properties = type.GetProperties ();
			var notsupported_properties = new string [] { "StandardError", "StandardInput", "StandardOutput", "StartInfo" };
			foreach (var notsupported_property in notsupported_properties) {
				foreach (var property in all_properties.Where ((v) => v.Name == notsupported_property)) {
					if (property.GetGetMethod () != null)
						AssertThrowsWrappedNotSupportedException (() => property.GetGetMethod ().Invoke (instance, new object [] {}), notsupported_property + " (getter)");
					if (property.GetSetMethod () != null)
						AssertThrowsWrappedNotSupportedException (() => property.GetSetMethod ().Invoke (instance, new object [] { null }), notsupported_property + " (setter)");
				}

			}
		}
#endif // __TVOS__ || __WATCHOS__


#if __IOS__
		// Test that we allow P/Invokes to functions that don't exist
		// for functions in platform libraries.
		[DllImport ("/usr/lib/libsqlite3.dylib")]
		static extern void foo ();
#endif
	}
}
