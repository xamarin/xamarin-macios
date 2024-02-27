//
// Test the generated API selectors against typos or non-existing cases
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc. All rights reserved.
//

using System;
using System.Reflection;
using NUnit.Framework;

using Foundation;
using ObjCRuntime;

namespace Introspection {

	public abstract class CoreSelectorTest : ApiSelectorTest {

		protected override bool CheckResponse (bool value, Type actualType, MethodBase method, ref string name)
		{
			if (value)
				return true;

			var declaredType = method.DeclaringType;

			switch (declaredType.Name) {
			case "NSUrlSessionTaskMetrics":
			case "NSUrlSessionTaskTransactionMetrics":
				// does not respond but the properties works (see monotouch-test for a partial list)
				if (TestRuntime.CheckXcodeVersion (8, 0))
					return true;
				break;
			// broken with Xcode 12 beta 1
			case "MidiCISession":
				switch (name) {
				case "deviceIdentification":
					if (TestRuntime.CheckXcodeVersion (12, 0))
						return true;
					break;
				}
				break;
			}

			switch (name) {
			// optional stuff defined in NSObject (but not implemented by every subclasses)
			case "encodeWithCoder:":
			case "objectDidEndEditing:":
			case "commitEditing":
			case "commitEditingWithDelegate:didCommitSelector:contextInfo:":
				if (declaredType.Name == "NSObject")
					return true;
				break;
			// internal stuff that must be used
			case "_setCFClientFlags:callback:context:":
			case "_scheduleInCFRunLoop:forMode:":
			case "_unscheduleFromCFRunLoop:forMode:":
			// init* works (see monotouchtest.app) but does not respond when queried
			case "initWithFileAtPath:":
			case "initWithData:":
			case "initWithURL:":
				if (declaredType.Name == "NSInputStream")
					return true;
				break;
			// init* works (see monotouchtest.app) but does not respond when queried
			case "initToMemory":
			case "initToFileAtPath:append:":
				if (declaredType.Name == "NSOutputStream")
					return true;
				break;
			// init* works (see monotouchtest.app) but does not respond when queried
			case "initWithFileDescriptor:":
			case "initWithFileDescriptor:closeOnDealloc:":
				if (declaredType.Name == "NSFileHandle")
					return true;
				break;
			case "initWithString:":
			case "initWithString:attributes:":
			case "initWithAttributedString:":
				if (declaredType.Name == "NSAttributedString" || declaredType.Name == "NSMutableAttributedString")
					return true;
				break;
			}
			return base.CheckResponse (value, actualType, method, ref name);
		}

		protected override bool Skip (Type type)
		{
			switch (type.Name) {
			case "MTLRenderPassAttachmentDescriptor":
				// This is an abstract(-ish...) type, iOS allows creating an instance of it, but the instance doesn't respond to most of the selector in the headers.
				return true;
			}

			return base.Skip (type);
		}

		protected override IntPtr GetClassForType (Type type)
		{
			switch (type.Namespace) {
			case "MonoTouch.Metal":
			case "MonoMac.Metal":
			case "Metal":
				switch (type.Name) {
				case "MTLArgument":
				case "MTLArrayType":
				case "MTLCompileOptions":
				case "MTLComputePipelineDescriptor":
				case "MTLComputePipelineReflection":
				case "MTLDepthStencilDescriptor":
				case "MTLRenderPassAttachmentDescriptor":
				case "MTLRenderPassColorAttachmentDescriptor":
				case "MTLRenderPassDepthAttachmentDescriptor":
				case "MTLRenderPassDescriptor":
				case "MTLRenderPassStencilAttachmentDescriptor":
				case "MTLRenderPipelineColorAttachmentDescriptor":
				case "MTLRenderPipelineDescriptor":
				case "MTLRenderPipelineReflection":
				case "MTLSamplerDescriptor":
				case "MTLStencilDescriptor":
				case "MTLStructMember":
				case "MTLStructType":
				case "MTLTextureDescriptor":
				case "MTLVertexAttribute":
				case "MTLVertexAttributeDescriptor":
				case "MTLVertexBufferLayoutDescriptor":
				case "MTLVertexDescriptor":
					var ctor = type.GetConstructor (Type.EmptyTypes);
					using (var obj = ctor.Invoke (null) as NSObject) {
						return IntPtr_objc_msgSend (obj.Handle, Selector.GetHandle ("class"));
					}
				}
				break;
			}

			return base.GetClassForType (type);
		}
	}
}
