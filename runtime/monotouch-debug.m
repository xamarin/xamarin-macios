//
// debug.m: Debugging code for MonoTouch
// 
// Authors:
//   Geoff Norton
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2009 Novell, Inc.
// Copyright 2011-2013 Xamarin Inc. 
//

#ifdef DEBUG

//#define LOG_HTTP(...) do { NSLog (@ __VA_ARGS__); } while (0);
#define LOG_HTTP(...)

#include <UIKit/UIKit.h>

#include <zlib.h>

#include <stdlib.h>
#include <string.h>
#include <arpa/inet.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <sys/select.h>
#include <sys/time.h>
#include <netinet/in.h>
#include <netinet/tcp.h>
#include <unistd.h>
#include <fcntl.h>
#include <errno.h>
#include <ctype.h>
#include <pthread.h>
#include <objc/objc.h>
#include <objc/runtime.h>
#include <sys/shm.h>
#include <libkern/OSAtomic.h>

#include "xamarin/xamarin.h"
#include "runtime-internal.h"
#include "monotouch-debug.h"
#include "product.h"

// permanent connection variables
int monodevelop_port = -1;
int sdb_fd = -1;
int profiler_fd = -1;
int heapshot_fd = -1; // this is the socket to write 'heapshot' to to requests heapshots from the profiler
int heapshot_port = -1;
char *profiler_description = NULL; 
// old variables
int output_port;
int debug_port; 
char *debug_host = NULL;

enum DebuggingMode
{
	DebuggingModeNone,
	DebuggingModeUsb,
	DebuggingModeWifi,
	DebuggingModeHttp,
};

static pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
static pthread_cond_t cond = PTHREAD_COND_INITIALIZER;
static bool debugging_configured = false;
static bool profiler_configured = false;
static bool config_timedout = false;
static bool connection_failed = false;
static DebuggingMode debugging_mode = DebuggingModeWifi;
static const char *connection_mode = "default"; // this is set from the cmd line, can be either 'usb', 'wifi', 'http' or 'none'

void monotouch_connect_usb ();
void monotouch_connect_wifi (NSMutableArray *hosts);
void xamarin_connect_http (NSMutableArray *hosts);
int monotouch_debug_listen (int debug_port, int output_port);
int monotouch_debug_connect (NSMutableArray *hosts, int debug_port, int output_port);
void monotouch_configure_debugging ();
void monotouch_load_profiler ();
void monotouch_load_debugger ();
bool monotouch_process_connection (int fd);

static struct timeval wait_tv;
static struct timespec wait_ts;

static NSURLSessionConfiguration *http_session_config = NULL;
static volatile int http_connect_counter = 0;

/*
 * XamarinHttpConnection
 */

/*
 * Debugging over http
 *
 * The watchOS device has limited networking support; in particular
 * it does not allow inbound/output network connections using 'bind'
 * (kernel-level sandbox restrictions).
 *
 * This means that we can't use BSD sockets to connect to the debugger
 * in the IDE on the desktop. Instead we create an http tunnel that
 * knows how to convert socket send/recv data into http requests on
 * both sides.
 *
 * To avoid touching existing (and working) code as much as possible,
 * the following is done:
 *
 * A pair of socket is created in the process (since this is not
 * inbound/output networking it's apparently allowed) using socketpair.
 * One of those sockets is given to mono/sdb, and for mono/sdb no
 * code changes are required. Then we create read/write threads for the
 * other socket that transforms recv/send calls into http requests.
 *
 * A complication is that there doesn't seem to be a way to create
 * a streaming http upload using NSUrlSession, the data to upload
 * must be known when creating the request. This means that we need
 * to create a new http request for every write on the socket done
 * by mono/sdb.
 *
 * * The only API (that I could find) that implements 
 *   a streaming upload is [NSURLSession uploadTaskWithStreamedRequest:].
 *   However that API uses an NSInputStream to fetch the data, and
 *   there is no built-in way in the API to create a streaming NSInputStream,
 *   you have to provide an existing file or NSData. It is technically
 *   possible to subclass NSInputStream (using private API), and this works
 *   fine on iOS, but not on watchOS (NSInputStreams are really CFReadStreams
 *   in disguise, and watchOS casts the NSInputStream to a CFReadStream and
 *   pokes directly into CFReadSTream fields, thus accessing
 *   random memory). There is CFStreamCreatePairWithSocket, which creates
 *   a CFReadStream from a socket (which we could in theory connect directly
 *   to one of the in-process sockets we got), but it doesn't work 
 *   on watchOS (it works fine on iOS) - no data is ever read from the
 *   socket.
 *
 * The process goes as follows:
 * 
 * a) The IDE listens for connections on a port on the desktop
 *    (for the IDE this is exactly the same as the WiFi debug mode).
 *    and asks mlaunch to launch the app on the watch, passing
 *    the port as an argument + environment variable.
 * b) mlaunch will intervene, and create the desktop side of the
 *    http tunnel. This involves a different port, so mlaunch
 *    will change the arguments/environment variables that are
 *    passed to the app to reflect the different port. mlaunch
 *    will also enable the 'http' mode.
 * c) The app will launch on device, and create the app side of
 *    the http tunnel.
 * c) When the app connects to the IDE, an HTTP GET request is sent.
 *    This request is kept open, and will stream/download data as
 *    it's written to the socket on the desktop by the IDE.
 *    The request includes the PID, so that mlaunch can ignore
 *    requests from other processes (it seems watchOS sends the
 *    requests from a different process, because the desktop can
 *    receive requests way after a process has terminated, which
 *    also means a lingering request from an earlier process would
 *    confuse the IDE if mlaunch didn't filter them out). The GET 
 *    request also contains a monotonically increasing ID.
 *
 *    The app has a list of potential IP addresses, and for the first
 *    connection an http request is sent to all IP addresses. This
 *    first request also has a 'uniqueRequest' value, which tells
 *    mlaunch that it should only accept one of those requests (if
 *    multiple requests are received, which might happen if more than
 *    one of those IP addresses are on the same network). All other
 *    request will be rejected by mlaunch (HTTP 400 error).
 *
 * d) When the app sends sends data to the IDE, a HTTP POST request is
 *    sent. The full data for the post has to be provided when the
 *    request is sent, which means that we'll send a HTTP POST request
 *    every time the app calls 'send' on the app's socket. The request
 *    includes the PID and the ID from the corresponding GET request,
 *    so that mlaunch can match multiple POST requests to the correct
 *    GET request. The request also includes a monotonically increasing
 *    upload-id, so that mlaunch can order them properly, because http
 *    requests may not reach the desktop in the same order they were sent.
 *
 */

@interface XamarinHttpConnection : NSObject<NSURLSessionDelegate> {
	NSURLSession *http_session;
	int http_sockets[2];
	volatile int http_send_counter;
}
	@property (copy) void (^completion_handler)(bool);
	@property (copy) NSString* ip;
	@property int id;
	@property bool uniqueRequest;

	-(void) dealloc;

	-(int) fileDescriptor;
	-(int) localDescriptor;
	-(void) reportCompletion: (bool) success;

	-(void) connect: (NSString *) ip port: (int) port completionHandler: (void (^)(bool)) completionHandler;
	-(void) sendData: (void *) buffer length: (int) length;

	/* NSURLSessionDelegate */
	-(void) URLSession:(NSURLSession *)session didBecomeInvalidWithError:(NSError *)error;
	-(void) URLSession:(NSURLSession *)session didReceiveChallenge:(NSURLAuthenticationChallenge *)challenge completionHandler:(void (^)(NSURLSessionAuthChallengeDisposition disposition, NSURLCredential *credential))completionHandler;

	/* NSURLSessionDataDelegate */
	-(void) URLSession:(NSURLSession *)session dataTask:(NSURLSessionDataTask *)dataTask didReceiveResponse:(NSURLResponse *)response completionHandler:(void (^)(NSURLSessionResponseDisposition disposition))completionHandler;
	-(void) URLSession:(NSURLSession *)session dataTask:(NSURLSessionDataTask *)dataTask didReceiveData:(NSData *)data;

	/* NSURLSessionTaskDelegate */
	-(void) URLSession:(NSURLSession *)session task:(NSURLSessionTask *)task didCompleteWithError:(NSError *)error;
@end

static void *
xamarin_http_send (void *c)
{
	XamarinHttpConnection *connection = (XamarinHttpConnection *) c;
	@autoreleasepool {
		int fd = connection.localDescriptor;
		void* buf [1024];
		do {
			LOG_HTTP ("%i http send reading to send data to fd=%i", connection.id, fd);
			errno = 0;
			int rv = read (fd, buf, 1024);
			LOG_HTTP ("%i http send read %i bytes from fd=%i; %i=%s", connection.id, rv, fd, errno, strerror (errno));
			if (rv > 0) {
				[connection sendData: buf length: rv];
			} else if (rv == -1) {
				if (errno == EINTR)
					continue;
				LOG_HTTP ("%i http send: %i => %s", connection.id, errno, strerror (errno));
				break;
			} else {
				LOG_HTTP ("%i http send: eof", connection.id);
				break;
			}
		} while (true);
		LOG_HTTP ("%i http send done", connection.id);
	}
	return NULL;
}

@implementation XamarinHttpConnection
-(void) dealloc
{
	self.ip = NULL;
	self.completion_handler = NULL;
	[super dealloc];
}

-(void) reportCompletion: (bool) success
{
	LOG_HTTP ("%i reportCompletion (%i) completion_handler: %p", self.id, success, self.completion_handler);
	self.completion_handler (success);
}


-(int) fileDescriptor
{
	return http_sockets [0];	
}

-(int) localDescriptor
{
	return http_sockets [1];
}

-(void) connect: (NSString *) ip port: (int) port completionHandler: (void (^)(bool)) completionHandler
{
	LOG_HTTP ("Connecting to: %@:%i", ip, port);

	self.completion_handler = completionHandler;
	self.id = ++http_connect_counter;

	int rv = socketpair (PF_LOCAL, SOCK_STREAM, 0, http_sockets);
	if (rv != 0) {
		[self reportCompletion: false];
		return;
	}

	LOG_HTTP ("%i Created socket pair: %i, %i", self.id, http_sockets [0], http_sockets [1]);

	pthread_t thr;
	pthread_create (&thr, NULL, xamarin_http_send, self);
	pthread_detach (thr);

	if (http_session_config == NULL) {
		http_session_config = [NSURLSessionConfiguration ephemeralSessionConfiguration];
		http_session_config.allowsCellularAccess = NO; // debugging data should never go over cellular
		http_session_config.networkServiceType = NSURLNetworkServiceTypeVoIP; // not quite right, but this will wake up the app for incoming network traffic
		http_session_config.timeoutIntervalForRequest = 3600; // 1 hour
		http_session_config.requestCachePolicy = NSURLRequestReloadIgnoringLocalCacheData; // do not cache anything
		http_session_config.HTTPMaximumConnectionsPerHost = 20;
	}

	http_session = [NSURLSession sessionWithConfiguration: http_session_config delegate: self delegateQueue: NULL];

	NSURL *downloadURL = [NSURL URLWithString: [NSString stringWithFormat: @"http://%@:%i/download?pid=%i&id=%i&uniqueRequest=%i", self.ip, monodevelop_port, getpid (), self.id, self.uniqueRequest]];
	NSURLSessionDataTask *downloadTask = [http_session dataTaskWithURL: downloadURL];
	[downloadTask resume];

	LOG_HTTP ("%i Connecting to: %@:%i downloadTask: %@", self.id, ip, port, [[downloadTask currentRequest] URL]);
}

-(void) sendData: (void *) buffer length: (int) length
{
	int c = OSAtomicIncrement32Barrier (&http_send_counter);

	NSURL *uploadURL = [NSURL URLWithString: [NSString stringWithFormat: @"http://%@:%i/upload?pid=%i&id=%i&upload-id=%i", self.ip, monodevelop_port, getpid (), self.id, c]];
	LOG_HTTP ("%i sendData length: %i url: %@", self.id, length, uploadURL);
	NSMutableURLRequest *uploadRequest = [[[NSMutableURLRequest alloc] initWithURL: uploadURL] autorelease];
	uploadRequest.HTTPMethod = @"POST";
	NSURLSessionUploadTask *uploadTask = [http_session uploadTaskWithRequest: uploadRequest fromData: [NSData dataWithBytes: buffer length: length]];
	[uploadTask resume];
}

/* NSURLSessionDataDelegate */
-(void) URLSession: (NSURLSession *) session didBecomeInvalidWithError: (NSError *) error
{
	NSLog (@PRODUCT ": Connection to the debugger failed (id: %i didBecomeInvalidWithError: %@).", self.id, error);
	[self reportCompletion: false];
}

-(void) URLSession: (NSURLSession *) session didReceiveChallenge: (NSURLAuthenticationChallenge *) challenge completionHandler: (void (^)(NSURLSessionAuthChallengeDisposition disposition, NSURLCredential *credential)) completionHandler
{
	LOG_HTTP ("%i didReceiveChallenge", self.id);
}

-(void) URLSession: (NSURLSession *) session dataTask: (NSURLSessionDataTask *) dataTask didReceiveResponse: (NSURLResponse *) response completionHandler: (void (^)(NSURLSessionResponseDisposition disposition)) completionHandler
{
	NSHTTPURLResponse *http_response = (NSHTTPURLResponse *) response;

	LOG_HTTP ("%i didReceiveResponse: task: %@ url: %@ response: %@", self.id, dataTask, [[dataTask originalRequest] URL], response);
	completionHandler (NSURLSessionResponseAllow);
	[self reportCompletion: http_response.statusCode == 200];
}

-(void) URLSession: (NSURLSession *) session dataTask: (NSURLSessionDataTask *) dataTask didReceiveData: (NSData *) data
{
	// We got data from the IDE.
	LOG_HTTP ("%i didReceiveData length: %li %@", self.id, (unsigned long) [data length], data);

	[data enumerateByteRangesUsingBlock: ^(const void *bytes, NSRange byteRange, BOOL *stop) {
		int fd = self.localDescriptor;
		int wr;
		NSUInteger total = byteRange.length;
		NSUInteger left = total;
		while (left > 0) {
			do {
				wr = write (fd, bytes, left);
			} while (wr == -1 && errno == EINTR);
			if (wr > 0) {
				left -= wr;
				LOG_HTTP ("%i didReceiveData wrote %i/%lu bytes to %i; %lu bytes left", self.id, wr, (unsigned long) total, fd, (unsigned long) left);
			} else if (wr == 0) {
				LOG_HTTP ("%i didReceiveData no data written.", self.id);
			} else {
				LOG_HTTP ("%i didReceiveData error occured: %i = %s", self.id, errno, strerror (errno));
				break;
			}
		}
	}];
}

-(void) URLSession: (NSURLSession *) session task: (NSURLSessionTask *) task didCompleteWithError: (NSError *) error
{
	if (error) {
		NSLog (@PRODUCT ": Connection to the debugger failed (id: %i didCompleteWithError: %@ task: %@ url: %@)", self.id, error, task, [[task originalRequest] URL]);
	} else {
		LOG_HTTP ("%i didCompleteWithError: SUCCESS task: %@ url: %@", self.id, task, [[task originalRequest] URL]);
	}
}
@end /* XamarinHttpConnection */

void
monotouch_set_connection_mode (const char *mode)
{
	connection_mode = mode;
}

void
monotouch_set_monodevelop_port (int port)
{
	monodevelop_port = port;
}

void
monotouch_start_debugging ()
{
	// COOP: this is at startup and doesn't access managed memory: safe mode.
	MONO_ASSERT_GC_STARTING;
	
	bool debug_enabled = strcmp (connection_mode, "none");
	if (xamarin_debug_mode) {
		if (debug_enabled) {
			// wait for debug configuration to finish
			gettimeofday(&wait_tv, NULL);

			int timeout;
			int iterations = 1;
#if TARGET_OS_WATCH && !TARGET_OS_SIMULATOR
			// When debugging on a watch device, we ensure that a native debugger is attached as well
			// (since otherwise the launch sequence takes so long that watchOS kills us).
			// Ensuring that a native debugger is attached when debugging also makes it possible
			// to not wait at all when a native debugger is not attached, which would otherwise
			// slow down every debug launch. And *that* allows us to wait for as long as we wish
			// when debugging.
			timeout = 10;
			iterations = 360;
#else
			timeout = 2;
#endif
			wait_ts.tv_sec = wait_tv.tv_sec + timeout;
			wait_ts.tv_nsec = wait_tv.tv_usec * 1000;
			
			pthread_mutex_lock (&mutex);
			while (!debugging_configured && !config_timedout && !connection_failed) {
				if (pthread_cond_timedwait (&cond, &mutex, &wait_ts) == ETIMEDOUT) {
					iterations--;
					if (iterations <= 0) {
						config_timedout = true;
					} else {
						LOG (PRODUCT ": Waiting for connection to the IDE...")
						// Try again
						gettimeofday(&wait_tv, NULL);
						wait_ts.tv_sec = wait_tv.tv_sec + timeout;
						wait_ts.tv_nsec = wait_tv.tv_usec * 1000;
					}
				}
			}
			pthread_mutex_unlock (&mutex);
			
			if (connection_failed) {
				LOG (PRODUCT ": Debugger not loaded (failed to connect to the IDE).\n");
			} else if (config_timedout) {
				LOG (PRODUCT ": Debugger not loaded (timed out while trying to connect to the IDE).\n");
			} else {
				monotouch_load_debugger ();
			}
		} else {
			LOG (PRODUCT ": Not connecting to the IDE, debug has been disabled\n");
		}
	
		char *trace = getenv ("MONO_TRACE");
		if (trace && *trace) {
			if (!strncmp (trace, "--trace=", 8))
				trace += 8;

			MONO_ENTER_GC_UNSAFE;
			mono_jit_set_trace_options (trace);
			MONO_EXIT_GC_UNSAFE;
		}
	}
}

void
monotouch_start_profiling ()
{
	// COOP: at startup, should be in safe mode here. If that's not the case, we need to switch to safe mode when calling pthread_mutex_lock.
	MONO_ASSERT_GC_STARTING;
	
	bool debug_enabled = strcmp (connection_mode, "none");
	if (xamarin_debug_mode && debug_enabled) {
		// wait for profiler configuration to finish
		pthread_mutex_lock (&mutex);
		while (!profiler_configured && !config_timedout) {
			if (pthread_cond_timedwait (&cond, &mutex, &wait_ts) == ETIMEDOUT)
				config_timedout = true;
		}
		pthread_mutex_unlock (&mutex);
	
		if (!config_timedout)
			monotouch_load_profiler ();
	}
}

static NSString *
get_preference (NSArray *preferences, NSUserDefaults *defaults, NSString *lookupKey)
{
	NSDictionary *dict;
 
	// Apple appears to return nil if the user has never opened the Settings, so we
	// manually parse it here.  This has the added benefits that if people don't open
	// settings we can control the default from MD

	// User Preferences have the highest precedence
	for (dict in preferences) {
		NSString *key = [dict objectForKey:@"Key"];
		if (![key isEqualToString:lookupKey])
			continue;
		
		return [dict objectForKey:@"DefaultValue"];
	}

	// Global Defaults have the second highest precedence
	return defaults ? [defaults stringForKey:lookupKey] : nil;
}

void monotouch_configure_debugging ()
{
	// COOP: this is at startup, before initializing the mono runtime, so we're in the STARTING mode. If that's not the case, at the very least we must transition to safe mode when calling pthread_mutex_lock.
	MONO_ASSERT_GC_STARTING;
	
	// This method is invoked on a separate thread
	NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
	NSString *bundle_path = [NSString stringWithUTF8String:xamarin_get_bundle_path ()];
	NSString *settings_path = [bundle_path stringByAppendingPathComponent:@"Settings.bundle"]; 
	NSString *root_plist = [settings_path stringByAppendingPathComponent:@"Root.plist"];
	NSDictionary *settings = [NSDictionary dictionaryWithContentsOfFile: root_plist];
	NSArray *preferences = [settings objectForKey:@"PreferenceSpecifiers"];
	NSMutableArray *hosts = [NSMutableArray array];
	bool debug_enabled;
	NSString *monodevelop_host;
	NSString *monotouch_debug_enabled;

	if (!strcmp (connection_mode, "default")) {
		char *evar = getenv ("__XAMARIN_DEBUG_MODE__");
		if (evar && *evar) {
			connection_mode = evar;
			LOG (PRODUCT ": Found debug mode %s in environment variables\n", connection_mode);
			unsetenv ("__XAMARIN_DEBUG_MODE__");
		}
	}
	
	if (!strcmp (connection_mode, "none")) {
		// nothing to do
		return;
	}
 
	// If debugging is enabled
	monotouch_debug_enabled = get_preference (preferences, NULL, @"__monotouch_debug_enabled"); 
	if (monotouch_debug_enabled != nil) {
		debug_enabled = [monotouch_debug_enabled isEqualToString:@"1"];
	} else {
		debug_enabled = [defaults boolForKey:@"__monotouch_debug_enabled"];
	}

#if TARGET_OS_WATCH && !TARGET_OS_SIMULATOR
	if (debug_enabled) {
		if (!xamarin_is_native_debugger_attached ()) {
			LOG (PRODUCT ": Debugging was disabled for watchOS because no native debugger is attached.");
			debug_enabled = false;
		}
	}
#endif

	//        We get the IPs of the dev machine + one port (monodevelop_port).
	//        We start up a thread (using the same thread that we have to start up
	//        anyway to initialize cocoa threading) and then establishes several
	//        connections to MD (for usb we listen for connections and for wifi we
	//        connect to MD using any of the IPs we got). MD then sends instructions
	//        on those connections telling us what to do with them. We never stop
	//        processing connections and commands from that thread - this way MD can
	//        send an exit request when MD wants us to exit.
	monodevelop_host = get_preference (preferences, defaults, @"__monodevelop_host");
	if (monodevelop_host != nil && ![monodevelop_host isEqualToString:@"automatic"]) {
		[hosts addObject:monodevelop_host];
		LOG (PRODUCT ": Added host from settings to look for the IDE: %s\n", [monodevelop_host UTF8String]);
	}

	char *evar = getenv ("__XAMARIN_DEBUG_PORT__");
	if (evar && *evar) {
		if (monodevelop_port == -1) {
			monodevelop_port = strtol (evar, NULL, 10);
			LOG (PRODUCT ": Found port %i in environment variables\n", monodevelop_port);
		}
		unsetenv ("__XAMARIN_DEBUG_PORT__");
	}

	evar = getenv ("__XAMARIN_DEBUG_HOSTS__");
	if (evar && *evar) {
		NSArray *ips = [[NSString stringWithUTF8String:evar] componentsSeparatedByString:@";"];
		for (int i = 0; i < [ips count]; i++) {
			NSString *ip = [ips objectAtIndex:i];
			if (![hosts containsObject:ip]) {
				[hosts addObject:ip];
				LOG (PRODUCT ": Found host %s in environment variables\n", [ip UTF8String]);
			}
		}
		unsetenv ("__XAMARIN_DEBUG_HOSTS__");
	}

#if MONOTOUCH && (defined(__i386__) || defined (__x86_64__))
	// Try to read shared memory as well
	key_t shmkey;
	if (xamarin_launch_mode == XamarinLaunchModeApp) {
		// Don't read shared memory in normal apps, because we're always able to pass
		// the debug data (host/port) using either command-line arguments or environment variables
	} else if ((shmkey = ftok ("/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/bin/mtouch", 0)) == -1) {
		LOG (PRODUCT ": Could not create shared memory key: %s\n", strerror (errno));
	} else {
		int shmsize = 1024;
		int shmid = shmget (shmkey, shmsize, 0);
		if (shmid == -1) {
			LOG (PRODUCT ": Could not get shared memory id: %s\n", strerror (errno));
		} else {
			void *ptr = shmat (shmid, NULL, SHM_RDONLY);
			if (ptr == NULL || ptr == (void *) -1) {
				LOG (PRODUCT ": Could not map shared memory: %s\n", strerror (errno));
			} else {
				LOG (PRODUCT ": Read %i bytes from shared memory: %p with key %i and id %i\n", shmsize, ptr, shmkey, shmid);
				// Make a local copy of the shared memory, so that it doesn't change while we're parsing it.
				char *data = strndup ((const char *) ptr, shmsize); // strndup will null-terminate
				char *line = data;
				// Parse!
				while (*line) {
					char *nline = line;
					// find the end of the line, null-terminate the line and make 'nptr' to the next line. 
					do {
						if (*nline == '\n') {
							*nline = 0;
							nline++;
							break;
						}
					} while (*++nline);

					if (!strncmp (line, "__XAMARIN_DEBUG_PORT__=", 23)) {
						int shm_monodevelop_port = strtol (line + 23, NULL, 10);
						if (monodevelop_port == -1) {
							monodevelop_port = shm_monodevelop_port;
							LOG (PRODUCT ": Found port %i in shared memory\n", monodevelop_port);
						} else  {
							LOG (PRODUCT ": Found port %i in shared memory, but not overriding existing port %i\n", shm_monodevelop_port, monodevelop_port);
						}
					} else {
						LOG (PRODUCT ": Unknown data found in shared memory: %s\n", line);
					}
					line = nline;
				}
				free (data);
				shmdt (ptr);
			}
		}
	}
#endif

	// Finally, fall back to loading values from MonoTouchDebugConfiguration.txt
	FILE *debug_conf = fopen ("MonoTouchDebugConfiguration.txt", "r");
	if (debug_conf != NULL) { 
		bool add_hosts = [hosts count] == 0;
		char line [128];
		int i;

		while (!feof (debug_conf)) {
			if (fgets (line, sizeof (line), debug_conf) != NULL) {
				// Remove trailing newline
				for (i = 0; line[i]; i++) {
					if (line [i] == '\n' || line [i] == '\r') {
						line [i] = 0;
						break;
					}
				}
				
				if (!strncmp ("IP: ", line, 4)) {
					if (add_hosts) {
						NSString *ip;

						ip = [NSString stringWithUTF8String:line + 4];
						if (![hosts containsObject:ip]) {
							[hosts addObject:ip];
							LOG (PRODUCT ": Added IP to look for the IDE: %s\n", [ip UTF8String]);
						}
					}
				} else if (!strncmp ("USB Debugging: ", line, 15) && (connection_mode == NULL || !strcmp (connection_mode, "default"))) {
#if defined(__arm__) || defined(__aarch64__)
					debugging_mode = !strncmp ("USB Debugging: 1", line, 16) ? DebuggingModeUsb : DebuggingModeWifi;
#endif
				} else if (!strncmp ("Port: ", line, 6) && monodevelop_port == -1) {
					monodevelop_port = strtol (line + 6, NULL, 10);
				}
			}
		}
		
		fclose (debug_conf);
	}

	if (debug_enabled) {
		// connection_mode is set from the command line, and will override any other setting
		if (connection_mode != NULL) {
			if (!strcmp (connection_mode, "usb")) {
				debugging_mode = DebuggingModeUsb;
			} else if (!strcmp (connection_mode, "wifi")) {
				debugging_mode = DebuggingModeWifi;
			} else if (!strcmp (connection_mode, "http")) {
				debugging_mode = DebuggingModeHttp;
			}
		}

		if (monodevelop_port <= 0) {
			LOG (PRODUCT ": Invalid IDE Port: %i\n", monodevelop_port);
		} else {
			LOG (PRODUCT ": IDE Port: %i Transport: %s\n", monodevelop_port, debugging_mode == DebuggingModeHttp ? "HTTP" : (debugging_mode == DebuggingModeUsb ? "USB" : "WiFi"));
			if (debugging_mode == DebuggingModeUsb) {
				monotouch_connect_usb ();
			} else if (debugging_mode == DebuggingModeWifi) {
				monotouch_connect_wifi (hosts);
			} else if (debugging_mode == DebuggingModeHttp) {
				xamarin_connect_http (hosts);
			}
		}
	}

	profiler_configured = true;
	debugging_configured = true;
	MONO_ASSERT_GC_STARTING;
	pthread_mutex_lock (&mutex);
	pthread_cond_signal (&cond);
	pthread_mutex_unlock (&mutex);
}

void sdb_connect (const char *address)
{
	gboolean shaked;

	MONO_ENTER_GC_UNSAFE;
	shaked = mono_debugger_agent_transport_handshake ();
	MONO_EXIT_GC_UNSAFE;
	
	if (!shaked)
		PRINT (PRODUCT ": Handshake error with IDE.");

	return;
}

void sdb_close1 (void)
{
	shutdown (sdb_fd, SHUT_RD);
}

void sdb_close2 (void)
{
	shutdown (sdb_fd, SHUT_RDWR);
}

gboolean send_uninterrupted (int fd, const void *buf, int len)
{
	int res;

	do {
		res = send (fd, buf, len, 0);
	} while (res == -1 && errno == EINTR);

	return res == len;
}

int recv_uninterrupted (int fd, void *buf, int len)
{
	int res;
	int total = 0;
	int flags = 0;

	do { 
		res = recv (fd, (char *) buf + total, len - total, flags); 
		if (res > 0)
			total += res;
	} while ((res > 0 && total < len) || (res == -1 && errno == EINTR));

	return total;
}

gboolean sdb_send (void *buf, int len)
{
	gboolean rv;

	if (debugging_configured) {
		MONO_ENTER_GC_SAFE;
		rv = send_uninterrupted (sdb_fd, buf, len);
		MONO_EXIT_GC_SAFE;
	} else {
		rv = send_uninterrupted (sdb_fd, buf, len);
	}

	return rv;
}


int sdb_recv (void *buf, int len)
{
	int rv;

	if (debugging_configured) {
		MONO_ENTER_GC_SAFE;
		rv = recv_uninterrupted (sdb_fd, buf, len);
		MONO_EXIT_GC_SAFE;
	} else {
		rv = recv_uninterrupted (sdb_fd, buf, len);
	}

	return rv;
}

static XamarinHttpConnection *connected_connection = NULL;
static NSString *connected_ip = NULL;
static pthread_cond_t connected_event = PTHREAD_COND_INITIALIZER;
static pthread_mutex_t connected_mutex = PTHREAD_MUTEX_INITIALIZER;
static int pending_connections = 0;

void
xamarin_connect_http (NSMutableArray *ips)
{
	// COOP: this is at startup and doesn't access managed memory, so we should be in safe mode here.
	MONO_ASSERT_GC_STARTING;
	
	int ip_count = [ips count];
	NSMutableArray<XamarinHttpConnection *> *connections = NULL;

	if (ip_count == 0) {
		NSLog (@PRODUCT ": No IPs to connect to.");
		return;
	}
	
	NSLog (@PRODUCT ": Connecting to %i IPs.", ip_count);

	connections = [[[NSMutableArray<XamarinHttpConnection *> alloc] init] autorelease];

	bool unique_request = true;
	do {
		pthread_mutex_lock (&connected_mutex);
		if (connected_connection != NULL) {
			LOG_HTTP ("Will reconnect");
			// We've already made sure one IP works, no need to try the others again.
			[ips removeAllObjects];
			[ips addObject: connected_ip];
			connected_connection = NULL;
		}
		pthread_mutex_unlock (&connected_mutex);

		pending_connections = [ips count];
		for (int i = 0; i < [ips count]; i++) {
			XamarinHttpConnection *connection = [[[XamarinHttpConnection alloc] init] autorelease];
			connection.ip = [ips objectAtIndex: i];
			connection.uniqueRequest = unique_request;
			[connections addObject: connection];
			[connection connect: [ips objectAtIndex: i] port: monodevelop_port completionHandler: ^void (bool success)
			{
				LOG_HTTP ("Connected: %@: %i", connection, success);
				pthread_mutex_lock (&connected_mutex);
				if (success) {
					if (connected_connection == NULL) {
						connected_ip = [connection ip];
						connected_connection = connection;
					}
				}
				pending_connections--;
				pthread_cond_signal (&connected_event);
				pthread_mutex_unlock (&connected_mutex);
			}];
		}

		unique_request = false;

		LOG_HTTP ("Will wait for connections");
		pthread_mutex_lock (&connected_mutex);
		while (connected_connection == NULL && pending_connections > 0)
			pthread_cond_wait (&connected_event, &connected_mutex);
		pthread_mutex_unlock (&connected_mutex);
		if (connected_connection == NULL) {
			pthread_mutex_lock (&mutex);
			connection_failed = true;
			pthread_cond_signal (&cond);
			pthread_mutex_unlock (&mutex);
			break;
		}
		LOG_HTTP ("Connection received fd: %i", connected_connection.fileDescriptor);
	} while (monotouch_process_connection (connected_connection.fileDescriptor));

	return;
}

void
monotouch_connect_wifi (NSMutableArray *ips)
{
	// COOP: this is at startup and doesn't access managed memory, so we should be in safe mode here.
	MONO_ASSERT_GC_STARTING;
	
	int listen_port = monodevelop_port;
	unsigned char sockaddr[sizeof (struct sockaddr_in6)];
	struct sockaddr_in6 *sin6 = (struct sockaddr_in6 *) sockaddr;
	struct sockaddr_in *sin = (struct sockaddr_in *) sockaddr;
	int family, waiting, len, rv, i;
	int ip_count = [ips count];
	const char *family_str;
	int connected;
	const char *ip;
	int *sockets;
	long flags;
	
	if (ip_count == 0) {
		PRINT (PRODUCT ": No IPs to connect to.");
		return;
	}
	
	sockets = (int *) malloc (sizeof (int) * ip_count);
	for (i = 0; i < ip_count; i++)
		sockets[i] = -2;
	
	// Open a socket and try to establish a connection for each IP
	do {
		waiting = 0;
		connected = -1;
		for (i = 0; i < ip_count; i++) {
			if (sockets [i] == -1)
				continue;

			ip = [[ips objectAtIndex:i] UTF8String];
			
			memset (sockaddr, 0, sizeof (sockaddr));
			
			// Parse the host IP, assuming IPv4 and falling back to IPv6
			if ((rv = inet_pton (AF_INET, ip, &sin->sin_addr)) == 1) {
				len = sin->sin_len = sizeof (struct sockaddr_in);
				family = sin->sin_family = AF_INET;
				sin->sin_port = htons (listen_port);
				family_str = "IPv4";
			} else if (rv == 0 && (rv = inet_pton (AF_INET6, ip, &sin6->sin6_addr)) == 1) {
				len = sin6->sin6_len = sizeof (struct sockaddr_in6);
				family = sin6->sin6_family = AF_INET6;
				sin6->sin6_port = htons (listen_port);
				family_str = "IPv6";
			} else {
				PRINT (PRODUCT ": Error parsing '%s': %s", ip, errno ? strerror (errno) : "unsupported address type");
				sockets[i] = -1;
				continue;
			}
			
			if ((sockets[i] = socket (family, SOCK_STREAM, IPPROTO_TCP)) == -1) {
				PRINT (PRODUCT ": Failed to create %s socket: %s", family_str, strerror (errno));
				continue;
			}
			
			// Make the socket non-blocking
			flags = fcntl (sockets[i], F_GETFL, NULL);
			fcntl (sockets[i], F_SETFL, flags | O_NONBLOCK);
			
			// Connect to the host
			if ((rv = connect (sockets[i], (struct sockaddr *) sockaddr, len)) == 0) {
				// connection completed, this is our man.
				connected = i;
				break;
			}
			
			if (rv < 0 && errno != EINPROGRESS) {
				PRINT (PRODUCT ": Failed to connect to %s on port %d: %s", ip, listen_port, strerror (errno));
				close (sockets[i]);
				sockets[i] = -1;
				continue;
			}
			
			// asynchronous connect
			waiting++;
		}
	
		// Wait for async socket connections to become available
		while (connected == -1 && waiting > 0) {
			socklen_t optlen = sizeof (int);
			fd_set rset, wset, xset;
			struct timeval tv;
			int max_fd = -1;
			int error;
			
			tv.tv_sec = 2;
			tv.tv_usec = 0;
			
			FD_ZERO (&rset);
			FD_ZERO (&wset);
			FD_ZERO (&xset);
			
			for (i = 0; i < ip_count; i++) {
				if (sockets[i] < 0)
					continue;
				
				max_fd = MAX (max_fd, sockets[i]);
				FD_SET (sockets[i], &rset);
				FD_SET (sockets[i], &wset);
				FD_SET (sockets[i], &xset);
			}
			
			if ((rv = select (max_fd + 1, &rset, &wset, &xset, &tv)) == 0) {
				// timeout hit, no connections available.
				free (sockets);
				return;
			}
			
			if (rv < 0) {
				if (errno == EINTR || errno == EAGAIN)
					continue;
				
				// irrecoverable error
				PRINT (PRODUCT ": Error while waiting for connections: %s", strerror (errno));
				free (sockets);
				return;
			}
			
			for (i = 0; i < ip_count; i++) {
				if (sockets[i] < 0)
					continue;
				
				if (FD_ISSET (sockets[i], &xset)) {
					// exception on this socket
					close (sockets[i]);
					sockets[i] = -1;
					waiting--;
					continue;
				}
				
				if (!FD_ISSET (sockets[i], &rset) && !FD_ISSET (sockets[i], &wset)) {
					// still waiting...
					continue;
				}
				
				// okay, this socket is ready for reading or writing...
				if (getsockopt (sockets[i], SOL_SOCKET, SO_ERROR, &error, &optlen) < 0) {
					PRINT (PRODUCT ": Error while trying to get socket options for %s: %s", [[ips objectAtIndex:i] UTF8String], strerror (errno));
					close (sockets[i]);
					sockets[i] = -1;
					waiting--;
					continue;
				}
				
				if (error != 0) {
					PRINT (PRODUCT ": Socket error while connecting to IDE on %s:%d: %s", [[ips objectAtIndex:i] UTF8String], listen_port, strerror (error));
					close (sockets[i]);
					sockets[i] = -1;
					waiting--;
					continue;
				}
				
				// success!
				connected = i;
				break;
			}
		}
	
		if (connected == -1) {
			free (sockets);
			return;
		}
	
		// close the remaining sockets
		for (i = 0; i < ip_count; i++) {
			if (i == connected || sockets[i] < 0)
				continue;
			
			close (sockets[i]);
			sockets[i] = -1;
		}
	
		LOG (PRODUCT ": Established connection with the IDE (fd: %i)\n", sockets [connected]);
	} while (monotouch_process_connection (sockets [connected]));

	free (sockets);

	return;
}

void
monotouch_connect_usb ()
{
	// COOP: this is at startup and doesn't access managed memory, so we should be in safe mode here.
	MONO_ASSERT_GC_STARTING;
	
	int listen_port = monodevelop_port;
	struct sockaddr_in listen_addr;
	int listen_socket = -1;
	int fd;
	socklen_t len;
	int rv;
	fd_set rset;
	struct timeval tv;
	struct timeval start;
	struct timeval now;
	int flags;
	
	// Create the listen socket and set it up
	listen_socket = socket (PF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (listen_socket == -1) {
		PRINT (PRODUCT ": Could not create socket for the IDE to connect to: %s", strerror (errno));
		return;
	}
	
	flags = 1;
	if (setsockopt (listen_socket, SOL_SOCKET, SO_REUSEADDR, &flags, sizeof (flags)) == -1) {
		PRINT (PRODUCT ": Could not set SO_REUSEADDR on the listening socket (%s)", strerror (errno));
		// not a fatal failure
	}
	
	// Bind
	memset (&listen_addr, 0, sizeof (listen_addr));
	listen_addr.sin_family = AF_INET;
	listen_addr.sin_port = htons (listen_port);
	listen_addr.sin_addr.s_addr = INADDR_ANY;
	rv = bind (listen_socket, (struct sockaddr *) &listen_addr, sizeof (listen_addr));
	if (rv == -1) {
		PRINT (PRODUCT ": Could not bind to address: %s", strerror (errno));
		goto cleanup;
	}

	// Make the socket non-blocking
	flags = fcntl (listen_socket, F_GETFL, NULL);
	flags |= O_NONBLOCK;
	fcntl (listen_socket, F_SETFL, flags);

	rv = listen (listen_socket, 1);
	if (rv == -1) {
		PRINT (PRODUCT ": Could not listen for the IDE: %s", strerror (errno));
		goto cleanup;
	}

	// Wait for connections
	start.tv_sec = 0;
	start.tv_usec = 0;
	do {
		FD_ZERO (&rset);
		FD_SET (listen_socket, &rset);

		do {
			// Calculate how long we can wait if we can only work for 2s since we started
			gettimeofday (&now, NULL);
			if (start.tv_sec == 0) {
				start.tv_sec = now.tv_sec;
				start.tv_usec = now.tv_usec;
				tv.tv_sec = 2;
				tv.tv_usec = 0;
			} else if ((start.tv_sec + 2 == now.tv_sec && start.tv_usec < now.tv_usec) || start.tv_sec + 2 < now.tv_sec) {
				// timeout
			} else {
				tv.tv_sec = start.tv_sec + 2 - now.tv_sec;
				if (start.tv_usec > now.tv_usec) {
					tv.tv_usec = start.tv_usec - now.tv_usec;
				} else {
					tv.tv_sec--;
					tv.tv_usec = 1000000 + start.tv_usec - now.tv_usec;
				}
			}

			// LOG (PRODUCT ": Waiting for connections from the IDE, sec: %i usec: %i\n", (int) tv.tv_sec, (int) tv.tv_usec);

			if ((rv = select (listen_socket + 1, &rset, NULL, NULL, &tv)) == 0) {
				// timeout hit, no connections available.
				LOG (PRODUCT ": Listened for connections from the IDE for 2 seconds, nobody connected.\n");
				goto cleanup;
			}
		} while (rv == -1 && errno == EINTR);

		if (rv == -1) {
			PRINT (PRODUCT ": Failed while waiting for the IDE to connect: %s", strerror (errno));
			goto cleanup;
		}

		len = sizeof (struct sockaddr_in);
		fd = accept (listen_socket, (struct sockaddr *) &listen_addr, &len);
		if (fd == -1) {
			PRINT (PRODUCT ": Failed to accept connection from the IDE: %s", strerror (errno));
			goto cleanup;
		}

		flags = 1;
		if (setsockopt (fd, IPPROTO_TCP, TCP_NODELAY, (char *) &flags, sizeof (flags)) < 0) {
			PRINT (PRODUCT ": Could not set TCP_NODELAY on socket (%s)", strerror (errno));
			// not a fatal failure
		}

		LOG (PRODUCT ": Successfully received USB connection from the IDE on port %i, fd: %i\n", listen_port, fd);
	} while (monotouch_process_connection (fd));

	LOG (PRODUCT ": Successfully talked to the IDE. Will continue startup now.\n");

cleanup:
	close (listen_socket);
	return;
}

void
monotouch_dump_objc_api (Class klass)
{
	unsigned int c;
	Ivar *vars;
	Method *methods;
	objc_property_t *props;
	
	printf ("Dumping class %p = %s\n", klass, class_getName (klass));
	
	vars = class_copyIvarList (klass, &c);
	printf ("\t%i instance variables:\n", c);
	for (int i = 0; i < c; i++)
		printf ("\t\t#%i: %s\n", i + 1, ivar_getName (vars [i]));
	free (vars);
	
	methods = class_copyMethodList (klass, &c);
	printf ("\t%i instance methods:\n", c);
	for (int i = 0; i < c; i++)
		printf ("\t\t#%i: %s\n", i + 1, sel_getName (method_getName (methods [i])));
	free (methods);
	
	props = class_copyPropertyList (klass, &c);
	printf ("\t%i instance properties:\n", c);
	for (int i = 0; i < c; i++)
		printf ("\t\t#%i: %s\n", i + 1, property_getName (props [i]));
	free (props);
	
	fflush (stdout);
}

void
monotouch_load_debugger ()
{
	// COOP: this is at startup and doesn't access managed memory, so we should be in safe mode here.
	MONO_ASSERT_GC_STARTING;
	
	// main thread only 
	if (sdb_fd != -1) {
		DebuggerTransport transport;
		transport.name = "custom_transport";
		transport.connect = sdb_connect;
		transport.close1 = sdb_close1;
		transport.close2 = sdb_close2;
		transport.send = sdb_send;
		transport.recv = sdb_recv;

		mono_debugger_agent_register_transport (&transport);
	
		mono_debugger_agent_parse_options ("transport=custom_transport,address=dummy,embedding=1");

		LOG (PRODUCT ": Debugger loaded with custom transport (fd: %i)\n", sdb_fd);
	} else {
		LOG (PRODUCT ": Debugger not loaded (disabled).\n");
	}
}

void
monotouch_load_profiler ()
{
	// COOP: this is at startup and doesn't access managed memory, so we should be in safe mode here.
	MONO_ASSERT_GC_STARTING;
	
	// TODO: make this generic enough for other profilers to work too
	// Main thread only
	if (profiler_description != NULL) {
		mono_profiler_load (profiler_description);

		LOG (PRODUCT ": Profiler loaded: %s\n", profiler_description);
		free (profiler_description);
		profiler_description = NULL;
	} else {
		LOG (PRODUCT ": Profiler not loaded (disabled)\n");
	}
}

// returns true if it's necessary to create more
// connections to process more data.
bool
monotouch_process_connection (int fd)
{
	// COOP: should be in safe mode here. If that's not the case, at the very least need to switch to safe mode when calling pthread_mutex_lock.
	MONO_ASSERT_GC_STARTING;
	
	// make sure the fd/socket blocks on reads/writes
	fcntl (fd, F_SETFL, fcntl (fd, F_GETFL, NULL) & ~O_NONBLOCK);

	while (true) {
		char command [257];
		int rv;
		unsigned char cmd_len;

		rv = recv_uninterrupted (fd, &cmd_len, 1);
		if (rv <= 0) {
			LOG (PRODUCT ": Error while receiving command from the IDE (%s)\n", strerror (errno));
			return false;
		}

		rv = recv_uninterrupted (fd, command, cmd_len);
		if (rv <= 0) {
			LOG (PRODUCT ": Error while receiving command from the IDE (%s)\n", strerror (errno));
			return false;
		}
		
		// null-terminate
		command [cmd_len] = 0;

		LOG (PRODUCT ": Processing: '%s'\n", command);
		
		if (!strcmp (command, "connect output")) {
			dup2 (fd, 1);
			dup2 (fd, 2);
			return true; 
		} else if (!strcmp (command, "connect stdout")) {
			dup2 (fd, 1);
			return true;
		} else if (!strcmp (command, "connect stderr")) {
			dup2 (fd, 2);
			return true;
		} else if (!strcmp (command, "discard")) {
			return true;
		} else if (!strcmp (command, "ping")) {
			if (!send_uninterrupted (fd, "pong", 5))
				LOG (PRODUCT ": Got keepalive request from the IDE, but could not send response back (%s)\n", strerror (errno));
		} else if (!strcmp (command, "exit process")) {
			LOG (PRODUCT ": The IDE requested an exit, will exit immediately.\n");
			fflush (stderr);
			exit (0);
		} else if (!strncmp (command, "start debugger: ", 16)) {
			const char *debugger = command + 16;
			bool use_fd = false;
			if (!strcmp (debugger, "no")) {
				/* disabled */
			} else if (!strcmp (debugger, "sdb")) {
				sdb_fd = fd;
				use_fd = true;
			}
			debugging_configured = true;
			MONO_ASSERT_GC_STARTING;
			pthread_mutex_lock (&mutex);
			pthread_cond_signal (&cond);
			pthread_mutex_unlock (&mutex);
			if (use_fd)
				return true;
		} else if (!strncmp (command, "start profiler: ", 16)) {
			// initialize the log profiler if we're debugging
			const char *prof = command + 16;
			bool use_fd = false;
			
			if (!strcmp (prof, "no")) {
				/* disabled */
			} else if (!strncmp (prof, "log:", 4)) {
#if defined(__i386__) || defined (__x86_64__)
				profiler_description = strdup (prof);
#else
				use_fd = true;
				profiler_fd = fd;
				profiler_description = xamarin_strdup_printf ("%s,output=#%i", prof, profiler_fd);
#endif
				xamarin_set_gc_pump_enabled (false);
			} else {
				LOG (PRODUCT ": Unknown profiler, expect unexpected behavior (%s)\n", prof);
				profiler_description = strdup (prof);
			}
			profiler_configured = true;
			MONO_ASSERT_GC_STARTING;
			pthread_mutex_lock (&mutex);
			pthread_cond_signal (&cond);
			pthread_mutex_unlock (&mutex);
			if (use_fd)
				return true;
		} else if (!strncmp (command, "set heapshot port: ", 19)) {
			heapshot_port = strtol (command + 19, NULL, 0);
			LOG (PRODUCT ": HeapShot port is now: %i\n", heapshot_port);
		} else if (!strcmp (command, "heapshot")) {
			if (heapshot_fd == -1) {
				struct sockaddr_in heapshot_addr;

				memset (&heapshot_addr, 0, sizeof (heapshot_addr));
				heapshot_addr.sin_len = sizeof (heapshot_addr);
				heapshot_addr.sin_port = htons (heapshot_port);
				heapshot_addr.sin_addr.s_addr = htonl (INADDR_LOOPBACK);
				heapshot_addr.sin_family = AF_INET;

				if ((heapshot_fd = socket (AF_INET, SOCK_STREAM, IPPROTO_TCP)) == -1) {
					LOG (PRODUCT ": Failed to create socket to connect to profiler: %s\n", strerror (errno));
				} else if (connect (heapshot_fd, (struct sockaddr *) &heapshot_addr, sizeof (heapshot_addr)) != 0) {
					LOG (PRODUCT ": Failed to connect to profiler to request a heapshot: %s\n", strerror (errno));
					close (heapshot_fd);
					heapshot_fd = -1;
				} else {
					// Success!
				}
			}
			if (heapshot_fd != -1) {
				if (!send_uninterrupted (heapshot_fd, "heapshot\n", 9))
					LOG (PRODUCT ": Failed to request heapshot: %s\n", strerror (errno));
			}
		} else {
			LOG (PRODUCT ": Unknown command received from the IDE: '%s'\n", command);
		}
	}
}

int monotouch_debug_listen (int debug_port, int output_port)
{
	struct sockaddr_in listen_addr;
	int listen_socket;
	int output_socket;
	socklen_t len;
	int rv;
	long flags;
	int flag;
	fd_set rset;
	struct timeval tv;
	
	// Create the listen socket and set it up
	listen_socket = socket (PF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (listen_socket == -1) {
		PRINT (PRODUCT ": Could not create socket for the IDE to connect to: %s", strerror (errno));
		return 1;
	} else {
		flag = 1;
		if (setsockopt (listen_socket, SOL_SOCKET, SO_REUSEADDR, &flag, sizeof (flag)) == -1) {
			PRINT (PRODUCT ": Could not set SO_REUSEADDR on the listening socket (%s)", strerror (errno));
			// not a fatal failure
		}

		memset (&listen_addr, 0, sizeof (listen_addr));
		listen_addr.sin_family = AF_INET;
		listen_addr.sin_port = htons (output_port);
		listen_addr.sin_addr.s_addr = INADDR_ANY;
		rv = bind (listen_socket, (struct sockaddr *) &listen_addr, sizeof (listen_addr));
		if (rv == -1) {
			PRINT (PRODUCT ": Could not bind to address: %s", strerror (errno));
			close (listen_socket);
			return 2;
		} else {
			// Make the socket non-blocking
			flags = fcntl (listen_socket, F_GETFL, NULL);
			flags |= O_NONBLOCK;
			fcntl (listen_socket, F_SETFL, flags);

			rv = listen (listen_socket, 1);
			if (rv == -1) {
				PRINT (PRODUCT ": Could not listen for the IDE: %s", strerror (errno));
				close (listen_socket);
				return 2;
			} else {
				// Yay!
			}
		}
	}

	tv.tv_sec = 2;
	tv.tv_usec = 0;
	
	FD_ZERO (&rset);
	FD_SET (listen_socket, &rset);
	
	do {
		if ((rv = select (listen_socket + 1, &rset, NULL, NULL, &tv)) == 0) {
			// timeout hit, no connections available.
			PRINT (PRODUCT ": Listened for connections from the IDE for 2 seconds, nobody connected.");
			close (listen_socket);
			return 3;
		}
	} while (rv == -1 && errno == EINTR);
	
	if (rv == -1) {
		PRINT (PRODUCT ": Failed while waiting for the IDE to connect: %s", strerror (errno));
		close (listen_socket);
		return 2;
	}

	len = sizeof (struct sockaddr_in);
	output_socket = accept (listen_socket, (struct sockaddr *) &listen_addr, &len);
	if (output_socket == -1) {
		PRINT (PRODUCT ": Failed to accept connection from the IDE: %s", strerror (errno));
		close (listen_socket);
		return 3;
	}

	flag = 1;
	if (setsockopt (output_socket, IPPROTO_TCP, TCP_NODELAY, (char *) &flag, sizeof (flag)) < 0) {
		PRINT (PRODUCT ": Could not set TCP_NODELAY on socket (%s)", strerror (errno));
		// not a fatal failure
	}
		
	LOG (PRODUCT ": Successfully received USB connection from the IDE on port %i.\n", output_port);

	// make the socket block on reads/writes
	flags = fcntl (output_socket, F_GETFL, NULL);
	fcntl (output_socket, F_SETFL, flags & ~O_NONBLOCK);

	dup2 (output_socket, 1);
	dup2 (output_socket, 2);

	close (listen_socket); // no need to listen anymore
 
	debug_host = strdup ("127.0.0.1");

	return 0;
}

// SUCCESS = 0
// FAILURE > 0
int monotouch_debug_connect (NSMutableArray *ips, int debug_port, int output_port)
{
	// COOP: this is at startup and doesn't access managed memory, so we should be in safe mode here.
	MONO_ASSERT_GC_STARTING;
	
	unsigned char sockaddr[sizeof (struct sockaddr_in6)];
	struct sockaddr_in6 *sin6 = (struct sockaddr_in6 *) sockaddr;
	struct sockaddr_in *sin = (struct sockaddr_in *) sockaddr;
	int family, waiting, len, rv, i;
	int ip_count = [ips count];
	const char *family_str;
	int connected = -1;
	const char *ip;
	int *sockets;
	long flags;
	
	if (ip_count == 0) {
		PRINT (PRODUCT ": No IPs to connect to.");
		return 2;
	}
	
	sockets = (int *) malloc (sizeof (int) * ip_count);
	for (i = 0; i < ip_count; i++)
		sockets[i] = -1;
	
	// Open a socket and try to establish a connection for each IP
	waiting = 0;
	for (i = 0; i < ip_count; i++) {
		ip = [[ips objectAtIndex:i] UTF8String];
		
		memset (sockaddr, 0, sizeof (sockaddr));
		
		// Parse the host IP, assuming IPv4 and falling back to IPv6
		if ((rv = inet_pton (AF_INET, ip, &sin->sin_addr)) == 1) {
			len = sin->sin_len = sizeof (struct sockaddr_in);
			family = sin->sin_family = AF_INET;
			sin->sin_port = htons (output_port);
			family_str = "IPv4";
		} else if (rv == 0 && (rv = inet_pton (AF_INET6, ip, &sin6->sin6_addr)) == 1) {
			len = sin6->sin6_len = sizeof (struct sockaddr_in6);
			family = sin6->sin6_family = AF_INET6;
			sin6->sin6_port = htons (output_port);
			family_str = "IPv6";
		} else {
			PRINT (PRODUCT ": Error parsing '%s': %s", ip, errno ? strerror (errno) : "unsupported address type");
			sockets[i] = -1;
			continue;
		}
		
		if ((sockets[i] = socket (family, SOCK_STREAM, IPPROTO_TCP)) == -1) {
			PRINT (PRODUCT ": Failed to create %s socket: %s", family_str, strerror (errno));
			continue;
		}
		
		// Make the socket non-blocking
		flags = fcntl (sockets[i], F_GETFL, NULL);
		fcntl (sockets[i], F_SETFL, flags | O_NONBLOCK);
		
		// Connect to the host
		if ((rv = connect (sockets[i], (struct sockaddr *) sockaddr, len)) == 0) {
			// connection completed, this is our man.
			connected = i;
			break;
		}
		
		if (rv < 0 && errno != EINPROGRESS) {
			PRINT (PRODUCT ": Failed to connect to %s on port %d: %s", ip, output_port, strerror (errno));
			close (sockets[i]);
			sockets[i] = -1;
			continue;
		}
		
		// asynchronous connect
		waiting++;
	}
	
	// Wait for async socket connections to become available
	while (connected == -1 && waiting > 0) {
		socklen_t optlen = sizeof (int);
		fd_set rset, wset, xset;
		struct timeval tv;
		int max_fd = -1;
		int error;
		
		tv.tv_sec = 2;
		tv.tv_usec = 0;
		
		FD_ZERO (&rset);
		FD_ZERO (&wset);
		FD_ZERO (&xset);
		
		for (i = 0; i < ip_count; i++) {
			if (sockets[i] < 0)
				continue;
			
			max_fd = MAX (max_fd, sockets[i]);
			FD_SET (sockets[i], &rset);
			FD_SET (sockets[i], &wset);
			FD_SET (sockets[i], &xset);
		}
		
		if ((rv = select (max_fd + 1, &rset, &wset, &xset, &tv)) == 0) {
			// timeout hit, no connections available.
			free (sockets);
			return 1;
		}
		
		if (rv < 0) {
			if (errno == EINTR || errno == EAGAIN)
				continue;
			
			// irrecoverable error
			PRINT (PRODUCT ": Error while waiting for connections: %s", strerror (errno));
			free (sockets);
			return 1;
		}
		
		for (i = 0; i < ip_count; i++) {
			if (sockets[i] < 0)
				continue;
			
			if (FD_ISSET (sockets[i], &xset)) {
				// exception on this socket
				close (sockets[i]);
				sockets[i] = -1;
				waiting--;
				continue;
			}
			
			if (!FD_ISSET (sockets[i], &rset) && !FD_ISSET (sockets[i], &wset)) {
				// still waiting...
				continue;
			}
			
			// okay, this socket is ready for reading or writing...
			if (getsockopt (sockets[i], SOL_SOCKET, SO_ERROR, &error, &optlen) < 0) {
				PRINT (PRODUCT ": Error while trying to get socket options for %s: %s", [[ips objectAtIndex:i] UTF8String], strerror (errno));
				close (sockets[i]);
				sockets[i] = -1;
				waiting--;
				continue;
			}
			
			if (error != 0) {
				PRINT (PRODUCT ": Socket error while connecting to the IDE on %s:%d: %s", [[ips objectAtIndex:i] UTF8String], output_port, strerror (error));
				close (sockets[i]);
				sockets[i] = -1;
				waiting--;
				continue;
			}
			
			// success!
			connected = i;
			break;
		}
	}
	
	if (connected == -1) {
		free (sockets);
		return 1;
	}
	
	// make the socket block on reads/writes
	flags = fcntl (sockets[connected], F_GETFL, NULL);
	fcntl (sockets[connected], F_SETFL, flags & ~O_NONBLOCK);
 
	LOG (PRODUCT ": Connected output to the IDE on %s:%d\n", [[ips objectAtIndex:i] UTF8String], output_port);

	dup2 (sockets[connected], 1);
	dup2 (sockets[connected], 2);

	debug_host = strdup ([[ips objectAtIndex:connected] UTF8String]);
	
	// close the remaining sockets
	for (i = 0; i < ip_count; i++) {
		if (i == connected || sockets[i] < 0)
			continue;
		
		close (sockets[i]);
		sockets[i] = -1;
	}
	
	free (sockets);
	
	return 0;
}

#if TARGET_OS_WATCH && !TARGET_OS_SIMULATOR
#include <sys/param.h>
extern "C" {
// from sys/proc_info.h
// this header is not present for device architectures (none of them), but the
// function is still available on device, so still use it. It's obviously not public
// API, but then again this is just for debugging purposes and only included
// in debug builds, so it will not be shipped to the App Store.
#define PROC_PIDT_SHORTBSDINFO 13
#define PROC_FLAG_TRACED        2
struct proc_bsdshortinfo {
	uint32_t  pbsi_pid;
	uint32_t  pbsi_ppid;
	uint32_t  pbsi_pgid;
	uint32_t  pbsi_status;
	char      pbsi_comm [MAXCOMLEN];
	uint32_t  pbsi_flags;
	uid_t     pbsi_uid;
	gid_t     pbsi_gid;
	uid_t     pbsi_ruid;
	gid_t     pbsi_rgid;
	uid_t     pbsi_svuid;
	gid_t     pbsi_svgid;
	uint32_t  pbsi_rfu;
};
int proc_pidinfo(int pid, int flavor, uint64_t arg, void *buffer, int buffersize);
}

bool
xamarin_is_native_debugger_attached ()
{
	struct proc_bsdshortinfo info;
	int rv = proc_pidinfo (getpid (), PROC_PIDT_SHORTBSDINFO, 0, (void *) &info, sizeof (info));
	// I couldn't find any documentation for the return value for proc_pidinfo,
	// but it seems to always be the size of the proc_bsdshortinfo struct, so
	// only accept any output if that's the case. We do not want to think that
	// the debugger is attached when it really isn't, since then the app would
	// just be killed upon launch by watchOS when hitting the launch watchdog.
	if (rv != sizeof (info)) {
		LOG (PRODUCT ": Couldn't get process info to determine whether a native debugger is attached (%i: %s)", errno, strerror (errno));
		return false;
	}

	return info.pbsi_flags & PROC_FLAG_TRACED;
}
#endif /* TARGET_OS_WATCH && !TARGET_OS_SIMULATOR */

#else
int fix_ranlib_warning_about_no_symbols_v2;
#endif /* DEBUG */

