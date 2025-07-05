using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EmuWarface
{
    internal class DedicatedServer
    {
        internal static long PORT = 64090;
        internal static long DEDICATED_STARTET_COUNT;
        internal static string PATH = "WARFACE BUILD 1.22100.2101.41200 DEBUG/Bin64/DedicatedServer.exe";
        public static void DedicatedServerStart()
        {
            DedicateStatus status = (DedicateStatus)EmuConfig.Settings.dedicated_debug;
            DEDICATED_STARTET_COUNT = EmuConfig.Settings.dedicated_count;
            Init(status);
        }
        private enum DedicateStatus
        {
            RELEASE,
            DEV
        }
        private static void Init(DedicateStatus DEDICATEDSTATUS)
        {
            foreach (var mscfg in EmuConfig.MasterServers)
            {
                for (int i = 0; i < DEDICATED_STARTET_COUNT; i++)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = PATH,
                        Arguments = String.Format("+online_server_port {0} +online_masterserver_resource {1} +sv_port {2} +online_server {3} +online_use_tls 1 +online_use_protect 1 +log_Verbosity=0 -simple_console {4} {5}", EmuConfig.Settings.Host, mscfg.Resource, PORT++, EmuConfig.Settings.Host, (DEDICATEDSTATUS == DedicateStatus.RELEASE) ? "-nodevmode" : "", (EmuConfig.Settings.daemon) ? "-daemon" : "")
                    });
                }
            }
        }
    }
}
