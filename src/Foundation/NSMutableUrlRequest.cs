namespace Foundation {

	public partial class NSUrlRequest {
		public string this [string key] {
			get {
				return Header (key);
			}
		}
	}

	public partial class NSMutableUrlRequest {
		public new string this [string key] {
			get {
				return Header (key);
			}

			set {
				_SetValue (value, key);
			}
		}
	}
}
