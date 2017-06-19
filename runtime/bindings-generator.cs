using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
 * This code will generate native functions that are referenced by the MarshalDirective
 * attribute in binding code.
 * 
 * If a MarshalDirective makes the generator create a P/Invoke to a signature
 * that doesn't already exist, a new signature entry will have to be added
 * in the GetFunctionData method.
 * 
 * Then just re-build and bindings-generated.m will be re-created with the
 * new signature.
 * 
 */

namespace Xamarin.BindingMethods.Generator
{
	class MainClass
	{
		public static int Main (string[] args)
		{
			if (args.Length == 0) {
				Console.WriteLine ("Output filename must be specified");
				return 1;
			}

			var fn = args [0];

			var data = GetFunctionData ();

			using (var writer = new StringWriter ()) {
				writer.WriteLine ("/* This file is generated */");
				writer.WriteLine ();
				writer.WriteLine ("#include \"bindings.h\"");
				writer.WriteLine ();
				writer.WriteLine ("#ifdef __cplusplus");
				writer.WriteLine ("extern \"C\" {");
				writer.WriteLine ("#endif");
				writer.WriteLine ();

				foreach (var func in data) {
					writer.WriteLine (func.Comment);
					writer.WriteLine ();
					if ((func.Variants & Variants.msgSend) == Variants.msgSend) {
						Write_objc_msgSend (writer, func);
					}
					if ((func.Variants & Variants.msgSendSuper) == Variants.msgSendSuper) {
						Write_objc_msgSendSuper (writer, func);
					}
					if ((func.Variants & Variants.msgSend_stret) == Variants.msgSend_stret) {
						Write_objc_msgSend_stret (writer, func);
					}
					if ((func.Variants & Variants.msgSendSuper_stret) == Variants.msgSendSuper_stret) {
						Write_objc_msgSendSuper_stret (writer, func);
					}
				}

				writer.WriteLine ("#ifdef __cplusplus");
				writer.WriteLine ("} /* extern \"C\" */");
				writer.WriteLine ("#endif");
				writer.WriteLine ();

				var generated_data = writer.ToString ();
				if (!File.Exists (fn) || File.ReadAllText (fn) != generated_data)
					File.WriteAllText (fn, generated_data);

				return 0;
			}
		}

		static IEnumerable<FunctionData> GetFunctionData ()
		{
			var data = new List<FunctionData> ();

			data.Add (
				new FunctionData {
					Comment = " // void func (Vector3)",
					Prefix = "simd__",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 }
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector3 func (Vector3)",
					Prefix = "simd__",
					Variants = Variants.All,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 }
					},
					ReturnType = Types.Vector3,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector3 func ()",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector3,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector3 func (Double)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector3,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Double }
					}
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (Vector3, Double)",
					Prefix = "simd__",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.Double },
					}
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.IntPtr },
					}
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector3 func () => Vector4f func ()",
					Prefix = "vector_float3__",
					Variants = Variants.All,
					ReturnType = Types.Vector3As4,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (Vector3) => void func (Vector4f)",
					Prefix = "vector_float3__",
					Variants = Variants.NonStret, 
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3As4 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (Vector2)",
					Prefix = "simd__",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector2 }
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector2 func ()",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector2,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (Vector4)",
					Prefix = "simd__",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector4 }
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector4 func ()",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector4,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, nuint, Vector3)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Vector3 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, nuint, Vector4)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Vector4 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, nuint, Vector2)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Vector2 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector2i func ()",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector2i,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (Vector2i)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector2i },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, UInt32, Vector2i)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.UInt32 },
						new ParameterData { TypeData = Types.Vector2i },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector2)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector2 }
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector2 func (nuint)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector2,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.NUInt }
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector2i, Int32, Int32, bool)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Bool }
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, IntPtr, Vector2i)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, IntPtr, Vector2i, float)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Float },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, bool, IntPtr, Vector2i, nint, nuint, int, bool)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Bool },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, bool, IntPtr, Vector2i, nint, nuint, Int64, bool)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Int64 },
						new ParameterData { TypeData = Types.Bool },
					},
				}
			);
			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector2, Vector2i, nint, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, nuint, nuint, int, bool, bool, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, nuint, nuint, Int64, bool, bool, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Int64 },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, Vector3i, nint, bool, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.Vector3i },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, Vector3i, bool, nint, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.Vector3i },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (Vector3, Vector3)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.Vector3 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, Vector2, nuint, nuint, int, bool, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, Vector2, nuint, nuint, Int64, bool, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Int64 },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, Vector2, nuint, nuint, nuint, int, bool, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, Vector2, nuint, nuint, nuint, Int64, bool, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Int64 },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // bool func (Vector2i, IntPtr, IntPtr, IntPtr, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.Bool,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // bool func (Vector2i, nint, float, IntPtr, IntPtr, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.Bool,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // MDLAxisAlignedBoundingBox func ()",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.MDLAxisAlignedBoundingBox,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (MDLAxisAlignedBoundingBox)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.MDLAxisAlignedBoundingBox },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (MDLAxisAlignedBoundingBox, bool)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.MDLAxisAlignedBoundingBox },
						new ParameterData { TypeData = Types.Bool },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // MDLAxisAlignedBoundingBox func (Vector4i)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.MDLAxisAlignedBoundingBox,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector4i },
					},
				}
			);
				
			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, MDLAxisAlignedBoundingBox, Vector3)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.MDLAxisAlignedBoundingBox },
						new ParameterData { TypeData = Types.Vector3 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // MDLAxisAlignedBoundingBox func (double)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.MDLAxisAlignedBoundingBox,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Double },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector3 func (Vector4i)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector3,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector4i },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (Vector4i)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector4i },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector4i func (Vector3)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector4i,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, IntPtr, Vector2i, Int64)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Int64 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (bool, IntPtr, Int64, Vector2i, float, float, float, float)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Int64 },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Float },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // bool func (Vector4i, bool, bool, bool, bool)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.Bool,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector4i },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.Bool },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, nint, Vector2i, float, float, float, float)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Float },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, float, IntPtr, Vector2i)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, IntPtr, Vector2i, int)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Int32 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, IntPtr, Vector2i, int, Int64, bool)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int64 },
						new ParameterData { TypeData = Types.Bool },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, IntPtr, Vector2i, int, Int64, IntPtr, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int64 },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, IntPtr, Vector2i, int, int, bool)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Bool },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, IntPtr, Vector2i, int, int, IntPtr, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, IntPtr, IntPtr, Vector2i)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2i },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector2i)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector2i },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector3 func (Vector2i, Vector2i)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector3,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Vector2i },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector3 func (IntPtr)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector3,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, nuint, Matrix4)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.NUInt },
						new ParameterData { TypeData = Types.Matrix4f },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Matrix4 func ()",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Matrix4f,
				}
			);
			
			data.Add (
				new FunctionData {
					Comment = " // void func (Matrix4)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Matrix4f },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (Matrix4, Double)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Matrix4f },
						new ParameterData { TypeData = Types.Double },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Matrix4 func (Double)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Matrix4f,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Double },
					},
				}
			);
			
			data.Add (
				new FunctionData {
					Comment = " // Matrix4 func (IntPtr, Double)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Matrix4f,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Double },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Matrix4)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Matrix4f },
					},
				}
			);
			
			data.Add (
				new FunctionData {
					Comment = " // Vector3f func (Vector3f)",
					Prefix = "vector_float3__",
					Variants = Variants.All,
					ReturnType = Types.Vector3,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
					},
				}
			);
			

			/* Exception handling */
			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
					},
					ReturnType = Types.IntPtr,
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, ref IntPtr); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr, IsRef = true },
					},
					ReturnType = Types.IntPtr,
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, IntPtr, int); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Int32 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, IntPtr, uint); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.UInt32 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, IntPtr, UInt64); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.UInt64 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (int, int, int, int); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, int, int, int); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, int, int, int, int); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (int, int, int); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (nint, nint, nint, uint32); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.UInt32 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (nint, nint, nint, uint64); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.UInt64 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, nint, nint, nint, uint64); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.UInt64 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, nint, nint, nint, uint32); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.UInt32 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, uint32, nint, uint32); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.UInt32 },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.UInt32 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, uint64, nint, uint64); @try",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.UInt64 },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.UInt64 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, Vector2); @try",
					Prefix = "simd__",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, Vector2, Vector2); @try",
					Prefix = "simd__",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Vector2 },
										},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector2, Vector2); @try",
					Prefix = "simd__",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Vector2 },
					},
					MarshalExceptions = true,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector2, Vector2, float); @try",
					Prefix = "simd__",
					Variants = Variants.msgSend | Variants.msgSendSuper,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Float },
					},
					MarshalExceptions = true,
				}
			);

			// Required for SpriteKit
			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, Matrix2)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Matrix2f },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, Matrix3)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Matrix3f },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, Matrix4)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Matrix4f },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, Vector3)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector3 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, Vector4)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector4 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector4)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector4 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (nint, nint, Vector2, Vector2)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Vector2 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Matrix2 func ()",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Matrix2f,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Matrix3 func ()",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Matrix3f,
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector2 func (nint)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector2,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.NInt },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (Matrix2)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Matrix2f },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (Matrix3)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Matrix3f },
					},
				}
			);

			// Required for ModelIO
			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, bool, nint, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, Vector2i, bool, IntPtr, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Matrix4, bool);",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Matrix4f },
						new ParameterData { TypeData = Types.Bool },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, Vector2i, bool, nint, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, nint, UInt32, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.UInt32 },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, Vector2i, bool, bool, nint, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, Vector2i, bool, bool, bool, nint, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, Vector2i, int, bool, nint, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector3, Vector2i, nint, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData[] {
						new ParameterData { TypeData = Types.Vector3 },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.NInt },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, Vector2d, Vector2d, Vector2i, bool)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2d },
						new ParameterData { TypeData = Types.Vector2d },
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Bool },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (Vector2i, int, int, bool, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.Vector2i },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Int32 },
						new ParameterData { TypeData = Types.Bool },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, Vector2, Vector2)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Vector2 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (float, Vector2, Vector2, IntPtr)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.IntPtr },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector2d func ()",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector2d
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // Vector3 func (nuint)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.Vector3,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.NUInt },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // float func (Vector2)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.Float,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.Vector2 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // float func (Vector2i)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.Float,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.Vector2i },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (IntPtr, Vector2, Vector2, Int64)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Int64 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (IntPtr, Vector2, Vector2, Int)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Int32 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (IntPtr, Vector2, Vector2)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.Vector2 },
						new ParameterData { TypeData = Types.Vector2 },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (Vector3d)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.Vector3d },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // void func (float, Vector2i)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.Float },
						new ParameterData { TypeData = Types.Vector2i },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // GKBox func ()",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.GKBox
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (GKBox)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.GKBox },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (GKBox, float)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.GKBox },
						new ParameterData { TypeData = Types.Float },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, GKBox)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.GKBox },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // GKQuad func ()",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.GKQuad
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (GKQuad)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.GKQuad },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (GKQuad, float)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.GKQuad },
						new ParameterData { TypeData = Types.Float },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // IntPtr func (IntPtr, GKQuad)",
					Prefix = "simd__",
					Variants = Variants.NonStret,
					ReturnType = Types.IntPtr,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.IntPtr },
						new ParameterData { TypeData = Types.GKQuad },
					},
				}
			);

			data.Add (
				new FunctionData {
					Comment = " // GKTriangle func (nuint)",
					Prefix = "simd__",
					Variants = Variants.All,
					ReturnType = Types.GKTriangle,
					Parameters = new ParameterData [] {
						new ParameterData { TypeData = Types.NUInt },
					},
				}
			);

			// We must expand functions with native types to their actual type as well.
			for (int i = data.Count - 1; i >= 0; i--) {
				if (!data [i].HasNativeType)
					continue;
				data.Add (data [i].CloneAsResolvedNativeType (true));
				data.Add (data [i].CloneAsResolvedNativeType (false));
			}

			return data;
		}

		static string GetTypeNameForSignature (string managed_type)
		{
			return managed_type;
		}

		static void MarshalToManaged (StringWriter writer, TypeData type, string nativeVariable, string managedVariable)
		{
			switch (type.ManagedType) {
			case "Vector2d":
			case "Vector2i":
			case "Vector2":
				writer.WriteLine ("\t{0}.a = {1} [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.b = {1} [1];", managedVariable, nativeVariable);
				break;
			case "Vector3d":
			case "Vector3i":
			case "Vector3":
				writer.WriteLine ("\t{0}.a = {1} [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.b = {1} [1];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.c = {1} [2];", managedVariable, nativeVariable);
				break;
			case "Vector4d":
			case "Vector4i":
			case "Vector4":
				if (type.NativeType == "vector_float3") {
					writer.WriteLine ("\t{0}.a = {1} [0];", managedVariable, nativeVariable);
					writer.WriteLine ("\t{0}.b = {1} [1];", managedVariable, nativeVariable);
					writer.WriteLine ("\t{0}.c = {1} [2];", managedVariable, nativeVariable);
					writer.WriteLine ("\t{0}.d = 0;", managedVariable);
				} else if (type.NativeType == "vector_float4" || type.NativeType == "vector_int4" || type.NativeType == "vector_double4") {
					writer.WriteLine ("\t{0}.a = {1} [0];", managedVariable, nativeVariable);
					writer.WriteLine ("\t{0}.b = {1} [1];", managedVariable, nativeVariable);
					writer.WriteLine ("\t{0}.c = {1} [2];", managedVariable, nativeVariable);
					writer.WriteLine ("\t{0}.d = {1} [3];", managedVariable, nativeVariable);
				} else {
					goto default;
				}
				break;
			case "Matrix2":
				writer.WriteLine ("\tfor (int i = 0; i < 2; i++) {");
				writer.WriteLine ("\t\t{0}.columns [i].a = {1}.columns [i] [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t\t{0}.columns [i].b = {1}.columns [i] [1];", managedVariable, nativeVariable);
				writer.WriteLine ("\t}");
				break;
			case "Matrix3":
				writer.WriteLine ("\tfor (int i = 0; i < 3; i++) {");
				writer.WriteLine ("\t\t{0}.columns [i].a = {1}.columns [i] [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t\t{0}.columns [i].b = {1}.columns [i] [1];", managedVariable, nativeVariable);
				writer.WriteLine ("\t\t{0}.columns [i].c = {1}.columns [i] [2];", managedVariable, nativeVariable);
				writer.WriteLine ("\t}");
				break;
			case "Matrix4":
				writer.WriteLine ("\tfor (int i = 0; i < 4; i++) {");
				writer.WriteLine ("\t\t{0}.columns [i].a = {1}.columns [i] [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t\t{0}.columns [i].b = {1}.columns [i] [1];", managedVariable, nativeVariable);
				writer.WriteLine ("\t\t{0}.columns [i].c = {1}.columns [i] [2];", managedVariable, nativeVariable);
				writer.WriteLine ("\t\t{0}.columns [i].d = {1}.columns [i] [3];", managedVariable, nativeVariable);
				writer.WriteLine ("\t}");
				break;
			case "MDLAxisAlignedBoundingBox":
				writer.WriteLine ("\t{0}.maxBounds.a = {1}.maxBounds [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.maxBounds.b = {1}.maxBounds [1];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.maxBounds.c = {1}.maxBounds [2];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.minBounds.a = {1}.minBounds [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.minBounds.b = {1}.minBounds [1];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.minBounds.c = {1}.minBounds [2];", managedVariable, nativeVariable);
				break;
			case "GKBox":
				writer.WriteLine ("\t{0}.maxBounds.a = {1}.maxBounds [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.maxBounds.b = {1}.maxBounds [1];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.maxBounds.c = {1}.maxBounds [2];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.minBounds.a = {1}.minBounds [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.minBounds.b = {1}.minBounds [1];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.minBounds.c = {1}.minBounds [2];", managedVariable, nativeVariable);
				break;
			case "GKQuad":
				writer.WriteLine ("\t{0}.maxBounds.a = {1}.maxBounds [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.maxBounds.b = {1}.maxBounds [1];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.minBounds.a = {1}.minBounds [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t{0}.minBounds.b = {1}.minBounds [1];", managedVariable, nativeVariable);
				break;
			case "GKTriangle":
				writer.WriteLine ("\tfor (int i = 0; i < 3; i++) {");
				writer.WriteLine ("\t\t{0}.points [i].a = {1}.points [i] [0];", managedVariable, nativeVariable);
				writer.WriteLine ("\t\t{0}.points [i].b = {1}.points [i] [1];", managedVariable, nativeVariable);
				writer.WriteLine ("\t\t{0}.points [i].c = {1}.points [i] [2];", managedVariable, nativeVariable);
				writer.WriteLine ("\t}");
				break;
			default:
				throw new NotImplementedException (string.Format ("MarshalToManaged for: NativeType: {0} ManagedType: {1}", type.NativeType, type.ManagedType));
			}
		}

		static void MarshalToNative (StringWriter writer, TypeData type, string nativeVariable, string managedVariable)
		{
			switch (type.ManagedType) {
			case "Vector2d":
			case "Vector2i":
			case "Vector2":
				writer.WriteLine ("\t{0} [0] = {1}.a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0} [1] = {1}.b;", nativeVariable, managedVariable);
				break;
			case "Vector3d":
			case "Vector3i":
			case "Vector3":
				writer.WriteLine ("\t{0} [0] = {1}.a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0} [1] = {1}.b;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0} [2] = {1}.c;", nativeVariable, managedVariable);
				break;
			case "Vector4d":
			case "Vector4i":
			case "Vector4":
				if (type.NativeType == "vector_float3") {
					writer.WriteLine ("\t{0} [0] = {1}.a;", nativeVariable, managedVariable);
					writer.WriteLine ("\t{0} [1] = {1}.b;", nativeVariable, managedVariable);
					writer.WriteLine ("\t{0} [2] = {1}.c;", nativeVariable, managedVariable);
				} else if (type.NativeType == "vector_float4" || type.NativeType == "vector_int4" || type.NativeType == "vector_double4") {
					writer.WriteLine ("\t{0} [0] = {1}.a;", nativeVariable, managedVariable);
					writer.WriteLine ("\t{0} [1] = {1}.b;", nativeVariable, managedVariable);
					writer.WriteLine ("\t{0} [2] = {1}.c;", nativeVariable, managedVariable);
					writer.WriteLine ("\t{0} [3] = {1}.d;", nativeVariable, managedVariable);
				} else {
					goto default;
				}
				break;
			case "Matrix2":
				writer.WriteLine ("\tfor (int i = 0; i < 2; i++) {");
				writer.WriteLine ("\t\t{0}.columns [i][0] = {1}.columns [i].a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t\t{0}.columns [i][1] = {1}.columns [i].b;", nativeVariable, managedVariable);
				writer.WriteLine ("\t}");
				break;
			case "Matrix3":
				writer.WriteLine ("\tfor (int i = 0; i < 3; i++) {");
				writer.WriteLine ("\t\t{0}.columns [i][0] = {1}.columns [i].a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t\t{0}.columns [i][1] = {1}.columns [i].b;", nativeVariable, managedVariable);
				writer.WriteLine ("\t\t{0}.columns [i][2] = {1}.columns [i].c;", nativeVariable, managedVariable);
				writer.WriteLine ("\t}");
				break;
			case "Matrix4":
				writer.WriteLine ("\tfor (int i = 0; i < 4; i++) {");
				writer.WriteLine ("\t\t{0}.columns [i][0] = {1}.columns [i].a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t\t{0}.columns [i][1] = {1}.columns [i].b;", nativeVariable, managedVariable);
				writer.WriteLine ("\t\t{0}.columns [i][2] = {1}.columns [i].c;", nativeVariable, managedVariable);
				writer.WriteLine ("\t\t{0}.columns [i][3] = {1}.columns [i].d;", nativeVariable, managedVariable);
				writer.WriteLine ("\t}");
				break;
			case "MDLAxisAlignedBoundingBox":
				writer.WriteLine ("\t{0}.maxBounds [0] = {1}.maxBounds.a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.maxBounds [1] = {1}.maxBounds.b;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.maxBounds [2] = {1}.maxBounds.c;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.minBounds [0] = {1}.minBounds.a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.minBounds [1] = {1}.minBounds.b;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.minBounds [2] = {1}.minBounds.c;", nativeVariable, managedVariable);
				break;
			case "GKBox":
				writer.WriteLine ("\t{0}.maxBounds [0] = {1}.maxBounds.a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.maxBounds [1] = {1}.maxBounds.b;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.maxBounds [2] = {1}.maxBounds.c;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.minBounds [0] = {1}.minBounds.a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.minBounds [1] = {1}.minBounds.b;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.minBounds [2] = {1}.minBounds.c;", nativeVariable, managedVariable);
				break;
			case "GKQuad":
				writer.WriteLine ("\t{0}.maxBounds [0] = {1}.maxBounds.a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.maxBounds [1] = {1}.maxBounds.b;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.minBounds [0] = {1}.minBounds.a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t{0}.minBounds [1] = {1}.minBounds.b;", nativeVariable, managedVariable);
				break;
			case "GKTriangle":
				writer.WriteLine ("\tfor (int i = 0; i < 3; i++) {");
				writer.WriteLine ("\t\t{0}.points [i][0] = {1}.points [i].a;", nativeVariable, managedVariable);
				writer.WriteLine ("\t\t{0}.points [i][1] = {1}.points [i].b;", nativeVariable, managedVariable);
				writer.WriteLine ("\t\t{0}.points [i][2] = {1}.points [i].c;", nativeVariable, managedVariable);
				writer.WriteLine ("\t}");
				break;
			default:
				throw new NotImplementedException (string.Format ("MarshalToNative for: NativeType: {0} ManagedType: {1}", type.NativeType, type.ManagedType));
			}
		}

		static string GetFuncName (FunctionData func, string overload)
		{
			var funcName = new StringBuilder ();
			funcName.Append ("xamarin_");
			funcName.Append (func.Prefix);
			if (func.ReturnType != null) {
				funcName.Append (GetTypeNameForSignature (func.ReturnType.ManagedType));
			} else {
				funcName.Append ("void");
			}
			funcName.Append ("_").Append (overload);
			if (func.Parameters != null) {
				for (int i = 0; i < func.Parameters.Length; i++) {
					funcName.Append ("_");
					if (func.Parameters [i].IsRef)
						funcName.Append ("ref_");
					funcName.Append (GetTypeNameForSignature (func.Parameters [i].TypeData.ManagedType));
				}
			}
			return funcName.ToString ();
		}

		static void WriteParametersMarshal (StringWriter writer, ParameterData [] ps)
		{
			if (ps == null)
				return;
			
			for (int i = 0; i < ps.Length; i++) {
				var p = ps [i];
				if (!p.TypeData.RequireMarshal)
					continue;

				writer.WriteLine ("\t{0} v{1};", p.TypeData.NativeType, i);
				MarshalToNative (writer, p.TypeData, "v" + i.ToString (), "p" + i.ToString ());
			}
		}

		static void WriteParametersInvoke (StringWriter writer, ParameterData [] ps)
		{
			if (ps == null)
				return;
			
			for (int i = 0; i < ps.Length; i++) {
				var p = ps [i];
				if (p.TypeData.RequireMarshal) {
					writer.Write (", v{0}", i);
				} else {
					writer.Write (", p{0}", i);
				}
			}
		}

		static void WriteMessageStretSenderCode (StringWriter writer, TypeData type, bool isSuper)
		{
			var nonstret = isSuper ? "objc_msgSendSuper" : "objc_msgSend";
			var stret = isSuper ? "objc_msgSendSuper_stret" : "objc_msgSend_stret";

			writer.WriteLine ("#if __i386__");
			writer.WriteLine ("\tIMP msgSend = (IMP) {0};", type.IsX86Stret ? stret : nonstret);
			writer.WriteLine ("#elif __x86_64__");
			writer.WriteLine ("\tIMP msgSend = (IMP) {0};", type.IsX64Stret ? stret : nonstret);
			writer.WriteLine ("#elif __arm64__");
			writer.WriteLine ("\tIMP msgSend = (IMP) {0};", nonstret);
			writer.WriteLine ("#elif __arm__");
			writer.WriteLine ("\tIMP msgSend = (IMP) {0};", type.IsARMStret ? stret : nonstret);
			writer.WriteLine ("#else");
			writer.WriteLine ("#error unknown architecture");
			writer.WriteLine ("#endif");
		}

		static void WriteCatchHandler (StringWriter writer)
		{
			writer.WriteLine ("\t} @catch (NSException *e) {");
			writer.WriteLine ("\t\txamarin_process_nsexception_using_mode (e, true);");
			writer.WriteLine ("\t}");
		}

		static void Write_objc_msgSend (StringWriter writer, FunctionData func)
		{
			// func name
			var overload = "objc_msgSend";
			var funcName = GetFuncName (func, overload);

			// typedef
			writer.Write ("typedef {0} (*func_{1}) (id self, SEL sel", func.ReturnType == null ? "void" : func.ReturnType.NativeType, funcName.ToString ());
			if (func.Parameters != null) {
				for (int i = 0; i < func.Parameters.Length; i++) {
					writer.Write (", {0} f{1}", func.Parameters [i].TypeData.NativeType, i);
				}
			}
			writer.WriteLine (");");

			// declaration
			writer.WriteLine (func.ReturnType != null ? func.ReturnType.NativeWrapperType : "void");
			writer.Write (funcName);
			writer.Write (" (id self, SEL sel");
			if (func.Parameters != null) {
				for (int i = 0; i < func.Parameters.Length; i++) {
					writer.Write (", {0} p{1}", func.Parameters [i].TypeData.NativeWrapperType, i);
				}
			}
			writer.WriteLine (")");

			// body
			writer.WriteLine ("{");
			if (func.ReturnType != null && func.ReturnType.RequireMarshal) {
				writer.WriteLine ("\t{0} rv;", func.ReturnType.NativeType);
			}

			// marshal managed parameters to native format
			WriteParametersMarshal (writer, func.Parameters);

			if (func.ReturnType != null && func.ReturnType.IsAnyStret)
				WriteMessageStretSenderCode (writer, func.ReturnType, false);
			
			// @try
			string indent = "\t";
			if (func.MarshalExceptions) {
				writer.WriteLine ("\t@try {");
				indent = "\t\t";
			}

			// invoke
			writer.Write (indent);
			if (func.ReturnType != null) {
				if (func.ReturnType.RequireMarshal) {
					writer.Write ("rv = ");
				} else {
					writer.Write ("return ");
				}
			}
			writer.Write ("((func_{0}) {1}) (self, sel", funcName, (func.ReturnType != null && func.ReturnType.IsAnyStret) ? "msgSend" : overload);
			WriteParametersInvoke (writer, func.Parameters);
			writer.WriteLine (");");

			if (func.ReturnType != null) {
				if (func.ReturnType.RequireMarshal) {
					// Marshal return value back
					writer.WriteLine ("{0}{1} rvm;", indent, func.ReturnType.NativeWrapperType);
					MarshalToManaged (writer, func.ReturnType, "rv", "rvm");
					writer.WriteLine ("{0}return rvm;", indent);
				}
			}

			// @catch
			if (func.MarshalExceptions)
				WriteCatchHandler (writer);

			writer.WriteLine ("}");
			writer.WriteLine ();
		}

		static void Write_objc_msgSendSuper (StringWriter writer, FunctionData func)
		{
			// func name
			var overload = "objc_msgSendSuper";
			var funcName = GetFuncName (func, overload);

			// typedef
			writer.Write ("typedef {0} (*func_{1}) (struct objc_super *super, SEL sel", func.ReturnType == null ? "void" : func.ReturnType.NativeType, funcName.ToString ());
			if (func.Parameters != null) {
				for (int i = 0; i < func.Parameters.Length; i++) {
					writer.Write (", {0} f{1}", func.Parameters [i].TypeData.NativeType, i);
				}
			}
			writer.WriteLine (");");

			// declaration
			writer.WriteLine (func.ReturnType != null ? func.ReturnType.NativeWrapperType : "void");
			writer.Write (funcName);
			writer.Write (" (struct objc_super *super, SEL sel");
			if (func.Parameters != null) {
				for (int i = 0; i < func.Parameters.Length; i++) {
					writer.Write (", {0} p{1}", func.Parameters [i].TypeData.NativeWrapperType, i);
				}
			}
			writer.WriteLine (")");

			// body
			writer.WriteLine ("{");
			if (func.ReturnType != null && func.ReturnType.RequireMarshal) {
				writer.WriteLine ("\t{0} rv;", func.ReturnType.NativeType);
			}

			// marshal managed parameters to native format
			WriteParametersMarshal (writer, func.Parameters);

			if (func.ReturnType != null && func.ReturnType.IsAnyStret)
				WriteMessageStretSenderCode (writer, func.ReturnType, true);

			// @try
			string indent = "\t";
			if (func.MarshalExceptions) {
				writer.WriteLine ("\t@try {");
				indent = "\t\t";
			}

			// invoke
			writer.Write (indent);
			if (func.ReturnType != null) {
				if (func.ReturnType.RequireMarshal) {
					writer.Write ("rv = ");
				} else {
					writer.Write ("return ");
				}
			}
			writer.Write ("((func_{0}) {1}) (super, sel", funcName, (func.ReturnType != null && func.ReturnType.IsAnyStret) ? "msgSend" : overload);
			WriteParametersInvoke (writer, func.Parameters);
			writer.WriteLine (");");

			if (func.ReturnType != null) {
				if (func.ReturnType.RequireMarshal) {
					// Marshal return value back
					writer.WriteLine ("{0}{1} rvm;", indent, func.ReturnType.NativeWrapperType);
					MarshalToManaged (writer, func.ReturnType, "rv", "rvm");
					writer.WriteLine ("{0}return rvm;", indent);
				}
			}

			// @catch
			if (func.MarshalExceptions)
				WriteCatchHandler (writer);
				
			writer.WriteLine ("}");
			writer.WriteLine ();
		}

		static void Write_objc_msgSend_stret (StringWriter writer, FunctionData func)
		{
			if (func.ReturnType == null)
				throw new Exception (string.Format ("stret functions must have a return type: {0}", func.Comment));

			// func name
			var overload = "objc_msgSend_stret";
			var funcName = GetFuncName (func, overload);

			// typedef
			writer.Write ("typedef {0} (*func_{1}) (id self, SEL sel", func.ReturnType.NativeType, funcName.ToString ());
			if (func.Parameters != null) {
				for (int i = 0; i < func.Parameters.Length; i++) {
					writer.Write (", {0} f{1}", func.Parameters [i].TypeData.NativeType, i);
				}
			}
			writer.WriteLine (");");

			// declaration
			writer.WriteLine ("void");
			writer.Write (funcName);
			writer.Write (" ({0} *stret_rv, id self, SEL sel", func.ReturnType.NativeWrapperType);
			if (func.Parameters != null) {
				for (int i = 0; i < func.Parameters.Length; i++) {
					writer.Write (", {0} p{1}", func.Parameters [i].TypeData.NativeWrapperType, i);
				}
			}
			writer.WriteLine (")");

			// body
			writer.WriteLine ("{");
			if (func.ReturnType.RequireMarshal)
				writer.WriteLine ("\t{0} rv;", func.ReturnType.NativeType);

			// marshal managed parameters to native format
			WriteParametersMarshal (writer, func.Parameters);

			// @try
			string indent = "\t";
			if (func.MarshalExceptions) {
				writer.WriteLine ("\t@try {");
				indent = "\t\t";
			}

			if (func.ReturnType.IsAnyStret)
				WriteMessageStretSenderCode (writer, func.ReturnType, false);

			// invoke
			writer.Write (indent);
			if (func.ReturnType.RequireMarshal) {
				writer.Write ("rv = ");
			} else {
				writer.Write ("*stret_rv = ");
			}
			writer.Write ("((func_{0}) {1}) (self, sel", funcName, func.ReturnType.IsAnyStret ? "msgSend" : overload.Replace ("_stret", ""));
			WriteParametersInvoke (writer, func.Parameters);
			writer.WriteLine (");");

			if (func.ReturnType.RequireMarshal) {
				// Marshal return value back
				MarshalToManaged (writer, func.ReturnType, "rv", "(*stret_rv)");
			}

			// @catch
			if (func.MarshalExceptions)
				WriteCatchHandler (writer);

			writer.WriteLine ("}");
			writer.WriteLine ();
		}

		static void Write_objc_msgSendSuper_stret (StringWriter writer, FunctionData func)
		{
			if (func.ReturnType == null)
				throw new Exception (string.Format ("stret functions must have a return type: {0}", func.Comment));
			
			// func name
			var overload = "objc_msgSendSuper_stret";
			var funcName = GetFuncName (func, overload);

			// typedef
			writer.Write ("typedef {0} (*func_{1}) (struct objc_super *super, SEL sel", func.ReturnType.NativeType, funcName.ToString ());
			if (func.Parameters != null) {
				for (int i = 0; i < func.Parameters.Length; i++) {
					writer.Write (", {0} f{1}", func.Parameters [i].TypeData.NativeType, i);
				}
			}
			writer.WriteLine (");");

			// declaration
			writer.WriteLine ("void");
			writer.Write (funcName);
			writer.Write (" ({0} *stret_rv, struct objc_super *super, SEL sel", func.ReturnType.NativeWrapperType);
			if (func.Parameters != null) {
				for (int i = 0; i < func.Parameters.Length; i++) {
					writer.Write (", {0} p{1}", func.Parameters [i].TypeData.NativeWrapperType, i);
				}
			}
			writer.WriteLine (")");

			// body
			writer.WriteLine ("{");
			if (func.ReturnType.RequireMarshal)
				writer.WriteLine ("\t{0} rv;", func.ReturnType.NativeType);

			// marshal managed parameters to native format
			WriteParametersMarshal (writer, func.Parameters);

			if (func.ReturnType.IsAnyStret)
				WriteMessageStretSenderCode (writer, func.ReturnType, true);

			// @try
			string indent = "\t";
			if (func.MarshalExceptions) {
				writer.WriteLine ("\t@try {");
				indent = "\t\t";
			}

			// invoke
			writer.Write (indent);
			if (func.ReturnType.RequireMarshal) {
				writer.Write ("rv = ");
			} else {
				writer.Write ("*stret_rv = ");
			}
			writer.Write ("((func_{0}) {1}) (super, sel", funcName, func.ReturnType.IsAnyStret ? "msgSend" : overload.Replace ("_stret", ""));
			WriteParametersInvoke (writer, func.Parameters);
			writer.WriteLine (");");

			if (func.ReturnType.RequireMarshal) {
				// Marshal return value back
				MarshalToManaged (writer, func.ReturnType, "rv", "(*stret_rv)");
			}

			// @catch
			if (func.MarshalExceptions)
				WriteCatchHandler (writer);

			writer.WriteLine ("}");
			writer.WriteLine ();
		}

		public static class Types {
			public static TypeData Vector2 = new TypeData {
				ManagedType = "Vector2",
				NativeType = "vector_float2",
				NativeWrapperType = "struct Vector2f",
				RequireMarshal = true,
				IsX86Stret = true,
			};
			public static TypeData Vector3 = new TypeData {
				ManagedType = "Vector3",
				NativeType = "vector_float3",
				NativeWrapperType = "struct Vector3f",
				RequireMarshal = true,
			};
			public static TypeData Vector4 = new TypeData {
				ManagedType = "Vector4",
				NativeType = "vector_float4",
				NativeWrapperType = "struct Vector4f",
				RequireMarshal = true,
			};
			public static TypeData Vector3As4 = new TypeData {
				ManagedType = "Vector4",
				NativeType = "vector_float3",
				NativeWrapperType = "struct Vector4f",
				RequireMarshal = true,
			};
			public static TypeData Vector2i = new TypeData {
				ManagedType = "Vector2i",
				NativeType = "vector_int2",
				NativeWrapperType = "struct Vector2i",
				RequireMarshal = true,
				IsX86Stret = true,
			};
			public static TypeData Vector3i = new TypeData {
				ManagedType = "Vector3i",
				NativeType = "vector_int3",
				NativeWrapperType = "struct Vector3i",
				RequireMarshal = true,
			};
			public static TypeData Vector4i = new TypeData {
				ManagedType = "Vector4i",
				NativeType = "vector_int4",
				NativeWrapperType = "struct Vector4i",
				RequireMarshal = true,
			};
			public static TypeData Vector2d = new TypeData {
				ManagedType = "Vector2d",
				NativeType = "vector_double2",
				NativeWrapperType = "struct Vector2d",
				RequireMarshal = true
			};
			public static TypeData Vector3d = new TypeData {
				ManagedType = "Vector3d",
				NativeType = "vector_double3",
				NativeWrapperType = "struct Vector3d",
				RequireMarshal = true,
			};
			public static TypeData Vector4d = new TypeData {
				ManagedType = "Vector4d",
				NativeType = "vector_double4",
				NativeWrapperType = "struct Vector4d",
				RequireMarshal = true,
			};
			public static TypeData Matrix2f = new TypeData {
				ManagedType = "Matrix2",
				NativeType = "matrix_float2x2",
				NativeWrapperType = "struct Matrix2f",
				RequireMarshal = true,
				IsARMStret = true,
				IsX86Stret = true,
				IsX64Stret = false,
			};
			public static TypeData Matrix3f = new TypeData {
				ManagedType = "Matrix3",
				NativeType = "matrix_float3x3",
				NativeWrapperType = "struct Matrix3f",
				RequireMarshal = true,
				IsARMStret = true,
				IsX86Stret = true,
				IsX64Stret = true,
			};
			public static TypeData Matrix4f = new TypeData {
				ManagedType = "Matrix4",
				NativeType = "matrix_float4x4",
				NativeWrapperType = "struct Matrix4f",
				RequireMarshal = true,
				IsARMStret = true,
				IsX86Stret = true,
				IsX64Stret = true,
			};
			public static TypeData Double = new TypeData {
				ManagedType = "Double",
				NativeType = "double",
				NativeWrapperType = "double",
				RequireMarshal = false,
			};
			public static TypeData IntPtr = new TypeData {
				ManagedType = "IntPtr",
				NativeType = "void *",
				NativeWrapperType = "void *",
				RequireMarshal = false,
			};
			public static TypeData Float = new TypeData {
				ManagedType = "float",
				NativeType = "float",
				NativeWrapperType = "float",
				RequireMarshal = false,
			};
			public static TypeData Int32 = new TypeData {
				ManagedType = "int",
				NativeType = "int",
				NativeWrapperType = "int",
				RequireMarshal = false,
			};
			public static TypeData UInt32 = new TypeData {
				ManagedType = "UInt32",
				NativeType = "uint32_t",
				NativeWrapperType = "uint32_t",
				RequireMarshal = false,
			};
			public static TypeData Int64 = new TypeData {
				ManagedType = "Int64",
				NativeType = "int64_t",
				NativeWrapperType = "int64_t",
				RequireMarshal = false,
			};
			public static TypeData UInt64 = new TypeData {
				ManagedType = "UInt64",
				NativeType = "uint64_t",
				NativeWrapperType = "uint64_t",
				RequireMarshal = false,
			};
			public static TypeData NInt = new TypeData {
				ManagedType = "nint",
				NativeType = "xm_nint_t",
				NativeWrapperType = "xm_nint_t",
				RequireMarshal = false,
			};
			public static TypeData NUInt = new TypeData {
				ManagedType = "nuint",
				NativeType = "xm_nuint_t",
				NativeWrapperType = "xm_nuint_t",
				RequireMarshal = false,
			};
			public static TypeData NFloat = new TypeData {
				ManagedType = "nfloat",
				NativeType = "xm_nfloat_t",
				NativeWrapperType = "xm_nfloat_t",
				RequireMarshal = false,
			};
			public static TypeData Bool = new TypeData {
				ManagedType = "bool",
				NativeType = "bool",
				NativeWrapperType = "bool",
				RequireMarshal = false,
			};

			public static TypeData MDLAxisAlignedBoundingBox = new TypeData {
				ManagedType = "MDLAxisAlignedBoundingBox",
				NativeType = "MDLAxisAlignedBoundingBox",
				NativeWrapperType = "MDLAxisAlignedBoundingBoxWrapper",
				RequireMarshal = true,
				IsX86Stret = true,
				IsX64Stret = true,
				IsARMStret = true,
			};

			public static TypeData GKBox = new TypeData {
				ManagedType = "GKBox",
				NativeType = "GKBox",
				NativeWrapperType = "struct GKBoxWrapper",
				RequireMarshal = true,
				IsX86Stret = true,
				IsX64Stret = true,
				IsARMStret = true,
			};

			public static TypeData GKQuad = new TypeData {
				ManagedType = "GKQuad",
				NativeType = "GKQuad",
				NativeWrapperType = "struct GKQuadWrapper",
				RequireMarshal = true,
				IsARMStret = true,
				IsX86Stret = true,
				IsX64Stret = false,
			};

			public static TypeData GKTriangle = new TypeData {
				ManagedType = "GKTriangle",
				NativeType = "GKTriangle",
				NativeWrapperType = "struct GKTriangleWrapper",
				RequireMarshal = true,
				IsARMStret = true,
				IsX86Stret = true,
				IsX64Stret = true,
			};
		}
	}


	enum Variants {
		msgSend = 1,
		msgSend_stret = 2,
		msgSendSuper = 4,
		msgSendSuper_stret = 8,
		All = msgSend | msgSend_stret | msgSendSuper | msgSendSuper_stret,
		NonStret = msgSend | msgSendSuper,
	}

	class TypeData {
		public string ManagedType;
		public string NativeWrapperType;
		public string NativeType;
		public bool RequireMarshal;
		public bool IsX86Stret;
#pragma warning disable 649
		// warning 649:  Field `Xamarin.BindingMethods.Generator.TypeData.IsX64Stret' is never assigned to, and will always have its default value `false'
		// this warning will disappear if we ever get a type that requires stret on these platforms, so just ignore it.
		public bool IsX64Stret;
		public bool IsARMStret;
#pragma warning restore 649
		public bool IsAnyStret { get { return IsX86Stret || IsX64Stret || IsARMStret; } }
		public bool IsNativeType { get { return ManagedType == "nint" || ManagedType == "nuint" || ManagedType == "nfloat"; } }

		public TypeData AsSpecificNativeType (bool as32bit)
		{
			switch (ManagedType) {
			case "nint":
				return as32bit ? MainClass.Types.Int32 : MainClass.Types.Int64;
			case "nuint":
				return as32bit ? MainClass.Types.UInt32 : MainClass.Types.UInt64;
			case "nfloat":
				return as32bit ? MainClass.Types.Float : MainClass.Types.Double;
			default:
				return this;
			}

		}
	}

	class ParameterData {
		public TypeData TypeData;
		public bool IsRef;
	}

	class FunctionData {
		public string Comment;
		public string Prefix = string.Empty;
		// Variants is a [Flags] enum, specifying which of the objc_msgSend variants
		// should be generated. You'll usually use "All", which will generate all 4,
		// or "NonStret", which will just generate objc_msgSend and objc_msgSendSuper.
		public Variants Variants;
		// The return type of the function. Use null for void.
		public TypeData ReturnType;
		// The parameters. Use null for void.
		public ParameterData[] Parameters;
		public bool MarshalExceptions;

		public bool HasNativeType {
			get {
				if (ReturnType != null && ReturnType.IsNativeType)
					return true;
				if (Parameters != null) {
					foreach (var pd in Parameters) {
						if (pd.TypeData.IsNativeType)
							return true;
					}
				}
				return false;
			}
		}

		public FunctionData CloneAsResolvedNativeType (bool as32bit)
		{
			var rv = new FunctionData ();
			rv.Comment = Comment + " cloned for " + (as32bit ? "32bit" : "64bit");
			rv.Prefix = Prefix;
			rv.Variants = Variants;
			if (ReturnType != null)
				rv.ReturnType = ReturnType.AsSpecificNativeType (as32bit);
			if (Parameters != null) {
				rv.Parameters = new ParameterData [Parameters.Length];
				for (int i = 0; i < Parameters.Length; i++) {
					rv.Parameters [i] = new ParameterData ();
					rv.Parameters [i].TypeData = Parameters [i].TypeData.AsSpecificNativeType (as32bit);
					rv.Parameters [i].IsRef = Parameters [i].IsRef;
				}
			}
			rv.MarshalExceptions = MarshalExceptions;
			return rv;
		}
	}

}
