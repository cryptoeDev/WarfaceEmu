using EmuWarface.Game.Items;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using Telegram.Bot.Types;

namespace EmuWarface
{
    internal class RconListener
    {
        internal Socket Socket;

        public static string Base64Decode(string text)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(text);
            string decodedText = Encoding.UTF8.GetString(base64EncodedBytes);
            return decodedText;
        }

        internal async void ClientConnected(object player)
        {
            RconListener User = (RconListener)player;
            Socket sUser = User.Socket;

            while (sUser.Connected)
            {
                try
                {
                    byte[] data = new byte[4096];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = sUser.Receive(data);
                        builder.Append(Encoding.ASCII.GetString(data, 0, bytes));
                    }
                    while (sUser.Available > 0);
                    XElement packet = XElement.Parse(builder.ToString());

                    

                    switch (packet.Name.LocalName)
                    {
                        case "item":

                            string type = packet.Attribute("type").Value;

                            string nickname = Base64Decode(packet.Attribute("nickname").Value);

                            string id = packet.Attribute("id").Value;

                            switch (type)
                            {
                                case "permanent":

                                   Console.WriteLine("Никнейм: "+nickname);

                                    API.GiveItem(UserStatus.Administrator, 6530883040, nickname, id, ItemType.Basic);

                                    break;
                                case "temp":

                                    var date = packet.Attribute("date").Value;

                                    //API.GiveItem(UserStatus.Administrator, 6530883040, nickname, id, ItemType.Expiration);

                                    API.GiveItem(UserStatus.Administrator, 6530883040, nickname, id, ItemType.Expiration, EmuExtensions.GetTotalSeconds(date));

                                    break;
                            }

                            break;
                    }

                }
                catch
                {

                }
            }
        }
    }
}
