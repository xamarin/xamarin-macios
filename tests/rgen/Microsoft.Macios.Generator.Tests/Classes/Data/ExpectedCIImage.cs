// <auto-generated />

#nullable enable

using Foundation;
using ObjCBindings;
using ObjCRuntime;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace TestNamespace;

[Register ("CIImage", true)]
public partial class CIImage
{
	[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
	static readonly NativeHandle class_ptr = Class.GetHandle ("CIImage");

	public override NativeHandle ClassHandle => class_ptr;

	[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
	[DesignatedInitializer]
	[Export ("init")]
	public CIImage () : base (NSObjectFlag.Empty)
	{
		if (IsDirectBinding)
			InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSend (this.Handle, global::ObjCRuntime.Selector.GetHandle ("init")), "init");
		else
			InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper (this.SuperHandle, global::ObjCRuntime.Selector.GetHandle ("init")), "init");
	}

	[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	protected CIImage (NSObjectFlag t) : base (t) {}

	[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
	[EditorBrowsable (EditorBrowsableState.Advanced)]
	protected internal CIImage (NativeHandle handle) : base (handle) {}

	static Foundation.NSString? _DidProcessEditingNotification;

	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("maccatalyst13.1")]
	[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
	[Advice ("Use 'CIImage.Notifications.DidProcessEditingNotification' helper method instead.")]
	public static partial Foundation.NSString DidProcessEditingNotification
	{
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("maccatalyst13.1")]
		get
		{
			if (_DidProcessEditingNotification is null)
				_DidProcessEditingNotification = Dlfcn.GetStringConstant (Libraries.TestNamespace.Handle, "kCIFormatLA8")!;
			return _DidProcessEditingNotification;
		}
	}


	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("maccatalyst13.1")]
	[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
	public static partial int FormatABGR8
	{
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("maccatalyst13.1")]
		get
		{
			return Dlfcn.GetInt32 (Libraries.TestNamespace.Handle, "kCIFormatABGR8");
		}
	}


	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("maccatalyst13.1")]
	[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
	public static partial int FormatLA8
	{
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("maccatalyst13.1")]
		get
		{
			return Dlfcn.GetInt32 (Libraries.TestNamespace.Handle, "kCIFormatLA8");
		}

		[SupportedOSPlatform ("macos14.0")]
		[SupportedOSPlatform ("ios17.0")]
		[SupportedOSPlatform ("tvos17.0")]
		[SupportedOSPlatform ("maccatalyst17.0")]
		set
		{
			Dlfcn.SetInt32 (Libraries.TestNamespace.Handle, "kCIFormatLA8", value);
		}
	}


	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("ios11.0")]
	[SupportedOSPlatform ("tvos11.0")]
	[SupportedOSPlatform ("maccatalyst13.1")]
	[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]
	public static partial int FormatRGBA16Int
	{
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios11.0")]
		[SupportedOSPlatform ("tvos11.0")]
		[SupportedOSPlatform ("maccatalyst13.1")]
		get
		{
			return Dlfcn.GetInt32 (Libraries.TestNamespace.Handle, "FormatRGBA16Int");
		}
	}
	// TODO: add binding code here
}