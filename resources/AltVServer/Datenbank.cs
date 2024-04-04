using AltV.Net;
using AltV.Net.Elements.Entities;
using AltVTutorial;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.Sig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AltVServer
{
    class Datenbank : Server
    {
        public static bool DBConnection = false;
        public static MySqlConnection dbcon;
        public String Host { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String Database { get; set; }
    

        public Datenbank()
        {
            this.Host = "";
            this.Username = "";
            this.Password = "";
            this.Database = "";
        }

        public static String GetConnectionString()
        {
            Datenbank sql = new Datenbank();
            String SQLConnection = $"SERVER={sql.Host}; DATABASE={sql.Database}; UID={sql.Username}; Password={sql.Password}";
            return SQLConnection;
        }

        public static void InitConnection()
        {
            String SQLConnection = GetConnectionString();
            dbcon = new MySqlConnection(SQLConnection);
            try
            {
                dbcon.Open();
                DBConnection = true;
                Alt.Log("MYSQL Verbindung erfolgreich aufgebaut!");
            }catch(Exception e)
            {
                Alt.Log("MYSQL Verbindung Fehlgeschlagen!");
                Alt.Log(e.ToString());
            }
        }
    
        public static bool IstderAccountexistent(string name)
        {
            MySqlCommand command = dbcon.CreateCommand();
            command.CommandText = "SELECT * FROM accounts WHERE name=@name LIMIT 1";
            command.Parameters.AddWithValue("@name", name);
            using(MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    return true;
                }
            }
            return false;
        }


        public static int CreateAccount(string username, string password)
        {
            string saltedPW = BCrypt.HashPassword(password, BCrypt.GenerateSalt());

            try
            {
                MySqlCommand command = dbcon.CreateCommand();
                command.CommandText = "INSERT INTO accounts (name,password) VALUES (@username, @password)";
                command.Parameters.AddWithValue("@password", saltedPW);
                command.Parameters.AddWithValue("@username", username);
                command.ExecuteNonQuery();
                return (int)command.LastInsertedId;
            }catch(Exception e)
            {
                Alt.Log("Fehler bei Accounterstellung: " + e.ToString());
                return -1;
            }
        }



        public static void loadAccount(TPlayer.TPlayer tplayer)
        {
            MySqlCommand command = dbcon.CreateCommand();
            command.CommandText = "SELECT * FROM accounts WHERE name = @name LIMIT 1";
            command.Parameters.AddWithValue("@name", tplayer.Name);
            
            using(MySqlDataReader rdr = command.ExecuteReader())
            {
                if (rdr.HasRows)
                {
                    rdr.Read();
                    tplayer.SpielerID = rdr.GetInt32("id");
                    tplayer.Adminlevel = rdr.GetInt16("adminlevel");
                    tplayer.Geld = rdr.GetInt16("geld");
                    tplayer.Fraktion = rdr.GetInt16("fraktion");
                    tplayer.FRang = rdr.GetInt16("frang");
                    tplayer.PayDay = rdr.GetInt16("payday");
                }
            }
        }



        public static void saveAccount(TPlayer.TPlayer tplayer)
        {
            MySqlCommand command = dbcon.CreateCommand();
            command.CommandText = "UPDATE accounts SET adminlevel=@adminlevel, geld=@geld, fraktion=@frak, frang=@frank, payday=@payday WHERE id = @id";
            command.Parameters.AddWithValue("@adminlevel", tplayer.Adminlevel);
            command.Parameters.AddWithValue("@geld", tplayer.Geld);
            command.Parameters.AddWithValue("@frak", tplayer.Fraktion);
            command.Parameters.AddWithValue("@frank", tplayer.FRang);
            command.Parameters.AddWithValue("@payday", tplayer.PayDay);
            command.Parameters.AddWithValue("id", tplayer.Id);
        }

        public static bool PasswordCheck(string name, string passwordinput)
        {
            string password = "";
            MySqlCommand command = dbcon.CreateCommand();
            command.CommandText = "SELECT password FROM accounts WHERE name=@name";
            command.Parameters.AddWithValue ("name", name);

            using(MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows) { 
                reader.Read();
                password = reader.GetString("password");
                }
            }


            if (BCrypt.CheckPassword(passwordinput, password)) return true;
            return false;

        }
    
    }
}
