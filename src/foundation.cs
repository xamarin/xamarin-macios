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
#if IOS
using QuickLook;
#endif
#if !TVOS
using Contacts;
#endif
#if !WATCH
using CoreAnimation;
using CoreMedia;
using CoreSpotlight;
#endif
using SceneKit;
using Security;
#if IOS && XAMCORE_2_0
using FileProvider;
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

#if MONOMAC
// In Apple headers, this is a typedef to a pointer to a private struct
using NSAppleEventManagerSuspensionID = System.IntPtr;
// These two are both four char codes i.e. defined on a uint with constant like 'xxxx'
using AEKeyword = System.UInt32;
using OSType = System.UInt32;
// typedef double NSTimeInterval;
using NSTimeInterval = System.Double;

// dummy usings to make code compile without having the actual types available (for [NoMac] to work)
using NSDirectionalEdgeInsets = Foundation.NSObject;
using UIEdgeInsets = Foundation.NSObject;
using UIOffset = Foundation.NSObject;
#endif

#if WATCH
// dummy usings to make code compile without having the actual types available (for [NoWatch] to work)
using CMTime = Foundation.NSObject;
using CMTimeMapping = Foundation.NSObject;
using CMTimeRange = Foundation.NSObject;
#endif

#if WATCH
using CIBarcodeDescriptor = Foundation.NSObject;
#else
using CoreImage;
#endif

// This little gem comes from a btouch bug that wrote the NSFilePresenterReacquirer delegate to the wrong
// namespace for a while (it should go into Foundation, but due to the bug it went into UIKit). In order
// to not break backwards compatibility (once the btouch bug was fixed), we need to make sure the delegate
// stays in UIKit for Xamarin.iOS/Classic (the delegate was always in the right namespace for Xamarin.Mac).
#if XAMCORE_2_0 || MONOMAC
namespace Foundation {
#else
namespace UIKit {
#endif
	delegate void NSFilePresenterReacquirer ([BlockCallback] Action reacquirer);
}

namespace Foundation
{
#if XAMCORE_2_0
	delegate NSComparisonResult NSComparator (NSObject obj1, NSObject obj2);
#else
	delegate int /* !XAMCORE_2_0 */ NSComparator (NSObject obj1, NSObject obj2);
#endif
	delegate void NSAttributedRangeCallback (NSDictionary attrs, NSRange range, ref bool stop);
	delegate void NSAttributedStringCallback (NSObject value, NSRange range, ref bool stop);

	delegate bool NSEnumerateErrorHandler (NSUrl url, NSError error);
	delegate void NSMetadataQueryEnumerationCallback (NSObject result, nuint idx, ref bool stop);
	delegate void NSItemProviderCompletionHandler (NSObject itemBeingLoaded, NSError error);
	delegate void NSItemProviderLoadHandler ([BlockCallback] NSItemProviderCompletionHandler completionHandler, Class expectedValueClass, NSDictionary options);
	delegate void EnumerateDatesCallback (NSDate date, bool exactMatch, ref bool stop);
	delegate void EnumerateIndexSetCallback (nuint idx, ref bool stop);
#if MONOMAC
	delegate void CloudKitRegistrationPreparationAction ([BlockCallback] CloudKitRegistrationPreparationHandler handler);
	delegate void CloudKitRegistrationPreparationHandler (CKShare share, CKContainer container, NSError error);
#endif

	interface NSArray<TValue> : NSArray {}

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSArray : NSSecureCoding, NSMutableCopying, INSFastEnumeration, CKRecordValue {
		[Export ("count")]
		nuint Count { get; }

		[Export ("objectAtIndex:")]
		IntPtr ValueAt (nuint idx);

		[Static]
		[Internal]
		[Export ("arrayWithObjects:count:")]
		IntPtr FromObjects (IntPtr array, nint count);

		[Export ("valueForKey:")]
		[MarshalNativeExceptions]
		NSObject ValueForKey (NSString key);

		[Export ("setValue:forKey:")]
		void SetValueForKey (NSObject value, NSString key);

		[Export ("writeToFile:atomically:")]
		bool WriteToFile (string path, bool useAuxiliaryFile);

		[Export ("arrayWithContentsOfFile:")][Static]
		NSArray FromFile (string path);
		
		[Export ("sortedArrayUsingComparator:")]
		NSArray Sort (NSComparator cmptr);
		
		[Export ("filteredArrayUsingPredicate:")]
		NSArray Filter (NSPredicate predicate);

		[Internal]
		[Sealed]
		[Export ("containsObject:")]
		bool _Contains (IntPtr anObject);

		[Export ("containsObject:")]
		bool Contains (NSObject anObject);

		[Internal]
		[Sealed]
		[Export ("indexOfObject:")]
		nuint _IndexOf (IntPtr anObject);

		[Export ("indexOfObject:")]
		nuint IndexOf (NSObject anObject);

		[Export ("addObserver:toObjectsAtIndexes:forKeyPath:options:context:")]
		void AddObserver (NSObject observer, NSIndexSet indexes, string keyPath, NSKeyValueObservingOptions options, IntPtr context);

		[Export ("removeObserver:fromObjectsAtIndexes:forKeyPath:context:")]
		void RemoveObserver (NSObject observer, NSIndexSet indexes, string keyPath, IntPtr context);

		[Export ("removeObserver:fromObjectsAtIndexes:forKeyPath:")]
		void RemoveObserver (NSObject observer, NSIndexSet indexes, string keyPath);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("writeToURL:error:")]
		bool Write (NSUrl url, out NSError error);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("arrayWithContentsOfURL:error:")]
		[return: NullAllowed]
		NSArray FromUrl (NSUrl url, out NSError error);
	}

#if MONOMAC
	interface NSAttributedStringDocumentAttributes { }
#endif

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
		IntPtr LowLevelGetAttributes (nint location, out NSRange effectiveRange);

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
		IntPtr Constructor (string str);

#if !MONOMAC

#if IOS
		// New API in iOS9 with same signature as an older alternative.
		// We expose only the *new* one for the new platforms as the old
		// one was moved to `NSDeprecatedKitAdditions (NSAttributedString)`
		[iOS (9,0)]
		[Internal]
		[Export ("initWithURL:options:documentAttributes:error:")]
		IntPtr InitWithURL (NSUrl url, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);
		// but we still need to allow the API to work before iOS 9.0
		// and to compleify matters the old one was deprecated in 9.0
		[iOS (7,0)]
		[Internal]
		[Deprecated (PlatformName.iOS, 9, 0)]
		[Export ("initWithFileURL:options:documentAttributes:error:")]
		IntPtr InitWithFileURL (NSUrl url, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);
#elif TVOS || WATCH
		[iOS (9,0)]
		[Export ("initWithURL:options:documentAttributes:error:")]
		IntPtr Constructor (NSUrl url, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);
#endif

		[iOS (7,0)]
		[Wrap ("this (url, options == null ? null : options.Dictionary, out resultDocumentAttributes, ref error)")]
		IntPtr Constructor (NSUrl url, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, ref NSError error);

		[iOS (7,0)]
		[Export ("initWithData:options:documentAttributes:error:")]
		IntPtr Constructor (NSData data, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes, ref NSError error);

		[iOS (7,0)]
		[Wrap ("this (data, options == null ? null : options.Dictionary, out resultDocumentAttributes, ref error)")]
		IntPtr Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, ref NSError error);

		[iOS (7,0)]
		[Export ("dataFromRange:documentAttributes:error:")]
		NSData GetDataFromRange (NSRange range, NSDictionary attributes, ref NSError error);

		[iOS (7,0)]
		[Wrap ("GetDataFromRange (range, documentAttributes == null ? null : documentAttributes.Dictionary, ref error)")]
		NSData GetDataFromRange (NSRange range, NSAttributedStringDocumentAttributes documentAttributes, ref NSError error);

		[iOS (7,0)]
		[Export ("fileWrapperFromRange:documentAttributes:error:")]
		NSFileWrapper GetFileWrapperFromRange (NSRange range, NSDictionary attributes, ref NSError error);

		[iOS (7,0)]
		[Wrap ("GetFileWrapperFromRange (range, documentAttributes == null ? null : documentAttributes.Dictionary, ref error)")]
		NSFileWrapper GetFileWrapperFromRange (NSRange range, NSAttributedStringDocumentAttributes documentAttributes, ref NSError error);

#endif
		
		[Export ("initWithString:attributes:")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		IntPtr Constructor (string str, [NullAllowed] NSDictionary attributes);

		[Export ("initWithAttributedString:")]
		IntPtr Constructor (NSAttributedString other);

		[Export ("enumerateAttributesInRange:options:usingBlock:")]
		void EnumerateAttributes (NSRange range, NSAttributedStringEnumeration options, NSAttributedRangeCallback callback);

		[Export ("enumerateAttribute:inRange:options:usingBlock:")]
		void EnumerateAttribute (NSString attributeName, NSRange inRange, NSAttributedStringEnumeration options, NSAttributedStringCallback callback);


#if MONOMAC && !XAMCORE_2_0
		[Field ("NSFontAttributeName", "AppKit")]
		NSString FontAttributeName { get; }

		[Field ("NSLinkAttributeName", "AppKit")]
		NSString LinkAttributeName { get; }

		[Field ("NSUnderlineStyleAttributeName", "AppKit")]
		NSString UnderlineStyleAttributeName { get; }

		[Field ("NSStrikethroughStyleAttributeName", "AppKit")]
		NSString StrikethroughStyleAttributeName { get; }

		[Field ("NSStrokeWidthAttributeName", "AppKit")]
		NSString StrokeWidthAttributeName { get; }

		[Field ("NSParagraphStyleAttributeName", "AppKit")]
		NSString ParagraphStyleAttributeName { get; }

		[Field ("NSForegroundColorAttributeName", "AppKit")]
		NSString ForegroundColorAttributeName { get; }

		[Field ("NSBackgroundColorAttributeName", "AppKit")]
		NSString BackgroundColorAttributeName { get; }

		[Field ("NSLigatureAttributeName", "AppKit")]
		NSString LigatureAttributeName { get; } 

		[Field ("NSObliquenessAttributeName", "AppKit")]
		NSString ObliquenessAttributeName { get; }

		[Field ("NSSuperscriptAttributeName", "AppKit")]
		NSString SuperscriptAttributeName { get; }

		[Field ("NSAttachmentAttributeName", "AppKit")]
		NSString AttachmentAttributeName { get; }
		
		[Field ("NSBaselineOffsetAttributeName", "AppKit")]
		NSString BaselineOffsetAttributeName { get; }
		
		[Field ("NSKernAttributeName", "AppKit")]
		NSString KernAttributeName { get; }
		
		[Field ("NSStrokeColorAttributeName", "AppKit")]
		NSString StrokeColorAttributeName { get; }
		
		[Field ("NSUnderlineColorAttributeName", "AppKit")]
		NSString UnderlineColorAttributeName { get; }
		
		[Field ("NSStrikethroughColorAttributeName", "AppKit")]
		NSString StrikethroughColorAttributeName { get; }
		
		[Field ("NSShadowAttributeName", "AppKit")]
		NSString ShadowAttributeName { get; }
		
		[Field ("NSExpansionAttributeName", "AppKit")]
		NSString ExpansionAttributeName { get; }
		
		[Field ("NSCursorAttributeName", "AppKit")]
		NSString CursorAttributeName { get; }
		
		[Field ("NSToolTipAttributeName", "AppKit")]
		NSString ToolTipAttributeName { get; }
		
		[Field ("NSMarkedClauseSegmentAttributeName", "AppKit")]
		NSString MarkedClauseSegmentAttributeName { get; }
		
		[Field ("NSWritingDirectionAttributeName", "AppKit")]
		NSString WritingDirectionAttributeName { get; }
		
		[Field ("NSVerticalGlyphFormAttributeName", "AppKit")]
		NSString VerticalGlyphFormAttributeName { get; }
#endif

#if MONOMAC
		[Export("size")]
		CGSize Size { get; }

		[Export ("initWithData:options:documentAttributes:error:")]
		IntPtr Constructor (NSData data, [NullAllowed] NSDictionary options, out NSDictionary docAttributes, out NSError error);

		[Export ("initWithDocFormat:documentAttributes:")]
		IntPtr Constructor(NSData wordDocFormat, out NSDictionary docAttributes);

		[Export ("initWithHTML:baseURL:documentAttributes:")]
		IntPtr Constructor (NSData htmlData, NSUrl baseUrl, out NSDictionary docAttributes);
		
		[Export ("drawAtPoint:")]
		void DrawString (CGPoint point);
		
		[Export ("drawInRect:")]
		void DrawString (CGRect rect);
		
		[Export ("drawWithRect:options:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options);	

		[Export ("initWithURL:options:documentAttributes:error:")]
		IntPtr Constructor (NSUrl url, [NullAllowed] NSDictionary options, out NSDictionary resultDocumentAttributes, out NSError error);

		[Wrap ("this (url, options == null ? null : options.Dictionary, out resultDocumentAttributes, out error)")]
		IntPtr Constructor (NSUrl url, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, out NSError error);

		[Wrap ("this (data, options == null ? null : options.Dictionary, out resultDocumentAttributes, out error)")]
		IntPtr Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes, out NSError error);

		[Export ("initWithPath:documentAttributes:")]
		IntPtr Constructor (string path, out NSDictionary resultDocumentAttributes);

		[Export ("initWithURL:documentAttributes:")]
		IntPtr Constructor (NSUrl url, out NSDictionary resultDocumentAttributes);

		[Internal, Export ("initWithRTF:documentAttributes:")]
		IntPtr InitWithRtf (NSData data, out NSDictionary resultDocumentAttributes);

		[Internal, Export ("initWithRTFD:documentAttributes:")]
		IntPtr InitWithRtfd (NSData data, out NSDictionary resultDocumentAttributes);

		[Internal, Export ("initWithHTML:documentAttributes:")]
		IntPtr InitWithHTML (NSData data, out NSDictionary resultDocumentAttributes);

		[Export ("initWithHTML:options:documentAttributes:")]
		IntPtr Constructor (NSData data, [NullAllowed]  NSDictionary options, out NSDictionary resultDocumentAttributes);

		[Wrap ("this (data, options == null ? null : options.Dictionary, out resultDocumentAttributes)")]
		IntPtr Constructor (NSData data, NSAttributedStringDocumentAttributes options, out NSDictionary resultDocumentAttributes);

		[Export ("initWithRTFDFileWrapper:documentAttributes:")]
		IntPtr Constructor (NSFileWrapper wrapper, out NSDictionary resultDocumentAttributes);

		[Export ("containsAttachments")]
		bool ContainsAttachments { get; }

		[Export ("fontAttributesInRange:")]
		NSDictionary GetFontAttributes (NSRange range);

		[Export ("rulerAttributesInRange:")]
		NSDictionary GetRulerAttributes (NSRange range);

		[Export ("lineBreakBeforeIndex:withinRange:")]
		nuint GetLineBreak (nuint beforeIndex, NSRange aRange);

		[Export ("lineBreakByHyphenatingBeforeIndex:withinRange:")]
		nuint GetLineBreakByHyphenating (nuint beforeIndex, NSRange aRange);

		[Export ("doubleClickAtIndex:")]
		NSRange DoubleClick (nuint index);

		[Export ("nextWordFromIndex:forward:")]
		nuint GetNextWord (nuint fromIndex, bool isForward);

		[Export ("URLAtIndex:effectiveRange:")]
		NSUrl GetUrl (nuint index, out NSRange effectiveRange);

		[Export ("rangeOfTextBlock:atIndex:")]
		NSRange GetRange (NSTextBlock textBlock, nuint index);

		[Export ("rangeOfTextTable:atIndex:")]
		NSRange GetRange (NSTextTable textTable, nuint index);

		[Export ("rangeOfTextList:atIndex:")]
		NSRange GetRange (NSTextList textList, nuint index);

		[Export ("itemNumberInTextList:atIndex:")]
		nint GetItemNumber (NSTextList textList, nuint index);

		[Export ("dataFromRange:documentAttributes:error:")]
		NSData GetData (NSRange range, [NullAllowed] NSDictionary options, out NSError error);

		[Wrap ("this.GetData (range, options == null ? null : options.Dictionary, out error)")]
		NSData GetData (NSRange range, NSAttributedStringDocumentAttributes options, out NSError error);

		[Export ("fileWrapperFromRange:documentAttributes:error:")]
		NSFileWrapper GetFileWrapper (NSRange range, [NullAllowed] NSDictionary options, out NSError error);

		[Wrap ("this.GetFileWrapper (range, options == null ? null : options.Dictionary, out error)")]
		NSFileWrapper GetFileWrapper (NSRange range, NSAttributedStringDocumentAttributes options, out NSError error);

		[Export ("RTFFromRange:documentAttributes:")]
		NSData GetRtf (NSRange range, [NullAllowed] NSDictionary options);

		[Wrap ("this.GetRtf (range, options == null ? null : options.Dictionary)")]
		NSData GetRtf (NSRange range, NSAttributedStringDocumentAttributes options);

		[Export ("RTFDFromRange:documentAttributes:")]
		NSData GetRtfd (NSRange range, [NullAllowed] NSDictionary options);

		[Wrap ("this.GetRtfd (range, options == null ? null : options.Dictionary)")]
		NSData GetRtfd (NSRange range, NSAttributedStringDocumentAttributes options);

		[Export ("RTFDFileWrapperFromRange:documentAttributes:")]
		NSFileWrapper GetRtfdFileWrapper (NSRange range, [NullAllowed] NSDictionary options);

		[Wrap ("this.GetRtfdFileWrapper (range, options == null ? null : options.Dictionary)")]
		NSFileWrapper GetRtfdFileWrapper (NSRange range, NSAttributedStringDocumentAttributes options);

		[Export ("docFormatFromRange:documentAttributes:")]
		NSData GetDocFormat (NSRange range, [NullAllowed] NSDictionary options);

		[Wrap ("this.GetDocFormat (range, options == null ? null : options.Dictionary)")]
		NSData GetDocFormat (NSRange range, NSAttributedStringDocumentAttributes options);
#else
		[iOS (6,0)]
		[Export ("size")]
		CGSize Size { get; }

		[iOS (6,0)]
		[Export ("drawAtPoint:")]
		void DrawString (CGPoint point);

		[iOS (6,0)]
		[Export ("drawInRect:")]
		void DrawString (CGRect rect);

		[iOS (6,0)]
		[Export ("drawWithRect:options:context:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options, [NullAllowed] NSStringDrawingContext context);

		[iOS (6,0)]
		[Export ("boundingRectWithSize:options:context:")]
		CGRect GetBoundingRect (CGSize size, NSStringDrawingOptions options, [NullAllowed] NSStringDrawingContext context);
#endif

		// -(BOOL)containsAttachmentsInRange:(NSRange)range __attribute__((availability(macosx, introduced=10.11)));
		[Mac (10,11)][iOS (9,0)]
		[Export ("containsAttachmentsInRange:")]
		bool ContainsAttachmentsInRange (NSRange range);
	}

	[BaseType (typeof (NSObject),
		   Delegates=new string [] { "WeakDelegate" },
		   Events=new Type [] { typeof (NSCacheDelegate)} )]
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

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
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

	[BaseType (typeof (NSObject), Name="NSCachedURLResponse")]
	// instance created with 'init' will crash when Dispose is called
	[DisableDefaultCtor]
	interface NSCachedUrlResponse : NSCoding, NSSecureCoding, NSCopying {
		[Export ("initWithResponse:data:userInfo:storagePolicy:")]
		IntPtr Constructor (NSUrlResponse response, NSData data, [NullAllowed] NSDictionary userInfo, NSUrlCacheStoragePolicy storagePolicy);

		[Export ("initWithResponse:data:")]
		IntPtr Constructor (NSUrlResponse response, NSData data);
          
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
		IntPtr Constructor (NSString identifier);

		[Export ("calendarIdentifier")]
		string Identifier { get; }

		[Export ("currentCalendar")] [Static]
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

#if !XAMCORE_4_0
		[Obsolete ("Use the overload with a 'NSCalendarOptions' parameter.")]
		[Wrap ("Components (unitFlags, fromDate, toDate, (NSCalendarOptions) opts)")]
		NSDateComponents Components (NSCalendarUnit unitFlags, NSDate fromDate, NSDate toDate, NSDateComponentsWrappingBehavior opts);
#endif

		[Export ("dateByAddingComponents:toDate:options:")]
		NSDate DateByAddingComponents (NSDateComponents comps, NSDate date, NSCalendarOptions opts);

#if !XAMCORE_4_0
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

		[Mac(10,10)][iOS(8,0)]
		[Field ("NSCalendarIdentifierIslamicTabular"), Internal]
		NSString IslamicTabularCalendar { get; }

		[Mac(10,10)][iOS(8,0)]
		[Field ("NSCalendarIdentifierIslamicUmmAlQura"), Internal]
		NSString IslamicUmmAlQuraCalendar { get; }

		[Export ("eraSymbols")]
		[Mac(10,7)]
		string [] EraSymbols { get; }

		[Export ("longEraSymbols")]
		[Mac(10,7)]
		string [] LongEraSymbols { get; }

		[Export ("monthSymbols")]
		[Mac(10,7)]
		string [] MonthSymbols { get; }

		[Export ("shortMonthSymbols")]
		[Mac(10,7)]
		string [] ShortMonthSymbols { get; }

		[Export ("veryShortMonthSymbols")]
		[Mac(10,7)]
		string [] VeryShortMonthSymbols { get; }

		[Export ("standaloneMonthSymbols")]
		[Mac(10,7)]
		string [] StandaloneMonthSymbols { get; }

		[Export ("shortStandaloneMonthSymbols")]
		[Mac(10,7)]
		string [] ShortStandaloneMonthSymbols { get; }

		[Export ("veryShortStandaloneMonthSymbols")]
		[Mac(10,7)]
		string [] VeryShortStandaloneMonthSymbols { get; }

		[Export ("weekdaySymbols")]
		[Mac(10,7)]
		string [] WeekdaySymbols { get; }

		[Export ("shortWeekdaySymbols")]
		[Mac(10,7)]
		string [] ShortWeekdaySymbols { get; }

		[Export ("veryShortWeekdaySymbols")]
		[Mac(10,7)]
		string [] VeryShortWeekdaySymbols { get; }

		[Export ("standaloneWeekdaySymbols")]
		[Mac(10,7)]
		string [] StandaloneWeekdaySymbols { get; }

		[Export ("shortStandaloneWeekdaySymbols")]
		[Mac(10,7)]
		string [] ShortStandaloneWeekdaySymbols { get; }

		[Export ("veryShortStandaloneWeekdaySymbols")]
		[Mac(10,7)]
		string [] VeryShortStandaloneWeekdaySymbols { get; }

		[Export ("quarterSymbols")]
		[Mac(10,7)]
		string [] QuarterSymbols { get; }

		[Export ("shortQuarterSymbols")]
		[Mac(10,7)]
		string [] ShortQuarterSymbols { get; }

		[Export ("standaloneQuarterSymbols")]
		[Mac(10,7)]
		string [] StandaloneQuarterSymbols { get; }

		[Export ("shortStandaloneQuarterSymbols")]
		[Mac(10,7)]
		string [] ShortStandaloneQuarterSymbols { get; }

		[Export ("AMSymbol")]
		[Mac(10,7)]
		string AMSymbol { get; }

		[Export ("PMSymbol")]
		[Mac(10,7)]
		string PMSymbol { get; }

		[Export ("compareDate:toDate:toUnitGranularity:")]
		[Mac(10,9)][iOS(8,0)]
		NSComparisonResult CompareDate(NSDate date1, NSDate date2, NSCalendarUnit granularity);

		[Export ("component:fromDate:")]
		[Mac(10,9)][iOS(8,0)]
		nint GetComponentFromDate (NSCalendarUnit unit, NSDate date);

		[Export ("components:fromDateComponents:toDateComponents:options:")]
		[Mac(10,9)][iOS(8,0)]
		NSDateComponents ComponentsFromDateToDate (NSCalendarUnit unitFlags, NSDateComponents startingDate, NSDateComponents resultDate, NSCalendarOptions options);

		[Export ("componentsInTimeZone:fromDate:")]
		[Mac(10,9)][iOS(8,0)]
		NSDateComponents ComponentsInTimeZone (NSTimeZone timezone, NSDate date);

		[Export ("date:matchesComponents:")]
		[Mac(10,9)][iOS(8,0)]
		bool Matches (NSDate date, NSDateComponents components);

		[Export ("dateByAddingUnit:value:toDate:options:")]
		[Mac(10,9)][iOS(8,0)]
		NSDate DateByAddingUnit (NSCalendarUnit unit, nint value, NSDate date, NSCalendarOptions options);

		[Export ("dateBySettingHour:minute:second:ofDate:options:")]
		[Mac(10,9)][iOS(8,0)]
		NSDate DateBySettingsHour (nint hour, nint minute, nint second, NSDate date, NSCalendarOptions options);

		[Export ("dateBySettingUnit:value:ofDate:options:")]
		[Mac(10,9)][iOS(8,0)]
		NSDate DateBySettingUnit (NSCalendarUnit unit, nint value, NSDate date, NSCalendarOptions options);

		[Export ("dateWithEra:year:month:day:hour:minute:second:nanosecond:")]
		[Mac(10,9)][iOS(8,0)]
		NSDate Date (nint era, nint year, nint month, nint date, nint hour, nint minute, nint second, nint nanosecond);

		[Export ("dateWithEra:yearForWeekOfYear:weekOfYear:weekday:hour:minute:second:nanosecond:")]
		[Mac(10,9)][iOS(8,0)]
		NSDate DateForWeekOfYear (nint era, nint year, nint week, nint weekday, nint hour, nint minute, nint second, nint nanosecond);

		[Export ("enumerateDatesStartingAfterDate:matchingComponents:options:usingBlock:")]
		[Mac(10,9)][iOS(8,0)]
		void EnumerateDatesStartingAfterDate (NSDate start, NSDateComponents matchingComponents, NSCalendarOptions options, [BlockCallback] EnumerateDatesCallback callback);

		[Export ("getEra:year:month:day:fromDate:")]
		[Mac(10,9)][iOS(8,0)]
		void GetComponentsFromDate (out nint era, out nint year, out nint month, out nint day, NSDate date);

		[Export ("getEra:yearForWeekOfYear:weekOfYear:weekday:fromDate:")]
		[Mac(10,9)][iOS(8,0)]
		void GetComponentsFromDateForWeekOfYear (out nint era, out nint year, out nint weekOfYear, out nint weekday, NSDate date);

		[Export ("getHour:minute:second:nanosecond:fromDate:")]
		[Mac(10,9)][iOS(8,0)]
		void GetHourComponentsFromDate (out nint hour, out nint minute, out nint second, out nint nanosecond, NSDate date);

		[Export ("isDate:equalToDate:toUnitGranularity:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsEqualToUnitGranularity (NSDate date1, NSDate date2, NSCalendarUnit unit);

		[Export ("isDate:inSameDayAsDate:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsInSameDay (NSDate date1, NSDate date2);

		[Export ("isDateInToday:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsDateInToday (NSDate date);

		[Export ("isDateInTomorrow:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsDateInTomorrow (NSDate date);

		[Export ("isDateInWeekend:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsDateInWeekend (NSDate date);

		[Export ("isDateInYesterday:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsDateInYesterday (NSDate date);

		[Export ("nextDateAfterDate:matchingComponents:options:")]
		[Mac(10,9)][iOS(8,0)]
		[MarshalNativeExceptions]
		NSDate FindNextDateAfterDateMatching (NSDate date, NSDateComponents components, NSCalendarOptions options);

		[Export ("nextDateAfterDate:matchingHour:minute:second:options:")]
		[Mac(10,9)][iOS(8,0)]
		[MarshalNativeExceptions]
		NSDate FindNextDateAfterDateMatching (NSDate date, nint hour, nint minute, nint second, NSCalendarOptions options);

		[Export ("nextDateAfterDate:matchingUnit:value:options:")]
		[Mac(10,9)][iOS(8,0)]
		[MarshalNativeExceptions]
		NSDate FindNextDateAfterDateMatching (NSDate date, NSCalendarUnit unit, nint value, NSCalendarOptions options);

		[Export ("nextWeekendStartDate:interval:options:afterDate:")]
		[Mac(10,9)][iOS(8,0)]
		bool FindNextWeekend (out NSDate date, out double /* NSTimeInterval */ interval, NSCalendarOptions options, NSDate afterDate);

		[Export ("rangeOfWeekendStartDate:interval:containingDate:")]
		[Mac(10,9)][iOS(8,0)]
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
		[Mac(10,9)][iOS(8,0)]
		NSDate StartOfDayForDate (NSDate date);

		[Mac(10,9)][iOS(8,0)]
		[Notification]
		[Field ("NSCalendarDayChangedNotification")]
		NSString DayChangedNotification { get; }
	}

#if MONOMAC
	// Obsolete, but the only API surfaced by WebKit.WebHistory.
	[Availability (Deprecated = Platform.Mac_10_10, Message="Use NSCalendar and NSDateComponents.")]
	[BaseType (typeof (NSDate))]
	interface NSCalendarDate {
		[Export ("initWithString:calendarFormat:locale:")]
		[Availability (Deprecated = Platform.Mac_10_10)]
		IntPtr Constructor (string description, string calendarFormat, NSObject locale);

		[Export ("initWithString:calendarFormat:")]
		[Availability (Deprecated = Platform.Mac_10_10)]
		IntPtr Constructor (string description, string calendarFormat);

		[Export ("initWithString:")]
		[Availability (Deprecated = Platform.Mac_10_10)]
		IntPtr Constructor (string description);

		[Export ("initWithYear:month:day:hour:minute:second:timeZone:")]
		[Availability (Deprecated = Platform.Mac_10_10)]
		IntPtr Constructor (nint year, nuint month, nuint day, nuint hour, nuint minute, nuint second, NSTimeZone aTimeZone);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("dateByAddingYears:months:days:hours:minutes:seconds:")]
		NSCalendarDate DateByAddingYears (nint year, nint month, nint day, nint hour, nint minute, nint second);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("dayOfCommonEra")]
		nint DayOfCommonEra { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("dayOfMonth")]
		nint DayOfMonth { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("dayOfWeek")]
		nint DayOfWeek { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("dayOfYear")]
		nint DayOfYear { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("hourOfDay")]
		nint HourOfDay { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("minuteOfHour")]
		nint MinuteOfHour { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("monthOfYear")]
		nint MonthOfYear { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("secondOfMinute")]
		nint SecondOfMinute { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("yearOfCommonEra")]
		nint YearOfCommonEra { get; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("calendarFormat")]
		string CalendarFormat { get; set; }

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("descriptionWithCalendarFormat:locale:")]
		string GetDescription (string calendarFormat, NSObject locale);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("descriptionWithCalendarFormat:")]
		string GetDescription (string calendarFormat);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("descriptionWithLocale:")]
		string GetDescription (NSLocale locale);

		[Availability (Deprecated = Platform.Mac_10_10)]
		[Export ("timeZone")]
		NSTimeZone TimeZone { get; set; }
	}
#endif

	[BaseType (typeof (NSObject))]
	interface NSCharacterSet : NSSecureCoding, NSMutableCopying {
		[Static, Export ("alphanumericCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Alphanumerics {get;}

		[Static, Export ("capitalizedLetterCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Capitalized {get;}

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
		NSCharacterSet Controls {get;}

		[Static, Export ("decimalDigitCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet DecimalDigits {get;}

		[Static, Export ("decomposableCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Decomposables {get;}

		[Static, Export ("illegalCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Illegals {get;}

		[Static, Export ("letterCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Letters {get;}

		[Static, Export ("lowercaseLetterCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet LowercaseLetters {get;}

		[Static, Export ("newlineCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Newlines {get;}

		[Static, Export ("nonBaseCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Marks {get;}

		[Static, Export ("punctuationCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Punctuation {get;}

		[Static, Export ("symbolCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Symbols {get;}

		[Static, Export ("uppercaseLetterCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UppercaseLetters {get;}

		[Static, Export ("whitespaceAndNewlineCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet WhitespaceAndNewlines {get;}

		[Static, Export ("whitespaceCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet Whitespaces {get;}

		[Export ("bitmapRepresentation")]
		NSData GetBitmapRepresentation ();

		[Export ("characterIsMember:")]
		bool Contains (char aCharacter);

		[Export ("hasMemberInPlane:")]
		bool HasMemberInPlane (byte thePlane);

		[Export ("invertedSet")]
		NSCharacterSet InvertedSet {get;}

		[Export ("isSupersetOfSet:")]
		bool IsSupersetOf (NSCharacterSet theOtherSet);

		[Export ("longCharacterIsMember:")]
		bool Contains (uint /* UTF32Char = UInt32 */ theLongChar);
	}

	[iOS (8,0), Mac(10,10)]
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
	
#if !MONOMAC

	// Already exists in MonoMac: from from foundation-desktop?
	
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

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("alphanumericCharacterSet")]
		NSCharacterSet Alphanumerics {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("capitalizedLetterCharacterSet")]
		NSCharacterSet Capitalized {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("characterSetWithBitmapRepresentation:")]
		NSCharacterSet FromBitmapRepresentation (NSData data);

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("characterSetWithCharactersInString:")]
		NSCharacterSet FromString (string aString);

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("characterSetWithContentsOfFile:")]
		NSCharacterSet FromFile (string path);

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("characterSetWithRange:")]
		NSCharacterSet FromRange (NSRange aRange);

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("controlCharacterSet")]
		NSCharacterSet Controls {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("decimalDigitCharacterSet")]
		NSCharacterSet DecimalDigits {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("decomposableCharacterSet")]
		NSCharacterSet Decomposables {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("illegalCharacterSet")]
		NSCharacterSet Illegals {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("letterCharacterSet")]
		NSCharacterSet Letters {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("lowercaseLetterCharacterSet")]
		NSCharacterSet LowercaseLetters {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("newlineCharacterSet")]
		NSCharacterSet Newlines {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("nonBaseCharacterSet")]
		NSCharacterSet Marks {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("punctuationCharacterSet")]
		NSCharacterSet Punctuation {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("symbolCharacterSet")]
		NSCharacterSet Symbols {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("uppercaseLetterCharacterSet")]
		NSCharacterSet UppercaseLetters {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("whitespaceAndNewlineCharacterSet")]
		NSCharacterSet WhitespaceAndNewlines {get;}

		[Mac(10,10)][iOS(8,0)]
		[Static, Export ("whitespaceCharacterSet")]
		NSCharacterSet Whitespaces {get;}
	}
#endif
	
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

#if XAMCORE_2_0
		[Export ("encodeInteger:forKey:")]
		void Encode (nint val, string key);
#endif

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

#if XAMCORE_2_0
		[Export ("decodeIntegerForKey:")]
		nint DecodeNInt (string key);
#endif

		[Export ("decodeObjectForKey:")]
		NSObject DecodeObject (string key);

		[Export ("decodeBytesForKey:returnedLength:")]
		IntPtr DecodeBytes (string key, out nuint length);

		[Export ("decodeBytesWithReturnedLength:")]
		IntPtr DecodeBytes (out nuint length);

		[iOS (6,0)]
		[Export ("allowedClasses")]
		NSSet AllowedClasses { get; }

		[iOS (6,0)]
		[Export ("requiresSecureCoding")]
		bool RequiresSecureCoding ();

		[iOS (9,0), Mac (10,11)]
		[Export ("decodeTopLevelObjectAndReturnError:")]
		NSObject DecodeTopLevelObject (out NSError error);

		[iOS (9,0), Mac (10,11)]
		[Export ("decodeTopLevelObjectForKey:error:")]
		NSObject DecodeTopLevelObject (string key, out NSError error);

		[iOS (9,0), Mac (10,11)]
		[Export ("decodeTopLevelObjectOfClass:forKey:error:")]
		NSObject DecodeTopLevelObject (Class klass, string key, out NSError error);

		[iOS (9,0), Mac (10,11)]
		[Export ("decodeTopLevelObjectOfClasses:forKey:error:")]
		NSObject DecodeTopLevelObject ([NullAllowed] NSSet<Class> setOfClasses, string key, out NSError error);

		[iOS (9,0), Mac (10,11)]
		[Export ("failWithError:")]
		void Fail (NSError error);

		[Export ("systemVersion")]
		uint SystemVersion { get; }

		[iOS (9,0)][Mac (10,11)]
		[Export ("decodingFailurePolicy")]
		NSDecodingFailurePolicy DecodingFailurePolicy { get; }

		[iOS (9,0)][Mac (10,11)]
		[NullAllowed, Export ("error", ArgumentSemantic.Copy)]
		NSError Error { get; }
	}
	
	[BaseType (typeof (NSPredicate))]
	interface NSComparisonPredicate : NSSecureCoding {
		[Static, Export ("predicateWithLeftExpression:rightExpression:modifier:type:options:")]
		NSComparisonPredicate Create (NSExpression leftExpression, NSExpression rightExpression, NSComparisonPredicateModifier comparisonModifier, NSPredicateOperatorType operatorType, NSComparisonPredicateOptions comparisonOptions);

		[Static, Export ("predicateWithLeftExpression:rightExpression:customSelector:")]
		NSComparisonPredicate FromSelector (NSExpression leftExpression, NSExpression rightExpression, Selector selector);

		[DesignatedInitializer]
		[Export ("initWithLeftExpression:rightExpression:modifier:type:options:")]
		IntPtr Constructor (NSExpression leftExpression, NSExpression rightExpression, NSComparisonPredicateModifier comparisonModifier, NSPredicateOperatorType operatorType, NSComparisonPredicateOptions comparisonOptions);
		
		[DesignatedInitializer]
		[Export ("initWithLeftExpression:rightExpression:customSelector:")]
		IntPtr Constructor (NSExpression leftExpression, NSExpression rightExpression, Selector selector);

		[Export ("predicateOperatorType")]
		NSPredicateOperatorType PredicateOperatorType { get; }

		[Export ("comparisonPredicateModifier")]
		NSComparisonPredicateModifier ComparisonPredicateModifier { get; }

		[Export ("leftExpression")]
		NSExpression LeftExpression { get; }

		[Export ("rightExpression")]
		NSExpression RightExpression { get; }

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
		IntPtr Constructor (NSCompoundPredicateType type, NSPredicate[] subpredicates);

		[Export ("compoundPredicateType")]
		NSCompoundPredicateType Type { get; }

		[Export ("subpredicates")]
		NSPredicate[] Subpredicates { get; } 

		[Static]
		[Export ("andPredicateWithSubpredicates:")]
		NSCompoundPredicate CreateAndPredicate (NSPredicate[] subpredicates);

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

		[Export ("dataWithContentsOfFile:")][Static]
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
#if XAMCORE_2_0
		[Internal]
#endif
		bool _Save (string file, nint options, IntPtr addr);
		
		[Export ("writeToURL:options:error:")]
#if XAMCORE_2_0
		[Internal]
#endif
		bool _Save (NSUrl url, nint options, IntPtr addr);

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

		[iOS (7,0), Mac (10, 9)] // 10.9
		[Export ("initWithBase64EncodedString:options:")]
		IntPtr Constructor (string base64String, NSDataBase64DecodingOptions options);

		[iOS (7,0), Mac (10, 9)] // 10.9
		[Export ("initWithBase64EncodedData:options:")]
		IntPtr Constructor (NSData base64Data, NSDataBase64DecodingOptions options);

		[iOS (7,0), Mac (10, 9)] // 10.9
		[Export ("base64EncodedDataWithOptions:")]
		NSData GetBase64EncodedData (NSDataBase64EncodingOptions options);

		[iOS (7,0), Mac (10, 9)] // 10.9
		[Export ("base64EncodedStringWithOptions:")]
		string GetBase64EncodedString (NSDataBase64EncodingOptions options);

		[iOS (7,0), Mac (10, 9)]
		[Export ("enumerateByteRangesUsingBlock:")]
		void EnumerateByteRange (NSDataByteRangeEnumerator enumerator);

		[iOS (7,0), Mac (10, 9)]
		[Export ("initWithBytesNoCopy:length:deallocator:")]
		IntPtr Constructor (IntPtr bytes, nuint length, Action<IntPtr,nuint> deallocator);
	}

	[BaseType (typeof (NSRegularExpression))]
	interface NSDataDetector : NSCopying, NSCoding {
		// Invalid parent ctor: -[NSDataDetector initWithPattern:options:error:]: Not valid for NSDataDetector
//		[Export ("initWithPattern:options:error:")]
//		IntPtr Constructor (NSString pattern, NSRegularExpressionOptions options, out NSError error);

		[Export ("dataDetectorWithTypes:error:"), Static]
		NSDataDetector Create (NSTextCheckingTypes checkingTypes, out NSError error);

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
		[Mac(10,7)]
		nint Nanosecond { get; set; }

		[Export ("week")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use 'WeekOfMonth' or 'WeekOfYear' instead.")]
		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'WeekOfMonth' or 'WeekOfYear' instead.")]
		nint Week { get; set; }

		[Export ("weekday")]
		nint Weekday { get; set; }

		[Export ("weekdayOrdinal")]
		nint WeekdayOrdinal { get; set; }

		[Mac(10,7)]
		[Export ("weekOfMonth")]
		nint WeekOfMonth { get; set; }

		[Mac(10,7)]
		[Export ("weekOfYear")]
		nint WeekOfYear { get; set; }
		
		[Mac(10,7)]
		[Export ("yearForWeekOfYear")]
		nint YearForWeekOfYear { get; set; }

		[Mac(10,8)][iOS(6,0)]
		[Export ("leapMonth")]
		bool IsLeapMonth { [Bind ("isLeapMonth")] get; set; }

		[Export ("isValidDate")]
		[Mac(10,9)][iOS(8,0)]
		bool IsValidDate { get; }

		[Export ("isValidDateInCalendar:")]
		[Mac(10,9)][iOS(8,0)]
		bool IsValidDateInCalendar (NSCalendar calendar);

		[Export ("setValue:forComponent:")]
		[Mac(10,9)][iOS(8,0)]
		void SetValueForComponent (nint value, NSCalendarUnit unit);

		[Export ("valueForComponent:")]
		[Mac(10,9)][iOS(8,0)]
		nint GetValueForComponent (NSCalendarUnit unit);
	}
	
	[iOS (6,0)]
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
		bool Adaptive { [Bind ("isAdaptive")] get; set;  }

		[Export ("zeroPadsFractionDigits")]
		bool ZeroPadsFractionDigits { get; set;  }

		[Static]
		[Export ("stringFromByteCount:countStyle:")]
		string Format (long byteCount, NSByteCountFormatterCountStyle countStyle);

		[Export ("stringFromByteCount:")]
		string Format (long byteCount);

		[Export ("allowedUnits")]
		NSByteCountFormatterUnits AllowedUnits { get; set; }

		[Export ("countStyle")]
		NSByteCountFormatterCountStyle CountStyle { get; set; }

		[iOS (8,0), Mac(10,10)]
		[Export ("formattingContext")]
		NSFormattingContext FormattingContext { get; set; }
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
		string GetDateFormatFromTemplate (string template, nuint options, NSLocale locale);

		[Export ("doesRelativeDateFormatting")]
		bool DoesRelativeDateFormatting { get; set; }

		[iOS (8,0), Mac (10,10)]
		[Export ("setLocalizedDateFormatFromTemplate:")]
		void SetLocalizedDateFormatFromTemplate (string dateFormatTemplate);

		[Watch (2, 0), TV (9, 0), Mac (10, 10), iOS (8, 0)]
		[Export ("formattingContext", ArgumentSemantic.Assign)]
		NSFormattingContext FormattingContext { get; set; }
	}

	[iOS (8,0)][Mac(10,10)]
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

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("referenceDate", ArgumentSemantic.Copy)]
		NSDate ReferenceDate { get; set; }
	}

	[iOS (8,0)][Mac(10,10)]
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

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Export ("stringFromDateInterval:")]
		[return: NullAllowed]
		string ToString (NSDateInterval dateInterval);
	}

	[iOS (8,0)][Mac(10,10)]
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

#if !XAMCORE_2_0
	delegate void NSFileHandleUpdateHandler (NSFileHandle handle);
#endif

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
		
		[Export ("readDataToEndOfFile")]
		NSData ReadDataToEndOfFile ();

		[Export ("readDataOfLength:")]
		NSData ReadDataOfLength (nuint length);

		[Export ("writeData:")]
		void WriteData (NSData data);

		[Export ("offsetInFile")]
		ulong OffsetInFile ();

		[Export ("seekToEndOfFile")]
		ulong SeekToEndOfFile ();

		[Export ("seekToFileOffset:")]
		void SeekToFileOffset (ulong offset);

		[Export ("truncateFileAtOffset:")]
		void TruncateFileAtOffset (ulong offset);

		[Export ("synchronizeFile")]
		void SynchronizeFile ();

		[Export ("closeFile")]
		void CloseFile ();
		
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
		IntPtr Constructor (int /* int, not NSInteger */ fd, bool closeOnDealloc);
		
		[Export ("initWithFileDescriptor:")]
		IntPtr Constructor (int /* int, not NSInteger */ fd);

		[Export ("fileDescriptor")]
		int FileDescriptor { get; } /* int, not NSInteger */

		[Export ("setReadabilityHandler:")]
#if XAMCORE_2_0
		void SetReadabilityHandler ([NullAllowed] Action<NSFileHandle> readCallback);
#else
		void SetReadabilityHandler ([NullAllowed] NSFileHandleUpdateHandler readCallback);
#endif

		[Export ("setWriteabilityHandler:")]
#if XAMCORE_2_0
		void SetWriteabilityHandle ([NullAllowed] Action<NSFileHandle> writeCallback);
#else
		void SetWriteabilityHandle ([NullAllowed] NSFileHandleUpdateHandler writeCallback);
#endif

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

	[iOS (9,0), Mac(10,11)]
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
	

	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(NSObject))]
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

	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(NSFormatter))]
	interface NSPersonNameComponentsFormatter
	{
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

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Export ("personNameComponentsFromString:")]
		[return: NullAllowed]
		NSPersonNameComponents GetComponents (string @string);
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

#if XAMCORE_2_0
		[Internal]
		[Sealed]
		[Export ("attributedStringForObjectValue:withDefaultAttributes:")]
		NSAttributedString GetAttributedString (NSObject obj, NSDictionary defaultAttributes);
#endif

		// -(NSAttributedString *)attributedStringForObjectValue:(id)obj withDefaultAttributes:(NSDictionary *)attrs;
		[Export ("attributedStringForObjectValue:withDefaultAttributes:")]
		NSAttributedString GetAttributedString (NSObject obj, NSDictionary<NSString, NSObject> defaultAttributes);

		[Wrap ("GetAttributedString (obj, defaultAttributes == null ? null : defaultAttributes.Dictionary)")]
#if MONOMAC
		NSAttributedString GetAttributedString (NSObject obj, NSStringAttributes defaultAttributes);
#else
		NSAttributedString GetAttributedString (NSObject obj, UIStringAttributes defaultAttributes);
#endif

		[Export ("getObjectValue:forString:errorDescription:")]
		bool GetObjectValue (out NSObject obj, string str, out NSString error);

		[Export ("isPartialStringValid:newEditingString:errorDescription:")]
		bool IsPartialStringValid (string partialString, out string newString, out NSString error);

		[Export ("isPartialStringValid:proposedSelectedRange:originalString:originalSelectedRange:errorDescription:")]
		bool IsPartialStringValid (ref string partialString, out NSRange proposedSelRange, string origString, NSRange origSelRange, out string error);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSCoding {
#if XAMCORE_2_0
		// [Abstract]
		[Export ("initWithCoder:")]
		IntPtr Constructor (NSCoder decoder);

		[Abstract]
		[Export ("encodeWithCoder:")]
		void EncodeTo (NSCoder encoder);
#endif
	}

	[Protocol]
	interface NSSecureCoding : NSCoding {
		// note: +supportsSecureCoding being static it is not a good "generated" binding candidate
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSCopying {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("copyWithZone:")]
		NSObject Copy ([NullAllowed] NSZone zone);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSMutableCopying : NSCopying {
#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("mutableCopyWithZone:")]
		[return: Release ()]
		NSObject MutableCopy ([NullAllowed] NSZone zone);
	}

	interface INSMutableCopying {}

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
		   Delegates=new string [] {"WeakDelegate"},
		   Events=new Type [] { typeof (NSKeyedArchiverDelegate) })]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[NSKeyedArchiver init]: cannot use -init for initialization
	[DisableDefaultCtor]
	interface NSKeyedArchiver {

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("initRequiringSecureCoding:")]
		IntPtr Constructor (bool requiresSecureCoding);

		// hack so we can decorate the default .ctor with availability attributes
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[iOS (10,0)][TV (10,0)][Watch (3,0)][Mac (10,12)]
		[Export ("init")]
		IntPtr Constructor ();

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'NSKeyedArchiver (bool)' instead.")]
		[Export ("initForWritingWithMutableData:")]
		IntPtr Constructor (NSMutableData data);
	
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("archivedDataWithRootObject:requiringSecureCoding:error:")]
		[return: NullAllowed]
#if XAMCORE_4_0
		NSData GetArchivedData (NSObject @object, bool requiresSecureCoding, [NullAllowed] out NSError error);
#else
		NSData ArchivedDataWithRootObject (NSObject @object, bool requiresSecureCoding, [NullAllowed] out NSError error);
#endif

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Export ("archivedDataWithRootObject:")]
		[Static]
#if XAMCORE_4_0
		NSData GetArchivedData (NSObject root);
#else
		NSData ArchivedDataWithRootObject (NSObject root);
#endif
		
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'ArchivedDataWithRootObject (NSObject, bool, out NSError)' instead.")]
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

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("setClassName:forClass:")]
		void SetClassName (string name, Class kls);

		[Export ("classNameForClass:")]
		string GetClassName (Class kls);

		[iOS (7,0), Mac (10, 9)]
		[Field ("NSKeyedArchiveRootObjectKey")]
		NSString RootObjectKey { get; }

		[iOS (6,0), Mac (10, 8)] // Yup, right, this is being "back-supported" to iOS 6
		[Export ("setRequiresSecureCoding:")]
		void SetRequiresSecureCoding (bool requireSecureEncoding);

		[iOS (6,0), Mac(10,8)] // Yup, right, this is being back-supported to iOS 6
		[Export ("requiresSecureCoding")]
		bool GetRequiresSecureCoding ();

		[Watch (3,0)][TV (10,0)][Mac (10, 12)][iOS (10,0)]
		[Export ("encodedData", ArgumentSemantic.Strong)]
		NSData EncodedData { get; }
	}
	
	[BaseType (typeof (NSCoder),
		   Delegates=new string [] {"WeakDelegate"},
		   Events=new Type [] { typeof (NSKeyedUnarchiverDelegate) })]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[NSKeyedUnarchiver init]: cannot use -init for initialization
	[DisableDefaultCtor]
	interface NSKeyedUnarchiver {
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("initForReadingFromData:error:")]
		IntPtr Constructor (NSData data, [NullAllowed] out NSError error);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("unarchivedObjectOfClass:fromData:error:")]
		[return: NullAllowed]
		NSObject GetUnarchivedObject (Class cls, NSData data, [NullAllowed] out NSError error);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Static]
		[Wrap ("GetUnarchivedObject (new Class (type), data, out error)")]
		[return: NullAllowed]
		NSObject GetUnarchivedObject (Type type, NSData data, [NullAllowed] out NSError error);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("unarchivedObjectOfClasses:fromData:error:")]
		[return: NullAllowed]
		NSObject GetUnarchivedObject (NSSet<Class> classes, NSData data, [NullAllowed] out NSError error);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Static]
		[Wrap ("GetUnarchivedObject (new NSSet<Class> (Array.ConvertAll (types, t => new Class (t))), data, out error)")]
		[return: NullAllowed]
		NSObject GetUnarchivedObject (Type [] types, NSData data, [NullAllowed] out NSError error);

		[Export ("initForReadingWithData:")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'NSKeyedUnarchiver (NSData, out NSError)' instead.")]
		[MarshalNativeExceptions]
		IntPtr Constructor (NSData data);
	
		[Static, Export ("unarchiveObjectWithData:")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GetUnarchivedObject ()' instead.")]
		[MarshalNativeExceptions]
		NSObject UnarchiveObject (NSData data);

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Static, Export ("unarchiveTopLevelObjectWithData:error:")]
		[iOS (9,0), Mac(10,11)]
		// FIXME: [MarshalNativeExceptions]
		NSObject UnarchiveTopLevelObject (NSData data, out NSError error);
		
		[Static, Export ("unarchiveObjectWithFile:")]
		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'GetUnarchivedObject ()' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'GetUnarchivedObject ()' instead.")]
		[MarshalNativeExceptions]
		NSObject UnarchiveFile (string file);

		[Export ("finishDecoding")]
		void FinishDecoding ();

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSKeyedUnarchiverDelegate Delegate { get; set; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Export ("setClass:forClassName:")]
		void SetClass (Class kls, string codedName);

		[Export ("classForClassName:")]
		Class GetClass (string codedName);

		[iOS (6,0), Mac (10, 8)] // Yup, right, this is being "back-supported" to iOS 6
		[Export ("setRequiresSecureCoding:")]
		void SetRequiresSecureCoding (bool requireSecureEncoding);

		[iOS (6,0), Mac(10,8)] // Yup, right, this is being back-supported to iOS 6
		[Export ("requiresSecureCoding")]
		bool GetRequiresSecureCoding ();

	}

	[BaseType (typeof (NSObject), Delegates=new string [] { "Delegate" }, Events=new Type [] { typeof (NSMetadataQueryDelegate)})]
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
		NSMetadataItem[] Results { get; }

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
		NSSortDescriptor[] SortDescriptors { get; set; }

		[Export ("valueListAttributes", ArgumentSemantic.Copy)]
		NSObject[] ValueListAttributes { get; set; }

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
#if MONOMAC
		[Field ("NSMetadataQueryUserHomeScope")]
		NSString UserHomeScope { get; }
		
		[Field ("NSMetadataQueryLocalComputerScope")]
		NSString LocalComputerScope { get; }

#if !XAMCORE_2_0
		[Field ("NSMetadataQueryNetworkScope")]
		[Obsolete ("Use NetworkScope")]
		NSString QueryNetworkScope { get; }

		[Field ("NSMetadataQueryLocalDocumentsScope")]
		[Obsolete ("Use LocalDocumentsScope")]
		NSString QueryLocalDocumentsScope { get; }

#endif 
		[Field ("NSMetadataQueryLocalDocumentsScope")]
		NSString LocalDocumentsScope { get; }

		[Field ("NSMetadataQueryNetworkScope")]
		NSString NetworkScope { get; }

#endif

#if !XAMCORE_2_0
		[Field ("NSMetadataQueryUbiquitousDocumentsScope")]
		[Obsolete ("Use 'UbiquitousDocumentsScope' instead.")]
		NSString QueryUbiquitousDocumentsScope { get; }

		[Field ("NSMetadataQueryUbiquitousDataScope")]
		[Obsolete ("Use 'UbiquitousDataScope' instead.")]
		NSString QueryUbiquitousDataScope { get; }
#endif

		[Field ("NSMetadataQueryUbiquitousDocumentsScope")]
		NSString UbiquitousDocumentsScope { get; }

		[Field ("NSMetadataQueryUbiquitousDataScope")]
		NSString UbiquitousDataScope { get; }


		[iOS(8,0),Mac(10,10)]
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

		[iOS(8,0),Mac(10,9)]
		[Field ("NSMetadataItemContentTypeKey")]
		NSString ContentTypeKey { get; }

		[iOS(8,0),Mac(10,9)]
		[Field ("NSMetadataItemContentTypeTreeKey")]
		NSString ContentTypeTreeKey { get; }
		

		[Field ("NSMetadataItemIsUbiquitousKey")]
		NSString ItemIsUbiquitousKey { get; }

		[Field ("NSMetadataUbiquitousItemHasUnresolvedConflictsKey")]
		NSString UbiquitousItemHasUnresolvedConflictsKey { get; }

		[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'UbiquitousItemDownloadingStatusKey' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 9, message : "Use 'UbiquitousItemDownloadingStatusKey' instead.")]
		[Field ("NSMetadataUbiquitousItemIsDownloadedKey")]
		NSString UbiquitousItemIsDownloadedKey { get; }

		[Field ("NSMetadataUbiquitousItemIsDownloadingKey")]
		NSString UbiquitousItemIsDownloadingKey { get; }

		[Field ("NSMetadataUbiquitousItemIsUploadedKey")]
		NSString UbiquitousItemIsUploadedKey { get; }

		[Field ("NSMetadataUbiquitousItemIsUploadingKey")]
		NSString UbiquitousItemIsUploadingKey { get; }

		[iOS (7,0)]
		[Mac (10,9)]
		[Field ("NSMetadataUbiquitousItemDownloadingStatusKey")]
		NSString UbiquitousItemDownloadingStatusKey { get; }

		[iOS (7,0)]
		[Mac (10,9)]
		[Field ("NSMetadataUbiquitousItemDownloadingErrorKey")]
		NSString UbiquitousItemDownloadingErrorKey { get; }

		[iOS (7,0)]
		[Mac (10,9)]
		[Field ("NSMetadataUbiquitousItemUploadingErrorKey")]
		NSString UbiquitousItemUploadingErrorKey { get; }

		[Field ("NSMetadataUbiquitousItemPercentDownloadedKey")]
		NSString UbiquitousItemPercentDownloadedKey { get; }

		[Field ("NSMetadataUbiquitousItemPercentUploadedKey")]
		NSString UbiquitousItemPercentUploadedKey { get; }

		[iOS(8,0),Mac(10,10)]
		[Field ("NSMetadataUbiquitousItemDownloadRequestedKey")]
		NSString UbiquitousItemDownloadRequestedKey { get; }

		[iOS(8,0),Mac(10,10)]
		[Field ("NSMetadataUbiquitousItemIsExternalDocumentKey")]
		NSString UbiquitousItemIsExternalDocumentKey { get; }
		
		[iOS(8,0),Mac(10,10)]
		[Field ("NSMetadataUbiquitousItemContainerDisplayNameKey")]
		NSString UbiquitousItemContainerDisplayNameKey { get; }
		
		[iOS(8,0),Mac(10,10)]
		[Field ("NSMetadataUbiquitousItemURLInLocalContainerKey")]
		NSString UbiquitousItemURLInLocalContainerKey { get; }

#if MONOMAC
		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemKeywordsKey")]
		NSString KeywordsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemTitleKey")]
		NSString TitleKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAuthorsKey")]
		NSString AuthorsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemEditorsKey")]
		NSString EditorsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemParticipantsKey")]
		NSString ParticipantsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemProjectsKey")]
		NSString ProjectsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemDownloadedDateKey")]
		NSString DownloadedDateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemWhereFromsKey")]
		NSString WhereFromsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemCommentKey")]
		NSString CommentKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemCopyrightKey")]
		NSString CopyrightKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemLastUsedDateKey")]
		NSString LastUsedDateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemContentCreationDateKey")]
		NSString ContentCreationDateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemContentModificationDateKey")]
		NSString ContentModificationDateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemDateAddedKey")]
		NSString DateAddedKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemDurationSecondsKey")]
		NSString DurationSecondsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemContactKeywordsKey")]
		NSString ContactKeywordsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemVersionKey")]
		NSString VersionKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPixelHeightKey")]
		NSString PixelHeightKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPixelWidthKey")]
		NSString PixelWidthKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPixelCountKey")]
		NSString PixelCountKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemColorSpaceKey")]
		NSString ColorSpaceKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemBitsPerSampleKey")]
		NSString BitsPerSampleKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemFlashOnOffKey")]
		NSString FlashOnOffKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemFocalLengthKey")]
		NSString FocalLengthKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAcquisitionMakeKey")]
		NSString AcquisitionMakeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAcquisitionModelKey")]
		NSString AcquisitionModelKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemISOSpeedKey")]
		NSString IsoSpeedKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemOrientationKey")]
		NSString OrientationKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemLayerNamesKey")]
		NSString LayerNamesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemWhiteBalanceKey")]
		NSString WhiteBalanceKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemApertureKey")]
		NSString ApertureKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemProfileNameKey")]
		NSString ProfileNameKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemResolutionWidthDPIKey")]
		NSString ResolutionWidthDpiKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemResolutionHeightDPIKey")]
		NSString ResolutionHeightDpiKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemExposureModeKey")]
		NSString ExposureModeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemExposureTimeSecondsKey")]
		NSString ExposureTimeSecondsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemEXIFVersionKey")]
		NSString ExifVersionKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemCameraOwnerKey")]
		NSString CameraOwnerKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemFocalLength35mmKey")]
		NSString FocalLength35mmKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemLensModelKey")]
		NSString LensModelKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemEXIFGPSVersionKey")]
		NSString ExifGpsVersionKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAltitudeKey")]
		NSString AltitudeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemLatitudeKey")]
		NSString LatitudeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemLongitudeKey")]
		NSString LongitudeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemSpeedKey")]
		NSString SpeedKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemTimestampKey")]
		NSString TimestampKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSTrackKey")]
		NSString GpsTrackKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemImageDirectionKey")]
		NSString ImageDirectionKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemNamedLocationKey")]
		NSString NamedLocationKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSStatusKey")]
		NSString GpsStatusKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSMeasureModeKey")]
		NSString GpsMeasureModeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDOPKey")]
		NSString GpsDopKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSMapDatumKey")]
		NSString GpsMapDatumKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDestLatitudeKey")]
		NSString GpsDestLatitudeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDestLongitudeKey")]
		NSString GpsDestLongitudeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDestBearingKey")]
		NSString GpsDestBearingKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDestDistanceKey")]
		NSString GpsDestDistanceKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSProcessingMethodKey")]
		NSString GpsProcessingMethodKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSAreaInformationKey")]
		NSString GpsAreaInformationKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDateStampKey")]
		NSString GpsDateStampKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGPSDifferentalKey")]
		NSString GpsDifferentalKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemCodecsKey")]
		NSString CodecsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemMediaTypesKey")]
		NSString MediaTypesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemStreamableKey")]
		NSString StreamableKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemTotalBitRateKey")]
		NSString TotalBitRateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemVideoBitRateKey")]
		NSString VideoBitRateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAudioBitRateKey")]
		NSString AudioBitRateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemDeliveryTypeKey")]
		NSString DeliveryTypeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAlbumKey")]
		NSString AlbumKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemHasAlphaChannelKey")]
		NSString HasAlphaChannelKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemRedEyeOnOffKey")]
		NSString RedEyeOnOffKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemMeteringModeKey")]
		NSString MeteringModeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemMaxApertureKey")]
		NSString MaxApertureKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemFNumberKey")]
		NSString FNumberKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemExposureProgramKey")]
		NSString ExposureProgramKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemExposureTimeStringKey")]
		NSString ExposureTimeStringKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemHeadlineKey")]
		NSString HeadlineKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemInstructionsKey")]
		NSString InstructionsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemCityKey")]
		NSString CityKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemStateOrProvinceKey")]
		NSString StateOrProvinceKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemCountryKey")]
		NSString CountryKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemTextContentKey")]
		NSString TextContentKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAudioSampleRateKey")]
		NSString AudioSampleRateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAudioChannelCountKey")]
		NSString AudioChannelCountKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemTempoKey")]
		NSString TempoKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemKeySignatureKey")]
		NSString KeySignatureKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemTimeSignatureKey")]
		NSString TimeSignatureKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAudioEncodingApplicationKey")]
		NSString AudioEncodingApplicationKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemComposerKey")]
		NSString ComposerKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemLyricistKey")]
		NSString LyricistKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAudioTrackNumberKey")]
		NSString AudioTrackNumberKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemRecordingDateKey")]
		NSString RecordingDateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemMusicalGenreKey")]
		NSString MusicalGenreKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemIsGeneralMIDISequenceKey")]
		NSString IsGeneralMidiSequenceKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemRecordingYearKey")]
		NSString RecordingYearKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemOrganizationsKey")]
		NSString OrganizationsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemLanguagesKey")]
		NSString LanguagesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemRightsKey")]
		NSString RightsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPublishersKey")]
		NSString PublishersKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemContributorsKey")]
		NSString ContributorsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemCoverageKey")]
		NSString CoverageKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemSubjectKey")]
		NSString SubjectKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemThemeKey")]
		NSString ThemeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemDescriptionKey")]
		NSString DescriptionKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemIdentifierKey")]
		NSString IdentifierKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAudiencesKey")]
		NSString AudiencesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemNumberOfPagesKey")]
		NSString NumberOfPagesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPageWidthKey")]
		NSString PageWidthKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPageHeightKey")]
		NSString PageHeightKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemSecurityMethodKey")]
		NSString SecurityMethodKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemCreatorKey")]
		NSString CreatorKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemEncodingApplicationsKey")]
		NSString EncodingApplicationsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemDueDateKey")]
		NSString DueDateKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemStarRatingKey")]
		NSString StarRatingKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPhoneNumbersKey")]
		NSString PhoneNumbersKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemEmailAddressesKey")]
		NSString EmailAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemInstantMessageAddressesKey")]
		NSString InstantMessageAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemKindKey")]
		NSString KindKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemRecipientsKey")]
		NSString RecipientsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemFinderCommentKey")]
		NSString FinderCommentKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemFontsKey")]
		NSString FontsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAppleLoopsRootKeyKey")]
		NSString AppleLoopsRootKeyKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAppleLoopsKeyFilterTypeKey")]
		NSString AppleLoopsKeyFilterTypeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAppleLoopsLoopModeKey")]
		NSString AppleLoopsLoopModeKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAppleLoopDescriptorsKey")]
		NSString AppleLoopDescriptorsKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemMusicalInstrumentCategoryKey")]
		NSString MusicalInstrumentCategoryKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemMusicalInstrumentNameKey")]
		NSString MusicalInstrumentNameKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemCFBundleIdentifierKey")]
		NSString CFBundleIdentifierKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemInformationKey")]
		NSString InformationKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemDirectorKey")]
		NSString DirectorKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemProducerKey")]
		NSString ProducerKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemGenreKey")]
		NSString GenreKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemPerformersKey")]
		NSString PerformersKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemOriginalFormatKey")]
		NSString OriginalFormatKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemOriginalSourceKey")]
		NSString OriginalSourceKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAuthorEmailAddressesKey")]
		NSString AuthorEmailAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemRecipientEmailAddressesKey")]
		NSString RecipientEmailAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemAuthorAddressesKey")]
		NSString AuthorAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemRecipientAddressesKey")]
		NSString RecipientAddressesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemIsLikelyJunkKey")]
		NSString IsLikelyJunkKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemExecutableArchitecturesKey")]
		NSString ExecutableArchitecturesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemExecutablePlatformKey")]
		NSString ExecutablePlatformKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemApplicationCategoriesKey")]
		NSString ApplicationCategoriesKey { get; }

		[NoWatch, NoTV, NoiOS, Mac (10, 9)]
		[Field ("NSMetadataItemIsApplicationManagedKey")]
		NSString IsApplicationManagedKey { get; }
#endif

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSMetadataUbiquitousItemIsSharedKey")]
		NSString UbiquitousItemIsSharedKey { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSMetadataUbiquitousSharedItemCurrentUserRoleKey")]
		NSString UbiquitousSharedItemCurrentUserRoleKey { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSMetadataUbiquitousSharedItemCurrentUserPermissionsKey")]
		NSString UbiquitousSharedItemCurrentUserPermissionsKey { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSMetadataUbiquitousSharedItemOwnerNameComponentsKey")]
		NSString UbiquitousSharedItemOwnerNameComponentsKey { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSMetadataUbiquitousSharedItemMostRecentEditorNameComponentsKey")]
		NSString UbiquitousSharedItemMostRecentEditorNameComponentsKey { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSMetadataUbiquitousSharedItemRoleOwner")]
		NSString UbiquitousSharedItemRoleOwner { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSMetadataUbiquitousSharedItemRoleParticipant")]
		NSString UbiquitousSharedItemRoleParticipant { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSMetadataUbiquitousSharedItemPermissionsReadOnly")]
		NSString UbiquitousSharedItemPermissionsReadOnly { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSMetadataUbiquitousSharedItemPermissionsReadWrite")]
		NSString UbiquitousSharedItemPermissionsReadWrite { get; }
		
		[iOS (7,0), Mac (10, 9)]
		[NullAllowed] // by default this property is null
		[Export ("searchItems", ArgumentSemantic.Copy)]
		// DOC: object is a mixture of NSString, NSMetadataItem, NSUrl
		NSObject [] SearchItems { get; set; }

		[iOS (7,0), Mac (10, 9)]
		[NullAllowed] // by default this property is null
		[Export ("operationQueue", ArgumentSemantic.Retain)]
		NSOperationQueue OperationQueue { get; set; }
		
		[iOS (7,0), Mac (10, 9)]
		[Export ("enumerateResultsUsingBlock:")]
		void EnumerateResultsUsingBlock (NSMetadataQueryEnumerationCallback callback);

		[iOS (7,0), Mac (10, 9), Export ("enumerateResultsWithOptions:usingBlock:")]
		void EnumerateResultsWithOptions (NSEnumerationOptions opts, NSMetadataQueryEnumerationCallback block);

		//
		// These are for NSMetadataQueryDidUpdateNotification 
		//
		[Mac (10,9)][iOS (8,0)]
		[Field ("NSMetadataQueryUpdateAddedItemsKey")]
		NSString QueryUpdateAddedItemsKey { get; }

		[Mac (10,9)][iOS (8,0)]
		[Field ("NSMetadataQueryUpdateChangedItemsKey")]
		NSString QueryUpdateChangedItemsKey { get; }
		
		[Mac (10,9)][iOS (8,0)]
		[Field ("NSMetadataQueryUpdateRemovedItemsKey")]
		NSString QueryUpdateRemovedItemsKey { get; }
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSMetadataQueryDelegate {
		[Export ("metadataQuery:replacementObjectForResultObject:"), DelegateName ("NSMetadataQueryObject"), DefaultValue(null)]
		NSObject ReplacementObjectForResultObject (NSMetadataQuery query, NSMetadataItem result);

		[Export ("metadataQuery:replacementValueForAttribute:value:"), DelegateName ("NSMetadataQueryValue"), DefaultValue(null)]
		NSObject ReplacementValueForAttributevalue (NSMetadataQuery query, string attributeName, NSObject value);
	}

	[BaseType (typeof (NSObject))]
#if XAMCORE_4_0
	[DisableDefaultCtor] // points to nothing so access properties crash the apps
#endif
	interface NSMetadataItem {

		[NoiOS][NoTV][NoWatch]
		[Mac (10,9)]
		[DesignatedInitializer]
		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl url);

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
		IntPtr Constructor (nuint capacity);

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

		[iOS (8,0), Mac(10,10)]
		[Static, Export ("arrayWithContentsOfFile:")]
		NSMutableArray FromFile (string path);

		[iOS (8,0), Mac(10,10)]
		[Static, Export ("arrayWithContentsOfURL:")]
		NSMutableArray FromUrl (NSUrl url);
		
	}
	
	interface NSMutableArray<TValue> : NSMutableArray {}

	[BaseType (typeof (NSAttributedString))]
	interface NSMutableAttributedString {
		[Export ("initWithString:")]
		IntPtr Constructor (string str);
		
		[Export ("initWithString:attributes:")]
		IntPtr Constructor (string str, [NullAllowed] NSDictionary attributes);

		[Export ("initWithAttributedString:")]
		IntPtr Constructor (NSAttributedString other);

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

#if MONOMAC
		[Wrap ("AddAttributes (attributes == null ? null : attributes.Dictionary, range)")]
		void AddAttributes (NSStringAttributes attributes, NSRange range);
#endif
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

#if !MONOMAC
		[NoTV]
		[iOS (7,0)]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'ReadFromUrl' instead.")]
		[Export ("readFromFileURL:options:documentAttributes:error:")]
		bool ReadFromFile (NSUrl url, NSDictionary options, ref NSDictionary returnOptions, ref NSError error);

		[NoTV]
		[Deprecated (PlatformName.iOS, 9, 0, message: "Use 'ReadFromUrl' instead.")]
		[Wrap ("ReadFromFile (url, options == null ? null : options.Dictionary, ref returnOptions, ref error)")]
		bool ReadFromFile (NSUrl url, NSAttributedStringDocumentAttributes options, ref NSDictionary returnOptions, ref NSError error);

		[iOS (7,0)]
		[Export ("readFromData:options:documentAttributes:error:")]
		bool ReadFromData (NSData data, NSDictionary options, ref NSDictionary returnOptions, ref NSError error);
		
		[Wrap ("ReadFromData (data, options == null ? null : options.Dictionary, ref returnOptions, ref error)")]
		bool ReadFromData (NSData data, NSAttributedStringDocumentAttributes options, ref NSDictionary returnOptions, ref NSError error);

#endif

#if XAMCORE_2_0
		[Internal]
		[Sealed]
		[iOS(9,0), Mac(10,11)]
		[Export ("readFromURL:options:documentAttributes:error:")]
		bool ReadFromUrl (NSUrl url, NSDictionary options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);
#endif

		[iOS(9,0), Mac(10,11)]
		[Export ("readFromURL:options:documentAttributes:error:")]
		bool ReadFromUrl (NSUrl url, NSDictionary<NSString, NSObject> options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);

		[iOS(9,0), Mac(10,11)]
		[Wrap ("ReadFromUrl (url, options.Dictionary, ref returnOptions, ref error)")]
		bool ReadFromUrl (NSUrl url, NSAttributedStringDocumentAttributes options, ref NSDictionary<NSString, NSObject> returnOptions, ref NSError error);
	}

	[BaseType (typeof (NSData))]
	interface NSMutableData {
		[Static, Export ("dataWithCapacity:")] [Autorelease]
		[PreSnippet ("if (capacity < 0 || capacity > nint.MaxValue) throw new ArgumentOutOfRangeException ();")]
		NSMutableData FromCapacity (nint capacity);

		[Static, Export ("dataWithLength:")] [Autorelease]
		[PreSnippet ("if (length < 0 || length > nint.MaxValue) throw new ArgumentOutOfRangeException ();")]
		NSMutableData FromLength (nint length);
		
		[Static, Export ("data")] [Autorelease]
		NSMutableData Create ();

		[Export ("mutableBytes")]
		IntPtr MutableBytes { get; }

		[Export ("initWithCapacity:")]
		[PreSnippet ("if (capacity > (ulong) nint.MaxValue) throw new ArgumentOutOfRangeException ();")]
		IntPtr Constructor (nuint capacity);

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
		
	}

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSDate : NSSecureCoding, NSCopying, CKRecordValue {
		[Export ("timeIntervalSinceReferenceDate")]
		double SecondsSinceReferenceDate { get; }

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
	}
	
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSDictionary : NSSecureCoding, NSMutableCopying, NSFetchRequestResult, INSFastEnumeration {
		[Export ("dictionaryWithContentsOfFile:")]
		[Static]
		NSDictionary FromFile (string path);

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
		IntPtr Constructor (NSDictionary other);

		[Export ("initWithDictionary:copyItems:")]
		IntPtr Constructor (NSDictionary other, bool copyItems);

		[Export ("initWithContentsOfFile:")]
		IntPtr Constructor (string fileName);

		[Export ("initWithObjects:forKeys:"), Internal]
		IntPtr Constructor (NSArray objects, NSArray keys);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl url);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("initWithContentsOfURL:error:")]
		IntPtr Constructor (NSUrl url, out NSError error);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
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

		[Export ("allKeys")][Autorelease]
		NSObject [] Keys { get; }

		[Internal]
		[Sealed]
		[Export ("allKeysForObject:")]
		IntPtr _AllKeysForObject (IntPtr obj);

		[Export ("allKeysForObject:")][Autorelease]
		NSObject [] KeysForObject (NSObject obj);

		[Internal]
		[Sealed]
		[Export ("allValues")]
		IntPtr _AllValues ();

		[Export ("allValues")][Autorelease]
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

		[Export ("objectsForKeys:notFoundMarker:")][Autorelease]
		NSObject [] ObjectsForKeys (NSArray keys, NSObject marker);
		
		[Export ("writeToFile:atomically:")]
		bool WriteToFile (string path, bool useAuxiliaryFile);

		[Export ("writeToURL:atomically:")]
		bool WriteToUrl (NSUrl url, bool atomically);

		[iOS (6,0)]
		[Static]
		[Export ("sharedKeySetForKeys:")]
		NSObject GetSharedKeySetForKeys (NSObject [] keys);

	}

	interface NSDictionary<K,V> : NSDictionary {}

	[BaseType (typeof (NSObject))]
	interface NSEnumerator {
		[Export ("nextObject")]
		NSObject NextObject (); 
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSError : NSSecureCoding, NSCopying {
		[Static, Export ("errorWithDomain:code:userInfo:")]
		NSError FromDomain (NSString domain, nint code, [NullAllowed] NSDictionary userInfo);

		[DesignatedInitializer]
		[Export ("initWithDomain:code:userInfo:")]
		IntPtr Constructor (NSString domain, nint code, [NullAllowed] NSDictionary userInfo);
		
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

		[Field ("NSCocoaErrorDomain")]
		NSString CocoaErrorDomain { get;}

		[Field ("NSPOSIXErrorDomain")]
		NSString PosixErrorDomain { get; }

		[Field ("NSOSStatusErrorDomain")]
		NSString OsStatusErrorDomain { get; }

		[Field ("NSMachErrorDomain")]
		NSString MachErrorDomain { get; }

		[Field ("NSURLErrorDomain")]
		NSString NSUrlErrorDomain { get; }

		[Field ("NSNetServicesErrorDomain")]
		NSString NSNetServicesErrorDomain { get; }

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
		[Field ("CMErrorDomain", "CoreMotion")]
		NSString CoreMotionErrorDomain { get; }

#if !XAMCORE_3_0
		// now exposed with the corresponding EABluetoothAccessoryPickerError enum
		[NoMac, NoTV, NoWatch]
		[iOS (6,0)]
		[NoTV]
		[Field ("EABluetoothAccessoryPickerErrorDomain", "ExternalAccessory")]
		NSString EABluetoothAccessoryPickerErrorDomain { get; }

		// now exposed with the corresponding MKErrorCode enum
		[TV (9,2)]
		[NoMac][NoWatch]
		[Field ("MKErrorDomain", "MapKit")]
		NSString MapKitErrorDomain { get; }

		// now exposed with the corresponding WKErrorCode enum
		[NoMac, NoTV]
		[iOS (8,2)]
		[Field ("WatchKitErrorDomain", "WatchKit")]
		NSString WatchKitErrorDomain { get; }
#endif
		
		[Field ("NSUnderlyingErrorKey")]
		NSString UnderlyingErrorKey { get; }

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

		[iOS (9,0)][Mac (10,11)]
		[Field ("NSDebugDescriptionErrorKey")]
		NSString DebugDescriptionErrorKey { get; }

		[iOS (11,0), Mac (10,13), Watch (4,0), TV (11,0)]
		[Field ("NSLocalizedFailureErrorKey")]
		NSString LocalizedFailureErrorKey { get; }

		[iOS (9,0)][Mac (10,11)]
		[Static]
		[Export ("setUserInfoValueProviderForDomain:provider:")]
		void SetUserInfoValueProvider (string errorDomain, [NullAllowed] NSErrorUserInfoValueProvider provider);

		[iOS (9,0)][Mac (10,11)]
		[Static]
		[Export ("userInfoValueProviderForDomain:")]
		[return: NullAllowed]
		NSErrorUserInfoValueProvider GetUserInfoValueProvider (string errorDomain);

#if XAMCORE_2_0 && IOS

		// From NSError (NSFileProviderError) Category to avoid static category uglyness

		[iOS (11,0)]
		[Static]
		[Export ("fileProviderErrorForCollisionWithItem:")]
		NSError GetFileProviderError (INSFileProviderItem existingItem);

		[iOS (11,0)]
		[Static]
		[Export ("fileProviderErrorForNonExistentItemWithIdentifier:")]
		NSError GetFileProviderError (string nonExistentItemIdentifier);
#endif
		
#if false
		// FIXME that value is present in the header (7.0 DP 6) files but returns NULL (i.e. unusable)
		// we're also missing other NSURLError* fields (which we should add)
		[iOS (7,0)]
		[Field ("NSURLErrorBackgroundTaskCancelledReasonKey")]
		NSString NSUrlErrorBackgroundTaskCancelledReasonKey { get; }
#endif
	}

	delegate NSObject NSErrorUserInfoValueProvider (NSError error, NSString userInfoKey);	

	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface NSException : NSCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithName:reason:userInfo:")]
		IntPtr Constructor (string name, string reason, [NullAllowed] NSDictionary userInfo);

		[Export ("name")]
		string Name { get; }
	
		[Export ("reason")]
		string Reason { get; }
		
		[Export ("userInfo")]
		NSObject UserInfo { get; }

		[Export ("callStackReturnAddresses")]
		NSNumber[] CallStackReturnAddresses { get; }

		[Export ("callStackSymbols")]
		string[] CallStackSymbols { get; }
	}

#if !XAMCORE_4_0 && !WATCH
	[Obsolete("NSExpressionHandler is deprecated, please use FromFormat (string, NSObject[]) instead.")]
	delegate void NSExpressionHandler (NSObject evaluatedObject, NSExpression [] expressions, NSMutableDictionary context);
#endif
	delegate NSObject NSExpressionCallbackHandler (NSObject evaluatedObject, NSExpression [] expressions, NSMutableDictionary context);
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -predicateFormat cannot be sent to an abstract object of class NSExpression: Create a concrete instance!
	[DisableDefaultCtor]
	interface NSExpression : NSSecureCoding, NSCopying {
		[Static, Export ("expressionForConstantValue:")]
		NSExpression FromConstant (NSObject obj);

		[Static, Export ("expressionForEvaluatedObject")]
		NSExpression ExpressionForEvaluatedObject { get; }

		[Static, Export ("expressionForVariable:")]
		NSExpression FromVariable (string string1);

		[Static, Export ("expressionForKeyPath:")]
		NSExpression FromKeyPath (string keyPath);

		[Static, Export ("expressionForFunction:arguments:")]
		NSExpression FromFunction (string name, NSExpression[] parameters);

		[Static, Export ("expressionWithFormat:")]
		NSExpression FromFormat (string expressionFormat);

#if !XAMCORE_4_0 && !WATCH
		[Obsolete("Use 'FromFormat (string, NSObject[])' instead.")]
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
		NSExpression FromFunction (NSExpression target, string name, NSExpression[] parameters);

#if !XAMCORE_4_0 && !WATCH
		[Obsolete("Use 'FromFunction (NSExpressionCallbackHandler, NSExpression[])' instead.")]
		[Static, Export ("expressionForBlock:arguments:")]
		NSExpression FromFunction (NSExpressionHandler target, NSExpression[] parameters);
#endif

		[Static, Export ("expressionForBlock:arguments:")]
		NSExpression FromFunction (NSExpressionCallbackHandler target, NSExpression[] parameters);

		[iOS (7,0), Mac (10, 9)]
		[Static]
		[Export ("expressionForAnyKey")]
		NSExpression FromAnyKey ();

		[iOS(9,0),Mac(10,11)]
		[Static]
		[Export ("expressionForConditional:trueExpression:falseExpression:")]
		NSExpression FromConditional (NSPredicate predicate, NSExpression trueExpression, NSExpression falseExpression);
			
		[iOS (7,0), Mac (10, 9)]
		[Export ("allowEvaluation")]
		void AllowEvaluation ();
		
		[DesignatedInitializer]
		[Export ("initWithExpressionType:")]
		IntPtr Constructor (NSExpressionType type);

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
		NSExpression[] _Arguments { get; }

		[Sealed, Internal, Export ("collection")]
		NSObject _Collection { get; }

		[Sealed, Internal, Export ("predicate")]
		NSPredicate _Predicate { get; }

		[Sealed, Internal, Export ("leftExpression")]
		NSExpression _LeftExpression { get; }

		[Sealed, Internal, Export ("rightExpression")]
		NSExpression _RightExpression { get; }

		[Mac(10,11),iOS(9,0)]
		[Sealed, Internal, Export ("trueExpression")]
		NSExpression _TrueExpression { get; }

		[Mac(10,11),iOS(9,0)]
		[Sealed, Internal, Export ("falseExpression")]
		NSExpression _FalseExpression { get; }
		
		[Export ("expressionValueWithObject:context:")]
		[return: NullAllowed]
		NSObject EvaluateWith ([NullAllowed] NSObject obj, [NullAllowed] NSMutableDictionary context);
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
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

#if !MONOMAC
		[iOS (8,2)]
		[Notification]
		[Field ("NSExtensionHostWillEnterForegroundNotification")]
		NSString HostWillEnterForegroundNotification { get; }

		[iOS (8,2)]
		[Notification]
		[Field ("NSExtensionHostDidEnterBackgroundNotification")]
		NSString HostDidEnterBackgroundNotification { get; }

		[iOS (8,2)]
		[Notification]
		[Field ("NSExtensionHostWillResignActiveNotification")]
		NSString HostWillResignActiveNotification { get; }

		[iOS (8,2)]
		[Notification]
		[Field ("NSExtensionHostDidBecomeActiveNotification")]
		NSString HostDidBecomeActiveNotification { get; }
#endif
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
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
		NSNull Null { get; }
	}

	[iOS (8,0)]
	[Mac(10,10)]
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

	[BaseType (typeof (NSObject))]
	interface NSLinguisticTagger {
		[DesignatedInitializer]
		[Export ("initWithTagSchemes:options:")]
		IntPtr Constructor (NSString [] tagSchemes, NSLinguisticTaggerOptions opts);

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

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("tagsInRange:unit:scheme:options:tokenRanges:")]
		string[] GetTags (NSRange range, NSLinguisticTaggerUnit unit, string scheme, NSLinguisticTaggerOptions options, [NullAllowed] out NSValue[] tokenRanges);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("enumerateTagsInRange:unit:scheme:options:usingBlock:")]
		void EnumerateTags (NSRange range, NSLinguisticTaggerUnit unit, string scheme, NSLinguisticTaggerOptions options, LinguisticTagEnumerator enumerator);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("tagAtIndex:unit:scheme:tokenRange:")]
		[return: NullAllowed]
		string GetTag (nuint charIndex, NSLinguisticTaggerUnit unit, string scheme, [NullAllowed] ref NSRange tokenRange);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("tokenRangeAtIndex:unit:")]
		NSRange GetTokenRange (nuint charIndex, NSLinguisticTaggerUnit unit);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("availableTagSchemesForUnit:language:")]
		string[] GetAvailableTagSchemes (NSLinguisticTaggerUnit unit, string language);

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("dominantLanguage")]
		string DominantLanguage { get; }

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("dominantLanguageForString:")]
		[return: NullAllowed]
		string GetDominantLanguage (string str);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("tagForString:atIndex:unit:scheme:orthography:tokenRange:")]
		[return: NullAllowed]
		string GetTag (string str, nuint charIndex, NSLinguisticTaggerUnit unit, string scheme, [NullAllowed] NSOrthography orthography, [NullAllowed] ref NSRange tokenRange);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("tagsForString:range:unit:scheme:options:orthography:tokenRanges:")]
		string[] GetTags (string str, NSRange range, NSLinguisticTaggerUnit unit, string scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, [NullAllowed] out NSValue[] tokenRanges);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Static]
		[Export ("enumerateTagsForString:range:unit:scheme:options:orthography:usingBlock:")]
		void EnumerateTags (string str, NSRange range, NSLinguisticTaggerUnit unit, string scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, LinguisticTagEnumerator enumerator);
	}

	delegate void LinguisticTagEnumerator (string tag, NSRange tokenRange, bool stop);

#if !XAMCORE_4_0
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
		IntPtr Constructor (string identifier);

		[Export ("localeIdentifier")]
		string LocaleIdentifier { get; }

		[Export ("availableLocaleIdentifiers", ArgumentSemantic.Copy)][Static]
		string [] AvailableLocaleIdentifiers { get; }

		[Export ("ISOLanguageCodes", ArgumentSemantic.Copy)][Static]
		string [] ISOLanguageCodes { get; }

		[Export ("ISOCurrencyCodes", ArgumentSemantic.Copy)][Static]
		string [] ISOCurrencyCodes { get; }

		[Export ("ISOCountryCodes", ArgumentSemantic.Copy)][Static]
		string [] ISOCountryCodes { get; }

		[Export ("commonISOCurrencyCodes", ArgumentSemantic.Copy)][Static]
		string [] CommonISOCurrencyCodes { get; }

		[Export ("preferredLanguages", ArgumentSemantic.Copy)][Static]
		string [] PreferredLanguages { get; }

		[Export ("componentsFromLocaleIdentifier:")][Static]
		NSDictionary ComponentsFromLocaleIdentifier (string identifier);

		[Export ("localeIdentifierFromComponents:")][Static]
		string LocaleIdentifierFromComponents (NSDictionary dict);

		[Export ("canonicalLanguageIdentifierFromString:")][Static]
		string CanonicalLanguageIdentifierFromString (string str);

		[Export ("canonicalLocaleIdentifierFromString:")][Static]
		string CanonicalLocaleIdentifierFromString (string str);

		[Export ("characterDirectionForLanguage:")][Static]
		NSLocaleLanguageDirection GetCharacterDirection (string isoLanguageCode);

		[Export ("lineDirectionForLanguage:")][Static]
		NSLocaleLanguageDirection GetLineDirection (string isoLanguageCode);

		[iOS (7,0)] // already in OSX 10.6
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

		[Watch (3, 0), TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Export ("calendarIdentifier")]
		string CalendarIdentifier { get; }

		[Watch (3,0), TV (10,0), Mac (10,12), iOS (10,0)]
		[Export ("localizedStringForCalendarIdentifier:")]
		[return: NullAllowed]
		string GetLocalizedCalendarIdentifier (string calendarIdentifier);
	}

	delegate void NSMatchEnumerator (NSTextCheckingResult result, NSMatchingFlags flags, ref bool stop);

	// This API surfaces NSString instead of strings, because we already have the .NET version that uses
	// strings, so it makes sense to use NSString here (and also, the replacing functionality operates on
	// NSMutableStrings)
	[BaseType (typeof (NSObject))]
	interface NSRegularExpression : NSCopying, NSSecureCoding {
		[DesignatedInitializer]
		[Export ("initWithPattern:options:error:")]
		IntPtr Constructor (NSString pattern, NSRegularExpressionOptions options, out NSError error);

		[Export ("pattern")]
		NSString Pattern { get; }

		[Export ("options")]
		NSRegularExpressionOptions Options { get; }

		[Export ("numberOfCaptureGroups")]
		nuint NumberOfCaptureGroups { get; }

		[Export ("escapedPatternForString:")]
		[Static]
		NSString GetEscapedPattern (NSString str);

		[Export ("enumerateMatchesInString:options:range:usingBlock:")]
		void EnumerateMatches (NSString str, NSMatchingOptions options, NSRange range, NSMatchEnumerator enumerator);

		[Export ("matchesInString:options:range:")]
		NSString [] GetMatches (NSString str, NSMatchingOptions options, NSRange range);

		[Export ("numberOfMatchesInString:options:range:")]
		nuint GetNumberOfMatches (NSString str, NSMatchingOptions options, NSRange range);
		
		[Export ("firstMatchInString:options:range:")]
		NSTextCheckingResult FindFirstMatch (string str, NSMatchingOptions options, NSRange range);
		
		[Export ("rangeOfFirstMatchInString:options:range:")]
		NSRange GetRangeOfFirstMatch (string str, NSMatchingOptions options, NSRange range);
		
		[Export ("stringByReplacingMatchesInString:options:range:withTemplate:")]
		string ReplaceMatches (string sourceString, NSMatchingOptions options, NSRange range, string template);
		
		[Export ("replaceMatchesInString:options:range:withTemplate:")]
		nuint ReplaceMatches (NSMutableString mutableString, NSMatchingOptions options, NSRange range,  NSString template);

		[Export ("replacementStringForResult:inString:offset:template:")]
		NSString GetReplacementString (NSTextCheckingResult result, NSString str, nint offset, NSString template);

		[Static, Export ("escapedTemplateForString:")]
		NSString GetEscapedTemplate (NSString str);
		
	}
	
	[BaseType (typeof (NSObject))]
	// init returns NIL
	[DisableDefaultCtor]
	interface NSRunLoop {
		[Export ("currentRunLoop", ArgumentSemantic.Strong)][Static][IsThreadStatic]
		NSRunLoop Current { get; }

		[Export ("mainRunLoop", ArgumentSemantic.Strong)][Static]
		NSRunLoop Main { get; }

		[Export ("currentMode")]
		NSString CurrentMode { get; }

		[Wrap ("NSRunLoopModeExtensions.GetValue (CurrentMode)")]
		NSRunLoopMode CurrentRunLoopMode { get; }

		[Export ("getCFRunLoop")]
		CFRunLoop GetCFRunLoop ();

		[Export ("addTimer:forMode:")]
		void AddTimer (NSTimer timer, NSString forMode);

		[Wrap ("AddTimer (timer, forMode.GetConstant ())")]
		void AddTimer (NSTimer timer, NSRunLoopMode forMode);

		[Export ("limitDateForMode:")]
		NSDate LimitDateForMode (NSString mode);

		[Wrap ("LimitDateForMode (mode.GetConstant ())")]
		NSDate LimitDateForMode (NSRunLoopMode mode);

		[Export ("acceptInputForMode:beforeDate:")]
		void AcceptInputForMode (NSString mode, NSDate limitDate);

		[Wrap ("AcceptInputForMode (mode.GetConstant (), limitDate)")]
		void AcceptInputForMode (NSRunLoopMode mode, NSDate limitDate);

		[Export ("run")]
		void Run ();

		[Export ("runUntilDate:")]
		void RunUntil (NSDate date);

		[Export ("runMode:beforeDate:")]
		bool RunUntil (NSString runLoopMode, NSDate limitdate);

		[Wrap ("RunUntil (runLoopMode.GetConstant (), limitDate)")]
		bool RunUntil (NSRunLoopMode runLoopMode, NSDate limitDate);

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Export ("performBlock:")]
		void Perform (Action block);

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Export ("performInModes:block:")]
		void Perform (NSString[] modes, Action block);

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Wrap ("Perform (modes.GetConstants (), block)")]
		void Perform (NSRunLoopMode[] modes, Action block);

#if !XAMCORE_4_0
		[Field ("NSDefaultRunLoopMode")]
		NSString NSDefaultRunLoopMode { get; }

		[Field ("NSRunLoopCommonModes")]
		NSString NSRunLoopCommonModes { get; }

		[Availability (Deprecated = Platform.Mac_10_13, Message = "Use 'NSXpcConnection' instead.")]
		[NoiOS, NoWatch, NoTV]
		[Field ("NSConnectionReplyMode")]
		NSString NSRunLoopConnectionReplyMode { get; }

		[NoiOS, NoWatch, NoTV]
		[Field ("NSModalPanelRunLoopMode", "AppKit")]
		NSString NSRunLoopModalPanelMode { get; }

		[NoiOS, NoWatch, NoTV]
		[Field ("NSEventTrackingRunLoopMode", "AppKit")]
		NSString NSRunLoopEventTracking { get; }

		[NoMac][NoWatch]
		[Field ("UITrackingRunLoopMode", "UIKit")]
		NSString UITrackingRunLoopMode { get; }
#endif
	}

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSSet : NSSecureCoding, NSMutableCopying {
		[Export ("set")][Static]
		NSSet CreateSet ();

		[Export ("initWithSet:")]
		IntPtr Constructor (NSSet other);
		
		[Export ("initWithArray:")]
		IntPtr Constructor (NSArray other);
		
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
		bool _Contains (IntPtr id);

		[Export ("containsObject:")]
		bool Contains (NSObject id);

		[Export ("allObjects")][Internal]
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
		IntPtr _SetByAddingObjectsFromSet (IntPtr other);

		[Export ("setByAddingObjectsFromSet:"), Internal]
		NSSet SetByAddingObjectsFromSet (NSSet other);

		[Export ("intersectsSet:")]
		bool IntersectsSet (NSSet other);

		[Internal]
		[Static]
		[Export ("setWithArray:")]
		IntPtr _SetWithArray (IntPtr array);

#if MACCORE
		[Mac (10,11)]
		[Static]
		[Export ("setWithCollectionViewIndexPath:")]
		NSSet FromCollectionViewIndexPath (NSIndexPath indexPath);

		[Mac (10,11)]
		[Static]
		[Export ("setWithCollectionViewIndexPaths:")]
		NSSet FromCollectionViewIndexPaths (NSIndexPath[] indexPaths);

		[Mac (10,11)]
		[Export ("enumerateIndexPathsWithOptions:usingBlock:")]
		void Enumerate (NSEnumerationOptions opts, Action<NSIndexPath, out bool> block);
#endif
	}

	interface NSSet<TKey> : NSSet {}

	[BaseType (typeof (NSObject))]
	interface NSSortDescriptor : NSSecureCoding, NSCopying {
		[Export ("initWithKey:ascending:")]
		IntPtr Constructor (string key, bool ascending);

		[Export ("initWithKey:ascending:selector:")]
		IntPtr Constructor (string key, bool ascending, Selector selector);

		[Export ("initWithKey:ascending:comparator:")]
		IntPtr Constructor (string key, bool ascending, NSComparator comparator);

		[Export ("key")]
		string Key { get; }

		[Export ("ascending")]
		bool Ascending { get; }

		[Export ("selector")]
		Selector Selector { get; }

		[Export ("compareObject:toObject:")]
		NSComparisonResult Compare (NSObject object1, NSObject object2);

		[Export ("reversedSortDescriptor")]
		NSObject ReversedSortDescriptor { get; }

		[iOS (7,0), Mac (10, 9)]
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
	
	[BaseType (typeof(NSObject))]
	[Dispose ("if (disposing) { Invalidate (); } ")]
	// init returns NIL
	[DisableDefaultCtor]
	interface NSTimer {

		[Static, Export ("scheduledTimerWithTimeInterval:target:selector:userInfo:repeats:")]
		NSTimer CreateScheduledTimer (double seconds, NSObject target, Selector selector, [NullAllowed] NSObject userInfo, bool repeats);

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Static]
		[Export ("scheduledTimerWithTimeInterval:repeats:block:")]
		NSTimer CreateScheduledTimer (double interval, bool repeats, Action<NSTimer> block);

		[Static, Export ("timerWithTimeInterval:target:selector:userInfo:repeats:")]
		NSTimer CreateTimer (double seconds, NSObject target, Selector selector, [NullAllowed] NSObject userInfo, bool repeats);

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Static]
		[Export ("timerWithTimeInterval:repeats:block:")]
		NSTimer CreateTimer (double interval, bool repeats, Action<NSTimer> block);

		[DesignatedInitializer]
		[Export ("initWithFireDate:interval:target:selector:userInfo:repeats:")]
		IntPtr Constructor (NSDate date, double seconds, NSObject target, Selector selector, [NullAllowed] NSObject userInfo, bool repeats);

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Export ("initWithFireDate:interval:repeats:block:")]
		IntPtr Constructor (NSDate date, double seconds, bool repeats, Action<NSTimer> block);

		[Export ("fire")]
		void Fire ();

		[NullAllowed] // by default this property is null
		[Export ("fireDate", ArgumentSemantic.Copy)]
		NSDate FireDate { get; set; }

		[Export ("invalidate")]
		void Invalidate ();

		[Export ("isValid")]
		bool IsValid { get; }

		[Export ("timeInterval")]
		double TimeInterval { get; }

		[Export ("userInfo")]
		NSObject UserInfo { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("tolerance")]
		double Tolerance { get; set; }
	}

	[BaseType (typeof(NSObject))]
	// NSTimeZone is an abstract class that defines the behavior of time zone objects. -> http://developer.apple.com/library/ios/#documentation/Cocoa/Reference/Foundation/Classes/NSTimeZone_Class/Reference/Reference.html
	// calling 'init' returns a NIL pointer, i.e. an unusable instance
	[DisableDefaultCtor]
	interface NSTimeZone : NSSecureCoding, NSCopying {
		[Export ("initWithName:")]
		IntPtr Constructor (string name);
		
		[Export ("initWithName:data:")]
		IntPtr Constructor (string name, NSData data);

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
		string[] _KnownTimeZoneNames { get; }

		[Export ("timeZoneDataVersion"), Static]
		string DataVersion { get; }

		[Export ("localizedName:locale:")]
		string GetLocalizedName (NSTimeZoneNameStyle style, NSLocale locale);
	}

	interface NSUbiquitousKeyValueStoreChangeEventArgs {
		[Export ("NSUbiquitousKeyValueStoreChangedKeysKey")]
		string [] ChangedKeys { get; }
	
		[Export ("NSUbiquitousKeyValueStoreChangeReasonKey")]
		NSUbiquitousKeyValueStoreChangeReason ChangeReason { get; }
	}

	[BaseType (typeof (NSObject))]
#if WATCH
	[Advice ("Not available on watchOS")]
	[DisableDefaultCtor] // "NSUbiquitousKeyValueStore is unavailable" is printed to the log.
#endif
	interface NSUbiquitousKeyValueStore {
		[Static]
		[Export ("defaultStore")]
		NSUbiquitousKeyValueStore DefaultStore { get; }

		[Export ("objectForKey:"), Internal]
		NSObject ObjectForKey (string aKey);

		[Export ("setObject:forKey:"), Internal]
		void SetObjectForKey (NSObject anObject, string aKey);

		[Export ("removeObjectForKey:")]
		void Remove (string aKey);

		[Export ("stringForKey:")]
		string GetString (string aKey);

		[Export ("arrayForKey:")]
		NSObject [] GetArray (string aKey);

		[Export ("dictionaryForKey:")]
		NSDictionary GetDictionary (string aKey);

		[Export ("dataForKey:")]
		NSData GetData (string aKey);

		[Export ("longLongForKey:")]
		long GetLong (string aKey);

		[Export ("doubleForKey:")]
		double GetDouble (string aKey);

		[Export ("boolForKey:")]
		bool GetBool (string aKey);

		[Export ("setString:forKey:"), Internal]
		void _SetString (string aString, string aKey);

		[Export ("setData:forKey:"), Internal]
		void _SetData (NSData data, string key);

		[Export ("setArray:forKey:"), Internal]
		void _SetArray (NSObject [] array, string key);

		[Export ("setDictionary:forKey:"), Internal]
		void _SetDictionary (NSDictionary aDictionary, string aKey);

		[Export ("setLongLong:forKey:"), Internal]
		void _SetLong (long value, string aKey);

		[Export ("setDouble:forKey:"), Internal]
		void _SetDouble (double value, string aKey);

		[Export ("setBool:forKey:"), Internal]
		void _SetBool (bool value, string aKey);

		[Export ("dictionaryRepresentation")]
#if XAMCORE_2_0
		NSDictionary ToDictionary ();
#else
		[Obsolete ("Use 'ToDictionary' instead.")]
		NSDictionary DictionaryRepresentation ();
#endif

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

	[iOS (6,0)]
	[BaseType (typeof (NSObject), Name="NSUUID")]
	[DesignatedDefaultCtor]
	interface NSUuid : NSSecureCoding, NSCopying {
		[Export ("initWithUUIDString:")]
		IntPtr Constructor (string str);

		// bound manually to keep the managed/native signatures identical
		//[Export ("initWithUUIDBytes:"), Internal]
		//IntPtr Constructor (IntPtr bytes, bool unused);

		[Export ("getUUIDBytes:"), Internal]
		void GetUuidBytes (IntPtr uuid);

		[Export ("UUIDString")]
		string AsString ();
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true), Watch (2,0), TV (9,0)] // .objc_class_name_NSUserActivity", referenced from '' not found
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // xcode 8 beta 4 marks it as API_DEPRECATED
	partial interface NSUserActivity {
	
		[DesignatedInitializer]
		[Export ("initWithActivityType:")]
#if XAMCORE_2_0
		IntPtr Constructor (string activityType);
#else
		IntPtr Constructor (NSString activityType);
#endif

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
		[Async (ResultTypeName="NSUserActivityContinuation")]
		void GetContinuationStreams (Action<NSInputStream,NSOutputStream,NSError> completionHandler);

		[Mac(10,11), iOS (9,0), Watch (3,0), TV (10,0)]
		[Export ("requiredUserInfoKeys", ArgumentSemantic.Copy)]
		NSSet<NSString> RequiredUserInfoKeys { get; set; }

		[Mac(10,11), iOS (9,0), Watch (3,0), TV (10,0)]
		[Export ("expirationDate", ArgumentSemantic.Copy)]
		NSDate ExpirationDate { get; set; }

		[Mac(10,11), iOS (9,0), Watch (3,0), TV (10,0)]
		[Export ("keywords", ArgumentSemantic.Copy)]
		NSSet<NSString> Keywords { get; set; }

		[Mac(10,11), iOS (9,0), Watch (3,0), TV (10,0)]
		[Export ("resignCurrent")]
		void ResignCurrent ();

		[Mac(10,11), iOS (9,0), Watch (3,0), TV (10,0)]
		[Export ("eligibleForHandoff")]
		bool EligibleForHandoff { [Bind ("isEligibleForHandoff")] get; set; }

		[Mac(10,11), iOS (9,0), Watch (3,0), TV (10,0)]
		[Export ("eligibleForSearch")]
		bool EligibleForSearch { [Bind ("isEligibleForSearch")] get; set; }

		[Mac(10,11), iOS (9,0), Watch (3,0), TV (10,0)]
		[Export ("eligibleForPublicIndexing")]
		bool EligibleForPublicIndexing { [Bind ("isEligibleForPublicIndexing")] get; set; }
		
#if IOS || MONOMAC
		[iOS (9,0)]
		[Mac (10,13, onlyOn64: true)]
		[NullAllowed]
		[Export ("contentAttributeSet", ArgumentSemantic.Copy)] // From CSSearchableItemAttributeSet.h
		CSSearchableItemAttributeSet ContentAttributeSet { get; set; }
#endif

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("referrerURL", ArgumentSemantic.Copy)]
		NSUrl ReferrerUrl { get; set; }

		// From NSUserActivity (CIBarcodeDescriptor)

		[TV (11,3), Mac (10,13,4, onlyOn64: true), iOS (11,3), NoWatch]
		[NullAllowed, Export ("detectedBarcodeDescriptor", ArgumentSemantic.Copy)]
		CIBarcodeDescriptor DetectedBarcodeDescriptor { get; }

		// From NSUserActivity (CLSDeepLinks)

		[NoWatch, NoTV, NoMac, iOS (11,4)]
		[Export ("isClassKitDeepLink")]
		bool IsClassKitDeepLink { get; }

		[NoWatch, NoTV, NoMac, iOS (11,4)]
		[NullAllowed, Export ("contextIdentifierPath", ArgumentSemantic.Strong)]
		string[] ContextIdentifierPath { get; }

		// From NSUserActivity (IntentsAdditions)

		[Watch (5,0), NoTV, NoMac, iOS (12,0)]
		[NullAllowed, Export ("suggestedInvocationPhrase")]
		string SuggestedInvocationPhrase { get; set; }

		[Watch (5, 0), NoTV, NoMac, iOS (12, 0)]
		[Export ("eligibleForPrediction")]
		bool EligibleForPrediction { [Bind ("isEligibleForPrediction")] get; set; }

		[Watch (5, 0), NoTV, NoMac, iOS (12, 0)]
		[NullAllowed, Export ("persistentIdentifier")]
		string PersistentIdentifier { get; set; }

		[Watch (5,0), NoTV, NoMac, iOS (12,0)]
		[Static]
		[Async]
		[Export ("deleteSavedUserActivitiesWithPersistentIdentifiers:completionHandler:")]
		void DeleteSavedUserActivities (string[] persistentIdentifiers, Action handler);

		[Watch (5,0), NoTV, NoMac, iOS (12,0)]
		[Static]
		[Async]
		[Export ("deleteAllSavedUserActivitiesWithCompletionHandler:")]
		void DeleteAllSavedUserActivities (Action handler);
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true)] // same as NSUserActivity
	[Static]
	partial interface NSUserActivityType {
		[Field ("NSUserActivityTypeBrowsingWeb")]
		NSString BrowsingWeb { get; }
	}

	[iOS (8,0)][Mac (10,10, onlyOn64 : true), Watch (3,0), TV (9,0)] // same as NSUserActivity
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
		[iOS (7,0), Mac (10, 9)]
		[Export ("initWithSuiteName:")]
		IntPtr InitWithSuiteName (string suiteName);

		[Export ("objectForKey:")][Internal]
		NSObject ObjectForKey (string defaultName);
	
		[Export ("setObject:forKey:")][Internal]
		void SetObjectForKey (NSObject value, string defaultName);
	
		[Export ("removeObjectForKey:")]
		void RemoveObject (string defaultName);
	
		[Export ("stringForKey:")]
		string StringForKey (string defaultName);
	
		[Export ("arrayForKey:")]
		NSObject [] ArrayForKey (string defaultName);
	
		[Export ("dictionaryForKey:")]
		NSDictionary DictionaryForKey (string defaultName);
	
		[Export ("dataForKey:")]
		NSData DataForKey (string defaultName);
	
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
		void SetBool (bool value, string  defaultName);
	
		[Export ("registerDefaults:")]
		void RegisterDefaults (NSDictionary registrationDictionary);
	
		[Export ("addSuiteNamed:")]
		void AddSuite (string suiteName);
	
		[Export ("removeSuiteNamed:")]
		void RemoveSuite (string suiteName);
	
		[Export ("dictionaryRepresentation")]
#if XAMCORE_2_0
		NSDictionary ToDictionary ();
#else
		[Obsolete ("Use 'ToDictionary' instead.")]
		NSDictionary AsDictionary ();
#endif
	
		[Export ("volatileDomainNames")]
		string [] VolatileDomainNames ();
	
		[Export ("volatileDomainForName:")]
		NSDictionary GetVolatileDomain (string domainName);
	
		[Export ("setVolatileDomain:forName:")]
		void SetVolatileDomain (NSDictionary domain, string domainName);
	
		[Export ("removeVolatileDomainForName:")]
		void RemoveVolatileDomain (string domainName);
	
		[Deprecated (PlatformName.iOS, 7, 0)]
		[Deprecated (PlatformName.MacOSX, 10, 9)]
		[Export ("persistentDomainNames")]
		string [] PersistentDomainNames ();
	
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

		[iOS (9,3)]
		[NoMac][NoTV]
		[Notification]
		[Field ("NSUserDefaultsSizeLimitExceededNotification")]
		NSString SizeLimitExceededNotification { get; }

		[iOS (10,0)][TV (10,0)][Watch (3,0)][NoMac]
		[Notification]
		[Field ("NSUbiquitousUserDefaultsNoCloudAccountNotification")]
		NSString NoCloudAccountNotification { get; }

		[iOS (9,3)]
		[NoMac][NoTV]
		[Notification]
		[Field ("NSUbiquitousUserDefaultsDidChangeAccountsNotification")]
		NSString DidChangeAccountsNotification { get; }

		[iOS (9,3)]
		[NoMac][NoTV]
		[Notification]
		[Field ("NSUbiquitousUserDefaultsCompletedInitialSyncNotification")]
		NSString CompletedInitialSyncNotification { get; }

		[Notification]
		[Field ("NSUserDefaultsDidChangeNotification")]
		NSString DidChangeNotification { get; }
	}
	
	[BaseType (typeof (NSObject), Name="NSURL")]
	// init returns NIL
	[DisableDefaultCtor]
	partial interface NSUrl : NSSecureCoding, NSCopying
#if MONOMAC
	, NSPasteboardReading, NSPasteboardWriting
#endif
#if !(MONOMAC && !XAMCORE_2_0) // exclude Classic/XM
	, NSItemProviderWriting, NSItemProviderReading
#endif
#if IOS || MONOMAC
	, QLPreviewItem
#endif
	{
		[Export ("initWithScheme:host:path:")]
		IntPtr Constructor (string scheme, string host, string path);

		[DesignatedInitializer]
		[Export ("initFileURLWithPath:isDirectory:")]
		IntPtr Constructor (string path, bool isDir);

		[Export ("initWithString:")]
		IntPtr Constructor (string urlString);

		[DesignatedInitializer]
		[Export ("initWithString:relativeToURL:")]
		IntPtr Constructor (string urlString, NSUrl relativeToUrl);

		[Export ("URLWithString:")][Static]
		NSUrl FromString (string s);

		[Export ("URLWithString:relativeToURL:")][Internal][Static]
		NSUrl _FromStringRelative (string url, NSUrl relative);
		
		[Export ("absoluteString")]
		string AbsoluteString { get; }

		[Export ("absoluteURL")]
		NSUrl AbsoluteUrl { get; }

		[Export ("baseURL")]
		NSUrl BaseUrl { get; }

		[Export ("fragment")]
		string Fragment { get; }

		[Export ("host")]
		string Host { get; }

#if XAMCORE_2_0
		[Internal]
#endif
		[Export ("isEqual:")]
		bool IsEqual ([NullAllowed] NSUrl other);

		[Export ("isFileURL")]
		bool IsFileUrl { get; }

		[Export ("parameterString")]
		string ParameterString { get;}

		[Export ("password")]
		string Password { get;}

		[Export ("path")]
		string Path { get;}

		[Export ("query")]
		string Query { get;}

		[Export ("relativePath")]
		string RelativePath { get;}

		[Export ("pathComponents")]
		string [] PathComponents { get; }

		[Export ("lastPathComponent")]
		string LastPathComponent { get; }

		[Export ("pathExtension")]
		string PathExtension { get; }

		[Export ("relativeString")]
		string RelativeString { get;}

		[Export ("resourceSpecifier")]
		string ResourceSpecifier { get;}

		[Export ("scheme")]
		string Scheme { get;}

		[Export ("user")]
		string User { get;}

		[Export ("standardizedURL")]
		NSUrl StandardizedUrl { get; }

		[Export ("URLByAppendingPathComponent:isDirectory:")]
		NSUrl Append (string pathComponent, bool isDirectory);

		[Export ("URLByAppendingPathExtension:")]
		NSUrl AppendPathExtension (string extension);

		[Export ("URLByDeletingLastPathComponent")]
		NSUrl RemoveLastPathComponent ();

		[Export ("URLByDeletingPathExtension")]
		NSUrl RemovePathExtension ();

		[iOS (7,0), Mac (10, 9)]
		[Export ("getFileSystemRepresentation:maxLength:")]
		bool GetFileSystemRepresentation (IntPtr buffer, nint maxBufferLength);

		[iOS (7,0), Mac (10, 9)]
		[Export ("fileSystemRepresentation")]
		IntPtr GetFileSystemRepresentationAsUtf8Ptr { get; }

		[iOS (7,0), Mac (10, 9)]
		[Export ("removeCachedResourceValueForKey:")]
		void RemoveCachedResourceValueForKey (NSString key);

		[iOS (7,0), Mac (10, 9)]
		[Export ("removeAllCachedResourceValues")]
		void RemoveAllCachedResourceValues ();

		[iOS (7,0), Mac (10, 9)]
		[Export ("setTemporaryResourceValue:forKey:")]
		void SetTemporaryResourceValue (NSObject value, NSString key);

		[DesignatedInitializer]
		[iOS (7,0), Mac (10, 9)]
		[Export ("initFileURLWithFileSystemRepresentation:isDirectory:relativeToURL:")]
		IntPtr Constructor (IntPtr ptrUtf8path, bool isDir, NSUrl baseURL);

		[iOS (7,0), Mac (10, 9), Static, Export ("fileURLWithFileSystemRepresentation:isDirectory:relativeToURL:")]
		NSUrl FromUTF8Pointer (IntPtr ptrUtf8path, bool isDir, NSUrl baseURL);

#if MONOMAC

		/* These methods come from NURL_AppKitAdditions */

		[Export ("URLFromPasteboard:")]
		[Static]
		NSUrl FromPasteboard (NSPasteboard pasteboard);

		[Export ("writeToPasteboard:")]
		void WriteToPasteboard (NSPasteboard pasteboard);
#endif
		[Export("bookmarkDataWithContentsOfURL:error:")]
		[Static]
		NSData GetBookmarkData (NSUrl bookmarkFileUrl, out NSError error);

		[Export("URLByResolvingBookmarkData:options:relativeToURL:bookmarkDataIsStale:error:")]
		[Static]
		NSUrl FromBookmarkData (NSData data, NSUrlBookmarkResolutionOptions options, [NullAllowed] NSUrl relativeToUrl, out bool isStale, out NSError error);

		[Export("writeBookmarkData:toURL:options:error:")]
		[Static]
		bool WriteBookmarkData (NSData data, NSUrl bookmarkFileUrl, NSUrlBookmarkCreationOptions options, out NSError error);

		[Export("filePathURL")]
		NSUrl FilePathUrl { get; }

		[Export("fileReferenceURL")]
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
		NSString IsAliasFileKey	{ get; }

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

		[Watch (3, 0), TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLVolumeIsEncryptedKey")]
		NSString VolumeIsEncryptedKey { get; }

		[Watch (3, 0), TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLVolumeIsRootFileSystemKey")]
		NSString VolumeIsRootFileSystemKey { get; }

		[Watch (3, 0), TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLVolumeSupportsCompressionKey")]
		NSString VolumeSupportsCompressionKey { get; }

		[Watch (3, 0), TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLVolumeSupportsFileCloningKey")]
		NSString VolumeSupportsFileCloningKey { get; }

		[Watch (3, 0), TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLVolumeSupportsSwapRenamingKey")]
		NSString VolumeSupportsSwapRenamingKey { get; }

		[Watch (3, 0), TV (10, 0), Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLVolumeSupportsExclusiveRenamingKey")]
		NSString VolumeSupportsExclusiveRenamingKey { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Field ("NSURLVolumeSupportsImmutableFilesKey")]
		NSString VolumeSupportsImmutableFilesKey { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Field ("NSURLVolumeSupportsAccessPermissionsKey")]
		NSString VolumeSupportsAccessPermissionsKey { get; }

		[NoWatch, NoTV, Mac (10, 13), iOS (11, 0)]
		[Field ("NSURLVolumeAvailableCapacityForImportantUsageKey")]
		NSString VolumeAvailableCapacityForImportantUsageKey { get; }

		[NoWatch, NoTV, Mac (10, 13), iOS (11, 0)]
		[Field ("NSURLVolumeAvailableCapacityForOpportunisticUsageKey")]
		NSString VolumeAvailableCapacityForOpportunisticUsageKey { get; }

		[Field ("NSURLIsUbiquitousItemKey")]
		NSString IsUbiquitousItemKey { get; }

		[Field ("NSURLUbiquitousItemHasUnresolvedConflictsKey")]
		NSString UbiquitousItemHasUnresolvedConflictsKey { get; }

		[Field ("NSURLUbiquitousItemIsDownloadedKey")]
		NSString UbiquitousItemIsDownloadedKey { get; }

		[Field ("NSURLUbiquitousItemIsDownloadingKey")]
		[Availability (Deprecated = Platform.iOS_7_0)]
		NSString UbiquitousItemIsDownloadingKey { get; }

		[Field ("NSURLUbiquitousItemIsUploadedKey")]
		NSString UbiquitousItemIsUploadedKey { get; }

		[Field ("NSURLUbiquitousItemIsUploadingKey")]
		NSString UbiquitousItemIsUploadingKey { get; }

		[Field ("NSURLUbiquitousItemPercentDownloadedKey")]
		[Deprecated (PlatformName.iOS, 6, 0, message : "Use 'NSMetadataQuery.UbiquitousItemPercentDownloadedKey' on 'NSMetadataItem' instead.")]
		[Mac (10, 7)]
		[Deprecated (PlatformName.MacOSX, 10, 8, message : "Use 'NSMetadataQuery.UbiquitousItemPercentDownloadedKey' on 'NSMetadataItem' instead.")]
		NSString UbiquitousItemPercentDownloadedKey { get; }

		[Deprecated (PlatformName.iOS, 6, 0, message : "Use 'NSMetadataQuery.UbiquitousItemPercentUploadedKey' on 'NSMetadataItem' instead.")]
		[Mac (10, 7)]
		[Deprecated (PlatformName.MacOSX, 10, 8, message : "Use 'NSMetadataQuery.UbiquitousItemPercentUploadedKey' on 'NSMetadataItem' instead.")]
		[Field ("NSURLUbiquitousItemPercentUploadedKey")]
		NSString UbiquitousItemPercentUploadedKey { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLUbiquitousItemIsSharedKey")]
		NSString UbiquitousItemIsSharedKey { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLUbiquitousSharedItemCurrentUserRoleKey")]
		NSString UbiquitousSharedItemCurrentUserRoleKey { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLUbiquitousSharedItemCurrentUserPermissionsKey")]
		NSString UbiquitousSharedItemCurrentUserPermissionsKey { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLUbiquitousSharedItemOwnerNameComponentsKey")]
		NSString UbiquitousSharedItemOwnerNameComponentsKey { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLUbiquitousSharedItemMostRecentEditorNameComponentsKey")]
		NSString UbiquitousSharedItemMostRecentEditorNameComponentsKey { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLUbiquitousSharedItemRoleOwner")]
		NSString UbiquitousSharedItemRoleOwner { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLUbiquitousSharedItemRoleParticipant")]
		NSString UbiquitousSharedItemRoleParticipant { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLUbiquitousSharedItemPermissionsReadOnly")]
		NSString UbiquitousSharedItemPermissionsReadOnly { get; }

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[Field ("NSURLUbiquitousSharedItemPermissionsReadWrite")]
		NSString UbiquitousSharedItemPermissionsReadWrite { get; }

		[Mac (10, 8)]
		[Field ("NSURLIsExcludedFromBackupKey")]
		NSString IsExcludedFromBackupKey { get; }

		[Export ("bookmarkDataWithOptions:includingResourceValuesForKeys:relativeToURL:error:")]
		NSData CreateBookmarkData (NSUrlBookmarkCreationOptions options, string [] resourceValues, [NullAllowed] NSUrl relativeUrl, out NSError error);

		[Export ("initByResolvingBookmarkData:options:relativeToURL:bookmarkDataIsStale:error:")]
		IntPtr Constructor (NSData bookmarkData, NSUrlBookmarkResolutionOptions resolutionOptions, [NullAllowed] NSUrl relativeUrl, out bool bookmarkIsStale, out NSError error);

		[Field ("NSURLPathKey")]
		[iOS (6,0)][Mac (10, 8)]
		NSString PathKey { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("NSURLUbiquitousItemDownloadingStatusKey")]
		NSString UbiquitousItemDownloadingStatusKey { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("NSURLUbiquitousItemDownloadingErrorKey")]
		NSString UbiquitousItemDownloadingErrorKey { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("NSURLUbiquitousItemUploadingErrorKey")]
		NSString UbiquitousItemUploadingErrorKey { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("NSURLUbiquitousItemDownloadingStatusNotDownloaded")]
		NSString UbiquitousItemDownloadingStatusNotDownloaded { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("NSURLUbiquitousItemDownloadingStatusDownloaded")]
		NSString UbiquitousItemDownloadingStatusDownloaded { get; }

		[iOS (7,0), Mac (10, 9)]
		[Field ("NSURLUbiquitousItemDownloadingStatusCurrent")]
		NSString UbiquitousItemDownloadingStatusCurrent { get; }

		[Mac (10,7), iOS (8,0)]
		[Export ("startAccessingSecurityScopedResource")]
		bool StartAccessingSecurityScopedResource ();

		[Mac (10,7), iOS (8,0)]
		[Export ("stopAccessingSecurityScopedResource")]
		void StopAccessingSecurityScopedResource ();

		[Mac (10,10), iOS (8,0)]
		[Static, Export ("URLByResolvingAliasFileAtURL:options:error:")]
		NSUrl ResolveAlias  (NSUrl aliasFileUrl, NSUrlBookmarkResolutionOptions options, out NSError error);

		[Static, Export ("fileURLWithPathComponents:")]
		NSUrl CreateFileUrl (string [] pathComponents);

		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLAddedToDirectoryDateKey")]
		NSString AddedToDirectoryDateKey { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLDocumentIdentifierKey")]
		NSString DocumentIdentifierKey { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLGenerationIdentifierKey")]
		NSString GenerationIdentifierKey { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLThumbnailDictionaryKey")]
		NSString ThumbnailDictionaryKey { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLUbiquitousItemContainerDisplayNameKey")]
		NSString UbiquitousItemContainerDisplayNameKey { get; }
		
		[Mac (10,10), iOS (8,0)]
		[Field ("NSURLUbiquitousItemDownloadRequestedKey")]
		NSString UbiquitousItemDownloadRequestedKey { get; }

		//
		// iOS 9.0/osx 10.11 additions
		//
		[DesignatedInitializer]
		[iOS (9,0), Mac(10,11)]
		[Export ("initFileURLWithPath:isDirectory:relativeToURL:")]
		IntPtr Constructor (string path, bool isDir, [NullAllowed] NSUrl relativeToUrl);

		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("fileURLWithPath:isDirectory:relativeToURL:")]
		NSUrl CreateFileUrl (string path, bool isDir, [NullAllowed] NSUrl relativeToUrl);

		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("fileURLWithPath:relativeToURL:")]
		NSUrl CreateFileUrl (string path, [NullAllowed] NSUrl relativeToUrl);

		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("URLWithDataRepresentation:relativeToURL:")]
		NSUrl CreateWithDataRepresentation (NSData data, [NullAllowed] NSUrl relativeToUrl);

		[iOS (9,0), Mac(10,11)]
		[Static]
		[Export ("absoluteURLWithDataRepresentation:relativeToURL:")]
		NSUrl CreateAbsoluteUrlWithDataRepresentation (NSData data, [NullAllowed] NSUrl relativeToUrl);

		[iOS (9,0), Mac(10,11)]
		[Export ("dataRepresentation", ArgumentSemantic.Copy)]
		NSData DataRepresentation { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("hasDirectoryPath")]
		bool HasDirectoryPath { get; }

		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLIsApplicationKey")]
		NSString IsApplicationKey { get; }

#if !MONOMAC
		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLFileProtectionKey")]
		NSString FileProtectionKey { get; }

		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLFileProtectionNone")]
		NSString FileProtectionNone { get; }
		
		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLFileProtectionComplete")]
		NSString FileProtectionComplete { get; }
		
		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLFileProtectionCompleteUnlessOpen")]
		NSString FileProtectionCompleteUnlessOpen { get; }

		[iOS (9,0), Mac(10,11)]
		[Field ("NSURLFileProtectionCompleteUntilFirstUserAuthentication")]
		NSString FileProtectionCompleteUntilFirstUserAuthentication { get; }
#endif

		// From the NSItemProviderReading protocol
		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Static]
		[Export ("readableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if XAMCORE_2_0 || !MONOMAC
		new
#endif
		string[] ReadableTypeIdentifiers { get; }

		// From the NSItemProviderReading protocol
		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Static]
		[Export ("objectWithItemProviderData:typeIdentifier:error:")]
		[return: NullAllowed]
#if XAMCORE_2_0 || !MONOMAC
		new
#endif
		NSUrl GetObject (NSData data, string typeIdentifier, [NullAllowed] out NSError outError);

		// From the NSItemProviderWriting protocol
		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Static]
		[Export ("writableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if XAMCORE_2_0 || !MONOMAC
		new
#endif
		string[] WritableTypeIdentifiers { get; }
	}

	
	//
	// Just a category so we can document the three methods together
	//
	[Category, BaseType (typeof (NSUrl))]
	partial interface NSUrl_PromisedItems {
		[Mac (10,10), iOS (8,0)]
		[Export ("checkPromisedItemIsReachableAndReturnError:")]
		bool CheckPromisedItemIsReachable (out NSError error);

		[Mac (10,10), iOS (8,0)]
		[Export ("getPromisedItemResourceValue:forKey:error:")]
		bool GetPromisedItemResourceValue (out NSObject value, NSString key, out NSError error);

		[Mac (10,10), iOS (8,0)]
		[Export ("promisedItemResourceValuesForKeys:error:")]
		NSDictionary GetPromisedItemResourceValues (NSString [] keys, out NSError error);
		
	}

	[iOS (8,0), Mac (10,10)]
	[BaseType (typeof (NSObject), Name="NSURLQueryItem")]
	interface NSUrlQueryItem : NSSecureCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithName:value:")]
		IntPtr Constructor (string name, string value);

		[Export ("name")]
		string Name { get; }

		[Export ("value")]
		string Value { get; }
	}

	[Category, BaseType (typeof (NSCharacterSet))]
	partial interface NSUrlUtilities_NSCharacterSet {
		[iOS (7,0), Static, Export ("URLUserAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlUserAllowedCharacterSet { get; }
	
		[iOS (7,0), Static, Export ("URLPasswordAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlPasswordAllowedCharacterSet { get; }
	
		[iOS (7,0), Static, Export ("URLHostAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlHostAllowedCharacterSet { get; }
	
		[iOS (7,0), Static, Export ("URLPathAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlPathAllowedCharacterSet { get; }
	
		[iOS (7,0), Static, Export ("URLQueryAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlQueryAllowedCharacterSet { get; }
	
		[iOS (7,0), Static, Export ("URLFragmentAllowedCharacterSet", ArgumentSemantic.Copy)]
		NSCharacterSet UrlFragmentAllowedCharacterSet { get; }
	}
		
	[BaseType (typeof (NSObject), Name="NSURLCache")]
	interface NSUrlCache {
		[Export ("sharedURLCache", ArgumentSemantic.Strong), Static]
		NSUrlCache SharedCache { get; set; }

		[Export ("initWithMemoryCapacity:diskCapacity:diskPath:")]
		IntPtr Constructor (nuint memoryCapacity, nuint diskCapacity, string diskPath);

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

		[Mac(10,10)][iOS(8,0)]
		[Export ("removeCachedResponsesSinceDate:")]
		void RemoveCachedResponsesSinceDate (NSDate date);

		[iOS (8,0), Mac(10,10)]
		[Export ("storeCachedResponse:forDataTask:")]
		void StoreCachedResponse (NSCachedUrlResponse cachedResponse, NSUrlSessionDataTask dataTask);

		[iOS (8,0), Mac(10,10)]
		[Export ("getCachedResponseForDataTask:completionHandler:")]
		[Async]
		void GetCachedResponse (NSUrlSessionDataTask dataTask, Action<NSCachedUrlResponse> completionHandler);

		[iOS (8,0), Mac(10,10)]
		[Export ("removeCachedResponseForDataTask:")]
		void RemoveCachedResponse (NSUrlSessionDataTask dataTask);
	}
	
	[iOS (7,0), Mac (10, 9)]
	[BaseType (typeof (NSObject), Name="NSURLComponents")]
	partial interface NSUrlComponents : NSCopying {
		[Export ("initWithURL:resolvingAgainstBaseURL:")]
		IntPtr Constructor (NSUrl url, bool resolveAgainstBaseUrl);
	
		[Static, Export ("componentsWithURL:resolvingAgainstBaseURL:")]
		NSUrlComponents FromUrl (NSUrl url, bool resolvingAgainstBaseUrl);
	
		[Export ("initWithString:")]
		IntPtr Constructor (string urlString);
	
		[Static, Export ("componentsWithString:")]
		NSUrlComponents FromString (string urlString);
	
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
		[Export ("percentEncodedHost", ArgumentSemantic.Copy)]
		string PercentEncodedHost { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("percentEncodedPath", ArgumentSemantic.Copy)]
		string PercentEncodedPath { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("percentEncodedQuery", ArgumentSemantic.Copy)]
		string PercentEncodedQuery { get; set; }
	
		[NullAllowed] // by default this property is null
		[Export ("percentEncodedFragment", ArgumentSemantic.Copy)]
		string PercentEncodedFragment { get; set; }

		[Mac (10,10), iOS (8,0)]
		[NullAllowed] // by default this property is null
		[Export ("queryItems")]
		NSUrlQueryItem [] QueryItems { get; set; }

		[Mac (10,10), iOS (8,0)]
		[Export ("string")]
		string AsString ();

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfScheme"), Mac(10,11)]
		NSRange RangeOfScheme { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfUser"), Mac(10,11)]
		NSRange RangeOfUser { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfPassword"), Mac(10,11)]
		NSRange RangeOfPassword { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfHost"), Mac(10,11)]
		NSRange RangeOfHost { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfPort"), Mac(10,11)]
		NSRange RangeOfPort { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfPath"), Mac(10,11)]
		NSRange RangeOfPath { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfQuery"), Mac(10,11)]
		NSRange RangeOfQuery { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("rangeOfFragment"), Mac(10,11)]
		NSRange RangeOfFragment { get; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("percentEncodedQueryItems", ArgumentSemantic.Copy)]
		NSUrlQueryItem[] PercentEncodedQueryItems { get; set; }
	}
	
	[BaseType (typeof (NSObject), Name="NSURLAuthenticationChallenge")]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface NSUrlAuthenticationChallenge : NSSecureCoding {
		[Export ("initWithProtectionSpace:proposedCredential:previousFailureCount:failureResponse:error:sender:")]
		IntPtr Constructor (NSUrlProtectionSpace space, NSUrlCredential credential, nint previousFailureCount, NSUrlResponse response, [NullAllowed] NSError error, NSUrlConnection sender);
		
		[Export ("initWithAuthenticationChallenge:sender:")]
		IntPtr Constructor (NSUrlAuthenticationChallenge  challenge, NSUrlConnection sender);
	
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
#if XAMCORE_4_0
	interface NSUrlAuthenticationChallengeSender {
#else
	[Model]
	[BaseType (typeof (NSObject))]
	interface NSURLAuthenticationChallengeSender {
#endif
		[Abstract]
		[Export ("useCredential:forAuthenticationChallenge:")]
#if XAMCORE_2_0
		void UseCredential (NSUrlCredential credential, NSUrlAuthenticationChallenge challenge);
#else
		void UseCredentials (NSUrlCredential credential, NSUrlAuthenticationChallenge challenge);
#endif

		[Abstract]
		[Export ("continueWithoutCredentialForAuthenticationChallenge:")]
#if XAMCORE_2_0
		void ContinueWithoutCredential (NSUrlAuthenticationChallenge challenge);
#else
		void ContinueWithoutCredentialForAuthenticationChallenge (NSUrlAuthenticationChallenge challenge);
#endif

		[Abstract]
		[Export ("cancelAuthenticationChallenge:")]
		void CancelAuthenticationChallenge (NSUrlAuthenticationChallenge challenge);

		[Export ("performDefaultHandlingForAuthenticationChallenge:")]
#if XAMCORE_2_0
		void PerformDefaultHandling (NSUrlAuthenticationChallenge challenge);
#else
		[Abstract]
		void PerformDefaultHandlingForChallenge (NSUrlAuthenticationChallenge challenge);
#endif

		[Export ("rejectProtectionSpaceAndContinueWithChallenge:")]
#if XAMCORE_2_0
		void RejectProtectionSpaceAndContinue (NSUrlAuthenticationChallenge challenge);
#else
		[Abstract]
		void RejectProtectionSpaceAndContinueWithChallenge (NSUrlAuthenticationChallenge challenge);
#endif
	}


	delegate void NSUrlConnectionDataResponse (NSUrlResponse response, NSData data, NSError error);
	
	[BaseType (typeof (NSObject), Name="NSURLConnection")]
	interface NSUrlConnection : 
#if XAMCORE_4_0
		NSUrlAuthenticationChallengeSender
#else
		NSURLAuthenticationChallengeSender
#endif
	{
		[Export ("canHandleRequest:")][Static]
		bool CanHandleRequest (NSUrlRequest request);
	
		[NoWatch]
		[Deprecated (PlatformName.iOS, 9,0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,11, message: "Use 'NSUrlSession' instead.")]
		[Export ("connectionWithRequest:delegate:")][Static]
		NSUrlConnection FromRequest (NSUrlRequest request, [Protocolize] NSUrlConnectionDelegate connectionDelegate);
	
		[Deprecated (PlatformName.iOS, 9,0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,11, message: "Use 'NSUrlSession' instead.")]
		[Export ("initWithRequest:delegate:")]
		IntPtr Constructor (NSUrlRequest request, [Protocolize] NSUrlConnectionDelegate connectionDelegate);
	
		[Deprecated (PlatformName.iOS, 9,0, message: "Use 'NSUrlSession' instead.")]
		[Deprecated (PlatformName.MacOSX, 10,11, message: "Use 'NSUrlSession' instead.")]
		[Export ("initWithRequest:delegate:startImmediately:")]
		IntPtr Constructor (NSUrlRequest request, [Protocolize] NSUrlConnectionDelegate connectionDelegate, bool startImmediately);
	
		[Export ("start")]
		void Start ();
	
		[Export ("cancel")]
		void Cancel ();
	
		[Export ("scheduleInRunLoop:forMode:")]
		void Schedule (NSRunLoop aRunLoop, NSString forMode);

		[Wrap ("Schedule (aRunLoop, forMode.GetConstant ())")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);
	
		[Export ("unscheduleFromRunLoop:forMode:")]
		void Unschedule (NSRunLoop aRunLoop, NSString forMode);

		[Wrap ("Unschedule (aRunLoop, forMode.GetConstant ())")]
		void Unschedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

#if !MONOMAC
		[Export ("originalRequest")]
		NSUrlRequest OriginalRequest { get; }

		[Export ("currentRequest")]
		NSUrlRequest CurrentRequest { get; }
#endif
		[Export ("setDelegateQueue:")]
		void SetDelegateQueue (NSOperationQueue queue);

		[NoWatch]
		[Static]
		[Export ("sendAsynchronousRequest:queue:completionHandler:")]
		[Async (ResultTypeName = "NSUrlAsyncResult", MethodName="SendRequestAsync")]
		void SendAsynchronousRequest (NSUrlRequest request, NSOperationQueue queue, NSUrlConnectionDataResponse completionHandler);
		
#if IOS
		// Extension from iOS5, NewsstandKit
		[Export ("newsstandAssetDownload", ArgumentSemantic.Weak)]
		NewsstandKit.NKAssetDownload NewsstandAssetDownload { get; }
#endif
	}

	[BaseType (typeof (NSObject), Name="NSURLConnectionDelegate")]
	[Model]
	[Protocol]
	interface NSUrlConnectionDelegate {
#if !XAMCORE_2_0
		// part of NSURLConnectionDataDelegate
		[Export ("connection:willSendRequest:redirectResponse:")]
		NSUrlRequest WillSendRequest (NSUrlConnection connection, NSUrlRequest request, NSUrlResponse response);

		[Export ("connection:needNewBodyStream:")]
		NSInputStream NeedNewBodyStream (NSUrlConnection connection, NSUrlRequest request);
#endif

		[Export ("connection:canAuthenticateAgainstProtectionSpace:")]
		[Availability (Deprecated=Platform.iOS_8_0|Platform.Mac_10_10, Message="Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		bool CanAuthenticateAgainstProtectionSpace (NSUrlConnection connection, NSUrlProtectionSpace protectionSpace);

		[Export ("connection:didReceiveAuthenticationChallenge:")]
		[Availability (Deprecated=Platform.iOS_8_0|Platform.Mac_10_10, Message="Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		void ReceivedAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);

		[Export ("connection:didCancelAuthenticationChallenge:")]
		[Availability (Deprecated=Platform.iOS_8_0|Platform.Mac_10_10, Message="Use 'WillSendRequestForAuthenticationChallenge' instead.")]
		void CanceledAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);

		[Export ("connectionShouldUseCredentialStorage:")]
		bool ConnectionShouldUseCredentialStorage (NSUrlConnection connection);

#if !XAMCORE_2_0
		// part of NSURLConnectionDataDelegate
		[Export ("connection:didReceiveResponse:")]
		void ReceivedResponse (NSUrlConnection connection, NSUrlResponse response);

		[Export ("connection:didReceiveData:")]
		void ReceivedData (NSUrlConnection connection, NSData data);

		[Export ("connection:didSendBodyData:totalBytesWritten:totalBytesExpectedToWrite:")]
		void SentBodyData (NSUrlConnection connection, nint bytesWritten, nint totalBytesWritten, nint totalBytesExpectedToWrite);

		[Export ("connection:willCacheResponse:")]
		NSCachedUrlResponse WillCacheResponse (NSUrlConnection connection, NSCachedUrlResponse cachedResponse);

		[Export ("connectionDidFinishLoading:")]
		void FinishedLoading (NSUrlConnection connection);
#endif

		[Export ("connection:didFailWithError:")]
		void FailedWithError (NSUrlConnection connection, NSError error);

		[Export ("connection:willSendRequestForAuthenticationChallenge:")]
		void WillSendRequestForAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge);
	}

#if XAMCORE_2_0
	[BaseType (typeof (NSUrlConnectionDelegate), Name="NSURLConnectionDataDelegate")]
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
#endif

	[BaseType (typeof (NSUrlConnectionDelegate), Name="NSURLConnectionDownloadDelegate")]
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
		
	[BaseType (typeof (NSObject), Name="NSURLCredential")]
	// crash when calling NSObjecg.get_Description (and likely other selectors)
	[DisableDefaultCtor]
	interface NSUrlCredential : NSSecureCoding, NSCopying {

		[Export ("initWithTrust:")]
		IntPtr Constructor (SecTrust trust);

		[Export ("persistence")]
		NSUrlCredentialPersistence Persistence { get; }

		[Export ("initWithUser:password:persistence:")]
		IntPtr Constructor (string user, string password, NSUrlCredentialPersistence persistence);
	
		[Static]
		[Export ("credentialWithUser:password:persistence:")]
		NSUrlCredential FromUserPasswordPersistance (string user, string password, NSUrlCredentialPersistence persistence);

		[Export ("user")]
		string User { get; }
	
		[Export ("password")]
		string Password { get; }
	
		[Export ("hasPassword")]
		bool HasPassword {get; }
	
		[Export ("initWithIdentity:certificates:persistence:")]
		[Internal]
		IntPtr Constructor (IntPtr identity, IntPtr certificates, NSUrlCredentialPersistence persistance);
	
		[Static]
		[Internal]
		[Export ("credentialWithIdentity:certificates:persistence:")]
		NSUrlCredential FromIdentityCertificatesPersistanceInternal (IntPtr identity, IntPtr certificates, NSUrlCredentialPersistence persistence);
	
#if XAMCORE_2_0
		[Internal]
#else
		[Obsolete ("Use SecIdentity property")]
#endif
		[Export ("identity")]
		IntPtr Identity { get; }
	
		[Export ("certificates")]
		SecCertificate [] Certificates { get; }
	
		// bound manually to keep the managed/native signatures identical
		//[Export ("initWithTrust:")]
		//IntPtr Constructor (IntPtr SecTrustRef_trust, bool ignored);
	
#if XAMCORE_2_0
		[Internal]
#else
		[Obsolete ("Use 'NSUrlCredential(SecTrust)' constructor.")]
#endif
		[Static]
		[Export ("credentialForTrust:")]
		NSUrlCredential FromTrust (IntPtr trust);
	}

	[BaseType (typeof (NSObject), Name="NSURLCredentialStorage")]
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

		[iOS (7,0), Mac (10, 9)]
		[Export ("removeCredential:forProtectionSpace:options:")]
		void RemoveCredential (NSUrlCredential credential, NSUrlProtectionSpace forProtectionSpace, NSDictionary options);

		[iOS (7,0), Mac (10, 9)]
		[Field ("NSURLCredentialStorageRemoveSynchronizableCredentials")]
		NSString RemoveSynchronizableCredentials { get; }

		[Field ("NSURLCredentialStorageChangedNotification")]
		[Notification]
		NSString ChangedNotification { get; }

		[iOS (8,0), Mac (10,10)]
		[Async]
		[Export ("getCredentialsForProtectionSpace:task:completionHandler:")]
		void GetCredentials (NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task, [NullAllowed] Action<NSDictionary> completionHandler);

		[iOS (8,0), Mac (10,10)]
		[Export ("setCredential:forProtectionSpace:task:")]
		void SetCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task);

		[iOS (8,0), Mac (10,10)]
		[Export ("removeCredential:forProtectionSpace:options:task:")]
		void RemoveCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSDictionary options, NSUrlSessionTask task);

		[iOS (8,0), Mac (10,10)]
		[Async]
		[Export ("getDefaultCredentialForProtectionSpace:task:completionHandler:")]
		void GetDefaultCredential (NSUrlProtectionSpace space, NSUrlSessionTask task, [NullAllowed] Action<NSUrlCredential> completionHandler);

		[iOS (8,0), Mac (10,10)]
		[Export ("setDefaultCredential:forProtectionSpace:task:")]
		void SetDefaultCredential (NSUrlCredential credential, NSUrlProtectionSpace protectionSpace, NSUrlSessionTask task);
		
	}

#if XAMCORE_4_0
	delegate void NSUrlSessionPendingTasks (NSUrlSessionTask [] dataTasks, NSUrlSessionTask [] uploadTasks, NSUrlSessionTask[] downloadTasks);
#elif XAMCORE_3_0
	delegate void NSUrlSessionPendingTasks2 (NSUrlSessionTask [] dataTasks, NSUrlSessionTask [] uploadTasks, NSUrlSessionTask[] downloadTasks);
#else
	delegate void NSUrlSessionPendingTasks (NSUrlSessionDataTask [] dataTasks, NSUrlSessionUploadTask [] uploadTasks, NSUrlSessionDownloadTask[] downloadTasks);
	delegate void NSUrlSessionPendingTasks2 (NSUrlSessionTask [] dataTasks, NSUrlSessionTask [] uploadTasks, NSUrlSessionTask[] downloadTasks);
#endif
	delegate void NSUrlSessionAllPendingTasks (NSUrlSessionTask [] tasks);
	delegate void NSUrlSessionResponse (NSData data, NSUrlResponse response, NSError error);
	delegate void NSUrlSessionDownloadResponse (NSUrl data, NSUrlResponse response, NSError error);

	delegate void NSUrlDownloadSessionResponse (NSUrl location, NSUrlResponse response, NSError error);

	interface INSUrlSessionDelegate {}

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
	[iOS (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)] // 64-bit on 10.9, but 32/64-bit on 10.10
	[BaseType (typeof (NSObject), Name="NSURLSession")]
#if XAMCORE_2_0
	[DisableDefaultCtorAttribute]
#endif
	partial interface NSUrlSession {
	
		[Static, Export ("sharedSession", ArgumentSemantic.Strong)]
		NSUrlSession SharedSession { get; }
	
		[Static, Export ("sessionWithConfiguration:")]
		NSUrlSession FromConfiguration (NSUrlSessionConfiguration configuration);
	
		[Static, Export ("sessionWithConfiguration:delegate:delegateQueue:")]
		NSUrlSession FromWeakConfiguration (NSUrlSessionConfiguration configuration, [NullAllowed] NSObject weakDelegate, [NullAllowed] NSOperationQueue delegateQueue);
	
#if !XAMCORE_4_0
		[Obsolete ("Use the overload with a 'INSUrlSessionDelegate' parameter.")]
		[Static, Wrap ("FromWeakConfiguration (configuration, sessionDelegate, delegateQueue);")]
		NSUrlSession FromConfiguration (NSUrlSessionConfiguration configuration, NSUrlSessionDelegate sessionDelegate, NSOperationQueue delegateQueue);
#endif
		[Static, Wrap ("FromWeakConfiguration (configuration, (NSObject) sessionDelegate, delegateQueue);")]
		NSUrlSession FromConfiguration (NSUrlSessionConfiguration configuration, INSUrlSessionDelegate sessionDelegate, NSOperationQueue delegateQueue);

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
		// broken version that we must keep for XAMCORE_2_0 binary compatibility
		// but that we do not have to expose on tvOS and watchOS, forcing people to use the correct API
		[Obsolete ("Use GetTasks2 instead. This method may throw spurious InvalidCastExceptions, in particular for backgrounded tasks.")]
		[Export ("getTasksWithCompletionHandler:")]
		[Async (ResultTypeName="NSUrlSessionActiveTasks")]
		void GetTasks (NSUrlSessionPendingTasks completionHandler);
#elif XAMCORE_4_0
		// Fixed version (breaking change) only for XAMCORE_4_0
		[Export ("getTasksWithCompletionHandler:")]
		[Async (ResultTypeName="NSUrlSessionActiveTasks")]
		void GetTasks (NSUrlSessionPendingTasks completionHandler);
#endif

#if !XAMCORE_4_0
		// Workaround, not needed for XAMCORE_4_0+
		[Sealed]
		[Export ("getTasksWithCompletionHandler:")]
		[Async (ResultTypeName="NSUrlSessionActiveTasks2")]
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
		[Async (ResultTypeName="NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDataTask CreateDataTask (NSUrlRequest request, [NullAllowed] NSUrlSessionResponse completionHandler);
	
		[Export ("dataTaskWithURL:completionHandler:")]
		[return: ForcedType]
		[Async(ResultTypeName="NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDataTask CreateDataTask (NSUrl url, [NullAllowed] NSUrlSessionResponse completionHandler);
	
		[Export ("uploadTaskWithRequest:fromFile:completionHandler:")]
		[return: ForcedType]
		[Async(ResultTypeName="NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSUrl fileURL, NSUrlSessionResponse completionHandler);
	
		[Export ("uploadTaskWithRequest:fromData:completionHandler:")]
		[return: ForcedType]
		[Async(ResultTypeName="NSUrlSessionDataTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSData bodyData, NSUrlSessionResponse completionHandler);
	
		[Export ("downloadTaskWithRequest:completionHandler:")]
		[return: ForcedType]
		[Async(ResultTypeName="NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrlRequest request, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);
	
		[Export ("downloadTaskWithURL:completionHandler:")]
		[return: ForcedType]
		[Async(ResultTypeName="NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDownloadTask CreateDownloadTask (NSUrl url, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);

		[Export ("downloadTaskWithResumeData:completionHandler:")]
		[return: ForcedType]
		[Async(ResultTypeName="NSUrlSessionDownloadTaskRequest", PostNonResultSnippet = "result.Resume ();")]
		NSUrlSessionDownloadTask CreateDownloadTaskFromResumeData (NSData resumeData, [NullAllowed] NSUrlDownloadSessionResponse completionHandler);

        
		[iOS (9,0), Mac(10,11)]
		[Export ("getAllTasksWithCompletionHandler:")]
		[Async (ResultTypeName="NSUrlSessionCombinedTasks")]
		void GetAllTasks (NSUrlSessionAllPendingTasks completionHandler);

		[NoWatch]
		[iOS (9,0), Mac(10,11)]
		[Export ("streamTaskWithHostName:port:")]
		NSUrlSessionStreamTask CreateBidirectionalStream (string hostname, nint port);

		[NoWatch]
		[iOS (9,0), Mac(10,11)]
		[Export ("streamTaskWithNetService:")]
		NSUrlSessionStreamTask CreateBidirectionalStream (NSNetService service);
	}

	[iOS (9,0)]
	[Protocol, Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name="NSURLSessionStreamDelegate")]
	interface NSUrlSessionStreamDelegate
	{
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
	[iOS (9,0), Mac(10,11)]
	[BaseType (typeof(NSUrlSessionTask), Name="NSURLSessionStreamTask")]
	[DisableDefaultCtor]
	interface NSUrlSessionStreamTask
	{
		[Export ("readDataOfMinLength:maxLength:timeout:completionHandler:")]
		[Async (ResultTypeName="NSUrlSessionStreamDataRead")]
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
	
		[Export ("stopSecureConnection")]
		void StopSecureConnection ();
	}
	
	[iOS (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[BaseType (typeof (NSObject), Name="NSURLSessionTask")]
	partial interface NSUrlSessionTask : NSCopying, NSProgressReporting
	{
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

#if !MONOMAC || XAMCORE_2_0
		[iOS (8,0), Mac (10,10, onlyOn64 : true)]
		[Export ("priority")]
		float Priority { get; set; } /* float, not CGFloat */
#endif

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("earliestBeginDate", ArgumentSemantic.Copy)]
		NSDate EarliestBeginDate { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("countOfBytesClientExpectsToSend")]
		long CountOfBytesClientExpectsToSend { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("countOfBytesClientExpectsToReceive")]
		long CountOfBytesClientExpectsToReceive { get; set; }

	}

	[Static]
	[iOS (8,0)]
	[Mac (10,10)]
#if XAMCORE_2_0
	interface NSUrlSessionTaskPriority {
#else
	interface NSURLSessionTaskPriority {
#endif
		[Field ("NSURLSessionTaskPriorityDefault")]
		float Default { get; } /* float, not CGFloat */
		
		[Field ("NSURLSessionTaskPriorityLow")]
		float Low { get; } /* float, not CGFloat */
		
		[Field ("NSURLSessionTaskPriorityHigh")]
		float High { get; } /* float, not CGFloat */
	}
	
	// All of the NSUrlSession APIs are either 10.10, or 10.9 and 64-bit only
	// "NSURLSession is not available for i386 targets before Mac OS X 10.10."

	//
	// Empty interfaces, just to distinguish semantically their usage
	//
	[iOS (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[BaseType (typeof (NSUrlSessionTask), Name="NSURLSessionDataTask")]
	partial interface NSUrlSessionDataTask {}

	[iOS (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[BaseType (typeof (NSUrlSessionDataTask), Name="NSURLSessionUploadTask")]
	partial interface NSUrlSessionUploadTask {}

	[iOS (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[BaseType (typeof (NSUrlSessionTask), Name="NSURLSessionDownloadTask")]
	partial interface NSUrlSessionDownloadTask {
		[Export ("cancelByProducingResumeData:")]
		void Cancel (Action<NSData> resumeCallback);
	}
	

	[iOS (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[BaseType (typeof (NSObject), Name="NSURLSessionConfiguration")]
#if XAMCORE_2_0
	[DisableDefaultCtorAttribute]
#endif
	partial interface NSUrlSessionConfiguration : NSCopying {
	
		[Static, Export ("defaultSessionConfiguration", ArgumentSemantic.Strong)]
		NSUrlSessionConfiguration DefaultSessionConfiguration { get; }
	
		[Static, Export ("ephemeralSessionConfiguration", ArgumentSemantic.Strong)]
		NSUrlSessionConfiguration EphemeralSessionConfiguration { get; }
	
		[Static, Export ("backgroundSessionConfiguration:")]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'CreateBackgroundSessionConfiguration' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'CreateBackgroundSessionConfiguration' instead.")]
		NSUrlSessionConfiguration BackgroundSessionConfiguration (string identifier);
	
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
	
		[Export ("sessionSendsLaunchEvents")]
		bool SessionSendsLaunchEvents { get; set; }

		[NullAllowed]
		[Export ("connectionProxyDictionary", ArgumentSemantic.Copy)]
		NSDictionary ConnectionProxyDictionary { get; set; }
	
		[Export ("TLSMinimumSupportedProtocol")]
		SslProtocol TLSMinimumSupportedProtocol { get; set; }
	
		[Export ("TLSMaximumSupportedProtocol")]
		SslProtocol TLSMaximumSupportedProtocol { get; set; }
	
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
		[iOS (8,0), Mac (10,10)]
		[Export ("sharedContainerIdentifier")]
		string SharedContainerIdentifier { get; set; }

		[iOS (8,0)]
		[Static, Export ("backgroundSessionConfigurationWithIdentifier:")]
		NSUrlSessionConfiguration CreateBackgroundSessionConfiguration (string identifier);

		[iOS (9,0), Mac(10,11)]
		[Export ("shouldUseExtendedBackgroundIdleMode")]
		bool ShouldUseExtendedBackgroundIdleMode { get; set; }

		[NoWatch, NoTV, NoMac, iOS (11, 0)]
		[Export ("multipathServiceType", ArgumentSemantic.Assign)]
		NSUrlSessionMultipathServiceType MultipathServiceType { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[Export ("waitsForConnectivity")]
		bool WaitsForConnectivity { get; set; }
	}

	[iOS (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[Model, BaseType (typeof (NSObject), Name="NSURLSessionDelegate")]
	[Protocol]
	partial interface NSUrlSessionDelegate {
		[Export ("URLSession:didBecomeInvalidWithError:")]
		void DidBecomeInvalid (NSUrlSession session, NSError error);
	
		[Export ("URLSession:didReceiveChallenge:completionHandler:")]
		void DidReceiveChallenge (NSUrlSession session, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition,NSUrlCredential> completionHandler);
	
		[Export ("URLSessionDidFinishEventsForBackgroundURLSession:")]
		void DidFinishEventsForBackgroundSession (NSUrlSession session);
	}

	[iOS (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[Model]
	[BaseType (typeof (NSUrlSessionDelegate), Name="NSURLSessionTaskDelegate")]
	[Protocol]
	partial interface NSUrlSessionTaskDelegate {
	
		[Export ("URLSession:task:willPerformHTTPRedirection:newRequest:completionHandler:")]
		void WillPerformHttpRedirection (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, Action<NSUrlRequest> completionHandler);
	
		[Export ("URLSession:task:didReceiveChallenge:completionHandler:")]
		void DidReceiveChallenge (NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition,NSUrlCredential> completionHandler);
	
		[Export ("URLSession:task:needNewBodyStream:")]
		void NeedNewBodyStream (NSUrlSession session, NSUrlSessionTask task, Action<NSInputStream> completionHandler);
	
		[Export ("URLSession:task:didSendBodyData:totalBytesSent:totalBytesExpectedToSend:")]
		void DidSendBodyData (NSUrlSession session, NSUrlSessionTask task, long bytesSent, long totalBytesSent, long totalBytesExpectedToSend);
	
		[Export ("URLSession:task:didCompleteWithError:")]
		void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, NSError error);

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Export ("URLSession:task:didFinishCollectingMetrics:")]
		void DidFinishCollectingMetrics (NSUrlSession session, NSUrlSessionTask task, NSUrlSessionTaskMetrics metrics);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("URLSession:task:willBeginDelayedRequest:completionHandler:")]
		void WillBeginDelayedRequest (NSUrlSession session, NSUrlSessionTask task, NSUrlRequest request, Action<NSUrlSessionDelayedRequestDisposition, NSUrlRequest> completionHandler);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("URLSession:taskIsWaitingForConnectivity:")]
		void TaskIsWaitingForConnectivity (NSUrlSession session, NSUrlSessionTask task);
	}
	
	[iOS (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name="NSURLSessionDataDelegate")]
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

		[iOS(9,0), Mac(10,11)]
		[Export ("URLSession:dataTask:didBecomeStreamTask:")]
		void DidBecomeStreamTask (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlSessionStreamTask streamTask);
	}
	
	[iOS (7,0)]
	[Availability (Introduced = Platform.Mac_10_9)]
	[Model]
	[BaseType (typeof (NSUrlSessionTaskDelegate), Name="NSURLSessionDownloadDelegate")]
	[Protocol]
	partial interface NSUrlSessionDownloadDelegate {
	
#if XAMCORE_2_0
		[Abstract]
#endif
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
		
#if XAMCORE_4_0
		[Internal]
		[Export ("runLoopModes")]
		NSString [] _RunLoopModes { get; set; } 

		[Wrap ("RunLoopModes.GetConstants ()")]
		NSRunLoop [] RunLoopModes { get; set; } 
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

		[Advice ("Use the correctly named method: 'SetActionName'.")]
		[Export ("setActionName:")]
		void SetActionname (string actionName); 

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

	    [iOS (9,0), Mac(10,11)]
		[Export ("registerUndoWithTarget:handler:")]
		void RegisterUndo (NSObject target, Action<NSObject> undoHandler);

	}
	
	[BaseType (typeof (NSObject), Name="NSURLProtectionSpace")]
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
		nint  Port { get; }
	
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
#if XAMCORE_2_0
		[Internal]
#else
		[Obsolete ("Use ServerSecTrust")]
#endif
		[Export ("serverTrust")]
		IntPtr ServerTrust { get ; }

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
	
	[BaseType (typeof (NSObject), Name="NSURLRequest")]
	interface NSUrlRequest : NSSecureCoding, NSMutableCopying {
		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl url);

		[DesignatedInitializer]
		[Export ("initWithURL:cachePolicy:timeoutInterval:")]
		IntPtr Constructor (NSUrl url, NSUrlRequestCachePolicy cachePolicy, double timeoutInterval);

		[Export ("requestWithURL:")][Static]
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

		[iOS (6,0)]
		[Export ("allowsCellularAccess")]
		bool AllowsCellularAccess { get; }
		
		[Export ("HTTPMethod")]
		string HttpMethod { get; }

		[Export ("allHTTPHeaderFields")]
		NSDictionary Headers { get; }

		[Internal][Export ("valueForHTTPHeaderField:")]
		string Header (string field);

		[Export ("HTTPBody")]
		NSData Body { get; }

		[Export ("HTTPBodyStream")]
		NSInputStream BodyStream { get; }

		[Export ("HTTPShouldHandleCookies")]
		bool ShouldHandleCookies { get; }
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
		[Static,New]
		NSMutableDictionary FromDictionary (NSDictionary source);

		[Export ("dictionaryWithObjects:forKeys:count:")]
		[Static, Internal]
		NSMutableDictionary FromObjectsAndKeysInternalCount (NSArray objects, NSArray keys, nint count);

		[Export ("dictionaryWithObjects:forKeys:")]
		[Static, Internal, New]
		NSMutableDictionary FromObjectsAndKeysInternal (NSArray objects, NSArray Keys);
		
		[Export ("initWithDictionary:")]
		IntPtr Constructor (NSDictionary other);

		[Export ("initWithDictionary:copyItems:")]
		IntPtr Constructor (NSDictionary other, bool copyItems);

		[Export ("initWithContentsOfFile:")]
		IntPtr Constructor (string fileName);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl url);

		[Internal]
		[Export ("initWithObjects:forKeys:")]
		IntPtr Constructor (NSArray objects, NSArray keys);

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

		[iOS (6,0)]
		[Static]
		[Export ("dictionaryWithSharedKeySet:")]
		NSDictionary FromSharedKeySet (NSObject sharedKeyToken);
	}

	[BaseType (typeof (NSSet))]
	[DesignatedDefaultCtor]
	interface NSMutableSet {
		[Export ("initWithArray:")]
		IntPtr Constructor (NSArray other);

		[Export ("initWithSet:")]
		IntPtr Constructor (NSSet other);
		
		[DesignatedInitializer]
		[Export ("initWithCapacity:")]
		IntPtr Constructor (nint capacity);

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
	
	[BaseType (typeof (NSUrlRequest), Name="NSMutableURLRequest")]
	interface NSMutableUrlRequest {
		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl url);

		[Export ("initWithURL:cachePolicy:timeoutInterval:")]
		IntPtr Constructor (NSUrl url, NSUrlRequestCachePolicy cachePolicy, double timeoutInterval);

		[NullAllowed] // by default this property is null
		[New][Export ("URL")]
		NSUrl Url { get; set; }

		[New][Export ("cachePolicy")]
		NSUrlRequestCachePolicy CachePolicy { get; set; }

		[New][Export ("timeoutInterval")]
		double TimeoutInterval { set; get; }

		[NullAllowed] // by default this property is null
		[New][Export ("mainDocumentURL")]
		NSUrl MainDocumentURL { get; set; }

		[New][Export ("HTTPMethod")]
		string HttpMethod { get; set; }

		[NullAllowed] // by default this property is null
		[New][Export ("allHTTPHeaderFields")]
		NSDictionary Headers { get; set; }

		[Internal][Export ("setValue:forHTTPHeaderField:")]
		void _SetValue (string value, string field);

		[NullAllowed] // by default this property is null
		[New][Export ("HTTPBody")]
		NSData Body { get; set; }

		[NullAllowed] // by default this property is null
		[New][Export ("HTTPBodyStream")]
		NSInputStream BodyStream { get; set; }

		[New][Export ("HTTPShouldHandleCookies")]
		bool ShouldHandleCookies { get; set; }

		[Export ("networkServiceType")]
		NSUrlRequestNetworkServiceType NetworkServiceType { set; get; }

		[iOS (6,0)]
		[New] [Export ("allowsCellularAccess")]
		bool AllowsCellularAccess { get; set; }
	}
	
	[BaseType (typeof (NSObject), Name="NSURLResponse")]
	interface NSUrlResponse : NSSecureCoding, NSCopying {
		[DesignatedInitializer]
		[Export ("initWithURL:MIMEType:expectedContentLength:textEncodingName:")]
		IntPtr Constructor (NSUrl url, string mimetype, nint expectedContentLength, [NullAllowed] string textEncodingName);

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

	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSStreamDelegate)} )]
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

#if XAMCORE_4_0
		[Abstract]
#endif
		[Protected]
		[Export ("propertyForKey:")]
		NSObject GetProperty (NSString key);
	
#if XAMCORE_4_0
		[Abstract]
#endif
		[Protected]
		[Export ("setProperty:forKey:")]
		bool SetProperty ([NullAllowed] NSObject property, NSString key);
	
#if XAMCORE_4_0
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
		[Wrap ("Schedule (aRunLoop, mode.GetConstant ())")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode mode);

		[Wrap ("Unschedule (aRunLoop, mode.GetConstant ())")]
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
		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Field ("NSStreamNetworkServiceTypeCallSignaling")]
		NSString NetworkServiceTypeCallSignaling { get; }

		[iOS (8,0), Mac(10,10)]
		[Static, Export ("getBoundStreamsWithBufferSize:inputStream:outputStream:")]
		void GetBoundStreams (nuint bufferSize, out NSInputStream inputStream, out NSOutputStream outputStream);

		[NoWatch]
		[iOS (8,0), Mac (10,10)]
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
#if !(MONOMAC && !XAMCORE_2_0) // exclude Classic/XM
		, NSItemProviderReading, NSItemProviderWriting
#endif
	{
		[Export ("initWithData:encoding:")]
		IntPtr Constructor (NSData data, NSStringEncoding encoding);
#if MONOMAC
		[Bind ("sizeWithAttributes:")]
		CGSize StringSize ([NullAllowed] NSDictionary attributedStringAttributes);
		
		[Bind ("boundingRectWithSize:options:attributes:")]
		CGRect BoundingRectWithSize (CGSize size, NSStringDrawingOptions options, NSDictionary attributes);
		
		[Bind ("drawAtPoint:withAttributes:")]
		void DrawString (CGPoint point, NSDictionary attributes);
		
		[Bind ("drawInRect:withAttributes:")]
		void DrawString (CGRect rect, NSDictionary attributes);
		
		[Bind ("drawWithRect:options:attributes:")]
		void DrawString (CGRect rect, NSStringDrawingOptions options, NSDictionary attributes);
#else
#if !XAMCORE_2_0
		[Bind ("sizeWithFont:")]
		//[Obsolete ("Deprecated in iOS7.   Use 'NSString.GetSizeUsingAttributes (UIStringAttributes)' instead.")]
		CGSize StringSize (UIFont font);
		
		[Bind ("sizeWithFont:forWidth:lineBreakMode:")]
		//[Obsolete ("Deprecated in iOS7.   Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes,NSStringDrawingContext)' instead.")]
		CGSize StringSize (UIFont font, nfloat forWidth, UILineBreakMode breakMode);
		
		[Bind ("sizeWithFont:constrainedToSize:")]
		//[Obsolete ("Deprecated in iOS7.   Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes,NSStringDrawingContext)' instead.")]
		CGSize StringSize (UIFont font, CGSize constrainedToSize);
		
		[Bind ("sizeWithFont:constrainedToSize:lineBreakMode:")]
		//[Obsolete ("Deprecated in iOS7.   Use 'NSString.GetBoundingRect (CGSize, NSStringDrawingOptions, UIStringAttributes,NSStringDrawingContext)' instead.")]
		CGSize StringSize (UIFont font, CGSize constrainedToSize, UILineBreakMode lineBreakMode);

		[Bind ("sizeWithFont:minFontSize:actualFontSize:forWidth:lineBreakMode:")]
		// Wait for guidance here: https://devforums.apple.com/thread/203655
		//[Obsolete ("Deprecated on iOS7.   No guidance.")]
		CGSize StringSize (UIFont font, nfloat minFontSize, ref nfloat actualFontSize, nfloat forWidth, UILineBreakMode lineBreakMode);

		[Bind ("drawAtPoint:withFont:")]
		//[Obsolete ("Deprecated in iOS7.  Use 'NSString.DrawString(CGPoint, UIStringAttributes)' instead.")]
		CGSize DrawString (CGPoint point, UIFont font);

		[Bind ("drawAtPoint:forWidth:withFont:lineBreakMode:")]
		//[Obsolete ("Deprecated in iOS7.  Use 'NSString.DrawString(CGRect, UIStringAttributes)' instead.")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, UILineBreakMode breakMode);

		[Bind ("drawAtPoint:forWidth:withFont:fontSize:lineBreakMode:baselineAdjustment:")]
		//[Obsolete ("Deprecated in iOS7.  Use 'NSString.DrawString(CGRect, UIStringAttributes)' instead.")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, nfloat fontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment);

		[Bind ("drawAtPoint:forWidth:withFont:minFontSize:actualFontSize:lineBreakMode:baselineAdjustment:")]
		//[Obsolete ("Deprecated in iOS7.  Use 'NSString.DrawString(CGRect, UIStringAttributes)' instead.")]
		CGSize DrawString (CGPoint point, nfloat width, UIFont font, nfloat minFontSize, ref nfloat actualFontSize, UILineBreakMode breakMode, UIBaselineAdjustment adjustment);

		[Bind ("drawInRect:withFont:")]
		//[Obsolete ("Deprecated in iOS7.  Use 'NSString.DrawString(CGRect, UIStringAttributes)' instead.")]
		CGSize DrawString (CGRect rect, UIFont font);

		[Bind ("drawInRect:withFont:lineBreakMode:")]
		//[Obsolete ("Deprecated in iOS7.  Use 'NSString.DrawString(CGRect, UIStringAttributes)' instead.")]
		CGSize DrawString (CGRect rect, UIFont font, UILineBreakMode mode);

		[Bind ("drawInRect:withFont:lineBreakMode:alignment:")]
		//[Obsolete ("Deprecated in iOS7.  Use 'NSString.DrawString(CGRect, UIStringAttributes)' instead.")]
		CGSize DrawString (CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment);
#endif
#endif

#if XAMCORE_2_0
		[Internal]
#endif
		[Export ("characterAtIndex:")]
		char _characterAtIndex (nint index);

		[Export ("length")]
		nint Length {get;}

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
		[Export("pathWithComponents:")]
		string[] PathWithComponents( string[] components );

		[Export("pathComponents")]
		string[] PathComponents { get; }

		[Export("isAbsolutePath")]
		bool IsAbsolutePath { get; }

		[Export("lastPathComponent")]
		NSString LastPathComponent { get; }

		[Export("stringByDeletingLastPathComponent")]
		NSString DeleteLastPathComponent();
 
 		[Export("stringByAppendingPathComponent:")]
 		NSString AppendPathComponent( NSString str );

 		[Export("pathExtension")]
 		NSString PathExtension { get; }

 		[Export("stringByDeletingPathExtension")]
 		NSString DeletePathExtension();

 		[Export("stringByAppendingPathExtension:")]
 		NSString AppendPathExtension( NSString str );
 
 		[Export("stringByAbbreviatingWithTildeInPath")]
 		NSString AbbreviateTildeInPath();

 		[Export("stringByExpandingTildeInPath")]
 		NSString ExpandTildeInPath();
 
 		[Export("stringByStandardizingPath")]
 		NSString StandarizePath();

 		[Export("stringByResolvingSymlinksInPath")]
 		NSString ResolveSymlinksInPath();

		[Export("stringsByAppendingPaths:")]
 		string[] AppendPaths( string[] paths );

		// end methods from NSStringPathExtensions category

		[iOS (6,0)]
		[Export ("capitalizedStringWithLocale:")]
		string Capitalize ([NullAllowed] NSLocale locale);
		
		[iOS (6,0)]
		[Export ("lowercaseStringWithLocale:")]
		string ToLower (NSLocale locale);
		
		[iOS (6,0)]
		[Export ("uppercaseStringWithLocale:")]
		string ToUpper (NSLocale locale);

		[iOS (8,0)]
		[Export ("containsString:")]
		bool Contains (NSString str);

		[iOS (8,0), Mac (10,10)]
		[Export ("localizedCaseInsensitiveContainsString:")]
		bool LocalizedCaseInsensitiveContains (NSString str);

		[iOS (8,0), Mac (10,10)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Static, Export ("stringEncodingForData:encodingOptions:convertedString:usedLossyConversion:")]
		nuint DetectStringEncoding (NSData rawData, NSDictionary options, out string convertedString, out bool usedLossyConversion);

		[iOS (8,0), Mac(10,10)]
		[Static, Wrap ("DetectStringEncoding(rawData,options == null ? null : options.Dictionary, out convertedString, out usedLossyConversion)")]
		nuint DetectStringEncoding (NSData rawData, EncodingDetectionOptions options, out string convertedString, out bool usedLossyConversion);

		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionSuggestedEncodingsKey")]
		NSString EncodingDetectionSuggestedEncodingsKey { get; }

		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionDisallowedEncodingsKey")]
		NSString EncodingDetectionDisallowedEncodingsKey { get; }
		
		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionUseOnlySuggestedEncodingsKey")]
		NSString EncodingDetectionUseOnlySuggestedEncodingsKey { get; }
		
		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionAllowLossyKey")]
		NSString EncodingDetectionAllowLossyKey { get; }

		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionFromWindowsKey")]
		NSString EncodingDetectionFromWindowsKey { get; }

		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionLossySubstitutionKey")]
		NSString EncodingDetectionLossySubstitutionKey { get; }

		[iOS (8,0),Mac(10,10)]
		[Internal, Field ("NSStringEncodingDetectionLikelyLanguageKey")]
		NSString EncodingDetectionLikelyLanguageKey { get; }

		[Export ("lineRangeForRange:")]
		NSRange LineRangeForRange (NSRange range);

		[Export ("getLineStart:end:contentsEnd:forRange:")]
		void GetLineStart (out nuint startPtr, out nuint lineEndPtr, out nuint contentsEndPtr, NSRange range);

		[iOS (9,0), Mac(10,11)]
		[Export ("variantFittingPresentationWidth:")]
		NSString GetVariantFittingPresentationWidth (nint width);

		[iOS (9,0), Mac(10,11)]
		[Export ("localizedStandardContainsString:")]
		bool LocalizedStandardContainsString (NSString str);

		[iOS (9,0), Mac(10,11)]
		[Export ("localizedStandardRangeOfString:")]
		NSRange LocalizedStandardRangeOfString (NSString str);

		[iOS (9,0), Mac(10,11)]
		[Export ("localizedUppercaseString")]
		NSString LocalizedUppercaseString { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("localizedLowercaseString")]
		NSString LocalizedLowercaseString { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("localizedCapitalizedString")]
		NSString LocalizedCapitalizedString { get; }

		[iOS (9,0), Mac(10,11)]
		[Export ("stringByApplyingTransform:reverse:")]
		[return: NullAllowed]
		NSString TransliterateString (NSString transform, bool reverse);

		[Export ("hasPrefix:")]
		bool HasPrefix (NSString prefix);

		[Export ("hasSuffix:")]
		bool HasSuffix (NSString suffix);

		// UNUserNotificationCenterSupport category
		[iOS (10,0), Watch (3,0), NoTV, Mac (10,14, onlyOn64: true)]
		[Static]
		[Export ("localizedUserNotificationStringForKey:arguments:")]
		string GetLocalizedUserNotificationString (string key, [Params] [NullAllowed] NSObject [] arguments);

		[Export ("getParagraphStart:end:contentsEnd:forRange:")]
		void GetParagraphPositions (out nuint paragraphStartPosition, out nuint paragraphEndPosition, out nuint contentsEndPosition, NSRange range);

		[Export ("paragraphRangeForRange:")]
		NSRange GetParagraphRange (NSRange range);

		// From the NSItemProviderReading protocol

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Static]
		[Export ("readableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if XAMCORE_2_0 || !MONOMAC
		new
#endif
		string[] ReadableTypeIdentifiers { get; }

		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Static]
		[Export ("objectWithItemProviderData:typeIdentifier:error:")]
		[return: NullAllowed]
#if XAMCORE_2_0 || !MONOMAC
		new
#endif
		NSString GetObject (NSData data, string typeIdentifier, [NullAllowed] out NSError outError);

		// From the NSItemProviderWriting protocol
		[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
		[Static]
		[Export ("writableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
#if XAMCORE_2_0 || !MONOMAC
		new
#endif
		string[] WritableTypeIdentifiers { get; }
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
		IntPtr Constructor (nint capacity);

		[PreSnippet ("Check (index);")]
		[Export ("insertString:atIndex:")]
		void Insert (NSString str, nint index);

		[PreSnippet ("Check (range);")]
		[Export ("deleteCharactersInRange:")]
		void DeleteCharacters (NSRange range);

		[Export ("appendString:")]
		void Append (NSString str);

		[Export ("setString:")]
		void SetString (NSString str);

		[PreSnippet ("Check (range);")]
		[Export ("replaceOccurrencesOfString:withString:options:range:")]
		nuint ReplaceOcurrences (NSString target, NSString replacement, NSStringCompareOptions options, NSRange range);

		[iOS (9,0), Mac(10,11)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("applyTransform:reverse:range:updatedRange:")]
		bool ApplyTransform (NSString transform, bool reverse, NSRange range, out NSRange resultingRange);

		[iOS (9,0)][Mac (10,11)]
		[Wrap ("ApplyTransform (transform.GetConstant (), reverse, range, out resultingRange)")]
		bool ApplyTransform (NSStringTransform transform, bool reverse, NSRange range, out NSRange resultingRange);

		[Export ("replaceCharactersInRange:withString:")]
		void ReplaceCharactersInRange (NSRange range, NSString aString);
	}
	
	[Category, BaseType (typeof (NSString))]
#if XAMCORE_2_0
	partial interface NSUrlUtilities_NSString {
#else
	partial interface NSURLUtilities_NSString {
#endif
		[iOS (7,0)]
		[Export ("stringByAddingPercentEncodingWithAllowedCharacters:")]
		NSString CreateStringByAddingPercentEncoding (NSCharacterSet allowedCharacters);
	
		[iOS (7,0)]
		[Export ("stringByRemovingPercentEncoding")]
		NSString CreateStringByRemovingPercentEncoding ();
	
		[Export ("stringByAddingPercentEscapesUsingEncoding:")]
		NSString CreateStringByAddingPercentEscapes (NSStringEncoding enc);
	
		[Export ("stringByReplacingPercentEscapesUsingEncoding:")]
		NSString CreateStringByReplacingPercentEscapes (NSStringEncoding enc);
	}

	
#if !MONOMAC
	// This comes from UIKit.framework/Headers/NSStringDrawing.h
	[iOS (6,0)]
	[BaseType (typeof (NSObject))]
	interface NSStringDrawingContext {
		[Export ("minimumScaleFactor")]
		nfloat MinimumScaleFactor { get; set;  }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0)]
		[Export ("minimumTrackingAdjustment")]
		nfloat MinimumTrackingAdjustment { get; set;  }

		[Export ("actualScaleFactor")]
		nfloat ActualScaleFactor { get;  }

		[NoTV]
		[Availability (Deprecated = Platform.iOS_7_0)]
		[Export ("actualTrackingAdjustment")]
		nfloat ActualTrackingAdjustment { get;  }

		[Export ("totalBounds")]
		CGRect TotalBounds { get;  }
	}
#endif

	[BaseType (typeof (NSStream))]
	[DefaultCtorVisibility (Visibility.Protected)]
	interface NSInputStream {
		[Export ("hasBytesAvailable")]
		bool HasBytesAvailable ();
	
		[Export ("initWithFileAtPath:")]
		IntPtr Constructor (string path);

		[DesignatedInitializer]
		[Export ("initWithData:")]
		IntPtr Constructor (NSData data);

		[DesignatedInitializer]
		[Export ("initWithURL:")]
		IntPtr Constructor (NSUrl url);

		[Static]
		[Export ("inputStreamWithData:")]
		NSInputStream FromData (NSData data);
	
		[Static]
		[Export ("inputStreamWithFileAtPath:")]
		NSInputStream FromFile (string  path);

		[Static]
		[Export ("inputStreamWithURL:")]
		NSInputStream FromUrl (NSUrl url);

#if XAMCORE_4_0
		[Export ("propertyForKey:"), Override]
		NSObject GetProperty (NSString key);

		[Export ("setProperty:forKey:"), Override]
		bool SetProperty ([NullAllowed] NSObject property, NSString key);

#endif

	}

	delegate bool NSEnumerateLinguisticTagsEnumerator (NSString tag, NSRange tokenRange, NSRange sentenceRange, ref bool stop);

	[Category]
	[BaseType (typeof(NSString))]
	interface NSLinguisticAnalysis {
#if XAMCORE_4_0
		[return: BindAs (typeof (NSLinguisticTag []))]
#else
		[return: BindAs (typeof (NSLinguisticTagUnit []))]
#endif
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("linguisticTagsInRange:scheme:options:orthography:tokenRanges:")]
		NSString[] GetLinguisticTags (NSRange range, NSString scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, [NullAllowed] out NSValue[] tokenRanges);

		[Wrap ("GetLinguisticTags (This, range, scheme.GetConstant (), options, orthography, out tokenRanges)")]
#if XAMCORE_4_0
		NSLinguisticTag[] GetLinguisticTags (NSRange range, NSLinguisticTagScheme scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, [NullAllowed] out NSValue[] tokenRanges);
#else
		NSLinguisticTagUnit[] GetLinguisticTags (NSRange range, NSLinguisticTagScheme scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, [NullAllowed] out NSValue[] tokenRanges);
#endif

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[Export ("enumerateLinguisticTagsInRange:scheme:options:orthography:usingBlock:")]
		void EnumerateLinguisticTags (NSRange range, NSString scheme, NSLinguisticTaggerOptions options, [NullAllowed] NSOrthography orthography, NSEnumerateLinguisticTagsEnumerator handler);

		[Wrap ("EnumerateLinguisticTags (This, range, scheme.GetConstant (), options, orthography, handler)")]
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
		bool ConformsToProtocol (IntPtr /* Protocol */ aProtocol);

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
#if MONOMAC
		// Cocoa Bindings added by Kenneth J. Pouncey 2010/11/17
		[Sealed]
		[Export ("valueClassForBinding:")]
		Class GetBindingValueClass (NSString binding);

#if !XAMCORE_4_0
		[Obsolete ("Use 'Bind (NSString binding, NSObject observable, string keyPath, [NullAllowed] NSDictionary options)' instead.")]
		[Export ("bind:toObject:withKeyPath:options:")]
		void Bind (string binding, NSObject observable, string keyPath, [NullAllowed] NSDictionary options);

		[Obsolete ("Use 'Unbind (NSString binding)' instead.")]
		[Export ("unbind:")]
		void Unbind (string binding);

		[Obsolete ("Use 'GetBindingValueClass (NSString binding)' instead.")]
		[Export ("valueClassForBinding:")]
		Class BindingValueClass (string binding);

		[Obsolete ("Use 'GetBindingInfo (NSString binding)' instead.")]
		[Export ("infoForBinding:")]
		NSDictionary BindingInfo (string binding);

		[Obsolete ("Use 'GetBindingOptionDescriptions (NSString aBinding)' instead.")]
		[Export ("optionDescriptionsForBinding:")]
		NSObject[] BindingOptionDescriptions (string aBinding);

		[Static]
		[Wrap ("GetDefaultPlaceholder (marker, (NSString) binding)")]
		NSObject GetDefaultPlaceholder (NSObject marker, string binding);

		[Static]
		[Obsolete ("Use 'SetDefaultPlaceholder (NSObject placeholder, NSObject marker, NSString binding)' instead.")]
		[Wrap ("SetDefaultPlaceholder (placeholder, marker, (NSString) binding)")]
		void SetDefaultPlaceholder (NSObject placeholder, NSObject marker, string binding);

		[Export ("exposedBindings")]
		NSString[] ExposedBindings ();
#else
		[Export ("exposedBindings")]
		NSString[] ExposedBindings { get; }
#endif
		[Sealed]
		[Export ("bind:toObject:withKeyPath:options:")]
		void Bind (NSString binding, NSObject observable, string keyPath, [NullAllowed] NSDictionary options);

		[Sealed]
		[Export ("unbind:")]
		void Unbind (NSString binding);

		[Sealed]
		[Export ("infoForBinding:")]
		NSDictionary GetBindingInfo (NSString binding);

		[Sealed]
		[Export ("optionDescriptionsForBinding:")]
		NSObject[] GetBindingOptionDescriptions (NSString aBinding);

		// NSPlaceholders (informal) protocol
		[Static]
		[Export ("defaultPlaceholderForMarker:withBinding:")]
		NSObject GetDefaultPlaceholder (NSObject marker, NSString binding);

		[Static]
		[Export ("setDefaultPlaceholder:forMarker:withBinding:")]
		void SetDefaultPlaceholder (NSObject placeholder, NSObject marker, NSString binding);

		[Export ("objectDidEndEditing:")]
		void ObjectDidEndEditing (NSObject editor);

		[Export ("commitEditing")]
		bool CommitEditing ();

		[Export ("commitEditingWithDelegate:didCommitSelector:contextInfo:")]
		//void CommitEditingWithDelegateDidCommitSelectorContextInfo (NSObject objDelegate, Selector didCommitSelector, IntPtr contextInfo);
		void CommitEditing (NSObject objDelegate, Selector didCommitSelector, IntPtr contextInfo);
#endif
		[Export ("methodForSelector:")]
		IntPtr GetMethodForSelector (Selector sel);

		[PreSnippet ("if (!(this is INSCopying)) throw new InvalidOperationException (\"Type does not conform to NSCopying\");")]
		[Export ("copy")]
		[return: Release ()]
		NSObject Copy ();

		[PreSnippet ("if (!(this is INSMutableCopying)) throw new InvalidOperationException (\"Type does not conform to NSMutableCopying\");")]
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
		void PerformSelector (Selector selector, NSThread onThread, [NullAllowed] NSObject withObject, bool waitUntilDone, NSString [] nsRunLoopModes);
		
		[Static, Export ("cancelPreviousPerformRequestsWithTarget:")]
		void CancelPreviousPerformRequest (NSObject aTarget);

		[Static, Export ("cancelPreviousPerformRequestsWithTarget:selector:object:")]
		void CancelPreviousPerformRequest (NSObject aTarget, Selector selector, [NullAllowed] NSObject argument);

		[iOS (8,0), Mac (10,10)]
		[NoWatch]
		[Export ("prepareForInterfaceBuilder")]
		void PrepareForInterfaceBuilder ();

		[NoWatch]
#if MONOMAC
		// comes from NSNibAwaking category and does not requires calling super
#else
		[RequiresSuper] // comes from UINibLoadingAdditions category - which is decorated
#endif
		[Export ("awakeFromNib")]
		void AwakeFromNib ();
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
		[Export ("self")][Transient]
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
		bool ConformsToProtocol ([NullAllowed] IntPtr /* Protocol */ aProtocol);

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
#if XAMCORE_2_0
		nuint RetainCount { get; }
#else
		nint RetainCount { get; }
#endif

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

		[Export ("addDependency:")][PostGet ("Dependencies")]
		void AddDependency (NSOperation op);

		[Export ("removeDependency:")][PostGet ("Dependencies")]
		void RemoveDependency (NSOperation op);

		[Export ("dependencies")]
		NSOperation [] Dependencies { get; }

		[NullAllowed]
		[Export ("completionBlock", ArgumentSemantic.Copy)]
		Action CompletionBlock { get; set; }

		[Export ("waitUntilFinished")]
		void WaitUntilFinished ();

		[Export ("threadPriority")]
		double ThreadPriority { get; set; }

		//Detected properties
		[Export ("queuePriority")]
		NSOperationQueuePriority QueuePriority { get; set; }

		[iOS (7,0)]
		[Export ("asynchronous")]
		bool Asynchronous { [Bind ("isAsynchronous")] get; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("qualityOfService")]
		NSQualityOfService QualityOfService { get; set; }

		[iOS (8,0)][Mac (10,10)]
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
	interface NSOperationQueue {
		[Export ("addOperation:")][PostGet ("Operations")]
		void AddOperation ([NullAllowed] NSOperation op);

		[Export ("addOperations:waitUntilFinished:")][PostGet ("Operations")]
		void AddOperations ([NullAllowed] NSOperation [] operations, bool waitUntilFinished);

		[Export ("addOperationWithBlock:")][PostGet ("Operations")]
		void AddOperation (/* non null */ Action operation);

		[Export ("operations")]
		NSOperation [] Operations { get; }

		[Export ("operationCount")]
		nint OperationCount { get; }

		[Export ("name")]
		string Name { get; set; }

		[Export ("cancelAllOperations")][PostGet ("Operations")]
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
		bool Suspended { [Bind ("isSuspended")]get; set; }

		[iOS (8,0)][Mac (10,10)]
		[Export ("qualityOfService")]
		NSQualityOfService QualityOfService { get; set; }

		[NullAllowed]
		[iOS (8,0)][Mac (10,10)]
		[Export ("underlyingQueue", ArgumentSemantic.UnsafeUnretained)]
		DispatchQueue UnderlyingQueue { get; set; }
		
	}

#if XAMCORE_2_0
	interface NSOrderedSet<TKey> : NSOrderedSet {}
#endif

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSOrderedSet : NSSecureCoding, NSMutableCopying {
		[Export ("initWithObject:")]
		IntPtr Constructor (NSObject start);

		[Export ("initWithArray:"), Internal]
		IntPtr Constructor (NSArray array);

		[Export ("initWithSet:")]
		IntPtr Constructor (NSSet source);

		[Export ("initWithOrderedSet:")]
		IntPtr Constructor (NSOrderedSet source);

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
		NSObject FirstObject ();

		[Internal]
		[Sealed]
		[Export ("lastObject")]
		IntPtr _LastObject ();

		[Export ("lastObject")]
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
	}

#if XAMCORE_2_0
	interface NSMutableOrderedSet<TKey> : NSMutableOrderedSet {}
#endif

	[BaseType (typeof (NSOrderedSet))]
	[DesignatedDefaultCtor]
	interface NSMutableOrderedSet {
		[Export ("initWithObject:")]
		IntPtr Constructor (NSObject start);

		[Export ("initWithSet:")]
		IntPtr Constructor (NSSet source);

		[Export ("initWithOrderedSet:")]
		IntPtr Constructor (NSOrderedSet source);

		[DesignatedInitializer]
		[Export ("initWithCapacity:")]
		IntPtr Constructor (nint capacity);

		[Export ("initWithArray:"), Internal]
		IntPtr Constructor (NSArray array);

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
	}
	
	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: *** -[__NSArrayM insertObject:atIndex:]: object cannot be nil
	[DisableDefaultCtor]
	interface NSOrthography : NSSecureCoding, NSCopying {
		[Export ("dominantScript")]
		string DominantScript { get;  }

		[Export ("languageMap")]
		NSDictionary LanguageMap { get;  }

		[Export ("dominantLanguage")]
		string DominantLanguage { get;  }

		[Export ("allScripts")]
		string [] AllScripts { get;  }

		[Export ("allLanguages")]
		string [] AllLanguages { get;  }

		[Export ("languagesForScript:")]
		string [] LanguagesForScript (string script);

		[Export ("dominantLanguageForScript:")]
		string DominantLanguageForScript (string script);

		[DesignatedInitializer]
		[Export ("initWithDominantScript:languageMap:")]
		IntPtr Constructor (string dominantScript, [NullAllowed] NSDictionary languageMap);
	}
	
	[BaseType (typeof (NSStream))]
	[DisableDefaultCtor] // crash when used
	interface NSOutputStream {
		[DesignatedInitializer]
		[Export ("initToMemory")]
		IntPtr Constructor ();

		[Export ("hasSpaceAvailable")]
		bool HasSpaceAvailable ();
	
		//[Export ("initToBuffer:capacity:")]
		//IntPtr Constructor (uint8_t  buffer, NSUInteger capacity);

		[Export ("initToFileAtPath:append:")]
		IntPtr Constructor (string path, bool shouldAppend);

		[Static]
		[Export ("outputStreamToMemory")]
#if XAMCORE_2_0
		NSObject OutputStreamToMemory ();
#else
		NSOutputStream OutputStreamToMemory ();
#endif

		//[Static]
		//[Export ("outputStreamToBuffer:capacity:")]
		//NSObject OutputStreamToBuffer (uint8_t  buffer, NSUInteger capacity);

		[Static]
		[Export ("outputStreamToFileAtPath:append:")]
		NSOutputStream CreateFile (string path, bool shouldAppend);

#if XAMCORE_4_0
		[Export ("propertyForKey:"), Override]
		NSObject GetProperty (NSString key);

		[Export ("setProperty:forKey:"), Override]
		bool SetProperty ([NullAllowed] NSObject property, NSString key);

#endif
	}

	[BaseType (typeof (NSObject), Name="NSHTTPCookie")]
	// default 'init' crash both simulator and devices
	[DisableDefaultCtor]
	interface NSHttpCookie {
		[Export ("initWithProperties:")]
		IntPtr Constructor (NSDictionary properties);

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

#if XAMCORE_2_0
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
#endif
	}

	[BaseType (typeof (NSObject), Name="NSHTTPCookieStorage")]
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
		[Mac (10,10)][iOS (8,0)]
		[Export ("removeCookiesSinceDate:")]
		void RemoveCookiesSinceDate (NSDate date);

		[iOS (9,0), Mac (10,11)]
		[Static]
		[Export ("sharedCookieStorageForGroupContainerIdentifier:")]
		NSHttpCookieStorage GetSharedCookieStorage (string groupContainerIdentifier);
		
#if !MONOMAC || XAMCORE_2_0
		[Mac (10,10)][iOS (8,0)]
		[Async]
		[Export ("getCookiesForTask:completionHandler:")]
		void GetCookiesForTask (NSUrlSessionTask task, Action<NSHttpCookie []> completionHandler);

		[Mac (10,10)][iOS (8,0)]
		[Export ("storeCookies:forTask:")]
		void StoreCookies (NSHttpCookie [] cookies, NSUrlSessionTask task);
#endif
#if XAMCORE_2_0
		[Notification]
		[Field ("NSHTTPCookieManagerAcceptPolicyChangedNotification")]
		NSString CookiesChangedNotification { get; }

		[Notification]
		[Field ("NSHTTPCookieManagerCookiesChangedNotification")]
		NSString AcceptPolicyChangedNotification { get; }
#endif
	}
	
	[BaseType (typeof (NSUrlResponse), Name="NSHTTPURLResponse")]
	interface NSHttpUrlResponse {
		[Export ("initWithURL:MIMEType:expectedContentLength:textEncodingName:")]
		IntPtr Constructor (NSUrl url, string mimetype, nint expectedContentLength, [NullAllowed] string textEncodingName);

		[Export ("initWithURL:statusCode:HTTPVersion:headerFields:")]
		IntPtr Constructor (NSUrl url, nint statusCode, string httpVersion, NSDictionary headerFields);
		
		[Export ("statusCode")]
		nint StatusCode { get; }

		[Export ("allHeaderFields")]
		NSDictionary AllHeaderFields { get; }

		[Export ("localizedStringForStatusCode:")][Static]
		string LocalizedStringForStatusCode (nint statusCode);
	}
	
	[BaseType (typeof (NSObject))]
#if MONOMAC
	[DisableDefaultCtor] // An uncaught exception was raised: -[__NSCFDictionary removeObjectForKey:]: attempt to remove nil key
#endif
	partial interface NSBundle {
		[Export ("mainBundle")][Static]
		NSBundle MainBundle { get; }

		[Export ("bundleWithPath:")][Static]
		NSBundle FromPath (string path);

		[DesignatedInitializer]
		[Export ("initWithPath:")]
		IntPtr Constructor (string path);

		[Export ("bundleForClass:")][Static]
		NSBundle FromClass (Class c);

		[Export ("bundleWithIdentifier:")][Static]
		NSBundle FromIdentifier (string str);

		[Export ("allBundles")][Static]
		NSBundle [] _AllBundles { get; }

		[Export ("allFrameworks")][Static]
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
		string  ResourcePath { get; }
		
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

		[Export ("pathForResource:ofType:inDirectory:")][Static]
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
		NSDictionary InfoDictionary{ get; }

		// Additions from AppKit
#if MONOMAC
		[Mac (10,8)]
		[Export ("loadNibNamed:owner:topLevelObjects:")]
		bool LoadNibNamed (string nibName, [NullAllowed] NSObject owner, out NSArray topLevelObjects);

		// https://developer.apple.com/library/mac/#documentation/Cocoa/Reference/ApplicationKit/Classes/NSBundle_AppKitAdditions/Reference/Reference.html
		[Static]
		[Export ("loadNibNamed:owner:")]
		bool LoadNib (string nibName, NSObject owner);

		[Export ("pathForImageResource:")]
		string PathForImageResource (string resource);

		[Export ("pathForSoundResource:")]
		string PathForSoundResource (string resource);

		[Export ("URLForImageResource:")]
		NSUrl GetUrlForImageResource (string resource);

		[Export ("contextHelpForKey:")]
		NSAttributedString GetContextHelp (string key);
#else
		// http://developer.apple.com/library/ios/#documentation/uikit/reference/NSBundle_UIKitAdditions/Introduction/Introduction.html
		[NoWatch]
		[Export ("loadNibNamed:owner:options:")]
		NSArray LoadNib (string nibName, [NullAllowed] NSObject owner, [NullAllowed] NSDictionary options);
#endif

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
		IntPtr Constructor (NSUrl url);
		
		[Static, Export ("bundleWithURL:")]
		NSBundle FromUrl (NSUrl url);

		[Export ("preferredLocalizations")]
		string [] PreferredLocalizations { get; }

		[Export ("localizations")]
		string [] Localizations { get; }

		[iOS (7,0)]
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
		NSUrl [] GetUrlsForResourcesWithExtension (string fileExtension, [NullAllowed] string subdirectory,  [NullAllowed] string localizationName);

#if !MONOMAC
		[iOS (9,0)]
		[Export ("preservationPriorityForTag:")]
		double GetPreservationPriority (NSString tag);

		[iOS (9,0)]
		[Export ("setPreservationPriority:forTags:")]
		void SetPreservationPriority (double priority, NSSet<NSString> tags);
#endif
	}

#if !MONOMAC
	[iOS (9,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSBundleResourceRequest : NSProgressReporting
	{
		[Export ("initWithTags:")]
		IntPtr Constructor (NSSet<NSString> tags);
	
		[Export ("initWithTags:bundle:")]
		[DesignatedInitializer]
		IntPtr Constructor (NSSet<NSString> tags, NSBundle bundle);
	
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
#endif
		
	[BaseType (typeof (NSObject))]
	interface NSIndexPath : NSCoding, NSSecureCoding, NSCopying {
		[Export ("indexPathWithIndex:")][Static]
		NSIndexPath FromIndex (nuint index);

		[Export ("indexPathWithIndexes:length:")][Internal][Static]
		NSIndexPath _FromIndex (IntPtr indexes, nint len);

		[Export ("indexPathByAddingIndex:")]
		NSIndexPath IndexPathByAddingIndex (nuint index);
		
		[Export ("indexPathByRemovingLastIndex")]
		NSIndexPath IndexPathByRemovingLastIndex ();

		[Export ("indexAtPosition:")]
		nuint IndexAtPosition (nint position);

		[Export ("length")]
		nint Length { get; } 

		[Export ("getIndexes:")][Internal]
		void _GetIndexes (IntPtr target);

		[Mac (10,9)][iOS (7,0)]
		[Export ("getIndexes:range:")][Internal]
		void _GetIndexes (IntPtr target, NSRange positionRange);

		[Export ("compare:")]
		nint Compare (NSIndexPath other);

#if !MONOMAC
		// NSIndexPath UIKit Additions Reference
		// https://developer.apple.com/library/ios/#documentation/UIKit/Reference/NSIndexPath_UIKitAdditions/Reference/Reference.html

#if XAMCORE_2_0
		// see monotouch/src/UIKit/Addition.cs for int-returning Row/Section properties

		[NoWatch]
		[Export ("row")]
		nint LongRow { get; }

		[NoWatch]
		[Export ("section")]
		nint LongSection { get; }
#else
		[Export ("row")]
		nint Row { get; }

		[Export ("section")]
		nint Section { get; }
#endif

		[NoWatch]
		[Static]
		[Export ("indexPathForRow:inSection:")]
		NSIndexPath FromRowSection (nint row, nint section);
#else

		[Mac (10,11)]
		[Export ("section")]
		nint Section { get; }
#endif

		[NoWatch]
		[Static]
		[iOS (6,0)][Mac (10,11)]
		[Export ("indexPathForItem:inSection:")]
		NSIndexPath FromItemSection (nint item, nint section);

		[NoWatch]
		[Export ("item")]
		[iOS (6,0)][Mac (10,11)]
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
		IntPtr Constructor (nuint index);

		[DesignatedInitializer]
		[Export ("initWithIndexSet:")]
		IntPtr Constructor (NSIndexSet other);

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


	[iOS (8,0)][Mac (10,10, onlyOn64 : true)] // Not defined in 32-bit
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	partial interface NSItemProvider : NSCopying {
		[DesignatedInitializer]
		[Export ("initWithItem:typeIdentifier:")]
		IntPtr Constructor ([NullAllowed] NSObject item, string typeIdentifier);

		[Export ("initWithContentsOfURL:")]
		IntPtr Constructor (NSUrl fileUrl);

		[Export ("registeredTypeIdentifiers", ArgumentSemantic.Copy)]
		string [] RegisteredTypeIdentifiers { get; }

		[Export ("registerItemForTypeIdentifier:loadHandler:")]
		void RegisterItemForTypeIdentifier (string typeIdentifier, NSItemProviderLoadHandler loadHandler);

		[Export ("hasItemConformingToTypeIdentifier:")]
		bool HasItemConformingTo (string typeIdentifier);

		[Async]
		[Export ("loadItemForTypeIdentifier:options:completionHandler:")]
		void LoadItem (string typeIdentifier, [NullAllowed] NSDictionary options, [NullAllowed] Action<NSObject,NSError> completionHandler);

		[Field ("NSItemProviderPreferredImageSizeKey")]
		NSString PreferredImageSizeKey { get; }		

		[Export ("setPreviewImageHandler:")]
		void SetPreviewImageHandler (NSItemProviderLoadHandler handler);

		[Async]
		[Export ("loadPreviewImageWithOptions:completionHandler:")]
		void LoadPreviewImage (NSDictionary options, Action<NSObject,NSError> completionHandler);

		[Field ("NSItemProviderErrorDomain")]
		NSString ErrorDomain { get; }

#if MONOMAC
		[Mac (10,10)]
		[Export ("sourceFrame")]
		CGRect SourceFrame { get; }

		[Mac (10,10)]
		[Export ("containerFrame")]
		CGRect ContainerFrame { get; }

		[Mac (10,10)]
		[Export ("preferredPresentationSize")]
		CGSize PreferredPresentationSize { get; }

		[Mac (10,12)] // [Async] handled by NSItemProvider.cs for backwards compat reasons
		[Export ("registerCloudKitShareWithPreparationHandler:")]
		void RegisterCloudKitShare (CloudKitRegistrationPreparationAction preparationHandler);

		[Mac (10,12)]
		[Export ("registerCloudKitShare:container:")]
		void RegisterCloudKitShare (CKShare share, CKContainer container);
#endif
		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("registerDataRepresentationForTypeIdentifier:visibility:loadHandler:")]
		void RegisterDataRepresentation (string typeIdentifier, NSItemProviderRepresentationVisibility visibility, RegisterDataRepresentationLoadHandler loadHandler);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("registerFileRepresentationForTypeIdentifier:fileOptions:visibility:loadHandler:")]
		void RegisterFileRepresentation (string typeIdentifier, NSItemProviderFileOptions fileOptions, NSItemProviderRepresentationVisibility visibility, RegisterFileRepresentationLoadHandler loadHandler);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("registeredTypeIdentifiersWithFileOptions:")]
		string[] GetRegisteredTypeIdentifiers (NSItemProviderFileOptions fileOptions);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("hasRepresentationConformingToTypeIdentifier:fileOptions:")]
		bool HasConformingRepresentation (string typeIdentifier, NSItemProviderFileOptions fileOptions);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Async, Export ("loadDataRepresentationForTypeIdentifier:completionHandler:")]
		NSProgress LoadDataRepresentation (string typeIdentifier, Action <NSData, NSError> completionHandler);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Async, Export ("loadFileRepresentationForTypeIdentifier:completionHandler:")]
		NSProgress LoadFileRepresentation (string typeIdentifier, Action <NSUrl, NSError> completionHandler);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Async (ResultTypeName = "LoadInPlaceResult"), Export ("loadInPlaceFileRepresentationForTypeIdentifier:completionHandler:")]
		NSProgress LoadInPlaceFileRepresentation (string typeIdentifier, LoadInPlaceFileRepresentationHandler completionHandler);

		[NoWatch, NoTV, NoMac, iOS (11, 0)]
		[NullAllowed, Export ("suggestedName")]
		string SuggestedName { get; set; }

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("initWithObject:")]
		IntPtr Constructor (INSItemProviderWriting @object);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("registerObject:visibility:")]
		void RegisterObject (INSItemProviderWriting @object, NSItemProviderRepresentationVisibility visibility);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("registerObjectOfClass:visibility:loadHandler:")]
		void RegisterObject (Class aClass, NSItemProviderRepresentationVisibility visibility, RegisterObjectRepresentationLoadHandler loadHandler);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Wrap ("RegisterObject (new Class (type), visibility, loadHandler)")]
		void RegisterObject (Type type, NSItemProviderRepresentationVisibility visibility, RegisterObjectRepresentationLoadHandler loadHandler);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("canLoadObjectOfClass:")]
		bool CanLoadObject (Class aClass);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Wrap ("CanLoadObject (new Class (type))")]
		bool CanLoadObject (Type type);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Async, Export ("loadObjectOfClass:completionHandler:")]
		NSProgress LoadObject (Class aClass, Action<INSItemProviderReading, NSError> completionHandler);
#if !MONOMAC
		// NSItemProvider_UIKitAdditions category

		[NoWatch, NoTV]
		[iOS (11,0)]
		[NullAllowed, Export ("teamData", ArgumentSemantic.Copy)]
		NSData TeamData { get; set; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("preferredPresentationSize", ArgumentSemantic.Assign)]
		CGSize PreferredPresentationSize { get; set; }

		[NoWatch, NoTV]
		[iOS (11,0)]
		[Export ("preferredPresentationStyle", ArgumentSemantic.Assign)]
		UIPreferredPresentationStyle PreferredPresentationStyle { get; set; }
#endif // !MONOMAC
	}
    
	delegate NSProgress RegisterFileRepresentationLoadHandler ([BlockCallback] RegisterFileRepresentationCompletionHandler completionHandler);
	delegate void RegisterFileRepresentationCompletionHandler (NSUrl fileUrl, bool coordinated, NSError error);
	delegate void ItemProviderDataCompletionHandler (NSData data, NSError error);
	delegate NSProgress RegisterDataRepresentationLoadHandler ([BlockCallback] ItemProviderDataCompletionHandler completionHandler);
	delegate void LoadInPlaceFileRepresentationHandler (NSUrl fileUrl, bool isInPlace, NSError error);
	delegate NSProgress RegisterObjectRepresentationLoadHandler ([BlockCallback] RegisterObjectRepresentationCompletionHandler completionHandler);
	delegate void RegisterObjectRepresentationCompletionHandler (INSItemProviderWriting @object, NSError error);

	interface INSItemProviderReading {}
	
	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Protocol]
	interface NSItemProviderReading
	{
		// This static method has to be implemented on each class that implements
		// this, this is not a capability that exists in C#.
		// We are inlining these on each class that implements NSItemProviderReading
		// for the sake of the method being callable from C#, for user code, the
		// user needs to manually [Export] the selector on a static method, like
		// they do for the "layer" property on CALayer subclasses.
		//
		[Static, Abstract]
		[Export ("readableTypeIdentifiersForItemProvider", ArgumentSemantic.Copy)]
		string[] ReadableTypeIdentifiers { get; }

		[Static, Abstract]
		[Export ("objectWithItemProviderData:typeIdentifier:error:")]
		[return: NullAllowed]
		INSItemProviderReading GetObject (NSData data, string typeIdentifier, [NullAllowed] out NSError outError);
	}

	interface INSItemProviderWriting {}

	[Watch (4,0), TV (11,0), Mac (10,13, onlyOn64: true), iOS (11,0)]
	[Protocol]
	interface NSItemProviderWriting
	{
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
		string[] WritableTypeIdentifiers { get; }

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
		string[] WritableTypeIdentifiersForItemProvider { get; }

		[Export ("itemProviderVisibilityForRepresentationWithTypeIdentifier:")]
		// 'GetItemProviderVisibility' is a nicer name, but there's a static method with that name.
		NSItemProviderRepresentationVisibility GetItemProviderVisibilityForTypeIdentifier (string typeIdentifier);

		[Abstract]
		[Async, Export ("loadDataWithTypeIdentifier:forItemProviderCompletionHandler:")]
		[return: NullAllowed]
		NSProgress LoadData (string typeIdentifier, Action<NSData, NSError> completionHandler);
	}

#if XAMCORE_2_0
	[Static]
#endif
	[iOS (8,0), Mac (10,10, onlyOn64: true)]
	partial interface NSJavaScriptExtension {
		[Field ("NSExtensionJavaScriptPreprocessingResultsKey")]
		NSString PreprocessingResultsKey { get; }

		[Field ("NSExtensionJavaScriptFinalizeArgumentKey")]
		NSString FinalizeArgumentKey { get; }
	}

	[iOS (8,0), Mac (10,10)]
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

	[BaseType (typeof (NSObject), Name="NSJSONSerialization")]
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
		IntPtr Constructor (nuint index);

		[Export ("initWithIndexSet:")]
		IntPtr Constructor (NSIndexSet other);

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

	[NoWatch]
#if XAMCORE_3_0
	[DisableDefaultCtor] // the instance just crash when trying to call selectors
#endif
	[BaseType (typeof (NSObject), Delegates=new string [] { "WeakDelegate" }, Events=new Type [] { typeof (NSNetServiceDelegate)})]
	interface NSNetService {
		[DesignatedInitializer]
		[Export ("initWithDomain:type:name:port:")]
		IntPtr Constructor (string domain, string type, string name, int /* int, not NSInteger */ port);

		[Export ("initWithDomain:type:name:")]
		IntPtr Constructor (string domain, string type, string name);
		
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSNetServiceDelegate Delegate { get; set; }

#if XAMCORE_4_0
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
		[Wrap ("Schedule (aRunLoop, forMode.GetConstant ())")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

		[Wrap ("Unschedule (aRunLoop, forMode.GetConstant ())")]
		void Unschedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

		[Export ("domain", ArgumentSemantic.Copy)]
		string Domain { get; }

		[Export ("type", ArgumentSemantic.Copy)]
		string Type { get; }

		[Export ("name", ArgumentSemantic.Copy)]
		string Name { get; }

		[Export ("addresses", ArgumentSemantic.Copy)]
		NSData [] Addresses { get; }

		[Export ("port")]
		nint Port { get; }

		[Export ("publish")]
		void Publish ();

		[Export ("publishWithOptions:")]
		void Publish (NSNetServiceOptions options);

		[Export ("resolve")]
		[Deprecated (PlatformName.iOS, 2, 0, message : "Use 'Resolve (double)' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 4, message : "Use 'Resolve (double)' instead.")]
		[NoWatch]
		void Resolve ();

		[Export ("resolveWithTimeout:")]
		void Resolve (double timeOut);

		[Export ("stop")]
		void Stop ();

		[Static, Export ("dictionaryFromTXTRecordData:")]
		NSDictionary DictionaryFromTxtRecord (NSData data);
		
		[Static, Export ("dataFromTXTRecordDictionary:")]
		NSData DataFromTxtRecord (NSDictionary dictionary);
		
		[Export ("hostName", ArgumentSemantic.Copy)]
		string HostName { get; }

		[Export ("getInputStream:outputStream:")]
		bool GetStreams (out NSInputStream inputStream, out NSOutputStream outputStream);
		
		[Export ("TXTRecordData")]
		NSData GetTxtRecordData ();

		[Export ("setTXTRecordData:")]
		bool SetTxtRecordData (NSData data);

		//NSData TxtRecordData { get; set; }

		[Export ("startMonitoring")]
		void StartMonitoring ();

		[Export ("stopMonitoring")]
		void StopMonitoring ();

		[iOS (7,0), Mac (10,10)]
		[Export ("includesPeerToPeer")]
		bool IncludesPeerToPeer { get; set; }
	}

	[NoWatch]
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

		[iOS (7,0)]
		[Export ("netService:didAcceptConnectionWithInputStream:outputStream:"), EventArgs ("NSNetServiceConnection")]
		void DidAcceptConnection (NSNetService sender, NSInputStream inputStream, NSOutputStream outputStream);
	}

	[NoWatch]
	[BaseType (typeof (NSObject),
		   Delegates=new string [] {"WeakDelegate"},
		   Events=new Type [] {typeof (NSNetServiceBrowserDelegate)})]
	interface NSNetServiceBrowser {
		[Export ("delegate", ArgumentSemantic.Assign), NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSNetServiceBrowserDelegate Delegate { get; set; }

#if XAMCORE_4_0
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

		[Wrap ("Schedule (aRunLoop, forMode.GetConstant ())")]
		void Schedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

		[Wrap ("Unschedule (aRunLoop, forMode.GetConstant ())")]
		void Unschedule (NSRunLoop aRunLoop, NSRunLoopMode forMode);

		[Export ("searchForBrowsableDomains")]
		void SearchForBrowsableDomains ();

		[Export ("searchForRegistrationDomains")]
		void SearchForRegistrationDomains ();

		[Export ("searchForServicesOfType:inDomain:")]
		void SearchForServices (string type, string domain);

		[Export ("stop")]
		void Stop ();

		[iOS (7,0), Mac(10,10)]
		[Export ("includesPeerToPeer")]
		bool IncludesPeerToPeer { get; set; }
	}

	[NoWatch]
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

		[Export ("notificationWithName:object:")][Static]
		NSNotification FromName (string name, [NullAllowed] NSObject obj);

		[Export ("notificationWithName:object:userInfo:")][Static]
		NSNotification FromName (string name,[NullAllowed]  NSObject obj, [NullAllowed] NSDictionary userInfo);
		
	}

	[BaseType (typeof (NSObject))]
	interface NSNotificationCenter {
		[Static][Export ("defaultCenter", ArgumentSemantic.Strong)]
		NSNotificationCenter DefaultCenter { get; }
	
		[Export ("addObserver:selector:name:object:")]
		[PostSnippet ("AddObserverToList (observer, aName, anObject);")]
		void AddObserver (NSObject observer, Selector aSelector, [NullAllowed] NSString aName, [NullAllowed] NSObject anObject);
	
		[Export ("postNotification:")]
		void PostNotification (NSNotification notification);
	
		[Export ("postNotificationName:object:")]
		void PostNotificationName (string aName, [NullAllowed] NSObject anObject);
	
		[Export ("postNotificationName:object:userInfo:")]
		void PostNotificationName (string aName, [NullAllowed] NSObject anObject, [NullAllowed] NSDictionary aUserInfo);
	
		[Export ("removeObserver:")]
		[PostSnippet ("RemoveObserversFromList (observer, null, null);")]
		void RemoveObserver (NSObject observer);
	
		[Export ("removeObserver:name:object:")]
		[PostSnippet ("RemoveObserversFromList (observer, aName, anObject);")]
		void RemoveObserver (NSObject observer, [NullAllowed] string aName, [NullAllowed] NSObject anObject);

		[Export ("addObserverForName:object:queue:usingBlock:")]
#if XAMCORE_2_0
		NSObject AddObserver ([NullAllowed] string name, [NullAllowed] NSObject obj, [NullAllowed] NSOperationQueue queue, Action<NSNotification> handler);
#else
		NSObject AddObserver ([NullAllowed] string name, [NullAllowed] NSObject obj, [NullAllowed] NSOperationQueue queue, NSNotificationHandler handler);
#endif
	}

#if MONOMAC
	[Mac (10, 10)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSDistributedLock
	{
		[Static]
		[Export ("lockWithPath:")]
		[return: NullAllowed]
		NSDistributedLock FromPath (string path);

		[Export ("initWithPath:")]
		[DesignatedInitializer]
		IntPtr Constructor (string path);

		[Export ("tryLock")]
		bool TryLock ();

		[Export ("unlock")]
		void Unlock ();

		[Export ("breakLock")]
		void BreakLock ();

		[Export ("lockDate", ArgumentSemantic.Copy)]
		NSDate LockDate { get; }
	}

	[BaseType (typeof (NSNotificationCenter))]
	interface NSDistributedNotificationCenter {
		[Static]
		[Export ("defaultCenter")]
		NSObject DefaultCenter { get; }

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
		NSString NSLocalNotificationCenterType {get;}
	}
#endif
	
	[BaseType (typeof (NSObject))]
	interface NSNotificationQueue {
		[Static][IsThreadStatic]
		[Export ("defaultQueue", ArgumentSemantic.Strong)]
		NSNotificationQueue DefaultQueue { get; }

		[DesignatedInitializer]
		[Export ("initWithNotificationCenter:")]
		IntPtr Constructor (NSNotificationCenter notificationCenter);

		[Export ("enqueueNotification:postingStyle:")]
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle);

		[Export ("enqueueNotification:postingStyle:coalesceMask:forModes:")]
#if !XAMCORE_4_0
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle, NSNotificationCoalescing coalesceMask, string [] modes);
#else
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle, NSNotificationCoalescing coalesceMask, NSString [] modes);

		[Wrap ("EnqueueNotification (notification, postingStyle, coalesceMask, modes.GetConstants ())")]
		void EnqueueNotification (NSNotification notification, NSPostingStyle postingStyle, NSNotificationCoalescing coalesceMask, NSRunLoopMode [] modes);
#endif

		[Export ("dequeueNotificationsMatching:coalesceMask:")]
		void DequeueNotificationsMatchingcoalesceMask (NSNotification notification, NSNotificationCoalescing coalesceMask);
	}

#if !XAMCORE_2_0
	delegate void NSNotificationHandler (NSNotification notification);
#endif

	[BaseType (typeof (NSObject))]
	// init returns NIL
	[DisableDefaultCtor]
	partial interface NSValue : NSSecureCoding, NSCopying {
		[Deprecated (PlatformName.MacOSX, 10, 13, message:"Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
		[Deprecated (PlatformName.iOS, 11, 0, message:"Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message:"Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message:"Potential for buffer overruns. Use 'StoreValueAtAddress (IntPtr, nuint)' instead.")]
		[Export ("getValue:")]
		void StoreValueAtAddress (IntPtr value);

		[Export ("objCType")][Internal]
		IntPtr ObjCTypePtr ();
		
		//[Export ("initWithBytes:objCType:")][Internal]
		//NSValue InitFromBytes (IntPtr byte_ptr, IntPtr char_ptr_type);
		//[Export ("valueWithBytes:objCType:")][Static][Internal]
		//+ (NSValue *)valueWithBytes:(const void *)value objCType:(const char *)type;
		//+ (NSValue *)value:(const void *)value withObjCType:(const char *)type;

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
		
		[Export ("valueWithRange:")][Static]
		NSValue FromRange(NSRange range);

		[Export ("rangeValue")]
		NSRange RangeValue { get; }

		[Mac (10, 7)]
		[NoWatch]
		[Static, Export ("valueWithCMTime:")]
		NSValue FromCMTime (CMTime time);

		[Mac (10, 7)]
		[NoWatch]
		[Export ("CMTimeValue")]
		CMTime CMTimeValue { get; }

		[Mac (10, 7)]
		[NoWatch]
		[Static, Export ("valueWithCMTimeMapping:")]
		NSValue FromCMTimeMapping (CMTimeMapping timeMapping);

		[Mac (10, 7)]
		[NoWatch]
		[Export ("CMTimeMappingValue")]
		CMTimeMapping CMTimeMappingValue { get; }

		[Mac (10, 7)]
		[NoWatch]
		[Static, Export ("valueWithCMTimeRange:")]
		NSValue FromCMTimeRange (CMTimeRange timeRange);

		[Mac (10, 7)]
		[NoWatch]
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

#if MONOMAC
		[Export ("rectValue")]
#else
		[Export ("CGRectValue")]
#endif
		CGRect CGRectValue { get; }

#if MONOMAC
		[Export ("sizeValue")]
#else
		[Export ("CGSizeValue")]
#endif
		CGSize CGSizeValue { get; }

#if MONOMAC
		[Export ("pointValue")]
#else
		[Export ("CGPointValue")]
#endif
		CGPoint CGPointValue { get; }

		[NoMac]
		[Export ("CGAffineTransformValue")]
		CoreGraphics.CGAffineTransform CGAffineTransformValue { get; }
		
		[NoMac]
		[Export ("UIEdgeInsetsValue")]
		UIEdgeInsets UIEdgeInsetsValue { get; }

		[NoMac]
		[Watch (4,0), TV (11,0), iOS (11,0)]
		[Export ("directionalEdgeInsetsValue")]
		NSDirectionalEdgeInsets DirectionalEdgeInsetsValue { get; }

		[NoMac]
		[Export ("valueWithCGAffineTransform:")][Static]
		NSValue FromCGAffineTransform (CoreGraphics.CGAffineTransform tran);

		[NoMac]
		[Export ("valueWithUIEdgeInsets:")][Static]
		NSValue FromUIEdgeInsets (UIEdgeInsets insets);

		[Watch (4,0), TV (11,0), iOS (11,0)]
		[NoMac]
		[Static]
		[Export ("valueWithDirectionalEdgeInsets:")]
		NSValue FromDirectionalEdgeInsets (NSDirectionalEdgeInsets insets);

		[Export ("valueWithUIOffset:")][Static]
		[NoMac]
		NSValue FromUIOffset (UIOffset insets);

		[Export ("UIOffsetValue")]
		[NoMac]
		UIOffset UIOffsetValue { get; }
		// from UIGeometry.h - those are in iOS8 only (even if the header is silent about them)
		// and not in OSX 10.10

		[iOS (8,0)]
		[Export ("CGVectorValue")]
		[NoMac]
		CGVector CGVectorValue { get; }

		[iOS (8,0)]
		[Static, Export ("valueWithCGVector:")]
		[NoMac]
		NSValue FromCGVector (CGVector vector);

		// Maybe we should include this inside mapkit.cs instead (it's a partial interface, so that's trivial)?
		[TV (9,2)]
		[iOS (6,0)]
		[Mac (10,9, onlyOn64 : true)] // The header doesn't say, but the rest of the API from the same file (MKGeometry.h) was introduced in 10.9
		[Static, Export ("valueWithMKCoordinate:")]
		NSValue FromMKCoordinate (CoreLocation.CLLocationCoordinate2D coordinate);

		[TV (9,2)]
		[iOS (6,0)]
		[Mac (10,9, onlyOn64 : true)] // The header doesn't say, but the rest of the API from the same file (MKGeometry.h) was introduced in 10.9
		[Static, Export ("valueWithMKCoordinateSpan:")]
		NSValue FromMKCoordinateSpan (MapKit.MKCoordinateSpan coordinateSpan);

		[TV (9,2)]
		[iOS (6,0)]
		[Mac (10, 9, onlyOn64 : true)]
		[Export ("MKCoordinateValue")]
		CoreLocation.CLLocationCoordinate2D CoordinateValue { get; }

		[TV (9,2)]
		[iOS (6,0)]
		[Mac (10, 9, onlyOn64 : true)]
		[Export ("MKCoordinateSpanValue")]
		MapKit.MKCoordinateSpan CoordinateSpanValue { get; }

#if !WATCH
		[Export ("valueWithCATransform3D:")][Static]
		NSValue FromCATransform3D (CoreAnimation.CATransform3D transform);

		[Export ("CATransform3DValue")]
		CoreAnimation.CATransform3D CATransform3DValue { get; }
#endif

		#region SceneKit Additions

		[iOS (8,0)][Mac (10,9, onlyOn64 : true)]
		[Static, Export ("valueWithSCNVector3:")]
		NSValue FromVector (SCNVector3 vector);

		[iOS (8,0)][Mac (10,9, onlyOn64 : true)]
		[Export ("SCNVector3Value")]
		SCNVector3 Vector3Value { get; }

		[iOS (8,0)][Mac (10,9, onlyOn64 : true)]
		[Static, Export ("valueWithSCNVector4:")]
		NSValue FromVector (SCNVector4 vector);

		[iOS (8,0)][Mac (10,9, onlyOn64 : true)]
		[Export ("SCNVector4Value")]
		SCNVector4 Vector4Value { get; }

		[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
		[Static, Export ("valueWithSCNMatrix4:")]
		NSValue FromSCNMatrix4 (SCNMatrix4 matrix);

		[iOS (8,0)][Mac (10,10, onlyOn64 : true)]
		[Export ("SCNMatrix4Value")]
		SCNMatrix4 SCNMatrix4Value { get; }

		#endregion
	}

	[BaseType (typeof (NSObject))]
#if !MONOMAC || !XAMCORE_4_0
	// there were some, partial bindings in foundation-desktop.cs which did not define it as abstract for XM :(
	[Abstract] // Apple docs: NSValueTransformer is an abstract class...
#endif
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
		string[] ValueTransformerNames { get; }

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

#if IOS && !XAMCORE_4_0
		[iOS (9, 3)]
		[Notification]
		[Obsolete ("Use 'NSUserDefaults.SizeLimitExceededNotification' instead.")]
		[Field ("NSUserDefaultsSizeLimitExceededNotification")]
		NSString SizeLimitExceededNotification { get; }

		[iOS (9, 3)]
		[Notification]
		[Obsolete ("Use 'NSUserDefaults.DidChangeAccountsNotification' instead.")]
		[Field ("NSUbiquitousUserDefaultsDidChangeAccountsNotification")]
		NSString DidChangeAccountsNotification { get; }

		[iOS (9, 3)]
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
		[Field ("NSUnarchiveFromDataTransformerName")]
		NSString UnarchiveFromDataTransformerName { get; }

		[Deprecated (PlatformName.TvOS, 12, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.WatchOS, 5, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.iOS, 12, 0, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 14, message: "Use 'SecureUnarchiveFromDataTransformerName' instead.")]
		[Field ("NSKeyedUnarchiveFromDataTransformerName")]
		NSString KeyedUnarchiveFromDataTransformerName { get; }

		[Watch (5, 0), TV (12, 0), Mac (10, 14, onlyOn64: true), iOS (12, 0)]
		[Field ("NSSecureUnarchiveFromDataTransformerName")]
		NSString SecureUnarchiveFromDataTransformerName { get; }
	}

	// Class [] return value is currently broken - https://github.com/xamarin/xamarin-macios/issues/4441
// 	[Watch (5,0), TV (12,0), Mac (10,14, onlyOn64: true), iOS (12,0)]
// 	[BaseType (typeof(NSValueTransformer))]
// 	interface NSSecureUnarchiveFromDataTransformer {
// 		[Static]
// 		[Export ("allowedTopLevelClasses", ArgumentSemantic.Copy)]
// 		Class[] AllowedTopLevelClasses { get; }
// 
// 		[Static]
// 		[Wrap ("Array.ConvertAll (AllowedTopLevelClasses, c => Class.Lookup (c))")]
// 		Type [] AllowedTopLevelTypes { get; }
// 	}
	
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
#if XAMCORE_2_0
		nint NIntValue { get; }
#else
		nint IntValue { get; }
#endif

		[Export ("unsignedIntegerValue")]
#if XAMCORE_2_0
		nuint NUIntValue { get; }
#else
		nuint UnsignedIntegerValue { get; }
#endif

		[Export ("stringValue")]
		string StringValue { get; }

		[Export ("compare:")]
		nint Compare (NSNumber otherNumber);
	
#if XAMCORE_2_0
		[Internal] // Equals(object) or IEquatable<T>'s Equals(NSNumber)
#endif
		[Export ("isEqualToNumber:")]
		bool IsEqualToNumber (NSNumber number);
	
		[Export ("descriptionWithLocale:")]
		string DescriptionWithLocale (NSLocale locale);

		[DesignatedInitializer]
		[Export ("initWithChar:")]
		IntPtr Constructor (sbyte value);
	
		[DesignatedInitializer]
		[Export ("initWithUnsignedChar:")]
		IntPtr Constructor (byte value);
	
		[DesignatedInitializer]
		[Export ("initWithShort:")]
		IntPtr Constructor (short value);
	
		[DesignatedInitializer]
		[Export ("initWithUnsignedShort:")]
		IntPtr Constructor (ushort value);
	
		[DesignatedInitializer]
		[Export ("initWithInt:")]
		IntPtr Constructor (int /* int, not NSInteger */ value);
	
		[DesignatedInitializer]
		[Export ("initWithUnsignedInt:")]
		IntPtr Constructor (uint /* unsigned int, not NSUInteger */value);
	
		//[Export ("initWithLong:")]
		//IntPtr Constructor (long value);
		//
		//[Export ("initWithUnsignedLong:")]
		//IntPtr Constructor (ulong value);
	
		[DesignatedInitializer]
		[Export ("initWithLongLong:")]
		IntPtr Constructor (long value);
	
		[DesignatedInitializer]
		[Export ("initWithUnsignedLongLong:")]
		IntPtr Constructor (ulong value);
	
		[DesignatedInitializer]
		[Export ("initWithFloat:")]
		IntPtr Constructor (float /* float, not CGFloat */ value);
	
		[DesignatedInitializer]
		[Export ("initWithDouble:")]
		IntPtr Constructor (double value);
	
		[DesignatedInitializer]
		[Export ("initWithBool:")]
		IntPtr Constructor (bool value);

#if XAMCORE_2_0
		[DesignatedInitializer]
		[Export ("initWithInteger:")]
		IntPtr Constructor (nint value);

		[DesignatedInitializer]
		[Export ("initWithUnsignedInteger:")]
		IntPtr Constructor (nuint value);
#endif
	
		[Export ("numberWithChar:")][Static]
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

#if !XAMCORE_2_0
		[Internal]
#endif
		[Static]
		[Export ("numberWithInteger:")]
		NSNumber FromNInt (nint value);

#if !XAMCORE_2_0
		[Internal]
#endif
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
		bool Lenient { [Bind ("isLenient")]get; set; }

		[Export ("usesSignificantDigits")]
		bool UsesSignificantDigits { get; set; }

		[Export ("minimumSignificantDigits")]
		nuint MinimumSignificantDigits { get; set; }

		[Export ("maximumSignificantDigits")]
		nuint MaximumSignificantDigits { get; set; }

		[Export ("partialStringValidationEnabled")]
		bool PartialStringValidationEnabled { [Bind ("isPartialStringValidationEnabled")]get; set; }
	}

	[BaseType (typeof (NSNumber))]
	interface NSDecimalNumber : NSSecureCoding {
		[Export ("initWithMantissa:exponent:isNegative:")]
		IntPtr Constructor (long mantissa, short exponent, bool isNegative);
		
		[DesignatedInitializer]
		[Export ("initWithDecimal:")]
		IntPtr Constructor (NSDecimal dec);

		[Export ("initWithString:")]
		IntPtr Constructor (string numberValue);

		[Export ("initWithString:locale:")]
		IntPtr Constructor (string numberValue, NSObject locale);

		[Export ("descriptionWithLocale:")]
		[Override]
		string DescriptionWithLocale (NSLocale locale);

		[Export ("decimalValue")]
		NSDecimal NSDecimalValue { get; }

		[Export ("zero", ArgumentSemantic.Copy)][Static]
		NSDecimalNumber Zero { get; }

		[Export ("one", ArgumentSemantic.Copy)][Static]
		NSDecimalNumber One { get; }
		
		[Export ("minimumDecimalNumber", ArgumentSemantic.Copy)][Static]
		NSDecimalNumber MinValue { get; }
		
		[Export ("maximumDecimalNumber", ArgumentSemantic.Copy)][Static]
		NSDecimalNumber MaxValue { get; }

		[Export ("notANumber", ArgumentSemantic.Copy)][Static]
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
		NSDecimalNumber RaiseTo (nuint power, NSObject Behavior);
		
		[Export ("decimalNumberByMultiplyingByPowerOf10:")]
		NSDecimalNumber MultiplyPowerOf10 (short power);

		[Export ("decimalNumberByMultiplyingByPowerOf10:withBehavior:")]
		NSDecimalNumber MultiplyPowerOf10 (short power, NSObject Behavior);

		[Export ("decimalNumberByRoundingAccordingToBehavior:")]
		NSDecimalNumber Rounding (NSObject behavior);

		[Export ("compare:")]
		[Override]
		nint Compare (NSNumber other);

		[Export ("defaultBehavior", ArgumentSemantic.Strong)][Static]
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

		[Mac (10,10), iOS (8,0)]
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

		[Wrap ("ScheduleInRunLoop (runLoop, runLoopMode.GetConstant ())")]
		void ScheduleInRunLoop (NSRunLoop runLoop, NSRunLoopMode runLoopMode);

		[Export ("removeFromRunLoop:forMode:")]
		void RemoveFromRunLoop (NSRunLoop runLoop, NSString runLoopMode);

		[Wrap ("RemoveFromRunLoop (runLoop, runLoopMode.GetConstant ())")]
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
		[Export ("handlePortMessage:")]
		void MessageReceived (NSPortMessage message);
	}

	[BaseType (typeof (NSObject))]
	interface NSPortMessage {
#if MONOMAC
		[DesignatedInitializer]
		[Export ("initWithSendPort:receivePort:components:")]
		IntPtr Constructor (NSPort sendPort, NSPort recvPort, NSArray components);

		[Export ("components")]
		NSArray Components { get; }

		// Apple started refusing applications that use those selectors (desk #63237)
		// The situation is a bit confusing since NSPortMessage.h is not part of iOS SDK - 
		// but the type is used (from NSPort[Delegate]) but not _itself_ documented
		// The selectors Apple *currently* dislike are removed from the iOS build

		[Export ("sendBeforeDate:")]
		bool SendBefore (NSDate date);

		[Export ("receivePort")]
		NSPort ReceivePort { get; }

		[Export ("sendPort")]
		NSPort SendPort { get; }

		[Export ("msgid")]
		uint MsgId { get; set; } /* uint32_t */
#endif
	}

	[BaseType (typeof (NSPort))]
	interface NSMachPort {
		[DesignatedInitializer]
		[Export ("initWithMachPort:")]
		IntPtr Constructor (uint /* uint32_t */ machPort);

		[DesignatedInitializer]
		[Export ("initWithMachPort:options:")]
		IntPtr Constructor (uint /* uint32_t */ machPort, NSMachPortRights options);
		
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
		[Export ("processInfo", ArgumentSemantic.Strong)][Static]
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

		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'OperatingSystemVersion' or 'IsOperatingSystemAtLeastVersion' instead.")]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'OperatingSystemVersion' or 'IsOperatingSystemAtLeastVersion' instead.")]
		[Export ("operatingSystem")]
		nint OperatingSystem { get; }

		[Deprecated (PlatformName.MacOSX, 10, 10, message : "Use 'OperatingSystemVersionString' instead.")]
		[Deprecated (PlatformName.iOS, 8, 0, message : "Use 'OperatingSystemVersionString' instead.")]
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

		[iOS (7,0), Mac (10, 9)]
		[Export ("beginActivityWithOptions:reason:")]
		NSObject BeginActivity (NSActivityOptions options, string reason);

		[iOS (7,0), Mac (10, 9)]
		[Export ("endActivity:")]
		void EndActivity (NSObject activity);

		[iOS (7,0), Mac (10, 9)]
		[Export ("performActivityWithOptions:reason:usingBlock:")]
		void PerformActivity (NSActivityOptions options, string reason, Action runCode);

		[Mac (10,10)]
		[iOS (8,0)]
		[Export ("isOperatingSystemAtLeastVersion:")]
		bool IsOperatingSystemAtLeastVersion (NSOperatingSystemVersion version);

		[Mac (10,10)]
		[iOS (8,0)]
		[Export ("operatingSystemVersion")]
		NSOperatingSystemVersion OperatingSystemVersion { get; }
		
#if MONOMAC
		[Export ("enableSuddenTermination")]
		void EnableSuddenTermination  ();

		[Export ("disableSuddenTermination")]
		void DisableSuddenTermination ();

		[Export ("enableAutomaticTermination:")]
		void EnableAutomaticTermination (string reason);

		[Export ("disableAutomaticTermination:")]
		void DisableAutomaticTermination (string reason);

		[Export ("automaticTerminationSupportEnabled")]
		bool AutomaticTerminationSupportEnabled { get; set; }
#else
		[iOS (8,2)]
		[Export ("performExpiringActivityWithReason:usingBlock:")]
		void PerformExpiringActivity (string reason, Action<bool> block);

		[iOS (9,0)]
		[Export ("lowPowerModeEnabled")]
		bool LowPowerModeEnabled { [Bind ("isLowPowerModeEnabled")] get; }

		[iOS (9,0)]
		[Notification]
		[Field ("NSProcessInfoPowerStateDidChangeNotification")]
		NSString PowerStateDidChangeNotification { get; }
#endif

		[Mac (10,10,3)]
		[Watch (4,0)]
		[TV (11, 0)]
		[iOS (11,0)]
		[Export ("thermalState")]
		NSProcessInfoThermalState ThermalState { get; }

		[Mac (10,10,3)]
		[Field ("NSProcessInfoThermalStateDidChangeNotification")]
		[Watch (4,0)]
		[TV (11, 0)]
		[iOS (11,0)]
		[Notification]
		NSString ThermalStateDidChangeNotification { get; }
	}

	[NoWatch][NoTV][NoiOS]
	[Mac (10,12)]
	[Category]
	[BaseType (typeof (NSProcessInfo))]
	interface NSProcessInfo_NSUserInformation {
		[Export ("userName")]
		string GetUserName ();

		[Export ("fullUserName")]
		string GetFullUserName ();
	}

	[iOS (7,0), Mac (10, 9)]
	[BaseType (typeof (NSObject))]
	partial interface NSProgress {
	
		[Static, Export ("currentProgress")]
		NSProgress CurrentProgress { get; }
	
		[Static, Export ("progressWithTotalUnitCount:")]
		NSProgress FromTotalUnitCount (long unitCount);

		[iOS (9,0), Mac (10,11)]
		[Static, Export ("discreteProgressWithTotalUnitCount:")]
		NSProgress GetDiscreteProgress (long unitCount);

		[iOS (9,0), Mac (10,11)]
		[Static, Export ("progressWithTotalUnitCount:parent:pendingUnitCount:")]
		NSProgress FromTotalUnitCount (long unitCount, NSProgress parent, long portionOfParentTotalUnitCount);
	
		[DesignatedInitializer]
		[Export ("initWithParent:userInfo:")]
		IntPtr Constructor ([NullAllowed] NSProgress parentProgress, [NullAllowed] NSDictionary userInfo);
	
		[Export ("becomeCurrentWithPendingUnitCount:")]
		void BecomeCurrent (long pendingUnitCount);
	
		[Export ("resignCurrent")]
		void ResignCurrent ();
	
		[iOS (9,0), Mac (10,11)]
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

		[iOS (9,0), Mac (10,11)]
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

		[iOS (9,0), Mac (10,11)]
		[Export ("resume")]
		void Resume ();
	
		[Export ("userInfo")]
		NSDictionary UserInfo { get; }
	
		[NullAllowed] // by default this property is null
		[Export ("kind", ArgumentSemantic.Copy)]
		NSString Kind { get; set; }

#if MONOMAC
		[Export ("publish")]
		void Publish ();
	
		[Export ("unpublish")]
		void Unpublish ();
	
		[Export ("setAcknowledgementHandler:forAppBundleIdentifier:")]
		void SetAcknowledgementHandler (Action<bool> acknowledgementHandler, string appBundleIdentifier);
	
		[Static, Export ("addSubscriberForFileURL:withPublishingHandler:")]
		NSObject AddSubscriberForFile (NSUrl url, Action<NSProgress> publishingHandler);
	
		[Static, Export ("removeSubscriber:")]
		void RemoveSubscriber (NSObject subscriber);
	
		[Export ("acknowledgeWithSuccess:")]
		void AcknowledgeWithSuccess (bool success);
	
		[Export ("old")]
		bool Old { [Bind ("isOld")] get; }
#endif
		[iOS (7,0), Field ("NSProgressKindFile")]
		NSString KindFile { get; }
	
		[iOS (7,0), Field ("NSProgressEstimatedTimeRemainingKey")]
		NSString EstimatedTimeRemainingKey { get; }
	
		[iOS (7,0), Field ("NSProgressThroughputKey")]
		NSString ThroughputKey { get; }
	
		[iOS (7,0), Field ("NSProgressFileOperationKindKey")]
		NSString FileOperationKindKey { get; }
	
		[iOS (7,0), Field ("NSProgressFileOperationKindDownloading")]
		NSString FileOperationKindDownloading { get; }
	
		[iOS (7,0), Field ("NSProgressFileOperationKindDecompressingAfterDownloading")]
		NSString FileOperationKindDecompressingAfterDownloading { get; }
	
		[iOS (7,0), Field ("NSProgressFileOperationKindReceiving")]
		NSString FileOperationKindReceiving { get; }
	
		[iOS (7,0), Field ("NSProgressFileOperationKindCopying")]
		NSString FileOperationKindCopying { get; }
	
		[iOS (7,0), Field ("NSProgressFileURLKey")]
		NSString FileURLKey { get; }
	
		[iOS (7,0), Field ("NSProgressFileTotalCountKey")]
		NSString FileTotalCountKey { get; }
	
		[iOS (7,0), Field ("NSProgressFileCompletedCountKey")]
		NSString FileCompletedCountKey { get; }

#if MONOMAC
		[Field ("NSProgressFileAnimationImageKey")]
		NSString FileAnimationImageKey { get; }
	
		[Field ("NSProgressFileAnimationImageOriginalRectKey")]
		NSString FileAnimationImageOriginalRectKey { get; }
	
		[Field ("NSProgressFileIconKey")]
		NSString FileIconKey { get; }
#endif

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Async, Export ("performAsCurrentWithPendingUnitCount:usingBlock:")]
		void PerformAsCurrent (long unitCount, Action work);

		[Export ("finished")]
		bool Finished { [Bind ("isFinished")] get; }

		[Internal]
		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("estimatedTimeRemaining", ArgumentSemantic.Copy)]
		//[BindAs (typeof (nint?))]
		NSNumber _EstimatedTimeRemaining { get; set; }

		[Internal]
		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("throughput", ArgumentSemantic.Copy)]
		//[BindAs (typeof (nint?))]
		NSNumber _Throughput { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("fileOperationKind")]
		string FileOperationKind { get; set; }

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("fileURL", ArgumentSemantic.Copy)]
		NSUrl FileUrl { get; set; }

		[Internal]
		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("fileTotalCount", ArgumentSemantic.Copy)]
		//[BindAs (typeof (nint?))]
		NSNumber _FileTotalCount { get; set; }

		[Internal]
		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[NullAllowed, Export ("fileCompletedCount", ArgumentSemantic.Copy)]
		//[BindAs (typeof (nint?))]
		NSNumber _FileCompletedCount { get; set; }
	}

	interface INSProgressReporting {}

	[iOS (9,0)][Mac (10,11)]
	[Protocol]
	interface NSProgressReporting {
#if XAMCORE_4_0
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

#if !XAMCORE_2_0
	delegate void NSFileCoordinatorWorker (NSUrl newUrl);
#endif
	delegate void NSFileCoordinatorWorkerRW (NSUrl newReadingUrl, NSUrl newWritingUrl);

	interface INSFilePresenter {}

	[BaseType (typeof (NSObject))]
	interface NSFileCoordinator {
		[Static, Export ("addFilePresenter:")][PostGet ("FilePresenters")]
		void AddFilePresenter ([Protocolize] NSFilePresenter filePresenter);

		[Static]
		[Export ("removeFilePresenter:")][PostGet ("FilePresenters")]
		void RemoveFilePresenter ([Protocolize] NSFilePresenter filePresenter);

		[Static]
		[Export ("filePresenters", ArgumentSemantic.Copy)]
		[Protocolize]
		NSFilePresenter [] FilePresenters { get; }

		[DesignatedInitializer]
		[Export ("initWithFilePresenter:")]
		IntPtr Constructor ([NullAllowed] INSFilePresenter filePresenterOrNil);

#if !XAMCORE_4_0
		[Obsolete ("Use '.ctor(INSFilePresenter)' instead.")]
		[Wrap ("this ((INSFilePresenter) filePresenterOrNil)")]
		IntPtr Constructor ([NullAllowed] NSFilePresenter filePresenterOrNil);
#endif

		[Export ("coordinateReadingItemAtURL:options:error:byAccessor:")]
#if XAMCORE_2_0
		void CoordinateRead (NSUrl itemUrl, NSFileCoordinatorReadingOptions options, out NSError error, /* non null */ Action<NSUrl> worker);
#else
		void CoordinateRead (NSUrl itemUrl, NSFileCoordinatorReadingOptions options, out NSError error, /* non null */ NSFileCoordinatorWorker worker);
#endif

		[Export ("coordinateWritingItemAtURL:options:error:byAccessor:")]
#if XAMCORE_2_0
		void CoordinateWrite (NSUrl url, NSFileCoordinatorWritingOptions options, out NSError error, /* non null */ Action<NSUrl> worker);
#else
		void CoordinateWrite (NSUrl url, NSFileCoordinatorWritingOptions options, out NSError error, /* non null */ NSFileCoordinatorWorker worker);
#endif

		[Export ("coordinateReadingItemAtURL:options:writingItemAtURL:options:error:byAccessor:")]
		void CoordinateReadWrite (NSUrl readingURL, NSFileCoordinatorReadingOptions readingOptions, NSUrl writingURL, NSFileCoordinatorWritingOptions writingOptions, out NSError error, /* non null */ NSFileCoordinatorWorkerRW readWriteWorker);
		
		[Export ("coordinateWritingItemAtURL:options:writingItemAtURL:options:error:byAccessor:")]
		void CoordinateWriteWrite (NSUrl writingURL, NSFileCoordinatorWritingOptions writingOptions, NSUrl writingURL2, NSFileCoordinatorWritingOptions writingOptions2, out NSError error, /* non null */ NSFileCoordinatorWorkerRW writeWriteWorker);

		[Export ("prepareForReadingItemsAtURLs:options:writingItemsAtURLs:options:error:byAccessor:")]
		void CoordinateBatc (NSUrl [] readingURLs, NSFileCoordinatorReadingOptions readingOptions, NSUrl [] writingURLs, NSFileCoordinatorWritingOptions writingOptions, out NSError error, /* non null */ Action batchHandler);

		[iOS (8,0)][Mac (10,10)]
		[Export ("coordinateAccessWithIntents:queue:byAccessor:")]
		void CoordinateAccess (NSFileAccessIntent [] intents, NSOperationQueue executionQueue, Action<NSError> accessor);

		[Export ("itemAtURL:didMoveToURL:")]
		void ItemMoved (NSUrl fromUrl, NSUrl toUrl);

		[Export ("cancel")]
		void Cancel ();

		[iOS (6,0)]
		[Mac (10, 8)]
		[Export ("itemAtURL:willMoveToURL:")]
		void WillMove (NSUrl oldUrl, NSUrl newUrl);

		[Mac (10,7)]
		[Export ("purposeIdentifier")]
		string PurposeIdentifier { get; set; }

		[NoWatch, NoTV, Mac (10,13), iOS (11,0)]
		[Export ("itemAtURL:didChangeUbiquityAttributes:")]
		void ItemUbiquityAttributesChanged (NSUrl url, NSSet<NSString> attributes);
	}

	[iOS (8,0)][Mac (10,10)]
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
		[Field("NSFileType")]
		NSString NSFileType { get; }

		[Field("NSFileTypeDirectory")]
		NSString TypeDirectory { get; }

		[Field("NSFileTypeRegular")]
		NSString TypeRegular { get; }

		[Field("NSFileTypeSymbolicLink")]
		NSString TypeSymbolicLink { get; }

		[Field("NSFileTypeSocket")]
		NSString TypeSocket { get; }

		[Field("NSFileTypeCharacterSpecial")]
		NSString TypeCharacterSpecial { get; }

		[Field("NSFileTypeBlockSpecial")]
		NSString TypeBlockSpecial { get; }

		[Field("NSFileTypeUnknown")]
		NSString TypeUnknown { get; }

		[Field("NSFileSize")]
		NSString Size { get; }

		[Field("NSFileModificationDate")]
		NSString ModificationDate { get; }

		[Field("NSFileReferenceCount")]
		NSString ReferenceCount { get; }

		[Field("NSFileDeviceIdentifier")]
		NSString DeviceIdentifier { get; }

		[Field("NSFileOwnerAccountName")]
		NSString OwnerAccountName { get; }

		[Field("NSFileGroupOwnerAccountName")]
		NSString GroupOwnerAccountName { get; }

		[Field("NSFilePosixPermissions")]
		NSString PosixPermissions { get; }

		[Field("NSFileSystemNumber")]
		NSString SystemNumber { get; }

		[Field("NSFileSystemFileNumber")]
		NSString SystemFileNumber { get; }

		[Field("NSFileExtensionHidden")]
		NSString ExtensionHidden { get; }

		[Field("NSFileHFSCreatorCode")]
		NSString HfsCreatorCode { get; }

		[Field("NSFileHFSTypeCode")]
		NSString HfsTypeCode { get; }

		[Field("NSFileImmutable")]
		NSString Immutable { get; }

		[Field("NSFileAppendOnly")]
		NSString AppendOnly { get; }

		[Field("NSFileCreationDate")]
		NSString CreationDate { get; }

		[Field("NSFileOwnerAccountID")]
		NSString OwnerAccountID { get; }

		[Field("NSFileGroupOwnerAccountID")]
		NSString GroupOwnerAccountID { get; }

		[Field("NSFileBusy")]
		NSString Busy { get; }

#if !MONOMAC
		[Field ("NSFileProtectionKey")]
		NSString FileProtectionKey { get; }

		[Field ("NSFileProtectionNone")]
		NSString FileProtectionNone { get; }

		[Field ("NSFileProtectionComplete")]
		NSString FileProtectionComplete { get; }

		[Field ("NSFileProtectionCompleteUnlessOpen")]
		NSString FileProtectionCompleteUnlessOpen { get; }

		[Field ("NSFileProtectionCompleteUntilFirstUserAuthentication")]
		NSString FileProtectionCompleteUntilFirstUserAuthentication  { get; }
#endif
		[Field("NSFileSystemSize")]
		NSString SystemSize { get; }

		[Field("NSFileSystemFreeSize")]
		NSString SystemFreeSize { get; }

		[Field("NSFileSystemNodes")]
		NSString SystemNodes { get; }

		[Field("NSFileSystemFreeNodes")]
		NSString SystemFreeNodes { get; }

		[Static, Export ("defaultManager", ArgumentSemantic.Strong)]
		NSFileManager DefaultManager { get; }

		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		[Wrap ("WeakDelegate")]
		[Protocolize]
		NSFileManagerDelegate Delegate { get; set; }

		[Export ("setAttributes:ofItemAtPath:error:")]
		bool SetAttributes (NSDictionary attributes, string path, out NSError error);

		[Export ("createDirectoryAtPath:withIntermediateDirectories:attributes:error:")]
		bool CreateDirectory (string path, bool createIntermediates, [NullAllowed] NSDictionary attributes, out NSError error);

		[Export ("contentsOfDirectoryAtPath:error:")]
		string[] GetDirectoryContent (string path, out NSError error);

		[Export ("subpathsOfDirectoryAtPath:error:")]
		string[] GetDirectoryContentRecursive (string path, out NSError error);

		[Export ("attributesOfItemAtPath:error:")][Internal]
		NSDictionary _GetAttributes (string path, out NSError error);

		[Export ("attributesOfFileSystemForPath:error:")][Internal]
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
		string[] ComponentsToDisplay (string path);

		[Export ("enumeratorAtPath:")]
		NSDirectoryEnumerator GetEnumerator (string path);

		[Export ("subpathsAtPath:")]
		string[] Subpaths (string path);

		[Export ("contentsAtPath:")]
		NSData Contents (string path);

		[Export ("createFileAtPath:contents:attributes:")]
		bool CreateFile (string path, NSData data, [NullAllowed] NSDictionary attr);

		[Export ("contentsOfDirectoryAtURL:includingPropertiesForKeys:options:error:")]
		NSUrl[] GetDirectoryContent (NSUrl url, [NullAllowed] NSArray properties, NSDirectoryEnumerationOptions options, out NSError error);

		[Export ("copyItemAtURL:toURL:error:")]
		bool Copy (NSUrl srcUrl, NSUrl dstUrl, out NSError error);

		[Export ("moveItemAtURL:toURL:error:")]
		bool Move (NSUrl srcUrl, NSUrl dstUrl, out NSError error);

		[Export ("linkItemAtURL:toURL:error:")]
		bool Link (NSUrl srcUrl, NSUrl dstUrl, out NSError error);

		[Export ("removeItemAtURL:error:")]
		bool Remove ([NullAllowed] NSUrl url, out NSError error);

		[Export ("enumeratorAtURL:includingPropertiesForKeys:options:errorHandler:")]
#if XAMCORE_2_0
		NSDirectoryEnumerator GetEnumerator (NSUrl url, [NullAllowed] NSString[] keys, NSDirectoryEnumerationOptions options, [NullAllowed] NSEnumerateErrorHandler handler);
#else
		NSDirectoryEnumerator GetEnumerator (NSUrl url, [NullAllowed] NSArray properties, NSDirectoryEnumerationOptions options, [NullAllowed] NSEnumerateErrorHandler handler);
#endif

		[Export ("URLForDirectory:inDomain:appropriateForURL:create:error:")]
		NSUrl GetUrl (NSSearchPathDirectory directory, NSSearchPathDomain domain, [NullAllowed] NSUrl url, bool shouldCreate, out NSError error);

		[Export ("URLsForDirectory:inDomains:")]
		NSUrl[] GetUrls (NSSearchPathDirectory directory, NSSearchPathDomain domains);

		[Export ("replaceItemAtURL:withItemAtURL:backupItemName:options:resultingItemURL:error:")]
		bool Replace (NSUrl originalItem, NSUrl newItem, [NullAllowed] string backupItemName, NSFileManagerItemReplacementOptions options, out NSUrl resultingURL, out NSError error);

		[Export ("mountedVolumeURLsIncludingResourceValuesForKeys:options:")]
		NSUrl[] GetMountedVolumes ([NullAllowed] NSArray properties, NSVolumeEnumerationOptions options);

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

		[iOS (6,0)]
		[Mac (10, 8)]
		[Export ("ubiquityIdentityToken")]
		NSObject UbiquityIdentityToken { get; }

		[iOS (6,0)]
		[Mac (10, 8)]
		[Field ("NSUbiquityIdentityDidChangeNotification")]
		[Notification]
		NSString UbiquityIdentityDidChangeNotification { get; }

		[iOS (7,0), Mac (10, 8)]
		[Export ("containerURLForSecurityApplicationGroupIdentifier:")]
		NSUrl GetContainerUrl (string securityApplicationGroupIdentifier);

		[iOS (8,0), Mac (10,10)]
		[Export ("getRelationship:ofDirectory:inDomain:toItemAtURL:error:")]
#if XAMCORE_2_0
		bool GetRelationship (out NSUrlRelationship outRelationship, NSSearchPathDirectory directory, NSSearchPathDomain domain, NSUrl toItemAtUrl, out NSError error);
#else
		bool GetRelationship (out NSURLRelationship outRelationship, NSSearchPathDirectory directory, NSSearchPathDomain domain, NSUrl toItemAtUrl, out NSError error);
#endif

		[iOS (8,0), Mac (10,10)]
		[Export ("getRelationship:ofDirectoryAtURL:toItemAtURL:error:")]
#if XAMCORE_2_0
		bool GetRelationship (out NSUrlRelationship outRelationship, NSUrl directoryURL, NSUrl otherURL, out NSError error);
#else
		bool GetRelationship (out NSURLRelationship outRelationship, NSUrl directoryURL, NSUrl otherURL, out NSError error);
#endif

#if MONOMAC
		[NoWatch][NoTV][NoiOS][Mac (10, 11)][Async]
		[Export ("unmountVolumeAtURL:options:completionHandler:")]
		void UnmountVolume (NSUrl url, NSFileManagerUnmountOptions mask, Action<NSError> completionHandler);
#endif

#if !WATCH && !TVOS
		[NoWatch, NoTV, Mac (10,13), iOS (11,0)]
		[Async, Export ("getFileProviderServicesForItemAtURL:completionHandler:")]
		void GetFileProviderServices (NSUrl url, Action<NSDictionary<NSString, NSFileProviderService>, NSError> completionHandler);
#endif
	}

	[BaseType(typeof(NSObject))]
	[Model]
	[Protocol]
	interface NSFileManagerDelegate {
		[Export("fileManager:shouldCopyItemAtPath:toPath:")]
		bool ShouldCopyItemAtPath(NSFileManager fm, NSString srcPath, NSString dstPath);

#if !MONOMAC
		[Export("fileManager:shouldCopyItemAtURL:toURL:")]
		bool ShouldCopyItemAtUrl(NSFileManager fm, NSUrl srcUrl, NSUrl dstUrl);
		
		[Export ("fileManager:shouldLinkItemAtURL:toURL:")]
		bool ShouldLinkItemAtUrl (NSFileManager fileManager, NSUrl srcUrl, NSUrl dstUrl);

		[Export ("fileManager:shouldMoveItemAtURL:toURL:")]
		bool ShouldMoveItemAtUrl (NSFileManager fileManager, NSUrl srcUrl, NSUrl dstUrl);

		[Export ("fileManager:shouldProceedAfterError:copyingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorCopyingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		[Export ("fileManager:shouldProceedAfterError:linkingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorLinkingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		[Export ("fileManager:shouldProceedAfterError:movingItemAtURL:toURL:")]
		bool ShouldProceedAfterErrorMovingItem (NSFileManager fileManager, NSError error, NSUrl srcUrl, NSUrl dstUrl);

		[Export ("fileManager:shouldRemoveItemAtURL:")]
		bool ShouldRemoveItemAtUrl (NSFileManager fileManager, NSUrl url);

		[Export ("fileManager:shouldProceedAfterError:removingItemAtURL:")]
		bool ShouldProceedAfterErrorRemovingItem (NSFileManager fileManager, NSError error, NSUrl url);
#endif

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

		[NoWatch][NoTV][NoiOS][Mac (10, 12)]
		[Export ("homeDirectoryForCurrentUser")]
		NSUrl GetHomeDirectoryForCurrentUser ();

		[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
		[Export ("temporaryDirectory")]
		NSUrl GetTemporaryDirectory ();

		[NoWatch][NoTV][NoiOS][Mac (10, 12)]
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
		NSUrl PresentedItemURL { get; }

#if XAMCORE_2_0
		[Abstract]
#endif
		[Export ("presentedItemOperationQueue", ArgumentSemantic.Retain)]
#if XAMCORE_4_0
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

		[NoWatch, NoTV, Mac (10,13), iOS (11,0)]
		[Export ("presentedItemDidChangeUbiquityAttributes:")]
		void PresentedItemChangedUbiquityAttributes (NSSet<NSString> attributes);

		[NoWatch, NoTV, Mac (10, 13), iOS (11, 0)]
		[Export ("observedPresentedItemUbiquityAttributes", ArgumentSemantic.Strong)]
		NSSet<NSString> PresentedItemObservedUbiquityAttributes { get; }
	}

	delegate void NSFileVersionNonlocalVersionsCompletionHandler ([NullAllowed] NSFileVersion[] nonlocalFileVersions, [NullAllowed] NSError error);

	[BaseType (typeof (NSObject))]
	// Objective-C exception thrown.  Name: NSGenericException Reason: -[NSFileVersion init]: You have to use one of the factory methods to instantiate NSFileVersion.
	[DisableDefaultCtor]
	interface NSFileVersion {
		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get;  }

		[Export ("localizedName", ArgumentSemantic.Copy)]
		string LocalizedName { get;  }

		[Export ("localizedNameOfSavingComputer", ArgumentSemantic.Copy)]
		string LocalizedNameOfSavingComputer { get;  }

		[Export ("modificationDate", ArgumentSemantic.Copy)]
		NSDate ModificationDate { get;  }

		[Export ("persistentIdentifier", ArgumentSemantic.Retain)]
		NSObject PersistentIdentifier { get;  }

		[Export ("conflict")]
		bool IsConflict { [Bind ("isConflict")] get;  }

		[Export ("resolved")]
		bool Resolved { [Bind ("isResolved")] get; set;  }
#if MONOMAC
		[Export ("discardable")]
		bool Discardable { [Bind ("isDiscardable")] get; set;  }
#endif

		[Mac (10,10)]
		[iOS (8,0)]
		[Export ("hasLocalContents")]
		bool HasLocalContents { get; }

		[Mac (10,10)]
		[iOS (8,0)]
		[Export ("hasThumbnail")]
		bool HasThumbnail { get; }

		[Static]
		[Export ("currentVersionOfItemAtURL:")]
		NSFileVersion GetCurrentVersion (NSUrl url);

		[Mac (10,10)]
		[iOS (8,0)]
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

#if MONOMAC
		[Static]
		[Export ("addVersionOfItemAtURL:withContentsOfURL:options:error:")]
		NSFileVersion AddVersion (NSUrl url, NSUrl contentsURL, NSFileVersionAddingOptions options, out NSError outError);

		[Static]
		[Export ("temporaryDirectoryURLForNewVersionOfItemAtURL:")]
		NSUrl TemporaryDirectoryForItem (NSUrl url);
#endif

		[Export ("replaceItemAtURL:options:error:")]
		NSUrl ReplaceItem (NSUrl url, NSFileVersionReplacingOptions options, out NSError error);

		[Export ("removeAndReturnError:")]
		bool Remove (out NSError outError);

		[Static]
		[Export ("removeOtherVersionsOfItemAtURL:error:")]
		bool RemoveOtherVersions (NSUrl url, out NSError outError);

		[NoWatch, NoTV, Mac (10, 12), iOS (10, 0)]
		[NullAllowed, Export ("originatorNameComponents", ArgumentSemantic.Copy)]
		NSPersonNameComponents OriginatorNameComponents { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSFileWrapper : NSCoding {
		[DesignatedInitializer]
		[Export ("initWithURL:options:error:")]
		IntPtr Constructor (NSUrl url, NSFileWrapperReadingOptions options, out NSError outError);

		[DesignatedInitializer]
		[Export ("initDirectoryWithFileWrappers:")]
		IntPtr Constructor (NSDictionary childrenByPreferredName);

		[DesignatedInitializer]
		[Export ("initRegularFileWithContents:")]
		IntPtr Constructor (NSData contents);

		[DesignatedInitializer]
		[Export ("initSymbolicLinkWithDestinationURL:")]
		IntPtr Constructor (NSUrl urlToSymbolicLink);

		// Constructor clash
		//[Export ("initWithSerializedRepresentation:")]
		//IntPtr Constructor (NSData serializeRepresentation);

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
		bool Write (NSUrl url, NSFileWrapperWritingOptions options, NSUrl originalContentsURL, out NSError outError);

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

#if MONOMAC
		[Export ("icon", ArgumentSemantic.Retain)]
		NSImage Icon { get; set; }
#endif
	}

	[BaseType (typeof (NSEnumerator))]
	interface NSDirectoryEnumerator {
		[Export ("fileAttributes")]
		NSDictionary FileAttributes { get; }

		[Export ("directoryAttributes")]
		NSDictionary DirectoryAttributes { get; }

		[Export ("skipDescendents")]
		void SkipDescendents ();

#if !MONOMAC
		[Export ("level")]
		nint Level { get; }
#else
#if !XAMCORE_2_0
		////- (unsigned long long)fileSize;
		//[Export ("fileSize")]
		//unsigned long long FileSize ([Target] NSDictionary fileAttributes);

		[Bind ("fileModificationDate")]
		[Obsolete ("Use 'ToFileAttributes ().ModificationDate' instead.")]
		NSDate FileModificationDate ([Target] NSDictionary fileAttributes);

		[Bind ("fileType")]
		[Obsolete ("Use 'ToFileAttributes ().Type' instead.")]
		string FileType ([Target] NSDictionary fileAttributes);

		[Bind ("filePosixPermissions")]
		[Obsolete ("Use 'ToFileAttributes ().PosixPermissions' instead.")]
		uint /* unsigned short */ FilePosixPermissions ([Target] NSDictionary fileAttributes);

		[Bind ("fileOwnerAccountName")]
		[Obsolete ("Use 'ToFileAttributes ().OwnerAccountName' instead.")]
		string FileOwnerAccountName ([Target] NSDictionary fileAttributes);

		[Bind ("fileGroupOwnerAccountName")]
		[Obsolete ("Use 'ToFileAttributes ().GroupOwnerAccountName' instead.")]
		string FileGroupOwnerAccountName ([Target] NSDictionary fileAttributes);

		[Bind ("fileSystemNumber")]
		[Obsolete ("Use 'ToFileAttributes ().SystemNumber' instead.")]
		nint FileSystemNumber ([Target] NSDictionary fileAttributes);

		[Bind ("fileSystemFileNumber")]
		[Obsolete ("Use 'ToFileAttributes ().SystemFileNumber' instead.")]
		nuint FileSystemFileNumber ([Target] NSDictionary fileAttributes);

		[Bind ("fileExtensionHidden")]
		[Obsolete ("Use 'ToFileAttributes ().ExtensionHidden' instead.")]
		bool FileExtensionHidden ([Target] NSDictionary fileAttributes);

		[Bind ("fileHFSCreatorCode")]
		[Obsolete ("Use 'ToFileAttributes ().HfsCreatorCode' instead.")]
		nuint FileHfsCreatorCode ([Target] NSDictionary fileAttributes);

		[Bind ("fileHFSTypeCode")]
		[Obsolete ("Use 'ToFileAttributes ().HfsTypeCode' instead.")]
		nuint FileHfsTypeCode ([Target] NSDictionary fileAttributes);

		[Bind ("fileIsImmutable")]
		[Obsolete ("Use 'ToFileAttributes ().IsImmutable' instead.")]
		bool FileIsImmutable ([Target] NSDictionary fileAttributes);

		[Bind ("fileIsAppendOnly")]
		[Obsolete ("Use 'ToFileAttributes ().IsAppendOnly' instead.")]
		bool FileIsAppendOnly ([Target] NSDictionary fileAttributes);

		[Bind ("fileCreationDate")]
		[Obsolete ("Use 'ToFileAttributes ().CreationDate' instead.")]
		NSDate FileCreationDate ([Target] NSDictionary fileAttributes);

		[Bind ("fileOwnerAccountID")]
		[Obsolete ("Use 'ToFileAttributes ().OwnerAccountID' instead.")]
		NSNumber FileOwnerAccountID ([Target] NSDictionary fileAttributes);

		[Bind ("fileGroupOwnerAccountID")]
		[Obsolete ("Use 'ToFileAttributes ().GroupOwnerAccountID' instead.")]
		NSNumber FileGroupOwnerAccountID ([Target] NSDictionary fileAttributes);
#endif
#endif
	}

	delegate bool NSPredicateEvaluator (NSObject evaluatedObject, NSDictionary bindings);
	
	[BaseType (typeof (NSObject))]
	// 'init' returns NIL
	[DisableDefaultCtor]
	interface NSPredicate : NSSecureCoding, NSCopying {
		[Static]
		[Internal]
		[Export ("predicateWithFormat:argumentArray:")]
		NSPredicate _FromFormat (string predicateFormat, [NullAllowed] NSObject[] arguments);

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
#if MONOMAC
		// 10.9+
		[Static]
		[Mac (10, 9)]
		[Export ("predicateFromMetadataQueryString:")]
		NSPredicate FromMetadataQueryString (string query);
#endif
		[iOS (7,0), Mac (10, 9)]
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
	
#if MONOMAC
	[BaseType (typeof (NSObject), Name="NSURLDownload")]
	interface NSUrlDownload {
		[Static, Export ("canResumeDownloadDecodedWithEncodingMIMEType:")]
		bool CanResumeDownloadDecodedWithEncodingMimeType (string mimeType);

		[Export ("initWithRequest:delegate:")]
		IntPtr Constructor (NSUrlRequest request, NSObject delegate1);

		[Export ("initWithResumeData:delegate:path:")]
		IntPtr Constructor (NSData resumeData, NSObject delegate1, string path);

		[Export ("cancel")]
		void Cancel ();

		[Export ("setDestination:allowOverwrite:")]
		void SetDestination (string path, bool allowOverwrite);

		[Export ("request")]
		NSUrlRequest Request { get; }

		[Export ("resumeData")]
		NSData ResumeData { get; }

		[Export ("deletesFileUponFailure")]
		bool DeletesFileUponFailure { get; set; }
	}

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
		void FailedWithError(NSUrlDownload download, NSError error);
	}
#endif

#if XAMCORE_2_0 && !MONOMAC
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
#else
	interface NSUrlProtocolClient {}
#endif

	interface INSUrlProtocolClient {}

	[BaseType (typeof (NSObject),
		   Name="NSURLProtocol",
		   Delegates=new string [] {"WeakClient"})]
	interface NSUrlProtocol {
		[DesignatedInitializer]
		[Export ("initWithRequest:cachedResponse:client:")]
#if XAMCORE_2_0 && !MONOMAC
		IntPtr Constructor (NSUrlRequest request, [NullAllowed] NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client);
#else
		IntPtr Constructor (NSUrlRequest request, [NullAllowed] NSCachedUrlResponse cachedResponse, NSUrlProtocolClient client);
#endif

#if XAMCORE_2_0 && !MONOMAC
		[Export ("client")]
		INSUrlProtocolClient Client { get; }
#else
		[Export ("client")]
		NSObject WeakClient { get; }
#endif

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

//		[iOS (8,0)]
//		[Export ("initWithTask:cachedResponse:client:")]
//#if XAMCORE_2_0
//		IntPtr Constructor (NSUrlSessionTask task, [NullAllowed] NSCachedUrlResponse cachedResponse, INSUrlProtocolClient client);
//#else
//		IntPtr Constructor (NSUrlSessionTask task, [NullAllowed] NSCachedUrlResponse cachedResponse, NSUrlProtocolClient client);
//#endif
//		[iOS (8,0)]
//		[Export ("task", ArgumentSemantic.Copy)]
//		NSUrlSessionTask Task { get; }
//
//		[iOS (8,0)]
//		[Static, Export ("canInitWithTask:")]
//		bool CanInitWithTask (NSUrlSessionTask task);
	}

	[BaseType (typeof(NSObject))]
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

	[iOS (8,0)][Mac (10,10, onlyOn64:true)] // Not defined in 32-bit
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
		NSTextCheckingType ResultType { get;  }

		[Export ("range")]
		NSRange Range { get;  }

		// From the NSTextCheckingResultOptional category on NSTextCheckingResult
		[Export ("orthography")]
		NSOrthography Orthography { get; }

		[Export ("grammarDetails")]
		string[] GrammarDetails { get; }

		[Export ("date")]
		NSDate Date { get; }

		[Export ("timeZone")]
		NSTimeZone TimeZone { get; }

		[Export ("duration")]
		double TimeInterval { get; }

		[Export ("components")]
		[Mac (10, 7)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSDictionary WeakComponents { get; }

		[Wrap ("WeakComponents")]
		NSTextCheckingTransitComponents Components { get; }

		[Export ("URL")]
		NSUrl Url { get; }

		[Export ("replacementString")]
		string ReplacementString { get; }

		[Export ("alternativeStrings")]
		[iOS (7, 0)]
		[Mac (10, 9)]
		string [] AlternativeStrings { get; }

//		NSRegularExpression isn't bound
//		[Export ("regularExpression")]
//		[Mac (10, 7)]
//		NSRegularExpression RegularExpression { get; }

		[Export ("phoneNumber")]
		[Mac (10, 7)]
		string PhoneNumber { get; }

		[Export ("addressComponents")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSDictionary WeakAddressComponents { get; }

		[Wrap ("WeakAddressComponents")]
		NSTextCheckingAddressComponents AddressComponents { get; }

		[Export ("numberOfRanges")]
		[Mac (10, 7)]
		nuint NumberOfRanges { get; }

		[Export ("rangeAtIndex:")]
		[Mac (10, 7)]
		NSRange RangeAtIndex (nuint idx);

		[Export ("resultByAdjustingRangesWithOffset:")]
		[Mac (10, 7)]
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
		NSTextCheckingResult GrammarCheckingResult (NSRange range, string[] details);

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
		[Wrap ("AddressCheckingResult (range, components != null ? components.Dictionary : null)")]
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
		[iOS (7, 0)]
		[Mac (10, 9)]
		NSTextCheckingResult CorrectionCheckingResult (NSRange range, string replacementString, string[] alternativeStrings);

//		NSRegularExpression isn't bound
//		[Export ("regularExpressionCheckingResultWithRanges:count:regularExpression:")]
//		[Mac (10, 7)]
//		[Internal] // FIXME
//		NSTextCheckingResult RegularExpressionCheckingResult (ref NSRange ranges, nuint count, NSRegularExpression regularExpression);

		[Static]
		[Export ("phoneNumberCheckingResultWithRange:phoneNumber:")]
		[Mac (10, 7)]
		NSTextCheckingResult PhoneNumberCheckingResult (NSRange range, string phoneNumber);

		[Static]
		[Export ("transitInformationCheckingResultWithRange:components:")]
		[Mac (10, 7)]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		NSTextCheckingResult TransitInformationCheckingResult (NSRange range, NSDictionary components);

		[Static]
		[Wrap ("TransitInformationCheckingResult (range, components != null ? components.Dictionary : null)")]
		NSTextCheckingResult TransitInformationCheckingResult (NSRange range, NSTextCheckingTransitComponents components);

		[Watch (4,0), TV (11,0), Mac (10,13), iOS (11,0)]
		[Export ("rangeWithName:")]
		NSRange GetRange (string name);

	}

	[StrongDictionary ("NSTextChecking")]
	interface NSTextCheckingTransitComponents {
		[Mac (10, 7)]
		string Airline { get; }

		[Mac (10, 7)]
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
		[Mac (10, 7)]
		NSString AirlineKey { get; }

		[Field ("NSTextCheckingFlightKey")]
		[Mac (10, 7)]
		NSString FlightKey { get; }
	}

	[BaseType (typeof(NSObject))]
	interface NSLock : NSLocking
	{
		[Export ("tryLock")]
		bool TryLock (); 

		[Export ("lockBeforeDate:")]
		bool LockBeforeDate (NSDate limit);

		[Export ("name")]
		string Name { get; [NullAllowed] set; }
	}

	[BaseType (typeof(NSObject))]
	interface NSConditionLock : NSLocking {

		[DesignatedInitializer]
		[Export ("initWithCondition:")]
		IntPtr Constructor (nint condition);

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
		string Name { get; [NullAllowed] set; }
	}

	[BaseType (typeof(NSObject))]
	interface NSRecursiveLock : NSLocking
	{
		[Export ("tryLock")]
		bool TryLock (); 

		[Export ("lockBeforeDate:")]
		bool LockBeforeDate (NSDate limit);

		[Export ("name")]
		string Name { get; [NullAllowed] set; }
	}

	[BaseType (typeof(NSObject))]
	interface NSCondition : NSLocking
	{
		[Export ("wait")]
		void Wait ();

		[Export ("waitUntilDate:")]
		bool WaitUntilDate (NSDate limit);

		[Export ("signal")]
		void Signal ();

		[Export ("broadcast")]
		void Broadcast ();

		[Export ("name")]
		string Name { get; [NullAllowed] set; }
	}

// Not yet, the IntPtr[] argument isn't handled correctly by the generator (it tries to convert to NSArray, while the native method expects a C array).
//	[Protocol]
//	interface NSFastEnumeration {
//		[Abstract]
//		[Export ("countByEnumeratingWithState:objects:count:")]
//		nuint Enumerate (ref NSFastEnumerationState state, IntPtr[] objects, nuint count);
//	}

	// Placeholer, just so we can start flagging things
	interface INSFastEnumeration {}
	
#if MONOMAC
	partial interface NSBundle {
		// - (NSImage *)imageForResource:(NSString *)name NS_AVAILABLE_MAC(10_7);
		[Mac (10, 7), Export ("imageForResource:")]
		NSImage ImageForResource (string name);
	}
#endif

	partial interface NSAttributedString {

#if MONOMAC
		[Mac (10, 7), Field ("NSTextLayoutSectionOrientation", "AppKit")]
#else
		[iOS (7,0)]
		[Field ("NSTextLayoutSectionOrientation", "UIKit")]
#endif
		NSString TextLayoutSectionOrientation { get; }

#if MONOMAC
		[Mac (10, 7), Field ("NSTextLayoutSectionRange", "AppKit")]
#else
		[iOS (7,0)]
		[Field ("NSTextLayoutSectionRange", "UIKit")]
#endif
		NSString TextLayoutSectionRange { get; }

#if MONOMAC
		[Mac (10, 7), Field ("NSTextLayoutSectionsAttribute", "AppKit")]
#else
		[iOS (7,0)]
		[Field ("NSTextLayoutSectionsAttribute", "UIKit")]
#endif
		NSString TextLayoutSectionsAttribute { get; }

		#if !XAMCORE_2_0 && MONOMAC
		[Field ("NSCharacterShapeAttributeName", "AppKit")]
		NSString CharacterShapeAttributeName { get; }

		[Field ("NSGlyphInfoAttributeName", "AppKit")]
		NSString GlyphInfoAttributeName { get; }

		[Field ("NSSpellingStateAttributeName", "AppKit")]
		NSString SpellingStateAttributeName { get; }

		[Mac (10, 8), Field ("NSTextAlternativesAttributeName", "AppKit")]
		NSString TextAlternativesAttributeName { get; }
		#endif

		[NoiOS, NoWatch, NoTV][Availability (Deprecated = Platform.Mac_10_11)]
		[Field ("NSUnderlineByWordMask", "AppKit")]
		nint UnderlineByWordMaskAttributeName { get; }
	}

	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
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
		IntPtr Constructor (NSDate startDate, double duration);

		[Export ("initWithStartDate:endDate:")]
		IntPtr Constructor (NSDate startDate, NSDate endDate);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSObject))]
	interface NSUnit : NSCopying, NSSecureCoding {
		[Export ("symbol")]
		string Symbol { get; }

		[Export ("initWithSymbol:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol);
	}

	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSObject))]
	interface NSUnitConverter {
		[Export ("baseUnitValueFromValue:")]
		double GetBaseUnitValue (double value);

		[Export ("valueFromBaseUnitValue:")]
		double GetValue (double baseUnitValue);
	}

	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSUnitConverter))]
	interface NSUnitConverterLinear : NSSecureCoding {

		[Export ("coefficient")]
		double Coefficient { get; }

		[Export ("constant")]
		double Constant { get; }

		[Export ("initWithCoefficient:")]
		IntPtr Constructor (double coefficient);

		[Export ("initWithCoefficient:constant:")]
		[DesignatedInitializer]
		IntPtr Constructor (double coefficient, double constant);
	}

	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSUnit))]
	[Abstract] // abstract subclass of NSUnit
	[DisableDefaultCtor] // there's a designated initializer
	interface NSDimension : NSSecureCoding {
		// Inlined from base type
		[Export ("initWithSymbol:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol);

		[Export ("converter", ArgumentSemantic.Copy)]
		NSUnitConverter Converter { get; }

		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

		// needs to be overriden in suubclasses
		//	NSInvalidArgumentException Reason: *** You must override baseUnit in your class NSDimension to define its base unit.
		// we provide a basic, managed, implementation that throws with a similar message
		//[Static]
		//[Export ("baseUnit")]
		//NSDimension BaseUnit { get; }
	}

	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	[DisableDefaultCtor] // base type has a designated initializer
	interface NSUnitTemperature : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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

#if !WATCH && !TVOS
	[Mac (10,8), iOS (11,0), NoWatch, NoTV]
	partial interface NSFileManager {

		[Mac (10, 8), Export ("trashItemAtURL:resultingItemURL:error:")]
		bool TrashItem (NSUrl url, out NSUrl resultingItemUrl, out NSError error);
	}

	[NoWatch, NoTV, Mac (10,13), iOS (11,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface NSFileProviderService
	{
		[Export ("name")]
		string Name { get; }
	}
#endif

#if MONOMAC
	partial interface NSFilePresenter {

		[Mac (10, 8), Export ("primaryPresentedItemURL")]
		NSUrl PrimaryPresentedItemUrl { get; }
	}

	partial interface NSAttributedString {

		[Export ("boundingRectWithSize:options:")]
		CGRect BoundingRectWithSize (CGSize size, NSStringDrawingOptions options);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	partial interface NSHost {

		[Static, Internal, Export ("currentHost")]
		NSHost _Current { get;}

		[Static, Internal, Export ("hostWithName:")]
		NSHost _FromName (string name);

		[Static, Internal, Export ("hostWithAddress:")]
		NSHost _FromAddress (string address);

		[Export ("isEqualToHost:")]
		bool Equals (NSHost host);

		[Export ("name")]
		string Name { get; }

		[Export ("localizedName")]
		string LocalizedName { get; }

		[Export ("names")]
		string [] Names { get; }

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

	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface NSScriptCommand : NSCoding {

		[Internal]
		[DesignatedInitializer]
		[Export ("initWithCommandDescription:")]
		IntPtr Constructor (NSScriptCommandDescription cmdDescription);

		[Internal]
		[Static]
		[Export ("currentCommand")]
		IntPtr GetCurrentCommand ();

		[Export ("appleEvent")]
		NSAppleEventDescriptor AppleEvent { get; }

		[Export ("executeCommand")]
		IntPtr Execute ();
		
		[Export ("evaluatedReceivers")]
		NSObject EvaluatedReceivers { get; }
	}

	[StrongDictionary ("NSScriptCommandArgumentDescriptionKeys")]
	partial interface NSScriptCommandArgumentDescription {
		string AppleEventCode { get; set; }
		string Type { get; set;}
		string Optional { get; set; }
	}

	[StrongDictionary ("NSScriptCommandDescriptionDictionaryKeys")]
	partial interface NSScriptCommandDescriptionDictionary {
		string CommandClass { get; set; } 
		string AppleEventCode { get; set; } 
		string AppleEventClassCode { get; set; }
		string Type { get; set;}
		string ResultAppleEventCode { get; set; }
		NSMutableDictionary Arguments { get; set; }
	}

	[DisableDefaultCtor]
	[BaseType (typeof (NSObject))]
	partial interface NSScriptCommandDescription : NSCoding {

		[Internal]
		[DesignatedInitializer]
		[Export ("initWithSuiteName:commandName:dictionary:")]
		IntPtr Constructor (NSString suiteName, NSString commandName, NSDictionary commandDeclaration);

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

		[Internal]
		[Export ("typeForArgumentWithName:")]
		NSString GetNSTypeForArgument (NSString name);

		[Internal]
		[Export ("appleEventCodeForReturnType")]
		int FCCAppleEventCodeForReturnType { get; }

		[Export ("returnType")]
		string ReturnType { get; }

		[Internal]
		[Export ("createCommandInstance")]
		IntPtr CreateCommandInstancePtr ();
	}

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSAffineTransform : NSSecureCoding, NSCopying {
		[Export ("initWithTransform:")]
		IntPtr Constructor (NSAffineTransform transform);

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
		
		[Export ("transformBezierPath:")]
		NSBezierPath TransformBezierPath (NSBezierPath path);

		[Export ("set")]
		void Set ();

		[Export ("concat")]
		void Concat ();

		[Export ("transformStruct")]
		CGAffineTransform TransformStruct { get; set; }
	}

	[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'NSXpcConnection' instead.")]
	[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'NSXpcConnection' instead.")]
	[Deprecated (PlatformName.WatchOS, 2, 0, message : "Use 'NSXpcConnection' instead.")]
	[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'NSXpcConnection' instead.")]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSConnection {
		[Static, Export ("connectionWithReceivePort:sendPort:")]
		NSConnection Create ([NullAllowed] NSPort receivePort, [NullAllowed] NSPort sendPort);

		[Export ("runInNewThread")]
		void RunInNewThread ();

		// enableMultipleThreads, multipleThreadsEnabled - no-op in 10.5+ (always enabled)

		[Export ("addRunLoop:")]
		void AddRunLoop (NSRunLoop runLoop);

		[Export ("removeRunLoop:")]
		void RemoveRunLoop (NSRunLoop runLoop);

		[Static, Export ("serviceConnectionWithName:rootObject:usingNameServer:")]
		NSConnection CreateService (string name, NSObject root, NSPortNameServer server);

		[Static, Export ("serviceConnectionWithName:rootObject:")]
		NSConnection CreateService (string name, NSObject root);

		[Export ("registerName:")]
		bool RegisterName (string name);

		[Export ("registerName:withNameServer:")]
		bool RegisterName (string name, NSPortNameServer server);

		[Export ("rootObject", ArgumentSemantic.Retain)]
		NSObject RootObject { get; set; }

		[Static, Export ("connectionWithRegisteredName:host:")]
		NSConnection LookupService (string name, [NullAllowed] string hostName);

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

	[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'NSXpcConnection' instead.")]
	[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'NSXpcConnection' instead.")]
	[Deprecated (PlatformName.WatchOS, 2, 0, message : "Use 'NSXpcConnection' instead.")]
	[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'NSXpcConnection' instead.")]
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

	[Deprecated (PlatformName.MacOSX, 10, 13, message : "Use 'NSXpcConnection' instead.")]
	[Deprecated (PlatformName.iOS, 11, 0, message : "Use 'NSXpcConnection' instead.")]
	[Deprecated (PlatformName.WatchOS, 2, 0, message : "Use 'NSXpcConnection' instead.")]
	[Deprecated (PlatformName.TvOS, 11, 0, message : "Use 'NSXpcConnection' instead.")]
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

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSPortNameServer {
		[Static, Export ("systemDefaultPortNameServer")]
		NSPortNameServer SystemDefault { get; }

		[Export ("portForName:")]
		NSPort GetPort (string portName);

		[Export ("portForName:host:")]
		NSPort GetPort (string portName, string hostName);

		[Export ("registerPort:name:")]
		bool RegisterPort (NSPort port, string portName);

		[Export ("removePortForName:")]
		bool RemovePort (string portName);
	}
	
	// FAK Left off until I understand how to do structs
	//struct NSAffineTransformStruct {
	//	public float M11, M12, M21, M22;
	//	public float tX, tY;
	//}

	[BaseType (typeof (NSCharacterSet))]
	interface NSMutableCharacterSet {
		[Export ("removeCharactersInRange:")]
		void RemoveCharacters (NSRange aRange);

		[Export ("addCharactersInString:")]
		void AddCharacters (string aString);

		[Export ("removeCharactersInString:")]
		void RemoveCharacters (string aString);

		[Export ("formUnionWithCharacterSet:")]
		void UnionWith (NSCharacterSet otherSet);

		[Export ("formIntersectionWithCharacterSet:")]
		void IntersectWith (NSCharacterSet otherSet);

		[Export ("invert")]
		void Invert ();

	}

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

		[Export ("paramDescriptorForKeyword:")]
		NSAppleEventDescriptor ParamDescriptorForKeyword (AEKeyword keyword);

		[Export ("removeParamDescriptorWithKeyword:")]
		void RemoveParamDescriptorWithKeyword (AEKeyword keyword);

		[Export ("setAttributeDescriptor:forKeyword:")]
		void SetAttributeDescriptorforKeyword (NSAppleEventDescriptor descriptor, AEKeyword keyword);

		[Export ("attributeDescriptorForKeyword:")]
		NSAppleEventDescriptor AttributeDescriptorForKeyword (AEKeyword keyword);

		[Export ("numberOfItems")]
		nint NumberOfItems { get; }

		[Export ("insertDescriptor:atIndex:")]
		void InsertDescriptoratIndex (NSAppleEventDescriptor descriptor, nint index);

		[Export ("descriptorAtIndex:")]
		NSAppleEventDescriptor DescriptorAtIndex (nint index);

		[Export ("removeDescriptorAtIndex:")]
		void RemoveDescriptorAtIndex (nint index);

		[Export ("setDescriptor:forKeyword:")]
		void SetDescriptorforKeyword (NSAppleEventDescriptor descriptor, AEKeyword keyword);

		[Export ("descriptorForKeyword:")]
		NSAppleEventDescriptor DescriptorForKeyword (AEKeyword keyword);

		[Export ("removeDescriptorWithKeyword:")]
		void RemoveDescriptorWithKeyword (AEKeyword keyword);

		[Export ("keywordForDescriptorAtIndex:")]
		AEKeyword KeywordForDescriptorAtIndex (nint index);

		/*[Export ("coerceToDescriptorType:")]
		NSAppleEventDescriptor CoerceToDescriptorType (DescType descriptorType);*/

		[Mac (10, 11)]
		[Static]
		[Export ("currentProcessDescriptor")]
		NSAppleEventDescriptor CurrentProcessDescriptor { get; }

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithDouble:")]
		NSAppleEventDescriptor FromDouble (double doubleValue);

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithDate:")]
		NSAppleEventDescriptor FromDate (NSDate date);

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithFileURL:")]
		NSAppleEventDescriptor FromFileURL (NSUrl fileURL);

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithProcessIdentifier:")]
		NSAppleEventDescriptor FromProcessIdentifier (int processIdentifier);

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithBundleIdentifier:")]
		NSAppleEventDescriptor FromBundleIdentifier (string bundleIdentifier);

		[Mac (10,11)]
		[Static]
		[Export ("descriptorWithApplicationURL:")]
		NSAppleEventDescriptor FromApplicationURL (NSUrl applicationURL);

		[Mac (10, 11)]
		[Export ("doubleValue")]
		double DoubleValue { get; }

		[Mac (10,11)]
		[Export ("sendEventWithOptions:timeout:error:")]
		[return: NullAllowed]
		NSAppleEventDescriptor SendEvent (NSAppleEventSendOptions sendOptions, double timeoutInSeconds, [NullAllowed] out NSError error);

		[Mac (10, 11)]
		[Export ("isRecordDescriptor")]
		bool IsRecordDescriptor { get; }

		[Mac (10, 11)]
		[NullAllowed, Export ("dateValue", ArgumentSemantic.Copy)]
		NSDate DateValue { get; }

		[Mac (10, 11)]
		[NullAllowed, Export ("fileURLValue", ArgumentSemantic.Copy)]
		NSUrl FileURLValue { get; }
	}

	[BaseType (typeof (NSObject))]
	interface NSAppleEventManager {
		[Static]
		[Export ("sharedAppleEventManager")]
		NSAppleEventManager SharedAppleEventManager { get; }

		[Export ("setEventHandler:andSelector:forEventClass:andEventID:")]
		void SetEventHandler (NSObject handler, Selector handleEventSelector, AEEventClass eventClass, AEEventID eventID);

		[Export ("removeEventHandlerForEventClass:andEventID:")]
#if XAMCORE_2_0
		void RemoveEventHandler (AEEventClass eventClass, AEEventID eventID);
#else
		[Obsolete ("Use 'RemoveEventHandler' instead.")]
		void RemoveEventHandlerForEventClassandEventID (AEEventClass eventClass, AEEventID eventID);
#endif

		[Export ("currentAppleEvent")]
		NSAppleEventDescriptor CurrentAppleEvent { get; }

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

	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSTask {
		[Export ("launch")]
		void Launch ();

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
		[Export ("launchedTaskWithLaunchPath:arguments:")]
		NSTask LaunchFromPath (string path, string[] arguments);

		//Detected properties
		[Export ("launchPath")]
		string LaunchPath { get; set; }

		[Export ("arguments")]
		string [] Arguments { get; set; }

		[Export ("environment", ArgumentSemantic.Copy)]
		NSDictionary Environment { get; set; }

		[Export ("currentDirectoryPath")]
		string CurrentDirectoryPath { get; set; }

		[Export ("standardInput", ArgumentSemantic.Retain)]
		NSObject StandardInput { get; set; }

		[Export ("standardOutput", ArgumentSemantic.Retain)]
		NSObject StandardOutput { get; set; }

		[Export ("standardError", ArgumentSemantic.Retain)]
		NSObject StandardError { get; set; }

		[Export ("isRunning")]
		bool IsRunning { get; }

		[Export ("processIdentifier")]
		int ProcessIdentifier { get; } /* pid_t = int */

		[Export ("terminationStatus")]
		int TerminationStatus { get; } /* int, not NSInteger */

		[Export ("terminationReason")]
		NSTaskTerminationReason TerminationReason { get; }

#if !XAMCORE_4_0
		[Field ("NSTaskDidTerminateNotification")]
		NSString NSTaskDidTerminateNotification { get; }
#endif

		[Field ("NSTaskDidTerminateNotification")]
		[Notification]
		NSString DidTerminateNotification { get; }
	}

	[Mac (10, 8)]
	[BaseType (typeof (NSObject))]
	[DesignatedDefaultCtor]
	interface NSUserNotification : NSCoding, NSCopying {
		[Export ("title", ArgumentSemantic.Copy)]
		string Title { get; set; }
		
		[Export ("subtitle", ArgumentSemantic.Copy)]
		string Subtitle { get; set; }
		
		[Export ("informativeText", ArgumentSemantic.Copy)]
		string InformativeText { get; set; }
		
		[Export ("actionButtonTitle", ArgumentSemantic.Copy)]
		string ActionButtonTitle { get; set; }
		
		[Export ("userInfo", ArgumentSemantic.Copy)]
		NSDictionary UserInfo { get; set; }
		
		[Export ("deliveryDate", ArgumentSemantic.Copy)]
		NSDate DeliveryDate { get; set; }
		
		[Export ("deliveryTimeZone", ArgumentSemantic.Copy)]
		NSTimeZone DeliveryTimeZone { get; set; }
		
		[Export ("deliveryRepeatInterval", ArgumentSemantic.Copy)]
		NSDateComponents DeliveryRepeatInterval { get; set; }
		
		[Export ("actualDeliveryDate")]
		NSDate ActualDeliveryDate { get; }
		
		[Export ("presented")]
		bool Presented { [Bind("isPresented")] get; }
		
		[Export ("remote")]
		bool Remote { [Bind("isRemote")] get; }
		
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

		[Mac (10, 9)]
		[NullAllowed, Export ("identifier")]
		string Identifier { get; set; }

		[Mac (10, 9)]
		[NullAllowed, Export ("contentImage", ArgumentSemantic.Copy)]
		NSImage ContentImage { get; set; }

		[Mac (10, 9)]
		[Export ("hasReplyButton")]
		bool HasReplyButton { get; set; }

		[Mac (10, 9)]
		[NullAllowed, Export ("responsePlaceholder")]
		string ResponsePlaceholder { get; set; }

		[Mac (10, 9)]
		[NullAllowed, Export ("response", ArgumentSemantic.Copy)]
		NSAttributedString Response { get; }

		[Mac (10, 10)]
		[NullAllowed, Export ("additionalActions", ArgumentSemantic.Copy)]
		NSUserNotificationAction[] AdditionalActions { get; set; }

		[Mac (10, 10)]
		[NullAllowed, Export ("additionalActivationAction", ArgumentSemantic.Copy)]
		NSUserNotificationAction AdditionalActivationAction { get; }
	}

	[Mac (10,10)]
	[BaseType (typeof(NSObject))]
	interface NSUserNotificationAction : NSCopying
	{
		[Static]
		[Export ("actionWithIdentifier:title:")]
		NSUserNotificationAction GetAction ([NullAllowed] string identifier, [NullAllowed] string title);

		[NullAllowed, Export ("identifier")]
		string Identifier { get; }

		[NullAllowed, Export ("title")]
		string Title { get; }
	}
	
	[Mac (10, 8)]
	[BaseType (typeof (NSObject),
	           Delegates=new string [] {"WeakDelegate"},
	Events=new Type [] { typeof (NSUserNotificationCenterDelegate) })]
	[DisableDefaultCtor] // crash with: NSUserNotificationCenter designitated initializer is _centerForBundleIdentifier
	interface NSUserNotificationCenter 
	{
		[Export ("defaultUserNotificationCenter")][Static]
		NSUserNotificationCenter DefaultUserNotificationCenter { get; }
		
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }
		
		[Wrap ("WeakDelegate")][NullAllowed]
		[Protocolize]
		NSUserNotificationCenterDelegate Delegate { get; set; }
		
		[Export ("scheduledNotifications", ArgumentSemantic.Copy)]
		NSUserNotification [] ScheduledNotifications { get; set; }
		
		[Export ("scheduleNotification:")][PostGet ("ScheduledNotifications")]
		void ScheduleNotification (NSUserNotification notification);
		
		[Export ("removeScheduledNotification:")][PostGet ("ScheduledNotifications")]
		void RemoveScheduledNotification (NSUserNotification notification);
		
		[Export ("deliveredNotifications")]
		NSUserNotification [] DeliveredNotifications { get; }
		
		[Export ("deliverNotification:")][PostGet ("DeliveredNotifications")]
		void DeliverNotification (NSUserNotification notification);
		
		[Export ("removeDeliveredNotification:")][PostGet ("DeliveredNotifications")]
		void RemoveDeliveredNotification (NSUserNotification notification);
		
		[Export ("removeAllDeliveredNotifications")][PostGet ("DeliveredNotifications")]
		void RemoveAllDeliveredNotifications ();
	}
	
	[Mac (10, 8)]
	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface NSUserNotificationCenterDelegate 
	{
		[Export ("userNotificationCenter:didDeliverNotification:"), EventArgs ("UNCDidDeliverNotification")]
		void DidDeliverNotification (NSUserNotificationCenter center, NSUserNotification notification);
		
		[Export ("userNotificationCenter:didActivateNotification:"), EventArgs ("UNCDidActivateNotification")]
		void DidActivateNotification (NSUserNotificationCenter center, NSUserNotification notification);
		
		[Export ("userNotificationCenter:shouldPresentNotification:"), DelegateName ("UNCShouldPresentNotification"), DefaultValue (false)]
		bool ShouldPresentNotification (NSUserNotificationCenter center, NSUserNotification notification);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NSAppleScript : NSCopying {

		// @required - (instancetype)initWithContentsOfURL:(NSURL *)url error:(NSDictionary **)errorInfo;
		[DesignatedInitializer]
		[Export ("initWithContentsOfURL:error:")]
		IntPtr Constructor (NSUrl url, out NSDictionary errorInfo);

		// @required - (instancetype)initWithSource:(NSString *)source;
		[DesignatedInitializer]
		[Export ("initWithSource:")]
		IntPtr Constructor (string source);

		// @property (readonly, copy) NSString * source;
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

		[Export ("richTextSource", ArgumentSemantic.Retain)]
		NSAttributedString RichTextSource { get; }
	}
#endif // MONOMAC

	[iOS (10,0)][TV (10,0)][Watch (3,0)][Mac (10,12)]
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
	
	[iOS (10,0)][TV (10,0)][Watch (3,0)][Mac (10,12)]
	[BaseType (typeof (NSObject), Name = "NSURLSessionTaskTransactionMetrics")]
	interface NSUrlSessionTaskTransactionMetrics {

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
	}

	[iOS (10,0)][TV (10,0)][Watch (3,0)][Mac (10,12)]
	[BaseType (typeof (NSObject), Name = "NSURLSessionTaskMetrics")]
	interface NSUrlSessionTaskMetrics {

		[Export ("transactionMetrics", ArgumentSemantic.Copy)]
		NSUrlSessionTaskTransactionMetrics[] TransactionMetrics { get; }

		[Export ("taskInterval", ArgumentSemantic.Copy)]
		NSDateInterval TaskInterval { get; }

		[Export ("redirectCount")]
		nuint RedirectCount { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitAcceleration : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitAngle : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitArea : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitConcentrationMass : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitDispersion : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("partsPerMillion", ArgumentSemantic.Copy)]
		NSUnitDispersion PartsPerMillion { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitDuration : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitElectricCharge : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitElectricCurrent : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitElectricPotentialDifference : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);
		
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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitElectricResistance : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitEnergy : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitFrequency : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitFuelEfficiency : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitLength : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitIlluminance : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

		[Static]
		[Export ("lux", ArgumentSemantic.Copy)]
		NSUnitIlluminance Lux { get; }

		[New] // kind of overloading a static member
		[Static]
		[Export ("baseUnit")]
		NSDimension BaseUnit { get; }
	}

	[DisableDefaultCtor] // -init should never be called on NSUnit!
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitMass : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitPower : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitPressure : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitSpeed : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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
	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
	[BaseType (typeof (NSDimension))]
	interface NSUnitVolume : NSSecureCoding {
		// inline from base type
		[Export ("initWithSymbol:converter:")]
		[DesignatedInitializer]
		IntPtr Constructor (string symbol, NSUnitConverter converter);

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

	[iOS (10,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
#if !XAMCORE_2_0
	interface NSMeasurement : NSCopying, NSSecureCoding {
#else
	interface NSMeasurement<UnitType> : NSCopying, NSSecureCoding
		where UnitType : NSUnit {
#endif
		[Export ("unit", ArgumentSemantic.Copy)]
		NSUnit Unit { get; }

		[Export ("doubleValue")]
		double DoubleValue { get; }

		[Export ("initWithDoubleValue:unit:")]
		[DesignatedInitializer]
		IntPtr Constructor (double doubleValue, NSUnit unit);

		[Export ("canBeConvertedToUnit:")]
		bool CanBeConvertedTo (NSUnit unit);

#if XAMCORE_2_0
		[Export ("measurementByConvertingToUnit:")]
		NSMeasurement<UnitType> GetMeasurementByConverting (NSUnit unit);

		[Export ("measurementByAddingMeasurement:")]
		NSMeasurement<UnitType> GetMeasurementByAdding (NSMeasurement<UnitType> measurement);

		[Export ("measurementBySubtractingMeasurement:")]
		NSMeasurement<UnitType> GetMeasurementBySubtracting (NSMeasurement<UnitType> measurement);
#endif
	}

	[Watch (3,0)][TV (10,0)][Mac (10,12)][iOS (10,0)]
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

#if XAMCORE_2_0
		[Export ("stringFromMeasurement:")]
		string ToString (NSMeasurement<NSUnit> measurement);
#endif

		[Export ("stringFromUnit:")]
		string ToString (NSUnit unit);
	}

	[iOS (6,0), Mac (10,8), Watch (2,0), TV (9,0)]
	[BaseType (typeof (NSObject), Name = "NSXPCListenerEndpoint")]
	[DisableDefaultCtor]
	interface NSXpcListenerEndpoint : NSSecureCoding
	{
	}
}
