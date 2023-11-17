using System.Collections.Generic;
#nullable enable

public class BindingTouchConfig
{
	public bool show_help = false;
	public bool zero_copy = false;
	public string? basedir = null;
	public string? tmpdir = null;
	public string? ns = null;
	public bool delete_temp = true;
	public bool debug = false;
	public bool unsafef = true;
	public bool external = false;
	public bool public_mode = true;
	public bool nostdlib = false;
	public bool? inline_selectors = null;
	public List<string> sources;
	public List<string> resources = new List<string> ();
	public List<string> linkwith = new List<string> ();
	public List<string> api_sources = new List<string> ();
	public List<string> core_sources = new List<string> ();
	public List<string> extra_sources = new List<string> ();
	public List<string> defines = new List<string> ();
	public string? generate_file_list = null;
	public bool process_enums = false;
}
