using System;
using Foundation;
using NUnit.Framework;
using ObjCRuntime;

namespace MonoTouchFixtures.Foundation
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSKeyedUnarchiverTest
	{
		[Test]
		public void GetUnarchivedObject_TypeWrappers ()
		{
			if (!TestRuntime.CheckXcodeVersion (10, 0))
				Assert.Ignore ("Ignoring GetUnarchivedObject_TypeWrappers as requires new APIs");

			NSDictionary<NSString, NSString> testValues = new NSDictionary<NSString, NSString> ((NSString)"1", (NSString)"a");
			NSData data = NSKeyedArchiver.ArchivedDataWithRootObject (testValues, true, out NSError error);
			Assert.IsNull (error);

			Type dictionaryType = typeof (NSDictionary<NSString, NSString>);
			Class dictionaryClass = new Class (dictionaryType);
			NSObject o = NSKeyedUnarchiver.GetUnarchivedObject (dictionaryClass, data, out error);
			Assert.IsNotNull (o);
			Assert.IsNull (error);

			o = NSKeyedUnarchiver.GetUnarchivedObject (new NSSet<Class> (new Class [] { dictionaryClass }), data, out error);
			Assert.IsNotNull (o);
			Assert.IsNull (error);

			o = NSKeyedUnarchiver.GetUnarchivedObject (dictionaryType, data, out error);
			Assert.IsNotNull (o);
			Assert.IsNull (error);

			o = NSKeyedUnarchiver.GetUnarchivedObject (new Type [] { dictionaryType }, data, out error);
			Assert.IsNotNull (o);
			Assert.IsNull (error);
		}

		[Test]
		public void DataTransformer_AllowedTopLevelTypes_WrapperTests ()
		{
			if (!TestRuntime.CheckXcodeVersion (10, 0))
				Assert.Ignore ("Ignoring DataTransformer_AllowedTopLevelTypes_WrapperTests as requires new APIs");

			Class [] classes = NSSecureUnarchiveFromDataTransformer.AllowedTopLevelClasses;
			Type [] types =  NSSecureUnarchiveFromDataTransformer.AllowedTopLevelTypes;

			Assert.AreEqual (classes.Length, types.Length);
		}
	}
}
