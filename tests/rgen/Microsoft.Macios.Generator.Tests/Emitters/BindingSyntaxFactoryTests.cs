// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Context;
using Microsoft.Macios.Generator.DataModel;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;
using static Microsoft.Macios.Generator.Emitters.BindingSyntaxFactory;

namespace Microsoft.Macios.Generator.Tests.Emitters;

public class BindingSyntaxFactoryTests : BaseGeneratorTestClass {
	
	class TestDataFieldConstantGetter : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string nsStringFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial NSString GenericGray { get; }

	}
}
";
			
			yield return [nsStringFieldProperty, 
				"Dlfcn.GetStringConstant (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];

			const string byteFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial byte GenericGray { get; }

	}
}
";
			
			yield return [byteFieldProperty, 
				"Dlfcn.GetByte (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string otherByteFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial Byte GenericGray { get; }

	}
}
";
			yield return [otherByteFieldProperty, 
				"Dlfcn.GetByte (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string sbyteFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial sbyte GenericGray { get; }

	}
}
";
			
			yield return [sbyteFieldProperty, 
				"Dlfcn.GetSByte (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string otherSbyteFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial SByte GenericGray { get; }

	}
}
";
			
			yield return [otherSbyteFieldProperty, 
				"Dlfcn.GetSByte (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string int16FieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial short GenericGray { get; }

	}
}
";
			
			yield return [int16FieldProperty, 
				"Dlfcn.GetInt16 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string otherInt16FieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial Int16 GenericGray { get; }

	}
}
";
			
			yield return [otherInt16FieldProperty, 
				"Dlfcn.GetInt16 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string uint16FieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial ushort GenericGray { get; }

	}
}
";
			
			yield return [uint16FieldProperty, 
				"Dlfcn.GetUInt16 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string otherUint16FieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial UInt16 GenericGray { get; }

	}
}
";
			
			yield return [otherUint16FieldProperty, 
				"Dlfcn.GetUInt16 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string int32FieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial int GenericGray { get; }

	}
}
";
			
			yield return [int32FieldProperty, 
				"Dlfcn.GetInt32 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string otherInt32FieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial Int32 GenericGray { get; }

	}
}
";
			
			yield return [otherInt32FieldProperty, 
				"Dlfcn.GetInt32 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string uint32FieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial uint GenericGray { get; }

	}
}
";
			
			yield return [uint32FieldProperty, 
				"Dlfcn.GetUInt32 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string otherUint32FieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial UInt32 GenericGray { get; }

	}
}
";
			
			yield return [otherUint32FieldProperty, 
				"Dlfcn.GetUInt32 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string doubleFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial double GenericGray { get; }

	}
}
";
			
			yield return [doubleFieldProperty, 
				"Dlfcn.GetDouble (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string otherDoubleFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial Double GenericGray { get; }

	}
}
";
			
			yield return [otherDoubleFieldProperty, 
				"Dlfcn.GetDouble (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string floatFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial float GenericGray { get; }

	}
}
";
			
			yield return [floatFieldProperty, 
				"Dlfcn.GetFloat (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string otherFloatFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial Single GenericGray { get; }

	}
}
";
			
			yield return [otherFloatFieldProperty, 
				"Dlfcn.GetFloat (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string intPtrFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial IntPtr GenericGray { get; }

	}
}
";
			
			yield return [intPtrFieldProperty, 
				"Dlfcn.GetIntPtr (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string uintPtrFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial UIntPtr GenericGray { get; }

	}
}
";
			
			yield return [uintPtrFieldProperty, 
				"Dlfcn.GetUIntPtr (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string sizeFFieldProperty = @"
using System;
using System.Drawing;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial SizeF GenericGray { get; }

	}
}
";
			
			yield return [sizeFFieldProperty, 
				"Dlfcn.GetSizeF (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string nintFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial nint GenericGray { get; }

	}
}
";
			
			yield return [nintFieldProperty, 
				"Dlfcn.GetIntPtr (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string nuintFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial nuint GenericGray { get; }

	}
}
";
			
			yield return [nuintFieldProperty, 
				"Dlfcn.GetUIntPtr (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string nfloatFieldProperty = @"
using System;
using Foundation;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial nfloat GenericGray { get; }

	}
}
";

			yield return [nfloatFieldProperty, 
				"Dlfcn.GetNFloat (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string cgsizeFieldProperty = @"
using System;
using Foundation;
using CoreGraphics;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial CGSize GenericGray { get; }

	}
}
";

			yield return [cgsizeFieldProperty, 
				"Dlfcn.GetCGSize (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string cmtagFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial CMTag GenericGray { get; }

	}
}
";

			yield return [cmtagFieldProperty, 
				"Dlfcn.GetStruct<CoreMedia.CMTag> (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"];
			
			const string nsArrayFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial NSArray GenericGray { get; }

	}
}
";

			yield return [nsArrayFieldProperty, 
				"Runtime.GetNSObject<Foundation.NSArray> (Dlfcn.GetIndirect (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\"))!;"];
			
			const string nsNumberFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial NSNumber GenericGray { get; }

	}
}
";

			yield return [nsNumberFieldProperty, 
				"Runtime.GetNSObject<Foundation.NSNumber> (Dlfcn.GetIndirect (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\"))!;"];
			
			const string sbyteEnumFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	public enum CGColorSpaceGenericGray : sbyte{
		First, 
		Second,
	}

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial CGColorSpaceGenericGray GenericGray { get; }

	}
}
";

			yield return [
				sbyteEnumFieldProperty,
				"Dlfcn.GetSByte (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"
			];
			
			const string byteEnumFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	public enum CGColorSpaceGenericGray : byte{
		First, 
		Second,
	}

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial CGColorSpaceGenericGray GenericGray { get; }

	}
}
";

			yield return [
				byteEnumFieldProperty,
				"Dlfcn.GetByte (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"
			];
			
			const string shortEnumFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	public enum CGColorSpaceGenericGray : short {
		First, 
		Second,
	}

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial CGColorSpaceGenericGray GenericGray { get; }

	}
}
";

			yield return [
				shortEnumFieldProperty,
				"Dlfcn.GetInt16 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"
			];
			
			const string ushortEnumFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	public enum CGColorSpaceGenericGray : ushort {
		First, 
		Second,
	}

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial CGColorSpaceGenericGray GenericGray { get; }

	}
}
";

			yield return [
				ushortEnumFieldProperty,
				"Dlfcn.GetUInt16 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"
			];
			
			const string intEnumFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	public enum CGColorSpaceGenericGray : int {
		First, 
		Second,
	}

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial CGColorSpaceGenericGray GenericGray { get; }

	}
}
";

			yield return [
				intEnumFieldProperty,
				"Dlfcn.GetInt32 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"
			];
			
			const string uintEnumFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	public enum CGColorSpaceGenericGray : uint {
		First, 
		Second,
	}

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial CGColorSpaceGenericGray GenericGray { get; }

	}
}
";

			yield return [
				uintEnumFieldProperty,
				"Dlfcn.GetUInt32 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"
			];
			
			const string longEnumFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	public enum CGColorSpaceGenericGray : long {
		First, 
		Second,
	}

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial CGColorSpaceGenericGray GenericGray { get; }

	}
}
";

			yield return [
				longEnumFieldProperty,
				"Dlfcn.GetInt64 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"
			];
			
			const string ulongEnumFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	public enum CGColorSpaceGenericGray : ulong {
		First, 
		Second,
	}

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial CGColorSpaceGenericGray GenericGray { get; }

	}
}
";

			yield return [
				ulongEnumFieldProperty,
				"Dlfcn.GetUInt64 (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\");"
			];
			
			const string cmTimeFieldProperty = @"
using System;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial CMTime GenericGray { get; }

	}
}
";

			yield return [
				cmTimeFieldProperty,
				"*((CoreMedia.CMTime*) Dlfcn.dlsym (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\"));" 
			];
			
			const string whiteFieldProperty = @"
using System;
using AVFoundation;
using Foundation;
using CoreMedia;
using ObjCBindings;

namespace CoreGraphics {

	[BindingType<Class>]
	public partial class CGColorSpaceNames {

		[Field<Property> (""kCGColorSpaceGenericGray"")]
		public partial AVCaptureWhiteBalanceGains GenericGray { get; }

	}
}
";

			yield return [
				whiteFieldProperty,
				"*((AVFoundation.AVCaptureWhiteBalanceGains*) Dlfcn.dlsym (Libraries.CoreGraphics.Handle, \"kCGColorSpaceGenericGray\"));" 
			];
		}
		
		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataFieldConstantGetter>]
	void FieldConstantGetterTests (ApplePlatform platform, string inputText, string expectedCall)
	{
		var (compilation, sourceTrees) =
			CreateCompilation (platform, sources: inputText);
		Assert.Single (sourceTrees);
		// get the declarations we want to work with and the semantic model
		var node = sourceTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<PropertyDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (node);
		var semanticModel = compilation.GetSemanticModel (sourceTrees [0]);
		var context = new RootBindingContext (semanticModel);
		Assert.True(Property.TryCreate(node, context, out var property));
		var compilationUnit = FieldConstantGetter (property.Value).ToString ();
		var str = compilationUnit.ToString ();
		Assert.Equal (expectedCall, FieldConstantGetter (property.Value).ToString ());
	}
}
