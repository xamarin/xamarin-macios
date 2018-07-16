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
using System.Diagnostics;
#if MONOMAC
using AppKit;
using CoreVideo;
using OpenGL;
#else
using OpenGLES;
using UIKit;
#endif
using Foundation;
using CoreImage;
using CoreGraphics;
using ObjCRuntime;
#if XAMCORE_2_0 || !MONOMAC
using Metal;
#endif
using SceneKit; // For SCNAnimationEvent

namespace CoreAnimation {

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface CAMediaTiming {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("beginTime")]
		double BeginTime { get; set; }
	
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("duration")]
		double Duration { get; set; }
	
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("speed")]
		float Speed { get; set; } /* float, not CGFloat */
	
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("timeOffset")]
		double TimeOffset { get; set; }
	
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("repeatCount")]
		float RepeatCount { get; set; } /* float, not CGFloat */
	
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("repeatDuration")]
		double RepeatDuration { get; set; }
	
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("autoreverses")]
		bool AutoReverses { get;set; }
	
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("fillMode", ArgumentSemantic.Copy)]
		string FillMode { get; set; }
	}

	interface ICAMediaTiming {}

#if MONOMAC
	[BaseType (typeof (NSObject))]
	interface CAConstraintLayoutManager : NSCoding {
		[Static]
		[Export ("layoutManager")]
		CAConstraintLayoutManager LayoutManager { get; }
	}
	
	[BaseType (typeof (NSObject))]
	interface CAConstraint : NSSecureCoding {
		[Export ("attribute")]
		CAConstraintAttribute Attribute { get;  }

		[Export ("sourceName")]
		string SourceName { get;  }

		[Export ("sourceAttribute")]
		CAConstraintAttribute SourceAttribute { get;  }

		[Export ("scale")]
		nfloat Scale { get;  }

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
		IntPtr Constructor (CAConstraintAttribute attribute, string relativeToSource, CAConstraintAttribute srcAttr, nfloat scale, nfloat offset);
	}
	
#else
	[BaseType (typeof (NSObject))]
	interface CADisplayLink {
		[Export ("displayLinkWithTarget:selector:")][Static]
		CADisplayLink Create (NSObject target, Selector sel);
	
		[Export ("addToRunLoop:forMode:")]
		void AddToRunLoop (NSRunLoop runloop, [NullAllowed] NSString mode);

		[Wrap ("AddToRunLoop (runloop, mode.GetConstant ())")]
		void AddToRunLoop (NSRunLoop runloop, NSRunLoopMode mode);
	
		[Export ("removeFromRunLoop:forMode:")]
		void RemoveFromRunLoop (NSRunLoop runloop, [NullAllowed] NSString mode);

		[Wrap ("RemoveFromRunLoop (runloop, mode.GetConstant ())")]
		void RemoveFromRunLoop (NSRunLoop runloop, NSRunLoopMode mode);

		[Export ("invalidate")]
		void Invalidate ();
	
		[Export ("timestamp")]
		double Timestamp { get; }
	
		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; set; }
	
		[Deprecated (PlatformName.iOS, 10,0, message: "Use 'PreferredFramesPerSecond' property.")]
		[Deprecated (PlatformName.TvOS, 10,0, message: "Use 'PreferredFramesPerSecond' property.")]
		[Deprecated (PlatformName.WatchOS, 3,0, message: "Use 'PreferredFramesPerSecond' property.")]
		[Export ("frameInterval")]
		nint FrameInterval { get; set;  }

		[Export ("duration")]
		double Duration { get; }

		[Watch (3,0)][TV (10,0)][iOS (10,0)]
		[Export ("targetTimestamp")]
		double TargetTimestamp { get; }

		[Watch (3,0)][TV (10,0)][iOS (10,0)]
		[Export ("preferredFramesPerSecond")]
		nint PreferredFramesPerSecond { get; set; }
	}
#endif

	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	enum CAContentsFormat {
		[Field ("kCAContentsFormatGray8Uint")]
		Gray8Uint,
		[Field ("kCAContentsFormatRGBA8Uint")]
		Rgba8Uint,
		[Field ("kCAContentsFormatRGBA16Float")]
		Rgba16Float,
	}

	[BaseType (typeof (NSObject))]
	[Dispose ("OnDispose ();")]
	interface CALayer : CAMediaTiming, NSSecureCoding {
		[Export ("layer")][Static]
		CALayer Create ();

		[Export ("presentationLayer")]
		CALayer PresentationLayer { get; }

		[Export ("modelLayer")]
		CALayer ModelLayer { get; }

		[Static]
		[Export ("defaultValueForKey:")]
		NSObject DefaultValue ([NullAllowed] string key);

		[Static]
		[Export ("needsDisplayForKey:")]
		bool NeedsDisplayForKey (string key);

		[Export ("bounds")]
		CGRect Bounds  { get; set; }

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
		CALayer SuperLayer { get; }

		[Export ("removeFromSuperlayer")]
		void RemoveFromSuperLayer ();

		[NullAllowed] // by default this property is null
		[Export ("sublayers", ArgumentSemantic.Copy)]
		CALayer [] Sublayers { get; set; }
		
		[Export ("addSublayer:")][PostGet ("Sublayers")]
		void AddSublayer (CALayer layer);

		[Export ("insertSublayer:atIndex:")][PostGet ("Sublayers")]
		void InsertSublayer (CALayer layer, int index);

		[Export ("insertSublayer:below:")][PostGet ("Sublayers")]
		void InsertSublayerBelow (CALayer layer, CALayer sibling);
		
		[Export ("insertSublayer:above:")][PostGet ("Sublayers")]
		void InsertSublayerAbove (CALayer layer, CALayer sibling);

		[Export ("replaceSublayer:with:")][PostGet ("Sublayers")]
		void ReplaceSublayer (CALayer layer, CALayer with);

		[Export ("sublayerTransform")]
		CATransform3D SublayerTransform { get; set; }

		[Export ("mask", ArgumentSemantic.Strong)][NullAllowed]
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
		CALayer HitTest (CGPoint p);

		[Export ("containsPoint:")]
		bool Contains (CGPoint p);

		[DebuggerBrowsable (DebuggerBrowsableState.Never)]
		[Export ("contents", ArgumentSemantic.Strong), NullAllowed]
		CGImage Contents { get; set; }

		[Export ("contents", ArgumentSemantic.Strong)][Internal][Sealed]
		IntPtr _Contents { get; set; }

#if MONOMAC
		[Export ("layoutManager", ArgumentSemantic.Retain)]
		NSObject LayoutManager { get; set; }
#endif

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
		NSObject DefaultActionForKey (string eventKey);

		[Export ("actionForKey:")]
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
		CAAnimation AnimationForKey (string key);

		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak)][NullAllowed]
		NSObject WeakDelegate { get; [PostSnippet (@"SetCALayerDelegate (value as CALayerDelegate);")] set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		CALayerDelegate Delegate { get; set; }

		[Export ("shadowColor")]
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

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Internal]
		[Export ("contentsFormat")]
		NSString _ContentsFormat { get; set; }

		[Export ("visibleRect")]
		CGRect VisibleRect { get;  }

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

#if MONOMAC
		[Export ("autoresizingMask")]
		CAAutoresizingMask AutoresizingMask { get; set; }

		[Export ("resizeSublayersWithOldSize:")]
		void ResizeSublayers (CGSize oldSize);

		[Export ("resizeWithOldSuperlayerSize:")]
		void Resize (CGSize oldSuperlayerSize);
		
		[Export ("constraints")]
		CAConstraint[] Constraints { get; set;  }

		[Export ("addConstraint:")]
		void AddConstraint (CAConstraint c);
#endif

		[Export ("shouldRasterize")]
		bool ShouldRasterize { get; set; }

		[NullAllowed]
		[Export ("shadowPath")]
		CGPath ShadowPath { get; set; }

		[Export ("rasterizationScale")]
		nfloat RasterizationScale { get; set; }

		[iOS (6,0)]
		[Mac (10,8)]
		[Export ("drawsAsynchronously")]
		bool DrawsAsynchronously { get; set; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("allowsEdgeAntialiasing")]
		bool AllowsEdgeAntialiasing { get; set; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("allowsGroupOpacity")]
		bool AllowsGroupOpacity { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("compositingFilter", ArgumentSemantic.Strong)]
		NSObject CompositingFilter { get; set; }

		[NoWatch] // headers not updated
		[TV (11,0)][Mac (10,13)][iOS (11,0)]
		[Export ("maskedCorners", ArgumentSemantic.Assign)]
		CACornerMask MaskedCorners { get; set; }
	}

#if XAMCORE_2_0 || !MONOMAC
	interface ICAMetalDrawable {}

	[Protocol]
	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
	interface CAMetalDrawable : MTLDrawable {
		[Abstract]
		[Export ("texture")]
		IMTLTexture Texture { get; }

		[Abstract]
		[Export ("layer")]
		CAMetalLayer Layer { get; }
	}

	[iOS (8,0)][Mac (10,11, onlyOn64 : true)]
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

		[Export ("newDrawable")]
		ICAMetalDrawable CreateDrawable ();

		[Export ("nextDrawable")]
		ICAMetalDrawable NextDrawable ();
		
		[Export ("presentsWithTransaction")]
		bool PresentsWithTransaction { [Bind ("presentsWithTransaction")] get; set; }

		[NoWatch][NoTV][NoiOS]
		[Mac (10,13)]
		[Export ("displaySyncEnabled")]
		bool DisplaySyncEnabled { get; set; }

		[NoWatch] // headers not updated
		[TV (11,0)][Mac (10,13)][iOS (11,0)]
		[Export ("allowsNextDrawableTimeout")]
		bool AllowsNextDrawableTimeout { get; set; }

		[NoWatch] // headers not updated
		[TV (11,2)][Mac (10,13,2)][iOS (11,2)]
		[Export ("maximumDrawableCount")]
		nuint MaximumDrawableCount { get; set; }
	}
#endif

	[BaseType (typeof (CALayer))]
	interface CATiledLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();
		
		[Static][Export ("fadeDuration")]
		double FadeDuration { get; }

		[Export ("levelsOfDetail")]
		nint LevelsOfDetail { get; set; }

		[Export ("levelsOfDetailBias")]
		nint LevelsOfDetailBias { get; set; }

		[Export ("tileSize")]
		CGSize TileSize { get; set; }
	}

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


	[BaseType (typeof (CALayer))]
	interface CAScrollLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

#if XAMCORE_4_0
		[Protected]
#endif
		[Export ("scrollMode", ArgumentSemantic.Copy)]
		NSString ScrollMode { get; set;  }

		[Export ("scrollToPoint:")]
		void ScrollToPoint (CGPoint p);

		[Export ("scrollToRect:")]
		void ScrollToRect (CGRect r);
	}

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
	
	[BaseType (typeof (CALayer))]
	interface CAShapeLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[Export ("path")] [NullAllowed]
		CGPath Path { get; set; }

		[Export ("fillColor")] [NullAllowed]
		CGColor FillColor { get; set; }

		[Export ("fillRule", ArgumentSemantic.Copy)]
		NSString FillRule { get; set; }

		[Export ("lineCap", ArgumentSemantic.Copy)]
		NSString LineCap { get; set; }

		[Export ("lineDashPattern", ArgumentSemantic.Copy)] [NullAllowed]
		NSNumber [] LineDashPattern { get; set; }

		[Export ("lineDashPhase")]
		nfloat LineDashPhase { get; set; }

		[Export ("lineJoin", ArgumentSemantic.Copy)]
		NSString LineJoin { get; set; }

		[Export ("lineWidth")]
		nfloat LineWidth { get; set; }

		[Export ("miterLimit")]
		nfloat MiterLimit { get; set; }

		[Export ("strokeColor")] [NullAllowed]
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

	[BaseType (typeof (CALayer))]
	interface CATextLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[NullAllowed] // by default this property is null
		[Export ("string", ArgumentSemantic.Copy)]
		string String { get; set; }

		[Sealed] [Internal]
		[NullAllowed] // by default this property is null
		[Export ("string", ArgumentSemantic.Copy)]
		IntPtr _AttributedString { get; set; }

		[Export ("fontSize")]
		nfloat FontSize { get; set; }

		[Export ("font"), Internal]
		IntPtr _Font { get; set; }
		
		[Export ("foregroundColor")]
		CGColor ForegroundColor { get; set; }

		[Export ("wrapped")]
		bool Wrapped { [Bind ("isWrapped")] get; set; }

		[Protected]
		[Export ("truncationMode", ArgumentSemantic.Copy)]
		NSString WeakTruncationMode { get; set; }

		[Protected]
		[Export ("alignmentMode", ArgumentSemantic.Copy)]
		NSString WeakAlignmentMode { get; set; }

#if !XAMCORE_4_0 // Use smart enums instead, CATruncationMode and CATextLayerAlignmentMode.
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
#endif // !XAMCORE_4_0

		[iOS(9,0)]
		[Export ("allowsFontSubpixelQuantization")]
		bool AllowsFontSubpixelQuantization { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
#if IOS || TVOS
	[Protocol (FormalSince = "10.0")]
#elif MONOMAC
	[Protocol (FormalSince = "10.12")]
#elif WATCH
	[Protocol (FormalSince = "3.0"]
#else
	[Protocol]
#endif
	interface CALayerDelegate {
		[Export ("displayLayer:")]
		void DisplayLayer (CALayer layer);

		[Export ("drawLayer:inContext:"), EventArgs ("CALayerDrawEventArgs")]
		void DrawLayer (CALayer layer, CGContext context);

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Export ("layerWillDraw:")]
		void WillDrawLayer (CALayer layer);

		[Export ("layoutSublayersOfLayer:")]
		void LayoutSublayersOfLayer (CALayer layer);

		[Export ("actionForLayer:forKey:"), EventArgs ("CALayerDelegateAction"), DefaultValue (null)]
		NSObject ActionForLayer (CALayer layer, string eventKey);
	}
	
#if !MONOMAC
	[BaseType (typeof (CALayer))]
	interface CAEAGLLayer : EAGLDrawable {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[iOS (9,0)]
		[Export ("presentsWithTransaction")]
		bool PresentsWithTransaction { get; set; }
	}
#endif

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[DisableDefaultCtor]
	interface CAAction {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("runActionForKey:object:arguments:")]
		void RunAction (string eventKey, NSObject obj, [NullAllowed] NSDictionary arguments);
	}
	
	[BaseType (typeof (NSObject)
#if XAMCORE_4_0
		, Delegates = new string [] {"WeakDelegate"}, Events = new Type [] { typeof (CAAnimationDelegate) }
#endif
	)]
	interface CAAnimation : CAAction, CAMediaTiming, NSSecureCoding, NSMutableCopying, SCNAnimationProtocol {
		[Export ("animation"), Static]
		CAAnimation CreateAnimation ();
	
		[Static]
		[Export ("defaultValueForKey:")]
		NSObject DefaultValue ([NullAllowed] string key);
	
		[NullAllowed] // by default this property is null
		[Export ("timingFunction", ArgumentSemantic.Strong)]
		CAMediaTimingFunction TimingFunction { get; set; }
	
#if XAMCORE_4_0
		// before that we need to be wrap this manually to avoid the BI1110 error
		[Wrap ("WeakDelegate")]
		ICAAnimationDelegate Delegate { get; set; }
#endif
	
		[Export ("delegate", ArgumentSemantic.Strong)][NullAllowed]
		NSObject WeakDelegate { get; set; }
	
		[Export ("removedOnCompletion")]
		bool RemovedOnCompletion { [Bind ("isRemovedOnCompletion")] get; set; }

		[Export ("willChangeValueForKey:")]
		void WillChangeValueForKey (string key);
	
		[Export ("didChangeValueForKey:")]
		void DidChangeValueForKey (string key);

		[Export ("shouldArchiveValueForKey:")]
		bool ShouldArchiveValueForKey ([NullAllowed] string key);

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
				
#if !XAMCORE_4_0
		[Field ("kCAAnimationDiscrete")]
		[Obsolete ("The name has been fixed, use 'AnimationDiscrete' instead.")]
		NSString AnimationDescrete { get; }
#endif
		[Field ("kCAAnimationDiscrete")]
		NSString AnimationDiscrete { get; }
		
		[Field ("kCAAnimationPaced")]
		NSString AnimationPaced { get; }

		[Mac (10, 7)] // iOS 4.0
		[Field ("kCAAnimationCubic")]
		NSString AnimationCubic { get; }

		[Mac (10, 7)] // iOS 4.0
		[Field ("kCAAnimationCubicPaced")]
		NSString AnimationCubicPaced { get; }

		/* 'rotationMode' strings. */
		[Field ("kCAAnimationRotateAuto")]
		NSString RotateModeAuto { get; }

		[Field ("kCAAnimationRotateAutoReverse")]
		NSString RotateModeAutoReverse { get; }

		#region SceneKitAdditions

		[TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0, onlyOn64: true), NoWatch]
		[Static]
		[Export ("animationWithSCNAnimation:")]
		CAAnimation FromSCNAnimation (SCNAnimation animation);

		[iOS (8,0)][Mac (10,9, onlyOn64 : true)]
		[Export ("usesSceneTimeBase")]
		bool UsesSceneTimeBase { get; set; }

		[iOS (8,0)][Mac (10,9, onlyOn64 : true)]
		[Export ("fadeInDuration")]
		nfloat FadeInDuration { get; set; }

		[iOS (8,0)][Mac (10,9, onlyOn64 : true)]
		[Export ("fadeOutDuration")]
		nfloat FadeOutDuration { get; set; }

		[Mac (10,9, onlyOn64 : true), iOS (8, 0)]
		[NullAllowed] // by default this property is null
		[Export ("animationEvents", ArgumentSemantic.Retain)]
		SCNAnimationEvent [] AnimationEvents { get; set; }

		#endregion
	}

	interface ICAAnimationDelegate {}

	[BaseType (typeof (NSObject))]
#if IOS || TVOS
	[Protocol (FormalSince = "10.0")]
#elif MONOMAC
	[Protocol (FormalSince = "10.12")]
#elif WATCH
	[Protocol (FormalSince = "3.0"]
#else
	[Synthetic]
#endif
	[Model]
	interface CAAnimationDelegate {
		[Export ("animationDidStart:")]
		void AnimationStarted ([NullAllowed] CAAnimation anim);
	
		[Export ("animationDidStop:finished:"), EventArgs ("CAAnimationState")]
		void AnimationStopped ([NullAllowed] CAAnimation anim, bool finished);
	
	}
	
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
	
	[BaseType (typeof (CAPropertyAnimation))]
	interface CABasicAnimation {
		[Static, New, Export ("animationWithKeyPath:")]
		CABasicAnimation FromKeyPath ([NullAllowed] string path);

		[Export ("fromValue", ArgumentSemantic.Strong)][Internal][Sealed]
		IntPtr _From { get; set; }

		[Export ("fromValue", ArgumentSemantic.Strong)][NullAllowed]
		NSObject From { get; set; }

		[Export ("toValue", ArgumentSemantic.Strong)][Internal][Sealed]
		IntPtr _To { get; set; }

		[Export ("toValue", ArgumentSemantic.Strong)][NullAllowed]
		NSObject To { get; set; }

		[Export ("byValue", ArgumentSemantic.Strong)][Internal][Sealed]
		IntPtr _By { get; set; }

		[Export ("byValue", ArgumentSemantic.Strong)][NullAllowed]
		NSObject By { get; set; }
	}

	[iOS(9,0), Mac(10,11)]
	[BaseType (typeof (CABasicAnimation))]
	interface CASpringAnimation {
		[Static, New, Export ("animationWithKeyPath:")]
		CABasicAnimation FromKeyPath ([NullAllowed] string path);

		[Export ("mass")]
		nfloat Mass { get; set; }

		[Export ("stiffness")]
		nfloat Stiffness { get; set; }

		[Export("damping")]
		nfloat Damping { get; set; }

		[Export ("initialVelocity")]
		nfloat InitialVelocity { get; set; }

		[Export ("settlingDuration")]
		double /* CFTimeInterval */ SettlingDuration { get; }
	}
	
	[BaseType (typeof (CAPropertyAnimation), Name="CAKeyframeAnimation")]
	interface CAKeyFrameAnimation {
		[Static, Export ("animationWithKeyPath:")]
#if XAMCORE_2_0
		CAKeyFrameAnimation FromKeyPath ([NullAllowed] string path);
#else
		CAKeyFrameAnimation GetFromKeyPath ([NullAllowed] string path);
#endif

		[NullAllowed] // by default this property is null
		[Export ("values", ArgumentSemantic.Copy)]
		NSObject [] Values { get; set; }

		[Export ("values", ArgumentSemantic.Strong)][Internal][Sealed]
		NSArray _Values { get; set; }
	
		[NullAllowed]
		[Export ("path")]
		CGPath Path { get; set; }
	
		[Export ("keyTimes", ArgumentSemantic.Copy)][NullAllowed]
		NSNumber [] KeyTimes { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("timingFunctions", ArgumentSemantic.Copy)]
		CAMediaTimingFunction [] TimingFunctions { get; set; }
	
		[Export ("calculationMode", ArgumentSemantic.Copy)]
		[Internal]
		NSString _CalculationMode { get; set; }
	
		[Export ("rotationMode", ArgumentSemantic.Copy)][NullAllowed]
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
		
#if !XAMCORE_2_0
		// duplicated, wrong prefix + missing members

		[Field ("kCAAnimationLinear")]
		NSString CalculationLinear { get; }

		[Field ("kCAAnimationDiscrete")]
		NSString CalculationDiscrete { get; }
		
		[Field ("kCAAnimationDiscrete")]
		NSString CalculationPaced { get; }
#endif
	}
	
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
	
		[Export ("filter", ArgumentSemantic.Strong)][NullAllowed]
		NSObject Filter { get; set; }

#if !XAMCORE_2_0
		[Wrap ("Filter")]
		[Obsolete ("The name has been fixed, use 'Filter' instead.")]
		NSObject filter { get; set; }
#endif
	}

#if XAMCORE_2_0
	[Static]
#else
	[Partial] // keep default .ctor for API compatibility
#endif
	interface CAFillMode {
		[Field ("kCAFillModeForwards")]
		NSString Forwards { get; }

		[Field ("kCAFillModeBackwards")]
		NSString Backwards { get; }

		[Field ("kCAFillModeBoth")]
		NSString Both { get; }

		[Field ("kCAFillModeRemoved")]
		NSString Removed { get; }

#if !XAMCORE_2_0
		[Availability (Deprecated = Platform.iOS_4_0, Message = "Use 'CAFillMode.Forwards' instead.")]
		[Field ("kCAFillModeFrozen")]
		NSString Frozen { get; }
#endif
	}

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

	[BaseType (typeof (CAAnimation))]
	interface CAAnimationGroup {
		[NullAllowed] // by default this property is null
		[Export ("animations", ArgumentSemantic.Copy)]
		CAAnimation [] Animations { get; set; }

		[Export ("animation"), Static, New]
		CAAnimationGroup CreateAnimation ();
	}

	[BaseType (typeof (CALayer))]
	interface CAGradientLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[NullAllowed] // by default this property is null
		[Export ("colors", ArgumentSemantic.Copy)][Internal]
		IntPtr _Colors { get; set;  }
	
		[NullAllowed] // by default this property is null
		[Export ("locations", ArgumentSemantic.Copy)]
		NSNumber [] Locations { get; set;  }
	
		[Export ("startPoint")]
		CGPoint StartPoint { get; set;  }

		[Export ("endPoint")]
		CGPoint EndPoint { get; set;  }
	
		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; set;  }

		[Field ("kCAGradientLayerAxial")]
		NSString GradientLayerAxial { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface CAMediaTimingFunction : NSSecureCoding {
		[Export ("functionWithName:")][Static]
		CAMediaTimingFunction FromName (NSString  name);

		[Static]
		[Export ("functionWithControlPoints::::")]
		CAMediaTimingFunction FromControlPoints (float c1x, float c1y, float c2x, float c2y); /* all float, not CGFloat */
	
		[Export ("initWithControlPoints::::")]
		IntPtr Constructor (float c1x, float c1y, float c2x, float c2y); /* all float, not CGFloat */
	
		[Export ("getControlPointAtIndex:values:"), Internal]
		void GetControlPointAtIndex (nint idx, IntPtr /* float[2] */ point);
	
		[Field("kCAMediaTimingFunctionLinear")]
		NSString Linear { get; }
		
		[Field("kCAMediaTimingFunctionEaseIn")]
		NSString EaseIn { get; }
		
		[Field("kCAMediaTimingFunctionEaseOut")]
		NSString EaseOut { get; }
		
		[Field("kCAMediaTimingFunctionEaseInEaseOut")]
		NSString EaseInEaseOut { get; }

		[Field("kCAMediaTimingFunctionDefault")]
		NSString Default { get; }
	}

	[BaseType (typeof (NSObject))]
	interface CAValueFunction : NSSecureCoding {
		[Export ("functionWithName:"), Static]
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

#if MONOMAC
	[Deprecated (PlatformName.MacOSX, 10, 14, message : "Use Metal.")]
	[BaseType (typeof (CALayer))]
	interface CAOpenGLLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[Export ("asynchronous")]
		bool Asynchronous { [Bind ("isAsynchronous")]get; set; }	

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
#endif

	[BaseType (typeof (NSObject))]
	interface CAEmitterCell : CAMediaTiming, NSSecureCoding {
		[NullAllowed] // by default this property is null
		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; set;  }

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set;  }

		[Export ("birthRate")]
		float BirthRate { get; set;  } /* float, not CGFloat */

		[Export ("lifetime")]
		float LifeTime { get; set;  } /* float, not CGFloat */

		[Export ("lifetimeRange")]
		float LifetimeRange { get; set;  } /* float, not CGFloat */

		[Export ("emissionLatitude")]
		nfloat EmissionLatitude { get; set;  }

		[Export ("emissionLongitude")]
		nfloat EmissionLongitude { get; set;  }

		[Export ("emissionRange")]
		nfloat EmissionRange { get; set;  }

		[Export ("velocity")]
		nfloat Velocity { get; set;  }

		[Export ("velocityRange")]
		nfloat VelocityRange { get; set;  }

		[Export ("xAcceleration")]
		nfloat AccelerationX { get; set;  }

		[Export ("yAcceleration")]
		nfloat AccelerationY { get; set;  }

		[Export ("zAcceleration")]
		nfloat AccelerationZ { get; set;  }

		[Export ("scale")]
		nfloat Scale { get; set;  }

		[Export ("scaleRange")]
		nfloat ScaleRange { get; set;  }

		[Export ("scaleSpeed")]
		nfloat ScaleSpeed { get; set;  }

		[Export ("spin")]
		nfloat Spin { get; set;  }

		[Export ("spinRange")]
		nfloat SpinRange { get; set;  }
		
		[Export ("color")]
		CGColor Color { get; set;  }

		[Export ("redSpeed")]
		float RedSpeed { get; set;  } /* float, not CGFloat */

		[Export ("greenSpeed")]
		float GreenSpeed { get; set;  } /* float, not CGFloat */

		[Export ("blueSpeed")]
		float BlueSpeed { get; set;  } /* float, not CGFloat */

		[Export ("alphaSpeed")]
		float AlphaSpeed { get; set;  } /* float, not CGFloat */

		[NullAllowed] // by default this property is null
		[Export ("contents", ArgumentSemantic.Strong)]
		NSObject WeakContents { get; set;  }

		[NullAllowed] // just like it's weak property
		[Sealed]
		[Export ("contents", ArgumentSemantic.Strong)]
		CGImage Contents { get; set; }

		[Export ("contentsRect")]
		CGRect ContentsRect { get; set;  }

		[Export ("minificationFilter", ArgumentSemantic.Copy)]
		string MinificationFilter { get; set;  }

		[Export ("magnificationFilter", ArgumentSemantic.Copy)]
		string MagnificationFilter { get; set;  }

		[Export ("minificationFilterBias")]
		float MinificationFilterBias { get; set;  } /* float, not CGFloat */

		[NullAllowed] // by default this property is null
		[Export ("emitterCells", ArgumentSemantic.Copy)]
		CAEmitterCell[] Cells { get; set;  }

		[NullAllowed] // by default this property is null
		[Export ("style", ArgumentSemantic.Copy)]
		NSDictionary Style { get; set;  }
		
		[Static]
		[Export ("emitterCell")]
		CAEmitterCell EmitterCell ();

		[Static]
		[Export ("defaultValueForKey:")]
		NSObject DefaultValueForKey ([NullAllowed] string key);

		[Export ("shouldArchiveValueForKey:")]
		bool ShouldArchiveValueForKey ([NullAllowed] string key);

		[Export ("redRange")]
		float RedRange { get; set; } /* float, not CGFloat */
		
		[Export ("greenRange")]
		float GreenRange { get; set; } /* float, not CGFloat */

		[Export ("blueRange")]
		float BlueRange { get; set; } /* float, not CGFloat */

		[Export ("alphaRange")]
		float AlphaRange { get; set; } /* float, not CGFloat */

		[iOS(9,0)][Mac (10,11)]
		[Export ("contentsScale")]
		nfloat ContentsScale { get; set; }
	}

	[BaseType (typeof (CALayer))]
	interface CAEmitterLayer {
		[Export ("layer"), New, Static]
		CALayer Create ();

		[NullAllowed] // by default this property is null
		[Export ("emitterCells", ArgumentSemantic.Copy)]
		CAEmitterCell[] Cells { get; set;  }

		[Export ("birthRate")]
		float BirthRate { get; set;  } /* float, not CGFloat */

		[Export ("lifetime")]
		float LifeTime { get; set;  } /* float, not CGFloat */

		[Export ("emitterPosition")]
		CGPoint Position { get; set;  }

		[Export ("emitterZPosition")]
		nfloat ZPosition { get; set;  }

		[Export ("emitterSize")]
		CGSize Size { get; set;  }

		[Export ("emitterDepth")]
		nfloat Depth { get; set;  }

		[Export ("emitterShape", ArgumentSemantic.Copy)]
		string Shape { get; set;  }

		[Export ("emitterMode", ArgumentSemantic.Copy)]
		string Mode { get; set;  }

		[Export ("renderMode", ArgumentSemantic.Copy)]
		string RenderMode { get; set;  }

		[Export ("preservesDepth")]
		bool PreservesDepth { get; set;  }

		[Export ("velocity")]
		float Velocity { get; set;  } /* float, not CGFloat */

		[Export ("scale")]
		float Scale { get; set;  } /* float, not CGFloat */

		[Export ("spin")]
		float Spin { get; set;  } /* float, not CGFloat */

		[Export ("seed")]
		int Seed { get; set;  } // unsigned int
		
		/** `emitterShape' values. **/
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
	
		/** `emitterMode' values. **/
		[Field ("kCAEmitterLayerPoints")]
		NSString ModePoints { get; }			

		[Field ("kCAEmitterLayerOutline")]
		NSString ModeOutline { get; }			

		[Field ("kCAEmitterLayerSurface")]
		NSString ModeSurface { get; }			

		[Field ("kCAEmitterLayerVolume")]
		NSString ModeVolume { get; }			

		/** `renderOrder' values. **/		
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

	[iOS (7,0), Mac (10, 9)]
	[BaseType (typeof (NSObject))]
	interface CAEmitterBehavior : NSSecureCoding {

		[Export ("initWithType:")]
		IntPtr Constructor (NSString type);

		[Export ("enabled")]
		bool Enabled { [Bind ("isEnabled")] get; set; }

		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[Export ("type")]
		string Type { get; }

		[Static][Export ("behaviorTypes")]
		NSString[] BehaviorTypes { get; }

		[Static][Export ("behaviorWithType:")]
		CAEmitterBehavior Create (NSString type);

		[Field ("kCAEmitterBehaviorAlignToMotion")]
		NSString AlignToMotion { get; }			

		[Field ("kCAEmitterBehaviorAttractor")]
		NSString Attractor { get; }			

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
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
}
