#if !__WATCHOS__
using System;
using System.Collections.Generic;
using System.Threading;
#if XAMCORE_2_0
using CoreFoundation;
using Foundation;
using Network;
using ObjCRuntime;
using Security;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.Network;
using MonoTouch.Security;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Network
{
	[TestFixture]
	[Preserve(AllMembers = true)]
	public class NWTxtRecordTest
	{
		NWTxtRecord record;
		string randomKey = "MyData";
		[SetUp]
		public void SetUp ()
		{
			record = NWTxtRecord.CreateDictionary ();
            record.SetValue (randomKey, new byte[3] { 0, 0, 0 });
		}

		[Test]
		public void TestFromBytes ()
		{
			// get the raw data from the dictionary create txt record, and recreate a new one
			ReadOnlyMemory<byte> rawData = null;
			record.GetRawBytes (
				(d) => {
					rawData = d;
				}
			);
            Assert.AreNotEqual (0, rawData.Length, "Raw data length.");
			// do the required conversion
			using (var otherRecord = NWTxtRecord.FromBytes (rawData))
	            Assert.IsTrue (record.Equals (otherRecord), "Equals");
		}

		[TearDown]
		public void TearDown ()
		{
			record?.Dispose ();
		}

		[Test]
		public void TestMissingKey () => Assert.AreEqual (NWTxtRecordFindKey.NotPresent, record.FindKey ("foo"));

		[Test]
		public void TestPresentKey () => Assert.AreEqual (NWTxtRecordFindKey.NonEmptyValue, record.FindKey (randomKey));

		[Test]
		public void TestSetValueByteValue ()
		{
			var data = new byte [] {10, 20, 30, 40};
			var mySecondKey = "secondKey";
			Assert.True (record.SetValue (mySecondKey, data), "SetValue");
			Assert.AreEqual (NWTxtRecordFindKey.NonEmptyValue, record.FindKey (mySecondKey));
		}

		[Test]
		public void TestSetValueNoValue ()
		{
			var mySecondKey = "secondLKey";
			Assert.True (record.SetValue (mySecondKey), "SetValue");
			Assert.AreEqual (NWTxtRecordFindKey.NoValue, record.FindKey (mySecondKey));
		}

		[Test]
		public void TestSetValueStringValue ()
		{
			var data = "hello";
			var mySecondKey = "secondLKey";
			Assert.True (record.SetValue (mySecondKey, data), "SetValue");
			Assert.AreEqual (NWTxtRecordFindKey.NonEmptyValue, record.FindKey (mySecondKey));
		}

		[Test]
		public void TestSetValueNullStringValue ()
		{
			string data = null;
			var mySecondKey = "secondLKey";
			Assert.True (record.SetValue (mySecondKey, data), "SetValue");
			Assert.AreEqual (NWTxtRecordFindKey.NoValue, record.FindKey (mySecondKey));
		}

		[Test]
		public void TestRemoveMissingKey () => Assert.IsFalse (record.RemoveValue ("NotPresentKey"));

		[Test]
		public void TestRemovePresentKey ()
		{
			Assert.True (record.RemoveValue (randomKey), "Remove");
			Assert.AreEqual (NWTxtRecordFindKey.NotPresent, record.FindKey (randomKey), "FindKey");
		}

		[Test]
		public void TestKeyCount () => Assert.AreEqual (1, record.KeyCount);

		[Test]
		public void TestIsDictionary () => Assert.IsTrue (record.IsDictionary);

		[Test]
		public void TestNotNullEquals () => Assert.IsFalse (record.Equals (null));

		[Test]
		public void TestApply ()
		{
			// fill the txt with several keys to be iterated
			var keys = new List<string> {"first", "second", "third", randomKey};
			foreach (var key in keys) {
				record.SetValue (key, key);
			}
			// apply and ensure that we do get all the keys
			var keyCount = 0;
			record.Apply ((k, r, v) => {
				keyCount++;
				Assert.IsTrue (keys.Contains (k), k);
			});
			Assert.AreEqual (keys.Count, keyCount, "keycount");
		}

		[Test]
		public void TestGetValueMissing ()
		{
			var missing = "missingKey";
			record.GetValue (missing, (k, r, value) => {
				Assert.AreEqual (missing, k, "key");
				Assert.AreEqual (NWTxtRecordFindKey.NotPresent, r, "result");
				Assert.AreEqual (0, value.Length, "value");
			});
		}

		[Test]
		public void TestGetValuePresent ()
		{
			record.GetValue (randomKey, (k, r, value) => {
				Assert.AreEqual (randomKey, k, "key");
				Assert.AreEqual (NWTxtRecordFindKey.NonEmptyValue, r, "result");
				Assert.AreNotEqual (0, value.Length, "value");
			});
		}

		[Test]
		public void TestGetRaw ()
		{
			ReadOnlyMemory<byte> rawData = null;
			record.GetRawBytes (
				(d) => {
					rawData = d;
				}
			);
			Assert.AreNotEqual (0, rawData.Length);
		}
	}
}
#endif