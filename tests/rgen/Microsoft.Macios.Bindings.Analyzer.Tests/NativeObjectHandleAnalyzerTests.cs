// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Bindings.Analyzer.Tests;

public class NativeObjectHandleAnalyzerTests : BaseGeneratorWithAnalyzerTestClass {
	class ErrorTestCases : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			// Method with misuse
			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class Test
				{
					void Method(NSUuid foo) { _ = foo.Handle; }
				}
				"""];

			// Method with misuse of INativeObject itself
			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class Test
				{
					void Method(NSUuid foo) { _ = foo.Handle; }
				}
				"""];

			// Method with misuse of INativeObject generic parameter
			yield return [
				"""
				using ObjCRuntime;

				class Test<T> where T : INativeObject
				{
					void Method(T foo) { _ = foo.Handle; }
				}
				"""];

			// Expression with misuse
			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class Test
				{
					void Method(NSUuid foo) => _ = foo.GetNonNullHandle();
				}
				"""];

			// Constructor with misuse
			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class BaseTest
				{
					protected BaseTest(NativeHandle handle) { }
				}

				class Test : BaseTest
				{
					Test(NSUuid foo) : base(foo.Handle) { }
				}
				"""];

			// Constructor expression with misuse
			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class BaseTest
				{
					protected BaseTest(NativeHandle handle) { }
				}

				class Test : BaseTest
				{
					Test(NSUuid foo) : base(foo.Handle) => {}
				}
				"""];

			// Method call with handle access and no intermediate local variable
			yield return [
				"""
				using ObjCRuntime;
				using CoreFoundation;

				class Test
				{
					void Method()
					{
						_ = CFArray.FromStrings().GetHandle();
					}
				}
				"""];

			// Constructor with handle access and no intermediate local variable
			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class Test
				{
					void Method()
					{
						_ = (new NSUuid(new byte[16])).Handle;
					}
				}
				"""];

			// if statement
			yield return [
				"""
				using ObjCRuntime;
				using CoreMedia;
				using AVFoundation;

				public unsafe partial class AVCaptureMetadataInputTest : AVCaptureInput {
					internal AVCaptureMetadataInputTest (nint desc, global::CoreMedia.CMClock clock)
						: base (NSObjectFlag.Empty)
					{
						var clock__handle__ = clock!.GetNonNullHandle (nameof (clock));
						if (IsDirectBinding) {
							InitializeHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend_IntPtr_NativeHandle (this.Handle, Selector.GetHandle ("initWithFormatDescription:clock:"), desc, clock.Handle), "initWithFormatDescription:clock:");
						} else {
							InitializeHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSendSuper_IntPtr_NativeHandle (this.SuperHandle, Selector.GetHandle ("initWithFormatDescription:clock:"), desc, clock.Handle), "initWithFormatDescription:clock:");
						}
					}
				}
				
				"""];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	class NoErrorTestCases : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class Test
				{
					void Method(NSUuid foo) { _ = foo.Handle; GC.KeepAlive(foo); }
				}
				"""];

			// Pattern variable accessed later in the same if statement
			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class Test
				{
					void UseObject(INativeObject value) { }

					void Method(object value)
					{
						if (value is INativeObject nativeObject && nativeObject.Handle != NativeHandle.Zero) {
							UseObject(nativeObject);
						}
					}
				}
				"""];

			// Calling this.Handle is okay
			yield return [
				"""
				using ObjCRuntime;

				class Test : INativeObject
				{
					NativeHandle Handle { get; }
					void Method() { _ = this.Handle; }
				}
				"""];

			// Guard by a using block
			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class Test
				{
					void Method()
					{
						using (NSString foo = new NSString("foo"))
							_ = foo.Handle;

						using (NSString bar = new NSString("foo")) {
							_ = bar.Handle;
						}

						NSString foo2 = new NSString("foo");
						using (foo2)
							_ = foo2.Handle;

						NSString bar2 = new NSString("foo");
						using (bar2) {
							_ = bar2.Handle;
						}

						using var foo3 = new NSString("foo");
						_ = foo3.Handle;
					}
				}
				"""];

			// Constructor with GC.KeepAlive in body
			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class BaseTest
				{
					protected BaseTest(NativeHandle handle) { }
				}

				class Test : BaseTest
				{
					Test(NSString foo) : base(foo.Handle)
					{
						GC.KeepAlive(foo);
					}
				}
				"""];

			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class Test
				{
					void Method(NSString foo) { _ = foo.DangerousRetain().DangerousAutorelease().Handle; }
				}
				"""];

			yield return [
				"""
				using ObjCRuntime;
				using Foundation;

				class Test
				{
					void Method(NSString foo) { _ = foo.GetConstant().Handle; }
				}
				"""];

			// Equals implementation
			yield return [
				"""
				using ObjCRuntime;

				class Test : INativeObject
				{
					NativeHandle Handle { get; }
					bool Equals(Test other) { return this.Handle == other.Handle; }
				}
				"""];
			// if statement
			yield return [
				"""
				using ObjCRuntime;
				using CoreMedia;
				using AVFoundation;

				public unsafe partial class AVCaptureMetadataInputTest : AVCaptureInput {
					internal AVCaptureMetadataInputTest (nint desc, global::CoreMedia.CMClock clock)
						: base (NSObjectFlag.Empty)
					{
						var clock__handle__ = clock!.GetNonNullHandle (nameof (clock));
						if (IsDirectBinding) {
							InitializeHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSend_IntPtr_NativeHandle (this.Handle, Selector.GetHandle ("initWithFormatDescription:clock:"), desc, clock.Handle), "initWithFormatDescription:clock:");
						} else {
							InitializeHandle (global::ObjCRuntime.Messaging.NativeHandle_objc_msgSendSuper_IntPtr_NativeHandle (this.SuperHandle, Selector.GetHandle ("initWithFormatDescription:clock:"), desc, clock.Handle), "initWithFormatDescription:clock:");
						}
						GC.KeepAlive (clock);
					}
				}
				
				"""];

			// inside the if/else
			yield return [
				"""
				using ObjCRuntime;
				using Foundation;
				using CoreMedia;
				using AVFoundation;

				public unsafe static partial class NSCoder_Extensions {
					public static CGRect DecodeCGRect (this NSCoder This, string key)
					{
						global::UIKit.UIApplication.EnsureUIThread ();
						if (key is null)
							ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
						var nskey = CFString.CreateNative (key);
						CGRect ret;
						if (global::ObjCRuntime.Runtime.IsARM64CallingConvention) {
							ret = global::ObjCRuntime.Messaging.CGRect_objc_msgSend_NativeHandle (This.Handle, Selector.GetHandle ("decodeCGRectForKey:"), nskey);
							GC.KeepAlive (This);
						} else if (IntPtr.Size == 8) {
							ret = global::ObjCRuntime.Messaging.CGRect_objc_msgSend_stret_NativeHandle (This.Handle, Selector.GetHandle ("decodeCGRectForKey:"), nskey);
							GC.KeepAlive (This);
						} else if (Runtime.Arch == Arch.DEVICE) {
							ret = global::ObjCRuntime.Messaging.CGRect_objc_msgSend_NativeHandle (This.Handle, Selector.GetHandle ("decodeCGRectForKey:"), nskey);
							GC.KeepAlive (This);
						} else {
							ret = global::ObjCRuntime.Messaging.CGRect_objc_msgSend_stret_NativeHandle (This.Handle, Selector.GetHandle ("decodeCGRectForKey:"), nskey);
							GC.KeepAlive (This);
						}
						CFString.ReleaseNative (nskey);
						return ret!;
					}
				}
				
				"""];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<ErrorTestCases>]
	public async Task GCHoleTests (ApplePlatform platform, string inputText)
	{
		var (compilation, _) = CreateCompilation (platform, sources: inputText);
		var diagnostics = await RunAnalyzer (new NativeObjectHandleAnalyzer (), compilation);
		var analyzerDiagnotics = diagnostics.Where (d => d.Id == "RBI0014").ToArray ();
		// ensure that we have at least one error; it might be the case that we get more than one because
		// we access the handle multiple times.
		Assert.True (analyzerDiagnotics.Length != 0);
	}

	[Theory]
	[AllSupportedPlatformsClassData<NoErrorTestCases>]
	public async Task NoGCHoleTests (ApplePlatform platform, string inputText)
	{
		var (compilation, _) = CreateCompilation (platform, sources: inputText);
		var diagnostics = await RunAnalyzer (new NativeObjectHandleAnalyzer (), compilation);
		var analyzerDiagnotics = diagnostics.Where (d => d.Id == "RBI0014").ToArray ();
		Assert.Empty (analyzerDiagnotics);
	}
}
