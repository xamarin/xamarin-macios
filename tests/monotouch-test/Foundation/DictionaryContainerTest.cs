//
// Unit tests for DictionaryContainer
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2014 Xamarin Inc. All rights reserved.
//

using System;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Foundation {

	public class DictionaryContainerPoker : DictionaryContainer {

		public DictionaryContainerPoker ()
		{
		}


		public void SetArrayValue_ (NSString key, NSNumber [] values)
		{
			SetArrayValue (key, values);
		}

		public void SetArrayValue_ (NSString key, string [] values)
		{
			SetArrayValue (key, values);
		}

		public void SetArrayValue_<T> (NSString key, T [] values)
		{
			SetArrayValue<T> (key, values);
		}

		public void SetArrayValue_ (NSString key, INativeObject [] values)
		{
			SetArrayValue (key, values);
		}

		public void SetBooleanValue_ (NSString key, bool? value)
		{
			SetBooleanValue (key, value);
		}

		public void SetNumberValue_ (NSString key, int? value)
		{
			SetNumberValue (key, value);
		}

		public void SetNumberValue_ (NSString key, uint? value)
		{
			SetNumberValue (key, value);
		}

		public void SetNumberValue_ (NSString key, nint? value)
		{
			SetNumberValue (key, value);
		}

		public void SetNumberValue_ (NSString key, nuint? value)
		{
			SetNumberValue (key, value);
		}

		public void SetStringValue_ (NSString key, string value)
		{
			SetStringValue (key, value);
		}
	}

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DictionaryContainerTest {

		[Test]
		public void Empty ()
		{
			var dc = new DictionaryContainerPoker ();
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (0), "Count");
		}

		NSString key = new NSString ("key");

		[Test]
		public void SetArrayValue_NSNumber ()
		{
			var numbers = new NSNumber [] { (NSNumber) 0, (NSNumber) 1 };
			var dc = new DictionaryContainerPoker ();

			Assert.Throws<ArgumentNullException> (delegate
			{
				dc.SetArrayValue_ (null, numbers);
			}, "null key");

			dc.SetArrayValue_ (key, numbers);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (1), "1");
			Assert.That ((int) (dc.Dictionary [key] as NSArray).Count, Is.EqualTo (2), "2");

			numbers = null;
			dc.SetArrayValue_ (key, numbers);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (0), "0");
		}

		[Test]
		public void SetArrayValue_String ()
		{
			var strings = new String [] { "xamarin", "monkeys" };
			var dc = new DictionaryContainerPoker ();

			Assert.Throws<ArgumentNullException> (delegate
			{
				dc.SetArrayValue_ (null, strings);
			}, "null key");

			dc.SetArrayValue_ (key, strings);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (1), "1");
			Assert.That ((int) (dc.Dictionary [key] as NSArray).Count, Is.EqualTo (2), "2");

			strings = null;
			dc.SetArrayValue_ (key, strings);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (0), "0");
		}

		[Test]
		public void SetArrayValue_T_Enum ()
		{
			var enums = new NSStringEncoding [] { NSStringEncoding.ISOLatin1, NSStringEncoding.ISOLatin2 };
			var dc = new DictionaryContainerPoker ();

			Assert.Throws<ArgumentNullException> (delegate
			{
				dc.SetArrayValue_ (null, enums);
			}, "null key");

			dc.SetArrayValue_ (key, enums);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (1), "1");
			Assert.That ((int) (dc.Dictionary [key] as NSArray).Count, Is.EqualTo (2), "2");

			enums = null;
			dc.SetArrayValue_ (key, enums);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (0), "0");
		}

		[Test]
		public void SetArrayValue_INativeObject ()
		{
			var native = new INativeObject [] { new CFString ("xamarin"), CFRunLoop.Main };
			var dc = new DictionaryContainerPoker ();

			Assert.Throws<ArgumentNullException> (delegate
			{
				dc.SetArrayValue_ (null, native);
			}, "null key");

			dc.SetArrayValue_ (key, native);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (1), "1");
			Assert.That ((int) (dc.Dictionary [key] as NSArray).Count, Is.EqualTo (2), "2");

			native = null;
			dc.SetArrayValue_ (key, native);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (0), "0");
		}

		[Test]
		public void SetBooleanValue ()
		{
			var dc = new DictionaryContainerPoker ();

			Assert.Throws<ArgumentNullException> (delegate
			{
				dc.SetBooleanValue_ (null, true);
			}, "null key");

			dc.SetBooleanValue_ (key, true);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (1), "1");

			dc.SetBooleanValue_ (key, null);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (0), "0");
		}

		[Test]
		public void SetNumberValue_Int32 ()
		{
			var dc = new DictionaryContainerPoker ();

			Assert.Throws<ArgumentNullException> (delegate
			{
				dc.SetNumberValue_ (null, int.MinValue);
			}, "null key");

			dc.SetNumberValue_ (key, int.MinValue);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (1), "1");

			dc.SetNumberValue_ (key, (int?) null);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (0), "0");
		}

		[Test]
		public void SetNumberValue_UInt32 ()
		{
			var dc = new DictionaryContainerPoker ();

			Assert.Throws<ArgumentNullException> (delegate
			{
				dc.SetNumberValue_ (null, uint.MaxValue);
			}, "null key");

			dc.SetNumberValue_ (key, uint.MaxValue);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (1), "1");

			dc.SetNumberValue_ (key, (uint?) null);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (0), "0");
		}

		[Test]
		public void SetStringValue ()
		{
			var dc = new DictionaryContainerPoker ();

			Assert.Throws<ArgumentNullException> (delegate
			{
				dc.SetStringValue_ (null, String.Empty);
			}, "null key");

			dc.SetStringValue_ (key, (NSString) "monkey");
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (1), "1");

			dc.SetStringValue_ (key, null);
			Assert.That ((int) dc.Dictionary.Count, Is.EqualTo (0), "0");
		}
	}
}
