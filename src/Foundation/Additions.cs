// Copyright 2014 Xamarin Inc. All rights reserved.

#if !MONOMAC

using System;

namespace Foundation {

	// NSIndexPath UIKit Additions Reference
	// https://developer.apple.com/library/ios/#documentation/UIKit/Reference/NSIndexPath_UIKitAdditions/Reference/Reference.html
	public partial class NSIndexPath {

		// to avoid a lot of casting inside user source code we decided to expose `int` returning properties
		// https://trello.com/c/5SoMWz30/336-nsindexpath-expose-longrow-longsection-longitem-instead-of-changing-the-int-nature-of-them
		// their usage makes it very unlikely to ever exceed 2^31

		/// <summary>The index of a row within a <see cref="P:Foundation.NSIndexPath.Section" /> of a <see cref="T:UIKit.UITableView" /> (read-only).</summary>
		///         <value>
		///         </value>
		///         <remarks>On 64-bit platforms, the value is truncated from a 64-bit integer to a 32-bit integer.   To avoid this, use the <see cref="P:Foundation.NSIndexPath.LongRow" /> property.</remarks>
		public int Row {
			get { return (int) LongRow; }
		}

		/// <summary>The index of a section within a <see cref="T:UIKit.UITableView" /> (read-only).</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>This section contains the <see cref="P:Foundation.NSIndexPath.Row" /> referenced by this object.</para>
		///           <para />
		///           <para>On 64-bit platforms, the value is truncated from a 64-bit integer to a 32-bit integer.   To avoid this, use the <see cref="P:Foundation.NSIndexPath.LongSection" /> property.</para>
		///         </remarks>
		public int Section {
			get { return (int) LongSection; }
		}
	}
}

#endif // !MONOMAC
