using System;
using System.IO;

namespace nnyeah {
    class Program {
        static void Main (string [] args)
        {
            if (args.Length != 2) {
                Console.Error.WriteLine ("Usage: mono nnyeah.exe /path/to/input/file.dll /path/to/output/file.dll");
                Environment.Exit (1);
            }
            using (var stm = new FileStream (args [0], FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                var reworker = new Reworker (stm);
                reworker.Load ();
                if (reworker.NeedsReworking ()) {
                    using (var ostm = new FileStream (args [1], FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) {
                        reworker.Rework (ostm);
                    }
                }
            }
        }
    }
}
