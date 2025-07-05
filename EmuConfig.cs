﻿using EmuWarface.Game.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EmuWarface
{
    public static class EmuConfig
    {
        public static XmlElement Load(string path)
        {
            using (XmlReader reader = XmlReader.Create(path, new XmlReaderSettings() { IgnoreComments = true }))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                return doc.DocumentElement;
            }
        }
        public static APIConfig API; 
        public static SettingsConfig Settings; 
        public static SqlConfig Sql; 
        public static GameRoomConfig GameRoom; 
        public static List<MasterServerConfig> MasterServers;
        public static List<DefaultItemConfig> DefaultItems;
        public static List<string> ObsceneWords;
        public static List<XElement> achievements = new List<XElement>();
        public static XmlElement exp { get; private set; }
        static EmuConfig()
        {
            Init();
            Task.Run(() =>
            {
                while (true)
                {
                    Init();
                    Log.Debug("API: Update");
                    Thread.Sleep(120000);
                }
            });
        }
        public static void Init()
        {
            API = LoadConfig<APIConfig>("Config/api.json");
            Settings = LoadConfig<SettingsConfig>("Config/settings.json");
            Sql = LoadConfig<SqlConfig>("Config/sql.json");
            GameRoom = LoadConfig<GameRoomConfig>("Config/room.json");
            MasterServers = LoadConfig<List<MasterServerConfig>>("Config/masterservers.json");
            //TODO сломалось создание профиля
            DefaultItems = LoadConfig<List<DefaultItemConfig>>("Config/defaultItems.json");
            ObsceneWords = LoadConfig<List<string>>("Config/obsceneWords.json");
            ObsceneWords = LoadConfig<List<string>>("Config/obsceneWords.json");
        }
        public static void AchievementsInit()
        {
            exp = Load("Config/exp.xml");
            string[] files;
            files = Directory.GetFiles("Config/achievements", "*.xml", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                XElement xElement = XElement.Load(file);
                achievements.Add(xElement);
            }
        }

        public static T LoadConfig<T>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.CreateText(fileName).Dispose();
                throw new FileNotFoundException();
            }

            using (StreamReader reader = File.OpenText(fileName))
            {
                var text = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(text);
            }
        }
    }

    public class MasterServerConfig
    {
        [JsonProperty("resource")]
        public string Resource { get; set; }
        [JsonProperty("server_id")]
        public int ServerId { get; set; }
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("rank_group")]
        public string RankGroup { get; set; }
        [JsonProperty("min_rank")]
        public int MinRank { get; set; }
        [JsonProperty("max_rank")]
        public int MaxRank { get; set; }
        [JsonProperty("bootstrap")]
        public string Bootstrap { get; set; }
    }

    public class SqlConfig
    {
        [JsonProperty("server")]
        public string Server { get; set; }
        [JsonProperty("user")]
        public string User { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("database")]
        public string Database { get; set; }
        [JsonProperty("characterSet")]
        public string CharacterSet { get; set; }
        [JsonProperty("port")]
        public uint Port { get; set; }
    }

    public class DefaultItemConfig
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public ItemSlot Type { get; set; }
        [JsonProperty("classes")]
        public Class Classes { get; set; }
    }

    public class SettingsConfig
    {
        [JsonProperty("host")]
        public string Host { get; set; }
        [JsonProperty("port")]
        public int Port { get; set; }
        [JsonProperty("rconPort")]
        public int RconPort { get; set; }
        [JsonProperty("rconAllowedHosts")]
        public List<string> RconHosts { get; set; }
        [JsonProperty("dedicatedHosts")]
        public List<string> DedicatedHosts { get; set; }
        [JsonProperty("onlinePath")]
        public string OnlinePath { get; set; }
        [JsonProperty("gameVersion")]
        public string GameVersion { get; set; }
        [JsonProperty("certSecret")]
        public string CertSecret { get; set; }
        [JsonProperty("xmpp_debug")]
        public bool XmppDebug { get; set; }
        [JsonProperty("xmpp_debug_console")]
        public bool XmppDebugConsole { get; set; }
        [JsonProperty("dedicated_debug")]
        public int dedicated_debug { get; set; }
        [JsonProperty("daemon")]
        public bool daemon { get; set; }
        [JsonProperty("dedicated_count")]
        public int dedicated_count { get; set; }
        [JsonProperty("RoomPath")]
        public string RoomPath { get; set; }
    }

    public class APIConfig
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("admins")]
        public List<AdminConfig> Admins { get; set; }
        [JsonProperty("commands")]
        public Dictionary<string, string> Commands { get; set; }
        [JsonProperty("rules")]
        public Dictionary<string, string[]> Rules { get; set; }
    }

    public class GameRoomConfig
    {
        /*
         public const int ROOM_PVP_PUBLIC_MIN_PLAYERS_READY = 2;
        public const int ROOM_PVP_AUTOSTART_MIN_PLAYERS_READY = 2;
        public const int ROOM_PVE_PRIVATE_MIN_PLAYERS_READY = 1;
        public const int ROOM_PVE_AUTOSTART_MIN_PLAYERS_READY = 2;
        public const int ROOM_PVP_CLANWAR_MIN_PLAYERS_READY = 4;
         */
        [JsonProperty("min_players_ready_pvp_public")]
        public int PVP_PUBLIC_MIN_PLAYERS_READY { get; set; }
        [JsonProperty("min_players_ready_pvp_autostart")]
        public int PVP_AUTOSTART_MIN_PLAYERS_READY { get; set; }
        [JsonProperty("min_players_ready_pve_private")]
        public int PVE_PRIVATE_MIN_PLAYERS_READY { get; set; }
        [JsonProperty("min_players_ready_pve_autostart")]
        public int PVE_AUTOSTART_MIN_PLAYERS_READY { get; set; }
        [JsonProperty("min_players_ready_pvp_clanwar")]
        public int PVP_CLANWAR_MIN_PLAYERS_READY { get; set; }
        [JsonProperty("min_players_ready_pvp_rating")]
        public int PVP_RATING_MIN_PLAYERS_READY { get; set; }
    }

    public class AdminConfig
    {
        [JsonProperty("profile_id")]
        public ulong ProfileId { get; set; }
        [JsonProperty("telegram_id")]
        public long TelegramId { get; set; }
        [JsonProperty("status")]
        public UserStatus Status { get; set; }
    }
}
