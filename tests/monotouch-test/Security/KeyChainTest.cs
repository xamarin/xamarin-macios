// Copyright 2011, 2013 Xamarin Inc. All rights reserved

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

using CoreFoundation;
using Foundation;
using Security;
using ObjCRuntime;
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

			var query = new SecRecord (SecKind.Certificate) {
				Label = $"Internet Widgits Pty Ltd",
			};
			var rec = query.Clone ();
			rec.SetValueRef (new SecCertificate (data));

			try {
				// delete any existing certificates first.
				SecKeyChain.Remove (query);
				// add the new certificate
				var rc = SecKeyChain.Add (rec);
				Assert.That (rc, Is.EqualTo (SecStatusCode.Success), "Add_Certificate");
			} finally {
				// clean up after ourselves
				SecKeyChain.Remove (query);
			}
		}

#if !MONOMAC // No QueryAsConcreteType on Mac
		[Test]
		public void AddQueryRemove_Identity ()
		{
			using (SecRecord rec = new SecRecord (SecKind.Identity))
			using (var id = IdentityTest.GetIdentity ()) {
				rec.SetValueRef (id);
				SecStatusCode code = SecKeyChain.Add (rec);
				Assert.That (code, Is.EqualTo (SecStatusCode.DuplicateItem).Or.EqualTo (SecStatusCode.Success), "code");
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

		string uniqueString;
		string UniqueString {
			get {
				if (uniqueString == null)
					uniqueString = $"{CFBundle.GetMain ().Identifier}-{GetType ().FullName}-{Process.GetCurrentProcess ().Id}";
				return uniqueString;
			}
		}

		string RecordLabel {
			get {
				return $"{UniqueString}-Label";
			}
		}

		// The uniqueness of GenericPassword entries are based on Service+Account (only).
		// Here we have a per-process + per-test unique Service value,
		// and a constant Account value (which makes it easier to find all entries if need be).
		string RecordService {
			get {
				return $"{UniqueString}-Service";
			}
		}

		string RecordAccount {
			get {
				return $"XAMARIN_KEYCHAIN_ACCOUNT";
			}
		}

		Guid GetID ()
		{
			SecStatusCode code;
			SecRecord queryRec = new SecRecord (SecKind.GenericPassword) { 
				Service = RecordService,
				Account = RecordAccount,
			};
			var queryResponse = SecKeyChain.QueryAsRecord (queryRec, out code);
			if (code == SecStatusCode.Success && queryResponse?.Generic != null)
				return new Guid (NSString.FromData (queryResponse.Generic, NSStringEncoding.UTF8));
			
			return Guid.Empty;
		}

		[Test]
		public void QueryAsData ()
		{
			SecStatusCode code;
			SecRecord queryRec = new SecRecord (SecKind.GenericPassword) {
				Service = RecordService,
				Account = RecordAccount,
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
				Service = RecordService,
				Account = RecordAccount,
			};
			var data = SecKeyChain.QueryAsData (queryRec, true, 1, out code);
			if (code == SecStatusCode.Success && queryRec != null) {
				Assert.NotNull (data [0].Bytes);
			}
		}

		SecStatusCode RemoveID ()
		{
			var queryRec = new SecRecord (SecKind.GenericPassword) {
				Service = RecordService,
				Account = RecordAccount,
			};
			return SecKeyChain.Remove (queryRec);
		}

		SecStatusCode SetID (Guid setID)
		{
			var queryRec = new SecRecord (SecKind.GenericPassword) {
				Service = RecordService,
				Account = RecordAccount,
			};
			var record = queryRec.Clone ();
			record.Generic = NSData.FromString (Convert.ToString (setID), NSStringEncoding.UTF8);
			record.Accessible = SecAccessible.Always;
			record.Label = RecordLabel;

			Query (queryRec, "SetID 1 - before add");

			SecStatusCode code = SecKeyChain.Add (record);

			Query (queryRec, $"SetID 1 - after add, rv: {code}");

			if (code == SecStatusCode.DuplicateItem) {
				code = RemoveID ();
				Query (queryRec, $"SetID 1 - after remove, rv: {code}");
				if (code == SecStatusCode.Success) {
					code = SecKeyChain.Add (record);
					Query (queryRec, $"SetID 1 - after readd, rv: {code}");
				}
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
			Query ("CheckID before add");
			try {
				Assert.That (SetID (g), Is.EqualTo (SecStatusCode.Success), "success");
				Query ("CheckID after add");
				Assert.That (GetID (), Is.EqualTo (g), "same guid");
			} finally {
				RemoveID ();
				Query ("CheckID after cleanup");
			}
		}

		void Query (SecRecord query, string name = "Query.")
		{
			Console.WriteLine ($"{name} Service: {query.Service} Label: {query.Label} Account: {query.Account}");
			var records = SecKeyChain.QueryAsRecord (query, 10, out var code);
			if (records != null) {
				Console.WriteLine ($"    Query result: {code}. Got back {records?.Length} records:");
				if (records != null) {
					for (var i = 0; i < records.Length; i++) {
						var rec = records [i];
						Console.WriteLine ($"        #{i + 1}: {rec} - Service: {rec.Service} Label: {rec.Label} Account: {rec.Account}");
					}
				}
			} else {
				Console.WriteLine ($"    Query result: {code}. No results.");
			}
		}

		void Query (string name = "Query:")
		{
			var queryRec = new SecRecord (SecKind.GenericPassword) {
				Service = "KEYCHAIN_SERVICE",
				Label = RecordLabel,
				Account = "KEYCHAIN_ACCOUNT"
			};
			Query (queryRec, name);


			queryRec = new SecRecord (SecKind.GenericPassword) {
				Label = RecordLabel,
				Account = "KEYCHAIN_ACCOUNT"
			};

			queryRec = new SecRecord (SecKind.GenericPassword) {
				Service = "KEYCHAIN_SERVICE",
				Account = "KEYCHAIN_ACCOUNT"
			};
			Query (queryRec, name);

			queryRec = new SecRecord (SecKind.GenericPassword) {
				Service = "KEYCHAIN_SERVICE",
				Label = RecordLabel,
			};
			Query (queryRec, name);


			queryRec = new SecRecord (SecKind.GenericPassword) {
				Account = "KEYCHAIN_ACCOUNT"
			};
			Query (queryRec, name);

			queryRec = new SecRecord (SecKind.GenericPassword) {
				Label = RecordLabel,
			};
			Query (queryRec, name);
			queryRec = new SecRecord (SecKind.GenericPassword) {
				Service = "KEYCHAIN_SERVICE",
			};
			Query (queryRec, name);
		}
	}
}
