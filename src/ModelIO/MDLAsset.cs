#if XAMCORE_2_0 || !MONOMAC
using System;
namespace XamCore.ModelIO {
	public partial class MDLAsset {
		public MDLObject this [nuint index] {
			get {
				return GetObject (index);
			}
		}
	}
}
#endif