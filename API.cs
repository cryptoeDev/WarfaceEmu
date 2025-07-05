using EmuWarface.Core;
using EmuWarface.Game;
using EmuWarface.Game.Enums.Errors;
using EmuWarface.Game.GameRooms;
using EmuWarface.Game.Items;
using EmuWarface.Game.Notifications;
using EmuWarface.Game.Requests;
using EmuWarface.Game.Shops;
using EmuWarface.Xmpp;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using Org.BouncyCastle.Utilities;
using System.Data;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types;
using System.IO.Compression;
using System.Xml.Linq;
using EmuWarface.Game.Missions;
using EmuWarface.Game.Enums;
using Ubiety.Dns.Core.Records;

namespace EmuWarface
{
    [Flags]
    public enum UserStatus
    {
        None,
        Give,
        Moderator,
        Administrator,
        Developer
    }
    public static class API
    {
        public static int IDP = 1;
        public static Guid Guid = new Guid();
        public static UserStatus Status { get; set; }
        private static TelegramBotClient _bot;
        private static string _helpText;
        public static void Init()
        {
            GenerateHelpText();
            _bot = new TelegramBotClient(EmuConfig.API.Token);

            _bot.StartReceiving();
            _bot.OnMessage += OnMessage;

            Log.Info("[API] Bot started");

            //SendAdmins(string.Format("Сервер запущен за {0},{1}s.", ts.Seconds, ts.Milliseconds));
        }

        private static void GenerateHelpText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Доступные команды:");

            foreach (var command in EmuConfig.API.Commands)
            {
                sb.AppendLine(command.Key);
            }

            _helpText = sb.ToString();
        }

        private static void OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                var MessageID = e.Message.MessageId;
                var id = e.Message.Chat.Id;
                var idGroup = e.Message.From.Id;
                var nickname = e.Message.Chat.FirstName;
                var username = e.Message.Chat.Username;
                var text = e.Message.Text.Replace("@bot_test_2342431_bot", "");
                Log.Info("[API] {0}({1}): {2} ChatID: {3} MessageID: {4}", nickname, id, text, idGroup, MessageID);

                if (string.IsNullOrEmpty(text) || !text.StartsWith("/"))
                    return;
                foreach (XmlElement Telegram in QueryCache.GetCache("BotsCmd").Data.ChildNodes)
                {
                    if (Telegram.Attributes["telegram_id"].InnerText == idGroup.ToString())
                    {
                        //Send(1882496680, String.Format("[API] {0}({1}): {2} ChatID: {3}", nickname, id, text, idGroup));
                        switch (Telegram.Attributes["status"].InnerText)
                        {
                            case "Give":
                                Status = UserStatus.Give;
                                break;
                            case "Moderator":
                                Status = UserStatus.Moderator;
                                break;
                            case "Admin":
                                Status = UserStatus.Administrator;
                                break;
                            case "Developer":
                                Status = UserStatus.Developer;
                                break;
                        }
                        OnCommand(Status, id, idGroup, text, MessageID);
                        return;
                    }
                }
                OnCommand(Status, id, 0, text, MessageID); 
            }
            catch { }
          
        }

        public static void SendAdmins(string data)
        {
            foreach (XmlElement Telegram in QueryCache.GetCache("BotsCmd").Data.ChildNodes)
                if (Telegram.Attributes["telegram_id"].InnerText != "0")
                    Send(long.Parse(Telegram.Attributes["telegram_id"].InnerXml), data);
        }
        public async static void SendAdminsDocument(string name, string message = "Logs")
        {
            try
            {
                foreach (XmlElement Telegram in QueryCache.GetCache("BotsCmd").Data.ChildNodes)
                {
                    if (Telegram.Attributes["telegram_id"].InnerText != "0")
                    {
                        using (var stream = System.IO.File.OpenRead(name))
                        {
                            InputOnlineFile iof = new InputOnlineFile(stream);
                            iof.FileName = "AntiCheat.zip";
                            await _bot.SendDocumentAsync(long.Parse(Telegram.Attributes["telegram_id"].InnerXml), iof, "Logs", message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ошибка: " + e.Message);
                //await EmuExtensions.Delay(10).ContinueWith(task => Send(id, data));
            }
        }
        public static void DeleteMessage(long chatid, int messageid)
        {
            _bot.DeleteMessageAsync(chatid, messageid);
        }
        public static async void Send(long id, string data)
        {
            if (id == 0)
                return;

            if (data.Length < 4096)
            {
                try
                {
                    await _bot.SendTextMessageAsync(id, data);
                }
                catch
                {
                    await EmuExtensions.Delay(10).ContinueWith(task => Send(id, data));
                }
            }
        }

        public static void OnCommand(UserStatus status, long id, long tg_id, string text, int MessageID)
        {
            try
            {
                List<string> commands = text.Split('\n').ToList();
                foreach (var command in commands)
                {
                    List<string> cmd = command.Split(' ').ToList();
                    cmd.RemoveAll(x => x == " " || x == string.Empty);
                    if (cmd[0] == "/help")
                    {
                        string Cmds = "";
                        foreach (XmlElement Telegram in QueryCache.GetCache("BotsCmd").Data.ChildNodes)
                        {
                            if (Telegram.Attributes["telegram_id"].InnerText == tg_id.ToString())
                            {
                                foreach (XmlElement cmds in Telegram.ChildNodes)
                                {
                                    Cmds += cmds.Attributes["name"].InnerText + " " + cmds.Attributes["descriptions"].InnerText + "\n";
                                }
                            }
                        }
                        Send(id, Cmds);
                        return;
                    }
                    foreach (XmlElement Telegram in QueryCache.GetCache("BotsCmd").Data.ChildNodes)
                    {
                        if (Telegram.Attributes["telegram_id"].InnerText == tg_id.ToString())
                        {
                            foreach (XmlElement Cmds in Telegram.ChildNodes)
                            {
                                if (Cmds.Attributes["name"].InnerText == cmd[0])
                                {
                                    switch (Cmds.Attributes["function"].InnerXml)
                                    {
                                        case "OnlinePlayers":
                                            GetOnline(status, id);
                                            break;
                                        case "RoomsOnlinePlayers":
                                            GetRoomsOnline(id);
                                            break;
                                        case "GiveItemPermPlayer":
                                            if (cmd.Count != 3)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            GiveItem(status, id, cmd[1], cmd[2], ItemType.Basic);
                                            break;
                                        case "GiveItemTimePlayer":
                                            if (cmd.Count != 4)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }

                                            var seconds = EmuExtensions.GetTotalSeconds(cmd[3]);

                                            if (seconds == -1)
                                            {
                                                Send(id, "Неверно указано время. (1d, 1h, 1m)");
                                                return;
                                            }

                                            GiveItem(status, id, cmd[1], cmd[2], ItemType.Expiration, seconds);
                                            break;
                                        case "GiveItemAmountPlayer":
                                            if (cmd.Count != 4)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            int quantity;
                                            int.TryParse(cmd[3], out quantity);
                                            if (quantity == 0)
                                            {
                                                Send(id, "Неверно указано количество.");
                                                return;
                                            }
                                            GiveItem(status, id, cmd[1], cmd[2], ItemType.Consumable, quantity: int.Parse(cmd[3]));
                                            break;
                                        case "AddMoneyPlayer":
                                            if (cmd.Count != 4)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }

                                            int ammount;
                                            int.TryParse(cmd[3], out ammount);

                                            if (ammount == 0)
                                            {
                                                Send(id, "Неверно указано количество.");
                                                return;
                                            }

                                            GiveMoney(status, id, cmd[1], cmd[2], ammount);
                                            break;
                                        case "AddExperiencePlayer":
                                        case "SetExperiencePlayer":
                                            if (cmd.Count != 3)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            int count;
                                            int.TryParse(cmd[2], out count);
                                            if (count == 0)
                                            {
                                                Send(id, "Неверно указано количество.");
                                                return;
                                            }
                                            GiveExp(status, id, cmd[0] == "addexp", cmd[1], count);
                                            break;
                                        case "AddAchievementsPlayer":
                                            if (cmd.Count != 3)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }

                                            uint idd;
                                            uint.TryParse(cmd[2], out idd);

                                            if (id == 0)
                                            {
                                                Send(id, "Неверно указано достижение.");
                                                return;
                                            }

                                            GiveAchiev(status, id, cmd[1], idd);
                                            break;

                                        case "SetMissionPlayer":

                                            Client client4;
                                            client4 = Server.Clients.FirstOrDefault(x => x.Profile?.Nickname == cmd[1]);

                                            if (client4 == null)
                                            {
                                                Send(id, "Игрок не найден.");
                                                return;
                                            }

                                            var room3 = client4.Profile.RoomPlayer?.Room;

                                            if (room3 == null)
                                            {
                                                Send(id, "Игрок не в комнате.");
                                                return;
                                            }

                                            room3.SetMission(Mission.GetMission(RoomType.PvP_Rating, "cdb2fe18-c99e-56b0-9e7f-ffc1d7733f53"));

                                            break;


                                        case "SetRoomNamePlayer":

                                            Client client3;
                                            client3 = Server.Clients.FirstOrDefault(x => x.Profile?.Nickname == cmd[1]);

                                            if (client3 == null)
                                            {
                                                Send(id, "Игрок не найден.");
                                                return;
                                            }

                                            var room2 = client3.Profile.RoomPlayer?.Room;

                                            if (room2 == null)
                                            {
                                                Send(id, "Игрок не в комнате.");
                                                return;
                                            }

                                            room2.SetRoomName(cmd[2]);

                                            Send(id, "Название комнаты поменялось!");

                                            break;

                                        case "SetMasterPlayer":
                                            Client client2;

                                            client2 = Server.Clients.FirstOrDefault(x => x.ProfileId == ulong.Parse(cmd[1]));

                                            if (client2 == null)
                                            {
                                                Send(id, "Игрок не найден.");
                                                return;
                                            }

                                            var room = client2.Profile.RoomPlayer?.Room;
                                            if (room == null)
                                            {
                                                Send(id, "Игрок не в комнате.");
                                                return;
                                            }
                                            room.SetMaster(client2);

                                            Send(id, "Игрок сделан главой комнаты.");

                                            break;

                                        case "KickOnServerPlayer":
                                            if (cmd.Count != 3)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            //if (!EmuConfig.API.Rules[cmd[0]].Contains(cmd[2]))
                                            //{
                                            //    Send(id, "Причина не найдена.");
                                            //    return;
                                            //}
                                            Client client;
                                            lock (Server.Clients)
                                            {
                                                client = Server.Clients.FirstOrDefault(x => x.Profile?.Nickname == cmd[1]);
                                            }
                                            if (client == null)
                                            {
                                                Send(id, "Игрок не найден.");
                                                return;
                                            }
                                            client.Dispose();
                                            Send(id, "Игрок кикнут.");
                                            break;
                                        case "BannedPlayer":
                                            if (cmd.Count < 3)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            try
                                            {
                                                string Nickname = cmd[1];
                                                string Time = cmd[2];
                                                var seconds1 = EmuExtensions.GetTotalSeconds(Time);
                                                if (seconds1 != -1)
                                                    cmd.RemoveAt(2);
                                                cmd.RemoveAt(1);
                                                cmd.RemoveAt(0);
                                                Ban(Nickname, string.Join(' ', cmd), (seconds1 != -1) ? Time : "12600d");
                                            }
                                            catch (ServerException e)
                                            {
                                                Send(id, e.Message);
                                                return;
                                            }
                                            Send(id, "Игрок успешно забанен.");
                                            break;
                                        case "MutePlayer":
                                            if (cmd.Count < 3)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            try
                                            {
                                                string Nickname = cmd[1];
                                                string Time = cmd[2];
                                                var seconds1 = EmuExtensions.GetTotalSeconds(Time);
                                                if (seconds1 != -1)
                                                    cmd.RemoveAt(2);
                                                cmd.RemoveAt(1);
                                                cmd.RemoveAt(0);
                                                Mute(Nickname, string.Join(' ', cmd), (seconds1 != -1) ? Time : "12600d");
                                            }
                                            catch (ServerException e)
                                            {
                                                Send(id, e.Message);
                                                return;
                                            }

                                            Send(id, "Игрок успешно выдан мут.");
                                            break;
                                        case "ClearProfilePlayer":
                                            if (cmd.Count != 2)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            Profile profile = Profile.GetProfileForNickname(cmd[1]);
                                            if (profile == null)
                                            {
                                                Send(id, "Игрок не найден.");
                                                return;
                                            }
                                            lock (Server.Clients)
                                            {
                                                Server.Clients.Where(x => x.Profile?.Nickname == cmd[1]).ToList().ForEach(x => x.Dispose());
                                            }
                                            SQL.Query($"DELETE FROM emu_profiles WHERE profile_id={profile.Id}");
                                            Send(id, "Профиль успешно удален.");
                                            break;
                                        case "ClearNickPlayer":
                                            if (cmd.Count != 2)
                                            {
                                                Send(id, EmuConfig.API.Commands[cmd[0]]);
                                                return;
                                            }

                                            Profile profile1 = Profile.GetProfileForNickname(cmd[1]);
                                            if (profile1 == null)
                                            {
                                                Send(id, "Игрок не найден.");
                                                return;
                                            }

                                            profile1.Nickname = $"боец_{profile1.Id}" + profile1.Id;
                                            //profile.Update();
                                            SQL.Query($"UPDATE emu_profiles SET nickname='{profile1.Nickname}' WHERE profile_id={profile1.Id}");

                                            lock (Server.Clients)
                                            {
                                                Server.Clients.Where(x => x.Profile?.Nickname == profile1.Nickname).ToList().ForEach(x => x.Dispose());
                                            }
                                            //Server.Clients.Where(x => x.Profile?.Nickname == profile.Nickname).ToList().ForEach(x => x.Dispose());

                                            Send(id, "Никнейм успешно сброшен.");
                                            break;
                                        case "UnBannedPlayer":

                                            if (cmd.Count != 2)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            try
                                            {
                                                Unban(cmd[1]);
                                            }
                                            catch (ServerException e)
                                            {
                                                Send(id, e.Message);
                                                return;
                                            }
                                            Send(id, "Блокировка снята.");
                                            break;
                                        case "UnMutePlayer":
                                            if (cmd.Count != 2)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            try
                                            {
                                                Unmute(cmd[1]);
                                            }
                                            catch (ServerException e)
                                            {
                                                Send(id, e.Message);
                                                return;
                                            }
                                            break;
                                        case "GreenNotif":
                                            if (cmd.Count <= 1)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            cmd.RemoveAt(0);
                                            var message = string.Join(' ', cmd);

                                            lock (Server.Clients)
                                            {
                                                foreach (var target in Server.Clients)
                                                {
                                                    Notification.SyncNotifications(target, Notification.AnnouncementNotification(message));
                                                }
                                            }
                                            break;
                                        case "DeleteItemPlayer":
                                            if(cmd.Count <= 2)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            var db = SQL.QueryRead($"SELECT * FROM emu_items WHERE profile_id={GetProfileID(cmd[1])}");
                                            foreach (DataRow row in db.Rows)
                                            {
                                                if (cmd[2] == row["name"].ToString())
                                                {
                                                    SQL.Query($"DELETE FROM emu_items WHERE id='{row["id"]}' AND profile_id='{GetProfileID(cmd[1])}';");
                                                }
                                            }
                                            Send(id,"Предмет успешно удален");
                                            break;
                                        case "GetProfileAll":
                                            try
                                            {
                                                if (System.IO.File.Exists("result.zip"))
                                                    System.IO.File.Delete("result.zip");
                                                string guid = Guid.NewGuid().ToString().Replace("-", "");
                                                if (cmd.Count <= 1)
                                                {
                                                    Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                    return;
                                                }
                                                SelectUser(cmd[1], guid);
                                                Send(id, "Пожалуйста, подождите...");
                                                SelectClan(cmd[1], guid);
                                                SelectItemsProfile(cmd[1], guid);
                                                SelectAchievementsProfile(cmd[1], guid);
                                                ZipFile.CreateFromDirectory(string.Format("GetPlayers/profile_{0}_{1}/", cmd[1], guid), "result.zip");
                                                DeleteMessage(id, MessageID + 1);
                                                SendDocument(id, "result.zip");
                                            }
                                            catch
                                            {
                                                Send(id, "Профиль не найден");
                                            }
                                            break;
                                        case "GetLogs":
                                            ZipFile.CreateFromDirectory("Logs/chat", "result.zip");
                                            SendDocument(id, "result.zip");
                                            break;
                                        case "ChangeNickName":
                                            if (cmd.Count <= 2)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            SQL.Query($"UPDATE emu_profiles SET nickname='{cmd[2]}' where nickname='{cmd[1]}';");
                                            Send(id, "Никнейм успешно изменен");
                                            break;
                                        case "ExcuteСmd":
  
                                            if (cmd.Count <= 1)
                                            {
                                                Send(id, Cmds.Attributes["name"].InnerXml + " " + Cmds.Attributes["descriptions"].InnerXml);
                                                return;
                                            }
                                            Client client1;
                                            lock (Server.Clients)
                                            {
                                                client1 = Server.Clients.FirstOrDefault(x => x.Profile?.Nickname == cmd[1]);
                                            }
                                            if (client1 == null)
                                            {
                                                Send(tg_id, "Игрок не найден.");
                                                return;
                                            }
                                            ExecCommand.Command(client1, null, command.Replace(cmd[0], "").Replace(cmd[1], "").Replace("  ", ""));
                                            break;
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static void SelectItemsProfile(string nick, string guid)
        {
            string items = "";
            var db1 = SQL.QueryRead($"SELECT * FROM emu_items WHERE profile_id={GetProfileID(nick)}");
            foreach (DataRow row in db1.Rows)
            {
                XmlElement xmlElement = QueryCache.GetName(row["name"].ToString());
                if (xmlElement == null)
                    continue;
                string Format = "";
                if ((long)row["buy_time_utc"] > 0)
                    Format = String.Format("[{0}] {1} | {2} | На время: до ({3})", row["id"], QueryCache.GetTranslation(xmlElement), row["name"], UnixTimeStampToDateTime((long)row["buy_time_utc"]));
                else if((int)row["quantity"] > 0)
                    Format = String.Format("[{0}] {1} | {2} | Количество: ({3})", row["id"], QueryCache.GetTranslation(xmlElement), row["name"], (int)row["quantity"]);
                else
                    Format = String.Format("[{0}] {1} | {2} | (Навсегда)", row["id"], QueryCache.GetTranslation(xmlElement), row["name"]);
                items += Format + "\n";
            }
            Directory.CreateDirectory($"GetPlayers/profile_{nick}_" + guid);
            System.IO.File.WriteAllText(string.Format("GetPlayers/profile_{0}_{1}/items.txt", nick, guid), items);
        }
        public static void SelectAchievementsProfile(string nick, string guid)
        {
            string AchievementsP = "";
            var db1 = SQL.QueryRead($"SELECT * FROM emu_achievements WHERE profile_id='{GetProfileID(nick)}';");
            foreach(DataRow dataRow in db1.Rows)
            {
                if(dataRow["achievement_id"] != null)
                {
                    XmlElement xmlElement = Achievements.Achievement.Find(at => at.Attributes["id"] != null && at.Attributes["id"].InnerText  == dataRow["achievement_id"].ToString());
                    if (xmlElement != null)
                    {
                        AchievementsP += String.Format("[{0}] AchievementName: {1} | Progress: {2} of {3} | CompletionTime: {4}\n",
    xmlElement.Attributes["id"].InnerText,
    QueryCache.GetTranslation(QueryCache.text_weapons_list.Find(at => at.Attributes["key"].InnerText == xmlElement["UI"].Attributes["name"].InnerText.Replace("@", ""))),
    dataRow["progress"],
    xmlElement.Attributes["amount"].InnerText,
    (dataRow["completion_time"].ToString() != "0" ? UnixTimeStampToDateTime((long)dataRow["completion_time"]).ToString() : "Ещё не получен"));
                    }
                }
            }
            Directory.CreateDirectory($"GetPlayers/profile_{nick}_" + guid);
            System.IO.File.WriteAllText(string.Format("GetPlayers/profile_{0}_{1}/achievements.txt", nick, guid), AchievementsP);
        }
        public static void SelectUser(string nick, string guid)
        {
            var db1 = SQL.QueryRead($"SELECT * FROM emu_users WHERE user_id={GetUserID(nick)}");
            string User = String.Format("user_id: {0}\nvk_id: vk.com/id{1}\ntoken: {2}\ncry_token: {3}\nipaddress: {4}\nbalance_market: {5}\nvk_first_name: {6}\nvk_last_name: {7}\nvk_photo_200: {8}", 
                db1.Rows[0]["user_id"],
                db1.Rows[0]["vk_id"],
                db1.Rows[0]["token"],
                db1.Rows[0]["cry_token"],
                db1.Rows[0]["ipaddress"],
                db1.Rows[0]["balance"],
                db1.Rows[0]["first_name"],
                db1.Rows[0]["last_name"],
                db1.Rows[0]["photo_200"]);
            Directory.CreateDirectory($"GetPlayers/profile_{nick}_" + guid);
            System.IO.File.WriteAllText(string.Format("GetPlayers/profile_{0}_{1}/user.txt", nick, guid), User);
        }
        public static void SelectClan(string nick, string guid)
        {
            string ClanInfo = "";
            UInt64 ClanID = GetClanID(nick);
            if(ClanID != 0)
            {
                ClanInfo += GetClanInfo(nick);
                ClanInfo += "Members:\n";
                var db1 = SQL.QueryRead($"SELECT * FROM emu_clan_members WHERE clan_id='{(UInt64)GetClanID(nick)}';");
                foreach (DataRow dataRow in db1.Rows)
                {
                    switch ((int)dataRow["clan_role"])
                    {
                        case 1:
                            ClanInfo += string.Format("ProfileID: {0} | Nickname: {1} | ClanRole: Глава\n", (UInt64)dataRow["profile_id"], GetProfileNick((UInt64)dataRow["profile_id"]));
                            break;
                        case 2:
                            ClanInfo += string.Format("ProfileID: {0} | Nickname: {1} | ClanRole: Офицер\n", (UInt64)dataRow["profile_id"], GetProfileNick((UInt64)dataRow["profile_id"]));
                            break;
                        case 3:
                            ClanInfo += string.Format("ProfileID: {0} | Nickname: {1} | ClanRole: Рядовой\n", (UInt64)dataRow["profile_id"], GetProfileNick((UInt64)dataRow["profile_id"]));
                            break;
                    }
                }
                Directory.CreateDirectory($"GetPlayers/profile_{nick}_" + guid);
                System.IO.File.WriteAllText(string.Format("GetPlayers/profile_{0}_{1}/Clan.txt", nick, guid), ClanInfo);
            }
        }
        public static UInt64 GetClanID(string nick)
        {
            var db1 = SQL.QueryRead($"SELECT * FROM emu_clan_members WHERE profile_id={GetProfileID(nick)}");
            return (db1.Rows.Count != 0 ? (UInt64)db1.Rows[0]["clan_id"] : 0);
        }
        public static string GetClanInfo(string nick)
        {
            var db1 = SQL.QueryRead($"SELECT * FROM emu_clans WHERE clan_id='{GetClanID(nick)}';");
            return string.Format("ClanID: {0} | ClanName: {1} | Description: {2} | CreateClan: {3}\n", (UInt64)db1.Rows[0]["clan_id"], db1.Rows[0]["name"].ToString(), Base64Decode(db1.Rows[0]["description"].ToString()), UnixTimeStampToDateTime((UInt32)db1.Rows[0]["creation_date"]));
        }
        public static string GetProfileID(string nickname)
        {
            var ID = SQL.QueryRead($"SELECT * FROM emu_profiles WHERE nickname='{nickname}'");
            return ID.Rows[0]["profile_id"].ToString();
        }
        public static string GetUserID(string nickname)
        {
            var ID = SQL.QueryRead($"SELECT * FROM emu_profiles WHERE nickname='{nickname}'");
            return ID.Rows[0]["user_id"].ToString();
        }
        public static string GetProfileNick(UInt64 profile_id)
        {
            var ID = SQL.QueryRead($"SELECT * FROM emu_profiles WHERE profile_id='{profile_id}'");
            if(ID.Rows.Count != 0)
                return ID.Rows[0]["nickname"].ToString();
            return "без_имени_" + IDP++;
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        public static async void SendDocument(long id, string name)
        {
            try
            {
                using (var stream = System.IO.File.OpenRead(name))
                {
                    Telegram.Bot.Types.InputFiles.InputOnlineFile iof = new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream);
                    iof.FileName = "Profile.zip";
                    var send = await _bot.SendDocumentAsync(id, iof, "Логи");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("ошибка: " + e.Message);
                //await EmuExtensions.Delay(10).ContinueWith(task => Send(id, data));
            }
        }
        public static void GetOnline(UserStatus status, long tg_id)
        {
            List<string> strings = new List<string>();
            lock (Server.Clients)
            {
                foreach (Client client in Server.Clients)
                {
                    if (client.Profile != null)
                    {
                        strings.Add(String.Format("Nickname: {0} | VK: vk.com/id{1} | IP: {2} | {3}", client.Profile.Nickname, VKID(client.UserId), client.IPAddress, GetStatus(client)));
                    }
                }
                Send(tg_id, $"Онлайн: {Server.Clients.Where(x => !x.IsDedicated).Count()}\n{string.Join("\n", strings)}");
            }
        }
        public static string GetStatus(Client client)
        {
            try
            {
                int i = 1;
                if (client.Presence.ToString().Split(' ').Count() > 2)
                    i = 2;
                switch (client.Presence.ToString().Split(' ')[i].Replace(",", ""))
                {
                    case "Away":
                        return "Status: Афк";
                    case "InLobby":
                        return "Status: В лобби";
                    case "InGameRoom":
                        return "Status: В комнате | Название: " + QueryCache.GetTranslation(QueryCache.text_weapons_list.Find(at => at.Attributes["key"].InnerText == client.Profile.Room.GetExtension<GameRoomMission>().Name.Replace("@", "")));
                    case "InGame":
                        return "Status: В бою | Название: " + QueryCache.GetTranslation(QueryCache.text_weapons_list.Find(at => at.Attributes["key"].InnerText == client.Profile.Room.GetExtension<GameRoomMission>().Name.ToString().Replace("@", ""))); ;
                    case "InShop":
                        return "Status: В магазине";
                    case "InCustomize":
                        return "Status: На складе";
                    case "InRatingGame":
                        return "Status: На РМ";
                    case "InTutorialGame":
                        return "Status: В обучении";
                }
            }
            catch { }       
            return "";
        }
        public static void GetRoomsOnline(long tg_id)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var channel in Server.Channels)
            {
                sb.AppendLine(channel.Resource);

                lock (channel.Rooms)
                {
                    foreach (var room in channel.Rooms)
                    {
                        var rCore = room.GetExtension<GameRoomCore>();
                        var rCustomParams = room.GetExtension<GameRoomCustomParams>();

                        sb.AppendLine(string.Format(@"  ""{0}"" {1}/{2}", rCore.Name, rCore.Players.Count, rCustomParams.GetCurrentRestriction("max_players")));
                    }
                }
            }
            Send(tg_id, sb.ToString());
        }
        public static string Request(string query)
        {
            WebRequest request = WebRequest.Create(query);
            WebResponse response = request.GetResponse();
            string answer;
            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    answer = reader.ReadToEnd();
                    return answer;
                }
            }
        }
        public static void Unmute(string nickname)
        {
            Profile profile = Profile.GetProfileForNickname(nickname);
            if (profile == null)
            {
                throw new ServerException("Игрок не найден.");
            }

            ulong user_id = Profile.GetUserId(profile.Id);

            SQL.QueryRead($"DELETE FROM emu_mutes WHERE user_id={user_id}");
        }
        public static void Unban(string nickname)
        {
            Profile profile = Profile.GetProfileForNickname(nickname);
            if (profile == null)
            {
                throw new ServerException("Игрок не найден.");
            }

            ulong user_id = Profile.GetUserId(profile.Id);

            SQL.QueryRead($"DELETE FROM emu_bans WHERE user_id={user_id}");
        }
        public static void Ban(string nickname, string rule, string time = "0s")
        {
            var seconds = EmuExtensions.GetTotalSeconds(time);
            long unban_time = 0;

            if (seconds == -1)
            {
                throw new ServerException("Неверно указано время. (1d, 1h, 1m)");
            }
            if (seconds != 0)
            {
                unban_time = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + seconds;
            }

            Profile profile = Profile.GetProfileForNickname(nickname);
            if (profile == null)
            {
                throw new ServerException("Игрок не найден.");
            }

            ulong user_id = Profile.GetUserId(profile.Id);

            var db = SQL.QueryRead($"SELECT * FROM emu_bans WHERE user_id={user_id}");
            if (db.Rows.Count != 0)
            {
                long ban_time = (long)db.Rows[0]["unban_time"];
                if (ban_time > DateTimeOffset.UtcNow.ToUnixTimeSeconds() || ban_time == 0)
                {
                    throw new ServerException("Игрок уже имеет блокировку");
                }
                else
                {
                    //SQL.QueryRead($"DELETE FROM emu_bans WHERE user_id={user_id}");
                }
            }

            try
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO emu_bans (`user_id`, `rule`, `unban_time`) VALUES (@user_id, @rule, @unban_time);");
                cmd.Parameters.AddWithValue("@user_id", user_id);
                cmd.Parameters.AddWithValue("@rule", rule);
                cmd.Parameters.AddWithValue("@unban_time", unban_time);
                SQL.QueryRead(cmd);

            }
            catch (Exception e)
            {
                string exception = e.ToString();

                if (exception.Contains("Duplicate"))
                {
                    throw new ServerException("Игрок уже имеет игровую блокировку. (джага джага)");
                }
                else
                {
                    throw new ServerException("Не удалось заблокировать игрока.");
                }
            }

            lock (Server.Clients)
            {
                Server.Clients.FirstOrDefault(x => x.Profile?.Nickname == nickname)?.Dispose();
            }
            //Server.Clients.FirstOrDefault(x => x.Profile?.Nickname == nickname)?.Dispose();
        }
        public static void Mute(string nickname, string rule, string time = "3650d")
        {
            var seconds = EmuExtensions.GetTotalSeconds(time);
            long unmute_time = 0;

            if (seconds == -1)
            {
                throw new ServerException("Неверно указано время. (1d, 1h, 1m)");
            }
            if (seconds != 0)
            {
                unmute_time = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + seconds;
            }

            Profile profile = Profile.GetProfileForNickname(nickname);
            if (profile == null)
            {
                throw new ServerException("Игрок не найден.");
            }

            ulong user_id = Profile.GetUserId(profile.Id);

            var db = SQL.QueryRead($"SELECT * FROM emu_mutes WHERE user_id={user_id}");
            if(db.Rows.Count != 0)
            {
                long mute_time = (long)db.Rows[0]["unmute_time"];
                if (mute_time > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    throw new ServerException("Игрок уже имеет мут. (джага джага)");
                }
                else
                {
                    SQL.QueryRead($"DELETE FROM emu_mutes WHERE user_id={user_id}");
                }
            }

            try
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO emu_mutes (`user_id`, `rule`, `unmute_time`) VALUES (@user_id, @rule, @unmute_time);");
                cmd.Parameters.AddWithValue("@user_id", user_id);
                cmd.Parameters.AddWithValue("@rule", rule);
                cmd.Parameters.AddWithValue("@unmute_time", unmute_time);
                SQL.QueryRead(cmd);

            }
            catch (Exception e)
            {
                string exception = e.ToString();

                if (exception.Contains("Duplicate"))
                {
                    throw new ServerException("Игрок уже имеет мут. (джага джага)");
                }
                else
                {
                    throw new ServerException("Не удалось выдать мут игроку.");
                }
            }

            //кик
            Client client = null;
            lock (Server.Clients)
            {
                client = Server.Clients.FirstOrDefault(x => x.Profile?.Nickname == nickname);
            }

            if (client != null)
            {
                var notif = Notification.MessageNotification("Вы лишены возможности использовать чат до " + DateTimeOffset.FromUnixTimeSeconds(unmute_time‬).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                Notification.SyncNotifications(client, notif);
            }
        }
        public static void ActivatePin(string nickname, string pin)
        {
            Profile profile = Profile.GetProfileForNickname(nickname);
            if (profile == null)
                return;

            MySqlCommand cmd = new MySqlCommand("SELECT * FROM emu_pin_codes WHERE pin=@pin");
            cmd.Parameters.AddWithValue("@pin", pin);

            var db = SQL.QueryRead(cmd);

            if (db.Rows.Count == 0)
                return;

            var reward = (string)db.Rows[0]["reward"];

            foreach (var command in reward.Split("<SPLIT>"))
            {
                //OnCommand(UserStatus.Administrator, 0, 0, string.Format(command, nickname));
            }
        }
        public static void GiveMoney(UserStatus status, long tg_id,  string nickname, string currency, int ammount)
        {
            Profile profile = Profile.GetProfileForNickname(nickname);
            if (profile == null)
            {
                Send(tg_id, "Игрок не найден.");
                return;
            }


            switch (currency)
            {
                case "game":
                    profile.GameMoney += ammount;
                    break;
                case "crown":
                    profile.CrownMoney += ammount;
                    break;
                case "cry":
                    profile.CryMoney += ammount;
                    break;
                default:
                    Send(tg_id, "Тип валюты указан неверно.");
                    return;
            }

            profile.Update();

            var notif = Notification.GiveMoneyNotification(currency + "_money", ammount, true);

            Notification.AddNotification(profile.Id, notif);

            lock (Server.Clients)
            {
                Notification.SyncNotifications(Server.Clients.FirstOrDefault(x => x.ProfileId == profile.Id), notif);
            }
            //Notification.SyncNotifications(Server.Clients.FirstOrDefault(x => x.ProfileId == profile.Id), notif);

            Send(tg_id, "Бабки на месте блять.");
        }
        public static void GiveAchiev(UserStatus status, long tg_id, string nickname, uint achiev_id)
        {
            Profile profile = Profile.GetProfileForNickname(nickname);
            if (profile == null)
            {
                Send(tg_id, "Игрок не найден.");
                return;
            }


            var achiev = Achievement.SetAchiev(profile.Id, achiev_id, int.MaxValue, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            Notification.SyncNotifications(profile.Id, Notification.AchievementNotification(achiev.AchievementId, achiev.Progress, achiev.CompletionTimeUnixTimestamp));

            //Server.Clients.FirstOrDefault(x => x.Profile?.Nickname == nickname)?.ResyncProfie();

            Send(tg_id, "Нашивка успешно выдана!");
        }
        public static void GiveExp(UserStatus status, long tg_id,  bool isGive, string nickname, int exp)
        {
            Profile profile = Profile.GetProfileForNickname(nickname);
            if (profile == null)
            {
                Send(tg_id, "Игрок не найден.");
                return;
            }

            if (isGive)
            {
                profile.Experience += exp;
            }
            else
            {
                profile.Experience = exp;
            }

            profile.CheckRankUpdated();
            profile.Update();

            lock (Server.Clients)
            {
                Server.Clients.FirstOrDefault(x => x.Profile?.Nickname == nickname)?.ResyncProfie();
            }
           // Server.Clients.FirstOrDefault(x => x.Profile?.Nickname == nickname)?.ResyncProfie();

            Send(tg_id, "Ранг успешно выдан!");
        }
        public static void GiveBox(UserStatus status, long tg_id,string nickname, string box_name)
        {
            Profile profile = Profile.GetProfileForNickname(nickname);
            if (profile == null)
            {
                Send(tg_id, "Игрок не найден.");
                return;
            }

            if (!ValidateItem(box_name))
            {
                Send(tg_id, "Предмет не найден.");
                return;
            }

            //profile.GiveRandomBoxCards();
            var notif = profile.GiveRandomBox(box_name);

            if(notif == null)
            {
                Send(tg_id, "Не удалось выдать коробку.");
                return;
            }

            //Notification.AddNotification(profile_id, notif);
            //Notification.SyncNotifications(profile_id, notif);

            Send(tg_id, "Предмет успешно выдан.");
        }
        public static void GiveItem(UserStatus status, long tg_id, string nickname, string item_name, ItemType type = ItemType.Permanent, long seconds = 0, int quantity = 0)
        {
            Profile profile = Profile.GetProfileForNickname(nickname);
            if (profile == null)
            {
                Send(tg_id, $"{nickname} Игрок не найден.");
                return;
            }
            if (!ValidateItem(item_name))
            {
                Send(tg_id, "Предмет не найден.");
                return;
            }

            var item = profile.GiveItem(item_name, type, seconds, quantity);

            if(item != null)
            {
                var notif = Notification.GiveItemNotification(item_name, type.ToString(), true, seconds, quantity);

                Notification.AddNotification(profile.Id, notif);

                lock (Server.Clients)
                {
                    Notification.SyncNotifications(Server.Clients.FirstOrDefault(x => x.ProfileId == profile.Id), notif);
                }
                //Notification.SyncNotifications(Server.Clients.FirstOrDefault(x => x.ProfileId == profile.Id), notif);

                Send(tg_id, "Предмет успешно выдан.");
            }
        }
        public static bool ValidateItem(string item_name)
        {
            var items = QueryCache.GetCache("items");

            foreach (XmlElement item in items.Data.ChildNodes)
            {
                //TODO max_buy_amount только 1, надо проверить
                if (item.GetAttribute("name") == item_name)
                    return true;
            }

            return false;
        }
        public static int VKID(ulong user_id)
        {
            return (int)SQL.QueryRead($"SELECT * FROM emu_users WHERE user_id={user_id}").Rows[0]["vk_id"];
        }
        public static ulong UserID(ulong profile_id)
        {
            return (ulong)SQL.QueryRead($"SELECT * FROM emu_profiles WHERE profile_id={profile_id}").Rows[0]["user_id"];
        }
        public static DataTable UserGet(ulong profileid)
        {
            var db = SQL.QueryRead($"SELECT * FROM emu_users WHERE user_id={Profile.GetUserId(profileid)}");
            if (db.Rows.Count == 0)
                return null;
            return db;
        }
    }
}
