using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVServer;
using AltVServer.TPlayer;

namespace AltVTutorial
{
    class Server : Resource
    {
        public override void OnStart()
        {
            Alt.Log("Server wurde gestartet!");
            Utils.adminLog("Server wurde gestartet", "Server");
            //MYSQL
            Datenbank.InitConnection();
            //TIMER
            Timer paydayTimer = new Timer(OnPaydayTimer,null,10000,10000);
        }

        public static void OnPaydayTimer(object state)
        {
            foreach (TPlayer tplayer in Alt.GetAllPlayers())
            {
                tplayer.PayDay--;
                if(tplayer.PayDay <= 0)
                {
                    tplayer.Geld += 500;
                    Utils.SendNotification(tplayer, "info", "Du hast einen PayDay in Höhe von $500 erhaltem");
                    tplayer.PayDay = 1;
                }
            }
        }

        public override void OnStop()
        {
            Alt.Log("Server wurde beendet!");
        }

        public override IEntityFactory<IPlayer> GetPlayerFactory()
        {
            return new TPlayerFactory();
        }
    }
}
