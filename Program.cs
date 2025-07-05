using EmuWarface.Core;
using EmuWarface.Game;
using EmuWarface.Game.Clans;
using EmuWarface.Game.Enums;
using EmuWarface.Game.Shops;
using EmuWarface.Xmpp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace EmuWarface
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //EmuConfig.AchievementsInit();
            Process.Start(new ProcessStartInfo
            {
                FileName = "GeneratorPVE.exe",
                UseShellExecute = false,
            });
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var s = EmuConfig.API;
            sw.Stop();
            Log.Debug(string.Format("EmuConfig {0},{1}s.", sw.Elapsed.Seconds, sw.Elapsed.Milliseconds));
            sw.Restart();
            SQL.Init();
            sw.Stop();
            Log.Debug(string.Format("SQL {0},{1}s.", sw.Elapsed.Seconds, sw.Elapsed.Milliseconds));
            sw.Restart();
            QueryBinder.Init();
            sw.Stop();
            Log.Debug(string.Format("QueryBinder {0},{1}s.", sw.Elapsed.Seconds, sw.Elapsed.Milliseconds));
            sw.Restart();
            QueryCache.Init();
            sw.Stop();
            Log.Debug(string.Format("QueryCache {0},{1}s.", sw.Elapsed.Seconds, sw.Elapsed.Milliseconds));
            GameData.Init();
            sw.Stop();
            Log.Debug(string.Format("GameData {0},{1}s.", sw.Elapsed.Seconds, sw.Elapsed.Milliseconds));
            sw.Restart();
            Shop.Init();
            //ItemL.Init();
            //Achievements.Init();
            Server.Init();
            API.Init(); //TelegramBot настройка в файле Config/api.json и Game/cache/BotsCmd.xml
            Rcon.Init();
            Clan.GenerateClanList();
            EmuExtensions.UpdateOnline();
            //DedicatedServer.DedicatedServerStart();
            GameData.UpdatePVE_PVP();
            Thread.Sleep(-1);
        }
        public static JObject Request(string query)
        {
            WebRequest request = WebRequest.Create(query);
            WebResponse response = request.GetResponse();
            string answer;
            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    answer = reader.ReadToEnd();
                    JObject array2 = (JObject)JsonConvert.DeserializeObject(answer);
                    return array2;
                }
            }
        }
    }
}
