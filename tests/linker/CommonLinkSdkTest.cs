using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;

using NUnit.Framework;

using Foundation;

namespace LinkSdk {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class CommonLinkSdkTest {

		[Test]
		public void TypeDescriptor_A7793 ()
		{
			var c = TypeDescriptor.GetConverter (typeof (DateTimeOffset));
			Assert.That (c.GetType ().Name, Is.EqualTo ("DateTimeOffsetConverter"), "DateTimeOffsetConverter");

			c = TypeDescriptor.GetConverter (typeof (decimal));
			Assert.That (c.GetType ().Name, Is.EqualTo ("DecimalConverter"), "DecimalConverter");

			c = TypeDescriptor.GetConverter (typeof (string));
			Assert.That (c.GetType ().Name, Is.EqualTo ("StringConverter"), "StringConverter");

			c = TypeDescriptor.GetConverter (typeof (sbyte));
			Assert.That (c.GetType ().Name, Is.EqualTo ("SByteConverter"), "SByteConverter");

			c = TypeDescriptor.GetConverter (typeof (Collection<string>));
			Assert.That (c.GetType ().Name, Is.EqualTo ("CollectionConverter"), "CollectionConverter");

			c = TypeDescriptor.GetConverter (typeof (INSCoding));
			Assert.That (c.GetType ().Name, Is.EqualTo ("ReferenceConverter"), "ReferenceConverter");

			c = TypeDescriptor.GetConverter (typeof (Type));
			Assert.That (c.GetType ().Name, Is.EqualTo ("TypeConverter"), "TypeConverter");

			c = TypeDescriptor.GetConverter (typeof (ulong));
			Assert.That (c.GetType ().Name, Is.EqualTo ("UInt64Converter"), "UInt64Converter");

			c = TypeDescriptor.GetConverter (typeof (int []));
			Assert.That (c.GetType ().Name, Is.EqualTo ("ArrayConverter"), "ArrayConverter");

			c = TypeDescriptor.GetConverter (typeof (int?));
			Assert.That (c.GetType ().Name, Is.EqualTo ("NullableConverter"), "NullableConverter");

			c = TypeDescriptor.GetConverter (typeof (short));
			Assert.That (c.GetType ().Name, Is.EqualTo ("Int16Converter"), "Int16Converter");

			c = TypeDescriptor.GetConverter (typeof (CultureInfo));
			Assert.That (c.GetType ().Name, Is.EqualTo ("CultureInfoConverter"), "CultureInfoConverter");

			c = TypeDescriptor.GetConverter (typeof (float));
			Assert.That (c.GetType ().Name, Is.EqualTo ("SingleConverter"), "SingleConverter");

			c = TypeDescriptor.GetConverter (typeof (ushort));
			Assert.That (c.GetType ().Name, Is.EqualTo ("UInt16Converter"), "UInt16Converter");

			c = TypeDescriptor.GetConverter (typeof (Guid));
			Assert.That (c.GetType ().Name, Is.EqualTo ("GuidConverter"), "GuidConverter");

			c = TypeDescriptor.GetConverter (typeof (double));
			Assert.That (c.GetType ().Name, Is.EqualTo ("DoubleConverter"), "DoubleConverter");

			c = TypeDescriptor.GetConverter (typeof (int));
			Assert.That (c.GetType ().Name, Is.EqualTo ("Int32Converter"), "Int32Converter");

			c = TypeDescriptor.GetConverter (typeof (TimeSpan));
			Assert.That (c.GetType ().Name, Is.EqualTo ("TimeSpanConverter"), "TimeSpanConverter");

			c = TypeDescriptor.GetConverter (typeof (char));
			Assert.That (c.GetType ().Name, Is.EqualTo ("CharConverter"), "CharConverter");

			c = TypeDescriptor.GetConverter (typeof (long));
			Assert.That (c.GetType ().Name, Is.EqualTo ("Int64Converter"), "Int64Converter");

			c = TypeDescriptor.GetConverter (typeof (bool));
			Assert.That (c.GetType ().Name, Is.EqualTo ("BooleanConverter"), "BooleanConverter");

			c = TypeDescriptor.GetConverter (typeof (long));
			Assert.That (c.GetType ().Name, Is.EqualTo ("Int64Converter"), "Int64Converter");

			c = TypeDescriptor.GetConverter (typeof (uint));
			Assert.That (c.GetType ().Name, Is.EqualTo ("UInt32Converter"), "UInt32Converter");

			c = TypeDescriptor.GetConverter (typeof (FileShare));
			Assert.That (c.GetType ().Name, Is.EqualTo ("EnumConverter"), "EnumConverter");

			// special case - it's not in the default list we reflect in dont link tests

			c = TypeDescriptor.GetConverter (typeof (IComponent));
			Assert.That (c.GetType ().Name, Is.EqualTo ("ComponentConverter"), "ComponentConverter");
		}
	}
}
