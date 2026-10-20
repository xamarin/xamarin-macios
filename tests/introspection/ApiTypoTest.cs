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


using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
#if MONOMAC
using AppKit;
using SpellChecker = AppKit.NSSpellChecker;
#else
using UIKit;
using SpellChecker = UIKit.UITextChecker;
#endif
using Xamarin.Tests;
using Xamarin.Utils;

#nullable enable

namespace Introspection {
	[TestFixture]
	public class ApiTypoTest : ApiBaseTest {
		const ApplePlatform All = ApplePlatform.MacOSX | ApplePlatform.iOS | ApplePlatform.TVOS | ApplePlatform.MacCatalyst;

		public ApiTypoTest ()
		{
			ContinueOnFailure = true;
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

		Dictionary<string, ApplePlatform> allowed = new Dictionary<string, ApplePlatform> () {
			{ "Aac", All }, // Advanced Audio Coding
			{ "Abgr", All }, // alpha-blue-green-red
			{ "Achivements", All }, // Apple API spelling
			{ "Ack", All }, // acknowledgment
			{ "Acn", All }, // Ambisonic Channel Numbering
			{ "Acos", All }, // arc cosine
			{ "Acosh", All }, // inverse hyperbolic cosine
			{ "Activatable", All }, // valid English derivative
			{ "Addin", All }, // compound word
			{ "Addl", All }, // additional abbreviation
			{ "Addons", ApplePlatform.MacOSX }, // compound word
			{ "Addr", All }, // address abbreviation
			{ "Adessive", All }, // linguistic case
			{ "Adposition", All }, // linguistic term
			{ "Aes", All }, // Advanced Encryption Standard
			{ "Afi", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Apple API abbreviation
			{ "Agc", All }, // automatic gain control
			{ "Ahap", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // Apple Haptics Pattern
			{ "Aifc", All }, // AIFF-C audio format
			{ "Aiff", All }, // Audio Interchange File
			{ "Aime", All }, // AVMetadataIdentifierQuickTimeMetadataAIMEData
			{ "Aio", ApplePlatform.MacOSX }, // all-in-one abbreviation
			{ "Alg", All }, // short for Algorithm
			{ "Alem", All }, // Ethiopic "Amete Alem" calendar
			{ "Aliasable", All }, // valid English derivative
			{ "Allative", All }, // linguistic case
			{ "Amete", All }, // Ethiopic calendar term
			{ "Amr", All }, // Adaptive Multi-Rate
			{ "Ancs", All & ~ApplePlatform.MacOSX }, // Apple Notification Center
			{ "Ane", All }, // Apple Neural Engine
			{ "Anglet", All }, // measurement term
			{ "Apac", All }, // Asia-Pacific abbreviation
			{ "Apdu", All }, // smart-card protocol unit
			{ "Apl", All & ~ApplePlatform.TVOS }, // APL language acronym
			{ "Apng", All }, // Animated Portable Network Graphics
			{ "Apns", All & ~ApplePlatform.TVOS }, // Apple Push Notification Service
			{ "Applei", All }, // Apple API selector fragment
			{ "Aps", ApplePlatform.MacOSX }, // Apple Push Service
			{ "Apv", ApplePlatform.MacOSX }, // Apple API abbreviation
			{ "Arcball", All }, // 3D rotation control
			{ "Argb", All }, // alpha-red-green-blue
#if !XAMCORE_5_0
			{ "Arraycollation", All & ~ApplePlatform.MacOSX }, // SortedArrayFromArraycollationStringSelector - will be renamed in XAMCORE_5_0
#endif
			{ "Asin", All }, // arc sine
			{ "Asinh", All }, // inverse hyperbolic sine
			{ "Astc", All }, // Adaptive Scalable Texture Compression
			{ "Atan", All }, // arc tangent
			{ "Atanh", All }, // inverse hyperbolic tangent
			{ "Atm", All }, // asynchronous transfer mode
			{ "Atmos", All }, // Dolby Atmos
			{ "Atr", All }, // Answer To Reset
			{ "Ats", All }, // App Transport Security
			{ "Atsc", All }, // TV broadcast standard
			{ "Attr", ApplePlatform.MacOSX }, // attribute abbreviation
			{ "Attrib", All }, // attribute abbreviation
#if !XAMCORE_5_0
			{ "Attributevalue", All }, // ReplacementValueForAttributevalue - will be renamed in XAMCORE_5_0
#endif
			{ "Attrs", All }, // Attributes (used by Apple for keys)
			{ "Audiofile", All }, // compound word
			{ "Audiograph", ApplePlatform.MacOSX }, // compound word
			{ "Authenticatable", ApplePlatform.MacOSX }, // valid English derivative
			{ "Automapping", All }, // compound word
			{ "Automatch", All }, // compound word
			{ "Automounted", All }, // compound word
			{ "Autoredirect", ApplePlatform.MacCatalyst | ApplePlatform.TVOS }, // compound word
			{ "Autospace", ApplePlatform.MacOSX }, // compound word
			{ "Autostarts", ApplePlatform.MacOSX }, // compound word
			{ "Avb", All }, // acronym: Audio Video Bridging
			{ "Avci", All }, // file type
			{ "Avg", All }, // average abbreviation
			{ "Axept", All & ~ApplePlatform.TVOS }, // payment terminal brand
			{ "Bancomat", All & ~ApplePlatform.TVOS }, // Italian payment network
			{ "Bancaires", All & ~ApplePlatform.TVOS }, // Cartes Bancaires payment network
			{ "Bary", All }, // barycentric abbreviation
			{ "Ber", All }, // Basic Encoding Rules
			{ "Bggr", All }, // acronym for Blue, Green, Green, Red
			{ "Bgra", All }, // acrnym for Blue, Green, Red, Alpha
			{ "Bgrx", All }, // blue-green-red-unused
			{ "Bim", All }, // BIM file/code term
			{ "Bitangent", All }, // geometry term
			{ "Blinn", All }, // Blinn shading model
			{ "Blit", All }, // graphics copy operation
			{ "Blockmap", ApplePlatform.MacOSX }, // compound word
			{ "Blockquote", ApplePlatform.MacOSX }, // HTML element name
			{ "Brotli", All }, // compression format
			{ "Bsd", ApplePlatform.MacOSX }, // Berkeley Software Distribution
			{ "Bsln", All }, // OpenType baseline tag
			{ "Bssid", All & ~ApplePlatform.TVOS }, // wireless network identifier
			{ "Btle", ApplePlatform.MacOSX }, // Bluetooth Low Energy
			{ "Bto", All }, // CIConvertRGBtoLab follows the native filter name
			{ "Cabac", All }, // video coding acronym
			{ "Caf", All }, // acronym: Core Audio Format
			{ "Callables", All }, // valid English plural
			{ "Cartes", All & ~ApplePlatform.TVOS }, // french
			{ "Catmull", All }, // Catmull-Rom term
			{ "Cavlc", All }, // video coding acronym
			{ "Ccitt", ApplePlatform.MacOSX }, // standards body acronym
			{ "Cbc", All }, // Cipher Block Chaining
			{ "Cct", All }, // correlated color temperature
			{ "Ccw", All }, // counterclockwise abbreviation
			{ "Cda", All & ~ApplePlatform.TVOS }, // acronym: Clinical Document Architecture
			{ "Cdf", All }, // Cumulative Distribution Function
			{ "Cdma", All }, // cellular standard
			{ "Cdrom", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // Compact Disc Read-Only Memory
			{ "Cea", All }, // standards acronym
			{ "Celp", All }, // MPEG4ObjectID
			{ "Celu", All }, // Continuously Differentiable Exponential Linear Unit (ML)
			{ "Cfa", All }, // acronym: Color Filter Array
			{ "Chacha", All }, // ChaCha cipher
			{ "Chapv", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // CHAP variant acronym
#if !XAMCORE_5_0
			{ "Characterteristic", All }, // UpdatedCharacterteristicValue - will be renamed in XAMCORE_5_0
#endif
			{ "Cholesky", All }, // matrix decomposition
			{ "Chromaticities", All }, // color science term
			{ "Chw", All }, // tensor layout abbrev
			{ "Ciexyz", ApplePlatform.MacOSX }, // CIE XYZ color space
			{ "Cif", All }, // Common Intermediate Format
			{ "Ciff", All }, // Canon image format
			{ "Cinemagraph", ApplePlatform.TVOS }, // still-photo animation
			{ "Cinepak", All }, // video codec
			{ "Ciphersuite", All }, // TLS cipher suite
			{ "Cla", All }, // command class byte
			{ "Clearcoat", All }, // material property
			{ "Clockstamp", All }, // timestamp variant
			{ "Cmaf", All }, // Common Media Application Format (mpeg4)
			{ "Cmy", ApplePlatform.MacOSX }, // acronym: Cyan, magenta, yellow
			{ "Cmyk", All }, // acronym: Cyan, magenta, yellow and key
			{ "Cmyka", ApplePlatform.MacOSX }, // CMYK plus alpha
			{ "Cnn", All }, // Convolutional Neural Network
			{ "Cns", ApplePlatform.MacOSX }, // Chinese National Standard
			{ "Codabar", All }, // barcode symbology
#if !XAMCORE_5_0
			{ "Commited", ApplePlatform.MacOSX }, // CommitedLoad - will be renamed in XAMCORE_5_0
#endif
			{ "Conf", All }, // configuration abbreviation
			{ "Conecs", All & ~ApplePlatform.TVOS }, // card network acronym
			{ "Conv", All }, // convolution abbreviation
			{ "Cooldown", All & ~ApplePlatform.TVOS }, // compound word
			{ "Copyback", All }, // storage operation
			{ "Cose", All & ~ApplePlatform.TVOS }, // CBOR Object Signing and Encryption
			{ "Crosstraining", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // compound word
			{ "Csr", All }, // certificate signing request
			{ "Csc", All }, // cosecant
			{ "Ctm", ApplePlatform.MacOSX }, // current transform matrix
			{ "Ctor", All }, // constructor abbreviation
			{ "Cubemap", All }, // graphics texture type
			{ "Cymk", ApplePlatform.MacOSX }, // CMYK channel order
			{ "Cymka", ApplePlatform.MacOSX }, // CMYK plus alpha
			{ "Daap", All }, // Digital Audio Access Protocol
			{ "Dangi", All }, // Korean calendar
			{ "Dankort", All & ~ApplePlatform.TVOS }, // Danish payment card
			{ "Dav", All & ~ApplePlatform.TVOS }, // DAV protocol term
			{ "Dcip", All }, // acronym: Digital Cinema Implementation Partners
			{ "Deca", All & ~ApplePlatform.TVOS }, // metric prefix
			{ "Decomposables", All }, // valid English plural
			{ "Deinterlace", All }, // video processing term
			{ "Denoise", All }, // noise reduction term
			{ "Denoised", All }, // noise reduction term
			{ "Denoiser", All }, // noise reduction filter
			{ "Depthwise", All }, // ML convolution term
			{ "Dequantize", All }, // signal processing term
			{ "Despeckle", All }, // image noise correction term
			{ "Dfsi", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Apple API abbreviation
			{ "Dhe", All }, // Diffie–Hellman key exchange
			{ "Dhs", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Apple API abbreviation
			{ "Dhwio", All }, // tensor layout abbrev
			{ "Dicom", All }, // medical imaging standard
			{ "Diconnection", All }, // Apple API spelling
			{ "Diffable", All }, // that you can diff it.. made up word from apple
			{ "Diffie", All }, // Diffie-Hellman name
			{ "Dirbursement", All & ~ApplePlatform.TVOS }, // Apple API spelling
			{ "Dirs", ApplePlatform.MacOSX }, // directories abbreviation
			{ "Dismissable", ApplePlatform.MacOSX }, // variant of dismissible
			{ "Dist", All }, // distance abbreviation
			{ "Distinguised", ApplePlatform.MacOSX }, // ITLibPlaylistPropertyDistinguisedKind
			{ "dlclose", All }, // POSIX dynamic loader API
			{ "dlerror", All }, // POSIX dynamic loader API
#if !XAMCORE_5_0
			{ "Directionfor", All & ~ApplePlatform.MacOSX }, // SetBaseWritingDirectionforRange - will be renamed in XAMCORE_5_0
#endif
			{ "Dlfcn", All }, // dynamic loader header
			{ "Dls", ApplePlatform.MacOSX }, // Downloadable Sounds
			{ "Dng", All }, // Digital Negative format
			{ "Dnssec", All }, // DNS Security Extensions
			{ "Dont", All }, // selector fragment without apostrophe
			{ "Dop", All }, // data object parameter
			{ "Dopesheet", All }, // animation timeline view
			{ "Downmix", All }, // Sound terminology that means making a stereo mix from a 5.1 surround mix.
			{ "Dpa", All }, // Apple API abbreviation
			{ "Dpad", All }, // Directional pad (D-pad)
			{ "Dpads", All }, // plural of above
			{ "Drm", ApplePlatform.MacOSX }, // MediaItemProperty.IsDrmProtected
			{ "Droste", All }, // Droste effect name
			{ "Dsf", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Apple API abbreviation
			{ "Dsfi", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Apple API abbreviation
			{ "Dstu", All & ~ApplePlatform.TVOS }, // Ukrainian standard acronym
			{ "Dtls", All }, // Datagram TLS
			{ "Dtmf", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // DTMF
			{ "Dtss", ApplePlatform.MacOSX }, // audio format acronym
			{ "dy", All }, // tensor dimension symbol
			{ "Eap", All }, // Extensible Authentication Protocol
			{ "Ean", All }, // European Article Number (barcode standard)
			{ "Ebu", All }, // European Broadcasting Union
			{ "Ecc", All }, // Elliptic Curve Cryptography
			{ "Ecdh", All }, // Elliptic Curve Diffie–Hellman
			{ "Ecdhe", All }, // Elliptic Curve Diffie-Hellman Ephemeral
			{ "Ecdsa", All }, // Elliptic Curve Digital Signature Algorithm
			{ "Ecg", All & ~ApplePlatform.TVOS }, // electrocardiogram
			{ "Echos", ApplePlatform.MacOSX }, // plural of echo
			{ "Ecies", All }, // Elliptic Curve Integrated Encryption Scheme
			{ "Ecn", All }, // Explicit Congestion Notification
			{ "Ect", All }, // ECN Capable Transport
			{ "Editability", All & ~ApplePlatform.MacOSX }, // valid English noun
			{ "Edr", All }, // extended dynamic range
			{ "Eftpos", All & ~ApplePlatform.TVOS }, // Electronic funds transfer at point of sale
			{ "Eisu", ApplePlatform.MacOSX }, // Japanese input mode
			{ "Elative", All }, // linguistic case
			{ "Elcard", All & ~ApplePlatform.TVOS }, // Kyrgyz payment network
			{ "Elu", All }, // activation function
			{ "Emagic", All }, // audio software brand
			{ "Embd", All }, // embedded abbreviation
#if !XAMCORE_5_0
			{ "Emebedding", All }, // NLContextualEmebeddingKey - will be renamed in XAMCORE_5_0
#endif
			{ "Emsg", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // 4cc
			{ "Enc", All }, // encoding abbreviation
			{ "Endc", All }, // 5G dual connectivity
			{ "Eof", All }, // acronym End-Of-File
			{ "Eppc", All }, // program-to-program comms
			{ "Epub", All }, // ebook file format
			{ "Erf", All }, // Ericsson Texture Compression format
			{ "Essive", All }, // linguistic case
			{ "Evdo", All }, // cellular data standard
			{ "Evictable", ApplePlatform.MacOSX | ApplePlatform.iOS }, // valid English derivative
			{ "Exabits", All }, // SI unit name
			{ "Exbibits", All }, // IEC unit name
			{ "Exbibytes", All }, // IEC unit name
			{ "Exp", All }, // exponent/exponential
			{ "Expr", All }, // expression abbreviation
			{ "Exr", All }, // OpenEXR image format
			{ "Extrinsics", All }, // computer vision term
			{ "Fcp", All }, // Apple ATS Forward Compatibility Policy
			{ "Feli", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // FeliCa term
			{ "Felica", All & ~ApplePlatform.TVOS }, // Japanese contactless RFID smart card system
			{ "Femtowatts", All }, // SI unit name
			{ "Fft", All }, // Fast Fourier Transform
			{ "Fhir", All & ~ApplePlatform.TVOS }, // healthcare data standard
			{ "Fieldset", All & ~ApplePlatform.MacCatalyst }, // HTML element name
			{ "Formati", All }, // FormatiTunesMetadata - word split of "Format" + "iTunes"
			{ "Fov", All }, // field of view
			{ "Fqdns", All }, // fully qualified domain names
			{ "Framebuffer", All }, // graphics buffer
			{ "Framesetter", All }, // Core Text type
			{ "Freq", All }, // frequency abbreviation
			{ "Froms", ApplePlatform.MacOSX }, // NSMetadataItemWhereFromsKey
			{ "Ftps", All }, // FTP over TLS
			{ "Func", All }, // function abbreviation
			{ "Gadu", All & ~ApplePlatform.TVOS }, // Gadu-Gadu messenger
			{ "Gainmap", All }, // HDR image term
			{ "Gbrg", All }, // acronym for Green-Blue-Reg-Green
			{ "Gbtac", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Apple API abbreviation
			{ "Gbtdc", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Apple API abbreviation
			{ "Gcm", All }, // Galois/Counter Mode
			{ "Gelu", All }, // Gaussian Error Linear Unit (ML)
			{ "Gen", All }, // generation (e.g. SiriRemote1stGen)
			{ "Genlock", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // generator locking synchronization
			{ "Gibibits", All }, // IEC unit name
			{ "Gid", ApplePlatform.MacOSX }, // group identifier
			{ "Gigapascals", All }, // SI unit name
			{ "Girocard", All & ~ApplePlatform.TVOS }, // German payment network
			{ "Gles", ApplePlatform.iOS | ApplePlatform.TVOS }, // OpenGL ES abbreviation
			{ "Glorot", All }, // NN
			{ "Gop", All }, // acronym for Group Of Pictures
			{ "Gpp", All }, // 3GPP standards acronym
			{ "Gps", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // Global Positioning System
			{ "Gsm", All }, // Global System for Mobile Communications
			{ "Grbg", All }, // acronym for Green-Red-Blue-Green
			{ "Groupless", All & ~ApplePlatform.TVOS }, // valid English derivative
			{ "Gru", All }, // gated recurrent unit
			{ "Gtin", All }, // Global Trade Item Number
			{ "Gui", All }, // graphical user interface
			{ "Handwashing", All & ~ApplePlatform.TVOS }, // compound word
			{ "Hankaku", All & ~ApplePlatform.MacOSX }, // Japanese half-width text
			{ "Hardlink", ApplePlatform.MacOSX }, // filesystem term
			{ "Hdmi", All & ~ApplePlatform.MacOSX }, // video connector standard
			{ "Hdr", All }, // high dynamic range
			{ "Heic", All }, // file type
			{ "Heics", All }, // High Efficiency Image File Format (Sequence)
			{ "Heif", All }, // High Efficiency Image File Format
			{ "Hectopascals", All }, // SI unit name
			{ "Hevc", All }, // CMVideoCodecType / High Efficiency Video Coding
			{ "Hfp", All & ~ApplePlatform.MacOSX }, // Bluetooth Hands-Free Profile
			{ "Hhr", All }, // Apple API abbreviation
			{ "Himyan", All & ~ApplePlatform.TVOS }, // South Arabian script
			{ "Humo", All & ~ApplePlatform.TVOS }, // Uzbek payment network
			{ "Hermitean", All }, // Apple's spelling of Hermitian in MPSGraph FFT methods
			{ "Hindlegs", All }, // compound word
			{ "Hipass", All }, // high-pass filter
			{ "Histogrammed", All & ~ApplePlatform.TVOS }, // valid technical term
			{ "Hlg", All }, // Hybrid Log-Gamma
			{ "Hls", All }, // HTTP Live Streaming
			{ "Hoa", All }, // higher-order ambisonics
			{ "Hov", ApplePlatform.iOS }, // high-occupancy vehicle
			{ "Hpke", ApplePlatform.MacOSX }, // Hybrid Public Key Encryption
			{ "Hrtf", All }, // acronym used in AUSpatializationAlgorithm
			{ "Hsb", ApplePlatform.MacOSX }, // hue-saturation-brightness
			{ "Hsba", ApplePlatform.MacOSX }, // HSB plus alpha
			{ "Hvxc", All }, // MPEG4ObjectID
			{ "Hwc", All }, // tensor layout abbrev
			{ "Hwio", All }, // tensor layout abbrev
			{ "Iap", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // in-app purchase
			{ "Ibss", ApplePlatform.MacOSX }, // Wi-Fi ad hoc mode
			{ "Icns", All }, // Apple icon format
			{ "Ico", All }, // Windows icon format
			{ "Icq", All & ~ApplePlatform.TVOS }, // ICQ messenger name
			{ "Identd", All }, // ident daemon name
			{ "Ident", All }, // identifier abbreviation
			{ "Iec", All }, // International Electrotechnical Commission
			{ "Ies", All }, // lighting data format
			{ "Ikev", All }, // Internet Key Exchange v2
			{ "Ima", All }, // Interactive Multimedia Association
			{ "Imageblock", All }, // compound word
			{ "Imap", All }, // Internet Message Access Protocol
			{ "Imaps", All }, // IMAP over TLS
			{ "Imei", All & ~ApplePlatform.MacOSX }, // device identifier acronym
			{ "Img", All }, // image abbreviation
			{ "Impl", All }, // BindingImplAttribute
			{ "Incrementor", ApplePlatform.MacOSX }, // valid English noun
			{ "Indoorcycle", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // compound word
			{ "Indoorrun", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // compound word
			{ "Indoorwalk", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // compound word
			{ "Inessive", All }, // linguistic case
			{ "Ingles", All }, // El Corte Ingles = Spanish payment card
			{ "Inklist", All }, // compound word
			{ "Inode", ApplePlatform.MacOSX }, // filesystem metadata term
			{ "Inser", All }, // Apple API selector fragment
			{ "Instamatic", ApplePlatform.MacOSX }, // Kodak camera brand
			{ "Interac", All & ~ApplePlatform.TVOS }, // Canadian payment network
			{ "Interactable", ApplePlatform.MacOSX }, // valid English derivative
			{ "Interframe", All }, // video coding term
			{ "Interitem", All }, // compound word
			{ "Intermenstrual", All & ~ApplePlatform.TVOS }, // medical term
			{ "Intravaginal", All & ~ApplePlatform.TVOS }, // medical term
			{ "Inv", All }, // inverse abbreviation
			{ "Invitable", All }, // valid English derivative
			{ "Iou", All }, // IOU abbreviation
			{ "Ipa", All }, // International Phonetic Alphabet
			{ "Ipp", All }, // Internet Printing Protocol
			{ "Iptc", All }, // photo metadata standard
			{ "Ircs", All }, // remote control standard
			{ "Isrc", All }, // recording code standard
			{ "Itf", All }, // Interleaved 2 of 5
			{ "Itt", All & ~ApplePlatform.TVOS }, // International Tape Association
			{ "Itu", All }, // International Telecommunication Union
			{ "Itur", All }, // Itur_2020_Hlg
			{ "Jaywan", All & ~ApplePlatform.TVOS }, // Jordanian payment network
			{ "Jcb", All & ~ApplePlatform.TVOS }, // Japanese credit card company
			{ "Jfif", All }, // JPEG File Interchange Format
			{ "Jis", ApplePlatform.MacOSX }, // Japanese Industrial Standards
			{ "Jrts", All & ~ApplePlatform.TVOS }, // Apple API abbreviation
			{ "Jws", ApplePlatform.MacOSX }, // JSON Web Signature
			{ "Jwks", ApplePlatform.MacOSX }, // JSON Web Key Set
			{ "Jwt", ApplePlatform.MacOSX }, // JSON Web Token
			{ "Keepalive", All }, // networking compound word
			{ "Keycode", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // compound word
			{ "Keyerror", All }, // compound word
			{ "Keyi", All }, // Apple API selector fragment
			{ "Keypath", ApplePlatform.MacOSX }, // compound word
			{ "Keypoint", All }, // computer vision term
			{ "Keypoints", All }, // computer vision term
			{ "Kibibits", All }, // IEC unit name
			{ "Kickboard", All & ~ApplePlatform.TVOS }, // compound word
			{ "Kiloampere", All }, // SI unit name
			{ "Kiloamperes", All }, // SI unit name
			{ "Kiloohms", All }, // SI unit name
			{ "Kilopascals", All }, // SI unit name
			{ "Koio", ApplePlatform.MacOSX }, // Kernel Offloaded I/O
			{ "ks", All }, // word fragment from spell checker
			{ "Kullback", All }, // Kullback-Leibler Divergence
			{ "Lacunarity", All }, // fractal geometry term
			{ "Latm", All }, //  Low Overhead Audio Transport Multiplex
			{ "Lbc", All }, // audio codec acronym
			{ "Ldaps", All }, // LDAP over TLS
			{ "Leibler", All }, // Kullback-Leibler divergence
			{ "Lerp", All }, // linear interpolation
			{ "libcompression", All }, // Apple library name
			{ "libdispatch", All }, // Apple library name
			{ "Lingustic", All }, // Apple API spelling
			{ "Lite", All }, // lightweight variant
			{ "Loas", All }, // Low Overhead Audio Stream
			{ "Lod", All }, // level of detail
			{ "Lopass", All }, // low-pass filter
			{ "Lowlevel", All }, // compound word
			{ "Lpcm", All }, // Linear PCM audio
			{ "Lsb", All }, // Least Significant Bit
			{ "Lstm", All }, // long short-term memory
			{ "Lte", All }, // Long-Term Evolution
			{ "Ltp", All }, // AAC Long Term Prediction
			{ "Ltr", All }, // left-to-right abbreviation
			{ "Luma", All }, // luminance component in video
			{ "Lun", All }, // logical unit number
			{ "Lut", All }, // lookup table
			{ "Lzfse", All }, // acronym
			{ "Lzma", All }, // acronym
			{ "Lzw", ApplePlatform.MacOSX }, // Lempel-Ziv-Welch
			{ "Maal", All & ~ApplePlatform.TVOS }, // Maldivian payment network
			{ "Mada", All & ~ApplePlatform.TVOS }, // payment system
			{ "Mcp", All }, // metacarpophalangeal (hand)
			{ "Mebibits", All }, // IEC unit name
			{ "Mebx", All }, // image metadata box
			{ "Meeza", All & ~ApplePlatform.TVOS }, // Egyptian payment network
			{ "Megaampere", All }, // SI unit name
			{ "Megaamperes", All }, // SI unit name
			{ "Megaliters", All }, // SI unit name
			{ "Megameters", All }, // SI unit name
			{ "Megaohms", All }, // SI unit name
			{ "Megapascals", All }, // SI unit name
			{ "Mennekes", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // EV connector brand
			{ "Metacharacters", All }, // regex syntax term
			{ "Metadatas", All }, // valid technical plural
			{ "Metalness", All }, // PBR material property
			{ "Mgmt", All }, // management abbreviation
			{ "Microampere", All }, // SI unit name
			{ "Microamperes", All }, // SI unit name
			{ "Microohms", All }, // SI unit name
			{ "Microwatts", All }, // SI unit name
			{ "Mifare", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // NFC card brand
			{ "Mihret", All }, // Ethiopic "Amete Mihret" calendar
			{ "Millimoles", All }, // SI unit name
			{ "Milliohms", All }, // SI unit name
			{ "Minification", All }, // graphics scaling term
			{ "Mmw", All }, // millimeter wave
			{ "Mncs", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Apple API abbreviation
			{ "Mobike", All }, // acronym
			{ "Monoline", All & ~ApplePlatform.TVOS }, // single-stroke design term
			{ "Morpher", All }, // graphics/animation term
			{ "Mpe", All }, // acronym
			{ "Mps", All }, // metal performance shaders
			{ "Msaa", All }, // multisample anti-aliasing
			{ "Msb", All }, // Most Significant Bit
			{ "Msi", All }, // installer package format
			{ "Mtc", All }, // acronym
			{ "Mtgp", All }, // PRNG algorithm name
			{ "Mtl", All }, // Metal framework prefix
			{ "Mtu", All }, // acronym
			{ "Muid", All & ~ApplePlatform.TVOS }, // MIDI universal identifier
			{ "Mul", All }, // multiply abbreviation
			{ "Mult", All }, // multiply abbreviation
			{ "Multiary", All }, // math/logic term
			{ "Multipath", All }, // networking term
			{ "Multipeer", All }, // Apple framework term
			{ "Multiscript", All }, // compound word
			{ "Multiselect", All & ~ApplePlatform.MacOSX }, // compound word
			{ "Multivariant", All }, // valid technical term
			{ "Multiview", All }, // graphics term
			{ "Muxed", All }, // multiplexed media term
			{ "Nacs", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // EV charging standard
			{ "Nai", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // network access identifier
			{ "Nal", All }, // Network Abstraction Layer (video coding)
			{ "Nanaco", All & ~ApplePlatform.TVOS }, // Japanese payment card
			{ "Nand", All }, // flash memory type
			{ "Nanograms", All }, // SI unit name
			{ "Nanowatts", All }, // SI unit name
			{ "Napas", All & ~ApplePlatform.TVOS }, // Vietnamese payment network
			{ "Ncdhw", All }, // tensor layout abbrev
			{ "Nchw", All }, // tensor layout abbrev
			{ "nd", All }, // tensor dimension symbol
			{ "Ndef", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // NFC data exchange format
			{ "Ndhwc", All }, // tensor layout abbrev
			{ "Nesterov", All }, // optimization method
			{ "Nestrov", All }, // Apple API spelling
			{ "Nfc", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // Near Field Communication
			{ "Nfnt", All }, // classic Mac font format
			{ "Nhwc", All }, // tensor layout abbrev
			{ "Nntps", All }, // NNTP over TLS
			{ "Nonenumerated", ApplePlatform.MacOSX }, // valid English derivative
			{ "Noninteractive", All & ~ApplePlatform.TVOS }, // valid English derivative
			{ "Noop", All }, // no-op abbreviation
			{ "Nop", ApplePlatform.MacOSX }, // no-op instruction
			{ "Nsa", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // network service access
			{ "Nsevent", ApplePlatform.MacOSX }, // Apple API class name
			{ "Nsl", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // InternetLocationNslNeighborhoodIcon
			{ "Ntlm", All }, // Windows auth protocol
			{ "Ntsc", All }, // video standard acronym
			{ "Nyquist", All & ~ApplePlatform.MacOSX }, // sampling theorem term
			{ "Oaep", All }, // Optimal Asymmetric Encryption Padding
			{ "Objectness", All }, // ML detection score
			{ "Ocr", All }, // optical character recognition
			{ "Ocsp", All }, // Online Certificate Status Protocol
			{ "Octree", All }, // spatial partition tree
			{ "Odia", All }, // Indic language name
			{ "Ohwi", All }, // tensor layout abbrev
			{ "Oid", All }, // object identifier
			{ "Oidhw", All }, // tensor layout abbrev
			{ "Oihw", All }, // tensor layout abbrev
			{ "Onnx", All }, // Open Neural Network Exchange
			{ "Ootf", All }, // Opto-Optical Transfer Function (HDR)
			{ "Oper", All & ~ApplePlatform.MacOSX }, // operator abbreviation
			{ "Organisation", All }, // kCGImagePropertyIPTCExtRegistryOrganisationID in Xcode9.3-b1
			{ "Orth", All }, // orthographic abbreviation
			{ "Osa", All }, // Open Scripting Architecture
			{ "Otsu", All }, // threshold for image binarization
			{ "ove", All }, // word fragment from spell checker
			{ "Overline", All & ~ApplePlatform.TVOS }, // typography term
			{ "Paeth", All }, // PNG filter
			{ "Pak", All & ~ApplePlatform.TVOS }, // PayPak, Pakistani payment network
			{ "Palettize", All }, // graphics term
			{ "Parms", All }, // parameters abbreviation
			{ "Pausable", All }, // valid English derivative
			{ "Pbm", ApplePlatform.MacOSX }, // Portable Bitmap format
			{ "Pci", All & ~ApplePlatform.MacOSX }, // Peripheral Component Interconnect
			{ "Pcl", All }, // Printer Command Language
			{ "Pcm", All }, // pulse-code modulation
			{ "Pde", ApplePlatform.MacOSX }, // partial differential equation
			{ "Pdu", All }, // protocol data unit
			{ "Peap", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Protected EAP
			{ "Pebibits", All }, // IEC unit name
			{ "Pebibytes", All }, // IEC unit name
			{ "Perlin", All }, // Perlin noise name
			{ "Persistable", All }, // valid English derivative
			{ "Petabits", All }, // SI unit name
			{ "Pfs", All }, // acronym
			{ "Philox", All }, // PRNG algorithm name
			{ "Phong", All }, // Phong shading/reflection model
			{ "Photoplethysmogram", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // medical sensor term
			{ "Phq", All & ~ApplePlatform.TVOS }, // questionnaire acronym
			{ "Phy", ApplePlatform.MacOSX }, // physical layer term
			{ "Picometers", All }, // SI unit name
			{ "Pickleball", All & ~ApplePlatform.TVOS }, // sport name
			{ "Picowatts", All }, // SI unit name
			{ "Pkcs", All }, // crypto standard acronym
			{ "Placemark", All }, // mapping term
			{ "Playout", All }, // broadcasting term
			{ "Plessey", All }, // MSI/Plessey barcode symbology
			{ "Pnc", All }, // MIDI
			{ "Pnorm", All }, // Lp norm notation
			{ "Polyline", All }, // graphics geometry term
			{ "Polylines", All }, // graphics geometry term
			{ "Popularimeter", All }, // ID3 metadata field
			{ "Postback", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // marketing/webhook term
			{ "Ppd", ApplePlatform.MacOSX }, // PostScript Printer Description
			{ "Ppk", All }, // Apple API abbreviation
			{ "Preauthentication", ApplePlatform.MacOSX }, // compound word
			{ "Preds", All }, // predictions abbreviation
			{ "Prefilter", All }, // compound word
			{ "Prereleased", All }, // compound word
			{ "Prerolls", All }, // media playback term
			{ "Preseti", All }, // Apple API selector fragment
			{ "Prev", All }, // previous abbreviation
			{ "Previewable", ApplePlatform.MacOSX }, // valid English derivative
			{ "Prf", All & ~ApplePlatform.TVOS }, // pseudo-random function
			{ "Psec", All }, // picosecond abbreviation
			{ "Psk", All }, // pre-shared key
			{ "Pskc", All & ~ApplePlatform.TVOS }, // PSKC key container
			{ "Psm", All }, // Protocol/Service Multiplexer
			{ "Privs", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // privileges abbreviation
			{ "Pss", All }, // Probabilistic Signature Scheme (RSA-PSS)
			{ "Ptp", ApplePlatform.MacOSX }, // Precision Time Protocol
			{ "Ptss", All & ~ApplePlatform.TVOS }, // Presentation Timestamps (plural)
			{ "Pvr", All }, // PowerVR graphics brand
			{ "Pvrtc", All }, // MTLBlitOption - PowerVR Texture Compression
			{ "Qos", All }, // quality of service
			{ "Quadding", All }, // typesetting term
			{ "Quaterniond", All }, // double quaternion type
			{ "Quic", All }, // transport protocol
			{ "Qura", All }, // payment network name
			{ "Qwac", All }, // qualified website cert
			{ "Raycast", ApplePlatform.iOS }, // graphics/AR term
			{ "Raycasts", ApplePlatform.iOS }, // graphics/AR term
			{ "Rdd", All }, // SMPTE RDD 18 ancillary data standard
			{ "Reacquirer", All }, // valid English noun
			{ "Reassociation", ApplePlatform.MacOSX }, // networking term
			{ "Reauthentication", ApplePlatform.MacOSX }, // compound word
			{ "Reinvitation", All }, // valid English noun
			{ "Reinvite", All }, // session protocol term
			{ "Rel", All }, // relation abbreviation
			{ "Relocalization", ApplePlatform.iOS }, // AR/vision term
			{ "Relu", All }, // Rectified Linear Unit (ML)
			{ "Replayable", All }, // valid English derivative
			{ "Reprojection", All }, // graphics/vision term
			{ "Rfc", All }, // Request for Comments
			{ "Rgb", All }, // red-green-blue
			{ "Rgba", All }, // red-green-blue-alpha
			{ "Rgbaf", All }, // RGBA float format
			{ "Rgbah", All }, // RGBA half-float format
			{ "Rgbx", All }, // RGB plus unused byte
			{ "Rggb", All }, // acronym for Red, Green, Green, Blue
			{ "Rint", All }, // round-to-integer function
			{ "Rle", All }, // run-length encoding
			{ "Rms", All }, // root mean square
			{ "Rmssd", All & ~ApplePlatform.TVOS }, // root mean square of successive differences
			{ "Rnn", All }, // recurrent neural network
			{ "Roi", All }, // region of interest
			{ "Romm", All }, // acronym: Reference Output Medium Metric
			{ "Rpa", All }, // Resolvable Private Address
			{ "Rpn", All }, // acronym
			{ "Rsa", All }, // Rivest, Shamir and Adleman
			{ "Rsapss", All }, // RSA-PSS signature scheme
			{ "Rsqrt", All }, // reciprocal square root
			{ "Rssi", All }, // signal strength acronym
			{ "Rtl", All }, // right-to-left abbreviation
			{ "Rtp", All & ~ApplePlatform.MacOSX }, // Real-time Transport Protocol
			{ "Rtsp", All }, // streaming control protocol
			{ "Saml", All & ~ApplePlatform.MacCatalyst }, // acronym
			{ "Sbr", All }, // Spectral Band Replication (AAC)
			{ "Scc", All }, // subtitle/timing format
			{ "Scn", All }, // SceneKit prefix
			{ "Sdh", ApplePlatform.TVOS }, // subtitles for deaf/hard hearing
			{ "Sdk", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // software development kit
			{ "Sdnn", All & ~ApplePlatform.TVOS }, // Apple API abbreviation
			{ "Sdof", ApplePlatform.MacOSX }, // synthetic depth of field
			{ "Sdpa", All }, // Scaled Dot Product Attention
			{ "Sdr", All }, // standard dynamic range
			{ "Sdtv", ApplePlatform.TVOS }, // acronym: Standard Definition Tele Vision
			{ "Securit", ApplePlatform.iOS }, // Apple API selector fragment
			{ "Seekable", All }, // valid English derivative
			{ "Sel", All & ~ApplePlatform.MacOSX }, // Objective-C selector
			{ "Selu", All }, // Scaled Exponential Linear unit (ML)
			{ "Semitransient", ApplePlatform.MacOSX }, // valid technical term
			{ "Sensel", All }, // pressure sensor brand
			{ "Sha", All }, // Secure Hash Algorithm
			{ "Shadable", All }, // graphics term
			{ "Siemen", All & ~ApplePlatform.TVOS }, // Apple API singular form
			{ "Signbit", All }, // math library term
			{ "Simd", All }, // Single Instruction, Multiple Data
			{ "Sint", All }, // as in "Signed Integer"
			{ "Sixtyfour", ApplePlatform.MacOSX }, // compound number word
			{ "Slerp", All }, // spherical interpolation
			{ "Slomo", All }, // slow motion shorthand
			{ "Smpte", All }, // media standards body
			{ "Snapshotter", All }, // valid English noun
			{ "Snn", All }, // Apple API abbreviation
			{ "Snorm", All }, // signed normalized format
			{ "Sobel", All }, // image filter name
			{ "Softmax", All }, // get_SoftmaxNormalization
			{ "Sopen", ApplePlatform.MacOSX }, // Apple API abbreviation
			{ "Spacei", All }, // Apple API selector fragment
			{ "Spl", All }, // sound pressure level
			{ "Sqrt", All }, // square root function
			{ "Srgb", All }, // standard RGB color space
			{ "Ssid", All }, // Wi-Fi network identifier
			{ "Ssids", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // plural of SSID
			{ "Ssml", All }, // Speech Synthesis Markup Language
			{ "Sso", ApplePlatform.MacOSX }, // single sign-on
			{ "Ssr", All }, // Scalable Sample Rate (AAC)
			{ "st", All }, // ordinal suffix
			{ "Sta", ApplePlatform.MacOSX }, // station mode acronym
			{ "Strided", All }, // linear algebra term
			{ "Subband", All & ~ApplePlatform.TVOS }, // signal processing term
			{ "Subbeat", All }, // music timing term
			{ "Subcaption", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // compound word
			{ "Subcardioid", All & ~ApplePlatform.MacOSX }, // microphone polar pattern
			{ "Subentities", All }, // valid English plural
			{ "Subfilter", All & ~ApplePlatform.TVOS }, // compound word
			{ "Subfilters", All & ~ApplePlatform.TVOS }, // compound word
			{ "Subheadline", All }, // compound word
			{ "Sublocality", All }, // MapKit placemark term
			{ "Sublocation", All }, // compound word
			{ "Submesh", All }, // graphics geometry term
			{ "Submeshes", All }, // graphics geometry term
			{ "Subpixel", All }, // display/graphics term
			{ "Subresources", All }, // valid English plural
			{ "Subsec", All }, // subsecond abbreviation
			{ "Suica", All & ~ApplePlatform.TVOS }, // Japanese contactless smart card type
			{ "Superentity", All }, // Core Data term
			{ "Supertype", All }, // type-system term
			{ "Supertypes", All }, // type-system term
			{ "Svfg", All }, // stochastic variance filtering
			{ "Svg", All }, // Scalable Vector Graphics
			{ "Svgf", All }, // spatiotemporal variance-guided filter
			{ "Swolf", All & ~ApplePlatform.TVOS }, // swim efficiency score
			{ "Symbologies", All }, // plural of symbology (barcode)
			{ "Synchronizable", All }, // valid English derivative
			{ "Sysex", All }, // MIDI system exclusive
			{ "Tbgr", All }, // texture color format
			{ "Tdoa", ApplePlatform.iOS }, // time difference of arrival
			{ "Tebibits", All }, // IEC unit name
			{ "Tensorflow", All }, // machine learning framework
			{ "Tessellator", All }, // graphics term
			{ "Texcoord", All }, // texture coordinate
			{ "Texel", All }, // texture pixel term
			{ "Tga", All }, // Targa image format
			{ "th", All }, // ordinal suffix
			{ "Threadgroup", All }, // Metal compute term
			{ "Threadgroups", All }, // Metal compute term
			{ "Thumbnailing", All & ~ApplePlatform.TVOS }, // valid technical term
			{ "Thumbstick", All }, // game controller term
			{ "Thumbsticks", ApplePlatform.iOS }, // plural of thumbstick
			{ "Timecodes", All & ~ApplePlatform.TVOS }, // media timing term
			{ "Timelapse", All }, // compound word
			{ "Timelapses", All }, // plural of timelapse
			{ "Tls", All }, // Transport Layer Security
			{ "Tlv", All }, // tag-length-value
			{ "Tmoney", All & ~ApplePlatform.TVOS }, // Korean transit card
			{ "Toc", All }, // table of contents
			{ "Toci", All }, // Apple API selector fragment
			{ "Tonemap", All }, // image processing term
			{ "Touchpads", All }, // plural of touchpad
			{ "Transceive", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // communications verb
			{ "Trc", All }, // tone reproduction curve
			{ "Tri", All }, // triangle abbreviation
			{ "Ttls", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // tunneled TLS
			{ "Tweening", All }, // animation term
			{ "Twentyfour", ApplePlatform.MacOSX }, // compound number word
			{ "Twips", ApplePlatform.MacOSX }, // typography unit
			{ "tx", All }, // translation x axis
			{ "ty", All }, // translation y axis
			{ "Udam", All }, // User Defined Acquisition Metadata
			{ "Udi", All & ~ApplePlatform.TVOS }, // device identifier standard
			{ "Udp", All }, // User Datagram Protocol
			{ "Uid", All }, // user identifier
			{ "Unconfigured", All & ~ApplePlatform.MacOSX }, // valid English derivative
			{ "Undecodable", All }, // valid English derivative
			{ "Underrun", All }, // audio/buffer term
			{ "Unemphasized", ApplePlatform.MacOSX }, // valid English derivative
			{ "Unentitled", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // security entitlement term
			{ "Unfetched", All }, // valid English derivative
			{ "Unfocus", All }, // valid UI verb
			{ "Unioning", All }, // set operation term
			{ "Unmap", All }, // memory mapping verb
			{ "Unmatch", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Apple API verb
			{ "Unorm", All }, // unsigned normalized format
			{ "Unpair", ApplePlatform.MacOSX }, // device pairing verb
			{ "Unpremultiplied", All }, // graphics term
			{ "Unpremultiplying", All }, // graphics term
			{ "Unprepare", All }, // API verb form
			{ "Unproject", All }, // graphics math term
			{ "Unpublish", All }, // content management verb
			{ "Unsend", All & ~ApplePlatform.TVOS }, // messaging verb
			{ "Unsolo", All }, // audio control verb
			{ "Unsynced", ApplePlatform.MacOSX | ApplePlatform.iOS }, // sync state adjective
			{ "Untrash", ApplePlatform.iOS }, // mail/files verb
			{ "Upce", All }, // UPC-E barcode
			{ "Upi", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Unified Payments Interface
			{ "Uri", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // Uniform Resource Identifier
			{ "Usac", All }, // Unified Speech and Audio Coding
			{ "Usd", All }, // Universal Scene Description
			{ "Usdz", All }, // USD zip
			{ "Usec", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // microsecond abbreviation
			{ "Ussd", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // cellular signaling code
			{ "Uterance", All }, // speech synthesis term
			{ "Utf", All }, // Unicode Transformation Format
			{ "Uti", All & ~ApplePlatform.TVOS }, // Uniform Type Identifier
			{ "Varispeed", All }, // audio playback effect
			{ "Vbr", All }, // variable bitrate
			{ "Vbv", All }, // video buffering verifier
			{ "Vergence", All }, // binocular vision term
			{ "Vnode", All }, // virtual node term
			{ "Voip", ApplePlatform.MacCatalyst }, // voice over IP
			{ "Voronoi", All }, // geometry diagram name
			{ "Vpn", All }, // virtual private network
			{ "Vtt", All }, // WebVTT subtitle format
			{ "Waon", All & ~ApplePlatform.TVOS }, // Japanese e-money card
			{ "Warichu", All }, // Japanese annotation term
			{ "Warpable", All }, // valid English derivative
			{ "Wcdma", All }, // cellular standard
			{ "Wep", ApplePlatform.iOS | ApplePlatform.MacCatalyst }, // Wi-Fi security protocol
			{ "Wlan", ApplePlatform.MacOSX | ApplePlatform.MacCatalyst }, // wireless LAN acronym
			{ "Wpa", All & ~ApplePlatform.TVOS }, // Wi-Fi security standard
			{ "Writeability", All }, // variant of writability
			{ "Xattr", ApplePlatform.MacOSX }, // extended attribute
			{ "Xattrs", ApplePlatform.MacOSX }, // plural of xattr
			{ "Xbgr", All }, // X-blue-green-red format
			{ "Xmp", All }, // Extensible Metadata Platform
			{ "Xnor", All }, // logic gate name
			{ "Xrgb", All }, // X-red-green-blue format
			{ "xy", All }, // coordinate axis pair
			{ "Xyz", All }, // axis order abbreviation
			{ "Xzy", All }, // axis order abbreviation
			{ "Yobibits", All }, // IEC unit name
			{ "Yobibytes", All }, // IEC unit name
			{ "Yottabits", All }, // SI unit name
			{ "Yuv", ApplePlatform.MacOSX }, // luma/chroma color space
			{ "Yuvk", ApplePlatform.MacOSX }, // YUV plus black channel
			{ "yuvs", All }, // packed YUV format
			{ "yx", All }, // coordinate axis pair
			{ "Yxz", All }, // axis order abbreviation
			{ "yy", All }, // coordinate repetition term
			{ "Yyy", All }, // axis placeholder term
			{ "Yzx", All }, // axis order abbreviation
			{ "Zebibits", All }, // IEC unit name
			{ "Zebibytes", All }, // IEC unit name
			{ "Zenkaku", All & ~ApplePlatform.MacOSX }, // Japanese full-width text
			{ "Zettabits", All }, // SI unit name
			{ "Zlib", All }, // compression library name
			{ "Zxy", All }, // axis order abbreviation
			{ "Zyx", All }, // axis order abbreviation
		};

		// Check if any API name in the assembly contains the given word.
		// This is used to avoid false "unnecessary allowed typo" reports caused
		// by the spell checker not flagging the word on some machines (the spell
		// checker is non-deterministic across machines/OS versions/locales).
		bool IsWordInAnyApiName (Type [] types, string word)
		{
			foreach (var t in types) {
				if (!t.IsPublic || IsObsolete (t))
					continue;
				if (t.Name.Contains (word, StringComparison.OrdinalIgnoreCase))
					return true;
				foreach (var f in t.GetFields ()) {
					if ((!f.IsPublic && !f.IsFamily) || IsObsolete (f))
						continue;
					if (f.Name.Contains (word, StringComparison.OrdinalIgnoreCase))
						return true;
				}
				foreach (var m in t.GetMethods ()) {
					if ((!m.IsPublic && !m.IsFamily) || IsObsolete (m))
						continue;
					if (m.Name.Contains (word, StringComparison.OrdinalIgnoreCase))
						return true;
				}
			}
			return false;
		}

		bool IsObsolete (MemberInfo? mi)
		{
			if (mi is null)
				return false;
			if (mi.GetCustomAttributes<ObsoleteAttribute> (true).Any ())
				return true;
			if (MemberHasObsolete (mi))
				return true;
			if (MemberHasEditorBrowsableNever (mi))
				return true;
			// Property accessors may not have [Obsolete] even if the property does
			if (mi is MethodInfo method && method.IsSpecialName && mi.DeclaringType is not null) {
				var name = mi.Name;
				if (name.StartsWith ("get_", StringComparison.Ordinal) || name.StartsWith ("set_", StringComparison.Ordinal)) {
					var propName = name.Substring (4);
					foreach (var prop in mi.DeclaringType.GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)) {
						if (prop.Name != propName)
							continue;
						if (prop.GetCustomAttributes<ObsoleteAttribute> (true).Any () || MemberHasObsolete (prop))
							return true;
					}
				}
			}
			return IsObsolete (mi.DeclaringType);
		}

		[Test]
		public virtual void AttributeTypoTest ()
		{
			var types = Assembly.GetTypes ();
			int totalErrors = 0;
			foreach (Type t in types)
				AttributeTypo (t, ref totalErrors);

			Assert.That (totalErrors, Is.EqualTo (0), "Attributes have typos!");
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
			AssertMatchingOSVersionAndSdkVersion ();

			// that's slow and there's no value to run it on devices as the API names
			// being verified won't change from the simulator
			TestRuntime.AssertSimulatorOrDesktop ("Typos only detected on simulator/desktop");

			using var checker = new SpellChecker ();

			// Collect all unique words from public API names (split on uppercase boundaries)
			var words = new Dictionary<string, List<MemberInfo>> (StringComparer.Ordinal);
			var types = Assembly.GetTypes ();
			foreach (Type t in types) {
				if (!t.IsPublic || IsObsolete (t))
					continue;

				SplitIntoWords (words, t.Name, t);

				foreach (FieldInfo f in t.GetFields ()) {
					if ((!f.IsPublic && !f.IsFamily) || IsObsolete (f))
						continue;
					SplitIntoWords (words, f.Name, f);
				}

				foreach (MethodInfo m in t.GetMethods ()) {
					if ((!m.IsPublic && !m.IsFamily) || IsObsolete (m))
						continue;
					if (Skip (m))
						continue;
					SplitIntoWords (words, m.Name, m);
				}
			}

			// Check each unique word individually with the spell checker
			var typos = new Dictionary<string, List<MemberInfo>> (StringComparer.Ordinal);
			foreach (var kvp in words) {
				var word = kvp.Key;
				var checkRange = new NSRange (0, word.Length);
#if MONOMAC
				var typoRange = checker.CheckSpelling (word, 0, "en_US", false, 0, out var _);
#else
				var typoRange = checker.RangeOfMisspelledWordInString (word, checkRange, checkRange.Location, false, "en_US");
#endif
				if (typoRange.Length > 0)
					typos [word.Substring ((int) typoRange.Location, (int) typoRange.Length)] = kvp.Value;
			}

			// Check each typo against allowed list
			int totalErrors = 0;
			var currentPlatform = TestRuntime.CurrentPlatform;
			var usedAllowed = new HashSet<string> ();
			foreach (var kvp in typos) {
				var typo = kvp.Key;
				if (allowed.TryGetValue (typo, out var platforms) && platforms.HasFlag (currentPlatform)) {
					usedAllowed.Add (typo);
					continue;
				}
				ReportError ($"Typo: {typo}, found in {kvp.Value.Count} APIs:\n\t{string.Join ("\n\t", kvp.Value.Select (v => v.ToString ()))}", typo);
				totalErrors++;
			}

			// Verify that all allowed words for the current platform are still needed
			var unusedAllowed = allowed.Keys
				.Where (w => allowed [w].HasFlag (currentPlatform))
				.Except (usedAllowed);
			foreach (var typo in unusedAllowed) {
				if (IsWordInAnyApiName (types, typo))
					continue;
				ReportError ($"Unnecessary allowed typo \"{typo}\" is not present in any API name");
				totalErrors++;
			}
			Assert.That (totalErrors, Is.EqualTo (0), "Typos!");
		}

		// Split an API name into words on uppercase/digit/symbol boundaries and add to the set
		static void SplitIntoWords (Dictionary<string, List<MemberInfo>> words, string name, MemberInfo member)
		{
			int start = -1;
			for (int i = 0; i < name.Length; i++) {
				char c = name [i];
				if (Char.IsUpper (c)) {
					if (start >= 0 && i > start)
						Add (words, name.Substring (start, i - start), member);
					start = i;
				} else if (Char.IsDigit (c) || c == '<' || c == '>' || c == '_') {
					if (start >= 0 && i > start)
						Add (words, name.Substring (start, i - start), member);
					start = -1;
				} else if (start < 0) {
					// lowercase char with no word start — skip
				}
			}
			if (start >= 0 && name.Length > start)
				Add (words, name.Substring (start), member);
		}

		static void Add (Dictionary<string, List<MemberInfo>> words, string name, MemberInfo member)
		{
			if (!words.TryGetValue (name, out var list))
				words [name] = list = new List<MemberInfo> ();
			list.Add (member);
		}

		bool Skip (MethodInfo method)
		{
			// These are methods injected by the registrar, and not API we expose, so skip them.
			switch (method.Name) {
			case "_Xamarin_ConstructINativeObject":
			case "_Xamarin_ConstructNSObject":
				return true;
			}

			return false;
		}

		string? GetMessage (object attribute)
		{
			string? message = null;
			if (attribute is AdviceAttribute)
				message = ((AdviceAttribute) attribute).Message;
			if (attribute is ObsoleteAttribute)
				message = ((ObsoleteAttribute) attribute).Message;

			return message;
		}

		void AttributesMessageTypoRules (MemberInfo mi, string typeName, ref int totalErrors)
		{
			if (mi is null)
				return;

			foreach (object ca in mi.GetCustomAttributes ()) {
				string? message = GetMessage (ca);
				if (message is not null) {
					var memberAndTypeFormat = mi.Name == typeName ? "Type: {0}" : "Member name: {1}, Type: {0}";
					var memberAndType = string.Format (memberAndTypeFormat, typeName, mi.Name);

					// Rule 1: https://github.com/dotnet/macios/wiki/BINDINGS#rule-1
					// Note: we don't enforce that rule for the Obsolete (not Obsoleted) attribute since the attribute itself doesn't support versions.
					if (!(ca is ObsoleteAttribute)) {
						var forbiddenOSNames = new [] { "iOS", "watchOS", "tvOS", "macOS" };
						if (forbiddenOSNames.Any (s => Regex.IsMatch (message, $"({s} ?)[0-9]+"))) {
							ReportError ("[Rule 1] Don't put OS information in attribute's message: \"{0}\" - {1}", message, memberAndType);
							totalErrors++;
						}
					}

					// Rule 2: https://github.com/dotnet/macios/wiki/BINDINGS#rule-2
					if (message.Contains ('`')) {
						ReportError ("[Rule 2] Replace grave accent (`) by apostrophe (') in attribute's message: \"{0}\" - {1}", message, memberAndType);
						totalErrors++;
					}

					// Rule 3: https://github.com/dotnet/macios/wiki/BINDINGS#rule-3
					if (!message.EndsWith (".", StringComparison.Ordinal)) {
						if (!allowedRule3.Contains (typeName)) {
							ReportError ("[Rule 3] Missing '.' in attribute's message: \"{0}\" - {1}", message, memberAndType);
							totalErrors++;
						}
					}

					// Rule 4: https://github.com/dotnet/macios/wiki/BINDINGS#rule-4
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

		bool CheckLibrary (string? lib)
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
					Assert.That (Version.TryParse (s, out _), Is.True, fi.Name);
					break;
#if !XAMCORE_5_0
				case "AssetsLibraryLibrary":
				case "NewsstandKitLibrary": // Removed from iOS, but we have to keep the constant around for binary compatibility.
					break;
#endif
#if !__MACOS__
				case "AVSystemRoutingLibrary":
				case "CinematicLibrary":
				case "ScreenCaptureKitLibrary":
				case "ThreadNetworkLibrary":
				case "MediaSetupLibrary":
				case "MLComputeLibrary":
					// Xcode 12 beta 2 does not ship these framework/headers for the simulators
					if (TestRuntime.IsDevice)
						Assert.That (CheckLibrary (s), Is.True, fi.Name);
					break;
#endif
#if __TVOS__
				case "MetalPerformanceShadersGraphLibrary":
					// not supported in tvOS (12.1) simulator so load fails
					if (TestRuntime.IsSimulatorOrDesktop)
						break;
					goto default;
#endif
				case "MetalFXLibrary":
					if (TestRuntime.IsSimulatorOrDesktop)
						break;
					goto default;
				case "SensorKitLibrary": // SensorKit doesn't exist on iPads
					if (TestRuntime.IsDevice && TestRuntime.IsiPad)
						break;
					goto default;
#if __TVOS__
				// This framework is only available on device
				case "BrowserEngineKitLibrary":
					if (TestRuntime.CheckXcodeVersion (16, 2) && TestRuntime.IsSimulator)
						continue;
					goto default;
#endif // __TVOS__
				default:
					if (fi.Name.EndsWith ("Library", StringComparison.Ordinal)) {
#if __IOS__
						if (fi.Name == "CoreNFCLibrary") {
							// NFC is currently not available on iPad
							if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
								continue;
						}
#endif
#if __MACOS__
						// Only available in macOS 10.15.4+
						if (fi.Name == "AutomaticAssessmentConfigurationLibrary" && !TestRuntime.CheckXcodeVersion (11, 4))
							continue;
#endif
						Assert.That (CheckLibrary (s), Is.True, fi.Name);
					} else {
						Assert.Fail ($"Unknown '{fi.Name}' field cannot be verified - please fix me!");
					}
					break;
				}
			}
		}
	}
}
