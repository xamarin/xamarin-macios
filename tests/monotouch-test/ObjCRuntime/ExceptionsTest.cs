using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif
using Bindings.Test;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif

using NUnit.Framework;

#if __MACOS__
using ObjCException = Foundation.ObjCException;
#else
using ObjCException = Foundation.MonoTouchException;
#endif

namespace MonoTouchFixtures.ObjCRuntime {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ExceptionsTest {

#if __WATCHOS__
		MarshalObjectiveCExceptionMode defaultObjectiveCExceptionMode = MarshalObjectiveCExceptionMode.ThrowManagedException;
		MarshalManagedExceptionMode defaultManagedExceptionMode = MarshalManagedExceptionMode.Default;
#else
		MarshalObjectiveCExceptionMode defaultObjectiveCExceptionMode = MarshalObjectiveCExceptionMode.UnwindManagedCode;
		MarshalManagedExceptionMode defaultManagedExceptionMode = MarshalManagedExceptionMode.Default;
#endif

		static List<MarshalObjectiveCExceptionEventArgs> objcEventArgs;
		static List<MarshalManagedExceptionEventArgs> managedEventArgs;
		static MarshalManagedExceptionMode? managedTargetMode;
		static MarshalObjectiveCExceptionMode? objcTargetMode;

		static void ObjExceptionHandler (object sender, MarshalObjectiveCExceptionEventArgs args)
		{
			objcEventArgs.Add (new MarshalObjectiveCExceptionEventArgs () {
				Exception = args.Exception,
				ExceptionMode = args.ExceptionMode,
			});
			if (objcTargetMode.HasValue)
				args.ExceptionMode = objcTargetMode.Value;
		}

		static void ManagedExceptionHandler (object sender, MarshalManagedExceptionEventArgs args)
		{
			managedEventArgs.Add (new MarshalManagedExceptionEventArgs () {
				Exception = args.Exception,
				ExceptionMode = args.ExceptionMode,
			});
			if (managedTargetMode.HasValue)
				args.ExceptionMode = managedTargetMode.Value;
		}

		static void ClearExceptionData ()
		{
			objcEventArgs = new List<MarshalObjectiveCExceptionEventArgs> ();
			managedEventArgs = new List<MarshalManagedExceptionEventArgs> ();
			objcTargetMode = null;
			managedTargetMode = null;
		}

		static void InstallHandlers ()
		{
			ClearExceptionData ();
			Runtime.MarshalManagedException += ManagedExceptionHandler;
			Runtime.MarshalObjectiveCException += ObjExceptionHandler;
		}

		static void UninstallHandlers ()
		{
			Runtime.MarshalManagedException -= ManagedExceptionHandler;
			Runtime.MarshalObjectiveCException -= ObjExceptionHandler;
		}

		// Simulator/desktop only
		[Test]
		public void ObjCException ()
		{
#if !__WATCHOS__ && !__MACOS__
			if (Runtime.Arch == Arch.DEVICE)
				Assert.Ignore ("This test requires wrapper functions, which are not enabled for monotouch-test on device.");
#endif

#if !DEBUG && !__WATCHOS__ && !__MACOS__
			if (Runtime.Arch != Arch.DEVICE)
				Assert.Ignore ("This test only works in debug mode in the simulator.");
#endif
			InstallHandlers ();

			try {
				using (var e = new ObjCExceptionTest ()) {
					ObjCException thrownException = null;
					try {
						objcTargetMode = MarshalObjectiveCExceptionMode.ThrowManagedException;
						e.ThrowObjCException ();
						Assert.Fail ("managed exception not thrown");
					} catch (ObjCException ex) {
						thrownException = ex;
					}
					Assert.AreEqual ("exception was thrown", thrownException.Reason, "objc reason");
					Assert.AreEqual ("Some exception", thrownException.Name, "objc name");
					Assert.AreEqual (1, objcEventArgs.Count, "objc exception");
					Assert.AreEqual (thrownException.NSException.Handle, objcEventArgs [0].Exception.Handle, "objc exception");
					Assert.AreEqual (defaultObjectiveCExceptionMode, objcEventArgs [0].ExceptionMode, "objc mode");
					Assert.AreEqual (0, managedEventArgs.Count, "managed exception");
				}
			} finally {
				UninstallHandlers ();
			}
		}

		class ManagedExceptionTest : ObjCExceptionTest {
			public Exception Exception;
			public override void ThrowManagedException ()
			{
				Exception = new ApplicationException ("3,14");
				throw Exception;
			}
		}

		// Simulator/desktop only test
		[Test]
		public void ManagedExceptionPassthrough ()
		{
			Exception thrownException = null;

#if !__WATCHOS__ && !__MACOS__
			if (Runtime.Arch == Arch.DEVICE)
				Assert.Ignore ("This test requires wrapper functions, which are not enabled for monotouch-test on device.");
#endif

#if !DEBUG && !__WATCHOS__ && !__MACOS__
			if (Runtime.Arch != Arch.DEVICE)
				Assert.Ignore ("This test only works in debug mode in the simulator.");
#endif
			var hasDebugger = global::System.Diagnostics.Debugger.IsAttached;

			InstallHandlers ();
			try {
				using (var e = new ManagedExceptionTest ()) {
					try {
						objcTargetMode = MarshalObjectiveCExceptionMode.ThrowManagedException;
						managedTargetMode = MarshalManagedExceptionMode.ThrowObjectiveCException;
						e.InvokeManagedExceptionThrower ();
						Assert.Fail ("no exception thrown 1");
					} catch (Exception ex) {
						thrownException = ex;
					}
					Assert.AreSame (e.Exception, thrownException, "exception");
					Assert.AreEqual ("3,14", thrownException.Message, "1 thrown message");
					Assert.AreSame (typeof (ApplicationException), thrownException.GetType (), "1 thrown type");
					if (hasDebugger) {
						Assert.AreEqual (0, objcEventArgs.Count, "1 objc exception");
					} else {
						Assert.AreEqual (1, objcEventArgs.Count, "1 objc exception");
						Assert.AreEqual (defaultObjectiveCExceptionMode, objcEventArgs [0].ExceptionMode, "1 objc mode");
						Assert.AreEqual ("System.ApplicationException", objcEventArgs [0].Exception.Name, "1 objc reason");
						Assert.AreEqual ("3,14", objcEventArgs [0].Exception.Reason, "1 objc message");
					}
					if (hasDebugger) {
						Assert.AreEqual (0, managedEventArgs.Count, "1 managed count");
					} else {
						Assert.AreEqual (1, managedEventArgs.Count, "1 managed count");
						Assert.AreEqual (defaultManagedExceptionMode, managedEventArgs [0].ExceptionMode, "1 managed mode");
						Assert.AreSame (thrownException, managedEventArgs [0].Exception, "1 managed exception");
					}

					ClearExceptionData ();
					try {
						objcTargetMode = MarshalObjectiveCExceptionMode.ThrowManagedException;
						managedTargetMode = MarshalManagedExceptionMode.ThrowObjectiveCException;
						e.InvokeManagedExceptionThrowerAndRethrow ();
						Assert.Fail ("no exception thrown 2");
					} catch (Exception ex) {
						thrownException = ex;
					}
					if (hasDebugger) {
						Assert.AreSame (e.Exception, thrownException, "exception");
					} else {
						Assert.AreNotSame (e.Exception, thrownException, "exception");
						Assert.AreSame (typeof (ObjCException), thrownException.GetType (), "2 thrown type");
						Assert.AreEqual ("Caught exception", ((ObjCException) thrownException).Name, "2 thrown name");
						Assert.AreEqual ("exception was rethrown", ((ObjCException) thrownException).Reason, "2 thrown reason");
					}
					if (hasDebugger) {
						Assert.AreEqual (0, objcEventArgs.Count, "2 objc exception");
					} else {
						Assert.AreEqual (1, objcEventArgs.Count, "2 objc exception");
						Assert.AreEqual (defaultObjectiveCExceptionMode, objcEventArgs [0].ExceptionMode, "2 objc mode");
						Assert.AreEqual ("Caught exception", objcEventArgs [0].Exception.Name, "2 objc reason");
						Assert.AreEqual ("exception was rethrown", objcEventArgs [0].Exception.Reason, "2 objc message");
					}
					if (hasDebugger) {
						Assert.AreEqual (0, managedEventArgs.Count, "2 managed count");
					} else {
						Assert.AreEqual (1, managedEventArgs.Count, "2 managed count");
						Assert.AreEqual (defaultManagedExceptionMode, managedEventArgs [0].ExceptionMode, "2 managed mode");
						Assert.AreSame (e.Exception, managedEventArgs [0].Exception, "2 managed exception");
					}

					ClearExceptionData ();
					if (!hasDebugger) {
						objcTargetMode = MarshalObjectiveCExceptionMode.ThrowManagedException;
						managedTargetMode = MarshalManagedExceptionMode.ThrowObjectiveCException;
						e.InvokeManagedExceptionThrowerAndCatch (); // no exception.
						Assert.AreEqual (0, objcEventArgs.Count, "3 objc exception");
						Assert.AreEqual (1, managedEventArgs.Count, "3 managed count");
						Assert.AreEqual (defaultManagedExceptionMode, managedEventArgs [0].ExceptionMode, "3 managed mode");
						Assert.AreSame (e.Exception, managedEventArgs [0].Exception, "3 managed exception");
					}
				}
			} finally {
				UninstallHandlers ();      
			}
		}
	}
}
