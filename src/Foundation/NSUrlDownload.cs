#if MONOMAC
namespace Foundation {

	public partial class NSUrlDownload {
		
		public override string ToString ()
		{
			return GetType ().ToString ();
		}
	}
}
#endif
