#if __MACOS__
// Copyright 2015 Xamarin, Inc.

using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSScriptCommandDescriptionTest {

		NSScriptCommandDescription scriptDescription = null;
		Dictionary<string, NSScriptCommandArgumentDescription> args;
		NSScriptCommandDescriptionDictionary dict = null;
		string suiteName, commandName, cmdClass, eventCode, eventClass, returnType, resultAppleEvent = null;

		[SetUp]
		public void Init ()
		{
			args = new Dictionary<string, NSScriptCommandArgumentDescription> {
				{"firstArg", new NSScriptCommandArgumentDescription {Name="firstArg", AppleEventCode="fArg", Type="integer", IsOptional=true}},
				{"secondArg", new NSScriptCommandArgumentDescription {Name="secondArg", AppleEventCode="sArg", Type="NSNumber"}},
				{"thirdArg", new NSScriptCommandArgumentDescription {Name="thirdArg", AppleEventCode="tArg", Type="integer"}}
			};

			suiteName = "Chromium Suite";
			commandName = "Exec Python";
			cmdClass = "NSScriptCommand";
			eventCode = "ExPy";
			eventClass = "CrSu";
			returnType = "NSString";
			resultAppleEvent = "text";
			dict = new NSScriptCommandDescriptionDictionary {
				CommandClass = cmdClass,
				AppleEventCode = eventCode,
				AppleEventClassCode = eventClass,
				Type = returnType,
				ResultAppleEventCode = resultAppleEvent
			};

			foreach (var arg in args.Values) {
				dict.Add (arg);
			}
			scriptDescription = NSScriptCommandDescription.Create (suiteName, commandName, dict);
		}

		[TearDown]
		public void Dispose ()
		{
			if (scriptDescription is not null)
				scriptDescription.Dispose ();
		}

		[Test]
		public void TestCreateWithDictWrongArgDescription ()
		{
			var description = new NSScriptCommandDescriptionDictionary ();
			Assert.Throws<ArgumentException> (() => NSScriptCommandDescription.Create (suiteName, commandName, description));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestCreateWithDictNullOrEmptySuitName (string code)
		{
			var description = new NSScriptCommandDescriptionDictionary ();
			Assert.Throws<ArgumentException> (() => NSScriptCommandDescription.Create (code, commandName, description));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestCreateWithDictNullOrEmptyCommandName (string code)
		{
			var description = new NSScriptCommandDescriptionDictionary ();
			Assert.Throws<ArgumentException> (() => NSScriptCommandDescription.Create (suiteName, code, description));
		}

		[Test]
		public void TestCreateWithDictNullDict ()
		{
			NSScriptCommandDescriptionDictionary dict = null;
			Assert.Throws<ArgumentNullException> (() => NSScriptCommandDescription.Create (suiteName, commandName, dict));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestCreateSuiteNameNullOrEmpty (string code)
		{
			Assert.Throws<ArgumentException> (() => NSScriptCommandDescription.Create (code, commandName, dict));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestCreateCommandNameNullOrEmpty (string code)
		{
			Assert.Throws<ArgumentException> (() => NSScriptCommandDescription.Create (suiteName, code, dict));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestCreateCmdClassNullOrEmpty (string code)
		{
			dict.CommandClass = code;
			Assert.Throws<ArgumentException> (() => NSScriptCommandDescription.Create (suiteName, commandName, dict));
		}

		[TestCase ("")]
		[TestCase (null)]
		public void TestCreateEventCodeNullOrEmpty (string code)
		{
			dict.AppleEventCode = code;
			Assert.Throws<ArgumentException> (() => NSScriptCommandDescription.Create (suiteName, commandName, dict));
		}

		[TestCase ("TooLong")]
		[TestCase ("srt")]
		public void TestCreateEventCodeWrongLength (string code)
		{
			dict.AppleEventCode = code;
			Assert.Throws<ArgumentException> (() => NSScriptCommandDescription.Create (suiteName, commandName, dict));
		}

		[TestCase ("TooLong")]
		[TestCase ("srt")]
		public void TestCreateResultAppleEventWrongLength (string code)
		{
			dict.ResultAppleEventCode = code;
			Assert.Throws<ArgumentException> (() => NSScriptCommandDescription.Create (suiteName, commandName, dict));
		}

		[Test]
		public void TestClassName ()
		{
			Assert.AreEqual (cmdClass, scriptDescription.ClassName);
		}

		[Test]
		public void TestName ()
		{
			Assert.AreEqual (commandName, scriptDescription.Name);
		}

		[Test]
		public void TestSuiteName ()
		{
			Assert.AreEqual (suiteName, scriptDescription.SuitName);
		}

		[Test]
		public void TestArgumentsNames ()
		{
			Assert.AreEqual (args.Keys.Count, scriptDescription.ArgumentNames.Length);
			foreach (var argName in scriptDescription.ArgumentNames) {
				Assert.IsTrue (args.Keys.Contains (argName), "Arg {0} is missing", argName);
			}
		}

		[Test]
		public void TestAppleEventClassCode ()
		{
			Assert.AreEqual (eventClass, scriptDescription.AppleEventClassCode);
		}

		[Test]
		public void TestAppleEventCode ()
		{
			Assert.AreEqual (eventCode, scriptDescription.AppleEventCode);
		}

		[Test]
		public void TestIsOptionalArgument ()
		{
			foreach (KeyValuePair<string, NSScriptCommandArgumentDescription> kvp in args) {
				Assert.AreEqual (kvp.Value.IsOptional, scriptDescription.IsOptionalArgument (kvp.Key),
					"Wrong apple event code for arg {0}", kvp.Key);
			}
		}

		[Test]
		public void TestGetAppleEventCodeForArgument ()
		{
			foreach (KeyValuePair<string, NSScriptCommandArgumentDescription> kvp in args) {
				Assert.AreEqual (kvp.Value.AppleEventCode, scriptDescription.GetAppleEventCodeForArgument (kvp.Key),
					"Wrong apple event code for arg {0}", kvp.Key);
			}
		}

		[Test]
		public void TestReturnType ()
		{
			Assert.AreEqual (returnType, scriptDescription.ReturnType);
		}
	}
}
#endif // __MACOS__
