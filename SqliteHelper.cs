using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBServerMonitor
{
    public class SqliteHelper
    {
        private string db;
        private SQLiteConnection connection;

        public SqliteHelper(string db)
        {
            this.db = db;
        }

        private void createConnection()
        {
            connection = new SQLiteConnection("Data Source=" + db + "");
            connection.Open();
        }
        private void closeCurrentConnection()
        {
            connection.Close();
        }

        public void ExecuteSql(string sql)
        {
            createConnection();
            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = sql;
            command.ExecuteNonQuery();
            closeCurrentConnection();
        }

        public SQLiteDataReader ExecuteSqlReader(string sql)
        {
            createConnection();
            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = sql;
            var reader = command.ExecuteReader();
            closeCurrentConnection();
            return reader;
        }
    }
}
