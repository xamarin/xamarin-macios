//
// Helper functions to make FromVisualFormat more palatable
//
// Author:
//   Miguel de Icaza
//
// Copyright 2014 Xamarin Inc
//

#if MONOMAC
using View = AppKit.NSView;
#else
using View = UIKit.UIView;
#endif

#nullable enable

#if MONOMAC
namespace AppKit
#else
namespace UIKit
#endif
{
	public partial class NSLayoutConstraint {
		static NSNumber? AsNumber (object o)
		{
			if (o is NSNumber) return (NSNumber) o;
			if (o is double) return new NSNumber ((double) o);
			if (o is int) return new NSNumber ((int) o);
			if (o is float) return new NSNumber ((float) o);
			if (o is long) return new NSNumber ((long) o);
			if (o is uint) return new NSNumber ((uint) o);
			if (o is ulong) return new NSNumber ((ulong) o);
			if (o is byte) return new NSNumber ((byte) o);
			if (o is sbyte) return new NSNumber ((sbyte) o);
			if (o is ushort) return new NSNumber ((ushort) o);
			if (o is short) return new NSNumber ((short) o);
			if (o is nint) return new NSNumber ((nint) o);
			if (o is nuint) return new NSNumber ((nuint) o);
			if (o is nfloat) return new NSNumber ((nfloat) o);
			return null;
		}

		/// <summary>Factory method for creating a constraint using Visual Format Language.</summary>
		/// <param name="format">Visual format to use to create the constraints.</param>
		/// <param name="formatOptions">Options for the format.</param>
		/// <param name="viewsAndMetrics">
		///   <para>Pairs of names and values. The names should be strings (or <see cref="Foundation.NSString" />) and the values should be either <see cref="View" />s, numbers (any C# number or <see cref="Foundation.NSNumber" />) or <see cref="ObjCRuntime.INativeObject" /> instances that are suitable to be passed to the underlying engine.</para>
		///   <para>This binds the provided name with the view or binds the name with the number as a metric.</para>
		/// </param>
		/// <returns>An array of layout constraints that are suitable to be added to a <see cref="View" /> using the <see cref="View.AddConstraints" /> method.</returns>
		/// <remarks>
		///   <example>
		///     <code lang="csharp lang-csharp"><![CDATA[
		/// NSLayoutConstraint.FromVisualFormat ("|-8-[messageView]-8-|", NSLayoutFormatOptions.None, "messageView", MakeMessageView ());
		/// ]]></code>
		///   </example>
		/// </remarks>
		static public NSLayoutConstraint [] FromVisualFormat (string format, NSLayoutFormatOptions formatOptions, params object [] viewsAndMetrics)
		{
			ArgumentNullException.ThrowIfNull (format);
			ArgumentNullException.ThrowIfNull (viewsAndMetrics);
			NSMutableDictionary? views = null, metrics = null;
			var count = viewsAndMetrics.Length;
			if (count != 0) {
				if ((count % 2) != 0)
					throw new ArgumentException ("You should provide pairs and values, the parameter passed is not even", "viewsAndMetrics");

				for (int i = 0; i < count; i += 2) {
					var key = viewsAndMetrics [i];
					NSString nskey;

					if (key is string)
						nskey = new NSString ((string) key);
					else if (key is NSString)
						nskey = (NSString) key;
					else
						throw new ArgumentException (String.Format ("Item at {0} is not a string or an NSString", i), "viewsAndMetrics");

					var value = viewsAndMetrics [i + 1];
					if (value is View) {
						if (views is null)
							views = new NSMutableDictionary ();
						views [nskey] = (NSObject) value;
						continue;
					} else if (value is INativeObject iNativeObjectValue && Messaging.bool_objc_msgSend_IntPtr (iNativeObjectValue.Handle, Selector.GetHandle ("isKindOfClass:"), Class.GetHandle (typeof (View))) != 0) {
						if (views is null)
							views = new NSMutableDictionary ();
						views.LowlevelSetObject (iNativeObjectValue, nskey);
						continue;
					}
#if !MONOMAC
					// This requires UILayoutSupport class which is not exist on Mac
					else if (value is INativeObject iNativeObjectValue2 && Messaging.bool_objc_msgSend_IntPtr (iNativeObjectValue2.Handle, Selector.GetHandle ("conformsToProtocol:"), Protocol.GetHandle (typeof (UILayoutSupport).Name)) != 0) {
						if (views is null)
							views = new NSMutableDictionary ();
						views.LowlevelSetObject (iNativeObjectValue2, nskey);
						continue;
					}
#endif // !MONOMAC

					var number = AsNumber (value);
					if (number is null)
						throw new ArgumentException (String.Format ("Item at {0} is not a number or a view", i + 1), "viewsAndMetrics");
					if (metrics is null)
						metrics = new NSMutableDictionary ();
					metrics [nskey] = number;
				}
			}
			if (views is null)
				throw new ArgumentException ("You should at least provide a pair of name, view", "viewAndMetrics");

			return FromVisualFormat (format, formatOptions, metrics, views);
		}

		/// <summary>Factory method for creating a constraint.</summary>
		/// <param name="view1">The view or object in the constraint.</param>
		/// <param name="attribute1">The attribute for the view.</param>
		/// <param name="relation">The relationship in the constraint.</param>
		/// <param name="multiplier">The multiplier applied to the attribute.</param>
		/// <param name="constant">The constant value in the constraint.</param>
		/// <returns>A new constraint with the specified parameters.</returns>
		/// <remarks>Creates a constraint for a single view with a specified attribute, relation, multiplier, and constant value.</remarks>
		public static NSLayoutConstraint Create (NSObject view1, NSLayoutAttribute attribute1, NSLayoutRelation relation, nfloat multiplier, nfloat constant)
		{
			return NSLayoutConstraint.Create (view1, attribute1, relation, null, NSLayoutAttribute.NoAttribute, multiplier, constant);
		}

		/// <summary>Factory method to create a constraint based on a <see cref="View" />, an <see cref="NSLayoutAttribute" />, and an <see cref="NSLayoutRelation" />.</summary>
		/// <param name="view1">The view or object in the constraint.</param>
		/// <param name="attribute1">The attribute for the view.</param>
		/// <param name="relation">The relationship in the constraint.</param>
		/// <returns>A new constraint with the specified parameters and default multiplier (1.0) and constant (0.0).</returns>
		/// <remarks>Creates a constraint for a single view with default multiplier and constant values.</remarks>
		public static NSLayoutConstraint Create (NSObject view1, NSLayoutAttribute attribute1, NSLayoutRelation relation)
		{
			return NSLayoutConstraint.Create (view1, attribute1, relation, null, NSLayoutAttribute.NoAttribute, 1.0f, 0f);
		}

		/// <summary>Factory method for creating a constraint.</summary>
		/// <param name="view1">First view in the constraint.</param>
		/// <param name="attribute1">Attribute for the first view.</param>
		/// <param name="relation">Relationship between the <paramref name="view1" /> and the <paramref name="view2" />.</param>
		/// <param name="view2">Second view in the constraint. This parameter can be <see langword="null" />.</param>
		/// <param name="attribute2">Attribute for the second view.</param>
		/// <param name="multiplier">Multiplier applied to the second attribute.</param>
		/// <param name="constant">Constant to add.</param>
		/// <returns>A new constraint with the specified parameters.</returns>
		/// <remarks>Creates a constraint relationship between the <paramref name="view1" /> and the <paramref name="view2" /> that satisfies the following linear equation: <paramref name="attribute1" /> = <paramref name="multiplier" /> x <paramref name="attribute2" /> + <paramref name="constant" />.</remarks>
		public static NSLayoutConstraint Create (NSObject view1, NSLayoutAttribute attribute1, NSLayoutRelation relation,
					NSObject? view2, NSLayoutAttribute attribute2, nfloat multiplier, nfloat constant)
		{
			return Create ((INativeObject) view1, attribute1, relation, view2, attribute2, multiplier, constant);
		}

		/// <summary>For an anchor-based constraint, returns the first anchor, properly downcast to <typeparamref name="AnchorType" />.</summary>
		/// <typeparam name="AnchorType">The type of anchor to return.</typeparam>
		/// <returns>The first anchor of the constraint, cast to the specified anchor type.</returns>
		/// <remarks>This method is useful for retrieving the first anchor when working with anchor-based constraints.</remarks>
		public NSLayoutAnchor<AnchorType>? FirstAnchor<AnchorType> () where AnchorType : NSObject
		{
			return Runtime.GetNSObject<NSLayoutAnchor<AnchorType>> (_FirstAnchor ());
		}

		/// <summary>For an anchor-based constraint, returns the second anchor, properly downcast to <typeparamref name="AnchorType" />.</summary>
		/// <typeparam name="AnchorType">The type of anchor to return.</typeparam>
		/// <returns>The second anchor of the constraint, cast to the specified anchor type.</returns>
		/// <remarks>This method is useful for retrieving the second anchor when working with anchor-based constraints.</remarks>
		public NSLayoutAnchor<AnchorType>? SecondAnchor<AnchorType> () where AnchorType : NSObject
		{
			return Runtime.GetNSObject<NSLayoutAnchor<AnchorType>> (_SecondAnchor ());
		}
	}
}
