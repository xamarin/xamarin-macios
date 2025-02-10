#if NET
#if MONOMAC
using System;

using Foundation;
using ObjCRuntime;

namespace MediaExtension {
	/// <summary>This enum is used to select how to initialize a new instance of a <see cref="MERawProcessingListParameter" />.</summary>
	public enum MERawProcessingListParameterInitializationOption {
		/// <summary>The <c>neutralOrCameraValue</c> parameter passed to the constructor is a neutral value.</summary>
		NeutralValue,
		/// <summary>The <c>neutralOrCameraValue</c> parameter passed to the constructor is a camera value.</summary>
		CameraValue,
	}

	public partial class MERawProcessingListParameter {
		/// <summary>Create a new <see cref="MERawProcessingListParameter" /> instance.</summary>
		/// <param name="name">The name of the parameter.</param>
		/// <param name="key">The key value for the parameter.</param>
		/// <param name="description">The description for the parameter.</param>
		/// <param name="listElements">The parameter's list elements value.</param>
		/// <param name="initialValue">The parameter's initial value.</param>
		/// <param name="neutralOrCameraValue">The parameter's neutral or camera value.</param>
		/// <param name="option">Specifies whether <paramref name="neutralOrCameraValue" /> is a neutral or a camera value.</param>
		public MERawProcessingListParameter (string name, string key, string description, MERawProcessingListElementParameter[] listElements, nint initialValue, nint neutralOrCameraValue, MERawProcessingListParameterInitializationOption option)
			: base (NSObjectFlag.Empty)
		{
			switch (option) {
			case MERawProcessingListParameterInitializationOption.NeutralValue:
				InitializeHandle (_InitWithNeutralValue (name, key, description, listElements, initialValue, neutralOrCameraValue));
				break;
			case MERawProcessingListParameterInitializationOption.CameraValue:
				InitializeHandle (_InitWithCameraValue (name, key, description, listElements, initialValue, neutralOrCameraValue));
				break;
			default:
				throw new ArgumentOutOfRangeException (nameof (option), option, "Invalid enum value.");
			}
		}
	}
}
#endif // MONOMAC
#endif // NET
