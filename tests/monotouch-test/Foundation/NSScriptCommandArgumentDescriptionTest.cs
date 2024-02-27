#if __MACOS__
// Copyright 2015 Xamarin, Inc.

using System;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSScriptCommandArgumentDescriptionKeysTest {
		[Test]
		public void TestAppleEventCodeKey ()
		{
			Assert.IsNotNull (NSScriptCommandArgumentDescriptionKeys.AppleEventCodeKey);
			Assert.AreEqual ("AppleEventCode", NSScriptCommandArgumentDescriptionKeys.AppleEventCodeKey?.ToString ());
		}

		[Test]
		public void TestTypeKey ()
		{
			Assert.AreEqual ("Type", NSScriptCommandArgumentDescriptionKeys.TypeKey.ToString ());
		}

		[Test]
		public void TestOptionalKey ()
		{
			Assert.AreEqual ("Optional", NSScriptCommandArgumentDescriptionKeys.OptionalKey.ToString ());
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSScriptCommandArgumentDescriptionTest {

		[TestCase ("")]
		[TestCase (null)]
		public void TestConstructorNameNullOrEmpty (string name)
		{
			Assert.Throws<ArgumentException> (() => new NSScriptCommandArgumentDescription (name, "eeee", "NSString", false));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestConstructorEventCodeNullOrEmpty (string eventCode)
		{
			Assert.Throws<ArgumentException> (() => new NSScriptCommandArgumentDescription ("name", eventCode, "NSString", false));
		}

		[TestCase ("srf")]
		[TestCase ("TooLong")]
		public void TestConstructorEventCodeWrongLength (string eventCode)
		{
			Assert.Throws<ArgumentException> (() => new NSScriptCommandArgumentDescription ("name", eventCode, "NSString", false));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestConstructorTypeNullOrEmpty (string type)
		{
			Assert.Throws<ArgumentException> (() => new NSScriptCommandArgumentDescription ("name", "****", type, false));
		}

		[TestCase ("name", "cdfd", "NSString", true)]
		[TestCase ("name", "cdfd", "NSNumber", false)]
		[TestCase ("name", "****", "NSNumber", true)]
		[TestCase ("otherName", "****", "NSNumber", false)]
		public void TestDescription (string name, string code, string type, bool isOptional)
		{
			var arg = new NSScriptCommandArgumentDescription (name, code, type, isOptional);
			var description = arg.Dictionary;

			Assert.AreEqual (code, description [new NSString ("AppleEventCode")].ToString ());
			Assert.AreEqual (type, description [new NSString ("Type")].ToString ());
			Assert.AreEqual (isOptional ? "Yes" : "No", description [new NSString ("Optional")].ToString ());
		}
	}
}
#endif // __MACOS__
