namespace Xamarin.MacDev.Tasks {
	public interface IHasResourcePrefix {
		string ResourcePrefix { get; set; }
	}

	public interface IHasProjectDir {
		string ProjectDir { get; set; }
	}

	public interface IHasSessionId {
		string SessionId { get; set; }
	}
}
