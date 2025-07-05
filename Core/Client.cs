using EmuWarface.Game;
using EmuWarface.Game.Clans;
using EmuWarface.Game.Enums;
using EmuWarface.Game.GameRooms;
using EmuWarface.Xmpp;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EmuWarface.Core
{
    public class Client : IDisposable
    {
        private const uint MAGIC = 0xFEEDDEAD;

        private bool disposed;

        private Socket _socket;
        private Stream _stream;

        private ConcurrentQueue<byte[]> _writePendingData = new ConcurrentQueue<byte[]>();
        private bool _sendingData = false;

        public ConnectionState State { get; set; }
        public PlayerStatus Presence { get; set; }
        public MasterServer Channel { get; set; }
        public ulong UserId { get; private set; }
        public Jid Jid { get; private set; }
        public string IPAddress { get; private set; }
        public bool IsDedicated { get; private set; }
        public DedicatedServer Dedicated { get; set; }

        public bool IsConnected
        {
            /*get
            {
                try
                {
                    return !(_socket.Poll(1, SelectMode.SelectRead) && _socket.Available == 0);
                }
                catch (SocketException) { return false; }
            }*/
            get => _socket.Connected;
        }
        private Profile _profile;
        public Profile Profile
        {
            get
            {
                if (IsDedicated)
                    return null;

                if (UserId == 0)
                    return null;

                if (_profile == null)
                    _profile = Profile.GetProfileWithUserId(UserId);

                return _profile;
            }
        }

        public ulong ProfileId
        {
            get
            {
                if (_profile != null)
                    return _profile.Id;

                if (UserId == 0)
                    return 0;

                return Profile.GetProfileId(UserId);
            }
        }

        public Client(Socket socket)
        {
            _socket = socket;
            _stream = new NetworkStream(socket);

            IPAddress = ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();
        }

        public string Read()
        {
            if (disposed)
                throw new ServerException("Read disposed object");

            byte[] buffer = new byte[12];

            int received = 0;
            while (received != 12)
            {
                received += _stream.Read(buffer, received, 12 - received);

                if (received == 0)
                    return null;
            }

            if (buffer[0] == 60)
                return null;

            var magicBuff = new byte[4];
            var lengthBuff = new byte[4];

            Array.Copy(buffer, 0, magicBuff, 0, 4);
            Array.Copy(buffer, 4, lengthBuff, 0, 4);

            var magic = BitConverter.ToUInt32(magicBuff);
            int length = BitConverter.ToInt32(lengthBuff);

            if (magic != MAGIC)
            {
                if (buffer[0] == 60)
                    throw new ServerException("No support without 'online_use_protect'");

                return null;
            }

            buffer = new byte[length];
            received = 0;

            while (received != length)
            {
                received += _stream.Read(buffer, received, length - received);

                if (received == 0)
                    return null;
            }

            return Encoding.UTF8.GetString(buffer);
        }

        public void StreamFeatures()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<stream:features>");

            if (State == ConnectionState.Connected)
            {
                if (_stream is NetworkStream)
                    sb.Append("<starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>");

                sb.Append("<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'><mechanism>WARFACE</mechanism></mechanisms>");
            }
            else
            {
                sb.Append("<bind xmlns='urn:ietf:params:xml:ns:xmpp-bind' /><session xmlns='urn:ietf:params:xml:ns:xmpp-session' />");
            }
            sb.Append("</stream:features>");

            Send(sb.ToString());
        }

        public void StartTls()
        {
            Send(Xml.Element("proceed", "urn:ietf:params:xml:ns:xmpp-tls"));

            SslStream sslStream = new SslStream(_stream, false, (sender, cert, chain, err) => true);
            sslStream.AuthenticateAsServer(Server.Certificate, true, SslProtocols.Tls, false);

            _stream = sslStream;
        }

        public bool Authenticate(XmlElement e)
        {
            string[] data = EmuExtensions.Base64Decode(e.InnerText).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);

            if (data.Length != 2)
                return false;

            if (data[1].Contains("dedicated"))
            {
                if (!EmuConfig.Settings.DedicatedHosts.Contains(IPAddress))
                    return false;

                IsDedicated = true;
            }
            else
            {
                var uid     = ulong.Parse(data[1]);
                var token = data[0].Replace("{:B:}russia", "").Replace("{:B:}bo_mycom", "");

                if (string.IsNullOrEmpty(token))
                {
                    Send(Xml.Element("failure", "urn:ietf:params:xml:ns:xmpp-sasl"));
                    return false;
                }

                //<failure xmlns='urn:ietf:params:xml:ns:xmpp-sasl'><warface-failure>&lt;error&gt;&lt;code&gt;"+accountData.error.code+"&lt;/code&gt;&lt;message&gt;"+accountData.error.message+"&lt;/message&gt;&lt;unbantime&gt;"+accountData.error.unbantime+"&lt;/unbantime&gt;&lt;/error&gt;</warface-failure></failure>

                string ban_reason;
                var unban_time = Profile.GetBanTime(uid, out ban_reason);

                if(unban_time != -1)
                {
                    Send("<failure xmlns='urn:ietf:params:xml:ns:xmpp-sasl'><warface-failure>&lt;error&gt;&lt;code&gt;" + 1 + "&lt;/code&gt;&lt;message&gt;" + ban_reason + "&lt;/message&gt;&lt;unbantime&gt;" + (unban_time != 0 ? unban_time.ToString() : "1832014800") + "&lt;/unbantime&gt;&lt;/error&gt;</warface-failure></failure>");
                    return false;
                }

#if !DEBUGLOCAL
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM emu_users WHERE user_id=@user_id AND cry_token=@cry_token");

                cmd.Parameters.AddWithValue("@user_id", uid);
                cmd.Parameters.AddWithValue("@cry_token", token);

                var db = SQL.QueryRead(cmd);

                if (db.Rows.Count == 0)
                {
                    Send(Xml.Element("failure", "urn:ietf:params:xml:ns:xmpp-sasl"));
                    return false;
                }
#endif

                UserId = uid;

                lock (Server.Clients)
                {
                    Server.Clients.Where(x => x.UserId == uid).ToList().ForEach(x => x.Dispose());
                }
                //Server.Clients.Where(x => x.UserId == uid).ToList().ForEach(x => x.Dispose());
#if RELEASE
               // SQL.Query($"UPDATE emu_users SET cry_token='{Guid.NewGuid()}' WHERE user_id='{uid}'");
#endif
                SQL.Query($"INSERT IGNORE INTO emu_connects (user_id, ipaddress) VALUES('{uid}', '{IPAddress}')");
            }

            Send(Xml.Element("success", "urn:ietf:params:xml:ns:xmpp-sasl"));

            State = ConnectionState.Authed;
            return true;
        }

        public void Bind(Iq iq)
        {
            if (IsDedicated)
            {
                Dedicated = new DedicatedServer(this);

                Server.DedicatedSeed++;
                Jid = new Jid("warface", "dedicated", "DEDICATED_" + Server.DedicatedSeed);
            }
            else
            {
                Jid = new Jid("warface", UserId.ToString(), "GameClient");
            }

            var jid = Xml.Element("jid");
            jid.InnerText = Jid.ToString();

            iq.Element["bind"].RemoveAll();
            iq.Element["bind"].Child(jid);

            IqResult(iq);

            State = ConnectionState.Binded;

            lock (Server.Clients)
            {
                Server.Clients.Add(this);
            }
            //Server.Clients.Add(this);

            //EmuExtensions.UpdateOnline();
        }

        public void ResyncProfie()
        {
            //Iq iq = new Iq(IqType.Get, Jid, "k01.warface");
            //QueryGet(iq.SetQuery(Profile.ResyncProfie()));
            QueryGet(Profile.ResyncProfie());
        }

        public void PeerStatusUpdateBroadcast(XmlElement peer_status_update = null)
        {
            if (peer_status_update == null)
                peer_status_update = Xml.Element("peer_status_update")
                        .Attr("nickname",           Profile.Nickname)
                        .Attr("profile_id",         Profile.Id)
                        .Attr("status",             (int)PlayerStatus.Logout)
                        .Attr("experience",         Profile.Experience)
                        .Attr("place_token",        "")
                        .Attr("place_info_token",   "")
                        .Attr("mode_info_token",    "")
                        .Attr("mission_info_token", "");

            var friends = Friend.GetFriends(ProfileId);

            lock (Server.Clients)
            {
                foreach (var friend_id in friends)
                {
                    var target = Server.Clients.FirstOrDefault(x => x.ProfileId == friend_id);
                    if (target != null)
                    {
                        target.QueryGet(peer_status_update, Jid);
                    }
                }
            }
        }

        //<player profile_id='25460487' team_id='1' status='1' observer='0' skill='0.000' nickname='0633558123' clanName='' class_id='0' online_id='736990918@warface/GameClient' group_id='f050601d-535e-4637-9b0d-758fb2988746'
        //presence='17' experience='10242' rank='7' banner_badge='4294967295' banner_mark='4294967295' banner_stripe='4294967295' region_id='global' />
        public XmlElement RoomSerialize()
        {
            return Xml.Element("player")
                .Attr("profile_id",     Profile.Id)
                .Attr("team_id",        (int)Profile.RoomPlayer.TeamId)
                .Attr("status",         (int)Profile.RoomPlayer.Status)
                .Attr("observer",       Profile.RoomPlayer.Observer ? 1 : 0)
                .Attr("skill",          Profile.RoomPlayer.Skill.ToString("F3").Replace(',', '.'))
                .Attr("nickname",       Profile.Nickname)
                .Attr("clanName",       /*TODO get name*/Game.Clans.Clan.GetClanName(Profile.ClanId))
                .Attr("class_id",       (int)Profile.CurrentClass)
                .Attr("online_id",      Jid.ToString())
                .Attr("group_id",       Profile.RoomPlayer.GroupId)
                .Attr("presence",       (int)Presence)
                .Attr("experience",     Profile.Experience)
                .Attr("rank",           Profile.GetRank())
                .Attr("banner_badge",   Profile.BannerBadge)
                .Attr("banner_mark",    Profile.BannerMark)
                .Attr("banner_stripe",  Profile.BannerStripe)
                .Attr("region_id",      Profile.RoomPlayer.RegionId);
        }

        public void QueryResult(Iq iq)
        {
            if (iq.Element["query"] != null)
            {
                if (iq.Query != null && iq.Query.OuterXml.Length > 800)
                    iq.Compress();
            }
            else
            {
                iq.Element.Child(Xml.Element("query", "urn:cryonline:k01"));
            }

            if (string.IsNullOrEmpty(iq.Id))
                iq.Id = Iq.GenerateId();

            IqResult(iq);
        }

        /*public void QueryGet(Iq iq)
        {
            if (iq.Element["query"] != null)
            {
                if (iq.Query.OuterXml.Length > 800)
                    iq.Compress();
            }

            iq.Id = Iq.GenerateId();

            Send(iq); 
        }*/

        public void QueryGet(XmlElement query, Jid from = null)
        {
            Iq iq = new Iq(IqType.Get, Jid, from != null ? from : "k01.warface");
            iq.SetQuery(query);

            if (iq.Query.OuterXml.Length > 800)
                iq.Compress();

            iq.Id = Iq.GenerateId();

            Send(iq);
        }

        public void IqResult(Iq iq)
        {
            if (iq == null)
                throw new ArgumentNullException("iq");

            iq.SwapDirection();
            iq.Type = IqType.Result;

            Send(iq);
        }

        public void Send(Iq iq)                 => Send(iq.ToString());
        public void Send(XmlElement message)    => Send(message.OuterXml);
        public void Send(string message) 
        {
            if (disposed)
                return;

            Send(Encoding.UTF8.GetBytes(message));

            if (EmuConfig.Settings.XmppDebug)
                Log.Xmpp(message);
        }

        public async void Send(byte[] data)
        {
            if (disposed)
                return;

            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write(MAGIC);
            binaryWriter.Write(data.LongLength);
            binaryWriter.Write(data);

            EnqueueDataForWrite(memoryStream.ToArray());
            //await _stream.WriteAsync(memoryStream.ToArray());
        }

        private void EnqueueDataForWrite(byte[] buffer)
        {
            if (disposed)
                return;

            if (buffer == null)
                return;

            _writePendingData.Enqueue(buffer);

            lock (_writePendingData)
            {
                if (_sendingData)
                {
                    return;
                }
                else
                {
                    _sendingData = true;
                }
            }

            Write();
        }

        private void Write()
        {
            byte[] buffer = null;
            try
            {
                if (_writePendingData.Count > 0 && _writePendingData.TryDequeue(out buffer))
                {
                    _stream.BeginWrite(buffer, 0, buffer.Length, WriteCallback, _stream);
                    //_stream.Write(buffer, 0, buffer.Length);

                    Write();
                }
                else
                {
                    lock (_writePendingData)
                    {
                        _sendingData = false;
                    }

                    //return true;
                }
            }
            catch (Exception ex)
            {
                // handle exception then
                lock (_writePendingData)
                {
                    _sendingData = false;
                }

                Dispose();
            }

            //return false;
        }

        private void WriteCallback(IAsyncResult ar)
        {
            Stream stream = (Stream)ar.AsyncState;
            try
            {
                stream.EndWrite(ar);
            }
            catch (Exception ex)
            {
                //Log.Warn("Stream Write Exception 2");

                Dispose();
                return;
                // handle exception                
            }

            Write();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            try
            {
                Presence = PlayerStatus.Logout;

                if (Profile != null)
                {
                    Profile.Room?.LeftPlayer(this);
                    Clan.ClanMembersUpdated(Profile.ClanId, ProfileId);

                    Profile.Update();
                    PeerStatusUpdateBroadcast();
                }

                if (IsDedicated)
                {
                    Dedicated?.Room?.AbortSession();
                    Dedicated = null;
                }
            }
            catch(Exception e)
            {
                Log.Error("Dispose client error: " + e.ToString());
                API.SendAdmins("Dispose client error: " + e.ToString());
            }
            finally
            {
                _socket?.Dispose();
                _stream?.Dispose();

                lock (Server.Clients)
                {
                    Server.Clients.RemoveAll(x => x.Presence == PlayerStatus.Logout);
                }
                //Server.Clients.RemoveAll(x => x.Presence == PlayerStatus.Logout);

                EmuExtensions.UpdateOnline();

                Log.Info(IPAddress + " disconnected");
            }
        }
    }
}
