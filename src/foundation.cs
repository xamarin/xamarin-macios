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
#define DOUBLE_BLOCKS
using ObjCRuntime;
using CloudKit;
using CoreData;
using CoreFoundation;
using Foundation;
using CoreGraphics;
using UniformTypeIdentifiers;
#if HAS_APPCLIP
using AppClip;
#endif
#if IOS
using QuickLook;
#endif
#if !TVOS
using Contacts;
#endif
#if !WATCH
using CoreAnimation;
using CoreSpotlight;
#endif
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

using System;
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

#if WATCH
// dummy usings to make code compile without having the actual types available (for [NoWatch] to work)
using NSAppleEventSendOptions = Foundation.NSObject;
using NSBezierPath = Foundation.NSObject;
using NSImage = Foundation.NSObject;
using CSSearchableItemAttributeSet = Foundation.NSObject;
#endif

#if WATCH
using CIBarcodeDescriptor = Foundation.NSObject;
#else
using CoreImage;
#endif

#if !IOS
using APActivationPayload = Foundation.NSObject;
#endif

#if __MACCATALYST__
using NSAppleEventSendOptions = Foundation.NSObject;
using NSBezierPath = Foundation.NSObject;
using NSImage = Foundation.NSObject;
#endif

#if IOS || WATCH || TVOS
using NSNotificationSuspensionBehavior = Foundation.NSObject;
using NSNotificationFlags = Foundation.NSObject;
using NSTextBlock = Foundation.NSObject;
using NSTextTable = Foundation.NSString; // Different frmo NSTextBlock, because some methods overload on these two types.
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Foundation {
	delegate void NSFilePresenterReacquirer ([BlockCallback] Action reacquirer);
}

namespace Foundation {
	delegate NSComparisonResult NSComparator (NSObject obj1, NSObject obj2);
	delegate void NSAttributedRangeCallback (NSDictionary attrs, NSRange range, ref bool stop);
	delegate void NSAttributedStringCallback (NSObject value, NSRange range, ref bool stop);

	delegate bool NSEnumerateErrorHandler (NSUrl url, NSError error);
	delegate void NSMetadataQueryEnumerationCallback (NSObject result, nuint idx, ref bool stop);
#if NET
	delegate void NSItemProviderCompletionHandler (INSSecureCoding itemBeingLoaded, NSError error);
#else
	delegate void NSItemProviderCompletionHandler (NSObject itemBeingLoaded, NSError error);
#endif
	delegate void NSItemProviderLoadHandler ([BlockCallback] NSItemProviderCompletionHandler completionHandler, Class expectedValueClass, NSDictionary options);
	delegate void EnumerateDatesCallback (NSDate date, bool exactMatch, ref bool stop);
	delegate void EnumerateIndexSetCallback (nuint idx, ref bool stop);
	delegate void CloudKitRegistrationPreparationAction ([BlockCallback] CloudKitRegistrationPreparationHandler handler);
	delegate void CloudKitRegistrationPreparationHandler (CKShare share, CKContainer container, NSError error);

#if NET
	[BaseType (typeof (NSObject))]
	interface NSAutoreleasePool {
	}
#endif

	interface NSArray<TValue> : NSArray { }

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {
		[Export ("count")]
		nuint Count { get; }

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
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'Write (NSUrl, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Write (NSUrl, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Write (NSUrl, out NSError)' instead.")]
		[Export ("writeToFile:atomically:")]
		bool WriteToFile (string path, bool useAuxiliaryFile);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSMutableArray.FromFile' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSMutableArray.FromFile' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NSMutableArray.FromFile' instead.")]
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

#if false // https://github.com/xamarin/xamarin-macios/issues/15577
		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Internal]
		[Export ("differenceFromArray:withOptions:")]
		NativeHandle _GetDifference (NSArray other, NSOrderedCollectionDifferenceCalculationOptions options);

		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Wrap ("Runtime.GetNSObject <NSOrderedCollectionDifference> (_GetDifference (NSArray.FromNSObjects (other), options))")]
		[return: NullAllowed]
		NSOrderedCollectionDifference GetDifference (NSObject[] other, NSOrderedCollectionDifferenceCalculationOptions options);

		[Internal]
		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Export ("differenceFromArray:")]
		NativeHandle _GetDifference (NSArray other);

		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Wrap ("Runtime.GetNSObject <NSOrderedCollectionDifference> (_GetDifference(NSArray.FromNSObjects (other)))")]
		[return: NullAllowed]
		NSOrderedCollectionDifference GetDifference (NSObject[] other);

		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Export ("arrayByApplyingDifference:")]
		[return: NullAllowed]
		NativeHandle _GetArrayByApplyingDifference (NSOrderedCollectionDifference difference);

		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Wrap ("NSArray.ArrayFromHandle<NSObject> (_GetArrayByApplyingDifference (difference))")]
		[return: NullAllowed]
		NSObject[] GetArrayByApplyingDifference (NSOrderedCollectionDifference difference);

		[Internal]
		[Watch (6,0), TV (13,0), iOS (13,0)]
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
#if !WATCH
		[Static, Export ("attributedStringWithAttachment:")]
		NSAttributedString FromAttachment (NSTextAttachment attachment);
#endif

		[Export ("string")]
		IntPtr LowLevelValue { get; }

		[Export ("attributesAtIndex:effectiveRange:")]
#if NET
		IntPtr LowLevelGetAttributes (nint location, IntPtr effectiveRange);
#else
		IntPtr LowLevelGetAttributes (nint location, out NSRange effectiveRange);
#endif

		[Export ("length")]
		nint Length { get; }

		// TODO: figure out the type, this deserves to be strongly typed if possble
		[Export ("attribute:atIndex:effectiveRange:")]
		NSObject GetAttribute (string attribute, nint location, out NSRange effectiveRange);

		[Export ("attributedSubstringFromRange:"), Internal]
		NSAttributedString Substring (NSRange range);

		[Export ("attributesAtIndex:longestEffectiveRange:inRange:")]
		NSDictionary GetAttributes (nint location, out NSRange longestEffectiveRange, NSRange rangeLimit);

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

		[Export ("initWithURL:options:documentAttributes:error:")]
#if !(__MACOS__ || XAMCORE_5_0)
		NativeHandle Constructor (NSUrl url, NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);
#else
		NativeHandle Constructor (NSUrl url, NSDictionary options, out NSDictionary resultDocumentAttributes, out NSError error);
#endif

		[Export ("initWithData:options:documentAttributes:error:")]
#if XAMCORE_5_0
		NativeHandle Constructor (NSData data, NSDictionary options, out NSDictionary resultDocumentAttributes, out NSError error);
#elif __MACOS__
		NativeHandle Constructor (NSData data, NSDictionary options, out NSDictionary docAttributes, out NSError error);
#else
		NativeHandle Constructor (NSData data, NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);
#endif

#if __MACOS__ || XAMCORE_5_0
		[Wrap ("this (url, options.GetDictionary ()!, out resultDocumentAttributes, out error)")]
		NativeHandle Constructor (NSUrl url, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, out NSError error);
#else
		[Wrap ("this (url, options.GetDictionary ()!, out resultDocumentAttributes, ref error)")]
		NativeHandle Constructor (NSUrl url, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, ref NSError error);
#endif

#if __MACOS__ || XAMCORE_5_0
		[Wrap ("this (data, options.GetDictionary ()!, out resultDocumentAttributes, out error)")]
		NativeHandle Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, out NSError error);
#else
		[Wrap ("this (data, options.GetDictionary ()!, out resultDocumentAttributes, ref error)")]
		NativeHandle Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, ref NSError error);
#endif

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("initWithDocFormat:documentAttributes:")]
		NativeHandle Constructor (NSData wordDocFormat, out NSDictionary docAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("initWithHTML:baseURL:documentAttributes:")]
		NativeHandle Constructor (NSData htmlData, NSUrl baseUrl, out NSDictionary docAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("drawWithRect:options:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSAttributedString (NSUrl, NSDictionary, out NSDictionary, ref NSError)' instead.")]
		[Export ("initWithPath:documentAttributes:")]
		NativeHandle Constructor (string path, out NSDictionary resultDocumentAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSAttributedString (NSUrl, NSDictionary, out NSDictionary, ref NSError)' instead.")]
		[Export ("initWithURL:documentAttributes:")]
		NativeHandle Constructor (NSUrl url, out NSDictionary resultDocumentAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Internal, Export ("initWithRTF:documentAttributes:")]
		IntPtr InitWithRtf (NSData data, out NSDictionary resultDocumentAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Internal, Export ("initWithRTFD:documentAttributes:")]
		IntPtr InitWithRtfd (NSData data, out NSDictionary resultDocumentAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Internal, Export ("initWithHTML:documentAttributes:")]
		IntPtr InitWithHTML (NSData data, out NSDictionary resultDocumentAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("initWithHTML:options:documentAttributes:")]
		NativeHandle Constructor (NSData data, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Wrap ("this (data, options.GetDictionary (), out resultDocumentAttributes)")]
		NativeHandle Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("initWithRTFDFileWrapper:documentAttributes:")]
		NativeHandle Constructor (NSFileWrapper wrapper, out NSDictionary resultDocumentAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("containsAttachments")]
		bool ContainsAttachments { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("fontAttributesInRange:")]
		NSDictionary GetFontAttributes (NSRange range);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("rulerAttributesInRange:")]
		NSDictionary GetRulerAttributes (NSRange range);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("lineBreakBeforeIndex:withinRange:")]
		nuint GetLineBreak (nuint beforeIndex, NSRange aRange);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("lineBreakByHyphenatingBeforeIndex:withinRange:")]
		nuint GetLineBreakByHyphenating (nuint beforeIndex, NSRange aRange);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("doubleClickAtIndex:")]
		NSRange DoubleClick (nuint index);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("nextWordFromIndex:forward:")]
		nuint GetNextWord (nuint fromIndex, bool isForward);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSDataDetector' instead.")]
		[Export ("URLAtIndex:effectiveRange:")]
		NSUrl GetUrl (nuint index, out NSRange effectiveRange);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("rangeOfTextBlock:atIndex:")]
		NSRange GetRange (NSTextBlock textBlock, nuint index);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("rangeOfTextTable:atIndex:")]
		NSRange GetRange (NSTextTable textTable, nuint index);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("rangeOfTextList:atIndex:")]
		NSRange GetRange (NSTextList textList, nuint index);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("itemNumberInTextList:atIndex:")]
		nint GetItemNumber (NSTextList textList, nuint index);

#if !(MONOMAC || XAMCORE_5_0)
		[Sealed]
#endif
		[return: NullAllowed]
		[Export ("dataFromRange:documentAttributes:error:")]
		NSData GetData (NSRange range, NSDictionary options, out NSError error);

		[return: NullAllowed]
		[Wrap ("this.GetData (range, options.GetDictionary ()!, out error)")]
		NSData GetData (NSRange range, NSAttributedStringDocumentAttributes options, out NSError error);

#if !(MONOMAC || XAMCORE_5_0)
		[return: NullAllowed]
		[Obsolete ("Use 'GetData' instead.")]
		[Export ("dataFromRange:documentAttributes:error:")]
		NSData GetDataFromRange (NSRange range, NSDictionary attributes, ref NSError error);
#endif

#if !(MONOMAC || XAMCORE_5_0)
		[return: NullAllowed]
		[Obsolete ("Use 'GetData' instead.")]
		[Wrap ("GetDataFromRange (range, documentAttributes.GetDictionary ()!, ref error)")]
		NSData GetDataFromRange (NSRange range, NSAttributedStringDocumentAttributes documentAttributes, ref NSError error);
#endif

#if !(MONOMAC || XAMCORE_5_0)
		[Sealed]
#endif
		[return: NullAllowed]
		[Export ("fileWrapperFromRange:documentAttributes:error:")]
		NSFileWrapper GetFileWrapper (NSRange range, NSDictionary options, out NSError error);

#if !(MONOMAC || XAMCORE_5_0)
		[return: NullAllowed]
		[Obsolete ("Use 'GetFileWrapper' instead.")]
		[Export ("fileWrapperFromRange:documentAttributes:error:")]
		NSFileWrapper GetFileWrapperFromRange (NSRange range, NSDictionary attributes, ref NSError error);
#endif

		[return: NullAllowed]
		[Wrap ("this.GetFileWrapper (range, options.GetDictionary ()!, out error)")]
		NSFileWrapper GetFileWrapper (NSRange range, NSAttributedStringDocumentAttributes options, out NSError error);

#if !(MONOMAC || XAMCORE_5_0)
		[return: NullAllowed]
		[Obsolete ("Use 'GetFileWrapper' instead.")]
		[Wrap ("GetFileWrapperFromRange (range, documentAttributes.GetDictionary ()!, ref error)")]
		NSFileWrapper GetFileWrapperFromRange (NSRange range, NSAttributedStringDocumentAttributes documentAttributes, ref NSError error);
#endif

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("RTFFromRange:documentAttributes:")]
		NSData GetRtf (NSRange range, [NullAllowed] NSDictionary options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Wrap ("this.GetRtf (range, options.GetDictionary ())")]
		NSData GetRtf (NSRange range, NSAttributedStringDocumentAttributes options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("RTFDFromRange:documentAttributes:")]
		NSData GetRtfd (NSRange range, [NullAllowed] NSDictionary options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Wrap ("this.GetRtfd (range, options.GetDictionary ())")]
		NSData GetRtfd (NSRange range, NSAttributedStringDocumentAttributes options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("RTFDFileWrapperFromRange:documentAttributes:")]
		NSFileWrapper GetRtfdFileWrapper (NSRange range, [NullAllowed] NSDictionary options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Wrap ("this.GetRtfdFileWrapper (range, options.GetDictionary ())")]
		NSFileWrapper GetRtfdFileWrapper (NSRange range, NSAttributedStringDocumentAttributes options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("docFormatFromRange:documentAttributes:")]
		NSData GetDocFormat (NSRange range, [NullAllowed] NSDictionary options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Wrap ("this.GetDocFormat (range, options.GetDictionary ())")]
		NSData GetDocFormat (NSRange range, NSAttributedStringDocumentAttributes options);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("drawWithRect:options:context:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options, [NullAllowed] NSStringDrawingContext context);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("boundingRectWithSize:options:context:")]
		CGRect GetBoundingRect (CGSize size, NSStringDrawingOptions options, [NullAllowed] NSStringDrawingContext context);

		[MacCatalyst (13, 1)]
		[Export ("size")]
		CGSize Size { get; }

		[Export ("drawAtPoint:")]
		void DrawString (CGPoint point);

		[Export ("drawInRect:")]
		void DrawString (CGRect rect);

		// -(BOOL)containsAttachmentsInRange:(NSRange)range __attribute__((availability(macosx, introduced=10.11)));
		[MacCatalyst (13, 1)]
		[Export ("containsAttachmentsInRange:")]
		bool ContainsAttachmentsInRange (NSRange range);

		// inlined from NSAttributedStringWebKitAdditions category (since they are all static members)

		[NoWatch]
		[NoTV] // really inside WebKit
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("loadFromHTMLWithRequest:options:completionHandler:")]
		[PreSnippet ("GC.KeepAlive (WebKit.WKContentMode.Recommended); // no-op to ensure WebKit.framework is loaded into memory", Optimizable = true)]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		void LoadFromHtml (NSUrlRequest request, NSDictionary options, NSAttributedStringCompletionHandler completionHandler);

		[NoWatch]
		[NoTV] // really inside WebKit
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[Wrap ("LoadFromHtml (request, options.GetDictionary ()!, completionHandler)")]
		void LoadFromHtml (NSUrlRequest request, NSAttributedStringDocumentAttributes options, NSAttributedStringCompletionHandler completionHandler);

		[NoWatch]
		[NoTV] // really inside WebKit
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("loadFromHTMLWithFileURL:options:completionHandler:")]
		[PreSnippet ("GC.KeepAlive (WebKit.WKContentMode.Recommended); // no-op to ensure WebKit.framework is loaded into memory", Optimizable = true)]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		void LoadFromHtml (NSUrl fileUrl, NSDictionary options, NSAttributedStringCompletionHandler completionHandler);

		[NoWatch]
		[NoTV] // really inside WebKit
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[Wrap ("LoadFromHtml (fileUrl, options.GetDictionary ()!, completionHandler)")]
		void LoadFromHtml (NSUrl fileUrl, NSAttributedStringDocumentAttributes options, NSAttributedStringCompletionHandler completionHandler);

		[NoWatch]
		[NoTV] // really inside WebKit
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("loadFromHTMLWithString:options:completionHandler:")]
		[PreSnippet ("GC.KeepAlive (WebKit.WKContentMode.Recommended); // no-op to ensure WebKit.framework is loaded into memory", Optimizable = true)]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		void LoadFromHtml (string @string, NSDictionary options, NSAttributedStringCompletionHandler completionHandler);

		[NoWatch]
		[NoTV] // really inside WebKit
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[Wrap ("LoadFromHtml (@string, options.GetDictionary ()!, completionHandler)")]
		void LoadFromHtml (string @string, NSAttributedStringDocumentAttributes options, NSAttributedStringCompletionHandler completionHandler);

		[NoWatch]
		[NoTV] // really inside WebKit
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("loadFromHTMLWithData:options:completionHandler:")]
		[PreSnippet ("GC.KeepAlive (WebKit.WKContentMode.Recommended); // no-op to ensure WebKit.framework is loaded into memory", Optimizable = true)]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		void LoadFromHtml (NSData data, NSDictionary options, NSAttributedStringCompletionHandler completionHandler);

		[NoWatch]
		[NoTV] // really inside WebKit
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Async (ResultTypeName = "NSLoadFromHtmlResult")]
		[Wrap ("LoadFromHtml (data, options.GetDictionary ()!, completionHandler)")]
		void LoadFromHtml (NSData data, NSAttributedStringDocumentAttributes options, NSAttributedStringCompletionHandler completionHandler);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithContentsOfMarkdownFileAtURL:options:baseURL:error:")]
		NativeHandle Constructor (NSUrl markdownFile, [NullAllowed] NSAttributedStringMarkdownParsingOptions options, [NullAllowed] NSUrl baseUrl, [NullAllowed] out NSError error);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithMarkdown:options:baseURL:error:")]
		NativeHandle Constructor (NSData markdown, [NullAllowed] NSAttributedStringMarkdownParsingOptions options, [NullAllowed] NSUrl baseUrl, [NullAllowed] out NSError error);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithMarkdownString:options:baseURL:error:")]
		NativeHandle Constructor (string markdownString, [NullAllowed] NSAttributedStringMarkdownParsingOptions options, [NullAllowed] NSUrl baseUrl, [NullAllowed] out NSError error);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("attributedStringByInflectingString")]
		NSAttributedString AttributedStringByInflectingString { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("boundingRectWithSize:options:")]
		CGRect BoundingRectWithSize (CGSize size, NSStringDrawingOptions options);

#if MONOMAC
		[Field ("NSTextLayoutSectionOrientation", "AppKit")]
#else
		[Field ("NSTextLayoutSectionOrientation", "UIKit")]
#endif
		NSString TextLayoutSectionOrientation { get; }

#if MONOMAC
		[Field ("NSTextLayoutSectionRange", "AppKit")]
#else
		[Field ("NSTextLayoutSectionRange", "UIKit")]
#endif
		NSString TextLayoutSectionRange { get; }

#if !XAMCORE_5_0
#if MONOMAC
		[Field ("NSTextLayoutSectionsAttribute", "AppKit")]
#else
		[Field ("NSTextLayoutSectionsAttribute", "UIKit")]
#endif
		NSString TextLayoutSectionsAttribute { get; }
#endif // !XAMCORE_5_0

		[NoiOS, NoWatch, NoTV]
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
		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		NSString TextScalingDocumentAttribute { get; }
#endif // !XAMCORE_5_0

#if !XAMCORE_5_0
#if MONOMAC
		[Field ("NSSourceTextScalingDocumentAttribute", "AppKit")]
#else
		[Field ("NSSourceTextScalingDocumentAttribute", "UIKit")]
#endif
		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		NSString SourceTextScalingDocumentAttribute { get; }
#endif // !XAMCORE_5_0

#if !XAMCORE_5_0
#if MONOMAC
		[Field ("NSCocoaVersionDocumentAttribute", "AppKit")]
#else
		[Field ("NSCocoaVersionDocumentAttribute", "UIKit")]
#endif
		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		NSString CocoaVersionDocumentAttribute { get; }
#endif // !XAMCORE_5_0
	}

	// we follow the API found in swift
	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Field ("NSMarkdownSourcePositionAttributeName")]
		MarkdownSourcePosition,

		[Field ("NSMorphologyAttributeName")]
		Morphology,

		[Field ("NSPresentationIntentAttributeName")]
		PresentationIntentAttributeName,

		[Field ("NSReplacementIndexAttributeName")]
		ReplacementIndex,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSInflectionAgreementArgumentAttributeName")]
		InflectionAgreementArgument,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSInflectionAgreementConceptAttributeName")]
		InflectionAgreementConcept,

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSInflectionReferentConceptAttributeName")]
		InflectionReferentConcept,

	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NSGrammaticalPronounType : long {
		NotSet = 0,
		Personal,
		Reflexive,
		Possessive,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NSGrammaticalDefiniteness : long {
		NotSet = 0,
		Indefinite,
		Definite,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NSGrammaticalDetermination : long {
		NotSet = 0,
		Independent,
		Dependent,
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	public enum NSGrammaticalPerson : long {
		NotSet = 0,
		First,
		Second,
		Third,
	}

	[NoWatch]
	[NoTV] // really inside WebKit
	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	delegate void NSAttributedStringCompletionHandler ([NullAllowed] NSAttributedString attributedString, [NullAllowed] NSDictionary<NSString, NSObject> attributes, [NullAllowed] NSError error);

	[BaseType (typeof (NSObject),
		   Delegates = new string [] { "WeakDelegate" },
		   Events = new Type [] { typeof (NSCacheDelegate) })]
	interface NSCache {
		[Export ("objectForKey:")]
		NSObject ObjectForKey (NSObject key);

		[Export ("setObject:forKey:")]
		void SetObjectforKey (NSObject obj, NSObject key);

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

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSCacheDelegate Delegate { get; set; }

		[Export ("totalCostLimit")]
		nuint TotalCostLimit { get; set; }

		[Export ("countLimit")]
		nuint CountLimit { get; set; }

		[Export ("evictsObjectsWithDiscardedContent")]
		bool EvictsObjectsWithDiscardedContent { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSCacheDelegate {
		[Export ("cache:willEvictObject:"), EventArgs ("NSObject")]
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
	// 'init' returns NIL - `init` now marked as NS_UNAVAILABLE
	[DisableDefaultCtor]
	interface NSCalendar : NSSecureCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithCalendarIdentifier:")]
		NativeHandle Constructor (NSString identifier);

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

#if !NET
		[Obsolete ("Use the overload with a 'NSCalendarOptions' parameter.")]
		[Wrap ("Components (unitFlags, fromDate, toDate, (NSCalendarOptions) opts)")]
		NSDateComponents Components (NSCalendarUnit unitFlags, NSDate fromDate, NSDate toDate, NSDateComponentsWrappingBehavior opts);
#endif

		[Export ("dateByAddingComponents:toDate:options:")]
		NSDate DateByAddingComponents (NSDateComponents comps, NSDate date, NSCalendarOptions opts);

#if !NET
		[Obsolete ("Use the overload with a 'NSCalendarOptions' parameter.")]
		[Wrap ("DateByAddingComponents (comps, date, (NSCalendarOptions) opts)")]
		NSDate DateByAddingComponents (NSDateComponents comps, NSDate date, NSDateComponentsWrappingBehavior opts);
#endif

		[Export ("dateFromComponents:")]
		NSDate DateFromComponents (NSDateComponents comps);

		[Field ("NSCalendarIdentifierGregorian"), Internal]
		NSString NSGregorianCalendar { get; }

		[Field ("NSCalendarIdentifierBuddhist"), Internal]
		NSString NSBuddhistCalendar { get; }

		[Field ("NSCalendarIdentifierChinese"), Internal]
		NSString NSChineseCalendar { get; }

		[Field ("NSCalendarIdentifierHebrew"), Internal]
		NSString NSHebrewCalendar { get; }

		[Field ("NSIslamicCalendar"), Internal]
		NSString NSIslamicCalendar { get; }

		[Field ("NSCalendarIdentifierIslamicCivil"), Internal]
		NSString NSIslamicCivilCalendar { get; }

		[Field ("NSCalendarIdentifierJapanese"), Internal]
		NSString NSJapaneseCalendar { get; }

		[Field ("NSCalendarIdentifierRepublicOfChina"), Internal]
		NSString NSRepublicOfChinaCalendar { get; }

		[Field ("NSCalendarIdentifierPersian"), Internal]
		NSString NSPersianCalendar { get; }

		[Field ("NSCalendarIdentifierIndian"), Internal]
		NSString NSIndianCalendar { get; }

		[Field ("NSCalendarIdentifierISO8601"), Internal]
		NSString NSISO8601Calendar { get; }

		[Field ("NSCalendarIdentifierCoptic"), Internal]
		NSString CopticCalendar { get; }

		[Field ("NSCalendarIdentifierEthiopicAmeteAlem"), Internal]
		NSString EthiopicAmeteAlemCalendar { get; }

		[Field ("NSCalendarIdentifierEthiopicAmeteMihret"), Internal]
		NSString EthiopicAmeteMihretCalendar { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSCalendarIdentifierIslamicTabular"), Internal]
		NSString IslamicTabularCalendar { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSCalendarIdentifierIslamicUmmAlQura"), Internal]
		NSString IslamicUmmAlQuraCalendar { get; }

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

		[Export ("dateByAddingUnit:value:toDate:options:")]
		[MacCatalyst (13, 1)]
		NSDate DateByAddingUnit (NSCalendarUnit unit, nint value, NSDate date, NSCalendarOptions options);

		[Export ("dateBySettingHour:minute:second:ofDate:options:")]
		[MacCatalyst (13, 1)]
		NSDate DateBySettingsHour (nint hour, nint minute, nint second, NSDate date, NSCalendarOptions options);

		[Export ("dateBySettingUnit:value:ofDate:options:")]
		[MacCatalyst (13, 1)]
		NSDate DateBySettingUnit (NSCalendarUnit unit, nint value, NSDate date, NSCalendarOptions options);

		[Export ("dateWithEra:year:month:day:hour:minute:second:nanosecond:")]
		[MacCatalyst (13, 1)]
		NSDate Date (nint era, nint year, nint month, nint date, nint hour, nint minute, nint second, nint nanosecond);

		[Export ("dateWithEra:yearForWeekOfYear:weekOfYear:weekday:hour:minute:second:nanosecond:")]
		[MacCatalyst (13, 1)]
		NSDate DateForWeekOfYear (nint era, nint year, nint week, nint weekday, nint hour, nint minute, nint second, nint nanosecond);

		[Export ("enumerateDatesStartingAfterDate:matchingComponents:options:usingBlock:")]
		[MacCatalyst (13, 1)]
		void EnumerateDatesStartingAfterDate (NSDate start, NSDateComponents matchingComponents, NSCalendarOptions options, [BlockCallback] EnumerateDatesCallback callback);

		[Export ("getEra:year:month:day:fromDate:")]
		[MacCatalyst (13, 1)]
		void GetComponentsFromDate (out nint era, out nint year, out nint month, out nint day, NSDate date);

		[Export ("getEra:yearForWeekOfYear:weekOfYear:weekday:fromDate:")]
		[MacCatalyst (13, 1)]
		void GetComponentsFromDateForWeekOfYear (out nint era, out nint year, out nint weekOfYear, out nint weekday, NSDate date);

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

		[Export ("nextDateAfterDate:matchingHour:minute:second:options:")]
		[MacCatalyst (13, 1)]
		[MarshalNativeExceptions]
		[return: NullAllowed]
		NSDate FindNextDateAfterDateMatching (NSDate date, nint hour, nint minute, nint second, NSCalendarOptions options);

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

	// Obsolete, but the only API surfaced by WebKit.WebHistory.
	[Deprecated (PlatformName.MacOSX, 10, 1, message: "Use NSCalendar and NSDateComponents.")]
	[NoiOS]
	[NoMacCatalyst]
	[NoWatch]
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
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSFormatter))]
	interface NSMassFormatter {
		[Export ("numberFormatter", ArgumentSemantic.Copy)]
		NSNumberFormatter NumberFormatter { get; set; }

		[Export ("unitStyle")]
		NSFormattingUnitStyle UnitStyle { get; set; }

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
#if MONOMAC && !NET
		void AddCharacters (string str);
#else
		void AddCharacters (NSString str);
#endif

		[Export ("removeCharactersInString:")]
#if MONOMAC && !NET
		void RemoveCharacters (string str);
#else
		void RemoveCharacters (NSString str);
#endif

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
		void EncodeRoot ([NullAllowed] NSObject obj);

		[Export ("decodeObject")]
		NSObject DecodeObject ();

		//
		// Encoding and decoding with keys
		// 
		[Export ("encodeConditionalObject:forKey:")]
		void EncodeConditionalObject ([NullAllowed] NSObject val, string key);

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

		[Export ("encodeInteger:forKey:")]
		void Encode (nint val, string key);

		[Export ("encodeBytes:length:forKey:")]
		void EncodeBlock (IntPtr bytes, nint length, string key);

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

		[Export ("decodeObjectForKey:")]
		NSObject DecodeObject (string key);

		[Export ("decodeBytesForKey:returnedLength:")]
		IntPtr DecodeBytes (string key, out nuint length);

		[Export ("decodeBytesWithReturnedLength:")]
		IntPtr DecodeBytes (out nuint length);

		[Export ("allowedClasses")]
		NSSet AllowedClasses { get; }

		[Export ("requiresSecureCoding")]
		bool RequiresSecureCoding ();

		[MacCatalyst (13, 1)]
		[Export ("decodeTopLevelObjectAndReturnError:")]
		NSObject DecodeTopLevelObject (out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("decodeTopLevelObjectForKey:error:")]
		NSObject DecodeTopLevelObject (string key, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("decodeTopLevelObjectOfClass:forKey:error:")]
		NSObject DecodeTopLevelObject (Class klass, string key, out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("decodeTopLevelObjectOfClasses:forKey:error:")]
		NSObject DecodeTopLevelObject ([NullAllowed] NSSet<Class> setOfClasses, string key, out NSError error);

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

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("decodeArrayOfObjectsOfClass:forKey:")]
		[return: NullAllowed]
		NSObject [] DecodeArrayOfObjects (Class @class, string key);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("decodeArrayOfObjectsOfClasses:forKey:")]
		[return: NullAllowed]
		NSObject [] DecodeArrayOfObjects (NSSet<Class> classes, string key);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("decodeDictionaryWithKeysOfClass:objectsOfClass:forKey:")]
		[return: NullAllowed]
		NSDictionary DecodeDictionary (Class keyClass, Class objectClass, string key);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("decodeDictionaryWithKeysOfClasses:objectsOfClasses:forKey:")]
		[return: NullAllowed]
		NSDictionary DecodeDictionary (NSSet<Class> keyClasses, NSSet<Class> objectClasses, string key);
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
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'Save (NSUrl,bool)' instead.")]
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

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("decompressedDataUsingAlgorithm:error:")]
		[return: NullAllowed]
		NSData Decompress (NSDataCompressionAlgorithm algorithm, [NullAllowed] out NSError error);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

		[Export ("leapMonth")]
		bool IsLeapMonth { [Bind ("isLeapMonth")] get; set; }

		[Export ("isValidDate")]
		[MacCatalyst (13, 1)]
		bool IsValidDate { get; }

		[Export ("isValidDateInCalendar:")]
		[MacCatalyst (13, 1)]
		bool IsValidDateInCalendar (NSCalendar calendar);

		[Export ("setValue:forComponent:")]
		[MacCatalyst (13, 1)]
		void SetValueForComponent (nint value, NSCalendarUnit unit);

		[Export ("valueForComponent:")]
		[MacCatalyst (13, 1)]
		nint GetValueForComponent (NSCalendarUnit unit);
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

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("stringForObjectValue:")]
		[return: NullAllowed]
		string GetString ([NullAllowed] NSObject obj);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("stringFromMeasurement:countStyle:")]
		string Create (NSUnitInformationStorage measurement, NSByteCountFormatterCountStyle countStyle);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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
		[Export ("NSFileHandleNotificationDataItem")]
		NSData AvailableData { get; }

		[Export ("NSFileHandleError", ArgumentSemantic.Assign)]
		nint UnixErrorCode { get; }
	}

	interface NSFileHandleConnectionAcceptedEventArgs {
		[Export ("NSFileHandleNotificationFileHandleItem")]
		NSFileHandle NearSocketConnection { get; }

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
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'ReadToEnd (out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'ReadToEnd (out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ReadToEnd (out NSError)' instead.")]
		[Export ("readDataToEndOfFile")]
		NSData ReadDataToEndOfFile ();

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("readDataToEndOfFileAndReturnError:")]
		[return: NullAllowed]
		NSData ReadToEnd ([NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Read (nuint, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Read (nuint, out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'Read (nuint, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Read (nuint, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Read (nuint, out NSError)' instead.")]
		[Export ("readDataOfLength:")]
		NSData ReadDataOfLength (nuint length);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("readDataUpToLength:error:")]
		[return: NullAllowed]
		NSData Read (nuint length, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Write (out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Write (out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'Write (out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Write (out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Write (out NSError)' instead.")]
		[Export ("writeData:")]
		void WriteData (NSData data);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("writeData:error:")]
		bool Write (NSData data, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'GetOffset (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'GetOffset (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'GetOffset (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'GetOffset (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetOffset (out ulong, out NSError)' instead.")]
		[Export ("offsetInFile")]
		ulong OffsetInFile ();

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("getOffset:error:")]
		bool GetOffset (out ulong offsetInFile, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'SeekToEnd (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'SeekToEnd (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'SeekToEnd (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'SeekToEnd (out ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SeekToEnd (out ulong, out NSError)' instead.")]
		[Export ("seekToEndOfFile")]
		ulong SeekToEndOfFile ();

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("seekToEndReturningOffset:error:")]
		bool SeekToEnd ([NullAllowed] out ulong offsetInFile, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Seek (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Seek (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'Seek (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Seek (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Seek (ulong, out NSError)' instead.")]
		[Export ("seekToFileOffset:")]
		void SeekToFileOffset (ulong offset);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("seekToOffset:error:")]
		bool Seek (ulong offset, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Truncate (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Truncate (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'Truncate (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Truncate (ulong, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Truncate (ulong, out NSError)' instead.")]
		[Export ("truncateFileAtOffset:")]
		void TruncateFileAtOffset (ulong offset);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("truncateAtOffset:error:")]
		bool Truncate (ulong offset, [NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Synchronize (out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Synchronize (out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'Synchronize (out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Synchronize (out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Synchronize (out NSError)' instead.")]
		[Export ("synchronizeFile")]
		void SynchronizeFile ();

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("synchronizeAndReturnError:")]
		bool Synchronize ([NullAllowed] out NSError error);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'Close (out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'Close (out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'Close (out NSError)' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'Close (out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'Close (out NSError)' instead.")]
		[Export ("closeFile")]
		void CloseFile ();

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

		[Wrap ("ReadInBackground (notifyRunLoopModes.GetConstants ())")]
		void ReadInBackground (NSRunLoopMode [] notifyRunLoopModes);

		[Export ("readInBackgroundAndNotify")]
		void ReadInBackground ();

		[Export ("readToEndOfFileInBackgroundAndNotifyForModes:")]
		void ReadToEndOfFileInBackground (NSString [] notifyRunLoopModes);

		[Wrap ("ReadToEndOfFileInBackground (notifyRunLoopModes.GetConstants ())")]
		void ReadToEndOfFileInBackground (NSRunLoopMode [] notifyRunLoopModes);

		[Export ("readToEndOfFileInBackgroundAndNotify")]
		void ReadToEndOfFileInBackground ();

		[Export ("acceptConnectionInBackgroundAndNotifyForModes:")]
		void AcceptConnectionInBackground (NSString [] notifyRunLoopModes);

		[Wrap ("AcceptConnectionInBackground (notifyRunLoopModes.GetConstants ())")]
		void AcceptConnectionInBackground (NSRunLoopMode [] notifyRunLoopModes);

		[Export ("acceptConnectionInBackgroundAndNotify")]
		void AcceptConnectionInBackground ();

		[Export ("waitForDataInBackgroundAndNotifyForModes:")]
		void WaitForDataInBackground (NSString [] notifyRunLoopModes);

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

		[Export ("setReadabilityHandler:")]
		void SetReadabilityHandler ([NullAllowed] Action<NSFileHandle> readCallback);

		[Export ("setWriteabilityHandler:")]
		void SetWriteabilityHandle ([NullAllowed] Action<NSFileHandle> writeCallback);

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

	[MacCatalyst (13, 1)]
	[Static]
	interface NSPersonNameComponent {
		[Field ("NSPersonNameComponentKey")]
		NSString ComponentKey { get; }

		[Field ("NSPersonNameComponentGivenName")]
		NSString GivenName { get; }

		[Field ("NSPersonNameComponentFamilyName")]
		NSString FamilyName { get; }

		[Field ("NSPersonNameComponentMiddleName")]
		NSString MiddleName { get; }

		[Field ("NSPersonNameComponentPrefix")]
		NSString Prefix { get; }

		[Field ("NSPersonNameComponentSuffix")]
		NSString Suffix { get; }

		[Field ("NSPersonNameComponentNickname")]
		NSString Nickname { get; }

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

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSCoding {
		// [Abstract]
		[Export ("initWithCoder:")]
		NativeHandle Constructor (NSCoder decoder);

		[Abstract]
		[Export ("encodeWithCoder:")]
		void EncodeTo (NSCoder encoder);
	}

	[Protocol]
	interface NSSecureCoding : NSCoding {
		// note: +supportsSecureCoding being static it is not a good "generated" binding candidate
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSCopying {
		[Abstract]
		[return: Release]
		[Export ("copyWithZone:")]
		NSObject Copy ([NullAllowed] NSZone zone);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSMutableCopying : NSCopying {
		[Abstract]
		[Export ("mutableCopyWithZone:")]
		[return: Release ()]
		NSObject MutableCopy ([NullAllowed] NSZone zone);
	}

	interface INSMutableCopying { }

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSKeyedArchiverDelegate {
		[Export ("archiver:didEncodeObject:"), EventArgs ("NSObject")]
		void EncodedObject (NSKeyedArchiver archiver, NSObject obj);

		[Export ("archiverDidFinish:")]
		void Finished (NSKeyedArchiver archiver);

		[Export ("archiver:willEncodeObject:"), DelegateName ("NSEncodeHook"), DefaultValue (null)]
		NSObject WillEncode (NSKeyedArchiver archiver, NSObject obj);

		[Export ("archiverWillFinish:")]
		void Finishing (NSKeyedArchiver archiver);

		[Export ("archiver:willReplaceObject:withObject:"), EventArgs ("NSArchiveReplace")]
		void ReplacingObject (NSKeyedArchiver archiver, NSObject oldObject, NSObject newObject);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSKeyedUnarchiverDelegate {
		[Export ("unarchiver:didDecodeObject:"), DelegateName ("NSDecoderCallback"), DefaultValue (null)]
		NSObject DecodedObject (NSKeyedUnarchiver unarchiver, NSObject obj);

		[Export ("unarchiverDidFinish:")]
		void Finished (NSKeyedUnarchiver unarchiver);

		[Export ("unarchiver:cannotDecodeObjectOfClassName:originalClasses:"), DelegateName ("NSDecoderHandler"), DefaultValue (null)]
		Class CannotDecodeClass (NSKeyedUnarchiver unarchiver, string klass, string [] classes);

		[Export ("unarchiverWillFinish:")]
		void Finishing (NSKeyedUnarchiver unarchiver);

		[Export ("unarchiver:willReplaceObject:withObject:"), EventArgs ("NSArchiveReplace")]
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
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Export ("initForWritingWithMutableData:")]
		NativeHandle Constructor (NSMutableData data);

		[MacCatalyst (13, 1)]
		[Static]
		[Export ("archivedDataWithRootObject:requiringSecureCoding:error:")]
		[return: NullAllowed]
#if NET
		NSData GetArchivedData (NSObject @object, bool requiresSecureCoding, [NullAllowed] out NSError error);
#else
		NSData ArchivedDataWithRootObject (NSObject @object, bool requiresSecureCoding, [NullAllowed] out NSError error);
#endif

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Export ("archivedDataWithRootObject:")]
		[Static]
#if NET
		NSData GetArchivedData (NSObject root);
#else
		NSData ArchivedDataWithRootObject (NSObject root);
#endif

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Export ("archiveRootObject:toFile:")]
		[Static]
		bool ArchiveRootObjectToFile (NSObject root, string file);

		[Export ("finishEncoding")]
		void FinishEncoding ();

		[Export ("outputFormat")]
		NSPropertyListFormat PropertyListFormat { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSKeyedArchiverDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("setClassName:forClass:")]
		void SetClassName (string name, Class kls);

		[Export ("classNameForClass:")]
		string GetClassName (Class kls);

		[MacCatalyst (13, 1)]
		[Field ("NSKeyedArchiveRootObjectKey")]
		NSString RootObjectKey { get; }

#if NET
		[Export ("requiresSecureCoding")]
		bool RequiresSecureCoding { get; set; }
#else
		[Export ("setRequiresSecureCoding:")]
		void SetRequiresSecureCoding (bool requireSecureEncoding);

		[Export ("requiresSecureCoding")]
		bool GetRequiresSecureCoding ();
#endif

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

		[MacCatalyst (13, 1)]
		[Static]
		[Wrap ("GetUnarchivedObject (new NSSet<Class> (Array.ConvertAll (types, t => new Class (t))), data, out error)")]
		[return: NullAllowed]
		NSObject GetUnarchivedObject (Type [] types, NSData data, [NullAllowed] out NSError error);

		[Export ("initForReadingWithData:")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[MarshalNativeExceptions]
		NativeHandle Constructor (NSData data);

		[Static, Export ("unarchiveObjectWithData:")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetUnarchivedObject ()' instead.")]
		[MarshalNativeExceptions]
		NSObject UnarchiveObject (NSData data);

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Static, Export ("unarchiveTopLevelObjectWithData:error:")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetUnarchivedObject ()' instead.")]
		// FIXME: [MarshalNativeExceptions]
		NSObject UnarchiveTopLevelObject (NSData data, out NSError error);

		[Static, Export ("unarchiveObjectWithFile:")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'GetUnarchivedObject ()' instead.")]
		[MarshalNativeExceptions]
		NSObject UnarchiveFile (string file);

		[Export ("finishDecoding")]
		void FinishDecoding ();

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSKeyedUnarchiverDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("setClass:forClassName:")]
		void SetClass (Class kls, string codedName);

		[Export ("classForClassName:")]
		[return: NullAllowed]
		Class GetClass (string codedName);

#if NET
		[Export ("requiresSecureCoding")]
		bool RequiresSecureCoding { get; set; }
#else
		[Export ("setRequiresSecureCoding:")]
		void SetRequiresSecureCoding (bool requireSecureEncoding);

		[Export ("requiresSecureCoding")]
		bool GetRequiresSecureCoding ();
#endif

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("unarchivedArrayOfObjectsOfClass:fromData:error:")]
		[return: NullAllowed]
		NSObject [] GetUnarchivedArray (Class @class, NSData data, [NullAllowed] out NSError error);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("unarchivedArrayOfObjectsOfClasses:fromData:error:")]
		[return: NullAllowed]
		NSObject [] GetUnarchivedArray (NSSet<Class> classes, NSData data, [NullAllowed] out NSError error);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("unarchivedDictionaryWithKeysOfClass:objectsOfClass:fromData:error:")]
		[return: NullAllowed]
		NSDictionary GetUnarchivedDictionary (Class keyClass, Class valueClass, NSData data, [NullAllowed] out NSError error);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
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

		[Export ("isStarted")]
		bool IsStarted { get; }

		[Export ("isGathering")]
		bool IsGathering { get; }

		[Export ("isStopped")]
		bool IsStopped { get; }

		[Export ("disableUpdates")]
		void DisableUpdates ();

		[Export ("enableUpdates")]
		void EnableUpdates ();

		[Export ("resultCount")]
		nint ResultCount { get; }

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

		[Export ("valueOfAttribute:forResultAtIndex:")]
		NSObject ValueOfAttribute (string attribyteName, nint atIndex);

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSMetadataQueryDelegate Delegate { get; set; }

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

		[Field ("NSMetadataQueryResultContentRelevanceAttribute")]
		NSString ResultContentRelevanceAttribute { get; }

		// Scope constants for defined search locations
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Field ("NSMetadataQueryUserHomeScope")]
		NSString UserHomeScope { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Field ("NSMetadataQueryLocalComputerScope")]
		NSString LocalComputerScope { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Field ("NSMetadataQueryLocalDocumentsScope")]
		NSString LocalDocumentsScope { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Field ("NSMetadataQueryNetworkScope")]
		NSString NetworkScope { get; }

		[Field ("NSMetadataQueryUbiquitousDocumentsScope")]
		NSString UbiquitousDocumentsScope { get; }

		[Field ("NSMetadataQueryUbiquitousDataScope")]
		NSString UbiquitousDataScope { get; }


		[MacCatalyst (13, 1)]
		[Field ("NSMetadataQueryAccessibleUbiquitousExternalDocumentsScope")]
		NSString AccessibleUbiquitousExternalDocumentsScope { get; }

		[Field ("NSMetadataItemFSNameKey")]
		NSString ItemFSNameKey { get; }

		[Field ("NSMetadataItemDisplayNameKey")]
		NSString ItemDisplayNameKey { get; }

		[Field ("NSMetadataItemURLKey")]
		NSString ItemURLKey { get; }

		[Field ("NSMetadataItemPathKey")]
		NSString ItemPathKey { get; }

		[Field ("NSMetadataItemFSSizeKey")]
		NSString ItemFSSizeKey { get; }

		[Field ("NSMetadataItemFSCreationDateKey")]
		NSString ItemFSCreationDateKey { get; }

		[Field ("NSMetadataItemFSContentChangeDateKey")]
		NSString ItemFSContentChangeDateKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSMetadataItemContentTypeKey")]
		NSString ContentTypeKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSMetadataItemContentTypeTreeKey")]
		NSString ContentTypeTreeKey { get; }


		[Field ("NSMetadataItemIsUbiquitousKey")]
		NSString ItemIsUbiquitousKey { get; }

		[Field ("NSMetadataUbiquitousItemHasUnresolvedConflictsKey")]
		NSString UbiquitousItemHasUnresolvedConflictsKey { get; }

		[Deprecated (PlatformName.iOS, 7, 0, message: "Use 'UbiquitousItemDownloadingStatusKey' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'UbiquitousItemDownloadingStatusKey' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message: "Use 'UbiquitousItemDownloadingStatusKey' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'UbiquitousItemDownloadingStatusKey' instead.")]
		[Field ("NSMetadataUbiquitousItemIsDownloadedKey")]
		NSString UbiquitousItemIsDownloadedKey { get; }

		[Field ("NSMetadataUbiquitousItemIsDownloadingKey")]
		NSString UbiquitousItemIsDownloadingKey { get; }

		[Field ("NSMetadataUbiquitousItemIsUploadedKey")]
		NSString UbiquitousItemIsUploadedKey { get; }

		[Field ("NSMetadataUbiquitousItemIsUploadingKey")]
		NSString UbiquitousItemIsUploadingKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemDownloadingStatusKey")]
		NSString UbiquitousItemDownloadingStatusKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemDownloadingErrorKey")]
		NSString UbiquitousItemDownloadingErrorKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemUploadingErrorKey")]
		NSString UbiquitousItemUploadingErrorKey { get; }

		[Field ("NSMetadataUbiquitousItemPercentDownloadedKey")]
		NSString UbiquitousItemPercentDownloadedKey { get; }

		[Field ("NSMetadataUbiquitousItemPercentUploadedKey")]
		NSString UbiquitousItemPercentUploadedKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemDownloadRequestedKey")]
		NSString UbiquitousItemDownloadRequestedKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemIsExternalDocumentKey")]
		NSString UbiquitousItemIsExternalDocumentKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemContainerDisplayNameKey")]
		NSString UbiquitousItemContainerDisplayNameKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemURLInLocalContainerKey")]
		NSString UbiquitousItemURLInLocalContainerKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemKeywordsKey")]
		NSString KeywordsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTitleKey")]
		NSString TitleKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAuthorsKey")]
		NSString AuthorsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemEditorsKey")]
		NSString EditorsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemParticipantsKey")]
		NSString ParticipantsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemProjectsKey")]
		NSString ProjectsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDownloadedDateKey")]
		NSString DownloadedDateKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemWhereFromsKey")]
		NSString WhereFromsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCommentKey")]
		NSString CommentKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCopyrightKey")]
		NSString CopyrightKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLastUsedDateKey")]
		NSString LastUsedDateKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemContentCreationDateKey")]
		NSString ContentCreationDateKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemContentModificationDateKey")]
		NSString ContentModificationDateKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDateAddedKey")]
		NSString DateAddedKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDurationSecondsKey")]
		NSString DurationSecondsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemContactKeywordsKey")]
		NSString ContactKeywordsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemVersionKey")]
		NSString VersionKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPixelHeightKey")]
		NSString PixelHeightKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPixelWidthKey")]
		NSString PixelWidthKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPixelCountKey")]
		NSString PixelCountKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemColorSpaceKey")]
		NSString ColorSpaceKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemBitsPerSampleKey")]
		NSString BitsPerSampleKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFlashOnOffKey")]
		NSString FlashOnOffKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFocalLengthKey")]
		NSString FocalLengthKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAcquisitionMakeKey")]
		NSString AcquisitionMakeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAcquisitionModelKey")]
		NSString AcquisitionModelKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemISOSpeedKey")]
		NSString IsoSpeedKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemOrientationKey")]
		NSString OrientationKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLayerNamesKey")]
		NSString LayerNamesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemWhiteBalanceKey")]
		NSString WhiteBalanceKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemApertureKey")]
		NSString ApertureKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemProfileNameKey")]
		NSString ProfileNameKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemResolutionWidthDPIKey")]
		NSString ResolutionWidthDpiKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemResolutionHeightDPIKey")]
		NSString ResolutionHeightDpiKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExposureModeKey")]
		NSString ExposureModeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExposureTimeSecondsKey")]
		NSString ExposureTimeSecondsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemEXIFVersionKey")]
		NSString ExifVersionKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCameraOwnerKey")]
		NSString CameraOwnerKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFocalLength35mmKey")]
		NSString FocalLength35mmKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLensModelKey")]
		NSString LensModelKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemEXIFGPSVersionKey")]
		NSString ExifGpsVersionKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAltitudeKey")]
		NSString AltitudeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLatitudeKey")]
		NSString LatitudeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLongitudeKey")]
		NSString LongitudeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemSpeedKey")]
		NSString SpeedKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTimestampKey")]
		NSString TimestampKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSTrackKey")]
		NSString GpsTrackKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemImageDirectionKey")]
		NSString ImageDirectionKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemNamedLocationKey")]
		NSString NamedLocationKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSStatusKey")]
		NSString GpsStatusKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSMeasureModeKey")]
		NSString GpsMeasureModeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDOPKey")]
		NSString GpsDopKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSMapDatumKey")]
		NSString GpsMapDatumKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDestLatitudeKey")]
		NSString GpsDestLatitudeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDestLongitudeKey")]
		NSString GpsDestLongitudeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDestBearingKey")]
		NSString GpsDestBearingKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDestDistanceKey")]
		NSString GpsDestDistanceKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSProcessingMethodKey")]
		NSString GpsProcessingMethodKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSAreaInformationKey")]
		NSString GpsAreaInformationKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDateStampKey")]
		NSString GpsDateStampKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGPSDifferentalKey")]
		NSString GpsDifferentalKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCodecsKey")]
		NSString CodecsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMediaTypesKey")]
		NSString MediaTypesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemStreamableKey")]
		NSString StreamableKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTotalBitRateKey")]
		NSString TotalBitRateKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemVideoBitRateKey")]
		NSString VideoBitRateKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudioBitRateKey")]
		NSString AudioBitRateKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDeliveryTypeKey")]
		NSString DeliveryTypeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAlbumKey")]
		NSString AlbumKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemHasAlphaChannelKey")]
		NSString HasAlphaChannelKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRedEyeOnOffKey")]
		NSString RedEyeOnOffKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMeteringModeKey")]
		NSString MeteringModeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMaxApertureKey")]
		NSString MaxApertureKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFNumberKey")]
		NSString FNumberKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExposureProgramKey")]
		NSString ExposureProgramKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExposureTimeStringKey")]
		NSString ExposureTimeStringKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemHeadlineKey")]
		NSString HeadlineKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemInstructionsKey")]
		NSString InstructionsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCityKey")]
		NSString CityKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemStateOrProvinceKey")]
		NSString StateOrProvinceKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCountryKey")]
		NSString CountryKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTextContentKey")]
		NSString TextContentKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudioSampleRateKey")]
		NSString AudioSampleRateKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudioChannelCountKey")]
		NSString AudioChannelCountKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTempoKey")]
		NSString TempoKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemKeySignatureKey")]
		NSString KeySignatureKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemTimeSignatureKey")]
		NSString TimeSignatureKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudioEncodingApplicationKey")]
		NSString AudioEncodingApplicationKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemComposerKey")]
		NSString ComposerKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLyricistKey")]
		NSString LyricistKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudioTrackNumberKey")]
		NSString AudioTrackNumberKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRecordingDateKey")]
		NSString RecordingDateKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMusicalGenreKey")]
		NSString MusicalGenreKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemIsGeneralMIDISequenceKey")]
		NSString IsGeneralMidiSequenceKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRecordingYearKey")]
		NSString RecordingYearKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemOrganizationsKey")]
		NSString OrganizationsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemLanguagesKey")]
		NSString LanguagesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRightsKey")]
		NSString RightsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPublishersKey")]
		NSString PublishersKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemContributorsKey")]
		NSString ContributorsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCoverageKey")]
		NSString CoverageKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemSubjectKey")]
		NSString SubjectKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemThemeKey")]
		NSString ThemeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDescriptionKey")]
		NSString DescriptionKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemIdentifierKey")]
		NSString IdentifierKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAudiencesKey")]
		NSString AudiencesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemNumberOfPagesKey")]
		NSString NumberOfPagesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPageWidthKey")]
		NSString PageWidthKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPageHeightKey")]
		NSString PageHeightKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemSecurityMethodKey")]
		NSString SecurityMethodKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCreatorKey")]
		NSString CreatorKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemEncodingApplicationsKey")]
		NSString EncodingApplicationsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDueDateKey")]
		NSString DueDateKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemStarRatingKey")]
		NSString StarRatingKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPhoneNumbersKey")]
		NSString PhoneNumbersKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemEmailAddressesKey")]
		NSString EmailAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemInstantMessageAddressesKey")]
		NSString InstantMessageAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemKindKey")]
		NSString KindKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRecipientsKey")]
		NSString RecipientsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFinderCommentKey")]
		NSString FinderCommentKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemFontsKey")]
		NSString FontsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAppleLoopsRootKeyKey")]
		NSString AppleLoopsRootKeyKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAppleLoopsKeyFilterTypeKey")]
		NSString AppleLoopsKeyFilterTypeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAppleLoopsLoopModeKey")]
		NSString AppleLoopsLoopModeKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAppleLoopDescriptorsKey")]
		NSString AppleLoopDescriptorsKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMusicalInstrumentCategoryKey")]
		NSString MusicalInstrumentCategoryKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemMusicalInstrumentNameKey")]
		NSString MusicalInstrumentNameKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemCFBundleIdentifierKey")]
		NSString CFBundleIdentifierKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemInformationKey")]
		NSString InformationKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemDirectorKey")]
		NSString DirectorKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemProducerKey")]
		NSString ProducerKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemGenreKey")]
		NSString GenreKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemPerformersKey")]
		NSString PerformersKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemOriginalFormatKey")]
		NSString OriginalFormatKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemOriginalSourceKey")]
		NSString OriginalSourceKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAuthorEmailAddressesKey")]
		NSString AuthorEmailAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRecipientEmailAddressesKey")]
		NSString RecipientEmailAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemAuthorAddressesKey")]
		NSString AuthorAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemRecipientAddressesKey")]
		NSString RecipientAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemIsLikelyJunkKey")]
		NSString IsLikelyJunkKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExecutableArchitecturesKey")]
		NSString ExecutableArchitecturesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemExecutablePlatformKey")]
		NSString ExecutablePlatformKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemApplicationCategoriesKey")]
		NSString ApplicationCategoriesKey { get; }

		[NoWatch, NoTV, NoiOS, NoMacCatalyst]
		[Field ("NSMetadataItemIsApplicationManagedKey")]
		NSString IsApplicationManagedKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousItemIsSharedKey")]
		NSString UbiquitousItemIsSharedKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemCurrentUserRoleKey")]
		NSString UbiquitousSharedItemCurrentUserRoleKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemCurrentUserPermissionsKey")]
		NSString UbiquitousSharedItemCurrentUserPermissionsKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemOwnerNameComponentsKey")]
		NSString UbiquitousSharedItemOwnerNameComponentsKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemMostRecentEditorNameComponentsKey")]
		NSString UbiquitousSharedItemMostRecentEditorNameComponentsKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemRoleOwner")]
		NSString UbiquitousSharedItemRoleOwner { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemRoleParticipant")]
		NSString UbiquitousSharedItemRoleParticipant { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataUbiquitousSharedItemPermissionsReadOnly")]
		NSString UbiquitousSharedItemPermissionsReadOnly { get; }

		[NoWatch, NoTV]
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
		[MacCatalyst (13, 1)]
		[Field ("NSMetadataQueryUpdateAddedItemsKey")]
		NSString QueryUpdateAddedItemsKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSMetadataQueryUpdateChangedItemsKey")]
		NSString QueryUpdateChangedItemsKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSMetadataQueryUpdateRemovedItemsKey")]
		NSString QueryUpdateRemovedItemsKey { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSMetadataQueryDelegate {
		[Export ("metadataQuery:replacementObjectForResultObject:"), DelegateName ("NSMetadataQueryObject"), DefaultValue (null)]
		NSObject ReplacementObjectForResultObject (NSMetadataQuery query, NSMetadataItem result);

		[Export ("metadataQuery:replacementValueForAttribute:value:"), DelegateName ("NSMetadataQueryValue"), DefaultValue (null)]
		NSObject ReplacementValueForAttributevalue (NSMetadataQuery query, string attributeName, NSObject value);
	}

	[BaseType (typeof (NSObject))]
#if NET
	[DisableDefaultCtor] // points to nothing so access properties crash the apps
#endif
	interface NSMetadataItem {

		[NoiOS]
		[NoTV]
		[NoWatch]
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

		[Export ("insertObject:atIndex:")]
		void Insert (NSObject obj, nint index);

		[Export ("removeLastObject")]
		void RemoveLastObject ();

		[Export ("removeObjectAtIndex:")]
		void RemoveObject (nint index);

		[Internal]
		[Sealed]
		[Export ("replaceObjectAtIndex:withObject:")]
		void _ReplaceObject (nint index, IntPtr withObject);

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

#if false // https://github.com/xamarin/xamarin-macios/issues/15577
		[Watch (6,0), TV (13,0), iOS (13,0)]
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

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Wrap ("AddAttributes (attributes.GetDictionary ()!, range)")]
		void AddAttributes (NSStringAttributes attributes, NSRange range);

		[Export ("removeAttribute:range:")]
		void RemoveAttribute (string name, NSRange range);

		[Export ("replaceCharactersInRange:withAttributedString:")]
		void Replace (NSRange range, NSAttributedString value);

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

		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'ReadFromUrl' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ReadFromUrl' instead.")]
		[Export ("readFromFileURL:options:documentAttributes:error:")]
		bool ReadFromFile (NSUrl url, NSDictionary options, ref NSDictionary returnOptions, ref NSError error);

		[NoMac]
		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'ReadFromUrl' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'ReadFromUrl' instead.")]
		[Wrap ("ReadFromFile (url, options.GetDictionary ()!, ref returnOptions, ref error)")]
		bool ReadFromFile (NSUrl url, NSAttributedStringDocumentAttributes options, ref NSDictionary returnOptions, ref NSError error);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("readFromData:options:documentAttributes:error:")]
		bool ReadFromData (NSData data, NSDictionary options, ref NSDictionary returnOptions, ref NSError error);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Wrap ("ReadFromData (data, options.GetDictionary ()!, ref returnOptions, ref error)")]
		bool ReadFromData (NSData data, NSAttributedStringDocumentAttributes options, ref NSDictionary returnOptions, ref NSError error);

		[Internal]
		[Sealed]
		[MacCatalyst (13, 1)]
		[Export ("readFromURL:options:documentAttributes:error:")]
		bool ReadFromUrl (NSUrl url, NSDictionary options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);

		[MacCatalyst (13, 1)]
		[Export ("readFromURL:options:documentAttributes:error:")]
		bool ReadFromUrl (NSUrl url, NSDictionary<NSString, NSObject> options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);

		[MacCatalyst (13, 1)]
		[Wrap ("ReadFromUrl (url, options.GetDictionary ()!, ref returnOptions, ref error)")]
		bool ReadFromUrl (NSUrl url, NSAttributedStringDocumentAttributes options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);
	}

	[BaseType (typeof (NSData))]
	interface NSMutableData {
		[Static, Export ("dataWithCapacity:")]
		[Autorelease]
		[PreSnippet ("if (capacity < 0 || capacity > nint.MaxValue) throw new ArgumentOutOfRangeException ();", Optimizable = true)]
		NSMutableData FromCapacity (nint capacity);

		[Static, Export ("dataWithLength:")]
		[Autorelease]
		[PreSnippet ("if (length < 0 || length > nint.MaxValue) throw new ArgumentOutOfRangeException ();", Optimizable = true)]
		NSMutableData FromLength (nint length);

		[Static, Export ("data")]
		[Autorelease]
		NSMutableData Create ();

		[Export ("mutableBytes")]
		IntPtr MutableBytes { get; }

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

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("decompressUsingAlgorithm:error:")]
		bool Decompress (NSDataCompressionAlgorithm algorithm, [NullAllowed] out NSError error);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("compressUsingAlgorithm:error:")]
		bool Compress (NSDataCompressionAlgorithm algorithm, [NullAllowed] out NSError error);
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

		[NoWatch, NoTV, NoMac]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Static]
		[Export ("dateWithSRAbsoluteTime:")]
		NSDate CreateFromSRAbsoluteTime (double time);

		[NoWatch, NoTV, NoMac]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("initWithSRAbsoluteTime:")]
		NativeHandle Constructor (double srAbsoluteTime);

		[NoWatch, NoTV, NoMac]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("srAbsoluteTime")]
		double SrAbsoluteTime { get; }
	}

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSDictionary : NSSecureCoding, NSMutableCopying, NSFetchRequestResult, INSFastEnumeration {
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSMutableDictionary.FromFile' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSMutableDictionary.FromFile' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NSMutableDictionary.FromFile' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSMutableDictionary.FromFile' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSMutableDictionary.FromFile' instead.")]
		[Export ("dictionaryWithContentsOfFile:")]
		[Static]
		NSDictionary FromFile (string path);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSMutableDictionary.FromUrl' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSMutableDictionary.FromUrl' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NSMutableDictionary.FromUrl' instead.")]
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
		NSDictionary FromObjectsAndKeysInternal ([NullAllowed] NSArray objects, [NullAllowed] NSArray keys);

		[Export ("initWithDictionary:")]
		NativeHandle Constructor (NSDictionary other);

		[Export ("initWithDictionary:copyItems:")]
		NativeHandle Constructor (NSDictionary other, bool copyItems);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSMutableDictionary(string)' constructor instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSMutableDictionary(string)' constructor instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NSMutableDictionary(string)' constructor instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSMutableDictionary(string)' constructor instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSMutableDictionary(string)' constructor instead.")]
		[Export ("initWithContentsOfFile:")]
		NativeHandle Constructor (string fileName);

		[Export ("initWithObjects:forKeys:"), Internal]
		NativeHandle Constructor (NSArray objects, NSArray keys);

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSMutableDictionary(NSUrl)' constructor instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSMutableDictionary(NSUrl)' constructor instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NSMutableDictionary(NSUrl)' constructor instead.")]
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
		[Deprecated (PlatformName.WatchOS, 6, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("writeToFile:atomically:")]
		bool WriteToFile (string path, bool useAuxiliaryFile);

		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.WatchOS, 6, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("writeToURL:atomically:")]
		bool WriteToUrl (NSUrl url, bool atomically);

		[Static]
		[Export ("sharedKeySetForKeys:")]
		NSObject GetSharedKeySetForKeys (NSObject [] keys);

	}

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
		[Static, Export ("errorWithDomain:code:userInfo:")]
		NSError FromDomain (NSString domain, nint code, [NullAllowed] NSDictionary userInfo);

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

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("underlyingErrors", ArgumentSemantic.Copy)]
		NSError [] UnderlyingErrors { get; }

		[Field ("NSCocoaErrorDomain")]
		NSString CocoaErrorDomain { get; }

		[Field ("NSPOSIXErrorDomain")]
		NSString PosixErrorDomain { get; }

		[Field ("NSOSStatusErrorDomain")]
		NSString OsStatusErrorDomain { get; }

		[Field ("NSMachErrorDomain")]
		NSString MachErrorDomain { get; }

		[Field ("NSURLErrorDomain")]
		NSString NSUrlErrorDomain { get; }

#if NET
		[NoWatch]
		[MacCatalyst (13, 1)]
#else
		[Obsoleted (PlatformName.WatchOS, 7, 0)]
#endif
		[Field ("NSNetServicesErrorDomain")]
		NSString NSNetServicesErrorDomain { get; }

		[NoWatch]
		[Field ("NSNetServicesErrorCode")]
		NSString NSNetServicesErrorCode { get; }

		[Field ("NSStreamSocketSSLErrorDomain")]
		NSString NSStreamSocketSSLErrorDomain { get; }

		[Field ("NSStreamSOCKSErrorDomain")]
		NSString NSStreamSOCKSErrorDomain { get; }

		[Field ("kCLErrorDomain", "CoreLocation")]
		NSString CoreLocationErrorDomain { get; }

#if !WATCH
		[Field ("kCFErrorDomainCFNetwork", "CFNetwork")]
		NSString CFNetworkErrorDomain { get; }
#endif

		[NoMac, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("CMErrorDomain", "CoreMotion")]
		NSString CoreMotionErrorDomain { get; }

#if !XAMCORE_3_0
		// now exposed with the corresponding EABluetoothAccessoryPickerError enum
		[NoMac, NoTV, NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("EABluetoothAccessoryPickerErrorDomain", "ExternalAccessory")]
		NSString EABluetoothAccessoryPickerErrorDomain { get; }

		// now exposed with the corresponding MKErrorCode enum
		[NoMac]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Field ("MKErrorDomain", "MapKit")]
		NSString MapKitErrorDomain { get; }

		// now exposed with the corresponding WKErrorCode enum
		[NoMac, NoTV]
		[Unavailable (PlatformName.iOS)]
		[MacCatalyst (13, 1)]
		[Field ("WatchKitErrorDomain", "WatchKit")]
		NSString WatchKitErrorDomain { get; }
#endif

		[Field ("NSUnderlyingErrorKey")]
		NSString UnderlyingErrorKey { get; }

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("NSMultipleUnderlyingErrorsKey")]
		NSString MultipleUnderlyingErrorsKey { get; }

		[Field ("NSLocalizedDescriptionKey")]
		NSString LocalizedDescriptionKey { get; }

		[Field ("NSLocalizedFailureReasonErrorKey")]
		NSString LocalizedFailureReasonErrorKey { get; }

		[Field ("NSLocalizedRecoverySuggestionErrorKey")]
		NSString LocalizedRecoverySuggestionErrorKey { get; }

		[Field ("NSLocalizedRecoveryOptionsErrorKey")]
		NSString LocalizedRecoveryOptionsErrorKey { get; }

		[Field ("NSRecoveryAttempterErrorKey")]
		NSString RecoveryAttempterErrorKey { get; }

		[Field ("NSHelpAnchorErrorKey")]
		NSString HelpAnchorErrorKey { get; }

		[Field ("NSStringEncodingErrorKey")]
		NSString StringEncodingErrorKey { get; }

		[Field ("NSURLErrorKey")]
		NSString UrlErrorKey { get; }

		[Field ("NSFilePathErrorKey")]
		NSString FilePathErrorKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSDebugDescriptionErrorKey")]
		NSString DebugDescriptionErrorKey { get; }

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

		[NoMacCatalyst]
		[NoTV]
		[NoWatch]
		[Static]
		[Export ("fileProviderErrorForCollisionWithItem:")]
		NSError GetFileProviderError (INSFileProviderItem existingItem);

		[NoMacCatalyst]
		[NoTV]
		[NoWatch]
		[Static]
		[Export ("fileProviderErrorForNonExistentItemWithIdentifier:")]
		NSError GetFileProviderError (string nonExistentItemIdentifier);

		[iOS (16, 0)]
		[Mac (11, 0)]
		[NoMacCatalyst]
		[NoTV]
		[NoWatch]
		[Static]
		[Export ("fileProviderErrorForRejectedDeletionOfItem:")]
		NSError GetFileProviderErrorForRejectedDeletion (INSFileProviderItem updatedVersion);

#if false
		// FIXME that value is present in the header (7.0 DP 6) files but returns NULL (i.e. unusable)
		// we're also missing other NSURLError* fields (which we should add)
		[Field ("NSURLErrorBackgroundTaskCancelledReasonKey")]
		NSString NSUrlErrorBackgroundTaskCancelledReasonKey { get; }
#endif
	}

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

#if !NET && !WATCH
	[Obsolete ("NSExpressionHandler is deprecated, please use FromFormat (string, NSObject[]) instead.")]
	delegate void NSExpressionHandler (NSObject evaluatedObject, NSExpression [] expressions, NSMutableDictionary context);
#endif
	delegate NSObject NSExpressionCallbackHandler (NSObject evaluatedObject, NSExpression [] expressions, NSMutableDictionary context);
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

#if !NET && !WATCH
		[Obsolete ("Use 'FromFormat (string, NSObject[])' instead.")]
		[Static, Export ("expressionWithFormat:argumentArray:")]
		NSExpression FromFormat (string format, NSExpression [] parameters);
#endif

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

#if !NET && !WATCH
		[Obsolete ("Use 'FromFunction (NSExpressionCallbackHandler, NSExpression[])' instead.")]
		[Static, Export ("expressionForBlock:arguments:")]
		NSExpression FromFunction (NSExpressionHandler target, [NullAllowed] NSExpression [] parameters);
#endif

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

		[Async]
		[Export ("completeRequestReturningItems:completionHandler:")]
		void CompleteRequest (NSExtensionItem [] returningItems, [NullAllowed] Action<bool> completionHandler);

		[Export ("cancelRequestWithError:")]
		void CancelRequest (NSError error);

		[Export ("openURL:completionHandler:")]
		[Async]
		void OpenUrl (NSUrl url, [NullAllowed] Action<bool> completionHandler);

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

		[Field ("NSExtensionItemAttributedTitleKey")]
		NSString AttributedTitleKey { get; }

		[Field ("NSExtensionItemAttributedContentTextKey")]
		NSString AttributedContentTextKey { get; }

		[Field ("NSExtensionItemAttachmentsKey")]
		NSString AttachmentsKey { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSNull : NSSecureCoding, NSCopying
#if !WATCH
	, CAAction
#endif
	{
		[Export ("null"), Static]
		[Internal]
		NSNull _Null { get; }
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

		[Export ("forPersonHeightUse")]
		bool ForPersonHeightUse { [Bind ("isForPersonHeightUse")] get; set; }
	}

	delegate void NSLingusticEnumerator (NSString tag, NSRange tokenRange, NSRange sentenceRange, ref bool stop);

	[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'NaturalLanguage.*' API instead.")]
	[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'NaturalLanguage.*' API instead.")]
	[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'NaturalLanguage.*' API instead.")]
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

		[Export ("orthographyAtIndex:effectiveRange:")]
		NSOrthography GetOrthography (nint charIndex, ref NSRange effectiveRange);

		[Export ("stringEditedInRange:changeInLength:")]
		void StringEditedInRange (NSRange newRange, nint delta);

		[Export ("enumerateTagsInRange:scheme:options:usingBlock:")]
		void EnumerateTagsInRange (NSRange range, NSString tagScheme, NSLinguisticTaggerOptions opts, NSLingusticEnumerator enumerator);

		[Export ("sentenceRangeForRange:")]
		NSRange GetSentenceRangeForRange (NSRange range);

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

		[MacCatalyst (13, 1)]
		[Export ("tagAtIndex:unit:scheme:tokenRange:")]
		[return: NullAllowed]
		string GetTag (nuint charIndex, NSLinguisticTaggerUnit unit, string scheme, [NullAllowed] ref NSRange tokenRange);

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

#if !NET
	[Obsolete ("Use 'NSLinguisticTagUnit' enum instead.")]
	[Static]
	interface NSLinguisticTag {
		[Field ("NSLinguisticTagSchemeTokenType")]
		NSString SchemeTokenType { get; }

		[Field ("NSLinguisticTagSchemeLexicalClass")]
		NSString SchemeLexicalClass { get; }

		[Field ("NSLinguisticTagSchemeNameType")]
		NSString SchemeNameType { get; }

		[Field ("NSLinguisticTagSchemeNameTypeOrLexicalClass")]
		NSString SchemeNameTypeOrLexicalClass { get; }

		[Field ("NSLinguisticTagSchemeLemma")]
		NSString SchemeLemma { get; }

		[Field ("NSLinguisticTagSchemeLanguage")]
		NSString SchemeLanguage { get; }

		[Field ("NSLinguisticTagSchemeScript")]
		NSString SchemeScript { get; }

		[Field ("NSLinguisticTagWord")]
		NSString Word { get; }

		[Field ("NSLinguisticTagPunctuation")]
		NSString Punctuation { get; }

		[Field ("NSLinguisticTagWhitespace")]
		NSString Whitespace { get; }

		[Field ("NSLinguisticTagOther")]
		NSString Other { get; }

		[Field ("NSLinguisticTagNoun")]
		NSString Noun { get; }

		[Field ("NSLinguisticTagVerb")]
		NSString Verb { get; }

		[Field ("NSLinguisticTagAdjective")]
		NSString Adjective { get; }

		[Field ("NSLinguisticTagAdverb")]
		NSString Adverb { get; }

		[Field ("NSLinguisticTagPronoun")]
		NSString Pronoun { get; }

		[Field ("NSLinguisticTagDeterminer")]
		NSString Determiner { get; }

		[Field ("NSLinguisticTagParticle")]
		NSString Particle { get; }

		[Field ("NSLinguisticTagPreposition")]
		NSString Preposition { get; }

		[Field ("NSLinguisticTagNumber")]
		NSString Number { get; }

		[Field ("NSLinguisticTagConjunction")]
		NSString Conjunction { get; }

		[Field ("NSLinguisticTagInterjection")]
		NSString Interjection { get; }

		[Field ("NSLinguisticTagClassifier")]
		NSString Classifier { get; }

		[Field ("NSLinguisticTagIdiom")]
		NSString Idiom { get; }

		[Field ("NSLinguisticTagOtherWord")]
		NSString OtherWord { get; }

		[Field ("NSLinguisticTagSentenceTerminator")]
		NSString SentenceTerminator { get; }

		[Field ("NSLinguisticTagOpenQuote")]
		NSString OpenQuote { get; }

		[Field ("NSLinguisticTagCloseQuote")]
		NSString CloseQuote { get; }

		[Field ("NSLinguisticTagOpenParenthesis")]
		NSString OpenParenthesis { get; }

		[Field ("NSLinguisticTagCloseParenthesis")]
		NSString CloseParenthesis { get; }

		[Field ("NSLinguisticTagWordJoiner")]
		NSString WordJoiner { get; }

		[Field ("NSLinguisticTagDash")]
		NSString Dash { get; }

		[Field ("NSLinguisticTagOtherPunctuation")]
		NSString OtherPunctuation { get; }

		[Field ("NSLinguisticTagParagraphBreak")]
		NSString ParagraphBreak { get; }

		[Field ("NSLinguisticTagOtherWhitespace")]
		NSString OtherWhitespace { get; }

		[Field ("NSLinguisticTagPersonalName")]
		NSString PersonalName { get; }

		[Field ("NSLinguisticTagPlaceName")]
		NSString PlaceName { get; }

		[Field ("NSLinguisticTagOrganizationName")]
		NSString OrganizationName { get; }
	}
#endif

	[BaseType (typeof (NSObject))]
	// 'init' returns NIL so it's not usable evenif it does not throw an ObjC exception
	// funnily it was "added" in iOS 7 and header files says "do not invoke; not a valid initializer for this class"
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

		[Export ("objectForKey:"), Internal]
		NSObject ObjectForKey (NSString key);

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

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("languageIdentifier")]
		string LanguageIdentifier { get; }

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("regionCode")]
		string RegionCode { get; }
	}

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

#if !NET
		[Obsolete ("Use 'GetMatches2' instead, this method has the wrong return type.")]
		[Export ("matchesInString:options:range:")]
		NSString [] GetMatches (NSString str, NSMatchingOptions options, NSRange range);
#endif

		[Export ("matchesInString:options:range:")]
#if NET
		NSTextCheckingResult [] GetMatches (NSString str, NSMatchingOptions options, NSRange range);
#else
		[Sealed]
		NSTextCheckingResult [] GetMatches2 (NSString str, NSMatchingOptions options, NSRange range);
#endif

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

		[Wrap ("NSRunLoopModeExtensions.GetValue (CurrentMode)")]
		NSRunLoopMode CurrentRunLoopMode { get; }

		[Export ("getCFRunLoop")]
		CFRunLoop GetCFRunLoop ();

		[Export ("addTimer:forMode:")]
		void AddTimer (NSTimer timer, NSString forMode);

		[Wrap ("AddTimer (timer, forMode.GetConstant ()!)")]
		void AddTimer (NSTimer timer, NSRunLoopMode forMode);

		[Export ("limitDateForMode:")]
		NSDate LimitDateForMode (NSString mode);

		[Wrap ("LimitDateForMode (mode.GetConstant ()!)")]
		NSDate LimitDateForMode (NSRunLoopMode mode);

		[Export ("acceptInputForMode:beforeDate:")]
		void AcceptInputForMode (NSString mode, NSDate limitDate);

		[Wrap ("AcceptInputForMode (mode.GetConstant ()!, limitDate)")]
		void AcceptInputForMode (NSRunLoopMode mode, NSDate limitDate);

		[Export ("run")]
		void Run ();

		[Export ("runUntilDate:")]
		void RunUntil (NSDate date);

		[Export ("runMode:beforeDate:")]
		bool RunUntil (NSString runLoopMode, NSDate limitdate);

		[Wrap ("RunUntil (runLoopMode.GetConstant ()!, limitDate)")]
		bool RunUntil (NSRunLoopMode runLoopMode, NSDate limitDate);

		[MacCatalyst (13, 1)]
		[Export ("performBlock:")]
		void Perform (Action block);

		[MacCatalyst (13, 1)]
		[Export ("performInModes:block:")]
		void Perform (NSString [] modes, Action block);

		[MacCatalyst (13, 1)]
		[Wrap ("Perform (modes.GetConstants ()!, block)")]
		void Perform (NSRunLoopMode [] modes, Action block);

#if !NET
		[Obsolete ("Use the 'NSRunLoopMode' enum instead.")]
		[Field ("NSDefaultRunLoopMode")]
		NSString NSDefaultRunLoopMode { get; }

		[Obsolete ("Use the 'NSRunLoopMode' enum instead.")]
		[Field ("NSRunLoopCommonModes")]
		NSString NSRunLoopCommonModes { get; }

		[Obsolete ("Use the 'NSRunLoopMode' enum instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSXpcConnection' instead.")]
		[NoiOS, NoWatch, NoTV, MacCatalyst (15, 0)]
		[Field ("NSConnectionReplyMode")]
		NSString NSRunLoopConnectionReplyMode { get; }

		[Obsolete ("Use the 'NSRunLoopMode' enum instead.")]
		[NoiOS, NoWatch, NoTV]
		[Field ("NSModalPanelRunLoopMode", "AppKit")]
		NSString NSRunLoopModalPanelMode { get; }

		[Obsolete ("Use the 'NSRunLoopMode' enum instead.")]
		[NoiOS, NoWatch, NoTV]
		[Field ("NSEventTrackingRunLoopMode", "AppKit")]
		NSString NSRunLoopEventTracking { get; }

		[Obsolete ("Use the 'NSRunLoopMode' enum instead.")]
		[NoMac]
		[NoWatch]
		[Field ("UITrackingRunLoopMode", "UIKit")]
		NSString UITrackingRunLoopMode { get; }
#endif
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

	[Category, BaseType (typeof (NSOrderedSet))]
	partial interface NSKeyValueSorting_NSOrderedSet {
		[Export ("sortedArrayUsingDescriptors:")]
		NSObject [] GetSortedArray (NSSortDescriptor [] sortDescriptors);
	}

#pragma warning disable 618
	[Category, BaseType (typeof (NSMutableArray))]
#pragma warning restore 618
	partial interface NSSortDescriptorSorting_NSMutableArray {
		[Export ("sortUsingDescriptors:")]
		void SortUsingDescriptors (NSSortDescriptor [] sortDescriptors);
	}

	[Category, BaseType (typeof (NSMutableOrderedSet))]
	partial interface NSKeyValueSorting_NSMutableOrderedSet {
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

		// Note: preserving this member allows us to re-enable the `Optimizable` binding flag
		[Preserve (Conditional = true)]
		[Export ("invalidate")]
		void Invalidate ();

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
	}

	interface NSUbiquitousKeyValueStoreChangeEventArgs {
		[Export ("NSUbiquitousKeyValueStoreChangedKeysKey")]
		string [] ChangedKeys { get; }

		[Export ("NSUbiquitousKeyValueStoreChangeReasonKey")]
		NSUbiquitousKeyValueStoreChangeReason ChangeReason { get; }
	}

	[BaseType (typeof (NSObject))]
#if WATCH
	[Advice (Constants.UnavailableOnWatchOS)]
	[DisableDefaultCtor] // "NSUbiquitousKeyValueStore is unavailable" is printed to the log.
#endif
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

		[Field ("NSUbiquitousKeyValueStoreChangeReasonKey")]
		NSString ChangeReasonKey { get; }

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

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSUserActivityDelegate Delegate { get; set; }

		[Export ("addUserInfoEntriesFromDictionary:")]
		void AddUserInfoEntries (NSDictionary otherDictionary);

		[Export ("becomeCurrent")]
		void BecomeCurrent ();

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("getContinuationStreamsWithCompletionHandler:")]
		[Async (ResultTypeName = "NSUserActivityContinuation")]
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

		[MacCatalyst (13, 1)]
		[Export ("eligibleForHandoff")]
		bool EligibleForHandoff { [Bind ("isEligibleForHandoff")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("eligibleForSearch")]
		bool EligibleForSearch { [Bind ("isEligibleForSearch")] get; set; }

		[MacCatalyst (13, 1)]
		[Export ("eligibleForPublicIndexing")]
		bool EligibleForPublicIndexing { [Bind ("isEligibleForPublicIndexing")] get; set; }

		[NoWatch]
		[NoTV]
		[MacCatalyst (13, 1)]
		[NullAllowed]
		[Export ("contentAttributeSet", ArgumentSemantic.Copy)] // From CSSearchableItemAttributeSet.h
		CSSearchableItemAttributeSet ContentAttributeSet { get; set; }

		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("referrerURL", ArgumentSemantic.Copy)]
		NSUrl ReferrerUrl { get; set; }

		// From NSUserActivity (CIBarcodeDescriptor)

		[TV (11, 3), iOS (11, 3), NoWatch]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("detectedBarcodeDescriptor", ArgumentSemantic.Copy)]
		CIBarcodeDescriptor DetectedBarcodeDescriptor { get; }

		// From NSUserActivity (CLSDeepLinks)

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[NoWatch, NoTV, Mac (11, 0), iOS (11, 4)]
		[Export ("isClassKitDeepLink")]
		bool IsClassKitDeepLink { get; }

		[Introduced (PlatformName.MacCatalyst, 14, 0)]
		[NoWatch, NoTV, Mac (11, 0), iOS (11, 4)]
		[NullAllowed, Export ("contextIdentifierPath", ArgumentSemantic.Strong)]
		string [] ContextIdentifierPath { get; }

		// From NSUserActivity (IntentsAdditions)

		[Watch (5, 0), NoTV, Mac (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("suggestedInvocationPhrase")]
		string SuggestedInvocationPhrase {
			// This _simply_ ensure that the Intents namespace (via the enum) will be present which,
			// in turns, means that the Intents.framework is loaded into memory and this makes the
			// selectors (getter and setter) work at runtime. Other selectors do not need it.
			// reference: https://github.com/xamarin/xamarin-macios/issues/4894
			[PreSnippet ("GC.KeepAlive (Intents.INCallCapabilityOptions.AudioCall); // no-op to ensure Intents.framework is loaded into memory", Optimizable = true)]
			get;
			[PreSnippet ("GC.KeepAlive (Intents.INCallCapabilityOptions.AudioCall); // no-op to ensure Intents.framework is loaded into memory", Optimizable = true)]
			set;
		}

		[Watch (5, 0), NoTV, NoMac, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Export ("eligibleForPrediction")]
		bool EligibleForPrediction { [Bind ("isEligibleForPrediction")] get; set; }

		[Watch (5, 0), NoTV, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("persistentIdentifier")]
		string PersistentIdentifier { get; set; }

		[Watch (5, 0), NoTV, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Async]
		[Export ("deleteSavedUserActivitiesWithPersistentIdentifiers:completionHandler:")]
		void DeleteSavedUserActivities (string [] persistentIdentifiers, Action handler);

		[Watch (5, 0), NoTV, iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Async]
		[Export ("deleteAllSavedUserActivitiesWithCompletionHandler:")]
		void DeleteAllSavedUserActivities (Action handler);

		// Inlined from NSUserActivity (UISceneActivationConditions)

		[iOS (13, 0), TV (13, 0), Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("targetContentIdentifier")]
		string TargetContentIdentifier { get; set; }

#if HAS_APPCLIP
		// Inlined from NSUserActivity (AppClip)
		[iOS (14,0)][NoTV][NoMac][NoWatch]
		[MacCatalyst (14, 0)]
		[Export ("appClipActivationPayload", ArgumentSemantic.Strong)]
		[NullAllowed]
		APActivationPayload AppClipActivationPayload { get; }
#endif
	}

	[MacCatalyst (13, 1)]
	[Static]
	partial interface NSUserActivityType {
		[Field ("NSUserActivityTypeBrowsingWeb")]
		NSString BrowsingWeb { get; }
	}

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	partial interface NSUserActivityDelegate {
		[Export ("userActivityWillSave:")]
		void UserActivityWillSave (NSUserActivity userActivity);

		[Export ("userActivityWasContinued:")]
		void UserActivityWasContinued (NSUserActivity userActivity);

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
		IntPtr InitWithUserName (string username);

		[Internal]
		[MacCatalyst (13, 1)]
		[Export ("initWithSuiteName:")]
		IntPtr InitWithSuiteName ([NullAllowed] string suiteName);

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

		[Field ("NSGlobalDomain")]
		NSString GlobalDomain { get; }

		[Field ("NSArgumentDomain")]
		NSString ArgumentDomain { get; }

		[Field ("NSRegistrationDomain")]
		NSString RegistrationDomain { get; }

		[NoMac]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSUserDefaultsSizeLimitExceededNotification")]
		NSString SizeLimitExceededNotification { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSUbiquitousUserDefaultsNoCloudAccountNotification")]
		NSString NoCloudAccountNotification { get; }

		[NoMac]
		[NoTV]
		[MacCatalyst (13, 1)]
		[Notification]
		[Field ("NSUbiquitousUserDefaultsDidChangeAccountsNotification")]
		NSString DidChangeAccountsNotification { get; }

		[NoMac]
		[NoTV]
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
		[Deprecated (PlatformName.WatchOS, 2, 0, message: "Use 'NSUrlComponents' instead.")]
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

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

		[Export ("isFileURL")]
		bool IsFileUrl { get; }

		[Export ("isFileReferenceURL")]
		bool IsFileReferenceUrl { get; }

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Always return 'null'. Use and parse 'Path' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Always return 'null'. Use and parse 'Path' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Always return 'null'. Use and parse 'Path' instead.")]
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
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("URLFromPasteboard:")]
		[Static]
		[return: NullAllowed]
		NSUrl FromPasteboard (NSPasteboard pasteboard);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
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

		[Field ("NSURLNameKey")]
		NSString NameKey { get; }

		[Field ("NSURLLocalizedNameKey")]
		NSString LocalizedNameKey { get; }

		[Field ("NSURLIsRegularFileKey")]
		NSString IsRegularFileKey { get; }

		[Field ("NSURLIsDirectoryKey")]
		NSString IsDirectoryKey { get; }

		[Field ("NSURLIsSymbolicLinkKey")]
		NSString IsSymbolicLinkKey { get; }

		[Field ("NSURLIsVolumeKey")]
		NSString IsVolumeKey { get; }

		[Field ("NSURLIsPackageKey")]
		NSString IsPackageKey { get; }

		[Field ("NSURLIsSystemImmutableKey")]
		NSString IsSystemImmutableKey { get; }

		[Field ("NSURLIsUserImmutableKey")]
		NSString IsUserImmutableKey { get; }

		[Field ("NSURLIsHiddenKey")]
		NSString IsHiddenKey { get; }

		[Field ("NSURLHasHiddenExtensionKey")]
		NSString HasHiddenExtensionKey { get; }

		[Field ("NSURLCreationDateKey")]
		NSString CreationDateKey { get; }

		[Field ("NSURLContentAccessDateKey")]
		NSString ContentAccessDateKey { get; }

		[Field ("NSURLContentModificationDateKey")]
		NSString ContentModificationDateKey { get; }

		[Field ("NSURLAttributeModificationDateKey")]
		NSString AttributeModificationDateKey { get; }

		[Field ("NSURLLinkCountKey")]
		NSString LinkCountKey { get; }

		[Field ("NSURLParentDirectoryURLKey")]
		NSString ParentDirectoryURLKey { get; }

		[Field ("NSURLVolumeURLKey")]
		NSString VolumeURLKey { get; }

		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'ContentTypeKey' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'ContentTypeKey' instead.")]
		[Deprecated (PlatformName.WatchOS, 7, 0, message: "Use 'ContentTypeKey' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'ContentTypeKey' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'ContentTypeKey' instead.")]
		[Field ("NSURLTypeIdentifierKey")]
		NSString TypeIdentifierKey { get; }

		[Field ("NSURLLocalizedTypeDescriptionKey")]
		NSString LocalizedTypeDescriptionKey { get; }

		[Field ("NSURLLabelNumberKey")]
		NSString LabelNumberKey { get; }

		[Field ("NSURLLabelColorKey")]
		NSString LabelColorKey { get; }

		[Field ("NSURLLocalizedLabelKey")]
		NSString LocalizedLabelKey { get; }

		[Field ("NSURLEffectiveIconKey")]
		NSString EffectiveIconKey { get; }

		[Field ("NSURLCustomIconKey")]
		NSString CustomIconKey { get; }

		[Field ("NSURLFileSizeKey")]
		NSString FileSizeKey { get; }

		[Field ("NSURLFileAllocatedSizeKey")]
		NSString FileAllocatedSizeKey { get; }

		[Field ("NSURLIsAliasFileKey")]
		NSString IsAliasFileKey { get; }

		[Field ("NSURLVolumeLocalizedFormatDescriptionKey")]
		NSString VolumeLocalizedFormatDescriptionKey { get; }

		[Field ("NSURLVolumeTotalCapacityKey")]
		NSString VolumeTotalCapacityKey { get; }

		[Field ("NSURLVolumeAvailableCapacityKey")]
		NSString VolumeAvailableCapacityKey { get; }

		[Field ("NSURLVolumeResourceCountKey")]
		NSString VolumeResourceCountKey { get; }

		[Field ("NSURLVolumeSupportsPersistentIDsKey")]
		NSString VolumeSupportsPersistentIDsKey { get; }

		[Field ("NSURLVolumeSupportsSymbolicLinksKey")]
		NSString VolumeSupportsSymbolicLinksKey { get; }

		[Field ("NSURLVolumeSupportsHardLinksKey")]
		NSString VolumeSupportsHardLinksKey { get; }

		[Field ("NSURLVolumeSupportsJournalingKey")]
		NSString VolumeSupportsJournalingKey { get; }

		[Field ("NSURLVolumeIsJournalingKey")]
		NSString VolumeIsJournalingKey { get; }

		[Field ("NSURLVolumeSupportsSparseFilesKey")]
		NSString VolumeSupportsSparseFilesKey { get; }

		[Field ("NSURLVolumeSupportsZeroRunsKey")]
		NSString VolumeSupportsZeroRunsKey { get; }

		[Field ("NSURLVolumeSupportsCaseSensitiveNamesKey")]
		NSString VolumeSupportsCaseSensitiveNamesKey { get; }

		[Field ("NSURLVolumeSupportsCasePreservedNamesKey")]
		NSString VolumeSupportsCasePreservedNamesKey { get; }

		// 5.0 Additions
		[Field ("NSURLKeysOfUnsetValuesKey")]
		NSString KeysOfUnsetValuesKey { get; }

		[Field ("NSURLFileResourceIdentifierKey")]
		NSString FileResourceIdentifierKey { get; }

		[Field ("NSURLVolumeIdentifierKey")]
		NSString VolumeIdentifierKey { get; }

		[Field ("NSURLPreferredIOBlockSizeKey")]
		NSString PreferredIOBlockSizeKey { get; }

		[Field ("NSURLIsReadableKey")]
		NSString IsReadableKey { get; }

		[Field ("NSURLIsWritableKey")]
		NSString IsWritableKey { get; }

		[Field ("NSURLIsExecutableKey")]
		NSString IsExecutableKey { get; }

		[Field ("NSURLIsMountTriggerKey")]
		NSString IsMountTriggerKey { get; }

		[Field ("NSURLFileSecurityKey")]
		NSString FileSecurityKey { get; }

		[Field ("NSURLFileResourceTypeKey")]
		NSString FileResourceTypeKey { get; }

		[Watch (9, 4), TV (16, 4), Mac (13, 3), iOS (16, 4)]
		[MacCatalyst (16, 4)]
		[Field ("NSURLFileIdentifierKey")]
		NSString FileIdentifierKey { get; }

		[Field ("NSURLFileResourceTypeNamedPipe")]
		NSString FileResourceTypeNamedPipe { get; }

		[Field ("NSURLFileResourceTypeCharacterSpecial")]
		NSString FileResourceTypeCharacterSpecial { get; }

		[Field ("NSURLFileResourceTypeDirectory")]
		NSString FileResourceTypeDirectory { get; }

		[Field ("NSURLFileResourceTypeBlockSpecial")]
		NSString FileResourceTypeBlockSpecial { get; }

		[Field ("NSURLFileResourceTypeRegular")]
		NSString FileResourceTypeRegular { get; }

		[Field ("NSURLFileResourceTypeSymbolicLink")]
		NSString FileResourceTypeSymbolicLink { get; }

		[Field ("NSURLFileResourceTypeSocket")]
		NSString FileResourceTypeSocket { get; }

		[Field ("NSURLFileResourceTypeUnknown")]
		NSString FileResourceTypeUnknown { get; }

		[Field ("NSURLTotalFileSizeKey")]
		NSString TotalFileSizeKey { get; }

		[Field ("NSURLTotalFileAllocatedSizeKey")]
		NSString TotalFileAllocatedSizeKey { get; }

		[Field ("NSURLVolumeSupportsRootDirectoryDatesKey")]
		NSString VolumeSupportsRootDirectoryDatesKey { get; }

		[Field ("NSURLVolumeSupportsVolumeSizesKey")]
		NSString VolumeSupportsVolumeSizesKey { get; }

		[Field ("NSURLVolumeSupportsRenamingKey")]
		NSString VolumeSupportsRenamingKey { get; }

		[Field ("NSURLVolumeSupportsAdvisoryFileLockingKey")]
		NSString VolumeSupportsAdvisoryFileLockingKey { get; }

		[Field ("NSURLVolumeSupportsExtendedSecurityKey")]
		NSString VolumeSupportsExtendedSecurityKey { get; }

		[Field ("NSURLVolumeIsBrowsableKey")]
		NSString VolumeIsBrowsableKey { get; }

		[Field ("NSURLVolumeMaximumFileSizeKey")]
		NSString VolumeMaximumFileSizeKey { get; }

		[Field ("NSURLVolumeIsEjectableKey")]
		NSString VolumeIsEjectableKey { get; }

		[Field ("NSURLVolumeIsRemovableKey")]
		NSString VolumeIsRemovableKey { get; }

		[Field ("NSURLVolumeIsInternalKey")]
		NSString VolumeIsInternalKey { get; }

		[Field ("NSURLVolumeIsAutomountedKey")]
		NSString VolumeIsAutomountedKey { get; }

		[Field ("NSURLVolumeIsLocalKey")]
		NSString VolumeIsLocalKey { get; }

		[Field ("NSURLVolumeIsReadOnlyKey")]
		NSString VolumeIsReadOnlyKey { get; }

		[Field ("NSURLVolumeCreationDateKey")]
		NSString VolumeCreationDateKey { get; }

		[Field ("NSURLVolumeURLForRemountingKey")]
		NSString VolumeURLForRemountingKey { get; }

		[Field ("NSURLVolumeUUIDStringKey")]
		NSString VolumeUUIDStringKey { get; }

		[Field ("NSURLVolumeNameKey")]
		NSString VolumeNameKey { get; }

		[Field ("NSURLVolumeLocalizedNameKey")]
		NSString VolumeLocalizedNameKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeIsEncryptedKey")]
		NSString VolumeIsEncryptedKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeIsRootFileSystemKey")]
		NSString VolumeIsRootFileSystemKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsCompressionKey")]
		NSString VolumeSupportsCompressionKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsFileCloningKey")]
		NSString VolumeSupportsFileCloningKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsSwapRenamingKey")]
		NSString VolumeSupportsSwapRenamingKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsExclusiveRenamingKey")]
		NSString VolumeSupportsExclusiveRenamingKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsImmutableFilesKey")]
		NSString VolumeSupportsImmutableFilesKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeSupportsAccessPermissionsKey")]
		NSString VolumeSupportsAccessPermissionsKey { get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLVolumeSupportsFileProtectionKey")]
		NSString VolumeSupportsFileProtectionKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeAvailableCapacityForImportantUsageKey")]
		NSString VolumeAvailableCapacityForImportantUsageKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLVolumeAvailableCapacityForOpportunisticUsageKey")]
		NSString VolumeAvailableCapacityForOpportunisticUsageKey { get; }

		[Watch (9, 4), TV (16, 4), Mac (13, 3), iOS (16, 4)]
		[MacCatalyst (16, 4)]
		[Field ("NSURLVolumeTypeNameKey")]
		NSString VolumeTypeNameKey { get; }

		[Watch (9, 4), TV (16, 4), Mac (13, 3), iOS (16, 4)]
		[MacCatalyst (16, 4)]
		[Field ("NSURLVolumeSubtypeKey")]
		NSString VolumeSubtypeKey { get; }

		[Watch (9, 4), TV (16, 4), Mac (13, 3), iOS (16, 4)]
		[MacCatalyst (16, 4)]
		[Field ("NSURLVolumeMountFromLocationKey")]
		NSString VolumeMountFromLocationKey { get; }

		[Field ("NSURLIsUbiquitousItemKey")]
		NSString IsUbiquitousItemKey { get; }

		[Field ("NSURLUbiquitousItemHasUnresolvedConflictsKey")]
		NSString UbiquitousItemHasUnresolvedConflictsKey { get; }

		[Field ("NSURLUbiquitousItemIsDownloadedKey")]
		NSString UbiquitousItemIsDownloadedKey { get; }

		[Field ("NSURLUbiquitousItemIsDownloadingKey")]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		NSString UbiquitousItemIsDownloadingKey { get; }

		[Field ("NSURLUbiquitousItemIsUploadedKey")]
		NSString UbiquitousItemIsUploadedKey { get; }

		[Field ("NSURLUbiquitousItemIsUploadingKey")]
		NSString UbiquitousItemIsUploadingKey { get; }

		[Field ("NSURLUbiquitousItemPercentDownloadedKey")]
		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'NSMetadataQuery.UbiquitousItemPercentDownloadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSMetadataQuery.UbiquitousItemPercentDownloadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Use 'NSMetadataQuery.UbiquitousItemPercentDownloadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSMetadataQuery.UbiquitousItemPercentDownloadedKey' on 'NSMetadataItem' instead.")]
		NSString UbiquitousItemPercentDownloadedKey { get; }

		[Deprecated (PlatformName.iOS, 6, 0, message: "Use 'NSMetadataQuery.UbiquitousItemPercentUploadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSMetadataQuery.UbiquitousItemPercentUploadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 8, message: "Use 'NSMetadataQuery.UbiquitousItemPercentUploadedKey' on 'NSMetadataItem' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSMetadataQuery.UbiquitousItemPercentUploadedKey' on 'NSMetadataItem' instead.")]
		[Field ("NSURLUbiquitousItemPercentUploadedKey")]
		NSString UbiquitousItemPercentUploadedKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemIsSharedKey")]
		NSString UbiquitousItemIsSharedKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemCurrentUserRoleKey")]
		NSString UbiquitousSharedItemCurrentUserRoleKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemCurrentUserPermissionsKey")]
		NSString UbiquitousSharedItemCurrentUserPermissionsKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemOwnerNameComponentsKey")]
		NSString UbiquitousSharedItemOwnerNameComponentsKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemMostRecentEditorNameComponentsKey")]
		NSString UbiquitousSharedItemMostRecentEditorNameComponentsKey { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemRoleOwner")]
		NSString UbiquitousSharedItemRoleOwner { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemRoleParticipant")]
		NSString UbiquitousSharedItemRoleParticipant { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemPermissionsReadOnly")]
		NSString UbiquitousSharedItemPermissionsReadOnly { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousSharedItemPermissionsReadWrite")]
		NSString UbiquitousSharedItemPermissionsReadWrite { get; }

		[Field ("NSURLIsExcludedFromBackupKey")]
		NSString IsExcludedFromBackupKey { get; }

		[Export ("bookmarkDataWithOptions:includingResourceValuesForKeys:relativeToURL:error:")]
		NSData CreateBookmarkData (NSUrlBookmarkCreationOptions options, [NullAllowed] string [] resourceValues, [NullAllowed] NSUrl relativeUrl, out NSError error);

		[Export ("initByResolvingBookmarkData:options:relativeToURL:bookmarkDataIsStale:error:")]
		NativeHandle Constructor (NSData bookmarkData, NSUrlBookmarkResolutionOptions resolutionOptions, [NullAllowed] NSUrl relativeUrl, out bool bookmarkIsStale, out NSError error);

		[Field ("NSURLPathKey")]
		NSString PathKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemDownloadingStatusKey")]
		NSString UbiquitousItemDownloadingStatusKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemDownloadingErrorKey")]
		NSString UbiquitousItemDownloadingErrorKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemUploadingErrorKey")]
		NSString UbiquitousItemUploadingErrorKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemDownloadingStatusNotDownloaded")]
		NSString UbiquitousItemDownloadingStatusNotDownloaded { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemDownloadingStatusDownloaded")]
		NSString UbiquitousItemDownloadingStatusDownloaded { get; }

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

		[MacCatalyst (13, 1)]
		[Field ("NSURLAddedToDirectoryDateKey")]
		NSString AddedToDirectoryDateKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLDocumentIdentifierKey")]
		NSString DocumentIdentifierKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLGenerationIdentifierKey")]
		NSString GenerationIdentifierKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLThumbnailDictionaryKey")]
		NSString ThumbnailDictionaryKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLUbiquitousItemContainerDisplayNameKey")]
		NSString UbiquitousItemContainerDisplayNameKey { get; }

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("NSURLUbiquitousItemIsExcludedFromSyncKey")]
		NSString UbiquitousItemIsExcludedFromSyncKey { get; }

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

		[MacCatalyst (13, 1)]
		[Field ("NSURLIsApplicationKey")]
		NSString IsApplicationKey { get; }

		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSURLFileProtectionKey")]
		NSString FileProtectionKey { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLFileProtectionNone")]
		NSString FileProtectionNone { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLFileProtectionComplete")]
		NSString FileProtectionComplete { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLFileProtectionCompleteUnlessOpen")]
		NSString FileProtectionCompleteUnlessOpen { get; }

		[MacCatalyst (13, 1)]
		[Field ("NSURLFileProtectionCompleteUntilFirstUserAuthentication")]
		NSString FileProtectionCompleteUntilFirstUserAuthentication { get; }

		[Watch (10, 0), TV (17, 0), NoMac, iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSURLFileProtectionCompleteWhenUserInactive")]
		NSString FileProtectionCompleteWhenUserInactive { get; }

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSURLDirectoryEntryCountKey")]
		NSString DirectoryEntryCountKey { get; }

		[Watch (7, 0)]
		[TV (14, 0)]
		[Mac (11, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLContentTypeKey")]
		NSString ContentTypeKey { get; }

		[Watch (7, 0)]
		[TV (14, 0)]
		[Mac (11, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLFileContentIdentifierKey")]
		NSString FileContentIdentifierKey { get; }

		[Watch (7, 0)]
		[TV (14, 0)]
		[Mac (11, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLIsPurgeableKey")]
		NSString IsPurgeableKey { get; }

		[Watch (7, 0)]
		[TV (14, 0)]
		[Mac (11, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLIsSparseKey")]
		NSString IsSparseKey { get; }

		[Watch (7, 0)]
		[TV (14, 0)]
		[Mac (11, 0)]
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Field ("NSURLMayHaveExtendedAttributesKey")]
		NSString MayHaveExtendedAttributesKey { get; }

		[Watch (7, 0)]
		[TV (14, 0)]
		[Mac (11, 0)]
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
	[Category, BaseType (typeof (NSUrl))]
	partial interface NSUrl_PromisedItems {
		[MacCatalyst (13, 1)]
		[Export ("checkPromisedItemIsReachableAndReturnError:")]
		bool CheckPromisedItemIsReachable (out NSError error);

		[MacCatalyst (13, 1)]
		[Export ("getPromisedItemResourceValue:forKey:error:")]
		bool GetPromisedItemResourceValue (out NSObject value, NSString key, out NSError error);

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

	[Category, BaseType (typeof (NSCharacterSet))]
	partial interface NSUrlUtilities_NSCharacterSet {
		[Static, Export ("URLUserAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlUserAllowedCharacterSet { get; }

		[Static, Export ("URLPasswordAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlPasswordAllowedCharacterSet { get; }

		[Static, Export ("URLHostAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlHostAllowedCharacterSet { get; }

		[Static, Export ("URLPathAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlPathAllowedCharacterSet { get; }

		[Static, Export ("URLQueryAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlQueryAllowedCharacterSet { get; }

		[Static, Export ("URLFragmentAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlFragmentAllowedCharacterSet { get; }
	}

	[BaseType (typeof (NSObject), Name = "NSURLCache")]
	interface NSUrlCache {
		[Export ("sharedURLCache", ArgumentSemantic.Strong), Static]
		NSUrlCache SharedCache { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use the overload that accepts an 'NSUrl' parameter instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use the overload that accepts an 'NSUrl' parameter instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use the overload that accepts an 'NSUrl' parameter instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use the overload that accepts an 'NSUrl' parameter instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use the overload that accepts an 'NSUrl' parameter instead.")]
		[Export ("initWithMemoryCapacity:diskCapacity:diskPath:")]
		NativeHandle Constructor (nuint memoryCapacity, nuint diskCapacity, [NullAllowed] string diskPath);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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
		[Async]
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

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0)]
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

		[Export ("sender")]
		NSUrlConnection Sender { get; }
	}

	[Protocol (Name = "NSURLAuthenticationChallengeSender")]
#if NET
	interface NSUrlAuthenticationChallengeSender {
#else
	[Model]
	[BaseType (typeof (NSObject))]
	interface NSURLAuthenticationChallengeSender {
#endif
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


	delegate void NSUrlConnectionDataResponse (NSUrlResponse response, NSData data, NSError error);

	[BaseType (typeof (NSObject), Name = "NSURLConnection")]
	interface NSUrlConnection :
#if NET
		NSUrlAuthenticationChallengeSender
#else
		NSURLAuthenticationChallengeSender
#endif
	{
		[Export ("canHandleRequest:")]
		[Static]
		bool CanHandleRequest (NSUrlRequest request);

		[return: NullAllowed]
		[NoWatch]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSUrlSession' instead.")]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSUrlSession' instead.")]
		[Export ("connectionWithRequest:delegate:")]
		[Static]
		NSUrlConnection FromRequest (NSUrlRequest request, [NullAllowed, Protocolize] NSUrlConnectionDelegate connectionDelegate);

		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSUrlSession' instead.")]
		[Export ("initWithRequest:delegate:")]
		NativeHandle Constructor (NSUrlRequest request, [NullAllowed, Protocolize] NSUrlConnectionDelegate connectionDelegate);

		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSUrlSession' instead.")]
		[Export ("initWithRequest:delegate:startImmediately:")]
		NativeHandle Constructor (NSUrlRequest request, [NullAllowed, Protocolize] NSUrlConnectionDelegate connectionDelegate, bool startImmediately);

		[Export ("start")]
		void Start ();

		[Export ("cancel")]
		void Cancel ();

		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, NSString forMode);

		[Wrap ("Schedule (aRunLoop, forMode.GetConstant ()!)")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

		[Export ("unscheduleFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, NSString forMode);

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
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSUrlSession' instead.")]
		[Static]
		[Export ("sendSynchronousRequest:returningResponse:error:")]
		[return: NullAllowed]
		NSData SendSynchronousRequest (NSUrlRequest request, out NSUrlResponse response, out NSError error);

		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'NSUrlSession.CreateDataTask' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'NSUrlSession.CreateDataTask' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 11, message: "Use 'NSUrlSession.CreateDataTask' instead.")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSUrlSession.CreateDataTask' instead.")]
		[Static]
		[Export ("sendAsynchronousRequest:queue:completionHandler:")]
		[Async (ResultTypeName = "NSUrlAsyncResult", MethodName = "SendRequestAsync")]
		void SendAsynchronousRequest (NSUrlRequest request, NSOperationQueue queue, NSUrlConnectionDataResponse completionHandler);
	}

	[BaseType (typeof (NSObject), Name = "NSURLConnectionDelegate")]
	[Model]
	[Protocol]
	interface NSUrlConnectionDelegate {
		[Export ("connection:canAuthenticateAgainstProtectionSpace:")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		bool CanAuthenticateAgainstProtectionSpace (NSUrlConnection connection, NSUrlProtectionSpace protectionSpace);

		[Export ("connection:didReceiveAuthenticationChallenge:")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		void ReceivedAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);

		[Export ("connection:didCancelAuthenticationChallenge:")]
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.TvOS, 9, 0, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		void CanceledAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);

		[Export ("connectionShouldUseCredentialStorage:")]
		bool ConnectionShouldUseCredentialStorage (NSUrlConnection connection);

		[Export ("connection:didFailWithError:")]
		void FailedWithError (NSUrlConnection connection, NSError error);

		[Export ("connection:willSendRequestForAuthenticationChallenge:")]
		void WillSendRequestForAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);
	}

	[BaseType (typeof (NSUrlConnectionDelegate), Name = "NSURLConnectionDataDelegate")]
	[Protocol, Model]
	interface NSUrlConnectionDataDelegate {

		[Export ("connection:willSendRequest:redirectResponse:")]
		NSUrlRequest WillSendRequest (NSUrlConnection connection, NSUrlRequest request, NSUrlResponse response);

		[Export ("connection:didReceiveResponse:")]
		void ReceivedResponse (NSUrlConnection connection, NSUrlResponse response);

		[Export ("connection:didReceiveData:")]
		void ReceivedData (NSUrlConnection connection, NSData data);

		[Export ("connection:needNewBodyStream:")]
		NSInputStream NeedNewBodyStream (NSUrlConnection connection, NSUrlRequest request);

		[Export ("connection:didSendBodyData:totalBytesWritten:totalBytesExpectedToWrite:")]
		void SentBodyData (NSUrlConnection connection, nint bytesWritten, nint totalBytesWritten, nint totalBytesExpectedToWrite);

		[Export ("connection:willCacheResponse:")]
		NSCachedUrlResponse WillCacheResponse (NSUrlConnection connection, NSCachedUrlResponse cachedResponse);

		[Export ("connectionDidFinishLoading:")]
		void FinishedLoading (NSUrlConnection connection);
	}

	[BaseType (typeof (NSUrlConnectionDelegate), Name = "NSURLConnectionDownloadDelegate")]
	[Model]
	[Protocol]
	interface NSUrlConnectionDownloadDelegate {
		[Export ("connection:didWriteData:totalBytesWritten:expectedTotalBytes:")]
		void WroteData (NSUrlConnection connection, long bytesWritten, long totalBytesWritten, long expectedTotalBytes);

		[Export ("connectionDidResumeDownloading:totalBytesWritten:expectedTotalBytes:")]
		void ResumedDownloading (NSUrlConnection connection, long totalBytesWritten, long expectedTotalBytes);

		[Abstract]
		[Export ("connectionDidFinishDownloading:destinationURL:")]
		void FinishedDownloading (NSUrlConnection connection, NSUrl destinationUrl);
	}

	[BaseType (typeof (NSObject), Name = "NSURLCredential")]
	// crash when calling NSObjecg.get_Description (and likely other selectors)
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
		NSUrlCredential FromUserPasswordPersistance (string user, string password, NSUrlCredentialPersistence persistence);

		[Export ("user")]
		string User { get; }

		[Export ("password")]
		string Password { get; }

		[Export ("hasPassword")]
		bool HasPassword { get; }

		[Export ("initWithIdentity:certificates:persistence:")]
		[Internal]
		NativeHandle Constructor (IntPtr identity, IntPtr certificates, NSUrlCredentialPersistence persistance);

		[Static]
		[Internal]
		[Export ("credentialWithIdentity:certificates:persistence:")]
		NSUrlCredential FromIdentityCertificatesPersistanceInternal (IntPtr identity, IntPtr certificates, NSUrlCredentialPersistence persistence);

		[Internal]
		[Export ("identity")]
		IntPtr Identity { get; }

		[Export ("certificates")]
		SecCertificate [] Certificates { get; }

		// bound manually to keep the managed/native signatures identical
		//[Export ("initWithTrust:")]
		//NativeHandle Constructor (IntPtr SecTrustRef_trust, bool ignored);

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

		[MacCatalyst (13, 1)]
		[Field ("NSURLCredentialStorageRemoveSynchronizableCredentials")]
		NSString RemoveSynchronizableCredentials { get; }

		[Field ("NSURLCredentialStorageChangedNotification")]
		[Notification]
		NSString ChangedNotification { get; }

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("getCredentialsForProtectionSpace:task:completionHandler:")]
		void GetCredentials (NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task, [NullAllowed] Action<NSDictionary> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("setCredential:forProtectionSpace:task:")]
		void SetCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task);

		[MacCatalyst (13, 1)]
		[Export ("removeCredential:forProtectionSpace:options:task:")]
		void RemoveCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSDictionary options, NSUrlSessionTask task);

		[MacCatalyst (13, 1)]
		[Async]
		[Export ("getDefaultCredentialForProtectionSpace:task:completionHandler:")]
		void GetDefaultCredential (NSUrlProtectionSpace space, NSUrlSessionTask task, [NullAllowed] Action<NSUrlCredential> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("setDefaultCredential:forProtectionSpace:task:")]
		void SetDefaultCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task);

	}

#if NET
	delegate void NSUrlSessionPendingTasks (NSUrlSessionTask [] dataTasks, NSUrlSessionTask [] uploadTasks, NSUrlSessionTask[] downloadTasks);
#elif XAMCORE_3_0
	delegate void NSUrlSessionPendingTasks2 (NSUrlSessionTask [] dataTasks, NSUrlSessionTask [] uploadTasks, NSUrlSessionTask[] downloadTasks);
#else
	delegate void NSUrlSessionPendingTasks (NSUrlSessionDataTask [] dataTasks, NSUrlSessionUploadTask [] uploadTasks, NSUrlSessionDownloadTask [] downloadTasks);
	delegate void NSUrlSessionPendingTasks2 (NSUrlSessionTask [] dataTasks, NSUrlSessionTask [] uploadTasks, NSUrlSessionTask [] downloadTasks);
#endif
	delegate void NSUrlSessionAllPendingTasks (NSUrlSessionTask [] tasks);
	delegate void NSUrlSessionResponse (NSData data, NSUrlResponse response, NSError error);
	delegate void NSUrlSessionDownloadResponse (NSUrl data, NSUrlResponse response, NSError error);

	delegate void NSUrlDownloadSessionResponse (NSUrl location, NSUrlResponse response, NSError error);

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

#if !NET
		[Obsolete ("Use the overload with a 'INSUrlSessionDelegate' parameter.")]
		[Static, Wrap ("FromWeakConfiguration (configuration, sessionDelegate, delegateQueue);")]
		NSUrlSession FromConfiguration (NSUrlSessionConfiguration configuration, NSUrlSessionDelegate sessionDelegate, [NullAllowed] NSOperationQueue delegateQueue);
#endif
		[Static, Wrap ("FromWeakConfiguration (configuration, (NSObject) sessionDelegate, delegateQueue);")]
		NSUrlSession FromConfiguration (NSUrlSessionConfiguration configuration, INSUrlSessionDelegate sessionDelegate, [NullAllowed] NSOperationQueue delegateQueue);

		[Export ("delegateQueue", ArgumentSemantic.Retain)]
		NSOperationQueue DelegateQueue { get; }

		[Export ("delegate", ArgumentSemantic.Retain), NullAllowed]
		NSObject WeakDelegate { get; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSUrlSessionDelegate Delegate { get; }

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
		[Async]
		void Reset (Action completionHandler);

		[Export ("flushWithCompletionHandler:")]
		[Async]
		void Flush (Action completionHandler);

#if !XAMCORE_3_0
		// broken version that we must keep for XAMCORE_3_0 binary compatibility
		// but that we do not have to expose on tvOS and watchOS, forcing people to use the correct API
		[Obsolete ("Use GetTasks2 instead. This method may throw spurious InvalidCastExceptions, in particular for backgrounded tasks.")]
		[Export ("getTasksWithCompletionHandler:")]
		[Async (ResultTypeName = "NSUrlSessionActiveTasks")]
		void GetTasks (NSUrlSessionPendingTasks completionHandler);
#elif NET
		// Fixed version (breaking change) only for NET
		[Export ("getTasksWithCompletionHandler:")]
		[Async (ResultTypeName="NSUrlSessionActiveTasks")]
		void GetTasks (NSUrlSessionPendingTasks completionHandler);
#endif

#if !NET
		// Workaround, not needed for NET+
		[Sealed]
		[Export ("getTasksWithCompletionHandler:")]
		[Async (ResultTypeName = "NSUrlSessionActiveTasks2")]
		void GetTasks2 (NSUrlSessionPendingTasks2 completionHandler);
#endif

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
		[Async (ResultTypeName = "NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDataTask CreateDataTask (NSUrlRequest request, [NullAllowed] NSUrlSessionResponse completionHandler);

		[Export ("dataTaskWithURL:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDataTask CreateDataTask (NSUrl url, [NullAllowed] NSUrlSessionResponse completionHandler);

		[Export ("uploadTaskWithRequest:fromFile:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSUrl fileURL, NSUrlSessionResponse completionHandler);

		[Export ("uploadTaskWithRequest:fromData:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSData bodyData, NSUrlSessionResponse completionHandler);

		[Export ("downloadTaskWithRequest:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrlRequest request, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);

		[Export ("downloadTaskWithURL:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrl url, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);

		[Export ("downloadTaskWithResumeData:completionHandler:")]
		[return: ForcedType]
		[Async (ResultTypeName = "NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDownloadTask CreateDownloadTaskFromResumeData (NSData resumeData, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);


		[MacCatalyst (13, 1)]
		[Export ("getAllTasksWithCompletionHandler:")]
		[Async (ResultTypeName = "NSUrlSessionCombinedTasks")]
		void GetAllTasks (NSUrlSessionAllPendingTasks completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("streamTaskWithHostName:port:")]
		NSUrlSessionStreamTask CreateBidirectionalStream (string hostname, nint port);

		[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the Network.framework instead.")]
		[Deprecated (PlatformName.iOS, 15, 0, message: "Use the Network.framework instead.")]
		[Deprecated (PlatformName.TvOS, 15, 0, message: "Use the Network.framework instead.")]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the Network.framework instead.")]
		[Export ("streamTaskWithNetService:")]
		NSUrlSessionStreamTask CreateBidirectionalStream (NSNetService service);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("webSocketTaskWithURL:")]
		NSUrlSessionWebSocketTask CreateWebSocketTask (NSUrl url);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("webSocketTaskWithURL:protocols:")]
		NSUrlSessionWebSocketTask CreateWebSocketTask (NSUrl url, string [] protocols);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("webSocketTaskWithRequest:")]
		NSUrlSessionWebSocketTask CreateWebSocketTask (NSUrlRequest request);

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("uploadTaskWithResumeData:")]
		[return: ForcedType]
		NSUrlSessionUploadTask CreateUploadTask (NSData resumeData);

		[Async (ResultTypeName = "NSUrlSessionUploadTaskResumeRequest")]
		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("uploadTaskWithResumeData:completionHandler:")]
		[return: ForcedType]
		NSUrlSessionUploadTask CreateUploadTask (NSData resumeData, Action<NSData, NSUrlResponse, NSError> completionHandler);
	}

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name = "NSURLSessionStreamDelegate")]
	interface NSUrlSessionStreamDelegate {
		[Export ("URLSession:readClosedForStreamTask:")]
		void ReadClosed (NSUrlSession session, NSUrlSessionStreamTask streamTask);

		[Export ("URLSession:writeClosedForStreamTask:")]
		void WriteClosed (NSUrlSession session, NSUrlSessionStreamTask streamTask);

		[Export ("URLSession:betterRouteDiscoveredForStreamTask:")]
		void BetterRouteDiscovered (NSUrlSession session, NSUrlSessionStreamTask streamTask);

		//
		// Note: the names of this methods do not exactly match the Objective-C name
		// because it was a bad name, and does not describe what this does, so the name
		// was picked from the documentation and what it does.
		//
		[Export ("URLSession:streamTask:didBecomeInputStream:outputStream:")]
		void CompletedTaskCaptureStreams (NSUrlSession session, NSUrlSessionStreamTask streamTask, NSInputStream inputStream, NSOutputStream outputStream);
	}

	delegate void NSUrlSessionDataRead (NSData data, bool atEof, NSError error);
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSUrlSessionTask), Name = "NSURLSessionStreamTask")]
	[DisableDefaultCtor] // now (xcode11) marked as deprecated
	interface NSUrlSessionStreamTask {
		[Export ("readDataOfMinLength:maxLength:timeout:completionHandler:")]
		[Async (ResultTypeName = "NSUrlSessionStreamDataRead")]
		void ReadData (nuint minBytes, nuint maxBytes, double timeout, NSUrlSessionDataRead completionHandler);

		[Export ("writeData:timeout:completionHandler:")]
		[Async]
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
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "A secure (TLS) connection cannot become drop back to insecure (non-TLS).")]
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
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "This type is not meant to be user created.")]
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

		[Field ("NSURLSessionTransferSizeUnknown")]
		long TransferSizeUnknown { get; }

		[MacCatalyst (13, 1)]
		[Export ("priority")]
		float Priority { get; set; } /* float, not CGFloat */

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
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

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[NullAllowed, Export ("delegate", ArgumentSemantic.Retain)]
		NSObject WeakDelegate { get; set; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		INSUrlSessionTaskDelegate Delegate { get; set; }
	}

	[Static]
	[MacCatalyst (13, 1)]
	interface NSUrlSessionTaskPriority {
		[Field ("NSURLSessionTaskPriorityDefault")]
		float Default { get; } /* float, not CGFloat */

		[Field ("NSURLSessionTaskPriorityLow")]
		float Low { get; } /* float, not CGFloat */

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
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NSURLSession.CreateDataTask' instead.")]
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
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NSURLSession.CreateUploadTask' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSURLSession.CreateUploadTask' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSURLSession.CreateUploadTask' instead.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Field ("NSURLSessionUploadTaskResumeData")]
		NSString ResumeDataKey { get; }

		[Async]
		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("cancelByProducingResumeData:")]
		void CancelByProducingResumeData (Action<NSData> completionHandler);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSUrlSessionTask), Name = "NSURLSessionDownloadTask")]
	[DisableDefaultCtor]
	partial interface NSUrlSessionDownloadTask {
		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'NSURLSession.CreateDownloadTask' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'NSURLSession.CreateDownloadTask' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'NSURLSession.CreateDownloadTask' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'NSURLSession.CreateDownloadTask' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'NSURLSession.CreateDownloadTask' instead.")]
		[Export ("init")]
		NativeHandle Constructor ();

		[Export ("cancelByProducingResumeData:")]
		void Cancel (Action<NSData> resumeCallback);
	}

	[Internal]
	[Static]
	[NoWatch]
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

	[NoWatch]
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

		[Export ("discretionary")]
		bool Discretionary { [Bind ("isDiscretionary")] get; set; }

		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		[Export ("sessionSendsLaunchEvents")]
		bool SessionSendsLaunchEvents { get; set; }

		[NullAllowed]
		[Export ("connectionProxyDictionary", ArgumentSemantic.Copy)]
		NSDictionary ConnectionProxyDictionary { get; set; }

		[NoWatch]
		[MacCatalyst (13, 1)]
		ProxyConfigurationDictionary StrongConnectionProxyDictionary {
			[Wrap ("new ProxyConfigurationDictionary (ConnectionProxyDictionary!)")]
			get;
			[Wrap ("ConnectionProxyDictionary = value.GetDictionary ()")]
			set;
		}

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'TlsMinimumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TlsMinimumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'TlsMinimumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'TlsMinimumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'TlsMinimumSupportedProtocolVersion' instead.")]
		[Export ("TLSMinimumSupportedProtocol")]
		SslProtocol TLSMinimumSupportedProtocol { get; set; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("TLSMinimumSupportedProtocolVersion", ArgumentSemantic.Assign)]
		TlsProtocolVersion TlsMinimumSupportedProtocolVersion { get; set; }

		[Deprecated (PlatformName.MacOSX, 10, 15, message: "Use 'TlsMaximumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.iOS, 13, 0, message: "Use 'TlsMaximumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "Use 'TlsMaximumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "Use 'TlsMaximumSupportedProtocolVersion' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'TlsMaximumSupportedProtocolVersion' instead.")]
		[Export ("TLSMaximumSupportedProtocol")]
		SslProtocol TLSMaximumSupportedProtocol { get; set; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("TLSMaximumSupportedProtocolVersion", ArgumentSemantic.Assign)]
		TlsProtocolVersion TlsMaximumSupportedProtocolVersion { get; set; }

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

		[MacCatalyst (13, 1)]
		[Export ("shouldUseExtendedBackgroundIdleMode")]
		bool ShouldUseExtendedBackgroundIdleMode { get; set; }

		[NoWatch, NoTV, NoMac]
		[MacCatalyst (13, 1)]
		[Export ("multipathServiceType", ArgumentSemantic.Assign)]
		NSUrlSessionMultipathServiceType MultipathServiceType { get; set; }

		[MacCatalyst (13, 1)]
		[Export ("waitsForConnectivity")]
		bool WaitsForConnectivity { get; set; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("allowsExpensiveNetworkAccess")]
		bool AllowsExpensiveNetworkAccess { get; set; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("allowsConstrainedNetworkAccess")]
		bool AllowsConstrainedNetworkAccess { get; set; }

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("requiresDNSSECValidation")]
		bool RequiresDnsSecValidation { get; set; }

		// not yet ready, waiting on pr https://github.com/xamarin/xamarin-macios/pull/7539 to be fixed
		// to land this and the manual property
		// [Internal]
		// [Export ("proxyConfigurations", ArgumentSemantic.Copy)]
		// IntPtr[] _ProxyConfigurations { get; set; }
	}

	[MacCatalyst (13, 1)]
	[Model, BaseType (typeof (NSObject), Name = "NSURLSessionDelegate")]
	[Protocol]
	partial interface NSUrlSessionDelegate {
		[Export ("URLSession:didBecomeInvalidWithError:")]
		void DidBecomeInvalid (NSUrlSession session, NSError error);

		[Export ("URLSession:didReceiveChallenge:completionHandler:")]
		void DidReceiveChallenge (NSUrlSession session, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler);

		[Mac (11, 0)]
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

		[Export ("URLSession:task:willPerformHTTPRedirection:newRequest:completionHandler:")]
		void WillPerformHttpRedirection (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, Action<NSUrlRequest> completionHandler);

		[Export ("URLSession:task:didReceiveChallenge:completionHandler:")]
		void DidReceiveChallenge (NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler);

		[Export ("URLSession:task:needNewBodyStream:")]
		void NeedNewBodyStream (NSUrlSession session, NSUrlSessionTask task, Action<NSInputStream> completionHandler);

		[Export ("URLSession:task:didSendBodyData:totalBytesSent:totalBytesExpectedToSend:")]
		void DidSendBodyData (NSUrlSession session, NSUrlSessionTask task, long bytesSent, long totalBytesSent, long totalBytesExpectedToSend);

		[Export ("URLSession:task:didCompleteWithError:")]
		void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, [NullAllowed] NSError error);

		[MacCatalyst (13, 1)]
		[Export ("URLSession:task:didFinishCollectingMetrics:")]
		void DidFinishCollectingMetrics (NSUrlSession session, NSUrlSessionTask task, NSUrlSessionTaskMetrics metrics);

		[MacCatalyst (13, 1)]
		[Export ("URLSession:task:willBeginDelayedRequest:completionHandler:")]
		void WillBeginDelayedRequest (NSUrlSession session, NSUrlSessionTask task, NSUrlRequest request, Action<NSUrlSessionDelayedRequestDisposition, NSUrlRequest> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("URLSession:taskIsWaitingForConnectivity:")]
		void TaskIsWaitingForConnectivity (NSUrlSession session, NSUrlSessionTask task);

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("URLSession:didCreateTask:")]
		void DidCreateTask (NSUrlSession session, NSUrlSessionTask task);

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("URLSession:task:didReceiveInformationalResponse:")]
		void DidReceiveInformationalResponse (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response);

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("URLSession:task:needNewBodyStreamFromOffset:completionHandler:")]
		void NeedNewBodyStream (NSUrlSession session, NSUrlSessionTask task, long offset, Action<NSInputStream> completionHandler);
	}

	[MacCatalyst (13, 1)]
	[Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name = "NSURLSessionDataDelegate")]
	[Protocol]
	partial interface NSUrlSessionDataDelegate {
		[Export ("URLSession:dataTask:didReceiveResponse:completionHandler:")]
		void DidReceiveResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler);

		[Export ("URLSession:dataTask:didBecomeDownloadTask:")]
		void DidBecomeDownloadTask (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlSessionDownloadTask downloadTask);

		[Export ("URLSession:dataTask:didReceiveData:")]
		void DidReceiveData (NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data);

		[Export ("URLSession:dataTask:willCacheResponse:completionHandler:")]
		void WillCacheResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSCachedUrlResponse proposedResponse, Action<NSCachedUrlResponse> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("URLSession:dataTask:didBecomeStreamTask:")]
		void DidBecomeStreamTask (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlSessionStreamTask streamTask);
	}

	[MacCatalyst (13, 1)]
	[Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name = "NSURLSessionDownloadDelegate")]
	[Protocol]
	partial interface NSUrlSessionDownloadDelegate {

		[Abstract]
		[Export ("URLSession:downloadTask:didFinishDownloadingToURL:")]
		void DidFinishDownloading (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location);

		[Export ("URLSession:downloadTask:didWriteData:totalBytesWritten:totalBytesExpectedToWrite:")]
		void DidWriteData (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long bytesWritten, long totalBytesWritten, long totalBytesExpectedToWrite);

		[Export ("URLSession:downloadTask:didResumeAtOffset:expectedTotalBytes:")]
		void DidResume (NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long resumeFileOffset, long expectedTotalBytes);

		[Field ("NSURLSessionDownloadTaskResumeData")]
		NSString TaskResumeDataKey { get; }
	}

	interface NSUndoManagerCloseUndoGroupEventArgs {
		// Bug in docs, see header file
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

		[Export ("isUndoRegistrationEnabled")]
		bool IsUndoRegistrationEnabled { get; }

		[Export ("groupsByEvent")]
		bool GroupsByEvent { get; set; }

		[Export ("levelsOfUndo")]
		nint LevelsOfUndo { get; set; }

#if NET
		[Export ("runLoopModes", ArgumentSemantic.Copy)]
		NSString [] WeakRunLoopModes { get; set; }
#else
		[Export ("runLoopModes")]
		string [] RunLoopModes { get; set; }
#endif

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

		[Export ("isUndoing")]
		bool IsUndoing { get; }

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

#if NET
		[Export ("setActionName:")]
		void SetActionName (string actionName);
#else
		[Advice ("Use the correctly named method: 'SetActionName'.")]
		[Export ("setActionName:")]
		void SetActionname (string actionName);
#endif

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

		[Field ("NSUndoManagerGroupIsDiscardableKey")]
		NSString GroupIsDiscardableKey { get; }

		[Field ("NSUndoManagerDidCloseUndoGroupNotification")]
		[Notification (typeof (NSUndoManagerCloseUndoGroupEventArgs))]
		NSString DidCloseUndoGroupNotification { get; }

		[MacCatalyst (13, 1)]
		[Export ("registerUndoWithTarget:handler:")]
		void RegisterUndo (NSObject target, Action<NSObject> undoHandler);

	}

	[BaseType (typeof (NSObject), Name = "NSURLProtectionSpace")]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface NSUrlProtectionSpace : NSSecureCoding, NSCopying {

		[Internal]
		[Export ("initWithHost:port:protocol:realm:authenticationMethod:")]
		IntPtr Init (string host, nint port, [NullAllowed] string protocol, [NullAllowed] string realm, [NullAllowed] string authenticationMethod);

		[Internal]
		[Export ("initWithProxyHost:port:type:realm:authenticationMethod:")]
		IntPtr InitWithProxy (string host, nint port, [NullAllowed] string type, [NullAllowed] string realm, [NullAllowed] string authenticationMethod);

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

		[Field ("NSURLProtectionSpaceHTTP")]
		NSString HTTP { get; }

		[Field ("NSURLProtectionSpaceHTTPS")]
		NSString HTTPS { get; }

		[Field ("NSURLProtectionSpaceFTP")]
		NSString FTP { get; }

		[Field ("NSURLProtectionSpaceHTTPProxy")]
		NSString HTTPProxy { get; }

		[Field ("NSURLProtectionSpaceHTTPSProxy")]
		NSString HTTPSProxy { get; }

		[Field ("NSURLProtectionSpaceFTPProxy")]
		NSString FTPProxy { get; }

		[Field ("NSURLProtectionSpaceSOCKSProxy")]
		NSString SOCKSProxy { get; }

		[Field ("NSURLAuthenticationMethodDefault")]
		NSString AuthenticationMethodDefault { get; }

		[Field ("NSURLAuthenticationMethodHTTPBasic")]
		NSString AuthenticationMethodHTTPBasic { get; }

		[Field ("NSURLAuthenticationMethodHTTPDigest")]
		NSString AuthenticationMethodHTTPDigest { get; }

		[Field ("NSURLAuthenticationMethodHTMLForm")]
		NSString AuthenticationMethodHTMLForm { get; }

		[Field ("NSURLAuthenticationMethodNTLM")]
		NSString AuthenticationMethodNTLM { get; }

		[Field ("NSURLAuthenticationMethodNegotiate")]
		NSString AuthenticationMethodNegotiate { get; }

		[Field ("NSURLAuthenticationMethodClientCertificate")]
		NSString AuthenticationMethodClientCertificate { get; }

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

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("allowsExpensiveNetworkAccess")]
		bool AllowsExpensiveNetworkAccess { get; [NotImplemented] set; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("allowsConstrainedNetworkAccess")]
		bool AllowsConstrainedNetworkAccess { get; [NotImplemented] set; }

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

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("assumesHTTP3Capable")]
		bool AssumesHttp3Capable { get; [NotImplemented] set; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("attribution")]
		NSURLRequestAttribution Attribution { get; }

		// macOS is documented out of sync with iOS here
		[Watch (9, 1), TV (16, 1), Mac (13, 0), iOS (16, 1)]
		[MacCatalyst (16, 1)]
		[Export ("requiresDNSSECValidation")]
		bool RequiresDnsSecValidation { get; }
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

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("allowsExpensiveNetworkAccess")]
		bool AllowsExpensiveNetworkAccess { get; set; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("allowsConstrainedNetworkAccess")]
		bool AllowsConstrainedNetworkAccess { get; set; }

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Export ("assumesHTTP3Capable")]
		bool AssumesHttp3Capable { get; set; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("attribution", ArgumentSemantic.Assign)]
		NSURLRequestAttribution Attribution { get; set; }

		// Documented as 16.0 but did not work until 16.1 - https://github.com/xamarin/maccore/issues/2608 - https://feedbackassistant.apple.com/feedback/10897552
		[Watch (9, 1), TV (16, 1), Mac (13, 0), iOS (16, 1)]
		[MacCatalyst (16, 1)]
		[Export ("requiresDNSSECValidation")]
		bool RequiresDnsSecValidation { get; set; }
	}

	[BaseType (typeof (NSObject), Name = "NSURLResponse")]
	interface NSUrlResponse : NSSecureCoding, NSCopying {
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

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSStreamDelegate Delegate { get; set; }

#if NET
		[Abstract]
#endif
		[return: NullAllowed]
		[Protected]
		[Export ("propertyForKey:")]
		NSObject GetProperty (NSString key);

#if NET
		[Abstract]
#endif
		[Protected]
		[Export ("setProperty:forKey:")]
		bool SetProperty ([NullAllowed] NSObject property, NSString key);

#if NET
		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, NSString mode);

		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, NSString mode);
#else
		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, string mode);

		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, string mode);
#endif
		[Wrap ("Schedule (aRunLoop, mode.GetConstant ()!)")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode mode);

		[Wrap ("Unschedule (aRunLoop, mode.GetConstant ()!)")]
		void Unschedule (NSRunLoop aRunLoop, NSRunLoopMode mode);

		[Export ("streamStatus")]
		NSStreamStatus Status { get; }

		[Export ("streamError")]
		NSError Error { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelKey")]
		NSString SocketSecurityLevelKey { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelNone")]
		NSString SocketSecurityLevelNone { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelSSLv2")]
		NSString SocketSecurityLevelSslV2 { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelSSLv3")]
		NSString SocketSecurityLevelSslV3 { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelTLSv1")]
		NSString SocketSecurityLevelTlsV1 { get; }

		[Advanced, Field ("NSStreamSocketSecurityLevelNegotiatedSSL")]
		NSString SocketSecurityLevelNegotiatedSsl { get; }

		[Advanced, Field ("NSStreamSOCKSProxyConfigurationKey")]
		NSString SocksProxyConfigurationKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyHostKey")]
		NSString SocksProxyHostKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyPortKey")]
		NSString SocksProxyPortKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyVersionKey")]
		NSString SocksProxyVersionKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyUserKey")]
		NSString SocksProxyUserKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyPasswordKey")]
		NSString SocksProxyPasswordKey { get; }

		[Advanced, Field ("NSStreamSOCKSProxyVersion4")]
		NSString SocksProxyVersion4 { get; }

		[Advanced, Field ("NSStreamSOCKSProxyVersion5")]
		NSString SocksProxyVersion5 { get; }

		[Advanced, Field ("NSStreamDataWrittenToMemoryStreamKey")]
		NSString DataWrittenToMemoryStreamKey { get; }

		[Advanced, Field ("NSStreamFileCurrentOffsetKey")]
		NSString FileCurrentOffsetKey { get; }

		[Advanced, Field ("NSStreamSocketSSLErrorDomain")]
		NSString SocketSslErrorDomain { get; }

		[Advanced, Field ("NSStreamSOCKSErrorDomain")]
		NSString SocksErrorDomain { get; }

		[Advanced, Field ("NSStreamNetworkServiceType")]
		NSString NetworkServiceType { get; }

		[Advanced, Field ("NSStreamNetworkServiceTypeVoIP")]
		NSString NetworkServiceTypeVoIP { get; }

		[Advanced, Field ("NSStreamNetworkServiceTypeVideo")]
		NSString NetworkServiceTypeVideo { get; }

		[Advanced, Field ("NSStreamNetworkServiceTypeBackground")]
		NSString NetworkServiceTypeBackground { get; }

		[Advanced, Field ("NSStreamNetworkServiceTypeVoice")]
		NSString NetworkServiceTypeVoice { get; }

		[Advanced]
		[MacCatalyst (13, 1)]
		[Field ("NSStreamNetworkServiceTypeCallSignaling")]
		NSString NetworkServiceTypeCallSignaling { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("getBoundStreamsWithBufferSize:inputStream:outputStream:")]
		void GetBoundStreams (nuint bufferSize, out NSInputStream inputStream, out NSOutputStream outputStream);

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static, Export ("getStreamsToHostWithName:port:inputStream:outputStream:")]
		void GetStreamsToHost (string hostname, nint port, out NSInputStream inputStream, out NSOutputStream outputStream);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSStreamDelegate {
		[Export ("stream:handleEvent:"), EventArgs ("NSStream"), EventName ("OnEvent")]
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

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Bind ("sizeWithAttributes:")]
		CGSize StringSize ([NullAllowed] NSDictionary attributedStringAttributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Bind ("boundingRectWithSize:options:attributes:")]
		CGRect BoundingRectWithSize (CGSize size, NSStringDrawingOptions options, NSDictionary attributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Bind ("drawAtPoint:withAttributes:")]
		void DrawString (CGPoint point, NSDictionary attributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Bind ("drawInRect:withAttributes:")]
		void DrawString (CGRect rect, NSDictionary attributes);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Bind ("drawWithRect:options:attributes:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options, NSDictionary attributes);

		[Internal]
		[Export ("characterAtIndex:")]
		char _characterAtIndex (nint index);

		[Export ("length")]
		nint Length { get; }

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
		NSString StandarizePath ();

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

		[Export ("getLineStart:end:contentsEnd:forRange:")]
		void GetLineStart (out nuint startPtr, out nuint lineEndPtr, out nuint contentsEndPtr, NSRange range);

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
		[NoTV]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("localizedUserNotificationStringForKey:arguments:")]
		NSString GetLocalizedUserNotificationString (NSString key, [Params][NullAllowed] NSObject [] arguments);

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

	[StrongDictionary ("NSString")]
	interface EncodingDetectionOptions {
		NSStringEncoding [] EncodingDetectionSuggestedEncodings { get; set; }
		NSStringEncoding [] EncodingDetectionDisallowedEncodings { get; set; }
		bool EncodingDetectionUseOnlySuggestedEncodings { get; set; }
		bool EncodingDetectionAllowLossy { get; set; }
		bool EncodingDetectionFromWindows { get; set; }
		NSString EncodingDetectionLossySubstitution { get; set; }
		NSString EncodingDetectionLikelyLanguage { get; set; }
	}

	[BaseType (typeof (NSString))]
	// hack: it seems that generator.cs can't track NSCoding correctly ? maybe because the type is named NSString2 at that time
	interface NSMutableString : NSCoding {
		[Export ("initWithCapacity:")]
		NativeHandle Constructor (nint capacity);

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
		nuint ReplaceOcurrences (NSString target, NSString replacement, NSStringCompareOptions options, NSRange range);

		[MacCatalyst (13, 1)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("applyTransform:reverse:range:updatedRange:")]
		bool ApplyTransform (NSString transform, bool reverse, NSRange range, out NSRange resultingRange);

		[MacCatalyst (13, 1)]
		[Wrap ("ApplyTransform (transform.GetConstant ()!, reverse, range, out resultingRange)")]
		bool ApplyTransform (NSStringTransform transform, bool reverse, NSRange range, out NSRange resultingRange);

		[Export ("replaceCharactersInRange:withString:")]
		void ReplaceCharactersInRange (NSRange range, NSString aString);
	}

	[Category, BaseType (typeof (NSString))]
	partial interface NSUrlUtilities_NSString {
		[Export ("stringByAddingPercentEncodingWithAllowedCharacters:")]
		NSString CreateStringByAddingPercentEncoding (NSCharacterSet allowedCharacters);

		[Export ("stringByRemovingPercentEncoding")]
		NSString CreateStringByRemovingPercentEncoding ();

		[Export ("stringByAddingPercentEscapesUsingEncoding:")]
		NSString CreateStringByAddingPercentEscapes (NSStringEncoding enc);

		[Export ("stringByReplacingPercentEscapesUsingEncoding:")]
		NSString CreateStringByReplacingPercentEscapes (NSStringEncoding enc);
	}

	// This comes from UIKit.framework/Headers/NSStringDrawing.h
	[NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NSStringDrawingContext {
		[Export ("minimumScaleFactor")]
		nfloat MinimumScaleFactor { get; set; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[NoMacCatalyst]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("minimumTrackingAdjustment")]
		nfloat MinimumTrackingAdjustment { get; set; }

		[Export ("actualScaleFactor")]
		nfloat ActualScaleFactor { get; }

		[NoTV]
		[Deprecated (PlatformName.iOS, 7, 0)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("actualTrackingAdjustment")]
		nfloat ActualTrackingAdjustment { get; }

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

#if NET
		[return: NullAllowed]
		[Protected]
		[Export ("propertyForKey:"), Override]
		NSObject GetProperty (NSString key);

		[Protected]
		[Export ("setProperty:forKey:"), Override]
		bool SetProperty ([NullAllowed] NSObject property, NSString key);

#endif

	}

	delegate bool NSEnumerateLinguisticTagsEnumerator (NSString tag, NSRange tokenRange, NSRange sentenceRange, ref bool stop);

	[Category]
	[BaseType (typeof (NSString))]
	interface NSLinguisticAnalysis {
#if NET
		[return: BindAs (typeof (NSLinguisticTag []))]
#else
		[return: BindAs (typeof (NSLinguisticTagUnit []))]
#endif
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("linguisticTagsInRange:scheme:options:orthography:tokenRanges:")]
		NSString [] GetLinguisticTags (NSRange range, NSString scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, [NullAllowed] out NSValue [] tokenRanges);

		[Wrap ("GetLinguisticTags (This, range, scheme.GetConstant ()!, options, orthography, out tokenRanges)")]
#if NET
		NSLinguisticTag[] GetLinguisticTags (NSRange range, NSLinguisticTagScheme scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, [NullAllowed] out NSValue[] tokenRanges);
#else
		NSLinguisticTagUnit [] GetLinguisticTags (NSRange range, NSLinguisticTagScheme scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, [NullAllowed] out NSValue [] tokenRanges);
#endif

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("enumerateLinguisticTagsInRange:scheme:options:orthography:usingBlock:")]
		void EnumerateLinguisticTags (NSRange range, NSString scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, NSEnumerateLinguisticTagsEnumerator handler);

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
#if !NET
		[Sealed]
#endif
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("valueClassForBinding:")]
		Class GetBindingValueClass (NSString binding);

#if !NET
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Obsolete ("Use 'Bind (NSString binding, NSObject observable, string keyPath, [NullAllowed] NSDictionary options)' instead.")]
		[Export ("bind:toObject:withKeyPath:options:")]
		void Bind (string binding, NSObject observable, string keyPath, [NullAllowed] NSDictionary options);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Obsolete ("Use 'Unbind (NSString binding)' instead.")]
		[Export ("unbind:")]
		void Unbind (string binding);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Obsolete ("Use 'GetBindingValueClass (NSString binding)' instead.")]
		[Export ("valueClassForBinding:")]
		Class BindingValueClass (string binding);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Obsolete ("Use 'GetBindingInfo (NSString binding)' instead.")]
		[Export ("infoForBinding:")]
		NSDictionary BindingInfo (string binding);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Obsolete ("Use 'GetBindingOptionDescriptions (NSString aBinding)' instead.")]
		[Export ("optionDescriptionsForBinding:")]
		NSObject [] BindingOptionDescriptions (string aBinding);

		[Static]
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Wrap ("GetDefaultPlaceholder (marker, (NSString) binding)")]
		NSObject GetDefaultPlaceholder (NSObject marker, string binding);

		[Static]
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Obsolete ("Use 'SetDefaultPlaceholder (NSObject placeholder, NSObject marker, NSString binding)' instead.")]
		[Wrap ("SetDefaultPlaceholder (placeholder, marker, (NSString) binding)")]
		void SetDefaultPlaceholder (NSObject placeholder, NSObject marker, string binding);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("exposedBindings")]
		NSString [] ExposedBindings ();
#else
		[NoiOS][NoMacCatalyst][NoWatch][NoTV]
		[Export ("exposedBindings")]
		NSString[] ExposedBindings { get; }
#endif

#if !NET
		[Sealed]
#endif
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("bind:toObject:withKeyPath:options:")]
		void Bind (NSString binding, NSObject observable, string keyPath, [NullAllowed] NSDictionary options);

#if !NET
		[Sealed]
#endif
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("unbind:")]
		void Unbind (NSString binding);

#if !NET
		[Sealed]
#endif
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("infoForBinding:")]
		NSDictionary GetBindingInfo (NSString binding);

#if !NET
		[Sealed]
#endif
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("optionDescriptionsForBinding:")]
		NSObject [] GetBindingOptionDescriptions (NSString aBinding);

		// NSPlaceholders (informal) protocol
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Static]
		[Export ("defaultPlaceholderForMarker:withBinding:")]
		NSObject GetDefaultPlaceholder (NSObject marker, NSString binding);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Static]
		[Export ("setDefaultPlaceholder:forMarker:withBinding:")]
		void SetDefaultPlaceholder (NSObject placeholder, NSObject marker, NSString binding);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, message: "Now on 'NSEditor' protocol.")]
		[Export ("objectDidEndEditing:")]
		void ObjectDidEndEditing (NSObject editor);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Deprecated (PlatformName.MacOSX, message: "Now on 'NSEditor' protocol.")]
		[Export ("commitEditing")]
		bool CommitEditing ();

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
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

		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("prepareForInterfaceBuilder")]
		void PrepareForInterfaceBuilder ();

		[NoWatch]
		[MacCatalyst (13, 1)]
#if MONOMAC
		// comes from NSNibAwaking category and does not requires calling super
#else
		[RequiresSuper] // comes from UINibLoadingAdditions category - which is decorated
#endif
		[Export ("awakeFromNib")]
		void AwakeFromNib ();

		[NoWatch, TV (13, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("accessibilityRespondsToUserInteraction")]
		bool AccessibilityRespondsToUserInteraction { get; set; }

		[NoWatch, TV (13, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("accessibilityUserInputLabels", ArgumentSemantic.Strong)]
		string [] AccessibilityUserInputLabels { get; set; }

		[NoWatch, TV (13, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[Export ("accessibilityAttributedUserInputLabels", ArgumentSemantic.Copy)]
		NSAttributedString [] AccessibilityAttributedUserInputLabels { get; set; }

		[NoWatch, TV (13, 0), iOS (13, 0), NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("accessibilityTextualContext", ArgumentSemantic.Strong)]
		string AccessibilityTextualContext { get; set; }
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	[NoWatch]
	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	interface NSBindingSelectionMarker : NSCopying {
		[Static]
		[Export ("multipleValuesSelectionMarker", ArgumentSemantic.Strong)]
		NSBindingSelectionMarker MultipleValuesSelectionMarker { get; }

		[Static]
		[Export ("noSelectionMarker", ArgumentSemantic.Strong)]
		NSBindingSelectionMarker NoSelectionMarker { get; }

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

	[Protocol (Name = "NSObject")] // exists both as a type and a protocol in ObjC, Swift uses NSObjectProtocol
	interface NSObjectProtocol {

		[Abstract]
		[Export ("description")]
		string Description { get; }

		[Export ("debugDescription")]
		string DebugDescription { get; }

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("superclass")]
		Class Superclass { get; }

		// defined multiple times (method, property and even static), one (not static) is required
		// and that match Apple's documentation
		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("hash")]
		nuint GetNativeHash ();

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isEqual:")]
		bool IsEqual ([NullAllowed] NSObject anObject);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("class")]
		Class Class { get; }

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Export ("self")]
		[Transient]
		NSObject Self { get; }

		[Abstract]
		[Export ("performSelector:")]
		NSObject PerformSelector (Selector aSelector);

		[Abstract]
		[Export ("performSelector:withObject:")]
		NSObject PerformSelector (Selector aSelector, [NullAllowed] NSObject anObject);

		[Abstract]
		[Export ("performSelector:withObject:withObject:")]
		NSObject PerformSelector (Selector aSelector, [NullAllowed] NSObject object1, [NullAllowed] NSObject object2);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isProxy")]
		bool IsProxy { get; }

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isKindOfClass:")]
		bool IsKindOfClass ([NullAllowed] Class aClass);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("isMemberOfClass:")]
		bool IsMemberOfClass ([NullAllowed] Class aClass);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("conformsToProtocol:")]
		bool ConformsToProtocol ([NullAllowed] NativeHandle /* Protocol */ aProtocol);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("respondsToSelector:")]
		bool RespondsToSelector ([NullAllowed] Selector sel);

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("retain")]
		NSObject DangerousRetain ();

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("release")]
		void DangerousRelease ();

		[Abstract]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("autorelease")]
		NSObject DangerousAutorelease ();

		[Abstract]
		[Export ("retainCount")]
		nuint RetainCount { get; }

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

		[Export ("isCancelled")]
		bool IsCancelled { get; }

		[Export ("cancel")]
		void Cancel ();

		[Export ("isExecuting")]
		bool IsExecuting { get; }

		[Export ("isFinished")]
		bool IsFinished { get; }

		[Export ("isConcurrent")]
		bool IsConcurrent { get; }

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

		[Export ("waitUntilFinished")]
		void WaitUntilFinished ();

		[Export ("threadPriority")]
		[Deprecated (PlatformName.iOS, 8, 0)]
		[Deprecated (PlatformName.WatchOS, 2, 0)]
		[Deprecated (PlatformName.TvOS, 9, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 10)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		double ThreadPriority { get; set; }

		//Detected properties
		[Export ("queuePriority")]
		NSOperationQueuePriority QueuePriority { get; set; }

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
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "This API should not be used as it is subject to race conditions. If synchronization is needed use 'AddBarrier' instead.")]
		[Deprecated (PlatformName.TvOS, 13, 0, message: "This API should not be used as it is subject to race conditions. If synchronization is needed use 'AddBarrier' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "This API should not be used as it is subject to race conditions. If synchronization is needed use 'AddBarrier' instead.")]
		[Export ("operations")]
		NSOperation [] Operations { get; }

		[Deprecated (PlatformName.MacOSX, 10, 15)]
		[Deprecated (PlatformName.iOS, 13, 0)]
		[Deprecated (PlatformName.WatchOS, 6, 0)]
		[Deprecated (PlatformName.TvOS, 13, 0)]
		[Deprecated (PlatformName.MacCatalyst, 13, 1)]
		[Export ("operationCount")]
		nint OperationCount { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

#if false // https://github.com/xamarin/xamarin-macios/issues/15577
		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Wrap ("Runtime.GetNSObject <NSOrderedCollectionDifference> (_GetDifference (other, options))")]
		[return: NullAllowed]
		NSOrderedCollectionDifference GetDifference (NSOrderedSet other, NSOrderedCollectionDifferenceCalculationOptions options);
		
		[Internal]
		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Export ("differenceFromOrderedSet:withOptions:")]
		IntPtr _GetDifference (NSOrderedSet other, NSOrderedCollectionDifferenceCalculationOptions options);

		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Wrap ("Runtime.GetNSObject <NSOrderedCollectionDifference> (_GetDifference (other))")]
		[return: NullAllowed]
		NSOrderedCollectionDifference GetDifference (NSOrderedSet other);
		
		[Internal]
		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Export ("differenceFromOrderedSet:")]
		IntPtr _GetDifference (NSOrderedSet other);

		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Wrap ("Runtime.GetNSObject <NSOrderedSet> (_GetOrderedSet (difference))")]
		[return: NullAllowed]
		NSOrderedSet GetOrderedSet (NSOrderedCollectionDifference difference);
		
		[Internal]
		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Export ("orderedSetByApplyingDifference:")]
		[return: NullAllowed]
		IntPtr _GetOrderedSet (NSOrderedCollectionDifference difference);

		[Internal]
		[Watch (6,0), TV (13,0), iOS (13,0)]
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

		[Export ("insertObject:atIndex:")]
		void Insert (NSObject obj, nint atIndex);

		[Export ("removeObjectAtIndex:")]
		void Remove (nint index);

		[Internal]
		[Sealed]
		[Export ("replaceObjectAtIndex:withObject:")]
		void _Replace (nint objectAtIndex, IntPtr newObject);

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

		[Export ("exchangeObjectAtIndex:withObjectAtIndex:")]
		void ExchangeObject (nint first, nint second);

		[Export ("moveObjectsAtIndexes:toIndex:")]
		void MoveObjects (NSIndexSet indexSet, nint destination);

		[Internal]
		[Sealed]
		[Export ("setObject:atIndex:")]
		void _SetObject (IntPtr obj, nint index);

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

#if false // https://github.com/xamarin/xamarin-macios/issues/15577
		[Internal]
		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Export ("applyDifference:")]
		void _ApplyDifference (IntPtr difference);

		[Sealed]
		[Watch (6,0), TV (13,0), iOS (13,0)]
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

#if NET
		[return: NullAllowed]
		[Protected]
		[Export ("propertyForKey:"), Override]
		NSObject GetProperty (NSString key);

		[Protected]
		[Export ("setProperty:forKey:"), Override]
		bool SetProperty ([NullAllowed] NSObject property, NSString key);

#endif
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

		[Export ("isSessionOnly")]
		bool IsSessionOnly { get; }

		[Export ("domain")]
		string Domain { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("path")]
		string Path { get; }

		[Export ("isSecure")]
		bool IsSecure { get; }

		[Export ("isHTTPOnly")]
		bool IsHttpOnly { get; }

		[Export ("comment")]
		string Comment { get; }

		[Export ("commentURL")]
		NSUrl CommentUrl { get; }

		[Export ("portList")]
		NSNumber [] PortList { get; }

		[Field ("NSHTTPCookieName")]
		NSString KeyName { get; }

		[Field ("NSHTTPCookieValue")]
		NSString KeyValue { get; }

		[Field ("NSHTTPCookieOriginURL")]
		NSString KeyOriginUrl { get; }

		[Field ("NSHTTPCookieVersion")]
		NSString KeyVersion { get; }

		[Field ("NSHTTPCookieDomain")]
		NSString KeyDomain { get; }

		[Field ("NSHTTPCookiePath")]
		NSString KeyPath { get; }

		[Field ("NSHTTPCookieSecure")]
		NSString KeySecure { get; }

		[Field ("NSHTTPCookieExpires")]
		NSString KeyExpires { get; }

		[Field ("NSHTTPCookieComment")]
		NSString KeyComment { get; }

		[Field ("NSHTTPCookieCommentURL")]
		NSString KeyCommentUrl { get; }

		[Field ("NSHTTPCookieDiscard")]
		NSString KeyDiscard { get; }

		[Field ("NSHTTPCookieMaximumAge")]
		NSString KeyMaximumAge { get; }

		[Field ("NSHTTPCookiePort")]
		NSString KeyPort { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSHTTPCookieSameSitePolicy")]
		NSString KeySameSitePolicy { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSHTTPCookieSameSiteLax")]
		NSString KeySameSiteLax { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSHTTPCookieSameSiteStrict")]
		NSString KeySameSiteStrict { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("sameSitePolicy")]
		NSString SameSitePolicy { get; }
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
		[Async]
		[Export ("getCookiesForTask:completionHandler:")]
		void GetCookiesForTask (NSUrlSessionTask task, Action<NSHttpCookie []> completionHandler);

		[MacCatalyst (13, 1)]
		[Export ("storeCookies:forTask:")]
		void StoreCookies (NSHttpCookie [] cookies, NSUrlSessionTask task);

		[Notification]
		[Field ("NSHTTPCookieManagerAcceptPolicyChangedNotification")]
		NSString CookiesChangedNotification { get; }

		[Notification]
		[Field ("NSHTTPCookieManagerCookiesChangedNotification")]
		NSString AcceptPolicyChangedNotification { get; }
	}

	[BaseType (typeof (NSUrlResponse), Name = "NSHTTPURLResponse")]
	interface NSHttpUrlResponse {
		[Export ("initWithURL:MIMEType:expectedContentLength:textEncodingName:")]
		NativeHandle Constructor (NSUrl url, string mimetype, nint expectedContentLength, [NullAllowed] string textEncodingName);

		[Export ("initWithURL:statusCode:HTTPVersion:headerFields:")]
		NativeHandle Constructor (NSUrl url, nint statusCode, [NullAllowed] string httpVersion, [NullAllowed] NSDictionary headerFields);

		[Export ("statusCode")]
		nint StatusCode { get; }

		[Export ("allHeaderFields")]
		NSDictionary AllHeaderFields { get; }

		[Export ("localizedStringForStatusCode:")]
		[Static]
		string LocalizedStringForStatusCode (nint statusCode);

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("valueForHTTPHeaderField:")]
		[return: NullAllowed]
		string GetHttpHeaderValue (string headerField);
	}

	[BaseType (typeof (NSObject))]
#if MONOMAC
	[DisableDefaultCtor] // An uncaught exception was raised: -[__NSCFDictionary removeObjectForKey:]: attempt to remove nil key
#endif
	partial interface NSBundle {
		[Export ("mainBundle")]
		[Static]
		NSBundle MainBundle { get; }

		[Export ("bundleWithPath:")]
		[Static]
		NSBundle FromPath (string path);

		[DesignatedInitializer]
		[Export ("initWithPath:")]
		NativeHandle Constructor (string path);

		[Export ("bundleForClass:")]
		[Static]
		NSBundle FromClass (Class c);

		[Export ("bundleWithIdentifier:")]
		[Static]
		NSBundle FromIdentifier (string str);

#if !XAMCORE_5_0
		[Internal]
		[Export ("allBundles")]
		[Static]
		NSArray _InternalAllBundles { get; }

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

		[Export ("isLoaded")]
		bool IsLoaded { get; }

		[Export ("unload")]
		bool Unload ();

		[Export ("bundlePath")]
		string BundlePath { get; }

		[Export ("resourcePath")]
		string ResourcePath { get; }

		[Export ("executablePath")]
		string ExecutablePath { get; }

		[Export ("pathForAuxiliaryExecutable:")]
		string PathForAuxiliaryExecutable (string s);


		[Export ("privateFrameworksPath")]
		string PrivateFrameworksPath { get; }

		[Export ("sharedFrameworksPath")]
		string SharedFrameworksPath { get; }

		[Export ("sharedSupportPath")]
		string SharedSupportPath { get; }

		[Export ("builtInPlugInsPath")]
		string BuiltinPluginsPath { get; }

		[Export ("bundleIdentifier")]
		string BundleIdentifier { get; }

		[Export ("classNamed:")]
		Class ClassNamed (string className);

		[Export ("principalClass")]
		Class PrincipalClass { get; }

		[Export ("pathForResource:ofType:inDirectory:")]
		[Static]
		string PathForResourceAbsolute (string name, [NullAllowed] string ofType, string bundleDirectory);

		[Export ("pathForResource:ofType:")]
		string PathForResource (string name, [NullAllowed] string ofType);

		[Export ("pathForResource:ofType:inDirectory:")]
		string PathForResource (string name, [NullAllowed] string ofType, [NullAllowed] string subpath);

		[Export ("pathForResource:ofType:inDirectory:forLocalization:")]
		string PathForResource (string name, [NullAllowed] string ofType, string subpath, string localizationName);

		[Export ("localizedStringForKey:value:table:")]
		NSString GetLocalizedString ([NullAllowed] NSString key, [NullAllowed] NSString value, [NullAllowed] NSString table);

		[Export ("objectForInfoDictionaryKey:")]
		NSObject ObjectForInfoDictionary (string key);

		[Export ("developmentLocalization")]
		string DevelopmentLocalization { get; }

		[Export ("infoDictionary")]
		NSDictionary InfoDictionary { get; }

		// Additions from AppKit
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("loadNibNamed:owner:topLevelObjects:")]
		bool LoadNibNamed (string nibName, [NullAllowed] NSObject owner, out NSArray topLevelObjects);

		// https://developer.apple.com/library/mac/#documentation/Cocoa/Reference/ApplicationKit/Classes/NSBundle_AppKitAdditions/Reference/Reference.html
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Static]
		[Deprecated (PlatformName.MacOSX, 10, 8)]
		[Export ("loadNibNamed:owner:")]
		bool LoadNib (string nibName, NSObject owner);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("pathForImageResource:")]
		string PathForImageResource (string resource);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("pathForSoundResource:")]
		string PathForSoundResource (string resource);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("URLForImageResource:")]
		NSUrl GetUrlForImageResource (string resource);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("contextHelpForKey:")]
		NSAttributedString GetContextHelp (string key);

		// http://developer.apple.com/library/ios/#documentation/uikit/reference/NSBundle_UIKitAdditions/Introduction/Introduction.html
		[NoMac]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("loadNibNamed:owner:options:")]
		NSArray LoadNib (string nibName, [NullAllowed] NSObject owner, [NullAllowed] NSDictionary options);

		[Export ("bundleURL")]
		NSUrl BundleUrl { get; }

		[Export ("resourceURL")]
		NSUrl ResourceUrl { get; }

		[Export ("executableURL")]
		NSUrl ExecutableUrl { get; }

		[Export ("URLForAuxiliaryExecutable:")]
		NSUrl UrlForAuxiliaryExecutable (string executable);

		[Export ("privateFrameworksURL")]
		NSUrl PrivateFrameworksUrl { get; }

		[Export ("sharedFrameworksURL")]
		NSUrl SharedFrameworksUrl { get; }

		[Export ("sharedSupportURL")]
		NSUrl SharedSupportUrl { get; }

		[Export ("builtInPlugInsURL")]
		NSUrl BuiltInPluginsUrl { get; }

		[Export ("initWithURL:")]
		NativeHandle Constructor (NSUrl url);

		[Static, Export ("bundleWithURL:")]
		NSBundle FromUrl (NSUrl url);

		[Export ("preferredLocalizations")]
		string [] PreferredLocalizations { get; }

		[Export ("localizations")]
		string [] Localizations { get; }

		[Export ("appStoreReceiptURL")]
		NSUrl AppStoreReceiptUrl { get; }

		[Export ("pathsForResourcesOfType:inDirectory:")]
		string [] PathsForResources (string fileExtension, [NullAllowed] string subDirectory);

		[Export ("pathsForResourcesOfType:inDirectory:forLocalization:")]
		string [] PathsForResources (string fileExtension, [NullAllowed] string subDirectory, [NullAllowed] string localizationName);

		[Static, Export ("pathsForResourcesOfType:inDirectory:")]
		string [] GetPathsForResources (string fileExtension, string bundlePath);

		[Static, Export ("URLForResource:withExtension:subdirectory:inBundleWithURL:")]
		NSUrl GetUrlForResource (string name, string fileExtension, [NullAllowed] string subdirectory, NSUrl bundleURL);

		[Static, Export ("URLsForResourcesWithExtension:subdirectory:inBundleWithURL:")]
		NSUrl [] GetUrlsForResourcesWithExtension (string fileExtension, [NullAllowed] string subdirectory, NSUrl bundleURL);

		[Export ("URLForResource:withExtension:")]
		NSUrl GetUrlForResource (string name, string fileExtension);

		[Export ("URLForResource:withExtension:subdirectory:")]
		NSUrl GetUrlForResource (string name, string fileExtension, [NullAllowed] string subdirectory);

		[Export ("URLForResource:withExtension:subdirectory:localization:")]
		NSUrl GetUrlForResource (string name, string fileExtension, [NullAllowed] string subdirectory, [NullAllowed] string localizationName);

		[Export ("URLsForResourcesWithExtension:subdirectory:")]
		NSUrl [] GetUrlsForResourcesWithExtension (string fileExtension, [NullAllowed] string subdirectory);

		[Export ("URLsForResourcesWithExtension:subdirectory:localization:")]
		NSUrl [] GetUrlsForResourcesWithExtension (string fileExtension, [NullAllowed] string subdirectory, [NullAllowed] string localizationName);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("preservationPriorityForTag:")]
		double GetPreservationPriority (NSString tag);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("setPreservationPriority:forTags:")]
		void SetPreservationPriority (double priority, NSSet<NSString> tags);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("localizedAttributedStringForKey:value:table:")]
		NSAttributedString GetLocalizedAttributedString (string key, [NullAllowed] string value, [NullAllowed] string tableName);
	}

	[NoMac]
	[MacCatalyst (13, 1)]
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
		[Async]
		void BeginAccessingResources (Action<NSError> completionHandler);

		[Export ("conditionallyBeginAccessingResourcesWithCompletionHandler:")]
		[Async]
		void ConditionallyBeginAccessingResources (Action<bool> completionHandler);

		[Export ("endAccessingResources")]
		void EndAccessingResources ();

		[Field ("NSBundleResourceRequestLowDiskSpaceNotification")]
		[Notification]
		NSString LowDiskSpaceNotification { get; }

		[Field ("NSBundleResourceRequestLoadingPriorityUrgent")]
		double LoadingPriorityUrgent { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSIndexPath : NSCoding, NSSecureCoding, NSCopying {
		[Export ("indexPathWithIndex:")]
		[Static]
		NSIndexPath FromIndex (nuint index);

		[Export ("indexPathWithIndexes:length:")]
		[Internal]
		[Static]
		NSIndexPath _FromIndex (IntPtr indexes, nint len);

		[Export ("indexPathByAddingIndex:")]
		NSIndexPath IndexPathByAddingIndex (nuint index);

		[Export ("indexPathByRemovingLastIndex")]
		NSIndexPath IndexPathByRemovingLastIndex ();

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
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("row")]
		nint LongRow { get; }

		[NoMac]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("section")]
		nint LongSection { get; }

		[NoMac]
		[NoWatch]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("indexPathForRow:inSection:")]
		NSIndexPath FromRowSection (nint row, nint section);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("section")]
		nint Section { get; }

		[NoWatch]
		[Static]
		[MacCatalyst (13, 1)]
		[Export ("indexPathForItem:inSection:")]
		NSIndexPath FromItemSection (nint item, nint section);

		[NoWatch]
		[Export ("item")]
		[MacCatalyst (13, 1)]
		nint Item { get; }
	}

	delegate void NSRangeIterator (NSRange range, ref bool stop);

	[BaseType (typeof (NSObject))]
	interface NSIndexSet : NSCoding, NSSecureCoding, NSMutableCopying {
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

		[Export ("indexGreaterThanIndex:")]
		nuint IndexGreaterThan (nuint index);

		[Export ("indexLessThanIndex:")]
		nuint IndexLessThan (nuint index);

		[Export ("indexGreaterThanOrEqualToIndex:")]
		nuint IndexGreaterThanOrEqual (nuint index);

		[Export ("indexLessThanOrEqualToIndex:")]
		nuint IndexLessThanOrEqual (nuint index);

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
		[DesignatedInitializer]
		[Export ("initWithItem:typeIdentifier:")]
		NativeHandle Constructor ([NullAllowed] NSObject item, string typeIdentifier);

		[Export ("initWithContentsOfURL:")]
		NativeHandle Constructor (NSUrl fileUrl);

		[Export ("registeredTypeIdentifiers", ArgumentSemantic.Copy)]
		string [] RegisteredTypeIdentifiers { get; }

		[Export ("registerItemForTypeIdentifier:loadHandler:")]
		void RegisterItemForTypeIdentifier (string typeIdentifier, NSItemProviderLoadHandler loadHandler);

		[Export ("hasItemConformingToTypeIdentifier:")]
		bool HasItemConformingTo (string typeIdentifier);

		[Async]
		[Export ("loadItemForTypeIdentifier:options:completionHandler:")]
		void LoadItem (string typeIdentifier, [NullAllowed] NSDictionary options, [NullAllowed] Action<NSObject, NSError> completionHandler);

		[Field ("NSItemProviderPreferredImageSizeKey")]
		NSString PreferredImageSizeKey { get; }

		[Export ("setPreviewImageHandler:")]
		void SetPreviewImageHandler (NSItemProviderLoadHandler handler);

		[Async]
		[Export ("loadPreviewImageWithOptions:completionHandler:")]
		void LoadPreviewImage (NSDictionary options, Action<NSObject, NSError> completionHandler);

		[Field ("NSItemProviderErrorDomain")]
		NSString ErrorDomain { get; }

		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		[Export ("sourceFrame")]
		CGRect SourceFrame { get; }

		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		[Export ("containerFrame")]
		CGRect ContainerFrame { get; }

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("preferredPresentationSize")]
		CGSize PreferredPresentationSize {
			get;
			[NoMac]
			[MacCatalyst (13, 1)]
			set;
		}

		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
		[Export ("registerCloudKitShareWithPreparationHandler:")]
		void RegisterCloudKitShare (CloudKitRegistrationPreparationAction preparationHandler);

		[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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
		[Async, Export ("loadDataRepresentationForTypeIdentifier:completionHandler:")]
		NSProgress LoadDataRepresentation (string typeIdentifier, Action<NSData, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Async, Export ("loadFileRepresentationForTypeIdentifier:completionHandler:")]
		NSProgress LoadFileRepresentation (string typeIdentifier, Action<NSUrl, NSError> completionHandler);

		[MacCatalyst (13, 1)]
		[Async (ResultTypeName = "LoadInPlaceResult"), Export ("loadInPlaceFileRepresentationForTypeIdentifier:completionHandler:")]
		NSProgress LoadInPlaceFileRepresentation (string typeIdentifier, LoadInPlaceFileRepresentationHandler completionHandler);

		[NoWatch, NoTV]
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

		[MacCatalyst (13, 1)]
		[Wrap ("RegisterObject (new Class (type), visibility, loadHandler)")]
		void RegisterObject (Type type, NSItemProviderRepresentationVisibility visibility, RegisterObjectRepresentationLoadHandler loadHandler);

		[MacCatalyst (13, 1)]
		[Export ("canLoadObjectOfClass:")]
		bool CanLoadObject (Class aClass);

		[MacCatalyst (13, 1)]
		[Wrap ("CanLoadObject (new Class (type))")]
		bool CanLoadObject (Type type);

		[MacCatalyst (13, 1)]
		[Async, Export ("loadObjectOfClass:completionHandler:")]
		NSProgress LoadObject (Class aClass, Action<INSItemProviderReading, NSError> completionHandler);

		// NSItemProvider_UIKitAdditions category

		[NoWatch, NoTV]
		[NoMac]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("teamData", ArgumentSemantic.Copy)]
		NSData TeamData { get; set; }

		[NoWatch, NoTV]
		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("preferredPresentationStyle", ArgumentSemantic.Assign)]
		UIPreferredPresentationStyle PreferredPresentationStyle { get; set; }

		// extension methods from CloudKit

		[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registerCKShareWithContainer:allowedSharingOptions:preparationHandler:")]
		void RegisterCKShare (CKContainer container, CKAllowedSharingOptions allowedOptions, Action preparationHandler);

		[NoWatch, NoTV, Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registerCKShare:container:allowedSharingOptions:")]
		void RegisterCKShare (CKShare share, CKContainer container, CKAllowedSharingOptions allowedOptions);

		// from interface UTType (NSItemProvider)

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("initWithContentsOfURL:contentType:openInPlace:coordinated:visibility:")]
		NativeHandle Constructor (NSUrl fileUrl, [NullAllowed] UTType contentType, bool openInPlace, bool coordinated, NSItemProviderRepresentationVisibility visibility);

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registerDataRepresentationForContentType:visibility:loadHandler:")]
		void RegisterDataRepresentation (UTType contentType, NSItemProviderRepresentationVisibility visibility, NSItemProviderUTTypeLoadDelegate loadHandler);

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registerFileRepresentationForContentType:visibility:openInPlace:loadHandler:")]
		void RegisterFileRepresentation (UTType contentType, NSItemProviderRepresentationVisibility visibility, bool openInPlace, NSItemProviderUTTypeLoadDelegate loadHandler);

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registeredContentTypes", ArgumentSemantic.Copy)]
		UTType [] RegisteredContentTypes { get; }

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registeredContentTypesForOpenInPlace", ArgumentSemantic.Copy)]
		UTType [] RegisteredContentTypesForOpenInPlace { get; }

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("registeredContentTypesConformingToContentType:")]
		UTType [] RegisteredContentTypesConforming (UTType contentType);

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("loadDataRepresentationForContentType:completionHandler:")]
		NSProgress LoadDataRepresentation (UTType contentType, ItemProviderDataCompletionHandler completionHandler);

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("loadFileRepresentationForContentType:openInPlace:completionHandler:")]
		NSProgress LoadFileRepresentation (UTType contentType, bool openInPlace, LoadFileRepresentationHandler completionHandler);
	}

	[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	delegate NSProgress NSItemProviderUTTypeLoadDelegate ([BlockCallback] ItemProviderDataCompletionHandler completionHandler);
	[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
	delegate void LoadFileRepresentationHandler (NSUrl fileUrl, bool openInPlace, NSError error);
	delegate NSProgress RegisterFileRepresentationLoadHandler ([BlockCallback] RegisterFileRepresentationCompletionHandler completionHandler);
	delegate void RegisterFileRepresentationCompletionHandler (NSUrl fileUrl, bool coordinated, NSError error);
	delegate void ItemProviderDataCompletionHandler (NSData data, NSError error);
	delegate NSProgress RegisterDataRepresentationLoadHandler ([BlockCallback] ItemProviderDataCompletionHandler completionHandler);
	delegate void LoadInPlaceFileRepresentationHandler (NSUrl fileUrl, bool isInPlace, NSError error);
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
		[Static, Abstract]
		[Export ("readableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
		string [] ReadableTypeIdentifiers { get; }

		[Static, Abstract]
		[Export ("objectWithItemProviderData:typeIdentifier:error:")]
		[return: NullAllowed]
		INSItemProviderReading GetObject (NSData data, string typeIdentifier, [NullAllowed] out NSError outError);
	}

	interface INSItemProviderWriting { }

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

		[Export ("writableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
		// 'WritableTypeIdentifiers' is a nicer name, but there's a static property with that name.
		string [] WritableTypeIdentifiersForItemProvider { get; }

		[Export ("itemProviderVisibilityForRepresentationWithTypeIdentifier:")]
		// 'GetItemProviderVisibility' is a nicer name, but there's a static method with that name.
		NSItemProviderRepresentationVisibility GetItemProviderVisibilityForTypeIdentifier (string typeIdentifier);

		[Abstract]
		[Async, Export ("loadDataWithTypeIdentifier:forItemProviderCompletionHandler:")]
		[return: NullAllowed]
		NSProgress LoadData (string typeIdentifier, Action<NSData, NSError> completionHandler);
	}

	[Static]
	[MacCatalyst (13, 1)]
	partial interface NSJavaScriptExtension {
		[Field ("NSExtensionJavaScriptPreprocessingResultsKey")]
		NSString PreprocessingResultsKey { get; }

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

		[Export ("addIndex:")]
		void Add (nuint index);

		[Export ("removeIndex:")]
		void Remove (nuint index);

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
	[NoWatch]
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

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSNetServiceDelegate Delegate { get; set; }

#if NET
		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, NSString forMode);

		// For consistency with other APIs (NSUrlConnection) we call this Unschedule
		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, NSString forMode);
#else
		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, string forMode);

		// For consistency with other APIs (NSUrlConnection) we call this Unschedule
		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, string forMode);
#endif
		[Wrap ("Schedule (aRunLoop, forMode.GetConstant ()!)")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

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
		[NoWatch]
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

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	interface NSNetServiceDelegate {
		[Export ("netServiceWillPublish:")]
		void WillPublish (NSNetService sender);

		[Export ("netServiceDidPublish:")]
		void Published (NSNetService sender);

		[Export ("netService:didNotPublish:"), EventArgs ("NSNetServiceError")]
		void PublishFailure (NSNetService sender, NSDictionary errors);

		[Export ("netServiceWillResolve:")]
		void WillResolve (NSNetService sender);

		[Export ("netServiceDidResolveAddress:")]
		void AddressResolved (NSNetService sender);

		[Export ("netService:didNotResolve:"), EventArgs ("NSNetServiceError")]
		void ResolveFailure (NSNetService sender, NSDictionary errors);

		[Export ("netServiceDidStop:")]
		void Stopped (NSNetService sender);

		[Export ("netService:didUpdateTXTRecordData:"), EventArgs ("NSNetServiceData")]
		void UpdatedTxtRecordData (NSNetService sender, NSData data);

		[Export ("netService:didAcceptConnectionWithInputStream:outputStream:"), EventArgs ("NSNetServiceConnection")]
		void DidAcceptConnection (NSNetService sender, NSInputStream inputStream, NSOutputStream outputStream);
	}

	[Deprecated (PlatformName.MacOSX, 12, 0, message: "Use the Network.framework instead.")]
	[Deprecated (PlatformName.iOS, 15, 0, message: "Use the Network.framework instead.")]
	[Deprecated (PlatformName.TvOS, 15, 0, message: "Use the Network.framework instead.")]
	[NoWatch]
	[MacCatalyst (13, 1)]
	[Deprecated (PlatformName.MacCatalyst, 15, 0, message: "Use the Network.framework instead.")]
	[BaseType (typeof (NSObject),
		   Delegates = new string [] { "WeakDelegate" },
		   Events = new Type [] { typeof (NSNetServiceBrowserDelegate) })]
	interface NSNetServiceBrowser {
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSNetServiceBrowserDelegate Delegate { get; set; }

#if NET
		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, NSString forMode);

		// For consistency with other APIs (NSUrlConnection) we call this Unschedule
		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, NSString forMode);
#else
		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, string forMode);

		// For consistency with other APIs (NSUrlConnection) we call this Unschedule
		[Export ("removeFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, string forMode);
#endif

		[Wrap ("Schedule (aRunLoop, forMode.GetConstant ()!)")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

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

	[NoWatch]
	[MacCatalyst (13, 1)]
	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	interface NSNetServiceBrowserDelegate {
		[Export ("netServiceBrowserWillSearch:")]
		void SearchStarted (NSNetServiceBrowser sender);

		[Export ("netServiceBrowserDidStopSearch:")]
		void SearchStopped (NSNetServiceBrowser sender);

		[Export ("netServiceBrowser:didNotSearch:"), EventArgs ("NSNetServiceError")]
		void NotSearched (NSNetServiceBrowser sender, NSDictionary errors);

		[Export ("netServiceBrowser:didFindDomain:moreComing:"), EventArgs ("NSNetDomain")]
		void FoundDomain (NSNetServiceBrowser sender, string domain, bool moreComing);

		[Export ("netServiceBrowser:didFindService:moreComing:"), EventArgs ("NSNetService")]
		void FoundService (NSNetServiceBrowser sender, NSNetService service, bool moreComing);

		[Export ("netServiceBrowser:didRemoveDomain:moreComing:"), EventArgs ("NSNetDomain")]
		void DomainRemoved (NSNetServiceBrowser sender, string domain, bool moreComing);

		[Export ("netServiceBrowser:didRemoveService:moreComing:"), EventArgs ("NSNetService")]
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
	[NoWatch]
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

	[NoiOS]
	[NoTV]
	[NoWatch]
	[MacCatalyst (15, 0)]
	[BaseType (typeof (NSNotificationCenter))]
	interface NSDistributedNotificationCenter {
		[Static]
		[Export ("defaultCenter")]
#if NET
		NSDistributedNotificationCenter DefaultCenter { get; }
#else
		NSDistributedNotificationCenter GetDefaultCenter ();

		[Static]
		[Advice ("Use 'GetDefaultCenter ()' for a strongly typed version.")]
		[Wrap ("GetDefaultCenter ()")]
		NSObject DefaultCenter { get; }
#endif

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
#if !NET
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle, NSNotificationCoalescing coalesceMask, [NullAllowed] string [] modes);
#else
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle, NSNotificationCoalescing coalesceMask, [NullAllowed] NSString [] modes);

		[Wrap ("EnqueueNotification (notification, postingStyle, coalesceMask, modes?.GetConstants ())")]
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle, NSNotificationCoalescing coalesceMask, [NullAllowed] NSRunLoopMode [] modes);
#endif

		[Export ("dequeueNotificationsMatching:coalesceMask:")]
		void DequeueNotificationsMatchingcoalesceMask (NSNotification notification, NSNotificationCoalescing coalesceMask);
	}

	[BaseType (typeof (NSObject))]
	// init returns NIL
	[DisableDefaultCtor]
	partial interface NSValue : NSSecureCoding, NSCopying {
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
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

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithCMTime:")]
		NSValue FromCMTime (CMTime time);

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("CMTimeValue")]
		CMTime CMTimeValue { get; }

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithCMTimeMapping:")]
		NSValue FromCMTimeMapping (CMTimeMapping timeMapping);

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Export ("CMTimeMappingValue")]
		CMTimeMapping CMTimeMappingValue { get; }

		[Watch (6, 0)]
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithCMTimeRange:")]
		NSValue FromCMTimeRange (CMTimeRange timeRange);

		[Watch (6, 0)]
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

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("CGAffineTransformValue")]
		CoreGraphics.CGAffineTransform CGAffineTransformValue { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("UIEdgeInsetsValue")]
		UIEdgeInsets UIEdgeInsetsValue { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("directionalEdgeInsetsValue")]
		NSDirectionalEdgeInsets DirectionalEdgeInsetsValue { get; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("valueWithCGAffineTransform:")]
		[Static]
		NSValue FromCGAffineTransform (CoreGraphics.CGAffineTransform tran);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("valueWithUIEdgeInsets:")]
		[Static]
		NSValue FromUIEdgeInsets (UIEdgeInsets insets);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("valueWithDirectionalEdgeInsets:")]
		NSValue FromDirectionalEdgeInsets (NSDirectionalEdgeInsets insets);

		[Export ("valueWithUIOffset:")]
		[Static]
		[NoMac]
		[MacCatalyst (13, 1)]
		NSValue FromUIOffset (UIOffset insets);

		[Export ("UIOffsetValue")]
		[NoMac]
		[MacCatalyst (13, 1)]
		UIOffset UIOffsetValue { get; }
		// from UIGeometry.h - those are in iOS8 only (even if the header is silent about them)
		// and not in OSX 10.10

		[Export ("CGVectorValue")]
		[NoMac]
		[MacCatalyst (13, 1)]
		CGVector CGVectorValue { get; }

		[Static, Export ("valueWithCGVector:")]
		[NoMac]
		[MacCatalyst (13, 1)]
		NSValue FromCGVector (CGVector vector);

		// Maybe we should include this inside mapkit.cs instead (it's a partial interface, so that's trivial)?
		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithMKCoordinate:")]
		NSValue FromMKCoordinate (CoreLocation.CLLocationCoordinate2D coordinate);

		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithMKCoordinateSpan:")]
		NSValue FromMKCoordinateSpan (MapKit.MKCoordinateSpan coordinateSpan);

		[MacCatalyst (13, 1)]
		[Export ("MKCoordinateValue")]
		CoreLocation.CLLocationCoordinate2D CoordinateValue { get; }

		[MacCatalyst (13, 1)]
		[Export ("MKCoordinateSpanValue")]
		MapKit.MKCoordinateSpan CoordinateSpanValue { get; }

#if !WATCH
		[Export ("valueWithCATransform3D:")]
		[Static]
		NSValue FromCATransform3D (CoreAnimation.CATransform3D transform);

		[Export ("CATransform3DValue")]
		CoreAnimation.CATransform3D CATransform3DValue { get; }
#endif

		[iOS (16, 0)]
		[Mac (13, 0)]
		[MacCatalyst (16, 0)]
		[TV (16, 0)]
		[Watch (9, 0)]
		[Export ("CMVideoDimensionsValue")]
		CMVideoDimensions CMVideoDimensionsValue { get; }

		[iOS (16, 0)]
		[Mac (13, 0)]
		[MacCatalyst (16, 0)]
		[TV (16, 0)]
		[Watch (9, 0)]
		[Export ("valueWithCMVideoDimensions:")]
		[Static]
		NSValue FromCMVideoDimensions (CMVideoDimensions value);

		#region SceneKit Additions

		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithSCNVector3:")]
		NSValue FromVector (SCNVector3 vector);

		[MacCatalyst (13, 1)]
		[Export ("SCNVector3Value")]
		SCNVector3 Vector3Value { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithSCNVector4:")]
		NSValue FromVector (SCNVector4 vector);

		[MacCatalyst (13, 1)]
		[Export ("SCNVector4Value")]
		SCNVector4 Vector4Value { get; }

		[MacCatalyst (13, 1)]
		[Static, Export ("valueWithSCNMatrix4:")]
		NSValue FromSCNMatrix4 (SCNMatrix4 matrix);

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

#if IOS && !NET
		[Notification]
		[Obsolete ("Use 'NSUserDefaults.SizeLimitExceededNotification' instead.")]
		[Field ("NSUserDefaultsSizeLimitExceededNotification")]
		NSString SizeLimitExceededNotification { get; }

		[Notification]
		[Obsolete ("Use 'NSUserDefaults.DidChangeAccountsNotification' instead.")]
		[Field ("NSUbiquitousUserDefaultsDidChangeAccountsNotification")]
		NSString DidChangeAccountsNotification { get; }

		[Notification]
		[Obsolete ("Use 'NSUserDefaults.CompletedInitialSyncNotification' instead.")]
		[Field ("NSUbiquitousUserDefaultsCompletedInitialSyncNotification")]
		NSString CompletedInitialSyncNotification { get; }

		[Notification]
		[Obsolete ("Use 'NSUserDefaults.DidChangeNotification' instead.")]
		[Field ("NSUserDefaultsDidChangeNotification")]
		NSString UserDefaultsDidChangeNotification { get; }
#endif

		[Field ("NSNegateBooleanTransformerName")]
		NSString BooleanTransformerName { get; }

		[Field ("NSIsNilTransformerName")]
		NSString IsNilTransformerName { get; }

		[Field ("NSIsNotNilTransformerName")]
		NSString IsNotNilTransformerName { get; }

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Field ("NSUnarchiveFromDataTransformerName")]
		NSString UnarchiveFromDataTransformerName { get; }

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Field ("NSKeyedUnarchiveFromDataTransformerName")]
		NSString KeyedUnarchiveFromDataTransformerName { get; }

		[Watch (5, 0), TV (12, 0), iOS (12, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSSecureUnarchiveFromDataTransformerName")]
		NSString SecureUnarchiveFromDataTransformerName { get; }
	}

	[Watch (5, 0), TV (12, 0), iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSValueTransformer))]
	interface NSSecureUnarchiveFromDataTransformer {
		[Static]
		[Export ("allowedTopLevelClasses", ArgumentSemantic.Copy)]
		Class [] AllowedTopLevelClasses { get; }

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

		[Internal] // Equals(object) or IEquatable<T>'s Equals(NSNumber)
		[Export ("isEqualToNumber:")]
		bool IsEqualToNumber (NSNumber number);

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

		[DesignatedInitializer]
		[Export ("initWithInteger:")]
		NativeHandle Constructor (nint value);

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

		[Static]
		[Export ("numberWithLong:")]
		NSNumber FromLong (nint value);
		//
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

		[Static]
		[Export ("numberWithInteger:")]
		NSNumber FromNInt (nint value);

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

		[Static]
		[Export ("localizedStringFromNumber:numberStyle:")]
		string LocalizedStringFromNumbernumberStyle (NSNumber num, NSNumberFormatterStyle nstyle);

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

		[Export ("lenient")]
		bool Lenient { [Bind ("isLenient")] get; set; }

		[Export ("usesSignificantDigits")]
		bool UsesSignificantDigits { get; set; }

		[Export ("minimumSignificantDigits")]
		nuint MinimumSignificantDigits { get; set; }

		[Export ("maximumSignificantDigits")]
		nuint MaximumSignificantDigits { get; set; }

		[Export ("partialStringValidationEnabled")]
		bool PartialStringValidationEnabled { [Bind ("isPartialStringValidationEnabled")] get; set; }
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

		[Export ("decimalNumberByRaisingToPower:")]
		NSDecimalNumber RaiseTo (nuint power);

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

		[Export ("isExecuting")]
		bool IsExecuting { get; }

		[Export ("isFinished")]
		bool IsFinished { get; }

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

	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSPort : NSCoding, NSCopying {
		[Static, Export ("port")]
		NSPort Create ();

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("isValid")]
		bool IsValid { get; }

		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate"), NullAllowed]
		[Protocolize]
		NSPortDelegate Delegate { get; set; }

		[Export ("scheduleInRunLoop:forMode:")]
		void ScheduleInRunLoop (NSRunLoop runLoop, NSString runLoopMode);

		[Wrap ("ScheduleInRunLoop (runLoop, runLoopMode.GetConstant ()!)")]
		void ScheduleInRunLoop (NSRunLoop runLoop, NSRunLoopMode runLoopMode);

		[Export ("removeFromRunLoop:forMode:")]
		void RemoveFromRunLoop (NSRunLoop runLoop, NSString runLoopMode);

		[Wrap ("RemoveFromRunLoop (runLoop, runLoopMode.GetConstant ()!)")]
		void RemoveFromRunLoop (NSRunLoop runLoop, NSRunLoopMode runLoopMode);

		// Disable warning for NSMutableArray
#pragma warning disable 618
		[Export ("sendBeforeDate:components:from:reserved:")]
		bool SendBeforeDate (NSDate limitDate, [NullAllowed] NSMutableArray components, [NullAllowed] NSPort receivePort, nuint headerSpaceReserved);

		[Export ("sendBeforeDate:msgid:components:from:reserved:")]
		bool SendBeforeDate (NSDate limitDate, nuint msgID, [NullAllowed] NSMutableArray components, [NullAllowed] NSPort receivePort, nuint headerSpaceReserved);
#pragma warning restore 618
	}

	[Model, BaseType (typeof (NSObject))]
	[Protocol]
	interface NSPortDelegate {
		[NoMacCatalyst]
		[Export ("handlePortMessage:")]
		void MessageReceived (NSPortMessage message);
	}

	[BaseType (typeof (NSObject))]
	[MacCatalyst (15, 0)]
	interface NSPortMessage {
		[NoiOS]
		[NoWatch]
		[NoTV]
		[MacCatalyst (15, 0)]
		[DesignatedInitializer]
		[Export ("initWithSendPort:receivePort:components:")]
		NativeHandle Constructor ([NullAllowed] NSPort sendPort, [NullAllowed] NSPort recvPort, [NullAllowed] NSArray components);

		[NullAllowed]
		[NoiOS]
		[NoWatch]
		[NoTV]
		[MacCatalyst (15, 0)]
		[Export ("components")]
		NSArray Components { get; }

		// Apple started refusing applications that use those selectors (desk #63237)
		// The situation is a bit confusing since NSPortMessage.h is not part of iOS SDK - 
		// but the type is used (from NSPort[Delegate]) but not _itself_ documented
		// The selectors Apple *currently* dislike are removed from the iOS build
		[NoiOS]
		[NoWatch]
		[NoTV]
		[MacCatalyst (15, 0)]
		[Export ("sendBeforeDate:")]
		bool SendBefore (NSDate date);

		[NullAllowed]
		[NoiOS]
		[NoWatch]
		[NoTV]
		[MacCatalyst (15, 0)]
		[Export ("receivePort")]
		NSPort ReceivePort { get; }

		[NullAllowed]
		[NoiOS]
		[NoWatch]
		[NoTV]
		[MacCatalyst (15, 0)]
		[Export ("sendPort")]
		NSPort SendPort { get; }

		[NoiOS]
		[NoWatch]
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

		[Wrap ("WeakDelegate"), NullAllowed]
		[Protocolize]
		NSMachPortDelegate Delegate { get; set; }
	}

	[Model, BaseType (typeof (NSPortDelegate))]
	[Protocol]
	interface NSMachPortDelegate {
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
		[NoWatch]
		[NoTV]
		[Export ("enableSuddenTermination")]
		void EnableSuddenTermination ();

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("disableSuddenTermination")]
		void DisableSuddenTermination ();

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("enableAutomaticTermination:")]
		void EnableAutomaticTermination (string reason);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("disableAutomaticTermination:")]
		void DisableAutomaticTermination (string reason);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("automaticTerminationSupportEnabled")]
		bool AutomaticTerminationSupportEnabled { get; set; }

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("performExpiringActivityWithReason:usingBlock:")]
		void PerformExpiringActivity (string reason, Action<bool> block);

		[TV (15, 0)]
		[MacCatalyst (13, 1)]
		[Export ("lowPowerModeEnabled")]
		bool LowPowerModeEnabled { [Bind ("isLowPowerModeEnabled")] get; }

		[Mac (12, 0)]
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
		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("macCatalystApp")]
		bool IsMacCatalystApplication { [Bind ("isMacCatalystApp")] get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("iOSAppOnMac")]
		bool IsiOSApplicationOnMac { [Bind ("isiOSAppOnMac")] get; }
		#endregion
	}

	[NoWatch]
	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[Category]
	[BaseType (typeof (NSProcessInfo))]
	interface NSProcessInfo_NSUserInformation {
		[Export ("userName")]
		string GetUserName ();

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

		[Export ("cancellable")]
		bool Cancellable { [Bind ("isCancellable")] get; set; }

		[Export ("pausable")]
		bool Pausable { [Bind ("isPausable")] get; set; }

		[Export ("cancelled")]
		bool Cancelled { [Bind ("isCancelled")] get; }

		[Export ("paused")]
		bool Paused { [Bind ("isPaused")] get; }

		[Export ("setCancellationHandler:")]
		void SetCancellationHandler (Action handler);

		[Export ("setPausingHandler:")]
		void SetPauseHandler (Action handler);

		[MacCatalyst (13, 1)]
		[Export ("setResumingHandler:")]
		void SetResumingHandler (Action handler);

		[Export ("setUserInfoObject:forKey:")]
		void SetUserInfo ([NullAllowed] NSObject obj, NSString key);

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
		[NoWatch]
		[NoTV]
		[Export ("publish")]
		void Publish ();

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("unpublish")]
		void Unpublish ();

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("setAcknowledgementHandler:forAppBundleIdentifier:")]
		void SetAcknowledgementHandler (Action<bool> acknowledgementHandler, string appBundleIdentifier);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Static, Export ("addSubscriberForFileURL:withPublishingHandler:")]
		NSObject AddSubscriberForFile (NSUrl url, Action<NSProgress> publishingHandler);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Static, Export ("removeSubscriber:")]
		void RemoveSubscriber (NSObject subscriber);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("acknowledgeWithSuccess:")]
		void AcknowledgeWithSuccess (bool success);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("old")]
		bool Old { [Bind ("isOld")] get; }

		[Field ("NSProgressKindFile")]
		NSString KindFile { get; }

		[Field ("NSProgressEstimatedTimeRemainingKey")]
		NSString EstimatedTimeRemainingKey { get; }

		[Field ("NSProgressThroughputKey")]
		NSString ThroughputKey { get; }

		[Field ("NSProgressFileOperationKindKey")]
		NSString FileOperationKindKey { get; }

		[Field ("NSProgressFileOperationKindDownloading")]
		NSString FileOperationKindDownloading { get; }

		[Field ("NSProgressFileOperationKindDecompressingAfterDownloading")]
		NSString FileOperationKindDecompressingAfterDownloading { get; }

		[Field ("NSProgressFileOperationKindReceiving")]
		NSString FileOperationKindReceiving { get; }

		[Field ("NSProgressFileOperationKindCopying")]
		NSString FileOperationKindCopying { get; }

		[Watch (7, 4), TV (14, 5), Mac (11, 3), iOS (14, 5)]
		[MacCatalyst (14, 5)]
		[Field ("NSProgressFileOperationKindUploading")]
		NSString FileOperationKindUploading { get; }

		[Field ("NSProgressFileURLKey")]
		NSString FileURLKey { get; }

		[Field ("NSProgressFileTotalCountKey")]
		NSString FileTotalCountKey { get; }

		[Field ("NSProgressFileCompletedCountKey")]
		NSString FileCompletedCountKey { get; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Field ("NSProgressFileOperationKindDuplicating")]
		NSString FileOperationKindDuplicatingKey { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Field ("NSProgressFileAnimationImageKey")]
		NSString FileAnimationImageKey { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Field ("NSProgressFileAnimationImageOriginalRectKey")]
		NSString FileAnimationImageOriginalRectKey { get; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Field ("NSProgressFileIconKey")]
		NSString FileIconKey { get; }

		[MacCatalyst (13, 1)]
		[Async, Export ("performAsCurrentWithPendingUnitCount:usingBlock:")]
		void PerformAsCurrent (long unitCount, Action work);

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
#if NET
		[Abstract]
#endif
		[Export ("progress")]
		NSProgress Progress { get; }
	}

	[BaseType (typeof (NSMutableData))]
	interface NSPurgeableData : NSSecureCoding, NSMutableCopying, NSDiscardableContent {
	}

	[Protocol]
	interface NSDiscardableContent {
		[Abstract]
		[Export ("beginContentAccess")]
		bool BeginContentAccess ();

		[Abstract]
		[Export ("endContentAccess")]
		void EndContentAccess ();

		[Abstract]
		[Export ("discardContentIfPossible")]
		void DiscardContentIfPossible ();

		[Abstract]
		[Export ("isContentDiscarded")]
		bool IsContentDiscarded { get; }
	}

	delegate void NSFileCoordinatorWorkerRW (NSUrl newReadingUrl, NSUrl newWritingUrl);

	interface INSFilePresenter { }

	[BaseType (typeof (NSObject))]
	interface NSFileCoordinator {
		[Static, Export ("addFilePresenter:")]
		[PostGet ("FilePresenters")]
		void AddFilePresenter ([Protocolize] NSFilePresenter filePresenter);

		[Static]
		[Export ("removeFilePresenter:")]
		[PostGet ("FilePresenters")]
		void RemoveFilePresenter ([Protocolize] NSFilePresenter filePresenter);

		[Static]
		[Export ("filePresenters", ArgumentSemantic.Copy)]
		[Protocolize]
		NSFilePresenter [] FilePresenters { get; }

		[DesignatedInitializer]
		[Export ("initWithFilePresenter:")]
		NativeHandle Constructor ([NullAllowed] INSFilePresenter filePresenterOrNil);

#if !NET
		[Obsolete ("Use '.ctor(INSFilePresenter)' instead.")]
		[Wrap ("this (filePresenterOrNil as INSFilePresenter)")]
		NativeHandle Constructor ([NullAllowed] NSFilePresenter filePresenterOrNil);
#endif

		[Export ("coordinateReadingItemAtURL:options:error:byAccessor:")]
		void CoordinateRead (NSUrl itemUrl, NSFileCoordinatorReadingOptions options, out NSError error, /* non null */ Action<NSUrl> worker);

		[Export ("coordinateWritingItemAtURL:options:error:byAccessor:")]
		void CoordinateWrite (NSUrl url, NSFileCoordinatorWritingOptions options, out NSError error, /* non null */ Action<NSUrl> worker);

		[Export ("coordinateReadingItemAtURL:options:writingItemAtURL:options:error:byAccessor:")]
		void CoordinateReadWrite (NSUrl readingURL, NSFileCoordinatorReadingOptions readingOptions, NSUrl writingURL, NSFileCoordinatorWritingOptions writingOptions, out NSError error, /* non null */ NSFileCoordinatorWorkerRW readWriteWorker);

		[Export ("coordinateWritingItemAtURL:options:writingItemAtURL:options:error:byAccessor:")]
		void CoordinateWriteWrite (NSUrl writingURL, NSFileCoordinatorWritingOptions writingOptions, NSUrl writingURL2, NSFileCoordinatorWritingOptions writingOptions2, out NSError error, /* non null */ NSFileCoordinatorWorkerRW writeWriteWorker);

#if !NET
		[Obsolete ("Use 'CoordinateBatch' instead.")]
		[Wrap ("CoordinateBatch (readingURLs, readingOptions, writingURLs, writingOptions, out error, batchHandler)", IsVirtual = true)]
		void CoordinateBatc (NSUrl [] readingURLs, NSFileCoordinatorReadingOptions readingOptions, NSUrl [] writingURLs, NSFileCoordinatorWritingOptions writingOptions, out NSError error, /* non null */ Action batchHandler);
#endif

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

		[NoWatch, NoTV]
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
		[Field ("NSFileType")]
		NSString NSFileType { get; }

		[Field ("NSFileTypeDirectory")]
		NSString TypeDirectory { get; }

		[Field ("NSFileTypeRegular")]
		NSString TypeRegular { get; }

		[Field ("NSFileTypeSymbolicLink")]
		NSString TypeSymbolicLink { get; }

		[Field ("NSFileTypeSocket")]
		NSString TypeSocket { get; }

		[Field ("NSFileTypeCharacterSpecial")]
		NSString TypeCharacterSpecial { get; }

		[Field ("NSFileTypeBlockSpecial")]
		NSString TypeBlockSpecial { get; }

		[Field ("NSFileTypeUnknown")]
		NSString TypeUnknown { get; }

		[Field ("NSFileSize")]
		NSString Size { get; }

		[Field ("NSFileModificationDate")]
		NSString ModificationDate { get; }

		[Field ("NSFileReferenceCount")]
		NSString ReferenceCount { get; }

		[Field ("NSFileDeviceIdentifier")]
		NSString DeviceIdentifier { get; }

		[Field ("NSFileOwnerAccountName")]
		NSString OwnerAccountName { get; }

		[Field ("NSFileGroupOwnerAccountName")]
		NSString GroupOwnerAccountName { get; }

		[Field ("NSFilePosixPermissions")]
		NSString PosixPermissions { get; }

		[Field ("NSFileSystemNumber")]
		NSString SystemNumber { get; }

		[Field ("NSFileSystemFileNumber")]
		NSString SystemFileNumber { get; }

		[Field ("NSFileExtensionHidden")]
		NSString ExtensionHidden { get; }

		[Field ("NSFileHFSCreatorCode")]
		NSString HfsCreatorCode { get; }

		[Field ("NSFileHFSTypeCode")]
		NSString HfsTypeCode { get; }

		[Field ("NSFileImmutable")]
		NSString Immutable { get; }

		[Field ("NSFileAppendOnly")]
		NSString AppendOnly { get; }

		[Field ("NSFileCreationDate")]
		NSString CreationDate { get; }

		[Field ("NSFileOwnerAccountID")]
		NSString OwnerAccountID { get; }

		[Field ("NSFileGroupOwnerAccountID")]
		NSString GroupOwnerAccountID { get; }

		[Field ("NSFileBusy")]
		NSString Busy { get; }

		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSFileProtectionKey")]
		NSString FileProtectionKey { get; }

		[Obsolete ("Use the 'NSFileProtectionType' instead.")]
		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSFileProtectionNone")]
		NSString FileProtectionNone { get; }

		[Obsolete ("Use the 'NSFileProtectionType' instead.")]
		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSFileProtectionComplete")]
		NSString FileProtectionComplete { get; }

		[Obsolete ("Use the 'NSFileProtectionType' instead.")]
		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSFileProtectionCompleteUnlessOpen")]
		NSString FileProtectionCompleteUnlessOpen { get; }

		[Obsolete ("Use the 'NSFileProtectionType' instead.")]
		[Mac (11, 0)]
		[MacCatalyst (13, 1)]
		[Field ("NSFileProtectionCompleteUntilFirstUserAuthentication")]
		NSString FileProtectionCompleteUntilFirstUserAuthentication { get; }

		[Field ("NSFileSystemSize")]
		NSString SystemSize { get; }

		[Field ("NSFileSystemFreeSize")]
		NSString SystemFreeSize { get; }

		[Field ("NSFileSystemNodes")]
		NSString SystemNodes { get; }

		[Field ("NSFileSystemFreeNodes")]
		NSString SystemFreeNodes { get; }

		[Static, Export ("defaultManager", ArgumentSemantic.Strong)]
		NSFileManager DefaultManager { get; }

		[Export ("delegate", ArgumentSemantic.Assign)]
		[NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		[NullAllowed]
		NSFileManagerDelegate Delegate { get; set; }

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

		[NoWatch]
		[NoTV]
		[NoiOS]
		[NoMacCatalyst]
		[Async]
		[Export ("unmountVolumeAtURL:options:completionHandler:")]
		void UnmountVolume (NSUrl url, NSFileManagerUnmountOptions mask, Action<NSError> completionHandler);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Async, Export ("getFileProviderServicesForItemAtURL:completionHandler:")]
		void GetFileProviderServices (NSUrl url, Action<NSDictionary<NSString, NSFileProviderService>, NSError> completionHandler);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSFileManagerDelegate {
		[Export ("fileManager:shouldCopyItemAtPath:toPath:")]
		bool ShouldCopyItemAtPath (NSFileManager fm, NSString srcPath, NSString dstPath);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldCopyItemAtURL:toURL:")]
		bool ShouldCopyItemAtUrl (NSFileManager fm, NSUrl srcUrl, NSUrl dstUrl);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldLinkItemAtURL:toURL:")]
		bool ShouldLinkItemAtUrl (NSFileManager fileManager, NSUrl srcUrl, NSUrl dstUrl);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldMoveItemAtURL:toURL:")]
		bool ShouldMoveItemAtUrl (NSFileManager fileManager, NSUrl srcUrl, NSUrl dstUrl);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldProceedAfterError:copyingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorCopyingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldProceedAfterError:linkingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorLinkingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldProceedAfterError:movingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorMovingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldRemoveItemAtURL:")]
		bool ShouldRemoveItemAtUrl (NSFileManager fileManager, NSUrl url);

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("fileManager:shouldProceedAfterError:removingItemAtURL:")]
		bool ShouldProceedAfterErrorRemovingItem (NSFileManager fileManager, NSError error, NSUrl url);

		[Export ("fileManager:shouldProceedAfterError:copyingItemAtPath:toPath:")]
		bool ShouldProceedAfterErrorCopyingItem (NSFileManager fileManager, NSError error, string srcPath, string dstPath);

		[Export ("fileManager:shouldMoveItemAtPath:toPath:")]
		bool ShouldMoveItemAtPath (NSFileManager fileManager, string srcPath, string dstPath);

		[Export ("fileManager:shouldProceedAfterError:movingItemAtPath:toPath:")]
		bool ShouldProceedAfterErrorMovingItem (NSFileManager fileManager, NSError error, string srcPath, string dstPath);

		[Export ("fileManager:shouldLinkItemAtPath:toPath:")]
		bool ShouldLinkItemAtPath (NSFileManager fileManager, string srcPath, string dstPath);

		[Export ("fileManager:shouldProceedAfterError:linkingItemAtPath:toPath:")]
		bool ShouldProceedAfterErrorLinkingItem (NSFileManager fileManager, NSError error, string srcPath, string dstPath);

		[Export ("fileManager:shouldRemoveItemAtPath:")]
		bool ShouldRemoveItemAtPath (NSFileManager fileManager, string path);

		[Export ("fileManager:shouldProceedAfterError:removingItemAtPath:")]
		bool ShouldProceedAfterErrorRemovingItem (NSFileManager fileManager, NSError error, string path);
	}

	[Category]
	[BaseType (typeof (NSFileManager))]
	interface NSFileManager_NSUserInformation {

		[NoWatch]
		[NoTV]
		[NoiOS]
		[NoMacCatalyst]
		[Export ("homeDirectoryForCurrentUser")]
		NSUrl GetHomeDirectoryForCurrentUser ();

		[MacCatalyst (13, 1)]
		[Export ("temporaryDirectory")]
		NSUrl GetTemporaryDirectory ();

		[NoWatch]
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
		[Abstract]
		[Export ("presentedItemURL", ArgumentSemantic.Retain)]
		[NullAllowed]
#if NET
		NSUrl PresentedItemUrl { get; }
#else
		NSUrl PresentedItemURL { get; }
#endif

		[Abstract]
		[Export ("presentedItemOperationQueue", ArgumentSemantic.Retain)]
#if NET
		NSOperationQueue PresentedItemOperationQueue { get; }
#else
		NSOperationQueue PesentedItemOperationQueue { get; }
#endif

#if DOUBLE_BLOCKS
		[Export ("relinquishPresentedItemToReader:")]
		void RelinquishPresentedItemToReader (NSFilePresenterReacquirer readerAction);

		[Export ("relinquishPresentedItemToWriter:")]
		void RelinquishPresentedItemToWriter (NSFilePresenterReacquirer writerAction);
#endif

		[Export ("savePresentedItemChangesWithCompletionHandler:")]
		void SavePresentedItemChanges (Action<NSError> completionHandler);

		[Export ("accommodatePresentedItemDeletionWithCompletionHandler:")]
		void AccommodatePresentedItemDeletion (Action<NSError> completionHandler);

		[Export ("presentedItemDidMoveToURL:")]
		void PresentedItemMoved (NSUrl newURL);

		[Export ("presentedItemDidChange")]
		void PresentedItemChanged ();

		[Export ("presentedItemDidGainVersion:")]
		void PresentedItemGainedVersion (NSFileVersion version);

		[Export ("presentedItemDidLoseVersion:")]
		void PresentedItemLostVersion (NSFileVersion version);

		[Export ("presentedItemDidResolveConflictVersion:")]
		void PresentedItemResolveConflictVersion (NSFileVersion version);

		[Export ("accommodatePresentedSubitemDeletionAtURL:completionHandler:")]
		void AccommodatePresentedSubitemDeletion (NSUrl url, Action<NSError> completionHandler);

		[Export ("presentedSubitemDidAppearAtURL:")]
		void PresentedSubitemAppeared (NSUrl atUrl);

		[Export ("presentedSubitemAtURL:didMoveToURL:")]
		void PresentedSubitemMoved (NSUrl oldURL, NSUrl newURL);

		[Export ("presentedSubitemDidChangeAtURL:")]
		void PresentedSubitemChanged (NSUrl url);

		[Export ("presentedSubitemAtURL:didGainVersion:")]
		void PresentedSubitemGainedVersion (NSUrl url, NSFileVersion version);

		[Export ("presentedSubitemAtURL:didLoseVersion:")]
		void PresentedSubitemLostVersion (NSUrl url, NSFileVersion version);

		[Export ("presentedSubitemAtURL:didResolveConflictVersion:")]
		void PresentedSubitemResolvedConflictVersion (NSUrl url, NSFileVersion version);

		[NoWatch, NoTV]
		[MacCatalyst (13, 1)]
		[Export ("presentedItemDidChangeUbiquityAttributes:")]
		void PresentedItemChangedUbiquityAttributes (NSSet<NSString> attributes);

		[NoWatch, NoTV]
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

		[Export ("conflict")]
		bool IsConflict { [Bind ("isConflict")] get; }

		[Export ("resolved")]
		bool Resolved { [Bind ("isResolved")] get; set; }

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
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
		[Async]
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
		[NoWatch]
		[NoTV]
		[return: NullAllowed]
		[Static]
		[Export ("addVersionOfItemAtURL:withContentsOfURL:options:error:")]
		NSFileVersion AddVersion (NSUrl url, NSUrl contentsURL, NSFileVersionAddingOptions options, out NSError outError);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
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

		[NoWatch, NoTV]
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

		[Export ("isDirectory")]
		bool IsDirectory { get; }

		[Export ("isRegularFile")]
		bool IsRegularFile { get; }

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

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
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
		void SkipDescendents ();

		[NoMac]
		[MacCatalyst (13, 1)]
		[Export ("level")]
		nint Level { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("isEnumeratingDirectoryPostOrder")]
		bool IsEnumeratingDirectoryPostOrder { get; }
	}

	delegate bool NSPredicateEvaluator (NSObject evaluatedObject, NSDictionary bindings);

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
		[NoWatch]
		[NoTV]
		[Export ("predicateFromMetadataQueryString:")]
		NSPredicate FromMetadataQueryString (string query);

		[MacCatalyst (13, 1)]
		[Export ("allowEvaluation")]
		void AllowEvaluation ();
	}

	[Category, BaseType (typeof (NSOrderedSet))]
	partial interface NSPredicateSupport_NSOrderedSet {
		[Export ("filteredOrderedSetUsingPredicate:")]
		NSOrderedSet FilterUsingPredicate (NSPredicate p);
	}

	[Category, BaseType (typeof (NSMutableOrderedSet))]
	partial interface NSPredicateSupport_NSMutableOrderedSet {
		[Export ("filterUsingPredicate:")]
		void FilterUsingPredicate (NSPredicate p);
	}

	[Category, BaseType (typeof (NSArray))]
	partial interface NSPredicateSupport_NSArray {
		[Export ("filteredArrayUsingPredicate:")]
		NSArray FilterUsingPredicate (NSArray array);
	}

#pragma warning disable 618
	[Category, BaseType (typeof (NSMutableArray))]
#pragma warning restore 618
	partial interface NSPredicateSupport_NSMutableArray {
		[Export ("filterUsingPredicate:")]
		void FilterUsingPredicate (NSPredicate predicate);
	}

	[Category, BaseType (typeof (NSSet))]
	partial interface NSPredicateSupport_NSSet {
		[Export ("filteredSetUsingPredicate:")]
		NSSet FilterUsingPredicate (NSPredicate predicate);
	}

	[Category, BaseType (typeof (NSMutableSet))]
	partial interface NSPredicateSupport_NSMutableSet {
		[Export ("filterUsingPredicate:")]
		void FilterUsingPredicate (NSPredicate predicate);
	}

	[NoiOS]
	[NoMacCatalyst]
	[NoWatch]
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
	[NoWatch]
	[NoTV]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol (Name = "NSURLDownloadDelegate")]
	interface NSUrlDownloadDelegate {
		[Export ("downloadDidBegin:")]
		void DownloadBegan (NSUrlDownload download);

		[Export ("download:willSendRequest:redirectResponse:")]
		NSUrlRequest WillSendRequest (NSUrlDownload download, NSUrlRequest request, NSUrlResponse redirectResponse);

		[Export ("download:didReceiveAuthenticationChallenge:")]
		void ReceivedAuthenticationChallenge (NSUrlDownload download, NSUrlAuthenticationChallenge challenge);

		[Export ("download:didCancelAuthenticationChallenge:")]
		void CanceledAuthenticationChallenge (NSUrlDownload download, NSUrlAuthenticationChallenge challenge);

		[Export ("download:didReceiveResponse:")]
		void ReceivedResponse (NSUrlDownload download, NSUrlResponse response);

		//- (void)download:(NSUrlDownload *)download willResumeWithResponse:(NSUrlResponse *)response fromByte:(long long)startingByte;
		[Export ("download:willResumeWithResponse:fromByte:")]
		void Resume (NSUrlDownload download, NSUrlResponse response, long startingByte);

		//- (void)download:(NSUrlDownload *)download didReceiveDataOfLength:(NSUInteger)length;
		[Export ("download:didReceiveDataOfLength:")]
		void ReceivedData (NSUrlDownload download, nuint length);

		[Export ("download:shouldDecodeSourceDataOfMIMEType:")]
		bool DecodeSourceData (NSUrlDownload download, string encodingType);

		[Export ("download:decideDestinationWithSuggestedFilename:")]
		void DecideDestination (NSUrlDownload download, string suggestedFilename);

		[Export ("download:didCreateDestination:")]
		void CreatedDestination (NSUrlDownload download, string path);

		[Export ("downloadDidFinish:")]
		void Finished (NSUrlDownload download);

		[Export ("download:didFailWithError:")]
		void FailedWithError (NSUrlDownload download, NSError error);
	}

	// Users are not supposed to implement the NSUrlProtocolClient protocol, they're 
	// only supposed to consume it. This is why there's no model for this protocol.
	[Protocol (Name = "NSURLProtocolClient")]
	interface NSUrlProtocolClient {
		[Abstract]
		[Export ("URLProtocol:wasRedirectedToRequest:redirectResponse:")]
		void Redirected (NSUrlProtocol protocol, NSUrlRequest redirectedToEequest, NSUrlResponse redirectResponse);

		[Abstract]
		[Export ("URLProtocol:cachedResponseIsValid:")]
		void CachedResponseIsValid (NSUrlProtocol protocol, NSCachedUrlResponse cachedResponse);

		[Abstract]
		[Export ("URLProtocol:didReceiveResponse:cacheStoragePolicy:")]
		void ReceivedResponse (NSUrlProtocol protocol, NSUrlResponse response, NSUrlCacheStoragePolicy policy);

		[Abstract]
		[Export ("URLProtocol:didLoadData:")]
		void DataLoaded (NSUrlProtocol protocol, NSData data);

		[Abstract]
		[Export ("URLProtocolDidFinishLoading:")]
		void FinishedLoading (NSUrlProtocol protocol);

		[Abstract]
		[Export ("URLProtocol:didFailWithError:")]
		void FailedWithError (NSUrlProtocol protocol, NSError error);

		[Abstract]
		[Export ("URLProtocol:didReceiveAuthenticationChallenge:")]
		void ReceivedAuthenticationChallenge (NSUrlProtocol protocol, NSUrlAuthenticationChallenge challenge);

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
		[Abstract]
		// @required - (void)beginRequestWithExtensionContext:(NSExtensionContext *)context;
		[Export ("beginRequestWithExtensionContext:")]
		void BeginRequestWithExtensionContext (NSExtensionContext context);
	}

	[Protocol]
	interface NSLocking {

		[Abstract]
		[Export ("lock")]
		void Lock ();

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

		[Wrap ("WeakAddressComponents")]
		NSTextCheckingAddressComponents AddressComponents { get; }

		[Export ("numberOfRanges")]
		nuint NumberOfRanges { get; }

		[Export ("rangeAtIndex:")]
		NSRange RangeAtIndex (nuint idx);

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

		[Static]
		[Wrap ("TransitInformationCheckingResult (range, components.GetDictionary ()!)")]
		NSTextCheckingResult TransitInformationCheckingResult (NSRange range, NSTextCheckingTransitComponents components);

		[MacCatalyst (13, 1)]
		[Export ("rangeWithName:")]
		NSRange GetRange (string name);

	}

	[StrongDictionary ("NSTextChecking")]
	interface NSTextCheckingTransitComponents {
		string Airline { get; }

		string Flight { get; }
	}

	[StrongDictionary ("NSTextChecking")]
	interface NSTextCheckingAddressComponents {
		string Name { get; }

		string JobTitle { get; }

		string Organization { get; }

		string Street { get; }

		string City { get; }

		string State { get; }

		[Export ("ZipKey")]
		string ZIP { get; }

		string Country { get; }

		string Phone { get; }
	}

	[Static]
	interface NSTextChecking {
		[Field ("NSTextCheckingNameKey")]
		NSString NameKey { get; }

		[Field ("NSTextCheckingJobTitleKey")]
		NSString JobTitleKey { get; }

		[Field ("NSTextCheckingOrganizationKey")]
		NSString OrganizationKey { get; }

		[Field ("NSTextCheckingStreetKey")]
		NSString StreetKey { get; }

		[Field ("NSTextCheckingCityKey")]
		NSString CityKey { get; }

		[Field ("NSTextCheckingStateKey")]
		NSString StateKey { get; }

		[Field ("NSTextCheckingZIPKey")]
		NSString ZipKey { get; }

		[Field ("NSTextCheckingCountryKey")]
		NSString CountryKey { get; }

		[Field ("NSTextCheckingPhoneKey")]
		NSString PhoneKey { get; }

		[Field ("NSTextCheckingAirlineKey")]
		NSString AirlineKey { get; }

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

		[DesignatedInitializer]
		[Export ("initWithCondition:")]
		NativeHandle Constructor (nint condition);

		[Export ("condition")]
		nint Condition { get; }

		[Export ("lockWhenCondition:")]
		void LockWhenCondition (nint condition);

		[Export ("tryLock")]
		bool TryLock ();

		[Export ("tryLockWhenCondition:")]
		bool TryLockWhenCondition (nint condition);

		[Export ("unlockWithCondition:")]
		void UnlockWithCondition (nint condition);

		[Export ("lockBeforeDate:")]
		bool LockBeforeDate (NSDate limit);

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
		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Export ("imageForResource:")]
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

	[DisableDefaultCtor] // -init should never be called on NSUnit!
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

		[NoTV, NoWatch]
		[MacCatalyst (13, 1)]
		[Export ("trashItemAtURL:resultingItemURL:error:")]
		bool TrashItem (NSUrl url, out NSUrl resultingItemUrl, out NSError error);

		[NoiOS]
		[NoMacCatalyst]
		[NoWatch]
		[NoTV]
		[Static]
		[Export ("fileManagerWithAuthorization:")]
		NSFileManager FromAuthorization (NSWorkspaceAuthorization authorization);
	}

	[NoWatch, NoTV]
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
		[NoiOS][NoMacCatalyst][NoWatch][NoTV]
		[NullAllowed]
		[Export ("primaryPresentedItemURL")]
		NSUrl PrimaryPresentedItemUrl { get; }
	}

	[NoiOS][NoMacCatalyst][NoWatch][NoTV]
	[Deprecated (PlatformName.MacOSX, 12, 0, message : "Use the Network.framework instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	partial interface NSHost {

		[Static, Internal, Export ("currentHost")]
		NSHost _Current { get;}

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
		string [] _Addresses  { get; }

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
	[NoWatch]
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
	[NoWatch]
	[MacCatalyst (15, 0)]
	[StrongDictionary ("NSScriptCommandArgumentDescriptionKeys")]
	partial interface NSScriptCommandArgumentDescription {
		string AppleEventCode { get; set; }
		string Type { get; set; }
		string Optional { get; set; }
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[MacCatalyst (15, 0)]
	[StrongDictionary ("NSScriptCommandDescriptionDictionaryKeys")]
	partial interface NSScriptCommandDescriptionDictionary {
		string CommandClass { get; set; }
		string AppleEventCode { get; set; }
		string AppleEventClassCode { get; set; }
		string Type { get; set; }
		string ResultAppleEventCode { get; set; }
		NSMutableDictionary Arguments { get; set; }
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
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

	[NoiOS, NoTV, NoWatch]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	[MacCatalyst (13, 1)]
	interface NSAffineTransform : NSSecureCoding, NSCopying {
		[Export ("initWithTransform:")]
		NativeHandle Constructor (NSAffineTransform transform);

		[Export ("translateXBy:yBy:")]
		void Translate (nfloat deltaX, nfloat deltaY);

		[Export ("rotateByDegrees:")]
		void RotateByDegrees (nfloat angle);

		[Export ("rotateByRadians:")]
		void RotateByRadians (nfloat angle);

		[Export ("scaleBy:")]
		void Scale (nfloat scale);

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

		[NoMacCatalyst]
		[Export ("transformBezierPath:")]
		NSBezierPath TransformBezierPath (NSBezierPath path);

		[Export ("set")]
		void Set ();

		[Export ("concat")]
		void Concat ();

		[Export ("transformStruct")]
		CGAffineTransform TransformStruct { get; set; }
	}

	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSXpcConnection' instead.")]
	[NoMacCatalyst]
	[NoiOS, NoTV, NoWatch]
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

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSConnectionDelegate Delegate { get; set; }
	}

	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSXpcConnection' instead.")]
	[NoMacCatalyst]
	[NoiOS, NoTV, NoWatch]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSConnectionDelegate {
		[Export ("authenticateComponents:withData:")]
		bool AuthenticateComponents (NSArray components, NSData authenticationData);

		[Export ("authenticationDataForComponents:")]
		NSData GetAuthenticationData (NSArray components);

		[Export ("connection:shouldMakeNewConnection:")]
		bool ShouldMakeNewConnection (NSConnection parentConnection, NSConnection newConnection);

		[Export ("connection:handleRequest:")]
		bool HandleRequest (NSConnection connection, NSDistantObjectRequest request);

		[Export ("createConversationForConnection:")]
		NSObject CreateConversation (NSConnection connection);

		[Export ("makeNewConnection:sender:")]
		bool AllowNewConnection (NSConnection newConnection, NSConnection parentConnection);
	}

	[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'NSXpcConnection' instead.")]
	[NoMacCatalyst]
	[NoiOS, NoTV, NoWatch]
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
	[NoiOS, NoTV, NoWatch]
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

	[NoiOS, NoTV, NoWatch]
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

#if !XAMCORE_3_0
		[Obsolete ("Use the constructor instead.")]
		[Export ("initListDescriptor")]
		NSObject InitListDescriptor ();

		[Obsolete ("Use the constructor instead.")]
		[Export ("initRecordDescriptor")]
		NSObject InitRecordDescriptor ();
#endif

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
		void SetParamDescriptorforKeyword (NSAppleEventDescriptor descriptor, AEKeyword keyword);

		[return: NullAllowed]
		[Export ("paramDescriptorForKeyword:")]
		NSAppleEventDescriptor ParamDescriptorForKeyword (AEKeyword keyword);

		[Export ("removeParamDescriptorWithKeyword:")]
		void RemoveParamDescriptorWithKeyword (AEKeyword keyword);

		[Export ("setAttributeDescriptor:forKeyword:")]
		void SetAttributeDescriptorforKeyword (NSAppleEventDescriptor descriptor, AEKeyword keyword);

		[return: NullAllowed]
		[Export ("attributeDescriptorForKeyword:")]
		NSAppleEventDescriptor AttributeDescriptorForKeyword (AEKeyword keyword);

		[Export ("numberOfItems")]
		nint NumberOfItems { get; }

		[Export ("insertDescriptor:atIndex:")]
		void InsertDescriptoratIndex (NSAppleEventDescriptor descriptor, nint index);

		[return: NullAllowed]
		[Export ("descriptorAtIndex:")]
		NSAppleEventDescriptor DescriptorAtIndex (nint index);

		[Export ("removeDescriptorAtIndex:")]
		void RemoveDescriptorAtIndex (nint index);

		[Export ("setDescriptor:forKeyword:")]
		void SetDescriptorforKeyword (NSAppleEventDescriptor descriptor, AEKeyword keyword);

		[return: NullAllowed]
		[Export ("descriptorForKeyword:")]
		NSAppleEventDescriptor DescriptorForKeyword (AEKeyword keyword);

		[Export ("removeDescriptorWithKeyword:")]
		void RemoveDescriptorWithKeyword (AEKeyword keyword);

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

	[NoiOS, NoTV, NoWatch]
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

	}

	[NoiOS, NoTV, NoWatch]
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

#if !NET && MONOMAC
		[Field ("NSTaskDidTerminateNotification")]
		NSString NSTaskDidTerminateNotification { get; }
#endif

		[Field ("NSTaskDidTerminateNotification")]
		[Notification]
		NSString DidTerminateNotification { get; }
	}

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

		[Export ("presented")]
		bool Presented { [Bind ("isPresented")] get; }

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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
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

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		[Protocolize]
		NSUserNotificationCenterDelegate Delegate { get; set; }

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

	[NoiOS, NoTV, NoWatch, NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'UserNotifications.*' API instead.")]
	interface NSUserNotificationCenterDelegate {
		[Export ("userNotificationCenter:didDeliverNotification:"), EventArgs ("UNCDidDeliverNotification")]
		void DidDeliverNotification (NSUserNotificationCenter center, NSUserNotification notification);

		[Export ("userNotificationCenter:didActivateNotification:"), EventArgs ("UNCDidActivateNotification")]
		void DidActivateNotification (NSUserNotificationCenter center, NSUserNotification notification);

		[Export ("userNotificationCenter:shouldPresentNotification:"), DelegateName ("UNCShouldPresentNotification"), DefaultValue (false)]
		bool ShouldPresentNotification (NSUserNotificationCenter center, NSUserNotification notification);
	}

	[NoiOS, NoTV, NoWatch]
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
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "This type is not meant to be user created.")]
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

		[Export ("proxyConnection")]
		bool ProxyConnection { [Bind ("isProxyConnection")] get; }

		[Export ("reusedConnection")]
		bool ReusedConnection { [Bind ("isReusedConnection")] get; }

		[Export ("resourceFetchType", ArgumentSemantic.Assign)]
		NSUrlSessionTaskMetricsResourceFetchType ResourceFetchType { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("countOfRequestHeaderBytesSent")]
		long CountOfRequestHeaderBytesSent { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("countOfRequestBodyBytesSent")]
		long CountOfRequestBodyBytesSent { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("countOfRequestBodyBytesBeforeEncoding")]
		long CountOfRequestBodyBytesBeforeEncoding { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("countOfResponseHeaderBytesReceived")]
		long CountOfResponseHeaderBytesReceived { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("countOfResponseBodyBytesReceived")]
		long CountOfResponseBodyBytesReceived { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("countOfResponseBodyBytesAfterDecoding")]
		long CountOfResponseBodyBytesAfterDecoding { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("localAddress")]
		string LocalAddress { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("localPort", ArgumentSemantic.Copy)]
		// 0-1023
		[BindAs (typeof (ushort?))]
		NSNumber LocalPort { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("remoteAddress")]
		string RemoteAddress { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("remotePort", ArgumentSemantic.Copy)]
		// 0-1023
		[BindAs (typeof (ushort?))]
		NSNumber RemotePort { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("negotiatedTLSProtocolVersion", ArgumentSemantic.Copy)]
		// <quote>It is a 2-byte sequence in host byte order.</quote> but it refers to (nicer) `tls_protocol_version_t`
		[BindAs (typeof (SslProtocol?))]
		NSNumber NegotiatedTlsProtocolVersion { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("negotiatedTLSCipherSuite", ArgumentSemantic.Copy)]
		// <quote>It is a 2-byte sequence in host byte order.</quote> but it refers to (nicer) `tls_ciphersuite_t`
#if NET
		[BindAs (typeof (TlsCipherSuite?))]
#else
		[BindAs (typeof (SslCipherSuite?))]
#endif
		NSNumber NegotiatedTlsCipherSuite { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("cellular")]
		bool Cellular { [Bind ("isCellular")] get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("expensive")]
		bool Expensive { [Bind ("isExpensive")] get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("constrained")]
		bool Constrained { [Bind ("isConstrained")] get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("multipath")]
		bool Multipath { [Bind ("isMultipath")] get; }

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
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
		[Deprecated (PlatformName.WatchOS, 6, 0, message: "This type is not meant to be user created.")]
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

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("milliseconds", ArgumentSemantic.Copy)]
		NSUnitDuration Milliseconds { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("microseconds", ArgumentSemantic.Copy)]
		NSUnitDuration Microseconds { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("nanoseconds", ArgumentSemantic.Copy)]
		NSUnitDuration Nanoseconds { get; }

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

		[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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
		[NoWatch]
		[NoTV]
		[NoMacCatalyst]
		NativeHandle Constructor (string xpcServiceName);

		[Export ("serviceName")]
		string ServiceName { get; }

		[Export ("initWithMachServiceName:options:")]
		[NoiOS]
		[NoWatch]
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
		[iOS (13, 0)]
		[Watch (6, 0)]
		[TV (13, 0)]
		[MacCatalyst (13, 1)]
		void ScheduleSendBarrier (Action block);

		[Export ("remoteObjectProxy"), Internal]
		IntPtr _CreateRemoteObjectProxy ();

		[Export ("remoteObjectProxyWithErrorHandler:"), Internal]
		IntPtr _CreateRemoteObjectProxy ([BlockCallback] Action<NSError> errorHandler);

		[MacCatalyst (13, 1)]
		[Export ("synchronousRemoteObjectProxyWithErrorHandler:"), Internal]
		IntPtr _CreateSynchronousRemoteObjectProxy ([BlockCallback] Action<NSError> errorHandler);

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("activate")]
		void Activate ();

		[NoWatch, NoTV, NoiOS, Mac (13, 0)]
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
		[NoWatch]
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

		[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
		[MacCatalyst (14, 0)]
		[Export ("activate")]
		void Activate ();

		[NoWatch, NoTV, NoiOS, Mac (13, 0)]
		[NoMacCatalyst]
		[Export ("setConnectionCodeSigningRequirement:")]
		void SetConnectionCodeSigningRequirement (string requirement);
	}

	[BaseType (typeof (NSObject), Name = "NSXPCListenerDelegate")]
#if NET
	[Protocol, Model]
#else
	[Model (AutoGeneratedName = true), Protocol]
#endif
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

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NSRelativeDateTimeFormatterStyle : long {
		Numeric = 0,
		Named,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NSRelativeDateTimeFormatterUnitsStyle : long {
		Full = 0,
		SpellOut,
		Short,
		Abbreviated,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NSCollectionChangeType : long {
		Insert,
		Remove,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NSOrderedCollectionDifferenceCalculationOptions : ulong {
		OmitInsertedObjects = (1uL << 0),
		OmitRemovedObjects = (1uL << 1),
		InferMoves = (1uL << 2),
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NSUrlSessionWebSocketMessageType : long {
		Data = 0,
		String = 1,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
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

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol][Model]
#else
	[Protocol]
	[Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name = "NSURLSessionWebSocketDelegate")]
	interface NSUrlSessionWebSocketDelegate {

		[Export ("URLSession:webSocketTask:didOpenWithProtocol:")]
		void DidOpen (NSUrlSession session, NSUrlSessionWebSocketTask webSocketTask, [NullAllowed] string protocol);

		[Export ("URLSession:webSocketTask:didCloseWithCode:reason:")]
		void DidClose (NSUrlSession session, NSUrlSessionWebSocketTask webSocketTask, NSUrlSessionWebSocketCloseCode closeCode, [NullAllowed] NSData reason);
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NSUrlErrorNetworkUnavailableReason : long {
		Cellular = 0,
		Expensive = 1,
		Constrained = 2,
	}

	[NoWatch, NoTV, NoiOS]
	[NoMacCatalyst]
	[Native]
	public enum NSBackgroundActivityResult : long {
		Finished = 1,
		Deferred = 2,
	}

	delegate void NSBackgroundActivityCompletionHandler (NSBackgroundActivityResult result);

	delegate void NSBackgroundActivityCompletionAction ([BlockCallback] NSBackgroundActivityCompletionHandler handler);

	[NoWatch, NoTV, NoiOS]
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

	[Watch (7, 0), TV (14, 0), Mac (11, 0), iOS (14, 0)]
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
	[NoWatch]
	[MacCatalyst (15, 0)]
	[Native]
	public enum NSNotificationSuspensionBehavior : ulong {
		Drop = 1,
		Coalesce = 2,
		Hold = 3,
		DeliverImmediately = 4,
	}

	[NoiOS]
	[NoTV]
	[NoWatch]
	[MacCatalyst (15, 0)]
	[Flags]
	[Native]
	public enum NSNotificationFlags : ulong {
		DeliverImmediately = (1 << 0),
		PostToAllSessions = (1 << 1),
	}

	[NoWatch]
	[NoTV]
	[NoiOS]
	[NoMacCatalyst]
	[Native]
	[Flags]
	public enum NSFileManagerUnmountOptions : ulong {
		AllPartitionsAndEjectDisk = 1 << 0,
		WithoutUI = 1 << 1,
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

		[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0)]
		[MacCatalyst (16, 0)]
		[Export ("appliesSourcePositionAttributes")]
		bool AppliesSourcePositionAttributes { get; set; }
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSInflectionRule : NSCopying, NSSecureCoding {
		[Static]
		[Export ("automaticRule")]
		NSInflectionRule AutomaticRule { get; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("canInflectLanguage:")]
		bool CanInflectLanguage (string language);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0)]
		[MacCatalyst (15, 0)]
		[Static]
		[Export ("canInflectPreferredLocalization")]
		bool CanInflectPreferredLocalization { get; }
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSInflectionRule))]
	interface NSInflectionRuleExplicit {
		[Export ("initWithMorphology:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSMorphology morphology);

		[Export ("morphology", ArgumentSemantic.Copy)]
		NSMorphology Morphology { get; }
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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
		[Obsoleted (PlatformName.WatchOS, 10, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Export ("customPronounForLanguage:")]
		[return: NullAllowed]
		NSMorphologyCustomPronoun GetCustomPronoun (string language);

		[Obsoleted (PlatformName.MacOSX, 14, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Obsoleted (PlatformName.iOS, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Obsoleted (PlatformName.MacCatalyst, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Obsoleted (PlatformName.TvOS, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Obsoleted (PlatformName.WatchOS, 10, 0, message: "Use 'NSTermOfAddress' instead.")]
		[Export ("setCustomPronoun:forLanguage:error:")]
		bool SetCustomPronoun ([NullAllowed] NSMorphologyCustomPronoun features, string language, [NullAllowed] out NSError error);

		[Export ("unspecified")]
		bool Unspecified { [Bind ("isUnspecified")] get; }

		[Static]
		[Export ("userMorphology")]
		NSMorphology UserMorphology { get; }

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("grammaticalCase", ArgumentSemantic.Assign)]
		NSGrammaticalCase GrammaticalCase { get; set; }

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("determination", ArgumentSemantic.Assign)]
		NSGrammaticalDetermination Determination { get; set; }

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("grammaticalPerson", ArgumentSemantic.Assign)]
		NSGrammaticalPerson GrammaticalPerson { get; set; }

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("pronounType", ArgumentSemantic.Assign)]
		NSGrammaticalPronounType PronounType { get; set; }

		[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[Export ("definiteness", ArgumentSemantic.Assign)]
		NSGrammaticalDefiniteness Definiteness { get; set; }
	}

	[Obsoleted (PlatformName.MacOSX, 14, 0, message: "Use 'NSTermOfAddress' instead.")]
	[Obsoleted (PlatformName.iOS, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
	[Obsoleted (PlatformName.MacCatalyst, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
	[Obsoleted (PlatformName.TvOS, 17, 0, message: "Use 'NSTermOfAddress' instead.")]
	[Obsoleted (PlatformName.WatchOS, 10, 0, message: "Use 'NSTermOfAddress' instead.")]
	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
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

#if false // https://github.com/xamarin/xamarin-macios/issues/15577
	interface NSOrderedCollectionChange <TKey> : NSOrderedCollectionChange {}
	
	[Watch (6,0), TV (13,0), iOS (13,0)]
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
	
	[Watch (6,0), TV (13,0), iOS (13,0)]
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
		[Watch (6,0), TV (13,0), iOS (13,0)]
		[Export ("inverseDifference")]
		NativeHandle _InverseDifference ();
	}
#endif

	[Watch (9, 0), TV (16, 0), Mac (13, 0), iOS (16, 0)]
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

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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
	}

	[Watch (10, 0), TV (17, 0), Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
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
}
