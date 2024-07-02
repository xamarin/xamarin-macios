//
// Test the generated API selectors against typos or non-existing cases
//
// Authors:
//	Paola Villarreal  <paola.villarreal@xamarin.com>
//
// Copyright 2015 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using ObjCRuntime;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using Foundation;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Introspection {
	public abstract class ApiTypoTest : ApiBaseTest {
		protected ApiTypoTest ()
		{
			ContinueOnFailure = true;
		}

		public virtual bool Skip (Type baseType, string typo)
		{
			return SkipAllowed (baseType.Name, null, typo);
		}

		public virtual bool Skip (MemberInfo methodName, string typo)
		{
			return SkipAllowed (methodName.DeclaringType.Name, methodName.Name, typo);
		}

		readonly HashSet<string> allowedRule3 = new HashSet<string> {
			"IARAnchorCopying", // We're showing a code snippet in the 'Advice' message and that shouldn't end with a dot.
		};

		HashSet<string> allowedMemberRule4 = new HashSet<string> {
			"Platform",
			"PlatformHelper",
			"AvailabilityAttribute",
			"iOSAttribute",
			"MacAttribute",
		};

		HashSet<string> allowed = new HashSet<string> () {
			"Aac",
			"Accurracy",
			"Achivements",
			"Acos",
			"Acosh",
			"Acn",
			"Actionname",
			"Activitiy",
			"Addin",
			"Addl",
			"Addr",
			"Adjustmentfor",
			"Aes", // Advanced Encryption Standard
			"Aifc",
			"Aiff",
			"Agc",
			"Aio",
			"Alg", // short for Algorithm
			"Alpn", // Application-Layer Protocol Negotiation RFC7301
			"Amete",
			"Amr",
			"Anglet",
			"Apng", // Animated Portable Network Graphics
			"Aps",
			"Arraycollation",
			"Argb",
			"Asin",
			"Asinh",
			"Atan",
			"Atanh",
			"Atmos", // Dolby Atmos
			"Ats",	// App Transport Security
			"Attrib",
			"Attributevalue",
			"Attrs", // Attributes (used by Apple for keys)
			"Audiofile",
			"Automapping",
			"Automatch",
			"Automounted",
			"Autoredirect",
			"Autospace",
			"Autostarts",
			"Avci", // file type
			"Avb", // acronym: Audio Video Bridging
			"Aliasable",
			"Arcball",
			"Atm",
			"Avg",
			"Backface",
			"Bancaire", // french
			"Bancaires", // french
			"Bary",
			"Batc",
			"Bgra", // acrnym for Blue, Green, Red, Alpha
			"Bim",
			"Biquad",
			"Bitangent",
			"Blinn",
			"Blit",
			"Bokeh",
			"Bggr", // acronym for Blue, Green, Green, Red 
			"Bsln",
			"Bssid",
			"Btle", // Bluetooth Low Energy
			"Bzip",
			"Cabac",
			"Caf", // acronym: Core Audio Format
			"Cancellable",
			"Cartes", // french
			"Cavlc",
			"Cda", // acronym: Clinical Document Architecture
			"Cdrom",
			"Celu", // Continuously Differentiable Exponential Linear Unit (ML)
			"Chip", // framework name
			"Cfa", // acronym: Color Filter Array
			"Celp", // MPEG4ObjectID
			"Characterteristic",
			"Chapv",
			"Cholesky",
			"Chacha",
			"Chromaticities",
			"Ciexyz",
			"Ciff",
			"Cinepak",
			"Clearcoat",
			"Cnn", // Convolutional Neural Network
			"Cns",
			"Colos",
			"Commerical",
			"Composable",
			"Conflictserror",
			"Connnect",
			"Counterclock",
			"Copyback",
			"Craete",
			"Crosstraining",
			"Cubemap",
			"Cmaf", // Common Media Application Format (mpeg4)
			"Cmy", // acronym: Cyan, magenta, yellow
			"Cmyk", // acronym: Cyan, magenta, yellow and key
			"Daap",
			"Dav",
			"Dcip", // acronym: Digital Cinema Implementation Partners
			"Deca",
			"Decomposables",
			"Deinterlace",
			"Depthwise",
			"Descendents",
			"Descrete",
			"Dhe", // Diffie–Hellman key exchange
			"Diffable", // that you can diff it.. made up word from apple
			"Differental",
			"Diffie",
			"Directionfor",
			"Dist",
			"dlclose",
			"dlerror",
			"Dlfcn",
			"dlopen",
			"Dls",
			"Dlsym",
			"dlsym",
			"Dng",
			"Dns",
			"Dont",
			"Dop",
			"Dopesheet",
			"Downsample",
			"Downmix", // Sound terminology that means making a stereo mix from a 5.1 surround mix.
			"Dpa",
			"Dpad", // Directional pad (D-pad)
			"Dpads", // plural of above
			"Droste",
			"Dtls",
			"Dtmf", // DTMF
			"dy",
			"Eap",
			"Ebu",
			"Ecc",   // Elliptic Curve Cryptography
			"Ecdh",  // Elliptic Curve Diffie–Hellman
			"Ecdhe", // Elliptic Curve Diffie-Hellman Ephemeral
			"Ecdsa", // Elliptic Curve Digital Signature Algorithm
			"Ecies", // Elliptic Curve Integrated Encryption Scheme
			"Ecn",   // Explicit Congestion Notification
			"Ect",   // ECN Capable Transport
			"Editability",
			"Edr",
			"Eof", // acronym End-Of-File
			"Elu",
			"Emagic",
			"Emaili",
			"Embd",
			"Emsg",	// 4cc
			"Enc",
			"Eppc",
			"Epub",
			"Eftpos", // Electronic funds transfer at point of sale
			"Eotf", // DisplayP3_PQ_Eotf
			"Exabits",
			"Exbibits",
			"Exbibytes",
			"Exhange",
			"Exp",
			"Expr",
			"Exr",
			"Felica", // Japanese contactless RFID smart card system
			"Femtowatts",
			"Fhir",
			"Flipside",
			"Formati",
			"Fov",
			"Framebuffer",
			"Framesetter",
			"Froms", // NSMetadataItemWhereFromsKey
			"Freq",
			"Ftps",
			"Func",
			"Gadu",
			"Gbrg",	// acronym for Green-Blue-Reg-Green
			"Gelu", // Gaussian Error Linear Unit (ML)
			"Geocoder",
			"Gigapascals",
			"Gibibits",
			"Gibibytes",
			"Girocard",
			"Glorot", // NN
			"Gop", // acronym for Group Of Pictures
			"Gpp",
			"Gps",
			"Gpu",	// acronym for Graphics Processing Unit
			"Grbg", // acronym for Green-Red-Blue-Green
			"Gru",
			"Greeking",
			"Gtin",
			"Gui",
			"Hardlink",
			"Heics", // High Efficiency Image File Format (Sequence)
			"Hdmi",
			"Hdr",
			"Hectopascals",
			"Heic", // file type
			"Heif", // file type
			"Hevc", // CMVideoCodecType / High Efficiency Video Coding
			"Heif", // High Efficiency Image File Format
			"Hfp",
			"Hipass",
			"Hlg", // Hybrid Log-Gamma
			"Hls",
			"Hoa",
			"Hrtf", // acronym used in AUSpatializationAlgorithm
			"Hvxc", // MPEG4ObjectID
			"Icns",
			"Ico",
			"Ies",
			"Icq",
			"Ident",
			"Identd",
			"Imageblock",
			"Imagefor",
			"Imap",
			"Imaps",
			"Img",
			"Impl", // BindingImplAttribute
			"Inv",
			"Indoorrun",
			"Indoorcycle",
			"Inklist",
			"Indeterm",
			"Indoorwalk",
			"Inode",
			"Inser",
			"Instamatic",
			"Interac",
			"Interframe",
			"Interitem",
			"Intermenstrual",
			"Intersector",
			"Intoi",
			"Invitable",
			"Ios",
			"Iou",
			"Ipa",
			"Ipp",
			"Iptc",
			"Ircs",
			"Iso",
			"Itf",
			"Itu",
			"Itur", // Itur_2020_Hlg
			"Jcb", // Japanese credit card company
			"Jfif",
			"Jis",
			"Json",
			"Keyerror",
			"Keyi",
			"Keypoint",
			"Keypoints",
			"Keyspace",
			"ks",
			"Kibibits",
			"Kibibytes",
			"Kiloampere",
			"Kiloamperes",
			"Kiloohms",
			"Kilopascals",
			"Kullback", // Kullback-Leibler Divergence
			"Langauges",
			"Lacunarity",
			"Latm", //  Low Overhead Audio Transport Multiplex
			"Ldaps",
			"Lerp",
			"Linecap",
			"Lingustic",
			"libcompression",
			"libdispatch",
			"Loas", // Low Overhead Audio Stream 
			"Lod",
			"Lopass",
			"Lowlevel",
			"Lstm",
			"Lun",
			"Luma",
			"Lzfse", // acronym
			"Lzma", // acronym
			"Mada", // payment system
			"Mapbuffer",
			"Matchingcoalesce",
			"Mcp", // metacarpophalangeal (hand)
			"Mebibits",
			"Mebibytes",
			"Megaampere",
			"Megaamperes",
			"Megaliters",
			"Megameters",
			"Megaohms",
			"Megapascals",
			"Metacharacters",
			"Metalness",
			"Metadatas",
			"Microampere",
			"Microamperes",
			"Microohms",
			"Microwatts",
			"Millimoles",
			"Milliohms",
			"Mimap",
			"Minification",
			"Mncs",
			"Mgmt",
			"Mobike", // acronym
			"Morpher",
			"mtouch",
			"Mpe", // acronym
			"Mps",
			"Msaa", // multisample anti-aliasing 
			"Mtu", // acronym
			"Mtc", // acronym
			"Mtgp",
			"Mul",
			"Mult",
			"Multihead",
			"Multipath",
			"Multipeer",
			"Muxed",
			"Nai",
			"Nanograms",
			"Nanowatts",
			"Nestrov",
			"Nesterov",
			"nfloat",
			"Nfnt",
			"nint",
			"Nntps",
			"Ntlm",
			"Nsl", // InternetLocationNslNeighborhoodIcon
			"Ntsc",
			"nuint",
			"Ndef",
			"Noi", // From NoiOSAttribute
			"Nop",
			"Numbernumber",
			"Nyquist",
			"Oaep", // Optimal asymmetric encryption padding
			"Objectfor",
			"Objectness",
			"Occlussion",
			"Ocurrences",
			"Ocsp", // Online Certificate Status Protocol
			"Octree",
			"Oid",
			"Oneup", // TVElementKeyOneupTemplate
			"Organisation", // kCGImagePropertyIPTCExtRegistryOrganisationID in Xcode9.3-b1
			"Orthographyrange",
			"Orth",
			"Osa", // Open Scripting Architecture
			"Otsu", // threshold for image binarization
			"ove",
			"Paeth", // PNG filter
			"Palettize",
			"Parms", // short for Parameters
			"Peap",
			"Pebibits",
			"Pebibytes",
			"Petabits",
			"Perlin",
			"Persistable",
			"Pausable",
			"Pcl",
			"Pcm",
			"Pdu",
			"Persistance",
			"Pesented",
			"Pfs", // acronym
			"Philox",
			"Picometers",
			"Picowatts",
			"Pkcs",
			"Placemark",
			"Playthrough",
			"Pnc", // MIDI
			"Pnorm",
			"Pointillize",
			"Polyline",
			"Polylines",
			"Popularimeter",
			"Preds", // short for Predicates
			"Prerolls",
			"Preseti",
			"Prev",
			"Privs", // SharingPrivsNotApplicableIcon
			"Propogate",
			"Psec",
			"Psm", // Protocol/Service Multiplexer
			"Psk",
			"Ptp",
			"Pvrtc", // MTLBlitOption - PowerVR Texture Compression
			"Qos",
			"Quaterniond",
			"Quadding",
			"Qura",
			"Quic",
			"Reacquirer",
			"Reinvitation",
			"Reinvite",
			"Rel",
			"Relocalization",
			"Relu", // Rectified Linear Unit (ML)
			"Relun", // ReLUn - degree n Hermite coefficients
			"Reprandial",
			"Replayable",
			"Requestwith",
			"Ridesharing",
			"Rgb",
			"Rgba",
			"Rggb", // acronym for Red, Green, Green, Blue
			"Rnn",
			"Roi",
			"Romm", // acronym: Reference Output Medium Metric
			"Rpa",
			"Rpn", // acronym
			"Rsa", // Rivest, Shamir and Adleman
			"Rsqrt", // reciprocal square root
			"Rssi",
			"Rtp",
			"Rtl",
			"Rtsp",
			"Saml", // acronym
			"Sdof",
			"Scn",
			"Sdk",
			"Sdtv", // acronym: Standard Definition Tele Vision
			"Sdnn",
			"Seekable",
			"Selu", // Scaled Exponential Linear unit (ML)
			"Sgd", // Stochastic Gradient Descent (ML)
			"Shadable",
			"Sharegroup",
			"Sha", //  Secure Hash Algorithm
			"Siemen",
			"simd",
			"Sinh",
			"Sint", // as in "Signed Integer"
			"Simd",
			"Slerp",
			"Slomo",
			"Smpte",
			"Snapshotter",
			"Snorm",
			"Sobel",
			"Softmax", // get_SoftmaxNormalization
			"Spacei",
			"Sqrt",
			"Srgb",
			"Ssid",
			"Ssids",
			"Standarize",
			"Stateful",
			"Stateright",
			"Subbeat",
			"Subcaption",
			"Subcardioid",
			"Subentities",
			"Subheadline",
			"Sublocality",
			"Sublocation",
			"Submesh",
			"Submeshes",
			"Subpixel",
			"Subresource",
			"Subresources",
			"Subsec",
			"Suica", // Japanese contactless smart card type
			"Superentity",
			"Supertype",
			"Supertypes",
			"Svg", // Scalable Vector Graphics
			"Sym",
			"Synchronizable",
			"Symbologies",
			"Tanh",
			"Tebibits",
			"Tebibytes",
			"Tensorflow",
			"Tessellator",
			"Texcoord",
			"Texel",
			"th",
			"Threadgroup",
			"Threadgroups",
			"Thumbnailing",
			"Thumbstick",
			"Thumbsticks",
			"Timecodes",
			"Timelapse",
			"Timelapses",
			"Tls",
			"Ttls",
			"Tlv",
			"Toc",
			"Toci",
			"Toi",
			"Transceive",
			"Trc",
			"Truncantion",
			"Tweening",
			"Twips",
			"tx",
			"ty",
			"Udi",
			"Udp",
			"Unconfigured",
			"Undecodable",
			"Unemphasized",
			"Underrun",
			"Unflagged",
			"Unfocusing",
			"Uid",
			"Unmap",
			"Unorm",
			"Unpremultiplied",
			"Unpremultiplying",
			"Unprepare",
			"Unproject",
			"Unpublish",
			"Uterance",
			"Unentitled",
			"Untrash",
			"Utf",
			"Upce",
			"Uri",
			"Usac", // Unified Speech and Audio Coding
			"Usd", // Universal Scene Description
			"Usdz", // USD zip
			"Uti",
			"Varispeed",
			"Vergence",
			"Voronoi",
			"Vnode",
			"Vpn",
			"Warichu",
			"Wep",
			"Wpa",
			"Warpable",
			"Whitespaces",
			"Wifes",
			"Writeability",
			"Xnor",
			"Xpc",
			"xy",
			"Xyz",
			"Xzy",
			"Yobibits",
			"Yobibytes",
			"Yottabits",
			"Yxz",
			"Yzx",
			"Zxy",
			"Zyx",
			"Yuv",
			"Yuvk",
			"yuvs",
			"yx",
			"yy",
			"Yyy",
			"Zebibits",
			"Zebibytes",
			"Zettabits",
			"Zlib",
#if MONOMAC
			"Abbr",
			"Accum",
			"Ack", // TcpSetDisableAckStretching
			"Addin",
			"Addons",
			"Appactive",
			"Approx",
			"Arae",
			"Attr",
			"Attributesfor",
			"Autoresizin",
			"Avc",
			"Callpout",
			"Ccitt",
			"Commited",
			"Constrainted",
			"Ctm",
			"Cymk",
			"Cymka",
			"Cmyka",
			"Compat",
			"Credendtials",
			"Descriptorat",
			"Descriptorfor",
			"Dimensionsfor",
			"Dissapearing",
			"Distinguised", // ITLibPlaylistPropertyDistinguisedKind
			"Dirs",
			"Drm", // MediaItemProperty.IsDrmProtected 
			"Editability",
			"Eisu",
			"Entryat",
			"Equiv",
			"Fourty",
			"Grammarl",
			"Greeking",
			"Hsb",
			"Hsba",
			"Ibss",
			"Iconfor",
			"Incrementor",
			"Indexeffective",
			"Indexestable",
			"Itemto",
			"Lowsrc",
			"Lpcm",
			"Lzw",
			"Mihret",
			"Mps",
			"Nonenumerated",
			"Nsevent",
			"Numberof",
			"Orginal",
			"Parms",
			"Pbm",
			"Pde",
			"Performwith",
			"Phy",
			"Pmgt",
			"Preceeding",
			"Preds",
			"Previewable",
			"Qtvr",
			"Rangewith",
			"Rangeswith",
			"Reassociation",
			"Rectfrom",
			"Registeration",
			"Segmentnew",
			"Semitransient",
			"Sixtyfour",
			"Sourcei",
			"Steppable",
			"Stringto",
			"Succesfully",
			"Supression",
			"Targetand",
			"Tkip",
			"Tsn",
			"Tunesi",
			"Twentyfour",
			"Uneditable",
			"Unfocus",
			"Unpublish",
			"Usec",
			"Usedby",
			"Viewwrite",
			"Wep",
			"Wlan",
			"Wme",
			"Writeln",
			"Xattr",
#endif
#if !NET
			"Actionfrom",
			"Asal", // Typo, should be 'Basal', fixed in 'HKInsulinDeliveryReason'
			"Attributefor",
			"Attributest",
			"Failwith",
			"Imageimage",
			"Libary",
			"Musthold",
			"Olus", // Typo, should be 'Bolus', fixed in 'HKInsulinDeliveryReason'
			"Ostprandial", // Typo, should be 'Postprandial', fixed in 'HKBloodGlucoseMealTime'
			"Pathpath",
			"Rangefor",
			"Reprandial", // Typo, should be 'Preprandial', fixed in 'HKBloodGlucoseMealTime'
			"Failwith",
			"Tearm",
			"Theevent",
			"Timestampe", // Existing binding so we can't just remove it.
			"Toplevel",
			"Tripple",
#endif
		};

		// ease maintenance of the list
		HashSet<string> used = new HashSet<string> ();

		bool SkipAllowed (string typeName, string methodName, string typo)
		{
			if (allowed.Contains (typo)) {
				used.Add (typo);
				return true;
			}
			return false;
		}

		bool IsObsolete (MemberInfo mi)
		{
			if (mi is null)
				return false;
			if (mi.GetCustomAttributes<ObsoleteAttribute> (true).Any ())
				return true;
			if (MemberHasObsolete (mi))
				return true;
			return IsObsolete (mi.DeclaringType);
		}

		[Test]
		public virtual void AttributeTypoTest ()
		{
			var types = Assembly.GetTypes ();
			int totalErrors = 0;
			foreach (Type t in types)
				AttributeTypo (t, ref totalErrors);

			Assert.AreEqual (0, totalErrors, "Attributes have typos!");
		}

		void AttributeTypo (Type t, ref int totalErrors)
		{
			AttributesMessageTypoRules (t, t.Name, ref totalErrors);

			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
			foreach (var f in t.GetFields (flags))
				AttributesMessageTypoRules (f, t.Name, ref totalErrors);

			foreach (var p in t.GetProperties (flags))
				AttributesMessageTypoRules (p, t.Name, ref totalErrors);

			foreach (var m in t.GetMethods (flags))
				AttributesMessageTypoRules (m, t.Name, ref totalErrors);

			foreach (var e in t.GetEvents (flags))
				AttributesMessageTypoRules (e, t.Name, ref totalErrors);

			foreach (var nt in t.GetNestedTypes ())
				AttributeTypo (nt, ref totalErrors);
		}

		[Test]
		public virtual void TypoTest ()
		{
			var types = Assembly.GetTypes ();
			int totalErrors = 0;
			foreach (Type t in types) {
				if (t.IsPublic) {
					if (IsObsolete (t))
						continue;

					string txt = NameCleaner (t.Name);
					var typo = GetCachedTypo (txt);
					if (typo.Length > 0) {
						if (!Skip (t, typo)) {
							ReportError ("Typo in TYPE: {0} - {1} ", t.Name, typo);
							totalErrors++;
						}
					}

					var fields = t.GetFields ();
					foreach (FieldInfo f in fields) {
						if (!f.IsPublic && !f.IsFamily)
							continue;

						if (IsObsolete (f))
							continue;

						txt = NameCleaner (f.Name);
						typo = GetCachedTypo (txt);
						if (typo.Length > 0) {
							if (!Skip (f, typo)) {
								ReportError ("Typo in FIELD name: {0} - {1}, Type: {2}", f.Name, typo, t.Name);
								totalErrors++;
							}
						}
					}

					var methods = t.GetMethods ();
					foreach (MethodInfo m in methods) {
						if (!m.IsPublic && !m.IsFamily)
							continue;

						if (IsObsolete (m))
							continue;

						txt = NameCleaner (m.Name);
						typo = GetCachedTypo (txt);
						if (typo.Length > 0) {
							if (!Skip (m, typo)) {
								ReportError ("Typo in METHOD name: {0} - {1}, Type: {2}", m.Name, typo, t.Name);
								totalErrors++;
							}
						}
#if false
						var parameters = m.GetParameters ();
						foreach (ParameterInfo p in parameters) {
							txt = NameCleaner (p.Name);
							typo = GetCachedTypo (txt);
							if (typo.Length > 0) {
								ReportError ("Typo in PARAMETER Name: {0} - {1}, Method: {2}, Type: {3}", p.Name, typo, m.Name, t.Name);
								totalErrors++;
							}
						}
#endif
					}
				}
			}
#if false
			// ease removal of unrequired values (but needs to be checked for every profile)
			var unused = allowed.Except (used);
			foreach (var typo in unused)
				Console.WriteLine ("Unused entry \"{0}\"", typo);
#endif
			Assert.AreEqual (0, totalErrors, "Typos!");
		}

		string GetMessage (object attribute)
		{
			string message = null;
			if (attribute is AdviceAttribute)
				message = ((AdviceAttribute) attribute).Message;
			if (attribute is ObsoleteAttribute)
				message = ((ObsoleteAttribute) attribute).Message;
#if !NET
			if (attribute is AvailabilityBaseAttribute)
				message = ((AvailabilityBaseAttribute) attribute).Message;
#endif

			return message;
		}

		void AttributesMessageTypoRules (MemberInfo mi, string typeName, ref int totalErrors)
		{
			if (mi is null)
				return;

			foreach (object ca in mi.GetCustomAttributes ()) {
				string message = GetMessage (ca);
				if (message is not null) {
					var memberAndTypeFormat = mi.Name == typeName ? "Type: {0}" : "Member name: {1}, Type: {0}";
					var memberAndType = string.Format (memberAndTypeFormat, typeName, mi.Name);

					// Rule 1: https://github.com/xamarin/xamarin-macios/wiki/BINDINGS#rule-1
					// Note: we don't enforce that rule for the Obsolete (not Obsoleted) attribute since the attribute itself doesn't support versions.
					if (!(ca is ObsoleteAttribute)) {
						var forbiddenOSNames = new [] { "iOS", "watchOS", "tvOS", "macOS" };
						if (forbiddenOSNames.Any (s => Regex.IsMatch (message, $"({s} ?)[0-9]+"))) {
							ReportError ("[Rule 1] Don't put OS information in attribute's message: \"{0}\" - {1}", message, memberAndType);
							totalErrors++;
						}
					}

					// Rule 2: https://github.com/xamarin/xamarin-macios/wiki/BINDINGS#rule-2
					if (message.Contains ('`')) {
						ReportError ("[Rule 2] Replace grave accent (`) by apostrophe (') in attribute's message: \"{0}\" - {1}", message, memberAndType);
						totalErrors++;
					}

					// Rule 3: https://github.com/xamarin/xamarin-macios/wiki/BINDINGS#rule-3
					if (!message.EndsWith (".", StringComparison.Ordinal)) {
						if (!allowedRule3.Contains (typeName)) {
							ReportError ("[Rule 3] Missing '.' in attribute's message: \"{0}\" - {1}", message, memberAndType);
							totalErrors++;
						}
					}

					// Rule 4: https://github.com/xamarin/xamarin-macios/wiki/BINDINGS#rule-4
					if (!allowedMemberRule4.Contains (mi.Name)) {
						var forbiddenAvailabilityKeywords = new [] { "introduced", "deprecated", "obsolete", "obsoleted" };
						if (forbiddenAvailabilityKeywords.Any (s => Regex.IsMatch (message, $"({s})", RegexOptions.IgnoreCase))) {
							ReportError ("[Rule 4] Don't use availability keywords in attribute's message: \"{0}\" - {1}", message, memberAndType);
							totalErrors++;
						}
					}

					var forbiddensWords = new [] { "OSX", "OS X" };
					for (int i = 0; i < forbiddensWords.Length; i++) {
						var word = forbiddensWords [i];
						if (Regex.IsMatch (message, $"({word})", RegexOptions.IgnoreCase)) {
							ReportError ("Don't use {0} in attribute's message: \"{1}\" - {2}", word, message, memberAndType);
							totalErrors++;
						}
					}
				}
			}
		}

		Dictionary<string, string> cached_typoes = new Dictionary<string, string> ();
		string GetCachedTypo (string txt)
		{
			string rv;
			if (!cached_typoes.TryGetValue (txt, out rv))
				cached_typoes [txt] = rv = GetTypo (txt);
			return rv;
		}
		public abstract string GetTypo (string txt);

		static StringBuilder clean = new StringBuilder ();

		static string NameCleaner (string name)
		{
			clean.Clear ();
			foreach (char c in name) {
				if (Char.IsUpper (c)) {
					clean.Append (' ').Append (c);
					continue;
				}
				if (Char.IsDigit (c)) {
					clean.Append (' ');
					continue;
				}
				switch (c) {
				case '<':
				case '>':
				case '_':
					clean.Append (' ');
					break;
				default:
					clean.Append (c);
					break;
				}
			}
			return clean.ToString ();
		}

		bool CheckLibrary (string lib)
		{
#if MONOMAC
			// on macOS the file should exist on the specified path
			// for iOS the simulator paths do not match the strings
			switch (lib) {
			// location changed in 10.8 but it loads fine (and fixing it breaks on earlier macOS)
			case Constants.CFNetworkLibrary:
			// location changed in 10.10 but it loads fine (and fixing it breaks on earlier macOS)
			case Constants.CoreBluetoothLibrary:
			// location changed in 10.11 but it loads fine (and fixing it breaks on earlier macOS)
			case Constants.CoreImageLibrary:
				break;
			default:
				if (TestRuntime.CheckSystemVersion (ApplePlatform.MacOSX, 11, 0)) {
					// on macOS 11.0 the frameworks binary files are not present (cache) but can be loaded
					if (!Directory.Exists (Path.GetDirectoryName (lib)))
						return false;
				} else if (!File.Exists (lib))
					return false;
				break;
			}
#endif
			var h = IntPtr.Zero;
			try {
				h = Dlfcn.dlopen (lib, 0);
				if (h != IntPtr.Zero)
					return true;
#if MONOMAC
				// on macOS it might be wrong architecture
				// i.e. 64 bits only (thin) libraries running on 32 bits process
				if (IntPtr.Size == 4)
					return true;
#endif
			} finally {
				Dlfcn.dlclose (h);
			}
			return false;
		}

		protected void AssertMatchingOSVersionAndSdkVersion ()
		{
			var sdk = new Version (Constants.SdkVersion);
#if MONOMAC
			if (!NSProcessInfo.ProcessInfo.IsOperatingSystemAtLeastVersion (new NSOperatingSystemVersion (sdk.Major, sdk.Minor, sdk.Build == -1 ? 0 : sdk.Build)))
#elif __WATCHOS__
			if (!WatchKit.WKInterfaceDevice.CurrentDevice.CheckSystemVersion (sdk.Major, sdk.Minor))
#else
			if (!UIDevice.CurrentDevice.CheckSystemVersion (sdk.Major, sdk.Minor))
#endif
				Assert.Ignore ($"This test only executes using the latest OS version ({sdk.Major}.{sdk.Minor})");
		}

		[Test]
		public void ConstantsCheck ()
		{
			// The constants are file paths for frameworks / dylibs
			// unless the latest OS is used there's likely to be missing ones
			// so we run this test only on the latest supported (matching SDK) OS
			AssertMatchingOSVersionAndSdkVersion ();

			var c = typeof (Constants);
			foreach (var fi in c.GetFields ()) {
				if (!fi.IsPublic)
					continue;
				var s = fi.GetValue (null) as string;
				switch (fi.Name) {
				case "Version":
				case "SdkVersion":
					Assert.True (Version.TryParse (s, out _), fi.Name);
					break;
#if !XAMCORE_5_0
				case "AssetsLibraryLibrary":
				case "NewsstandKitLibrary": // Removed from iOS, but we have to keep the constant around for binary compatibility.
					break;
#endif
#if !NET
#if __TVOS__
				case "PassKitLibrary": // not part of tvOS
					break;
#endif
				case "libcompression": // bad (missing) suffix
					Assert.True (CheckLibrary (s), fi.Name);
					break;
#endif
				case "ChipLibrary": // Chip is removed entirely beginning Xcode 14
					if (!TestRuntime.CheckXcodeVersion (14, 0))
						if (TestRuntime.IsDevice)
							Assert.True (CheckLibrary (s), fi.Name);
					break;
#if !__MACOS__
				case "CinematicLibrary":
				case "ThreadNetworkLibrary":
				case "MediaSetupLibrary":
				case "MLComputeLibrary":
					// Xcode 12 beta 2 does not ship these framework/headers for the simulators
					if (TestRuntime.IsDevice)
						Assert.True (CheckLibrary (s), fi.Name);
					break;
#endif
#if __TVOS__
				case "MetalPerformanceShadersLibrary":
				case "MetalPerformanceShadersGraphLibrary":
					// not supported in tvOS (12.1) simulator so load fails
					if (TestRuntime.IsSimulatorOrDesktop)
						break;
					goto default;
				case "PhaseLibrary":
					// framework support for tvOS was added in xcode 15
					// but not supported on tvOS simulator so load fails
					if (TestRuntime.IsSimulatorOrDesktop)
						break;
					goto default;
#endif
				case "MetalFXLibrary":
#if __TVOS__
					goto default;
#else
					if (TestRuntime.IsSimulatorOrDesktop)
						break;
					goto default;
#endif
				default:
					if (fi.Name.EndsWith ("Library", StringComparison.Ordinal)) {
#if __IOS__
						if (fi.Name == "CoreNFCLibrary") {
							// NFC is currently not available on iPad
							if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
								continue;
							// Phone works unless Xcode 12 on simulator
							if (TestRuntime.IsSimulatorOrDesktop && TestRuntime.CheckXcodeVersion (12, 0))
								continue;
						}
#endif
#if __MACOS__
						// Only available in macOS 10.15.4+
						if (fi.Name == "AutomaticAssessmentConfigurationLibrary" && !TestRuntime.CheckXcodeVersion (11, 4))
							continue;
#endif
#if __WATCHOS__
						// added with watchOS 4 (mistake)
						if (fi.Name == "VisionLibrary")
							continue;
#endif
						Assert.True (CheckLibrary (s), fi.Name);
					} else {
						Assert.Fail ($"Unknown '{fi.Name}' field cannot be verified - please fix me!");
					}
					break;
				}
			}
		}
	}
}
