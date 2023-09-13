// Copyright 2016 Xamarin Inc. All rights reserved.

using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace SensitiveContentAnalysis
{
	[NoTV, NoWatch, Mac(14, 0), iOS(17, 0), MacCatalyst(17, 0)]
	[BaseType(typeof(NSObject))]
	[DisableDefaultCtor]
	interface SCSensitivityAnalysis
	{
		[Export("sensitive")]
		bool Sensitive { [Bind("isSensitive")] get; }
	}

	[NoTV, NoWatch, Mac(14, 0), iOS(17, 0), MacCatalyst(17, 0)]
	[Native]
	public enum SCSensitivityAnalysisPolicy : long
	{
		Disabled = 0,
		SimpleInterventions = 1,
		DescriptiveInterventions = 2,
	}

	[NoTV, NoWatch, Mac(14, 0), iOS(17, 0), MacCatalyst(17, 0)]
	[BaseType(typeof(NSObject))]
	interface SCSensitivityAnalyzer
	{
		[Export("analysisPolicy", ArgumentSemantic.Assign)]
		SCSensitivityAnalysisPolicy AnalysisPolicy { get; }

		[Export("analyzeImageFile:completionHandler:")]
		[Async]
		void AnalyzeImage(NSUrl fileURL, Action<SCSensitivityAnalysis, NSError> completionHandler);

		[Export("analyzeCGImage:completionHandler:")]
		[Async]
		void AnalyzeImage(CGImage image, Action<SCSensitivityAnalysis, NSError> completionHandler);

		[Export("analyzeVideoFile:completionHandler:")]
		[Async]
		NSProgress AnalyzeVideo(NSUrl fileURL, Action<SCSensitivityAnalysis, NSError> completionHandler);
	}
}
