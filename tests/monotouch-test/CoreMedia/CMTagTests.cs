//
// Unit tests for CMTag
//
// Authors:
//	Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2025 Microsoft Corp. All rights reserved.
//
using System;

using CoreFoundation;
using CoreMedia;
using Foundation;
using ObjCRuntime;

using NUnit.Framework;

using Xamarin.Utils;

using MonoTouchFixtures.AVFoundation;

namespace MonoTouchFixtures.CoreMedia {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class CMTagTests {

		public static string FourCC (int value)
		{
			return AVFoundationEnumTest.FourCC (value);
		}

		public static string FourCC (uint value)
		{
			return AVFoundationEnumTest.FourCC (value);
		}

		public static uint FourCC (string value)
		{
			return AVFoundationEnumTest.FourCC (value);
		}

		[Test]
		public void Default ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var tag = default (CMTag);
			AssertTag (tag, CMTagCategory.Undefined, CMTagDataType.Invalid, 0, false, false, 0, false, 0, false, 0, false, 0, "Default");
		}

		[Test]
		public void Create ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			Assert.Multiple (() => {
				AssertTag (
					CMTag.CreateWithSInt64Value (CMTagCategory.MediaType, 314),
					CMTagCategory.MediaType, CMTagDataType.SInt64, 314, true, false, 0, false, 0, false, 0, true, 314, "CreateWithSInt64Value");

				AssertTag (
					CMTag.CreateWithFloat64Value (CMTagCategory.MediaType, 3.14),
					CMTagCategory.MediaType, CMTagDataType.Float64, 4614253070214989087, true, true, 3.14, false, 0, false, 0, false, 0, "CreateWithFloat64Value");

				AssertTag (
					CMTag.CreateWithOSTypeValue (CMTagCategory.MediaType, 314),
					CMTagCategory.MediaType, CMTagDataType.OSType, 314, true, false, 0, true, 314, false, 0, false, 0, "CreateWithOSTypeValue");

				AssertTag (
					CMTag.CreateWithFlagsValue (CMTagCategory.MediaType, 314),
					CMTagCategory.MediaType, CMTagDataType.Flags, 314, true, false, 0, false, 0, true, 314, false, 0, "CreateWithFlagsValue");
			});
		}

		[Test]
		public void Equals ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			Assert.Multiple (() => {
				Assert.AreEqual (true, CMTag.Equals (default (CMTag), default (CMTag)), "Default");
				Assert.AreEqual (true, CMTag.Equals (CMTag.Invalid, CMTag.Invalid), "Invalid");
				Assert.AreEqual (true, CMTag.Equals (CMTag.MediaTypeVideo, CMTag.MediaTypeVideo), "MediaTypeVideo");
				Assert.AreEqual (true, CMTag.Equals (CMTag.MediaSubTypeMebx, CMTag.MediaSubTypeMebx), "MediaSubTypeMebx");
				Assert.AreEqual (true, CMTag.Equals (CMTag.MediaTypeAudio, CMTag.MediaTypeAudio), "MediaTypeAudio");
				Assert.AreEqual (true, CMTag.Equals (CMTag.MediaTypeMetadata, CMTag.MediaTypeMetadata), "MediaTypeMetadata");
				Assert.AreEqual (true, CMTag.Equals (CMTag.StereoLeftEye, CMTag.StereoLeftEye), "StereoLeftEye");
				Assert.AreEqual (true, CMTag.Equals (CMTag.StereoRightEye, CMTag.StereoRightEye), "StereoRightEye");
				Assert.AreEqual (true, CMTag.Equals (CMTag.StereoLeftAndRightEye, CMTag.StereoLeftAndRightEye), "StereoLeftAndRightEye");
				Assert.AreEqual (true, CMTag.Equals (CMTag.StereoNone, CMTag.StereoNone), "StereoNone");
				Assert.AreEqual (true, CMTag.Equals (CMTag.StereoInterpretationOrderReversed, CMTag.StereoInterpretationOrderReversed), "StereoInterpretationOrderReversed");
				Assert.AreEqual (true, CMTag.Equals (CMTag.ProjectionTypeRectangular, CMTag.ProjectionTypeRectangular), "ProjectionTypeRectangular");
				Assert.AreEqual (true, CMTag.Equals (CMTag.ProjectionTypeEquirectangular, CMTag.ProjectionTypeEquirectangular), "ProjectionTypeEquirectangular");
				Assert.AreEqual (true, CMTag.Equals (CMTag.ProjectionTypeHalfEquirectangular, CMTag.ProjectionTypeHalfEquirectangular), "ProjectionTypeHalfEquirectangular");
				Assert.AreEqual (true, CMTag.Equals (CMTag.ProjectionTypeFisheye, CMTag.ProjectionTypeFisheye), "ProjectionTypeFisheye");
				Assert.AreEqual (true, CMTag.Equals (CMTag.PackingTypeNone, CMTag.PackingTypeNone), "PackingTypeNone");
				Assert.AreEqual (true, CMTag.Equals (CMTag.PackingTypeSideBySide, CMTag.PackingTypeSideBySide), "PackingTypeSideBySide");
				Assert.AreEqual (true, CMTag.Equals (CMTag.PackingTypeOverUnder, CMTag.PackingTypeOverUnder), "PackingTypeOverUnder");

				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.MediaTypeVideo), "Invalid vs MediaTypeVideo");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.MediaSubTypeMebx), "Invalid vs MediaSubTypeMebx");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.MediaTypeAudio), "Invalid vs MediaTypeAudio");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.MediaTypeMetadata), "Invalid vs MediaTypeMetadata");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.StereoLeftEye), "Invalid vs StereoLeftEye");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.StereoRightEye), "Invalid vs StereoRightEye");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.StereoLeftAndRightEye), "Invalid vs StereoLeftAndRightEye");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.StereoNone), "Invalid vs StereoNone");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.StereoInterpretationOrderReversed), "Invalid vs StereoInterpretationOrderReversed");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.ProjectionTypeRectangular), "Invalid vs ProjectionTypeRectangular");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.ProjectionTypeEquirectangular), "Invalid vs ProjectionTypeEquirectangular");
				Assert.AreEqual (!TestRuntime.CheckXcodeVersion (16, 0), CMTag.Equals (CMTag.Invalid, CMTag.ProjectionTypeHalfEquirectangular), "Invalid vs ProjectionTypeHalfEquirectangular");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.ProjectionTypeFisheye), "Invalid vs ProjectionTypeFisheye");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.PackingTypeNone), "Invalid vs PackingTypeNone");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.PackingTypeSideBySide), "Invalid vs PackingTypeSideBySide");
				Assert.AreEqual (false, CMTag.Equals (CMTag.Invalid, CMTag.PackingTypeOverUnder), "Invalid vs PackingTypeOverUnder");

				Assert.AreEqual (false, CMTag.Equals (CMTag.MediaTypeVideo, CMTag.Invalid), "MediaTypeVideo vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.MediaSubTypeMebx, CMTag.Invalid), "MediaSubTypeMebx vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.MediaTypeAudio, CMTag.Invalid), "MediaTypeAudio vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.MediaTypeMetadata, CMTag.Invalid), "MediaTypeMetadata vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.StereoLeftEye, CMTag.Invalid), "StereoLeftEye vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.StereoRightEye, CMTag.Invalid), "StereoRightEye vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.StereoLeftAndRightEye, CMTag.Invalid), "StereoLeftAndRightEye vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.StereoNone, CMTag.Invalid), "StereoNone vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.StereoInterpretationOrderReversed, CMTag.Invalid), "StereoInterpretationOrderReversed vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.ProjectionTypeRectangular, CMTag.Invalid), "ProjectionTypeRectangular vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.ProjectionTypeEquirectangular, CMTag.Invalid), "ProjectionTypeEquirectangular vs Invalid");
				Assert.AreEqual (!TestRuntime.CheckXcodeVersion (16, 0), CMTag.Equals (CMTag.ProjectionTypeHalfEquirectangular, CMTag.Invalid), "ProjectionTypeHalfEquirectangular vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.ProjectionTypeFisheye, CMTag.Invalid), "ProjectionTypeFisheye vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.PackingTypeNone, CMTag.Invalid), "PackingTypeNone vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.PackingTypeSideBySide, CMTag.Invalid), "PackingTypeSideBySide vs Invalid");
				Assert.AreEqual (false, CMTag.Equals (CMTag.PackingTypeOverUnder, CMTag.Invalid), "PackingTypeOverUnder vs Invalid");
			});
		}

		[Test]
		public void Compare ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			Assert.Multiple (() => {
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (default (CMTag), default (CMTag)), "Default");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.Invalid, CMTag.Invalid), "Invalid");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.MediaTypeVideo, CMTag.MediaTypeVideo), "MediaTypeVideo");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.MediaSubTypeMebx, CMTag.MediaSubTypeMebx), "MediaSubTypeMebx");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.MediaTypeAudio, CMTag.MediaTypeAudio), "MediaTypeAudio");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.MediaTypeMetadata, CMTag.MediaTypeMetadata), "MediaTypeMetadata");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.StereoLeftEye, CMTag.StereoLeftEye), "StereoLeftEye");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.StereoRightEye, CMTag.StereoRightEye), "StereoRightEye");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.StereoLeftAndRightEye, CMTag.StereoLeftAndRightEye), "StereoLeftAndRightEye");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.StereoNone, CMTag.StereoNone), "StereoNone");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.StereoInterpretationOrderReversed, CMTag.StereoInterpretationOrderReversed), "StereoInterpretationOrderReversed");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.ProjectionTypeRectangular, CMTag.ProjectionTypeRectangular), "ProjectionTypeRectangular");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.ProjectionTypeEquirectangular, CMTag.ProjectionTypeEquirectangular), "ProjectionTypeEquirectangular");
				if (TestRuntime.CheckXcodeVersion (16, 0)) {
					Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.ProjectionTypeHalfEquirectangular, CMTag.ProjectionTypeHalfEquirectangular), "ProjectionTypeHalfEquirectangular");
				} else {
					Assert.AreNotEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.ProjectionTypeHalfEquirectangular, CMTag.ProjectionTypeHalfEquirectangular), "ProjectionTypeHalfEquirectangular");
				}
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.ProjectionTypeFisheye, CMTag.ProjectionTypeFisheye), "ProjectionTypeFisheye");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.PackingTypeNone, CMTag.PackingTypeNone), "PackingTypeNone");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.PackingTypeSideBySide, CMTag.PackingTypeSideBySide), "PackingTypeSideBySide");
				Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.PackingTypeOverUnder, CMTag.PackingTypeOverUnder), "PackingTypeOverUnder");

				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.MediaTypeVideo), "Invalid vs MediaTypeVideo");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.MediaSubTypeMebx), "Invalid vs MediaSubTypeMebx");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.MediaTypeAudio), "Invalid vs MediaTypeAudio");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.MediaTypeMetadata), "Invalid vs MediaTypeMetadata");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.StereoLeftEye), "Invalid vs StereoLeftEye");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.StereoRightEye), "Invalid vs StereoRightEye");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.StereoLeftAndRightEye), "Invalid vs StereoLeftAndRightEye");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.StereoNone), "Invalid vs StereoNone");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.StereoInterpretationOrderReversed), "Invalid vs StereoInterpretationOrderReversed");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.ProjectionTypeRectangular), "Invalid vs ProjectionTypeRectangular");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.ProjectionTypeEquirectangular), "Invalid vs ProjectionTypeEquirectangular");
				if (TestRuntime.CheckXcodeVersion (16, 0)) {
					Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.ProjectionTypeHalfEquirectangular), "Invalid vs ProjectionTypeHalfEquirectangular");
				} else {
					Assert.AreEqual (CFComparisonResult.Equal, CMTag.Compare (CMTag.Invalid, CMTag.ProjectionTypeHalfEquirectangular), "Invalid vs ProjectionTypeHalfEquirectangular");
				}
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.ProjectionTypeFisheye), "Invalid vs ProjectionTypeFisheye");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.PackingTypeNone), "Invalid vs PackingTypeNone");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.PackingTypeSideBySide), "Invalid vs PackingTypeSideBySide");
				Assert.AreEqual (CFComparisonResult.LessThan, CMTag.Compare (CMTag.Invalid, CMTag.PackingTypeOverUnder), "Invalid vs PackingTypeOverUnder");

				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.MediaTypeVideo, CMTag.Invalid), "MediaTypeVideo vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.MediaSubTypeMebx, CMTag.Invalid), "MediaSubTypeMebx vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.MediaTypeAudio, CMTag.Invalid), "MediaTypeAudio vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.MediaTypeMetadata, CMTag.Invalid), "MediaTypeMetadata vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.StereoLeftEye, CMTag.Invalid), "StereoLeftEye vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.StereoRightEye, CMTag.Invalid), "StereoRightEye vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.StereoLeftAndRightEye, CMTag.Invalid), "StereoLeftAndRightEye vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.StereoNone, CMTag.Invalid), "StereoNone vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.StereoInterpretationOrderReversed, CMTag.Invalid), "StereoInterpretationOrderReversed vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.ProjectionTypeRectangular, CMTag.Invalid), "ProjectionTypeRectangular vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.ProjectionTypeEquirectangular, CMTag.Invalid), "ProjectionTypeEquirectangular vs Invalid");
				if (TestRuntime.CheckXcodeVersion (16, 0)) {
					Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.ProjectionTypeHalfEquirectangular, CMTag.Invalid), "ProjectionTypeHalfEquirectangular vs Invalid");
				} else {
					Assert.AreEqual (CFComparisonResult.EqualTo, CMTag.Compare (CMTag.ProjectionTypeHalfEquirectangular, CMTag.Invalid), "ProjectionTypeHalfEquirectangular vs Invalid");
				}
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.ProjectionTypeFisheye, CMTag.Invalid), "ProjectionTypeFisheye vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.PackingTypeNone, CMTag.Invalid), "PackingTypeNone vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.PackingTypeSideBySide, CMTag.Invalid), "PackingTypeSideBySide vs Invalid");
				Assert.AreEqual (CFComparisonResult.GreaterThan, CMTag.Compare (CMTag.PackingTypeOverUnder, CMTag.Invalid), "PackingTypeOverUnder vs Invalid");
			});
		}

		void AssertTag (CMTag tag, CMTagCategory category, CMTagDataType dataType, ulong value, bool isValid, bool hasFloat64Value, double float64Value, bool hasOSTypeValue, uint osTypeValue, bool hasFlagsValue, ulong flagsValue, bool hasInt64Value, long int64Value, string message)
		{
			Assert.AreEqual (category, tag.Category, $"{message}: Category");
			Assert.AreEqual (dataType, tag.DataType, $"{message}: DataType");
			Assert.AreEqual (value, tag.Value, $"{message}: Value");
			Assert.AreEqual (isValid, tag.IsValid, $"{message}: IsValid");
			Assert.AreEqual (hasFloat64Value, tag.HasFloat64Value, $"{message}: HasFloat64Value");
			if (hasFloat64Value)
				Assert.AreEqual (float64Value, tag.Float64Value, $"{message}: Float64Value");
			Assert.AreEqual (hasOSTypeValue, tag.HasOSTypeValue, $"{message}: HasOSTypeValue");
			if (hasOSTypeValue)
				Assert.AreEqual (osTypeValue, tag.OSTypeValue, $"{message}: OSTypeValue ({AVFoundationEnumTest.FourCC (osTypeValue)}={osTypeValue} vs {AVFoundationEnumTest.FourCC (tag.OSTypeValue)}={tag.OSTypeValue})");
			Assert.AreEqual (hasFlagsValue, tag.HasFlagsValue, $"{message}: HasFlagsValue");
			if (hasFlagsValue)
				Assert.AreEqual (flagsValue, tag.FlagsValue, $"{message}: FlagsValue");
			Assert.AreEqual (hasInt64Value, tag.HasInt64Value, $"{message}: HasInt64Value");
			if (hasInt64Value)
				Assert.AreEqual (int64Value, tag.Int64Value, $"{message}: Int64Value");
		}

		[Test]
		public void Fields ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			Assert.Multiple (() => {
				AssertTag (CMTag.Invalid, CMTagCategory.Undefined, CMTagDataType.Invalid, 0, false, false, 0, false, 0, false, 0, false, 0, "Invalid");
				AssertTag (CMTag.MediaTypeVideo, CMTagCategory.MediaType, CMTagDataType.OSType, FourCC ("vide"), true, false, 0, true, FourCC ("vide"), false, 0, false, 0, "MediaTypeVideo");
				AssertTag (CMTag.MediaSubTypeMebx, CMTagCategory.MediaSubType, CMTagDataType.OSType, FourCC ("mebx"), true, false, 0, true, FourCC ("mebx"), false, 0, false, 0, "MediaSubTypeMebx");
				AssertTag (CMTag.MediaTypeAudio, CMTagCategory.MediaType, CMTagDataType.OSType, FourCC ("soun"), true, false, 0, true, FourCC ("soun"), false, 0, false, 0, "MediaTypeAudio");
				AssertTag (CMTag.MediaTypeMetadata, CMTagCategory.MediaType, CMTagDataType.OSType, FourCC ("meta"), true, false, 0, true, FourCC ("meta"), false, 0, false, 0, "MediaTypeMetadata");
				AssertTag (CMTag.StereoLeftEye, CMTagCategory.StereoView, CMTagDataType.Flags, 1, true, false, 0, false, 0, true, 1, false, 0, "StereoLeftEye");
				AssertTag (CMTag.StereoRightEye, CMTagCategory.StereoView, CMTagDataType.Flags, 2, true, false, 0, false, 0, true, 2, false, 0, "StereoRightEye");
				AssertTag (CMTag.StereoLeftAndRightEye, CMTagCategory.StereoView, CMTagDataType.Flags, 3, true, false, 0, false, 0, true, 3, false, 0, "StereoLeftAndRightEye");
				AssertTag (CMTag.StereoNone, CMTagCategory.StereoView, CMTagDataType.Flags, 0, true, false, 0, false, 0, true, 0, false, 0, "StereoNone");
				AssertTag (CMTag.StereoInterpretationOrderReversed, CMTagCategory.StereoViewInterpretation, CMTagDataType.Flags, 1, true, false, 0, false, 0, true, 1, false, 0, "StereoInterpretationOrderReversed");
				AssertTag (CMTag.ProjectionTypeRectangular, CMTagCategory.ProjectionType, CMTagDataType.OSType, FourCC ("rect"), true, false, 0, true, FourCC ("rect"), false, 0, false, 0, "ProjectionTypeRectangular");
				AssertTag (CMTag.ProjectionTypeEquirectangular, CMTagCategory.ProjectionType, CMTagDataType.OSType, FourCC ("equi"), true, false, 0, true, FourCC ("equi"), false, 0, false, 0, "ProjectionTypeEquirectangular");
				if (TestRuntime.AssertXcodeVersion (16, 0)) {
					AssertTag (CMTag.ProjectionTypeHalfEquirectangular, CMTagCategory.ProjectionType, CMTagDataType.OSType, FourCC ("hequ"), true, false, 0, true, FourCC ("hequ"), false, 0, false, 0, "ProjectionTypeHalfEquirectangular");
				} else {
					AssertTag (CMTag.ProjectionTypeHalfEquirectangular, CMTagCategory.Undefined, CMTagDataType.Invalid, 0, false, false, 0, false, 0, false, 0, false, 0, "Invalid");
				}
				AssertTag (CMTag.ProjectionTypeFisheye, CMTagCategory.ProjectionType, CMTagDataType.OSType, FourCC ("fish"), true, false, 0, true, FourCC ("fish"), false, 0, false, 0, "ProjectionTypeFisheye");
				AssertTag (CMTag.PackingTypeNone, CMTagCategory.PackingType, CMTagDataType.OSType, FourCC ("none"), true, false, 0, true, FourCC ("none"), false, 0, false, 0, "PackingTypeNone");
				AssertTag (CMTag.PackingTypeSideBySide, CMTagCategory.PackingType, CMTagDataType.OSType, FourCC ("side"), true, false, 0, true, FourCC ("side"), false, 0, false, 0, "PackingTypeSideBySide");
				AssertTag (CMTag.PackingTypeOverUnder, CMTagCategory.PackingType, CMTagDataType.OSType, FourCC ("over"), true, false, 0, true, FourCC ("over"), false, 0, false, 0, "PackingTypeOverUnder");
			});
		}

		[Test]
		public void ToStringTests ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			Assert.Multiple (() => {
				Assert.AreEqual ("{category:''{INVALID}", default (CMTag).ToString (), "Default");
				Assert.AreEqual ("{category:''{INVALID}", CMTag.Invalid.ToString (), "Invalid");
				Assert.AreEqual ("{category:'mdia' value:'vide' <OSType>}", CMTag.MediaTypeVideo.ToString (), "MediaTypeVideo");
				Assert.AreEqual ("{category:'msub' value:'mebx' <OSType>}", CMTag.MediaSubTypeMebx.ToString (), "MediaSubTypeMebx");
				Assert.AreEqual ("{category:'mdia' value:'soun' <OSType>}", CMTag.MediaTypeAudio.ToString (), "MediaTypeAudio");
				Assert.AreEqual ("{category:'mdia' value:'meta' <OSType>}", CMTag.MediaTypeMetadata.ToString (), "MediaTypeMetadata");
				Assert.AreEqual ("{category:'eyes' value:0x1 <flags>}", CMTag.StereoLeftEye.ToString (), "StereoLeftEye");
				Assert.AreEqual ("{category:'eyes' value:0x2 <flags>}", CMTag.StereoRightEye.ToString (), "StereoRightEye");
				Assert.AreEqual ("{category:'eyes' value:0x3 <flags>}", CMTag.StereoLeftAndRightEye.ToString (), "StereoLeftAndRightEye");
				Assert.AreEqual ("{category:'eyes' value:0x0 <flags>}", CMTag.StereoNone.ToString (), "StereoNone");
				Assert.AreEqual ("{category:'eyip' value:0x1 <flags>}", CMTag.StereoInterpretationOrderReversed.ToString (), "StereoInterpretationOrderReversed");
				Assert.AreEqual ("{category:'proj' value:'rect' <OSType>}", CMTag.ProjectionTypeRectangular.ToString (), "ProjectionTypeRectangular");
				Assert.AreEqual ("{category:'proj' value:'equi' <OSType>}", CMTag.ProjectionTypeEquirectangular.ToString (), "ProjectionTypeEquirectangular");
				if (TestRuntime.CheckXcodeVersion (16, 0)) {
					Assert.AreEqual ("{category:'proj' value:'hequ' <OSType>}", CMTag.ProjectionTypeHalfEquirectangular.ToString (), "ProjectionTypeHalfEquirectangular");
				} else {
					Assert.AreEqual ("{category:''{INVALID}", CMTag.ProjectionTypeHalfEquirectangular.ToString (), "ProjectionTypeHalfEquirectangular");
				}
				Assert.AreEqual ("{category:'proj' value:'fish' <OSType>}", CMTag.ProjectionTypeFisheye.ToString (), "ProjectionTypeFisheye");
				Assert.AreEqual ("{category:'pack' value:'none' <OSType>}", CMTag.PackingTypeNone.ToString (), "PackingTypeNone");
				Assert.AreEqual ("{category:'pack' value:'side' <OSType>}", CMTag.PackingTypeSideBySide.ToString (), "PackingTypeSideBySide");
				Assert.AreEqual ("{category:'pack' value:'over' <OSType>}", CMTag.PackingTypeOverUnder.ToString (), "PackingTypeOverUnder");
			});
		}

		[Test]
		public void Dictionary ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			var roundTrip = new Action<CMTag, string> ((tag, message) => {
				var dict = tag.ToDictionary ();
				var deserializedTag = CMTag.Create (dict);
				Assert.AreEqual (true, CMTag.Equals (tag, deserializedTag), message);
			});

			Assert.Multiple (() => {
				roundTrip (default (CMTag), "Default");
				roundTrip (CMTag.Invalid, "Invalid");
				roundTrip (CMTag.MediaTypeVideo, "MediaTypeVideo");
				roundTrip (CMTag.MediaSubTypeMebx, "MediaSubTypeMebx");
				roundTrip (CMTag.MediaTypeAudio, "MediaTypeAudio");
				roundTrip (CMTag.MediaTypeMetadata, "MediaTypeMetadata");
				roundTrip (CMTag.StereoLeftEye, "StereoLeftEye");
				roundTrip (CMTag.StereoRightEye, "StereoRightEye");
				roundTrip (CMTag.StereoLeftAndRightEye, "StereoLeftAndRightEye");
				roundTrip (CMTag.StereoNone, "StereoNone");
				roundTrip (CMTag.StereoInterpretationOrderReversed, "StereoInterpretationOrderReversed");
				roundTrip (CMTag.ProjectionTypeRectangular, "ProjectionTypeRectangular");
				roundTrip (CMTag.ProjectionTypeEquirectangular, "ProjectionTypeEquirectangular");
				roundTrip (CMTag.ProjectionTypeHalfEquirectangular, "ProjectionTypeHalfEquirectangular");
				roundTrip (CMTag.ProjectionTypeFisheye, "ProjectionTypeFisheye");
				roundTrip (CMTag.PackingTypeNone, "PackingTypeNone");
				roundTrip (CMTag.PackingTypeSideBySide, "PackingTypeSideBySide");
				roundTrip (CMTag.PackingTypeOverUnder, "PackingTypeOverUnder");
			});
		}

		[Test]
		public void Hash ()
		{
			TestRuntime.AssertXcodeVersion (15, 0);

			Assert.Multiple (() => {
				Assert.AreNotEqual (0, default (CMTag).GetHashCode (), "Default");
				Assert.AreNotEqual (0, CMTag.Invalid.GetHashCode (), "Invalid");
				Assert.AreNotEqual (0, CMTag.MediaTypeVideo.GetHashCode (), "MediaTypeVideo");
				Assert.AreNotEqual (0, CMTag.MediaSubTypeMebx.GetHashCode (), "MediaSubTypeMebx");
				Assert.AreNotEqual (0, CMTag.MediaTypeAudio.GetHashCode (), "MediaTypeAudio");
				Assert.AreNotEqual (0, CMTag.MediaTypeMetadata.GetHashCode (), "MediaTypeMetadata");
				Assert.AreNotEqual (0, CMTag.StereoLeftEye.GetHashCode (), "StereoLeftEye");
				Assert.AreNotEqual (0, CMTag.StereoRightEye.GetHashCode (), "StereoRightEye");
				Assert.AreNotEqual (0, CMTag.StereoLeftAndRightEye.GetHashCode (), "StereoLeftAndRightEye");
				Assert.AreNotEqual (0, CMTag.StereoNone.GetHashCode (), "StereoNone");
				Assert.AreNotEqual (0, CMTag.StereoInterpretationOrderReversed.GetHashCode (), "StereoInterpretationOrderReversed");
				Assert.AreNotEqual (0, CMTag.ProjectionTypeRectangular.GetHashCode (), "ProjectionTypeRectangular");
				Assert.AreNotEqual (0, CMTag.ProjectionTypeEquirectangular.GetHashCode (), "ProjectionTypeEquirectangular");
				Assert.AreNotEqual (0, CMTag.ProjectionTypeHalfEquirectangular.GetHashCode (), "ProjectionTypeHalfEquirectangular");
				Assert.AreNotEqual (0, CMTag.ProjectionTypeFisheye.GetHashCode (), "ProjectionTypeFisheye");
				Assert.AreNotEqual (0, CMTag.PackingTypeNone.GetHashCode (), "PackingTypeNone");
				Assert.AreNotEqual (0, CMTag.PackingTypeSideBySide.GetHashCode (), "PackingTypeSideBySide");
				Assert.AreNotEqual (0, CMTag.PackingTypeOverUnder.GetHashCode (), "PackingTypeOverUnder");
			});
		}
	}
}
