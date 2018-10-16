// Copyright 2014-2015 Xamarin Inc. All rights reserved

using System;
#if XAMCORE_2_0
using Foundation;
using Security;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
#else
using MonoTouch.Foundation;
using MonoTouch.Security;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Security.Cryptography.X509Certificates;

namespace MonoTouchFixtures.Security {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RecordTest {

		[Test]
		public void Identity ()
		{
			var rec = new SecRecord (SecKind.Identity) {
				Account = "Username",
				ValueData = NSData.FromString ("Password"),
			};

			// prior to iOS7 you had to deal without the class
			using (var dict = rec.ToDictionary ()) {
				var hasIdnt = true;
#if !__MACOS__
				if (!TestRuntime.CheckXcodeVersion (5, 0))
					hasIdnt = false;
#endif
				if (hasIdnt)
					Assert.That (dict ["class"].ToString (), Is.EqualTo ("idnt"), "idnt");
				else
					Assert.Null (dict ["class"], "idnt");
			}
		}

		void Accessible (SecAccessible access)
		{
			var rec = new SecRecord (SecKind.GenericPassword) {
				Account = "Username"
			};
			SecKeyChain.Remove (rec); // it might already exists (or not)

			rec = new SecRecord (SecKind.GenericPassword) {
				Account = "Username",
				ValueData = NSData.FromString ("Password"),
				Accessible = access
			};

			Assert.That (SecKeyChain.Add (rec), Is.EqualTo (SecStatusCode.Success), "Add");

			SecStatusCode code;
			var match = SecKeyChain.QueryAsRecord (rec, out code);
			Assert.That (code, Is.EqualTo (SecStatusCode.Success), "QueryAsRecord");

			Assert.That (match.Accessible, Is.EqualTo (access), "Accessible");
		}

		[Test]
		public void Match ()
		{
			var rec = new SecRecord (SecKind.GenericPassword) {
				Account = "Username",
			};
			Assert.Null (rec.MatchIssuers, "MatchIssuers");
			// we do not have a way (except the getter) to craete SecKeyChain instances
			Assert.Null (rec.MatchItemList, "MatchItemList");

			using (var data = new NSData ()) {
				rec.MatchIssuers = new NSData[] { data };
				Assert.AreSame (rec.MatchIssuers [0], data, "MatchIssuers [0]");
			}

			if (!TestRuntime.CheckXcodeVersion (7, 0))
				return;
			Assert.That (rec.AuthenticationUI, Is.EqualTo (SecAuthenticationUI.NotSet), "AuthenticationUI-get");
			rec.AuthenticationUI = SecAuthenticationUI.Allow;
			Assert.That (rec.AuthenticationUI, Is.EqualTo (SecAuthenticationUI.Allow), "AuthenticationUI-set");
		}

		[Test]
#if MONOMAC
		[Ignore ("Returns SecAccessible.Invalid")]
#endif
		public void Accessible_17579 ()
		{
			Accessible (SecAccessible.AfterFirstUnlock);
			Accessible (SecAccessible.AfterFirstUnlockThisDeviceOnly);
			Accessible (SecAccessible.Always);
			Accessible (SecAccessible.AlwaysThisDeviceOnly);
			Accessible (SecAccessible.WhenUnlocked);
			Accessible (SecAccessible.WhenUnlockedThisDeviceOnly);
		}

		void Protocol (SecProtocol protocol)
		{
			var rec = new SecRecord (SecKind.InternetPassword) {
				Account = "Protocol"
			};
			SecKeyChain.Remove (rec); // it might already exists (or not)

			rec = new SecRecord (SecKind.InternetPassword) {
				Account = "Protocol",
				ValueData = NSData.FromString ("Password"),
				Protocol = protocol,
				Server = "www.xamarin.com"
			};

			Assert.That (SecKeyChain.Add (rec), Is.EqualTo (SecStatusCode.Success), "Add");

			SecStatusCode code;
			var match = SecKeyChain.QueryAsRecord (rec, out code);
			Assert.That (code, Is.EqualTo (SecStatusCode.Success), "QueryAsRecord");

			Assert.That (match.Protocol, Is.EqualTo (protocol), "Protocol");
		}

		[Test]
		public void Protocol_17579 ()
		{
			Protocol (SecProtocol.Afp);
			Protocol (SecProtocol.AppleTalk);
			Protocol (SecProtocol.Daap);
			Protocol (SecProtocol.Eppc);
			Protocol (SecProtocol.Ftp);
			Protocol (SecProtocol.FtpAccount);
			Protocol (SecProtocol.FtpProxy);
			Protocol (SecProtocol.Ftps);
			Protocol (SecProtocol.Http);
			Protocol (SecProtocol.HttpProxy);
			Protocol (SecProtocol.Https);
			Protocol (SecProtocol.HttpsProxy);
			Protocol (SecProtocol.Imap);
			Protocol (SecProtocol.Imaps);
			Protocol (SecProtocol.Ipp);
			Protocol (SecProtocol.Irc);
			Protocol (SecProtocol.Ircs);
			Protocol (SecProtocol.Ldap);
			Protocol (SecProtocol.Ldaps);
			Protocol (SecProtocol.Nntp);
			Protocol (SecProtocol.Nntps);
			Protocol (SecProtocol.Pop3);
			Protocol (SecProtocol.Pop3s);
			Protocol (SecProtocol.Rtsp);
			Protocol (SecProtocol.RtspProxy);
			Protocol (SecProtocol.Smb);
			Protocol (SecProtocol.Smtp);
			Protocol (SecProtocol.Socks);
			Protocol (SecProtocol.Ssh);
			Protocol (SecProtocol.Telnet);
			Protocol (SecProtocol.Telnets);
		}

		void AuthenticationType (SecAuthenticationType type)
		{
			var rec = new SecRecord (SecKind.InternetPassword) {
				Account = "AuthenticationType"
			};
			SecKeyChain.Remove (rec); // it might already exists (or not)

			rec = new SecRecord (SecKind.InternetPassword) {
				Account = "AuthenticationType",
				ValueData = NSData.FromString ("Password"),
				AuthenticationType = type,
				Server = "www.xamarin.com"
			};

			Assert.That (SecKeyChain.Add (rec), Is.EqualTo (SecStatusCode.Success), "Add");

			SecStatusCode code;
			var match = SecKeyChain.QueryAsRecord (rec, out code);
			Assert.That (code, Is.EqualTo (SecStatusCode.Success), "QueryAsRecord");

			Assert.That (match.AuthenticationType, Is.EqualTo (type), "AuthenticationType");
		}

		[Test]
		public void AuthenticationType_17579 ()
		{
			AuthenticationType (SecAuthenticationType.Default);
			AuthenticationType (SecAuthenticationType.Dpa);
			AuthenticationType (SecAuthenticationType.HtmlForm);
			AuthenticationType (SecAuthenticationType.HttpBasic);
			AuthenticationType (SecAuthenticationType.HttpDigest);
			AuthenticationType (SecAuthenticationType.Msn);
			AuthenticationType (SecAuthenticationType.Ntlm);
			AuthenticationType (SecAuthenticationType.Rpa);
		}

		// Test Case provided by user
		// This test case scenario used to fail under iOS 6 or lower
		[Test]
		public void DeskCase_83099_InmutableDictionary ()
		{
			var testUsername = "testusername";

			//TEST 1: Save a keychain value
			var test1 = SaveUserPassword (testUsername, "testValue1");
			Assert.IsTrue (test1, "Password could not be saved to keychain");

			//TEST 2: Get the saved keychain value
			var test2 = GetUserPassword (testUsername);
			Assert.IsTrue (StringUtil.StringsEqual (test2, "testValue1", false));

			//TEST 3: Update the keychain value
			var test3 = SaveUserPassword (testUsername, "testValue2");
			Assert.IsTrue (test3, "Password could not be saved to keychain");

			//TEST 4: Get the updated keychain value
			var test4 = GetUserPassword (testUsername);
			Assert.IsTrue (StringUtil.StringsEqual (test4, "testValue2", false));

			//TEST 5: Clear the keychain values
			var test5 = ClearUserPassword (testUsername);
			Assert.IsTrue (test5, "Password could not be cleared from keychain");

			//TEST 6: Verify no keychain value
			var test6 = GetUserPassword (testUsername);
			Assert.IsNull (test6, "No password should exist here");
		}

		public static string GetUserPassword (string username)
		{
			string password = null;
			var searchRecord = new SecRecord (SecKind.InternetPassword) {
				Server = "Test1",
				Account = username.ToLower()
			};
			SecStatusCode code;
			var record = SecKeyChain.QueryAsRecord(searchRecord, out code);
			if (code == SecStatusCode.Success && record != null)
				password = NSString.FromData (record.ValueData, NSStringEncoding.UTF8);
			return password;
		}

		public static bool SaveUserPassword (string username, string password)
		{
			var success = false;
			var searchRecord = new SecRecord (SecKind.InternetPassword) {
				Server = "Test1",
				Account = username.ToLower ()
			};
			SecStatusCode queryCode;
			var record = SecKeyChain.QueryAsRecord (searchRecord, out queryCode);
			if (queryCode == SecStatusCode.ItemNotFound) {
				record = new SecRecord (SecKind.InternetPassword) {
					Server = "Test1",
					Account = username.ToLower (),
					ValueData = NSData.FromString (password)
				};
				var addCode = SecKeyChain.Add (record);
				success = (addCode == SecStatusCode.Success);
			}
			if (queryCode == SecStatusCode.Success && record != null) {
				record.ValueData = NSData.FromString (password);
				var updateCode = SecKeyChain.Update (searchRecord, record);
				success = (updateCode == SecStatusCode.Success);
			}
			return success;
		}

		public static bool ClearUserPassword (string username)
		{
			var success = false;
			var searchRecord = new SecRecord (SecKind.InternetPassword) {
				Server = "Test1",
				Account = username.ToLower ()
			};
			SecStatusCode queryCode;
			var record = SecKeyChain.QueryAsRecord (searchRecord, out queryCode);

			if (queryCode == SecStatusCode.Success && record != null) {
				var removeCode = SecKeyChain.Remove (searchRecord);
				success = (removeCode == SecStatusCode.Success);
			}
			return success;
		}

		[Test]
#if MONOMAC
		[Ignore ("SecStatusCode code = SecKeyChain.Add (rec); returns SecStatusCode.Param")]
#endif
		public void IdentityRecordTest ()
		{
			using (var identity = IdentityTest.GetIdentity ())
			using (var rec = new SecRecord (identity)) {
				SecStatusCode code = SecKeyChain.Add (rec);
				Assert.True (code == SecStatusCode.DuplicateItem || code == SecStatusCode.Success, "Identity added");

				var ret = rec.GetIdentity ();
				Assert.NotNull (ret, "ret is null");
				Assert.That (identity.Handle, Is.EqualTo (ret.Handle), "Same Handle");

				Assert.Throws<InvalidOperationException> (() => rec.GetKey (), "GetKey should throw");
				Assert.Throws<InvalidOperationException> (() => rec.GetCertificate (), "GetCertificate should throw");
			}
		}

#if !MONOMAC // Works different on Mac
		[Test]
		public void SecRecordRecordTest ()
		{
			using (var cert = new X509Certificate (CertificateTest.mail_google_com))
			using (var sc = new SecCertificate (cert))
			using (var rec = new SecRecord (sc)) {
				Assert.NotNull (rec, "rec is null");

				var ret = rec.GetCertificate ();
				Assert.That (ret.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
				Assert.That (ret.Handle, Is.EqualTo (cert.Handle), "Same Handle");
				Assert.That (cert.ToString (true), Is.EqualTo (ret.ToX509Certificate ().ToString (true)), "X509Certificate");

				Assert.Throws<InvalidOperationException> (() => rec.GetKey (), "GetKey should throw");
				Assert.Throws<InvalidOperationException> (() => rec.GetIdentity (), "GetIdentity should throw");
			}
		}

		[Test]
		public void KeyRecordTest ()
		{
			using (var cert = new X509Certificate2 (ImportExportTest.farscape_pfx, "farscape"))
			using (var policy = SecPolicy.CreateBasicX509Policy ())
			using (var trust = new SecTrust (cert, policy)) {
				trust.Evaluate ();
				using (SecKey pubkey = trust.GetPublicKey ())
				using (var rec = new SecRecord (pubkey)) {
					Assert.NotNull (rec, "rec is null");

					var ret = rec.GetKey ();
					Assert.That (ret.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
					Assert.That (ret.Handle, Is.EqualTo (pubkey.Handle), "Same Handle");

					Assert.Throws<InvalidOperationException> (() => rec.GetCertificate (), "GetCertificate should throw");
					Assert.Throws<InvalidOperationException> (() => rec.GetIdentity (), "GetIdentity should throw");
				}
			}
		}
#endif
	}
}
