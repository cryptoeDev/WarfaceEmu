using EmuWarface.Core;
using EmuWarface.Game;
using EmuWarface.Game.GameRooms;
using EmuWarface.Game.Items;
using EmuWarface.Game.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace EmuWarface
{
    public static class Rcon
    {
        private static Socket _socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        public static void Init()
        {
            int port = 6001;

            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(10);
            _socket.BeginAccept(OnAccept, null);

            Log.Info($"[Rcon] Started on {port} port");
            _socket.BeginAccept(OnAccept, null);
        }

        private static void OnAccept(IAsyncResult Result)
        {
            Socket sClient = _socket.EndAccept(Result);

            RconListener User = new RconListener()
            {
                Socket = sClient
            };

            RconListener Client = new RconListener();
            new Thread(Client.ClientConnected).Start(User);
            _socket.BeginAccept(OnAccept, null);


        }

        public static void Send(Socket s, string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);
            s.Send(msg);
        }
        public static void Request(string query)
        {
            WebRequest request = WebRequest.Create(query);
            request.GetResponse();
        }
    }
}
