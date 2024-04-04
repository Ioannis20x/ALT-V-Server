using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltVServer
{
    public class Events : IScript
    {
        [ScriptEvent(ScriptEventType.PlayerConnect)]
        public void OnPlayerConnect(TPlayer.TPlayer tplayer, string reason)
        {
            Alt.Log($"Spieler {tplayer.Name} betritt den Server.");
        }

        [ScriptEvent(ScriptEventType.PlayerDisconnect)]
        public void OnPlayerDiscornnect(TPlayer.TPlayer tplayer, string reason)
        {
            Alt.Log($"Spieler {tplayer.Name} hat den Server verlassen. Grund: {reason}");
            Utils.adminLog($"{tplayer.Name} hat den Server verlassen!", "SERVER");
        }

        [ClientEvent("Event.Register")]
        public void OnPlayerRegister(TPlayer.TPlayer tplayer, String name, String password)
        {
            if (!Datenbank.IstderAccountexistent(name))
            {
                if(!tplayer.Eingeloggt && name.Length > 3 && password.Length > 5)
                {
                    Datenbank.CreateAccount(name, password);
                    tplayer.Spawn(new AltV.Net.Data.Position(-425, 1120, 325), 0);
                    tplayer.Model = (uint)PedModel.Agent14;
                    tplayer.Eingeloggt = true;
                    tplayer.Emit("CloseLoginHud");
                    tplayer.SendChatMessage("{00c900}Registrierung erfolgreich.");
                    Utils.adminLog($"{tplayer.Name} hat sich registriert!", "SERVER");
                }
            }
            else
            {
                tplayer.Emit("SendErrorMessage", "Es existiert bereits ein Spieler mit diesem Namen!");
            }
        }



        [ClientEvent("Event.Login")]
        public void OnPlayerLogin(TPlayer.TPlayer tplayer, String name, String password)
        {
            if (Datenbank.IstderAccountexistent(name))
            {
                if(!tplayer.Eingeloggt && name.Length > 3 && password.Length > 5)
                {
                    if(Datenbank.PasswordCheck(name, password))
                    {
                        tplayer.SpielerName = name;
                        Datenbank.loadAccount(tplayer);
                        tplayer.Spawn(new AltV.Net.Data.Position(-425, 1120, 325), 0);
                        tplayer.Model = (uint)PedModel.Agent14;
                        tplayer.Eingeloggt = true;
                        tplayer.Emit("CloseLoginHud");
                        tplayer.SendChatMessage("{00c900}ERFOLG: Du hast dich erfolgreich eingeloggt!");
                        tplayer.SendChatMessage("{D60215}SERVER: {FFFFFF}Willkommen " + tplayer.Name);
                        Utils.adminLog($"{tplayer.Name} hat sich eingeloggt!", "SERVER");
                        tplayer.Health = 200;
                        tplayer.Emit("setPB", (int)TPlayer.TPlayer.ProgressBars.Healthbar, 1.0);
                        tplayer.Emit("setPB", (int)TPlayer.TPlayer.ProgressBars.Hungerbar, 0.5);
                        tplayer.Emit("setPB", (int)TPlayer.TPlayer.ProgressBars.Thirstbar, 0.3);

                    }
                    else
                    {
                        tplayer.Emit("SendErrorMessage", "Das eingegebene Passwort ist falsch!");
                    }
                }
                else
                {
                    tplayer.Emit("SendErrorMessage", "Der Spieler ist bereits eingeloggt!");
                }
            }
            else
            {
                tplayer.Emit("SendErrorMessage", "Ungültige Eingaben, bitte kontrolieren!");
            }
        }

    }
}
