using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

#nullable enable

namespace nnyeah {
	public class MethodTransformations {
		static List<Transformation>? allTransforms;
		static Dictionary<string, Transformation>? transformTable;
		const string ConvertibleMessage = "IConvertible interfaces are not supported yet. If this code gets called, it will fail. Consider contacting the library maintainer to request a dotnet 6 upgrade.";
		const string CopyArrayMessage = "CopyArray for nuint is not supported yet. If this method is called, it will not function correctly.";

		public Dictionary<string, Transformation> GetTransforms (ModuleDefinition module, Func<List<bool>, CustomAttribute> attrBuilder)
		{
			if (transformTable is not null) {
				return transformTable;
			}

			// there are two types of transforms here: ones that can be made statically
			// and ones that need more in the way of state and intermediate variables.

			var singleBool = new List<bool> () { true };

			allTransforms = new List<Transformation> (transforms.Length);
			allTransforms.AddRange (transforms);

			// nint
			var mref = new MethodReference ("CompareTo", module.TypeSystem.Int32, module.TypeSystem.IntPtr);
			var parm = new ParameterDefinition (module.TypeSystem.IntPtr);
			parm.CustomAttributes.Add (attrBuilder (singleBool));
			mref.Parameters.Add (parm);
			allTransforms.Add (new Transformation ("System.Int32 System.nint::CompareTo(System.nint)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("CompareTo", module.TypeSystem.Int32, module.TypeSystem.IntPtr);
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.Object));
			allTransforms.Add (new Transformation ("System.Int32 System.nint::CompareTo(System.Object)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Equals", module.TypeSystem.Boolean, module.TypeSystem.IntPtr);
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.Object));
			allTransforms.Add (new Transformation ("System.Boolean System.nint::Equals(System.Object)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Equals", module.TypeSystem.Boolean, module.TypeSystem.IntPtr);
			parm = new ParameterDefinition (module.TypeSystem.IntPtr);
			parm.CustomAttributes.Add (attrBuilder (singleBool));
			mref.Parameters.Add (parm);
			allTransforms.Add (new Transformation ("System.Boolean System.nint::Equals(System.nint)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("GetHashCode", module.TypeSystem.Int32, module.TypeSystem.IntPtr);
			allTransforms.Add (new Transformation ("System.Int32 System.nint::GetHashCode ()",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("ToString", module.TypeSystem.String, module.TypeSystem.IntPtr);
			allTransforms.Add (new Transformation ("System.String System.nint::ToString()",
				Instruction.Create (OpCodes.Call, mref)));

			var formatProviderTypeRef = module.ImportReference (typeof (IFormatProvider));

			var formatProviderParam = new ParameterDefinition (formatProviderTypeRef);
			mref = new MethodReference ("ToString", module.TypeSystem.String, module.TypeSystem.IntPtr);
			mref.Parameters.Add (formatProviderParam);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.String System.nint::ToString(System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			var stringParam = new ParameterDefinition (module.TypeSystem.String);
			mref = new MethodReference ("ToString", module.TypeSystem.String, module.TypeSystem.IntPtr);
			mref.Parameters.Add (stringParam);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.String System.nint::ToString(System.String)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("ToString", module.TypeSystem.String, module.TypeSystem.IntPtr);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (formatProviderParam);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.String System.nint::ToString(System.String,System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", module.TypeSystem.IntPtr, module.TypeSystem.IntPtr);
			mref.Parameters.Add (stringParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nint System.nint::Parse(System.String)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", module.TypeSystem.IntPtr, module.TypeSystem.IntPtr);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (formatProviderParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nint System.nint::Parse(System.String,System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			var globNumberStylesTypeRef = module.ImportReference (typeof (System.Globalization.NumberStyles));
			var globNumberStylesParam = new ParameterDefinition (globNumberStylesTypeRef);
			mref = new MethodReference ("Parse", module.TypeSystem.IntPtr, module.TypeSystem.IntPtr);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nint System.nint::Parse(System.String,System.Globalization.NumberStyles)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", module.TypeSystem.IntPtr, module.TypeSystem.IntPtr);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.Parameters.Add (formatProviderParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nint System.nint::Parse(System.String,System.Globalization.NumberStyles,System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			var nintRefParam = new ParameterDefinition (module.TypeSystem.IntPtr);
			nintRefParam.CustomAttributes.Add (attrBuilder (singleBool));
			nintRefParam.IsOut = true;
			mref = new MethodReference ("TryParse", module.TypeSystem.Boolean, module.TypeSystem.IntPtr);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (nintRefParam);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Boolean System.nint::TryParse(System.String,System.nint&)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("TryParse", module.TypeSystem.Boolean, module.TypeSystem.IntPtr);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.Parameters.Add (formatProviderParam);
			mref.Parameters.Add (nintRefParam);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Boolean System.nint::TryParse(System.String,System.Globalization.NumberStyles,System.IFormatProvider,System.nint&)",
				Instruction.Create (OpCodes.Call, mref)));

			var decimalTypeReference = new TypeReference ("System", "Decimal", module, module.TypeSystem.CoreLibrary);
			mref = new MethodReference ("op_Implicit", decimalTypeReference, decimalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.Int64));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Decimal System.nint::op_Implicit(System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Conv_I8),
					Instruction.Create (OpCodes.Call, mref) }));

			mref = new MethodReference ("op_Explicit", module.TypeSystem.Int64, decimalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (decimalTypeReference));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nint System.nint::op_Explicit(System.Decimal)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Call, mref),
					Instruction.Create (OpCodes.Conv_I) }));

			var nfloatTypeRef = new TypeReference ("System.Runtime.InteropServices", "NFloat", module, module.TypeSystem.CoreLibrary);
			mref = new MethodReference (".ctor", module.TypeSystem.Void, nfloatTypeRef);
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.Double));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nfloat System.nint::op_Implicit(System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Conv_R8),
					Instruction.Create (OpCodes.Newobj, mref) }));

			var nintVar = new VariableDefinition (module.TypeSystem.Int64);
			var typeCodeRef = new TypeReference ("System", "TypeCode", module, module.TypeSystem.CoreLibrary);
			mref = new MethodReference ("GetTypeCode", typeCodeRef, module.TypeSystem.Int64);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.TypeCode System.nint::GetTypeCode()",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldobj, module.TypeSystem.IntPtr),
					Instruction.Create (OpCodes.Conv_I8),
					Instruction.Create (OpCodes.Stloc, nintVar),
					Instruction.Create (OpCodes.Ldloca, nintVar),
					Instruction.Create (OpCodes.Call, mref)
				}));

			var iconvertibleTypeRef = new TypeReference ("System", "IConvertible", module, module.TypeSystem.CoreLibrary);
			var iformatProviderTypeRef = new TypeReference ("System", "IFormatProvider", module, module.TypeSystem.CoreLibrary);
			var iformatProviderVar = new VariableDefinition (iformatProviderTypeRef);
			mref = new MethodReference ("ToBoolean", module.TypeSystem.Boolean, iconvertibleTypeRef);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Boolean System.nint::System.IConvertible.ToBoolean(System.IFormatProvider)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Stloc, iformatProviderVar),
					Instruction.Create (OpCodes.Unbox, module.TypeSystem.IntPtr),
					Instruction.Create (OpCodes.Conv_I8),
					Instruction.Create (OpCodes.Box, module.TypeSystem.Int64),
					Instruction.Create (OpCodes.Ldloc, iformatProviderVar),
					Instruction.Create (OpCodes.Call, mref)
				}));

			var marshalTypeReference = new TypeReference ("System.Runtime.InteropServices", "Marshal", module, module.TypeSystem.CoreLibrary);
			mref = new MethodReference ("CopyArray", module.TypeSystem.Void, marshalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.IntPtr));
			mref.Parameters.Add (new ParameterDefinition (new ArrayType (module.TypeSystem.IntPtr)));
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.Int32));
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.Int32));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Void System.nint::CopyArray(System.IntPtr,System.nint[],System.Int32,System.Int32)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Call, mref)
				}));

			mref = new MethodReference ("CopyArray", module.TypeSystem.Void, marshalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (new ArrayType (module.TypeSystem.IntPtr)));
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.IntPtr));
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.Int32));
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.Int32));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Void System.nint::CopyArray(System.nint[],System.Int32,System.IntPtr,System.Int32)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Call, mref)
				}));

			//"System.nint System.nint::op_Explicit(System.nfloat)"

			// nuint
			mref = new MethodReference ("CompareTo", module.TypeSystem.Int32, module.TypeSystem.UIntPtr);
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.UIntPtr));
			parm.CustomAttributes.Add (attrBuilder (singleBool));
			allTransforms.Add (new Transformation ("System.Int32 System.nuint::CompareTo(System.nuint)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("CompareTo", module.TypeSystem.Int32, module.TypeSystem.UIntPtr);
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.Object));
			allTransforms.Add (new Transformation ("System.Int32 System.nuint::CompareTo(System.Object)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Equals", module.TypeSystem.Boolean, module.TypeSystem.UIntPtr);
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.Object));
			allTransforms.Add (new Transformation ("System.Boolean System.nuint::Equals(System.Object)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Equals", module.TypeSystem.Boolean, module.TypeSystem.UIntPtr);
			parm = new ParameterDefinition (module.TypeSystem.UIntPtr);
			parm.CustomAttributes.Add (attrBuilder (singleBool));
			mref.Parameters.Add (parm);
			allTransforms.Add (new Transformation ("System.Boolean System.nuint::Equals(System.nuint)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("GetHashCode", module.TypeSystem.Int32, module.TypeSystem.UIntPtr);
			allTransforms.Add (new Transformation ("System.Int32 System.nuint::GetHashCode()",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("ToString", module.TypeSystem.String, module.TypeSystem.UIntPtr);
			allTransforms.Add (new Transformation ("System.String System.nuint::ToString()",
				Instruction.Create (OpCodes.Call, mref)));

			formatProviderParam = new ParameterDefinition (formatProviderTypeRef);
			mref = new MethodReference ("ToString", module.TypeSystem.String, module.TypeSystem.UIntPtr);
			mref.Parameters.Add (formatProviderParam);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.String System.nuint::ToString(System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			stringParam = new ParameterDefinition (module.TypeSystem.String);
			mref = new MethodReference ("ToString", module.TypeSystem.String, module.TypeSystem.UIntPtr);
			mref.Parameters.Add (stringParam);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.String System.nuint::ToString(System.String)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", module.TypeSystem.UIntPtr, module.TypeSystem.UIntPtr);
			mref.Parameters.Add (stringParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nuint System.nuint::Parse(System.String)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", module.TypeSystem.UIntPtr, module.TypeSystem.UIntPtr);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (formatProviderParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nuint System.nuint::Parse(System.String,System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			globNumberStylesTypeRef = module.ImportReference (typeof (System.Globalization.NumberStyles));
			globNumberStylesParam = new ParameterDefinition (globNumberStylesTypeRef);
			mref = new MethodReference ("Parse", module.TypeSystem.UIntPtr, module.TypeSystem.UIntPtr);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nuint System.nuint::Parse(System.String,System.Globalization.NumberStyles)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", module.TypeSystem.UIntPtr, module.TypeSystem.UIntPtr);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.Parameters.Add (formatProviderParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nuint System.nuint::Parse(System.String,System.Globalization.NumberStyles,System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			nintRefParam = new ParameterDefinition (module.TypeSystem.UIntPtr);
			nintRefParam.CustomAttributes.Add (attrBuilder (singleBool));
			nintRefParam.IsOut = true;
			mref = new MethodReference ("TryParse", module.TypeSystem.Boolean, module.TypeSystem.UIntPtr);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (nintRefParam);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Boolean System.nuint::TryParse(System.String,System.nuint&)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("TryParse", module.TypeSystem.Boolean, module.TypeSystem.UIntPtr);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.Parameters.Add (formatProviderParam);
			mref.Parameters.Add (nintRefParam);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Boolean System.nuint::TryParse(System.String,System.Globalization.NumberStyles,System.IFormatProvider,System.nuint&)",
				Instruction.Create (OpCodes.Call, mref)));

			decimalTypeReference = new TypeReference ("System", "Decimal", module, module.TypeSystem.CoreLibrary);
			mref = new MethodReference ("op_Implicit", decimalTypeReference, decimalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.UInt64));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Decimal System.nuint::op_Implicit(System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Conv_U8),
					Instruction.Create (OpCodes.Call, mref) }));

			mref = new MethodReference ("op_Explicit", module.TypeSystem.UInt64, decimalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (decimalTypeReference));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nuint System.nuint::op_Explicit(System.Decimal)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Call, mref),
					Instruction.Create (OpCodes.Conv_U) }));

			nfloatTypeRef = new TypeReference ("System.Runtime.InteropServices", "NFloat", module, module.TypeSystem.CoreLibrary);
			mref = new MethodReference (".ctor", module.TypeSystem.Void, nfloatTypeRef);
			mref.Parameters.Add (new ParameterDefinition (module.TypeSystem.Double));
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nfloat System.nuint::op_Implicit(System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Conv_R8),
					Instruction.Create (OpCodes.Newobj, mref) }));

			var nuintVar = new VariableDefinition (module.TypeSystem.UInt64);
			mref = new MethodReference ("GetTypeCode", typeCodeRef, module.TypeSystem.UInt64);
			mref = module.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.TypeCode System.nuint::GetTypeCode()",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldobj, module.TypeSystem.UIntPtr),
					Instruction.Create (OpCodes.Conv_U8),
					Instruction.Create (OpCodes.Stloc, nuintVar),
					Instruction.Create (OpCodes.Ldloca, nuintVar),
					Instruction.Create (OpCodes.Call, mref)
				}));

			//"System.nuint System.nuint::op_Explicit(System.nfloat)"

			transformTable = new Dictionary<string, Transformation> ();

			foreach (var xform in allTransforms) {
				transformTable.Add (xform.Operand, xform);
			}
			return transformTable;
		}


		static Transformation [] transforms = new Transformation [] {
			new Transformation (
				"System.nuint System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_U)),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.IntPtr)"
				),
			new Transformation(
				"System.IntPtr System.nint::op_Explicit(System.nint)"
				),
			new Transformation(
				"System.nint System.nint::op_Implicit(System.SByte)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation(
				"System.SByte System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_I1)
				),
			new Transformation(
				"System.nint System.nint::op_Implicit(System.Byte)",
				Instruction.Create (OpCodes.Conv_U)
				),
			new Transformation(
				"System.Byte System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_U1)
				),
			new Transformation(
				"System.nint System.nint::op_Implicit(System.Char)",
				Instruction.Create (OpCodes.Conv_U)
				),
			new Transformation(
				"System.Char System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_U2)
				),
			new Transformation(
				"System.nint System.nint::op_Implicit(System.Int16)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation(
				"System.Int16 System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_I2)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.UInt16)",
				Instruction.Create (OpCodes.Conv_U)
				),
			new Transformation(
				"System.UInt16 System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_U2)
				),
			new Transformation(
				"System.nint System.nint::op_Implicit(System.Int32)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation(
				"System.Int32 System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_I4)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.UInt32)",
				Instruction.Create (OpCodes.Conv_U)
				),
			new Transformation(
				"System.UInt32 System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_U4)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.Int64)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation(
				"System.Int64 System.nint::op_Implicit(System.nint)",
				Instruction.Create (OpCodes.Conv_I8)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.UInt64)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation(
				"System.UInt64 System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_I8)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.Single)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation (
				"System.Single System.nint::op_Implicit(System.nint)",
				Instruction.Create (OpCodes.Conv_R4)
				),
			new Transformation (
				"System.nint System.nint::op_Explicit(System.Double)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation (
				"System.Double System.nint::op_Implicit(System.nint)",
				Instruction.Create (OpCodes.Conv_R8)
				),
			new Transformation (
				"System.nint System.nint::op_UnaryPlus(System.nint)"
				),
			new Transformation (
				"System.nint System.nint::op_UnaryNegation(System.nint)",
				Instruction.Create (OpCodes.Neg)
				),
			new Transformation(
				"System.nint System.nint::op_OnesComplement(System.nint)",
				Instruction.Create (OpCodes.Not)
				),
			new Transformation(
				"System.nint System.nint::op_Increment(System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldc_I4_1),
					Instruction.Create (OpCodes.Add),
				}
				),
			new Transformation(
				"System.nint System.nint::op_Decrement(System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldc_I4_1),
					Instruction.Create (OpCodes.Sub),
				}
				),
			new Transformation(
				"System.nint System.nint::op_Addition(System.nint,System.nint)",
				Instruction.Create (OpCodes.Add)
				),
			new Transformation(
				"System.nint System.nint::op_Subtraction(System.nint,System.nint)",
				Instruction.Create (OpCodes.Sub)
				),
			new Transformation(
				"System.nint System.nint::op_Multiply(System.nint,System.nint)",
				Instruction.Create (OpCodes.Mul)
				),
			new Transformation(
				"System.nint System.nint::op_Division(System.nint,System.nint)",
				Instruction.Create (OpCodes.Div)
				),
			new Transformation(
				"System.nint System.nint::op_Modulus(System.nint,System.nint)",
				Instruction.Create (OpCodes.Rem)
				),
			new Transformation(
				"System.nint System.nint::op_BitwiseAnd(System.nint,System.nint)",
				Instruction.Create (OpCodes.And)
				),
			new Transformation(
				"System.nint System.nint::op_BitwiseOr(System.nint,System.nint)",
				Instruction.Create (OpCodes.Or)
				),
			new Transformation(
				"System.nint System.nint::op_ExclusiveOr(System.nint,System.nint)",
				Instruction.Create (OpCodes.Xor)
				),
			new Transformation(
				"System.nint System.nint::op_LeftShift(System.nint,System.Int32)",
				Instruction.Create (OpCodes.Shl)
				),
			new Transformation(
				"System.nint System.nint::op_RightShift(System.nint,System.Int32)",
				Instruction.Create (OpCodes.Shr)
				),
			new Transformation(
				"System.Boolean System.nint::op_Equality(System.nint,System.nint)",
				Instruction.Create (OpCodes.Ceq)
				),
			new Transformation(
				"System.Boolean System.nint::op_Inequality(System.nint,System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ceq),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation(
				"System.Boolean System.nint::op_LessThan(System.nint,System.nint)",
				Instruction.Create (OpCodes.Clt)
				),
			new Transformation(
				"System.Boolean System.nint::op_GreaterThan(System.nint,System.nint)",
				Instruction.Create (OpCodes.Cgt)
				),
			new Transformation(
				"System.Boolean System.nint::op_LessThanOrEqual(System.nint,System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Cgt),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation(
				"System.Boolean System.nint::op_GreaterThanOrEqual(System.nint,System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Clt),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation(
				"System.Void System.nint::.ctor(System.nint)"
				),
			new Transformation(
				"System.Void System.nint::.ctor(System.Int32)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.Void System.nint::.ctor(System.Int64)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.Void System.nint::.cctor()"
				),

			new Transformation ("System.Boolean System.nint::System.IConvertible.ToBoolean(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Byte System.nint::System.IConvertible.ToByte(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Char System.nint::System.IConvertible.ToChar(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.DateTime System.nint::System.IConvertible.ToDateTime(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Decimal System.nint::System.IConvertible.ToDecimal(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Double System.nint::System.IConvertible.ToDouble(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Int16 System.nint::System.IConvertible.ToInt16(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Int32 System.nint::System.IConvertible.ToInt32(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Int64 System.nint::System.IConvertible.ToInt64(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.SByte System.nint::System.IConvertible.ToSByte(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Single System.nint::System.IConvertible.ToSingle(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.UInt16 System.nint::System.IConvertible.ToUInt16(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.UInt32 System.nint::System.IConvertible.ToUInt32(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.UInt64 System.nint::System.IConvertible.ToUInt64(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Object System.nint::System.IConvertible.ToType(System.Type,System.IFormatProvider)", ConvertibleMessage),

			// nuint
			new Transformation(
				"System.Void System.nuint::.ctor(System.UInt32)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Void System.nuint::.ctor(System.UInt64)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Void System.nuint::.ctor(System.nuint)"
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.IntPtr)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.IntPtr System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.UIntPtr)"
				),
			new Transformation(
				"System.UIntPtr System.nuint::op_Explicit(System.nuint)"
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.SByte)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.SByte System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_I1)
				),
			new Transformation(
				"System.nuint System.nuint::op_Implicit(System.Byte)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Byte System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U1)
				),
			new Transformation(
				"System.nuint System.nuint::op_Implicit(System.Char)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Char System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U2)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.Int16)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.Int16 System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_I2)
				),
			new Transformation(
				"System.nuint System.nuint::op_Implicit(System.UInt16)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.UInt16 System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U2)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.Int32)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.Int32 System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_I4)
				),
			new Transformation(
				"System.nuint System.nuint::op_Implicit(System.UInt32)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.UInt32 System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U4)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.Int64)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Int64 System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U8)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.UInt64)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.UInt64 System.nuint::op_Implicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U8)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.Single)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Single System.nuint::op_Implicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_R4)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.Double)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Double System.nuint::op_Implicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_R8)
				),
			new Transformation(
				"System.nuint System.nuint::op_UnaryPlus(System.nuint)"
				),
			new Transformation(
				"System.nuint System.nuint::op_OnesComplement(System.nuint)",
				Instruction.Create(OpCodes.Not)
				),
			new Transformation(
				"System.nuint System.nuint::op_Increment(System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldc_I4_1),
					Instruction.Create (OpCodes.Add)
				}
				),
			new Transformation(
				"System.nuint System.nuint::op_Decrement(System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldc_I4_1),
					Instruction.Create (OpCodes.Sub)
				}
				),
			new Transformation(
				"System.nuint System.nuint::op_Addition(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Add)
				),
			new Transformation(
				"System.nuint System.nuint::op_Subtraction(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Sub)
				),
			new Transformation(
				"System.nuint System.nuint::op_Multiply(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Mul)
				),
			new Transformation(
				"System.nuint System.nuint::op_Division(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Div_Un)
				),
			new Transformation(
				"System.nuint System.nuint::op_Modulus(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Rem_Un)
				),
			new Transformation(
				"System.nuint System.nuint::op_BitwiseAnd(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.And)
				),
			new Transformation(
				"System.nuint System.nuint::op_BitwiseOr(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Or)
				),
			new Transformation(
				"System.nuint System.nuint::op_ExclusiveOr(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Xor)
				),
			new Transformation(
				"System.nuint System.nuint::op_LeftShift(System.nuint,System.Int32)",
				Instruction.Create (OpCodes.Shl)
				),
			new Transformation (
				"System.nuint System.nuint::op_RightShift(System.nuint,System.Int32)",
				Instruction.Create (OpCodes.Shr)
				),
			new Transformation(
				"System.Boolean System.nuint::op_Equality(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Ceq)
				),
			new Transformation(
				"System.Boolean System.nuint::op_Inequality(System.nuint,System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ceq),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation(
				"System.Boolean System.nuint::op_LessThan(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Clt_Un)
				),
			new Transformation(
				"System.Boolean System.nuint::op_GreaterThan(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Cgt_Un)
				),
			new Transformation(
				"System.Boolean System.nuint::op_LessThanOrEqual(System.nuint,System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Cgt_Un),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation(
				"System.Boolean System.nuint::op_GreaterThanOrEqual(System.nuint,System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Clt_Un),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation (
				"System.Void System.nuint::.cctor()"
				),

			new Transformation ("System.Boolean System.nuint::System.IConvertible.ToBoolean(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Byte System.nuint::System.IConvertible.ToByte(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Char System.nuint::System.IConvertible.ToChar(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.DateTime System.nuint::System.IConvertible.ToDateTime(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Decimal System.nuint::System.IConvertible.ToDecimal(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Double System.nuint::System.IConvertible.ToDouble(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Int16 System.nuint::System.IConvertible.ToInt16(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Int32 System.nuint::System.IConvertible.ToInt32(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Int64 System.nuint::System.IConvertible.ToInt64(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.SByte System.nuint::System.IConvertible.ToSByte(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Single System.nuint::System.IConvertible.ToSingle(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.UInt16 System.nuint::System.IConvertible.ToUInt16(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.UInt32 System.nuint::System.IConvertible.ToUInt32(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.UInt64 System.nuint::System.IConvertible.ToUInt64(System.IFormatProvider)", ConvertibleMessage),
			new Transformation ("System.Object System.nuint::System.IConvertible.ToType(System.Type,System.IFormatProvider)", ConvertibleMessage),

			// why warnings for this?
			// Because in .NET 6 there is no direct equivalent of this call.
			// In order to support this, we need to inject helper methods that do the actual
			// copying and revector to those helpers.
			// Not doing this right now since a survey of nuget.org shows that these methods
			// are never called in the wild.
			new Transformation ("System.Void System.nuint::CopyArray(System.IntPtr,System.nuint[],System.Int32,System.Int32)",
				CopyArrayMessage),
			new Transformation ("System.Void System.nuint::CopyArray(System.nuint[],System.Int32,System.IntPtr,System.Int32)",
				CopyArrayMessage),
		};
	}
}
