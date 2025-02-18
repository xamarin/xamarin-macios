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
		/// <summary>A flag that signifies that the native library supports the Simulator (i386 architecture).</summary>
		Simulator = 1,
		/// <summary>A flag that signifies that the native library supports the Simulator (i386 architecture).</summary>
		i386 = Simulator,
		/// <summary>A flag that signifies that the native library supports the ARMv6 architecture.</summary>
		ArmV6 = 2,
		/// <summary>A flag that signifies that the native library supports the ARMv7 architecture.</summary>
		ArmV7 = 4,
		/// <summary>A flag that specifies that the native library targets the Thumb subset of <see cref="F:ObjCRuntime.LinkTarget.ArmV6" /> or <see cref="F:ObjCRuntime.LinkTarget.ArmV7" />.</summary>
		Thumb = 8,
		/// <summary>A flag that signifies that the native library supports the ARMv7s architecture.</summary>
		ArmV7s = 16,
		/// <summary>A flag that signifies that the native library supports the ARM-64 architecture.</summary>
		Arm64 = 32,
		/// <summary>A flag that signifies that the native library supports the Simulator (x86-64 architecture).</summary>
		Simulator64 = 64,
		/// <summary>A flag that signifies that the native library supports the Simulator (x86-64 architecture).</summary>
		x86_64 = Simulator64
	}

	public enum DlsymOption {
		/// <summary>Use the default value for the platform (for backwards compatibility reasons the default is to use dlsym on platforms that support it - this may change in the future).</summary>
		Default,
		/// <summary>This library requires using dlsym to resolve P/Invokes to native functions.</summary>
		Required,
		/// <summary>This library does not depend on using dlsym to resolve P/Invokes to native functions.</summary>
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

		/// <summary>Specifies whether or not the -force_load clang argument is required when linking this native library.</summary>
		///         <value>This value should be set to <c>true</c> if the -force_load argument is required, or <c>false</c> otherwise.</value>
		///         <remarks>
		///         </remarks>
		public bool ForceLoad {
			get; set;
		}

		/// <summary>Specifies a space-delimited list of platform Frameworks required by the native library.</summary>
		///         <value>This value should be set to a string containing a space-delimited list of platform Frameworks (e.g. "CoreGraphics CoreLocation CoreMedia MediaPlayer QuartzCore").</value>
		///         <remarks>
		///         </remarks>
		public string? Frameworks {
			get; set;
		}

		/// <summary>Specifies a list of space-delimited platform Frameworks that should be weakly linked.</summary>
		///         <value>This value should be set to a string containing a space-delimited list of platform Frameworks (e.g. "CoreBluetooth NewsstandKit Twitter").</value>
		///         <remarks>
		///         </remarks>
		public string? WeakFrameworks {
			get; set;
		}

		/// <summary>The name of the native library.</summary>
		///         <value>A string representing the name of the native library.</value>
		///         <remarks>
		///         </remarks>
		public string? LibraryName {
			get; private set;
		}

		/// <summary>Additional linker flags that are required for linking the native library to an application.</summary>
		///         <value>A string containing the extra linker flags to be passed to clang or <c>null</c> if unneeded.</value>
		///         <remarks>
		///         </remarks>
		public string? LinkerFlags {
			get; set;
		}

		/// <summary>The target platform (or platforms) that this library is built for.</summary>
		///         <value>A bitwise-or'ing of <see cref="T:ObjCRuntime.LinkTarget" />.</value>
		///         <remarks>
		///           <para>This field is ignored, Xamarin.iOS will instead look in the native library to see which architectures are present in the library.</para>
		///         </remarks>
		public LinkTarget LinkTarget {
			get; set;
		}

		/// <summary>Specifies whether or not the native library requires linking with the GCC Exception Handling library (libgcc_eh).</summary>
		///         <value>This value should be set to <c>true</c> if the native library requires linking with -lgcc_eh, or <c>false</c> otherwise.</value>
		///         <remarks>
		///         </remarks>
		public bool NeedsGccExceptionHandling {
			get; set;
		}

		/// <summary>Specifies whether or not the native library is a C++ library.</summary>
		///         <value>This value should be set to <c>true</c> if a c++ compiler should be used when building the application consuming this native library.</value>
		///         <remarks>
		///         </remarks>
		public bool IsCxx {
			get; set;
		}

		/// <summary>Determines whether the library uses Swift.</summary>
		/// <remarks>If this is true, the app will automatically link with the Swift system libraries.</remarks>
		public bool LinkWithSwiftSystemLibraries {
			get; set;
		}

		/// <summary>If this is set true, the ForceLoad value will be ignored when it is deemed safe to do so.</summary>
		///         <value>This value should be set to true to let Xamarin.iOS determine whether ForceLoad is required or not.</value>
		///         <remarks>
		///           <para>In the current implementation the ForceLoad flag is usually not required when the static registrar is used at compilation time. The exact logic is however an implementation detail and may change in future versions.</para>
		///         </remarks>
		public bool SmartLink {
			get; set;
		}

		/// <summary>Specifies if the managed assembly requires using dlsym to resolve P/Invokes to native functions.</summary>
		///         <value>Developers should set this value to <see cref="ObjCRuntime.DlsymOption.Disabled" /> if the library does not require using dlsym, since the AOT compiler can generate faster and smaller code.</value>
		///         <remarks>
		///           <para>The library requires using dlsym to resolve P/Invokes to native functions if it contains P/Invokes to native functions that don't exist on the target platform.</para>
		///         </remarks>
		public DlsymOption Dlsym {
			get; set;
		}
	}
}
