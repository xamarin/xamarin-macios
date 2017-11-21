namespace Xamarin.Bundler {
	public class Optimizations
	{
		public const string REMOVE_UITHREAD_CHECKS = "remove-uithread-checks";
		public bool? RemoveUIThreadChecks;
		public bool? InlineSetupBlock;
		public bool? InlineIsDirectBinding;
		public bool? RemoveDynamicRegistrar;
		public bool? InlineIntPtrSize;

		public void Parse (params string [] options)
		{
			foreach (var option in options)
				Parse (option);
		}

		public void Parse (string option)
		{
			if (option == null || option.Length < 2)
				throw ErrorHelper.CreateError (9999, "?");

			bool enabled;
			string opt;
			switch (option [0]) {
			case '-':
				enabled = false;
				opt = option.Substring (1);
				break;
			case '+':
				enabled = true;
				opt = option.Substring (1);
				break;
			default:
				opt = option;
				enabled = true;
				break;
			}

			switch (opt) {
			case "remove-dynamic-registrar":
				RemoveDynamicRegistrar = enabled;
				break;
			case REMOVE_UITHREAD_CHECKS:
				RemoveUIThreadChecks = enabled;
				break;
			case "inline-setupblock":
				InlineSetupBlock = enabled;
				break;
			case "inline-isdirectbinding":
				InlineIsDirectBinding = enabled;
				break;
			case "inline-intptr-size":
				InlineIntPtrSize = enabled;
				break;
			case "all":
				RemoveDynamicRegistrar = enabled;
				RemoveUIThreadChecks = enabled;
				InlineSetupBlock = enabled;
				InlineIsDirectBinding = enabled;
				InlineIntPtrSize = enabled;
				break;
			default:
				ErrorHelper.Warning (9999, $"Unknown optimization: {opt}");
				break;
			}
		}
	}
}