using System;
using System.Runtime.InteropServices;

using Foundation;
using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public partial class MonoRuntimeTests {
		static string cookie;

		[Test]
		public void Bug26989 ()
		{
			strlen ("abc");
			Assert.AreEqual ("ChocolateCookie", cookie);
		}

		[DllImport (Constants.libcLibrary)]
		static extern int strlen ([MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof (UTF8StringMarshaler), MarshalCookie = "ChocolateCookie")] string str);

		[Preserve (AllMembers = true)]
		class UTF8StringMarshaler : ICustomMarshaler {
			static UTF8StringMarshaler singleton;

			IntPtr ICustomMarshaler.MarshalManagedToNative (object managedObject)
			{
				return Marshal.StringToHGlobalAuto ((string) managedObject);
			}

			object ICustomMarshaler.MarshalNativeToManaged (IntPtr nativeDataPtr)
			{
				if (nativeDataPtr == IntPtr.Zero)
					return null;

				return Marshal.PtrToStringAuto (nativeDataPtr);
			}

			void ICustomMarshaler.CleanUpNativeData (IntPtr nativeDataPtr)
			{
				Marshal.FreeHGlobal (nativeDataPtr);
			}

			void ICustomMarshaler.CleanUpManagedData (object managedObject)
			{
			}

			int ICustomMarshaler.GetNativeDataSize ()
			{
				throw new NotImplementedException ();
			}

			public static ICustomMarshaler GetInstance (string cookie)
			{
				MonoRuntimeTests.cookie = cookie;
				if (singleton is null)
					return singleton = new UTF8StringMarshaler ();
				return singleton;
			}
		}
	}
}
