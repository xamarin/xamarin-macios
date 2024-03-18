// Copyright 2011, 2013 Xamarin Inc. All rights reserved

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Threading;

using CoreFoundation;
using Foundation;
using Security;
using ObjCRuntime;
using NUnit.Framework;
using Xamarin.Utils;

namespace MonoTouchFixtures.Security {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class KeyChainTest {

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static int CFGetRetainCount (IntPtr handle);

		[Test]
#if __MACCATALYST__
		[Ignore ("This test requires an app signed with the keychain-access-groups entitlement, and for Mac Catalyst that requires a custom provisioning profile.")]
#endif
		public void Add_Certificate ()
		{
#if MONOMAC && !NET
			Stream certStream = typeof (KeyChainTest).Assembly.GetManifestResourceStream ("xammac_tests.Security.openssl_crt.der");
#else
			Stream certStream = typeof (KeyChainTest).Assembly.GetManifestResourceStream ("monotouchtest.Security.openssl_crt.der");
#endif
			NSData data = NSData.FromStream (certStream);

			var query = RecordTest.CreateSecRecord (SecKind.Certificate,
				label: $"Internet Widgits Pty Ltd"
			);
			var rec = query.Clone ();
			rec.SetValueRef (new SecCertificate (data));

			try {
				// delete any existing certificates first.
				SecKeyChain.Remove (query);
				// add the new certificate
				SecStatusCode rc = SecKeyChain.Add (rec);
				// Try again a few times if we get SecStatusCode.DuplicateItem - we might be running in parallel with another test run in another process.
				var attemptsLeft = 10;
				while (rc == SecStatusCode.DuplicateItem && attemptsLeft-- > 0) {
					Thread.Sleep (100);
					rc = SecKeyChain.Add (rec);
				}
				Assert.That (rc, Is.EqualTo (SecStatusCode.Success), "Add_Certificate");
			} finally {
				// clean up after ourselves
				SecKeyChain.Remove (query);
			}
		}

#if !MONOMAC // No QueryAsConcreteType on Mac
		[Test]
#if __MACCATALYST__
		[Ignore ("This test requires an app signed with the keychain-access-groups entitlement, and for Mac Catalyst that requires a custom provisioning profile.")]
#endif
		public void AddQueryRemove_Identity ()
		{
			if (TestRuntime.CheckXcodeVersion (13, 0))
				Assert.Ignore ("code == errSecInternal (-26276)");
			using (var rec = RecordTest.CreateSecRecord (SecKind.Identity))
			using (var id = IdentityTest.GetIdentity ()) {
				rec.SetValueRef (id);
				SecStatusCode code = SecKeyChain.Add (rec);
				Assert.That (code, Is.EqualTo (SecStatusCode.DuplicateItem).Or.EqualTo (SecStatusCode.Success), "code");
			}

			if (!TestRuntime.CheckXcodeVersion (5, 0))
				Assert.Inconclusive ("QueryAsConcreteType does not work before iOS7");

			using (var rec = RecordTest.CreateSecRecord (SecKind.Identity)) {
				SecStatusCode code;
				var match = SecKeyChain.QueryAsConcreteType (rec, out code);
				if ((match is null) && (code == SecStatusCode.ItemNotFound))
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
#if __MACCATALYST__
		[Ignore ("This test requires an app signed with the keychain-access-groups entitlement, and for Mac Catalyst that requires a custom provisioning profile.")]
#endif
		public void SecItemAdd_Identity ()
		{
			using (NSString valueref = new NSString ("v_Ref"))
			using (NSMutableDictionary data = new NSMutableDictionary ())
			using (var id = IdentityTest.GetIdentity ()) {
				data.LowlevelSetObject (id.Handle, valueref.Handle);
				SecStatusCode code = SecItemAdd (data.Handle, IntPtr.Zero);
				var expected = Is.EqualTo (SecStatusCode.DuplicateItem).Or.EqualTo (SecStatusCode.Success);
#if __MACOS__
				if (!TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 10, 9))
					expected = Is.EqualTo (SecStatusCode.Param);
#endif
				Assert.That (code, expected);
			}
		}

		string uniqueString;
		string UniqueString {
			get {
				if (uniqueString is null)
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
			var queryRec = RecordTest.CreateSecRecord (SecKind.GenericPassword,
				service: RecordService,
				account: RecordAccount
			);
			var queryResponse = SecKeyChain.QueryAsRecord (queryRec, out code);
			if (code == SecStatusCode.Success && queryResponse?.Generic is not null)
				return new Guid (NSString.FromData (queryResponse.Generic, NSStringEncoding.UTF8));

			return Guid.Empty;
		}

		[Test]
		public void QueryAsData ()
		{
			SecStatusCode code;
			var queryRec = RecordTest.CreateSecRecord (SecKind.GenericPassword,
				service: RecordService,
				account: RecordAccount
			);
			var data = SecKeyChain.QueryAsData (queryRec, true, out code);
			if (code == SecStatusCode.Success && queryRec is not null) {
				Assert.NotNull (data.Bytes);
			}
		}

		[Test]
		public void QueryAsDataArray ()
		{
			SecStatusCode code;
			var queryRec = RecordTest.CreateSecRecord (SecKind.GenericPassword,
				service: RecordService,
				account: RecordAccount
			);
			var data = SecKeyChain.QueryAsData (queryRec, true, 1, out code);
			if (code == SecStatusCode.Success && queryRec is not null) {
				Assert.NotNull (data [0].Bytes);
			}
		}

		SecStatusCode RemoveID ()
		{
			var queryRec = RecordTest.CreateSecRecord (SecKind.GenericPassword,
				service: RecordService,
				account: RecordAccount
			);
			return SecKeyChain.Remove (queryRec);
		}

		SecStatusCode SetID (Guid setID)
		{
			var queryRec = RecordTest.CreateSecRecord (SecKind.GenericPassword,
				service: RecordService,
				account: RecordAccount
			);
			var record = queryRec.Clone ();
			record.Generic = NSData.FromString (Convert.ToString (setID), NSStringEncoding.UTF8);
			record.Accessible = SecAccessible.Always;
			record.Label = RecordLabel;
			SecStatusCode code = SecKeyChain.Add (record);
			if (code == SecStatusCode.DuplicateItem) {
				code = RemoveID ();
				if (code == SecStatusCode.Success)
					code = SecKeyChain.Add (record);
			}
			return code;
		}

		[Test]
#if __MACCATALYST__
		[Ignore ("This test requires an app signed with the keychain-access-groups entitlement, and for Mac Catalyst that requires a custom provisioning profile.")]
#endif
		public void CheckId ()
		{
			TestRuntime.AssertXcodeVersion (5, 1); // macOS 10.9
												   // test case from http://stackoverflow.com/questions/9481860/monotouch-cant-get-value-of-existing-keychain-item
												   // not a bug (no class lib fix) just a misuse of the API wrt status codes
			Guid g = Guid.NewGuid ();
			try {
				Assert.That (SetID (g), Is.EqualTo (SecStatusCode.Success), "success");
				Assert.That (GetID (), Is.EqualTo (g), "same guid");
			} finally {
				RemoveID ();
			}
		}
	}
}
