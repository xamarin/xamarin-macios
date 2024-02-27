//
// This file describes the API that the generator will produce
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2012 Xamarin Inc.
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
using System;
using System.ComponentModel;
using ObjCRuntime;
using CoreFoundation;
using Foundation;
using CoreGraphics;
#if !WATCH
using CoreMedia;
#endif

namespace Foundation {
	public enum NSCalendarType {
		Gregorian,
		Buddhist,
		Chinese,
		Hebrew,
		Islamic,
		IslamicCivil,
		Japanese,
		Taiwan,
#if !XAMCORE_5_0
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete]
		RepublicOfChina = Taiwan,
#endif
		Persian,
		Indian,
		ISO8601,
		Coptic,
		EthiopicAmeteAlem,
		EthiopicAmeteMihret,
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		IslamicTabular,
#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		IslamicUmmAlQura,
	}

	public partial class NSCalendar {
		static NSString GetCalendarIdentifier (NSCalendarType type)
		{
			switch (type) {
			case NSCalendarType.Gregorian:
				return NSGregorianCalendar;
			case NSCalendarType.Buddhist:
				return NSBuddhistCalendar;
			case NSCalendarType.Chinese:
				return NSChineseCalendar;
			case NSCalendarType.Hebrew:
				return NSHebrewCalendar;
			case NSCalendarType.Islamic:
				return NSIslamicCalendar;
			case NSCalendarType.IslamicCivil:
				return NSIslamicCivilCalendar;
			case NSCalendarType.Japanese:
				return NSJapaneseCalendar;
#pragma warning disable 612 // RepublicOfChina is obsolete
			case NSCalendarType.RepublicOfChina:
#pragma warning restore 612
				return NSRepublicOfChinaCalendar;
			case NSCalendarType.Persian:
				return NSPersianCalendar;
			case NSCalendarType.Indian:
				return NSIndianCalendar;
			case NSCalendarType.ISO8601:
				return NSISO8601Calendar;
			case NSCalendarType.Coptic:
				return CopticCalendar;
			case NSCalendarType.EthiopicAmeteAlem:
				return EthiopicAmeteAlemCalendar;
			case NSCalendarType.EthiopicAmeteMihret:
				return EthiopicAmeteMihretCalendar;
			case NSCalendarType.IslamicTabular:
				return IslamicTabularCalendar;
			case NSCalendarType.IslamicUmmAlQura:
				return IslamicUmmAlQuraCalendar;
			default:
				throw new ArgumentException ("Unknown NSCalendarType value");
			}
		}

		public NSCalendar (NSCalendarType calendarType) : this (GetCalendarIdentifier (calendarType)) { }
	}
}
