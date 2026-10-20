using System.Collections.Generic;
using Mono.Options;

#nullable enable

public class BindingTouchConfig {
	public bool ShowHelp = false;
	public string? BindingFilesOutputDirectory = null;
	public string? TemporaryFileDirectory = null;
	public string? HelperClassNamespace = null;
	public bool DeleteTemporaryFiles = true;
	public bool IsDebug = false;
	public bool IsExternal = false;
	public bool IsPublicMode = true;
	public bool? OmitStandardLibrary = null;
	public bool? InlineSelectors = null;
	public List<string> LinkWith = new ();
	public string? GeneratedFileList = null;
	public bool ProcessEnums = false;
	public string? TargetFramework = null;
	public string? Baselibdll = null;
	public string? Attributedll = null;
	public OptionSet OptionSet = new ();
}
