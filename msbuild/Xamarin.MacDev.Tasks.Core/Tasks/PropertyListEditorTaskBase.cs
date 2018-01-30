using System;
using System.IO;
using System.Text;
using System.Globalization;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public enum PropertyListEditorAction {
		Add,
		Clear,
		Delete,
		Import,
		Merge,
		Set
	}

	public abstract class PropertyListEditorTaskBase : Task
	{
		[Required]
		public string PropertyList { get; set; }

		[Required]
		public string Action { get; set; }

		public string Entry { get; set; }

		public string Type { get; set; }

		public string Value;

		static string GetType (PObject value)
		{
			switch (value.Type) {
			case PObjectType.Dictionary: return "dict";
			case PObjectType.Array:      return "array";
			case PObjectType.Real:       return "real";
			case PObjectType.Number:     return "integer";
			case PObjectType.Boolean:    return "bool";
			case PObjectType.Data:       return "data";
			case PObjectType.String:     return "string";
			case PObjectType.Date:       return "date";
			default: return null;
			}
		}

		bool CreateValue (string type, string text, out PObject value)
		{
			DateTime date = DateTime.Now;
			bool boolean = false;
			double real = 0;
			int integer = 0;

			value = null;

			switch (type.ToLowerInvariant ()) {
			case "string":
				value = new PString (text);
				return true;
			case "array":
				value = new PArray ();
				return true;
			case "dict":
				value = new PDictionary ();
				return true;
			case "bool":
				if (text == null || !bool.TryParse (text, out boolean))
					boolean = false;

				value = new PBoolean (boolean);
				return true;
			case "real":
				if (text != null && !double.TryParse (text, NumberStyles.Float, CultureInfo.InvariantCulture, out real)) {
					Log.LogError (7045, null, "Unrecognized Real Format");
					return false;
				}

				value = new PReal (real);
				return true;
			case "integer":
				if (text != null && !int.TryParse (text, NumberStyles.Integer, CultureInfo.InvariantCulture, out integer)) {
					Log.LogError (7045, null, "Unrecognized Integer Format");
					return false;
				}

				value = new PNumber (integer);
				return true;
			case "date":
				if (text != null && !DateTime.TryParse (text, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) {
					Log.LogError (7045, null, "Unrecognized Date Format");
					return false;
				}

				value = new PDate (date);
				return true;
			case "data":
				if (text != null)
					value = new PData (Encoding.UTF8.GetBytes (Value));
				else
					value = new PData (new byte[0]);
				return true;
			default:
				Log.LogError (7045, null, $"Unrecognized Type: {type}");
				value = null;
				return false;
			}
		}

		string[] GetPropertyPath ()
		{
			var path = Entry;

			if (string.IsNullOrEmpty (path))
				return new string[0];

			if (path[0] == ':')
				path = path.Substring (1);

			return path.Split (':');
		}

		bool Add (PObject plist)
		{
			var path = GetPropertyPath ();
			var current = plist;
			PDictionary dict;
			PObject value;
			PArray array;
			int index;
			int i = 0;

			if (path.Length == 0) {
				Log.LogError (7046, null, $"Add: Entry, \"{Entry}\", Incorrectly Specified");
				return false;
			}

			while (i < path.Length - 1) {
				dict = current as PDictionary;
				array = current as PArray;

				if (array != null) {
					if (!int.TryParse (path[i], out index) || index < 0 || index >= array.Count) {
						Log.LogError (7047, null, $"Add: Entry, \"{Entry}\", Contains Invalid Array Index");
						return false;
					}

					current = array[index];
				} else if (dict != null) {
					if (!dict.TryGetValue (path[i], out current))
						dict[path[i]] = current = new PDictionary ();
				} else {
					Log.LogError (7046, null, $"Add: Entry, \"{Entry}\", Incorrectly Specified");
					return false;
				}

				i++;
			}

			dict = current as PDictionary;
			array = current as PArray;

			if (array != null) {
				if (path[i].Length == 0) {
					index = array.Count;
				} else if (!int.TryParse (path[i], out index) || index < 0) {
					Log.LogError (7047, null, $"Add: Entry, \"{Entry}\", Contains Invalid Array Index");
					return false;
				}

				if (!CreateValue (Type ?? string.Empty, Value, out value))
					return false;

				if (index < array.Count)
					array.Insert (index, value);
				else
					array.Add (value);
			} else if (dict != null) {
				if (dict.ContainsKey (path[i])) {
					Log.LogError (7048, null, $"Add: \"{Entry}\" Entry Already Exists");
					return false;
				}

				if (!CreateValue (Type ?? string.Empty, Value, out value))
					return false;

				dict[path[i]] = value;
			} else {
				Log.LogError (7049, null, $"Add: Can't Add Entry, \"{Entry}\", to Parent");
				return false;
			}

			return true;
		}

		bool Clear (ref PObject plist)
		{
			if (Type != null) {
				switch (Type.ToLowerInvariant ()) {
				case "string": plist = new PString (string.Empty); break;
				case "array": plist = new PArray (); break;
				case "dict": plist = new PDictionary (); break;
				case "bool": plist = new PBoolean (false); break;
				case "real": plist = new PReal (0); break;
				case "integer": plist = new PNumber (0); break;
				case "date": plist = new PDate (DateTime.Now); break;
				case "data": plist = new PData (new byte[1]); break;
				default:
					Log.LogError (7045, null, $"Unrecognized Type: {Type}");
					return false;
				}
			} else {
				plist = PObject.Create (plist.Type);
			}

			return true;
		}

		bool Delete (PObject plist)
		{
			var path = GetPropertyPath ();
			var current = plist;
			PDictionary dict;
			PArray array;
			int index;
			int i = 0;

			if (path.Length == 0) {
				Log.LogError (7050, null, $"Delete: Can't Delete Entry, \"{Entry}\", from Parent");
				return false;
			}

			while (i < path.Length) {
				dict = current as PDictionary;
				array = current as PArray;

				if (array != null) {
					if (!int.TryParse (path[i], out index) || index < 0) {
						Log.LogError (7051, null, $"Delete: Entry, \"{Entry}\", Contains Invalid Array Index");
						return false;
					}

					if (index >= array.Count) {
						Log.LogError (7052, null, $"Delete: Entry, \"{Entry}\", Does Not Exist");
						return false;
					}

					current = array[index];
				} else if (dict != null) {
					if (!dict.TryGetValue (path[i], out current)) {
						Log.LogError (7052, null, $"Delete: Entry, \"{Entry}\", Does Not Exist");
						return false;
					}
				} else {
					Log.LogError (7052, null, $"Delete: Entry, \"{Entry}\", Does Not Exist");
					return false;
				}

				i++;
			}

			current.Remove ();

			return true;
		}

		bool Import (PObject plist)
		{
			var path = GetPropertyPath ();
			var current = plist;
			PDictionary dict;
			PObject value;
			PArray array;
			int index;
			int i = 0;

			if (path.Length == 0) {
				Log.LogError (7053, null, $"Import: Entry, \"{Entry}\", Incorrectly Specified");
				return false;
			}

			while (i < path.Length - 1) {
				dict = current as PDictionary;
				array = current as PArray;

				if (array != null) {
					if (!int.TryParse (path[i], out index) || index < 0 || index >= array.Count) {
						Log.LogError (7054, null, $"Import: Entry, \"{Entry}\", Contains Invalid Array Index");
						return false;
					}

					current = array[index];
				} else if (dict != null) {
					if (!dict.TryGetValue (path[i], out current))
						dict[path[i]] = current = new PDictionary ();
				} else {
					Log.LogError (7053, null, $"Import: Entry, \"{Entry}\", Incorrectly Specified");
					return false;
				}

				i++;
			}

			dict = current as PDictionary;
			array = current as PArray;

			if (array != null) {
				if (path[i].Length == 0) {
					index = array.Count;
				} else if (!int.TryParse (path[i], out index) || index < 0) {
					Log.LogError (7054, null, $"Import: Entry, \"{Entry}\", Contains Invalid Array Index");
					return false;
				}

				try {
					value = new PData (File.ReadAllBytes (Value));
				} catch {
					Log.LogError (7055, null, $"Import: Error Reading File: {Value}", Value);
					return false;
				}

				if (index < array.Count)
					array.Insert (index, value);
				else
					array.Add (value);
			} else if (dict != null) {
				try {
					value = new PData (File.ReadAllBytes (Value));
				} catch {
					Log.LogError (7055, null, $"Import: Error Reading File: {Value}", Value);
					return false;
				}

				dict[path[i]] = value;
			} else {
				Log.LogError (7056, null, $"Import: Can't Add Entry, \"{Entry}\", to Parent");
				return false;
			}

			return true;
		}

		bool Merge (PObject plist, PObject value)
		{
			switch (plist.Type) {
			case PObjectType.Dictionary:
				if (value.Type == PObjectType.Array) {
					Log.LogError (7057, PropertyList, "Merge: Can't Add array Entries to dict");
					return false;
				}

				var dict = (PDictionary) plist;

				if (value.Type == PObjectType.Dictionary) {
					var import = (PDictionary) value;

					foreach (var item in import) {
						if (dict.ContainsKey (item.Key)) {
							Log.LogMessage (MessageImportance.Low, "Duplicate Entry Was Skipped: {0}", item.Key);
							continue;
						}

						dict.Add (item.Key, item.Value.Clone ());
					}
				} else if (!dict.ContainsKey (string.Empty)) {
					dict.Add (string.Empty, value);
				}
				break;
			case PObjectType.Array:
				var array = (PArray) plist;

				if (value.Type == PObjectType.Array) {
					var import = (PArray) value;

					for (int i = 0; i < import.Count; i++)
						array.Add (import[i].Clone ());
				} else {
					array.Add (value);
				}
				break;
			default:
				Log.LogError (7058, PropertyList, "Merge: Specified Entry Must Be a Container");
				return false;
			}

			return true;
		}

		bool Merge (PObject plist)
		{
			if (Entry != null) {
				var path = GetPropertyPath ();
				var current = plist;
				PDictionary dict;
				PObject value;
				PArray array;
				int index;
				int i = 0;

				while (i < path.Length - 1) {
					dict = current as PDictionary;
					array = current as PArray;

					if (array != null) {
						if (!int.TryParse (path[i], out index) || index < 0) {
							Log.LogError (7059, PropertyList, $"Merge: Entry, \"{Entry}\", Contains Invalid Array Index");
							return false;
						}

						if (index >= array.Count) {
							Log.LogError (7060, PropertyList, $"Merge: Entry, \"{Entry}\", Does Not Exist");
							return false;
						}

						current = array[index];
					} else if (dict != null) {
						if (!dict.TryGetValue (path[i], out current)) {
							Log.LogError (7060, PropertyList, $"Merge: Entry, \"{Entry}\", Does Not Exist");
							return false;
						}
					} else {
						Log.LogError (7060, PropertyList, $"Merge: Entry, \"{Entry}\", Does Not Exist");
						return false;
					}

					i++;
				}

				dict = current as PDictionary;
				array = current as PArray;
				PObject root;

				if (array != null) {
					if (i > 0 || path[i].Length > 0) {
						if (!int.TryParse (path[i], out index) || index < 0) {
							Log.LogError (7059, PropertyList, $"Merge: Entry, \"{Entry}\", Contains Invalid Array Index");
							return false;
						}

						if (index >= array.Count) {
							Log.LogError (7060, PropertyList, $"Merge: Entry, \"{Entry}\", Does Not Exist");
							return false;
						}

						root = array[index];
					} else {
						root = array;
					}

					try {
						value = PObject.FromFile (Value);
					} catch {
						Log.LogError (7061, PropertyList, $"Merge: Error Reading File: {Value}", Value);
						return false;
					}

					return Merge (root, value);
				}

				if (dict != null) {
					if (i > 0 || path[i].Length > 0) {
						if (!dict.TryGetValue (path[i], out root)) {
							Log.LogError (7060, PropertyList, $"Merge: Entry, \"{Entry}\", Does Not Exist");
							return false;
						}
					} else {
						root = dict;
					}

					try {
						value = PObject.FromFile (Value);
					} catch {
						Log.LogError (7061, PropertyList, $"Merge: Error Reading File: {Value}", Value);
						return false;
					}

					return Merge (root, value);
				}

				Log.LogError (7060, PropertyList, $"Merge: Entry, \"{Entry}\", Does Not Exist");
				return false;
			} else {
				PObject value;

				try {
					value = PObject.FromFile (Value);
				} catch {
					Log.LogError (7061, PropertyList, $"Merge: Error Reading File: {Value}", Value);
					return false;
				}

				return Merge (plist, value);
			}
		}

		bool Set (PObject plist)
		{
			var path = GetPropertyPath ();
			var current = plist;
			PDictionary dict;
			PObject value;
			PArray array;
			int index;
			int i = 0;

			if (path.Length == 0) {
				Log.LogError (7062, PropertyList, $"Set: Entry, \"{Entry}\", Incorrectly Specified");
				return false;
			}

			while (i < path.Length - 1) {
				dict = current as PDictionary;
				array = current as PArray;

				if (array != null) {
					if (!int.TryParse (path[i], out index) || index < 0) {
						Log.LogError (7063, PropertyList, $"Set: Entry, \"{Entry}\", Contains Invalid Array Index");
						return false;
					}

					if (index >= array.Count) {
						Log.LogError (7064, PropertyList, $"Set: Entry, \"{Entry}\", Does Not Exist");
						return false;
					}

					current = array[index];
				} else if (dict != null) {
					if (!dict.TryGetValue (path[i], out current)) {
						Log.LogError (7064, PropertyList, $"Set: Entry, \"{Entry}\", Does Not Exist");
						return false;
					}
				} else {
					Log.LogError (7064, PropertyList, $"Set: Entry, \"{Entry}\", Does Not Exist");
					return false;
				}

				i++;
			}

			dict = current as PDictionary;
			array = current as PArray;

			if (array != null) {
				if (!int.TryParse (path[i], out index) || index < 0) {
					Log.LogError (7063, PropertyList, $"Set: Entry, \"{Entry}\", Contains Invalid Array Index");
					return false;
				}

				if (index >= array.Count) {
					Log.LogError (7064, PropertyList, $"Set: Entry, \"{Entry}\", Does Not Exist");
					return false;
				}

				if (!CreateValue (Type ?? string.Empty, Value, out value))
					return false;

				array[index] = value;
			} else if (dict != null) {
				if (!dict.TryGetValue (path[i], out value)) {
					Log.LogError (7064, PropertyList, $"Set: Entry, \"{Entry}\", Does Not Exist");
					return false;
				}

				// fall back to the existing type if a type was not explicitly specified
				var type = Type ?? GetType (value);

				if (!CreateValue (type, Value, out value))
					return false;

				dict[path[i]] = value;
			} else {
				Log.LogError (7064, PropertyList, $"Set: Entry, \"{Entry}\", Does Not Exist");
				return false;
			}

			return true;
		}

		public override bool Execute ()
		{
			PropertyListEditorAction action;
			PObject plist;
			bool binary;

			if (!Enum.TryParse (Action, out action)) {
				Log.LogError (7065, null, $"Unknown PropertyList editor action: {Action}");
				return false;
			}

			if (File.Exists (PropertyList)) {
				try {
					plist = PObject.FromFile (PropertyList, out binary);
				} catch (Exception ex) {
					Log.LogError (7066, PropertyList, $"Error loading '{PropertyList}': {ex.Message}", PropertyList);
					return false;
				}
			} else {
				Log.LogMessage (MessageImportance.Low, "File Doesn't Exist, Will Create: {0}", PropertyList);
				plist = new PDictionary ();
				binary = false;
			}

			switch (action) {
			case PropertyListEditorAction.Add:
				Add (plist);
				break;
			case PropertyListEditorAction.Clear:
				Clear (ref plist);
				break;
			case PropertyListEditorAction.Delete:
				Delete (plist);
				break;
			case PropertyListEditorAction.Import:
				Import (plist);
				break;
			case PropertyListEditorAction.Merge:
				Merge (plist);
				break;
			case PropertyListEditorAction.Set:
				Set (plist);
				break;
			}

			if (!Log.HasLoggedErrors) {
				try {
					if (plist is PDictionary) {
						((PDictionary) plist).Save (PropertyList, true, binary);
					} else {
						File.WriteAllText (PropertyList, plist.ToXml ());
					}
				} catch (Exception ex) {
					Log.LogError (7067, PropertyList, $"Error saving '{PropertyList}': {ex.Message}", PropertyList);
				}
			}

			return !Log.HasLoggedErrors;
		}
	}
}
