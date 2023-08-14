using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMashine
{
    public class MySQL
    {
        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
        public MySQL(string dbName)
        {
            builder.UserID = userName;
            builder.Password = password;
            builder.Server = serverName;
            builder.Database = dbName;
            builder.Pooling = false;
            builder.Port = port;
        }
        public MySQL(string serverName, string userName, string dbName, uint port, string password)
        {
            builder.UserID = userName;
            builder.Password = password;
            builder.Server = serverName;
            builder.Database = dbName;
            builder.Pooling = false;
            builder.Port = port;
        }
        public DataTable GetDataTableSQL(string sql)
        {
            DataTable dt = new DataTable();
            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                connection.Open();
                MySqlCommand sqlCom = new MySqlCommand(sql, connection);
                sqlCom.ExecuteNonQuery();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom);
                dataAdapter.Fill(dt);
                connection.Close();
            }
            return dt;
        }
        public void SendSQL(string sql)
        {
            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                connection.Open();
                MySqlCommand sqlCom = new MySqlCommand(sql, connection);
                sqlCom.ExecuteNonQuery();
            }
        }
        private string serverName = "astf3-stp5"; // Адрес сервера (для локальной базы пишите "localhost")
        private string userName = "root"; // Имя пользователя
        private string dbName = "zabbix"; //Имя базы данных
        private uint port = 3307; // Порт для подключения
        private string password = "Fralkon"; // Пароль для подключения


        //private string serverName = "192.168.0.10"; // Адрес сервера (для локальной базы пишите "localhost")
        //private string userName = "root"; // Имя пользователя
        //private string password = "Fralkon"; // Пароль для подключения
    }
}
