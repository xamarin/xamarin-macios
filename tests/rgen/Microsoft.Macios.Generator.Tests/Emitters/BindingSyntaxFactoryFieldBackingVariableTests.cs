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

public class BindingSyntaxFactoryFieldBackingVariableTests : BaseGeneratorTestClass {

	class TestDataFieldPropertyBackingVariable : IEnumerable<object []> {
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

			yield return [
				nsStringFieldProperty,
				"static Foundation.NSString? _GenericGray;"
			];

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

			yield return [
				byteFieldProperty,
				"static byte _GenericGray;"
			];

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
			yield return [
				otherByteFieldProperty,
				"static byte _GenericGray;"
			];

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

			yield return [
				sbyteFieldProperty,
				"static sbyte _GenericGray;"
			];

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

			yield return [
				otherSbyteFieldProperty,
				"static sbyte _GenericGray;"
			];

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

			yield return [
				int16FieldProperty,
				"static short _GenericGray;"
			];

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

			yield return [
				otherInt16FieldProperty,
				"static short _GenericGray;"
			];

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

			yield return [
				uint16FieldProperty,
				"static ushort _GenericGray;"
			];

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

			yield return [
				otherUint16FieldProperty,
				"static ushort _GenericGray;"
			];

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

			yield return [
				int32FieldProperty,
				"static int _GenericGray;"
			];

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

			yield return [
				otherInt32FieldProperty,
				"static int _GenericGray;"
			];

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

			yield return [
				uint32FieldProperty,
				"static uint _GenericGray;"
			];

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

			yield return [
				otherUint32FieldProperty,
				"static uint _GenericGray;"
			];

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

			yield return [
				doubleFieldProperty,
				"static double _GenericGray;"
			];

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

			yield return [
				otherDoubleFieldProperty,
				"static double _GenericGray;"
			];

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

			yield return [
				floatFieldProperty,
				"static float _GenericGray;"
			];

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

			yield return [
				otherFloatFieldProperty,
				"static float _GenericGray;"
			];

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

			yield return [
				intPtrFieldProperty,
				"static IntPtr _GenericGray;"
			];


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

			yield return [
				uintPtrFieldProperty,
				"static UIntPtr _GenericGray;"
			];

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

			yield return [
				nintFieldProperty,
				"static IntPtr _GenericGray;"
			];

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

			yield return [
				nuintFieldProperty,
				"static UIntPtr _GenericGray;"
			];

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

			yield return [
				nfloatFieldProperty,
				"static nfloat? _GenericGray;"
			];

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

			yield return [
				cgsizeFieldProperty,
				"static CoreGraphics.CGSize _GenericGray;"
			];

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

			yield return [
				cmtagFieldProperty,
				"static CoreMedia.CMTag _GenericGray;"
			];

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

			yield return [
				nsArrayFieldProperty,
				"static Foundation.NSArray? _GenericGray;"
			];

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

			yield return [
				nsNumberFieldProperty,
				"static Foundation.NSNumber? _GenericGray;"
			];

			const string enumFieldProperty = @"
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
				enumFieldProperty,
				"static CoreGraphics.CGColorSpaceGenericGray _GenericGray;"
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataFieldPropertyBackingVariable>]
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
		var context = new RootContext (semanticModel);
		Assert.True (Property.TryCreate (node, context, out var property));
		Assert.Equal (expectedCall, FieldPropertyBackingVariable (property.Value).ToString ());
	}
}
