#if !__WATCHOS__
using System.Collections.Generic;
using System.Threading;

using Foundation;
using Network;

using NUnit.Framework;

namespace MonoTouchFixtures.Network {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NWTxtRecordTest {
		NWTxtRecord record;
		string randomKey = "MyData";

		[OneTimeSetUp]
		public void Init () => TestRuntime.AssertXcodeVersion (11, 0);


		[SetUp]
		public void SetUp ()
		{
			record = NWTxtRecord.CreateDictionary ();
			record.Add (randomKey, new byte [3] { 0, 0, 0 });
		}

		[Test]
		public void TestFromBytes ()
		{
			// get the raw data from the dictionary create txt record, and recreate a new one
			var e = new AutoResetEvent (false);
			record.GetRawBytes (
				(d) => {
					Assert.AreNotEqual (0, d.Length, "Raw data length.");
					e.Set ();
				}
			);
			e.WaitOne ();
		}

		[TearDown]
		public void TearDown ()
		{
			record.Dispose ();
		}

		[Test]
		public void TestMissingKey () => Assert.AreEqual (NWTxtRecordFindKey.NotPresent, record.FindKey ("foo"));

		[Test]
		public void TestPresentKey () => Assert.AreEqual (NWTxtRecordFindKey.NonEmptyValue, record.FindKey (randomKey));

		[Test]
		public void TestAddByteValue ()
		{
			var data = new byte [] { 10, 20, 30, 40 };
			var mySecondKey = "secondKey";
			Assert.True (record.Add (mySecondKey, data), "Add");
			Assert.AreEqual (NWTxtRecordFindKey.NonEmptyValue, record.FindKey (mySecondKey));
		}

		[Test]
		public void TestAddNoValue ()
		{
			var mySecondKey = "secondLKey";
			Assert.True (record.Add (mySecondKey), "Add");
			Assert.AreEqual (NWTxtRecordFindKey.NoValue, record.FindKey (mySecondKey));
		}

		[Test]
		public void TestAddStringValue ()
		{
			var data = "hello";
			var mySecondKey = "secondLKey";
			Assert.True (record.Add (mySecondKey, data), "Add");
			Assert.AreEqual (NWTxtRecordFindKey.NonEmptyValue, record.FindKey (mySecondKey));
		}

		[Test]
		public void TestAddNullStringValue ()
		{
			string data = null;
			var mySecondKey = "secondLKey";
			Assert.True (record.Add (mySecondKey, data), "Add");
			Assert.AreEqual (NWTxtRecordFindKey.NoValue, record.FindKey (mySecondKey));
		}

		[Test]
		public void TestRemoveMissingKey () => Assert.IsFalse (record.Remove ("NotPresentKey"));

		[Test]
		public void TestRemovePresentKey ()
		{
			Assert.True (record.Remove (randomKey), "Remove");
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
			var keys = new List<string> { "first", "second", "third", randomKey };
			foreach (var key in keys) {
				record.Add (key, key);
			}
			// apply and ensure that we do get all the keys
#if !NET
			var keyCount = 0;
			record.Apply ((k, r, v) => {
				keyCount++;
				Assert.IsTrue (keys.Contains (k), k);
			});
#endif
			var keyCount2 = 0;
			record.Apply ((k, r, v) => {
				keyCount2++;
				Assert.IsTrue (keys.Contains (k), k);
				return true;
			});
#if !NET
			Assert.AreEqual (keys.Count, keyCount, "keycount");
#endif
			Assert.AreEqual (keys.Count, keyCount2, "keycount2");
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
			var e = new AutoResetEvent (false);
			record.GetRawBytes (
				(d) => {
					Assert.AreNotEqual (0, d.Length);
					e.Set ();
				}
			);
			e.WaitOne ();

		}
	}
}
#endif
