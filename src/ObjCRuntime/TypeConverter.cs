// Copyright 2011, 2012 Xamarin Inc
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using Foundation;

namespace ObjCRuntime {

	public static class TypeConverter {
#if !COREBUILD
		/*
		 * Converts Obj-C type encodings to managed types
		 * This list is not exhaustive.  If you come across a missing type see:
		 *
		 * http://developer.apple.com/documentation/DeveloperTools/gcc-4.0.1/gcc/Type-encoding.html
		 */
		[BindingImpl (BindingImplOptions.Optimizable)] // To inline the Runtime.DynamicRegistrationSupported code if possible.
		public static Type ToManaged (string type)
		{
			if (!Runtime.DynamicRegistrationSupported) // The call to Runtime.GetAssemblies further below requires the dynamic registrar.
				throw ErrorHelper.CreateError (8026, "TypeConverter.ToManaged is not supported when the dynamic registrar has been linked away.");

			switch (type [0]) {
			case '@':
				return typeof (IntPtr);
			case '#':
				return typeof (IntPtr);
			case ':':
				return typeof (IntPtr);
			case 'c':
				return typeof (char);
			case 'C':
				return typeof (char);
			case 's':
				return typeof (short);
			case 'S':
				return typeof (ushort);
			case 'i':
			case 'l':
				return typeof (int);
			case 'I':
			case 'L':
				return typeof (uint);
			case 'q':
				return typeof (long);
			case 'Q':
				return typeof (ulong);
			case 'f':
				return typeof (float);
			case 'd':
				return typeof (double);
			case 'b':
				return typeof (char); // XXX: bitfield?
			case 'B':
				return typeof (bool);
			case 'v':
				return typeof (void);
			case '?':
				return typeof (IntPtr); // XXX: undef?
			case '^':
				return typeof (IntPtr);
			case '*':
				return typeof (string);
			case '%':
				return typeof (IntPtr); // XXX: Atom?
			case '[':
				throw new NotImplementedException ("arrays");
			case '(':
				throw new NotImplementedException ("unions");
			case '{': {
				string struct_name = type.Substring (1, type.IndexOf ('=') - 1);
				var assemblies = Runtime.GetAssemblies ();

				// TODO: caching? valuetype specific list of structs to walk?  speed me up
				foreach (Assembly a in assemblies)
					foreach (Type t in a.GetTypes ())
						if (t.IsValueType && !t.IsEnum && t.Name == struct_name)
							return t;

				throw new NotImplementedException ("struct marshalling: " + struct_name + " " + type);
			}
			case '!':
				throw new NotImplementedException ("vectors");
			case 'r':
				throw new NotImplementedException ("consts");
			}

			throw new Exception ("Teach me how to parse: " + type);
		}

		/*
		 * Converts managed types to Obj-C type encodings
		 * This list is not exhaustive.  If you come across a missing type see:
		 *
		 * http://developer.apple.com/documentation/DeveloperTools/gcc-4.0.1/gcc/Type-encoding.html
		 */
		public static string ToNative (Type type)
		{
			if (type.IsGenericParameter)
				throw new ArgumentException ("Unable to convert generic types");

			if (type.IsByRef) return "^" + ToNative (type.GetElementType ());
			if (type == typeof (IntPtr)) return "^v";
			if (type == typeof (byte)) return "C";
			if (type == typeof (sbyte)) return "c";
			if (type == typeof (char)) return "c";
			//			if (type == typeof (uchar)) return "C";
			if (type == typeof (short)) return "s";
			if (type == typeof (ushort)) return "S";
			if (type == typeof (int)) return "i";
			if (type == typeof (uint)) return "I";
			if (type == typeof (long)) return "q";
			if (type == typeof (ulong)) return "Q";
			if (type == typeof (float)) return "f";
			if (type == typeof (double)) return "d";
			if (type == typeof (bool)) return "c"; // map managed 'bool' to ObjC BOOL = unsigned char
			if (type == typeof (void)) return "v";
			if (type == typeof (string)) return "@"; // We handle NSString as MonoString automagicaly
			if (type == typeof (Selector)) return ":";
			if (type == typeof (Class)) return "#";
			if (type == typeof (nfloat)) return IntPtr.Size == 8 ? "d" : "f";
			if (type == typeof (nint)) return IntPtr.Size == 8 ? "q" : "i";
			if (type == typeof (nuint)) return IntPtr.Size == 8 ? "Q" : "I";
			if (typeof (INativeObject).IsAssignableFrom (type)) return "@";
			if (type.IsValueType && !type.IsEnum) {
				// TODO: We should cache the results of this in a temporary hash that we destroy when we're done initializing/registrations
				StringBuilder sb = new StringBuilder ();

				sb.AppendFormat ("{{{0}=", type.Name);
				foreach (FieldInfo field in type.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
					sb.Append (ToNative (field.FieldType));
				sb.Append ("}");

				return sb.ToString ();
			}
			if (type.IsValueType && type.IsEnum) return ToNative (Enum.GetUnderlyingType (type));
			if (type.IsArray) return "@"; // All arrays are NSArray's
			if (type.IsSubclassOf (typeof (System.Delegate))) return "^v"; // Assume that every delegate is a block
			throw new NotImplementedException ("Don't know how to marshal: " + type.ToString ());
		}
#endif // !COREBUILD
	}
}
