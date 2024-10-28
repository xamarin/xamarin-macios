using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class NamedTypeSymbolExtensionsTests : BaseGeneratorTestClass {
	[Theory]
	[AllSupportedPlatforms]
	public void TryGetEnumFieldsNotEnum (ApplePlatform platform)
	{
		const string inputString = @"
namespace Test;
public class NotEnum {
}
";
		var (compilation, syntaxTrees) = CreateCompilation (nameof(TryGetEnumFieldsNotEnum), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.False (symbol.TryGetEnumFields (out var fields, out var diagnostics));
		Assert.Null (fields);
		Assert.Single (diagnostics);
	}

	const string emptyEnum = @"
namespace Test;
public enum MyEnum {
}
";

	const string missingFieldAttributes = @"
namespace Test;
public enum MyEnum {
	First,
	Second,
	Last,
}
";

	[Theory]
	[AllSupportedPlatforms (emptyEnum)]
	[AllSupportedPlatforms (missingFieldAttributes)]
	public void TryGetEnumFieldsNoFields (ApplePlatform platform, string inputString)
	{
		var (compilation, syntaxTrees) = CreateCompilation (nameof(TryGetEnumFieldsNotEnum), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.True (symbol.TryGetEnumFields (out var fields, out var diagnostics));
		Assert.NotNull (fields);
		Assert.Empty (fields);
		Assert.Null (diagnostics);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void TryGetEnumFieldsWithAttr (ApplePlatform platform)
	{
		const string inputString = @"
using ObjCRuntime;
using ObjCBindings;

namespace Test;
public enum MyEnum {
	[Field<EnumValue> (""FirstBackendField"")]
	First,
	[Field<EnumValue> (""SecondBackendField"")]
	Second,
	// should be ignored because it does not have the FieldAttribute
	Last,
}
";
		var (compilation, syntaxTrees) = CreateCompilation (nameof(TryGetEnumFieldsNotEnum), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.True (symbol.TryGetEnumFields (out var fields, out var diagnostics));
		// we should get no fields because there are no attributes
		Assert.Null (diagnostics);
		Assert.NotNull (fields);
		Assert.Equal (2, fields.Value.Length);
		// assert the data from the field attr
		Assert.Equal ("FirstBackendField", fields.Value [0].FieldData.SymbolName);
		Assert.Equal ("SecondBackendField", fields.Value [1].FieldData.SymbolName);
	}

	[Theory]
	[AllSupportedPlatforms]
	public void TryGetBindingDataNoBinding (ApplePlatform platform)
	{
		const string inputString = @"
using ObjCRuntime;
using ObjCBindings;

namespace Test;
// no binding type attr
public class MyTestClass {
}
";
		var (compilation, syntaxTrees) = CreateCompilation (nameof(TryGetEnumFieldsNotEnum), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.False (symbol.TryGetBindingData (out var bindingData));
		Assert.Null (bindingData);
	}

	const string classWithBindingNoName = @"
using System;
using Foundation;
using ObjCBindings;
namespace TestNamespace;

[BindingType]
public partial class AVAudioPcmBuffer : AVAudioBuffer {
}
";

	const string classWithBindingWithName = @"
using System;
using Foundation;
using ObjCBindings;
namespace TestNamespace;

[BindingType (Name = ""AVAudioPCMBuffer"")]
public partial class AVAudioPcmBuffer : AVAudioBuffer {
}
";

	[Theory]
	[AllSupportedPlatforms (classWithBindingNoName, null!)]
	[AllSupportedPlatforms (classWithBindingWithName, "AVAudioPCMBuffer")]
	public void TryGetBindingData (ApplePlatform platform, string inputString, string? expectedName)
	{
		var (compilation, syntaxTrees) = CreateCompilation (nameof(TryGetEnumFieldsNotEnum), platform, inputString);
		Assert.Single (syntaxTrees);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.True (symbol.TryGetBindingData (out var bindingData));
		Assert.NotNull (bindingData);
		Assert.Equal (expectedName, bindingData.Name);
	}

	record ExpectedData (string [] Constructors, string [] Methods, string [] Properties) {
		public ExpectedData () : this ([], [], []) { }
		public string [] Constructors { get; set; } = Constructors;
		public string [] Methods { get; set; } = Methods;
		public string [] Properties { get; set; } = Properties;
	}

	class TestDataTryGetExportedMembers : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			foreach (var platform in Configuration.GetAllPlatforms (true)) {
				const string emptyClass = @"
using System;
using ObjCBindings;
namespace TestNamespace;

[BindingType (Name = ""AVAudioPCMBuffer"")]
public partial class AVAudioPcmBuffer {
}
";
				yield return [platform, emptyClass, new ExpectedData ()];

				const string propertyClass = @"
using System;
using AVFoundation;
using Foundation;
using ObjCBindings;
namespace TestNamespace;

[BindingType (Name = ""AVAudioPCMBuffer"")]
public partial class AVAudioPcmBuffer {
	[Export<Property> (""frameCapacity"")]
	public virtual partial uint FrameCapacity { get; } /* AVAudioFrameCount = uint32_t */

	[Export<Property> (""frameLength"")]
	public virtual partial uint FrameLength { get; set; } /* AVAudioFrameCount = uint32_t */

	[Export<Property> (""stride"")]
	public virtual partial nuint Stride { get; }
}
";
				yield return [platform, propertyClass,
					new ExpectedData ([], [], ["FrameCapacity", "FrameLength", "Stride"] )];

				const string methodClass = @"
using System;
using AVFoundation;
using Foundation;
using ObjCBindings;
namespace TestNamespace;

[BindingType (Name = ""AVAudioPCMBuffer"")]
public partial class AVAudioPcmBuffer {
	[Export<Method> (""audioPlayerDidFinishPlaying:successfully:"")]
	public virtual partial void FinishedPlaying (AVAudioPlayer player, bool flag);

	[Export<Method> (""audioPlayerDecodeErrorDidOccur:error:"")]
	public virtual partial void DecoderError (AVAudioPlayer player, [NullAllowed] NSError error);
}
";
				yield return [platform, methodClass,
					new ExpectedData ([], ["FinishedPlaying", "FinishedPlaying"], [])];

				const string constructorClass = @"
using System;
using Foundation;
using ObjCBindings;
namespace TestNamespace;

[BindingType (Name = ""AVAudioPCMBuffer"")]
public partial class AVAudioPcmBuffer {
	[Export<Constructor> (""initWithPCMFormat:frameCapacity:"")]
	public virtual partial NativeHandle InitWithPCMFormatFrameCapacity (AVAudioFormat format, uint /* AVAudioFrameCount = uint32_t */ frameCapacity);

	[Export<Constructor> (""initWithPCMFormat:bufferListNoCopy:deallocator:"")]
	public virtual partial NativeHandle InitWithPCMFormatBufferListNoCopy (AVAudioFormat format, AudioBuffers bufferList, [NullAllowed] Action<AudioBuffers> deallocator);
}
";
				yield return [platform, constructorClass,
					new ExpectedData (["InitWithPCMFormatFrameCapacity", "InitWithPCMFormatBufferListNoCopy"], [], [])];

				const string allMembersClass = @"
using System;
using AVFoundation;
using Foundation;
using ObjCBindings;
namespace TestNamespace;

[BindingType (Name = ""AVAudioPCMBuffer"")]
public partial class AVAudioPcmBuffer {
	[Export<Constructor> (""initWithPCMFormat:frameCapacity:"")]
	public virtual partial NativeHandle InitWithPCMFormatFrameCapacity (AVAudioFormat format, uint /* AVAudioFrameCount = uint32_t */ frameCapacity);

	[Export<Constructor> (""initWithPCMFormat:bufferListNoCopy:deallocator:"")]
	public virtual partial NativeHandle InitWithPCMFormatBufferListNoCopy (AVAudioFormat format, AudioBuffers bufferList, [NullAllowed] Action<AudioBuffers> deallocator);

	[Export<Property> (""frameCapacity"")]
	public virtual partial uint FrameCapacity { get; } /* AVAudioFrameCount = uint32_t */

	[Export<Property> (""frameLength"")]
	public virtual partial uint FrameLength { get; set; } /* AVAudioFrameCount = uint32_t */

	[Export<Property> (""stride"")]
	public virtual partial nuint Stride { get; }

	[Export<Method> (""audioPlayerDidFinishPlaying:successfully:"")]
	public virtual partial void FinishedPlaying (AVAudioPlayer player, bool flag);

	[Export<Method> (""audioPlayerDecodeErrorDidOccur:error:"")]
	public virtual partial void DecoderError (AVAudioPlayer player, [NullAllowed] NSError error);
}
";
				yield return [platform, allMembersClass, new ExpectedData (
					["InitWithPCMFormatFrameCapacity", "InitWithPCMFormatBufferListNoCopy"],
					["FinishedPlaying", "DecoderError"],
					["FrameCapacity", "FrameLength", "Stride"])];

				const string missingExportClass = @"
using System;
using Foundation;
using ObjCBindings;
namespace TestNamespace;

[BindingType (Name = ""AVAudioPCMBuffer"")]
public partial class AVAudioPcmBuffer {

	[Export<Constructor> (""initWithPCMFormat:frameCapacity:"")]
	public virtual partial NativeHandle InitWithPCMFormatFrameCapacity (AVAudioFormat format, uint /* AVAudioFrameCount = uint32_t */ frameCapacity);

	public virtual NativeHandle InitWithPCMFormatBufferListNoCopy (AVAudioFormat format, AudioBuffers bufferList, [NullAllowed] Action<AudioBuffers> deallocator) {
		return IntPtr.Zero;
	}

	[Export<Property> (""frameCapacity"")]
	public virtual partial uint FrameCapacity { get; } /* AVAudioFrameCount = uint32_t */

	[Export<Property> (""frameLength"")]
	public virtual partial uint FrameLength { get; set; } /* AVAudioFrameCount = uint32_t */

	public virtual nuint Stride { get; } = 0;

	[Export<Method> (""audioPlayerDidFinishPlaying:successfully:"")]
	public virtual partial void FinishedPlaying (AVAudioPlayer player, bool flag);

	public virtual void DecoderError (AVAudioPlayer player, [NullAllowed] NSError error) {}
}
";
				yield return [platform, missingExportClass, new ExpectedData (
					["InitWithPCMFormatFrameCapacity"],
					["FinishedPlaying"],
					["FrameCapacity", "FrameLength"])];
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData (typeof(TestDataTryGetExportedMembers))]
	void TryGetExportedMembers (ApplePlatform platform, string inputText, ExpectedData expectedMembers)
	{
		var (compilation, syntaxTrees) = CreateCompilation (nameof(TryGetExportedMembers), platform, inputText);
		var declaration = syntaxTrees [0].GetRoot ()
			.DescendantNodes ()
			.OfType<ClassDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		var semanticModel = compilation.GetSemanticModel (syntaxTrees [0]);
		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);
		Assert.True (symbol.TryGetExportedMembers (out var members));
		Assert.NotNull (members);
		Assert.Equal (expectedMembers.Constructors.Length, members.Value.Constructors.Length);
		// collect all symbol names and assert that they are present
		var constructorNames = members.Value.Constructors.Select (i => i.Symbol.Name).ToArray ();
		Assert.Multiple (() => {
			foreach (var name in expectedMembers.Constructors)
				Assert.Contains (name, constructorNames);
		});
		Assert.Equal (expectedMembers.Methods.Length, members.Value.Methods.Length);
		var methodNames = members.Value.Methods.Select (i => i.Symbol.Name).ToArray ();
		Assert.Multiple (() => {
			foreach (var name in expectedMembers.Methods)
				Assert.Contains (name, methodNames);
		});
		Assert.Equal (expectedMembers.Properties.Length, members.Value.Properties.Length);
		var propertyNames = members.Value.Properties.Select (i => i.Symbol.Name).ToArray ();
		Assert.Multiple (() => {
			foreach (var name in expectedMembers.Properties)
				Assert.Contains (name, propertyNames);
		});
	}
}
