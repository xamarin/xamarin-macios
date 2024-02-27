//
// Copyright 2011, Novell, Inc.
// Copyright 2011, Regan Sarwas
//
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

//
// Enums.cs: Enums for ImageKit
//

#nullable enable

using System;

using ObjCRuntime;

namespace ImageKit {

	[Native]
	public enum IKCameraDeviceViewDisplayMode : long {
		None = -1,
		Table = 0,
		Icon = 1
	};

	[Native]
	public enum IKCameraDeviceViewTransferMode : long {
		File = 0,
		Memory = 1
	};

	[Native]
	public enum IKDeviceBrowserViewDisplayMode : long {
		Table = 0,
		Outline = 1,
		Icon = 2
	};

	// Untyped enum in ObjC
	public enum IKImageBrowserCellState : int {
		NoImage = 0,
		Invalid = 1,
		Ready = 2
	};

	[Flags]
	[Native]
	public enum IKCellsStyle : ulong {
		None = 0,
		Shadowed = 1 << 0,
		Outlined = 1 << 1,
		Titled = 1 << 2,
		Subtitled = 1 << 3
	};

	//used as a value for the IKImageBrowserGroupStyleKey in the NSDictionary that defines a group in IKImageBrowserView
	[Native]
	public enum IKGroupStyle : long {
		Bezel = 0,
		Disclosure = 1
	};

	// Untyped enum in ObjC
	public enum IKImageBrowserDropOperation : int {
		On = 0,
		Before = 1
	};

	[Native]
	public enum IKScannerDeviceViewTransferMode : long {
		File = 0,
		Memory = 1
	};

	[Native]
	public enum IKScannerDeviceViewDisplayMode : long {
		None = -1,
		Simple = 0,
		Advanced = 1
	};

	[Flags]
	public enum IKFilterBrowserPanelStyleMask : uint {
		Normal = 0,
		Textured = 1 << 8
		// Other NSWindow Style Mask bit settings do not apply to this panel
	}
}
