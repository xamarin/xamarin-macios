#if !XAMCORE_3_0
using FileProvider;

namespace Foundation {
	public partial class NSNetService {

		[Obsolete ("")]
		public NSNetService ()
		{
		}
	}
	
#if !XAMCORE_4_0
	public partial class NSNetService {
		
		[Obsolete ("This API has been removed.")]
		public static NSError GetFileProviderErrorForOutOfDateItem (INSFileProviderItem updatedVersion)
		{
			return null;
		}
	}
#endif
	
}
#endif
