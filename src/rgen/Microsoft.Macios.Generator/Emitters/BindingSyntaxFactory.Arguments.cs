// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.DataModel;
using Microsoft.Macios.Generator.Formatters;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.Macios.Generator.Nomenclator;
using TypeInfo = Microsoft.Macios.Generator.DataModel.TypeInfo;

namespace Microsoft.Macios.Generator.Emitters;

static partial class BindingSyntaxFactory {

	/// <summary>
	/// Generates an initialization statement for a 'byref' parameter that is passed as a pointer.
	/// This is used to initialize 'out' parameters to their default value before a native call.
	/// It generates the following code: `*parameterName = default;`
	/// </summary>
	/// <param name="parameterName">The name of the pointer parameter.</param>
	/// <returns>An immutable array containing a single <see cref="ExpressionStatementSyntax"/> for the initialization.</returns>
	internal static ImmutableArray<SyntaxNode> GetNativeInitializationByRefArgument (string parameterName)
	{
		// create the pointer variable and assign it to its default value
		// generates the following:
		// *{ParameterName} = default;
		var expr = ExpressionStatement (
			AssignmentExpression (
				SyntaxKind.SimpleAssignmentExpression,
				IdentifierName (parameterName),
				LiteralExpression (
					SyntaxKind.DefaultLiteralExpression,
					Token (SyntaxKind.DefaultKeyword)))).NormalizeWhitespace ();
		return [expr];
	}

	/// <summary>
	/// Generates a list of syntax nodes representing the necessary initializations for an argument before a native invocation.
	/// This method is used to set up variables that will be passed by reference to native code.
	/// For 'byref' parameters, it generates initialization code. For other parameters, it does nothing.
	/// </summary>
	/// <param name="argumentInfo">The information about the argument for which to generate initializations.</param>
	/// <returns>An immutable array of <see cref="SyntaxNode"/> representing the initialization statements.</returns>
	internal static ImmutableArray<SyntaxNode> GetNativeInvokeArgumentInitializations (in ArgumentInfo argumentInfo)
	{
		// decide the type of conversion we need to do based on the type of the parameter
#pragma warning disable format
		return argumentInfo switch { 
			{ IsByRef: true, ReferenceKind: ReferenceKind.Out, Type.IsReferenceType: false, Type.SpecialType: not SpecialType.System_Boolean} 
				=> GetNativeInitializationByRefArgument (argumentInfo.Name),
			_ => []
		};
#pragma warning restore format
	}

	internal static ImmutableArray<SyntaxNode> GetNativeInvokeArgumentValidations (in ArgumentInfo argumentInfo)
	{
		// if the parameter does not allow the object to be null and it is a reference type, we need to add the null check
		// otherwise ignore it. We do not want this check for INativeObjects (includes NSObject) because the GetNonNullableHandle
		// will throw an exception if the object is null.
		if (argumentInfo.Type is { IsReferenceType: true, IsINativeObject: false, IsNullable: false } && !argumentInfo.IsByRef) {
			return [ThrowIfNull (argumentInfo.Name)];
		}
		return ImmutableArray<SyntaxNode>.Empty;
	}

	/// <summary>
	/// Generates a list of syntax nodes representing the necessary conversions or cleanup for an argument after a native invocation.
	/// This method is primarily used for resource management after a native call has completed. It generates statements that:
	/// - Keep managed objects alive to prevent premature garbage collection if their native counterparts are still in use.
	///   This applies to smart enums, arrays of native objects, protocols, and other INativeObject-derived types.
	/// - Release native resources, such as releasing an NSString that was created from a C# string.
	/// </summary>
	/// <param name="parameter">The information about the argument for which to generate post-invoke conversions.</param>
	/// <returns>An immutable array of <see cref="SyntaxNode"/> representing the post-invocation statements.</returns>
	internal static ImmutableArray<SyntaxNode> GetPostNativeInvokeArgumentConversions (in ArgumentInfo parameter)
	{
		// decide the type of conversion we need to do based on the type of the parameter
#pragma warning disable format
		return parameter switch { 
			// String handle to string
			{ IsByRef: true, Type.IsArray: false, Type.IsNullable: true, Type.SpecialType: SpecialType.System_String }
				=> [
					// set the string from the aux pointer
					VariableAssignment (
						variableName: parameter.Name, 
						value: StringFromHandle (
							[
								Argument (IdentifierName (GetNameForVariableType (parameter.Name, VariableType.NSString)!)), 
								BoolArgument (false)
							])
					),
				],
			
			// string handle to string not null
			{ IsByRef: true, Type.IsArray: false, Type.IsNullable: false, Type.SpecialType: SpecialType.System_String }
				=> [
					// set the string from the aux pointer
					VariableAssignment (
						variableName: parameter.Name, 
						value: SuppressNullableWarning (StringFromHandle (
						[
							Argument (IdentifierName (GetNameForVariableType (parameter.Name, VariableType.NSString)!)), 
							BoolArgument (false)
						]))
					),
				],
			
			// nothing to do for enums
			{ IsByRef: true, Type.IsEnum: true, Type.IsSmartEnum: false} => [],
			
			// nothing other types that are not objects
			{ IsByRef: true, Type.SpecialType: not SpecialType.None } => [], 
			
			// nothing to do with pointers
			{ IsByRef: true, Type.IsStruct: true} => [],
			
			{ IsByRef: true, Type.IsArray: false, Type.IsINativeObject: true, Type.IsNSObject: false}
				=> [
					// set the nsobject from the aux pointer
					VariableAssignment (
						variableName: parameter.Name,
						value: GetINativeObject (
							nsObjectType: parameter.Type.WithNullable (isNullable: false).GetIdentifierSyntax (), 
							args: [
								Argument (IdentifierName (GetNameForVariableType (parameter.Name, VariableType.Handle)!)),
							], 
							suppressNullableWarning: !parameter.Type.IsNullable
						)
					),
				],
			
			// nsobject or inative object need a native handle to pass to the native code by ref
			{ IsByRef: true, Type.IsArray: false, Type.IsNSObject: true }
				=> [
					// set the nsobject from the aux pointer
					VariableAssignment (
						variableName: parameter.Name,
						value: GetNSObject (
							nsObjectType: parameter.Type.WithNullable (isNullable: false).GetIdentifierSyntax (), 
							args: [
								Argument (IdentifierName (GetNameForVariableType (parameter.Name, VariableType.Handle)!)),
							],
							suppressNullableWarning: !parameter.Type.IsNullable)
					)
				],
			
			// get the string array that is nullable
			{ IsByRef: true, Type.IsArray: true, Type.IsNullable: true, Type.ArrayElementType: SpecialType.System_String }
				=> [
					VariableAssignment (
						variableName: parameter.Name,
						value: StringArrayFromHandle ([
							Argument (IdentifierName (GetNameForVariableType (parameter.Name, VariableType.NSArray)!)),
							BoolArgument (false)
						])
					),
				],
			
			// get non-nullable string array
			{ IsByRef: true, Type.IsArray: true, Type.IsNullable: false, Type.ArrayElementType: SpecialType.System_String }
				=> [
					VariableAssignment (
						variableName: parameter.Name,
						value: SuppressNullableWarning (StringArrayFromHandle ([
							Argument (IdentifierName (GetNameForVariableType (parameter.Name, VariableType.NSArray)!)),
							BoolArgument (false)
						]))
					),
				],
			
			// by ref we need to create a native handle for the NSArray 
			{ IsByRef: true, Type.IsArray: true, Type.ArrayElementTypeIsWrapped: true }
				=> [
					VariableAssignment (
 						variableName: parameter.Name,
					    value: GetCFArrayFromHandle (
						    nsObjectType: parameter.Type.ToArrayElementType ().WithNullable (isNullable: false).GetIdentifierSyntax (), 
						    args: [ 
							    Argument (IdentifierName(GetNameForVariableType (parameter.Name, VariableType.NSArray)!)) 
						    ], 
						    suppressNullableWarning: !parameter.Type.IsNullable) 
					)
				],
			
			// BindAs
			{ BindAs: not null } => [
				ExpressionStatement (
					KeepAlive (
						GetNameForVariableType (
							parameter.Name, 
							parameter.BindAs.Value.Type.IsArray 
								? VariableType.NSArray : VariableType.BindFrom)! 
				))],
			
			{ Type.IsPointer: true } => [],
			
			{ Type.IsDelegate: true } => [],
			
			// ensure that the gc does not collect the smart NSString value
			{ Type.IsSmartEnum: true} =>  [ExpressionStatement (
				KeepAlive (
					// use the nomenclator to get the name for the variable type
					GetNameForVariableType (parameter.Name, VariableType.BindFrom)!
				))],

			// boolean, nothing to do
			{ Type.SpecialType: SpecialType.System_Boolean } => [],
			
			// ensure that the gc does not collect the NSArray value
			{ Type.IsArray: true, Type.ArrayElementType: SpecialType.System_String } => [ExpressionStatement (
				KeepAlive (
					//  use the nomenclator to get the name for the variable type
					GetNameForVariableType (parameter.Name, VariableType.NSArray)!
				))],

			{ Type.IsArray: true, Type.ArrayElementIsINativeObject: true } => [ExpressionStatement (
				KeepAlive (
					//  use the nomenclator to get the name for the variable type
					GetNameForVariableType (parameter.Name, VariableType.NSArray)!
				))],

			{ Type.SpecialType: SpecialType.System_String } =>  [ExpressionStatement (
					StringReleaseNative(
						[Argument(IdentifierName(
							//  use the nomenclator to get the name for the variable type
							GetNameForVariableType (parameter.Name, VariableType.NSString)!
						))
						]))],

			{ Type.IsProtocol: true } => [ExpressionStatement (KeepAlive (parameter.Name))],

			// special types

			// CoreMedia.CMSampleBuffer
			{ Type.FullyQualifiedName: "CoreMedia.CMSampleBuffer" } => [ExpressionStatement (KeepAlive (parameter.Name))],

			// AudioToolbox.AudioBuffers
			{ Type.FullyQualifiedName: "AudioToolbox.AudioBuffers" } => [ExpressionStatement (KeepAlive (parameter.Name))],

			// general NSObject/INativeObject, has to be after the special types otherwise the special types will
			// fall into the NSObject/INativeObject case

			// same name, native handle
			{ Type.IsNSObject: true } => [ExpressionStatement (KeepAlive (parameter.Name))],

			// same name, native handle
			{ Type.IsINativeObject: true } => [ExpressionStatement (KeepAlive (parameter.Name))],
			
			// by default, we will use the parameter name as is and the type of the parameter
			_ => [],
		};
#pragma warning restore format
	}

	/// <summary>
	/// Generates a list of syntax nodes representing the necessary conversions for an argument before a native invocation.
	/// This method prepares managed types for a native call by generating statements that:
	/// - Perform null checks for non-nullable reference types.
	/// - Create auxiliary variables for native representations, such as:
	///   - Native blocks from C# delegates.
	///   - NSStrings from C# strings or smart enums.
	///   - NSArrays from C# arrays.
	///   - Native handles (IntPtr) from INativeObject instances.
	/// </summary>
	/// <param name="argumentInfo">The information about the argument for which to generate pre-invoke conversions.</param>
	/// <returns>An immutable array of <see cref="SyntaxNode"/> representing the conversion statements.</returns>
	internal static ImmutableArray<SyntaxNode> GetPreNativeInvokeArgumentConversions (in ArgumentInfo argumentInfo)
	{
		// based on the trampoline name and the parameter we will lower the parameter to the expected type for the invoker
		// which is the lower type of the parameter
#pragma warning disable format
		ImmutableArray<SyntaxNode> conversions = argumentInfo switch {
			// by ref we need to create a native handle for the NSString
			{ IsByRef: true, Type.IsArray: false, Type.SpecialType: SpecialType.System_String }
				=> [
					VariableInitialization (
						variableName: GetNameForVariableType (argumentInfo.Name, VariableType.NSString)!, 
						type: NativeHandle)
				],
			
			// enums that are not smart enums are going to be passed as pointers
			{ IsByRef: true, Type.IsEnum: true, Type.IsSmartEnum: false} => [],
			
			// other types that are not objects
			{ IsByRef: true, Type.SpecialType: not SpecialType.None } => [], 

			// structs just need the pointer
			{ IsByRef: true, Type.IsStruct: true} => [],
			
			{ IsByRef: true, Type.IsArray: false, Type.IsINativeObject: true, Type.IsNSObject: false }
				=> [
					VariableInitialization (
						variableName: GetNameForVariableType (argumentInfo.Name, VariableType.Handle)!, 
						type: NativeHandle)
				],
			
			// nsobject or interface need a native handle to pass to the native code by ref
			{ IsByRef: true, Type.IsArray: false, Type.IsWrapped: true}
				=> [
					VariableInitialization (
						variableName: GetNameForVariableType (argumentInfo.Name, VariableType.Handle)!, 
						type: NativeHandle)
				],
			
			// by ref we need to create a native handle for the NSArray 
			{ IsByRef: true, Type.IsArray: true, Type.ArrayElementType: SpecialType.System_String }
				=> [
					VariableInitialization (
						variableName: GetNameForVariableType (argumentInfo.Name, VariableType.NSArray)!, 
						type: NativeHandle)
				],
			
			// by ref we need to create a native handle for the NSArray 
			{ IsByRef: true, Type.IsArray: true, Type.ArrayElementIsINativeObject: true }
				=> [
					VariableInitialization (
						variableName: GetNameForVariableType (argumentInfo.Name, VariableType.NSArray)!, 
						type: NativeHandle)
				],
			
			// pointer parameter 
			{ Type.IsPointer: true } => [],
			
			// block delegate parameter is a NativeHandle
			{ Type.IsDelegate: true, IsBlockCallback: true} => [
				GetNullableBlockAuxVariable (argumentInfo)
					.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space)),
				GetBlockLiteralAuxVariable (argumentInfo),
			],
			
			{ Type.IsDelegate: true, IsCCallback: true} => [],
			
			// This is the case when the delegate has not been decorated with a BlockCallback or CCallback attribute.
			// this is the default behaviour with properties and methods. In that case we assume we are dealing with
			// a block callback
			{ Type.IsDelegate: true, IsBlockCallback: false, IsCCallback: false } => [
				GetNullableBlockAuxVariable (argumentInfo)
					.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space)),
				GetBlockLiteralAuxVariable (argumentInfo),
			],
			
			// return the conversion expression to the native type
			{ Type.IsSmartEnum: true} => [
				GetNSStringSmartEnumAuxVariable (argumentInfo)!
					.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space)) 
			],

			// boolean, convert it to byte
			{ Type.SpecialType: SpecialType.System_Boolean } => [],
			
			{ Type.IsArray: true, Type.ArrayElementType: SpecialType.System_String } => [
				GetNSArrayAuxVariable (argumentInfo)!
					.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space)) 
			],

			{ Type.IsArray: true, Type.ArrayElementIsINativeObject: true } => [
				GetNSArrayAuxVariable (argumentInfo)!
					.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space)) 
			],
			
			{ Type.SpecialType: SpecialType.System_String } =>  [
				// CFString.CreateNative returns a NativeHandle, which doesn't work with using.
				GetStringAuxVariable (argumentInfo)!
			],

			{ Type.IsProtocol: true } => [GetHandleAuxVariable (argumentInfo)!],

			// special types

			// CoreMedia.CMSampleBuffer
			{ Type.FullyQualifiedName: "CoreMedia.CMSampleBuffer" } => [GetHandleAuxVariable (argumentInfo)!],

			// AudioToolbox.AudioBuffers
			{ Type.FullyQualifiedName: "AudioToolbox.AudioBuffers" } => [GetHandleAuxVariable (argumentInfo)!],

			// general NSObject/INativeObject, has to be after the special types otherwise the special types will
			// fall into the NSObject/INativeObject case

			// same name, native handle
			{ Type.IsNSObject: true } => [GetHandleAuxVariable (argumentInfo)!],

			// same name, native handle
			{ Type.IsINativeObject: true } => [GetHandleAuxVariable (argumentInfo)!],
			
			// by default, we will use the parameter name as is and the type of the parameter
			_ => [],
		};
#pragma warning restore format
		
		// special case when we are dealing with a BindAs attribute, we need to convert the NSValue/NSNumber/NSString
		if (argumentInfo.BindAs is not null) {
			
#pragma warning disable format
			// we need to convert the NSValue/NSNumber/NSString to the expected type
			conversions = (BindAs: argumentInfo.BindAs.Value.Type, Type: argumentInfo.Type) switch {
				{ BindAs.FullyQualifiedName: "Foundation.NSValue", Type.IsArray: false } =>
					[
						GetNSValueAuxVariable (argumentInfo)!
							.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space)), 
						GetHandleAuxVariable (GetNameForVariableType (argumentInfo.Name, VariableType.BindFrom)!, argumentInfo.BindAs.Value.Type)!
					],
				{ BindAs.FullyQualifiedName: "Foundation.NSNumber", Type.IsArray: false } =>
					[
						GetNSNumberAuxVariable (argumentInfo)! 
							.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space)), 
						GetHandleAuxVariable (GetNameForVariableType (argumentInfo.Name, VariableType.BindFrom)!, argumentInfo.BindAs.Value.Type)!
					],
				{ BindAs.FullyQualifiedName: "Foundation.NSString", Type.IsArray: false } =>
					[
						GetNSStringSmartEnumAuxVariable (argumentInfo)!
							.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space)), 
						GetHandleAuxVariable (GetNameForVariableType (argumentInfo.Name, VariableType.BindFrom)!, argumentInfo.BindAs.Value.Type)!
					],
				// array support: NSArray.ArrayFromHandleFunc<parameterType> (parameterName, NSValue.FromHandle, false)!
				{ BindAs.FullyQualifiedName: "Foundation.NSValue", Type.IsArray: true } =>
					[
						GetNSArrayAuxVariable (argumentInfo)!
							.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space)), 
						GetHandleAuxVariable (GetNameForVariableType (argumentInfo.Name, VariableType.BindFrom)!, argumentInfo.BindAs.Value.Type)!
					],
				{ BindAs.FullyQualifiedName: "Foundation.NSNumber", Type.IsArray: true } =>
					[
						GetNSArrayAuxVariable (argumentInfo)!
							.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space)), 
						GetHandleAuxVariable (GetNameForVariableType (argumentInfo.Name, VariableType.BindFrom)!, argumentInfo.BindAs.Value.Type)!
					],
				{ BindAs.FullyQualifiedName: "Foundation.NSString", Type.IsArray: true } =>
					[
						GetNSArrayAuxVariable (argumentInfo)!
							.WithUsingKeyword (Token (SyntaxKind.UsingKeyword).WithTrailingTrivia (Space)), 
						GetHandleAuxVariable (GetNameForVariableType (argumentInfo.Name, VariableType.BindFrom)!, argumentInfo.BindAs.Value.Type)!
					],
				_ => conversions
			};
#pragma warning restore format
		}
		// should contain any null check and the required conversions to the native type
		return conversions;
	}

	/// <summary>
	/// Generates an <see cref="ArgumentSyntax"/> for a parameter in a native invocation.
	/// This method constructs the appropriate expression to pass a managed type as an argument to a native function,
	/// handling various conversions and special cases based on the argument's type and attributes.
	/// It handles:
	/// - 'ref' parameters by converting them to pointers.
	/// - Delegates (blocks and C-style callbacks) by using their native block literal representation.
	/// - Native enums by casting them to their underlying native type.
	/// - Booleans by converting them to bytes.
	/// - Arrays of various element types by using their corresponding native array handle (e.g., NSArray).
	/// - Strings by using their native string handle (e.g., NSString).
	/// - Protocols and other INativeObject-derived types by using their handle.
	/// - Special types like CMSampleBuffer and AudioBuffers by using their handle.
	/// - Parameters with a [BindAs] attribute by converting them to the specified intermediate type (e.g., NSValue, NSNumber, NSString).
	/// </summary>
	/// <param name="argumentInfo">The information about the argument for which to generate the syntax.</param>
	/// <returns>An <see cref="ArgumentSyntax"/> representing the argument for the native invocation.</returns>
	internal static ArgumentSyntax GetNativeInvokeArgument (in ArgumentInfo argumentInfo)
	{
		// build the necessary expression based on the information of the parameter and its type, taking into account
		// that the type of the parameter might be different from the type specified in the BindAs attribute.
		TypeInfo parameterType = argumentInfo.BindAs?.Type ?? argumentInfo.Type;
		var parameterIdentifier = IdentifierName (argumentInfo.Name);
#pragma warning disable format
		var expression = (Type: parameterType, Parameter: argumentInfo ) switch {
			
			// if it is by ref AND a string, using the handle of the nsstring aux variable
			{ Parameter.IsByRef: true, Type.IsArray: false, Type.SpecialType: SpecialType.System_String }
				=> PrefixUnaryExpression (
					SyntaxKind.AddressOfExpression, 
					IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.NSString)!)), 
			
			// enums that are not smart enums are going to be passed as pointers
			{ Parameter.IsByRef: true, Type.IsEnum: true, Type.IsSmartEnum: false} 
				=> AsPointer (parameterType, [ArgumentForParameter (argumentInfo.Name, ReferenceKind.Ref)]),
			
			// ref parameters have to be converted to a pointer in they are not a wrapped type, int, byte etc..
			{ Parameter.IsByRef: true, Type.SpecialType: not SpecialType.None } 
				=> AsPointer (parameterType, [ArgumentForParameter (argumentInfo.Name, ReferenceKind.Ref)]),
			
			// structs are passed as pointers
			// ref parameters have to be converted to a pointer in they are not a wrapped type, int, byte etc..
			{ Parameter.IsByRef: true, Type.IsStruct: true} 
				=> AsPointer (parameterType, [ArgumentForParameter (argumentInfo.Name, ReferenceKind.Ref)]),
			
			// ref parameters that are wrapped types, we need to use the native handle of the wrapped type
			{ Parameter.IsByRef: true, Type.IsArray: false, Type.IsWrapped: true } 
				=> PrefixUnaryExpression (
					SyntaxKind.AddressOfExpression, 
					IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.Handle)!)), 
			
			// deal with arrays of string
			{ Parameter.IsByRef: true, Type.IsArray: true, Type.ArrayElementType: SpecialType.System_String }
				=> PrefixUnaryExpression ( 
					SyntaxKind.AddressOfExpression, 
					IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.NSArray)!)), 
			
			// deal with arrays of INativeObjects
			{ Parameter.IsByRef: true, Type.IsArray: true, Type.ArrayElementIsINativeObject: true }
				=> PrefixUnaryExpression ( 
					SyntaxKind.AddressOfExpression, 
			IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.NSArray)!)), 
			
			// delegate parameter, c callback
			// System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<ParameterType> (ParameterName)
			{ Type.IsDelegate: true, Parameter.IsCCallback: true } 
				=> CastExpression (
					NativeHandle, 
					IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.BlockLiteral)!).WithLeadingTrivia (Space)),
			
			// delegate parameter, block callback
			// TrampolineNativeInvocationClass.Create (ParameterName)!
			{ Type.IsDelegate: true, Parameter.IsBlockCallback: true }
				=> CastExpression(
					NativeHandle, 
					IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.BlockLiteral)!).WithLeadingTrivia (Space)),
			
			// this happens when the parameter is not decorated. This is the default behaviour with properties and methods
			// if that is the case, we always assume we are dealing with a block callback
			{ Type.IsDelegate: true, Parameter.IsBlockCallback: false, Parameter.IsCCallback: false }
				=> CastExpression(
					NativeHandle, 
					IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.BlockLiteral)!).WithLeadingTrivia (Space)),
    
			// smart enums must use the aux variable
			{ Type.IsSmartEnum: true} 
				=> GetHandleMember (IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.BindFrom)!)),
			
			// native enum, return the conversion expression to the native type
			{ Type.IsNativeEnum: true} 
				=> CastNativeToEnum (IdentifierName (argumentInfo.Name), argumentInfo.Type)!,
			
			// boolean, convert it to byte
			{ Type.SpecialType: SpecialType.System_Boolean } 
				=> CastToByte (argumentInfo.Name, parameterType)!,
			
			// array types
			
			// use the native handle of the array
			{ Type.IsArray: true, Type.ArrayElementTypeIsWrapped: true } =>
				GetHandleMember (IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.NSArray)!)),
			
			// NSArray.ArrayFromHandle<{0}> ({1})!
			{ Type.IsArray: true, Type.ArrayElementIsINativeObject: true } =>
				GetHandleMember (IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.NSArray)!)),
			
			// string[]
			// CFArray.StringArrayFromHandle (ParameterName)!
			{ Type.IsArray: true, Type.ArrayElementType: SpecialType.System_String } =>
				GetHandleMember (IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.NSArray)!)),
			
			// string
			// CFString.FromHandle (ParameterName)!
			{ Type.SpecialType: SpecialType.System_String } =>
				IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.NSString)!),
			
			// Runtime.GetINativeObject<ParameterType> (ParameterName, false)!
			{ Type.IsProtocol: true } => 
				IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.Handle)!),
			
			// special types
			
			{ Type.FullyQualifiedName: "CoreMedia.CMSampleBuffer" } =>
				IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.Handle)!),
			
			{ Type.FullyQualifiedName: "AudioToolbox.AudioBuffers" } =>
				IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.Handle)!),
			
			// general NSObject/INativeObject, has to be after the special types otherwise the special types will
			// fall into the NSObject/INativeObject case
			
			{ Type.IsNSObject: true } =>
				IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.Handle)!),
			
			// Runtime.GetINativeObject<ParameterType> (ParameterName, false)!
			{ Type.IsINativeObject: true } =>
				IdentifierName (GetNameForVariableType (argumentInfo.Name, VariableType.Handle)!),
			
			// by default, we will use the parameter name as is
			_ => parameterIdentifier
		};
#pragma warning restore format
		
		// at this point we have the native type to the manage type conversion done BUT if we are using a BindFrom
		// attribute, we need get that expression and convert the NSValue/NSNumber to the expected type.
		if (argumentInfo.BindAs is not null) {
			// the name of the bind as aux variable
			var variableName = GetNameForVariableType (argumentInfo.Name,
				argumentInfo.Type.IsArray ? VariableType.NSArray : VariableType.BindFrom)!;
			// we need to use the name used for the handle
			variableName = GetNameForVariableType (variableName, VariableType.Handle)!;
			expression = IdentifierName (variableName);
		}

		// Argument syntax is the same as the expression syntax, but we need to add the ref kind keyword if needed
		return Argument (expression);
	}
}
