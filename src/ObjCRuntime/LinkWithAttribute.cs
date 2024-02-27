//
// Authors: Jeffrey Stedfast
//
// Copyright 2011 Xamarin Inc.
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
using System.IO;

#nullable enable

namespace ObjCRuntime {
	[Flags]
	public enum LinkTarget : int {
		Simulator = 1,
		i386 = Simulator,
		ArmV6 = 2,
		ArmV7 = 4,
		Thumb = 8,
		ArmV7s = 16,
		Arm64 = 32,
		Simulator64 = 64,
		x86_64 = Simulator64
	}

	public enum DlsymOption {
		Default,
		Required,
		Disabled,
	}

	[AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class LinkWithAttribute : Attribute {
		public LinkWithAttribute (string libraryName, LinkTarget target, string linkerFlags)
		{
			LibraryName = libraryName;
			LinkerFlags = linkerFlags;
			LinkTarget = target;
		}

		public LinkWithAttribute (string libraryName, LinkTarget target)
		{
			LibraryName = libraryName;
			LinkTarget = target;
		}

		public LinkWithAttribute (string libraryName)
		{
			LibraryName = libraryName;
		}

		public LinkWithAttribute ()
		{
		}

		public bool ForceLoad {
			get; set;
		}

		public string? Frameworks {
			get; set;
		}

		public string? WeakFrameworks {
			get; set;
		}

		public string? LibraryName {
			get; private set;
		}

		public string? LinkerFlags {
			get; set;
		}

		public LinkTarget LinkTarget {
			get; set;
		}

		public bool NeedsGccExceptionHandling {
			get; set;
		}

		public bool IsCxx {
			get; set;
		}

		public bool SmartLink {
			get; set;
		}

		public DlsymOption Dlsym {
			get; set;
		}
	}
}
