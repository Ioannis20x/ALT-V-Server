using AltV.Net;
using K4os.Compression.LZ4.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltVServer
{
    class Utils
    {
        public static void adminLog(string text, string username)
        {
            HTTP.Post("https://discord.com/api/webhooks/1155142361026932839/bj1Yl48lVxtlPtlnJq8FT1byCybrFbzrzNFNC3jR0O0pRgAgZtyDCJJGfl-zS3HtM8F1", new System.Collections.Specialized.NameValueCollection()
            {
                {
                    "username",
                    username
                },
                {
                    "content",
                    text
                }
            });
        }

        public static void SendNotification(TPlayer.TPlayer tplayer, string status, string text)
        {
            tplayer.Emit("sendNotification",status, text);
            return;
        }

        public static TPlayer.TPlayer GetPlayerByName(string name)
        {
            foreach(TPlayer.TPlayer p in Alt.GetAllPlayers())
            {
                if(p.Name.ToLower() == name.ToLower())
                {
                    return p;
                }
            }
            return null;
        }
    }
}
