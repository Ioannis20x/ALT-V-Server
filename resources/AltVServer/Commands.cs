using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AltVServer
{
    public class Commands : IScript
    {

        [CommandEvent(CommandEventType.CommandNotFound)]
        public void OnCommandNotFound(TPlayer.TPlayer tplayer, string command)
        {
            tplayer.SendChatMessage("{FF0000}Befehl " + command + " nicht gefunden!");
        }

        [Command("veh")]
        public void CMD_veh(TPlayer.TPlayer tplayer, string VehicleName, int R = 0, int G = 0, int B = 0)
        {
            if (!tplayer.IsPlayerAdmin((int)TPlayer.TPlayer.Adminranks.Moderator))
            {
                Utils.SendNotification(tplayer,"error", "Dein Adminrang ist zu niedrig");
                return;
            }
            IVehicle veh = Alt.CreateVehicle(Alt.Hash(VehicleName), new AltV.Net.Data.Position(tplayer.Position.X, tplayer.Position.Y+1, tplayer.Position.Z),tplayer.Rotation);
            if(veh != null)
            {
                veh.PrimaryColorRgb = new AltV.Net.Data.Rgba((byte)R, (byte)G, (byte)B, 255);
                Utils.SendNotification(tplayer, "warning", $"Fahrzeug {VehicleName} gespawned!");
                Utils.adminLog($"{tplayer.Name} hat eine/n {VehicleName} gespawned", "SERVER");
            }
            else
            {
                tplayer.SendChatMessage("{FF0000} Das Fahrzeug konnte nicht gespawned werden!");
                Utils.SendNotification(tplayer, "error", "Das Fahrzeug konnte nicht gespawned werden!");
            }
        }


        [Command("freezeme")]
        public void CMD_freezeme(TPlayer.TPlayer tplayer, bool freeze)
        {
            if (!tplayer.IsPlayerAdmin((int)TPlayer.TPlayer.Adminranks.Moderator)){
                tplayer.SendChatMessage("{FF0000} Dein Adminrang ist zu niedrig");
                return;
            }
            tplayer.Emit("freezePlayer", freeze);
            tplayer.SendChatMessage("{04B404}Der freeze Befehl wurde ausgeführt!");
        }

        [Command("gotoxyz")]
        public void CMD_gotoxyz(TPlayer.TPlayer tplayer, float x, float y, float z)
        {
            if (!tplayer.IsPlayerAdmin((int)TPlayer.TPlayer.Adminranks.Administrator))
            {
                tplayer.SendChatMessage("{FF0000} Dein Adminrang ist zu niedrig");
                return;
            }
            AltV.Net.Data.Position position = new AltV.Net.Data.Position(x, y, z+0.2f);
            tplayer.Position = position;
            tplayer.SendChatMessage("{04B404}Du hast dich zu den Koordinaten teleportiert!");
            return;
        }


        [Command("fraktionsinfo")]
        public void CMD_fraktionsinfo(TPlayer.TPlayer tplayer)
        {
            tplayer.SendChatMessage($"Fraktion: {tplayer.Fraktion}  Rang: {tplayer.GetFRankName()}({tplayer.FRang})");
            return;
        }


        [Command("makeleader")]
        public void CMD_makeleader(TPlayer.TPlayer tplayer, string playertarget, int frak)
        {
            if (!tplayer.IsPlayerAdmin((int)TPlayer.TPlayer.Adminranks.Administrator)){
                tplayer.SendChatMessage("{FF0000}Dein Adminrang ist zu niedrig!");
                return;
            }

            TPlayer.TPlayer target = Utils.GetPlayerByName(playertarget);
            if(target == null)
            {
                tplayer.SendChatMessage("{FF0000}Der Spieler existiert nicht!");
                return;
            }
            if(frak < 0 || frak > TPlayer.TPlayer.Fraktionen.Length)
            {
                tplayer.SendChatMessage("{FF0000}FEHLER: ungültige Fraktion!");
                return;
            }
            tplayer.Fraktion = frak;
            tplayer.FRang = 6;
            tplayer.SendChatMessage($"Du hast {target.Name} zum Leader der Fraktion {TPlayer.TPlayer.Fraktionen[frak]} ernannt.");
            target.SendChatMessage($"Du wurdest von {tplayer.Name} zum Leader der Fraktion {TPlayer.TPlayer.Fraktionen[frak]} ernannt.");
        }

        [Command("invite")]
        public void CMD_invite(TPlayer.TPlayer tplayer, string playertarget)
        {
            TPlayer.TPlayer target = Utils.GetPlayerByName(playertarget);
            //Abfrage: Antragsteller Leader?
            if (!tplayer.IsPlayerFrakLeader())
            {
                tplayer.SendChatMessage("{FF0000} Du bist kein Leader einer Fraktion.");
                return;
            }

            //Abfrage: eingabe vorhanden?
            if (target == null)
            {
                tplayer.SendChatMessage("{FF0000}Der Spieler existiert nicht!");
                return;
            }
            //Abfrage: Antragsteller in Frak?
            if(tplayer.Fraktion == 0)
            {
                tplayer.SendChatMessage("Du bist in keiner Fraktion!");
                return;
            }
            if(target.Fraktion != 0)
            {
                tplayer.SendChatMessage("{FF0000}Der Spieler ist bereits in einer Fraktion!");
                return;
            }
            else
            {
                //Invite!
                target.Fraktion = tplayer.Fraktion;
                target.FRang = 1;
                tplayer.SendChatMessage($"Du hast {target.Name} in deine Fraktion {TPlayer.TPlayer.Fraktionen[tplayer.Fraktion]} eingeladen.");
                target.SendChatMessage($"{tplayer.Name} hat dich in die Fraktion {TPlayer.TPlayer.Fraktionen[tplayer.Fraktion]} eingeladen!");
                return;
            }
        }

        //CMD:givegun
        [Command("givegun")]
        public void CMD_givegun(TPlayer.TPlayer tplayer)
        {
            if (!tplayer.IsPlayerAdmin((int)TPlayer.TPlayer.Adminranks.Moderator))
            {
                string msg = "{FF0000} Dein Adminrang ist zu niedrig!";
                tplayer.SendChatMessage(msg); 
                return;
            }

            tplayer.GiveWeapon(AltV.Net.Enums.WeaponModel.Pistol, 500, true);
            tplayer.SendChatMessage("{04B404}Du hast dir eine Pistole gegeben!");
            return;
        }

        //CMD:SAVE
        [Command("save", greedyArg:true)]
        public void CMD_save(TPlayer.TPlayer tplayer, string position)
        {
            if (!tplayer.IsPlayerAdmin((int)TPlayer.TPlayer.Adminranks.Moderator))
            {
                Utils.SendNotification(tplayer, "error", "Dein Adminrang ist dafür zu niedrig!");
                return;
            }

            string status = (tplayer.IsInVehicle) ? "Im Fahrzeug" : "Zu Fuß";
            Vector3 pos = (tplayer.IsInVehicle) ? tplayer.Vehicle.Position : tplayer.Position;
            Vector3 rot = (tplayer.IsInVehicle) ? tplayer.Vehicle.Rotation : tplayer.Rotation;

            string msg =
                $"{status} -> {position} : {pos.X.ToString(new CultureInfo("en-US")):N3},{pos.Y.ToString(new CultureInfo("en-US")):N3},{pos.Z.ToString(new CultureInfo("en-US")):N3},{rot.X.ToString(new CultureInfo("en-US")):N3},{rot.Y.ToString(new CultureInfo("en-US")):N3},{rot.Z.ToString(new CultureInfo("en-US")):N3}";
            tplayer.SendChatMessage(msg);
            
            using(StreamWriter file =  new StreamWriter(@"./savedpositions.txt",true)) 
            {
                file.WriteLine(msg);
            }

        }

        [Command("sethp")]
        public void CMD_sethp(TPlayer.TPlayer tplayer,int bar,float wert)
        {
            tplayer.Emit("setPB",bar,wert);
            tplayer.Health = (ushort)(100 + (ushort)wert);
            if (wert <= 0)
            {
                tplayer.Spawn(tplayer.Position, 0);
                Utils.SendNotification(tplayer, "info", $"Bar: {bar}  Wert: {wert}");
                return;
            }
            Utils.SendNotification(tplayer,"info", $"Bar: {bar}  Wert: {wert}");
            return;
        }


        [Command("reviveme")]
        public void CMD_reviveme(TPlayer.TPlayer tplayer)
        {
            tplayer.Spawn(tplayer.Position, 0);
            Utils.SendNotification(tplayer, "info", "Du hast dich selbst reanimiert!");
            return;
        }

    }
}
