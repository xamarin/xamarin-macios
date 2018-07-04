// Copyright 2011, 2013 Xamarin Inc. All rights reserved

using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;


#if XAMCORE_2_0
using Foundation;
using Security;
using ObjCRuntime;
#else
using MonoTouch;
using MonoTouch.Foundation;
using MonoTouch.Security;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Security {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class KeyChainTest {
		
		[DllImport (Constants.CoreFoundationLibrary)]
		extern static int CFGetRetainCount (IntPtr handle);

		[Test]
		public void Add_Certificate ()
		{
#if MONOMAC
			Stream certStream = typeof (KeyChainTest).Assembly.GetManifestResourceStream ("xammac_tests.Security.openssl_crt.der");
#else
			Stream certStream = typeof(KeyChainTest).Assembly.GetManifestResourceStream ("monotouchtest.Security.openssl_crt.der");
#endif
			NSData data = NSData.FromStream (certStream);

			var rec = new SecRecord (SecKind.Certificate) {
				Label = "MyCert"
			};
			rec.SetValueRef (new SecCertificate (data));

			var rc = SecKeyChain.Add (rec);
			Assert.IsTrue (rc == SecStatusCode.Success || rc == SecStatusCode.DuplicateItem, "Add_Certificate");
		}

#if !MONOMAC // No QueryAsConcreteType on Mac
		[Test]
		public void AddQueryRemove_Identity ()
		{
			using (SecRecord rec = new SecRecord (SecKind.Identity))
			using (var id = IdentityTest.GetIdentity ()) {
				rec.SetValueRef (id);
				SecStatusCode code = SecKeyChain.Add (rec);
				Assert.True (code == SecStatusCode.DuplicateItem || code == SecStatusCode.Success);
			}

			if (!TestRuntime.CheckXcodeVersion (5, 0))
				Assert.Inconclusive ("QueryAsConcreteType does not work before iOS7");

			using (SecRecord rec = new SecRecord (SecKind.Identity)) {
				SecStatusCode code;
				var match = SecKeyChain.QueryAsConcreteType (rec, out code);
				if ((match == null) && (code == SecStatusCode.ItemNotFound))
					Assert.Inconclusive ("Test randomly fails (race condition between addtion/commit/query?");

				Assert.That (code, Is.EqualTo (SecStatusCode.Success), "QueryAsRecord-2");
				Assert.NotNull (match, "match-2");

				code = SecKeyChain.Remove (rec);
				Assert.That (code, Is.EqualTo (SecStatusCode.Success), "Remove");

				match = SecKeyChain.QueryAsConcreteType (rec, out code);
				Assert.That (code, Is.EqualTo (SecStatusCode.ItemNotFound), "QueryAsRecord-3");
				Assert.Null (match, "match-3");
			}
		}
#endif

		[DllImport ("/System/Library/Frameworks/Security.framework/Security")]
		internal extern static SecStatusCode SecItemAdd (IntPtr cfDictRef, IntPtr result);
		
		[Test]
		// same as Add_Identity but directly p/invoking - shows that the type MUST NOT be included for Identity
		public void SecItemAdd_Identity ()
		{
			using (NSString valueref = new NSString ("v_Ref"))
			using (NSMutableDictionary data = new NSMutableDictionary ())
			using (var id = IdentityTest.GetIdentity ()) {
				data.LowlevelSetObject (id.Handle, valueref.Handle);
				SecStatusCode code = SecItemAdd (data.Handle, IntPtr.Zero);
				var expected = Is.EqualTo (SecStatusCode.DuplicateItem).Or.EqualTo (SecStatusCode.Success);
#if __MACOS__
				if (!TestRuntime.CheckSystemVersion (PlatformName.MacOSX, 10, 9))
					expected = Is.EqualTo (SecStatusCode.Param);
#endif
				Assert.That (code, expected);
			}
		}
		
		static Guid GetID()
		{           
			Guid returnGuid = Guid.Empty;
			SecStatusCode code;
			SecRecord queryRec = new SecRecord (SecKind.GenericPassword) { 
				Service = "KEYCHAIN_SERVICE", 
				Label = "KEYCHAIN_SERVICE", 
				Account = "KEYCHAIN_ACCOUNT" 
			};
			queryRec = SecKeyChain.QueryAsRecord (queryRec, out code);
			
			if (code == SecStatusCode.Success && queryRec != null && queryRec.Generic != null )
			{
				returnGuid = new Guid(NSString.FromData(queryRec.Generic, NSStringEncoding.UTF8));
			}
			
			return returnGuid;
		}

		[Test]
		public void QueryAsData ()
		{
			SecStatusCode code;
			SecRecord queryRec = new SecRecord (SecKind.GenericPassword) {
				Service = "KEYCHAIN_SERVICE",
				Label = "KEYCHAIN_SERVICE",
				Account = "KEYCHAIN_ACCOUNT"
			};
			var data = SecKeyChain.QueryAsData (queryRec, true, out code);
			if (code == SecStatusCode.Success && queryRec != null) {
				Assert.NotNull (data.Bytes);
			}
		}

		[Test]
		public void QueryAsDataArray ()
		{
			SecStatusCode code;
			SecRecord queryRec = new SecRecord (SecKind.GenericPassword) {
				Service = "KEYCHAIN_SERVICE",
				Label = "KEYCHAIN_SERVICE",
				Account = "KEYCHAIN_ACCOUNT"
			};
			var data = SecKeyChain.QueryAsData (queryRec, true, 1, out code);
			if (code == SecStatusCode.Success && queryRec != null) {
				Assert.NotNull (data [0].Bytes);
			}
		}
		
		static SecStatusCode SetID (Guid setID)
		{
			var queryRec = new SecRecord (SecKind.GenericPassword) { 
				Service = "KEYCHAIN_SERVICE", 
				Label = "KEYCHAIN_SERVICE", 
				Account = "KEYCHAIN_ACCOUNT" 
			};
			var record = queryRec.Clone ();
			record.Generic = NSData.FromString (Convert.ToString (setID), NSStringEncoding.UTF8);
			record.Accessible = SecAccessible.Always;
			SecStatusCode code = SecKeyChain.Add (record);
			if (code == SecStatusCode.DuplicateItem) {
				code = SecKeyChain.Remove (queryRec);
				if (code == SecStatusCode.Success)
					code = SecKeyChain.Add (record);
			}
			return code;
		}
		
		[Test]
		public void CheckId ()
		{
			TestRuntime.AssertXcodeVersion (5, 1); // macOS 10.9
			// test case from http://stackoverflow.com/questions/9481860/monotouch-cant-get-value-of-existing-keychain-item
			// not a bug (no class lib fix) just a misuse of the API wrt status codes
			Guid g = Guid.NewGuid ();
			SetID (g);
			Assert.That (g, Is.EqualTo (GetID ()), "same guid");
		}
	}
}
