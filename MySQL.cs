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
        }
        public MySQL(string serverName, string userName, string dbName, string port, string password)
        {
            builder.UserID = userName;
            builder.Password = password;
            builder.Server = serverName;
            builder.Database = dbName;
            builder.Pooling = false;
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
        //private string serverName = "localhost"; // Адрес сервера (для локальной базы пишите "localhost")
        //private string userName = "root"; // Имя пользователя
        //private string dbName = "zabbix"; //Имя базы данных
        //private string port = "3306"; // Порт для подключения
        //private string password = ""; // Пароль для подключения

        private string serverName = "astf3-stp5"; // Адрес сервера (для локальной базы пишите "localhost")
        private string userName = "user"; // Имя пользователя
        private string password = "Ussd1801"; // Пароль для подключения
    }
}
