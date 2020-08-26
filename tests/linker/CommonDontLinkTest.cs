using System;
using System.Collections;
using System.Reflection;

using NUnit.Framework;

using Foundation;

namespace DontLink {

	[TestFixture]
	public class CommonDontLinkTest {

		[Test]
		public void TypeDescriptorCanary ()
		{
			// this will fail is ReflectTypeDescriptionProvider.cs is modified
			var rtdp = typeof (System.ComponentModel.BooleanConverter).Assembly.GetType ("System.ComponentModel.ReflectTypeDescriptionProvider");
			Assert.NotNull (rtdp, "type");
			var p = rtdp.GetProperty ("IntrinsicTypeConverters", BindingFlags.Static | BindingFlags.NonPublic);
			Assert.NotNull (p, "property");
			var ht = (Hashtable) p.GetGetMethod (true).Invoke (null, null);
			Assert.NotNull (ht, "Hashtable");

#if NET
			var expectedCount = 28;
#else
			var expectedCount = 26;
#endif
			Assert.That (ht.Count, Is.EqualTo (expectedCount), "Count");

			foreach (var item in ht.Values) {
				var name = item.ToString ();
				switch (name) {
				case "System.ComponentModel.DateTimeOffsetConverter":
				case "System.ComponentModel.DecimalConverter":
				case "System.ComponentModel.StringConverter":
				case "System.ComponentModel.SByteConverter":
				case "System.ComponentModel.CollectionConverter":
				case "System.ComponentModel.ReferenceConverter":
				case "System.ComponentModel.TypeConverter":
				case "System.ComponentModel.DateTimeConverter":
				case "System.ComponentModel.UInt64Converter":
				case "System.ComponentModel.ArrayConverter":
				case "System.ComponentModel.NullableConverter":
				case "System.ComponentModel.Int16Converter":
				case "System.ComponentModel.CultureInfoConverter":
				case "System.ComponentModel.SingleConverter":
				case "System.ComponentModel.UInt16Converter":
				case "System.ComponentModel.GuidConverter":
				case "System.ComponentModel.DoubleConverter":
				case "System.ComponentModel.Int32Converter":
				case "System.ComponentModel.TimeSpanConverter":
				case "System.ComponentModel.CharConverter":
				case "System.ComponentModel.Int64Converter":
				case "System.ComponentModel.BooleanConverter":
				case "System.ComponentModel.UInt32Converter":
				case "System.ComponentModel.ByteConverter":
				case "System.ComponentModel.EnumConverter":
#if NET
				case "System.ComponentModel.VersionConverter":
				case "System.UriTypeConverter":
#endif
					break;
				default:
					Assert.Fail ($"Unknown type descriptor {name}");
					break;
				}
			}
		}
	}
}
