using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace AudioUnit
{
#if XAMCORE_2_0 || !MONOMAC
	[iOS (9,0), Mac(10,11)]
	public partial class AUParameter
	{
		public string GetString (float? value)
		{
			unsafe {
				if (value != null && value.HasValue) {
					float f = value.Value;
					return this._GetString (new IntPtr (&f));
				}
				else {
					return this._GetString (IntPtr.Zero);
				}
			}
		}

		public void SetValue (float value, AUParameterObserverToken originator)
		{
#pragma warning disable CS0618
			SetValue (value, originator.ObserverToken);
#pragma warning restore CS0618
		}

		public void SetValue (float value, AUParameterObserverToken originator, ulong hostTime)
		{
#pragma warning disable CS0618
			SetValue (value, originator.ObserverToken, hostTime);
#pragma warning restore CS0618
		}
	}
#endif
}
