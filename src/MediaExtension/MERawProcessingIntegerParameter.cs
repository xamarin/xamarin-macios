#if NET
#if MONOMAC
using System;

using Foundation;
using ObjCRuntime;

namespace MediaExtension {
	/// <summary>This enum is used to select how to initialize a new instance of a <see cref="MERawProcessingIntegerParameter" />.</summary>
	public enum MERawProcessingIntegerParameterInitializationOption {
		/// <summary>The <c>neutralOrCameraValue</c> parameter passed to the constructor is a neutral value.</summary>
		NeutralValue,
		/// <summary>The <c>neutralOrCameraValue</c> parameter passed to the constructor is a camera value.</summary>
		CameraValue,
	}

	public partial class MERawProcessingIntegerParameter {
		/// <summary>Create a new <see cref="MERawProcessingIntegerParameter" /> instance.</summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="key">The key value for the parameter.</param>
		/// <param name="description">The description for the parameter.</param>
		/// <param name="initialValue">The parameter's initial value.</param>
		/// <param name="maximum">The parameter's maximum value.</param>
		/// <param name="minimum">The parameter's minimum value.</param>
		/// <param name="neutralOrCameraValue">The parameter's neutral or camera value.</param>
		/// <param name="option">Specifies whether <paramref name="neutralOrCameraValue" /> is a neutral or a camera value.</param>
		public MERawProcessingIntegerParameter (string name, string key, string description, nint initialValue, nint maximum, nint minimum, nint neutralOrCameraValue, MERawProcessingIntegerParameterInitializationOption option)
			: base (NSObjectFlag.Empty)
		{
			switch (option) {
			case MERawProcessingIntegerParameterInitializationOption.NeutralValue:
				InitializeHandle (_InitWithNeutralValue (name, key, description, initialValue, maximum, minimum, neutralOrCameraValue));
				break;
			case MERawProcessingIntegerParameterInitializationOption.CameraValue:
				InitializeHandle (_InitWithCameraValue (name, key, description, initialValue, maximum, minimum, neutralOrCameraValue));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (option), option, "Invalid enum value.");
			}
		}
	}
}
#endif // MONOMAC
#endif // NET
