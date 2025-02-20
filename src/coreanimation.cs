//
// coreanimation.cs: API definition for CoreAnimation binding
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyright 2010, Novell, Inc.
// Copyright 2011, 2012, 2015 Xamarin Inc
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
using System.ComponentModel;
using System.Diagnostics;
#if MONOMAC
using AppKit;
using CoreVideo;
using OpenGL;
#else
using UIKit;
#endif
#if HAS_OPENGLES
using OpenGLES;
#endif
using Foundation;
using CoreImage;
using CoreGraphics;
using ObjCRuntime;
using Metal;
using SceneKit; // For SCNAnimationEvent

#if __TVOS__
using CAEdrMetadata = Foundation.NSObject;
#endif

#if !MONOMAC
using CGLPixelFormat = Foundation.NSObject;
using CVTimeStamp = Foundation.NSObject;
using CGLContext = System.IntPtr;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreAnimation {

	/// <summary>Provides a hierarchical timing system, with support for repetition and sequencing.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAMediaTiming_protocol/index.html">Apple documentation for <c>CAMediaTiming</c></related>
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface CAMediaTiming {
		[Abstract]
		[Export ("beginTime")]
		double BeginTime { get; set; }

		[Abstract]
		[Export ("duration")]
		double Duration { get; set; }

		[Abstract]
		[Export ("speed")]
		float Speed { get; set; } /* float, not CGFloat */

		[Abstract]
		[Export ("timeOffset")]
		double TimeOffset { get; set; }

		[Abstract]
		[Export ("repeatCount")]
		float RepeatCount { get; set; } /* float, not CGFloat */

		[Abstract]
		[Export ("repeatDuration")]
		double RepeatDuration { get; set; }

		[Abstract]
		[Export ("autoreverses")]
		bool AutoReverses { get; set; }

		[Abstract]
		[Export ("fillMode", ArgumentSemantic.Copy)]
		string FillMode { get; set; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:CoreAnimation.CAMediaTiming" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:CoreAnimation.CAMediaTiming" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:CoreAnimation.CAMediaTiming" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <format type="text/html"><a href="https://docs.microsoft.com/en-us/search/index?search=Core%20Animation%20CAMedia%20Timing_%20Extensions&amp;scope=Xamarin" title="T:CoreAnimation.CAMediaTiming_Extensions">T:CoreAnimation.CAMediaTiming_Extensions</a></format> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ICAMediaTiming { }

	[NoiOS]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CAConstraintLayoutManager : NSCoding {
		[Static]
		[Export ("layoutManager")]
		CAConstraintLayoutManager LayoutManager { get; }
	}

	[NoiOS]
	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CAConstraint : NSSecureCoding {
		[Export ("attribute")]
		CAConstraintAttribute Attribute { get; }

		[Export ("sourceName")]
		string SourceName { get; }

		[Export ("sourceAttribute")]
		CAConstraintAttribute SourceAttribute { get; }

		[Export ("scale")]
		nfloat Scale { get; }

		[Static]
		[Export ("constraintWithAttribute:relativeTo:attribute:scale:offset:")]
		CAConstraint Create (CAConstraintAttribute attribute, string relativeToSource, CAConstraintAttribute srcAttr, nfloat scale, nfloat offset);

		[Static]
		[Export ("constraintWithAttribute:relativeTo:attribute:offset:")]
		CAConstraint Create (CAConstraintAttribute attribute, string relativeToSource, CAConstraintAttribute srcAttr, nfloat offset);

		[Static]
		[Export ("constraintWithAttribute:relativeTo:attribute:")]
		CAConstraint Create (CAConstraintAttribute attribute, string relativeToSource, CAConstraintAttribute srcAttribute);

		[Export ("initWithAttribute:relativeTo:attribute:scale:offset:")]
		NativeHandle Constructor (CAConstraintAttribute attribute, string relativeToSource, CAConstraintAttribute srcAttr, nfloat scale, nfloat offset);
	}

	/// <include file="../docs/api/CoreAnimation/CADisplayLink.xml" path="/Documentation/Docs[@DocId='T:CoreAnimation.CADisplayLink']/*" />
	[Mac (14, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface CADisplayLink {
		[Export ("displayLinkWithTarget:selector:")]
		[Static]
		CADisplayLink Create (NSObject target, Selector sel);

		[Export ("addToRunLoop:forMode:")]
		void AddToRunLoop (NSRunLoop runloop, NSString mode);

		[Wrap ("AddToRunLoop (runloop, mode.GetConstant ()!)")]
		void AddToRunLoop (NSRunLoop runloop, NSRunLoopMode mode);

		[Export ("removeFromRunLoop:forMode:")]
		void RemoveFromRunLoop (NSRunLoop runloop, NSString mode);

		[Wrap ("RemoveFromRunLoop (runloop, mode.GetConstant ()!)")]
		void RemoveFromRunLoop (NSRunLoop runloop, NSRunLoopMode mode);

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("timestamp")]
		double Timestamp { get; }

		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; set; }

		[Obsoleted (PlatformName.iOS, 10, 0, message: "Use 'PreferredFramesPerSecond' property.")]
		[Obsoleted (PlatformName.TvOS, 10, 0, message: "Use 'PreferredFramesPerSecond' property.")]
		[Obsoleted (PlatformName.MacCatalyst, 13, 1, message: "Use 'PreferredFramesPerSecond' property.")]
		[NoMac]
		[Export ("frameInterval")]
		nint FrameInterval { get; set; }

		[Export ("duration")]
		double Duration { get; }

		[MacCatalyst (13, 1)]
		[Export ("targetTimestamp")]
		double TargetTimestamp { get; }

		[Deprecated (PlatformName.iOS, 15, 0, message: "Use 'PreferredFrameRateRange' property.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use 'PreferredFrameRateRange' property.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use 'PreferredFrameRateRange' property.")]
		[NoMac]
		[Export ("preferredFramesPerSecond")]
		nint PreferredFramesPerSecond { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("preferredFrameRateRange", ArgumentSemantic.Assign)]
		CAFrameRateRange PreferredFrameRateRange { get; set; }
	}

	[MacCatalyst (13, 1)]
	enum CAContentsFormat {
		[Field ("kCAContentsFormatGray8Uint")]
		Gray8Uint,
		[Field ("kCAContentsFormatRGBA8Uint")]
		Rgba8Uint,
		[Field ("kCAContentsFormatRGBA16Float")]
		Rgba16Float,
	}


	/// <include file="../docs/api/CoreAnimation/CALayer.xml" path="/Documentation/Docs[@DocId='T:CoreAnimation.CALayer']/*" />
	[BaseType (typeof (NSObject))]
	[Dispose ("OnDispose ();", Optimizable = true)]
	interface CALayer : CAMediaTiming, NSSecureCoding {
		[Export ("layer")]
		[Static]
		CALayer Create ();

		[Export ("presentationLayer")]
		[NullAllowed]
		CALayer PresentationLayer { get; }

		[Export ("modelLayer")]
		CALayer ModelLayer { get; }

		[Static]
		[Export ("defaultValueForKey:")]
		[return: NullAllowed]
		NSObject DefaultValue (string key);

		[Static]
		[Export ("needsDisplayForKey:")]
		bool NeedsDisplayForKey (string key);

		[Export ("bounds")]
		CGRect Bounds { get; set; }

		[Export ("zPosition")]
		nfloat ZPosition { get; set; }

		[Export ("anchorPoint")]
		CGPoint AnchorPoint { get; set; }

		[Export ("anchorPointZ")]
		nfloat AnchorPointZ { get; set; }

		[Export ("position")]
		CGPoint Position { get; set; }

		[Export ("transform")]
		CATransform3D Transform { get; set; }

		[Export ("affineTransform")]
		CGAffineTransform AffineTransform { get; set; }

		[Export ("frame")]
		CGRect Frame { get; set; }

		[Export ("hidden")] // Setter needs setHidden instead
		bool Hidden { [Bind ("isHidden")] get; set; }

		[Export ("doubleSided")]  // Setter needs setDoubleSided
		bool DoubleSided { [Bind ("isDoubleSided")] get; set; }

		[Export ("geometryFlipped")]
		bool GeometryFlipped { [Bind ("isGeometryFlipped")] get; set; }

		[Export ("contentsAreFlipped")]
		bool ContentsAreFlipped { get; }

		[Export ("superlayer")]
		[NullAllowed]
		CALayer SuperLayer { get; }

		[Export ("removeFromSuperlayer")]
		void RemoveFromSuperLayer ();

		[NullAllowed] // by default this property is null
		[Export ("sublayers", ArgumentSemantic.Copy)]
		CALayer [] Sublayers { get; set; }

		[Export ("addSublayer:")]
		[PostGet ("Sublayers")]
		void AddSublayer (CALayer layer);

		[Export ("insertSublayer:atIndex:")]
		[PostGet ("Sublayers")]
		void InsertSublayer (CALayer layer, int index);

		[Export ("insertSublayer:below:")]
		[PostGet ("Sublayers")]
		void InsertSublayerBelow (CALayer layer, [NullAllowed] CALayer sibling);

		[Export ("insertSublayer:above:")]
		[PostGet ("Sublayers")]
		void InsertSublayerAbove (CALayer layer, [NullAllowed] CALayer sibling);

		[Export ("replaceSublayer:with:")]
		[PostGet ("Sublayers")]
		void ReplaceSublayer (CALayer layer, CALayer with);

		[Export ("sublayerTransform")]
		CATransform3D SublayerTransform { get; set; }

		[Export ("mask", ArgumentSemantic.Strong)]
		[NullAllowed]
		CALayer Mask { get; set; }

		[Export ("masksToBounds")]
		bool MasksToBounds { get; set; }

		[Export ("convertPoint:fromLayer:")]
		CGPoint ConvertPointFromLayer (CGPoint point, [NullAllowed] CALayer layer);

		[Export ("convertPoint:toLayer:")]
		CGPoint ConvertPointToLayer (CGPoint point, [NullAllowed] CALayer layer);

		[Export ("convertRect:fromLayer:")]
		CGRect ConvertRectFromLayer (CGRect rect, [NullAllowed] CALayer layer);

		[Export ("convertRect:toLayer:")]
		CGRect ConvertRectToLayer (CGRect rect, [NullAllowed] CALayer layer);

		[Export ("convertTime:fromLayer:")]
		double ConvertTimeFromLayer (double timeInterval, [NullAllowed] CALayer layer);

		[Export ("convertTime:toLayer:")]
		double ConvertTimeToLayer (double timeInterval, [NullAllowed] CALayer layer);

		[Export ("hitTest:")]
		[return: NullAllowed]
		CALayer HitTest (CGPoint p);

		[Export ("containsPoint:")]
		bool Contains (CGPoint p);

		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		[Export ("contents", ArgumentSemantic.Strong), NullAllowed]
		CGImage Contents { get; set; }

		[Export ("contents", ArgumentSemantic.Strong)]
		[Internal]
		[Sealed]
		IntPtr _Contents { get; set; }

		[NoiOS]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("layoutManager", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSObject LayoutManager { get; set; }

		[Export ("contentsScale")]
		nfloat ContentsScale { get; set; }

		[Export ("contentsRect")]
		CGRect ContentsRect { get; set; }

		[Export ("contentsGravity", ArgumentSemantic.Copy)]
		string ContentsGravity { get; set; }

		[Export ("contentsCenter")]
		CGRect ContentsCenter { get; set; }

		[Export ("minificationFilter", ArgumentSemantic.Copy)]
		string MinificationFilter { get; set; }

		[Export ("magnificationFilter", ArgumentSemantic.Copy)]
		string MagnificationFilter { get; set; }

		[Export ("opaque")]
		bool Opaque { [Bind ("isOpaque")] get; set; }

		[Export ("display")]
		void Display ();

		[Export ("needsDisplay")]
		bool NeedsDisplay { get; }

		[Export ("setNeedsDisplay")]
		void SetNeedsDisplay ();

		[Export ("setNeedsDisplayInRect:")]
		void SetNeedsDisplayInRect (CGRect r);

		[Export ("displayIfNeeded")]
		void DisplayIfNeeded ();

		[Export ("needsDisplayOnBoundsChange")]
		bool NeedsDisplayOnBoundsChange { get; set; }

		[Export ("drawInContext:")]
		void DrawInContext (CGContext ctx);

		[Export ("renderInContext:")]
		void RenderInContext (CGContext ctx);

		[NullAllowed]
		[Export ("backgroundColor")]
		CGColor BackgroundColor { get; set; }

		[Export ("cornerRadius")]
		nfloat CornerRadius { get; set; }

		[Export ("borderWidth")]
		nfloat BorderWidth { get; set; }

		[Export ("borderColor")]
		[NullAllowed]
		CGColor BorderColor { get; set; }

		[Export ("opacity")]
		float Opacity { get; set; } /* float, not CGFloat */

		[Export ("edgeAntialiasingMask")]
		CAEdgeAntialiasingMask EdgeAntialiasingMask { get; set; }

		// Layout methods

		[Export ("preferredFrameSize")]
		CGSize PreferredFrameSize ();

		[Export ("setNeedsLayout")]
		void SetNeedsLayout ();

		[Export ("needsLayout")]
		bool NeedsLayout ();

		[Export ("layoutIfNeeded")]
		void LayoutIfNeeded ();

		[Export ("layoutSublayers")]
		void LayoutSublayers ();

		[Static]
		[Export ("defaultActionForKey:")]
		[return: NullAllowed]
		NSObject DefaultActionForKey (string eventKey);

		[Export ("actionForKey:")]
		[return: NullAllowed]
		NSObject ActionForKey (string eventKey);

		[NullAllowed] // by default this property is null
		[Export ("actions", ArgumentSemantic.Copy)]
		NSDictionary Actions { get; set; }

		[Export ("addAnimation:forKey:")]
		void AddAnimation (CAAnimation animation, [NullAllowed] string key);

		[Export ("removeAllAnimations")]
		void RemoveAllAnimations ();

		[Export ("removeAnimationForKey:")]
		void RemoveAnimation (string key);

		[Export ("animationKeys"), NullAllowed]
		string [] AnimationKeys { get; }

		[Export ("animationForKey:")]
		[return: NullAllowed]
		CAAnimation AnimationForKey (string key);

		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak)]
		[NullAllowed]
		NSObject WeakDelegate { get; [PostSnippet (@"SetCALayerDelegate (value as CALayerDelegate);", Optimizable = true)] set; }

		[Wrap ("WeakDelegate")]
		ICALayerDelegate Delegate { get; set; }

		[Export ("shadowColor")]
		[NullAllowed]
		CGColor ShadowColor { get; set; }

		[Export ("shadowOffset")]
		CGSize ShadowOffset { get; set; }

		[Export ("shadowOpacity")]
		float ShadowOpacity { get; set; } /* float, not CGFloat */

		[Export ("shadowRadius")]
		nfloat ShadowRadius { get; set; }

		[Field ("kCATransition")]
		NSString Transition { get; }

		[Field ("kCAGravityCenter")]
		NSString GravityCenter { get; }

		[Field ("kCAGravityTop")]
		NSString GravityTop { get; }

		[Field ("kCAGravityBottom")]
		NSString GravityBottom { get; }

		[Field ("kCAGravityLeft")]
		NSString GravityLeft { get; }

		[Field ("kCAGravityRight")]
		NSString GravityRight { get; }

		[Field ("kCAGravityTopLeft")]
		NSString GravityTopLeft { get; }

		[Field ("kCAGravityTopRight")]
		NSString GravityTopRight { get; }

		[Field ("kCAGravityBottomLeft")]
		NSString GravityBottomLeft { get; }

		[Field ("kCAGravityBottomRight")]
		NSString GravityBottomRight { get; }

		[Field ("kCAGravityResize")]
		NSString GravityResize { get; }

		[Field ("kCAGravityResizeAspect")]
		NSString GravityResizeAspect { get; }

		[Field ("kCAGravityResizeAspectFill")]
		NSString GravityResizeAspectFill { get; }

		[Field ("kCAFilterNearest")]
		NSString FilterNearest { get; }

		[Field ("kCAFilterLinear")]
		NSString FilterLinear { get; }

		[Field ("kCAFilterTrilinear")]
		NSString FilterTrilinear { get; }

		[Field ("kCAOnOrderIn")]
		NSString OnOrderIn { get; }

		[Field ("kCAOnOrderOut")]
		NSString OnOrderOut { get; }

		[MacCatalyst (13, 1)]
		[Internal]
		[Export ("contentsFormat")]
		NSString _ContentsFormat { get; set; }

		[Export ("visibleRect")]
		CGRect VisibleRect { get; }

		[Export ("scrollPoint:")]
		void ScrollPoint (CGPoint p);

		[Export ("scrollRectToVisible:")]
		void ScrollRectToVisible (CGRect r);

		[NullAllowed] // by default this property is null
		[Export ("filters", ArgumentSemantic.Copy)]
		CIFilter [] Filters { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("backgroundFilters", ArgumentSemantic.Copy)]
		CIFilter [] BackgroundFilters { get; set; }

		[Export ("style", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary Style { get; set; }

		[Export ("minificationFilterBias")]
		float MinificationFilterBias { get; set; } /* float, not CGFloat */

		[NoiOS]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("autoresizingMask")]
		CAAutoresizingMask AutoresizingMask { get; set; }

		[NoiOS]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("resizeSublayersWithOldSize:")]
		void ResizeSublayers (CGSize oldSize);

		[NoiOS]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("resizeWithOldSuperlayerSize:")]
		void Resize (CGSize oldSuperlayerSize);

		[NoiOS]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("constraints")]
		[NullAllowed]
		CAConstraint [] Constraints { get; set; }

		[NoiOS]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("addConstraint:")]
		void AddConstraint (CAConstraint c);

		[Export ("shouldRasterize")]
		bool ShouldRasterize { get; set; }

		[NullAllowed]
		[Export ("shadowPath")]
		CGPath ShadowPath { get; set; }

		[Export ("rasterizationScale")]
		nfloat RasterizationScale { get; set; }

		[Export ("drawsAsynchronously")]
		bool DrawsAsynchronously { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsEdgeAntialiasing")]
		bool AllowsEdgeAntialiasing { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsGroupOpacity")]
		bool AllowsGroupOpacity { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("compositingFilter", ArgumentSemantic.Strong)]
		NSObject CompositingFilter { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("maskedCorners", ArgumentSemantic.Assign)]
		CACornerMask MaskedCorners { get; set; }

		[BindAs (typeof (CACornerCurve))]
		[TV (13, 0)]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("cornerCurve")]
		NSString CornerCurve { get; set; }

		[TV (13, 0)]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("cornerCurveExpansionFactor:")]
		nfloat GetCornerCurveExpansionFactor ([BindAs (typeof (CACornerCurve))] NSString curve);

		[NoTV]
		[iOS (17, 0)]
		[MacCatalyst (17, 0)]
		[Mac (14, 0)]
		[Export ("wantsExtendedDynamicRangeContent")]
		bool WantsExtendedDynamicRangeContent { get; set; }

		[Mac (15, 0), iOS (18, 0), TV (18, 0), MacCatalyst (18, 0)]
		[Export ("toneMapMode")]
		[BindAs (typeof (CAToneMapMode))]
		NSString ToneMapMode { get; set; }
	}

	[TV (13, 0)]
	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	enum CACornerCurve {
		[DefaultEnumValue]
		[Field ("kCACornerCurveCircular")]
		Circular,
		[Field ("kCACornerCurveContinuous")]
		Continuous,
	}

	[Mac (15, 0), iOS (18, 0), TV (18, 0), MacCatalyst (18, 0)]
	enum CAToneMapMode {
		[DefaultEnumValue]
		[Field ("CAToneMapModeAutomatic")]
		Automatic,
		[Field ("CAToneMapModeNever")]
		Never,
		[Field ("CAToneMapModeIfSupported")]
		IfSupported,
	}

	interface ICAMetalDrawable { }

	/// <summary>Interface that defines a protocol for a display buffer at the metal layer.</summary>
	[Protocol]
	[MacCatalyst (13, 1)]
	interface CAMetalDrawable : MTLDrawable {
		[Abstract]
		[Export ("texture")]
		IMTLTexture Texture { get; }

		[Abstract]
		[Export ("layer")]
		CAMetalLayer Layer { get; }
	}

	/// <summary>A <see cref="T:CoreAnimation.CALayer" /> that is rendered using Metal functions.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Animation/Reference/CAMetalLayer_Ref/index.html">Apple documentation for <c>CAMetalLayer</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CALayer))]
	interface CAMetalLayer {
		[NullAllowed] // by default this property is null
		[Export ("device")]
		IMTLDevice Device { get; set; }

		[Export ("pixelFormat")]
		MTLPixelFormat PixelFormat { get; set; }

		[Export ("framebufferOnly")]
		bool FramebufferOnly { get; set; }

		[Export ("drawableSize")]
		CGSize DrawableSize { get; set; }

		[Export ("nextDrawable")]
		[return: NullAllowed]
		ICAMetalDrawable NextDrawable ();

		[Export ("presentsWithTransaction")]
		bool PresentsWithTransaction { [Bind ("presentsWithTransaction")] get; set; }

		[NoTV]
		[NoiOS]
		[MacCatalyst (13, 1)]
		[Export ("displaySyncEnabled")]
		bool DisplaySyncEnabled { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsNextDrawableTimeout")]
		bool AllowsNextDrawableTimeout { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("maximumDrawableCount")]
		nuint MaximumDrawableCount { get; set; }

		[TV (13, 0)]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("colorspace", ArgumentSemantic.Assign)]
		CGColorSpace ColorSpace { get; set; }

		[TV (13, 0)]
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("preferredDevice")]
		IMTLDevice PreferredDevice { get; }

		[iOS (16, 0)]
		[NoTV]
		[MacCatalyst (16, 0)]
		[NullAllowed, Export ("EDRMetadata", ArgumentSemantic.Strong)]
		CAEdrMetadata EdrMetadata { get; set; }

		[NoTV]
		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("wantsExtendedDynamicRangeContent")]
		bool WantsExtendedDynamicRangeContent { get; set; }

		[TV (16, 0)]
		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Mac (13, 0)]
		[Export ("developerHUDProperties", ArgumentSemantic.Copy)]
		[NullAllowed]
		// There's no documentation about which values are valid in this dictionary, so we can't create any strong bindings for it.
		NSDictionary DeveloperHudProperties { get; set; }
	}

	/// <summary>Layer whose content can be provided asynchronously, and with multiple levels of detail.</summary>
	///     <remarks>
	///       <para>
	///    When you want to use one of the CALayer subclasses as your UIView's backing layer, you need to add the following code snippet to your class:
	/// </para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	/// class MyView : UIView {
	///     //
	///     // This instructs the runtime that whenever a MyView is created
	///     // that it should instantiate a CATiledLayer and assign that to the
	///     // UIView.Layer property
	///     //
	///     [Export ("layerClass")]
	///     public static Class LayerClass () {
	///         return new Class (typeof (CATilerLayer));
	///     }
	/// }
	///   ]]></code>
	///       </example>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CATiledLayer_class/index.html">Apple documentation for <c>CATiledLayer</c></related>
	[BaseType (typeof (CALayer))]
	interface CATiledLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[Static]
		[Export ("fadeDuration")]
		double FadeDuration { get; }

		[Export ("levelsOfDetail")]
		nint LevelsOfDetail { get; set; }

		[Export ("levelsOfDetailBias")]
		nint LevelsOfDetailBias { get; set; }

		[Export ("tileSize")]
		CGSize TileSize { get; set; }
	}

	/// <summary>A layer that replicates an existing layer, with some attributes (color, transform) altered.</summary>
	///     <remarks>
	///       <para>
	///    When you want to use one of the CALayer subclasses as your UIView's backing layer, you need to add the following code snippet to your class:
	/// </para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	/// class MyView : UIView {
	///     //
	///     // This instructs the runtime that whenever a MyView is created
	///     // that it should instantiate a CAReplicatorLayer and assign that to the
	///     // UIView.Layer property
	///     //
	///     [Export ("layerClass")]
	///     public static Class LayerClass () {
	///         return new Class (typeof (CAReplicatorLayer));
	///     }
	/// }
	///   ]]></code>
	///       </example>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAReplicatorLayer_class/index.html">Apple documentation for <c>CAReplicatorLayer</c></related>
	[BaseType (typeof (CALayer))]
	interface CAReplicatorLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[Export ("instanceCount")]
		nint InstanceCount { get; set; }

		[Export ("instanceDelay")]
		double InstanceDelay { get; set; }

		[Export ("instanceTransform")]
		CATransform3D InstanceTransform { get; set; }

		[Export ("preservesDepth")]
		bool PreservesDepth { get; set; }

		[Export ("instanceColor")]
		[NullAllowed]
		CGColor InstanceColor { get; set; }

		[Export ("instanceRedOffset")]
		float InstanceRedOffset { get; set; } /* float, not CGFloat */

		[Export ("instanceGreenOffset")]
		float InstanceGreenOffset { get; set; } /* float, not CGFloat */

		[Export ("instanceBlueOffset")]
		float InstanceBlueOffset { get; set; } /* float, not CGFloat */

		[Export ("instanceAlphaOffset")]
		float InstanceAlphaOffset { get; set; } /* float, not CGFloat */
	}


	/// <summary>Layer used to show portions of another layer.</summary>
	///     <remarks>
	///       <para>
	///    When you want to use one of the CALayer subclasses as your UIView's backing layer, you need to add the following code snippet to your class:
	/// </para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	/// class MyView : UIView {
	///     //
	///     // This instructs the runtime that whenever a MyView is created
	///     // that it should instantiate a CAScrollLayer and assign that to the
	///     // UIView.Layer property
	///     //
	///     [Export ("layerClass")]
	///     public static Class LayerClass () {
	///         return new Class (typeof (CAScrollLayer));
	///     }
	/// }
	///   ]]></code>
	///       </example>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAScrollLayer_class/index.html">Apple documentation for <c>CAScrollLayer</c></related>
	[BaseType (typeof (CALayer))]
	interface CAScrollLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

#if NET
		[Protected]
		[Export ("scrollMode", ArgumentSemantic.Copy)]
		NSString WeakScrollMode { get; set; }

		CAScroll ScrollMode {
			[Wrap ("CAScrollExtensions.GetValue (WeakScrollMode)")]
			get;
			[Wrap ("WeakScrollMode = value.GetConstant ()!")]
			set;
		}
#else
		[Export ("scrollMode", ArgumentSemantic.Copy)]
		NSString ScrollMode { get; set; }
#endif

		[Export ("scrollToPoint:")]
		void ScrollToPoint (CGPoint p);

		[Export ("scrollToRect:")]
		void ScrollToRect (CGRect r);
	}

	/// <summary>Enumerates scrolling directions.</summary>
	enum CAScroll {

		[Field ("kCAScrollNone")]
		None,

		[Field ("kCAScrollVertically")]
		Vertically,

		[Field ("kCAScrollHorizontally")]
		Horizontally,

		[Field ("kCAScrollBoth")]
		Both,
	}

	/// <summary>Draws a bezier curve and composes the result with its first sublayer.</summary>
	///     <remarks>
	///       <para>
	///    When you want to use one of the CALayer subclasses as your UIView's backing layer, you need to add the following code snippet to your class:
	/// </para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	/// class MyView : UIView {
	///     //
	///     // This instructs the runtime that whenever a MyView is created
	///     // that it should instantiate a CAShapeLayer and assign that to the
	///     // UIView.Layer property
	///     //
	///     [Export ("layerClass")]
	///     public static Class LayerClass () {
	///         return new Class (typeof (CAShapeLayer));
	///     }
	/// }
	///   ]]></code>
	///       </example>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAShapeLayer_class/index.html">Apple documentation for <c>CAShapeLayer</c></related>
	[BaseType (typeof (CALayer))]
	interface CAShapeLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[Export ("path")]
		[NullAllowed]
		CGPath Path { get; set; }

		[Export ("fillColor")]
		[NullAllowed]
		CGColor FillColor { get; set; }

		[Export ("fillRule", ArgumentSemantic.Copy)]
		NSString FillRule { get; set; }

		[Export ("lineCap", ArgumentSemantic.Copy)]
		NSString LineCap { get; set; }

		[Export ("lineDashPattern", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSNumber [] LineDashPattern { get; set; }

		[Export ("lineDashPhase")]
		nfloat LineDashPhase { get; set; }

		[Export ("lineJoin", ArgumentSemantic.Copy)]
		NSString LineJoin { get; set; }

		[Export ("lineWidth")]
		nfloat LineWidth { get; set; }

		[Export ("miterLimit")]
		nfloat MiterLimit { get; set; }

		[Export ("strokeColor")]
		[NullAllowed]
		CGColor StrokeColor { get; set; }

		[Export ("strokeStart")]
		nfloat StrokeStart { get; set; }

		[Export ("strokeEnd")]
		nfloat StrokeEnd { get; set; }

		[Field ("kCALineJoinMiter")]
		NSString JoinMiter { get; }

		[Field ("kCALineJoinRound")]
		NSString JoinRound { get; }

		[Field ("kCALineJoinBevel")]
		NSString JoinBevel { get; }

		[Field ("kCALineCapButt")]
		NSString CapButt { get; }

		[Field ("kCALineCapRound")]
		NSString CapRound { get; }

		[Field ("kCALineCapSquare")]
		NSString CapSquare { get; }

		[Field ("kCAFillRuleNonZero")]
		NSString FillRuleNonZero { get; }

		[Field ("kCAFillRuleEvenOdd")]
		NSString FillRuleEvenOdd { get; }
	}

	/// <summary>3D compositing layer.</summary>
	///     <remarks>
	///       <para>
	///    When you want to use one of the CALayer subclasses as your UIView's backing layer, you need to add the following code snippet to your class:
	/// </para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	/// class MyView : UIView {
	///     //
	///     // This instructs the runtime that whenever a MyView is created
	///     // that it should instantiate a CATransformLayer and assign that to the
	///     // UIView.Layer property
	///     //
	///     [Export ("layerClass")]
	///     public static Class LayerClass () {
	///         return new Class (typeof (CATransformLayer));
	///     }
	/// }
	///   ]]></code>
	///       </example>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CATransformLayer_class/index.html">Apple documentation for <c>CATransformLayer</c></related>
	[BaseType (typeof (CALayer))]
	interface CATransformLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[Export ("hitTest:")]
		CALayer HitTest (CGPoint thePoint);
	}

	enum CATextLayerTruncationMode {
		[Field ("kCATruncationNone")]
		None,

		[Field ("kCATruncationStart")]
		Start,

		[Field ("kCATruncationMiddle")]
		Middle,

		[Field ("kCATruncationEnd")]
		End,
	}

	enum CATextLayerAlignmentMode {
		[Field ("kCAAlignmentLeft")]
		Left,

		[Field ("kCAAlignmentRight")]
		Right,

		[Field ("kCAAlignmentCenter")]
		Center,

		[Field ("kCAAlignmentJustified")]
		Justified,

		[Field ("kCAAlignmentNatural")]
		Natural,
	}

	/// <summary>Simple text layour and rendering of regular or attributed text.</summary>
	///     <remarks>
	///       <para>
	///    When you want to use one of the CALayer subclasses as your UIView's backing layer, you need to add the following code snippet to your class:
	/// </para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	/// class MyView : UIView {
	///     //
	///     // This instructs the runtime that whenever a MyView is created
	///     // that it should instantiate a CATextLayer and assign that to the
	///     // UIView.Layer property
	///     //
	///     [Export ("layerClass")]
	///     public static Class LayerClass () {
	///         return new Class (typeof (CATextLayer));
	///     }
	/// }
	///   ]]></code>
	///       </example>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CATextLayer_class/index.html">Apple documentation for <c>CATextLayer</c></related>
	[BaseType (typeof (CALayer))]
	interface CATextLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[NullAllowed] // by default this property is null
		[Export ("string", ArgumentSemantic.Copy)]
		string String { get; set; }

		[Sealed]
		[Internal]
		[NullAllowed] // by default this property is null
		[Export ("string", ArgumentSemantic.Copy)]
		IntPtr _AttributedString { get; set; }

		[Export ("fontSize")]
		nfloat FontSize { get; set; }

		[Export ("font"), Internal]
		IntPtr _Font { get; set; }

		[Export ("foregroundColor")]
		[NullAllowed]
		CGColor ForegroundColor { get; set; }

		[Export ("wrapped")]
		bool Wrapped { [Bind ("isWrapped")] get; set; }

		[Protected]
		[Export ("truncationMode", ArgumentSemantic.Copy)]
		NSString WeakTruncationMode { get; set; }

		[Protected]
		[Export ("alignmentMode", ArgumentSemantic.Copy)]
		NSString WeakAlignmentMode { get; set; }

#if !NET // Use smart enums instead, CATruncationMode and CATextLayerAlignmentMode.
		[Obsolete ("Use 'CATextLayerTruncationMode.None.GetConstant ()' instead.")]
		[Static]
		[Wrap ("CATextLayerTruncationMode.None.GetConstant ()")]
		NSString TruncationNone { get; }

		[Obsolete ("Use 'CATextLayerTruncationMode.Start.GetConstant ()' instead.")]
		[Static]
		[Wrap ("CATextLayerTruncationMode.Start.GetConstant ()")]
		NSString TruncantionStart { get; }

		[Obsolete ("Use 'CATextLayerTruncationMode.End.GetConstant ()' instead.")]
		[Static]
		[Wrap ("CATextLayerTruncationMode.End.GetConstant ()")]
		NSString TruncantionEnd { get; }

		[Obsolete ("Use 'CATextLayerTruncationMode.Middle.GetConstant ()' instead.")]
		[Static]
		[Wrap ("CATextLayerTruncationMode.Middle.GetConstant ()")]
		NSString TruncantionMiddle { get; }

		[Obsolete ("Use 'CATextLayerAlignmentMode.Natural.GetConstant ()' instead.")]
		[Static]
		[Wrap ("CATextLayerAlignmentMode.Natural.GetConstant ()")]
		NSString AlignmentNatural { get; }

		[Obsolete ("Use 'CATextLayerAlignmentMode.Left.GetConstant ()' instead.")]
		[Static]
		[Wrap ("CATextLayerAlignmentMode.Left.GetConstant ()")]
		NSString AlignmentLeft { get; }

		[Obsolete ("Use 'CATextLayerAlignmentMode.Right.GetConstant ()' instead.")]
		[Static]
		[Wrap ("CATextLayerAlignmentMode.Right.GetConstant ()")]
		NSString AlignmentRight { get; }

		[Obsolete ("Use 'CATextLayerAlignmentMode.Center.GetConstant ()' instead.")]
		[Static]
		[Wrap ("CATextLayerAlignmentMode.Center.GetConstant ()")]
		NSString AlignmentCenter { get; }

		[Obsolete ("Use 'CATextLayerAlignmentMode.Justified.GetConstant ()' instead.")]
		[Static]
		[Wrap ("CATextLayerAlignmentMode.Justified.GetConstant ()")]
		NSString AlignmentJustified { get; }
#endif // !NET

		[MacCatalyst (13, 1)]
		[Export ("allowsFontSubpixelQuantization")]
		bool AllowsFontSubpixelQuantization { get; set; }
	}

	/// <summary>Interface representing the required methods (if any) of the protocol <see cref="T:CoreAnimation.CALayerDelegate" />.</summary>
	///     <remarks>
	///       <para>This interface contains the required methods (if any) from the protocol defined by <see cref="T:CoreAnimation.CALayerDelegate" />.</para>
	///       <para>If developers create classes that implement this interface, the implementation methods will automatically be exported to Objective-C with the matching signature from the method defined in the <see cref="T:CoreAnimation.CALayerDelegate" /> protocol.</para>
	///       <para>Optional methods (if any) are provided by the <see cref="T:CoreAnimation.CALayerDelegate_Extensions" /> class as extension methods to the interface, allowing developers to invoke any optional methods on the protocol.</para>
	///     </remarks>
	interface ICALayerDelegate { }

	/// <summary>Delegate class for the CALayer.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CoreAnimation/CALayerDelegate">Apple documentation for <c>CALayerDelegate</c></related>
	[BaseType (typeof (NSObject))]
	[Model]
#if IOS || TVOS
	[Protocol (FormalSince = "10.0")]
#elif MONOMAC
	[Protocol (FormalSince = "10.12")]
#else
	[Protocol]
#endif
	interface CALayerDelegate {
		[Export ("displayLayer:")]
		void DisplayLayer (CALayer layer);

		[Export ("drawLayer:inContext:"), EventArgs ("CALayerDrawEventArgs")]
		void DrawLayer (CALayer layer, CGContext context);

		[MacCatalyst (13, 1)]
		[Export ("layerWillDraw:")]
		void WillDrawLayer (CALayer layer);

		[Export ("layoutSublayersOfLayer:")]
		void LayoutSublayersOfLayer (CALayer layer);

		[Export ("actionForLayer:forKey:"), EventArgs ("CALayerDelegateAction"), DefaultValue (null)]
		[return: NullAllowed]
		NSObject ActionForLayer (CALayer layer, string eventKey);
	}

#if HAS_OPENGLES
	/// <summary>Layer
	///  used to render OpenGL content.</summary>
	///     <remarks>
	///       <para>
	///    When you want to use one of the CALayer subclasses as your UIView's backing layer, you need to add the following code snippet to your class:
	/// </para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	/// class MyView : UIView {
	///     //
	///     // This instructs the runtime that whenever a MyView is created
	///     // that it should instantiate a CAEAGLLayer and assign that to the
	///     // UIView.Layer property
	///     //
	///     [Export ("layerClass")]
	///     public static Class LayerClass () {
	///         return new Class (typeof (CAEAGLLayer));
	///     }
	/// }
	///   ]]></code>
	///       </example>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/QuartzCore/Reference/CAEAGLLayer_Class/index.html">Apple documentation for <c>CAEAGLLayer</c></related>
	[NoMac]
	[NoMacCatalyst]
	[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'CAMetalLayer' instead.")]
	[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'CAMetalLayer' instead.")]
	[BaseType (typeof (CALayer))]
	interface CAEAGLLayer : EAGLDrawable {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[Export ("presentsWithTransaction")]
		bool PresentsWithTransaction { get; set; }
	}
#endif

	/// <summary>An interface implemented by objects that participate in animations coordinated by a CALayer.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAAction_protocol/index.html">Apple documentation for <c>CAAction</c></related>
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[DisableDefaultCtor]
	interface CAAction {
		[Abstract]
		[Export ("runActionForKey:object:arguments:")]
		void RunAction (string eventKey, NSObject obj, [NullAllowed] NSDictionary arguments);
	}

	/// <include file="../docs/api/CoreAnimation/CAAnimation.xml" path="/Documentation/Docs[@DocId='T:CoreAnimation.CAAnimation']/*" />
	[BaseType (typeof (NSObject)
#if NET
		, Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (CAAnimationDelegate) }
#endif
	)]
	interface CAAnimation : CAAction, CAMediaTiming, NSSecureCoding, NSMutableCopying, SCNAnimationProtocol {
		[Export ("animation"), Static]
		CAAnimation CreateAnimation ();

		[Static]
		[Export ("defaultValueForKey:")]
		[return: NullAllowed]
		NSObject DefaultValue (string key);

		[NullAllowed] // by default this property is null
		[Export ("timingFunction", ArgumentSemantic.Strong)]
		CAMediaTimingFunction TimingFunction { get; set; }

#if NET
		// before that we need to be wrap this manually to avoid the BI1110 error
		[Wrap ("WeakDelegate")]
		ICAAnimationDelegate Delegate { get; set; }
#endif

		[Export ("delegate", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("removedOnCompletion")]
		bool RemovedOnCompletion { [Bind ("isRemovedOnCompletion")] get; set; }

		[Export ("willChangeValueForKey:")]
		void WillChangeValueForKey (string key);

		[Export ("didChangeValueForKey:")]
		void DidChangeValueForKey (string key);

		[Export ("shouldArchiveValueForKey:")]
		bool ShouldArchiveValueForKey (string key);

		[Field ("kCATransitionFade")]
		NSString TransitionFade { get; }

		[Field ("kCATransitionMoveIn")]
		NSString TransitionMoveIn { get; }

		[Field ("kCATransitionPush")]
		NSString TransitionPush { get; }

		[Field ("kCATransitionReveal")]
		NSString TransitionReveal { get; }

		[Field ("kCATransitionFromRight")]
		NSString TransitionFromRight { get; }

		[Field ("kCATransitionFromLeft")]
		NSString TransitionFromLeft { get; }

		[Field ("kCATransitionFromTop")]
		NSString TransitionFromTop { get; }

		[Field ("kCATransitionFromBottom")]
		NSString TransitionFromBottom { get; }

		/* 'calculationMode' strings. */
		[Field ("kCAAnimationLinear")]
		NSString AnimationLinear { get; }

#if !NET
		[Field ("kCAAnimationDiscrete")]
		[Obsolete ("The name has been fixed, use 'AnimationDiscrete' instead.")]
		NSString AnimationDescrete { get; }
#endif
		[Field ("kCAAnimationDiscrete")]
		NSString AnimationDiscrete { get; }

		[Field ("kCAAnimationPaced")]
		NSString AnimationPaced { get; }

		[Field ("kCAAnimationCubic")]
		NSString AnimationCubic { get; }

		[Field ("kCAAnimationCubicPaced")]
		NSString AnimationCubicPaced { get; }

		/* 'rotationMode' strings. */
		[Field ("kCAAnimationRotateAuto")]
		NSString RotateModeAuto { get; }

		[Field ("kCAAnimationRotateAutoReverse")]
		NSString RotateModeAutoReverse { get; }

		#region SceneKitAdditions

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("animationWithSCNAnimation:")]
		CAAnimation FromSCNAnimation (SCNAnimation animation);

		[MacCatalyst (13, 1)]
		[Export ("usesSceneTimeBase")]
		bool UsesSceneTimeBase { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("fadeInDuration")]
		nfloat FadeInDuration { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("fadeOutDuration")]
		nfloat FadeOutDuration { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("animationEvents", ArgumentSemantic.Retain)]
		SCNAnimationEvent [] AnimationEvents { get; set; }

		#endregion

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("preferredFrameRateRange", ArgumentSemantic.Assign)]
		CAFrameRateRange PreferredFrameRateRange { get; set; }
	}

	interface ICAAnimationDelegate { }

	/// <summary>Delegate for responding to animation lifecycle events.</summary>
	[BaseType (typeof (NSObject))]
#if IOS || TVOS
	[Protocol (FormalSince = "10.0")]
#elif MONOMAC
	[Protocol (FormalSince = "10.12")]
#else
	[Synthetic]
#endif
	[Model]
	interface CAAnimationDelegate {
		[Export ("animationDidStart:")]
		void AnimationStarted (CAAnimation anim);

		[Export ("animationDidStop:finished:"), EventArgs ("CAAnimationState")]
		void AnimationStopped (CAAnimation anim, bool finished);

	}

	/// <summary>An animation that can animate object properties.</summary>
	///     <remarks>For a list of common properties to animate, see the documentation for <see cref="P:CoreAnimation.CAPropertyAnimation.KeyPath" /></remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAPropertyAnimation_class/index.html">Apple documentation for <c>CAPropertyAnimation</c></related>
	[BaseType (typeof (CAAnimation))]
	interface CAPropertyAnimation {
		[Static]
		[Export ("animationWithKeyPath:")]
		CAPropertyAnimation FromKeyPath ([NullAllowed] string path);

		[NullAllowed] // by default this property is null
		[Export ("keyPath", ArgumentSemantic.Copy)]
		string KeyPath { get; set; }

		[Export ("additive")]
		bool Additive { [Bind ("isAdditive")] get; set; }

		[Export ("cumulative")]
		bool Cumulative { [Bind ("isCumulative")] get; set; }

		[NullAllowed] // by default this property is null
		[Export ("valueFunction", ArgumentSemantic.Strong)]
		CAValueFunction ValueFunction { get; set; }
	}

	/// <include file="../docs/api/CoreAnimation/CABasicAnimation.xml" path="/Documentation/Docs[@DocId='T:CoreAnimation.CABasicAnimation']/*" />
	[BaseType (typeof (CAPropertyAnimation))]
	interface CABasicAnimation {
		[Static, New, Export ("animationWithKeyPath:")]
		CABasicAnimation FromKeyPath ([NullAllowed] string path);

		[Export ("fromValue", ArgumentSemantic.Strong)]
		[Internal]
		[Sealed]
		IntPtr _From { get; set; }

		[Export ("fromValue", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSObject From { get; set; }

		[Export ("toValue", ArgumentSemantic.Strong)]
		[Internal]
		[Sealed]
		IntPtr _To { get; set; }

		[Export ("toValue", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSObject To { get; set; }

		[Export ("byValue", ArgumentSemantic.Strong)]
		[Internal]
		[Sealed]
		IntPtr _By { get; set; }

		[Export ("byValue", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSObject By { get; set; }
	}

	/// <summary>A spring animation with stiffness, mass, and damping.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/reference/CoreAnimation/CASpringAnimation">Apple documentation for <c>CASpringAnimation</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (CABasicAnimation))]
	interface CASpringAnimation {
		[Static, New, Export ("animationWithKeyPath:")]
		CABasicAnimation FromKeyPath ([NullAllowed] string path);

		[Export ("mass")]
		nfloat Mass { get; set; }

		[Export ("stiffness")]
		nfloat Stiffness { get; set; }

		[Export ("damping")]
		nfloat Damping { get; set; }

		[Export ("initialVelocity")]
		nfloat InitialVelocity { get; set; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("settlingDuration")]
		double /* CFTimeInterval */ SettlingDuration { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("allowsOverdamping")]
		bool AllowsOverdamping { get; set; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("initWithPerceptualDuration:bounce:")]
		NativeHandle Constructor (double /* CFTimeInterval */ perceptualDuration, nfloat bounce);

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("perceptualDuration")]
		double /* CFTimeInterval */ PerceptualDuration { get; }

		[iOS (17, 0), TV (17, 0), MacCatalyst (17, 0), Mac (14, 0)]
		[Export ("bounce")]
		nfloat Bounce { get; }
	}

	/// <summary>Keyframe-based animation support.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAKeyframeAnimation_class/index.html">Apple documentation for <c>CAKeyframeAnimation</c></related>
	[BaseType (typeof (CAPropertyAnimation), Name = "CAKeyframeAnimation")]
	interface CAKeyFrameAnimation {
		[Static, Export ("animationWithKeyPath:")]
		CAKeyFrameAnimation FromKeyPath ([NullAllowed] string path);

		[NullAllowed] // by default this property is null
		[Export ("values", ArgumentSemantic.Copy)]
		NSObject [] Values { get; set; }

		[Export ("values", ArgumentSemantic.Strong)]
		[Internal]
		[Sealed]
		NSArray _Values { get; set; }

		[NullAllowed]
		[Export ("path")]
		CGPath Path { get; set; }

		[Export ("keyTimes", ArgumentSemantic.Copy)]
		[NullAllowed]
		NSNumber [] KeyTimes { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("timingFunctions", ArgumentSemantic.Copy)]
		CAMediaTimingFunction [] TimingFunctions { get; set; }

		[Export ("calculationMode", ArgumentSemantic.Copy)]
		[Internal]
		NSString _CalculationMode { get; set; }

		[Export ("rotationMode", ArgumentSemantic.Copy)]
		[NullAllowed]
		string RotationMode { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("tensionValues", ArgumentSemantic.Copy)]
		NSNumber [] TensionValues { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("continuityValues", ArgumentSemantic.Copy)]
		NSNumber [] ContinuityValues { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("biasValues", ArgumentSemantic.Copy)]
		NSNumber [] BiasValues { get; set; }
	}

	/// <summary>Transition animations for a layer.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CATransition_class/index.html">Apple documentation for <c>CATransition</c></related>
	[BaseType (typeof (CAAnimation))]
	interface CATransition {
		[Export ("animation"), Static, New]
		CATransition CreateAnimation ();

		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("subtype", ArgumentSemantic.Copy)]
		string Subtype { get; set; }

		[Export ("startProgress")]
		float StartProgress { get; set; } /* float, not CGFloat */

		[Export ("endProgress")]
		float EndProgress { get; set; } /* float, not CGFloat */

		[Export ("filter", ArgumentSemantic.Strong)]
		[NullAllowed]
		NSObject Filter { get; set; }
	}

	/// <summary>Constants used for the FillMode property in CAAnimation and CALayer, used to control the behavior of objects once the animation has completed.</summary>
	///     <remarks>These are used in the FillMode property of CAAnimation and CALayer.</remarks>
	[Static]
	interface CAFillMode {
		[Field ("kCAFillModeForwards")]
		NSString Forwards { get; }

		[Field ("kCAFillModeBackwards")]
		NSString Backwards { get; }

		[Field ("kCAFillModeBoth")]
		NSString Both { get; }

		[Field ("kCAFillModeRemoved")]
		NSString Removed { get; }
	}

	/// <summary>Framework to synchronize multiple transformation operations.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CATransaction_class/index.html">Apple documentation for <c>CATransaction</c></related>
	[BaseType (typeof (NSObject))]
	interface CATransaction {
		[Static]
		[Export ("begin")]
		void Begin ();

		[Static]
		[Export ("commit")]
		void Commit ();

		[Static]
		[Export ("flush")]
		void Flush ();

		[Static]
		[Export ("lock")]
		void Lock ();

		[Static]
		[Export ("unlock")]
		void Unlock ();

		[Static]
		[Export ("animationDuration")]
		double AnimationDuration { get; set; }

		[Static, NullAllowed]
		[Export ("animationTimingFunction")]
		CAMediaTimingFunction AnimationTimingFunction { get; set; }

		[Static]
		[Export ("disableActions")]
		bool DisableActions { get; set; }

		[Static]
		[Export ("valueForKey:")]
		[return: NullAllowed]
		NSObject ValueForKey (NSString key);

		[Static]
		[Export ("setValue:forKey:")]
		void SetValueForKey ([NullAllowed] NSObject anObject, NSString key);

		[Static, Export ("completionBlock"), NullAllowed]
		Action CompletionBlock { get; set; }

		[Field ("kCATransactionAnimationDuration")]
		NSString AnimationDurationKey { get; }

		[Field ("kCATransactionDisableActions")]
		NSString DisableActionsKey { get; }

		[Field ("kCATransactionAnimationTimingFunction")]
		NSString TimingFunctionKey { get; }

		[Field ("kCATransactionCompletionBlock")]
		NSString CompletionBlockKey { get; }
	}

	/// <summary>Groups and orchestrates multiple animations.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAAnimationGroup_class/index.html">Apple documentation for <c>CAAnimationGroup</c></related>
	[BaseType (typeof (CAAnimation))]
	interface CAAnimationGroup {
		[NullAllowed] // by default this property is null
		[Export ("animations", ArgumentSemantic.Copy)]
		CAAnimation [] Animations { get; set; }

		[Export ("animation"), Static, New]
		CAAnimationGroup CreateAnimation ();
	}

	/// <summary>Layer that renders a gradient over its background.</summary>
	///     <remarks>
	///       <para>
	///    When you want to use one of the CALayer subclasses as your UIView's backing layer, you need to add the following code snippet to your class:
	/// </para>
	///       <example>
	///         <code lang="csharp lang-csharp"><![CDATA[
	/// class MyView : UIView {
	///     //
	///     // This instructs the runtime that whenever a MyView is created
	///     // that it should instantiate a CAGradientLayer and assign that to the
	///     // UIView.Layer property
	///     //
	///     [Export ("layerClass")]
	///     public static Class LayerClass () {
	///         return new Class (typeof (CAGradientLayer));
	///     }
	/// }
	///   ]]></code>
	///       </example>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAGradientLayer_class/index.html">Apple documentation for <c>CAGradientLayer</c></related>
	[BaseType (typeof (CALayer))]
	interface CAGradientLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[NullAllowed] // by default this property is null
		[Export ("colors", ArgumentSemantic.Copy)]
		[Internal]
		IntPtr _Colors { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("locations", ArgumentSemantic.Copy)]
		NSNumber [] Locations { get; set; }

		[Export ("startPoint")]
		CGPoint StartPoint { get; set; }

		[Export ("endPoint")]
		CGPoint EndPoint { get; set; }

#if NET
		CAGradientLayerType LayerType {
			[Wrap ("CAGradientLayerTypeExtensions.GetValue (WeakLayerType)")]
			get;
			[Wrap ("WeakLayerType = value.GetConstant ()!")]
			set;
		}

		[Export ("type", ArgumentSemantic.Copy)]
		NSString WeakLayerType { get; set; }
#else
		CAGradientLayerType LayerType {
			[Wrap ("CAGradientLayerTypeExtensions.GetValue ((NSString) Type)")]
			get;
			[Wrap ("Type = value.GetConstant ()")]
			set;
		}

		[Obsolete ("Use 'LayerType' property instead.")]
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; set; }

		[Obsolete ("Use 'CAGradientLayerType.Axial' enum instead.")]
		[Field ("kCAGradientLayerAxial")]
		NSString GradientLayerAxial { get; }
#endif
	}

	enum CAGradientLayerType {
		[Field ("kCAGradientLayerAxial")]
		Axial,

		[MacCatalyst (13, 1)]
		[Field ("kCAGradientLayerRadial")]
		Radial,

		[MacCatalyst (13, 1)]
		[Field ("kCAGradientLayerConic")]
		Conic,
	}

	/// <summary>Defines the pacing of an animation.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Cocoa/Reference/CAMediaTimingFunction_class/index.html">Apple documentation for <c>CAMediaTimingFunction</c></related>
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CAMediaTimingFunction : NSSecureCoding {
		[Export ("functionWithName:")]
		[Static]
		CAMediaTimingFunction FromName (NSString name);

		[Static]
		[Export ("functionWithControlPoints::::")]
		CAMediaTimingFunction FromControlPoints (float c1x, float c1y, float c2x, float c2y); /* all float, not CGFloat */

		[Export ("initWithControlPoints::::")]
		NativeHandle Constructor (float c1x, float c1y, float c2x, float c2y); /* all float, not CGFloat */

		[Export ("getControlPointAtIndex:values:"), Internal]
		void GetControlPointAtIndex (nint idx, IntPtr /* float[2] */ point);

		[Field ("kCAMediaTimingFunctionLinear")]
		NSString Linear { get; }

		[Field ("kCAMediaTimingFunctionEaseIn")]
		NSString EaseIn { get; }

		[Field ("kCAMediaTimingFunctionEaseOut")]
		NSString EaseOut { get; }

		[Field ("kCAMediaTimingFunctionEaseInEaseOut")]
		NSString EaseInEaseOut { get; }

		[Field ("kCAMediaTimingFunctionDefault")]
		NSString Default { get; }
	}

	/// <summary>Class used to apply functions to property values during an animation.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAValueFunction_class/index.html">Apple documentation for <c>CAValueFunction</c></related>
	[BaseType (typeof (NSObject))]
	interface CAValueFunction : NSSecureCoding {
		[Export ("functionWithName:"), Static]
		[return: NullAllowed]
		CAValueFunction FromName (string name);

		[Export ("name")]
		string Name { get; }

		[Field ("kCAValueFunctionRotateX")]
		NSString RotateX { get; }

		[Field ("kCAValueFunctionRotateY")]
		NSString RotateY { get; }

		[Field ("kCAValueFunctionRotateZ")]
		NSString RotateZ { get; }

		[Field ("kCAValueFunctionScale")]
		NSString Scale { get; }

		[Field ("kCAValueFunctionScaleX")]
		NSString ScaleX { get; }

		[Field ("kCAValueFunctionScaleY")]
		NSString ScaleY { get; }

		[Field ("kCAValueFunctionScaleZ")]
		NSString ScaleZ { get; }

		[Field ("kCAValueFunctionTranslate")]
		NSString Translate { get; }

		[Field ("kCAValueFunctionTranslateX")]
		NSString TranslateX { get; }

		[Field ("kCAValueFunctionTranslateY")]
		NSString TranslateY { get; }

		[Field ("kCAValueFunctionTranslateZ")]
		NSString TranslateZ { get; }

	}

	[NoiOS]
	[NoTV]
	[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'CAMetalLayer' instead.")]
	[NoMacCatalyst]
	[BaseType (typeof (CALayer))]
	interface CAOpenGLLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[Export ("asynchronous")]
		bool Asynchronous { [Bind ("isAsynchronous")] get; set; }

		[Export ("canDrawInCGLContext:pixelFormat:forLayerTime:displayTime:")]
		bool CanDrawInCGLContext (CGLContext glContext, CGLPixelFormat pixelFormat, double timeInterval, ref CVTimeStamp timeStamp);

		[Export ("drawInCGLContext:pixelFormat:forLayerTime:displayTime:")]
		void DrawInCGLContext (CGLContext glContext, CGLPixelFormat pixelFormat, double timeInterval, ref CVTimeStamp timeStamp);

		[Export ("copyCGLPixelFormatForDisplayMask:")]
		CGLPixelFormat CopyCGLPixelFormatForDisplayMask (UInt32 mask);

		[Export ("releaseCGLPixelFormat:")]
		void Release (CGLPixelFormat pixelFormat);

		[Export ("copyCGLContextForPixelFormat:")]
		CGLContext CopyContext (CGLPixelFormat pixelFormat);

		[Export ("releaseCGLContext:")]
		void Release (CGLContext glContext);
	}

	/// <summary>A source of particles emitted by a <see cref="T:CoreAnimation.CAEmitterLayer" /> instance.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAEmitterCell_class/index.html">Apple documentation for <c>CAEmitterCell</c></related>
	[BaseType (typeof (NSObject))]
	interface CAEmitterCell : CAMediaTiming, NSSecureCoding {
		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[Export ("birthRate")]
		float BirthRate { get; set; } /* float, not CGFloat */

		[Export ("lifetime")]
		float LifeTime { get; set; } /* float, not CGFloat */

		[Export ("lifetimeRange")]
		float LifetimeRange { get; set; } /* float, not CGFloat */

		[Export ("emissionLatitude")]
		nfloat EmissionLatitude { get; set; }

		[Export ("emissionLongitude")]
		nfloat EmissionLongitude { get; set; }

		[Export ("emissionRange")]
		nfloat EmissionRange { get; set; }

		[Export ("velocity")]
		nfloat Velocity { get; set; }

		[Export ("velocityRange")]
		nfloat VelocityRange { get; set; }

		[Export ("xAcceleration")]
		nfloat AccelerationX { get; set; }

		[Export ("yAcceleration")]
		nfloat AccelerationY { get; set; }

		[Export ("zAcceleration")]
		nfloat AccelerationZ { get; set; }

		[Export ("scale")]
		nfloat Scale { get; set; }

		[Export ("scaleRange")]
		nfloat ScaleRange { get; set; }

		[Export ("scaleSpeed")]
		nfloat ScaleSpeed { get; set; }

		[Export ("spin")]
		nfloat Spin { get; set; }

		[Export ("spinRange")]
		nfloat SpinRange { get; set; }

		[Export ("color")]
		[NullAllowed]
		CGColor Color { get; set; }

		[Export ("redSpeed")]
		float RedSpeed { get; set; } /* float, not CGFloat */

		[Export ("greenSpeed")]
		float GreenSpeed { get; set; } /* float, not CGFloat */

		[Export ("blueSpeed")]
		float BlueSpeed { get; set; } /* float, not CGFloat */

		[Export ("alphaSpeed")]
		float AlphaSpeed { get; set; } /* float, not CGFloat */

		[NullAllowed] // by default this property is null
		[Export ("contents", ArgumentSemantic.Strong)]
		NSObject WeakContents { get; set; }

		[NullAllowed] // just like it's weak property
		[Sealed]
		[Export ("contents", ArgumentSemantic.Strong)]
		CGImage Contents { get; set; }

		[Export ("contentsRect")]
		CGRect ContentsRect { get; set; }

		[Export ("minificationFilter", ArgumentSemantic.Copy)]
		string MinificationFilter { get; set; }

		[Export ("magnificationFilter", ArgumentSemantic.Copy)]
		string MagnificationFilter { get; set; }

		[Export ("minificationFilterBias")]
		float MinificationFilterBias { get; set; } /* float, not CGFloat */

		[NullAllowed] // by default this property is null
		[Export ("emitterCells", ArgumentSemantic.Copy)]
		CAEmitterCell [] Cells { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("style", ArgumentSemantic.Copy)]
		NSDictionary Style { get; set; }

		[Static]
		[Export ("emitterCell")]
		CAEmitterCell EmitterCell ();

		[Static]
		[Export ("defaultValueForKey:")]
		[return: NullAllowed]
		NSObject DefaultValueForKey (string key);

		[Export ("shouldArchiveValueForKey:")]
		bool ShouldArchiveValueForKey (string key);

		[Export ("redRange")]
		float RedRange { get; set; } /* float, not CGFloat */

		[Export ("greenRange")]
		float GreenRange { get; set; } /* float, not CGFloat */

		[Export ("blueRange")]
		float BlueRange { get; set; } /* float, not CGFloat */

		[Export ("alphaRange")]
		float AlphaRange { get; set; } /* float, not CGFloat */

		[MacCatalyst (13, 1)]
		[Export ("contentsScale")]
		nfloat ContentsScale { get; set; }
	}

	/// <summary>A particle-system emitter. Particle types are defined by <see cref="T:CoreAnimation.CAEmitterCell" />.</summary>
	///     
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/GraphicsImaging/Reference/CAEmitterLayer_class/index.html">Apple documentation for <c>CAEmitterLayer</c></related>
	[BaseType (typeof (CALayer))]
	interface CAEmitterLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[NullAllowed] // by default this property is null
		[Export ("emitterCells", ArgumentSemantic.Copy)]
		CAEmitterCell [] Cells { get; set; }

		[Export ("birthRate")]
		float BirthRate { get; set; } /* float, not CGFloat */

		[Export ("lifetime")]
		float LifeTime { get; set; } /* float, not CGFloat */

		[Export ("emitterPosition")]
		CGPoint Position { get; set; }

		[Export ("emitterZPosition")]
		nfloat ZPosition { get; set; }

		[Export ("emitterSize")]
		CGSize Size { get; set; }

		[Export ("emitterDepth")]
		nfloat Depth { get; set; }

		[Export ("emitterShape", ArgumentSemantic.Copy)]
		string Shape { get; set; }

		[Export ("emitterMode", ArgumentSemantic.Copy)]
		string Mode { get; set; }

		[Export ("renderMode", ArgumentSemantic.Copy)]
		string RenderMode { get; set; }

		[Export ("preservesDepth")]
		bool PreservesDepth { get; set; }

		[Export ("velocity")]
		float Velocity { get; set; } /* float, not CGFloat */

		[Export ("scale")]
		float Scale { get; set; } /* float, not CGFloat */

		[Export ("spin")]
		float Spin { get; set; } /* float, not CGFloat */

		[Export ("seed")]
		int Seed { get; set; } // unsigned int

		/* `emitterShape' values. */
		[Field ("kCAEmitterLayerPoint")]
		NSString ShapePoint { get; }

		[Field ("kCAEmitterLayerLine")]
		NSString ShapeLine { get; }

		[Field ("kCAEmitterLayerRectangle")]
		NSString ShapeRectangle { get; }

		[Field ("kCAEmitterLayerCuboid")]
		NSString ShapeCuboid { get; }

		[Field ("kCAEmitterLayerCircle")]
		NSString ShapeCircle { get; }

		[Field ("kCAEmitterLayerSphere")]
		NSString ShapeSphere { get; }

		/* `emitterMode' values. */
		[Field ("kCAEmitterLayerPoints")]
		NSString ModePoints { get; }

		[Field ("kCAEmitterLayerOutline")]
		NSString ModeOutline { get; }

		[Field ("kCAEmitterLayerSurface")]
		NSString ModeSurface { get; }

		[Field ("kCAEmitterLayerVolume")]
		NSString ModeVolume { get; }

		/*  `renderOrder' values. */
		[Field ("kCAEmitterLayerUnordered")]
		NSString RenderUnordered { get; }

		[Field ("kCAEmitterLayerOldestFirst")]
		NSString RenderOldestFirst { get; }

		[Field ("kCAEmitterLayerOldestLast")]
		NSString RenderOldestLast { get; }

		[Field ("kCAEmitterLayerBackToFront")]
		NSString RenderBackToFront { get; }

		[Field ("kCAEmitterLayerAdditive")]
		NSString RenderAdditive { get; }
	}

	// Corresponding headers were removed in Xcode 9 without any explanation
	// rdar #33590997 was filled - no news
	// 'initWithType:', 'behaviorWithType:' and 'behaviorTypes' API now cause rejection
	// https://trello.com/c/J8BDDUV9/86-33590997-coreanimation-quartzcore-api-removals
#if !NET
	[BaseType (typeof (NSObject))]
	interface CAEmitterBehavior : NSSecureCoding {

		// [Export ("initWithType:")]
		// NativeHandle Constructor (NSString type);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[Export ("type")]
		string Type { get; }

		// [Static][Export ("behaviorWithType:")]
		// CAEmitterBehavior Create (NSString type);

		[Field ("kCAEmitterBehaviorAlignToMotion")]
		NSString AlignToMotion { get; }

		[Field ("kCAEmitterBehaviorAttractor")]
		NSString Attractor { get; }

		[Field ("kCAEmitterBehaviorSimpleAttractor")]
		NSString SimpleAttractor { get; }

		[Field ("kCAEmitterBehaviorColorOverLife")]
		NSString ColorOverLife { get; }

		[Field ("kCAEmitterBehaviorDrag")]
		NSString Drag { get; }

		[Field ("kCAEmitterBehaviorLight")]
		NSString Light { get; }

		[Field ("kCAEmitterBehaviorValueOverLife")]
		NSString ValueOverLife { get; }

		[Field ("kCAEmitterBehaviorWave")]
		NSString Wave { get; }
	}
#endif

	[Internal]
	[Static]
	partial interface CARendererOptionKeys {
		[Field ("kCARendererColorSpace")]
		NSString ColorSpace { get; }

		[Field ("kCARendererMetalCommandQueue")]
		NSString MetalCommandQueue { get; }
	}

	[StrongDictionary ("CARendererOptionKeys")]
	interface CARendererOptions {

		[Export ("ColorSpace")]
		CGColorSpace ColorSpace { get; set; }

		[NoMacCatalyst]
		[Export ("MetalCommandQueue")]
		IMTLCommandQueue MetalCommandQueue { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface CARenderer {
		[Static]
		[Export ("rendererWithMTLTexture:options:")]
		CARenderer Create (IMTLTexture tex, [NullAllowed] NSDictionary dict);

		[Static]
		[Wrap ("Create (tex, options.GetDictionary ())")]
		CARenderer Create (IMTLTexture tex, [NullAllowed] CARendererOptions options);

		[NullAllowed, Export ("layer", ArgumentSemantic.Strong)]
		CALayer Layer { get; set; }

		[Export ("bounds", ArgumentSemantic.Assign)]
		CGRect Bounds { get; set; }

		[Export ("beginFrameAtTime:timeStamp:")]
		void BeginFrame (double timeInSeconds, ref CVTimeStamp ts);

		[Sealed]
		[Internal] // since the timestamp is nullable
		[Export ("beginFrameAtTime:timeStamp:")]
		void BeginFrame (double timeInSeconds, IntPtr ts);

		[Wrap ("BeginFrame (timeInSeconds, IntPtr.Zero)")]
		void BeginFrame (double timeInSeconds);

		[Export ("updateBounds")]
		CGRect UpdateBounds ();

		[Export ("addUpdateRect:")]
		void AddUpdate (CGRect r);

		[Export ("render")]
		void Render ();

		[Export ("nextFrameTime")]
		double /* CFTimeInterval */ GetNextFrameTime ();

		[Export ("endFrame")]
		void EndFrame ();

		[Export ("setDestination:")]
		void SetDestination (IMTLTexture tex);
	}

	[iOS (16, 0)]
	[NoTV]
	[MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject), Name = "CAEDRMetadata")]
	[DisableDefaultCtor]
	interface CAEdrMetadata : NSCopying, NSSecureCoding {

		[Static]
		[Export ("HDR10MetadataWithDisplayInfo:contentInfo:opticalOutputScale:")]
		CAEdrMetadata GetHdr10Metadata ([NullAllowed] NSData displayData, [NullAllowed] NSData contentData, float scale);

		[Static]
		[Export ("HDR10MetadataWithMinLuminance:maxLuminance:opticalOutputScale:")]
		CAEdrMetadata GetHdr10Metadata (float minNits, float maxNits, float scale);

		[Static]
		[Export ("HLGMetadata", ArgumentSemantic.Retain)]
		CAEdrMetadata HlgMetadata { get; }

		[Mac (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("available")]
		bool Available { [Bind ("isAvailable")] get; }

		[Static]
		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("HLGMetadataWithAmbientViewingEnvironment:")]
		CAEdrMetadata GetHlgMetadata (NSData ambientViewingEnvironmentData);
	}

	[BaseType (typeof (NSObject))]
	[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[DisableDefaultCtor]
	interface CAMetalDisplayLinkUpdate {
		[Export ("drawable")]
		ICAMetalDrawable Drawable { get; }

		[Export ("targetTimestamp")]
		double /* CFTimeInterval */ TargetTimestamp { get; }

		[Export ("targetPresentationTimestamp")]
		double /* CFTimeInterval */ TargetPresentationTimestamp { get; }
	}

	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	[BaseType (typeof (NSObject))]
	[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	interface CAMetalDisplayLinkDelegate {
		[Abstract]
		[Export ("metalDisplayLink:needsUpdate:")]
		void NeedsUpdate (CAMetalDisplayLink link, CAMetalDisplayLinkUpdate update);
	}

	interface ICAMetalDisplayLinkDelegate { }

	[BaseType (typeof (NSObject))]
	[iOS (17, 0), TV (17, 0), Mac (14, 0), MacCatalyst (17, 0)]
	[DisableDefaultCtor]
	interface CAMetalDisplayLink {
		[Export ("initWithMetalLayer:")]
		NativeHandle Constructor (CAMetalLayer layer);

		[Export ("addToRunLoop:forMode:")]
		void AddToRunLoop (NSRunLoop runloop, NSRunLoopMode mode);

		[Export ("removeFromRunLoop:forMode:")]
		void RemoveFromRunLoop (NSRunLoop runloop, NSRunLoopMode mode);

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate"), NullAllowed]
		ICAMetalDisplayLinkDelegate Delegate { get; set; }

		[Export ("preferredFrameLatency")]
		float PreferredFrameLatency { get; set; }

		[Export ("preferredFrameRateRange")]
		CAFrameRateRange PreferredFrameRateRange { get; set; }

		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; set; }
	}
}
