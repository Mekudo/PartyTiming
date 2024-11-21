using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Wbsc
{
    internal class DBConnection
    {
        static string DBconnect = "server=localhost;user=root;password=Qwerty12;database=auth";
        static public MySqlDataAdapter msDataAdapter;
        static MySqlConnection myconnect;
        static public MySqlCommand msCommand;

        public static bool ConnectionDB()
        {
            try
            {
                myconnect = new MySqlConnection(DBconnect);
                myconnect.Open();
                msCommand = new MySqlCommand();
                msCommand.Connection = myconnect;
                msDataAdapter = new MySqlDataAdapter(msCommand);
                return true;
            }
            catch
            {
                MessageBox.Show("Ошибка соединения с базой данных!","Ошибка!");
                return false;
            }
        }

        public static void CloseDb()
        {
            myconnect.Close();
        }

        public MySqlConnection GetConnection()
        {
            return myconnect;
        }
    }
}
