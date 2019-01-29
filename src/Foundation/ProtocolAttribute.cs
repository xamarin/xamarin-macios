//
// Copyright 2014 - 2015, Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;

using ObjCRuntime;

namespace Foundation {

	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Interface)]
	public sealed class ProtocolAttribute : Attribute {
		
		public ProtocolAttribute () {}

		public Type WrapperType { get; set; }
		public string Name { get; set; }
		public bool IsInformal { get; set; }
		// In which SDK version this protocol switched from being informal (i.e. a category) to a formal protocol.
		// System.Version is not a valid type for attributes, so we're using a string instead.
		string informal_until;
		public string FormalSince {
			get {
				return informal_until;
			}
			set {
				if (value != null)
					Version.Parse (value); // This will throw an exception with invalid input, which is what we want.
				informal_until = value;
			}
		}
	}

	[AttributeUsage (AttributeTargets.Interface, AllowMultiple = true)]
	public sealed class ProtocolMemberAttribute : Attribute {
		public ProtocolMemberAttribute () {}

		public bool IsRequired { get; set; }
		public bool IsProperty { get; set; }
		public bool IsStatic { get; set; }
		public string Name { get; set; }
		public string Selector { get; set; }
		public Type ReturnType { get; set; }
		public Type ReturnTypeDelegateProxy { get; set; }
		public Type[] ParameterType { get; set; }
		public bool[] ParameterByRef { get; set; }
		public Type[] ParameterBlockProxy { get; set; }
		public bool IsVariadic { get; set; }

		public Type PropertyType { get; set; }
		public string GetterSelector { get; set; }
		public string SetterSelector { get; set; }
		public ArgumentSemantic ArgumentSemantic { get; set; }
	}
}
