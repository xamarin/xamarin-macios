using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xharness.Execution;
using Xharness.Logging;

namespace Xharness.Hardware {

	public interface ITCCDatabase {
		IProcessManager ProcessManager { get; set; }
		Task AgreeToPromptsAsync (string simRuntime, string dataPath, ILog log, params string [] bundle_identifiers);
		int GetTCCFormat (string simRuntime);
	}

	public class TCCDatabase : ITCCDatabase {
		static readonly string iOSSimRuntimePrefix = "com.apple.CoreSimulator.SimRuntime.iOS-";
		static readonly string tvOSSimRuntimePrefix = "com.apple.CoreSimulator.SimRuntime.tvOS-";
		static readonly string watchOSRuntimePrefix = "com.apple.CoreSimulator.SimRuntime.watchOS-";

		public IProcessManager ProcessManager { get; set; } = new ProcessManager ();

		public int GetTCCFormat (string simRuntime) {
			
			// v1: < iOS 9
			// v2: >= iOS 9 && < iOS 12
			// v3: >= iOS 12
			if (simRuntime.StartsWith (iOSSimRuntimePrefix, StringComparison.Ordinal)) {
				var v = Version.Parse (simRuntime.Substring (iOSSimRuntimePrefix.Length).Replace ('-', '.'));
				if (v.Major >= 12) {
					return 3;
				} else if (v.Major >= 9) {
					return 2;
				} else {
					return 1;
				}
			} else if (simRuntime.StartsWith (tvOSSimRuntimePrefix, StringComparison.Ordinal)) {
				var v = Version.Parse (simRuntime.Substring (tvOSSimRuntimePrefix.Length).Replace ('-', '.'));
				if (v.Major >= 12) {
					return 3;
				} else {
					return 2;
				}
			} else if (simRuntime.StartsWith (watchOSRuntimePrefix, StringComparison.Ordinal)) {
				var v = Version.Parse (simRuntime.Substring (watchOSRuntimePrefix.Length).Replace ('-', '.'));
				if (v.Major >= 5) {
					return 3;
				} else {
					return 2;
				}
			} else {
				throw new NotImplementedException ();
			}
		}

		public async Task AgreeToPromptsAsync (string simRuntime, string TCCDb, ILog log, params string [] bundle_identifiers)
		{
			if (bundle_identifiers == null || bundle_identifiers.Length == 0) {
				log.WriteLine ("No bundle identifiers given when requested permission editing.");
				return;
			}

			var sim_services = new string [] {
					"kTCCServiceAddressBook",
					"kTCCServiceCalendar",
					"kTCCServicePhotos",
					"kTCCServiceMediaLibrary",
					"kTCCServiceMicrophone",
					"kTCCServiceUbiquity",
					"kTCCServiceWillow"
				};

			var failure = false;
			var tcc_edit_timeout = 30;
			var watch = new Stopwatch ();
			watch.Start ();

			do {
				if (failure) {
					log.WriteLine ("Failed to edit TCC.db, trying again in 1 second... ", (int)(tcc_edit_timeout - watch.Elapsed.TotalSeconds));
					await Task.Delay (TimeSpan.FromSeconds (1));
				}
				failure = false;
				foreach (var bundle_identifier in bundle_identifiers) {
					var args = new List<string> ();
					var sql = new System.Text.StringBuilder ();
					args.Add (TCCDb);
					foreach (var service in sim_services) {
						switch (GetTCCFormat (simRuntime)) {
						case 1:
							// CREATE TABLE access (service TEXT NOT NULL, client TEXT NOT NULL, client_type INTEGER NOT NULL, allowed INTEGER NOT NULL, prompt_count INTEGER NOT NULL, csreq BLOB, CONSTRAINT key PRIMARY KEY (service, client, client_type));
							sql.AppendFormat ("INSERT INTO access VALUES('{0}','{1}',0,1,0,NULL);", service, bundle_identifier);
							sql.AppendFormat ("INSERT INTO access VALUES('{0}','{1}',0,1,0,NULL);", service, bundle_identifier + ".watchkitapp");
							break;
						case 2:
							// CREATE TABLE access (service	TEXT NOT NULL, client TEXT NOT NULL, client_type INTEGER NOT NULL, allowed INTEGER NOT NULL, prompt_count INTEGER NOT NULL, csreq BLOB, policy_id INTEGER, PRIMARY KEY (service, client, client_type), FOREIGN KEY (policy_id) REFERENCES policies(id) ON DELETE CASCADE ON UPDATE CASCADE);
							sql.AppendFormat ("INSERT INTO access VALUES('{0}','{1}',0,1,0,NULL,NULL);", service, bundle_identifier);
							sql.AppendFormat ("INSERT INTO access VALUES('{0}','{1}',0,1,0,NULL,NULL);", service, bundle_identifier + ".watchkitapp");
							break;
						case 3: // Xcode 10+
							// CREATE TABLE access (    service        TEXT        NOT NULL,     client         TEXT        NOT NULL,     client_type    INTEGER     NOT NULL,     allowed        INTEGER     NOT NULL,     prompt_count   INTEGER     NOT NULL,     csreq          BLOB,     policy_id      INTEGER,     indirect_object_identifier_type    INTEGER,     indirect_object_identifier         TEXT,     indirect_object_code_identity      BLOB,     flags          INTEGER,     last_modified  INTEGER     NOT NULL DEFAULT (CAST(strftime('%s','now') AS INTEGER)),     PRIMARY KEY (service, client, client_type, indirect_object_identifier),    FOREIGN KEY (policy_id) REFERENCES policies(id) ON DELETE CASCADE ON UPDATE CASCADE)
							sql.AppendFormat ("INSERT OR REPLACE INTO access VALUES('{0}','{1}',0,1,0,NULL,NULL,NULL,'UNUSED',NULL,NULL,{2});", service, bundle_identifier, DateTimeOffset.Now.ToUnixTimeSeconds ());
							sql.AppendFormat ("INSERT OR REPLACE INTO access VALUES('{0}','{1}',0,1,0,NULL,NULL,NULL,'UNUSED',NULL,NULL,{2});", service, bundle_identifier + ".watchkitapp", DateTimeOffset.Now.ToUnixTimeSeconds ());
							break;
						default:
							throw new NotImplementedException ();
						}
					}
					args.Add (sql.ToString ());
					var rv = await ProcessManager.ExecuteCommandAsync ("sqlite3", args, log, TimeSpan.FromSeconds (5));
					if (!rv.Succeeded) {
						failure = true;
						break;
					}
				}
			} while (failure && watch.Elapsed.TotalSeconds <= tcc_edit_timeout);

			if (failure) {
				log.WriteLine ("Failed to edit TCC.db, the test run might hang due to permission request dialogs");
			} else {
				log.WriteLine ("Successfully edited TCC.db");
			}

			log.WriteLine ("Current TCC database contents:");
			await ProcessManager.ExecuteCommandAsync ("sqlite3", new [] { TCCDb, ".dump" }, log, TimeSpan.FromSeconds (5));
		}
	}
}
