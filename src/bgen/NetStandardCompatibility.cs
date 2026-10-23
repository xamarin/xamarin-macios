// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !NET
namespace System.Diagnostics.CodeAnalysis {
	[AttributeUsage (AttributeTargets.All, Inherited = false)]
	public sealed class ExperimentalAttribute : Attribute {
		public ExperimentalAttribute (string diagnosticId)
		{
			DiagnosticId = diagnosticId;
		}

		public string DiagnosticId { get; }
		public string? UrlFormat { get; set; }
	}

	[AttributeUsage (AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	sealed class UnconditionalSuppressMessageAttribute : Attribute {
		public UnconditionalSuppressMessageAttribute (string category, string checkId)
		{
			Category = category;
			CheckId = checkId;
		}

		public string Category { get; }
		public string CheckId { get; }
		public string? Justification { get; set; }
	}
}

namespace System.Runtime.CompilerServices {
	sealed class IsExternalInit {
	}

	[AttributeUsage (AttributeTargets.All, Inherited = false)]
	sealed class RequiredMemberAttribute : Attribute {
	}

	[AttributeUsage (AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	sealed class CompilerFeatureRequiredAttribute : Attribute {
		public CompilerFeatureRequiredAttribute (string featureName)
		{
			FeatureName = featureName;
		}

		public string FeatureName { get; }
		public bool IsOptional { get; init; }
	}

	[AttributeUsage (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, Inherited = false)]
	public sealed class OverloadResolutionPriorityAttribute : Attribute {
		public OverloadResolutionPriorityAttribute (int priority)
		{
			Priority = priority;
		}

		public int Priority { get; }
	}
}

namespace System.Runtime.Versioning {
	abstract class OSPlatformAttribute : Attribute {
		protected OSPlatformAttribute (string platformName)
		{
			PlatformName = platformName;
		}

		public string PlatformName { get; }
	}

	sealed class SupportedOSPlatformAttribute : OSPlatformAttribute {
		public SupportedOSPlatformAttribute (string platformName) : base (platformName)
		{
		}
	}

	sealed class UnsupportedOSPlatformAttribute : OSPlatformAttribute {
		public UnsupportedOSPlatformAttribute (string platformName) : base (platformName)
		{
		}

		public string? Message { get; set; }
	}

	sealed class ObsoletedOSPlatformAttribute : OSPlatformAttribute {
		public ObsoletedOSPlatformAttribute (string platformName) : base (platformName)
		{
		}

		public string? Message { get; set; }
		public string? Url { get; set; }
	}
}
#endif
