using MySql.Data.MySqlClient;
using System.Xml;
using System.Data;

namespace ClickMashine
{
    class MySQLExeption : Exception
    {
        public MySQLExeption(string? message) : base(message)
        {

        }
    }
    public class MySQL
    {
        MySqlConnectionStringBuilder builder;
        public MySQL(string name)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("C:/ClickMashine/Settings/MySQL.xml");
            XmlElement? xRoot = xDoc.DocumentElement;
            XmlNode? tomNode = xRoot?.SelectSingleNode("MySQL[@name='"+name+"']");
            
            if(tomNode == null) {
                throw new MySQLExeption("Error load setting MySQL");
            }
            try
            {
                builder = new MySqlConnectionStringBuilder
                {
                    Server = tomNode.SelectSingleNode("server").InnerText.Trim(),
                    Port = uint.Parse(tomNode.SelectSingleNode("port").InnerText.Trim()),
                    Database = tomNode.SelectSingleNode("bd").InnerText.Trim(),
                    UserID = tomNode.SelectSingleNode("user").InnerText.Trim(),
                    Password = tomNode.SelectSingleNode("password").InnerText.Trim(),
                    Pooling = false
                };
            }
            catch (Exception ex)
            {
                throw new MySQLExeption(ex.Message);
            }
        }
        public DataTable GetDataTableSQL(string SQL)
        {
            lock (builder)
            {
                DataTable dt = new DataTable();
                using (var connection = new MySqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    MySqlCommand sqlCom = new MySqlCommand(SQL, connection);
                    sqlCom.ExecuteNonQuery();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom);
                    dataAdapter.Fill(dt);
                    connection.Close();
                }
                return dt;
            }
        }
        public void SendSQL(string SQL)
        {
            lock (builder)
            {
                using (var connection = new MySqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    MySqlCommand sqlCom = new MySqlCommand(SQL, connection);
                    sqlCom.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        public long InsertSQL(string SQL)
        {
            lock (builder)
            {
                using (var connection = new MySqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    MySqlCommand sqlCom = new MySqlCommand(SQL, connection);
                    sqlCom.ExecuteNonQuery();
                    connection.Close();
                    return sqlCom.LastInsertedId;
                }
            }
        }
    }
}
