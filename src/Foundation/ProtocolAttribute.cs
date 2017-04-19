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

using XamCore.ObjCRuntime;

namespace XamCore.Foundation {

	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Interface)]
	public sealed class ProtocolAttribute : Attribute {
		
		public ProtocolAttribute () {}

		public Type WrapperType { get; set; }
		public string Name { get; set; }
		public bool IsInformal { get; set; }
		// In which SDK version this protocol switched from (or to, depending on the current IsInformal value) being informal.
		// If IsInformal = true, then this protocol switched to being informal in the specified SDK version
		// If IsInformal = false, then this protocol switched to a real protocol in the specified SDK version
		// Version is not a valid type for attributes, so we're using an array of ints.
		int[] informal_switch;
		public int [] InformalSwitch {
			get {
				return informal_switch;
			}
			set {
				if (value != null && (value.Length < 2 || value.Length > 4))
					throw new ArgumentOutOfRangeException ("value", "array must either be null, or have between 2 and 4 elements");
				informal_switch = value;
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
		public Type[] ParameterType { get; set; }
		public bool[] ParameterByRef { get; set; }
		public bool IsVariadic { get; set; }

		public Type PropertyType { get; set; }
		public string GetterSelector { get; set; }
		public string SetterSelector { get; set; }
		public ArgumentSemantic ArgumentSemantic { get; set; }
	}
}
