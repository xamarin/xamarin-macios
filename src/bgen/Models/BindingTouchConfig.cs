using System.Collections.Generic;
#nullable enable

public class BindingTouchConfig {
	public bool ShowHelp = false;
	public bool UseZeroCopy = false;
	public string? BindingFilesOutputDirectory = null;
	public string? TemporaryFileDirectory = null;
	public string? HelperClassNamespace = null;
	public bool DeleteTemporaryFiles = true;
	public bool IsDebug = false;
	public bool IsUnsafe = true;
	public bool IsExternal = false;
	public bool IsPublicMode = true;
	public bool? OmitStandardLibrary = null;
	public bool? InlineSelectors = null;
	public List<string> Sources = new ();
	public List<string> Resources = new ();
	public List<string> LinkWith = new ();
	public List<string> ApiSources = new ();
	public List<string> CoreSources = new ();
	public List<string> ExtraSources = new ();
	public List<string> Defines = new ();
	public string? GeneratedFileList = null;
	public bool ProcessEnums = false;
	public string? TargetFramework = null;
	public string? Baselibdll = null;
	public string? Attributedll = null;
}
