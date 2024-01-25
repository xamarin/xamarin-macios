using System;
using System.IO;

using NUnit.Framework;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class PropertyListEditorTaskTests : TestBase {
		static void CheckArray (PArray array, PArray expected)
		{
			Assert.AreEqual (expected.Count, array.Count, "Unexpected number of array elements");

			for (int i = 0; i < expected.Count; i++) {
				Assert.AreEqual (expected [i].Type, array [i].Type, "Type-mismatch for array element {0}", i);
				CheckValue (array [i], expected [i]);
			}
		}

		static void CheckDictionary (PDictionary dict, PDictionary expected)
		{
			foreach (var kvp in expected) {
				PObject value;

				Assert.IsTrue (dict.TryGetValue (kvp.Key, out value), "Expected key '{0}'", kvp.Key);
				Assert.AreEqual (kvp.Value.Type, value.Type, "Type-mismatch for '{0}'", kvp.Key);

				CheckValue (value, kvp.Value);
			}
		}

		static void CheckValue (PObject value, PObject expected)
		{
			switch (value.Type) {
			case PObjectType.Dictionary:
				CheckDictionary ((PDictionary) value, (PDictionary) expected);
				break;
			case PObjectType.Array:
				CheckArray ((PArray) value, (PArray) expected);
				break;
			case PObjectType.Real:
				Assert.AreEqual (((PReal) expected).Value, ((PReal) value).Value);
				break;
			case PObjectType.Number:
				Assert.AreEqual (((PNumber) expected).Value, ((PNumber) value).Value);
				break;
			case PObjectType.Boolean:
				Assert.AreEqual (((PBoolean) expected).Value, ((PBoolean) value).Value);
				break;
			case PObjectType.Data:
				// TODO: implement this
				break;
			case PObjectType.String:
				Assert.AreEqual (((PString) expected).Value, ((PString) value).Value);
				break;
			case PObjectType.Date:
				Assert.AreEqual (((PDate) expected).Value, ((PDate) value).Value);
				break;
			}
		}

		void TestExecuteTask (PDictionary input, PropertyListEditorAction action, string entry, string type, string value, PObject expected)
		{
			var task = CreateTask<PropertyListEditor> ();
			task.PropertyList = Path.Combine (Cache.CreateTemporaryDirectory (), "propertyList.plist");
			task.Action = action.ToString ();
			task.Entry = entry;
			task.Type = type;
			task.Value = value;
			input.Save (task.PropertyList);

			if (expected is null) {
				Assert.IsFalse (task.Execute (), "Task was expected to fail.");
				return;
			}

			Assert.IsTrue (task.Execute (), "Task was expected to execute successfully.");

			var output = PObject.FromFile (task.PropertyList);

			Assert.AreEqual (expected.Type, output.Type, "Task produced the incorrect plist output.");

			CheckValue (output, expected);
		}

		[Test]
		public void TestAddNewProperty ()
		{
			const string property = "CFBundleIdentifier";
			const string value = "com.microsoft.add-property";
			var expected = new PDictionary ();
			var plist = new PDictionary ();

			expected.Add (property, value);

			TestExecuteTask (plist, PropertyListEditorAction.Add, ":" + property, "string", value, expected);
		}

		[Test]
		public void TestAddExistingProperty ()
		{
			const string property = "CFBundleIdentifier";
			const string value = "com.microsoft.add-property";
			var plist = new PDictionary ();

			plist.Add (property, value);

			// Note: Add will fail if the property already exists
			TestExecuteTask (plist, PropertyListEditorAction.Add, ":" + property, "string", value, null);
		}

		[Test]
		public void TestAddNestedProperty ()
		{
			var plist = new PDictionary ();
			plist.Add ("CFBundleIdentifier", "com.microsoft.add-nested-property");

			var expected = (PDictionary) plist.Clone ();
			var primary = new PDictionary ();
			var icons = new PDictionary ();

			primary.Add ("UIPrerenderedIcon", new PBoolean (true));
			icons.Add ("CFBundlePrimaryIcon", primary);
			expected.Add ("CFBundleIcons", icons);

			TestExecuteTask (plist, PropertyListEditorAction.Add, ":CFBundleIcons:CFBundlePrimaryIcon:UIPrerenderedIcon", "bool", "true", expected);

			plist = (PDictionary) expected.Clone ();
			var files = new PArray ();

			primary.Add ("CFBundleIconFiles", files);

			TestExecuteTask (plist, PropertyListEditorAction.Add, ":CFBundleIcons:CFBundlePrimaryIcon:CFBundleIconFiles", "array", null, expected);

			plist = (PDictionary) expected.Clone ();
			files.Add ("icon0");

			TestExecuteTask (plist, PropertyListEditorAction.Add, ":CFBundleIcons:CFBundlePrimaryIcon:CFBundleIconFiles:", "string", "icon0", expected);
		}

		[Test]
		public void TestAddArrayValue ()
		{
			var plist = new PDictionary ();
			var primary = new PDictionary ();
			var icons = new PDictionary ();
			var files = new PArray ();

			plist.Add ("CFBundleIdentifier", "com.microsoft.add-array-value");
			plist.Add ("CFBundleIcons", icons);
			icons.Add ("CFBundlePrimaryIcon", primary);
			primary.Add ("CFBundleIconFiles", files);
			files.Add ("icon0");
			files.Add ("icon1");
			files.Add ("icon2");

			var expected = (PDictionary) plist.Clone ();
			files.RemoveAt (0);

			TestExecuteTask (plist, PropertyListEditorAction.Add, ":CFBundleIcons:CFBundlePrimaryIcon:CFBundleIconFiles:0", "string", "icon0", expected);
		}

		[Test]
		public void TestSetNewProperty ()
		{
			const string property = "CFBundleIdentifier";
			const string value = "com.microsoft.set-property";
			var plist = new PDictionary ();

			// Note: Set will fail if the property doesn't already exist
			TestExecuteTask (plist, PropertyListEditorAction.Set, ":" + property, "string", value, null);
		}

		[Test]
		public void TestSetExistingProperty ()
		{
			const string property = "CFBundleIdentifier";
			const string value = "com.microsoft.set-property";
			var expected = new PDictionary ();
			var plist = new PDictionary ();

			plist.Add (property, "com.microsoft.initial-value");
			expected.Add (property, value);

			TestExecuteTask (plist, PropertyListEditorAction.Set, ":" + property, "string", value, expected);
		}

		[Test]
		public void TestSetNestedProperty ()
		{
			var plist = new PDictionary ();
			plist.Add ("CFBundleIdentifier", "com.microsoft.set-nested-property");

			// Note: Set will fail if the property doesn't already exist
			TestExecuteTask (plist, PropertyListEditorAction.Set, ":CFBundleIcons:CFBundlePrimaryIcon:UIPrerenderedIcon", "bool", "true", null);
		}

		[Test]
		public void TestSetArrayValue ()
		{
			var plist = new PDictionary ();
			var primary = new PDictionary ();
			var icons = new PDictionary ();
			var files = new PArray ();

			plist.Add ("CFBundleIdentifier", "com.microsoft.set-array-value");
			plist.Add ("CFBundleIcons", icons);
			icons.Add ("CFBundlePrimaryIcon", primary);
			primary.Add ("CFBundleIconFiles", files);
			files.Add ("icon0");
			files.Add ("icon1");
			files.Add ("icon2");

			var expected = (PDictionary) plist.Clone ();
			files [0] = new PString ("icon");

			TestExecuteTask (plist, PropertyListEditorAction.Set, ":CFBundleIcons:CFBundlePrimaryIcon:CFBundleIconFiles:0", "string", "icon0", expected);

			// Note: this will fail due to the index being out of range
			TestExecuteTask (plist, PropertyListEditorAction.Set, ":CFBundleIcons:CFBundlePrimaryIcon:CFBundleIconFiles:3", "string", "icon3", null);
		}

		[Test]
		public void TestClear ()
		{
			var plist = new PDictionary ();

			plist.Add ("CFBundleIdentifier", "com.microsoft.clear");
			plist.Add ("CFBundleShortVersionString", "1.0");
			plist.Add ("CFBundleVersion", "1");

			TestExecuteTask (plist, PropertyListEditorAction.Clear, null, null, null, new PDictionary ());
			TestExecuteTask (plist, PropertyListEditorAction.Clear, null, "dict", null, new PDictionary ());
			TestExecuteTask (plist, PropertyListEditorAction.Clear, null, "array", null, new PArray ());
			TestExecuteTask (plist, PropertyListEditorAction.Clear, null, "bool", null, new PBoolean (false));
			TestExecuteTask (plist, PropertyListEditorAction.Clear, null, "integer", null, new PNumber (0));
			TestExecuteTask (plist, PropertyListEditorAction.Clear, null, "real", null, new PReal (0));
			TestExecuteTask (plist, PropertyListEditorAction.Clear, null, "string", null, new PString (string.Empty));
			TestExecuteTask (plist, PropertyListEditorAction.Clear, null, "data", null, new PData (new byte [0]));
		}

		[Test]
		public void TestDeleteProperty ()
		{
			var plist = new PDictionary ();

			plist.Add ("CFBundleIdentifier", "com.microsoft.delete-property");
			plist.Add ("CFBundleShortVersionString", "1.0");

			var expected = (PDictionary) plist.Clone ();

			plist.Add ("CFBundleVersion", "1");

			TestExecuteTask (plist, PropertyListEditorAction.Delete, ":CFBundleVersion", null, null, expected);

			TestExecuteTask (plist, PropertyListEditorAction.Delete, ":CFDoesNotExist", null, null, null);
		}

		[Test]
		public void TestDeleteNestedProperty ()
		{
			var plist = new PDictionary ();
			var primary = new PDictionary ();
			var icons = new PDictionary ();
			var files = new PArray ();

			plist.Add ("CFBundleIdentifier", "com.microsoft.delete-nested-property");
			plist.Add ("CFBundleIcons", icons);
			icons.Add ("CFBundlePrimaryIcon", primary);
			primary.Add ("CFBundleIconFiles", files);
			files.Add ("icon0");
			files.Add ("icon1");

			var expected = (PDictionary) plist.Clone ();

			files.Add ("icon2");

			TestExecuteTask (plist, PropertyListEditorAction.Delete, ":CFBundleIcons:CFBundlePrimaryIcon:CFBundleIconFiles:2", null, null, expected);

			var plist2 = (PDictionary) expected.Clone ();

			files.Remove ();

			var expected2 = (PDictionary) plist.Clone ();

			TestExecuteTask (plist2, PropertyListEditorAction.Delete, ":CFBundleIcons:CFBundlePrimaryIcon:CFBundleIconFiles", null, null, expected2);
		}

		[Test]
		public void TestMergeRoot ()
		{
			var expected = new PDictionary ();
			var primary = new PDictionary ();
			var icons = new PDictionary ();
			var files = new PArray ();

			expected.Add ("CFBundleIdentifier", "com.microsoft.merge-root");
			expected.Add ("CFBundleIcons", icons);
			icons.Add ("CFBundlePrimaryIcon", primary);
			primary.Add ("UIPrerenderedIcon", new PBoolean (true));
			primary.Add ("CFBundleIconFiles", files);
			files.Add ("icon0");
			files.Add ("icon1");
			files.Add ("icon2");

			var plist = (PDictionary) expected.Clone ();
			plist.Remove ("CFBundleIcons");

			var merge = (PDictionary) expected.Clone ();
			merge.Remove ("CFBundleIdentifier");

			var tmp = Path.Combine (Cache.CreateTemporaryDirectory (), "tmpfile");

			merge.Save (tmp);

			TestExecuteTask (plist, PropertyListEditorAction.Merge, null, null, tmp, expected);
		}

		[Test]
		public void TestMergeArrays ()
		{
			var plist = new PDictionary ();
			var array0 = new PArray ();

			array0.Add ("item0");
			array0.Add ("item1");
			array0.Add ("item2");
			array0.Add ("item3");

			plist.Add ("CFBundleIdentifier", "com.microsoft.merge-arrays");
			plist.Add ("CFArrayItems", array0);

			var array1 = new PArray ();
			array1.Add ("item2");
			array1.Add ("item3");

			var expected = (PDictionary) plist.Clone ();
			array0.RemoveAt (3);
			array0.RemoveAt (2);

			var tmp = Path.Combine (Cache.CreateTemporaryDirectory (), "tmpfile");

			array1.Save (tmp);

			TestExecuteTask (plist, PropertyListEditorAction.Merge, ":CFArrayItems", null, tmp, expected);
		}
	}
}
