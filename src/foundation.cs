//
// This file describes the API that the generator will produce
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//   Aaron Bockover
//
// Copyright 2009, Novell, Inc.
// Copyright 2010, Novell, Inc.
// Copyright 2011-2013 Xamarin Inc.
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

global using nfloat = global::System.Runtime.InteropServices.NFloat;

using CloudKit;
using CoreData;
using CoreFoundation;
using CoreGraphics;
using UniformTypeIdentifiers;
using Network;
#if HAS_APPCLIP
using AppClip;
#endif
#if IOS
using QuickLook;
#endif
#if !TVOS
using Contacts;
#endif
using CoreAnimation;
using CoreSpotlight;
using CoreMedia;
using SceneKit;
using Security;
#if IOS || MONOMAC
using FileProvider;
#else
using INSFileProviderItem = Foundation.NSObject;
#endif

#if MONOMAC
using AppKit;
using QuickLookUI;
#else
using CoreLocation;
using UIKit;
#endif

using System.ComponentModel;

// In Apple headers, this is a typedef to a pointer to a private struct
using NSAppleEventManagerSuspensionID = System.IntPtr;
// These two are both four char codes i.e. defined on a uint with constant like 'xxxx'
using AEKeyword = System.UInt32;
using OSType = System.UInt32;
// typedef double NSTimeInterval;
using NSTimeInterval = System.Double;

#if MONOMAC
// dummy usings to make code compile without having the actual types available (for [NoMac] to work)
using NSDirectionalEdgeInsets = Foundation.NSObject;
using UIEdgeInsets = Foundation.NSObject;
using UIOffset = Foundation.NSObject;
using UIPreferredPresentationStyle = Foundation.NSObject;
#else
using NSBindingOptions = Foundation.NSObject;
using NSPasteboard = Foundation.NSObject;
using NSWorkspaceAuthorization = Foundation.NSObject;

using NSStringAttributes = UIKit.UIStringAttributes;
#endif

#if IOS && !__MACCATALYST__
using NSAppleEventSendOptions = Foundation.NSObject;
using NSBezierPath = Foundation.NSObject;
using NSImage = Foundation.NSObject;
#endif

#if TVOS
using NSAppleEventSendOptions = Foundation.NSObject;
using NSBezierPath = Foundation.NSObject;
using NSImage = Foundation.NSObject;
#endif

using CoreImage;

#if !IOS
using APActivationPayload = Foundation.NSObject;
#endif

#if __MACCATALYST__
using NSAppleEventSendOptions = Foundation.NSObject;
using NSBezierPath = Foundation.NSObject;
using NSImage = Foundation.NSObject;
#endif

#if IOS || TVOS
using NSAppearance = UIKit.UIAppearance;
using NSColor = UIKit.UIColor;
using NSNotificationSuspensionBehavior = Foundation.NSObject;
using NSNotificationFlags = Foundation.NSObject;
using NSTextBlock = Foundation.NSObject;
using NSTextTable = Foundation.NSString; // Different frmo NSTextBlock, because some methods overload on these two types.
#endif

namespace Foundation {
	/// <param name="reacquirer">To be added.</param>
	/// <summary>Completion handler for relinquishing a file to a reader.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSFilePresenterReacquirer ([BlockCallback] Action reacquirer);
}

namespace Foundation {
	/// <param name="obj1">To be added.</param>
	/// <param name="obj2">To be added.</param>
	/// <summary>A delegate that defines the comparison function to be used with functions such as <see cref="Foundation.NSArray.Sort(Foundation.NSComparator)" />.</summary>
	/// <returns>To be added.</returns>
	/// <remarks>To be added.</remarks>
	delegate NSComparisonResult NSComparator (NSObject obj1, NSObject obj2);
	/// <param name="attrs">To be added.</param>
	/// <param name="range">To be added.</param>
	/// <param name="stop">To be added.</param>
	/// <summary>A delegate that specifies the callback for the <see cref="Foundation.NSAttributedString.EnumerateAttributes(Foundation.NSRange,Foundation.NSAttributedStringEnumeration,Foundation.NSAttributedRangeCallback)" /> method.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSAttributedRangeCallback (NSDictionary attrs, NSRange range, ref bool stop);
	/// <param name="value">To be added.</param>
	/// <param name="range">To be added.</param>
	/// <param name="stop">To be added.</param>
	/// <summary>A delegate that specifies the callback for the <see cref="Foundation.NSAttributedString.EnumerateAttribute(Foundation.NSString,Foundation.NSRange,Foundation.NSAttributedStringEnumeration,Foundation.NSAttributedStringCallback)" /> method.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSAttributedStringCallback (NSObject value, NSRange range, ref bool stop);

	/// <param name="url">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>A delegate that specifies the error handler for use in <see cref="Foundation.NSFileManager.GetEnumerator(Foundation.NSUrl,Foundation.NSString[],Foundation.NSDirectoryEnumerationOptions,Foundation.NSEnumerateErrorHandler)" />.</summary>
	/// <returns>To be added.</returns>
	/// <remarks>To be added.</remarks>
	delegate bool NSEnumerateErrorHandler (NSUrl url, NSError error);
	/// <param name="result">To be added.</param>
	/// <param name="idx">To be added.</param>
	/// <param name="stop">To be added.</param>
	/// <summary>The delegate used as the callback in calls to <see cref="Foundation.NSMetadataQuery.EnumerateResultsUsingBlock(Foundation.NSMetadataQueryEnumerationCallback)" /> and <see cref="Foundation.NSMetadataQuery.EnumerateResultsWithOptions(Foundation.NSEnumerationOptions,Foundation.NSMetadataQueryEnumerationCallback)" />.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSMetadataQueryEnumerationCallback (NSObject result, nuint idx, ref bool stop);
	/// <param name="itemBeingLoaded">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>The completion handler used with <see cref="Foundation.NSItemProviderLoadHandler" /> delegates.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSItemProviderCompletionHandler (INSSecureCoding itemBeingLoaded, NSError error);
	/// <param name="completionHandler">To be added.</param>
	/// <param name="expectedValueClass">To be added.</param>
	/// <param name="options">To be added.</param>
	/// <summary>Defines the load handler for use with the <see cref="Foundation.NSItemProvider.RegisterItemForTypeIdentifier(System.String,Foundation.NSItemProviderLoadHandler)" /> and <see cref="Foundation.NSItemProvider.SetPreviewImageHandler(Foundation.NSItemProviderLoadHandler)" /> methods.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSItemProviderLoadHandler ([BlockCallback, NullAllowed] NSItemProviderCompletionHandler completionHandler, [NullAllowed] Class expectedValueClass, [NullAllowed] NSDictionary options);
	/// <param name="date">To be added.</param>
	/// <param name="exactMatch">To be added.</param>
	/// <param name="stop">To be added.</param>
	/// <summary>Completion handler for the <see cref="Foundation.NSCalendar.EnumerateDatesStartingAfterDate(Foundation.NSDate,Foundation.NSDateComponents,Foundation.NSCalendarOptions,Foundation.EnumerateDatesCallback)" /> method.</summary>
	/// <remarks>To be added.</remarks>
	delegate void EnumerateDatesCallback (NSDate date, bool exactMatch, ref bool stop);
	/// <param name="idx">To be added.</param>
	/// <param name="stop">To be added.</param>
	/// <summary>Defines the enumerator callback in calls to <see cref="Foundation.NSIndexSet.EnumerateIndexes(Foundation.NSRange,Foundation.NSEnumerationOptions,Foundation.EnumerateIndexSetCallback)" />.</summary>
	/// <remarks>To be added.</remarks>
	delegate void EnumerateIndexSetCallback (nuint idx, ref bool stop);
	delegate void CloudKitRegistrationPreparationAction ([BlockCallback] CloudKitRegistrationPreparationHandler handler);
	delegate void CloudKitRegistrationPreparationHandler (CKShare share, CKContainer container, NSError error);

	[BaseType (typeof (NSObject))]
	interface NSAutoreleasePool {
	}

	interface NSArray<TValue> : NSArray { }

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {
		[Export ("count")]
		nuint Count { get; }

		/// <param name="idx">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("objectAtIndex:")]
		NativeHandle ValueAt (nuint idx);

		[Static]
		[Internal]
		[Export ("arrayWithObjects:count:")]
		IntPtr FromObjects (IntPtr array, nint count);

		[Export ("valueForKey:")]
		[MarshalNativeExceptions]
		NSObject ValueForKey (NSString key);

		[Export ("setValue:forKey:")]
		void SetValueForKey (NSObject value, NSString key);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Write (NSUrl, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Write (NSUrl, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Write (NSUrl, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Write (NSUrl, out NSError)' instead.")]
		[Export ("writeToFile:atomically:")]
		bool WriteToFile (string path, bool useAuxiliaryFile);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSMutableArray.FromFile' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSMutableArray.FromFile' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSMutableArray.FromFile' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSMutableArray.FromFile' instead.")]
		[Export ("arrayWithContentsOfFile:")]
		[Static]
		NSArray FromFile (string path);

		[Export ("sortedArrayUsingComparator:")]
		NSArray Sort (NSComparator cmptr);

		[Export ("filteredArrayUsingPredicate:")]
		NSArray Filter (NSPredicate predicate);

		[Internal]
		[Sealed]
		[Export ("containsObject:")]
		bool _Contains (NativeHandle anObject);

		[Export ("containsObject:")]
		bool Contains (NSObject anObject);

		[Internal]
		[Sealed]
		[Export ("indexOfObject:")]
		nuint _IndexOf (NativeHandle anObject);

		[Export ("indexOfObject:")]
		nuint IndexOf (NSObject anObject);

		[Export ("addObserver:toObjectsAtIndexes:forKeyPath:options:context:")]
		void AddObserver (NSObject observer, NSIndexSet indexes, string keyPath, NSKeyValueObservingOptions options, IntPtr context);

		[Export ("removeObserver:fromObjectsAtIndexes:forKeyPath:context:")]
		void RemoveObserver (NSObject observer, NSIndexSet indexes, string keyPath, IntPtr context);

		[Export ("removeObserver:fromObjectsAtIndexes:forKeyPath:")]
		void RemoveObserver (NSObject observer, NSIndexSet indexes, string keyPath);

		[MacCatalyst (13, 1)]
		[Export ("writeToURL:error:")]
		bool Write (NSUrl url, out NSError error);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("arrayWithContentsOfURL:error:")]
		[return: NullAllowed]
		NSArray FromUrl (NSUrl url, out NSError error);

#if false // https://github.com/dotnet/macios/issues/15577
		[Internal]
		[Export ("differenceFromArray:withOptions:")]
		NativeHandle _GetDifference (NSArray other, NSOrderedCollectionDifferenceCalculationOptions options);

		[Wrap ("Runtime.GetNSObject <NSOrderedCollectionDifference> (_GetDifference (NSArray.FromNSObjects (other), options))")]
		[return: NullAllowed]
		NSOrderedCollectionDifference GetDifference (NSObject[] other, NSOrderedCollectionDifferenceCalculationOptions options);

		[Internal]
		[Export ("differenceFromArray:")]
		NativeHandle _GetDifference (NSArray other);

		[Wrap ("Runtime.GetNSObject <NSOrderedCollectionDifference> (_GetDifference(NSArray.FromNSObjects (other)))")]
		[return: NullAllowed]
		NSOrderedCollectionDifference GetDifference (NSObject[] other);

		[Export ("arrayByApplyingDifference:")]
		[return: NullAllowed]
		NativeHandle _GetArrayByApplyingDifference (NSOrderedCollectionDifference difference);

		[Wrap ("NSArray.ArrayFromHandle<NSObject> (_GetArrayByApplyingDifference (difference))")]
		[return: NullAllowed]
		NSObject[] GetArrayByApplyingDifference (NSOrderedCollectionDifference difference);

		[Internal]
		[Export ("differenceFromArray:withOptions:usingEquivalenceTest:")]
		NativeHandle _GetDifferenceFromArray (NSArray other, NSOrderedCollectionDifferenceCalculationOptions options, /* Func<NSObject, NSObject, bool> block */ ref BlockLiteral block);
#endif
	}

	[BaseType (typeof (NSObject))]
	partial interface NSAttributedString : NSCoding, NSMutableCopying, NSSecureCoding
#if MONOMAC
		, NSPasteboardReading, NSPasteboardWriting
#endif
#if IOS
		, NSItemProviderReading, NSItemProviderWriting
#endif
	{
		// Inlined from the NSAttributedStringAttachmentConveniences category
		/// <param name="attachment">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static, Export ("attributedStringWithAttachment:")]
		NSAttributedString FromAttachment (NSTextAttachment attachment);

		// Inlined from the NSAttributedStringAttachmentConveniences category
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Static, Export ("attributedStringWithAttachment:attributes:")]
		NSAttributedString FromAttachment (NSTextAttachment attachment, NSDictionary<NSString, NSObject> attributes);

		[Export ("string")]
		IntPtr LowLevelValue { get; }

		[Export ("attributesAtIndex:effectiveRange:")]
		IntPtr LowLevelGetAttributes (nint location, IntPtr effectiveRange);

		[Export ("length")]
		nint Length { get; }

		// TODO: figure out the type, this deserves to be strongly typed if possble
		/// <param name="attribute">To be added.</param>
		/// <param name="location">To be added.</param>
		/// <param name="effectiveRange">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("attribute:atIndex:effectiveRange:")]
		NSObject GetAttribute (string attribute, nint location, out NSRange effectiveRange);

		[Export ("attributedSubstringFromRange:"), Internal]
		NSAttributedString Substring (NSRange range);

		/// <param name="location">To be added.</param>
		/// <param name="longestEffectiveRange">To be added.</param>
		/// <param name="rangeLimit">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("attributesAtIndex:longestEffectiveRange:inRange:")]
		NSDictionary GetAttributes (nint location, out NSRange longestEffectiveRange, NSRange rangeLimit);

		/// <param name="attribute">To be added.</param>
		/// <param name="location">To be added.</param>
		/// <param name="longestEffectiveRange">To be added.</param>
		/// <param name="rangeLimit">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("attribute:atIndex:longestEffectiveRange:inRange:")]
		NSObject GetAttribute (string attribute, nint location, out NSRange longestEffectiveRange, NSRange rangeLimit);

		[Export ("isEqualToAttributedString:")]
		bool IsEqual (NSAttributedString other);

		[Export ("initWithString:")]
		NativeHandle Constructor (string str);

		[Export ("initWithString:attributes:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NativeHandle Constructor (string str, [NullAllowed] NSDictionary attributes);

		[Export ("initWithAttributedString:")]
		NativeHandle Constructor (NSAttributedString other);

		[Export ("enumerateAttributesInRange:options:usingBlock:")]
		void EnumerateAttributes (NSRange range, NSAttributedStringEnumeration options, NSAttributedRangeCallback callback);

		[Export ("enumerateAttribute:inRange:options:usingBlock:")]
		void EnumerateAttribute (NSString attributeName, NSRange inRange, NSAttributedStringEnumeration options, NSAttributedStringCallback callback);

#if !XAMCORE_5_0
		/// <param name="url">To be added.</param>
		/// <param name="options">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="resultDocumentAttributes">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Obsolete ("Use the 'Create' method instead, because there's no way to return an error from a constructor.")]
		[Export ("initWithURL:options:documentAttributes:error:")]
#if !__MACOS__
		NativeHandle Constructor (NSUrl url, NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);
#else
		NativeHandle Constructor (NSUrl url, NSDictionary options, out NSDictionary resultDocumentAttributes, out NSError error);
#endif
#endif // !XAMCORE_5_0

		[Internal]
		[Sealed]
		[Export ("initWithURL:options:documentAttributes:error:")]
		NativeHandle _InitWithUrl (NSUrl url, NSDictionary options, out NSDictionary resultDocumentAttributes, out NSError error);

#if !XAMCORE_5_0
#if __MACOS__
		/// <param name="data">To be added.</param>
		/// <param name="options">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="docAttributes">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
#else
		/// <param name="data">To be added.</param>
		/// <param name="options">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="resultDocumentAttributes">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
#endif
		[Obsolete ("Use the 'Create' method instead, because there's no way to return an error from a constructor.")]
		[Export ("initWithData:options:documentAttributes:error:")]
#if __MACOS__
		NativeHandle Constructor (NSData data, NSDictionary options, out NSDictionary docAttributes, out NSError error);
#else
		NativeHandle Constructor (NSData data, NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);
#endif
#endif // !XAMCORE_5_0

		[Internal]
		[Sealed]
		[Export ("initWithData:options:documentAttributes:error:")]
		NativeHandle _InitWithData (NSData data, NSDictionary options, out NSDictionary resultDocumentAttributes, out NSError error);

#if !XAMCORE_5_0
		/// <param name="url">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="resultDocumentAttributes">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Obsolete ("Use the 'Create' method instead, because there's no way to return an error from a constructor.")]
#if __MACOS__
		[Wrap ("this (url, options.GetDictionary ()!, out resultDocumentAttributes, out error)")]
		NativeHandle Constructor (NSUrl url, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, out NSError error);
#else
		[Wrap ("this (url, options.GetDictionary ()!, out resultDocumentAttributes, ref error)")]
		NativeHandle Constructor (NSUrl url, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, ref NSError error);
#endif
#endif // !XAMCORE_5_0

		/// <param name="data">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="resultDocumentAttributes">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Obsolete ("Use the 'Create' method instead, because there's no way to return an error from a constructor.")]
#if !XAMCORE_5_0
#if __MACOS__
		[Wrap ("this (data, options.GetDictionary ()!, out resultDocumentAttributes, out error)")]
		NativeHandle Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, out NSError error);
#else
		[Wrap ("this (data, options.GetDictionary ()!, out resultDocumentAttributes, ref error)")]
		NativeHandle Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, ref NSError error);
#endif
#endif // !XAMCORE_5_0

		/// <param name="wordDocFormat">To be added.</param>
		/// <param name="docAttributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithDocFormat:documentAttributes:")]
		[Internal]
		NativeHandle _InitWithDocFormat (NSData wordDocFormat, out NSDictionary docAttributes);

#if !XAMCORE_5_0
		/// <param name="wordDocFormat">To be added.</param>
		/// <param name="docAttributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithDocFormat:documentAttributes:")]
		[Sealed]
		[Obsolete ("Use 'CreateWithDocFormat' instead.")]
		NativeHandle Constructor (NSData wordDocFormat, out NSDictionary docAttributes);
#endif

		/// <param name="htmlData">To be added.</param>
		/// <param name="baseUrl">To be added.</param>
		/// <param name="docAttributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithHTML:baseURL:documentAttributes:")]
		[Internal]
		NativeHandle _InitWithHTML (NSData htmlData, NSUrl baseUrl, out NSDictionary docAttributes);

#if !XAMCORE_5_0
		/// <param name="htmlData">To be added.</param>
		/// <param name="baseUrl">To be added.</param>
		/// <param name="docAttributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithHTML:baseURL:documentAttributes:")]
		[Sealed]
		[Obsolete ("Use 'CreateWithHTML' instead.")]
		NativeHandle Constructor (NSData htmlData, NSUrl baseUrl, out NSDictionary docAttributes);
#endif

		/// <param name="rect">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("drawWithRect:options:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options);

		/// <param name="path">To be added.</param>
		/// <param name="resultDocumentAttributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'Create (NSUrl, NSDictionary, out NSDictionary, out NSError)' instead.")]
		[Export ("initWithPath:documentAttributes:")]
		NativeHandle Constructor (string path, out NSDictionary resultDocumentAttributes);

		/// <param name="url">To be added.</param>
		/// <param name="resultDocumentAttributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'Create (NSUrl, NSDictionary, out NSDictionary, out NSError)' instead.")]
		[Export ("initWithURL:documentAttributes:")]
		NativeHandle Constructor (NSUrl url, out NSDictionary resultDocumentAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Internal, Export ("initWithRTF:documentAttributes:")]
		IntPtr _InitWithRtf (NSData data, out NSDictionary resultDocumentAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Internal, Export ("initWithRTFD:documentAttributes:")]
		IntPtr _InitWithRtfd (NSData data, out NSDictionary resultDocumentAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Internal, Export ("initWithHTML:documentAttributes:")]
		IntPtr _InitWithHTML (NSData data, out NSDictionary resultDocumentAttributes);

#if !XAMCORE_5_0
		/// <param name="data">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="resultDocumentAttributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithHTML:options:documentAttributes:")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CreateWithHTML (NSData, NSDictionary, out NSDictionary)' instead.")]
		[Sealed]
		NativeHandle Constructor (NSData data, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes);
#endif

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithHTML:options:documentAttributes:")]
		[Internal]
		NativeHandle _InitWithHTML (NSData data, NSDictionary options, out NSDictionary resultDocumentAttributes);

#if !XAMCORE_5_0
		/// <param name="data">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="resultDocumentAttributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Wrap ("this (data, options.GetDictionary (), out resultDocumentAttributes)")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'CreateWithHTML (NSData, NSAttributedStringDocumentAttributes, out NSDictionary)' instead.")]
		NativeHandle Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes);
#endif

#if !XAMCORE_5_0
		/// <param name="wrapper">To be added.</param>
		/// <param name="resultDocumentAttributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithRTFDFileWrapper:documentAttributes:")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'Create (NSFileWrapper, out NSDictionary)' instead.")]
		[Sealed]
		NativeHandle Constructor (NSFileWrapper wrapper, out NSDictionary resultDocumentAttributes);
#endif

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("initWithRTFDFileWrapper:documentAttributes:")]
		[Internal]
		NativeHandle _InitWithRTFDFileWrapper (NSFileWrapper wrapper, out NSDictionary resultDocumentAttributes);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("containsAttachments")]
		bool ContainsAttachments { get; }

		/// <param name="range">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("fontAttributesInRange:")]
		NSDictionary GetFontAttributes (NSRange range);

		/// <param name="range">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("rulerAttributesInRange:")]
		NSDictionary GetRulerAttributes (NSRange range);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("lineBreakBeforeIndex:withinRange:")]
		nuint GetLineBreak (nuint beforeIndex, NSRange aRange);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("lineBreakByHyphenatingBeforeIndex:withinRange:")]
		nuint GetLineBreakByHyphenating (nuint beforeIndex, NSRange aRange);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("doubleClickAtIndex:")]
		NSRange DoubleClick (nuint index);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("nextWordFromIndex:forward:")]
		nuint GetNextWord (nuint fromIndex, bool isForward);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSDataDetector' instead.")]
		[Export ("URLAtIndex:effectiveRange:")]
		NSUrl GetUrl (nuint index, out NSRange effectiveRange);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("rangeOfTextBlock:atIndex:")]
		NSRange GetRange (NSTextBlock textBlock, nuint index);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("rangeOfTextTable:atIndex:")]
		NSRange GetRange (NSTextTable textTable, nuint index);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("rangeOfTextList:atIndex:")]
		NSRange GetRange (NSTextList textList, nuint index);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("itemNumberInTextList:atIndex:")]
		nint GetItemNumber (NSTextList textList, nuint index);

#if !(MONOMAC || XAMCORE_5_0)
		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Sealed]
#endif
		[return: NullAllowed]
		[Export ("dataFromRange:documentAttributes:error:")]
		NSData GetData (NSRange range, NSDictionary options, out NSError error);

		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[return: NullAllowed]
		[Wrap ("this.GetData (range, options.GetDictionary ()!, out error)")]
		NSData GetData (NSRange range, NSAttributedStringDocumentAttributes options, out NSError error);

#if !(MONOMAC || XAMCORE_5_0)
		/// <param name="range">To be added.</param>
		///         <param name="attributes">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[return: NullAllowed]
		[Obsolete ("Use 'GetData' instead.")]
		[Export ("dataFromRange:documentAttributes:error:")]
		NSData GetDataFromRange (NSRange range, NSDictionary attributes, ref NSError error);
#endif

#if !(MONOMAC || XAMCORE_5_0)
		/// <param name="range">To be added.</param>
		///         <param name="documentAttributes">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[return: NullAllowed]
		[Obsolete ("Use 'GetData' instead.")]
		[Wrap ("GetDataFromRange (range, documentAttributes.GetDictionary ()!, ref error)")]
		NSData GetDataFromRange (NSRange range, NSAttributedStringDocumentAttributes documentAttributes, ref NSError error);
#endif

#if !(MONOMAC || XAMCORE_5_0)
		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Sealed]
#endif
		[return: NullAllowed]
		[Export ("fileWrapperFromRange:documentAttributes:error:")]
		NSFileWrapper GetFileWrapper (NSRange range, NSDictionary options, out NSError error);

#if !(MONOMAC || XAMCORE_5_0)
		/// <param name="range">To be added.</param>
		///         <param name="attributes">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[return: NullAllowed]
		[Obsolete ("Use 'GetFileWrapper' instead.")]
		[Export ("fileWrapperFromRange:documentAttributes:error:")]
		NSFileWrapper GetFileWrapperFromRange (NSRange range, NSDictionary attributes, ref NSError error);
#endif

		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[return: NullAllowed]
		[Wrap ("this.GetFileWrapper (range, options.GetDictionary ()!, out error)")]
		NSFileWrapper GetFileWrapper (NSRange range, NSAttributedStringDocumentAttributes options, out NSError error);

#if !(MONOMAC || XAMCORE_5_0)
		/// <param name="range">To be added.</param>
		///         <param name="documentAttributes">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[return: NullAllowed]
		[Obsolete ("Use 'GetFileWrapper' instead.")]
		[Wrap ("GetFileWrapperFromRange (range, documentAttributes.GetDictionary ()!, ref error)")]
		NSFileWrapper GetFileWrapperFromRange (NSRange range, NSAttributedStringDocumentAttributes documentAttributes, ref NSError error);
#endif

		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("RTFFromRange:documentAttributes:")]
		NSData GetRtf (NSRange range, [NullAllowed] NSDictionary options);

		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Wrap ("this.GetRtf (range, options.GetDictionary ())")]
		NSData GetRtf (NSRange range, NSAttributedStringDocumentAttributes options);

		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("RTFDFromRange:documentAttributes:")]
		NSData GetRtfd (NSRange range, [NullAllowed] NSDictionary options);

		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Wrap ("this.GetRtfd (range, options.GetDictionary ())")]
		NSData GetRtfd (NSRange range, NSAttributedStringDocumentAttributes options);

		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("RTFDFileWrapperFromRange:documentAttributes:")]
		NSFileWrapper GetRtfdFileWrapper (NSRange range, [NullAllowed] NSDictionary options);

		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Wrap ("this.GetRtfdFileWrapper (range, options.GetDictionary ())")]
		NSFileWrapper GetRtfdFileWrapper (NSRange range, NSAttributedStringDocumentAttributes options);

		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("docFormatFromRange:documentAttributes:")]
		NSData GetDocFormat (NSRange range, [NullAllowed] NSDictionary options);

		/// <param name="range">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Wrap ("this.GetDocFormat (range, options.GetDictionary ())")]
		NSData GetDocFormat (NSRange range, NSAttributedStringDocumentAttributes options);

		/// <param name="rect">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="context">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("drawWithRect:options:context:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options, [NullAllowed] NSStringDrawingContext context);

		/// <param name="size">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="context">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("boundingRectWithSize:options:context:")]
		CGRect GetBoundingRect (CGSize size, NSStringDrawingOptions options, [NullAllowed] NSStringDrawingContext context);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("size")]
		CGSize Size { get; }

		/// <param name="point">To be added.</param>
		///         <summary>Draws the string at the specified point.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("drawAtPoint:")]
		void DrawString (CGPoint point);

		/// <param name="rect">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("drawInRect:")]
		void DrawString (CGRect rect);

		// Inlined from the NSAttributedStringKitAdditions category
		/// <param name="range">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("containsAttachmentsInRange:")]
		bool ContainsAttachmentsInRange (NSRange range);

		// Inlined from the NSAttributedStringKitAdditions category
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("prefersRTFDInRange:")]
		bool PrefersRtfdInRange (NSRange range);

		// inlined from NSAttributedStringWebKitAdditions category (since they are all static members)

		[NoTV] // really inside WebKit
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("loadFromHTMLWithRequest:options:completionHandler:")]
		[PreSnippet ("GC.KeepAlive (WebKit.WKContentMode.Recommended); // no-op to ensure WebKit.framework is loaded into memory", Optimizable = true)]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		void LoadFromHtml (NSUrlRequest request, NSDictionary options, NSAttributedStringCompletionHandler completionHandler);

		[NoTV] // really inside WebKit
		[MacCatalyst (13, 1)]
		[Static]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[Wrap ("LoadFromHtml (request, options.GetDictionary ()!, completionHandler)")]
		void LoadFromHtml (NSUrlRequest request, NSAttributedStringDocumentAttributes options, NSAttributedStringCompletionHandler completionHandler);

		[NoTV] // really inside WebKit
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("loadFromHTMLWithFileURL:options:completionHandler:")]
		[PreSnippet ("GC.KeepAlive (WebKit.WKContentMode.Recommended); // no-op to ensure WebKit.framework is loaded into memory", Optimizable = true)]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		void LoadFromHtml (NSUrl fileUrl, NSDictionary options, NSAttributedStringCompletionHandler completionHandler);

		[NoTV] // really inside WebKit
		[MacCatalyst (13, 1)]
		[Static]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[Wrap ("LoadFromHtml (fileUrl, options.GetDictionary ()!, completionHandler)")]
		void LoadFromHtml (NSUrl fileUrl, NSAttributedStringDocumentAttributes options, NSAttributedStringCompletionHandler completionHandler);

		[NoTV] // really inside WebKit
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("loadFromHTMLWithString:options:completionHandler:")]
		[PreSnippet ("GC.KeepAlive (WebKit.WKContentMode.Recommended); // no-op to ensure WebKit.framework is loaded into memory", Optimizable = true)]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		void LoadFromHtml (string @string, NSDictionary options, NSAttributedStringCompletionHandler completionHandler);

		[NoTV] // really inside WebKit
		[MacCatalyst (13, 1)]
		[Static]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[Wrap ("LoadFromHtml (@string, options.GetDictionary ()!, completionHandler)")]
		void LoadFromHtml (string @string, NSAttributedStringDocumentAttributes options, NSAttributedStringCompletionHandler completionHandler);

		[NoTV] // really inside WebKit
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("loadFromHTMLWithData:options:completionHandler:")]
		[PreSnippet ("GC.KeepAlive (WebKit.WKContentMode.Recommended); // no-op to ensure WebKit.framework is loaded into memory", Optimizable = true)]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		void LoadFromHtml (NSData data, NSDictionary options, NSAttributedStringCompletionHandler completionHandler);

		[NoTV] // really inside WebKit
		[MacCatalyst (13, 1)]
		[Static]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[Wrap ("LoadFromHtml (data, options.GetDictionary ()!, completionHandler)")]
		void LoadFromHtml (NSData data, NSAttributedStringDocumentAttributes options, NSAttributedStringCompletionHandler completionHandler);

#if !XAMCORE_5_0
		[Obsolete ("Use the 'Create' method instead, because there's no way to return an error from a constructor.")]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithContentsOfMarkdownFileAtURL:options:baseURL:error:")]
		NativeHandle Constructor (NSUrl markdownFile, [NullAllowed] NSAttributedStringMarkdownParsingOptions options, [NullAllowed] NSUrl baseUrl, [NullAllowed] out NSError error);
#endif

		[Internal]
		[Sealed]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithContentsOfMarkdownFileAtURL:options:baseURL:error:")]
		NativeHandle _InitWithContentsOfMarkdownFile (NSUrl markdownFile, [NullAllowed] NSAttributedStringMarkdownParsingOptions options, [NullAllowed] NSUrl baseUrl, [NullAllowed] out NSError error);

#if !XAMCORE_5_0
		[Obsolete ("Use the 'Create' method instead, because there's no way to return an error from a constructor.")]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithMarkdown:options:baseURL:error:")]
		NativeHandle Constructor (NSData markdown, [NullAllowed] NSAttributedStringMarkdownParsingOptions options, [NullAllowed] NSUrl baseUrl, [NullAllowed] out NSError error);
#endif

		[Internal]
		[Sealed]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithMarkdown:options:baseURL:error:")]
		NativeHandle _InitWithMarkdown (NSData markdown, [NullAllowed] NSAttributedStringMarkdownParsingOptions options, [NullAllowed] NSUrl baseUrl, [NullAllowed] out NSError error);

#if !XAMCORE_5_0
		[Obsolete ("Use the 'Create' method instead, because there's no way to return an error from a constructor.")]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithMarkdownString:options:baseURL:error:")]
		NativeHandle Constructor (string markdownString, [NullAllowed] NSAttributedStringMarkdownParsingOptions options, [NullAllowed] NSUrl baseUrl, [NullAllowed] out NSError error);
#endif

		[Internal]
		[Sealed]
		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithMarkdownString:options:baseURL:error:")]
		NativeHandle _InitWithMarkdownString (string markdownString, [NullAllowed] NSAttributedStringMarkdownParsingOptions options, [NullAllowed] NSUrl baseUrl, [NullAllowed] out NSError error);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("attributedStringByInflectingString")]
		NSAttributedString AttributedStringByInflectingString { get; }

		/// <param name="size">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("boundingRectWithSize:options:")]
		CGRect BoundingRectWithSize (CGSize size, NSStringDrawingOptions options);

#if MONOMAC
		[Field ("NSTextLayoutSectionOrientation", "AppKit")]
#else
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextLayoutSectionOrientation", "UIKit")]
#endif
		NSString TextLayoutSectionOrientation { get; }

#if MONOMAC
		[Field ("NSTextLayoutSectionRange", "AppKit")]
#else
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextLayoutSectionRange", "UIKit")]
#endif
		NSString TextLayoutSectionRange { get; }

#if !XAMCORE_5_0
#if MONOMAC
		[Field ("NSTextLayoutSectionsAttribute", "AppKit")]
#else
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextLayoutSectionsAttribute", "UIKit")]
#endif
		NSString TextLayoutSectionsAttribute { get; }
#endif // !XAMCORE_5_0

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS, NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11)]
		[NoMacCatalyst]
		[Field ("NSUnderlineByWordMask", "AppKit")]
		nint UnderlineByWordMaskAttributeName { get; }

#if !XAMCORE_5_0
#if MONOMAC
		[Field ("NSTextScalingDocumentAttribute", "AppKit")]
#else
		[Field ("NSTextScalingDocumentAttribute", "UIKit")]
#endif
		[MacCatalyst (13, 1)]
		NSString TextScalingDocumentAttribute { get; }
#endif // !XAMCORE_5_0

#if !XAMCORE_5_0
#if MONOMAC
		[Field ("NSSourceTextScalingDocumentAttribute", "AppKit")]
#else
		[Field ("NSSourceTextScalingDocumentAttribute", "UIKit")]
#endif
		[MacCatalyst (13, 1)]
		NSString SourceTextScalingDocumentAttribute { get; }
#endif // !XAMCORE_5_0

#if !XAMCORE_5_0
#if MONOMAC
		[Field ("NSCocoaVersionDocumentAttribute", "AppKit")]
#else
		[Field ("NSCocoaVersionDocumentAttribute", "UIKit")]
#endif
		[MacCatalyst (13, 1)]
		NSString CocoaVersionDocumentAttribute { get; }
#endif // !XAMCORE_5_0

		// Inlined from the NSAttributedStringAdaptiveImageGlyphConveniences category
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Static]
		[Export ("attributedStringWithAdaptiveImageGlyph:attributes:")]
		NSAttributedString Create (NSAdaptiveImageGlyph adaptiveImageGlyph, NSDictionary<NSString, NSObject> attributes);
	}

	// we follow the API found in swift
	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	public enum NSAttributedStringNameKey {

		[Field ("NSAlternateDescriptionAttributeName")]
		AlternateDescription,

		[Field ("NSImageURLAttributeName")]
		ImageUrl,

		[Field ("NSInflectionRuleAttributeName")]
		InflectionRule,

		[Field ("NSInflectionAlternativeAttributeName")]
		InflectionAlternative,

		[Field ("NSInlinePresentationIntentAttributeName")]
		InlinePresentationIntent,

		[Field ("NSLanguageIdentifierAttributeName")]
		LanguageIdentifier,

		[TV (16, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Field ("NSMarkdownSourcePositionAttributeName")]
		MarkdownSourcePosition,

		[Field ("NSMorphologyAttributeName")]
		Morphology,

		[Field ("NSPresentationIntentAttributeName")]
		PresentationIntentAttributeName,

		[Field ("NSReplacementIndexAttributeName")]
		ReplacementIndex,

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSInflectionAgreementArgumentAttributeName")]
		InflectionAgreementArgument,

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSInflectionAgreementConceptAttributeName")]
		InflectionAgreementConcept,

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSInflectionReferentConceptAttributeName")]
		InflectionReferentConcept,

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Field ("NSLocalizedNumberFormatAttributeName")]
		LocalizedNumberFormat,

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Field ("NSListItemDelimiterAttributeName")]
		ListItemDelimiter,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NSGrammaticalCase : long {
		NotSet = 0,
		Nominative,
		Accusative,
		Dative,
		Genitive,
		Prepositional,
		Ablative,
		Adessive,
		Allative,
		Elative,
		Illative,
		Essive,
		Inessive,
		Locative,
		Translative,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NSGrammaticalPronounType : long {
		NotSet = 0,
		Personal,
		Reflexive,
		Possessive,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NSGrammaticalDefiniteness : long {
		NotSet = 0,
		Indefinite,
		Definite,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NSGrammaticalDetermination : long {
		NotSet = 0,
		Independent,
		Dependent,
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NSGrammaticalPerson : long {
		NotSet = 0,
		First,
		Second,
		Third,
	}

	[NoTV] // really inside WebKit
	[MacCatalyst (13, 1)]
	delegate void NSAttributedStringCompletionHandler ([NullAllowed] NSAttributedString attributedString, [NullAllowed] NSDictionary<NSString, NSObject> attributes, [NullAllowed] NSError error);

	[BaseType (typeof (NSObject),
		   Delegates = new string [] { "WeakDelegate" },
		   Events = new Type [] { typeof (NSCacheDelegate) })]
	interface NSCache {
		[Export ("objectForKey:")]
		NSObject ObjectForKey (NSObject key);

#if !XAMCORE_5_0
		[Obsolete ("Use 'SetObjectForKey' instead.")]
		[Export ("setObject:forKey:")]
		void SetObjectforKey (NSObject obj, NSObject key);
#endif

#if !XAMCORE_5_0
		[Sealed]
#endif
		[Export ("setObject:forKey:")]
		void SetObjectForKey (NSObject obj, NSObject key);

		/// <param name="obj">To be added.</param>
		/// <param name="key">To be added.</param>
		/// <param name="cost">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("setObject:forKey:cost:")]
		void SetCost (NSObject obj, NSObject key, nuint cost);

		[Export ("removeObjectForKey:")]
		void RemoveObjectForKey (NSObject key);

		[Export ("removeAllObjects")]
		void RemoveAllObjects ();

		//Detected properties
		[Export ("name")]
		string Name { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the Foundation.INSCacheDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSCacheDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		INSCacheDelegate Delegate { get; set; }

		[Export ("totalCostLimit")]
		nuint TotalCostLimit { get; set; }

		[Export ("countLimit")]
		nuint CountLimit { get; set; }

		[Export ("evictsObjectsWithDiscardedContent")]
		bool EvictsObjectsWithDiscardedContent { get; set; }
	}

	interface INSCacheDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSCacheDelegate {
		/// <param name="cache">To be added.</param>
		/// <param name="obj">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("cache:willEvictObject:"), EventArgs ("NSObject", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void WillEvictObject (NSCache cache, NSObject obj);
	}

	[BaseType (typeof (NSObject), Name = "NSCachedURLResponse")]
	// instance created with 'init' will crash when Dispose is called
	[DisableDefaultCtor]
	interface NSCachedUrlResponse : NSCoding, NSSecureCoding, NSCopying {
		[Export ("initWithResponse:data:userInfo:storagePolicy:")]
		NativeHandle Constructor (NSUrlResponse response, NSData data, [NullAllowed] NSDictionary userInfo, NSUrlCacheStoragePolicy storagePolicy);

		[Export ("initWithResponse:data:")]
		NativeHandle Constructor (NSUrlResponse response, NSData data);

		[Export ("response")]
		NSUrlResponse Response { get; }

		[Export ("data")]
		NSData Data { get; }

		[Export ("userInfo")]
		NSDictionary UserInfo { get; }

		[Export ("storagePolicy")]
		NSUrlCacheStoragePolicy StoragePolicy { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSCalendar : NSSecureCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithCalendarIdentifier:")]
		NativeHandle Constructor (NSString identifier);

		[Wrap ("this (calendarType.GetConstant ()!)")]
		NativeHandle Constructor (NSCalendarType calendarType);

		[Export ("calendarIdentifier")]
		string Identifier { get; }

		[Export ("currentCalendar")]
		[Static]
		NSCalendar CurrentCalendar { get; }

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; }

		[Export ("firstWeekday")]
		nuint FirstWeekDay { get; set; }

		[Export ("minimumDaysInFirstWeek")]
		nuint MinimumDaysInFirstWeek { get; set; }

		[Export ("components:fromDate:")]
		NSDateComponents Components (NSCalendarUnit unitFlags, NSDate fromDate);

		[Export ("components:fromDate:toDate:options:")]
		NSDateComponents Components (NSCalendarUnit unitFlags, NSDate fromDate, NSDate toDate, NSCalendarOptions opts);

		[Export ("dateByAddingComponents:toDate:options:")]
		NSDate DateByAddingComponents (NSDateComponents comps, NSDate date, NSCalendarOptions opts);

		[Export ("dateFromComponents:")]
		NSDate DateFromComponents (NSDateComponents comps);

		[Export ("eraSymbols")]
		string [] EraSymbols { get; }

		[Export ("longEraSymbols")]
		string [] LongEraSymbols { get; }

		[Export ("monthSymbols")]
		string [] MonthSymbols { get; }

		[Export ("shortMonthSymbols")]
		string [] ShortMonthSymbols { get; }

		[Export ("veryShortMonthSymbols")]
		string [] VeryShortMonthSymbols { get; }

		[Export ("standaloneMonthSymbols")]
		string [] StandaloneMonthSymbols { get; }

		[Export ("shortStandaloneMonthSymbols")]
		string [] ShortStandaloneMonthSymbols { get; }

		[Export ("veryShortStandaloneMonthSymbols")]
		string [] VeryShortStandaloneMonthSymbols { get; }

		[Export ("weekdaySymbols")]
		string [] WeekdaySymbols { get; }

		[Export ("shortWeekdaySymbols")]
		string [] ShortWeekdaySymbols { get; }

		[Export ("veryShortWeekdaySymbols")]
		string [] VeryShortWeekdaySymbols { get; }

		[Export ("standaloneWeekdaySymbols")]
		string [] StandaloneWeekdaySymbols { get; }

		[Export ("shortStandaloneWeekdaySymbols")]
		string [] ShortStandaloneWeekdaySymbols { get; }

		[Export ("veryShortStandaloneWeekdaySymbols")]
		string [] VeryShortStandaloneWeekdaySymbols { get; }

		[Export ("quarterSymbols")]
		string [] QuarterSymbols { get; }

		[Export ("shortQuarterSymbols")]
		string [] ShortQuarterSymbols { get; }

		[Export ("standaloneQuarterSymbols")]
		string [] StandaloneQuarterSymbols { get; }

		[Export ("shortStandaloneQuarterSymbols")]
		string [] ShortStandaloneQuarterSymbols { get; }

		[Export ("AMSymbol")]
		string AMSymbol { get; }

		[Export ("PMSymbol")]
		string PMSymbol { get; }

		[Export ("compareDate:toDate:toUnitGranularity:")]
		[MacCatalyst (13, 1)]
		NSComparisonResult CompareDate (NSDate date1, NSDate date2, NSCalendarUnit granularity);

		[Export ("component:fromDate:")]
		[MacCatalyst (13, 1)]
		nint GetComponentFromDate (NSCalendarUnit unit, NSDate date);

		[Export ("components:fromDateComponents:toDateComponents:options:")]
		[MacCatalyst (13, 1)]
		NSDateComponents ComponentsFromDateToDate (NSCalendarUnit unitFlags, NSDateComponents startingDate, NSDateComponents resultDate, NSCalendarOptions options);

		[Export ("componentsInTimeZone:fromDate:")]
		[MacCatalyst (13, 1)]
		NSDateComponents ComponentsInTimeZone (NSTimeZone timezone, NSDate date);

		[Export ("date:matchesComponents:")]
		[MacCatalyst (13, 1)]
		bool Matches (NSDate date, NSDateComponents components);

		/// <param name="unit">To be added.</param>
		/// <param name="value">To be added.</param>
		/// <param name="date">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("dateByAddingUnit:value:toDate:options:")]
		[MacCatalyst (13, 1)]
		NSDate DateByAddingUnit (NSCalendarUnit unit, nint value, NSDate date, NSCalendarOptions options);

		/// <param name="hour">To be added.</param>
		/// <param name="minute">To be added.</param>
		/// <param name="second">To be added.</param>
		/// <param name="date">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("dateBySettingHour:minute:second:ofDate:options:")]
		[MacCatalyst (13, 1)]
		NSDate DateBySettingsHour (nint hour, nint minute, nint second, NSDate date, NSCalendarOptions options);

		/// <param name="unit">To be added.</param>
		/// <param name="value">To be added.</param>
		/// <param name="date">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("dateBySettingUnit:value:ofDate:options:")]
		[MacCatalyst (13, 1)]
		NSDate DateBySettingUnit (NSCalendarUnit unit, nint value, NSDate date, NSCalendarOptions options);

		/// <param name="era">To be added.</param>
		/// <param name="year">To be added.</param>
		/// <param name="month">To be added.</param>
		/// <param name="date">To be added.</param>
		/// <param name="hour">To be added.</param>
		/// <param name="minute">To be added.</param>
		/// <param name="second">To be added.</param>
		/// <param name="nanosecond">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("dateWithEra:year:month:day:hour:minute:second:nanosecond:")]
		[MacCatalyst (13, 1)]
		NSDate Date (nint era, nint year, nint month, nint date, nint hour, nint minute, nint second, nint nanosecond);

		/// <param name="era">To be added.</param>
		/// <param name="year">To be added.</param>
		/// <param name="week">To be added.</param>
		/// <param name="weekday">To be added.</param>
		/// <param name="hour">To be added.</param>
		/// <param name="minute">To be added.</param>
		/// <param name="second">To be added.</param>
		/// <param name="nanosecond">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("dateWithEra:yearForWeekOfYear:weekOfYear:weekday:hour:minute:second:nanosecond:")]
		[MacCatalyst (13, 1)]
		NSDate DateForWeekOfYear (nint era, nint year, nint week, nint weekday, nint hour, nint minute, nint second, nint nanosecond);

		[Export ("enumerateDatesStartingAfterDate:matchingComponents:options:usingBlock:")]
		[MacCatalyst (13, 1)]
		void EnumerateDatesStartingAfterDate (NSDate start, NSDateComponents matchingComponents, NSCalendarOptions options, [BlockCallback] EnumerateDatesCallback callback);

		/// <param name="era">To be added.</param>
		/// <param name="year">To be added.</param>
		/// <param name="month">To be added.</param>
		/// <param name="day">To be added.</param>
		/// <param name="date">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("getEra:year:month:day:fromDate:")]
		[MacCatalyst (13, 1)]
		void GetComponentsFromDate (out nint era, out nint year, out nint month, out nint day, NSDate date);

		/// <param name="era">To be added.</param>
		/// <param name="year">To be added.</param>
		/// <param name="weekOfYear">To be added.</param>
		/// <param name="weekday">To be added.</param>
		/// <param name="date">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("getEra:yearForWeekOfYear:weekOfYear:weekday:fromDate:")]
		[MacCatalyst (13, 1)]
		void GetComponentsFromDateForWeekOfYear (out nint era, out nint year, out nint weekOfYear, out nint weekday, NSDate date);

		/// <param name="hour">To be added.</param>
		/// <param name="minute">To be added.</param>
		/// <param name="second">To be added.</param>
		/// <param name="nanosecond">To be added.</param>
		/// <param name="date">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("getHour:minute:second:nanosecond:fromDate:")]
		[MacCatalyst (13, 1)]
		void GetHourComponentsFromDate (out nint hour, out nint minute, out nint second, out nint nanosecond, NSDate date);

		[Export ("isDate:equalToDate:toUnitGranularity:")]
		[MacCatalyst (13, 1)]
		bool IsEqualToUnitGranularity (NSDate date1, NSDate date2, NSCalendarUnit unit);

		[Export ("isDate:inSameDayAsDate:")]
		[MacCatalyst (13, 1)]
		bool IsInSameDay (NSDate date1, NSDate date2);

		[Export ("isDateInToday:")]
		[MacCatalyst (13, 1)]
		bool IsDateInToday (NSDate date);

		[Export ("isDateInTomorrow:")]
		[MacCatalyst (13, 1)]
		bool IsDateInTomorrow (NSDate date);

		[Export ("isDateInWeekend:")]
		[MacCatalyst (13, 1)]
		bool IsDateInWeekend (NSDate date);

		[Export ("isDateInYesterday:")]
		[MacCatalyst (13, 1)]
		bool IsDateInYesterday (NSDate date);

		[Export ("nextDateAfterDate:matchingComponents:options:")]
		[MacCatalyst (13, 1)]
		[MarshalNativeExceptions]
		[return: NullAllowed]
		NSDate FindNextDateAfterDateMatching (NSDate date, NSDateComponents components, NSCalendarOptions options);

		/// <param name="date">To be added.</param>
		/// <param name="hour">To be added.</param>
		/// <param name="minute">To be added.</param>
		/// <param name="second">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("nextDateAfterDate:matchingHour:minute:second:options:")]
		[MacCatalyst (13, 1)]
		[MarshalNativeExceptions]
		[return: NullAllowed]
		NSDate FindNextDateAfterDateMatching (NSDate date, nint hour, nint minute, nint second, NSCalendarOptions options);

		/// <param name="date">To be added.</param>
		/// <param name="unit">To be added.</param>
		/// <param name="value">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("nextDateAfterDate:matchingUnit:value:options:")]
		[MacCatalyst (13, 1)]
		[MarshalNativeExceptions]
		[return: NullAllowed]
		NSDate FindNextDateAfterDateMatching (NSDate date, NSCalendarUnit unit, nint value, NSCalendarOptions options);

		[Export ("nextWeekendStartDate:interval:options:afterDate:")]
		[MacCatalyst (13, 1)]
		bool FindNextWeekend (out NSDate date, out double /* NSTimeInterval */ interval, NSCalendarOptions options, NSDate afterDate);

		[Export ("rangeOfWeekendStartDate:interval:containingDate:")]
		[MacCatalyst (13, 1)]
		bool RangeOfWeekendContainingDate (out NSDate weekendStartDate, out double /* NSTimeInterval */ interval, NSDate date);

		// although the ideal would be to use GetRange, we already have the method
		// RangeOfWeekendContainingDate and for the sake of consistency we are 
		// going to use the same name pattern.
		[Export ("minimumRangeOfUnit:")]
		NSRange MinimumRange (NSCalendarUnit unit);

		[Export ("maximumRangeOfUnit:")]
		NSRange MaximumRange (NSCalendarUnit unit);

		[Export ("rangeOfUnit:inUnit:forDate:")]
		NSRange Range (NSCalendarUnit smaller, NSCalendarUnit larger, NSDate date);

		[Export ("ordinalityOfUnit:inUnit:forDate:")]
		nuint Ordinality (NSCalendarUnit smaller, NSCalendarUnit larger, NSDate date);

		[Export ("rangeOfUnit:startDate:interval:forDate:")]
		bool Range (NSCalendarUnit unit, [NullAllowed] out NSDate datep, out double /* NSTimeInterval */ interval, NSDate date);

		[Export ("startOfDayForDate:")]
		[MacCatalyst (13, 1)]
		NSDate StartOfDayForDate (NSDate date);

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSCalendarDayChangedNotification")]
		NSString DayChangedNotification { get; }
	}

	/// <summary>Calendar types that can be used with the NSCalendar constructor.</summary>
	enum NSCalendarType {
		/// <summary>Gregorian calendar.</summary>
		[Field ("NSCalendarIdentifierGregorian")]
		Gregorian = 0,

		/// <summary>Buddhist calendar.</summary>
		[Field ("NSCalendarIdentifierBuddhist")]
		Buddhist = 1,

		/// <summary>Chinese calendar.</summary>
		[Field ("NSCalendarIdentifierChinese")]
		Chinese = 2,

		/// <summary>Hebrew calendar.</summary>
		[Field ("NSCalendarIdentifierHebrew")]
		Hebrew = 3,

		/// <summary>Islamic calendar.</summary>
		[Field ("NSIslamicCalendar")]
		Islamic = 4,

		/// <summary>IslamicCivil calendar.</summary>
		[Field ("NSCalendarIdentifierIslamicCivil")]
		IslamicCivil = 5,

		/// <summary>Japanese calendar.</summary>
		[Field ("NSCalendarIdentifierJapanese")]
		Japanese = 6,

#if !XAMCORE_5_0
		/// <summary>Calendar for Taiwan.</summary>
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Use 'RepublicOfChina' instead.")]
		Taiwan = 7,
#endif

		/// <summary>Calendar for the Republic of China.</summary>
		[Field ("NSCalendarIdentifierRepublicOfChina")]
		RepublicOfChina = 7,

		/// <summary>Persian calendar.</summary>
		[Field ("NSCalendarIdentifierPersian")]
		Persian = 8,

		/// <summary>Indian calendar.</summary>
		[Field ("NSCalendarIdentifierIndian")]
		Indian = 9,

		/// <summary>ISO8601 calendar.</summary>
		[Field ("NSCalendarIdentifierISO8601")]
		ISO8601 = 10,

		/// <summary>Coptic calendar.</summary>
		[Field ("NSCalendarIdentifierCoptic")]
		Coptic = 11,

		/// <summary>Ethiopic (Amete Alem) calendar.</summary>
		[Field ("NSCalendarIdentifierEthiopicAmeteAlem")]
		EthiopicAmeteAlem = 12,

		/// <summary>Ethiopic (Amete Mihret) calendar.</summary>
		[Field ("NSCalendarIdentifierEthiopicAmeteMihret")]
		EthiopicAmeteMihret = 13,

		/// <summary>Tabular Islamic calendar.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSCalendarIdentifierIslamicTabular")]
		IslamicTabular = 14,

		/// <summary>Islamic Umm al-Qura calendar.</summary>
		[MacCatalyst (13, 1)]
		[Field ("NSCalendarIdentifierIslamicUmmAlQura")]
		IslamicUmmAlQura = 15,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSCalendarIdentifierBangla")]
		Bangla,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSCalendarIdentifierGujarati")]
		Gujarati,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSCalendarIdentifierKannada")]
		Kannada,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSCalendarIdentifierMalayalam")]
		Malayalam,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSCalendarIdentifierMarathi")]
		Marathi,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSCalendarIdentifierOdia")]
		Odia,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSCalendarIdentifierTamil")]
		Tamil,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSCalendarIdentifierTelugu")]
		Telugu,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSCalendarIdentifierVikram")]
		Vikram,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSCalendarIdentifierDangi")]
		Dangi,

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSCalendarIdentifierVietnamese")]
		Vietnamese,
	}

	// Obsolete, but the only API surfaced by WebKit.WebHistory.
	[Deprecated (PlatformName.MacOSX, 10, 1, message: "Use NSCalendar and NSDateComponents.")]
	[NoiOS]
	[NoMacCatalyst]
	[NoTV]
	[BaseType (typeof (NSDate))]
	interface NSCalendarDate {
		[Export ("initWithString:calendarFormat:locale:")]
		[Deprecated (PlatformName.MacOSX, 10, 0)]
		NativeHandle Constructor (string description, string calendarFormat, [NullAllowed] NSObject locale);

		[Export ("initWithString:calendarFormat:")]
		[Deprecated (PlatformName.MacOSX, 10, 0)]
		NativeHandle Constructor (string description, string calendarFormat);

		[Export ("initWithString:")]
		[Deprecated (PlatformName.MacOSX, 10, 0)]
		NativeHandle Constructor (string description);

		[Export ("initWithYear:month:day:hour:minute:second:timeZone:")]
		[Deprecated (PlatformName.MacOSX, 10, 0)]
		NativeHandle Constructor (nint year, nuint month, nuint day, nuint hour, nuint minute, nuint second, [NullAllowed] NSTimeZone aTimeZone);

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("dateByAddingYears:months:days:hours:minutes:seconds:")]
		NSCalendarDate DateByAddingYears (nint year, nint month, nint day, nint hour, nint minute, nint second);

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("dayOfCommonEra")]
		nint DayOfCommonEra { get; }

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("dayOfMonth")]
		nint DayOfMonth { get; }

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("dayOfWeek")]
		nint DayOfWeek { get; }

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("dayOfYear")]
		nint DayOfYear { get; }

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("hourOfDay")]
		nint HourOfDay { get; }

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("minuteOfHour")]
		nint MinuteOfHour { get; }

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("monthOfYear")]
		nint MonthOfYear { get; }

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("secondOfMinute")]
		nint SecondOfMinute { get; }

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("yearOfCommonEra")]
		nint YearOfCommonEra { get; }

		[NullAllowed]
		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("calendarFormat")]
		string CalendarFormat { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("descriptionWithCalendarFormat:locale:")]
		string GetDescription (string calendarFormat, [NullAllowed] NSObject locale);

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("descriptionWithCalendarFormat:")]
		string GetDescription (string calendarFormat);

		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("descriptionWithLocale:")]
		string GetDescription ([NullAllowed] NSLocale locale);

		[NullAllowed]
		[Deprecated (PlatformName.MacOSX, 10, 0)]
		[Export ("timeZone")]
		NSTimeZone TimeZone { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSCharacterSet : NSSecureCoding, NSMutableCopying {
		[Static, Export ("alphanumericCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Alphanumerics { get; }

		[Static, Export ("capitalizedLetterCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Capitalized { get; }

		// TODO/FIXME: constructor?
		[Static, Export ("characterSetWithBitmapRepresentation:")]
		NSCharacterSet FromBitmap (NSData data);

		// TODO/FIXME: constructor?
		[Static, Export ("characterSetWithCharactersInString:")]
		NSCharacterSet FromString (string aString);

		[return: NullAllowed]
		[Static, Export ("characterSetWithContentsOfFile:")]
		NSCharacterSet FromFile (string path);

		[Static, Export ("characterSetWithRange:")]
		NSCharacterSet FromRange (NSRange aRange);

		[Static, Export ("controlCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Controls { get; }

		[Static, Export ("decimalDigitCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet DecimalDigits { get; }

		[Static, Export ("decomposableCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Decomposables { get; }

		[Static, Export ("illegalCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Illegals { get; }

		[Static, Export ("letterCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Letters { get; }

		[Static, Export ("lowercaseLetterCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet LowercaseLetters { get; }

		[Static, Export ("newlineCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Newlines { get; }

		[Static, Export ("nonBaseCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Marks { get; }

		[Static, Export ("punctuationCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Punctuation { get; }

		[Static, Export ("symbolCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Symbols { get; }

		[Static, Export ("uppercaseLetterCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UppercaseLetters { get; }

		[Static, Export ("whitespaceAndNewlineCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet WhitespaceAndNewlines { get; }

		[Static, Export ("whitespaceCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Whitespaces { get; }

		[Export ("bitmapRepresentation")]
		NSData GetBitmapRepresentation ();

		[Export ("characterIsMember:")]
		bool Contains (char aCharacter);

		[Export ("hasMemberInPlane:")]
		bool HasMemberInPlane (byte thePlane);

		[Export ("invertedSet")]
		NSCharacterSet InvertedSet { get; }

		[Export ("isSupersetOfSet:")]
		bool IsSupersetOf (NSCharacterSet theOtherSet);

		[Export ("longCharacterIsMember:")]
		bool Contains (uint /* UTF32Char = UInt32 */ theLongChar);

		[Static]
		[Export ("URLFragmentAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlFragmentAllowed { get; }

		[Static]
		[Export ("URLHostAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlHostAllowed { get; }

		[Static]
		[Export ("URLPasswordAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlPasswordAllowed { get; }

		[Static]
		[Export ("URLPathAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlPathAllowed { get; }

		[Static]
		[Export ("URLQueryAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlQueryAllowed { get; }

		[Static]
		[Export ("URLUserAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlUserAllowed { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter))]
	interface NSMassFormatter {
		[Export ("numberFormatter", ArgumentSemantic.Copy)]
		NSNumberFormatter NumberFormatter { get; set; }

		[Export ("unitStyle")]
		NSFormattingUnitStyle UnitStyle { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("forPersonMassUse")]
		bool ForPersonMassUse { [Bind ("isForPersonMassUse")] get; set; }

		[Export ("stringFromValue:unit:")]
		string StringFromValue (double value, NSMassFormatterUnit unit);

		[Export ("stringFromKilograms:")]
		string StringFromKilograms (double numberInKilograms);

		[Export ("unitStringFromValue:unit:")]
		string UnitStringFromValue (double value, NSMassFormatterUnit unit);

		[Export ("unitStringFromKilograms:usedUnit:")]
		string UnitStringFromKilograms (double numberInKilograms, ref NSMassFormatterUnit unitp);

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject obj, string str, out string error);
	}

	[BaseType (typeof (NSCharacterSet))]
	interface NSMutableCharacterSet {
		[Export ("addCharactersInRange:")]
		void AddCharacters (NSRange aRange);

		[Export ("removeCharactersInRange:")]
		void RemoveCharacters (NSRange aRange);

		[Export ("addCharactersInString:")]
		void AddCharacters (NSString str);

		[Export ("removeCharactersInString:")]
		void RemoveCharacters (NSString str);

		[Export ("formUnionWithCharacterSet:")]
		void UnionWith (NSCharacterSet otherSet);

		[Export ("formIntersectionWithCharacterSet:")]
		void IntersectWith (NSCharacterSet otherSet);

		[Export ("invert")]
		void Invert ();

		[MacCatalyst (13, 1)]
		[Static, Export ("alphanumericCharacterSet")]
		NSCharacterSet Alphanumerics { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("capitalizedLetterCharacterSet")]
		NSCharacterSet Capitalized { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("characterSetWithBitmapRepresentation:")]
		NSCharacterSet FromBitmapRepresentation (NSData data);

		[MacCatalyst (13, 1)]
		[Static, Export ("characterSetWithCharactersInString:")]
		NSCharacterSet FromString (string aString);

		[return: NullAllowed]
		[MacCatalyst (13, 1)]
		[Static, Export ("characterSetWithContentsOfFile:")]
		NSCharacterSet FromFile (string path);

		[MacCatalyst (13, 1)]
		[Static, Export ("characterSetWithRange:")]
		NSCharacterSet FromRange (NSRange aRange);

		[MacCatalyst (13, 1)]
		[Static, Export ("controlCharacterSet")]
		NSCharacterSet Controls { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("decimalDigitCharacterSet")]
		NSCharacterSet DecimalDigits { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("decomposableCharacterSet")]
		NSCharacterSet Decomposables { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("illegalCharacterSet")]
		NSCharacterSet Illegals { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("letterCharacterSet")]
		NSCharacterSet Letters { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("lowercaseLetterCharacterSet")]
		NSCharacterSet LowercaseLetters { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("newlineCharacterSet")]
		NSCharacterSet Newlines { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("nonBaseCharacterSet")]
		NSCharacterSet Marks { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("punctuationCharacterSet")]
		NSCharacterSet Punctuation { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("symbolCharacterSet")]
		NSCharacterSet Symbols { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("uppercaseLetterCharacterSet")]
		NSCharacterSet UppercaseLetters { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("whitespaceAndNewlineCharacterSet")]
		NSCharacterSet WhitespaceAndNewlines { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("whitespaceCharacterSet")]
		NSCharacterSet Whitespaces { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSCoder {

		//
		// Encoding and decoding
		//
		[Export ("encodeObject:")]
		void Encode ([NullAllowed] NSObject obj);

		[Export ("encodeRootObject:")]
		void EncodeRoot (NSObject obj);

		[return: NullAllowed]
		[Export ("decodeObject")]
		NSObject DecodeObject ();

		[Export ("decodeValueOfObjCType:at:size:")]
		void DecodeValue (IntPtr objCTypeCode, IntPtr data, nuint size);

		//
		// Encoding and decoding with keys
		// 
		[Export ("encodeConditionalObject:forKey:")]
		void EncodeConditionalObject ([NullAllowed] NSObject val, string key);

		[Export ("encodeConditionalObject:")]
		void EncodeConditionalObject ([NullAllowed] NSObject value);

		[Export ("encodeObject:forKey:")]
		void Encode ([NullAllowed] NSObject val, string key);

		[Export ("encodeBool:forKey:")]
		void Encode (bool val, string key);

		[Export ("encodeDouble:forKey:")]
		void Encode (double val, string key);

		[Export ("encodeFloat:forKey:")]
		void Encode (float /* float, not CGFloat */ val, string key);

		[Export ("encodeInt32:forKey:")]
		void Encode (int /* int32 */ val, string key);

		[Export ("encodeInt64:forKey:")]
		void Encode (long val, string key);

		/// <param name="val">Native integer value to encode.</param>
		/// <param name="key">Key to associate with the object being encoded.</param>
		/// <summary>Encodes the platform-specific native integer (32 or 64 bits) using the specified associated key.</summary>
		/// <remarks>
		///         </remarks>
		[Export ("encodeInteger:forKey:")]
		void Encode (nint val, string key);

		[Export ("encodeBytes:length:forKey:")]
		void EncodeBlock (IntPtr bytes, nint length, string key);

		[Export ("encodeBytes:length:")]
		void Encode (IntPtr bytes, nint length);

		[Export ("containsValueForKey:")]
		bool ContainsKey (string key);

		[Export ("decodeBoolForKey:")]
		bool DecodeBool (string key);

		[Export ("decodeDoubleForKey:")]
		double DecodeDouble (string key);

		[Export ("decodeFloatForKey:")]
		float DecodeFloat (string key); /* float, not CGFloat */

		[Export ("decodeInt32ForKey:")]
		int DecodeInt (string key); /* int, not NSInteger */

		[Export ("decodeInt64ForKey:")]
		long DecodeLong (string key);

		[Export ("decodeIntegerForKey:")]
		nint DecodeNInt (string key);

		[return: NullAllowed]
		[Export ("decodeObjectForKey:")]
		NSObject DecodeObject (string key);

		/// <param name="key">The key identifying the item to decode.</param>
		/// <param name="length">Number of bytes in the returned block.</param>
		/// <summary>Low-level: decodes the item with the associated key into a memory block,
		/// 	and returns a pointer to it.</summary>
		/// <returns>Pointer to the block of memory that contains at least
		/// 	the number of bytes set on the length parameter.</returns>
		/// <remarks>
		///         </remarks>
		[Export ("decodeBytesForKey:returnedLength:")]
		IntPtr DecodeBytes (string key, out nuint length);

		/// <param name="length">Number of bytes in the returned block.</param>
		/// <summary>Low-level: decodes the next item into a memory block,
		/// 	and returns a pointer to it.</summary>
		/// <returns>Pointer to the block of memory that contains at least
		/// 	the number of bytes set on the length parameter.</returns>
		/// <remarks>
		///         </remarks>
		[Export ("decodeBytesWithReturnedLength:")]
		IntPtr DecodeBytes (out nuint length);

		[NullAllowed]
		[Export ("allowedClasses")]
		NSSet AllowedClasses { get; }

		[Export ("requiresSecureCoding")]
		bool RequiresSecureCoding ();

		[return: NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("decodeTopLevelObjectAndReturnError:")]
		NSObject DecodeTopLevelObject ([NullAllowed] out NSError error);

		[return: NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("decodeTopLevelObjectForKey:error:")]
		NSObject DecodeTopLevelObject (string key, [NullAllowed] out NSError error);

		[return: NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("decodeTopLevelObjectOfClass:forKey:error:")]
		NSObject DecodeTopLevelObject (Class klass, string key, [NullAllowed] out NSError error);

		[return: NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("decodeTopLevelObjectOfClasses:forKey:error:")]
		NSObject DecodeTopLevelObject ([NullAllowed] NSSet<Class> setOfClasses, string key, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("failWithError:")]
		void Fail (NSError error);

		[Export ("systemVersion")]
		uint SystemVersion { get; }

		[MacCatalyst (13, 1)]
		[Export ("decodingFailurePolicy")]
		NSDecodingFailurePolicy DecodingFailurePolicy { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("error", ArgumentSemantic.Copy)]
		NSError Error { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("decodeArrayOfObjectsOfClass:forKey:")]
		[return: NullAllowed]
		NSObject [] DecodeArrayOfObjects (Class @class, string key);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("decodeArrayOfObjectsOfClasses:forKey:")]
		[return: NullAllowed]
		NSObject [] DecodeArrayOfObjects (NSSet<Class> classes, string key);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("decodeDictionaryWithKeysOfClass:objectsOfClass:forKey:")]
		[return: NullAllowed]
		NSDictionary DecodeDictionary (Class keyClass, Class objectClass, string key);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("decodeDictionaryWithKeysOfClasses:objectsOfClasses:forKey:")]
		[return: NullAllowed]
		NSDictionary DecodeDictionary (NSSet<Class> keyClasses, NSSet<Class> objectClasses, string key);

		[TV (18, 4), iOS (18, 4), MacCatalyst (18, 4), Mac (15, 4)]
		[Export ("decodeBytesWithMinimumLength:")]
		IntPtr DecodeBytes (nuint minimumLength);

		[TV (18, 4), iOS (18, 4), MacCatalyst (18, 4), Mac (15, 4)]
		[Export ("decodeBytesForKey:minimumLength:")]
		IntPtr DecodeBytes (string key, nuint minimumLength);

		[Export ("allowsKeyedCoding")]
		bool AllowsKeyedCoding { get; }

		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("encodePropertyList:")]
		void EncodePropertyList (NSObject aPropertyList);

		[NoiOS, NoTV, NoMacCatalyst]
		[return: NullAllowed]
		[Export ("decodePropertyList")]
		NSObject DecodePropertyList ();

		[Export ("decodePropertyListForKey:")]
		[return: NullAllowed]
		NSObject DecodePropertyList (string key);

		[Export ("decodeObjectOfClass:forKey:")]
		[return: NullAllowed]
		NSObject DecodeObject (Class @class, string key);

		[Wrap ("DecodeObject (new Class (type), key)")]
		[return: NullAllowed]
		NSObject DecodeObject (Type type, string key);

		[Export ("decodeObjectOfClasses:forKey:")]
		[return: NullAllowed]
		NSObject DecodeObject ([NullAllowed] NSSet<Class> classes, string key);

		[Wrap ("DecodeObject (new NSSet<Class> (Array.ConvertAll (types, t => new Class (t))), key)")]
		[return: NullAllowed]
		NSObject DecodeObject (Type [] types, string key);

		[Export ("encodeBycopyObject:")]
		void EncodeBycopyObject ([NullAllowed] NSObject anObject);

		[Export ("encodeByrefObject:")]
		void EncodeByrefObject ([NullAllowed] NSObject anObject);

		[Export ("encodeDataObject:")]
		void Encode (NSData data);

		[return: NullAllowed]
		[Export ("decodeDataObject")]
		NSData DecodeDataObject ();

		// from the NSGeometryCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("encodePoint:")]
		void Encode (CGPoint point);

		// from the NSGeometryCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("decodePoint")]
		CGPoint DecodePoint ();

		// from the NSGeometryCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("encodeSize:")]
		void Encode (CGSize size);

		// from the NSGeometryCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("decodeSize")]
		CGSize DecodeSize ();

		// from the NSGeometryCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("encodeRect:")]
		void Encode (CGRect rect);

		// from the NSGeometryCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("decodeRect")]
		CGRect DecodeRect ();

		// from the NSGeometryKeyedCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("encodePoint:forKey:")]
		void Encode (CGPoint point, string key);

		// from the NSGeometryKeyedCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("encodeSize:forKey:")]
		void Encode (CGSize size, string key);

		// from the NSGeometryKeyedCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("encodeRect:forKey:")]
		void Encode (CGRect rect, string key);

		// from the NSGeometryKeyedCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("decodePointForKey:")]
		CGPoint DecodePoint (string key);

		// from the NSGeometryKeyedCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("decodeSizeForKey:")]
		CGSize DecodeSize (string key);

		// from the NSGeometryKeyedCoding (NSCoder) category
		[NoiOS, NoTV, MacCatalyst (13, 1)]
		[Export ("decodeRectForKey:")]
		CGRect DecodeRect (string key);
	}

	[BaseType (typeof (NSPredicate))]
	interface NSComparisonPredicate : NSSecureCoding {
		[Static, Export ("predicateWithLeftExpression:rightExpression:modifier:type:options:")]
		NSComparisonPredicate Create (NSExpression leftExpression, NSExpression rightExpression, NSComparisonPredicateModifier comparisonModifier, NSPredicateOperatorType operatorType, NSComparisonPredicateOptions comparisonOptions);

		[Static, Export ("predicateWithLeftExpression:rightExpression:customSelector:")]
		NSComparisonPredicate FromSelector (NSExpression leftExpression, NSExpression rightExpression, Selector selector);

		[DesignatedInitializer]
		[Export ("initWithLeftExpression:rightExpression:modifier:type:options:")]
		NativeHandle Constructor (NSExpression leftExpression, NSExpression rightExpression, NSComparisonPredicateModifier comparisonModifier, NSPredicateOperatorType operatorType, NSComparisonPredicateOptions comparisonOptions);

		[DesignatedInitializer]
		[Export ("initWithLeftExpression:rightExpression:customSelector:")]
		NativeHandle Constructor (NSExpression leftExpression, NSExpression rightExpression, Selector selector);

		[Export ("predicateOperatorType")]
		NSPredicateOperatorType PredicateOperatorType { get; }

		[Export ("comparisonPredicateModifier")]
		NSComparisonPredicateModifier ComparisonPredicateModifier { get; }

		[Export ("leftExpression")]
		NSExpression LeftExpression { get; }

		[Export ("rightExpression")]
		NSExpression RightExpression { get; }

		[NullAllowed]
		[Export ("customSelector")]
		Selector CustomSelector { get; }

		[Export ("options")]
		NSComparisonPredicateOptions Options { get; }
	}

	[BaseType (typeof (NSPredicate))]
	[DisableDefaultCtor] // An uncaught exception was raised: Can't have a NOT predicate with no subpredicate.
	interface NSCompoundPredicate : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithType:subpredicates:")]
		NativeHandle Constructor (NSCompoundPredicateType type, NSPredicate [] subpredicates);

		[Export ("compoundPredicateType")]
		NSCompoundPredicateType Type { get; }

		[Export ("subpredicates")]
		NSPredicate [] Subpredicates { get; }

		[Static]
		[Export ("andPredicateWithSubpredicates:")]
		NSCompoundPredicate CreateAndPredicate (NSPredicate [] subpredicates);

		[Static]
		[Export ("orPredicateWithSubpredicates:")]
		NSCompoundPredicate CreateOrPredicate (NSPredicate [] subpredicates);

		[Static]
		[Export ("notPredicateWithSubpredicate:")]
		NSCompoundPredicate CreateNotPredicate (NSPredicate predicate);

	}

	/// <param name="bytes">To be added.</param>
	/// <param name="range">To be added.</param>
	/// <param name="stop">To be added.</param>
	/// <summary>The delegate used to enumerate in calls to <see cref="Foundation.NSData.EnumerateByteRange(Foundation.NSDataByteRangeEnumerator)" />.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSDataByteRangeEnumerator (IntPtr bytes, NSRange range, ref bool stop);

	[BaseType (typeof (NSObject))]
	interface NSData : NSSecureCoding, NSMutableCopying, CKRecordValue {
		[Export ("dataWithContentsOfURL:")]
		[Static]
		NSData FromUrl (NSUrl url);

		[Export ("dataWithContentsOfURL:options:error:")]
		[Static]
		NSData FromUrl (NSUrl url, NSDataReadingOptions mask, out NSError error);

		[Export ("dataWithContentsOfFile:")]
		[Static]
		NSData FromFile (string path);

		[Export ("dataWithContentsOfFile:options:error:")]
		[Static]
		NSData FromFile (string path, NSDataReadingOptions mask, out NSError error);

		[Export ("dataWithData:")]
		[Static]
		NSData FromData (NSData source);

		[Export ("dataWithBytes:length:"), Static]
		NSData FromBytes (IntPtr bytes, nuint size);

		[Export ("dataWithBytesNoCopy:length:"), Static]
		NSData FromBytesNoCopy (IntPtr bytes, nuint size);

		[Export ("dataWithBytesNoCopy:length:freeWhenDone:"), Static]
		NSData FromBytesNoCopy (IntPtr bytes, nuint size, bool freeWhenDone);

		[Export ("bytes")]
		IntPtr Bytes { get; }

		[Export ("length")]
		nuint Length { get; [NotImplemented ("Not available on NSData, only available on NSMutableData")] set; }

		[Export ("writeToFile:options:error:")]
		[Internal]
		bool _Save (string file, nint options, IntPtr addr);

		[Export ("writeToURL:options:error:")]
		[Internal]
		bool _Save (NSUrl url, nint options, IntPtr addr);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Save (NSUrl,bool)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Save (NSUrl,bool)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Save (NSUrl,bool)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Save (NSUrl,bool)' instead.")]
		[Export ("writeToFile:atomically:")]
		bool Save (string path, bool atomically);

		[Export ("writeToURL:atomically:")]
		bool Save (NSUrl url, bool atomically);

		[Export ("subdataWithRange:")]
		NSData Subdata (NSRange range);

		[Export ("getBytes:length:")]
		void GetBytes (IntPtr buffer, nuint length);

		[Export ("getBytes:range:")]
		void GetBytes (IntPtr buffer, NSRange range);

		[Export ("rangeOfData:options:range:")]
		NSRange Find (NSData dataToFind, NSDataSearchOptions searchOptions, NSRange searchRange);

		[MacCatalyst (13, 1)]
		[Export ("initWithBase64EncodedString:options:")]
		NativeHandle Constructor (string base64String, NSDataBase64DecodingOptions options);

		[MacCatalyst (13, 1)]
		[Export ("initWithBase64EncodedData:options:")]
		NativeHandle Constructor (NSData base64Data, NSDataBase64DecodingOptions options);

		[MacCatalyst (13, 1)]
		[Export ("base64EncodedDataWithOptions:")]
		NSData GetBase64EncodedData (NSDataBase64EncodingOptions options);

		[MacCatalyst (13, 1)]
		[Export ("base64EncodedStringWithOptions:")]
		string GetBase64EncodedString (NSDataBase64EncodingOptions options);

		[MacCatalyst (13, 1)]
		[Export ("enumerateByteRangesUsingBlock:")]
		void EnumerateByteRange (NSDataByteRangeEnumerator enumerator);

		[MacCatalyst (13, 1)]
		[Export ("initWithBytesNoCopy:length:deallocator:")]
		NativeHandle Constructor (IntPtr bytes, nuint length, [NullAllowed] Action<IntPtr, nuint> deallocator);

		// NSDataCompression (NSData)

		[MacCatalyst (13, 1)]
		[Export ("decompressedDataUsingAlgorithm:error:")]
		[return: NullAllowed]
		NSData Decompress (NSDataCompressionAlgorithm algorithm, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("compressedDataUsingAlgorithm:error:")]
		[return: NullAllowed]
		NSData Compress (NSDataCompressionAlgorithm algorithm, [NullAllowed] out NSError error);
	}

	[BaseType (typeof (NSRegularExpression))]
	interface NSDataDetector : NSCopying, NSCoding {
		[DesignatedInitializer]
		[Export ("initWithTypes:error:")]
		NativeHandle Constructor (NSTextCheckingTypes options, out NSError error);

		[Wrap ("this ((NSTextCheckingTypes) options, out error)")]
		NativeHandle Constructor (NSTextCheckingType options, out NSError error);

		[Export ("dataDetectorWithTypes:error:"), Static]
		NSDataDetector Create (NSTextCheckingTypes checkingTypes, out NSError error);

		[Static]
		[Wrap ("Create ((NSTextCheckingTypes) checkingTypes, out error)")]
		NSDataDetector Create (NSTextCheckingType checkingTypes, out NSError error);

		[Export ("checkingTypes")]
		NSTextCheckingTypes CheckingTypes { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSDateComponents : NSSecureCoding, NSCopying, INSCopying, INSSecureCoding, INativeObject {
		[NullAllowed] // by default this property is null
		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; set; }

		[Export ("quarter")]
		nint Quarter { get; set; }

		[Export ("date")]
		NSDate Date { get; }

		//Detected properties
		[Export ("era")]
		nint Era { get; set; }

		[Export ("year")]
		nint Year { get; set; }

		[Export ("month")]
		nint Month { get; set; }

		[Export ("day")]
		nint Day { get; set; }

		[Export ("hour")]
		nint Hour { get; set; }

		[Export ("minute")]
		nint Minute { get; set; }

		[Export ("second")]
		nint Second { get; set; }

		[Export ("nanosecond")]
		nint Nanosecond { get; set; }

		[Export ("week")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'WeekOfMonth' or 'WeekOfYear' instead.")]
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'WeekOfMonth' or 'WeekOfYear' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'WeekOfMonth' or 'WeekOfYear' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WeekOfMonth' or 'WeekOfYear' instead.")]
		nint Week { get; set; }

		[Export ("weekday")]
		nint Weekday { get; set; }

		[Export ("weekdayOrdinal")]
		nint WeekdayOrdinal { get; set; }

		[Export ("weekOfMonth")]
		nint WeekOfMonth { get; set; }

		[Export ("weekOfYear")]
		nint WeekOfYear { get; set; }

		[Export ("yearForWeekOfYear")]
		nint YearForWeekOfYear { get; set; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("dayOfYear")]
		nint DayOfYear { get; set; }

		/// <summary>Whether the month is a leap month.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("leapMonth")]
		bool IsLeapMonth { [Bind ("isLeapMonth")] get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isValidDate")]
		[MacCatalyst (13, 1)]
		bool IsValidDate { get; }

		[Export ("isValidDateInCalendar:")]
		[MacCatalyst (13, 1)]
		bool IsValidDateInCalendar (NSCalendar calendar);

		/// <param name="value">To be added.</param>
		/// <param name="unit">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("setValue:forComponent:")]
		[MacCatalyst (13, 1)]
		void SetValueForComponent (nint value, NSCalendarUnit unit);

		[Export ("valueForComponent:")]
		[MacCatalyst (13, 1)]
		nint GetValueForComponent (NSCalendarUnit unit);

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("repeatedDay")]
		bool IsRepeatedDay {
			[Bind ("isRepeatedDay")]
			get;
			set;
		}
	}

	[BaseType (typeof (NSFormatter))]
	interface NSByteCountFormatter {
		[Export ("allowsNonnumericFormatting")]
		bool AllowsNonnumericFormatting { get; set; }

		[Export ("includesUnit")]
		bool IncludesUnit { get; set; }

		[Export ("includesCount")]
		bool IncludesCount { get; set; }

		[Export ("includesActualByteCount")]
		bool IncludesActualByteCount { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("adaptive")]
		bool Adaptive { [Bind ("isAdaptive")] get; set; }

		[Export ("zeroPadsFractionDigits")]
		bool ZeroPadsFractionDigits { get; set; }

		[Static]
		[Export ("stringFromByteCount:countStyle:")]
		string Format (long byteCount, NSByteCountFormatterCountStyle countStyle);

		[Export ("stringFromByteCount:")]
		string Format (long byteCount);

		[Export ("allowedUnits")]
		NSByteCountFormatterUnits AllowedUnits { get; set; }

		[Export ("countStyle")]
		NSByteCountFormatterCountStyle CountStyle { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("formattingContext")]
		NSFormattingContext FormattingContext { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("stringForObjectValue:")]
		[return: NullAllowed]
		string GetString ([NullAllowed] NSObject obj);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("stringFromMeasurement:countStyle:")]
		string Create (NSUnitInformationStorage measurement, NSByteCountFormatterCountStyle countStyle);

		[MacCatalyst (13, 1)]
		[Export ("stringFromMeasurement:")]
		string Create (NSUnitInformationStorage measurement);
	}

	[BaseType (typeof (NSFormatter))]
	interface NSDateFormatter {
		[Export ("stringFromDate:")]
		string ToString (NSDate date);

		[Export ("dateFromString:")]
		NSDate Parse (string date);

		[Export ("dateFormat")]
		string DateFormat { get; set; }

		[Export ("dateStyle")]
		NSDateFormatterStyle DateStyle { get; set; }

		[Export ("timeStyle")]
		NSDateFormatterStyle TimeStyle { get; set; }

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("generatesCalendarDates")]
		bool GeneratesCalendarDates { get; set; }

		[Export ("formatterBehavior")]
		NSDateFormatterBehavior Behavior { get; set; }

		[Export ("defaultFormatterBehavior"), Static]
		NSDateFormatterBehavior DefaultBehavior { get; set; }

		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; set; }

		// not exposed as a property in documentation
		/// <summary>Whether this formatter uses heuristics when parsing a string.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isLenient")]
		bool IsLenient { get; [Bind ("setLenient:")] set; }

		[Export ("twoDigitStartDate", ArgumentSemantic.Copy)]
		NSDate TwoDigitStartDate { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("defaultDate", ArgumentSemantic.Copy)]
		NSDate DefaultDate { get; set; }

		[Export ("eraSymbols")]
		string [] EraSymbols { get; set; }

		[Export ("monthSymbols")]
		string [] MonthSymbols { get; set; }

		[Export ("shortMonthSymbols")]
		string [] ShortMonthSymbols { get; set; }

		[Export ("weekdaySymbols")]
		string [] WeekdaySymbols { get; set; }

		[Export ("shortWeekdaySymbols")]
		string [] ShortWeekdaySymbols { get; set; }

		[Export ("AMSymbol")]
		string AMSymbol { get; set; }

		[Export ("PMSymbol")]
		string PMSymbol { get; set; }

		[Export ("longEraSymbols")]
		string [] LongEraSymbols { get; set; }

		[Export ("veryShortMonthSymbols")]
		string [] VeryShortMonthSymbols { get; set; }

		[Export ("standaloneMonthSymbols")]
		string [] StandaloneMonthSymbols { get; set; }

		[Export ("shortStandaloneMonthSymbols")]
		string [] ShortStandaloneMonthSymbols { get; set; }

		[Export ("veryShortStandaloneMonthSymbols")]
		string [] VeryShortStandaloneMonthSymbols { get; set; }

		[Export ("veryShortWeekdaySymbols")]
		string [] VeryShortWeekdaySymbols { get; set; }

		[Export ("standaloneWeekdaySymbols")]
		string [] StandaloneWeekdaySymbols { get; set; }

		[Export ("shortStandaloneWeekdaySymbols")]
		string [] ShortStandaloneWeekdaySymbols { get; set; }

		[Export ("veryShortStandaloneWeekdaySymbols")]
		string [] VeryShortStandaloneWeekdaySymbols { get; set; }

		[Export ("quarterSymbols")]
		string [] QuarterSymbols { get; set; }

		[Export ("shortQuarterSymbols")]
		string [] ShortQuarterSymbols { get; set; }

		[Export ("standaloneQuarterSymbols")]
		string [] StandaloneQuarterSymbols { get; set; }

		[Export ("shortStandaloneQuarterSymbols")]
		string [] ShortStandaloneQuarterSymbols { get; set; }

		[Export ("gregorianStartDate", ArgumentSemantic.Copy)]
		NSDate GregorianStartDate { get; set; }

		[Export ("localizedStringFromDate:dateStyle:timeStyle:")]
		[Static]
		string ToLocalizedString (NSDate date, NSDateFormatterStyle dateStyle, NSDateFormatterStyle timeStyle);

		/// <param name="template">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="locale">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("dateFormatFromTemplate:options:locale:")]
		[Static]
		string GetDateFormatFromTemplate (string template, nuint options, [NullAllowed] NSLocale locale);

		[Export ("doesRelativeDateFormatting")]
		bool DoesRelativeDateFormatting { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("setLocalizedDateFormatFromTemplate:")]
		void SetLocalizedDateFormatFromTemplate (string dateFormatTemplate);

		[MacCatalyst (13, 1)]
		[Export ("formattingContext", ArgumentSemantic.Assign)]
		NSFormattingContext FormattingContext { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter))]
	interface NSDateComponentsFormatter {
		[Export ("unitsStyle")]
		NSDateComponentsFormatterUnitsStyle UnitsStyle { get; set; }

		[Export ("allowedUnits")]
		NSCalendarUnit AllowedUnits { get; set; }

		[Export ("zeroFormattingBehavior")]
		NSDateComponentsFormatterZeroFormattingBehavior ZeroFormattingBehavior { get; set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; set; }

		[Export ("allowsFractionalUnits")]
		bool AllowsFractionalUnits { get; set; }

		[Export ("maximumUnitCount")]
		nint MaximumUnitCount { get; set; }

		[Export ("collapsesLargestUnit")]
		bool CollapsesLargestUnit { get; set; }

		[Export ("includesApproximationPhrase")]
		bool IncludesApproximationPhrase { get; set; }

		[Export ("includesTimeRemainingPhrase")]
		bool IncludesTimeRemainingPhrase { get; set; }

		[Export ("formattingContext")]
		NSFormattingContext FormattingContext { get; set; }

		[Export ("stringForObjectValue:")]
		string StringForObjectValue ([NullAllowed] NSObject obj);

		[Export ("stringFromDateComponents:")]
		string StringFromDateComponents (NSDateComponents components);

		[Export ("stringFromDate:toDate:")]
		string StringFromDate (NSDate startDate, NSDate endDate);

		[Export ("stringFromTimeInterval:")]
		string StringFromTimeInterval (double ti);

		[Static, Export ("localizedStringFromDateComponents:unitsStyle:")]
		string LocalizedStringFromDateComponents (NSDateComponents components, NSDateComponentsFormatterUnitsStyle unitsStyle);

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject obj, string str, out string error);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("referenceDate", ArgumentSemantic.Copy)]
		NSDate ReferenceDate { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter))]
	interface NSDateIntervalFormatter {

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; set; }

		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; }

		[Export ("dateTemplate")]
		string DateTemplate { get; set; }

		[Export ("dateStyle")]
		NSDateIntervalFormatterStyle DateStyle { get; set; }

		[Export ("timeStyle")]
		NSDateIntervalFormatterStyle TimeStyle { get; set; }

		[Export ("stringFromDate:toDate:")]
		string StringFromDate (NSDate fromDate, NSDate toDate);

		[MacCatalyst (13, 1)]
		[Export ("stringFromDateInterval:")]
		[return: NullAllowed]
		string ToString (NSDateInterval dateInterval);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter))]
	interface NSEnergyFormatter {
		[Export ("numberFormatter", ArgumentSemantic.Copy)]
		NSNumberFormatter NumberFormatter { get; set; }

		[Export ("unitStyle")]
		NSFormattingUnitStyle UnitStyle { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("forFoodEnergyUse")]
		bool ForFoodEnergyUse { [Bind ("isForFoodEnergyUse")] get; set; }

		[Export ("stringFromValue:unit:")]
		string StringFromValue (double value, NSEnergyFormatterUnit unit);

		[Export ("stringFromJoules:")]
		string StringFromJoules (double numberInJoules);

		[Export ("unitStringFromValue:unit:")]
		string UnitStringFromValue (double value, NSEnergyFormatterUnit unit);

		[Export ("unitStringFromJoules:usedUnit:")]
		string UnitStringFromJoules (double numberInJoules, out NSEnergyFormatterUnit unitp);

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject obj, string str, out string error);
	}

	interface NSFileHandleReadEventArgs {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("NSFileHandleNotificationDataItem")]
		NSData AvailableData { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("NSFileHandleError", ArgumentSemantic.Assign)]
		nint UnixErrorCode { get; }
	}

	interface NSFileHandleConnectionAcceptedEventArgs {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("NSFileHandleNotificationFileHandleItem")]
		NSFileHandle NearSocketConnection { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("NSFileHandleError", ArgumentSemantic.Assign)]
		nint UnixErrorCode { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // return invalid handle
	interface NSFileHandle : NSSecureCoding {
		[Export ("availableData")]
		NSData AvailableData ();

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'ReadToEnd (out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'ReadToEnd (out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'ReadToEnd (out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ReadToEnd (out NSError)' instead.")]
		[Export ("readDataToEndOfFile")]
		NSData ReadDataToEndOfFile ();

		[MacCatalyst (13, 1)]
		[Export ("readDataToEndOfFileAndReturnError:")]
		[return: NullAllowed]
		NSData ReadToEnd ([NullAllowed] out NSError error);

		/// <param name="length">To be added.</param>
		/// <summary>Reads a block of data of the specified length from the file represented by this NSFileHandle.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Read (nuint, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Read (nuint, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Read (nuint, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Read (nuint, out NSError)' instead.")]
		[Export ("readDataOfLength:")]
		NSData ReadDataOfLength (nuint length);

		[MacCatalyst (13, 1)]
		[Export ("readDataUpToLength:error:")]
		[return: NullAllowed]
		NSData Read (nuint length, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Write (out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Write (out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Write (out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Write (out NSError)' instead.")]
		[Export ("writeData:")]
		void WriteData (NSData data);

		[MacCatalyst (13, 1)]
		[Export ("writeData:error:")]
		bool Write (NSData data, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GetOffset (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetOffset (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GetOffset (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetOffset (out ulong, out NSError)' instead.")]
		[Export ("offsetInFile")]
		ulong OffsetInFile ();

		[MacCatalyst (13, 1)]
		[Export ("getOffset:error:")]
		bool GetOffset (out ulong offsetInFile, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'SeekToEnd (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'SeekToEnd (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'SeekToEnd (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SeekToEnd (out ulong, out NSError)' instead.")]
		[Export ("seekToEndOfFile")]
		ulong SeekToEndOfFile ();

		[MacCatalyst (13, 1)]
		[Export ("seekToEndReturningOffset:error:")]
		bool SeekToEnd ([NullAllowed] out ulong offsetInFile, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Seek (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Seek (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Seek (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Seek (ulong, out NSError)' instead.")]
		[Export ("seekToFileOffset:")]
		void SeekToFileOffset (ulong offset);

		[MacCatalyst (13, 1)]
		[Export ("seekToOffset:error:")]
		bool Seek (ulong offset, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Truncate (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Truncate (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Truncate (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Truncate (ulong, out NSError)' instead.")]
		[Export ("truncateFileAtOffset:")]
		void TruncateFileAtOffset (ulong offset);

		[MacCatalyst (13, 1)]
		[Export ("truncateAtOffset:error:")]
		bool Truncate (ulong offset, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Synchronize (out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Synchronize (out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Synchronize (out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Synchronize (out NSError)' instead.")]
		[Export ("synchronizeFile")]
		void SynchronizeFile ();

		[MacCatalyst (13, 1)]
		[Export ("synchronizeAndReturnError:")]
		bool Synchronize ([NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Close (out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Close (out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Close (out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Close (out NSError)' instead.")]
		[Export ("closeFile")]
		void CloseFile ();

		[MacCatalyst (13, 1)]
		[Export ("closeAndReturnError:")]
		bool Close ([NullAllowed] out NSError error);

		[Static]
		[Export ("fileHandleWithStandardInput")]
		NSFileHandle FromStandardInput ();

		[Static]
		[Export ("fileHandleWithStandardOutput")]
		NSFileHandle FromStandardOutput ();

		[Static]
		[Export ("fileHandleWithStandardError")]
		NSFileHandle FromStandardError ();

		[Static]
		[Export ("fileHandleWithNullDevice")]
		NSFileHandle FromNullDevice ();

		[Static]
		[Export ("fileHandleForReadingAtPath:")]
		NSFileHandle OpenRead (string path);

		[Static]
		[Export ("fileHandleForWritingAtPath:")]
		NSFileHandle OpenWrite (string path);

		[Static]
		[Export ("fileHandleForUpdatingAtPath:")]
		NSFileHandle OpenUpdate (string path);

		[Static]
		[Export ("fileHandleForReadingFromURL:error:")]
		NSFileHandle OpenReadUrl (NSUrl url, out NSError error);

		[Static]
		[Export ("fileHandleForWritingToURL:error:")]
		NSFileHandle OpenWriteUrl (NSUrl url, out NSError error);

		[Static]
		[Export ("fileHandleForUpdatingURL:error:")]
		NSFileHandle OpenUpdateUrl (NSUrl url, out NSError error);

		[Export ("readInBackgroundAndNotifyForModes:")]
		void ReadInBackground (NSString [] notifyRunLoopModes);

		/// <param name="notifyRunLoopModes">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("ReadInBackground (notifyRunLoopModes.GetConstants ())")]
		void ReadInBackground (NSRunLoopMode [] notifyRunLoopModes);

		[Export ("readInBackgroundAndNotify")]
		void ReadInBackground ();

		[Export ("readToEndOfFileInBackgroundAndNotifyForModes:")]
		void ReadToEndOfFileInBackground (NSString [] notifyRunLoopModes);

		/// <param name="notifyRunLoopModes">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("ReadToEndOfFileInBackground (notifyRunLoopModes.GetConstants ())")]
		void ReadToEndOfFileInBackground (NSRunLoopMode [] notifyRunLoopModes);

		[Export ("readToEndOfFileInBackgroundAndNotify")]
		void ReadToEndOfFileInBackground ();

		[Export ("acceptConnectionInBackgroundAndNotifyForModes:")]
		void AcceptConnectionInBackground (NSString [] notifyRunLoopModes);

		/// <param name="notifyRunLoopModes">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("AcceptConnectionInBackground (notifyRunLoopModes.GetConstants ())")]
		void AcceptConnectionInBackground (NSRunLoopMode [] notifyRunLoopModes);

		[Export ("acceptConnectionInBackgroundAndNotify")]
		void AcceptConnectionInBackground ();

		[Export ("waitForDataInBackgroundAndNotifyForModes:")]
		void WaitForDataInBackground (NSString [] notifyRunLoopModes);

		/// <param name="notifyRunLoopModes">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("WaitForDataInBackground (notifyRunLoopModes.GetConstants ())")]
		void WaitForDataInBackground (NSRunLoopMode [] notifyRunLoopModes);

		[Export ("waitForDataInBackgroundAndNotify")]
		void WaitForDataInBackground ();

		[DesignatedInitializer]
		[Export ("initWithFileDescriptor:closeOnDealloc:")]
		NativeHandle Constructor (int /* int, not NSInteger */ fd, bool closeOnDealloc);

		[Export ("initWithFileDescriptor:")]
		NativeHandle Constructor (int /* int, not NSInteger */ fd);

		[Export ("fileDescriptor")]
		int FileDescriptor { get; } /* int, not NSInteger */

		/// <param name="readCallback">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("setReadabilityHandler:")]
		void SetReadabilityHandler ([NullAllowed] Action<NSFileHandle> readCallback);

		/// <param name="writeCallback">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("setWriteabilityHandler:")]
		void SetWriteabilityHandle ([NullAllowed] Action<NSFileHandle> writeCallback);

		/// <summary>Represents the value associated with the constant NSFileHandleOperationException</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileHandleOperationException")]
		NSString OperationException { get; }

		[Field ("NSFileHandleReadCompletionNotification")]
		[Notification (typeof (NSFileHandleReadEventArgs))]
		NSString ReadCompletionNotification { get; }

		[Field ("NSFileHandleReadToEndOfFileCompletionNotification")]
		[Notification (typeof (NSFileHandleReadEventArgs))]
		NSString ReadToEndOfFileCompletionNotification { get; }

		[Field ("NSFileHandleConnectionAcceptedNotification")]
		[Notification (typeof (NSFileHandleConnectionAcceptedEventArgs))]
		NSString ConnectionAcceptedNotification { get; }

		[Field ("NSFileHandleDataAvailableNotification")]
		[Notification]
		NSString DataAvailableNotification { get; }
	}

	/// <summary>Represents the components of a person name.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Static]
	interface NSPersonNameComponent {
		/// <summary>Gets the key for the name component.</summary>
		///         <value>The key for the name component.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSPersonNameComponentKey")]
		NSString ComponentKey { get; }

		/// <summary>Gets the person's given name.</summary>
		///         <value>The person's given name.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSPersonNameComponentGivenName")]
		NSString GivenName { get; }

		/// <summary>Gets the person's family name.</summary>
		///         <value>The person's family name.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSPersonNameComponentFamilyName")]
		NSString FamilyName { get; }

		/// <summary>Gets the person's middle name.</summary>
		///         <value>The person's middle name.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSPersonNameComponentMiddleName")]
		NSString MiddleName { get; }

		/// <summary>Gets the prefix to the person's name.</summary>
		///         <value>The prefix to the person's name.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSPersonNameComponentPrefix")]
		NSString Prefix { get; }

		/// <summary>Gets the suffix to the person's name.</summary>
		///         <value>The suffix to the person's name.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSPersonNameComponentSuffix")]
		NSString Suffix { get; }

		/// <summary>Gets the person's nickname.</summary>
		///         <value>The person's nickname.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSPersonNameComponentNickname")]
		NSString Nickname { get; }

		/// <summary>Gets the delimiter.</summary>
		///         <value>The delimiter.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSPersonNameComponentDelimiter")]
		NSString Delimiter { get; }
	}


	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSPersonNameComponents : NSCopying, NSSecureCoding {

		[NullAllowed, Export ("namePrefix")]
		string NamePrefix { get; set; }

		[NullAllowed, Export ("givenName")]
		string GivenName { get; set; }

		[NullAllowed, Export ("middleName")]
		string MiddleName { get; set; }

		[NullAllowed, Export ("familyName")]
		string FamilyName { get; set; }

		[NullAllowed, Export ("nameSuffix")]
		string NameSuffix { get; set; }

		[NullAllowed, Export ("nickname")]
		string Nickname { get; set; }

		[NullAllowed, Export ("phoneticRepresentation", ArgumentSemantic.Copy)]
		NSPersonNameComponents PhoneticRepresentation { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter))]
	interface NSPersonNameComponentsFormatter {
		[Export ("style", ArgumentSemantic.Assign)]
		NSPersonNameComponentsFormatterStyle Style { get; set; }

		/// <summary>Gets or sets a value that controls whether the receiver should  use only phonetic name component representations. Default is <see langword="false" />.</summary>
		///         <value>A value that controls whether the receiver should  use only phonetic name component representations.</value>
		///         <remarks>Default is <see langword="false" />.</remarks>
		[Export ("phonetic")]
		bool Phonetic { [Bind ("isPhonetic")] get; set; }

		[Static]
		[Export ("localizedStringFromPersonNameComponents:style:options:")]
		string GetLocalizedString (NSPersonNameComponents components, NSPersonNameComponentsFormatterStyle nameFormatStyle, NSPersonNameComponentsFormatterOptions nameOptions);

		[Export ("stringFromPersonNameComponents:")]
		string GetString (NSPersonNameComponents components);

		[Export ("annotatedStringFromPersonNameComponents:")]
		NSAttributedString GetAnnotatedString (NSPersonNameComponents components);

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject result, string str, out string errorDescription);

		[MacCatalyst (13, 1)]
		[Export ("personNameComponentsFromString:")]
		[return: NullAllowed]
		NSPersonNameComponents GetComponents (string @string);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }
	}


	[BaseType (typeof (NSObject))]
	interface NSPipe {

		[Export ("fileHandleForReading")]
		NSFileHandle ReadHandle { get; }

		[Export ("fileHandleForWriting")]
		NSFileHandle WriteHandle { get; }

		[Static]
		[Export ("pipe")]
		NSPipe Create ();
	}

	[BaseType (typeof (NSObject))]
	interface NSFormatter : NSCoding, NSCopying {
		[Export ("stringForObjectValue:")]
		string StringFor ([NullAllowed] NSObject value);

		// - (NSAttributedString *)attributedStringForObjectValue:(id)obj withDefaultAttributes:(NSDictionary *)attrs;

		[Export ("editingStringForObjectValue:")]
		string EditingStringFor (NSObject value);

		[Internal]
		[Sealed]
		[Export ("attributedStringForObjectValue:withDefaultAttributes:")]
		NSAttributedString GetAttributedString (NSObject obj, NSDictionary defaultAttributes);

		// -(NSAttributedString *)attributedStringForObjectValue:(id)obj withDefaultAttributes:(NSDictionary *)attrs;
		[Export ("attributedStringForObjectValue:withDefaultAttributes:")]
		NSAttributedString GetAttributedString (NSObject obj, NSDictionary<NSString, NSObject> defaultAttributes);

		/// <param name="obj">To be added.</param>
		///         <param name="defaultAttributes">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Wrap ("GetAttributedString (obj, defaultAttributes.GetDictionary ()!)")]
#if MONOMAC
		NSAttributedString GetAttributedString (NSObject obj, NSStringAttributes defaultAttributes);
#else
		NSAttributedString GetAttributedString (NSObject obj, UIStringAttributes defaultAttributes);
#endif

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject obj, string str, out NSString error);

		[Export ("isPartialStringValid:newEditingString:errorDescription:")]
		bool IsPartialStringValid (string partialString, [NullAllowed] out string newString, [NullAllowed] out NSString error);

		[Export ("isPartialStringValid:proposedSelectedRange:originalString:originalSelectedRange:errorDescription:")]
		bool IsPartialStringValid (ref string partialString, out NSRange proposedSelRange, string origString, NSRange origSelRange, [NullAllowed] out string error);
	}

#if !XAMCORE_5_0
	[BaseType (typeof (NSObject))]
	[Model]
#endif
	[Protocol]
	interface NSCoding {
		// [Abstract]
		[Export ("initWithCoder:")]
		NativeHandle Constructor (NSCoder decoder);

		/// <summary>Encodes the state of the object using the provided encoder.</summary>
		/// <param name="encoder">The encoder object where the state of the object will be stored</param>
		/// <remarks>
		///   <para>This method is part of the <see cref="INSCoding" /> protocol and is used by applications to preserve the state of the object into an archive.</para>
		///   <para>Developers will typically create an <see cref="NSKeyedArchiver" /> and then invoke the <see cref="NSKeyedArchiver.ArchiveRootObjectToFile(Foundation.NSObject,System.String)" /> method which will call into this method.</para>
		///   <para>If developers want to allow their object to be archived, they should override this method and store their state in using the provided <paramref name="encoder" /> parameter. In addition, developers should also implement a constructor that takes an NSCoder argument and is exported with <c>[Export ("initWithCoder:")]</c>.</para>
		///   <example>
		///   <code lang="csharp lang-csharp"><![CDATA[public void override EncodeTo (NSCoder coder) {
		/// coder.Encode (1, key: "version");
		/// coder.Encode (userName, key: "userName");
		/// coder.Encode (hostName, key: "hostName");]]></code>
		/// </example>
		/// </remarks>
		[Abstract]
		[Export ("encodeWithCoder:")]
		void EncodeTo (NSCoder encoder);
	}

	interface INSCoding { }

	/// <summary>The secure coding category.</summary>
	/// <remarks>To be added.</remarks>
	[Protocol]
	interface NSSecureCoding : NSCoding {
		// note: +supportsSecureCoding being static it is not a good "generated" binding candidate
	}

#if !XAMCORE_5_0
	[BaseType (typeof (NSObject))]
	[Model]
#endif
	[Protocol]
	interface NSCopying {
		/// <param name="zone">Developers should pass <see langword="null" />.  Memory zones are no longer used.</param>
		/// <summary>Performs a copy of the underlying Objective-C object.</summary>
		/// <returns>The newly-allocated object.</returns>
		/// <remarks>
		///           <para>This method performs a "shallow copy" of <see langword="this" />. If this object contains references to external objects, the new object will contain references to the same object.</para>
		///         </remarks>
		[Abstract]
		[return: Release]
		[Export ("copyWithZone:")]
		NSObject Copy ([NullAllowed] NSZone zone);
	}

#if !XAMCORE_5_0
	[BaseType (typeof (NSObject))]
	[Model]
#endif
	[Protocol]
	interface NSMutableCopying : NSCopying {
		/// <param name="zone">Zone to use to allocate this object, or null to use the default zone.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("mutableCopyWithZone:")]
		[return: Release ()]
		NSObject MutableCopy ([NullAllowed] NSZone zone);
	}

	interface INSMutableCopying { }

	interface INSKeyedArchiverDelegate { }

	/// <summary>Methods that can be invoked by the NSKeyedArchiver during serialization.</summary>
	/// <remarks>To be added.</remarks>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Cocoa/Reference/NSKeyedArchiverDelegate_Protocol/index.html">Apple documentation for <c>NSKeyedArchiverDelegate</c></related>
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSKeyedArchiverDelegate {
		/// <param name="archiver">To be added.</param>
		/// <param name="obj">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("archiver:didEncodeObject:"), EventArgs ("NSObject", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void EncodedObject (NSKeyedArchiver archiver, NSObject obj);

		/// <param name="archiver">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("archiverDidFinish:")]
		void Finished (NSKeyedArchiver archiver);

		/// <param name="archiver">To be added.</param>
		/// <param name="obj">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Delegate invoked by the object to get a value.</summary>
			<value>To be added.</value>
			<remarks>Developers assign a function, delegate or anonymous method to this property to return a value to the object.   If developers assign a value to this property, it this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("archiver:willEncodeObject:"), DelegateName ("NSEncodeHook"), DefaultValue (null)]
		NSObject WillEncode (NSKeyedArchiver archiver, NSObject obj);

		/// <param name="archiver">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("archiverWillFinish:")]
		void Finishing (NSKeyedArchiver archiver);

		/// <param name="archiver">To be added.</param>
		/// <param name="oldObject">To be added.</param>
		/// <param name="newObject">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("archiver:willReplaceObject:withObject:"), EventArgs ("NSArchiveReplace", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void ReplacingObject (NSKeyedArchiver archiver, NSObject oldObject, NSObject newObject);
	}

	interface INSKeyedUnarchiverDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSKeyedUnarchiverDelegate {
		/// <param name="unarchiver">To be added.</param>
		/// <param name="obj">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Delegate invoked by the object to get a value.</summary>
			<value>To be added.</value>
			<remarks>Developers assign a function, delegate or anonymous method to this property to return a value to the object.   If developers assign a value to this property, it this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("unarchiver:didDecodeObject:"), DelegateName ("NSDecoderCallback"), DefaultValue (null)]
		NSObject DecodedObject (NSKeyedUnarchiver unarchiver, NSObject obj);

		/// <param name="unarchiver">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("unarchiverDidFinish:")]
		void Finished (NSKeyedUnarchiver unarchiver);

		/// <param name="unarchiver">To be added.</param>
		/// <param name="klass">To be added.</param>
		/// <param name="classes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Delegate invoked by the object to get a value.</summary>
			<value>To be added.</value>
			<remarks>Developers assign a function, delegate or anonymous method to this property to return a value to the object.   If developers assign a value to this property, it this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("unarchiver:cannotDecodeObjectOfClassName:originalClasses:"), DelegateName ("NSDecoderHandler"), DefaultValue (null)]
		Class CannotDecodeClass (NSKeyedUnarchiver unarchiver, string klass, string [] classes);

		/// <param name="unarchiver">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("unarchiverWillFinish:")]
		void Finishing (NSKeyedUnarchiver unarchiver);

		/// <param name="unarchiver">To be added.</param>
		/// <param name="oldObject">To be added.</param>
		/// <param name="newObject">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("unarchiver:willReplaceObject:withObject:"), EventArgs ("NSArchiveReplace", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void ReplacingObject (NSKeyedUnarchiver unarchiver, NSObject oldObject, NSObject newObject);
	}

	[BaseType (typeof (NSCoder),
		   Delegates = new string [] { "WeakDelegate" },
		   Events = new Type [] { typeof (NSKeyedArchiverDelegate) })]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[NSKeyedArchiver init]: cannot use -init for initialization
	[DisableDefaultCtor]
	interface NSKeyedArchiver {

		[MacCatalyst (13, 1)]
		[Export ("initRequiringSecureCoding:")]
		NativeHandle Constructor (bool requiresSecureCoding);

		// hack so we can decorate the default .ctor with availability attributes
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Export ("initForWritingWithMutableData:")]
		NativeHandle Constructor (NSMutableData data);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("archivedDataWithRootObject:requiringSecureCoding:error:")]
		[return: NullAllowed]
		NSData GetArchivedData (NSObject @object, bool requiresSecureCoding, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetArchivedData (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetArchivedData (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GetArchivedData (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetArchivedData (NSObject, bool, out NSError)' instead.")]
		[Export ("archivedDataWithRootObject:")]
		[Static]
		NSData GetArchivedData (NSObject root);

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetArchivedData (NSObject, bool, out NSError)' and then 'NSData.Save' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetArchivedData (NSObject, bool, out NSError)' and then 'NSData.Save' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GetArchivedData (NSObject, bool, out NSError)' and then 'NSData.Save' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetArchivedData (NSObject, bool, out NSError)' and then 'NSData.Save' instead.")]
		[Export ("archiveRootObject:toFile:")]
		[Static]
		bool ArchiveRootObjectToFile (NSObject root, string file);

		[Export ("finishEncoding")]
		void FinishEncoding ();

		[Export ("outputFormat")]
		NSPropertyListFormat PropertyListFormat { get; set; }

		/// <summary>An instance of the Foundation.INSKeyedArchiverDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSKeyedArchiverDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		INSKeyedArchiverDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("setClassName:forClass:")]
		void SetClassName (string name, Class kls);

		[Export ("classNameForClass:")]
		string GetClassName (Class kls);

		/// <summary>Represents the value associated with the constant NSKeyedArchiveRootObjectKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSKeyedArchiveRootObjectKey")]
		NSString RootObjectKey { get; }

		[Export ("requiresSecureCoding")]
		bool RequiresSecureCoding { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("encodedData", ArgumentSemantic.Strong)]
		NSData EncodedData { get; }
	}

	[BaseType (typeof (NSCoder),
		   Delegates = new string [] { "WeakDelegate" },
		   Events = new Type [] { typeof (NSKeyedUnarchiverDelegate) })]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[NSKeyedUnarchiver init]: cannot use -init for initialization
	[DisableDefaultCtor]
	interface NSKeyedUnarchiver {
		[MacCatalyst (13, 1)]
		[Export ("initForReadingFromData:error:")]
		NativeHandle Constructor (NSData data, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unarchivedObjectOfClass:fromData:error:")]
		[return: NullAllowed]
		NSObject GetUnarchivedObject (Class cls, NSData data, [NullAllowed] out NSError error);

		/// <param name="type">To be added.</param>
		///         <param name="data">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetUnarchivedObject (new Class (type), data, out error)")]
		[return: NullAllowed]
		NSObject GetUnarchivedObject (Type type, NSData data, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("unarchivedObjectOfClasses:fromData:error:")]
		[return: NullAllowed]
		NSObject GetUnarchivedObject (NSSet<Class> classes, NSData data, [NullAllowed] out NSError error);

		/// <param name="types">To be added.</param>
		///         <param name="data">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetUnarchivedObject (new NSSet<Class> (Array.ConvertAll (types, t => new Class (t))), data, out error)")]
		[return: NullAllowed]
		NSObject GetUnarchivedObject (Type [] types, NSData data, [NullAllowed] out NSError error);

		[Export ("initForReadingWithData:")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[MarshalNativeExceptions]
		NativeHandle Constructor (NSData data);

		[Static, Export ("unarchiveObjectWithData:")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetUnarchivedObject ()' instead.")]
		[MarshalNativeExceptions]
		NSObject UnarchiveObject (NSData data);

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Static, Export ("unarchiveTopLevelObjectWithData:error:")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetUnarchivedObject ()' instead.")]
		// FIXME: [MarshalNativeExceptions]
		NSObject UnarchiveTopLevelObject (NSData data, out NSError error);

		[Static, Export ("unarchiveObjectWithFile:")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetUnarchivedObject ()' instead.")]
		[MarshalNativeExceptions]
		NSObject UnarchiveFile (string file);

		[Export ("finishDecoding")]
		void FinishDecoding ();

		/// <summary>An instance of the Foundation.INSKeyedUnarchiverDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSKeyedUnarchiverDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		INSKeyedUnarchiverDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("setClass:forClassName:")]
		void SetClass (Class kls, string codedName);

		[Export ("classForClassName:")]
		[return: NullAllowed]
		Class GetClass (string codedName);

		[Export ("requiresSecureCoding")]
		bool RequiresSecureCoding { get; set; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("unarchivedArrayOfObjectsOfClass:fromData:error:")]
		[return: NullAllowed]
		NSObject [] GetUnarchivedArray (Class @class, NSData data, [NullAllowed] out NSError error);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("unarchivedArrayOfObjectsOfClasses:fromData:error:")]
		[return: NullAllowed]
		NSObject [] GetUnarchivedArray (NSSet<Class> classes, NSData data, [NullAllowed] out NSError error);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("unarchivedDictionaryWithKeysOfClass:objectsOfClass:fromData:error:")]
		[return: NullAllowed]
		NSDictionary GetUnarchivedDictionary (Class keyClass, Class valueClass, NSData data, [NullAllowed] out NSError error);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("unarchivedDictionaryWithKeysOfClasses:objectsOfClasses:fromData:error:")]
		[return: NullAllowed]
		NSDictionary GetUnarchivedDictionary (NSSet<Class> keyClasses, NSSet<Class> valueClasses, NSData data, [NullAllowed] out NSError error);
	}

	[BaseType (typeof (NSObject), Delegates = new string [] { "Delegate" }, Events = new Type [] { typeof (NSMetadataQueryDelegate) })]
	interface NSMetadataQuery {
		[Export ("startQuery")]
		bool StartQuery ();

		[Export ("stopQuery")]
		void StopQuery ();

		/// <summary>Whether this query has begun.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isStarted")]
		bool IsStarted { get; }

		/// <summary>Whether this query is in the initial gathering phase of the query.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isGathering")]
		bool IsGathering { get; }

		/// <summary>Whether this query has stopped.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isStopped")]
		bool IsStopped { get; }

		[Export ("disableUpdates")]
		void DisableUpdates ();

		[Export ("enableUpdates")]
		void EnableUpdates ();

		[Export ("resultCount")]
		nint ResultCount { get; }

		/// <param name="idx">To be added.</param>
		/// <summary>The result at the specified index.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("resultAtIndex:")]
		NSObject ResultAtIndex (nint idx);

		[Export ("results")]
		NSMetadataItem [] Results { get; }

		[Export ("indexOfResult:")]
		nint IndexOfResult (NSObject result);

		[Export ("valueLists")]
		NSDictionary ValueLists { get; }

		[Export ("groupedResults")]
		NSObject [] GroupedResults { get; }

		/// <param name="attribyteName">To be added.</param>
		/// <param name="atIndex">To be added.</param>
		/// <summary>The value of the specified attributeName in the result at the specified index in the Results array.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("valueOfAttribute:forResultAtIndex:")]
		NSObject ValueOfAttribute (string attribyteName, nint atIndex);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the Foundation.INSMetadataQueryDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSMetadataQueryDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		INSMetadataQueryDelegate Delegate { get; set; }

		[Export ("predicate", ArgumentSemantic.Copy)]
		[NullAllowed] // by default this property is null
		NSPredicate Predicate { get; set; }

		[Export ("sortDescriptors", ArgumentSemantic.Copy)]
		NSSortDescriptor [] SortDescriptors { get; set; }

		[Export ("valueListAttributes", ArgumentSemantic.Copy)]
		NSObject [] ValueListAttributes { get; set; }

		[Export ("groupingAttributes", ArgumentSemantic.Copy)]
		NSArray GroupingAttributes { get; set; }

		[Export ("notificationBatchingInterval")]
		double NotificationBatchingInterval { get; set; }

		[Export ("searchScopes", ArgumentSemantic.Copy)]
		NSObject [] SearchScopes { get; set; }

		// There is no info associated with these notifications
		[Field ("NSMetadataQueryDidStartGatheringNotification")]
		[Notification]
		NSString DidStartGatheringNotification { get; }

		[Field ("NSMetadataQueryGatheringProgressNotification")]
		[Notification]
		NSString GatheringProgressNotification { get; }

		[Field ("NSMetadataQueryDidFinishGatheringNotification")]
		[Notification]
		NSString DidFinishGatheringNotification { get; }

		[Field ("NSMetadataQueryDidUpdateNotification")]
		[Notification]
		NSString DidUpdateNotification { get; }

		/// <summary>Represents the value associated with the constant NSMetadataQueryResultContentRelevanceAttribute</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataQueryResultContentRelevanceAttribute")]
		NSString ResultContentRelevanceAttribute { get; }

		// Scope constants for defined search locations
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Field ("NSMetadataQueryUserHomeScope")]
		NSString UserHomeScope { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Field ("NSMetadataQueryLocalComputerScope")]
		NSString LocalComputerScope { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Field ("NSMetadataQueryLocalDocumentsScope")]
		NSString LocalDocumentsScope { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Field ("NSMetadataQueryNetworkScope")]
		NSString NetworkScope { get; }

		/// <summary>Represents the value associated with the constant NSMetadataQueryUbiquitousDocumentsScope</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataQueryUbiquitousDocumentsScope")]
		NSString UbiquitousDocumentsScope { get; }

		/// <summary>Represents the value associated with the constant NSMetadataQueryUbiquitousDataScope</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataQueryUbiquitousDataScope")]
		NSString UbiquitousDataScope { get; }


		/// <summary>Represents the value associated with the constant NSMetadataQueryAccessibleUbiquitousExternalDocumentsScope</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataQueryAccessibleUbiquitousExternalDocumentsScope")]
		NSString AccessibleUbiquitousExternalDocumentsScope { get; }

		/// <summary>Represents the value associated with the constant NSMetadataItemFSNameKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataItemFSNameKey")]
		NSString ItemFSNameKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataItemDisplayNameKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataItemDisplayNameKey")]
		NSString ItemDisplayNameKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataItemURLKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataItemURLKey")]
		NSString ItemURLKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataItemPathKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataItemPathKey")]
		NSString ItemPathKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataItemFSSizeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataItemFSSizeKey")]
		NSString ItemFSSizeKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataItemFSCreationDateKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataItemFSCreationDateKey")]
		NSString ItemFSCreationDateKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataItemFSContentChangeDateKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataItemFSContentChangeDateKey")]
		NSString ItemFSContentChangeDateKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataItemContentTypeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataItemContentTypeKey")]
		NSString ContentTypeKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataItemContentTypeTreeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataItemContentTypeTreeKey")]
		NSString ContentTypeTreeKey { get; }


		/// <summary>Represents the value associated with the constant NSMetadataItemIsUbiquitousKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataItemIsUbiquitousKey")]
		NSString ItemIsUbiquitousKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemHasUnresolvedConflictsKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataUbiquitousItemHasUnresolvedConflictsKey")]
		NSString UbiquitousItemHasUnresolvedConflictsKey { get; }

		/// <summary>Developers should not use this deprecated property. Developers should use 'UbiquitousItemDownloadingStatusKey' instead.</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'UbiquitousItemDownloadingStatusKey' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'UbiquitousItemDownloadingStatusKey' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'UbiquitousItemDownloadingStatusKey' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UbiquitousItemDownloadingStatusKey' instead.")]
		[Field ("NSMetadataUbiquitousItemIsDownloadedKey")]
		NSString UbiquitousItemIsDownloadedKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemIsDownloadingKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataUbiquitousItemIsDownloadingKey")]
		NSString UbiquitousItemIsDownloadingKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemIsUploadedKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataUbiquitousItemIsUploadedKey")]
		NSString UbiquitousItemIsUploadedKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemIsUploadingKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataUbiquitousItemIsUploadingKey")]
		NSString UbiquitousItemIsUploadingKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemDownloadingStatusKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemDownloadingStatusKey")]
		NSString UbiquitousItemDownloadingStatusKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemDownloadingErrorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemDownloadingErrorKey")]
		NSString UbiquitousItemDownloadingErrorKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemUploadingErrorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemUploadingErrorKey")]
		NSString UbiquitousItemUploadingErrorKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemPercentDownloadedKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataUbiquitousItemPercentDownloadedKey")]
		NSString UbiquitousItemPercentDownloadedKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemPercentUploadedKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMetadataUbiquitousItemPercentUploadedKey")]
		NSString UbiquitousItemPercentUploadedKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemDownloadRequestedKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemDownloadRequestedKey")]
		NSString UbiquitousItemDownloadRequestedKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemIsExternalDocumentKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemIsExternalDocumentKey")]
		NSString UbiquitousItemIsExternalDocumentKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemContainerDisplayNameKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemContainerDisplayNameKey")]
		NSString UbiquitousItemContainerDisplayNameKey { get; }

		/// <summary>Represents the value associated with the constant NSMetadataUbiquitousItemURLInLocalContainerKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemURLInLocalContainerKey")]
		NSString UbiquitousItemURLInLocalContainerKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemKeywordsKey")]
		NSString KeywordsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTitleKey")]
		NSString TitleKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAuthorsKey")]
		NSString AuthorsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemEditorsKey")]
		NSString EditorsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemParticipantsKey")]
		NSString ParticipantsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemProjectsKey")]
		NSString ProjectsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDownloadedDateKey")]
		NSString DownloadedDateKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemWhereFromsKey")]
		NSString WhereFromsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCommentKey")]
		NSString CommentKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCopyrightKey")]
		NSString CopyrightKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLastUsedDateKey")]
		NSString LastUsedDateKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemContentCreationDateKey")]
		NSString ContentCreationDateKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemContentModificationDateKey")]
		NSString ContentModificationDateKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDateAddedKey")]
		NSString DateAddedKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDurationSecondsKey")]
		NSString DurationSecondsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemContactKeywordsKey")]
		NSString ContactKeywordsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemVersionKey")]
		NSString VersionKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPixelHeightKey")]
		NSString PixelHeightKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPixelWidthKey")]
		NSString PixelWidthKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPixelCountKey")]
		NSString PixelCountKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemColorSpaceKey")]
		NSString ColorSpaceKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemBitsPerSampleKey")]
		NSString BitsPerSampleKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFlashOnOffKey")]
		NSString FlashOnOffKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFocalLengthKey")]
		NSString FocalLengthKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAcquisitionMakeKey")]
		NSString AcquisitionMakeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAcquisitionModelKey")]
		NSString AcquisitionModelKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemISOSpeedKey")]
		NSString IsoSpeedKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemOrientationKey")]
		NSString OrientationKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLayerNamesKey")]
		NSString LayerNamesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemWhiteBalanceKey")]
		NSString WhiteBalanceKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemApertureKey")]
		NSString ApertureKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemProfileNameKey")]
		NSString ProfileNameKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemResolutionWidthDPIKey")]
		NSString ResolutionWidthDpiKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemResolutionHeightDPIKey")]
		NSString ResolutionHeightDpiKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExposureModeKey")]
		NSString ExposureModeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExposureTimeSecondsKey")]
		NSString ExposureTimeSecondsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemEXIFVersionKey")]
		NSString ExifVersionKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCameraOwnerKey")]
		NSString CameraOwnerKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFocalLength35mmKey")]
		NSString FocalLength35mmKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLensModelKey")]
		NSString LensModelKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemEXIFGPSVersionKey")]
		NSString ExifGpsVersionKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAltitudeKey")]
		NSString AltitudeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLatitudeKey")]
		NSString LatitudeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLongitudeKey")]
		NSString LongitudeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemSpeedKey")]
		NSString SpeedKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTimestampKey")]
		NSString TimestampKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSTrackKey")]
		NSString GpsTrackKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemImageDirectionKey")]
		NSString ImageDirectionKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemNamedLocationKey")]
		NSString NamedLocationKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSStatusKey")]
		NSString GpsStatusKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSMeasureModeKey")]
		NSString GpsMeasureModeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDOPKey")]
		NSString GpsDopKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSMapDatumKey")]
		NSString GpsMapDatumKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDestLatitudeKey")]
		NSString GpsDestLatitudeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDestLongitudeKey")]
		NSString GpsDestLongitudeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDestBearingKey")]
		NSString GpsDestBearingKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDestDistanceKey")]
		NSString GpsDestDistanceKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSProcessingMethodKey")]
		NSString GpsProcessingMethodKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSAreaInformationKey")]
		NSString GpsAreaInformationKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDateStampKey")]
		NSString GpsDateStampKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDifferentalKey")]
		NSString GpsDifferentialKey { get; }

#if !XAMCORE_5_0
		[Obsolete ("Use 'GpsDifferentialKey' instead.")]
		[NoTV, NoiOS, NoMacCatalyst]
		[Wrap ("GpsDifferentialKey")]
		NSString GpsDifferentalKey { get; }
#endif

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCodecsKey")]
		NSString CodecsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMediaTypesKey")]
		NSString MediaTypesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemStreamableKey")]
		NSString StreamableKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTotalBitRateKey")]
		NSString TotalBitRateKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemVideoBitRateKey")]
		NSString VideoBitRateKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudioBitRateKey")]
		NSString AudioBitRateKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDeliveryTypeKey")]
		NSString DeliveryTypeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAlbumKey")]
		NSString AlbumKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemHasAlphaChannelKey")]
		NSString HasAlphaChannelKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRedEyeOnOffKey")]
		NSString RedEyeOnOffKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMeteringModeKey")]
		NSString MeteringModeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMaxApertureKey")]
		NSString MaxApertureKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFNumberKey")]
		NSString FNumberKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExposureProgramKey")]
		NSString ExposureProgramKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExposureTimeStringKey")]
		NSString ExposureTimeStringKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemHeadlineKey")]
		NSString HeadlineKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemInstructionsKey")]
		NSString InstructionsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCityKey")]
		NSString CityKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemStateOrProvinceKey")]
		NSString StateOrProvinceKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCountryKey")]
		NSString CountryKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTextContentKey")]
		NSString TextContentKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudioSampleRateKey")]
		NSString AudioSampleRateKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudioChannelCountKey")]
		NSString AudioChannelCountKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTempoKey")]
		NSString TempoKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemKeySignatureKey")]
		NSString KeySignatureKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTimeSignatureKey")]
		NSString TimeSignatureKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudioEncodingApplicationKey")]
		NSString AudioEncodingApplicationKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemComposerKey")]
		NSString ComposerKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLyricistKey")]
		NSString LyricistKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudioTrackNumberKey")]
		NSString AudioTrackNumberKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRecordingDateKey")]
		NSString RecordingDateKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMusicalGenreKey")]
		NSString MusicalGenreKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemIsGeneralMIDISequenceKey")]
		NSString IsGeneralMidiSequenceKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRecordingYearKey")]
		NSString RecordingYearKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemOrganizationsKey")]
		NSString OrganizationsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLanguagesKey")]
		NSString LanguagesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRightsKey")]
		NSString RightsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPublishersKey")]
		NSString PublishersKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemContributorsKey")]
		NSString ContributorsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCoverageKey")]
		NSString CoverageKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemSubjectKey")]
		NSString SubjectKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemThemeKey")]
		NSString ThemeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDescriptionKey")]
		NSString DescriptionKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemIdentifierKey")]
		NSString IdentifierKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudiencesKey")]
		NSString AudiencesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemNumberOfPagesKey")]
		NSString NumberOfPagesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPageWidthKey")]
		NSString PageWidthKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPageHeightKey")]
		NSString PageHeightKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemSecurityMethodKey")]
		NSString SecurityMethodKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCreatorKey")]
		NSString CreatorKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemEncodingApplicationsKey")]
		NSString EncodingApplicationsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDueDateKey")]
		NSString DueDateKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemStarRatingKey")]
		NSString StarRatingKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPhoneNumbersKey")]
		NSString PhoneNumbersKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemEmailAddressesKey")]
		NSString EmailAddressesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemInstantMessageAddressesKey")]
		NSString InstantMessageAddressesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemKindKey")]
		NSString KindKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRecipientsKey")]
		NSString RecipientsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFinderCommentKey")]
		NSString FinderCommentKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFontsKey")]
		NSString FontsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAppleLoopsRootKeyKey")]
		NSString AppleLoopsRootKeyKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAppleLoopsKeyFilterTypeKey")]
		NSString AppleLoopsKeyFilterTypeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAppleLoopsLoopModeKey")]
		NSString AppleLoopsLoopModeKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAppleLoopDescriptorsKey")]
		NSString AppleLoopDescriptorsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMusicalInstrumentCategoryKey")]
		NSString MusicalInstrumentCategoryKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMusicalInstrumentNameKey")]
		NSString MusicalInstrumentNameKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCFBundleIdentifierKey")]
		NSString CFBundleIdentifierKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemInformationKey")]
		NSString InformationKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDirectorKey")]
		NSString DirectorKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemProducerKey")]
		NSString ProducerKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGenreKey")]
		NSString GenreKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPerformersKey")]
		NSString PerformersKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemOriginalFormatKey")]
		NSString OriginalFormatKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemOriginalSourceKey")]
		NSString OriginalSourceKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAuthorEmailAddressesKey")]
		NSString AuthorEmailAddressesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRecipientEmailAddressesKey")]
		NSString RecipientEmailAddressesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAuthorAddressesKey")]
		NSString AuthorAddressesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRecipientAddressesKey")]
		NSString RecipientAddressesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemIsLikelyJunkKey")]
		NSString IsLikelyJunkKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExecutableArchitecturesKey")]
		NSString ExecutableArchitecturesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExecutablePlatformKey")]
		NSString ExecutablePlatformKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemApplicationCategoriesKey")]
		NSString ApplicationCategoriesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemIsApplicationManagedKey")]
		NSString IsApplicationManagedKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemIsSharedKey")]
		NSString UbiquitousItemIsSharedKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemCurrentUserRoleKey")]
		NSString UbiquitousSharedItemCurrentUserRoleKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemCurrentUserPermissionsKey")]
		NSString UbiquitousSharedItemCurrentUserPermissionsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemOwnerNameComponentsKey")]
		NSString UbiquitousSharedItemOwnerNameComponentsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemMostRecentEditorNameComponentsKey")]
		NSString UbiquitousSharedItemMostRecentEditorNameComponentsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemRoleOwner")]
		NSString UbiquitousSharedItemRoleOwner { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemRoleParticipant")]
		NSString UbiquitousSharedItemRoleParticipant { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemPermissionsReadOnly")]
		NSString UbiquitousSharedItemPermissionsReadOnly { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemPermissionsReadWrite")]
		NSString UbiquitousSharedItemPermissionsReadWrite { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("searchItems", ArgumentSemantic.Copy)]
		// DOC: object is a mixture of NSString, NSMetadataItem, NSUrl
		NSObject [] SearchItems { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("operationQueue", ArgumentSemantic.Retain)]
		NSOperationQueue OperationQueue { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("enumerateResultsUsingBlock:")]
		void EnumerateResultsUsingBlock (NSMetadataQueryEnumerationCallback callback);

		[Export ("enumerateResultsWithOptions:usingBlock:")]
		[MacCatalyst (13, 1)]
		void EnumerateResultsWithOptions (NSEnumerationOptions opts, NSMetadataQueryEnumerationCallback block);

		//
		// These are for NSMetadataQueryDidUpdateNotification 
		//
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataQueryUpdateAddedItemsKey")]
		NSString QueryUpdateAddedItemsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataQueryUpdateChangedItemsKey")]
		NSString QueryUpdateChangedItemsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataQueryUpdateRemovedItemsKey")]
		NSString QueryUpdateRemovedItemsKey { get; }
	}

	interface INSMetadataQueryDelegate { }

	/// <summary>Defines optional methods relating to the lifecycle of <see cref="Foundation.NSMetadataQuery" />s.</summary>
	/// <remarks>To be added.</remarks>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/Cocoa/Reference/NSMetadataQueryDelegate_Protocol/index.html">Apple documentation for <c>NSMetadataQueryDelegate</c></related>
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSMetadataQueryDelegate {
		/// <param name="query">To be added.</param>
		/// <param name="result">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Delegate invoked by the object to get a value.</summary>
			<value>To be added.</value>
			<remarks>Developers assign a function, delegate or anonymous method to this property to return a value to the object.   If developers assign a value to this property, it this will reset the value for the Delegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("metadataQuery:replacementObjectForResultObject:"), DelegateName ("NSMetadataQueryObject"), DefaultValue (null)]
		NSObject ReplacementObjectForResultObject (NSMetadataQuery query, NSMetadataItem result);

		/// <param name="query">To be added.</param>
		/// <param name="attributeName">To be added.</param>
		/// <param name="value">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Delegate invoked by the object to get a value.</summary>
			<value>To be added.</value>
			<remarks>Developers assign a function, delegate or anonymous method to this property to return a value to the object.   If developers assign a value to this property, it this will reset the value for the Delegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("metadataQuery:replacementValueForAttribute:value:"), DelegateName ("NSMetadataQueryValue"), DefaultValue (null)]
#if XAMCORE_5_0
		NSObject ReplacementValueForAttributeValue (NSMetadataQuery query, string attributeName, NSObject value);
#else
		NSObject ReplacementValueForAttributevalue (NSMetadataQuery query, string attributeName, NSObject value);
#endif
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // points to nothing so access properties crash the apps
	interface NSMetadataItem {

		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		[DesignatedInitializer]
		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[Export ("valueForAttribute:")]
		NSObject ValueForAttribute (string key);

		[Sealed]
		[Internal]
		[Export ("valueForAttribute:")]
		IntPtr GetHandle (NSString key);
		[Export ("valuesForAttributes:")]
		NSDictionary ValuesForAttributes (NSArray keys);

		[Export ("attributes")]
		NSObject [] Attributes { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSMetadataQueryAttributeValueTuple {
		[Export ("attribute")]
		string Attribute { get; }

		[Export ("value")]
		NSObject Value { get; }

		[Export ("count")]
		nint Count { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSMetadataQueryResultGroup {
		[Export ("attribute")]
		string Attribute { get; }

		[Export ("value")]
		NSObject Value { get; }

		[Export ("subgroups")]
		NSObject [] Subgroups { get; }

		[Export ("resultCount")]
		nint ResultCount { get; }

		/// <param name="idx">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("resultAtIndex:")]
		NSObject ResultAtIndex (nuint idx);

		[Export ("results")]
		NSObject [] Results { get; }

	}

	// Sadly, while this API is a poor API and we should in general not use it
	// Apple has now surfaced it on a few methods.   So we need to take the Obsolete
	// out, and we will have to fully support it.
	[BaseType (typeof (NSArray))]
	[DesignatedDefaultCtor]
	interface NSMutableArray {
		/// <param name="capacity">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[DesignatedInitializer]
		[Export ("initWithCapacity:")]
		NativeHandle Constructor (nuint capacity);

		[Internal]
		[Sealed]
		[Export ("addObject:")]
		void _Add (IntPtr obj);

		[Export ("addObject:")]
		void Add (NSObject obj);

		[Internal]
		[Sealed]
		[Export ("insertObject:atIndex:")]
		void _Insert (IntPtr obj, nint index);

		/// <param name="obj">To be added.</param>
		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("insertObject:atIndex:")]
		void Insert (NSObject obj, nint index);

		[Export ("removeLastObject")]
		void RemoveLastObject ();

		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("removeObjectAtIndex:")]
		void RemoveObject (nint index);

		[Internal]
		[Sealed]
		[Export ("replaceObjectAtIndex:withObject:")]
		void _ReplaceObject (nint index, IntPtr withObject);

		/// <param name="index">To be added.</param>
		/// <param name="withObject">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("replaceObjectAtIndex:withObject:")]
		void ReplaceObject (nint index, NSObject withObject);

		[Export ("removeAllObjects")]
		void RemoveAllObjects ();

		[Export ("addObjectsFromArray:")]
		void AddObjects (NSObject [] source);

		[Internal]
		[Sealed]
		[Export ("insertObjects:atIndexes:")]
		void _InsertObjects (IntPtr objects, NSIndexSet atIndexes);

		[Export ("insertObjects:atIndexes:")]
		void InsertObjects (NSObject [] objects, NSIndexSet atIndexes);

		[Export ("removeObjectsAtIndexes:")]
		void RemoveObjectsAtIndexes (NSIndexSet indexSet);

		[MacCatalyst (13, 1)]
		[Static, Export ("arrayWithContentsOfFile:")]
		NSMutableArray FromFile (string path);

		[MacCatalyst (13, 1)]
		[Static, Export ("arrayWithContentsOfURL:")]
		NSMutableArray FromUrl (NSUrl url);

#if false // https://github.com/dotnet/macios/issues/15577
		[Export ("applyDifference:")]
		void ApplyDifference (NSOrderedCollectionDifference difference);
#endif
	}

	interface NSMutableArray<TValue> : NSMutableArray { }

	[BaseType (typeof (NSAttributedString))]
	interface NSMutableAttributedString {
		[Export ("initWithString:")]
		NativeHandle Constructor (string str);

		[Export ("initWithString:attributes:")]
		NativeHandle Constructor (string str, [NullAllowed] NSDictionary attributes);

		[Export ("initWithAttributedString:")]
		NativeHandle Constructor (NSAttributedString other);

		[Export ("replaceCharactersInRange:withString:")]
		void Replace (NSRange range, string newValue);

		[Export ("setAttributes:range:")]
		void LowLevelSetAttributes (IntPtr dictionaryAttrsHandle, NSRange range);

		[Export ("mutableString", ArgumentSemantic.Retain)]
		NSMutableString MutableString { get; }

		[Export ("addAttribute:value:range:")]
		void AddAttribute (NSString attributeName, NSObject value, NSRange range);

		[Export ("addAttributes:range:")]
		void AddAttributes (NSDictionary attrs, NSRange range);

		/// <param name="attributes">To be added.</param>
		///         <param name="range">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Wrap ("AddAttributes (attributes.GetDictionary ()!, range)")]
		void AddAttributes (NSStringAttributes attributes, NSRange range);

		[Export ("removeAttribute:range:")]
		void RemoveAttribute (string name, NSRange range);

		[Export ("replaceCharactersInRange:withAttributedString:")]
		void Replace (NSRange range, NSAttributedString value);

		/// <param name="attrString">Attributed string to insert.</param>
		/// <param name="location">Location where the string will be inserted.</param>
		/// <summary>Inserts an attributed string into the current string.</summary>
		/// <remarks>Any attributes that spanned the insertion point will be expanded, so they will continue to cover both the original text as well as the new text.</remarks>
		[Export ("insertAttributedString:atIndex:")]
		void Insert (NSAttributedString attrString, nint location);

		[Export ("appendAttributedString:")]
		void Append (NSAttributedString attrString);

		[Export ("deleteCharactersInRange:")]
		void DeleteRange (NSRange range);

		[Export ("setAttributedString:")]
		void SetString (NSAttributedString attrString);

		[Export ("beginEditing")]
		void BeginEditing ();

		[Export ("endEditing")]
		void EndEditing ();

		/// <param name="url">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="returnOptions">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'ReadFromUrl' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ReadFromUrl' instead.")]
		[Export ("readFromFileURL:options:documentAttributes:error:")]
		bool ReadFromFile (NSUrl url, NSDictionary options, ref NSDictionary returnOptions, ref NSError error);

		/// <param name="url">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="returnOptions">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>Developers should not use this deprecated method. Developers should use 'ReadFromUrl' instead.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'ReadFromUrl' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ReadFromUrl' instead.")]
		[Wrap ("ReadFromFile (url, options.GetDictionary ()!, ref returnOptions, ref error)")]
		bool ReadFromFile (NSUrl url, NSAttributedStringDocumentAttributes options, ref NSDictionary returnOptions, ref NSError error);

		/// <param name="data">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="returnOptions">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("readFromData:options:documentAttributes:error:")]
		bool ReadFromData (NSData data, NSDictionary options, ref NSDictionary returnOptions, ref NSError error);

		/// <param name="data">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="returnOptions">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("ReadFromData (data, options.GetDictionary ()!, ref returnOptions, ref error)")]
		bool ReadFromData (NSData data, NSAttributedStringDocumentAttributes options, ref NSDictionary returnOptions, ref NSError error);

		[Internal]
		[Sealed]
		[MacCatalyst (13, 1)]
		[Export ("readFromURL:options:documentAttributes:error:")]
		bool ReadFromUrl (NSUrl url, NSDictionary options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);

		/// <param name="url">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="returnOptions">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("readFromURL:options:documentAttributes:error:")]
		bool ReadFromUrl (NSUrl url, NSDictionary<NSString, NSObject> options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);

		/// <param name="url">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="returnOptions">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("ReadFromUrl (url, options.GetDictionary ()!, ref returnOptions, ref error)")]
		bool ReadFromUrl (NSUrl url, NSAttributedStringDocumentAttributes options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);
	}

	[BaseType (typeof (NSData))]
	interface NSMutableData {
		/// <param name="capacity">To be added.</param>
		/// <summary>Factory method that instantiates an NSMutableData instance that can initially hold the specified capacity, in bytes.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static, Export ("dataWithCapacity:")]
		[Autorelease]
		[PreSnippet ("if (capacity < 0 || capacity > nint.MaxValue) throw new ArgumentOutOfRangeException ();", Optimizable = true)]
		NSMutableData FromCapacity (nint capacity);

		/// <param name="length">To be added.</param>
		/// <summary>Factory method that instantiates itself with the specified length of zeroed-out bytes.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static, Export ("dataWithLength:")]
		[Autorelease]
		[PreSnippet ("if (length < 0 || length > nint.MaxValue) throw new ArgumentOutOfRangeException ();", Optimizable = true)]
		NSMutableData FromLength (nint length);

		[Static, Export ("data")]
		[Autorelease]
		NSMutableData Create ();

		[Export ("mutableBytes")]
		IntPtr MutableBytes { get; }

		/// <param name="capacity">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithCapacity:")]
		[PreSnippet ("if (capacity > (ulong) nint.MaxValue) throw new ArgumentOutOfRangeException ();", Optimizable = true)]
		NativeHandle Constructor (nuint capacity);

		[Export ("appendData:")]
		void AppendData (NSData other);

		[Export ("appendBytes:length:")]
		void AppendBytes (IntPtr bytes, nuint len);

		[Export ("setData:")]
		void SetData (NSData data);

		[Export ("length")]
		[Override]
		nuint Length { get; set; }

		[Export ("replaceBytesInRange:withBytes:")]
		void ReplaceBytes (NSRange range, IntPtr buffer);

		[Export ("resetBytesInRange:")]
		void ResetBytes (NSRange range);

		[Export ("replaceBytesInRange:withBytes:length:")]
		void ReplaceBytes (NSRange range, IntPtr buffer, nuint length);

		// NSMutableDataCompression (NSMutableData)

		[MacCatalyst (13, 1)]
		[Export ("decompressUsingAlgorithm:error:")]
		bool Decompress (NSDataCompressionAlgorithm algorithm, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("compressUsingAlgorithm:error:")]
		bool Compress (NSDataCompressionAlgorithm algorithm, [NullAllowed] out NSError error);
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum NSDataCompressionAlgorithm : long {
		Lzfse = 0,
		Lz4,
		Lzma,
		Zlib,
	}

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSDate : NSSecureCoding, NSCopying, CKRecordValue {
		[Export ("timeIntervalSinceReferenceDate")]
		double SecondsSinceReferenceDate { get; }

		[Export ("timeIntervalSinceDate:")]
		double GetSecondsSince (NSDate anotherDate);

		[Export ("timeIntervalSinceNow")]
		double SecondsSinceNow { get; }

		[Export ("timeIntervalSince1970")]
		double SecondsSince1970 { get; }

		[Export ("dateWithTimeIntervalSinceReferenceDate:")]
		[Static]
		NSDate FromTimeIntervalSinceReferenceDate (double secs);

		[Static, Export ("dateWithTimeIntervalSince1970:")]
		NSDate FromTimeIntervalSince1970 (double secs);

		[Export ("date")]
		[Static]
		NSDate Now { get; }

		[Export ("distantPast")]
		[Static]
		NSDate DistantPast { get; }

		[Export ("distantFuture")]
		[Static]
		NSDate DistantFuture { get; }

		[Export ("dateByAddingTimeInterval:")]
		NSDate AddSeconds (double seconds);

		[Export ("dateWithTimeIntervalSinceNow:")]
		[Static]
		NSDate FromTimeIntervalSinceNow (double secs);

		[Export ("descriptionWithLocale:")]
		string DescriptionWithLocale (NSLocale locale);

		[Export ("earlierDate:")]
		NSDate EarlierDate (NSDate anotherDate);

		[Export ("laterDate:")]
		NSDate LaterDate (NSDate anotherDate);

		[Export ("compare:")]
		NSComparisonResult Compare (NSDate other);

		[Export ("isEqualToDate:")]
		bool IsEqualToDate (NSDate other);

		// NSDate_SensorKit

		[NoTV, NoMac]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("dateWithSRAbsoluteTime:")]
		NSDate CreateFromSRAbsoluteTime (double time);

		[NoTV, NoMac]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithSRAbsoluteTime:")]
		NativeHandle Constructor (double srAbsoluteTime);

		[NoTV, NoMac]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("srAbsoluteTime")]
		double SrAbsoluteTime { get; }

		[Field ("NSSystemClockDidChangeNotification")]
		[Notification]
		NSString SystemClockDidChangeNotification { get; }
	}

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSDictionary : NSSecureCoding, NSMutableCopying, NSFetchRequestResult, INSFastEnumeration {
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSMutableDictionary.FromFile' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSMutableDictionary.FromFile' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSMutableDictionary.FromFile' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSMutableDictionary.FromFile' instead.")]
		[Export ("dictionaryWithContentsOfFile:")]
		[Static]
		NSDictionary FromFile (string path);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSMutableDictionary.FromUrl' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSMutableDictionary.FromUrl' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSMutableDictionary.FromUrl' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSMutableDictionary.FromUrl' instead.")]
		[Export ("dictionaryWithContentsOfURL:")]
		[Static]
		NSDictionary FromUrl (NSUrl url);

		[Export ("dictionaryWithObject:forKey:")]
		[Static]
		NSDictionary FromObjectAndKey (NSObject obj, NSObject key);

		[Export ("dictionaryWithDictionary:")]
		[Static]
		NSDictionary FromDictionary (NSDictionary source);

		[Export ("dictionaryWithObjects:forKeys:count:")]
		[Static, Internal]
		IntPtr _FromObjectsAndKeysInternal (IntPtr objects, IntPtr keys, nint count);

		[Export ("dictionaryWithObjects:forKeys:count:")]
		[Static, Internal]
		NSDictionary FromObjectsAndKeysInternal ([NullAllowed] NSArray objects, [NullAllowed] NSArray keys, nint count);

		[Export ("dictionaryWithObjects:forKeys:")]
		[Static, Internal]
		IntPtr _FromObjectsAndKeysInternal (IntPtr objects, IntPtr keys);

		[Export ("dictionaryWithObjects:forKeys:")]
		[Static, Internal]
		NSDictionary FromObjectsAndKeysInternal (NSArray objects, NSArray keys);

		[Export ("initWithDictionary:")]
		NativeHandle Constructor (NSDictionary other);

		[Export ("initWithDictionary:copyItems:")]
		NativeHandle Constructor (NSDictionary other, bool copyItems);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSMutableDictionary(string)' constructor instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSMutableDictionary(string)' constructor instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSMutableDictionary(string)' constructor instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSMutableDictionary(string)' constructor instead.")]
		[Export ("initWithContentsOfFile:")]
		NativeHandle Constructor (string fileName);

		[Export ("initWithObjects:forKeys:"), Internal]
		NativeHandle Constructor (NSArray objects, NSArray keys);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSMutableDictionary(NSUrl)' constructor instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSMutableDictionary(NSUrl)' constructor instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSMutableDictionary(NSUrl)' constructor instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSMutableDictionary(NSUrl)' constructor instead.")]
		[Export ("initWithContentsOfURL:")]
		NativeHandle Constructor (NSUrl url);

		[MacCatalyst (13, 1)]
		[Export ("initWithContentsOfURL:error:")]
		NativeHandle Constructor (NSUrl url, out NSError error);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("dictionaryWithContentsOfURL:error:")]
		[return: NullAllowed]
		NSDictionary<NSString, NSObject> FromUrl (NSUrl url, out NSError error);

		[Export ("count")]
		nuint Count { get; }

		[Internal]
		[Sealed]
		[Export ("objectForKey:")]
		IntPtr _ObjectForKey (IntPtr key);

		[return: NullAllowed]
		[Export ("objectForKey:")]
		NSObject ObjectForKey (NSObject key);

		[Internal]
		[Sealed]
		[Export ("allKeys")]
		IntPtr _AllKeys ();

		[Export ("allKeys")]
		[Autorelease]
		NSObject [] Keys { get; }

		[Internal]
		[Sealed]
		[Export ("allKeysForObject:")]
		IntPtr _AllKeysForObject (IntPtr obj);

		[Export ("allKeysForObject:")]
		[Autorelease]
		NSObject [] KeysForObject (NSObject obj);

		[Internal]
		[Sealed]
		[Export ("allValues")]
		IntPtr _AllValues ();

		[Export ("allValues")]
		[Autorelease]
		NSObject [] Values { get; }

		[Export ("descriptionInStringsFileFormat")]
		string DescriptionInStringsFileFormat { get; }

		[Export ("isEqualToDictionary:")]
		bool IsEqualToDictionary (NSDictionary other);

		[Export ("objectEnumerator")]
		NSEnumerator ObjectEnumerator { get; }

		[Internal]
		[Sealed]
		[Export ("objectsForKeys:notFoundMarker:")]
		IntPtr _ObjectsForKeys (IntPtr keys, IntPtr marker);

		[Export ("objectsForKeys:notFoundMarker:")]
		[Autorelease]
		NSObject [] ObjectsForKeys (NSArray keys, NSObject marker);

		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("writeToFile:atomically:")]
		bool WriteToFile (string path, bool useAuxiliaryFile);

		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("writeToURL:atomically:")]
		bool WriteToUrl (NSUrl url, bool atomically);

		[Static]
		[Export ("sharedKeySetForKeys:")]
		NSObject GetSharedKeySetForKeys (NSObject [] keys);

		[Export ("descriptionWithLocale:")]
		string GetDescription ([NullAllowed] NSObject locale);

		[Export ("descriptionWithLocale:indent:")]
		string GetDescription ([NullAllowed] NSObject locale, nuint level);

		[Export ("enumerateKeysAndObjectsUsingBlock:")]
		void Enumerate (NSDictionaryEnumerator block);

		[Export ("enumerateKeysAndObjectsWithOptions:usingBlock:")]
		void Enumerate (NSEnumerationOptions options, NSDictionaryEnumerator block);

		[Export ("keysOfEntriesPassingTest:")]
		NSSet GetKeys (NSDictionaryKeyFilter predicate);

		[Export ("keysOfEntriesWithOptions:passingTest:")]
		NSSet GetKeys (NSEnumerationOptions options, NSDictionaryKeyFilter predicate);

		[Export ("keysSortedByValueUsingComparator:")]
		NSObject [] GetKeysSortedByValue (NSComparator comparator);

		[Export ("keysSortedByValueWithOptions:usingComparator:")]
		NSObject [] GetKeysSortedByValue (NSSortOptions options, NSComparator comparator);

		[return: NullAllowed]
		[Export ("valueForKey:")]
		[MarshalNativeExceptions]
		NSObject ValueForKey (NSString key);

		[Export ("writeToURL:error:")]
		bool WriteToUrl (NSUrl url, [NullAllowed] out NSError error);

		[Export ("keyEnumerator")]
		NSEnumerator KeyEnumerator { get; }
	}

	delegate void NSDictionaryEnumerator (NSObject key, NSObject obj, ref bool stop);
	delegate bool NSDictionaryKeyFilter (NSObject key, NSObject obj, ref bool stop);

	interface NSDictionary<K, V> : NSDictionary { }

	[BaseType (typeof (NSObject))]
	interface NSEnumerator {
		[Export ("nextObject")]
		NSObject NextObject ();
	}

	interface NSEnumerator<T> : NSEnumerator { }

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSError : NSSecureCoding, NSCopying {
		/// <param name="domain">To be added.</param>
		/// <param name="code">To be added.</param>
		/// <param name="userInfo">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static, Export ("errorWithDomain:code:userInfo:")]
		NSError FromDomain (NSString domain, nint code, [NullAllowed] NSDictionary userInfo);

		/// <param name="domain">To be added.</param>
		/// <param name="code">To be added.</param>
		/// <param name="userInfo">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[DesignatedInitializer]
		[Export ("initWithDomain:code:userInfo:")]
		NativeHandle Constructor (NSString domain, nint code, [NullAllowed] NSDictionary userInfo);

		[Export ("domain")]
		string Domain { get; }

		[Export ("code")]
		nint Code { get; }

		[Export ("userInfo")]
		NSDictionary UserInfo { get; }

		[Export ("localizedDescription")]
		string LocalizedDescription { get; }

		[Export ("localizedFailureReason")]
		string LocalizedFailureReason { get; }

		[Export ("localizedRecoverySuggestion")]
		string LocalizedRecoverySuggestion { get; }

		[Export ("localizedRecoveryOptions")]
		string [] LocalizedRecoveryOptions { get; }

		[Export ("helpAnchor")]
		string HelpAnchor { get; }

		[TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("underlyingErrors", ArgumentSemantic.Copy)]
		NSError [] UnderlyingErrors { get; }

		/// <summary>The Cocoa error domain.</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSCocoaErrorDomain")]
		NSString CocoaErrorDomain { get; }

		/// <summary>Represents the value associated with the constant NSPOSIXErrorDomain</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSPOSIXErrorDomain")]
		NSString PosixErrorDomain { get; }

		/// <summary>Represents the value associated with the constant NSOSStatusErrorDomain</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSOSStatusErrorDomain")]
		NSString OsStatusErrorDomain { get; }

		/// <summary>Represents the value associated with the constant NSMachErrorDomain</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSMachErrorDomain")]
		NSString MachErrorDomain { get; }

		/// <summary>Represents the value associated with the constant NSURLErrorDomain</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLErrorDomain")]
		NSString NSUrlErrorDomain { get; }

		/// <summary>Represents the value associated with the constant NSNetServicesErrorDomain</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSNetServicesErrorDomain")]
		NSString NSNetServicesErrorDomain { get; }

		[Field ("NSNetServicesErrorCode")]
		NSString NSNetServicesErrorCode { get; }

		/// <summary>Represents the value associated with the constant NSStreamSocketSSLErrorDomain</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSStreamSocketSSLErrorDomain")]
		NSString NSStreamSocketSSLErrorDomain { get; }

		/// <summary>Represents the value associated with the constant NSStreamSOCKSErrorDomain</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSStreamSOCKSErrorDomain")]
		NSString NSStreamSOCKSErrorDomain { get; }

		/// <summary>The CoreLocation error domain.</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("kCLErrorDomain", "CoreLocation")]
		NSString CoreLocationErrorDomain { get; }

		/// <summary>The CFNetwork error domain.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("kCFErrorDomainCFNetwork", "CFNetwork")]
		NSString CFNetworkErrorDomain { get; }

		/// <summary>The CoreMotion error domain.</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoMac, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("CMErrorDomain", "CoreMotion")]
		NSString CoreMotionErrorDomain { get; }

		[NoMac, NoTV]
		[NoMacCatalyst] // We don't expose CarPlay on Mac Catalyst for the moment // [MacCatalyst (14, 0)]
		[Field ("CarPlayErrorDomain", "CarPlay")]
		NSString CarPlayErrorDomain { get; }

		/// <summary>Represents the value associated with the constant NSUnderlyingErrorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSUnderlyingErrorKey")]
		NSString UnderlyingErrorKey { get; }

		[TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("NSMultipleUnderlyingErrorsKey")]
		NSString MultipleUnderlyingErrorsKey { get; }

		/// <summary>Standard key to use in the userInfo payload for associating a localized description of the problem to provide to the user.</summary>
		///         <value />
		///         <remarks>To be added.</remarks>
		[Field ("NSLocalizedDescriptionKey")]
		NSString LocalizedDescriptionKey { get; }

		/// <summary>Represents the value associated with the constant NSLocalizedFailureReasonErrorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSLocalizedFailureReasonErrorKey")]
		NSString LocalizedFailureReasonErrorKey { get; }

		/// <summary>Represents the value associated with the constant NSLocalizedRecoverySuggestionErrorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSLocalizedRecoverySuggestionErrorKey")]
		NSString LocalizedRecoverySuggestionErrorKey { get; }

		/// <summary>Represents the value associated with the constant NSLocalizedRecoveryOptionsErrorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSLocalizedRecoveryOptionsErrorKey")]
		NSString LocalizedRecoveryOptionsErrorKey { get; }

		/// <summary>Represents the value associated with the constant NSRecoveryAttempterErrorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSRecoveryAttempterErrorKey")]
		NSString RecoveryAttempterErrorKey { get; }

		/// <summary>Standard key to use in the userInfo payload for associating a hint to provide to the user.</summary>
		///         <value />
		///         <remarks>To be added.</remarks>
		[Field ("NSHelpAnchorErrorKey")]
		NSString HelpAnchorErrorKey { get; }

		/// <summary>Represents the value associated with the constant NSStringEncodingErrorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSStringEncodingErrorKey")]
		NSString StringEncodingErrorKey { get; }

		/// <summary>Represents the value associated with the constant NSURLErrorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLErrorKey")]
		NSString UrlErrorKey { get; }

		/// <summary>Standard key to use in the userInfo payload for associating a file path with the error.</summary>
		///         <value />
		///         <remarks>To be added.</remarks>
		[Field ("NSFilePathErrorKey")]
		NSString FilePathErrorKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSDebugDescriptionErrorKey")]
		NSString DebugDescriptionErrorKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSLocalizedFailureErrorKey")]
		NSString LocalizedFailureErrorKey { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("setUserInfoValueProviderForDomain:provider:")]
		void SetUserInfoValueProvider (string errorDomain, [NullAllowed] NSErrorUserInfoValueProvider provider);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("userInfoValueProviderForDomain:")]
		[return: NullAllowed]
		NSErrorUserInfoValueProvider GetUserInfoValueProvider (string errorDomain);

		// From NSError (NSFileProviderError) Category to avoid static category uglyness

		/// <param name="existingItem">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMacCatalyst]
		[NoTV]
		[Static]
		[Export ("fileProviderErrorForCollisionWithItem:")]
		NSError GetFileProviderError (INSFileProviderItem existingItem);

		/// <param name="nonExistentItemIdentifier">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMacCatalyst]
		[NoTV]
		[Static]
		[Export ("fileProviderErrorForNonExistentItemWithIdentifier:")]
		NSError GetFileProviderError (string nonExistentItemIdentifier);

		[iOS (16, 0)]
		[NoMacCatalyst]
		[NoTV]
		[Static]
		[Export ("fileProviderErrorForRejectedDeletionOfItem:")]
		NSError GetFileProviderErrorForRejectedDeletion (INSFileProviderItem updatedVersion);

#if false
		// FIXME that value is present in the header (7.0 DP 6) files but returns NULL (i.e. unusable)
		// we're also missing other NSURLError* fields (which we should add)
		[Field ("NSURLErrorBackgroundTaskCancelledReasonKey")]
		NSString NSUrlErrorBackgroundTaskCancelledReasonKey { get; }
#endif

#if IOS && !MACCATALYST
		[iOS (18, 2), NoMacCatalyst, NoTV, NoMac]
		[Field ("UIApplicationCategoryDefaultRetryAvailabilityDateErrorKey", "UIKit")]
		NSString UIApplicationCategoryDefaultRetryAvailabilityDateErrorKey { get; }

		[iOS (18, 2), NoMacCatalyst, NoTV, NoMac]
		[Field ("UIApplicationCategoryDefaultStatusLastProvidedDateErrorKey", "UIKit")]
		NSString UIApplicationCategoryDefaultStatusLastProvidedDateErrorKey { get; }
#endif
	}

	/// <param name="error">To be added.</param>
	/// <param name="userInfoKey">To be added.</param>
	/// <summary>Delegate returned by <see cref="Foundation.NSError.GetUserInfoValueProvider(System.String)" />.</summary>
	/// <returns>To be added.</returns>
	/// <remarks>To be added.</remarks>
	delegate NSObject NSErrorUserInfoValueProvider (NSError error, NSString userInfoKey);

	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface NSException : NSSecureCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithName:reason:userInfo:")]
		NativeHandle Constructor (string name, string reason, [NullAllowed] NSDictionary userInfo);

		[Export ("name")]
		string Name { get; }

		[Export ("reason")]
		string Reason { get; }

		[Export ("userInfo")]
		NSObject UserInfo { get; }

		[Export ("callStackReturnAddresses")]
		NSNumber [] CallStackReturnAddresses { get; }

		[Export ("callStackSymbols")]
		string [] CallStackSymbols { get; }
	}

	delegate NSObject NSExpressionCallbackHandler ([NullAllowed] NSObject evaluatedObject, NSExpression [] expressions, [NullAllowed] NSMutableDictionary context);
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -predicateFormat cannot be sent to an abstract object of class NSExpression: Create a concrete instance!
	[DisableDefaultCtor]
	interface NSExpression : NSSecureCoding, NSCopying {
		[Static, Export ("expressionForConstantValue:")]
		NSExpression FromConstant ([NullAllowed] NSObject obj);

		[Static, Export ("expressionForEvaluatedObject")]
		NSExpression ExpressionForEvaluatedObject { get; }

		[Static, Export ("expressionForVariable:")]
		NSExpression FromVariable (string string1);

		[Static, Export ("expressionForKeyPath:")]
		NSExpression FromKeyPath (string keyPath);

		[Static, Export ("expressionForFunction:arguments:")]
		NSExpression FromFunction (string name, NSExpression [] parameters);

		[Static, Export ("expressionWithFormat:")]
		NSExpression FromFormat (string expressionFormat);

		[Static, Export ("expressionWithFormat:argumentArray:")]
		NSExpression FromFormat (string format, NSObject [] parameters);

		//+ (NSExpression *)expressionForAggregate:(NSArray *)subexpressions; 
		[Static, Export ("expressionForAggregate:")]
		NSExpression FromAggregate (NSExpression [] subexpressions);

		[Static, Export ("expressionForUnionSet:with:")]
		NSExpression FromUnionSet (NSExpression left, NSExpression right);

		[Static, Export ("expressionForIntersectSet:with:")]
		NSExpression FromIntersectSet (NSExpression left, NSExpression right);

		[Static, Export ("expressionForMinusSet:with:")]
		NSExpression FromMinusSet (NSExpression left, NSExpression right);

		//+ (NSExpression *)expressionForSubquery:(NSExpression *)expression usingIteratorVariable:(NSString *)variable predicate:(id)predicate; 
		[Static, Export ("expressionForSubquery:usingIteratorVariable:predicate:")]
		NSExpression FromSubquery (NSExpression expression, string variable, NSObject predicate);

		[Static, Export ("expressionForFunction:selectorName:arguments:")]
		NSExpression FromFunction (NSExpression target, string name, NSExpression [] parameters);

		[Static, Export ("expressionForBlock:arguments:")]
		NSExpression FromFunction (NSExpressionCallbackHandler target, NSExpression [] parameters);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("expressionForAnyKey")]
		NSExpression FromAnyKey ();

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("expressionForConditional:trueExpression:falseExpression:")]
		NSExpression FromConditional (NSPredicate predicate, NSExpression trueExpression, NSExpression falseExpression);

		[MacCatalyst (13, 1)]
		[Export ("allowEvaluation")]
		void AllowEvaluation ();

		[DesignatedInitializer]
		[Export ("initWithExpressionType:")]
		NativeHandle Constructor (NSExpressionType type);

		[Export ("expressionType")]
		NSExpressionType ExpressionType { get; }

		[Sealed, Internal, Export ("expressionBlock")]
		NSExpressionCallbackHandler _Block { get; }

		[Sealed, Internal, Export ("constantValue")]
		NSObject _ConstantValue { get; }

		[Sealed, Internal, Export ("keyPath")]
		string _KeyPath { get; }

		[Sealed, Internal, Export ("function")]
		string _Function { get; }

		[Sealed, Internal, Export ("variable")]
		string _Variable { get; }

		[Sealed, Internal, Export ("operand")]
		NSExpression _Operand { get; }

		[Sealed, Internal, Export ("arguments")]
		NSExpression [] _Arguments { get; }

		[Sealed, Internal, Export ("collection")]
		NSObject _Collection { get; }

		[Sealed, Internal, Export ("predicate")]
		NSPredicate _Predicate { get; }

		[Sealed, Internal, Export ("leftExpression")]
		NSExpression _LeftExpression { get; }

		[Sealed, Internal, Export ("rightExpression")]
		NSExpression _RightExpression { get; }

		[MacCatalyst (13, 1)]
		[Sealed, Internal, Export ("trueExpression")]
		NSExpression _TrueExpression { get; }

		[MacCatalyst (13, 1)]
		[Sealed, Internal, Export ("falseExpression")]
		NSExpression _FalseExpression { get; }

		[Export ("expressionValueWithObject:context:")]
		[return: NullAllowed]
		NSObject EvaluateWith ([NullAllowed] NSObject obj, [NullAllowed] NSMutableDictionary context);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface NSExtensionContext {

		[Export ("inputItems", ArgumentSemantic.Copy)]
		NSExtensionItem [] InputItems { get; }

		[Async (XmlDocs = """
			<param name="returningItems">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous CompleteRequest operation.  The value of the TResult parameter is of type System.Action&lt;System.Boolean&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("completeRequestReturningItems:completionHandler:")]
		void CompleteRequest (NSExtensionItem [] returningItems, [NullAllowed] Action<bool> completionHandler);

		[Export ("cancelRequestWithError:")]
		void CancelRequest (NSError error);

		[Export ("openURL:completionHandler:")]
		[Async (XmlDocs = """
			<param name="url">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous OpenUrl operation.  The value of the TResult parameter is of type System.Action&lt;System.Boolean&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The OpenUrlAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void OpenUrl (NSUrl url, [NullAllowed] Action<bool> completionHandler);

		/// <summary>Represents the value associated with the constant NSExtensionItemsAndErrorsKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSExtensionItemsAndErrorsKey")]
		NSString ItemsAndErrorsKey { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSExtensionHostWillEnterForegroundNotification")]
		NSString HostWillEnterForegroundNotification { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSExtensionHostDidEnterBackgroundNotification")]
		NSString HostDidEnterBackgroundNotification { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSExtensionHostWillResignActiveNotification")]
		NSString HostWillResignActiveNotification { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSExtensionHostDidBecomeActiveNotification")]
		NSString HostDidBecomeActiveNotification { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface NSExtensionItem : NSCopying, NSSecureCoding {

		[NullAllowed] // by default this property is null
		[Export ("attributedTitle", ArgumentSemantic.Copy)]
		NSAttributedString AttributedTitle { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("attributedContentText", ArgumentSemantic.Copy)]
		NSAttributedString AttributedContentText { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("attachments", ArgumentSemantic.Copy)]
		NSItemProvider [] Attachments { get; set; }

		[Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary UserInfo { get; set; }

		/// <summary>Represents the value associated with the constant NSExtensionItemAttributedTitleKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSExtensionItemAttributedTitleKey")]
		NSString AttributedTitleKey { get; }

		/// <summary>Represents the value associated with the constant NSExtensionItemAttributedContentTextKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSExtensionItemAttributedContentTextKey")]
		NSString AttributedContentTextKey { get; }

		/// <summary>Represents the value associated with the constant NSExtensionItemAttachmentsKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSExtensionItemAttachmentsKey")]
		NSString AttachmentsKey { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSNull : NSSecureCoding, NSCopying
	, CAAction {
		[Export ("null"), Static]
		[Internal]
		NSNull _Null { get; }

		[Export ("null"), Static]
		[Internal]
		IntPtr _NullHandle { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter))]
	interface NSLengthFormatter {
		[Export ("numberFormatter", ArgumentSemantic.Copy)]
		NSNumberFormatter NumberFormatter { get; set; }

		[Export ("unitStyle")]
		NSFormattingUnitStyle UnitStyle { get; set; }

		[Export ("stringFromValue:unit:")]
		string StringFromValue (double value, NSLengthFormatterUnit unit);

		[Export ("stringFromMeters:")]
		string StringFromMeters (double numberInMeters);

		[Export ("unitStringFromValue:unit:")]
		string UnitStringFromValue (double value, NSLengthFormatterUnit unit);

		[Export ("unitStringFromMeters:usedUnit:")]
		string UnitStringFromMeters (double numberInMeters, ref NSLengthFormatterUnit unitp);

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject obj, string str, out string error);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("forPersonHeightUse")]
		bool ForPersonHeightUse { [Bind ("isForPersonHeightUse")] get; set; }
	}

	/// <param name="tag">To be added.</param>
	/// <param name="tokenRange">To be added.</param>
	/// <param name="sentenceRange">To be added.</param>
	/// <param name="stop">To be added.</param>
	/// <summary>A delegate that enumerates values for <see cref="Foundation.NSLinguisticTagger.EnumerateTagsInRange(Foundation.NSRange,Foundation.NSString,Foundation.NSLinguisticTaggerOptions,Foundation.NSLingusticEnumerator)" />.</summary>
	/// <remarks>To be added.</remarks>
#if XAMCORE_5_0
	delegate void NSLinguisticEnumerator ([NullAllowed] NSString tag, NSRange tokenRange, NSRange sentenceRange, ref bool stop);
#else
	delegate void NSLingusticEnumerator ([NullAllowed] NSString tag, NSRange tokenRange, NSRange sentenceRange, ref bool stop);
#endif

	[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'NaturalLanguage.*' API instead.")]
	[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'NaturalLanguage.*' API instead.")]
	[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'NaturalLanguage.*' API instead.")]
	[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'NaturalLanguage.*' API instead.")]
	[BaseType (typeof (NSObject))]
	interface NSLinguisticTagger {
		[DesignatedInitializer]
		[Export ("initWithTagSchemes:options:")]
		NativeHandle Constructor (NSString [] tagSchemes, NSLinguisticTaggerOptions opts);

		[Export ("tagSchemes")]
		NSString [] TagSchemes { get; }

		[Static]
		[Export ("availableTagSchemesForLanguage:")]
		NSString [] GetAvailableTagSchemesForLanguage (string language);

		[Export ("setOrthography:range:")]
		void SetOrthographyrange (NSOrthography orthography, NSRange range);

		/// <param name="charIndex">To be added.</param>
		/// <param name="effectiveRange">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("orthographyAtIndex:effectiveRange:")]
		NSOrthography GetOrthography (nint charIndex, ref NSRange effectiveRange);

		/// <param name="newRange">To be added.</param>
		/// <param name="delta">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("stringEditedInRange:changeInLength:")]
		void StringEditedInRange (NSRange newRange, nint delta);

		[Export ("enumerateTagsInRange:scheme:options:usingBlock:")]
#if XAMCORE_5_0
		void EnumerateTagsInRange (NSRange range, NSString tagScheme, NSLinguisticTaggerOptions opts, NSLinguisticEnumerator enumerator);
#else
		void EnumerateTagsInRange (NSRange range, NSString tagScheme, NSLinguisticTaggerOptions opts, NSLingusticEnumerator enumerator);
#endif

		[Export ("sentenceRangeForRange:")]
		NSRange GetSentenceRangeForRange (NSRange range);

		/// <param name="charIndex">To be added.</param>
		/// <param name="tagScheme">To be added.</param>
		/// <param name="tokenRange">To be added.</param>
		/// <param name="sentenceRange">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("tagAtIndex:scheme:tokenRange:sentenceRange:")]
		string GetTag (nint charIndex, NSString tagScheme, ref NSRange tokenRange, ref NSRange sentenceRange);

		[Export ("tagsInRange:scheme:options:tokenRanges:"), Internal]
		NSString [] GetTagsInRange (NSRange range, NSString tagScheme, NSLinguisticTaggerOptions opts, ref NSArray tokenRanges);

		[Export ("possibleTagsAtIndex:scheme:tokenRange:sentenceRange:scores:"), Internal]
		NSString [] GetPossibleTags (nint charIndex, NSString tagScheme, ref NSRange tokenRange, ref NSRange sentenceRange, ref NSArray scores);

		//Detected properties
		[NullAllowed] // by default this property is null
		[Export ("string", ArgumentSemantic.Retain)]
		string AnalysisString { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("tagsInRange:unit:scheme:options:tokenRanges:")]
		string [] GetTags (NSRange range, NSLinguisticTaggerUnit unit, string scheme, NSLinguisticTaggerOptions options, [NullAllowed] out NSValue [] tokenRanges);

		[MacCatalyst (13, 1)]
		[Export ("enumerateTagsInRange:unit:scheme:options:usingBlock:")]
		void EnumerateTags (NSRange range, NSLinguisticTaggerUnit unit, string scheme, NSLinguisticTaggerOptions options, LinguisticTagEnumerator enumerator);

		/// <param name="charIndex">To be added.</param>
		/// <param name="unit">To be added.</param>
		/// <param name="scheme">To be added.</param>
		/// <param name="tokenRange">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("tagAtIndex:unit:scheme:tokenRange:")]
		[return: NullAllowed]
		string GetTag (nuint charIndex, NSLinguisticTaggerUnit unit, string scheme, [NullAllowed] ref NSRange tokenRange);

		/// <param name="charIndex">To be added.</param>
		/// <param name="unit">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("tokenRangeAtIndex:unit:")]
		NSRange GetTokenRange (nuint charIndex, NSLinguisticTaggerUnit unit);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("availableTagSchemesForUnit:language:")]
		string [] GetAvailableTagSchemes (NSLinguisticTaggerUnit unit, string language);

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("dominantLanguage")]
		string DominantLanguage { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("dominantLanguageForString:")]
		[return: NullAllowed]
		string GetDominantLanguage (string str);

		/// <param name="str">To be added.</param>
		/// <param name="charIndex">To be added.</param>
		/// <param name="unit">To be added.</param>
		/// <param name="scheme">To be added.</param>
		/// <param name="orthography">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="tokenRange">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("tagForString:atIndex:unit:scheme:orthography:tokenRange:")]
		[return: NullAllowed]
		string GetTag (string str, nuint charIndex, NSLinguisticTaggerUnit unit, string scheme, [NullAllowed] NSOrthography orthography, [NullAllowed] ref NSRange tokenRange);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("tagsForString:range:unit:scheme:options:orthography:tokenRanges:")]
		string [] GetTags (string str, NSRange range, NSLinguisticTaggerUnit unit, string scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, [NullAllowed] out NSValue [] tokenRanges);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("enumerateTagsForString:range:unit:scheme:options:orthography:usingBlock:")]
		void EnumerateTags (string str, NSRange range, NSLinguisticTaggerUnit unit, string scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, LinguisticTagEnumerator enumerator);
	}

	delegate void LinguisticTagEnumerator (string tag, NSRange tokenRange, bool stop);

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSLocale : NSSecureCoding, NSCopying {
		[Static]
		[Export ("systemLocale", ArgumentSemantic.Copy)]
		NSLocale SystemLocale { get; }

		[Static]
		[Export ("currentLocale", ArgumentSemantic.Copy)]
		NSLocale CurrentLocale { get; }

		[Static]
		[Export ("autoupdatingCurrentLocale", ArgumentSemantic.Strong)]
		NSLocale AutoUpdatingCurrentLocale { get; }

		[DesignatedInitializer]
		[Export ("initWithLocaleIdentifier:")]
		NativeHandle Constructor (string identifier);

		[Export ("localeIdentifier")]
		string LocaleIdentifier { get; }

		[Export ("availableLocaleIdentifiers", ArgumentSemantic.Copy)]
		[Static]
		string [] AvailableLocaleIdentifiers { get; }

		[Export ("ISOLanguageCodes", ArgumentSemantic.Copy)]
		[Static]
		string [] ISOLanguageCodes { get; }

		[Export ("ISOCurrencyCodes", ArgumentSemantic.Copy)]
		[Static]
		string [] ISOCurrencyCodes { get; }

		[Export ("ISOCountryCodes", ArgumentSemantic.Copy)]
		[Static]
		string [] ISOCountryCodes { get; }

		[Export ("commonISOCurrencyCodes", ArgumentSemantic.Copy)]
		[Static]
		string [] CommonISOCurrencyCodes { get; }

		[Export ("preferredLanguages", ArgumentSemantic.Copy)]
		[Static]
		string [] PreferredLanguages { get; }

		[Export ("componentsFromLocaleIdentifier:")]
		[Static]
		NSDictionary ComponentsFromLocaleIdentifier (string identifier);

		[Export ("localeIdentifierFromComponents:")]
		[Static]
		string LocaleIdentifierFromComponents (NSDictionary dict);

		[Export ("canonicalLanguageIdentifierFromString:")]
		[Static]
		string CanonicalLanguageIdentifierFromString (string str);

		[Export ("canonicalLocaleIdentifierFromString:")]
		[Static]
		string CanonicalLocaleIdentifierFromString (string str);

		[Export ("characterDirectionForLanguage:")]
		[Static]
		NSLocaleLanguageDirection GetCharacterDirection (string isoLanguageCode);

		[Export ("lineDirectionForLanguage:")]
		[Static]
		NSLocaleLanguageDirection GetLineDirection (string isoLanguageCode);

		[Static]
		[Export ("localeWithLocaleIdentifier:")]
		NSLocale FromLocaleIdentifier (string ident);

		[Field ("NSCurrentLocaleDidChangeNotification")]
		[Notification]
		NSString CurrentLocaleDidChangeNotification { get; }

		[return: NullAllowed]
		[Export ("objectForKey:"), Internal]
		NSObject ObjectForKey (NSString key);

		[return: NullAllowed]
		[Export ("displayNameForKey:value:"), Internal]
		NSString DisplayNameForKey (NSString key, string value);

		[Internal, Field ("NSLocaleIdentifier")]
		NSString _Identifier { get; }

		[Internal, Field ("NSLocaleLanguageCode")]
		NSString _LanguageCode { get; }

		[Internal, Field ("NSLocaleCountryCode")]
		NSString _CountryCode { get; }

		[Internal, Field ("NSLocaleScriptCode")]
		NSString _ScriptCode { get; }

		[Internal, Field ("NSLocaleVariantCode")]
		NSString _VariantCode { get; }

		[Internal, Field ("NSLocaleExemplarCharacterSet")]
		NSString _ExemplarCharacterSet { get; }

		[Internal, Field ("NSLocaleCalendar")]
		NSString _Calendar { get; }

		[Internal, Field ("NSLocaleCollationIdentifier")]
		NSString _CollationIdentifier { get; }

		[Internal, Field ("NSLocaleUsesMetricSystem")]
		NSString _UsesMetricSystem { get; }

		[Internal, Field ("NSLocaleMeasurementSystem")]
		NSString _MeasurementSystem { get; }

		[Internal, Field ("NSLocaleDecimalSeparator")]
		NSString _DecimalSeparator { get; }

		[Internal, Field ("NSLocaleGroupingSeparator")]
		NSString _GroupingSeparator { get; }

		[Internal, Field ("NSLocaleCurrencySymbol")]
		NSString _CurrencySymbol { get; }

		[Internal, Field ("NSLocaleCurrencyCode")]
		NSString _CurrencyCode { get; }

		[Internal, Field ("NSLocaleCollatorIdentifier")]
		NSString _CollatorIdentifier { get; }

		[Internal, Field ("NSLocaleQuotationBeginDelimiterKey")]
		NSString _QuotationBeginDelimiterKey { get; }

		[Internal, Field ("NSLocaleQuotationEndDelimiterKey")]
		NSString _QuotationEndDelimiterKey { get; }

		[Internal, Field ("NSLocaleAlternateQuotationBeginDelimiterKey")]
		NSString _AlternateQuotationBeginDelimiterKey { get; }

		[Internal, Field ("NSLocaleAlternateQuotationEndDelimiterKey")]
		NSString _AlternateQuotationEndDelimiterKey { get; }

		// follow the pattern of NSLocale.cs which included managed helpers that did the same

		[MacCatalyst (13, 1)]
		[Export ("calendarIdentifier")]
		string CalendarIdentifier { get; }

		[MacCatalyst (13, 1)]
		[Export ("localizedStringForCalendarIdentifier:")]
		[return: NullAllowed]
		string GetLocalizedCalendarIdentifier (string calendarIdentifier);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("languageIdentifier")]
		string LanguageIdentifier { get; }

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("regionCode")]
		string RegionCode { get; }
	}

	/// <param name="result">To be added.</param>
	/// <param name="flags">To be added.</param>
	/// <param name="stop">To be added.</param>
	/// <summary>Delegate applied to results in <see cref="Foundation.NSRegularExpression.EnumerateMatches(Foundation.NSString,Foundation.NSMatchingOptions,Foundation.NSRange,Foundation.NSMatchEnumerator)" />.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSMatchEnumerator (NSTextCheckingResult result, NSMatchingFlags flags, ref bool stop);

	// This API surfaces NSString instead of strings, because we already have the .NET version that uses
	// strings, so it makes sense to use NSString here (and also, the replacing functionality operates on
	// NSMutableStrings)
	[BaseType (typeof (NSObject))]
	interface NSRegularExpression : NSCopying, NSSecureCoding {
		[DesignatedInitializer]
		[Export ("initWithPattern:options:error:")]
		NativeHandle Constructor (NSString pattern, NSRegularExpressionOptions options, out NSError error);

		[Static]
		[Export ("regularExpressionWithPattern:options:error:")]
		NSRegularExpression Create (NSString pattern, NSRegularExpressionOptions options, out NSError error);

		[Export ("pattern")]
		NSString Pattern { get; }

		[Export ("options")]
		NSRegularExpressionOptions Options { get; }

		[Export ("numberOfCaptureGroups")]
		nuint NumberOfCaptureGroups { get; }

		[Export ("escapedPatternForString:")]
		[Static]
		NSString GetEscapedPattern (NSString str);

		/* From the NSMatching category */

		[Export ("enumerateMatchesInString:options:range:usingBlock:")]
		void EnumerateMatches (NSString str, NSMatchingOptions options, NSRange range, NSMatchEnumerator enumerator);

		[Export ("matchesInString:options:range:")]
		NSTextCheckingResult [] GetMatches (NSString str, NSMatchingOptions options, NSRange range);

		[Export ("numberOfMatchesInString:options:range:")]
		nuint GetNumberOfMatches (NSString str, NSMatchingOptions options, NSRange range);

		[Export ("firstMatchInString:options:range:")]
		[return: NullAllowed]
		NSTextCheckingResult FindFirstMatch (string str, NSMatchingOptions options, NSRange range);

		[Export ("rangeOfFirstMatchInString:options:range:")]
		NSRange GetRangeOfFirstMatch (string str, NSMatchingOptions options, NSRange range);

		/* From the NSReplacement category */

		[Export ("stringByReplacingMatchesInString:options:range:withTemplate:")]
		string ReplaceMatches (string sourceString, NSMatchingOptions options, NSRange range, string template);

		[Export ("replaceMatchesInString:options:range:withTemplate:")]
		nuint ReplaceMatches (NSMutableString mutableString, NSMatchingOptions options, NSRange range, NSString template);

		/// <param name="result">To be added.</param>
		/// <param name="str">To be added.</param>
		/// <param name="offset">To be added.</param>
		/// <param name="template">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("replacementStringForResult:inString:offset:template:")]
		NSString GetReplacementString (NSTextCheckingResult result, NSString str, nint offset, NSString template);

		[Static, Export ("escapedTemplateForString:")]
		NSString GetEscapedTemplate (NSString str);

	}

	[BaseType (typeof (NSObject))]
	// init returns NIL
	[DisableDefaultCtor]
	interface NSRunLoop {
		[Export ("currentRunLoop", ArgumentSemantic.Strong)]
		[Static]
		[IsThreadStatic]
		NSRunLoop Current { get; }

		[Export ("mainRunLoop", ArgumentSemantic.Strong)]
		[Static]
		NSRunLoop Main { get; }

		[Export ("currentMode")]
		NSString CurrentMode { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Wrap ("NSRunLoopModeExtensions.GetValue (CurrentMode)")]
		NSRunLoopMode CurrentRunLoopMode { get; }

		[Export ("getCFRunLoop")]
		CFRunLoop GetCFRunLoop ();

		[Export ("addTimer:forMode:")]
		void AddTimer (NSTimer timer, NSString forMode);

		/// <param name="timer">To be added.</param>
		///         <param name="forMode">The runloop to insert this into.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("AddTimer (timer, forMode.GetConstant ()!)")]
		void AddTimer (NSTimer timer, NSRunLoopMode forMode);

		[Export ("limitDateForMode:")]
		NSDate LimitDateForMode (NSString mode);

		/// <param name="mode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Wrap ("LimitDateForMode (mode.GetConstant ()!)")]
		NSDate LimitDateForMode (NSRunLoopMode mode);

		[Export ("acceptInputForMode:beforeDate:")]
		void AcceptInputForMode (NSString mode, NSDate limitDate);

		/// <param name="mode">To be added.</param>
		///         <param name="limitDate">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("AcceptInputForMode (mode.GetConstant ()!, limitDate)")]
		void AcceptInputForMode (NSRunLoopMode mode, NSDate limitDate);

		[Export ("run")]
		void Run ();

		[Export ("runUntilDate:")]
		void RunUntil (NSDate date);

		[Export ("runMode:beforeDate:")]
		bool RunUntil (NSString runLoopMode, NSDate limitdate);

		/// <param name="runLoopMode">To be added.</param>
		///         <param name="limitDate">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Wrap ("RunUntil (runLoopMode.GetConstant ()!, limitDate)")]
		bool RunUntil (NSRunLoopMode runLoopMode, NSDate limitDate);

		[MacCatalyst (13, 1)]
		[Export ("performBlock:")]
		void Perform (Action block);

		[MacCatalyst (13, 1)]
		[Export ("performInModes:block:")]
		void Perform (NSString [] modes, Action block);

		/// <param name="modes">To be added.</param>
		///         <param name="block">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("Perform (modes.GetConstants ()!, block)")]
		void Perform (NSRunLoopMode [] modes, Action block);
	}

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSSet : NSSecureCoding, NSMutableCopying {
		[Export ("set")]
		[Static]
		NSSet CreateSet ();

		[Export ("initWithSet:")]
		NativeHandle Constructor (NSSet other);

		[Export ("initWithArray:")]
		NativeHandle Constructor (NSArray other);

		[Export ("count")]
		nuint Count { get; }

		[Internal]
		[Sealed]
		[Export ("member:")]
		IntPtr _LookupMember (IntPtr probe);

		[Export ("member:")]
		NSObject LookupMember (NSObject probe);

		[Internal]
		[Sealed]
		[Export ("anyObject")]
		IntPtr _AnyObject { get; }

		[Export ("anyObject")]
		NSObject AnyObject { get; }

		[Internal]
		[Sealed]
		[Export ("containsObject:")]
		bool _Contains (NativeHandle id);

		[Export ("containsObject:")]
		bool Contains (NSObject id);

		[Export ("allObjects")]
		[Internal]
		IntPtr _AllObjects ();

		[Export ("isEqualToSet:")]
		bool IsEqualToSet (NSSet other);

		[Export ("objectEnumerator"), Internal]
		NSEnumerator _GetEnumerator ();

		[Export ("isSubsetOfSet:")]
		bool IsSubsetOf (NSSet other);

		[Export ("enumerateObjectsUsingBlock:")]
		void Enumerate (NSSetEnumerator enumerator);

		[Internal]
		[Sealed]
		[Export ("setByAddingObjectsFromSet:")]
		NativeHandle _SetByAddingObjectsFromSet (NativeHandle other);

		[Export ("setByAddingObjectsFromSet:"), Internal]
		NSSet SetByAddingObjectsFromSet (NSSet other);

		[Export ("intersectsSet:")]
		bool IntersectsSet (NSSet other);

		[Internal]
		[Static]
		[Export ("setWithArray:")]
		NativeHandle _SetWithArray (NativeHandle array);
	}

	interface NSSet<TKey> : NSSet { }

	[BaseType (typeof (NSObject))]
	interface NSSortDescriptor : NSSecureCoding, NSCopying {
		[Export ("initWithKey:ascending:")]
		NativeHandle Constructor (string key, bool ascending);

		[Export ("initWithKey:ascending:selector:")]
		NativeHandle Constructor (string key, bool ascending, [NullAllowed] Selector selector);

		[Export ("initWithKey:ascending:comparator:")]
		NativeHandle Constructor (string key, bool ascending, NSComparator comparator);

		[Export ("key")]
		string Key { get; }

		[Export ("ascending")]
		bool Ascending { get; }

		[NullAllowed]
		[Export ("selector")]
		Selector Selector { get; }

		[Export ("compareObject:toObject:")]
		NSComparisonResult Compare (NSObject object1, NSObject object2);

		[Export ("reversedSortDescriptor")]
		NSObject ReversedSortDescriptor { get; }

		[MacCatalyst (13, 1)]
		[Export ("allowEvaluation")]
		void AllowEvaluation ();
	}

	/// <summary>Defines an extension method for <see cref="Foundation.NSOrderedSet" /> objects, allowing sorting by <see cref="Foundation.NSSortDescriptor" /> objects.</summary>
	/// <remarks>To be added.</remarks>
	[Category, BaseType (typeof (NSOrderedSet))]
	partial interface NSKeyValueSorting_NSOrderedSet {
		/// <param name="sortDescriptors">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("sortedArrayUsingDescriptors:")]
		NSObject [] GetSortedArray (NSSortDescriptor [] sortDescriptors);
	}

#pragma warning disable 618
	/// <summary>Defines a static method for sorting <see cref="Foundation.NSMutableArray" /> objects using <see cref="Foundation.NSSortDescriptor" /> objects.</summary>
	/// <remarks>To be added.</remarks>
	[Category, BaseType (typeof (NSMutableArray))]
#pragma warning restore 618
	partial interface NSSortDescriptorSorting_NSMutableArray {
		/// <param name="sortDescriptors">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("sortUsingDescriptors:")]
		void SortUsingDescriptors (NSSortDescriptor [] sortDescriptors);
	}

	/// <summary>Defines an extension method for <see cref="NSMutableOrderedSet" /> objects, allowing them to be sorted using <see cref="Foundation.NSSortDescriptor" /> objects.</summary>
	/// <remarks>To be added.</remarks>
	[Category, BaseType (typeof (NSMutableOrderedSet))]
	partial interface NSKeyValueSorting_NSMutableOrderedSet {
		/// <param name="sortDescriptors">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("sortUsingDescriptors:")]
		void SortUsingDescriptors (NSSortDescriptor [] sortDescriptors);
	}

	[BaseType (typeof (NSObject))]
	[Dispose ("if (disposing) { Invalidate (); } ", Optimizable = true)]
	// init returns NIL
	[DisableDefaultCtor]
	interface NSTimer {

		[Static, Export ("scheduledTimerWithTimeInterval:target:selector:userInfo:repeats:")]
		NSTimer CreateScheduledTimer (double seconds, NSObject target, Selector selector, [NullAllowed] NSObject userInfo, bool repeats);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("scheduledTimerWithTimeInterval:repeats:block:")]
		NSTimer CreateScheduledTimer (double interval, bool repeats, Action<NSTimer> block);

		[Static, Export ("timerWithTimeInterval:target:selector:userInfo:repeats:")]
		NSTimer CreateTimer (double seconds, NSObject target, Selector selector, [NullAllowed] NSObject userInfo, bool repeats);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("timerWithTimeInterval:repeats:block:")]
		NSTimer CreateTimer (double interval, bool repeats, Action<NSTimer> block);

		[DesignatedInitializer]
		[Export ("initWithFireDate:interval:target:selector:userInfo:repeats:")]
		NativeHandle Constructor (NSDate date, double seconds, NSObject target, Selector selector, [NullAllowed] NSObject userInfo, bool repeats);

		[MacCatalyst (13, 1)]
		[Export ("initWithFireDate:interval:repeats:block:")]
		NativeHandle Constructor (NSDate date, double seconds, bool repeats, Action<NSTimer> block);

		[Export ("fire")]
		void Fire ();

		[NullAllowed] // by default this property is null
		[Export ("fireDate", ArgumentSemantic.Copy)]
		NSDate FireDate { get; set; }

		[Export ("invalidate")]
		void Invalidate ();

		/// <summary>Returns <see langword="true" /> if the the timer will still fire at some point in the future.</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Export ("isValid")]
		bool IsValid { get; }

		[Export ("timeInterval")]
		double TimeInterval { get; }

		[Export ("userInfo")]
		NSObject UserInfo { get; }

		[MacCatalyst (13, 1)]
		[Export ("tolerance")]
		double Tolerance { get; set; }
	}

	[BaseType (typeof (NSObject))]
	// NSTimeZone is an abstract class that defines the behavior of time zone objects. -> http://developer.apple.com/library/ios/#documentation/Cocoa/Reference/Foundation/Classes/NSTimeZone_Class/Reference/Reference.html
	// calling 'init' returns a NIL pointer, i.e. an unusable instance
	[DisableDefaultCtor]
	interface NSTimeZone : NSSecureCoding, NSCopying {
		[Export ("initWithName:")]
		NativeHandle Constructor (string name);

		[Export ("initWithName:data:")]
		NativeHandle Constructor (string name, NSData data);

		[Export ("name")]
		string Name { get; }

		[Export ("data")]
		NSData Data { get; }

		[Export ("secondsFromGMTForDate:")]
		nint SecondsFromGMT (NSDate date);

		[Static]
		[Export ("abbreviationDictionary")]
		NSDictionary Abbreviations { get; }

		[Export ("abbreviation")]
		string Abbreviation ();

		[Export ("abbreviationForDate:")]
		string Abbreviation (NSDate date);

		[Export ("isDaylightSavingTimeForDate:")]
		bool IsDaylightSavingsTime (NSDate date);

		[Export ("daylightSavingTimeOffsetForDate:")]
		double DaylightSavingTimeOffset (NSDate date);

		[Export ("nextDaylightSavingTimeTransitionAfterDate:")]
		NSDate NextDaylightSavingTimeTransitionAfter (NSDate date);

		[Static, Export ("timeZoneWithName:")]
		NSTimeZone FromName (string tzName);

		[Static, Export ("timeZoneWithName:data:")]
		NSTimeZone FromName (string tzName, NSData data);

		/// <param name="seconds">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("timeZoneForSecondsFromGMT:")]
		NSTimeZone FromGMT (nint seconds);

		[Static, Export ("localTimeZone", ArgumentSemantic.Copy)]
		NSTimeZone LocalTimeZone { get; }

		[Export ("secondsFromGMT")]
		nint GetSecondsFromGMT { get; }

		[Export ("defaultTimeZone", ArgumentSemantic.Copy), Static]
		NSTimeZone DefaultTimeZone { get; set; }

		[Export ("resetSystemTimeZone"), Static]
		void ResetSystemTimeZone ();

		[Export ("systemTimeZone", ArgumentSemantic.Copy), Static]
		NSTimeZone SystemTimeZone { get; }

		[Export ("timeZoneWithAbbreviation:"), Static]
		NSTimeZone FromAbbreviation (string abbreviation);

		[Export ("knownTimeZoneNames"), Static, Internal]
		string [] _KnownTimeZoneNames { get; }

		[Export ("timeZoneDataVersion"), Static]
		string DataVersion { get; }

		[Export ("localizedName:locale:")]
		string GetLocalizedName (NSTimeZoneNameStyle style, [NullAllowed] NSLocale locale);

		[Notification]
		[Field ("NSSystemTimeZoneDidChangeNotification")]
		NSString SystemTimeZoneDidChangeNotification { get; }
	}

	interface NSUbiquitousKeyValueStoreChangeEventArgs {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("NSUbiquitousKeyValueStoreChangedKeysKey")]
		string [] ChangedKeys { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("NSUbiquitousKeyValueStoreChangeReasonKey")]
		NSUbiquitousKeyValueStoreChangeReason ChangeReason { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSUbiquitousKeyValueStore {
		[Static]
		[Export ("defaultStore")]
		NSUbiquitousKeyValueStore DefaultStore { get; }

		[return: NullAllowed]
		[Export ("objectForKey:"), Internal]
		NSObject ObjectForKey (string aKey);

		[Export ("setObject:forKey:"), Internal]
		void SetObjectForKey ([NullAllowed] NSObject anObject, string aKey);

		[Export ("removeObjectForKey:")]
		void Remove (string aKey);

		[return: NullAllowed]
		[Export ("stringForKey:")]
		string GetString (string aKey);

		[return: NullAllowed]
		[Export ("arrayForKey:")]
		NSObject [] GetArray (string aKey);

		[return: NullAllowed]
		[Export ("dictionaryForKey:")]
		NSDictionary GetDictionary (string aKey);

		[return: NullAllowed]
		[Export ("dataForKey:")]
		NSData GetData (string aKey);

		[Export ("longLongForKey:")]
		long GetLong (string aKey);

		[Export ("doubleForKey:")]
		double GetDouble (string aKey);

		[Export ("boolForKey:")]
		bool GetBool (string aKey);

		[Export ("setString:forKey:"), Internal]
		void _SetString ([NullAllowed] string aString, string aKey);

		[Export ("setData:forKey:"), Internal]
		void _SetData ([NullAllowed] NSData data, string key);

		[Export ("setArray:forKey:"), Internal]
		void _SetArray ([NullAllowed] NSObject [] array, string key);

		[Export ("setDictionary:forKey:"), Internal]
		void _SetDictionary ([NullAllowed] NSDictionary aDictionary, string aKey);

		[Export ("setLongLong:forKey:"), Internal]
		void _SetLong (long value, string aKey);

		[Export ("setDouble:forKey:"), Internal]
		void _SetDouble (double value, string aKey);

		[Export ("setBool:forKey:"), Internal]
		void _SetBool (bool value, string aKey);

		[Export ("dictionaryRepresentation")]
		NSDictionary ToDictionary ();

		[Export ("synchronize")]
		bool Synchronize ();

		[Field ("NSUbiquitousKeyValueStoreDidChangeExternallyNotification")]
		[Notification (typeof (NSUbiquitousKeyValueStoreChangeEventArgs))]
		NSString DidChangeExternallyNotification { get; }

		/// <summary>Represents the value associated with the constant NSUbiquitousKeyValueStoreChangeReasonKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSUbiquitousKeyValueStoreChangeReasonKey")]
		NSString ChangeReasonKey { get; }

		/// <summary>Represents the value associated with the constant NSUbiquitousKeyValueStoreChangedKeysKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSUbiquitousKeyValueStoreChangedKeysKey")]
		NSString ChangedKeysKey { get; }
	}

	[BaseType (typeof (NSObject), Name = "NSUUID")]
	[DesignatedDefaultCtor]
	interface NSUuid : NSSecureCoding, NSCopying {
		[Export ("initWithUUIDString:")]
		NativeHandle Constructor (string str);

		// bound manually to keep the managed/native signatures identical
		//[Export ("initWithUUIDBytes:"), Internal]
		//NativeHandle Constructor (IntPtr bytes, bool unused);

		[Export ("getUUIDBytes:"), Internal]
		void GetUuidBytes (IntPtr uuid);

		[Export ("UUIDString")]
		string AsString ();

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("compare:")]
		NSComparisonResult Compare (NSUuid otherUuid);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // xcode 8 beta 4 marks it as API_DEPRECATED
	partial interface NSUserActivity
#if IOS // iOS only.
	: NSItemProviderReading, NSItemProviderWriting
#endif
	{
		[DesignatedInitializer]
		[Export ("initWithActivityType:")]
		NativeHandle Constructor (string activityType);

		[Export ("activityType")]
		string ActivityType { get; }

		[NullAllowed] // by default this property is null
		[Export ("title")]
		string Title { get; set; }

		[Export ("userInfo", ArgumentSemantic.Copy), NullAllowed]
		NSDictionary UserInfo { get; set; }

		[Export ("needsSave")]
		bool NeedsSave { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("webpageURL", ArgumentSemantic.Copy)]
		NSUrl WebPageUrl { get; set; }

		[Export ("supportsContinuationStreams")]
		bool SupportsContinuationStreams { get; set; }

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the Foundation.INSUserActivityDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSUserActivityDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		INSUserActivityDelegate Delegate { get; set; }

		[Export ("addUserInfoEntriesFromDictionary:")]
		void AddUserInfoEntries (NSDictionary otherDictionary);

		[Export ("becomeCurrent")]
		void BecomeCurrent ();

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("getContinuationStreamsWithCompletionHandler:")]
		[Async (ResultTypeName = "NSUserActivityContinuation", XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous GetContinuationStreams operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSInputStream,Foundation.NSOutputStream,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The GetContinuationStreamsAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void GetContinuationStreams (Action<NSInputStream, NSOutputStream, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("requiredUserInfoKeys", ArgumentSemantic.Copy)]
		NSSet<NSString> RequiredUserInfoKeys { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("keywords", ArgumentSemantic.Copy)]
		NSSet<NSString> Keywords { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("resignCurrent")]
		void ResignCurrent ();

		/// <summary>Gets or sets whether this NSUserActivity is eligible for handoff.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("eligibleForHandoff")]
		bool EligibleForHandoff { [Bind ("isEligibleForHandoff")] get; set; }

		/// <summary>Gets or sets whether this NSUserActivity is eligible for search.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("eligibleForSearch")]
		bool EligibleForSearch { [Bind ("isEligibleForSearch")] get; set; }

		/// <summary>Gets or sets whether this NSUserActivity may have entries in public indices.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("eligibleForPublicIndexing")]
		bool EligibleForPublicIndexing { [Bind ("isEligibleForPublicIndexing")] get; set; }

		/// <summary>Gets or sets the list of searchable properties for this activity.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("contentAttributeSet", ArgumentSemantic.Copy)] // From CSSearchableItemAttributeSet.h
		CSSearchableItemAttributeSet ContentAttributeSet { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("referrerURL", ArgumentSemantic.Copy)]
		NSUrl ReferrerUrl { get; set; }

		// From NSUserActivity (CIBarcodeDescriptor)

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("detectedBarcodeDescriptor", ArgumentSemantic.Copy)]
		CIBarcodeDescriptor DetectedBarcodeDescriptor { get; }

		// From NSUserActivity (CLSDeepLinks)

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[NoTV]
		[Export ("isClassKitDeepLink")]
		bool IsClassKitDeepLink { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[NoTV]
		[NullAllowed, Export ("contextIdentifierPath", ArgumentSemantic.Strong)]
		string [] ContextIdentifierPath { get; }

		// From NSUserActivity (IntentsAdditions)

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("suggestedInvocationPhrase")]
		string SuggestedInvocationPhrase {
			// This _simply_ ensure that the Intents namespace (via the enum) will be present which,
			// in turns, means that the Intents.framework is loaded into memory and this makes the
			// selectors (getter and setter) work at runtime. Other selectors do not need it.
			// reference: https://github.com/dotnet/macios/issues/4894
			[PreSnippet ("GC.KeepAlive (Intents.INCallCapabilityOptions.AudioCall); // no-op to ensure Intents.framework is loaded into memory", Optimizable = true)]
			get;
			[PreSnippet ("GC.KeepAlive (Intents.INCallCapabilityOptions.AudioCall); // no-op to ensure Intents.framework is loaded into memory", Optimizable = true)]
			set;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("eligibleForPrediction")]
		bool EligibleForPrediction { [Bind ("isEligibleForPrediction")] get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("persistentIdentifier")]
		string PersistentIdentifier { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Async (XmlDocs = """
			<param name="persistentIdentifiers">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("deleteSavedUserActivitiesWithPersistentIdentifiers:completionHandler:")]
		void DeleteSavedUserActivities (string [] persistentIdentifiers, Action handler);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("deleteAllSavedUserActivitiesWithCompletionHandler:")]
		void DeleteAllSavedUserActivities (Action handler);

		// Inlined from NSUserActivity (UISceneActivationConditions)

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("targetContentIdentifier")]
		string TargetContentIdentifier { get; set; }

#if HAS_APPCLIP
		// Inlined from NSUserActivity (AppClip)
		[iOS (14, 0)]
		[NoTV]
		[NoMac]
		[MacCatalyst (14, 0)]
		[Export ("appClipActivationPayload", ArgumentSemantic.Strong)]
		[NullAllowed]
		APActivationPayload AppClipActivationPayload { get; }
#endif
	}

	/// <summary>Defines types of <see cref="Foundation.NSUserActivity" /> available from the system (currently only browsing the Web).</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Static]
	partial interface NSUserActivityType {
		/// <summary>Represents the value associated with the constant NSUserActivityTypeBrowsingWeb</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSUserActivityTypeBrowsingWeb")]
		NSString BrowsingWeb { get; }
	}

	interface INSUserActivityDelegate { }

	/// <summary>Delegate object for <see cref="Foundation.NSUserActivity" /> objects, exposing events relating to an activity begun on one device and continued on another.</summary>
	/// <remarks>To be added.</remarks>
	/// <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AppKit/Reference/NSUserActivityDelegate_Protocol/index.html">Apple documentation for <c>NSUserActivityDelegate</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface NSUserActivityDelegate {
		/// <param name="userActivity">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("userActivityWillSave:")]
		void UserActivityWillSave (NSUserActivity userActivity);

		/// <param name="userActivity">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("userActivityWasContinued:")]
		void UserActivityWasContinued (NSUserActivity userActivity);

		/// <param name="userActivity">To be added.</param>
		/// <param name="inputStream">To be added.</param>
		/// <param name="outputStream">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("userActivity:didReceiveInputStream:outputStream:")]
		void UserActivityReceivedData (NSUserActivity userActivity, NSInputStream inputStream, NSOutputStream outputStream);
	}

	[BaseType (typeof (NSObject))]
	interface NSUserDefaults {
		[Export ("URLForKey:")]
		[return: NullAllowed]
		NSUrl URLForKey (string defaultName);

		[Export ("setURL:forKey:")]
		void SetURL ([NullAllowed] NSUrl url, string defaultName);

		[Static]
		[Export ("standardUserDefaults", ArgumentSemantic.Strong)]
		NSUserDefaults StandardUserDefaults { get; }

		[Static]
		[Export ("resetStandardUserDefaults")]
		void ResetStandardUserDefaults ();

		[Internal]
		[Export ("initWithUser:")]
		IntPtr _InitWithUserName (string username);

		[Internal]
		[MacCatalyst (13, 1)]
		[Export ("initWithSuiteName:")]
		IntPtr _InitWithSuiteName ([NullAllowed] string suiteName);

		[Export ("objectForKey:")]
		[Internal]
		[return: NullAllowed]
		NSObject ObjectForKey (string defaultName);

		[Export ("setObject:forKey:")]
		[Internal]
		void SetObjectForKey ([NullAllowed] NSObject value, string defaultName);

		[Export ("removeObjectForKey:")]
		void RemoveObject (string defaultName);

		[return: NullAllowed]
		[Export ("stringForKey:")]
		string StringForKey (string defaultName);

		[return: NullAllowed]
		[Export ("arrayForKey:")]
		NSObject [] ArrayForKey (string defaultName);

		[return: NullAllowed]
		[Export ("dictionaryForKey:")]
		NSDictionary DictionaryForKey (string defaultName);

		[return: NullAllowed]
		[Export ("dataForKey:")]
		NSData DataForKey (string defaultName);

		[return: NullAllowed]
		[Export ("stringArrayForKey:")]
		string [] StringArrayForKey (string defaultName);

		[Export ("integerForKey:")]
		nint IntForKey (string defaultName);

		[Export ("floatForKey:")]
		float FloatForKey (string defaultName); // this is defined as float, not CGFloat.

		[Export ("doubleForKey:")]
		double DoubleForKey (string defaultName);

		[Export ("boolForKey:")]
		bool BoolForKey (string defaultName);

		/// <param name="value">To be added.</param>
		/// <param name="defaultName">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("setInteger:forKey:")]
		void SetInt (nint value, string defaultName);

		[Export ("setFloat:forKey:")]
		void SetFloat (float value /* this is defined as float, not CGFloat */, string defaultName);

		[Export ("setDouble:forKey:")]
		void SetDouble (double value, string defaultName);

		[Export ("setBool:forKey:")]
		void SetBool (bool value, string defaultName);

		[Export ("registerDefaults:")]
		void RegisterDefaults (NSDictionary registrationDictionary);

		[Export ("addSuiteNamed:")]
		void AddSuite (string suiteName);

		[Export ("removeSuiteNamed:")]
		void RemoveSuite (string suiteName);

		[Export ("dictionaryRepresentation")]
		NSDictionary ToDictionary ();

		[Export ("volatileDomainNames")]
#if XAMCORE_5_0
		string [] VolatileDomainNames { get; }
#else
		string [] VolatileDomainNames ();
#endif

		[Export ("volatileDomainForName:")]
		NSDictionary GetVolatileDomain (string domainName);

		[Export ("setVolatileDomain:forName:")]
		void SetVolatileDomain (NSDictionary domain, string domainName);

		[Export ("removeVolatileDomainForName:")]
		void RemoveVolatileDomain (string domainName);

		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("persistentDomainNames")]
		string [] PersistentDomainNames ();

		[return: NullAllowed]
		[Export ("persistentDomainForName:")]
		NSDictionary PersistentDomainForName (string domainName);

		[Export ("setPersistentDomain:forName:")]
		void SetPersistentDomain (NSDictionary domain, string domainName);

		[Export ("removePersistentDomainForName:")]
		void RemovePersistentDomain (string domainName);

		[Export ("synchronize")]
		bool Synchronize ();

		[Export ("objectIsForcedForKey:")]
		bool ObjectIsForced (string key);

		[Export ("objectIsForcedForKey:inDomain:")]
		bool ObjectIsForced (string key, string domain);

		/// <summary>This is the key used to retrieve the global user defaults domain.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This key is used to retrieve the global user defaults.
		/// 	  </para>
		///           <example>
		///             <code lang="c#">
		/// // Retrieve the global NSButtonDelay setting on macOS:
		/// var global = new NSUserDefaults (NSUserDefaults.GlobalDomain);
		/// Console.WriteLine ("Delay: " + j.FloatForKey ("NSButtonDelay"));
		/// 	    </code>
		///           </example>
		///         </remarks>
		[Field ("NSGlobalDomain")]
		NSString GlobalDomain { get; }

		/// <summary>This is the key used to retrieve the domain associated with the command line arguments passed at startup.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///           <para>
		/// 	    This domain contains the command line arguments that were
		/// 	    parsed at application startup.
		///
		/// 	  </para>
		///           <para>
		/// 	    For each command line argument of the form -NAME VALUE
		/// 	    that is passed at startup to your application, the "NAME"
		/// 	    is used as the key, with the value set to VALUE.  
		///
		/// 	  </para>
		///           <example>
		///             <code lang="c#">
		/// // Retrieve the global NSButtonDelay setting on macOS:
		/// var global = new NSUserDefaults (NSUserDefaults.GlobalDomain);
		/// Console.WriteLine ("Delay: " + j.FloatForKey ("NSButtonDelay"));
		/// 	    </code>
		///           </example>
		///         </remarks>
		[Field ("NSArgumentDomain")]
		NSString ArgumentDomain { get; }

		/// <summary>Represents the value associated with the constant NSRegistrationDomain</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSRegistrationDomain")]
		NSString RegistrationDomain { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSUserDefaultsSizeLimitExceededNotification")]
		NSString SizeLimitExceededNotification { get; }

		[Deprecated (PlatformName.iOS, 9, 3, "This notification is never posted.")]
		[Deprecated (PlatformName.TvOS, 9, 3, "This notification is never posted.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, "This notification is never posted.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSUbiquitousUserDefaultsNoCloudAccountNotification")]
		NSString NoCloudAccountNotification { get; }

		[Deprecated (PlatformName.iOS, 9, 3, "This notification is never posted.")]
		[Deprecated (PlatformName.TvOS, 9, 3, "This notification is never posted.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, "This notification is never posted.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSUbiquitousUserDefaultsDidChangeAccountsNotification")]
		NSString DidChangeAccountsNotification { get; }

		[Deprecated (PlatformName.iOS, 9, 3, "This notification is never posted.")]
		[Deprecated (PlatformName.TvOS, 9, 3, "This notification is never posted.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, "This notification is never posted.")]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSUbiquitousUserDefaultsCompletedInitialSyncNotification")]
		NSString CompletedInitialSyncNotification { get; }

		[Notification]
		[Field ("NSUserDefaultsDidChangeNotification")]
		NSString DidChangeNotification { get; }
	}

	[BaseType (typeof (NSObject), Name = "NSURL")]
	// init returns NIL
	[DisableDefaultCtor]
	partial interface NSUrl : NSSecureCoding, NSCopying
#if MONOMAC
	, NSPasteboardReading, NSPasteboardWriting
#endif
	, NSItemProviderWriting, NSItemProviderReading
#if IOS || MONOMAC
	, QLPreviewItem
#endif
	{
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSUrlComponents' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSUrlComponents' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSUrlComponents' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSUrlComponents' instead.")]
		[Export ("initWithScheme:host:path:")]
		NativeHandle Constructor (string scheme, string host, string path);

		[DesignatedInitializer]
		[Export ("initFileURLWithPath:isDirectory:")]
		NativeHandle Constructor (string path, bool isDir);

		[Export ("initWithString:")]
		NativeHandle Constructor (string urlString);

		[DesignatedInitializer]
		[Export ("initWithString:relativeToURL:")]
		NativeHandle Constructor (string urlString, NSUrl relativeToUrl);

		[return: NullAllowed]
		[Export ("URLWithString:")]
		[Static]
		NSUrl FromString ([NullAllowed] string s);

		[Export ("URLWithString:relativeToURL:")]
		[Internal]
		[Static]
		NSUrl _FromStringRelative (string url, NSUrl relative);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("URLWithString:encodingInvalidCharacters:")]
		[return: NullAllowed]
		NSUrl FromString (string url, bool encodingInvalidCharacters);

		[Export ("absoluteString")]
		[NullAllowed]
		string AbsoluteString { get; }

		[Export ("absoluteURL")]
		[NullAllowed]
		NSUrl AbsoluteUrl { get; }

		[Export ("baseURL")]
		[NullAllowed]
		NSUrl BaseUrl { get; }

		[Export ("fragment")]
		[NullAllowed]
		string Fragment { get; }

		[Export ("host")]
		[NullAllowed]
		string Host { get; }

		[Internal]
		[Export ("isEqual:")]
		bool IsEqual ([NullAllowed] NSUrl other);

		/// <summary>Whether this NSUrl uses the file scheme.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isFileURL")]
		bool IsFileUrl { get; }

		[Export ("isFileReferenceURL")]
		bool IsFileReferenceUrl { get; }

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Always return 'null'. Use and parse 'Path' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Always return 'null'. Use and parse 'Path' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Always return 'null'. Use and parse 'Path' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Always return 'null'. Use and parse 'Path' instead.")]
		[Export ("parameterString")]
		[NullAllowed]
		string ParameterString { get; }

		[Export ("password")]
		[NullAllowed]
		string Password { get; }

		[Export ("path")]
		[NullAllowed]
		string Path { get; }

		[Export ("query")]
		[NullAllowed]
		string Query { get; }

		[Export ("relativePath")]
		[NullAllowed]
		string RelativePath { get; }

		[Export ("pathComponents")]
		[NullAllowed]
		string [] PathComponents { get; }

		[Export ("lastPathComponent")]
		[NullAllowed]
		string LastPathComponent { get; }

		[Export ("pathExtension")]
		[NullAllowed]
		string PathExtension { get; }

		[Export ("relativeString")]
		string RelativeString { get; }

		[Export ("resourceSpecifier")]
		[NullAllowed]
		string ResourceSpecifier { get; }

		[Export ("scheme")]
		[NullAllowed]
		string Scheme { get; }

		[Export ("user")]
		[NullAllowed]
		string User { get; }

		[Export ("standardizedURL")]
		[NullAllowed]
		NSUrl StandardizedUrl { get; }

		[Export ("URLByAppendingPathComponent:isDirectory:")]
		NSUrl Append (string pathComponent, bool isDirectory);

		[Export ("URLByAppendingPathExtension:")]
		NSUrl AppendPathExtension (string extension);

		[Export ("URLByDeletingLastPathComponent")]
		NSUrl RemoveLastPathComponent ();

		[Export ("URLByDeletingPathExtension")]
		NSUrl RemovePathExtension ();

		[MacCatalyst (13, 1)]
		[Export ("getFileSystemRepresentation:maxLength:")]
		bool GetFileSystemRepresentation (IntPtr buffer, nint maxBufferLength);

		[MacCatalyst (13, 1)]
		[Export ("fileSystemRepresentation")]
		IntPtr GetFileSystemRepresentationAsUtf8Ptr { get; }

		[MacCatalyst (13, 1)]
		[Export ("removeCachedResourceValueForKey:")]
		void RemoveCachedResourceValueForKey (NSString key);

		[MacCatalyst (13, 1)]
		[Export ("removeAllCachedResourceValues")]
		void RemoveAllCachedResourceValues ();

		[MacCatalyst (13, 1)]
		[Export ("setTemporaryResourceValue:forKey:")]
		void SetTemporaryResourceValue (NSObject value, NSString key);

		[DesignatedInitializer]
		[MacCatalyst (13, 1)]
		[Export ("initFileURLWithFileSystemRepresentation:isDirectory:relativeToURL:")]
		NativeHandle Constructor (IntPtr ptrUtf8path, bool isDir, [NullAllowed] NSUrl baseURL);

		[Static, Export ("fileURLWithFileSystemRepresentation:isDirectory:relativeToURL:")]
		[MacCatalyst (13, 1)]
		NSUrl FromUTF8Pointer (IntPtr ptrUtf8path, bool isDir, [NullAllowed] NSUrl baseURL);

		/* These methods come from NURL_AppKitAdditions */
		/// <param name="pasteboard">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("URLFromPasteboard:")]
		[Static]
		[return: NullAllowed]
		NSUrl FromPasteboard (NSPasteboard pasteboard);

		/// <param name="pasteboard">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("writeToPasteboard:")]
		void WriteToPasteboard (NSPasteboard pasteboard);

		[Export ("bookmarkDataWithContentsOfURL:error:")]
		[Static]
		NSData GetBookmarkData (NSUrl bookmarkFileUrl, out NSError error);

		[Export ("URLByResolvingBookmarkData:options:relativeToURL:bookmarkDataIsStale:error:")]
		[Static]
		NSUrl FromBookmarkData (NSData data, NSUrlBookmarkResolutionOptions options, [NullAllowed] NSUrl relativeToUrl, out bool isStale, out NSError error);

		[Export ("writeBookmarkData:toURL:options:error:")]
		[Static]
		bool WriteBookmarkData (NSData data, NSUrl bookmarkFileUrl, NSUrlBookmarkCreationOptions options, out NSError error);

		[Export ("filePathURL")]
		[NullAllowed]
		NSUrl FilePathUrl { get; }

		[Export ("fileReferenceURL")]
		[NullAllowed]
		NSUrl FileReferenceUrl { get; }

		[Export ("getResourceValue:forKey:error:"), Internal]
		bool GetResourceValue (out NSObject value, NSString key, out NSError error);

		[Export ("resourceValuesForKeys:error:")]
		NSDictionary GetResourceValues (NSString [] keys, out NSError error);

		[Export ("setResourceValue:forKey:error:"), Internal]
		bool SetResourceValue (NSObject value, NSString key, out NSError error);

		[Export ("port"), Internal]
		[NullAllowed]
		NSNumber PortNumber { get; }

		/// <summary>Represents the value associated with the constant NSURLNameKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLNameKey")]
		NSString NameKey { get; }

		/// <summary>Represents the value associated with the constant NSURLLocalizedNameKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLLocalizedNameKey")]
		NSString LocalizedNameKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsRegularFileKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsRegularFileKey")]
		NSString IsRegularFileKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsDirectoryKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsDirectoryKey")]
		NSString IsDirectoryKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsSymbolicLinkKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsSymbolicLinkKey")]
		NSString IsSymbolicLinkKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsVolumeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsVolumeKey")]
		NSString IsVolumeKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsPackageKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsPackageKey")]
		NSString IsPackageKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsSystemImmutableKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsSystemImmutableKey")]
		NSString IsSystemImmutableKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsUserImmutableKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsUserImmutableKey")]
		NSString IsUserImmutableKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsHiddenKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsHiddenKey")]
		NSString IsHiddenKey { get; }

		/// <summary>Represents the value associated with the constant NSURLHasHiddenExtensionKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLHasHiddenExtensionKey")]
		NSString HasHiddenExtensionKey { get; }

		/// <summary>Represents the value associated with the constant NSURLCreationDateKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLCreationDateKey")]
		NSString CreationDateKey { get; }

		/// <summary>Represents the value associated with the constant NSURLContentAccessDateKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLContentAccessDateKey")]
		NSString ContentAccessDateKey { get; }

		/// <summary>Represents the value associated with the constant NSURLContentModificationDateKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLContentModificationDateKey")]
		NSString ContentModificationDateKey { get; }

		/// <summary>Represents the value associated with the constant NSURLAttributeModificationDateKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLAttributeModificationDateKey")]
		NSString AttributeModificationDateKey { get; }

		/// <summary>Represents the value associated with the constant NSURLLinkCountKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLLinkCountKey")]
		NSString LinkCountKey { get; }

		/// <summary>Represents the value associated with the constant NSURLParentDirectoryURLKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLParentDirectoryURLKey")]
		NSString ParentDirectoryURLKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeURLKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeURLKey")]
		NSString VolumeURLKey { get; }

		/// <summary>Represents the value associated with the constant NSURLTypeIdentifierKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'ContentTypeKey' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'ContentTypeKey' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'ContentTypeKey' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'ContentTypeKey' instead.")]
		[Field ("NSURLTypeIdentifierKey")]
		NSString TypeIdentifierKey { get; }

		/// <summary>Represents the value associated with the constant NSURLLocalizedTypeDescriptionKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLLocalizedTypeDescriptionKey")]
		NSString LocalizedTypeDescriptionKey { get; }

		/// <summary>Represents the value associated with the constant NSURLLabelNumberKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLLabelNumberKey")]
		NSString LabelNumberKey { get; }

		/// <summary>Represents the value associated with the constant NSURLLabelColorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLLabelColorKey")]
		NSString LabelColorKey { get; }

		/// <summary>Represents the value associated with the constant NSURLLocalizedLabelKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLLocalizedLabelKey")]
		NSString LocalizedLabelKey { get; }

		/// <summary>Represents the value associated with the constant NSURLEffectiveIconKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLEffectiveIconKey")]
		NSString EffectiveIconKey { get; }

		/// <summary>Represents the value associated with the constant NSURLCustomIconKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLCustomIconKey")]
		NSString CustomIconKey { get; }

		/// <summary>Represents the value associated with the constant NSURLFileSizeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileSizeKey")]
		NSString FileSizeKey { get; }

		/// <summary>Represents the value associated with the constant NSURLFileAllocatedSizeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileAllocatedSizeKey")]
		NSString FileAllocatedSizeKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsAliasFileKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsAliasFileKey")]
		NSString IsAliasFileKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeLocalizedFormatDescriptionKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeLocalizedFormatDescriptionKey")]
		NSString VolumeLocalizedFormatDescriptionKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeTotalCapacityKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeTotalCapacityKey")]
		NSString VolumeTotalCapacityKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeAvailableCapacityKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeAvailableCapacityKey")]
		NSString VolumeAvailableCapacityKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeResourceCountKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeResourceCountKey")]
		NSString VolumeResourceCountKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsPersistentIDsKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsPersistentIDsKey")]
		NSString VolumeSupportsPersistentIDsKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsSymbolicLinksKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsSymbolicLinksKey")]
		NSString VolumeSupportsSymbolicLinksKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsHardLinksKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsHardLinksKey")]
		NSString VolumeSupportsHardLinksKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsJournalingKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsJournalingKey")]
		NSString VolumeSupportsJournalingKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeIsJournalingKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeIsJournalingKey")]
		NSString VolumeIsJournalingKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsSparseFilesKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsSparseFilesKey")]
		NSString VolumeSupportsSparseFilesKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsZeroRunsKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsZeroRunsKey")]
		NSString VolumeSupportsZeroRunsKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsCaseSensitiveNamesKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsCaseSensitiveNamesKey")]
		NSString VolumeSupportsCaseSensitiveNamesKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsCasePreservedNamesKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsCasePreservedNamesKey")]
		NSString VolumeSupportsCasePreservedNamesKey { get; }

		// 5.0 Additions
		/// <summary>Represents the value associated with the constant NSURLKeysOfUnsetValuesKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLKeysOfUnsetValuesKey")]
		NSString KeysOfUnsetValuesKey { get; }

		/// <summary>Represents the value associated with the constant NSURLFileResourceIdentifierKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileResourceIdentifierKey")]
		NSString FileResourceIdentifierKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeIdentifierKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeIdentifierKey")]
		NSString VolumeIdentifierKey { get; }

		/// <summary>Represents the value associated with the constant NSURLPreferredIOBlockSizeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLPreferredIOBlockSizeKey")]
		NSString PreferredIOBlockSizeKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsReadableKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsReadableKey")]
		NSString IsReadableKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsWritableKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsWritableKey")]
		NSString IsWritableKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsExecutableKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsExecutableKey")]
		NSString IsExecutableKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsMountTriggerKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsMountTriggerKey")]
		NSString IsMountTriggerKey { get; }

		/// <summary>Represents the value associated with the constant NSURLFileSecurityKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileSecurityKey")]
		NSString FileSecurityKey { get; }

		/// <summary>Represents the value associated with the constant NSURLFileResourceTypeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileResourceTypeKey")]
		NSString FileResourceTypeKey { get; }

		[TV (16, 4), iOS (16, 4)]
		[MacCatalyst (16, 4)]
		[Field ("NSURLFileIdentifierKey")]
		NSString FileIdentifierKey { get; }

		/// <summary>Represents the value associated with the constant NSURLFileResourceTypeNamedPipe</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileResourceTypeNamedPipe")]
		NSString FileResourceTypeNamedPipe { get; }

		/// <summary>Represents the value associated with the constant NSURLFileResourceTypeCharacterSpecial</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileResourceTypeCharacterSpecial")]
		NSString FileResourceTypeCharacterSpecial { get; }

		/// <summary>Represents the value associated with the constant NSURLFileResourceTypeDirectory</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileResourceTypeDirectory")]
		NSString FileResourceTypeDirectory { get; }

		/// <summary>Represents the value associated with the constant NSURLFileResourceTypeBlockSpecial</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileResourceTypeBlockSpecial")]
		NSString FileResourceTypeBlockSpecial { get; }

		/// <summary>Represents the value associated with the constant NSURLFileResourceTypeRegular</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileResourceTypeRegular")]
		NSString FileResourceTypeRegular { get; }

		/// <summary>Represents the value associated with the constant NSURLFileResourceTypeSymbolicLink</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileResourceTypeSymbolicLink")]
		NSString FileResourceTypeSymbolicLink { get; }

		/// <summary>Represents the value associated with the constant NSURLFileResourceTypeSocket</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileResourceTypeSocket")]
		NSString FileResourceTypeSocket { get; }

		/// <summary>Represents the value associated with the constant NSURLFileResourceTypeUnknown</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLFileResourceTypeUnknown")]
		NSString FileResourceTypeUnknown { get; }

		/// <summary>Represents the value associated with the constant NSURLTotalFileSizeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLTotalFileSizeKey")]
		NSString TotalFileSizeKey { get; }

		/// <summary>Represents the value associated with the constant NSURLTotalFileAllocatedSizeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLTotalFileAllocatedSizeKey")]
		NSString TotalFileAllocatedSizeKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsRootDirectoryDatesKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsRootDirectoryDatesKey")]
		NSString VolumeSupportsRootDirectoryDatesKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsVolumeSizesKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsVolumeSizesKey")]
		NSString VolumeSupportsVolumeSizesKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsRenamingKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsRenamingKey")]
		NSString VolumeSupportsRenamingKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsAdvisoryFileLockingKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsAdvisoryFileLockingKey")]
		NSString VolumeSupportsAdvisoryFileLockingKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeSupportsExtendedSecurityKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeSupportsExtendedSecurityKey")]
		NSString VolumeSupportsExtendedSecurityKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeIsBrowsableKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeIsBrowsableKey")]
		NSString VolumeIsBrowsableKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeMaximumFileSizeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeMaximumFileSizeKey")]
		NSString VolumeMaximumFileSizeKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeIsEjectableKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeIsEjectableKey")]
		NSString VolumeIsEjectableKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeIsRemovableKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeIsRemovableKey")]
		NSString VolumeIsRemovableKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeIsInternalKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeIsInternalKey")]
		NSString VolumeIsInternalKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeIsAutomountedKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeIsAutomountedKey")]
		NSString VolumeIsAutomountedKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeIsLocalKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeIsLocalKey")]
		NSString VolumeIsLocalKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeIsReadOnlyKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeIsReadOnlyKey")]
		NSString VolumeIsReadOnlyKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeCreationDateKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeCreationDateKey")]
		NSString VolumeCreationDateKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeURLForRemountingKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeURLForRemountingKey")]
		NSString VolumeURLForRemountingKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeUUIDStringKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeUUIDStringKey")]
		NSString VolumeUUIDStringKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeNameKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeNameKey")]
		NSString VolumeNameKey { get; }

		/// <summary>Represents the value associated with the constant NSURLVolumeLocalizedNameKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLVolumeLocalizedNameKey")]
		NSString VolumeLocalizedNameKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeIsEncryptedKey")]
		NSString VolumeIsEncryptedKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeIsRootFileSystemKey")]
		NSString VolumeIsRootFileSystemKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsCompressionKey")]
		NSString VolumeSupportsCompressionKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsFileCloningKey")]
		NSString VolumeSupportsFileCloningKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsSwapRenamingKey")]
		NSString VolumeSupportsSwapRenamingKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsExclusiveRenamingKey")]
		NSString VolumeSupportsExclusiveRenamingKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsImmutableFilesKey")]
		NSString VolumeSupportsImmutableFilesKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsAccessPermissionsKey")]
		NSString VolumeSupportsAccessPermissionsKey { get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLVolumeSupportsFileProtectionKey")]
		NSString VolumeSupportsFileProtectionKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeAvailableCapacityForImportantUsageKey")]
		NSString VolumeAvailableCapacityForImportantUsageKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeAvailableCapacityForOpportunisticUsageKey")]
		NSString VolumeAvailableCapacityForOpportunisticUsageKey { get; }

		[TV (16, 4), iOS (16, 4)]
		[MacCatalyst (16, 4)]
		[Field ("NSURLVolumeTypeNameKey")]
		NSString VolumeTypeNameKey { get; }

		[TV (16, 4), iOS (16, 4)]
		[MacCatalyst (16, 4)]
		[Field ("NSURLVolumeSubtypeKey")]
		NSString VolumeSubtypeKey { get; }

		[TV (16, 4), iOS (16, 4)]
		[MacCatalyst (16, 4)]
		[Field ("NSURLVolumeMountFromLocationKey")]
		NSString VolumeMountFromLocationKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsUbiquitousItemKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsUbiquitousItemKey")]
		NSString IsUbiquitousItemKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemHasUnresolvedConflictsKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLUbiquitousItemHasUnresolvedConflictsKey")]
		NSString UbiquitousItemHasUnresolvedConflictsKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemIsDownloadedKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLUbiquitousItemIsDownloadedKey")]
		NSString UbiquitousItemIsDownloadedKey { get; }

		/// <summary>Developers should not use this deprecated property. </summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLUbiquitousItemIsDownloadingKey")]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		NSString UbiquitousItemIsDownloadingKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemIsUploadedKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLUbiquitousItemIsUploadedKey")]
		NSString UbiquitousItemIsUploadedKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemIsUploadingKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLUbiquitousItemIsUploadingKey")]
		NSString UbiquitousItemIsUploadingKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemPercentDownloadedKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLUbiquitousItemPercentDownloadedKey")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'NSMetadataQuery.UbiquitousItemPercentDownloadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSMetadataQuery.UbiquitousItemPercentDownloadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Use 'NSMetadataQuery.UbiquitousItemPercentDownloadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSMetadataQuery.UbiquitousItemPercentDownloadedKey' on 'NSMetadataItem' instead.")]
		NSString UbiquitousItemPercentDownloadedKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemPercentUploadedKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'NSMetadataQuery.UbiquitousItemPercentUploadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSMetadataQuery.UbiquitousItemPercentUploadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Use 'NSMetadataQuery.UbiquitousItemPercentUploadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSMetadataQuery.UbiquitousItemPercentUploadedKey' on 'NSMetadataItem' instead.")]
		[Field ("NSURLUbiquitousItemPercentUploadedKey")]
		NSString UbiquitousItemPercentUploadedKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemIsSharedKey")]
		NSString UbiquitousItemIsSharedKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemCurrentUserRoleKey")]
		NSString UbiquitousSharedItemCurrentUserRoleKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemCurrentUserPermissionsKey")]
		NSString UbiquitousSharedItemCurrentUserPermissionsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemOwnerNameComponentsKey")]
		NSString UbiquitousSharedItemOwnerNameComponentsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemMostRecentEditorNameComponentsKey")]
		NSString UbiquitousSharedItemMostRecentEditorNameComponentsKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemRoleOwner")]
		NSString UbiquitousSharedItemRoleOwner { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemRoleParticipant")]
		NSString UbiquitousSharedItemRoleParticipant { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemPermissionsReadOnly")]
		NSString UbiquitousSharedItemPermissionsReadOnly { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemPermissionsReadWrite")]
		NSString UbiquitousSharedItemPermissionsReadWrite { get; }

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSURLUbiquitousItemSupportedSyncControlsKey")]
		NSString UbiquitousItemSupportedSyncControlsKey { get; }

		[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
		[Field ("NSURLUbiquitousItemIsSyncPausedKey")]
		NSString UbiquitousItemIsSyncPausedKey { get; }

		/// <summary>Represents the value associated with the constant NSURLIsExcludedFromBackupKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLIsExcludedFromBackupKey")]
		NSString IsExcludedFromBackupKey { get; }

		[Export ("bookmarkDataWithOptions:includingResourceValuesForKeys:relativeToURL:error:")]
		NSData CreateBookmarkData (NSUrlBookmarkCreationOptions options, [NullAllowed] string [] resourceValues, [NullAllowed] NSUrl relativeUrl, out NSError error);

		[Export ("initByResolvingBookmarkData:options:relativeToURL:bookmarkDataIsStale:error:")]
		NativeHandle Constructor (NSData bookmarkData, NSUrlBookmarkResolutionOptions resolutionOptions, [NullAllowed] NSUrl relativeUrl, out bool bookmarkIsStale, out NSError error);

		/// <summary>Represents the value associated with the constant NSURLPathKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLPathKey")]
		NSString PathKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemDownloadingStatusKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemDownloadingStatusKey")]
		NSString UbiquitousItemDownloadingStatusKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemDownloadingErrorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemDownloadingErrorKey")]
		NSString UbiquitousItemDownloadingErrorKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemUploadingErrorKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemUploadingErrorKey")]
		NSString UbiquitousItemUploadingErrorKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemDownloadingStatusNotDownloaded</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemDownloadingStatusNotDownloaded")]
		NSString UbiquitousItemDownloadingStatusNotDownloaded { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemDownloadingStatusDownloaded</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemDownloadingStatusDownloaded")]
		NSString UbiquitousItemDownloadingStatusDownloaded { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemDownloadingStatusCurrent</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemDownloadingStatusCurrent")]
		NSString UbiquitousItemDownloadingStatusCurrent { get; }

		[MacCatalyst (13, 1)]
		[Export ("startAccessingSecurityScopedResource")]
		bool StartAccessingSecurityScopedResource ();

		[MacCatalyst (13, 1)]
		[Export ("stopAccessingSecurityScopedResource")]
		void StopAccessingSecurityScopedResource ();

		[MacCatalyst (13, 1)]
		[Static, Export ("URLByResolvingAliasFileAtURL:options:error:")]
		NSUrl ResolveAlias (NSUrl aliasFileUrl, NSUrlBookmarkResolutionOptions options, out NSError error);

		[Static, Export ("fileURLWithPathComponents:")]
		NSUrl CreateFileUrl (string [] pathComponents);

		/// <summary>Represents the value associated with the constant NSURLAddedToDirectoryDateKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLAddedToDirectoryDateKey")]
		NSString AddedToDirectoryDateKey { get; }

		/// <summary>Represents the value associated with the constant NSURLDocumentIdentifierKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLDocumentIdentifierKey")]
		NSString DocumentIdentifierKey { get; }

		/// <summary>Represents the value associated with the constant NSURLGenerationIdentifierKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLGenerationIdentifierKey")]
		NSString GenerationIdentifierKey { get; }

		/// <summary>Represents the value associated with the constant NSURLThumbnailDictionaryKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLThumbnailDictionaryKey")]
		NSString ThumbnailDictionaryKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemContainerDisplayNameKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemContainerDisplayNameKey")]
		NSString UbiquitousItemContainerDisplayNameKey { get; }

		[TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("NSURLUbiquitousItemIsExcludedFromSyncKey")]
		NSString UbiquitousItemIsExcludedFromSyncKey { get; }

		/// <summary>Represents the value associated with the constant NSURLUbiquitousItemDownloadRequestedKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemDownloadRequestedKey")]
		NSString UbiquitousItemDownloadRequestedKey { get; }

		//
		// iOS 9.0/osx 10.11 additions
		//
		[DesignatedInitializer]
		[MacCatalyst (13, 1)]
		[Export ("initFileURLWithPath:isDirectory:relativeToURL:")]
		NativeHandle Constructor (string path, bool isDir, [NullAllowed] NSUrl relativeToUrl);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("fileURLWithPath:isDirectory:relativeToURL:")]
		NSUrl CreateFileUrl (string path, bool isDir, [NullAllowed] NSUrl relativeToUrl);

		[Static]
		[Export ("fileURLWithPath:isDirectory:")]
		NSUrl CreateFileUrl (string path, bool isDir);

		[Static]
		[Export ("fileURLWithPath:relativeToURL:")]
		NSUrl CreateFileUrl (string path, [NullAllowed] NSUrl relativeToUrl);

		[Static]
		[Export ("fileURLWithPath:")]
		NSUrl CreateFileUrl (string path);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("URLWithDataRepresentation:relativeToURL:")]
		NSUrl CreateWithDataRepresentation (NSData data, [NullAllowed] NSUrl relativeToUrl);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("absoluteURLWithDataRepresentation:relativeToURL:")]
		NSUrl CreateAbsoluteUrlWithDataRepresentation (NSData data, [NullAllowed] NSUrl relativeToUrl);

		[MacCatalyst (13, 1)]
		[Export ("dataRepresentation", ArgumentSemantic.Copy)]
		NSData DataRepresentation { get; }

		[MacCatalyst (13, 1)]
		[Export ("hasDirectoryPath")]
		bool HasDirectoryPath { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLIsApplicationKey")]
		NSString IsApplicationKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLFileProtectionKey")]
		NSString FileProtectionKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLFileProtectionNone")]
		NSString FileProtectionNone { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLFileProtectionComplete")]
		NSString FileProtectionComplete { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLFileProtectionCompleteUnlessOpen")]
		NSString FileProtectionCompleteUnlessOpen { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLFileProtectionCompleteUntilFirstUserAuthentication")]
		NSString FileProtectionCompleteUntilFirstUserAuthentication { get; }

		[TV (17, 0), NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSURLFileProtectionCompleteWhenUserInactive")]
		NSString FileProtectionCompleteWhenUserInactive { get; }

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSURLDirectoryEntryCountKey")]
		NSString DirectoryEntryCountKey { get; }

		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLContentTypeKey")]
		NSString ContentTypeKey { get; }

		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLFileContentIdentifierKey")]
		NSString FileContentIdentifierKey { get; }

		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLIsPurgeableKey")]
		NSString IsPurgeableKey { get; }

		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLIsSparseKey")]
		NSString IsSparseKey { get; }

		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLMayHaveExtendedAttributesKey")]
		NSString MayHaveExtendedAttributesKey { get; }

		[TV (14, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLMayShareFileContentKey")]
		NSString MayShareFileContentKey { get; }

		// From the NSItemProviderReading protocol
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("readableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
		new string [] ReadableTypeIdentifiers { get; }

		// From the NSItemProviderReading protocol
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("objectWithItemProviderData:typeIdentifier:error:")]
		[return: NullAllowed]
		new NSUrl GetObject (NSData data, string typeIdentifier, [NullAllowed] out NSError outError);

		// From the NSItemProviderWriting protocol
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("writableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
		new string [] WritableTypeIdentifiers { get; }
	}


	//
	// Just a category so we can document the three methods together
	//
	/// <summary>Defines static methods for dealing with promised items.</summary>
	/// <remarks>
	///       <para>Promised items are resources whose presence in the local file system is not guaranteed until an <see cref="Foundation.NSFileCoordinator" /> performs a coordinated read on the URL, resulting in the contents being either downloaded or generated.</para>
	///     </remarks>
	[Category, BaseType (typeof (NSUrl))]
	partial interface NSUrl_PromisedItems {
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("checkPromisedItemIsReachableAndReturnError:")]
		bool CheckPromisedItemIsReachable (out NSError error);

		/// <param name="value">To be added.</param>
		/// <param name="key">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("getPromisedItemResourceValue:forKey:error:")]
		bool GetPromisedItemResourceValue (out NSObject value, NSString key, out NSError error);

		/// <param name="keys">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("promisedItemResourceValuesForKeys:error:")]
		[return: NullAllowed]
		NSDictionary GetPromisedItemResourceValues (NSString [] keys, out NSError error);

	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NSURLQueryItem")]
	interface NSUrlQueryItem : NSSecureCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithName:value:")]
		NativeHandle Constructor (string name, string value);

		[Export ("name")]
		string Name { get; }

		[Export ("value")]
		string Value { get; }
	}

	/// <summary>Defines static methods defining character sets for various subcomponents of a <see cref="Foundation.NSUrl" />.</summary>
	/// <remarks>To be added.</remarks>
	[Category, BaseType (typeof (NSCharacterSet))]
	partial interface NSUrlUtilities_NSCharacterSet {
		/// <summary>The <see cref="Foundation.NSCharacterSet" /> for characters allowed in a URL user component.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static, Export ("URLUserAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlUserAllowedCharacterSet { get; }

		/// <summary>The <see cref="Foundation.NSCharacterSet" /> for characters allowed in a URL password component.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static, Export ("URLPasswordAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlPasswordAllowedCharacterSet { get; }

		/// <summary>The <see cref="Foundation.NSCharacterSet" /> for characters allowed in a host URL.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static, Export ("URLHostAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlHostAllowedCharacterSet { get; }

		/// <summary>The <see cref="Foundation.NSCharacterSet" /> for characters allowed in a URL path component.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static, Export ("URLPathAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlPathAllowedCharacterSet { get; }

		/// <summary>The <see cref="Foundation.NSCharacterSet" /> for characters allowed in a URL query component.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static, Export ("URLQueryAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlQueryAllowedCharacterSet { get; }

		/// <summary>The <see cref="Foundation.NSCharacterSet" /> for characters allowed in a fragment URL component.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static, Export ("URLFragmentAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlFragmentAllowedCharacterSet { get; }
	}

	[BaseType (typeof (NSObject), Name = "NSURLCache")]
	interface NSUrlCache {
		[Export ("sharedURLCache", ArgumentSemantic.Strong), Static]
		NSUrlCache SharedCache { get; set; }

		/// <param name="memoryCapacity">To be added.</param>
		/// <param name="diskCapacity">To be added.</param>
		/// <param name="diskPath">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the overload that accepts an 'NSUrl' parameter instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the overload that accepts an 'NSUrl' parameter instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the overload that accepts an 'NSUrl' parameter instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that accepts an 'NSUrl' parameter instead.")]
		[Export ("initWithMemoryCapacity:diskCapacity:diskPath:")]
		NativeHandle Constructor (nuint memoryCapacity, nuint diskCapacity, [NullAllowed] string diskPath);

		[MacCatalyst (13, 1)]
		[Export ("initWithMemoryCapacity:diskCapacity:directoryURL:")]
		NativeHandle Constructor (nuint memoryCapacity, nuint diskCapacity, [NullAllowed] NSUrl directoryUrl);

		[Export ("cachedResponseForRequest:")]
		NSCachedUrlResponse CachedResponseForRequest (NSUrlRequest request);

		[Export ("storeCachedResponse:forRequest:")]
		void StoreCachedResponse (NSCachedUrlResponse cachedResponse, NSUrlRequest forRequest);

		[Export ("removeCachedResponseForRequest:")]
		void RemoveCachedResponse (NSUrlRequest request);

		[Export ("removeAllCachedResponses")]
		void RemoveAllCachedResponses ();

		[Export ("memoryCapacity")]
		nuint MemoryCapacity { get; set; }

		[Export ("diskCapacity")]
		nuint DiskCapacity { get; set; }

		[Export ("currentMemoryUsage")]
		nuint CurrentMemoryUsage { get; }

		[Export ("currentDiskUsage")]
		nuint CurrentDiskUsage { get; }

		[MacCatalyst (13, 1)]
		[Export ("removeCachedResponsesSinceDate:")]
		void RemoveCachedResponsesSinceDate (NSDate date);

		[MacCatalyst (13, 1)]
		[Export ("storeCachedResponse:forDataTask:")]
		void StoreCachedResponse (NSCachedUrlResponse cachedResponse, NSUrlSessionDataTask dataTask);

		[MacCatalyst (13, 1)]
		[Export ("getCachedResponseForDataTask:completionHandler:")]
		[Async (XmlDocs = """
			<param name="dataTask">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous GetCachedResponse operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSCachedUrlResponse&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The GetCachedResponseAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void GetCachedResponse (NSUrlSessionDataTask dataTask, Action<NSCachedUrlResponse> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("removeCachedResponseForDataTask:")]
		void RemoveCachedResponse (NSUrlSessionDataTask dataTask);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NSURLComponents")]
	partial interface NSUrlComponents : NSCopying {
		[Export ("initWithURL:resolvingAgainstBaseURL:")]
		NativeHandle Constructor (NSUrl url, bool resolveAgainstBaseUrl);

		[Static, Export ("componentsWithURL:resolvingAgainstBaseURL:")]
		NSUrlComponents FromUrl (NSUrl url, bool resolvingAgainstBaseUrl);

		[Export ("initWithString:")]
		NativeHandle Constructor (string urlString);

		[Static, Export ("componentsWithString:")]
		NSUrlComponents FromString (string urlString);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Static]
		[Export ("componentsWithString:encodingInvalidCharacters:")]
		[return: NullAllowed]
		NSUrlComponents FromString (string url, bool encodingInvalidCharacters);

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("URLRelativeToURL:")]
		NSUrl GetRelativeUrl (NSUrl baseUrl);

		[NullAllowed] // by default this property is null
		[Export ("scheme", ArgumentSemantic.Copy)]
		string Scheme { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("user", ArgumentSemantic.Copy)]
		string User { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("password", ArgumentSemantic.Copy)]
		string Password { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("host", ArgumentSemantic.Copy)]
		string Host { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("port", ArgumentSemantic.Copy)]
		NSNumber Port { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("path", ArgumentSemantic.Copy)]
		string Path { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("query", ArgumentSemantic.Copy)]
		string Query { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("fragment", ArgumentSemantic.Copy)]
		string Fragment { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("percentEncodedUser", ArgumentSemantic.Copy)]
		string PercentEncodedUser { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("percentEncodedPassword", ArgumentSemantic.Copy)]
		string PercentEncodedPassword { get; set; }

		[NullAllowed] // by default this property is null
		[Advice ("Use 'EncodedHost' instead.")]
		[Export ("percentEncodedHost", ArgumentSemantic.Copy)]
		string PercentEncodedHost { get; set; }

		[TV (16, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[NullAllowed, Export ("encodedHost")]
		string EncodedHost { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("percentEncodedPath", ArgumentSemantic.Copy)]
		string PercentEncodedPath { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("percentEncodedQuery", ArgumentSemantic.Copy)]
		string PercentEncodedQuery { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("percentEncodedFragment", ArgumentSemantic.Copy)]
		string PercentEncodedFragment { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("queryItems")]
		NSUrlQueryItem [] QueryItems { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("string")]
		string AsString ();

		[MacCatalyst (13, 1)]
		[Export ("rangeOfScheme")]
		NSRange RangeOfScheme { get; }

		[MacCatalyst (13, 1)]
		[Export ("rangeOfUser")]
		NSRange RangeOfUser { get; }

		[MacCatalyst (13, 1)]
		[Export ("rangeOfPassword")]
		NSRange RangeOfPassword { get; }

		[MacCatalyst (13, 1)]
		[Export ("rangeOfHost")]
		NSRange RangeOfHost { get; }

		[MacCatalyst (13, 1)]
		[Export ("rangeOfPort")]
		NSRange RangeOfPort { get; }

		[MacCatalyst (13, 1)]
		[Export ("rangeOfPath")]
		NSRange RangeOfPath { get; }

		[MacCatalyst (13, 1)]
		[Export ("rangeOfQuery")]
		NSRange RangeOfQuery { get; }

		[MacCatalyst (13, 1)]
		[Export ("rangeOfFragment")]
		NSRange RangeOfFragment { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("percentEncodedQueryItems", ArgumentSemantic.Copy)]
		NSUrlQueryItem [] PercentEncodedQueryItems { get; set; }
	}

	[BaseType (typeof (NSObject), Name = "NSURLAuthenticationChallenge")]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface NSUrlAuthenticationChallenge : NSSecureCoding {
		/// <param name="space">To be added.</param>
		/// <param name="credential">To be added.</param>
		/// <param name="previousFailureCount">To be added.</param>
		/// <param name="response">To be added.</param>
		/// <param name="error">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithProtectionSpace:proposedCredential:previousFailureCount:failureResponse:error:sender:")]
		NativeHandle Constructor (NSUrlProtectionSpace space, NSUrlCredential credential, nint previousFailureCount, [NullAllowed] NSUrlResponse response, [NullAllowed] NSError error, NSUrlConnection sender);

		[Export ("initWithAuthenticationChallenge:sender:")]
		NativeHandle Constructor (NSUrlAuthenticationChallenge challenge, NSUrlConnection sender);

		[Export ("protectionSpace")]
		NSUrlProtectionSpace ProtectionSpace { get; }

		[Export ("proposedCredential")]
		NSUrlCredential ProposedCredential { get; }

		[Export ("previousFailureCount")]
		nint PreviousFailureCount { get; }

		[Export ("failureResponse")]
		NSUrlResponse FailureResponse { get; }

		[Export ("error")]
		NSError Error { get; }

#if XAMCORE_5_0
		[Export ("sender")]
		INSUrlAuthenticationChallengeSender Sender { get; }

#if !XAMCORE_6_0
		[Obsolete ("Use 'Sender' instead.")]
		[Export ("sender")]
		[Sealed]
		INSUrlAuthenticationChallengeSender SenderObject { get; }
#endif
#else
		[Obsolete ("Use 'SenderObject' instead, this property has the wrong type.")]
		[Export ("sender")]
		NSUrlConnection Sender { get; }

		[Export ("sender")]
		[Sealed]
		INSUrlAuthenticationChallengeSender SenderObject { get; }
#endif
	}

	interface INSUrlAuthenticationChallengeSender { }

	[Protocol (Name = "NSURLAuthenticationChallengeSender")]
	interface NSUrlAuthenticationChallengeSender {
		[Abstract]
		[Export ("useCredential:forAuthenticationChallenge:")]
		void UseCredential (NSUrlCredential credential, NSUrlAuthenticationChallenge challenge);

		[Abstract]
		[Export ("continueWithoutCredentialForAuthenticationChallenge:")]
		void ContinueWithoutCredential (NSUrlAuthenticationChallenge challenge);

		[Abstract]
		[Export ("cancelAuthenticationChallenge:")]
		void CancelAuthenticationChallenge (NSUrlAuthenticationChallenge challenge);

		[Export ("performDefaultHandlingForAuthenticationChallenge:")]
		void PerformDefaultHandling (NSUrlAuthenticationChallenge challenge);

		[Export ("rejectProtectionSpaceAndContinueWithChallenge:")]
		void RejectProtectionSpaceAndContinue (NSUrlAuthenticationChallenge challenge);
	}


	/// <param name="response">To be added.</param>
	/// <param name="data">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>The delegate used as the completion handler for <see cref="Foundation.NSUrlConnection.SendAsynchronousRequest(Foundation.NSUrlRequest,Foundation.NSOperationQueue,Foundation.NSUrlConnectionDataResponse)" />.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSUrlConnectionDataResponse ([NullAllowed] NSUrlResponse response, [NullAllowed] NSData data, [NullAllowed] NSError error);

	[BaseType (typeof (NSObject), Name = "NSURLConnection")]
	interface NSUrlConnection :
		NSUrlAuthenticationChallengeSender {
		[Export ("canHandleRequest:")]
		[Static]
		bool CanHandleRequest (NSUrlRequest request);

		[return: NullAllowed]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSUrlSession' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSUrlSession' instead.")]
		[Export ("connectionWithRequest:delegate:")]
		[Static]
		NSUrlConnection FromRequest (NSUrlRequest request, [NullAllowed] INSUrlConnectionDelegate connectionDelegate);

		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSUrlSession' instead.")]
		[Export ("initWithRequest:delegate:")]
		NativeHandle Constructor (NSUrlRequest request, [NullAllowed] INSUrlConnectionDelegate connectionDelegate);

		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSUrlSession' instead.")]
		[Export ("initWithRequest:delegate:startImmediately:")]
		NativeHandle Constructor (NSUrlRequest request, [NullAllowed] INSUrlConnectionDelegate connectionDelegate, bool startImmediately);

		[Export ("start")]
		void Start ();

		[Export ("cancel")]
		void Cancel ();

		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, NSString forMode);

		/// <param name="aRunLoop">To be added.</param>
		///         <param name="forMode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("Schedule (aRunLoop, forMode.GetConstant ()!)")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

		[Export ("unscheduleFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, NSString forMode);

		/// <param name="aRunLoop">To be added.</param>
		///         <param name="forMode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("Unschedule (aRunLoop, forMode.GetConstant ()!)")]
		void Unschedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("originalRequest")]
		NSUrlRequest OriginalRequest { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("currentRequest")]
		NSUrlRequest CurrentRequest { get; }

		[Export ("setDelegateQueue:")]
		void SetDelegateQueue (NSOperationQueue queue);

		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSUrlSession' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSUrlSession' instead.")]
		[Static]
		[Export ("sendSynchronousRequest:returningResponse:error:")]
		[return: NullAllowed]
		NSData SendSynchronousRequest (NSUrlRequest request, out NSUrlResponse response, out NSError error);

		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSUrlSession.CreateDataTask' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSUrlSession.CreateDataTask' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSUrlSession.CreateDataTask' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSUrlSession.CreateDataTask' instead.")]
		[Static]
		[Export ("sendAsynchronousRequest:queue:completionHandler:")]
		[Async (ResultTypeName = "NSUrlAsyncResult", MethodName = "SendRequestAsync", XmlDocs = """
			<param name="request">Request to perform</param>
			<param name="queue">Operation queue to dispatch the completion to.</param>
			<summary>Loads the data and invokes a method upon completion.</summary>
			<returns>
			          <para>A task that represents the asynchronous SendAsynchronousRequest operation. The value of the TResult parameter is of type <c>Action&lt;Foundation.NSUrlAsyncResult&gt;</c>.</para>
			        </returns>
			<remarks>
			          <para copied="true">The SendRequestAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			        </remarks>
			""")]
		void SendAsynchronousRequest (NSUrlRequest request, NSOperationQueue queue, NSUrlConnectionDataResponse completionHandler);
	}

	interface INSUrlConnectionDelegate { }

	[BaseType (typeof (NSObject), Name = "NSURLConnectionDelegate")]
	[Model]
	[Protocol]
	interface NSUrlConnectionDelegate {
		/// <param name="connection">To be added.</param>
		/// <param name="protectionSpace">To be added.</param>
		/// <summary>Developers should not use this deprecated method. Developers should use 'WillSendRequestForAuthenticationChallenge' instead.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("connection:canAuthenticateAgainstProtectionSpace:")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		bool CanAuthenticateAgainstProtectionSpace (NSUrlConnection connection, NSUrlProtectionSpace protectionSpace);

		/// <param name="connection">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("connection:didReceiveAuthenticationChallenge:")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		void ReceivedAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);

		/// <param name="connection">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("connection:didCancelAuthenticationChallenge:")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		void CanceledAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);

		/// <param name="connection">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("connectionShouldUseCredentialStorage:")]
		bool ConnectionShouldUseCredentialStorage (NSUrlConnection connection);

		/// <param name="connection">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("connection:didFailWithError:")]
		void FailedWithError (NSUrlConnection connection, NSError error);

		/// <param name="connection">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("connection:willSendRequestForAuthenticationChallenge:")]
		void WillSendRequestForAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);
	}

	[BaseType (typeof (NSUrlConnectionDelegate), Name = "NSURLConnectionDataDelegate")]
	[Protocol, Model]
	interface NSUrlConnectionDataDelegate {

		/// <param name="connection">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <param name="response">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("connection:willSendRequest:redirectResponse:")]
		NSUrlRequest WillSendRequest (NSUrlConnection connection, NSUrlRequest request, NSUrlResponse response);

		/// <param name="connection">To be added.</param>
		/// <param name="response">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("connection:didReceiveResponse:")]
		void ReceivedResponse (NSUrlConnection connection, NSUrlResponse response);

		/// <param name="connection">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("connection:didReceiveData:")]
		void ReceivedData (NSUrlConnection connection, NSData data);

		/// <param name="connection">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("connection:needNewBodyStream:")]
		NSInputStream NeedNewBodyStream (NSUrlConnection connection, NSUrlRequest request);

		/// <param name="connection">To be added.</param>
		/// <param name="bytesWritten">To be added.</param>
		/// <param name="totalBytesWritten">To be added.</param>
		/// <param name="totalBytesExpectedToWrite">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("connection:didSendBodyData:totalBytesWritten:totalBytesExpectedToWrite:")]
		void SentBodyData (NSUrlConnection connection, nint bytesWritten, nint totalBytesWritten, nint totalBytesExpectedToWrite);

		/// <param name="connection">To be added.</param>
		/// <param name="cachedResponse">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("connection:willCacheResponse:")]
		NSCachedUrlResponse WillCacheResponse (NSUrlConnection connection, NSCachedUrlResponse cachedResponse);

		/// <param name="connection">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("connectionDidFinishLoading:")]
		void FinishedLoading (NSUrlConnection connection);
	}

	[BaseType (typeof (NSUrlConnectionDelegate), Name = "NSURLConnectionDownloadDelegate")]
	[Model]
	[Protocol]
	interface NSUrlConnectionDownloadDelegate {
		/// <param name="connection">To be added.</param>
		/// <param name="bytesWritten">To be added.</param>
		/// <param name="totalBytesWritten">To be added.</param>
		/// <param name="expectedTotalBytes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("connection:didWriteData:totalBytesWritten:expectedTotalBytes:")]
		void WroteData (NSUrlConnection connection, long bytesWritten, long totalBytesWritten, long expectedTotalBytes);

		/// <param name="connection">To be added.</param>
		/// <param name="totalBytesWritten">To be added.</param>
		/// <param name="expectedTotalBytes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("connectionDidResumeDownloading:totalBytesWritten:expectedTotalBytes:")]
		void ResumedDownloading (NSUrlConnection connection, long totalBytesWritten, long expectedTotalBytes);

		/// <param name="connection">To be added.</param>
		/// <param name="destinationUrl">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("connectionDidFinishDownloading:destinationURL:")]
		void FinishedDownloading (NSUrlConnection connection, NSUrl destinationUrl);
	}

	[BaseType (typeof (NSObject), Name = "NSURLCredential")]
	// crash when calling NSObject.get_Description (and likely other selectors)
	[DisableDefaultCtor]
	interface NSUrlCredential : NSSecureCoding, NSCopying {

		[Export ("initWithTrust:")]
		NativeHandle Constructor (SecTrust trust);

		[Export ("persistence")]
		NSUrlCredentialPersistence Persistence { get; }

		[Export ("initWithUser:password:persistence:")]
		NativeHandle Constructor (string user, string password, NSUrlCredentialPersistence persistence);

		[Static]
		[Export ("credentialWithUser:password:persistence:")]
		NSUrlCredential Create (string user, string password, NSUrlCredentialPersistence persistence);

#if !XAMCORE_5_0
		[Obsolete ("Use 'Create' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Wrap ("Create (user, password, persistence)")]
		[Static]
		NSUrlCredential FromUserPasswordPersistance (string user, string password, NSUrlCredentialPersistence persistence);
#endif

		[Export ("user")]
		string User { get; }

		[Export ("password")]
		string Password { get; }

		[Export ("hasPassword")]
		bool HasPassword { get; }

		[Internal]
		[Export ("initWithIdentity:certificates:persistence:")]
		NativeHandle _InitWithIdentity (IntPtr identity, IntPtr certificates, NSUrlCredentialPersistence persistence);

		[Static]
		[Internal]
		[Export ("credentialWithIdentity:certificates:persistence:")]
		NSUrlCredential FromIdentityCertificatesPersistanceInternal (IntPtr identity, IntPtr certificates, NSUrlCredentialPersistence persistence);

		[Internal]
		[Export ("identity")]
		IntPtr Identity { get; }

		[Export ("certificates")]
		SecCertificate [] Certificates { get; }

		[Internal]
		[Static]
		[Export ("credentialForTrust:")]
		NSUrlCredential FromTrust (IntPtr trust);
	}

	[BaseType (typeof (NSObject), Name = "NSURLCredentialStorage")]
	// init returns NIL -> SharedCredentialStorage
	[DisableDefaultCtor]
	interface NSUrlCredentialStorage {
		[Static]
		[Export ("sharedCredentialStorage", ArgumentSemantic.Strong)]
		NSUrlCredentialStorage SharedCredentialStorage { get; }

		[Export ("credentialsForProtectionSpace:")]
		NSDictionary GetCredentials (NSUrlProtectionSpace forProtectionSpace);

		[Export ("allCredentials")]
		NSDictionary AllCredentials { get; }

		[Export ("setCredential:forProtectionSpace:")]
		void SetCredential (NSUrlCredential credential, NSUrlProtectionSpace forProtectionSpace);

		[Export ("removeCredential:forProtectionSpace:")]
		void RemoveCredential (NSUrlCredential credential, NSUrlProtectionSpace forProtectionSpace);

		[Export ("defaultCredentialForProtectionSpace:")]
		NSUrlCredential GetDefaultCredential (NSUrlProtectionSpace forProtectionSpace);

		[Export ("setDefaultCredential:forProtectionSpace:")]
		void SetDefaultCredential (NSUrlCredential credential, NSUrlProtectionSpace forProtectionSpace);

		[MacCatalyst (13, 1)]
		[Export ("removeCredential:forProtectionSpace:options:")]
		void RemoveCredential (NSUrlCredential credential, NSUrlProtectionSpace forProtectionSpace, NSDictionary options);

		/// <summary>Represents the value associated with the constant NSURLCredentialStorageRemoveSynchronizableCredentials</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSURLCredentialStorageRemoveSynchronizableCredentials")]
		NSString RemoveSynchronizableCredentials { get; }

		[Deprecated (PlatformName.MacOSX, 10, 14, "This notification is never posted.")]
		[Deprecated (PlatformName.iOS, 12, 0, "This notification is never posted.")]
		[Deprecated (PlatformName.TvOS, 12, 0, "This notification is never posted.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, "This notification is never posted.")]
		[Field ("NSURLCredentialStorageChangedNotification")]
		[Notification]
		NSString ChangedNotification { get; }

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="protectionSpace">To be added.</param>
			<param name="task">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous GetCredentials operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSDictionary&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("getCredentialsForProtectionSpace:task:completionHandler:")]
		void GetCredentials (NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task, [NullAllowed] Action<NSDictionary> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("setCredential:forProtectionSpace:task:")]
		void SetCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task);

		[MacCatalyst (13, 1)]
		[Export ("removeCredential:forProtectionSpace:options:task:")]
		void RemoveCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSDictionary options, NSUrlSessionTask task);

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="space">To be added.</param>
			<param name="task">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous GetDefaultCredential operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSUrlCredential&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The GetDefaultCredentialAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		[Export ("getDefaultCredentialForProtectionSpace:task:completionHandler:")]
		void GetDefaultCredential (NSUrlProtectionSpace space, NSUrlSessionTask task, [NullAllowed] Action<NSUrlCredential> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("setDefaultCredential:forProtectionSpace:task:")]
		void SetDefaultCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task);

	}

	/// <param name="dataTasks">To be added.</param>
	/// <param name="uploadTasks">To be added.</param>
	/// <param name="downloadTasks">To be added.</param>
	/// <summary>The delegate that serves as the completion handler for <see cref="Foundation.NSUrlSession.GetTasks(Foundation.NSUrlSessionPendingTasks)" />.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSUrlSessionPendingTasks (NSUrlSessionTask [] dataTasks, NSUrlSessionTask [] uploadTasks, NSUrlSessionTask [] downloadTasks);
	delegate void NSUrlSessionAllPendingTasks (NSUrlSessionTask [] tasks);
	/// <param name="data">Data that was received.</param>
	/// <param name="response">The object representing the response.</param>
	/// <param name="error">Error code, if any.</param>
	/// <summary>Signature for callbacks invoked by NSUrlSession for various background operations.</summary>
	/// <remarks>
	///     </remarks>
	delegate void NSUrlSessionResponse ([NullAllowed] NSData data, [NullAllowed] NSUrlResponse response, [NullAllowed] NSError error);
	delegate void NSUrlSessionDownloadResponse (NSUrl data, NSUrlResponse response, NSError error);

	/// <param name="location">To be added.</param>
	/// <param name="response">To be added.</param>
	/// <param name="error">To be added.</param>
	/// <summary>Completion handler for calls to <see cref="Foundation.NSUrlSession.CreateDownloadTask" /> and <see cref="Foundation.NSUrlSession.CreateDownloadTaskFromResumeData(Foundation.NSData,Foundation.NSUrlDownloadSessionResponse)" />.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSUrlDownloadSessionResponse ([NullAllowed] NSUrl location, [NullAllowed] NSUrlResponse response, [NullAllowed] NSError error);

	interface INSUrlSessionDelegate { }

	//
	// Some of the XxxTaskWith methods that take a completion were flagged as allowing a null in
	// 083d9cba1eb997eac5c5ded77db32180c3eef566 with comment:
	//
	// "Add missing [NullAllowed] on NSUrlSession since the
	// delegate is optional and the handler can be null when one
	// is provided (but requiring a delegate along with handlers
	// only duplicates code)"
	//
	// but Apple has flagged these as not allowing null.
	//
	// Leaving the null allowed for now.
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NSURLSession")]
	[DisableDefaultCtorAttribute]
	partial interface NSUrlSession {

		[Static, Export ("sharedSession", ArgumentSemantic.Strong)]
		NSUrlSession SharedSession { get; }

		[Static, Export ("sessionWithConfiguration:")]
		NSUrlSession FromConfiguration (NSUrlSessionConfiguration configuration);

		[Static, Export ("sessionWithConfiguration:delegate:delegateQueue:")]
		NSUrlSession FromWeakConfiguration (NSUrlSessionConfiguration configuration, [NullAllowed] NSObject weakDelegate, [NullAllowed] NSOperationQueue delegateQueue);

		/// <param name="configuration">To be added.</param>
		///         <param name="sessionDelegate">To be added.</param>
		///         <param name="delegateQueue">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static, Wrap ("FromWeakConfiguration (configuration, (NSObject) sessionDelegate, delegateQueue);")]
		NSUrlSession FromConfiguration (NSUrlSessionConfiguration configuration, INSUrlSessionDelegate sessionDelegate, [NullAllowed] NSOperationQueue delegateQueue);

		[Export ("delegateQueue", ArgumentSemantic.Retain)]
		NSOperationQueue DelegateQueue { get; }

		[Export ("delegate", ArgumentSemantic.Retain), NullAllowed]
		NSObject WeakDelegate { get; }

		/// <summary>An instance of the Foundation.INSUrlSessionDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSUrlSessionDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		INSUrlSessionDelegate Delegate { get; }

		[Export ("configuration", ArgumentSemantic.Copy)]
		NSUrlSessionConfiguration Configuration { get; }

		[NullAllowed]
		[Export ("sessionDescription", ArgumentSemantic.Copy)]
		string SessionDescription { get; set; }

		[Export ("finishTasksAndInvalidate")]
		void FinishTasksAndInvalidate ();

		[Export ("invalidateAndCancel")]
		void InvalidateAndCancel ();

		[Export ("resetWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous Reset operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void Reset (Action completionHandler);

		[Export ("flushWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous Flush operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void Flush (Action completionHandler);

		// Fixed version (breaking change) only for NET
		[Export ("getTasksWithCompletionHandler:")]
		[Async (ResultTypeName = "NSUrlSessionActiveTasks", XmlDocs = """
			<summary>Requests the groups of pending tasks (data, upload and downloads).</summary>
			<returns>
			          <para>A task that represents the asynchronous GetTasks operation.   The value of the TResult parameter is of type <c>Action&lt;Foundation.NSUrlSessionActiveTasks&gt;</c>.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void GetTasks (NSUrlSessionPendingTasks completionHandler);

		[Export ("dataTaskWithRequest:")]
		[return: ForcedType]
		NSUrlSessionDataTask CreateDataTask (NSUrlRequest request);

		[Export ("dataTaskWithURL:")]
		[return: ForcedType]
		NSUrlSessionDataTask CreateDataTask (NSUrl url);

		[Export ("uploadTaskWithRequest:fromFile:")]
		[return: ForcedType]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSUrl fileURL);

		[Export ("uploadTaskWithRequest:fromData:")]
		[return: ForcedType]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSData bodyData);

		[Export ("uploadTaskWithStreamedRequest:")]
		[return: ForcedType]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request);

		[Export ("downloadTaskWithRequest:")]
		[return: ForcedType]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrlRequest request);

		[Export ("downloadTaskWithURL:")]
		[return: ForcedType]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrl url);

		[Export ("downloadTaskWithResumeData:")]
		[return: ForcedType]
		NSUrlSessionDownloadTask CreateDownloadTask (NSData resumeData);

		[Export ("dataTaskWithRequest:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();", XmlDocs = """
			<param name="request">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous CreateDataTask operation.   The value of the TResult parameter is of type <c>Action&lt;Foundation.NSUrlSessionDataTaskRequest&gt;</c>.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="request">To be added.</param>
			<param name="result">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		NSUrlSessionDataTask CreateDataTask (NSUrlRequest request, [NullAllowed] NSUrlSessionResponse completionHandler);

		[Export ("dataTaskWithURL:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();", XmlDocs = """
			<param name="url">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous CreateDataTask operation.   The value of the TResult parameter is of type <c>Action&lt;Foundation.NSUrlSessionDataTaskRequest&gt;</c>.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="url">To be added.</param>
			<param name="result">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		NSUrlSessionDataTask CreateDataTask (NSUrl url, [NullAllowed] NSUrlSessionResponse completionHandler);

		[Export ("uploadTaskWithRequest:fromFile:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();", XmlDocs = """
			<param name="request">To be added.</param>
			<param name="fileURL">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous CreateUploadTask operation.   The value of the TResult parameter is of type <c>Action&lt;Foundation.NSUrlSessionDataTaskRequest&gt;</c>.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="request">To be added.</param>
			<param name="fileURL">To be added.</param>
			<param name="result">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSUrl fileURL, NSUrlSessionResponse completionHandler);

		[Export ("uploadTaskWithRequest:fromData:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();", XmlDocs = """
			<param name="request">To be added.</param>
			<param name="bodyData">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous CreateUploadTask operation.   The value of the TResult parameter is of type <c>Action&lt;Foundation.NSUrlSessionDataTaskRequest&gt;</c>.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="request">To be added.</param>
			<param name="bodyData">To be added.</param>
			<param name="result">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSData bodyData, NSUrlSessionResponse completionHandler);

		[Export ("downloadTaskWithRequest:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();", XmlDocs = """
			<param name="request">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous CreateDownloadTask operation.</para>
			        </returns>
			<remarks>
			          <para>
			The downloaded content is stored in a temporary file, whose path is stored in the returned NSUrlSessionDownloadTaskRequest instance. Since this is a temporary file, it will be deleted once the NSUrlSessionDownloadTaskRequest instance is garbage collected (or disposed).
			        </para>
			        </remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="request">A url request that specifies the resource to download.</param>
			<param name="result">Upon return contains the NSUrlSessionDownloadTask for that was created.</param>
			<summary>Downloads a url resource asynchronously to a temporary file.</summary>
			<returns>
			          <para>A task that represents the asynchronous CreateDownloadTask operation.</para>
			        </returns>
			<remarks>
			          <para>
			The downloaded content is stored in a temporary file, whose path is stored in the returned NSUrlSessionDownloadTaskRequest instance. Since this is a temporary file, it will be deleted once the NSUrlSessionDownloadTaskRequest instance is garbage collected (or disposed).
			          </para>
			        </remarks>
			""")]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrlRequest request, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);

		[Export ("downloadTaskWithURL:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();", XmlDocs = """
			<param name="url">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous CreateDownloadTask operation.</para>
			        </returns>
			<remarks>
			          <para>
			The downloaded content is stored in a temporary file, whose path is stored in the returned NSUrlSessionDownloadTaskRequest instance. Since this is a temporary file, it will be deleted once the NSUrlSessionDownloadTaskRequest instance is garbage collected (or disposed).
			        </para>
			        </remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="url">The url that specifies the resource to download.</param>
			<param name="result">Upon return contains the NSUrlSessionDownloadTask for that was created.</param>
			<summary>Downloads a url resource asynchronously to a temporary file.</summary>
			<returns>
			          <para>A task that represents the asynchronous CreateDownloadTask operation.</para>
			        </returns>
			<remarks>
			The downloaded content is stored in a temporary file, whose path is stored in the returned NSUrlSessionDownloadTaskRequest instance. Since this is a temporary file, it will be deleted once the NSUrlSessionDownloadTaskRequest instance is garbage collected (or disposed).
			        </remarks>
			""")]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrl url, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);

		[Export ("downloadTaskWithResumeData:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();", XmlDocs = """
			<param name="resumeData">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous CreateDownloadTaskFromResumeData operation.   The value of the TResult parameter is of type <c>Action&lt;Foundation.NSUrlSessionDownloadTaskRequest&gt;</c>.</para>
			        </returns>
			<remarks>
			          <para>
			The downloaded content is stored in a temporary file, whose path is stored in the returned NSUrlSessionDownloadTaskRequest instance. Since this is a temporary file, it will be deleted once the NSUrlSessionDownloadTaskRequest instance is garbage collected (or disposed).
			          </para>
			        </remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="resumeData">To be added.</param>
			<param name="result">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		NSUrlSessionDownloadTask CreateDownloadTaskFromResumeData (NSData resumeData, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);


		[MacCatalyst (13, 1)]
		[Export ("getAllTasksWithCompletionHandler:")]
		[Async (ResultTypeName = "NSUrlSessionCombinedTasks", XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous GetAllTasks operation.   The value of the TResult parameter is an array of NSUrlSessionTask.  The base class for data-transfer tasks created by a .</para>
			        </returns>
			<remarks>
			          <para copied="true">The GetAllTasksAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void GetAllTasks (NSUrlSessionAllPendingTasks completionHandler);

		/// <param name="hostname">To be added.</param>
		/// <param name="port">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("streamTaskWithHostName:port:")]
		NSUrlSessionStreamTask CreateBidirectionalStream (string hostname, nint port);

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the Network.framework instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use the Network.framework instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use the Network.framework instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the Network.framework instead.")]
		[Export ("streamTaskWithNetService:")]
		NSUrlSessionStreamTask CreateBidirectionalStream (NSNetService service);

		[MacCatalyst (13, 1)]
		[Export ("webSocketTaskWithURL:")]
		NSUrlSessionWebSocketTask CreateWebSocketTask (NSUrl url);

		[MacCatalyst (13, 1)]
		[Export ("webSocketTaskWithURL:protocols:")]
		NSUrlSessionWebSocketTask CreateWebSocketTask (NSUrl url, string [] protocols);

		[MacCatalyst (13, 1)]
		[Export ("webSocketTaskWithRequest:")]
		NSUrlSessionWebSocketTask CreateWebSocketTask (NSUrlRequest request);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("uploadTaskWithResumeData:")]
		[return: ForcedType]
		NSUrlSessionUploadTask CreateUploadTask (NSData resumeData);

		[Async (ResultTypeName = "NSUrlSessionUploadTaskResumeRequest")]
		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("uploadTaskWithResumeData:completionHandler:")]
		[return: ForcedType]
		NSUrlSessionUploadTask CreateUploadTask (NSData resumeData, Action<NSData, NSUrlResponse, NSError> completionHandler);
	}

	/// <summary>Delegate object for <see cref="Foundation.NSUrlSession" /> objects that have <see cref="Foundation.NSUrlSessionStreamTask" /> objects.</summary>
	/// <remarks>To be added.</remarks>
	/// <related type="externalDocumentation" href="https://developer.apple.com/reference/Foundation/NSURLSessionStreamDelegate">Apple documentation for <c>NSURLSessionStreamDelegate</c></related>
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name = "NSURLSessionStreamDelegate")]
	interface NSUrlSessionStreamDelegate {
		/// <param name="session">To be added.</param>
		/// <param name="streamTask">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:readClosedForStreamTask:")]
		void ReadClosed (NSUrlSession session, NSUrlSessionStreamTask streamTask);

		/// <param name="session">To be added.</param>
		/// <param name="streamTask">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:writeClosedForStreamTask:")]
		void WriteClosed (NSUrlSession session, NSUrlSessionStreamTask streamTask);

		/// <param name="session">To be added.</param>
		/// <param name="streamTask">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:betterRouteDiscoveredForStreamTask:")]
		void BetterRouteDiscovered (NSUrlSession session, NSUrlSessionStreamTask streamTask);

		//
		// Note: the names of this methods do not exactly match the Objective-C name
		// because it was a bad name, and does not describe what this does, so the name
		// was picked from the documentation and what it does.
		//
		/// <param name="session">To be added.</param>
		/// <param name="streamTask">To be added.</param>
		/// <param name="inputStream">To be added.</param>
		/// <param name="outputStream">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:streamTask:didBecomeInputStream:outputStream:")]
		void CompletedTaskCaptureStreams (NSUrlSession session, NSUrlSessionStreamTask streamTask, NSInputStream inputStream, NSOutputStream outputStream);
	}

	delegate void NSUrlSessionDataRead ([NullAllowed] NSData data, bool atEof, [NullAllowed] NSError error);
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSUrlSessionTask), Name = "NSURLSessionStreamTask")]
	[DisableDefaultCtor] // now (xcode11) marked as deprecated
	interface NSUrlSessionStreamTask {
		/// <param name="minBytes">To be added.</param>
		/// <param name="maxBytes">To be added.</param>
		/// <param name="timeout">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("readDataOfMinLength:maxLength:timeout:completionHandler:")]
		[Async (ResultTypeName = "NSUrlSessionStreamDataRead", XmlDocs = """
			<param name="minBytes">To be added.</param>
			<param name="maxBytes">To be added.</param>
			<param name="timeout">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous ReadData operation.   The value of the TResult parameter is of type Foundation.NSUrlSessionStreamDataRead.  To be added.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		void ReadData (nuint minBytes, nuint maxBytes, double timeout, NSUrlSessionDataRead completionHandler);

		[Export ("writeData:timeout:completionHandler:")]
		[Async (XmlDocs = """
			<param name="data">To be added.</param>
			<param name="timeout">To be added.</param>
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous WriteData operation</returns>
			<remarks>
			          <para copied="true">The WriteDataAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void WriteData (NSData data, double timeout, Action<NSError> completionHandler);

		[Export ("captureStreams")]
		void CaptureStreams ();

		[Export ("closeWrite")]
		void CloseWrite ();

		[Export ("closeRead")]
		void CloseRead ();

		[Export ("startSecureConnection")]
		void StartSecureConnection ();

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "A secure (TLS) connection cannot become drop back to insecure (non-TLS).")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "A secure (TLS) connection cannot become drop back to insecure (non-TLS).")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "A secure (TLS) connection cannot become drop back to insecure (non-TLS).")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "A secure (TLS) connection cannot become drop back to insecure (non-TLS).")]
		[Export ("stopSecureConnection")]
		void StopSecureConnection ();
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NSURLSessionTask")]
	[DisableDefaultCtor]
	partial interface NSUrlSessionTask : NSCopying, NSProgressReporting {
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "This type is not meant to be user created.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "This type is not meant to be user created.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "This type is not meant to be user created.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "This type is not meant to be user created.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("taskIdentifier")]
		nuint TaskIdentifier { get; }

		[Export ("originalRequest", ArgumentSemantic.Copy), NullAllowed]
		NSUrlRequest OriginalRequest { get; }

		[Export ("currentRequest", ArgumentSemantic.Copy), NullAllowed]
		NSUrlRequest CurrentRequest { get; }

		[Export ("response", ArgumentSemantic.Copy), NullAllowed]
		NSUrlResponse Response { get; }

		[Export ("countOfBytesReceived")]
		long BytesReceived { get; }

		[Export ("countOfBytesSent")]
		long BytesSent { get; }

		[Export ("countOfBytesExpectedToSend")]
		long BytesExpectedToSend { get; }

		[Export ("countOfBytesExpectedToReceive")]
		long BytesExpectedToReceive { get; }

		[NullAllowed] // by default this property is null
		[Export ("taskDescription", ArgumentSemantic.Copy)]
		string TaskDescription { get; set; }

		[Export ("cancel")]
		void Cancel ();

		[Export ("state")]
		NSUrlSessionTaskState State { get; }

		[Export ("error", ArgumentSemantic.Copy), NullAllowed]
		NSError Error { get; }

		[Export ("suspend")]
		void Suspend ();

		[Export ("resume")]
		void Resume ();

		/// <summary>Represents the value associated with the constant NSUrlSessionTransferSizeUnknown</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLSessionTransferSizeUnknown")]
		long TransferSizeUnknown { get; }

		[MacCatalyst (13, 1)]
		[Export ("priority")]
		float Priority { get; set; } /* float, not CGFloat */

		[TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("prefersIncrementalDelivery")]
		bool PrefersIncrementalDelivery { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("earliestBeginDate", ArgumentSemantic.Copy)]
		NSDate EarliestBeginDate { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("countOfBytesClientExpectsToSend")]
		long CountOfBytesClientExpectsToSend { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("countOfBytesClientExpectsToReceive")]
		long CountOfBytesClientExpectsToReceive { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Retain)]
		NSObject WeakDelegate { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		INSUrlSessionTaskDelegate Delegate { get; set; }
	}

	/// <summary>Defines constants for use with <see cref="Foundation.NSUrlSessionTask.Priority" />.</summary>
	/// <remarks>To be added.</remarks>
	[Static]
	[MacCatalyst (13, 1)]
	interface NSUrlSessionTaskPriority {
		/// <summary>Represents the value associated with the constant NSURLSessionTaskPriorityDefault</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLSessionTaskPriorityDefault")]
		float Default { get; } /* float, not CGFloat */

		/// <summary>Represents the value associated with the constant NSURLSessionTaskPriorityLow</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLSessionTaskPriorityLow")]
		float Low { get; } /* float, not CGFloat */

		/// <summary>Represents the value associated with the constant NSURLSessionTaskPriorityHigh</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLSessionTaskPriorityHigh")]
		float High { get; } /* float, not CGFloat */
	}

	// All of the NSUrlSession APIs are either 10.10, or 10.9 and 64-bit only
	// "NSURLSession is not available for i386 targets before Mac OS X 10.10."

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSUrlSessionTask), Name = "NSURLSessionDataTask")]
	[DisableDefaultCtor]
	partial interface NSUrlSessionDataTask {
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSURLSession.CreateDataTask' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSURLSession.CreateDataTask' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSURLSession.CreateDataTask' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSURLSession.CreateDataTask' instead.")]
		[Export ("init")]
		NativeHandle Constructor ();
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSUrlSessionDataTask), Name = "NSURLSessionUploadTask")]
	[DisableDefaultCtor]
	partial interface NSUrlSessionUploadTask {
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSURLSession.CreateUploadTask' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSURLSession.CreateUploadTask' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSURLSession.CreateUploadTask' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSURLSession.CreateUploadTask' instead.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSURLSessionUploadTaskResumeData")]
		NSString ResumeDataKey { get; }

		[Async]
		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("cancelByProducingResumeData:")]
		void CancelByProducingResumeData (Action<NSData> completionHandler);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSUrlSessionTask), Name = "NSURLSessionDownloadTask")]
	[DisableDefaultCtor]
	partial interface NSUrlSessionDownloadTask {
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSURLSession.CreateDownloadTask' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSURLSession.CreateDownloadTask' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSURLSession.CreateDownloadTask' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSURLSession.CreateDownloadTask' instead.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("cancelByProducingResumeData:")]
		void Cancel (Action<NSData> resumeCallback);
	}

	[Internal]
	[Static]
	[MacCatalyst (13, 1)]
	interface ProxyConfigurationDictionaryKeys {
		[Field ("kCFNetworkProxiesHTTPEnable")]
		NSString HttpEnableKey { get; }

		[Field ("kCFStreamPropertyHTTPProxyHost")]
		NSString HttpProxyHostKey { get; }

		[Field ("kCFStreamPropertyHTTPProxyPort")]
		NSString HttpProxyPortKey { get; }

		[NoiOS, NoTV]
		[NoMacCatalyst]
		[Field ("kCFNetworkProxiesHTTPSEnable")]
		NSString HttpsEnableKey { get; }

		[Field ("kCFStreamPropertyHTTPSProxyHost")]
		NSString HttpsProxyHostKey { get; }

		[Field ("kCFStreamPropertyHTTPSProxyPort")]
		NSString HttpsProxyPortKey { get; }
	}

	[MacCatalyst (13, 1)]
	[StrongDictionary ("ProxyConfigurationDictionaryKeys")]
	interface ProxyConfigurationDictionary {
		bool HttpEnable { get; set; }
		string HttpProxyHost { get; set; }
		int HttpProxyPort { get; set; }
		[NoiOS, NoTV]
		[NoMacCatalyst]
		bool HttpsEnable { get; set; }
		string HttpsProxyHost { get; set; }
		int HttpsProxyPort { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NSURLSessionConfiguration")]
	[DisableDefaultCtorAttribute]
	partial interface NSUrlSessionConfiguration : NSCopying {

		[Internal]
		[Static, Export ("defaultSessionConfiguration", ArgumentSemantic.Strong)]
		NSUrlSessionConfiguration _DefaultSessionConfiguration { get; }

		[Internal]
		[Static, Export ("ephemeralSessionConfiguration", ArgumentSemantic.Strong)]
		NSUrlSessionConfiguration _EphemeralSessionConfiguration { get; }

		[Internal]
		[Static, Export ("backgroundSessionConfiguration:")]
		NSUrlSessionConfiguration _BackgroundSessionConfiguration (string identifier);

		[Export ("identifier", ArgumentSemantic.Copy), NullAllowed]
		string Identifier { get; }

		[Export ("requestCachePolicy")]
		NSUrlRequestCachePolicy RequestCachePolicy { get; set; }

		[Export ("timeoutIntervalForRequest")]
		double TimeoutIntervalForRequest { get; set; }

		[Export ("timeoutIntervalForResource")]
		double TimeoutIntervalForResource { get; set; }

		[Export ("networkServiceType")]
		NSUrlRequestNetworkServiceType NetworkServiceType { get; set; }

		[Export ("allowsCellularAccess")]
		bool AllowsCellularAccess { get; set; }

		/// <summary>Whether background tasks can be scheduled at the discretion of the system in order to improve performance.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("discretionary")]
		bool Discretionary { [Bind ("isDiscretionary")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("sessionSendsLaunchEvents")]
		bool SessionSendsLaunchEvents { get; set; }

		[NullAllowed]
		[Export ("connectionProxyDictionary", ArgumentSemantic.Copy)]
		NSDictionary ConnectionProxyDictionary { get; set; }

		[MacCatalyst (13, 1)]
		ProxyConfigurationDictionary StrongConnectionProxyDictionary {
			[Wrap ("new ProxyConfigurationDictionary (ConnectionProxyDictionary!)")]
			get;
			[Wrap ("ConnectionProxyDictionary = value.GetDictionary ()")]
			set;
		}

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'TlsMinimumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TlsMinimumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'TlsMinimumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'TlsMinimumSupportedProtocolVersion' instead.")]
		[Export ("TLSMinimumSupportedProtocol")]
		SslProtocol TLSMinimumSupportedProtocol { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("TLSMinimumSupportedProtocolVersion", ArgumentSemantic.Assign)]
		TlsProtocolVersion TlsMinimumSupportedProtocolVersion { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'TlsMaximumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TlsMaximumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'TlsMaximumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'TlsMaximumSupportedProtocolVersion' instead.")]
		[Export ("TLSMaximumSupportedProtocol")]
		SslProtocol TLSMaximumSupportedProtocol { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("TLSMaximumSupportedProtocolVersion", ArgumentSemantic.Assign)]
		TlsProtocolVersion TlsMaximumSupportedProtocolVersion { get; set; }

		[Deprecated (PlatformName.iOS, 18, 4, message: "Use HTTP/2 or HTTP/3 instead. Only supported when the classic loader is enabled.")]
		[Deprecated (PlatformName.TvOS, 18, 4, message: "Use HTTP/2 or HTTP/3 instead. Only supported when the classic loader is enabled.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 4, message: "Use HTTP/2 or HTTP/3 instead. Only supported when the classic loader is enabled.")]
		[Deprecated (PlatformName.MacOSX, 15, 4, message: "Use HTTP/2 or HTTP/3 instead. Only supported when the classic loader is enabled.")]
		[Export ("HTTPShouldUsePipelining")]
		bool HttpShouldUsePipelining { get; set; }

		[Export ("HTTPShouldSetCookies")]
		bool HttpShouldSetCookies { get; set; }

		[Export ("HTTPCookieAcceptPolicy")]
		NSHttpCookieAcceptPolicy HttpCookieAcceptPolicy { get; set; }

		[NullAllowed]
		[Export ("HTTPAdditionalHeaders", ArgumentSemantic.Copy)]
		NSDictionary HttpAdditionalHeaders { get; set; }

		[Export ("HTTPMaximumConnectionsPerHost")]
		nint HttpMaximumConnectionsPerHost { get; set; }

		[NullAllowed]
		[Export ("HTTPCookieStorage", ArgumentSemantic.Retain)]
		NSHttpCookieStorage HttpCookieStorage { get; set; }

		[NullAllowed]
		[Export ("URLCredentialStorage", ArgumentSemantic.Retain)]
		NSUrlCredentialStorage URLCredentialStorage { get; set; }

		[NullAllowed]
		[Export ("URLCache", ArgumentSemantic.Retain)]
		NSUrlCache URLCache { get; set; }

		[NullAllowed]
		[Export ("protocolClasses", ArgumentSemantic.Copy)]
		NSArray WeakProtocolClasses { get; set; }

		[NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("sharedContainerIdentifier")]
		string SharedContainerIdentifier { get; set; }

		[Internal]
		[MacCatalyst (13, 1)]
		[Static, Export ("backgroundSessionConfigurationWithIdentifier:")]
		NSUrlSessionConfiguration _CreateBackgroundSessionConfiguration (string identifier);

		[Deprecated (PlatformName.iOS, 18, 4, message: "Not supported anymore.")]
		[Deprecated (PlatformName.TvOS, 18, 4, message: "Not supported anymore.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 4, message: "Not supported anymore.")]
		[Deprecated (PlatformName.MacOSX, 15, 4, message: "Not supported anymore.")]
		[MacCatalyst (13, 1)]
		[Export ("shouldUseExtendedBackgroundIdleMode")]
		bool ShouldUseExtendedBackgroundIdleMode { get; set; }

		[NoTV, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("multipathServiceType", ArgumentSemantic.Assign)]
		NSUrlSessionMultipathServiceType MultipathServiceType { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("waitsForConnectivity")]
		bool WaitsForConnectivity { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsExpensiveNetworkAccess")]
		bool AllowsExpensiveNetworkAccess { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsConstrainedNetworkAccess")]
		bool AllowsConstrainedNetworkAccess { get; set; }

		[TV (26, 1), Mac (26, 1), iOS (26, 1), MacCatalyst (26, 1)]
		[Export ("allowsUltraConstrainedNetworkAccess")]
		bool AllowsUltraConstrainedNetworkAccess { get; set; }

		[TV (16, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("requiresDNSSECValidation")]
		bool RequiresDnsSecValidation { get; set; }

		[Internal]
		[Export ("proxyConfigurations", ArgumentSemantic.Copy)]
		IntPtr _ProxyConfigurations { get; set; }

		[iOS (18, 4), TV (18, 4), MacCatalyst (18, 4), Mac (15, 4)]
		[Export ("usesClassicLoadingMode")]
		bool UsesClassicLoadingMode { get; set; }

		[TV (26, 0), Mac (26, 0), iOS (26, 0), MacCatalyst (26, 0)]
		[Export ("enablesEarlyData")]
		bool EnablesEarlyData { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Model, BaseType (typeof (NSObject), Name = "NSURLSessionDelegate")]
	[Protocol]
	partial interface NSUrlSessionDelegate {
		/// <param name="session">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:didBecomeInvalidWithError:")]
		void DidBecomeInvalid (NSUrlSession session, NSError error);

		/// <param name="session">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:didReceiveChallenge:completionHandler:")]
		void DidReceiveChallenge (NSUrlSession session, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler);

		/// <param name="session">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("URLSessionDidFinishEventsForBackgroundURLSession:")]
		void DidFinishEventsForBackgroundSession (NSUrlSession session);
	}

	public interface INSUrlSessionTaskDelegate { }

	[MacCatalyst (13, 1)]
	[Model]
	[BaseType (typeof (NSUrlSessionDelegate), Name = "NSURLSessionTaskDelegate")]
	[Protocol]
	partial interface NSUrlSessionTaskDelegate {

		/// <param name="session">To be added.</param>
		/// <param name="task">To be added.</param>
		/// <param name="response">To be added.</param>
		/// <param name="newRequest">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:task:willPerformHTTPRedirection:newRequest:completionHandler:")]
		void WillPerformHttpRedirection (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, Action<NSUrlRequest?> completionHandler);

		/// <param name="session">To be added.</param>
		/// <param name="task">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:task:didReceiveChallenge:completionHandler:")]
		void DidReceiveChallenge (NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler);

		/// <param name="session">To be added.</param>
		/// <param name="task">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:task:needNewBodyStream:")]
		void NeedNewBodyStream (NSUrlSession session, NSUrlSessionTask task, Action<NSInputStream> completionHandler);

		/// <param name="session">To be added.</param>
		/// <param name="task">To be added.</param>
		/// <param name="bytesSent">To be added.</param>
		/// <param name="totalBytesSent">To be added.</param>
		/// <param name="totalBytesExpectedToSend">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:task:didSendBodyData:totalBytesSent:totalBytesExpectedToSend:")]
		void DidSendBodyData (NSUrlSession session, NSUrlSessionTask task, long bytesSent, long totalBytesSent, long totalBytesExpectedToSend);

		/// <param name="session">To be added.</param>
		/// <param name="task">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:task:didCompleteWithError:")]
		void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, [NullAllowed] NSError error);

		/// <param name="session">To be added.</param>
		/// <param name="task">To be added.</param>
		/// <param name="metrics">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("URLSession:task:didFinishCollectingMetrics:")]
		void DidFinishCollectingMetrics (NSUrlSession session, NSUrlSessionTask task, NSUrlSessionTaskMetrics metrics);

		/// <param name="session">To be added.</param>
		/// <param name="task">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("URLSession:task:willBeginDelayedRequest:completionHandler:")]
		void WillBeginDelayedRequest (NSUrlSession session, NSUrlSessionTask task, NSUrlRequest request, Action<NSUrlSessionDelayedRequestDisposition, NSUrlRequest> completionHandler);

		/// <param name="session">To be added.</param>
		/// <param name="task">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("URLSession:taskIsWaitingForConnectivity:")]
		void TaskIsWaitingForConnectivity (NSUrlSession session, NSUrlSessionTask task);

		[TV (16, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("URLSession:didCreateTask:")]
		void DidCreateTask (NSUrlSession session, NSUrlSessionTask task);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("URLSession:task:didReceiveInformationalResponse:")]
		void DidReceiveInformationalResponse (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response);

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("URLSession:task:needNewBodyStreamFromOffset:completionHandler:")]
		void NeedNewBodyStream (NSUrlSession session, NSUrlSessionTask task, long offset, Action<NSInputStream> completionHandler);
	}

	[MacCatalyst (13, 1)]
	[Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name = "NSURLSessionDataDelegate")]
	[Protocol]
	partial interface NSUrlSessionDataDelegate {
		/// <param name="session">To be added.</param>
		/// <param name="dataTask">To be added.</param>
		/// <param name="response">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:dataTask:didReceiveResponse:completionHandler:")]
		void DidReceiveResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler);

		/// <param name="session">To be added.</param>
		/// <param name="dataTask">To be added.</param>
		/// <param name="downloadTask">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:dataTask:didBecomeDownloadTask:")]
		void DidBecomeDownloadTask (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlSessionDownloadTask downloadTask);

		/// <param name="session">To be added.</param>
		/// <param name="dataTask">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:dataTask:didReceiveData:")]
		void DidReceiveData (NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data);

		/// <param name="session">To be added.</param>
		/// <param name="dataTask">To be added.</param>
		/// <param name="proposedResponse">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:dataTask:willCacheResponse:completionHandler:")]
		void WillCacheResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSCachedUrlResponse proposedResponse, Action<NSCachedUrlResponse> completionHandler);

		/// <param name="session">To be added.</param>
		/// <param name="dataTask">To be added.</param>
		/// <param name="streamTask">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("URLSession:dataTask:didBecomeStreamTask:")]
		void DidBecomeStreamTask (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlSessionStreamTask streamTask);
	}

	[MacCatalyst (13, 1)]
	[Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name = "NSURLSessionDownloadDelegate")]
	[Protocol]
	partial interface NSUrlSessionDownloadDelegate {

		/// <param name="session">To be added.</param>
		/// <param name="downloadTask">To be added.</param>
		/// <param name="location">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("URLSession:downloadTask:didFinishDownloadingToURL:")]
		void DidFinishDownloading (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location);

		/// <param name="session">To be added.</param>
		/// <param name="downloadTask">To be added.</param>
		/// <param name="bytesWritten">To be added.</param>
		/// <param name="totalBytesWritten">To be added.</param>
		/// <param name="totalBytesExpectedToWrite">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:downloadTask:didWriteData:totalBytesWritten:totalBytesExpectedToWrite:")]
		void DidWriteData (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long bytesWritten, long totalBytesWritten, long totalBytesExpectedToWrite);

		/// <param name="session">To be added.</param>
		/// <param name="downloadTask">To be added.</param>
		/// <param name="resumeFileOffset">To be added.</param>
		/// <param name="expectedTotalBytes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("URLSession:downloadTask:didResumeAtOffset:expectedTotalBytes:")]
		void DidResume (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long resumeFileOffset, long expectedTotalBytes);

		/// <summary>Represents the value associated with the constant NSURLSessionDownloadTaskResumeData</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLSessionDownloadTaskResumeData")]
		NSString TaskResumeDataKey { get; }
	}

	interface NSUndoManagerCloseUndoGroupEventArgs {
		// Bug in docs, see header file
		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Export ("NSUndoManagerGroupIsDiscardableKey")]
		[NullAllowed]
		bool Discardable { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSUndoManager {
		[Export ("beginUndoGrouping")]
		void BeginUndoGrouping ();

		[Export ("endUndoGrouping")]
		void EndUndoGrouping ();

		[Export ("groupingLevel")]
		nint GroupingLevel { get; }

		[Export ("disableUndoRegistration")]
		void DisableUndoRegistration ();

		[Export ("enableUndoRegistration")]
		void EnableUndoRegistration ();

		/// <summary>Whether the recording of undo operations is enabled.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isUndoRegistrationEnabled")]
		bool IsUndoRegistrationEnabled { get; }

		[Export ("groupsByEvent")]
		bool GroupsByEvent { get; set; }

		[Export ("levelsOfUndo")]
		nint LevelsOfUndo { get; set; }

		[Export ("runLoopModes", ArgumentSemantic.Copy)]
		NSString [] WeakRunLoopModes { get; set; }

		[Export ("undo")]
		void Undo ();

		[Export ("redo")]
		void Redo ();

		[Export ("undoNestedGroup")]
		void UndoNestedGroup ();

		[Export ("canUndo")]
		bool CanUndo { get; }

		[Export ("canRedo")]
		bool CanRedo { get; }

		[TV (17, 4), Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
		[Export ("undoCount")]
		nuint UndoCount { get; }

		[TV (17, 4), Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
		[Export ("redoCount")]
		nuint RedoCount { get; }

		/// <summary>Whether this NSUndoManager is in the process of performing its undo or UndoNestedGroup method.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isUndoing")]
		bool IsUndoing { get; }

		/// <summary>Whether this NSUndoManager is in the process of performing its redo method.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isRedoing")]
		bool IsRedoing { get; }

		[Export ("removeAllActions")]
		void RemoveAllActions ();

		[Export ("removeAllActionsWithTarget:")]
		void RemoveAllActions (NSObject target);

		[Export ("registerUndoWithTarget:selector:object:")]
		void RegisterUndoWithTarget (NSObject target, Selector selector, [NullAllowed] NSObject anObject);

		[Export ("prepareWithInvocationTarget:")]
		NSObject PrepareWithInvocationTarget (NSObject target);

		[Export ("undoActionName")]
		string UndoActionName { get; }

		[Export ("redoActionName")]
		string RedoActionName { get; }

		[Export ("setActionName:")]
		void SetActionName (string actionName);

		[Export ("undoMenuItemTitle")]
		string UndoMenuItemTitle { get; }

		[Export ("redoMenuItemTitle")]
		string RedoMenuItemTitle { get; }

		[Export ("undoMenuTitleForUndoActionName:")]
		string UndoMenuTitleForUndoActionName (string name);

		[Export ("redoMenuTitleForUndoActionName:")]
		string RedoMenuTitleForUndoActionName (string name);

		[Field ("NSUndoManagerCheckpointNotification")]
		[Notification]
		NSString CheckpointNotification { get; }

		[Field ("NSUndoManagerDidOpenUndoGroupNotification")]
		[Notification]
		NSString DidOpenUndoGroupNotification { get; }

		[Field ("NSUndoManagerDidRedoChangeNotification")]
		[Notification]
		NSString DidRedoChangeNotification { get; }

		[Field ("NSUndoManagerDidUndoChangeNotification")]
		[Notification]
		NSString DidUndoChangeNotification { get; }

		[Field ("NSUndoManagerWillCloseUndoGroupNotification")]
		[Notification (typeof (NSUndoManagerCloseUndoGroupEventArgs))]
		NSString WillCloseUndoGroupNotification { get; }

		[Field ("NSUndoManagerWillRedoChangeNotification")]
		[Notification]
		NSString WillRedoChangeNotification { get; }

		[Field ("NSUndoManagerWillUndoChangeNotification")]
		[Notification]
		NSString WillUndoChangeNotification { get; }

		[Export ("setActionIsDiscardable:")]
		void SetActionIsDiscardable (bool discardable);

		[Export ("undoActionIsDiscardable")]
		bool UndoActionIsDiscardable { get; }

		[Export ("redoActionIsDiscardable")]
		bool RedoActionIsDiscardable { get; }

		/// <summary>Represents the value associated with the constant NSUndoManagerGroupIsDiscardableKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSUndoManagerGroupIsDiscardableKey")]
		NSString GroupIsDiscardableKey { get; }

		[Field ("NSUndoManagerDidCloseUndoGroupNotification")]
		[Notification (typeof (NSUndoManagerCloseUndoGroupEventArgs))]
		NSString DidCloseUndoGroupNotification { get; }

		[MacCatalyst (13, 1)]
		[Export ("registerUndoWithTarget:handler:")]
		void RegisterUndo (NSObject target, Action<NSObject> undoHandler);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("undoActionUserInfoValueForKey:")]
		[return: NullAllowed]
		NSObject GetUndoActionUserInfoValue (string key);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("redoActionUserInfoValueForKey:")]
		[return: NullAllowed]
		NSObject GetRedoActionUserInfoValue (string key);

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("setActionUserInfoValue:forKey:")]
		void SetActionUserInfoValue ([NullAllowed] NSObject info, string key);
	}

	[BaseType (typeof (NSObject), Name = "NSURLProtectionSpace")]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface NSUrlProtectionSpace : NSSecureCoding, NSCopying {

		[Internal]
		[Export ("initWithHost:port:protocol:realm:authenticationMethod:")]
		IntPtr _Init (string host, nint port, [NullAllowed] string protocol, [NullAllowed] string realm, [NullAllowed] string authenticationMethod);

		[Internal]
		[Export ("initWithProxyHost:port:type:realm:authenticationMethod:")]
		IntPtr _InitWithProxy (string host, nint port, [NullAllowed] string type, [NullAllowed] string realm, [NullAllowed] string authenticationMethod);

		[Export ("realm")]
		string Realm { get; }

		[Export ("receivesCredentialSecurely")]
		bool ReceivesCredentialSecurely { get; }

		[Export ("isProxy")]
		bool IsProxy { get; }

		[Export ("host")]
		string Host { get; }

		[Export ("port")]
		nint Port { get; }

		[Export ("proxyType")]
		string ProxyType { get; }

		[Export ("protocol")]
		string Protocol { get; }

		[Export ("authenticationMethod")]
		string AuthenticationMethod { get; }

		// NSURLProtectionSpace(NSClientCertificateSpace)

		[Export ("distinguishedNames")]
		NSData [] DistinguishedNames { get; }

		// NSURLProtectionSpace(NSServerTrustValidationSpace)
		[Internal]
		[Export ("serverTrust")]
		IntPtr ServerTrust { get; }

		/// <summary>Represents the value associated with the constant NSURLProtectionSpaceHTTP</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLProtectionSpaceHTTP")]
		NSString HTTP { get; }

		/// <summary>Represents the value associated with the constant NSURLProtectionSpaceHTTPS</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLProtectionSpaceHTTPS")]
		NSString HTTPS { get; }

		/// <summary>Represents the value associated with the constant NSURLProtectionSpaceFTP</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 18, 4, message: "FTP is deprecated. Only supported when the classic loader is enabled.")]
		[Deprecated (PlatformName.TvOS, 18, 4, message: "FTP is deprecated. Only supported when the classic loader is enabled.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 4, message: "FTP is deprecated. Only supported when the classic loader is enabled.")]
		[Deprecated (PlatformName.MacOSX, 15, 4, message: "FTP is deprecated. Only supported when the classic loader is enabled.")]
		[Field ("NSURLProtectionSpaceFTP")]
		NSString FTP { get; }

		/// <summary>Represents the value associated with the constant NSURLProtectionSpaceHTTPProxy</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.iOS, 18, 4, message: "FTP is deprecated. Only supported when the classic loader is enabled.")]
		[Deprecated (PlatformName.TvOS, 18, 4, message: "FTP is deprecated. Only supported when the classic loader is enabled.")]
		[Deprecated (PlatformName.MacCatalyst, 18, 4, message: "FTP is deprecated. Only supported when the classic loader is enabled.")]
		[Deprecated (PlatformName.MacOSX, 15, 4, message: "FTP is deprecated. Only supported when the classic loader is enabled.")]
		[Field ("NSURLProtectionSpaceHTTPProxy")]
		NSString HTTPProxy { get; }

		/// <summary>Represents the value associated with the constant NSURLProtectionSpaceHTTPSProxy</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLProtectionSpaceHTTPSProxy")]
		NSString HTTPSProxy { get; }

		/// <summary>Represents the value associated with the constant NSURLProtectionSpaceFTPProxy</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLProtectionSpaceFTPProxy")]
		NSString FTPProxy { get; }

		/// <summary>Represents the value associated with the constant NSURLProtectionSpaceSOCKSProxy</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLProtectionSpaceSOCKSProxy")]
		NSString SOCKSProxy { get; }

		/// <summary>Represents the value associated with the constant NSURLAuthenticationMethodDefault</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLAuthenticationMethodDefault")]
		NSString AuthenticationMethodDefault { get; }

		/// <summary>Represents the value associated with the constant NSURLAuthenticationMethodHTTPBasic</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLAuthenticationMethodHTTPBasic")]
		NSString AuthenticationMethodHTTPBasic { get; }

		/// <summary>Represents the value associated with the constant NSURLAuthenticationMethodHTTPDigest</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLAuthenticationMethodHTTPDigest")]
		NSString AuthenticationMethodHTTPDigest { get; }

		/// <summary>Represents the value associated with the constant NSURLAuthenticationMethodHTMLForm</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLAuthenticationMethodHTMLForm")]
		NSString AuthenticationMethodHTMLForm { get; }

		/// <summary>Represents the value associated with the constant NSURLAuthenticationMethodNTLM</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLAuthenticationMethodNTLM")]
		NSString AuthenticationMethodNTLM { get; }

		/// <summary>Represents the value associated with the constant NSURLAuthenticationMethodNegotiate</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLAuthenticationMethodNegotiate")]
		NSString AuthenticationMethodNegotiate { get; }

		/// <summary>Represents the value associated with the constant NSURLAuthenticationMethodClientCertificate</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLAuthenticationMethodClientCertificate")]
		NSString AuthenticationMethodClientCertificate { get; }

		/// <summary>Represents the value associated with the constant NSURLAuthenticationMethodServerTrust</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSURLAuthenticationMethodServerTrust")]
		NSString AuthenticationMethodServerTrust { get; }
	}

	[BaseType (typeof (NSObject), Name = "NSURLRequest")]
	interface NSUrlRequest : NSSecureCoding, NSMutableCopying {
		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[DesignatedInitializer]
		[Export ("initWithURL:cachePolicy:timeoutInterval:")]
		NativeHandle Constructor (NSUrl url, NSUrlRequestCachePolicy cachePolicy, double timeoutInterval);

		[Export ("requestWithURL:")]
		[Static]
		NSUrlRequest FromUrl (NSUrl url);

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("cachePolicy")]
		NSUrlRequestCachePolicy CachePolicy { get; }

		[Export ("timeoutInterval")]
		double TimeoutInterval { get; }

		[Export ("mainDocumentURL")]
		NSUrl MainDocumentURL { get; }

		[Export ("networkServiceType")]
		NSUrlRequestNetworkServiceType NetworkServiceType { get; }

		[Export ("allowsCellularAccess")]
		bool AllowsCellularAccess { get; }

		[MacCatalyst (13, 1)]
		[Export ("allowsExpensiveNetworkAccess")]
		bool AllowsExpensiveNetworkAccess { get; [NotImplemented] set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsConstrainedNetworkAccess")]
		bool AllowsConstrainedNetworkAccess { get; [NotImplemented] set; }

		// It's in the headers, but introspection says it's not, and an Xcode test confirms it's not there
		// [TV (26, 1), Mac (26, 1), iOS (26, 1), MacCatalyst (26, 1)]
		// [Export ("allowsUltraConstrainedNetworkAccess")]
		// bool AllowsUltraConstrainedNetworkAccess { get; [NotImplemented] set; }

		[Export ("HTTPMethod")]
		string HttpMethod { get; }

		[Export ("allHTTPHeaderFields")]
		NSDictionary Headers { get; }

		[Internal]
		[Export ("valueForHTTPHeaderField:")]
		string Header (string field);

		[Export ("HTTPBody")]
		NSData Body { get; }

		[Export ("HTTPBodyStream")]
		NSInputStream BodyStream { get; }

		[Export ("HTTPShouldHandleCookies")]
		bool ShouldHandleCookies { get; }

		[TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("assumesHTTP3Capable")]
		bool AssumesHttp3Capable { get; [NotImplemented] set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("attribution")]
		NSURLRequestAttribution Attribution { get; }

		// macOS is documented out of sync with iOS here
		[TV (16, 1), iOS (16, 1)]
		[MacCatalyst (16, 1)]
		[Export ("requiresDNSSECValidation")]
		bool RequiresDnsSecValidation { get; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("allowsPersistentDNS")]
		bool AllowsPersistentDns { get; }

		[TV (18, 2), iOS (18, 2), MacCatalyst (18, 2), Mac (15, 2)]
		[Export ("cookiePartitionIdentifier", ArgumentSemantic.Copy), NullAllowed]
		string CookiePartitionIdentifier { get; }
	}

	[BaseType (typeof (NSDictionary))]
	[DesignatedDefaultCtor]
	interface NSMutableDictionary {
		[Export ("dictionaryWithContentsOfFile:")]
		[Static]
		NSMutableDictionary FromFile (string path);

		[Export ("dictionaryWithContentsOfURL:")]
		[Static]
		NSMutableDictionary FromUrl (NSUrl url);

		[Export ("dictionaryWithObject:forKey:")]
		[Static]
		NSMutableDictionary FromObjectAndKey (NSObject obj, NSObject key);

		[Export ("dictionaryWithDictionary:")]
		[Static, New]
		NSMutableDictionary FromDictionary (NSDictionary source);

		[Export ("dictionaryWithObjects:forKeys:count:")]
		[Static, Internal]
		NSMutableDictionary FromObjectsAndKeysInternalCount (NSArray objects, NSArray keys, nint count);

		[Export ("dictionaryWithObjects:forKeys:")]
		[Static, Internal, New]
		NSMutableDictionary FromObjectsAndKeysInternal (NSArray objects, NSArray Keys);

		[Export ("initWithDictionary:")]
		NativeHandle Constructor (NSDictionary other);

		[Export ("initWithDictionary:copyItems:")]
		NativeHandle Constructor (NSDictionary other, bool copyItems);

		[Export ("initWithContentsOfFile:")]
		NativeHandle Constructor (string fileName);

		[Export ("initWithContentsOfURL:")]
		NativeHandle Constructor (NSUrl url);

		[Internal]
		[Export ("initWithObjects:forKeys:")]
		NativeHandle Constructor (NSArray objects, NSArray keys);

		[Export ("removeAllObjects"), Internal]
		void RemoveAllObjects ();

		[Sealed]
		[Internal]
		[Export ("removeObjectForKey:")]
		void _RemoveObjectForKey (IntPtr key);

		[Export ("removeObjectForKey:"), Internal]
		void RemoveObjectForKey (NSObject key);

		[Sealed]
		[Internal]
		[Export ("setObject:forKey:")]
		void _SetObject (IntPtr obj, IntPtr key);

		[Export ("setObject:forKey:"), Internal]
		void SetObject (NSObject obj, NSObject key);

		[Static]
		[Export ("dictionaryWithSharedKeySet:")]
		NSDictionary FromSharedKeySet (NSObject sharedKeyToken);

		[Export ("addEntriesFromDictionary:")]
		void AddEntries (NSDictionary other);
	}

	interface NSMutableDictionary<K, V> : NSDictionary { }

	[BaseType (typeof (NSSet))]
	[DesignatedDefaultCtor]
	interface NSMutableSet {
		[Export ("initWithArray:")]
		NativeHandle Constructor (NSArray other);

		[Export ("initWithSet:")]
		NativeHandle Constructor (NSSet other);

		/// <param name="capacity">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[DesignatedInitializer]
		[Export ("initWithCapacity:")]
		NativeHandle Constructor (nint capacity);

		[Internal]
		[Sealed]
		[Export ("addObject:")]
		void _Add (IntPtr obj);

		[Export ("addObject:")]
		void Add (NSObject nso);

		[Internal]
		[Sealed]
		[Export ("removeObject:")]
		void _Remove (IntPtr nso);

		[Export ("removeObject:")]
		void Remove (NSObject nso);

		[Export ("removeAllObjects")]
		void RemoveAll ();

		[Internal]
		[Sealed]
		[Export ("addObjectsFromArray:")]
		void _AddObjects (IntPtr objects);

		[Export ("addObjectsFromArray:")]
		void AddObjects (NSObject [] objects);

		[Internal, Export ("minusSet:")]
		void MinusSet (NSSet other);

		[Internal, Export ("unionSet:")]
		void UnionSet (NSSet other);
	}

	[BaseType (typeof (NSUrlRequest), Name = "NSMutableURLRequest")]
	interface NSMutableUrlRequest {
		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[Export ("initWithURL:cachePolicy:timeoutInterval:")]
		NativeHandle Constructor (NSUrl url, NSUrlRequestCachePolicy cachePolicy, double timeoutInterval);

		[NullAllowed] // by default this property is null
		[New]
		[Export ("URL")]
		NSUrl Url { get; set; }

		[New]
		[Export ("cachePolicy")]
		NSUrlRequestCachePolicy CachePolicy { get; set; }

		[New]
		[Export ("timeoutInterval")]
		double TimeoutInterval { set; get; }

		[NullAllowed] // by default this property is null
		[New]
		[Export ("mainDocumentURL")]
		NSUrl MainDocumentURL { get; set; }

		[New]
		[Export ("HTTPMethod")]
		string HttpMethod { get; set; }

		[NullAllowed] // by default this property is null
		[New]
		[Export ("allHTTPHeaderFields")]
		NSDictionary Headers { get; set; }

		[Internal]
		[Export ("setValue:forHTTPHeaderField:")]
		void _SetValue (string value, string field);

		[NullAllowed] // by default this property is null
		[New]
		[Export ("HTTPBody")]
		NSData Body { get; set; }

		[NullAllowed] // by default this property is null
		[New]
		[Export ("HTTPBodyStream")]
		NSInputStream BodyStream { get; set; }

		[New]
		[Export ("HTTPShouldHandleCookies")]
		bool ShouldHandleCookies { get; set; }

		[Export ("networkServiceType")]
		NSUrlRequestNetworkServiceType NetworkServiceType { set; get; }

		[New]
		[Export ("allowsCellularAccess")]
		bool AllowsCellularAccess { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsExpensiveNetworkAccess")]
		bool AllowsExpensiveNetworkAccess { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("allowsConstrainedNetworkAccess")]
		bool AllowsConstrainedNetworkAccess { get; set; }

		// It's in the headers, but introspection says it's not, and an Xcode test confirms it's not there
		// [TV (26, 1), Mac (26, 1), iOS (26, 1), MacCatalyst (26, 1)]
		// [Export ("allowsUltraConstrainedNetworkAccess")]
		// bool AllowsUltraConstrainedNetworkAccess { get; set; }

		[TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("assumesHTTP3Capable")]
		bool AssumesHttp3Capable { get; set; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("attribution", ArgumentSemantic.Assign)]
		NSURLRequestAttribution Attribution { get; set; }

		// Documented as 16.0 but did not work until 16.1 - https://github.com/xamarin/maccore/issues/2608 - https://feedbackassistant.apple.com/feedback/10897552
		[TV (16, 1), iOS (16, 1)]
		[MacCatalyst (16, 1)]
		[Export ("requiresDNSSECValidation")]
		bool RequiresDnsSecValidation { get; set; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("allowsPersistentDNS")]
		bool AllowsPersistentDns { get; set; }

		[TV (18, 2), iOS (18, 2), MacCatalyst (18, 2), Mac (15, 2)]
		[Export ("cookiePartitionIdentifier", ArgumentSemantic.Copy), NullAllowed]
		string CookiePartitionIdentifier { get; set; }
	}

	[BaseType (typeof (NSObject), Name = "NSURLResponse")]
	interface NSUrlResponse : NSSecureCoding, NSCopying {
		/// <param name="url">To be added.</param>
		/// <param name="mimetype">To be added.</param>
		/// <param name="expectedContentLength">To be added.</param>
		/// <param name="textEncodingName">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[DesignatedInitializer]
		[Export ("initWithURL:MIMEType:expectedContentLength:textEncodingName:")]
		NativeHandle Constructor (NSUrl url, string mimetype, nint expectedContentLength, [NullAllowed] string textEncodingName);

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("MIMEType")]
		string MimeType { get; }

		[Export ("expectedContentLength")]
		long ExpectedContentLength { get; }

		[Export ("textEncodingName")]
		string TextEncodingName { get; }

		[Export ("suggestedFilename")]
		string SuggestedFilename { get; }
	}

	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (NSStreamDelegate) })]
	interface NSStream {
		[Export ("open")]
		void Open ();

		[Export ("close")]
		void Close ();

		// Header says:
		//    assign /* actually weak */
		// so bind as weak
		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the Foundation.INSStreamDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSStreamDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		INSStreamDelegate Delegate { get; set; }

		[Abstract]
		[return: NullAllowed]
		[Protected]
		[Export ("propertyForKey:")]
		NSObject GetProperty (NSString key);

		[Abstract]
		[Protected]
		[Export ("setProperty:forKey:")]
		bool SetProperty ([NullAllowed] NSObject property, NSString key);

		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, NSString mode);

		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, NSString mode);
		/// <param name="aRunLoop">To be added.</param>
		///         <param name="mode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("Schedule (aRunLoop, mode.GetConstant ()!)")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode mode);

		/// <param name="aRunLoop">To be added.</param>
		///         <param name="mode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("Unschedule (aRunLoop, mode.GetConstant ()!)")]
		void Unschedule (NSRunLoop aRunLoop, NSRunLoopMode mode);

		[Export ("streamStatus")]
		NSStreamStatus Status { get; }

		[Export ("streamError")]
		NSError Error { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSocketSecurityLevelKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSocketSecurityLevelKey")]
		NSString SocketSecurityLevelKey { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSocketSecurityLevelNone</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSocketSecurityLevelNone")]
		NSString SocketSecurityLevelNone { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSocketSecurityLevelSSLv2</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSocketSecurityLevelSSLv2")]
		NSString SocketSecurityLevelSslV2 { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSocketSecurityLevelSSLv3</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSocketSecurityLevelSSLv3")]
		NSString SocketSecurityLevelSslV3 { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSocketSecurityLevelTLSv1</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSocketSecurityLevelTLSv1")]
		NSString SocketSecurityLevelTlsV1 { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSocketSecurityLevelNegotiatedSSL</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSocketSecurityLevelNegotiatedSSL")]
		NSString SocketSecurityLevelNegotiatedSsl { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSOCKSProxyConfigurationKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSOCKSProxyConfigurationKey")]
		NSString SocksProxyConfigurationKey { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSOCKSProxyHostKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSOCKSProxyHostKey")]
		NSString SocksProxyHostKey { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSOCKSProxyPortKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSOCKSProxyPortKey")]
		NSString SocksProxyPortKey { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSOCKSProxyVersionKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSOCKSProxyVersionKey")]
		NSString SocksProxyVersionKey { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSOCKSProxyUserKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSOCKSProxyUserKey")]
		NSString SocksProxyUserKey { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSOCKSProxyPasswordKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSOCKSProxyPasswordKey")]
		NSString SocksProxyPasswordKey { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSOCKSProxyVersion4</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSOCKSProxyVersion4")]
		NSString SocksProxyVersion4 { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSOCKSProxyVersion5</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSOCKSProxyVersion5")]
		NSString SocksProxyVersion5 { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamDataWrittenToMemoryStreamKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamDataWrittenToMemoryStreamKey")]
		NSString DataWrittenToMemoryStreamKey { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamFileCurrentOffsetKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamFileCurrentOffsetKey")]
		NSString FileCurrentOffsetKey { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSocketSSLErrorDomain</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSocketSSLErrorDomain")]
		NSString SocketSslErrorDomain { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamSOCKSErrorDomain</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamSOCKSErrorDomain")]
		NSString SocksErrorDomain { get; }

		/// <summary>The low-level service type that this stream is providing.</summary>
		///         <value>
		///         </value>
		///         <remarks>This is the low-level token-based setting for the service type, in general, you can use the strongly typed ServiceType instead.</remarks>
		[Advanced, Field ("NSStreamNetworkServiceType")]
		NSString NetworkServiceType { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamNetworkServiceTypeVoIP</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamNetworkServiceTypeVoIP")]
		NSString NetworkServiceTypeVoIP { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamNetworkServiceTypeVideo</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamNetworkServiceTypeVideo")]
		NSString NetworkServiceTypeVideo { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamNetworkServiceTypeBackground</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		[Advanced, Field ("NSStreamNetworkServiceTypeBackground")]
		NSString NetworkServiceTypeBackground { get; }

		/// <summary>Low-level key: Represents the value associated with the constant NSStreamNetworkServiceTypeVoice</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Advanced, Field ("NSStreamNetworkServiceTypeVoice")]
		NSString NetworkServiceTypeVoice { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Advanced]
		[MacCatalyst (13, 1)]
		[Field ("NSStreamNetworkServiceTypeCallSignaling")]
		NSString NetworkServiceTypeCallSignaling { get; }

		/// <param name="bufferSize">To be added.</param>
		/// <param name="inputStream">To be added.</param>
		/// <param name="outputStream">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Export ("getBoundStreamsWithBufferSize:inputStream:outputStream:")]
		void GetBoundStreams (nuint bufferSize, out NSInputStream inputStream, out NSOutputStream outputStream);

		/// <param name="hostname">To be added.</param>
		/// <param name="port">To be added.</param>
		/// <param name="inputStream">To be added.</param>
		/// <param name="outputStream">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Export ("getStreamsToHostWithName:port:inputStream:outputStream:")]
		void GetStreamsToHost (string hostname, nint port, out NSInputStream inputStream, out NSOutputStream outputStream);
	}

	interface INSStreamDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSStreamDelegate {
		/// <param name="theStream">To be added.</param>
		/// <param name="streamEvent">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("stream:handleEvent:"), EventArgs ("NSStream", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			"""), EventName ("OnEvent")]
		void HandleEvent (NSStream theStream, NSStreamEvent streamEvent);
	}

	[BaseType (typeof (NSObject)), Bind ("NSString")]
	[DesignatedDefaultCtor]
	interface NSString2 : NSSecureCoding, NSMutableCopying, CKRecordValue
#if MONOMAC
		, NSPasteboardReading, NSPasteboardWriting // Documented that it implements NSPasteboard protocols even if header doesn't show it
#endif
		, NSItemProviderReading, NSItemProviderWriting {
		[Export ("initWithData:encoding:")]
		NativeHandle Constructor (NSData data, NSStringEncoding encoding);

		/// <param name="attributedStringAttributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Bind ("sizeWithAttributes:")]
		CGSize StringSize ([NullAllowed] NSDictionary attributedStringAttributes);

		/// <param name="size">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="attributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Bind ("boundingRectWithSize:options:attributes:")]
		CGRect BoundingRectWithSize (CGSize size, NSStringDrawingOptions options, NSDictionary attributes);

		/// <param name="point">To be added.</param>
		/// <param name="attributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Bind ("drawAtPoint:withAttributes:")]
		void DrawString (CGPoint point, NSDictionary attributes);

		/// <param name="rect">To be added.</param>
		/// <param name="attributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Bind ("drawInRect:withAttributes:")]
		void DrawString (CGRect rect, NSDictionary attributes);

		/// <param name="rect">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="attributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Bind ("drawWithRect:options:attributes:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options, NSDictionary attributes);

		[Internal]
		[Export ("characterAtIndex:")]
		char _characterAtIndex (nint index);

		[Export ("length")]
		nint Length { get; }

		/// <param name="handle">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Sealed]
		[Export ("isEqualToString:")]
		bool IsEqualTo (IntPtr handle);

		[Export ("compare:")]
		NSComparisonResult Compare (NSString aString);

		[Export ("compare:options:")]
		NSComparisonResult Compare (NSString aString, NSStringCompareOptions mask);

		[Export ("compare:options:range:")]
		NSComparisonResult Compare (NSString aString, NSStringCompareOptions mask, NSRange range);

		[Export ("compare:options:range:locale:")]
		NSComparisonResult Compare (NSString aString, NSStringCompareOptions mask, NSRange range, [NullAllowed] NSLocale locale);

		[Export ("stringByReplacingCharactersInRange:withString:")]
		NSString Replace (NSRange range, NSString replacement);

		[Export ("commonPrefixWithString:options:")]
		NSString CommonPrefix (NSString aString, NSStringCompareOptions options);

		// start methods from NSStringPathExtensions category

		[Static]
		[Export ("pathWithComponents:")]
		string [] PathWithComponents (string [] components);

		[Export ("pathComponents")]
		string [] PathComponents { get; }

		/// <summary>Whether this is an absolute path.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Export ("isAbsolutePath")]
		bool IsAbsolutePath { get; }

		[Export ("lastPathComponent")]
		NSString LastPathComponent { get; }

		[Export ("stringByDeletingLastPathComponent")]
		NSString DeleteLastPathComponent ();

		[Export ("stringByAppendingPathComponent:")]
		NSString AppendPathComponent (NSString str);

		[Export ("pathExtension")]
		NSString PathExtension { get; }

		[Export ("stringByDeletingPathExtension")]
		NSString DeletePathExtension ();

		[Export ("stringByAppendingPathExtension:")]
		NSString AppendPathExtension (NSString str);

		[Export ("stringByAbbreviatingWithTildeInPath")]
		NSString AbbreviateTildeInPath ();

		[Export ("stringByExpandingTildeInPath")]
		NSString ExpandTildeInPath ();

		[Export ("stringByStandardizingPath")]
#if XAMCORE_5_0
		NSString StandardizePath ();
#else
		[Obsolete ("Use 'StandardizePath' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		NSString StandarizePath ();

		[Wrap ("StandarizePath ()")]
		NSString StandardizePath ();
#endif

		[Export ("stringByResolvingSymlinksInPath")]
		NSString ResolveSymlinksInPath ();

		[Export ("stringsByAppendingPaths:")]
		string [] AppendPaths (string [] paths);

		// end methods from NSStringPathExtensions category

		[Export ("capitalizedStringWithLocale:")]
		string Capitalize ([NullAllowed] NSLocale locale);

		[Export ("lowercaseStringWithLocale:")]
		string ToLower (NSLocale locale);

		[Export ("uppercaseStringWithLocale:")]
		string ToUpper (NSLocale locale);

		[MacCatalyst (13, 1)]
		[Export ("containsString:")]
		bool Contains (NSString str);

		[MacCatalyst (13, 1)]
		[Export ("localizedCaseInsensitiveContainsString:")]
		bool LocalizedCaseInsensitiveContains (NSString str);

		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static, Export ("stringEncodingForData:encodingOptions:convertedString:usedLossyConversion:")]
		nuint DetectStringEncoding (NSData rawData, NSDictionary options, out string convertedString, out bool usedLossyConversion);

		/// <param name="rawData">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="convertedString">To be added.</param>
		/// <param name="usedLossyConversion">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Wrap ("DetectStringEncoding(rawData,options.GetDictionary ()!, out convertedString, out usedLossyConversion)")]
		nuint DetectStringEncoding (NSData rawData, EncodingDetectionOptions options, out string convertedString, out bool usedLossyConversion);

		[MacCatalyst (13, 1)]
		[Internal, Field ("NSStringEncodingDetectionSuggestedEncodingsKey")]
		NSString EncodingDetectionSuggestedEncodingsKey { get; }

		[MacCatalyst (13, 1)]
		[Internal, Field ("NSStringEncodingDetectionDisallowedEncodingsKey")]
		NSString EncodingDetectionDisallowedEncodingsKey { get; }

		[MacCatalyst (13, 1)]
		[Internal, Field ("NSStringEncodingDetectionUseOnlySuggestedEncodingsKey")]
		NSString EncodingDetectionUseOnlySuggestedEncodingsKey { get; }

		[MacCatalyst (13, 1)]
		[Internal, Field ("NSStringEncodingDetectionAllowLossyKey")]
		NSString EncodingDetectionAllowLossyKey { get; }

		[MacCatalyst (13, 1)]
		[Internal, Field ("NSStringEncodingDetectionFromWindowsKey")]
		NSString EncodingDetectionFromWindowsKey { get; }

		[MacCatalyst (13, 1)]
		[Internal, Field ("NSStringEncodingDetectionLossySubstitutionKey")]
		NSString EncodingDetectionLossySubstitutionKey { get; }

		[MacCatalyst (13, 1)]
		[Internal, Field ("NSStringEncodingDetectionLikelyLanguageKey")]
		NSString EncodingDetectionLikelyLanguageKey { get; }

		[Export ("lineRangeForRange:")]
		NSRange LineRangeForRange (NSRange range);

		/// <param name="startPtr">To be added.</param>
		/// <param name="lineEndPtr">To be added.</param>
		/// <param name="contentsEndPtr">To be added.</param>
		/// <param name="range">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("getLineStart:end:contentsEnd:forRange:")]
		void GetLineStart (out nuint startPtr, out nuint lineEndPtr, out nuint contentsEndPtr, NSRange range);

		/// <param name="width">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("variantFittingPresentationWidth:")]
		NSString GetVariantFittingPresentationWidth (nint width);

		[MacCatalyst (13, 1)]
		[Export ("localizedStandardContainsString:")]
		bool LocalizedStandardContainsString (NSString str);

		[MacCatalyst (13, 1)]
		[Export ("localizedStandardRangeOfString:")]
		NSRange LocalizedStandardRangeOfString (NSString str);

		[MacCatalyst (13, 1)]
		[Export ("localizedUppercaseString")]
		NSString LocalizedUppercaseString { get; }

		[MacCatalyst (13, 1)]
		[Export ("localizedLowercaseString")]
		NSString LocalizedLowercaseString { get; }

		[MacCatalyst (13, 1)]
		[Export ("localizedCapitalizedString")]
		NSString LocalizedCapitalizedString { get; }

		[MacCatalyst (13, 1)]
		[Export ("stringByApplyingTransform:reverse:")]
		[return: NullAllowed]
		NSString TransliterateString (NSString transform, bool reverse);

		[Export ("hasPrefix:")]
		bool HasPrefix (NSString prefix);

		[Export ("hasSuffix:")]
		bool HasSuffix (NSString suffix);

		// UNUserNotificationCenterSupport category
		/// <param name="key">To be added.</param>
		/// <param name="arguments">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localizedUserNotificationStringForKey:arguments:")]
		NSString GetLocalizedUserNotificationString (NSString key, [Params][NullAllowed] NSObject [] arguments);

		/// <param name="paragraphStartPosition">To be added.</param>
		/// <param name="paragraphEndPosition">To be added.</param>
		/// <param name="contentsEndPosition">To be added.</param>
		/// <param name="range">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("getParagraphStart:end:contentsEnd:forRange:")]
		void GetParagraphPositions (out nuint paragraphStartPosition, out nuint paragraphEndPosition, out nuint contentsEndPosition, NSRange range);

		[Export ("paragraphRangeForRange:")]
		NSRange GetParagraphRange (NSRange range);

		[Export ("componentsSeparatedByString:")]
		NSString [] SeparateComponents (NSString separator);

		[Export ("componentsSeparatedByCharactersInSet:")]
		NSString [] SeparateComponents (NSCharacterSet separator);

		// From the NSItemProviderReading protocol

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("readableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
		new string [] ReadableTypeIdentifiers { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("objectWithItemProviderData:typeIdentifier:error:")]
		[return: NullAllowed]
		new NSString GetObject (NSData data, string typeIdentifier, [NullAllowed] out NSError outError);

		// From the NSItemProviderWriting protocol
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("writableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
		new string [] WritableTypeIdentifiers { get; }
	}

	/// <summary>A <see cref="Foundation.DictionaryContainer" /> containing hints for detecting the encoding of an <see cref="Foundation.NSString" />.</summary>
	/// <remarks>To be added.</remarks>
	[StrongDictionary ("NSString")]
	interface EncodingDetectionOptions {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		NSStringEncoding [] EncodingDetectionSuggestedEncodings { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		NSStringEncoding [] EncodingDetectionDisallowedEncodings { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		bool EncodingDetectionUseOnlySuggestedEncodings { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		bool EncodingDetectionAllowLossy { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		bool EncodingDetectionFromWindows { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		NSString EncodingDetectionLossySubstitution { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		NSString EncodingDetectionLikelyLanguage { get; set; }
	}

	[BaseType (typeof (NSString))]
	// hack: it seems that generator.cs can't track NSCoding correctly ? maybe because the type is named NSString2 at that time
	interface NSMutableString : NSCoding {
		/// <param name="capacity">Initial capacity for the mutable string.</param>
		/// <summary>Creates a new mutable string with the specified initial capacity.</summary>
		/// <remarks>
		///         </remarks>
		[Export ("initWithCapacity:")]
		NativeHandle Constructor (nint capacity);

		/// <param name="str">String to insert.</param>
		/// <param name="index">Position in the mutable string where the string will be inserted.</param>
		/// <summary>Inserts a string into the mutable string.</summary>
		/// <remarks>
		///         </remarks>
		[PreSnippet ("Check (index);", Optimizable = true)]
		[Export ("insertString:atIndex:")]
		void Insert (NSString str, nint index);

		[PreSnippet ("Check (range);", Optimizable = true)]
		[Export ("deleteCharactersInRange:")]
		void DeleteCharacters (NSRange range);

		[Export ("appendString:")]
		void Append (NSString str);

		[Export ("setString:")]
		void SetString (NSString str);

		[PreSnippet ("Check (range);", Optimizable = true)]
		[Export ("replaceOccurrencesOfString:withString:options:range:")]
#if XAMCORE_5_0
		nuint ReplaceOccurrences (NSString target, NSString replacement, NSStringCompareOptions options, NSRange range);
#else
		[Obsolete ("Use 'ReplaceOccurrences' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		nuint ReplaceOcurrences (NSString target, NSString replacement, NSStringCompareOptions options, NSRange range);

		[Wrap ("ReplaceOcurrences (target, replacement, options, range)")]
		nuint ReplaceOccurrences (NSString target, NSString replacement, NSStringCompareOptions options, NSRange range);
#endif

		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("applyTransform:reverse:range:updatedRange:")]
		bool ApplyTransform (NSString transform, bool reverse, NSRange range, out NSRange resultingRange);

		/// <param name="transform">To be added.</param>
		///         <param name="reverse">To be added.</param>
		///         <param name="range">To be added.</param>
		///         <param name="resultingRange">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("ApplyTransform (transform.GetConstant ()!, reverse, range, out resultingRange)")]
		bool ApplyTransform (NSStringTransform transform, bool reverse, NSRange range, out NSRange resultingRange);

		[Export ("replaceCharactersInRange:withString:")]
		void ReplaceCharactersInRange (NSRange range, NSString aString);
	}

	/// <summary>Defines static methods for URL encoding and escaping.</summary>
	/// <remarks>To be added.</remarks>
	[Category, BaseType (typeof (NSString))]
	partial interface NSUrlUtilities_NSString {
		/// <param name="allowedCharacters">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("stringByAddingPercentEncodingWithAllowedCharacters:")]
		NSString CreateStringByAddingPercentEncoding (NSCharacterSet allowedCharacters);

		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("stringByRemovingPercentEncoding")]
		NSString CreateStringByRemovingPercentEncoding ();

		/// <param name="enc">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("stringByAddingPercentEscapesUsingEncoding:")]
		NSString CreateStringByAddingPercentEscapes (NSStringEncoding enc);

		/// <param name="enc">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("stringByReplacingPercentEscapesUsingEncoding:")]
		NSString CreateStringByReplacingPercentEscapes (NSStringEncoding enc);
	}

	// This comes from UIKit.framework/Headers/NSStringDrawing.h
	/// <include file="../docs/api/Foundation/NSStringDrawingContext.xml" path="/Documentation/Docs[@DocId='T:Foundation.NSStringDrawingContext']/*" />
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[ObjectiveCFramework ("UIKit")]
	interface NSStringDrawingContext {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("minimumScaleFactor")]
		nfloat MinimumScaleFactor { get; set; }

		/// <summary>Desired tracking adjustment (minimum space to maintain between characters) to be used during the drawing operation.</summary>
		/// <value>The value is specified in points, typically a value between -0.5f and 0.</value>
		/// <remarks>Zero means that standard spacing should be used.  Use negative values to adjust the tracking, for example -0.5f allows characters to be closer together by half a point. </remarks>
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("minimumTrackingAdjustment")]
		nfloat MinimumTrackingAdjustment { get; set; }

		/// <summary>Actual scale factor used during the drawing operation.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		[Export ("actualScaleFactor")]
		nfloat ActualScaleFactor { get; }

		/// <summary>Developers should not use this deprecated property. </summary>
		///         <value>The value is specified in points.</value>
		///         <remarks>
		///         </remarks>
		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("actualTrackingAdjustment")]
		nfloat ActualTrackingAdjustment { get; }

		/// <summary>Boundaries used by the drawing operation.</summary>
		///         <value>
		///         </value>
		///         <remarks>This value is updated after a drawing operation.</remarks>
		[Export ("totalBounds")]
		CGRect TotalBounds { get; }
	}

	[BaseType (typeof (NSStream))]
	[DefaultCtorVisibility (Visibility.Protected)]
	interface NSInputStream {
		[Export ("hasBytesAvailable")]
		bool HasBytesAvailable ();

		[Export ("initWithFileAtPath:")]
		NativeHandle Constructor (string path);

		[DesignatedInitializer]
		[Export ("initWithData:")]
		NativeHandle Constructor (NSData data);

		[DesignatedInitializer]
		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[Static]
		[Export ("inputStreamWithData:")]
		NSInputStream FromData (NSData data);

		[Static]
		[Export ("inputStreamWithFileAtPath:")]
		NSInputStream FromFile (string path);

		[Static]
		[Export ("inputStreamWithURL:")]
		NSInputStream FromUrl (NSUrl url);

		[return: NullAllowed]
		[Protected]
		[Export ("propertyForKey:"), Override]
		NSObject GetProperty (NSString key);

		[Protected]
		[Export ("setProperty:forKey:"), Override]
		bool SetProperty ([NullAllowed] NSObject property, NSString key);
	}

	/// <param name="tag">To be added.</param>
	/// <param name="tokenRange">To be added.</param>
	/// <param name="sentenceRange">To be added.</param>
	/// <param name="stop">To be added.</param>
	/// <summary>An enumerator to pass to methods in the <see cref="Foundation.NSLinguisticAnalysis" /> class.</summary>
	/// <returns>To be added.</returns>
	/// <remarks>To be added.</remarks>
	delegate bool NSEnumerateLinguisticTagsEnumerator ([NullAllowed] NSString tag, NSRange tokenRange, NSRange sentenceRange, ref bool stop);

	[Category]
	[BaseType (typeof (NSString))]
	interface NSLinguisticAnalysis {
		/// <param name="range">To be added.</param>
		/// <param name="scheme">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="orthography">To be added.</param>
		/// <param name="tokenRanges">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[return: BindAs (typeof (NSLinguisticTag []))]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("linguisticTagsInRange:scheme:options:orthography:tokenRanges:")]
		NSString [] GetLinguisticTags (NSRange range, NSString scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, [NullAllowed] out NSValue [] tokenRanges);

		/// <param name="range">To be added.</param>
		/// <param name="scheme">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="orthography">To be added.</param>
		/// <param name="tokenRanges">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Wrap ("GetLinguisticTags (This, range, scheme.GetConstant ()!, options, orthography, out tokenRanges)")]
		NSLinguisticTag [] GetLinguisticTags (NSRange range, NSLinguisticTagScheme scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, [NullAllowed] out NSValue [] tokenRanges);

		/// <param name="range">To be added.</param>
		/// <param name="scheme">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="orthography">To be added.</param>
		/// <param name="handler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("enumerateLinguisticTagsInRange:scheme:options:orthography:usingBlock:")]
		void EnumerateLinguisticTags (NSRange range, NSString scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, NSEnumerateLinguisticTagsEnumerator handler);

		/// <param name="range">To be added.</param>
		/// <param name="scheme">To be added.</param>
		/// <param name="options">To be added.</param>
		/// <param name="orthography">To be added.</param>
		/// <param name="handler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Wrap ("EnumerateLinguisticTags (This, range, scheme.GetConstant ()!, options, orthography, handler)")]
		void EnumerateLinguisticTags (NSRange range, NSLinguisticTagScheme scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, NSEnumerateLinguisticTagsEnumerator handler);
	}

	//
	// We expose NSString versions of these methods because it could
	// avoid an extra lookup in cases where there is a large volume of
	// calls being made and the keys are mostly tokens
	//
	[BaseType (typeof (NSObject)), Bind ("NSObject")]
	interface NSObject2 : NSObjectProtocol {

		// those are to please the compiler while creating the definition .dll
		// but, for the final binary, we'll be using manually bounds alternatives
		// not the generated code
#pragma warning disable 108
		[Manual]
		[Export ("conformsToProtocol:")]
		bool ConformsToProtocol (NativeHandle /* Protocol */ aProtocol);

		[Manual]
		[Export ("retain")]
		NSObject DangerousRetain ();

		[Manual]
		[Export ("release")]
		void DangerousRelease ();

		[Manual]
		[Export ("autorelease")]
		NSObject DangerousAutorelease ();
#pragma warning restore 108

		[Export ("doesNotRecognizeSelector:")]
		void DoesNotRecognizeSelector (Selector sel);

		[Export ("observeValueForKeyPath:ofObject:change:context:")]
		void ObserveValue (NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context);

		[Export ("addObserver:forKeyPath:options:context:")]
		void AddObserver (NSObject observer, NSString keyPath, NSKeyValueObservingOptions options, IntPtr context);

		[Wrap ("AddObserver (observer, (NSString) keyPath, options, context)")]
		void AddObserver (NSObject observer, string keyPath, NSKeyValueObservingOptions options, IntPtr context);

		[Export ("removeObserver:forKeyPath:context:")]
		void RemoveObserver (NSObject observer, NSString keyPath, IntPtr context);

		[Wrap ("RemoveObserver (observer, (NSString) keyPath, context)")]
		void RemoveObserver (NSObject observer, string keyPath, IntPtr context);

		[Export ("removeObserver:forKeyPath:")]
		void RemoveObserver (NSObject observer, NSString keyPath);

		[Wrap ("RemoveObserver (observer, (NSString) keyPath)")]
		void RemoveObserver (NSObject observer, string keyPath);

		[Export ("willChangeValueForKey:")]
		void WillChangeValue (string forKey);

		[Export ("didChangeValueForKey:")]
		void DidChangeValue (string forKey);

		[Export ("willChange:valuesAtIndexes:forKey:")]
		void WillChange (NSKeyValueChange changeKind, NSIndexSet indexes, NSString forKey);

		[Export ("didChange:valuesAtIndexes:forKey:")]
		void DidChange (NSKeyValueChange changeKind, NSIndexSet indexes, NSString forKey);

		[Export ("willChangeValueForKey:withSetMutation:usingObjects:")]
		void WillChange (NSString forKey, NSKeyValueSetMutationKind mutationKind, NSSet objects);

		[Export ("didChangeValueForKey:withSetMutation:usingObjects:")]
		void DidChange (NSString forKey, NSKeyValueSetMutationKind mutationKind, NSSet objects);

		[Static, Export ("keyPathsForValuesAffectingValueForKey:")]
		NSSet GetKeyPathsForValuesAffecting (NSString key);

		[Static, Export ("automaticallyNotifiesObserversForKey:")]
		bool AutomaticallyNotifiesObserversForKey (string key);

		[Export ("valueForKey:")]
		[MarshalNativeExceptions]
		NSObject ValueForKey (NSString key);

		[Export ("setValue:forKey:")]
		void SetValueForKey (NSObject value, NSString key);

		[Export ("valueForKeyPath:")]
		NSObject ValueForKeyPath (NSString keyPath);

		[Export ("setValue:forKeyPath:")]
		void SetValueForKeyPath (NSObject value, NSString keyPath);

		[Export ("valueForUndefinedKey:")]
		NSObject ValueForUndefinedKey (NSString key);

		[Export ("setValue:forUndefinedKey:")]
		void SetValueForUndefinedKey (NSObject value, NSString undefinedKey);

		[Export ("setNilValueForKey:")]
		void SetNilValueForKey (NSString key);

		[Export ("dictionaryWithValuesForKeys:")]
		NSDictionary GetDictionaryOfValuesFromKeys (NSString [] keys);

		[Export ("setValuesForKeysWithDictionary:")]
		void SetValuesForKeysWithDictionary (NSDictionary keyedValues);

		[Field ("NSKeyValueChangeKindKey")]
		NSString ChangeKindKey { get; }

		[Field ("NSKeyValueChangeNewKey")]
		NSString ChangeNewKey { get; }

		[Field ("NSKeyValueChangeOldKey")]
		NSString ChangeOldKey { get; }

		[Field ("NSKeyValueChangeIndexesKey")]
		NSString ChangeIndexesKey { get; }

		[Field ("NSKeyValueChangeNotificationIsPriorKey")]
		NSString ChangeNotificationIsPriorKey { get; }

		// Cocoa Bindings added by Kenneth J. Pouncey 2010/11/17
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("valueClassForBinding:")]
		Class GetBindingValueClass (NSString binding);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("exposedBindings")]
		NSString [] ExposedBindings { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("bind:toObject:withKeyPath:options:")]
		void Bind (NSString binding, NSObject observable, string keyPath, [NullAllowed] NSDictionary options);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Wrap ("Bind (binding, observable, keyPath, options.GetDictionary ())")]
		void Bind (NSString binding, NSObject observable, string keyPath, [NullAllowed] NSBindingOptions options);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("unbind:")]
		void Unbind (NSString binding);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("infoForBinding:")]
		NSDictionary GetBindingInfo (NSString binding);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("optionDescriptionsForBinding:")]
		NSObject [] GetBindingOptionDescriptions (NSString aBinding);

		// NSPlaceholders (informal) protocol
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Static]
		[Export ("defaultPlaceholderForMarker:withBinding:")]
		NSObject GetDefaultPlaceholder (NSObject marker, NSString binding);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Static]
		[Export ("setDefaultPlaceholder:forMarker:withBinding:")]
		void SetDefaultPlaceholder (NSObject placeholder, NSObject marker, NSString binding);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, message: "Now on 'NSEditor' protocol.")]
		[Export ("objectDidEndEditing:")]
		void ObjectDidEndEditing (NSObject editor);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, message: "Now on 'NSEditor' protocol.")]
		[Export ("commitEditing")]
		bool CommitEditing ();

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, message: "Now on 'NSEditor' protocol.")]
		[Export ("commitEditingWithDelegate:didCommitSelector:contextInfo:")]
		void CommitEditing (NSObject objDelegate, Selector didCommitSelector, IntPtr contextInfo);

		[Export ("methodForSelector:")]
		IntPtr GetMethodForSelector (Selector sel);

		[PreSnippet ("if (!(this is INSCopying)) throw new InvalidOperationException (\"Type does not conform to NSCopying\");", Optimizable = true)]
		[Export ("copy")]
		[return: Release ()]
		NSObject Copy ();

		[PreSnippet ("if (!(this is INSMutableCopying)) throw new InvalidOperationException (\"Type does not conform to NSMutableCopying\");", Optimizable = true)]
		[Export ("mutableCopy")]
		[return: Release ()]
		NSObject MutableCopy ();

		//
		// Extra Perform methods, with selectors
		//
		[Export ("performSelector:withObject:afterDelay:inModes:")]
		void PerformSelector (Selector selector, [NullAllowed] NSObject withObject, double afterDelay, NSString [] nsRunLoopModes);

		[Export ("performSelector:withObject:afterDelay:")]
		void PerformSelector (Selector selector, [NullAllowed] NSObject withObject, double delay);

		[Export ("performSelector:onThread:withObject:waitUntilDone:")]
		void PerformSelector (Selector selector, NSThread onThread, [NullAllowed] NSObject withObject, bool waitUntilDone);

		[Export ("performSelector:onThread:withObject:waitUntilDone:modes:")]
		void PerformSelector (Selector selector, NSThread onThread, [NullAllowed] NSObject withObject, bool waitUntilDone, [NullAllowed] NSString [] nsRunLoopModes);

		[Static, Export ("cancelPreviousPerformRequestsWithTarget:")]
		void CancelPreviousPerformRequest (NSObject aTarget);

		[Static, Export ("cancelPreviousPerformRequestsWithTarget:selector:object:")]
		void CancelPreviousPerformRequest (NSObject aTarget, Selector selector, [NullAllowed] NSObject argument);

		[MacCatalyst (13, 1)]
		[Export ("prepareForInterfaceBuilder")]
		void PrepareForInterfaceBuilder ();

		[MacCatalyst (13, 1)]
#if MONOMAC
		// comes from NSNibAwaking category and does not requires calling super
#else
		[RequiresSuper] // comes from UINibLoadingAdditions category - which is decorated
#endif
		[Export ("awakeFromNib")]
		void AwakeFromNib ();

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("accessibilityRespondsToUserInteraction")]
		bool AccessibilityRespondsToUserInteraction { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("accessibilityUserInputLabels", ArgumentSemantic.Strong)]
		string [] AccessibilityUserInputLabels { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("accessibilityAttributedUserInputLabels", ArgumentSemantic.Copy)]
		NSAttributedString [] AccessibilityAttributedUserInputLabels { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("accessibilityTextualContext", ArgumentSemantic.Strong)]
		string AccessibilityTextualContext { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[ObjectiveCFramework ("AppKit")]
	interface NSBindingSelectionMarker : NSCopying {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("multipleValuesSelectionMarker", ArgumentSemantic.Strong)]
		NSBindingSelectionMarker MultipleValuesSelectionMarker { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("noSelectionMarker", ArgumentSemantic.Strong)]
		NSBindingSelectionMarker NoSelectionMarker { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Export ("notApplicableSelectionMarker", ArgumentSemantic.Strong)]
		NSBindingSelectionMarker NotApplicableSelectionMarker { get; }

		[NoMacCatalyst]
		[Static]
		[Export ("setDefaultPlaceholder:forMarker:onClass:withBinding:")]
		void SetDefaultPlaceholder ([NullAllowed] NSObject placeholder, [NullAllowed] NSBindingSelectionMarker marker, Class objectClass, string binding);

		[NoMacCatalyst]
		[Static]
		[Export ("defaultPlaceholderForMarker:onClass:withBinding:")]
		[return: NullAllowed]
		NSObject GetDefaultPlaceholder ([NullAllowed] NSBindingSelectionMarker marker, Class objectClass, string binding);
	}

	/// <summary>Base-level object protocol required to be considered a first class Objective-C object.</summary>
	/// <remarks>To be added.</remarks>
	[Protocol (Name = "NSObject")] // exists both as a type and a protocol in ObjC, Swift uses NSObjectProtocol
	interface NSObjectProtocol {

		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("description")]
		string Description { get; }

		[Export ("debugDescription")]
		string DebugDescription { get; }

		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("superclass")]
		Class Superclass { get; }

		// defined multiple times (method, property and even static), one (not static) is required
		// and that match Apple's documentation
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("hash")]
		nuint GetNativeHash ();

		/// <param name="anObject">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isEqual:")]
		bool IsEqual ([NullAllowed] NSObject anObject);

		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("class")]
		Class Class { get; }

		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Export ("self")]
		[Transient]
		NSObject Self { get; }

		/// <param name="aSelector">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("performSelector:")]
		NSObject PerformSelector (Selector aSelector);

		/// <param name="aSelector">To be added.</param>
		/// <param name="anObject">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("performSelector:withObject:")]
		NSObject PerformSelector (Selector aSelector, [NullAllowed] NSObject anObject);

		/// <param name="aSelector">To be added.</param>
		/// <param name="object1">To be added.</param>
		/// <param name="object2">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("performSelector:withObject:withObject:")]
		NSObject PerformSelector (Selector aSelector, [NullAllowed] NSObject object1, [NullAllowed] NSObject object2);

		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isProxy")]
		bool IsProxy { get; }

		/// <param name="aClass">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isKindOfClass:")]
		bool IsKindOfClass ([NullAllowed] Class aClass);

		/// <param name="aClass">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isMemberOfClass:")]
		bool IsMemberOfClass ([NullAllowed] Class aClass);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("conformsToProtocol:")]
		bool ConformsToProtocol ([NullAllowed] NativeHandle /* Protocol */ aProtocol);

		/// <param name="sel">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("respondsToSelector:")]
		bool RespondsToSelector ([NullAllowed] Selector sel);

		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("retain")]
		NSObject DangerousRetain ();

		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("release")]
		void DangerousRelease ();

		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("autorelease")]
		NSObject DangerousAutorelease ();

		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("retainCount")]
		nuint RetainCount { get; }

		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("zone")]
		NSZone Zone { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSOperation {
		[Export ("start")]
		void Start ();

		[Export ("main")]
		void Main ();

		/// <summary>Whether this NSOperation has been cancelled.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isCancelled")]
		bool IsCancelled { get; }

		[Export ("cancel")]
		void Cancel ();

		/// <summary>Whether this operation is currently executing.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isExecuting")]
		bool IsExecuting { get; }

		/// <summary>Whether this operation is done executing.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isFinished")]
		bool IsFinished { get; }

		/// <summary>Whether this operation runs asynchronously.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isConcurrent")]
		bool IsConcurrent { get; }

		/// <summary>Whether this operation can be performed now.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isReady")]
		bool IsReady { get; }

		[Export ("addDependency:")]
		[PostGet ("Dependencies")]
		void AddDependency (NSOperation op);

		[Export ("removeDependency:")]
		[PostGet ("Dependencies")]
		void RemoveDependency (NSOperation op);

		[Export ("dependencies")]
		NSOperation [] Dependencies { get; }

		[NullAllowed]
		[Export ("completionBlock", ArgumentSemantic.Copy)]
		Action CompletionBlock { get; set; }

		/// <summary>Blocks the current thread until this operation finishes.</summary>
		[Export ("waitUntilFinished")]
		void WaitUntilFinished ();

		[Export ("threadPriority")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		double ThreadPriority { get; set; }

		//Detected properties
		[Export ("queuePriority")]
		NSOperationQueuePriority QueuePriority { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("asynchronous")]
		bool Asynchronous { [Bind ("isAsynchronous")] get; }

		[MacCatalyst (13, 1)]
		[Export ("qualityOfService")]
		NSQualityOfService QualityOfService { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }
	}

	[BaseType (typeof (NSOperation))]
	interface NSBlockOperation {
		[Static]
		[Export ("blockOperationWithBlock:")]
		NSBlockOperation Create (/* non null */ Action method);

		[Export ("addExecutionBlock:")]
		void AddExecutionBlock (/* non null */ Action method);

		[Export ("executionBlocks")]
		NSObject [] ExecutionBlocks { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSOperationQueue : NSProgressReporting {
		[Export ("addOperation:")]
		[PostGet ("Operations")]
		void AddOperation ([NullAllowed] NSOperation op);

		[Export ("addOperations:waitUntilFinished:")]
		[PostGet ("Operations")]
		void AddOperations ([NullAllowed] NSOperation [] operations, bool waitUntilFinished);

		[Export ("addOperationWithBlock:")]
		[PostGet ("Operations")]
		void AddOperation (/* non null */ Action operation);

		[Deprecated (PlatformName.MacOSX, 10, 15, 0, message: "This API should not be used as it is subject to race conditions. If synchronization is needed use 'AddBarrier' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "This API should not be used as it is subject to race conditions. If synchronization is needed use 'AddBarrier' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "This API should not be used as it is subject to race conditions. If synchronization is needed use 'AddBarrier' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "This API should not be used as it is subject to race conditions. If synchronization is needed use 'AddBarrier' instead.")]
		[Export ("operations")]
		NSOperation [] Operations { get; }

		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("operationCount")]
		nint OperationCount { get; }

		[MacCatalyst (13, 1)]
		[Export ("addBarrierBlock:")]
		void AddBarrier (Action barrier);

		[Export ("name")]
		string Name { get; set; }

		[Export ("cancelAllOperations")]
		[PostGet ("Operations")]
		void CancelAllOperations ();

		[Export ("waitUntilAllOperationsAreFinished")]
		void WaitUntilAllOperationsAreFinished ();

		[Static]
		[Export ("currentQueue", ArgumentSemantic.Strong)]
		NSOperationQueue CurrentQueue { get; }

		[Static]
		[Export ("mainQueue", ArgumentSemantic.Strong)]
		NSOperationQueue MainQueue { get; }

		//Detected properties
		[Export ("maxConcurrentOperationCount")]
		nint MaxConcurrentOperationCount { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("suspended")]
		bool Suspended { [Bind ("isSuspended")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("qualityOfService")]
		NSQualityOfService QualityOfService { get; set; }

		[NullAllowed]
		[MacCatalyst (13, 1)]
		[Export ("underlyingQueue", ArgumentSemantic.UnsafeUnretained)]
		DispatchQueue UnderlyingQueue { get; set; }

	}

	interface NSOrderedSet<TKey> : NSOrderedSet { }

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSOrderedSet : NSSecureCoding, NSMutableCopying {
		[Export ("initWithObject:")]
		NativeHandle Constructor (NSObject start);

		[Export ("initWithArray:"), Internal]
		NativeHandle Constructor (NSArray array);

		[Export ("initWithSet:")]
		NativeHandle Constructor (NSSet source);

		[Export ("initWithOrderedSet:")]
		NativeHandle Constructor (NSOrderedSet source);

		[Export ("count")]
		nint Count { get; }

		[Internal]
		[Sealed]
		[Export ("objectAtIndex:")]
		IntPtr _GetObject (nint idx);

		[Export ("objectAtIndex:"), Internal]
		NSObject GetObject (nint idx);

		[Export ("array"), Internal]
		IntPtr _ToArray ();

		[Internal]
		[Sealed]
		[Export ("indexOfObject:")]
		nint _IndexOf (IntPtr obj);

		[Export ("indexOfObject:")]
		nint IndexOf (NSObject obj);

		[Export ("objectEnumerator"), Internal]
		NSEnumerator _GetEnumerator ();

		[Internal]
		[Sealed]
		[Export ("set")]
		IntPtr _AsSet ();

		[Export ("set")]
		NSSet AsSet ();

		[Internal]
		[Sealed]
		[Export ("containsObject:")]
		bool _Contains (IntPtr obj);

		[Export ("containsObject:")]
		bool Contains (NSObject obj);

		[Internal]
		[Sealed]
		[Export ("firstObject")]
		IntPtr _FirstObject ();

		[Export ("firstObject")]
		[return: NullAllowed]
		NSObject FirstObject ();

		[Internal]
		[Sealed]
		[Export ("lastObject")]
		IntPtr _LastObject ();

		[Export ("lastObject")]
		[return: NullAllowed]
		NSObject LastObject ();

		[Export ("isEqualToOrderedSet:")]
		bool IsEqualToOrderedSet (NSOrderedSet other);

		[Export ("intersectsOrderedSet:")]
		bool Intersects (NSOrderedSet other);

		[Export ("intersectsSet:")]
		bool Intersects (NSSet other);

		[Export ("isSubsetOfOrderedSet:")]
		bool IsSubset (NSOrderedSet other);

		[Export ("isSubsetOfSet:")]
		bool IsSubset (NSSet other);

		[Export ("reversedOrderedSet")]
		NSOrderedSet GetReverseOrderedSet ();

#if false // https://github.com/dotnet/macios/issues/15577
		[Wrap ("Runtime.GetNSObject <NSOrderedCollectionDifference> (_GetDifference (other, options))")]
		[return: NullAllowed]
		NSOrderedCollectionDifference GetDifference (NSOrderedSet other, NSOrderedCollectionDifferenceCalculationOptions options);
		
		[Internal]
		[Export ("differenceFromOrderedSet:withOptions:")]
		IntPtr _GetDifference (NSOrderedSet other, NSOrderedCollectionDifferenceCalculationOptions options);

		[Wrap ("Runtime.GetNSObject <NSOrderedCollectionDifference> (_GetDifference (other))")]
		[return: NullAllowed]
		NSOrderedCollectionDifference GetDifference (NSOrderedSet other);
		
		[Internal]
		[Export ("differenceFromOrderedSet:")]
		IntPtr _GetDifference (NSOrderedSet other);

		[Wrap ("Runtime.GetNSObject <NSOrderedSet> (_GetOrderedSet (difference))")]
		[return: NullAllowed]
		NSOrderedSet GetOrderedSet (NSOrderedCollectionDifference difference);
		
		[Internal]
		[Export ("orderedSetByApplyingDifference:")]
		[return: NullAllowed]
		IntPtr _GetOrderedSet (NSOrderedCollectionDifference difference);

		[Internal]
		[Export ("differenceFromOrderedSet:withOptions:usingEquivalenceTest:")]
		/* NSOrderedCollectionDifference<NSObject>*/ IntPtr _GetDifference (NSOrderedSet other, NSOrderedCollectionDifferenceCalculationOptions options, /* Func<NSObject, NSObject, bool> */ ref BlockLiteral block);
#endif
	}

	interface NSMutableOrderedSet<TKey> : NSMutableOrderedSet { }

	[BaseType (typeof (NSOrderedSet))]
	[DesignatedDefaultCtor]
	interface NSMutableOrderedSet {
		[Export ("initWithObject:")]
		NativeHandle Constructor (NSObject start);

		[Export ("initWithSet:")]
		NativeHandle Constructor (NSSet source);

		[Export ("initWithOrderedSet:")]
		NativeHandle Constructor (NSOrderedSet source);

		/// <param name="capacity">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[DesignatedInitializer]
		[Export ("initWithCapacity:")]
		NativeHandle Constructor (nint capacity);

		[Export ("initWithArray:"), Internal]
		NativeHandle Constructor (NSArray array);

		[Export ("unionSet:"), Internal]
		void UnionSet (NSSet other);

		[Export ("minusSet:"), Internal]
		void MinusSet (NSSet other);

		[Export ("unionOrderedSet:"), Internal]
		void UnionSet (NSOrderedSet other);

		[Export ("minusOrderedSet:"), Internal]
		void MinusSet (NSOrderedSet other);

		[Internal]
		[Sealed]
		[Export ("insertObject:atIndex:")]
		void _Insert (IntPtr obj, nint atIndex);

		/// <param name="obj">To be added.</param>
		/// <param name="atIndex">To be added.</param>
		/// <summary>Inserts the specified object at the specified index.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("insertObject:atIndex:")]
		void Insert (NSObject obj, nint atIndex);

		/// <param name="index">To be added.</param>
		/// <summary>Removes the object at the specified index.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("removeObjectAtIndex:")]
		void Remove (nint index);

		[Internal]
		[Sealed]
		[Export ("replaceObjectAtIndex:withObject:")]
		void _Replace (nint objectAtIndex, IntPtr newObject);

		/// <param name="objectAtIndex">To be added.</param>
		/// <param name="newObject">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("replaceObjectAtIndex:withObject:")]
		void Replace (nint objectAtIndex, NSObject newObject);

		[Internal]
		[Sealed]
		[Export ("addObject:")]
		void _Add (IntPtr obj);

		[Export ("addObject:")]
		void Add (NSObject obj);

		[Internal]
		[Sealed]
		[Export ("addObjectsFromArray:")]
		void _AddObjects (NSArray source);

		[Export ("addObjectsFromArray:")]
		void AddObjects (NSObject [] source);

		[Internal]
		[Sealed]
		[Export ("insertObjects:atIndexes:")]
		void _InsertObjects (NSArray objects, NSIndexSet atIndexes);

		[Export ("insertObjects:atIndexes:")]
		void InsertObjects (NSObject [] objects, NSIndexSet atIndexes);

		[Export ("removeObjectsAtIndexes:")]
		void RemoveObjects (NSIndexSet indexSet);

		/// <param name="first">To be added.</param>
		/// <param name="second">To be added.</param>
		/// <summary>Exchanges the objects at the specified indices.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("exchangeObjectAtIndex:withObjectAtIndex:")]
		void ExchangeObject (nint first, nint second);

		/// <param name="indexSet">To be added.</param>
		/// <param name="destination">To be added.</param>
		/// <summary>Moves the objects currently at the specified indices so that they start at the specified destination index.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("moveObjectsAtIndexes:toIndex:")]
		void MoveObjects (NSIndexSet indexSet, nint destination);

		[Internal]
		[Sealed]
		[Export ("setObject:atIndex:")]
		void _SetObject (IntPtr obj, nint index);

		/// <param name="obj">To be added.</param>
		/// <param name="index">To be added.</param>
		/// <summary>Appends or replaces the object at the specified index.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("setObject:atIndex:")]
		void SetObject (NSObject obj, nint index);

		[Internal]
		[Sealed]
		[Export ("replaceObjectsAtIndexes:withObjects:")]
		void _ReplaceObjects (NSIndexSet indexSet, NSArray replacementObjects);

		[Export ("replaceObjectsAtIndexes:withObjects:")]
		void ReplaceObjects (NSIndexSet indexSet, NSObject [] replacementObjects);

		[Export ("removeObjectsInRange:")]
		void RemoveObjects (NSRange range);

		[Export ("removeAllObjects")]
		void RemoveAllObjects ();

		[Internal]
		[Sealed]
		[Export ("removeObject:")]
		void _RemoveObject (IntPtr obj);

		[Export ("removeObject:")]
		void RemoveObject (NSObject obj);

		[Internal]
		[Sealed]
		[Export ("removeObjectsInArray:")]
		void _RemoveObjects (NSArray objects);

		[Export ("removeObjectsInArray:")]
		void RemoveObjects (NSObject [] objects);

		[Export ("intersectOrderedSet:")]
		void Intersect (NSOrderedSet intersectWith);

		[Export ("intersectSet:")]
		void Intersect (NSSet intersectWith);

		[Export ("sortUsingComparator:")]
		void Sort (NSComparator comparator);

		[Export ("sortWithOptions:usingComparator:")]
		void Sort (NSSortOptions sortOptions, NSComparator comparator);

		[Export ("sortRange:options:usingComparator:")]
		void SortRange (NSRange range, NSSortOptions sortOptions, NSComparator comparator);

#if false // https://github.com/dotnet/macios/issues/15577
		[Internal]
		[Export ("applyDifference:")]
		void _ApplyDifference (IntPtr difference);

		[Sealed]
		[Export ("applyDifference:")]
		void ApplyDifference (NSOrderedCollectionDifference<NSObject> difference);
#endif
	}

	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[__NSArrayM insertObject:atIndex:]: object cannot be nil
	[DisableDefaultCtor]
	interface NSOrthography : NSSecureCoding, NSCopying {
		[Export ("dominantScript")]
		string DominantScript { get; }

		[Export ("languageMap")]
		NSDictionary LanguageMap { get; }

		[Export ("dominantLanguage")]
		string DominantLanguage { get; }

		[Export ("allScripts")]
		string [] AllScripts { get; }

		[Export ("allLanguages")]
		string [] AllLanguages { get; }

		[Export ("languagesForScript:")]
		string [] LanguagesForScript (string script);

		[Export ("dominantLanguageForScript:")]
		string DominantLanguageForScript (string script);

		[DesignatedInitializer]
		[Export ("initWithDominantScript:languageMap:")]
		NativeHandle Constructor (string dominantScript, [NullAllowed] NSDictionary languageMap);
	}

	[BaseType (typeof (NSStream))]
	[DisableDefaultCtor] // crash when used
	interface NSOutputStream {
		[DesignatedInitializer]
		[Export ("initToMemory")]
		NativeHandle Constructor ();

		[Export ("hasSpaceAvailable")]
		bool HasSpaceAvailable ();

		//[Export ("initToBuffer:capacity:")]
		//NativeHandle Constructor (uint8_t  buffer, NSUInteger capacity);

		[Export ("initToFileAtPath:append:")]
		NativeHandle Constructor (string path, bool shouldAppend);

		[Static]
		[Export ("outputStreamToMemory")]
		NSObject OutputStreamToMemory ();

		//[Static]
		//[Export ("outputStreamToBuffer:capacity:")]
		//NSObject OutputStreamToBuffer (uint8_t  buffer, NSUInteger capacity);

		[Static]
		[Export ("outputStreamToFileAtPath:append:")]
		NSOutputStream CreateFile (string path, bool shouldAppend);

		[return: NullAllowed]
		[Protected]
		[Export ("propertyForKey:"), Override]
		NSObject GetProperty (NSString key);

		[Protected]
		[Export ("setProperty:forKey:"), Override]
		bool SetProperty ([NullAllowed] NSObject property, NSString key);
	}

	[BaseType (typeof (NSObject), Name = "NSHTTPCookie")]
	// default 'init' crash both simulator and devices
	[DisableDefaultCtor]
	interface NSHttpCookie {
		[Export ("initWithProperties:")]
		NativeHandle Constructor (NSDictionary properties);

		[Export ("cookieWithProperties:"), Static]
		NSHttpCookie CookieFromProperties (NSDictionary properties);

		[Export ("requestHeaderFieldsWithCookies:"), Static]
		NSDictionary RequestHeaderFieldsWithCookies (NSHttpCookie [] cookies);

		[Export ("cookiesWithResponseHeaderFields:forURL:"), Static]
		NSHttpCookie [] CookiesWithResponseHeaderFields (NSDictionary headerFields, NSUrl url);

		[Export ("properties")]
		NSDictionary Properties { get; }

		[Export ("version")]
		nuint Version { get; }

		[Export ("value")]
		string Value { get; }

		[Export ("expiresDate")]
		NSDate ExpiresDate { get; }

		/// <summary>Whether this cookie should be discarded at the end of the session, ignoring the expiration date.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isSessionOnly")]
		bool IsSessionOnly { get; }

		[Export ("domain")]
		string Domain { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("path")]
		string Path { get; }

		/// <summary>Whether this cookie should only be sent over secure channels.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isSecure")]
		bool IsSecure { get; }

		/// <summary>Whether this cookie should only be sent to HTTP servers per RFC 2965.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isHTTPOnly")]
		bool IsHttpOnly { get; }

		[Export ("comment")]
		string Comment { get; }

		[Export ("commentURL")]
		NSUrl CommentUrl { get; }

		[Export ("portList")]
		NSNumber [] PortList { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookieName")]
		NSString KeyName { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookieValue")]
		NSString KeyValue { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookieOriginURL")]
		NSString KeyOriginUrl { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookieVersion")]
		NSString KeyVersion { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookieDomain")]
		NSString KeyDomain { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookiePath")]
		NSString KeyPath { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookieSecure")]
		NSString KeySecure { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookieExpires")]
		NSString KeyExpires { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookieComment")]
		NSString KeyComment { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookieCommentURL")]
		NSString KeyCommentUrl { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookieDiscard")]
		NSString KeyDiscard { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookieMaximumAge")]
		NSString KeyMaximumAge { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSHTTPCookiePort")]
		NSString KeyPort { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSHTTPCookieSameSitePolicy")]
		NSString KeySameSitePolicy { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSHTTPCookieSameSiteLax")]
		NSString KeySameSiteLax { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSHTTPCookieSameSiteStrict")]
		NSString KeySameSiteStrict { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("sameSitePolicy")]
		NSString SameSitePolicy { get; }

		[TV (18, 2), iOS (18, 2), MacCatalyst (18, 2), Mac (15, 2)]
		[Field ("NSHTTPCookieSetByJavaScript")]
		NSString KeySetByJavaScript { get; }
	}

	[BaseType (typeof (NSObject), Name = "NSHTTPCookieStorage")]
	// NSHTTPCookieStorage implements a singleton object -> use SharedStorage since 'init' returns NIL
	[DisableDefaultCtor]
	interface NSHttpCookieStorage {
		[Export ("sharedHTTPCookieStorage", ArgumentSemantic.Strong), Static]
		NSHttpCookieStorage SharedStorage { get; }

		[Export ("cookies")]
		NSHttpCookie [] Cookies { get; }

		[Export ("setCookie:")]
		void SetCookie (NSHttpCookie cookie);

		[Export ("deleteCookie:")]
		void DeleteCookie (NSHttpCookie cookie);

		[Export ("cookiesForURL:")]
		NSHttpCookie [] CookiesForUrl (NSUrl url);

		[Export ("setCookies:forURL:mainDocumentURL:")]
		void SetCookies (NSHttpCookie [] cookies, NSUrl forUrl, NSUrl mainDocumentUrl);

		[Export ("cookieAcceptPolicy")]
		NSHttpCookieAcceptPolicy AcceptPolicy { get; set; }

		[Export ("sortedCookiesUsingDescriptors:")]
		NSHttpCookie [] GetSortedCookies (NSSortDescriptor [] sortDescriptors);

		// @required - (void)removeCookiesSinceDate:(NSDate *)date;
		[MacCatalyst (13, 1)]
		[Export ("removeCookiesSinceDate:")]
		void RemoveCookiesSinceDate (NSDate date);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("sharedCookieStorageForGroupContainerIdentifier:")]
		NSHttpCookieStorage GetSharedCookieStorage (string groupContainerIdentifier);

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="task">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous GetCookiesForTask operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSHttpCookie[]&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The GetCookiesForTaskAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		[Export ("getCookiesForTask:completionHandler:")]
		void GetCookiesForTask (NSUrlSessionTask task, Action<NSHttpCookie []> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("storeCookies:forTask:")]
		void StoreCookies (NSHttpCookie [] cookies, NSUrlSessionTask task);

		[Notification]
		[Field ("NSHTTPCookieManagerAcceptPolicyChangedNotification")]
		NSString CookiesChangedNotification { get; }

		[Deprecated (PlatformName.MacOSX, 10, 11, "This notification is never posted.")]
		[Deprecated (PlatformName.iOS, 9, 0, "This notification is never posted.")]
		[Deprecated (PlatformName.TvOS, 9, 0, "This notification is never posted.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, "This notification is never posted.")]
		[Notification]
		[Field ("NSHTTPCookieManagerCookiesChangedNotification")]
		NSString AcceptPolicyChangedNotification { get; }
	}

	[BaseType (typeof (NSUrlResponse), Name = "NSHTTPURLResponse")]
	interface NSHttpUrlResponse {
		/// <param name="url">To be added.</param>
		/// <param name="mimetype">To be added.</param>
		/// <param name="expectedContentLength">To be added.</param>
		/// <param name="textEncodingName">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithURL:MIMEType:expectedContentLength:textEncodingName:")]
		NativeHandle Constructor (NSUrl url, string mimetype, nint expectedContentLength, [NullAllowed] string textEncodingName);

		/// <param name="url">To be added.</param>
		/// <param name="statusCode">To be added.</param>
		/// <param name="httpVersion">To be added.</param>
		/// <param name="headerFields">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithURL:statusCode:HTTPVersion:headerFields:")]
		NativeHandle Constructor (NSUrl url, nint statusCode, [NullAllowed] string httpVersion, [NullAllowed] NSDictionary headerFields);

		[Export ("statusCode")]
		nint StatusCode { get; }

		[Export ("allHeaderFields")]
		NSDictionary AllHeaderFields { get; }

		/// <param name="statusCode">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("localizedStringForStatusCode:")]
		[Static]
		string LocalizedStringForStatusCode (nint statusCode);

		[MacCatalyst (13, 1)]
		[Export ("valueForHTTPHeaderField:")]
		[return: NullAllowed]
		string GetHttpHeaderValue (string headerField);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	partial interface NSBundle {
		[Export ("mainBundle")]
		[Static]
		NSBundle MainBundle { get; }

		[Export ("bundleWithPath:")]
		[Static]
		[return: NullAllowed]
		NSBundle FromPath (string path);

		[DesignatedInitializer]
		[Export ("initWithPath:")]
		NativeHandle Constructor (string path);

		[Export ("bundleForClass:")]
		[Static]
		NSBundle FromClass (Class c);

		[Export ("bundleWithIdentifier:")]
		[Static]
		[return: NullAllowed]
		NSBundle FromIdentifier (string str);

#if !XAMCORE_5_0
		[Internal]
		[Export ("allBundles")]
		[Static]
		NSArray _InternalAllBundles { get; }

		/// <summary>An array of all the application's non-framework bundles.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'AllBundles' property instead.")]
		[Wrap ("_InternalAllBundles")]
		[Static]
		NSBundle [] _AllBundles { get; }

		[Wrap ("_InternalAllBundles")]
		[Static]
		NSBundle [] AllBundles { get; }
#else
		[Export ("allBundles")][Static]
		NSBundle [] AllBundles { get; }
#endif

		[Export ("allFrameworks")]
		[Static]
		NSBundle [] AllFrameworks { get; }

		[Export ("load")]
		bool Load ();

		/// <summary>Whether this bundle is currently loaded.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isLoaded")]
		bool IsLoaded { get; }

		[Export ("unload")]
		bool Unload ();

		[Export ("bundlePath")]
		string BundlePath { get; }

		[Export ("resourcePath")]
		[NullAllowed]
		string ResourcePath { get; }

		[Export ("executablePath")]
		[NullAllowed]
		string ExecutablePath { get; }

		[Export ("pathForAuxiliaryExecutable:")]
		[return: NullAllowed]
		string PathForAuxiliaryExecutable (string s);


		[Export ("privateFrameworksPath")]
		[NullAllowed]
		string PrivateFrameworksPath { get; }

		[Export ("sharedFrameworksPath")]
		[NullAllowed]
		string SharedFrameworksPath { get; }

		[Export ("sharedSupportPath")]
		[NullAllowed]
		string SharedSupportPath { get; }

		[Export ("builtInPlugInsPath")]
		[NullAllowed]
		string BuiltinPluginsPath { get; }

		[Export ("bundleIdentifier")]
		[NullAllowed]
		string BundleIdentifier { get; }

		[Export ("classNamed:")]
		[return: NullAllowed]
		Class ClassNamed (string className);

		[Export ("principalClass")]
		[NullAllowed]
		Class PrincipalClass { get; }

		[Export ("pathForResource:ofType:inDirectory:")]
		[Static]
		[return: NullAllowed]
		string PathForResourceAbsolute ([NullAllowed] string name, [NullAllowed] string ofType, string bundleDirectory);

		[Export ("pathForResource:ofType:")]
		[return: NullAllowed]
		string PathForResource ([NullAllowed] string name, [NullAllowed] string ofType);

		[Export ("pathForResource:ofType:inDirectory:")]
		[return: NullAllowed]
		string PathForResource ([NullAllowed] string name, [NullAllowed] string ofType, [NullAllowed] string subpath);

		[Export ("pathForResource:ofType:inDirectory:forLocalization:")]
		[return: NullAllowed]
		string PathForResource ([NullAllowed] string name, [NullAllowed] string ofType, [NullAllowed] string subpath, [NullAllowed] string localizationName);

		/// <summary>Get a localized version of the string for the specified key in the specified table.</summary>
		/// <param name="key">The key to lookup</param>
		/// <param name="value">The value to return if the key is null, or the key was not found on the localization table.</param>
		/// <param name="table">The table to search, if the value is null, this uses the Localizable.strings table.</param>
		/// <summary>A localized version of the string for the specified key in the specified table.</summary>
		[Export ("localizedStringForKey:value:table:")]
		NSString GetLocalizedString ([NullAllowed] NSString key, [NullAllowed] NSString value, [NullAllowed] NSString table);

		[Export ("objectForInfoDictionaryKey:")]
		[return: NullAllowed]
		NSObject ObjectForInfoDictionary (string key);

		[Export ("developmentLocalization")]
		[NullAllowed]
		string DevelopmentLocalization { get; }

		[Export ("infoDictionary")]
		[NullAllowed]
		NSDictionary InfoDictionary { get; }

		// Additions from AppKit
		/// <param name="nibName">To be added.</param>
		///         <param name="owner">To be added.</param>
		///         <param name="topLevelObjects">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("loadNibNamed:owner:topLevelObjects:")]
		bool LoadNibNamed (string nibName, [NullAllowed] NSObject owner, out NSArray topLevelObjects);

		// https://developer.apple.com/library/mac/#documentation/Cocoa/Reference/ApplicationKit/Classes/NSBundle_AppKitAdditions/Reference/Reference.html
		/// <param name="nibName">To be added.</param>
		///         <param name="owner">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Static]
		[Deprecated (PlatformName.MacOSX, 10, 8)]
		[Export ("loadNibNamed:owner:")]
		bool LoadNib (string nibName, NSObject owner);

		/// <param name="resource">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("pathForImageResource:")]
		[return: NullAllowed]
		string PathForImageResource (string resource);

		/// <param name="resource">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("pathForSoundResource:")]
		[return: NullAllowed]
		string PathForSoundResource (string resource);

		/// <param name="resource">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("URLForImageResource:")]
		[return: NullAllowed]
		NSUrl GetUrlForImageResource (string resource);

		/// <param name="key">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("contextHelpForKey:")]
		[return: NullAllowed]
		NSAttributedString GetContextHelp (string key);

		// http://developer.apple.com/library/ios/#documentation/uikit/reference/NSBundle_UIKitAdditions/Introduction/Introduction.html
		/// <param name="nibName">To be added.</param>
		///         <param name="owner">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <param name="options">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		///         <summary>Loads the specified nib and returns the top-level objects. </summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("loadNibNamed:owner:options:")]
		[return: NullAllowed]
		NSArray LoadNib (string nibName, [NullAllowed] NSObject owner, [NullAllowed] NSDictionary options);

		[Export ("bundleURL")]
		NSUrl BundleUrl { get; }

		[Export ("resourceURL")]
		[NullAllowed]
		NSUrl ResourceUrl { get; }

		[Export ("executableURL")]
		[NullAllowed]
		NSUrl ExecutableUrl { get; }

		[Export ("URLForAuxiliaryExecutable:")]
		[return: NullAllowed]
		NSUrl UrlForAuxiliaryExecutable (string executable);

		[Export ("privateFrameworksURL")]
		[NullAllowed]
		NSUrl PrivateFrameworksUrl { get; }

		[Export ("sharedFrameworksURL")]
		[NullAllowed]
		NSUrl SharedFrameworksUrl { get; }

		[Export ("sharedSupportURL")]
		[NullAllowed]
		NSUrl SharedSupportUrl { get; }

		[Export ("builtInPlugInsURL")]
		[NullAllowed]
		NSUrl BuiltInPluginsUrl { get; }

		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[Static, Export ("bundleWithURL:")]
		[return: NullAllowed]
		NSBundle FromUrl (NSUrl url);

		[Export ("preferredLocalizations")]
		string [] PreferredLocalizations { get; }

		[Export ("localizations")]
		string [] Localizations { get; }

		[Export ("appStoreReceiptURL")]
		[NullAllowed]
		NSUrl AppStoreReceiptUrl { get; }

		[Export ("pathsForResourcesOfType:inDirectory:")]
		string [] PathsForResources ([NullAllowed] string fileExtension, [NullAllowed] string subDirectory);

		[Export ("pathsForResourcesOfType:inDirectory:forLocalization:")]
		string [] PathsForResources ([NullAllowed] string fileExtension, [NullAllowed] string subDirectory, [NullAllowed] string localizationName);

		[Static, Export ("pathsForResourcesOfType:inDirectory:")]
		string [] GetPathsForResources ([NullAllowed] string fileExtension, string bundlePath);

		[Static, Export ("URLForResource:withExtension:subdirectory:inBundleWithURL:")]
		[return: NullAllowed]
		NSUrl GetUrlForResource ([NullAllowed] string name, [NullAllowed] string fileExtension, [NullAllowed] string subdirectory, NSUrl bundleURL);

		[Static, Export ("URLsForResourcesWithExtension:subdirectory:inBundleWithURL:")]
		[return: NullAllowed]
		NSUrl [] GetUrlsForResourcesWithExtension ([NullAllowed] string fileExtension, [NullAllowed] string subdirectory, NSUrl bundleURL);

		[Export ("URLForResource:withExtension:")]
		[return: NullAllowed]
		NSUrl GetUrlForResource ([NullAllowed] string name, [NullAllowed] string fileExtension);

		[Export ("URLForResource:withExtension:subdirectory:")]
		[return: NullAllowed]
		NSUrl GetUrlForResource ([NullAllowed] string name, [NullAllowed] string fileExtension, [NullAllowed] string subdirectory);

		[Export ("URLForResource:withExtension:subdirectory:localization:")]
		[return: NullAllowed]
		NSUrl GetUrlForResource ([NullAllowed] string name, [NullAllowed] string fileExtension, [NullAllowed] string subdirectory, [NullAllowed] string localizationName);

		[Export ("URLsForResourcesWithExtension:subdirectory:")]
		[return: NullAllowed]
		NSUrl [] GetUrlsForResourcesWithExtension ([NullAllowed] string fileExtension, [NullAllowed] string subdirectory);

		[Export ("URLsForResourcesWithExtension:subdirectory:localization:")]
		[return: NullAllowed]
		NSUrl [] GetUrlsForResourcesWithExtension ([NullAllowed] string fileExtension, [NullAllowed] string subdirectory, [NullAllowed] string localizationName);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.iOS, 27, 0, message: "Use Background Assets instead.")]
		[Deprecated (PlatformName.TvOS, 27, 0, message: "Use Background Assets instead.")]
		[Deprecated (PlatformName.MacCatalyst, 27, 0, message: "Use Background Assets instead.")]
		[Export ("preservationPriorityForTag:")]
		double GetPreservationPriority (NSString tag);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.iOS, 27, 0, message: "Use Background Assets instead.")]
		[Deprecated (PlatformName.TvOS, 27, 0, message: "Use Background Assets instead.")]
		[Deprecated (PlatformName.MacCatalyst, 27, 0, message: "Use Background Assets instead.")]
		[Export ("setPreservationPriority:forTags:")]
		void SetPreservationPriority (double priority, NSSet<NSString> tags);

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("localizedAttributedStringForKey:value:table:")]
		NSAttributedString GetLocalizedAttributedString (string key, [NullAllowed] string value, [NullAllowed] string tableName);

		[Notification]
		[Field ("NSBundleDidLoadNotification")]
		NSString BundleDidLoadNotification { get; }

		// bound using NSString instead of string, because often localized NSString has extra information we can't marshal to a C# string
		[TV (18, 4), iOS (18, 4), MacCatalyst (18, 4), Mac (15, 4)]
		[Export ("localizedStringForKey:value:table:localizations:")]
		NSString GetLocalizedString (NSString key, [NullAllowed] NSString value, [NullAllowed] NSString tableName, NSString [] localizations);
	}

	[NoMac]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.iOS, 27, 0, message: "Use Background Assets instead.")]
	[Deprecated (PlatformName.TvOS, 27, 0, message: "Use Background Assets instead.")]
	[Deprecated (PlatformName.MacCatalyst, 27, 0, message: "Use Background Assets instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSBundleResourceRequest : NSProgressReporting {
		[Export ("initWithTags:")]
		NativeHandle Constructor (NSSet<NSString> tags);

		[Export ("initWithTags:bundle:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSSet<NSString> tags, NSBundle bundle);

		[Export ("loadingPriority")]
		double LoadingPriority { get; set; }

		[Export ("tags", ArgumentSemantic.Copy)]
		NSSet<NSString> Tags { get; }

		[Export ("bundle", ArgumentSemantic.Strong)]
		NSBundle Bundle { get; }

		[Export ("beginAccessingResourcesWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous BeginAccessingResources operation</returns>
			<remarks>To be added.</remarks>
			""")]
		void BeginAccessingResources (Action<NSError> completionHandler);

		[Export ("conditionallyBeginAccessingResourcesWithCompletionHandler:")]
		[Async (XmlDocs = """
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous ConditionallyBeginAccessingResources operation.  The value of the TResult parameter is of type System.Action&lt;System.Boolean&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The ConditionallyBeginAccessingResourcesAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		void ConditionallyBeginAccessingResources (Action<bool> completionHandler);

		[Export ("endAccessingResources")]
		void EndAccessingResources ();

		[Field ("NSBundleResourceRequestLowDiskSpaceNotification")]
		[Notification]
		NSString LowDiskSpaceNotification { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSBundleResourceRequestLoadingPriorityUrgent")]
		double LoadingPriorityUrgent { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSIndexPath : NSCoding, NSSecureCoding, NSCopying {
		/// <summary>Create a new index-path object with the specified object.</summary>
		/// <param name="index">Object to place in the index-path.</param>
		[Export ("indexPathWithIndex:")]
		[Static]
		NSIndexPath FromIndex (nuint index);

		[Export ("indexPathWithIndexes:length:")]
		[Internal]
		[Static]
		NSIndexPath _FromIndex (IntPtr indexes, nint len);

		/// <summary>Returns a new index-path containing those in this object plus the new <paramref name="index" />.</summary>
		/// <param name="index">Index to be appended.</param>
		[Export ("indexPathByAddingIndex:")]
		NSIndexPath IndexPathByAddingIndex (nuint index);

		[Export ("indexPathByRemovingLastIndex")]
		NSIndexPath IndexPathByRemovingLastIndex ();

		/// <summary>Return the index at the given <paramref name="position" /> in the index-path.</summary>
		/// <param name="position">Position of index to return.</param>
		[Export ("indexAtPosition:")]
		nuint IndexAtPosition (nint position);

		[Export ("length")]
		nint Length { get; }

		[Export ("getIndexes:")]
		[Internal]
		void _GetIndexes (IntPtr target);

		[MacCatalyst (13, 1)]
		[Export ("getIndexes:range:")]
		[Internal]
		void _GetIndexes (IntPtr target, NSRange positionRange);

		[Export ("compare:")]
		nint Compare (NSIndexPath other);

		// NSIndexPath UIKit Additions Reference
		// https://developer.apple.com/library/ios/#documentation/UIKit/Reference/NSIndexPath_UIKitAdditions/Reference/Reference.html

		// see monotouch/src/UIKit/Addition.cs for int-returning Row/Section properties
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("row")]
		nint LongRow { get; }

		/// <summary>The index of a section within a <see cref="UIKit.UITableView" /> (read-only).</summary>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("section")]
		nint LongSection { get; }

		/// <summary>Returns an index-path object initialized with the given row and section details.</summary>
		/// <param name="row">The row index within the corresponding <paramref name="section" /> of a <see cref="UIKit.UITableView" />.</param>
		/// <param name="section">The index of the section in the <see cref="UIKit.UITableView" /> that contains the <paramref name="row" />.</param>
		/// <returns>An <see cref="Foundation.NSIndexPath" /> object, or <see langword="null" /> if it could not be created.</returns>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("indexPathForRow:inSection:")]
		NSIndexPath FromRowSection (nint row, nint section);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("section")]
		nint Section { get; }

		/// <summary>Creates an <see cref="NSIndexPath" /> from the given item and section values.</summary>
		/// <param name="item">Item value.</param>
		/// <param name="section">Section value.</param>
		/// <returns>New instance of the <see cref="NSIndexPath" />.</returns>
		[Static]
		[MacCatalyst (13, 1)]
		[Export ("indexPathForItem:inSection:")]
		NSIndexPath FromItemSection (nint item, nint section);

		/// <summary>The item component of this <see cref="NSIndexPath" />.</summary>
		[Export ("item")]
		[MacCatalyst (13, 1)]
		nint Item { get; }
	}

	/// <param name="range">To be added.</param>
	/// <param name="stop">To be added.</param>
	/// <summary>A delegate used to specify the iterator used by <see cref="Foundation.NSIndexSet.EnumerateRanges(Foundation.NSRange,Foundation.NSEnumerationOptions,Foundation.NSRangeIterator)" />.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSRangeIterator (NSRange range, ref bool stop);

	[BaseType (typeof (NSObject))]
	interface NSIndexSet : NSCoding, NSSecureCoding, NSMutableCopying {
		/// <param name="idx">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static, Export ("indexSetWithIndex:")]
		NSIndexSet FromIndex (nint idx);

		[Static, Export ("indexSetWithIndexesInRange:")]
		NSIndexSet FromNSRange (NSRange indexRange);

		[Export ("initWithIndex:")]
		NativeHandle Constructor (nuint index);

		[DesignatedInitializer]
		[Export ("initWithIndexSet:")]
		NativeHandle Constructor (NSIndexSet other);

		[Export ("count")]
		nint Count { get; }

		[Export ("isEqualToIndexSet:")]
		bool IsEqual (NSIndexSet other);

		[Export ("firstIndex")]
		nuint FirstIndex { get; }

		[Export ("lastIndex")]
		nuint LastIndex { get; }

		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("indexGreaterThanIndex:")]
		nuint IndexGreaterThan (nuint index);

		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("indexLessThanIndex:")]
		nuint IndexLessThan (nuint index);

		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("indexGreaterThanOrEqualToIndex:")]
		nuint IndexGreaterThanOrEqual (nuint index);

		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("indexLessThanOrEqualToIndex:")]
		nuint IndexLessThanOrEqual (nuint index);

		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("containsIndex:")]
		bool Contains (nuint index);

		[Export ("containsIndexes:")]
		bool Contains (NSIndexSet indexes);

		[Export ("enumerateRangesUsingBlock:")]
		void EnumerateRanges (NSRangeIterator iterator);

		[Export ("enumerateRangesWithOptions:usingBlock:")]
		void EnumerateRanges (NSEnumerationOptions opts, NSRangeIterator iterator);

		[Export ("enumerateRangesInRange:options:usingBlock:")]
		void EnumerateRanges (NSRange range, NSEnumerationOptions opts, NSRangeIterator iterator);

		[Export ("enumerateIndexesUsingBlock:")]
		void EnumerateIndexes (EnumerateIndexSetCallback enumeratorCallback);

		[Export ("enumerateIndexesWithOptions:usingBlock:")]
		void EnumerateIndexes (NSEnumerationOptions opts, EnumerateIndexSetCallback enumeratorCallback);

		[Export ("enumerateIndexesInRange:options:usingBlock:")]
		void EnumerateIndexes (NSRange range, NSEnumerationOptions opts, EnumerateIndexSetCallback enumeratorCallback);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // from the docs: " you should not create these objects using alloc and init."
	interface NSInvocation {

		[Export ("selector")]
		Selector Selector { get; set; }

		[Export ("target", ArgumentSemantic.Assign), NullAllowed]
		NSObject Target { get; set; }

		// FIXME: We need some special marshaling support to handle these buffers...
		[Internal, Export ("setArgument:atIndex:")]
		void _SetArgument (IntPtr buffer, nint index);

		[Internal, Export ("getArgument:atIndex:")]
		void _GetArgument (IntPtr buffer, nint index);

		[Internal, Export ("setReturnValue:")]
		void _SetReturnValue (IntPtr buffer);

		[Internal, Export ("getReturnValue:")]
		void _GetReturnValue (IntPtr buffer);

		[Export ("invoke")]
		void Invoke ();

		[Export ("invokeWithTarget:")]
		void Invoke (NSObject target);

		[Export ("methodSignature")]
		NSMethodSignature MethodSignature { get; }
	}


	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	partial interface NSItemProvider : NSCopying {
		[Deprecated (PlatformName.MacOSX, 27, 0, message: "Use the 'NSItemProvider (INSItemProviderWriting)' constructor instead.")]
		[Deprecated (PlatformName.iOS, 27, 0, message: "Use the 'NSItemProvider (INSItemProviderWriting)' constructor instead.")]
		[Deprecated (PlatformName.TvOS, 27, 0, message: "Use the 'NSItemProvider (INSItemProviderWriting)' constructor instead.")]
		[Deprecated (PlatformName.MacCatalyst, 27, 0, message: "Use the 'NSItemProvider (INSItemProviderWriting)' constructor instead.")]
		[DesignatedInitializer]
		[Export ("initWithItem:typeIdentifier:")]
		NativeHandle Constructor ([NullAllowed] NSObject item, string typeIdentifier);

		[Export ("initWithContentsOfURL:")]
		NativeHandle Constructor (NSUrl fileUrl);

		[Export ("registeredTypeIdentifiers", ArgumentSemantic.Copy)]
		string [] RegisteredTypeIdentifiers { get; }

		[Deprecated (PlatformName.MacOSX, 27, 0, message: "Use the 'RegisterObject' method instead.")]
		[Deprecated (PlatformName.iOS, 27, 0, message: "Use the 'RegisterObject' method instead.")]
		[Deprecated (PlatformName.TvOS, 27, 0, message: "Use the 'RegisterObject' method instead.")]
		[Deprecated (PlatformName.MacCatalyst, 27, 0, message: "Use the 'RegisterObject' method instead.")]
		[Export ("registerItemForTypeIdentifier:loadHandler:")]
		void RegisterItemForTypeIdentifier (string typeIdentifier, NSItemProviderLoadHandler loadHandler);

		[Export ("hasItemConformingToTypeIdentifier:")]
		bool HasItemConformingTo (string typeIdentifier);

		[Deprecated (PlatformName.MacOSX, 27, 0, message: "Use the 'LoadObject' method instead.")]
		[Deprecated (PlatformName.iOS, 27, 0, message: "Use the 'LoadObject' method instead.")]
		[Deprecated (PlatformName.TvOS, 27, 0, message: "Use the 'LoadObject' method instead.")]
		[Deprecated (PlatformName.MacCatalyst, 27, 0, message: "Use the 'LoadObject' method instead.")]
		[Async (XmlDocs = """
			<param name="typeIdentifier">To be added.</param>
			<param name="options">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadItem operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSObject,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("loadItemForTypeIdentifier:options:completionHandler:")]
		void LoadItem (string typeIdentifier, [NullAllowed] NSDictionary options, [NullAllowed] Action<NSObject, NSError> completionHandler);

		/// <summary>Represents the value associated with the constant NSItemProviderPreferredImageSizeKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSItemProviderPreferredImageSizeKey")]
		NSString PreferredImageSizeKey { get; }

		/// <param name="handler">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("setPreviewImageHandler:")]
		void SetPreviewImageHandler (NSItemProviderLoadHandler handler);

		[Async (XmlDocs = """
			<param name="options">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadPreviewImage operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSObject,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("loadPreviewImageWithOptions:completionHandler:")]
		void LoadPreviewImage (NSDictionary options, Action<NSObject, NSError> completionHandler);

		/// <summary>Represents the value associated with the constant NSItemProviderErrorDomain</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSItemProviderErrorDomain")]
		NSString ErrorDomain { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("sourceFrame")]
		CGRect SourceFrame { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("containerFrame")]
		CGRect ContainerFrame { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("preferredPresentationSize")]
		CGSize PreferredPresentationSize {
			get;
			[NoMac]
			[MacCatalyst (13, 1)]
			set;
		}

		/// <param name="preparationHandler">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("registerCloudKitShareWithPreparationHandler:")]
		void RegisterCloudKitShare (CloudKitRegistrationPreparationAction preparationHandler);

		/// <param name="share">To be added.</param>
		///         <param name="container">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS, NoTV, NoMacCatalyst]
		[Export ("registerCloudKitShare:container:")]
		void RegisterCloudKitShare (CKShare share, CKContainer container);

		[MacCatalyst (13, 1)]
		[Export ("registerDataRepresentationForTypeIdentifier:visibility:loadHandler:")]
		void RegisterDataRepresentation (string typeIdentifier, NSItemProviderRepresentationVisibility visibility, RegisterDataRepresentationLoadHandler loadHandler);

		[MacCatalyst (13, 1)]
		[Export ("registerFileRepresentationForTypeIdentifier:fileOptions:visibility:loadHandler:")]
		void RegisterFileRepresentation (string typeIdentifier, NSItemProviderFileOptions fileOptions, NSItemProviderRepresentationVisibility visibility, RegisterFileRepresentationLoadHandler loadHandler);

		[MacCatalyst (13, 1)]
		[Export ("registeredTypeIdentifiersWithFileOptions:")]
		string [] GetRegisteredTypeIdentifiers (NSItemProviderFileOptions fileOptions);

		[MacCatalyst (13, 1)]
		[Export ("hasRepresentationConformingToTypeIdentifier:fileOptions:")]
		bool HasConformingRepresentation (string typeIdentifier, NSItemProviderFileOptions fileOptions);

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="typeIdentifier">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadDataRepresentation operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSData,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="typeIdentifier">To be added.</param>
			<param name="result">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			"""), Export ("loadDataRepresentationForTypeIdentifier:completionHandler:")]
		NSProgress LoadDataRepresentation (string typeIdentifier, Action<NSData, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="typeIdentifier">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadFileRepresentation operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSUrl,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="typeIdentifier">To be added.</param>
			<param name="result">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			"""), Export ("loadFileRepresentationForTypeIdentifier:completionHandler:")]
		NSProgress LoadFileRepresentation (string typeIdentifier, Action<NSUrl, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Async (ResultTypeName = "LoadInPlaceResult", XmlDocs = """
			<param name="typeIdentifier">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous LoadInPlaceFileRepresentation operation.   The value of the TResult parameter is of type Foundation.LoadInPlaceResult.</para>
			        </returns>
			<remarks>To be added.</remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="typeIdentifier">To be added.</param>
			<param name="result">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			"""), Export ("loadInPlaceFileRepresentationForTypeIdentifier:completionHandler:")]
		NSProgress LoadInPlaceFileRepresentation (string typeIdentifier, LoadInPlaceFileRepresentationHandler completionHandler);

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("suggestedName")]
		string SuggestedName { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("initWithObject:")]
		NativeHandle Constructor (INSItemProviderWriting @object);

		[MacCatalyst (13, 1)]
		[Export ("registerObject:visibility:")]
		void RegisterObject (INSItemProviderWriting @object, NSItemProviderRepresentationVisibility visibility);

		[MacCatalyst (13, 1)]
		[Export ("registerObjectOfClass:visibility:loadHandler:")]
		void RegisterObject (Class aClass, NSItemProviderRepresentationVisibility visibility, RegisterObjectRepresentationLoadHandler loadHandler);

		/// <param name="type">To be added.</param>
		///         <param name="visibility">To be added.</param>
		///         <param name="loadHandler">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("RegisterObject (new Class (type), visibility, loadHandler)")]
		void RegisterObject (Type type, NSItemProviderRepresentationVisibility visibility, RegisterObjectRepresentationLoadHandler loadHandler);

		[MacCatalyst (13, 1)]
		[Export ("canLoadObjectOfClass:")]
		bool CanLoadObject (Class aClass);

		/// <param name="type">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Wrap ("CanLoadObject (new Class (type))")]
		bool CanLoadObject (Type type);

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="aClass">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous LoadObject operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.INSItemProviderReading,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The LoadObjectAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""",
			XmlDocsWithOutParameter = """
			<param name="aClass">To be added.</param>
			<param name="result">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			"""), Export ("loadObjectOfClass:completionHandler:")]
		NSProgress LoadObject (Class aClass, Action<INSItemProviderReading, NSError> completionHandler);

		// NSItemProvider_UIKitAdditions category

		/// <summary>To be added.</summary>
		///         <value>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="nullallowed">This value can be <see langword="null" />.</para>
		///         </value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("teamData", ArgumentSemantic.Copy)]
		NSData TeamData { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoTV]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("preferredPresentationStyle", ArgumentSemantic.Assign)]
		UIPreferredPresentationStyle PreferredPresentationStyle { get; set; }

		// extension methods from CloudKit

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registerCKShareWithContainer:allowedSharingOptions:preparationHandler:")]
		void RegisterCKShare (CKContainer container, CKAllowedSharingOptions allowedOptions, Action preparationHandler);

		[NoTV, iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registerCKShare:container:allowedSharingOptions:")]
		void RegisterCKShare (CKShare share, CKContainer container, CKAllowedSharingOptions allowedOptions);

		// from interface UTType (NSItemProvider)

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithContentsOfURL:contentType:openInPlace:coordinated:visibility:")]
		NativeHandle Constructor (NSUrl fileUrl, [NullAllowed] UTType contentType, bool openInPlace, bool coordinated, NSItemProviderRepresentationVisibility visibility);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registerDataRepresentationForContentType:visibility:loadHandler:")]
		void RegisterDataRepresentation (UTType contentType, NSItemProviderRepresentationVisibility visibility, NSItemProviderUTTypeLoadDelegate loadHandler);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registerFileRepresentationForContentType:visibility:openInPlace:loadHandler:")]
		void RegisterFileRepresentation (UTType contentType, NSItemProviderRepresentationVisibility visibility, bool openInPlace, NSItemProviderUTTypeLoadDelegate loadHandler);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registeredContentTypes", ArgumentSemantic.Copy)]
		UTType [] RegisteredContentTypes { get; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registeredContentTypesForOpenInPlace", ArgumentSemantic.Copy)]
		UTType [] RegisteredContentTypesForOpenInPlace { get; }

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registeredContentTypesConformingToContentType:")]
		UTType [] RegisteredContentTypesConforming (UTType contentType);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("loadDataRepresentationForContentType:completionHandler:")]
		NSProgress LoadDataRepresentation (UTType contentType, ItemProviderDataCompletionHandler completionHandler);

		[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("loadFileRepresentationForContentType:openInPlace:completionHandler:")]
		NSProgress LoadFileRepresentation (UTType contentType, bool openInPlace, LoadFileRepresentationHandler completionHandler);
	}

	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	delegate NSProgress NSItemProviderUTTypeLoadDelegate ([BlockCallback] ItemProviderDataCompletionHandler completionHandler);
	[TV (16, 0), iOS (16, 0), MacCatalyst (16, 0)]
	delegate void LoadFileRepresentationHandler ([NullAllowed] NSUrl fileUrl, bool openInPlace, [NullAllowed] NSError error);
	delegate NSProgress RegisterFileRepresentationLoadHandler ([BlockCallback] RegisterFileRepresentationCompletionHandler completionHandler);
	delegate void RegisterFileRepresentationCompletionHandler (NSUrl fileUrl, bool coordinated, NSError error);
	delegate void ItemProviderDataCompletionHandler ([NullAllowed] NSData data, [NullAllowed] NSError error);
	delegate NSProgress RegisterDataRepresentationLoadHandler ([BlockCallback] ItemProviderDataCompletionHandler completionHandler);
	delegate void LoadInPlaceFileRepresentationHandler ([NullAllowed] NSUrl fileUrl, bool isInPlace, [NullAllowed] NSError error);
	delegate NSProgress RegisterObjectRepresentationLoadHandler ([BlockCallback] RegisterObjectRepresentationCompletionHandler completionHandler);
	delegate void RegisterObjectRepresentationCompletionHandler (INSItemProviderWriting @object, NSError error);

	interface INSItemProviderReading { }

	[MacCatalyst (13, 1)]
	[Protocol]
	interface NSItemProviderReading {
		// This static method has to be implemented on each class that implements
		// this, this is not a capability that exists in C#.
		// We are inlining these on each class that implements NSItemProviderReading
		// for the sake of the method being callable from C#, for user code, the
		// user needs to manually [Export] the selector on a static method, like
		// they do for the "layer" property on CALayer subclasses.
		//
		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Static, Abstract]
		[Export ("readableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
		string [] ReadableTypeIdentifiers { get; }

		/// <param name="data">To be added.</param>
		/// <param name="typeIdentifier">To be added.</param>
		/// <param name="outError">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static, Abstract]
		[Export ("objectWithItemProviderData:typeIdentifier:error:")]
		[return: NullAllowed]
		INSItemProviderReading GetObject (NSData data, string typeIdentifier, [NullAllowed] out NSError outError);
	}

	interface INSItemProviderWriting { }

	/// <summary>Interface used by <see cref="Foundation.NSItemProvider" /> for retrieving data from an object.</summary>
	/// <remarks>To be added.</remarks>
	[MacCatalyst (13, 1)]
	[Protocol]
	interface NSItemProviderWriting {
		//
		// This static method has to be implemented on each class that implements
		// this, this is not a capability that exists in C#.
		// We are inlining these on each class that implements NSItemProviderWriting
		// for the sake of the method being callable from C#, for user code, the
		// user needs to manually [Export] the selector on a static method, like
		// they do for the "layer" property on CALayer subclasses.
		//
		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Static, Abstract]
		[Export ("writableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
		string [] WritableTypeIdentifiers { get; }

		// This is an optional method, which means the generator will inline it in any classes
		// that implements this interface. Unfortunately none of the native classes that implements
		// the protocol actually implements this method, which means that inlining the method will cause
		// introspection to complain (rightly). So comment out this method to avoid generator a lot of unusable API.
		// See also https://bugzilla.xamarin.com/show_bug.cgi?id=59308.
		//
		// [Static]
		// [Export ("itemProviderVisibilityForRepresentationWithTypeIdentifier:")]
		// NSItemProviderRepresentationVisibility GetItemProviderVisibility (string typeIdentifier);

		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[Export ("writableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
		// 'WritableTypeIdentifiers' is a nicer name, but there's a static property with that name.
		string [] WritableTypeIdentifiersForItemProvider { get; }

		/// <param name="typeIdentifier">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("itemProviderVisibilityForRepresentationWithTypeIdentifier:")]
		// 'GetItemProviderVisibility' is a nicer name, but there's a static method with that name.
		NSItemProviderRepresentationVisibility GetItemProviderVisibilityForTypeIdentifier (string typeIdentifier);

		/// <param name="typeIdentifier">A Universal Type Identifier (UTI) indicating the type of data to load.</param>
		/// <param name="completionHandler">The method called after the data is loaded.</param>
		/// <summary>Implement this method to customize the loading of data by an <see cref="Foundation.NSItemProvider" />.</summary>
		/// <returns>An <see cref="NSProgress" /> object reflecting the data-loading operation.</returns>
		/// <remarks>
		///           <para>The <paramref name="typeIdentifier" /> must be in the set of values returned by <see cref="Foundation.NSItemProviderWriting_Extensions.GetWritableTypeIdentifiersForItemProvider(Foundation.INSItemProviderWriting)" />.</para>
		///         </remarks>
		[Abstract]
		[Async (XmlDocs = """
			<param name="typeIdentifier">A Universal Type Identifier (UTI) indicating the type of data to load.</param>
			<summary>Asynchronously loads data for the identified type from an item provider, returning a task that contains the data.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			"""), Export ("loadDataWithTypeIdentifier:forItemProviderCompletionHandler:")]
		[return: NullAllowed]
		NSProgress LoadData (string typeIdentifier, Action<NSData, NSError> completionHandler);
	}

	/// <summary>Defines the strings associated with the constants <c>NSExtensionJavaScriptFinalizeArgumentKey</c> and <c>NSExtensionJavaScriptPreprocessingResultsKey</c>.</summary>
	/// <remarks>To be added.</remarks>
	[Static]
	[MacCatalyst (13, 1)]
	partial interface NSJavaScriptExtension {
		/// <summary>A key whose value is an <see cref="Foundation.NSDictionary" /> whose contents are the argument to a JavaScript completion function.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSExtensionJavaScriptPreprocessingResultsKey")]
		NSString PreprocessingResultsKey { get; }

		/// <summary>A key whose value is an <see cref="Foundation.NSDictionary" /> whose contents are the arguments to a JavaScript finalize method.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSExtensionJavaScriptFinalizeArgumentKey")]
		NSString FinalizeArgumentKey { get; }
	}

	[MacCatalyst (13, 1)]
	interface NSTypeIdentifier {
		[Field ("NSTypeIdentifierDateText")]
		NSString DateText { get; }

		[Field ("NSTypeIdentifierAddressText")]
		NSString AddressText { get; }

		[Field ("NSTypeIdentifierPhoneNumberText")]
		NSString PhoneNumberText { get; }

		[Field ("NSTypeIdentifierTransitInformationText")]
		NSString TransitInformationText { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // `init` returns a null handle
	interface NSMethodSignature {
		[Static]
		[Export ("signatureWithObjCTypes:")]
		NSMethodSignature FromObjcTypes (IntPtr utf8objctypes);

		[Export ("numberOfArguments")]
		nuint NumberOfArguments { get; }

		[Export ("frameLength")]
		nuint FrameLength { get; }

		[Export ("methodReturnLength")]
		nuint MethodReturnLength { get; }

		[Export ("isOneway")]
		bool IsOneway { get; }

		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("getArgumentTypeAtIndex:")]
		IntPtr GetArgumentType (nuint index);

		[Export ("methodReturnType")]
		IntPtr MethodReturnType { get; }
	}

	[BaseType (typeof (NSObject), Name = "NSJSONSerialization")]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** +[NSJSONSerialization allocWithZone:]: Do not create instances of NSJSONSerialization in this release
	[DisableDefaultCtor]
	interface NSJsonSerialization {
		[Static]
		[Export ("isValidJSONObject:")]
		bool IsValidJSONObject (NSObject obj);

		[Static]
		[Export ("dataWithJSONObject:options:error:")]
		NSData Serialize (NSObject obj, NSJsonWritingOptions opt, out NSError error);

		[Static]
		[Export ("JSONObjectWithData:options:error:")]
		NSObject Deserialize (NSData data, NSJsonReadingOptions opt, out NSError error);

		[Static]
		[Export ("writeJSONObject:toStream:options:error:")]
		nint Serialize (NSObject obj, NSOutputStream stream, NSJsonWritingOptions opt, out NSError error);

		[Static]
		[Export ("JSONObjectWithStream:options:error:")]
		NSObject Deserialize (NSInputStream stream, NSJsonReadingOptions opt, out NSError error);

	}

	[BaseType (typeof (NSIndexSet))]
	interface NSMutableIndexSet : NSSecureCoding {
		/// <param name="index">To be added.</param>
		/// <summary>Initializes a new index set from an existing unsigned integer.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("initWithIndex:")]
		NativeHandle Constructor (nuint index);

		[Export ("initWithIndexSet:")]
		NativeHandle Constructor (NSIndexSet other);

		[Export ("addIndexes:")]
		void Add (NSIndexSet other);

		[Export ("removeIndexes:")]
		void Remove (NSIndexSet other);

		[Export ("removeAllIndexes")]
		void Clear ();

		/// <param name="index">The index to add.</param>
		/// <summary>Adds a single index to the existing set.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("addIndex:")]
		void Add (nuint index);

		/// <param name="index">The index to remove.</param>
		/// <summary>Removes a single index from the collection.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("removeIndex:")]
		void Remove (nuint index);

		/// <param name="startIndex">To be added.</param>
		/// <param name="delta">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("shiftIndexesStartingAtIndex:by:")]
		void ShiftIndexes (nuint startIndex, nint delta);

		[Export ("addIndexesInRange:")]
		void AddIndexesInRange (NSRange range);

		[Export ("removeIndexesInRange:")]
		void RemoveIndexesInRange (NSRange range);
	}

	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the Network.framework instead.")]
	[Deprecated (PlatformName.iOS, 15, 0, message: "Use the Network.framework instead.")]
	[Deprecated (PlatformName.TvOS, 15, 0, message: "Use the Network.framework instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the Network.framework instead.")]
	[DisableDefaultCtor] // the instance just crash when trying to call selectors
	[BaseType (typeof (NSObject), Delegates = new string [] { "WeakDelegate" }, Events = new Type [] { typeof (NSNetServiceDelegate) })]
	interface NSNetService {
		[DesignatedInitializer]
		[Export ("initWithDomain:type:name:port:")]
		NativeHandle Constructor (string domain, string type, string name, int /* int, not NSInteger */ port);

		[Export ("initWithDomain:type:name:")]
		NativeHandle Constructor (string domain, string type, string name);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the Foundation.INSNetServiceDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSNetServiceDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		INSNetServiceDelegate Delegate { get; set; }

		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, NSString forMode);

		// For consistency with other APIs (NSUrlConnection) we call this Unschedule
		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, NSString forMode);

		/// <param name="aRunLoop">To be added.</param>
		///         <param name="forMode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("Schedule (aRunLoop, forMode.GetConstant ()!)")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

		/// <param name="aRunLoop">To be added.</param>
		///         <param name="forMode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("Unschedule (aRunLoop, forMode.GetConstant ()!)")]
		void Unschedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

		[Export ("domain", ArgumentSemantic.Copy)]
		string Domain { get; }

		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; }

		[NullAllowed]
		[Export ("addresses", ArgumentSemantic.Copy)]
		NSData [] Addresses { get; }

		[Export ("port")]
		nint Port { get; }

		[Export ("publish")]
		void Publish ();

		[Export ("publishWithOptions:")]
		void Publish (NSNetServiceOptions options);

		[Export ("resolve")]
		[Deprecated (PlatformName.iOS, 2, 0, message: "Use 'Resolve (double)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 4, message: "Use 'Resolve (double)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Resolve (double)' instead.")]
		void Resolve ();

		[Export ("resolveWithTimeout:")]
		void Resolve (double timeOut);

		[Export ("stop")]
		void Stop ();

		[Static, Export ("dictionaryFromTXTRecordData:")]
		NSDictionary DictionaryFromTxtRecord (NSData data);

		[Static, Export ("dataFromTXTRecordDictionary:")]
		NSData DataFromTxtRecord (NSDictionary dictionary);

		[NullAllowed]
		[Export ("hostName", ArgumentSemantic.Copy)]
		string HostName { get; }

		[Export ("getInputStream:outputStream:")]
		bool GetStreams (out NSInputStream inputStream, out NSOutputStream outputStream);

		[return: NullAllowed]
		[Export ("TXTRecordData")]
		NSData GetTxtRecordData ();

		[Export ("setTXTRecordData:")]
		bool SetTxtRecordData ([NullAllowed] NSData data);

		//NSData TxtRecordData { get; set; }

		[Export ("startMonitoring")]
		void StartMonitoring ();

		[Export ("stopMonitoring")]
		void StopMonitoring ();

		[MacCatalyst (13, 1)]
		[Export ("includesPeerToPeer")]
		bool IncludesPeerToPeer { get; set; }
	}

	interface INSNetServiceDelegate { }

	[MacCatalyst (13, 1)]
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	interface NSNetServiceDelegate {
		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("netServiceWillPublish:")]
		void WillPublish (NSNetService sender);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("netServiceDidPublish:")]
		void Published (NSNetService sender);

		/// <param name="sender">To be added.</param>
		/// <param name="errors">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("netService:didNotPublish:"), EventArgs ("NSNetServiceError", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void PublishFailure (NSNetService sender, NSDictionary errors);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("netServiceWillResolve:")]
		void WillResolve (NSNetService sender);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("netServiceDidResolveAddress:")]
		void AddressResolved (NSNetService sender);

		/// <param name="sender">To be added.</param>
		/// <param name="errors">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("netService:didNotResolve:"), EventArgs ("NSNetServiceError", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void ResolveFailure (NSNetService sender, NSDictionary errors);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("netServiceDidStop:")]
		void Stopped (NSNetService sender);

		/// <param name="sender">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("netService:didUpdateTXTRecordData:"), EventArgs ("NSNetServiceData", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void UpdatedTxtRecordData (NSNetService sender, NSData data);

		/// <param name="sender">To be added.</param>
		/// <param name="inputStream">To be added.</param>
		/// <param name="outputStream">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("netService:didAcceptConnectionWithInputStream:outputStream:"), EventArgs ("NSNetServiceConnection", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void DidAcceptConnection (NSNetService sender, NSInputStream inputStream, NSOutputStream outputStream);
	}

	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the Network.framework instead.")]
	[Deprecated (PlatformName.iOS, 15, 0, message: "Use the Network.framework instead.")]
	[Deprecated (PlatformName.TvOS, 15, 0, message: "Use the Network.framework instead.")]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the Network.framework instead.")]
	[BaseType (typeof (NSObject),
		   Delegates = new string [] { "WeakDelegate" },
		   Events = new Type [] { typeof (NSNetServiceBrowserDelegate) })]
	interface NSNetServiceBrowser {
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the Foundation.INSNetServiceBrowserDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSNetServiceBrowserDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		INSNetServiceBrowserDelegate Delegate { get; set; }

		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, NSString forMode);

		// For consistency with other APIs (NSUrlConnection) we call this Unschedule
		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, NSString forMode);

		/// <param name="aRunLoop">To be added.</param>
		///         <param name="forMode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("Schedule (aRunLoop, forMode.GetConstant ()!)")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

		/// <param name="aRunLoop">To be added.</param>
		///         <param name="forMode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("Unschedule (aRunLoop, forMode.GetConstant ()!)")]
		void Unschedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

		[Export ("searchForBrowsableDomains")]
		void SearchForBrowsableDomains ();

		[Export ("searchForRegistrationDomains")]
		void SearchForRegistrationDomains ();

		[Export ("searchForServicesOfType:inDomain:")]
		void SearchForServices (string type, string domain);

		[Export ("stop")]
		void Stop ();

		[MacCatalyst (13, 1)]
		[Export ("includesPeerToPeer")]
		bool IncludesPeerToPeer { get; set; }
	}

	interface INSNetServiceBrowserDelegate { }

	[MacCatalyst (13, 1)]
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	interface NSNetServiceBrowserDelegate {
		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("netServiceBrowserWillSearch:")]
		void SearchStarted (NSNetServiceBrowser sender);

		/// <param name="sender">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		[Export ("netServiceBrowserDidStopSearch:")]
		void SearchStopped (NSNetServiceBrowser sender);

		/// <param name="sender">To be added.</param>
		/// <param name="errors">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("netServiceBrowser:didNotSearch:"), EventArgs ("NSNetServiceError", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void NotSearched (NSNetServiceBrowser sender, NSDictionary errors);

		/// <param name="sender">To be added.</param>
		/// <param name="domain">To be added.</param>
		/// <param name="moreComing">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("netServiceBrowser:didFindDomain:moreComing:"), EventArgs ("NSNetDomain", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void FoundDomain (NSNetServiceBrowser sender, string domain, bool moreComing);

		/// <param name="sender">To be added.</param>
		/// <param name="service">To be added.</param>
		/// <param name="moreComing">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("netServiceBrowser:didFindService:moreComing:"), EventArgs ("NSNetService", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void FoundService (NSNetServiceBrowser sender, NSNetService service, bool moreComing);

		/// <param name="sender">To be added.</param>
		/// <param name="domain">To be added.</param>
		/// <param name="moreComing">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("netServiceBrowser:didRemoveDomain:moreComing:"), EventArgs ("NSNetDomain", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void DomainRemoved (NSNetServiceBrowser sender, string domain, bool moreComing);

		/// <param name="sender">To be added.</param>
		/// <param name="service">To be added.</param>
		/// <param name="moreComing">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("netServiceBrowser:didRemoveService:moreComing:"), EventArgs ("NSNetService", XmlDocs = """
			<summary>Event raised by the object.</summary>
			<remarks>If developers do not assign a value to this event, this will reset the value for the WeakDelegate property to an internal handler that maps delegates to events.</remarks>
			""")]
		void ServiceRemoved (NSNetServiceBrowser sender, NSNetService service, bool moreComing);
	}

	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: *** -[NSConcreteNotification init]: should never be used
	[DisableDefaultCtor] // added in iOS7 but header files says "do not invoke; not a valid initializer for this class"
	interface NSNotification : NSCoding, NSCopying {
		[Export ("name")]
		// Null not allowed
		string Name { get; }

		[Export ("object")]
		[NullAllowed]
		NSObject Object { get; }

		[Export ("userInfo")]
		[NullAllowed]
		NSDictionary UserInfo { get; }

		[Export ("notificationWithName:object:")]
		[Static]
		NSNotification FromName (string name, [NullAllowed] NSObject obj);

		[Export ("notificationWithName:object:userInfo:")]
		[Static]
		NSNotification FromName (string name, [NullAllowed] NSObject obj, [NullAllowed] NSDictionary userInfo);

	}

	[BaseType (typeof (NSObject))]
	interface NSNotificationCenter {
		[Static]
		[Export ("defaultCenter", ArgumentSemantic.Strong)]
		NSNotificationCenter DefaultCenter { get; }

		[Export ("addObserver:selector:name:object:")]
		[PostSnippet ("AddObserverToList (observer, aName, anObject);", Optimizable = true)]
		void AddObserver (NSObject observer, Selector aSelector, [NullAllowed] NSString aName, [NullAllowed] NSObject anObject);

		[Export ("postNotification:")]
		void PostNotification (NSNotification notification);

		[Export ("postNotificationName:object:")]
		void PostNotificationName (string aName, [NullAllowed] NSObject anObject);

		[Export ("postNotificationName:object:userInfo:")]
		void PostNotificationName (string aName, [NullAllowed] NSObject anObject, [NullAllowed] NSDictionary aUserInfo);

		[Export ("removeObserver:")]
		[PostSnippet ("RemoveObserversFromList (observer, null, null);", Optimizable = true)]
		void RemoveObserver (NSObject observer);

		[Export ("removeObserver:name:object:")]
		[PostSnippet ("RemoveObserversFromList (observer, aName, anObject);", Optimizable = true)]
		void RemoveObserver (NSObject observer, [NullAllowed] string aName, [NullAllowed] NSObject anObject);

		[Export ("addObserverForName:object:queue:usingBlock:")]
		NSObject AddObserver ([NullAllowed] string name, [NullAllowed] NSObject obj, [NullAllowed] NSOperationQueue queue, Action<NSNotification> handler);
	}

	[NoiOS]
	[NoTV]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSDistributedLock {
		[Static]
		[Export ("lockWithPath:")]
		[return: NullAllowed]
		NSDistributedLock FromPath (string path);

		[Export ("initWithPath:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string path);

		[Export ("tryLock")]
		bool TryLock ();

		[Export ("unlock")]
		void Unlock ();

		[Export ("breakLock")]
		void BreakLock ();

		[Export ("lockDate", ArgumentSemantic.Copy)]
		NSDate LockDate { get; }
	}

	/// <summary>Allows notifications to be sent to objects in other tasks.</summary>
	[NoiOS]
	[NoTV]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (NSNotificationCenter))]
	interface NSDistributedNotificationCenter {
		[Static]
		[Export ("defaultCenter")]
		NSDistributedNotificationCenter DefaultCenter { get; }

		[Export ("addObserver:selector:name:object:suspensionBehavior:")]
		void AddObserver (NSObject observer, Selector selector, [NullAllowed] string notificationName, [NullAllowed] string notificationSenderc, NSNotificationSuspensionBehavior suspensionBehavior);

		[Export ("postNotificationName:object:userInfo:deliverImmediately:")]
		void PostNotificationName (string name, [NullAllowed] string anObject, [NullAllowed] NSDictionary userInfo, bool deliverImmediately);

		[Export ("postNotificationName:object:userInfo:options:")]
		void PostNotificationName (string name, [NullAllowed] string anObjecb, [NullAllowed] NSDictionary userInfo, NSNotificationFlags options);

		[Export ("addObserver:selector:name:object:")]
		void AddObserver (NSObject observer, Selector aSelector, [NullAllowed] string aName, [NullAllowed] NSObject anObject);

		[Export ("postNotificationName:object:")]
		void PostNotificationName (string aName, [NullAllowed] string anObject);

		[Export ("postNotificationName:object:userInfo:")]
		void PostNotificationName (string aName, [NullAllowed] string anObject, [NullAllowed] NSDictionary aUserInfo);

		[Export ("removeObserver:name:object:")]
		void RemoveObserver (NSObject observer, [NullAllowed] string aName, [NullAllowed] NSObject anObject);

		//Detected properties
		[Export ("suspended")]
		bool Suspended { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSLocalNotificationCenterType")]
		NSString NSLocalNotificationCenterType { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSNotificationQueue {
		[Static]
		[IsThreadStatic]
		[Export ("defaultQueue", ArgumentSemantic.Strong)]
		NSNotificationQueue DefaultQueue { get; }

		[DesignatedInitializer]
		[Export ("initWithNotificationCenter:")]
		NativeHandle Constructor (NSNotificationCenter notificationCenter);

		[Export ("enqueueNotification:postingStyle:")]
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle);

		[Export ("enqueueNotification:postingStyle:coalesceMask:forModes:")]
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle, NSNotificationCoalescing coalesceMask, [NullAllowed] NSString [] modes);

		[Wrap ("EnqueueNotification (notification, postingStyle, coalesceMask, modes?.GetConstants ())")]
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle, NSNotificationCoalescing coalesceMask, [NullAllowed] NSRunLoopMode [] modes);

		[Export ("dequeueNotificationsMatching:coalesceMask:")]
#if XAMCORE_5_0
		void DequeueNotifications (NSNotification notification, NSNotificationCoalescing coalesceMask);
#else
		[Obsolete ("Use 'DequeueNotifications' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		void DequeueNotificationsMatchingcoalesceMask (NSNotification notification, NSNotificationCoalescing coalesceMask);

		[Wrap ("DequeueNotificationsMatchingcoalesceMask (notification, coalesceMask)")]
		void DequeueNotifications (NSNotification notification, NSNotificationCoalescing coalesceMask);
#endif
	}

	[BaseType (typeof (NSObject))]
	// init returns NIL
	[DisableDefaultCtor]
	partial interface NSValue : NSSecureCoding, NSCopying {
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
		[Export ("getValue:")]
		void StoreValueAtAddress (IntPtr value);

		[MacCatalyst (13, 1)]
		[Export ("getValue:size:")]
		void StoreValueAtAddress (IntPtr value, nuint size);

		[Export ("objCType")]
		[Internal]
		IntPtr ObjCTypePtr ();

		//[Export ("initWithBytes:objCType:")][Internal]
		//NSValue InitFromBytes (IntPtr byte_ptr, IntPtr char_ptr_type);

		[Static]
		[Internal]
		[Export ("valueWithBytes:objCType:")]
		NSValue Create (IntPtr bytes, IntPtr objCType);

		[Static]
		[Export ("valueWithNonretainedObject:")]
		NSValue ValueFromNonretainedObject (NSObject anObject);

		[Export ("nonretainedObjectValue")]
		NSObject NonretainedObjectValue { get; }

		[Static]
		[Export ("valueWithPointer:")]
		NSValue ValueFromPointer (IntPtr pointer);

		[Export ("pointerValue")]
		IntPtr PointerValue { get; }

		[Export ("isEqualToValue:")]
		bool IsEqualTo (NSValue value);

		[Export ("valueWithRange:")]
		[Static]
		NSValue FromRange (NSRange range);

		[Export ("rangeValue")]
		NSRange RangeValue { get; }

		/// <param name="time">To be added.</param>
		///         <summary>Creates an NSValue that wraps a CMTime object..</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithCMTime:")]
		NSValue FromCMTime (CMTime time);

		/// <summary>Returns the CMTime value wrapped by this NSValue object.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("CMTimeValue")]
		CMTime CMTimeValue { get; }

		/// <param name="timeMapping">To be added.</param>
		///         <summary>Creates an NSValue that wraps a CMTimeMapping object.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithCMTimeMapping:")]
		NSValue FromCMTimeMapping (CMTimeMapping timeMapping);

		/// <summary>Returns the CMTimeMapping value wrapped by this NSValue object.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("CMTimeMappingValue")]
		CMTimeMapping CMTimeMappingValue { get; }

		/// <param name="timeRange">To be added.</param>
		///         <summary>Creates an NSValue that wraps a CMTimeRange object.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithCMTimeRange:")]
		NSValue FromCMTimeRange (CMTimeRange timeRange);

		/// <summary>Returns the CMTimeRange value wrapped by this NSValue object.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("CMTimeRangeValue")]
		CMTimeRange CMTimeRangeValue { get; }

#if MONOMAC
		[Export ("valueWithRect:")]
#else
		[Export ("valueWithCGRect:")]
#endif
		[Static]
		NSValue FromCGRect (CGRect rect);

#if MONOMAC
		[Export ("valueWithSize:")]
#else
		[Export ("valueWithCGSize:")]
#endif
		[Static]
		NSValue FromCGSize (CGSize size);

#if MONOMAC
		[Export ("valueWithPoint:")]
#else
		[Export ("valueWithCGPoint:")]
#endif
		[Static]
		NSValue FromCGPoint (CGPoint point);

		[MacCatalyst (15, 0)]
#if MONOMAC
		[Export ("rectValue")]
#else
		[Export ("CGRectValue")]
#endif
		CGRect CGRectValue { get; }

		[MacCatalyst (15, 0)]
#if MONOMAC
		[Export ("sizeValue")]
#else
		[Export ("CGSizeValue")]
#endif
		CGSize CGSizeValue { get; }

		[MacCatalyst (15, 0)]
#if MONOMAC
		[Export ("pointValue")]
#else
		[Export ("CGPointValue")]
#endif
		CGPoint CGPointValue { get; }

		/// <summary>Returns the CGAffineTransform value wrapped by this NSValue object.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("CGAffineTransformValue")]
		CoreGraphics.CGAffineTransform CGAffineTransformValue { get; }

		/// <summary>Returns the UIEdgeInsets value wrapped by this NSValue object.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("UIEdgeInsetsValue")]
		UIEdgeInsets UIEdgeInsetsValue { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("directionalEdgeInsetsValue")]
		NSDirectionalEdgeInsets DirectionalEdgeInsetsValue { get; }

		/// <param name="tran">To be added.</param>
		///         <summary>Creates an NSValue that wraps a CGAffineTransform object..</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("valueWithCGAffineTransform:")]
		[Static]
		NSValue FromCGAffineTransform (CoreGraphics.CGAffineTransform tran);

		/// <param name="insets">To be added.</param>
		///         <summary>Creates an NSValue that wraps a UIEdgeInsets object.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///         </remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("valueWithUIEdgeInsets:")]
		[Static]
		NSValue FromUIEdgeInsets (UIEdgeInsets insets);

		/// <param name="insets">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("valueWithDirectionalEdgeInsets:")]
		NSValue FromDirectionalEdgeInsets (NSDirectionalEdgeInsets insets);

		/// <param name="insets">The UIOffset instance</param>
		///         <summary>Creates an NSValue that wraps an UIOffset structure.</summary>
		///         <returns>
		///         </returns>
		///         <remarks>
		///         </remarks>
		[Export ("valueWithUIOffset:")]
		[Static]
		[NoMac]
		[MacCatalyst (13, 1)]
		NSValue FromUIOffset (UIOffset insets);

		/// <summary>Returns the UIOffset value wrapped by in this NSValue.</summary>
		///         <value>
		///         </value>
		///         <remarks>
		///         </remarks>
		[Export ("UIOffsetValue")]
		[NoMac]
		[MacCatalyst (13, 1)]
		UIOffset UIOffsetValue { get; }
		// from UIGeometry.h - those are in iOS8 only (even if the header is silent about them)
		// and not in OSX 10.10

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("CGVectorValue")]
		[NoMac]
		[MacCatalyst (13, 1)]
		CGVector CGVectorValue { get; }

		/// <param name="vector">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static, Export ("valueWithCGVector:")]
		[NoMac]
		[MacCatalyst (13, 1)]
		NSValue FromCGVector (CGVector vector);

		// Maybe we should include this inside mapkit.cs instead (it's a partial interface, so that's trivial)?
		/// <param name="coordinate">To be added.</param>
		///         <summary>Creates an NSValue that stores a CLLocationCoordinate2D.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithMKCoordinate:")]
		NSValue FromMKCoordinate (CoreLocation.CLLocationCoordinate2D coordinate);

		/// <param name="coordinateSpan">To be added.</param>
		///         <summary>Creates an NSValue that stores an MKCoordinateSpan.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithMKCoordinateSpan:")]
		NSValue FromMKCoordinateSpan (MapKit.MKCoordinateSpan coordinateSpan);

		/// <summary>The CLLocationCoordinate2D stored in this NSValue.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("MKCoordinateValue")]
		CoreLocation.CLLocationCoordinate2D CoordinateValue { get; }

		/// <summary>The MKCoordinateSpan stored in this NSValue.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("MKCoordinateSpanValue")]
		MapKit.MKCoordinateSpan CoordinateSpanValue { get; }

		/// <param name="transform">To be added.</param>
		///         <summary>Creates an NSValue that wraps a CATransform3D object.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Export ("valueWithCATransform3D:")]
		[Static]
		NSValue FromCATransform3D (CoreAnimation.CATransform3D transform);

		/// <summary>Returns the CATransform3D value wrapped by this NSValue object.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("CATransform3DValue")]
		CoreAnimation.CATransform3D CATransform3DValue { get; }

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[TV (16, 0)]
		[Export ("CMVideoDimensionsValue")]
		CMVideoDimensions CMVideoDimensionsValue { get; }

		[iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[TV (16, 0)]
		[Export ("valueWithCMVideoDimensions:")]
		[Static]
		NSValue FromCMVideoDimensions (CMVideoDimensions value);

		#region SceneKit Additions

		/// <param name="vector">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithSCNVector3:")]
		NSValue FromVector (SCNVector3 vector);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("SCNVector3Value")]
		SCNVector3 Vector3Value { get; }

		/// <param name="vector">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithSCNVector4:")]
		NSValue FromVector (SCNVector4 vector);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("SCNVector4Value")]
		SCNVector4 Vector4Value { get; }

		/// <param name="matrix">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithSCNMatrix4:")]
		NSValue FromSCNMatrix4 (SCNMatrix4 matrix);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("SCNMatrix4Value")]
		SCNMatrix4 SCNMatrix4Value { get; }

		#endregion
	}

	[BaseType (typeof (NSObject))]
	[Abstract] // Apple docs: NSValueTransformer is an abstract class...
	interface NSValueTransformer {
		[Static]
		[Export ("setValueTransformer:forName:")]
		void SetValueTransformer ([NullAllowed] NSValueTransformer transformer, string name);

		[Static]
		[Export ("valueTransformerForName:")]
		[return: NullAllowed]
		NSValueTransformer GetValueTransformer (string name);

		[Static]
		[Export ("valueTransformerNames")]
		string [] ValueTransformerNames { get; }

		[Static]
		[Export ("transformedValueClass")]
		Class TransformedValueClass { get; }

		[Static]
		[Export ("allowsReverseTransformation")]
		bool AllowsReverseTransformation { get; }

		[Export ("transformedValue:")]
		[return: NullAllowed]
		NSObject TransformedValue ([NullAllowed] NSObject value);

		[Export ("reverseTransformedValue:")]
		[return: NullAllowed]
		NSObject ReverseTransformedValue ([NullAllowed] NSObject value);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSNegateBooleanTransformerName")]
		NSString BooleanTransformerName { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSIsNilTransformerName")]
		NSString IsNilTransformerName { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSIsNotNilTransformerName")]
		NSString IsNotNilTransformerName { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Field ("NSUnarchiveFromDataTransformerName")]
		NSString UnarchiveFromDataTransformerName { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Field ("NSKeyedUnarchiveFromDataTransformerName")]
		NSString KeyedUnarchiveFromDataTransformerName { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSSecureUnarchiveFromDataTransformerName")]
		NSString SecureUnarchiveFromDataTransformerName { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSValueTransformer))]
	interface NSSecureUnarchiveFromDataTransformer {
		[Static]
		[Export ("allowedTopLevelClasses", ArgumentSemantic.Copy)]
		Class [] AllowedTopLevelClasses { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Static]
		[Wrap ("Array.ConvertAll (AllowedTopLevelClasses, c => Class.Lookup (c))")]
		Type [] AllowedTopLevelTypes { get; }
	}

	[BaseType (typeof (NSValue))]
	// init returns NIL
	[DisableDefaultCtor]
	interface NSNumber : CKRecordValue, NSFetchRequestResult {
		[Export ("charValue")]
		sbyte SByteValue { get; }

		[Export ("unsignedCharValue")]
		byte ByteValue { get; }

		[Export ("shortValue")]
		short Int16Value { get; }

		[Export ("unsignedShortValue")]
		ushort UInt16Value { get; }

		[Export ("intValue")]
		int Int32Value { get; }

		[Export ("unsignedIntValue")]
		uint UInt32Value { get; }

		[Export ("longValue")]
		nint LongValue { get; }

		[Export ("unsignedLongValue")]
		nuint UnsignedLongValue { get; }

		[Export ("longLongValue")]
		long Int64Value { get; }

		[Export ("unsignedLongLongValue")]
		ulong UInt64Value { get; }

		[Export ("floatValue")]
		float FloatValue { get; } /* float, not CGFloat */

		[Export ("doubleValue")]
		double DoubleValue { get; }

		[Export ("decimalValue")]
		NSDecimal NSDecimalValue { get; }

		[Export ("boolValue")]
		bool BoolValue { get; }

		[Export ("integerValue")]
		nint NIntValue { get; }

		[Export ("unsignedIntegerValue")]
		nuint NUIntValue { get; }

		[Export ("stringValue")]
		string StringValue { get; }

		[Export ("compare:")]
		nint Compare (NSNumber otherNumber);

		[Export ("isEqualToNumber:")]
		bool IsEqualTo (IntPtr number);

		[Export ("descriptionWithLocale:")]
		string DescriptionWithLocale (NSLocale locale);

		[DesignatedInitializer]
		[Export ("initWithChar:")]
		NativeHandle Constructor (sbyte value);

		[DesignatedInitializer]
		[Export ("initWithUnsignedChar:")]
		NativeHandle Constructor (byte value);

		[DesignatedInitializer]
		[Export ("initWithShort:")]
		NativeHandle Constructor (short value);

		[DesignatedInitializer]
		[Export ("initWithUnsignedShort:")]
		NativeHandle Constructor (ushort value);

		[DesignatedInitializer]
		[Export ("initWithInt:")]
		NativeHandle Constructor (int /* int, not NSInteger */ value);

		[DesignatedInitializer]
		[Export ("initWithUnsignedInt:")]
		NativeHandle Constructor (uint /* unsigned int, not NSUInteger */value);

		//[Export ("initWithLong:")]
		//NativeHandle Constructor (long value);
		//
		//[Export ("initWithUnsignedLong:")]
		//NativeHandle Constructor (ulong value);

		[DesignatedInitializer]
		[Export ("initWithLongLong:")]
		NativeHandle Constructor (long value);

		[DesignatedInitializer]
		[Export ("initWithUnsignedLongLong:")]
		NativeHandle Constructor (ulong value);

		[DesignatedInitializer]
		[Export ("initWithFloat:")]
		NativeHandle Constructor (float /* float, not CGFloat */ value);

		[DesignatedInitializer]
		[Export ("initWithDouble:")]
		NativeHandle Constructor (double value);

		[DesignatedInitializer]
		[Export ("initWithBool:")]
		NativeHandle Constructor (bool value);

		/// <param name="value">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[DesignatedInitializer]
		[Export ("initWithInteger:")]
		NativeHandle Constructor (nint value);

		/// <param name="value">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[DesignatedInitializer]
		[Export ("initWithUnsignedInteger:")]
		NativeHandle Constructor (nuint value);

		[Export ("numberWithChar:")]
		[Static]
		NSNumber FromSByte (sbyte value);

		[Static]
		[Export ("numberWithUnsignedChar:")]
		NSNumber FromByte (byte value);

		[Static]
		[Export ("numberWithShort:")]
		NSNumber FromInt16 (short value);

		[Static]
		[Export ("numberWithUnsignedShort:")]
		NSNumber FromUInt16 (ushort value);

		[Static]
		[Export ("numberWithInt:")]
		NSNumber FromInt32 (int /* int, not NSInteger */ value);

		[Static]
		[Export ("numberWithUnsignedInt:")]
		NSNumber FromUInt32 (uint /* unsigned int, not NSUInteger */ value);

		/// <param name="value">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("numberWithLong:")]
		NSNumber FromLong (nint value);
		//
		/// <param name="value">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("numberWithUnsignedLong:")]
		NSNumber FromUnsignedLong (nuint value);

		[Static]
		[Export ("numberWithLongLong:")]
		NSNumber FromInt64 (long value);

		[Static]
		[Export ("numberWithUnsignedLongLong:")]
		NSNumber FromUInt64 (ulong value);

		[Static]
		[Export ("numberWithFloat:")]
		NSNumber FromFloat (float /* float, not CGFloat */ value);

		[Static]
		[Export ("numberWithDouble:")]
		NSNumber FromDouble (double value);

		[Static]
		[Export ("numberWithBool:")]
		NSNumber FromBoolean (bool value);

		/// <param name="value">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("numberWithInteger:")]
		NSNumber FromNInt (nint value);

		/// <param name="value">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Static]
		[Export ("numberWithUnsignedInteger:")]
		NSNumber FromNUInt (nuint value);
	}


	[BaseType (typeof (NSFormatter))]
	interface NSNumberFormatter {
		[Export ("stringFromNumber:")]
		string StringFromNumber (NSNumber number);

		[Export ("numberFromString:")]
		NSNumber NumberFromString (string text);

#if !XAMCORE_5_0
		[Obsolete ("Use 'GetLocalizedString' instead.")]
		[Static]
		[Export ("localizedStringFromNumber:numberStyle:")]
		string LocalizedStringFromNumbernumberStyle (NSNumber num, NSNumberFormatterStyle nstyle);
#endif

		[Static]
		[Export ("localizedStringFromNumber:numberStyle:")]
		string GetLocalizedString (NSNumber number, NSNumberFormatterStyle numberStyle);

		//Detected properties
		[Export ("numberStyle")]
		NSNumberFormatterStyle NumberStyle { get; set; }

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("generatesDecimalNumbers")]
		bool GeneratesDecimalNumbers { get; set; }

		[Export ("formatterBehavior")]
		NSNumberFormatterBehavior FormatterBehavior { get; set; }

		[Static]
		[Export ("defaultFormatterBehavior")]
		NSNumberFormatterBehavior DefaultFormatterBehavior { get; set; }

		[Export ("negativeFormat")]
		string NegativeFormat { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForNegativeValues", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForNegativeValues { get; set; }

		[Export ("positiveFormat")]
		string PositiveFormat { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForPositiveValues", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForPositiveValues { get; set; }

		[Export ("allowsFloats")]
		bool AllowsFloats { get; set; }

		[Export ("decimalSeparator")]
		string DecimalSeparator { get; set; }

		[Export ("alwaysShowsDecimalSeparator")]
		bool AlwaysShowsDecimalSeparator { get; set; }

		[Export ("currencyDecimalSeparator")]
		string CurrencyDecimalSeparator { get; set; }

		[Export ("usesGroupingSeparator")]
		bool UsesGroupingSeparator { get; set; }

		[Export ("groupingSeparator")]
		string GroupingSeparator { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("zeroSymbol")]
		string ZeroSymbol { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForZero", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForZero { get; set; }

		[Export ("nilSymbol")]
		string NilSymbol { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForNil", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForNil { get; set; }

		[Export ("notANumberSymbol")]
		string NotANumberSymbol { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForNotANumber", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForNotANumber { get; set; }

		[Export ("positiveInfinitySymbol")]
		string PositiveInfinitySymbol { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForPositiveInfinity", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForPositiveInfinity { get; set; }

		[Export ("negativeInfinitySymbol")]
		string NegativeInfinitySymbol { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("textAttributesForNegativeInfinity", ArgumentSemantic.Copy)]
		NSDictionary TextAttributesForNegativeInfinity { get; set; }

		[Export ("positivePrefix")]
		string PositivePrefix { get; set; }

		[Export ("positiveSuffix")]
		string PositiveSuffix { get; set; }

		[Export ("negativePrefix")]
		string NegativePrefix { get; set; }

		[Export ("negativeSuffix")]
		string NegativeSuffix { get; set; }

		[Export ("currencyCode")]
		string CurrencyCode { get; set; }

		[Export ("currencySymbol")]
		string CurrencySymbol { get; set; }

		[Export ("internationalCurrencySymbol")]
		string InternationalCurrencySymbol { get; set; }

		[Export ("percentSymbol")]
		string PercentSymbol { get; set; }

		[Export ("perMillSymbol")]
		string PerMillSymbol { get; set; }

		[Export ("minusSign")]
		string MinusSign { get; set; }

		[Export ("plusSign")]
		string PlusSign { get; set; }

		[Export ("exponentSymbol")]
		string ExponentSymbol { get; set; }

		[Export ("groupingSize")]
		nuint GroupingSize { get; set; }

		[Export ("secondaryGroupingSize")]
		nuint SecondaryGroupingSize { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("multiplier", ArgumentSemantic.Copy)]
		NSNumber Multiplier { get; set; }

		[Export ("formatWidth")]
		nuint FormatWidth { get; set; }

		[Export ("paddingCharacter")]
		string PaddingCharacter { get; set; }

		[Export ("paddingPosition")]
		NSNumberFormatterPadPosition PaddingPosition { get; set; }

		[Export ("roundingMode")]
		NSNumberFormatterRoundingMode RoundingMode { get; set; }

		[Export ("roundingIncrement", ArgumentSemantic.Copy)]
		NSNumber RoundingIncrement { get; set; }

		[Export ("minimumIntegerDigits")]
		nint MinimumIntegerDigits { get; set; }

		[Export ("maximumIntegerDigits")]
		nint MaximumIntegerDigits { get; set; }

		[Export ("minimumFractionDigits")]
		nint MinimumFractionDigits { get; set; }

		[Export ("maximumFractionDigits")]
		nint MaximumFractionDigits { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("minimum", ArgumentSemantic.Copy)]
		NSNumber Minimum { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("maximum", ArgumentSemantic.Copy)]
		NSNumber Maximum { get; set; }

		[Export ("currencyGroupingSeparator")]
		string CurrencyGroupingSeparator { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("lenient")]
		bool Lenient { [Bind ("isLenient")] get; set; }

		[Export ("usesSignificantDigits")]
		bool UsesSignificantDigits { get; set; }

		[Export ("minimumSignificantDigits")]
		nuint MinimumSignificantDigits { get; set; }

		[Export ("maximumSignificantDigits")]
		nuint MaximumSignificantDigits { get; set; }

		/// <summary>Whether partial string validation is currently enabled.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("partialStringValidationEnabled")]
		bool PartialStringValidationEnabled { [Bind ("isPartialStringValidationEnabled")] get; set; }

		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		[Export ("minimumGroupingDigits")]
		nint MinimumGroupingDigits { get; set; }
	}

	[BaseType (typeof (NSNumber))]
	interface NSDecimalNumber : NSSecureCoding {
		[Export ("initWithMantissa:exponent:isNegative:")]
		NativeHandle Constructor (long mantissa, short exponent, bool isNegative);

		[DesignatedInitializer]
		[Export ("initWithDecimal:")]
		NativeHandle Constructor (NSDecimal dec);

		[Export ("initWithString:")]
		NativeHandle Constructor (string numberValue);

		[Export ("initWithString:locale:")]
		NativeHandle Constructor (string numberValue, NSObject locale);

		[Export ("descriptionWithLocale:")]
		[Override]
		string DescriptionWithLocale (NSLocale locale);

		[Export ("decimalValue")]
		NSDecimal NSDecimalValue { get; }

		[Export ("zero", ArgumentSemantic.Copy)]
		[Static]
		NSDecimalNumber Zero { get; }

		[Export ("one", ArgumentSemantic.Copy)]
		[Static]
		NSDecimalNumber One { get; }

		[Export ("minimumDecimalNumber", ArgumentSemantic.Copy)]
		[Static]
		NSDecimalNumber MinValue { get; }

		[Export ("maximumDecimalNumber", ArgumentSemantic.Copy)]
		[Static]
		NSDecimalNumber MaxValue { get; }

		[Export ("notANumber", ArgumentSemantic.Copy)]
		[Static]
		NSDecimalNumber NaN { get; }

		//
		// All the behavior ones require:
		// id <NSDecimalNumberBehaviors>)behavior;

		[Export ("decimalNumberByAdding:")]
		NSDecimalNumber Add (NSDecimalNumber d);

		[Export ("decimalNumberByAdding:withBehavior:")]
		NSDecimalNumber Add (NSDecimalNumber d, NSObject Behavior);

		[Export ("decimalNumberBySubtracting:")]
		NSDecimalNumber Subtract (NSDecimalNumber d);

		[Export ("decimalNumberBySubtracting:withBehavior:")]
		NSDecimalNumber Subtract (NSDecimalNumber d, NSObject Behavior);

		[Export ("decimalNumberByMultiplyingBy:")]
		NSDecimalNumber Multiply (NSDecimalNumber d);

		[Export ("decimalNumberByMultiplyingBy:withBehavior:")]
		NSDecimalNumber Multiply (NSDecimalNumber d, NSObject Behavior);

		[Export ("decimalNumberByDividingBy:")]
		NSDecimalNumber Divide (NSDecimalNumber d);

		[Export ("decimalNumberByDividingBy:withBehavior:")]
		NSDecimalNumber Divide (NSDecimalNumber d, NSObject Behavior);

		/// <param name="power">To be added.</param>
		/// <summary>Raises this number to the specified power.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("decimalNumberByRaisingToPower:")]
		NSDecimalNumber RaiseTo (nuint power);

		/// <param name="power">To be added.</param>
		/// <param name="Behavior">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("decimalNumberByRaisingToPower:withBehavior:")]
		NSDecimalNumber RaiseTo (nuint power, [NullAllowed] NSObject Behavior);

		[Export ("decimalNumberByMultiplyingByPowerOf10:")]
		NSDecimalNumber MultiplyPowerOf10 (short power);

		[Export ("decimalNumberByMultiplyingByPowerOf10:withBehavior:")]
		NSDecimalNumber MultiplyPowerOf10 (short power, [NullAllowed] NSObject Behavior);

		[Export ("decimalNumberByRoundingAccordingToBehavior:")]
		NSDecimalNumber Rounding (NSObject behavior);

		[Export ("compare:")]
		[Override]
		nint Compare (NSNumber other);

		[Export ("defaultBehavior", ArgumentSemantic.Strong)]
		[Static]
		NSObject DefaultBehavior { get; set; }

		[Export ("doubleValue")]
		[Override]
		double DoubleValue { get; }
	}

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSThread {
		[Static, Export ("currentThread", ArgumentSemantic.Strong)]
		NSThread Current { get; }

		[Static, Export ("callStackSymbols", ArgumentSemantic.Copy)]
		string [] NativeCallStack { get; }

		//+ (void)detachNewThreadSelector:(SEL)selector toTarget:(id)target withObject:(id)argument;

		[Static, Export ("isMultiThreaded")]
		bool IsMultiThreaded { get; }

		//- (NSMutableDictionary *)threadDictionary;

		[Static, Export ("sleepUntilDate:")]
		void SleepUntil (NSDate date);

		[Static, Export ("sleepForTimeInterval:")]
		void SleepFor (double timeInterval);

		[Static, Export ("exit")]
		void Exit ();

		[Static, Export ("threadPriority"), Internal]
		double _GetPriority ();

		[Static, Export ("setThreadPriority:"), Internal]
		bool _SetPriority (double priority);

		//+ (NSArray *)callStackReturnAddresses;

		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[Export ("stackSize")]
		nuint StackSize { get; set; }

		[Export ("isMainThread")]
		bool IsMainThread { get; }

		// MainThread is already used for the instance selector and we can't reuse the same name
		[Static]
		[Export ("isMainThread")]
		bool IsMain { get; }

		[Static]
		[Export ("mainThread", ArgumentSemantic.Strong)]
		NSThread MainThread { get; }

		/// <summary>Whether this NSThread is executing.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isExecuting")]
		bool IsExecuting { get; }

		/// <summary>Whether this NSThread has finished processing.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isFinished")]
		bool IsFinished { get; }

		/// <summary>Whether this NSThread is cancelled.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isCancelled")]
		bool IsCancelled { get; }

		[Export ("cancel")]
		void Cancel ();

		[Export ("start")]
		void Start ();

		[Export ("main")]
		void Main ();

		[MacCatalyst (13, 1)]
		[Export ("qualityOfService")]
		NSQualityOfService QualityOfService { get; set; }

		[Deprecated (PlatformName.MacOSX, 26, 0, "This notification does not protect against race conditions.")]
		[Deprecated (PlatformName.iOS, 26, 0, "This notification does not protect against race conditions.")]
		[Deprecated (PlatformName.TvOS, 26, 0, "This notification does not protect against race conditions.")]
		[Deprecated (PlatformName.MacCatalyst, 26, 0, "This notification does not protect against race conditions.")]
		[Notification]
		[Field ("NSThreadWillExitNotification")]
		NSString ThreadWillExitNotification { get; }

		[Deprecated (PlatformName.MacOSX, 26, 0, "This notification does not protect against race conditions.")]
		[Deprecated (PlatformName.iOS, 26, 0, "This notification does not protect against race conditions.")]
		[Deprecated (PlatformName.TvOS, 26, 0, "This notification does not protect against race conditions.")]
		[Deprecated (PlatformName.MacCatalyst, 26, 0, "This notification does not protect against race conditions.")]
		[Notification]
		[Field ("NSWillBecomeMultiThreadedNotification")]
		NSString WillBecomeMultiThreadedNotification { get; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSPort : NSCoding, NSCopying {
		[Static, Export ("port")]
		NSPort Create ();

		[Export ("invalidate")]
		void Invalidate ();

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isValid")]
		bool IsValid { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the Foundation.INSPortDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSPortDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate"), NullAllowed]
		INSPortDelegate Delegate { get; set; }

		[Export ("scheduleInRunLoop:forMode:")]
		void ScheduleInRunLoop (NSRunLoop runLoop, NSString runLoopMode);

		/// <param name="runLoop">To be added.</param>
		///         <param name="runLoopMode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("ScheduleInRunLoop (runLoop, runLoopMode.GetConstant ()!)")]
		void ScheduleInRunLoop (NSRunLoop runLoop, NSRunLoopMode runLoopMode);

		[Export ("removeFromRunLoop:forMode:")]
		void RemoveFromRunLoop (NSRunLoop runLoop, NSString runLoopMode);

		/// <param name="runLoop">To be added.</param>
		///         <param name="runLoopMode">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Wrap ("RemoveFromRunLoop (runLoop, runLoopMode.GetConstant ()!)")]
		void RemoveFromRunLoop (NSRunLoop runLoop, NSRunLoopMode runLoopMode);

		// Disable warning for NSMutableArray
#pragma warning disable 618
		/// <param name="limitDate">To be added.</param>
		/// <param name="components">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="receivePort">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="headerSpaceReserved">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("sendBeforeDate:components:from:reserved:")]
		bool SendBeforeDate (NSDate limitDate, [NullAllowed] NSMutableArray components, [NullAllowed] NSPort receivePort, nuint headerSpaceReserved);

		/// <param name="limitDate">To be added.</param>
		/// <param name="msgID">To be added.</param>
		/// <param name="components">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="receivePort">
		///           <para>To be added.</para>
		///           <para tool="nullallowed">This parameter can be <see langword="null" />.</para>
		///         </param>
		/// <param name="headerSpaceReserved">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("sendBeforeDate:msgid:components:from:reserved:")]
		bool SendBeforeDate (NSDate limitDate, nuint msgID, [NullAllowed] NSMutableArray components, [NullAllowed] NSPort receivePort, nuint headerSpaceReserved);
#pragma warning restore 618

		[Notification]
		[Field ("NSPortDidBecomeInvalidNotification")]
		NSString PortDidBecomeInvalidNotification { get; }
	}

	interface INSPortDelegate { }

	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	interface NSPortDelegate {
		/// <param name="message">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoMacCatalyst]
		[Export ("handlePortMessage:")]
		void MessageReceived (NSPortMessage message);
	}

	[BaseType (typeof (NSObject))]
	[MacCatalyst (15, 0)]
	interface NSPortMessage {
		[NoiOS]
		[NoTV]
		[MacCatalyst (15, 0)]
		[DesignatedInitializer]
		[Export ("initWithSendPort:receivePort:components:")]
		NativeHandle Constructor ([NullAllowed] NSPort sendPort, [NullAllowed] NSPort recvPort, [NullAllowed] NSArray components);

		[NullAllowed]
		[NoiOS]
		[NoTV]
		[MacCatalyst (15, 0)]
		[Export ("components")]
		NSArray Components { get; }

		// Apple started refusing applications that use those selectors (desk #63237)
		// The situation is a bit confusing since NSPortMessage.h is not part of iOS SDK - 
		// but the type is used (from NSPort[Delegate]) but not _itself_ documented
		// The selectors Apple *currently* dislike are removed from the iOS build
		[NoiOS]
		[NoTV]
		[MacCatalyst (15, 0)]
		[Export ("sendBeforeDate:")]
		bool SendBefore (NSDate date);

		[NullAllowed]
		[NoiOS]
		[NoTV]
		[MacCatalyst (15, 0)]
		[Export ("receivePort")]
		NSPort ReceivePort { get; }

		[NullAllowed]
		[NoiOS]
		[NoTV]
		[MacCatalyst (15, 0)]
		[Export ("sendPort")]
		NSPort SendPort { get; }

		[NoiOS]
		[NoTV]
		[MacCatalyst (15, 0)]
		[Export ("msgid")]
		uint MsgId { get; set; } /* uint32_t */
	}

	[BaseType (typeof (NSPort))]
	interface NSMachPort {
		[DesignatedInitializer]
		[Export ("initWithMachPort:")]
		NativeHandle Constructor (uint /* uint32_t */ machPort);

		[DesignatedInitializer]
		[Export ("initWithMachPort:options:")]
		NativeHandle Constructor (uint /* uint32_t */ machPort, NSMachPortRights options);

		[Static, Export ("portWithMachPort:")]
		NSPort FromMachPort (uint /* uint32_t */ port);

		[Static, Export ("portWithMachPort:options:")]
		NSPort FromMachPort (uint /* uint32_t */ port, NSMachPortRights options);

		[Export ("machPort")]
		uint MachPort { get; } /* uint32_t */

		[Export ("removeFromRunLoop:forMode:")]
		[Override]
		void RemoveFromRunLoop (NSRunLoop runLoop, NSString mode);

		// note: wrap'ed version using NSRunLoopMode will call the override

		[Export ("scheduleInRunLoop:forMode:")]
		[Override]
		void ScheduleInRunLoop (NSRunLoop runLoop, NSString mode);

		// note: wrap'ed version using NSRunLoopMode will call the override

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		[Override]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the Foundation.INSMachPortDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSMachPortDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate"), NullAllowed]
		INSMachPortDelegate Delegate { get; set; }
	}

	interface INSMachPortDelegate { }

	[Model, BaseType (typeof (NSPortDelegate))]
	[Protocol]
	interface NSMachPortDelegate {
		/// <param name="msgHeader">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("handleMachMessage:")]
		void MachMessageReceived (IntPtr msgHeader);
	}

	[BaseType (typeof (NSObject))]
	interface NSProcessInfo {
		[Export ("processInfo", ArgumentSemantic.Strong)]
		[Static]
		NSProcessInfo ProcessInfo { get; }

		[Export ("arguments")]
		string [] Arguments { get; }

		[Export ("environment")]
		NSDictionary Environment { get; }

		[Export ("processIdentifier")]
		int ProcessIdentifier { get; } /* int, not NSInteger */

		[Export ("globallyUniqueString")]
		string GloballyUniqueString { get; }

		[Export ("processName")]
		string ProcessName { get; set; }

		[Export ("hostName")]
		string HostName { get; }

		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'OperatingSystemVersion' or 'IsOperatingSystemAtLeastVersion' instead.")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'OperatingSystemVersion' or 'IsOperatingSystemAtLeastVersion' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'OperatingSystemVersion' or 'IsOperatingSystemAtLeastVersion' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'OperatingSystemVersion' or 'IsOperatingSystemAtLeastVersion' instead.")]
		[Export ("operatingSystem")]
		nint OperatingSystem { get; }

		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'OperatingSystemVersionString' instead.")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'OperatingSystemVersionString' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'OperatingSystemVersionString' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'OperatingSystemVersionString' instead.")]
		[Export ("operatingSystemName")]
		string OperatingSystemName { get; }

		[Export ("operatingSystemVersionString")]
		string OperatingSystemVersionString { get; }

		[Export ("physicalMemory")]
		ulong PhysicalMemory { get; }

		[Export ("processorCount")]
		nint ProcessorCount { get; }

		[Export ("activeProcessorCount")]
		nint ActiveProcessorCount { get; }

		[Export ("systemUptime")]
		double SystemUptime { get; }

		[MacCatalyst (13, 1)]
		[Export ("beginActivityWithOptions:reason:")]
		NSObject BeginActivity (NSActivityOptions options, string reason);

		[MacCatalyst (13, 1)]
		[Export ("endActivity:")]
		void EndActivity (NSObject activity);

		[MacCatalyst (13, 1)]
		[Export ("performActivityWithOptions:reason:usingBlock:")]
		void PerformActivity (NSActivityOptions options, string reason, Action runCode);

		[MacCatalyst (13, 1)]
		[Export ("isOperatingSystemAtLeastVersion:")]
		bool IsOperatingSystemAtLeastVersion (NSOperatingSystemVersion version);

		[MacCatalyst (13, 1)]
		[Export ("operatingSystemVersion")]
		NSOperatingSystemVersion OperatingSystemVersion { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("enableSuddenTermination")]
		void EnableSuddenTermination ();

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("disableSuddenTermination")]
		void DisableSuddenTermination ();

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("enableAutomaticTermination:")]
		void EnableAutomaticTermination (string reason);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("disableAutomaticTermination:")]
		void DisableAutomaticTermination (string reason);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("automaticTerminationSupportEnabled")]
		bool AutomaticTerminationSupportEnabled { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("performExpiringActivityWithReason:usingBlock:")]
		void PerformExpiringActivity (string reason, Action<bool> block);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[TV (15, 0)]
		[MacCatalyst (13, 1)]
		[Export ("lowPowerModeEnabled")]
		bool LowPowerModeEnabled { [Bind ("isLowPowerModeEnabled")] get; }

		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSProcessInfoPowerStateDidChangeNotification")]
		NSString PowerStateDidChangeNotification { get; }

		[MacCatalyst (13, 1)]
		[Export ("thermalState")]
		NSProcessInfoThermalState ThermalState { get; }

		[Field ("NSProcessInfoThermalStateDidChangeNotification")]
		[MacCatalyst (13, 1)]
		[Notification]
		NSString ThermalStateDidChangeNotification { get; }

		#region NSProcessInfoPlatform (NSProcessInfo)
		[MacCatalyst (13, 1)]
		[Export ("macCatalystApp")]
		bool IsMacCatalystApplication { [Bind ("isMacCatalystApp")] get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("iOSAppOnMac")]
		bool IsiOSApplicationOnMac { [Bind ("isiOSAppOnMac")] get; }

		[TV (26, 1), Mac (26, 1), iOS (26, 1), MacCatalyst (26, 1)]
		[Export ("iOSAppOnVision")]
		bool IsiOSAppOnVision { [Bind ("isiOSAppOnVision")] get; }
		#endregion

		[Field ("NSProcessInfoPerformanceProfileDidChangeNotification", "Metal")]
		[Notification]
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		NSString PerformanceProfileDidChangeNotification { get; }
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[Category]
	[BaseType (typeof (NSProcessInfo))]
	interface NSProcessInfo_NSUserInformation {
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("userName")]
		string GetUserName ();

		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("fullUserName")]
		string GetFullUserName ();
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	partial interface NSProgress {

		[Static, Export ("currentProgress")]
		NSProgress CurrentProgress { get; }

		[Static, Export ("progressWithTotalUnitCount:")]
		NSProgress FromTotalUnitCount (long unitCount);

		[MacCatalyst (13, 1)]
		[Static, Export ("discreteProgressWithTotalUnitCount:")]
		NSProgress GetDiscreteProgress (long unitCount);

		[MacCatalyst (13, 1)]
		[Static, Export ("progressWithTotalUnitCount:parent:pendingUnitCount:")]
		NSProgress FromTotalUnitCount (long unitCount, NSProgress parent, long portionOfParentTotalUnitCount);

		[DesignatedInitializer]
		[Export ("initWithParent:userInfo:")]
		NativeHandle Constructor ([NullAllowed] NSProgress parentProgress, [NullAllowed] NSDictionary userInfo);

		[Export ("becomeCurrentWithPendingUnitCount:")]
		void BecomeCurrent (long pendingUnitCount);

		[Export ("resignCurrent")]
		void ResignCurrent ();

		[MacCatalyst (13, 1)]
		[Export ("addChild:withPendingUnitCount:")]
		void AddChild (NSProgress child, long pendingUnitCount);

		[Export ("totalUnitCount")]
		long TotalUnitCount { get; set; }

		[Export ("completedUnitCount")]
		long CompletedUnitCount { get; set; }

		[Export ("localizedDescription", ArgumentSemantic.Copy), NullAllowed]
		string LocalizedDescription { get; set; }

		[Export ("localizedAdditionalDescription", ArgumentSemantic.Copy), NullAllowed]
		string LocalizedAdditionalDescription { get; set; }

		/// <summary>Whether this is tracking work that can be cancelled.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("cancellable")]
		bool Cancellable { [Bind ("isCancellable")] get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("pausable")]
		bool Pausable { [Bind ("isPausable")] get; set; }

		/// <summary>Whether this is tracking work that was cancelled.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("cancelled")]
		bool Cancelled { [Bind ("isCancelled")] get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; }

		/// <param name="handler">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("setCancellationHandler:")]
		void SetCancellationHandler (Action handler);

		/// <param name="handler">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("setPausingHandler:")]
		void SetPauseHandler (Action handler);

		/// <param name="handler">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("setResumingHandler:")]
		void SetResumingHandler (Action handler);

		[Export ("setUserInfoObject:forKey:")]
		void SetUserInfo ([NullAllowed] NSObject obj, NSString key);

		/// <summary>Whether the tracked progress is indeterminate. Read-only.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("indeterminate")]
		bool Indeterminate { [Bind ("isIndeterminate")] get; }

		[Export ("fractionCompleted")]
		double FractionCompleted { get; }

		[Export ("cancel")]
		void Cancel ();

		[Export ("pause")]
		void Pause ();

		[MacCatalyst (13, 1)]
		[Export ("resume")]
		void Resume ();

		[Export ("userInfo")]
		NSDictionary UserInfo { get; }

		[NullAllowed] // by default this property is null
		[Export ("kind", ArgumentSemantic.Copy)]
		NSString Kind { get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("publish")]
		void Publish ();

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("unpublish")]
		void Unpublish ();

		/// <param name="acknowledgementHandler">To be added.</param>
		///         <param name="appBundleIdentifier">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("setAcknowledgementHandler:forAppBundleIdentifier:")]
		void SetAcknowledgementHandler (Action<bool> acknowledgementHandler, string appBundleIdentifier);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Static, Export ("addSubscriberForFileURL:withPublishingHandler:")]
		NSObject AddSubscriberForFile (NSUrl url, Action<NSProgress> publishingHandler);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Static, Export ("removeSubscriber:")]
		void RemoveSubscriber (NSObject subscriber);

		/// <param name="success">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("acknowledgeWithSuccess:")]
		void AcknowledgeWithSuccess (bool success);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("old")]
		bool Old { [Bind ("isOld")] get; }

		/// <summary>Represents the value associated with the constant NSProgressKindFile</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSProgressKindFile")]
		NSString KindFile { get; }

		/// <summary>Represents the value associated with the constant NSProgressEstimatedTimeRemainingKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSProgressEstimatedTimeRemainingKey")]
		NSString EstimatedTimeRemainingKey { get; }

		/// <summary>Represents the value associated with the constant NSProgressThroughputKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSProgressThroughputKey")]
		NSString ThroughputKey { get; }

		/// <summary>Represents the value associated with the constant NSProgressFileOperationKindKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSProgressFileOperationKindKey")]
		NSString FileOperationKindKey { get; }

		/// <summary>Represents the value associated with the constant NSProgressFileOperationKindDownloading</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSProgressFileOperationKindDownloading")]
		NSString FileOperationKindDownloading { get; }

		/// <summary>Represents the value associated with the constant NSProgressFileOperationKindDecompressingAfterDownloading</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSProgressFileOperationKindDecompressingAfterDownloading")]
		NSString FileOperationKindDecompressingAfterDownloading { get; }

		/// <summary>Represents the value associated with the constant NSProgressFileOperationKindReceiving</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSProgressFileOperationKindReceiving")]
		NSString FileOperationKindReceiving { get; }

		/// <summary>Represents the value associated with the constant NSProgressFileOperationKindCopying</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSProgressFileOperationKindCopying")]
		NSString FileOperationKindCopying { get; }

		[TV (14, 5), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("NSProgressFileOperationKindUploading")]
		NSString FileOperationKindUploading { get; }

		/// <summary>Represents the value associated with the constant NSProgressFileURLKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSProgressFileURLKey")]
		NSString FileURLKey { get; }

		/// <summary>Represents the value associated with the constant NSProgressFileTotalCountKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSProgressFileTotalCountKey")]
		NSString FileTotalCountKey { get; }

		/// <summary>Represents the value associated with the constant NSProgressFileCompletedCountKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSProgressFileCompletedCountKey")]
		NSString FileCompletedCountKey { get; }

		[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("NSProgressFileOperationKindDuplicating")]
		NSString FileOperationKindDuplicatingKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Field ("NSProgressFileAnimationImageKey")]
		NSString FileAnimationImageKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Field ("NSProgressFileAnimationImageOriginalRectKey")]
		NSString FileAnimationImageOriginalRectKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Field ("NSProgressFileIconKey")]
		NSString FileIconKey { get; }

		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="unitCount">To be added.</param>
			<summary>To be added.</summary>
			<returns>A task that represents the asynchronous PerformAsCurrent operation</returns>
			<remarks>
			          <para copied="true">The PerformAsCurrentAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			"""), Export ("performAsCurrentWithPendingUnitCount:usingBlock:")]
		void PerformAsCurrent (long unitCount, Action work);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("finished")]
		bool Finished { [Bind ("isFinished")] get; }

		[Internal]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("estimatedTimeRemaining", ArgumentSemantic.Copy)]
		//[BindAs (typeof (nint?))]
		NSNumber _EstimatedTimeRemaining { get; set; }

		[Internal]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("throughput", ArgumentSemantic.Copy)]
		//[BindAs (typeof (nint?))]
		NSNumber _Throughput { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("fileOperationKind")]
		string FileOperationKind { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("fileURL", ArgumentSemantic.Copy)]
		NSUrl FileUrl { get; set; }

		[Internal]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("fileTotalCount", ArgumentSemantic.Copy)]
		//[BindAs (typeof (nint?))]
		NSNumber _FileTotalCount { get; set; }

		[Internal]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("fileCompletedCount", ArgumentSemantic.Copy)]
		//[BindAs (typeof (nint?))]
		NSNumber _FileCompletedCount { get; set; }
	}

	interface INSProgressReporting { }

	[MacCatalyst (13, 1)]
	[Protocol]
	interface NSProgressReporting {
		[Abstract]
		[Export ("progress")]
		NSProgress Progress { get; }
	}

	[BaseType (typeof (NSMutableData))]
	interface NSPurgeableData : NSSecureCoding, NSMutableCopying, NSDiscardableContent {
	}

	/// <summary>Interface for a class that can save memory by discarding some of its subcomponents when they are not in use.</summary>
	/// <remarks>To be added.</remarks>
	[Protocol]
	interface NSDiscardableContent {
		/// <summary>Requests access to the content, and returns <see langword="true" /> if the contents are available and were successfully accessed. (Otherwise, returns <see langword="false" />.)</summary>
		/// <returns>
		///           <see langword="true" /> if the contents can be retrieved.</returns>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("beginContentAccess")]
		bool BeginContentAccess ();

		/// <summary>Indicates that access to the content is no longer needed.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("endContentAccess")]
		void EndContentAccess ();

		/// <summary>Discards the content if it is not being accessed.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("discardContentIfPossible")]
		void DiscardContentIfPossible ();

		/// <summary>Gets a Boolean value that tells whether the content has been discarded.</summary>
		/// <value>
		///           <see langword="true" /> if the content has been discarded.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("isContentDiscarded")]
		bool IsContentDiscarded { get; }
	}

	/// <param name="newReadingUrl">To be added.</param>
	/// <param name="newWritingUrl">To be added.</param>
	/// <summary>A delegate that used with a number of coordinated read-and-write functions in <see cref="Foundation.NSFileCoordinator" />.</summary>
	/// <remarks>To be added.</remarks>
	delegate void NSFileCoordinatorWorkerRW (NSUrl newReadingUrl, NSUrl newWritingUrl);

	interface INSFilePresenter { }

	[BaseType (typeof (NSObject))]
	interface NSFileCoordinator {
		[Static, Export ("addFilePresenter:")]
		[PostGet ("FilePresenters")]
		void AddFilePresenter (INSFilePresenter filePresenter);

		[Static]
		[Export ("removeFilePresenter:")]
		[PostGet ("FilePresenters")]
		void RemoveFilePresenter (INSFilePresenter filePresenter);

		[Static]
		[Export ("filePresenters", ArgumentSemantic.Copy)]
		INSFilePresenter [] FilePresenters { get; }

		[DesignatedInitializer]
		[Export ("initWithFilePresenter:")]
		NativeHandle Constructor ([NullAllowed] INSFilePresenter filePresenterOrNil);

		[Export ("coordinateReadingItemAtURL:options:error:byAccessor:")]
		void CoordinateRead (NSUrl itemUrl, NSFileCoordinatorReadingOptions options, out NSError error, /* non null */ Action<NSUrl> worker);

		[Export ("coordinateWritingItemAtURL:options:error:byAccessor:")]
		void CoordinateWrite (NSUrl url, NSFileCoordinatorWritingOptions options, out NSError error, /* non null */ Action<NSUrl> worker);

		[Export ("coordinateReadingItemAtURL:options:writingItemAtURL:options:error:byAccessor:")]
		void CoordinateReadWrite (NSUrl readingURL, NSFileCoordinatorReadingOptions readingOptions, NSUrl writingURL, NSFileCoordinatorWritingOptions writingOptions, out NSError error, /* non null */ NSFileCoordinatorWorkerRW readWriteWorker);

		[Export ("coordinateWritingItemAtURL:options:writingItemAtURL:options:error:byAccessor:")]
		void CoordinateWriteWrite (NSUrl writingURL, NSFileCoordinatorWritingOptions writingOptions, NSUrl writingURL2, NSFileCoordinatorWritingOptions writingOptions2, out NSError error, /* non null */ NSFileCoordinatorWorkerRW writeWriteWorker);

		[Export ("prepareForReadingItemsAtURLs:options:writingItemsAtURLs:options:error:byAccessor:")]
		void CoordinateBatch (NSUrl [] readingURLs, NSFileCoordinatorReadingOptions readingOptions, NSUrl [] writingURLs, NSFileCoordinatorWritingOptions writingOptions, out NSError error, /* non null */ Action batchHandler);

		[MacCatalyst (13, 1)]
		[Export ("coordinateAccessWithIntents:queue:byAccessor:")]
		void CoordinateAccess (NSFileAccessIntent [] intents, NSOperationQueue executionQueue, Action<NSError> accessor);

		[Export ("itemAtURL:didMoveToURL:")]
		void ItemMoved (NSUrl fromUrl, NSUrl toUrl);

		[Export ("cancel")]
		void Cancel ();

		[Export ("itemAtURL:willMoveToURL:")]
		void WillMove (NSUrl oldUrl, NSUrl newUrl);

		[Export ("purposeIdentifier")]
		string PurposeIdentifier { get; set; }

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("itemAtURL:didChangeUbiquityAttributes:")]
		void ItemUbiquityAttributesChanged (NSUrl url, NSSet<NSString> attributes);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSFileAccessIntent {
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Static, Export ("readingIntentWithURL:options:")]
		NSFileAccessIntent CreateReadingIntent (NSUrl url, NSFileCoordinatorReadingOptions options);

		[Static, Export ("writingIntentWithURL:options:")]
		NSFileAccessIntent CreateWritingIntent (NSUrl url, NSFileCoordinatorWritingOptions options);
	}

	[BaseType (typeof (NSObject))]
	partial interface NSFileManager {
		/// <summary>Represents the value associated with the constant NSFileType</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileType")]
		NSString NSFileType { get; }

		/// <summary>Represents the value associated with the constant NSFileTypeDirectory</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileTypeDirectory")]
		NSString TypeDirectory { get; }

		/// <summary>Represents the value associated with the constant NSFileTypeRegular</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileTypeRegular")]
		NSString TypeRegular { get; }

		/// <summary>Represents the value associated with the constant NSFileTypeSymbolicLink</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileTypeSymbolicLink")]
		NSString TypeSymbolicLink { get; }

		/// <summary>Represents the value associated with the constant NSFileTypeSocket</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileTypeSocket")]
		NSString TypeSocket { get; }

		/// <summary>Represents the value associated with the constant NSFileTypeCharacterSpecial</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileTypeCharacterSpecial")]
		NSString TypeCharacterSpecial { get; }

		/// <summary>Represents the value associated with the constant NSFileTypeBlockSpecial</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileTypeBlockSpecial")]
		NSString TypeBlockSpecial { get; }

		/// <summary>Represents the value associated with the constant NSFileTypeUnknown</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileTypeUnknown")]
		NSString TypeUnknown { get; }

		/// <summary>Represents the value associated with the constant NSFileSize</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileSize")]
		NSString Size { get; }

		/// <summary>Represents the value associated with the constant NSFileModificationDate</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileModificationDate")]
		NSString ModificationDate { get; }

		/// <summary>Represents the value associated with the constant NSFileReferenceCount</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileReferenceCount")]
		NSString ReferenceCount { get; }

		/// <summary>Represents the value associated with the constant NSFileDeviceIdentifier</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileDeviceIdentifier")]
		NSString DeviceIdentifier { get; }

		/// <summary>Represents the value associated with the constant NSFileOwnerAccountName</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileOwnerAccountName")]
		NSString OwnerAccountName { get; }

		/// <summary>Represents the value associated with the constant NSFileGroupOwnerAccountName</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileGroupOwnerAccountName")]
		NSString GroupOwnerAccountName { get; }

		/// <summary>Represents the value associated with the constant NSFilePosixPermissions</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFilePosixPermissions")]
		NSString PosixPermissions { get; }

		/// <summary>Represents the value associated with the constant NSFileSystemNumber</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileSystemNumber")]
		NSString SystemNumber { get; }

		/// <summary>Represents the value associated with the constant NSFileSystemFileNumber</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileSystemFileNumber")]
		NSString SystemFileNumber { get; }

		/// <summary>Represents the value associated with the constant NSFileExtensionHidden</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileExtensionHidden")]
		NSString ExtensionHidden { get; }

		/// <summary>Represents the value associated with the constant NSFileHFSCreatorCode</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileHFSCreatorCode")]
		NSString HfsCreatorCode { get; }

		/// <summary>Represents the value associated with the constant NSFileHFSTypeCode</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileHFSTypeCode")]
		NSString HfsTypeCode { get; }

		/// <summary>Represents the value associated with the constant NSFileImmutable</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileImmutable")]
		NSString Immutable { get; }

		/// <summary>Represents the value associated with the constant NSFileAppendOnly</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileAppendOnly")]
		NSString AppendOnly { get; }

		/// <summary>Represents the value associated with the constant NSFileCreationDate</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileCreationDate")]
		NSString CreationDate { get; }

		/// <summary>Represents the value associated with the constant NSFileOwnerAccountID</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileOwnerAccountID")]
		NSString OwnerAccountID { get; }

		/// <summary>Represents the value associated with the constant NSFileGroupOwnerAccountID</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileGroupOwnerAccountID")]
		NSString GroupOwnerAccountID { get; }

		/// <summary>Represents the value associated with the constant NSFileBusy</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileBusy")]
		NSString Busy { get; }

		/// <summary>Represents the value associated with the constant NSFileProtectionKey</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Field ("NSFileProtectionKey")]
		NSString FileProtectionKey { get; }

		/// <summary>Represents the value associated with the constant NSFileProtectionNone</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'NSFileProtectionType' instead.")]
		[MacCatalyst (13, 1)]
		[Field ("NSFileProtectionNone")]
		NSString FileProtectionNone { get; }

		/// <summary>Represents the value associated with the constant NSFileProtectionComplete</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'NSFileProtectionType' instead.")]
		[MacCatalyst (13, 1)]
		[Field ("NSFileProtectionComplete")]
		NSString FileProtectionComplete { get; }

		/// <summary>Represents the value associated with the constant NSFileProtectionCompleteUnlessOpen</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'NSFileProtectionType' instead.")]
		[MacCatalyst (13, 1)]
		[Field ("NSFileProtectionCompleteUnlessOpen")]
		NSString FileProtectionCompleteUnlessOpen { get; }

		/// <summary>Represents the value associated with the constant NSFileProtectionCompleteUntilFirstUserAuthentication</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Obsolete ("Use the 'NSFileProtectionType' instead.")]
		[MacCatalyst (13, 1)]
		[Field ("NSFileProtectionCompleteUntilFirstUserAuthentication")]
		NSString FileProtectionCompleteUntilFirstUserAuthentication { get; }

		/// <summary>Represents the value associated with the constant NSFileSystemSize</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileSystemSize")]
		NSString SystemSize { get; }

		/// <summary>Represents the value associated with the constant NSFileSystemFreeSize</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileSystemFreeSize")]
		NSString SystemFreeSize { get; }

		/// <summary>Represents the value associated with the constant NSFileSystemNodes</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileSystemNodes")]
		NSString SystemNodes { get; }

		/// <summary>Represents the value associated with the constant NSFileSystemFreeNodes</summary>
		///         <value>
		///         </value>
		///         <remarks>To be added.</remarks>
		[Field ("NSFileSystemFreeNodes")]
		NSString SystemFreeNodes { get; }

		[Static, Export ("defaultManager", ArgumentSemantic.Strong)]
		NSFileManager DefaultManager { get; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>An instance of the Foundation.INSFileManagerDelegate model class which acts as the class delegate.</summary>
		///         <value>The instance of the Foundation.INSFileManagerDelegate model class</value>
		///         <remarks>
		///           <para>The delegate instance assigned to this object will be used to handle events or provide data on demand to this class.</para>
		///           <para>When setting the Delegate or WeakDelegate values events will be delivered to the specified instance instead of being delivered to the C#-style events</para>
		///           <para>This is the strongly typed version of the object, developers should use the WeakDelegate property instead if they want to merely assign a class derived from NSObject that has been decorated with [Export] attributes.</para>
		///         </remarks>
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		INSFileManagerDelegate Delegate { get; set; }

		[Export ("setAttributes:ofItemAtPath:error:")]
		bool SetAttributes (NSDictionary attributes, string path, out NSError error);

		[Export ("createDirectoryAtPath:withIntermediateDirectories:attributes:error:")]
		bool CreateDirectory (string path, bool createIntermediates, [NullAllowed] NSDictionary attributes, out NSError error);

		[Export ("contentsOfDirectoryAtPath:error:")]
		string [] GetDirectoryContent (string path, out NSError error);

		[Export ("subpathsOfDirectoryAtPath:error:")]
		string [] GetDirectoryContentRecursive (string path, out NSError error);

		[Export ("attributesOfItemAtPath:error:")]
		[Internal]
		NSDictionary _GetAttributes (string path, out NSError error);

		[Export ("attributesOfFileSystemForPath:error:")]
		[Internal]
		NSDictionary _GetFileSystemAttributes (String path, out NSError error);

		[Export ("createSymbolicLinkAtPath:withDestinationPath:error:")]
		bool CreateSymbolicLink (string path, string destPath, out NSError error);

		[Export ("destinationOfSymbolicLinkAtPath:error:")]
		string GetSymbolicLinkDestination (string path, out NSError error);

		[Export ("copyItemAtPath:toPath:error:")]
		bool Copy (string srcPath, string dstPath, out NSError error);

		[Export ("moveItemAtPath:toPath:error:")]
		bool Move (string srcPath, string dstPath, out NSError error);

		[Export ("linkItemAtPath:toPath:error:")]
		bool Link (string srcPath, string dstPath, out NSError error);

		[Export ("removeItemAtPath:error:")]
		bool Remove ([NullAllowed] string path, out NSError error);

#if DEPRECATED
		// These are not available on iOS, and deprecated on OSX.
		[Export ("linkPath:toPath:handler:")]
		bool LinkPath (string src, string dest, IntPtr handler);

		[Export ("copyPath:toPath:handler:")]
		bool CopyPath (string src, string dest, IntPtr handler);

		[Export ("movePath:toPath:handler:")]
		bool MovePath (string src, string dest, IntPtr handler);

		[Export ("removeFileAtPath:handler:")]
		bool RemoveFileAtPath (string path, IntPtr handler);
#endif
		[Export ("currentDirectoryPath")]
		string GetCurrentDirectory ();

		[Export ("changeCurrentDirectoryPath:")]
		bool ChangeCurrentDirectory (string path);

		[Export ("fileExistsAtPath:")]
		bool FileExists (string path);

		[Export ("fileExistsAtPath:isDirectory:")]
		bool FileExists (string path, ref bool isDirectory);

		[Export ("isReadableFileAtPath:")]
		bool IsReadableFile (string path);

		[Export ("isWritableFileAtPath:")]
		bool IsWritableFile (string path);

		[Export ("isExecutableFileAtPath:")]
		bool IsExecutableFile (string path);

		[Export ("isDeletableFileAtPath:")]
		bool IsDeletableFile (string path);

		[Export ("contentsEqualAtPath:andPath:")]
		bool ContentsEqual (string path1, string path2);

		[Export ("displayNameAtPath:")]
		string DisplayName (string path);

		[Export ("componentsToDisplayForPath:")]
		string [] ComponentsToDisplay (string path);

		[Export ("enumeratorAtPath:")]
		NSDirectoryEnumerator GetEnumerator (string path);

		[Export ("subpathsAtPath:")]
		string [] Subpaths (string path);

		[Export ("contentsAtPath:")]
		NSData Contents (string path);

		[Export ("createFileAtPath:contents:attributes:")]
		bool CreateFile (string path, NSData data, [NullAllowed] NSDictionary attr);

		[Export ("contentsOfDirectoryAtURL:includingPropertiesForKeys:options:error:")]
		NSUrl [] GetDirectoryContent (NSUrl url, [NullAllowed] NSArray properties, NSDirectoryEnumerationOptions options, out NSError error);

		[Export ("copyItemAtURL:toURL:error:")]
		bool Copy (NSUrl srcUrl, NSUrl dstUrl, out NSError error);

		[Export ("moveItemAtURL:toURL:error:")]
		bool Move (NSUrl srcUrl, NSUrl dstUrl, out NSError error);

		[Export ("linkItemAtURL:toURL:error:")]
		bool Link (NSUrl srcUrl, NSUrl dstUrl, out NSError error);

		[Export ("removeItemAtURL:error:")]
		bool Remove ([NullAllowed] NSUrl url, out NSError error);

		[Export ("enumeratorAtURL:includingPropertiesForKeys:options:errorHandler:")]
		NSDirectoryEnumerator GetEnumerator (NSUrl url, [NullAllowed] NSString [] keys, NSDirectoryEnumerationOptions options, [NullAllowed] NSEnumerateErrorHandler handler);

		[Export ("URLForDirectory:inDomain:appropriateForURL:create:error:")]
		NSUrl GetUrl (NSSearchPathDirectory directory, NSSearchPathDomain domain, [NullAllowed] NSUrl url, bool shouldCreate, out NSError error);

		[Export ("URLsForDirectory:inDomains:")]
		NSUrl [] GetUrls (NSSearchPathDirectory directory, NSSearchPathDomain domains);

		[Export ("replaceItemAtURL:withItemAtURL:backupItemName:options:resultingItemURL:error:")]
		bool Replace (NSUrl originalItem, NSUrl newItem, [NullAllowed] string backupItemName, NSFileManagerItemReplacementOptions options, out NSUrl resultingURL, out NSError error);

		[Export ("mountedVolumeURLsIncludingResourceValuesForKeys:options:")]
		NSUrl [] GetMountedVolumes ([NullAllowed] NSArray properties, NSVolumeEnumerationOptions options);

		// Methods to convert paths to/from C strings for passing to system calls - Not implemented
		////- (const char *)fileSystemRepresentationWithPath:(NSString *)path;
		//[Export ("fileSystemRepresentationWithPath:")]
		//const char FileSystemRepresentationWithPath (string path);

		////- (NSString *)stringWithFileSystemRepresentation:(const char *)str length:(NSUInteger)len;
		//[Export ("stringWithFileSystemRepresentation:length:")]
		//string StringWithFileSystemRepresentation (const char str, uint len);

		[Export ("createDirectoryAtURL:withIntermediateDirectories:attributes:error:")]
		bool CreateDirectory (NSUrl url, bool createIntermediates, [NullAllowed] NSDictionary attributes, out NSError error);

		[Export ("createSymbolicLinkAtURL:withDestinationURL:error:")]
		bool CreateSymbolicLink (NSUrl url, NSUrl destURL, out NSError error);

		[Export ("setUbiquitous:itemAtURL:destinationURL:error:")]
		bool SetUbiquitous (bool flag, NSUrl url, NSUrl destinationUrl, out NSError error);

		[Export ("isUbiquitousItemAtURL:")]
		bool IsUbiquitous (NSUrl url);

		[Export ("startDownloadingUbiquitousItemAtURL:error:")]
		bool StartDownloadingUbiquitous (NSUrl url, out NSError error);

		[Export ("evictUbiquitousItemAtURL:error:")]
		bool EvictUbiquitous (NSUrl url, out NSError error);

		[Export ("URLForUbiquityContainerIdentifier:")]
		NSUrl GetUrlForUbiquityContainer ([NullAllowed] string containerIdentifier);

		[Export ("URLForPublishingUbiquitousItemAtURL:expirationDate:error:")]
		NSUrl GetUrlForPublishingUbiquitousItem (NSUrl url, out NSDate expirationDate, out NSError error);

		[Export ("ubiquityIdentityToken")]
		NSObject UbiquityIdentityToken { get; }

		[Field ("NSUbiquityIdentityDidChangeNotification")]
		[Notification]
		NSString UbiquityIdentityDidChangeNotification { get; }

		[Export ("containerURLForSecurityApplicationGroupIdentifier:")]
		NSUrl GetContainerUrl (string securityApplicationGroupIdentifier);

		[MacCatalyst (13, 1)]
		[Export ("getRelationship:ofDirectory:inDomain:toItemAtURL:error:")]
		bool GetRelationship (out NSUrlRelationship outRelationship, NSSearchPathDirectory directory, NSSearchPathDomain domain, NSUrl toItemAtUrl, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("getRelationship:ofDirectoryAtURL:toItemAtURL:error:")]
		bool GetRelationship (out NSUrlRelationship outRelationship, NSUrl directoryURL, NSUrl otherURL, out NSError error);

		[NoTV]
		[NoiOS]
		[NoMacCatalyst]
		[Async (XmlDocs = """
			<param name="url">To be added.</param>
			<param name="mask">To be added.</param>
			<summary>To be added.</summary>
			<returns>To be added.</returns>
			<remarks>To be added.</remarks>
			""")]
		[Export ("unmountVolumeAtURL:options:completionHandler:")]
		void UnmountVolume (NSUrl url, NSFileManagerUnmountOptions mask, Action<NSError> completionHandler);

		[NoTV]
		[MacCatalyst (13, 1)]
		[Async (XmlDocs = """
			<param name="url">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para class="improve-task-t-return-type-description">A task that represents the asynchronous GetFileProviderServices operation.  The value of the TResult parameter is of type System.Action&lt;Foundation.NSDictionary&lt;Foundation.NSString,Foundation.NSFileProviderService&gt;,Foundation.NSError&gt;.</para>
			        </returns>
			<remarks>
			          <para copied="true">The GetFileProviderServicesAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			"""), Export ("getFileProviderServicesForItemAtURL:completionHandler:")]
		void GetFileProviderServices (NSUrl url, Action<NSDictionary<NSString, NSFileProviderService>, NSError> completionHandler);

		[Async]
		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Export ("pauseSyncForUbiquitousItemAtURL:completionHandler:")]
		void PauseSyncForUbiquitousItem (NSUrl url, NSFileManagerSyncForUbiquitousItemHandler completionHandler);

		[Async]
		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Export ("resumeSyncForUbiquitousItemAtURL:withBehavior:completionHandler:")]
		void ResumeSyncForUbiquitousItem (NSUrl url, NSFileManagerResumeSyncBehavior behavior, NSFileManagerSyncForUbiquitousItemHandler completionHandler);

		[Async]
		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Export ("fetchLatestRemoteVersionOfItemAtURL:completionHandler:")]
		void FetchLatestRemoteVersionOfItem (NSUrl url, NSFileManagerFetchLatestRemoteVersionOfItemHandler completionHandler);

		[Async]
		[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
		[Export ("uploadLocalVersionOfUbiquitousItemAtURL:withConflictResolutionPolicy:completionHandler:")]
		void UploadLocalVersionOfUbiquitousItem (NSUrl url, NSFileManagerUploadLocalVersionConflictPolicy conflictResolutionPolicy, NSFileManagerUploadLocalVersionOfUbiquitousItemHandler completionHandler);
	}

	delegate void NSFileManagerSyncForUbiquitousItemHandler ([NullAllowed] NSError error);
	delegate void NSFileManagerFetchLatestRemoteVersionOfItemHandler ([NullAllowed] NSFileVersion latestRemoteVersion, [NullAllowed] NSError error);
	delegate void NSFileManagerUploadLocalVersionOfUbiquitousItemHandler ([NullAllowed] NSFileVersion uploadedVersion, [NullAllowed] NSError error);

	interface INSFileManagerDelegate { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSFileManagerDelegate {
		/// <param name="fm">To be added.</param>
		/// <param name="srcPath">To be added.</param>
		/// <param name="dstPath">To be added.</param>
		/// <summary>Creates a copy of this object, allocating memory from the specified zone or from the default zone if the argument is null.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("fileManager:shouldCopyItemAtPath:toPath:")]
		bool ShouldCopyItemAtPath (NSFileManager fm, NSString srcPath, NSString dstPath);

		/// <param name="fm">To be added.</param>
		/// <param name="srcUrl">To be added.</param>
		/// <param name="dstUrl">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldCopyItemAtURL:toURL:")]
		bool ShouldCopyItemAtUrl (NSFileManager fm, NSUrl srcUrl, NSUrl dstUrl);

		/// <param name="fileManager">To be added.</param>
		/// <param name="srcUrl">To be added.</param>
		/// <param name="dstUrl">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldLinkItemAtURL:toURL:")]
		bool ShouldLinkItemAtUrl (NSFileManager fileManager, NSUrl srcUrl, NSUrl dstUrl);

		/// <param name="fileManager">To be added.</param>
		/// <param name="srcUrl">To be added.</param>
		/// <param name="dstUrl">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldMoveItemAtURL:toURL:")]
		bool ShouldMoveItemAtUrl (NSFileManager fileManager, NSUrl srcUrl, NSUrl dstUrl);

		/// <param name="fileManager">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="srcUrl">To be added.</param>
		/// <param name="dstUrl">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldProceedAfterError:copyingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorCopyingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		/// <param name="fileManager">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="srcUrl">To be added.</param>
		/// <param name="dstUrl">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldProceedAfterError:linkingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorLinkingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		/// <param name="fileManager">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="srcUrl">To be added.</param>
		/// <param name="dstUrl">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldProceedAfterError:movingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorMovingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		/// <param name="fileManager">To be added.</param>
		/// <param name="url">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldRemoveItemAtURL:")]
		bool ShouldRemoveItemAtUrl (NSFileManager fileManager, NSUrl url);

		/// <param name="fileManager">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="url">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldProceedAfterError:removingItemAtURL:")]
		bool ShouldProceedAfterErrorRemovingItem (NSFileManager fileManager, NSError error, NSUrl url);

		/// <param name="fileManager">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="srcPath">To be added.</param>
		/// <param name="dstPath">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("fileManager:shouldProceedAfterError:copyingItemAtPath:toPath:")]
		bool ShouldProceedAfterErrorCopyingItem (NSFileManager fileManager, NSError error, string srcPath, string dstPath);

		/// <param name="fileManager">To be added.</param>
		/// <param name="srcPath">To be added.</param>
		/// <param name="dstPath">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("fileManager:shouldMoveItemAtPath:toPath:")]
		bool ShouldMoveItemAtPath (NSFileManager fileManager, string srcPath, string dstPath);

		/// <param name="fileManager">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="srcPath">To be added.</param>
		/// <param name="dstPath">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("fileManager:shouldProceedAfterError:movingItemAtPath:toPath:")]
		bool ShouldProceedAfterErrorMovingItem (NSFileManager fileManager, NSError error, string srcPath, string dstPath);

		/// <param name="fileManager">To be added.</param>
		/// <param name="srcPath">To be added.</param>
		/// <param name="dstPath">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("fileManager:shouldLinkItemAtPath:toPath:")]
		bool ShouldLinkItemAtPath (NSFileManager fileManager, string srcPath, string dstPath);

		/// <param name="fileManager">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="srcPath">To be added.</param>
		/// <param name="dstPath">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("fileManager:shouldProceedAfterError:linkingItemAtPath:toPath:")]
		bool ShouldProceedAfterErrorLinkingItem (NSFileManager fileManager, NSError error, string srcPath, string dstPath);

		/// <param name="fileManager">To be added.</param>
		/// <param name="path">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("fileManager:shouldRemoveItemAtPath:")]
		bool ShouldRemoveItemAtPath (NSFileManager fileManager, string path);

		/// <param name="fileManager">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <param name="path">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("fileManager:shouldProceedAfterError:removingItemAtPath:")]
		bool ShouldProceedAfterErrorRemovingItem (NSFileManager fileManager, NSError error, string path);
	}

	[Category]
	[BaseType (typeof (NSFileManager))]
	interface NSFileManager_NSUserInformation {

		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoTV]
		[NoiOS]
		[NoMacCatalyst]
		[Export ("homeDirectoryForCurrentUser")]
		NSUrl GetHomeDirectoryForCurrentUser ();

		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[MacCatalyst (13, 1)]
		[Export ("temporaryDirectory")]
		NSUrl GetTemporaryDirectory ();

		/// <param name="userName">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[NoTV]
		[NoiOS]
		[NoMacCatalyst]
		[Export ("homeDirectoryForUser:")]
		[return: NullAllowed]
		NSUrl GetHomeDirectory (string userName);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	partial interface NSFilePresenter {
		/// <summary>Gets URL of presented item.</summary>
		/// <value>
		///         </value>
		/// <remarks>
		///           <para>
		///           </para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[Abstract]
		[Export ("presentedItemURL", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSUrl PresentedItemUrl { get; }

		/// <summary>Gets the <see cref="NSOperationQueue" /> on which presenter-related methods are executed.</summary>
		/// <value>The <see cref="NSOperationQueue" /> on which methods are executed.</value>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("presentedItemOperationQueue", ArgumentSemantic.Retain)]
		NSOperationQueue PresentedItemOperationQueue { get; }

		/// <param name="readerAction">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("relinquishPresentedItemToReader:")]
		void RelinquishPresentedItemToReader (NSFilePresenterReacquirer readerAction);

		/// <param name="writerAction">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("relinquishPresentedItemToWriter:")]
		void RelinquishPresentedItemToWriter (NSFilePresenterReacquirer writerAction);

		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("savePresentedItemChangesWithCompletionHandler:")]
		void SavePresentedItemChanges (Action<NSError?> completionHandler);

		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("accommodatePresentedItemDeletionWithCompletionHandler:")]
		void AccommodatePresentedItemDeletion (Action<NSError?> completionHandler);

		[NoTV, Mac (14, 4), iOS (17, 4), MacCatalyst (17, 4)]
		[Export ("accommodatePresentedItemEvictionWithCompletionHandler:")]
		void AccommodatePresentedItemEviction (Action<NSError?> completionHandler);

		/// <param name="newURL">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("presentedItemDidMoveToURL:")]
		void PresentedItemMoved (NSUrl newURL);

		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("presentedItemDidChange")]
		void PresentedItemChanged ();

		/// <param name="version">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("presentedItemDidGainVersion:")]
		void PresentedItemGainedVersion (NSFileVersion version);

		/// <param name="version">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("presentedItemDidLoseVersion:")]
		void PresentedItemLostVersion (NSFileVersion version);

		/// <param name="version">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("presentedItemDidResolveConflictVersion:")]
		void PresentedItemResolveConflictVersion (NSFileVersion version);

		/// <param name="url">To be added.</param>
		/// <param name="completionHandler">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("accommodatePresentedSubitemDeletionAtURL:completionHandler:")]
		void AccommodatePresentedSubitemDeletion (NSUrl url, Action<NSError?> completionHandler);

		/// <param name="atUrl">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("presentedSubitemDidAppearAtURL:")]
		void PresentedSubitemAppeared (NSUrl atUrl);

		/// <param name="oldURL">To be added.</param>
		/// <param name="newURL">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("presentedSubitemAtURL:didMoveToURL:")]
		void PresentedSubitemMoved (NSUrl oldURL, NSUrl newURL);

		/// <param name="url">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("presentedSubitemDidChangeAtURL:")]
		void PresentedSubitemChanged (NSUrl url);

		/// <param name="url">To be added.</param>
		/// <param name="version">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("presentedSubitemAtURL:didGainVersion:")]
		void PresentedSubitemGainedVersion (NSUrl url, NSFileVersion version);

		/// <param name="url">To be added.</param>
		/// <param name="version">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("presentedSubitemAtURL:didLoseVersion:")]
		void PresentedSubitemLostVersion (NSUrl url, NSFileVersion version);

		/// <param name="url">To be added.</param>
		/// <param name="version">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("presentedSubitemAtURL:didResolveConflictVersion:")]
		void PresentedSubitemResolvedConflictVersion (NSUrl url, NSFileVersion version);

		/// <param name="attributes">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("presentedItemDidChangeUbiquityAttributes:")]
		void PresentedItemChangedUbiquityAttributes (NSSet<NSString> attributes);

		/// <summary>Gets the set of ubiquity attributes that will generate notifications if they are modified.</summary>
		/// <value>To be added.</value>
		/// <remarks>
		///           <para>(More documentation for this node is coming)</para>
		///           <para tool="threads">This can be used from a background thread.</para>
		///         </remarks>
		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("observedPresentedItemUbiquityAttributes", ArgumentSemantic.Strong)]
		NSSet<NSString> PresentedItemObservedUbiquityAttributes { get; }
	}

	delegate void NSFileVersionNonlocalVersionsCompletionHandler ([NullAllowed] NSFileVersion [] nonlocalFileVersions, [NullAllowed] NSError error);

	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: -[NSFileVersion init]: You have to use one of the factory methods to instantiate NSFileVersion.
	[DisableDefaultCtor]
	interface NSFileVersion {
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }

		[Export ("localizedName", ArgumentSemantic.Copy)]
		string LocalizedName { get; }

		[Export ("localizedNameOfSavingComputer", ArgumentSemantic.Copy)]
		string LocalizedNameOfSavingComputer { get; }

		[Export ("modificationDate", ArgumentSemantic.Copy)]
		NSDate ModificationDate { get; }

		[Export ("persistentIdentifier", ArgumentSemantic.Retain)]
		NSObject PersistentIdentifier { get; }

		/// <summary>Whether this NSFileVersion is in conflict with another NSFileVersion. Read-only.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("conflict")]
		bool IsConflict { [Bind ("isConflict")] get; }

		/// <summary>True if this version is not in conflict with another version. App devs should not assign the value false to this property.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("resolved")]
		bool Resolved { [Bind ("isResolved")] get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("discardable")]
		bool Discardable { [Bind ("isDiscardable")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("hasLocalContents")]
		bool HasLocalContents { get; }

		[MacCatalyst (13, 1)]
		[Export ("hasThumbnail")]
		bool HasThumbnail { get; }

		[Static]
		[Export ("currentVersionOfItemAtURL:")]
		NSFileVersion GetCurrentVersion (NSUrl url);

		[MacCatalyst (13, 1)]
		[Static]
		[Async (XmlDocs = """
			<param name="url">To be added.</param>
			<summary>To be added.</summary>
			<returns>
			          <para>A task that represents the asynchronous GetNonlocalVersions operation.   The value of the TResult parameter is a <see cref="Foundation.NSFileVersionNonlocalVersionsCompletionHandler" />.</para>
			        </returns>
			<remarks>
			          <para copied="true">The GetNonlocalVersionsAsync method is suitable to be used with C# async by returning control to the caller with a Task representing the operation.</para>
			          <para copied="true">To be added.</para>
			        </remarks>
			""")]
		[Export ("getNonlocalVersionsOfItemAtURL:completionHandler:")]
		void GetNonlocalVersions (NSUrl url, NSFileVersionNonlocalVersionsCompletionHandler completionHandler);

		[Static]
		[Export ("otherVersionsOfItemAtURL:")]
		NSFileVersion [] GetOtherVersions (NSUrl url);

		[Static]
		[Export ("unresolvedConflictVersionsOfItemAtURL:")]
		NSFileVersion [] GetUnresolvedConflictVersions (NSUrl url);

		[Static]
		[Export ("versionOfItemAtURL:forPersistentIdentifier:")]
		NSFileVersion GetSpecificVersion (NSUrl url, NSObject persistentIdentifier);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[return: NullAllowed]
		[Static]
		[Export ("addVersionOfItemAtURL:withContentsOfURL:options:error:")]
		NSFileVersion AddVersion (NSUrl url, NSUrl contentsURL, NSFileVersionAddingOptions options, out NSError outError);

		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Static]
		[Export ("temporaryDirectoryURLForNewVersionOfItemAtURL:")]
		NSUrl TemporaryDirectoryForItem (NSUrl url);

		[Export ("replaceItemAtURL:options:error:")]
		NSUrl ReplaceItem (NSUrl url, NSFileVersionReplacingOptions options, out NSError error);

		[Export ("removeAndReturnError:")]
		bool Remove (out NSError outError);

		[Static]
		[Export ("removeOtherVersionsOfItemAtURL:error:")]
		bool RemoveOtherVersions (NSUrl url, out NSError outError);

		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("originatorNameComponents", ArgumentSemantic.Copy)]
		NSPersonNameComponents OriginatorNameComponents { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSFileWrapper : NSSecureCoding {
		[DesignatedInitializer]
		[Export ("initWithURL:options:error:")]
		NativeHandle Constructor (NSUrl url, NSFileWrapperReadingOptions options, out NSError outError);

		[DesignatedInitializer]
		[Export ("initDirectoryWithFileWrappers:")]
		NativeHandle Constructor (NSDictionary childrenByPreferredName);

		[DesignatedInitializer]
		[Export ("initRegularFileWithContents:")]
		NativeHandle Constructor (NSData contents);

		[DesignatedInitializer]
		[Export ("initSymbolicLinkWithDestinationURL:")]
		NativeHandle Constructor (NSUrl urlToSymbolicLink);

		// Constructor clash
		//[Export ("initWithSerializedRepresentation:")]
		//NativeHandle Constructor (NSData serializeRepresentation);

		/// <summary>Whether this is a directory file wrapper.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isDirectory")]
		bool IsDirectory { get; }

		/// <summary>Whether this is a regular file wrapper.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isRegularFile")]
		bool IsRegularFile { get; }

		/// <summary>Whether this is a file wrapper for a symbolic link.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isSymbolicLink")]
		bool IsSymbolicLink { get; }

		[Export ("matchesContentsOfURL:")]
		bool MatchesContentsOfURL (NSUrl url);

		[Export ("readFromURL:options:error:")]
		bool Read (NSUrl url, NSFileWrapperReadingOptions options, out NSError outError);

		[Export ("writeToURL:options:originalContentsURL:error:")]
		bool Write (NSUrl url, NSFileWrapperWritingOptions options, [NullAllowed] NSUrl originalContentsURL, out NSError outError);

		[Export ("serializedRepresentation")]
		NSData GetSerializedRepresentation ();

		[Export ("addFileWrapper:")]
		string AddFileWrapper (NSFileWrapper child);

		[Export ("addRegularFileWithContents:preferredFilename:")]
		string AddRegularFile (NSData dataContents, string preferredFilename);

		[Export ("removeFileWrapper:")]
		void RemoveFileWrapper (NSFileWrapper child);

		[Export ("fileWrappers")]
		NSDictionary FileWrappers { get; }

		[Export ("keyForFileWrapper:")]
		string KeyForFileWrapper (NSFileWrapper child);

		[Export ("regularFileContents")]
		NSData GetRegularFileContents ();

		[Export ("symbolicLinkDestinationURL")]
		NSUrl SymbolicLinkDestinationURL { get; }

		//Detected properties
		// [NullAllowed] can't be used. It's null by default but, on device, it throws-n-crash
		// NSInvalidArgumentException -[NSFileWrapper setPreferredFilename:] *** preferredFilename cannot be empty.
		[Export ("preferredFilename")]
		string PreferredFilename { get; set; }

		[NullAllowed] // by default this property is null
		[Export ("filename")]
		string Filename { get; set; }

		[Export ("fileAttributes", ArgumentSemantic.Copy)]
		NSDictionary FileAttributes { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("icon", ArgumentSemantic.Retain)]
		NSImage Icon { get; set; }
	}

	[BaseType (typeof (NSEnumerator))]
	interface NSDirectoryEnumerator {
		[Export ("fileAttributes")]
		NSDictionary FileAttributes { get; }

		[Export ("directoryAttributes")]
		NSDictionary DirectoryAttributes { get; }

		[Export ("skipDescendents")]
#if XAMCORE_5_0
		void SkipDescendants ();
#else
		[Obsolete ("Use 'SkipDescendants' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		void SkipDescendents ();

		[Wrap ("SkipDescendents ()")]
		void SkipDescendants ();
#endif

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("level")]
		nint Level { get; }

		[MacCatalyst (13, 1)]
		[Export ("isEnumeratingDirectoryPostOrder")]
		bool IsEnumeratingDirectoryPostOrder { get; }
	}

	/// <param name="evaluatedObject">To be added.</param>
	/// <param name="bindings">To be added.</param>
	/// <summary>A delegate that represents the expression to use with <see cref="Foundation.NSPredicate.FromExpression(Foundation.NSPredicateEvaluator)" />.</summary>
	/// <returns>To be added.</returns>
	/// <remarks>To be added.</remarks>
	delegate bool NSPredicateEvaluator ([NullAllowed] NSObject evaluatedObject, [NullAllowed] NSDictionary bindings);

	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface NSPredicate : NSSecureCoding, NSCopying {
		[Static]
		[Internal]
		[Export ("predicateWithFormat:argumentArray:")]
		NSPredicate _FromFormat (string predicateFormat, [NullAllowed] NSObject [] arguments);

		[Static, Export ("predicateWithValue:")]
		NSPredicate FromValue (bool value);

		[Static, Export ("predicateWithBlock:")]
		NSPredicate FromExpression (NSPredicateEvaluator evaluator);

		[Export ("predicateFormat")]
		string PredicateFormat { get; }

		[Export ("predicateWithSubstitutionVariables:")]
		NSPredicate PredicateWithSubstitutionVariables (NSDictionary substitutionVariables);

		[Export ("evaluateWithObject:")]
		bool EvaluateWithObject (NSObject obj);

		[Export ("evaluateWithObject:substitutionVariables:")]
		bool EvaluateWithObject (NSObject obj, NSDictionary substitutionVariables);

		[return: NullAllowed]
		[Static]
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("predicateFromMetadataQueryString:")]
		NSPredicate FromMetadataQueryString (string query);

		[MacCatalyst (13, 1)]
		[Export ("allowEvaluation")]
		void AllowEvaluation ();

		/// <summary>Validates this predicate using the specified validator and, if valid, allows it to be evaluated.</summary>
		/// <param name="validator">The <see cref="INSPredicateValidating" /> object used to validate the predicate before allowing evaluation.</param>
		/// <param name="error">When this method returns <see langword="false" />, contains an <see cref="NSError" /> describing the validation failure; otherwise, <see langword="null" />.</param>
		/// <returns><see langword="true" /> if the predicate passed validation and is now allowed to be evaluated; otherwise, <see langword="false" />.</returns>
		[iOS (26, 4), TV (26, 4), Mac (26, 4), MacCatalyst (26, 4)]
		[Export ("allowEvaluationWithValidator:error:")]
		bool AllowEvaluation (INSPredicateValidating validator, [NullAllowed] out NSError error);
	}

	/// <summary>Protocol interface that represents the methods declared by the <see cref="NSPredicateValidating" /> protocol.</summary>
	interface INSPredicateValidating { }

	/// <summary>Provides custom validation logic for <see cref="NSPredicate" /> and <see cref="NSExpression" /> objects before they are evaluated.</summary>
	/// <remarks>
	///   <para>Implement this protocol to control which predicates and expressions are considered safe for evaluation. Each visitor method is called during
	///   validation, allowing the implementation to inspect and approve or reject individual components of a predicate tree.</para>
	/// </remarks>
	[iOS (26, 4), TV (26, 4), Mac (26, 4), MacCatalyst (26, 4)]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	[BaseType (typeof (NSObject))]
	interface NSPredicateValidating {
		/// <summary>Validates whether the specified predicate is allowed to be evaluated.</summary>
		/// <param name="predicate">The <see cref="NSPredicate" /> to validate.</param>
		/// <param name="error">When this method returns <see langword="false" />, contains an <see cref="NSError" /> describing why the predicate was rejected; otherwise, <see langword="null" />.</param>
		/// <returns><see langword="true" /> if the predicate is valid and allowed; otherwise, <see langword="false" />.</returns>
		[Export ("visitPredicate:error:")]
		bool VisitPredicate (NSPredicate predicate, [NullAllowed] out NSError error);

		/// <summary>Validates whether the specified expression is allowed to be evaluated.</summary>
		/// <param name="expression">The <see cref="NSExpression" /> to validate.</param>
		/// <param name="error">When this method returns <see langword="false" />, contains an <see cref="NSError" /> describing why the expression was rejected; otherwise, <see langword="null" />.</param>
		/// <returns><see langword="true" /> if the expression is valid and allowed; otherwise, <see langword="false" />.</returns>
		[Export ("visitExpression:error:")]
		bool VisitExpression (NSExpression expression, [NullAllowed] out NSError error);

		/// <summary>Validates whether the specified comparison operator type is allowed to be used in a predicate.</summary>
		/// <param name="operatorType">The <see cref="NSPredicateOperatorType" /> to validate.</param>
		/// <param name="error">When this method returns <see langword="false" />, contains an <see cref="NSError" /> describing why the operator type was rejected; otherwise, <see langword="null" />.</param>
		/// <returns><see langword="true" /> if the operator type is valid and allowed; otherwise, <see langword="false" />.</returns>
		[Export ("visitOperatorType:error:")]
		bool VisitOperatorType (NSPredicateOperatorType operatorType, [NullAllowed] out NSError error);

		/// <summary>Validates whether the specified key path expression is allowed to be evaluated.</summary>
		/// <param name="expression">The key path <see cref="NSExpression" /> to validate.</param>
		/// <param name="scope">The scope component of the key path, or <see langword="null" /> if no scope is specified.</param>
		/// <param name="key">The key component of the key path, or <see langword="null" /> if no key is specified.</param>
		/// <param name="error">When this method returns <see langword="false" />, contains an <see cref="NSError" /> describing why the key path expression was rejected; otherwise, <see langword="null" />.</param>
		/// <returns><see langword="true" /> if the key path expression is valid and allowed; otherwise, <see langword="false" />.</returns>
		[Export ("visitExpressionKeyPath:scope:key:error:")]
		bool VisitExpressionKeyPath (NSExpression expression, [NullAllowed] string scope, [NullAllowed] string key, [NullAllowed] out NSError error);
	}

	/// <summary>Defines an extension method for <see cref="Foundation.NSOrderedSet" /> objects allowing them to be filtered via an <see cref="Foundation.NSPredicate" />.</summary>
	/// <remarks>To be added.</remarks>
	[Category, BaseType (typeof (NSOrderedSet))]
	partial interface NSPredicateSupport_NSOrderedSet {
		/// <param name="p">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("filteredOrderedSetUsingPredicate:")]
		NSOrderedSet FilterUsingPredicate (NSPredicate p);
	}

	/// <summary>Defines an extension method for <see cref="Foundation.NSMutableOrderedSet" /> objects allowing them to be filtered using a <see cref="Foundation.NSPredicate" />.</summary>
	/// <remarks>To be added.</remarks>
	[Category, BaseType (typeof (NSMutableOrderedSet))]
	partial interface NSPredicateSupport_NSMutableOrderedSet {
		/// <param name="p">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("filterUsingPredicate:")]
		void FilterUsingPredicate (NSPredicate p);
	}

	/// <summary>Extension method for <see cref="Foundation.NSArray" /> objects, allowing them to be filtered with a <see cref="Foundation.NSPredicate" />.</summary>
	/// <remarks>To be added.</remarks>
	[Category, BaseType (typeof (NSArray))]
	partial interface NSPredicateSupport_NSArray {
		/// <param name="array">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("filteredArrayUsingPredicate:")]
		NSArray FilterUsingPredicate (NSArray array);
	}

#pragma warning disable 618
	/// <summary>Helper metohds for applying predicates to mutable arrays.</summary>
	/// <remarks>
	///     </remarks>
	[Category, BaseType (typeof (NSMutableArray))]
#pragma warning restore 618
	partial interface NSPredicateSupport_NSMutableArray {
		/// <param name="predicate">The predicate used to filter the
		/// 	elements of the array.</param>
		/// <summary>Filters the element of the array in place, by keeping
		/// 	only the elements that match.</summary>
		/// <remarks>
		///         </remarks>
		[Export ("filterUsingPredicate:")]
		void FilterUsingPredicate (NSPredicate predicate);
	}

	/// <summary>Helper methods for applying predicates to sets.</summary>
	/// <remarks>To be added.</remarks>
	[Category, BaseType (typeof (NSSet))]
	partial interface NSPredicateSupport_NSSet {
		/// <param name="predicate">The predicate used to filter the
		/// 	elements of the set.</param>
		/// <summary>Returns a new set that contains the elements that
		/// 	match the predicate.</summary>
		/// <returns>A new immutable set.</returns>
		/// <remarks>
		///         </remarks>
		[Export ("filteredSetUsingPredicate:")]
		NSSet FilterUsingPredicate (NSPredicate predicate);
	}

	/// <summary>Extension method for <see cref="Foundation.NSMutableSet" /> objects, allowing them to be filtered with a <see cref="Foundation.NSPredicate" />.</summary>
	/// <remarks>To be added.</remarks>
	[Category, BaseType (typeof (NSMutableSet))]
	partial interface NSPredicateSupport_NSMutableSet {
		/// <param name="predicate">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("filterUsingPredicate:")]
		void FilterUsingPredicate (NSPredicate predicate);
	}

	[NoiOS]
	[NoMacCatalyst]
	[NoTV]
	[BaseType (typeof (NSObject), Name = "NSURLDownload")]
	interface NSUrlDownload {
		[Static, Export ("canResumeDownloadDecodedWithEncodingMIMEType:")]
		bool CanResumeDownloadDecodedWithEncodingMimeType (string mimeType);

		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSURLSession' instead.")]
		[Export ("initWithRequest:delegate:")]
		NativeHandle Constructor (NSUrlRequest request, [NullAllowed] NSObject delegate1);

		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSURLSession' instead.")]
		[Export ("initWithResumeData:delegate:path:")]
		NativeHandle Constructor (NSData resumeData, [NullAllowed] NSObject delegate1, string path);

		[Export ("cancel")]
		void Cancel ();

		[Export ("setDestination:allowOverwrite:")]
		void SetDestination (string path, bool allowOverwrite);

		[Export ("request")]
		NSUrlRequest Request { get; }

		[NullAllowed]
		[Export ("resumeData")]
		NSData ResumeData { get; }

		[Export ("deletesFileUponFailure")]
		bool DeletesFileUponFailure { get; set; }
	}

	[NoiOS]
	[NoMacCatalyst]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (Name = "NSURLDownloadDelegate")]
	interface NSUrlDownloadDelegate {
		/// <param name="download">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("downloadDidBegin:")]
		void DownloadBegan (NSUrlDownload download);

		/// <param name="download">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <param name="redirectResponse">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("download:willSendRequest:redirectResponse:")]
		NSUrlRequest WillSendRequest (NSUrlDownload download, NSUrlRequest request, NSUrlResponse redirectResponse);

		/// <param name="download">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("download:didReceiveAuthenticationChallenge:")]
		void ReceivedAuthenticationChallenge (NSUrlDownload download, NSUrlAuthenticationChallenge challenge);

		/// <param name="download">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("download:didCancelAuthenticationChallenge:")]
		void CanceledAuthenticationChallenge (NSUrlDownload download, NSUrlAuthenticationChallenge challenge);

		/// <param name="download">To be added.</param>
		/// <param name="response">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("download:didReceiveResponse:")]
		void ReceivedResponse (NSUrlDownload download, NSUrlResponse response);

		//- (void)download:(NSUrlDownload *)download willResumeWithResponse:(NSUrlResponse *)response fromByte:(long long)startingByte;
		/// <param name="download">To be added.</param>
		/// <param name="response">To be added.</param>
		/// <param name="startingByte">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("download:willResumeWithResponse:fromByte:")]
		void Resume (NSUrlDownload download, NSUrlResponse response, long startingByte);

		//- (void)download:(NSUrlDownload *)download didReceiveDataOfLength:(NSUInteger)length;
		/// <param name="download">To be added.</param>
		/// <param name="length">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("download:didReceiveDataOfLength:")]
		void ReceivedData (NSUrlDownload download, nuint length);

		/// <param name="download">To be added.</param>
		/// <param name="encodingType">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("download:shouldDecodeSourceDataOfMIMEType:")]
		bool DecodeSourceData (NSUrlDownload download, string encodingType);

		/// <param name="download">To be added.</param>
		/// <param name="suggestedFilename">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("download:decideDestinationWithSuggestedFilename:")]
		void DecideDestination (NSUrlDownload download, string suggestedFilename);

		/// <param name="download">To be added.</param>
		/// <param name="path">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("download:didCreateDestination:")]
		void CreatedDestination (NSUrlDownload download, string path);

		/// <param name="download">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("downloadDidFinish:")]
		void Finished (NSUrlDownload download);

		/// <param name="download">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("download:didFailWithError:")]
		void FailedWithError (NSUrlDownload download, NSError error);
	}

	// Users are not supposed to implement the NSUrlProtocolClient protocol, they're 
	// only supposed to consume it. This is why there's no model for this protocol.
	/// <summary>The URL protocol client category.</summary>
	/// <remarks>To be added.</remarks>
	[Protocol (Name = "NSURLProtocolClient")]
	interface NSUrlProtocolClient {
		/// <param name="protocol">To be added.</param>
		/// <param name="redirectedToEequest">To be added.</param>
		/// <param name="redirectResponse">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("URLProtocol:wasRedirectedToRequest:redirectResponse:")]
		void Redirected (NSUrlProtocol protocol, NSUrlRequest redirectedToEequest, NSUrlResponse redirectResponse);

		/// <param name="protocol">To be added.</param>
		/// <param name="cachedResponse">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("URLProtocol:cachedResponseIsValid:")]
		void CachedResponseIsValid (NSUrlProtocol protocol, NSCachedUrlResponse cachedResponse);

		/// <param name="protocol">To be added.</param>
		/// <param name="response">To be added.</param>
		/// <param name="policy">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("URLProtocol:didReceiveResponse:cacheStoragePolicy:")]
		void ReceivedResponse (NSUrlProtocol protocol, NSUrlResponse response, NSUrlCacheStoragePolicy policy);

		/// <param name="protocol">To be added.</param>
		/// <param name="data">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("URLProtocol:didLoadData:")]
		void DataLoaded (NSUrlProtocol protocol, NSData data);

		/// <param name="protocol">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("URLProtocolDidFinishLoading:")]
		void FinishedLoading (NSUrlProtocol protocol);

		/// <param name="protocol">To be added.</param>
		/// <param name="error">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("URLProtocol:didFailWithError:")]
		void FailedWithError (NSUrlProtocol protocol, NSError error);

		/// <param name="protocol">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("URLProtocol:didReceiveAuthenticationChallenge:")]
		void ReceivedAuthenticationChallenge (NSUrlProtocol protocol, NSUrlAuthenticationChallenge challenge);

		/// <param name="protocol">To be added.</param>
		/// <param name="challenge">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("URLProtocol:didCancelAuthenticationChallenge:")]
		void CancelledAuthenticationChallenge (NSUrlProtocol protocol, NSUrlAuthenticationChallenge challenge);
	}

	interface INSUrlProtocolClient { }

	[BaseType (typeof (NSObject),
		   Name = "NSURLProtocol",
		   Delegates = new string [] { "WeakClient" })]
	interface NSUrlProtocol {
		[DesignatedInitializer]
		[Export ("initWithRequest:cachedResponse:client:")]
		NativeHandle Constructor (NSUrlRequest request, [NullAllowed] NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client);

		[Export ("client")]
		INSUrlProtocolClient Client { get; }

		[Export ("request")]
		NSUrlRequest Request { get; }

		[Export ("cachedResponse")]
		NSCachedUrlResponse CachedResponse { get; }

		[Static]
		[Export ("canInitWithRequest:")]
		bool CanInitWithRequest (NSUrlRequest request);

		[Static]
		[Export ("canonicalRequestForRequest:")]
		NSUrlRequest GetCanonicalRequest (NSUrlRequest forRequest);

		[Static]
		[Export ("requestIsCacheEquivalent:toRequest:")]
		bool IsRequestCacheEquivalent (NSUrlRequest first, NSUrlRequest second);

		[Export ("startLoading")]
		void StartLoading ();

		[Export ("stopLoading")]
		void StopLoading ();

		[Static]
		[Export ("propertyForKey:inRequest:")]
		NSObject GetProperty (string key, NSUrlRequest inRequest);

		[Static]
		[Export ("setProperty:forKey:inRequest:")]
		void SetProperty ([NullAllowed] NSObject value, string key, NSMutableUrlRequest inRequest);

		[Static]
		[Export ("removePropertyForKey:inRequest:")]
		void RemoveProperty (string propertyKey, NSMutableUrlRequest request);

		[Static]
		[Export ("registerClass:")]
		bool RegisterClass (Class protocolClass);

		[Static]
		[Export ("unregisterClass:")]
		void UnregisterClass (Class protocolClass);

		// Commented API are broken and we'll need to provide a workaround for them
		// https://trello.com/c/RthKXnyu/381-disabled-nsurlprotocol-api-reminder

		// * "task" does not answer and is not usable - maybe it only works if created from the new API ?!?
		//
		// * "canInitWithTask" can't be called as a .NET static method. The ObjC code uses the current type
		//    internally (which will always be NSURLProtocol in .NET never a subclass) and complains about it
		//    being abstract (which is true)
		//    -canInitWithRequest: cannot be sent to an abstract object of class NSURLProtocol: Create a concrete instance!

		//		
		//		[Export ("initWithTask:cachedResponse:client:")]
		//		NativeHandle Constructor (NSUrlSessionTask task, [NullAllowed] NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client);
		//
		//		
		//		[Export ("task", ArgumentSemantic.Copy)]
		//		NSUrlSessionTask Task { get; }
		//
		//		
		//		[Static, Export ("canInitWithTask:")]
		//		bool CanInitWithTask (NSUrlSessionTask task);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSPropertyListSerialization {
		[Static, Export ("dataWithPropertyList:format:options:error:")]
		NSData DataWithPropertyList (NSObject plist, NSPropertyListFormat format,
			NSPropertyListWriteOptions options, out NSError error);

		[Static, Export ("writePropertyList:toStream:format:options:error:")]
		nint WritePropertyList (NSObject plist, NSOutputStream stream, NSPropertyListFormat format,
			NSPropertyListWriteOptions options, out NSError error);

		[Static, Export ("propertyListWithData:options:format:error:")]
		NSObject PropertyListWithData (NSData data, NSPropertyListReadOptions options,
			ref NSPropertyListFormat format, out NSError error);

		[Static, Export ("propertyListWithStream:options:format:error:")]
		NSObject PropertyListWithStream (NSInputStream stream, NSPropertyListReadOptions options,
			ref NSPropertyListFormat format, out NSError error);

		[Static, Export ("propertyList:isValidForFormat:")]
		bool IsValidForFormat (NSObject plist, NSPropertyListFormat format);
	}

	interface INSExtensionRequestHandling { }

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NSExtensionRequestHandling {
		/// <param name="context">The <see cref="NSExtensionContext" /> containing extension-relevant data.</param>
		/// <summary>Developers can implement this method to prepare their extension for the host application request.</summary>
		/// <remarks>
		///           <para>Developers who implement this method must call <c>base.BeginRequestWithExtensionContext(context)</c> within their implementation.</para>
		///         </remarks>
		[Abstract]
		// @required - (void)beginRequestWithExtensionContext:(NSExtensionContext *)context;
		[Export ("beginRequestWithExtensionContext:")]
		void BeginRequestWithExtensionContext (NSExtensionContext context);
	}

	[Protocol]
	interface NSLocking {

		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("lock")]
		void Lock ();

		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Abstract]
		[Export ("unlock")]
		void Unlock ();
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // An uncaught exception was raised: *** -range cannot be sent to an abstract object of class NSTextCheckingResult: Create a concrete instance!
	interface NSTextCheckingResult : NSSecureCoding, NSCopying {
		[Export ("resultType")]
		NSTextCheckingType ResultType { get; }

		[Export ("range")]
		NSRange Range { get; }

		// From the NSTextCheckingResultOptional category on NSTextCheckingResult
		[Export ("orthography")]
		NSOrthography Orthography { get; }

		[Export ("grammarDetails")]
		string [] GrammarDetails { get; }

		[Export ("date")]
		NSDate Date { get; }

		[Export ("timeZone")]
		NSTimeZone TimeZone { get; }

		[Export ("duration")]
		double TimeInterval { get; }

		[Export ("components")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSDictionary WeakComponents { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Wrap ("WeakComponents")]
		NSTextCheckingTransitComponents Components { get; }

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("replacementString")]
		string ReplacementString { get; }

		[Export ("alternativeStrings")]
		[MacCatalyst (13, 1)]
		string [] AlternativeStrings { get; }

		//		NSRegularExpression isn't bound
		//		[Export ("regularExpression")]
		//		NSRegularExpression RegularExpression { get; }

		[Export ("phoneNumber")]
		string PhoneNumber { get; }

		[Export ("addressComponents")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSDictionary WeakAddressComponents { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Wrap ("WeakAddressComponents")]
		NSTextCheckingAddressComponents AddressComponents { get; }

		[Export ("numberOfRanges")]
		nuint NumberOfRanges { get; }

		/// <param name="idx">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("rangeAtIndex:")]
		NSRange RangeAtIndex (nuint idx);

		/// <param name="offset">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("resultByAdjustingRangesWithOffset:")]
		NSTextCheckingResult ResultByAdjustingRanges (nint offset);

		// From the NSTextCheckingResultCreation category on NSTextCheckingResult

		[Static]
		[Export ("orthographyCheckingResultWithRange:orthography:")]
		NSTextCheckingResult OrthographyCheckingResult (NSRange range, NSOrthography ortography);

		[Static]
		[Export ("spellCheckingResultWithRange:")]
		NSTextCheckingResult SpellCheckingResult (NSRange range);

		[Static]
		[Export ("grammarCheckingResultWithRange:details:")]
		NSTextCheckingResult GrammarCheckingResult (NSRange range, string [] details);

		[Static]
		[Export ("dateCheckingResultWithRange:date:")]
		NSTextCheckingResult DateCheckingResult (NSRange range, NSDate date);

		[Static]
		[Export ("dateCheckingResultWithRange:date:timeZone:duration:")]
		NSTextCheckingResult DateCheckingResult (NSRange range, NSDate date, NSTimeZone timezone, double duration);

		[Static]
		[Export ("addressCheckingResultWithRange:components:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSTextCheckingResult AddressCheckingResult (NSRange range, NSDictionary components);

		/// <param name="range">To be added.</param>
		///         <param name="components">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Wrap ("AddressCheckingResult (range, components.GetDictionary ()!)")]
		NSTextCheckingResult AddressCheckingResult (NSRange range, NSTextCheckingAddressComponents components);

		[Static]
		[Export ("linkCheckingResultWithRange:URL:")]
		NSTextCheckingResult LinkCheckingResult (NSRange range, NSUrl url);

		[Static]
		[Export ("quoteCheckingResultWithRange:replacementString:")]
		NSTextCheckingResult QuoteCheckingResult (NSRange range, NSString replacementString);

		[Static]
		[Export ("dashCheckingResultWithRange:replacementString:")]
		NSTextCheckingResult DashCheckingResult (NSRange range, string replacementString);

		[Static]
		[Export ("replacementCheckingResultWithRange:replacementString:")]
		NSTextCheckingResult ReplacementCheckingResult (NSRange range, string replacementString);

		[Static]
		[Export ("correctionCheckingResultWithRange:replacementString:")]
		NSTextCheckingResult CorrectionCheckingResult (NSRange range, string replacementString);

		[Static]
		[Export ("correctionCheckingResultWithRange:replacementString:alternativeStrings:")]
		[MacCatalyst (13, 1)]
		NSTextCheckingResult CorrectionCheckingResult (NSRange range, string replacementString, string [] alternativeStrings);

		//		NSRegularExpression isn't bound
		//		[Export ("regularExpressionCheckingResultWithRanges:count:regularExpression:")]
		//		[Internal] // FIXME
		//		NSTextCheckingResult RegularExpressionCheckingResult (ref NSRange ranges, nuint count, NSRegularExpression regularExpression);

		[Static]
		[Export ("phoneNumberCheckingResultWithRange:phoneNumber:")]
		NSTextCheckingResult PhoneNumberCheckingResult (NSRange range, string phoneNumber);

		[Static]
		[Export ("transitInformationCheckingResultWithRange:components:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSTextCheckingResult TransitInformationCheckingResult (NSRange range, NSDictionary components);

		/// <param name="range">To be added.</param>
		///         <param name="components">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[Static]
		[Wrap ("TransitInformationCheckingResult (range, components.GetDictionary ()!)")]
		NSTextCheckingResult TransitInformationCheckingResult (NSRange range, NSTextCheckingTransitComponents components);

		[MacCatalyst (13, 1)]
		[Export ("rangeWithName:")]
		NSRange GetRange (string name);

	}

	/// <summary>Contains the components of a recognized travel data.</summary>
	/// <remarks>To be added.</remarks>
	[StrongDictionary ("NSTextChecking")]
	interface NSTextCheckingTransitComponents {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string Airline { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string Flight { get; }
	}

	/// <summary>Contains the components of a recognized address.</summary>
	/// <remarks>To be added.</remarks>
	[StrongDictionary ("NSTextChecking")]
	interface NSTextCheckingAddressComponents {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string Name { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string JobTitle { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string Organization { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string Street { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string City { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string State { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("ZipKey")]
		string ZIP { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string Country { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string Phone { get; }
	}

	/// <summary>Contains keys that identify text checking results.</summary>
	/// <remarks>To be added.</remarks>
	[Static]
	interface NSTextChecking {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextCheckingNameKey")]
		NSString NameKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextCheckingJobTitleKey")]
		NSString JobTitleKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextCheckingOrganizationKey")]
		NSString OrganizationKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextCheckingStreetKey")]
		NSString StreetKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextCheckingCityKey")]
		NSString CityKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextCheckingStateKey")]
		NSString StateKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextCheckingZIPKey")]
		NSString ZipKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextCheckingCountryKey")]
		NSString CountryKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextCheckingPhoneKey")]
		NSString PhoneKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextCheckingAirlineKey")]
		NSString AirlineKey { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSTextCheckingFlightKey")]
		NSString FlightKey { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSLock : NSLocking {
		[Export ("tryLock")]
		bool TryLock ();

		[Export ("lockBeforeDate:")]
		bool LockBeforeDate (NSDate limit);

		[Export ("name")]
		[NullAllowed]
		string Name { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSConditionLock : NSLocking {

		/// <param name="condition">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[DesignatedInitializer]
		[Export ("initWithCondition:")]
		NativeHandle Constructor (nint condition);

		[Export ("condition")]
		nint Condition { get; }

		/// <param name="condition">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("lockWhenCondition:")]
		void LockWhenCondition (nint condition);

		[Export ("tryLock")]
		bool TryLock ();

		/// <param name="condition">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("tryLockWhenCondition:")]
		bool TryLockWhenCondition (nint condition);

		/// <param name="condition">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("unlockWithCondition:")]
		void UnlockWithCondition (nint condition);

		[Export ("lockBeforeDate:")]
		bool LockBeforeDate (NSDate limit);

		/// <param name="condition">To be added.</param>
		/// <param name="limit">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("lockWhenCondition:beforeDate:")]
		bool LockWhenCondition (nint condition, NSDate limit);

		[Export ("name")]
		[NullAllowed]
		string Name { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSRecursiveLock : NSLocking {
		[Export ("tryLock")]
		bool TryLock ();

		[Export ("lockBeforeDate:")]
		bool LockBeforeDate (NSDate limit);

		[Export ("name")]
		[NullAllowed]
		string Name { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface NSCondition : NSLocking {
		[Export ("wait")]
		void Wait ();

		[Export ("waitUntilDate:")]
		bool WaitUntilDate (NSDate limit);

		[Export ("signal")]
		void Signal ();

		[Export ("broadcast")]
		void Broadcast ();

		[Export ("name")]
		[NullAllowed]
		string Name { get; set; }
	}

	// Not yet, the IntPtr[] argument isn't handled correctly by the generator (it tries to convert to NSArray, while the native method expects a C array).
	//	[Protocol]
	//	interface NSFastEnumeration {
	//		[Abstract]
	//		[Export ("countByEnumeratingWithState:objects:count:")]
	//		nuint Enumerate (ref NSFastEnumerationState state, IntPtr[] objects, nuint count);
	//	}

	// Placeholer, just so we can start flagging things
	interface INSFastEnumeration { }

	partial interface NSBundle {
		// - (NSImage *)imageForResource:(NSString *)name NS_AVAILABLE_MAC(10_7);
		/// <param name="name">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Export ("imageForResource:")]
		[return: NullAllowed]
		NSImage ImageForResource (string name);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSDateInterval : NSCopying, NSSecureCoding {
		[Export ("startDate", ArgumentSemantic.Copy)]
		NSDate StartDate { get; }

		[Export ("endDate", ArgumentSemantic.Copy)]
		NSDate EndDate { get; }

		[Export ("duration")]
		double Duration { get; }

		[Export ("initWithStartDate:duration:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSDate startDate, double duration);

		[Export ("initWithStartDate:endDate:")]
		NativeHandle Constructor (NSDate startDate, NSDate endDate);

		[Export ("compare:")]
		NSComparisonResult Compare (NSDateInterval dateInterval);

		[Export ("isEqualToDateInterval:")]
		bool IsEqualTo (NSDateInterval dateInterval);

		[Export ("intersectsDateInterval:")]
		bool Intersects (NSDateInterval dateInterval);

		[Export ("intersectionWithDateInterval:")]
		[return: NullAllowed]
		NSDateInterval GetIntersection (NSDateInterval dateInterval);

		[Export ("containsDate:")]
		bool ContainsDate (NSDate date);
	}

	[DisableDefaultCtor]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSUnit : NSCopying, NSSecureCoding {
		[Export ("symbol")]
		string Symbol { get; }

		[Export ("initWithSymbol:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSUnitConverter {
		[Export ("baseUnitValueFromValue:")]
		double GetBaseUnitValue (double value);

		[Export ("valueFromBaseUnitValue:")]
		double GetValue (double baseUnitValue);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSUnitConverter))]
	interface NSUnitConverterLinear : NSSecureCoding {

		[Export ("coefficient")]
		double Coefficient { get; }

		[Export ("constant")]
		double Constant { get; }

		[Export ("initWithCoefficient:")]
		NativeHandle Constructor (double coefficient);

		[Export ("initWithCoefficient:constant:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double coefficient, double constant);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSUnit))]
	[Abstract] // abstract subclass of NSUnit
	[DisableDefaultCtor] // there's a designated initializer
	interface NSDimension : NSSecureCoding {
		// Inlined from base type
		[Export ("initWithSymbol:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol);

		[Export ("converter", ArgumentSemantic.Copy)]
		NSUnitConverter Converter { get; }

		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		// needs to be overriden in suubclasses
		//	NSInvalidArgumentException Reason: *** You must override baseUnit in your class NSDimension to define its base unit.
		// we provide a basic, managed, implementation that throws with a similar message
		//[Static]
		//[Export ("baseUnit")]
		//NSDimension BaseUnit { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	[DisableDefaultCtor] // base type has a designated initializer
	interface NSUnitTemperature : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("kelvin", ArgumentSemantic.Copy)]
		NSUnitTemperature Kelvin { get; }

		[Static]
		[Export ("celsius", ArgumentSemantic.Copy)]
		NSUnitTemperature Celsius { get; }

		[Static]
		[Export ("fahrenheit", ArgumentSemantic.Copy)]
		NSUnitTemperature Fahrenheit { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	partial interface NSFileManager {

		[NoTV]
		[MacCatalyst (13, 1)]
		[Export ("trashItemAtURL:resultingItemURL:error:")]
		bool TrashItem (NSUrl url, out NSUrl resultingItemUrl, out NSError error);

		/// <param name="authorization">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[Static]
		[Export ("fileManagerWithAuthorization:")]
		NSFileManager FromAuthorization (NSWorkspaceAuthorization authorization);
	}

	[NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSFileProviderService {
		[Export ("name")]
		string Name { get; }

		[Async]
		[Export ("getFileProviderConnectionWithCompletionHandler:")]
		void GetFileProviderConnection (Action<NSXpcConnection, NSError> completionHandler);
	}

#if MONOMAC
	partial interface NSFilePresenter {
		/// <summary>To be added.</summary>
		/// <value>To be added.</value>
		/// <remarks>To be added.</remarks>
		[NoiOS]
		[NoMacCatalyst]
		[NoTV]
		[NullAllowed]
		[Export ("primaryPresentedItemURL")]
		NSUrl PrimaryPresentedItemUrl { get; }
	}

	[NoiOS]
	[NoMacCatalyst]
	[NoTV]
	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the Network.framework instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	partial interface NSHost {

		[Static, Internal, Export ("currentHost")]
		NSHost _Current { get; }

		[Static, Internal, Export ("hostWithName:")]
		NSHost _FromName ([NullAllowed] string name);

		[Static, Internal, Export ("hostWithAddress:")]
		NSHost _FromAddress (string address);

		[Export ("isEqualToHost:")]
		bool Equals (NSHost host);

		[NullAllowed]
		[Export ("name")]
		string Name { get; }

		[NullAllowed]
		[Export ("localizedName")]
		string LocalizedName { get; }

		[Export ("names")]
		string [] Names { get; }

		[NullAllowed]
		[Internal, Export ("address")]
		string _Address { get; }

		[Internal, Export ("addresses")]
		string [] _Addresses { get; }

		[Export ("hash"), Internal]
		nuint _Hash { get; }

		/* Deprecated, here for completeness:

		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_7)]
		[Static, Export ("setHostCacheEnabled:")]
		void SetHostCacheEnabled (bool flag);

		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_7)]
		[Static, Export ("isHostCacheEnabled")]
		bool IsHostCacheEnabled ();

		[Availability (Introduced = Platform.Mac_10_0, Deprecated = Platform.Mac_10_7)]
		[Static, Export ("flushHostCache")]
		void FlushHostCache ();
		*/
	}
#endif

	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	[MacCatalyst (15, 0)]
	[NoiOS]
	[NoTV]
	partial interface NSScriptCommand : NSCoding {

		[Internal]
		[DesignatedInitializer]
		[Export ("initWithCommandDescription:")]
		NativeHandle Constructor (NSScriptCommandDescription cmdDescription);

		[Internal]
		[Static]
		[Export ("currentCommand")]
		IntPtr GetCurrentCommand ();

		[Export ("appleEvent")]
		[NullAllowed]
		NSAppleEventDescriptor AppleEvent { get; }

		[Export ("executeCommand")]
		IntPtr Execute ();

		[NullAllowed]
		[Export ("evaluatedReceivers")]
		NSObject EvaluatedReceivers { get; }
	}

	[NoiOS]
	[NoTV]
	[MacCatalyst (15, 0)]
	[StrongDictionary ("NSScriptCommandArgumentDescriptionKeys")]
	partial interface NSScriptCommandArgumentDescription {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string AppleEventCode { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string Type { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string Optional { get; set; }
	}

	[NoiOS]
	[NoTV]
	[MacCatalyst (15, 0)]
	[StrongDictionary ("NSScriptCommandDescriptionDictionaryKeys")]
	partial interface NSScriptCommandDescriptionDictionary {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string CommandClass { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string AppleEventCode { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string AppleEventClassCode { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string Type { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		string ResultAppleEventCode { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		NSMutableDictionary Arguments { get; set; }
	}

	[NoiOS]
	[NoTV]
	[MacCatalyst (15, 0)]
	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface NSScriptCommandDescription : NSCoding {

		[Internal]
		[DesignatedInitializer]
		[Export ("initWithSuiteName:commandName:dictionary:")]
		NativeHandle Constructor (NSString suiteName, NSString commandName, [NullAllowed] NSDictionary commandDeclaration);

		[Internal]
		[Export ("appleEventClassCode")]
		int FCCAppleEventClassCode { get; }

		[Internal]
		[Export ("appleEventCode")]
		int FCCAppleEventCode { get; }

		[Export ("commandClassName")]
		string ClassName { get; }

		[Export ("commandName")]
		string Name { get; }

		[Export ("suiteName")]
		string SuitName { get; }

		[Internal]
		[Export ("appleEventCodeForArgumentWithName:")]
		int FCCAppleEventCodeForArgument (NSString name);

		[Export ("argumentNames")]
		string [] ArgumentNames { get; }

		[Internal]
		[Export ("isOptionalArgumentWithName:")]
		bool NSIsOptionalArgument (NSString name);

		[return: NullAllowed]
		[Internal]
		[Export ("typeForArgumentWithName:")]
		NSString GetNSTypeForArgument (NSString name);

		[Internal]
		[Export ("appleEventCodeForReturnType")]
		int FCCAppleEventCodeForReturnType { get; }

		[NullAllowed]
		[Export ("returnType")]
		string ReturnType { get; }

		[Internal]
		[Export ("createCommandInstance")]
		IntPtr CreateCommandInstancePtr ();
	}

	[NoiOS, NoTV]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	[MacCatalyst (13, 1)]
	interface NSAffineTransform : NSSecureCoding, NSCopying {
		[Export ("initWithTransform:")]
		NativeHandle Constructor (NSAffineTransform transform);

		/// <param name="deltaX">To be added.</param>
		/// <param name="deltaY">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("translateXBy:yBy:")]
		void Translate (nfloat deltaX, nfloat deltaY);

		/// <param name="angle">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("rotateByDegrees:")]
		void RotateByDegrees (nfloat angle);

		/// <param name="angle">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("rotateByRadians:")]
		void RotateByRadians (nfloat angle);

		/// <param name="scale">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("scaleBy:")]
		void Scale (nfloat scale);

		/// <param name="scaleX">To be added.</param>
		/// <param name="scaleY">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("scaleXBy:yBy:")]
		void Scale (nfloat scaleX, nfloat scaleY);

		[Export ("invert")]
		void Invert ();

		[Export ("appendTransform:")]
		void AppendTransform (NSAffineTransform transform);

		[Export ("prependTransform:")]
		void PrependTransform (NSAffineTransform transform);

		[Export ("transformPoint:")]
		CGPoint TransformPoint (CGPoint aPoint);

		[Export ("transformSize:")]
		CGSize TransformSize (CGSize aSize);

		/// <param name="path">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[NoMacCatalyst]
		[Export ("transformBezierPath:")]
		NSBezierPath TransformBezierPath (NSBezierPath path);

		[Export ("set")]
		void Set ();

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[Export ("concat")]
		void Concat ();

		[Export ("transformStruct")]
		CGAffineTransform TransformStruct { get; set; }
	}

	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSXpcConnection' instead.")]
	[NoMacCatalyst]
	[NoiOS, NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSConnection {
		[return: NullAllowed]
		[Static, Export ("connectionWithReceivePort:sendPort:")]
		NSConnection Create ([NullAllowed] NSPort receivePort, [NullAllowed] NSPort sendPort);

		[Export ("runInNewThread")]
		void RunInNewThread ();

		// enableMultipleThreads, multipleThreadsEnabled - no-op in 10.5+ (always enabled)

		[Export ("addRunLoop:")]
		void AddRunLoop (NSRunLoop runLoop);

		[Export ("removeRunLoop:")]
		void RemoveRunLoop (NSRunLoop runLoop);

		[return: NullAllowed]
		[Static, Export ("serviceConnectionWithName:rootObject:usingNameServer:")]
		NSConnection CreateService (string name, NSObject root, NSPortNameServer server);

		[return: NullAllowed]
		[Static, Export ("serviceConnectionWithName:rootObject:")]
		NSConnection CreateService (string name, NSObject root);

		[Export ("registerName:")]
		bool RegisterName ([NullAllowed] string name);

		[Export ("registerName:withNameServer:")]
		bool RegisterName ([NullAllowed] string name, NSPortNameServer server);

		[NullAllowed]
		[Export ("rootObject", ArgumentSemantic.Retain)]
		NSObject RootObject { get; set; }

		[return: NullAllowed]
		[Static, Export ("connectionWithRegisteredName:host:")]
		NSConnection LookupService (string name, [NullAllowed] string hostName);

		[return: NullAllowed]
		[Static, Export ("connectionWithRegisteredName:host:usingNameServer:")]
		NSConnection LookupService (string name, [NullAllowed] string hostName, NSPortNameServer server);

		[Internal, Export ("rootProxy")]
		IntPtr _GetRootProxy ();

		[Internal, Static, Export ("rootProxyForConnectionWithRegisteredName:host:")]
		IntPtr _GetRootProxy (string name, [NullAllowed] string hostName);

		[Internal, Static, Export ("rootProxyForConnectionWithRegisteredName:host:usingNameServer:")]
		IntPtr _GetRootProxy (string name, [NullAllowed] string hostName, NSPortNameServer server);

		[Export ("remoteObjects")]
		NSObject [] RemoteObjects { get; }

		[Export ("localObjects")]
		NSObject [] LocalObjects { get; }

		[NullAllowed]
		[Static, Export ("currentConversation")]
		NSObject CurrentConversation { get; }

		[Static, Export ("allConnections")]
		NSConnection [] AllConnections { get; }

		[Export ("requestTimeout")]
		NSTimeInterval RequestTimeout { get; set; }

		[Export ("replyTimeout")]
		NSTimeInterval ReplyTimeout { get; set; }

		[Export ("independentConversationQueueing")]
		bool IndependentConversationQueueing { get; set; }

		[Export ("addRequestMode:")]
		void AddRequestMode (NSString runLoopMode);

		[Export ("removeRequestMode:")]
		void RemoveRequestMode (NSString runLoopMode);

		[Export ("requestModes")]
		NSString [] RequestModes { get; }

		[Export ("invalidate")]
		void Invalidate ();

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isValid")]
		bool IsValid { get; }

		[Export ("receivePort")]
		NSPort ReceivePort { get; }

		[Export ("sendPort")]
		NSPort SendPort { get; }

		[Export ("dispatchWithComponents:")]
		void Dispatch (NSArray components);

		[Export ("statistics")]
		NSDictionary Statistics { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Wrap ("WeakDelegate")]
		INSConnectionDelegate Delegate { get; set; }
	}

	interface INSConnectionDelegate { }

	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSXpcConnection' instead.")]
	[NoMacCatalyst]
	[NoiOS, NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSConnectionDelegate {
		/// <param name="components">To be added.</param>
		/// <param name="authenticationData">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("authenticateComponents:withData:")]
		bool AuthenticateComponents (NSArray components, NSData authenticationData);

		/// <param name="components">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("authenticationDataForComponents:")]
		NSData GetAuthenticationData (NSArray components);

		/// <param name="parentConnection">To be added.</param>
		/// <param name="newConnection">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("connection:shouldMakeNewConnection:")]
		bool ShouldMakeNewConnection (NSConnection parentConnection, NSConnection newConnection);

		/// <param name="connection">To be added.</param>
		/// <param name="request">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("connection:handleRequest:")]
		bool HandleRequest (NSConnection connection, NSDistantObjectRequest request);

		/// <param name="connection">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("createConversationForConnection:")]
		NSObject CreateConversation (NSConnection connection);

		/// <param name="newConnection">To be added.</param>
		/// <param name="parentConnection">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("makeNewConnection:sender:")]
		bool AllowNewConnection (NSConnection newConnection, NSConnection parentConnection);
	}

	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSXpcConnection' instead.")]
	[NoMacCatalyst]
	[NoiOS, NoTV]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSDistantObjectRequest {
		[Export ("connection")]
		NSConnection Connection { get; }

		[Export ("conversation")]
		NSObject Conversation { get; }

		[Export ("invocation")]
		NSInvocation Invocation { get; }

		[Export ("replyWithException:")]
		void Reply ([NullAllowed] NSException exception);
	}

	[NoMacCatalyst]
	[NoiOS, NoTV]
	[Deprecated (PlatformName.MacOSX, 10, 13)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSPortNameServer {
		[Static, Export ("systemDefaultPortNameServer")]
		NSPortNameServer SystemDefault { get; }

		[return: NullAllowed]
		[Export ("portForName:")]
		NSPort GetPort (string portName);

		[return: NullAllowed]
		[Export ("portForName:host:")]
		NSPort GetPort (string portName, [NullAllowed] string hostName);

		[Export ("registerPort:name:")]
		bool RegisterPort (NSPort port, string portName);

		[Export ("removePortForName:")]
		bool RemovePort (string portName);
	}

	[NoiOS, NoTV]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface NSAppleEventDescriptor : NSSecureCoding, NSCopying {
		[Static]
		[Export ("nullDescriptor")]
		NSAppleEventDescriptor NullDescriptor { get; }

		/*		[Static]
		[Export ("descriptorWithDescriptorType:bytes:length:")]
		NSAppleEventDescriptor DescriptorWithDescriptorTypebyteslength (DescType descriptorType, void bytes, uint byteCount);

		[Static]
		[Export ("descriptorWithDescriptorType:data:")]
		NSAppleEventDescriptor DescriptorWithDescriptorTypedata (DescType descriptorType, NSData data);*/

		[Static]
		[Export ("descriptorWithBoolean:")]
		NSAppleEventDescriptor DescriptorWithBoolean (Boolean boolean);

		[Static]
		[Export ("descriptorWithEnumCode:")]
		NSAppleEventDescriptor DescriptorWithEnumCode (OSType enumerator);

		[Static]
		[Export ("descriptorWithInt32:")]
		NSAppleEventDescriptor DescriptorWithInt32 (int /* int32 */ signedInt);

		[Static]
		[Export ("descriptorWithTypeCode:")]
		NSAppleEventDescriptor DescriptorWithTypeCode (OSType typeCode);

		[Static]
		[Export ("descriptorWithString:")]
		NSAppleEventDescriptor DescriptorWithString (string str);

		/*[Static]
		[Export ("appleEventWithEventClass:eventID:targetDescriptor:returnID:transactionID:")]
		NSAppleEventDescriptor AppleEventWithEventClasseventIDtargetDescriptorreturnIDtransactionID (AEEventClass eventClass, AEEventID eventID, NSAppleEventDescriptor targetDescriptor, AEReturnID returnID, AETransactionID transactionID);*/

		[Static]
		[Export ("listDescriptor")]
		NSAppleEventDescriptor ListDescriptor { get; }

		[Static]
		[Export ("recordDescriptor")]
		NSAppleEventDescriptor RecordDescriptor { get; }

		/*[Export ("initWithAEDescNoCopy:")]
		NSObject InitWithAEDescNoCopy (const AEDesc aeDesc);

		[Export ("initWithDescriptorType:bytes:length:")]
		NSObject InitWithDescriptorTypebyteslength (DescType descriptorType, void bytes, uint byteCount);

		[Export ("initWithDescriptorType:data:")]
		NSObject InitWithDescriptorTypedata (DescType descriptorType, NSData data);

		[Export ("initWithEventClass:eventID:targetDescriptor:returnID:transactionID:")]
		NSObject InitWithEventClasseventIDtargetDescriptorreturnIDtransactionID (AEEventClass eventClass, AEEventID eventID, NSAppleEventDescriptor targetDescriptor, AEReturnID returnID, AETransactionID transactionID);*/

		[Internal]
		[Sealed]
		[Export ("initListDescriptor")]
		IntPtr _InitListDescriptor ();

		[Internal]
		[Sealed]
		[Export ("initRecordDescriptor")]
		IntPtr _InitRecordDescriptor ();

		/*[Export ("aeDesc")]
		const AEDesc AeDesc ();

		[Export ("descriptorType")]
		DescType DescriptorType ();*/

		[Export ("data")]
		NSData Data { get; }

		[Export ("booleanValue")]
		Boolean BooleanValue { get; }

		[Export ("enumCodeValue")]
		OSType EnumCodeValue ();

		[Export ("int32Value")]
		Int32 Int32Value { get; }

		[Export ("typeCodeValue")]
		OSType TypeCodeValue { get; }

		[NullAllowed]
		[Export ("stringValue")]
		string StringValue { get; }

		[Export ("eventClass")]
		AEEventClass EventClass { get; }

		[Export ("eventID")]
		AEEventID EventID { get; }

		/*[Export ("returnID")]
		AEReturnID ReturnID ();

		[Export ("transactionID")]
		AETransactionID TransactionID ();*/

		[Export ("setParamDescriptor:forKeyword:")]
#if XAMCORE_5_0
		void SetParamDescriptor (NSAppleEventDescriptor descriptor, AEKeyword keyword);
#else
		[Obsolete ("Use 'SetParamDescriptor' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		void SetParamDescriptorforKeyword (NSAppleEventDescriptor descriptor, AEKeyword keyword);

		[Wrap ("SetParamDescriptorforKeyword (descriptor, keyword)")]
		void SetParamDescriptor (NSAppleEventDescriptor descriptor, AEKeyword keyword);
#endif

		[return: NullAllowed]
		[Export ("paramDescriptorForKeyword:")]
		NSAppleEventDescriptor ParamDescriptorForKeyword (AEKeyword keyword);

		[Export ("removeParamDescriptorWithKeyword:")]
		void RemoveParamDescriptorWithKeyword (AEKeyword keyword);

		[Export ("setAttributeDescriptor:forKeyword:")]
#if XAMCORE_5_0
		void SetAttributeDescriptor (NSAppleEventDescriptor descriptor, AEKeyword keyword);
#else
		[Obsolete ("Use 'SetAttributeDescriptor' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		void SetAttributeDescriptorforKeyword (NSAppleEventDescriptor descriptor, AEKeyword keyword);

		[Wrap ("SetAttributeDescriptorforKeyword (descriptor, keyword)")]
		void SetAttributeDescriptor (NSAppleEventDescriptor descriptor, AEKeyword keyword);
#endif

		[return: NullAllowed]
		[Export ("attributeDescriptorForKeyword:")]
		NSAppleEventDescriptor AttributeDescriptorForKeyword (AEKeyword keyword);

		[Export ("numberOfItems")]
		nint NumberOfItems { get; }

		/// <param name="descriptor">To be added.</param>
		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("insertDescriptor:atIndex:")]
#if XAMCORE_5_0
		void InsertDescriptor (NSAppleEventDescriptor descriptor, nint index);
#else
		[Obsolete ("Use 'InsertDescriptor' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		void InsertDescriptoratIndex (NSAppleEventDescriptor descriptor, nint index);

		[Wrap ("InsertDescriptoratIndex (descriptor, index)")]
		void InsertDescriptor (NSAppleEventDescriptor descriptor, nint index);
#endif

		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[return: NullAllowed]
		[Export ("descriptorAtIndex:")]
		NSAppleEventDescriptor DescriptorAtIndex (nint index);

		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("removeDescriptorAtIndex:")]
		void RemoveDescriptorAtIndex (nint index);

		[Export ("setDescriptor:forKeyword:")]
#if XAMCORE_5_0
		void SetDescriptor (NSAppleEventDescriptor descriptor, AEKeyword keyword);
#else
		[Obsolete ("Use 'SetDescriptor' instead.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		void SetDescriptorforKeyword (NSAppleEventDescriptor descriptor, AEKeyword keyword);

		[Wrap ("SetDescriptorforKeyword (descriptor, keyword)")]
		void SetDescriptor (NSAppleEventDescriptor descriptor, AEKeyword keyword);
#endif

		[return: NullAllowed]
		[Export ("descriptorForKeyword:")]
		NSAppleEventDescriptor DescriptorForKeyword (AEKeyword keyword);

		[Export ("removeDescriptorWithKeyword:")]
		void RemoveDescriptorWithKeyword (AEKeyword keyword);

		/// <param name="index">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[Export ("keywordForDescriptorAtIndex:")]
		AEKeyword KeywordForDescriptorAtIndex (nint index);

		/*[Export ("coerceToDescriptorType:")]
		NSAppleEventDescriptor CoerceToDescriptorType (DescType descriptorType);*/

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("currentProcessDescriptor")]
		NSAppleEventDescriptor CurrentProcessDescriptor { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("descriptorWithDouble:")]
		NSAppleEventDescriptor FromDouble (double doubleValue);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("descriptorWithDate:")]
		NSAppleEventDescriptor FromDate (NSDate date);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("descriptorWithFileURL:")]
		NSAppleEventDescriptor FromFileURL (NSUrl fileURL);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("descriptorWithProcessIdentifier:")]
		NSAppleEventDescriptor FromProcessIdentifier (int processIdentifier);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("descriptorWithBundleIdentifier:")]
		NSAppleEventDescriptor FromBundleIdentifier (string bundleIdentifier);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("descriptorWithApplicationURL:")]
		NSAppleEventDescriptor FromApplicationURL (NSUrl applicationURL);

		[MacCatalyst (13, 1)]
		[Export ("doubleValue")]
		double DoubleValue { get; }

		[NoMacCatalyst]
		[Export ("sendEventWithOptions:timeout:error:")]
		[return: NullAllowed]
		NSAppleEventDescriptor SendEvent (NSAppleEventSendOptions sendOptions, double timeoutInSeconds, [NullAllowed] out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("isRecordDescriptor")]
		bool IsRecordDescriptor { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("dateValue", ArgumentSemantic.Copy)]
		NSDate DateValue { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("fileURLValue", ArgumentSemantic.Copy)]
		NSUrl FileURLValue { get; }
	}

	[NoiOS, NoTV]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface NSAppleEventManager {
		[Static]
		[Export ("sharedAppleEventManager")]
		NSAppleEventManager SharedAppleEventManager { get; }

		[Export ("setEventHandler:andSelector:forEventClass:andEventID:")]
		void SetEventHandler (NSObject handler, Selector handleEventSelector, AEEventClass eventClass, AEEventID eventID);

		[Export ("removeEventHandlerForEventClass:andEventID:")]
		void RemoveEventHandler (AEEventClass eventClass, AEEventID eventID);

		[NullAllowed]
		[Export ("currentAppleEvent")]
		NSAppleEventDescriptor CurrentAppleEvent { get; }

		[NullAllowed]
		[Export ("currentReplyAppleEvent")]
		NSAppleEventDescriptor CurrentReplyAppleEvent { get; }

		[Export ("suspendCurrentAppleEvent")]
		NSAppleEventManagerSuspensionID SuspendCurrentAppleEvent ();

		[Export ("appleEventForSuspensionID:")]
		NSAppleEventDescriptor AppleEventForSuspensionID (NSAppleEventManagerSuspensionID suspensionID);

		[Export ("replyAppleEventForSuspensionID:")]
		NSAppleEventDescriptor ReplyAppleEventForSuspensionID (NSAppleEventManagerSuspensionID suspensionID);

		[Export ("setCurrentAppleEventAndReplyEventWithSuspensionID:")]
		void SetCurrentAppleEventAndReplyEventWithSuspensionID (NSAppleEventManagerSuspensionID suspensionID);

		[Export ("resumeWithSuspensionID:")]
		void ResumeWithSuspensionID (NSAppleEventManagerSuspensionID suspensionID);

		[Notification]
		[Field ("NSAppleEventManagerWillProcessFirstEventNotification")]
		NSString WillProcessFirstEventNotification { get; }
	}

	[NoiOS, NoTV]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSTask {
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Export ("launch")]
		void Launch ();

		[NoMacCatalyst]
		[Export ("launchAndReturnError:")]
		bool Launch ([NullAllowed] out NSError error);

		[Export ("interrupt")]
		void Interrupt ();

		[Export ("terminate")]
		void Terminate ();

		[Export ("suspend")]
		bool Suspend ();

		[Export ("resume")]
		bool Resume ();

		[Export ("waitUntilExit")]
		void WaitUntilExit ();

		[Static]
		[Deprecated (PlatformName.MacOSX, 10, 15)]
#if XAMCORE_5_0
		[NoMacCatalyst]
#else
#if MACCATALYST
		[Obsolete ("Do not use; this method is not available on Mac Catalyst.")]
		[EditorBrowsable (EditorBrowsableState.Never)]
#endif // MACCATALYST
#endif // XAMCORE_5_0
		[Export ("launchedTaskWithLaunchPath:arguments:")]
		NSTask LaunchFromPath (string path, string [] arguments);

		[Static]
		[NoMacCatalyst]
		[Export ("launchedTaskWithExecutableURL:arguments:error:terminationHandler:")]
		[return: NullAllowed]
		NSTask LaunchFromUrl (NSUrl url, string [] arguments, [NullAllowed] out NSError error, [NullAllowed] Action<NSTask> terminationHandler);

		//Detected properties
		[NullAllowed]
		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[NoMacCatalyst]
		[Export ("launchPath")]
		string LaunchPath { get; set; }

		[NullAllowed]
		[NoMacCatalyst]
		[Export ("executableURL")]
		NSUrl ExecutableUrl { get; set; }

		[NullAllowed]
		[Export ("arguments")]
		string [] Arguments { get; set; }

		[NullAllowed]
		[Export ("environment", ArgumentSemantic.Copy)]
		NSDictionary Environment { get; set; }

		[NoMacCatalyst]
		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Export ("currentDirectoryPath")]
		string CurrentDirectoryPath { get; set; }

		[NullAllowed]
		[NoMacCatalyst]
		[Export ("currentDirectoryURL")]
		NSUrl CurrentDirectoryUrl { get; set; }

		[NullAllowed]
		[Mac (14, 4), MacCatalyst (17, 4)]
		[Export ("launchRequirementData", ArgumentSemantic.Copy)]
		NSData LaunchRequirementData { get; set; }

		[NullAllowed]
		[Export ("standardInput", ArgumentSemantic.Retain)]
		NSObject StandardInput { get; set; }

		[NullAllowed]
		[Export ("standardOutput", ArgumentSemantic.Retain)]
		NSObject StandardOutput { get; set; }

		[NullAllowed]
		[Export ("standardError", ArgumentSemantic.Retain)]
		NSObject StandardError { get; set; }

		[Export ("qualityOfService")]
		NSQualityOfService QualityOfService { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("isRunning")]
		bool IsRunning { get; }

		[Export ("processIdentifier")]
		int ProcessIdentifier { get; } /* pid_t = int */

		[Export ("terminationStatus")]
		int TerminationStatus { get; } /* int, not NSInteger */

		[NullAllowed]
		[NoMacCatalyst]
		[Export ("terminationHandler")]
		Action<NSTask> TerminationHandler { get; set; }

		[NoMacCatalyst]
		[Export ("terminationReason")]
		NSTaskTerminationReason TerminationReason { get; }

		[Field ("NSTaskDidTerminateNotification")]
		[Notification]
		NSString DidTerminateNotification { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	[Advice ("'NSUserNotification' usages should be replaced with 'UserNotifications' framework.")]
	interface NSUserNotification : NSCoding, NSCopying {
		[NullAllowed]
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }

		[NullAllowed]
		[Export ("subtitle", ArgumentSemantic.Copy)]
		string Subtitle { get; set; }

		[NullAllowed]
		[Export ("informativeText", ArgumentSemantic.Copy)]
		string InformativeText { get; set; }

		[Export ("actionButtonTitle", ArgumentSemantic.Copy)]
		string ActionButtonTitle { get; set; }

		[NullAllowed]
		[Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary UserInfo { get; set; }

		[NullAllowed]
		[Export ("deliveryDate", ArgumentSemantic.Copy)]
		NSDate DeliveryDate { get; set; }

		[NullAllowed]
		[Export ("deliveryTimeZone", ArgumentSemantic.Copy)]
		NSTimeZone DeliveryTimeZone { get; set; }

		[NullAllowed]
		[Export ("deliveryRepeatInterval", ArgumentSemantic.Copy)]
		NSDateComponents DeliveryRepeatInterval { get; set; }

		[NullAllowed]
		[Export ("actualDeliveryDate")]
		NSDate ActualDeliveryDate { get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("presented")]
		bool Presented { [Bind ("isPresented")] get; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("remote")]
		bool Remote { [Bind ("isRemote")] get; }

		[NullAllowed]
		[Export ("soundName", ArgumentSemantic.Copy)]
		string SoundName { get; set; }

		[Export ("hasActionButton")]
		bool HasActionButton { get; set; }

		[Export ("activationType")]
		NSUserNotificationActivationType ActivationType { get; }

		[Export ("otherButtonTitle", ArgumentSemantic.Copy)]
		string OtherButtonTitle { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Field ("NSUserNotificationDefaultSoundName")]
		NSString NSUserNotificationDefaultSoundName { get; }

		[NullAllowed, Export ("identifier")]
		string Identifier { get; set; }

		[NullAllowed, Export ("contentImage", ArgumentSemantic.Copy)]
		NSImage ContentImage { get; set; }

		[Export ("hasReplyButton")]
		bool HasReplyButton { get; set; }

		[NullAllowed, Export ("responsePlaceholder")]
		string ResponsePlaceholder { get; set; }

		[NullAllowed, Export ("response", ArgumentSemantic.Copy)]
		NSAttributedString Response { get; }

		[NullAllowed, Export ("additionalActions", ArgumentSemantic.Copy)]
		NSUserNotificationAction [] AdditionalActions { get; set; }

		[NullAllowed, Export ("additionalActivationAction", ArgumentSemantic.Copy)]
		NSUserNotificationAction AdditionalActivationAction { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[Advice ("'NSUserNotification' usages should be replaced with 'UserNotifications' framework.")]
	interface NSUserNotificationAction : NSCopying {
		[Static]
		[Export ("actionWithIdentifier:title:")]
		NSUserNotificationAction GetAction ([NullAllowed] string identifier, [NullAllowed] string title);

		[NullAllowed, Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("title")]
		string Title { get; }
	}

	[NoiOS, NoTV, NoMacCatalyst]
	[BaseType (typeof (NSObject),
			   Delegates = new string [] { "WeakDelegate" },
	Events = new Type [] { typeof (NSUserNotificationCenterDelegate) })]
	[DisableDefaultCtor] // crash with: NSUserNotificationCenter designitated initializer is _centerForBundleIdentifier
	[Advice ("'NSUserNotification' usages should be replaced with 'UserNotifications' framework.")]
	interface NSUserNotificationCenter {
		[Export ("defaultUserNotificationCenter")]
		[Static]
		NSUserNotificationCenter DefaultUserNotificationCenter { get; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		INSUserNotificationCenterDelegate Delegate { get; set; }

		[Export ("scheduledNotifications", ArgumentSemantic.Copy)]
		NSUserNotification [] ScheduledNotifications { get; set; }

		[Export ("scheduleNotification:")]
		[PostGet ("ScheduledNotifications")]
		void ScheduleNotification (NSUserNotification notification);

		[Export ("removeScheduledNotification:")]
		[PostGet ("ScheduledNotifications")]
		void RemoveScheduledNotification (NSUserNotification notification);

		[Export ("deliveredNotifications")]
		NSUserNotification [] DeliveredNotifications { get; }

		[Export ("deliverNotification:")]
		[PostGet ("DeliveredNotifications")]
		void DeliverNotification (NSUserNotification notification);

		[Export ("removeDeliveredNotification:")]
		[PostGet ("DeliveredNotifications")]
		void RemoveDeliveredNotification (NSUserNotification notification);

		[Export ("removeAllDeliveredNotifications")]
		[PostGet ("DeliveredNotifications")]
		void RemoveAllDeliveredNotifications ();
	}

	interface INSUserNotificationCenterDelegate { }

	[NoiOS, NoTV, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'UserNotifications.*' API instead.")]
	interface NSUserNotificationCenterDelegate {
		/// <param name="center">To be added.</param>
		/// <param name="notification">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("userNotificationCenter:didDeliverNotification:"), EventArgs ("UNCDidDeliverNotification", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void DidDeliverNotification (NSUserNotificationCenter center, NSUserNotification notification);

		/// <param name="center">To be added.</param>
		/// <param name="notification">To be added.</param>
		/// <summary>To be added.</summary>
		/// <remarks>To be added.</remarks>
		[Export ("userNotificationCenter:didActivateNotification:"), EventArgs ("UNCDidActivateNotification", XmlDocs = """
			<summary>To be added.</summary>
			<remarks>To be added.</remarks>
			""")]
		void DidActivateNotification (NSUserNotificationCenter center, NSUserNotification notification);

		/// <param name="center">To be added.</param>
		/// <param name="notification">To be added.</param>
		/// <summary>To be added.</summary>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		[EventArgs ("", XmlDocs = """
			<summary>To be added.</summary>
			<value>To be added.</value>
			<remarks>To be added.</remarks>
			""")]
		[Export ("userNotificationCenter:shouldPresentNotification:"), DelegateName ("UNCShouldPresentNotification"), DefaultValue (false)]
		bool ShouldPresentNotification (NSUserNotificationCenter center, NSUserNotification notification);
	}

	[NoiOS, NoTV]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSAppleScript : NSCopying {

		// @required - (instancetype)initWithContentsOfURL:(NSURL *)url error:(NSDictionary **)errorInfo;
		[DesignatedInitializer]
		[Export ("initWithContentsOfURL:error:")]
		NativeHandle Constructor (NSUrl url, out NSDictionary errorInfo);

		// @required - (instancetype)initWithSource:(NSString *)source;
		[DesignatedInitializer]
		[Export ("initWithSource:")]
		NativeHandle Constructor (string source);

		// @property (readonly, copy) NSString * source;
		[NullAllowed]
		[Export ("source")]
		string Source { get; }

		// @property (readonly, getter = isCompiled) BOOL compiled;
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("compiled")]
		bool Compiled { [Bind ("isCompiled")] get; }

		// @required - (BOOL)compileAndReturnError:(NSDictionary **)errorInfo;
		[Export ("compileAndReturnError:")]
		bool CompileAndReturnError (out NSDictionary errorInfo);

		// @required - (NSAppleEventDescriptor *)executeAndReturnError:(NSDictionary **)errorInfo;
		[Export ("executeAndReturnError:")]
		NSAppleEventDescriptor ExecuteAndReturnError (out NSDictionary errorInfo);

		// @required - (NSAppleEventDescriptor *)executeAppleEvent:(NSAppleEventDescriptor *)event error:(NSDictionary **)errorInfo;
		[Export ("executeAppleEvent:error:")]
		NSAppleEventDescriptor ExecuteAppleEvent (NSAppleEventDescriptor eventDescriptor, out NSDictionary errorInfo);

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NullAllowed]
		[Export ("richTextSource", ArgumentSemantic.Retain)]
		NSAttributedString RichTextSource { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter), Name = "NSISO8601DateFormatter")]
	[DesignatedDefaultCtor]
	interface NSIso8601DateFormatter : NSSecureCoding {

		[Export ("timeZone", ArgumentSemantic.Copy)]
		NSTimeZone TimeZone { get; set; }

		[Export ("formatOptions", ArgumentSemantic.Assign)]
		NSIso8601DateFormatOptions FormatOptions { get; set; }

		[Export ("stringFromDate:")]
		string ToString (NSDate date);

		[Export ("dateFromString:")]
		[return: NullAllowed]
		NSDate ToDate (string @string);

		[Static]
		[Export ("stringFromDate:timeZone:formatOptions:")]
		string Format (NSDate date, NSTimeZone timeZone, NSIso8601DateFormatOptions formatOptions);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NSURLSessionTaskTransactionMetrics")]
	[DisableDefaultCtor]
	interface NSUrlSessionTaskTransactionMetrics {

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "This type is not meant to be user created.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "This type is not meant to be user created.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "This type is not meant to be user created.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "This type is not meant to be user created.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("request", ArgumentSemantic.Copy)]
		NSUrlRequest Request { get; }

		[NullAllowed, Export ("response", ArgumentSemantic.Copy)]
		NSUrlResponse Response { get; }

		[NullAllowed, Export ("fetchStartDate", ArgumentSemantic.Copy)]
		NSDate FetchStartDate { get; }

		[NullAllowed, Export ("domainLookupStartDate", ArgumentSemantic.Copy)]
		NSDate DomainLookupStartDate { get; }

		[NullAllowed, Export ("domainLookupEndDate", ArgumentSemantic.Copy)]
		NSDate DomainLookupEndDate { get; }

		[NullAllowed, Export ("connectStartDate", ArgumentSemantic.Copy)]
		NSDate ConnectStartDate { get; }

		[NullAllowed, Export ("secureConnectionStartDate", ArgumentSemantic.Copy)]
		NSDate SecureConnectionStartDate { get; }

		[NullAllowed, Export ("secureConnectionEndDate", ArgumentSemantic.Copy)]
		NSDate SecureConnectionEndDate { get; }

		[NullAllowed, Export ("connectEndDate", ArgumentSemantic.Copy)]
		NSDate ConnectEndDate { get; }

		[NullAllowed, Export ("requestStartDate", ArgumentSemantic.Copy)]
		NSDate RequestStartDate { get; }

		[NullAllowed, Export ("requestEndDate", ArgumentSemantic.Copy)]
		NSDate RequestEndDate { get; }

		[NullAllowed, Export ("responseStartDate", ArgumentSemantic.Copy)]
		NSDate ResponseStartDate { get; }

		[NullAllowed, Export ("responseEndDate", ArgumentSemantic.Copy)]
		NSDate ResponseEndDate { get; }

		[NullAllowed, Export ("networkProtocolName")]
		string NetworkProtocolName { get; }

		/// <summary>
		///           <see langword="true" /> if a proxy was used to retrieve the resource.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("proxyConnection")]
		bool ProxyConnection { [Bind ("isProxyConnection")] get; }

		/// <summary>
		///           <see langword="true" /> if the resource was retrieved via a persistent connection.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[Export ("reusedConnection")]
		bool ReusedConnection { [Bind ("isReusedConnection")] get; }

		[Export ("resourceFetchType", ArgumentSemantic.Assign)]
		NSUrlSessionTaskMetricsResourceFetchType ResourceFetchType { get; }

		[MacCatalyst (13, 1)]
		[Export ("countOfRequestHeaderBytesSent")]
		long CountOfRequestHeaderBytesSent { get; }

		[MacCatalyst (13, 1)]
		[Export ("countOfRequestBodyBytesSent")]
		long CountOfRequestBodyBytesSent { get; }

		[MacCatalyst (13, 1)]
		[Export ("countOfRequestBodyBytesBeforeEncoding")]
		long CountOfRequestBodyBytesBeforeEncoding { get; }

		[MacCatalyst (13, 1)]
		[Export ("countOfResponseHeaderBytesReceived")]
		long CountOfResponseHeaderBytesReceived { get; }

		[MacCatalyst (13, 1)]
		[Export ("countOfResponseBodyBytesReceived")]
		long CountOfResponseBodyBytesReceived { get; }

		[MacCatalyst (13, 1)]
		[Export ("countOfResponseBodyBytesAfterDecoding")]
		long CountOfResponseBodyBytesAfterDecoding { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("localAddress")]
		string LocalAddress { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("localPort", ArgumentSemantic.Copy)]
		// 0-1023
		[BindAs (typeof (ushort?))]
		NSNumber LocalPort { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("remoteAddress")]
		string RemoteAddress { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("remotePort", ArgumentSemantic.Copy)]
		// 0-1023
		[BindAs (typeof (ushort?))]
		NSNumber RemotePort { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("negotiatedTLSProtocolVersion", ArgumentSemantic.Copy)]
		// <quote>It is a 2-byte sequence in host byte order.</quote> but it refers to (nicer) `tls_protocol_version_t`
		[BindAs (typeof (SslProtocol?))]
		NSNumber NegotiatedTlsProtocolVersion { get; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("negotiatedTLSCipherSuite", ArgumentSemantic.Copy)]
		// <quote>It is a 2-byte sequence in host byte order.</quote> but it refers to (nicer) `tls_ciphersuite_t`
		[BindAs (typeof (TlsCipherSuite?))]
		NSNumber NegotiatedTlsCipherSuite { get; }

		[MacCatalyst (13, 1)]
		[Export ("cellular")]
		bool Cellular { [Bind ("isCellular")] get; }

		[MacCatalyst (13, 1)]
		[Export ("expensive")]
		bool Expensive { [Bind ("isExpensive")] get; }

		[MacCatalyst (13, 1)]
		[Export ("constrained")]
		bool Constrained { [Bind ("isConstrained")] get; }

		[MacCatalyst (13, 1)]
		[Export ("multipath")]
		bool Multipath { [Bind ("isMultipath")] get; }

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("domainResolutionProtocol")]
		NSUrlSessionTaskMetricsDomainResolutionProtocol DomainResolutionProtocol { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NSURLSessionTaskMetrics")]
	[DisableDefaultCtor]
	interface NSUrlSessionTaskMetrics {

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "This type is not meant to be user created.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "This type is not meant to be user created.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "This type is not meant to be user created.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "This type is not meant to be user created.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("transactionMetrics", ArgumentSemantic.Copy)]
		NSUrlSessionTaskTransactionMetrics [] TransactionMetrics { get; }

		[Export ("taskInterval", ArgumentSemantic.Copy)]
		NSDateInterval TaskInterval { get; }

		[Export ("redirectCount")]
		nuint RedirectCount { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitAcceleration : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("metersPerSecondSquared", ArgumentSemantic.Copy)]
		NSUnitAcceleration MetersPerSecondSquared { get; }

		[Static]
		[Export ("gravity", ArgumentSemantic.Copy)]
		NSUnitAcceleration Gravity { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitAngle : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("degrees", ArgumentSemantic.Copy)]
		NSUnitAngle Degrees { get; }

		[Static]
		[Export ("arcMinutes", ArgumentSemantic.Copy)]
		NSUnitAngle ArcMinutes { get; }

		[Static]
		[Export ("arcSeconds", ArgumentSemantic.Copy)]
		NSUnitAngle ArcSeconds { get; }

		[Static]
		[Export ("radians", ArgumentSemantic.Copy)]
		NSUnitAngle Radians { get; }

		[Static]
		[Export ("gradians", ArgumentSemantic.Copy)]
		NSUnitAngle Gradians { get; }

		[Static]
		[Export ("revolutions", ArgumentSemantic.Copy)]
		NSUnitAngle Revolutions { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitArea : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("squareMegameters", ArgumentSemantic.Copy)]
		NSUnitArea SquareMegameters { get; }

		[Static]
		[Export ("squareKilometers", ArgumentSemantic.Copy)]
		NSUnitArea SquareKilometers { get; }

		[Static]
		[Export ("squareMeters", ArgumentSemantic.Copy)]
		NSUnitArea SquareMeters { get; }

		[Static]
		[Export ("squareCentimeters", ArgumentSemantic.Copy)]
		NSUnitArea SquareCentimeters { get; }

		[Static]
		[Export ("squareMillimeters", ArgumentSemantic.Copy)]
		NSUnitArea SquareMillimeters { get; }

		[Static]
		[Export ("squareMicrometers", ArgumentSemantic.Copy)]
		NSUnitArea SquareMicrometers { get; }

		[Static]
		[Export ("squareNanometers", ArgumentSemantic.Copy)]
		NSUnitArea SquareNanometers { get; }

		[Static]
		[Export ("squareInches", ArgumentSemantic.Copy)]
		NSUnitArea SquareInches { get; }

		[Static]
		[Export ("squareFeet", ArgumentSemantic.Copy)]
		NSUnitArea SquareFeet { get; }

		[Static]
		[Export ("squareYards", ArgumentSemantic.Copy)]
		NSUnitArea SquareYards { get; }

		[Static]
		[Export ("squareMiles", ArgumentSemantic.Copy)]
		NSUnitArea SquareMiles { get; }

		[Static]
		[Export ("acres", ArgumentSemantic.Copy)]
		NSUnitArea Acres { get; }

		[Static]
		[Export ("ares", ArgumentSemantic.Copy)]
		NSUnitArea Ares { get; }

		[Static]
		[Export ("hectares", ArgumentSemantic.Copy)]
		NSUnitArea Hectares { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitConcentrationMass : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("gramsPerLiter", ArgumentSemantic.Copy)]
		NSUnitConcentrationMass GramsPerLiter { get; }

		[Static]
		[Export ("milligramsPerDeciliter", ArgumentSemantic.Copy)]
		NSUnitConcentrationMass MilligramsPerDeciliter { get; }

		[Static]
		[Export ("millimolesPerLiterWithGramsPerMole:")]
		NSUnitConcentrationMass GetMillimolesPerLiter (double gramsPerMole);

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitDispersion : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("partsPerMillion", ArgumentSemantic.Copy)]
		NSUnitDispersion PartsPerMillion { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitDuration : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("seconds", ArgumentSemantic.Copy)]
		NSUnitDuration Seconds { get; }

		[Static]
		[Export ("minutes", ArgumentSemantic.Copy)]
		NSUnitDuration Minutes { get; }

		[Static]
		[Export ("hours", ArgumentSemantic.Copy)]
		NSUnitDuration Hours { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("milliseconds", ArgumentSemantic.Copy)]
		NSUnitDuration Milliseconds { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("microseconds", ArgumentSemantic.Copy)]
		NSUnitDuration Microseconds { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("nanoseconds", ArgumentSemantic.Copy)]
		NSUnitDuration Nanoseconds { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("picoseconds", ArgumentSemantic.Copy)]
		NSUnitDuration Picoseconds { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitElectricCharge : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("coulombs", ArgumentSemantic.Copy)]
		NSUnitElectricCharge Coulombs { get; }

		[Static]
		[Export ("megaampereHours", ArgumentSemantic.Copy)]
		NSUnitElectricCharge MegaampereHours { get; }

		[Static]
		[Export ("kiloampereHours", ArgumentSemantic.Copy)]
		NSUnitElectricCharge KiloampereHours { get; }

		[Static]
		[Export ("ampereHours", ArgumentSemantic.Copy)]
		NSUnitElectricCharge AmpereHours { get; }

		[Static]
		[Export ("milliampereHours", ArgumentSemantic.Copy)]
		NSUnitElectricCharge MilliampereHours { get; }

		[Static]
		[Export ("microampereHours", ArgumentSemantic.Copy)]
		NSUnitElectricCharge MicroampereHours { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitElectricCurrent : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("megaamperes", ArgumentSemantic.Copy)]
		NSUnitElectricCurrent Megaamperes { get; }

		[Static]
		[Export ("kiloamperes", ArgumentSemantic.Copy)]
		NSUnitElectricCurrent Kiloamperes { get; }

		[Static]
		[Export ("amperes", ArgumentSemantic.Copy)]
		NSUnitElectricCurrent Amperes { get; }

		[Static]
		[Export ("milliamperes", ArgumentSemantic.Copy)]
		NSUnitElectricCurrent Milliamperes { get; }

		[Static]
		[Export ("microamperes", ArgumentSemantic.Copy)]
		NSUnitElectricCurrent Microamperes { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitElectricPotentialDifference : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("megavolts", ArgumentSemantic.Copy)]
		NSUnitElectricPotentialDifference Megavolts { get; }

		[Static]
		[Export ("kilovolts", ArgumentSemantic.Copy)]
		NSUnitElectricPotentialDifference Kilovolts { get; }

		[Static]
		[Export ("volts", ArgumentSemantic.Copy)]
		NSUnitElectricPotentialDifference Volts { get; }

		[Static]
		[Export ("millivolts", ArgumentSemantic.Copy)]
		NSUnitElectricPotentialDifference Millivolts { get; }

		[Static]
		[Export ("microvolts", ArgumentSemantic.Copy)]
		NSUnitElectricPotentialDifference Microvolts { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitElectricResistance : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("megaohms", ArgumentSemantic.Copy)]
		NSUnitElectricResistance Megaohms { get; }

		[Static]
		[Export ("kiloohms", ArgumentSemantic.Copy)]
		NSUnitElectricResistance Kiloohms { get; }

		[Static]
		[Export ("ohms", ArgumentSemantic.Copy)]
		NSUnitElectricResistance Ohms { get; }

		[Static]
		[Export ("milliohms", ArgumentSemantic.Copy)]
		NSUnitElectricResistance Milliohms { get; }

		[Static]
		[Export ("microohms", ArgumentSemantic.Copy)]
		NSUnitElectricResistance Microohms { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitEnergy : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("kilojoules", ArgumentSemantic.Copy)]
		NSUnitEnergy Kilojoules { get; }

		[Static]
		[Export ("joules", ArgumentSemantic.Copy)]
		NSUnitEnergy Joules { get; }

		[Static]
		[Export ("kilocalories", ArgumentSemantic.Copy)]
		NSUnitEnergy Kilocalories { get; }

		[Static]
		[Export ("calories", ArgumentSemantic.Copy)]
		NSUnitEnergy Calories { get; }

		[Static]
		[Export ("kilowattHours", ArgumentSemantic.Copy)]
		NSUnitEnergy KilowattHours { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitFrequency : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("terahertz", ArgumentSemantic.Copy)]
		NSUnitFrequency Terahertz { get; }

		[Static]
		[Export ("gigahertz", ArgumentSemantic.Copy)]
		NSUnitFrequency Gigahertz { get; }

		[Static]
		[Export ("megahertz", ArgumentSemantic.Copy)]
		NSUnitFrequency Megahertz { get; }

		[Static]
		[Export ("kilohertz", ArgumentSemantic.Copy)]
		NSUnitFrequency Kilohertz { get; }

		[Static]
		[Export ("hertz", ArgumentSemantic.Copy)]
		NSUnitFrequency Hertz { get; }

		[Static]
		[Export ("millihertz", ArgumentSemantic.Copy)]
		NSUnitFrequency Millihertz { get; }

		[Static]
		[Export ("microhertz", ArgumentSemantic.Copy)]
		NSUnitFrequency Microhertz { get; }

		[Static]
		[Export ("nanohertz", ArgumentSemantic.Copy)]
		NSUnitFrequency Nanohertz { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("framesPerSecond", ArgumentSemantic.Copy)]
		NSUnitFrequency FramesPerSecond { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitFuelEfficiency : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("litersPer100Kilometers", ArgumentSemantic.Copy)]
		NSUnitFuelEfficiency LitersPer100Kilometers { get; }

		[Static]
		[Export ("milesPerImperialGallon", ArgumentSemantic.Copy)]
		NSUnitFuelEfficiency MilesPerImperialGallon { get; }

		[Static]
		[Export ("milesPerGallon", ArgumentSemantic.Copy)]
		NSUnitFuelEfficiency MilesPerGallon { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitLength : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("megameters", ArgumentSemantic.Copy)]
		NSUnitLength Megameters { get; }

		[Static]
		[Export ("kilometers", ArgumentSemantic.Copy)]
		NSUnitLength Kilometers { get; }

		[Static]
		[Export ("hectometers", ArgumentSemantic.Copy)]
		NSUnitLength Hectometers { get; }

		[Static]
		[Export ("decameters", ArgumentSemantic.Copy)]
		NSUnitLength Decameters { get; }

		[Static]
		[Export ("meters", ArgumentSemantic.Copy)]
		NSUnitLength Meters { get; }

		[Static]
		[Export ("decimeters", ArgumentSemantic.Copy)]
		NSUnitLength Decimeters { get; }

		[Static]
		[Export ("centimeters", ArgumentSemantic.Copy)]
		NSUnitLength Centimeters { get; }

		[Static]
		[Export ("millimeters", ArgumentSemantic.Copy)]
		NSUnitLength Millimeters { get; }

		[Static]
		[Export ("micrometers", ArgumentSemantic.Copy)]
		NSUnitLength Micrometers { get; }

		[Static]
		[Export ("nanometers", ArgumentSemantic.Copy)]
		NSUnitLength Nanometers { get; }

		[Static]
		[Export ("picometers", ArgumentSemantic.Copy)]
		NSUnitLength Picometers { get; }

		[Static]
		[Export ("inches", ArgumentSemantic.Copy)]
		NSUnitLength Inches { get; }

		[Static]
		[Export ("feet", ArgumentSemantic.Copy)]
		NSUnitLength Feet { get; }

		[Static]
		[Export ("yards", ArgumentSemantic.Copy)]
		NSUnitLength Yards { get; }

		[Static]
		[Export ("miles", ArgumentSemantic.Copy)]
		NSUnitLength Miles { get; }

		[Static]
		[Export ("scandinavianMiles", ArgumentSemantic.Copy)]
		NSUnitLength ScandinavianMiles { get; }

		[Static]
		[Export ("lightyears", ArgumentSemantic.Copy)]
		NSUnitLength Lightyears { get; }

		[Static]
		[Export ("nauticalMiles", ArgumentSemantic.Copy)]
		NSUnitLength NauticalMiles { get; }

		[Static]
		[Export ("fathoms", ArgumentSemantic.Copy)]
		NSUnitLength Fathoms { get; }

		[Static]
		[Export ("furlongs", ArgumentSemantic.Copy)]
		NSUnitLength Furlongs { get; }

		[Static]
		[Export ("astronomicalUnits", ArgumentSemantic.Copy)]
		NSUnitLength AstronomicalUnits { get; }

		[Static]
		[Export ("parsecs", ArgumentSemantic.Copy)]
		NSUnitLength Parsecs { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitIlluminance : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("lux", ArgumentSemantic.Copy)]
		NSUnitIlluminance Lux { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitMass : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("kilograms", ArgumentSemantic.Copy)]
		NSUnitMass Kilograms { get; }

		[Static]
		[Export ("grams", ArgumentSemantic.Copy)]
		NSUnitMass Grams { get; }

		[Static]
		[Export ("decigrams", ArgumentSemantic.Copy)]
		NSUnitMass Decigrams { get; }

		[Static]
		[Export ("centigrams", ArgumentSemantic.Copy)]
		NSUnitMass Centigrams { get; }

		[Static]
		[Export ("milligrams", ArgumentSemantic.Copy)]
		NSUnitMass Milligrams { get; }

		[Static]
		[Export ("micrograms", ArgumentSemantic.Copy)]
		NSUnitMass Micrograms { get; }

		[Static]
		[Export ("nanograms", ArgumentSemantic.Copy)]
		NSUnitMass Nanograms { get; }

		[Static]
		[Export ("picograms", ArgumentSemantic.Copy)]
		NSUnitMass Picograms { get; }

		[Static]
		[Export ("ounces", ArgumentSemantic.Copy)]
		NSUnitMass Ounces { get; }

		[Static]
		[Export ("poundsMass", ArgumentSemantic.Copy)]
		NSUnitMass Pounds { get; }

		[Static]
		[Export ("stones", ArgumentSemantic.Copy)]
		NSUnitMass Stones { get; }

		[Static]
		[Export ("metricTons", ArgumentSemantic.Copy)]
		NSUnitMass MetricTons { get; }

		[Static]
		[Export ("shortTons", ArgumentSemantic.Copy)]
		NSUnitMass ShortTons { get; }

		[Static]
		[Export ("carats", ArgumentSemantic.Copy)]
		NSUnitMass Carats { get; }

		[Static]
		[Export ("ouncesTroy", ArgumentSemantic.Copy)]
		NSUnitMass OuncesTroy { get; }

		[Static]
		[Export ("slugs", ArgumentSemantic.Copy)]
		NSUnitMass Slugs { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitPower : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("terawatts", ArgumentSemantic.Copy)]
		NSUnitPower Terawatts { get; }

		[Static]
		[Export ("gigawatts", ArgumentSemantic.Copy)]
		NSUnitPower Gigawatts { get; }

		[Static]
		[Export ("megawatts", ArgumentSemantic.Copy)]
		NSUnitPower Megawatts { get; }

		[Static]
		[Export ("kilowatts", ArgumentSemantic.Copy)]
		NSUnitPower Kilowatts { get; }

		[Static]
		[Export ("watts", ArgumentSemantic.Copy)]
		NSUnitPower Watts { get; }

		[Static]
		[Export ("milliwatts", ArgumentSemantic.Copy)]
		NSUnitPower Milliwatts { get; }

		[Static]
		[Export ("microwatts", ArgumentSemantic.Copy)]
		NSUnitPower Microwatts { get; }

		[Static]
		[Export ("nanowatts", ArgumentSemantic.Copy)]
		NSUnitPower Nanowatts { get; }

		[Static]
		[Export ("picowatts", ArgumentSemantic.Copy)]
		NSUnitPower Picowatts { get; }

		[Static]
		[Export ("femtowatts", ArgumentSemantic.Copy)]
		NSUnitPower Femtowatts { get; }

		[Static]
		[Export ("horsepower", ArgumentSemantic.Copy)]
		NSUnitPower Horsepower { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitPressure : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("newtonsPerMetersSquared", ArgumentSemantic.Copy)]
		NSUnitPressure NewtonsPerMetersSquared { get; }

		[Static]
		[Export ("gigapascals", ArgumentSemantic.Copy)]
		NSUnitPressure Gigapascals { get; }

		[Static]
		[Export ("megapascals", ArgumentSemantic.Copy)]
		NSUnitPressure Megapascals { get; }

		[Static]
		[Export ("kilopascals", ArgumentSemantic.Copy)]
		NSUnitPressure Kilopascals { get; }

		[Static]
		[Export ("hectopascals", ArgumentSemantic.Copy)]
		NSUnitPressure Hectopascals { get; }

		[Static]
		[Export ("inchesOfMercury", ArgumentSemantic.Copy)]
		NSUnitPressure InchesOfMercury { get; }

		[Static]
		[Export ("bars", ArgumentSemantic.Copy)]
		NSUnitPressure Bars { get; }

		[Static]
		[Export ("millibars", ArgumentSemantic.Copy)]
		NSUnitPressure Millibars { get; }

		[Static]
		[Export ("millimetersOfMercury", ArgumentSemantic.Copy)]
		NSUnitPressure MillimetersOfMercury { get; }

		[Static]
		[Export ("poundsForcePerSquareInch", ArgumentSemantic.Copy)]
		NSUnitPressure PoundsForcePerSquareInch { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitSpeed : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("metersPerSecond", ArgumentSemantic.Copy)]
		NSUnitSpeed MetersPerSecond { get; }

		[Static]
		[Export ("kilometersPerHour", ArgumentSemantic.Copy)]
		NSUnitSpeed KilometersPerHour { get; }

		[Static]
		[Export ("milesPerHour", ArgumentSemantic.Copy)]
		NSUnitSpeed MilesPerHour { get; }

		[Static]
		[Export ("knots", ArgumentSemantic.Copy)]
		NSUnitSpeed Knots { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitVolume : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("megaliters", ArgumentSemantic.Copy)]
		NSUnitVolume Megaliters { get; }

		[Static]
		[Export ("kiloliters", ArgumentSemantic.Copy)]
		NSUnitVolume Kiloliters { get; }

		[Static]
		[Export ("liters", ArgumentSemantic.Copy)]
		NSUnitVolume Liters { get; }

		[Static]
		[Export ("deciliters", ArgumentSemantic.Copy)]
		NSUnitVolume Deciliters { get; }

		[Static]
		[Export ("centiliters", ArgumentSemantic.Copy)]
		NSUnitVolume Centiliters { get; }

		[Static]
		[Export ("milliliters", ArgumentSemantic.Copy)]
		NSUnitVolume Milliliters { get; }

		[Static]
		[Export ("cubicKilometers", ArgumentSemantic.Copy)]
		NSUnitVolume CubicKilometers { get; }

		[Static]
		[Export ("cubicMeters", ArgumentSemantic.Copy)]
		NSUnitVolume CubicMeters { get; }

		[Static]
		[Export ("cubicDecimeters", ArgumentSemantic.Copy)]
		NSUnitVolume CubicDecimeters { get; }

		[Static]
		[Export ("cubicCentimeters", ArgumentSemantic.Copy)]
		NSUnitVolume CubicCentimeters { get; }

		[Static]
		[Export ("cubicMillimeters", ArgumentSemantic.Copy)]
		NSUnitVolume CubicMillimeters { get; }

		[Static]
		[Export ("cubicInches", ArgumentSemantic.Copy)]
		NSUnitVolume CubicInches { get; }

		[Static]
		[Export ("cubicFeet", ArgumentSemantic.Copy)]
		NSUnitVolume CubicFeet { get; }

		[Static]
		[Export ("cubicYards", ArgumentSemantic.Copy)]
		NSUnitVolume CubicYards { get; }

		[Static]
		[Export ("cubicMiles", ArgumentSemantic.Copy)]
		NSUnitVolume CubicMiles { get; }

		[Static]
		[Export ("acreFeet", ArgumentSemantic.Copy)]
		NSUnitVolume AcreFeet { get; }

		[Static]
		[Export ("bushels", ArgumentSemantic.Copy)]
		NSUnitVolume Bushels { get; }

		[Static]
		[Export ("teaspoons", ArgumentSemantic.Copy)]
		NSUnitVolume Teaspoons { get; }

		[Static]
		[Export ("tablespoons", ArgumentSemantic.Copy)]
		NSUnitVolume Tablespoons { get; }

		[Static]
		[Export ("fluidOunces", ArgumentSemantic.Copy)]
		NSUnitVolume FluidOunces { get; }

		[Static]
		[Export ("cups", ArgumentSemantic.Copy)]
		NSUnitVolume Cups { get; }

		[Static]
		[Export ("pints", ArgumentSemantic.Copy)]
		NSUnitVolume Pints { get; }

		[Static]
		[Export ("quarts", ArgumentSemantic.Copy)]
		NSUnitVolume Quarts { get; }

		[Static]
		[Export ("gallons", ArgumentSemantic.Copy)]
		NSUnitVolume Gallons { get; }

		[Static]
		[Export ("imperialTeaspoons", ArgumentSemantic.Copy)]
		NSUnitVolume ImperialTeaspoons { get; }

		[Static]
		[Export ("imperialTablespoons", ArgumentSemantic.Copy)]
		NSUnitVolume ImperialTablespoons { get; }

		[Static]
		[Export ("imperialFluidOunces", ArgumentSemantic.Copy)]
		NSUnitVolume ImperialFluidOunces { get; }

		[Static]
		[Export ("imperialPints", ArgumentSemantic.Copy)]
		NSUnitVolume ImperialPints { get; }

		[Static]
		[Export ("imperialQuarts", ArgumentSemantic.Copy)]
		NSUnitVolume ImperialQuarts { get; }

		[Static]
		[Export ("imperialGallons", ArgumentSemantic.Copy)]
		NSUnitVolume ImperialGallons { get; }

		[Static]
		[Export ("metricCups", ArgumentSemantic.Copy)]
		NSUnitVolume MetricCups { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSMeasurement<UnitType> : NSCopying, NSSecureCoding
		where UnitType : NSUnit {
		[Export ("unit", ArgumentSemantic.Copy)]
		NSUnit Unit { get; }

		[Export ("doubleValue")]
		double DoubleValue { get; }

		[Export ("initWithDoubleValue:unit:")]
		[DesignatedInitializer]
		NativeHandle Constructor (double doubleValue, NSUnit unit);

		[Export ("canBeConvertedToUnit:")]
		bool CanBeConvertedTo (NSUnit unit);

		[Export ("measurementByConvertingToUnit:")]
		NSMeasurement<UnitType> GetMeasurementByConverting (NSUnit unit);

		[Export ("measurementByAddingMeasurement:")]
		NSMeasurement<UnitType> GetMeasurementByAdding (NSMeasurement<UnitType> measurement);

		[Export ("measurementBySubtractingMeasurement:")]
		NSMeasurement<UnitType> GetMeasurementBySubtracting (NSMeasurement<UnitType> measurement);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter))]
	interface NSMeasurementFormatter : NSSecureCoding {

		[Export ("unitOptions", ArgumentSemantic.Assign)]
		NSMeasurementFormatterUnitOptions UnitOptions { get; set; }

		[Export ("unitStyle", ArgumentSemantic.Assign)]
		NSFormattingUnitStyle UnitStyle { get; set; }

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("numberFormatter", ArgumentSemantic.Copy)]
		NSNumberFormatter NumberFormatter { get; set; }

		[Export ("stringFromMeasurement:")]
		string ToString (NSMeasurement<NSUnit> measurement);

		[Export ("stringFromUnit:")]
		string ToString (NSUnit unit);
	}

	[BaseType (typeof (NSObject), Name = "NSXPCConnection")]
	[DisableDefaultCtor]
	interface NSXpcConnection {
		[Export ("initWithServiceName:")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		NativeHandle Constructor (string xpcServiceName);

		[Export ("serviceName")]
		string ServiceName { get; }

		[Export ("initWithMachServiceName:options:")]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		NativeHandle Constructor (string machServiceName, NSXpcConnectionOptions options);

		[Export ("initWithListenerEndpoint:")]
		NativeHandle Constructor (NSXpcListenerEndpoint endpoint);

		[Export ("endpoint")]
		NSXpcListenerEndpoint Endpoint { get; }

		[Export ("exportedInterface", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSXpcInterface ExportedInterface { get; set; }

		[Export ("exportedObject", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSObject ExportedObject { get; set; }

		[Export ("remoteObjectInterface", ArgumentSemantic.Retain)]
		[NullAllowed]
		NSXpcInterface RemoteInterface { get; set; }

		[Export ("interruptionHandler", ArgumentSemantic.Copy)]
		Action InterruptionHandler { get; set; }

		[Export ("invalidationHandler", ArgumentSemantic.Copy)]
		Action InvalidationHandler { get; set; }

		[Advice ("Prefer using 'Activate' for initial activation of a connection.")]
		[Export ("resume")]
		void Resume ();

		[Export ("suspend")]
		void Suspend ();

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("auditSessionIdentifier")]
		int AuditSessionIdentifier { get; }

		[Export ("processIdentifier")]
		int PeerProcessIdentifier { get; }

		[Export ("effectiveUserIdentifier")]
		int PeerEffectiveUserId { get; }

		[Export ("effectiveGroupIdentifier")]
		int PeerEffectiveGroupId { get; }

		[Export ("currentConnection")]
		[Static]
		NSXpcConnection CurrentConnection { [return: NullAllowed] get; }

		[Export ("scheduleSendBarrierBlock:")]
		[MacCatalyst (13, 1)]
		void ScheduleSendBarrier (Action block);

		[Export ("remoteObjectProxy"), Internal]
		IntPtr _CreateRemoteObjectProxy ();

		[Export ("remoteObjectProxyWithErrorHandler:"), Internal]
		IntPtr _CreateRemoteObjectProxy ([BlockCallback] Action<NSError> errorHandler);

		[MacCatalyst (13, 1)]
		[Export ("synchronousRemoteObjectProxyWithErrorHandler:"), Internal]
		IntPtr _CreateSynchronousRemoteObjectProxy ([BlockCallback] Action<NSError> errorHandler);

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("activate")]
		void Activate ();

		[NoTV, NoiOS]
		[NoMacCatalyst]
		[Export ("setCodeSigningRequirement:")]
		void SetCodeSigningRequirement (string requirement);
	}

	interface INSXpcListenerDelegate { }

	[BaseType (typeof (NSObject), Name = "NSXPCListener", Delegates = new string [] { "WeakDelegate" })]
	[DisableDefaultCtor]
	interface NSXpcListener {
		[Export ("serviceListener")]
		[Static]
		NSXpcListener ServiceListener { get; }

		[Export ("anonymousListener")]
		[Static]
		NSXpcListener AnonymousListener { get; }

		[Export ("initWithMachServiceName:")]
		[DesignatedInitializer]
		[NoiOS]
		[NoTV]
		[NoMacCatalyst]
		NativeHandle Constructor (string machServiceName);

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		INSXpcListenerDelegate Delegate { get; set; }

		[Export ("endpoint")]
		NSXpcListenerEndpoint Endpoint { get; }

		[Advice ("Prefer using 'Activate' for initial activation of a listener.")]
		[Export ("resume")]
		void Resume ();

		[Export ("suspend")]
		void Suspend ();

		[Export ("invalidate")]
		void Invalidate ();

		[TV (14, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("activate")]
		void Activate ();

		[NoTV, NoiOS]
		[NoMacCatalyst]
		[Export ("setConnectionCodeSigningRequirement:")]
		void SetConnectionCodeSigningRequirement (string requirement);
	}

	[BaseType (typeof (NSObject), Name = "NSXPCListenerDelegate")]
	[Protocol, Model]
	interface NSXpcListenerDelegate {
		[Export ("listener:shouldAcceptNewConnection:")]
		bool ShouldAcceptConnection (NSXpcListener listener, NSXpcConnection newConnection);
	}

	[BaseType (typeof (NSObject), Name = "NSXPCInterface")]
	[DisableDefaultCtor]
	interface NSXpcInterface {
		[Export ("interfaceWithProtocol:")]
		[Static]
		NSXpcInterface Create (Protocol protocol);

		[Export ("protocol", ArgumentSemantic.Assign)]
		Protocol Protocol { get; set; }

		[Export ("setClasses:forSelector:argumentIndex:ofReply:")]
		void SetAllowedClasses (NSSet<Class> allowedClasses, Selector methodSelector, nuint argumentIndex, bool forReplyBlock);

		[Export ("classesForSelector:argumentIndex:ofReply:")]
		NSSet<Class> GetAllowedClasses (Selector methodSelector, nuint argumentIndex, bool forReplyBlock);

		// Methods taking xpc_type_t have been skipped.
	}

	[BaseType (typeof (NSObject), Name = "NSXPCListenerEndpoint")]
	[DisableDefaultCtor]
	interface NSXpcListenerEndpoint : NSSecureCoding {
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter))]
	interface NSListFormatter {

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[NullAllowed, Export ("itemFormatter", ArgumentSemantic.Copy)]
		NSFormatter ItemFormatter { get; set; }

		[Static]
		[Export ("localizedStringByJoiningStrings:")]
		// using `NSString[]` since they might be one (or many) `NSString` subclass(es) that handle localization
		string GetLocalizedString (NSString [] joinedStrings);

		[Export ("stringFromItems:")]
		[return: NullAllowed]
		string GetString (NSObject [] items);

		[Export ("stringForObjectValue:")]
		[return: NullAllowed]
		string GetString ([NullAllowed] NSObject obj);
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum NSRelativeDateTimeFormatterStyle : long {
		Numeric = 0,
		Named,
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum NSRelativeDateTimeFormatterUnitsStyle : long {
		Full = 0,
		SpellOut,
		Short,
		Abbreviated,
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter))]
	interface NSRelativeDateTimeFormatter {

		[Export ("dateTimeStyle", ArgumentSemantic.Assign)]
		NSRelativeDateTimeFormatterStyle DateTimeStyle { get; set; }

		[Export ("unitsStyle", ArgumentSemantic.Assign)]
		NSRelativeDateTimeFormatterUnitsStyle UnitsStyle { get; set; }

		[Export ("formattingContext", ArgumentSemantic.Assign)]
		NSFormattingContext FormattingContext { get; set; }

		[Export ("calendar", ArgumentSemantic.Copy)]
		NSCalendar Calendar { get; set; }

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; set; }

		[Export ("localizedStringFromDateComponents:")]
		string GetLocalizedString (NSDateComponents dateComponents);

		[Export ("localizedStringFromTimeInterval:")]
		string GetLocalizedString (double timeInterval);

		[Export ("localizedStringForDate:relativeToDate:")]
		string GetLocalizedString (NSDate date, NSDate referenceDate);

		[Export ("stringForObjectValue:")]
		[return: NullAllowed]
		string GetString ([NullAllowed] NSObject obj);
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum NSCollectionChangeType : long {
		Insert,
		Remove,
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum NSOrderedCollectionDifferenceCalculationOptions : ulong {
		OmitInsertedObjects = (1uL << 0),
		OmitRemovedObjects = (1uL << 1),
		InferMoves = (1uL << 2),
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSDimension))]
	[DisableDefaultCtor] // NSGenericException Reason: -init should never be called on NSUnit!
	interface NSUnitInformationStorage : NSSecureCoding {

		// Inlined from base type
		[Export ("initWithSymbol:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol);

		// Inlined from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("bytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Bytes { get; }

		[Static]
		[Export ("bits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Bits { get; }

		[Static]
		[Export ("nibbles", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Nibbles { get; }

		[Static]
		[Export ("yottabytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Yottabytes { get; }

		[Static]
		[Export ("zettabytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Zettabytes { get; }

		[Static]
		[Export ("exabytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Exabytes { get; }

		[Static]
		[Export ("petabytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Petabytes { get; }

		[Static]
		[Export ("terabytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Terabytes { get; }

		[Static]
		[Export ("gigabytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Gigabytes { get; }

		[Static]
		[Export ("megabytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Megabytes { get; }

		[Static]
		[Export ("kilobytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Kilobytes { get; }

		[Static]
		[Export ("yottabits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Yottabits { get; }

		[Static]
		[Export ("zettabits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Zettabits { get; }

		[Static]
		[Export ("exabits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Exabits { get; }

		[Static]
		[Export ("petabits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Petabits { get; }

		[Static]
		[Export ("terabits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Terabits { get; }

		[Static]
		[Export ("gigabits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Gigabits { get; }

		[Static]
		[Export ("megabits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Megabits { get; }

		[Static]
		[Export ("kilobits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Kilobits { get; }

		[Static]
		[Export ("yobibytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Yobibytes { get; }

		[Static]
		[Export ("zebibytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Zebibytes { get; }

		[Static]
		[Export ("exbibytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Exbibytes { get; }

		[Static]
		[Export ("pebibytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Pebibytes { get; }

		[Static]
		[Export ("tebibytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Tebibytes { get; }

		[Static]
		[Export ("gibibytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Gibibytes { get; }

		[Static]
		[Export ("mebibytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Mebibytes { get; }

		[Static]
		[Export ("kibibytes", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Kibibytes { get; }

		[Static]
		[Export ("yobibits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Yobibits { get; }

		[Static]
		[Export ("zebibits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Zebibits { get; }

		[Static]
		[Export ("exbibits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Exbibits { get; }

		[Static]
		[Export ("pebibits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Pebibits { get; }

		[Static]
		[Export ("tebibits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Tebibits { get; }

		[Static]
		[Export ("gibibits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Gibibits { get; }

		[Static]
		[Export ("mebibits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Mebibits { get; }

		[Static]
		[Export ("kibibits", ArgumentSemantic.Copy)]
		NSUnitInformationStorage Kibibits { get; }
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum NSUrlSessionWebSocketMessageType : long {
		Data = 0,
		String = 1,
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NSURLSessionWebSocketMessage")]
	[DisableDefaultCtor]
	interface NSUrlSessionWebSocketMessage {

		[Export ("initWithData:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSData data);

		[Export ("initWithString:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string @string);

		[Export ("type")]
		NSUrlSessionWebSocketMessageType Type { get; }

		[NullAllowed, Export ("data", ArgumentSemantic.Copy)]
		NSData Data { get; }

		[NullAllowed, Export ("string")]
		string String { get; }
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum NSUrlSessionWebSocketCloseCode : long {
		Invalid = 0,
		NormalClosure = 1000,
		GoingAway = 1001,
		ProtocolError = 1002,
		UnsupportedData = 1003,
		NoStatusReceived = 1005,
		AbnormalClosure = 1006,
		InvalidFramePayloadData = 1007,
		PolicyViolation = 1008,
		MessageTooBig = 1009,
		MandatoryExtensionMissing = 1010,
		InternalServerError = 1011,
		TlsHandshakeFailure = 1015,
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSUrlSessionTask), Name = "NSURLSessionWebSocketTask")]
	[DisableDefaultCtor]
	interface NSUrlSessionWebSocketTask {

		[Export ("sendMessage:completionHandler:")]
		[Async]
		void SendMessage (NSUrlSessionWebSocketMessage message, Action<NSError> completionHandler);

		[Export ("receiveMessageWithCompletionHandler:")]
		[Async]
		void ReceiveMessage (Action<NSUrlSessionWebSocketMessage, NSError> completionHandler);

		[Export ("sendPingWithPongReceiveHandler:")]
		[Async]
		void SendPing (Action<NSError> pongReceiveHandler);

		[Export ("cancelWithCloseCode:reason:")]
		void Cancel (NSUrlSessionWebSocketCloseCode closeCode, [NullAllowed] NSData reason);

		[Export ("maximumMessageSize")]
		nint MaximumMessageSize { get; set; }

		[Export ("closeCode")]
		NSUrlSessionWebSocketCloseCode CloseCode { get; }

		[NullAllowed, Export ("closeReason", ArgumentSemantic.Copy)]
		NSData CloseReason { get; }
	}

	[MacCatalyst (13, 1)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name = "NSURLSessionWebSocketDelegate")]
	interface NSUrlSessionWebSocketDelegate {

		[Export ("URLSession:webSocketTask:didOpenWithProtocol:")]
		void DidOpen (NSUrlSession session, NSUrlSessionWebSocketTask webSocketTask, [NullAllowed] string protocol);

		[Export ("URLSession:webSocketTask:didCloseWithCode:reason:")]
		void DidClose (NSUrlSession session, NSUrlSessionWebSocketTask webSocketTask, NSUrlSessionWebSocketCloseCode closeCode, [NullAllowed] NSData reason);
	}

	[MacCatalyst (13, 1)]
	[Native]
	enum NSUrlErrorNetworkUnavailableReason : long {
		Cellular = 0,
		Expensive = 1,
		Constrained = 2,
		[TV (26, 1), Mac (26, 1), iOS (26, 1)]
		UltraConstrained = 3,
	}

	[NoTV, NoiOS]
	[NoMacCatalyst]
	[Native]
	public enum NSBackgroundActivityResult : long {
		Finished = 1,
		Deferred = 2,
	}

	delegate void NSBackgroundActivityCompletionHandler (NSBackgroundActivityResult result);

	delegate void NSBackgroundActivityCompletionAction ([BlockCallback] NSBackgroundActivityCompletionHandler handler);

	[NoTV, NoiOS]
	[NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSBackgroundActivityScheduler {
		[Export ("initWithIdentifier:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string identifier);

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("qualityOfService", ArgumentSemantic.Assign)]
		NSQualityOfService QualityOfService { get; set; }

		[Export ("repeats")]
		bool Repeats { get; set; }

		[Export ("interval")]
		double Interval { get; set; }

		[Export ("tolerance")]
		double Tolerance { get; set; }

		[Export ("scheduleWithBlock:")]
		void Schedule (NSBackgroundActivityCompletionAction action);

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("shouldDefer")]
		bool ShouldDefer { get; }
	}

	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[Native]
	public enum NSUrlSessionTaskMetricsDomainResolutionProtocol : long {
		Unknown,
		Udp,
		Tcp,
		Tls,
		Https,
	}

	[NoiOS]
	[NoTV]
	[MacCatalyst (15, 0)]
	[Native]
	public enum NSNotificationSuspensionBehavior : ulong {
		/// <summary>To be added.</summary>
		Drop = 1,
		/// <summary>To be added.</summary>
		Coalesce = 2,
		/// <summary>To be added.</summary>
		Hold = 3,
		/// <summary>To be added.</summary>
		DeliverImmediately = 4,
	}

	[NoiOS]
	[NoTV]
	[MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum NSNotificationFlags : ulong {
		/// <summary>To be added.</summary>
		DeliverImmediately = (1 << 0),
		/// <summary>To be added.</summary>
		PostToAllSessions = (1 << 1),
	}

	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[Native]
	[Flags]
	public enum NSFileManagerUnmountOptions : ulong {
		/// <summary>To be added.</summary>
		AllPartitionsAndEjectDisk = 1 << 0,
		/// <summary>To be added.</summary>
		WithoutUI = 1 << 1,
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSPresentationIntent : NSCopying, NSSecureCoding {
		[Export ("intentKind")]
		NSPresentationIntentKind IntentKind { get; }

		[NullAllowed, Export ("parentIntent", ArgumentSemantic.Strong)]
		NSPresentationIntent ParentIntent { get; }

		[Static]
		[Export ("paragraphIntentWithIdentity:nestedInsideIntent:")]
		NSPresentationIntent CreateParagraphIntent (nint identity, [NullAllowed] NSPresentationIntent parent);

		[Static]
		[Export ("headerIntentWithIdentity:level:nestedInsideIntent:")]
		NSPresentationIntent CreateHeaderIntent (nint identity, nint level, [NullAllowed] NSPresentationIntent parent);

		[Static]
		[Export ("codeBlockIntentWithIdentity:languageHint:nestedInsideIntent:")]
		NSPresentationIntent CreateCodeBlockIntent (nint identity, [NullAllowed] string languageHint, [NullAllowed] NSPresentationIntent parent);

		[Static]
		[Export ("thematicBreakIntentWithIdentity:nestedInsideIntent:")]
		NSPresentationIntent CreateThematicBreakIntent (nint identity, [NullAllowed] NSPresentationIntent parent);

		[Static]
		[Export ("orderedListIntentWithIdentity:nestedInsideIntent:")]
		NSPresentationIntent CreateOrderedListIntent (nint identity, [NullAllowed] NSPresentationIntent parent);

		[Static]
		[Export ("unorderedListIntentWithIdentity:nestedInsideIntent:")]
		NSPresentationIntent CreateUnorderedListIntent (nint identity, [NullAllowed] NSPresentationIntent parent);

		[Static]
		[Export ("listItemIntentWithIdentity:ordinal:nestedInsideIntent:")]
		NSPresentationIntent CreateListItemIntent (nint identity, nint ordinal, [NullAllowed] NSPresentationIntent parent);

		[Static]
		[Export ("blockQuoteIntentWithIdentity:nestedInsideIntent:")]
		NSPresentationIntent CreateBlockQuoteIntent (nint identity, [NullAllowed] NSPresentationIntent parent);

		[Static]
		[Export ("tableIntentWithIdentity:columnCount:alignments:nestedInsideIntent:")]
		NSPresentationIntent CreateTableIntent (nint identity, nint columnCount, NSNumber [] alignments, [NullAllowed] NSPresentationIntent parent);

		[Static]
		[Export ("tableHeaderRowIntentWithIdentity:nestedInsideIntent:")]
		NSPresentationIntent CreateTableHeaderRowIntent (nint identity, [NullAllowed] NSPresentationIntent parent);

		[Static]
		[Export ("tableRowIntentWithIdentity:row:nestedInsideIntent:")]
		NSPresentationIntent CreateTableRowIntent (nint identity, nint row, [NullAllowed] NSPresentationIntent parent);

		[Static]
		[Export ("tableCellIntentWithIdentity:column:nestedInsideIntent:")]
		NSPresentationIntent CreateTableCellIntent (nint identity, nint column, [NullAllowed] NSPresentationIntent parent);

		[Export ("identity")]
		nint Identity { get; }

		[Export ("ordinal")]
		nint Ordinal { get; }

		[NullAllowed, Export ("columnAlignments")]
		NSNumber [] ColumnAlignments { get; }

		[Export ("columnCount")]
		nint ColumnCount { get; }

		[Export ("headerLevel")]
		nint HeaderLevel { get; }

		[NullAllowed, Export ("languageHint")]
		string LanguageHint { get; }

		[Export ("column")]
		nint Column { get; }

		[Export ("row")]
		nint Row { get; }

		[Export ("indentationLevel")]
		nint IndentationLevel { get; }

		[Export ("isEquivalentToPresentationIntent:")]
		bool IsEquivalent (NSPresentationIntent other);
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface NSAttributedStringMarkdownParsingOptions : NSCopying {
		[Export ("allowsExtendedAttributes")]
		bool AllowsExtendedAttributes { get; set; }

		[Export ("interpretedSyntax", ArgumentSemantic.Assign)]
		NSAttributedStringMarkdownInterpretedSyntax InterpretedSyntax { get; set; }

		[Export ("failurePolicy", ArgumentSemantic.Assign)]
		NSAttributedStringMarkdownParsingFailurePolicy FailurePolicy { get; set; }

		[NullAllowed, Export ("languageCode")]
		string LanguageCode { get; set; }

		[TV (16, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("appliesSourcePositionAttributes")]
		bool AppliesSourcePositionAttributes { get; set; }
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSInflectionRule : NSCopying, NSSecureCoding {
		[Static]
		[Export ("automaticRule")]
		NSInflectionRule AutomaticRule { get; }

		[TV (15, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("canInflectLanguage:")]
		bool CanInflectLanguage (string language);

		[TV (15, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("canInflectPreferredLocalization")]
		bool CanInflectPreferredLocalization { get; }
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSInflectionRule))]
	interface NSInflectionRuleExplicit {
		[Export ("initWithMorphology:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSMorphology morphology);

		[Export ("morphology", ArgumentSemantic.Copy)]
		NSMorphology Morphology { get; }
	}

	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface NSMorphology : NSCopying, NSSecureCoding {
		[Export ("grammaticalGender", ArgumentSemantic.Assign)]
		NSGrammaticalGender GrammaticalGender { get; set; }

		[Export ("partOfSpeech", ArgumentSemantic.Assign)]
		NSGrammaticalPartOfSpeech PartOfSpeech { get; set; }

		[Export ("number", ArgumentSemantic.Assign)]
		NSGrammaticalNumber Number { get; set; }

		[Obsoleted (PlatformName.MacOSX, 14, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Obsoleted (PlatformName.iOS, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Obsoleted (PlatformName.MacCatalyst, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Obsoleted (PlatformName.TvOS, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Export ("customPronounForLanguage:")]
		[return: NullAllowed]
		NSMorphologyCustomPronoun GetCustomPronoun (string language);

		[Obsoleted (PlatformName.MacOSX, 14, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Obsoleted (PlatformName.iOS, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Obsoleted (PlatformName.MacCatalyst, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Obsoleted (PlatformName.TvOS, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Export ("setCustomPronoun:forLanguage:error:")]
		bool SetCustomPronoun ([NullAllowed] NSMorphologyCustomPronoun features, string language, [NullAllowed] out NSError error);

		[Export ("unspecified")]
		bool Unspecified { [Bind ("isUnspecified")] get; }

		[Static]
		[Export ("userMorphology")]
		NSMorphology UserMorphology { get; }

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("grammaticalCase", ArgumentSemantic.Assign)]
		NSGrammaticalCase GrammaticalCase { get; set; }

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("determination", ArgumentSemantic.Assign)]
		NSGrammaticalDetermination Determination { get; set; }

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("grammaticalPerson", ArgumentSemantic.Assign)]
		NSGrammaticalPerson GrammaticalPerson { get; set; }

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("pronounType", ArgumentSemantic.Assign)]
		NSGrammaticalPronounType PronounType { get; set; }

		[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("definiteness", ArgumentSemantic.Assign)]
		NSGrammaticalDefiniteness Definiteness { get; set; }
	}

	[Obsoleted (PlatformName.MacOSX, 14, 0, message: "Use 'NSTermOfAddress' instead.")]
	[Obsoleted (PlatformName.iOS, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
	[Obsoleted (PlatformName.MacCatalyst, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
	[Obsoleted (PlatformName.TvOS, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
	[TV (15, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	interface NSMorphologyCustomPronoun : NSCopying, NSSecureCoding {
		[Static]
		[Export ("isSupportedForLanguage:")]
		bool IsSupported (string language);

		[Static]
		[Export ("requiredKeysForLanguage:")]
		string [] GetRequiredKeysForLanguage (string language);

		[NullAllowed, Export ("subjectForm")]
		string SubjectForm { get; set; }

		[NullAllowed, Export ("objectForm")]
		string ObjectForm { get; set; }

		[NullAllowed, Export ("possessiveForm")]
		string PossessiveForm { get; set; }

		[NullAllowed, Export ("possessiveAdjectiveForm")]
		string PossessiveAdjectiveForm { get; set; }

		[NullAllowed, Export ("reflexiveForm")]
		string ReflexiveForm { get; set; }
	}

#if false // https://github.com/dotnet/macios/issues/15577
	interface NSOrderedCollectionChange <TKey> : NSOrderedCollectionChange {}
	
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSOrderedCollectionChange
	{
		[Internal]
		[Static]
		[Export ("changeWithObject:type:index:")]
		NativeHandle _ChangeWithObject ([NullAllowed] IntPtr anObject, NSCollectionChangeType type, nuint index);

		[Internal]
		[Static]
		[Export ("changeWithObject:type:index:associatedIndex:")]
		NativeHandle _ChangeWithObject ([NullAllowed] IntPtr anObject, NSCollectionChangeType type, nuint index, nuint associatedIndex);

		[Internal]
		[NullAllowed, Export ("object", ArgumentSemantic.Strong)]
		NativeHandle _Object { get; }

		[Export ("changeType")]
		NSCollectionChangeType ChangeType { get; }

		[Export ("index")]
		nuint Index { get; }

		[Export ("associatedIndex")]
		nuint AssociatedIndex { get; }

		[Internal]
		[Export ("initWithObject:type:index:")]
		NativeHandle Constructor (IntPtr anObject, NSCollectionChangeType type, nuint index);
		
		[Wrap ("this (anObject!.Handle, type, index)")]
		NativeHandle Constructor ([NullAllowed] NSObject anObject, NSCollectionChangeType type, nuint index);

		[Internal]
		[DesignatedInitializer]
		[Export ("initWithObject:type:index:associatedIndex:")]
		NativeHandle Constructor (IntPtr anObject, NSCollectionChangeType type, nuint index, nuint associatedIndex);
		
		[Wrap ("this (anObject!.Handle, type, index, associatedIndex)")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] NSObject anObject, NSCollectionChangeType type, nuint index, nuint associatedIndex);
	}

	interface NSOrderedCollectionDifference <TKey> : NSOrderedCollectionDifference {}
	
	[BaseType (typeof (NSObject))]
	interface NSOrderedCollectionDifference : INSFastEnumeration
	{
		[Export ("initWithChanges:")]
		NativeHandle Constructor (NSOrderedCollectionChange[] changes);
		
		[Internal]
		[DesignatedInitializer]
		[Export ("initWithInsertIndexes:insertedObjects:removeIndexes:removedObjects:additionalChanges:")]
		NativeHandle Constructor (NSIndexSet inserts, [NullAllowed] NSArray insertedObjects, NSIndexSet removes, [NullAllowed] NSArray removedObjects, NSOrderedCollectionChange[] changes);

		[Wrap ("this (inserts, NSArray.FromNSObjects (insertedObjects), removes, NSArray.FromNSObjects (removedObjects), changes)")]
		NativeHandle Constructor (NSIndexSet inserts, [NullAllowed] NSObject[] insertedObjects, NSIndexSet removes, [NullAllowed] NSObject[] removedObjects, NSOrderedCollectionChange[] changes);

		[Internal]
		[Export ("initWithInsertIndexes:insertedObjects:removeIndexes:removedObjects:")]
		NativeHandle Constructor (NSIndexSet inserts, [NullAllowed] NSArray insertedObjects, NSIndexSet removes, [NullAllowed] NSArray removedObjects);
		
		[Wrap ("this (inserts, NSArray.FromNSObjects (insertedObjects), removes, NSArray.FromNSObjects (removedObjects))")]
		NativeHandle Constructor (NSIndexSet inserts, [NullAllowed] NSObject[] insertedObjects, NSIndexSet removes, [NullAllowed] NSObject[] removedObjects);

		[Internal]
		[Export ("insertions", ArgumentSemantic.Strong)]
		NativeHandle _Insertions { get; }

		[Internal]
		[Export ("removals", ArgumentSemantic.Strong)]
		NativeHandle _Removals { get; }

		[Export ("hasChanges")]
		bool HasChanges { get; }

		[Internal]
		[Export ("differenceByTransformingChangesWithBlock:")]
		NativeHandle _GetDifference (/* Func<NSOrderedCollectionChange<NSObject>, NSOrderedCollectionChange<NSObject>>*/ ref BlockLiteral block); 

		[Internal]
		[Export ("inverseDifference")]
		NativeHandle _InverseDifference ();
	}
#endif

	[TV (16, 0), iOS (16, 0)]
	[MacCatalyst (16, 0)]
	[BaseType (typeof (NSObject))]
	interface NSAttributedStringMarkdownSourcePosition : NSCopying, NSSecureCoding {
		[Export ("startLine")]
		nint StartLine { get; }

		[Export ("startColumn")]
		nint StartColumn { get; }

		[Export ("endLine")]
		nint EndLine { get; }

		[Export ("endColumn")]
		nint EndColumn { get; }

		[Export ("initWithStartLine:startColumn:endLine:endColumn:")]
		NativeHandle Constructor (nint startLine, nint startColumn, nint endLine, nint endColumn);

		[Export ("rangeInString:")]
		NSRange RangeInString (string @string);
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSTermOfAddress : NSCopying, NSSecureCoding {
		[Static]
		[Export ("neutral")]
		NSTermOfAddress Neutral { get; }

		[Static]
		[Export ("feminine")]
		NSTermOfAddress Feminine { get; }

		[Static]
		[Export ("masculine")]
		NSTermOfAddress Masculine { get; }

		[Static]
		[Export ("localizedForLanguageIdentifier:withPronouns:")]
		NSTermOfAddress GetLocalized (string language, NSMorphologyPronoun [] pronouns);

		[NullAllowed, Export ("languageIdentifier")]
		string LanguageIdentifier { get; }

		[NullAllowed, Export ("pronouns", ArgumentSemantic.Copy)]
		NSMorphologyPronoun [] Pronouns { get; }

		[Static]
		[Export ("currentUser")]
		[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
		NSTermOfAddress CurrentUser { get; }
	}

	[TV (17, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSMorphologyPronoun : NSCopying, NSSecureCoding {
		[Export ("initWithPronoun:morphology:dependentMorphology:")]
		NativeHandle Constructor (string pronoun, NSMorphology morphology, [NullAllowed] NSMorphology dependentMorphology);

		[Export ("pronoun")]
		string Pronoun { get; }

		[Export ("morphology", ArgumentSemantic.Copy)]
		NSMorphology Morphology { get; }

		[NullAllowed, Export ("dependentMorphology", ArgumentSemantic.Copy)]
		NSMorphology DependentMorphology { get; }
	}

	[StrongDictionary (nameof (NSAttributedStringDocumentAttributeKey), Suffix = "DocumentAttribute")]
	interface NSAttributedStringDocumentAttributes {
		// Wait with this one until XAMCORE_5_0, using the strong dictionary logic would be an API break.
#if XAMCORE_5_0
		NSAttributedStringDocumentType DocumentType { get; set; }
#endif

		NSStringEncoding CharacterEncoding { get; set; }

		NSAttributedStringDocumentAttributes DefaultAttributes { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		CGSize PaperSize { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoMac]
		UIEdgeInsets PaperMargin { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		CGSize ViewSize { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		float ViewZoom { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		NSDocumentViewMode ViewMode { get; set; }

		// The definition for this boolean is very specific in the header file:
		// "NSNumber containing integer; if missing, or 0 or negative, not readonly; 1 or more, readonly"
		// So keep the manual code, the generic strong dictionary logic is slightly different.
		// #if XAMCORE_5_0
		// bool? ReadOnly { get; set; }
		// #else
		// bool ReadOnly { get; set; }
		// #endif

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		NSColor BackgroundColor { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		float HyphenationFactor {
			get;
			[PreSnippet ("if (value < 0 || value > 1.0f) throw new ArgumentOutOfRangeException (nameof (value), value, \"Value must be between 0 and 1\");")]
			set;
		}

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		float DefaultTabInterval {
			get;
			[PreSnippet ("if (value < 0 || value > 1.0f) throw new ArgumentOutOfRangeException (nameof (value), value, \"Value must be between 0 and 1\");")]
			set;
		}

		// This would need a custom binding, it's an array of another strong dictionary.
		// [Export ("TextLayoutSectionsAttribute")]
		// NSTextLayoutSection[] TextLayout { get; set; }

		NSTextScalingType TextScaling { get; set; }

		NSTextScalingType SourceTextScaling { get; set; }

		float CocoaVersion { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		int Converted { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string FileType { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string Title { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string Company { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string Copyright { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string Subject { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string Author { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string [] Keywords { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string Comment { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string Editor { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		NSDate CreationTime { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		NSDate ModificationTime { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string Manager { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string Category { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		NSAppearance Appearance { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		float LeftMargin { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		float RightMargin { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		float TopMargin { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		float BottomMargin { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		string [] ExcludedElements { get; set; }

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[NoiOS, NoTV, NoMacCatalyst]
		string TextEncodingName { get; set; }

		[NoiOS, NoTV, NoMacCatalyst]
		int PrefixSpaces { get; set; }
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSKeyValueSharedObserversSnapshot {
	}

	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSKeyValueSharedObservers {
		[Export ("initWithObservableClass:")]
		NativeHandle Constructor (Class observableClass);

		[Wrap ("this (new Class (observableType))")]
		NativeHandle Constructor (Type observableType);

		[Export ("addSharedObserver:forKey:options:context:")]
		void AddSharedObserver (NSObject observer, string forKey, NSKeyValueObservingOptions options, IntPtr context);

		[Export ("snapshot")]
		NSKeyValueSharedObserversSnapshot GetSnapshot ();
	}

	[Category, BaseType (typeof (NSObject))]
	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	interface NSKeyValueSharedObserverRegistration_NSObject {
		[Export ("setSharedObservers:")]
		void SetSharedObservers ([NullAllowed] NSKeyValueSharedObserversSnapshot sharedObservers);
	}

	[BaseType (typeof (NSObject))]
	[TV (18, 0), Mac (15, 0), iOS (18, 0), MacCatalyst (18, 0)]
	[DisableDefaultCtor]
	interface NSLocalizedNumberFormatRule : NSCopying, NSSecureCoding {
		[Static]
		[Export ("automatic")]
		NSLocalizedNumberFormatRule Automatic { get; }
	}

	[MacCatalyst (26, 0), TV (26, 0), Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum NSFileManagerSupportedSyncControls : ulong {
		PauseSync = 1uL << 0,
		FailUploadOnConflict = 1uL << 1,
	}

	[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum NSFileManagerResumeSyncBehavior : long {
		PreserveLocalChanges = 0,
		AfterUploadWithFailOnConflict = 1,
		DropLocalChanges = 2,
	}

	[MacCatalyst (26, 0), NoTV, Mac (26, 0), iOS (26, 0)]
	[Native]
	public enum NSFileManagerUploadLocalVersionConflictPolicy : long {
		Default = 0,
		FailOnConflict = 1,
	}

}
