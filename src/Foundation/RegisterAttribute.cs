//
// Copyright 2010, Novell, Inc.
// Copyright 2011, 2012 Xamarin Inc
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

#nullable enable

namespace Foundation {

	[AttributeUsage (AttributeTargets.Class)]
	public sealed class RegisterAttribute : Attribute {
		string? name;
		bool is_wrapper;

		public RegisterAttribute () { }
		public RegisterAttribute (string name)
		{
			this.name = name;
		}

		public RegisterAttribute (string name, bool isWrapper)
		{
			this.name = name;
			this.is_wrapper = isWrapper;
		}

		public string? Name {
			get { return this.name; }
			set { this.name = value; }
		}

		public bool IsWrapper {
			get { return this.is_wrapper; }
			set { this.is_wrapper = value; }
		}

		public bool SkipRegistration { get; set; }
	}
}
