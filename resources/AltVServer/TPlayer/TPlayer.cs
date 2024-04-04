using AltV.Net;
using AltV.Net.Elements.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace AltVServer.TPlayer
{
    public class TPlayer : Player
    {
        public static String[] Fraktionen = new String[3] {"Zivilist", "LSPD", "SAM AG" };
        public static String[] RangNamen = new String[7] {"Kein Rang","Praktikant", "Azubi", "Angestellter", "Abteilungsleiter", "Aubilder", "Chef"};

        public enum ProgressBars { Healthbar = 1, Hungerbar , Thirstbar };
        public enum Adminranks {Spieler, Clanmember, Moderator, Administrator, Super_Administrator, Management};

        public int SpielerID { get; set; }
        public String SpielerName { get; set; }
        public long Geld { get; set; }
        public int Adminlevel { get; set; }
        public int Fraktion { get; set; }
        public int FRang { get; set; }
        public bool Eingeloggt { get; set; }
        public int PayDay { get; set; }

        public TPlayer(ICore core, IntPtr nativePointer, ushort id) : base(core, nativePointer, id)
        {
            Geld = 5000;
            Adminlevel = 0;
            Eingeloggt = false;
            Fraktion = 0;
            FRang = 0;
            PayDay = 60;
        }


        public bool IsPlayerAdmin(int alevel)
        {
            return Adminlevel >= alevel;
        }

        public bool IsPlayerInFrak(int fID)
        {
            return Fraktion == fID;
        }

        public bool IsPlayerFrakLeader()
        {
           if(FRang == 6 || FRang == 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public String GetFrakName()
        {
            return Fraktionen[Fraktion];
        }


        public int GetFRank()
        {
            return FRang;
        }

        public String GetFRankName()
        {
            return RangNamen[FRang];
        }
    }
}
