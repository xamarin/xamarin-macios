// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.Macios.Generator.Emitters.BindingSyntaxFactory;

namespace Microsoft.Macios.Generator.Tests.Emitters;

public class BindingSyntaxFactoryRuntimeTests {

	[Theory]
	[InlineData ("Test", "Selector.GetHandle (\"Test\")")]
	[InlineData ("name", "Selector.GetHandle (\"name\")")]
	[InlineData ("setName:", "Selector.GetHandle (\"setName:\")")]
	void GetHandleTest (string selector, string expectedDeclaration)
	{
		var declaration = GetHandle (selector);
		Assert.Equal (expectedDeclaration, declaration.ToFullString ());
	}
	
	class TestDataMessagingInvocationTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// no extra params
			yield return [
				"IntPtr_objc_msgSend",
				"string",
				ImmutableArray<ArgumentSyntax>.Empty,
				"global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, Selector.GetHandle (\"string\"))"
			];
			
			// one param extra
			ImmutableArray<ArgumentSyntax> args = ImmutableArray.Create (
				Argument (IdentifierName ("arg1"))
			);
			yield return [
				"IntPtr_objc_msgSend",
				"string",
				args,
				"global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, Selector.GetHandle (\"string\"), arg1)"
			];
			
			// several params
			args = ImmutableArray.Create (
				Argument (IdentifierName ("arg1")),
				Argument (IdentifierName ("arg2")),
				Argument (IdentifierName ("arg3"))
			);
			yield return [
				"IntPtr_objc_msgSend",
				"string",
				args,
				"global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, Selector.GetHandle (\"string\"), arg1, arg2, arg3)"
			];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[ClassData(typeof (TestDataMessagingInvocationTests))]
	void MessagingInvocationTests (string objcMsgSendMethod, string selector, ImmutableArray<ArgumentSyntax> parameters, 
		string expectedDeclaration)
	{
		var declaration = MessagingInvocation (objcMsgSendMethod, selector, parameters);
		Assert.Equal (expectedDeclaration, declaration.ToFullString ());
	}
}
